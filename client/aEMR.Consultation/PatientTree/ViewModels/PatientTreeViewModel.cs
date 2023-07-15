using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Utilities;
using Castle.Windsor;
using aEMR.Common.BaseModel;

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IPatientTree)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientTreeViewModel : ViewModelBase, IPatientTree, IHandle<ModuleTreeChangeEvent>
        , IHandle<ModuleTreeExChangeEvent>
        , IHandle<ModuleLoadCompleteEvent>
        , IHandle<ShowPatientInfo_KHAMBENH_LSBENHAN<Patient, PatientRegistration, PatientRegistrationDetail>>
    {
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        [ImportingConstructor]
        public PatientTreeViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            GetAllPatientSummaryEnum();
            ToDate = Globals.GetCurServerDateTime();
            FromDate = ToDate.AddDays(-365);
        }
        public void InitPatientInfo()
        {
            if (Registration_DataStorage.CurrentPatient != null)
            {
                GetPatientServicesTreeView(Registration_DataStorage.CurrentPatient.PatientID);
            }
        }
        protected override void OnActivate()
        {
            InitPatientInfo();
            Globals.EventAggregator.Subscribe(this);
            //base.OnActivate();
            //GetAllPatientSummaryEnum();
            //if (Registration_DataStorage.CurrentPatient != null)
            //{
            //    GetPatientServicesTreeView(Registration_DataStorage.CurrentPatient.PatientID);
            //}
            //Globals.EventAggregator.Subscribe(this);
        }
        public override void DeactivateItem(object item, bool close)
        {
            base.DeactivateItem(item, close);
            Globals.EventAggregator.Unsubscribe(this);
        }
        #region properties

        private ObservableCollection<patientSummaryEnum> _allpatientSummaryEnum;
        public ObservableCollection<patientSummaryEnum> allpatientSummaryEnum
        {
            get
            {
                return _allpatientSummaryEnum;
            }
            set
            {
                if (_allpatientSummaryEnum == value)
                    return;
                _allpatientSummaryEnum = value;
                NotifyOfPropertyChange(() => allpatientSummaryEnum);
            }
        }

        private patientSummaryEnum _curpatientSummaryEnum;
        public patientSummaryEnum curpatientSummaryEnum
        {
            get
            {
                return _curpatientSummaryEnum;
            }
            set
            {
                if (_curpatientSummaryEnum == value)
                    return;
                _curpatientSummaryEnum = value;
                NotifyOfPropertyChange(() => curpatientSummaryEnum);

                // TxD 11/09/2014 Commented out the following because it may not be needed
                //RefreshBtn();

                Globals.EventAggregator.Publish(new PatientSummaryChange() { curPatientSummary = curpatientSummaryEnum.enumPS });
            }
        }

        private ObservableCollection<PatientServicesTree> _lstPatientServicesTree;
        public ObservableCollection<PatientServicesTree> lstPatientServicesTree
        {
            get
            {
                return _lstPatientServicesTree;
            }
            set
            {
                if (_lstPatientServicesTree == value)
                    return;
                _lstPatientServicesTree = value;
                NotifyOfPropertyChange(() => lstPatientServicesTree);
            }
        }

        private DateTime _FromDate;
        public DateTime FromDate
        {
            get
            {
                return _FromDate;
            }
            set
            {
                if (_FromDate == value)
                    return;
                _FromDate = value;
                NotifyOfPropertyChange(() => FromDate);
            }
        }
        private DateTime _ToDate;
        public DateTime ToDate
        {
            get
            {
                return _ToDate;
            }
            set
            {
                if (_ToDate == value)
                    return;
                _ToDate = value;
                NotifyOfPropertyChange(() => ToDate);
            }
        }

        #endregion

        public void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e != null
                && e.NewValue != null)
            {
                if ((((PatientServicesTree)e.NewValue)).Level == 1)
                {
                    Globals.EventAggregator.Publish(new PatientTreeChange() { curPatientServicesTree = (((PatientServicesTree)e.NewValue)) });
                }
                else
                if ((((PatientServicesTree)e.NewValue)).Level == 2)
                {
                    Globals.EventAggregator.Publish(new PatientTreeChange() { curPatientServicesTree = (((PatientServicesTree)e.NewValue)) });
                }
                else
                    if ((((PatientServicesTree)e.NewValue)).Level == 3)
                {
                    Globals.EventAggregator.Publish(new PatientTreeChange() { curPatientServicesTree = (((PatientServicesTree)e.NewValue)) });
                }
            }

        }

        #region property

        private ObservableCollection<ModulesTree> _allModulesTree;
        public ObservableCollection<ModulesTree> allModulesTree
        {
            get
            {
                return _allModulesTree;
            }
            set
            {
                if (_allModulesTree == value)
                    return;
                _allModulesTree = value;
                NotifyOfPropertyChange(() => allModulesTree);
            }
        }

        #endregion

        public void RefreshBtn()
        {
            if (Registration_DataStorage.CurrentPatient != null && Registration_DataStorage.CurrentPatient.PatientID > 0)
            {
                if ((int)curpatientSummaryEnum.enumPS > 1)
                {
                    GetPatientServicesTreeViewEnum(Registration_DataStorage.CurrentPatient.PatientID, (int)curpatientSummaryEnum.enumPS);
                }
                else
                {
                    GetPatientServicesTreeView(Registration_DataStorage.CurrentPatient.PatientID);
                }
            }
        }


        public void Handle(ModuleTreeChangeEvent obj)
        {
            if (obj != null)
            {

            }
        }
        public void Handle(ModuleTreeExChangeEvent obj)
        {
            if (obj != null)
            {

            }
        }
        public void Handle(ModuleLoadCompleteEvent obj)
        {
            if (obj != null)
            {
                allModulesTree = obj.allModulesTree;
            }

        }
        public void Handle(ShowPatientInfo_KHAMBENH_LSBENHAN<Patient, PatientRegistration, PatientRegistrationDetail> message)
        {
            if (Registration_DataStorage.CurrentPatient != null)
            {
                //GlobalsNAV.PatientInfo.FullName
                curpatientSummaryEnum = allpatientSummaryEnum[0];
                RefreshBtn();
                // GetPatientServicesTreeView(Registration_DataStorage.CurrentPatient.PatientID);
                //phat su kien de refesh lai cac thong tin ben lich su benh an

                Globals.EventAggregator.Publish(new PatientInfoChange());
            }
        }
        #region method
        public void GetAllPatientSummaryEnum()
        {
            allpatientSummaryEnum = new ObservableCollection<patientSummaryEnum>();

            for (int i = 1; i < (int)AllLookupValues.PatientSummary.count; i++)
            {
                patientSummaryEnum pse = new patientSummaryEnum();
                pse.enumPS = (AllLookupValues.PatientSummary)i;
                allpatientSummaryEnum.Add(pse);
            }
            curpatientSummaryEnum = allpatientSummaryEnum[0];
        }



        private void GetPatientServicesTreeView(long patientID)
        {
            if (IsShowSummaryContent)
                this.ShowBusyIndicator();
            else
                this.DlgShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetPatientServicesTreeView(patientID, IsCriterion_PCLResult, FromDate, ToDate, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetPatientServicesTreeView(asyncResult);

                            // TxD 11/09/2014 Reset the binding collection here to clean up the Tree of previous loaded stuff
                            lstPatientServicesTree = new ObservableCollection<PatientServicesTree>();

                            if (results != null)
                            {
                                foreach (var p in results)
                                {
                                    lstPatientServicesTree.Add(p);
                                }
                                NotifyOfPropertyChange(() => lstPatientServicesTree);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            if (IsShowSummaryContent)
                                this.HideBusyIndicator();
                            else
                                this.DlgHideBusyIndicator();
                        }

                    }), null);
                }
            });
            t.Start();
        }

        private void GetPatientServicesTreeViewEnum(long patientID, int PatientSummaryEnum)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetPatientServicesTreeViewEnum(patientID, PatientSummaryEnum, FromDate, ToDate
                        , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetPatientServicesTreeViewEnum(asyncResult);
                                if (results != null)
                                {
                                    lstPatientServicesTree = new ObservableCollection<PatientServicesTree>();
                                    foreach (var p in results)
                                    {
                                        lstPatientServicesTree.Add(p);
                                    }
                                    NotifyOfPropertyChange(() => lstPatientServicesTree);
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                //Globals.IsBusy = false;
                                IsLoading = false;
                            }

                        }), null);

                }
                IsLoading = false;

            });

            t.Start();
        }
        #endregion
        private bool _IsShowSummaryContent = true;
        public bool IsShowSummaryContent
        {
            get => _IsShowSummaryContent; set
            {
                _IsShowSummaryContent = value;
                NotifyOfPropertyChange(() => IsShowSummaryContent);
            }
        }

        private IRegistration_DataStorage _Registration_DataStorage;
        public IRegistration_DataStorage Registration_DataStorage
        {
            get
            {
                return _Registration_DataStorage;
            }
            set
            {
                if (_Registration_DataStorage == value)
                {
                    return;
                }
                _Registration_DataStorage = value;
                NotifyOfPropertyChange(() => Registration_DataStorage);
            }
        }
        private bool _IsCriterion_PCLResult = false;
        public bool IsCriterion_PCLResult
        {
            get { return _IsCriterion_PCLResult; }
            set
            {
                if (_IsCriterion_PCLResult != value)
                {
                    _IsCriterion_PCLResult = value;
                    NotifyOfPropertyChange(() => IsCriterion_PCLResult);
                }
            }
        }
    }
    public class patientSummaryEnum
    {
        private AllLookupValues.PatientSummary _enumPS;
        public AllLookupValues.PatientSummary enumPS
        {
            get
            {
                return _enumPS;
            }
            set
            {
                if (_enumPS == value)
                    return;
                _enumPS = value;
                switch (value)
                {
                    case AllLookupValues.PatientSummary.ChonLoaiDV:
                        PSName = "--Chọn tất cả dịch vụ--"; break;
                    case AllLookupValues.PatientSummary.KhamBenh_ChanDoan:
                        PSName = Helpers.GetEnumDescription(AllLookupValues.PatientSummary.KhamBenh_ChanDoan);
                        break;
                    case AllLookupValues.PatientSummary.ToaThuoc:
                        PSName = Helpers.GetEnumDescription(AllLookupValues.PatientSummary.ToaThuoc);
                        break;
                    case AllLookupValues.PatientSummary.CanLamSang_HinhAnh:
                        PSName = Helpers.GetEnumDescription(AllLookupValues.PatientSummary.CanLamSang_HinhAnh);
                        break;
                    case AllLookupValues.PatientSummary.CanLamSang_XetNghiem:
                        PSName = Helpers.GetEnumDescription(AllLookupValues.PatientSummary.CanLamSang_XetNghiem);
                        break;
                    case AllLookupValues.PatientSummary.HoiChan:
                        PSName = Helpers.GetEnumDescription(AllLookupValues.PatientSummary.HoiChan);
                        break;
                    case AllLookupValues.PatientSummary.NoiTru:
                        PSName = Helpers.GetEnumDescription(AllLookupValues.PatientSummary.NoiTru);
                        break;
                    case AllLookupValues.PatientSummary.GiaiPhauKyThuatCao:
                        PSName = Helpers.GetEnumDescription(AllLookupValues.PatientSummary.GiaiPhauKyThuatCao);
                        break;
                    case AllLookupValues.PatientSummary.CanLamSang_HinhAnh_NgoaiVien:
                        PSName = Helpers.GetEnumDescription(AllLookupValues.PatientSummary.CanLamSang_HinhAnh_NgoaiVien);
                        break;
                    case AllLookupValues.PatientSummary.ThuThuat:
                        PSName = Helpers.GetEnumDescription(AllLookupValues.PatientSummary.ThuThuat);
                        break;
                }
            }
        }

        private string _PSName;
        public string PSName
        {
            get
            {
                return _PSName;
            }
            set
            {
                if (_PSName == value)
                    return;
                _PSName = value;
            }
        }
    }
}