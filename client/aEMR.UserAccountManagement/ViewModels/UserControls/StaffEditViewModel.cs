using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using Image = System.Windows.Controls.Image;
using Microsoft.Win32;
using aEMR.Common.Collections;
using aEMR.Common.BaseModel;

/*
 * #001 20180921 TNHX: Refactor code, Add DlgBusyIndicator
 * #002 20210107 TNHX: Add staffeditID
 */
namespace aEMR.UserAccountManagement.ViewModels
{
    [Export(typeof(IStaffEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class StaffEditViewModel : ViewModelBase, IStaffEdit
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public StaffEditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            GetAllRefStaffCategories();
            GetAllProvinces();
            GetAllCountries();
            GetAllEthnics();
            GetAllDepartments();
            GetAllReligion();
            GetAllMaritalStatus();
            GetAllBankName();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            //==== 20161206 CMN Begin: Disable method called in onloaded
            //GetAllRefStaffCategories();
            //GetAllProvinces();
            //GetAllCountries();
            //GetAllEthnics();
            //GetAllDepartments();
            //GetAllReligion();
            //GetAllMaritalStatus();
            //GetAllBankName();
            //==== 20161206 CMN End.
        }

        #region properties
        public object PerImage { get; set; }

        private ObservableCollection<RefStaffCategory> _allRefStaffCategory;
        public ObservableCollection<RefStaffCategory> allRefStaffCategory
        {
            get
            {
                return _allRefStaffCategory;
            }
            set
            {
                if (_allRefStaffCategory == value)
                    return;
                _allRefStaffCategory = value;
                NotifyOfPropertyChange(() => allRefStaffCategory);
            }
        }

        private ObservableCollection<Staff> _allStaff;
        public ObservableCollection<Staff> allStaff
        {
            get
            {
                return _allStaff;
            }
            set
            {
                if (_allStaff == value)
                    return;
                _allStaff = value;
                NotifyOfPropertyChange(() => allStaff);
            }
        }

        private Staff _SelectedStaff;
        public Staff SelectedStaff
        {
            get
            {
                return _SelectedStaff;
            }
            set
            {
                if (_SelectedStaff == value)
                    return;
                _SelectedStaff = value;
                
            }
        }
        
        private Staff _curStaff;
        public Staff curStaff
        {
            get
            {
                return _curStaff;
            }
            set
            {
                if (_curStaff == value)
                    return;
                _curStaff = value;
                NotifyOfPropertyChange(()=>curStaff);
                //NotifyOfPropertyChange(() => curStaff.RefDepartment);
                //NotifyOfPropertyChange(() => curStaff.RefStaffCategory);
                //NotifyOfPropertyChange(() => curStaff.RefCountry);
                //NotifyOfPropertyChange(() => curStaff.CitiesProvince);
                //NotifyOfPropertyChange(() => curStaff.Ethnic);
                //NotifyOfPropertyChange(() => curStaff.MaritalStatus);
                //NotifyOfPropertyChange(() => curStaff.Religion);
            }
        }

        private ObservableCollection<CitiesProvince> _allCitiesProvince;
        public ObservableCollection<CitiesProvince> allCitiesProvince
        {
            get
            {
                return _allCitiesProvince;
            }
            set
            {
                if (_allCitiesProvince == value)
                    return;
                _allCitiesProvince = value;
                NotifyOfPropertyChange(() => allCitiesProvince);
            }
        }
        
        private ObservableCollection<RefCountry> _allRefCountry;
        public ObservableCollection<RefCountry> allRefCountry
        {
          get
          {
            return _allRefCountry;
          }
          set
          {
            if (_allRefCountry == value)
              return;
            _allRefCountry = value;
              NotifyOfPropertyChange(()=>allRefCountry);
          }
        }

        private ObservableCollection<Lookup> _allEthnics;
        public ObservableCollection<Lookup> allEthnics
        {
            get
            {
                return _allEthnics;
            }
            set
            {
                if (_allEthnics == value)
                    return;
                _allEthnics = value;
                NotifyOfPropertyChange(() => allEthnics);
            }
        }

        private ObservableCollection<RefDepartment> _allRefDepartment;
        public ObservableCollection<RefDepartment> allRefDepartment
        {
            get
            {
                return _allRefDepartment;
            }
            set
            {
                if (_allRefDepartment == value)
                    return;
                _allRefDepartment = value;
                NotifyOfPropertyChange(() => allRefDepartment);
            }
        }

        private ObservableCollection<Lookup> _allReligion;
        public ObservableCollection<Lookup> allReligion
        {
            get
            {
                return _allReligion;
            }
            set
            {
                if (_allReligion == value)
                    return;
                _allReligion = value;
                NotifyOfPropertyChange(() => allReligion);
            }
        }
       
        private ObservableCollection<Lookup> _allMaritalStatus;
        public ObservableCollection<Lookup> allMaritalStatus
        {
            get
            {
                return _allMaritalStatus;
            }
            set
            {
                if (_allMaritalStatus == value)
                    return;
                _allMaritalStatus = value;
                NotifyOfPropertyChange(() => allMaritalStatus);
            }
        }
        
        private ObservableCollection<Lookup> _allBankName;
        public ObservableCollection<Lookup> allBankName
        {
            get
            {
                return _allBankName;
            }
            set
            {
                if (_allBankName == value)
                    return;
                _allBankName = value;
                NotifyOfPropertyChange(() => allBankName);
            }
        }
        #endregion

        public void PersonImageLoaded(object sender, RoutedEventArgs e)
        {
            PerImage = sender as Image;
            if(curStaff.PImage!=null)
            {
                ImageLoaded();
            }else
            {
                Uri uri = new Uri("/eHCMSCal;component/Assets/Images/Anonymous.png", UriKind.Relative);
                ImageSource img = new BitmapImage(uri);
                ((Image)PerImage).SetValue(Image.SourceProperty, img);    
            }
        }

        public void ImageLoaded()
        {
            //ImageSource img = new BitmapImage(uri);
            //((Image)PerImage).SetValue(Image.SourceProperty, img);

            Image image = new Image();
            BitmapImage bitmapimage = new BitmapImage();
            MemoryStream stream = new MemoryStream(curStaff.PImage);
            //bitmapimage.SetSource(stream);
            //((Image)PerImage).Source = bitmapimage;

            //((Image)PerImage).SetValue(Image.SourceProperty, image);
            
        }

        public void butBrowse()
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "All files(*.*)|*.*|PNG Images(*.png)|*.png|JPEG Images(*.jpg, *.jpeg)|*.jpg";
            ofd.FilterIndex = 1;
            if (true == ofd.ShowDialog())
            {
                //Stream stream = ofd.File.OpenRead();
                //BitmapImage bi = new BitmapImage();
                //bi.SetSource(stream);

                //((Image)PerImage).Source = bi;
                //stream.Position = 0;
                //byte[] data = new byte[stream.Length];
                //stream.Read(data, 0, (int)stream.Length);
                //curStaff.PImage = data;
                //stream.Close();
            }
        }
        
        public void butUpdate()
        {
            if(curStaff!=null)
            {
                //check valid
                if (curStaff.RefDepartment!=null)
                {
                    curStaff.DeptID = curStaff.RefDepartment.DeptID;
                }
                if (curStaff.RefCountry!= null)
                {
                    curStaff.CountryID = curStaff.RefCountry.CountryID;
                }
                if (curStaff.RefStaffCategory!= null)
                {
                    curStaff.StaffCatgID = curStaff.RefStaffCategory.StaffCatgID;
                }
                if (curStaff.CitiesProvince!= null)
                {
                    curStaff.CityProvinceID = curStaff.CitiesProvince.CityProvinceID;
                }
                if (curStaff.Ethnic != null)
                {
                    curStaff.V_Ethnic= curStaff.Ethnic.LookupID;
                }
                if (curStaff.MaritalStatus!= null)
                {
                    curStaff.V_MaritalStatus= curStaff.MaritalStatus.LookupID;
                }
                if (curStaff.Religion != null)
                {
                    curStaff.V_Religion= curStaff.Religion.LookupID;
                }
                if (curStaff.BankName != null)
                {
                    curStaff.V_BankName= curStaff.BankName.LookupID;
                }
                
                if(UpdateStaff(curStaff))
                {
                    TryClose();    
                }
            }
        }

        public void butExit()
        {
            TryClose();
        }

        #region method
        /*▼====: #001*/
        private void GetAllRefStaffCategories()
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ResourcesManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllRefStaffCategories(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetAllRefStaffCategories(asyncResult);
                                if (results != null)
                                {
                                    allRefStaffCategory = results.ToObservableCollection();
                                    NotifyOfPropertyChange(() => allRefStaffCategory);
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        private void GetAllProvinces()
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllProvinces(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetAllProvinces(asyncResult);
                                if (results != null)
                                {
                                    allCitiesProvince = results.ToObservableCollection();
                                    NotifyOfPropertyChange(() => allCitiesProvince);
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }
        
        private void GetAllCountries()
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllCountries(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetAllCountries(asyncResult);
                                if (results != null)
                                {
                                    allRefCountry = results.ToObservableCollection();
                                    NotifyOfPropertyChange(() => allRefCountry);
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        private void GetAllEthnics()
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllEthnics(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetAllEthnics(asyncResult);
                                if (results != null)
                                {
                                    allEthnics = results.ToObservableCollection();
                                    NotifyOfPropertyChange(() => allEthnics);
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        private void GetAllDepartments()
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllDepartments(false, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetAllDepartments(asyncResult);
                                if (results != null)
                                {
                                    allRefDepartment = results.ToObservableCollection();
                                    NotifyOfPropertyChange(() => allRefDepartment);
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        private void GetAllReligion()
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllReligion(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetAllReligion(asyncResult);
                                if (results != null)
                                {
                                    allReligion = results.ToObservableCollection();
                                    NotifyOfPropertyChange(() => allReligion);
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        private void GetAllMaritalStatus()
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllMaritalStatus(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetAllMaritalStatus(asyncResult);
                                if (results != null)
                                {
                                    allMaritalStatus = results.ToObservableCollection();
                                    NotifyOfPropertyChange(() => allMaritalStatus);
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        private void GetAllBankName()
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllBankName(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetAllBankName(asyncResult);
                                if (results != null)
                                {
                                    allBankName = results.ToObservableCollection();
                                    NotifyOfPropertyChange(() => allBankName);
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }

        public bool UpdateStaff(Staff newStaff)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            bool isSuccess = true;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        //▼====: #002
                        contract.BeginUpdateStaff(newStaff, Globals.LoggedUserAccount.StaffID, Globals.DispatchCallback((asyncResult) =>
                        //▲====: #002
                        {
                            try
                            {
                                var results = contract.EndUpdateStaff(asyncResult);
                                Globals.EventAggregator.Publish(new allStaffChangeEvent { });
                                //Globals.ShowMessage("Chỉnh sửa nhân viên thành công!", "");
                                MessageBox.Show(eHCMSResources.A0295_G1_Msg_InfoSuaNhVienOK);
                                isSuccess = true;
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                MessageBox.Show(eHCMSResources.A0294_G1_Msg_InfoSuaNhVienBiLoi);
                                isSuccess = false;
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    MessageBox.Show(eHCMSResources.A0294_G1_Msg_InfoSuaNhVienBiLoi);
                    isSuccess = false;
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
            return isSuccess;
        }
        /*▲====: #001*/
        #endregion
        public void txtUserNameKeyUp()
        {
            curStaff.FullName = curStaff.SLastName.Trim() + " " + curStaff.SMiddleName.Trim() + " " + curStaff.SFirstName.Trim();
        }

        public void txtUserNameLostFocus()
        {
            curStaff.SLastName = curStaff.SLastName.Trim();
            curStaff.SMiddleName = curStaff.SMiddleName.Trim();
            curStaff.SFirstName = curStaff.SFirstName.Trim();

            if (curStaff.FullName != "" && curStaff.SLastName != "" && curStaff.SMiddleName != "" && curStaff.SFirstName != "")
            {
                curStaff.FullName = curStaff.SLastName.Trim() + " " + curStaff.SMiddleName.Trim() + " " + curStaff.SFirstName.Trim();
            }
        }
    }
}
