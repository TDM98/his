using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;

using DataEntities;
using System.Threading;
using aEMR.ViewContracts;
using aEMR.Common.Printing;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using System.Windows.Controls;
using aEMR.Common;
using aEMR.Common.ConfigurationManager.Printer;
using aEMR.ServiceClient;

namespace aEMR.SystemConfiguration.ViewModels
{
    [Export(typeof(IPrinterSettings)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PrinterSettingsViewModel : Conductor<object>, IPrinterSettings
        ,IHandle<ItemSelected<PrinterInfo>>
    {
        [ImportingConstructor]
        public PrinterSettingsViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            authorization();
            AvailablePrintersContent = Globals.GetViewModel<IPrinterListing>();
            AvailablePrintersContent.GetAllPrinters();
            GetAllPrinterTypeAssignments();
            eventArg.Subscribe(this);
        }

        private bool _mPrinterSettings_SaveOrNot = true;

        public bool mPrinterSettings_SaveOrNot
        {
            get
            {
                return _mPrinterSettings_SaveOrNot;
            }
            set
            {
                if (_mPrinterSettings_SaveOrNot == value)
                    return;
                _mPrinterSettings_SaveOrNot = value;
                NotifyOfPropertyChange(() => mPrinterSettings_SaveOrNot);
            }
        }

        private bool _mPrinterSettings_TestPrint = true;

        public bool mPrinterSettings_TestPrint
        {
            get
            {
                return _mPrinterSettings_TestPrint;
            }
            set
            {
                if (_mPrinterSettings_TestPrint == value)
                    return;
                _mPrinterSettings_TestPrint = value;
                NotifyOfPropertyChange(() => mPrinterSettings_TestPrint);
            }
        }


        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            mPrinterSettings_SaveOrNot = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mSystem_Management
                                               , (int)eSystem_Management.mPrinterSettings,
                                               (int)oSystem_ManagementEx.mSave, (int)ePermission.mView);
            mPrinterSettings_TestPrint = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mSystem_Management
                                               , (int)eSystem_Management.mPrinterSettings,
                                               (int)oSystem_ManagementEx.mTestPrint, (int)ePermission.mView);
        }

        private IPrinterListing _availablePrintersContent;
        public IPrinterListing AvailablePrintersContent
        {
            get { return _availablePrintersContent; }
            set
            {
                _availablePrintersContent = value;
                NotifyOfPropertyChange(() => AvailablePrintersContent);
            }
        }
        private ObservableCollection<PrinterTypePrinterAssignment> _tempAllPrinterTypePrinterAssignments;

        private ObservableCollection<PrinterTypePrinterAssignment> _allPrinterTypePrinterAssignments;

        public ObservableCollection<PrinterTypePrinterAssignment> AllPrinterTypePrinterAssignments
        {
            get { return _allPrinterTypePrinterAssignments; }
            set
            {
                _allPrinterTypePrinterAssignments = value;
                NotifyOfPropertyChange(() => AllPrinterTypePrinterAssignments);
            }
        }

        private bool _isSaving;
        public bool IsSaving
        {
            get
            {
                return _isSaving;
            }
            set
            {
                _isSaving = value;
                NotifyOfPropertyChange(() => IsSaving);

                NotifyOfPropertyChange(() => CanCancelChangesCmd);
                NotifyOfPropertyChange(() => CanSaveCmd);
            }
        }

        public void GetAllPrinterTypeAssignments()
        {
            var printerConfigManager = new PrinterConfigurationManager();
            var allAssignedPrinterTypes = printerConfigManager.GetAllAssignedPrinterType();
            var enumList = new ObservableCollection<EnumDescription>();

            Array enumValues = Enum.GetValues(typeof(PrinterType));
            var allAssignments = new ObservableCollection<PrinterTypePrinterAssignment>();
            if (enumValues != null)
            {
                foreach (PrinterType enumValue in enumValues)
                {
                    var temp = new PrinterTypePrinterAssignment {PrinterType = enumValue};
                    if(allAssignedPrinterTypes.ContainsKey(enumValue))
                    {
                        temp.AssignedPrinterName = allAssignedPrinterTypes[enumValue];
                    }
                    allAssignments.Add(temp);
                }
            }
            AllPrinterTypePrinterAssignments = allAssignments;

            if (AllPrinterTypePrinterAssignments != null && AllPrinterTypePrinterAssignments.Count > 0)
            {
                SelectedPrinterTypeAssigment = AllPrinterTypePrinterAssignments[0];
            }

            BeginEdit();
        }

        public void SelectPrinterCmd(Button sender, EventArgs eventArgs)
        {
            _selectedPrinterTypeAssigment = (PrinterTypePrinterAssignment)sender.DataContext;
            Action<IPrinterListing> onInitDlg = delegate (IPrinterListing vm)
            {
                vm.GetAllPrinters();
                vm.ShowButtonPanel = true;
            };
            GlobalsNAV.ShowDialog<IPrinterListing>(onInitDlg);
        }

        private PrinterTypePrinterAssignment _selectedPrinterTypeAssigment = null;
        public PrinterTypePrinterAssignment SelectedPrinterTypeAssigment
        {
            get
            {
                return _selectedPrinterTypeAssigment;
            }
            set
            {
                if (_selectedPrinterTypeAssigment != value)
                {
                    _selectedPrinterTypeAssigment = value;
                    NotifyOfPropertyChange(() => SelectedPrinterTypeAssigment);
                }
            }
        }

        public void RemovePrinterCmd(Button sender, EventArgs eventArgs)
        {
            var ctx = (PrinterTypePrinterAssignment)sender.DataContext;
            ctx.AssignedPrinterName = string.Empty;
            InfoHasChanged = true;
        }

        public void Handle(ItemSelected<PrinterInfo> message)
        {
            if(GetView() != null && message != null)
            {
                if(_selectedPrinterTypeAssigment != null && message.Item != null)
                {
                    _selectedPrinterTypeAssigment.AssignedPrinterName = message.Item.PrinterName;
                    InfoHasChanged = true;
                }
            }
        }
        private bool _infoHasChanged;
        private bool InfoHasChanged
        {
            get
            {
                return _infoHasChanged;
            }
            set
            {
                _infoHasChanged = value;
                NotifyOfPropertyChange(() => InfoHasChanged);

                NotifyOfPropertyChange(() => CanSaveCmd);
                NotifyOfPropertyChange(() => CanCancelChangesCmd);
            }
        }
        public bool CanSaveCmd
        {
            get
            {
                return !IsSaving && InfoHasChanged;
            }
        }
        public void SaveCmd()
        {
            IsSaving = true;
            try
            {
                var items = new Dictionary<PrinterType, string>();
                foreach (var assignment in AllPrinterTypePrinterAssignments)
                {
                    items.Add(assignment.PrinterType, assignment.AssignedPrinterName);
                }
                var printerConfigManager = new PrinterConfigurationManager();
                printerConfigManager.SavePrinterTypeAssignment(items);
                MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                //Luu xong thi doc lai tu config
                GetAllPrinterTypeAssignments();
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogInfo(ex.ToString());
            }
            finally
            {
                IsSaving = false;
            }
        }

        public bool CanCancelChangesCmd
        {
            get
            {
                return !IsSaving && InfoHasChanged;
            }
        }
        public void CancelChangesCmd()
        {
            CancelEdit();
            BeginEdit();
        }
        public void BeginEdit()
        {
            if(_allPrinterTypePrinterAssignments == null)
            {
                _tempAllPrinterTypePrinterAssignments = null;
            }

            _tempAllPrinterTypePrinterAssignments = _allPrinterTypePrinterAssignments.DeepCopy();
            InfoHasChanged = false;
        }
        public void CancelEdit()
        {
            AllPrinterTypePrinterAssignments = _tempAllPrinterTypePrinterAssignments;
            _tempAllPrinterTypePrinterAssignments = null;
            InfoHasChanged = false;
        }

        public void TestPrintCmd()
        {
            if (SelectedPrinterTypeAssigment == null)
            {
                MessageBox.Show(eHCMSResources.K0342_G1_ChonLoaiMayInCanInThu, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            switch (SelectedPrinterTypeAssigment.PrinterType)
            {
                case (PrinterType.IN_HOA_DON):
                    {
                        TestPrintReceiptReport();
                        break;
                    }
                case (PrinterType.IN_PHIEU):
                    {
                        TestPrintConfirmHIReport();
                        break;
                    }
                default:
                    {
                        MessageBox.Show(eHCMSResources.A0741_G1_Msg_InfoKhTimThayLoaiMayIn, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        break;
                    }
            }
        }

        public void TestPrintReceiptReport()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ReportServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetOutPatientReceiptTestPrintInPdfFormat(
                                Globals.DispatchCallback(asyncResult =>
                                {
                                    try
                                    {
                                        var data = contract.EndGetOutPatientReceiptTestPrintInPdfFormat(asyncResult);

                                        var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_HOA_DON, data, ActiveXPrintType.ByteArray);
                                        Globals.EventAggregator.Publish(printEvt);
                                    }
                                    catch (Exception ex)
                                    {
                                        ClientLoggerHelper.LogInfo(ex.ToString());
                                        MessageBox.Show(eHCMSResources.A0694_G1_Msg_InfoKhTheLayDataInHD);
                                    }
                                    finally
                                    {
                                        this.HideBusyIndicator();
                                    }
                                }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.HideBusyIndicator();
                }
            });
            t.Start();

        }


        private void TestPrintConfirmHIReport()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ReportServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetConfirmHITestPrintInPdfFormat(
                                Globals.DispatchCallback(asyncResult =>
                                {
                                    try
                                    {
                                        var data = contract.EndGetConfirmHITestPrintInPdfFormat(asyncResult);

                                        var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, data, ActiveXPrintType.ByteArray);
                                        Globals.EventAggregator.Publish(printEvt);
                                    }
                                    catch (Exception ex)
                                    {
                                        ClientLoggerHelper.LogInfo(ex.ToString());
                                        MessageBox.Show(eHCMSResources.A0695_G1_Msg_InfoKhTheLayDataDeInPhYC);
                                    }
                                    finally
                                    {
                                        this.HideBusyIndicator();
                                    }
                                }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
    }
}