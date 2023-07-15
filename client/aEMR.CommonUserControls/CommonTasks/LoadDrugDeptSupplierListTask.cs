using eHCMSLanguage;
using System;
using System.Collections.Generic;
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
    public class LoadDrugDeptSupplierListTask:IResult
    {
        public Exception Error { get; private set; }
        private ObservableCollection<DrugDeptSupplier> _DrugDeptSupplierList;
        public ObservableCollection<DrugDeptSupplier> DrugDeptSupplierList
        {
            get { return _DrugDeptSupplierList; }
        }
        private bool _addSelectedAllItem;
        private bool _addSelectOneItem;
        int _type;
        public LoadDrugDeptSupplierListTask(int type,bool addSelectOneItem = true, bool addSelectedAllItem = false)
        {
            _type = type;
            _addSelectOneItem = addSelectOneItem;
            _addSelectedAllItem = addSelectedAllItem;
        }

        public void Execute(ActionExecutionContext context)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacySuppliersServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginDrugDeptSupplier_GetCbx(_type,
                            Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<DrugDeptSupplier> allItems = new ObservableCollection<DrugDeptSupplier>();
                            try
                            {
                                allItems = contract.EndDrugDeptSupplier_GetCbx(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                Error = ex;
                            }
                            finally
                            {
                                _DrugDeptSupplierList = new ObservableCollection<DrugDeptSupplier>(allItems);
                                if(_addSelectOneItem)
                                {
                                    DrugDeptSupplier firstItem = new DrugDeptSupplier();
                                    firstItem.SupplierID = -1;
                                    firstItem.SupplierName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0111_G1_HayChon1GTri);
                                    DrugDeptSupplierList.Insert(0, firstItem);
                                }
                                if (_addSelectedAllItem)
                                {
                                    DrugDeptSupplier firstItem = new DrugDeptSupplier();
                                    firstItem.SupplierID= -2;
                                    firstItem.SupplierName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                    DrugDeptSupplierList.Insert(0, firstItem);
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
