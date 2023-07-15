using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

using aEMR.Infrastructure;
using aEMR.ServiceClient;
using Caliburn.Micro;
using DataEntities;

namespace aEMR.CommonTasks
{
    public class LoadListStoreForResponTask:IResult
    {
        public Exception Error { private get; set; }
        private ObservableCollection<RefStorageWarehouseLocation> _lookupList;
        public ObservableCollection<RefStorageWarehouseLocation> LookupList
        {
            get { return _lookupList; }
        }
        long? _type;
        bool? _bNo;
        private bool _addSelectedAllItem;
        private bool _addSelectOneItem;
        private List<long> _allListStore;
        public LoadListStoreForResponTask(List<long> allListStore,long? type,bool? bNo, bool addSelectOneItem = true, bool addSelectedAllItem = false)
        {
            _type=type;
            _bNo=bNo;
            _allListStore = allListStore;
            _addSelectOneItem = addSelectOneItem;
            _addSelectedAllItem = addSelectedAllItem;
        }
        public void Execute(ActionExecutionContext context)
        {
            var t = new Thread
            (()=>
                {
                    try
                    {
                        using(var serviceFactory=new PharmacyStoragesServiceClient())
                        {
                            var contract = serviceFactory.ServiceInstance;
                            contract.BeginGetAllStorages_ForRespon(_allListStore,_type,_bNo
                                , Globals.DispatchCallback((asyncResult) =>
                                    {
                                        IList<RefStorageWarehouseLocation> allItems = new ObservableCollection<RefStorageWarehouseLocation>();
                                        try
                                        {
                                            allItems = contract.EndGetAllStorages_ForRespon(asyncResult);
                                        }
                                        catch (Exception ex)
                                        {
                                            Error = ex;
                                        }
                                        finally
                                        {
                                            _lookupList = new ObservableCollection<RefStorageWarehouseLocation>(allItems);
                                            Completed(this, new ResultCompletionEventArgs
                                            {
                                                Error = null,
                                                WasCancelled = false
                                            });
                                        }
                                    }
                                
                            ),null);
                        }
                             
                    }catch(Exception ex)
                    {
                        Error = ex;
                        Completed(this, new ResultCompletionEventArgs
                        {
                            Error = null,
                            WasCancelled = false
                        });
                    }
                }
            );
            t.Start();
        }

        public event EventHandler<ResultCompletionEventArgs> Completed;

    }
}
