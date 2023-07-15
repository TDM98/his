using System;
using System.Windows;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using DataEntities;
using aEMR.Infrastructure.Events;
using System.Collections.Generic;
using aEMR.CommonTasks;
using System.Threading;
using aEMR.ServiceClient;
using eHCMSLanguage;
using System.Linq;
using System.Collections.ObjectModel;
using aEMR.Common.Collections;
using System.Windows.Controls;
using Castle.Windsor;
using aEMR.Common.BaseModel;
using System.Text;
using aEMR.Common;
using System.Windows.Data;
using aEMR.Controls;
using System.Windows.Input;
using aEMR.Common.HotKeyManagement;
using System.ComponentModel;
using eHCMS.Services.Core.Base;
using System.Windows.Media;
using System.Text.RegularExpressions;
using Service.Core.Common;
using aEMR.ServiceClient.Consultation_PCLs;
using aEMR.ViewContracts.Configuration;
using System.Xml;
/*
* 20220110 #001 BLQ: Create New
*/
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(ITransmissionMonitor)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class TransmissionMonitorViewModel : ViewModelBase, ITransmissionMonitor
        , IHandle<RegistrationSelectedForInPtInstruction_1>
        , IHandle<RegistrationSelectedForInPtInstruction_2>
    {
        [ImportingConstructor]
        public TransmissionMonitorViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            MedicalInstructionDateContent = Globals.GetViewModel<IMinHourDateControl>();
            InPatientDailyDiagnosisContent = Globals.GetViewModel<IInPatientDailyDiagnosis>();
            MedicalInstructionDateContent.DateTime = new DateTime(Globals.GetCurServerDateTime().Year, Globals.GetCurServerDateTime().Month, Globals.GetCurServerDateTime().Day, Globals.GetCurServerDateTime().Hour, Globals.GetCurServerDateTime().Minute, 0);
            LoadDoctorStaffCollection();
        }
        #region Properties
        private IMinHourDateControl _MedicalInstructionDateContent;
        public IMinHourDateControl MedicalInstructionDateContent
        {
            get { return _MedicalInstructionDateContent; }
            set
            {
                _MedicalInstructionDateContent = value;
                NotifyOfPropertyChange(() => MedicalInstructionDateContent);
            }
        }

        private IInPatientDailyDiagnosis _inPatientDailyDiagnosisContent;

        public IInPatientDailyDiagnosis InPatientDailyDiagnosisContent
        {
            get
            {
                return _inPatientDailyDiagnosisContent;
            }
            set
            {
                _inPatientDailyDiagnosisContent = value;
                NotifyOfPropertyChange(() => InPatientDailyDiagnosisContent);
            }
        }

        private PatientRegistration _curRegistration;
        public PatientRegistration CurRegistration
        {
            get
            {
                return _curRegistration;
            }
            set
            {
                if (_curRegistration != value)
                {
                    _curRegistration = value;
                    _curRegistration.InPatientInstruction = new InPatientInstruction();
                    NotifyOfPropertyChange(() => CurRegistration);
                }
            }
        }

        private ObservableCollection<InPatientDeptDetail> _InPatientDeptDetails;
        public ObservableCollection<InPatientDeptDetail> InPatientDeptDetails
        {
            get
            {
                return _InPatientDeptDetails;
            }
            set
            {
                _InPatientDeptDetails = value;
                NotifyOfPropertyChange(() => InPatientDeptDetails);
            }
        }

        private InPatientDeptDetail _SelectedInPatientDeptDetail;
        public InPatientDeptDetail SelectedInPatientDeptDetail
        {
            get
            {
                return _SelectedInPatientDeptDetail;
            }
            set
            {
                _SelectedInPatientDeptDetail = value;
                NotifyOfPropertyChange(() => SelectedInPatientDeptDetail);
            }
        }

        private ObservableCollection<DeptLocation> _gAttachLocationCollection;
        public ObservableCollection<DeptLocation> gAttachLocationCollection
        {
            get
            {
                return _gAttachLocationCollection;
            }
            set
            {
                _gAttachLocationCollection = value;
                NotifyOfPropertyChange(() => gAttachLocationCollection);
            }
        }

        private ObservableCollection<BedPatientAllocs> _BedAllocations;
        public ObservableCollection<BedPatientAllocs> BedAllocations
        {
            get
            {
                return _BedAllocations;
            }
            set
            {
                _BedAllocations = value;
                NotifyOfPropertyChange(() => BedAllocations);
            }
        }

        private BedPatientAllocs _SelectedBedAllocation;
        public BedPatientAllocs SelectedBedAllocation
        {
            get
            {
                return _SelectedBedAllocation;
            }

            set
            {
                _SelectedBedAllocation = value;
                NotifyOfPropertyChange(() => SelectedBedAllocation);
            }
        }

        private IRegistration_DataStorage _Registration_DataStorage;
        public IRegistration_DataStorage Registration_DataStorage
        {
            get
            {
                return _Registration_DataStorage;
            }
            set
            {
                if (_Registration_DataStorage == value)
                {
                    return;
                }
                _Registration_DataStorage = value;
                NotifyOfPropertyChange(() => Registration_DataStorage);
                InPatientDailyDiagnosisContent.Registration_DataStorage = Registration_DataStorage;
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

        private ObservableCollection<InPatientInstruction> _ListInPatientInstruction = new ObservableCollection<InPatientInstruction>();
        public ObservableCollection<InPatientInstruction> ListInPatientInstruction
        {
            get
            {
                return _ListInPatientInstruction;
            }
            set
            {
                if (_ListInPatientInstruction != value)
                {
                    _ListInPatientInstruction = value;
                    NotifyOfPropertyChange(() => ListInPatientInstruction);
                }
            }
        }

        private ObservableCollection<InPatientInstruction> _AllInPatientInstruction;
        public ObservableCollection<InPatientInstruction> AllInPatientInstruction
        {
            get
            {
                return _AllInPatientInstruction;
            }
            set
            {
                if (_AllInPatientInstruction != value)
                {
                    _AllInPatientInstruction = value;
                    NotifyOfPropertyChange(() => AllInPatientInstruction);
                }
            }
        }

        private InPatientInstruction _SelectedInPatientInstruction;
        public InPatientInstruction SelectedInPatientInstruction
        {
            get
            {
                return _SelectedInPatientInstruction;
            }
            set
            {
                if (_SelectedInPatientInstruction != value)
                {
                    _SelectedInPatientInstruction = value;
                    NotifyOfPropertyChange(() => SelectedInPatientInstruction);
                }
            }
        }
        private ObservableCollection<TransmissionMonitor> _TransmissionMonitorList;
        public ObservableCollection<TransmissionMonitor> TransmissionMonitorList
        {
            get
            {
                return _TransmissionMonitorList;
            }
            set
            {
                if (_TransmissionMonitorList != value)
                {
                    _TransmissionMonitorList = value;
                    NotifyOfPropertyChange(() => TransmissionMonitorList);
                }
            }
        }
        #endregion
        #region Handle
        public void Handle(RegistrationSelectedForInPtInstruction_1 message)
        {
            if (this.GetView() != null && message != null && message.PtRegistration != null)
            {
                OpenRegistration(message.PtRegistration.PtRegistrationID);
            }
        }
        public void Handle(RegistrationSelectedForInPtInstruction_2 message)
        {
            if (this.GetView() != null && message != null && message.PtRegistration != null)
            {
                OpenRegistration(message.PtRegistration.PtRegistrationID);
            }
        }
        #endregion
        #region Events
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }
        public void OpenRegistration(long regID)
        {
            Coroutine.BeginExecute(DoOpenRegistration(regID));
        }
        public void cboDepartments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedInPatientDeptDetail == null || CurRegistration == null) return;
            gAttachLocationCollection = new ObservableCollection<DeptLocation> { SelectedInPatientDeptDetail.DeptLocation };
            BedAllocations = CurRegistration.BedAllocations.Where(x => x.ResponsibleDeptID == SelectedInPatientDeptDetail.DeptLocation.DeptID).ToObservableCollection();
            SelectedBedAllocation = BedAllocations != null ? BedAllocations.FirstOrDefault() : null;
            if (CurRegistration.InPatientInstruction != null)
            {
                CurRegistration.InPatientInstruction.LocationInDept = gAttachLocationCollection.FirstOrDefault();
            }
            GetInPatientInstructionByDept();
        }
        public void cboInPatientInstruction_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(SelectedInPatientInstruction == null)
            {
                return;
            }
            MedicalInstructionDateContent.DateTime = SelectedInPatientInstruction.InstructionDate;
            gSelectedDoctorStaff = SelectedInPatientInstruction.DoctorStaff;
            InPatientDailyDiagnosisContent.IntPtDiagDrInstructionID = SelectedInPatientInstruction.IntPtDiagDrInstructionID;
            InPatientDailyDiagnosisContent.PatientRegistrationContent = CurRegistration;
            InPatientDailyDiagnosisContent.GetLatestDiagnosisTreatment_InPtByInstructionID_V2(CurRegistration.PatientID.Value, SelectedInPatientInstruction.IntPtDiagDrInstructionID, (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_DAILY, false);
            GetTransmissionMonitorByInPatientInstructionID(SelectedInPatientInstruction.IntPtDiagDrInstructionID);
        }
        public IEnumerator<IResult> DoOpenRegistration(long regID)
        {
            //KMx: Chỉ lấy những thông tin cần thiết của đăng ký thôi, không load hết (17/09/2014 17:31).
            LoadRegistrationSwitch LoadRegisSwitch = new LoadRegistrationSwitch();
            LoadRegisSwitch.IsGetAdmissionInfo = true;
            LoadRegisSwitch.IsGetBedAllocations = true;
            var loadRegTask = new LoadRegistrationInfo_InPtTask(regID, (int)AllLookupValues.V_FindPatientType.NOI_TRU, LoadRegisSwitch);
            yield return loadRegTask;
            LoadRegistrationInfo(loadRegTask.Registration);
        }
        public void LoadRegistrationInfo(PatientRegistration aRegistration, bool LoadInstructionAlso = true)
        {
            RefreshTransmissionMonitor();
            if (aRegistration == null)
            {
                //Thong bao khong load duoc dang ky
                Globals.EventAggregator.Publish(new ShowMessageEvent { Message = "Error!(7)" });
            }
            else
            {
                CurRegistration = aRegistration;
                this.InPatientDeptDetails = CurRegistration != null && CurRegistration.AdmissionInfo != null && CurRegistration.AdmissionInfo.InPatientDeptDetails != null 
                    ? CurRegistration.AdmissionInfo.InPatientDeptDetails.Where(x=>x.DeptLocation.DeptID == Globals.DeptLocation.DeptID).ToObservableCollection() 
                    : null;
                SelectedInPatientDeptDetail = this.InPatientDeptDetails.FirstOrDefault();
                LoadInPatientInstruction();
            }
           
        }
        private void RefreshTransmissionMonitor()
        {
            TransmissionMonitorList = new ObservableCollection<TransmissionMonitor>();
            ListInPatientInstruction = new ObservableCollection<InPatientInstruction>();
            
            MedicalInstructionDateContent.DateTime = null;
            gSelectedDoctorStaff = new Staff();
            InPatientDailyDiagnosisContent.setDefaultValueWhenReNew();
            InPatientDailyDiagnosisContent.DiagnosisTreatmentContent.DiagnosisFinal = "";
        }
        #endregion
        #region Methods
        private void GetInPatientInstructionByDept()
        {
            if(CurRegistration == null || AllInPatientInstruction == null || SelectedInPatientDeptDetail == null)
            {
                return;
            }
            if (AllInPatientInstruction.Count == 0)
            {
                return;
            }
            ListInPatientInstruction = AllInPatientInstruction.Where(x => x.InPatientDeptDetailID == SelectedInPatientDeptDetail.InPatientDeptDetailID).ToObservableCollection();
        }
        private void LoadInPatientInstruction()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetInPatientInstructionCollectionForTransmissionMonitor(CurRegistration.PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var mPatientInstruction = contract.EndGetInPatientInstructionCollectionForTransmissionMonitor(asyncResult);
                                AllInPatientInstruction = mPatientInstruction.ToObservableCollection();
                                GetInPatientInstructionByDept();
                            }
                            catch (Exception ex)
                            {
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
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void GetTransmissionMonitorByInPatientInstructionID(long InPatientInstructionID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetTransmissionMonitorByInPatientInstructionID(InPatientInstructionID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var mTransmissionMonitor = contract.EndGetTransmissionMonitorByInPatientInstructionID(asyncResult);
                                TransmissionMonitorList = mTransmissionMonitor.ToObservableCollection();
                            }
                            catch (Exception ex)
                            {
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
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        #endregion
        private void LoadDoctorStaffCollection()
        {
            DoctorStaffs = new ObservableCollection<Staff>(Globals.AllStaffs.Where(x => x.RefStaffCategory != null 
                                                                                    && x.RefStaffCategory.V_StaffCatType == Globals.ServerConfigSection.CommonItems.StaffCatTypeBAC_SI
                                                                                    && (!x.IsStopUsing)).ToList());
            gSelectedDoctorStaff = DoctorStaffs != null ? DoctorStaffs.FirstOrDefault(x => x.StaffID == Globals.LoggedUserAccount.StaffID) : null;
        }
        public void DoctorStaff_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            if (Globals.ServerConfigSection.ConsultationElements.FilterDoctorByDeptResponsibilitiesInPt)
            {
                string tempCurDeptID = SelectedInPatientDeptDetail != null && SelectedInPatientDeptDetail.DeptLocation != null ? SelectedInPatientDeptDetail.DeptLocation.DeptID.ToString() : "";
                DoctorStaffs = DoctorStaffs.Where(x => x.ListDeptResponsibilities != null && ((x.ListDeptResponsibilities.Contains(tempCurDeptID) || tempCurDeptID == ""))).ToObservableCollection();
            }
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
        public void AddTM_Click(object datacontext, object eventArgs)
        {
            if (datacontext == null || !(datacontext is TransmissionMonitor))
            {
                return;
            }
            TransmissionMonitor temp = datacontext as TransmissionMonitor;
            temp.PtRegistrationID = CurRegistration.PtRegistrationID;
            temp.IntPtDiagDrInstructionID = SelectedInPatientInstruction.IntPtDiagDrInstructionID;
            ITransmissionMonitorEdit aView = Globals.GetViewModel<ITransmissionMonitorEdit>();
            aView.CurTransmissionMonitor = temp;
            aView.InitTransmissionMonitor(true);
            GlobalsNAV.ShowDialog_V3<ITransmissionMonitorEdit>(aView, null, null, false, false);
            GetTransmissionMonitorByInPatientInstructionID(SelectedInPatientInstruction.IntPtDiagDrInstructionID);
        }
        public void EditTM_Click(object datacontext, object eventArgs)
        {
            if (datacontext == null || !(datacontext is TransmissionMonitor) || (datacontext as TransmissionMonitor).TransmissionMonitorID == 0)
            {
                return;
            }
            TransmissionMonitor temp = datacontext as TransmissionMonitor;
            ITransmissionMonitorEdit aView = Globals.GetViewModel<ITransmissionMonitorEdit>();
            aView.CurTransmissionMonitor = temp;
            aView.InitTransmissionMonitor(false);
            GlobalsNAV.ShowDialog_V3<ITransmissionMonitorEdit>(aView, null, null, false, false);
            GetTransmissionMonitorByInPatientInstructionID(SelectedInPatientInstruction.IntPtDiagDrInstructionID);
        }
        public void DeleteTM_Click(object datacontext, object eventArgs)
        {
          
        }

        public void btnPreview()
        {
            if (SelectedInPatientInstruction == null) return;
            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
            {
                proAlloc.eItem = ReportName.PhieuTheoDoiDichTruyen;
                proAlloc.IntPtDiagDrInstructionID = SelectedInPatientInstruction.IntPtDiagDrInstructionID;
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        }
        public void btnPreviewTH()
        {
            if (CurRegistration == null && CurRegistration.InPatientInstruction == null) return;

            IReportCriteria ReportCriteria = Globals.GetViewModel<IReportCriteria>();
            GlobalsNAV.ShowDialog_V3<IReportCriteria>(ReportCriteria, (aView) =>
            {
                aView.FromDate = Globals.GetCurServerDateTime();
                aView.ToDate = Globals.GetCurServerDateTime();
            });
            if (ReportCriteria.IsCompleted)
            {
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                {
                    proAlloc.eItem = ReportName.PhieuTheoDoiDichTruyen;
                    proAlloc.RegistrationID = CurRegistration.PtRegistrationID;
                    proAlloc.FromDate = ReportCriteria.FromDate;
                    proAlloc.ToDate = ReportCriteria.ToDate;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
            }
        }
    }
}
