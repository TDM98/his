using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using DataEntities;
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using System.Linq;
using aEMR.Common;
using System.Windows.Controls;
using Castle.Windsor;
using Caliburn.Micro;

namespace aEMR.Registration.ViewModels
{
    [Export(typeof(ICharityOrganization)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CharityOrganizationViewModel : ViewModelBase, ICharityOrganization
    {
        [ImportingConstructor]
        public CharityOrganizationViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Globals.EventAggregator.Subscribe(this);
            CharityOrganization = new CharityOrganization();
            GetAllCharityOrganization();
        }

        private CharityOrganization _CharityOrganization;
        public CharityOrganization CharityOrganization
        {
            get
            {
                return _CharityOrganization;
            }
            set
            {
                if(_CharityOrganization != value)
                {
                    _CharityOrganization = value;
                    NotifyOfPropertyChange(() => CharityOrganization);
                }
            }
        }

        private ObservableCollection<CharityOrganization> _ObsCharityOrganization;
        public ObservableCollection<CharityOrganization> ObsCharityOrganization
        {
            get
            {
                return _ObsCharityOrganization;
            }
            set
            {
                if (_ObsCharityOrganization != value)
                {
                    _ObsCharityOrganization = value;
                    NotifyOfPropertyChange(() => ObsCharityOrganization);
                }
            }
        }

        private CharityOrganization _selectedCharityOrganization;
        public CharityOrganization selectedCharityOrganization
        {
            get
            {
                return _selectedCharityOrganization;
            }
            set
            {
                if (_selectedCharityOrganization != value)
                {
                    _selectedCharityOrganization = value;
                    NotifyOfPropertyChange(() => selectedCharityOrganization);
                }
            }
        }

        public void btnAdd()
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginAddCharityOrganization(CharityOrganization.CharityOrgName, Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    bool result = contract.EndAddCharityOrganization(asyncResult);
                                    if (result)
                                    {
                                        MessageBox.Show(eHCMSResources.A1027_G1_Msg_InfoThemOK);
                                        GetAllCharityOrganization();
                                        CharityOrganization = new CharityOrganization();
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.A1026_G1_Msg_InfoThemFail);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                    ClientLoggerHelper.LogError(ex.Message);
                                }
                                finally
                                {
                                    this.DlgHideBusyIndicator();
                                }

                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }

        public void btnEdit()
        {
            if (selectedCharityOrganization != null)
            {
                EditCharityOrganization(Convert.ToInt64(selectedCharityOrganization.CharityOrgID), CharityOrganization.CharityOrgName);
            }
            else
            {
                MessageBox.Show(eHCMSResources.Z2636_G1_ChuaChonToChuc);
            }
        }
        public void EditCharityOrganization(long CharityOrgID, string CharityOrgName)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginEditCharityOrganization(CharityOrgID, CharityOrgName, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool result = contract.EndEditCharityOrganization(asyncResult);
                                if (result)
                                {
                                    MessageBox.Show(eHCMSResources.A0296_G1_Msg_InfoSuaOK);
                                    GetAllCharityOrganization();
                                    CharityOrganization = new CharityOrganization();
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.Z1723_G1_ChSuaThatBai);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                ClientLoggerHelper.LogError(ex.Message);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }

        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCharityOrganization != null)
            {
                if (MessageBox.Show(eHCMSResources.Z2637_G1_BanMuonXoaToChuc, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    DeleteCharityOrganization(Convert.ToInt64(selectedCharityOrganization.CharityOrgID));
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.Z2636_G1_ChuaChonToChuc);
            }
        }

        private void DeleteCharityOrganization(long CharityOrgID)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeleteCharityOrganization(CharityOrgID, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool result = contract.EndDeleteCharityOrganization(asyncResult);
                                if (result)
                                {
                                    MessageBox.Show(eHCMSResources.K0537_G1_XoaOk);
                                    GetAllCharityOrganization();
                                    CharityOrganization = new CharityOrganization();
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.K0484_G1_XoaFail);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                ClientLoggerHelper.LogError(ex.Message);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }

        public void GetAllCharityOrganization()
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllCharityOrganization(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                List<CharityOrganization> result = contract.EndGetAllCharityOrganization(asyncResult);
                                if (result != null)
                                {
                                    ObsCharityOrganization = new ObservableCollection<CharityOrganization>();
                                    foreach (var charity in result)
                                    {
                                        ObsCharityOrganization.Add(charity);
                                    }
                                }
                                NotifyOfPropertyChange(() => ObsCharityOrganization);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                ClientLoggerHelper.LogError(ex.Message);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }

        public void CharityOrganizationSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectedCharityOrganization != null)
            {
                CharityOrganization = selectedCharityOrganization.DeepCopy();
            }
        }
    }
}



