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
    public class LoadRefGenDrugBHYT_CategoryListTask:IResult
    {
        public Exception Error { get; private set; }
        private ObservableCollection<RefGenDrugBHYT_Category> _RefGenDrugBHYT_CategoryList;
        public ObservableCollection<RefGenDrugBHYT_Category> RefGenDrugBHYT_CategoryList
        {
            get { return _RefGenDrugBHYT_CategoryList; }
        }
        private bool _addSelectedAllItem;
        private bool _addSelectOneItem;
        bool? _IsClassification;
        bool? _IsCombined;
        public LoadRefGenDrugBHYT_CategoryListTask(bool? isClassification, bool? isCombined, bool addSelectOneItem = true, bool addSelectedAllItem = false)
        {
            _IsClassification = isClassification;
            _IsCombined = isCombined;
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
                    using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginRefGenDrugBHYT_Category_Load(_IsClassification, _IsCombined,
                            Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<RefGenDrugBHYT_Category> allItems = new ObservableCollection<RefGenDrugBHYT_Category>();
                            try
                            {
                                allItems = contract.EndRefGenDrugBHYT_Category_Load(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                Error = ex;
                            }
                            finally
                            {
                                _RefGenDrugBHYT_CategoryList = new ObservableCollection<RefGenDrugBHYT_Category>(allItems);
                                if(_addSelectOneItem)
                                {
                                    RefGenDrugBHYT_Category firstItem = new RefGenDrugBHYT_Category();
                                    firstItem.RefGenDrugBHYT_CatID = -1;
                                    firstItem.CategoryName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0111_G1_HayChon1GTri);
                                    RefGenDrugBHYT_CategoryList.Insert(0, firstItem);
                                }
                                if (_addSelectedAllItem)
                                {
                                    RefGenDrugBHYT_Category firstItem = new RefGenDrugBHYT_Category();
                                    firstItem.RefGenDrugBHYT_CatID = -2;
                                    firstItem.CategoryName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T2468_G1_KhongChon);
                                    RefGenDrugBHYT_CategoryList.Insert(0, firstItem);
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
