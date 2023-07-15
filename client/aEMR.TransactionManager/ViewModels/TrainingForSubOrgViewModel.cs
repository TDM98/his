using System;
using System.Windows;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using DataEntities;
using System.Collections.Generic;
using aEMR.CommonTasks;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Threading;
using aEMR.ServiceClient;
using eHCMSLanguage;
using System.Linq;

namespace aEMR.TransactionManager.ViewModels
{
    [Export(typeof(ITrainingForSubOrg)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class TrainingForSubOrgViewModel : Conductor<object>, ITrainingForSubOrg
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public TrainingForSubOrgViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            Coroutine.BeginExecute(TrainningTypeList());
            TrainingForSubOrg_Paging = new PagedSortableCollectionView<TrainingForSubOrg>();
            TrainingForSubOrg_Current = new TrainingForSubOrg();
        }
        private Lookup _SelectedActivityType;
        public Lookup SelectedActivityType
        {
            get
            {
                return _SelectedActivityType;
            }
            set
            {
                _SelectedActivityType = value;
                 
                NotifyOfPropertyChange("SelectedActivityType");
            }
        }
        public void btnSearch()
        {
            bool valid = ValidateGeneralInfo(TrainingForSubOrg_Current);
            if (valid)
            {
                TrainingForSubOrg_Paging.Clear();
                Coroutine.BeginExecute(GetTrainingForSubOrgList());
            }

        }
        public bool ValidateGeneralInfo(TrainingForSubOrg Training)
        {
            
            if (Training.TrainingStartDate > Training.TrainingEndDate)
            {
                MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc ");
                return false;
            }

            return true;
        }
        public void btnAdd()
        {
            //var vm = Globals.GetViewModel<IInsertUpdateTrainingForSubOrg>();
            //vm.ISAdd = true;
            //vm.TitleForm = "Thêm hoạt động chỉ đạo tuyến";
            //Globals.ShowDialog(vm as Conductor<object>);

            Action<IInsertUpdateTrainingForSubOrg> onInitDlg = (vm) =>
            {
                vm.ISAdd = true;
                vm.TitleForm = "Thêm hoạt động chỉ đạo tuyến";
            };
            GlobalsNAV.ShowDialog<IInsertUpdateTrainingForSubOrg>(onInitDlg);

        }

        public void hplDelete_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                TrainingForSubOrg TrainingFor = selectedItem as TrainingForSubOrg;
                long ID = (long)TrainingFor.TrainingID;
                DeleteTrainingForSubOrg(ID);
            }
        }
        public void hplEdit_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                //var vm = Globals.GetViewModel<IInsertUpdateTrainingForSubOrg>();
                //vm.TitleForm = "Cập nhật hoạt động chỉ đạo khoa học";
                //vm.ISAdd = false;
                //vm.TrainingForSubOrg_Current = (selectedItem as TrainingForSubOrg).EntityDeepCopy();
                //Globals.ShowDialog(vm as Conductor<object>);

                Action<IInsertUpdateTrainingForSubOrg> onInitDlg = (vm) =>
                {
                    vm.TitleForm = "Cập nhật hoạt động chỉ đạo khoa học";
                    vm.ISAdd = false;
                    vm.TrainingForSubOrg_Current = (selectedItem as TrainingForSubOrg).EntityDeepCopy();
                };
                GlobalsNAV.ShowDialog<IInsertUpdateTrainingForSubOrg>(onInitDlg);
            }
        }

        private void DeleteTrainingForSubOrg(long TrainingID)
        {
            var t = new Thread(() =>
            {

                using (var serviceFactory = new PharmacyUnitsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginDeleteTrainingForSubOrg(TrainingID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int Result;

                            contract.EndDeleteTrainingForSubOrg(out Result, asyncResult);
                            if (Result == 1)
                            {
                                Coroutine.BeginExecute(GetTrainingForSubOrgList());
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

        private IEnumerator<IResult> GetTrainingForSubOrgList()
        {
            TrainingForSubOrg_Paging.Clear();
            var paymentTypeTask = new GetTrainingForSubOrgListTask(TrainingForSubOrg_Current);
            yield return paymentTypeTask;
            ObservableCollection<TrainingForSubOrg> Allitem = paymentTypeTask.Activity;
            if (Allitem != null)
            {
                foreach (var item in Allitem)
                {

                   TrainingForSubOrg_Paging.Add(item);

                }
            }
            else
            {
                MessageBox.Show("Không có hoạt động thoả điều kiện tìm kiếm ");

            }
            yield break;

        }

        private IEnumerator<IResult> TrainningTypeList()
        {
            var paymentTypeTask = new GetTrainningTypeListTask();
            yield return paymentTypeTask;
            ObservableCollection<Lookup> Allitem = paymentTypeTask.Lookup;
            if (Allitem != null)
            {
                foreach (var item in Allitem)
                {

                    V_TrainingType.Add(item);

                }
                SelectedActivityType = V_TrainingType.FirstOrDefault();
            }           
            yield break;

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
        private TrainingForSubOrg _TrainingForSubOrg_Current;
        public TrainingForSubOrg TrainingForSubOrg_Current
        {
            get { return _TrainingForSubOrg_Current; }
            set
            {
                _TrainingForSubOrg_Current = value;
                NotifyOfPropertyChange(() => TrainingForSubOrg_Current);
            }
        }
        private ObservableCollection<Lookup> _V_TrainingType = new ObservableCollection<Lookup>();
        public ObservableCollection<Lookup> V_TrainingType
        {
            get { return _V_TrainingType; }
            set
            {
                _V_TrainingType = value;
                NotifyOfPropertyChange("V_TrainingType");
            }
        }

        private PagedSortableCollectionView<DataEntities.TrainingForSubOrg> _TrainingForSubOrg_Paging;
        public PagedSortableCollectionView<DataEntities.TrainingForSubOrg> TrainingForSubOrg_Paging
        {
            get { return _TrainingForSubOrg_Paging; }
            set
            {
                _TrainingForSubOrg_Paging = value;


                NotifyOfPropertyChange(() => TrainingForSubOrg_Paging);
            }
        }

    }
}
