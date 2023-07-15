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
    public class LoadPharmaceuticalCompanyListTask:IResult
    {
        public Exception Error { get; private set; }
        private ObservableCollection<PharmaceuticalCompany> _PharmaceuticalCompanyList;
        public ObservableCollection<PharmaceuticalCompany> PharmaceuticalCompanyList
        {
            get { return _PharmaceuticalCompanyList; }
        }
        private bool _addSelectedAllItem;
        private bool _addSelectOneItem;
        public LoadPharmaceuticalCompanyListTask(bool addSelectOneItem = true, bool addSelectedAllItem = false)
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
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetPharmaceuticalCompanyCbx(
                            Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<PharmaceuticalCompany> allItems = new ObservableCollection<PharmaceuticalCompany>();
                            try
                            {
                                allItems = contract.EndGetPharmaceuticalCompanyCbx(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                Error = ex;
                            }
                            finally
                            {
                                _PharmaceuticalCompanyList = new ObservableCollection<PharmaceuticalCompany>(allItems);
                                if(_addSelectOneItem)
                                {
                                    PharmaceuticalCompany firstItem = new PharmaceuticalCompany();
                                    firstItem.PCOID = -1;
                                    firstItem.PCOName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0111_G1_HayChon1GTri);
                                    PharmaceuticalCompanyList.Insert(0, firstItem);
                                }
                                if (_addSelectedAllItem)
                                {
                                    PharmaceuticalCompany firstItem = new PharmaceuticalCompany();
                                    firstItem.PCOID= -2;
                                    firstItem.PCOName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                    PharmaceuticalCompanyList.Insert(0, firstItem);
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
