using eHCMSLanguage;
using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using aEMR.Common;
using aEMR.ServiceClient;
using System.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.ViewModels
{
    //[Export(typeof(IHome)),PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(IHome))]
    public class HomeViewModel : Conductor<object>, IHome
        //, IHandle<ChangePCLDepartmentEvent>
        , IHandle<InitialPCLImage_Step1_Event>
        //, IHandle<PatientFindByChange>
    {
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                NotifyOfPropertyChange(() => IsLoading);
            }
        }

        public void BusyIndicator(bool bShow)
        {
            IsLoading = bShow;
        }

        private AllLookupValues.PatientFindBy _PatientFindBy = (long)AllLookupValues.PatientFindBy.NGOAITRU;
        public AllLookupValues.PatientFindBy PatientFindBy
        {
            get { return _PatientFindBy; }
            set
            {
                _PatientFindBy = value;
                NotifyOfPropertyChange(() => PatientFindBy);
            }
        }
        IEventAggregator _eventArg;
        INavigationService _navigationService;
        IWindsorContainer _container;
        [ImportingConstructor]
        public HomeViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            _eventArg = eventArg;
            _navigationService = navigationService;
            _container = container;
            _eventArg.Subscribe(this);

            authorization();
        }

        public void ResetDefault()
        {
            Globals.HIRegistrationForm = "";

        }

        protected override void OnActivate()
        {
            base.OnActivate();
            authorization();
            ActiveContent = null;
            LeftMenu = null;
            //==== 20161115 CMN Begin: Fix load PCL Module
            _eventArg.Subscribe(this);
            //==== 20161115 CMN End.
        }
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                resetAdmin();
                return;
            }
            bVatu = Globals.CheckModule(Globals.listRefModule, (int)eModules.mResources);
            bConfigurationMgnt = Globals.CheckModule(Globals.listRefModule, (int)eModules.mConfiguration_Management);
            bConsultations = Globals.CheckModule(Globals.listRefModule, (int)eModules.mConsultation);
            bCLSLaboratory = Globals.CheckModule(Globals.listRefModule, (int)eModules.mCLSLaboratory);
            bCLSImaging = Globals.CheckModule(Globals.listRefModule, (int)eModules.mCLSImaging);
            bPharmacies = Globals.CheckModule(Globals.listRefModule, (int)eModules.mPharmacy);
            bRegisterMenuItemCmd = Globals.CheckModule(Globals.listRefModule, (int)eModules.mPatient);
            bDrugDept = Globals.CheckModule(Globals.listRefModule, (int)eModules.mKhoaDuoc);
            bAppointmentMenuItemCmd = Globals.CheckModule(Globals.listRefModule, (int)eModules.mAppointment_System);
            bTransactionCmd = Globals.CheckModule(Globals.listRefModule, (int)eModules.mTransaction_Management) || Globals.CheckModule(Globals.listRefModule, (int)eModules.mYVu_Management);
            bUserAccountCmd = Globals.CheckModule(Globals.listRefModule, (int)eModules.mUserAccount);
            bClinicManagementCmd = Globals.CheckModule(Globals.listRefModule, (int)eModules.mClinicManagement);
            bStoreDeptCmd = Globals.CheckModule(Globals.listRefModule, (int)eModules.mKhoPhong);
            bSystemConfigCmd = Globals.CheckModule(Globals.listRefModule, (int)eModules.mSystem_Management);
        }

        public void resetAdmin()
        {
            bVatu = true;
            bConfigurationMgnt = true;
            bConsultations = true;
            bCLSLaboratory = true;
            bPharmacies = true;
            bRegisterMenuItemCmd = true;
            bDrugDept = true;
            bAppointmentMenuItemCmd = true;
            bTransactionCmd = true;
            bUserAccountCmd = true;
            bClinicManagementCmd = true;

        }
        private object _activeContent;
        public object ActiveContent
        {
            get { return _activeContent; }
            set
            {
                _activeContent = value;
                NotifyOfPropertyChange(() => ActiveContent);
                ActivateItem(_activeContent);
            }
        }

        private object _outstandingTaskContent;
        public object OutstandingTaskContent
        {
            get { return _outstandingTaskContent; }
            set
            {
                _outstandingTaskContent = value;
                NotifyOfPropertyChange(() => OutstandingTaskContent);
            }
        }

        private bool _bVatu = true;
        private bool _bConfigurationMgnt = true;
        private bool _bConsultations = true;
        private bool _bCLSLaboratory = true;
        private bool _bCLSImaging = true;
        private bool _bPharmacies = true;
        private bool _bRegisterMenuItemCmd = true;
        private bool _bDrugDept = true;
        private bool _bAppointmentMenuItemCmd = true;
        private bool _bTransactionCmd = true;
        private bool _bUserAccountCmd = true;
        private bool _bClinicManagementCmd = true;
        private bool _bStoreDeptCmd = true;
        private bool _bSystemConfigCmd = true;

        public bool bVatu
        {
            get
            {
                return _bVatu;
            }
            set
            {
                if (_bVatu == value)
                    return;
                _bVatu = value;
                NotifyOfPropertyChange(() => bVatu);
            }
        }
        public bool bConfigurationMgnt
        {
            get
            {
                return _bConfigurationMgnt;
            }
            set
            {
                if (_bConfigurationMgnt == value)
                    return;
                _bConfigurationMgnt = value;
                NotifyOfPropertyChange(() => bConfigurationMgnt);
            }
        }
        public bool bConsultations
        {
            get
            {
                return _bConsultations;
            }
            set
            {
                if (_bConsultations == value)
                    return;
                _bConsultations = value;
                NotifyOfPropertyChange(() => bConsultations);
            }
        }
        public bool bCLSLaboratory
        {
            get
            {
                return _bCLSLaboratory;
            }
            set
            {
                if (_bCLSLaboratory == value)
                    return;
                _bCLSLaboratory = value;
                NotifyOfPropertyChange(() => bCLSLaboratory);
            }
        }
        public bool bCLSImaging
        {
            get
            {
                return _bCLSImaging;
            }
            set
            {
                if (_bCLSImaging == value)
                    return;
                _bCLSImaging = value;
            }
        }
        public bool bPharmacies
        {
            get
            {
                return _bPharmacies;
            }
            set
            {
                if (_bPharmacies == value)
                    return;
                _bPharmacies = value;
                NotifyOfPropertyChange(() => bPharmacies);
            }
        }
        public bool bRegisterMenuItemCmd
        {
            get
            {
                return _bRegisterMenuItemCmd;
            }
            set
            {
                if (_bRegisterMenuItemCmd == value)
                    return;
                _bRegisterMenuItemCmd = value;
                NotifyOfPropertyChange(() => bRegisterMenuItemCmd);
            }
        }
        public bool bDrugDept
        {
            get
            {
                return _bDrugDept;
            }
            set
            {
                if (_bDrugDept == value)
                    return;
                _bDrugDept = value;
                NotifyOfPropertyChange(() => bDrugDept);
            }
        }
        public bool bAppointmentMenuItemCmd
        {
            get
            {
                return _bAppointmentMenuItemCmd;
            }
            set
            {
                if (_bAppointmentMenuItemCmd == value)
                    return;
                _bAppointmentMenuItemCmd = value;
                NotifyOfPropertyChange(() => bAppointmentMenuItemCmd);
            }
        }
        public bool bTransactionCmd
        {
            get
            {
                return _bTransactionCmd;
            }
            set
            {
                if (_bTransactionCmd == value)
                    return;
                _bTransactionCmd = value;
                NotifyOfPropertyChange(() => bTransactionCmd);
            }
        }
        public bool bUserAccountCmd
        {
            get
            {
                return _bUserAccountCmd;
            }
            set
            {
                if (_bUserAccountCmd == value)
                    return;
                _bUserAccountCmd = value;
                NotifyOfPropertyChange(() => bUserAccountCmd);
            }
        }
        public bool bClinicManagementCmd
        {
            get
            {
                return _bClinicManagementCmd;
            }
            set
            {
                if (_bClinicManagementCmd == value)
                    return;
                _bClinicManagementCmd = value;
                NotifyOfPropertyChange(() => bClinicManagementCmd);
            }
        }
        public bool bStoreDeptCmd
        {
            get
            {
                return _bStoreDeptCmd;
            }
            set
            {
                if (_bStoreDeptCmd == value)
                    return;
                _bStoreDeptCmd = value;
                NotifyOfPropertyChange(() => bStoreDeptCmd);
            }
        }

        public bool bSystemConfigCmd
        {
            get
            {
                return _bSystemConfigCmd;
            }
            set
            {
                if (_bSystemConfigCmd == value)
                    return;
                _bSystemConfigCmd = value;
                NotifyOfPropertyChange(() => bSystemConfigCmd);
            }
        }

        public void GotoVatTu()
        {
            if (!string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb == null || GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        IsLoading = true;
                        Globals.HomeModuleActive = HomeModuleActive.VATTU;
                        Globals.LoadDynamicModule<IResourcesHome>("eHCMS.ResourceMaintenance.xap", "", LoadDynamicModuleCompleted);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                        OutstandingTaskContent = null;
                    }
                });
            }

        }

        public void ConfigurationMgnt()
        {
            if (!string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb == null || GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        IsLoading = true;
                        Globals.HomeModuleActive = HomeModuleActive.CAUHINH;
                        Globals.LoadDynamicModule<IConfigurationModule>("eHCMS.Configuration.xap", "", LoadDynamicModuleCompleted);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                        OutstandingTaskContent = null;
                    }
                });
            }
        }

        public void Consultations()
        {
            if (!string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb == null || GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        IsLoading = true;
                        Globals.PatientAllDetails.PatientInfo = null;
                        Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NGOAITRU;
                        //var loginVm = Globals.GetViewModel<ILogin>();
                        //loginVm.DeptLocation = null;
                        Globals.HomeModuleActive = HomeModuleActive.KHAMBENH;
                        KhamBenh = true;
                        DangKy = false;
                        Globals.LoadDynamicModule<IConsultationModule>("eHCMS.ConsultantEPrescription.xap", "", LoadDynamicModuleCompleted);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                        return;
                    }
                });

            }
        }
        public void LoadDynamicModuleCompleted(object o)
        {
            IsLoading = false;
        }
        public void Pharmacies()
        {
            if (!string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb == null || GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        IsLoading = true;
                        //var loginVm = Globals.GetViewModel<ILogin>();
                        //loginVm.DeptLocation = null;
                        Globals.HomeModuleActive = HomeModuleActive.NHATHUOC;
                        Globals.LoadDynamicModule<IPharmacyModule>("eHCMS.Pharmacy.xap", "", LoadDynamicModuleCompleted);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });

            }

        }
        public void DrugDept()
        {
            if (!string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb == null || GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        IsLoading = true;
                        Globals.HomeModuleActive = HomeModuleActive.KHOADUOC;
                        Globals.LoadDynamicModule<IDrugModule>("eHCMS.DrugDept.xap", "", LoadDynamicModuleCompleted);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });

            }

        }

        public void RegisterMenuItemCmd()
        {
            IsLoading = true;
            Globals.HomeModuleActive = HomeModuleActive.DANGKY;
            KhamBenh = false;
            DangKy = true;
            Globals.LoadDynamicModule<IRegistrationModule>("eHCMS.Registration.xap", "", LoadDynamicModuleCompleted);
        }
        public void AppointmentMenuItemCmd()
        {
            if (!string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb == null || GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        IsLoading = true;
                        Globals.HomeModuleActive = HomeModuleActive.HENBENH;
                        Globals.LoadDynamicModule<IAppointmentModule>("eHCMS.Appointment.xap", "", LoadDynamicModuleCompleted);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });

            }

        }

        public void TransactionCmd()
        {
            if (!string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb == null || GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        IsLoading = true;
                        Globals.HomeModuleActive = HomeModuleActive.TRANSACTION;
                        Globals.LoadDynamicModule<ITransactionModule>("eHCMS.TransactionManager.xap", "", LoadDynamicModuleCompleted);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });

            }

        }

        public void UserAccountCmd()
        {
            if (!string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb == null || GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        IsLoading = true;
                        Globals.HomeModuleActive = HomeModuleActive.QUANLYNGUOIDUNG;
                        Globals.LoadDynamicModule<IUserAccountManagementHome>("eHCMS.UserAccountManagement.xap", "", LoadDynamicModuleCompleted);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                        OutstandingTaskContent = null;
                    }
                });

            }

        }

        public void ClinicManagementCmd()
        {
            if (!string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb == null || GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        IsLoading = true;
                        Globals.HomeModuleActive = HomeModuleActive.QUANLYPHONGKHAM;
                        Globals.LoadDynamicModule<IClinicManagement>("eHCMS.ClinicManagement.xap", "", LoadDynamicModuleCompleted);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                        OutstandingTaskContent = null;
                    }
                });

            }

        }

        public void StoreDeptCmd()
        {
            if (!string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb == null || GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Globals.IsAdmission = true;
                        IsLoading = true;
                        Globals.HomeModuleActive = HomeModuleActive.KHONOITRU;
                        Globals.LoadDynamicModule<IStoreDeptHome>("eHCMS.StoreDept.xap", "", LoadDynamicModuleCompleted);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                        OutstandingTaskContent = null;
                    }
                });

            }

        }

        public void SystemConfigurationCmd()
        {
            ActiveContent = null;
            LeftMenu = Globals.GetViewModel<ISystemConfigurationLeftMenu>();
        }
        private object _leftMenu;
        public object LeftMenu
        {
            get { return _leftMenu; }
            set
            {
                _leftMenu = value;
                NotifyOfPropertyChange(() => LeftMenu);
            }
        }

        private eLeftMenuByPTType _homeLeftMenuActive;
        public eLeftMenuByPTType HomeLeftMenuActive
        {
            get { return _homeLeftMenuActive; }
            set
            {
                _homeLeftMenuActive = value;
                NotifyOfPropertyChange(() => HomeLeftMenuActive);
            }
        }


        public void FindRegistrationCmd()
        {
            DateTime _FromDate = Globals.ServerDate.Value;

            if (Globals.PatientFindBy_ForConsultation == AllLookupValues.PatientFindBy.NOITRU)
            {
                _FromDate = Globals.ServerDate.Value.AddDays(-30);
            }

            if (ActiveContent is ILaboratoryHome)/*Phần CLS LAB*/
            {
                if (Globals.PatientFindBy_ForLab == null)
                {
                    Globals.PatientFindBy_ForLab = AllLookupValues.PatientFindBy.NGOAITRU;
                }

                Action<ISearchPCLRequest> onInitDlg = delegate (ISearchPCLRequest typeInfo)
                {
                    typeInfo.PatientFindBy = Globals.PatientFindBy_ForLab.Value;
                };
                GlobalsNAV.ShowDialog<ISearchPCLRequest>(onInitDlg);
            }
            else if (ActiveContent is IPCLDepartmentContent)/*Phần CLS*/
            {
                GlobalsNAV.ShowDialog<ISearchPCLRequest>();
            }
            else/*Phần Tuyên trước đến giờ*/
            {
                if (ActiveContent is IRegistrationModule)
                {
                    var RegVM = Globals.GetViewModel<IRegistrationLeftMenu>();
                    HomeLeftMenuActive = RegVM.LeftMenuByPTType;

                    if (HomeLeftMenuActive == eLeftMenuByPTType.IN_PT)
                    {
                        Action<IFindRegistrationInPt> onInitDlg = delegate (IFindRegistrationInPt vm)
                        {
                            vm.CloseAfterSelection = true;
                            var seachCritical = new SeachPtRegistrationCriteria { FromDate = _FromDate, ToDate = Globals.ServerDate.Value, PatientFindBy = AllLookupValues.PatientFindBy.NOITRU, IsAdmission = Globals.IsAdmission };
                            vm.SearchCriteria = seachCritical;
                            vm.LoadRefDeparments();
                        };
                        GlobalsNAV.ShowDialog<IFindRegistrationInPt>(onInitDlg);
                    }
                    else if (HomeLeftMenuActive == eLeftMenuByPTType.OUT_PT)
                    {
                        Action<IFindRegistration> onInitDlg = delegate (IFindRegistration vm)
                        {
                            vm.CloseAfterSelection = true;
                            var seachCritical = new SeachPtRegistrationCriteria { FromDate = _FromDate, ToDate = Globals.ServerDate.Value, PatientFindBy = AllLookupValues.PatientFindBy.NGOAITRU, IsAdmission = Globals.IsAdmission };
                            vm.SearchCriteria = seachCritical;
                        };
                        GlobalsNAV.ShowDialog<IFindRegistration>(onInitDlg);
                    }
                    else
                    {
                        MessageBox.Show(eHCMSResources.A0425_G1_Msg_InfoCNTimDKKhSuDungDc, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    }
                }
                else if (ActiveContent is IConsultationModule)
                {
                    Action<IFindRegistration> onInitDlg = delegate (IFindRegistration vm)
                    {
                        vm.CloseAfterSelection = true;
                        var seachCritical = new SeachPtRegistrationCriteria
                        {
                            FromDate = _FromDate,
                            ToDate = Globals.ServerDate.Value,
                            PatientFindBy = Globals.PatientFindBy_ForConsultation.GetValueOrDefault(),
                            KhamBenh = true,
                            IsAdmission = Globals.IsAdmission
                        };
                        vm.SearchCriteria = seachCritical;
                    };
                    GlobalsNAV.ShowDialog<IFindRegistration>(onInitDlg);
                }
                else
                {
                    //var vm = Globals.GetViewModel<ISearchRegistration>();
                    //Globals.ShowDialog(vm as Conductor<object>);
                    Action<IFindRegistration> onInitDlg = delegate (IFindRegistration vm)
                    {
                        vm.CloseAfterSelection = true;
                        //var seachCritical = new SeachPtRegistrationCriteria { FromDate = _FromDate, ToDate = Globals.ServerDate.Value, PatientFindBy = PatientFindBy, IsAdmission = Globals.IsAdmission };
                        var seachCritical = new SeachPtRegistrationCriteria { FromDate = _FromDate, ToDate = Globals.ServerDate.Value, PatientFindBy = AllLookupValues.PatientFindBy.NGOAITRU, IsAdmission = Globals.IsAdmission };
                        vm.SearchCriteria = seachCritical;
                    };
                    GlobalsNAV.ShowDialog<IFindRegistration>(onInitDlg);
                }
            }
        }
        public void FindRegistrationDetailCmd()
        {
            DateTime _FromDate = Globals.ServerDate.Value;

            if (Globals.PatientFindBy_ForConsultation == AllLookupValues.PatientFindBy.NOITRU)
            {
                _FromDate = Globals.ServerDate.Value.AddDays(-30);
            }

            if (ActiveContent is ILaboratoryHome)/*Phần CLS LAB*/
            {
                if (Globals.PatientFindBy_ForLab == null)
                {
                    Globals.PatientFindBy_ForLab = AllLookupValues.PatientFindBy.NGOAITRU;
                }

                Action<ISearchPCLRequest> onInitDlg = delegate (ISearchPCLRequest typeInfo)
                {
                    typeInfo.PatientFindBy = Globals.PatientFindBy_ForLab.Value;
                };
                GlobalsNAV.ShowDialog<ISearchPCLRequest>(onInitDlg);
            }
            else if (ActiveContent is IPCLDepartmentContent)/*Phần CLS*/
            {
                GlobalsNAV.ShowDialog<ISearchPCLRequest>();
            }
            // TxD 11/10/2017 Begin: Try to Fix problem with Poping up the IFindRegistrationDetail dialog in the wrong context
            //                  To be TESTED AND REVIEW may require further fixing here??
            else if (ActiveContent is IConsultationModule)
            {
                Action<IFindRegistrationDetail> onInitDlg = delegate (IFindRegistrationDetail vm)
                {
                    vm.IsPopup = true;

                    var _searchRegCriteria = new SeachPtRegistrationCriteria();

                    _searchRegCriteria.PatientFindBy = PatientFindBy;
                    _searchRegCriteria.KhamBenh = true;
                    _searchRegCriteria.FromDate = _FromDate;
                    _searchRegCriteria.ToDate = Globals.GetCurServerDateTime();
                    _searchRegCriteria.IsAdmission = null;
                    _searchRegCriteria.DeptID = Globals.ObjRefDepartment.DeptID;
                    _searchRegCriteria.DeptLocationID = Globals.DeptLocation.DeptLocationID;

                    vm.SearchCriteria = _searchRegCriteria;
                    vm.CloseAfterSelection = true;
                    vm.CopyExistingPatientList(null, vm.SearchCriteria, 0);
                };
                GlobalsNAV.ShowDialog<IFindRegistrationDetail>(onInitDlg);
            }

            // TxD 11/10/2017 Commented the following because searching for Registration already handled by a different Link

            //else/*Phần Tuyên trước đến giờ*/
            //{
            //    //cho nay bat on ne

            //    bool _KhamBenh = false;
            //    if (ActiveContent is IConsultationModule)
            //    {
            //        _KhamBenh = true;
            //    }
            //    if (Globals.PatientFindBy_ForConsultation == AllLookupValues.PatientFindBy.NGOAITRU)
            //    {
            //        var vm = Globals.GetViewModel<IFindRegistrationDetail>();
            //        vm.CloseAfterSelection = true;
            //        var seachCritical = new SeachPtRegistrationCriteria
            //        {
            //            FromDate = _FromDate,
            //            ToDate = Globals.ServerDate.Value,
            //            PatientFindBy = Globals.PatientFindBy_ForConsultation.GetValueOrDefault(),
            //            KhamBenh = _KhamBenh,
            //            IsAdmission = Globals.IsAdmission
            //        };
            //        vm.SearchCriteria = seachCritical;
            //        Globals.ShowDialog(vm as Conductor<object>);
            //    }
            //    else
            //    {
            //        var vm = Globals.GetViewModel<IFindRegistration>();
            //        vm.CloseAfterSelection = true;
            //        var seachCritical = new SeachPtRegistrationCriteria
            //        {
            //            FromDate = _FromDate,
            //            ToDate = Globals.ServerDate.Value,
            //            PatientFindBy = Globals.PatientFindBy_ForConsultation.GetValueOrDefault(),
            //            KhamBenh = _KhamBenh,
            //            IsAdmission = Globals.IsAdmission
            //        };
            //        vm.SearchCriteria = seachCritical;
            //        Globals.ShowDialog(vm as Conductor<object>);
            //    }

            //}

            // TxD 11/10/2017 End:

        }

        public void FindPatientCmd()
        {
            //vm.SearchCriteria = _searchCriteria;
            //vm.CopyExistingPatientList(allItems, _searchCriteria, totalCount);
            GlobalsNAV.ShowDialog<IFindPatient>();
        }

        public void PCLDepartmentCmd()
        {
            GlobalsNAV.ShowDialog<IInitPCLDept>();
        }

        public void Handle(InitialPCLImage_Step1_Event message)
        {
            if (GetView() != null)
            {
                IsLoading = true;
                Globals.PatientAllDetails.PatientInfo = null;
                Globals.PatientPCLReqID_Imaging = 0;
                Globals.HomeModuleActive = HomeModuleActive.SIEUAM;
                Globals.LoadDynamicModule<IPCLDepartmentContent>("eHCMS.PCLDepartment.xap", "", LoadDynamicModuleCompleted);
            }
        }

        //KMx: Khi click vào left menu không bắn event nữa, mà set LeftMenuByPTType là Nội trú hoặc Ngoại trú luôn (28/08/2014 18:00).
        //public void Handle(PatientFindByChange obj)
        //{
        //    if (obj != null)
        //    {
        //        PatientFindBy = obj.patientFindBy;
        //    }
        //}


        public void LaboratoryCmd()
        {
            try
            {
                if (GetView() != null)
                {
                    if (!string.IsNullOrEmpty(Globals.HIRegistrationForm))
                    {
                        Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                        {
                            if (GlobalsNAV.msgb == null || GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                            {
                                IsLoading = true;
                                Globals.PatientFindBy_ForLab = AllLookupValues.PatientFindBy.NGOAITRU;
                                //---tai sao co hai thu o day ta
                                Globals.LoadDynamicModule<ILaboratoryHome>("eHCMS.PCLDepartment.xap", "", LoadDynamicModuleCompleted);
                                Globals.LoadDynamicModule<IDrugModule>("eHCMS.DrugDept.xap", "", LoadDynamicModuleCompleted);
                                GlobalsNAV.msgb = null;
                                Globals.HIRegistrationForm = "";
                            }
                        });

                    }
                }
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogInfo(ex.ToString());
            }
        }


        private bool _FindRegistrationCmdVisibility = true;
        public bool FindRegistrationCmdVisibility
        {
            get { return _FindRegistrationCmdVisibility; }
            set
            {
                _FindRegistrationCmdVisibility = value;
                NotifyOfPropertyChange(() => FindRegistrationCmdVisibility);
            }
        }

        private bool _KhamBenh;
        public bool KhamBenh
        {
            get { return _KhamBenh; }
            set
            {
                _KhamBenh = value;
                NotifyOfPropertyChange(() => KhamBenh);
            }
        }

        private bool _DangKy;
        public bool DangKy
        {
            get { return _DangKy; }
            set
            {
                _DangKy = value;
                NotifyOfPropertyChange(() => DangKy);
            }
        }

        #region setvalue menu
        #region load
        public Button RegisMenu { get; set; }
        public Button ConfigMenu { get; set; }
        public Button ConsultMenu { get; set; }
        public Button CLSLabMenu { get; set; }
        public Button CLSImageMenu { get; set; }
        public Button PharmacyMenu { get; set; }
        public Button DrugMenu { get; set; }
        public Button ApointmentMenu { get; set; }
        public Button TransactionMenu { get; set; }
        public Button ResourceMenu { get; set; }
        public Button AccountMenu { get; set; }
        public Button ClinicMenu { get; set; }
        public Button StoreMenu { get; set; }
        public Button SystemConfigMenu { get; set; }
        public Button GeneralEnquireMenu { get; set; }

        public void RegisLoaded(object sender)
        {
            RegisMenu = sender as Button;
            RegisMenu.Foreground = new SolidColorBrush(MenuColor);
        }
        public void ConfigLoaded(object sender)
        {
            ConfigMenu = sender as Button;
            ConfigMenu.Foreground = new SolidColorBrush(MenuColor);
        }
        public void ConsultLoaded(object sender)
        {
            ConsultMenu = sender as Button;
            ConsultMenu.Foreground = new SolidColorBrush(MenuColor);
        }
        public void CLSLabLoaded(object sender)
        {
            CLSLabMenu = sender as Button;
            CLSLabMenu.Foreground = new SolidColorBrush(MenuColor);
        }
        public void CLSImageLoaded(object sender)
        {
            CLSImageMenu = sender as Button;
            CLSImageMenu.Foreground = new SolidColorBrush(MenuColor);
        }
        public void PharmacyLoaded(object sender)
        {
            PharmacyMenu = sender as Button;
            PharmacyMenu.Foreground = new SolidColorBrush(MenuColor);
        }
        public void DrugLoaded(object sender)
        {
            DrugMenu = sender as Button;
            DrugMenu.Foreground = new SolidColorBrush(MenuColor);
        }
        public void ApointmentLoaded(object sender)
        {
            ApointmentMenu = sender as Button;
            ApointmentMenu.Foreground = new SolidColorBrush(MenuColor);
        }
        public void TransactionLoaded(object sender)
        {
            TransactionMenu = sender as Button;
            TransactionMenu.Foreground = new SolidColorBrush(MenuColor);
        }
        public void ResourceLoaded(object sender)
        {
            ResourceMenu = sender as Button;
            ResourceMenu.Foreground = new SolidColorBrush(MenuColor);
        }
        public void AccountLoaded(object sender)
        {
            AccountMenu = sender as Button;
            AccountMenu.Foreground = new SolidColorBrush(MenuColor);
        }
        public void ClinicLoaded(object sender)
        {
            ClinicMenu = sender as Button;
            ClinicMenu.Foreground = new SolidColorBrush(MenuColor);
        }
        public void StoreLoaded(object sender)
        {
            StoreMenu = sender as Button;
            StoreMenu.Foreground = new SolidColorBrush(MenuColor);
        }
        public void SystemConfigLoaded(object sender)
        {
            SystemConfigMenu = sender as Button;
            SystemConfigMenu.Foreground = new SolidColorBrush(MenuColor);
        }

        public void GeneralEnquireLoaded(object sender)
        {
            GeneralEnquireMenu = sender as Button;
            GeneralEnquireMenu.Foreground = new SolidColorBrush(MenuColor);
        }
        #endregion

        private void ShowScreen<T>()
        {
            var screen = IoC.GetInstance(typeof(T), null);
            if (screen != null)
            {
                var home = Globals.GetViewModel<IHome>();
                home.ActiveContent = screen;
                var activeScreen = (home as Conductor<object>);
                activeScreen.ActivateItem(screen);
            }
        }
        //-----------------
        public void RegisClick(object sender)
        {
            RegisMenu = sender as Button;
            resetMenuColor();
            RegisMenu.Foreground = new SolidColorBrush(MenuClickColor);
            IsLoading = true;
            Globals.HomeModuleActive = HomeModuleActive.DANGKY;
            KhamBenh = false;
            DangKy = true;
            //var loginVm = Globals.GetViewModel<ILogin>();
            //loginVm.DeptLocation = null;
            Globals.LoadDynamicModule<IRegistrationModule>("eHCMS.Registration.xap", "", LoadDynamicModuleCompleted);
            ShowScreen<IRegistrationModule>();
        }
        public void ConfigClick(object sender)
        {
            if (!string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb == null || GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ConfigModule(sender);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";

                    }
                    return;
                });
            }
            else
            {
                ConfigModule(sender);
            }
        }

        private void ConfigModule(object sender)
        {
            ConfigMenu = sender as Button;
            resetMenuColor();
            ConfigMenu.Foreground = new SolidColorBrush(MenuClickColor);

            IsLoading = true;
            Globals.HomeModuleActive = HomeModuleActive.CAUHINH;
            Globals.LoadDynamicModule<IConfigurationModule>("eHCMS.Configuration.xap", "", LoadDynamicModuleCompleted);
            OutstandingTaskContent = null;
        }

        public void ConsultClick(object sender)
        {
            if (!string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb == null || GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ConsultModule(sender);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                    return;
                });
            }
            else
            {
                ConsultModule(sender);
            }
        }

        private void ConsultModule(object sender)
        {
            ConsultMenu = sender as Button;
            resetMenuColor();
            ConsultMenu.Foreground = new SolidColorBrush(MenuClickColor);

            IsLoading = true;
            Globals.PatientAllDetails.PatientInfo = null;
            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NGOAITRU;
            //var loginVm = Globals.GetViewModel<ILogin>();
            //loginVm.DeptLocation = null;
            Globals.HomeModuleActive = HomeModuleActive.KHAMBENH;
            KhamBenh = true;
            DangKy = false;
            Globals.LoadDynamicModule<IConsultationModule>("eHCMS.ConsultantEPrescription.xap", "", LoadDynamicModuleCompleted);
        }

        public void CLSLabClick(object sender)
        {
            if (!string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb == null || GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        CLSLabModule(sender);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                    return;
                });
            }
            else
            {
                CLSLabModule(sender);
            }
        }

        private void CLSLabModule(object sender)
        {
            CLSLabMenu = sender as Button;
            resetMenuColor();
            CLSLabMenu.Foreground = new SolidColorBrush(MenuClickColor);

            try
            {
                if (this.GetView() != null)
                {
                    IsLoading = true;
                    Globals.PatientAllDetails.PatientInfo = null;
                    Globals.PatientFindBy_ForLab = AllLookupValues.PatientFindBy.NGOAITRU;
                    Globals.HomeModuleActive = HomeModuleActive.SIEUAM;
                    Globals.LoadDynamicModule<ILaboratoryHome>("eHCMS.PCLDepartment.xap", "", LoadDynamicModuleCompleted);
                    OutstandingTaskContent = null;
                }

            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogInfo(ex.ToString());
            }
        }

        public void CLSImageClick(object sender)
        {
            if (!string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb == null || GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        CLSImageModule(sender);

                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";

                    }
                    return;
                });
            }
            else
            {
                CLSImageModule(sender);
            }

        }

        private void CLSImageModule(object sender)
        {
            CLSImageMenu = sender as Button;
            resetMenuColor();
            CLSImageMenu.Foreground = new SolidColorBrush(MenuClickColor);
            Globals.PatientPCLRequest_Result = null;

            /**/
            OutstandingTaskContent = null;
            ActiveContent = null;
            LeftMenu = null;
            _eventArg.Subscribe(this);
            /**/

            GlobalsNAV.ShowDialog<IInitPCLDept>();
        }

        public void PharmacyClick(object sender)
        {
            if (!string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb == null || GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        PharmacyModule(sender);

                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";

                    }
                    return;
                });
            }
            else
            {
                PharmacyModule(sender);
            }

        }

        private void PharmacyModule(object sender)
        {
            PharmacyMenu = sender as Button;
            resetMenuColor();
            PharmacyMenu.Foreground = new SolidColorBrush(MenuClickColor);

            IsLoading = true;
            //var loginVm = Globals.GetViewModel<ILogin>();
            //loginVm.DeptLocation = null;
            Globals.HomeModuleActive = HomeModuleActive.NHATHUOC;
            Globals.LoadDynamicModule<IPharmacyModule>("eHCMS.Pharmacy.xap", "", LoadDynamicModuleCompleted);
        }

        public void DrugClick(object sender)
        {
            if (!string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb == null || GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DrugModule(sender);

                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                    return;
                });
            }
            else
            {
                DrugModule(sender);
            }
        }

        private void DrugModule(object sender)
        {
            DrugMenu = sender as Button;
            resetMenuColor();
            DrugMenu.Foreground = new SolidColorBrush(MenuClickColor);

            IsLoading = true;
            Globals.HomeModuleActive = HomeModuleActive.KHOADUOC;
            Globals.LoadDynamicModule<IDrugModule>("eHCMS.DrugDept.xap", "", LoadDynamicModuleCompleted);
            OutstandingTaskContent = null;
        }

        public void ApointmentClick(object sender)
        {
            if (!string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb == null || GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ApointmentModule(sender);

                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";

                    }
                    return;
                });
            }
            else
            {
                ApointmentModule(sender);
            }
        }

        private void ApointmentModule(object sender)
        {
            ApointmentMenu = sender as Button;
            resetMenuColor();
            ApointmentMenu.Foreground = new SolidColorBrush(MenuClickColor);

            IsLoading = true;
            Globals.HomeModuleActive = HomeModuleActive.HENBENH;
            Globals.LoadDynamicModule<IAppointmentModule>("eHCMS.Appointment.xap", "", LoadDynamicModuleCompleted);
        }

        public void TransactionClick(object sender)
        {
            if (!string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb == null || GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TransactionModule(sender);

                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";

                    }
                    return;
                });
            }
            else
            {
                TransactionModule(sender);
            }

        }

        private void TransactionModule(object sender)
        {
            TransactionMenu = sender as Button;
            resetMenuColor();
            TransactionMenu.Foreground = new SolidColorBrush(MenuClickColor);

            IsLoading = true;
            Globals.HomeModuleActive = HomeModuleActive.TRANSACTION;
            Globals.LoadDynamicModule<ITransactionModule>("eHCMS.TransactionManager.xap", "", LoadDynamicModuleCompleted);
            OutstandingTaskContent = null;
        }
        public void ResourceClick(object sender)
        {
            if (!string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb == null || GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ResourceModule(sender);

                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                    return;
                });
            }
            else
            {
                ResourceModule(sender);
            }

        }

        private void ResourceModule(object sender)
        {
            ResourceMenu = sender as Button;
            resetMenuColor();
            ResourceMenu.Foreground = new SolidColorBrush(MenuClickColor);

            IsLoading = true;
            Globals.HomeModuleActive = HomeModuleActive.VATTU;
            Globals.LoadDynamicModule<IResourcesHome>("eHCMS.ResourceMaintenance.xap", "", LoadDynamicModuleCompleted);
        }

        public void AccountClick(object sender)
        {
            if (!string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb == null || GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        AccountModule(sender);

                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                    return;
                });
            }
            else
            {
                AccountModule(sender);
            }
        }

        private void AccountModule(object sender)
        {
            AccountMenu = sender as Button;
            resetMenuColor();
            AccountMenu.Foreground = new SolidColorBrush(MenuClickColor);

            IsLoading = true;
            Globals.HomeModuleActive = HomeModuleActive.QUANLYNGUOIDUNG;
            Globals.LoadDynamicModule<IUserAccountManagementHome>("eHCMS.UserAccountManagement.xap", "", LoadDynamicModuleCompleted);
            OutstandingTaskContent = null;
        }
        public void ClinicClick(object sender)
        {
            if (!string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb == null || GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ClinicModule(sender);

                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                    return;
                });
            }
            else
            {
                ClinicModule(sender);
            }

        }

        private void ClinicModule(object sender)
        {
            ClinicMenu = sender as Button;
            resetMenuColor();
            ClinicMenu.Foreground = new SolidColorBrush(MenuClickColor);

            IsLoading = true;
            Globals.HomeModuleActive = HomeModuleActive.QUANLYPHONGKHAM;
            Globals.LoadDynamicModule<IClinicManagement>("eHCMS.ClinicManagement.xap", "", LoadDynamicModuleCompleted);
            OutstandingTaskContent = null;
        }
        public void StoreClick(object sender)
        {
            if (!string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb == null || GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        StoreDeptModule(sender);

                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";

                    }
                    return;
                });
            }
            else
            {
                StoreDeptModule(sender);
            }

        }

        private void StoreDeptModule(object sender)
        {
            StoreMenu = sender as Button;
            resetMenuColor();
            StoreMenu.Foreground = new SolidColorBrush(MenuClickColor);

            IsLoading = true;
            Globals.IsAdmission = true;
            Globals.HomeModuleActive = HomeModuleActive.KHONOITRU;
            Globals.LoadDynamicModule<IStoreDeptHome>("eHCMS.StoreDept.xap", "", LoadDynamicModuleCompleted);
            OutstandingTaskContent = null;
        }
        public void SystemConfigClick(object sender)
        {
            if (!string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb == null || GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        SystemConfigModule(sender);

                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                    return;
                });
            }
            else
            {
                SystemConfigModule(sender);
            }

        }

        private void SystemConfigModule(object sender)
        {
            SystemConfigMenu = sender as Button;
            resetMenuColor();
            SystemConfigMenu.Foreground = new SolidColorBrush(MenuClickColor);

            ActiveContent = null;
            LeftMenu = Globals.GetViewModel<ISystemConfigurationLeftMenu>();
        }

        public void GeneralEnquireClick(object sender)
        {
            if (!string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb == null || GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        GeneralEnquireModule(sender);

                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";

                    }
                    return;
                });
            }
            else
            {
                GeneralEnquireModule(sender);
            }
        }

        private void GeneralEnquireModule(object sender)
        {
            GeneralEnquireMenu = sender as Button;
            resetMenuColor();
            GeneralEnquireMenu.Foreground = new SolidColorBrush(MenuClickColor);

            IsLoading = true;
            Globals.HomeModuleActive = HomeModuleActive.YEUCAUCHUNG;
            Globals.LoadDynamicModule<IGeneralEnquireHome>("eHCMS.GeneralEnquire.xap", "", LoadDynamicModuleCompleted);
            OutstandingTaskContent = null;
        }

        private Color MenuColor = Color.FromArgb(255, 240, 248, 255);
        private Color MenuClickColor = Color.FromArgb(255, 245, 186, 16);
        public void resetMenuColor()
        {
            RegisMenu.Foreground = new SolidColorBrush(MenuColor);
            ConfigMenu.Foreground = new SolidColorBrush(MenuColor);
            ConsultMenu.Foreground = new SolidColorBrush(MenuColor);
            CLSLabMenu.Foreground = new SolidColorBrush(MenuColor);
            CLSImageMenu.Foreground = new SolidColorBrush(MenuColor);
            PharmacyMenu.Foreground = new SolidColorBrush(MenuColor);
            DrugMenu.Foreground = new SolidColorBrush(MenuColor);
            ApointmentMenu.Foreground = new SolidColorBrush(MenuColor);
            TransactionMenu.Foreground = new SolidColorBrush(MenuColor);
            ResourceMenu.Foreground = new SolidColorBrush(MenuColor);
            AccountMenu.Foreground = new SolidColorBrush(MenuColor);
            ClinicMenu.Foreground = new SolidColorBrush(MenuColor);
            StoreMenu.Foreground = new SolidColorBrush(MenuColor);
            SystemConfigMenu.Foreground = new SolidColorBrush(MenuColor);
            GeneralEnquireMenu.Foreground = new SolidColorBrush(MenuColor);
        }
        #endregion

        public void LoadAllHospitalInfoAction()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        long nfilterHosID = 0;
                        contract.BeginHospital_ByCityProvinceID(nfilterHosID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Hospital> allItems = new ObservableCollection<Hospital>();
                                try
                                {
                                    allItems = contract.EndHospital_ByCityProvinceID(asyncResult);
                                    Globals.allHospitals = new ObservableCollection<Hospital>(allItems);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                    ClientLoggerHelper.LogError(ex.Message);
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                }

                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    ClientLoggerHelper.LogError(ex.Message);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

    }
}

