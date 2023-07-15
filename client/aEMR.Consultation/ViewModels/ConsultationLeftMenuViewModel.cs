using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using aEMR.Common;
using System.Windows.Controls;
using Castle.Windsor;
/*
 * 20170103 #001 CMN: Added StaffDept
 * 20171124 #002 CMN: Added Daily diagnostic
 * 20181022 #003 TTM: BM0003214: Fix lỗi PatientInfo không hiển thị cho khám bệnh nội trú (Chuyển sang cách làm mới comment activeControl).
 */
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IConsultationLeftMenu)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class ConsultationLeftMenuViewModel : Conductor<object>, IConsultationLeftMenu
    //, IHandle<LocationSelected>
    {
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public ConsultationLeftMenuViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            _eventArg = eventArg;
            Globals.PageName = "";
            Globals.TitleForm = "";
            Globals.EventAggregator.Subscribe(this);

            authorization();

            if (Globals.allContraIndicatorDrugsRelToMedCond == null)
            {
                GetAllContrainIndicatorDrugs();
            }

        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bCommonRecs = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConsultation
                                                 , (int)eConsultation.mPtDashboardCommonRecs);
            bConsultationCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConsultation
                                                 , (int)eConsultation.mPtPMRConsultationNew);

            bPrescriptionCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConsultation
                                                 , (int)eConsultation.mPtePrescriptionTab);

            bSummaryCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConsultation
                                                 , (int)eConsultation.mPtePatientSummaryRecord);

            bAppointmentRequestCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConsultation
                                                 , (int)eConsultation.mPteAppointmentTab);
            mHenCanLamSang = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConsultation
                                                 , (int)eConsultation.mPtHenXetNghiem);


            bPatientPCLRequestCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConsultation
                                                 , (int)eConsultation.mPtPCLRequest);
            bPatientPCLImagingResultsCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConsultation
                                                 , (int)eConsultation.mPatientPCLImagingResultsCmd);
            bPatientPCLLaboratoryResultsCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConsultation
                                                 , (int)eConsultation.mPatientPCLLaboratoryResultsCmd);
            bPSRCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConsultation
                                                 , (int)eConsultation.mPtePatientSummaryRecord);

            mThongKe_DSBenhNhanDaKham = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConsultation
                                                 , (int)eConsultation.mThongKe_DSBenhNhanDaKham);
            mThongKe_DSBacSiKham = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConsultation
                                                 , (int)eConsultation.mThongKe_DSBacSiKham);
            mThongKe_BangKeChiTietKhamBenh = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConsultation
                                          , (int)eConsultation.mThongKe_BangKeChiTietKhamBenh);

            mConsultingDiagnosysEditAuth = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                                   , (int)eConsultation.mConsultingDiagnosy
                                                   , (int)oConsultationEx.mConsultingDiagnosys_ConsultingEdit, (int)ePermission.mView);
            mConsultingDiagnosysFullOpAuth = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mConsultingDiagnosy
                                               , (int)oConsultationEx.mConsultingDiagnosys_Full, (int)ePermission.mView);
            if (mConsultingDiagnosysFullOpAuth) mConsultingDiagnosysEditAuth = true;
            gPrevSurgeryListReport = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mConsultingDiagnosy
                                               , (int)oConsultationEx.mConsultingDiagnosys_PrevSurgeryList, (int)ePermission.mView);
            if (Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mConsultingDiagnosy
                                               , (int)oConsultationEx.mConsultingDiagnosys_InCompleteFileList, (int)ePermission.mView)
                    || Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mConsultingDiagnosy
                                               , (int)oConsultationEx.mConsultingDiagnosys_AppliedList, (int)ePermission.mView)
                    || Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mConsultingDiagnosy
                                               , (int)oConsultationEx.mConsultingDiagnosys_OperatedList, (int)ePermission.mView)
                    || Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mConsultingDiagnosy
                                               , (int)oConsultationEx.mConsultingDiagnosys_WaitForSurgeryList, (int)ePermission.mView)
                    || Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mConsultingDiagnosy
                                               , (int)oConsultationEx.mConsultingDiagnosys_DuraGraftList, (int)ePermission.mView))
                gSummarySurgeryListReport = true;
            else
                gSummarySurgeryListReport = false;


            mLichSu = bSummaryCmd;
            mKhamBenh = bSummaryCmd || bCommonRecs || bConsultationCmd || bPrescriptionCmd || bPSRCmd;
            mCanLamSang = bPatientPCLRequestCmd || bPatientPCLLaboratoryResultsCmd || bPatientPCLImagingResultsCmd;
            mHenCanLamSang = mHenCanLamSang;
            mThongKe = mThongKe_DSBenhNhanDaKham || mThongKe_DSBacSiKham || bSummaryCmd || mThongKe_BangKeChiTietKhamBenh;
        }

        #region Property
        private bool _bCommonRecs = true;
        private bool _bConsultationCmd = true;
        private bool _bPrescriptionCmd = true;
        private bool _bSummaryCmd = true;
        private bool _bAppointmentRequestCmd = true;
        private bool _bPatientPCLRequestCmd = true;
        private bool _bPatientPCLImagingResultsCmd = true;
        private bool _bPatientPCLLaboratoryResultsCmd = true;
        private bool _bPSRCmd = true;

        private bool _mThongKe_DSBenhNhanDaKham = true;
        private bool _mThongKe_DSBacSiKham = true;
        private bool _mThongKe_BangKeChiTietKhamBenh = true;
        private bool _mThongKe = true;
        private bool _mHenCanLamSang = true;
        private bool _mCanLamSang = true;
        private bool _mKhamBenh = true;
        private bool _mLichSu = true;


        public bool bCommonRecs
        {
            get
            {
                return _bCommonRecs;
            }
            set
            {
                if (_bCommonRecs == value)
                    return;
                _bCommonRecs = value;
            }
        }
        public bool bConsultationCmd
        {
            get
            {
                return _bConsultationCmd;
            }
            set
            {
                if (_bConsultationCmd == value)
                    return;
                _bConsultationCmd = value;
            }
        }
        public bool bPrescriptionCmd
        {
            get
            {
                return _bPrescriptionCmd;
            }
            set
            {
                if (_bPrescriptionCmd == value)
                    return;
                _bPrescriptionCmd = value;
            }
        }
        public bool bSummaryCmd
        {
            get
            {
                return _bSummaryCmd;
            }
            set
            {
                if (_bSummaryCmd == value)
                    return;
                _bSummaryCmd = value;
            }
        }
        public bool bAppointmentRequestCmd
        {
            get
            {
                return _bAppointmentRequestCmd;
            }
            set
            {
                if (_bAppointmentRequestCmd == value)
                    return;
                _bAppointmentRequestCmd = value;
            }
        }
        public bool bPatientPCLRequestCmd
        {
            get
            {
                return _bPatientPCLRequestCmd;
            }
            set
            {
                if (_bPatientPCLRequestCmd == value)
                    return;
                _bPatientPCLRequestCmd = value;
            }
        }
        public bool bPatientPCLImagingResultsCmd
        {
            get
            {
                return _bPatientPCLImagingResultsCmd;
            }
            set
            {
                if (_bPatientPCLImagingResultsCmd == value)
                    return;
                _bPatientPCLImagingResultsCmd = value;
            }
        }
        public bool bPatientPCLLaboratoryResultsCmd
        {
            get
            {
                return _bPatientPCLLaboratoryResultsCmd;
            }
            set
            {
                if (_bPatientPCLLaboratoryResultsCmd == value)
                    return;
                _bPatientPCLLaboratoryResultsCmd = value;
            }
        }
        public bool bPSRCmd
        {
            get
            {
                return _bPSRCmd;
            }
            set
            {
                if (_bPSRCmd == value)
                    return;
                _bPSRCmd = value;
            }
        }

        public bool mThongKe_DSBenhNhanDaKham
        {
            get
            {
                return _mThongKe_DSBenhNhanDaKham
                ;
            }
            set
            {
                if (_mThongKe_DSBenhNhanDaKham
                 == value)
                    return;
                _mThongKe_DSBenhNhanDaKham
                 = value;
                NotifyOfPropertyChange(() => mThongKe_DSBenhNhanDaKham
                );
            }
        }
        public bool mThongKe_DSBacSiKham
        {
            get
            {
                return _mThongKe_DSBacSiKham;
            }
            set
            {
                if (_mThongKe_DSBacSiKham == value)
                    return;
                _mThongKe_DSBacSiKham = value;
                NotifyOfPropertyChange(() => mThongKe_DSBacSiKham);
            }
        }

        public bool mThongKe_BangKeChiTietKhamBenh
        {
            get
            {
                return _mThongKe_BangKeChiTietKhamBenh;
            }
            set
            {
                if (_mThongKe_BangKeChiTietKhamBenh == value)
                    return;
                _mThongKe_BangKeChiTietKhamBenh = value;
                NotifyOfPropertyChange(() => mThongKe_BangKeChiTietKhamBenh);
            }
        }

        public bool mThongKe
        {
            get
            {
                return _mThongKe;
            }
            set
            {
                if (_mThongKe == value)
                    return;
                _mThongKe = value;
                NotifyOfPropertyChange(() => mThongKe);
            }
        }


        public bool mHenCanLamSang
        {
            get
            {
                return _mHenCanLamSang;
            }
            set
            {
                if (_mHenCanLamSang == value)
                    return;
                _mHenCanLamSang = value;
                NotifyOfPropertyChange(() => mHenCanLamSang);
            }
        }

        public bool mCanLamSang
        {
            get
            {
                return _mCanLamSang;
            }
            set
            {
                if (_mCanLamSang == value)
                    return;
                _mCanLamSang = value;
                NotifyOfPropertyChange(() => mCanLamSang);
            }
        }

        public bool mKhamBenh
        {
            get
            {
                return _mKhamBenh;
            }
            set
            {
                if (_mKhamBenh == value)
                    return;
                _mKhamBenh = value;
                NotifyOfPropertyChange(() => mKhamBenh);
            }
        }

        public bool mLichSu
        {
            get
            {
                return _mLichSu;
            }
            set
            {
                if (_mLichSu == value)
                    return;
                _mLichSu = value;
                NotifyOfPropertyChange(() => mLichSu);
            }
        }

        private bool _VisibleDailyDiagnostic = Globals.ServerConfigSection.CommonItems.IsUseDailyDiagnostic;
        public bool VisibleDailyDiagnostic
        {
            get
            {
                return _VisibleDailyDiagnostic;
            }
            set
            {
                _VisibleDailyDiagnostic = value;
                NotifyOfPropertyChange(() => VisibleDailyDiagnostic);
            }
        }

        private bool _mConsultingDiagnosysEditAuth = true;
        public bool mConsultingDiagnosysEditAuth
        {
            get
            {
                return _mConsultingDiagnosysEditAuth;
            }
            set
            {
                if (_mConsultingDiagnosysEditAuth == value) return;
                _mConsultingDiagnosysEditAuth = value;
                NotifyOfPropertyChange(() => mConsultingDiagnosysEditAuth);
            }
        }
        private bool _mConsultingDiagnosysFullOpAuth = true;

        public bool mConsultingDiagnosysFullOpAuth
        {
            get
            {
                return _mConsultingDiagnosysFullOpAuth;
            }
            set
            {
                if (_mConsultingDiagnosysFullOpAuth == value) return;
                _mConsultingDiagnosysFullOpAuth = value;
                NotifyOfPropertyChange(() => mConsultingDiagnosysFullOpAuth);
            }
        }
        #endregion

        public void GetAllContrainIndicatorDrugs()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetAllContrainIndicatorDrugs(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetAllContrainIndicatorDrugs(asyncResult);
                            if (results != null)
                            {
                                if (Globals.allContraIndicatorDrugsRelToMedCond == null)
                                {
                                    Globals.allContraIndicatorDrugsRelToMedCond = new ObservableCollection<ContraIndicatorDrugsRelToMedCond>();
                                }
                                else
                                {
                                    Globals.allContraIndicatorDrugsRelToMedCond.Clear();
                                }
                                foreach (var p in results)
                                {
                                    Globals.allContraIndicatorDrugsRelToMedCond.Add(p);
                                }
                                NotifyOfPropertyChange(() => Globals.allContraIndicatorDrugsRelToMedCond);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            Globals.IsBusy = false;

                        }

                    }), null);

                }

            });

            t.Start();
        }

        //public void AppointmentRequestCmd(object source)
        //{
        //    SetHyperlinkSelectedStyle(source as Button);
        //    Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_HENCLS_HENCLS;
        //    var Conslt = Globals.GetViewModel<IConsultationModule>();
        //    var apptVm = Globals.GetViewModel<IAppointmentRequest>();

        //    Conslt.MainContent = apptVm;
        //    (Conslt as Conductor<object>).ActivateItem(apptVm);
        //}

        private void PCLRequestCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            //KMx: Phân biệt Left Module Active giữa Phiếu YC Hình ảnh và Phiếu YC Xét nghiệm rõ ràng, không để chung (25/05/2014 10:05)
            //Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CLS_PHIEUYEUCAU;
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CLS_PHIEUYEUCAUHINHANH;
            Globals.PageName = Globals.TitleForm;
            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var VM = Globals.GetViewModel<IPatientPCLRequestImage>();
            Conslt.MainContent = VM;
            (Conslt as Conductor<object>).ActivateItem(VM);
        }

        public void PCLRequestCmd(object source)
        {
            if (!CheckAvailable(AllLookupValues.V_PCLRequestType.NGOAI_TRU))
            {
                return;
            }
            Globals.TitleForm = string.Format("{0} ", eHCMSResources.P0382_G1_PhYeuCauHA);
            PCLRequestCmd_In(source);

        }

        private void PCLLaboratoryRequestCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            //KMx: Phân biệt Left Module Active giữa Phiếu YC Hình ảnh và Phiếu YC Xét nghiệm rõ ràng, không để chung (25/05/2014 10:05)
            //Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CLS_PHIEUYEUCAU;
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CLS_PHIEUYEUCAUXETNGHIEM;
            Globals.PageName = Globals.TitleForm;
            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var VM = Globals.GetViewModel<IPatientPCLRequest>();
            Conslt.MainContent = VM;
            (Conslt as Conductor<object>).ActivateItem(VM);
        }

        public void PCLLaboratoryRequestCmd(object source)
        {
            if (!CheckAvailable(AllLookupValues.V_PCLRequestType.NGOAI_TRU))
            {
                return;
            }
            Globals.TitleForm = eHCMSResources.P0383_G1_PhYeuCauXetNghiem;
            PCLLaboratoryRequestCmd_In(source);
        }

        public void PCLImagingResultsCmd(object source)
        {
            if (!CheckAvailable(AllLookupValues.V_PCLRequestType.NGOAI_TRU))
            {
                return;
            }
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CLS_KETQUAHINHANH;
            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var VM = Globals.GetViewModel<IPatientPCLImagingResult>();

            Conslt.MainContent = VM;
            (Conslt as Conductor<object>).ActivateItem(VM);

            ////Load OutStandingstask
            //var shell = Globals.GetViewModel<IHome>();
            //var UCPCLDepartmentOutstandingTaskView = Globals.GetViewModel<IPCL_ViewResults_Image_OutstandingTask>();

            //shell.OutstandingTaskContent = UCPCLDepartmentOutstandingTaskView;
            //(shell as Conductor<object>).ActivateItem(UCPCLDepartmentOutstandingTaskView);
        }

        //PCLExamTargetCmd
        private void PCLExamTargetCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);

            Globals.LeftModuleActive = LeftModuleActive.NONE;

            Globals.PageName = Globals.TitleForm;

            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var VM = Globals.GetViewModel<ICommonPCLExamTarget>();

            Conslt.MainContent = VM;
            (Conslt as Conductor<object>).ActivateItem(VM);
        }

        public void PCLExamTargetCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0479_G1_DanhSachCLSSo;
            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                PCLExamTargetCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        PCLExamTargetCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }


        public void PCLImagingExtResultsCmd(object source)
        {
            if (!CheckAvailable(AllLookupValues.V_PCLRequestType.NGOAI_TRU))
            {
                return;
            }
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CLS_NGOAIVIEN_HINHANH;
            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var VM = Globals.GetViewModel<IPatientPCLDeptImagingExtHome>();
            Conslt.MainContent = VM;
            (Conslt as Conductor<object>).ActivateItem(VM);
        }

        public void PCLLaboratoryResultsCmd(object source)
        {
            if (!CheckAvailable(AllLookupValues.V_PCLRequestType.NGOAI_TRU))
            {
                return;
            }
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CLS_KETQUAXETNGHIEM;
            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var VM = Globals.GetViewModel<IPatientPCLLaboratoryResult>();

            Conslt.MainContent = VM;
            (Conslt as Conductor<object>).ActivateItem(VM);
        }


        private void DiagnosisTreatmentByDoctorStaffIDCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            //var loginVm = Globals.GetViewModel<ILogin>();
            //if (loginVm.DeptLocation == null) //Chưa chọn phòng => Yêu cầu Chọn phòng.
            //{
            //    var locationVm = Globals.GetViewModel<ISelectLocation>();
            //    locationVm.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNgoaiTru;
            //    Globals.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNgoaiTru;
            //    locationVm.ItemActivated = Globals.GetViewModel<IDiagnosisTreatmentByDoctorStaffID>();
            //    Globals.ShowDialog(locationVm as Conductor<object>);
            //}
            //else
            {
                Globals.LeftModuleActive = LeftModuleActive.NONE;
                var Conslt = Globals.GetViewModel<IConsultationModule>();
                var VM = Globals.GetViewModel<IDiagnosisTreatmentByDoctorStaffID>();

                Conslt.MainContent = VM;
                (Conslt as Conductor<object>).ActivateItem(VM);
            }
        }

        public void DiagnosisTreatmentByDoctorStaffIDCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K2944_G1_DSBNDaKham;
            DiagnosisTreatmentByDoctorStaffIDCmd_In(source);

            //if (string.IsNullOrEmpty(Globals.PageName))
            //{
            //    DiagnosisTreatmentByDoctorStaffIDCmd_In(source);
            //}
            //else if (Globals.PageName != Globals.TitleForm)
            //{
            //    Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
            //    {
            //        if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
            //        {
            //            DiagnosisTreatmentByDoctorStaffIDCmd_In(source);
            //            GlobalsNAV.msgb = null;
            //        }
            //    });
            //}
        }

        private void AllDiagnosisGroupByDoctorStaffIDDeptLocationIDCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.NONE;
            Globals.PageName = Globals.TitleForm;
            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var VM = Globals.GetViewModel<IAllDiagnosisGroupByDoctorStaffIDDeptLocationID>();

            Conslt.MainContent = VM;
            (Conslt as Conductor<object>).ActivateItem(VM);
        }

        public void AllDiagnosisGroupByDoctorStaffIDDeptLocationIDCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K3075_G1_DSTatCaBsiKham;
            AllDiagnosisGroupByDoctorStaffIDDeptLocationIDCmd_In(source);

            //if (string.IsNullOrEmpty(Globals.PageName))
            //{
            //    AllDiagnosisGroupByDoctorStaffIDDeptLocationIDCmd_In(source);
            //}
            //else if (Globals.PageName != Globals.TitleForm)
            //{
            //    Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
            //    {
            //        if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
            //        {
            //            AllDiagnosisGroupByDoctorStaffIDDeptLocationIDCmd_In(source);
            //            GlobalsNAV.msgb = null;
            //        }
            //    });
            //}
        }


        //public void Handle(LocationSelected message)
        //{
        //    if (message != null && message.DeptLocation != null)
        //    {
        //        var regModule = Globals.GetViewModel<IConsultationModule>();
        //        if (message.ItemActivated == null)
        //        {
        //            //không làm gì hết vì chưa chọn phòng khám
        //        }
        //        else
        //        {
        //            regModule.MainContent = message.ItemActivated;
        //            (regModule as Conductor<object>).ActivateItem(message.ItemActivated);
        //        }

        //    }
        //}


        #region "CRUD Khám Bệnh"
        private void SummaryCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_THONGTINCHUNG;

            Globals.PageName = Globals.TitleForm;

            var homeVM = Globals.GetViewModel<IHome>();
            homeVM.FindRegistrationCmdVisibility = true;


            //var loginVm = Globals.GetViewModel<ILogin>();
            //if (loginVm.DeptLocation == null) //Chưa chọn phòng => Yêu cầu Chọn phòng.
            //{
            //    var locationVm = Globals.GetViewModel<ISelectLocation>();
            //    locationVm.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNgoaiTru;
            //    Globals.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNgoaiTru;
            //    locationVm.ItemActivated = Globals.GetViewModel<IPtDashboardSummary>();
            //    Globals.ShowDialog(locationVm as Conductor<object>);
            //}
            //else
            {
                var Conslt = Globals.GetViewModel<IConsultationModule>();
                var PrescriptionVM = Globals.GetViewModel<IPtDashboardSummary>();
                PrescriptionVM.V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU;
                Conslt.MainContent = PrescriptionVM;
                (Conslt as Conductor<object>).ActivateItem(PrescriptionVM);
            }
        }

        public void SummaryCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.G0574_G1_TTinChung;

            SummaryCmd_In(source);

            //if (string.IsNullOrEmpty(Globals.PageName))
            //{
            //    SummaryCmd_In(source);
            //}
            //else if (Globals.PageName != Globals.TitleForm)
            //{
            //    Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
            //    {
            //        if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
            //        {
            //            SummaryCmd_In(source);
            //            GlobalsNAV.msgb = null;
            //        }
            //    });
            //}
        }

        private void CommonRecs_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_TONGQUAT;
            Globals.PageName = Globals.TitleForm;

            //var loginVm = Globals.GetViewModel<ILogin>();
            //if (loginVm.DeptLocation == null) //Chưa chọn phòng => Yêu cầu Chọn phòng.
            //{
            //    var locationVm = Globals.GetViewModel<ISelectLocation>();
            //    locationVm.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNgoaiTru;
            //    Globals.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNgoaiTru;
            //    locationVm.ItemActivated = Globals.GetViewModel<ICommonRecs>();
            //    Globals.ShowDialog(locationVm as Conductor<object>);
            //}
            //else
            {
                var Conslt = Globals.GetViewModel<IConsultationModule>();
                var CommonRecs = Globals.GetViewModel<ICommonRecs>();

                Conslt.MainContent = CommonRecs;
                this.ActivateItem(CommonRecs);
            }
        }

        public void CommonRecs(object source)
        {
            Globals.TitleForm = eHCMSResources.G1527_G1_TQuat;

            CommonRecs_In(source);
        }

        private void ConsultationCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CHANDOAN;
            Globals.PageName = Globals.TitleForm;
            Globals.ConsultationIsChildWindow = false;

            //var loginVm = Globals.GetViewModel<ILogin>();
            //if (loginVm.DeptLocation == null) //Chưa chọn phòng => Yêu cầu Chọn phòng.
            //{
            //    var locationVm = Globals.GetViewModel<ISelectLocation>();
            //    locationVm.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNgoaiTru;
            //    Globals.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNgoaiTru;
            //    locationVm.ItemActivated = Globals.GetViewModel<IConsultations>();
            //    Globals.ShowDialog(locationVm as Conductor<object>);
            //}
            //else
            {
                var Conslt = Globals.GetViewModel<IConsultationModule>();
                var ConsultationVM = Globals.GetViewModel<IConsultations>();
                Conslt.MainContent = ConsultationVM;
                (Conslt as Conductor<object>).ActivateItem(ConsultationVM);
            }
        }

        public void ConsultationCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K1746_G1_CDoan;
            ConsultationCmd_In(source);

            //if (string.IsNullOrEmpty(Globals.PageName))
            //{
            //    ConsultationCmd_In(source);
            //}
            //else if (Globals.PageName != Globals.TitleForm)
            //{
            //    Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
            //    {
            //        if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
            //        {
            //            ConsultationCmd_In(source);
            //            GlobalsNAV.msgb = null;
            //        }
            //    });
            //}
        }

        private void PrescriptionCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_RATOA;
            Globals.PageName = Globals.TitleForm;

            Globals.ConsultationIsChildWindow = true;
            Globals.PrescriptionIsChildWindow = false;

            //var loginVm = Globals.GetViewModel<ILogin>();
            //if (loginVm.DeptLocation == null) //Chưa chọn phòng => Yêu cầu Chọn phòng.
            //{
            //    var locationVm = Globals.GetViewModel<ISelectLocation>();
            //    locationVm.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNgoaiTru;
            //    Globals.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNgoaiTru;
            //    locationVm.ItemActivated = Globals.GetViewModel<IePrescriptions>();
            //    Globals.ShowDialog(locationVm as Conductor<object>);
            //}
            //else
            {
                var Conslt = Globals.GetViewModel<IConsultationModule>();
                var PrescriptionVM = Globals.GetViewModel<IePrescriptions>();

                Conslt.MainContent = PrescriptionVM;
                (Conslt as Conductor<object>).ActivateItem(PrescriptionVM);
            }

        }

        public void PrescriptionCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.R0501_G1_RaToa;
            PrescriptionCmd_In(source);
        }


        private void Consultation_InPt_Cmd_OutDept_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CHANDOAN_NOITRU;
            Globals.PageName = Globals.TitleForm;


            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var ConsultationVM = Globals.GetViewModel<IConsultations_InPt>();
            //▼====== #003
            //ConsultationVM.activeControl();
            //▲====== #003
            Conslt.MainContent = ConsultationVM;
            (Conslt as Conductor<object>).ActivateItem(ConsultationVM);
        }

        public void Consultation_InPt_OutDept_Cmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K1777_G1_CDoanXKhoa;
            Consultation_InPt_Cmd_OutDept_In(source);
        }
        /*▼====: #002*/
        public void Consultation_InPt_Daily_Cmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2157_G1_ChanDoanHangNgay;

            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CHANDOAN_NOITRU;
            Globals.PageName = Globals.TitleForm;

            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var ConsultationVM = Globals.GetViewModel<IConsultations_InPt>();
            ConsultationVM.IsDailyDiagnosis = true;
            //▼====== #003
            //ConsultationVM.activeControl();
            //▲====== #003
            Conslt.MainContent = ConsultationVM;
            (Conslt as Conductor<object>).ActivateItem(ConsultationVM);
        }
        /*▲====: #002*/

        private void Consultation_InPt_Cmd_OutHos_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CHANDOAN_NOITRU;
            Globals.PageName = Globals.TitleForm;


            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var ConsultationVM = Globals.GetViewModel<IConsultations_InPt>();
            ConsultationVM.IsDiagnosisOutHospital = true;
            //▼====== #003
            //ConsultationVM.activeControl();
            //▲====== #003
            Conslt.MainContent = ConsultationVM;
            (Conslt as Conductor<object>).ActivateItem(ConsultationVM);

        }

        public void Consultation_InPt_OutHos_Cmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K1771_G1_CDoanNhapXV;
            Consultation_InPt_Cmd_OutHos_In(source);
        }

        private void InPrescriptionCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_RATOA_XUATVIEN;
            Globals.PageName = Globals.TitleForm;

            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var PrescriptionVM = Globals.GetViewModel<IeInPrescriptions>();

            Conslt.MainContent = PrescriptionVM;
            (Conslt as Conductor<object>).ActivateItem(PrescriptionVM);

        }

        public void InPrescriptionCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.G1444_G1_ToaXV;
            InPrescriptionCmd_In(source);
        }

        private void InPatientDischargeCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            //Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_RATOA_XUATVIEN;
            Globals.PageName = Globals.TitleForm;

            var consultModule = Globals.GetViewModel<IConsultationModule>();
            var InPtDischargeNewVM = Globals.GetViewModel<IDischargeNew>();

            InPtDischargeNewVM.IsConsultation = true;

            InPtDischargeNewVM.InitView(true);

            consultModule.MainContent = InPtDischargeNewVM;
            (consultModule as Conductor<object>).ActivateItem(InPtDischargeNewVM);

        }

        public void InPatientDischargeCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0482_G1_XuatVienBenhNhan;
            InPatientDischargeCmd_In(source);
        }

        private void PatientInstructionCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            //Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_RATOA_XUATVIEN;
            Globals.PageName = Globals.TitleForm;

            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var medicalInstructionVM = Globals.GetViewModel<IInPatientInstruction>();

            Conslt.MainContent = medicalInstructionVM;
            (Conslt as Conductor<object>).ActivateItem(medicalInstructionVM);

        }

        public void PatientInstructionCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.G1444_G1_ToaXV;
            PatientInstructionCmd_In(source);
        }



        private void SurgeryBookingCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            //Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_RATOA_XUATVIEN;
            Globals.PageName = Globals.TitleForm;

            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var medicalInstructionVM = Globals.GetViewModel<ISurgeryBooking>();

            Conslt.MainContent = medicalInstructionVM;
            (Conslt as Conductor<object>).ActivateItem(medicalInstructionVM);

        }
        public void SurgeryBookingCmd(object source)
        {
            Globals.TitleForm = "LẬP CA PHẪU THUẬT";
            SurgeryBookingCmd_In(source);
        }

        private void SurgicalReportCmd_In(object source)
        {
            //SetHyperlinkSelectedStyle(source as Button);
            //Globals.PageName = Globals.TitleForm;

            //var Conslt = Globals.GetViewModel<IConsultationModule>();
            //var medicalInstructionVM = Globals.GetViewModel<ISurgicalReport>();

            //Conslt.MainContent = medicalInstructionVM;
            //(Conslt as Conductor<object>).ActivateItem(medicalInstructionVM);
        }
        public void SurgicalReportCmd(object source)
        {
            Globals.TitleForm = "TƯỜNG TRÌNH PHẪU THUẬT";
            SurgicalReportCmd_In(source);
        }

        private void PatientSummaryCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_LSBENHAN;
            Globals.PageName = Globals.TitleForm;

            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var apptVm = Globals.GetViewModel<IPatientTreeForm>();

            Conslt.MainContent = apptVm;
            (Conslt as Conductor<object>).ActivateItem(apptVm);
        }

        public void PatientSummaryCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.T2588_G1_LSuBA;
            PatientSummaryCmd_In(source); 
        }
        #endregion


        #region History Khám Bệnh
   

        private void ConsultRoomDetailCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);

            Globals.LeftModuleActive = LeftModuleActive.NONE;

            Globals.PageName = Globals.TitleForm;

            var homeVM = Globals.GetViewModel<IHome>();
            //homeVM.FindRegistrationCmdVisibility = false;

            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var ConsultRoomDetailVM = Globals.GetViewModel<IConsultRoomDetail>();
            ConsultRoomDetailVM.LoadRefDept(V_DeptTypeOperation.KhoaNgoaiTru);
            Conslt.MainContent = ConsultRoomDetailVM;
            (Conslt as Conductor<object>).ActivateItem(ConsultRoomDetailVM);

        }

        public void ConsultRoomDetailCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K1035_G1_BKeCTietKB;
            ConsultRoomDetailCmd_In(source);
        }
        #endregion

        #region Hẹn Cận Lâm Sàng
        private void PCLRequestHenCLSCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_HENCLS_HENCLS;
            Globals.PageName = Globals.TitleForm;

            //var loginVm = Globals.GetViewModel<ILogin>();
            //if (loginVm.DeptLocation == null) //Chưa chọn phòng => Yêu cầu Chọn phòng.
            //{
            //    var locationVm = Globals.GetViewModel<ISelectLocation>();
            //    locationVm.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNgoaiTru;
            //    Globals.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNgoaiTru;
            //    locationVm.ItemActivated = Globals.GetViewModel<IPatientAppointments_PCL_InConsultation>();
            //    Globals.ShowDialog(locationVm as Conductor<object>);
            //}
            //else
            {
                var Conslt = Globals.GetViewModel<IConsultationModule>();
                var VM = Globals.GetViewModel<IPatientAppointments_PCL_InConsultation>();
                VM.IsCreateApptFromConsultation = true;
                Conslt.MainContent = VM;
                (Conslt as Conductor<object>).ActivateItem(VM);
            }

        }

        public void PCLRequestHenCLSCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.T1464_G1_HenCLS;
            PCLRequestHenCLSCmd_In(source);

            //if (string.IsNullOrEmpty(Globals.PageName))
            //{
            //    PCLRequestHenCLSCmd_In(source);
            //}
            //else if (Globals.PageName != Globals.TitleForm)
            //{
            //    Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
            //    {
            //        if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
            //        {
            //            PCLRequestHenCLSCmd_In(source);
            //            GlobalsNAV.msgb = null;
            //        }
            //    });
            //}
        }
        #endregion


        public void PrescriptionNoteTemplatesCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.T3744_G1_MauLoiDan;
            Globals.PageName = Globals.TitleForm;

            SetHyperlinkSelectedStyle(source as Button);
            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var VM = Globals.GetViewModel<IPrescriptionNoteTemplates>();

            Conslt.MainContent = VM;
            (Conslt as Conductor<object>).ActivateItem(VM);
        }

        public void ManagePatientDetailsCmd(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_QUANLY_THONGTIN_BN;

            Globals.PageName = Globals.TitleForm;

            //LeftMenuByPTType = eLeftMenuByPTType.NONE;

            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var VM = Globals.GetViewModel<IManagePatientDetails>();

            VM.PageTitle = eHCMSResources.G0525_G1_TTinBN;
            VM.RegistrationType = AllLookupValues.RegistrationType.NGOAI_TRU;

            VM.mNhanBenh_ThongTin_Sua = false;
            VM.mNhanBenh_TheBH_ThemMoi = false;
            VM.mNhanBenh_TheBH_XacNhan = false;
            VM.mNhanBenh_DangKy = false;
            VM.mNhanBenh_TheBH_Sua = false;


            VM.mPatient_TimBN = true;
            VM.mPatient_ThemBN = true;
            VM.mPatient_TimDangKy = false;

            VM.mInfo_CapNhatThongTinBN = true;
            VM.mInfo_XacNhan = false;
            VM.mInfo_XoaThe = false;
            VM.mInfo_XemPhongKham = false;

            Globals.IsAdmission = null;

            Conslt.MainContent = VM;
            (Conslt as Conductor<object>).ActivateItem(VM);

            //Globals.EventAggregator.Publish(new PatientFindByChange { patientFindBy = AllLookupValues.PatientFindBy.NGOAITRU });

        }
        #region menu
        protected override void OnActivate()
        {
            base.OnActivate();
            authorization();
            Globals.EventAggregator.Subscribe(this);
            if (_currentView != null)
            {
                _currentView.ResetMenuColor();
            }
            Globals.IsAdmission = true;
        }

        ILeftMenuView _currentView;
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            _currentView = view as ILeftMenuView;
            if (_currentView != null)
            {
                _currentView.ResetMenuColor();
            }
        }
        private void SetHyperlinkSelectedStyle(Button lnk)
        {
            if (_currentView != null)
            {
                _currentView.ResetMenuColor();
            }
            if (lnk != null)
            {
                lnk.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];
            }
        }
        #endregion

        #region Cận lâm sàng nội trú
        public void PCLLaboratoryRequestCmd_InPt(object source)
        {
            if (!CheckAvailable(AllLookupValues.V_PCLRequestType.NOI_TRU))
            {
                return;
            }
            Globals.TitleForm = string.Format("{0} - {1}", eHCMSResources.P0383_G1_PhYeuCauXetNghiem, eHCMSResources.T3713_G1_NoiTru);
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CLS_PHIEUYEUCAUXETNGHIEM_NT;
            Globals.PageName = Globals.TitleForm;
            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var VM = Globals.GetViewModel<IPatientPCLRequest>();
            Conslt.MainContent = VM;
            (Conslt as Conductor<object>).ActivateItem(VM);
        }
        public void PCLImageRequestCmd_InPt(object source)
        {
            if (!CheckAvailable(AllLookupValues.V_PCLRequestType.NOI_TRU))
            {
                return;
            }
            Globals.TitleForm = string.Format("{0} - {1}", eHCMSResources.P0382_G1_PhYeuCauHA, eHCMSResources.T3713_G1_NoiTru);
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CLS_PHIEUYEUCAUHINHANH_NT;
            Globals.PageName = Globals.TitleForm;
            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var VM = Globals.GetViewModel<IPatientPCLRequestImage>();
            Conslt.MainContent = VM;
            (Conslt as Conductor<object>).ActivateItem(VM);
        }
        public void PCLLaboratoryResultsCmd_InPt(object source)
        {
            //do something after completed PCL request InPt
        }
        public void PCLImagingResultsCmd_InPt(object source)
        {
            //do something after completed PCL request InPt
        }
        public bool CheckAvailable(AllLookupValues.V_PCLRequestType PCLRequestType)
        {
            //if (Globals.PatientAllDetails.PtRegistrationInfo == null)
            //{
            //    return true;
            //}
            //if (PCLRequestType == AllLookupValues.V_PCLRequestType.NGOAI_TRU 
            //    && Globals.PatientAllDetails.PtRegistrationInfo.V_RegistrationType != AllLookupValues.RegistrationType.NGOAI_TRU)
            //{
            //    MessageBox.Show(eHCMSResources.Z0395_G1_DKKgPhaiNgoaiTru);
            //    return false;
            //}
            //if (PCLRequestType == AllLookupValues.V_PCLRequestType.NOI_TRU 
            //    && Globals.PatientAllDetails.PtRegistrationInfo.V_RegistrationType != AllLookupValues.RegistrationType.NOI_TRU)
            //{
            //    MessageBox.Show(eHCMSResources.A0494_G1_Msg_InfoKhTheThaoTac);
            //    return false;
            //}
            return true;
        }
        #endregion
        //==== #001
        private void StaffDeptPresence_In(bool IsUpdateRequiredNumber)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IConsultationModule>();
            var StaffDeptPresenceVm = Globals.GetViewModel<IStaffPresence>();
            StaffDeptPresenceVm.IsUpdateRequiredNumber = IsUpdateRequiredNumber;
            (module as Conductor<object>).ActivateItem(StaffDeptPresenceVm);
            module.MainContent = StaffDeptPresenceVm;
        }
        public void UpdateRequiredNumberCmd(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.TitleForm = eHCMSLanguage.eHCMSResources.Z1924_G1_QLyChiTieuNhanSu;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                StaffDeptPresence_In(true);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        StaffDeptPresence_In(true);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void UpdatePresenceDailyCmd(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.TitleForm = eHCMSLanguage.eHCMSResources.Z1925_G1_QLyTinhHinhKhoaNoiTru;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                StaffDeptPresence_In(false);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        StaffDeptPresence_In(false);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void TransferForm_In(int V_TransferFormType)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IConsultationModule>();
            var TransferFromVm = Globals.GetViewModel<IPaperReferalFull>();
            TransferFromVm.V_TransferFormType = V_TransferFormType;
            (module as Conductor<object>).ActivateItem(TransferFromVm);
            module.MainContent = TransferFromVm;
        }

        public void TransferToCmd(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.TitleForm = "Giấy chuyển đi";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TransferForm_In((int)AllLookupValues.V_TransferFormType.CHUYEN_Di);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TransferForm_In((int)AllLookupValues.V_TransferFormType.CHUYEN_Di);
                        GlobalsNAV.msgb = null;
                    }
                });
            }

        }

        public void TransferFromCmd(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.TitleForm = "Giấy chuyển đến";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TransferForm_In((int)AllLookupValues.V_TransferFormType.CHUYEN_DEN);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TransferForm_In((int)AllLookupValues.V_TransferFormType.CHUYEN_DEN);
                        GlobalsNAV.msgb = null;
                    }
                });
            }

        }
        public void TransferPCLCmd(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.TitleForm = "Giấy chuyển đi làm cận lâm sàng";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TransferForm_In((int)AllLookupValues.V_TransferFormType.CHUYEN_DI_CLS);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TransferForm_In((int)AllLookupValues.V_TransferFormType.CHUYEN_DI_CLS);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //==== #001

        private void HoiChan_In()
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IConsultationModule>();
            var Vm = Globals.GetViewModel<IConsultingDiagnosys>();
            (module as Conductor<object>).ActivateItem(Vm);
            module.MainContent = Vm;
        }

        public void HoiChanCmd(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.TitleForm = "Khám hội chẩn";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                HoiChan_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        HoiChan_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void SurgeryCmd_In(bool IsWaitOnly = false)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IConsultationModule>();
            var Vm = Globals.GetViewModel<IConsultingDiagnosysReport>();
            Vm.IsWaitOnly = IsWaitOnly;
            Vm.TitleForm = Globals.PageName;
            (module as Conductor<object>).ActivateItem(Vm);
            module.MainContent = Vm;
        }
        public void SurgeryCmd(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.TitleForm = eHCMSResources.Z2143_G1_BCBNChiDinhMo.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                SurgeryCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        SurgeryCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void WaitForSurgeryCmd(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.TitleForm = eHCMSResources.Z2182_G1_DanhSachBNDuKienPT.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                SurgeryCmd_In(true);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        SurgeryCmd_In(true);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ReportSurgeryCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var vm = Globals.GetViewModel<IReportSurgeryDept>();

            Conslt.MainContent = vm;
            (Conslt as Conductor<object>).ActivateItem(vm);

        }
        public void ReportSurgeryCmd(object source)
        {
            Globals.TitleForm = "THỐNG KÊ BỆNH NHÂN KHOA PHẪU THUẬT";
            ReportSurgeryCmd_In(source);
        }

        private bool _EnableTestFunction = Globals.ServerConfigSection.CommonItems.EnableTestFunction;
        public bool EnableTestFunction
        {
            get
            {
                return _EnableTestFunction;
            }
            set
            {
                _EnableTestFunction = value;
                NotifyOfPropertyChange(() => EnableTestFunction);
            }
        }

        private bool _PrevSurgeryListReport = true;
        public bool gPrevSurgeryListReport
        {
            get
            {
                return _PrevSurgeryListReport;
            }
            set
            {
                _PrevSurgeryListReport = value;
                NotifyOfPropertyChange(() => gPrevSurgeryListReport);
            }
        }

        private bool _SummarySurgeryListReport = true;
        public bool gSummarySurgeryListReport
        {
            get
            {
                return _SummarySurgeryListReport;
            }
            set
            {
                _SummarySurgeryListReport = value;
                NotifyOfPropertyChange(() => gSummarySurgeryListReport);
            }
        }
    }
}