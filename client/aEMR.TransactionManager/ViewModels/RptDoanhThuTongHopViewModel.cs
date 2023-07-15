using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using aEMR.Infrastructure.Events;
using aEMR.CommonTasks;
using aEMR.Common.ExportExcel;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using eHCMS.CommonUserControls.CommonTasks;
using eHCMSLanguage;
using Microsoft.Win32;

namespace aEMR.TransactionManager.ViewModels
{
    [Export(typeof(IRptDoanhThuTongHop)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RptDoanhThuTongHopViewModel : Conductor<object>, IRptDoanhThuTongHop
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
        public RptDoanhThuTongHopViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            authorization();

            // TxD 02/08/2014: Use Globals Server Date instead
            //GetCurrentDate();
            DateTime todayDate = Globals.GetCurServerDateTime();
            FromDate = todayDate;
            ToDate = todayDate;

            LoadAllDepartment();
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            else
            {
                mExportToExcel = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                 (int)eTransaction_Management.mTempTongHopDoanhThu_PK, (int)oTransaction_ManagementEx.mExportToExcel, (int)ePermission.mView);
                mViewAndPrint = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                 (int)eTransaction_Management.mTempTongHopDoanhThu_PK, (int)oTransaction_ManagementEx.mViewAndPrint, (int)ePermission.mView);
            }
        }

        private bool _mExportToExcel = true;
        private bool _mViewAndPrint = true;

        public bool mExportToExcel
        {
            get
            {
                return _mExportToExcel;
            }
            set
            {
                if (_mExportToExcel == value)
                    return;
                _mExportToExcel = value;
                NotifyOfPropertyChange(() => mExportToExcel);
            }
        }

        public bool mViewAndPrint
        {
            get
            {
                return _mViewAndPrint;
            }
            set
            {
                if (_mViewAndPrint == value)
                    return;
                _mViewAndPrint = value;
                NotifyOfPropertyChange(() => mViewAndPrint);
            }
        }

        // TxD 02/08/2014: The following method is nolonger required
        //public void GetCurrentDate()
        //{
        //    var t = new Thread(() =>
        //    {
        //        try
        //        {
        //            IsLoading = true;

        //            using (var serviceFactory = new CommonServiceClient())
        //            {
        //                var contract = serviceFactory.ServiceInstance;

        //                contract.BeginGetDate(Globals.DispatchCallback((asyncResult) =>
        //                {
        //                    try
        //                    {
        //                        DateTime date = contract.EndGetDate(asyncResult);
        //                        FromDate = date;
        //                        ToDate = date;
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                    }
        //                    finally
        //                    {
        //                        IsLoading = false;
        //                    }

        //                }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //        }
        //    });
        //    t.Start();
        //}

        private Nullable<DateTime> _FromDate;
        public Nullable<DateTime> FromDate
        {
            get { return _FromDate; }
            set
            {
                if (_FromDate != value)
                {
                    _FromDate = value;
                    NotifyOfPropertyChange(() => FromDate);
                }
            }
        }

        private Nullable<DateTime> _ToDate;
        public Nullable<DateTime> ToDate
        {
            get { return _ToDate; }
            set
            {
                if (_ToDate != value)
                {
                    _ToDate = value;
                    NotifyOfPropertyChange(() => ToDate);
                }
            }
        }


        public void btView()
        {
            if (CheckDateValid())
            {
                //var proAlloc = Globals.GetViewModel<ICommonPreviewView>();
                //proAlloc.TieuDeRpt = string.Format("Báo Cáo Tổng Hợp Doanh Thu PK Từ Ngày {0} Đến Ngày {1}", FromDate.Value.ToString("dd/MM/yyyy"), ToDate.Value.ToString("dd/MM/yyyy"));
                //proAlloc.DeptName = curNewDepartments != null && curNewDepartments.DeptID > 0 ? curNewDepartments.DeptName : null;
                //proAlloc.DeptID = curNewDepartments != null ? curNewDepartments.DeptID : 0;
                //proAlloc.LocationName = SelectedLocation != null && SelectedLocation.Location != null && SelectedLocation.Location.LID > 0 ? SelectedLocation.Location.LocationName : null;
                //proAlloc.DeptLocID = SelectedLocation != null ? SelectedLocation.DeptLocationID : 0;
                //proAlloc.FromDate = FromDate;
                //proAlloc.ToDate = ToDate;
                //proAlloc.eItem = ReportName.RptTongHopDoanhThu;

                //var instance = proAlloc as Conductor<object>;
                //Globals.ShowDialog(instance, (o) => { });

                Action<ICommonPreviewView> onInitDlg = (proAlloc) =>
                {
                    proAlloc.TieuDeRpt = string.Format("Báo Cáo Tổng Hợp Doanh Thu PK Từ Ngày {0} Đến Ngày {1}", FromDate.Value.ToString("dd/MM/yyyy"), ToDate.Value.ToString("dd/MM/yyyy"));
                    proAlloc.DeptName = curNewDepartments != null && curNewDepartments.DeptID > 0 ? curNewDepartments.DeptName : null;
                    proAlloc.DeptID = curNewDepartments != null ? curNewDepartments.DeptID : 0;
                    proAlloc.LocationName = SelectedLocation != null && SelectedLocation.Location != null && SelectedLocation.Location.LID > 0 ? SelectedLocation.Location.LocationName : null;
                    proAlloc.DeptLocID = SelectedLocation != null ? SelectedLocation.DeptLocationID : 0;
                    proAlloc.FromDate = FromDate;
                    proAlloc.ToDate = ToDate;
                    proAlloc.eItem = ReportName.RptTongHopDoanhThu;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
            }
        }

        private bool CheckDateValid()
        {
            if (FromDate != null && ToDate != null)
            {
                DateTime F = DateTime.Now;
                DateTime.TryParse(FromDate.ToString(), out F);

                DateTime T = DateTime.Now;
                DateTime.TryParse(ToDate.ToString(), out T);

                if (F > T)
                {
                    MessageBox.Show(eHCMSResources.Z0467_G1_TuNgNhoHonBangDenNg, eHCMSResources.G0981_G1_Tim, MessageBoxButton.OK);
                    return false;
                }
                return true;
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0884_G1_Msg_InfoNhapTuNgDenNg);
                return false;
            }
        }

        private ObservableCollection<RefDepartment> _NewDepartments;
        public ObservableCollection<RefDepartment> NewDepartments
        {
            get
            {
                return _NewDepartments;
            }
            set
            {
                _NewDepartments = value;
                NotifyOfPropertyChange(() => NewDepartments);
            }
        }

        private RefDepartment _curNewDepartments;
        public RefDepartment curNewDepartments
        {
            get
            {
                return _curNewDepartments;
            }
            set
            {
                _curNewDepartments = value;
                NotifyOfPropertyChange(() => curNewDepartments);
                if (curNewDepartments == null || curNewDepartments.DeptID <= 0)
                {
                    Locations = null;
                    SelectedLocation = null;
                }
                else
                {
                    LoadLocations(curNewDepartments.DeptID);
                }
            }
        }

        private DeptLocation _selectedLocation;
        public DeptLocation SelectedLocation
        {
            get
            {
                return _selectedLocation;
            }
            set
            {
                _selectedLocation = value;
                NotifyOfPropertyChange(() => SelectedLocation);
            }
        }

        private ObservableCollection<DeptLocation> _locations;
        public ObservableCollection<DeptLocation> Locations
        {
            get
            {
                return _locations;
            }
            set
            {
                _locations = value;
                NotifyOfPropertyChange(() => Locations);
            }
        }

        private IEnumerator<IResult> DoLoadLocations(long deptId)
        {
            var deptLoc = new LoadDeptLoctionByIDTask(deptId);
            yield return deptLoc;
            if (deptLoc.DeptLocations != null)
            {
                Locations = new ObservableCollection<DeptLocation>(deptLoc.DeptLocations);
            }
            else
            {
                Locations = new ObservableCollection<DeptLocation>();
            }

            var itemDefault = new DeptLocation();
            itemDefault.Location = new Location();
            itemDefault.Location.LID = -1;
            itemDefault.Location.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2094_G1_ChonPg);
            Locations.Insert(0, itemDefault);
            SelectedLocation = itemDefault;
            yield break;
        }

        public void LoadLocations(long? deptId)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0115_G1_LayDSPgBan) });

            var list = new List<refModule>();

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetAllLocationsByDeptIDOld(deptId, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var allItems = contract.EndGetAllLocationsByDeptIDOld(asyncResult);

                            if (allItems != null)
                            {
                                Locations = new ObservableCollection<DeptLocation>(allItems);
                            }
                            else
                            {
                                Locations = new ObservableCollection<DeptLocation>();
                            }

                            var itemDefault = new DeptLocation();
                            itemDefault.DeptID = -1;
                            itemDefault.Location = new Location();
                            itemDefault.Location.LID = -1;
                            itemDefault.Location.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0116_G1_HayChonPg);
                            Locations.Insert(0, itemDefault);

                            SelectedLocation = itemDefault;
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

        public void LoadAllDepartment()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0115_G1_LayDSPgBan) });

            var list = new List<refModule>();

            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetAllDepartments(false, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var allItems = contract.EndGetAllDepartments(asyncResult);

                            if (allItems != null)
                            {
                                NewDepartments = new ObservableCollection<RefDepartment>(allItems);
                            }
                            else
                            {
                                NewDepartments = new ObservableCollection<RefDepartment>();
                            }

                            var itemDefault = new RefDepartment();
                            itemDefault.DeptID = -1;
                            itemDefault.DeptName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0493_G1_HayChonKhoa);
                            NewDepartments.Insert(0, itemDefault);

                            curNewDepartments = itemDefault;
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


        #region Export excel from database

        public void btnExportExcel()
        {
            if (CheckDateValid())
            {
                //ExportToExcellAll_DoanhThuPK();
                SaveFileDialog objSFD = new SaveFileDialog()
                {
                    DefaultExt = ".xls",
                    Filter = "Excel xls (*.xls)|*.xls",
                    FilterIndex = 1
                };
                if (objSFD.ShowDialog() != true)
                {
                    return;
                }
                IsLoading = true;
                ReportParameters RptParameters = new ReportParameters 
                { FromDate = FromDate, ToDate = ToDate, DeptLocID = SelectedLocation != null ? SelectedLocation.DeptLocationID : 0, DeptID = curNewDepartments != null ? curNewDepartments.DeptID : 0 };
                RptParameters.ReportType = ReportType.BAOCAO_TONGHOP;
                RptParameters.Show = "TongHopDoanhThu";
                RptParameters.reportName = ReportName.RptTongHopDoanhThu;

                ExportToExcelGeneric.Action(RptParameters, objSFD, this);
                //Coroutine.BeginExecute(DoSaveExcel(RptParameters, objSFD));
            }
        }

        //private IEnumerator<IResult> DoSaveExcel(ReportParameters rptParameters, SaveFileDialog objSFD) 
        //{
        //    var res = new ExportToExcellAllGenericTask(rptParameters, objSFD);
        //    yield return res;
        //    IsLoading = false;
        //    yield break;
        //}
        public void ExportToExcellAll_DoanhThuPK()
        {
            ReportParameters RptParameters = new ReportParameters { FromDate=FromDate,ToDate=ToDate, DeptLocID = SelectedLocation != null ? SelectedLocation.DeptLocationID : 0, DeptID =curNewDepartments != null ? curNewDepartments.DeptID : 0};
            //isLoadingSearch = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new TransactionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginExportToExcellAll_DoanhThuTongHop(RptParameters, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndExportToExcellAll_DoanhThuTongHop(asyncResult);
                            ExportToExcelFileAllData.Export(results, "DoanhThuTongHop");
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //isLoadingSearch = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }
        
        #endregion


    }

}
