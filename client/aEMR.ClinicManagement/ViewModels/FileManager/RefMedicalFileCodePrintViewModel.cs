using System.ComponentModel.Composition;
using Caliburn.Micro;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using System.Collections.Generic;
using DataEntities;
using System.Threading;
using aEMR.ServiceClient;
using System;
using System.Windows;
using eHCMSLanguage;
using aEMR.Common;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.ClinicManagement.ViewModels
{
    [Export(typeof(IRefMedicalFileCodePrint)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RefMedicalFileCodePrintViewModel : Conductor<object>, IRefMedicalFileCodePrint
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public RefMedicalFileCodePrintViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

        }

        TextBox txtFileCode;
        private string _FileCodeNumber;
        public string FileCodeNumber
        {
            get
            {
                return _FileCodeNumber;
            }
            set
            {
                if (_FileCodeNumber != value)
                {
                    _FileCodeNumber = value;
                    NotifyOfPropertyChange(() => FileCodeNumber);
                }
            }
        }
        private long _PatientMedicalFileID;
        public long PatientMedicalFileID
        {
            get
            {
                return _PatientMedicalFileID;
            }
            set
            {
                if (_PatientMedicalFileID != value)
                {
                    _PatientMedicalFileID = value;
                    NotifyOfPropertyChange(() => PatientMedicalFileID);
                }
            }
        }
        private ObservableCollection<PatientMedicalFileStorage> _AllPatientMedicalFileStorage = new ObservableCollection<PatientMedicalFileStorage>();
        public ObservableCollection<PatientMedicalFileStorage> AllPatientMedicalFileStorage
        {
            get
            {
                return _AllPatientMedicalFileStorage;
            }
            set
            {
                if (_AllPatientMedicalFileStorage != value)
                {
                    _AllPatientMedicalFileStorage = value;
                    NotifyOfPropertyChange(() => AllPatientMedicalFileStorage);
                }
            }
        }
        public void txtFileCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.ShowBusyIndicator();
                FileCodeNumber = txtFileCode.Text;
                var t = new Thread(() =>
                {
                    try
                    {
                        using (var serviceFactory = new ClinicManagementServiceClient())
                        {
                            var contract = serviceFactory.ServiceInstance;
                            contract.BeginGetPatientMedicalFileStorage(0, FileCodeNumber,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                if (string.IsNullOrEmpty(FileCodeNumber))
                                {
                                    this.HideBusyIndicator();
                                    FocusFileCode();
                                    return;
                                }
                                if (AllPatientMedicalFileStorage.Any(x => x.FileCodeNumber == FileCodeNumber))
                                {
                                    MessageBox.Show(eHCMSResources.Z1957_G1_HSDaTonTai, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                    this.HideBusyIndicator();
                                    FocusFileCode();
                                    return;
                                }
                                long PatientMedicalFileID;
                                var GettedPatientMedicalFileStorage = contract.EndGetPatientMedicalFileStorage(out PatientMedicalFileID, asyncResult);
                                this.PatientMedicalFileID = PatientMedicalFileID;
                                if (PatientMedicalFileID == 0)
                                {
                                    MessageBox.Show(eHCMSResources.Z1958_G1_SoHSKgHopLe, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                    this.HideBusyIndicator();
                                    FocusFileCode();
                                    return;
                                }
                                if (GettedPatientMedicalFileStorage.Count == 1)
                                    AllPatientMedicalFileStorage.Add(GettedPatientMedicalFileStorage.First().DeepCopy());
                                else
                                    MessageBox.Show(eHCMSResources.Z1982_G1_KgTimThayHS, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                this.HideBusyIndicator();
                                FocusFileCode();
                            }), null);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                        this.HideBusyIndicator();
                    }
                });
                t.Start();
            }
        }
        public void txtFileCode_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                txtFileCode = sender as TextBox;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
            }
        }
        public void btnPrint()
        {
            //var t = new Thread(() =>
            //{
            //    try
            //    {
            //        using (var serviceFactory = new ClinicManagementServiceClient())
            //        {
            //            var contract = serviceFactory.ServiceInstance;
            //            contract.BeginGetXMLPatientMedicalFileStorages(AllPatientMedicalFileStorage.ToList(),
            //            Globals.DispatchCallback((asyncResult) =>
            //            {
            //                if (string.IsNullOrEmpty(FileCodeNumber))
            //                {
            //                    this.HideBusyIndicator();
            //                    FocusFileCode();
            //                    return;
            //                }
            //                var XMLPatientMedicalFileStorages = contract.EndGetXMLPatientMedicalFileStorages(asyncResult);
            //                this.HideBusyIndicator();
            //                FocusFileCode();
            //                var proAlloc = Globals.GetViewModel<ICommonPreviewView>();
            //                proAlloc.eItem = ReportName.MEDICALFILECODEPRINT;
            //                proAlloc.XMLPatientMedicalFileStorages = XMLPatientMedicalFileStorages;
            //                var instance = proAlloc as Conductor<object>;
            //                Globals.ShowDialog(instance, (o) => { });
            //            }), null);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
            //        this.HideBusyIndicator();
            //    }
            //});
            //t.Start();
        }
        private void FocusFileCode()
        {
            txtFileCode.Text = "";
            txtFileCode.Focus();
        }
    }
}
