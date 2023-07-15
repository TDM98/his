using eHCMSLanguage;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using DataEntities;
using System.Collections.ObjectModel;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using System.Threading;
using System;
using System.Windows.Controls;
using System.Windows;
using aEMR.Controls;
using aEMR.Common;
using aEMR.ServiceClient.Consultation_PCLs;
using Castle.Windsor;
using System.Linq;
using aEMR.Common.Collections;
using aEMR.Common.BaseModel;
using aEMR.CommonTasks;
using System.Collections.Generic;
/*
* 20180922 #001 TTM:   Chuyển lấy Lookup từ gọi về Service sang lấy từ cache trên client. Vì đã có lấy tất cả Lookup lúc đăng nhập rồi không cần phải
*                      gọi về Service tốn thời gian.
* 20181023 #002 TTM:   BM 0002173: Thay đổi cách lưu, cập nhật và lấy lên của tình trạng thể chất => tất cả đều dựa vào lần đăng ký.             
* 20190519 #003 TTM:   BM 0006857: Cho phép nhập tình trạng thể chất dấu hiệu sinh tồn trước khi bác sĩ chẩn đoán và ra toa (Thư ký y khoa sẽ nhập ở màn hình thông tin chung).
* 20210907 #004 TNHX:   Chỉnh lại kiểm tra giá trị tối đa cho DHST + cảnh báo
*/
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IcwPhysiscalExam)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class cwPhysiscalExamViewModel : ViewModelBase, IcwPhysiscalExam
    {
        private bool _isAddNew = true;
        public bool isAddNew
        {
            get { return _isAddNew; }
            set
            {
                if (_isAddNew != value)
                {
                    _isAddNew = value;
                    NotifyOfPropertyChange(() => isAddNew);
                }
            }
        }

        private bool _isEdit;
        public bool isEdit
        {
            get { return _isEdit; }
            set
            {
                if (_isEdit != value)
                {
                    _isEdit = value;
                    NotifyOfPropertyChange(() => isEdit);
                    isAddNew = !isEdit;
                }
            }
        }

        private bool _isYearSmoke = true;
        public bool isYearSmoke
        {
            get { return _isYearSmoke; }
            set
            {
                if (_isYearSmoke != value)
                {
                    _isYearSmoke = value;
                    NotifyOfPropertyChange(() => isYearSmoke);
                }
            }
        }

        private bool _isYearQuitSmoke = true;
        public bool isYearQuitSmoke
        {
            get { return _isYearQuitSmoke; }
            set
            {
                if (_isYearQuitSmoke != value)
                {
                    _isYearQuitSmoke = value;
                    NotifyOfPropertyChange(() => isYearQuitSmoke);
                }
            }
        }

        private bool _isSmokeNever = true;
        public bool isSmokeNever
        {
            get { return _isSmokeNever; }
            set
            {
                if (_isSmokeNever != value)
                {
                    _isSmokeNever = value;
                    NotifyOfPropertyChange(() => isSmokeNever);
                }
            }
        }

        private ObservableCollection<Lookup> _V_HealthClassCollection;
        public ObservableCollection<Lookup> V_HealthClassCollection
        {
            get
            {
                return _V_HealthClassCollection;
            }
            set
            {
                _V_HealthClassCollection = value;
                NotifyOfPropertyChange(() => V_HealthClassCollection);
            }
        }

        [ImportingConstructor]
        public cwPhysiscalExamViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            GetRefSmokeStatus();
            GetRefAlcoholDrinkingStatus();
            PtPhyExamItem = new PhysicalExamination();
            V_HealthClassCollection = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_HealthyClassification).ToObservableCollection();
        }

        #region Properties member

        private Visibility _IsVisibility;
        public Visibility IsVisibility
        {
            get
            {
                return _IsVisibility;
            }
            set
            {
                if (_IsVisibility != value)
                {
                    _IsVisibility = value;
                    NotifyOfPropertyChange(() => IsVisibility);
                    if (_IsVisibility == Visibility.Visible)
                    {
                        Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private long? _PatientID;
        public long? PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                if (_PatientID != value)
                {
                    _PatientID = value;
                    NotifyOfPropertyChange(() => PatientID);
                }
            }
        }

        private Visibility _Visibility;
        public Visibility Visibility
        {
            get
            {
                return _Visibility;
            }
            set
            {
                if (_Visibility != value)
                {
                    _Visibility = value;
                    NotifyOfPropertyChange(() => Visibility);
                }
            }
        }

        private PhysicalExamination _pPhyExamItem;
        public PhysicalExamination PtPhyExamItem
        {
            get
            {
                return _pPhyExamItem;
            }
            set
            {
                if (_pPhyExamItem != value)
                {
                    _pPhyExamItem = value;
                    NotifyOfPropertyChange(() => PtPhyExamItem);
                }
            }
        }

        private ObservableCollection<Lookup> _refSmokeStatus;
        public ObservableCollection<Lookup> refSmokeStatus
        {
            get
            {
                return _refSmokeStatus;
            }
            set
            {
                if (_refSmokeStatus != value)
                {
                    _refSmokeStatus = value;
                    NotifyOfPropertyChange(() => refSmokeStatus);
                }
            }
        }

        private ObservableCollection<Lookup> _refAlcoholDrinkingStatus;
        public ObservableCollection<Lookup> refAlcoholDrinkingStatus
        {
            get
            {
                return _refAlcoholDrinkingStatus;
            }
            set
            {
                if (_refAlcoholDrinkingStatus != value)
                {
                    _refAlcoholDrinkingStatus = value;
                    NotifyOfPropertyChange(() => refAlcoholDrinkingStatus);
                }
            }
        }
        #endregion

        private Staff GetStaffLogin()
        {
            return Globals.LoggedUserAccount.Staff;
        }
        WarningWithConfirmMsgBoxTask errorMessageBox = null;

        private bool CheckValidPhysiscalExam(object temp)
        {
            PhysicalExamination u = temp as PhysicalExamination;
            if (u == null)
            {
                return false;
            }
            //▼====: #004
            // kiểm tra chặn trước
            //▼====: #003
            string ErrorMessage = "";            
            if (u.Weight > GlobalsNAV.BLOCK_WEIGHT_TOP || u.Weight <= GlobalsNAV.BLOCK_WEIGHT_BOTTOM)
            {
                ErrorMessage += string.Format("- Giá trị \"Cân nặng\" ngoài khoảng cho phép {0} - {1}!", GlobalsNAV.BLOCK_WEIGHT_BOTTOM, GlobalsNAV.BLOCK_WEIGHT_TOP);
            }
            if (u.V_RegistrationType == (long)AllLookupValues.RegistrationType.NOI_TRU)
            {
                if (u.Pulse > GlobalsNAV.BLOCK_PULSE_TOP || u.Pulse < GlobalsNAV.BLOCK_PULSE_BOTTOM)
                {
                    ErrorMessage += string.Format("\n - Giá trị \"Nhịp tim\" ngoài khoảng cho phép {0} - {1}!", GlobalsNAV.BLOCK_PULSE_BOTTOM, GlobalsNAV.BLOCK_PULSE_TOP);
                }
                if (u.SystolicPressure > GlobalsNAV.BLOCK_UPPER_PRESSURE_TOP || u.SystolicPressure < GlobalsNAV.BLOCK_PRESSURE_BOTTOM)
                {
                    ErrorMessage += string.Format("\n - Giá trị \"Huyết áp trên\" ngoài khoảng cho phép {0} - {1}!", GlobalsNAV.BLOCK_PRESSURE_BOTTOM, GlobalsNAV.BLOCK_UPPER_PRESSURE_TOP);
                }
                if (u.DiastolicPressure > GlobalsNAV.BLOCK_LOWER_PRESSURE_TOP || u.DiastolicPressure < GlobalsNAV.BLOCK_PRESSURE_BOTTOM)
                {
                    ErrorMessage += string.Format("\n - Giá trị \"Huyết áp dưới\" ngoài khoảng cho phép {0} - {1}!", GlobalsNAV.BLOCK_PRESSURE_BOTTOM, GlobalsNAV.BLOCK_LOWER_PRESSURE_TOP);
                }
                if (u.Temperature > GlobalsNAV.BLOCK_TEMPERATURE_TOP || u.Temperature < GlobalsNAV.BLOCK_TEMPERATURE_BOTTOM)
                {
                    ErrorMessage += string.Format("\n - Giá trị \"Nhiệt độ\" ngoài khoảng cho phép {0} - {1}!", GlobalsNAV.BLOCK_TEMPERATURE_BOTTOM, GlobalsNAV.BLOCK_TEMPERATURE_TOP);
                }
                if (u.RespiratoryRate > GlobalsNAV.BLOCK_RESPIRATORY_RATE_TOP || u.RespiratoryRate < GlobalsNAV.BLOCK_RESPIRATORY_RATE_BOTTOM)
                {
                    ErrorMessage += string.Format("\n - Giá trị \"Nhịp thở\" ngoài khoảng cho phép {0} - {1}!", GlobalsNAV.BLOCK_RESPIRATORY_RATE_BOTTOM, GlobalsNAV.BLOCK_RESPIRATORY_RATE_TOP);
                }
                if (u.SpO2 > GlobalsNAV.BLOCK_SPO2_TOP || u.SpO2 < GlobalsNAV.BLOCK_SPO2_BOTTOM)
                {
                    ErrorMessage += string.Format("\n - Giá trị \"SpO2\" ngoài khoảng cho phép {0} - {1}!", GlobalsNAV.BLOCK_SPO2_BOTTOM, GlobalsNAV.BLOCK_SPO2_TOP);
                }
            }
            else
            {
                if (u.Pulse > GlobalsNAV.BLOCK_PULSE_TOP || u.Pulse <= GlobalsNAV.BLOCK_PULSE_BOTTOM)
                {
                    ErrorMessage += string.Format("\n - Giá trị \"Nhịp tim\" ngoài khoảng cho phép {0} - {1}!", GlobalsNAV.BLOCK_PULSE_BOTTOM, GlobalsNAV.BLOCK_PULSE_TOP);
                }
                if (u.SystolicPressure > GlobalsNAV.BLOCK_UPPER_PRESSURE_TOP || u.SystolicPressure <= GlobalsNAV.BLOCK_PRESSURE_BOTTOM)
                {
                    ErrorMessage += string.Format("\n - Giá trị \"Huyết áp trên\" ngoài khoảng cho phép {0} - {1}!", GlobalsNAV.BLOCK_PRESSURE_BOTTOM, GlobalsNAV.BLOCK_UPPER_PRESSURE_TOP);
                    
                }
                if (u.SystolicPressure.GetValueOrDefault() > 0)
                {
                    if (u.DiastolicPressure.GetValueOrDefault() > GlobalsNAV.BLOCK_LOWER_PRESSURE_TOP || u.DiastolicPressure.GetValueOrDefault() <= GlobalsNAV.BLOCK_PRESSURE_BOTTOM)
                    {
                        ErrorMessage += string.Format("\n - Giá trị \"Huyết áp dưới\" ngoài khoảng cho phép {0} - {1}!", GlobalsNAV.BLOCK_PRESSURE_BOTTOM, GlobalsNAV.BLOCK_LOWER_PRESSURE_TOP);
                    }
                }
                if (u.DiastolicPressure > GlobalsNAV.BLOCK_LOWER_PRESSURE_TOP || u.DiastolicPressure <= GlobalsNAV.BLOCK_PRESSURE_BOTTOM)
                {
                    ErrorMessage += string.Format("\n - Giá trị \"Huyết áp dưới\" ngoài khoảng cho phép {0} - {1}!", GlobalsNAV.BLOCK_PRESSURE_BOTTOM, GlobalsNAV.BLOCK_LOWER_PRESSURE_TOP);
                }
                if (u.DiastolicPressure.GetValueOrDefault() > 0)
                {
                    if (u.SystolicPressure.GetValueOrDefault() > GlobalsNAV.BLOCK_UPPER_PRESSURE_TOP || u.SystolicPressure.GetValueOrDefault() <= GlobalsNAV.BLOCK_PRESSURE_BOTTOM)
                    {
                        ErrorMessage += string.Format("\n - Giá trị \"Huyết áp trên\" ngoài khoảng cho phép {0} - {1}!", GlobalsNAV.BLOCK_PRESSURE_BOTTOM, GlobalsNAV.BLOCK_UPPER_PRESSURE_TOP);
                    }
                }
                if (u.Temperature > GlobalsNAV.BLOCK_TEMPERATURE_TOP || u.Temperature < GlobalsNAV.BLOCK_TEMPERATURE_BOTTOM)
                {
                    ErrorMessage += string.Format("\n - Giá trị \"Nhiệt độ\" ngoài khoảng cho phép {0} - {1}!", GlobalsNAV.BLOCK_TEMPERATURE_BOTTOM, GlobalsNAV.BLOCK_TEMPERATURE_TOP);
                }
                if (u.RespiratoryRate > GlobalsNAV.BLOCK_RESPIRATORY_RATE_TOP || u.RespiratoryRate < GlobalsNAV.BLOCK_RESPIRATORY_RATE_BOTTOM)
                {
                    ErrorMessage += string.Format("\n - Giá trị \"Nhịp thở\" ngoài khoảng cho phép {0} - {1}!", GlobalsNAV.BLOCK_RESPIRATORY_RATE_BOTTOM, GlobalsNAV.BLOCK_RESPIRATORY_RATE_TOP);
                }
                if (u.SpO2 > GlobalsNAV.BLOCK_SPO2_TOP || u.SpO2 <= GlobalsNAV.BLOCK_SPO2_BOTTOM)
                {
                    ErrorMessage += string.Format("\n - Giá trị \"SpO2\" ngoài khoảng cho phép {0} - {1}!", GlobalsNAV.BLOCK_SPO2_BOTTOM, GlobalsNAV.BLOCK_SPO2_TOP);
                }
            }
            
            if (ErrorMessage != "")
            {
                void onInitDlg(IErrorBold confDlg)
                {
                    confDlg.ErrorTitle = eHCMSResources.K1576_G1_CBao;
                    confDlg.isCheckBox = false;
                    confDlg.SetMessage(ErrorMessage, "");
                    confDlg.FireOncloseEvent = true;
                }
                GlobalsNAV.ShowDialog<IErrorBold>(onInitDlg);
                return false;

            }
            //▲====== #003
            // kiểm tra cảnh báo
            string WarningMessage = "";
            if ((u.Temperature <= GlobalsNAV.BLOCK_TEMPERATURE_TOP && u.Temperature > GlobalsNAV.WARNING_TEMPERATURE_TOP)
                || (u.Temperature < GlobalsNAV.WARNING_TEMPERATURE_BOTTOM && u.Temperature >= GlobalsNAV.BLOCK_TEMPERATURE_BOTTOM))
            {
                WarningMessage += string.Format(" - Giá trị \"Nhiệt độ\" ngoài khoảng {0} - {1}!", GlobalsNAV.WARNING_TEMPERATURE_BOTTOM, GlobalsNAV.WARNING_TEMPERATURE_TOP);
            }
            if ((u.Pulse < GlobalsNAV.BLOCK_PULSE_TOP && u.Pulse > GlobalsNAV.WARNING_PULSE_TOP)
                || (u.Pulse < GlobalsNAV.WARNING_PULSE_BOTTOM && u.Pulse > GlobalsNAV.BLOCK_PULSE_BOTTOM))
            {
                WarningMessage += string.Format("\n - Giá trị \"Nhịp tim\" ngoài khoảng {0} - {1}!", GlobalsNAV.WARNING_PULSE_BOTTOM, GlobalsNAV.WARNING_PULSE_TOP);
            }
            if ((u.SpO2 < GlobalsNAV.BLOCK_SPO2_TOP && u.SpO2 > GlobalsNAV.WARNING_SPO2_TOP)
                || (u.SpO2 < GlobalsNAV.WARNING_SPO2_BOTTOM && u.SpO2 > GlobalsNAV.BLOCK_SPO2_BOTTOM))
            {
                WarningMessage += string.Format("\n - Giá trị \"SpO2\" ngoài khoảng {0} - {1}!", GlobalsNAV.WARNING_SPO2_BOTTOM, GlobalsNAV.WARNING_SPO2_TOP);
            }
            if ((u.Weight < GlobalsNAV.BLOCK_WEIGHT_TOP && u.Weight > GlobalsNAV.WARNING_WEIGHT_TOP)
                || (u.Weight < GlobalsNAV.WARNING_WEIGHT_BOTTOM && u.Weight > GlobalsNAV.BLOCK_WEIGHT_BOTTOM))
            {
                WarningMessage += string.Format("\n - Giá trị \"Cân nặng\" ngoài khoảng {0} - {1}!", GlobalsNAV.WARNING_WEIGHT_BOTTOM, GlobalsNAV.WARNING_WEIGHT_TOP);
            }
            if ((u.SystolicPressure < GlobalsNAV.BLOCK_UPPER_PRESSURE_TOP && u.SystolicPressure > GlobalsNAV.WARNING_UPPER_PRESSURE_TOP)
                || (u.SystolicPressure < GlobalsNAV.WARNING_UPPER_PRESSURE_BOTTOM && u.SystolicPressure > GlobalsNAV.BLOCK_PRESSURE_BOTTOM))
            {
                WarningMessage += string.Format("\n - Giá trị \"Huyết áp trên\" ngoài khoảng {0} - {1}!", GlobalsNAV.WARNING_UPPER_PRESSURE_BOTTOM, GlobalsNAV.WARNING_UPPER_PRESSURE_TOP);
            }
            if ((u.DiastolicPressure < GlobalsNAV.BLOCK_LOWER_PRESSURE_TOP && u.DiastolicPressure > GlobalsNAV.WARNING_LOWER_PRESSURE_TOP)
                || (u.DiastolicPressure < GlobalsNAV.WARNING_LOWER_PRESSURE_BOTTOM && u.DiastolicPressure > GlobalsNAV.BLOCK_PRESSURE_BOTTOM))
            {
                WarningMessage += string.Format("\n - Giá trị \"Huyết áp dưới\" ngoài khoảng {0} - {1}!", GlobalsNAV.WARNING_LOWER_PRESSURE_BOTTOM, GlobalsNAV.WARNING_LOWER_PRESSURE_TOP);
            }
            if (u.RespiratoryRate > GlobalsNAV.WARNING_RESPIRATORY_RATE_TOP || u.RespiratoryRate < GlobalsNAV.WARNING_RESPIRATORY_RATE_BOTTOM)
            {
                WarningMessage += string.Format("\n - Giá trị \"Nhịp thở\" ngoài khoảng {0} - {1}!", GlobalsNAV.WARNING_RESPIRATORY_RATE_BOTTOM, GlobalsNAV.WARNING_RESPIRATORY_RATE_TOP);
            }
            if (WarningMessage != "")
            {
                IErrorBold MessBox = Globals.GetViewModel<IErrorBold>();
                MessBox.isCheckBox = true;
                MessBox.ErrorTitle = "Cảnh báo bệnh lý";
                MessBox.SetMessage(WarningMessage, eHCMSResources.Z0627_G1_TiepTucLuu);
                MessBox.FireOncloseEvent = true;
                GlobalsNAV.ShowDialog_V3(MessBox);
                if (MessBox.IsAccept)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            //▲====== #004
            return u.Validate();
        }

        private void GetRefSmokeStatus()
        {
            //▼====== #001
            refSmokeStatus = new ObservableCollection<Lookup>();
            foreach (var tmpLookup in Globals.AllLookupValueList)
            {
                if (tmpLookup.ObjectTypeID == (long)(LookupValues.SMOKE_STATUS))
                {
                    refSmokeStatus.Add(tmpLookup);
                }
            }
            NotifyOfPropertyChange(() => refSmokeStatus);
            //▲====== #001
        }

        private void GetRefAlcoholDrinkingStatus()
        {
            //▼====== #001
            refAlcoholDrinkingStatus = new ObservableCollection<Lookup>();
            foreach (var tmpLookup in Globals.AllLookupValueList)
            {
                if (tmpLookup.ObjectTypeID == (long)(LookupValues.ALCOHOL_DRINKING_STATUS))
                {
                    refAlcoholDrinkingStatus.Add(tmpLookup);
                }
            }
            NotifyOfPropertyChange(() => refAlcoholDrinkingStatus);
            //▲====== #001
        }

        private void UpdatePhyExam_V2(PhysicalExamination Exam, long? StaffID, long? ExamID)
        {
            this.DlgShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new SummaryServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdatePhysicalExamination_V2(Exam, StaffID, Globals.DispatchCallback((asyncResult) =>
                          {
                              try
                              {
                                  var results = contract.EndUpdatePhysicalExamination_V2(asyncResult);

                                  TryClose();
                                  PublishEvent();
                              }
                              catch (Exception ex)
                              {
                                  Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                              }
                              finally
                              {
                                  this.DlgHideBusyIndicator();
                              }

                          }), null);

                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }

            });
            t.Start();
        }

        //▲====== #002
        public void OKButton(object sender, RoutedEventArgs e)
        {
            //update physiscal here
            if (CheckValidPhysiscalExam(PtPhyExamItem))
            {
                PtPhyExamItem.CommonMedicalRecord = new CommonMedicalRecord();
                PtPhyExamItem.CommonMedicalRecord.PatientID = PatientID;
                //▼====== #002: Chuyển từ việc nếu như update thì thêm dòng mới => cập nhật trên dòng luôn
                //AddNewPhyExam();

                UpdatePhyExam_V2(PtPhyExamItem, GetStaffLogin().StaffID, PtPhyExamItem.PhyExamID);
                //▲====== #002
            }
        }

        public void CancelButton(object sender, RoutedEventArgs e)
        {
            TryClose();
        }

        //▼====== #002
        //private void AddNewPhyExam()
        //{
        //    //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
        //    var t = new Thread(() =>
        //    {
        //        IsLoading = true;

        //        using (var serviceFactory = new SummaryServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;
        //            contract.BeginAddNewPhysicalExamination(PtPhyExamItem,GetStaffLogin().StaffID, Globals.DispatchCallback((asyncResult) =>
        //            {

        //                try
        //                {
        //                    var results = contract.EndAddNewPhysicalExamination(asyncResult);
        //                    //Globals.ShowMessage(eHCMSResources.A1027_G1_Msg_InfoThemOK,"");
        //                    //phat ra su kien de load lai Physical sau cung
        //                    //Globals.EventAggregator.Publish(new CommonClosedPhysicalEvent { });
        //                    PublishEvent();
        //                    TryClose();
        //                }
        //                catch (Exception ex)
        //                {
        //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                }
        //                finally
        //                {
        //                    //Globals.IsBusy = false;
        //                    IsLoading = false;
        //                }

        //            }), null);

        //        }

        //    });
        //    t.Start();
        //}
        private void AddNewPhyExam_V2()
        {
            this.DlgShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new SummaryServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginAddNewPhysicalExamination_V2(PtPhyExamItem, GetStaffLogin().StaffID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndAddNewPhysicalExamination_V2(asyncResult);
                                PublishEvent();
                                TryClose();
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }
        //▲====== #002

        public void PublishEvent()
        {
            switch (Globals.LeftModuleActive)
            {
                case LeftModuleActive.KHAMBENH_THONGTINCHUNG:
                case LeftModuleActive.KHAMBENH_CHANDOAN_RATOA:
                    Globals.EventAggregator.Publish(new CommonClosedPhysicalForSummaryEvent());
                    break;
                //▼====== #003: bắn sự kiện đọc dữ liệu cho nội trú vì nội trú đã tách ra khỏi ngoại trú nên ModuleActive của nội trú đã thay đổi.
                case LeftModuleActive.KHAMBENH_THONGTINCHUNG_NOITRU:
                    Globals.EventAggregator.Publish(new CommonClosedPhysicalForSummaryEvent());
                    break;
                //▲====== #003
                case LeftModuleActive.KHAMBENH_CHANDOAN:
                    Globals.EventAggregator.Publish(new CommonClosedPhysicalForDiagnosisEvent());
                    break;
                case LeftModuleActive.KHAMBENH_CHANDOAN_NOITRU:
                    Globals.EventAggregator.Publish(new CommonClosedPhysicalForDiagnosis_InPtEvent());
                    break;
            }

        }

        public void OKSave(object sender, RoutedEventArgs e)
        {
            //add new here             
            if (CheckValidPhysiscalExam(PtPhyExamItem))
            {
                PtPhyExamItem.CommonMedicalRecord = new CommonMedicalRecord();
                PtPhyExamItem.CommonMedicalRecord.PatientID = PatientID;
                //▼====== #002
                //AddNewPhyExam();
                AddNewPhyExam_V2();
                //▲======#002
            }
        }

        public void checkMonthYear()
        {
            if (PtPhyExamItem.MonthHaveSmoked > 0)
            {
                if (isYearSmoke)
                {
                    PtPhyExamItem.MonthHaveSmoked = 12 * PtPhyExamItem.MonthHaveSmoked;
                }
            }
            if (PtPhyExamItem.MonthHaveSmoked > 0)
            {
                if (isYearSmoke)
                {
                    PtPhyExamItem.MonthHaveSmoked = 12 * PtPhyExamItem.MonthHaveSmoked;
                }
            }
        }

        public void listSmokeStatusSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((Lookup)((AxComboBox)(sender)).SelectedItemEx).LookupID == 5103)
            {
                isSmokeNever = false;

                PtPhyExamItem.SmokeCigarettePerDay = 0;
                PtPhyExamItem.MonthHaveSmoked = 0;
                PtPhyExamItem.MonthQuitSmoking = 0;
            }
            else
            {
                isSmokeNever = true;
            }
        }
    }
}
