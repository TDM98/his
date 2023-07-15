using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Common;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using eHCMSLanguage;
using aEMR.Common.Collections;

namespace aEMR.OutstandingTasks.ViewModels
{
    [Export(typeof(IClinicRoomTask))]
    public class ClinicRoomTaskViewModel : Conductor<object>, IClinicRoomTask
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        private ConsultationTimeSegments cts=new ConsultationTimeSegments();

        [ImportingConstructor]
        public ClinicRoomTaskViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            OutstandingTitle = eHCMSResources.K3901_G1_DSPK;
            //06082018 TTM: Comment 2 dòng 39, 40 để vào nút rs để tăng tốc độ load trang đăng ký dịch vụ lên, khi nào cần load dữ liêu người dùng sẽ click vào nút refresh.
            //GetAllConsultationTimeSegments();
            //GetDeptLocTreeViewFunction(((long)VRoomType.Khoa), (long)RoomFunction.KHAM_BENH);            
            lstDeptLocation = new ObservableCollection<DeptLocation>();
            //20191015 TBL: Khi vào màn hình đăng ký dịch vụ thì load lên danh sách phòng khám
            GetAllConsultationTimeSegments();
            GetDeptLocTreeViewFunction(((long)VRoomType.Khoa), (long)RoomFunction.KHAM_BENH);
        }
        protected override void OnActivate()
        {
            //06082018 TTM: khi cần xài thì bấm vào nút refresh chứ không tự load => làm chậm khi click vào đăng ký dịch vụ
            //GetAllConsultationTimeSegments();
            OutstandingTitle = eHCMSResources.K3901_G1_DSPK;
            base.OnActivate();

            //06082018 TTM: khi cần xài thì bấm vào nút refresh chứ không tự load => làm chậm khi click vào đăng ký dịch vụ
            //if (lstDeptLocation.Count > 0)
            //{
            //    GetAllRegisDeptLoc(lstDeptLocation, 0);
            //}
        }
        public void RefreshBtn()
        {
            //06082018 TTM: Người dùng muốn xài OutStandingTask thì click vào Refresh chứ không tự load. 
            //20191015 TBL: Mỗi lần bấm Refresh thì lại phải gọi xuống store để lấy danh sách phòng khám
            //GetAllConsultationTimeSegments();
            //GetDeptLocTreeViewFunction(((long)VRoomType.Khoa), (long)RoomFunction.KHAM_BENH);
            if (lstDeptLocation.Count > 0)
            {
                //GetAllRegisDeptLoc(lstDeptLocation,(int)cts.ConsultationTimeSegmentID);
                GetAllRegisDeptLoc(lstDeptLocation, (int)curConsultationTimeSegments.ConsultationTimeSegmentID);
                //20191015 TBL: Khi set cho curConsultationTimeSegments thì lại phải chạy hàm GetAllRegisDeptLoc
                //curConsultationTimeSegments = lstConsultationTimeSegments[0];
            }
        }

        private string _outstandingTitle;

        public string OutstandingTitle
        {
            get
            {
                return _outstandingTitle;
            }
            set
            {
                _outstandingTitle = value;
                NotifyOfPropertyChange(() => OutstandingTitle);
            }
        }
        
        #region properties

        private ObservableCollection<DeptLocInfo> _lstDeptLocInfo;
        public ObservableCollection<DeptLocInfo> lstDeptLocInfo
        {
            get
            {
                return _lstDeptLocInfo;
            }
            set
            {
                if (_lstDeptLocInfo == value)
                    return;
                _lstDeptLocInfo = value;
                NotifyOfPropertyChange(() => lstDeptLocInfo);
            }
        }

        private ObservableCollection<RefDepartmentsTree> _allRefDepartmentsTree;
        public ObservableCollection<RefDepartmentsTree> allRefDepartmentsTree
        {
            get
            {
                return _allRefDepartmentsTree;
            }
            set
            {
                if (_allRefDepartmentsTree == value)
                    return;
                _allRefDepartmentsTree = value;
                NotifyOfPropertyChange(() => allRefDepartmentsTree);
            }
        }

        private ObservableCollection<DeptLocation> _lstDeptLocation;
        public ObservableCollection<DeptLocation> lstDeptLocation
        {
            get
            {
                return _lstDeptLocation;
            }
            set
            {
                if (_lstDeptLocation == value)
                    return;
                _lstDeptLocation = value;
                NotifyOfPropertyChange(() => lstDeptLocation);
            }
        }

        private ObservableCollection<ConsultationTimeSegments> _lstConsultationTimeSegments;
        public ObservableCollection<ConsultationTimeSegments> lstConsultationTimeSegments
        {
            get
            {
                return _lstConsultationTimeSegments;
            }
            set
            {
                if (_lstConsultationTimeSegments == value)
                    return;
                _lstConsultationTimeSegments = value;
                NotifyOfPropertyChange(() => lstConsultationTimeSegments);
            }
        }

        private ConsultationTimeSegments _curConsultationTimeSegments;
        public ConsultationTimeSegments curConsultationTimeSegments
        {
            get
            {
                return _curConsultationTimeSegments;
            }
            set
            {
                if (_curConsultationTimeSegments == value)
                    return;
                _curConsultationTimeSegments = value;
                NotifyOfPropertyChange(() => curConsultationTimeSegments);
                if (curConsultationTimeSegments.ConsultationTimeSegmentID>0)
                {
                    GetAllRegisDeptLoc(lstDeptLocation, (int)curConsultationTimeSegments.ConsultationTimeSegmentID);
                }
            }
        }

        private ObservableCollection<Staff> _memAllStaff;
        public ObservableCollection<Staff> memAllStaff
        {
            get
            {
                return _memAllStaff;
            }
            set
            {
                if (_memAllStaff == value)
                    return;
                _memAllStaff = value;
                NotifyOfPropertyChange(() => memAllStaff);
            }
        }

        #endregion

        #region method

        private void GetDeptLocTreeViewFunction(long DeptType, long RoomFunction)
        {
            // su dung ham nay ne RefDepartments_Tree(strV_DeptType, ShowDeptLocation);
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z1026_G1_DSRoomType });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetDeptLocationFunc(DeptType, RoomFunction
                        , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var items = contract.EndGetDeptLocationFunc(asyncResult);
                                if (items != null)
                                {
                                    allRefDepartmentsTree = new ObservableCollection<RefDepartmentsTree>();
                                    lstDeptLocation = new ObservableCollection<DeptLocation>();
                                    lstDeptLocInfo = new ObservableCollection<DeptLocInfo>();
                                    foreach (var Department in items)
                                    {
                                        lstDeptLocation.Add(Department);
                                        DeptLocInfo dt = new DeptLocInfo();
                                        dt.DeptLocationID = Department.DeptLocationID;
                                        dt.Location = new Location();
                                        dt.Location.LocationName = Department.Location.LocationName;
                                        dt.RefDepartment = new RefDepartment();
                                        dt.RefDepartment.DeptName = Department.RefDepartment.DeptName;
                                        lstDeptLocInfo.Add(dt);
                                    }
                                    //GetAllRegisDeptLoc(lstDeptLocation,(int)cts.ConsultationTimeSegmentID);
                                    //20191015 TBL: Khi lấy được danh sách phòng khám thì không cần tự động gọi GetAllRegisDeptLoc
                                    //GetAllRegisDeptLoc(lstDeptLocation, 0);
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

        private void GetAllRegisDeptLoc(ObservableCollection<DeptLocation> lstDeptLocation, int ConsultationTimeSegmentID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetAllRegisDeptLocStaff(lstDeptLocation, ConsultationTimeSegmentID,Globals.DispatchCallback((asyncResult) =>
                        {
                            lstDeptLocInfo = new ObservableCollection<DeptLocInfo>();
                            
                            try
                            {
                                var item = client.EndGetAllRegisDeptLocStaff(asyncResult);
                                if (item != null && item.Count > 0)
                                {
                                    lstDeptLocInfo = item.ToObservableCollection();
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

                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void GetConsultationRoomStaffAllocations(ref string staffName,long DeptLocationID, long ConsultationTimeSegmentID)
        {
            string stName = "";
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetConsultationRoomStaffAllocations(DeptLocationID, ConsultationTimeSegmentID
                        , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetConsultationRoomStaffAllocations(asyncResult);
                                if (results != null)
                                {
                                    
                                    memAllStaff = new ObservableCollection<Staff>();
                                    if (results.Count > 0)
                                    {
                                        foreach (var RoomStaffAllocation in results)
                                        {
                                            if (RoomStaffAllocation.AllocationDate.ToShortDateString() == results[0].AllocationDate.ToShortDateString())
                                            {
                                                if (RoomStaffAllocation.Staff.RefStaffCategory.V_StaffCatType ==
                                                        (long)V_StaffCatType.BacSi)
                                                {
                                                    memAllStaff.Add(RoomStaffAllocation.Staff);
                                                    stName += ", " + RoomStaffAllocation.Staff.FullName;
                                                }
                                            }
                                        }
                                    }
                                }
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

            });
            staffName = stName;
            t.Start();
        }
        private void GetAllConsultationTimeSegments()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllConsultationTimeSegments(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetAllConsultationTimeSegments(asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                lstConsultationTimeSegments = new ObservableCollection<ConsultationTimeSegments>();
                                ConsultationTimeSegments tempCTimeSeg=new ConsultationTimeSegments();
                                tempCTimeSeg.SegmentName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.A0303_G1_Msg_InfoChonCaKham);
                                tempCTimeSeg.ConsultationTimeSegmentID = -1;
                                lstConsultationTimeSegments.Add(tempCTimeSeg);
                                foreach (var consTimeSeg in results)
                                {
                                    lstConsultationTimeSegments.Add(consTimeSeg);
                                    if (consTimeSeg.StartTime.Hour < Globals.ServerDate.Value.Hour && Globals.ServerDate.Value.Hour < consTimeSeg.EndTime.Hour)
                                    {
                                        cts = consTimeSeg;
                                    }
                                }
                                curConsultationTimeSegments = lstConsultationTimeSegments[0];
                            }
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

            });

            t.Start();
        }
        #endregion
      

    }
}
