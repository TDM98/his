using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.ViewContracts.ResourcesManage.Maintenance;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using eHCMSLanguage;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;

namespace aEMR.ResourceMaintenance.ViewModels
{
    [Export(typeof(IResourceInfoTrans))]
    public class InfoTransViewModel : Conductor<object>, IResourceInfoTrans, IHandle<ResourcePropLocationsEvent>, IHandle<DeptLocSelectedEvent>
    {
        private bool _IsChildWindowForChonDiBaoTri=false;
        public bool IsChildWindowForChonDiBaoTri
        {
            get { return _IsChildWindowForChonDiBaoTri; }
            set
            {
                if (_IsChildWindowForChonDiBaoTri != value)
                {
                    _IsChildWindowForChonDiBaoTri = value;
                    NotifyOfPropertyChange(() => IsChildWindowForChonDiBaoTri);
                }
            }
        }
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public InfoTransViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            //Globals.EventAggregator.Subscribe(this);
            authorization();
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            authorization();
            CurrentDeptLoc = null;
            CurrentResource = null;
        }
        private object _ActiveContent;
        public object ActiveContent
        {
            get
            {
                return _ActiveContent;
            }
            set
            {
                _ActiveContent = value;
                NotifyOfPropertyChange(()=>ActiveContent);
            }
        }

        private Resources _CurrentResource;
        public Resources CurrentResource
        {
            get
            {
                return _CurrentResource;
            }
            set
            {
                if (_CurrentResource == value)
                    return;
                _CurrentResource = value;
                NotifyOfPropertyChange(() => CurrentResource);
            }
        }

        private RefDepartmentsTree _CurrentDeptLoc;
        public RefDepartmentsTree CurrentDeptLoc
        {
            get
            {
                return _CurrentDeptLoc;
            }
            set
            {
                if (_CurrentDeptLoc == value)
                    return;
                _CurrentDeptLoc = value;
                NotifyOfPropertyChange(() => CurrentDeptLoc);
            }
        }

        private ResourcePropLocations _selectedResourcePropLocations;
        public ResourcePropLocations selectedResourcePropLocations
        {
            get
            {
                return _selectedResourcePropLocations;
            }
            set
            {
                if (_selectedResourcePropLocations == value)
                    return;
                _selectedResourcePropLocations = value;
                NotifyOfPropertyChange(() => selectedResourcePropLocations);
            }
        }

#region overide event in Interface
        public void Handle(ResourcePropLocationsEvent Obj)
        {
            if (Obj != null)
            {
                selectedResourcePropLocations = (ResourcePropLocations)Obj.selResourcePropLocations;
                CurrentResource = selectedResourcePropLocations.VRscrProperty.VResources;
            }
            
        }
        public void Handle(DeptLocSelectedEvent Obj)
        {
            if (Obj != null)
            {
                CurrentDeptLoc = (RefDepartmentsTree)Obj.curDeptLoc;
                CurrentResource = null;
            }
        }
        
        public void ResourceTranf()
        {
            if (CurrentDeptLoc != null && CurrentResource != null)
            {
                //var resTran = Globals.GetViewModel<IPropTranfer>();
                //resTran.selectedResourcePropLocations= selectedResourcePropLocations;
                
                //var instance = resTran as Conductor<object>;
                //this.ActivateItem(resTran);
                //Globals.ShowDialog(instance, (o) => { });

                //Globals.EventAggregator.Publish(new PropDeptEvent(CurrentResource, CurrentDeptLoc));

                Action<IPropTranfer> onInitDlg = (resTran) =>
                {
                    resTran.selectedResourcePropLocations = selectedResourcePropLocations;
                };
                GlobalsNAV.ShowDialog<IPropTranfer>(onInitDlg);
            }
            else
            {
                Globals.ShowMessage(eHCMSResources.Z1744_G1_ChuaChonVTuDieuChuyen, eHCMSResources.G0442_G1_TBao, (o) => { });
            }
        }
        public void ResourceMaint()
        {
            if (CurrentDeptLoc != null && CurrentResource != null)
            {
                //Check chưa đi bảo trì
                CheckRscrDiBaoTri(selectedResourcePropLocations.RscrPropertyID);
                //Check chưa đi bảo trì
            }
            else
            {
                Globals.ShowMessage(eHCMSResources.Z1745_G1_ChuaChonVTuBaoTri, eHCMSResources.G0442_G1_TBao, (o) => { });
            }
        }
        public void ResourceMoveHis()
        {
            if (CurrentDeptLoc != null && CurrentResource != null)
            {
                //var propHisTran = Globals.GetViewModel<IStoragesHistory>();
                //propHisTran.selectedResourcePropLocations = selectedResourcePropLocations;

                //var instance = propHisTran as Conductor<object>;
                //this.ActivateItem(propHisTran);
                //Globals.ShowDialog(instance, (o) => { });

                Action<IStoragesHistory> onInitDlg = (propHisTran) =>
                {
                    propHisTran.selectedResourcePropLocations = selectedResourcePropLocations;
                };
                GlobalsNAV.ShowDialog<IStoragesHistory>(onInitDlg);

                //Globals.EventAggregator.Publish(new PropDeptEvent(CurrentResource, CurrentDeptLoc));
            }
            else
            {
                Globals.ShowMessage(eHCMSResources.Z1746_G1_ChuaChonVTuXemLSuDChuyen, eHCMSResources.G0442_G1_TBao, (o) => { });
            }
        }

        private void CheckRscrDiBaoTri(Int64 RscrPropertyID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0840_G1_DangKTra)});

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginCheckRscrDiBaoTri(RscrPropertyID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            if (contract.EndCheckRscrDiBaoTri(asyncResult))
                            {
                                
                                if(IsChildWindowForChonDiBaoTri)
                                {
                                    //var typeInfo = Globals.GetViewModel<IResourceMaintenanceLog_Add>();
                                    //typeInfo.KhoiTaoYeuCauVaGanBien(selectedResourcePropLocations);

                                    //var instance = typeInfo as Conductor<object>;

                                    //Globals.ShowDialog(instance, (o) =>
                                    //{
                                    //    //lam gi do
                                    //});

                                    Action<IResourceMaintenanceLog_Add> onInitDlg = (typeInfo) =>
                                    {
                                        typeInfo.KhoiTaoYeuCauVaGanBien(selectedResourcePropLocations);
                                    };
                                    GlobalsNAV.ShowDialog<IResourceMaintenanceLog_Add>(onInitDlg);
                                }
                                else
                                {
                                    Globals.EventAggregator.Publish(new InfoTransViewModelEvent_ChooseRscrForMaintenance() { ObjResourcePropLocations = selectedResourcePropLocations });    
                                }
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.A0132_G1_Msg_InfoTBiDangBTri, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);
                            }

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bResourceTranf = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mResources
                                                      , (int)eResources.mPtDashboardNewTranfers
                                                      , (int)oResourcesEx.mResourceNewTranfers
                                                      , (int)ePermission.mAdd);
            bResourceMaint = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mResources
                                                      , (int)eResources.mPtDashboardNewTranfers
                                                      , (int)oResourcesEx.mResourceMaint
                                                      , (int)ePermission.mAdd);
            bResourceMoveHis = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mResources
                                                      , (int)eResources.mPtDashboardNewTranfers
                                                      , (int)oResourcesEx.mResourceMoveHis
                                                      , (int)ePermission.mView);
        }

#region checking account

        private bool _bResourceTranf = true;
        private bool _bResourceMaint = true;
        private bool _bResourceMoveHis = true;
        public bool bResourceTranf
        {
            get
            {
                return _bResourceTranf;
            }
            set
            {
                if (_bResourceTranf == value)
                    return;
                _bResourceTranf = value;
            }
        }
        public bool bResourceMaint
        {
            get
            {
                return _bResourceMaint;
            }
            set
            {
                if (_bResourceMaint == value)
                    return;
                _bResourceMaint = value;
            }
        }
        public bool bResourceMoveHis
        {
            get
            {
                return _bResourceMoveHis;
            }
            set
            {
                if (_bResourceMoveHis == value)
                    return;
                _bResourceMoveHis = value;
            }
        }
#endregion

        #endregion
    }
}
