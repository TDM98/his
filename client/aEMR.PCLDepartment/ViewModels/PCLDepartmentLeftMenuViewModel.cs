using eHCMSLanguage;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof (IPCLDepartmentLeftMenu)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLDepartmentLeftMenuViewModel : Conductor<object>, IPCLDepartmentLeftMenu
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PCLDepartmentLeftMenuViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            Globals.PageName = "";
            Globals.TitleForm = "";
        }
        //Ds phiếu yêu cầu
        private void PCLRequest_Cmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            //var typeInfo = Globals.GetViewModel<ISearchPCLRequest>();

            //var home = Globals.GetViewModel<IHome>();

            //var activeItem = home.ActiveContent;

            
            //var instance = typeInfo as Conductor<object>;

            //Globals.ShowDialog(instance, (o) =>
            //                                 {
            //                                     //lam gi do
            //                                 });

           

            Action<ISearchPCLRequest> onInitDlg = (typeInfo) =>
            {
                typeInfo.LoadData();
            };
            //GlobalsNAV.ShowDialog<ISearchPCLRequest>(onInitDlg);
            GlobalsNAV.ShowDialog<ISearchPCLRequest>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSizeInput(800, 570));

        }

        public void PCLRequest_Cmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K3050_G1_DSPhYC;
            PCLRequest_Cmd_In(source);

            //if (string.IsNullOrEmpty(Globals.PageName))
            //{
            //    PCLRequest_Cmd_In(source);
            //}
            //else if (Globals.PageName != Globals.TitleForm)
            //{
            //    Coroutine.BeginExecute(Globals.DoMessageBox(), null, (o, e) =>
            //    {
            //        if (Globals.msgb.Result == AxMessageBoxResult.Ok)
            //        {
            //            PCLRequest_Cmd_In(source);
            //            Globals.msgb = null;
            //        }
            //    });
            //}
        }
        //Ds phiếu yêu cầu

        //Nhập kết quả xét nghiệm
        private void InputResultClick_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;
            Globals.EventAggregator.Publish(new LoadFormInputResultPCL_ImagingEvent(){});
        }

        public void InputResultClick(object source)
        {
            Globals.TitleForm = eHCMSResources.T2081_G1_KQuaXN;
            InputResultClick_In(source);

            
            //if (string.IsNullOrEmpty(Globals.PageName))
            //{
            //    InputResultClick_In(source);
            //}
            //else if (Globals.PageName != Globals.TitleForm)
            //{
            //    Coroutine.BeginExecute(Globals.DoMessageBox(), null, (o, e) =>
            //    {
            //        if (Globals.msgb.Result == AxMessageBoxResult.Ok)
            //        {
            //            InputResultClick_In(source);
            //            Globals.msgb = null;
            //        }
            //    });
            //}
        }
        //Nhập kết quả xét nghiệm


        //Ds Các lần xét nghiệm
        private void PatientPCLRequest_ByPatientIDV_ParamClick_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;
            var Module = Globals.GetViewModel<IPCLDepartmentContent>();
            var VM = Globals.GetViewModel<IListPCLRequest_ImagingByPatientIDV_Param>();
            Module.MainContent = VM;
            (Module as Conductor<object>).ActivateItem(VM);
        }

        public void PatientPCLRequest_ByPatientIDV_ParamClick(object source)
        {
            Globals.TitleForm = eHCMSResources.K1483_G1_LanXN;
            PatientPCLRequest_ByPatientIDV_ParamClick_In(source);

            //if (string.IsNullOrEmpty(Globals.PageName))
            //{
            //    PatientPCLRequest_ByPatientIDV_ParamClick_In(source);
            //}
            //else if (Globals.PageName != Globals.TitleForm)
            //{
            //    Coroutine.BeginExecute(Globals.DoMessageBox(), null, (o, e) =>
            //    {
            //        if (Globals.msgb.Result == AxMessageBoxResult.Ok)
            //        {
            //            PatientPCLRequest_ByPatientIDV_ParamClick_In(source);
            //            Globals.msgb = null;
            //        }
            //    });
            //}
        }
        //Ds Các lần xét nghiệm


        public void PCLLaboratoryResults_Cmd(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            //var Module = Globals.GetViewModel<IPCLDepartmentContent>();
            //var VM = Globals.GetViewModel<IPatientPCLLaboratoryResult>();

            //Module.MainContent = VM;
            //(Module as Conductor<object>).ActivateItem(VM);
        }

        public void PCLImageCapture_Cmd(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            var Module = Globals.GetViewModel<IPCLDepartmentContent>();
            var ImageCaptureVM = Globals.GetViewModel<IImageCapture_V3>();

            Module.MainContent = ImageCaptureVM;
            (Module as Conductor<object>).ActivateItem(ImageCaptureVM);
        }

        public void PCLImagingResults_Cmd(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            var Conslt = Globals.GetViewModel<IPCLDepartmentContent>();
            var VM = Globals.GetViewModel<IPCLDeptImagingResult>();

            Conslt.MainContent = VM;
            (Conslt as Conductor<object>).ActivateItem(VM);
        }

        public void SieuAmTim_Cmd(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            var Module = Globals.GetViewModel<IPCLDepartmentContent>();
            var VM = Globals.GetViewModel<ISieuAmTim>();

            Module.MainContent = VM;
            (Module as Conductor<object>).ActivateItem(VM);
        }

        public void SieuAmTimThai_Cmd(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            var Module = Globals.GetViewModel<IPCLDepartmentContent>();
            var VM = Globals.GetViewModel<ISieuAmTimThaiHome>();

            Module.MainContent = VM;
            (Module as Conductor<object>).ActivateItem(VM);
        }

        public void SieuAmMachMau_Cmd(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            var Module = Globals.GetViewModel<IPCLDepartmentContent>();
            var VM = Globals.GetViewModel<ISieuAmMachMauHome>();

            Module.MainContent = VM;
            (Module as Conductor<object>).ActivateItem(VM);
        }

        public void SieuAmGangSucDoBu_Cmd(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            var Module = Globals.GetViewModel<IPCLDepartmentContent>();
            var VM = Globals.GetViewModel<ISieuAmTimGangSucDobutamineHome>();

            Module.MainContent = VM;
            (Module as Conductor<object>).ActivateItem(VM);
        }

        public void SieuAmGangSucDipyridamole_Cmd(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            var Module = Globals.GetViewModel<IPCLDepartmentContent>();
            var VM = Globals.GetViewModel<ISieuAmTimGangSucDipyridamoleHome>();

            Module.MainContent = VM;
            (Module as Conductor<object>).ActivateItem(VM);
        }

        public void SieuAmTimQuaThucQuan_Cmd(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            var Module = Globals.GetViewModel<IPCLDepartmentContent>();
            var VM = Globals.GetViewModel<ISieuAmTimQuaThucQuanHome>();

            Module.MainContent = VM;
            (Module as Conductor<object>).ActivateItem(VM);
        }

        private void SieuAmResultTemplate_Cmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;
            var Module = Globals.GetViewModel<IPCLDepartmentContent>();
            var VM = Globals.GetViewModel<ISieuAmResultTemplate>();
            Module.MainContent = VM;
            (Module as Conductor<object>).ActivateItem(VM);
        }

        public void SieuAmResultTemplate_Cmd(object source)
        {
            Globals.TitleForm = eHCMSResources.T2078_G1_KQuaTemplate;
            SieuAmResultTemplate_Cmd_In(source);

            //if (string.IsNullOrEmpty(Globals.PageName))
            //{
            //    SieuAmResultTemplate_Cmd_In(source);
            //}
            //else if (Globals.PageName != Globals.TitleForm)
            //{
            //    Coroutine.BeginExecute(Globals.DoMessageBox(), null, (o, e) =>
            //    {
            //        if (Globals.msgb.Result == AxMessageBoxResult.Ok)
            //        {
            //            SieuAmResultTemplate_Cmd_In(source);
            //            Globals.msgb = null;
            //        }
            //    });
            //}
        }

        private void UltrasoundStatistics_Cmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;
            var Module = Globals.GetViewModel<IPCLDepartmentContent>();
            var VM = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            VM.eItem = ReportName.ULTRASOUND_STATISTICS;

            if (VM.aucHoldConsultDoctor != null)
            {
                VM.aucHoldConsultDoctor.StaffCatType = (long)V_StaffCatType.BacSi;
            }

            VM.RptParameters.HideFindPatient = false;
            Module.MainContent = VM;
            (Module as Conductor<object>).ActivateItem(VM);
        }

        public void UltrasoundStatistics_Cmd(object source)
        {
            Globals.TitleForm = eHCMSResources.T2078_G1_KQuaTemplate;
            UltrasoundStatistics_Cmd_In(source);
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