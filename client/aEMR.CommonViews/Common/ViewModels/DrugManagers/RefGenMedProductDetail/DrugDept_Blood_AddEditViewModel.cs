using eHCMSLanguage;
using System;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using DataEntities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using aEMR.ViewContracts;
using aEMR.Common.BaseModel;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure;
using aEMR.Common.Collections;
using aEMR.ServiceClient;
using aEMR.CommonTasks;
using aEMR.Controls;
using aEMR.Common.Utilities;
using aEMR.DataContracts;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
/*
 * 23102017 #001 DPT: Added button create new from old value
 * 20170810 #002 CMN: Added Bid Service
 * 20180109 #003 CMN: Added more properties for HI Informations.
 * 20180505 #004 TBLD: Added method get services type and services. Added out list RefMedicalServiceItem.
 * 20181006 #005 TTM:    Thay đổi cách lấy giá trị cột từ e.Column.GetValue(FrameworkElement.NameProperty).ToString()
 *                       => e.Column.Equals("Tên Grid".GetColumnByName("Tên cột") vì cách cũ bị sai => Lấy giá trị cột không đc. 
 * 20190916 #006 TBL: BM 0014339: Khi tạo mới dựa trên cũ xóa những thông tin của thầu 
 * 20191223 #007 TBL: BM 0021758: Thay đổi cách lưu trường Nhóm hàng, Code BHYT trong danh mục Y cụ
 */
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IDrugDept_Blood_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DrugDept_Blood_AddEditViewModel : ViewModelBase, IDrugDept_Blood_AddEdit
        , IHandle<DrugDeptCloseSearchSupplierEvent>
        , IHandle<ChooseDrugClass>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public DrugDept_Blood_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            eventArg.Subscribe(this);

            Hospitals = new ObservableCollection<Hospital>();
            /*▼====: #003*/
            ProductScopeCollection = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_ProductScope).ToObservableCollection();
            LoadHITransactions();
            /*▲====: #003*/
            /*▼====: #004*/
            GetServiceTypes();
            /*▲====: #004*/
            eventArg.Subscribe(this);
            if (Globals.allHospitals == null)
            {
                var vm = Globals.GetViewModel<IHome>();
                vm.LoadAllHospitalInfoAction();
            }
        }
        /*▼====: #001*/
        private bool _HasOldValue = true;
        public bool HasOldValue
        {
            get { return _HasOldValue; }
            set
            {
                if (_HasOldValue != value)
                {
                    _HasOldValue = value;
                    NotifyOfPropertyChange(() => HasOldValue);
                }
            }
        }
        public void btAddNewFromOldValue()
        {
            ObjRefGenMedProductDetails_Current.GenMedProductID = 0;
            /*▼====: #006*/
            ObjRefGenMedProductDetails_Current.InsuranceCover = false;
            ObjRefGenMedProductDetails_Current.HICode = "";
            ObjRefGenMedProductDetails_Current.ReportBrandName = "";
            ObjRefGenMedProductDetails_Current.BidName = "";
            ObjRefGenMedProductDetails_Current.BidCode = "";
            ObjRefGenMedProductDetails_Current.TLThanhToan = 0;
            ObjRefGenMedProductDetails_Current.MaxHIPay = 0;
            ObjRefGenMedProductDetails_Current.BiddingHospital = "";
            ObjRefGenMedProductDetails_Current.BidDecisionNumAndOrdinalNum = "";
            ObjRefGenMedProductDetails_Current.V_ProductScope = 0;
            ObjRefGenMedProductDetails_Current.BidDecisionNumAndEffectiveDate = "";
            ObjRefGenMedProductDetails_Current.BidExpirationDate = "";
            ObjRefGenMedProductDetails_Current.OtherDecisionNumAndEffectiveDate = "";
            ObjRefGenMedProductDetails_Current.CeilingPrice2ndItem = null;
            ObjRefGenMedProductDetails_Current.PaymentRateOfHIAddedItem = null;
            ObjRefGenMedProductDetails_Current.VAT = 0;
            ObjRefGenMedProductDetails_Current.IsNotVat = false;
            /*▲====: #006*/
            HasOldValue = false;
        }
        /*▲====: #001*/

        private bool _IsShowDispenseVolume;
        public bool IsShowDispenseVolume
        {
            get
            {
                return _IsShowDispenseVolume;
            }
            set
            {
                _IsShowDispenseVolume = value;
                NotifyOfPropertyChange(() => IsShowDispenseVolume);
            }
        }

        private bool _CanEdit = true;
        public bool CanEdit
        {
            get { return _CanEdit; }
            set
            {
                _CanEdit = value;
                NotifyOfPropertyChange(() => CanEdit);
            }

        }

        public override bool IsProcessing
        {
            get
            {
                return _isWaitingSave;
            }
        }

        public override string StatusText
        {
            get
            {
                if (_isWaitingSave)
                {
                    return eHCMSResources.Z0343_G1_DangLuu;
                }
                return string.Empty;
            }
        }

        private bool _isWaitingSave;
        public bool IsWaitingSave
        {
            get { return _isWaitingSave; }
            set
            {
                if (_isWaitingSave != value)
                {
                    _isWaitingSave = value;
                    NotifyOfPropertyChange(() => IsWaitingSave);
                    NotifyWhenBusy();

                    NotifyOfPropertyChange(() => CanbtSave);
                }
            }
        }

        public bool IsReadOnlyCode
        {
            get { return Globals.ServerConfigSection.MedDeptElements.AutoCreateMedCode; }
        }

        private Int64 _V_MedProductType;
        public Int64 V_MedProductType
        {
            get { return _V_MedProductType; }
            set
            {
                _V_MedProductType = value;
                NotifyOfPropertyChange(() => V_MedProductType);
            }

        }


        protected override void OnActivate()
        {
            base.OnActivate();

            #region get reference member

            Coroutine.BeginExecute(DoGetSupplierList());
            //Coroutine.BeginExecute(DoGetHospitalList());
            Coroutine.BeginExecute(DoGetRefGenericDrugCategory_1List());

            Coroutine.BeginExecute(DoGetUnitList());
            Coroutine.BeginExecute(DoGetDrugClassList());
            Coroutine.BeginExecute(DoCountryListList());
            Coroutine.BeginExecute(DoPharmaceuticalCompanyListList());
            #endregion
        }

        private string _TitleForm;
        public string TitleForm
        {
            get { return _TitleForm; }
            set
            {
                _TitleForm = value;
                NotifyOfPropertyChange(() => TitleForm);
            }
        }

        private RefGenMedProductDetails _ObjRefGenMedProductDetails_Current;
        public RefGenMedProductDetails ObjRefGenMedProductDetails_Current
        {
            get { return _ObjRefGenMedProductDetails_Current; }
            set
            {
                _ObjRefGenMedProductDetails_Current = value;
                if (ObjRefGenMedProductDetails_Current != null && string.IsNullOrEmpty(ObjRefGenMedProductDetails_Current.HIReportGroupCode))
                { ObjRefGenMedProductDetails_Current.HIReportGroupCode = "10"; }
                NotifyOfPropertyChange(() => ObjRefGenMedProductDetails_Current);
            }
        }
        AutoCompleteBox Auto_DrugClass;
        AutoCompleteBox Auto_PharmaCompany;
        AutoCompleteBox Auto_PharmaCountry;
        AutoCompleteBox Auto_Hospital;

        public void Auto_DrugClass_Loaded(object sender, RoutedEventArgs e)
        {
            Auto_DrugClass = sender as AutoCompleteBox;
        }

        public void Auto_DrugClass_Populating(object sender, PopulatingEventArgs e)
        {
            if (AllFamilyTherapies == null || AllFamilyTherapies.Count <= 0)
            {
                return;
            }
            //20180713 TBL: Them kiem tra e.Parameter neu khong se bao loi 
            if (e.Parameter == null)
            {
                return;
            }

            FamilyTherapies = new ObservableCollection<DrugClass>();
            foreach (var x in AllFamilyTherapies)
            {
                string str = VNConvertString.ConvertString(x.FaName);
                str = str.ToLower();
                bool bres = str.Contains(VNConvertString.ConvertString(e.Parameter).ToLower());
                if (bres)
                {
                    FamilyTherapies.Add(x);
                }
            }
            Auto_DrugClass.ItemsSource = FamilyTherapies;
            Auto_DrugClass.PopulateComplete();
        }

        public void Auto_PharmaCompany_Loaded(object sender, RoutedEventArgs e)
        {
            Auto_PharmaCompany = sender as AutoCompleteBox;
        }

        public void Auto_PharmaCompany_Populating(object sender, PopulatingEventArgs e)
        {
            if (AllPharmaceuticalCompanies == null || AllPharmaceuticalCompanies.Count <= 0)
            {
                return;
            }
            //20180713 TBL: Them kiem tra e.Parameter neu khong se bao loi 
            if (e.Parameter == null)
            {
                return;
            }

            PharmaceuticalCompanies = new ObservableCollection<DrugDeptPharmaceuticalCompany>();
            foreach (var x in AllPharmaceuticalCompanies)
            {
                string str = VNConvertString.ConvertString(x.PCOName);
                str = str.ToLower();
                bool bres = str.Contains(VNConvertString.ConvertString(e.Parameter).ToLower());
                if (bres)
                {
                    PharmaceuticalCompanies.Add(x);
                }
            }
            Auto_PharmaCompany.ItemsSource = PharmaceuticalCompanies;
            Auto_PharmaCompany.PopulateComplete();
        }

        public void Auto_PharmaCountry_Loaded(object sender, RoutedEventArgs e)
        {
            Auto_PharmaCountry = sender as AutoCompleteBox;
        }

        public void Auto_PharmaCountry_Populating(object sender, PopulatingEventArgs e)
        {
            if (AllCountries == null || AllCountries.Count <= 0)
            {
                return;
            }
            //20180713 TBL: Them kiem tra e.Parameter neu khong se bao loi 
            if (e.Parameter == null)
            {
                return;
            }

            Countries = new ObservableCollection<RefCountry>();
            foreach (var x in AllCountries)
            {
                string str = VNConvertString.ConvertString(x.CountryName);
                str = str.ToLower();
                bool bres = str.Contains(VNConvertString.ConvertString(e.Parameter).ToLower());
                if (bres)
                {
                    Countries.Add(x);
                }
            }
            Auto_PharmaCountry.ItemsSource = Countries;
            Auto_PharmaCountry.PopulateComplete();
        }

        public void Auto_Hospital_Loaded(object sender, RoutedEventArgs e)
        {
            Auto_Hospital = sender as AutoCompleteBox;
        }

        public void InitializeNewItem(Int64 pV_MedProductType)
        {
            ObjRefGenMedProductDetails_Current = new RefGenMedProductDetails();

            ObjRefGenMedProductDetails_Current.UnitPackaging = 1;
            ObjRefGenMedProductDetails_Current.NumberOfEstimatedMonths_F = 1;
            ObjRefGenMedProductDetails_Current.V_MedProductType = pV_MedProductType;

            ObjRefGenMedProductDetails_Current.IsActive = true;

            ObjRefGenMedProductDetails_Current.RefGenMedDrugDetails = new RefGenMedDrugDetails();

            ObjRefGenMedProductDetails_Current.RefGenMedDrugDetails.AdvTimeBeforeExpire = 1;
            AddBlankRow();

            if (Auto_DrugClass != null)
            {
                Auto_DrugClass.Text = "";
            }

            if (Auto_PharmaCompany != null)
            {
                Auto_PharmaCompany.Text = "";
            }

            if (Auto_PharmaCountry != null)
            {
                Auto_PharmaCountry.Text = "";
            }

            if (Auto_Hospital != null)
            {
                Auto_Hospital.Text = "";
            }
            /*▼====: #001*/
            HasOldValue = false;
            /*▲====: #001*/

        }
        ObservableCollection<RefMedContraIndicationTypes> temp = null;
        private void RefGenMedProductDetails_Save(RefGenMedProductDetails Obj)
        {
            if (Obj.InsuranceCover == true && Obj.V_ProductScope != (long)AllLookupValues.V_ProductScope.InHIScope)
            {
                MessageBox.Show(eHCMSResources.Z2194_G1_PhamViBHKhongHopLe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                IsWaitingSave = false;
                return;
            }
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0343_G1_DangLuu) });
            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginRefGenMedProductDetails_Save(Obj, temp, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Res = "";
                            long GenMedProductID;
                            contract.EndRefGenMedProductDetails_Save(out Res, out GenMedProductID, asyncResult);
                            /*▼====: #001*/
                            if (GenMedProductID > 0)
                                HasOldValue = true;
                            /*▲====: #001*/
                            string[] arrRes = new string[2];

                            switch (Res)
                            {
                                case "Update-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A0608_G1_Msg_InfoHChinhFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-1":
                                    {
                                        TitleForm = string.Format("{0} ", eHCMSResources.T1484_G1_HChinh) + TextType() + " " + ": " + ObjRefGenMedProductDetails_Current.BrandName.Trim();

                                        Globals.EventAggregator.Publish(new Drug_AddEditViewModel_Save_Event() { Result = true });

                                        MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);

                                        break;
                                    }
                                case "Insert-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A1027_G1_Msg_InfoThemOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Insert-1":
                                    {
                                        Obj.GenMedProductID = GenMedProductID;
                                        Globals.EventAggregator.Publish(new Drug_AddEditViewModel_Save_Event() { Result = true });

                                        MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                        break;
                                    }
                                default:
                                    {
                                        arrRes = Res.Split('@');

                                        string msg1 = "";
                                        string msg2 = "";

                                        if (arrRes[0] != "")
                                        {
                                            msg1 = eHCMSResources.Z0724_G1_TenThuongMaiDaDuocSD;
                                        }

                                        if (arrRes[1] != "")
                                        {
                                            msg2 = eHCMSResources.Z0725_G1_CodeDaDuocSD;
                                        }

                                        string msg = (msg1 == "" ? "" : "- " + msg1) + (msg2 != "" ? Environment.NewLine + "- " + msg2 : "");

                                        MessageBox.Show(msg + Environment.NewLine + eHCMSResources.I0946_G1_I, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                        IsWaitingSave = false;
                                        break;
                                    }
                            }

                        }
                        catch (Exception ex)
                        {
                            //Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        }
                        finally
                        {
                            IsWaitingSave = false;
                            this.HideBusyIndicator();
                        }
                    }), null);
                }


            });
            t.Start();
        }

        private bool CheckData()
        {
            if (ObjRefGenMedProductDetails_Current.SelectedCountry != null)
            {
                ObjRefGenMedProductDetails_Current.CountryID = ObjRefGenMedProductDetails_Current.SelectedCountry.CountryID;
            }

            if (ObjRefGenMedProductDetails_Current.SelectedUnit != null)
            {
                ObjRefGenMedProductDetails_Current.UnitID = ObjRefGenMedProductDetails_Current.SelectedUnit.UnitID;

            }

            if (ObjRefGenMedProductDetails_Current.SelectedUnitUse != null)
            {
                ObjRefGenMedProductDetails_Current.UnitUseID = ObjRefGenMedProductDetails_Current.SelectedUnitUse.UnitID;
            }


            if (ObjRefGenMedProductDetails_Current.SupplierGenMedProducts == null || ObjRefGenMedProductDetails_Current.SupplierGenMedProducts.Count == 0)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0722_G1_ChonNCCChoThuoc));
                return false;
            }
            else
            {
                int icount = 0;
                for (int i = 0; i < ObjRefGenMedProductDetails_Current.SupplierGenMedProducts.Count; i++)
                {
                    if (ObjRefGenMedProductDetails_Current.SupplierGenMedProducts[i].SupplierID > 0)
                    {
                        if (ObjRefGenMedProductDetails_Current.SupplierGenMedProducts[i].UnitPrice <= 0 || ObjRefGenMedProductDetails_Current.SupplierGenMedProducts[i].PackagePrice <= 0)
                        {
                            MessageBox.Show(string.Format("{0} {1}", eHCMSResources.A0519_G1_Msg_InfoDGiaLonHon0, eHCMSResources.A1028_G1_Msg_YCKtraTTin));
                            return false;
                        }
                        if (ObjRefGenMedProductDetails_Current.SupplierGenMedProducts[i].VAT < 1)
                        {
                            MessageBox.Show(eHCMSResources.K0265_G1_VATKhongHopLe3);
                            return false;
                        }
                    }
                    if (ObjRefGenMedProductDetails_Current.SupplierGenMedProducts[i].SupplierID > 0 && ObjRefGenMedProductDetails_Current.SupplierGenMedProducts[i].IsMain == true)
                    {
                        icount++;
                        if (icount > 1)
                        {
                            MessageBox.Show(eHCMSResources.A0436_G1_Msg_InfoCoNhieuHon1NCC);
                            return false;
                        }
                    }
                }
                if (icount == 0)
                {
                    MessageBox.Show(eHCMSResources.K0028_G1_ThuocChuaCoNCCChinh);
                    return false;
                }
                if (ObjRefGenMedProductDetails_Current.TLThanhToan < 0 || ObjRefGenMedProductDetails_Current.TLThanhToan > 100)
                {
                    MessageBox.Show("Tỷ lệ thanh toán không nhỏ hơn 0% và không lớn hơn 100%");
                    return false;
                }

            }
            //▼===== 20200312 TTM: BM 0025010: Kiểm tra thông tin VAT cho danh mục
            if (ObjRefGenMedProductDetails_Current.VAT < 0 || ObjRefGenMedProductDetails_Current.VAT > 1)
            {
                MessageBox.Show(eHCMSResources.Z2991_G1_VATKhongHopLe);
                return false;
            }
            //▲===== 
            return true;
        }


        public bool CanbtSave
        {
            get
            {
                return !IsWaitingSave;
            }
        }

        public void btClear()
        {
            if (MessageBox.Show(eHCMSResources.A0173_G1_Msg_ConfClearAll, eHCMSResources.G2363_G1_XNhan, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                TitleForm = string.Format("{0} ", eHCMSResources.G0276_G1_ThemMoi) + TextType();
                InitializeNewItem(V_MedProductType);
            }
        }

        public void btSave()
        {
            if (CheckValid(ObjRefGenMedProductDetails_Current))
            {
                if (CheckData())
                {
                    IsWaitingSave = true;
                    RefGenMedProductDetails_Save(ObjRefGenMedProductDetails_Current);
                }
            }
        }
        //DPT 30/08/2017 Thêm Data Cho combobox Nhóm HIReportGroupCode
        private ObservableCollection<string> _HIGroupCodeCollection = new ObservableCollection<string>(new string[] { "10", "11" });
        public ObservableCollection<string> HIGroupCodeCollection
        {
            get { return _HIGroupCodeCollection; }
            set
            {
                _HIGroupCodeCollection = value;
                NotifyOfPropertyChange("HIGroupCodeCollection");
            }
        }
        //
        public bool CheckValid(object temp)
        {
            RefGenMedProductDetails u = temp as RefGenMedProductDetails;
            if (u == null)
            {
                return false;
            }
            string error = "";
            //KMx: Nếu cấu hình không tự tạo code mà người dùng không nhập code thì không cho lưu (21/08/2014 11:51).
            if (!Globals.ServerConfigSection.MedDeptElements.AutoCreateMedCode && string.IsNullOrEmpty(u.Code))
            {
                error += eHCMSResources.Z0852_G1_ChuaNhapMaCode + Environment.NewLine;
            }
            if (string.IsNullOrEmpty(u.BrandName))
            {
                error += eHCMSResources.Z2845_G1_ChuaNhapTenHang + Environment.NewLine;
            }
            if (u.CountryID <= 0)
            {
                error += eHCMSResources.Z0853_G1_ChuaChonQuocGia + Environment.NewLine;
            }
            if (u.SelectedUnit == null || u.SelectedUnit.UnitID <= 0)
            {
                error += eHCMSResources.Z2834_G1_ChuaChonDVT + Environment.NewLine;
            }
            if (u.SelectedUnitUse == null || u.SelectedUnitUse.UnitID <= 0)
            {
                error += eHCMSResources.Z2835_G1_ChuaChonDVD + Environment.NewLine;
            }
            if (u.DispenseVolume <= 0)
            {
                error += eHCMSResources.Z0856_G1_DispenseVolume + Environment.NewLine;
            }
            if (string.IsNullOrEmpty(u.Administration))
            {
                error += eHCMSResources.Z2836_G1_ChuaNhapCachDung + Environment.NewLine;
            }
            if (u.NumOfUse <= 0 && V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU) //20191211 TBL: Chỉ có y cụ mới kiểm tra trường này
            {
                error += string.Format("{0}. ", eHCMSResources.Z1046_G1_SoLanTaiSD) + Environment.NewLine;
            }
            if (u.PCOID == null || u.PCOID <= 0)
            {
                error += eHCMSResources.Z0854_G1_ChuaChonNSX + Environment.NewLine;
            }
            if (u.IsWatchOutQty)
            {
                if (u.RemainWarningLevel1 < 0 || u.RemainWarningLevel2 < 0)
                {
                    error += eHCMSResources.Z0858_G1_SLgCanhBaoLonHonBang0 + Environment.NewLine;
                }
                if (u.LimitedOutQty <= 0)
                {
                    error += eHCMSResources.Z0859_G1_SLgGioiHanLonHon0 + Environment.NewLine;
                }
                if (u.LimitedOutQty < u.RemainWarningLevel2 || u.LimitedOutQty < u.RemainWarningLevel1 || u.RemainWarningLevel2 < u.RemainWarningLevel1)
                {
                    error += eHCMSResources.Z0860_G1_I + Environment.NewLine;
                }
            }
            /*==== #001 ====*/
            if (u.InsuranceCover == true)
            {
                if (string.IsNullOrEmpty(u.Visa))
                {
                    error += eHCMSResources.Z2844_G1_ChuaNhapVisa + Environment.NewLine;
                }
                if ((u.V_MedProductType == 11002 && (u.SelectedDrugClass == null || string.IsNullOrEmpty(u.SelectedDrugClass.FaName))))
                {
                    error += eHCMSResources.Z0855_G1_ChuaChonNhomThuoc + Environment.NewLine;
                }
                if ((u.V_MedProductType != 11002 && string.IsNullOrEmpty(u.HICode)))
                {
                    error += eHCMSResources.Z2838_G1_ChuaNhapHICode + Environment.NewLine;
                }
            }
            /*==== #001 ====*/
            if (!u.IsNotVat && u.VAT == null)
            {
                error += eHCMSResources.Z2996_G1_VATRong + Environment.NewLine;
            }
            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error);
                return false;
            }
            //20190122 TBL: Do Validate nen khong the them NCC nen tam thoi bo ra
            //return u.Validate();
            return true;
        }


        public void btClose()
        {
            TryClose();
        }

        private string TextType()
        {
            string Result = "";
            switch (V_MedProductType)
            {
                case 11001:
                    {
                        Result = eHCMSResources.G0787_G1_Thuoc;
                        break;
                    }
                case 11002:
                    {
                        Result = eHCMSResources.G2907_G1_YCu;
                        break;
                    }
                case 11003:
                    {
                        Result = eHCMSResources.T1616_G1_HC;
                        break;
                    }
                case 11010:
                    {
                        Result = eHCMSResources.T3709_G1_Mau;
                        break;
                    }
            }
            return Result;
        }

        #region Supplier member

        public void SupplierGenMedProduct_LoadDrugIDNotPaging(long GenMedProductID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSupplierGenMedProduct_LoadDrugIDNotPaging(GenMedProductID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            List<RefMedicalServiceItem> ServiceList = null;
                            var results = contract.EndSupplierGenMedProduct_LoadDrugIDNotPaging(out ServiceList, asyncResult);
                            ObjRefGenMedProductDetails_Current.SupplierGenMedProducts = results.ToObservableCollection();
                            AddBlankRow();
                            ObjRefGenMedProductDetails_Current.RefMedicalServiceItems = ServiceList.ToObservableCollection();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }
        #region Properties reference member

        private ObservableCollection<DrugDeptSupplier> _Suppliers;
        public ObservableCollection<DrugDeptSupplier> Suppliers
        {
            get
            {
                return _Suppliers;
            }
            set
            {
                if (_Suppliers != value)
                {
                    _Suppliers = value;
                    NotifyOfPropertyChange(() => Suppliers);
                }
            }
        }

        private ObservableCollection<Hospital> _Hospitals;
        public ObservableCollection<Hospital> Hospitals
        {
            get
            {
                return _Hospitals;
            }
            set
            {
                if (_Hospitals != value)
                {
                    _Hospitals = value;
                    NotifyOfPropertyChange(() => Hospitals);
                }
            }
        }


        private ObservableCollection<RefGenericDrugCategory_1> _RefGenericDrugCategory_1s;
        public ObservableCollection<RefGenericDrugCategory_1> RefGenericDrugCategory_1s
        {
            get
            {
                return _RefGenericDrugCategory_1s;
            }
            set
            {
                if (_RefGenericDrugCategory_1s != value)
                {
                    _RefGenericDrugCategory_1s = value;
                    NotifyOfPropertyChange(() => RefGenericDrugCategory_1s);
                }
            }
        }

        private ObservableCollection<RefCountry> _AllCountries;
        public ObservableCollection<RefCountry> AllCountries
        {
            get
            {
                return _AllCountries;
            }
            set
            {
                if (_AllCountries != value)
                {
                    _AllCountries = value;
                    NotifyOfPropertyChange(() => AllCountries);
                }
            }
        }

        private ObservableCollection<RefCountry> _countries;
        public ObservableCollection<RefCountry> Countries
        {
            get
            {
                return _countries;
            }
            set
            {
                if (_countries != value)
                {
                    _countries = value;
                    NotifyOfPropertyChange(() => Countries);
                }
            }
        }

        private ObservableCollection<RefUnit> _units;
        public ObservableCollection<RefUnit> Units
        {
            get
            {
                return _units;
            }
            set
            {
                if (_units != value)
                {
                    _units = value;
                    NotifyOfPropertyChange(() => Units);
                }
            }
        }

        private ObservableCollection<DrugClass> _AllFamilytherapies;
        public ObservableCollection<DrugClass> AllFamilyTherapies
        {
            get
            {
                return _AllFamilytherapies;
            }
            set
            {
                if (_AllFamilytherapies != value)
                {
                    _AllFamilytherapies = value;
                    NotifyOfPropertyChange(() => AllFamilyTherapies);
                }
            }
        }

        private ObservableCollection<DrugClass> _familytherapies;
        public ObservableCollection<DrugClass> FamilyTherapies
        {
            get
            {
                return _familytherapies;
            }
            set
            {
                if (_familytherapies != value)
                {
                    _familytherapies = value;
                    NotifyOfPropertyChange(() => FamilyTherapies);
                }
            }
        }

        private ObservableCollection<DrugDeptPharmaceuticalCompany> _AllPharmaceuticalCompanies;
        public ObservableCollection<DrugDeptPharmaceuticalCompany> AllPharmaceuticalCompanies
        {
            get { return _AllPharmaceuticalCompanies; }
            set
            {
                if (_AllPharmaceuticalCompanies != value)
                    _AllPharmaceuticalCompanies = value;
                NotifyOfPropertyChange(() => AllPharmaceuticalCompanies);
            }
        }

        private ObservableCollection<DrugDeptPharmaceuticalCompany> _pharmaceuticalCompanies;
        public ObservableCollection<DrugDeptPharmaceuticalCompany> PharmaceuticalCompanies
        {
            get { return _pharmaceuticalCompanies; }
            set
            {
                if (_pharmaceuticalCompanies != value)
                    _pharmaceuticalCompanies = value;
                NotifyOfPropertyChange(() => PharmaceuticalCompanies);
            }
        }
        #endregion

        #region get reference member

        private IEnumerator<IResult> DoGetSupplierList()
        {
            var paymentTypeTask = new LoadDrugDeptSupplierListTask((int)AllLookupValues.SupplierType.CUNG_CAP_THIET_BI_Y_TE, false, false);
            yield return paymentTypeTask;
            Suppliers = paymentTypeTask.DrugDeptSupplierList;
            yield break;
        }

        //private long? CityProvinceID = 42;//tam thoi de TP HCM la vay di

        //private IEnumerator<IResult> DoGetHospitalList()
        //{
        //    var paymentTypeTask = new LoadHospitalListTask(CityProvinceID, true, false);
        //    yield return paymentTypeTask;
        //    Hospitals = paymentTypeTask.HospitalList;
        //    yield break;
        //}

        private IEnumerator<IResult> DoGetRefGenericDrugCategory_1List()
        {
            var paymentTypeTask = new LoadRefGenericDrugCategory_1ListTask(V_MedProductType, false, false);
            yield return paymentTypeTask;
            RefGenericDrugCategory_1s = paymentTypeTask.RefGenericDrugCategory_1List;
            yield break;
        }

        private IEnumerator<IResult> DoGetUnitList()
        {
            var paymentTypeTask = new LoadDrugDeptUnitListTask(false, false);
            yield return paymentTypeTask;
            Units = paymentTypeTask.RefUnitList;
            yield break;
        }

        private IEnumerator<IResult> DoGetDrugClassList()
        {
            var paymentTypeTask = new LoadDrugDeptClassListTask(V_MedProductType, false, false);
            yield return paymentTypeTask;
            AllFamilyTherapies = paymentTypeTask.DrugClassList;
            yield break;
        }

        private IEnumerator<IResult> DoCountryListList()
        {
            var paymentTypeTask = new LoadCountryListTask(false, false);
            yield return paymentTypeTask;
            AllCountries = paymentTypeTask.RefCountryList;
            yield break;
        }

        private IEnumerator<IResult> DoPharmaceuticalCompanyListList()
        {
            var paymentTypeTask = new LoadDrugDeptPharmaceuticalCompanyListTask(false, false);
            yield return paymentTypeTask;
            AllPharmaceuticalCompanies = paymentTypeTask.DrugDeptPharmaceuticalCompanyList;
            yield break;
        }

        #endregion

        private void AddBlankRow()
        {
            if (ObjRefGenMedProductDetails_Current.SupplierGenMedProducts == null)
            {
                ObjRefGenMedProductDetails_Current.SupplierGenMedProducts = new ObservableCollection<SupplierGenMedProduct>();
            }
            SupplierGenMedProduct item = new SupplierGenMedProduct();
            ObjRefGenMedProductDetails_Current.SupplierGenMedProducts.Add(item);
        }

        public void acbDrug_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as ComboBox).ItemsSource = Suppliers;
        }

        private bool CheckExists(SupplierGenMedProduct ite)
        {
            if (ite != null && ite.SelectedSupplier != null)
            {
                ite.SupplierID = ite.SelectedSupplier.SupplierID;
                var value = ObjRefGenMedProductDetails_Current.SupplierGenMedProducts.Where(x => x.SupplierID == ite.SupplierID);
                if (value != null && value.Count() > 1)
                {
                    MessageBox.Show(eHCMSResources.A0827_G1_Msg_InfoNCCDaChon);
                    ite.SelectedSupplier = null;
                    return false;
                }
            }
            else
            {
                //  MessageBox.Show(eHCMSResources.K0347_G1_ChonNCC);
                return false;
            }
            return true;
        }
        string value = "";
        DataGrid GridSuppliers = null;
        private SupplierGenMedProduct EditedDetailItem { get; set; }
        private string EditedColumnName { get; set; }
        public void GridSuppliers_Loaded(object sender, RoutedEventArgs e)
        {
            GridSuppliers = sender as DataGrid;
            if (GridSuppliers != null)
            {
                GridSuppliers.CurrentCellChanged += GridSuppliers_CurrentCellChanged;
            }
        }


        string PreparingCellForEdit = "";
        public void GridSuppliers_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            AxTextBox tbl = GridSuppliers.CurrentColumn.GetCellContent(GridSuppliers.SelectedItem) as AxTextBox;
            if (tbl != null)
            {
                PreparingCellForEdit = tbl.Text;
            }
        }

        //public void GridSuppliers_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        //{
        //    SupplierGenMedProduct item = e.Row.DataContext as SupplierGenMedProduct;

        //    //KMx: Không sử dụng index nữa, chuyển sang dùng column name, vì khi user đổi thứ tự column thì logic sẽ bị sai (22/01/2015 10:58).
        //    //if (e.Column.DisplayIndex == 1)//NCC

        //    //▼====== #005
        //    //if (e.Column.GetValue(FrameworkElement.NameProperty).ToString() == "colSupplierName")
        //    if (e.Column.Equals(GridSuppliers.GetColumnByName("colSupplierName")))
        //    {
        //        if (CheckExists(item))
        //        {
        //            if (e.Row.GetIndex() == (ObjRefGenMedProductDetails_Current.SupplierGenMedProducts.Count - 1) && e.EditAction == DataGridEditAction.Commit)
        //            {
        //                AddBlankRow();
        //            }
        //        }
        //    }
        //    //if (e.Column.DisplayIndex == 3)//NCC
        //    //else if (e.Column.GetValue(FrameworkElement.NameProperty).ToString() == "colUnitPrice")
        //    else if (e.Column.Equals(GridSuppliers.GetColumnByName("colUnitPrice")))
        //    {
        //        decimal value = 0;
        //        decimal.TryParse(PreparingCellForEdit, out value);

        //        if (value == item.UnitPrice)
        //        {
        //            return;
        //        }
        //        item.PackagePrice = item.UnitPrice * ObjRefGenMedProductDetails_Current.UnitPackaging.GetValueOrDefault(1);
        //    }
        //    //if (e.Column.DisplayIndex == 4)//NCC
        //    //else if (e.Column.GetValue(FrameworkElement.NameProperty).ToString() == "colPackagePrice")
        //    else if (e.Column.Equals(GridSuppliers.GetColumnByName("colPackagePrice")))
        //    {
        //        decimal value = 0;
        //        decimal.TryParse(PreparingCellForEdit, out value);
        //        if (value == item.PackagePrice)
        //        {
        //            return;
        //        }
        //        if (ObjRefGenMedProductDetails_Current.UnitPackaging.GetValueOrDefault() > 0)
        //        {
        //            item.UnitPrice = item.PackagePrice / ObjRefGenMedProductDetails_Current.UnitPackaging.GetValueOrDefault(1);
        //        }
        //    }
        //    //▲====== #005
        //}

        public void GridSuppliers_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            EditedDetailItem = e.Row.DataContext as SupplierGenMedProduct;
            EditedColumnName = null;
            if (e.Column.Equals(GridSuppliers.GetColumnByName("colSupplierName")))
            {
                if (CheckExists(EditedDetailItem))
                {
                    if (e.Row.GetIndex() == (ObjRefGenMedProductDetails_Current.SupplierGenMedProducts.Count - 1) && e.EditAction == DataGridEditAction.Commit)
                    {
                        AddBlankRow();
                    }
                }
            }
            else if (e.Column.Equals(GridSuppliers.GetColumnByName("colUnitPrice")))
            {
                EditedColumnName = "colUnitPrice";
            }
            else if (e.Column.Equals(GridSuppliers.GetColumnByName("colPackagePrice")))
            {
                EditedColumnName = "colPackagePrice";
            }
        }
        private void GridSuppliers_CurrentCellChanged(object sender, EventArgs e)
        {
            if (EditedDetailItem == null || string.IsNullOrEmpty(EditedColumnName))
            {
                return;
            }
            if (EditedColumnName.Equals("colUnitPrice"))
            {
                EditedColumnName = null;
                decimal ite = 0;
                Decimal.TryParse(value, out ite);
                SupplierGenMedProduct item = EditedDetailItem;
                if (item != null)
                {
                    if (ite == item.UnitPrice)
                    {
                        return;
                    }
                    item.PackagePrice = item.UnitPrice * ObjRefGenMedProductDetails_Current.UnitPackaging.GetValueOrDefault(1);
                }
            }
            else if (EditedColumnName.Equals("colPackagePrice"))
            {
                EditedColumnName = null;
                decimal ite = 0;
                Decimal.TryParse(value, out ite);
                SupplierGenMedProduct item = EditedDetailItem;
                if (item != null)
                {
                    if (ite == item.PackagePrice)
                    {
                        return;
                    }
                    if (ObjRefGenMedProductDetails_Current.UnitPackaging.GetValueOrDefault() > 0)
                    {
                        item.UnitPrice = item.PackagePrice / ObjRefGenMedProductDetails_Current.UnitPackaging.GetValueOrDefault(1);
                    }
                }
            }
        }
        string txt = "";
        public void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txt != (sender as TextBox).Text)
            {
                txt = (sender as TextBox).Text;
                if (!string.IsNullOrEmpty(txt))
                {
                    if (ObjRefGenMedProductDetails_Current != null && ObjRefGenMedProductDetails_Current.SupplierGenMedProducts != null)
                    {
                        int value = 0;
                        int.TryParse(txt, out value);

                        ObjRefGenMedProductDetails_Current.UnitPackaging = value;
                        foreach (SupplierGenMedProduct p in ObjRefGenMedProductDetails_Current.SupplierGenMedProducts)
                        {
                            p.PackagePrice = p.UnitPrice * ObjRefGenMedProductDetails_Current.UnitPackaging.GetValueOrDefault(1);
                        }
                    }
                }
            }

        }


        public void btnDeleteCC_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.A0119_G1_Msg_ConfXoaNCC, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (GridSuppliers != null && GridSuppliers.SelectedItem != null)
                {
                    ObjRefGenMedProductDetails_Current.SupplierGenMedProducts.Remove(GridSuppliers.SelectedItem as SupplierGenMedProduct);
                }
                if (ObjRefGenMedProductDetails_Current.SupplierGenMedProducts.Count == 0)
                {
                    AddBlankRow();
                }
            }
        }

        #endregion

        private SupplierGenMedProduct _CurrentSupplierGenMedProduct;
        public SupplierGenMedProduct CurrentSupplierGenMedProduct
        {
            get
            {
                return _CurrentSupplierGenMedProduct;
            }
            set
            {
                if (_CurrentSupplierGenMedProduct != value)
                {
                    _CurrentSupplierGenMedProduct = value;
                    NotifyOfPropertyChange(() => CurrentSupplierGenMedProduct);
                }
            }
        }

        public void Supplier_Click(object sender, RoutedEventArgs e)
        {
            void onInitDlg(ISupplierProduct proAlloc)
            {
                proAlloc.IsChildWindow = true;
            }
            GlobalsNAV.ShowDialog<ISupplierProduct>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());

        }

        #region IHandle<DrugDeptCloseSearchSupplierEvent> Members

        public void Handle(DrugDeptCloseSearchSupplierEvent message)
        {
            if (message != null && this.IsActive)
            {
                CurrentSupplierGenMedProduct.SelectedSupplier = message.SelectedSupplier as DrugDeptSupplier;
            }
        }

        #endregion

        public void BvApThau_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox bvApThauBox = sender as AutoCompleteBox;
            if (e.Parameter == null || e.Parameter.Length == 0)
            {
                Hospitals = null;
                bvApThauBox.PopulateComplete();
                return;
            }

            e.Cancel = true;
            string SelHosName = e.Parameter;

            ObservableCollection<Hospital> allHos = Globals.allHospitals;
            var qryRes = (from c in allHos
                          where VNConvertString.ConvertString(c.HosName).ToLower().Contains(VNConvertString.ConvertString(bvApThauBox.SearchText).ToLower())
                          select c);

            Hospitals = new ObservableCollection<Hospital>(qryRes);

            bvApThauBox.PopulateComplete();

        }

        public void ApGiaThau_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            AutoCompleteBox axApThau = sender as AutoCompleteBox;
            if (ObjRefGenMedProductDetails_Current != null)
            {
                if (axApThau != null && axApThau.SelectedItem != null)
                {
                    Hospital item = axApThau.SelectedItem as Hospital;
                    if (item != null && item.HosID > 0)
                    {
                        ObjRefGenMedProductDetails_Current.CurrentHospital = item;
                        ObjRefGenMedProductDetails_Current.HosID = item.HosID;
                    }
                    else
                    {
                        ObjRefGenMedProductDetails_Current.CurrentHospital = null;
                        ObjRefGenMedProductDetails_Current.HosID = null;
                    }
                }
                else
                {
                    ObjRefGenMedProductDetails_Current.CurrentHospital = null;
                    ObjRefGenMedProductDetails_Current.HosID = null;
                }
            }
        }

        public void Country_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            AutoCompleteBox axCountry = sender as AutoCompleteBox;
            if (ObjRefGenMedProductDetails_Current != null)
            {
                if (axCountry != null && axCountry.SelectedItem != null)
                {
                    RefCountry item = axCountry.SelectedItem as RefCountry;
                    if (item != null && item.CountryID > 0)
                    {
                        ObjRefGenMedProductDetails_Current.SelectedCountry = item;
                        ObjRefGenMedProductDetails_Current.CountryID = item.CountryID;
                    }
                    else
                    {
                        ObjRefGenMedProductDetails_Current.SelectedCountry = null;
                        ObjRefGenMedProductDetails_Current.CountryID = 0;
                    }
                }
                else
                {
                    ObjRefGenMedProductDetails_Current.SelectedCountry = null;
                    ObjRefGenMedProductDetails_Current.CountryID = 0;
                }
            }

        }

        public void HangSX_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            AutoCompleteBox axCountry = sender as AutoCompleteBox;
            if (ObjRefGenMedProductDetails_Current != null)
            {
                if (axCountry != null && axCountry.SelectedItem != null)
                {
                    DrugDeptPharmaceuticalCompany item = axCountry.SelectedItem as DrugDeptPharmaceuticalCompany;
                    if (item != null && item.PCOID > 0)
                    {
                        ObjRefGenMedProductDetails_Current.PharmaceuticalCompany = item;
                        ObjRefGenMedProductDetails_Current.PCOID = item.PCOID;
                    }
                    else
                    {
                        ObjRefGenMedProductDetails_Current.PharmaceuticalCompany = null;
                        ObjRefGenMedProductDetails_Current.PCOID = 0;
                    }
                }
                else
                {
                    ObjRefGenMedProductDetails_Current.PharmaceuticalCompany = null;
                    ObjRefGenMedProductDetails_Current.PCOID = 0;
                }
            }
        }

        public void NhomHang_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            AutoCompleteBox axCountry = sender as AutoCompleteBox;
            if (ObjRefGenMedProductDetails_Current != null)
            {
                if (axCountry != null && axCountry.SelectedItem != null)
                {
                    DrugClass item = axCountry.SelectedItem as DrugClass;
                    if (item != null && item.DrugClassID > 0)
                    {
                        ObjRefGenMedProductDetails_Current.SelectedDrugClass = item;
                        ObjRefGenMedProductDetails_Current.DrugClassID = item.DrugClassID;
                    }
                    else
                    {
                        ObjRefGenMedProductDetails_Current.SelectedDrugClass = null;
                        ObjRefGenMedProductDetails_Current.DrugClassID = 0;
                    }
                }
                else
                {
                    ObjRefGenMedProductDetails_Current.SelectedDrugClass = null;
                    ObjRefGenMedProductDetails_Current.DrugClassID = 0;
                }
            }
        }

        #region RadioButton Vật tư y tế

        private bool _IsShowMedicalMaterial;
        public bool IsShowMedicalMaterial
        {
            get
            {
                return _IsShowMedicalMaterial;
            }
            set
            {
                _IsShowMedicalMaterial = value;
                NotifyOfPropertyChange(() => IsShowMedicalMaterial);
            }
        }

        private bool _IsReplaceMedicalMaterial = true;
        public bool IsReplaceMedicalMaterial
        {
            get
            {
                return _IsReplaceMedicalMaterial;
            }
            set
            {
                _IsReplaceMedicalMaterial = value;
                if (_IsReplaceMedicalMaterial)
                {
                    IsDisposeMedicalMaterial = !_IsReplaceMedicalMaterial;
                    IsNonMedicalMaterial = !_IsReplaceMedicalMaterial;
                }
                NotifyOfPropertyChange(() => IsReplaceMedicalMaterial);
            }
        }

        private bool _IsDisposeMedicalMaterial;
        public bool IsDisposeMedicalMaterial
        {
            get
            {
                return _IsDisposeMedicalMaterial;
            }
            set
            {
                _IsDisposeMedicalMaterial = value;
                if (_IsDisposeMedicalMaterial)
                {
                    IsReplaceMedicalMaterial = !_IsDisposeMedicalMaterial;
                    IsNonMedicalMaterial = !_IsDisposeMedicalMaterial;
                }
                NotifyOfPropertyChange(() => IsDisposeMedicalMaterial);
            }
        }

        private bool _IsNonMedicalMaterial;
        public bool IsNonMedicalMaterial
        {
            get
            {
                return _IsNonMedicalMaterial;
            }
            set
            {
                _IsNonMedicalMaterial = value;
                if (_IsNonMedicalMaterial)
                {
                    IsReplaceMedicalMaterial = !_IsNonMedicalMaterial;
                    IsDisposeMedicalMaterial = !_IsNonMedicalMaterial;
                }
                NotifyOfPropertyChange(() => IsNonMedicalMaterial);
            }
        }

        public void RdbMedicalMaterial_Click(object sender, RoutedEventArgs e)
        {
            if (ObjRefGenMedProductDetails_Current == null)
            {
                return;
            }
            if (IsReplaceMedicalMaterial)
            {
                ObjRefGenMedProductDetails_Current.V_MedicalMaterial = (long)AllLookupValues.V_MedicalMaterial.VTYT_THAYTHE;
            }
            else if (IsDisposeMedicalMaterial)
            {
                ObjRefGenMedProductDetails_Current.V_MedicalMaterial = (long)AllLookupValues.V_MedicalMaterial.VTYT_TIEUHAO;
            }
            else
            {
                ObjRefGenMedProductDetails_Current.V_MedicalMaterial = 0;
            }
        }

        public void SetRadioButton()
        {
            if (ObjRefGenMedProductDetails_Current == null)
            {
                return;
            }
            if (ObjRefGenMedProductDetails_Current.V_MedicalMaterial == (long)AllLookupValues.V_MedicalMaterial.VTYT_THAYTHE)
            {
                IsReplaceMedicalMaterial = true;
            }
            else if (ObjRefGenMedProductDetails_Current.V_MedicalMaterial == (long)AllLookupValues.V_MedicalMaterial.VTYT_TIEUHAO)
            {
                IsDisposeMedicalMaterial = true;
            }
            else
            {
                IsNonMedicalMaterial = true;
            }
        }

        #endregion

        #region Giới hạn số lượng xuất
        public void txtLimitedOutQty_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ObjRefGenMedProductDetails_Current == null)
            {
                return;
            }

            string txt = "";
            txt = (sender as TextBox).Text;
            if (!string.IsNullOrEmpty(txt))
            {
                int result = Globals.ConvertStringToInt(txt);
                ObjRefGenMedProductDetails_Current.LimitedOutQty = result > 0 ? result : 0;
            }
        }


        public void txtRemainWarningLevel1_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ObjRefGenMedProductDetails_Current == null)
            {
                return;
            }

            string txt = "";
            txt = (sender as TextBox).Text;
            if (!string.IsNullOrEmpty(txt))
            {
                int result = Globals.ConvertStringToInt(txt);
                ObjRefGenMedProductDetails_Current.RemainWarningLevel1 = result > 0 ? result : 0;
            }
        }


        public void txtRemainWarningLevel2_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ObjRefGenMedProductDetails_Current == null)
            {
                return;
            }

            string txt = "";
            txt = (sender as TextBox).Text;
            if (!string.IsNullOrEmpty(txt))
            {
                int result = Globals.ConvertStringToInt(txt);
                ObjRefGenMedProductDetails_Current.RemainWarningLevel2 = result > 0 ? result : 0;
            }
        }



        CheckBox chkWatchOutQty;
        public void chkWatchOutQty_Loaded(object sender, RoutedEventArgs e)
        {
            chkWatchOutQty = sender as CheckBox;
        }

        public void chkWatchOutQty_UnCheck(object sender, RoutedEventArgs e)
        {
            if (chkWatchOutQty == null || ObjRefGenMedProductDetails_Current == null || (string.IsNullOrWhiteSpace(ObjRefGenMedProductDetails_Current.LimitedOutQty.ToString()) && string.IsNullOrWhiteSpace(ObjRefGenMedProductDetails_Current.RemainWarningLevel1.ToString()) && string.IsNullOrWhiteSpace(ObjRefGenMedProductDetails_Current.RemainWarningLevel1.ToString())))
            {
                return;
            }

            if (MessageBox.Show(eHCMSResources.A0430_G1_Msg_InfoTuDongXoaSLgGHan_SLgCBao, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                ObjRefGenMedProductDetails_Current.LimitedOutQty = 0;
                ObjRefGenMedProductDetails_Current.RemainWarningLevel1 = 0;
                ObjRefGenMedProductDetails_Current.RemainWarningLevel2 = 0;
            }
            else
            {
                chkWatchOutQty.IsChecked = true;
            }
        }

        #endregion

        /*▼====: #002*/
        public void btnBidName()
        {
            if (MessageBox.Show(eHCMSResources.Z2110_G1_XoaMatHangKhoiDotThau, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                this.ShowBusyIndicator();
                var t = new Thread(() =>
                {
                    using (var mServiceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                    {
                        var contract = mServiceFactory.ServiceInstance;
                        try
                        {
                            contract.BeginRemoveBidDetails(ObjRefGenMedProductDetails_Current.BidID, ObjRefGenMedProductDetails_Current.GenMedProductID, true, Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    if (contract.EndRemoveBidDetails(asyncResult))
                                    {
                                        ObjRefGenMedProductDetails_Current.BidName = null;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                }
                            }), null);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                            this.HideBusyIndicator();
                        }
                    }
                });
                t.Start();
            }
        }
        /*▲====: #002*/
        /*▼====: #003*/
        private ObservableCollection<Lookup> _ProductScopeCollection;
        public ObservableCollection<Lookup> ProductScopeCollection
        {
            get
            {
                return _ProductScopeCollection;
            }
            set
            {
                _ProductScopeCollection = value;
                NotifyOfPropertyChange(() => ProductScopeCollection);
            }
        }
        private ObservableCollection<HITransactionType> _HITransactionTypeCollection;
        public ObservableCollection<HITransactionType> HITransactionTypeCollection
        {
            get
            {
                return _HITransactionTypeCollection;
            }
            set
            {
                if (_HITransactionTypeCollection == value)
                    return;
                _HITransactionTypeCollection = value;
                NotifyOfPropertyChange(() => HITransactionTypeCollection);
            }
        }
        private void LoadHITransactions()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginHITransactionType_GetListNoParentID(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                HITransactionTypeCollection = new ObservableCollection<HITransactionType>(contract.EndHITransactionType_GetListNoParentID(asyncResult));
                                if (HITransactionTypeCollection != null && V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                                    HITransactionTypeCollection = HITransactionTypeCollection.Where(x => x.IsShowOnDrugConfig).ToObservableCollection();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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
                    MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        /*▲====: #003*/

        /*▼====: #004*/
        private RefMedicalServiceType _medServiceType;
        public RefMedicalServiceType MedServiceType
        {
            get { return _medServiceType; }
            set
            {
                _medServiceType = value;
                NotifyOfPropertyChange(() => MedServiceType);
            }
        }
        private ObservableCollection<RefMedicalServiceType> _serviceTypes;
        public ObservableCollection<RefMedicalServiceType> ServiceTypes
        {
            get { return _serviceTypes; }
            set
            {
                _serviceTypes = value;
                NotifyOfPropertyChange(() => ServiceTypes);
            }
        }
        private RefMedicalServiceItem _medServiceItem;
        public RefMedicalServiceItem MedServiceItem
        {
            get { return _medServiceItem; }
            set
            {
                _medServiceItem = value;
                NotifyOfPropertyChange(() => MedServiceItem);
            }
        }
        private ObservableCollection<RefMedicalServiceItem> _medicalServiceItems;
        public ObservableCollection<RefMedicalServiceItem> MedicalServiceItems
        {
            get { return _medicalServiceItems; }
            set
            {
                _medicalServiceItems = value;
                NotifyOfPropertyChange(() => MedicalServiceItems);
            }
        }
        public void AxComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MedServiceItem = new RefMedicalServiceItem();
            GetAllMedicalServiceItemsByType(MedServiceType);
        }
        public void cboSelectedService_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Globals.EventAggregator.Publish(new ItemSelected<RefMedicalServiceItem>() { Item = MedServiceItem });
        }
        public void GetServiceTypes()
        {
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0621_G1_DangLayDSLoaiDV)
                });
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        List<long> outTypes = new List<long>();
                        outTypes.Add((long)AllLookupValues.V_RefMedicalServiceInOutOthers.NOITRU);
                        outTypes.Add((long)AllLookupValues.V_RefMedicalServiceInOutOthers.NOITRU_NGOAITRU);
                        contract.BeginGetMedicalServiceTypesByInOutType(outTypes,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var allItems = contract.EndGetMedicalServiceTypesByInOutType(asyncResult);
                                    if (_serviceTypes == null)
                                    {
                                        _serviceTypes = new ObservableCollection<RefMedicalServiceType>();
                                    }
                                    else
                                    {
                                        _serviceTypes.Clear();
                                    }
                                    _serviceTypes.Add(new RefMedicalServiceType() { MedicalServiceTypeID = -1, MedicalServiceTypeName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K1916_G1_Chon) });
                                    foreach (RefMedicalServiceType info in allItems)
                                    {
                                        _serviceTypes.Add(info);
                                    }
                                    NotifyOfPropertyChange(() => ServiceTypes);
                                }
                                catch (Exception innerEx)
                                {
                                    error = new AxErrorEventArgs(innerEx);
                                }
                                finally
                                {
                                    Globals.IsBusy = false;
                                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
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
                finally
                {
                    Globals.IsBusy = false;
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                }
            });
            t.Start();
        }
        public void GetAllMedicalServiceItemsByType(RefMedicalServiceType serviceType)
        {
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0992_G1_DangLayDSCacDV)
                });
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        long? serviceTypeID = null;
                        if (serviceType != null)
                        {
                            serviceTypeID = serviceType.MedicalServiceTypeID;
                        }

                        contract.BeginGetAllMedicalServiceItemsByType(serviceTypeID, null,null,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<RefMedicalServiceItem> allItem = new ObservableCollection<RefMedicalServiceItem>();
                                try
                                {
                                    allItem = contract.EndGetAllMedicalServiceItemsByType(asyncResult);
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                }
                                if (allItem == null)
                                {
                                    MedicalServiceItems = null;
                                }
                                else
                                {
                                    var sType = (RefMedicalServiceType)asyncResult.AsyncState;
                                    var col = new ObservableCollection<RefMedicalServiceItem>();
                                    foreach (var item in allItem)
                                    {
                                        item.RefMedicalServiceType = sType;
                                        col.Add(item);
                                    }
                                    MedicalServiceItems = col;
                                }
                            }), serviceType);
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
                    Globals.IsBusy = false;
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                }
            });
            t.Start();
        }
        private RefMedicalServiceItem _SelectedMed;
        public RefMedicalServiceItem SelectedMed
        {
            get { return _SelectedMed; }
            set
            {
                if (_SelectedMed != value)
                    _SelectedMed = value;
                NotifyOfPropertyChange(() => SelectedMed);
            }
        }
        private void cboServiceStyle(object sender, SelectionChangedEventArgs e)
        {
            MedServiceItem = new RefMedicalServiceItem();
            GetAllMedicalServiceItemsByType(MedServiceType);
            Globals.EventAggregator.Publish(new ItemSelected<RefMedicalServiceItem>() { Item = MedServiceItem });
        }
        public void btnAddService()
        {
            if (SelectedMed == null || ObjRefGenMedProductDetails_Current == null)
                return;
            if (ObjRefGenMedProductDetails_Current.RefMedicalServiceItems == null)
                ObjRefGenMedProductDetails_Current.RefMedicalServiceItems = new ObservableCollection<RefMedicalServiceItem>();
            foreach (RefMedicalServiceItem ServiceItem in ObjRefGenMedProductDetails_Current.RefMedicalServiceItems)
            {
                if (ServiceItem.MedServiceID == SelectedMed.MedServiceID)
                {
                    MessageBox.Show(eHCMSResources.A0510_G1_Msg_InfoDaTonTaiDV);
                    return;
                }
            }
            ObjRefGenMedProductDetails_Current.RefMedicalServiceItems.Add(SelectedMed);
        }
        DataGrid GridService = null;
        public void GridService_Loaded(object sender, RoutedEventArgs e)
        {
            GridService = sender as DataGrid;
        }
        public void btnDeleteService_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.Z1183_G1_CoMuonXoaDVNayKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (GridService != null && GridService.SelectedItem != null)
                {
                    ObjRefGenMedProductDetails_Current.RefMedicalServiceItems.Remove(GridService.SelectedItem as RefMedicalServiceItem);
                }
                if (ObjRefGenMedProductDetails_Current.RefMedicalServiceItems.Count == 0)
                {
                    btnAddService();
                }
            }
        }
        /*▲====: #004*/
        public void ckbApplyCer04_CheckedChanged(object sender, RoutedEventArgs e)
        {
            CheckBox CurrentCheckbox = sender as CheckBox;
            if (CurrentCheckbox.IsChecked.GetValueOrDefault(false))
            {
                ObjRefGenMedProductDetails_Current.CeilingPrice2ndItem = ObjRefGenMedProductDetails_Current.CeilingPrice2ndItem == null ? Globals.ServerConfigSection.HealthInsurances.MaxHIPaidOnMoreAddedItem : ObjRefGenMedProductDetails_Current.CeilingPrice2ndItem;
            }
            else
            {
                ObjRefGenMedProductDetails_Current.CeilingPrice2ndItem = null;
            }
        }
        /*▼====: #007*/
        //public void btnDrugClass()
        //{
        //    IDrugDeptClass DialogView = Globals.GetViewModel<IDrugDeptClass>();
        //    DialogView.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
        //    DialogView.GetSearchTreeView((long)AllLookupValues.MedProductType.Y_CU);
        //    DialogView.IsDoubleClick = true;
        //    GlobalsNAV.ShowDialog_V3(DialogView, null, null, false, true, new Size(600, 700));
        //}

        public void btnDrugClass()
        {
            Action<IDrugDeptSelectClass> onInitDlg = delegate (IDrugDeptSelectClass theVm)
            {
                theVm.V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
                theVm.GetSearchTreeView((long)AllLookupValues.MedProductType.Y_CU);
                theVm.IsDoubleClick = true;
            };
            GlobalsNAV.ShowDialog<IDrugDeptSelectClass>(onInitDlg);
        }

        public void Handle(ChooseDrugClass message)
        {
            if (message != null && message.ChildrenSelected != null && message.ParentSelected != null && this.IsActive)
            {
                if (ObjRefGenMedProductDetails_Current.SelectedDrugClass == null)
                {
                    ObjRefGenMedProductDetails_Current.SelectedDrugClass = new DrugClass();
                }
                ObjRefGenMedProductDetails_Current.SelectedDrugClass.FaName = message.ParentSelected.NodeText;
                ObjRefGenMedProductDetails_Current.DrugClassName = message.ParentSelected.NodeText;
                ObjRefGenMedProductDetails_Current.SelectedDrugClass.DrugClassID = message.ParentSelected.NodeID;
                ObjRefGenMedProductDetails_Current.DrugClassID = message.ParentSelected.NodeID;
                ObjRefGenMedProductDetails_Current.HICode = message.ChildrenSelected.NodeText;
            }
        }
        /*▲====: #007*/
        public void txt_VAT_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ObjRefGenMedProductDetails_Current == null)
            {
                return;
            }

            string txt = "";
            txt = (sender as TextBox).Text;
            if (!string.IsNullOrEmpty(txt))
            {
                double value = 0;
                double.TryParse(txt, out value);
                ObjRefGenMedProductDetails_Current.VAT = value > 0 ? value : 0;
            }
            else
            {
                ObjRefGenMedProductDetails_Current.VAT = null;
            }
        }
    }
}
