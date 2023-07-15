using System;
using System.Windows;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
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
using eHCMSLanguage;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(ISearchRegistration)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class SearchRegistrationViewModel : Conductor<object>, ISearchRegistration
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SearchRegistrationViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            ResetFilter();
            //if (!DesignerProperties.IsInDesignTool)
            bool designTime = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());
            if (!designTime)
            {
                GetMaxExamDate();
            }
            Registrations = new PagedSortableCollectionView<PatientRegistration>();
            Registrations.OnRefresh += Registrations_OnRefresh;
        }

        void Registrations_OnRefresh(object sender, aEMR.Common.Collections.RefreshEventArgs e)
        {
            SearchRegistrations(Registrations.PageIndex, Registrations.PageSize, false);
        }
        protected void ResetFilter()
        {
            SearchCriteria = new SeachPtRegistrationCriteria();
            
            SearchCriteria.FromDate = Globals.ServerDate.Value;
            SearchCriteria.ToDate = Globals.ServerDate.Value;
        }
        public void CancelCmd()
        {
            SelectedRegistration = null;
            TryClose();
        }

        public void OkCmd()
        {
            Globals.EventAggregator.Publish(new ItemSelected<PatientRegistration>() { Item = SelectedRegistration });
            TryClose();
        }

        private PatientRegistration _selectedRegistration;

        public PatientRegistration SelectedRegistration
        {
            get { return _selectedRegistration; }
            set
            {
                _selectedRegistration = value;
                NotifyOfPropertyChange(()=>SelectedRegistration);
            }
        }

        private SeachPtRegistrationCriteria _searchCriteria;
        public SeachPtRegistrationCriteria SearchCriteria
        {
            get { return _searchCriteria; }
            set 
            {
                _searchCriteria = value; 
                NotifyOfPropertyChange(()=>SearchCriteria);
            }
        }

        private PagedSortableCollectionView<PatientRegistration> _registrations;
        public PagedSortableCollectionView<PatientRegistration> Registrations
        {
            get { return _registrations; }
            private set
            {
                _registrations = value;
                NotifyOfPropertyChange(()=>Registrations);
            }
        }

        /// <summary>
        /// Bien nay dung de tim kiem dang ky.
        /// Khi nguoi dung thay doi thong tin tim kiem tren form thi khong set lai bien nay.
        /// Khi nao nguoi dung bat dau search thi moi lay gia tri cua SearchCriteria gan cho no.
        /// </summary>
        private SeachPtRegistrationCriteria _criteria;

        public void SearchCmd()
        {
            _criteria = _searchCriteria.DeepCopy();
            _criteria.PatientFindBy = Globals.PatientFindBy_ForConsultation!=null?Globals.PatientFindBy_ForConsultation.Value: AllLookupValues.PatientFindBy.NGOAITRU;
            SearchRegistrations(0, Registrations.PageSize, true);
        }
        private void SearchRegistrations(int pageIndex, int pageSize, bool bCountTotal)
        {
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0601_G1_DangTimDK) });
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginSearchPtRegistration(_criteria, pageSize, pageIndex, bCountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int totalCount = 0;
                            IList<PatientRegistration> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndSearchPtRegistration(out totalCount, asyncResult);
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

                            Registrations.Clear();
                            if (bOK)
                            {
                                if (bCountTotal)
                                {
                                    Registrations.TotalItemCount = totalCount;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        Registrations.Add(item);
                                    }

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
                    Globals.IsBusy = false;
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                }
            });
            t.Start();
        }
        public void ResetFilterCmd()
        {
            ResetFilter();
        }
        public void DoubleClick(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;
            SelectedRegistration = eventArgs.Value as PatientRegistration;

            Globals.EventAggregator.Publish(new ItemSelected<PatientRegistration>() { Item = SelectedRegistration });
            TryClose();
            Globals.EventAggregator.Unsubscribe(this);
        }
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                NotifyOfPropertyChange(()=>IsLoading);
            }
        }
        private void GetMaxExamDate()
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetMaxExamDate(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                DateTime date = client.EndGetMaxExamDate(asyncResult);
                                SearchCriteria.FromDate = date;
                                SearchCriteria.ToDate = date;

                                SearchCmd();
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
            });
            t.Start();
        }
    }
}
