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
    public class LoadRefGenericDrugCategory_2ListTask:IResult
    {
        public Exception Error { get; private set; }
        private ObservableCollection<RefGenericDrugCategory_2> _RefGenericDrugCategory_2List;
        public ObservableCollection<RefGenericDrugCategory_2> RefGenericDrugCategory_2List
        {
            get { return _RefGenericDrugCategory_2List; }
        }
        private bool _addSelectedAllItem;
        private bool _addSelectOneItem;
        public LoadRefGenericDrugCategory_2ListTask(bool addSelectOneItem = true, bool addSelectedAllItem = false)
        {
   
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
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginRefGenericDrugCategory_2_Get(0,
                            Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<RefGenericDrugCategory_2> allItems = new ObservableCollection<RefGenericDrugCategory_2>();
                            try
                            {
                                allItems = contract.EndRefGenericDrugCategory_2_Get(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                Error = ex;
                            }
                            finally
                            {
                                _RefGenericDrugCategory_2List = new ObservableCollection<RefGenericDrugCategory_2>(allItems);
                                if(_addSelectOneItem)
                                {
                                    RefGenericDrugCategory_2 firstItem = new RefGenericDrugCategory_2();
                                    firstItem.RefGenDrugCatID_2 = -1;
                                    firstItem.CategoryName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0111_G1_HayChon1GTri);
                                    RefGenericDrugCategory_2List.Insert(0, firstItem);
                                }
                                if (_addSelectedAllItem)
                                {
                                    RefGenericDrugCategory_2 firstItem = new RefGenericDrugCategory_2();
                                    firstItem.RefGenDrugCatID_2 = -2;
                                    firstItem.CategoryName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                    RefGenericDrugCategory_2List.Insert(0, firstItem);
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
