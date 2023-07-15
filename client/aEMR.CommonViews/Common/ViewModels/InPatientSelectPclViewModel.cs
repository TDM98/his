using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Threading;
using System.Windows.Controls;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using aEMR.ViewContracts;
using System.ComponentModel.Composition;
using DataEntities;
using aEMR.Common;
using aEMR.Controls;
using System.Linq;
using System.Windows.Input;
using aEMR.Common.Utilities;
using aEMR.Common.Collections;
using System.Collections.Generic;
/*
 * 20181208 #001 TTM: BM 0004209: Cho phép lọc tìm kiếm CLS màn hình đăng ký dịch vụ.
 * 20200725 #002 TTM: BM 0022848: Bổ sung kiểm tra phác đồ cho chỉ định khám bệnh ngoại trú.
 * 20200810 #003 TTM:   BM 0039422: Bổ sung code kiểm tra phác đồ nếu bệnh nhân có nhiều ICD cùng 1 phác đồ thì chỉ insert dữ liệu dịch vụ 1 lần thôi.
 */
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IInPatientSelectPcl)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InPatientSelectPclViewModel : Conductor<object>, IInPatientSelectPcl
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public InPatientSelectPclViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            _pclCategories = new ObservableCollection<Lookup>();

            SelectedPCLCategory = new Lookup();
            SelectedPCLCategory.LookupID = V_PCLMainCategory;

            SelectedPCLForm = new PCLForm();
            SelectedPCLForm.PCLFormID = -1;

            LoadPCLForms(SelectedPCLCategory.LookupID);

            _pclExamTypes = new ObservableCollection<PCLExamType>();

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
                    LoadPCLExamTypes(PCLExamTypeCollectionByCurrentCer);
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
            PclExamTypes.Add(new PCLExamType { PCLExamTypeID = -1, PCLExamTypeName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0162_G1_HayChonDV) });
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
                return _isLoadingPclForm;
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
                return "";
            }
        }
        public long? DeptID { get; set; }

        public void LoadPCLForms(long? pclCategory)
        {
            var t = new Thread(() =>
            {
                IsLoadingPclForm = true;
                Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0178_G1_DSPCLForms) });
                AxErrorEventArgs error = null;
                try
                {
                    SearchCriteria = new PCLFormsSearchCriteria { V_PCLMainCategory = V_PCLMainCategory, OrderBy = "" };
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
                if (value != null && value.MedServiceItemPriceListID > 0)
                {
                    LoadAllPclExamTypesAction();
                }
                else
                {
                    PCLExamTypeCollectionByCurrentCer = Globals.PCLExamTypeCollection;
                }
                _CurrentInPatientAdmDisDetail = value;
            }
        }
        private void LoadPCLExamTypes(IList<PCLExamType> PCLExamTypeCollection)
        {
            //▼===== #002
            List<PCLExamType> listpcl = new List<PCLExamType>();
            if (IsRegimenChecked && ListRegiment != null)
            {
                PCLExamType TreatmentRegimenPCLDetail = new PCLExamType();
                foreach (var item in ListRegiment)
                {
                    if (item.RefTreatmentRegimenPCLDetails != null && item.RefTreatmentRegimenPCLDetails.Count > 0)
                    {
                        foreach (var detail in item.RefTreatmentRegimenPCLDetails)
                        {
                            TreatmentRegimenPCLDetail = PCLExamTypeCollection.Where(x => x.PCLExamTypeID == detail.PCLExamTypeID && x.V_PCLMainCategory == V_PCLMainCategory).FirstOrDefault();
                            //▼===== #003
                            //if (TreatmentRegimenPCLDetail != null)
                            if (TreatmentRegimenPCLDetail != null && !listpcl.Any(x => x.PCLExamTypeID == detail.PCLExamTypeID))
                            //▲===== #003
                            {
                                listpcl.Add(TreatmentRegimenPCLDetail);
                            }
                        }
                    }
                }
            }
            else
            {
                listpcl = PCLExamTypeCollection.Where(x => x.V_PCLMainCategory == V_PCLMainCategory).ToList();
            }
            //var listpcl = PCLExamTypeCollection.Where(x => x.V_PCLMainCategory == V_PCLMainCategory);
            //▲===== #002
            PclExamTypes = new ObservableCollection<PCLExamType>();
            if (listpcl != null && listpcl.Count() > 0 && SelectedPCLForm != null)
            {
                var res = (from c in listpcl
                           where c.PCLFormID == SelectedPCLForm.PCLFormID
                           select c);
                foreach (var item in res)
                {
                    PclExamTypes.Add(item);
                }
            }
        }

        //private void LoadPCLExamTypes()
        //{
        //    //_criteria.PCLGroupID = _selectedPCLGroup != null ? (long?) _selectedPCLGroup.PCLGroupID : null;
        //    //_criteria.PclCategory = _selectedPCLCategory != null ? (long?)_selectedPCLCategory.LookupID : null;

        //    var t = new Thread(() =>
        //    {
        //        Globals.ShowBusy("Đang tìm danh sách dịch vụ CLS...");
        //        IsLoadingPclExamTypes = true;
        //       // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang tìm danh sách dịch vụ CLS..." });
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
        //           // Globals.IsBusy = false;
        //            Globals.HideBusy();
        //        }
        //        if (error != null)
        //        {
        //            Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
        //        }
        //    });
        //    t.Start();
        //}
        public void LoadPCLMainCategory()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                //Globals.EventAggregator.Publish(new BusyEvent
                //{
                //    IsBusy = true,
                //    Message = "Danh Sách Loại..."
                //});
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllLookupValuesByType(LookupValues.V_PCLMainCategory,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    var allItems = contract.EndGetAllLookupValuesByType(asyncResult);

                                    _pclCategories = new ObservableCollection<Lookup>(allItems);
                                    var firstItem = new Lookup { LookupID = -1, ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0111_G1_HayChon1GTri) };
                                    _pclCategories.Insert(0, firstItem);
                                    NotifyOfPropertyChange(() => PCLCategories);
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    error = new AxErrorEventArgs(fault);
                                    Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
                                }
                                catch (Exception innerEx)
                                {
                                    error = new AxErrorEventArgs(innerEx);
                                    Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
                                }
                                finally
                                {
                                    //Globals.IsBusy = false;
                                }

                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                    Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        public void gridPcl_DoubleClick(object eventArgs)
        {
            Globals.EventAggregator.Publish(new DoubleClick { Source = this, EventArgs = eventArgs as EventArgs<object> });
        }
        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            ReadOnlyDataGrid dtgList = (sender as ReadOnlyDataGrid);
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        //▼====== #001
        public void PCLExamTypeName_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox ctr = (sender as TextBox);
            string str = ctr.Text.Trim();
            LoadPCLExamTypesLinq(str, PCLExamTypeCollectionByCurrentCer);
            //LoadPCLExamTypesLinq_V2(str);
        }
        private void LoadPCLExamTypesLinq(string str, IList<PCLExamType> PCLExamTypeCollection)
        { 
            //▼===== #002
            List<PCLExamType> ListPCLImage = new List<PCLExamType>();
            if (IsRegimenChecked && ListRegiment != null)
            {
                PCLExamType TreatmentRegimenPCLDetail = new PCLExamType();
                foreach (var item in ListRegiment)
                {
                    if (item.RefTreatmentRegimenPCLDetails != null && item.RefTreatmentRegimenPCLDetails.Count > 0)
                    {
                        foreach (var detail in item.RefTreatmentRegimenPCLDetails)
                        {
                            TreatmentRegimenPCLDetail = PCLExamTypeCollection.Where(x => x.PCLExamTypeID == detail.PCLExamTypeID && x.V_PCLMainCategory == V_PCLMainCategory).FirstOrDefault();
                            //▼===== #003
                            //if (TreatmentRegimenPCLDetail != null)
                            if (TreatmentRegimenPCLDetail != null && !ListPCLImage.Any(x => x.PCLExamTypeID == detail.PCLExamTypeID))
                            //▲===== #003
                            {
                                ListPCLImage.Add(TreatmentRegimenPCLDetail);
                            }
                        }
                    }
                }
            }
            else
            {
                ListPCLImage = PCLExamTypeCollection.Where(x => x.V_PCLMainCategory == V_PCLMainCategory).ToList();
            }
            //var ListPCLImage = PCLExamTypeCollection.Where(x => x.V_PCLMainCategory == V_PCLMainCategory);
            //▲===== #002
            ObservableCollection<PCLExamType> ListPCLImageTMP = new ObservableCollection<PCLExamType>();
            if (PclExamTypes != null)
            {
                PclExamTypes.Clear();
            }
            else
            {
                PclExamTypes = new ObservableCollection<PCLExamType>();
            }
            if (ListPCLImage == null || ListPCLImage.Count() == 0)
            {
                return;
            }
            if (string.IsNullOrEmpty(str))
            {
                long CurrentPCLExamTypeID = 0;
                foreach (var TmpListPCLExamType in ListPCLImage)
                {
                    if (SelectedPCLForm == null || SelectedPCLForm.PCLFormID == -1)
                    {
                        if (CurrentPCLExamTypeID != TmpListPCLExamType.PCLExamTypeID)
                        {
                            PclExamTypes.Add(TmpListPCLExamType);
                        }
                        CurrentPCLExamTypeID = TmpListPCLExamType.PCLExamTypeID;
                    }
                    else if (TmpListPCLExamType.PCLFormID == SelectedPCLForm.PCLFormID)
                    {
                        PclExamTypes.Add(TmpListPCLExamType);
                    }
                }
            }
            else
            {
                long CurrentPCLExamTypeID = 0;
                foreach (var TmpListPCLExamType in ListPCLImage)
                {
                    if (SelectedPCLForm == null || SelectedPCLForm.PCLFormID == -1)
                    {
                        if (CurrentPCLExamTypeID != TmpListPCLExamType.PCLExamTypeID)
                        {
                            ListPCLImageTMP.Add(TmpListPCLExamType);
                        }
                        CurrentPCLExamTypeID = TmpListPCLExamType.PCLExamTypeID;
                    }
                    else if (TmpListPCLExamType.PCLFormID == SelectedPCLForm.PCLFormID)
                    {
                        ListPCLImageTMP.Add(TmpListPCLExamType);
                    }
                }
                foreach (var TmpListPCLExamType_V2 in ListPCLImageTMP)
                {
                    if (VNConvertString.ConvertString(TmpListPCLExamType_V2.PCLExamTypeName).ToUpper().Contains(VNConvertString.ConvertString(str).ToUpper()))
                    {
                        PclExamTypes.Add(TmpListPCLExamType_V2);
                    }
                }
            }
        }
        ////20190714 #001 TTM: Thay đổi nếu tìm kiếm bằng cách gõ Textbox thì không cần quan tâm loại mà tìm tất cả các loại có ký tự phù hợp.
        //private void LoadPCLExamTypesLinq_V2(string str)
        //{
        //    ObservableCollection<PCLExamType> ListPCLImage = Globals.ListPclExamTypesAllPCLFormImages;
        //    ObservableCollection<PCLExamType> ListPCLImageTMP = new ObservableCollection<PCLExamType>();
        //    if (ListPCLImage != null && ListPCLImage.Count > 0)
        //    {
        //        if (!string.IsNullOrEmpty(str))
        //        {
        //            PclExamTypes.Clear();
        //            foreach (PCLExamType tmpPCLImage in ListPCLImage)
        //            {
        //                if (VNConvertString.ConvertString(tmpPCLImage.PCLExamTypeName).ToLower().Contains(VNConvertString.ConvertString(str).ToLower()) && tmpPCLImage.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Imaging)
        //                {
        //                    PclExamTypes.Add(tmpPCLImage);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            PclExamTypes.Clear();
        //            foreach (var TmpListPCLExamType in ListPCLImage)
        //            {
        //                if (TmpListPCLExamType.PCLFormID == SelectedPCLForm.PCLFormID)
        //                {
        //                    PclExamTypes.Add(TmpListPCLExamType);
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (PclExamTypes != null)
        //        {
        //            PclExamTypes.Clear();
        //        }
        //    }
        //}
        //▲====== #001
        private long _V_PCLMainCategory = (long)AllLookupValues.V_PCLMainCategory.Imaging;
        public long V_PCLMainCategory
        {
            get
            {
                return _V_PCLMainCategory;
            }
            set
            {
                if (V_PCLMainCategory == value)
                {
                    return;
                }
                _V_PCLMainCategory = value;
                LoadPCLForms(V_PCLMainCategory);
                NotifyOfPropertyChange(() => V_PCLMainCategory);
            }
        }
        private List<PCLExamType> PCLExamTypeCollectionByCurrentCer = Globals.PCLExamTypeCollection;
        private void LoadAllPclExamTypesAction()
        {
            this.ShowBusyIndicator();
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
                                    listpcl.Add(pclItem);
                                }
                                PCLExamTypeCollectionByCurrentCer = listpcl;
                            }
                            catch
                            {
                            }
                            finally
                            {
                                this.HideBusyIndicator();
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
        //▼===== #002
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
        //▲===== #002
    }
}