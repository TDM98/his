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
    public class LoadRefGenericDrugCategory_1ListTask:IResult
    {
        public Exception Error { get; private set; }
        private ObservableCollection<RefGenericDrugCategory_1> _RefGenericDrugCategory_1List;
        public ObservableCollection<RefGenericDrugCategory_1> RefGenericDrugCategory_1List
        {
            get { return _RefGenericDrugCategory_1List; }
        }
        private bool _addSelectedAllItem;
        private bool _addSelectOneItem;
        long _type;
        public LoadRefGenericDrugCategory_1ListTask(long type,bool addSelectOneItem = true, bool addSelectedAllItem = false)
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
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginRefGenericDrugCategory_1_Get(_type,
                            Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<RefGenericDrugCategory_1> allItems = new ObservableCollection<RefGenericDrugCategory_1>();
                            try
                            {
                                allItems = contract.EndRefGenericDrugCategory_1_Get(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                Error = ex;
                            }
                            finally
                            {
                                _RefGenericDrugCategory_1List = new ObservableCollection<RefGenericDrugCategory_1>(allItems);
                                if(_addSelectOneItem)
                                {
                                    RefGenericDrugCategory_1 firstItem = new RefGenericDrugCategory_1();
                                    firstItem.RefGenDrugCatID_1 = -1;
                                    firstItem.CategoryName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0111_G1_HayChon1GTri);
                                    RefGenericDrugCategory_1List.Insert(0, firstItem);
                                }
                                if (_addSelectedAllItem)
                                {
                                    RefGenericDrugCategory_1 firstItem = new RefGenericDrugCategory_1();
                                    firstItem.RefGenDrugCatID_1 = -2;
                                    firstItem.CategoryName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                    RefGenericDrugCategory_1List.Insert(0, firstItem);
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
