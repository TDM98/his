using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ServiceClient.Consultation_PCLs;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Controls;
using eHCMSLanguage;
using System.Linq;
using aEMR.Common.BaseModel;
using aEMR.Common;
using System.Windows.Media;
/*
* 20210427 #001 TNHX: 
* 20220406 #002 TNHX: 1115 - Chỉnh quy trình gửi hồ sơ cho KHTH chờ kiểm duyệt
* 20220711 #003 DatTB: Đổi màu trạng thái khi gửi lần 2.
* 20220730 #004 DatTB: Đổi vị trí 2 else if (Hồ sơ gửi lần 2 vẫn đổi màu khi DLS xử lí)
* 20230619 #005 DatTB: Lấy thêm chức danh cho BS.
*/
namespace aEMR.ConsultantEPrescription.CommonRecs.ViewModels
{
    [Export(typeof(IDeathCheckRecord)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DeathCheckRecordViewModel : ViewModelBase, IDeathCheckRecord
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public DeathCheckRecordViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            CheckRecordDateTime = Globals.GetViewModel<IMinHourDateControl>();
            CheckRecordDateTime.DateTime = null;
            CurDeathCheckRecord = new DeathCheckRecord();
            DoctorStaffs = Globals.AllStaffs.Where(x => !x.IsStopUsing 
                && x.RefStaffCategory != null
                && x.RefStaffCategory.V_StaffCatType == (long)V_StaffCatType.BacSi).ToList();
            Staffs = Globals.AllStaffs.Where(x => !x.IsStopUsing).ToList();
            Globals.EventAggregator.Subscribe(this);
            authorization();
        }

        public void InitPatientInfo(PatientRegistration CurrentPatientRegistration = null)
        {
            GetMedicalCode();
            GetDeathCheckRecord();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            //InitPatientInfo();
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }


        #region method
      
        private void SaveDeathCheckRecordByPtRegID(bool IsDeleted)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSaveDeathCheckRecord(CurDeathCheckRecord, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool res = contract.EndSaveDeathCheckRecord(asyncResult);
                                Globals.EventAggregator.Publish(new SaveDeathCheckRecord_Event { Result = res});
                                if (IsDeleted)
                                {
                                    this.TryClose();
                                }
                                GetDeathCheckRecordByPtRegID(CurDeathCheckRecord.PtRegistrationID);
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
        private void GetDeathCheckRecordByPtRegID(long PtRegistrationID)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetDeathCheckRecordByPtRegID(PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                CurDeathCheckRecord = contract.EndGetDeathCheckRecordByPtRegID(asyncResult);
                                GetDeathCheckRecord();
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
        #endregion
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
                CurrentPatientRegistration = Registration_DataStorage.CurrentPatientRegistration;
                CurPatient = Registration_DataStorage.CurrentPatient;
                NotifyOfPropertyChange(() => Registration_DataStorage);
            }
        }

        private PatientRegistration _CurrentPatientRegistration;
        public PatientRegistration CurrentPatientRegistration
        {
            get
            {
                return _CurrentPatientRegistration;
            }
            set
            {
                if (_CurrentPatientRegistration == value)
                {
                    return;
                }
                _CurrentPatientRegistration = value;
                NotifyOfPropertyChange(() => CurrentPatientRegistration);
            }
        }
        private Patient _CurPatient;
        public Patient CurPatient
        {
            get
            {
                return _CurPatient;
            }
            set
            {
                if (_CurPatient == value)
                {
                    return;
                }
                _CurPatient = value;
                NotifyOfPropertyChange(() => CurPatient);
            }
        }
        private string _MedicalCode;
        public string MedicalCode
        {
            get
            {
                return _MedicalCode;
            }
            set
            {
                _MedicalCode = value;
                NotifyOfPropertyChange(() => MedicalCode);
            }
        }
        private List<Staff> _DoctorStaffs;
        public List<Staff> DoctorStaffs
        {
            get
            {
                return _DoctorStaffs;
            }
            set
            {
                _DoctorStaffs = value;
                NotifyOfPropertyChange(() => DoctorStaffs);
            }
        }
        private List<Staff> _Staffs;
        public List<Staff> Staffs
        {
            get
            {
                return _Staffs;
            }
            set
            {
                _Staffs = value;
                NotifyOfPropertyChange(() => Staffs);
            }
        }
        private Staff _PresideStaffs;
        public Staff PresideStaffs
        {
            get
            {
                return _PresideStaffs;
            }
            set
            {
                _PresideStaffs = value;
                NotifyOfPropertyChange(() => PresideStaffs);
            }
        }
        private Staff _SecretaryStaffs;
        public Staff SecretaryStaffs
        {
            get
            {
                return _SecretaryStaffs;
            }
            set
            {
                _SecretaryStaffs = value;
                NotifyOfPropertyChange(() => SecretaryStaffs);
            }
        }
        private Staff _MemberStaffs;
        public Staff MemberStaffs
        {
            get
            {
                return _MemberStaffs;
            }
            set
            {
                _MemberStaffs = value;
                NotifyOfPropertyChange(() => MemberStaffs);
            }
        }
        
        private IMinHourDateControl _CheckRecordDateTime;
        public IMinHourDateControl CheckRecordDateTime
        {
            get { return _CheckRecordDateTime; }
            set
            {
                _CheckRecordDateTime = value;
                NotifyOfPropertyChange(() => CheckRecordDateTime);
            }
        }
        private DeathCheckRecord _CurDeathCheckRecord;
        public DeathCheckRecord CurDeathCheckRecord
        {
            get { return _CurDeathCheckRecord; }
            set
            {
                _CurDeathCheckRecord = value;
                NotifyOfPropertyChange(() => CurDeathCheckRecord);
            }
        }
        private void GetMedicalCode()
        { 
            if(CurPatient == null || CurrentPatientRegistration == null)
            {
                return;
            }
            string HospitalCode = Globals.ServerConfigSection.Hospitals.HospitalCode;
            string FileCode = CurPatient.FileCodeNumber;
            DateTime AdmissionDate = CurrentPatientRegistration.AdmissionInfo.AdmissionDate.Value;
            MedicalCode = HospitalCode.Substring(0, 2) + "/" + HospitalCode.Substring(2, 3) + "/" 
                + AdmissionDate.Date.ToString("yy") + "/"
                + FileCode.Substring(4,6);
        }
        private void GetDeathCheckRecord()
        { 
            if(CurDeathCheckRecord != null && CurDeathCheckRecord.DeathCheckRecordID > 0)
            {
                CheckRecordDateTime.DateTime = CurDeathCheckRecord.CheckRecordDate;
                PresideStaffs = DoctorStaffs.Where(x => x.StaffID == CurDeathCheckRecord.PresideStaffID).FirstOrDefault();
                SecretaryStaffs = DoctorStaffs.Where(x => x.StaffID == CurDeathCheckRecord.SecretaryStaffID).FirstOrDefault();
            }
        }
        public void cboDoctor_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            var AllItemsContext = new ObservableCollection<Staff>(DoctorStaffs.Where(x => x.FullName.ToLower().Contains(cboContext.SearchText.ToLower())).ToList());
            cboContext.ItemsSource = AllItemsContext;
            cboContext.PopulateComplete();
        }
        public void cboDoctorSecretary_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            var AllItemsContext = new ObservableCollection<Staff>(DoctorStaffs.Where(x => x.FullName.ToLower().Contains(cboContext.SearchText.ToLower())).ToList());
            cboContext.ItemsSource = AllItemsContext;
            cboContext.PopulateComplete();
        }
        public void cboDoctorMember_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            var AllItemsContext = new ObservableCollection<Staff>(Staffs.Where(x => x.FullName.ToLower().Contains(cboContext.SearchText.ToLower())).ToList());
            cboContext.ItemsSource = AllItemsContext;
            cboContext.PopulateComplete();
        }
        public void cboDoctorMember_SelectedItemChanged(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            Staff selectedStaff = cboContext.SelectedItem as Staff;
            if (CurDeathCheckRecord != null && cboContext.SelectedItem != null)
            {
                //▼==== #005
                CurDeathCheckRecord.MemberStaff += string.IsNullOrWhiteSpace(CurDeathCheckRecord.MemberStaff)
                    ? ("+ " + selectedStaff.PrintTitle + selectedStaff.FullName)
                    : Environment.NewLine + ("+ " + selectedStaff.PrintTitle + selectedStaff.FullName);
                //▲==== #005
            }
        }
        public void btnSave()
        {
            if(CurDeathCheckRecord == null)
            {
                MessageBox.Show("Lỗi khi khởi tạo. Vui lòng tìm lại bệnh nhân và tạo lại biên bản");
                return;
            }
            CurDeathCheckRecord.PatientID = CurPatient.PatientID;
            CurDeathCheckRecord.PtRegistrationID = CurrentPatientRegistration.PtRegistrationID;
            CurDeathCheckRecord.CheckRecordDate = CheckRecordDateTime.DateTime.GetValueOrDefault(Globals.GetCurServerDateTime());
            CurDeathCheckRecord.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
            CurDeathCheckRecord.PresideStaffID = PresideStaffs == null ? 0 :  PresideStaffs.StaffID;
            CurDeathCheckRecord.SecretaryStaffID = SecretaryStaffs == null ? 0 :  SecretaryStaffs.StaffID;
            CurDeathCheckRecord.MedicalCode = MedicalCode;
            SaveDeathCheckRecordByPtRegID(false);
        }
        public void btnPreview()
        {
            if (CurDeathCheckRecord == null || CurDeathCheckRecord.DeathCheckRecordID <= 0)
            {
                return;
            }
            if (CurDeathCheckRecord != null)
            {
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                {
                    proAlloc.eItem = ReportName.XRpt_BienBanKiemDiemTuVong;
                    proAlloc.ID = CurDeathCheckRecord.DeathCheckRecordID;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
            }
        }
        public void btnDelete()
        {
            if(CurDeathCheckRecord == null || CurDeathCheckRecord.DeathCheckRecordID <= 0)
            {
                return;
            }
            if (MessageBox.Show("Bạn có chắc muốn xóa Trích biên bản kiểm điểm tử vong này","",MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                CurDeathCheckRecord.IsDeleted = true;
                SaveDeathCheckRecordByPtRegID(true);
            }
        }
    }
}
