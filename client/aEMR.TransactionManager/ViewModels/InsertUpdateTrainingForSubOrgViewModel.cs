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
using System.Linq;

namespace aEMR.TransactionManager.ViewModels
{
     [Export(typeof(IInsertUpdateTrainingForSubOrg)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InsertUpdateTrainingForSubOrgViewModel : Conductor<object>, IInsertUpdateTrainingForSubOrg
    {        
         private Visibility _IsExtend3 = Visibility.Collapsed;
         public Visibility IsExtend3
         {
             get
             {
                 return _IsExtend3;
             }
             set
             {
                 _IsExtend3 = value;
                 NotifyOfPropertyChange("IsExtend3");
             }
         }
         private Visibility _IsExtend = Visibility.Collapsed;
         public Visibility IsExtend
         {
             get
             {
                 return _IsExtend;
             }
             set
             {
                 _IsExtend = value;
                 NotifyOfPropertyChange("IsExtend");
             }
         }
         private Visibility _IsExtend1 = Visibility.Collapsed;
         public Visibility IsExtend1
         {
             get
             {
                 return _IsExtend1;
             }
             set
             {
                 _IsExtend1 = value;
                 NotifyOfPropertyChange("IsExtend1");
             }
         }
         private Visibility _IsExtend2 = Visibility.Visible;
         public Visibility IsExtend2
         {
             get
             {
                 return _IsExtend2;
             }
             set
             {
                 _IsExtend2 = value;
                 NotifyOfPropertyChange("IsExtend2");
             }
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
                 if (SelectedActivityType.LookupID == 75001)
                 {
                     IsExtend1 = Visibility.Visible;
                     IsExtend2 = Visibility.Collapsed;
                     TrainingForSubOrg_Current.TrainingName = null;
                 }
                 else
                 {
                     IsExtend1 = Visibility.Collapsed;
                     IsExtend2 = Visibility.Visible;
                     TrainingForSubOrg_Current.V_ActivityClassType = 0;
                     TrainingForSubOrg_Current.ActivityClassID = 0;
                 }
                 if (SelectedActivityType.LookupID == 75002)
                 {
                     IsExtend3 = Visibility.Visible;
                     
                 }

                 else
                 {
                     IsExtend3 = Visibility.Collapsed;
                     TrainingForSubOrg_Current.TrainingEndDate = null;
                    
                 }
                 if ((SelectedActivityType.LookupID == 75003) || (SelectedActivityType.LookupID == 75005) || (SelectedActivityType.LookupID == 75004))
                 {
                     IsExtend = Visibility.Visible;
                 }
                 else
                 {
                     IsExtend = Visibility.Collapsed;
                 }
                     
                 NotifyOfPropertyChange("SelectedActivityType");
             }
         }

         private Lookup _SelectedActivityClassType;
         public Lookup SelectedActivityClassType
         {
             get
             {
                 return _SelectedActivityClassType;
             }
             set
             {
                 _SelectedActivityClassType = value;
                 if ((SelectedActivityClassType.LookupID == 81001)||(SelectedActivityClassType.LookupID == 81002))
                 {
                     ActivityClassListAll(SelectedActivityClassType.LookupID);
                 }
                 

                 NotifyOfPropertyChange("SelectedActivityClassType");
             }
         }
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public InsertUpdateTrainingForSubOrgViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
         {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            TrainingForSubOrg_Current = new TrainingForSubOrg();
             Coroutine.BeginExecute(TrainningTypeList());
             ObservableCollection<Lookup> V_ActivityClassTypeList1 = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_ActivityClassType).ToObservableCollection();
             V_ActivityClassTypeList.Add(new Lookup() { LookupID = 0, ObjectValue = "Hãy chọn một khối" });
             foreach (var item in V_ActivityClassTypeList1)
             {
                 V_ActivityClassTypeList.Add(item);
             }
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
                     if (item.LookupID == 0)
                     {
                         item.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0111_G1_HayChon1GTri);
                     }
                     V_TrainingType.Add(item);
                 }
                 if (ISAdd)
                 {
                     SelectedActivityType = V_TrainingType.FirstOrDefault();
                 }
                 
             }
             yield break;

         }

         public void btnSave()
         {
             if (TrainingForSubOrg_Current != null)
             {
                 bool valid = ValidateGeneralInfo(TrainingForSubOrg_Current);
                 if (valid)
                 {
                     InsertUpdateTrainingForSubOrg(ISAdd, TrainingForSubOrg_Current);
                 }
             }
             else
             {
                 MessageBox.Show("Chưa nhâp thông tin nghiên cứu khoa học");
             }


         }



         private void ActivityClassListAll(long ClassID)
         {
             var t = new Thread(() =>
             {

                 using (var serviceFactory = new PharmacyUnitsServiceClient())
                 {
                     var contract = serviceFactory.ServiceInstance;

                     contract.BeginActivityClassListAll(ClassID, Globals.DispatchCallback((asyncResult) =>
                     {
                         try
                         {
                             ObservableCollection<ActivityClasses> ActivityClassList = new ObservableCollection<ActivityClasses>();
                             ActivityClassList = new ObservableCollection<ActivityClasses>(contract.EndActivityClassListAll(asyncResult));
                             V_ActivityClass.Clear();
                             V_ActivityClass.Add(new ActivityClasses() { ActivityClassID = 0, ActivityClassName = "Hãy chọn tên đào tạo" });
                             foreach (var item in ActivityClassList)
                             {
                                 item.ActivityClassName = item.ActivityClassName +" "+'('+ item.TotalMonth.ToString()+" Tháng"+')';
                                 V_ActivityClass.Add(item);
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



         private void InsertUpdateTrainingForSubOrg(bool ISAdd, TrainingForSubOrg objTraining)
        {
            var t = new Thread(() =>
            {

                using (var serviceFactory = new PharmacyUnitsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginInsertUpdateTrainingForSubOrg(ISAdd, objTraining, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int Result;

                            contract.EndInsertUpdateTrainingForSubOrg(out Result, asyncResult);
                            if (Result == 1)
                            {
                                MessageBox.Show("Thêm hoạt dộng chỉ đạo thành công");
                                TryClose();
                            }
                            if (Result == 2)
                            {
                                MessageBox.Show("Cập nhật hoạt động chỉ đạo thành công");
                                TryClose();
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

         public bool ValidateGeneralInfo(TrainingForSubOrg Training)
         {

             if (string.IsNullOrEmpty(Training.TrainingName))
             {
                 if ((Training.ActivityClassID < 1)|| (Training.ActivityClassID == null))
                 {
                     MessageBox.Show("Bạn chưa nhâp tên hoạt động");
                     return false;
                 }
                
             }

             if (Training.V_TrainingType == null || Training.V_TrainingType <1)
             {
                 MessageBox.Show("Bạn chưa chọn loại hoạt động");
                 return false;
             }
            
             if (Training.TrainingStartDate == null)
             {
                 MessageBox.Show("Vui lòng chọn ngày bắt đầu hoạt động ");
                 return false;
             }
             if (Training.TotalAttendees == null)
             {
                 MessageBox.Show("Vui lòng chọn số người tham dự hoạt động ");
                 return false;
             }
             if (Training.TrainingStartDate > Training.TrainingEndDate)
             {
                 MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc ");
                 return false;
             }

             return true;
         }

         public void btnClose()
         {
             TryClose();
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

         private ObservableCollection<Lookup> _V_ActivityClassTypeList = new ObservableCollection<Lookup>();
         public ObservableCollection<Lookup> V_ActivityClassTypeList
         {
             get { return _V_ActivityClassTypeList; }
             set
             {
                 _V_ActivityClassTypeList = value;
                 NotifyOfPropertyChange("V_ActivityClassTypeList");
                 NotifyOfPropertyChange(() => TrainingForSubOrg_Current);
             }
         }
         private ObservableCollection<ActivityClasses> _V_ActivityClass = new ObservableCollection<ActivityClasses>();
         public ObservableCollection<ActivityClasses> V_ActivityClass
         {
             get { return _V_ActivityClass; }
             set
             {
                 _V_ActivityClass = value;
                 NotifyOfPropertyChange("V_ActivityClass");
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
                 if (!ISAdd && TrainingForSubOrg_Current.V_ActivityClassType > 0)
                     SelectedActivityClassType = V_ActivityClassTypeList.Where(x => x.LookupID == TrainingForSubOrg_Current.V_ActivityClassType).FirstOrDefault();
                 NotifyOfPropertyChange("SelectedActivityClassType");
             }
         }
    }
}
