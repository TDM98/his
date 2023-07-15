using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using aEMR.Common;
using eHCMSLanguage;
using aEMR.DataContracts;
using System.ServiceModel;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;

namespace aEMR.StoreDept.ViewModels
{
    [Export(typeof(IPatientManagement)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientManagementViewModel : Conductor<object>, IPatientManagement
        , IHandle<LoadPatientListBySeletedDeptID>

    {
        private object _ContentRefDepartments;
        public object ContentRefDepartments
        {
            get
            {
                return _ContentRefDepartments;
            }
            set
            {
                if (_ContentRefDepartments != value)
                {
                    _ContentRefDepartments = value;
                    NotifyOfPropertyChange(() => ContentRefDepartments);
                }
            }
        }
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PatientManagementViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            //Globals.EventAggregator.Subscribe(this);

            var UCRefDepartments_BystrV_DeptTypeViewModel = Globals.GetViewModel<IRefDepartments_BystrV_DeptType>();
            UCRefDepartments_BystrV_DeptTypeViewModel.strV_DeptType = "7000";
            UCRefDepartments_BystrV_DeptTypeViewModel.ShowDeptLocation = false;
            UCRefDepartments_BystrV_DeptTypeViewModel.Parent = this;
            UCRefDepartments_BystrV_DeptTypeViewModel.RefDepartments_Tree();
            ContentRefDepartments = UCRefDepartments_BystrV_DeptTypeViewModel;

            (this as Conductor<object>).ActivateItem(ContentRefDepartments);

            ObjDeptLocation_ByDeptID = new ObservableCollection<Location>();

            ObjPCLExamTypeLocations_ByDeptLocationID = new ObservableCollection<PCLExamType>();

            Registrations = new PagedSortableCollectionView<PatientRegistration>();

        }

        private DataEntities.RefDepartmentsTree _ObjKhoa_Current;
        public DataEntities.RefDepartmentsTree ObjKhoa_Current
        {
            get { return _ObjKhoa_Current; }
            set
            {
                _ObjKhoa_Current = value;
                NotifyOfPropertyChange(() => ObjKhoa_Current);
            }
        }

        private ObservableCollection<Location> _ObjDeptLocation_ByDeptID;
        public ObservableCollection<Location> ObjDeptLocation_ByDeptID
        {
            get { return _ObjDeptLocation_ByDeptID; }
            set
            {
                _ObjDeptLocation_ByDeptID = value;
                NotifyOfPropertyChange(() => ObjDeptLocation_ByDeptID);

            }
        }
        public void DeptLocation_ByDeptID(Int64 DeptID, Int64 RmTypeID, string LocationName)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginDeptLocation_ByDeptID(DeptID, RmTypeID, LocationName, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndDeptLocation_ByDeptID(asyncResult);

                            if (items != null)
                            {
                                ObjDeptLocation_ByDeptID = new ObservableCollection<DataEntities.Location>(items);
                                
                                //ItemDefault
                                DataEntities.Location ItemDefault = new DataEntities.Location();
                                ItemDefault.LID = -1;
                                ItemDefault.DeptLocationID=-1;
                                ItemDefault.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2094_G1_ChonPg);
                                ObjDeptLocation_ByDeptID.Insert(0, ItemDefault);
                                //ItemDefault
                            }
                            else
                            {
                                ObjDeptLocation_ByDeptID = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                        }
                    }), null);
                }


            });
            t.Start();
        }
        
        //Loại Phòng Change
        public void cboDeptLocationID_SelectedItemChanged(object selectedItem)
        {
            DataEntities.DeptLocation Item = (selectedItem as DataEntities.DeptLocation);
            if (Item != null)
            {
                if (Item.DeptLocationID >= 0)
                {
                    DeptLocation_ByDeptID(ObjKhoa_Current.NodeID, Item.Location.RmTypeID.Value,"");
                }
            }
        }


        private PCLExamType _ObjPCLExamType_SelectForAdd;
        public PCLExamType ObjPCLExamType_SelectForAdd
        {
            get { return _ObjPCLExamType_SelectForAdd; }
            set
            {
                _ObjPCLExamType_SelectForAdd = value;
                NotifyOfPropertyChange(() => ObjPCLExamType_SelectForAdd);
            }
        }



        private ObservableCollection<PCLExamType> _ObjPCLExamTypeLocations_ByDeptLocationID;
        public ObservableCollection<PCLExamType> ObjPCLExamTypeLocations_ByDeptLocationID
        {
            get { return _ObjPCLExamTypeLocations_ByDeptLocationID; }
            set
            {
                _ObjPCLExamTypeLocations_ByDeptLocationID = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypeLocations_ByDeptLocationID);

            }
        }

        private PagedSortableCollectionView<PatientRegistration> _registrations;
        public PagedSortableCollectionView<PatientRegistration> Registrations
        {
            get { return _registrations; }
            private set
            {
                _registrations = value;
                NotifyOfPropertyChange(() => Registrations);
            }
        }
        private SeachPtRegistrationCriteria _criteria;
        public void Handle(LoadPatientListBySeletedDeptID message)
        {
            if (message == null || message.DepartmentTree != null)
            {
                DataEntities.RefDepartmentsTree NodeTree = message.DepartmentTree as DataEntities.RefDepartmentsTree;
                if (NodeTree.ParentID == null)
                {
                    return;
                }
                _criteria = new SeachPtRegistrationCriteria();
                _criteria.PatientFindBy = Globals.PatientFindBy_ForConsultation != null ? Globals.PatientFindBy_ForConsultation.Value : AllLookupValues.PatientFindBy.NOITRU;
                _criteria.DeptID = NodeTree.NodeID;
                _criteria.IsAdmission = true;
                _criteria.IsDischarge = false;
                SearchRegistrationsInPt(0, Registrations.PageSize, true);
            }
        }

        private void SearchRegistrationsInPt(int pageIndex, int pageSize, bool bCountTotal)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginSearchRegistrationsInPt(_criteria, pageIndex, pageSize, bCountTotal, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            int totalCount = 0;
                            IList<PatientRegistration> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndSearchRegistrationsInPt(out totalCount, asyncResult);
                                bOK = true;
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
                                this.HideBusyIndicator();
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
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.HideBusyIndicator();
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        public void Preview02FormCmd()
        {
            MessageBox.Show(eHCMSResources.A0073_G1_CNangDangHThien);
            return;
        }
    }
}
