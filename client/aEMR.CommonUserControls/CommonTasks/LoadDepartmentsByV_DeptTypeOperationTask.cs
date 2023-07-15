using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using aEMR.Common.Collections;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using Caliburn.Micro;
using DataEntities;

namespace aEMR.CommonTasks
{
    public class LoadDepartmentsByV_DeptTypeOperationTask : IResult
    {
        public Exception Error { get; private set; }

        private ObservableCollection<RefDepartment> _departments;
        public ObservableCollection<RefDepartment> Departments
        {
            get { return _departments; }
        }

        private ObservableCollection<long> _LstRefDepartment;
        private bool isCheck;
        public ObservableCollection<long> LstRefDepartment
        {
            get { return _LstRefDepartment; }
            set { _LstRefDepartment = value; }
        }

        private long _V_DeptTypeOperation;
        public long V_DeptTypeOperation
        {
            get { return _V_DeptTypeOperation; }
            set { _V_DeptTypeOperation = value; }
        }

        private IList<long> V_DeptTypeOperationCollection { get; set; }
        private bool IsGetAllDept = false;
        public LoadDepartmentsByV_DeptTypeOperationTask()
        {
        }
        public LoadDepartmentsByV_DeptTypeOperationTask(bool aIsGetAllDept = false)
        {
            IsGetAllDept = aIsGetAllDept;
        }
        public LoadDepartmentsByV_DeptTypeOperationTask(long pV_DeptTypeOperation)
        {
            isCheck = false;
            this.V_DeptTypeOperation = pV_DeptTypeOperation;
        }
        public LoadDepartmentsByV_DeptTypeOperationTask(long pV_DeptTypeOperation, ObservableCollection<long> LstRefDepartment)
        {
            isCheck = true;
            this.V_DeptTypeOperation = pV_DeptTypeOperation;
            this.LstRefDepartment = LstRefDepartment;
        }
        public LoadDepartmentsByV_DeptTypeOperationTask(IList<long> aV_DeptTypeOperationCollection)
        {
            isCheck = false;
            V_DeptTypeOperationCollection = aV_DeptTypeOperationCollection;
        }
        public void Execute(ActionExecutionContext context)
        {
            if (Globals.AllRefDepartmentList != null && Globals.AllRefDepartmentList.Count > 0)
            {
                if (isCheck)
                {
                    _departments = new ObservableCollection<RefDepartment>();
                    if (LstRefDepartment != null && LstRefDepartment.Count > 0)
                    {
                        foreach (var globalDept in Globals.AllRefDepartmentList)
                        {
                            foreach (var authDeptID in LstRefDepartment)
                            {
                                if ((globalDept.V_DeptTypeOperation.HasValue) && (globalDept.V_DeptTypeOperation.Value == V_DeptTypeOperation) && (globalDept.DeptID == authDeptID))
                                {
                                    _departments.Add(globalDept);
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (V_DeptTypeOperationCollection != null && V_DeptTypeOperationCollection.Count > 0)
                {
                    _departments = new ObservableCollection<RefDepartment>();
                    foreach (var mDeptTypeOperation in V_DeptTypeOperationCollection)
                    {
                        foreach (var gDept in Globals.AllRefDepartmentList.Where(x => x.V_DeptTypeOperation == mDeptTypeOperation))
                        {
                            _departments.Add(gDept);
                        }
                    }
                }
                else if (IsGetAllDept && V_DeptTypeOperation == 0 && Globals.AllRefDepartmentList != null)
                {
                    _departments = Globals.AllRefDepartmentList.ToObservableCollection();
                }
                else
                {
                    _departments = new ObservableCollection<RefDepartment>();
                    foreach (var gDept in Globals.AllRefDepartmentList)
                    {
                        if (gDept.V_DeptTypeOperation.Value == V_DeptTypeOperation)
                        {
                            _departments.Add(gDept);
                        }
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

                        contract.BeginGetAllDepartmentsByV_DeptTypeOperation(V_DeptTypeOperation,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var allDepts = contract.EndGetAllDepartmentsByV_DeptTypeOperation(asyncResult);
                                    if (allDepts != null)
                                    {
                                        if (isCheck)
                                        {
                                            _departments = new ObservableCollection<RefDepartment>();
                                            if (this.LstRefDepartment != null && this.LstRefDepartment.Count > 0)
                                            {
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
                                        }
                                        else
                                        {
                                            _departments = new ObservableCollection<RefDepartment>(allDepts);
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