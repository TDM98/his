using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel;
using System.Threading;
using System.Windows.Controls;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using Castle.Windsor;
using aEMR.Common.BaseModel;
using eHCMSLanguage;
using Castle.Core.Logging;

namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IInPatientReturnDrug)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InPatientReturnDrugViewModel : ViewModelBase, IInPatientReturnDrug
    {
        private readonly ILogger _logger;

        [ImportingConstructor]
        public InPatientReturnDrugViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            MedProductListingContent = Globals.GetViewModel<IInPatientUsedMedProductListing>();
            _logger = container.Resolve<ILogger>();
        }

        private IInPatientUsedMedProductListing _medProductListingContent;
        public IInPatientUsedMedProductListing MedProductListingContent
        {
            get { return _medProductListingContent; }
            set
            {
                _medProductListingContent = value;
                NotifyOfPropertyChange(() => MedProductListingContent);
            }
        }

        private AllLookupValues.MedProductType _medProductType;
        public AllLookupValues.MedProductType MedProductType
        {
            get { return _medProductType; }
            set { _medProductType = value; }
        }

        private PatientRegistration _registration;
        public PatientRegistration Registration
        {
            get { return _registration; }
            set { _registration = value; }
        }

        private long? _DeptID = null;
        public void InitData(long? DeptID)
        {
            MedProductListingContent.Registration = Registration;
            MedProductListingContent.MedProductType = this.MedProductType;

            MedProductListingContent.LoadData(DeptID);
            _DeptID = DeptID;
        }

        public void ReturnDrugCmd()
        {
            ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults = null;
            if (!MedProductListingContent.ValidateInfo(out validationResults))
            {
                Action<IValidationError> onInitDlg = delegate (IValidationError errorVm)
                {
                    errorVm.SetErrors(validationResults);
                };
                GlobalsNAV.ShowDialog<IValidationError>(onInitDlg);
                return;
            }
            List<RefGenMedProductSummaryInfo> returnItems = MedProductListingContent.GetReturnItems();
            if (returnItems != null && returnItems.Count > 0)
            {
                ReturnDrug(returnItems,_DeptID);
            }
        }

        private void ReturnDrug(List<RefGenMedProductSummaryInfo> returnedItems,long? DeptID )
        {
            this.DlgShowBusyIndicator();
            var t = new Thread(() =>
            {
                //IsLoading = true;
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginReturnInPatientDrug(Registration.PtRegistrationID, returnedItems,DeptID, Globals.LoggedUserAccount.StaffID,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    bool bOK = contract.EndReturnInPatientDrug(asyncResult);
                                    if(bOK)
                                    {
                                        MedProductListingContent.LoadData(DeptID);
                                        TryClose();
                                        Globals.EventAggregator.Publish(new InPatientReturnMedProduct());
                                    }
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    error = new AxErrorEventArgs(fault);
                                    _logger.Info(error.ToString());
                                }
                                catch (Exception ex)
                                {
                                    error = new AxErrorEventArgs(ex);
                                    _logger.Info(error.ToString());
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
                    error = new AxErrorEventArgs(ex);
                    _logger.Info(error.ToString());
                    this.DlgHideBusyIndicator();
                }
                finally
                {
                    //IsLoading = false;
                }
                if (error != null)
                {
                    //Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
                    Globals.ShowMessage(error.ToString(), eHCMSResources.T0432_G1_Error);
                }
            });
            t.Start();
        }        
    }
}
