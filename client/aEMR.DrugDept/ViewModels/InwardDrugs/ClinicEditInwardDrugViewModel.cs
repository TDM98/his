using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ViewContracts;
using aEMR.ServiceClient;
using aEMR.Common.Collections;
using Caliburn.Micro;
using Castle.Windsor;
using Castle.Core.Logging;
using DataEntities;
using eHCMSLanguage;

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IDrugDeptClinicEditInwardDrug)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ClinicEditInwardDrugViewModel : Conductor<object>, IDrugDeptClinicEditInwardDrug
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ClinicEditInwardDrugViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            CurrentInwardDrugClinicDeptCopy = new InwardDrugClinicDept();
           // LoadGoodsTypeCbx();
        }

        #region Properties Member

        private InwardDrugClinicDept _CurrentInwardDrugClinicDeptCopy;
        public InwardDrugClinicDept CurrentInwardDrugClinicDeptCopy
        {
            get
            {
                return _CurrentInwardDrugClinicDeptCopy;
            }
            set
            {
                if (_CurrentInwardDrugClinicDeptCopy != value)
                {
                    _CurrentInwardDrugClinicDeptCopy = value;
                    NotifyOfPropertyChange(() => CurrentInwardDrugClinicDeptCopy);
                }
            }
        }

        private ObservableCollection<Lookup> _CbxGoodsTypes;
        public ObservableCollection<Lookup> CbxGoodsTypes
        {
            get
            {
                return _CbxGoodsTypes;
            }
            set
            {
                _CbxGoodsTypes = value;
                NotifyOfPropertyChange(() => CbxGoodsTypes);
            }
        }

        #endregion

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

        #region Auto For Location

        private ObservableCollection<RefShelfDrugLocation> _Location;
        public ObservableCollection<RefShelfDrugLocation> Location
        {
            get
            {
                return _Location;
            }
            set
            {
                if (_Location != value)
                {
                    _Location = value;
                    NotifyOfPropertyChange(() => Location);
                }
            }
        }

        AutoCompleteBox Au;
        private void SearchRefShelfLocation(string Name)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetRefShelfDrugLocationAutoComplete(Name, 0, Globals.PageSize, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetRefShelfDrugLocationAutoComplete(asyncResult);
                            Location = results.ToObservableCollection();
                            Au.ItemsSource = Location;
                            Au.PopulateComplete();
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

        public void AutoLocation_Text_Populating(object sender, PopulatingEventArgs e)
        {
            Au = sender as AutoCompleteBox;
            SearchRefShelfLocation(e.Parameter);
        }

        #endregion

        private void UpdateInwardDrug()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateInwardDrugClinicDept(CurrentInwardDrugClinicDeptCopy,Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int results = contract.EndUpdateInwardDrugClinicDept(asyncResult);
                            if (results == 0)
                            {
                                TryClose();
                                //phat su kien o day
                                Globals.EventAggregator.Publish(new ClinicDrugDeptCloseEditInwardEvent { });
                            }
                            else if (results == 1)
                            {
                                MessageBox.Show(eHCMSResources.K0049_G1_ThuocXuatLonHonSLggChSua, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                            }
                            else if (results == 2)
                            {
                                MessageBox.Show(eHCMSResources.K0069_G1_ThuocTheoSoLoDaTonTai, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                            }
                            else
                            {
                                MessageBox.Show(string.Format("{0}.", eHCMSResources.A0755_G1_Msg_InfoKhTonTaiDongData), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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

        public void OKButton(object sender, RoutedEventArgs e)
        {
            if (CheckValid())
            {
                if (CurrentInwardDrugClinicDeptCopy.InQuantity <= 0)
                {
                    Globals.ShowMessage(eHCMSResources.A0789_G1_Msg_InfoSLgNhapLonHon0, eHCMSResources.G0442_G1_TBao);
                    return;
                }
                if (Au != null)
                {
                    CurrentInwardDrugClinicDeptCopy.SdlDescription = Au.Text;
                }
                UpdateInwardDrug();
            }
        }
        public bool CheckValid()
        {
            if (CurrentInwardDrugClinicDeptCopy == null)
            {
                return false;
            }
            return CurrentInwardDrugClinicDeptCopy.Validate();
        }
        public void CancelButton(object sender, RoutedEventArgs e)
        {
            TryClose();
        }
    }
}
