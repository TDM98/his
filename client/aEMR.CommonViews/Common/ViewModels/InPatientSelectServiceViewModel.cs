using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Threading;
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
using System.Linq;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IInPatientSelectService)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InPatientSelectServiceViewModel : Conductor<object>, IInPatientSelectService
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public InPatientSelectServiceViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

        }

        private RefMedicalServiceType _medServiceType;
        public RefMedicalServiceType MedServiceType
        {
            get { return _medServiceType; }
            set
            {
                _medServiceType = value;
                NotifyOfPropertyChange(()=>MedServiceType);
                
                MedServiceItem=new RefMedicalServiceItem();
                MedicalServiceItems = null;
                ResetServiceToDefaultValue();
                GetAllMedicalServiceItemsByType(_medServiceType);
            }
        }

        private ObservableCollection<RefMedicalServiceType> _serviceTypes;
        public ObservableCollection<RefMedicalServiceType> ServiceTypes
        {
            get { return _serviceTypes; }
            set
            {
                _serviceTypes = value;
                NotifyOfPropertyChange(() => ServiceTypes);
            }
        }

        private RefMedicalServiceItem _medServiceItem;
        public RefMedicalServiceItem MedServiceItem
        {
            get { return _medServiceItem; }
            set
            {
                _medServiceItem = value;
                NotifyOfPropertyChange(() => MedServiceItem);

                Globals.EventAggregator.Publish(new ItemSelected<RefMedicalServiceItem>() {Item = _medServiceItem});
            }
        }

        private ObservableCollection<RefMedicalServiceItem> _medicalServiceItems;
        public ObservableCollection<RefMedicalServiceItem> MedicalServiceItems
        {
            get { return _medicalServiceItems; }
            set
            {
                _medicalServiceItems = value;
                NotifyOfPropertyChange(() => MedicalServiceItems);
            }
        }

        public long? DeptID { get; set; }
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
                if (_CurrentInPatientAdmDisDetail != null && MedServiceType != null && ServiceTypes != null)
                {
                    if (value == null || value.MedServiceItemPriceListID != _CurrentInPatientAdmDisDetail.MedServiceItemPriceListID)
                    {
                        MedServiceType = ServiceTypes.FirstOrDefault();
                    }
                }
                _CurrentInPatientAdmDisDetail = value;
            }
        }
        public void GetServiceTypes()
        {
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0621_G1_DangLayDSLoaiDV)
                });
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        List<long> outTypes = new List<long>();
                        outTypes.Add((long)AllLookupValues.V_RefMedicalServiceInOutOthers.NOITRU);
                        outTypes.Add((long)AllLookupValues.V_RefMedicalServiceInOutOthers.NOITRU_NGOAITRU);
                        contract.BeginGetMedicalServiceTypesByInOutType(outTypes,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var allItems = contract.EndGetMedicalServiceTypesByInOutType(asyncResult);
                                    if (_serviceTypes == null)
                                    {
                                        _serviceTypes = new ObservableCollection<RefMedicalServiceType>();
                                    }
                                    else
                                    {
                                        _serviceTypes.Clear();
                                    }
                                    _serviceTypes.Add(new RefMedicalServiceType() { MedicalServiceTypeID = -1, MedicalServiceTypeName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K1916_G1_Chon) });
                                    foreach (RefMedicalServiceType info in allItems.Where(x=>x.V_RefMedicalServiceTypes!= (long)AllLookupValues.V_RefMedicalServiceTypes.THUTHUAT
                                                                                            && x.V_RefMedicalServiceTypes != (long)AllLookupValues.V_RefMedicalServiceTypes.MAU 
                                                                                            && x.V_RefMedicalServiceTypes != (long)AllLookupValues.V_RefMedicalServiceTypes.GIUONGBENH))
                                    {
                                        _serviceTypes.Add(info);
                                    }
                                    NotifyOfPropertyChange(() => ServiceTypes);
                                }
                                catch (Exception innerEx)
                                {
                                    error = new AxErrorEventArgs(innerEx);
                                }
                                finally
                                {
                                    Globals.IsBusy = false;
                                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                                }
                            }), null);
                    }
                }
                catch(FaultException<AxException> fault)
                {
                    error = new AxErrorEventArgs(fault);
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {
                    Globals.IsBusy = false;
                }
                if(error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                }
            });
            t.Start();
        }

        public void GetAllMedicalServiceItemsByType(RefMedicalServiceType serviceType)
        {
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0992_G1_DangLayDSCacDV)
                });
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        long? serviceTypeID = null;
                        if (serviceType != null)
                        {
                            serviceTypeID = serviceType.MedicalServiceTypeID;
                        }
                        contract.BeginGetAllMedicalServiceItemsByType(serviceTypeID, CurrentInPatientAdmDisDetail == null ? null : CurrentInPatientAdmDisDetail.MedServiceItemPriceListID, 
                            0,Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<RefMedicalServiceItem> allItem = new ObservableCollection<RefMedicalServiceItem>();
                                try
                                {
                                    allItem = contract.EndGetAllMedicalServiceItemsByType(asyncResult);
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                }
                                if (allItem == null)
                                {
                                    MedicalServiceItems = null;
                                }
                                else
                                {
                                    var sType = (RefMedicalServiceType)asyncResult.AsyncState;
                                    var col = new ObservableCollection<RefMedicalServiceItem>();
                                    foreach (var item in allItem)
                                    {
                                        item.RefMedicalServiceType = sType;
                                        if (item.HITTypeID == 7 && item.allDeptLocation != null)
                                        {
                                            foreach (var DeptLoc in item.allDeptLocation)
                                            {
                                                if (DeptLoc.DeptID == DeptID)
                                                {
                                                    col.Add(item);
                                                }
                                            }
                                        }                                        
                                        else {
                                            col.Add(item);
                                        }
                                    }
                                    MedicalServiceItems = col;
                                }
                                ResetServiceToDefaultValue();
                            }), serviceType);
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
                    Globals.IsBusy = false;
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                }
            });
            t.Start();
        }
        public void ResetServiceToDefaultValue()
        {
            if (MedicalServiceItems != null && MedicalServiceItems.Count > 0)
            {
                MedServiceItem = MedicalServiceItems[0];
            }
            else
            {
                MedServiceItem = null;
            }
        }
    }
}
