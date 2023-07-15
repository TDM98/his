using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Common.BaseModel;
using System.Windows.Media;
/*
* 20191101 #001 TNHX: Init view model
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IInPatientChoosePriceList)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InPatientChoosePriceListViewModel : ViewModelBase, IInPatientChoosePriceList
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public InPatientChoosePriceListViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            MedPricePreEffectDate = Globals.GetCurServerDateTime();
            PCLPricePreEffectDate = Globals.GetCurServerDateTime();

            HasMedServiceList = false;
            HasPCLList = false;
            LoadingCreateDate();

            MedPriceListSearchCriteria = new MedServiceItemPriceListSearchCriteria
            {
                Month = -1,
                Year = DateTime.Now.Year
            };

            ObjMedServiceItemPriceList_GetList_Paging = new PagedSortableCollectionView<MedServiceItemPriceList>();
            ObjMedServiceItemPriceList_GetList_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjMedServiceItemPriceList_GetList_Paging_OnRefresh);
            ObjMedServiceItemPriceList_GetList_Paging.PageIndex = 0;
            ObjMedServiceItemPriceList_GetList_Paging.PageSize = 50;
            MedServiceItemPriceList_GetList_Paging(ObjMedServiceItemPriceList_GetList_Paging.PageIndex, ObjMedServiceItemPriceList_GetList_Paging.PageSize, false);

            PCLPriceListSearchCriteria = new PCLExamTypePriceListSearchCriteria
            {
                Month = -1,
                Year = DateTime.Now.Year
            };

            ObjPCLExamTypePriceList_GetList_Paging = new PagedSortableCollectionView<PCLExamTypePriceList>();
            ObjPCLExamTypePriceList_GetList_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjPCLExamTypePriceList_GetList_Paging_OnRefresh);
            ObjPCLExamTypePriceList_GetList_Paging.PageIndex = 0;
            ObjPCLExamTypePriceList_GetList_Paging.PageSize = 50;
            PCLExamTypePriceList_GetList_Paging(0, ObjPCLExamTypePriceList_GetList_Paging.PageSize, true);
        }

        private bool _HasMedServiceList;
        public bool HasMedServiceList
        {
            get
            {
                return _HasMedServiceList;
            }
            set
            {
                if (_HasMedServiceList != value)
                {
                    _HasMedServiceList = value;
                    NotifyOfPropertyChange(() => HasMedServiceList);
                }
            }
        }

        private bool _HasCheckMedPrice = false;
        public bool HasCheckMedPrice
        {
            get
            {
                return _HasCheckMedPrice;
            }
            set
            {
                if (_HasCheckMedPrice != value)
                {
                    _HasCheckMedPrice = value;
                    NotifyOfPropertyChange(() => HasCheckMedPrice);
                }
            }
        }

        private bool _HasCheckPCLPrice = false;
        public bool HasCheckPCLPrice
        {
            get
            {
                return _HasCheckPCLPrice;
            }
            set
            {
                if (_HasCheckPCLPrice != value)
                {
                    _HasCheckPCLPrice = value;
                    NotifyOfPropertyChange(() => HasCheckPCLPrice);
                }
            }
        }

        private bool _HasPCLList;
        public bool HasPCLList
        {
            get
            {
                return _HasPCLList;
            }
            set
            {
                if (_HasPCLList != value)
                {
                    _HasPCLList = value;
                    NotifyOfPropertyChange(() => HasPCLList);
                }
            }
        }

        private InPatientBillingInvoice _RecalBillingInvoice;
        public InPatientBillingInvoice RecalBillingInvoice
        {
            get
            {
                return _RecalBillingInvoice;
            }
            set
            {
                if (_RecalBillingInvoice != value)
                {
                    _RecalBillingInvoice = value;
                    LoadingCreateDate();
                    NotifyOfPropertyChange(() => RecalBillingInvoice);
                }
            }
        }

        private void LoadingCreateDate()
        {
            if (RecalBillingInvoice != null)
            {
                // get current date of list Medservice
                if (RecalBillingInvoice.RegistrationDetails != null && RecalBillingInvoice.RegistrationDetails.Count() > 0)
                {
                    HasMedServiceList = true;
                    CreateDateMedServiceList = (DateTime)RecalBillingInvoice.RegistrationDetails.FirstOrDefault().RecCreatedDate;
                }
                if (RecalBillingInvoice.PclRequests != null && RecalBillingInvoice.PclRequests.Count() > 0)
                {
                    HasPCLList = true;
                    CreateDatePCLList = RecalBillingInvoice.PclRequests.FirstOrDefault().RecCreatedDate;
                }
            }
        }

        #region Medservice Price List
        private PagedSortableCollectionView<MedServiceItemPriceList> _ObjMedServiceItemPriceList_GetList_Paging;
        public PagedSortableCollectionView<MedServiceItemPriceList> ObjMedServiceItemPriceList_GetList_Paging
        {
            get { return _ObjMedServiceItemPriceList_GetList_Paging; }
            set
            {
                _ObjMedServiceItemPriceList_GetList_Paging = value;
                NotifyOfPropertyChange(() => ObjMedServiceItemPriceList_GetList_Paging);
            }
        }

        private MedServiceItemPriceListSearchCriteria _MedPriceListSearchCriteria;
        public MedServiceItemPriceListSearchCriteria MedPriceListSearchCriteria
        {
            get
            {
                return _MedPriceListSearchCriteria;
            }
            set
            {
                _MedPriceListSearchCriteria = value;
                NotifyOfPropertyChange(() => MedPriceListSearchCriteria);
            }
        }

        private DateTime MedPricePreEffectDate;
        private DateTime PCLPricePreEffectDate;
        private DateTime CreateDateMedServiceList;
        private DateTime CreateDatePCLList;


        public void DtgMedPriceList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.DataContext is MedServiceItemPriceList objRows)
            {
                if (MedPricePreEffectDate != Globals.GetCurServerDateTime()
                    && objRows.EffectiveDate < CreateDateMedServiceList
                    && CreateDateMedServiceList < MedPricePreEffectDate
                    && !HasCheckMedPrice)
                {
                    e.Row.Foreground = new SolidColorBrush(Colors.Blue);
                    objRows.IsChecked = true;
                    HasCheckMedPrice = true;
                }
                else if(objRows.IsActive)
                {
                    e.Row.Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    e.Row.Foreground = new SolidColorBrush(Colors.Gray);
                }
                MedPricePreEffectDate = objRows.EffectiveDate;
            }
        }

        void ObjMedServiceItemPriceList_GetList_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            MedServiceItemPriceList_GetList_Paging(ObjMedServiceItemPriceList_GetList_Paging.PageIndex, ObjMedServiceItemPriceList_GetList_Paging.PageSize, false);
        }

        private void MedServiceItemPriceList_GetList_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            this.DlgShowBusyIndicator(eHCMSResources.K2922_G1_DSBGia);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginMedServiceItemPriceList_GetList_Paging(MedPriceListSearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<MedServiceItemPriceList> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndMedServiceItemPriceList_GetList_Paging(out Total, out DateTime currentDate, asyncResult);
                                bOK = true;
                                ObjMedServiceItemPriceList_GetList_Paging.Clear();
                                if (bOK)
                                {
                                    if (CountTotal)
                                    {
                                        ObjMedServiceItemPriceList_GetList_Paging.TotalItemCount = Total;
                                    }
                                    if (allItems != null)
                                    {
                                        foreach (var item in allItems)
                                        {
                                            ObjMedServiceItemPriceList_GetList_Paging.Add(item);
                                        }
                                    }
                                }
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }
        #endregion

        #region PCL Price List
        public void DtgPCLPriceList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.DataContext is PCLExamTypePriceList objRows)
            {
                if (PCLPricePreEffectDate != Globals.GetCurServerDateTime()
                    && objRows.EffectiveDate < CreateDatePCLList
                    && CreateDatePCLList < PCLPricePreEffectDate
                    && !HasCheckPCLPrice)
                {
                    e.Row.Foreground = new SolidColorBrush(Colors.Blue);
                    objRows.IsChecked = true;
                    HasCheckPCLPrice = true;
                }
                else if (objRows.IsActive)
                {
                    e.Row.Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    e.Row.Foreground = new SolidColorBrush(Colors.Gray);
                }
                PCLPricePreEffectDate = (DateTime)objRows.EffectiveDate;
            }
        }

        private PagedSortableCollectionView<PCLExamTypePriceList> _ObjPCLExamTypePriceList_GetList_Paging;
        public PagedSortableCollectionView<PCLExamTypePriceList> ObjPCLExamTypePriceList_GetList_Paging
        {
            get { return _ObjPCLExamTypePriceList_GetList_Paging; }
            set
            {
                _ObjPCLExamTypePriceList_GetList_Paging = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypePriceList_GetList_Paging);
            }
        }

        private PCLExamTypePriceListSearchCriteria _PCLPriceListSearchCriteria;
        public PCLExamTypePriceListSearchCriteria PCLPriceListSearchCriteria
        {
            get
            {
                return _PCLPriceListSearchCriteria;
            }
            set
            {
                _PCLPriceListSearchCriteria = value;
                NotifyOfPropertyChange(() => PCLPriceListSearchCriteria);
            }
        }

        void ObjPCLExamTypePriceList_GetList_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            PCLExamTypePriceList_GetList_Paging(ObjPCLExamTypePriceList_GetList_Paging.PageIndex, ObjPCLExamTypePriceList_GetList_Paging.PageSize, false);
        }

        private void PCLExamTypePriceList_GetList_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            this.DlgShowBusyIndicator(eHCMSResources.K2922_G1_DSBGia);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLExamTypePriceList_GetList_Paging(PCLPriceListSearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<PCLExamTypePriceList> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndPCLExamTypePriceList_GetList_Paging(out Total, out DateTime currentDate, asyncResult);
                                ObjPCLExamTypePriceList_GetList_Paging.Clear();
                                bOK = true;
                                if (bOK)
                                {
                                    if (CountTotal)
                                    {
                                        ObjPCLExamTypePriceList_GetList_Paging.TotalItemCount = Total;
                                    }
                                    if (allItems != null)
                                    {
                                        foreach (var item in allItems)
                                        {
                                            ObjPCLExamTypePriceList_GetList_Paging.Add(item);
                                        }
                                    }
                                }
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }
        #endregion

        #region
        public void BtnOk()
        {
            // Check MedPriceList, PCLPriceList has multi checked
            List<MedServiceItemPriceList> _checkMedPriceList = ObjMedServiceItemPriceList_GetList_Paging.Where(x => x.IsChecked).ToList();
            List<PCLExamTypePriceList> _checkPCLPriceList = ObjPCLExamTypePriceList_GetList_Paging.Where(x => x.IsChecked).ToList();

            if (HasMedServiceList && (_checkMedPriceList.Count() == 0 || _checkMedPriceList.Count() >= 2))
            {
                MessageBox.Show(eHCMSResources.Z2900_G1_ChonBangGiaDV, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return;
            }
            if (HasPCLList && (_checkPCLPriceList.Count() == 0 || _checkPCLPriceList.Count() >= 2))
            {
                MessageBox.Show(eHCMSResources.Z2900_G1_ChonBangGiaCLS, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return;
            }
            Globals.EventAggregator.Publish(new SelectPriceListForRecalcBillInvoiceEvent()
            {
                MedServiceItemPriceListID = _checkMedPriceList.Count() > 0 ? _checkMedPriceList.First().MedServiceItemPriceListID : 0,
                PCLExamTypePriceListID = _checkPCLPriceList.Count() > 0 ? _checkPCLPriceList.First().PCLExamTypePriceListID : 0,
                DoRecalcHiWithPriceList = (HasMedServiceList || HasPCLList) ? true : false
            });
            TryClose();
        }
        #endregion
    }
}
