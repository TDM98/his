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
    public class LoadSupplierListTask:IResult
    {
        public Exception Error { get; private set; }
        private ObservableCollection<Supplier> _SupplierList;
        public ObservableCollection<Supplier> SupplierList
        {
            get { return _SupplierList; }
        }
        private bool _addSelectedAllItem;
        private bool _addSelectOneItem;
        int _type;
        public LoadSupplierListTask(int type,bool addSelectOneItem = true, bool addSelectedAllItem = false)
        {
            _type = type;
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
                    using (var serviceFactory = new PharmacySuppliersServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllSupplierCbx(_type,
                            Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<Supplier> allItems = new ObservableCollection<Supplier>();
                            try
                            {
                                allItems = contract.EndGetAllSupplierCbx(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                Error = ex;
                            }
                            finally
                            {
                                _SupplierList = new ObservableCollection<Supplier>(allItems);
                                if(_addSelectOneItem)
                                {
                                    Supplier firstItem = new Supplier();
                                    firstItem.SupplierID = -1;
                                    firstItem.SupplierName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0111_G1_HayChon1GTri);
                                    SupplierList.Insert(0, firstItem);
                                }
                                if (_addSelectedAllItem)
                                {
                                    Supplier firstItem = new Supplier();
                                    firstItem.SupplierID= -2;
                                    firstItem.SupplierName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                    SupplierList.Insert(0, firstItem);
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
