using eHCMSLanguage;
using System.ComponentModel.Composition;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using System.Windows.Controls;
using DataEntities;
/*
* 20200409 #001 TNHX: [] add report BC_BNTaiKhamBenhManTinh
* 20210609 #002 DatTB: Báo cáo bệnh nhân khám bệnh BC_BenhNhanKhamBenh
* 20210615 #003 DatTB: Báo Cáo Bệnh Nhân Hẹn Tái Khám Bệnh Đặc Trưng BC_BNHenTaiKhamBenhDacTrung
* 20230625 #004 QTD:   Thêm màn hình danh sách hẹn xét nghiệm
*/
namespace aEMR.Appointment.ViewModels
{
    [Export(typeof(IAppointmentTopMenu))]
    public class AppointmentTopMenuViewModel : Conductor<object>, IAppointmentTopMenu
        , IHandle<LocationSelected>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public AppointmentTopMenuViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            Globals.PageName = "";
            Globals.TitleForm = "";
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            //Globals.EventAggregator.Subscribe(this);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }

        private void AppointmentManagementCmd_In(object source, bool IsShowSearchRegistrationButton = false)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;
            //var loginVm = Globals.GetViewModel<ILogin>();
            //if (loginVm.DeptLocation == null) //Chưa chọn phòng => Yêu cầu Chọn phòng.
            //{
            //    var locationVm = Globals.GetViewModel<ISelectLocation>();
            //    locationVm.ItemActivated = Globals.GetViewModel<IPatientAppointments>();
            //    Globals.ShowDialog(locationVm as Conductor<object>);
            //}
            //else
            {
                var regModule = Globals.GetViewModel<IAppointmentModule>();
                var regVm = Globals.GetViewModel<IPatientAppointments>();
                regVm.IsPCLBookingView = IsShowSearchRegistrationButton;
                regVm.IsShowSearchRegistrationButton = IsShowSearchRegistrationButton;
                regModule.MainContent = regVm;
                ((Conductor<object>)regModule).ActivateItem(regVm);
            }
        }
        public void AppointmentManagementCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Q0469_G1_QuanLyHenBenh;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                AppointmentManagementCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        AppointmentManagementCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void AppointmentCollectionCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0452_G1_DSHenBenh;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                AppointmentManagementCmd_In(source, true);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        AppointmentManagementCmd_In(source, true);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void AppointmentListingCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);

            Globals.PageName = Globals.TitleForm;

            //var loginVm = Globals.GetViewModel<ILogin>();
            //if (loginVm.DeptLocation == null) //Chưa chọn phòng => Yêu cầu Chọn phòng.
            //{
            //    var locationVm = Globals.GetViewModel<ISelectLocation>();
            //    locationVm.ItemActivated = Globals.GetViewModel<ISearchAppointments>();
            //    Globals.ShowDialog(locationVm as Conductor<object>);
            //}
            //else
            {
                var regModule = Globals.GetViewModel<IAppointmentModule>();
                var regVm = Globals.GetViewModel<ISearchAppointments>();
                regVm.TitleForm = eHCMSResources.Z0452_G1_DSHenBenh.ToUpper();
                regModule.MainContent = regVm;
                ((Conductor<object>)regModule).ActivateItem(regVm);
            }

        }

        public void AppointmentListingCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0452_G1_DSHenBenh.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                AppointmentListingCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        AppointmentListingCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }


        private void AppointmentTotalCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);

            Globals.PageName = Globals.TitleForm;

            var regModule = Globals.GetViewModel<IAppointmentModule>();
            var regVm = Globals.GetViewModel<ISearchAppointments>();
            regVm.IsStaff = true;
            regVm.TitleForm = eHCMSResources.Z0457_G1_TKeDSHenBenh.ToUpper();
            regVm.IsConsultation = false;

            regModule.MainContent = regVm;
            ((Conductor<object>)regModule).ActivateItem(regVm);
            

        }

        public void AppointmentTotalCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0457_G1_TKeDSHenBenh.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                AppointmentTotalCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        AppointmentTotalCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void AppointmentDetailCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);

            Globals.PageName = Globals.TitleForm;

            var regModule = Globals.GetViewModel<IAppointmentModule>();
            var regVm = Globals.GetViewModel<IPatientApptServicesDetails>();
            regVm.TitleForm = eHCMSResources.Z0464_G1_DSCTietDVHen.ToUpper();
            

            regModule.MainContent = regVm;
            ((Conductor<object>)regModule).ActivateItem(regVm);

        }

        public void AppointmentDetailCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0464_G1_DSCTietDVHen.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                AppointmentDetailCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        AppointmentDetailCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void AppointmentFromDiagCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);

            Globals.PageName = Globals.TitleForm;

            var regModule = Globals.GetViewModel<IAppointmentModule>();
            var regVm = Globals.GetViewModel<IAppointmentsFromDiagnosic>();
           
            regVm.TitleForm = eHCMSResources.T1463_G1_HBenhTuDSKham.ToUpper();
           

            regModule.MainContent = regVm;
            ((Conductor<object>)regModule).ActivateItem(regVm);


        }

        public void AppointmentFromDiagCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.T1463_G1_HBenhTuDSKham.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                AppointmentFromDiagCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        AppointmentFromDiagCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //PCLExamTargetCmd
        private void PCLExamTargetCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            var regModule = Globals.GetViewModel<IAppointmentModule>();
            var regVm = Globals.GetViewModel<ICommonPCLExamTarget>();

            regModule.MainContent = regVm;
            ((Conductor<object>)regModule).ActivateItem(regVm);
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

        public void Handle(LocationSelected message)
        {
            if (message != null && message.DeptLocation != null)
            {
                var regModule = Globals.GetViewModel<IAppointmentModule>();
                if (message.ItemActivated == null)
                {
                    //không làm gì hết vì chưa chọn phòng khám
                }
                else
                {
                    regModule.MainContent = message.ItemActivated;
                    (regModule as Conductor<object>).ActivateItem(message.ItemActivated);
                }

            }
        }

        private void HealthExaminationRecordCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;
            var mModule = Globals.GetViewModel<IAppointmentModule>();
            var mView = Globals.GetViewModel<IHealthExaminationRecord>();
            mModule.MainContent = mView;
            ((Conductor<object>)mModule).ActivateItem(mView);
        }
        public void HealthExaminationRecordCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2670_G1_HopDongKhamSucKhoe;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                HealthExaminationRecordCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        HealthExaminationRecordCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void PaymentHealthExaminationRecordCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;
            var mModule = Globals.GetViewModel<IAppointmentModule>();
            var mView = Globals.GetViewModel<IHealthExaminationRecord>();
            mView.IsPayment = true;
            mModule.MainContent = mView;
            ((Conductor<object>)mModule).ActivateItem(mView);
        }
        public void PaymentHealthExaminationRecordCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2787_G1_ThanhToanHopDong;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                PaymentHealthExaminationRecordCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        PaymentHealthExaminationRecordCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void HospitalClientCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;
            var mModule = Globals.GetViewModel<IAppointmentModule>();
            var mView = Globals.GetViewModel<IHospitalClient>();
            mModule.MainContent = mView;
            ((Conductor<object>)mModule).ActivateItem(mView);
        }
        public void HospitalClientCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2790_G1_KhachHangKhamSK;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                HospitalClientCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        HospitalClientCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //▼====: #001
        private void BCBNTaiKhamBenhManTinhCmd_In(object source, bool IsShowSearchRegistrationButton = false)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IAppointmentModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BC_BNTaiKhamBenhManTinh;
            reportVm.ShowAges = true;
            reportVm.RptParameters.HideFindPatient = false;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void BCBNTaiKhamBenhManTinhCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3009_G1_BCBNTaiKhamBenhManTinh;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCBNTaiKhamBenhManTinhCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCBNTaiKhamBenhManTinhCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #001

        //▼====: #002

        private void BCBenhNhanKhamBenhCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IAppointmentModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BC_BenhNhanKhamBenh;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.isAllStaff = false;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void BCBenhNhanKhamBenhCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3115_G1_BC_BenhNhanKhamBenh;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCBenhNhanKhamBenhCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCBenhNhanKhamBenhCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #002

        //▼====: #003

        private void BCBNHenTaiKhamBenhDacTrungCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IAppointmentModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BC_BNHenTaiKhamBenhDacTrung;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.isAllStaff = false;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void BCBNHenTaiKhamBenhDacTrungCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3116_G1_BC_BNHenTaiKhamBenhDacTrung;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCBNHenTaiKhamBenhDacTrungCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCBNHenTaiKhamBenhDacTrungCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #003

        public void BCPhatHanhTheKCBCmd(object source)
        {
            Globals.TitleForm = "Báo cáo Phát hành thẻ Khám chữa bệnh";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCPhatHanhTheKCBCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCPhatHanhTheKCBCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void BCPhatHanhTheKCBCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IAppointmentModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.XRpt_PhatHanhTheKCB;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.isAllStaff = false;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        public void BCThongKeSoLuongTheCmd(object source)
        {
            Globals.TitleForm = "BÁO CÁO THỐNG KÊ SỐ LƯỢNG THẺ KHÁM CHỮA BỆNH";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCThongKeSoLuongTheCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCThongKeSoLuongTheCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void BCThongKeSoLuongTheCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IAppointmentModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.XRpt_ThongKeSoLuongThe;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.IsEnabledToDatePicker = false;
            reportVm.isAllStaff = false;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        #region menu
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            if (_currentView != null)
            {
                _currentView.ResetMenuColor();
            }
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

        //▼====: #004
        private void AppointmentLabListingCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;
            {
                var regModule = Globals.GetViewModel<IAppointmentModule>();
                var regVm = Globals.GetViewModel<IAppointmentsLab>();
                //regVm.TitleForm = eHCMSResources.Z0452_G1_DSHenBenh.ToUpper();
                regModule.MainContent = regVm;
                ((Conductor<object>)regModule).ActivateItem(regVm);
            }
        }

        public void AppointmentLabListingCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0452_G1_DSHenBenh.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                AppointmentLabListingCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        AppointmentLabListingCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ReportAppointmentLabCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IAppointmentModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.XRpt_AppointmentLab;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.isAllStaff = false;
            reportVm.ShowSMSStatus = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void ReportAppointmentLabCmd(object source)
        {
            Globals.TitleForm = "Báo Cáo Danh Sách Bệnh Nhân Gửi SMS";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ReportAppointmentLabCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportAppointmentLabCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #004
    }
}