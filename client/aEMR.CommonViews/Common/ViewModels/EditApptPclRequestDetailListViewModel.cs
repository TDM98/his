using System.ComponentModel;
using System.Windows.Data;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Service.Core.Common;
using System.Collections.ObjectModel;
using DataEntities;
using System.Windows;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IEditApptPclRequestDetailList)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class EditApptPclRequestDetailListViewModel : Conductor<object>, IEditApptPclRequestDetailList
    {
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }
        private bool _IsEdit;
        public bool IsEdit
        {
            get { return _IsEdit; }
            set
            {
                if (_IsEdit != value)
                {
                    _IsEdit = value;
                    if (_IsEdit)
                    {

                    }
                    NotifyOfPropertyChange(() => IsEdit);
                }
            }
        }
        public ICollectionView FilteredCollection
        {
            get
            {
                var pclList = PCLRequest.ObjPatientApptPCLRequestDetailsList;
                if (!IsEdit)
                {
                    try
                    {
                        int temp = pclList.Count - 1;
                        if (pclList[temp].ObjPCLExamTypes.HIAllowedPrice.GetValueOrDefault() > 0)
                        {
                            pclList[temp].IsCountHI = true;
                        }
                        else
                        {
                            pclList[temp].IsCountHI = false;
                        }
                    }
                    catch
                    {

                    }
                }
                var source = new CollectionViewSource
                {
                    Source = pclList
                };
                if (source.View != null)
                {
                    source.View.Filter = Filter;
                }
                return source.View;
            }
        }

        public bool Filter(object obj)
        {
            var details = (PatientApptPCLRequestDetails)obj;
            return details.EntityState != EntityState.DELETED_PERSITED && details.EntityState != EntityState.DELETED_MODIFIED;
        }

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]

        public EditApptPclRequestDetailListViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }
        private PatientApptPCLRequests _pclRequest;

        public PatientApptPCLRequests PCLRequest
        {
            get { return _pclRequest; }
            set
            {
                _pclRequest = value;
                NotifyOfPropertyChange(() => PCLRequest);
            }
        }
        public void cboSegmentsPCL_SelectionChanged(PatientApptPCLRequestDetails datacontext, object eventargs)
        {
            if (datacontext != null && datacontext.EntityState == EntityState.PERSITED)
            {
                datacontext.EntityState = EntityState.MODIFIED;
            }
        }
        public void cboLocation_SelectionChanged(PatientApptPCLRequestDetails datacontext, object eventargs)
        {
            if (datacontext != null && datacontext.EntityState == EntityState.PERSITED)
            {
                datacontext.EntityState = EntityState.MODIFIED;
            }
        }

        public void hplDelete_Click(PatientApptPCLRequestDetails requestDetail)
        {
            if (requestDetail.PCLReqItemID > 0)
            {
                requestDetail.EntityState = EntityState.DELETED_MODIFIED;
            }
            else
            {
                PCLRequest.ObjPatientApptPCLRequestDetailsList.Remove(requestDetail);
            }
            RefreshView();
        }

        public void tbQty_LostFocus(object sender, RoutedEventArgs e)
        {
            var Ctr = (sender as TextBox);

            if (Ctr != null)
            {
                var objEdit = Ctr.DataContext as PatientApptPCLRequestDetails;
                if (objEdit != null)
                {
                    objEdit.EntityState = EntityState.MODIFIED;

                }
            }
        }

        public event EventHandler detailListChanged;
        public void RefreshView()
        {
            NotifyOfPropertyChange(() => FilteredCollection);
            if (detailListChanged != null)
            {
                detailListChanged(PCLRequest != null && PCLRequest.ObjPatientApptPCLRequestDetailsList != null
                    && PCLRequest.ObjPatientApptPCLRequestDetailsList.Count > 0 ? true : false, null);
            }
        }

        public void ckbIsCountHI_CheckedChanged(object sender, RoutedEventArgs e)
        {


            //PatientReg = new PatientRegistration();
            //PatientReg = Registration_DataStorage.CurrentPatientRegistration;
            CheckBox ckbIsCountHI = sender as CheckBox;
            if (!(ckbIsCountHI.DataContext is PatientApptPCLRequestDetails))
            {
                e.Handled = true;
                return;
            }
            
            PatientApptPCLRequestDetails datacontext = (PatientApptPCLRequestDetails)ckbIsCountHI.DataContext;
            if (datacontext != null && datacontext.EntityState == EntityState.PERSITED)
            {
                datacontext.EntityState = EntityState.MODIFIED;
            }
            datacontext.IsCountHI = ckbIsCountHI.IsChecked.GetValueOrDefault(true);
            //foreach (var item in PCLRequest.ObjPatientApptPCLRequestDetailsList)
            //{
            //    if (item.PCLExamTypeID == datacontext.PCLExamTypeID)
            //    {
            //        item.IsCountHI = ckbIsCountHI.IsChecked.GetValueOrDefault(true);
            //    }
            //}

            //PCLRequest.ObjPatientApptPCLRequestDetailsList. = datacontext;
            //MedRegItemBase mUpdateInvoiceItem = (MedRegItemBase)ckbIsCountHI.DataContext;
            //mUpdateInvoiceItem.IsCountHI = ckbIsCountHI.IsChecked.GetValueOrDefault(true);
            //mUpdateInvoiceItem.HIBenefit = mUpdateInvoiceItem.IsCountHI ? PatientReg.PtInsuranceBenefit : null;
            //mUpdateInvoiceItem.HisID = mUpdateInvoiceItem.IsCountHI ? PatientReg.HisID : null;
            //mUpdateInvoiceItem.HIAllowedPrice = mUpdateInvoiceItem.IsCountHI && mUpdateInvoiceItem.ChargeableItem != null ? mUpdateInvoiceItem.ChargeableItem.HIAllowedPrice : null;
            //mUpdateInvoiceItem.CreatedDate = Globals.GetCurServerDateTime();
            //CommonGlobals.ChangeHIBenefit(PatientReg, (ckbIsCountHI.DataContext as MedRegItemBase), mUpdateInvoiceItem);
            NotifyOfPropertyChange(() => datacontext.IsCountHI);


        }
    }
}
