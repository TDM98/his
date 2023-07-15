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
using Caliburn.Micro;
using aEMR.ViewContracts;
using System.ComponentModel.Composition;
using DataEntities;
using aEMR.Common;
using aEMR.Controls;
using System.Linq;
using Castle.Windsor;
using System.Collections.Generic;

namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IInPatientSelectPcl)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InPatientSelectPclViewModel : Conductor<object>, IInPatientSelectPcl
    {
        [ImportingConstructor]
        public InPatientSelectPclViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
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
                    LoadPCLExamTypes();
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
                    SearchCriteria = new PCLFormsSearchCriteria { V_PCLMainCategory = (long)AllLookupValues.V_PCLMainCategory.Imaging, OrderBy = "" };

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
        public InPatientAdmDisDetails CurrentInPatientAdmDisDetail { get; set; }
        private void LoadPCLExamTypes()
        {
            var listpcl = Globals.ListPclExamTypesAllPCLFormImages;
            if (listpcl != null && listpcl.Count > 0)
            {
                var res = (from c in listpcl
                           where c.PCLFormID == SelectedPCLForm.PCLFormID
                           select c);

                PclExamTypes.Clear();

                foreach (var item in res)
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
    }
}