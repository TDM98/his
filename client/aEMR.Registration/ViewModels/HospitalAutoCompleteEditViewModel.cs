using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows.Controls;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.CommonTasks;
using System.Windows;
using System.Linq;
using aEMR.Common;
using aEMR.DataContracts;
using aEMR.Common.Utilities;
using Castle.Windsor;

namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IHospitalAutoCompleteEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class HospitalAutoCompleteEditViewModel : Conductor<object>, IHospitalAutoCompleteEdit
    {
        [ImportingConstructor]
        public HospitalAutoCompleteEditViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            HospitalList = new PagedSortableCollectionView<Hospital>();
            HospitalList.OnRefresh += DrugList_OnRefresh;
            SelectedHospital = new Hospital();
            _searchCriteria = string.Empty;
            LoadProvinces();
            Coroutine.BeginExecute(LoadHospitalType());
            Coroutine.BeginExecute(LoadHospitalClass());
            CanAddNew = true;
            CanUpdate = true;
        }

        private void DrugList_OnRefresh(object sender, Common.Collections.RefreshEventArgs e)
        {
            LoadHospitalsNew(HospitalList.PageIndex, HospitalList.PageSize, false);
        }

        public string DisplayText
        {
            get
            {
                var currentView = this.GetView() as IAutoCompleteView;
                if (currentView != null && currentView.AutoCompleteBox != null)
                {
                    return currentView.AutoCompleteBox.SearchText;
                }
                return string.Empty;
            }
            set
            {
                var currentView = this.GetView() as IAutoCompleteView;
                if (currentView != null && currentView.AutoCompleteBox != null)
                {
                    currentView.AutoCompleteBox.Text = value;
                }
            }
        }

        private string _criteria;
        private string _searchCriteria;
        public string SearchCriteria
        {
            get { return _searchCriteria; }
            private set
            {
                if (_searchCriteria != value)
                {
                    _searchCriteria = value;
                    NotifyOfPropertyChange(() => SearchCriteria);
                }
            }
        }

        private Hospital _selectedHospital;
        public Hospital SelectedHospital
        {
            get { return _selectedHospital; }
            set
            {
                _selectedHospital = value;
                NotifyOfPropertyChange(() => SelectedHospital);
            }
        }

        private bool _CanAddNew;
        public bool CanAddNew
        {
            get { return _CanAddNew; }
            private set
            {
                if (_CanAddNew != value)
                {
                    _CanAddNew = value;
                    NotifyOfPropertyChange(() => CanAddNew);
                }
            }
        }

        private bool _CanUpdate;
        public bool CanUpdate
        {
            get { return _CanUpdate; }
            private set
            {
                if (_CanUpdate != value)
                {
                    _CanUpdate = value;
                    NotifyOfPropertyChange(() => CanUpdate);
                }
            }
        }

        private string _Title;
        public string Title
        {
            get { return _Title; }
            set
            {
                _Title = value;
                NotifyOfPropertyChange(() => Title);
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

        private ObservableCollection<Lookup> _HospitalType;
        public ObservableCollection<Lookup> HospitalType
        {
            get { return _HospitalType; }
            set
            {
                _HospitalType = value;
                NotifyOfPropertyChange(() => HospitalType);
            }
        }

        private IEnumerator<IResult> LoadHospitalType()
        {
            var paymentModeTask = new LoadLookupListTask(LookupValues.HOSPITAL_TYPE);
            yield return paymentModeTask;
            HospitalType = paymentModeTask.LookupList;
            SelectedHospital.V_HospitalType = -1;
            yield break;
        }

        private ObservableCollection<Lookup> _HospitalClass;
        public ObservableCollection<Lookup> HospitalClass
        {
            get { return _HospitalClass; }
            set
            {
                _HospitalClass = value;
                NotifyOfPropertyChange(() => HospitalClass);
            }
        }

        private IEnumerator<IResult> LoadHospitalClass()
        {
            var paymentModeTask = new LoadLookupListTask(LookupValues.V_HospitalClass);
            yield return paymentModeTask;
            HospitalClass = paymentModeTask.LookupList;
            SelectedHospital.V_HospitalClass = -1;
            yield break;
        }

        private bool _displayHiCode = true;
        public bool DisplayHiCode
        {
            get { return _displayHiCode; }
            set
            {
                _displayHiCode = value;
                NotifyOfPropertyChange(() => DisplayHiCode);
            }
        }

        private bool _IsUpdate;
        public bool IsUpdate
        {
            get { return _IsUpdate; }
            set
            {
                _IsUpdate = value;
                NotifyOfPropertyChange(() => IsUpdate);
            }
        }

        private bool _IsChildWindown;
        public bool IsChildWindown
        {
            get { return _IsChildWindown; }
            set
            {
                _IsChildWindown = value;
                NotifyOfPropertyChange(() => IsChildWindown);
            }
        }

        private bool _IsPaperReferal;
        public bool IsPaperReferal
        {
            get { return _IsPaperReferal; }
            set
            {
                _IsPaperReferal = value;
                NotifyOfPropertyChange(() => IsPaperReferal);
                if (SelectedHospital != null)
                {
                    SelectedHospital.UsedForPaperReferralOnly = true;
                }
            }
        }

        private PagedSortableCollectionView<Hospital> _hospitalList;
        public PagedSortableCollectionView<Hospital> HospitalList
        {
            get { return _hospitalList; }
            private set
            {
                _hospitalList = value;
                NotifyOfPropertyChange(() => HospitalList);
            }
        }

        private void LoadHospitals(int pageIndex, int pageSize, bool bCountTotal)
        {
            if (IsChildWindown)
            {
                this.DlgShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            }
            else
            {
                this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            }
            var t = new Thread(() =>
            {
                if (_criteria == null)
                {
                    _criteria = _searchCriteria;
                }
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginSearchHospitals(_criteria, pageIndex, pageSize, bCountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int totalCount = 0;
                            IList<Hospital> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndSearchHospitals(out totalCount, asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                error = new AxErrorEventArgs(fault);
                            }
                            catch (Exception ex)
                            {
                                error = new AxErrorEventArgs(ex);
                            }
                            finally
                            {
                                if (IsChildWindown)
                                {
                                    this.DlgHideBusyIndicator();
                                }
                                else
                                {
                                    this.HideBusyIndicator();
                                }
                            }

                            HospitalList.Clear();
                            if (bOK)
                            {
                                if (bCountTotal)
                                {
                                    HospitalList.TotalItemCount = totalCount;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        HospitalList.Add(item);
                                    }
                                }
                                var currentView = this.GetView() as IAutoCompleteView;
                                if (currentView != null)
                                {
                                    currentView.PopulateComplete();
                                }
                            }
                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                    if (IsChildWindown)
                    {
                        this.DlgHideBusyIndicator();
                    }
                    else
                    {
                        this.HideBusyIndicator();
                    }
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                }
            });

            t.Start();
        }

        private void LoadHospitalsNew(int pageIndex, int pageSize, bool bCountTotal)
        {
            if (IsChildWindown)
            {
                this.DlgShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            }
            else
            {
                this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            }
            HospitalSearchCriteria hosSearchCri = new HospitalSearchCriteria();
            var t = new Thread(() =>
            {
                if (_criteria == null)
                {
                    _criteria = _searchCriteria;
                }
                hosSearchCri.HosName = _searchCriteria;
                hosSearchCri.IsPaperReferal = IsPaperReferal;
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginSearchHospitalsNew(hosSearchCri, pageIndex, pageSize, bCountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int totalCount = 0;
                            IList<Hospital> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndSearchHospitalsNew(out totalCount, asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                error = new AxErrorEventArgs(fault);
                            }
                            catch (Exception ex)
                            {
                                error = new AxErrorEventArgs(ex);
                            }
                            finally
                            {
                                if (IsChildWindown)
                                {
                                    this.DlgHideBusyIndicator();
                                }
                                else
                                {
                                    this.HideBusyIndicator();
                                }
                            }

                            HospitalList.Clear();
                            if (bOK)
                            {
                                if (bCountTotal)
                                {
                                    HospitalList.TotalItemCount = totalCount;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        HospitalList.Add(item);
                                    }
                                }
                                var currentView = this.GetView() as IAutoCompleteView;
                                if (currentView != null)
                                {
                                    currentView.PopulateComplete();
                                }
                            }
                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                    if (IsChildWindown)
                    {
                        this.DlgHideBusyIndicator();
                    }
                    else
                    {
                        this.HideBusyIndicator();
                    }
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                }
            });

            t.Start();
        }

        public void HospitalCode_LostFocus(TextBox sender, EventArgs eventArgs)
        {
            if (IsUpdate
                && !string.IsNullOrEmpty(sender.Text))
            {
                LoadHospital(sender.Text, sender);
            }
            else
            {
                LoadHospital(sender.Text, sender, true);
            }
        }

        private void LoadHospital(string HospitalCode, TextBox sender, bool isNew = false)
        {
            if (IsChildWindown)
            {
                this.DlgShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            }
            else
            {
                this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            }
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSearchHospitalByHICode(HospitalCode,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    var item = contract.EndSearchHospitalByHICode(asyncResult);
                                    if (!isNew)
                                    {
                                        SelectedHospital = item;
                                    }
                                    else
                                    {
                                        if (item != null && item.HosID > 0)
                                        {
                                            MessageBox.Show(eHCMSResources.A0811_G1_Msg_InfoMaKCBDaTonTai);
                                            sender.Text = "";
                                            sender.Focus();
                                        }
                                    }
                                }
                                catch (Exception innerEx)
                                {
                                    ClientLoggerHelper.LogInfo(innerEx.ToString());
                                }
                                finally
                                {
                                    if (IsChildWindown)
                                    {
                                        this.DlgHideBusyIndicator();
                                    }
                                    else
                                    {
                                        this.HideBusyIndicator();
                                    }
                                }
                            }), null);
                    }
                }
                catch (FaultException<AxException> fault)
                {
                    ClientLoggerHelper.LogInfo(fault.ToString());
                    if (IsChildWindown)
                    {
                        this.DlgHideBusyIndicator();
                    }
                    else
                    {
                        this.HideBusyIndicator();
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    if (IsChildWindown)
                    {
                        this.DlgHideBusyIndicator();
                    }
                    else
                    {
                        this.HideBusyIndicator();
                    }
                }
            });

            t.Start();
        }

        public void LoadProvinces()
        {
            if (Globals.allCitiesProvince != null)
            {
                Provinces = Globals.allCitiesProvince.ToObservableCollection();
                return;
            }

            if (IsChildWindown)
            {
                this.DlgShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            }
            else
            {
                this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            }
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

                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                MessageBox.Show(eHCMSResources.A0693_G1_Msg_InfoKhTheLayDSTinhThanh);
                            }
                            finally
                            {
                                if (IsChildWindown)
                                {
                                    this.DlgHideBusyIndicator();
                                }
                                else
                                {
                                    this.HideBusyIndicator();
                                }
                            }

                            Provinces = allItems != null ? new ObservableCollection<CitiesProvince>(allItems) : null;
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    if (IsChildWindown)
                    {
                        this.DlgHideBusyIndicator();
                    }
                    else
                    {
                        this.HideBusyIndicator();
                    }
                }
            });

            t.Start();
        }

        public void StartSearching()
        {
            //_criteria = _searchCriteria.DeepCopy();

            HospitalList.PageIndex = 0;
            LoadHospitalsNew(HospitalList.PageIndex, HospitalList.PageSize, true);
        }

        public void ClearItems()
        {
            HospitalList.Clear();
            HospitalList.TotalItemCount = 0;
        }

        public void PopulatingCmd(object source, PopulatingEventArgs eventArgs)
        {
            //eventArgs.Cancel = true;
            //var currentView = this.GetView() as IAutoCompleteView;
            //if (currentView != null)
            //{
            //    if (SearchCriteria != currentView.AutoCompleteBox.SearchText)
            //    {
            //        SearchCriteria = currentView.AutoCompleteBox.SearchText;
            //        StartSearching();
            //    }
            //}
            if (!IsUpdate)
            {
                return;
            }
            if (SearchCriteria != ((AutoCompleteBox)source).SearchText)
            {
                SearchCriteria = ((AutoCompleteBox)source).SearchText;
                StartSearching();
            }
        }

        public void auc1_DropDownClosed(object sender, System.Windows.RoutedPropertyChangedEventArgs<bool> e)
        {

        }

        public void AcbCity_Populating(object sender, PopulatingEventArgs e)
        {
            if (sender != null && Provinces != null)
            {
                string SearchText = ((AutoCompleteBox)sender).SearchText;

                //16072018 TTM Viet lai Linq va dat them dieu kien neu searchtext != null thi moi lam linq
                if (SearchText != null)
                {
                    allProvince = new ObservableCollection<CitiesProvince>();
                    allProvince.Clear();
                    foreach (var item in Provinces)
                    {
                        string tmp = VNConvertString.ConvertString(item.CityProvinceName);
                        if (tmp.IndexOf(SearchText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        {
                            allProvince.Add(item);
                        }
                    }
                ((AutoCompleteBox)sender).ItemsSource = allProvince;
                    ((AutoCompleteBox)sender).PopulateComplete();
                }
                //allProvince = new ObservableCollection<CitiesProvince>(Provinces.Where(item => VNConvertString.ConvertString(item.CityProvinceName)
                //     .IndexOf(SearchText, StringComparison.InvariantCultureIgnoreCase) >= 0));
                //((AutoCompleteBox)sender).ItemsSource = allProvince;
                //((AutoCompleteBox)sender).PopulateComplete();
            }
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

        public void AcbCity_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (sender == null)
            {
                SelectedHospital.CityProvinceID = 0;
                return;
            }
            if (((AutoCompleteBox)sender).SelectedItem == null)
            {
                SelectedHospital.CityProvinceID = 0;
                return;
            }
            long CityProvinceID = ((CitiesProvince)(((AutoCompleteBox)sender).SelectedItem)).CityProvinceID;
            string CityProvinceHICode = ((CitiesProvince)(((AutoCompleteBox)sender).SelectedItem)).CityProviceHICode;
            if (CityProvinceID > 0)
            {
                SelectedHospital.CityProvinceID = CityProvinceID;
                if (CityProvinceHICode.Length >= 2)
                {
                    SelectedHospital.CityProvinceHICode = CityProvinceHICode.Substring(0, 2);
                }
            }
        }

        public bool CheckValidate()
        {
            if (string.IsNullOrEmpty(SelectedHospital.HosName))
            {
                MessageBox.Show(eHCMSResources.A1004_G1_Msg_InfoTenBVTrong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            //if (!IsPaperReferal && string.IsNullOrEmpty(SelectedHospital.HICode))
            if (string.IsNullOrEmpty(SelectedHospital.HICode))
            {
                MessageBox.Show(eHCMSResources.A0808_G1_Msg_InfoMaBVTrong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            if (SelectedHospital.CityProvinceID == null
                || SelectedHospital.CityProvinceID < 1 || SelectedHospital.CityProvinceHICode.Length < 2)
            {
                MessageBox.Show(eHCMSResources.A0402_G1_Msg_InfoChuaChonTinhThanh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            //if (!IsPaperReferal && (SelectedHospital.HICode.Length > 5 || SelectedHospital.HICode.Length < 5))
            if (SelectedHospital.HICode.Length > 5 || SelectedHospital.HICode.Length < 5)
            {
                MessageBox.Show(eHCMSResources.A0809_G1_Msg_InfoMaCode5KyTu, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            if (SelectedHospital.HICode.Substring(0, 2) != SelectedHospital.CityProvinceHICode.Substring(0, 2))
            {
                MessageBox.Show(eHCMSResources.Z0436_G1_MaCodeKgKhopTinhThanh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            return true;
        }

        public void CancelCmd()
        {
            TryClose();
        }

        public void UpdateCmd()
        {
            ////this.ShowBusyIndicator();
            Coroutine.BeginExecute(Update());
            ////this.HideBusyIndicator();
        }

        public IEnumerator<IResult> Update()
        {
            //this.ShowBusyIndicator();
            CanUpdate = false;
            if (!CheckValidate())
            {
                //this.HideBusyIndicator();
                CanUpdate = true;
                yield break;
            }
            if (SelectedHospital.V_HospitalType < 1)
            {
                var dialog = new MessageWarningShowDialogTask(eHCMSResources.Z0143_G1_ChuaChonLoaiBV, eHCMSResources.Z0217_G1_TiepTucLuuTTin);
                yield return dialog;
                if (dialog.IsAccept)
                {
                    //tiep tuc lam cong chuyen nay      
                    UpdateHospital();
                }
                CanUpdate = true;
                yield break;
            }
            UpdateHospital();
            //this.HideBusyIndicator();
        }

        public void UpdateHospital()
        {
            if (IsChildWindown)
            {
                this.DlgShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            }
            else
            {
                this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            }
            CanUpdate = true;
            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        if (SelectedHospital.HosID > 0)
                        {
                            SelectedHospital.DateModified = Convert.ToDateTime(Globals.ServerDate);
                            SelectedHospital.ModifiedLog = Globals.LoggedUserAccount.StaffID.ToString();
                        }
                        client.BeginUpdateHospital(SelectedHospital, Globals.DispatchCallback((asyncResult) =>
                        {
                            try

                            {
                                var bOK = client.EndUpdateHospital(asyncResult);
                                if (bOK)
                                {
                                    Globals.EventAggregator.Publish(new Hospital_Event_Save() { Result = true });
                                    MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK);
                                    TryClose();
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail);
                                }
                            }
                            catch (FaultException<AxException> fault)
                            {
                                error = new AxErrorEventArgs(fault);
                            }
                            catch (Exception ex)
                            {
                                error = new AxErrorEventArgs(ex);
                            }
                            finally
                            {
                                if (IsChildWindown)
                                {
                                    this.DlgHideBusyIndicator();
                                }
                                else
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
                    if (IsChildWindown)
                    {
                        this.DlgHideBusyIndicator();
                    }
                    else
                    {
                        this.HideBusyIndicator();
                    }
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                }
            });

            t.Start();
        }

        public void AddNewCmd()
        {
            ////this.ShowBusyIndicator();
            Coroutine.BeginExecute(AddNew());
            ////this.HideBusyIndicator();
        }

        public void AddHospital()
        {
            if (IsChildWindown)
            {
                this.DlgShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            }
            else
            {
                this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            }
            CanAddNew = false;
            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;

                        client.BeginAddHospital(SelectedHospital, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool bOK = client.EndAddHospital(asyncResult);
                                if (bOK)
                                {
                                    Globals.EventAggregator.Publish(new InsuranceBenefitCategories_Event_Save() { Result = true });
                                    MessageBox.Show(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK);
                                    TryClose();
                                }
                                else
                                {
                                    MessageBox.Show(string.Format("{0}. {1}", eHCMSResources.A1026_G1_Msg_InfoThemFail, eHCMSResources.A1028_G1_Msg_YCKtraTTin));
                                }
                            }
                            catch (FaultException<AxException> fault)
                            {
                                error = new AxErrorEventArgs(fault);
                                MessageBox.Show(error.ToString());
                            }
                            catch (Exception ex)
                            {
                                error = new AxErrorEventArgs(ex);
                                MessageBox.Show(error.ToString());
                            }
                            finally
                            {
                                if (IsChildWindown)
                                {
                                    this.DlgHideBusyIndicator();
                                }
                                else
                                {
                                    this.HideBusyIndicator();
                                }
                            }
                        }), null);
                    }
                }
                catch (FaultException<AxException> fault)
                {
                    error = new AxErrorEventArgs(fault);
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                    if (IsChildWindown)
                    {
                        this.DlgHideBusyIndicator();
                    }
                    else
                    {
                        this.HideBusyIndicator();
                    }
                }
            });
            t.Start();
        }

        public IEnumerator<IResult> AddNew()
        {
            //this.ShowBusyIndicator();
            CanAddNew = false;
            if (!CheckValidate())
            {
                CanAddNew = true;
                //this.HideBusyIndicator();
                yield break;
            }

            if (!IsPaperReferal && SelectedHospital.V_HospitalType < 1)
            {
                var dialog = new MessageWarningShowDialogTask(eHCMSResources.Z0143_G1_ChuaChonLoaiBV, eHCMSResources.Z0217_G1_TiepTucLuuTTin);
                yield return dialog;
                if (dialog.IsAccept)
                {
                    //tiep tuc lam cong chuyen nay      
                    AddHospital();
                }
                CanAddNew = true;
                //this.HideBusyIndicator();
                yield break;
            }

            AddHospital();
            //this.HideBusyIndicator();
        }
    }
}
