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
    public class LoadStaffListTask:IResult
    {
        public Exception Error { get; private set; }
        private ObservableCollection<Staff> _StaffList;
        public ObservableCollection<Staff> StaffList
        {
            get { return _StaffList; }
        }
        private bool _addSelectedAllItem;
        private bool _addSelectOneItem;
        long _StaffCategory;
        public LoadStaffListTask(bool addSelectOneItem = true, bool addSelectedAllItem = false, long StaffCategory=(long)AllLookupValues.StaffCatType.BAC_SI)
        {
            _addSelectOneItem = addSelectOneItem;
            _addSelectedAllItem = addSelectedAllItem;
            _StaffCategory = StaffCategory;
        }

        public void Execute(ActionExecutionContext context)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ResourcesManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllStaffByStaffCategory(_StaffCategory,
                            Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<Staff> allItems = new ObservableCollection<Staff>();
                            try
                            {
                                allItems = contract.EndGetAllStaffByStaffCategory(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                Error = ex;
                            }
                            finally
                            {
                                _StaffList = new ObservableCollection<Staff>(allItems);
                                if(_addSelectOneItem)
                                {
                                    Staff firstItem = new Staff();
                                    firstItem.StaffID = -1;
                                    firstItem.FullName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0111_G1_HayChon1GTri);
                                    StaffList.Insert(0, firstItem);
                                }
                                if (_addSelectedAllItem)
                                {
                                    Staff firstItem = new Staff();
                                    firstItem.StaffID= -2;
                                    firstItem.FullName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                    StaffList.Insert(0, firstItem);
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
