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
using aEMR.Common.Collections;
using System.Linq;

namespace aEMR.CommonTasks
{
    public class LoadNationalityListTask : IResult
    {
        public Exception Error { get; private set; }
        private ObservableCollection<RefNationality> _RefNationalityList;
        public ObservableCollection<RefNationality> RefNationalityList
        {
            get { return _RefNationalityList; }
        }

        private bool _addSelectedAllItem;
        private bool _addSelectOneItem;
        public LoadNationalityListTask(bool addSelectOneItem = true, bool addSelectedAllItem = false)
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

                        contract.BeginGetAllNationalities(Globals.DispatchCallback((asyncResult) =>
                        {                         
                            try
                            {
                                var allItems = contract.EndGetAllNationalities(asyncResult);
                                if (allItems != null)
                                {
                                    Globals.allNationalities = allItems.ToList();
                                    _RefNationalityList = Globals.allNationalities.ToObservableCollection();
                                }
                            }
                            catch (Exception ex)
                            {
                                Error = ex;
                            }
                            finally
                            {                                
                                if (_addSelectOneItem)
                                {
                                    RefNationality firstItem = new RefNationality();
                                    firstItem.NationalityID = -1;
                                    firstItem.NationalityName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0111_G1_HayChon1GTri);
                                    RefNationalityList.Insert(0, firstItem);
                                }
                                if (_addSelectedAllItem)
                                {
                                    RefNationality firstItem = new RefNationality();
                                    firstItem.NationalityID = -2;
                                    firstItem.NationalityName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                    RefNationalityList.Insert(0, firstItem);
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
