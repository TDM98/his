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
    public class LoadHospitalListTask:IResult
    {
        public Exception Error { get; private set; }
        private ObservableCollection<Hospital> _HospitalList;
        public ObservableCollection<Hospital> HospitalList
        {
            get { return _HospitalList; }
        }
        private bool _addSelectedAllItem;
        private bool _addSelectOneItem;
        long? _type;
        public LoadHospitalListTask(long? type,bool addSelectOneItem = true, bool addSelectedAllItem = false)
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

                        contract.BeginHospital_ByCityProvinceID(_type,
                            Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<Hospital> allItems = new ObservableCollection<Hospital>();
                            try
                            {
                                allItems = contract.EndHospital_ByCityProvinceID(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                Error = ex;
                            }
                            finally
                            {
                                _HospitalList = new ObservableCollection<Hospital>(allItems);
                                if(_addSelectOneItem)
                                {
                                    Hospital firstItem = new Hospital();
                                    firstItem.HosID = -1;
                                    firstItem.HosName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0111_G1_HayChon1GTri);
                                    HospitalList.Insert(0, firstItem);
                                }
                                if (_addSelectedAllItem)
                                {
                                    Hospital firstItem = new Hospital();
                                    firstItem.HosID= -2;
                                    firstItem.HosName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                    HospitalList.Insert(0, firstItem);
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
