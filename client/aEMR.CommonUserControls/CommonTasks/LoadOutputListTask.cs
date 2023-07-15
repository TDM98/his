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
    public class LoadOutputListTask:IResult
    {
        public Exception Error { get; private set; }
        private ObservableCollection<RefOutputType> _RefOutputTypeList;
        public ObservableCollection<RefOutputType> RefOutputTypeList
        {
            get { return _RefOutputTypeList; }
        }
        private bool _addSelectedAllItem;
        private bool _addSelectOneItem;
        private bool _all;
        public LoadOutputListTask(bool addSelectOneItem = true, bool addSelectedAllItem = false,bool all=true)
        {
            _addSelectOneItem = addSelectOneItem;
            _addSelectedAllItem = addSelectedAllItem;
            _all = all;
        }

        public void Execute(ActionExecutionContext context)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginRefOutputType_Get(_all, Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<RefOutputType> allItems = new ObservableCollection<RefOutputType>();
                            try
                            {
                                allItems = contract.EndRefOutputType_Get(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                Error = ex;
                            }
                            finally
                            {
                                _RefOutputTypeList = new ObservableCollection<RefOutputType>(allItems);
                                if(_addSelectOneItem)
                                {
                                    RefOutputType firstItem = new RefOutputType();
                                    firstItem.TypID = -1;
                                    firstItem.TypName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0111_G1_HayChon1GTri);
                                    firstItem.TypNamePharmacy = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0111_G1_HayChon1GTri);
                                    RefOutputTypeList.Insert(0, firstItem);
                                }
                                if (_addSelectedAllItem)
                                {
                                    RefOutputType firstItem = new RefOutputType();
                                    firstItem.TypID = -2;
                                    firstItem.TypName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                    firstItem.TypNamePharmacy = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                    RefOutputTypeList.Insert(0, firstItem);
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
