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
using Caliburn.Micro;
using aEMR.ViewContracts;
using System.ComponentModel.Composition;
using DataEntities;
using aEMR.Common;
using Castle.Windsor;

namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IInPatientSelectService)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InPatientSelectServiceViewModel : Conductor<object>, IInPatientSelectService
    {
        [ImportingConstructor]
        public InPatientSelectServiceViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
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
        public InPatientAdmDisDetails CurrentInPatientAdmDisDetail { get; set; }
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
                                    foreach (RefMedicalServiceType info in allItems)
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

                        contract.BeginGetAllMedicalServiceItemsByType(serviceTypeID, null,null,
                            Globals.DispatchCallback((asyncResult) =>
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
                                        col.Add(item);
                                    }
                                    MedicalServiceItems = col;
                                }
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
    }
}
