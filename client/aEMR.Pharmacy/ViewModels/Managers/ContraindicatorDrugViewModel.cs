
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
using Castle.Windsor;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Common;
using eHCMSLanguage;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IContraindicatorDrug)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ContraindicatorDrugViewModel : Conductor<object>, IContraindicatorDrug,IHandle<PharmacyContraIndicatorAddEvent>
    {
        public string TitleForm { get; set; }

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ContraindicatorDrugViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            authorization();
            _allContraIndicatorDrugsRelToMedCond=new PagedSortableCollectionView<ContraIndicatorDrugsRelToMedCond>();
            GetRefMedCondType();
            allContraIndicatorDrugsRelToMedCond.OnRefresh+=new EventHandler<RefreshEventArgs>(allContraIndicatorDrugsRelToMedCond_OnRefresh);

        }
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
        
        //TTM 10072018
        //Thêm điều kiện kiểm tra trước khi thực hiện
        void allContraIndicatorDrugsRelToMedCond_OnRefresh(object sender, RefreshEventArgs e)
        {
            if (selectedRefMedicalConditionType != null)
            {
                GetContraIndicatorDrugsRelToMedCondPaging(selectedRefMedicalConditionType.MedContraTypeID
                        , allContraIndicatorDrugsRelToMedCond.PageSize
                        , allContraIndicatorDrugsRelToMedCond.PageIndex, "", true);
            }
        }

        public  void AddContraIndicator()
        {
            if (selectedRefMedicalConditionType != null)
            {
                //var AddDrugContrainVM = Globals.GetViewModel<IAddDrugContrain>();
                //this.ActivateItem(AddDrugContrainVM);
                //Globals.ShowDialog(AddDrugContrainVM as Conductor<object>);

                Action<IAddDrugContrain> onInitDlg = (AddDrugContrainVM) =>
                {
                    this.ActivateItem(AddDrugContrainVM);
                };
                GlobalsNAV.ShowDialog<IAddDrugContrain>(onInitDlg);
            }
            else
            {
                Globals.ShowMessage(eHCMSResources.A0382_G1_Msg_InfoChuaChonDKienBenh, "");
                return;
            }
        }
        public void NewDrugContra()
        {
            //var MedicalControlVM = Globals.GetViewModel<IMedicalControl>();
            //MedicalControlVM.allRefMedicalConditionType = allRefMedicalConditionType;
            //this.ActivateItem(MedicalControlVM);
            //Globals.ShowDialog(MedicalControlVM as Conductor<object>);

            Action<IMedicalControl> onInitDlg = (MedicalControlVM) =>
            {
                MedicalControlVM.allRefMedicalConditionType = allRefMedicalConditionType;
                this.ActivateItem(MedicalControlVM);
            };
            GlobalsNAV.ShowDialog<IMedicalControl>(onInitDlg);

        }
        public void lnkDelete_Click(object sender,RoutedEventArgs e)
        {
            if (selectedContraIndicatorDrugsRelToMedCond!=null)
            {
                DeleteConIndicatorDrugsRelToMedCond(selectedContraIndicatorDrugsRelToMedCond.DrugsMCTypeID);
            }
            
        }
        public  void Handle(PharmacyContraIndicatorAddEvent obj)
        {
            if (obj.PharmacyContraIndicatorAdd != null) 
            {
                InsertConIndicatorDrugsRelToMedCondEx(
                                                (ObservableCollection<RefGenericDrugDetail>)obj.PharmacyContraIndicatorAdd
                                                , selectedRefMedicalConditionType.MedContraTypeID);
            }
        }
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bTim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyChongChiDinh,
                                               (int)oPharmacyEx.mQuanLyChongChiDinh_Tim, (int)ePermission.mView);
            bThem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyChongChiDinh,
                                               (int)oPharmacyEx.mQuanLyChongChiDinh_Them, (int)ePermission.mView);
            bChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyChongChiDinh,
                                               (int)oPharmacyEx.mQuanLyChongChiDinh_ChinhSua, (int)ePermission.mView);
            
        }
        #region checking account

        private bool _bTim = true;
        private bool _bThem = true;
        private bool _bChinhSua = true;

        public bool bTim
        {
            get
            {
                return _bTim;
            }
            set
            {
                if (_bTim == value)
                    return;
                _bTim = value;
            }
        }
        public bool bThem
        {
            get
            {
                return _bThem;
            }
            set
            {
                if (_bThem == value)
                    return;
                _bThem = value;
            }
        }
        public bool bChinhSua
        {
            get
            {
                return _bChinhSua;
            }
            set
            {
                if (_bChinhSua == value)
                    return;
                _bChinhSua = value;
            }
        }

        #endregion
        #region binding visibilty

        public Button lnkDelete { get; set; }

        public void lnkDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;
            lnkDelete.Visibility = Globals.convertVisibility(bChinhSua);
        }

        #endregion
#region properties

        private PagedSortableCollectionView<ContraIndicatorDrugsRelToMedCond> _allContraIndicatorDrugsRelToMedCond;
        public PagedSortableCollectionView<ContraIndicatorDrugsRelToMedCond> allContraIndicatorDrugsRelToMedCond
        {
            get
            {
                return _allContraIndicatorDrugsRelToMedCond;
            }
            set
            {
                if (_allContraIndicatorDrugsRelToMedCond == value)
                    return;
                _allContraIndicatorDrugsRelToMedCond = value;
                NotifyOfPropertyChange(()=>allContraIndicatorDrugsRelToMedCond);
            }
        }

        private ObservableCollection<RefMedContraIndicationTypes> _allRefMedicalConditionType;
        public ObservableCollection<RefMedContraIndicationTypes> allRefMedicalConditionType
        {
            get
            {
                return _allRefMedicalConditionType;
            }
            set
            {
                if (_allRefMedicalConditionType == value)
                    return;
                _allRefMedicalConditionType = value;
                NotifyOfPropertyChange(()=>allRefMedicalConditionType);
            }
        }

        private RefMedContraIndicationTypes _selectedRefMedicalConditionType;
        public RefMedContraIndicationTypes selectedRefMedicalConditionType
        {
            get
            {
                return _selectedRefMedicalConditionType;
            }
            set
            {
                if (_selectedRefMedicalConditionType == value)
                    return;
                _selectedRefMedicalConditionType = value;
                NotifyOfPropertyChange(() => selectedRefMedicalConditionType);
                allContraIndicatorDrugsRelToMedCond.PageIndex = 0;
                GetContraIndicatorDrugsRelToMedCondPaging(selectedRefMedicalConditionType.MedContraTypeID
                    , allContraIndicatorDrugsRelToMedCond.PageSize 
                    , allContraIndicatorDrugsRelToMedCond.PageIndex , "", true);
            }
        }

        private ContraIndicatorDrugsRelToMedCond _selectedContraIndicatorDrugsRelToMedCond ;
        public ContraIndicatorDrugsRelToMedCond selectedContraIndicatorDrugsRelToMedCond
        {
            get
            {
                return _selectedContraIndicatorDrugsRelToMedCond;
            }
            set
            {
                if (_selectedContraIndicatorDrugsRelToMedCond == value)
                    return;
                _selectedContraIndicatorDrugsRelToMedCond = value;
                NotifyOfPropertyChange(() => selectedContraIndicatorDrugsRelToMedCond);
            }
        }

#endregion
        
#region method
        private void GetRefMedCondType()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetRefMedCondType(Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetRefMedCondType(asyncResult);
                            if (results != null)
                            {
                                if (allRefMedicalConditionType == null)
                                {
                                    allRefMedicalConditionType = new ObservableCollection<RefMedContraIndicationTypes>();
                                }
                                else
                                {
                                    allRefMedicalConditionType.Clear();
                                }
                                foreach (var p in results)
                                {
                                    allRefMedicalConditionType.Add(p);
                                }
                                NotifyOfPropertyChange(() => allRefMedicalConditionType);
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

        private void GetContraIndicatorDrugsRelToMedCondPaging(long MCTypeID, int PageSize, int PageIndex, string OrderBy,bool CountTotal )
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetContraIndicatorDrugsRelToMedCondPaging(MCTypeID, PageSize, PageIndex, OrderBy,CountTotal ,Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int total = 0;
                            var results = contract.EndGetContraIndicatorDrugsRelToMedCondPaging(out total,asyncResult);
                            if (results != null)
                            {
                                if (allContraIndicatorDrugsRelToMedCond== null)
                                {
                                    allContraIndicatorDrugsRelToMedCond = new PagedSortableCollectionView<ContraIndicatorDrugsRelToMedCond>();
                                }
                                else
                                {
                                    allContraIndicatorDrugsRelToMedCond.Clear();
                                }
                                allContraIndicatorDrugsRelToMedCond.TotalItemCount = total;
                                foreach (var p in results)
                                {
                                    allContraIndicatorDrugsRelToMedCond.Add(p);
                                }
                                NotifyOfPropertyChange(() => allContraIndicatorDrugsRelToMedCond);
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
        private void DeleteConIndicatorDrugsRelToMedCond(long MCTypeID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDeleteConIndicatorDrugsRelToMedCond(MCTypeID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndDeleteConIndicatorDrugsRelToMedCond(asyncResult);
                            if (results == true)
                            {
                                GetContraIndicatorDrugsRelToMedCondPaging(selectedRefMedicalConditionType.MedContraTypeID
                                    , allContraIndicatorDrugsRelToMedCond.PageSize
                                    , allContraIndicatorDrugsRelToMedCond.PageIndex, "", true);
                                Globals.ShowMessage(eHCMSResources.A0478_G1_Msg_InfoXoaOK,"");
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

        private void InsertConIndicatorDrugsRelToMedCondEx(IList<RefGenericDrugDetail> lstRefGenericDrugDetail, long MCTypeID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginInsertConIndicatorDrugsRelToMedCondEx(lstRefGenericDrugDetail, MCTypeID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndInsertConIndicatorDrugsRelToMedCondEx(asyncResult);
                            if (results == true)
                            {
                                GetContraIndicatorDrugsRelToMedCondPaging(selectedRefMedicalConditionType.MedContraTypeID
                                    , allContraIndicatorDrugsRelToMedCond.PageSize
                                    , allContraIndicatorDrugsRelToMedCond.PageIndex, "", true);
                                Globals.ShowMessage(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK,"");
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
#endregion
    }
}
