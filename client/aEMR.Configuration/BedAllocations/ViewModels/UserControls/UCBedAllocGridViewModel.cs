using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;

/*
 * 20180413 #001 TBLD: Add new method LoadViewData().
 * 20181114 #002 TTM: BM 0005259: Chuyển đổi CellEditEnded => CellEditEnding.
*/
namespace aEMR.Configuration.BedAllocations.ViewModels
{
    [Export(typeof(IUCBedAllocGrid)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class UCBedAllocGridViewModel : Conductor<object>, IUCBedAllocGrid
        , IHandle<DeptLocBedSelectedEvent>
        , IHandle<SetNewBedForUCBedAllocGridViewModel>
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

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public UCBedAllocGridViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            authorization();
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            //lstBedNumberEX=new List<string>();
            Globals.EventAggregator.Subscribe(this);
        }
        private BedAllocation _selectedNewBedAllocation ;
        public BedAllocation selectedNewBedAllocation
        {
            get
            {
                return _selectedNewBedAllocation;
            }
            set
            {
                if (_selectedNewBedAllocation == value)
                    return;
                _selectedNewBedAllocation = value;
                NotifyOfPropertyChange(() => selectedNewBedAllocation);
            }
        }
        
        private ObservableCollection<BedAllocation> _allBedAllocation;
        public ObservableCollection<BedAllocation> allBedAllocation
        {
            get
            {
                return _allBedAllocation;
            }
            set
            {
                if (_allBedAllocation == value)
                    return;
                _allBedAllocation = value;
                NotifyOfPropertyChange(() => allBedAllocation);
                //NotifyOfPropertyChange(() => CanbutSave());
            }
        }

        private ObservableCollection<RoomPrices> _allRoomPrices;
        public ObservableCollection<RoomPrices> allRoomPrices
        {
            get
            {
                return _allRoomPrices;
            }
            set
            {
                if (_allRoomPrices == value)
                    return;
                _allRoomPrices = value;
                NotifyOfPropertyChange(() => allRoomPrices);
            }
        }

        private RefDepartmentsTree _SeletedRefDepartmentsTree;
        public RefDepartmentsTree SeletedRefDepartmentsTree
        {
            get
            {
                return _SeletedRefDepartmentsTree;
            }
            set
            {
                if (_SeletedRefDepartmentsTree == value)
                    return;
                _SeletedRefDepartmentsTree = value;
                NotifyOfPropertyChange(() => SeletedRefDepartmentsTree);
            }
        }

        //private List<String> _lstBedNumberEX;
        //public List<String> lstBedNumberEX
        //{
        //    get
        //    {
        //        return _lstBedNumberEX;
        //    }
        //    set
        //    {
        //        if (_lstBedNumberEX == value)
        //            return;
        //        _lstBedNumberEX = value;
        //        NotifyOfPropertyChange(() => lstBedNumberEX);
        //    }
        //}

        private DataGrid _grdBedAllocations;
        public DataGrid grdBedAllocations
        {
            get
            {
                return _grdBedAllocations;
            }
            set
            {
                if (_grdBedAllocations == value)
                    return;
                _grdBedAllocations = value;
                NotifyOfPropertyChange(() => grdBedAllocations);
            }
        }

        private int _Total;
        public int Total
        {
            get
            {
                return _Total;
            }
            set
            {
                if (_Total == value)
                    return;
                _Total = value;
                NotifyOfPropertyChange(() => Total);
            }
        }
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            blnkDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mQuanLyGiuongBenh,
                                               (int)oConfigurationEx.mDatGiuongChoPhong, (int)ePermission.mDelete);
            bbutSave= Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mQuanLyGiuongBenh,
                                               (int)oConfigurationEx.mDatGiuongChoPhong, (int)ePermission.mAdd);
            bbutAddBed = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                           , (int)eConfiguration_Management.mQuanLyGiuongBenh,
                                           (int)oConfigurationEx.mDatGiuongChoPhong, (int)ePermission.mAdd);
            bbutViewBed = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                           , (int)eConfiguration_Management.mQuanLyGiuongBenh,
                                           (int)oConfigurationEx.mDatGiuongChoPhong, (int)ePermission.mView);
        }
        #region checking account

        private bool _blnkDelete = true;
        private bool _bbutSave = true;
        private bool _bbutAddBed = true;
        private bool _bbutViewBed = true;
        public bool blnkDelete
        {
            get
            {
                return _blnkDelete;
            }
            set
            {
                if (_blnkDelete == value)
                    return;
                _blnkDelete = value;
            }
        }
        public bool bbutSave
        {
            get
            {
                return _bbutSave;
            }
            set
            {
                if (_bbutSave == value)
                    return;
                _bbutSave = value;
            }
        }
        public bool bbutAddBed
        {
            get
            {
                return _bbutAddBed;
            }
            set
            {
                if (_bbutAddBed == value)
                    return;
                _bbutAddBed = value;
            }
        }
        public bool bbutViewBed
        {
            get
            {
                return _bbutViewBed;
            }
            set
            {
                if (_bbutViewBed == value)
                    return;
                _bbutViewBed = value;
            }
        }
        #endregion
        #region binding visibilty

        public Button lnkDelete { get; set; }

        public void lnkDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;
            lnkDelete.Visibility = Globals.convertVisibility(blnkDelete);
        }


        #endregion
        public void lnkDeleteClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.A0161_G1_Msg_ConfXoaGiuong, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                return;
                
            if (selectedNewBedAllocation.Status == 1)
            {
                allBedAllocation.Remove(selectedNewBedAllocation);
            }
            else
            {
                if (selectedNewBedAllocation != null)
                {
                    DeleteBedAllocation(selectedNewBedAllocation.BedAllocationID);
                }
            }
        }

        public void butOrder()
        {
            //var cwdBedAllocVM = Globals.GetViewModel<IcwdBedAlloc>();
            //this.ActivateItem(cwdBedAllocVM);
            //cwdBedAllocVM.deptID = SeletedRefDepartmentsTree.NodeID;
            
            //Globals.ShowDialog(cwdBedAllocVM as Conductor<object>);

            Action<IcwdBedAlloc> onInitDlg = (cwdBedAllocVM) =>
            {
                this.ActivateItem(cwdBedAllocVM);
                cwdBedAllocVM.deptID = SeletedRefDepartmentsTree.NodeID;
            };
            
            GlobalsNAV.ShowDialog<IcwdBedAlloc>(onInitDlg);

        }
        public void butAddBed()
        {
            if (SeletedRefDepartmentsTree!=null
               && SeletedRefDepartmentsTree.IsDeptLocation==true)
            {
                //var cwdBedAllocationVM = Globals.GetViewModel<IcwdBedAllocation>();
                //this.ActivateItem(cwdBedAllocationVM);
                //cwdBedAllocationVM.allBedAllocation = allBedAllocation;
                //cwdBedAllocationVM.SeletedRefDepartmentsTree = SeletedRefDepartmentsTree;
                //Globals.ShowDialog(cwdBedAllocationVM as Conductor<object>);


                Action<IcwdBedAllocation> onInitDlg = (cwdBedAllocationVM) =>
                {
                    this.ActivateItem(cwdBedAllocationVM);
                    cwdBedAllocationVM.allBedAllocation = allBedAllocation;
                    cwdBedAllocationVM.SeletedRefDepartmentsTree = SeletedRefDepartmentsTree;
                };

                GlobalsNAV.ShowDialog<IcwdBedAllocation>(onInitDlg);
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0101_G1_Msg_InfoChuaChonPg);
            }
            
        }
        public void butHistory()
        {
            //var BedPatientAllocVM = Globals.GetViewModel<IBedPatientAlloc>();
            //this.ActivateItem(BedPatientAllocVM);

            //Globals.ShowDialog(BedPatientAllocVM as Conductor<object>);

            Action<IBedPatientAlloc> onInitDlg = (BedPatientAllocVM) =>
            {
                this.ActivateItem(BedPatientAllocVM);
            };

            GlobalsNAV.ShowDialog<IBedPatientAlloc>(onInitDlg);
        }

        public void Handle(DeptLocBedSelectedEvent obj)
        {
            if(obj!=null)
            {
                SeletedRefDepartmentsTree = (RefDepartmentsTree) obj.curDeptLoc;
                /*▼====: #001*/
                //GetAllRoomPricesByDeptID(SeletedRefDepartmentsTree.NodeID);
                //GetAllBedAllocByDeptID(SeletedRefDepartmentsTree.NodeID, 1);
                //lstBedNumberEX.Clear();
                LoadViewData();
                /*▲====: #001*/
            }
        }
        private void GetAllRoomPricesByDeptID(long DeptLocationID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new BedAllocationsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllRoomPricesByDeptID(DeptLocationID,Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetAllRoomPricesByDeptID(asyncResult);
                            if (results != null)
                            {
                                if (allRoomPrices == null)
                                {
                                    allRoomPrices = new ObservableCollection<RoomPrices>(); 
                                }
                                else
                                {
                                    allRoomPrices.Clear();
                                }
                                foreach (var p in results)
                                {
                                    allRoomPrices.Add(p);
                                }
                                NotifyOfPropertyChange(() => allRoomPrices);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }
        private void GetAllBedAllocByDeptID(long DeptLocationID, int IsActive)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new BedAllocationsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllBedAllocByDeptID(DeptLocationID, IsActive, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int tt = 0;
                            var results = contract.EndGetAllBedAllocByDeptID(out tt,asyncResult);
                            if (results != null)
                            {
                                if (allBedAllocation == null)
                                {
                                    allBedAllocation = new ObservableCollection<BedAllocation>();
                                }
                                else
                                {
                                    allBedAllocation.Clear();
                                }
                                foreach (var p in results)
                                {
                                    allBedAllocation.Add(p);
                                }
                                Total = tt;
                                NotifyOfPropertyChange(() => allBedAllocation);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void DeleteBedAllocation(long BedAllocationID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new BedAllocationsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDeleteBedAllocation(BedAllocationID,  Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndDeleteBedAllocation(asyncResult);
                            if (results==true)
                            {
                                GetAllBedAllocByDeptID(SeletedRefDepartmentsTree.NodeID, 1);
                                MessageBox.Show(eHCMSResources.K0537_G1_XoaOk);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }
        public bool CheckDuplicate(string stCheck)
        {
            int count = 0;
            foreach (var ba in allBedAllocation)
            {
                if(ba.BedNumber.ToUpper().Equals(stCheck.ToUpper()))
                {
                    count++;
                    if (count > 1) return false;
                }
            }
            return true;
        }
        //▼====== #002
        //public void grdBedAllocations_CellEditEnded(object sender, DataGridCellEditEndingEventArgs e)
        //{
        //    //kiem tra co trung khong
        //    if (selectedNewBedAllocation.BedNumber=="")
        //    {
        //        return;
        //    }
        //    if (!CheckDuplicate(selectedNewBedAllocation.BedNumber))
        //    {
        //        grdBedAllocations = sender as DataGrid;
        //        MessageBox.Show(eHCMSResources.A0815_G1_Msg_InfoMaGiuongDaTonTai);
        //        grdBedAllocations.BeginEdit();
        //    }
        //}
        public void grdBedAllocations_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (selectedNewBedAllocation.BedNumber == "")
            {
                return;
            }
            if (!CheckDuplicate(selectedNewBedAllocation.BedNumber))
            {
                grdBedAllocations = sender as DataGrid;
                MessageBox.Show(eHCMSResources.A0815_G1_Msg_InfoMaGiuongDaTonTai);
                grdBedAllocations.BeginEdit();
            }
        }
        //▲====== #002
        public bool CanbutSave()
        {
            return true;
        }
        public void butSave()
        {
            ObservableCollection<BedAllocation> tempBedLocationUpdate = new ObservableCollection<BedAllocation>();
            ObservableCollection<BedAllocation> tempBedLocationAddNew = new ObservableCollection<BedAllocation>();
            foreach (BedAllocation bal in allBedAllocation)
            {
                if (bal.BedNumber == "")
                {
                    MessageBox.Show(eHCMSResources.A0418_G1_Msg_InfoChuaNhapDuMaGiuong);
                    return;
                }
                if (bal.Status == 0)
                {
                    tempBedLocationUpdate.Add(bal);
                }
                else
                    if (bal.Status == 1)
                    {
                        tempBedLocationAddNew.Add(bal);
                    }
            }
            if (tempBedLocationUpdate.Count > 0)
            {
                UpdateBedAllocationList(tempBedLocationUpdate);
            }
            if (tempBedLocationAddNew.Count > 0)
            {
                lock (lockObj)
                {
                    AddNewBedAllocationList(tempBedLocationAddNew);
                }
            }

        }
        static object lockObj=new object();

        private void AddNewBedAllocationList(ObservableCollection<BedAllocation> LstBedAllocation)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.ShowBusyIndicator(eHCMSResources.A0501_G1_Msg_InfoDangTHien);
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new BedAllocationsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginAddNewBedAllocationList(LstBedAllocation, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndAddNewBedAllocationList(asyncResult);
                            foreach (BedAllocation bal in allBedAllocation)
                            {
                                bal.Status = 0;
                            }
                            Total = allBedAllocation.Count;                    
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            this.HideBusyIndicator();
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading = false;
                            /*▼====: #001*/
                            LoadViewData();
                            /*▲====: #001*/
                            this.HideBusyIndicator();
                        }
                        
                    }), null);

                }

            });

            t.Start();
        }
        private void UpdateBedAllocationList(ObservableCollection<BedAllocation> LstBedAllocation)
        {
            this.ShowBusyIndicator(eHCMSResources.A0501_G1_Msg_InfoDangTHien);
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new BedAllocationsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateBedAllocationList(LstBedAllocation, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndUpdateBedAllocationList(asyncResult);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            this.HideBusyIndicator();
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading = false;
                            /*▼====: #001*/
                            LoadViewData();
                            /*▲====: #001*/
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }
        /*▼====: #001*/
        public void LoadViewData()
        {
            GetAllRoomPricesByDeptID(SeletedRefDepartmentsTree.NodeID);
            GetAllBedAllocByDeptID(SeletedRefDepartmentsTree.NodeID, 1);
        }
        /*▲====: #001*/

        public void Handle(SetNewBedForUCBedAllocGridViewModel sources)
        {
            if (sources != null)
            {
                foreach (BedAllocation bedAllocation in sources.BedAllocationsList)
                {
                    allBedAllocation.Add(bedAllocation);
                }
            }
        }
    }
}
