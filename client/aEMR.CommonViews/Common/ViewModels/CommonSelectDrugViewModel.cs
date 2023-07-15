using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ServiceClient;
using aEMR.Common.Collections;
using Castle.Core.Logging;
using Castle.Windsor;
using Caliburn.Micro;
using DataEntities;
using System.Threading;
using eHCMSLanguage;
using System.Linq;
using Service.Core.Common;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(ICommonSelectDrug)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CommonSelectDrugViewModel : Conductor<object>, ICommonSelectDrug
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public CommonSelectDrugViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            RefGenMedProductDetails = new ObservableCollection<RefGenMedProductDetails>();

            Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_MedProductType).ToObservableCollection();
            if (Globals.ServerConfigSection.ConsultationElements.ModeShowInforDrugForAutoCompleteInstruction > 1)
            {
                IsVisibleForRemaining = false;
            }
        }

        private bool _IsInputDosage;
        public bool IsInputDosage
        {
            get { return _IsInputDosage; }
            set
            {
                if (_IsInputDosage != value)
                {
                    _IsInputDosage = value;
                    NotifyOfPropertyChange(() => IsInputDosage);
                }
            }
        }

        private bool _calByUnitUse;
        public bool CalByUnitUse
        {
            get { return _calByUnitUse; }
            set
            {
                if (_calByUnitUse != value)
                {
                    _calByUnitUse = value;
                    NotifyOfPropertyChange(() => CalByUnitUse);
                }
            }
        }

        private long _V_MedProductType = (long)AllLookupValues.MedProductType.THUOC; //11001 : thuoc, 11002 : y cu , 11003 :hoa chat
        public long V_MedProductType
        {
            get
            {
                return _V_MedProductType;
            }
            set
            {
                if (_V_MedProductType != value)
                {
                    _V_MedProductType = value;
                    NotifyOfPropertyChange(() => V_MedProductType);
                    DoseVisibility = V_MedProductType == (long)AllLookupValues.MedProductType.THUOC;
                    NotifyOfPropertyChange(() => DoseVisibility);
                }

            }
        }

        private bool _IsFocusTextCode;
        public bool IsFocusTextCode
        {
            get { return _IsFocusTextCode; }
            set
            {
                if (_IsFocusTextCode != value)
                {
                    _IsFocusTextCode = value;
                }
                NotifyOfPropertyChange(() => IsFocusTextCode);
            }
        }

        private string _brandName;
        public string BrandName
        {
            get { return _brandName; }
            set
            {
                if (_brandName != value)
                {
                    _brandName = value;
                }
                NotifyOfPropertyChange(() => BrandName);
            }
        }

        private ReqOutwardDrugClinicDeptPatient _CurrentReqOutwardDrugClinicDeptPatient;
        public ReqOutwardDrugClinicDeptPatient CurrentReqOutwardDrugClinicDeptPatient
        {
            get { return _CurrentReqOutwardDrugClinicDeptPatient; }
            set
            {
                if (_CurrentReqOutwardDrugClinicDeptPatient != value)
                {
                    _CurrentReqOutwardDrugClinicDeptPatient = value;
                    NotifyOfPropertyChange(() => CurrentReqOutwardDrugClinicDeptPatient);
                }
            }
        }

        private ObservableCollection<RefGenMedProductDetails> _RefGenMedProductDetails;
        public ObservableCollection<RefGenMedProductDetails> RefGenMedProductDetails
        {
            get
            {
                return _RefGenMedProductDetails;
            }
            set
            {
                if (_RefGenMedProductDetails != value)
                {
                    _RefGenMedProductDetails = value;
                    NotifyOfPropertyChange(() => RefGenMedProductDetails);
                }
            }
        }

        private aEMR.Common.PagedCollectionView.PagedCollectionView _ReqOutwardDrugClinicDeptPatientlst;
        public aEMR.Common.PagedCollectionView.PagedCollectionView  ReqOutwardDrugClinicDeptPatientlst
        {
            get { return _ReqOutwardDrugClinicDeptPatientlst; }
            set
            {
                if (_ReqOutwardDrugClinicDeptPatientlst != value)
                {
                    _ReqOutwardDrugClinicDeptPatientlst = value;
                    NotifyOfPropertyChange(() => ReqOutwardDrugClinicDeptPatientlst);
                }
            }
        }

        private Action<ReqOutwardDrugClinicDeptPatient> _addItemCallback;
        public Action<ReqOutwardDrugClinicDeptPatient> AddItemCallback
        {
            get { return _addItemCallback; }
            set
            {
                if (_addItemCallback != value)
                {
                    _addItemCallback = value;
                    NotifyOfPropertyChange(() => AddItemCallback);
                }
            }
        }

        void RefGenMedProductDetails_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetRefGenMedProductDetails_Auto(BrandName, false);
        }

        TextBox tbx = null;
        public void AxTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            tbx = sender as TextBox;
        }

        private AutoCompleteBox acbAutoDrug_Text = null;
        public void AutoDrug_Text_Loaded(object sender, RoutedEventArgs e)
        {
            acbAutoDrug_Text = sender as AutoCompleteBox;
        }

        public void AutoDrug_LostFocus(object sender, RoutedEventArgs e)
        {
            if (acbAutoDrug_Text == null || string.IsNullOrEmpty(acbAutoDrug_Text.Text))
            {
                return;
            }
            if (IsInputDosage)
            {
                if (tbxMDoseStr == null)
                {
                    return;
                }
                tbxMDoseStr.Focus();
            }
            else
            {
                if (tbxQty == null)
                {
                    return;
                }
                tbxQty.Focus();
            }
        }


        TextBox tbxQty;
        public void tbxQty_Loaded(object sender, RoutedEventArgs e)
        {
            tbxQty = sender as TextBox;
        }

        TextBox tbxMDoseStr;
        public void tbxMDoseStr_Loaded(object sender, RoutedEventArgs e)
        {
            tbxMDoseStr = sender as TextBox;
        }

        public void AutoCompleteBox_Populating(object sender, PopulatingEventArgs e)
        {
            e.Cancel = true;
            BrandName = e.Parameter;
            GetRefGenMedProductDetails_Auto(BrandName, false);

        }
        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (RefGenMedProductDetails == null || RefGenMedProductDetails.Count < 1)
            {
                return;
            }
            RefGenMedProductDetails obj = acbAutoDrug_Text.SelectedItem as RefGenMedProductDetails;
            if (CurrentReqOutwardDrugClinicDeptPatient == null)
            {
                CurrentReqOutwardDrugClinicDeptPatient = new ReqOutwardDrugClinicDeptPatient();
            }
            if (obj != null && CurrentReqOutwardDrugClinicDeptPatient != null)
            {
                CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = obj;
            }
        }

        private void GetRefGenMedProductDetails_Auto(string BrandName, bool IsCode)
        {
            if (!IsCode && BrandName.Length < 1)
            {
                return;
            }

            IsFocusTextCode = IsCode;

            long? RefGenDrugCatID_1 = null;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginRefGenMedProductDetails_SearchAutoPaging_Choose(IsCode, BrandName, null, V_MedProductType, RefGenDrugCatID_1, null, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndRefGenMedProductDetails_SearchAutoPaging_Choose(asyncResult);
                            if (IsVisibleForRemaining)
                            {
                                //20190710 TTM: Để hàm Gr ở đây để trường hợp lấy tồn thì Gr không thì không Gr lại.
                                GroupRemaining(results);
                            }
                            if (IsCode)
                            {
                                if (results != null && results.Count > 0)
                                {
                                    if (CurrentReqOutwardDrugClinicDeptPatient == null)
                                    {
                                        CurrentReqOutwardDrugClinicDeptPatient = new ReqOutwardDrugClinicDeptPatient();
                                    }
                                    CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = results.FirstOrDefault();

                                    if (IsInputDosage)
                                    {
                                        if (tbxMDoseStr == null)
                                        {
                                            return;
                                        }
                                        tbxMDoseStr.Focus();
                                    }
                                    else
                                    {
                                        if (tbxQty == null)
                                        {
                                            return;
                                        }
                                        tbxQty.Focus();
                                    }


                                }
                                else
                                {
                                    if (CurrentReqOutwardDrugClinicDeptPatient != null)
                                    {
                                        CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = null;
                                    }

                                    if (tbx != null)
                                    {
                                        txt = "";
                                        tbx.Text = "";
                                        tbx.Focus();
                                    }
                                    if (acbAutoDrug_Text != null)
                                    {
                                        acbAutoDrug_Text.Text = "";
                                    }
                                    MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                            }
                            else
                            {
                                RefGenMedProductDetails.Clear();
                                if (IsVisibleForRemaining)
                                {
                                    RefGenMedProductDetails = RefGenMedProductDetailsListSum;
                                }
                                else
                                {
                                    RefGenMedProductDetails = results.ToObservableCollection();
                                }
                                acbAutoDrug_Text.PopulateComplete();
                                if (RefGenMedProductDetails == null || RefGenMedProductDetails.Count == 0)
                                {
                                    RefGenMedProductDetails = new ObservableCollection<RefGenMedProductDetails>();
                                }
                            }
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


        string txt = "";
        public void AxTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txt != (sender as TextBox).Text)
            {
                txt = (sender as TextBox).Text;

                System.Diagnostics.Debug.WriteLine(" ====> AxTextBox_LostFocus 1 .....");

                if (!string.IsNullOrEmpty(txt))
                {
                    string Code = Globals.FormatCode(V_MedProductType, txt);

                    GetRefGenMedProductDetails_Auto(Code, true);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(" ====> AxTextBox_LostFocus 2 .....");
                    CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = new RefGenMedProductDetails();
                }
            }
        }

        public void tbxMDoseStr_LostFocus(object sender, RoutedEventArgs e)
        {
            Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.MDose, CurrentReqOutwardDrugClinicDeptPatient);
        }

        public void tbxADoseStr_LostFocus(object sender, RoutedEventArgs e)
        {
            Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.ADose, CurrentReqOutwardDrugClinicDeptPatient);
        }

        public void tbxEDoseStr_LostFocus(object sender, RoutedEventArgs e)
        {
            Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.EDose, CurrentReqOutwardDrugClinicDeptPatient);
        }

        public void tbxNDoseStr_LostFocus(object sender, RoutedEventArgs e)
        {
            Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.NDose, CurrentReqOutwardDrugClinicDeptPatient);
        }
        public void btnAddItem()
        {
            System.Diagnostics.Debug.WriteLine(" ========> btnAddItem  1 .....");

            if (CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail == null || CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail.GenMedProductID == 0
                                    || string.IsNullOrEmpty(CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail.Code))
            {
                CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = new RefGenMedProductDetails();

                Globals.ShowMessage(eHCMSResources.Z1147_G1_ChonHgCanYC, eHCMSResources.G0442_G1_TBao);
                return;
            }

            if (CurrentReqOutwardDrugClinicDeptPatient.ReqQty <= 0)
            {
                Globals.ShowMessage(eHCMSResources.Z1161_G1_SLgCDinhKgHopLe, eHCMSResources.G0442_G1_TBao);
                return;
            }

            if (CalByUnitUse)
            {
                CurrentReqOutwardDrugClinicDeptPatient.ReqQty = CurrentReqOutwardDrugClinicDeptPatient.ReqQty / (CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail.DispenseVolume == 0 ? 1 : (decimal)CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail.DispenseVolume);
                CurrentReqOutwardDrugClinicDeptPatient.ReqQty = Math.Round(CurrentReqOutwardDrugClinicDeptPatient.ReqQty, 2);
            }

            CurrentReqOutwardDrugClinicDeptPatient.DateTimeSelection = Globals.GetCurServerDateTime();
            {
                CurrentReqOutwardDrugClinicDeptPatient.EntityState = EntityState.NEW;
                CurrentReqOutwardDrugClinicDeptPatient.PrescribedQty = CurrentReqOutwardDrugClinicDeptPatient.ReqQty;

                var item = CurrentReqOutwardDrugClinicDeptPatient.DeepCopy();

                AddItemCallback(item);
            }

            CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = new RefGenMedProductDetails();
            CurrentReqOutwardDrugClinicDeptPatient.MDoseStr = "0";
            CurrentReqOutwardDrugClinicDeptPatient.ADoseStr = "0";
            CurrentReqOutwardDrugClinicDeptPatient.EDoseStr = "0";
            CurrentReqOutwardDrugClinicDeptPatient.NDoseStr = "0";
            CurrentReqOutwardDrugClinicDeptPatient.MDose = 0;
            CurrentReqOutwardDrugClinicDeptPatient.ADose = 0;
            CurrentReqOutwardDrugClinicDeptPatient.EDose = 0;
            CurrentReqOutwardDrugClinicDeptPatient.NDose = 0;
            CurrentReqOutwardDrugClinicDeptPatient.ReqQty = 0;

            if (IsFocusTextCode)
            {
                if (tbx != null)
                {
                    txt = "";
                    tbx.Text = "";
                    tbx.Focus();
                }
            }
            else
            {
                if (acbAutoDrug_Text != null)
                {
                    acbAutoDrug_Text.Text = "";
                    acbAutoDrug_Text.Focus();
                }
            }
        }

        private ObservableCollection<Lookup> _gMedProductTypeCollection = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_MedProductType).ToObservableCollection();
        public ObservableCollection<Lookup> gMedProductTypeCollection
        {
            get
            {
                return _gMedProductTypeCollection;
            }
            set
            {
                _gMedProductTypeCollection = value;
                NotifyOfPropertyChange(() => gMedProductTypeCollection);
            }
        }

        private bool _DoseVisibility = true;
        public bool DoseVisibility
        {
            get
            {
                return _DoseVisibility;
            }
            set
            {
                _DoseVisibility = value;
                NotifyOfPropertyChange(() => DoseVisibility);
            }
        }

        private ObservableCollection<RefGenMedProductDetails> _RefGenMedProductDetailsSum;
        public ObservableCollection<RefGenMedProductDetails> RefGenMedProductDetailsListSum
        {
            get { return _RefGenMedProductDetailsSum; }
            set
            {
                if (_RefGenMedProductDetailsSum != value)
                    _RefGenMedProductDetailsSum = value;
                NotifyOfPropertyChange(() => RefGenMedProductDetailsListSum);
            }
        }
        private void GroupRemaining(IList<RefGenMedProductDetails> results)
        {
            var ListRefGMP = from RefGMP in results
                             group RefGMP by new
                             {
                                 RefGMP.GenMedProductID,
                                 RefGMP.BrandName,
                                 RefGMP.SelectedUnit.UnitName,
                                 RefGMP.RequestQty,
                                 RefGMP.Code,
                                 RefGMP.ProductCodeRefNum,
                                 RefGMP.RefGenMedDrugDetails.Content,
                                 RefGMP.GenericName
                             } into RefGMPGroup
                             select new
                             {
                                 Remaining = RefGMPGroup.Sum(groupItem => groupItem.Remaining),
                                 GenMedProductID = RefGMPGroup.Key.GenMedProductID,
                                 UnitName = RefGMPGroup.Key.UnitName,
                                 BrandName = RefGMPGroup.Key.BrandName,
                                 GenericName = RefGMPGroup.Key.GenericName,
                                 Content = RefGMPGroup.Key.Content,
                                 Code = RefGMPGroup.Key.Code,
                                 Qty = RefGMPGroup.Key.RequestQty,
                                 ProductCodeRefNum = RefGMPGroup.Key.ProductCodeRefNum
                             };
            RefGenMedProductDetailsListSum = new ObservableCollection<RefGenMedProductDetails>();
            foreach (var Details in ListRefGMP)
            {
                RefGenMedProductDetails item = new RefGenMedProductDetails();
                item.GenMedProductID = Details.GenMedProductID;
                item.BrandName = Details.BrandName;
                item.SelectedUnit = new RefUnit();
                item.SelectedUnit.UnitName = Details.UnitName;
                item.Code = Details.Code;
                item.Remaining = Details.Remaining;
                item.RequestQty = Details.Qty;
                item.ProductCodeRefNum = Details.ProductCodeRefNum;
                RefGenMedProductDetailsListSum.Add(item);
            }
        }

        private bool _IsVisibleForRemaining = true;
        public bool IsVisibleForRemaining
        {
            get { return _IsVisibleForRemaining; }
            set
            {
                if (_IsVisibleForRemaining != value)
                {
                    _IsVisibleForRemaining = value;
                    NotifyOfPropertyChange(() => IsVisibleForRemaining);
                }
            }
        }

    }
}