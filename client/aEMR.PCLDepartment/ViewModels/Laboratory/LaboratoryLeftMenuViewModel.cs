using eHCMSLanguage;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;

namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof (ILaboratoryLeftMenu)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class LaboratoryLeftMenuViewModel : Conductor<object>, ILaboratoryLeftMenu
         , IHandle<LocationSelected>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public LaboratoryLeftMenuViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            
            Globals.PageName = "";
            Globals.TitleForm = "";
            eventAggregator.Subscribe(this);
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

        private void PCLRequest_Cmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

           // var loginVm = Globals.GetViewModel<ILogin>();
            //khong can vi khi dang nhap da chon phong roi,neu tim ko dc thi nguoi dung chon lai phong la dc
            //if (loginVm.DeptLocation == null
            //    ||Globals.V_DeptTypeOperation != V_DeptTypeOperation.KhoaCanLamSang) //Chưa chọn phòng => Yêu cầu Chọn phòng.
            //{
            //    var locationVm = Globals.GetViewModel<ISelectLocation>();
            //    locationVm.V_DeptTypeOperation = V_DeptTypeOperation.KhoaCanLamSang;
            //    Globals.V_DeptTypeOperation = V_DeptTypeOperation.KhoaCanLamSang;
            //    locationVm.ItemActivated = Globals.GetViewModel<ILaboratoryResultHome>();
            //    Globals.ShowDialog(locationVm as Conductor<object>);
            //}
            //else
            {
                var regModule = Globals.GetViewModel<ILaboratoryHome>();
                var regVm = Globals.GetViewModel<ILaboratoryResultHome>();
                regModule.MainContent = regVm;
                (regModule as Conductor<object>).ActivateItem(regVm);

                KhoiTaoPCLDepartmentLAB();

                //Load OutStandingstask
                var shell = Globals.GetViewModel<IHome>();
                //var UCPCLDepartmentOutstandingTaskView = Globals.GetViewModel<IPCLDepartmentOutstandingTask>();
                var UCPCLDepartmentOutstandingTaskView = Globals.GetViewModel<ILaboratoryResultList>();
                

                if(Globals.PatientFindBy_ForLab == null)
                    Globals.PatientFindBy_ForLab = AllLookupValues.PatientFindBy.NGOAITRU;

                //UCPCLDepartmentOutstandingTaskView.PatientFindBy = Globals.PatientFindBy_ForLab.Value;

                shell.OutstandingTaskContent = UCPCLDepartmentOutstandingTaskView;
                shell.IsExpandOST = true;
                (shell as Conductor<object>).ActivateItem(UCPCLDepartmentOutstandingTaskView);
            }
        }

        public void PCLRequest_Cmd(object source)
        {
            //do chi co mot link nen cho no refresh lai
            Globals.TitleForm = eHCMSResources.G2600_G1_XN;
            PCLRequest_Cmd_In(source);
            //nhieu hon se unmark lại chỗ này

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

        public void Handle(LocationSelected message)
        {
            if (message != null && message.DeptLocation != null)
            {
                KhoiTaoPCLDepartmentLAB();

                var regModule = Globals.GetViewModel<ILaboratoryHome>();
                if (message.ItemActivated == null)
                {
                    //không làm gì hết vì chưa có load Item nào lên, khi đó dựa vào chọn phòng ở Top sẽ load lại
                    var locationVm = Globals.GetViewModel<ISelectLocation>();
                    locationVm.ItemActivated = Globals.GetViewModel<ILaboratoryResultHome>();
                    locationVm.V_DeptTypeOperation = V_DeptTypeOperation.KhoaCanLamSang;
                    Globals.V_DeptTypeOperation = V_DeptTypeOperation.KhoaCanLamSang;
                    var Module = Globals.GetViewModel<ILaboratoryHome>();
                    var VM = Globals.GetViewModel<ILaboratoryResultHome>();

                    Module.MainContent = VM;
                    (Module as Conductor<object>).ActivateItem(VM);


                    //Load OutStandingstask
                    var shell = Globals.GetViewModel<IHome>();
                    //var UCPCLDepartmentOutstandingTaskView = Globals.GetViewModel<IPCLDepartmentOutstandingTask>();
                    var UCPCLDepartmentOutstandingTaskView = Globals.GetViewModel<ILaboratoryResultList>();


                    
                    if (Globals.PatientFindBy_ForLab==null)
                        Globals.PatientFindBy_ForLab= AllLookupValues.PatientFindBy.NGOAITRU;
                    
                    //UCPCLDepartmentOutstandingTaskView.PatientFindBy = Globals.PatientFindBy_ForLab.Value;

                    shell.OutstandingTaskContent = UCPCLDepartmentOutstandingTaskView;
                    shell.IsExpandOST = true;
                    (shell as Conductor<object>).ActivateItem(UCPCLDepartmentOutstandingTaskView);


                }
                else
                {

                    regModule.MainContent = message.ItemActivated;
                    (regModule as Conductor<object>).ActivateItem(message.ItemActivated);

                    //Load OutStandingstask
                    var shell = Globals.GetViewModel<IHome>();
                    //var UCPCLDepartmentOutstandingTaskView = Globals.GetViewModel<IPCLDepartmentOutstandingTask>();
                    var UCPCLDepartmentOutstandingTaskView = Globals.GetViewModel<ILaboratoryResultList>();
                    shell.OutstandingTaskContent = UCPCLDepartmentOutstandingTaskView;
                    shell.IsExpandOST = true;
                    if (Globals.PatientFindBy_ForLab == null)
                        Globals.PatientFindBy_ForLab = AllLookupValues.PatientFindBy.NGOAITRU;

                    //UCPCLDepartmentOutstandingTaskView.PatientFindBy =Globals.PatientFindBy_ForLab.Value;

                    (shell as Conductor<object>).ActivateItem(UCPCLDepartmentOutstandingTaskView);
                }
            }
        }

        private void KhoiTaoPCLDepartmentLAB()
        {
            Globals.PCLDepartment.ObjPCLExamTypeLocationsDeptLocationID = Globals.DeptLocation;
            Globals.PCLDepartment.ObjV_PCLMainCategory = new Lookup();
            Globals.PCLDepartment.ObjV_PCLMainCategory.LookupID = (long)AllLookupValues.V_PCLMainCategory.Laboratory;
            Globals.PCLDepartment.ObjPCLExamTypeSubCategoryID = new PCLExamTypeSubCategory();
            Globals.PCLDepartment.ObjPCLResultParamImpID = new PCLResultParamImplementations();

            //Globals.PatientAllDetails.PatientInfo = null;

            ////Show Info xóa thông tin BN trước đó
            //Globals.EventAggregator.Publish(new ShowPatientInfo<Patient, PatientRegistration, PatientRegistrationDetail>() { Pt = null, PtReg = null, PtRegDetail = null });
            ////Show Info xóa thông tin BN trước đó
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