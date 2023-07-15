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
    public class LoadDrugClassListTask:IResult
    {
        public Exception Error { get; private set; }
        private ObservableCollection<DrugClass> _DrugClassList;
        public ObservableCollection<DrugClass> DrugClassList
        {
            get { return _DrugClassList; }
        }
        private bool _addSelectedAllItem;
        private bool _addSelectOneItem;
        long _V_MedProductType;
        public LoadDrugClassListTask(long V_MedProductType, bool addSelectOneItem = true, bool addSelectedAllItem = false)
        {
            _V_MedProductType = V_MedProductType;
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
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetFamilyTherapies(_V_MedProductType,
                            Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<DrugClass> allItems = new ObservableCollection<DrugClass>();
                            try
                            {
                                allItems = contract.EndGetFamilyTherapies(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                Error = ex;
                            }
                            finally
                            {
                                _DrugClassList = new ObservableCollection<DrugClass>(allItems);
                                if(_addSelectOneItem)
                                {
                                    DrugClass firstItem = new DrugClass();
                                    firstItem.DrugClassID = -1;
                                    firstItem.FaName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0111_G1_HayChon1GTri);
                                    DrugClassList.Insert(0, firstItem);
                                }
                                if (_addSelectedAllItem)
                                {
                                    DrugClass firstItem = new DrugClass();
                                    firstItem.DrugClassID= -2;
                                    firstItem.FaName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                    DrugClassList.Insert(0, firstItem);
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

    public class LoadDrugDeptClassListTask : IResult
    {
        public Exception Error { get; private set; }
        private ObservableCollection<DrugClass> _DrugClassList;
        public ObservableCollection<DrugClass> DrugClassList
        {
            get { return _DrugClassList; }
        }
        private bool _addSelectedAllItem;
        private bool _addSelectOneItem;
        long _V_MedProductType;
        public LoadDrugDeptClassListTask(long V_MedProductType, bool addSelectOneItem = true, bool addSelectedAllItem = false)
        {
            _V_MedProductType = V_MedProductType;
            _addSelectOneItem = addSelectOneItem;
            _addSelectedAllItem = addSelectedAllItem;
        }

        public void Execute(ActionExecutionContext context)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetDrugDeptClasses(_V_MedProductType,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<DrugClass> allItems = new ObservableCollection<DrugClass>();
                                try
                                {
                                    allItems = contract.EndGetDrugDeptClasses(asyncResult);
                                }
                                catch (Exception ex)
                                {
                                    Error = ex;
                                }
                                finally
                                {
                                    _DrugClassList = new ObservableCollection<DrugClass>(allItems);
                                    if (_addSelectOneItem)
                                    {
                                        DrugClass firstItem = new DrugClass();
                                        firstItem.DrugClassID = -1;
                                        firstItem.FaName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0111_G1_HayChon1GTri);
                                        DrugClassList.Insert(0, firstItem);
                                    }
                                    if (_addSelectedAllItem)
                                    {
                                        DrugClass firstItem = new DrugClass();
                                        firstItem.DrugClassID = -2;
                                        firstItem.FaName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                        DrugClassList.Insert(0, firstItem);
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
