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
    public class LoadUnitListTask:IResult
    {
        public Exception Error { get; private set; }
        private ObservableCollection<RefUnit> _RefUnitList;
        public ObservableCollection<RefUnit> RefUnitList
        {
            get { return _RefUnitList; }
        }
        private bool _addSelectedAllItem;
        private bool _addSelectOneItem;
        public LoadUnitListTask(bool addSelectOneItem = true, bool addSelectedAllItem = false)
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
                    using (var serviceFactory = new PharmacyUnitsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetUnits(
                            Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<RefUnit> allItems = new ObservableCollection<RefUnit>();
                            try
                            {
                                allItems = contract.EndGetUnits(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                Error = ex;
                            }
                            finally
                            {
                                _RefUnitList = new ObservableCollection<RefUnit>(allItems);
                                if(_addSelectOneItem)
                                {
                                    RefUnit firstItem = new RefUnit();
                                    firstItem.UnitID = -1;
                                    firstItem.UnitName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0111_G1_HayChon1GTri);
                                    RefUnitList.Insert(0, firstItem);
                                }
                                if (_addSelectedAllItem)
                                {
                                    RefUnit firstItem = new RefUnit();
                                    firstItem.UnitID= -2;
                                    firstItem.UnitName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                    RefUnitList.Insert(0, firstItem);
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


    public class LoadDrugDeptUnitListTask : IResult
    {
        public Exception Error { get; private set; }
        private ObservableCollection<RefUnit> _RefUnitList;
        public ObservableCollection<RefUnit> RefUnitList
        {
            get { return _RefUnitList; }
        }
        private bool _addSelectedAllItem;
        private bool _addSelectOneItem;
        public LoadDrugDeptUnitListTask(bool addSelectOneItem = true, bool addSelectedAllItem = false)
        {
            _addSelectOneItem = addSelectOneItem;
            _addSelectedAllItem = addSelectedAllItem;
        }

        public void Execute(ActionExecutionContext context)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyUnitsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetDrugDeptUnits(
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<RefUnit> allItems = new ObservableCollection<RefUnit>();
                                try
                                {
                                    allItems = contract.EndGetDrugDeptUnits(asyncResult);
                                }
                                catch (Exception ex)
                                {
                                    Error = ex;
                                }
                                finally
                                {
                                    _RefUnitList = new ObservableCollection<RefUnit>(allItems);
                                    if (_addSelectOneItem)
                                    {
                                        RefUnit firstItem = new RefUnit();
                                        firstItem.UnitID = -1;
                                        firstItem.UnitName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0111_G1_HayChon1GTri);
                                        RefUnitList.Insert(0, firstItem);
                                    }
                                    if (_addSelectedAllItem)
                                    {
                                        RefUnit firstItem = new RefUnit();
                                        firstItem.UnitID = -2;
                                        firstItem.UnitName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                        RefUnitList.Insert(0, firstItem);
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
// -------------- DPT 03/10/2017 hoạt động chỉ đạo tuyến  và hoạt động ngiên cứu khoa học

    //public class LoadRefGeneralUnitsTask : IResult
    //{
    //    public Exception Error { get; private set; }
    //    private ObservableCollection<RefGeneralUnits> _RefUnitList;
    //    public ObservableCollection<RefGeneralUnits> RefUnitList
    //    {
    //        get { return _RefUnitList; }
    //    }
      
    //    public LoadRefGeneralUnitsTask()
    //    {

    //    }

    //    public void Execute(ActionExecutionContext context)
    //    {
    //        var t = new Thread(() =>
    //        {
    //            try
    //            {
    //                using (var serviceFactory = new PharmacyUnitsServiceClient())
    //                {
    //                    var contract = serviceFactory.ServiceInstance;

    //                    contract.BeginLoadRefGeneralUnits(
    //                        Globals.DispatchCallback((asyncResult) =>
    //                        {
    //                            ObservableCollection<RefGeneralUnits> allItems = new ObservableCollection<RefGeneralUnits>();
    //                            try
    //                            {
    //                                allItems = new ObservableCollection<RefGeneralUnits>(contract.EndLoadRefGeneralUnits(asyncResult));
    //                            }
    //                            catch (Exception ex)
    //                            {
    //                                Error = ex;
    //                            }
    //                            finally
    //                            {
    //                                _RefUnitList = new ObservableCollection<RefGeneralUnits>(allItems);
                                   
    //                                    RefGeneralUnits firstItem = new RefGeneralUnits();
    //                                    firstItem.GeneralUnitID = 0;
    //                                    firstItem.GeneralUnitName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0111_G1_HayChon1GTri);
    //                                    RefUnitList.Insert(0, firstItem);
    //                                Completed(this, new ResultCompletionEventArgs
    //                                {
    //                                    Error = null,
    //                                    WasCancelled = false
    //                                });
    //                            }

    //                        }), null);
    //                }
    //            }
    //            catch (Exception ex)
    //            {
    //                Error = ex;
    //                Completed(this, new ResultCompletionEventArgs
    //                {
    //                    Error = null,
    //                    WasCancelled = false
    //                });
    //            }
    //        });
    //        t.Start();
    //    }

    //    public event EventHandler<ResultCompletionEventArgs> Completed;
    //}


    public class GetScientificResearchActivityListTask : IResult
    {
        public Exception Error { get; private set; }
        private ObservableCollection<ScientificResearchActivities> _Activity;
        public ObservableCollection<ScientificResearchActivities> Activity
        {
            get { return _Activity; }
        }
        ScientificResearchActivities _scientactivity;
       
        public GetScientificResearchActivityListTask(ScientificResearchActivities ScientActivity)
        {
            _scientactivity = ScientActivity;
        }

        public void Execute(ActionExecutionContext context)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyUnitsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetScientificResearchActivityList(_scientactivity, 
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                ObservableCollection<ScientificResearchActivities> allItems = new ObservableCollection<ScientificResearchActivities>();
                                try
                                {
                                    allItems = new ObservableCollection<ScientificResearchActivities>(contract.EndGetScientificResearchActivityList(asyncResult));
                                }
                                catch (Exception ex)
                                {
                                    Error = ex;
                                }
                                finally
                                {
                                    _Activity = new ObservableCollection<ScientificResearchActivities>(allItems);

                                  
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


    public class GetTrainningTypeListTask : IResult
    {
        public Exception Error { get; private set; }
        private ObservableCollection<Lookup> _Lookup;
        public ObservableCollection<Lookup> Lookup
        {
            get { return _Lookup; }
        }
        
        public GetTrainningTypeListTask()
        {
            
        }

        public void Execute(ActionExecutionContext context)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyUnitsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetTrainningTypeList(
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                ObservableCollection<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = new ObservableCollection<Lookup>(contract.EndGetTrainningTypeList(asyncResult));
                                }
                                catch (Exception ex)
                                {
                                    Error = ex;
                                }
                                finally
                                {
                                    _Lookup = new ObservableCollection<Lookup>(allItems);

                                    Lookup firstItem = new Lookup();
                                    firstItem.LookupID = 0;
                                    firstItem.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                    Lookup.Insert(0, firstItem);

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

    public class  GetTrainingForSubOrgListTask : IResult
    {
        public Exception Error { get; private set; }
        private ObservableCollection<TrainingForSubOrg> _Activity;
        public ObservableCollection<TrainingForSubOrg> Activity
        {
            get { return _Activity; }
            set
            {
                _Activity = value;
            }
        }
        TrainingForSubOrg _Training;

        public GetTrainingForSubOrgListTask(TrainingForSubOrg Training)
        {
            _Training = Training;
        }

        public void Execute(ActionExecutionContext context)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyUnitsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetTrainingForSubOrgList(_Training,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                ObservableCollection<TrainingForSubOrg> allItems = new ObservableCollection<TrainingForSubOrg>();
                                try
                                {
                                    allItems = new ObservableCollection<TrainingForSubOrg>(contract.EndGetTrainingForSubOrgList(asyncResult));
                                }
                                catch (Exception ex)
                                {
                                    Error = ex;
                                }
                                finally
                                {
                                    Activity = new ObservableCollection<TrainingForSubOrg>(allItems);


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
// -------------- DPT 03/10/2017 hoạt động chỉ đạo tuyến  và hoạt động ngiên cứu khoa học
}
