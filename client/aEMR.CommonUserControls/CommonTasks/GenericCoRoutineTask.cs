using System;
using System.Threading;
using System.Collections.Generic;
using Caliburn.Micro;
using aEMR.ViewContracts;

namespace aEMR.CommonTasks
{
    public class GenericTask_WithWaitEvent
    {
        private System.Action<List<EventWaitHandle>> _executeAction;
        List<EventWaitHandle> WaitThreadEnd_Handles = null;
        private bool _isDlgBusyIndicator = false;
        public GenericTask_WithWaitEvent(System.Action<List<EventWaitHandle>> theLoadingFunc, int NumThreadCnt, bool isDlgBusyIndicator = false)
        {            
            WaitThreadEnd_Handles = new List<EventWaitHandle>();
            for (int idx = 0; idx < NumThreadCnt; ++idx)
            {
                WaitThreadEnd_Handles.Add(new EventWaitHandle(false, EventResetMode.AutoReset));                
            }
            _executeAction = new System.Action<List<EventWaitHandle>>(theLoadingFunc);
            _isDlgBusyIndicator = isDlgBusyIndicator;
        }

        public void ExecuteGenericTask(GenericCoRoutineTask theGenTask)
        {            
            if (_isDlgBusyIndicator)
            {
                this.DlgShowBusyIndicator();
            }
            else
            {
                this.ShowBusyIndicator();
            }
            var t = new Thread(() =>
            {
                _executeAction(WaitThreadEnd_Handles);                    
                WaitHandle.WaitAll(WaitThreadEnd_Handles.ToArray()); 
                if (_isDlgBusyIndicator)
                {
                    this.DlgHideBusyIndicator();
                }
                else
                {
                    this.HideBusyIndicator();
                }
                if (theGenTask != null)
                {
                    theGenTask.ActionComplete(true);
                }
                
            });
            t.Start();

        }
    }


    public class GenericCoRoutineTask : IResult
    {
        private object _objParam1 = null;
        private object _objParam2 = null;
        private object _objParam3 = null;
        private object _objParam4 = null;
        private object _objParam5 = null;

        private Exception _error = null;
        public Exception Error
        {
            get { return _error; }
            set
            {
                _error = value;
            }
        }

        private List<object> _listResultObjs;

        private System.Action<GenericCoRoutineTask> _executeAction;
        private System.Action<GenericCoRoutineTask, object> _executeActionOneParam;
        private System.Action<GenericCoRoutineTask, object, object> _executeActionTwoParams;
        private System.Action<GenericCoRoutineTask, object, object, object> _executeActionThreeParams;
        private System.Action<GenericCoRoutineTask, object, object, object, object> _executeActionFourParams;
        private System.Action<GenericCoRoutineTask, object, object, object, object, object> _executeActionFiveParams;

        public GenericCoRoutineTask(System.Action<GenericCoRoutineTask> exeAction)
        {
            this._executeAction = new System.Action<GenericCoRoutineTask>(exeAction);
            _listResultObjs = new List<object>();
        }
        public GenericCoRoutineTask(System.Action<GenericCoRoutineTask, object> exeAction, object param1)
        {
            this._executeActionOneParam = new System.Action<GenericCoRoutineTask, object>(exeAction);
            _listResultObjs = new List<object>();
            _objParam1 = param1;
        }
        public GenericCoRoutineTask(System.Action<GenericCoRoutineTask, object, object> exeAction, object param1, object param2)
        {
            this._executeActionTwoParams = new System.Action<GenericCoRoutineTask, object, object>(exeAction);
            _listResultObjs = new List<object>();
            _objParam1 = param1;
            _objParam2 = param2;
        }
        public GenericCoRoutineTask(System.Action<GenericCoRoutineTask, object, object, object> exeAction, object param1, object param2, object param3)
        {
            this._executeActionThreeParams = new System.Action<GenericCoRoutineTask, object, object, object>(exeAction);
            _listResultObjs = new List<object>();
            _objParam1 = param1;
            _objParam2 = param2;
            _objParam3 = param3;
        }
        public GenericCoRoutineTask(System.Action<GenericCoRoutineTask, object, object, object, object> exeAction, object param1, object param2, object param3, object param4)
        {
            this._executeActionFourParams = new System.Action<GenericCoRoutineTask, object, object, object, object>(exeAction);
            _listResultObjs = new List<object>();
            _objParam1 = param1;
            _objParam2 = param2;
            _objParam3 = param3;
            _objParam4 = param4;
        }
        public GenericCoRoutineTask(System.Action<GenericCoRoutineTask, object, object, object, object, object> exeAction, object param1, object param2, object param3, object param4, object param5)
        {
            this._executeActionFiveParams = new System.Action<GenericCoRoutineTask, object, object, object, object, object>(exeAction);
            _listResultObjs = new List<object>();
            _objParam1 = param1;
            _objParam2 = param2;
            _objParam3 = param3;
            _objParam4 = param4;
            _objParam5 = param5;
        }


        public void AddResultObj(object objRes)
        {            
            _listResultObjs.Add(objRes);
        }

        public object GetResultObj(int nIdx)
        {
            if (nIdx >= _listResultObjs.Count)
            {
                return null;
            }

            return _listResultObjs[nIdx];

        }

        public void ActionComplete(bool bContinue)
        {
            Completed(this, new ResultCompletionEventArgs
                            {
                                Error = null,
                                WasCancelled = !bContinue
                            });
        }

        public void Execute(ActionExecutionContext context)
        {
            if (_objParam5 != null)
            {
                _executeActionFiveParams(this, _objParam1, _objParam2, _objParam3, _objParam4, _objParam5);
            }
            else if (_objParam4 != null)
            {
                _executeActionFourParams(this, _objParam1, _objParam2, _objParam3, _objParam4);
            }
            else if (_objParam3 != null)
            {
                _executeActionThreeParams(this, _objParam1, _objParam2, _objParam3);
            }
            else if (_objParam2 != null)
            {
                _executeActionTwoParams(this, _objParam1, _objParam2);
            }
            else if (_objParam1 != null)
            {
                _executeActionOneParam(this, _objParam1);
            }
            else
            {
                _executeAction(this);
            }
        }

        public event EventHandler<ResultCompletionEventArgs> Completed;

        public static IResult StartTask(System.Action<GenericCoRoutineTask> exeAction)
        {
            return new GenericCoRoutineTask(exeAction);
        }
        public static IResult StartTask(System.Action<GenericCoRoutineTask, object> exeActionOneParam, object param1)
        {
            return new GenericCoRoutineTask(exeActionOneParam, param1);
        }
        public static IResult StartTask(System.Action<GenericCoRoutineTask, object, object> exeActionTwoParams, object param1, object param2)
        {
            return new GenericCoRoutineTask(exeActionTwoParams, param1, param2);
        }
        public static IResult StartTask(System.Action<GenericCoRoutineTask, object, object, object> exeActionThreeParams, object param1, object param2, object param3)
        {
            return new GenericCoRoutineTask(exeActionThreeParams, param1, param2, param3);
        }
        public static IResult StartTask(System.Action<GenericCoRoutineTask, object, object, object, object> exeActionFourParams, object param1, object param2, object param3, object param4)
        {
            return new GenericCoRoutineTask(exeActionFourParams, param1, param2, param3, param4);
        }
    }
}