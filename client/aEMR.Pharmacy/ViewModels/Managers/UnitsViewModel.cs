using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.Infrastructure.ServiceCore;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure.Events;
using DataEntities;
using UnitProxy;
using aEMR.Common.Collections;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using aEMR.Common;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using eHCMSLanguage;
using aEMR.Common.BaseModel;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IUnits)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class UnitsViewModel : ViewModelBase, IUnits
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public UnitsViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            authorization();
            Units = new PagedSortableCollectionView<RefUnit>();
            Units.OnRefresh += new EventHandler<aEMR.Common.Collections.RefreshEventArgs>(Units_OnRefresh);
            Units.PageSize = Globals.PageSize;
            SelectedUnit = new RefUnit();
            SelectedUnit.UnitVolume = 1;
            Content = ADD;
            Contenttitle = ADDTITLE;
        }

        void Units_OnRefresh(object sender, aEMR.Common.Collections.RefreshEventArgs e)
        {
            SearchUnits(Units.PageIndex, Units.PageSize);
        }

        protected override void OnActivate()
        {
            //base.OnActivate();
            Units.PageIndex = 0;
            SearchUnits(0, Units.PageSize);
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
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bTim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyDonViTinh,
                                               (int)oPharmacyEx.mQuanLyDonViTinh_Tim, (int)ePermission.mView);
            bThem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyDonViTinh,
                                               (int)oPharmacyEx.mQuanLyDonViTinh_Them, (int)ePermission.mView);
            bChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyDonViTinh,
                                               (int)oPharmacyEx.mQuanLyDonViTinh_ChinhSua, (int)ePermission.mView);
            

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

        #region binding visibilty

        public Button lnkDelete { get; set; }
        public Button lnkView { get; set; }

        public void lnkDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;
            lnkDelete.Visibility = Globals.convertVisibility(bChinhSua);
        }
        public void lnkView_Loaded(object sender)
        {
            lnkView = sender as Button;
            lnkView.Visibility = Globals.convertVisibility(bChinhSua);
        }
        #endregion
        #endregion

        public string TitleForm { get; set; }
        private PagedSortableCollectionView<RefUnit> _units;
        public PagedSortableCollectionView<RefUnit> Units
        {
            get
            {
                return _units;
            }
            set
            {
                if (_units != value)
                {
                    _units = value;
                    NotifyOfPropertyChange(() => Units);
                }
            }
        }


        private RefUnit _selectedUnit;
        public RefUnit SelectedUnit
        {
            get { return _selectedUnit; }
            set
            {
                if (_selectedUnit != value)
                {
                    _selectedUnit = value;
                    NotifyOfPropertyChange(() => SelectedUnit);
                }
            }
        }

        private string _unitName;
        public string UnitName
        {
            get { return _unitName; }
            set
            {
                if (_unitName != value)
                {
                    _unitName = value;
                    NotifyOfPropertyChange(() => UnitName);
                }
            }
        }

        private string ADD = eHCMSResources.G0156_G1_Them;
        private string UPDATE = eHCMSResources.K1599_G1_CNhat;

        private string ADDTITLE = string.Format("{0} {1}", eHCMSResources.G0276_G1_ThemMoi, eHCMSResources.K3966_G1_Dvt);
        private string UPDATETITLE = string.Format("{0} {1}", eHCMSResources.K1599_G1_CNhat, eHCMSResources.K3966_G1_Dvt);

        private Visibility _Visibility = Visibility.Collapsed;
        public Visibility Visibility
        {
            get
            {
                return _Visibility;
            }
            set
            {
                if (_Visibility != value)
                {
                    _Visibility = value;
                    NotifyOfPropertyChange(() => Visibility);
                }
                if (Visibility == Visibility.Visible)
                {
                    NotExpanded = false;
                }
                else
                {
                    NotExpanded = true;
                }
            }
        }

        private bool _NotExpanded = true;
        public bool NotExpanded
        {
            get
            {
                return _NotExpanded;
            }
            set
            {
                if (_NotExpanded != value)
                {
                    _NotExpanded = value;
                    NotifyOfPropertyChange(() => NotExpanded);
                }
            }
        }

        private string _Content;
        public string Content
        {
            get
            {
                return _Content;
            }
            set
            {
                if (_Content != value)
                {
                    _Content = value;
                    NotifyOfPropertyChange(() => Content);
                }
            }
        }

        private string _Contenttitle;
        public string Contenttitle
        {
            get
            {
                return _Contenttitle;
            }
            set
            {
                if (_Contenttitle != value)
                {
                    _Contenttitle = value;
                    NotifyOfPropertyChange(() => Contenttitle);
                }
            }
        }

        private void InitUnit()
        {
            SelectedUnit = null;
            SelectedUnit = new RefUnit();
        }

        public void AddNewUnit(RefUnit SelectedUnit)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            IsLoading = true;
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PharmacyUnitsServiceClient())
                                       {
                                           var contract = serviceFactory.ServiceInstance;

                                           contract.BeginAddNewUnit(SelectedUnit, Globals.DispatchCallback((asyncResult) =>
                                           {

                                               try
                                               {
                                                   contract.EndAddNewUnit(asyncResult);
                                                   Globals.ShowMessage(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK, eHCMSResources.G0442_G1_TBao);
                                                   Visibility = Visibility.Collapsed;
                                                   InitUnit();
                                                   Search();

                                               }
                                               catch (Exception ex)
                                               {
                                                   Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                                   _logger.Info(ex.Message);
                                               }
                                               finally
                                               {
                                                   this.DlgHideBusyIndicator();
                                                   IsLoading = false;
                                                   //Globals.IsBusy = false;
                                               }

                                           }), null);
                                       }
                                   });

            t.Start();
        }

        public void UpdateUnit(RefUnit SelectedUnit)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyUnitsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginUpdateUnit(SelectedUnit, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            contract.EndUpdateUnit(asyncResult);
                            Globals.ShowMessage(eHCMSResources.A0279_G1_Msg_InfoCNhatOK, eHCMSResources.G0442_G1_TBao);
                            Visibility = Visibility.Collapsed;
                            Content = ADD;
                            Contenttitle = ADDTITLE;
                            InitUnit();
                            Search();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            _logger.Info(ex.Message);
                        }
                        finally
                        {
                            this.DlgHideBusyIndicator();
                            IsLoading = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void DeleteUnit(long UnitID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyUnitsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginDeleteUnitByID(UnitID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            contract.EndDeleteUnitByID(asyncResult);
                            Globals.ShowMessage(eHCMSResources.K0537_G1_XoaOk, eHCMSResources.G0442_G1_TBao);
                            Search();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            _logger.Info(ex.Message);
                        }
                        finally
                        {
                            this.DlgHideBusyIndicator();
                            IsLoading = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void SearchUnits(int PageIndex, int PageSize)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            int totalCount = 0;
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyUnitsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSearchUnit(UnitName, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndSearchUnit(out totalCount, asyncResult);
                            if (results != null)
                            {
                                Units.Clear();
                                Units.TotalItemCount = totalCount;
                                foreach (RefUnit p in results)
                                {
                                    Units.Add(p);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            _logger.Info(ex.Message);
                        }
                        finally
                        {
                            this.DlgHideBusyIndicator();
                            IsLoading = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void txt_search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                UnitName = (sender as TextBox).Text;
                Units.PageIndex = 0;
                SearchUnits(0, Units.PageSize);
            }
        }

        public void Search()
        {
            Units.PageIndex = 0;
            SearchUnits(0, Units.PageSize);
        }
        DataGrid GridUnits = null;
        public void GridUnits_Loaded(object sender, RoutedEventArgs e)
        {
            GridUnits = sender as DataGrid;
        }
        public void ViewClick(object sender, RoutedEventArgs e)
        {
            if (GridUnits != null && GridUnits.SelectedItem != null)
            {
                Content = UPDATE;
                Contenttitle = UPDATETITLE;

                Visibility =  Visibility.Visible;
                SelectedUnit = (GridUnits.SelectedItem as RefUnit).DeepCopy();
            }
        }

        public void DoubleClick(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;
            SelectedUnit = eventArgs.Value as RefUnit;

            if (SelectedUnit != null)
            {
                Content = UPDATE;
                Contenttitle = UPDATETITLE;

                Visibility = Visibility.Visible;
            }
        }

        public void DeletedClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.A0158_G1_Msg_ConfXoaDVT, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (GridUnits.SelectedItem != null)
                    DeleteUnit(((RefUnit)GridUnits.SelectedItem).UnitID);
            }
        }

        public void btnCancel()
        {
            Content = ADD;
            Contenttitle = ADDTITLE;
            InitUnit();
            Visibility = Visibility.Collapsed;
        }

        public bool CheckValid(object temp)
        {
            RefUnit u = temp as RefUnit;
            if (u == null)
            {
                return false;
            }
            return u.Validate();
        }

        public void btnAddOrUpdate()
        {
            if (CheckValid(SelectedUnit))
            {
                if (Content == ADD)
                {
                    AddNewUnit(SelectedUnit);
                }
                else
                {
                    UpdateUnit(SelectedUnit);
                }
            }
        }
        public void hlbAdd()
        {
            Visibility = Visibility.Visible;
        }
    }
}
