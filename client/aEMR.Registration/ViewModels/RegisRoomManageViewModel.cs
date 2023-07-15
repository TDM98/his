using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Controls;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using Caliburn.Micro;
using aEMR.ViewContracts;
using System.ComponentModel.Composition;
using DataEntities;
using aEMR.CommonTasks;
using eHCMS.CommonUserControls.CommonTasks;
using Castle.Windsor;

namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IRegisRoomManage))]
    public class RegisRoomManageViewModel : Conductor<object>, IRegisRoomManage
    {
        [ImportingConstructor]
        public RegisRoomManageViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            LoadRefDept();
            SeachCriteria = new SeachPtRegistrationCriteria();
            SeachCriteria.FromDate = Globals.GetCurServerDateTime();
            SeachCriteria.ToDate = Globals.GetCurServerDateTime();
        }
        
        #region properties
        private decimal _PaymentSum;
        public decimal PaymentSum
        {
            get
            {
                return _PaymentSum;
            }
            set
            {
                if (_PaymentSum != value)
                {
                    _PaymentSum = value;
                    NotifyOfPropertyChange(() => PaymentSum);
                }
            }
        }

        private bool _IsEdit=true;
        public bool IsEdit
        {
            get
            {
                return _IsEdit;
            }
            set
            {
                if (_IsEdit != value)
                {
                    _IsEdit = value;
                    NotifyOfPropertyChange(() => IsEdit);
                }
            }
        }

        private SeachPtRegistrationCriteria _SeachCriteria;
        public SeachPtRegistrationCriteria SeachCriteria
        {
            get
            {
                return _SeachCriteria;
            }
            set
            {
                _SeachCriteria = value;
                NotifyOfPropertyChange(() => SeachCriteria);
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
                    SeachCriteria.DeptID = 0;
                }
                else
                {
                    SeachCriteria.DeptID = curNewDepartments.DeptID;
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
                SeachCriteria.DeptLocationID=SelectedLocation != null && SelectedLocation.DeptLocationID>0?
                    SelectedLocation.DeptLocationID:0;                
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
                
        #endregion

        public void Dept_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        public void LocCbx_SelectionChanged(object sender, SelectionChangedEventArgs e) 
        {
        
        }

        public void SearchCmd() 
        {

        }

        public void LoadRefDept()
        {
            Coroutine.BeginExecute(NewLoadDepartments());
        }
        private IEnumerator<IResult> NewLoadDepartments()
        {
            ObservableCollection<RefDepartment> tempDepartments = new ObservableCollection<RefDepartment>();
            var departmentTask = new LoadDepartmentsByV_DeptTypeOperationTask((long)V_DeptTypeOperation.KhoaNoi);
            yield return departmentTask;
            NewDepartments = departmentTask.Departments;            
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
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format("{0}.", eHCMSResources.Z0115_G1_LayDSPgBan) });

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

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            mBaoCaoTT_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mReportPaymentReceipt,
                                               (int)oRegistrionEx.mBaoCaoTT_Xem, (int)ePermission.mView);
            
        }
        #region checking account

        private bool _mBaoCaoTT_Xem = true;

        public bool mBaoCaoTT_Xem
        {
            get
            {
                return _mBaoCaoTT_Xem;
            }
            set
            {
                if (_mBaoCaoTT_Xem == value)
                    return;
                _mBaoCaoTT_Xem = value;
                NotifyOfPropertyChange(() => mBaoCaoTT_Xem);
            }
        }



        #endregion
    }
}