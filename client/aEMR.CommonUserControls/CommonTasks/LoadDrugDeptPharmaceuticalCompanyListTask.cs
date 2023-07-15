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
    public class LoadDrugDeptPharmaceuticalCompanyListTask:IResult
    {
        public Exception Error { get; private set; }
        private ObservableCollection<DrugDeptPharmaceuticalCompany> _DrugDeptPharmaceuticalCompanyList;
        public ObservableCollection<DrugDeptPharmaceuticalCompany> DrugDeptPharmaceuticalCompanyList
        {
            get { return _DrugDeptPharmaceuticalCompanyList; }
        }
        private bool _addSelectedAllItem;
        private bool _addSelectOneItem;
        public LoadDrugDeptPharmaceuticalCompanyListTask(bool addSelectOneItem = true, bool addSelectedAllItem = false)
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
                    using (var serviceFactory = new PharmacySuppliersServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetDrugDeptPharmaceuticalCompanyCbx(
                            Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<DrugDeptPharmaceuticalCompany> allItems = new ObservableCollection<DrugDeptPharmaceuticalCompany>();
                            try
                            {
                                allItems = contract.EndGetDrugDeptPharmaceuticalCompanyCbx(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                Error = ex;
                            }
                            finally
                            {
                                _DrugDeptPharmaceuticalCompanyList = new ObservableCollection<DrugDeptPharmaceuticalCompany>(allItems);
                                if(_addSelectOneItem)
                                {
                                    DrugDeptPharmaceuticalCompany firstItem = new DrugDeptPharmaceuticalCompany();
                                    firstItem.PCOID = -1;
                                    firstItem.PCOName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0111_G1_HayChon1GTri);
                                    DrugDeptPharmaceuticalCompanyList.Insert(0, firstItem);
                                }
                                if (_addSelectedAllItem)
                                {
                                    DrugDeptPharmaceuticalCompany firstItem = new DrugDeptPharmaceuticalCompany();
                                    firstItem.PCOID= -2;
                                    firstItem.PCOName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                    DrugDeptPharmaceuticalCompanyList.Insert(0, firstItem);
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
