using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts.Configuration;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using eHCMSLanguage;
using Caliburn.Micro;
using aEMR.ViewContracts;

namespace aEMR.Configuration.PCLExamTestItems.ViewModels
{
    [Export(typeof(IPCLExamTestItems)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLExamTestItemsViewModel : Conductor<object>, IPCLExamTestItems
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PCLExamTestItemsViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
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


        public void FormLoad()
        {
            if (ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList.Count > 0)
            {
                if (ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList.Count > 1)
                {
                    CheckTestItemIsExamType = true;
                }
                else
                {
                    if (ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList[0].V_PCLExamTestItem.TestItemIsExamType == false)
                    {
                        CheckTestItemIsExamType = true;
                    }
                    else
                    {
                        CheckTestItemIsExamType = false;
                    }
                }
            }
            else
            {
                CheckTestItemIsExamType = true;
            }

            InputMulti = false;
        }

        private DataEntities.PCLExamTestItems _ObjPCLExamTestItems_Current;
        public DataEntities.PCLExamTestItems ObjPCLExamTestItems_Current
        {
            get { return _ObjPCLExamTestItems_Current; }
            set
            {
                _ObjPCLExamTestItems_Current = value;
                NotifyOfPropertyChange(() => ObjPCLExamTestItems_Current);
            }
        }


        private DataEntities.PCLExamType _ObjPCLExamType_Current;
        public DataEntities.PCLExamType ObjPCLExamType_Current
        {
            get { return _ObjPCLExamType_Current; }
            set
            {
                _ObjPCLExamType_Current = value;
                NotifyOfPropertyChange(() => ObjPCLExamType_Current);
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
                    if (ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList.Count>0)
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


        public void chkInputMulti_Click(object sender, RoutedEventArgs e)
        {
            CheckBox Ctr = sender as CheckBox;
            if (Ctr != null)
            {
                InputMulti = Ctr.IsChecked.Value;
            }
        }

        public void btSave()
        {
            if (CheckValid())
            {
                Globals.EventAggregator.Publish(new SelectedObjectEvent<DataEntities.PCLExamTestItems>() { Result = ObjPCLExamTestItems_Current });
                if (ObjPCLExamTestItems_Current.TestItemIsExamType)
                {
                    TryClose();
                }
                else
                {
                    if (InputMulti)
                    {
                        InitializeNewItem();
                    }
                    else
                    {
                        TryClose();
                    }
                }
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

            //if (string.IsNullOrEmpty(ObjPCLExamTestItems_Current.PCLExamTestItemName))
            //{
            //    MessageBox.Show("Nhập Mã PCLExamTest!");
            //    return false;
            //}

            return true;
        }

        public void btClose()
        {
            TryClose();
        }
    }
}
