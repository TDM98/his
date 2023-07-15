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
    public class LoadLookupListTask:IResult
    {
        public Exception Error { get; private set; }
        private ObservableCollection<Lookup> _lookupList;
        public ObservableCollection<Lookup> LookupList
        {
            get { return _lookupList; }
        }
        private bool _addSelectedAllItem;
        private bool _addSelectOneItem;
        LookupValues _type;
        public LoadLookupListTask(LookupValues type,bool addSelectOneItem = true, bool addSelectedAllItem = false)
        {
            _type = type;
            _addSelectOneItem = addSelectOneItem;
            _addSelectedAllItem = addSelectedAllItem;
        }

        // Txd 20/12/2013: All Lookup Values has been READ in by LoginViewModel during login process
        // thus we DO NOT NEED to go to the SERVER anymore
        // JUST change IT HERE instead of changing the CODE everywhere this task is USED
        public void Execute(ActionExecutionContext context)
        {
            _lookupList = new ObservableCollection<Lookup>(Globals.GetAllLookupValuesByType((long)_type, _addSelectOneItem, _addSelectedAllItem));
            Completed(this, new ResultCompletionEventArgs
            {
                Error = null,
                WasCancelled = false
            });
        }

        //public void Execute(ActionExecutionContext context)
        //{
        //    var t = new Thread(() =>
        //    {
        //        try
        //        {
        //            using (var serviceFactory = new CommonServiceClient())
        //            {
        //                var contract = serviceFactory.ServiceInstance;

        //                contract.BeginGetAllLookupValuesByType(_type, Globals.DispatchCallback((asyncResult) =>
        //                {
        //                    IList<Lookup> allItems = new ObservableCollection<Lookup>();
        //                    try
        //                    {
        //                        allItems = contract.EndGetAllLookupValuesByType(asyncResult);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        Error = ex;
        //                    }
        //                    finally
        //                    {
        //                        _lookupList = new ObservableCollection<Lookup>(allItems);
        //                        if(_addSelectOneItem)
        //                        {
        //                            Lookup firstItem = new Lookup();
        //                            firstItem.LookupID = -1;
        //                            firstItem.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0111_G1_HayChon1GTri);
        //                            LookupList.Insert(0, firstItem);
        //                        }
        //                        if (_addSelectedAllItem)
        //                        {
        //                            Lookup firstItem = new Lookup();
        //                            firstItem.LookupID = -2;
        //                            firstItem.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
        //                            LookupList.Insert(0, firstItem);
        //                        }
        //                        Completed(this, new ResultCompletionEventArgs
        //                        {
        //                            Error = null,
        //                            WasCancelled = false
        //                        });
        //                    }

        //                }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Error = ex;
        //            Completed(this, new ResultCompletionEventArgs
        //            {
        //                Error = null,
        //                WasCancelled = false
        //            });
        //        }
        //    });
        //    t.Start();
        //}

        public event EventHandler<ResultCompletionEventArgs> Completed;
    }
}
