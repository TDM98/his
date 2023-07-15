using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataEntities;
using System.Data.SqlClient;
using System.Data;

using System.Reflection;
using eHCMS.Configurations;
using System.Collections.ObjectModel;
using eHCMS.DAL;



#region
/*******************************************************************
 * Author: NhanLe
 * Modified date: 2010-10-13
 * Contents: Consultation Services
/*******************************************************************/
#endregion

/*
 * 20180922 #001 TBL: BM 0000073. Them parameter List<string> listICD10Codes cho GetDrugsInTreatmentRegimen
 * 20181004 #002 TTM: BM 0000138: Thêm hàm chỉ lấy chi tiết toa thuốc (không đầy đủ, trả về là string).
 * 20181012 #003 TTM: 
 */
namespace eHCMS.DAL
{
    public abstract class ePrescriptionsProvider : DataProviderBase
    {
        static private ePrescriptionsProvider _instance = null;
        /// <summary>
        /// Returns an instance of the provider type specified in the config file (using reflection)
        /// </summary>
        static public ePrescriptionsProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    string tempPath = AppDomain.CurrentDomain.RelativeSearchPath;
                    if (string.IsNullOrEmpty(AppDomain.CurrentDomain.RelativeSearchPath))
                        tempPath = AppDomain.CurrentDomain.BaseDirectory;
                    else
                        tempPath = AppDomain.CurrentDomain.RelativeSearchPath;
                    string assemblyPath = System.IO.Path.Combine(tempPath, Globals.Settings.Consultations.Assembly + ".dll");
                    Assembly assem = Assembly.LoadFrom(assemblyPath);
                    Type t = assem.GetType(Globals.Settings.Consultations.ePrescription.ProviderType);
                    _instance = (ePrescriptionsProvider)Activator.CreateInstance(t);
                }
                return _instance;
            }
        }

        public ePrescriptionsProvider()
        {
            this.ConnectionString = Globals.Settings.Consultations.ConnectionString;
        }
        #region 0. Common
        public abstract List<Lookup> GetLookupPresriptionType();
        #endregion

        #region 1.Prescription

        public abstract IList<Prescription> GetAllPrescriptions();
        public abstract IList<Prescription> GetPrescriptionByPtID(long patientID, bool latest);

        public abstract IList<Prescription> GetPrescriptionByPtID_Paging(long patientID, long? V_PrescriptionType, bool isInPatient, int PageIndex, int PageSize, out int TotalCount);
        public abstract IList<Prescription> GetPrescriptionByID(long PrescriptID);

        public abstract IList<Prescription> GetPrescriptionByServiceRecID(long patientID, long? ServiecRecID, long? PtRegistrationID, DateTime? ExamDate);

        public abstract Prescription GetLatestPrescriptionByPtID(long patientID);
        public abstract Prescription GetLatestPrescriptionByPtID_InPt(long patientID);
        public abstract Prescription GetNewPrescriptionByPtID(long patientID, long doctorID);
        public abstract Prescription GetPrescriptionID(long PrescriptID);
        public abstract Prescription GetPrescriptionByIssueID(long IssueID);
        
        //▼====== #002
        public abstract string GetPrescriptDetailsStr_FromPrescriptID(long PrescriptID);
        //▲====== #002
        public abstract Prescription Prescription_ByPrescriptIDIssueID(Int64 PrescriptID, Int64 IssueID);

        public abstract IList<PrescriptionIssueHistory> GetPrescriptionIssueHistory(long PrescriptID);
        public abstract IList<PrescriptionIssueHistory> GetPrescriptionIssueHistoryBySerRecID(long ServiceRecID);
        public abstract IList<PrescriptionIssueHistory> PrescIssueHistoryByPtRegisID(long PtRegistrationID, bool IsHI = true);

        public abstract IList<Prescription> SearchPrescription(PrescriptionSearchCriteria Criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        public abstract IList<Prescription> Prescription_Seach_WithIsSoldIssueID(PrescriptionSearchCriteria Criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        public abstract bool DeletePrescription(Prescription entity);

        public abstract bool Prescriptions_Update(Prescription entity, Prescription entity_OLD, bool AllowUpdateThoughReturnDrugNotEnough, out string Result, out long NewPrescriptID, out long IssueID);

        public abstract void Prescriptions_InPt_Update(Prescription entity, Prescription entity_OLD, bool AllowUpdateThoughReturnDrugNotEnough, out string Result, out long NewPrescriptID, out long IssueID);


        public abstract void Prescriptions_UpdateDoctorAdvice(Prescription entity, out string Result);


        public abstract bool Prescriptions_Add(Int16 NumberTypePrescriptions_Rule, Prescription entity, out long newPrescriptID, out long IssueID, out string OutError);

        public abstract bool Prescriptions_InPt_Add(Prescription entity, out long newPrescriptID, out long IssueID, out string OutError);

        public abstract bool Prescriptions_DuocSiEdit(Prescription entity, Prescription entity_BacSi, out long newPrescriptID, out long IssueID, out string OutError);

        public abstract bool Prescriptions_DuocSiEditDuocSi(Prescription entity, Prescription entity_DuocSi, out long newPrescriptID, out long IssueID, out string OutError);


        public abstract List<Prescription> Prescriptions_TrongNgay_ByPatientID(Int64 PatientID);


        public abstract List<Prescription> Prescriptions_ListRootByPatientID_Paging(
            PrescriptionSearchCriteria SearchCriteria,

            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            out int Total);


        #endregion

        #region 2.GetDrugForPrescription

        public abstract IList<GetDrugForSellVisitor> GetDrugForPrescription_Auto(String BrandName, int IsInsurance, int PageSize, int PageIndex, int IsMedDept, long? StoreID);

        public abstract IList<GetDrugForSellVisitor> SearchDrugForPrescription_Paging(String BrandName, bool IsSearchByGenericName, int IsInsurance, long? StoreID, int PageIndex, int PageSize, bool CountTotal, out int Total);
        /*▼====: #001*/
        //public abstract IList<GetDrugForSellVisitor> GetDrugsInTreatmentRegimen(long PtRegDetailID);
        public abstract IList<GetDrugForSellVisitor> GetDrugsInTreatmentRegimen(long PtRegDetailID, List<string> listICD10Codes);
        /*▲====: #001*/
        public abstract IList<RefTreatmentRegimen> GetRefTreatmentRegimensAndDetail(long? TreatmentRegimenID = null, long? PtRegDetailID = null, List<string> listICD10Codes = null);
        public abstract bool EditRefTreatmentRegimen(RefTreatmentRegimen aRefTreatmentRegimen, out RefTreatmentRegimen aOutRefTreatmentRegimen);

        public abstract IList<GetDrugForSellVisitor> SearchGenMedProductForPrescription_Paging(String BrandName, int IsInsurance, long? StoreID, int PageIndex, int PageSize, bool CountTotal, out int Total);

        public abstract IList<GetDrugForSellVisitor> GetDrugForPrescription_Remaining(long? StoreID, string xml);

        public abstract IList<GetDrugForSellVisitor> GetListDrugPatientUsed(long PatientID, int PageIndex, int PageSize, out int Total);

        #endregion

        #region 3.Prescription Details

        public abstract IList<PrescriptionDetail> GetPrescriptionDetailsByPrescriptID(long PrescriptID);
        public abstract IList<PrescriptionDetail> GetPrescriptionDetailsByPrescriptID_WithNDay(long PrescriptID, out int NDay);
        //▼====== #003: New Method
        public abstract IList<PrescriptionDetail> GetPrescriptionDetailsByPrescriptID_V2(long PrescriptID, long? IssueID, long? AppointmentID, out bool CanEdit, out string ReasonCanEdit, out bool IsEdit);
        //▲====== #003
        public abstract IList<PrescriptionDetail> GetPrescriptionDetailsByPrescriptID_InPt(long PrescriptID, long[] V_CatDrugType = null);

        protected List<PrescriptionDetail> GetPrescriptionDetailCollectionFromReader(IDataReader reader)
        {
            var lst = new List<PrescriptionDetail>();
            int index = 0;
            while (reader.Read())
            {
                var p = GetPrescriptionDetailFromReader(reader);
                p.Index = index;
                lst.Add(p);
                index++;
            }

            return lst;
        }

        protected List<PrescriptionDetail> GetPrescriptionDetailCollectionFromReader_InPt(IDataReader reader)
        {
            var lst = new List<PrescriptionDetail>();
            int index = 0;
            while (reader.Read())
            {
                var p = GetPrescriptionDetailFromReader_InPt(reader);
                p.Index = index;
                lst.Add(p);
                index++;
            }

            return lst;
        }

        //Đọc tiếp chi tiết
        protected PrescriptionDetail GetPrescriptionDetailFromReader(IDataReader reader)
        {
            PrescriptionDetail p = GetPrescriptionDetailFromReaderBase(reader);
            p.ObjPrescriptionDetailSchedules = null;

            if (p.V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN)
            {
                //Xét Lịch Thuốc
                Int64 PrescriptDetailID = 0;
                List<PrescriptionDetailSchedules> ListShedule = new List<PrescriptionDetailSchedules>();

                Int64.TryParse(reader["PrescriptDetailID"].ToString(), out PrescriptDetailID);
                if (PrescriptDetailID > 0)
                {
                    ListShedule = PrescriptionDetailSchedules_ByPrescriptDetailID(PrescriptDetailID, p.IsDrugNotInCat);
                }

                p.ObjPrescriptionDetailSchedules = new ObservableCollection<PrescriptionDetailSchedules>(ListShedule);
            }
            //Xét Lịch Thuốc
            return p;
        }
        //Đọc tiếp chi tiết
        

        protected PrescriptionDetail GetPrescriptionDetailFromReader_InPt(IDataReader reader)
        {
            PrescriptionDetail p = GetPrescriptionDetailFromReaderBase(reader);
            p.ObjPrescriptionDetailSchedules = null;

            if (p.V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN)
            {
                //Xét Lịch Thuốc
                Int64 PrescriptDetailID = 0;
                List<PrescriptionDetailSchedules> ListShedule = new List<PrescriptionDetailSchedules>();

                Int64.TryParse(reader["PrescriptDetailID"].ToString(), out PrescriptDetailID);
                if (PrescriptDetailID > 0)
                {
                    ListShedule = PrescriptionDetailSchedules_ByPrescriptDetailID_InPt(PrescriptDetailID, p.IsDrugNotInCat);
                }

                p.ObjPrescriptionDetailSchedules = new ObservableCollection<PrescriptionDetailSchedules>(ListShedule);
            }
            //Xét Lịch Thuốc
            return p;
        }


        public abstract void UpdateDrugNotDisplayInList(long PatientID, long DrugID, bool? NotDisplayInList);
        #endregion

        #region 4.patientPaymentOld

        public abstract FeeDrug GetValuePatientPaymentOld(long PtRegistrationID);

        #endregion


        #region PrescriptionIssueHistory

        public abstract bool AddPrescriptIssueHistory(Int16 NumberTypePrescriptions_Rule, Prescription entity, out string OutError);
        #endregion

        #region choose dose member
        public abstract List<ChooseDose> InitChooseDoses();
        #endregion

        #region PrescriptionDetailSchedules
        public abstract List<PrescriptionDetailSchedules> PrescriptionDetailSchedules_ByPrescriptDetailID(Int64 PrescriptDetailID
            , bool IsNotIncat);

        public abstract List<PrescriptionDetailSchedules> PrescriptionDetailSchedules_ByPrescriptDetailID_InPt(Int64 PrescriptDetailID
            , bool IsNotIncat);
        #endregion

        #region PrescriptionDetailSchedulesLieuDung
        public abstract List<PrescriptionDetailSchedulesLieuDung> InitChoosePrescriptionDetailSchedulesLieuDung();
        #endregion

        //Đọc tiếp các Liều Dùng
        protected override PrescriptionDetailSchedules GetPrescriptionDetailSchedulesFromReader(IDataReader reader)
        {
            PrescriptionDetailSchedules p = base.GetPrescriptionDetailSchedulesFromReader(reader);
            /*
            //Đọc Liều Dùng
            List<PrescriptionDetailSchedulesLieuDung> ObjListLieuDung = InitChoosePrescriptionDetailSchedulesLieuDung();
            p.ObjPrescriptionDetailSchedulesLieuDung = ObjListLieuDung;
            //Đọc Liều Dùng
            */
            return p;
        }
        //Đọc tiếp các Liều Dùng

        #region Check Toa Được Phép Sửa
        protected override Prescription GetPtPrescriptionFromReader(IDataReader reader)
        {
            Prescription p = base.GetPtPrescriptionFromReader(reader);
            //▼====== #003: Không cần phải kiểm tra ở đây mà nên kiểm tra khi load đầy đủ chi tiết toa lúc người dùng double click chuyển sang màn hình ra toa
            //Xét CanEdit
            //bool CanEdit = false;
            //String ReasonCanEdit = "";

            //if (p != null && p.PrescriptID > 0)
            //{
            //    Prescriptions_CheckCanEdit(p.PrescriptID, p.IssueID, out CanEdit, out ReasonCanEdit);
            //}
            //p.CanEdit = CanEdit;
            //p.ReasonCanEdit = ReasonCanEdit;
            //Xét CanEdit
            //▲====== #003
            return p;
        }
        public abstract void Prescriptions_CheckCanEdit(Int64 PrescriptID, Int64 IssueID, out bool CanEdit, out string ReasonCanEdit);
        #endregion

        #region PrescriptionNoteTemplates
        public abstract IList<PrescriptionNoteTemplates> PrescriptionNoteTemplates_GetAll();

        public abstract IList<PrescriptionNoteTemplates> PrescriptionNoteTemplates_GetAllIsActive(PrescriptionNoteTemplates Obj);

        public abstract void PrescriptionNoteTemplates_Save(PrescriptionNoteTemplates Obj, out string Result);
        #endregion

        public abstract bool PrescriptionsTemplateInsert(PrescriptionTemplate Obj);

        public abstract bool PrescriptionsTemplateDelete(PrescriptionTemplate Obj);

        public abstract List<PrescriptionTemplate> PrescriptionsTemplateGetAll(PrescriptionTemplate Obj);

        public abstract void GetAppointmentID(long issueID, bool isInPatient, out long? appointmentID);
    }
}
