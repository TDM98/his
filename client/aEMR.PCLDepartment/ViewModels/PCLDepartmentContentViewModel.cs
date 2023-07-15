using System.ComponentModel.Composition;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Common.BaseModel;

// 20182508 #001 TTM: 

namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof(IPCLDepartmentContent)), PartCreationPolicy(CreationPolicy.Shared)]
    public class PCLDepartmentContentViewModel : ViewModelBase, IPCLDepartmentContent
        //, IHandle<ChangePCLDepartmentEvent>
        //#001: Vì đã đc HomeView nhận sự kiện InitialPCLImage_Step1_Event để load hàm OnActive (trong đó bao gồm Hàm LoadContentByParaEnum() không nhất thiết phải gọi Chụp sự kiện
        //Step2 để load lại LoadContentByParaEnum() 1 lần nữa làm gì).
        //, IHandle<InitialPCLImage_Step2_Event>
        , IHandle<LoadFormInputResultPCL_ImagingEvent>
    {
        private object _mainContent;
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PCLDepartmentContentViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            CreateSubVM();
            eventAggregator.Subscribe(this);
        }

        private void CreateSubVM()
        {
            VideoImageCaptureContent = Globals.GetViewModel<IImageCapture_V4>();
        }


        #region IHandle<ChangePCLDepartmentEvent> Members

        public void Handle(InitialPCLImage_Step2_Event message)
        {
            if (GetView() != null)
            {
                LoadContentByParaEnum();
            }
        }

        #endregion

        #region IPCLDepartmentContent Members

        
        public object VideoImageCaptureContent { get; set; }


        public object MainContent
        {
            get { return _mainContent; }
            set
            {
                _mainContent = value;
                NotifyOfPropertyChange(() => MainContent);
            }
        }

        #endregion

        #region Load Content BY ENUM

        private object CurrentActiveImagingVM { get; set; }
        private void LoadContentByParaEnum()
        {
            if (Globals.PCLDepartment.ObjV_PCLMainCategory == null || Globals.PCLDepartment.ObjPCLResultParamImpID == null)
            {
                return;
            }
            if (Globals.PCLDepartment.ObjV_PCLMainCategory.LookupID != (long)AllLookupValues.V_PCLMainCategory.Imaging)
            {
                return;
            }
            if (CurrentActiveImagingVM != null)
            {
                (CurrentActiveImagingVM as ViewModelBase).Deactivate(true);
                CurrentActiveImagingVM = null;
            }
            switch (Globals.PCLDepartment.ObjPCLResultParamImpID.ParamEnum)
            {
                case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_TIMMAU:
                                   
                    CurrentActiveImagingVM = Globals.GetViewModel<ISieuAmTim>();
                    ((ISieuAmTim)CurrentActiveImagingVM).SetVideoImageCaptureSourceVM(VideoImageCaptureContent);
                    break;
                case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_MACHMAU:
                    
                    CurrentActiveImagingVM = Globals.GetViewModel<ISieuAmMachMauHome>();
                    ((ISieuAmMachMauHome)CurrentActiveImagingVM).SetVideoImageCaptureSourceVM(VideoImageCaptureContent);
                    break;                    
                case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_GANGSUC_Dipyridamole:
                    
                    CurrentActiveImagingVM = Globals.GetViewModel<ISieuAmTimGangSucDipyridamoleHome>();
                    ((ISieuAmTimGangSucDipyridamoleHome)CurrentActiveImagingVM).SetVideoImageCaptureSourceVM(VideoImageCaptureContent);
                    break;                    
                case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_GANGSUC_Dobutamine:
                    
                    CurrentActiveImagingVM = Globals.GetViewModel<ISieuAmTimGangSucDobutamineHome>();
                    ((ISieuAmTimGangSucDobutamineHome)CurrentActiveImagingVM).SetVideoImageCaptureSourceVM(VideoImageCaptureContent);
                    break;                    
                case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_TIMTHAI:
                    
                    CurrentActiveImagingVM = Globals.GetViewModel<ISieuAmTimThaiHome>();
                    ((ISieuAmTimThaiHome)CurrentActiveImagingVM).SetVideoImageCaptureSourceVM(VideoImageCaptureContent);
                    break;                    
                case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_THUCQUAN:
                    
                    CurrentActiveImagingVM = Globals.GetViewModel<ISieuAmTimQuaThucQuanHome>();
                    ((ISieuAmTimQuaThucQuanHome)CurrentActiveImagingVM).SetVideoImageCaptureSourceVM(VideoImageCaptureContent);
                    break;                    
                case (int)AllLookupValues.PCLResultParamImpID.ABDOMINAL_ULTRASOUND:
                    
                    CurrentActiveImagingVM = Globals.GetViewModel<IAbdominalUltrasoundMain>();
                    ((IAbdominalUltrasoundMain)CurrentActiveImagingVM).SetVideoImageCaptureSourceVM(VideoImageCaptureContent);
                    break;                    
                default:
                
                    CurrentActiveImagingVM = Globals.GetViewModel<IPCLDeptImagingResult>();
                    ((IPCLDeptImagingResult)CurrentActiveImagingVM).SetVideoImageCaptureSourceVM(VideoImageCaptureContent);
                    break;
            }

            if (CurrentActiveImagingVM != null)
            {
                MainContent = CurrentActiveImagingVM;
                ActivateItem(CurrentActiveImagingVM);
            }

        }


    #endregion

        protected override void OnActivate()
        {
            base.OnActivate();
            
            ActivateItem(VideoImageCaptureContent);
            
            FormLoad();
            LoadContentByParaEnum();
        }

        protected override void OnDeactivate(bool close)
        {
            DeactivateItem(MainContent as Conductor<object>, true);
            MainContent = null;
            base.OnDeactivate(close);
        }

        public void FormLoad()
        {
            //Khi khoi tao module thi load menu ben trai luon.
            var shell = Globals.GetViewModel<IHome>();
            //var leftMenu = Globals.GetViewModel<IPCLDepartmentLeftMenu>();
            //var UCPCLDepartmentOutstandingTaskView = Globals.GetViewModel<IPCLDepartmentOutstandingTask>();

            if (Globals.PatientFindBy_ForImaging == null)
                Globals.PatientFindBy_ForImaging = AllLookupValues.PatientFindBy.NGOAITRU;

            var leftMenu = Globals.GetViewModel<IPCLDepartmentLeftMenu>();
            var topMenu = Globals.GetViewModel<IPCLDepartmentTopMenu>();
            UCPCLDepartmentOutstandingTaskView = Globals.GetViewModel<IPCLDepartmentOutstandingTask>();

            //UCPCLDepartmentOutstandingTaskView.PatientFindBy = Globals.PatientFindBy_ForImaging.Value;

            shell.LeftMenu = leftMenu;
            shell.TopMenuItems = topMenu;
            (shell as Conductor<object>).ActivateItem(leftMenu);
            (shell as Conductor<object>).ActivateItem(topMenu);

            shell.OutstandingTaskContent = UCPCLDepartmentOutstandingTaskView;
            shell.IsExpandOST = true;
            (shell as Conductor<object>).ActivateItem(UCPCLDepartmentOutstandingTaskView);

            (shell as Conductor<object>).ActivateItem(this);           
        }
        private IPCLDepartmentOutstandingTask UCPCLDepartmentOutstandingTaskView { get; set; }
        public void Handle(LoadFormInputResultPCL_ImagingEvent message)
        {
            if (message != null)
            {
                LoadContentByParaEnum();
                UCPCLDepartmentOutstandingTaskView.ClearObjPatientPCLRequest();
            }
        }
    }
}