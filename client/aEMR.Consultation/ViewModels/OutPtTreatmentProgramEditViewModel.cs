using aEMR.Common.BaseModel;
using aEMR.Controls;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Castle.Windsor;
using DataEntities;
using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

/*
 * 20220625 #001 DatTB: Thêm các thông tin: Mã số HSBA, Ngày đẩy cổng, Ngày tổng kết
 * 20220625 #002 DatTB: Thêm function lấy loại điều trị ngoại trú
 * 20220628 #003 DatTB: 
 * - Lấy ngày/giờ hiện tại cho Ngày bắt đầu
 * - Ngày tổng kết: đổi sang cho tự nhập và để trống
 * - Ngày đẩy cổng: check số nguyên
 * - Hiển thị Min/Max Ngày tổng kết, Ngày đẩy cổng.
 * 20220721 #004 DatTB: Validate dữ liệu trước khi gửi lên server
 * 20220811 #005 DatTB: Chỉnh sửa màn hình hồ sơ ĐTNT
 * + Chỉnh sửa text các trường
 * + Thêm trường Ngày dự kiến tổng kết
 * + Không cho phép chỉnh sửa loại hình, ngày dự kiến tổng kết khi đã lưu toa thuốc
 * + Truyền thêm biến xác định hồ sơ đã xác nhận mới chặn sửa
 * 20220812 #009 DatTB: Chỉnh sửa màn hình hồ sơ ĐTNT
 * + Thêm hàm check toa thuốc để kiểm tra chỉnh sửa loại hình ĐTNT
 * 20220919 #010 DatTB: Thêm điều kiện nếu hồ sơ cũ có loại hình đã bị "tạm ngưng" thì load lại loại hình
 */

namespace aEMR.Consultation.ViewModels
{
    [Export(typeof(IOutPtTreatmentProgramEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class OutPtTreatmentProgramEditViewModel : ViewModelBase, IOutPtTreatmentProgramEdit
    {
        [ImportingConstructor]
        public OutPtTreatmentProgramEditViewModel(IWindsorContainer aContainer, INavigationService aNavigation, ISalePosCaching aCaching)
        {
            ProgDateFromContent = Globals.GetViewModel<IMinHourDateControl>();
            ProgDateFromContent.DateTime = null;
            ProgDateToContent = Globals.GetViewModel<IMinHourDateControl>();
            ProgDateToContent.DateTime = null;
            //▼==== #005
            ProgDateFinalExpectContent = Globals.GetViewModel<IMinHourDateControl>();
            ProgDateFinalExpectContent.DateTime = null;
            //▲==== #005

            GetAllOutpatientTreatmentType(); // #002
        }
        #region Properties
        private IMinHourDateControl _ProgDateFromContent;
        public IMinHourDateControl ProgDateFromContent
        {
            get { return _ProgDateFromContent; }
            set
            {
                _ProgDateFromContent = value;
                NotifyOfPropertyChange(() => ProgDateFromContent);
            }
        }
        private IMinHourDateControl _ProgDateToContent;
        public IMinHourDateControl ProgDateToContent
        {
            get { return _ProgDateToContent; }
            set
            {
                _ProgDateToContent = value;
                NotifyOfPropertyChange(() => ProgDateToContent);
            }
        }

        private OutPtTreatmentProgram _CurrentOutPtTreatmentProgram;
        public OutPtTreatmentProgram CurrentOutPtTreatmentProgram
        {
            get
            {
                //▼==== #001
                if (_CurrentOutPtTreatmentProgram != null && _CurrentOutPtTreatmentProgram.OutPtTreatmentProgramID != 0)
                {
                    eProgDatePush = false;
                    eProgDateFinal = false;
                    //eOutPtTreatmentType = false;
                }
                //▲==== #001

                return _CurrentOutPtTreatmentProgram;
            }
            set
            {
                if (_CurrentOutPtTreatmentProgram != value)
                {
                    _CurrentOutPtTreatmentProgram = value;
                    ProgDateFromContent.DateTime = _CurrentOutPtTreatmentProgram.ProgDateFrom;
                    ProgDateToContent.DateTime = _CurrentOutPtTreatmentProgram.ProgDateTo;
                    //▼==== #005
                    ProgDateFinalExpectContent.DateTime = _CurrentOutPtTreatmentProgram.ProgDateFinalExpect;
                    //▲==== #005
                    //▼==== #003
                    //ProgDateFinalContent.DateTime = _CurrentOutPtTreatmentProgram.ProgDateFinal;
                    if (_CurrentOutPtTreatmentProgram.OutPtTreatmentProgramID == 0)
                    {
                        ProgDateFromContent.DateTime = Globals.GetCurServerDateTime();
                    }
                    //▲==== #003
                    NotifyOfPropertyChange(() => CurrentOutPtTreatmentProgram);
                }
            }
        }
        public bool IsSuccessed { get; set; } = false;
        #endregion
        #region Events
        public void SaveButton()
        {
            //▼==== #004
            if (CurrentOutPtTreatmentProgram == null)
            {
                return;
            }
            if (CurrentOutPtTreatmentProgram.OutpatientTreatmentTypeID <= 0)
            {
                Globals.ShowMessage(string.Format(eHCMSResources.Z0579_G1_VuiLongChon, eHCMSResources.T3814_G1_LoaiDTNT), eHCMSResources.G0442_G1_TBao);
                return;
            }
            CurrentOutPtTreatmentProgram.ProgDateFrom = (DateTime)ProgDateFromContent.DateTime;
            CurrentOutPtTreatmentProgram.ProgDateTo = ProgDateToContent.DateTime;
            CurrentOutPtTreatmentProgram.ProgDateFinalExpect = ProgDateFinalExpectContent.DateTime;//==== #005
            if (CurrentOutPtTreatmentProgram.ProgDateTo.HasValue &&
                CurrentOutPtTreatmentProgram.ProgDateTo.Value <= CurrentOutPtTreatmentProgram.ProgDateFrom)
            {
                Globals.ShowMessage(eHCMSResources.Z2956_G1_Msg, eHCMSResources.G0442_G1_TBao);
                return;
            }
            //if (CurrentOutPtTreatmentProgram.ProgDatePush == 0)
            //{
            //    Globals.ShowMessage(string.Format(eHCMSResources.Z0580_G1_VuiLongNhap, eHCMSResources.T3812_G1_NgDayCongHSDA), eHCMSResources.G0442_G1_TBao);
            //    return;
            //}
            //if (CurrentOutPtTreatmentProgram.ProgDatePush < SelectedOutpatientTreatmentType.MinNumOfDayTreatment)
            //{
            //    Globals.ShowMessage(string.Format(eHCMSResources.Z3252_G1_0NhoHon_LH, eHCMSResources.T3812_G1_NgDayCongHSDA, SelectedOutpatientTreatmentType.MinNumOfDayTreatment, eHCMSResources.Z3254_G1_LH_KHTH), eHCMSResources.G0442_G1_TBao);
            //    return;
            //}
            //if (CurrentOutPtTreatmentProgram.ProgDatePush > SelectedOutpatientTreatmentType.MaxNumOfDayTreatment)
            //{
            //    Globals.ShowMessage(string.Format(eHCMSResources.Z3253_G1_0LonHon_LH, eHCMSResources.T3812_G1_NgDayCongHSDA, SelectedOutpatientTreatmentType.MaxNumOfDayTreatment, eHCMSResources.Z3254_G1_LH_KHTH), eHCMSResources.G0442_G1_TBao);
            //    return;
            //}
            if (CurrentOutPtTreatmentProgram.ProgDateFinal == 0)
            {
                Globals.ShowMessage(string.Format(eHCMSResources.Z0580_G1_VuiLongNhap, eHCMSResources.T3813_G1_NgTongKetHSBA), eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (CurrentOutPtTreatmentProgram.ProgDateFinal < SelectedOutpatientTreatmentType.MinNumOfDayMedicalRecord)
            {
                Globals.ShowMessage(string.Format(eHCMSResources.Z3252_G1_0NhoHon_LH, eHCMSResources.T3813_G1_NgTongKetHSBA, SelectedOutpatientTreatmentType.MinNumOfDayMedicalRecord, eHCMSResources.Z3254_G1_LH_KHTH), eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (CurrentOutPtTreatmentProgram.ProgDateFinal > SelectedOutpatientTreatmentType.MaxNumOfDayMedicalRecord)
            {
                Globals.ShowMessage(string.Format(eHCMSResources.Z3253_G1_0LonHon_LH, eHCMSResources.T3813_G1_NgTongKetHSBA, SelectedOutpatientTreatmentType.MaxNumOfDayMedicalRecord, eHCMSResources.Z3254_G1_LH_KHTH), eHCMSResources.G0442_G1_TBao);
                return;
            }
            //▲==== #004

            this.DlgShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                using (var CurrentFactory = new CommonUtilsServiceClient())
                {
                    var CurrentContract = CurrentFactory.ServiceInstance;
                    CurrentContract.BeginSaveOutPtTreatmentProgram(CurrentOutPtTreatmentProgram, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            CurrentContract.EndSaveOutPtTreatmentProgram(asyncResult);
                            IsSuccessed = true;
                            TryClose();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.G0442_G1_TBao);
                        }
                        finally
                        {
                            this.DlgHideBusyIndicator();
                        }
                    }), null);
                }
            });
            CurrentThread.Start();
        }
        #endregion
        
        //▼==== #001
        
        private bool _eProgDatePush = true;
        public bool eProgDatePush
        {
            get { return _eProgDatePush; }
            set
            {
                _eProgDatePush = value;
                NotifyOfPropertyChange(() => eProgDatePush);
            }
        }

        private bool _eProgDateFinal = false; // #005
        public bool eProgDateFinal
        {
            get { return _eProgDateFinal; }
            set
            {
                _eProgDateFinal = value;
                NotifyOfPropertyChange(() => eProgDateFinal);
            }
        }
        //▲==== #001

        //▼==== #002
        private List<OutpatientTreatmentType> _OutpatientTreatmentTypes;
        public List<OutpatientTreatmentType> OutpatientTreatmentTypes
        {
            get { return _OutpatientTreatmentTypes; }
            set
            {
                _OutpatientTreatmentTypes = value;

                var itemDefault = new OutpatientTreatmentType
                {
                    OutpatientTreatmentTypeID = 0,
                    OutpatientTreatmentName = "--Vui lòng chọn loại hình ĐTNT--"
                };

                OutpatientTreatmentTypes.Insert(0, itemDefault);
                NotifyOfPropertyChange(() => OutpatientTreatmentTypes);
            }
        }
        
        private bool _eOutPtTreatmentType = true;
        public bool eOutPtTreatmentType
        {
            get { return _eOutPtTreatmentType; }
            set
            {
                _eOutPtTreatmentType = value;
                NotifyOfPropertyChange(() => eOutPtTreatmentType);
            }
        }
        private void GetAllOutpatientTreatmentType()
        {
            this.DlgShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                using (var CurrentFactory = new CommonUtilsServiceClient())
                {
                    var CurrentContract = CurrentFactory.ServiceInstance;
                    CurrentContract.BeginGetAllOutpatientTreatmentType(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            OutpatientTreatmentTypes = CurrentContract.EndGetAllOutpatientTreatmentType(asyncResult);
                            //▼==== #010
                            if (OutpatientTreatmentTypes.Find(x => CurrentOutPtTreatmentProgram.OutpatientTreatmentTypeID == x.OutpatientTreatmentTypeID) == null)
                            {
                                CurrentOutPtTreatmentProgram.OutpatientTreatmentTypeID = 0;
                            }
                            //▲==== #010
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.G0442_G1_TBao);
                        }
                        finally
                        {
                            this.DlgHideBusyIndicator();
                        }
                    }), null);
                }
            });
            CurrentThread.Start();
        }
        //▲==== #002
        //▼==== #003
        public void tbIntNumberFilter_KeyUp(object sender, KeyEventArgs e)
        {
            string v = (sender as TextBox).Text;
            if (!string.IsNullOrEmpty(v))
            {
                int num = 0;
                int.TryParse(v, out num);

                ((System.Windows.Controls.TextBox)(sender)).Text = num.ToString();//show ra giao diện
            }
        }
        
        private OutpatientTreatmentType _SelectedOutpatientTreatmentType;
        public OutpatientTreatmentType SelectedOutpatientTreatmentType
        {
            get { return _SelectedOutpatientTreatmentType; }
            set
            {
                _SelectedOutpatientTreatmentType = value;
                //▼==== #005
                if (OPtTreTypeChangedFirst)
                {
                    if (_SelectedOutpatientTreatmentType != null && _SelectedOutpatientTreatmentType.OutpatientTreatmentTypeID > 0)
                    {
                        CurrentOutPtTreatmentProgram.ProgDateFinal = _SelectedOutpatientTreatmentType.MaxNumOfDayMedicalRecord;
                        ProgDateFinalExpectContent.DateTime = ProgDateFromContent.DateTime.Value.AddDays(CurrentOutPtTreatmentProgram.ProgDateFinal - 1);
                    }
                    else
                    {
                        CurrentOutPtTreatmentProgram.ProgDateFinal = 0;
                        ProgDateFinalExpectContent.DateTime = null;
                    }
                }
                else
                {
                    OPtTreTypeChangedFirst = true;
                }
                //▲==== #005
                NotifyOfPropertyChange(() => SelectedOutpatientTreatmentType);
            }
        }

        AxComboBox cboOutPtTreatmentType { get; set; }
        public void cboOutPtTreatmentType_SelectionChanged(object sender, RoutedEventArgs e)
        {
            cboOutPtTreatmentType = sender as AxComboBox;
            if (cboOutPtTreatmentType == null)
            {
                return;
            }
            SelectedOutpatientTreatmentType = cboOutPtTreatmentType.SelectedItemEx as OutpatientTreatmentType;
        }
        //▲==== #003

        //▼==== #005
        private IMinHourDateControl _ProgDateFinalExpectContent;
        public IMinHourDateControl ProgDateFinalExpectContent
        {
            get { return _ProgDateFinalExpectContent; }
            set
            {
                _ProgDateFinalExpectContent = value;
                NotifyOfPropertyChange(() => ProgDateFinalExpectContent);
            }
        }
        
        private bool _eProgDateFinalExpect = true;
        public bool eProgDateFinalExpect
        {
            get { return _eProgDateFinalExpect; }
            set
            {
                _eProgDateFinalExpect = value;
                NotifyOfPropertyChange(() => eProgDateFinalExpect);
            }
        }

        public bool IsOutPtTProSubmited { get; set; } = false;

        // Thêm biến tạm để bỏ qua sự kiện cboOutPtTreatmentType_SelectionChanged lần đầu (Tự chạy khi khởi tạo cửa sổ) => Không load được dữ liệu cũ
        public bool OPtTreTypeChangedFirst { get; set; } = false;
        //▲==== #005

        //▼==== #006
        private long _PtRegDetailID = 0;
        public long PtRegDetailID
        {
            get { return _PtRegDetailID; }
            set
            {
                _PtRegDetailID = value;
                if (_PtRegDetailID > 0)
                {
                    GetPrescriptionIssueHistory_ByPtRegDetailID(_PtRegDetailID);
                }
                NotifyOfPropertyChange(() => PtRegDetailID);
            }
        }

        private void GetPrescriptionIssueHistory_ByPtRegDetailID(long PtRegDetailID, long V_RegistrationType = 24001)
        {
            this.DlgShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                using (var CurrentFactory = new PatientRegistrationServiceClient())
                {
                    var CurrentContract = CurrentFactory.ServiceInstance;
                    CurrentContract.BeginGetPrescriptionIssueHistory_ByPtRegDetailID(PtRegDetailID, V_RegistrationType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var LstPrescriptionIssueHistory = CurrentContract.EndGetPrescriptionIssueHistory_ByPtRegDetailID(asyncResult);
                            if (LstPrescriptionIssueHistory != null && LstPrescriptionIssueHistory.Count > 0 && IsOutPtTProSubmited)
                            {
                                eProgDateFinalExpect = false;
                                eOutPtTreatmentType = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.G0442_G1_TBao);
                        }
                        finally
                        {
                            this.DlgHideBusyIndicator();
                        }
                    }), null);
                }
            });
            CurrentThread.Start();
        }
        //▲==== #006
    }
}