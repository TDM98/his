using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using Castle.Windsor;
using aEMR.Common.BaseModel;
using System.Linq;
using aEMR.Controls;
using aEMR.ServiceClient.Consultation_PCLs;
using System.Windows.Media;
using System.Windows.Data;
using aEMR.Infrastructure.Events.SL_Events;
// 20210730: BAOLQ VacationInsuranceCertificate sử dụng cho giấy chứng nhận nghỉ việc và chứng nhận nghỉ dưỡng thai phân biệt bằng V_RegistrationType
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(ICirculars56)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class Circulars56ViewModel : ViewModelBase, ICirculars56
        , IHandle<Circulars56Event>
    {
        [ImportingConstructor]
        public Circulars56ViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            curObjPatientRegistration = new ObservableCollection<PatientRegistration>();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
        }

        public override void DeactivateItem(object item, bool close)
        {
            base.DeactivateItem(item, close);
            Globals.EventAggregator.Unsubscribe(this);
            var homevm = Globals.GetViewModel<IHome>();
            homevm.OutstandingTaskContent = null;
            homevm.IsExpandOST = false;
        }

        #region Properties
        AxAutoComplete AcbDoctorStaff { get; set; }
        AutoCompleteBox cboContext { get; set; }
        private int _CountPatientRegistration = 0;
        public int CountPatientRegistration
        {
            get
            {
                return _CountPatientRegistration;
            }
            set
            {
                if (_CountPatientRegistration != value)
                {
                    _CountPatientRegistration = value;
                    NotifyOfPropertyChange(() => CountPatientRegistration);
                }
            }
        }
        private string _FullName = "";
        public string FullName
        {
            get
            {
                return _FullName;
            }
            set
            {
                if (_FullName != value)
                {
                    _FullName = value;
                    NotifyOfPropertyChange(() => FullName);
                }
            }
        }
        private string _GenderString = "";
        public string GenderString
        {
            get
            {
                return _GenderString;
            }
            set
            {
                if (_GenderString != value)
                {
                    _GenderString = value;
                    NotifyOfPropertyChange(() => GenderString);
                }
            }
        }
        private string _Age = "";
        public string Age
        {
            get
            {
                return _Age;
            }
            set
            {
                if (_Age != value)
                {
                    _Age = value;
                    NotifyOfPropertyChange(() => Age);
                }
            }
        }

        private string _Weight = "";
        public string Weight
        {
            get
            {
                return _Weight;
            }
            set
            {
                if (_Weight != value)
                {
                    _Weight = value;
                    NotifyOfPropertyChange(() => Weight);
                }
            }
        }

        private string _Diagnosis = "";
        public string Diagnosis
        {
            get
            {
                return _Diagnosis;
            }
            set
            {
                if (_Diagnosis != value)
                {
                    _Diagnosis = value;
                    NotifyOfPropertyChange(() => Diagnosis);
                }
            }
        }
        private string _DiagnosisSub = "";
        public string DiagnosisSub
        {
            get
            {
                return _DiagnosisSub;
            }
            set
            {
                if (_DiagnosisSub != value)
                {
                    _DiagnosisSub = value;
                    NotifyOfPropertyChange(() => DiagnosisSub);
                }
            }
        }
        private string _ListICD10 = "";
        public string ListICD10
        {
            get
            {
                return _ListICD10;
            }
            set
            {
                if (_ListICD10 != value)
                {
                    _ListICD10 = value;
                    NotifyOfPropertyChange(() => ListICD10);
                }
            }
        }
        private ObservableCollection<PatientRegistration> _curObjPatientRegistration;
        public ObservableCollection<PatientRegistration> curObjPatientRegistration
        {
            get
            {
                return _curObjPatientRegistration;
            }
            set
            {
                if (_curObjPatientRegistration != value)
                {
                    _curObjPatientRegistration = value;
                    NotifyOfPropertyChange(() => curObjPatientRegistration);
                }
            }
        }

        private DateTime _FromDate = Globals.GetCurServerDateTime();
        public DateTime FromDate
        {
            get
            {
                return _FromDate;
            }
            set
            {
                if (_FromDate != value)
                {
                    _FromDate = value;
                    NotifyOfPropertyChange(() => FromDate);
                }
            }
        }

        private DateTime _ToDate = Globals.GetCurServerDateTime();
        public DateTime ToDate
        {
            get
            {
                return _ToDate;
            }
            set
            {
                if (_ToDate != value)
                {
                    _ToDate = value;
                    NotifyOfPropertyChange(() => ToDate);
                }
            }
        }
        private int _PatientTypeIndex = 0;
        public int PatientTypeIndex
        {
            get
            {
                return _PatientTypeIndex;
            }
            set
            {
                if (_PatientTypeIndex != value)
                {
                    _PatientTypeIndex = value;
                    NotifyOfPropertyChange(() => PatientTypeIndex);
                }
            }
        }
        private int _LastCodeXNDT = 0;
        public int LastCodeXNDT
        {
            get
            {
                return _LastCodeXNDT;
            }
            set
            {
                if (_LastCodeXNDT != value)
                {
                    _LastCodeXNDT = value;
                    NotifyOfPropertyChange(() => LastCodeXNDT);
                }
            }
        }
        private int _LastCodeCNTT = 0;
        public int LastCodeCNTT
        {
            get
            {
                return _LastCodeCNTT;
            }
            set
            {
                if (_LastCodeCNTT != value)
                {
                    _LastCodeCNTT = value;
                    NotifyOfPropertyChange(() => LastCodeCNTT);
                }
            }
        }
        private int _LastCodeGCS = 0;
        public int LastCodeGCS
        {
            get
            {
                return _LastCodeGCS;
            }
            set
            {
                if (_LastCodeGCS != value)
                {
                    _LastCodeGCS = value;
                    NotifyOfPropertyChange(() => LastCodeGCS);
                }
            }
        }

        private bool _IsHaveHSBA;
        public bool IsHaveHSBA
        {
            get
            {
                return _IsHaveHSBA;
            }
            set
            {
                if (_IsHaveHSBA != value)
                {
                    _IsHaveHSBA = value;
                    NotifyOfPropertyChange(() => IsHaveHSBA);
                }
            }
        }
        private bool _IsHaveChungNhanNDT;
        public bool IsHaveChungNhanNDT
        {
            get
            {
                return _IsHaveChungNhanNDT;
            }
            set
            {
                if (_IsHaveChungNhanNDT != value)
                {
                    _IsHaveChungNhanNDT = value;
                    NotifyOfPropertyChange(() => IsHaveChungNhanNDT);
                }
            }
        }
        private bool _IsHaveChungNhanNVHBH;
        public bool IsHaveChungNhanNVHBH
        {
            get
            {
                return _IsHaveChungNhanNVHBH;
            }
            set
            {
                if (_IsHaveChungNhanNVHBH != value)
                {
                    _IsHaveChungNhanNVHBH = value;
                    NotifyOfPropertyChange(() => IsHaveChungNhanNVHBH);
                }
            }
        }
        private bool _IsHaveChungNhanNVKHBH;
        public bool IsHaveChungNhanNVKHBH
        {
            get
            {
                return _IsHaveChungNhanNVKHBH;
            }
            set
            {
                if (_IsHaveChungNhanNVKHBH != value)
                {
                    _IsHaveChungNhanNVKHBH = value;
                    NotifyOfPropertyChange(() => IsHaveChungNhanNVKHBH);
                }
            }
        }
        private bool _IsHaveGiayBaoTu;
        public bool IsHaveGiayBaoTu
        {
            get
            {
                return _IsHaveGiayBaoTu;
            }
            set
            {
                if (_IsHaveGiayBaoTu != value)
                {
                    _IsHaveGiayBaoTu = value;
                    NotifyOfPropertyChange(() => IsHaveGiayBaoTu);
                }
            }
        }
        private bool _IsHaveXacNhanDTNgT;
        public bool IsHaveXacNhanDTNgT
        {
            get
            {
                return _IsHaveXacNhanDTNgT;
            }
            set
            {
                if (_IsHaveXacNhanDTNgT != value)
                {
                    _IsHaveXacNhanDTNgT = value;
                    NotifyOfPropertyChange(() => IsHaveXacNhanDTNgT);
                }
            }
        }
        private bool _IsHaveXacNhanDTNT;
        public bool IsHaveXacNhanDTNT
        {
            get
            {
                return _IsHaveXacNhanDTNT;
            }
            set
            {
                if (_IsHaveXacNhanDTNT != value)
                {
                    _IsHaveXacNhanDTNT = value;
                    NotifyOfPropertyChange(() => IsHaveXacNhanDTNT);
                }
            }
        }
        private bool _IsHaveChungNhanTTNgT;
        public bool IsHaveChungNhanTTNgT
        {
            get
            {
                return _IsHaveChungNhanTTNgT;
            }
            set
            {
                if (_IsHaveChungNhanTTNgT != value)
                {
                    _IsHaveChungNhanTTNgT = value;
                    NotifyOfPropertyChange(() => IsHaveChungNhanTTNgT);
                }
            }
        }
        private bool _IsHaveChungNhanTTNT;
        public bool IsHaveChungNhanTTNT
        {
            get
            {
                return _IsHaveChungNhanTTNT;
            }
            set
            {
                if (_IsHaveChungNhanTTNT != value)
                {
                    _IsHaveChungNhanTTNT = value;
                    NotifyOfPropertyChange(() => IsHaveChungNhanTTNT);
                }
            }
        }
        private bool _IsHaveGiayChungSinh;
        public bool IsHaveGiayChungSinh
        {
            get
            {
                return _IsHaveGiayChungSinh;
            }
            set
            {
                if (_IsHaveGiayChungSinh != value)
                {
                    _IsHaveGiayChungSinh = value;
                    NotifyOfPropertyChange(() => IsHaveGiayChungSinh);
                }
            }
        }
        private string _PatientCode;
        public string PatientCode
        {
            get
            {
                return _PatientCode;
            }
            set
            {
                if (_PatientCode != value)
                {
                    _PatientCode = value;
                    NotifyOfPropertyChange(() => PatientCode);
                }
            }
        }
        #endregion

        #region Method

        public void btnFindRegistration()
        {
            GetRegistationForCirculars56(FromDate, ToDate, PatientTypeIndex, PatientCode);
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
                                curObjPatientRegistration = new ObservableCollection<PatientRegistration>();
                                List<PatientRegistration> allItems = client.EndSearchRegistrationForCirculars56(asyncResult);
                                if (allItems != null && allItems.Count > 0)
                                {
                                    foreach (var items in allItems)
                                    {
                                        curObjPatientRegistration.Add(items);
                                    }
                                    CountPatientRegistration = allItems.Count();
                                }
                                else
                                {
                                    MessageBox.Show(String.Format("{0}", eHCMSResources.A0638_G1_Msg_InfoKhCoData));
                                    CountPatientRegistration = 0;
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

        public void DoubleClick(object source, EventArgs<object> eventArgs)
        {
            if (source == null || eventArgs == null)
            {
                return;
            }
            PatientRegistration SelectedPatientRegistation = eventArgs.Value as PatientRegistration;
            if (SelectedPatientRegistation != null && SelectedPatientRegistation.DiagnosisTreatment != null && !string.IsNullOrEmpty(SelectedPatientRegistation.Patient.FullName)
                && !string.IsNullOrEmpty(SelectedPatientRegistation.DiagnosisTreatment.DiagnosisFinal) && !string.IsNullOrEmpty(SelectedPatientRegistation.DiagnosisTreatment.ICD10List))
            {
                CurPatientRegistration = SelectedPatientRegistation;
                FullName = SelectedPatientRegistation.Patient.FullName;
                GenderString = SelectedPatientRegistation.Patient.Gender;
                Age = SelectedPatientRegistation.Patient.AgeString.ToString();
                GetPhyExam_ByPtRegID(SelectedPatientRegistation.PtRegistrationID, (long)SelectedPatientRegistation.V_RegistrationType);
                GetSummaryMedicalRecords_ByPtRegID(SelectedPatientRegistation.PtRegistrationID, (long)SelectedPatientRegistation.V_RegistrationType);
                GetPatientTreatmentCertificates_ByPtRegID(SelectedPatientRegistation.PtRegistrationID, (long)SelectedPatientRegistation.V_RegistrationType);
                GetInjuryCertificates_ByPtRegID(SelectedPatientRegistation.PtRegistrationID, (long)SelectedPatientRegistation.V_RegistrationType);
                //GetBirthCertificates_ByPtRegID(SelectedPatientRegistation.PtRegistrationID, (long)SelectedPatientRegistation.V_RegistrationType);
                GetVacationInsuranceCertificates_ByPtRegID(SelectedPatientRegistation.PtRegistrationID);
                GetVacationPrenatalCertificates_ByPtRegID(SelectedPatientRegistation.PtRegistrationID);
                if (SelectedPatientRegistation.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
                {
                    GetListDiagnosisTreatment(SelectedPatientRegistation.DiagnosisTreatment.ServiceRecID, SelectedPatientRegistation.Patient.PatientID);
                }
                else
                {
                    GetListDiagnosisTreatment_InPT(SelectedPatientRegistation.DiagnosisTreatment.DTItemID);
                }
                if (SelectedPatientRegistation.AdmissionInfo.V_DischargeCondition != null && SelectedPatientRegistation.AdmissionInfo.V_DischargeCondition.LookupID == (long)AllLookupValues.DischargeCondition.TU_VONG)
                {
                    IsHaveGiayBaoTu = true;
                }
                else
                {
                    IsHaveGiayBaoTu = false;
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.Z1688_G1_TTinKgHopLe);
            }
        }
        private void GetSummaryMedicalRecords_ByPtRegID(long PtRegistrationID, long V_RegistrationType)
        {
            //this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetSummaryMedicalRecords_ByPtRegID(PtRegistrationID, V_RegistrationType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                SummaryMedicalRecords result = contract.EndGetSummaryMedicalRecords_ByPtRegID(asyncResult);
                                CurSummaryMedicalRecords = result ?? new SummaryMedicalRecords();
                                IsHaveHSBA = CurSummaryMedicalRecords.SummaryMedicalRecordID > 0;
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
        private void GetPatientTreatmentCertificates_ByPtRegID(long PtRegistrationID, long V_RegistrationType)
        {
            //this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetPatientTreatmentCertificates_ByPtRegID(PtRegistrationID, V_RegistrationType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int tempCode;
                                PatientTreatmentCertificates result = contract.EndGetPatientTreatmentCertificates_ByPtRegID(out tempCode, asyncResult);
                                if (V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU)
                                {
                                    CurPatientTreatmentCertificates_NgT = result ?? new PatientTreatmentCertificates();
                                    CurPatientTreatmentCertificates_NT = new PatientTreatmentCertificates();
                                }
                                else
                                {
                                    CurPatientTreatmentCertificates_NT = result ?? new PatientTreatmentCertificates();
                                    CurPatientTreatmentCertificates_NgT = new PatientTreatmentCertificates();
                                }
                                IsHaveXacNhanDTNgT = CurPatientTreatmentCertificates_NgT.PatientTreatmentCertificateID > 0;
                                IsHaveXacNhanDTNT = CurPatientTreatmentCertificates_NT.PatientTreatmentCertificateID > 0;
                                if (Globals.ServerConfigSection.CommonItems.ApplyAutoCodeForCirculars56)
                                {
                                    LastCodeXNDT = tempCode;
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                //this.HideBusyIndicator();
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
        private void GetInjuryCertificates_ByPtRegID(long PtRegistrationID, long V_RegistrationType)
        {
            //this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetInjuryCertificates_ByPtRegID(PtRegistrationID, V_RegistrationType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int tempCode;
                                InjuryCertificates result = contract.EndGetInjuryCertificates_ByPtRegID(out tempCode, asyncResult);
                                if (V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU)
                                {
                                    CurInjuryCertificates_NgT = result ?? new InjuryCertificates();
                                    CurInjuryCertificates_NT = new InjuryCertificates();
                                }
                                else
                                {
                                    CurInjuryCertificates_NT = result ?? new InjuryCertificates();
                                    CurInjuryCertificates_NgT = new InjuryCertificates();
                                }
                                IsHaveChungNhanTTNgT = CurInjuryCertificates_NgT.InjuryCertificateID > 0;
                                IsHaveChungNhanTTNT = CurInjuryCertificates_NT.InjuryCertificateID > 0;
                                if (Globals.ServerConfigSection.CommonItems.ApplyAutoCodeForCirculars56)
                                {
                                    LastCodeCNTT = tempCode;
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                //this.HideBusyIndicator();
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
        //private void GetBirthCertificates_ByPtRegID(long PtRegistrationID, long V_RegistrationType)
        //{
        //    //this.ShowBusyIndicator();
        //    var t = new Thread(() =>
        //    {
        //        try
        //        {
        //            using (var serviceFactory = new PatientRegistrationServiceClient())
        //            {
        //                var contract = serviceFactory.ServiceInstance;
        //                contract.BeginGetBirthCertificates_ByPtRegID(PtRegistrationID, V_RegistrationType, Globals.DispatchCallback((asyncResult) =>
        //                {
        //                    try
        //                    {
        //                        int tempCode;
        //                        BirthCertificates result = contract.EndGetBirthCertificates_ByPtRegID(out tempCode, asyncResult);
        //                        CurBirthCertificates = result ?? new BirthCertificates();
        //                        IsHaveGiayChungSinh = CurBirthCertificates.BirthCertificateID > 0;
        //                        if (Globals.ServerConfigSection.CommonItems.ApplyAutoCodeForCirculars56)
        //                        {
        //                            LastCodeGCS = tempCode;
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                    }
        //                    finally
        //                    {
        //                        //this.HideBusyIndicator();
        //                    }
        //                }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //            this.HideBusyIndicator();
        //        }
        //    });

        //    t.Start();
        //}
        private void GetVacationInsuranceCertificates_ByPtRegID(long PtRegistrationID)
        {
            //this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetVacationInsuranceCertificates_ByPtRegID(PtRegistrationID, false, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int tempCode;
                                VacationInsuranceCertificates result = contract.EndGetVacationInsuranceCertificates_ByPtRegID(out tempCode, asyncResult);
                                CurVacationInsuranceCertificates = result ?? new VacationInsuranceCertificates { SeriNumber = tempCode + 1 };
                                IsHaveChungNhanNVKHBH = CurVacationInsuranceCertificates.VacationInsuranceCertificateID > 0;
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                //this.HideBusyIndicator();
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
        private void GetVacationPrenatalCertificates_ByPtRegID(long PtRegistrationID)
        {
            //this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetVacationInsuranceCertificates_ByPtRegID(PtRegistrationID, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int tempCode;
                                VacationInsuranceCertificates result = contract.EndGetVacationInsuranceCertificates_ByPtRegID(out tempCode, asyncResult);
                                CurVacationPrenatalCertificates = result ?? new VacationInsuranceCertificates { SeriNumber = tempCode + 1 };
                                IsHaveChungNhanNDT = CurVacationPrenatalCertificates.VacationInsuranceCertificateID > 0;
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                //this.HideBusyIndicator();
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

        private void GetPhyExam_ByPtRegID(long PtRegistrationID, long V_RegistrationType)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new SummaryServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetPhyExam_ByPtRegID(PtRegistrationID, V_RegistrationType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                PhysicalExamination result = contract.EndGetPhyExam_ByPtRegID(asyncResult);
                                CurPhysicalExamination = result ?? new PhysicalExamination();
                                Weight = result != null ? result.Weight + " (kg)" : "";
                                Pressure = result != null ? result.SystolicPressure + "/" + result.DiastolicPressure : "";
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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

        public string ReplaceICDString(string ICD10List, string ICD10Code)
        {
            string ListICD10AfterReplace = "";
            string tmpICD = "";
            if (!string.IsNullOrEmpty(ICD10List) && !string.IsNullOrEmpty(ICD10Code))
            {
                ICD10List = ICD10List.Trim();
                string[] ListArray = ICD10List.Split(',');
                if (ListArray.Length > 0)
                {
                    foreach (var strings in ListArray)
                    {
                        tmpICD = strings + ",";
                        if (String.Compare(strings, ICD10Code) == 0)
                        {
                            tmpICD = "";
                        }
                        ListICD10AfterReplace += tmpICD;
                    }
                }
            }
            return ListICD10AfterReplace;
        }
        #endregion

        #region Handle
        public void Handle(Circulars56Event message)
        {
            //if (message != null && this.IsActive)
            //{
            //    if (message.SavedSummaryMedicalRecords != null)
            //    {
            //        CurSummaryMedicalRecords = message.SavedSummaryMedicalRecords;
            //        //CurSummaryMedicalRecords.CurPatientRegistration = CurPatientRegistration;
            //        IsHaveHSBA = CurSummaryMedicalRecords.SummaryMedicalRecordID > 0;
            //    }
            //    if (message.SavedPatientTreatmentCertificates_NgT != null)
            //    {
            //        CurPatientTreatmentCertificates_NgT = message.SavedPatientTreatmentCertificates_NgT;
            //        IsHaveXacNhanDTNgT = CurPatientTreatmentCertificates_NgT.PatientTreatmentCertificateID > 0;
            //    }
            //    if (message.SavedPatientTreatmentCertificates_NT != null)
            //    {
            //        CurPatientTreatmentCertificates_NT = message.SavedPatientTreatmentCertificates_NT;
            //         IsHaveXacNhanDTNT = CurPatientTreatmentCertificates_NT.PatientTreatmentCertificateID > 0;
            //    }
            //    if (message.SavedInjuryCertificates_NgT != null)
            //    {
            //        CurInjuryCertificates_NgT = message.SavedInjuryCertificates_NgT;
            //        IsHaveChungNhanTTNgT = CurInjuryCertificates_NgT.InjuryCertificateID > 0;
            //    }
            //    if (message.SavedInjuryCertificates_NT != null)
            //    {
            //        CurInjuryCertificates_NT = message.SavedInjuryCertificates_NT;
            //        IsHaveChungNhanTTNT = CurInjuryCertificates_NT.InjuryCertificateID > 0;
            //    }
            //}
        }
        #endregion

        #region new func
        private PatientRegistration _CurPatientRegistration;
        public PatientRegistration CurPatientRegistration
        {
            get
            {
                return _CurPatientRegistration;
            }
            set
            {
                if (_CurPatientRegistration != value)
                {
                    _CurPatientRegistration = value;
                    NotifyOfPropertyChange(() => CurPatientRegistration);
                }
            }
        }

        private PhysicalExamination _CurPhysicalExamination;
        public PhysicalExamination CurPhysicalExamination
        {
            get
            {
                return _CurPhysicalExamination;
            }
            set
            {
                if (_CurPhysicalExamination != value)
                {
                    _CurPhysicalExamination = value;
                    NotifyOfPropertyChange(() => CurPhysicalExamination);
                }
            }
        }
        private SummaryMedicalRecords _CurSummaryMedicalRecords;
        public SummaryMedicalRecords CurSummaryMedicalRecords
        {
            get
            {
                return _CurSummaryMedicalRecords;
            }
            set
            {
                if (_CurSummaryMedicalRecords != value)
                {
                    _CurSummaryMedicalRecords = value;
                    NotifyOfPropertyChange(() => CurSummaryMedicalRecords);
                }
            }
        }
        private PatientTreatmentCertificates _CurPatientTreatmentCertificates_NgT;
        public PatientTreatmentCertificates CurPatientTreatmentCertificates_NgT
        {
            get
            {
                return _CurPatientTreatmentCertificates_NgT;
            }
            set
            {
                if (_CurPatientTreatmentCertificates_NgT != value)
                {
                    _CurPatientTreatmentCertificates_NgT = value;
                    NotifyOfPropertyChange(() => CurPatientTreatmentCertificates_NgT);
                }
            }
        }
        private PatientTreatmentCertificates _CurPatientTreatmentCertificates_NT;
        public PatientTreatmentCertificates CurPatientTreatmentCertificates_NT
        {
            get
            {
                return _CurPatientTreatmentCertificates_NT;
            }
            set
            {
                if (_CurPatientTreatmentCertificates_NT != value)
                {
                    _CurPatientTreatmentCertificates_NT = value;
                    NotifyOfPropertyChange(() => CurPatientTreatmentCertificates_NT);
                }
            }
        }
        private InjuryCertificates _CurInjuryCertificates_NgT;
        public InjuryCertificates CurInjuryCertificates_NgT
        {
            get
            {
                return _CurInjuryCertificates_NgT;
            }
            set
            {
                if (_CurInjuryCertificates_NgT != value)
                {
                    _CurInjuryCertificates_NgT = value;
                    NotifyOfPropertyChange(() => CurInjuryCertificates_NgT);
                }
            }
        }

        private InjuryCertificates _CurInjuryCertificates_NT;
        public InjuryCertificates CurInjuryCertificates_NT
        {
            get
            {
                return _CurInjuryCertificates_NT;
            }
            set
            {
                if (_CurInjuryCertificates_NT != value)
                {
                    _CurInjuryCertificates_NT = value;
                    NotifyOfPropertyChange(() => CurInjuryCertificates_NT);
                }
            }
        }
        private BirthCertificates _CurBirthCertificates;
        public BirthCertificates CurBirthCertificates
        {
            get
            {
                return _CurBirthCertificates;
            }
            set
            {
                if (_CurBirthCertificates != value)
                {
                    _CurBirthCertificates = value;
                    NotifyOfPropertyChange(() => CurBirthCertificates);
                }
            }
        }
        private VacationInsuranceCertificates _CurVacationInsuranceCertificates;
        public VacationInsuranceCertificates CurVacationInsuranceCertificates
        {
            get
            {
                return _CurVacationInsuranceCertificates;
            }
            set
            {
                if (_CurVacationInsuranceCertificates != value)
                {
                    _CurVacationInsuranceCertificates = value;
                    NotifyOfPropertyChange(() => CurVacationInsuranceCertificates);
                }
            }
        }

        private VacationInsuranceCertificates _CurVacationPrenatalCertificates;
        public VacationInsuranceCertificates CurVacationPrenatalCertificates
        {
            get
            {
                return _CurVacationPrenatalCertificates;
            }
            set
            {
                if (_CurVacationPrenatalCertificates != value)
                {
                    _CurVacationPrenatalCertificates = value;
                    NotifyOfPropertyChange(() => CurVacationPrenatalCertificates);
                }
            }
        }

        private string _Pressure;
        public string Pressure
        {
            get
            {
                return _Pressure;
            }
            set
            {
                if (_Pressure != value)
                {
                    _Pressure = value;
                    NotifyOfPropertyChange(() => Pressure);
                }
            }
        }

        private string _ListICD10Sub = "";
        public string ListICD10Sub
        {
            get
            {
                return _ListICD10Sub;
            }
            set
            {
                if (_ListICD10Sub != value)
                {
                    _ListICD10Sub = value;
                    NotifyOfPropertyChange(() => ListICD10Sub);
                }
            }
        }

        public void GetListDiagnosisTreatment(long? ServiceRecID, long? PatientID)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDiagnosisIcd10Items_Load(ServiceRecID, PatientID, false, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDiagnosisIcd10Items_Load(asyncResult);
                            ObservableCollection<DiagnosisIcd10Items> refIDC10List = results.ToObservableCollection();
                            if (refIDC10List == null)
                            {
                                refIDC10List = new ObservableCollection<DiagnosisIcd10Items>();
                            }
                            else
                            {
                                ListICD10 = refIDC10List.Where(x => x.IsMain).FirstOrDefault().ICD10Code;
                                Diagnosis = refIDC10List.Where(x => x.IsMain).FirstOrDefault().DiseasesReference.DiseaseNameVN;
                                ListICD10Sub = string.Join(";", refIDC10List.Where(x => !x.IsMain).Select(x => x.ICD10Code));
                                DiagnosisSub = string.Join(";", refIDC10List.Where(x => !x.IsMain).Select(x => x.DiseasesReference.DiseaseNameVN));
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
        public void GetListDiagnosisTreatment_InPT(long DTItemID)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDiagnosisIcd10Items_Load_InPt(DTItemID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDiagnosisIcd10Items_Load_InPt(asyncResult);
                            ObservableCollection<DiagnosisIcd10Items> refIDC10List = results.ToObservableCollection();
                            if (refIDC10List == null)
                            {
                                refIDC10List = new ObservableCollection<DiagnosisIcd10Items>();
                            }
                            else
                            {
                                ListICD10 = refIDC10List.Where(x => x.IsMain).FirstOrDefault().ICD10Code;
                                Diagnosis = refIDC10List.Where(x => x.IsMain).FirstOrDefault().DiseasesReference.DiseaseNameVN;
                                ListICD10Sub = string.Join(";", refIDC10List.Where(x => !x.IsMain).Select(x => x.ICD10Code));
                                DiagnosisSub = string.Join(";", refIDC10List.Where(x => !x.IsMain).Select(x => x.DiseasesReference.DiseaseNameVN));
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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

        public void AddHSBA()
        {
            if (CurPatientRegistration == null)
            {
                MessageBox.Show("Chưa chọn bệnh nhân", "Thông báo");
                return;
            }
            if (CurPatientRegistration.V_RegistrationType != AllLookupValues.RegistrationType.NOI_TRU)
            {
                MessageBox.Show("Bệnh nhân ngoại trú không thể tạo phiếu tóm tắt hồ sơ bệnh án", "Thông báo");
                return;
            }
            if (CurPatientRegistration.DischargeDate == null)
            {
                MessageBox.Show("Bệnh nhân chưa xuất viện", "Thông báo");
                return;
            }
            Action<ISummaryMedicalRecords> onInitDlg = delegate (ISummaryMedicalRecords summaryMedicalRecords)
            {
                this.ActivateItem(summaryMedicalRecords);
                summaryMedicalRecords.PtRegistration = CurPatientRegistration;
                summaryMedicalRecords.SetCurrentInformation(CurSummaryMedicalRecords.DeepCopy());
            };
            GlobalsNAV.ShowDialog<ISummaryMedicalRecords>(onInitDlg);
            GetSummaryMedicalRecords_ByPtRegID(CurPatientRegistration.PtRegistrationID, (long)CurPatientRegistration.V_RegistrationType);

        }
        public void EditHSBA()
        {
            Action<ISummaryMedicalRecords> onInitDlg = delegate (ISummaryMedicalRecords summaryMedicalRecords)
            {
                this.ActivateItem(summaryMedicalRecords);
                summaryMedicalRecords.PtRegistration = CurPatientRegistration;
                summaryMedicalRecords.SetCurrentInformation(CurSummaryMedicalRecords.DeepCopy());

            };
            GlobalsNAV.ShowDialog<ISummaryMedicalRecords>(onInitDlg);
            GetSummaryMedicalRecords_ByPtRegID(CurPatientRegistration.PtRegistrationID, (long)CurPatientRegistration.V_RegistrationType);
        }
        public void AddXacNhan_DTNgT()
        {
            if (CurPatientRegistration == null)
            {
                MessageBox.Show("Chưa chọn bệnh nhân", "Thông báo");
                return;
            }
            if (CurPatientRegistration.V_RegistrationType != AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                MessageBox.Show("Bệnh nhân nội trú không thể tạo phiếu xác nhận điều trị ngoại trú", "Thông báo");
                return;
            }
            Action<IPatientTreatmentCertificates> onInitDlg = delegate (IPatientTreatmentCertificates patientTreatmentCertificates)
            {
                this.ActivateItem(patientTreatmentCertificates);
                patientTreatmentCertificates.PtRegistration = CurPatientRegistration;
                patientTreatmentCertificates.SetCurrentInformation(new PatientTreatmentCertificates { PatientTreatmentCertificateCode = LastCodeXNDT + 1 });
            };
            GlobalsNAV.ShowDialog<IPatientTreatmentCertificates>(onInitDlg);
            GetPatientTreatmentCertificates_ByPtRegID(CurPatientRegistration.PtRegistrationID, (long)CurPatientRegistration.V_RegistrationType);
        }
        public void EditXacNhan_DTNgT()
        {
            Action<IPatientTreatmentCertificates> onInitDlg = delegate (IPatientTreatmentCertificates patientTreatmentCertificates)
            {
                this.ActivateItem(patientTreatmentCertificates);
                patientTreatmentCertificates.PtRegistration = CurPatientRegistration;
                patientTreatmentCertificates.SetCurrentInformation(CurPatientTreatmentCertificates_NgT.DeepCopy());

            };
            GlobalsNAV.ShowDialog<IPatientTreatmentCertificates>(onInitDlg);
            GetPatientTreatmentCertificates_ByPtRegID(CurPatientRegistration.PtRegistrationID, (long)CurPatientRegistration.V_RegistrationType);
        }
        public void AddXacNhan_DTNT()
        {
            if (CurPatientRegistration == null)
            {
                MessageBox.Show("Chưa chọn bệnh nhân", "Thông báo");
                return;
            }
            if (CurPatientRegistration.V_RegistrationType != AllLookupValues.RegistrationType.NOI_TRU)
            {
                MessageBox.Show("Bệnh nhân ngoại trú không thể tạo phiếu xác nhận điều trị nội trú", "Thông báo");
                return;
            }
            Action<IPatientTreatmentCertificates> onInitDlg = delegate (IPatientTreatmentCertificates patientTreatmentCertificates)
            {
                this.ActivateItem(patientTreatmentCertificates);
                patientTreatmentCertificates.PtRegistration = CurPatientRegistration;
                patientTreatmentCertificates.SetCurrentInformation(new PatientTreatmentCertificates { PatientTreatmentCertificateCode = LastCodeXNDT + 1 });
            };
            GlobalsNAV.ShowDialog<IPatientTreatmentCertificates>(onInitDlg);
            GetPatientTreatmentCertificates_ByPtRegID(CurPatientRegistration.PtRegistrationID, (long)CurPatientRegistration.V_RegistrationType);
        }
        public void EditXacNhan_DTNT()
        {
            Action<IPatientTreatmentCertificates> onInitDlg = delegate (IPatientTreatmentCertificates patientTreatmentCertificates)
            {
                this.ActivateItem(patientTreatmentCertificates);
                patientTreatmentCertificates.PtRegistration = CurPatientRegistration;
                patientTreatmentCertificates.SetCurrentInformation(CurPatientTreatmentCertificates_NT.DeepCopy());
            };
            GlobalsNAV.ShowDialog<IPatientTreatmentCertificates>(onInitDlg);
            GetPatientTreatmentCertificates_ByPtRegID(CurPatientRegistration.PtRegistrationID, (long)CurPatientRegistration.V_RegistrationType);
        }

        public void AddChungNhan_TTNgT()
        {
            if (CurPatientRegistration == null)
            {
                MessageBox.Show("Chưa chọn bệnh nhân", "Thông báo");
                return;
            }
            if (CurPatientRegistration.V_RegistrationType != AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                MessageBox.Show("Bệnh nhân nội trú không thể tạo phiếu chứng nhận thương tích ngoại trú", "Thông báo");
                return;
            }
            Action<IInjuryCertificates> onInitDlg = delegate (IInjuryCertificates injuryCertificates)
            {
                this.ActivateItem(injuryCertificates);
                injuryCertificates.IsOpenFromConsult = false;
                injuryCertificates.PtRegistration = CurPatientRegistration;
                injuryCertificates.SetCurrentInformation(new InjuryCertificates { InjuryCertificateCode = LastCodeCNTT + 1 });
            };
            GlobalsNAV.ShowDialog<IInjuryCertificates>(onInitDlg);

            GetInjuryCertificates_ByPtRegID(CurPatientRegistration.PtRegistrationID, (long)CurPatientRegistration.V_RegistrationType);
        }
        public void EditChungNhan_TTNgT()
        {
            Action<IInjuryCertificates> onInitDlg = delegate (IInjuryCertificates injuryCertificates)
            {
                this.ActivateItem(injuryCertificates);
                injuryCertificates.IsOpenFromConsult = false;
                injuryCertificates.PtRegistration = CurPatientRegistration;
                injuryCertificates.SetCurrentInformation(CurInjuryCertificates_NgT.DeepCopy());

            };
            GlobalsNAV.ShowDialog<IInjuryCertificates>(onInitDlg);
            GetInjuryCertificates_ByPtRegID(CurPatientRegistration.PtRegistrationID, (long)CurPatientRegistration.V_RegistrationType);

        }
        public void AddChungNhan_TTNT()
        {
            if (CurPatientRegistration == null)
            {
                MessageBox.Show("Chưa chọn bệnh nhân", "Thông báo");
                return;
            }
            if (CurPatientRegistration.V_RegistrationType != AllLookupValues.RegistrationType.NOI_TRU)
            {
                MessageBox.Show("Bệnh nhân ngoại trú không thể tạo phiếu chứng nhận thương tích nội trú", "Thông báo");
                return;
            }
            Action<IInjuryCertificates> onInitDlg = delegate (IInjuryCertificates injuryCertificates)
            {
                this.ActivateItem(injuryCertificates);
                injuryCertificates.IsOpenFromConsult = false;
                injuryCertificates.PtRegistration = CurPatientRegistration;
                injuryCertificates.SetCurrentInformation(new InjuryCertificates { InjuryCertificateCode = LastCodeCNTT + 1 });
            };
            GlobalsNAV.ShowDialog<IInjuryCertificates>(onInitDlg);
            GetInjuryCertificates_ByPtRegID(CurPatientRegistration.PtRegistrationID, (long)CurPatientRegistration.V_RegistrationType);

        }
        public void EditChungNhan_TTNT()
        {
            Action<IInjuryCertificates> onInitDlg = delegate (IInjuryCertificates injuryCertificates)
            {
                this.ActivateItem(injuryCertificates);
                injuryCertificates.IsOpenFromConsult = false;
                injuryCertificates.PtRegistration = CurPatientRegistration;
                injuryCertificates.SetCurrentInformation(CurInjuryCertificates_NT.DeepCopy());
            };
            GlobalsNAV.ShowDialog<IInjuryCertificates>(onInitDlg);
            GetInjuryCertificates_ByPtRegID(CurPatientRegistration.PtRegistrationID, (long)CurPatientRegistration.V_RegistrationType);
        }
        public void AddGiayChungSinh()
        {
            if (CurPatientRegistration == null)
            {
                MessageBox.Show("Chưa chọn bệnh nhân", "Thông báo");
                return;
            }
            if (CurPatientRegistration.V_RegistrationType != AllLookupValues.RegistrationType.NOI_TRU)
            {
                MessageBox.Show("Bệnh nhân ngoại trú không thể tạo phiếu chứng sinh", "Thông báo");
                return;
            }
            if (CurPatientRegistration.Patient.Gender != "Nữ")
            {
                MessageBox.Show("Bệnh nhân nam không thể tạo phiếu chứng sinh", "Thông báo");
                return;
            }
            Action<IBirthCertificates> onInitDlg = delegate (IBirthCertificates birthCertificates)
            {
                this.ActivateItem(birthCertificates);
                birthCertificates.PtRegistration = CurPatientRegistration;
                birthCertificates.SetCurrentInformation(new BirthCertificates());
            };
            GlobalsNAV.ShowDialog<IBirthCertificates>(onInitDlg);
            //GetBirthCertificates_ByPtRegID(CurPatientRegistration.PtRegistrationID, (long)CurPatientRegistration.V_RegistrationType);

        }
        public void EditGiayChungSinh()
        {
            Action<IBirthCertificates> onInitDlg = delegate (IBirthCertificates birthCertificates)
            {
                this.ActivateItem(birthCertificates);
                birthCertificates.PtRegistration = CurPatientRegistration;
                birthCertificates.SetCurrentInformation(CurBirthCertificates.DeepCopy());
            };
            GlobalsNAV.ShowDialog<IBirthCertificates>(onInitDlg);
            // GetBirthCertificates_ByPtRegID(CurPatientRegistration.PtRegistrationID, (long)CurPatientRegistration.V_RegistrationType);
        }
        public void AddChungNhanNDT()
        {
            if (CurPatientRegistration == null)
            {
                MessageBox.Show("Chưa chọn bệnh nhân", "Thông báo");
                return;
            }
            if (CurPatientRegistration.V_RegistrationType != AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                MessageBox.Show("Bệnh nhân nội trú không thể tạo Chứng nhận nghĩ dưỡng thai", "Thông báo");
                return;
            }
            if (CurPatientRegistration.Patient.Gender != "Nữ")
            {
                MessageBox.Show("Bệnh nhân nam không thể tạo phiếu nghĩ dưỡng thai", "Thông báo");
                return;
            }
            Action<IVacationInsuranceCertificate> onInitDlg = delegate (IVacationInsuranceCertificate vacationInsuranceCertificate)
            {
                this.ActivateItem(vacationInsuranceCertificate);
                vacationInsuranceCertificate.IsPrenatal = true;
                vacationInsuranceCertificate.TitleForm = eHCMSResources.Z3115_G1_ChungNhanNDT;
                vacationInsuranceCertificate.PtRegistration = CurPatientRegistration;
                vacationInsuranceCertificate.SetCurrentInformation(CurVacationPrenatalCertificates.DeepCopy());
                //vacationPrenatalCertificate.SetCurrentInformation(new BirthCertificates());
            };
            GlobalsNAV.ShowDialog<IVacationInsuranceCertificate>(onInitDlg);
            GetVacationPrenatalCertificates_ByPtRegID(CurPatientRegistration.PtRegistrationID);
        }
        public void EditChungNhanNDT()
        {
            Action<IVacationInsuranceCertificate> onInitDlg = delegate (IVacationInsuranceCertificate vacationInsuranceCertificate)
            {
                this.ActivateItem(vacationInsuranceCertificate);
                vacationInsuranceCertificate.IsPrenatal = true;
                vacationInsuranceCertificate.TitleForm = eHCMSResources.Z3115_G1_ChungNhanNDT;
                vacationInsuranceCertificate.PtRegistration = CurPatientRegistration;
                vacationInsuranceCertificate.SetCurrentInformation(CurVacationPrenatalCertificates.DeepCopy());
            };
            GlobalsNAV.ShowDialog<IVacationInsuranceCertificate>(onInitDlg);
            GetVacationPrenatalCertificates_ByPtRegID(CurPatientRegistration.PtRegistrationID);
        }
        //public void AddChungNhanNVHBH()
        //{
        //    if (CurPatientRegistration == null)
        //    {
        //        MessageBox.Show("Chưa chọn bệnh nhân", "Thông báo");
        //        return;
        //    }
        //    if (CurPatientRegistration.V_RegistrationType != AllLookupValues.RegistrationType.NGOAI_TRU)
        //    {
        //        MessageBox.Show("Bệnh nhân nội trú không thể tạo phiếu chứng nhận nghỉ việc", "Thông báo");
        //        return;
        //    }
        //    Action<IVacationInsuranceCertificate> onInitDlg = delegate (IVacationInsuranceCertificate vacationInsuranceCertificate)
        //    {
        //        this.ActivateItem(vacationInsuranceCertificate);
        //        vacationInsuranceCertificate.TitleForm = eHCMSResources.Z3115_G1_ChungNhanNVHBH;
        //        vacationInsuranceCertificate.PtRegistration = CurPatientRegistration;
        //        //vacationPrenatalCertificate.SetCurrentInformation(new BirthCertificates());
        //    };
        //    GlobalsNAV.ShowDialog<IVacationInsuranceCertificate>(onInitDlg);
        //    GetVacationInsuranceCertificates_ByPtRegID(CurPatientRegistration.PtRegistrationID);
        //}
        //public void EditChungNhanNVHBH()
        //{
        //    Action<IVacationInsuranceCertificate> onInitDlg = delegate (IVacationInsuranceCertificate vacationInsuranceCertificate)
        //    {
        //        this.ActivateItem(vacationInsuranceCertificate);
        //        vacationInsuranceCertificate.TitleForm = eHCMSResources.Z3115_G1_ChungNhanNVHBH;
        //        vacationInsuranceCertificate.PtRegistration = CurPatientRegistration;
        //        //vacationPrenatalCertificate.SetCurrentInformation(CurBirthCertificates.DeepCopy());
        //    };
        //    GlobalsNAV.ShowDialog<IVacationInsuranceCertificate>(onInitDlg);
        //    GetVacationInsuranceCertificates_ByPtRegID(CurPatientRegistration.PtRegistrationID);
        //}
        public void AddChungNhanNVKHBH()
        {
            if (CurPatientRegistration == null)
            {
                MessageBox.Show("Chưa chọn bệnh nhân", "Thông báo");
                return;
            }
            if (CurPatientRegistration.V_RegistrationType != AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                MessageBox.Show("Bệnh nhân nội trú không thể tạo phiếu chứng nhận nghỉ việc", "Thông báo");
                return;
            }
            Action<IVacationInsuranceCertificate> onInitDlg = delegate (IVacationInsuranceCertificate vacationInsuranceCertificate)
            {
                this.ActivateItem(vacationInsuranceCertificate);
                vacationInsuranceCertificate.IsPrenatal = false;
                vacationInsuranceCertificate.TitleForm = eHCMSResources.Z3115_G1_ChungNhanNVHBH;
                vacationInsuranceCertificate.PtRegistration = CurPatientRegistration;
                vacationInsuranceCertificate.SetCurrentInformation(CurVacationInsuranceCertificates.DeepCopy());
            };
            GlobalsNAV.ShowDialog<IVacationInsuranceCertificate>(onInitDlg);
            GetVacationInsuranceCertificates_ByPtRegID(CurPatientRegistration.PtRegistrationID);
        }
        public void EditChungNhanNVKHBH()
        {
            Action<IVacationInsuranceCertificate> onInitDlg = delegate (IVacationInsuranceCertificate vacationInsuranceCertificate)
            {
                this.ActivateItem(vacationInsuranceCertificate);
                vacationInsuranceCertificate.IsPrenatal = false;
                vacationInsuranceCertificate.TitleForm = eHCMSResources.Z3115_G1_ChungNhanNVHBH;
                vacationInsuranceCertificate.PtRegistration = CurPatientRegistration;
                vacationInsuranceCertificate.SetCurrentInformation(CurVacationInsuranceCertificates.DeepCopy());
            };
            GlobalsNAV.ShowDialog<IVacationInsuranceCertificate>(onInitDlg);
            GetVacationInsuranceCertificates_ByPtRegID(CurPatientRegistration.PtRegistrationID);
        }
        public void PrintGiayBaoTu()
        {
            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
            {
                proAlloc.RegistrationID = CurPatientRegistration.PtRegistrationID;
                proAlloc.eItem = ReportName.XRpt_GiayBaoTu;
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        }
        #endregion
    }
}