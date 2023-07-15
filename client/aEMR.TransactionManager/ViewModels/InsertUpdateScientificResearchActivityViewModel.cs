using System;
using System.Windows;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using System.Collections.ObjectModel;
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
    [Export(typeof(IInsertUpdateScientificResearchActivity)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InsertUpdateScientificResearchActivityViewModel : Conductor<object>, IInsertUpdateScientificResearchActivity
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public InsertUpdateScientificResearchActivityViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            
            ScientificResearchActivity_Current = new ScientificResearchActivities();
            ObservableCollection<Lookup> V_ActivityTypeList = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_ActivityType).ToObservableCollection();
            V_ActivityType.Add(new Lookup() { LookupID = 0, ObjectValue = "Hãy chọn một cấp" });
            foreach (var item in V_ActivityTypeList)
            {
                V_ActivityType.Add(item);
            }
            ObservableCollection<Lookup> V_ActivityMethodType = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_ActivityMethodType).ToObservableCollection();
            V_ActivityMethodTypeList.Add(new Lookup() { LookupID = 0, ObjectValue = "Hãy chọn một phương thức" });
            foreach (var item in V_ActivityMethodType)
            {
                V_ActivityMethodTypeList.Add(item);
            }
            
        }
        protected override void OnActivate()
        {
            base.OnActivate();

            Globals.EventAggregator.Subscribe(this);
            
            
        }
             
        public void btnSave()
        {
            if (ScientificResearchActivity_Current != null)
            {
                bool valid = ValidateGeneralInfo(ScientificResearchActivity_Current);
                if (valid)
                {
                    InsertUpdateScientificResearchActivity(ISAdd, ScientificResearchActivity_Current);
                }
            }
            else
            {
                MessageBox.Show("Chưa nhâp thông tin nghiên cứu khoa học");
            }


        }

        public void btnClose()
        {
            TryClose();
        }
        
        public bool ValidateGeneralInfo(ScientificResearchActivities Activity)
        {
           
                if (string.IsNullOrEmpty(Activity.ActivityName))
                {
                    MessageBox.Show("Bạn chưa nhâp tên đề tài");
                    return false;
                }
                
                //if (string.IsNullOrEmpty(Activity.AttendeeName))
                //{
                //    MessageBox.Show("Bạn chưa nhâp tên người thực hiện đề tài");
                //    return false;
                //}
                //if (Activity.V_ActivityType == null || Activity.V_ActivityType < 1)
                //{
                //    MessageBox.Show("Vui lòng chọn cấp đề tài ");
                //    return false;
                //}
                //if (Activity.StartDate == null)
                //{
                //    MessageBox.Show("Vui lòng chọn ngày bắt đầu đề tài ");
                //    return false;
                //}
                //if (Activity.EndDate == null)
                //{
                //    MessageBox.Show("Vui lòng chọn ngày kết thúc đề tài ");
                //    return false;
                //}
                if (Activity.StartDate > Activity.EndDate)
                {
                    MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc ");
                    return false;
                }
                if (Activity.ApprovedDate > Activity.AcceptedDate)
                {
                    MessageBox.Show("Ngày xét duyệt không được lớn hơn ngày nghiệm thu ");
                    return false;
                }
        
            return true;
        }
       

        private void InsertUpdateScientificResearchActivity(bool ISAdd, ScientificResearchActivities objActivity)
        {
            var t = new Thread(() =>
            {

                using (var serviceFactory = new PharmacyUnitsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginInsertUpdateScientificResearchActivity(ISAdd, objActivity, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int Result;

                            contract.EndInsertUpdateScientificResearchActivity(out Result, asyncResult);
                            if (Result == 1)
                            {
                                MessageBox.Show("Thêm đề tài nghiên cứu thành công");
                                TryClose();
                            }
                            if (Result == 2)
                            {
                                MessageBox.Show("Cập nhật đề tài nghiên cứu thành công");
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

        private ObservableCollection<Lookup> _V_ActivityType = new ObservableCollection<Lookup>();
        public ObservableCollection<Lookup> V_ActivityType
        {
            get { return _V_ActivityType; }
            set
            {
                _V_ActivityType = value;
                NotifyOfPropertyChange("V_ActivityType");
            }
        }

        private ObservableCollection<Lookup> _V_ActivityMethodTypeList = new ObservableCollection<Lookup>();
        public ObservableCollection<Lookup> V_ActivityMethodTypeList
        {
            get { return _V_ActivityMethodTypeList; }
            set
            {
                _V_ActivityMethodTypeList = value;
                NotifyOfPropertyChange("V_ActivityMethodTypeList");
            }
        }
        //private ObservableCollection<Lookup> _V_ActivityMethodTypeList;
        //public ObservableCollection<Lookup> V_ActivityMethodTypeList
        //{
        //    get { return _V_ActivityMethodTypeList; }
        //    set
        //    {
        //        if (_V_ActivityMethodTypeList != value)
        //        {
        //            _V_ActivityMethodTypeList = value;
        //            NotifyOfPropertyChange(() => V_ActivityMethodTypeList);
        //        }
        //    }
        //}
      
       
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
