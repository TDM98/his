using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Common.Utilities;
using System.Collections.Generic;
using System.Linq;
using aEMR.Controls;
using eHCMSLanguage;
using Castle.Windsor;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IRefGenDrug_Add)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RefGenDrug_AddViewModel : Conductor<object>, IRefGenDrug_Add
        , IHandle<PharContraEvent>
        , IHandle<PharmacyCloseSearchSupplierEvent>
    {

        private bool _IsAddFinishClosed = false;
        public bool IsAddFinishClosed
        {
            get
            {
                return _IsAddFinishClosed;
            }
            set
            {
                if (_IsAddFinishClosed != value)
                {
                    _IsAddFinishClosed = value;
                    NotifyOfPropertyChange(() => IsAddFinishClosed);
                }
            }
        }

        private bool _IsLoading = false;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set
            {
                if (_IsLoading != value)
                {
                    _IsLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _IsAdd = true;
        public bool IsAdd
        {
            get { return _IsAdd; }
            set
            {
                if (_IsAdd != value)
                {
                    _IsAdd = value;
                    NotifyOfPropertyChange(() => IsAdd);
                }
            }
        }

        private bool _IsEdit ;
        public bool IsEdit
        {
            get { return _IsEdit; }
            set
            {
                if (_IsEdit != value)
                {
                    _IsEdit = value;
                    NotifyOfPropertyChange(() => IsEdit);
                }
            }
        }

        public string TitleForm { get; set; }
        [ImportingConstructor]
        public RefGenDrug_AddViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            _SelectedNewRefMedicalCondition = new RefMedContraIndicationTypes();
            _allRefMedicalCondition = new ObservableCollection<RefMedContraIndicationTypes>();
            allContraIndicatorDrugsRelToMedCond = new ObservableCollection<ContraIndicatorDrugsRelToMedCond>();
            RefMedicalConditionType_Edit = new EntitiesEdit<RefMedContraIndicationTypes>();
            InitNewDrug();
            Globals.EventAggregator.Subscribe(this);
            Hospitals = new ObservableCollection<Hospital>();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            if (NewDrug != null
                &&NewDrug.DrugID>0)
            {
                GetContraIndicatorDrugsRelToMedCondList(0, NewDrug.DrugID);
                SupplierGenericDrug_LoadDrugIDNotPaging(NewDrug.DrugID);
            }
        }

        #region Properties Member

        private EntitiesEdit<RefMedContraIndicationTypes> _RefMedicalConditionType_Edit;
        public EntitiesEdit<RefMedContraIndicationTypes> RefMedicalConditionType_Edit
        {
            get
            {
                return _RefMedicalConditionType_Edit;
            }
            set
            {
                if (_RefMedicalConditionType_Edit == value)
                    return;
                _RefMedicalConditionType_Edit = value;
                NotifyOfPropertyChange(() => RefMedicalConditionType_Edit);
            }
        }

        private SupplierGenericDrug _CurrentSupplierGenericDrug;
        public SupplierGenericDrug CurrentSupplierGenericDrug
        {
            get
            {
                return _CurrentSupplierGenericDrug;
            }
            set
            {
                if (_CurrentSupplierGenericDrug != value)
                {
                    _CurrentSupplierGenericDrug = value;
                    NotifyOfPropertyChange(() => CurrentSupplierGenericDrug);
                }
            }
        }

        private ObservableCollection<Supplier> _Suppliers;
        public ObservableCollection<Supplier> Suppliers
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

        private ObservableCollection<RefGenDrugBHYT_Category> _RefGenDrugBHYT_Categorys;
        public ObservableCollection<RefGenDrugBHYT_Category> RefGenDrugBHYT_Categorys
        {
            get
            {
                return _RefGenDrugBHYT_Categorys;
            }
            set
            {
                if (_RefGenDrugBHYT_Categorys != value)
                {
                    _RefGenDrugBHYT_Categorys = value;
                    NotifyOfPropertyChange(() => RefGenDrugBHYT_Categorys);
                }
            }
        }

        private ObservableCollection<RefGenericDrugCategory_2> _RefGenericDrugCategory_2s;
        public ObservableCollection<RefGenericDrugCategory_2> RefGenericDrugCategory_2s
        {
            get
            {
                return _RefGenericDrugCategory_2s;
            }
            set
            {
                if (_RefGenericDrugCategory_2s != value)
                {
                    _RefGenericDrugCategory_2s = value;
                    NotifyOfPropertyChange(() => RefGenericDrugCategory_2s);
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

        private ObservableCollection<PharmaceuticalCompany> _pharmaceuticalCompanies;
        public ObservableCollection<PharmaceuticalCompany> PharmaceuticalCompanies
        {
            get { return _pharmaceuticalCompanies; }
            set
            {
                if (_pharmaceuticalCompanies != value)
                    _pharmaceuticalCompanies = value;
                NotifyOfPropertyChange(() => PharmaceuticalCompanies);
            }
        }

        private RefGenericDrugDetail _NewDrug;
        public RefGenericDrugDetail NewDrug
        {
            get
            {
                return _NewDrug;
            }
            set
            {
                if (_NewDrug != value)
                {
                    _NewDrug = value;
                    NotifyOfPropertyChange(() => NewDrug);
                }
            }
        }

        private ObservableCollection<ContraIndicatorDrugsRelToMedCond> _allContraIndicatorDrugsRelToMedCond;
        public ObservableCollection<ContraIndicatorDrugsRelToMedCond> allContraIndicatorDrugsRelToMedCond
        {
            get
            {
                return _allContraIndicatorDrugsRelToMedCond;
            }
            set
            {
                if (_allContraIndicatorDrugsRelToMedCond == value)
                    return;
                _allContraIndicatorDrugsRelToMedCond = value;
                NotifyOfPropertyChange(() => allContraIndicatorDrugsRelToMedCond);
            }
        }

        private ContraIndicatorDrugsRelToMedCond _SelectedContraIndicatorDrugsRelToMedCond;
        public ContraIndicatorDrugsRelToMedCond SelectedContraIndicatorDrugsRelToMedCond
        {
            get
            {
                return _SelectedContraIndicatorDrugsRelToMedCond;
            }
            set
            {
                if (_SelectedContraIndicatorDrugsRelToMedCond == value)
                    return;
                _SelectedContraIndicatorDrugsRelToMedCond = value;
                NotifyOfPropertyChange(() => SelectedContraIndicatorDrugsRelToMedCond);
            }
        }

        private ObservableCollection<RefMedContraIndicationTypes> _allRefMedicalCondition;
        public ObservableCollection<RefMedContraIndicationTypes> allRefMedicalCondition
        {
            get
            {
                return _allRefMedicalCondition;
            }
            set
            {
                if (_allRefMedicalCondition != value)
                {
                    _allRefMedicalCondition = value;
                    NotifyOfPropertyChange(() => allRefMedicalCondition);
                }
            }
        }

        private ObservableCollection<string> _allContrainName;
        public ObservableCollection<string> allContrainName
        {
            get
            {
                return _allContrainName;
            }
            set
            {
                if (_allContrainName == value)
                    return;
                _allContrainName = value;
            }
        }

        private RefMedContraIndicationTypes _SelectedNewRefMedicalCondition;
        public RefMedContraIndicationTypes SelectedNewRefMedicalCondition
        {
            get
            {
                return _SelectedNewRefMedicalCondition;
            }
            set
            {
                if (_SelectedNewRefMedicalCondition == value)
                    return;
                _SelectedNewRefMedicalCondition = value;
                NotifyOfPropertyChange(() => SelectedNewRefMedicalCondition);
            }
        }

        private long DrugID = 0;
        #endregion

        private void InitNewDrug()
        {
            NewDrug = null;
            NewDrug = new RefGenericDrugDetail();
            NewDrug.UnitPackaging = 1;
            NewDrug.NumberOfEstimatedMonths_F = 1;
            NewDrug.AdvTimeBeforeExpire = 0;
            AddBlankRow();
        }

       

        private void AddNewDrug()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginAddNewDrug(NewDrug, RefMedicalConditionType_Edit.TempObject, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            contract.EndAddNewDrug(out DrugID, asyncResult);
                            if (DrugID > 0)
                            {
                                if (MessageBox.Show(eHCMSResources.Z1487_G1_CoMuonThem1ThuocKhac, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                {
                                    InitNewDrug();
                                    RefMedicalConditionType_Edit.Clear();
                                }
                                else
                                {
                                    NewDrug.DrugID = DrugID;
                                    if (IsAddFinishClosed)
                                    {
                                        TryClose();
                                        //add xong dog lai lien va chon thuoc moi nay de lam viec luon
                                        Globals.EventAggregator.Publish(new PharmacyCloseFinishAddGenDrugEvent { SupplierDrug = NewDrug });
                                    }
                                    else
                                    {
                                        TryClose();
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.K0053_G1_ThuocDaTonTai);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);
                }
            });
            t.Start();
        }

        private void UpdateDrug()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateDrug(NewDrug, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            contract.EndUpdateDrug(asyncResult);
                            //phat ra su kien de form cha bat dc va load lai du lieu
                            Globals.EventAggregator.Publish(new PharmacyCloseEditGenDrugEvent { });
                            //Đóng Popup
                            MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK);
                            TryClose();

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void GetContraIndicatorDrugsRelToMedCondList(int MCTypeID, long DrugID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetContraIndicatorDrugsRelToMedCondList(MCTypeID, DrugID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetContraIndicatorDrugsRelToMedCondList(asyncResult);
                            if (results != null)
                            {
                                var obsRMCT=new ObservableCollection<RefMedContraIndicationTypes>();
                                foreach (var p in results)
                                {
                                    allContraIndicatorDrugsRelToMedCond.Add(p);
                                    obsRMCT.Add(p.RefMedicalConditionType);
                                    //RefMedicalConditionType_Edit.CurObject = allContraIndicatorDrugsRelToMedCond;
                                    RefMedicalConditionType_Edit = new EntitiesEdit<RefMedContraIndicationTypes>(obsRMCT);
                                }
                                NotifyOfPropertyChange(() => allContraIndicatorDrugsRelToMedCond);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void SupplierGenericDrug_LoadDrugIDNotPaging(long DrugID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSupplierGenericDrug_LoadDrugIDNotPaging(DrugID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndSupplierGenericDrug_LoadDrugIDNotPaging(asyncResult);
                            NewDrug.SupplierGenericDrugs = results.ToObservableCollection();
                            if (NewDrug.SupplierGenericDrugs == null)
                            {
                                NewDrug.SupplierGenericDrugs = new ObservableCollection<SupplierGenericDrug>();
                            }
                            AddBlankRow();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void InsertConIndicatorDrugsRelToMedCond(ObservableCollection<RefMedContraIndicationTypes> lstRefMedicalCondition, long DrugID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginInsertConIndicatorDrugsRelToMedCond(lstRefMedicalCondition, DrugID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            bool results = contract.EndInsertConIndicatorDrugsRelToMedCond(asyncResult);
                            if (results)
                            {
                                allRefMedicalCondition.Clear();
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void InsertDeleteUpdateConIndicatorDrugsRelToMedCondXML(IList<ContraIndicatorDrugsRelToMedCond> lstInsert
                                                    , IList<ContraIndicatorDrugsRelToMedCond> lstDelete
                                                    , IList<ContraIndicatorDrugsRelToMedCond> lstUpdate)
        {
            var t = new Thread(
                () =>{
                        using (var serviceFactory = new PharmacyDrugServiceClient())
                        {
                            var contract = serviceFactory.ServiceInstance;
                            contract.BeginInsertDeleteUpdateConIndicatorDrugsRelToMedCondXML(lstInsert, lstDelete
                                ,lstUpdate,  Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    bool results = contract.EndInsertDeleteUpdateConIndicatorDrugsRelToMedCondXML(asyncResult);
                                    if (results)
                                    {
                                        //MessageBox.Show("Cập nhật chống chỉ định cho thuốc thành công!");
                                        RefMedicalConditionType_Edit.Clear();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                }
                                finally
                                {
                                    IsLoading = false;
                                    Globals.IsBusy = false;
                                }

                            }), null);

                        }

                    });
            t.Start();
        }

        private bool CheckData()
        {
            NewDrug.StaffID = Globals.LoggedUserAccount.StaffID.GetValueOrDefault();
            if (NewDrug.SupplierGenericDrugs == null || NewDrug.SupplierGenericDrugs.Count == 0)
            {
                MessageBox.Show(eHCMSResources.A0195_G1_Msg_InfoChonNCCChoThuoc);
                return false;
            }
            else
            {
                int icount = 0;
                for (int i = 0; i < NewDrug.SupplierGenericDrugs.Count; i++)
                {
                    if (NewDrug.SupplierGenericDrugs[i].SupplierID > 0)
                    {
                        if (NewDrug.SupplierGenericDrugs[i].UnitPrice <= 0 || NewDrug.SupplierGenericDrugs[i].PackagePrice <= 0)
                        {
                            MessageBox.Show(eHCMSResources.A0519_G1_Msg_InfoDGiaLonHon0);
                            return false;
                        }
                    }
                    if (NewDrug.SupplierGenericDrugs[i].SupplierID > 0 && NewDrug.SupplierGenericDrugs[i].IsMain == true)
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
            }
            return true;
        }

        public void OKButton(object sender, RoutedEventArgs e)
        {
            if (CheckValid(NewDrug))
            {
                if (CheckData())
                {
                    if (NewDrug.PharmaceuticalCompany != null)
                    {
                        NewDrug.PCOID = NewDrug.PharmaceuticalCompany.PCOID;
                    }
                    AddNewDrug();
                }
            }
        }

        
        public ObservableCollection<ContraIndicatorDrugsRelToMedCond> createListNew()
        {
            var lstNew = new ObservableCollection<ContraIndicatorDrugsRelToMedCond>();
            foreach (var item in RefMedicalConditionType_Edit.NewObject)
            {
                ContraIndicatorDrugsRelToMedCond p=new ContraIndicatorDrugsRelToMedCond();
                p.RefGenericDrugDetail = NewDrug;
                p.RefMedicalConditionType = item;
                lstNew.Add(p);
            }
            return lstNew;
        }
        public ObservableCollection<ContraIndicatorDrugsRelToMedCond> createListDelete()
        {
            var lstNew = new ObservableCollection<ContraIndicatorDrugsRelToMedCond>();
            foreach (var item in RefMedicalConditionType_Edit.DeleteObject)
            {
                foreach (var contra in allContraIndicatorDrugsRelToMedCond)
                {
                    if (item.MedContraTypeID == contra.MCTypeID)
                    {
                        lstNew.Add(contra);
                    }
                }
            }
            return lstNew;
        }
        public ObservableCollection<ContraIndicatorDrugsRelToMedCond> createListUpdate()
        {
            var lstNew = new ObservableCollection<ContraIndicatorDrugsRelToMedCond>();

            return lstNew;
        }

        public void EditButton(object sender, RoutedEventArgs e)
        {
            if (CheckValid(NewDrug))
            {
                if (CheckData())
                {
                    if (NewDrug.PharmaceuticalCompany != null)
                    {
                        NewDrug.PCOID = NewDrug.PharmaceuticalCompany.PCOID;
                    }
                    UpdateDrug();
                    //if (RefMedicalConditionType_Edit.TempObject != null
                    //    && RefMedicalConditionType_Edit.TempObject.Count>0)
                    {
                        //tao ra 3 list o day
                        InsertDeleteUpdateConIndicatorDrugsRelToMedCondXML(createListNew()
                                                                          , createListDelete()
                                                                          , null);
                    }
                }
            }
        }


        public void Refesh(object sender, RoutedEventArgs e)
        {
            InitNewDrug();
        }
        public void CancelButton(object sender, RoutedEventArgs e)
        {
            TryClose();
            if (!IsAddFinishClosed)
            {
                //phat ra su kien de form cha bat dc va load lai du lieu
                Globals.EventAggregator.Publish(new PharmacyCloseAddGenDrugEvent { });
            }
        }
        ValidationSummary ValidationSummary1 = null;
        public void ValidationSummary1_Loaded(object sender, RoutedEventArgs e)
        {
            ValidationSummary1 = sender as ValidationSummary;

        }
        private bool CheckValid(object temp)
        {
            RefGenericDrugDetail u = temp as RefGenericDrugDetail;
            if (u == null)
            {
                return false;
            }
            if (ValidationSummary1 != null && ValidationSummary1.HasDisplayedErrors)
            {
                return false;
            }
            string error = "";
            if (u.CountryID == null || u.CountryID <= 0)
            {
                error += eHCMSResources.Z0853_G1_ChuaChonQuocGia + Environment.NewLine;
            }
            if (u.PCOID ==null || u.PCOID <= 0)
            {
                error += eHCMSResources.Z0854_G1_ChuaChonNSX + Environment.NewLine;
            }
            if (u.DrugClassID == null || u.DrugClassID <= 0)
            {
                error += eHCMSResources.Z0855_G1_ChuaChonNhomThuoc + Environment.NewLine;
            }
            //if (u.DispenseVolume < 1)
            //{
            //    error += "DispenseVolume >=1. " + Environment.NewLine;
            //}
            if (u.DispenseVolume <= 0)
            {
                error += eHCMSResources.Z0856_G1_DispenseVolume + Environment.NewLine;
            }


            if (u.MonitorOutQty)
            {
                //KMx: Phải thỏa điều kiện: LimitedOutQty >= RemainWarningLevel2 >= RemainWarningLevel1 (13/11/2014 16:32).
                if (u.LimitedOutQty <= 0)
                {
                    error += eHCMSResources.Z0859_G1_SLgGioiHanLonHon0 + Environment.NewLine;
                }
                else
                {
                    if (u.RemainWarningLevel1 < 0 || u.RemainWarningLevel1 > u.LimitedOutQty)
                    {
                        error += eHCMSResources.Z1490_G1_SLgCBaoMucDo1 + Environment.NewLine;
                    }
                    else
                    {
                        if (u.RemainWarningLevel2 < 0 || (u.RemainWarningLevel2 > 0 && (u.RemainWarningLevel2 > u.LimitedOutQty  || u.RemainWarningLevel2 < u.RemainWarningLevel1)))
                        {
                            error += eHCMSResources.Z1491_G1_SLgCBaoMucDo2 + Environment.NewLine;
                        }
                    }
                }
            }


            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error);
                return false;
            }
          
            return u.Validate();
        }

        public void AddMed(object sender, RoutedEventArgs e)
        {
            Action<IMedCondition> onInitDlg = delegate (IMedCondition MedConditionVM)
            {
                //if (allContraIndicatorDrugsRelToMedCond != null)
                //{
                //    RefMedicalConditionType_Edit = new EntitiesEdit<RefMedicalConditionType>(allRefMedicalCondition);
                //}
                MedConditionVM.RefMedicalConditionType_Edit = RefMedicalConditionType_Edit;
                MedConditionVM.NewDrug = NewDrug;
                this.ActivateItem(MedConditionVM);
            };
            GlobalsNAV.ShowDialog<IMedCondition>(onInitDlg);
        }
        public void DeleteContraind(object sender, RoutedEventArgs e)
        {
            allContrainName.Remove(SelectedNewRefMedicalCondition.MedContraIndicationType);
            allRefMedicalCondition.Remove(SelectedNewRefMedicalCondition);
        }

        #region event Handle
        public void Handle(PharContraEvent obj)
        {
            if (obj != null)
            {
                //allRefMedicalCondition = (ObservableCollection<RefMedicalConditionType>)obj.lstPharContra;
                //allContrainName = (ObservableCollection<string>)obj.lstPharContraName;
                RefMedicalConditionType_Edit = obj.refMedicalConditionType_Edit;
            }
        }

        #endregion
        private void AddBlankRow()
        {
            if (NewDrug.SupplierGenericDrugs == null)
            {
                NewDrug.SupplierGenericDrugs = new ObservableCollection<SupplierGenericDrug>();
            }
            SupplierGenericDrug item = new SupplierGenericDrug();
            NewDrug.SupplierGenericDrugs.Add(item);
        }

        public void acbDrug_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as ComboBox).ItemsSource = Suppliers;
        }

        private bool CheckExists(SupplierGenericDrug ite)
        {
            if (ite != null && ite.SelectedSupplier != null)
            {
                ite.SupplierID = ite.SelectedSupplier.SupplierID;
                var value = NewDrug.SupplierGenericDrugs.Where(x => x.SupplierID == ite.SupplierID);
                if (value != null && value.Count() > 1)
                {
                    MessageBox.Show(eHCMSResources.Z0731_G1_NCCDaDuocChon);
                    ite.SelectedSupplier = null;
                    return false;
                }
            }
            else
            {
                //MessageBox.Show(eHCMSResources.K0347_G1_ChonNCC);
                return false;
            }
            return true;
        }

        DataGrid GridSuppliers = null;
        public void GridSuppliers_Loaded(object sender, RoutedEventArgs e)
        {
            GridSuppliers = sender as DataGrid;
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
        public void GridSuppliers_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            SupplierGenericDrug item = e.Row.DataContext as SupplierGenericDrug;
            if (e.Column.DisplayIndex == 1)//NCC
            {
                if (CheckExists(item))
                {
                    if (e.Row.GetIndex() == (NewDrug.SupplierGenericDrugs.Count - 1) && e.EditAction == DataGridEditAction.Commit)
                    {
                        AddBlankRow();
                    }
                }
            }
            if (e.Column.DisplayIndex == 3)//NCC
            {
                decimal value = 0;
                decimal.TryParse(PreparingCellForEdit, out value);

                if (value == item.UnitPrice)
                {
                    return;
                }
                item.PackagePrice = item.UnitPrice * NewDrug.UnitPackaging.GetValueOrDefault(1);
            }
            if (e.Column.DisplayIndex == 4)//NCC
            {
                decimal value = 0;
                decimal.TryParse(PreparingCellForEdit, out value);
                if (value == item.PackagePrice)
                {
                    return;
                }
                if (NewDrug.UnitPackaging.GetValueOrDefault() > 0)
                {
                    item.UnitPrice = item.PackagePrice / NewDrug.UnitPackaging.GetValueOrDefault(1);
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
                    if (NewDrug != null && NewDrug.SupplierGenericDrugs != null)
                    {
                        int value = 0;
                        int.TryParse(txt, out value);

                        NewDrug.UnitPackaging = value;
                        foreach (SupplierGenericDrug p in NewDrug.SupplierGenericDrugs)
                        {
                            p.PackagePrice = p.UnitPrice * NewDrug.UnitPackaging.GetValueOrDefault(1);
                        }
                    }
                }
            }

        }

        public void btnDeleteCC_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSupplierGenericDrug==NewDrug.SupplierGenericDrugs[NewDrug.SupplierGenericDrugs.Count - 1])
            {
                MessageBox.Show(eHCMSResources.A0181_G1_Msg_InfoXoaDongTrong);
                return;
            }

            if (MessageBox.Show(eHCMSResources.A0119_G1_Msg_ConfXoaNCC, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (CurrentSupplierGenericDrug != null)
                {
                    NewDrug.SupplierGenericDrugs.Remove(CurrentSupplierGenericDrug);
                }
                if (NewDrug.SupplierGenericDrugs.Count == 0)
                {
                    AddBlankRow();
                }
            }
        }

        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            AutoCompleteBox axBHYT = sender as AutoCompleteBox;
            if (NewDrug != null)
            {
                if (axBHYT != null && axBHYT.SelectedItem != null)
                {
                    RefGenDrugBHYT_Category item = axBHYT.SelectedItem as RefGenDrugBHYT_Category;
                    if (item != null && item.RefGenDrugBHYT_CatID > 0)
                    {
                        NewDrug.CurrentRefGenDrugBHYT_Category = item;
                        //KMx: Khi chọn nhóm BHYT thì không được đổi tên chung, để người dùng tự nhập (Nhi nhà thuốc yêu cầu) (23/07/2014 09:53).
                        //NewDrug.GenericName = item.CategoryName;
                        NewDrug.RefGenDrugBHYT_CatID = item.RefGenDrugBHYT_CatID;
                        NewDrug.ActiveIngredient = item.CategoryName;
                        NewDrug.HIDrugCode = item.DrugOrderNo;
                    }
                    else
                    {
                        NewDrug.CurrentRefGenDrugBHYT_Category = null;
                        //KMx: Khi chọn nhóm BHYT thì không được đổi tên chung, để người dùng tự nhập (Nhi nhà thuốc yêu cầu) (23/07/2014 09:53).
                        //NewDrug.GenericName = "";
                        NewDrug.ActiveIngredient = "";
                        NewDrug.HIDrugCode = "";
                    }
                }
                else
                {
                    NewDrug.CurrentRefGenDrugBHYT_Category = null;
                    //KMx: Khi chọn nhóm BHYT thì không được đổi tên chung, để người dùng tự nhập (Nhi nhà thuốc yêu cầu) (23/07/2014 09:53).
                    //NewDrug.GenericName = "";
                    NewDrug.ActiveIngredient = "";
                    NewDrug.HIDrugCode = "";
                }
            }
        }

        public void ApGiaDau_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            AutoCompleteBox axApThau = sender as AutoCompleteBox;
            if (NewDrug != null)
            {
                if (axApThau != null && axApThau.SelectedItem != null)
                {
                    Hospital item = axApThau.SelectedItem as Hospital;
                    if (item != null && item.HosID > 0)
                    {
                        NewDrug.CurrentHospital = item;
                        NewDrug.HosID = item.HosID;
                    }
                    else
                    {
                        NewDrug.CurrentHospital = null;
                        NewDrug.HosID = null;
                    }
                }
                else
                {
                    NewDrug.CurrentHospital = null;
                    NewDrug.HosID = null;
                }
            }
        }

        public void Country_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            AutoCompleteBox axCountry = sender as AutoCompleteBox;
            if (NewDrug != null)
            {
                if (axCountry != null && axCountry.SelectedItem != null)
                {
                    RefCountry item = axCountry.SelectedItem as RefCountry;
                    if (item != null && item.CountryID > 0)
                    {
                        NewDrug.SeletedCountry = item;
                        NewDrug.CountryID = item.CountryID;
                    }
                    else
                    {
                        NewDrug.SeletedCountry = null;
                        NewDrug.CountryID = 0;
                    }
                }
                else
                {
                    NewDrug.SeletedCountry = null;
                    NewDrug.CountryID = 0;
                }
            }

        }

        public void HangSX_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            AutoCompleteBox axCountry = sender as AutoCompleteBox;
            if (NewDrug != null)
            {
                if (axCountry != null && axCountry.SelectedItem != null)
                {
                    PharmaceuticalCompany item = axCountry.SelectedItem as PharmaceuticalCompany;
                    if (item != null && item.PCOID > 0)
                    {
                        NewDrug.PharmaceuticalCompany = item;
                        NewDrug.PCOID = item.PCOID;
                    }
                    else
                    {
                        NewDrug.PharmaceuticalCompany = null;
                        NewDrug.PCOID = 0;
                    }
                }
                else
                {
                    NewDrug.PharmaceuticalCompany = null;
                    NewDrug.PCOID = 0;
                }
            }
        }

        public void NhomThuoc_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            AutoCompleteBox axCountry = sender as AutoCompleteBox;
            if (NewDrug != null)
            {
                if (axCountry != null && axCountry.SelectedItem != null)
                {
                    DrugClass item = axCountry.SelectedItem as DrugClass;
                    if (item != null && item.DrugClassID > 0)
                    {
                        NewDrug.SeletedDrugClass = item;
                        NewDrug.DrugClassID = item.DrugClassID;
                    }
                    else
                    {
                        NewDrug.SeletedDrugClass = null;
                        NewDrug.DrugClassID = 0;
                    }
                }
                else
                {
                    NewDrug.SeletedDrugClass = null;
                    NewDrug.DrugClassID = 0;
                }
            }
        }

        
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

        public void Supplier_Click(object sender, RoutedEventArgs e)
        {
            Action<ISuppliers> onInitDlg = delegate (ISuppliers proAlloc)
            {
                proAlloc.IsChildWindow = true;
            };
            GlobalsNAV.ShowDialog<ISuppliers>(onInitDlg);
        }

        #region IHandle<PharmacyCloseSearchSupplierEvent> Members

        public void Handle(PharmacyCloseSearchSupplierEvent message)
        {
            if (message != null && this.IsActive)
            {
                CurrentSupplierGenericDrug.SelectedSupplier = message.SelectedSupplier as Supplier;
            }
        }

        #endregion

        private int ConvertStringToInt(string strNumber)
        {
            int value = 0;
            int.TryParse(strNumber, out value);
            return value;
        }

        
        public void txtLimitedOutQty_LostFocus(object sender, RoutedEventArgs e)
        {
            if (NewDrug == null)
            {
                return;
            }

            string txt = "";
            txt = (sender as TextBox).Text;
            if (!string.IsNullOrEmpty(txt))
            {
                int result = ConvertStringToInt(txt);
                NewDrug.LimitedOutQty = result > 0 ? result : 0;
            }
        }


        public void txtRemainWarningLevel1_LostFocus(object sender, RoutedEventArgs e)
        {
            if (NewDrug == null)
            {
                return;
            }

            string txt = "";
            txt = (sender as TextBox).Text;
            if (!string.IsNullOrEmpty(txt))
            {
                int result = ConvertStringToInt(txt);
                NewDrug.RemainWarningLevel1 = result > 0 ? result : 0;
            }
        }


        public void txtRemainWarningLevel2_LostFocus(object sender, RoutedEventArgs e)
        {
            if (NewDrug == null)
            {
                return;
            }

            string txt = "";
            txt = (sender as TextBox).Text;
            if (!string.IsNullOrEmpty(txt))
            {
                int result = ConvertStringToInt(txt);
                NewDrug.RemainWarningLevel2 = result > 0 ? result : 0;
            }
        }


        CheckBox chkMonitorOutQty;
        public void chkMonitorOutQty_Loaded(object sender, RoutedEventArgs e)
        {
            chkMonitorOutQty = sender as CheckBox;
        }

        public void chkMonitorOutQty_UnCheck(object sender, RoutedEventArgs e)
        {
            if (chkMonitorOutQty == null || NewDrug == null || (string.IsNullOrWhiteSpace(NewDrug.LimitedOutQty.ToString()) && string.IsNullOrWhiteSpace(NewDrug.RemainWarningLevel1.ToString()) && string.IsNullOrWhiteSpace(NewDrug.RemainWarningLevel1.ToString())))
            {
                return;
            }

            if (MessageBox.Show(eHCMSResources.A0430_G1_Msg_InfoTuDongXoaSLgGHan_SLgCBao, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                NewDrug.LimitedOutQty = 0;
                NewDrug.RemainWarningLevel1 = 0;
                NewDrug.RemainWarningLevel2 = 0;
            }
            else
            {
                chkMonitorOutQty.IsChecked = true;
            }
        }


    }
}
