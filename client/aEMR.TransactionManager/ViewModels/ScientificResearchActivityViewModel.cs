using System;
using System.Windows;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using aEMR.CommonTasks;
using System.Threading;
using aEMR.ServiceClient;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using eHCMSLanguage;

namespace aEMR.TransactionManager.ViewModels
{
    [Export(typeof(IScientificResearchActivity)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ScientificResearchActivityViewModel : Conductor<object>, IScientificResearchActivity
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ScientificResearchActivityViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }
        protected override void OnActivate()
        {
            base.OnActivate();

            Globals.EventAggregator.Subscribe(this);
            ScientificResearchActivity_Current = new ScientificResearchActivities();
            ScientificResearchActivity_Paging = new PagedSortableCollectionView<ScientificResearchActivities>();                      
        }
    
        private IEnumerator<IResult> GetScientificResearchActivityList()
        {
            ScientificResearchActivity_Paging.Clear();
            var paymentTypeTask = new GetScientificResearchActivityListTask(ScientificResearchActivity_Current);
            yield return paymentTypeTask;
            ObservableCollection<ScientificResearchActivities> Allitem = paymentTypeTask.Activity;
            if (Allitem != null)
            {
                foreach (var item in Allitem)
                {

                    ScientificResearchActivity_Paging.Add(item);

                }
            }
            else
            {
                MessageBox.Show("Không có đề tài thoả điều kiện tìm kiếm ");
 
            }
            yield break;

        }
      
        public void btnAdd()
        {
            //var vm = Globals.GetViewModel<IInsertUpdateScientificResearchActivity>();
            //vm.ISAdd = true;
            //vm.TitleForm = "Thêm hoạt động nghiên cứu khoa học";
            //Globals.ShowDialog(vm as Conductor<object>);

            Action<IInsertUpdateScientificResearchActivity> onInitDlg = (vm) =>
            {
                vm.ISAdd = true;
                vm.TitleForm = "Thêm hoạt động nghiên cứu khoa học";
            };
            GlobalsNAV.ShowDialog<IInsertUpdateScientificResearchActivity>(onInitDlg);
        }
        public void btnSearch()
        {           
            bool valid = ValidateGeneralInfo(ScientificResearchActivity_Current);
            if (valid)
            {
                Coroutine.BeginExecute(GetScientificResearchActivityList());
            }
                
        }
        public bool ValidateGeneralInfo(ScientificResearchActivities Activity)
        {
           
                
                if(Activity.StartDate > Activity.EndDate)
                {
                    MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc ");
                    return false;
                }
        
            return true;
        }
        private void DeleteScientificResearchActivity(long ActivityID)
        {
            var t = new Thread(() =>
            {

                using (var serviceFactory = new PharmacyUnitsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginDeleteScientificResearchActivity(ActivityID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int Result;

                            contract.EndDeleteScientificResearchActivity(out Result, asyncResult);
                            if (Result == 1)
                            {
                                Coroutine.BeginExecute(GetScientificResearchActivityList());
                                MessageBox.Show("Xoá hoạt động thành công");

                            }

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {

                        }
                    }), null);
                }
            });
            t.Start();
        }

        //private void InsertUpdateScientificResearchActivity(bool ISAdd, ScientificResearchActivities objActivity)
        //{
        //    var t = new Thread(() =>
        //    {

        //        using (var serviceFactory = new PharmacyUnitsServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;

        //            contract.BeginInsertUpdateScientificResearchActivity(ISAdd, objActivity, Globals.DispatchCallback((asyncResult) =>
        //            {
        //                try
        //                {
        //                    int Result;

        //                    contract.EndInsertUpdateScientificResearchActivity(out Result, asyncResult);
        //                    if (Result == 1)
        //                    {
        //                        MessageBox.Show("Thêm đề tài nghiên cứu thành công");
        //                    }
        //                    if (Result == 2)
        //                    {
        //                        MessageBox.Show("Cập nhật đề tài nghiên cứu thành công");
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                }
        //                finally
        //                {

        //                }
        //            }), null);
        //        }


        //    });
        //    t.Start();
        //}


        public void hplEdit_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                //var vm = Globals.GetViewModel<IInsertUpdateScientificResearchActivity>();
                //vm.TitleForm = "Cập nhật hoạt động nghiên cứu khoa học";
                //vm.ISAdd = false;
                //vm.ScientificResearchActivity_Current = selectedItem as ScientificResearchActivities;
                //Globals.ShowDialog(vm as Conductor<object>);

                Action<IInsertUpdateScientificResearchActivity> onInitDlg = (vm) =>
                {
                    vm.TitleForm = "Cập nhật hoạt động nghiên cứu khoa học";
                    vm.ISAdd = false;
                    vm.ScientificResearchActivity_Current = selectedItem as ScientificResearchActivities;
                };
                GlobalsNAV.ShowDialog<IInsertUpdateScientificResearchActivity>(onInitDlg);
            }
        }

        public void hplDelete_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                ScientificResearchActivities scientactivity = selectedItem as ScientificResearchActivities;
                long ID = (long)scientactivity.ActivityID;
                DeleteScientificResearchActivity(ID);
            }
        }


        

        private bool _ISAdd;
        public bool ISAdd
        {
            get { return _ISAdd; }
            set
            {
                _ISAdd = value;
                NotifyOfPropertyChange(() => ISAdd);
            }
        }
        private string _TitleForm;
        public string TitleForm
        {
            get { return _TitleForm; }
            set
            {
                _TitleForm = value;
                NotifyOfPropertyChange(() => TitleForm);
            }
        }

        private ScientificResearchActivities _ScientificResearchActivity_Current;
        public ScientificResearchActivities ScientificResearchActivity_Current
        {
            get { return _ScientificResearchActivity_Current; }
            set
            {
                _ScientificResearchActivity_Current = value;
                NotifyOfPropertyChange(() => ScientificResearchActivity_Current);
            }
        }
        private PagedSortableCollectionView<DataEntities.ScientificResearchActivities> _ScientificResearchActivity_Paging;
        public PagedSortableCollectionView<DataEntities.ScientificResearchActivities> ScientificResearchActivity_Paging
        {
            get { return _ScientificResearchActivity_Paging; }
            set
            {
                _ScientificResearchActivity_Paging = value;


                NotifyOfPropertyChange(() => ScientificResearchActivity_Paging);
            }
        }
      

        //private ObservableCollection<RefGeneralUnits> _V_ActivityType = new ObservableCollection<RefGeneralUnits>();
        //public ObservableCollection<RefGeneralUnits> V_ActivityType
        //{
        //    get { return _V_ActivityType; }
        //    set
        //    {
        //        _V_ActivityType = value;
        //        NotifyOfPropertyChange("V_ActivityType");
        //    }
        //}    
            
    }
}
