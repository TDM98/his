using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using Caliburn.Micro;
using aEMR.ViewContracts;
using System.ComponentModel.Composition;
using DataEntities;
using aEMR.Common;
using aEMR.Controls;
using aEMR.Common.Collections;
using aEMR.Common.Utilities;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
/*
 * 20200725 #001 TTM: BM 0022848: Bổ sung kiểm tra phác đồ cho chỉ định khám bệnh ngoại trú.
 * 20200810 #002 TTM:   BM 0039422: Bổ sung code kiểm tra phác đồ nếu bệnh nhân có nhiều ICD cùng 1 phác đồ thì chỉ insert dữ liệu dịch vụ 1 lần thôi.
 */
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IInPatientSelectPclLAB)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InPatientSelectPclLABViewModel : Conductor<object>, IInPatientSelectPclLAB
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public InPatientSelectPclLABViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            _pclCategories = new ObservableCollection<Lookup>();

            SelectedPCLCategory = new Lookup();
            SelectedPCLCategory.LookupID = (long)AllLookupValues.V_PCLMainCategory.Laboratory;

            LoadV_PCLMainCategory();
            //LoadPCLForms(SelectedPCLCategory.LookupID); //20200414 TBL: Hàm này đã chạy 1 lần ở trên nên ở đây không cần chạy nữa
            PCLPCLExamTypeCombo_Search();

            _pclExamTypes = new ObservableCollection<PCLExamType>();
            dicPCLExamType = new Dictionary<long, PCLExamType>();
            foreach (var item in Globals.ListPclExamTypesAllPCLForms)
            {
                if (!dicPCLExamType.ContainsKey(item.PCLExamTypeID))
                {
                    dicPCLExamType.Add(item.PCLExamTypeID, item);
                }
            }
            foreach (var item in Globals.ListPclExamTypesAllCombos)
            {
                if (!dicPCLExamType.ContainsKey(item.PCLExamTypeID))
                {
                    dicPCLExamType.Add(item.PCLExamTypeID, item.PCLExamType);
                }
            }

        }
        private PCLFormsSearchCriteria _searchCriteria;
        public PCLFormsSearchCriteria SearchCriteria
        {
            get
            {
                return _searchCriteria;
            }
            set
            {
                _searchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);

            }
        }

        public Dictionary<long, PCLExamType> _dicPCLExamType;
        public Dictionary<long, PCLExamType> dicPCLExamType
        {
            get
            {
                return _dicPCLExamType;
            }
            set
            {
                _dicPCLExamType = value;
                NotifyOfPropertyChange(() => dicPCLExamType);

            }
        }

        private bool _showUsedField = true;
        public bool ShowUsedField
        {
            get { return _showUsedField; }
            set
            {
                _showUsedField = value;
                NotifyOfPropertyChange(() => ShowUsedField);
            }
        }
        private bool _showLocationSelection = true;
        public bool ShowLocationSelection
        {
            get { return _showLocationSelection; }
            set
            {
                _showLocationSelection = value;
                NotifyOfPropertyChange(() => ShowLocationSelection);
            }
        }

        private ObservableCollection<Lookup> _pclCategories;
        public ObservableCollection<Lookup> PCLCategories
        {
            get { return _pclCategories; }
        }

        private Lookup _selectedPCLCategory;
        public Lookup SelectedPCLCategory
        {
            get { return _selectedPCLCategory; }
            set
            {
                _selectedPCLCategory = value;
                NotifyOfPropertyChange(() => SelectedPCLCategory);
                long? pclCategory = null;
                if (_selectedPCLCategory != null)
                {
                    pclCategory = _selectedPCLCategory.LookupID;
                }
                if (_pclForms != null && _pclForms.Count > 0)
                {
                    SelectedPCLForm = _pclForms[0];
                }
                else
                {
                    SelectedPCLForm = null;
                }
                LoadPCLForms(pclCategory);
            }
        }

        private ObservableCollection<PCLForm> _pclForms;

        public ObservableCollection<PCLForm> PCLForms
        {
            get { return _pclForms; }
            set
            {
                _pclForms = value;
                NotifyOfPropertyChange(() => PCLForms);
            }
        }


        private PCLExamTypeCombo _selectedCombo;
        public PCLExamTypeCombo SelectedCombo
        {
            get { return _selectedCombo; }
            set
            {
                _selectedCombo = value;
                NotifyOfPropertyChange(() => SelectedCombo);
                if (_selectedCombo != null && _selectedCombo.PCLExamTypeComboID > 0)
                {
                    if (CurrentInPatientAdmDisDetail != null && CurrentInPatientAdmDisDetail.PCLExamTypePriceListID > 0)
                    {
                        LoadAllPCLExamTypesByComboIDAction(_selectedCombo.PCLExamTypeComboID);
                    }
                    else
                    {
                        PCLExamType_ByComboID(_selectedCombo.PCLExamTypeComboID, Globals.ListPclExamTypesAllCombos);
                    }
                }
                else
                {
                    if (PclExamTypes != null)
                        PclExamTypes.Clear();
                }
            }
        }
        private PCLForm _selectedPCLForm;

        public PCLForm SelectedPCLForm
        {
            get { return _selectedPCLForm; }
            set
            {
                _selectedPCLForm = value;
                NotifyOfPropertyChange(() => SelectedPCLForm);
                if (_selectedPCLForm != null && _selectedPCLForm.PCLFormID > 0)
                {
                    //LoadPCLExamTypes();
                    if (CurrentInPatientAdmDisDetail != null && CurrentInPatientAdmDisDetail.PCLExamTypePriceListID > 0)
                    {
                        LoadAllPclExamTypesAction("");
                    }
                    else
                    {
                        LoadPCLExamTypesLinq("", Globals.ListPclExamTypesAllPCLForms);
                    }
                }
                else
                {
                    if (PclExamTypes != null)
                        PclExamTypes.Clear();
                }
            }
        }

        private PCLExamTypeLocation _selectedPclExamTypeLocation;
        public PCLExamTypeLocation SelectedPclExamTypeLocation
        {
            get { return _selectedPclExamTypeLocation; }
            set
            {
                _selectedPclExamTypeLocation = value;
                NotifyOfPropertyChange(() => SelectedPclExamTypeLocation);
            }
        }

        private void ResetPCLExamTypes()
        {
            if (PclExamTypes == null)
            {
                PclExamTypes = new ObservableCollection<PCLExamType>();
            }
            else
            {
                PclExamTypes.Clear();
            }
            PclExamTypes.Add(new PCLExamType { PCLExamTypeID = -1, PCLExamTypeName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0156_G1_Chon1DV) });
            SelectedPCLExamType = PclExamTypes[0];
        }

        private bool _isLoadingPclForm;
        public bool IsLoadingPclForm
        {
            get { return _isLoadingPclForm; }
            set
            {
                if (_isLoadingPclForm != value)
                {
                    _isLoadingPclForm = value;
                    NotifyOfPropertyChange(() => IsLoadingPclForm);

                    NotifyWhenBusy();
                }
            }
        }
        private bool _isLoadingCombo;
        public bool IsLoadingCombo
        {
            get { return _isLoadingCombo; }
            set
            {
                if (_isLoadingCombo != value)
                {
                    _isLoadingCombo = value;
                    NotifyOfPropertyChange(() => IsLoadingCombo);

                    NotifyWhenBusy();
                }
            }
        }
        private ObservableCollection<PCLItem> _pclItemList;

        public ObservableCollection<PCLItem> PCLItemList
        {
            get { return _pclItemList; }
            set
            {
                _pclItemList = value;
                NotifyOfPropertyChange(() => PCLItemList);
            }
        }

        private bool _used;

        public bool Used
        {
            get { return _used; }
            set
            {
                _used = value;
                NotifyOfPropertyChange(() => Used);
            }
        }

        private void NotifyWhenBusy()
        {
            NotifyOfPropertyChange(() => IsProcessing);
            NotifyOfPropertyChange(() => StatusText);
        }
        public bool IsProcessing
        {
            get
            {
                return _isLoadingPclForm || _isLoadingCombo;
            }

        }
        public string StatusText
        {
            get
            {
                if (_isLoadingPclForm)
                {
                    return eHCMSResources.Z0176_G1_DangTaiDSNhomCLS;
                }
                else if (_isLoadingCombo)
                {
                    return eHCMSResources.Z0177_G1_DangTaiDSCombo;
                }
                return "";
            }
        }
        public long? DeptID { get; set; }

        public void LoadPCLForms(long? pclCategory)
        {
            var t = new Thread(() =>
            {
                IsLoadingPclForm = true;
                Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.K3022_G1_DSPCLForms) });
                AxErrorEventArgs error = null;
                try
                {
                    SearchCriteria = new PCLFormsSearchCriteria { V_PCLMainCategory = (long)AllLookupValues.V_PCLMainCategory.Laboratory, OrderBy = "" };

                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLForms_GetList_Paging(SearchCriteria, 0, 10000, "", false, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                int total;
                                var allItems = client.EndPCLForms_GetList_Paging(out total, asyncResult);
                                if (allItems != null)
                                {
                                    PCLForms = new ObservableCollection<PCLForm>(allItems);
                                    var firstItem = new PCLForm { PCLFormID = -1, PCLFormName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2074_G1_ChonNhom2) };
                                    PCLForms.Insert(0, firstItem);
                                }
                                else
                                {
                                    PCLForms = null;
                                }
                                SelectedPCLForm = PCLForms.FirstOrDefault();
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
                                IsLoadingPclForm = false;
                                Globals.IsBusy = false;
                                if (error != null)
                                {
                                    Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
                                }
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {
                    Globals.IsBusy = false;
                    if (error != null)
                    {
                        Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
                    }
                }
            });
            t.Start();
        }

        private PCLExamType _selectedPCLExamType;

        public PCLExamType SelectedPCLExamType
        {
            get { return _selectedPCLExamType; }
            set
            {
                if (_selectedPCLExamType != value)
                {
                    _selectedPCLExamType = value;
                    NotifyOfPropertyChange(() => SelectedPCLExamType);
                    Globals.EventAggregator.Publish(new ItemSelected<PCLExamType> { Item = _selectedPCLExamType });

                    SelectedPclExamTypeLocation = null;
                }
            }
        }

        private ObservableCollection<PCLExamType> _pclExamTypes;

        public ObservableCollection<PCLExamType> PclExamTypes
        {
            get { return _pclExamTypes; }
            private set
            {
                _pclExamTypes = value;
                NotifyOfPropertyChange(() => PclExamTypes);
            }
        }

        private bool _isLoadingPclExamTypes;
        public bool IsLoadingPclExamTypes
        {
            get { return _isLoadingPclExamTypes; }
            set
            {
                if (!_isLoadingPclExamTypes)
                {
                    _isLoadingPclExamTypes = value;
                    NotifyOfPropertyChange(() => IsLoadingPclExamTypes);
                }
            }
        }
        //private void LoadPCLExamTypes()
        //{
        //    //_criteria.PCLGroupID = _selectedPCLGroup != null ? (long?) _selectedPCLGroup.PCLGroupID : null;
        //    //_criteria.PclCategory = _selectedPCLCategory != null ? (long?)_selectedPCLCategory.LookupID : null;

        //    var t = new Thread(() =>
        //    {
        //        IsLoadingPclExamTypes = true;
        //        Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang tìm danh sách dịch vụ CLS..." });
        //        AxErrorEventArgs error = null;
        //        try
        //        {
        //            using (var serviceFactory = new ConfigurationManagerServiceClient())
        //            {
        //                var client = serviceFactory.ServiceInstance;
        //                var criteria = new PCLExamTypeSearchCriteria { V_PCLMainCategory = SelectedPCLCategory.LookupID };
        //                client.BeginPCLItems_ByPCLFormID(criteria, SelectedPCLForm.PCLFormID, Globals.DispatchCallback(
        //                    delegate(IAsyncResult asyncResult)
        //                    {
        //                        IList<PCLExamType> allItems = null;
        //                        bool bOK = false;
        //                        try
        //                        {
        //                            allItems = client.EndPCLItems_ByPCLFormID(asyncResult);
        //                            bOK = true;
        //                        }
        //                        catch (FaultException<AxException> fault)
        //                        {
        //                            error = new AxErrorEventArgs(fault);
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            error = new AxErrorEventArgs(ex);
        //                        }
        //                        finally
        //                        {
        //                            if (error != null)
        //                            {
        //                                Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
        //                            }
        //                            Globals.IsBusy = false;

        //                            PclExamTypes.Clear();

        //                            if (bOK)
        //                            {
        //                                if (allItems != null)
        //                                {
        //                                    foreach (var item in allItems)
        //                                    {
        //                                        PclExamTypes.Add(item);
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }), null)
        //                    ;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            error = new AxErrorEventArgs(ex);
        //        }
        //        finally
        //        {
        //            IsLoadingPclExamTypes = false;
        //            Globals.IsBusy = false;
        //        }
        //        if (error != null)
        //        {
        //            Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
        //        }
        //    });
        //    t.Start();
        //}

        private void LoadPCLExamTypesLinq(string str, IList<PCLExamType> ListPclExamTypesAllPCLForms)
        {
            //▼===== #001
            IList<PCLExamType> listpcl = new List<PCLExamType>();
            ObservableCollection<PCLExamTypeComboItem> listpclCombo = new ObservableCollection<PCLExamTypeComboItem>();
            Dictionary<long, PCLExamType> dicPCLExamTypeForTreatmentRegimen = null;
            if (IsRegimenChecked && ListRegiment != null)
            {
                PCLExamType TreatmentRegimenPCLDetail = new PCLExamType();

                foreach (var item in ListRegiment)
                {
                    if (item.RefTreatmentRegimenPCLDetails != null && item.RefTreatmentRegimenPCLDetails.Count > 0)
                    {
                        foreach(var detail in item.RefTreatmentRegimenPCLDetails)
                        {
                            TreatmentRegimenPCLDetail = ListPclExamTypesAllPCLForms.Where(x => x.PCLExamTypeID == detail.PCLExamTypeID).FirstOrDefault();
                            if (TreatmentRegimenPCLDetail != null && !listpcl.Any(x => x.PCLExamTypeID == detail.PCLExamTypeID))
                            {
                                listpcl.Add(TreatmentRegimenPCLDetail);
                            }
                            foreach(var detaildic in dicPCLExamType)
                            {
                                if (detaildic.Value.PCLExamTypeID == detail.PCLExamTypeID)
                                {
                                    if (dicPCLExamTypeForTreatmentRegimen == null)
                                    {
                                        dicPCLExamTypeForTreatmentRegimen = new Dictionary<long, PCLExamType>();
                                    }
                                    if (!dicPCLExamTypeForTreatmentRegimen.ContainsKey(detaildic.Value.PCLExamTypeID))
                                    {
                                        dicPCLExamTypeForTreatmentRegimen.Add(detaildic.Value.PCLExamTypeID, detaildic.Value);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                listpcl = ListPclExamTypesAllPCLForms;
                listpclCombo = Globals.ListPclExamTypesAllCombos;
            }
            //▲===== #001
            if (listpcl != null && listpcl.Count > 0)
            {
                if (!string.IsNullOrEmpty(str))
                {
                    PclExamTypes.Clear();
                    //var res = (from c in listpcl
                    //           where //c.PCLFormID == SelectedPCLForm.PCLFormID
                    //               //&& 
                    //           VNConvertString.ConvertString(c.PCLExamTypeName)
                    //           .ToLower().StartsWith(VNConvertString.ConvertString(str).ToLower()
                    //           )
                    //           select c);

                    //foreach (var item in res)
                    //{
                    //    PclExamTypes.Add(item);
                    //}
                    List<PCLExamType> resComBo = new List<PCLExamType>();
                    if (CurrentInPatientAdmDisDetail != null && CurrentInPatientAdmDisDetail.PCLExamTypePriceListID > 0)
                    {
                        resComBo = (from c in listpcl
                                    where !string.IsNullOrEmpty(c.PCLExamTypeName) && VNConvertString.ConvertString(c.PCLExamTypeName).ToLower().Contains(VNConvertString.ConvertString(str).ToLower())
                                    select c).ToList();
                    }
                    else
                    {
                        //▼===== #001
                        if (dicPCLExamTypeForTreatmentRegimen != null)
                        {
                            resComBo = (from c in dicPCLExamTypeForTreatmentRegimen
                                        where c.Value != null && (VNConvertString.ConvertString(c.Value.PCLExamTypeName).ToLower().Contains(VNConvertString.ConvertString(str).ToLower()))
                                        select c.Value).ToList();
                        }
                        else
                        {
                            resComBo = (from c in dicPCLExamType
                                        where c.Value != null && (VNConvertString.ConvertString(c.Value.PCLExamTypeName).ToLower().Contains(VNConvertString.ConvertString(str).ToLower()))
                                        select c.Value).ToList();
                        }
                        //▲===== #001
                    }
                    foreach (var item in resComBo)
                    {
                        PclExamTypes.Add(item);
                    }
                }
                else
                {
                    var res = (from c in listpcl where c.PCLFormID == SelectedPCLForm.PCLFormID select c);
                    PclExamTypes.Clear();
                    foreach (var item in res)
                    {
                        PclExamTypes.Add(item);
                    }
                }
            }
            else
            {
                if (PclExamTypes != null)
                {
                    PclExamTypes.Clear();
                }
            }
        }

        private Lookup _ObjV_PCLMainCategory_Selected;
        public Lookup ObjV_PCLMainCategory_Selected
        {
            get { return _ObjV_PCLMainCategory_Selected; }
            set
            {
                _ObjV_PCLMainCategory_Selected = value;
                NotifyOfPropertyChange(() => ObjV_PCLMainCategory_Selected);
                if (_ObjV_PCLMainCategory_Selected != null)
                {
                    if (PCLForms != null)
                    {
                        SelectedPCLForm = PCLForms.FirstOrDefault();
                    }
                    if (PCLExamTypeCombos != null)
                    {
                        SelectedCombo = PCLExamTypeCombos.FirstOrDefault();
                    }
                    SetVisibility();
                }
            }
        }

        private Visibility _VisibilityCombo = Visibility.Collapsed;
        public Visibility VisibilityCombo
        {
            get { return _VisibilityCombo; }
            set
            {
                _VisibilityCombo = value;
                NotifyOfPropertyChange(() => VisibilityCombo);
            }
        }
        private Visibility _VisibilityFrom = Visibility.Visible;
        public Visibility VisibilityFrom
        {
            get { return _VisibilityFrom; }
            set
            {
                _VisibilityFrom = value;
                NotifyOfPropertyChange(() => VisibilityFrom);
            }
        }

        private void SetVisibility()
        {
            if (ObjV_PCLMainCategory_Selected.LookupID == IDLABByName_Bo)
            {
                VisibilityFrom = Visibility.Collapsed;
                VisibilityCombo = Visibility.Visible;
            }
            else
            {
                VisibilityFrom = Visibility.Visible;
                VisibilityCombo = Visibility.Collapsed;
            }
        }


        private ObservableCollection<Lookup> _ObjV_PCLMainCategory;
        public ObservableCollection<Lookup> ObjV_PCLMainCategory
        {
            get { return _ObjV_PCLMainCategory; }
            set
            {
                _ObjV_PCLMainCategory = value;
                NotifyOfPropertyChange(() => ObjV_PCLMainCategory);
            }
        }

        private long IDLABByName_Bo = 28889;
        private InPatientAdmDisDetails _CurrentInPatientAdmDisDetail;
        public InPatientAdmDisDetails CurrentInPatientAdmDisDetail
        {
            get
            {
                return _CurrentInPatientAdmDisDetail;
            }
            set
            {
                if (_CurrentInPatientAdmDisDetail == value)
                {
                    return;
                }
                if (_CurrentInPatientAdmDisDetail != null && SelectedPCLForm != null && PCLForms != null)
                {
                    if (value == null || value.PCLExamTypePriceListID != _CurrentInPatientAdmDisDetail.PCLExamTypePriceListID)
                    {
                        SelectedPCLForm = PCLForms.FirstOrDefault();
                    }
                }
                if (_CurrentInPatientAdmDisDetail != null && SelectedCombo != null && PCLExamTypeCombos != null)
                {
                    if (value == null || value.PCLExamTypePriceListID != _CurrentInPatientAdmDisDetail.PCLExamTypePriceListID)
                    {
                        SelectedCombo = PCLExamTypeCombos.FirstOrDefault();
                    }
                }
                _CurrentInPatientAdmDisDetail = value;
            }
        }
        public void LoadV_PCLMainCategory()
        {
            List<Lookup> ObjList = new List<Lookup>();

            Lookup Item0 = new Lookup();
            Item0.LookupID = -1;/**/
            Item0.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2034_G1_ChonLoai2);
            ObjList.Add(Item0);

            Lookup Item5 = new Lookup();
            Item5.LookupID = IDLABByName_Bo;
            Item5.ObjectValue = eHCMSResources.Z0182_G1_XNTheoCombo;// "Laboratory(theo Form)";
            ObjList.Add(Item5);


            Lookup Item2 = new Lookup();
            Item2.LookupID = (long)AllLookupValues.V_PCLMainCategory.Laboratory;
            Item2.ObjectValue = eHCMSResources.Z0184_G1_XNTheoNhom;// "Laboratory(theo Form)";
            ObjList.Add(Item2);

            ObjV_PCLMainCategory = new ObservableCollection<Lookup>(ObjList);
            ObjV_PCLMainCategory_Selected = new Lookup();
            ObjV_PCLMainCategory_Selected.LookupID = -1;
        }

        //public void LoadPCLMainCategory()
        //{
        //    var t = new Thread(() =>
        //    {
        //        Globals.EventAggregator.Publish(new BusyEvent
        //        {
        //            IsBusy = true,
        //            Message = "Danh Sách Loại..."
        //        });
        //        AxErrorEventArgs error = null;
        //        try
        //        {
        //            using (var serviceFactory = new CommonServiceClient())
        //            {
        //                var contract = serviceFactory.ServiceInstance;

        //                contract.BeginGetAllLookupValuesByType(LookupValues.V_PCLMainCategory,
        //                    Globals.DispatchCallback(asyncResult =>
        //                    {
        //                        try
        //                        {
        //                            var allItems = contract.EndGetAllLookupValuesByType(asyncResult);

        //                            _pclCategories = new ObservableCollection<Lookup>(allItems);
        //                            var firstItem = new Lookup {LookupID = -1, ObjectValue = "-- Chọn một giá trị --"};
        //                            _pclCategories.Insert(0, firstItem);
        //                            NotifyOfPropertyChange(()=>PCLCategories);
        //                        }
        //                        catch (FaultException<AxException> fault)
        //                        {
        //                            error = new AxErrorEventArgs(fault);
        //                            Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
        //                        }
        //                        catch (Exception innerEx)
        //                        {
        //                            error = new AxErrorEventArgs(innerEx);
        //                            Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
        //                        }
        //                        finally
        //                        {
        //                            Globals.IsBusy = false;
        //                        }

        //                    }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            error = new AxErrorEventArgs(ex);
        //            Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
        //        }
        //        finally
        //        {
        //            Globals.IsBusy = false;
        //        }
        //    });
        //    t.Start();
        //}

        public void gridPcl_DoubleClick(object eventArgs)
        {
            Globals.EventAggregator.Publish(new DoubleClickAddReqLAB { Source = this, EventArgs = eventArgs as EventArgs<object> });
        }
        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            ReadOnlyDataGrid dtgList = (sender as ReadOnlyDataGrid);
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        private string _PCLExamTypeName;
        public string PCLExamTypeName
        {
            get { return _PCLExamTypeName; }
            set
            {
                if (_PCLExamTypeName != value)
                {
                    _PCLExamTypeName = value;
                    NotifyOfPropertyChange(() => PCLExamTypeName);
                }
            }

        }

        public void PCLExamTypeName_KeyUp(object sender, KeyEventArgs e)
        {
            //if (SelectedPCLForm != null && SelectedPCLForm.PCLFormID > 0)
            {
                TextBox ctr = (sender as TextBox);
                string str = ctr.Text.Trim();
                if (CurrentInPatientAdmDisDetail != null && CurrentInPatientAdmDisDetail.PCLExamTypePriceListID > 0)
                {
                    LoadAllPclExamTypesAction(str);
                }
                else
                {
                    LoadPCLExamTypesLinq(str, Globals.ListPclExamTypesAllPCLForms);
                }
            }
            //else
            //{
            //    MessageBox.Show(eHCMSResources.K2074_G1_ChonNhom2);
            //}
        }
        private ObservableCollection<PCLExamTypeCombo> _PCLExamTypeCombos;
        public ObservableCollection<PCLExamTypeCombo> PCLExamTypeCombos
        {
            get { return _PCLExamTypeCombos; }
            set
            {
                _PCLExamTypeCombos = value;
                NotifyOfPropertyChange(() => PCLExamTypeCombos);
            }
        }
        private void PCLPCLExamTypeCombo_Search()
        {
            var t = new Thread(() =>
            {
                IsLoadingCombo = true;

                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLExamTypeCombo_Search(new GeneralSearchCriteria(), Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var allItems = client.EndPCLExamTypeCombo_Search(asyncResult);
                                PCLExamTypeCombos = allItems.ToObservableCollection();
                                DataEntities.PCLExamTypeCombo ItemDefault = new DataEntities.PCLExamTypeCombo();
                                ItemDefault.PCLExamTypeComboID = -1;

                                ItemDefault.ComboName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0186_G1_ChonComboXN);
                                PCLExamTypeCombos.Insert(0, ItemDefault);
                            }
                            catch (Exception innerEx)
                            {
                                ClientLoggerHelper.LogInfo(innerEx.ToString());
                            }

                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
                finally
                {
                    IsLoadingCombo = false;
                }
            });
            t.Start();
        }
        public void PCLExamType_ByComboID(long ID, IList<PCLExamTypeComboItem> ListPclExamTypesAllCombos)
        {
            //▼===== #001
            IList<PCLExamTypeComboItem> listpcl = new List<PCLExamTypeComboItem>();
            ObservableCollection<PCLExamTypeComboItem> listpclCombo = new ObservableCollection<PCLExamTypeComboItem>();
            if (IsRegimenChecked && ListRegiment != null)
            {
                PCLExamTypeComboItem TreatmentRegimenPCLDetail = new PCLExamTypeComboItem();
                foreach (var item in ListRegiment)
                {
                    if (item.RefTreatmentRegimenPCLDetails != null && item.RefTreatmentRegimenPCLDetails.Count > 0)
                    {
                        foreach (var detail in item.RefTreatmentRegimenPCLDetails)
                        {
                            TreatmentRegimenPCLDetail = ListPclExamTypesAllCombos.Where(x => x.PCLExamTypeID == detail.PCLExamTypeID).FirstOrDefault();
                            if (TreatmentRegimenPCLDetail != null && !listpcl.Any(x => x.PCLExamTypeID == detail.PCLExamTypeID))
                            {
                                listpcl.Add(TreatmentRegimenPCLDetail);
                            }
                        }
                    }
                }
            }
            else
            {
                listpcl = ListPclExamTypesAllCombos;
            }
            //▲===== #001
            if (listpcl != null && listpcl.Count > 0)
            {
                var pclexamtypes = (from p in listpcl
                                    where p.PCLExamTypeComboID == ID
                                    select p.PCLExamType);

                PclExamTypes.Clear();

                foreach (var item in pclexamtypes)
                {
                    PclExamTypes.Add(item);
                }
            }
            else
            {
                if (PclExamTypes != null)
                    PclExamTypes.Clear();
            }
        }
        private void LoadAllPclExamTypesAction(string str)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        var criteria = new PCLExamTypeSearchCriteria { V_PCLMainCategory = 0 };
                        client.BeginPCLItems_ByPCLFormID(criteria, 0, CurrentInPatientAdmDisDetail.PCLExamTypePriceListID, Globals.DispatchCallback(delegate (IAsyncResult asyncResult)
                        {
                            try
                            {
                                List<PCLExamType> ListAllPclExamTypes = new List<PCLExamType>();
                                ListAllPclExamTypes = client.EndPCLItems_ByPCLFormID(asyncResult).ToList();
                                List<PCLExamType> listpcl = new List<PCLExamType>();
                                foreach (PCLExamType pclItem in ListAllPclExamTypes)
                                {
                                    if (pclItem.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
                                    {
                                        listpcl.Add(pclItem);
                                    }
                                }
                                LoadPCLExamTypesLinq(str, listpcl);
                            }
                            catch
                            {
                            }
                            finally
                            {
                            }
                        }), null);
                    }
                }
                catch
                {
                }
            });
            t.Start();
        }
        private void LoadAllPCLExamTypesByComboIDAction(long PCLExamTypeComboID)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLExamTypeComboItems_All(CurrentInPatientAdmDisDetail.PCLExamTypePriceListID, Globals.DispatchCallback(delegate (IAsyncResult asyncResult)
                        {
                            try
                            {
                                IList<PCLExamTypeComboItem> allItems = client.EndPCLExamTypeComboItems_All(asyncResult);
                                if (allItems == null)
                                {
                                    allItems = new List<PCLExamTypeComboItem>();
                                }
                                PCLExamType_ByComboID(_selectedCombo.PCLExamTypeComboID, allItems);
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogError(ex.ToString());
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogError(ex.ToString());
                }
            });
            t.Start();
        }
        //▼===== #001
        private bool _IsRegimenChecked;
        public bool IsRegimenChecked
        {
            get
            {
                return _IsRegimenChecked;
            }
            set
            {
                _IsRegimenChecked = value;
                NotifyOfPropertyChange(() => IsRegimenChecked);
            }
        }
        private List<RefTreatmentRegimen> _ListRegiment = new List<RefTreatmentRegimen>();
        public List<RefTreatmentRegimen> ListRegiment
        {
            get { return _ListRegiment; }
            set
            {
                if (_ListRegiment != value)
                {
                    _ListRegiment = value;
                    NotifyOfPropertyChange(() => ListRegiment);
                }
            }
        }
        //▲===== #001
    }
}