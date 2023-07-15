using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using System.Linq;
using System.Collections.Generic;
using aEMR.Common;
using aEMR.Common.Printing;
using aEMR.CommonTasks;
using aEMR.Controls;
using eHCMSLanguage;
using Castle.Windsor;
/*
 * 20200903 #001 TNHX [BM]: Cho phép xuất thuốc nhà thuốc tìm thuốc bằng tên hoạt chất.
 */
namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IEditOutwardInternal)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class EditOutwardInternalViewModel : Conductor<object>, IEditOutwardInternal
         , IHandle<PharmacyCloseSearchRequestEvent>
        , IHandle<EditChooseBatchNumberVisitorEvent>, IHandle<EditChooseBatchNumberVisitorResetQtyEvent>
    {
        public string TitleForm { get; set; }

        private enum DataGridCol
        {
            ColMultiDelete = 0,
            ColDelete = 1,
            MaThuoc = 2,
            TenThuoc = 3,
            HamLuong = 4,
            DVT = 5,
            LoSX = 6,
            SLYC = 7,
            ThucXuat = 8,
            DonGia = 9,
            ThanhTien = 10
        }

        [ImportingConstructor]
        public EditOutwardInternalViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Authorization();

            Globals.EventAggregator.Subscribe(this);

            Coroutine.BeginExecute(DoGetStore_All());
            Coroutine.BeginExecute(DoGetStore_EXTERNAL());
            Coroutine.BeginExecute(DoGetByOutPriceLookups());
            Coroutine.BeginExecute(DoRefOutputType_All());

            SearchCriteria = new SearchOutwardInfo
            {
                TypID = (long)AllLookupValues.RefOutputType.XUATNOIBO
            };

            RefeshData();
            SetDefaultForStore();

            GetAllStaffContrain();
            GetHospital_IsFriends();
        }

        private Staff GetStaffLogin()
        {
            StaffName = Globals.LoggedUserAccount.Staff.FullName;
            return Globals.LoggedUserAccount.Staff;
        }

        private void RefeshData()
        {
            SelectedOutInvoice = null;
            SelectedOutInvoice = new OutwardDrugInvoice
            {
                OutDate = Globals.ServerDate.Value,
                TypID = (long)AllLookupValues.RefOutputType.BANLE,
                OutwardDrugs = new ObservableCollection<OutwardDrug>()
            };

            GetDrugForSellVisitorList = null;
            GetDrugForSellVisitorList = new ObservableCollection<GetDrugForSellVisitor>();

            GetDrugForSellVisitorListSum = null;
            GetDrugForSellVisitorListSum = new ObservableCollection<GetDrugForSellVisitor>();

            GetDrugForSellVisitorTemp = null;
            GetDrugForSellVisitorTemp = new ObservableCollection<GetDrugForSellVisitor>();

            ListOutwardDrugFirst = null;
            ListOutwardDrugFirst = new ObservableCollection<OutwardDrug>();

            ListOutwardDrugFirstCopy = null;
            ListOutwardDrugFirstCopy = new ObservableCollection<OutwardDrug>();

            BrandName = "";
        }

        private void SetDefaultForStore()
        {
            if (StoreCbx != null)
            {
                if (StoreID == 0)
                {
                    StoreID = StoreCbx.FirstOrDefault().StoreID;
                }
                if (SearchCriteria != null)
                {
                    SearchCriteria.StoreID = StoreID;
                }
            }
        }

        public void Authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

            mXuatNoiBo_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mXuatNoiBo,
                                               (int)oPharmacyEx.mXuatNoiBo_Tim, (int)ePermission.mView);
            mXuatNoiBo_ThongTinPhieuXuat = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mXuatNoiBo,
                                               (int)oPharmacyEx.mXuatNoiBo_ThongTinPhieuXuat, (int)ePermission.mView);
            mXuatNoiBo_Them = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mXuatNoiBo,
                                               (int)oPharmacyEx.mXuatNoiBo_Them, (int)ePermission.mView);
            mXuatNoiBo_ThucHien = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mXuatNoiBo,
                                               (int)oPharmacyEx.mXuatNoiBo_ThucHien, (int)ePermission.mView);
            mXuatNoiBo_ThuTien = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mXuatNoiBo,
                                               (int)oPharmacyEx.mXuatNoiBo_ThuTien, (int)ePermission.mView);
            mXuatNoiBo_ReportIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mXuatNoiBo,
                                               (int)oPharmacyEx.mXuatNoiBo_ReportIn, (int)ePermission.mView);

        }

        #region checking account

        private bool _mXuatNoiBo_Tim = true;
        private bool _mXuatNoiBo_ThongTinPhieuXuat = true;
        private bool _mXuatNoiBo_Them = true;
        private bool _mXuatNoiBo_ThucHien = true;
        private bool _mXuatNoiBo_ThuTien = true;
        private bool _mXuatNoiBo_ReportIn = true;

        public bool mXuatNoiBo_Tim
        {
            get
            {
                return _mXuatNoiBo_Tim;
            }
            set
            {
                if (_mXuatNoiBo_Tim == value)
                    return;
                _mXuatNoiBo_Tim = value;
            }
        }
        public bool mXuatNoiBo_ThongTinPhieuXuat
        {
            get
            {
                return _mXuatNoiBo_ThongTinPhieuXuat;
            }
            set
            {
                if (_mXuatNoiBo_ThongTinPhieuXuat == value)
                    return;
                _mXuatNoiBo_ThongTinPhieuXuat = value;
            }
        }
        public bool mXuatNoiBo_Them
        {
            get
            {
                return _mXuatNoiBo_Them;
            }
            set
            {
                if (_mXuatNoiBo_Them == value)
                    return;
                _mXuatNoiBo_Them = value;
            }
        }
        public bool mXuatNoiBo_ThucHien
        {
            get
            {
                return _mXuatNoiBo_ThucHien;
            }
            set
            {
                if (_mXuatNoiBo_ThucHien == value)
                    return;
                _mXuatNoiBo_ThucHien = value;
            }
        }
        public bool mXuatNoiBo_ThuTien
        {
            get
            {
                return _mXuatNoiBo_ThuTien;
            }
            set
            {
                if (_mXuatNoiBo_ThuTien == value)
                    return;
                _mXuatNoiBo_ThuTien = value;
            }
        }
        public bool mXuatNoiBo_ReportIn
        {
            get
            {
                return _mXuatNoiBo_ReportIn;
            }
            set
            {
                if (_mXuatNoiBo_ReportIn == value)
                    return;
                _mXuatNoiBo_ReportIn = value;
            }
        }

        #endregion

        #region Properties Member

        private ObservableCollection<RefOutputType> _RefOutputTypeList;
        public ObservableCollection<RefOutputType> RefOutputTypeList
        {
            get { return _RefOutputTypeList; }
            set
            {
                _RefOutputTypeList = value;
                NotifyOfPropertyChange(() => RefOutputTypeList);
            }
        }

        private bool? IsCost = true;

        public bool IsInternalForStore
        {
            get
            {
                return _IsInternalForStore;
            }
            set
            {
                _IsInternalForStore = value;
                if (_IsInternalForStore)
                {
                    IsInternalForDoctor = false;
                    IsInternalForHospital = false;
                }
                NotifyOfPropertyChange(() => IsInternalForStore);
            }
        }
        private bool _IsInternalForStore = true;

        public bool IsInternalForDoctor
        {
            get
            {
                return _IsInternalForDoctor;
            }
            set
            {
                _IsInternalForDoctor = value;
                if (_IsInternalForDoctor)
                {
                    IsInternalForStore = false;
                    IsInternalForHospital = false;
                }
                NotifyOfPropertyChange(() => IsInternalForDoctor);
            }
        }
        private bool _IsInternalForDoctor = false;

        public bool IsInternalForHospital
        {
            get
            {
                return _IsInternalForHospital;
            }
            set
            {
                _IsInternalForHospital = value;
                if (_IsInternalForHospital)
                {
                    IsInternalForStore = false;
                    IsInternalForDoctor = false;
                }
                NotifyOfPropertyChange(() => IsInternalForHospital);
            }
        }
        private bool _IsInternalForHospital = false;

        private ObservableCollection<Hospital> _Hospitals;
        public ObservableCollection<Hospital> Hospitals
        {
            get { return _Hospitals; }
            set
            {
                if (_Hospitals != value)
                {
                    _Hospitals = value;
                    NotifyOfPropertyChange(() => Hospitals);
                }
            }
        }

        private ObservableCollection<Staff> _ListStaff;
        public ObservableCollection<Staff> ListStaff
        {
            get { return _ListStaff; }
            set
            {
                if (_ListStaff != value)
                {
                    _ListStaff = value;
                    NotifyOfPropertyChange(() => ListStaff);
                }
            }
        }

        private ObservableCollection<RefStorageWarehouseLocation> _FromStoreCbx;
        public ObservableCollection<RefStorageWarehouseLocation> FromStoreCbx
        {
            get
            {
                return _FromStoreCbx;
            }
            set
            {
                if (_FromStoreCbx != value)
                {
                    _FromStoreCbx = value;
                    NotifyOfPropertyChange(() => FromStoreCbx);
                }
            }
        }

        private ObservableCollection<RefStorageWarehouseLocation> _AllStoreCbx;
        public ObservableCollection<RefStorageWarehouseLocation> AllStoreCbx
        {
            get
            {
                return _AllStoreCbx;
            }
            set
            {
                if (_AllStoreCbx != value)
                {
                    _AllStoreCbx = value;
                    NotifyOfPropertyChange(() => AllStoreCbx);
                }
            }
        }

        private ObservableCollection<Lookup> _ByOutPriceLookups;
        public ObservableCollection<Lookup> ByOutPriceLookups
        {
            get
            {
                return _ByOutPriceLookups;
            }
            set
            {
                if (_ByOutPriceLookups != value)
                {
                    _ByOutPriceLookups = value;
                    NotifyOfPropertyChange(() => ByOutPriceLookups);
                }
            }
        }

        private SearchOutwardInfo _searchCriteria;
        public SearchOutwardInfo SearchCriteria
        {
            get { return _searchCriteria; }
            set
            {
                if (_searchCriteria != value)
                    _searchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }

        private decimal _SumTotalPrice;
        public decimal SumTotalPrice
        {
            get { return _SumTotalPrice; }
            set
            {
                if (_SumTotalPrice != value)
                    _SumTotalPrice = value;
                NotifyOfPropertyChange(() => SumTotalPrice);
            }
        }

        public decimal _SumTotalPriceFirst;
        public decimal SumTotalPriceFirst
        {
            get { return _SumTotalPriceFirst; }
            set
            {
                if (_SumTotalPriceFirst != value)
                    _SumTotalPriceFirst = value;
                NotifyOfPropertyChange(() => SumTotalPriceFirst);
            }
        }

        public long _IDFirst;
        public long IDFirst
        {
            get { return _IDFirst; }
            set
            {
                if (_IDFirst != value)
                    _IDFirst = value;
                NotifyOfPropertyChange(() => IDFirst);
            }
        }

        private ObservableCollection<OutwardDrug> _ListOutwardDrugFirst;
        public ObservableCollection<OutwardDrug> ListOutwardDrugFirst
        {
            get { return _ListOutwardDrugFirst; }
            set
            {
                if (_ListOutwardDrugFirst != value)
                    _ListOutwardDrugFirst = value;
                NotifyOfPropertyChange(() => ListOutwardDrugFirst);
            }
        }

        private ObservableCollection<OutwardDrug> _ListOutwardDrugFirstCopy;
        public ObservableCollection<OutwardDrug> ListOutwardDrugFirstCopy
        {
            get { return _ListOutwardDrugFirstCopy; }
            set
            {
                if (_ListOutwardDrugFirstCopy != value)
                    _ListOutwardDrugFirstCopy = value;
                NotifyOfPropertyChange(() => ListOutwardDrugFirstCopy);
            }
        }

        private ObservableCollection<OutwardDrug> _OutwardDrugListCopy;
        public ObservableCollection<OutwardDrug> OutwardDrugListCopy
        {
            get { return _OutwardDrugListCopy; }
            set
            {
                if (_OutwardDrugListCopy != value)
                    _OutwardDrugListCopy = value;
                NotifyOfPropertyChange(() => OutwardDrugListCopy);
            }
        }

        private ObservableCollection<RefStorageWarehouseLocation> _StoreCbx;
        public ObservableCollection<RefStorageWarehouseLocation> StoreCbx
        {
            get
            {
                return _StoreCbx;
            }
            set
            {
                if (_StoreCbx != value)
                {
                    _StoreCbx = value;
                    NotifyOfPropertyChange(() => StoreCbx);
                }
            }
        }

        private long _StoreID;
        public long StoreID
        {
            get { return _StoreID; }
            set
            {
                if (_StoreID != value)
                {
                    _StoreID = value;
                    NotifyOfPropertyChange(() => StoreID);
                }
            }
        }

        private OutwardDrugInvoice _SelectedOutInvoice;
        public OutwardDrugInvoice SelectedOutInvoice
        {
            get
            {
                return _SelectedOutInvoice;
            }
            set
            {
                if (_SelectedOutInvoice != value)
                {
                    _SelectedOutInvoice = value;
                    SumTotalPriceOutward();
                    NotifyOfPropertyChange("SelectedOutInvoice");
                }
            }
        }

        private OutwardDrugInvoice _SelectedOutInvoiceCoppy;
        public OutwardDrugInvoice SelectedOutInvoiceCoppy
        {
            get
            {
                return _SelectedOutInvoiceCoppy;
            }
            set
            {
                if (_SelectedOutInvoiceCoppy != value)
                {
                    _SelectedOutInvoiceCoppy = value;
                    NotifyOfPropertyChange("SelectedOutInvoiceCoppy");
                }
            }
        }

        private string _StaffName;
        public string StaffName
        {
            get { return _StaffName; }
            set
            {
                _StaffName = value;
                NotifyOfPropertyChange(() => StaffName);
            }
        }

        private bool _IsOther = false;
        public bool IsOther
        {
            get
            {
                return _IsOther;
            }
            set
            {
                _IsOther = value;
                NotifyOfPropertyChange(() => IsOther);
            }
        }
        #endregion


        private IEnumerator<IResult> DoRefOutputType_All()
        {
            var paymentTypeTask = new LoadOutputListTask(false, false, true);
            yield return paymentTypeTask;
            RefOutputTypeList = paymentTypeTask.RefOutputTypeList.Where(x => x.IsSelectedPharmacyInternal == true).ToObservableCollection();
            yield break;
        }

        private IEnumerator<IResult> DoGetStore_EXTERNAL()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_EXTERNAL, false, null, false, false);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList;
            SetDefaultForStore();
            yield break;
        }
        private IEnumerator<IResult> DoGetStore_All()
        {
            var paymentTypeTask = new LoadStoreListTask(null, false, null, false, false);
            yield return paymentTypeTask;
            AllStoreCbx = paymentTypeTask.LookupList;
            SetDefaultForStore();
            yield break;
        }
        private IEnumerator<IResult> DoGetByOutPriceLookups()
        {
            var paymentTypeTask = new LoadLookupListTask(LookupValues.V_ByOutPrice, false, false);
            yield return paymentTypeTask;
            ByOutPriceLookups = paymentTypeTask.LookupList;
            yield break;
        }

        private void GetAllStaffContrain()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllStaffContain(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetAllStaffContain(asyncResult);
                                if (results != null)
                                {
                                    ListStaff = results.ToObservableCollection();
                                }
                                else
                                {
                                    if (ListStaff != null)
                                    {
                                        ListStaff.Clear();
                                    }
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
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();

        }

        private void GetHospital_IsFriends()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginHopital_IsFriends(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndHopital_IsFriends(asyncResult);
                                Hospitals = results.ToObservableCollection();
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
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        public void grdPrescription_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        //ChangedWPF-CMN
        //public void grdPrescription_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        //{
        //    SumTotalPriceOutward();
        //}

        private bool CheckValid()
        {
            bool result = true;
            if (SelectedOutInvoice != null)
            {
                if (SelectedOutInvoice.OutwardDrugs == null)
                {
                    return false;
                }
                foreach (OutwardDrug item in SelectedOutInvoice.OutwardDrugs)
                {
                    if (item.Validate() == false)
                    {
                        result = false;
                        break;
                    }
                }
            }
            return result;
        }

        private void SumTotalPriceOutward()
        {
            if (!Compare2Object() || !Compare2ObjectInvoice())
            {
                IsOther = true;
            }
            else
            {
                IsOther = false;
            }
            if (SelectedOutInvoice != null)
            {
                SumTotalPrice = 0;
                SelectedOutInvoice.TotalInvoicePrice = 0;
                if (SelectedOutInvoice.OutwardDrugs != null && SelectedOutInvoice.OutwardDrugs.Count != 0)
                {
                    foreach (OutwardDrug p in SelectedOutInvoice.OutwardDrugs)
                    {
                        SumTotalPrice = SumTotalPrice + p.TotalPrice;
                        SelectedOutInvoice.TotalInvoicePrice += p.TotalPrice;
                    }
                }
            }
        }

        DataGrid grdPrescription = null;
        public void grdPrescription_Loaded(object sender, RoutedEventArgs e)
        {
            grdPrescription = sender as DataGrid;
        }

        public void grdPrescription_Unloaded(object sender, RoutedEventArgs e)
        {
            grdPrescription.SetValue(DataGrid.ItemsSourceProperty, null);
        }

        #region printing member

        public void btnPreview()
        {
            void onInitDlg(IReportDocumentPreview proAlloc)
            {
                proAlloc.ID = SelectedOutInvoice.outiID;
                proAlloc.eItem = ReportName.PHARMACY_XUATNOIBO;
            }
            GlobalsNAV.ShowDialog<IReportDocumentPreview>(onInitDlg);
        }

        public void btnPrint()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ReportServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetOutwardInternalInPdfFormat(SelectedOutInvoice.outiID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetOutwardInternalInPdfFormat(asyncResult);
                                var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, results, ActiveXPrintType.ByteArray);
                                Globals.EventAggregator.Publish(results);
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
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        #endregion

        #region auto Drug For Prescription member
        private string BrandName;
        private readonly bool IsHIPatient = false;

        private ObservableCollection<GetDrugForSellVisitor> GetDrugForSellVisitorList;

        private ObservableCollection<GetDrugForSellVisitor> _GetDrugForSellVisitorSum;
        public ObservableCollection<GetDrugForSellVisitor> GetDrugForSellVisitorListSum
        {
            get { return _GetDrugForSellVisitorSum; }
            set
            {
                if (_GetDrugForSellVisitorSum != value)
                    _GetDrugForSellVisitorSum = value;
                NotifyOfPropertyChange(() => GetDrugForSellVisitorListSum);
            }
        }

        private ObservableCollection<GetDrugForSellVisitor> GetDrugForSellVisitorTemp;

        private GetDrugForSellVisitor _SelectedSellVisitor;
        public GetDrugForSellVisitor SelectedSellVisitor
        {
            get { return _SelectedSellVisitor; }
            set
            {
                if (_SelectedSellVisitor != value)
                    _SelectedSellVisitor = value;
                NotifyOfPropertyChange(() => SelectedSellVisitor);
            }
        }

        AutoCompleteBox au;
        public void AutoDrug_Text_Loaded(object sender, RoutedEventArgs e)
        {
            au = sender as AutoCompleteBox;
        }

        public void AutoCompleteBox_Populating(object sender, PopulatingEventArgs e)
        {
            if (!IsCode.GetValueOrDefault())
            {
                BrandName = e.Parameter;
                //tim theo ten
                SearchGetDrugForSellVisitor(e.Parameter, false);
            }
        }
        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (au.SelectedItem != null)
            {
                SelectedSellVisitor = au.SelectedItem as GetDrugForSellVisitor;
            }
        }
        private void ListDisplayAutoComplete()
        {
            var hhh = from hd in GetDrugForSellVisitorList
                      group hd by new { hd.DrugID, hd.DrugCode, hd.BrandName, hd.UnitName, hd.Qty } into hdgroup
                      select new
                      {
                          Remaining = hdgroup.Sum(groupItem => groupItem.Remaining),
                          hdgroup.Key.DrugID
                          ,
                          hdgroup.Key.DrugCode
                          ,
                          hdgroup.Key.UnitName
                          ,
                          hdgroup.Key.BrandName
                          ,
                          hdgroup.Key.Qty
                      };
            for (int i = 0; i < hhh.Count(); i++)
            {
                GetDrugForSellVisitor item = new GetDrugForSellVisitor
                {
                    DrugID = hhh.ToList()[i].DrugID,
                    DrugCode = hhh.ToList()[i].DrugCode,
                    BrandName = hhh.ToList()[i].BrandName,
                    UnitName = hhh.ToList()[i].UnitName,
                    Remaining = hhh.ToList()[i].Remaining,
                    Qty = hhh.ToList()[i].Qty
                };
                GetDrugForSellVisitorListSum.Add(item);
            }
            if (IsCode.GetValueOrDefault())
            {
                if (GetDrugForSellVisitorListSum != null && GetDrugForSellVisitorListSum.Count > 0)
                {
                    var item = GetDrugForSellVisitorListSum.Where(x => x.DrugCode.ToUpper() == txt.ToUpper());
                    if (item != null && item.Count() > 0)
                    {
                        SelectedSellVisitor = item.ToList()[0];
                    }
                    else
                    {
                        MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                    }
                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                }
            }
            else
            {
                if (au != null)
                {
                    au.ItemsSource = GetDrugForSellVisitorListSum;
                    au.PopulateComplete();
                }
            }
        }

        //▼====== #001
        private void SearchGetDrugForSellVisitor(string Name, bool? IsCode)
        {
            long? RequestID = null;
            if (SelectedOutInvoice != null)
            {
                RequestID = SelectedOutInvoice.ReqDrugInID;
            }
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDrugForSellVisitorAutoComplete_ForRequestPharmacy(IsCost, Name, StoreID, RequestID, IsCode
                        , IsDlgSearchByGenericName, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDrugForSellVisitorAutoComplete_ForRequestPharmacy(asyncResult);
                            GetDrugForSellVisitorList.Clear();
                            GetDrugForSellVisitorListSum.Clear();
                            GetDrugForSellVisitorTemp.Clear();
                            foreach (GetDrugForSellVisitor s in results)
                            {
                                GetDrugForSellVisitorTemp.Add(s);
                            }
                            if (ListOutwardDrugFirstCopy != null && ListOutwardDrugFirstCopy.Count > 0)
                            {
                                foreach (OutwardDrug d in ListOutwardDrugFirstCopy)
                                {
                                    var value = results.Where(x => x.DrugID == d.DrugID && x.InBatchNumber == d.InBatchNumber && x.InID == d.InID);
                                    if (value.Count() > 0)
                                    {
                                        foreach (GetDrugForSellVisitor s in value.ToList())
                                        {
                                            s.Remaining = s.Remaining + d.OutQuantityOld;
                                            s.RemainingFirst = s.RemainingFirst + d.OutQuantityOld;
                                        }
                                    }
                                    else
                                    {
                                        GetDrugForSellVisitor p = d.GetDrugForSellVisitor;
                                        p.Remaining = d.OutQuantity;
                                        p.RemainingFirst = d.OutQuantity;
                                        p.InBatchNumber = d.InBatchNumber;
                                        p.SellingPrice = d.OutPrice;
                                        p.InID = Convert.ToInt64(d.InID);
                                        p.STT = d.STT;
                                        GetDrugForSellVisitorTemp.Add(p);
                                        // d = null;
                                    }
                                }
                            }
                            foreach (GetDrugForSellVisitor s in GetDrugForSellVisitorTemp)
                            {
                                if (SelectedOutInvoice.OutwardDrugs != null && SelectedOutInvoice.OutwardDrugs.Count > 0)
                                {
                                    foreach (OutwardDrug d in SelectedOutInvoice.OutwardDrugs)
                                    {
                                        if (d.DrugID == s.DrugID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID)
                                        {
                                            s.Remaining = s.Remaining - d.OutQuantity;
                                        }
                                    }
                                }
                                GetDrugForSellVisitorList.Add(s);
                            }
                            ListDisplayAutoComplete();

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                    }), null);
                }
            });

            t.Start();
        }
        //▲======= #001

        private bool CheckValidDrugAuto(GetDrugForSellVisitor temp)
        {
            if (temp == null)
            {
                return false;
            }
            return !temp.HasErrors;
        }

        private void CheckBatchNumberExists(OutwardDrug p)
        {
            bool kq = false;
            if (SelectedOutInvoice.OutwardDrugs != null)
            {
                foreach (OutwardDrug p1 in SelectedOutInvoice.OutwardDrugs)
                {
                    if (p.DrugID == p1.DrugID && p.InBatchNumber == p1.InBatchNumber && p.InID == p1.InID)
                    {
                        p1.OutQuantity += p.OutQuantity;
                        // p1.IsLoad = 0;
                        p1.QtyOffer += p.QtyOffer;
                        kq = true;
                        break;
                    }
                }
                if (!kq)
                {
                    p.HI = p.GetDrugForSellVisitor.InsuranceCover;

                    if (p.InwardDrug == null)
                    {
                        p.InwardDrug = new InwardDrug();
                        p.InwardDrug.InID = p.InID.GetValueOrDefault();
                        p.InwardDrug.DrugID = p.DrugID;
                    }
                    p.InvoicePrice = p.OutPrice;
                    p.InwardDrug.NormalPrice = p.OutPrice;
                    p.InwardDrug.HIPatientPrice = p.OutPrice;
                    p.InwardDrug.HIAllowedPrice = p.HIAllowedPrice;

                    SelectedOutInvoice.OutwardDrugs.Add(p);

                }
                txt = "";
                SelectedSellVisitor = null;
                if (IsCode.GetValueOrDefault())
                {
                    if (tbx != null)
                    {
                        tbx.Text = "";
                        tbx.Focus();
                    }

                }
                else
                {
                    if (au != null)
                    {
                        au.Text = "";
                        au.Focus();
                    }
                }
            }
        }

        private void ChooseBatchNumber(GetDrugForSellVisitor value)
        {
            var items = GetDrugForSellVisitorList.Where(x => x.DrugID == value.DrugID).OrderBy(p => p.STT);
            foreach (GetDrugForSellVisitor item in items)
            {
                OutwardDrug p = new OutwardDrug();
                if (item.Remaining > 0)
                {
                    if (item.Remaining - value.RequiredNumber < 0)
                    {
                        if (value.Qty > item.Remaining)
                        {
                            p.QtyOffer = item.Remaining;
                            value.Qty = value.Qty - item.Remaining;
                        }
                        else
                        {
                            p.QtyOffer = value.Qty;
                            value.Qty = 0;
                        }
                        value.RequiredNumber = value.RequiredNumber - item.Remaining;
                        p.GetDrugForSellVisitor = item;
                        p.DrugID = item.DrugID;
                        p.InBatchNumber = item.InBatchNumber;
                        p.InID = item.InID;
                        p.OutPrice = item.OutPrice;
                        p.OutQuantity = item.Remaining;
                        p.InExpiryDate = item.InExpiryDate;
                        p.SdlDescription = item.SdlDescription;
                        p.VAT = item.VAT;
                        CheckBatchNumberExists(p);
                        item.Remaining = 0;
                    }
                    else
                    {
                        p.GetDrugForSellVisitor = item;
                        p.DrugID = item.DrugID;
                        p.InBatchNumber = item.InBatchNumber;
                        p.InID = item.InID;
                        //if (value.Qty > (int)value.RequiredNumber)
                        //{
                        //    p.QtyOffer = (int)value.RequiredNumber;
                        //    value.Qty = value.Qty - (int)value.RequiredNumber;
                        //}
                        //else
                        {
                            p.QtyOffer = value.Qty;
                            value.Qty = 0;
                        }
                        p.OutQuantity = (int)value.RequiredNumber;
                        p.OutPrice = item.OutPrice;
                        p.InExpiryDate = item.InExpiryDate;
                        p.SdlDescription = item.SdlDescription;
                        p.VAT = item.VAT;
                        CheckBatchNumberExists(p);
                        item.Remaining = item.Remaining - (int)value.RequiredNumber;
                        break;
                    }
                }
            }
            SumTotalPriceOutward();
        }

        private void AddListOutwardDrug(GetDrugForSellVisitor value)
        {
            if (value != null)
            {
                if (value.RequiredNumber > 0)
                {
                    int intOutput = 0;
                    if (Int32.TryParse(value.RequiredNumber.ToString(), out intOutput))
                    {
                        int a = Convert.ToInt32(value.RequiredNumber);
                        if (CheckValidDrugAuto(value))
                        {
                            ChooseBatchNumber(value);
                        }
                    }
                    else
                    {
                        MessageBox.Show(eHCMSResources.A0972_G1_Msg_InfoSLgKhHopLe);
                    }
                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0776_G1_Msg_InfoSLgLonHon0);
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.K0389_G1_ChonThuocCanBan);
            }
        }

        private void ReCountQtyRequest()
        {
            if (SelectedOutInvoice != null && SelectedSellVisitor != null)
            {
                if (SelectedOutInvoice.OutwardDrugs == null)
                {
                    SelectedOutInvoice.OutwardDrugs = new ObservableCollection<OutwardDrug>();
                }
                var results1 = SelectedOutInvoice.OutwardDrugs.Where(x => x.DrugID == SelectedSellVisitor.DrugID);
                if (results1 != null && results1.Count() > 0)
                {
                    foreach (OutwardDrug p in results1)
                    {
                        if (p.QtyOffer > p.OutQuantity)
                        {
                            p.QtyOffer = p.OutQuantity;
                        }
                        SelectedSellVisitor.Qty = SelectedSellVisitor.Qty - p.QtyOffer;
                    }
                }
            }
        }

        public void AddItem(object sender, RoutedEventArgs e)
        {
            ReCountQtyRequest();
            AddListOutwardDrug(SelectedSellVisitor);
        }
        #region Properties member
        private ObservableCollection<GetDrugForSellVisitor> BatchNumberListTemp;
        private ObservableCollection<GetDrugForSellVisitor> BatchNumberListShow;
        private ObservableCollection<OutwardDrug> OutwardDrugListByDrugID;
        private ObservableCollection<OutwardDrug> OutwardDrugListByDrugIDFirst;

        private OutwardDrug _SelectedOutwardDrug;
        public OutwardDrug SelectedOutwardDrug
        {
            get { return _SelectedOutwardDrug; }
            set
            {
                if (_SelectedOutwardDrug != value)
                    _SelectedOutwardDrug = value;
                NotifyOfPropertyChange(() => SelectedOutwardDrug);
            }
        }
        #endregion

        private void GetDrugForSellVisitorBatchNumber(long DrugID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetDrugForSellVisitorBatchNumber(DrugID, StoreID, IsHIPatient, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDrugForSellVisitorBatchNumber(asyncResult);
                            BatchNumberListTemp = results.ToObservableCollection();
                            if (BatchNumberListTemp != null && BatchNumberListTemp.Count > 0)
                            {
                                UpdateListToShow();
                            }
                            else
                            {
                                Globals.ShowMessage(eHCMSResources.Z0891_G1_KgConLoNaoKhac, eHCMSResources.G0442_G1_TBao);
                            }

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

        public void lnkChooseBatchNumber_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedOutInvoice != null)
            {
                Button lnkBatchNumber = sender as Button;
                long DrugID = (long)lnkBatchNumber.CommandParameter;
                OutwardDrugListByDrugID = SelectedOutInvoice.OutwardDrugs.Where(x => x.DrugID == DrugID).ToObservableCollection();
                if (ListOutwardDrugFirstCopy != null)
                {
                    OutwardDrugListByDrugIDFirst = ListOutwardDrugFirstCopy.Where(x => x.DrugID == DrugID).ToObservableCollection();
                }
                GetDrugForSellVisitorBatchNumber(DrugID);
            }
        }

        #region IHandle<ChooseBatchNumberVisitorEvent> Members

        public void Handle(EditChooseBatchNumberVisitorEvent message)
        {
            if (message != null && this.IsActive)
            {
                SelectedOutwardDrug.GetDrugForSellVisitor = message.BatchNumberVisitorSelected;
                SelectedOutwardDrug.InBatchNumber = message.BatchNumberVisitorSelected.InBatchNumber;
                SelectedOutwardDrug.InExpiryDate = message.BatchNumberVisitorSelected.InExpiryDate;
                SelectedOutwardDrug.InID = message.BatchNumberVisitorSelected.InID;
                if (IsCost.GetValueOrDefault())
                {
                    SelectedOutwardDrug.OutPrice = message.BatchNumberVisitorSelected.InCost;
                }
                else
                {
                    SelectedOutwardDrug.OutPrice = message.BatchNumberVisitorSelected.SellingPrice;
                }
                SelectedOutwardDrug.SdlDescription = message.BatchNumberVisitorSelected.SdlDescription;
                SelectedOutwardDrug.VAT = message.BatchNumberVisitorSelected.VAT;
                SumTotalPriceOutward();
            }
        }

        #endregion

        #region IHandle<ChooseBatchNumberVisitorResetQtyEvent> Members

        public void Handle(EditChooseBatchNumberVisitorResetQtyEvent message)
        {
            if (message != null && this.IsActive)
            {
                SelectedOutwardDrug.GetDrugForSellVisitor = message.BatchNumberVisitorSelected;
                SelectedOutwardDrug.InBatchNumber = message.BatchNumberVisitorSelected.InBatchNumber;
                SelectedOutwardDrug.InExpiryDate = message.BatchNumberVisitorSelected.InExpiryDate;
                SelectedOutwardDrug.InID = message.BatchNumberVisitorSelected.InID;
                if (IsCost.GetValueOrDefault())
                {
                    SelectedOutwardDrug.OutPrice = message.BatchNumberVisitorSelected.InCost;
                }
                else
                {
                    SelectedOutwardDrug.OutPrice = message.BatchNumberVisitorSelected.SellingPrice;
                }
                SelectedOutwardDrug.SdlDescription = message.BatchNumberVisitorSelected.SdlDescription;
                SelectedOutwardDrug.OutQuantity = message.BatchNumberVisitorSelected.Remaining;
                SelectedOutwardDrug.VAT = message.BatchNumberVisitorSelected.VAT;
                SumTotalPriceOutward();
            }
        }

        #endregion

        public void UpdateListToShow()
        {
            if (OutwardDrugListByDrugIDFirst != null)
            {
                foreach (OutwardDrug d in OutwardDrugListByDrugIDFirst)
                {
                    var value = BatchNumberListTemp.Where(x => x.DrugID == d.DrugID && x.InBatchNumber == d.InBatchNumber && x.InID == d.InID);
                    if (value.Count() > 0)
                    {
                        foreach (GetDrugForSellVisitor s in value.ToList())
                        {
                            s.Remaining = s.Remaining + d.OutQuantityOld;
                            s.RemainingFirst = s.RemainingFirst + d.OutQuantityOld;
                        }
                    }
                    else
                    {
                        GetDrugForSellVisitor p = d.GetDrugForSellVisitor;
                        p.Remaining = d.OutQuantity;
                        p.RemainingFirst = d.OutQuantity;
                        p.InBatchNumber = d.InBatchNumber;
                        p.SellingPrice = d.OutPrice;
                        p.InID = Convert.ToInt64(d.InID);
                        p.STT = d.STT;
                        BatchNumberListTemp.Add(p);
                    }
                }
            }
            foreach (GetDrugForSellVisitor s in BatchNumberListTemp)
            {
                if (OutwardDrugListByDrugID.Count > 0)
                {
                    foreach (OutwardDrug d in OutwardDrugListByDrugID)
                    {
                        //20200422 TBL: Trừ luôn số lượng dòng nhập đang được chọn để đổi lô
                        //if (d.DrugID == s.DrugID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID && d.InID != SelectedOutwardDrug.InID)
                        if (d.DrugID == s.DrugID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID)
                        {
                            s.Remaining = s.Remaining - d.OutQuantity;
                        }
                    }
                }
            }

            BatchNumberListShow = BatchNumberListTemp.Where(x => x.Remaining > 0).ToObservableCollection();

            if (BatchNumberListShow != null && BatchNumberListShow.Count > 0)
            {
                Action<IChooseBatchNumberVisitor> onInitDlg = delegate (IChooseBatchNumberVisitor proAlloc)
                {
                    proAlloc.SelectedOutwardDrug = SelectedOutwardDrug.DeepCopy();
                    proAlloc.FormType = 2;//form chinh sua
                    if (BatchNumberListShow != null)
                    {
                        proAlloc.BatchNumberListShow = BatchNumberListShow.DeepCopy();
                    }
                    if (OutwardDrugListByDrugID != null)
                    {
                        proAlloc.OutwardDrugListByDrugID = OutwardDrugListByDrugID.DeepCopy();
                    }
                };
                GlobalsNAV.ShowDialog<IChooseBatchNumberVisitor>(onInitDlg);
            }
            else
            {
                Globals.ShowMessage(eHCMSResources.Z0891_G1_KgConLoNaoKhac, eHCMSResources.G0442_G1_TBao);
            }
        }
        #endregion

        private void DeleteInvoiceDrugInObject()
        {
            OutwardDrug p = SelectedOutwardDrug.DeepCopy();
            SelectedOutInvoice.OutwardDrugs.Remove(SelectedOutwardDrug);
            foreach (OutwardDrug item in SelectedOutInvoice.OutwardDrugs)
            {
                if (item.DrugID == p.DrugID)
                {
                    item.QtyOffer = item.QtyOffer + p.QtyOffer;
                    break;
                }
            }
            SelectedOutInvoice.OutwardDrugs = SelectedOutInvoice.OutwardDrugs.ToObservableCollection();
            SumTotalPriceOutward();
        }

        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedOutwardDrug != null)
            {
                DeleteInvoiceDrugInObject();
            }
            else
            {
                Globals.ShowMessage(eHCMSResources.A0915_G1_Msg_InfoPhChiXem, eHCMSResources.G0442_G1_TBao);
            }
        }

        private void ClearData()
        {
            ListOutwardDrugFirstCopy = null;

            if (GetDrugForSellVisitorList == null)
            {
                GetDrugForSellVisitorList = new ObservableCollection<GetDrugForSellVisitor>();
            }
            else
            {
                GetDrugForSellVisitorList.Clear();
            }
            if (GetDrugForSellVisitorListSum == null)
            {
                GetDrugForSellVisitorListSum = new ObservableCollection<GetDrugForSellVisitor>();
            }
            else
            {
                GetDrugForSellVisitorListSum.Clear();
            }

            if (GetDrugForSellVisitorTemp == null)
            {
                GetDrugForSellVisitorTemp = new ObservableCollection<GetDrugForSellVisitor>();
            }
            else
            {
                GetDrugForSellVisitorTemp.Clear();
            }
        }

        #region IHandle<PharmacyCloseSearchRequestEvent> Members

        public void Handle(PharmacyCloseSearchRequestEvent message)
        {
            if (message != null && IsActive)
            {
                RequestDrugInward Request = message.SelectedRequest as RequestDrugInward;
                if (Request != null)
                {
                    ClearData();
                    SelectedOutInvoice.ReqDrugInID = Request.ReqDrugInID;
                    SelectedOutInvoice.ReqNumCode = Request.ReqNumCode;
                    SelectedOutInvoice.ToStoreID = Request.InDeptStoreID;
                    SelectedOutInvoice.ToStaffID = Request.StaffID;
                    CheckAccept();
                    spGetInBatchNumberAndPrice_ByRequestID(Request.ReqDrugInID, StoreID);
                }
            }
        }

        private void CheckAccept()
        {
            if (SelectedOutInvoice.ToStoreID > 0)
            {
                IsInternalForStore = true;
            }
            else if (SelectedOutInvoice.ToStaffID > 0)
            {
                IsInternalForDoctor = true;
            }
            else
            {
                IsInternalForHospital = true;
            }
        }

        #endregion

        public void spGetInBatchNumberAndPrice_ByRequestID(long RequestID, long StoreID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginspGetInBatchNumberAndPrice_ByRequestPharmacy(IsCost, RequestID, StoreID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndspGetInBatchNumberAndPrice_ByRequestPharmacy(asyncResult);
                                SelectedOutInvoice.OutwardDrugs = results.ToObservableCollection();
                                ListOutwardDrugFirstCopy = null;
                                SumTotalPriceOutward();
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
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        public void btnFindRequest(object sender, RoutedEventArgs e)
        {
            //IsChanged = true;
            void onInitDlg(IRequestSearchPharmacy proAlloc)
            {
                if (proAlloc.SearchCriteria == null)
                {
                    proAlloc.SearchCriteria = new RequestSearchCriteria();
                }
                proAlloc.SearchCriteria.DaNhanHang = true;
                proAlloc.SearchRequestDrugInward(0, Globals.PageSize);
            }
            GlobalsNAV.ShowDialog<IRequestSearchPharmacy>(onInitDlg);
        }

        private void UpdateInvoiceInfo(OutwardDrugInvoice OutwardInvoice)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdateInvoiceInfo(OutwardInvoice, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool value = contract.EndUpdateInvoiceInfo(asyncResult);
                                Globals.EventAggregator.Publish(new PharmacyCloseEditPayed { SelectedOutwardInvoice = SelectedOutInvoice });
                                TryClose();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
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
                    this.HideBusyIndicator();
                    MessageBox.Show(ex.Message);
                }
            });

            t.Start();
        }

        private void OutwardDrugInvoice_SaveByType(OutwardDrugInvoice OutwardInvoice, bool bThuTien)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacySaleAndOutwardClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        //contract.BeginOutwardDrugInvoice_SaveByType(OutwardInvoice, Globals.DispatchCallback((asyncResult) =>
                        contract.BeginOutwardDrugInvoice_SaveByType_Pst(OutwardInvoice, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                //bool value = contract.EndOutwardDrugInvoice_SaveByType(out OutID, out StrError, asyncResult);
                                bool value = contract.EndOutwardDrugInvoice_SaveByType_Pst(out long OutID, out string StrError, asyncResult);
                                if (string.IsNullOrEmpty(StrError) && value)
                                {
                                    OutwardInvoice.outiID = OutID;
                                    //phat su kien de form o duoi load lai du lieu 
                                    Globals.EventAggregator.Publish(new PharmacyCloseEditPayed { SelectedOutwardInvoice = OutwardInvoice });
                                    TryClose();
                                }
                                else
                                {
                                    MessageBox.Show(StrError);
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
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        private void UpdateInvoicePayed(OutwardDrugInvoice OutwardInvoice)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacySaleAndOutwardClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        //contract.BeginUpdateInvoicePayed(OutwardInvoice, Globals.DispatchCallback((asyncResult) =>
                        contract.BeginUpdateInvoicePayed_Pst(OutwardInvoice, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                //bool value = contract.EndUpdateInvoicePayed(out OutID, out PaymemtID, out StrError, asyncResult);
                                bool value = contract.EndUpdateInvoicePayed_Pst(out long OutID, out long PaymemtID, out string StrError, asyncResult);
                                if (string.IsNullOrEmpty(StrError) && value)
                                {
                                    SelectedOutInvoice.outiID = OutID;
                                    //phat su kien de form o duoi load lai du lieu 
                                    Globals.EventAggregator.Publish(new PharmacyCloseEditPayed { SelectedOutwardInvoice = SelectedOutInvoice });
                                    TryClose();
                                    //  CountMoneyForVisitorPharmacy(OutID);
                                }
                                else
                                {
                                    MessageBox.Show(StrError);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
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
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        private bool Equal(OutwardDrug a, OutwardDrug b)
        {
            if (a.InID == b.InID && a.DrugID == b.DrugID && a.InBatchNumber == b.InBatchNumber && a.InExpiryDate == b.InExpiryDate && a.OutPrice == b.OutPrice && a.OutQuantity == b.OutQuantity)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool Compare2ObjectInvoice()
        {
            if (SelectedOutInvoice != null && SelectedOutInvoiceCoppy != null)
            {
                if (SelectedOutInvoice.OutDate != SelectedOutInvoiceCoppy.OutDate || SelectedOutInvoice.ToStaffID != SelectedOutInvoiceCoppy.ToStaffID
                    || SelectedOutInvoice.ToStoreID != SelectedOutInvoiceCoppy.ToStoreID || SelectedOutInvoice.HosID != SelectedOutInvoiceCoppy.HosID
                    || SelectedOutInvoice.TypID != SelectedOutInvoiceCoppy.TypID || SelectedOutInvoice.V_ByOutPrice != SelectedOutInvoiceCoppy.V_ByOutPrice)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        private bool Compare2Object()
        {
            if (SelectedOutInvoice.OutwardDrugs != null && OutwardDrugListCopy != null && SelectedOutInvoice.OutwardDrugs.Count == OutwardDrugListCopy.Count && SelectedOutInvoice.OutwardDrugs.Count > 0)
            {

                int icount = 0;
                for (int i = 0; i < OutwardDrugListCopy.Count; i++)
                {
                    for (int j = 0; j < SelectedOutInvoice.OutwardDrugs.Count; j++)
                    {
                        if (Equal(OutwardDrugListCopy[i], SelectedOutInvoice.OutwardDrugs[j]))
                        {
                            icount++;
                        }
                    }
                }
                if (icount == OutwardDrugListCopy.Count)
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
                return false;
            }
        }

        public void btnSave()
        {
            if (SelectedOutInvoice.OutwardDrugs != null && SelectedOutInvoice.OutwardDrugs.Count > 0)
            {
                if (CheckData())
                {
                    if (!Compare2Object())
                    {
                        string strError = "";
                        for (int i = 0; i < SelectedOutInvoice.OutwardDrugs.Count; i++)
                        {
                            if (SelectedOutInvoice.OutwardDrugs[i].OutPrice <= 0)
                            {
                                MessageBox.Show(eHCMSResources.A0525_G1_Msg_InfoGiaBanLonHon0);
                                return;
                            }
                            //neu ngay het han lon hon ngay hien tai
                            if (eHCMS.Services.Core.AxHelper.CompareDate(Globals.ServerDate.Value, SelectedOutInvoice.OutwardDrugs[i].InExpiryDate) == 1)
                            {
                                strError += string.Format(eHCMSResources.Z1401_G1_DaHetHanDung, SelectedOutInvoice.OutwardDrugs[i].GetDrugForSellVisitor.BrandName, (i + 1).ToString());
                            }
                        }
                        if (!string.IsNullOrEmpty(strError))
                        {
                            if (MessageBox.Show(strError + Environment.NewLine + eHCMSResources.I0939_G1_I, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                            {
                                return;
                            }
                        }
                        if (CheckValid())
                        {

                            if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUATNOIBO || SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUATNOIBO_LUANCHUYENKHO)
                            {
                                SelectedOutInvoice.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.SAVE;
                            }
                            else
                            {
                                SelectedOutInvoice.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.SAVE;
                            }

                            if (SelectedOutInvoice.PaidTime == null)
                            {
                                OutwardDrugInvoice_SaveByType(SelectedOutInvoice, false);
                            }
                            else
                            {
                                UpdateInvoicePayed(SelectedOutInvoice);
                            }
                        }
                        else
                        {
                            MessageBox.Show(eHCMSResources.A0540_G1_Msg_InfoDataKhHopLe);
                        }
                    }
                    else
                    {
                        //goi ham cap nhat hoa don
                        UpdateInvoiceInfo(SelectedOutInvoice);
                    }
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0639_G1_Msg_InfoKhCoCTietPhBanLe);
            }
        }

        private bool CheckData()
        {
            string strError = "";
            if (SelectedOutInvoice != null)
            {
                SelectedOutInvoice.StaffID = GetStaffLogin().StaffID;
                SelectedOutInvoice.StoreID = StoreID;
            }
            if (IsInternalForStore)
            {
                if (SelectedOutInvoice.ToStoreID == null || SelectedOutInvoice.ToStoreID == 0)
                {
                    MessageBox.Show(eHCMSResources.A0092_G1_Msg_InfoChuaChonKhoDen, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
                if (SelectedOutInvoice.StoreID == SelectedOutInvoice.ToStoreID)
                {
                    MessageBox.Show(eHCMSResources.A0618_G1_Msg_InfoKhoXuatKhacKhoNhan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
                SelectedOutInvoice.ToStaffID = 0;
                SelectedOutInvoice.HosID = 0;
            }
            else if (IsInternalForDoctor)
            {
                if (SelectedOutInvoice.ToStaffID == null || SelectedOutInvoice.ToStaffID == 0)
                {
                    MessageBox.Show(eHCMSResources.A0098_G1_Msg_InfoChuaChonNguoiNhan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
                SelectedOutInvoice.ToStoreID = 0;
                SelectedOutInvoice.HosID = 0;
            }
            else
            {
                if (SelectedOutInvoice.HosID == null || SelectedOutInvoice.HosID == 0)
                {
                    MessageBox.Show(eHCMSResources.A0089_G1_Msg_InfoChuaChonBVNhan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
                SelectedOutInvoice.ToStaffID = 0;
                SelectedOutInvoice.ToStoreID = 0;
            }

            if (SelectedOutInvoice.OutwardDrugs == null || SelectedOutInvoice.OutwardDrugs.Count == 0)
            {
                MessageBox.Show(eHCMSResources.A0640_G1_Msg_InfoKhCoCTietPhXuat, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
            else
            {
                SelectedOutInvoice.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.SAVE;
                if (SelectedOutInvoice.OutwardDrugs != null)
                {
                    for (int i = 0; i < SelectedOutInvoice.OutwardDrugs.Count; i++)
                    {
                        if (SelectedOutInvoice.OutwardDrugs[i].OutPrice <= 0)
                        {
                            MessageBox.Show("Đơn giá bán phải > 0", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            return false;
                        }
                        if (SelectedOutInvoice.OutwardDrugs[i].GetDrugForSellVisitor != null && SelectedOutInvoice.OutwardDrugs[i].OutQuantity <= 0)
                        {
                            MessageBox.Show(eHCMSResources.Z1174_G1_SLgXuatLonHon0, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            return false;
                        }
                        //neu ngay het han lon hon ngay hien tai
                        if (eHCMS.Services.Core.AxHelper.CompareDate(Globals.ServerDate.Value, SelectedOutInvoice.OutwardDrugs[i].InExpiryDate) == 1)
                        {
                            strError += string.Format(eHCMSResources.Z1401_G1_DaHetHanDung, SelectedOutInvoice.OutwardDrugs[i].GetDrugForSellVisitor.BrandName, (i + 1).ToString());
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(strError))
            {
                if (MessageBox.Show(strError + Environment.NewLine + eHCMSResources.I0939_G1_I, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private void DeepCopyOutwardDrug()
        {
            if (SelectedOutInvoice.OutwardDrugs != null)
            {
                ListOutwardDrugFirstCopy = SelectedOutInvoice.OutwardDrugs.DeepCopy();
            }
            else
            {
                ListOutwardDrugFirstCopy = null;
            }
            if (SelectedOutInvoice != null)
            {
                SelectedOutInvoiceCoppy = SelectedOutInvoice.DeepCopy();
            }
        }

        //private int icount = 0;
        public void SelectedByOutPrice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (SelectedOutInvoice != null && icount > 0)
            if (SelectedOutInvoice != null)
            {
                GetIsCost();

                if (SelectedOutInvoice.V_ByOutPrice == (long)AllLookupValues.V_ByOutPrice.GIATHONGTHUONG)
                {
                    grdPrescription.Columns[(int)DataGridCol.DonGia].IsReadOnly = true;
                    if (SelectedOutInvoice.OutwardDrugs != null)
                    {
                        for (int i = 0; i < SelectedOutInvoice.OutwardDrugs.Count; i++)
                        {
                            SelectedOutInvoice.OutwardDrugs[i].OutPrice = SelectedOutInvoice.OutwardDrugs[i].GetDrugForSellVisitor.SellingPrice;
                        }
                    }
                }
                else if (SelectedOutInvoice.V_ByOutPrice == (long)AllLookupValues.V_ByOutPrice.GIAVON)
                {
                    grdPrescription.Columns[(int)DataGridCol.DonGia].IsReadOnly = true;
                    if (SelectedOutInvoice.OutwardDrugs != null)
                    {
                        for (int i = 0; i < SelectedOutInvoice.OutwardDrugs.Count; i++)
                        {
                            SelectedOutInvoice.OutwardDrugs[i].OutPrice = SelectedOutInvoice.OutwardDrugs[i].GetDrugForSellVisitor.InCost;
                        }
                    }
                }
                else
                {
                    //mo cot gia len cho sua 
                    grdPrescription.Columns[(int)DataGridCol.DonGia].IsReadOnly = false;
                }
                SumTotalPriceOutward();
            }
            //icount++;
        }

        public void SelectedByToStore_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SumTotalPriceOutward();
        }
        public void SelectedByToStaff_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SumTotalPriceOutward();
        }
        public void SelectedByToHospital_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SumTotalPriceOutward();
        }

        public void SelectedOutputType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                RefOutputType temp = (RefOutputType)e.AddedItems[0];
                if (temp.TypID == (long)AllLookupValues.RefOutputType.XUATNOIBO_LUANCHUYENKHO)
                {
                    FromStoreCbx = AllStoreCbx.Where(x => x.StoreTypeID == (long)AllLookupValues.StoreType.STORAGE_EXTERNAL).ToObservableCollection();
                    CanSelectByOutPrice = false;
                    IsEnableToStore = SelectedOutInvoice != null && SelectedOutInvoice.outiID > 0 ? SelectedOutInvoice.IsEnableToStore : false;
                }
                else
                {
                    FromStoreCbx = AllStoreCbx;
                    IsEnableToStore = true;
                    CanSelectByOutPrice = SelectedOutInvoice != null && SelectedOutInvoice.outiID > 0 ? SelectedOutInvoice.CanEditPayed : true;
                }
            }
        }

        private void GetIsCost()
        {
            if (SelectedOutInvoice != null)
            {
                //if (SelectedOutInvoice.TypID !=(long)AllLookupValues.RefOutputType.XUATNOIBO)
                //{
                //    IsCost = true;
                //}
                //else
                //{
                if (SelectedOutInvoice.V_ByOutPrice == (long)AllLookupValues.V_ByOutPrice.GIAVON)
                {
                    IsCost = true;
                }
                else
                {
                    IsCost = false;
                }
                //}
            }
        }

        string txt = "";
        public void AxTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txt != (sender as TextBox).Text)
            {
                txt = (sender as TextBox).Text;

                if (!string.IsNullOrEmpty(txt))
                {
                    SearchGetDrugForSellVisitor(txt, true);
                }
            }
        }

        public void DrugCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // au.IsEnabled = false;
                string text = (sender as TextBox).Text;
                if (!string.IsNullOrEmpty(text))
                {
                    SearchGetDrugForSellVisitor((sender as TextBox).Text, true);
                }
            }
        }

        TextBox tbx = null;
        public void AxTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            tbx = sender as TextBox;
        }

        private bool _VisibilityName = true;
        public bool VisibilityName
        {
            get
            {
                if (SelectedOutInvoice != null)
                {
                    return _VisibilityName;
                }
                return _VisibilityName;
            }
            set
            {
                if (SelectedOutInvoice != null)
                {
                    _VisibilityName = value;
                    _VisibilityCode = !_VisibilityName;
                }
                else
                {
                    _VisibilityName = value;
                    _VisibilityCode = !_VisibilityName;
                }
                NotifyOfPropertyChange(() => VisibilityName);
                NotifyOfPropertyChange(() => VisibilityCode);
            }
        }

        private bool _VisibilityCode = false;
        public bool VisibilityCode
        {
            get
            {
                return _VisibilityCode;
            }
            set
            {
                if (_VisibilityCode != value)
                {
                    _VisibilityCode = value;
                    NotifyOfPropertyChange(() => VisibilityCode);
                }
            }
        }

        private bool? IsCode = false;
        public void Code_Checked(object sender, RoutedEventArgs e)
        {
            IsCode = true;
            VisibilityName = false;
        }

        public void Name_Checked(object sender, RoutedEventArgs e)
        {
            IsCode = false;
            VisibilityName = true;
        }

        AxTextBox AxQty = null;
        public void Quantity_Loaded(object sender, RoutedEventArgs e)
        {
            AxQty = sender as AxTextBox;
        }

        private bool _CanSelectByOutPrice = true;
        public bool CanSelectByOutPrice
        {
            get
            {
                return _CanSelectByOutPrice;
            }
            set
            {
                _CanSelectByOutPrice = value;
                NotifyOfPropertyChange(() => CanSelectByOutPrice);
            }
        }

        private bool _IsEnableToStore = false;
        public bool IsEnableToStore
        {
            get
            {
                return _IsEnableToStore;
            }
            set
            {
                _IsEnableToStore = value;
                NotifyOfPropertyChange(() => IsEnableToStore);
            }
        }

        #region Checked All Member
        private bool _AllChecked;
        public bool AllChecked
        {
            get
            {
                return _AllChecked;
            }
            set
            {
                if (_AllChecked != value)
                {
                    _AllChecked = value;
                    NotifyOfPropertyChange(() => AllChecked);
                    if (_AllChecked)
                    {
                        AllCheckedfc();
                    }
                    else
                    {
                        UnCheckedfc();
                    }
                }
            }
        }

        private void HideShowColumnDelete()
        {
            if (grdPrescription != null)
            {
                if (SelectedOutInvoice.CanSaveAndPaid && mXuatNoiBo_ThucHien)
                {
                    grdPrescription.Columns[(int)DataGridCol.ColDelete].Visibility = Visibility.Visible;
                    grdPrescription.Columns[(int)DataGridCol.ColMultiDelete].Visibility = Visibility.Visible;
                }
                else
                {
                    grdPrescription.Columns[(int)DataGridCol.ColDelete].Visibility = Visibility.Collapsed;
                    grdPrescription.Columns[(int)DataGridCol.ColMultiDelete].Visibility = Visibility.Collapsed;
                }
            }
        }

        private void AllCheckedfc()
        {
            if (SelectedOutInvoice != null && SelectedOutInvoice.OutwardDrugs != null && SelectedOutInvoice.OutwardDrugs.Count > 0)
            {
                for (int i = 0; i < SelectedOutInvoice.OutwardDrugs.Count; i++)
                {
                    SelectedOutInvoice.OutwardDrugs[i].Checked = true;
                }
            }
        }

        private void UnCheckedfc()
        {
            if (SelectedOutInvoice != null && SelectedOutInvoice.OutwardDrugs != null && SelectedOutInvoice.OutwardDrugs.Count > 0)
            {
                for (int i = 0; i < SelectedOutInvoice.OutwardDrugs.Count; i++)
                {
                    SelectedOutInvoice.OutwardDrugs[i].Checked = false;
                }
            }
        }

        public void btnDeleteHang()
        {
            if (SelectedOutInvoice != null && SelectedOutInvoice.OutwardDrugs != null && SelectedOutInvoice.OutwardDrugs.Count > 0)
            {
                var items = SelectedOutInvoice.OutwardDrugs.Where(x => x.Checked == true);
                if (items != null && items.Count() > 0)
                {
                    var lstremaning = SelectedOutInvoice.OutwardDrugs.Where(x => x.Checked == false).ToObservableCollection().DeepCopy();
                    var lstDelete = SelectedOutInvoice.OutwardDrugs.Where(x => x.Checked == true).ToObservableCollection().DeepCopy();

                    if (MessageBox.Show(eHCMSResources.Z0565_G1_CoChacXoaHangDaChon, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        foreach (OutwardDrug p in lstDelete)
                        {
                            foreach (OutwardDrug item in lstremaning)
                            {
                                if (item.DrugID == p.DrugID)
                                {
                                    item.QtyOffer = item.QtyOffer + p.QtyOffer;
                                    break;
                                }
                            }
                        }
                        SumTotalPriceOutward();
                        ListDisplayAutoComplete();
                    }
                    SelectedOutInvoice.OutwardDrugs = lstremaning;
                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0090_G1_Msg_InfoChuaChonHgCanXoa);
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0090_G1_Msg_InfoChuaChonHgCanXoa);
            }
        }
        #endregion

        public void grdPrescription_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.DisplayIndex == (int)DataGridCol.DonGia || e.Column.DisplayIndex == (int)DataGridCol.ThucXuat)
            {
                SumTotalPriceOutward();
            }
        }

        public void btnCancel()
        {
            TryClose();
        }

        //▼====== #001
        private bool _isDlgSearchByGenericName = false;
        public bool IsDlgSearchByGenericName
        {
            get { return _isDlgSearchByGenericName; }
            set
            {
                if (_isDlgSearchByGenericName != value)
                {
                    _isDlgSearchByGenericName = value;
                    NotifyOfPropertyChange(() => IsDlgSearchByGenericName);
                }
            }
        }

        private bool _visSearchByGenericName = Globals.ServerConfigSection.PharmacyElements.PharmacySearchByGenericName;
        public bool vIsSearchByGenericName
        {
            get { return _visSearchByGenericName; }
            set
            {
                if (_visSearchByGenericName != value)
                {
                    _visSearchByGenericName = value;
                    NotifyOfPropertyChange(() => vIsSearchByGenericName);
                }
            }
        }

        public void chkSearchByGenericName_Loaded(object sender, RoutedEventArgs e)
        {
            var chkDlgSearchByGenericName = sender as CheckBox;

            if (Globals.ServerConfigSection.PharmacyElements.PharmacySearchByGenericName)
            {
                chkDlgSearchByGenericName.IsChecked = true;
            }
            else
            {
                chkDlgSearchByGenericName.IsChecked = false;
            }
        }
        //▲======= #001
    }
}
