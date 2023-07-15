
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using System;
using eHCMSLanguage;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Common;
using aEMR.Common.BaseModel;
using System.Linq;
using aEMR.Common.Collections;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IMedicalControl)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MedicalControlViewModel : ViewModelBase, IMedicalControl
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public MedicalControlViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            RefMedicalConditionType = new RefMedContraIndicationTypes();
            refIDC10Code = new ObservableCollection<DiseasesReference>();
            GetRefMedCondType();
            var RefGenDrugListVM = Globals.GetViewModel<IRefGenDrugListEx>();
            DrugList = RefGenDrugListVM;
            ActivateItem(RefGenDrugListVM);

            ListV_AgeUnit = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_AgeUnit).ToObservableCollection();
            Lookup firstItem = new Lookup { LookupID = 0 };
            ListV_AgeUnit.Insert(0, firstItem);
        }
        private ObservableCollection<Lookup> _ListV_AgeUnit;
        public ObservableCollection<Lookup> ListV_AgeUnit
        {
            get { return _ListV_AgeUnit; }
            set
            {
                if (_ListV_AgeUnit != value)
                    _ListV_AgeUnit = value;
                NotifyOfPropertyChange(() => ListV_AgeUnit);
            }
        }
        private string _TitleForm = Globals.PageName;
        public string TitleForm
        {
            get
            {
                return _TitleForm;
            }
            set
            {
                _TitleForm = value;
                NotifyOfPropertyChange(() => TitleForm);
            }
        }

        public object DrugList { get; set; }

        private string _txtTypeName;
        public string txtTypeName
        {
            get
            {
                return _txtTypeName;
            }
            set
            {
                if (_txtTypeName == value)
                    return;
                _txtTypeName = value;
                NotifyOfPropertyChange(() => txtTypeName);
            }
        }
        private string _txtMedicalCond;
        public string txtMedicalCond
        {
            get
            {
                return _txtMedicalCond;
            }
            set
            {
                if (_txtMedicalCond == value)
                    return;
                _txtMedicalCond = value;
                NotifyOfPropertyChange(() => txtMedicalCond);
            }
        }

        private int? _AgeFrom;
        public int? AgeFrom
        {
            get
            {
                return _AgeFrom;
            }
            set
            {
                _AgeFrom = value;
                NotifyOfPropertyChange(() => AgeFrom);
            }
        }

        private int? _AgeTo;
        public int? AgeTo
        {
            get
            {
                return _AgeTo;
            }
            set
            {
                _AgeTo = value;
                NotifyOfPropertyChange(() => AgeTo);
            }
        }

        #region properties
        private ObservableCollection<RefMedContraIndicationTypes> _allRefMedicalConditionType;
        public ObservableCollection<RefMedContraIndicationTypes> allRefMedicalConditionType
        {
            get
            {
                return _allRefMedicalConditionType;
            }
            set
            {
                if (_allRefMedicalConditionType == value)
                    return;
                _allRefMedicalConditionType = value;
                NotifyOfPropertyChange(() => allRefMedicalConditionType);
            }
        }

        private RefMedContraIndicationTypes _selectedRefMedicalConditionType;

        public RefMedContraIndicationTypes selectedRefMedicalConditionType
        {
            get
            {
                return _selectedRefMedicalConditionType;
            }
            set
            {
                if (_selectedRefMedicalConditionType == value)
                    return;
                _selectedRefMedicalConditionType = value;
                NotifyOfPropertyChange(() => selectedRefMedicalConditionType);
            }
        }

        private RefMedContraIndicationTypes _RefMedicalConditionType;

        public RefMedContraIndicationTypes RefMedicalConditionType
        {
            get
            {
                return _RefMedicalConditionType;
            }
            set
            {
                if (_RefMedicalConditionType == value)
                    return;
                _RefMedicalConditionType = value;
                NotifyOfPropertyChange(() => RefMedicalConditionType);
            }
        }

        private ObservableCollection<RefMedContraIndicationICD> _allRefMedicalCondition;
        public ObservableCollection<RefMedContraIndicationICD> allRefMedicalCondition
        {
            get
            {
                return _allRefMedicalCondition;
            }
            set
            {
                if (_allRefMedicalCondition == value)
                    return;
                _allRefMedicalCondition = value;
                NotifyOfPropertyChange(() => allRefMedicalCondition);
            }
        }

        private RefMedContraIndicationICD _selectedRefMedicalCondition;
        public RefMedContraIndicationICD selectedRefMedicalCondition
        {
            get
            {
                return _selectedRefMedicalCondition;
            }
            set
            {
                if (_selectedRefMedicalCondition == value)
                    return;
                _selectedRefMedicalCondition = value;
                NotifyOfPropertyChange(() => selectedRefMedicalCondition);
            }
        }
        private ObservableCollection<DiseasesReference> _refIDC10Code;
        public ObservableCollection<DiseasesReference> refIDC10Code
        {
            get
            {
                return _refIDC10Code;
            }
            set
            {
                if (_refIDC10Code != value)
                {
                    _refIDC10Code = value;
                }
                NotifyOfPropertyChange(() => refIDC10Code);
            }
        }
        private string _BasicDiagTreatment;
        public string BasicDiagTreatment
        {
            get
            {
                return _BasicDiagTreatment;
            }
            set
            {
                if (_BasicDiagTreatment != value)
                {
                    _BasicDiagTreatment = value;
                }
                NotifyOfPropertyChange(() => BasicDiagTreatment);
            }
        }
        #endregion

        #region method
        public void butDelMedType()
        {
            if (selectedRefMedicalConditionType != null)
            {
                //20190322 TBL: Khong can set IsActive cho RefMedicalConditions
                //DeleteRefMedicalConditions(0,Convert.ToInt32(selectedRefMedicalConditionType.MCTypeID));
                DeleteRefMedicalConditionTypes(Convert.ToInt32(selectedRefMedicalConditionType.MedContraTypeID));
            }
            else
            {
                MessageBox.Show(eHCMSResources.Z2624_G1_ChuaChonLoaiChongChiDinhDeXoa);
            }
        }
        public void butAddMedType()
        {
            RefMedicalConditionType.AgeFrom = RefMedicalConditionType.AgeFrom == null ? 0 : RefMedicalConditionType.AgeFrom;
            RefMedicalConditionType.AgeTo = RefMedicalConditionType.AgeTo == null ? 0 : RefMedicalConditionType.AgeTo;
            if (RefMedicalConditionType.AgeTo != 0 && RefMedicalConditionType.AgeFrom != 0 && RefMedicalConditionType.AgeTo <= RefMedicalConditionType.AgeFrom)
            {
                MessageBox.Show("Kiểm tra lại giá trị nhập. Từ ngày lớn hơn đến ngày");
                return;
            }
            if (!CheckValidAge())
            {
                MessageBox.Show("Chống chỉ định theo độ tuổi phải nhập đầy đủ tuổi và loại tuổi");
                return;
            }
            if (RefMedicalConditionType.AgeTo < 0 || RefMedicalConditionType.AgeFrom < 0)
            {
                MessageBox.Show("Kiểm tra lại giá trị nhập, không được nhập số âm");
                return;
            }
            if (RefMedicalConditionType != null && !string.IsNullOrEmpty(RefMedicalConditionType.MedContraIndicationType))
            {
                InsertRefMedicalConditionTypes(RefMedicalConditionType.MedContraIndicationType, 0, RefMedicalConditionType.AgeFrom, RefMedicalConditionType.AgeTo, RefMedicalConditionType.V_AgeUnit);
                RefMedicalConditionType = new RefMedContraIndicationTypes();
            }
            else
            {
                MessageBox.Show(eHCMSResources.Z2625_G1_ChuaNhapLoaiChongChiDinh);
            }
        }
        public void butEditMedType()
        {
            RefMedicalConditionType.AgeFrom = RefMedicalConditionType.AgeFrom == null ? 0 : RefMedicalConditionType.AgeFrom;
            RefMedicalConditionType.AgeTo = RefMedicalConditionType.AgeTo == null ? 0 : RefMedicalConditionType.AgeTo;
            if (RefMedicalConditionType.AgeTo!=0 && RefMedicalConditionType.AgeFrom!=0 && RefMedicalConditionType.AgeTo <= RefMedicalConditionType.AgeFrom)
            {
                MessageBox.Show("Kiểm tra lại giá trị nhập. Từ ngày lớn hơn đến ngày");
                return;
            }
            if (!CheckValidAge())
            {
                MessageBox.Show("Chống chỉ định theo độ tuổi phải nhập đầy đủ tuổi và loại tuổi");
                return;
            }
            if (RefMedicalConditionType.AgeTo < 0 || RefMedicalConditionType.AgeFrom < 0)
            {
                MessageBox.Show("Kiểm tra lại giá trị nhập, không được nhập số âm");
                return;
            }
            if (selectedRefMedicalConditionType != null)
            {
                UpdateRefMedicalConditionTypes(Convert.ToInt32(selectedRefMedicalConditionType.MedContraTypeID), RefMedicalConditionType.MedContraIndicationType, RefMedicalConditionType.AgeFrom, RefMedicalConditionType.AgeTo, RefMedicalConditionType.V_AgeUnit);
                RefMedicalConditionType = new RefMedContraIndicationTypes();
            }
            else
            {
                MessageBox.Show(eHCMSResources.Z2626_G1_ChuaChonLoaiChongChiDinhDeSua);
            }
        }
        private bool CheckValidAge()
        {
            if(((RefMedicalConditionType.AgeFrom != 0 || RefMedicalConditionType.AgeTo != 0) && RefMedicalConditionType.V_AgeUnit != 0) 
                || ( RefMedicalConditionType.AgeFrom == 0 && RefMedicalConditionType.AgeTo == 0 && RefMedicalConditionType.V_AgeUnit == 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void butAdd()
        {
            if (selectedRefMedicalConditionType == null)
            {
                MessageBox.Show(eHCMSResources.Z2628_G1_ChuaChonLoaiChongChiDinh);
                return;
            }
            if (Acb_ICD10_Code.SelectedItem != null && (Acb_ICD10_Code.SelectedItem as DiseasesReference).IDCode > 0)
            {
                foreach (var item in allRefMedicalCondition)
                {
                    if (item.ICD10Code == (Acb_ICD10_Code.SelectedItem as DiseasesReference).ICD10Code)
                    {
                        MessageBox.Show(eHCMSResources.Z2627_G1_DaTonTaiICD10Nay);
                        return;
                    }
                }
                InsertRefMedicalConditions(Convert.ToInt32(selectedRefMedicalConditionType.MedContraTypeID), (Acb_ICD10_Code.SelectedItem as DiseasesReference).ICD10Code, (Acb_ICD10_Code.SelectedItem as DiseasesReference).DiseaseNameVN);
                BasicDiagTreatment = "";
                Acb_ICD10_Code.Text = "";
            }
            else
            {
                MessageBox.Show(eHCMSResources.Z2623_G1_ChuaChonICD10);
            }
        }
        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteRefMedicalConditions(Convert.ToInt32(selectedRefMedicalCondition.MCID), 0);

        }
        public void listContraSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectedRefMedicalConditionType != null)
            {
                RefMedicalConditionType = selectedRefMedicalConditionType.DeepCopy();
                GetRefMedicalConditions(Convert.ToInt32(selectedRefMedicalConditionType.MedContraTypeID));
            }
        }
        private void GetRefMedCondType()
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetRefMedCondType(Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetRefMedCondType(asyncResult);
                            if (results != null)
                            {
                                if (allRefMedicalConditionType == null)
                                {
                                    allRefMedicalConditionType = new ObservableCollection<RefMedContraIndicationTypes>();
                                }
                                else
                                {
                                    allRefMedicalConditionType.Clear();
                                }
                                foreach (var p in results)
                                {
                                    allRefMedicalConditionType.Add(p);
                                }
                                NotifyOfPropertyChange(() => allRefMedicalConditionType);
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

            });

            t.Start();
        }


        private void GetRefMedicalConditions(int MCTypeID)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetRefMedicalConditions(MCTypeID, Globals.DispatchCallback((asyncResult) =>
                     {
                         try
                         {
                             var results = contract.EndGetRefMedicalConditions(asyncResult);
                             if (results != null)
                             {
                                 if (allRefMedicalCondition == null)
                                 {
                                     allRefMedicalCondition = new ObservableCollection<RefMedContraIndicationICD>();
                                 }
                                 else
                                 {
                                     allRefMedicalCondition.Clear();
                                 }
                                 foreach (var p in results)
                                 {
                                     allRefMedicalCondition.Add(p);
                                 }
                                 NotifyOfPropertyChange(() => allRefMedicalCondition);
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

            });

            t.Start();
        }


        private void DeleteRefMedicalConditions(int MCID, int MCTypeID)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDeleteRefMedicalConditions(MCID, MCTypeID, Globals.LoggedUserAccount.StaffID.Value, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndDeleteRefMedicalConditions(asyncResult);
                            if (results == true)
                            {
                                GetRefMedicalConditions(Convert.ToInt32(selectedRefMedicalConditionType.MedContraTypeID));
                                MessageBox.Show(eHCMSResources.K0537_G1_XoaOk);
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

            });

            t.Start();
        }
        private void DeleteRefMedicalConditionTypes(int MCTypeID)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDeleteRefMedicalConditionTypes(MCTypeID, Globals.LoggedUserAccount.StaffID.Value, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndDeleteRefMedicalConditionTypes(asyncResult);
                            if (results == true)
                            {
                                GetRefMedCondType();
                                MessageBox.Show(eHCMSResources.K0537_G1_XoaOk);
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

            });

            t.Start();
        }
        private void UpdateRefMedicalConditionTypes(int MCTypeID, string MedConditionType, int? AgeFrom, int? AgeTo, long V_AgeUnit)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateRefMedicalConditionTypes(MCTypeID, MedConditionType, AgeFrom, AgeTo, V_AgeUnit, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndUpdateRefMedicalConditionTypes(asyncResult);
                            if (results == true)
                            {
                                GetRefMedCondType();
                                MessageBox.Show(eHCMSResources.A0296_G1_Msg_InfoSuaOK);
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

            });

            t.Start();
        }
        private void InsertRefMedicalConditionTypes(string MedConditionType, int Idx, int? AgeFrom, int? AgeTo, long V_AgeUnit)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginInsertRefMedicalConditionTypes(MedConditionType, Idx, AgeFrom, AgeTo, V_AgeUnit, Globals.DispatchCallback((asyncResult) =>
                   {

                       try
                       {
                           var results = contract.EndInsertRefMedicalConditionTypes(asyncResult);
                           if (results == true)
                           {
                               GetRefMedCondType();
                               MessageBox.Show(eHCMSResources.A1027_G1_Msg_InfoThemOK);
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

            });

            t.Start();
        }

        private void InsertRefMedicalConditions(int MCTypeID, string ICD10Code, string DiseaseNameVN)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginInsertRefMedicalConditions(MCTypeID, ICD10Code, DiseaseNameVN, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndInsertRefMedicalConditions(asyncResult);
                            if (results == true)
                            {
                                GetRefMedicalConditions(Convert.ToInt32(selectedRefMedicalConditionType.MedContraTypeID));
                                MessageBox.Show(eHCMSResources.A1027_G1_Msg_InfoThemOK);
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

            });

            t.Start();
        }
        AutoCompleteBox Acb_ICD10_Code = null;
        public void AcbICD10Code_Loaded(object sender, RoutedEventArgs e)
        {
            Acb_ICD10_Code = (AutoCompleteBox)sender;
        }
        public void aucICD10_Populating(object sender, PopulatingEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Parameter))
            {
                LoadRefDiseases(e.Parameter, 0, 0, 100);
            }
        }
        public void LoadRefDiseases(string name, byte type, int PageIndex, int PageSize)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginSearchRefDiseases(name, PageIndex, PageSize, type
                        , 0
                        , Globals.GetCurServerDateTime()
                        , Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int Total = 10;
                            var results = contract.EndSearchRefDiseases(out Total, asyncResult);
                            refIDC10Code.Clear();
                            if (results != null)
                            {
                                foreach (DiseasesReference p in results)
                                {
                                    refIDC10Code.Add(p);
                                }
                            }
                            Acb_ICD10_Code.ItemsSource = refIDC10Code;
                            Acb_ICD10_Code.PopulateComplete();
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
        private bool isDropDown = false;
        public void AxAutoComplete_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (!isDropDown)
            {
                return;
            }
            isDropDown = false;
            if (Acb_ICD10_Code != null)
            {
                DiseasesReference BDiagTreatment = Acb_ICD10_Code.SelectedItem as DiseasesReference;
                if (BDiagTreatment != null)
                {
                    BasicDiagTreatment = BDiagTreatment.DiseaseNameVN;
                }
            }
        }
        public void AxAutoComplete_DropDownClosing(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            isDropDown = true;
        }
        #endregion
    }
}
