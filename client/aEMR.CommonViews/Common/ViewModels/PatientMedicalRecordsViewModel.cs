using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
using Castle.Windsor;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.BaseModel;
using aEMR.Infrastructure;
using eHCMSLanguage;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IPatientMedicalRecords)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientMedicalRecordsViewModel : ViewModelBase, IPatientMedicalRecords
    {

        public override bool IsProcessing
        {
            get
            {
                return _IsLoadingGetPMRsByPtIDFinish
                    || _IsLoadingGetPMRsByPtIDCurrent
                    || _IsLoadingSave;
            }
        }

        public override string StatusText
        {
            get
            {
                if (_IsLoadingGetPMRsByPtIDFinish)
                {
                    return string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z1241_G1_LoadDSHSoCu);
                }
                if (_IsLoadingGetPMRsByPtIDCurrent)
                {
                    return string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.T1563_G1_HSoBAnHTai);
                }
                if (_IsLoadingSave)
                {
                    return string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0343_G1_DangLuu);
                }
                return string.Empty;
            }
        }

        private bool _IsLoadingGetPMRsByPtIDFinish;
        public bool IsLoadingGetPMRsByPtIDFinish
        {
            get { return _IsLoadingGetPMRsByPtIDFinish; }
            set
            {
                if (_IsLoadingGetPMRsByPtIDFinish != value)
                {
                    _IsLoadingGetPMRsByPtIDFinish = value;
                    NotifyOfPropertyChange(() => IsLoadingGetPMRsByPtIDFinish);
                    NotifyWhenBusy();
                }
            }
        }

        private bool _IsLoadingGetPMRsByPtIDCurrent;
        public bool IsLoadingGetPMRsByPtIDCurrent
        {
            get { return _IsLoadingGetPMRsByPtIDCurrent; }
            set
            {
                if (_IsLoadingGetPMRsByPtIDCurrent != value)
                {
                    _IsLoadingGetPMRsByPtIDCurrent = value;
                    NotifyOfPropertyChange(() => IsLoadingGetPMRsByPtIDCurrent);
                    NotifyWhenBusy();
                }
            }
        }


        private bool _IsLoadingSave;
        public bool IsLoadingSave
        {
            get { return _IsLoadingSave; }
            set
            {
                if (_IsLoadingSave != value)
                {
                    _IsLoadingSave = value;
                    NotifyOfPropertyChange(() => IsLoadingSave);
                    NotifyWhenBusy();
                }
            }
        }

        private ObservableCollection<PatientMedicalRecord> _ListPatientMedicalRecordFinish;
        public ObservableCollection<PatientMedicalRecord> ListPatientMedicalRecordFinish
        {
            get { return _ListPatientMedicalRecordFinish; }
            set
            {
                _ListPatientMedicalRecordFinish = value;
                NotifyOfPropertyChange(() => ListPatientMedicalRecordFinish);
            }
        }

        private PatientMedicalRecord _PatientMedicalRecordCurrent;
        public PatientMedicalRecord PatientMedicalRecordCurrent
        {
            get { return _PatientMedicalRecordCurrent; }
            set
            {
                _PatientMedicalRecordCurrent = value;
                NotifyOfPropertyChange(() => PatientMedicalRecordCurrent);
            }
        }

        private void GetPMRsByPtIDFinish(long? patientID)
        {
            IsLoadingGetPMRsByPtIDFinish = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetPMRsByPtID(patientID, 2, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndGetPMRsByPtID(asyncResult);

                            if (items != null)
                            {
                                ListPatientMedicalRecordFinish = new ObservableCollection<DataEntities.PatientMedicalRecord>(items);
                            }
                            else
                            {
                                ListPatientMedicalRecordFinish = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoadingGetPMRsByPtIDFinish = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        private void GetPMRsByPtIDCurrent(long? patientID)
        {
            IsLoadingGetPMRsByPtIDCurrent = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetPMRsByPtID(patientID, 1, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndGetPMRsByPtID(asyncResult);

                            if (items != null)
                            {
                                if (items.Count > 0)
                                {
                                    PatientMedicalRecordCurrent = items[0];
                                }
                                else
                                {
                                    PatientMedicalRecordCurrent = new PatientMedicalRecord();
                                }
                            }
                            else
                            {
                                PatientMedicalRecordCurrent = new PatientMedicalRecord();
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoadingGetPMRsByPtIDCurrent = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        private Patient _ObjPatient;
        public Patient ObjPatient
        {
            get { return _ObjPatient; }
            set
            {
                _ObjPatient = value;
            }
        }
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public PatientMedicalRecordsViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            PatientMedicalRecordCurrent = new PatientMedicalRecord();
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            GetPMRsByPtIDCurrent(ObjPatient.PatientID);
            GetPMRsByPtIDFinish(ObjPatient.PatientID);
        }

        public void CancelCmd()
        {
            TryClose();
        }

        public void cmdSave()
        {
            if (PatientMedicalRecordCurrent==null)
            {
                PatientMedicalRecordCurrent=new PatientMedicalRecord();
            }

            if (string.IsNullOrEmpty(PatientMedicalRecordCurrent.NationalMedicalCode))
            {
                MessageBox.Show(eHCMSResources.A0878_G1_Msg_InfoNhapSoHS);
                return;
            }

            PatientMedicalRecords_Save(PatientMedicalRecordCurrent.PatientRecID, ObjPatient.PatientID, PatientMedicalRecordCurrent.NationalMedicalCode);
        }


        private void PatientMedicalRecords_Save(long PatientRecID, long PatientID, string NationalMedicalCode)
        {
            IsLoadingSave = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPatientMedicalRecords_Save(PatientRecID,PatientID,NationalMedicalCode, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string msg = "";
                            var b = contract.EndPatientMedicalRecords_Save(out msg, asyncResult);
                            MessageBox.Show(msg);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoadingSave = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        
    }
}
