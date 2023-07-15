using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using aEMR.DataContracts;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using aEMR.Common;
using System.Collections.Generic;
using eHCMSLanguage;
using aEMR.CommonTasks;
using aEMR.ServiceClient.Consultation_PCLs;
using Castle.Windsor;
using System.Linq;

/*
 * 20181005 #001 TBL:   BM 0000141: Load đăng kí cũ có toa thuốc quá hạn chỉnh sửa 7 ngày, sẽ hiện thông báo đến 2 lần
 * 20181023 #002 TTM:   BM 0002173: Thay đổi cách lưu, cập nhật và lấy lên của tình trạng thể chất => tất cả đều dựa vào lần đăng ký.
 * 20181121 #003 TTM:   BM 0005257: Tạo sự kiện bắn theo trường hợp Out standing task nội trú (chỉ bắn đúng người đúng việc không bắn linh tinh).
 * 20191010 #004 TTM:   BM 0017443: [Kiểm soát nhiễm khuẩn]: Bổ sung màn hình hội chẩn.
 */
namespace aEMR.ConsultantEPrescription.ViewModels
{
    //phai su dung nonshared vi loi khi menu nay roi click cau hinh he thong 
    //sau do click lai no thi ko hien thi menu nay
    //[Export(typeof(IConsultationModule)), PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(IConsultationModule))]
    public class ConsultationModuleViewModel : Conductor<object>, IConsultationModule
        , IHandle<ItemSelected<Patient>>
        , IHandle<ResultFound<Patient>>
        , IHandle<ResultNotFound<Patient>>
        , IHandle<RegDetailSelectedForConsultation>
        , IHandle<RegDetailFromOutStandingTask>
        , IHandle<RegistrationSelectedForConsultation_K1>
        , IHandle<RegistrationSelectedForConsultation_K2>
        , IHandle<InPatientRegistrationSelectedForConsultation>
    {

        private bool _isLoading = false;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
            }
        }

        [ImportingConstructor]
        public ConsultationModuleViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Globals.isConsultationStateEdit = true;
            Globals.EventAggregator.Subscribe(this);
            RegistrationDetails = new ObservableCollection<PatientRegistrationDetail>();
        }

        protected override void OnActivate()
        {
            MainContent = null;
            Globals.isConsultationStateEdit = true;
            base.OnActivate();
            //Khi khoi tao module thi load menu ben trai luon.
            var shell = Globals.GetViewModel<IHome>();
            //var leftMenu = Globals.GetViewModel<IConsultationLeftMenu>();
            var topMenu = Globals.GetViewModel<IConsultationTopMenu>();
            //shell.LeftMenu = leftMenu;
            shell.TopMenuItems = topMenu;
            //(shell as Conductor<object>).ActivateItem(leftMenu);
            (shell as Conductor<object>).ActivateItem(topMenu);
            shell.OutstandingTaskContent = null;
            shell.IsExpandOST = false;
            //Check Phòng và nhắc nhở chọn phòng
            //var homeVM = Globals.GetViewModel<IHome>();
            //homeVM.FindRegistrationCmdVisibility = true;
            //var loginVm = Globals.GetViewModel<ILogin>();
            //if (loginVm.DeptLocation == null) //Chưa chọn phòng => Yêu cầu Chọn phòng.
            //{
            //    var locationVm = Globals.GetViewModel<ISelectLocation>();
            //    locationVm.ItemActivated = Globals.GetViewModel<IPtDashboardSummary>();
            //    locationVm.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNgoaiTru;
            //    Globals.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNgoaiTru;
            //    Globals.ShowDialog(locationVm as Conductor<object>);
            //}
            //else
            //{

            //20181029 TTM:     Thay đổi ModuleActive do sự kiện initdata của ConsultationsSummary nằm trong Khambenh_Chandoan_Ratoa, mà hiện tại đang lấy ConsultationsSummary làm màn hình mặc định
            //                  khi vào module khám bệnh thay cho màn hình Thông tin chung.
            //Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_TONGQUAT;
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CHANDOAN_RATOA;
            //}
            //▼====== #003:     Trước đây tạo 1 out standing task xài chung tất cả màn hình => sai
            //                  Bây giờ màn hình nào xài thì tạo out standing task đúng loại ở màn hình đó.
            //var homeVm = Globals.GetViewModel<IHome>();
            //homeVm.OutstandingTaskContent = Globals.GetViewModel<IConsultationOutstandingTask>();
            //▲====== #003
            DynamicLoadConsutationView();
        }
        protected override void OnDeactivate(bool close)
        {
            ActiveItem = null;
            MainContent = null;
            base.OnDeactivate(close);
        }
        public void DynamicLoadConsutationView()
        {
            var PrescriptionVM = Globals.GetViewModel<IConsultationsSummary_V2>();
            PrescriptionVM.IsShowEditTinhTrangTheChat = false;
            MainContent = PrescriptionVM;
            ActivateItem(PrescriptionVM);
        }

        private object _mainContent;
        public object MainContent
        {
            get { return _mainContent; }
            set
            {
                _mainContent = value;
                NotifyOfPropertyChange(() => MainContent);
            }
        }

        private Patient _currentPatient;
        public Patient CurrentPatient
        {
            get { return _currentPatient; }
            set
            {
                _currentPatient = value;
                NotifyOfPropertyChange(() => CurrentPatient);
            }
        }

        private PatientRegistration _registration;
        public PatientRegistration Registration
        {
            get { return _registration; }
            set
            {
                _registration = value;
                NotifyOfPropertyChange(() => Registration);
            }
        }

        private PatientRegistrationDetail _registrationDetail;
        public PatientRegistrationDetail RegistrationDetail
        {
            get { return _registrationDetail; }
            set
            {
                _registrationDetail = value;
                NotifyOfPropertyChange(() => RegistrationDetail);
            }
        }

        private bool _registrationLoading = false;
        /// <summary>
        /// Dang trong qua trinh lay thong tin dang ky tu server.
        /// </summary>
        public bool RegistrationLoading
        {
            get
            {
                return _registrationLoading;
            }
            set
            {
                _registrationLoading = value;
                NotifyOfPropertyChange(() => RegistrationLoading);
            }
        }


        public void OpenRegDetailForConsultation(PatientRegistrationDetail PtRegDetail)
        {
            var home = Globals.GetViewModel<IHome>();

            var activeItem = home.ActiveContent;

            IConsultationModule consultModule = activeItem as IConsultationModule;
            if (consultModule != null)
            {
                if (Globals.PatientFindBy_ForConsultation == AllLookupValues.PatientFindBy.NGOAITRU)
                {
                    GetAllRegistrationDetails_ForGoToKhamBenh_Ext(PtRegDetail);
                }
            }
        }

        //KMx: Chọn 1 dịch vụ khám từ pop-up tìm kiếm đăng ký (ngoại trú).
        public void Handle(RegDetailSelectedForConsultation message)
        {
            if (Globals.PatientFindBy_ForConsultation == AllLookupValues.PatientFindBy.NGOAITRU && MainContent is IConsultationsSummary)
            {
                (MainContent as IConsultationsSummary).GetAllRegistrationDetails_ForGoToKhamBenh_Ext(message.Source);
                return;
            }
            else if (Globals.PatientFindBy_ForConsultation == AllLookupValues.PatientFindBy.NGOAITRU && MainContent is IConsultationsSummary_V2)
            {
                (MainContent as IConsultationsSummary_V2).GetAllRegistrationDetails_ForGoToKhamBenh_Ext(message.Source);
                return;
            }
            else if (Globals.PatientFindBy_ForConsultation == AllLookupValues.PatientFindBy.NGOAITRU && MainContent is IDiagnosysConsultation)
            {
                (MainContent as IDiagnosysConsultation).CallSetPatientInfoForConsultation(message.Source);
                return;
            }
            OpenRegDetailForConsultation(message.Source);
        }

        //KMx: Chọn 1 dịch vụ đã khám rồi OutStandingTask (ngoại trú).
        public void Handle(RegDetailFromOutStandingTask message)
        {
            if (Globals.PatientFindBy_ForConsultation == AllLookupValues.PatientFindBy.NGOAITRU && MainContent is IConsultationsSummary)
            {
                (MainContent as IConsultationsSummary).GetAllRegistrationDetails_ForGoToKhamBenh_Ext(message.Source);
                return;
            }
            else if (Globals.PatientFindBy_ForConsultation == AllLookupValues.PatientFindBy.NGOAITRU && MainContent is IConsultationsSummary_V2)
            {
                (MainContent as IConsultationsSummary_V2).GetAllRegistrationDetails_ForGoToKhamBenh_Ext(message.Source);
                return;
            }
            OpenRegDetailForConsultation(message.Source);
        }

        private IEnumerator<IResult> SetInPatientInfoForConsultation(PatientRegistration InPtRegistration, PatientRegistrationDetail aPatientRegistrationDetail)
        {
            yield return GenericCoRoutineTask.StartTask(InitPhyExamAction, InPtRegistration);
            //yield return GenericCoRoutineTask.StartTask(GetInPtServiceRecordForKhamBenhAction, InPtRegistration);
            ////Lấy thông tin đăng ký đầy đủ để lưu lại trong module Khám Bệnh
            //yield return GenericCoRoutineTask.StartTask(GetRegistrationInPtAction, InPtRegistration, aPatientRegistrationDetail);
        }

        //KMx: Chọn 1 đăng ký để khám bệnh (nội trú).
        public void Handle(RegistrationSelectedForConsultation_K1 message)
        {
            if (MainContent is IConsultationsSummary_InPt)
            {
                (MainContent as IConsultationsSummary_InPt).CallSetInPatientInfoForConsultation(message.Source, message.IsSearchByRegistrationDetails ? message.Source.LastestPatientRegistrationDetail : new PatientRegistrationDetail());
                return;
            }
            else if (MainContent is IConsultations_InPt)
            {
                (MainContent as IConsultations_InPt).CallSetInPatientInfoForConsultation(message.Source, message.IsSearchByRegistrationDetails ? message.Source.LastestPatientRegistrationDetail : new PatientRegistrationDetail());
                return;
            }
            //▼===== #004: Load lại bệnh nhân khi tìm kiếm ngoại trú
            if (MainContent is IDiagnosysConsultation)
            {
                (MainContent as IDiagnosysConsultation).CallSetInPatientInfoForConsultation(message.Source, message.IsSearchByRegistrationDetails ? message.Source.LastestPatientRegistrationDetail : new PatientRegistrationDetail());
                return;
            }
            //▲===== #004
            Coroutine.BeginExecute(SetInPatientInfoForConsultation(message.Source, message.IsSearchByRegistrationDetails ? message.Source.LastestPatientRegistrationDetail : new PatientRegistrationDetail()));
        }

        //KMx: Chọn 1 đăng ký để khám bệnh (nội trú).
        public void Handle(RegistrationSelectedForConsultation_K2 message)
        {
            if (MainContent is IConsultationsSummary_InPt)
            {
                (MainContent as IConsultationsSummary_InPt).CallSetInPatientInfoForConsultation(message.Source, message.IsSearchByRegistrationDetails ? message.Source.LastestPatientRegistrationDetail : new PatientRegistrationDetail());
                return;
            }
            else if (MainContent is IConsultations_InPt)
            {
                (MainContent as IConsultations_InPt).CallSetInPatientInfoForConsultation(message.Source, message.IsSearchByRegistrationDetails ? message.Source.LastestPatientRegistrationDetail : new PatientRegistrationDetail());
                return;
            }
            //▼===== #004: Load lại bệnh nhân khi tìm kiếm ngoại trú
            else if (MainContent is IDiagnosysConsultation)
            {
                (MainContent as IDiagnosysConsultation).CallSetInPatientInfoForConsultation(message.Source, message.IsSearchByRegistrationDetails ? message.Source.LastestPatientRegistrationDetail : new PatientRegistrationDetail());
                return;
            }
            //▲===== #004
            Coroutine.BeginExecute(SetInPatientInfoForConsultation(message.Source, message.IsSearchByRegistrationDetails ? message.Source.LastestPatientRegistrationDetail : new PatientRegistrationDetail()));
  }

        //▼====== #003: Chụp sự kiện này vì khi chuyển bệnh nhân bằng out standing task nội trú cần phải load mới thông tin bệnh nhân và set vào Globals.
        //              Trước đây chuyển màn hình sẽ tự làm ở OnActive, Hàm khởi tạo, OnLoadedView, nhưng bây giờ đứng tại màn hình đó chuyển đổi bệnh nhân nên cần phải có sự kiện để làm chuyện này
        public void Handle(InPatientRegistrationSelectedForConsultation message)
        {
            if (MainContent is IConsultationsSummary_InPt)
            {
                (MainContent as IConsultationsSummary_InPt).CallSetInPatientInfoForConsultation(message.Source, new PatientRegistrationDetail());
                return;
            }
            else if (MainContent is IConsultations_InPt)
            {
                (MainContent as IConsultations_InPt).CallSetInPatientInfoForConsultation(message.Source, new PatientRegistrationDetail());
                return;
            }
            Coroutine.BeginExecute(SetInPatientInfoForConsultation(message.Source, new PatientRegistrationDetail()));
        }
        //▲====== #003
        #region Kiểm tra có ? dv cụ thể của 1 đăng ký
        private ObservableCollection<PatientRegistrationDetail> _registrationDetails;
        public ObservableCollection<PatientRegistrationDetail> RegistrationDetails
        {
            get { return _registrationDetails; }
            private set
            {
                _registrationDetails = value;
                NotifyOfPropertyChange(() => RegistrationDetails);
            }
        }
        private void GetAllRegistrationDetails_ForGoToKhamBenh_Ext(PatientRegistrationDetail PtRegDetail)
        {
            //this.ShowBusyIndicator();
            //RegistrationDetails.Clear();
            //var t = new Thread(() =>
            //{
            //    try
            //    {
            //        using (var serviceFactory = new PatientRegistrationServiceClient())
            //        {
            //            var contract = serviceFactory.ServiceInstance;
            //            contract.BeginGetAllRegistrationDetails_ForGoToKhamBenh(PtRegDetail.PatientRegistration.PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
            //            {
            //                try
            //                {
            //                    var items = contract.EndGetAllRegistrationDetails_ForGoToKhamBenh(asyncResult);
            //                    if (items != null)
            //                    {
            //                        Globals.PatientAllDetails.allPrescriptionIssueHistory = new ObservableCollection<PrescriptionIssueHistory>();
            //                        foreach (var item in items)
            //                        {
            //                            if (item.prescriptionIssueHistory != null)
            //                            {
            //                                Globals.PatientAllDetails.allPrescriptionIssueHistory.Add(item.prescriptionIssueHistory);
            //                            }
            //                        }
            //                    }
            //                    CheckForDiKhamBenh(PtRegDetail.PatientRegistration, PtRegDetail);
            //                }
            //                catch (Exception ex)
            //                {
            //                    ClientLoggerHelper.LogError(ex.Message);
            //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
            //                }
            //                finally
            //                {
            //                    this.HideBusyIndicator();
            //                }
            //            }), null);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
            //        ClientLoggerHelper.LogError(ex.Message);
            //        this.HideBusyIndicator();
            //    }
            //});
            //t.Start();
        }


        public void GetPtRegDetailNewByPatientID(Patient patient)
        {
            var t = new Thread(() =>
            {
                IsLoading = true;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetPtRegDetailNewByPatientID(patient.PatientID,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                RegistrationDetail = contract.EndGetPtRegDetailNewByPatientID(asyncResult);
                                Registration = RegistrationDetail.PatientRegistration;
                                if (RegistrationDetail == null
                                    || Registration == null)
                                {
                                    MessageBox.Show(eHCMSResources.A0235_G1_Msg_InfoBNChuaCoDK);
                                }
                                else
                                {
                                    Coroutine.BeginExecute(InitialInfo(patient, Registration, RegistrationDetail));
                                }
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            finally
                            {
                                RegistrationLoading = false;
                                IsLoading = false;
                            }
                        }), null);

                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    //Globals.IsBusy = false;
                    RegistrationLoading = false;
                    IsLoading = false;
                }
            });
            t.Start();
        }

        private void CheckForDiKhamBenh(PatientRegistration ObjPR, PatientRegistrationDetail p)
        {
            if (ObjPR.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                if (p.RefMedicalServiceItem != null && p.RefMedicalServiceItem.IsAllowToPayAfter == 0 && p.PaidTime == null && ObjPR.PatientClassID.GetValueOrDefault(0) != (long)ePatientClassification.PayAfter && ObjPR.PatientClassID.GetValueOrDefault(0) != (long)ePatientClassification.CompanyHealthRecord)
                {
                    MessageBox.Show(string.Format(eHCMSResources.Z1470_G1_DV0ChuaTraTienKgTheKB, p.RefMedicalServiceItem.MedServiceName.Trim()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }
            }
            if (p.V_ExamRegStatus == (long)V_ExamRegStatus.mDangKyKham || p.V_ExamRegStatus == (long)V_ExamRegStatus.mChoKham || p.V_ExamRegStatus == (long)V_ExamRegStatus.mHoanTat || p.V_ExamRegStatus == (long)V_ExamRegStatus.mBatDauThucHien)
            {
                //PatientServiceRecordsGetForKhamBenh(ObjPR,p);
                //Globals.EventAggregator.Publish(new ItemSelected<PatientRegistration, PatientRegistrationDetail>() { Sender = ObjPR, Item = p });
                //SetPatientInfoForConsultation(ObjPR, p);
                //TryClose();
                Coroutine.BeginExecute(SetPatientInfoForConsultation(ObjPR, p));
            }
            else
            {
                switch (p.V_ExamRegStatus)
                {
                    //case (Int64)AllLookupValues.ExamRegStatus.HOAN_TAT:
                    //    {
                    //        MessageBox.Show(string.Format("{0}: ", eHCMSResources.K3421_G1_DV) + p.RefMedicalServiceItem.MedServiceName.Trim() + ", Đã Hoàn Tất!" + Environment.NewLine + "Không Thể Tiến Hành Khám Bệnh!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    //        break;
                    //    }
                    case (Int64)AllLookupValues.ExamRegStatus.KHONG_XAC_DINH:
                        {
                            MessageBox.Show(string.Format(eHCMSResources.Z1394_G1_KgTheTienHanhKB, p.RefMedicalServiceItem.MedServiceName.Trim()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            break;
                        }
                    case (Int64)AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI:
                        {
                            MessageBox.Show(string.Format(eHCMSResources.Z1395_G1_DVDaNgungVaTraLaiTien, p.RefMedicalServiceItem.MedServiceName.Trim()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            break;
                        }
                }
            }
            if (Globals.IsLockRegistration(ObjPR.RegLockFlag, "Khám bệnh và chỉ định cận lâm sàng"))
            {
                return;
            }
        }


        #endregion


        #region Sau Khi Chọn
        //Chọn 1 BN không có đăng ký để làm việc tại Module Khám Bệnh
        public void Handle(ItemSelected<Patient> message)
        {
            if (message == null)
            {
                return;
            }

            if (Globals.LeftModuleActive == LeftModuleActive.KHAMBENH_QUANLY_THONGTIN_BN)
            {
                return;
            }

            if (Globals.LeftModuleActive == LeftModuleActive.KHAMBENH_LSBENHAN)
            {
                Globals.isConsultationStateEdit = false;
                Globals.EventAggregator.Publish(new isConsultationStateEditEvent { isConsultationStateEdit = false });
                GetPtRegDetailNewByPatientID(message.Item);
                return;
            }

            Globals.isConsultationStateEdit = true;
            Globals.EventAggregator.Publish(new isConsultationStateEditEvent { isConsultationStateEdit = true });
            var home = Globals.GetViewModel<IHome>();

            var activeItem = home.ActiveContent;

            IConsultationModule consultModule = activeItem as IConsultationModule;
            if (consultModule != null)
            {
                //Xét Hệ Thống này có Bật Tính Năng Cho khám VIP Không?

                //Int32 VIP = Convert.ToInt32(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.RegistrationVIP].ToString());
                // Txd 25/05/2014 Replaced ConfigList
                Int32 VIP = Globals.ServerConfigSection.Hospitals.RegistrationVIP;

                if (VIP >= 1)
                {
                    GetRegistraionVIPByPatientID(message);
                }
                else
                {
                    MessageBox.Show(string.Format("Bệnh nhân {0} chưa đăng ký! Không thể tiến hành khám bệnh!", message.Item.FullName));
                }
            }

        }


        private IEnumerator<IResult> InitialInfo(Patient Patient, PatientRegistration Registration, PatientRegistrationDetail RegistrationDetail)
        {
            yield return GenericCoRoutineTask.StartTask(InitPhyExamAction, Registration);
            yield return GenericCoRoutineTask.StartTask(GetPtServiceRecordForKhamBenhAction, Registration, RegistrationDetail);
            //Lấy thông tin đăng ký đầy đủ để lưu lại trong module Khám Bệnh
            yield return GenericCoRoutineTask.StartTask(GetRegistrationAction, Registration, RegistrationDetail);
        }


        private void InitPhyExam(long patientID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new SummaryServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetLastPhyExamByPtID(patientID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            Globals.curPhysicalExamination = contract.EndGetLastPhyExamByPtID(asyncResult);

                            //Globals.SystolicPressure = PtPhyExamItem.SystolicPressure.ToString();
                            //Globals.DiastolicPressure = PtPhyExamItem.DiastolicPressure.ToString();
                            //Globals.Pulse = PtPhyExamItem.Pulse.ToString();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }

                    }), null);

                }

            });
            t.Start();
        }

        #region Tìm đăng ký VIP
        private void SearchRegistrations(Patient aPatient)
        {
            this.ShowBusyIndicator(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0601_G1_DangTimDK));
            SeachPtRegistrationCriteria mSearchCriteria = new SeachPtRegistrationCriteria();
            mSearchCriteria.FromDate = Globals.GetCurServerDateTime();
            mSearchCriteria.ToDate = mSearchCriteria.FromDate;
            mSearchCriteria.IsAdmission = null;
            mSearchCriteria.PatientCode = aPatient.PatientCode;
            mSearchCriteria.PatientNameString = aPatient.PatientCode;
            mSearchCriteria.KhamBenh = true;
            mSearchCriteria.IsAdmission = null;
            if (Globals.ObjRefDepartment != null)
            {
                mSearchCriteria.DeptID = Globals.ObjRefDepartment.DeptID;
            }
            if (Globals.DeptLocation != null)
            {
                mSearchCriteria.DeptLocationID = Globals.DeptLocation.DeptLocationID;
            }
            mSearchCriteria.IsHoanTat = null;
            mSearchCriteria.IsSearchByRegistrationDetails = true;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginSearchRegistrationsForDiag(mSearchCriteria, 0, 10, false, Globals.DispatchCallback((asyncResult) =>
                        {
                            int totalCount = 0;
                            IList<PatientRegistrationDetail> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndSearchRegistrationsForDiag(out totalCount, asyncResult);
                                bOK = true;
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                            finally
                            {
                                if (bOK)
                                {
                                    if (allItems == null || allItems.Count(x => x.V_ExamRegStatus != (long)V_ExamRegStatus.mNgungTraTienLai) == 0)
                                    {
                                        var mActiveItem = Globals.GetViewModel<IHome>().ActiveContent;
                                        if (!(mActiveItem is IConsultationModule && (mActiveItem as IConsultationModule).MainContent is IConsultingDiagnosys))
                                        {
                                            if (MessageBox.Show(string.Format("{0}: ", eHCMSResources.K1167_G1_BN) + aPatient.FullName + string.Format(", {0}!", eHCMSResources.A0253_G1_ChuaDKL) + Environment.NewLine + eHCMSResources.A0255_G1_Msg_ConfKBKhongCanDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                            {
                                                CreateNewRegistrationVIP(aPatient);
                                            }
                                        }
                                    }
                                    else if (allItems.Count(x => x.V_ExamRegStatus != (long)V_ExamRegStatus.mNgungTraTienLai) == 1)
                                    {
                                        Globals.EventAggregator.Publish(new RegDetailSelectedForConsultation() { Source = allItems.FirstOrDefault(x => x.V_ExamRegStatus != (long)V_ExamRegStatus.mNgungTraTienLai) });
                                    }
                                    else
                                    {
                                        Action<IFindRegistrationDetail> onInitDlg = delegate (IFindRegistrationDetail vm)
                                        {
                                            vm.IsSearchPtByNameChecked = true;
                                            vm.IsAllowSearchingPtByName_Visible = true;
                                            vm.SearchCriteria = mSearchCriteria;
                                            vm.IsPopup = true;
                                            vm.IsSearchGoToKhamBenh = true;
                                            vm.CloseAfterSelection = true;
                                            vm.CopyExistingPatientList(allItems, mSearchCriteria, totalCount);
                                        };
                                        //GlobalsNAV.ShowDialog<IFindRegistrationDetail>(onInitDlg);
                                        GlobalsNAV.ShowDialog<IFindRegistrationDetail>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSizeInput(1100, 500));
                                    }
                                }
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void GetRegistraionVIPByPatientID(ItemSelected<Patient> message)
        {
            if (message == null || message.Item == null)
            {
                return;
            }
            SearchRegistrations(message.Item);
            //var t = new Thread(() =>
            //{
            //    using (var serviceFactory = new PatientRegistrationServiceClient())
            //    {
            //        var contract = serviceFactory.ServiceInstance;
            //        contract.BeginGetRegistraionVIPByPatientID(message.Item.PatientID, Globals.DispatchCallback((asyncResult) =>
            //        {

            //            try
            //            {
            //                PatientRegistration reginfo = new PatientRegistration();
            //                reginfo = contract.EndGetRegistraionVIPByPatientID(asyncResult);

            //                if (reginfo != null && reginfo.PtRegistrationID > 0)/*Có ĐK VIP*/
            //                {
            //                    Globals.PatientAllDetails.PtRegistrationInfo = reginfo;

            //                    //Globals.EventAggregator.Publish(new ShowPatientInfo<Patient, PatientRegistration, PatientRegistrationDetail>() { Pt = message.Item, PtReg = reginfo, PtRegDetail = null });
            //                    PublishEventShowPatientInfo(message.Item, reginfo, null);

            //                    //Phát sự kiện báo đang khám VIP
            //                    Globals.EventAggregator.Publish(new EventKhamChoVIP<PatientRegistration>() { PtReg = reginfo });
            //                    //Phát sự kiện báo đang khám VIP

            //                }
            //                else
            //                {
            //                    var mActiveItem = Globals.GetViewModel<IHome>().ActiveContent;
            //                    if (!(mActiveItem is IConsultationModule && (mActiveItem as IConsultationModule).MainContent is IConsultingDiagnosys))
            //                    {
            //                        if (MessageBox.Show(string.Format("{0}: ", eHCMSResources.K1167_G1_BN) + message.Item.FullName + string.Format(", {0}!", eHCMSResources.A0253_G1_ChuaDKL) + Environment.NewLine + eHCMSResources.A0255_G1_Msg_ConfKBKhongCanDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            //                        {
            //                            CreateNewRegistrationVIP(message.Item);
            //                        }
            //                    }
            //                }

            //            }
            //            catch (Exception ex)
            //            {
            //                MessageBox.Show(ex.Message);
            //            }
            //            finally
            //            {
            //                //Globals.IsBusy = false;
            //            }

            //        }), null);

            //    }

            //});
            //t.Start();
        }
        #endregion
        #region Tạo Đăng ký VIP
        private void CreateNewRegistrationVIP(Patient aPatient)
        {
            if (aPatient == null)
            {
                return;
            }
            Coroutine.BeginExecute(DoSaveAndPayNewServices(aPatient), null, (o, e) =>
            {
                this.HideBusyIndicator();
            });
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            //var t = new Thread(() =>
            //{
            //    using (var serviceFactory = new PatientRegistrationServiceClient())
            //    {
            //        var contract = serviceFactory.ServiceInstance;

            //        PatientRegistration reginfo = new PatientRegistration();
            //        reginfo.RegTypeID = 1;
            //        reginfo.PatientID = message.Item.PatientID;
            //        reginfo.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
            //        reginfo.V_RegistrationStatus = (long)AllLookupValues.RegistrationStatus.OPENED;
            //        reginfo.PatientClassification = new PatientClassification();
            //        reginfo.PatientClassification.PatientClassID = 1;
            //        reginfo.PatientClassID = 1;
            //        //reginfo.V_RegistrationType = AllLookupValues.RegistrationType.DANGKY_VIP;

            //        contract.BeginCreateNewRegistrationVIP(reginfo, Globals.DispatchCallback((asyncResult) =>
            //        {

            //            try
            //            {
            //                long newRegistrationID = 0;
            //                contract.EndCreateNewRegistrationVIP(out newRegistrationID, asyncResult);

            //                if (newRegistrationID > 0)
            //                {
            //                    reginfo.PtRegistrationID = newRegistrationID;

            //                    Globals.PatientAllDetails.PtRegistrationInfo = reginfo;

            //                    //Globals.EventAggregator.Publish(new ShowPatientInfo<Patient, PatientRegistration, PatientRegistrationDetail>() { Pt = message.Item, PtReg = reginfo, PtRegDetail = null });
            //                    PublishEventShowPatientInfo(message.Item, reginfo, null);
            //                    //Phát sự kiện báo đang khám VIP
            //                    Globals.EventAggregator.Publish(new EventKhamChoVIP<PatientRegistration>() { PtReg = reginfo });
            //                    //Phát sự kiện báo đang khám VIP


            //                }
            //                else
            //                {
            //                    MessageBox.Show(eHCMSResources.Z0399_G1_TaoDKVIPKgThanhCOng, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            //                }

            //            }
            //            catch (Exception ex)
            //            {
            //                MessageBox.Show(ex.Message);
            //            }
            //            finally
            //            {
            //                //Globals.IsBusy = false;
            //            }

            //        }), null);

            //    }

            //});
            //t.Start();
        }
        private RefMedicalServiceType VIPServiceType { get; set; }
        private RefMedicalServiceItem VIPServiceItem { get; set; }
        public void GetVIPServiceTypes(GenericCoRoutineTask aGenTask)
        {
            if (VIPServiceType != null)
            {
                aGenTask.ActionComplete(true);
                return;
            }
            var mThread = new Thread(() =>
            {
                using (var mFactory = new PatientRegistrationServiceClient())
                {
                    var mContract = mFactory.ServiceInstance;
                    mContract.BeginGetMedicalServiceTypesByInOutType(new List<long> { (long)AllLookupValues.V_RefMedicalServiceInOutOthers.NGOAITRU, (long)AllLookupValues.V_RefMedicalServiceInOutOthers.NOITRU_NGOAITRU }, Globals.DispatchCallback(asyncResult =>
                    {
                        try
                        {
                            var mItemCollection = mContract.EndGetMedicalServiceTypesByInOutType(asyncResult);
                            if (mItemCollection != null && mItemCollection.Any(x => x.MedicalServiceTypeID == 1))
                            {
                                VIPServiceType = mItemCollection.FirstOrDefault(x => x.MedicalServiceTypeID == 1);
                            }
                            else
                            {
                                aGenTask.Error = new Exception(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                            }
                        }
                        catch (Exception ex)
                        {
                            aGenTask.Error = ex;
                        }
                        finally
                        {
                            if (aGenTask.Error != null)
                            {
                                this.HideBusyIndicator();
                            }
                            aGenTask.ActionComplete(aGenTask.Error == null);
                        }
                    }), null);
                }
            });
            mThread.Start();
        }
        public void GetDefaultServiceForVip(GenericCoRoutineTask aGenTask)
        {
            if (VIPServiceItem != null)
            {
                aGenTask.ActionComplete(true);
                return;
            }
            var mThread = new Thread(() =>
            {
                using (var mFactory = new PatientRegistrationServiceClient())
                {
                    var mContract = mFactory.ServiceInstance;
                    mContract.BeginGetAllMedicalServiceItemsByType(VIPServiceType.MedicalServiceTypeID, null, null, Globals.DispatchCallback(asyncResult =>
                    {
                        try
                        {
                            var mItemCollection = mContract.EndGetAllMedicalServiceItemsByType(asyncResult);
                            if (mItemCollection == null || mItemCollection.Any(x => x.MedServiceID == Globals.ServerConfigSection.CommonItems.DefaultVIPServiceItemID))
                            {
                                VIPServiceItem = mItemCollection.First(x => x.MedServiceID == Globals.ServerConfigSection.CommonItems.DefaultVIPServiceItemID);
                            }
                            else
                            {
                                aGenTask.Error = new Exception(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                            }
                        }
                        catch (Exception ex)
                        {
                            aGenTask.Error = ex;
                        }
                        finally
                        {
                            if (aGenTask.Error != null)
                            {
                                this.HideBusyIndicator();
                            }
                            aGenTask.ActionComplete(aGenTask.Error == null);
                        }
                    }), null);
                }
            });
            mThread.Start();
        }
        private void AddNewRegistrationDetails(PatientRegistration aRegistration, RefMedicalServiceItem aServiceItem, DeptLocation aDeptLocation, RefMedicalServiceType aServiceType)
        {
            var newRegistrationDetail = new PatientRegistrationDetail { RefMedicalServiceItem = aServiceItem };
            newRegistrationDetail.RefMedicalServiceItem.RefMedicalServiceType = aServiceType;
            newRegistrationDetail.DeptLocation = aDeptLocation != null && aDeptLocation.DeptLocationID > 0 ? aDeptLocation : null;
            newRegistrationDetail.EntityState = Service.Core.Common.EntityState.DETACHED;
            newRegistrationDetail.ExamRegStatus = AllLookupValues.ExamRegStatus.DANG_KY_KHAM;
            newRegistrationDetail.CreatedDate = Globals.GetCurServerDateTime();
            newRegistrationDetail.Qty = 1;
            newRegistrationDetail.HisID = aRegistration.HisID;
            newRegistrationDetail.RecordState = RecordState.ADDED;
            newRegistrationDetail.CanDelete = true;
            newRegistrationDetail.HIAllowedPrice = aServiceItem.HIAllowedPrice;
            newRegistrationDetail.InvoicePrice = aRegistration.HealthInsurance != null ? aServiceItem.HIPatientPrice : aServiceItem.NormalPrice;
            newRegistrationDetail.ReqDeptID = Globals.DeptLocation.DeptID; //20181217 TBL: Them khoa de khi in report Phieu chi dinh se co noi chi dinh
            newRegistrationDetail.GetItemPrice(aRegistration, Globals.GetCurServerDateTime(), false, Globals.ServerConfigSection.HealthInsurances.FullHIBenefitForConfirm, Globals.ServerConfigSection.HealthInsurances.HiPolicyMinSalary);
            newRegistrationDetail.GetItemTotalPrice();
            aRegistration.PatientRegistrationDetails.Add(newRegistrationDetail);
            CommonGlobals.CorrectRegistrationDetails(aRegistration);
        }
        private IEnumerator<IResult> DoSaveAndPayNewServices(Patient aPatient)
        {
            this.ShowBusyIndicator(eHCMSResources.Z1539_G1_DangLuuDK);
            yield return GenericCoRoutineTask.StartTask(GetVIPServiceTypes);
            yield return GenericCoRoutineTask.StartTask(GetDefaultServiceForVip);
            PatientRegistration CurrentRegistration = new PatientRegistration
            {
                Patient = aPatient,
                ReqFromDeptLocID = Globals.DeptLocation.DeptLocationID,
                PatientID = aPatient.PatientID,
                PatientClassID = (long)ePatientClassification.PayAfter,
                V_RegistrationStatus = (long)AllLookupValues.RegistrationStatus.OPENED,
                V_RegistrationType = AllLookupValues.RegistrationType.NGOAI_TRU,
                StaffID = Globals.LoggedUserAccount.StaffID,
                RefDepartment = Globals.DeptLocation.RefDepartment,
                PatientClassification = new PatientClassification { PatientClassID = (long)ePatientClassification.PayAfter }
            };
            if (!Globals.CheckChildrenUnder6YearOlds(aPatient))
            {
                yield break;
            }
            //Se dang ky ngay o cho nay.
            AddNewRegistrationDetails(CurrentRegistration, VIPServiceItem, Globals.DeptLocation, VIPServiceType);
            var newServiceList = CurrentRegistration.PatientRegistrationDetails.ToList();
            //Lưu xuống ở đây
            var regTask = new AddUpdateNewRegItemTask(CurrentRegistration, newServiceList, null, null, null, null);
            yield return regTask;
            if (!string.IsNullOrEmpty(regTask.ErrorMesage))
            {
                var message = new MessageWarningShowDialogTask(regTask.ErrorMesage, "", false);
                yield return message;
                yield break;
            }
            if (regTask.Error != null)
            {
                //Thông báo lỗi
                Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = new AxErrorEventArgs(regTask.Error) });
                yield break;
            }
            CurrentRegistration = regTask.CurRegistration;
            //Thành công
            if (CurrentRegistration != null && CurrentRegistration.PatientRegistrationDetails != null)
            {
                Globals.EventAggregator.Publish(new RegDetailSelectedForConsultation() { Source = CurrentRegistration.PatientRegistrationDetails.FirstOrDefault() });
            }
        }
        #endregion


        private IEnumerator<IResult> SetPatientInfoForConsultation(PatientRegistration PtRegistration, PatientRegistrationDetail PtRegDetail)
        {
            if (PtRegistration == null || PtRegDetail == null)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0400_G1_KgNhanDuocDLieuLamViec), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                yield break;
            }

            Globals.isConsultationStateEdit = true;
            Globals.EventAggregator.Publish(new isConsultationStateEditEvent { isConsultationStateEdit = true });

            RegistrationDetail = PtRegDetail;

            yield return GenericCoRoutineTask.StartTask(InitPhyExamAction, PtRegistration);

            yield return GenericCoRoutineTask.StartTask(GetPtServiceRecordForKhamBenhAction, PtRegistration, PtRegDetail);

            //Lấy thông tin đăng ký đầy đủ để lưu lại trong module Khám Bệnh
            yield return GenericCoRoutineTask.StartTask(GetRegistrationAction, PtRegistration, PtRegDetail);

            PublishEventGlobalsPSRLoad(false);

        }
        //▼====== #002: Đổi InitPhyExamAction lấy last Phy => lấy Phy theo Ptregistration
        //private void InitPhyExamAction(GenericCoRoutineTask genTask, object ObjPtRegistration)
        //{
        //    var t = new Thread(() =>
        //    {
        //        try
        //        {
        //            using (var serviceFactory = new SummaryServiceClient())
        //            {
        //                bool bContinue = true;

        //                var contract = serviceFactory.ServiceInstance;
        //                contract.BeginGetLastPhyExamByPtID(((PatientRegistration)ObjPtRegistration).Patient.PatientID, Globals.DispatchCallback((asyncResult) =>
        //                {
        //                    try
        //                    {
        //                        Globals.curPhysicalExamination = contract.EndGetLastPhyExamByPtID(asyncResult);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
        //                        ClientLoggerHelper.LogError(ex.Message);
        //                        bContinue = false;
        //                    }
        //                    finally
        //                    {
        //                        genTask.ActionComplete(bContinue);
        //                        this.HideBusyIndicator();
        //                    }

        //                }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
        //            ClientLoggerHelper.LogError(ex.Message);
        //            genTask.ActionComplete(false);
        //            this.HideBusyIndicator();
        //        }
        //    });
        //    t.Start();
        //}

        private void InitPhyExamAction(GenericCoRoutineTask genTask, object ObjPtRegistration)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new SummaryServiceClient())
                    {
                        bool bContinue = true;

                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetPhyExam_ByPtRegID(((PatientRegistration)ObjPtRegistration).PtRegistrationID, (long)((PatientRegistration)ObjPtRegistration).V_RegistrationType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                Globals.curPhysicalExamination = contract.EndGetPhyExam_ByPtRegID(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                ClientLoggerHelper.LogError(ex.Message);
                                bContinue = false;
                            }
                            finally
                            {
                                genTask.ActionComplete(bContinue);
                                this.HideBusyIndicator();
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        //▲====== #002

        //KMx: Giống hàm PatientServiceRecordsGetForKhamBenh, nhưng không có bắn event. Sau khi SetInfoPatient ở cha của nó thì mới bắn (22/05/2014 16:57).
        private void GetPtServiceRecordForKhamBenhAction(GenericCoRoutineTask genTask, object ObjPtRegistration, object ObjPtRegDetail)
        {
            //this.ShowBusyIndicator();
            //PatientServiceRecord psrSearch = new PatientServiceRecord();
            //psrSearch.PtRegistrationID = ((PatientRegistration)ObjPtRegistration).PtRegistrationID;
            //psrSearch.PtRegDetailID = ((PatientRegistrationDetail)ObjPtRegDetail).PtRegDetailID;
            //psrSearch.V_RegistrationType = ((PatientRegistration)ObjPtRegistration).V_RegistrationType;
            //var t = new Thread(() =>
            //{
            //    try
            //    {
            //        using (var serviceFactory = new CommonUtilsServiceClient())
            //        {
            //            bool bContinue = true;
            //            var contract = serviceFactory.ServiceInstance;
            //            contract.BeginPatientServiceRecordsGetForKhamBenh(psrSearch,
            //            Globals.DispatchCallback((asyncResult) =>
            //            {
            //                try
            //                {
            //                    var psr = contract.EndPatientServiceRecordsGetForKhamBenh(asyncResult);
            //                    Globals.curLstPatientServiceRecord = new ObservableCollection<PatientServiceRecord>(psr);
            //                }
            //                catch (Exception ex)
            //                {
            //                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
            //                    ClientLoggerHelper.LogError(ex.Message);
            //                    bContinue = false;
            //                }
            //                finally
            //                {
            //                    genTask.ActionComplete(bContinue);
            //                    this.HideBusyIndicator();
            //                }
            //            }), null);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
            //        ClientLoggerHelper.LogError(ex.Message);
            //        genTask.ActionComplete(false);
            //        this.HideBusyIndicator();
            //    }
            //});
            //t.Start();
        }

        private void GetRegistrationAction(GenericCoRoutineTask genTask, object ObjPtRegistration, object ObjPtRegDetail)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        bool bContinue = true;
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRegistration(((PatientRegistration)ObjPtRegistration).PtRegistrationID, (int)Globals.PatientFindBy_ForConsultation.Value,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                PatientRegistration regInfo = contract.EndGetRegistration(asyncResult);

                                _currentPatient = regInfo.Patient;
                                regInfo.Patient.CurrentHealthInsurance = regInfo.HealthInsurance;
                                Registration = regInfo;

                                Globals.SetInfoPatient(regInfo.Patient, regInfo, (PatientRegistrationDetail)ObjPtRegDetail);
                                PublishEventShowPatientInfo(regInfo.Patient, regInfo, (PatientRegistrationDetail)ObjPtRegDetail);

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                ClientLoggerHelper.LogError(ex.Message);
                                bContinue = false;
                            }
                            finally
                            {
                                genTask.ActionComplete(bContinue);
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }


        private void GetInPtServiceRecordForKhamBenhAction(GenericCoRoutineTask genTask, object ObjPtRegistration)
        {
            //this.ShowBusyIndicator();
            //PatientServiceRecord psrSearch = new PatientServiceRecord();
            //psrSearch.PtRegistrationID = ((PatientRegistration)ObjPtRegistration).PtRegistrationID;
            //psrSearch.V_RegistrationType = ((PatientRegistration)ObjPtRegistration).V_RegistrationType;
            //var t = new Thread(() =>
            //{
            //    try
            //    {
            //        using (var serviceFactory = new CommonUtilsServiceClient())
            //        {
            //            bool bContinue = true;
            //            var contract = serviceFactory.ServiceInstance;
            //            contract.BeginPatientServiceRecordsGetForKhamBenh_InPt(psrSearch,
            //            Globals.DispatchCallback((asyncResult) =>
            //            {
            //                try
            //                {
            //                    var psr = contract.EndPatientServiceRecordsGetForKhamBenh_InPt(asyncResult);
            //                    Globals.curLstPatientServiceRecord = new ObservableCollection<PatientServiceRecord>(psr);
            //                }
            //                catch (Exception ex)
            //                {
            //                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
            //                    ClientLoggerHelper.LogError(ex.Message);
            //                    bContinue = false;
            //                }
            //                finally
            //                {
            //                    genTask.ActionComplete(bContinue);
            //                    this.HideBusyIndicator();
            //                }
            //            }), null);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
            //        ClientLoggerHelper.LogError(ex.Message);
            //        genTask.ActionComplete(false);
            //        this.HideBusyIndicator();
            //    }
            //});
            //t.Start();
        }

        private void GetRegistrationInPtAction(GenericCoRoutineTask genTask, object ObjPtRegistration, object aPatientRegistrationDetail)
        {
            //this.ShowBusyIndicator();
            //LoadRegistrationSwitch LoadRegisSwitch = new LoadRegistrationSwitch();
            //LoadRegisSwitch.IsGetAdmissionInfo = true;
            //var t = new Thread(() =>
            //{
            //    try
            //    {
            //        using (var serviceFactory = new PatientRegistrationServiceClient())
            //        {
            //            bool bContinue = true;
            //            var contract = serviceFactory.ServiceInstance;
            //            //KMx: Chuyển hàm, lý do cần lấy thêm AdmissionInfo để khi ra toa xuất viện lọc lại các khoa BN đã nằm (07/03/2015 10:21)
            //            //contract.BeginGetRegistration(((PatientRegistration)ObjPtRegistration).PtRegistrationID, (int)Globals.PatientFindBy_ForConsultation.Value,
            //            //Globals.DispatchCallback((asyncResult) =>
            //            contract.BeginGetRegistrationInfo_InPt(((PatientRegistration)ObjPtRegistration).PtRegistrationID, (int)Globals.PatientFindBy_ForConsultation.Value, LoadRegisSwitch, false,
            //            Globals.DispatchCallback((asyncResult) =>
            //            {
            //                try
            //                {
            //                    //PatientRegistration regInfo = contract.EndGetRegistration(asyncResult);
            //                    PatientRegistration regInfo = contract.EndGetRegistrationInfo_InPt(asyncResult);
            //                    _currentPatient = regInfo.Patient;
            //                    regInfo.Patient.CurrentHealthInsurance = regInfo.HealthInsurance;
            //                    Registration = regInfo;
            //                    if (aPatientRegistrationDetail != null && aPatientRegistrationDetail is PatientRegistrationDetail && (aPatientRegistrationDetail as PatientRegistrationDetail).PtRegDetailID > 0)
            //                    {
            //                        Globals.SetInfoPatient(regInfo.Patient, regInfo, aPatientRegistrationDetail as PatientRegistrationDetail);
            //                    }
            //                    else
            //                    {
            //                        Globals.SetInfoPatient(regInfo.Patient, regInfo, null);
            //                    }
            //                    //KMx: Hiển thị thông tin bệnh nhân (10/10/2014 16:45).
            //                    Globals.EventAggregator.Publish(new ShowInPatientInfoForConsultation() { Patient = regInfo.Patient, PtRegistration = regInfo });
            //                    //KMx: Load lại trang thông tin chung (10/10/2014 16:45).
            //                    if (Globals.LeftModuleActive == LeftModuleActive.KHAMBENH_THONGTINCHUNG || Globals.LeftModuleActive == LeftModuleActive.KHAMBENH_THONGTINCHUNG_NOITRU)
            //                    {
            //                        Globals.EventAggregator.Publish(new LoadGeneralInfoPageEvent() { Patient = regInfo.Patient });
            //                    }
            //                    //▼====== #003
            //                    #region Load lại thông tin bệnh nhân, khi chuyển bệnh nhân bằng out standing task.
            //                    else if (Globals.LeftModuleActive == LeftModuleActive.KHAMBENH_RATOA_XUATVIEN)
            //                    {
            //                        Globals.EventAggregator.Publish(new SetInPatientInfoAndRegistrationForePresciption_InPt() { });
            //                    }
            //                    else if (Globals.LeftModuleActive == LeftModuleActive.KHAMBENH_CHANDOAN_NOITRU)
            //                    {
            //                        Globals.EventAggregator.Publish(new SetInPatientInfoAndRegistrationForConsultations_InPt()
            //                        { MedServiceID = (aPatientRegistrationDetail as PatientRegistrationDetail) != null ?
            //                        (aPatientRegistrationDetail as PatientRegistrationDetail).MedServiceID.GetValueOrDefault() : 0}
            //                        );
            //                    }
            //                    else if (Globals.LeftModuleActive == LeftModuleActive.KHAMBENH_CLS_PHIEUYEUCAUXETNGHIEM_NT)
            //                    {
            //                        Globals.EventAggregator.Publish(new SetInPatientInfoAndRegistrationForPatientPCLRequest() { });
            //                    }
            //                    else if (Globals.LeftModuleActive == LeftModuleActive.KHAMBENH_CLS_PHIEUYEUCAUHINHANH_NT)
            //                    {
            //                        Globals.EventAggregator.Publish(new SetInPatientInfoAndRegistrationForPatientPCLRequestImage() { });
            //                    }
            //                    else if (Globals.LeftModuleActive == LeftModuleActive.XUATVIEN)
            //                    {
            //                        Globals.EventAggregator.Publish(new SetInPatientInfoAndRegistrationForInPatientDischarge() { });
            //                    }
            //                    #endregion
            //                    //▲====== #003
            //                }
            //                catch (Exception ex)
            //                {
            //                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
            //                    ClientLoggerHelper.LogError(ex.Message);
            //                    bContinue = false;
            //                }
            //                finally
            //                {
            //                    genTask.ActionComplete(bContinue);
            //                    this.HideBusyIndicator();
            //                }
            //            }), null);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
            //        ClientLoggerHelper.LogError(ex.Message);
            //        genTask.ActionComplete(false);
            //        this.HideBusyIndicator();
            //    }
            //});
            //t.Start();
        }

        //public void Handle(ReloadDataConsultationEvent message)
        //{
        //    if (Globals.PatientFindBy_ForConsultation.Value == AllLookupValues.PatientFindBy.NGOAITRU)
        //    {
        //        PatientServiceRecordsGetForKhamBenh(Registration.PtRegistrationID, RegistrationDetail.PtRegDetailID, Registration.V_RegistrationType);
        //    }

        //}

        public void PatientServiceRecordsGetForKhamBenh_Ext(bool bAllowModifyPrescription = false)
        {
            if (Registration == null)
            {
                MessageBox.Show(eHCMSResources.A0733_G1_Msg_InfoKhTimThayDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (Registration.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                PatientServiceRecordsGetForKhamBenh(Registration.PtRegistrationID, RegistrationDetail.PtRegDetailID, Registration.V_RegistrationType, bAllowModifyPrescription);
            }
            else
            {
                PatientServiceRecordsGetForKhamBenh_InPt(Registration.PtRegistrationID, Registration.V_RegistrationType);
            }

            //KMx: Dựa vào đăng ký của BN, không dựa vào tiêu chí tìm kiếm.
            //if (Globals.PatientFindBy_ForConsultation.Value == AllLookupValues.PatientFindBy.NGOAITRU)
            //{
            //    PatientServiceRecordsGetForKhamBenh(Registration.PtRegistrationID, RegistrationDetail.PtRegDetailID, Registration.V_RegistrationType, (int)AllLookupValues.PatientFindBy.NGOAITRU);
            //}
            //else
            //{
            //    //KMx: Truyền PtRegDetailID = 0 vì nội trú không có PtRegDetailID (09/10/2014 10:40).
            //    PatientServiceRecordsGetForKhamBenh(Registration.PtRegistrationID, 0, Registration.V_RegistrationType, (int)AllLookupValues.PatientFindBy.NOITRU);
            //}
        }

        public void PublishEventShowPatientInfo(Patient patient, PatientRegistration regInfo, PatientRegistrationDetail Item)
        {
            if (Globals.LeftModuleActive == LeftModuleActive.NONE)
            {
                return;
            }
            //publish cai nay cho patient info
            //Globals.EventAggregator.Publish(new ShowPatientInfo<Patient, PatientRegistration, PatientRegistrationDetail>() { Pt = patient, PtReg = regInfo, PtRegDetail = Item });
            //kiem tra thang nao dang active roi publish cho no

            //KMx: Show Patient Info (25/05/2014 12:07).
            Globals.EventAggregator.Publish(new ShowPatientInfoForConsultation() { Patient = patient, PtRegistration = regInfo });

            switch (Globals.LeftModuleActive)
            {
                case LeftModuleActive.KHAMBENH_THONGTINCHUNG:
                case LeftModuleActive.KHAMBENH_CHANDOAN_RATOA:
                    Globals.EventAggregator.Publish(new ShowPatientInfo_KHAMBENH_THONGTINCHUNG<Patient, PatientRegistration, PatientRegistrationDetail>() { Pt = patient, PtReg = regInfo, PtRegDetail = Item });
                    break;
                case LeftModuleActive.KHAMBENH_TONGQUAT:
                    Globals.EventAggregator.Publish(new ShowPatientInfo_KHAMBENH_TONGQUAT<Patient, PatientRegistration, PatientRegistrationDetail>() { Pt = patient, PtReg = regInfo, PtRegDetail = Item });
                    break;
                case LeftModuleActive.KHAMBENH_CHANDOAN:
                    Globals.EventAggregator.Publish(new ShowPtRegDetailForDiagnosis() { PtRegistration = regInfo, PtRegDetail = Item });
                    Globals.EventAggregator.Publish(new ShowPatientInfo_KHAMBENH_CHANDOAN<Patient, PatientRegistration, PatientRegistrationDetail>() { Pt = patient, PtReg = regInfo, PtRegDetail = Item });
                    break;
                case LeftModuleActive.KHAMBENH_RATOA:
                    Globals.EventAggregator.Publish(new ShowPtRegDetailForPrescription() { PtRegistration = regInfo, PtRegDetail = Item });
                    Globals.EventAggregator.Publish(new ClearPrescriptionListAfterSelectPatientEvent());
                    Globals.EventAggregator.Publish(new ClearDrugUsedAfterSelectPatientEvent());
                    Globals.EventAggregator.Publish(new ClearPrescriptTemplateAfterSelectPatientEvent());
                    //Globals.EventAggregator.Publish(new ShowPatientInfo_KHAMBENH_RATOA<Patient, PatientRegistration, PatientRegistrationDetail>() { Pt = patient, PtReg = regInfo, PtRegDetail = Item });
                    break;
                case LeftModuleActive.KHAMBENH_LSBENHAN:
                    Globals.EventAggregator.Publish(new ShowPatientInfo_KHAMBENH_LSBENHAN<Patient, PatientRegistration, PatientRegistrationDetail>() { Pt = patient, PtReg = regInfo, PtRegDetail = Item });
                    break;

                case LeftModuleActive.KHAMBENH_CLS_PHIEUYEUCAUXETNGHIEM:
                    Globals.EventAggregator.Publish(new ShowPatientInfo_KHAMBENH_CLS_PHIEUYEUCAU_XETNGHIEM() { Patient = patient });
                    Globals.EventAggregator.Publish(new ShowListPCLRequest_KHAMBENH_CLS_PHIEUYEUCAU_XETNGHIEM() { Patient = patient, PtRegistration = regInfo });
                    //Globals.EventAggregator.Publish(new ShowPatientInfo_KHAMBENH_CLS_PHIEUYEUCAU<Patient, PatientRegistration, PatientRegistrationDetail>() { Pt = patient, PtReg = regInfo, PtRegDetail = Item });
                    break;
                case LeftModuleActive.KHAMBENH_CLS_PHIEUYEUCAUHINHANH:
                    Globals.EventAggregator.Publish(new ShowPatientInfo_KHAMBENH_CLS_PHIEUYEUCAU_HINHANH() { Patient = patient });
                    Globals.EventAggregator.Publish(new ShowListPCLRequest_KHAMBENH_CLS_PHIEUYEUCAU_HINHANH() { Patient = patient, PtRegistration = regInfo });
                    //Globals.EventAggregator.Publish(new ShowPatientInfo_KHAMBENH_CLS_PHIEUYEUCAU<Patient, PatientRegistration, PatientRegistrationDetail>() { Pt = patient, PtReg = regInfo, PtRegDetail = Item });
                    break;
                case LeftModuleActive.KHAMBENH_HENCLS_HENCLS:
                    Globals.EventAggregator.Publish(new ShowPatientInfo_KHAMBENH_HENCLS_HENCLS<Patient, PatientRegistration, PatientRegistrationDetail>() { Pt = patient, PtReg = regInfo, PtRegDetail = Item });
                    break;

                case LeftModuleActive.KHAMBENH_CLS_KETQUAHINHANH:
                    Globals.EventAggregator.Publish(new InitDataForPtPCLImagingResult());
                    //Globals.EventAggregator.Publish(new ShowPatientInfo_KHAMBENH_CLS_KETQUA_HINHANH<Patient, PatientRegistration, PatientRegistrationDetail>() { Pt = patient, PtReg = regInfo, PtRegDetail = Item });
                    break;

                case LeftModuleActive.KHAMBENH_CLS_KETQUAXETNGHIEM:
                    Globals.EventAggregator.Publish(new InitDataForPtPCLLaboratoryResult());
                    //Globals.EventAggregator.Publish(new ShowPatientInfo_KHAMBENH_CLS_KETQUA_XETNGHIEM<Patient, PatientRegistration, PatientRegistrationDetail>() { Pt = patient, PtReg = regInfo, PtRegDetail = Item });
                    break;

                case LeftModuleActive.KHAMBENH_CLS_NGOAIVIEN_HINHANH:
                    Globals.EventAggregator.Publish(new ShowPatientInfo_KHAMBENH_CLS_NGOAIVIEN_HINHANH<Patient, PatientRegistration, PatientRegistrationDetail>() { Pt = patient, PtReg = regInfo, PtRegDetail = Item });
                    break;

                case LeftModuleActive.KHAMBENH_CLS_NGOAIVIEN_XETNGHIEM:
                    Globals.EventAggregator.Publish(new ShowPatientInfo_KHAMBENH_CLS_NGOAIVIEN_XETNGHIEM<Patient, PatientRegistration, PatientRegistrationDetail>() { Pt = patient, PtReg = regInfo, PtRegDetail = Item });
                    break;

                //default:
                //    Globals.EventAggregator.Publish(new ShowPatientInfo<Patient, PatientRegistration, PatientRegistrationDetail>() { Pt = patient, PtReg = regInfo, PtRegDetail = Item });
                //    break;
            }

        }

        public void PublishEventGlobalsPSRLoad(bool IsReloadPrescript, bool bAllowModifyPrescription = false)
        {
            switch (Globals.LeftModuleActive)
            {
                case LeftModuleActive.KHAMBENH_CHANDOAN:
                    Globals.EventAggregator.Publish(new GlobalCurPatientServiceRecordLoadComplete_Consult());
                    break;
                case LeftModuleActive.KHAMBENH_RATOA:
                    if (IsReloadPrescript)
                    {
                        Globals.EventAggregator.Publish(new LoadPrescriptionAfterSaved());
                    }
                    else
                    {
                        Globals.EventAggregator.Publish(new GlobalCurPatientServiceRecordLoadComplete_EPrescript());
                    }
                    break;
                case LeftModuleActive.KHAMBENH_CHANDOAN_RATOA:
                    Globals.EventAggregator.Publish(new GlobalCurPatientServiceRecordLoadComplete_Consult());
                    if (IsReloadPrescript && IsReloadPrescription && bAllowModifyPrescription)
                    {
                        Globals.EventAggregator.Publish(new LoadPrescriptionAfterSaved());
                    }
                    else
                    {
                        Globals.EventAggregator.Publish(new GlobalCurPatientServiceRecordLoadComplete_EPrescript { bJustCallAllowModifyPrescription = true });
                    }
                    break;
                case LeftModuleActive.KHAMBENH_CHANDOAN_NOITRU:
                    Globals.EventAggregator.Publish(new GlobalCurPatientServiceRecordLoadComplete_Consult_InPt());
                    break;
                case LeftModuleActive.KHAMBENH_RATOA_XUATVIEN:
                    Globals.EventAggregator.Publish(new LoadPrescriptionInPtAfterSaved());
                    break;
            }
        }

        #endregion

        #region Sau khi Tìm Bệnh Nhân
        public void Handle(ResultFound<Patient> message)
        {
            if (Globals.HomeModuleActive != HomeModuleActive.KHAMBENH)
            {
                return;
            }
            if (message != null && this.GetView() != null)
            {
                CurrentPatient = message.Result;
                if (CurrentPatient != null)
                {
                    Globals.EventAggregator.Publish(new ItemSelected<Patient>() { Item = CurrentPatient });
                }
            }
        }
        public void Handle(ResultNotFound<Patient> message)
        {
            if (Globals.HomeModuleActive != HomeModuleActive.KHAMBENH)
            {
                return;
            }
            if (message != null)
            {
                MessageBox.Show(eHCMSResources.Z0478_G1_KhongTimThayBenhNhan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
        }
        #endregion

        private bool _IsReloadPrescription = true;
        public bool IsReloadPrescription { get => _IsReloadPrescription; set => _IsReloadPrescription = value; }

        public void PatientServiceRecordsGetForKhamBenh(long PtRegistrationID, long PtRegDetailID, AllLookupValues.RegistrationType V_RegistrationType, bool bAllowModifyPrescription)
        {
            //this.ShowBusyIndicator();
            //PatientServiceRecord psrSearch = new PatientServiceRecord();
            //psrSearch.PtRegistrationID = PtRegistrationID;
            //psrSearch.PtRegDetailID = PtRegDetailID;
            //psrSearch.V_RegistrationType = V_RegistrationType;
            //var t = new Thread(() =>
            //{
            //    //IsLoading = true;
            //    //this.ShowBusyIndicator();
            //    try
            //    {
            //        using (var serviceFactory = new CommonUtilsServiceClient())
            //        {
            //            var contract = serviceFactory.ServiceInstance;
            //            contract.BeginPatientServiceRecordsGetForKhamBenh(psrSearch,
            //            Globals.DispatchCallback((asyncResult) =>
            //            {
            //                try
            //                {
            //                    var psr = contract.EndPatientServiceRecordsGetForKhamBenh(asyncResult);
            //                    Globals.curLstPatientServiceRecord = new ObservableCollection<PatientServiceRecord>(psr);
            //                    PublishEventGlobalsPSRLoad(IsReloadPrescription, bAllowModifyPrescription);
            //                }
            //                catch (FaultException<AxException> fault)
            //                {
            //                    ClientLoggerHelper.LogInfo(fault.ToString());
            //                }
            //                finally
            //                {
            //                    //RegistrationLoading = false;
            //                    //IsLoading = false;
            //                    this.HideBusyIndicator();
            //                }
            //            }), null);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
            //    }
            //    finally
            //    {
            //        //Globals.IsBusy = false;
            //        //RegistrationLoading = false;
            //        //IsLoading = false;
            //        this.HideBusyIndicator();
            //    }
            //});
            //t.Start();
        }


        public void PatientServiceRecordsGetForKhamBenh_InPt(long PtRegistrationID, AllLookupValues.RegistrationType V_RegistrationType)
        {
            //this.ShowBusyIndicator();
            //PatientServiceRecord psrSearch = new PatientServiceRecord();
            //psrSearch.PtRegistrationID = PtRegistrationID;
            //psrSearch.V_RegistrationType = V_RegistrationType;
            //var t = new Thread(() =>
            //{
            //    try
            //    {
            //        using (var serviceFactory = new CommonUtilsServiceClient())
            //        {
            //            var contract = serviceFactory.ServiceInstance;
            //            contract.BeginPatientServiceRecordsGetForKhamBenh_InPt(psrSearch,
            //            Globals.DispatchCallback((asyncResult) =>
            //            {
            //                try
            //                {
            //                    var psr = contract.EndPatientServiceRecordsGetForKhamBenh_InPt(asyncResult);
            //                    Globals.curLstPatientServiceRecord = new ObservableCollection<PatientServiceRecord>(psr);
            //                    PublishEventGlobalsPSRLoad(true);
            //                }
            //                catch (FaultException<AxException> fault)
            //                {
            //                    ClientLoggerHelper.LogInfo(fault.ToString());
            //                }
            //                finally
            //                {
            //                    this.HideBusyIndicator();
            //                }
            //            }), null);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
            //    }
            //    finally
            //    {
            //        this.HideBusyIndicator();
            //    }
            //});
            //t.Start();
        }

    }
}