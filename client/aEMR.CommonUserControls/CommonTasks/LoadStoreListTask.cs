using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using Caliburn.Micro;
using DataEntities;

namespace aEMR.CommonTasks
{
    public class LoadStoreListTask:IResult
    {
        public Exception Error { get; private set; }
        private ObservableCollection<RefStorageWarehouseLocation> _lookupList;
        public ObservableCollection<RefStorageWarehouseLocation> LookupList
        {
            get { return _lookupList; }
        }
        private bool _addSelectedAllItem;
        private bool _addSelectOneItem;
        long? _type;
        bool? _bNo;
        long? _DeptID;
        bool? _IsNotSubStorage;
        public LoadStoreListTask(long? type,bool bNo=false,long? DeptID=null, bool addSelectOneItem = true, bool addSelectedAllItem = false, bool IsNotSubStorage = false)
        {
            _type = type;
            _bNo = bNo;
            _DeptID = DeptID;
            _addSelectOneItem = addSelectOneItem;
            _addSelectedAllItem = addSelectedAllItem;
            _IsNotSubStorage = IsNotSubStorage;
        }

        public LoadStoreListTask(List<long> refResponStoreID,long? type, bool bNo = false, bool addSelectOneItem = true, bool addSelectedAllItem = false)
        {
            _type = type;
            _bNo = bNo;
            _addSelectOneItem = addSelectOneItem;
            _addSelectedAllItem = addSelectedAllItem;
        }

        public void Execute(ActionExecutionContext context)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyStoragesServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllStoragesNotPaging(_type,_bNo,_DeptID, _IsNotSubStorage,
                            Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<RefStorageWarehouseLocation> allItems = new ObservableCollection<RefStorageWarehouseLocation>();
                            try
                            {
                                allItems = contract.EndGetAllStoragesNotPaging(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                Error = ex;
                            }
                            finally
                            {
                                _lookupList = new ObservableCollection<RefStorageWarehouseLocation>(allItems);
                                if(_addSelectOneItem)
                                {
                                    RefStorageWarehouseLocation firstItem = new RefStorageWarehouseLocation();
                                    firstItem.StoreID = -1;
                                    firstItem.swhlName = "-- Chọn một giá trị --";
                                    LookupList.Insert(0, firstItem);
                                }
                                if (_addSelectedAllItem)
                                {
                                    RefStorageWarehouseLocation firstItem = new RefStorageWarehouseLocation();
                                    firstItem.StoreID = 0;
                                    firstItem.swhlName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                    LookupList.Insert(0, firstItem);
                                }
                                Completed(this, new ResultCompletionEventArgs
                                {
                                    Error = null,
                                    WasCancelled = false
                                });

                                this.HideBusyIndicator();
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
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        public event EventHandler<ResultCompletionEventArgs> Completed;
    }
}
