using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Reflection;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure.CachingUtils;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using aEMR.CommonTasks;
using eHCMSLanguage;
/*
 * 20181113 #001 TTM:   Comment hàm lấy quận huyện ra để giảm tải cho Login (hàm lấy quận huyện sẽ đc thực hiện khi người dùng vào màn hình PatientDetails). 
 *                      Bởi vì không phải user nào cũng cần phải làm chuyện này. 
 * 20191006 #002 TTM:   BM 0017421: [Ra toa] Thêm tên thư ký y khoa thực hiện toa                      
 */
namespace aEMRMain.ViewModels
{
    [Export(typeof(ILogin)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class LoginViewModel : Conductor<object>, ILogin
        , IHandle<AppCheckAndDownloadUpdateCompletedEvent>
    {
        private readonly INavigationService _navigationService;        
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public LoginViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            //IsProcessing = false;
            //GetCustomerIP();
            IsNotProcessing = true;
        }

        public bool IsDevLogin { get; set; }
        public void DevAutoLogin()
        {
            LoginCmd();
        }

        //private bool _OOB_Software_HasNotBeen_Updated = true;

        private bool _IsNotProcessing = true;
        public bool IsNotProcessing
        {
            get
            {
                return _IsNotProcessing;
            }
            set
            {
                _IsNotProcessing = value;
                NotifyOfPropertyChange(() => IsNotProcessing);
            }
        }

        private bool _IsProcPCLForm ;
        public bool IsProcPCLForm
        {
            get
            {
                return _IsProcPCLForm;
            }
            set
            {
                _IsProcPCLForm = value;
                NotifyOfPropertyChange(() => IsProcPCLForm);
            }
        }

        private bool _IsProcPCLCombo ;
        public bool IsProcPCLCombo
        {
            get
            {
                return _IsProcPCLCombo;
            }
            set
            {
                _IsProcPCLCombo = value;
                NotifyOfPropertyChange(() => IsProcPCLCombo);
            }
        }

        public void ThePasswordBox_Loaded(object sender, RoutedEventArgs e)
        {
            PwdBoxCtrl = sender as PasswordBox;
        }

        private PasswordBox _PwdBoxCtrl;
        public PasswordBox PwdBoxCtrl
        {
            get
            {
                return _PwdBoxCtrl;
            }
            set
            {
                _PwdBoxCtrl = value;
            }          
        }

        private string _loginName;
        public string LoginName
        {
            get { return _loginName; }
            set
            {
                _loginName = value;
                NotifyOfPropertyChange(()=>LoginName);
                NotifyOfPropertyChange(()=>CanLoginCmd);
            }
        }

        private string _password;
        public string ThePassword
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyOfPropertyChange(()=>ThePassword);
                NotifyOfPropertyChange(() => CanLoginCmd);
            }
        }

        private bool _isRemembered;
        public bool IsRemembered
        {
            get
            {
                return _isRemembered;
            }
            set
            {
                _isRemembered = value;
                NotifyOfPropertyChange(() => IsRemembered);
            }
        }

        private bool _rememberMe;
        public bool RememberMe
        {
            get
            {
                return _rememberMe;
            }
            set
            {
                _rememberMe = value;
                NotifyOfPropertyChange(() => RememberMe);
            }
        }

        private DeptLocation _deptLocation;
        public DeptLocation DeptLocation
        {
            get
            {
                return _deptLocation;
            }
            set
            {
                _deptLocation = value;
                NotifyOfPropertyChange(() => DeptLocation);
            }
        }

        public void SetActive() 
        {
            //IsProcessing = true;
            IsNotProcessing = true;
        }
        
        public bool CanLoginCmd
        {            
            get { return (!string.IsNullOrEmpty(LoginName) && !string.IsNullOrEmpty(ThePassword)); }
        }

        private int cnt = 0;
        public void LoginCmd()
        {
            ProcessLoginAndGetConfigRefValues();
        }

        private void ProcessLoginAndGetConfigRefValues()
        {
            try
            {
                //▼===== #002: Bổ sung thêm Coroutine để thực hiện việc đăng nhập vào chức năng thư ký y khoa.
                if (IsConfirmForSecretary)
                {
                    Coroutine.BeginExecute(GetConfirmLoginForSecretary_Routine());
                }
                else
                {
                    Coroutine.BeginExecute(GetConfigInitAndRefValues_Routine());
                }
                //▲===== #002
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogError(ex.ToString());
                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
            }
        }

        private IEnumerator<IResult> GetConfigInitAndRefValues_Routine()
        {
            // If the following Task Action break out and NOT CONTINUE ON then Hide Busy Indicator is in the Action itself
            // Because if we put Show and Hide Busy Indicator in each Task Action then it will create a flickering effect
            this.ShowBusyIndicator();
            // 0. Process login
            var loginTask = new GenericCoRoutineTask(ProcessLoginAction);
            yield return loginTask;
            List<refModule> list = loginTask.GetResultObj(0) as List<refModule>;
            UserAccount userAccount = loginTask.GetResultObj(1) as UserAccount;
            
            // 1. Get all App Config setting values
            // Txd 25/05/2014 Replaced ConfigList so the following Task is nolonger required
            //yield return GenericCoRoutineTask.StartTask(GetAllAppConfigValueAction);

            // TxD 07/12/2014 Added the following to store a list of all RefDepartment in global to prevent going back to server later on
            yield return GenericCoRoutineTask.StartTask(LoadAllDepartmentIncludeDeletedForRef_Action);

            // 2. Get All Staffs
            //yield return GenericCoRoutineTask.StartTask(GetAllStaffsAction);
            
            //TxD 07/02/2018 For now use the following to get All Staff until upgrade to new .NET framework and new version of WCF
            int nInitStaffID = 0;
            yield return GenericCoRoutineTask.StartTask(GetAllStaffsFromStaffID_Action, nInitStaffID);

            if (Globals.AllStaffs != null)
            {
                int nCount = Globals.AllStaffs.Count();
                if (nCount > 0)
                {
                    long nLastStaffID = Globals.AllStaffs[nCount - 1].StaffID + 1;
                    yield return GenericCoRoutineTask.StartTask(GetAllStaffsFromStaffID_Action, nLastStaffID as object);
                }
            }

            //// 2. Get Registration Office Staff list
            //yield return GenericCoRoutineTask.StartTask(GetAllRegisStaffAction);
            
            //// 2.1 Get Doctor Staff List
            //yield return GenericCoRoutineTask.StartTask(GetAllDoctorStaffAction);

            // 3. Load Server Configuration settings
            yield return GenericCoRoutineTask.StartTask(GetServerConfigurationAction);

            // 3.1 Checking Software and Database version based on Configuration settings just retrieved from the Step above
            yield return GenericCoRoutineTask.StartTask(CheckSoftwareAndDatabaseVersionAction);

            //if (!PassedSoftAndDBCheck)
            //{
            //    var warnConfDlg = new WarningWithConfirmMsgBoxTask(eHCMSResources.Z1270_G1_ChuaCNhatKgTheDangNhap, "", false);
            //    yield return warnConfDlg;
            //    this.HideBusyIndicator();
            //    //App app = (App)Application.Current;                
            //    //if (app.IsRunningOutOfBrowser)
            //    //{
            //        //app.MainWindow.Close();
            //    //}

            //    yield break;
            //}
            
            //▼====== #001
            //// 4. Get All Suburb Names
            //yield return GenericCoRoutineTask.StartTask(GetAllSuburbNameAction);
            //▲====== #001

            // 5. Get All Staff Positions
            yield return GenericCoRoutineTask.StartTask(GetAllStaffPositionAction);
            
            // 6. Get All Prescription Detail Schedules LieuDung
            yield return GenericCoRoutineTask.StartTask(GetInitChoosePrescriptionDetailSchedulesLieuDungAction);
            
            // 7. GetBlankPrescriptionDetailScheduleAction
            yield return GenericCoRoutineTask.StartTask(GetBlankPrescriptionDetailScheduleAction);
            
            // 8. Load All PCL exam Types
            yield return GenericCoRoutineTask.StartTask(LoadAllPclExamTypesAction);
            
            // 9. ListPclExamTypesAllComboAction
            yield return GenericCoRoutineTask.StartTask(ListPclExamTypesAllComboAction);
            
            // 10. Get ALL Lookup Values            
            yield return GenericCoRoutineTask.StartTask(GetAllLookupValueAction);
            
            // 11. get All Patient Payment Accounts
            var allPatientPaymentAccTask = new LoadPatientPaymentAccountListTask();
            yield return allPatientPaymentAccTask;

            Globals.AllPatientPaymentAccounts = new List<PatientPaymentAccount>(allPatientPaymentAccTask.PatientPaymentAccountList);

            PrepareWorkingViews(list, userAccount);

            // 11.1 Get cấu hình trách nhiệm của staff.            
            yield return GenericCoRoutineTask.StartTask(GetStaffDeptResponsibilitiesByDeptID);

            //KMx: 3 dòng code bên dưới được lấy từ trong hàm PrepareWorkingViews() ra. Lý do: Phải nằm sau hàm GetStaffDeptResponsibilitiesByDeptID() mới đủ dữ liệu (10/07/2014 17:13)
            //var locationVm = Globals.GetViewModel<ISelectLocation>();
            Action<ISelectLocation> onInitDlg = delegate (ISelectLocation locationVm)
            {
                locationVm.mCancel = false;
            };

            GlobalsNAV.ShowDialog<ISelectLocation>(onInitDlg);


            // 12. Get the current Date Time from Server
            var loadCurrentDate = new LoadCurrentDateTask();
            yield return loadCurrentDate;
            
            if (loadCurrentDate.CurrentDate == DateTime.MinValue)
            {
                Globals.ShowMessage(string.Format("{0}.", eHCMSResources.Z1257_G1_KgLayDcNgThTuServer), eHCMSResources.G0442_G1_TBao);
            }
            else
            {
                Globals.ServerDate = loadCurrentDate.CurrentDate;
                StartTimerToUpdateServerDateTime();
            }

            // 13. Get Medical Record Templates            
            yield return GenericCoRoutineTask.StartTask(GetAllMedicalRecordTemplatesAction);

            // 14. Get RefOutputType (Loại xuất cho Khoa dược).            
            yield return GenericCoRoutineTask.StartTask(GetAllRefOutputType);

            // 15. ListPclExamTypesAllComboAction
            //yield return GenericCoRoutineTask.StartTask(ListPackageTechnicalServiceDetailAllAction);

            this.HideBusyIndicator();

            yield break;

        }

        public void PrepareWorkingViews(List<refModule> list, UserAccount userAccount)
        {
            Globals.listRefModule = list;
            if (Globals.CheckModule(Globals.listRefModule, (int)eModules.mAdmin))
            {
                Globals.isAccountCheck = false;
                Globals.IsUserAdmin = true;
            }
            else
            {
                Globals.IsUserAdmin = false;
            }
            Globals.LoggedUserAccount = userAccount;
            Globals.LoggedUserAccount.AccountName = LoginName;
            //neu la admin thi co du tat ca quyen
            if (Globals.LoggedUserAccount.AccountName == "admin"
                || Globals.LoggedUserAccount.AccountName == "Admin"
                || Globals.LoggedUserAccount.AccountName == "administrator"
                || Globals.LoggedUserAccount.AccountName == "ADMIN"
                || Globals.LoggedUserAccount.AccountName == "ADMINISTRATOR"
                )
            {
                Globals.isAccountCheck = false;
            }

            
            var shell = Globals.GetViewModel<IShellViewModel>();
            var home = Globals.GetViewModel<IHome>();
            home.ActiveContent = null;

            //var header = Globals.GetViewModel<IAppHeader>();
            //header.UserName = Globals.LoggedUserAccount.AccountName;
            //header.ShowProfileInfo = true;

            shell.isUserLoggedIn = true;
            shell.IsBtnVisible = true;

            //shell.ContentItem = home;
            UserLoginInspector.Instance.SetUserInspector(Globals.LoggedUserAccount);
            //(shell as Conductor<object>).ActivateItem(home);

            _navigationService.NavigationTo(home as IScreen);
            //KMx: Phải load view này sau hàm GetStaffDeptResponsibilitiesByDeptID(). Vì khi đó mới có đủ dữ liệu (10/07/2014 17:11).
            //var locationVm = Globals.GetViewModel<ISelectLocation>();
            //locationVm.mCancel = false;

            //Globals.ShowDialog(locationVm as Conductor<object>);

            shell.NotifyChanged();
        }

        public void ProcessLoginAction(GenericCoRoutineTask genTask)
        {
            IsNotProcessing = false;

            string tmpPwd = "";
            if (IsDevLogin)
            {
                tmpPwd = ThePassword;
            }
            else
            {
                tmpPwd = PwdBoxCtrl.Password;
            }
            
            if (string.IsNullOrEmpty(LoginName) || string.IsNullOrEmpty(tmpPwd))
            {
                genTask.ActionComplete(false);
                //Globals.ShowMessage(string.Format("{0}.", eHCMSResources.Z1255_G1_KgTheDangNhap), eHCMSResources.T0432_G1_Error); 
                this.HideBusyIndicator();
                IsNotProcessing = true; 
                return;
            }

            string AccountName = LoginName.ToUpper();
            string AccountPassword = tmpPwd;
           
            var list = new List<refModule>();
            bool? IsOutOfSegments=null;
            var t = new Thread(() =>
            {
                try
                {                    
                    using (var serviceFactory = new UserManagementServiceClient())
                    {
                        bool bContinue = true;
                        var contract = serviceFactory.ServiceInstance;
                        if (Globals.isEncrypt)
                        {
                            AccountName = aEMR.Common.EncryptExtension.Encrypt(LoginName.ToUpper(), Globals.AxonKey, Globals.AxonPass);
                            AccountPassword = aEMR.Common.EncryptExtension.Encrypt(tmpPwd, Globals.AxonKey, Globals.AxonPass);
                        }
                        string hostname = Environment.MachineName;

                        contract.BeginAuthenticateUser(AccountName, AccountPassword, hostname, IsConfirmForSecretary, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var userAccount = contract.EndAuthenticateUser(out list, out IsOutOfSegments, asyncResult);
                                genTask.AddResultObj(list);
                                genTask.AddResultObj(userAccount);
                               
                                //IsOutOfSegments = false;
                                if (!IsConfirmForSecretary)
                                {
                                    Globals.isAccountCheck = true;
                                }
                                if (userAccount == null)
                                {
                                    bContinue = false;
                                    Globals.ShowMessage(eHCMSResources.Z1255_G1_KgTheDangNhap, eHCMSResources.T0432_G1_Error);                                    
                                }
                                if(IsOutOfSegments!=null && IsOutOfSegments==true)
                                {
                                    bContinue = false;
                                    Globals.ShowMessage("Không thể đăng nhập giờ hiện tại không nằm trong lịch khám", eHCMSResources.T0432_G1_Error);
                                }
                            }
                            catch (Exception ex)
                            {
                                bContinue = false;
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);                                
                            }
                            finally
                            {
                                genTask.ActionComplete(bContinue);
                                if (!bContinue)
                                {
                                    this.HideBusyIndicator();
                                }
                                IsNotProcessing = true;
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogError(ex.Message);
                    IsNotProcessing = true;
                    this.HideBusyIndicator();
                }

            });
            t.Start();
        }

        public void GetAllLookupValueAction(GenericCoRoutineTask genTask)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        bool bContinue = true;
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllLookupValuesByType(LookupValues.ALL_VALUE_TYPE, Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<Lookup> allItems = new ObservableCollection<Lookup>();
                            try
                            {
                                allItems = contract.EndGetAllLookupValuesByType(asyncResult);
                                Globals.AllLookupValueList = new List<Lookup>(allItems);
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                ClientLoggerHelper.LogError(ex.ToString());
                                bContinue = false;
                            }
                            finally
                            {
                                genTask.ActionComplete(bContinue);
                                if (!bContinue)
                                {
                                    this.HideBusyIndicator();
                                }
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    ClientLoggerHelper.LogError(ex.ToString());
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        public void GetAllMedicalRecordTemplatesAction(GenericCoRoutineTask genTask)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePMRsServiceClient())
                    {
                        bool bContinue = true;
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllMedRecTemplates(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetAllMedRecTemplates(asyncResult);
                                Globals.AllMedRecTemplates = new ObservableCollection<MedicalRecordTemplate>(results);
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                ClientLoggerHelper.LogError(ex.ToString());
                                bContinue = false;
                            }
                            finally
                            {
                                genTask.ActionComplete(bContinue);
                                if (!bContinue)
                                {
                                    this.HideBusyIndicator();
                                }
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    ClientLoggerHelper.LogError(ex.ToString());
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }


        public void GetAllRefOutputType(GenericCoRoutineTask genTask)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        bool bContinue = true;
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginRefOutputType_Get(true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndRefOutputType_Get(asyncResult);
                                Globals.RefOutputType = new ObservableCollection<RefOutputType>(results);
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                ClientLoggerHelper.LogError(ex.ToString());
                                bContinue = false;
                            }
                            finally
                            {
                                genTask.ActionComplete(bContinue);
                                if (!bContinue)
                                {
                                    this.HideBusyIndicator();
                                }
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    ClientLoggerHelper.LogError(ex.ToString());
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        private void GetAllStaffsAction(GenericCoRoutineTask genTask)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonUtilsServiceClient())
                    {
                        bool bContinue = true;
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllStaffs(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                Globals.AllStaffs = contract.EndGetAllStaffs(asyncResult).ToList();

                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                ClientLoggerHelper.LogError(ex.ToString());
                                bContinue = false;
                            }
                            finally
                            {
                                genTask.ActionComplete(bContinue);
                                if (!bContinue)
                                {
                                    this.HideBusyIndicator();
                                }
                            }

                        }), null);

                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    ClientLoggerHelper.LogError(ex.ToString());
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }

            });

            t.Start();
        }

        private void GetAllStaffsFromStaffID_Action(GenericCoRoutineTask genTask, object FromStaffID)
        {
            long nFromStaffID = Convert.ToInt32(FromStaffID);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonUtilsServiceClient())
                    {
                        bool bContinue = true;
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllStaffs_FromStaffID(nFromStaffID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                if (nFromStaffID > 0)
                                {
                                    List<Staff> newStaffList = contract.EndGetAllStaffs_FromStaffID(asyncResult).ToList();
                                    Globals.AllStaffs.AddRange(newStaffList);
                                }
                                else
                                {
                                    Globals.AllStaffs = contract.EndGetAllStaffs_FromStaffID(asyncResult).ToList();
                                }

                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                ClientLoggerHelper.LogError(ex.ToString());
                                bContinue = false;
                            }
                            finally
                            {
                                genTask.ActionComplete(bContinue);
                                if (!bContinue)
                                {
                                    this.HideBusyIndicator();
                                }
                            }

                        }), null);

                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    ClientLoggerHelper.LogError(ex.ToString());
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }

            });

            t.Start();
        }


        //private void GetAllRegisStaffAction(GenericCoRoutineTask genTask)
        //{            
        //    var t = new Thread(() =>
        //    {
        //        try
        //        {
        //            using (var serviceFactory = new CommonUtilsServiceClient())
        //            {
        //                bool bContinue = true;
        //                var contract = serviceFactory.ServiceInstance;
        //                contract.BeginSearchStaffCat(new Staff { FullName = "", RefStaffCategory = new RefStaffCategory { V_StaffCatType = (long)V_StaffCatType.NhanVienQuayDangKy } }, 0, 1000, Globals.DispatchCallback((asyncResult) =>
        //                {
        //                    try
        //                    {
        //                        Globals.AllRegisStaff = contract.EndSearchStaffCat(asyncResult).ToList();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                        ClientLoggerHelper.LogError(ex.ToString());
        //                        bContinue = false;
        //                    }
        //                    finally
        //                    {
        //                        genTask.ActionComplete(bContinue);
        //                        if (!bContinue)
        //                        {
        //                            this.HideBusyIndicator();
        //                        }
        //                    }

        //                }), null);

        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //            ClientLoggerHelper.LogError(ex.ToString());
        //            genTask.ActionComplete(false);
        //            this.HideBusyIndicator();
        //        }

        //    });

        //    t.Start();
        //}

        //private void GetAllDoctorStaffAction(GenericCoRoutineTask genTask)
        //{
        //    var t = new Thread(() =>
        //    {
        //        try
        //        {
        //            using (var serviceFactory = new CommonUtilsServiceClient())
        //            {
        //                bool bContinue = true;
        //                var contract = serviceFactory.ServiceInstance;
        //                contract.BeginSearchStaffCat(new Staff { FullName = "", RefStaffCategory = new RefStaffCategory { V_StaffCatType = (long)V_StaffCatType.BacSi } }, 0, 1000, Globals.DispatchCallback((asyncResult) =>
        //                {
        //                    try
        //                    {
        //                        Globals.AllDoctorStaff = contract.EndSearchStaffCat(asyncResult).ToList();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                        ClientLoggerHelper.LogError(ex.ToString());
        //                        bContinue = false;
        //                    }
        //                    finally
        //                    {
        //                        genTask.ActionComplete(bContinue);
        //                        if (!bContinue)
        //                        {
        //                            this.HideBusyIndicator();
        //                        }
        //                    }

        //                }), null);

        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //            ClientLoggerHelper.LogError(ex.ToString());
        //            genTask.ActionComplete(false);
        //            this.HideBusyIndicator();
        //        }

        //    });

        //    t.Start();
        //}

        public void GetStaffDeptResponsibilitiesByDeptID(GenericCoRoutineTask genTask)
        {
            StaffDeptResponsibilities curStaffDeptResponSearch = new StaffDeptResponsibilities();
            curStaffDeptResponSearch.Staff = Globals.LoggedUserAccount.Staff;
            curStaffDeptResponSearch.StaffID = Globals.LoggedUserAccount.StaffID.GetValueOrDefault();

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        bool bContinue = true;
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetStaffDeptResponsibilitiesByDeptID(curStaffDeptResponSearch, false, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetStaffDeptResponsibilitiesByDeptID(asyncResult);
                                if (Globals.LoggedUserAccount == null)
                                {
                                    Globals.LoggedUserAccount = new UserAccount();
                                }
                                if (Globals.DoctorAccountBorrowed == null)
                                {
                                    Globals.DoctorAccountBorrowed = new UserAccount();
                                }

                                Globals.LoggedUserAccount.AllStaffDeptResponsibilities = results;

                                //KMx: Danh sách các DepartmentID mà nhân viên được cấu hình trách nhiệm (15/09/2014 10:12).
                                Globals.LoggedUserAccount.DeptIDResponsibilityList = new ObservableCollection<long>();
                                if (results != null && results.Count > 0)
                                {
                                    foreach (var item in results)
                                    {
                                        Globals.LoggedUserAccount.DeptIDResponsibilityList.Add(item.DeptID);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                ClientLoggerHelper.LogError(ex.ToString());
                                bContinue = false;
                            }
                            finally
                            {
                                genTask.ActionComplete(bContinue);
                                if (!bContinue)
                                {
                                    this.HideBusyIndicator();
                                }
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    ClientLoggerHelper.LogError(ex.ToString());
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        private bool PassedSoftAndDBCheck = false;

        private void CheckSoftwareAndDatabaseVersionAction(GenericCoRoutineTask genTask)
        {
            PassedSoftAndDBCheck = false;
            if (CrossCheckSoftwareExpectedVersion(Globals.ServerConfigSection.CommonItems.ExpRelAndBuildVersion))
            {
                PassedSoftAndDBCheck = true;
            }
            genTask.ActionComplete(true);
        }


        private bool CrossCheckSoftwareExpectedVersion(int expSoftwareRelBuildVer)
        {
            Assembly app = Assembly.GetExecutingAssembly();
            string strSoftwareFileVersion = "1.0.0.0";
            AssemblyName assemblyName = new AssemblyName(app.FullName);
            strSoftwareFileVersion = assemblyName.Version.ToString();
                                         
            string[] verParams = strSoftwareFileVersion.Split(new char[] { '.' });
            int nSoftRelMajor = Convert.ToInt16(verParams[0]);
            int nSoftRelMinor = Convert.ToInt16(verParams[1]);
            int nSoftBuildMajor = Convert.ToInt16(verParams[2]);
            int nSoftBuildMinor = Convert.ToInt16(verParams[3]);
            long lExpVerNum = expSoftwareRelBuildVer;
            int expSoftRelMajor = Convert.ToInt32((lExpVerNum & 0xFF000000) >> 24);
            int expSoftRelMinor = Convert.ToInt32((lExpVerNum & 0x00FF0000) >> 16);
            int expSoftBuildMajor = Convert.ToInt32((lExpVerNum & 0x0000FF00) >> 8);
            int expSoftBuildMinor = Convert.ToInt32(lExpVerNum & 0x000000FF);

            if ((nSoftRelMajor != expSoftRelMajor) || (nSoftRelMinor != expSoftRelMinor) || (nSoftBuildMajor != expSoftBuildMajor) || (nSoftBuildMinor != expSoftBuildMinor))
            {
                return false;
            }

            return true;
        }

        private void LoadAllDepartmentIncludeDeletedForRef_Action(GenericCoRoutineTask genTask)
        {            
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        bool bContinue = true;
                        contract.BeginGetAllDepartments(true,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var allDepts = contract.EndGetAllDepartments(asyncResult);

                                    if (allDepts != null)
                                    {
                                        Globals.AllRefDepartmentList = new ObservableCollection<RefDepartment>(allDepts);
                                    }
                                    else
                                    {
                                        Globals.AllRefDepartmentList = new ObservableCollection<RefDepartment>();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogError(ex.ToString());
                                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                    bContinue = false;
                                }
                                finally
                                {
                                    genTask.ActionComplete(bContinue);
                                    if (!bContinue)
                                    {
                                        this.HideBusyIndicator();
                                    }
                                }

                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogError(ex.ToString());
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        private void GetServerConfigurationAction(GenericCoRoutineTask genTask)
        {

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        bool bContinue = true;
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetServerConfiguration(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                Globals.ServerConfigSection = contract.EndGetServerConfiguration(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogError(ex.ToString());
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                bContinue = false;
                            }
                            finally
                            {
                                genTask.ActionComplete(bContinue);
                                if (!bContinue)
                                {
                                    this.HideBusyIndicator();
                                }
                            }

                        }), null);

                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogError(ex.ToString());
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }

            });

            t.Start();
        }

        private void GetAllSuburbNameAction(GenericCoRoutineTask genTask)
        {

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        bool bContinue = true;
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllSuburbNames(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                Globals.allSuburbNames = contract.EndGetAllSuburbNames(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogError(ex.ToString());
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);                                
                                bContinue = false;
                            }
                            finally
                            {
                                genTask.ActionComplete(bContinue);
                                if (!bContinue)
                                {
                                    this.HideBusyIndicator();
                                }
                            }

                        }), null);

                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogError(ex.ToString());
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);                    
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void GetAllStaffPositionAction(GenericCoRoutineTask genTask)
        {

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        bool bContinue = true;
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllStaffPosition(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                Globals.allStaffPositions = contract.EndGetAllStaffPosition(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogError(ex.ToString());
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                bContinue = false;
                            }
                            finally
                            {
                                genTask.ActionComplete(bContinue);
                                if (!bContinue)
                                {
                                    this.HideBusyIndicator();
                                }
                            }

                        }), null);

                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogError(ex.ToString());
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void GetInitChoosePrescriptionDetailSchedulesLieuDungAction(GenericCoRoutineTask genTask)
        {

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePrescriptionsServiceClient())
                    {
                        bool bContinue = true;
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginInitChoosePrescriptionDetailSchedulesLieuDung(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                Globals.allPrescriptionDetailSchedulesLieuDung = contract.EndInitChoosePrescriptionDetailSchedulesLieuDung(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogError(ex.ToString());
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                bContinue = false;
                            }
                            finally
                            {
                                genTask.ActionComplete(bContinue);
                                if (!bContinue)
                                {
                                    this.HideBusyIndicator();
                                }
                            }

                        }), null);

                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogError(ex.ToString());
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }

            });

            t.Start();
        }

        private void GetBlankPrescriptionDetailScheduleAction(GenericCoRoutineTask genTask)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePrescriptionsServiceClient())
                    {
                        bool bContinue = true;
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPrescriptionDetailSchedules_ByPrescriptDetailID(0, false, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var items = contract.EndPrescriptionDetailSchedules_ByPrescriptDetailID(asyncResult);

                                if (items != null)
                                {
                                    Globals.blankPrescriptionDetailSchedules = new ObservableCollection<PrescriptionDetailSchedules>(items);
                                }
                                else
                                {
                                    //ObjPrescriptionDetailSchedules_ByPrescriptDetailID = null;
                                }
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogError(ex.ToString());
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                bContinue = false;
                            }
                            finally
                            {
                                genTask.ActionComplete(bContinue);
                                if (!bContinue)
                                {
                                    this.HideBusyIndicator();
                                }
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogError(ex.ToString());
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }

            });
            t.Start();
        }

        private void LoadAllPclExamTypesAction(GenericCoRoutineTask genTask)
        {
            var t = new Thread(() =>
            {
                bool bContinue = true;
                //IsProcPCLForm = true;
                //NotifyOfPropertyChange(() => IsProcessing);
                //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0180_G1_DangTimDSDVCLS) });
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        var criteria = new PCLExamTypeSearchCriteria { V_PCLMainCategory = 0 };
                        client.BeginPCLItems_ByPCLFormID(criteria, 0, null, Globals.DispatchCallback(
                            delegate(IAsyncResult asyncResult)
                            {
                                try
                                {
                                    ObservableCollection<PCLExamType> ListAllPclExamTypes = new ObservableCollection<PCLExamType>();
                                    ListAllPclExamTypes = client.EndPCLItems_ByPCLFormID(asyncResult).ToObservableCollection();
                                    Globals.ListPclExamTypesAllPCLForms = new ObservableCollection<PCLExamType>();
                                    Globals.ListPclExamTypesAllPCLFormImages = new ObservableCollection<PCLExamType>();
                                    int nTot = 0, nCntLab = 0, nCntImage = 0;
                                    foreach (PCLExamType pclItem in ListAllPclExamTypes)
                                    {
                                        ++nTot;
                                        Globals.PCLExamTypeCollection.Add(pclItem);
                                        if (pclItem.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
                                        {
                                            ++nCntLab;
                                            Globals.ListPclExamTypesAllPCLForms.Add(pclItem);
                                        }
                                        else if (pclItem.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Imaging)
                                        {
                                            ++nCntImage;
                                            Globals.ListPclExamTypesAllPCLFormImages.Add(pclItem);
                                        }
                                    }
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    error = new AxErrorEventArgs(fault);
                                    bContinue = false;
                                }
                                catch (Exception ex)
                                {
                                    error = new AxErrorEventArgs(ex);
                                    bContinue = false;
                                }
                                finally
                                {
                                    if (error != null)
                                    {
                                        Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
                                    }
                                    IsProcPCLForm = false;
                                    genTask.ActionComplete(bContinue);
                                    if (!bContinue)
                                    {
                                        this.HideBusyIndicator();
                                    }
                                }

                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }

                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
                }

            });
            t.Start();
        }

        //ListPclExamTypesAllCombos
        private void ListPclExamTypesAllComboAction(GenericCoRoutineTask genTask)
        {
            var t = new Thread(() =>
            {
                //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0180_G1_DangTimDSDVCLS) });
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        bool bContinue = true;
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLExamTypeComboItems_All(null, Globals.DispatchCallback(delegate(IAsyncResult asyncResult)
                        {
                            IList<PCLExamTypeComboItem> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndPCLExamTypeComboItems_All(asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogError(fault.ToString());
                                error = new AxErrorEventArgs(fault);
                                bContinue = false;
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogError(ex.ToString());
                                error = new AxErrorEventArgs(ex);
                                bContinue = false;
                            }
                            finally
                            {
                                if (error != null)
                                {
                                    Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
                                }

                                if (Globals.ListPclExamTypesAllCombos == null)
                                {
                                    Globals.ListPclExamTypesAllCombos = new ObservableCollection<PCLExamTypeComboItem>();
                                    }

                                Globals.ListPclExamTypesAllCombos.Clear();

                                if (bOK)
                                {
                                    if (allItems != null)
                                    {
                                        foreach (var item in allItems)
                                        {
                                            Globals.ListPclExamTypesAllCombos.Add(item);
                                        }
                                    }
                                }

                                genTask.ActionComplete(bContinue);
                                if (!bContinue)
                                {
                                    this.HideBusyIndicator();
                                }

                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogError(ex.ToString());
                    error = new AxErrorEventArgs(ex);
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }

                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
                }
            });
            t.Start();
        }

        //ListPackageTechnicalServiceDetailAllAction
        private void ListPackageTechnicalServiceDetailAllAction(GenericCoRoutineTask genTask)
        {
            var t = new Thread(() =>
            {
                //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0180_G1_DangTimDSDVCLS) });
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        bool bContinue = true;
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPackageTechnicalServiceDetail_All(null, Globals.DispatchCallback(delegate (IAsyncResult asyncResult)
                        {
                            IList<PackageTechnicalServiceDetail> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndPackageTechnicalServiceDetail_All(asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogError(fault.ToString());
                                error = new AxErrorEventArgs(fault);
                                bContinue = false;
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogError(ex.ToString());
                                error = new AxErrorEventArgs(ex);
                                bContinue = false;
                            }
                            finally
                            {
                                if (error != null)
                                {
                                    Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
                                }

                                if (Globals.ListPackageTechnicalServiceDetailAll == null)
                                {
                                    Globals.ListPackageTechnicalServiceDetailAll = new ObservableCollection<PackageTechnicalServiceDetail>();
                                }

                                Globals.ListPackageTechnicalServiceDetailAll.Clear();

                                if (bOK)
                                {
                                    if (allItems != null)
                                    {
                                        foreach (var item in allItems)
                                        {
                                            Globals.ListPackageTechnicalServiceDetailAll.Add(item);
                                        }
                                    }
                                }

                                genTask.ActionComplete(bContinue);
                                if (!bContinue)
                                {
                                    this.HideBusyIndicator();
                                }

                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogError(ex.ToString());
                    error = new AxErrorEventArgs(ex);
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }

                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
                }
            });
            t.Start();
        }

        private void GetServerDateTime()
        {
            Coroutine.BeginExecute(GetCurrentServerDateTime());
            StartTimerToUpdateServerDateTime();
        }

        private void StartTimerToUpdateServerDateTime()
        {
            Globals.UpdSvrDateTimer = new System.Windows.Threading.DispatcherTimer();
            Globals.UpdSvrDateTimer.Interval = new TimeSpan(0, 0, 0, 2);
            Globals.UpdSvrDateTimer.Tick += new EventHandler(GlobalTimerElapsed);
            Globals.UpdSvrDateTimer.Start();
        }

        private static void GlobalTimerElapsed(object o, EventArgs sender)
        {
            if (Globals.ServerDate != null && Globals.ServerDate.HasValue)
            {
                if (++Globals.UpdSvrDateTimerCnt < 15)
                {
                    Globals.ServerDate = Globals.ServerDate.Value.AddSeconds(2);
                    //System.Diagnostics.Debug.WriteLine(" =====>>>>> ++++++  GlobalTimerElapsed: ServerDate Updated LOCALLY {0} ++++++  <<<<<=====", Globals.ServerDate.Value);
                    return;
                }
                else
                {
                    Globals.UpdSvrDateTimer.Stop();
                    Globals.UpdSvrDateTimerCnt = 0;
                    Coroutine.BeginExecute(GetCurrentServerDateTime());
                    //System.Diagnostics.Debug.WriteLine(" =====>>>>> ++++++  GlobalTimerElapsed: ServerDate Updated FROM THE SERVER {0} ++++++  <<<<<=====", Globals.ServerDate.Value);
                    Globals.UpdSvrDateTimer.Start();
                    if (!CheckValidSegments())
                    {
                        Globals.ShowMessage("Đã hết ca khám", eHCMSResources.G0442_G1_TBao);
                        Application.Current.Shutdown();
                    }
                }
            }
        }
        private static bool CheckValidSegments()
        {
            if (!Globals.ServerConfigSection.CommonItems.IsApplyTimeSegments)
            {
                return true;
            }
            int WarningOutTimeSegments = Globals.ServerConfigSection.CommonItems.WarningOutTimeSegments;
            if ((long)Globals.LoggedUserAccount.Staff.StaffCatgID != 1)
            {
                return true;
            }
            if (Globals.LoggedUserAccount.ConsultationTimeSegmentsList.Where(x => x.EndTime2 != null).Count() > 0)
            {
                if (Globals.LoggedUserAccount.ConsultationTimeSegmentsList.Where(x => (x.EndTime2.Value.TimeOfDay - Globals.ServerDate.Value.TimeOfDay).TotalMinutes < WarningOutTimeSegments).Count() > 0)
                {
                    Globals.EventAggregator.Publish(new ShowWaringSegments { });
                }
                if (Globals.LoggedUserAccount.ConsultationTimeSegmentsList.Where(x => x.StartTime2.Value.TimeOfDay < Globals.ServerDate.Value.TimeOfDay
                                     && x.EndTime2.Value.TimeOfDay > Globals.ServerDate.Value.TimeOfDay).Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (Globals.LoggedUserAccount.ConsultationTimeSegmentsList.Where(x => (x.EndTime.TimeOfDay - Globals.ServerDate.Value.TimeOfDay).TotalMinutes < WarningOutTimeSegments).Count() > 0)
                {
                    Globals.EventAggregator.Publish(new ShowWaringSegments { });
                }
                if (Globals.LoggedUserAccount.ConsultationTimeSegmentsList.Where(x => x.StartTime.TimeOfDay < Globals.ServerDate.Value.TimeOfDay
                                     && x.EndTime.TimeOfDay > Globals.ServerDate.Value.TimeOfDay).Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private static IEnumerator<IResult> GetCurrentServerDateTime()
        {
            var loadCurrentDate = new LoadCurrentDateTask();
            yield return loadCurrentDate;

            if (loadCurrentDate.CurrentDate == DateTime.MinValue)
            {                
                Globals.ShowMessage(string.Format("{0}.", eHCMSResources.Z1257_G1_KgLayDcNgThTuServer), eHCMSResources.G0442_G1_TBao);                
            }
            else
            {
                Globals.ServerDate = loadCurrentDate.CurrentDate;
            }
        }

        public void GetCustomerIP()
        {
                        
            var t = new Thread(() =>
            {
                using (var serviceFactory = new UserManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetCustomerIP(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var st = contract.EndGetCustomerIP(asyncResult);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            
                        }
                    }), null);
                }


            });
            t.Start();        
        }
        
        private void GetCurrentDate()
        {
            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetDate(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                DateTime date = contract.EndGetDate(asyncResult);
                                Globals.ServerDate = date;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                error = new AxErrorEventArgs(fault);
                            }
                            catch (Exception ex)
                            {
                                error = new AxErrorEventArgs(ex);
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                }
            });
            t.Start();
        }

        public void Handle(AppCheckAndDownloadUpdateCompletedEvent mess) 
        {
            //IsProcessing = true;
            IsNotProcessing = true;
            //_OOB_Software_HasNotBeen_Updated = false;
        }

        #region Danh Sách Load Sẵn


        #endregion
        //▼===== #002
        private bool _IsConfirmForSecretary = false;
        public bool IsConfirmForSecretary
        {
            get { return _IsConfirmForSecretary; }
            set
            {
                if (_IsConfirmForSecretary != value)
                {
                    _IsConfirmForSecretary = value;
                }
                NotifyOfPropertyChange(() => IsConfirmForSecretary);
            }
        }
        public void DeleteSecretaryCmd()
        {
            Globals.ConfirmSecretaryLogin = new UserAccount();
            Globals.ConfirmSecretaryLogin.Staff = new Staff();
            this.HideBusyIndicator();
            this.TryClose();
        }

        private IEnumerator<IResult> GetConfirmLoginForSecretary_Routine()
        {
            this.ShowBusyIndicator();
            var loginForSecretatyTask = new GenericCoRoutineTask(ProcessLoginAction);
            yield return loginForSecretatyTask;
            if ((loginForSecretatyTask.GetResultObj(1) as UserAccount).Staff.StaffCatgID != (long)StaffCatg.ThuKyYKhoa)
            {
                MessageBox.Show(eHCMSResources.Z2872_G1_KhongPhaiTYYK);
                this.HideBusyIndicator();
                IsConfirmForSecretary = false;
                this.TryClose();
                yield break;
            }
            Globals.ConfirmSecretaryLogin = loginForSecretatyTask.GetResultObj(1) as UserAccount;
            this.HideBusyIndicator();
            IsConfirmForSecretary = false;
            this.TryClose();
            yield break;
        }
        //▲===== #002
    }
}