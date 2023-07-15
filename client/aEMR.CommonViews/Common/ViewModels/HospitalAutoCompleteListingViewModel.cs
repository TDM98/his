using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows.Controls;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Common.Converters;
using System.Windows;
using aEMR.Controls;
using eHCMSLanguage;

namespace aEMR.Common.ViewModels
{
    [Export(typeof (IHospitalAutoCompleteListing)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class HospitalAutoCompleteListingViewModel : Conductor<object>, IHospitalAutoCompleteListing
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public HospitalAutoCompleteListingViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            HospitalList = new PagedSortableCollectionView<Hospital>();
            HospitalList.OnRefresh += new EventHandler<aEMR.Common.Collections.RefreshEventArgs>(DrugList_OnRefresh);

            _searchCriteria = string.Empty;
        }

        void DrugList_OnRefresh(object sender, aEMR.Common.Collections.RefreshEventArgs e)
        {
            LoadHospitalsNew(HospitalList.PageIndex, HospitalList.PageSize, false);
        }

        private string _DisplayText;
        public string AcDisplayText
        {
            get
            {
                return _DisplayText;
            }
            set
            {
                _DisplayText = value;
                NotifyOfPropertyChange(() => AcDisplayText);
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

        private int _Count;
        public int Count
        {
            get { return _Count; }
            set
            {
                if (_Count != value)
                {
                    _Count = value;
                    NotifyOfPropertyChange(() => Count);
                }
            }
        }

        private long _hosID = 0;
        public long HosID
        {
            get { return _hosID; }
            set
            {
                _hosID = value;
                NotifyOfPropertyChange(() => HosID);
            }
        }


        private Hospital _selectedHospital;
        public Hospital SelectedHospital
        {
            get { return _selectedHospital; }
            set
            {
                if (_selectedHospital != value)
                {
                    if (value == null && bSelectedHospitalInit)
                    {
                        // TxD 27/03/2014 Work around for SetSelHospital that caused a Null value from Binding System (don't know why)
                        //                  It happen when the AutoCompleteBox has focus and Enter key without changing anything in the Box.
                        bSelectedHospitalInit = false;
                    }
                    else
                    {
                        if (value != null && string.IsNullOrEmpty(value.HICode))
                        {
                            value.IsNoneHICodeOrig = true;
                        }
                        _selectedHospital = value;
                        NotifyOfPropertyChange(() => SelectedHospital);
                        NotifyOfPropertyChange(() => SelectedHospital.HosName);
                        NotifyOfPropertyChange(() => SelectedHospital.HICode);
                        NotifyOfPropertyChange(() => CanEditHiCode);
                        HICode_Orig = SelectedHospital != null && SelectedHospital.HICode != null && SelectedHospital.HICode.Trim().Length >= 5 ? SelectedHospital.HICode : "";
                        if (Parent != null && Parent is IPaperReferalFull && Parent.CurrentTransferForm != null && _selectedHospital != null)
                        {
                            if (Parent.CurrentTransferForm.TransferFromHos == null)
                            {
                                Parent.CurrentTransferForm.TransferFromHos = new Hospital();
                            }
                            Parent.CurrentTransferForm.TransferFromHos = _selectedHospital;                                
                        }
                    }
                }
            }
        }

        public AxAutoComplete HosAutoComplete { get; set; }
        
        private bool _displayHiCode = true;
        public bool DisplayHiCode
        {
            get { return _displayHiCode; }
            set
            {
                _displayHiCode = value;
                NotifyOfPropertyChange(()=> DisplayHiCode);
            }
        }

        private bool _IsPaperReferal ;
        public bool IsPaperReferal
        {
            get { return _IsPaperReferal; }
            set
            {
                _IsPaperReferal = value;
                NotifyOfPropertyChange(() => IsPaperReferal);
                NotifyOfPropertyChange(() => CanEditHiCode);
            }
        }

        private bool _IsSearchAll = false;
        public bool IsSearchAll
        {
            get { return _IsSearchAll; }
            set
            {
                if (_IsSearchAll != value)
                {
                    _IsSearchAll = value;
                }
                NotifyOfPropertyChange(() => IsSearchAll);
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

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (!_isLoading)
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        public void InitBlankBindingValue()
        {
            SelectedHospital = new Hospital();
        }

        private bool bSelectedHospitalInit = false;
        public void SetSelHospital(Hospital selectedHospital)
        {
            if (selectedHospital == null || selectedHospital.HosName != null && selectedHospital.HosName.Length < 1)
            {
                return;
            }
            
            bSelectedHospitalInit = true;
            SelectedHospital = selectedHospital;
            HosID = selectedHospital.HosID;
        }

        public void ResetSelectedHospitalInit()
        {
            bSelectedHospitalInit = false;
        }

        private HealthInsurance _curSelHealthInsuranceCard = null;
        public HealthInsurance CurSelHealthInsuranceCard 
        {
            get { return _curSelHealthInsuranceCard; }
            set
            {
                _curSelHealthInsuranceCard = value;
            }
        }

        //public void Reset() 
        //{
        //    //PreventPopulate = false;
        //    //hasPopulate = false;
        //}

        //public void ResetPrevent()
        //{
        //    //PreventPopulate = true;
        //    //hasPopulate = false;
        //}


        private void LoadHospitalsNew(int pageIndex, int pageSize, bool bCountTotal)
        {
            // TxD 28/03/2014: Added this to STOP searching for ALL Hospitals which is NOT practical
            if (_searchCriteria.Length == 0)
            {
                return;
            }
            // TxD 28/03/2014: PLEASE NOTE THAT YOU SHOULD NOT put a BUSY INDICATOR in here because 
            //                  IT WILL STOP the Autocompletebox from DROPPING DOWN the selection list.

            HospitalSearchCriteria hosSearchCri=new HospitalSearchCriteria();            
            var t = new Thread(() =>
            {
                if (_criteria == null)
                {
                    _criteria = _searchCriteria;
                }
                hosSearchCri.HosName = _searchCriteria;
                hosSearchCri.IsPaperReferal = IsPaperReferal;
                hosSearchCri.IsSearchAll = IsSearchAll;
                
                IsLoading = true;
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
                            
                            try
                            {
                                allItems = client.EndSearchHospitalsNew(out totalCount, asyncResult);

                                HospitalList.Clear();

                                if (bCountTotal)
                                {
                                    HospitalList.TotalItemCount = totalCount;
                                }

                                if (allItems != null && allItems.Count > 0)
                                {
                                    foreach (var item in allItems)
                                    {
                                        HospitalList.Add(item);
                                    }
                                }
                                else
                                {
                                    if (_curSelHealthInsuranceCard != null)
                                    {
                                        _curSelHealthInsuranceCard.HosID = 0;
                                        _curSelHealthInsuranceCard.RegistrationCode = "";
                                        _curSelHealthInsuranceCard.RegistrationLocation = "";
                                        //KMx: Tỉnh thành nơi cư trú của BN dựa vào ký tự thứ 4 và 5 trên mã thẻ BH, không phải dựa vào mã KCB-BĐ (11/11/2014 11:33).
                                        //_curSelHealthInsuranceCard.CityProvinceName = "";
                                    }
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
                                HosAutoComplete.PopulateComplete();
                            }                           

                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                    //this.HideBusyIndicator();
                }
                finally
                {
                    IsLoading = false;
                    //Globals.IsBusy = false;
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                }
            });
            t.Start();
        }

        public void StartSearching()
        {            
            _criteria = _searchCriteria;

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
            bSelectedHospitalInit = false;
            eventArgs.Cancel = true;
            
            var currentView = this.GetView() as IAutoCompleteView;
            if (currentView != null)
            {
                if (SearchCriteria != currentView.AutoCompleteBox.SearchText)
                    //&& !string.IsNullOrEmpty(currentView.AutoCompleteBox.SearchText))
                {
                    SearchCriteria = currentView.AutoCompleteBox.SearchText;
                    StartSearching();
                }                
            }
        }

        public void DropDownClosed(object source, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (SelectedHospital != null)
            {
                HosID = SelectedHospital.HosID;                
                if (_curSelHealthInsuranceCard != null)
                {
                    _curSelHealthInsuranceCard.HosID = SelectedHospital.HosID;
                    _curSelHealthInsuranceCard.RegistrationCode = SelectedHospital.HICode;
                    _curSelHealthInsuranceCard.RegistrationLocation = SelectedHospital.HosName;
                    //KMx: Tỉnh thành nơi cư trú của BN dựa vào ký tự thứ 4 và 5 trên mã thẻ BH, không phải dựa vào mã KCB-BĐ (11/11/2014 11:33).
                    //_curSelHealthInsuranceCard.CityProvinceName = Globals.GetDistrictNameFromHICode(SelectedHospital.HICode);
                }
            }
            else
            {
                HosID = 0;                
                if (_curSelHealthInsuranceCard != null)
                {
                    _curSelHealthInsuranceCard.HosID = 0;
                    _curSelHealthInsuranceCard.RegistrationCode = "";
                    _curSelHealthInsuranceCard.RegistrationLocation = "";
                    //KMx: Tỉnh thành nơi cư trú của BN dựa vào ký tự thứ 4 và 5 trên mã thẻ BH, không phải dựa vào mã KCB-BĐ (11/11/2014 11:33).
                    //_curSelHealthInsuranceCard.CityProvinceName = "";
                }
            }
        }


        public void LoadedCmd(object sender)
        {
            HosAutoComplete=(AxAutoComplete)sender;
        }
        public bool CanEditHiCode
        {
            get
            {
                return Globals.ServerConfigSection.HealthInsurances.IsCheckHICodeInPaperReferal && IsPaperReferal
                    && SelectedHospital != null && SelectedHospital.HosID > 0 && SelectedHospital.IsNoneHICodeOrig;
                //Phải có bệnh viện trong textbox rồi và bệnh viện đó chưa có mã code mới cho đổi mã - Anh Tuấn yêu cầu sửa mới như vậy (14/05/2016)
            }
        }
        private string _HICode_Orig;
        public string HICode_Orig
        {
            get
            {
                return _HICode_Orig;
            }
            set
            {
                _HICode_Orig = value;
                NotifyOfPropertyChange(() => HICode_Orig);
            }
        }
        // HPT: chỉ thay đổi mã code khi áp dụng luật mới cho giấy chuyển viện
        public void HospitalCode_LostFocus()
        {
            if (!CanEditHiCode)
            {
                return;
            }
            if (Globals.ServerConfigSection.HealthInsurances.IsCheckHICodeInPaperReferal && SelectedHospital.HICode == null || SelectedHospital.HICode.Trim().Length < 5)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.A0807_G1_Msg_InfoMaBV5KyTu));
                SelectedHospital.HICode = HICode_Orig; 
                return;
            }
            // HPT: Đã từng cho kiểm tra trùng mã nhưng anh Tuấn nói không nên kiểm tra trùng sẽ gây khó khăn trong trường hợp dưới đây
            // Dữ liệu có trong bảng thể sai sót. Khi người dùng nhập mã nơi chuyển viện cho nơi chuyển viện cũ (chưa có mã), mã được nhập vào là mã đúng nhưng vì đã tồn tại trong CSDL, hệ thống chặn không nhập được
            HICode_Orig = SelectedHospital.HICode;
        }

        private IPaperReferalFull _Parent;
        public IPaperReferalFull Parent
        {
            get
            {
                return _Parent;
            }
            set
            {
                _Parent = value;
                NotifyOfPropertyChange(() => Parent);
            }
        }
    }
}
