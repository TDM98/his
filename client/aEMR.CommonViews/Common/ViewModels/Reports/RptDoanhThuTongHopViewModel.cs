using eHCMSLanguage;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using Caliburn.Micro;
using DataEntities;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using eHCMS.CommonUserControls.CommonTasks;
using System.Linq;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using aEMR.CommonTasks;
using aEMR.Common.Collections;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(ICommonRptDoanhThuTongHop)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RptDoanhThuTongHopViewModel : Conductor<object>, ICommonRptDoanhThuTongHop
    {
        [ImportingConstructor]
        public RptDoanhThuTongHopViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            // TxD 02/08/2014 User Globals Server Date instead
            //GetCurrentDate();
            DateTime todayDate = Globals.GetCurServerDateTime();
            FromDate = todayDate;
            ToDate = todayDate;

            //LoadAllDepartment();
            LoadRefDept(V_DeptTypeOperation.KhoaNgoaiTru);
        }

        public void LoadRefDept(V_DeptTypeOperation VDeptTypeOperation)
        {
            Coroutine.BeginExecute(NewLoadDepartments(VDeptTypeOperation));
        }

        private IEnumerator<IResult> NewLoadDepartments(V_DeptTypeOperation VDeptTypeOperation)
        {
            ObservableCollection<RefDepartment> tempDepartments = new ObservableCollection<RefDepartment>();
            var departmentTask = new LoadDepartmentsByV_DeptTypeOperationTask((long)VDeptTypeOperation);
            yield return departmentTask;

            //NewDepartments = departmentTask.Departments.Where(item => item.DeptID == Convert.ToInt32(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.KhoaPhongKham])).ToObservableCollection();
            // Txd 25/05/2014 Replaced ConfigList
            NewDepartments = departmentTask.Departments.Where(item => item.DeptID == Globals.ServerConfigSection.Hospitals.KhoaPhongKham).ToObservableCollection();

            curNewDepartments = NewDepartments.FirstOrDefault();
        }

        // TxD 02/08/2014 the following method is nolonger required
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
        //                        ClientLoggerHelper.LogInfo(ex.ToString());
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

        private DateTime? _FromDate;
        public DateTime? FromDate
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

        private DateTime? _ToDate;
        public DateTime? ToDate
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
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                {
                    proAlloc.TieuDeRpt = string.Format("{0} ", eHCMSResources.Z1148_G1_BCTgHopDThuPK) + string.Format("{0} ", eHCMSResources.G1933_G1_TuNg) + FromDate.Value.ToString("dd/MM/yyyy") + string.Format(" {0}", eHCMSResources.K3192_G1_DenNg) + ToDate.Value.ToString("dd/MM/yyyy");
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
                DateTime F = Globals.ServerDate.Value;
                DateTime.TryParse(FromDate.ToString(), out F);

                DateTime T = Globals.ServerDate.Value;
                DateTime.TryParse(ToDate.ToString(), out T);

                if (F > T)
                {
                    MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0467_G1_TuNgNhoHonBangDenNg), eHCMSResources.G0981_G1_Tim, MessageBoxButton.OK);
                    return false;
                }
                return true;
            }
            else
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.A0884_G1_Msg_InfoNhapTuNgDenNg));
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
                    //LoadLocations(curNewDepartments.DeptID);
                    Coroutine.BeginExecute(DoLoadLocations(curNewDepartments.DeptID));
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
            itemDefault.Location.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0116_G1_HayChonPg);
            Locations.Insert(0, itemDefault);
            SelectedLocation = itemDefault;
            yield break;
        }

        public void LoadLocations(long? deptId)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0115_G1_LayDSPgBan);
            var list = new List<refModule>();
            var t = new Thread(() =>
            {
                try
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
                                itemDefault.Location.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z1098_G1_TatCaPhg);
                                Locations.Insert(0, itemDefault);

                                SelectedLocation = itemDefault;
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });
            t.Start();
        }

        public void LoadAllDepartment()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0115_G1_LayDSPgBan);
            var list = new List<refModule>();
            var t = new Thread(() =>
            {
                try
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
                                itemDefault.DeptName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0044_G1_TatCaKhoa);
                                NewDepartments.Insert(0, itemDefault);

                                curNewDepartments = itemDefault;
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });
            t.Start();
        }
    }
}
