using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using System;
using aEMR.Common.Collections;
using eHCMSLanguage;

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IDrugDeptMedCondition)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class MedConditionViewModel : Conductor<object>, IDrugDeptMedCondition
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        public string OrderBy = "";
        public bool CountTotal = true;

        [ImportingConstructor]
        public MedConditionViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            _allNewRefMedicalCondition =new ObservableCollection<RefMedContraIndicationTypes>();
            _allRefMedicalCondition=new PagedSortableCollectionView<RefMedContraIndicationTypes>();
            _allContrainName=new ObservableCollection<string>();
            GetRefMedicalConditionTypesAllPaging(1000, 0
                                                 , OrderBy, CountTotal);
            
        }

        void allRefMedicalCondition_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetRefMedicalConditionTypesAllPaging(1000, 0
                                                 , OrderBy, CountTotal);
        }

        public object DrugList { get; set; }

        private bool _IsLoading = false;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set
            {
                if (_IsLoading != value)
                {
                    _IsLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }
        public void butSave()
        {
            Globals.EventAggregator.Publish(new MedDeptContraEvent() { refMedicalConditionType_Edit = RefMedicalConditionType_Edit });
            TryClose();
             
        }
        public void butExit()
        {
            TryClose();
        }
        public void btAddChoose()
        {
            if (selectedRefMedicalCondition != null)
            {
                if (!RefMedicalConditionType_Edit.Add(selectedRefMedicalCondition))
                {
                    MessageBox.Show(eHCMSResources.A0518_G1_Msg_DKienBenhDaCoTrongDSCCD);
                }
            }

        }
        
        public void lnkDelete_Click(object sender,RoutedEventArgs e)
        {
            RefMedicalConditionType_Edit.Remove(selectedNewRefMedicalCondition);
        }
        public void DoubleClick(object sender,Common.EventArgs<object> e)
        {
            if (selectedRefMedicalCondition != null)
            {
                if (!RefMedicalConditionType_Edit.Add(selectedRefMedicalCondition))
                {
                    MessageBox.Show(eHCMSResources.A0518_G1_Msg_DKienBenhDaCoTrongDSCCD);
                }
            }
        }
#region properties

        private RefGenMedProductDetails _NewDrug;
        public RefGenMedProductDetails NewDrug
        {
            get
            {
                return _NewDrug;
            }
            set
            {
                if (_NewDrug == value)
                    return;
                _NewDrug = value;
                NotifyOfPropertyChange(() => NewDrug);
            }
        }

        private EntitiesEdit<RefMedContraIndicationTypes> _RefMedicalConditionType_Edit;
        public EntitiesEdit<RefMedContraIndicationTypes> RefMedicalConditionType_Edit
        {
            get
            {
                return _RefMedicalConditionType_Edit;
            }
            set
            {
                if (_RefMedicalConditionType_Edit == value)
                    return;
                _RefMedicalConditionType_Edit = value;
                NotifyOfPropertyChange(() => RefMedicalConditionType_Edit);
            }
        }

        private ObservableCollection<string> _allContrainName;
        public ObservableCollection<string> allContrainName
        {
            get
            {
                return _allContrainName;
            }
            set
            {
                if (_allContrainName == value)
                    return;
                _allContrainName = value;
                NotifyOfPropertyChange(()=>allContrainName);
            }
        }

        private ObservableCollection<RefMedContraIndicationTypes> _allRefMedicalCondition;
        public ObservableCollection<RefMedContraIndicationTypes> allRefMedicalCondition
        {
            get
            {
                return _allRefMedicalCondition;
            }
            set
            {
                if (_allRefMedicalCondition == value)
                    return;
                _allRefMedicalCondition = value;
                NotifyOfPropertyChange(()=>allRefMedicalCondition);
            }
        }

        private RefMedContraIndicationTypes _selectedRefMedicalCondition;
        public RefMedContraIndicationTypes selectedRefMedicalCondition
        {
            get
            {
                return _selectedRefMedicalCondition;
            }
            set
            {
                if (_selectedRefMedicalCondition == value)
                    return;
                _selectedRefMedicalCondition = value;
                NotifyOfPropertyChange(() => selectedRefMedicalCondition);
            }
        }

        private RefMedContraIndicationTypes _selectedNewRefMedicalCondition;
        public RefMedContraIndicationTypes selectedNewRefMedicalCondition
        {
            get
            {
                return _selectedNewRefMedicalCondition;
            }
            set
            {
                if (_selectedNewRefMedicalCondition == value)
                    return;
                _selectedNewRefMedicalCondition = value;
                NotifyOfPropertyChange(() => selectedNewRefMedicalCondition);
            }
        }

        private ObservableCollection<RefMedContraIndicationTypes> _allNewRefMedicalCondition;
        public ObservableCollection<RefMedContraIndicationTypes> allNewRefMedicalCondition
        {
            get
            {
                return _allNewRefMedicalCondition;
            }
            set
            {
                if (_allNewRefMedicalCondition == value)
                    return;
                _allNewRefMedicalCondition = value;
                NotifyOfPropertyChange(() => allNewRefMedicalCondition);
            }
        }

        private ObservableCollection<RefMedContraIndicationTypes> _allDeleteRefMedicalCondition;
        public ObservableCollection<RefMedContraIndicationTypes> allDeleteRefMedicalCondition
        {
            get
            {
                return _allDeleteRefMedicalCondition;
            }
            set
            {
                if (_allDeleteRefMedicalCondition == value)
                    return;
                _allDeleteRefMedicalCondition = value;
                NotifyOfPropertyChange(() => allDeleteRefMedicalCondition);
            }
        }
        #endregion


#region method

        private void GetRefMedicalConditionTypesAllPaging(int PageSize, int PageIndex, string OrderBy, bool CountTotal)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetRefMedicalConditionTypesAllPaging(PageSize, PageIndex, OrderBy, CountTotal,Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int Total = 0;
                            var results = contract.EndGetRefMedicalConditionTypesAllPaging(out Total,asyncResult);
                            if (results != null)
                            {
                                if (allRefMedicalCondition == null)
                                {
                                    allRefMedicalCondition = new PagedSortableCollectionView<RefMedContraIndicationTypes>();
                                }
                                else
                                {
                                    allRefMedicalCondition.Clear();
                                }
                                foreach (var p in results)
                                {
                                    allRefMedicalCondition.Add(p);
                                }
                                
                                NotifyOfPropertyChange(() => allRefMedicalCondition);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }
        
        private void DeleteRefMedicalConditions(int MCID, int MCTypeID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDeleteRefMedicalConditions(MCID, MCTypeID, Globals.LoggedUserAccount.StaffID.Value, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndDeleteRefMedicalConditions(asyncResult);
                            //if (results == true)
                            //{
                            //    GetRefMedicalConditions(Convert.ToInt32(selectedRefMedicalConditionType.MCTypeID));
                            //    Globals.ShowMessage(string.Format("{0}!", eHCMSResources.K0537_G1_XoaOk), "");
                            //}
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }
        
        #endregion
    }
}
