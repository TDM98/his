/*
 * 20170914 #001 CMN: Added EmployeesReport
 * #002 20180921 TNHX: Apply DlgBusyIndicator, refactor code, title, button
 * #003 20180926 TNHX: Fix autocomplete templete, and clear button
 * #004 20210107 TNHX: Add StaffEditID
 * 20221208 #005 TNHX: 994 Thêm thông tin liên thông đơn thuốc điện tử + ràng buộc thông tin + kiểm tra user pass
*/
using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Image = System.Windows.Controls.Image;
using System.Linq;
using Microsoft.Win32;
using aEMR.Common.BaseModel;
using System.Net;
/*
 * 20230523 #001: QTD Sửa lại cách nối chuỗi họ tên
 * 20230708 #002: Thêm trường chức vụ
 */

namespace aEMR.UserAccountManagement.ViewModels
{
    [Export(typeof(IStaff)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class StaffViewModel : ViewModelBase, IStaff
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public StaffViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            JobPositionList = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_JobPosition).ToObservableCollection();
            Lookup firstItem = new Lookup();
            firstItem.LookupID = -1;
            firstItem.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0111_G1_HayChon1GTri);
            JobPositionList.Insert(0, firstItem);
        }
        private ObservableCollection<Lookup> _JobPositionList;
        public ObservableCollection<Lookup> JobPositionList
        {
            get { return _JobPositionList; }
            set
            {
                if (_JobPositionList != value)
                    _JobPositionList = value;
                NotifyOfPropertyChange(() => JobPositionList);
            }
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            LoadLookupValues();
            GetAllRefStaffCategories();
            GetAllProvinces();
            GetAllCountries();
            GetAllEthnics();
            GetAllDepartments();
            GetAllReligion();
            GetAllMaritalStatus();
            GetAllBankName();
            if (curStaff == null)
            {
                curStaff = new Staff();
            }
        }

        /*▼====: #002*/
        public void TitleForm_Loaded(TextBlock sender)
        {
            TitleForm = sender;
            TitleForm.Text = IsAddNew ? eHCMSResources.G0316_G1_ThemNhVien : eHCMSResources.K1671_G1_CNhatNhVien;
        }

        public TextBlock TitleForm { get; set; }
        /*▲====: #002*/

        public void txtUserNameKeyUp()
        {
            //▼====: #001
            //curStaff.FullName = curStaff.SLastName.Trim() + " " + curStaff.SMiddleName.Trim() + " " + curStaff.SFirstName.Trim();
            string sMiddleName = curStaff.SMiddleName.Trim() != "" ? " " + curStaff.SMiddleName.Trim() : "";
            string sFirstName = curStaff.SFirstName.Trim() != "" ? " " + curStaff.SFirstName.Trim() : "";
            curStaff.FullName = string.Format("{0}{1}{2}", curStaff.SLastName.Trim(), sMiddleName, sFirstName).Trim();
            //▲====: #001
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

        private bool _IsAddNew;
        public bool IsAddNew
        {
            get
            {
                return _IsAddNew;
            }
            set
            {
                if (_IsAddNew == value)
                    return;
                _IsAddNew = value;
                NotifyOfPropertyChange(() => IsAddNew);
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
                NotifyOfPropertyChange(() => curStaff);
                if (curStaff != null && curStaff.BankName != null)
                {
                    SelectedBankName = curStaff.BankName;
                }
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

        private CitiesProvince _defaultCitiesProvince;
        public CitiesProvince defaultCitiesProvince
        {
            get
            {
                return _defaultCitiesProvince;
            }
            set
            {
                if (_defaultCitiesProvince == value)
                    return;
                _defaultCitiesProvince = value;
                NotifyOfPropertyChange(() => defaultCitiesProvince);
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
                NotifyOfPropertyChange(() => allRefCountry);
            }
        }

        private RefCountry _defaultRefCountry;
        public RefCountry defaultRefCountry
        {
            get
            {
                return _defaultRefCountry;
            }
            set
            {
                if (_defaultRefCountry == value)
                    return;
                _defaultRefCountry = value;
                NotifyOfPropertyChange(() => defaultRefCountry);
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

        private Lookup _defaultEthnics;
        public Lookup defaultEthnics
        {
            get
            {
                return _defaultEthnics;
            }
            set
            {
                if (_defaultEthnics == value)
                    return;
                _defaultEthnics = value;
                NotifyOfPropertyChange(() => defaultEthnics);
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

        private Lookup _SelectedReligion;
        public Lookup SelectedReligion
        {
            get
            {
                return _SelectedReligion;
            }
            set
            {
                if (_SelectedReligion == value)
                    return;
                _SelectedReligion = value;
                NotifyOfPropertyChange(() => SelectedReligion);
                if (SelectedReligion != null)
                {
                    curStaff.Religion = SelectedReligion;
                    curStaff.V_Religion = SelectedReligion.LookupID;
                }
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

        private Lookup _SelectedMaritalStatus;
        public Lookup SelectedMaritalStatus
        {
            get
            {
                return _SelectedMaritalStatus;
            }
            set
            {
                if (_SelectedMaritalStatus == value)
                    return;
                _SelectedMaritalStatus = value;
                NotifyOfPropertyChange(() => SelectedMaritalStatus);
                if (SelectedMaritalStatus != null)
                {
                    curStaff.MaritalStatus = SelectedMaritalStatus;
                    curStaff.V_MaritalStatus = SelectedMaritalStatus.LookupID;
                }
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

        private Lookup _SelectedBankName;
        public Lookup SelectedBankName
        {
            get
            {
                return _SelectedBankName;
            }
            set
            {
                if (_SelectedBankName == value)
                    return;
                _SelectedBankName = value;
                NotifyOfPropertyChange(() => SelectedBankName);
            }
        }
        #endregion

        /*▼====: #002*/
        public Button butS { get; set; }
        public void butSaveLoaded(object sender)
        {
            butS = (Button)sender;
            butS.Content = IsAddNew ? eHCMSResources.G0316_G1_ThemNhVien : eHCMSResources.K1671_G1_CNhatNhVien;
        }
        /*▲====: #002*/

        public void PersonImageLoaded(object sender, RoutedEventArgs e)
        {
            PerImage = sender as Image;
            Uri uri = new Uri("/eHCMSCal;component/Assets/Images/Anonymous.png", UriKind.Relative);
            ImageSource img = new BitmapImage(uri);
            ((Image)PerImage).SetValue(Image.SourceProperty, img);
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

        public void butSaveClick()
        {
            if (curStaff != null)
            {
                //check valid
                if (curStaff.FullName == null || curStaff.FullName == "")
                {
                    MessageBox.Show(eHCMSResources.A0422_G1_Msg_InfoChuaNhapTenNhVien);
                    return;
                }
                //20181208 TBL: BM 0005325. Rang buoc bac si va dieu duong can phai nhap chung chi hanh nghe
                if (curStaff.RefStaffCategory != null && (curStaff.RefStaffCategory.V_StaffCatType == (long)V_StaffCatType.BacSi || curStaff.RefStaffCategory.V_StaffCatType == (long)V_StaffCatType.PhuTa)
                    && (curStaff.SCertificateNumber == null || curStaff.SCertificateNumber == ""))
                {
                    MessageBox.Show(eHCMSResources.Z2378_G1_ChuaNhapCCHN);
                    return;
                }
                //▼====: #002
                if (curStaff.V_JobPosition == 0)
                {
                    MessageBox.Show("Trường chức vụ chưa chọn. Vui lòng kiểm tra lại!");
                    return;
                }
                //▲====: #002
                //----------------------------
                //▼====: #005
                // kiểm tra 3 trường thông tin cần lưu + call API kiểm tra user + pass của đơn thuốc điện tử 
                if (!string.IsNullOrEmpty(curStaff.MaDinhDanhBsi) || !string.IsNullOrEmpty(curStaff.MaLienThongBacSi) || !string.IsNullOrEmpty(PwdBoxCtrl.Password))
                {
                    string errorMess = "";
                    if (string.IsNullOrEmpty(curStaff.MaDinhDanhBsi))
                    {
                        errorMess = System.Environment.NewLine + "- Mã định danh";
                    }
                    if (string.IsNullOrEmpty(curStaff.MaLienThongBacSi))
                    {
                        errorMess = errorMess + System.Environment.NewLine + "- Tài khoản ĐT";
                    }
                    if (string.IsNullOrEmpty(PwdBoxCtrl.Password) && ModifiedPasswordDT)
                    {
                        errorMess = errorMess + System.Environment.NewLine + "- Mật khẩu ĐT";
                    }
                    if (!string.IsNullOrEmpty(errorMess))
                    {
                        MessageBox.Show("Người dùng chưa nhập thông tin các trường: " + errorMess + System.Environment.NewLine + "Vui lòng nhập đầy đủ thông tin ở các trường bắt buộc nhập!");
                        return;
                    }
                    // Call Api check user + pass đúng sai
                    try
                    {
                        if (ModifiedPasswordDT)
                        {
                            CheckUserPassFromDonThuocQuocGia();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }
                }
                if (!string.IsNullOrEmpty(PwdBoxCtrl.Password))
                {
                    curStaff.ModifiedPasswordDT = true;
                    curStaff.MatKhauLienThongBacSi = EncryptExtension.Encrypt(PwdBoxCtrl.Password, Globals.AxonKey, Globals.AxonPass);
                }
                //▲====: #005
                if (curStaff.RefDepartment != null)
                {
                    curStaff.DeptID = curStaff.RefDepartment.DeptID;
                }
                if (curStaff.RefCountry != null)
                {
                    curStaff.CountryID = curStaff.RefCountry.CountryID;
                }
                if (curStaff.RefStaffCategory != null)
                {
                    curStaff.StaffCatgID = curStaff.RefStaffCategory.StaffCatgID;
                }
                if (curStaff.CitiesProvince != null)
                {
                    curStaff.CityProvinceID = curStaff.CitiesProvince.CityProvinceID;
                }
                if (curStaff.Ethnic != null)
                {
                    curStaff.V_Ethnic = curStaff.Ethnic.LookupID;
                }
                if (curStaff.MaritalStatus != null)
                {
                    curStaff.V_MaritalStatus = curStaff.MaritalStatus.LookupID;
                }
                if (curStaff.Religion != null)
                {
                    curStaff.V_Religion = curStaff.Religion.LookupID;
                }
                if (curStaff.BankName != null)
                {
                    curStaff.V_BankName = curStaff.BankName.LookupID;
                }                
                if (!IsAddNew)
                    UpdateStaff(curStaff);
                else
                    AddNewStaff(curStaff);
            }
        }

        public void butClear()
        {
            curStaff = new Staff();
            curStaff.CitiesProvince = defaultCitiesProvince;

            curStaff.Ethnic = defaultEthnics;
            /*▼====: #003*/
            SelectedMaritalStatus = null;
            SelectedAcademicDegree = null;
            SelectedAcademicRank = null;
            SelectedEducation = null;
            SelectedBankName = null;
            curStaff.RefCountry = defaultRefCountry;
            /*▲====: #003*/
        }

        #region method
        /*▼====: #002*/
        private void GetAllRefStaffCategories()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
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
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void GetAllProvinces()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
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
                                    foreach (var p in allCitiesProvince)
                                    {
                                        if (curStaff != null && curStaff.RefCountry == null
                                            && p.CityProvinceName.ToUpper().Contains("HOCHIMINH"))
                                        {
                                            defaultCitiesProvince = p;
                                            //SelectedCitiesProvince = p;
                                            curStaff.CitiesProvince = p;
                                            NotifyOfPropertyChange(() => curStaff.CitiesProvince);
                                        }
                                    }

                                    NotifyOfPropertyChange(() => allCitiesProvince);
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void GetAllCountries()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
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
                                    foreach (var p in allRefCountry)
                                    {
                                        if (curStaff != null && curStaff.RefCountry == null
                                            && p.CountryName.ToUpper().Contains("VIET"))
                                        {
                                            defaultRefCountry = p;
                                            //SelectedRefCountry = p;
                                            curStaff.RefCountry = p;
                                            NotifyOfPropertyChange(() => curStaff.RefCountry);
                                        }
                                    }

                                    NotifyOfPropertyChange(() => allRefCountry);
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void GetAllEthnics()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
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
                                    foreach (var p in allEthnics)
                                    {
                                        if (curStaff != null
                                            && curStaff.Ethnic == null
                                            && p.ObjectValue.ToUpper().Contains("KINH"))
                                        {
                                            defaultEthnics = p;
                                            //SelectedEthnics = p;
                                            curStaff.Ethnic = p;
                                            NotifyOfPropertyChange(() => curStaff.Ethnic);
                                        }
                                    }

                                    NotifyOfPropertyChange(() => allEthnics);
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void GetAllDepartments()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
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
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void GetAllReligion()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
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
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void GetAllMaritalStatus()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
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
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void GetAllBankName()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
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
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void AddNewStaff(Staff newStaff)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginAddNewStaff(newStaff, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndAddNewStaff(asyncResult);
                                MessageBox.Show(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK);
                                butClear();
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                MessageBox.Show(ex.Message);
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
            bool isSuccess = true;
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        //▼====: #004
                        contract.BeginUpdateStaff(newStaff, Globals.LoggedUserAccount.StaffID, Globals.DispatchCallback((asyncResult) =>
                        //▲====: #004
                        {
                            try
                            {
                                var results = contract.EndUpdateStaff(asyncResult);
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
            return isSuccess;
        }
        /*▲====: #002*/
        #endregion

        /*▼====: #001*/
        private ObservableCollection<Lookup> _AcademicRanks;
        public ObservableCollection<Lookup> AcademicRanks
        {
            get
            {
                return _AcademicRanks;
            }
            set
            {
                if (_AcademicRanks == value)
                    return;
                _AcademicRanks = value;
                NotifyOfPropertyChange(() => AcademicRanks);
            }
        }

        private ObservableCollection<Lookup> _AcademicDegrees;
        public ObservableCollection<Lookup> AcademicDegrees
        {
            get
            {
                return _AcademicDegrees;
            }
            set
            {
                if (_AcademicDegrees == value)
                    return;
                _AcademicDegrees = value;
                NotifyOfPropertyChange(() => AcademicDegrees);
            }
        }

        private ObservableCollection<Lookup> _Educations;
        public ObservableCollection<Lookup> Educations
        {
            get
            {
                return _Educations;
            }
            set
            {
                if (_Educations == value)
                    return;
                _Educations = value;
                NotifyOfPropertyChange(() => Educations);
            }
        }

        private Lookup _SelectedAcademicRank;
        public Lookup SelectedAcademicRank
        {
            get
            {
                return _SelectedAcademicRank;
            }
            set
            {
                _SelectedAcademicRank = value;
                NotifyOfPropertyChange(() => SelectedAcademicRank);
            }
        }

        private Lookup _SelectedAcademicDegree;
        public Lookup SelectedAcademicDegree
        {
            get
            {
                return _SelectedAcademicDegree;
            }
            set
            {
                _SelectedAcademicDegree = value;
                NotifyOfPropertyChange(() => SelectedAcademicDegree);
            }
        }

        private Lookup _SelectedEducation;
        public Lookup SelectedEducation
        {
            get
            {
                return _SelectedEducation;
            }
            set
            {
                _SelectedEducation = value;
                NotifyOfPropertyChange(() => SelectedEducation);
            }
        }

        private Lookup _SelectedMajor;
        public Lookup SelectedMajor
        {
            get
            {
                return _SelectedMajor;
            }
            set
            {
                _SelectedMajor = value;
                NotifyOfPropertyChange(() => SelectedMajor);
            }
        }

        private void LoadLookupValues()
        {
            this.ShowBusyIndicator();
            LoadAcademicRanks();
            LoadAcademicDegrees();
            LoadEducations();
            this.HideBusyIndicator();
        }

        public void LoadAcademicRanks()
        {
            AcademicRanks = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_AcademicRank).ToObservableCollection();
            if (curStaff != null && curStaff.V_AcademicRank > 0 && AcademicRanks != null)
                SelectedAcademicRank = AcademicRanks.Where(x => x.LookupID == curStaff.V_AcademicRank).FirstOrDefault();
        }

        public void LoadAcademicDegrees()
        {
            AcademicDegrees = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_AcademicDegree).ToObservableCollection();
            if (curStaff != null && curStaff.V_AcademicDegree > 0 && AcademicDegrees != null)
                SelectedAcademicDegree = AcademicDegrees.Where(x => x.LookupID == curStaff.V_AcademicDegree).FirstOrDefault();
        }

        public void LoadEducations()
        {
            Educations = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_Education).ToObservableCollection();
            if (curStaff != null && curStaff.V_Education > 0 && Educations != null)
                SelectedEducation = Educations.Where(x => x.LookupID == curStaff.V_Education).FirstOrDefault();
        }
        /*▼====: #003*/
        public AutoCompleteBox CboAcademicRank { get; set; }
        public void cboAcademicRank_Loaded(object sender, RoutedEventArgs e)
        {
            CboAcademicRank = sender as AutoCompleteBox;
        }

        public void cboAcademicRank_Populating(object sender, PopulatingEventArgs e)
        {
            e.Cancel = true;
            var AllItemsContext = new ObservableCollection<Lookup>(AcademicRanks.Where(x => x.ObjectValue.ToLower().Contains(CboAcademicRank.SearchText.ToLower())).ToList());
            CboAcademicRank.ItemsSource = AllItemsContext;
            CboAcademicRank.PopulateComplete();
        }
        /*▲====: #003*/

        public void cboAcademicRank_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            SelectedAcademicRank = ((AutoCompleteBox)sender).SelectedItem as Lookup;
            curStaff.V_AcademicRank = SelectedAcademicRank != null ? (long?)SelectedAcademicRank.LookupID : null;
        }
        /*▼====: #003*/
        public AutoCompleteBox CboAcademicDegree { get; set; }
        public void cboAcademicDegree_Loaded(object sender, RoutedEventArgs e)
        {
            CboAcademicDegree = sender as AutoCompleteBox;
        }

        public void cboAcademicDegree_Populating(object sender, PopulatingEventArgs e)
        {
            e.Cancel = true;
            var AllItemsContext = new ObservableCollection<Lookup>(AcademicDegrees.Where(x => x.ObjectValue.ToLower().Contains(CboAcademicDegree.SearchText.ToLower())).ToList());
            CboAcademicDegree.ItemsSource = AllItemsContext;
            CboAcademicDegree.PopulateComplete();
        }
        /*▲====: #003*/

        public void cboAcademicDegree_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            SelectedAcademicDegree = ((AutoCompleteBox)sender).SelectedItem as Lookup;
            curStaff.V_AcademicDegree = SelectedAcademicDegree != null ? (long?)SelectedAcademicDegree.LookupID : null;
        }

        /*▼====: #003*/
        public AutoCompleteBox CboEducation { get; set; }
        public void cboEducation_Loaded(object sender, RoutedEventArgs e)
        {
            CboEducation = sender as AutoCompleteBox;
        }

        public void cboEducation_Populating(object sender, PopulatingEventArgs e)
        {
            e.Cancel = true;
            var AllItemsContext = new ObservableCollection<Lookup>(Educations.Where(x => x.ObjectValue.ToLower().Contains(CboEducation.SearchText.ToLower())).ToList());
            CboEducation.ItemsSource = AllItemsContext;
            CboEducation.PopulateComplete();
        }
        /*▲====: #003*/

        public void cboEducation_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            SelectedEducation = ((AutoCompleteBox)sender).SelectedItem as Lookup;
            curStaff.V_Education = SelectedEducation != null ? (long?)SelectedEducation.LookupID : null;
        }
        /*▲====: #001*/

        //▼====: #005
        public void ThePasswordBox_Loaded(object sender, RoutedEventArgs e)
        {
            PwdBoxCtrl = sender as PasswordBox;
        }
        public PasswordBox PwdBoxCtrl { get; set; }

        private bool _ModifiedPasswordDT = false;
        public bool ModifiedPasswordDT
        {
            get
            {
                return _ModifiedPasswordDT;
            }
            set
            {
                if (_ModifiedPasswordDT == value)
                    return;
                _ModifiedPasswordDT = value;
                NotifyOfPropertyChange(() => ModifiedPasswordDT);
            }
        }

        public void btnResetPassword(object sender)
        {
            ModifiedPasswordDT = true;
        }

        public void CheckUserPassFromDonThuocQuocGia()
        {
            string mJsonData = string.Format("{{\"ma_lien_thong_bac_si\":\"{0}\",\"password\":\"{1}\",\"ma_lien_thong_co_so_kham_chua_benh\":\"{2}\"}}"
            , curStaff.MaLienThongBacSi, PwdBoxCtrl.Password, Globals.ServerConfigSection.CommonItems.DTDTUsername);
            try
            {
                var resultResponse = GlobalsNAV.RequestPOST(Globals.ServerConfigSection.CommonItems.DonThuocQuocGiaAPIUrl, "/api/auth/dang-nhap-bac-si", mJsonData);
            }
            catch (Exception ex)
            {
                throw new Exception(System.Text.RegularExpressions.Regex.Unescape(ex.Message));
            }
        }
        //▲====: #005
    }
}
