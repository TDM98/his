using System;

using Caliburn.Micro;
using System.Collections.ObjectModel;
using DataEntities;
using System.Threading;
using aEMR.ServiceClient;
using aEMR.Infrastructure;
using aEMR.ViewContracts;

namespace eHCMS.CommonUserControls.CommonTasks
{
    public class LoadDeptLoctionByIDTask:IResult
    {
        public Exception Error { get; private set; }
        public ObservableCollection<DeptLocation> DeptLocations;
        private long ID;
        private long? RoomFunction=null;

        public LoadDeptLoctionByIDTask(long _id) 
        {
            ID = _id;
            
            //if(ID == Globals.KhoaPhongKham )
            // Txd 25/05/2014 Replaced ConfigList
            if (ID == Globals.ServerConfigSection.Hospitals.KhoaPhongKham)
            {
                //RoomFunction = Globals.DiagRoomFunction;
                RoomFunction = Globals.ServerConfigSection.Hospitals.RoomFunction;
            }
            
        }
        //public LoadDeptLoctionByIDTask(long _id, long _RoomFunction)
        //{
        //    ID = _id;
        //    RoomFunction=_RoomFunction;
        //}
        public void Execute(ActionExecutionContext context) 
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetAllLocationsByDeptID(ID, RoomFunction, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var allItems = contract.EndGetAllLocationsByDeptID(asyncResult);

                            if (allItems != null)
                            {
                                DeptLocations = new ObservableCollection<DeptLocation>(allItems);
                            }
                            else
                            {
                                DeptLocations = new ObservableCollection<DeptLocation>();
                            }
                            
                        }
                        catch (Exception ex)
                        {
                            Error = ex;
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                            Completed(this, new ResultCompletionEventArgs
                            {
                                Error = null,
                                WasCancelled = false
                            });
                        }
                    }), null);
                }


            });
            t.Start();
        }

        public event EventHandler<ResultCompletionEventArgs> Completed;        
    }
}
