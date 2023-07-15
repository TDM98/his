using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure.GlobalFuncs;
using aEMR.ServiceClient;
using Caliburn.Micro;
using DataEntities;

namespace aEMR.CommonTasks
{
    public class LoadDepartmentsTask : IResult
    {
        public Exception Error { get; private set; }

        private ObservableCollection<RefDepartment> _departments;

        public ObservableCollection<RefDepartment> Departments
        {
            get { return _departments; }
        }

        private ObservableCollection<long> _LstRefDepartment;

        public ObservableCollection<long> LstRefDepartment
        {
            get { return _LstRefDepartment; }
            set { _LstRefDepartment = value; }
        }

        private long _curDeptID;

        public long curDeptID
        {
            get { return _curDeptID; }
            set { _curDeptID = value; }
        }

        private bool _addSelectedAllItem;
        private bool _addSelectOneItem;
        public LoadDepartmentsTask(bool addSelectOneItem = false, bool addSelectedAllItem = false)
        {
            _addSelectOneItem = addSelectOneItem;
            _addSelectedAllItem = addSelectedAllItem;
        }

        public LoadDepartmentsTask(ObservableCollection<long> LstRefDepartment, bool addSelectOneItem = false, bool addSelectedAllItem = false)
        {
            this.LstRefDepartment = LstRefDepartment;
            _addSelectOneItem = addSelectOneItem;
            _addSelectedAllItem = addSelectedAllItem;
        }

        public LoadDepartmentsTask(long curDeptID, bool addSelectOneItem = false, bool addSelectedAllItem = false)
        {
            this.curDeptID = curDeptID;
            _addSelectOneItem = addSelectOneItem;
            _addSelectedAllItem = addSelectedAllItem;
        }


        public void Execute(ActionExecutionContext context)
        {
            if (Globals.AllRefDepartmentList != null && Globals.AllRefDepartmentList.Count > 0)
            {
                _departments = new ObservableCollection<RefDepartment>();
                if (LstRefDepartment != null && LstRefDepartment.Count > 0)
                {
                    foreach (var globalDept in Globals.AllRefDepartmentList)
                    {
                        foreach (var authDeptID in LstRefDepartment)
                        {
                            if (globalDept.V_DeptTypeOperation.HasValue && (globalDept.V_DeptTypeOperation.Value == (long)(V_DeptTypeOperation.KhoaNoi))
                                                                        && globalDept.DeptID == authDeptID)
                            {
                                _departments.Add(globalDept);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (curDeptID > 0)
                    {
                        foreach (var globalDept2 in Globals.AllRefDepartmentList)
                        {
                            if (globalDept2.V_DeptTypeOperation.HasValue && (globalDept2.V_DeptTypeOperation.Value == (long)V_DeptTypeOperation.KhoaNoi)
                                                                        && globalDept2.DeptID == curDeptID)
                            {
                                _departments.Add(globalDept2);
                                break;
                            }
                        }
                    }
                    else
                    {
                        _departments = new ObservableCollection<RefDepartment>();
                        foreach (var gDept in Globals.AllRefDepartmentList)
                        {
                            if (gDept.V_DeptTypeOperation.Value == (long)V_DeptTypeOperation.KhoaNoi)
                            {
                                _departments.Add(gDept);
                            }
                        }                    
                    }
                }

                if (_departments != null && _departments.Count > 0)
                {
                    if (_addSelectOneItem)
                    {
                        RefDepartment firstItem = new RefDepartment();
                        firstItem.DeptID = 0;
                        firstItem.DeptName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0493_G1_HayChonKhoa);
                        _departments.Insert(0, firstItem);
                    }
                    else if (_addSelectedAllItem)
                    {
                        RefDepartment firstItem = new RefDepartment();
                        firstItem.DeptID = 0;
                        firstItem.DeptName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                        _departments.Insert(0, firstItem);
                    }
                }

                Completed(this, new ResultCompletionEventArgs
                {
                    Error = null,
                    WasCancelled = false
                });
                return;
            }


            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllDepartmentsByV_DeptTypeOperation((long)V_DeptTypeOperation.KhoaNoi,                        
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var allDepts = contract.EndGetAllDepartmentsByV_DeptTypeOperation(asyncResult);
                                    if (allDepts != null)
                                    {
                                        if (this.LstRefDepartment != null && this.LstRefDepartment.Count > 0)
                                        {
                                            _departments = new ObservableCollection<RefDepartment>();
                                            foreach (var refDepartment in allDepts)
                                            {
                                                foreach (var department in LstRefDepartment)
                                                {
                                                    if (refDepartment.DeptID == department)
                                                    {
                                                        _departments.Add(refDepartment);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (this.curDeptID > 0)
                                            {
                                                _departments = new ObservableCollection<RefDepartment>();
                                                foreach (var refDepartment in allDepts)
                                                {
                                                    if (refDepartment.DeptID != curDeptID)
                                                    {
                                                        _departments.Add(refDepartment);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                _departments = new ObservableCollection<RefDepartment>(allDepts);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        _departments = null;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Error = ex;
                                }
                                finally
                                {
                                    if (_addSelectOneItem)
                                    {
                                        RefDepartment firstItem = new RefDepartment();
                                        firstItem.DeptID = 0;
                                        firstItem.DeptName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0493_G1_HayChonKhoa);
                                        _departments.Insert(0, firstItem);
                                    }
                                    else if (_addSelectedAllItem)
                                    {
                                        RefDepartment firstItem = new RefDepartment();
                                        firstItem.DeptID = 0;
                                        firstItem.DeptName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                        _departments.Insert(0, firstItem);
                                    }
                                    Completed(this, new ResultCompletionEventArgs
                                    {
                                        Error = null,
                                        WasCancelled = false
                                    });
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Error = ex;
                    Completed(this, new ResultCompletionEventArgs
                    {
                        Error = null,
                        WasCancelled = false
                    });
                }
            });
            t.Start();
        }

        public event EventHandler<ResultCompletionEventArgs> Completed;
    }
}



        //public void Execute(ActionExecutionContext context)
        //{
        //    var t = new Thread(() =>
        //    {
        //        try
        //        {
        //            using (var serviceFactory = new CommonServiceClient())
        //            {
        //                var contract = serviceFactory.ServiceInstance;

        //                contract.BeginGetAllDepartments(
        //                    Globals.DispatchCallback((asyncResult) =>
        //                    {
        //                        try
        //                        {
        //                            var allDepts = contract.EndGetAllDepartments(asyncResult);
        //                            if (allDepts != null)
        //                            {
        //                                if (this.LstRefDepartment != null && this.LstRefDepartment.Count > 0)
        //                                {
        //                                    _departments = new ObservableCollection<RefDepartment>();
        //                                    foreach (var refDepartment in allDepts)
        //                                    {
        //                                        foreach (var department in LstRefDepartment)
        //                                        {
        //                                            if (refDepartment.DeptID == department)
        //                                            {
        //                                                _departments.Add(refDepartment);
        //                                                break;
        //                                            }
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    if (this.curDeptID > 0)
        //                                    {
        //                                        _departments = new ObservableCollection<RefDepartment>();
        //                                        foreach (var refDepartment in allDepts)
        //                                        {
        //                                            if (refDepartment.DeptID != curDeptID)
        //                                            {
        //                                                _departments.Add(refDepartment);
        //                                            }
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        _departments = new ObservableCollection<RefDepartment>(allDepts);
        //                                    }
        //                                }
        //                            }
        //                            else
        //                            {
        //                                _departments = null;
        //                            }
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            Error = ex;
        //                        }
        //                        finally
        //                        {
        //                            if (_addSelectOneItem)
        //                            {
        //                                RefDepartment firstItem = new RefDepartment();
        //                                firstItem.DeptID = 0;
        //                                firstItem.DeptName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0111_G1_HayChon1GTri);
        //                                _departments.Insert(0, firstItem);
        //                            }
        //                            else if (_addSelectedAllItem)
        //                            {
        //                                RefDepartment firstItem = new RefDepartment();
        //                                firstItem.DeptID = 0;
        //                                firstItem.DeptName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
        //                                _departments.Insert(0, firstItem);
        //                            }
        //                            Completed(this, new ResultCompletionEventArgs
        //                            {
        //                                Error = null,
        //                                WasCancelled = false
        //                            });
        //                        }
        //                    }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Error = ex;
        //            Completed(this, new ResultCompletionEventArgs
        //            {
        //                Error = null,
        //                WasCancelled = false
        //            });
        //        }
        //    });
        //    t.Start();
        //}