using aEMR.Common;
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using DataEntities;
using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
/*
* 20220811 #001 DatTB: Chỉnh sửa màn hình hồ sơ ĐTNT
* + Không cho phép chỉnh sửa loại hình, ngày dự kiến tổng kết khi đã lưu toa thuốc
* + Thêm ShowDialog_V5 sử dụng Dictionary để truyền thêm title cho Dialog
* + Truyền thêm biến xác định hồ sơ đã xác nhận mới chặn sửa
* 20220812 #002 DatTB: Chỉnh sửa màn hình hồ sơ ĐTNT
* + Thêm hàm check toa thuốc để kiểm tra chỉnh sửa loại hình ĐTNT
* 20220831 #003 BLQ: Bỏ không tắt màn hình để có thể chỉnh danh sách liệu trình
* 20220915 #004 BLQ: 
* + Thêm nút xóa đợt điều trị/ liệu trình
* + Chỉnh lại điều kiện không sửa thông tin đợt điều trị/ liệu trình khi xác nhận bảo hiểm
* + Chỉnh điều kiện khi chọn sửa hồ lỗi văng chương trình khi chọn chỉnh sửa hồ sơ
*/
namespace aEMR.Consultation.ViewModels
{
    [Export(typeof(IOutPtTreatmentProgram)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class OutPtTreatmentProgramViewModel : ViewModelBase, IOutPtTreatmentProgram
    {
        #region Properties
        public PatientRegistration CurrentRegistration { get; set; }

        //▼==== #002
        public long PtRegDetailID { get; set; } = 0;
        //▲==== #002

        private ObservableCollection<OutPtTreatmentProgram> _OutPtTreatmentProgramCollection;
        public ObservableCollection<OutPtTreatmentProgram> OutPtTreatmentProgramCollection
        {
            get
            {
                return _OutPtTreatmentProgramCollection;
            }
            set
            {
                if (_OutPtTreatmentProgramCollection != value)
                {
                    _OutPtTreatmentProgramCollection = value;
                    NotifyOfPropertyChange(() => OutPtTreatmentProgramCollection);
                }
            }
        }
        private ObservableCollection<PatientRegistration> _PatientRegistrationCollection;
        public ObservableCollection<PatientRegistration> PatientRegistrationCollection
        {
            get
            {
                return _PatientRegistrationCollection;
            }
            set
            {
                if (_PatientRegistrationCollection != value)
                {
                    _PatientRegistrationCollection = value;
                    NotifyOfPropertyChange(() => PatientRegistrationCollection);
                }
            }
        }
        private bool _ConfirmButtonEnable = false;
        public bool ConfirmButtonEnable
        {
            get
            {
                return _ConfirmButtonEnable;
            }
            set
            {
                if (_ConfirmButtonEnable != value)
                {
                    _ConfirmButtonEnable = value;
                    NotifyOfPropertyChange(() => ConfirmButtonEnable);
                }
            }
        }
        #endregion
        #region Events
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            GetOutPtTreatmentProgramCollection();
            GetRegistrationByOutPtTreatmentProgramID(CurrentRegistration.OutPtTreatmentProgramID.GetValueOrDefault(0));
        }
        public void AddNewButton()
        {
            if (Globals.LoggedUserAccount == null || Globals.LoggedUserAccount.StaffID.GetValueOrDefault(0) == 0)
            {
                return;
            }
            OutPtTreatmentProgram Item = new OutPtTreatmentProgram
            {
                PatientID = CurrentRegistration.Patient.PatientID,
                ProgDateFrom = CurrentRegistration.ExamDate.Date,
                DoctorStaffID = Globals.LoggedUserAccount.StaffID.Value,
                CreatorStaff = new Staff { StaffID = Globals.LoggedUserAccount.StaffID.Value }
            };
            IOutPtTreatmentProgramEdit DialogView = Globals.GetViewModel<IOutPtTreatmentProgramEdit>();
            DialogView.CurrentOutPtTreatmentProgram = Item;
            GlobalsNAV.ShowDialog_V3(DialogView);
            if (DialogView.IsSuccessed)
            {
                GetOutPtTreatmentProgramCollection();
            }
        }
        public void ConfirmButton()
        {
            if (OutPtTreatmentProgramCollection == null ||
                !OutPtTreatmentProgramCollection.Any(x => x.IsChecked))
            {
                return;
            }
            if (CurrentRegistration.OutPtTreatmentProgramID.GetValueOrDefault(0) > 0)
            {
                if (MessageBox.Show(eHCMSResources.Z2955_G1_Msg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    return;
                }
            }
            var OutPtTreatmentProgramID = OutPtTreatmentProgramCollection.First(x => x.IsChecked).OutPtTreatmentProgramID;
            long? OutpatientTreatmentTypeID = OutPtTreatmentProgramCollection.First(x => x.IsChecked).OutpatientTreatmentTypeID;
            bool IsChronic = OutPtTreatmentProgramCollection.First(x => x.IsChecked).OutpatientTreatmentType == null 
                ? false : OutPtTreatmentProgramCollection.First(x => x.IsChecked).OutpatientTreatmentType.IsChronic;
            this.DlgShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                using (var CurrentFactory = new CommonUtilsServiceClient())
                {
                    var CurrentContract = CurrentFactory.ServiceInstance;
                    CurrentContract.BeginUpdateTreatmentProgramIntoRegistration(CurrentRegistration.PtRegistrationID, PtRegDetailID, OutPtTreatmentProgramID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            CurrentContract.EndUpdateTreatmentProgramIntoRegistration(out int OutPrescriptionsAmount, asyncResult);
                            CurrentRegistration.OutPtTreatmentProgramID = OutPtTreatmentProgramID;
                            CurrentRegistration.OutpatientTreatmentTypeID = OutpatientTreatmentTypeID == 0 ? null : OutpatientTreatmentTypeID;
                            Globals.EventAggregator.Publish(new ConfirmOutPtTreatmentProgram { Result = true, IsChronic = IsChronic });
                            Globals.EventAggregator.Publish(new SaveOutPtTreatmentProgramItem { PrescriptionsAmount = OutPrescriptionsAmount });
                            //▼====: #003
                            //TryClose();
                            ConfirmButtonEnable = false;
                            GetRegistrationByOutPtTreatmentProgramID(OutPtTreatmentProgramCollection.Where(x => x.IsChecked).FirstOrDefault().OutPtTreatmentProgramID);
                            //▲====: #003
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
        public void EditCurrentItemButton(OutPtTreatmentProgram Item)
        {
            //▼==== #004
            if (Item == null || CurrentRegistration == null )
            //▲==== #004
            {
                return;
            }
            IOutPtTreatmentProgramEdit DialogView = Globals.GetViewModel<IOutPtTreatmentProgramEdit>();
            DialogView.CurrentOutPtTreatmentProgram = Item.DeepCopy();
            //▼==== #001
            //▼==== #004
            if (CurrentRegistration.OutPtTreatmentProgramID.GetValueOrDefault(0) > 0 
                && Item.OutPtTreatmentProgramID == CurrentRegistration.OutPtTreatmentProgramID.Value)
            //▲==== #004
            {
                DialogView.IsOutPtTProSubmited = true;
            }
            //▼==== #002
            if (PtRegDetailID > 0)
            {
                DialogView.PtRegDetailID = PtRegDetailID;
            }
            //▲==== #002
            GlobalsNAV.ShowDialog_V5(DialogView, null, null, new Dictionary<string, object> { ["Title"] = eHCMSResources.Z3271_G1_TTHoSoDTNT });
            //▲==== #001
            if (DialogView.IsSuccessed)
            {
                GetOutPtTreatmentProgramCollection();
            }
        }
        #endregion
        #region Methods
        private void GetOutPtTreatmentProgramCollection()
        {
            this.DlgShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                using (var CurrentFactory = new CommonUtilsServiceClient())
                {
                    var CurrentContract = CurrentFactory.ServiceInstance;
                    CurrentContract.BeginGetOutPtTreatmentProgramCollectionByPatientID(CurrentRegistration.Patient.PatientID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            OutPtTreatmentProgramCollection = CurrentContract.EndGetOutPtTreatmentProgramCollectionByPatientID(asyncResult).ToObservableCollection();
                            //▼==== #005
                            foreach (var item in OutPtTreatmentProgramCollection)
                            {
                                if (item.ProgDateTo != null)
                                {
                                    item.CanCheck = false;
                                }
                            }
                            //▲==== #005
                            if (CurrentRegistration.OutPtTreatmentProgramID.GetValueOrDefault(0) > 0 && OutPtTreatmentProgramCollection != null &&
                                OutPtTreatmentProgramCollection.Any(x => x.OutPtTreatmentProgramID == CurrentRegistration.OutPtTreatmentProgramID.Value))
                            {
                                OutPtTreatmentProgramCollection.First(x => x.OutPtTreatmentProgramID == CurrentRegistration.OutPtTreatmentProgramID.Value).IsChecked = true;
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
        #endregion
        public void hplEditOutPtTreatmentProgramItem_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                PatientRegistration patientRegistration = ObjectCopier.DeepCopy((selectedItem as PatientRegistration));
                //▼====: #004
                if (patientRegistration.ConfirmHIStaffID.GetValueOrDefault(0) > 0)
                {
                    Globals.ShowMessage("Đăng ký đã được xác nhận BHYT. Không thể chỉnh sửa", eHCMSResources.G0442_G1_TBao);
                    return;
                }
                //▲====: #004
                IOutPtTreatmentProgramItem dialogView = Globals.GetViewModel<IOutPtTreatmentProgramItem>();
                dialogView.CurrentRegistration = patientRegistration;
                dialogView.MinNumOfDayMedicine = OutPtTreatmentProgramCollection.Where(x => x.IsChecked).FirstOrDefault().OutpatientTreatmentType.MinNumOfDayMedicine;
                dialogView.MaxNumOfDayMedicine = OutPtTreatmentProgramCollection.Where(x => x.IsChecked).FirstOrDefault().OutpatientTreatmentType.MaxNumOfDayMedicine;
                dialogView.OutpatientTreatmentTypeID = OutPtTreatmentProgramCollection.Where(x => x.IsChecked).FirstOrDefault().OutpatientTreatmentType.OutpatientTreatmentTypeID;
                GlobalsNAV.ShowDialog_V5(dialogView, null, null, new Dictionary<string, object> { ["Title"] = "Thông tin đợt điều trị ngoại trú" });
                GetRegistrationByOutPtTreatmentProgramID(OutPtTreatmentProgramCollection.Where(x => x.IsChecked).FirstOrDefault().OutPtTreatmentProgramID);
            }
        }
        private void GetRegistrationByOutPtTreatmentProgramID(long OutPtTreatmentProgramID, bool? IsDischarePapers = false)
        {
            if(OutPtTreatmentProgramID <= 0)
            {
                ConfirmButtonEnable = true;
                return;
            }
            this.DlgShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                using (var CurrentFactory = new CommonUtilsServiceClient())
                {
                    var CurrentContract = CurrentFactory.ServiceInstance;
                    CurrentContract.BeginGetRegistrationByOutPtTreatmentProgramID(OutPtTreatmentProgramID, IsDischarePapers ?? false, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            PatientRegistrationCollection = CurrentContract.EndGetRegistrationByOutPtTreatmentProgramID(asyncResult).ToObservableCollection();
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
        public void IsCheck_Click(OutPtTreatmentProgram Item)
        {
            if (Item == null || CurrentRegistration == null || CurrentRegistration.OutPtTreatmentProgramID <= 0)
            {
                return;
            }
            GetRegistrationByOutPtTreatmentProgramID(Item.OutPtTreatmentProgramID);
        }
        public void DeleteCurrentItemButton(OutPtTreatmentProgram Item)
        {
            if (Item == null || CurrentRegistration == null || CurrentRegistration.OutPtTreatmentProgramID <= 0)
            {
                return;
            }

            string Reason = "";
            IErrorBold MessBox = Globals.GetViewModel<IErrorBold>();
            MessBox.FireOncloseEvent = true;
            MessBox.IsShowReason = true;
            MessBox.SetMessage("Nhập lý do", "Xác nhận xóa");
            GlobalsNAV.ShowDialog_V3(MessBox);
            if (!MessBox.IsAccept)
            {
                return;
            }
            Reason = MessBox.Reason;
        
            OutPtTreatmentProgram CurrentOutPtTreatmentProgram = Item.DeepCopy();
            CurrentOutPtTreatmentProgram.IsDeleted = true;
            CurrentOutPtTreatmentProgram.DeletedStaffID = Globals.LoggedUserAccount.Staff.StaffID;
            CurrentOutPtTreatmentProgram.DeletedReason = Reason;
            this.DlgShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                using (var CurrentFactory = new CommonUtilsServiceClient())
                {
                    var CurrentContract = CurrentFactory.ServiceInstance;
                    CurrentContract.BeginOutPtTreatmentProgramMarkDeleted(CurrentOutPtTreatmentProgram, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var result = CurrentContract.EndOutPtTreatmentProgramMarkDeleted(asyncResult);
                            if (result)
                            {
                                GetOutPtTreatmentProgramCollection();
                                MessageBox.Show("Đã hủy");
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
        //▼====: #004
        public void hplDeleteOutPtTreatmentProgramItem_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                IErrorBold MessBox = Globals.GetViewModel<IErrorBold>();
                MessBox.FireOncloseEvent = true;
                MessBox.SetMessage(eHCMSResources.Z2951_G1_Msg, "Xác nhận xóa");
                GlobalsNAV.ShowDialog_V3(MessBox);
                if (!MessBox.IsAccept)
                {
                    return;
                }

                PatientRegistration patientRegistration = ObjectCopier.DeepCopy((selectedItem as PatientRegistration));
                if (patientRegistration.ConfirmHIStaffID.GetValueOrDefault(0) > 0)
                {
                    Globals.ShowMessage("Đăng ký đã được xác nhận BHYT. Không thể xóa", eHCMSResources.G0442_G1_TBao);
                    return;
                }
                this.ShowBusyIndicator();
                var CurrentThread = new Thread(() =>
                {
                    using (var CurrentFactory = new CommonUtilsServiceClient())
                    {
                        var CurrentContract = CurrentFactory.ServiceInstance;
                        CurrentContract.BeginUpdateTreatmentProgramIntoRegistration(patientRegistration.PtRegistrationID, PtRegDetailID, null, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                CurrentContract.EndUpdateTreatmentProgramIntoRegistration(out int OutPrescriptionsAmount, asyncResult);
                                CurrentRegistration.OutPtTreatmentProgramID = null;
                                CurrentRegistration.OutpatientTreatmentTypeID = null;
                                Globals.EventAggregator.Publish(new DeleteOutPtTreatmentProgramItem { Result = true });
                                //GetOutPtTreatmentProgramCollection();
                                ConfirmButtonEnable = true;
                                GetRegistrationByOutPtTreatmentProgramID(OutPtTreatmentProgramCollection.Where(x => x.IsChecked).FirstOrDefault().OutPtTreatmentProgramID);
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.G0442_G1_TBao);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                               
                            }
                        }), null);
                    }
                });
                CurrentThread.Start();
            }
        }
        //▲====: #004
        public void grdOutPtTreatmentProgram_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            OutPtTreatmentProgram objRows = e.Row.DataContext as OutPtTreatmentProgram;
            if (objRows != null)
            {
                switch (objRows.CanDelete)
                {
                    case true:
                        e.Row.Background = new SolidColorBrush(Colors.White);
                        break;
                    default:
                        e.Row.Background = new SolidColorBrush(Color.FromRgb(186, 211, 255));
                        break;
                }
            }
        }
    }
}