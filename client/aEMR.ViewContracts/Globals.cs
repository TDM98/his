using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Hosting;
using System.Windows;
using System.Windows.Threading;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using System.IO;
using System.Text;
using Caliburn.Micro;
using DataEntities;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure.ServiceCore;
using aEMR.Infrastructure.CommonTasks;
using aEMR.Common;
using aEMR.Common.Collections;
using aEMR.Common.Enums;

/*
 * 20170113 #001 CMN: Add Hex decode method
 * 20180319 #002 CMN: Set CalByUnitUse into globals function
 * 20180406 #003 CMN: Changed round number cause of dispense volume can up to 1000
*/
namespace aEMR.ViewContracts
{
    public class txdConType
    {
        public Type txdIntType { get; set; }
    }
    public static partial class Globals
    {
        public static bool CheckAuthorisation()
        {
            return isAccountCheck;
        }
        public static bool CheckAuthorization(List<refModule> listRefModule, int module, int function, int operation, int permission)
        {
            if (listRefModule[module].lstFunction == null)
            {
                return false;
            }
            if (listRefModule[module].lstFunction[function].lstOperation == null)
            {
                return false;
            }
            if (listRefModule[module].lstFunction[function].lstOperation.Count <= operation || listRefModule[module].lstFunction[function].lstOperation[operation].mOperation == null)
            {
                return false;
            }
            switch (permission)
            {
                case (int)ePermission.mAdd:
                    if (listRefModule[module].lstFunction[function].lstOperation[operation].mPermission.pAdd == false)
                    {
                        return false;
                    }
                    break;
                case (int)ePermission.mDelete:
                    if (listRefModule[module].lstFunction[function].lstOperation[operation].mPermission.pDelete == false)
                    {
                        return false;
                    }
                    break;
                case (int)ePermission.mEdit:
                    if (listRefModule[module].lstFunction[function].lstOperation[operation].mPermission.pUpdate == false)
                    {
                        return false;
                    }
                    break;
                case (int)ePermission.mFull:
                    if (listRefModule[module].lstFunction[function].lstOperation[operation].mPermission.pFullControl == false)
                    {
                        return false;
                    }
                    break;
                case (int)ePermission.mPrint:
                    if (listRefModule[module].lstFunction[function].lstOperation[operation].mPermission.pPrint == false)
                    {
                        return false;
                    }
                    break;
                case (int)ePermission.mReport:
                    if (listRefModule[module].lstFunction[function].lstOperation[operation].mPermission.pReport == false)
                    {
                        return false;
                    }
                    break;
                case (int)ePermission.mView:
                    if (listRefModule[module].lstFunction[function].lstOperation[operation].mPermission.pView == false)
                    {
                        return false;
                    }
                    break;
            }

            return true;
        }
        public static bool CheckOperation(List<refModule> listRefModule, int module, int function, int operation)
        {
            if (listRefModule[module].lstFunction == null)
            {
                return false;
            }
            if (listRefModule[module].lstFunction[function].lstOperation == null)
            {
                return false;
            }
            if (listRefModule[module].lstFunction[function].lstOperation[operation].mOperation == null)
            {
                return false;
            }

            return true;
        }
        public static bool CheckFunction(List<refModule> listRefModule, int module, int function)
        {
            if (listRefModule[module].lstFunction == null)
            {
                return false;
            }
            if (listRefModule[module].lstFunction.Count <= function || listRefModule[module].lstFunction[function].mFunction == null)
            {
                return false;
            }
            return true;
        }
        public static bool CheckModule(List<refModule> listRefModule, int module)
        {
            if (listRefModule == null)
            {
                return false;
            }
            if (listRefModule[module].mModule == null)
            {
                return false;
            }
            return true;
        }

        // Hpt 05/10/2015: danh sach cac thong bao tu he thong
        //public static List<AppWarningAndAction> ListAppWarningAndAction = null;

        // HPT 21/09/2015: Thêm hàm kiểm tra đăng ký Vãng Lai hoặc tiền Giải Phẫu có quá hạn chưa
        // Chi kiem tra qua han doi voi nhung dang ky loai vang lai hoac tien giai phau chua nhap vien
        public static bool Check_CasualAndPreOpReg_StillValid(PatientRegistration CurRegistration)
        {
            if (!Globals.IsCasuaOrPreOpPt(CurRegistration.V_RegForPatientOfType))
            {
                return false;
            }          

            TimeSpan NumOfDaysAfterRegis = Globals.GetCurServerDateTime() - CurRegistration.ExamDate;
            int Max_NumOfDaysAfterRegis = 0;
            // Tùy theo loại đăng ký mà số ngày tối đa cho phép một đăng ký chờ được phục vụ là khác nhau
            // Sử dụng câu lệnh if để set giá trị cho biến Max_NumOfDaysAfterRegis theo trường hợp cụ thể            
            // 1. Nếu là đăng ký vãng lai - NumOfDayAllowPending_CasualReg = 2 ngày (10/09/2015)
            // 2. Nếu là đăng ký tiền giải phẫu - NumOfDayAllowPending_PreOpReg = 45 ngày (10/09/2015)
            if (CurRegistration.V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.DKBN_VANG_LAI)
            {
                Max_NumOfDaysAfterRegis = Globals.ServerConfigSection.InRegisElements.NumOfDayAllowPending_CasualReg;
            }            
            else if (CurRegistration.V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.NBNT_BN_TIEN_PHAU_KHONG_BHYT)
            {
                Max_NumOfDaysAfterRegis = Globals.ServerConfigSection.InRegisElements.NumOfDayAllowPending_PreOpReg;
            }

            // Đăng ký đã quá hạn: là Đăng ký  có số ngày chờ (tính từ ngày đăng ký đến ngày hiện tại) lớn hơn số ngày được phép chờ tối đa của loại đăng ký đó
            // Nếu đăng ký đã quá hạn thì cảnh báo, yêu cầu người sử dung tạo đăng ký mới
            if (NumOfDaysAfterRegis.Days > Max_NumOfDaysAfterRegis)
            {
                return false;
            }
            return true;
        }

        public static bool IsCasuaOrPreOpPt(AllLookupValues.V_RegForPatientOfType V_RegForPatientOfType)
        {
            return (V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.DKBN_VANG_LAI || V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.NBNT_BN_TIEN_PHAU_CO_BHYT || V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.NBNT_BN_TIEN_PHAU_KHONG_BHYT);
        }
        public static bool IsRunningOutOfBrowser = false;
        public static bool isAccountCheck = true;
        public static bool isConsultationStateEdit = true;
        public static Visibility convertVisibility(bool value)
        {
            return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public static Visibility convertVisibilityNot(bool? value)
        {
            return value == null || (!(bool)value) ? Visibility.Visible : Visibility.Collapsed ;
        }

        public static string AxonKey = "AxonHealthCare1`";
        public static string AxonPass = "Tuan~Thien$Dao";

        // Txd 25/05/2014 Replaced ConfigList

        //public static int EffectedDiagHours = 48;
        //public static int EditDiagDays = 7;
        //public static int EffectedPCLHours=48;
        //public static long DiagRoomFunction = 4101;
        //public static int KhoaPhongKham = 95;
        //public static int LaboRmTp = 8;
        //public static bool IsConfirmHI = false;

        private static long _DrugDeptID = 120;
        public static long DrugDeptID 
        { 
            get { return _DrugDeptID; }
            set
            {
                _DrugDeptID = value;
            }
        }

        private static bool _addDrugDeptToListRespDepts = true;
        public static bool AddDrugDeptToListRespDepts
        {
            get { return _addDrugDeptToListRespDepts; }
            set
            {
                _addDrugDeptToListRespDepts = value;
            }
        }

        private static DataContracts.AxServerConfigSection _serverConfigSection;
        public static DataContracts.AxServerConfigSection ServerConfigSection
        {
            get
            {
                return _serverConfigSection;
            }
            set
            {
                _serverConfigSection = value;
            }
        }

        public static bool isEncrypt = true;

        public static IWindowManager m_AppWindowManager;

        public static AggregateCatalog AggregateCatalog { get; set; }

        private static EventAggregator _eventAggregator = new EventAggregator();

        public static EventAggregator EventAggregator
        {
            get
            {
                return _eventAggregator;
            }
        }

        public static T GetViewModel<T>()
        {
            return IoC.Get<T>();            
        }

        public static bool? IsAdmission = false;//dang o form quan li nhap vien hay nhap vien

        private static string _PageName;
        public static string PageName
        {
            get { return _PageName; }
            set
            {
                _PageName = value;
            }
        }

        private static string _ModuleName;
        public static string ModuleName
        {
            get { return _ModuleName; }
            set
            {
                _ModuleName = value;
            }
        }
        private static IEnumerator<IResult> LoadDynamicXap<T>(string uri, string message)
        {
            //yield return AsyncLoader.Show(message);

            // TxD 26/05/2018: Commented OUT the following 2 lines BECAUSE Silverlight is NO LONGER applicable.
            yield break;
            //yield return new AsyncDownloadXap(uri);
            //yield return new ShowScreen(typeof(T));
            //yield return AsyncLoader.Hide();
        }

        public static void LoadDynamicModule<T>(string xapUri, string busymessage = null, Action<object> callback = null)
        {
            Coroutine.BeginExecute(LoadDynamicXap<T>(xapUri, busymessage), null, (o, e) =>
                                                                                {
                                                                                    if (callback != null)
                                                                                    {
                                                                                        callback(e);
                                                                                    }
                                                                                });
        }

        //public static void LoadDynamicModule<T>(string xapUri, string busymessage = null)
        //{
        //    Coroutine.BeginExecute(LoadDynamicXap<T>(xapUri, busymessage));
        //}

        //public static void ShowDialog(Screen viewModel, Action<object> callback = null)
        //{
        //    m_AppWindowManager.ShowDialog(viewModel);
        //    //EventAggregator.Publish( new ShowDialogEvent { DialogViewModel = viewModel} );
        //}

        //public static void ShowDialog(Screen viewModel,bool hasCloseButton, Action<object> callback = null)
        //{
        //    m_AppWindowManager.ShowDialog(viewModel, hasCloseButton);
        //    //EventAggregator.Publish( new ShowDialogEvent { DialogViewModel = viewModel} );
        //}

        //public static void ShowDialogV2(Screen viewModel, System.Action onCloseCallback = null)
        //{
        //    m_AppWindowManager.ShowDialogV2(viewModel, null, onCloseCallback);            
        //}

        //Action<T> onInitialize, Action<MsgBoxOptions, IScreen> onClose 
        //    , MsgBoxOptions msgBoxOptions
        //    , Dictionary<string, object> settings

        // TxD 31/05/2018: The following ShowDialog method has been rewritten to work with the Navigation Service and new version of Caliburn Micro's ShowDialog
        //                  All Existing codes that were using the old method of ShowDialog above (commented out) will have to be modified accordingly
        public static void ShowDialog<VM_Interface_Type>(bool bHasCloseBtn = false, Action<MsgBoxOptions, IScreen> onCloseCallback = null, Action<VM_Interface_Type> onInitDialog = null)
        {
            INavigationService _navService = IoC.Get<INavigationService>();
            _navService.ShowDialog<VM_Interface_Type>(onInitDialog, onCloseCallback, MsgBoxOptions.None);
        }

        public static void ShowMessage(string message, string title, Action<object> callback = null)
        {
            EventAggregator.Publish(new ShowDialogEvent { Message = message, Title = title, Callback = callback });
        }


        public static List<Lookup> AllLookupValueList { get; set; }

        public static ObservableCollection<MedicalRecordTemplate> AllMedRecTemplates { get; set; }

        public static ObservableCollection<RefOutputType> RefOutputType { get; set; }

        public static List<PatientPaymentAccount> AllPatientPaymentAccounts { get; set; }

        public static UserAccount LoggedUserAccount { get; set; }

        public static UserAccount SecretaryLogin { get; set; }

        public static List<refModule> listRefModule { get; set; }

        public static DeptLocation DeptLocation { get; set; }

        public static List<Staff> AllStaffs { get; set; }

        //public static List<Staff> AllRegisStaff { get; set; }

        //public static List<Staff> AllDoctorStaff { get; set; }

        public static V_DeptTypeOperation V_DeptTypeOperation = V_DeptTypeOperation.KhoaNgoaiTru;

        public static List<long> allStaffStoreResponsibilities { get; set; }

        public static List<SuburbNames> allSuburbNames { get; set; }

        public static List<DeptTransferDocReq> allDeptTransDocTypeReq { get; set; }

        public static List<CitiesProvince> allCitiesProvince { get; set; }

        public static List<StaffPosition> allStaffPositions { get; set; }

        public static List<PrescriptionDetailSchedulesLieuDung> allPrescriptionDetailSchedulesLieuDung { get; set; }

        public static ObservableCollection<RefStorageWarehouseLocation> allRefStorageWarehouseLocation { get; set; }

        public static ObservableCollection<RefStorageWarehouseLocation> checkStoreWareHouse(bool _addSelectOneItem, bool _addSelectedAllItem, bool _isMedicineStore = true, bool _isUtilStore = true, bool _isChemicalStore = true)
        {
            if (allRefStorageWarehouseLocation == null)
            {
                return null;
            }

            ObservableCollection<RefStorageWarehouseLocation> temp = new ObservableCollection<RefStorageWarehouseLocation>();

            if (!isAccountCheck)
            {
                temp = ObjectCopier.DeepCopy(allRefStorageWarehouseLocation.Where(x => ((_isMedicineStore && x.IsMedicineStore) || (_isUtilStore && x.IsUtilStore) || (_isChemicalStore && x.IsChemicalStore))).ToObservableCollection());
            }
            else
            {
                if (allStaffStoreResponsibilities == null || allStaffStoreResponsibilities.Count <= 0)
                {
                    return null;
                }

                temp = ObjectCopier.DeepCopy(allRefStorageWarehouseLocation.Where(x => allStaffStoreResponsibilities.Any(y => y == x.StoreID) && ((_isMedicineStore && x.IsMedicineStore) || (_isUtilStore && x.IsUtilStore) || (_isChemicalStore && x.IsChemicalStore))).ToObservableCollection());
            }

            if (_addSelectOneItem)
            {
                RefStorageWarehouseLocation firstItem = new RefStorageWarehouseLocation();
                firstItem.StoreID = -1;
                firstItem.swhlName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0111_G1_HayChon1GTri);
                temp.Insert(0, firstItem);
            }
            if (_addSelectedAllItem)
            {
                RefStorageWarehouseLocation firstItem = new RefStorageWarehouseLocation();
                firstItem.StoreID = 0;
                firstItem.swhlName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                temp.Insert(0, firstItem);
            }
            return temp;
        }

        public static ObservableCollection<Hospital> allHospitals { get; set; }

        public static List<Lookup> GetAllLookupValuesByType(long lookupType, bool addSelectOneItem = true, bool addSelectedAllItem = false)
        {
            List<Lookup> listLookupVal = new List<Lookup>(AllLookupValueList.Where( Item => Item.ObjectTypeID == lookupType ).ToList());
                        
            if (addSelectOneItem)
            {
                Lookup firstItem = new Lookup();
                firstItem.LookupID = -1;
                firstItem.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0111_G1_HayChon1GTri);
                listLookupVal.Insert(0, firstItem);
            }
            else if (addSelectedAllItem)
            {
                Lookup firstItem = new Lookup();
                firstItem.LookupID = -2;
                firstItem.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                listLookupVal.Insert(0, firstItem);
            }

            return listLookupVal;
        }

        // Txd 25/05/2014 Replaced ConfigList
        // Commented out ConFigList. It's now replaced with ServerConfigSection

        //lay config dang crosstab        
        //private static IList<string> _configList;
        //public static IList<string> ConfigList
        //{
        //    get
        //    {
        //        return _configList;
        //    }
        //    set
        //    {
        //        _configList = value;
        //    }
        //}
        //

        //private static int _pageSize = 15;
        private static int _pageSize = 50;
        public static int PageSize
        {
            get { return _pageSize; }
        }


        private static HomeModuleActive _HomeModuleActive = HomeModuleActive.NONE;
        public static HomeModuleActive HomeModuleActive
        {
            get { return _HomeModuleActive; }
            set
            {
                if (_HomeModuleActive != value)
                {
                    _HomeModuleActive = value;
                    isConsultationStateEdit = false;
                    LeftModuleActive = LeftModuleActive.NONE;
                    switch (HomeModuleActive)
                    {
                        case HomeModuleActive.KHAMBENH:
                            Globals.curLstPatientServiceRecord = new ObservableCollection<PatientServiceRecord>();
                            break;
                        default: EventAggregator.Publish(new isConsultationStateEditEvent { isConsultationStateEdit = true });
                            break;
                    }
                }

            }
        }

        public static ObservableCollection<PrescriptionDetailSchedules> blankPrescriptionDetailSchedules { get; set; }

        private static LeftModuleActive _LeftModuleActive = LeftModuleActive.NONE;
        public static LeftModuleActive LeftModuleActive
        {
            get
            {
                return _LeftModuleActive;
            }

            set
            {
                _LeftModuleActive = value;
            }
        }

        public static AsyncCallback DispatchCallback(AsyncCallback callback)
        {
            var asyncUtility = new AsyncUtility();
            return asyncUtility.DispatchCallback(callback);
        }

        #region Patient

        //Dị Ứng/Cảnh Báo
        private static String _Allergies;
        public static String Allergies
        {
            get { return _Allergies; }
            set
            {
                if (_Allergies != value)
                {
                    _Allergies = value;
                }
            }
        }

        private static String _Warning;
        public static String Warning
        {
            get { return _Warning; }
            set
            {
                if (_Warning != value)
                {
                    _Warning = value;
                }
            }
        }

        //private static String _SystolicPressure;
        //public static String SystolicPressure
        //{
        //    get { return _SystolicPressure; }
        //    set
        //    {
        //        if (_SystolicPressure != value)
        //        {
        //            _SystolicPressure = value;
        //        }
        //    }
        //}
        
        //private static String _DiastolicPressure;
        //public static String DiastolicPressure
        //{
        //    get { return _DiastolicPressure; }
        //    set
        //    {
        //        if (_DiastolicPressure != value)
        //        {
        //            _DiastolicPressure = value;
        //        }
        //    }
        //}

        //private static String _Pulse;
        //public static String Pulse
        //{
        //    get { return _Pulse; }
        //    set
        //    {
        //        if (_Pulse != value)
        //        {
        //            _Pulse = value;
        //        }
        //    }
        //}

        private static PhysicalExamination _curPhysicalExamination;
        public static PhysicalExamination curPhysicalExamination
        {
            get { return _curPhysicalExamination; }
            set
            {
                if (_curPhysicalExamination != value)
                {
                    _curPhysicalExamination = value;
                }
            }
        }


        public static class PatientAllDetails
        {
            private static Patient _PatientInfo;
            public static Patient PatientInfo
            {
                get { return _PatientInfo; }
                set
                {
                    if (_PatientInfo != value)
                    {
                        _PatientInfo = value;
                    }
                }
            }

            private static PatientRegistration _PtRegistrationInfo;
            public static PatientRegistration PtRegistrationInfo
            {
                get { return _PtRegistrationInfo; }
                set
                {
                    if (_PtRegistrationInfo != value)
                    {
                        _PtRegistrationInfo = value;
                        PatientInfo = PtRegistrationInfo.Patient;
                    }
                }
            }

            private static PatientRegistrationDetail _PtRegistrationDetailInfo;
            public static PatientRegistrationDetail PtRegistrationDetailInfo
            {
                get { return _PtRegistrationDetailInfo; }
                set
                {
                    if (_PtRegistrationDetailInfo != value)
                    {
                        _PtRegistrationDetailInfo = value;
                    }
                }
            }

            private static DiagnosisTreatment _curDiagnosisTreatmentByPtDetailID;
            public static DiagnosisTreatment curDiagnosisTreatmentByPtDetailID
            {
                get
                {
                    return _curDiagnosisTreatmentByPtDetailID;
                }
                set
                {
                    if (_curDiagnosisTreatmentByPtDetailID != value)
                    {
                        _curDiagnosisTreatmentByPtDetailID = value;
                    }
                }
            }

            private static Prescription _curPrecriptionsByPtDetailID;
            public static Prescription curPrecriptionsByPtDetailID
            {
                get
                {
                    return _curPrecriptionsByPtDetailID;
                }
                set
                {
                    if (_curPrecriptionsByPtDetailID != value)
                    {
                        _curPrecriptionsByPtDetailID = value;
                    }
                }
            }

            private static ObservableCollection<PrescriptionIssueHistory> _allPrescriptionIssueHistory;
            public static ObservableCollection<PrescriptionIssueHistory> allPrescriptionIssueHistory
            {
                get
                {
                    return _allPrescriptionIssueHistory;
                }
                set
                {
                    if (_allPrescriptionIssueHistory != value)
                    {
                        _allPrescriptionIssueHistory = value;
                    }
                }
            }

            //static PatientAllDetails()
            //{
            //    PatientInfo=new Patient();
            //    RegistrationDetailInfo=new PatientRegistrationDetail();
            //    RegistrationInfo=new PatientRegistration();
            //}
        }

        //private static PatientRegistration _RegistrationInfo;
        //public static PatientRegistration RegistrationInfo
        //{
        //    get { return _RegistrationInfo; }
        //    set
        //    {
        //        if (_RegistrationInfo != value)
        //        {
        //            _RegistrationInfo = value;
        //        }
        //    }
        //}

        //private static PatientRegistrationDetail _RegistrationDetailInfo;
        //public static PatientRegistrationDetail RegistrationDetailInfo
        //{
        //    get { return _RegistrationDetailInfo; }
        //    set
        //    {
        //        if (_RegistrationDetailInfo != value)
        //        {
        //            _RegistrationDetailInfo = value;
        //        }
        //    }
        //}

        //KMx: Sử dụng trực tiếp Globals.PatientAllDetails.PtRegistrationInfo.HealthInsurance.HICardNo luôn (30/10/2014 09:23).
        //private static string _currentHealthInsuranceNo;
        //public static string CurrentHealthInsuranceNo
        //{
        //    get { return _currentHealthInsuranceNo; }
        //    set
        //    {
        //        if (_currentHealthInsuranceNo != value)
        //        {
        //            _currentHealthInsuranceNo = value;
        //        }
        //    }
        //}

        //private static bool _isOutPatient;
        //public static bool IsOutPatient
        //{
        //    get { return _isOutPatient; }
        //    set
        //    {
        //        if (_isOutPatient != value)
        //        {
        //            _isOutPatient = value;
        //        }
        //    }
        //}

        private static ObservableCollection<ContraIndicatorDrugsRelToMedCond> _allContraIndicatorDrugsRelToMedCond;
        public static ObservableCollection<ContraIndicatorDrugsRelToMedCond> allContraIndicatorDrugsRelToMedCond
        {
            get
            {
                return _allContraIndicatorDrugsRelToMedCond;
            }
            set
            {
                if (_allContraIndicatorDrugsRelToMedCond != value)
                {
                    _allContraIndicatorDrugsRelToMedCond = value;
                }
            }
        }

        public static bool CheckContrain(ObservableCollection<MedicalConditionRecord> PtMedCond, long DrugID)
        {
            if (allContraIndicatorDrugsRelToMedCond == null)
                return true;

            bool flag = false;
            string stContrain = "";
            if (PtMedCond != null)
            {
                foreach (var MedCond in PtMedCond)
                {
                    long mcType = (long)MedCond.RefMedicalCondition.MCTypeID;
                    foreach (var cDR in allContraIndicatorDrugsRelToMedCond)
                    {
                        if (cDR.MCTypeID == mcType && cDR.DrugID == DrugID)
                        {
                            flag = true;
                            stContrain = string.Format(eHCMSResources.Z1498_G1_Thuoc0CCDVoiDKienBenh1, cDR.RefGenericDrugDetail.BrandName, cDR.RefMedicalConditionType.MedConditionType);
                            break;
                        }
                    }
                }
                if (flag == true)
                {
                    Globals.ShowMessage(eHCMSResources.Z1547_G1_CCDVoiDKienBenh + stContrain, "");
                    return false;
                }
            }
            return true;

        }

        public static void SetInfoPatient(Patient P, PatientRegistration PR, PatientRegistrationDetail PRD)
        {
            if (P != null)
            {
                Globals.PatientAllDetails.PatientInfo = new Patient();
                Globals.PatientAllDetails.PatientInfo = P;
            }

            if (PR != null)
            {
                Globals.PatientAllDetails.PtRegistrationInfo = new PatientRegistration();
                Globals.PatientAllDetails.PtRegistrationInfo = PR;

                Globals.PatientAllDetails.PtRegistrationDetailInfo = new PatientRegistrationDetail();
                Globals.PatientAllDetails.PtRegistrationDetailInfo = PRD;


                //KMx: Sử dụng trực tiếp Globals.PatientAllDetails.PtRegistrationInfo.HealthInsurance.HICardNo để hiển thị thông tin thẻ BH bên khám bệnh, không set CurrentHealthInsuranceNo nữa, rườm rà (30/10/1014).
                //if (Globals.PatientAllDetails.PtRegistrationInfo != null)
                //{
                //    IsOutPatient = Globals.PatientAllDetails.PtRegistrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU;

                //    if (Globals.PatientAllDetails.PtRegistrationInfo.HealthInsurance != null)
                //    {
                //        CurrentHealthInsuranceNo = Globals.PatientAllDetails.PtRegistrationInfo.HealthInsurance.HICardNo;
                //    }
                //    else
                //    {
                //        CurrentHealthInsuranceNo = string.Empty;
                //    }
                //}
                //else
                //{
                //    IsOutPatient = false;
                //    CurrentHealthInsuranceNo = string.Empty;
                //}

            }
            else//BN Không Đăng Ký
            {
                PatientRegistration NotDangKy = new PatientRegistration();
                NotDangKy.Patient = P;
                NotDangKy.PtRegistrationID = 0;

                //Để lưu phiếu yc CLS
                NotDangKy.V_RegistrationType = AllLookupValues.RegistrationType.Unknown;
                NotDangKy.PatientClassification = new PatientClassification();
                NotDangKy.PatientClassification.PatientClassID = 9;
                NotDangKy.HisID = null;
                NotDangKy.RegistrationStatus = AllLookupValues.RegistrationStatus.OPENED;
                NotDangKy.PatientID = P != null ? P.PatientID : 0;
                NotDangKy.StaffID = LoggedUserAccount.StaffID;
                //Để lưu phiếu yc CLS

                Globals.PatientAllDetails.PtRegistrationDetailInfo = new PatientRegistrationDetail();
                Globals.PatientAllDetails.PtRegistrationDetailInfo.PtRegDetailID = 0;

                Globals.PatientAllDetails.PatientInfo = NotDangKy.Patient;

                Globals.PatientAllDetails.PtRegistrationInfo = NotDangKy;

                //CurrentHealthInsuranceNo = string.Empty;
                //IsOutPatient = false;
            }
        }

        public static void ClearPatientAllDetails()
        {
            Globals.PatientAllDetails.PatientInfo = new Patient();
            Globals.PatientAllDetails.PtRegistrationInfo = new PatientRegistration();
            Globals.PatientAllDetails.PtRegistrationDetailInfo = new PatientRegistrationDetail();
        }

        private static PatientMedicalRecord _PatientMedicalRecordInfo;
        public static PatientMedicalRecord PatientMedicalRecordInfo
        {
            get
            {
                return _PatientMedicalRecordInfo;
            }
            set
            {
                if (_PatientMedicalRecordInfo != value)
                {
                    _PatientMedicalRecordInfo = value;
                }
            }
        }

        private static ObservableCollection<PatientServiceRecord> _curLstPatientServiceRecord;
        public static ObservableCollection<PatientServiceRecord> curLstPatientServiceRecord
        {
            get
            {
                return _curLstPatientServiceRecord;
            }
            set
            {
                if (_curLstPatientServiceRecord != value)
                {
                    _curLstPatientServiceRecord = value;
                }
            }
        }

        #region Chuẩn đoán của 1 RegistrationDetail(ServiceRecID,PtRegDetailID), để khi qua Tab Phát hành lại Toa thì lấy đúng ServiceRecID và PtRegDetailID
        private static DiagnosisTreatment _ObjGetDiagnosisTreatmentByPtID;
        public static DiagnosisTreatment ObjGetDiagnosisTreatmentByPtID
        {
            get
            {
                return _ObjGetDiagnosisTreatmentByPtID;
            }
            set
            {
                if (_ObjGetDiagnosisTreatmentByPtID != value)
                {
                    _ObjGetDiagnosisTreatmentByPtID = value;
                }
            }
        }
        #endregion

        //Khám Bệnh trong ngữ cảnh là ChildWindow
        private static bool _ConsultationIsChildWindow;
        public static bool ConsultationIsChildWindow
        {
            get
            {
                return _ConsultationIsChildWindow;
            }
            set
            {
                if (_ConsultationIsChildWindow != value)
                {
                    _ConsultationIsChildWindow = value;
                }
            }
        }
        //Khám Bệnh trong ngữ cảnh là ChildWindow

        //Toa thuốc trong cảnh là ChildWindow
        private static bool _PrescriptionIsChildWindow;
        public static bool PrescriptionIsChildWindow
        {
            get
            {
                return _PrescriptionIsChildWindow;
            }
            set
            {
                if (_PrescriptionIsChildWindow != value)
                {
                    _PrescriptionIsChildWindow = value;
                }
            }
        }
        //Toa thuốc trong cảnh là ChldWindow


        private static DeptLocation _ObjLocation;
        public static DeptLocation ObjLocation
        {
            get { return _ObjLocation; }
            set
            {
                if (_ObjLocation != value)
                {
                    _ObjLocation = value;
                }
            }

        }

        // TxD 07/12/2014: Was NOT sure what the following was used for and found out it seemed to be used for the select department at login time
        private static RefDepartment _ObjRefDepartment;
        public static RefDepartment ObjRefDepartment
        {
            get { return _ObjRefDepartment; }
            set
            {
                if (_ObjRefDepartment != value)
                {
                    _ObjRefDepartment = value;
                }
            }

        }

        private static ObservableCollection<RefDepartment> _AllRefDepartmentList;
        public static ObservableCollection<RefDepartment> AllRefDepartmentList
        {
            get
            {
                return _AllRefDepartmentList;
            }
            set
            {
                _AllRefDepartmentList = value;
            }
        }


        //1 : chỉ 1 loại số không BH; 2: 2 loại số BH và không BH
        // Txd 25/05/2014 Replaced ConfigList
        //public static Int16 NumberTypePrescriptions_Rule { get; set; }
        //1 : chỉ 1 loại số không BH; 2: 2 loại số BH và không BH


        #region Check là đăng ký bảo hiểm
        public static bool IsPatientInsurance()
        {
            if (Globals.PatientAllDetails.PtRegistrationInfo != null)
            {
                if (Globals.PatientAllDetails.PtRegistrationInfo.HisID != null)
                    return true;
                return false;
            }
            return false;
        }
        #endregion


        #region For PCLDepartment
        public static class PCLDepartment
        {
            private static DeptLocation _ObjPCLExamTypeLocationsDeptLocationID;
            public static DeptLocation ObjPCLExamTypeLocationsDeptLocationID
            {
                get { return _ObjPCLExamTypeLocationsDeptLocationID; }
                set
                {
                    if (_ObjPCLExamTypeLocationsDeptLocationID != value)
                    {
                        _ObjPCLExamTypeLocationsDeptLocationID = value;
                    }
                }
            }

            private static Lookup _ObjV_PCLMainCategory;
            public static Lookup ObjV_PCLMainCategory
            {
                get { return _ObjV_PCLMainCategory; }
                set
                {
                    if (_ObjV_PCLMainCategory != value)
                    {
                        _ObjV_PCLMainCategory = value;
                    }
                }
            }

            private static PCLExamTypeSubCategory _ObjPCLExamTypeSubCategoryID;
            public static PCLExamTypeSubCategory ObjPCLExamTypeSubCategoryID
            {
                get { return _ObjPCLExamTypeSubCategoryID; }
                set
                {
                    if (_ObjPCLExamTypeSubCategoryID != value)
                    {
                        _ObjPCLExamTypeSubCategoryID = value;
                    }
                }
            }

            private static PCLResultParamImplementations _ObjPCLResultParamImpID;
            public static PCLResultParamImplementations ObjPCLResultParamImpID
            {
                get { return _ObjPCLResultParamImpID; }
                set
                {
                    if (_ObjPCLResultParamImpID != value)
                    {
                        _ObjPCLResultParamImpID = value;
                    }
                }
            }

            private static DateTime _PCLRequestFromDate;
            public static DateTime PCLRequestFromDate
            {
                get { return _PCLRequestFromDate; }
                set
                {
                    if (_PCLRequestFromDate != value)
                    {
                        _PCLRequestFromDate = value;
                    }
                }
            }

            private static DateTime _PCLRequestToDate;
            public static DateTime PCLRequestToDate
            {
                get { return _PCLRequestToDate; }
                set
                {
                    if (_PCLRequestToDate != value)
                    {
                        _PCLRequestToDate = value;
                    }
                }
            }

            public static void SetInfo(Patient ObjPatient, PatientRegistration ObjRegis, PatientRegistrationDetail ObjRegisDetail)
            {
                Globals.PatientAllDetails.PatientInfo = ObjPatient;
                Globals.PatientAllDetails.PtRegistrationInfo = ObjRegis;
                Globals.PatientAllDetails.PtRegistrationDetailInfo = ObjRegisDetail;
                Globals.EventAggregator.Publish(new PatientReloadEvent { curPatient = ObjPatient });
            }

        }
        #endregion

        public static string GetTextV_MedProductType(long V_MedProductType)
        {
            string Result = "";
            switch (V_MedProductType)
            {
                case 11001:
                    {
                        Result = eHCMSResources.G0787_G1_Thuoc;
                        break;
                    }
                case 11002:
                    {
                        Result = eHCMSResources.G2907_G1_YCu;
                        break;
                    }
                case 11003:
                    {
                        Result = eHCMSResources.T1616_G1_HC;
                        break;
                    }
            }
            return Result;
        }

        public static string GetTextV_RegistrationType(long V_RegistrationType)
        {
            string Result = "";
            switch (V_RegistrationType)
            {
                case 24001:
                    {
                        Result = eHCMSResources.T3719_G1_Mau20NgTru;
                        break;
                    }
                case 24002:
                    {
                        Result = eHCMSResources.Z1548_G1_NgTruChuyenSangNoiTru;
                        break;
                    }
                case 24003:
                    {
                        Result = eHCMSResources.T3713_G1_NoiTru;
                        break;
                    }
                case 24004:
                    {
                        Result = eHCMSResources.Z1549_G1_KhamVipKgDK;
                        break;
                    }
            }
            return Result;
        }

        public static Nullable<AllLookupValues.PatientFindBy> PatientFindBy_ForLab { get; set; }
        //public static Nullable<AllLookupValues.PatientFindBy> PatientFindBy_ForConsultation { get; set; }

        private static Nullable<AllLookupValues.PatientFindBy> _PatientFindBy_ForConsultation;
        public static Nullable<AllLookupValues.PatientFindBy> PatientFindBy_ForConsultation
        {
            get { return _PatientFindBy_ForConsultation; }
            set
            {
                _PatientFindBy_ForConsultation = value;
            }
        }

        public static Nullable<AllLookupValues.PatientFindBy> PatientFindBy_ForImaging { get; set; }

        public static long PatientPCLReqID_Imaging { get; set; }
        public static PatientPCLRequest PatientPCLRequest_Imaging { get; set; }
        public static long PatientPCLReqID_LAB { get; set; }
        public static PatientPCLRequest PatientPCLRequest_LAB { get; set; }
        public static PatientPCLRequest PatientPCLRequest_Result { get; set; }
        #endregion

        private static int _updSvrDateTimerCnt;
        public static int UpdSvrDateTimerCnt
        {
            get { return _updSvrDateTimerCnt; }
            set
            {
                _updSvrDateTimerCnt = value;
            }
        }

        public static DispatcherTimer UpdSvrDateTimer
        {
            get;
            set;
        }

        private static object lockObj = new object();
        private static DateTime? _ServerDate;
        public static DateTime? ServerDate 
        {
            get
            {
                lock (lockObj)
                {
                    return _ServerDate;
                }
            }

            set
            {
                lock (lockObj)
                {
                    _ServerDate = value;
                }
            }
        }

        public static DateTime GetCurServerDateTime()
        {
            if (ServerDate != null && ServerDate.HasValue)
                return ServerDate.Value;

            return DateTime.Now;
        }

        #region Các Danh Mục Load Sẵn
        public static ObservableCollection<PCLExamType> ListPclExamTypesAllPCLForms { get; set; }

        public static ObservableCollection<PCLExamType> ListPclExamTypesAllPCLFormImages { get; set; }

        public static ObservableCollection<PCLExamTypeComboItem> ListPclExamTypesAllCombos { get; set; }

        public static Dictionary<long, PCLExamType> MAPPCLExamTypeDeptLoc { get; set; }

        private static ObservableCollection<ConsultationRoomTarget> _ConsultationRoomTarget;
        public static ObservableCollection<ConsultationRoomTarget> ConsultationRoomTarget
        {
            get { return _ConsultationRoomTarget; }
            set
            {
                _ConsultationRoomTarget = value;
            }
        }

        #endregion

        // Txd 25/05/2014 Replaced ConfigList
        //public static int FindRegistrationInDays_NgoaiTru { get; set; }

        private static string _TitleForm;
        public static string TitleForm
        {
            get { return _TitleForm; }
            set
            {
                _TitleForm = value;
            }
        }

        private static string _HIRegistrationForm;
        public static string HIRegistrationForm
        {
            get { return _HIRegistrationForm; }
            set
            {
                _HIRegistrationForm = value;
            }
        }
        public static MessageBoxTask msgb = null;
        public static IEnumerator<IResult> DoMessageBox()
        {
            msgb = new MessageBoxTask(string.Format(eHCMSResources.Z0465_G1_BanCoMuonQuaTrangHayKg, TitleForm), eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
            yield return msgb;
            yield break;
        }
        public static IEnumerator<IResult> DoMessageBoxHIRegis()
        {
            msgb = new MessageBoxTask(HIRegistrationForm, eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
            yield return msgb;
            yield break;
        }

        public static bool OOB_SoftwareUpdatedReqRestart = false;       // Out Of Browser Software has just been updated thus require the User to restart the software

        private static bool _isBusy;
        public static bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;

                EventAggregator.Publish(new BusyEvent { IsBusy = _isBusy });
            }
        }


        private static int _nBusyIndicatorCnt = 0;

        public static void ShowBusyIndicator<GEN_VM>(this GEN_VM genVM, string strBusyText = "")
        {
            System.Threading.Interlocked.Increment(ref _nBusyIndicatorCnt);
            if (_nBusyIndicatorCnt == 1)
            {
                var shellVM = Globals.GetViewModel<IShell>();
                shellVM.ShowBusy(strBusyText);
            }
        }

        public static void HideBusyIndicator<GEN_VM>(this GEN_VM genVM)
        {
            System.Threading.Interlocked.Decrement(ref _nBusyIndicatorCnt);
            if (_nBusyIndicatorCnt <= 0)
            {
                _nBusyIndicatorCnt = 0;
                var shellVM = Globals.GetViewModel<IShell>();
                shellVM.HideBusy();
            }
        }


        public static string GetDistrictNameFromHICardNo(string HICardNo)
        {
            string res = "";
            if (string.IsNullOrEmpty(HICardNo) || HICardNo.Length < 5)
            {
                return "";
            }

            res = (from item in Globals.allCitiesProvince
                   where //string.Equals(item.CityProviceHICode.ToString(), HICode.Substring(0,2))
                            item.CityProviceHICode.ToString().Trim() == HICardNo.Substring(3, 2)
                   select item.CityProvinceName).FirstOrDefault();

            return res;
        }

        public static string GetDistrictNameFromHICode(string HICode)
        {
            string res = "";
            if (string.IsNullOrEmpty(HICode))
            {
                return "";
            }

            res = (from item in Globals.allCitiesProvince
                   where //string.Equals(item.CityProviceHICode.ToString(), HICode.Substring(0,2))
                            item.CityProviceHICode.ToString().Trim() == HICode.Substring(0, 2)
                   select item.CityProvinceName).FirstOrDefault();

            return res;
        }

        // TxD 31/05/2018 Commented OUT because it's NOT USED anywhere
        //public static object ShowWarningAndConfirmDlg(string strMsg, string strConfirm, System.Action<bool> onCloseCallback)
        //{
        //    var dlgConf = IoC.Get<IErrorBold>();

        //    if (strConfirm.Length > 0)
        //    {
        //        dlgConf.isCheckBox = true;
        //        dlgConf.SetMessage(strMsg, strConfirm);
        //    }
        //    else
        //    {
        //        dlgConf.SetMessage(strMsg, "");
        //    }

        //    if (onCloseCallback != null)
        //    {
        //        dlgConf.SetDeActivateCallback(onCloseCallback);
        //    }

        //    var instance = dlgConf as Conductor<object>;

        //    m_AppWindowManager.ShowDialog(instance, false);

        //    return dlgConf;

        //}

        public static byte GetDayOfWeek(DateTime startDate)
        {
            switch (startDate.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return 0;
                case DayOfWeek.Tuesday:
                    return 1;
                case DayOfWeek.Wednesday:
                    return 2;
                case DayOfWeek.Thursday:
                    return 3;
                case DayOfWeek.Friday:
                    return 4;
                case DayOfWeek.Saturday:
                    return 5;
                case DayOfWeek.Sunday:
                    return 6;
                default:
                    return 0;
            }
        }


        public static float CalcWeeklySchedulePrescription(byte? DayOfWeek, int nNumDayPrescribed, float[] weeklySchedule, float dispenseVolume)
        {
            //KMx: DayOfWeek: Thứ 2 = 0, từ đó tăng lên (05/06/2014 17:36).
            if (DayOfWeek == null)
                return 0;

            if (nNumDayPrescribed <= 0)
                return 0;
            
            int nDayCnt = 0;
            float fTotalQty = 0;
            while (nDayCnt < nNumDayPrescribed)
            {
                for (int nIdx = 0; nIdx < 7; ++nIdx)
                {
                    if (nDayCnt == 0)
                    {
                        nIdx = DayOfWeek.GetValueOrDefault();
                    }
                    fTotalQty += weeklySchedule[nIdx];
                    ++nDayCnt;
                    if (nDayCnt >= nNumDayPrescribed)
                    {
                        //KMx: Phải Round trước rồi mới Ceiling sau, nếu không sẽ bị sai trong trường hợp kết quả có nhiều số lẻ. VD: 5.00001
                        return (float)Math.Ceiling(Math.Round(fTotalQty / (dispenseVolume <= 0 ? 1 : dispenseVolume), 2));
                    }
                }
            }
            return 0;
        }

        #region Định dạng code khi tìm kiếm (dành cho nội trú) (29/01/2015 11:48).
        public static string FormatCode(long V_MedProductType, string TextInput)
        {
            string Code = "";

            if (TextInput == null || TextInput.Length <= 0)
            {
                return Code;
            }

            string prefix_med = Globals.ServerConfigSection.MedDeptElements.PrefixCodeMedical; //3 ký tự đầu của Thuốc
            string prefix_mat = Globals.ServerConfigSection.MedDeptElements.PrefixCodeMachine; //3 ký tự đầu của Y Cụ
            string prefix_lab = Globals.ServerConfigSection.MedDeptElements.PrefixCodeChemical; //3 ký tự đầu của Hóa Chất

            int postfixlen_med = 4; //4 ký tự sau của Thuốc
            int postfixlen_mat = 5; //5 ký tự sau của Y Cụ
            int postfixlen_lab = 4; //4 ký tự sau của Hóa Chất

            int InputLen = TextInput.Length;

            if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            {
                if (InputLen < postfixlen_med)
                {
                    Code = prefix_med + new string('0', postfixlen_med - InputLen) + TextInput;
                }
                else if (InputLen == postfixlen_med)
                {
                    Code = prefix_med + TextInput;
                }
                else if (InputLen > postfixlen_med)
                {
                    Code = TextInput;
                }
            }
            else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
            {
                if (InputLen < postfixlen_mat)
                {
                    Code = prefix_mat + new string('0', postfixlen_mat - InputLen) + TextInput;
                }
                else if (InputLen == postfixlen_mat)
                {
                    Code = prefix_mat + TextInput;
                }
                else if (InputLen > postfixlen_mat)
                {
                    Code = TextInput;
                }
            }
            else if (V_MedProductType == (long)AllLookupValues.MedProductType.HOA_CHAT)
            {
                if (InputLen < postfixlen_lab)
                {
                    Code = prefix_lab + new string('0', postfixlen_lab - InputLen) + TextInput;
                }
                else if (InputLen == postfixlen_lab)
                {
                    Code = prefix_lab + TextInput;
                }
                else if (InputLen > postfixlen_lab)
                {
                    Code = TextInput;
                }
            }

            return Code;
        }
        #endregion

        #region KMx: Tính liều dùng ra số lượng (dành cho nội trú). Tất cả các hàm tính liều dùng được copy và chỉnh sửa từ "Ra toa" (16/04/2016 10:40).

        private static float CalDosageToQty(IDosage item, bool CalByUnitUse = false)
        {
            double dispenseVolume = 0;

            if (item is RefGenMedProductDetails)
            {
                dispenseVolume = ((RefGenMedProductDetails)item).DispenseVolume;
            }
            else if (item is OutwardDrugClinicDept)
            {
                if (((OutwardDrugClinicDept)item).GenMedProductItem == null)
                {
                    return 0;
                }

                dispenseVolume = ((OutwardDrugClinicDept)item).GenMedProductItem.DispenseVolume;
            }
            else if (item is ReqOutwardDrugClinicDeptPatient)
            {
                if (((ReqOutwardDrugClinicDeptPatient)item).RefGenericDrugDetail == null)
                {
                    return 0;
                }

                dispenseVolume = ((ReqOutwardDrugClinicDeptPatient)item).RefGenericDrugDetail.DispenseVolume;
            }

            float QtyAllDose = 0;
            float Result = 0;

            QtyAllDose = item.MDose + item.ADose + item.NDose + item.EDose;

            //▼====: #002
            if (!CalByUnitUse) dispenseVolume = 1;
            //▲====: #002

            //KMx: Phải nhân trước rồi chia sau để hạn chế kết quả có số lẻ (06/11/2014 11:11).
            Result = QtyAllDose / ((float)dispenseVolume == 0 ? 1 : (float)dispenseVolume);

            //KMx: Phải Round trước rồi mới Ceiling sau, nếu không sẽ bị sai trong trường hợp kết quả có nhiều số lẻ. VD: 5.00001
            //▼====: #003
            //return (float)Math.Round(Result, 2);
            return (float)Math.Round(Result, 3);
            //▲====: #003
        }


        public static void ChangeAnyDoseQty(AllLookupValues.Dosage nDoseType, IDosage item, bool CalByUnitUse = false)
        {
            if (item == null)
            {
                return;
            }

            switch (nDoseType)
            {
                case AllLookupValues.Dosage.MDose:
                    item.MDose = ChangeDoseStringToFloat(item.MDoseStr);
                    if (item.MDose <= 0)
                    {
                        item.MDoseStr = "0";
                    }
                    break;
                case AllLookupValues.Dosage.ADose:
                    item.ADose = ChangeDoseStringToFloat(item.ADoseStr);
                    if (item.ADose <= 0)
                    {
                        item.ADoseStr = "0";
                    }
                    break;
                case AllLookupValues.Dosage.EDose:
                    item.EDose = ChangeDoseStringToFloat(item.EDoseStr);
                    if (item.EDose <= 0)
                    {
                        item.EDoseStr = "0";
                    }
                    break;
                case AllLookupValues.Dosage.NDose:
                    item.NDose = ChangeDoseStringToFloat(item.NDoseStr);
                    if (item.NDose <= 0)
                    {
                        item.NDoseStr = "0";
                    }
                    break;
            }

            if (item is RefGenMedProductDetails)
            {
                ((RefGenMedProductDetails)item).RequiredNumber = Convert.ToDecimal(CalDosageToQty(item, CalByUnitUse));
            }
            else if (item is OutwardDrugClinicDept)
            {
                ((OutwardDrugClinicDept)item).OutQuantity = Convert.ToDecimal(CalDosageToQty(item, CalByUnitUse));
            }
            else if (item is ReqOutwardDrugClinicDeptPatient)
            {
                ((ReqOutwardDrugClinicDeptPatient)item).ReqQty = Convert.ToDecimal(CalDosageToQty(item, CalByUnitUse));
                ((ReqOutwardDrugClinicDeptPatient)item).PrescribedQty = ((ReqOutwardDrugClinicDeptPatient)item).ReqQty;
            }

        }


        //KMx: Hàm này được copy từ ra toa và chưa được kiểm tra (11/04/2016 16:57).
        private static float ChangeDoseStringToFloat(string value)
        {
            float result = 0;
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Contains("/"))
                {
                    string pattern = @"\b[\d]+/[\d]+\b";
                    if (!Regex.IsMatch(value, pattern))
                    {
                        Globals.ShowMessage(eHCMSResources.Z1069_G1_LieuDungKgHopLe, eHCMSResources.G0442_G1_TBao);
                        return 0;
                    }
                    else
                    {
                        string[] items = null;
                        items = value.Split('/');
                        if (items.Count() > 2 || items.Count() == 0)
                        {
                            Globals.ShowMessage(eHCMSResources.Z1069_G1_LieuDungKgHopLe, eHCMSResources.G0442_G1_TBao);
                            return 0;
                        }
                        else if (float.Parse(items[1]) == 0)
                        {
                            Globals.ShowMessage(eHCMSResources.Z1069_G1_LieuDungKgHopLe, eHCMSResources.G0442_G1_TBao);
                            return 0;
                        }

                        //KMx: Không được Round số lượng. Nếu không sẽ bị sai trong trường hợp thuốc 1/7 viên * 35 ngày.
                        //Kết quả không Round là 5, kết quả sau khi Round là 6.
                        //result = (float)Math.Round((float.Parse(items[0]) / float.Parse(items[1])), 3);

                        result = (float.Parse(items[0]) / float.Parse(items[1]));

                        if (result < 0)
                        {
                            Globals.ShowMessage(eHCMSResources.Z1071_G1_LieuDungKgNhoHon0, eHCMSResources.G0442_G1_TBao);
                            return 0;
                        }
                    }
                }
                else
                {
                    try
                    {
                        result = float.Parse(value);
                        if (result < 0)
                        {
                            Globals.ShowMessage(eHCMSResources.Z1071_G1_LieuDungKgNhoHon0, eHCMSResources.G0442_G1_TBao);
                            return 0;
                        }
                    }
                    catch
                    {
                        Globals.ShowMessage(eHCMSResources.Z1069_G1_LieuDungKgHopLe, eHCMSResources.G0442_G1_TBao);
                        return 0;
                    }
                }
            }
            return result;
        }

        #endregion

        //HPT: Hàm kiểm tra đăng ký có bị lock không?
        // true = Có lock và ngược lại
        public static bool IsLockRegistration(int RegLockFlag, string StrAction)
        {
            //HPT_20160619: Nếu cấu hình off thì không lock, return kết quả luôn không cần kiểm tra nữa 
            if (Globals.ServerConfigSection.InRegisElements.CheckToLockReportedRegistration <= 0)
            {
                return false;
            }
            if (RegLockFlag <= 0)
            {
                return false;
            }
            if (string.IsNullOrEmpty(StrAction) || string.IsNullOrWhiteSpace(StrAction))
            {
                StrAction = eHCMSResources.Z1428_G1_ChucNangNay;
            }
            MessageBox.Show(string.Format(eHCMSResources.Z1429_G1_DKDaDcBC, StrAction.Trim()));
            return true;
        }

        public static bool CanRegHIChildUnder6YearsOld(int Age)
        {
            DateTime Todate = new DateTime(GetCurServerDateTime().Year, 9, 30);
            if (Age < 6 && GetCurServerDateTime().Date <= Todate)
            {
                return true;
            }
            return false;
        }

        // HPT_20160618: Biến toàn cục lưu danh sách các tổ chức từ thiện.
        // Hàm load danh sách này được gọi khi mở màn hình Quyết toán, sau đó được gán vào Globals. Nếu kiểm tra trong Globals đã có dữ liệu thì không cần đi load lại
        private static ObservableCollection<CharityOrganization> _CharityOrganization;
        public static ObservableCollection<CharityOrganization> CharityOrganization
        {
            get
            {
                return _CharityOrganization;
            }
            set
            {
                _CharityOrganization = value;
            }
        }

        //HPT: danh sách bác sỹ
        private static ObservableCollection<Staff> _DoctorStaffs;
        public static ObservableCollection<Staff> DoctorStaffs
        {
            get
            {
                return _DoctorStaffs;
            }
            set
            {
                _DoctorStaffs = value;
            }
        }

        public static int ConvertStringToInt(string strNumber)
        {
            int value = 0;
            int.TryParse(strNumber, out value);
            return value;
        }

        public static byte[] ConvertStreamToByteArray(Stream inputStream)
        {
            if (inputStream is MemoryStream)
            {
                return ((MemoryStream)inputStream).ToArray();
            }
            else
            {
                byte[] buffer = new byte[16 * 1024];
                using (MemoryStream ms = new MemoryStream())
                {
                    int read;
                    while ((read = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                    return ms.ToArray();
                }
            }
        }

        //==== 20161008 CMN Begin: Change Image View to WriteableBitmap
        //public static WriteableBitmap GetWriteableBitmapFromStream(Stream pStream)
        //{
        //    pStream.Position = 0;
        //    ImageTools.Image mImage = new ImageTools.Image();
        //    BmpDecoder mDecoder = new BmpDecoder();
        //    WriteableBitmap mBitmap;
        //    try
        //    {
        //        mDecoder.Decode(mImage, pStream);
        //        mBitmap = ImageTools.ImageExtensions.ToBitmap(mImage);
        //    }
        //    catch
        //    {
        //        System.Windows.Media.Imaging.BitmapImage imgsource = new System.Windows.Media.Imaging.BitmapImage();
        //        imgsource.SetSource(pStream);
        //        mBitmap = new WriteableBitmap(imgsource);
        //    }
        //    return mBitmap;
        //}

        // TxD 31/05/2018 : Rewrite the following Method (commented OUT above) to WORK for WPF because ImageTool NO LONGER working under WPF
        //                  TO BE REVIEWED if NOT WORKING properly.
        public static WriteableBitmap GetWriteableBitmapFromStream(Stream pStream)
        {            
            BitmapImage theBitmapImage = new BitmapImage();
            theBitmapImage.BeginInit();
            theBitmapImage.StreamSource = pStream;
            theBitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            theBitmapImage.EndInit();
            theBitmapImage.Freeze();            
            return new WriteableBitmap(theBitmapImage);
        }

        private static ObservableCollection<Hospital> _CrossRegionHospital;
        public static ObservableCollection<Hospital> CrossRegionHospital
        {
            get
            {
                return _CrossRegionHospital;
            }
            set
            {
                _CrossRegionHospital = value;
            }
        }

        public static bool CheckAllowToCrossRegion(HealthInsurance ConfirmHIItem, AllLookupValues.RegistrationType V_RegistrationType)
        {
            if ((V_RegistrationType != AllLookupValues.RegistrationType.NGOAI_TRU && V_RegistrationType != AllLookupValues.RegistrationType.NOI_TRU)
                || (V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU && !Globals.ServerConfigSection.HealthInsurances.AllowOutPtCrossRegion)
                || (V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU && !Globals.ServerConfigSection.HealthInsurances.AllowInPtCrossRegion))
            {
                return false;
            }
            if (ConfirmHIItem == null)
            {
                return false;
            }
            if (CrossRegionHospital == null || CrossRegionHospital.Count() <= 0)
            {
                return false;
            }
            return CrossRegionHospital.Any(x => x.HosID == ConfirmHIItem.HosID);
        }
        //==== 20161008 CMN End.
        //==== #001
        public static string HexToString(string aInputString)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < aInputString.Length; i += 2)
                {
                    string hs = aInputString.Substring(i, 2);
                    sb.Append(Convert.ToChar(Convert.ToUInt32(hs, 16)));
                }
                return Utf8ToUnicode(sb.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static string Utf8ToUnicode(string aInputString)
        {
            try
            {
                byte[] utf8Bytes = new byte[aInputString.Length];
                for (int i = 0; i < aInputString.Length; ++i)
                {
                    utf8Bytes[i] = (byte)aInputString[i];
                }
                return Encoding.UTF8.GetString(utf8Bytes, 0, utf8Bytes.Length);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==== #001
        //Xóa dấu tiếng việt
        private static readonly string[] VietNamCharArray = new string[] {
            "aAeEoOuUiIdDyY",
            "áàạảãâấầậẩẫăắằặẳẵ",
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ",
            "óòọỏõôốồộổỗơớờợởỡ",
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "úùụủũưứừựửữ",
            "ÚÙỤỦŨƯỨỪỰỬỮ",
            "íìịỉĩ",
            "ÍÌỊỈĨ",
            "đ",
            "Đ",
            "ýỳỵỷỹ",
            "ÝỲỴỶỸ"
        };
        public static string RemoveVietnameseString(string aInputString)
        {
            if (string.IsNullOrEmpty(aInputString)) return "";
            for (int i = 1; i < VietNamCharArray.Length; i++)
            {
                for (int j = 0; j < VietNamCharArray[i].Length; j++)
                    aInputString = aInputString.Replace(VietNamCharArray[i][j], VietNamCharArray[0][i - 1]);
            }
            return aInputString;
        }
        public static DateTime ApplyValidMedicalInstructionDate(DateTime aMedicalInstructionDate, PatientRegistration aPatientRegistration)
        {
            aMedicalInstructionDate = aMedicalInstructionDate.Date;
            if (aPatientRegistration == null) return aMedicalInstructionDate;
            if (aPatientRegistration.AdmissionInfo != null
                && aPatientRegistration.AdmissionInfo.AdmissionDate.HasValue && aPatientRegistration.AdmissionInfo.AdmissionDate != null
                && aMedicalInstructionDate < aPatientRegistration.AdmissionInfo.AdmissionDate)
                aMedicalInstructionDate = aMedicalInstructionDate.Date.Add(TimeSpan.Parse(aPatientRegistration.AdmissionInfo.AdmissionDate.Value.AddSeconds(1).ToString("HH:mm:ss")));
            else if (aPatientRegistration.AdmissionDate.HasValue && aPatientRegistration.AdmissionDate != null
                && aMedicalInstructionDate < aPatientRegistration.AdmissionDate)
                aMedicalInstructionDate = aMedicalInstructionDate.Date.Add(TimeSpan.Parse(aPatientRegistration.AdmissionDate.Value.AddSeconds(1).ToString("HH:mm:ss")));
            else if ((aPatientRegistration.AdmissionInfo == null || !aPatientRegistration.AdmissionInfo.AdmissionDate.HasValue || aPatientRegistration.AdmissionInfo.AdmissionDate == null)
                && aPatientRegistration.ExamDate != null
                && aMedicalInstructionDate < aPatientRegistration.ExamDate)
                aMedicalInstructionDate = aMedicalInstructionDate.Date.Add(TimeSpan.Parse(aPatientRegistration.ExamDate.AddSeconds(1).ToString("HH:mm:ss")));
            return aMedicalInstructionDate;
        }
    }
    // TxD 23/08/2014: Moved the following MathExt to eHCMSCommon project
    //public enum MidpointRounding
    //{
    //    ToEven,
    //    AwayFromZero
    //}

    //public static class MathExt
    //{
    //    public static decimal Round(decimal d, MidpointRounding mode)
    //    {
    //        return MathExt.Round(d, 0, mode);
    //    }

    //    //MidpointRounding = ToEven : 1.x nếu x <= .5 thì làm tròn xuống. Nhưng có 1 số trường hợp .5 lại làm tròn lên, không nên sử dụng cái này.
    //    //MidpointRounding = AwayFromZero: 1.x nếu x >= .5 thì làm tròn lên. Đã test và sử dụng được (22/08/2014 10:04).
    //    public static decimal Round(decimal d, int decimals, MidpointRounding mode)
    //    {
    //        if (mode == MidpointRounding.ToEven)
    //        {
    //            return decimal.Round(d, decimals);
    //        }
    //        else
    //        {
    //            decimal factor = Convert.ToDecimal(Math.Pow(10, decimals));
    //            int sign = Math.Sign(d);
    //            return Decimal.Truncate(d * factor + 0.5m * sign) / factor;
    //        }
    //    }
    //}
}
