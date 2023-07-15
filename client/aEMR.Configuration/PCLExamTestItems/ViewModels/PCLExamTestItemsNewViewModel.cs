using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts.Configuration;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using System.Threading;
using aEMR.ServiceClient;
using aEMR.Common;
using DataEntities;
using eHCMSLanguage;
using aEMR.ViewContracts;

namespace aEMR.Configuration.PCLExamTestItems.ViewModels
{
    [Export(typeof(IPCLExamTestItemsNew)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLExamTestItemsNewViewModel : Conductor<object>, IPCLExamTestItemsNew
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PCLExamTestItemsNewViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }

        #region Properties member
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

        private bool _CheckTestItemIsExamType;
        public bool CheckTestItemIsExamType
        {
            get { return _CheckTestItemIsExamType; }
            set
            {
                if (_CheckTestItemIsExamType != value)
                {
                    _CheckTestItemIsExamType = value;
                    NotifyOfPropertyChange(() => CheckTestItemIsExamType);
                }
            }
        }


        private bool _InputMulti;
        public bool InputMulti
        {
            get { return _InputMulti; }
            set
            {
                if (_InputMulti != value)
                {
                    _InputMulti = value;
                    NotifyOfPropertyChange(() => InputMulti);
                }
            }
        }

        private DataEntities.PCLExamTestItems _ObjPCLExamTestItems_Current;
        public DataEntities.PCLExamTestItems ObjPCLExamTestItems_Current
        {
            get { return _ObjPCLExamTestItems_Current; }
            set
            {
                _ObjPCLExamTestItems_Current = value;
                NotifyOfPropertyChange(() => ObjPCLExamTestItems_Current);
                if (_ObjPCLExamTestItems_Current.PCLExamTestItemID > 0)
                {
                    TitleForm = "CHỈNH SỬA EXAMTEST";
                }
                else
                {
                    TitleForm = "THÊM MỚI EXAMTEST";
                }
                NotifyOfPropertyChange(() => TitleForm);
            }
        }


        private PCLExamType _ObjPCLExamType_Current;
        public PCLExamType ObjPCLExamType_Current
        {
            get { return _ObjPCLExamType_Current; }
            set
            {
                _ObjPCLExamType_Current = value;
                NotifyOfPropertyChange(() => ObjPCLExamType_Current);
            }
        }

        #endregion
        public void chkInputMulti_Click(object sender, RoutedEventArgs e)
        {
            CheckBox Ctr = sender as CheckBox;
            if (Ctr != null)
            {
                InputMulti = Ctr.IsChecked.Value;
            }
        }


        public void InitializeNewItem()
        {
            ObjPCLExamTestItems_Current = new DataEntities.PCLExamTestItems();
            ObjPCLExamTestItems_Current.PCLExamTestItemName = "";
            ObjPCLExamTestItems_Current.PCLExamTestItemDescription = "";
            ObjPCLExamTestItems_Current.PCLExamTestItemCode = "";
            ObjPCLExamTestItems_Current.PCLExamTestItemUnit = "";
            ObjPCLExamTestItems_Current.PCLExamTestItemRefScale = "";
            ObjPCLExamTestItems_Current.TestItemIsExamType = false;
            ObjPCLExamTestItems_Current.IsActive = true;
        }


        public void chkCheckTestItemIsExamType_Click(object sender, RoutedEventArgs e)
        {
            CheckBox Ctr = sender as CheckBox;
            if (Ctr != null)
            {
                if (ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList.Count == 0)
                {
                    CheckTestItemIsExamType = !Ctr.IsChecked.Value;
                    ObjPCLExamTestItems_Current.TestItemIsExamType = Ctr.IsChecked.Value;

                    AcceptForLab();
                }
                else
                {
                    if (ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList.Count > 0)
                    {
                        if (ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList.Count > 1)
                        {
                            if (MessageBox.Show(string.Format(eHCMSResources.Z1439_G1_I, ObjPCLExamType_Current.PCLExamTypeName.Trim()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                            {
                                CheckTestItemIsExamType = !Ctr.IsChecked.Value;
                                ObjPCLExamTestItems_Current.TestItemIsExamType = Ctr.IsChecked.Value;

                                ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList[0].V_PCLExamTestItem.TestItemIsExamType = true;

                                AcceptForLab();
                            }
                            else
                            {
                                ObjPCLExamTestItems_Current.TestItemIsExamType = false;
                            }
                        }
                        else
                        {
                            if (ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList[0].V_PCLExamTestItem.TestItemIsExamType == false)
                            {
                                if (MessageBox.Show(string.Format(eHCMSResources.Z1439_G1_I, ObjPCLExamType_Current.PCLExamTypeName.Trim()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                {

                                    CheckTestItemIsExamType = !Ctr.IsChecked.Value;
                                    ObjPCLExamTestItems_Current.TestItemIsExamType = Ctr.IsChecked.Value;

                                    ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList[0].V_PCLExamTestItem.TestItemIsExamType = true;

                                    AcceptForLab();
                                }
                                else
                                {
                                    ObjPCLExamTestItems_Current.TestItemIsExamType = false;
                                }
                            }
                            else
                            {
                                CheckTestItemIsExamType = !Ctr.IsChecked.Value;
                                ObjPCLExamTestItems_Current.TestItemIsExamType = Ctr.IsChecked.Value;

                                AcceptForLab();
                            }
                        }
                    }
                    else
                    {
                        CheckTestItemIsExamType = !Ctr.IsChecked.Value;
                        ObjPCLExamTestItems_Current.TestItemIsExamType = Ctr.IsChecked.Value;

                        AcceptForLab();
                    }
                }

            }
        }

        private void AcceptForLab()
        {
            ObjPCLExamTestItems_Current.PCLExamTestItemCode = ObjPCLExamType_Current.PCLExamTypeCode.Trim();
            ObjPCLExamTestItems_Current.PCLExamTestItemName = ObjPCLExamType_Current.PCLExamTypeName.Trim();
            ObjPCLExamType_Current.PCLExamTypeDescription = ObjPCLExamType_Current.PCLExamTypeDescription.Trim();
        }

        public void btSave()
        {
            if (CheckValid())
            {
                PCLExamTestItems_Save();
            }
        }

        private bool CheckValid()
        {
            if (ObjPCLExamTestItems_Current.TestItemIsExamType)
                return true;

            if (string.IsNullOrEmpty(ObjPCLExamTestItems_Current.PCLExamTestItemCode))
            {
                MessageBox.Show(eHCMSResources.A0873_G1_Msg_InfoNhapMaPCLExamTest);
                return false;
            }

            if (string.IsNullOrEmpty(ObjPCLExamTestItems_Current.PCLExamTestItemName))
            {
                MessageBox.Show(eHCMSResources.A0880_G1_Msg_InfoNhapTenPCLExamTest);
                return false;
            }

            return true;
        }

        public void btClose()
        {
            TryClose();
        }

        private void PCLExamTestItems_Save()
        {
            var t = new Thread(() =>
            {
                //IsLoading = true;

                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLExamTestItems_Save(ObjPCLExamTestItems_Current, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                long ID = 0;
                                var bOK = client.EndPCLExamTestItems_Save(out ID, asyncResult);
                                if (bOK)
                                {
                                    if (ObjPCLExamTestItems_Current.PCLExamTestItemID <= 0)
                                    {
                                        MessageBox.Show(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK);
                                        if (ObjPCLExamTestItems_Current.TestItemIsExamType)
                                        {
                                            ObjPCLExamTestItems_Current.PCLExamTestItemID = ID;
                                            Globals.EventAggregator.Publish(new SelectedObjectEvent<DataEntities.PCLExamTestItems>() { Result = ObjPCLExamTestItems_Current.DeepCopy() });
                                            TryClose();
                                        }
                                        else
                                        {
                                            InitializeNewItem();
                                            Globals.EventAggregator.Publish(new PCLItemsEvent());
                                            if (!InputMulti)
                                            {
                                                TryClose();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK);
                                        InitializeNewItem();
                                        Globals.EventAggregator.Publish(new PCLItemsEvent());
                                        TryClose();
                                    }
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0812_G1_Msg_InfoMaDaTonTai);
                                }
                            }
                            catch (Exception innerEx)
                            {
                                Globals.ShowMessage(innerEx.Message, eHCMSResources.T0432_G1_Error);
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

        public void btClear()
        {
            InitializeNewItem();
        }

    }
}
