using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using Castle.Core.Logging;
using Castle.Windsor;
using EPos.BusinessLayers;
using EPos.DataTransferObjects;
//using EPos.Domains;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Extensions;
using aEMR.Infrastructure.Utils;
using aEMR.Infrastructure.ViewUtils;
using EPos.ServiceContracts;
using EPos.ViewContracts;
using EPos.ViewContracts.SettingViewContracts;
using ESalePos.Views;
using Castle.Facilities.WcfIntegration;
using EPos.Common;
using System.Linq;
using aEMR.CommonTasks;
using aEMR.ViewContracts;

namespace ESalePos.ViewModels
{
    [Export(typeof(aEMR.ViewContracts.ILoginViewModel))]
    public class LoginViewModel : CommonView<StaffDto>, aEMR.ViewContracts.ILoginViewModel
    {
        private readonly INavigationService _navigationService;
        
        [ImportingConstructor]
        public LoginViewModel(IWindsorContainer container, INavigationService navigationService) : base(container)
        {
            _navigationService = navigationService;
        }

        private string _loginName;
        public string LoginName
        {
            get { return _loginName; }
            set
            {
                _loginName = value;
                NotifyOfPropertyChange(() => LoginName);
                NotifyOfPropertyChange(() => CanLoginCmd);
            }
        }

        public string Password
        {
            get
            {
                var password = string.Empty;
                var view = GetView() as LoginView;
                if ( view != null )
                {
                    password = view.PasswordBox.Password;
                }

                return password;
            }
            set
            {
                var view = GetView() as LoginView;
                if (view != null)
                {
                    view.PasswordBox.Password = value;
                }

                NotifyOfPropertyChange(() => CanLoginCmd);
            }
        }

     
        public override void Initial()
        {
            LoginName = null;
            var view = GetView() as LoginView;
            if (view != null)
            {
                view.PasswordBox.Password = null;
            }

            HeightShowLogin = 34;
            HeightShowSite = 0;

        }

        private void DisplayWarehouses( IEnumerable<WarehouseDto> warehouseDtos  )
        {
            if ( warehouseDtos != null )
            {
                WarehouseDtos = new List<WarehouseDto>(warehouseDtos);
            }
            LoginSuccessful(Globals.LoggedUser);
        }

        public void KeyUpCmd( KeyEventArgs eventArgs)
        {
            if ( eventArgs.Key == Key.Enter )
            {
                LoginCmd();
            }
            else
            {
                NotifyOfPropertyChange(() => CanLoginCmd);    
            }
        }

        public bool CanLoginCmd
        {
            get { return !string.IsNullOrEmpty(LoginName) && !string.IsNullOrEmpty(Password); }
        }


        private IEnumerator<IResult> ExecuteLogin()
        {
            yield return BusyIndicatorLoader.ShowBusy("Logon in progress, Please Wait .........");
            yield return
                AuthenticateAsyncLoader.Loader(new StaffDto { Loginname = LoginName, Pwd =
                    EncryptExtension.Encrypt(Password, Globals.AxonKey, Globals.AxonPass)
                }, ProcessLoginStepOne);            
            yield return BusyIndicatorLoader.HideBusy();

            //_navigationService.ShowDialog<IAboutViewModel>((vm) => { }, (opts, screen) => { }, MsgBoxOptions.Ok);
            //yield return ShopLocationConfirmMsgLoader.ShowMessage();
            
            yield return BusyIndicatorLoader.ShowBusy("Retrieving software configuration settings, Please wait .........");
            if (Globals.LoggedUser != null && Globals.LoggedUser.Staffpk > 0)
            {

                long? sitepk = null;
                if (checkUserIsWorker(Globals.LoggedUser))
                {
                    sitepk = Globals.LoggedUser.Sitepk;
                }
                //yield return WarehouseAsyncLoader.Loader(DisplayWarehouses, new WarehouseFilterDto { sitepk=sitepk});
                var warehouse = WarehouseDAsyncLoader.Loader(new WarehouseFilterDto { sitepk = sitepk });
                yield return warehouse;
                WarehouseDtos = warehouse.WarehouseDtos;
                LoginSuccessful(Globals.LoggedUser);

                
                var supplier = SupplierDAsyncLoader.Loader(new SupplierFilterDto());
                yield return supplier;
                Globals.Suppliers = supplier.SupplierDtos.ToObservableCollection();

                var category = CategoryDAsyncLoader.Loader(new CategoryFilterDto());
                yield return category;
                Globals.Categories = category.CategoryDtos.ToObservableCollection();

                var itemtypeInstance = ItemtypeInstanceDAsyncLoader.Loader();
                yield return itemtypeInstance;
                Globals.allItemtypeInstance = itemtypeInstance.ItemtypeInstanceDtos;

                var color = new ManualFieldValueAsyncLoader((int)LookupValueDto.V_FieldType.Color);
                yield return color;
                Globals.allColor = color.allFieldValueDto;
                var size = new ManualFieldValueAsyncLoader((int)LookupValueDto.V_FieldType.Size);
                yield return size;
                Globals.allSize = size.allFieldValueDto;
                var style = new ManualFieldValueAsyncLoader((int)LookupValueDto.V_FieldType.Style);
                yield return style;
                Globals.allStyle = style.allFieldValueDto;

                // LTN ----- 
                yield return GenericCoRoutineTask.StartTask(GetGlobalReferenceValues);
                 
            }
            yield return BusyIndicatorLoader.HideBusy();
        }
        // LTN 15/08/2015 Begin
        // method to get Global reference values from Lookup table in DB

        private void GetGlobalReferenceValues(GenericCoRoutineTask genTask)
        {
            var t = new Thread(() =>
            {
                try
                {
                    var _container = IoC.Get<IWindsorContainer>();
                    var _LookupService = _container.Resolve<ILookupService>();

                    //bool bContinue = true;

                    Lookups = new List<LookupDto>();

                    _LookupService.CallWcfAsync(s => s.FilterBy((long)LookupValueDto.ObjectType._globalValuesType),

                        (outLst, objData) =>
                        {
                            Lookups = outLst;
                            SetGlobalRefValues();
                            genTask.ActionComplete(true);
                        }

                        , null, null);


                    //contract.BeginGetAllStaffs(Globals.DispatchCallback((asyncResult) =>
                    //{
                    //    try
                    //    {
                    //        //lobals.AllStaffs = contract.EndGetAllStaffs(asyncResult).ToList();

                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        //Globals.ShowMessage(ex.Message, "Error");
                    //        //ClientLoggerHelper.LogError(ex.ToString());
                    //        bContinue = false;
                    //    }
                    //    finally
                    //    {
                    //        genTask.ActionComplete(bContinue);
                    //        if (!bContinue)
                    //        {
                    //            //this.HideBusyIndicator();
                    //        }
                    //    }

                    //}), null);


                }
                catch (Exception ex)
                {
                    //Globals.ShowMessage(ex.Message, "Error");
                    //ClientLoggerHelper.LogError(ex.ToString());
                    genTask.ActionComplete(false);
                    //this.HideBusyIndicator();
                }

            });

            t.Start();
        }

        // ltn 15/8/2015 --- gan gia tri trong csdl (bang lookup) vao trong global, de neu sau nay co muon thay doi, thi chi can thay doi trong csdl, k can thay doi code
        private void SetGlobalRefValues()
        {
            string strSaleAmtItemPkConfig = "SaleAmountAndFreeGiftCardPk";
            string strSaleAmtToApplySpecialDiscount = "SaleAmountToApplySpecialDiscount";
            int nCnt = 1;
            int nCnt2 = 1;
            foreach (var a in Lookups)
            {
                if (a.ObjectName == "GiftCardCategoryPk")
                {
                    Globals.CategorypkForPayGiftcard = Convert.ToInt32(a.ObjectValue);
                    continue;
                }
                if (a.ObjectName == "GiftCardExpiryTimeInDays")
                {
                    Globals.ExpirydateforGiftcard = Convert.ToInt32(a.ObjectValue);
                    continue;
                }

                string tmp;
                tmp = strSaleAmtItemPkConfig + "_" + nCnt.ToString();
                if (a.ObjectName == tmp)
                {
                    var theItem = new SaleAmountToGiftCardPk();
                    int commaPos = a.ObjectValue.IndexOf(',');
                    int nLenToAmt = a.ObjectValue.Length - commaPos - 1;
                    theItem.AmountFrom = Convert.ToDecimal(a.ObjectValue.Substring(0, commaPos));
                    theItem.AmountTo = Convert.ToDecimal(a.ObjectValue.Substring(commaPos + 1, nLenToAmt));
                  //  theItem.itempk = Convert.ToInt32(a.ObjectNotes);
                    //LTN --- 19/8/2015 --- them itemkey vo cho khi tim kiem de dang vi moi tim kiem deu tim bang itemkey 
                    theItem.itemkey = a.ObjectNotes;
                    if (Globals.SaleAmountToItemPkList == null)
                    {
                        Globals.SaleAmountToItemPkList = new List<SaleAmountToGiftCardPk>();
                    }
                    Globals.SaleAmountToItemPkList.Add(theItem);
                    nCnt++;
                    continue;
                }

                string tmp2;
                tmp2 = strSaleAmtToApplySpecialDiscount + "_" + nCnt2.ToString();
                if (a.ObjectName == tmp2)
                {
                    var theItem = new SaleAmountToApplySpecialDiscount();
                    int commaPos = a.ObjectValue.IndexOf(',');
                    int nLenToAmt = a.ObjectValue.Length - commaPos - 1;
                    theItem.AmountFrom = Convert.ToDecimal(a.ObjectValue.Substring(0, commaPos));
                    theItem.AmountTo = Convert.ToDecimal(a.ObjectValue.Substring(commaPos + 1, nLenToAmt));
                    theItem.DiscountPercent = Convert.ToDecimal(a.ObjectNotes);
                    if (Globals.SaleAmountToApplySpecialDiscountList == null)
                    {
                        Globals.SaleAmountToApplySpecialDiscountList = new List<SaleAmountToApplySpecialDiscount>();
                    }
                    Globals.SaleAmountToApplySpecialDiscountList.Add(theItem);
                    nCnt2++;
                    continue;
                }

            }

        }

        private IList<LookupDto> _Lookups;
        public IList<LookupDto> Lookups
        {
            get { return _Lookups; }
            set
            {
                _Lookups = value;
                NotifyOfPropertyChange(() => Lookups);
            }
        }

        private void LoadLookupCompleted(IList<LookupDto> lst, object data)
        {
            Lookups = lst;
            _navigationService.HideBusy();
        }


        // LTN 15/08/2015 End
       
        private IEnumerator<IResult> getWarehouse(StaffDto staff)
        {
            yield return BusyIndicatorLoader.ShowBusy();
            long? sitepk = null;
            if (checkUserIsWorker(staff))
            {
                sitepk = staff.Sitepk;
            }
            //yield return WarehouseAsyncLoader.Loader(DisplayWarehouses, new WarehouseFilterDto { sitepk=sitepk});
            var warehouse = WarehouseDAsyncLoader.Loader(new WarehouseFilterDto { sitepk = sitepk });
            yield return warehouse;
            WarehouseDtos = warehouse.WarehouseDtos;
            LoginSuccessful(Globals.LoggedUser);
            yield return BusyIndicatorLoader.HideBusy();
        }

        public void LoginCmd()
        {            
            if (HeightShowSite < 1)
            {
                this.TryExecute(() => Coroutine.BeginExecute(ExecuteLogin()), Logger);
            }
            else 
            {
                ProcessLogin(Globals.LoggedUser);
            }
        }

        private IList<WarehouseDto> _warehouseDtos;
        public IList<WarehouseDto> WarehouseDtos
        {
            get { return _warehouseDtos; }
            set
            {
                _warehouseDtos = value;
                NotifyOfPropertyChange(() => WarehouseDtos);
            }
        }

        private WarehouseDto _selectedWarehouseDto;
        public WarehouseDto SelectedWarehouseDto
        {
            get { return _selectedWarehouseDto; }
            set
            {
                _selectedWarehouseDto = value;
                NotifyOfPropertyChange(() => SelectedWarehouseDto);
            }
        }

        private int _heightShowLogin;
        public int HeightShowLogin
        {
            get { return _heightShowLogin; }
            set
            {
                _heightShowLogin = value;
                NotifyOfPropertyChange(() => HeightShowLogin);
            }
        }

        private int _heightShowSite;
        public int HeightShowSite
        {
            get { return _heightShowSite; }
            set
            {
                _heightShowSite = value;
                NotifyOfPropertyChange(() => HeightShowSite);
            }
        }
        public bool checkUserIsWorker(StaffDto loginUser) 
        {
            foreach (var item in loginUser.userAuth)
            {
                if (item.GroupID == (long)LookupValueDto.V_UserGroup.Administrator || item.GroupID == (long)LookupValueDto.V_UserGroup.Manager || item.GroupID == (long)LookupValueDto.V_UserGroup.ProductManager)
                {
                    return false;
                }
            }
            return true;
        }

        private void ProcessLogin( StaffDto loginUser )
        {
            if (loginUser != null && loginUser.Staffpk > 0)
            {
                var shellVm = IoC.Get<aEMR.ViewContracts.IShellViewModel>();
                Globals.LoggedUser = loginUser;                
                if (loginUser.Person != null)
                {
                    shellVm.UserName = loginUser.GetFullName();
                }
               
                //Check site of user here - only for worker
                if (checkUserIsWorker(loginUser))
                {
                    SelectedWarehouseDto = WarehouseDtos.Where(item=>item.Sitepk==loginUser.Sitepk).FirstOrDefault();
                    if (SelectedWarehouseDto != null)
                    {
                        Globals.SiteLogged = new Site
                        {
                            Sitepk = SelectedWarehouseDto.Sitepk.HasValue ? SelectedWarehouseDto.Sitepk.Value : 0,
                            Sitecode = SelectedWarehouseDto.SiteCode,
                            Sitename = SelectedWarehouseDto.SiteName
                        };

                        Globals.WarehouseDto = SelectedWarehouseDto;

                        shellVm.SiteName = SelectedWarehouseDto.SiteName;
                        //_navigationService.NavigationTo<IHomeViewModel>();
                        _navigationService.NavigationTo<aEMR.ViewContracts.IShopLocationConfirmViewModel>();
                        shellVm.isLogin = true;
                        shellVm.IsVisible = true;
                        TryClose();
                    }
                }
                else
                {
                    HeightShowLogin = 0;
                    HeightShowSite = 40;

                    if (SelectedWarehouseDto != null)
                    {
                        Globals.SiteLogged = new Site
                        {
                            Sitepk = SelectedWarehouseDto.Sitepk.HasValue ? SelectedWarehouseDto.Sitepk.Value : 0,
                            Sitecode = SelectedWarehouseDto.SiteCode,
                            Sitename = SelectedWarehouseDto.SiteName
                        };

                        Globals.WarehouseDto = SelectedWarehouseDto;

                        shellVm.SiteName = SelectedWarehouseDto.SiteName;
                        //_navigationService.NavigationTo<IHomeViewModel>();
                        _navigationService.NavigationTo<aEMR.ViewContracts.IShopLocationConfirmViewModel>();
                        shellVm.isLogin = true;
                        shellVm.IsVisible = true;
                        TryClose();
                    }
                }
            }
            else
            {
                LoginName = null;
                var loginView = (GetView() as LoginView);
                loginView.Dispatcher.BeginInvoke(new Func<string>(() => loginView.PasswordBox.Password = null));
                _navigationService.ShowMessage(" Login Fail ");
            }
        }


        private void ProcessLoginStepOne(StaffDto loginUser)
        {
            if (loginUser != null && loginUser.Staffpk > 0)
            {
                Globals.LoggedUser = loginUser;
            }
            else
            {
                LoginName = null;
                var loginView = (GetView() as LoginView);
                loginView.Dispatcher.BeginInvoke(new Func<string>(() => loginView.PasswordBox.Password = null));
                _navigationService.ShowMessage(" Login Fail ");
            }
        }

        private void LoginSuccessful(StaffDto loginUser)
        {
            var shellVm = IoC.Get<aEMR.ViewContracts.IShellViewModel>();
          
            if (loginUser.Person != null)
            {
                shellVm.UserName = loginUser.GetFullName();
            }

            //Check site of user here - only for worker
            if (checkUserIsWorker(loginUser))
            {
                SelectedWarehouseDto = WarehouseDtos.Where(item => item.Sitepk == loginUser.Sitepk).FirstOrDefault();
                if (SelectedWarehouseDto != null)
                {
                    Globals.SiteLogged = new Site
                    {
                        Sitepk = SelectedWarehouseDto.Sitepk.HasValue ? SelectedWarehouseDto.Sitepk.Value : 0,
                        Sitecode = SelectedWarehouseDto.SiteCode,
                        Sitename = SelectedWarehouseDto.SiteName
                    };

                    Globals.WarehouseDto = SelectedWarehouseDto;

                    shellVm.SiteName = SelectedWarehouseDto.SiteName;
                    //_navigationService.NavigationTo<IHomeViewModel>();
                    _navigationService.NavigationTo<aEMR.ViewContracts.IShopLocationConfirmViewModel>();
                    shellVm.isLogin = true;
                    shellVm.IsVisible = true;
                    TryClose();
                }
            }
            else
            {
                HeightShowLogin = 0;
                HeightShowSite = 40;

               
                if (SelectedWarehouseDto != null)
                {
                    Globals.SiteLogged = new Site
                    {
                        Sitepk = SelectedWarehouseDto.Sitepk.HasValue ? SelectedWarehouseDto.Sitepk.Value : 0,
                        Sitecode = SelectedWarehouseDto.SiteCode,
                        Sitename = SelectedWarehouseDto.SiteName
                    };

                    Globals.WarehouseDto = SelectedWarehouseDto;

                    shellVm.SiteName = SelectedWarehouseDto.SiteName;
                    //_navigationService.NavigationTo<IHomeViewModel>();
                    _navigationService.NavigationTo<aEMR.ViewContracts.IShopLocationConfirmViewModel>();
                    shellVm.isLogin = true;
                    shellVm.IsVisible = true;
                    TryClose();
                }
            }
        }

        public void AppExitCmd()
        {
            Application.Current.MainWindow.Close();
        }

    }
}
