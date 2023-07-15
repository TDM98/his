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
using System.ServiceModel;
using aEMR.DataContracts;
/*
* 20230311 #001 QTD:    Đổ lại dữ liệu quốc tịch
* 20230601 #002 QTD:    Sửa sự kiện thay đổi phương pháp điều trị
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(ISummaryMedicalRecords_OutPt)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class SummaryMedicalRecords_OutPtViewModel : Conductor<object>, ISummaryMedicalRecords_OutPt
    {

        public void SetCurrentInformation(SummaryMedicalRecords item)
        {
            if (item != null)
            {
                CurrentSummaryMedicalRecords = item;
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
                if (DoctorStaffs != null && CurrentSummaryMedicalRecords != null && CurrentSummaryMedicalRecords.ChiefDoctorStaffID > 0)
                {
                    gSelectedDoctorStaff = DoctorStaffs.FirstOrDefault(x => x.StaffID == CurrentSummaryMedicalRecords.ChiefDoctorStaffID);
                }
                SelectedOutDischargeCondition = V_OutDischargeCondition.FirstOrDefault(x => x.LookupID == CurrentSummaryMedicalRecords.V_OutDischargeCondition);
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
        public SummaryMedicalRecords_OutPtViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            _eventArg = eventArg;
            TitleForm = eHCMSResources.Z3115_G1_TomTatHSBA;
            //LoadCountries();
            //LoadEthnicsList();
            //▼====: #001
            //Coroutine.BeginExecute(DoNationalityList());
            //▲====: #001
            ObjNoteTemplates_GetAll = new ObservableCollection<PrescriptionNoteTemplates>();
            NoteTemplates_GetAllIsActive();
            GetV_OutDischargeCondition();
            LoadDoctorStaffCollection();
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
            CurrentSummaryMedicalRecords.ChiefDoctorStaffID = gSelectedDoctorStaff.StaffID;
        }

        public bool CheckValidBeforeSave()
        {
            if (string.IsNullOrWhiteSpace(CurrentSummaryMedicalRecords.PathologicalProcess))
            {
                MessageBox.Show("Vui lòng nhập quá trình bệnh lý");
                return false;
            }
            if (string.IsNullOrWhiteSpace(CurrentSummaryMedicalRecords.Treatment))
            {
                MessageBox.Show("Vui lòng nhập phương pháp điều trị");
                return false;
            }
            if (string.IsNullOrWhiteSpace(CurrentSummaryMedicalRecords.AdmissionDiagnosis))
            {
                MessageBox.Show("Vui lòng nhập chẩn đoán vào viện");
                return false;
            }
            if (string.IsNullOrWhiteSpace(CurrentSummaryMedicalRecords.DischargeDiagnosis))
            {
                MessageBox.Show("Vui lòng nhập chẩn đoán ra viện");
                return false;
            }
            if (string.IsNullOrWhiteSpace(CurrentSummaryMedicalRecords.DischargeStatus))
            {
                MessageBox.Show("Vui lòng nhập tình trạng người bệnh ra viện");
                return false;
            }
            if (CurrentSummaryMedicalRecords.V_OutDischargeCondition == 0)
            {
                MessageBox.Show("Vui lòng chọn kết quả điều trị");
                return false;
            }
            if (gSelectedDoctorStaff == null)
            {
                MessageBox.Show("Vui lòng chọn bác sĩ được ủy quyền");
                return false;
            }
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
                    contract.BeginGetSummaryPCLResultByPtRegistrationID(PtRegistrationID, (long)AllLookupValues.RegistrationType.NGOAI_TRU, Globals.DispatchCallback((asyncResult) =>
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
        private ObservableCollection<PrescriptionNoteTemplates> _ObjNoteTemplates_GetAll;
        public ObservableCollection<PrescriptionNoteTemplates> ObjNoteTemplates_GetAll
        {
            get { return _ObjNoteTemplates_GetAll; }
            set
            {
                _ObjNoteTemplates_GetAll = value;
                NotifyOfPropertyChange(() => ObjNoteTemplates_GetAll);
            }
        }
        private PrescriptionNoteTemplates _ObjNoteTemplates_Selected;
        public PrescriptionNoteTemplates ObjNoteTemplates_Selected
        {
            get { return _ObjNoteTemplates_Selected; }
            set
            {
                if (_ObjNoteTemplates_Selected == value)
                {
                    return;
                }
                _ObjNoteTemplates_Selected = value;
                NotifyOfPropertyChange(() => ObjNoteTemplates_Selected);
                //▼====: #002
                //if (_ObjNoteTemplates_Selected != null 
                //    && _ObjNoteTemplates_Selected.PrescriptNoteTemplateID > 0 
                //    && CurrentSummaryMedicalRecords != null)
                //{
                //    string str = CurrentSummaryMedicalRecords.Treatment;
                //    if (string.IsNullOrEmpty(str))
                //    {
                //        str = ObjNoteTemplates_Selected.DetailsTemplate;
                //    }
                //    else
                //    {
                //        str = str + Environment.NewLine + ObjNoteTemplates_Selected.DetailsTemplate;
                //    }
                //    CurrentSummaryMedicalRecords.Treatment = str;
                //}
                //▲====: #002
            }
        }
        private ObservableCollection<Lookup> _V_OutDischargeCondition;
        public ObservableCollection<Lookup> V_OutDischargeCondition
        {
            get
            {
                return _V_OutDischargeCondition;
            }
            set
            {
                if (_V_OutDischargeCondition != value)
                {
                    _V_OutDischargeCondition = value;
                    NotifyOfPropertyChange(() => V_OutDischargeCondition);
                }
            }
        }
        private Lookup _SelectedOutDischargeCondition;
        public Lookup SelectedOutDischargeCondition
        {
            get
            {
                return _SelectedOutDischargeCondition;
            }
            set
            {
                if (_SelectedOutDischargeCondition != value)
                {
                    _SelectedOutDischargeCondition = value;
                    if (CurrentSummaryMedicalRecords != null && _SelectedOutDischargeCondition != null)
                    {
                        CurrentSummaryMedicalRecords.V_OutDischargeCondition = _SelectedOutDischargeCondition.LookupID;
                    }
                    NotifyOfPropertyChange(() => SelectedOutDischargeCondition);
                }
            }
        }
        public void NoteTemplates_GetAllIsActive()
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePrescriptionsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        var pnt = new PrescriptionNoteTemplates();
                        pnt.V_PrescriptionNoteTempType = AllLookupValues.V_PrescriptionNoteTempType.Treatments;
                        contract.BeginPrescriptionNoteTemplates_GetAllIsActive(pnt, Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<PrescriptionNoteTemplates> allItems = new ObservableCollection<PrescriptionNoteTemplates>();
                            try
                            {
                                allItems = contract.EndPrescriptionNoteTemplates_GetAllIsActive(asyncResult);
                                ObjNoteTemplates_GetAll = new ObservableCollection<PrescriptionNoteTemplates>(allItems);
                                PrescriptionNoteTemplates firstItem = new PrescriptionNoteTemplates();
                                firstItem.PrescriptNoteTemplateID = -1;
                                firstItem.NoteDetails = "--Vui lòng chọn phương pháp--";
                                ObjNoteTemplates_GetAll.Insert(0, firstItem);
                                SetDefaultSelected();
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                }
            });
            t.Start();
        }
        private void SetDefaultSelected()
        {
            if (ObjNoteTemplates_GetAll != null)
            {
                ObjNoteTemplates_Selected = ObjNoteTemplates_GetAll.FirstOrDefault();
            }

            //if (ObjPrescriptionNoteTemplates_Treatments != null)
            //{
            //    ObjPrescriptionNoteTemplates_Treatments_Selected = ObjPrescriptionNoteTemplates_Treatments.FirstOrDefault();
            //}

            //if (ObjPrescriptionNoteTemplates_TreatmentDirectionFollowupRegimen != null)
            //{
            //    ObjPrescriptionNoteTemplates_TreatmentDirectionFollowupRegimen_Selected = ObjPrescriptionNoteTemplates_TreatmentDirectionFollowupRegimen.FirstOrDefault();
            //}
        }
        private void GetV_OutDischargeCondition()
        {
            V_OutDischargeCondition = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_OutDischargeCondition).ToObservableCollection();
            Lookup firstItem = new Lookup();
            firstItem.LookupID = 0;
            firstItem.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, "Vui lòng chọn kết quả điều trị");
            V_OutDischargeCondition.Insert(0, firstItem);
            SelectedOutDischargeCondition = V_OutDischargeCondition.FirstOrDefault();
        }
        private Staff _gSelectedDoctorStaff;
        public Staff gSelectedDoctorStaff
        {
            get
            {
                return _gSelectedDoctorStaff;
            }
            set
            {
                if (_gSelectedDoctorStaff == value) return;
                _gSelectedDoctorStaff = value;
                NotifyOfPropertyChange(() => gSelectedDoctorStaff);
            }
        }
        private ObservableCollection<Staff> _DoctorStaffs;
        public ObservableCollection<Staff> DoctorStaffs
        {
            get
            {
                return _DoctorStaffs;
            }
            set
            {
                if (_DoctorStaffs != value)
                {
                    _DoctorStaffs = value;
                    NotifyOfPropertyChange(() => DoctorStaffs);
                }
            }
        }
        public void DoctorStaff_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            if (Globals.ServerConfigSection.CommonItems.IsApplyTimeSegments)
            {
                DoctorStaffs = DoctorStaffs.Where(x =>
                            x.ConsultationTimeSegmentsList != null &&
                            (x.ConsultationTimeSegmentsList.Where(y =>
                                    y.StartTime.TimeOfDay < Globals.ServerDate.Value.TimeOfDay
                                    && y.EndTime.TimeOfDay > Globals.ServerDate.Value.TimeOfDay).Count() > 0
                            || x.ConsultationTimeSegmentsList.Where(y =>
                                    y.EndTime2 != null
                                    && y.StartTime2.Value.TimeOfDay < Globals.ServerDate.Value.TimeOfDay
                                    && y.EndTime2.Value.TimeOfDay > Globals.ServerDate.Value.TimeOfDay).Count() > 0)).ToObservableCollection();
            }
            var AllItemsContext = new ObservableCollection<Staff>(DoctorStaffs.Where(x => Globals.RemoveVietnameseString(x.FullName).ToLower().Contains(Globals.RemoveVietnameseString(cboContext.SearchText).ToLower())));
            cboContext.ItemsSource = AllItemsContext;
            cboContext.PopulateComplete();
        }
        public void DoctorStaff_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            gSelectedDoctorStaff = ((AutoCompleteBox)sender).SelectedItem as Staff;
        }
        private void LoadDoctorStaffCollection()
        {
            DoctorStaffs = new ObservableCollection<Staff>(Globals.AllStaffs.Where(x => x.RefStaffCategory != null
                && x.RefStaffCategory.V_StaffCatType == Globals.ServerConfigSection.CommonItems.StaffCatTypeBAC_SI && (!x.IsStopUsing) //&& x.IsUnitLeader
                && Globals.ServerConfigSection.CommonItems.ThuTruongDonVi.Contains("|" + x.V_JobPosition.ToString() + "|") ).ToList());
            //DoctorStaffs.Insert(0, new Staff { StaffID = 0, FullName = "Vui lòng chọn Bác sĩ ký tóm tắt HSBA!"});
            if(DoctorStaffs.Count() > 0 && DoctorStaffs.Any(x => x.StaffID == Globals.LoggedUserAccount.Staff.StaffID))
            {
                gSelectedDoctorStaff = DoctorStaffs.Where(x => x.StaffID == Globals.LoggedUserAccount.Staff.StaffID).FirstOrDefault();
            }
        }

        //▼====: #002
        public void cbo_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            PrescriptionNoteTemplates obj = (sender as ComboBox).SelectedItem as PrescriptionNoteTemplates;
            if (obj != null && obj.PrescriptNoteTemplateID > 0 && CurrentSummaryMedicalRecords != null)
            {
                string str = CurrentSummaryMedicalRecords.Treatment;
                if (string.IsNullOrEmpty(str))
                {
                    str = obj.DetailsTemplate;
                }
                else
                {
                    str = str + Environment.NewLine + obj.DetailsTemplate;
                }
                CurrentSummaryMedicalRecords.Treatment = str;
            }
        }
        //▲====: #002
    }
}