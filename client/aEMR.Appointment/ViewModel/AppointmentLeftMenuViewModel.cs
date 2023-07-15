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

namespace aEMR.Appointment.ViewModels
{
    [Export(typeof(IAppointmentLeftMenu))]
    public class AppointmentLeftMenuViewModel : Conductor<object>, IAppointmentLeftMenu
        , IHandle<LocationSelected>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public AppointmentLeftMenuViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
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

        private void AppointmentManagementCmd_In(object source)
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
    }
}