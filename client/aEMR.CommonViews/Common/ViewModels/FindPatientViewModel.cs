using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ServiceModel;
using System.Threading;
using System.Windows.Input;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using Caliburn.Micro;
using aEMR.ViewContracts;
using System.ComponentModel.Composition;
using DataEntities;
using aEMR.Common.Collections;
using eHCMS.Services.Core;
using System.Windows;
using aEMR.Controls;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Linq;
using System.Windows.Controls;
using aEMR.Common.BaseModel;
using System.Text.RegularExpressions;
/*
* 20170113 #001 CMN: Added QRCode
* 20171103 #002 CMN: Added Show HICardNo
* 20180904 #003 TTM: Ngăn không cho tìm kiếm rỗng
* 20181122 #004 TTM: BM 0005299: Cho phép tìm kiếm bệnh nhân kèm DOB
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IFindPatient)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class FindPatientViewModel : ViewModelBase, IFindPatient
         , IHandle<AddCompleted<Patient>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        public const string HICARD_REG_EXP = @"^(DN|HX|CH|NN|TK|HC|XK|CA|HT|TB|MS|XB|XN|TN|CC|CK|CB|KC|HD|BT|HN|TC|TQ|TA|TY|TE|HG|LS|CN|HS|GD|TL|XV|NO)([1-7])(\d{2})(\d{2})(\d{3})(\d{5})$";
        public const string PATIENT_CODE_REG_EXP = "^[0-9]{8}$";

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);

            //if (!DesignerProperties.IsInDesignTool)
            bool designTime = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());
            if (!designTime)
            {
                LoadGenders();
            }

            _criteria = _searchCriteria;
            Patients.PageIndex = 0;
            //SearchPatients(0, Patients.PageSize, true);

            //▼===== #005
            SelectedProvince = new CitiesProvince();
            SuburbNames = new ObservableCollection<SuburbNames>();
            SelectedSuburbName = new SuburbNames();
            LoadProvinces();
            GetAllSuburbName();
            //▲===== #005
        }
        protected override void OnDeactivate(bool close)
        {
            Globals.EventAggregator.Unsubscribe(this);
            base.OnDeactivate(close);
        }

        [ImportingConstructor]
        public FindPatientViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            SearchCriteria = new PatientSearchCriteria();

            Patients = new PagedSortableCollectionView<Patient>();
            Patients.OnRefresh += new WeakEventHandler<aEMR.Common.Collections.RefreshEventArgs>(Patients_OnRefresh).Handler;
        }

        public void Patients_OnRefresh(object sender, aEMR.Common.Collections.RefreshEventArgs e)
        {
            SearchPatients(Patients.PageIndex, Patients.PageSize, false);
        }

        private bool _IsSearchGoToKhamBenh;

        public bool IsSearchGoToKhamBenh
        {
            get { return _IsSearchGoToKhamBenh; }
            set
            {
                _IsSearchGoToKhamBenh = value;
                NotifyOfPropertyChange(() => IsSearchGoToKhamBenh);
            }
        }

        public void CancelCmd()
        {
            SelectedPatient = null;
            TryClose();
        }

        public void OkCmd()
        {
            Globals.EventAggregator.Publish(new ItemSelected<Patient>() { Item = SelectedPatient });
            TryClose();
        }

        private Patient _selectedPatient;

        public Patient SelectedPatient
        {
            get { return _selectedPatient; }
            set
            {
                _selectedPatient = value;
                NotifyOfPropertyChange(() => SelectedPatient);
            }
        }

        private ObservableCollection<Gender> _genders;
        public ObservableCollection<Gender> Genders
        {
            get { return _genders; }
            set
            {
                _genders = value;
                NotifyOfPropertyChange(() => Genders);
            }
        }

        /// <summary>
        /// Biến này lưu lại thông tin người dùng tìm kiếm trên form, 
        /// </summary>
        private PatientSearchCriteria _criteria;

        private PatientSearchCriteria _searchCriteria;
        public PatientSearchCriteria SearchCriteria
        {
            get { return _searchCriteria; }
            set
            {
                _searchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }

        private PagedSortableCollectionView<Patient> _patients;
        public PagedSortableCollectionView<Patient> Patients
        {
            get { return _patients; }
            private set
            {
                _patients = value;
                NotifyOfPropertyChange(() => Patients);
            }
        }

        public void CreatePatientCmd()
        {
            //var vm = Globals.GetViewModel<IPatientDetails>();
            //vm.CurrentAction = eHCMSResources.Z0037_G1_ThemBN;
            //vm.CreateNewPatient();
            ////==== #001
            //vm.QRCode = SearchCriteria.QRCode;
            ////==== #001
            //Globals.ShowDialog(vm as Conductor<object>);

            IPatientDetails mView = Globals.GetViewModel<IPatientDetails>();
            mView.CurrentAction = eHCMSResources.Z0037_G1_ThemBN;
            mView.CreateNewPatient();
            mView.QRCode = SearchCriteria.QRCode;
            GlobalsNAV.ShowDialog_V3<IPatientDetails>(mView);
            if (IsAddingMultiples && mView.CurrentPatient != null && mView.CurrentPatient.PatientID > 0)
            {
                Patients.Clear();
                Patients.Add(mView.CurrentPatient);
            }
        }
        //▼====== #003
        AxSearchPatientTextBox txtNameTextBox;
        public void txtName_Loaded(object sender, RoutedEventArgs e)
        {
            txtNameTextBox = (AxSearchPatientTextBox)sender;
        }
        private void SearchByNameAndDOB(string FullName, int DOBNumIndex)
        {
            if (SelectedSuburbName != null)
            {
                _searchCriteria.SuburbNameID = SelectedSuburbName.SuburbNameID;
            }
            if (SelectedProvince != null)
            {
                _searchCriteria.CityProvinceID = SelectedProvince.CityProvinceID;
            }
            _searchCriteria.FullName = FullName;
            _searchCriteria.DOBNumIndex = DOBNumIndex;
            _searchCriteria.PatientCode = null;
            _criteria = _searchCriteria.DeepCopy();
            Patients.PageIndex = 0;
            SearchPatients(0, Patients.PageSize, true);
        }
        public void SearchCmd()
        {
            if (String.IsNullOrEmpty(txtNameTextBox.Text))
            {
                MessageBox.Show(eHCMSResources.Z2291_G1_CanhBaoNhapDuChiTiet);
                return;
            }

            //▼====== #004
            if (txtNameTextBox.Text.Contains(':'))
            {
                int DOBNumIndex = 0;
                string FullName = "";
                //20181122 TTM: Hàm GetValueForSearchByNameAndDOB dùng để lấy giá trị cho DOBNumIndex để tìm kiếm kèm DOB, nếu DOBNumIndex = 0 sẽ tìm kiếm bằng tên.
                Globals.GetValueForSearchByNameAndDOB(txtNameTextBox.Text, out DOBNumIndex, out FullName);
                SearchByNameAndDOB(FullName, DOBNumIndex);
            }
            //▲====== #004
            else
            {
                _criteria = _searchCriteria.DeepCopy();
                //▼===== #005
                if (SelectedSuburbName != null)
                {
                    _criteria.SuburbNameID = SelectedSuburbName.SuburbNameID;
                }
                if (SelectedProvince != null)
                {
                    _criteria.CityProvinceID = SelectedProvince.CityProvinceID;
                }
                //▲===== #005
                _criteria.DOBNumIndex = 0;
                Patients.PageIndex = 0;
                SearchPatients(0, Patients.PageSize, true);
            }
        }
        //▲====== #003
        private void SearchPatients(int pageIndex, int pageSize, bool bCountTotal)
        {
            /*▼====: #002*/
            if (SearchCriteria.IsShowHICardNo && gGvPatients != null)
                gGvPatients.Columns.Where(x => x.Header != null && x.Header.ToString() == "HICardNo").FirstOrDefault().Visibility = Visibility.Visible;
            else if (gGvPatients != null)
                gGvPatients.Columns.Where(x => x.Header != null && x.Header.ToString() == "HICardNo").FirstOrDefault().Visibility = Visibility.Collapsed;
            /*▲====: #002*/
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z1097_G1_DangTimBN) });

            if (SearchCriteria.BirthDateEnabled)
            {
                if (string.IsNullOrEmpty(DateOfBirthBegin))
                {
                    MessageBox.Show(eHCMSResources.Z3058_G1_NhapNamTimKiem);
                    return;
                }
                else
                {
                    _criteria.BirthDateBegin = new DateTime(Convert.ToInt32(DateOfBirthBegin), 01, 01);
                    if (string.IsNullOrEmpty(DateOfBirthEnd))
                    {
                        DateOfBirthEnd = DateOfBirthBegin;
                        _criteria.BirthDateEnd = new DateTime(Convert.ToInt32(DateOfBirthEnd), 12, 31);
                    }
                    else
                    {
                        _criteria.BirthDateEnd = new DateTime(Convert.ToInt32(DateOfBirthEnd), 12, 31);
                    }
                }
            }

            this.DlgShowBusyIndicator(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z1097_G1_DangTimBN));
            var t = new Thread(() =>
            {
                try
                {
                    //IsLoading = true;
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginSearchPatients(_criteria, pageIndex, pageSize, bCountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int totalCount = 0;
                            IList<Patient> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndSearchPatients(out totalCount, asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }

                            Patients.Clear();
                            if (bOK)
                            {
                                if (bCountTotal)
                                {
                                    Patients.TotalItemCount = totalCount;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        Patients.Add(item);
                                    }

                                }
                            }
                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }

        public void ResetFilterCmd()
        {
            SearchCriteria = new PatientSearchCriteria();
        }

        public void DoubleClick(object sender, MouseButtonEventArgs args)
        {
            //EventArgs<object> eventArgs = args as EventArgs<object>;
            if ((sender as DataGrid).SelectedItem == null) return;
            SelectedPatient = (sender as DataGrid).SelectedItem as Patient;
            if (!string.IsNullOrEmpty(GlobalsNAV.HIRegistrationForm))
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        //==== #001
                        var mPatient = SelectedPatient;
                        if (SearchCriteria.QRCode != null)
                        {
                            mPatient.QRCode = SearchCriteria.QRCode;
                        }
                        //==== #001
                        if (IsPresenter && Globals.ServerConfigSection.OutRegisElements.IsPerformingTMVFunctionsA) //20200225 TBL: Nếu popup được mở từ người giới thiệu thì bắn sự kiện này
                        {
                            Globals.EventAggregator.Publish(new SelectPresenter() { PatientInfo = mPatient });
                        }
                        else
                        {
                            Globals.EventAggregator.Publish(new ItemSelected<Patient>() { Item = mPatient });
                            GlobalsNAV.msgb = null;
                            Globals.HIRegistrationForm = "";
                        }
                        TryClose();
                    }
                    else
                    {
                        TryClose();
                    }
                });
            }
            else
            {
                //==== #001
                var mPatient = SelectedPatient;
                if (SearchCriteria.QRCode != null)
                {
                    mPatient.QRCode = SearchCriteria.QRCode;
                }
                //==== #001
                if (IsPresenter && Globals.ServerConfigSection.OutRegisElements.IsPerformingTMVFunctionsA) //20200225 TBL: Nếu popup được mở từ người giới thiệu thì bắn sự kiện này
                {
                    Globals.EventAggregator.Publish(new SelectPresenter() { PatientInfo = mPatient });
                }
                else
                {
                    Globals.EventAggregator.Publish(new ItemSelected<Patient>() { Item = mPatient });
                }
                if (!IsAddingMultiples)
                {
                    TryClose();
                }
            }
        }

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

        public void SelectPatientAndClose(object context)
        {
            SelectedPatient = context as Patient;

            if (SelectedPatient != null)
            {
                Globals.EventAggregator.Publish(new ItemSelected<Patient>() { Item = SelectedPatient });
            }

            TryClose();
        }
        public void KeyUpCmd(KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (!IsLoading)
                {
                    if (e.Key == Key.Right)
                    {
                        //Go To Next Page.
                        Patients.MoveToNextPage();
                        e.Handled = true;
                    }
                    else if (e.Key == Key.Left)
                    {
                        //Back to Prev Page.
                        Patients.MoveToPreviousPage();
                        e.Handled = true;
                    }
                }
            }
        }

        /// <summary>
        /// Copy danh sách bệnh nhân vào Patient list. Khỏi mắc công search lại.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="total"></param>
        public void CopyExistingPatientList(IList<Patient> items, PatientSearchCriteria criteria, int total)
        {
            _criteria = criteria.DeepCopy();
            _patients.Clear();
            if (items != null && items.Count > 0)
            {
                foreach (Patient p in items)
                {
                    _patients.Add(p);
                }
                _patients.TotalItemCount = total;
            }
            NotifyOfPropertyChange(() => Patients);
        }

        public void LoadGenders()
        {
            var t = new Thread(() =>
            {
                /* Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message =
                        "Đang load dữ liệu..."
                }); */
                this.DlgShowBusyIndicator("Đang load dữ liệu...");
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllGenders(
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var allItems = contract.EndGetAllGenders(asyncResult);

                                    if (allItems != null)
                                    {
                                        Genders = new ObservableCollection<Gender>(allItems);
                                    }
                                    else
                                    {
                                        Genders = null;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                    ClientLoggerHelper.LogInfo(ex.ToString());
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }

        public void Handle(AddCompleted<Patient> message)
        {
            if (message != null && message.Item != null)
            {
                if (this.IsActive && !IsAddingMultiples)
                {
                    SearchPatients(Patients.PageIndex, Patients.PageSize, true);
                }
            }
        }

        /*▼====: #002*/
        ReadOnlyDataGrid gGvPatients;

        public void gridPatient_Loaded(object sender, RoutedEventArgs e)
        {
            gGvPatients = sender as ReadOnlyDataGrid;
        }
        /*▲====: #002*/

        public bool IsAddingMultiples { get; set; }
        private bool _IsPresenter;
        public bool IsPresenter
        {
            get { return _IsPresenter; }
            set
            {
                _IsPresenter = value;
                NotifyOfPropertyChange(() => IsPresenter);
            }
        }

        //▼===== #005
        #region Tìm kiếm bằng tỉnh thành
        private SuburbNames _SelectedSuburbName;
        public SuburbNames SelectedSuburbName
        {
            get { return _SelectedSuburbName; }
            set
            {
                _SelectedSuburbName = value;
                NotifyOfPropertyChange(() => SelectedSuburbName);
            }
        }
        private ObservableCollection<SuburbNames> _SuburbNames;
        public ObservableCollection<SuburbNames> SuburbNames
        {
            get { return _SuburbNames; }
            set
            {
                _SuburbNames = value;
                NotifyOfPropertyChange(() => SuburbNames);
            }
        }
        private ObservableCollection<CitiesProvince> _allProvince;
        public ObservableCollection<CitiesProvince> allProvince
        {
            get { return _allProvince; }
            set
            {
                _allProvince = value;
                NotifyOfPropertyChange(() => allProvince);
            }
        }
        private ObservableCollection<CitiesProvince> _provinces;
        public ObservableCollection<CitiesProvince> Provinces
        {
            get { return _provinces; }
            set
            {
                _provinces = value;
                NotifyOfPropertyChange(() => Provinces);
            }
        }
        private CitiesProvince _selectedProvince;
        public CitiesProvince SelectedProvince
        {
            get { return _selectedProvince; }
            set
            {
                _selectedProvince = value;
                NotifyOfPropertyChange(() => SelectedProvince);
            }
        }

        AxAutoComplete AcbCity { get; set; }
        AxComboBox cboSuburb { get; set; }
        public void AcbCity_Loaded(object sender, RoutedEventArgs e)
        {
            AcbCity = sender as AxAutoComplete;
        }
        public void cboSuburb_Loaded(object sender, RoutedEventArgs e)
        {
            cboSuburb = sender as AxComboBox;
        }
        public void cboSuburb_SelectionChanged(object sender, RoutedEventArgs e)
        {
            cboSuburb = sender as AxComboBox;
            if (cboSuburb == null)
            {
                return;
            }
            SelectedSuburbName = cboSuburb.SelectedItemEx as SuburbNames;
        }
        public string ConvertString(string stringInput)
        {
            stringInput = stringInput.ToUpper();
            string convert = "ĂÂÀẰẦÁẮẤẢẲẨÃẴẪẠẶẬỄẼỂẺÉÊÈỀẾẸỆÔÒỒƠỜÓỐỚỎỔỞÕỖỠỌỘỢƯÚÙỨỪỦỬŨỮỤỰÌÍỈĨỊỲÝỶỸỴĐăâàằầáắấảẳẩãẵẫạặậễẽểẻéêèềếẹệôòồơờóốớỏổởõỗỡọộợưúùứừủửũữụựìíỉĩịỳýỷỹỵđ";
            string To = "AAAAAAAAAAAAAAAAAEEEEEEEEEEEOOOOOOOOOOOOOOOOOUUUUUUUUUUUIIIIIYYYYYDaaaaaaaaaaaaaaaaaeeeeeeeeeeeooooooooooooooooouuuuuuuuuuuiiiiiyyyyyd";
            for (int i = 0; i < To.Length; i++)
            {
                stringInput = stringInput.Replace(convert[i], To[i]);
            }
            return stringInput;
        }
        public void AcbCity_Populating(object sender, PopulatingEventArgs e)
        {
            if (sender == null || Provinces == null)
            {
                return;
            }

            string SearchText = ((AutoCompleteBox)sender).SearchText;
            if (SearchText.Length == 0)
            {
                ((AutoCompleteBox)sender).PopulateComplete();
                PagingLinq(0);
                return;
            }

            allProvince = new ObservableCollection<CitiesProvince>(Provinces.Where(item => ConvertString(item.CityProvinceName)
                    .IndexOf(ConvertString(SearchText), StringComparison.InvariantCultureIgnoreCase) >= 0));
            ((AutoCompleteBox)sender).ItemsSource = allProvince;
            ((AutoCompleteBox)sender).PopulateComplete();
        }
        public void AcbCity_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (sender == null)
            {
                return;
            }
            if (((AutoCompleteBox)sender).SelectedItem == null)
            {
                return;
            }
            long CityProvinceID = ((CitiesProvince)(((AutoCompleteBox)sender).SelectedItem)).CityProvinceID;
            if (CityProvinceID > 0)
            {
                PagingLinq(CityProvinceID);
            }
            else
            {
                SuburbNames = new ObservableCollection<SuburbNames>();
                NotifyOfPropertyChange(() => SuburbNames);
            }
        }
        public void LoadProvinces()
        {
            if (Globals.allCitiesProvince != null && Globals.allCitiesProvince.Count > 0)
            {
                Provinces = new ObservableCollection<CitiesProvince>(Globals.allCitiesProvince);
                return;
            }

            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllProvinces(Globals.DispatchCallback(asyncResult =>
                        {
                            IList<CitiesProvince> allItems = null;
                            try
                            {
                                allItems = contract.EndGetAllProvinces(asyncResult);
                                if (Globals.allCitiesProvince == null)
                                {
                                    Globals.allCitiesProvince = new List<CitiesProvince>(allItems);
                                }
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                MessageBox.Show(eHCMSResources.A0693_G1_Msg_InfoKhTheLayDSTinhThanh);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }

                            Provinces = allItems != null ? new ObservableCollection<CitiesProvince>(allItems) : null;
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

        public void PagingLinq(long CityProvinceID)
        {
            SuburbNames = new ObservableCollection<SuburbNames>();
            SuburbNames firstItem = new SuburbNames();
            firstItem.SuburbNameID = 0;
            firstItem.SuburbName = "-- Tất cả --";
            SuburbNames.Add(firstItem);
            foreach (var item in Globals.allSuburbNames)
            {
                if (item.CityProvinceID == CityProvinceID)
                {
                    SuburbNames.Add(item);
                }
            }
        }
        private void GetAllSuburbName()
        {
            if (Globals.allSuburbNames != null && Globals.allSuburbNames.Count > 0)
            {
                return;
            }

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {

                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllSuburbNames(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var allItems = contract.EndGetAllSuburbNames(asyncResult);
                                if (allItems != null)
                                {
                                    Globals.allSuburbNames = allItems.ToList();
                                }
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogError(ex.ToString());
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {

                            }

                        }), null);

                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogError(ex.ToString());
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        #endregion
        #region lọc theo ngày tháng năm sinh.
        public const string RegExOnlyNumber = "^[0-9]*$";
        public string DateOfBirthBegin
        {
            get { return _DateOfBirthBegin; }
            set
            {
                _DateOfBirthBegin = value;
                NotifyOfPropertyChange(() => DateOfBirthBegin);
            }
        }
        private string _DateOfBirthBegin;

        public string DateOfBirthEnd
        {
            get { return _DateOfBirthEnd; }
            set
            {
                _DateOfBirthEnd = value;
                NotifyOfPropertyChange(() => DateOfBirthEnd);
            }
        }
        private string _DateOfBirthEnd;
        TextBox txtBirthDateBegin;
        TextBox txtBirthDateEnd;
        public void txtBirthDateBegin_Loaded(object sender, RoutedEventArgs e)
        {
            txtBirthDateBegin = sender as TextBox;
        }
        public void txtBirthDateEnd_Loaded(object sender, RoutedEventArgs e)
        {
            txtBirthDateEnd = sender as TextBox;
        }
        public void txtBirthDateBegin_Lostfocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBirthDateBegin.Text))
            {
                Regex regBirthDateBegin = new Regex(RegExOnlyNumber, RegexOptions.IgnoreCase);
                txtBirthDateBegin = sender as TextBox;
                if (!regBirthDateBegin.Match(txtBirthDateBegin.Text).Success)
                {
                    MessageBox.Show(eHCMSResources.Z3056_G1_ChiNhapSo);
                    txtBirthDateBegin.Text = "";
                    DateOfBirthBegin = "";
                    return;
                }
                if (Convert.ToInt32(DateOfBirthBegin) < 1900 || Convert.ToInt32(DateOfBirthBegin) > Globals.GetCurServerDateTime().Year)
                {
                    MessageBox.Show(string.Format(eHCMSResources.Z3057_G1_NamTimKiemTrongKhoang, Globals.GetCurServerDateTime().Year));
                    txtBirthDateBegin.Text = "";
                    DateOfBirthBegin = "";
                    return;
                }
            }
        }
        public void txtBirthDateEnd_Lostfocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBirthDateEnd.Text))
            {
                Regex regBirthDateBegin = new Regex(RegExOnlyNumber, RegexOptions.IgnoreCase);
                txtBirthDateEnd = sender as TextBox;
                if (!regBirthDateBegin.Match(txtBirthDateEnd.Text).Success)
                {
                    MessageBox.Show(eHCMSResources.Z3056_G1_ChiNhapSo);
                    txtBirthDateEnd.Text = "";
                    DateOfBirthEnd = "";
                    return;
                }
                if (Convert.ToInt32(DateOfBirthEnd) < 1900 || Convert.ToInt32(DateOfBirthEnd) > Globals.GetCurServerDateTime().Year)
                {
                    MessageBox.Show(string.Format(eHCMSResources.Z3057_G1_NamTimKiemTrongKhoang, Globals.GetCurServerDateTime().Year));
                    txtBirthDateEnd.Text = "";
                    DateOfBirthEnd = "";
                    return;
                }
            }
        }
        #endregion
        //▲===== #005
    }
}