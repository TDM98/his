using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel;
using System.Threading;
using System.ComponentModel.Composition;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;


namespace aEMR.Common.ViewModels
{
    //Hien gio khong su dung.
    //[Export(typeof (IPclExamTypeListing)),PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(IPclExamTypeListing)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PclExamTypeListingViewModel : Conductor<object>, IPclExamTypeListing
    {
        public PclExamTypeListingViewModel()
        {
            PclExamTypes = new PagedSortableCollectionView<PCLExamType>();
            PclExamTypes.OnRefresh += new EventHandler<aEMR.Common.Collections.RefreshEventArgs>(PclExamTypes_OnRefresh);

            _searchCriteria = new PCLExamTypeSearchCriteria();
        }

        void PclExamTypes_OnRefresh(object sender, aEMR.Common.Collections.RefreshEventArgs e)
        {
            LoadPCLExamTypes(PclExamTypes.PageIndex, PclExamTypes.PageSize, false);
        }

        private PCLExamTypeSearchCriteria _criteria;
        private PCLExamTypeSearchCriteria _searchCriteria;
        public PCLExamTypeSearchCriteria SearchCriteria
        {
            get { return _searchCriteria; }
            private set
            {
                if (_searchCriteria != value)
                {
                    _searchCriteria = value;
                    NotifyOfPropertyChange(() => SearchCriteria);
                    //LoadPCLItems(PCLItemList.PageIndex, PCLItemList.PageSize, true);
                }
            }
        }


        private PCLExamType _selectedPCLExamType;

        public PCLExamType SelectedPCLExamType
        {
            get { return _selectedPCLExamType; }
            set
            {
                _selectedPCLExamType = value;
                NotifyOfPropertyChange(() => SelectedPCLExamType);
                Globals.EventAggregator.Publish(new ItemSelected<PCLExamType>() { Item = _selectedPCLExamType });
            }
        }

        private PagedSortableCollectionView<PCLExamType> _pclExamTypes;

        public PagedSortableCollectionView<PCLExamType> PclExamTypes
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
                if(!_isLoadingPclExamTypes)
                {
                    _isLoadingPclExamTypes = value;
                    NotifyOfPropertyChange(()=>IsLoadingPclExamTypes);
                }
            }
        }
        private void LoadPCLExamTypes(int pageIndex, int pageSize, bool bCountTotal)
        {
            var t = new Thread(() =>
            {
                if (_criteria == null)
                {
                    _criteria = _searchCriteria;
                }
                IsLoadingPclExamTypes = true;
                //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0180_G1_DangTimDSDVCLS) });
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetAllActiveExamTypesPaging(_criteria, pageIndex, pageSize, bCountTotal,_criteria.OrderBy, Globals.DispatchCallback((asyncResult) =>
                        {
                            int totalCount = 0;
                            IList<PCLExamType> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndGetAllActiveExamTypesPaging(out totalCount, asyncResult);
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

                            PclExamTypes.Clear();
                            if (bOK)
                            {
                                if (bCountTotal)
                                {
                                    PclExamTypes.TotalItemCount = totalCount;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        PclExamTypes.Add(item);
                                    }
                                }
                                var currentView = this.GetView() as IPclExamTypeListingView;
                                if(currentView != null)
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
                }
                finally
                {
                    IsLoadingPclExamTypes = false;
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
            _criteria = _searchCriteria.DeepCopy();

            PclExamTypes.PageIndex = 0;
            LoadPCLExamTypes(PclExamTypes.PageIndex, PclExamTypes.PageSize,true);
        }

        public void ClearItems()
        {
            PclExamTypes.Clear();
            PclExamTypes.TotalItemCount = 0;
        }

        public void PopulatingCmd(object source, EventArgs eventArgs)
        {
            var currentView = this.GetView() as IPclExamTypeListingView;
            if (currentView != null)
            {
                if (SearchCriteria.PCLExamTypeName != currentView.PclExamTypes.SearchText)
                {
                    SearchCriteria.PCLExamTypeName = currentView.PclExamTypes.SearchText;
                    StartSearching();
                }
            }
           
        }
    }
}