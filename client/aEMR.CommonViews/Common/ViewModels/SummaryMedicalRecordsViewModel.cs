using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using DataEntities;
using System.Linq;
using System.ComponentModel;
using eHCMSLanguage;
using aEMR.Common.Collections;
using aEMR.ViewContracts;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.CommonTasks;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using static aEMR.Infrastructure.Events.TransferFormEvent;
using aEMR.Infrastructure.Events.SL_Events;
/*
 * 20230311 #001 QTD:    Đổ lại dữ liệu quốc tịch
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(ISummaryMedicalRecords)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class SummaryMedicalRecordsViewModel : Conductor<object>, ISummaryMedicalRecords
    {

        public void SetCurrentInformation(SummaryMedicalRecords item)
        {
            if (item != null)
            {
                CurrentSummaryMedicalRecords = item;
                //IsNew = CurrentSummaryMedicalRecords.SummaryMedicalRecordID == 0;
                if(IsNew = CurrentSummaryMedicalRecords.SummaryMedicalRecordID == 0)
                {
                //    CurrentSummaryMedicalRecords.AdmissionDiagnosis = PtRegistration.DiagnosisTreatment.DiagnosisFinal;
                //    CurrentSummaryMedicalRecords.DischargeDiagnosis = PtRegistration.AdmissionInfo.DischargeNote;
                //    CurrentSummaryMedicalRecords.Treatment = PtRegistration.DiagnosisTreatment.Treatment;
                //    CurrentSummaryMedicalRecords.DischargeStatus = PtRegistration.AdmissionInfo.DischargeStatus;
                //    CurrentSummaryMedicalRecords.Note = PtRegistration.AdmissionInfo.VDischargeType.ObjectValue;
                //    CurrentSummaryMedicalRecords.PathologicalProcess = PtRegistration.DiagnosisTreatment.OrientedTreatment;
                    GetSummaryPCLResultByPtRegistrationID(PtRegistration.PtRegistrationID);
                }

            }
        }

        private SummaryMedicalRecords _CurrentSummaryMedicalRecords = new SummaryMedicalRecords();
        public SummaryMedicalRecords CurrentSummaryMedicalRecords
        {
            get
            {
                return _CurrentSummaryMedicalRecords;
            }
            set
            {
                _CurrentSummaryMedicalRecords = value;
                NotifyOfPropertyChange(() => CurrentSummaryMedicalRecords);
            }
        }

        private PatientRegistration _PtRegistration;
        public PatientRegistration PtRegistration
        {
            get
            {
                return _PtRegistration;
            }
            set
            {
                _PtRegistration = value;
                NotifyOfPropertyChange(() => _PtRegistration);
                NotifyOfPropertyChange(() => PtRegistration);
            }
        }

        private string _TitleForm;
        public string TitleForm
        {
            get
            {
                return _TitleForm;
            }
            set
            {
                _TitleForm = value;
                NotifyOfPropertyChange(() => TitleForm);
            }
        }
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public SummaryMedicalRecordsViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            _eventArg = eventArg;
            TitleForm = eHCMSResources.Z3115_G1_TomTatHSBA;
            LoadCountries();
            LoadEthnicsList();
            //▼====: #001
            Coroutine.BeginExecute(DoNationalityList());
            //▲====: #001
        }

        private ObservableCollection<RefCountry> _countries;
        public ObservableCollection<RefCountry> Countries
        {
            get { return _countries; }
            set
            {
                _countries = value;
                NotifyOfPropertyChange(() => Countries);
            }
        }

        private ObservableCollection<Lookup> _ethnicsList;
        public ObservableCollection<Lookup> EthnicsList
        {
            get { return _ethnicsList; }
            set
            {
                _ethnicsList = value;
                NotifyOfPropertyChange(() => EthnicsList);
            }
        }

        public void LoadEthnicsList()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllLookupValuesByType(LookupValues.ETHNIC,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    var allItems = contract.EndGetAllLookupValuesByType(asyncResult);

                                    EthnicsList = allItems != null ? new ObservableCollection<Lookup>(allItems) : null;
                                }
                                catch (Exception ex1)
                                {
                                    ClientLoggerHelper.LogInfo(ex1.ToString());
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }

            });
            t.Start();
        }

        public void LoadCountries()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllCountries(Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var allItems = contract.EndGetAllCountries(asyncResult);

                                Countries = allItems != null ? new ObservableCollection<RefCountry>(allItems) : null;
                            }
                            catch (Exception ex1)
                            {
                                ClientLoggerHelper.LogInfo(ex1.ToString());
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }


        public void InitValueBeforeSave(bool IsDelete)
        {
            if (CurrentSummaryMedicalRecords.PtRegistrationID == 0)
            {
                CurrentSummaryMedicalRecords.PtRegistrationID = PtRegistration.PtRegistrationID;
                CurrentSummaryMedicalRecords.V_RegistrationType = (long)PtRegistration.V_RegistrationType;
            }
            CurrentSummaryMedicalRecords.IsDelete = IsDelete;
        }

        public bool CheckValidBeforeSave()
        {
            return true;
        }
        private void SaveSummaryMedicalRecords(bool IsDelete)
        {
            if (CheckValidBeforeSave() == false)
            {
                return;
            }
            InitValueBeforeSave(IsDelete);
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PatientRegistrationServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSaveSummaryMedicalRecords(CurrentSummaryMedicalRecords,(long)Globals.LoggedUserAccount.StaffID, Globals.GetCurServerDateTime().ToString("ddMMyyyy"), Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            SummaryMedicalRecords result = contract.EndSaveSummaryMedicalRecords(asyncResult);
                            if (result != null)
                            {
                                CurrentSummaryMedicalRecords = result.DeepCopy();
                                //Globals.EventAggregator.Publish(new Circulars56Event { SavedSummaryMedicalRecords = CurrentSummaryMedicalRecords });
                                MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                            }
                            IsNew = CurrentSummaryMedicalRecords.SummaryMedicalRecordID == 0;
                            this.HideBusyIndicator();
                            if (IsDelete && result == null)
                            {
                                MessageBox.Show(eHCMSResources.K0537_G1_XoaOk);
                                TryClose();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            this.HideBusyIndicator();
                        }

                    }), null);
                }
            });
            t.Start();
        }
        private void GetSummaryPCLResultByPtRegistrationID(long PtRegistrationID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PatientRegistrationServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetSummaryPCLResultByPtRegistrationID(PtRegistrationID, (long)AllLookupValues.RegistrationType.NOI_TRU, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string result = contract.EndGetSummaryPCLResultByPtRegistrationID(asyncResult);
                            if (result != null)
                            {
                                CurrentSummaryMedicalRecords.SummaryResultPCL = result;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            this.HideBusyIndicator();
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            t.Start();
        }
        public void SaveSummaryMedicalRecordsCmd()
        {
            SaveSummaryMedicalRecords(false);
        }

        public void DeleteSummaryMedicalRecordsCmd()
        {
            if (MessageBox.Show(eHCMSResources.K0483_G1_BanCoChacChanMuonXoa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                SaveSummaryMedicalRecords(true);
            }
        }

        public void PrintSummaryMedicalRecordsCmd()
        {
            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
            {
                proAlloc.ID = (long)CurrentSummaryMedicalRecords.SummaryMedicalRecordID;
                proAlloc.V_RegistrationType = (long)CurrentSummaryMedicalRecords.V_RegistrationType;
                proAlloc.eItem = ReportName.XRpt_TomTatHoSoBenhAn;
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        }

        protected override void OnActivate()
        {
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            _eventArg.Unsubscribe(this);
        }

        private bool _IsNew = true;
        public bool IsNew
        {
            get
            {
                return _IsNew;
            }
            set
            {
                if (_IsNew == value) return;
                _IsNew = value;
                NotifyOfPropertyChange(() => IsNew);
            }
        }

        //▼====: #001
        private ObservableCollection<RefNationality> _Nationalities;
        public ObservableCollection<RefNationality> Nationalities
        {
            get { return _Nationalities; }
            set
            {
                _Nationalities = value;
                NotifyOfPropertyChange(() => Nationalities);
            }
        }

        private IEnumerator<IResult> DoNationalityList()
        {
            if (Globals.allNationalities != null)
            {
                Nationalities = Globals.allNationalities.ToObservableCollection();
                yield break;
            }
            var paymentTask = new LoadNationalityListTask(false, false);
            yield return paymentTask;
            Nationalities = paymentTask.RefNationalityList;
            yield break;
        }
        //▲====: #001
    }
}