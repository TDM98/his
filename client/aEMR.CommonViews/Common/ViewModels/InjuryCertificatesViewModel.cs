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
using aEMR.Infrastructure.Events.SL_Events;
/*
 * 20230311 #001 QTD:    Đổ lại dữ liệu quốc tịch
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IInjuryCertificates)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class InjuryCertificatesViewModel : Conductor<object>, IInjuryCertificates
    {
        public void SetCurrentInformation(InjuryCertificates item)
        {
            if (item != null)
            {
                CurrentInjuryCertificates = item;
                if (IsNew = CurrentInjuryCertificates.InjuryCertificateID == 0)
                {
                    if (IsOpenFromConsult)
                    {
                        if (PtRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
                        {
                            CurrentInjuryCertificates.ClinicalSigns = PtRegistration.DiagnosisTreatment.Diagnosis;
                            CurrentInjuryCertificates.Treatment = PtRegistration.DiagnosisTreatment.Treatment;
                            CurrentInjuryCertificates.Diagnosis = PtRegistration.DiagnosisTreatment.DiagnosisFinal;
                        }
                        else
                        {
                            GetRegistationForCirculars56(Convert.ToDateTime(PtRegistration.AdmissionInfo.AdmissionDate), Convert.ToDateTime(PtRegistration.AdmissionInfo.AdmissionDate),
                                (long)AllLookupValues.RegistrationType.NOI_TRU, PtRegistration.Patient.PatientCode);
                        }
                    }
                    else
                    {
                        CurrentInjuryCertificates.ClinicalSigns = PtRegistration.DiagnosisTreatment.Diagnosis;
                        CurrentInjuryCertificates.Treatment = PtRegistration.DiagnosisTreatment.TreatmentType;
                        CurrentInjuryCertificates.Diagnosis = PtRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU
                            ? PtRegistration.DiagnosisTreatment.DiagnosisFinal : PtRegistration.AdmissionInfo.DischargeNote;
                        CurrentInjuryCertificates.ReasonAdmission = PtRegistration.AdmissionInfo.AdmissionNote;
                        CurrentInjuryCertificates.DischargeStatus = PtRegistration.AdmissionInfo.DischargeStatus;
                        CurrentInjuryCertificates.AdmissionStatus = PtRegistration.DiagnosisTreatment.Diagnosis;
                        GetSummaryPCLResultByPtRegistrationID(PtRegistration.PtRegistrationID);
                    }
                    //CurrentInjuryCertificates.DoctorAdvice = PtRegistration.DiagnosisTreatment.DoctorComments;
                }
            }
            if (PtRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                TitleForm = eHCMSResources.Z3115_G1_ChungNhanTTNgT;
            }
            else
            {
                TitleForm = eHCMSResources.Z3115_G1_ChungNhanTTNT;
            }
        }

        private InjuryCertificates _CurrentInjuryCertificates = new InjuryCertificates();
        public InjuryCertificates CurrentInjuryCertificates
        {
            get
            {
                return _CurrentInjuryCertificates;
            }
            set
            {
                _CurrentInjuryCertificates = value;
                NotifyOfPropertyChange(() => CurrentInjuryCertificates);
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

        private string _TitleForm = "";
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
        private bool _IsApplyAutoCode = Globals.ServerConfigSection.CommonItems.ApplyAutoCodeForCirculars56;
        public bool IsApplyAutoCode
        {
            get
            {
                return _IsApplyAutoCode;
            }
            set
            {
                _IsApplyAutoCode = value;
                NotifyOfPropertyChange(() => IsApplyAutoCode);
            }
        }
        private bool _IsOpenFromConsult;
        public bool IsOpenFromConsult
        {
            get
            {
                return _IsOpenFromConsult;
            }
            set
            {
                _IsOpenFromConsult = value;
                NotifyOfPropertyChange(() => IsOpenFromConsult);
            }
        }
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public InjuryCertificatesViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            _eventArg = eventArg;

            LoadCountries();
            LoadEthnicsList();
            //▼====: #001
            Coroutine.BeginExecute(DoNationalityListList());
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
            if (CurrentInjuryCertificates.PtRegistrationID == 0)
            {
                CurrentInjuryCertificates.PtRegistrationID = PtRegistration.PtRegistrationID;
                CurrentInjuryCertificates.V_RegistrationType = (long)PtRegistration.V_RegistrationType;
            }
            CurrentInjuryCertificates.IsDelete = IsDelete;

        }

        public bool CheckValidBeforeSave()
        {
            return true;
        }
        private void SaveInjuryCertificates(bool IsDelete)
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
                    contract.BeginSaveInjuryCertificates(CurrentInjuryCertificates, (long)Globals.LoggedUserAccount.StaffID, Globals.GetCurServerDateTime().ToString("ddMMyyyy"), Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            InjuryCertificates result = contract.EndSaveInjuryCertificates(asyncResult);
                            if (result != null)
                            {
                                CurrentInjuryCertificates = result.DeepCopy();
                                //Globals.EventAggregator.Publish(CurrentInjuryCertificates.V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU ?
                                //                                   new Circulars56Event { SavedInjuryCertificates_NgT = CurrentInjuryCertificates } :
                                //                                   new Circulars56Event { SavedInjuryCertificates_NT = CurrentInjuryCertificates });
                                MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                            }
                            IsNew = CurrentInjuryCertificates.InjuryCertificateID == 0;
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
                    contract.BeginGetPCLResultForInjuryCertificatesByPtRegistrationID(PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string resultFromExamination;
                            string result = contract.EndGetPCLResultForInjuryCertificatesByPtRegistrationID(out resultFromExamination, asyncResult);
                            if(resultFromExamination!= null)
                            {
                                CurrentInjuryCertificates.ClinicalSigns = CurrentInjuryCertificates.ClinicalSigns + Environment.NewLine + resultFromExamination;
                            }
                            if (result != null)
                            {
                                CurrentInjuryCertificates.ClinicalSigns = CurrentInjuryCertificates.ClinicalSigns+Environment.NewLine+ result;
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
        public void SaveInjuryCertificatesCmd()
        {
            SaveInjuryCertificates(false);
        }

        public void DeleteInjuryCertificatesCmd()
        {
            if (MessageBox.Show(eHCMSResources.K0483_G1_BanCoChacChanMuonXoa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                SaveInjuryCertificates(true);
            }
        }

        public void PrintInjuryCertificatesCmd()
        {
            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
            {
                proAlloc.ID = (long)CurrentInjuryCertificates.InjuryCertificateID;
                proAlloc.V_RegistrationType = (long)CurrentInjuryCertificates.V_RegistrationType;
                proAlloc.eItem = ReportName.XRpt_ChungNhanThuongTich_NgoaiTru_NoiTru;
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
        public void GetRegistationForCirculars56(DateTime FromDate, DateTime ToDate, long PatientFindBy, string PatientCode)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginSearchRegistrationForCirculars56(FromDate, ToDate, PatientFindBy, PatientCode
                            , Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    ObservableCollection<PatientRegistration> ListTemp = new ObservableCollection<PatientRegistration>();
                                    List<PatientRegistration> allItems = client.EndSearchRegistrationForCirculars56(asyncResult);
                                    if (allItems != null && allItems.Count > 0)
                                    {
                                        foreach (var items in allItems)
                                        {
                                            ListTemp.Add(items);
                                        }
                                        PatientRegistration firstRegistration = ListTemp.FirstOrDefault();
                                        CurrentInjuryCertificates.ClinicalSigns = firstRegistration.DiagnosisTreatment.Diagnosis;
                                        CurrentInjuryCertificates.Treatment = firstRegistration.DiagnosisTreatment.TreatmentType;
                                        CurrentInjuryCertificates.Diagnosis = firstRegistration.AdmissionInfo.DischargeNote;
                                        CurrentInjuryCertificates.ReasonAdmission = firstRegistration.AdmissionInfo.AdmissionNote;
                                        CurrentInjuryCertificates.DischargeStatus = firstRegistration.AdmissionInfo.DischargeStatus;
                                        CurrentInjuryCertificates.AdmissionStatus = firstRegistration.DiagnosisTreatment.Diagnosis;
                                        GetSummaryPCLResultByPtRegistrationID(firstRegistration.PtRegistrationID);
                                    }
                                    else
                                    {
                                        MessageBox.Show(String.Format("{0}", eHCMSResources.A0638_G1_Msg_InfoKhCoData));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    this.HideBusyIndicator();
                                    MessageBox.Show(ex.Message);
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
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
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

        private IEnumerator<IResult> DoNationalityListList()
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