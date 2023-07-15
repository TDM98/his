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
using aEMR.CommonTasks;
using System.Collections.Generic;
using System.Threading;
using System.Collections.ObjectModel;
using aEMR.ServiceClient;
using aEMR.Common.Collections;
using System.Windows.Input;
using System.Linq;
using eHCMS.Services.Core.Base;

/*
 * 20181118 #001 TBL: BM 0005286: Doi man hinh PatientInfo khong xoa
 * 20181208 #002 TTM: BM 0005401: Fix lỗi die chương trình khi click vào nút refresh.
 */
namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof(IPCLDepartmentTopMenu)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLDepartmentTopMenuViewModel : Conductor<object>, IPCLDepartmentTopMenu
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PCLDepartmentTopMenuViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
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
            Globals.EventAggregator.Publish(new LoadFormInputResultPCL_ImagingEvent() { });
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
            Coroutine.BeginExecute(LoadMenuItems());
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


        //▼====: #002
        #region Checking Account Properties
        private bool _mSAMachMau = true;
        private bool _mSATGSDipy = true;
        private bool _mSATGSDobu = true;
        private bool _mSATMau = true;
        private bool _mSATQuaThucQuan = true;
        private bool _mSATThai = true;
        private bool _mAbUltra = true;

        public bool mSAMachMau
        {
            get
            {
                return _mSAMachMau;
            }
            set
            {
                if (_mSAMachMau == value)
                    return;
                _mSAMachMau = value;
                NotifyOfPropertyChange(() => mSAMachMau);
            }
        }
        public bool mSATGSDipy
        {
            get
            {
                return _mSATGSDipy;
            }
            set
            {
                if (_mSATGSDipy == value)
                    return;
                _mSATGSDipy = value;
                NotifyOfPropertyChange(() => mSATGSDipy);
            }
        }
        public bool mSATGSDobu
        {
            get
            {
                return _mSATGSDobu;
            }
            set
            {
                if (_mSATGSDobu == value)
                    return;
                _mSATGSDobu = value;
                NotifyOfPropertyChange(() => mSATGSDobu);
            }
        }
        public bool mSATMau
        {
            get
            {
                return _mSATMau;
            }
            set
            {
                if (_mSATMau == value)
                    return;
                _mSATMau = value;
                NotifyOfPropertyChange(() => mSATMau);
            }
        }
        public bool mSATQuaThucQuan
        {
            get
            {
                return _mSATQuaThucQuan;
            }
            set
            {
                if (_mSATQuaThucQuan == value)
                    return;
                _mSATQuaThucQuan = value;
                NotifyOfPropertyChange(() => mSATQuaThucQuan);
            }
        }
        public bool mSATThai
        {
            get
            {
                return _mSATThai;
            }
            set
            {
                if (_mSATThai == value)
                    return;
                _mSATThai = value;
                NotifyOfPropertyChange(() => mSATThai);
            }
        }
        public bool mAbUltra
        {
            get
            {
                return _mAbUltra;
            }
            set
            {
                if (_mAbUltra == value)
                    return;
                _mAbUltra = value;
                NotifyOfPropertyChange(() => mAbUltra);
            }
        }
        #endregion
        private ObservableCollection<DataEntities.PCLResultParamImplementations> ObjPCLResultParamImplementations_GetAll_Root;
        private ObservableCollection<BindableMenuItem> _MainMenuItems = new ObservableCollection<BindableMenuItem>();
        private ObservableCollection<PCLExamTypeSubCategory> _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory;
        public ObservableCollection<PCLExamTypeSubCategory> ObjPCLExamTypeSubCategory_ByV_PCLMainCategory
        {
            get => _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory; set
            {
                _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypeSubCategory_ByV_PCLMainCategory);
            }
        }
        public ObservableCollection<BindableMenuItem> MainMenuItems
        {
            get => _MainMenuItems; set
            {
                _MainMenuItems = value;
                NotifyOfPropertyChange(() => MainMenuItems);
            }
        }
        private void PCLResultParamImplementations_GetAll(GenericCoRoutineTask aGenTask)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLResultParamImplementations_GetAll(Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<DataEntities.PCLResultParamImplementations> allItems = null;
                            try
                            {
                                allItems = client.EndPCLResultParamImplementations_GetAll(asyncResult);
                                if (allItems != null)
                                {
                                    if (!Globals.isAccountCheck)
                                    {
                                        ObjPCLResultParamImplementations_GetAll_Root = new ObservableCollection<DataEntities.PCLResultParamImplementations>(allItems);
                                    }
                                    else
                                    {
                                        ObjPCLResultParamImplementations_GetAll_Root = new ObservableCollection<DataEntities.PCLResultParamImplementations>();
                                        foreach (var item in allItems)
                                        {
                                            switch (item.ParamEnum)
                                            {
                                                case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_GANGSUC_Dipyridamole:
                                                    if (mSATGSDipy)
                                                        ObjPCLResultParamImplementations_GetAll_Root.Add(item);
                                                    break;
                                                case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_GANGSUC_Dobutamine:
                                                    if (mSATGSDobu)
                                                        ObjPCLResultParamImplementations_GetAll_Root.Add(item);
                                                    break;
                                                case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_MACHMAU:
                                                    if (mSAMachMau)
                                                        ObjPCLResultParamImplementations_GetAll_Root.Add(item);
                                                    break;
                                                case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_THUCQUAN:
                                                    if (mSATQuaThucQuan)
                                                        ObjPCLResultParamImplementations_GetAll_Root.Add(item);
                                                    break;
                                                case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_TIMMAU:
                                                    if (mSATMau)
                                                        ObjPCLResultParamImplementations_GetAll_Root.Add(item);
                                                    break;
                                                case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_TIMTHAI:
                                                    if (mSATThai)
                                                        ObjPCLResultParamImplementations_GetAll_Root.Add(item);
                                                    break;
                                                case (int)AllLookupValues.PCLResultParamImpID.ABDOMINAL_ULTRASOUND:
                                                    if (mAbUltra)
                                                        ObjPCLResultParamImplementations_GetAll_Root.Add(item);
                                                    break;
                                                case (int)AllLookupValues.PCLResultParamImpID.KHAC:
                                                case (int)AllLookupValues.PCLResultParamImpID.GeneralSurgery:
                                                    ObjPCLResultParamImplementations_GetAll_Root.Add(item);
                                                    break;
                                            }
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                aGenTask.ActionComplete(false);
                            }
                            finally
                            {
                                aGenTask.ActionComplete(true);
                            }
                        }), null);
                    }
                }
                catch
                {
                    aGenTask.ActionComplete(false);
                }
            });
            t.Start();
        }
        public void PCLExamTypeSubCategory_ByV_PCLMainCategory(GenericCoRoutineTask aGenTask, object V_PCLMainCategory)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPCLExamTypeSubCategory_ByV_PCLMainCategory((long)V_PCLMainCategory, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndPCLExamTypeSubCategory_ByV_PCLMainCategory(asyncResult);
                            if (items != null)
                            {
                                ObjPCLExamTypeSubCategory_ByV_PCLMainCategory = items.ToObservableCollection();
                            }
                            foreach (var item in ObjPCLExamTypeSubCategory_ByV_PCLMainCategory)
                            {
                                MainMenuItems.Add(new BindableMenuItem { Text = item.PCLSubCategoryName, PCLExamTypeSubCategoryID = item.PCLExamTypeSubCategoryID });
                            }
                            foreach (var item in MainMenuItems)
                            {
                                item.Childrens = ObjPCLResultParamImplementations_GetAll_Root.Where(x => x.PCLExamTypeSubCategoryID == item.PCLExamTypeSubCategoryID).Select(x => new BindableMenuItem { Text = x.ParamName, CommandParams = x, Command = MenuItemClickedCommand }).ToObservableCollection();
                            }
                            NotifyOfPropertyChange(() => MainMenuItems);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                            aGenTask.ActionComplete(false);
                        }
                        finally
                        {
                            aGenTask.ActionComplete(true);
                        }
                    }), null);
                }
            });
            t.Start();
        }
        private IEnumerator<IResult> LoadMenuItems()
        {
            this.ShowBusyIndicator();
            yield return GenericCoRoutineTask.StartTask(PCLResultParamImplementations_GetAll);
            yield return GenericCoRoutineTask.StartTask(PCLExamTypeSubCategory_ByV_PCLMainCategory, (long)AllLookupValues.V_PCLMainCategory.Imaging);
            this.HideBusyIndicator();
        }
        Menu MainMenu;
        public void MainMenu_Loaded(object sender, RoutedEventArgs e)
        {
            if (MainMenu == null)
            {
                MainMenu = sender as Menu;
            }
        }
        public ICommand MenuItemClickedCommand
        {
            get
            {
                RelayCommand mRelayCommand = new RelayCommand(OnMenuItemClicked);
                return mRelayCommand;
            }
        }
        public void OnMenuItemClicked(object oParam)
        {
            //==== #001 ====
            //TBL: Sau khi chon cls thi phai clear global di neu khong thi thong tin cua BN cu van se hien thi tren man hinh, lam cho khi qua man hinh cls khac van se luu duoc.
            ClearGlobalsData();
            //==== #001 ====
            if (oParam == null || !(oParam is PCLResultParamImplementations)) return;
            PCLResultParamImplementations mMenuItem = oParam as PCLResultParamImplementations;
            Globals.PCLDepartment.ObjV_PCLMainCategory = new Lookup { LookupID = (long)AllLookupValues.V_PCLMainCategory.Imaging };
            if (Globals.PCLDepartment.ObjPCLResultParamImpID == null) Globals.PCLDepartment.ObjPCLResultParamImpID = new PCLResultParamImplementations();
            Globals.PCLDepartment.ObjPCLResultParamImpID.ParamEnum = mMenuItem.ParamEnum;
            Globals.PCLDepartment.ObjPCLResultParamImpID.PCLResultParamImpID = mMenuItem.PCLResultParamImpID;

            Globals.PCLDepartment.ObjPCLResultParamImpID.ParamName = mMenuItem.ParamName;
            //▼====== #002: 
            if (Globals.PCLDepartment.ObjPCLExamTypeSubCategoryID == null)
            {
                Globals.PCLDepartment.ObjPCLExamTypeSubCategoryID = new PCLExamTypeSubCategory();
            }
            Globals.PCLDepartment.ObjPCLExamTypeSubCategoryID.PCLExamTypeSubCategoryID =(long) mMenuItem.PCLExamTypeSubCategoryID;
            //▲====== #002

            Globals.EventAggregator.Publish(new LoadFormInputResultPCL_ImagingEvent());
        }
        //▲====: #002
        private void ClearGlobalsData()
        {
            Globals.PatientPCLRequest_Result = null;
        }
    }
    public class RelayCommand : ICommand
    {
        readonly Action<Object> mExecute;
        readonly Predicate<Object> mCanExecute;

        public RelayCommand(Action<Object> execute)
            : this(execute, null)
        {

        }

        public RelayCommand(Action<Object> execute, Predicate<Object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("Execute");
            mExecute = execute;
            mCanExecute = canExecute;
        }

        #region ICommand Members
        public bool CanExecute(object parameter)
        {
            return mCanExecute == null ? true : mCanExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            mExecute(parameter);
        }
        #endregion
    }
    public class BindableMenuItem : NotifyChangedBase
    {
        public object CommandParams { get; set; }
        public long PCLExamTypeSubCategoryID { get; set; }
        public string Text { get; set; }
        public ObservableCollection<BindableMenuItem> _Childrens;
        public ObservableCollection<BindableMenuItem> Childrens
        {
            get => _Childrens; set
            {
                _Childrens = value;
                RaisePropertyChanged("Childrens");
            }
        }
        public ICommand Command { get; set; }
    }
}