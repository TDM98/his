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
using aEMR.ViewContracts;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.CommonTasks;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure.Events.SL_Events;
using aEMR.Common;
using eHCMS.Services.Core;
/*
 * 20230311 #001 QTD:    Đổ lại dữ liệu quốc tịch
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IVacationInsuranceCertificate)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class VacationInsuranceCertificateViewModel : Conductor<object>, IVacationInsuranceCertificate
    {
        public void SetCurrentInformation(VacationInsuranceCertificates item)
        {
            if (IsOpenFromConsult)
            {

                if (item != null)
                {
                    CurrentVacationInsuranceCertificates = item;
                    if (IsNew = CurrentVacationInsuranceCertificates.VacationInsuranceCertificateID == 0)
                    {
                        
                        CurrentVacationInsuranceCertificates.FromDate = (DateTime)PtRegistration.ExamDate;
                        if (String.IsNullOrEmpty(PtRegistration.DiagnosisTreatment.DoctorComments))
                        {
                            CurrentVacationInsuranceCertificates.ToDate = (DateTime)PtRegistration.ExamDate;
                        }
                        else
                        {
                            CurrentVacationInsuranceCertificates.ToDate = (DateTime)PtRegistration.ExamDate.AddDays(GetNgayNghiOm());
                        }
                        CurrentVacationInsuranceCertificates.Diagnosis = PtRegistration.DiagnosisTreatment.DiagnosisFinal + " " + PtRegistration.DiagnosisTreatment.DiagnosisOther;
                        CurrentVacationInsuranceCertificates.V_RegistrationType = (long)PtRegistration.V_RegistrationType;
                    }
                    else
                    {
                        gSelectedDoctorStaff = DoctorStaffs.Where(x => x.StaffID == CurrentVacationInsuranceCertificates.CheifDoctorStaffID).FirstOrDefault();
                        gSelectedDoctorStaffSign = DoctorStaffsSign.Where(x => x.StaffID == CurrentVacationInsuranceCertificates.DoctorStaffID).FirstOrDefault();
                    }
                }
            }
            else
            {
                if (item != null)
                {
                    CurrentVacationInsuranceCertificates = item;
                    if (IsNew = CurrentVacationInsuranceCertificates.VacationInsuranceCertificateID == 0)
                    {
                        CurrentVacationInsuranceCertificates.FromDate = (DateTime)PtRegistration.AdmissionDate;
                        if (String.IsNullOrEmpty(PtRegistration.DiagnosisTreatment.DoctorComments))
                        {
                            CurrentVacationInsuranceCertificates.ToDate = (DateTime)PtRegistration.AdmissionDate;
                        }
                        else
                        {
                            CurrentVacationInsuranceCertificates.ToDate = (DateTime)PtRegistration.AdmissionDate.Value.AddDays(GetNgayNghiOm());
                        }

                        CurrentVacationInsuranceCertificates.Diagnosis = PtRegistration.DiagnosisTreatment.DiagnosisFinal + " " + PtRegistration.DiagnosisTreatment.DiagnosisOther; 
                        CurrentVacationInsuranceCertificates.V_RegistrationType = (long)PtRegistration.V_RegistrationType;
                        CurrentVacationInsuranceCertificates.TreatmentMethod = PtRegistration.DiagnosisTreatment.OrientedTreatment;
                    }
                    else
                    {
                        gSelectedDoctorStaff = DoctorStaffs.Where(x => x.StaffID == CurrentVacationInsuranceCertificates.CheifDoctorStaffID).FirstOrDefault();
                        gSelectedDoctorStaffSign = DoctorStaffsSign.Where(x => x.StaffID == CurrentVacationInsuranceCertificates.DoctorStaffID).FirstOrDefault();
                    }
                }
                IsYounger7YO = (DateTime.Now.Year - ((DateTime)PtRegistration.Patient.DOB).Year) < 7;
            }
        }
        private string GetLine(string text, int lineNo)
        {
            string[] lines = text.Replace("\r", "").Split('\n');
            return lines.Length >= lineNo ? lines[lineNo - 1] : null;
        }
        private int GetNgayNghiOm()
        {
            string FirstLine = GetLine(PtRegistration.DiagnosisTreatment.DoctorComments, 1);
            string IntString = string.Empty;
            int val = 0;
            foreach(var ch in FirstLine)
            {
                if (Char.IsDigit(ch))
                    IntString += ch;
            }
            if (IntString.Length > 0)
                val = int.Parse(IntString);
            NgayNghiOm = val;
            return val;
        }
        private VacationInsuranceCertificates _CurrentVacationInsuranceCertificates = new VacationInsuranceCertificates();
        public VacationInsuranceCertificates CurrentVacationInsuranceCertificates
        {
            get
            {
                return _CurrentVacationInsuranceCertificates;
            }
            set
            {
                _CurrentVacationInsuranceCertificates = value;
                NotifyOfPropertyChange(() => CurrentVacationInsuranceCertificates);
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

        private string _TitleForm="" ;
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
        private bool _IsYounger7YO=false;
        public bool IsYounger7YO
        {
            get
            {
                return _IsYounger7YO;
            }
            set
            {
                _IsYounger7YO = value;
                NotifyOfPropertyChange(() => IsYounger7YO);
            }
        }
        private bool _IsPrenatal = false;
        public bool IsPrenatal
        {
            get
            {
                return _IsPrenatal;
            }
            set
            {
                _IsPrenatal = value;
                NotifyOfPropertyChange(() => IsPrenatal);
            }
        }
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public VacationInsuranceCertificateViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            _eventArg = eventArg;
            TitleForm = eHCMSResources.Z3115_G1_ChungNhanNVHBH;
            LoadCountries();
            LoadEthnicsList();
            LoadGenders();
            //▼====: #001
            Coroutine.BeginExecute(DoNationalityListList());
            //▲====: #001
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
        private ObservableCollection<Gender> _genders;
        public ObservableCollection<Gender> Genders
        {
            get { return _genders; }
            set
            {
                _genders = value;
                NotifyOfPropertyChange(() => Genders);
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
        public void LoadGenders()
        {
            if (Globals.allGenders != null)
            {
                Genders = Globals.allGenders.ToObservableCollection();
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllGenders(
                            Globals.DispatchCallback(asyncResult =>
                            {
                                IList<Gender> allItems = null;
                                try
                                {
                                    allItems = contract.EndGetAllGenders(asyncResult);
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                    MessageBox.Show(eHCMSResources.A0692_G1_Msg_InfoKhTheLayDSGioiTinh);
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                }
                                if (allItems != null)
                                {
                                    Globals.allGenders = allItems.ToList();
                                    Genders = Globals.allGenders.ToObservableCollection();
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
            if (CurrentVacationInsuranceCertificates.PtRegistrationID == 0)
            {
                CurrentVacationInsuranceCertificates.PtRegistrationID = PtRegistration.PtRegistrationID;
            }
            CurrentVacationInsuranceCertificates.CheifDoctorStaffID = gSelectedDoctorStaff == null ? 0: gSelectedDoctorStaff.StaffID;
            if (IsPrenatal)
            {
                CurrentVacationInsuranceCertificates.DoctorStaffID = Globals.LoggedUserAccount.Staff.StaffID;
            }
            else
            {
                CurrentVacationInsuranceCertificates.DoctorStaffID = gSelectedDoctorStaffSign == null ? 0 : gSelectedDoctorStaffSign.StaffID;
            }
            CurrentVacationInsuranceCertificates.IsDelete = IsDelete;
            CurrentVacationInsuranceCertificates.IsInsurance = true;// Set mặc định là nghĩ hưởng bảo hiểm
        }
        public bool CheckValidBeforeSave()
        {
            if (IsPrenatal && PtRegistration.HIReportID > 0)
            {
                MessageBox.Show("Giấy nghỉ dưỡng đã được đẩy cổng giám định BHXH, nen không thể hiệu chỉnh. " +
                    "Nếu muốn cấp lại giấy cho người bệnh, vui lòng liên hệ P. Kế toán BHYT để hủy đẩy cổng giám định BHXH!");
                return false;
            }
            if (IsPrenatal && NgayNghiOm > 30)
            {
                MessageBox.Show("Số ngày nghỉ vượt quá 30 ngày theo số ngày quy định nghỉ dưỡng thai. Vui lòng điều chỉnh trường đến ngày!");
                return false;
            }
            if (!IsPrenatal 
                //&& CurrentVacationInsuranceCertificates.IsInsurance 
                && CurrentVacationInsuranceCertificates.IsSuspendedPregnant 
                && (CurrentVacationInsuranceCertificates.GestationalAge < 1 || CurrentVacationInsuranceCertificates.GestationalAge > 42))
            {
                MessageBox.Show("Tuổi thai phải nằm trong khoảng 1-42 tuần!");
                return false;
            }
           
            if (!IsPrenatal 
                //&& CurrentVacationInsuranceCertificates.IsInsurance
                && CurrentVacationInsuranceCertificates.IsSuspendedPregnant
                && string.IsNullOrWhiteSpace(CurrentVacationInsuranceCertificates.ReasonSuspendedPregnant))
            {
                MessageBox.Show("Bắt buộc phải nhập nguyên nhân đình chỉ thai!");
                return false;
            }
            int SoNgayNghi = CheckSoNgayNghiHuongBHXH();
            if (!IsPrenatal
                //&& CurrentVacationInsuranceCertificates.IsInsurance
                && SoNgayNghi > 0
                && NgayNghiOm > SoNgayNghi)
            {
                MessageBox.Show("Số ngày nghỉ tối đa không được vượt quá " + SoNgayNghi + " ngày");
                return false;
            }
            if (!IsPrenatal
                && string.IsNullOrWhiteSpace(CurrentVacationInsuranceCertificates.TreatmentMethod))
            {
                MessageBox.Show("Chưa nhập phương pháp điều trị");
                return false;
            }
            if(CurrentVacationInsuranceCertificates.FromDate == null 
                || CurrentVacationInsuranceCertificates.ToDate == null 
                || (CurrentVacationInsuranceCertificates.ToDate.Date - CurrentVacationInsuranceCertificates.FromDate.Date).Days < 0 )
            {
                MessageBox.Show("Đến ngày không được phép nhỏ hơn từ ngày");
                return false;
            }
            //if (!IsPrenatal && Convert.ToInt32(PtRegistration.Patient.AgeString) < 7 && string.IsNullOrEmpty(CurrentVacationInsuranceCertificates.FatherName) && string.IsNullOrEmpty(CurrentVacationInsuranceCertificates.MotherName))
            //{
            //    MessageBox.Show("Phải nhập tên cha hoặc mẹ");
            //    return false;
            //}

            return true;
        }
        private int CheckSoNgayNghiHuongBHXH()
        {
            int SoNgay = 30; 
            if (CurrentVacationInsuranceCertificates.IsSuspendedPregnant && CurrentVacationInsuranceCertificates.GestationalAge >= 13)
            {
                SoNgay = 50;
            }
            else if (CurrentVacationInsuranceCertificates.IsTuberculosis)
            {
                SoNgay = 180;
            }
            return SoNgay;
        }
        private void SaveVacationInsuranceCertificates(bool IsDelete)
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
                    contract.BeginSaveVacationInsuranceCertificates(CurrentVacationInsuranceCertificates,  IsPrenatal, (long)Globals.LoggedUserAccount.StaffID, Globals.GetCurServerDateTime().ToString("ddMMyyyy"), Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            VacationInsuranceCertificates result = contract.EndSaveVacationInsuranceCertificates(asyncResult);
                            if (result != null)
                            {
                                CurrentVacationInsuranceCertificates.VacationInsuranceCertificateID = result.DeepCopy().VacationInsuranceCertificateID;
                                MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                            }
                            IsNew = CurrentVacationInsuranceCertificates.VacationInsuranceCertificateID == 0;
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
       
        public void SaveVacationInsuranceCertificatesCmd()
        {
            SaveVacationInsuranceCertificates(false);
        }

        public void DeleteVacationInsuranceCertificatesCmd()
        {
            if (MessageBox.Show(eHCMSResources.K0483_G1_BanCoChacChanMuonXoa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                SaveVacationInsuranceCertificates(true);
            }
        }

        public void PrintVacationInsuranceCertificatesCmd()
        {
            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
            {
                proAlloc.ID = (long)CurrentVacationInsuranceCertificates.VacationInsuranceCertificateID;
                proAlloc.eItem = ReportName.XRpt_GiayNghiViecKhongHuongBaoHiem;
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
        private bool _IsOpenFromConsult;
        public bool IsOpenFromConsult
        {
            get { return _IsOpenFromConsult; }
            set
            {
                _IsOpenFromConsult = value;
                NotifyOfPropertyChange(() => IsOpenFromConsult);
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
        private Staff _gSelectedDoctorStaffSign;
        public Staff gSelectedDoctorStaffSign
        {
            get
            {
                return _gSelectedDoctorStaffSign;
            }
            set
            {
                if (_gSelectedDoctorStaffSign == value) return;
                _gSelectedDoctorStaffSign = value;
                NotifyOfPropertyChange(() => gSelectedDoctorStaffSign);
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
        private ObservableCollection<Staff> _DoctorStaffsSign;
        public ObservableCollection<Staff> DoctorStaffsSign
        {
            get
            {
                return _DoctorStaffsSign;
            }
            set
            {
                if (_DoctorStaffsSign != value)
                {
                    _DoctorStaffsSign = value;
                    NotifyOfPropertyChange(() => DoctorStaffsSign);
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
        public void DoctorStaffSign_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            if (Globals.ServerConfigSection.CommonItems.IsApplyTimeSegments)
            {
                DoctorStaffsSign = DoctorStaffsSign.Where(x =>
                            x.ConsultationTimeSegmentsList != null &&
                            (x.ConsultationTimeSegmentsList.Where(y =>
                                    y.StartTime.TimeOfDay < Globals.ServerDate.Value.TimeOfDay
                                    && y.EndTime.TimeOfDay > Globals.ServerDate.Value.TimeOfDay).Count() > 0
                            || x.ConsultationTimeSegmentsList.Where(y =>
                                    y.EndTime2 != null
                                    && y.StartTime2.Value.TimeOfDay < Globals.ServerDate.Value.TimeOfDay
                                    && y.EndTime2.Value.TimeOfDay > Globals.ServerDate.Value.TimeOfDay).Count() > 0)).ToObservableCollection();
            }
            var AllItemsContext = new ObservableCollection<Staff>(DoctorStaffsSign.Where(x => Globals.RemoveVietnameseString(x.FullName).ToLower().Contains(Globals.RemoveVietnameseString(cboContext.SearchText).ToLower())));
            cboContext.ItemsSource = AllItemsContext;
            cboContext.PopulateComplete();
        }
        public void DoctorStaffSign_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            gSelectedDoctorStaffSign = ((AutoCompleteBox)sender).SelectedItem as Staff;
        }
        private void LoadDoctorStaffCollection()
        {
            DoctorStaffs = new ObservableCollection<Staff>(Globals.AllStaffs.Where(x => x.RefStaffCategory != null
                && x.RefStaffCategory.V_StaffCatType == Globals.ServerConfigSection.CommonItems.StaffCatTypeBAC_SI && (!x.IsStopUsing) //&& x.IsUnitLeader
                && Globals.ServerConfigSection.CommonItems.ThuTruongDonVi.Contains("|" + x.V_JobPosition.ToString() + "|")).ToList());
            //DoctorStaffs.Insert(0, new Staff { StaffID = 0, FullName = "Vui lòng chọn Bác sĩ ký tóm tắt HSBA!"});
            DoctorStaffsSign = new ObservableCollection<Staff>(Globals.AllStaffs.Where(x => x.RefStaffCategory != null
                && x.RefStaffCategory.V_StaffCatType == Globals.ServerConfigSection.CommonItems.StaffCatTypeBAC_SI && (!x.IsStopUsing)).ToList());

            if (DoctorStaffs.Count() > 0 && DoctorStaffs.Any(x => x.StaffID == Globals.LoggedUserAccount.Staff.StaffID))
            {
                gSelectedDoctorStaff = DoctorStaffs.Where(x => x.StaffID == Globals.LoggedUserAccount.Staff.StaffID).FirstOrDefault();
            }
            if (DoctorStaffsSign.Count() > 0 && DoctorStaffsSign.Any(x => x.StaffID == Globals.LoggedUserAccount.Staff.StaffID))
            {
                gSelectedDoctorStaffSign = DoctorStaffs.Where(x => x.StaffID == Globals.LoggedUserAccount.Staff.StaffID).FirstOrDefault();
            }
        }
        private int _NgayNghiOm;
        public int NgayNghiOm
        {
            get
            {
                return _NgayNghiOm;
            }
            set
            {
                if (_NgayNghiOm != value)
                {
                    _NgayNghiOm = value;
                    NotifyOfPropertyChange(() => NgayNghiOm);
                }
            }
        }
        public void DatePicker_SelectedDateChanged(object sender, PopulatingEventArgs e)
        {
            if(CurrentVacationInsuranceCertificates == null 
                || CurrentVacationInsuranceCertificates.FromDate == null 
                || CurrentVacationInsuranceCertificates.ToDate == null)
            {
                return;
            }
            NgayNghiOm = (CurrentVacationInsuranceCertificates.ToDate.Date - CurrentVacationInsuranceCertificates.FromDate.Date).Days + 1;
        }
        private string _InsuranceCertificatePrefix = Globals.ServerConfigSection.CommonItems.InsuranceCertificatePrefix;
        public string InsuranceCertificatePrefix
        {
            get
            {
                return _InsuranceCertificatePrefix;
            }
            set
            {
                if (_InsuranceCertificatePrefix != value)
                {
                    _InsuranceCertificatePrefix = value;
                    NotifyOfPropertyChange(() => InsuranceCertificatePrefix);
                }
            }
        }
        public void chk_Click(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            CheckBox checkBox = sender as CheckBox;
            int countCheck = 0;
            if (CurrentVacationInsuranceCertificates.IsSuspendedPregnant)
            {
                countCheck++;
            }
            if (CurrentVacationInsuranceCertificates.ChildUnder7)
            {
                countCheck++;
            }
            if (CurrentVacationInsuranceCertificates.IsTuberculosis)
            {
                countCheck++;
            }

            if (countCheck > 1)
            {
                MessageBox.Show("Chỉ có thể chọn một loại thông tin nghỉ! Vui lòng bỏ chọn trước khi chọn lại");
                checkBox.IsChecked = false;
            }
        }
    }
}