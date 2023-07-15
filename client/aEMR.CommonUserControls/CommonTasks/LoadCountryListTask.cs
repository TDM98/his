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
    public class LoadCountryListTask:IResult
    {
        public Exception Error { get; private set; }
        private ObservableCollection<RefCountry> _RefCountryList;
        public ObservableCollection<RefCountry> RefCountryList
        {
            get { return _RefCountryList; }
        }

        private bool _addSelectedAllItem;
        private bool _addSelectOneItem;
        public LoadCountryListTask(bool addSelectOneItem = true, bool addSelectedAllItem = false)
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

                        contract.BeginGetAllCountries(
                            Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<RefCountry> allItems = new ObservableCollection<RefCountry>();
                            try
                            {
                                allItems = contract.EndGetAllCountries(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                Error = ex;
                            }
                            finally
                            {
                                _RefCountryList = new ObservableCollection<RefCountry>(allItems);
                                if(_addSelectOneItem)
                                {
                                    RefCountry firstItem = new RefCountry();
                                    firstItem.CountryID = -1;
                                    firstItem.CountryName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0111_G1_HayChon1GTri);
                                    RefCountryList.Insert(0, firstItem);
                                }
                                if (_addSelectedAllItem)
                                {
                                    RefCountry firstItem = new RefCountry();
                                    firstItem.CountryID= -2;
                                    firstItem.CountryName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                    RefCountryList.Insert(0, firstItem);
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
