using eHCMSLanguage;
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using DataEntities;
using System.Threading;
using aEMR.ServiceClient;
using aEMR.Common.Collections;
using aEMR.Common;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.ViewContracts;

namespace aEMR.Configuration.PCLExamTypeCombo.ViewModels
{
    [Export(typeof(IPCLExamTypeCombo)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLExamTypeComboViewModel : Conductor<object>, IPCLExamTypeCombo
        , IHandle<PCLExamTypeComboEvent_AddEditSave>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PCLExamTypeComboViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            //Globals.EventAggregator.Subscribe(this);
            SearchCriteria = new GeneralSearchCriteria();
            btnSearch();
        }

        #region Properties member

        private GeneralSearchCriteria _SearchCriteria;
        public GeneralSearchCriteria SearchCriteria
        {
            get { return _SearchCriteria; }
            set
            {
                _SearchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }

        private ObservableCollection<DataEntities.PCLExamTypeCombo> _PCLExamTypeComboLst;
        public ObservableCollection<DataEntities.PCLExamTypeCombo> PCLExamTypeComboLst
        {
            get { return _PCLExamTypeComboLst; }
            set
            {
                _PCLExamTypeComboLst = value;
                NotifyOfPropertyChange(() => PCLExamTypeComboLst);
            }
        }

        #endregion

        #region Function member

        public void hplDelete_Click(object datacontext)
        {
            DataEntities.PCLExamTypeCombo p = (datacontext as DataEntities.PCLExamTypeCombo);

            if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, p.ComboName), eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                PCLPCLExamTypeCombo_Delete(p.PCLExamTypeComboID);
            }
        }

        public void hplAddNewExamTypeCombo_Click()
        {
            //var typeInfo = Globals.GetViewModel<IPCLExamTypeCombo_AddEdit>();
            //typeInfo.TitleForm = eHCMSResources.G0276_G1_ThemMoi;
            //typeInfo.ObjPCLExamTypeCombo_Current = new DataEntities.PCLExamTypeCombo();
            //typeInfo.ObjPCLExamTypeCombo_Current.CreatorStaffID = Globals.LoggedUserAccount.StaffID.GetValueOrDefault();
            //typeInfo.ObjPCLExamTypeCombo_Current.StaffName = Globals.LoggedUserAccount.Staff.FullName;

            //var instance = typeInfo as Conductor<object>;
            ////instance.DisplayName = "QUẢN LÝ EXAMTEST";
            //Globals.ShowDialog(instance, (o) =>
            //{
            //    //làm gì đó
            //});

            Action<IPCLExamTypeCombo_AddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = eHCMSResources.G0276_G1_ThemMoi;
                typeInfo.ObjPCLExamTypeCombo_Current = new DataEntities.PCLExamTypeCombo();
                typeInfo.ObjPCLExamTypeCombo_Current.CreatorStaffID = Globals.LoggedUserAccount.StaffID.GetValueOrDefault();
                typeInfo.ObjPCLExamTypeCombo_Current.StaffName = Globals.LoggedUserAccount.Staff.FullName;
            };
            GlobalsNAV.ShowDialog<IPCLExamTypeCombo_AddEdit>(onInitDlg);
        }

        public void DoubleClick(Common.EventArgs<object> e)
        {
            DataEntities.PCLExamTypeCombo item = e.Value as DataEntities.PCLExamTypeCombo;
            //var typeInfo = Globals.GetViewModel<IPCLExamTypeCombo_AddEdit>();
            //typeInfo.TitleForm = eHCMSResources.K1599_G1_CNhat;

            //typeInfo.ObjPCLExamTypeCombo_Current = item;
            //typeInfo.PCLExamTypeCombo_GetDetails(item.PCLExamTypeComboID);
            //var instance = typeInfo as Conductor<object>;
            ////instance.DisplayName = "QUẢN LÝ EXAMTEST";
            //Globals.ShowDialog(instance, (o) =>
            //{
            //    //làm gì đó
            //});

            Action<IPCLExamTypeCombo_AddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = eHCMSResources.K1599_G1_CNhat;
                typeInfo.ObjPCLExamTypeCombo_Current = item;
                typeInfo.PCLExamTypeCombo_GetDetails(item.PCLExamTypeComboID);
            };
            GlobalsNAV.ShowDialog<IPCLExamTypeCombo_AddEdit>(onInitDlg);

        }

        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        public void btnSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.FindName = (sender as TextBox).Text;
                }
                btnSearch();
            }
        }

        public void btnSearch()
        {
            PCLPCLExamTypeCombo_Search();
        }


        private void PCLPCLExamTypeCombo_Search()
        {
            var t = new Thread(() =>
            {
                //IsLoading = true;

                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLExamTypeCombo_Search(SearchCriteria, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var allItems = client.EndPCLExamTypeCombo_Search(asyncResult);
                                PCLExamTypeComboLst = allItems.ToObservableCollection();
                            }
                            catch (Exception innerEx)
                            {
                                ClientLoggerHelper.LogInfo(innerEx.ToString());
                            }

                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
            });
            t.Start();
        }

        private void PCLPCLExamTypeCombo_Delete(long ID)
        {
            var t = new Thread(() =>
            {
                //IsLoading = true;

                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLExamTypeCombo_Delete(ID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool bOK = client.EndPCLExamTypeCombo_Delete(asyncResult);
                                if (bOK)
                                {
                                    MessageBox.Show(eHCMSResources.A0478_G1_Msg_InfoXoaOK);
                                    btnSearch();
                                }
                              
                            }
                            catch (Exception innerEx)
                            {
                                ClientLoggerHelper.LogInfo(innerEx.ToString());
                            }

                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
            });
            t.Start();
        }
        #endregion

        public void Handle(PCLExamTypeComboEvent_AddEditSave message)
        {
            if (message != null)
            {
                btnSearch();
            }
        }
    }
}
