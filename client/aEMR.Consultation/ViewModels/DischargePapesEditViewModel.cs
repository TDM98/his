using aEMR.Common;
using aEMR.Common.BaseModel;
using aEMR.Controls;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using DataEntities;
using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
/*
* 20230414 #001 QTD: Tạo mới View
*/
namespace aEMR.Consultation.ViewModels
{
    [Export(typeof(IDischargePapersEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DischargePapesEditViewModel : ViewModelBase, IDischargePapersEdit
    {
        [ImportingConstructor]
        public DischargePapesEditViewModel(IWindsorContainer aContainer, INavigationService aNavigation, ISalePosCaching aCaching)
        {
            DischargeDateContent = Globals.GetViewModel<IMinHourDateControl>();
            DischargeDateContent.IsEnableMinHourControl = false;
            DischargeDateContent.DateTime = null;
            LoadDoctorStaffCollection();
            NoteTemplates_GetAllIsActive();
            DateOfPregnancyTermination = Globals.GetViewModel<IMinHourDateControl>();
            if (DischargePapersInfo != null)
            {
                DateOfPregnancyTermination.DateTime = DischargePapersInfo.PregnancyTerminationDateTime;
            }
            else
            {
                DateOfPregnancyTermination.DateTime = null;
            }
            (DateOfPregnancyTermination as INotifyPropertyChangedEx).PropertyChanged += DateOfPregnancyTermination_PropertyChanged;
        }
        #region Properties
        private PatientRegistration _CurrentRegistration;
        public PatientRegistration CurrentRegistration
        {
            get { return _CurrentRegistration; }
            set
            {
                _CurrentRegistration = value;
                if(CurrentRegistration.DischargeDate != null)
                {
                    DischargeDateContent.DateTime = CurrentRegistration.DischargeDate;
                }
                NotifyOfPropertyChange(() => CurrentRegistration);
            }
        }

        private IMinHourDateControl _DateOfPregnancyTermination;
        public IMinHourDateControl DateOfPregnancyTermination
        {
            get { return _DateOfPregnancyTermination; }
            set
            {
                _DateOfPregnancyTermination = value;
                NotifyOfPropertyChange(() => DateOfPregnancyTermination);
            }
        }

        private IMinHourDateControl _DischargeDateContent;
        public IMinHourDateControl DischargeDateContent
        {
            get { return _DischargeDateContent; }
            set
            {
                _DischargeDateContent = value;
                NotifyOfPropertyChange(() => DischargeDateContent);
            }
        }        

        private long _OutPtTreatmentProgramID;
        public long OutPtTreatmentProgramID
        {
            get { return _OutPtTreatmentProgramID; }
            set
            {
                _OutPtTreatmentProgramID = value;
                NotifyOfPropertyChange(() => OutPtTreatmentProgramID);
            }
        }

        private string _Notes;
        public string Notes
        {
            get { return _Notes; }
            set
            {
                _Notes = value;
                NotifyOfPropertyChange(() => Notes);
            }
        }

        private string _DischargeDiagnostic;
        public string DischargeDiagnostic
        {
            get { return _DischargeDiagnostic; }
            set
            {
                _DischargeDiagnostic = value;
                NotifyOfPropertyChange(() => DischargeDiagnostic);
            }
        }

        private bool _IsPregnancyTermination;

        public bool IsPregnancyTermination
        {
            get { return _IsPregnancyTermination; }
            set
            {
                if (_IsPregnancyTermination != value)
                {
                    _IsPregnancyTermination = value;
                    if (DischargePapersInfo != null)
                    {
                        DischargePapersInfo.IsPregnancyTermination = _IsPregnancyTermination;
                    }
                    NotifyOfPropertyChange(() => IsPregnancyTermination);
                }
            }
        }

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
                if (_ObjNoteTemplates_Selected != null && _ObjNoteTemplates_Selected.PrescriptNoteTemplateID > 0 && DischargePapersInfo != null)
                {
                    string str = DischargePapersInfo.TreatmentMethod;
                    if (string.IsNullOrEmpty(str))
                    {
                        str = ObjNoteTemplates_Selected.DetailsTemplate;
                    }
                    else
                    {
                        str = str + Environment.NewLine + ObjNoteTemplates_Selected.DetailsTemplate;
                    }
                    DischargePapersInfo.TreatmentMethod = str;
                }
            }
        }

        #endregion
        public void SaveButton()
        {
            if (DischargePapersInfo == null || CurrentRegistration == null || CurrentRegistration.PtRegistrationID == 0)
            {
                return;
            }
            if (string.IsNullOrEmpty(DischargePapersInfo.DischargeDiagnostic))
            {
                MessageBox.Show("Bắt buộc nhập chẩn đoán ra viện!");
                return;
            }
            if (string.IsNullOrEmpty(DischargePapersInfo.TreatmentMethod))
            {
                MessageBox.Show("Bắt buộc nhập phương pháp điều trị!");
                return;
            }
            if (DischargePapersInfo.HeadOfDepartmentDoctorStaffID == null || DischargePapersInfo.UnitLeaderDoctorStaffID == null)
            {
                MessageBox.Show("Bắt buộc chọn bác sĩ ký giấy ra viện!");
                return;
            }
            if (IsPregnancyTermination)
            {
                if (DischargePapersInfo.FetalAge == null)
                {
                    MessageBox.Show("Bắt buộc nhập tuổi thai khi chọn đình chỉ thai!");
                    return;
                }
                if (DischargePapersInfo.FetalAge != null &&
                (DischargePapersInfo.FetalAge > 42 || DischargePapersInfo.FetalAge < 1))
                {
                    MessageBox.Show("Tuổi thai luôn luôn lớn hơn hoặc bắng 1 và nhỏ hơn hoặc bằng 42 tuần tuổi");
                    return;
                }
                if (DischargePapersInfo.PregnancyTerminationDateTime == null)
                {
                    MessageBox.Show("Bắt buộc nhập ngày giờ đình chỉ thai!");
                    return;
                }
                if (string.IsNullOrEmpty(DischargePapersInfo.ReasonOfPregnancyTermination))
                {
                    MessageBox.Show("Bắt buộc nhập nguyên nhân đình chỉ thai!");
                    return;
                }
            }
            DischargePapersInfo.PtRegistrationID = CurrentRegistration.PtRegistrationID;
            DischargePapersInfo.V_RegistrationType = (long)CurrentRegistration.V_RegistrationType;
            DischargePapersInfo.DoctorStaffID = Globals.LoggedUserAccount.StaffID.GetValueOrDefault();
            this.DlgShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                using (var CurrentFactory = new CommonUtilsServiceClient())
                {
                    var CurrentContract = CurrentFactory.ServiceInstance;
                    CurrentContract.BeginSaveDisChargePapersInfo(DischargePapersInfo, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            bool bOk = CurrentContract.EndSaveDisChargePapersInfo(asyncResult);
                            if (bOk)
                            {
                                MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                                Globals.EventAggregator.Publish(new SaveDisChargePapersInfo_Event { Result = true });
                                GetDischargePapersInfo(DischargePapersInfo.PtRegistrationID, DischargePapersInfo.V_RegistrationType);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.G0442_G1_TBao);
                        }
                        finally
                        {
                            this.DlgHideBusyIndicator();
                        }
                    }), null);
                }
            });
            CurrentThread.Start();
        }

        public void GetDischargePapersInfo(long ptRegistrationID, long v_RegistrationType)
        {
            if(ptRegistrationID == 0 || v_RegistrationType == 0)
            {
                return;
            }
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDischargePapersInfo(ptRegistrationID, v_RegistrationType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string DoctorAdvice = "";
                            DischargePapersInfo = contract.EndGetDischargePapersInfo(out DoctorAdvice, asyncResult);
                            if (DischargePapersInfo == null)
                            {
                                DischargePapersInfo = new DischargePapersInfo();
                                DischargePapersInfo.DischargeDiagnostic = DischargeDiagnostic;
                                DischargePapersInfo.Notes = Notes;
                                DischargePapersInfo.Notes = DoctorAdvice;
                            }
                            else
                            {
                                if (DischargePapersInfo.PaperID != 0)
                                {
                                    DateOfPregnancyTermination.DateTime = DischargePapersInfo.PregnancyTerminationDateTime;
                                    gSelectedDoctorStaff = DoctorStaffs != null ? DoctorStaffs.FirstOrDefault(x => x.StaffID == DischargePapersInfo.HeadOfDepartmentDoctorStaffID) : null;
                                    gSelectedUnitLeaderDoctorStaff = DoctorStaffs != null ? DoctorStaffs.FirstOrDefault(x => x.StaffID == DischargePapersInfo.UnitLeaderDoctorStaffID) : null;
                                    IsPregnancyTermination = DischargePapersInfo.IsPregnancyTermination;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                        }
                    }), null);
                }
            });
            t.Start();
        }

        private void NoteTemplates_GetAllIsActive()
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
                                firstItem.NoteDetails = "--Chọn--";
                                ObjNoteTemplates_GetAll.Insert(0, firstItem);
                                if (ObjNoteTemplates_GetAll != null)
                                {
                                    ObjNoteTemplates_Selected = ObjNoteTemplates_GetAll.FirstOrDefault();
                                }
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

        private DischargePapersInfo _DischargePapersInfo = new DischargePapersInfo();
        public DischargePapersInfo DischargePapersInfo
        {
            get
            {
                return _DischargePapersInfo;
            }
            set
            {
                _DischargePapersInfo = value;
                NotifyOfPropertyChange(() => DischargePapersInfo);
            }
        }

        public bool HasPregnancyTermination
        {
            get
            {
                return CurrentRegistration != null && CurrentRegistration.Patient.Gender == "F";
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
                _gSelectedDoctorStaff = value;
                if(DischargePapersInfo != null && _gSelectedDoctorStaff != null)
                {
                    DischargePapersInfo.HeadOfDepartmentDoctorStaffID = _gSelectedDoctorStaff.StaffID;
                }
                NotifyOfPropertyChange(() => gSelectedDoctorStaff);
            }
        }

        private Staff _gSelectedUnitLeaderDoctorStaff;
        public Staff gSelectedUnitLeaderDoctorStaff
        {
            get
            {
                return _gSelectedUnitLeaderDoctorStaff;
            }
            set
            {
                _gSelectedUnitLeaderDoctorStaff = value;
                if (DischargePapersInfo != null && _gSelectedUnitLeaderDoctorStaff != null)
                {
                    DischargePapersInfo.UnitLeaderDoctorStaffID = _gSelectedUnitLeaderDoctorStaff.StaffID;
                }
                NotifyOfPropertyChange(() => gSelectedUnitLeaderDoctorStaff);
            }
        }

        private void LoadDoctorStaffCollection()
        {
            DoctorStaffs = new ObservableCollection<Staff>(Globals.AllStaffs.Where(x => x.RefStaffCategory != null
                                                                                    && x.RefStaffCategory.V_StaffCatType == Globals.ServerConfigSection.CommonItems.StaffCatTypeBAC_SI
                                                                                    && (!x.IsStopUsing)).ToList());
            //&& x.PrintTitle != null && x.PrintTitle.Trim().ToLower() == "bs.").ToList());
            //&& Globals.ServerConfigSection.CommonItems.BacSiTruongPhoKhoa.Contains("|" + x.V_JobPosition.ToString() + "|")).ToList());
            if (DoctorStaffs.Count() > 0 && DoctorStaffs.Any(x => Globals.ServerConfigSection.CommonItems.BacSiTruongPhoKhoa.Contains("|" + x.V_JobPosition.ToString() + "|") 
                && x.StaffID == Globals.LoggedUserAccount.Staff.StaffID))
            {
                gSelectedDoctorStaff = DoctorStaffs.Where(x => x.StaffID == Globals.LoggedUserAccount.Staff.StaffID).FirstOrDefault();
            }
            if (DoctorStaffs.Count() > 0 && DoctorStaffs.Any(x => Globals.ServerConfigSection.CommonItems.ThuTruongDonVi.Contains("|" + x.V_JobPosition.ToString() + "|") 
                && x.StaffID == Globals.LoggedUserAccount.Staff.StaffID))
            {
                gSelectedUnitLeaderDoctorStaff = DoctorStaffs.Where(x => x.StaffID == Globals.LoggedUserAccount.Staff.StaffID).FirstOrDefault();
            }
            
        }

        AxAutoComplete AcbDoctorStaff { get; set; }

        public void DoctorStaff_Loaded(object sender, RoutedEventArgs e)
        {
            AcbDoctorStaff = (AxAutoComplete)sender;
        }

        public void DoctorStaff_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            var AllItemsContext = new ObservableCollection<Staff>(DoctorStaffs.Where(x => Globals.ServerConfigSection.CommonItems.BacSiTruongPhoKhoa.Contains("|" + x.V_JobPosition.ToString() + "|") &&
                Globals.RemoveVietnameseString(x.FullName).ToLower().Contains(Globals.RemoveVietnameseString(cboContext.SearchText).ToLower())));
            cboContext.ItemsSource = AllItemsContext;
            cboContext.PopulateComplete();
        }

        public void DoctorStaff_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (sender != null)
            {
                gSelectedDoctorStaff = ((AutoCompleteBox)sender).SelectedItem as Staff;
            }
        }

        AxAutoComplete AcbUnitLeaderDoctorStaff { get; set; }
        public void UnitLeaderDoctorStaff_Loaded(object sender, RoutedEventArgs e)
        {
            AcbUnitLeaderDoctorStaff = (AxAutoComplete)sender;
        }

        public void UnitLeaderDoctorStaff_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            var AllItemsContext = new ObservableCollection<Staff>(DoctorStaffs.Where(x => Globals.ServerConfigSection.CommonItems.ThuTruongDonVi.Contains("|" + x.V_JobPosition.ToString() + "|") &&
                Globals.RemoveVietnameseString(x.FullName).ToLower().Contains(Globals.RemoveVietnameseString(cboContext.SearchText).ToLower())));
                                                                                        //&& x.IsUnitLeader));
            cboContext.ItemsSource = AllItemsContext;
            cboContext.PopulateComplete();
        }

        public void UnitLeaderDoctorStaff_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (sender != null)
            {
                gSelectedUnitLeaderDoctorStaff = ((AutoCompleteBox)sender).SelectedItem as Staff;
            }
        }

        void DateOfPregnancyTermination_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DateTime")
            {
                if (DischargePapersInfo != null)
                {
                    DischargePapersInfo.PregnancyTerminationDateTime = DateOfPregnancyTermination.DateTime.GetValueOrDefault(DateTime.MinValue);
                }
            }
        }

        public void PrintButton()
        {
            if (DischargePapersInfo == null || CurrentRegistration == null || CurrentRegistration.PtRegistrationID == 0)
            {
                return;
            }
            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
            {
                proAlloc.RegistrationID = CurrentRegistration.PtRegistrationID;
                proAlloc.V_RegistrationType = (long)CurrentRegistration.V_RegistrationType;
                proAlloc.eItem = ReportName.XRpt_DisChargePapers;
            };
            GlobalsNAV.ShowDialog(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        }
    }
}