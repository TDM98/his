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
using eHCMS.Services.Core;
using AxLogging;



#region
/*******************************************************************
 * Author: NhanLe
 * Modified date: 2010-10-13
 * Contents: Consultation Services
/*******************************************************************/
#endregion

/*
* 20180922 #001 TBL:   BM 0000073. Them parameter List<string> listICD10Codes cho GetDrugsInTreatmentRegimen
* 20181004 #002 TTM:   BM 0000138: Thêm hàm chỉ lấy chi tiết toa thuốc (không đầy đủ, trả về là string).
* 20181012 #003 TTM: 
* 20190623 #004 TTM:   BM 0011797: Xây dựng màn hình KCB của bác sĩ.
* 20191009 #005 TTM:   BM 0017421: [Ra toa] Thêm tên thư ký y khoa thực hiện toa 
* 20210315 #006 BaoLQ:  Task 237: Lấy danh sách cận lâm sàng nội trú
* 20220823 #007 DatTB: Chỉnh sửa màn hình chờ nhận thuốc
* 20220928 #008 BLQ: Thêm loại toa thuốc khi lưu 
* 20220929 #009 DatTB:
* + Thêm textbox tìm bệnh nhân theo tên/mã/stt
* + Thêm đối tượng ưu tiên
* 20230329 #010 DatTB: Thay đổi list soạn thuốc trước thành list phân trang
* 20230615 #011 DatTB: Thêm cột user bsi mượn OfficialAccountID
 */
namespace aEMR.DataAccessLayer.Providers
{
    public class ePrescriptionsProvider : DataProviderBase
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
                    _instance = new ePrescriptionsProvider();
                }
                return _instance;
            }
        }
        public ePrescriptionsProvider()
        {
            this.ConnectionString = Globals.Settings.Consultations.ConnectionString;
        }
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
            //reader.Close();
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
            //reader.Close();
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
        public List<Lookup> GetLookupPresriptionType()
        {
            List<Lookup> objLst = null;
            objLst = CommonProvider.Lookups.GetAllLookupsByType(LookupValues.PRESCRIPTION_TYPE);
            return objLst;
        }
        #endregion

        #region 1.Prescription

        public IList<Prescription> GetPrescriptionByServiceRecID(long patientID, long? ServiecRecID, long? PtRegistrationID, DateTime? ExamDate)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPrescriptions_ByServiceRecID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(patientID));
                cmd.AddParameter("@ServiceRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ServiecRecID));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@ExamDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(ExamDate));
                cn.Open();
                List<Prescription> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPtPrescriptionCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        public IList<Prescription> GetAllPrescriptions()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPrescriptions_ByPtID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                List<Prescription> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPrescriptionCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        public IList<Prescription> GetPrescriptionByPtID(long patientID, bool latest)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPrescriptions_ByPtID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(patientID));
                if (latest)
                    cmd.AddParameter("@LatestRec", SqlDbType.Bit, 1); // get latest prescription by PatientID
                else
                    cmd.AddParameter("@LatestRec", SqlDbType.Bit, 0); // get all prescription by PatientID

                cn.Open();
                List<Prescription> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPtPrescriptionCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        public IList<Prescription> GetPrescriptionByPtID_Paging(long patientID, long? V_PrescriptionType, bool isInPatient, int PageIndex, int PageSize, out int TotalCount)
        {
            TotalCount = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                //SqlCommand cmd = new SqlCommand("spPrescriptions_ByPtIDPaging", cn);

                SqlCommand cmd = new SqlCommand();
                if (isInPatient)
                {
                    cmd.CommandText = "spPrescriptions_ByPtIDPaging_InPt";
                }
                else
                {
                    cmd.CommandText = "spPrescriptions_ByPtIDPaging";
                }

                cmd.Connection = cn;

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(patientID));
                cmd.AddParameter("@V_PrescriptionType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_PrescriptionType));
                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(PageIndex));
                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paramTotal);
                cn.Open();
                List<Prescription> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPtPrescriptionCollectionFromReader(reader);

                //▼====== #003: Không cần phải kiểm tra ở đây mà nên kiểm tra khi load đầy đủ chi tiết toa lúc người dùng double click chuyển sang màn hình ra toa
                //foreach (var prescription in objLst)
                //{
                //    prescription.IsAllowEditNDay = CheckAllowEditNDay(prescription.AppointmentID);
                //}
                //▲====== #003
                reader.Close();
                if (paramTotal.Value != null)
                {
                    TotalCount = (int)paramTotal.Value;
                }
                else
                    TotalCount = -1;
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        public IList<Prescription> GetPrescriptionByID(long PrescriptID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPrescriptions_ByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PrescriptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PrescriptID));


                cn.Open();
                List<Prescription> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPtPrescriptionCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        public Prescription GetLatestPrescriptionByPtID(long patientID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPrescriptions_ByPtID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(patientID));
                cmd.AddParameter("@LatestRec", SqlDbType.Bit, 1);

                cn.Open();
                Prescription objItem = null;
                IDataReader reader = ExecuteReader(cmd);
                objItem = GetPtPrescriptionItemFromReader(reader);

                if (objItem != null && objItem.IssueID > 0)
                {
                    //Check Có Hẹn Detail chưa. Chưa thì được phép sửa, ngược lại thì không cho Sửa Ngày Hẹn
                    objItem.IsAllowEditNDay = CheckAllowEditNDay(objItem.AppointmentID);
                }

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objItem;
            }
        }


        public Prescription GetLatestPrescriptionByPtID_InPt(long patientID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPrescriptions_ByPtID_InPt", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(patientID));

                cn.Open();
                Prescription objItem = null;
                IDataReader reader = ExecuteReader(cmd);
                objItem = GetPtPrescriptionItemFromReader(reader);

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objItem;
            }
        }

        //Check Toa được phép sửa ngày hay không
        private bool CheckAllowEditNDay(Nullable<long> AppointmentID)
        {
            bool Res = false;

            if (AppointmentID == null || AppointmentID <= 0)
                return true;

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spCheckAllowEditNDay", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@AppointmentID", SqlDbType.BigInt, ConvertNullObjectToDBNull(AppointmentID));
                cmd.AddParameter("@IsEdit", SqlDbType.Bit, DBNull.Value, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@IsEdit"].Value != null)
                    Res = Convert.ToBoolean(cmd.Parameters["@IsEdit"].Value);
                CleanUpConnectionAndCommand(cn, cmd);
            }

            return Res;
        }
        //Check Toa được phép sửa ngày hay không

        public Prescription GetNewPrescriptionByPtID(long patientID, long doctorID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPrescriptions_BlankByPtID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(patientID));
                cmd.AddParameter("@DoctorID", SqlDbType.BigInt, ConvertNullObjectToDBNull(doctorID));

                cn.Open();
                Prescription objItem = null;
                IDataReader reader = ExecuteReader(cmd);
                objItem = GetPtPrescriptionItemFromReader(reader);

                System.Diagnostics.Debug.WriteLine("spPrescriptions_BlankByPtID " + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objItem;
            }
        }

        public Prescription GetPrescriptionID(long PrescriptID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPrescription_ByPrescriptID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PrescriptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PrescriptID));

                cn.Open();
                Prescription objItem = null;
                IDataReader reader = ExecuteReader(cmd);
                objItem = GetPtPrescriptionItemFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objItem;
            }
        }
        //▼====== #002
        public string GetPrescriptDetailsStr_FromPrescriptID(long PrescriptID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPrescriptDetailsStr_FromPrescriptID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PrescriptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PrescriptID));

                cn.Open();
                string objItem = null;
                IDataReader reader = ExecuteReader(cmd);
                objItem = GetPtPrescriptionDetailCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objItem;
            }
        }
        //▲====== #002
        public Prescription GetPrescriptionByIssueID(long IssueID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPrescription_IssueID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@IssueID", SqlDbType.BigInt, ConvertNullObjectToDBNull(IssueID));

                cn.Open();
                Prescription objItem = null;
                IDataReader reader = ExecuteReader(cmd);
                objItem = GetPtPrescriptionItemFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objItem;
            }
        }

        public IList<Prescription> SearchPrescription(PrescriptionSearchCriteria Criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            totalCount = 0;
            if (Criteria == null)
            {
                return null;
            }

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPrescription_Seach", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paramPrescriptID = new SqlParameter("@PrescriptID", SqlDbType.BigInt);
                paramPrescriptID.Value = ConvertNullObjectToDBNull(Criteria.PrescriptID);
                SqlParameter paramPatientID = new SqlParameter("@PatientCode", SqlDbType.VarChar);
                paramPatientID.Value = ConvertNullObjectToDBNull(Criteria.PatientCode);

                SqlParameter paramFullName = new SqlParameter("@FullName", SqlDbType.NVarChar, 128);
                paramFullName.Value = ConvertNullObjectToDBNull(Criteria.FullName);

                SqlParameter paramFromDate = new SqlParameter("@FromDate", SqlDbType.DateTime);
                paramFromDate.Value = ConvertNullObjectToDBNull(Criteria.FromDate);

                SqlParameter paramToDate = new SqlParameter("@ToDate", SqlDbType.DateTime);
                paramToDate.Value = ConvertNullObjectToDBNull(Criteria.ToDate);

                SqlParameter paramHICardNo = new SqlParameter("@HealthInsuranceCard", SqlDbType.VarChar);
                paramHICardNo.Value = ConvertNullObjectToDBNull(Criteria.HICardCode);

                SqlParameter paramIsHI = new SqlParameter("@IsInsurance", SqlDbType.Bit);
                paramIsHI.Value = ConvertNullObjectToDBNull(Criteria.IsInsurance);

                SqlParameter paramPageSize = new SqlParameter("@PageSize", SqlDbType.Int);
                paramPageSize.Value = pageSize;
                SqlParameter paramPageIndex = new SqlParameter("@PageIndex", SqlDbType.Int);
                paramPageIndex.Value = pageIndex;

                SqlParameter paramOrderBy = new SqlParameter("@OrderBy", SqlDbType.NVarChar);
                paramOrderBy.Value = "";

                SqlParameter paramCountTotal = new SqlParameter("@CountTotal", SqlDbType.Bit);
                paramCountTotal.Value = bCountTotal;
                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramPrescriptID);
                cmd.Parameters.Add(paramPatientID);
                cmd.Parameters.Add(paramFullName);
                cmd.Parameters.Add(paramFromDate);
                cmd.Parameters.Add(paramToDate);
                cmd.Parameters.Add(paramHICardNo);
                cmd.Parameters.Add(paramIsHI);

                cmd.Parameters.Add(paramPageSize);
                cmd.Parameters.Add(paramPageIndex);
                cmd.Parameters.Add(paramOrderBy);
                cmd.Parameters.Add(paramCountTotal);
                cmd.Parameters.Add(paramTotal);
                cmd.AddParameter("@PrescriptionIssueCode", SqlDbType.VarChar, Criteria.PrescriptionIssueCode);
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, Criteria.PtRegistrationID);

                cn.Open();
                List<Prescription> prescriptions = null;

                IDataReader reader = ExecuteReader(cmd);

                prescriptions = GetPtPrescriptionCollectionFromReader(reader);
                reader.Close();

                if (bCountTotal && paramTotal.Value != null)
                {
                    totalCount = (int)paramTotal.Value;
                }
                else
                    totalCount = -1;
                CleanUpConnectionAndCommand(cn, cmd);
                return prescriptions;
            }
        }
        public IList<Prescription> GetChoNhanThuocList(DateTime fromDate, DateTime toDate,int IsWaiting)
        {
            if (fromDate == null && toDate==null)
            {
                return null;
            }

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetChoNhanThuoc", cn);
                cmd.CommandType = CommandType.StoredProcedure;
              
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, fromDate);
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, toDate);
                cmd.AddParameter("@IsWaiting", SqlDbType.Int, IsWaiting);

                cn.Open();
                List<Prescription> prescriptions = null;

                IDataReader reader = ExecuteReader(cmd);

                prescriptions = GetChoNhanThuocListFromReader(reader);
                reader.Close();
                
                CleanUpConnectionAndCommand(cn, cmd);
                return prescriptions;
            }
        }
        public void UpdateChoNhanThuoc(long outiID, bool IsWaiting, int CountPrint,out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                 SqlCommand cmd = new SqlCommand("spUpdateChoNhanThuoc", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@outiID", SqlDbType.BigInt, ConvertNullObjectToDBNull(outiID));
                cmd.AddParameter("@IsWaiting", SqlDbType.Bit, ConvertNullObjectToDBNull(IsWaiting));
                cmd.AddParameter("@CountPrint", SqlDbType.Int, ConvertNullObjectToDBNull(CountPrint));
                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);
                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        //▼==== #007
        public IList<Prescription> GetChoSoanThuocList(DateTime fromDate, DateTime toDate, int IsWaiting, string sPatientCode, string sPatientName, int sStoreServiceSeqNum, int pageIndex, int pageSize, out int totalCount) //==== #008, #010
        {
            totalCount = 0;
            if (fromDate == null && toDate == null)
            {
                return null;
            }

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetChoSoanThuoc", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@FromDate", SqlDbType.DateTime, fromDate);
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, toDate);
                cmd.AddParameter("@IsWaiting", SqlDbType.Int, IsWaiting);
                //▼==== #009
                cmd.AddParameter("@sPatientCode", SqlDbType.NVarChar, sPatientCode);
                cmd.AddParameter("@sPatientName", SqlDbType.NVarChar, sPatientName);
                cmd.AddParameter("@sStoreServiceSeqNum", SqlDbType.Int, sStoreServiceSeqNum);
                //▲==== #009
                //▼==== #010
                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(pageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(pageIndex));
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);
                //▲==== #010

                cn.Open();
                List<Prescription> prescriptions = null;

                IDataReader reader = ExecuteReader(cmd);

                prescriptions = GetChoSoanThuocListFromReader(reader);
                reader.Close();

                //▼==== #010
                if (cmd.Parameters["@Total"].Value != DBNull.Value)
                {
                    totalCount = (int)cmd.Parameters["@Total"].Value;
                }
                else
                {
                    totalCount = -1;
                }
                //▲==== #010
                CleanUpConnectionAndCommand(cn, cmd);
                return prescriptions;
            }
        }
        public void UpdateChoSoanThuoc(long PtRegistrationID, bool IsWaiting, int CountPrint, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spUpdateChoSoanThuoc", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@IsWaiting", SqlDbType.Bit, ConvertNullObjectToDBNull(IsWaiting));
                cmd.AddParameter("@CountPrint", SqlDbType.Int, ConvertNullObjectToDBNull(CountPrint));
                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);
                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
                CleanUpConnectionAndCommand(cn, cmd);
            }
        }
        //▲==== #007

        public IList<Prescription> Prescription_Seach_WithIsSoldIssueID(PrescriptionSearchCriteria Criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            totalCount = 0;
            if (Criteria == null)
            {
                return null;
            }

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPrescription_Seach_WithIsSoldIssueID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paramPrescriptID = new SqlParameter("@PrescriptID", SqlDbType.BigInt);
                paramPrescriptID.Value = ConvertNullObjectToDBNull(Criteria.PrescriptID);
                SqlParameter paramPatientID = new SqlParameter("@PatientCode", SqlDbType.VarChar);
                paramPatientID.Value = ConvertNullObjectToDBNull(Criteria.PatientCode);

                SqlParameter paramFullName = new SqlParameter("@FullName", SqlDbType.NVarChar, 128);
                paramFullName.Value = ConvertNullObjectToDBNull(Criteria.FullName);

                SqlParameter paramFromDate = new SqlParameter("@FromDate", SqlDbType.DateTime);
                paramFromDate.Value = ConvertNullObjectToDBNull(Criteria.FromDate);

                SqlParameter paramToDate = new SqlParameter("@ToDate", SqlDbType.DateTime);
                paramToDate.Value = ConvertNullObjectToDBNull(Criteria.ToDate);

                SqlParameter paramHICardNo = new SqlParameter("@HealthInsuranceCard", SqlDbType.VarChar);
                paramHICardNo.Value = ConvertNullObjectToDBNull(Criteria.HICardCode);

                SqlParameter paramPageSize = new SqlParameter("@PageSize", SqlDbType.Int);
                paramPageSize.Value = pageSize;
                SqlParameter paramPageIndex = new SqlParameter("@PageIndex", SqlDbType.Int);
                paramPageIndex.Value = pageIndex;

                SqlParameter paramOrderBy = new SqlParameter("@OrderBy", SqlDbType.NVarChar);
                paramOrderBy.Value = "";

                SqlParameter paramCountTotal = new SqlParameter("@CountTotal", SqlDbType.Bit);
                paramCountTotal.Value = bCountTotal;
                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramPrescriptID);
                cmd.Parameters.Add(paramPatientID);
                cmd.Parameters.Add(paramFullName);
                cmd.Parameters.Add(paramFromDate);
                cmd.Parameters.Add(paramToDate);
                cmd.Parameters.Add(paramHICardNo);

                cmd.Parameters.Add(paramPageSize);
                cmd.Parameters.Add(paramPageIndex);
                cmd.Parameters.Add(paramOrderBy);
                cmd.Parameters.Add(paramCountTotal);
                cmd.Parameters.Add(paramTotal);

                cn.Open();
                List<Prescription> prescriptions = null;

                IDataReader reader = ExecuteReader(cmd);

                prescriptions = GetPtPrescriptionCollectionFromReader(reader);
                reader.Close();

                if (bCountTotal && paramTotal.Value != null)
                {
                    totalCount = (int)paramTotal.Value;
                }
                else
                    totalCount = -1;
                CleanUpConnectionAndCommand(cn, cmd);
                return prescriptions;
            }
        }

        public bool DeletePrescription(Prescription entity)
        {
            throw new NotImplementedException();
        }

        #region "Kiểm tra trùng toa thuốc trước khi update"
        private List<Prescription> SubTractPrecriptions_NeedEdit(List<Prescription> ListPrescriptions_TrongNgay, Prescription entityOld)
        {
            foreach (Prescription item in ListPrescriptions_TrongNgay)
            {
                if (item.PrescriptID == entityOld.PrescriptID)
                {
                    ListPrescriptions_TrongNgay.Remove(item);
                    break;
                }
            }
            return ListPrescriptions_TrongNgay;
        }
        private bool KiemTraTrungToaThuocTruocKhiUpdate(out string druglist, Prescription entity, Prescription entityOld)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                List<IList<PrescriptionDetail>> ListPrescriptionDetailTrongNgay = new List<IList<PrescriptionDetail>>();

                //Ds Toa Trong Ngày: Kiểm Tra Thuốc Toa Lần Này và Toa Trong Ngày Trùng Thuốc
                List<Prescription> ListPrescriptions_TrongNgay = Prescriptions_TrongNgay_ByPatientID(entityOld.PatientID.Value);

                if (ListPrescriptions_TrongNgay.Count > 0)
                {
                    List<Prescription> ListPrescriptions_TrongNgayAfterSubtract = SubTractPrecriptions_NeedEdit(ListPrescriptions_TrongNgay, entityOld);

                    foreach (Prescription item in ListPrescriptions_TrongNgayAfterSubtract)
                    {
                        ListPrescriptionDetailTrongNgay.Add(GetPrescriptionDetailsByPrescriptID(item.PrescriptID));
                    }

                    //if (CheckToaBiTrungTrongNgay(ListPrescriptionDetailTrongNgay, entity.PrescriptionDetails) == false)
                    //{
                    //    return true;
                    //}
                    //return false;
                    druglist = CheckToaThuocBiTrungTrongNgay(ListPrescriptionDetailTrongNgay, entity.PrescriptionDetails);
                    cn.Dispose();
                    if (druglist != "")
                    {
                        return false;
                    }
                    return true;
                }
                druglist = "";
                cn.Dispose();
                return true;
            }
        }

        private bool KiemTraTrungToaThuocTruocKhiAddNew(out string druglist, Prescription entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                List<IList<PrescriptionDetail>> ListPrescriptionDetailTrongNgay = new List<IList<PrescriptionDetail>>();

                //Ds Toa Trong Ngày: Kiểm Tra Thuốc Toa Lần Này và Toa Trong Ngày Trùng Thuốc
                List<Prescription> ListPrescriptions_TrongNgay = Prescriptions_TrongNgay_ByPatientID(entity.PatientID.Value);

                if (ListPrescriptions_TrongNgay.Count > 0)
                {
                    foreach (Prescription item in ListPrescriptions_TrongNgay)
                    {
                        ListPrescriptionDetailTrongNgay.Add(GetPrescriptionDetailsByPrescriptID(item.PrescriptID));
                    }

                    //if (CheckToaBiTrungTrongNgay(ListPrescriptionDetailTrongNgay, entity.PrescriptionDetails) == false)
                    //{
                    //    return true;
                    //}
                    //return false;
                    druglist = CheckToaThuocBiTrungTrongNgay(ListPrescriptionDetailTrongNgay, entity.PrescriptionDetails);
                    cn.Dispose();
                    if (druglist != "")
                    {
                        return false;
                    }
                    return true;
                }
                druglist = "";
                cn.Dispose();
                return true;
            }
        }
        #endregion

        public bool Prescriptions_Update(Prescription entity, Prescription entity_OLD, bool AllowUpdateThoughReturnDrugNotEnough, out string Result, out long NewPrescriptID, out long IssueID, bool IsUpdateWithoutChangeDoctorIDAndDatetime = false)
        {
            Result = "";
            NewPrescriptID = 0;
            IssueID = 0;
            string drugList = "";
            if (KiemTraTrungToaThuocTruocKhiUpdate(out drugList, entity, entity_OLD))
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand("spPrescriptions_UpdateV2New", cn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter par1 = cmd.Parameters.Add("@ServiceRecID", SqlDbType.BigInt);
                        SqlParameter par2 = cmd.Parameters.Add("@PtRegDetailID", SqlDbType.BigInt);

                        SqlParameter parIssuedDateTime = cmd.Parameters.Add("@IssuedDateTime", SqlDbType.DateTime);

                        SqlParameter par3 = cmd.Parameters.Add("@CreatorStaffID", SqlDbType.BigInt);
                        SqlParameter par4 = cmd.Parameters.Add("@ConsultantDoctorID", SqlDbType.BigInt);
                        SqlParameter par5 = cmd.Parameters.Add("@AppointmentDate", SqlDbType.DateTime);
                        SqlParameter par6 = cmd.Parameters.Add("@Diagnosis", SqlDbType.NVarChar);
                        SqlParameter par7 = cmd.Parameters.Add("@DoctorAdvice", SqlDbType.NVarChar);
                        SqlParameter par8 = cmd.Parameters.Add("@ForOutPatient", SqlDbType.Bit);
                        SqlParameter par9 = cmd.Parameters.Add("@V_PrescriptionType", SqlDbType.BigInt);
                        SqlParameter par10 = cmd.Parameters.Add("@V_PrescriptionNotes", SqlDbType.BigInt);
                        SqlParameter par11 = cmd.Parameters.Add("@OriginalPrescriptID", SqlDbType.BigInt);

                        SqlParameter parIX_IssuedDateTime = cmd.Parameters.Add("@IX_IssuedDateTime", SqlDbType.Int);

                        SqlParameter parxmlDetail = cmd.Parameters.Add("@XMLPrescriptionDetails", SqlDbType.Xml);

                        SqlParameter par13 = cmd.Parameters.Add("@NewPrescriptID", SqlDbType.BigInt);
                        par13.Direction = ParameterDirection.Output;
                        SqlParameter par14 = cmd.Parameters.Add("@IssueID", SqlDbType.BigInt);
                        par14.Direction = ParameterDirection.Output;

                        SqlParameter par19 = cmd.Parameters.Add("@HasAppointment", SqlDbType.Bit);
                        //▼====== #004
                        SqlParameter par27 = cmd.Parameters.Add("@PreNoDrug", SqlDbType.Bit);
                        SqlParameter par28 = cmd.Parameters.Add("@IsOutCatConfirmed", SqlDbType.Bit);
                        //▲====== #004
                        SqlParameter par15 = cmd.Parameters.Add("@PatientID", SqlDbType.BigInt);
                        SqlParameter par16 = cmd.Parameters.Add("@NDay", SqlDbType.Int);

                        //Toa Cũ
                        SqlParameter par17 = cmd.Parameters.Add("@PrescriptID_BacSi", SqlDbType.BigInt);
                        SqlParameter par18 = cmd.Parameters.Add("@IssueID_BacSi", SqlDbType.BigInt);
                        SqlParameter par20 = cmd.Parameters.Add("@OrigCreatorDoctorNames", SqlDbType.NVarChar);
                        SqlParameter par22 = cmd.Parameters.Add("@SecretaryStaffID", SqlDbType.BigInt);

                        SqlParameter par21 = cmd.Parameters.Add("@DeptLocID", SqlDbType.BigInt);
                        SqlParameter par23 = cmd.Parameters.Add("@PtRegistrationID", SqlDbType.BigInt);
                        SqlParameter par24 = cmd.Parameters.Add("@HisID", SqlDbType.BigInt);
                        SqlParameter par25 = cmd.Parameters.Add("@AllowUpdateThoughReturnDrugNotEnough", SqlDbType.Bit);
                        SqlParameter par26 = cmd.Parameters.Add("@V_PrescriptionIssuedCase", SqlDbType.BigInt);
                        //Toa Cũ

                        //20190518 TBL: Neu toa thuoc co canh bao ve tuong tac hoac tuong tu thi luu xuong bang de lam bao cao
                        SqlParameter par29 = cmd.Parameters.Add("@IsWarningSimilar", SqlDbType.Bit);
                        SqlParameter par30 = cmd.Parameters.Add("@IsWarningInteraction", SqlDbType.Bit);
                        //20190518

                        //▼===== #005
                        SqlParameter par31 = cmd.Parameters.Add("@MedSecretaryID", SqlDbType.BigInt);
                        //▲===== #005
                        //▼===== #011
                        cmd.AddParameter("@UserOfficialAccountID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.UserOfficialAccountID));
                        //▲===== #011
                        par1.Value = ConvertNullObjectToDBNull(entity.ServiceRecID);
                        par2.Value = ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.PtRegDetailID);

                        parIssuedDateTime.Value = ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.IssuedDateTime);

                        par3.Value = ConvertNullObjectToDBNull(entity.CreatorStaffID);
                        par4.Value = ConvertNullObjectToDBNull(entity.ConsultantID);
                        par5.Value = ConvertNullObjectToDBNull(entity.AppointmentDate);
                        par6.Value = ConvertNullObjectToDBNull(entity.Diagnosis);
                        par7.Value = ConvertNullObjectToDBNull(entity.DoctorAdvice);
                        par8.Value = ConvertNullObjectToDBNull(entity.ForOutPatient);
                        par9.Value = ConvertNullObjectToDBNull(entity.V_PrescriptionType);
                        par10.Value = ConvertNullObjectToDBNull(entity.V_PrescriptionNotes);
                        par11.Value = ConvertNullObjectToDBNull(entity.OriginalPrescriptID);

                        parIX_IssuedDateTime.Value = ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.IX_IssuedDateTime);

                        string xmlDetail = entity.ConvertPrescriptionDetailsWithScheduleToXML(entity.PrescriptionDetails);
                        parxmlDetail.Value = ConvertNullObjectToDBNull(xmlDetail);

                        par19.Value = ConvertNullObjectToDBNull(entity.HasAppointment);
                        par15.Value = ConvertNullObjectToDBNull(entity.PatientID);
                        par16.Value = ConvertNullObjectToDBNull(entity.NDay);

                        par17.Value = ConvertNullObjectToDBNull(entity_OLD.PrescriptID);
                        par18.Value = ConvertNullObjectToDBNull(entity_OLD.IssueID);
                        par20.Value = ConvertNullObjectToDBNull(entity_OLD.PrescriptionIssueHistory.OrigCreatorDoctorNames);
                        par21.Value = ConvertNullObjectToDBNull(entity_OLD.PrescriptionIssueHistory.DeptLocID);
                        par22.Value = ConvertNullObjectToDBNull(entity_OLD.SecretaryStaff == null ? 0 : entity_OLD.SecretaryStaff.StaffID);
                        par23.Value = ConvertNullObjectToDBNull(entity_OLD.PtRegistrationID);
                        par24.Value = ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.HisID);
                        par25.Value = ConvertNullObjectToDBNull(AllowUpdateThoughReturnDrugNotEnough);
                        par26.Value = ConvertNullObjectToDBNull(entity_OLD.V_PrescriptionIssuedCase);
                        //▼====== #004
                        par27.Value = ConvertNullObjectToDBNull(entity.PreNoDrug);
                        par28.Value = ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.IsOutCatConfirmed);
                        //▲====== #004
                        par29.Value = ConvertNullObjectToDBNull(entity.IsWarningSimilar);
                        par30.Value = ConvertNullObjectToDBNull(entity.IsWarningInteraction);

                        //▼===== #005
                        par31.Value = ConvertNullObjectToDBNull(entity.MedSecretaryID);
                        //▲===== #005

                        cmd.AddParameter("@IsUpdateWithoutChangeDoctorIDAndDatetime", SqlDbType.Bit, ConvertNullObjectToDBNull(IsUpdateWithoutChangeDoctorIDAndDatetime));
                        cmd.AddParameter("@Reason", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Reason));
                        cmd.AddParameter("@IsOutsideRegimen", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.IsOutsideRegimen));
                        cmd.AddParameter("@PercentExceed", SqlDbType.SmallInt, ConvertNullObjectToDBNull(entity.PercentExceed));
                        //▼===== #008
                        //cmd.AddParameter("@V_PrescriptionIssuedType", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.V_PrescriptionIssuedType));
                        //▲===== #008

                        cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                        cn.Open();

                        ExecuteNonQuery(cmd);

                        if (cmd.Parameters["@Result"].Value != DBNull.Value)
                            Result = cmd.Parameters["@Result"].Value.ToString();

                        if (cmd.Parameters["@NewPrescriptID"].Value != DBNull.Value)
                            NewPrescriptID = (long)cmd.Parameters["@NewPrescriptID"].Value;

                        if (cmd.Parameters["@IssueID"].Value != DBNull.Value)
                            IssueID = (long)cmd.Parameters["@IssueID"].Value;

                        //Modified Date: 22/02/2014 16:12
                        //KMx: Không update StoreServiceSeqNumType và StoreServiceSeqNum chung với Stored ở trên. Vì:
                        //     Khi xin số thứ tự rồi. Nếu như Result không phải là "OK" tức là toa không lưu được. Coi như bị mất STT đó.
                        //     Cho nên chỉ khi nào toa thuốc lưu thành công thì mới xin số và cập nhật lại.
                        //     Trường hợp toa lưu không được: Khi 1 ngày ra 2 toa bảo hiểm, Stored sẽ trả kết quả về để BS xác nhận 1 lần nữa.
                        if (Result == "OK")
                        {
                            //Lấy số
                            ServiceSequenceNumberProvider ssnp = new ServiceSequenceNumberProvider();
                            byte seqNumberType;
                            uint SeqNumber;

                            try
                            {
                                //KMx: Nếu cập nhật toa cũ mà ngày ra toa khác ngày hiện tại thì xin lại số.
                                if (entity_OLD.PrescriptionIssueHistory.IssuedDateTime.GetValueOrDefault().Date != DateTime.Now.Date)
                                {
                                    //KMx: Toa thuốc có BH hay không là do HisID quyết định chứ không phải entity.HICardNo. (22/02/2014 10:52)
                                    if (entity.PrescriptionIssueHistory.HisID > 0)
                                    {
                                        ssnp.PharmacyServiceNumber(out seqNumberType, PharmacyInvType.CO_BAO_HIEM, out SeqNumber);
                                    }
                                    else
                                    {
                                        ssnp.PharmacyServiceNumber(out seqNumberType, PharmacyInvType.KHONG_BAO_HIEM, out SeqNumber);
                                    }
                                    entity.PrescriptionIssueHistory.StoreServiceSeqNumType = seqNumberType;
                                    entity.PrescriptionIssueHistory.StoreServiceSeqNum = (short)SeqNumber;
                                }
                                else
                                {
                                    //Nếu cập nhật từ toa không BH thành toa có BH thì xin số lại (StoreServiceSeqNumType = 20 không BH, 15 có BH).
                                    if (entity.PrescriptionIssueHistory.HisID > 0 && entity_OLD.PrescriptionIssueHistory.StoreServiceSeqNumType == 20)
                                    {
                                        ssnp.PharmacyServiceNumber(out seqNumberType, PharmacyInvType.CO_BAO_HIEM, out SeqNumber);

                                        entity.PrescriptionIssueHistory.StoreServiceSeqNumType = seqNumberType;
                                        entity.PrescriptionIssueHistory.StoreServiceSeqNum = (short)SeqNumber;
                                    }
                                    //Nếu cập nhật từ toa BH thành toa không BH thì xin số lại.
                                    if (entity.PrescriptionIssueHistory.HisID <= 0 && entity_OLD.PrescriptionIssueHistory.StoreServiceSeqNumType == 15)
                                    {
                                        ssnp.PharmacyServiceNumber(out seqNumberType, PharmacyInvType.KHONG_BAO_HIEM, out SeqNumber);

                                        entity.PrescriptionIssueHistory.StoreServiceSeqNumType = seqNumberType;
                                        entity.PrescriptionIssueHistory.StoreServiceSeqNum = (short)SeqNumber;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                AxLogger.Instance.LogError(ex);
                                Result = ex.Message;
                                CleanUpConnectionAndCommand(cn, cmd);
                                return false;
                            }

                            if (entity.PrescriptionIssueHistory.StoreServiceSeqNum > 0 && entity.PrescriptionIssueHistory.StoreServiceSeqNumType > 0)
                            {
                                SqlCommand sqlcmd = new SqlCommand("spPrescriptions_UpdateSequenceNumber", cn);
                                sqlcmd.CommandType = CommandType.StoredProcedure;
                                SqlParameter parIssueID = sqlcmd.Parameters.Add("@IssueID", SqlDbType.BigInt);
                                SqlParameter parSeqNum = sqlcmd.Parameters.Add("@StoreServiceSeqNum", SqlDbType.SmallInt);
                                SqlParameter parSeqNumType = sqlcmd.Parameters.Add("@StoreServiceSeqNumType", SqlDbType.TinyInt);

                                parIssueID.Value = ConvertNullObjectToDBNull(IssueID);
                                parSeqNum.Value = ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.StoreServiceSeqNum);
                                parSeqNumType.Value = ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.StoreServiceSeqNumType);
                                ExecuteNonQuery(sqlcmd);
                            }
                            //----------------------------Modified by KMx------------------------------------------------------------
                        }
                        else
                        {
                            CleanUpConnectionAndCommand(cn, cmd);
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        AxLogger.Instance.LogError(ex);
                        Result = ex.Message;
                        return false;
                    }
                }
                return true;
            }
            else
            {
                Result = "Duplex-Prescriptions_PrescriptionsInDay" + "@" + drugList;//Toa Thuốc Bị Trùng Với 1 Toa Trong Ngày Hôm Nay
                return false;
            }
        }


        public void Prescriptions_InPt_Update(Prescription entity, Prescription entity_OLD, bool AllowUpdateThoughReturnDrugNotEnough, out string Result, out long NewPrescriptID, out long IssueID, out string ServerError)
        {
            Result = "";
            NewPrescriptID = 0;
            IssueID = 0;
            ServerError = "";
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("spPrescriptions_InPt_Update", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter par1 = cmd.Parameters.Add("@ServiceRecID", SqlDbType.BigInt);

                    SqlParameter parIssuedDateTime = cmd.Parameters.Add("@IssuedDateTime", SqlDbType.DateTime);

                    SqlParameter par3 = cmd.Parameters.Add("@CreatorStaffID", SqlDbType.BigInt);
                    SqlParameter par4 = cmd.Parameters.Add("@ConsultantDoctorID", SqlDbType.BigInt);
                    SqlParameter par5 = cmd.Parameters.Add("@AppointmentDate", SqlDbType.DateTime);
                    SqlParameter par6 = cmd.Parameters.Add("@Diagnosis", SqlDbType.NVarChar);
                    SqlParameter par7 = cmd.Parameters.Add("@DoctorAdvice", SqlDbType.NVarChar);
                    SqlParameter par8 = cmd.Parameters.Add("@ForOutPatient", SqlDbType.Bit);
                    SqlParameter par9 = cmd.Parameters.Add("@V_PrescriptionType", SqlDbType.BigInt);
                    SqlParameter par10 = cmd.Parameters.Add("@V_PrescriptionNotes", SqlDbType.BigInt);
                    SqlParameter par11 = cmd.Parameters.Add("@OriginalPrescriptID", SqlDbType.BigInt);

                    SqlParameter parIX_IssuedDateTime = cmd.Parameters.Add("@IX_IssuedDateTime", SqlDbType.Int);

                    SqlParameter parxmlDetail = cmd.Parameters.Add("@XMLPrescriptionDetails", SqlDbType.Xml);

                    SqlParameter par13 = cmd.Parameters.Add("@NewPrescriptID", SqlDbType.BigInt);
                    par13.Direction = ParameterDirection.Output;
                    SqlParameter par14 = cmd.Parameters.Add("@IssueID", SqlDbType.BigInt);
                    par14.Direction = ParameterDirection.Output;

                    SqlParameter par19 = cmd.Parameters.Add("@HasAppointment", SqlDbType.Bit);
                    SqlParameter par15 = cmd.Parameters.Add("@PatientID", SqlDbType.BigInt);
                    SqlParameter par16 = cmd.Parameters.Add("@NDay", SqlDbType.Int);

                    //Toa Cũ
                    SqlParameter par17 = cmd.Parameters.Add("@PrescriptID_BacSi", SqlDbType.BigInt);
                    SqlParameter par18 = cmd.Parameters.Add("@IssueID_BacSi", SqlDbType.BigInt);
                    SqlParameter par20 = cmd.Parameters.Add("@OrigCreatorDoctorNames", SqlDbType.NVarChar);
                    SqlParameter par22 = cmd.Parameters.Add("@SecretaryStaffID", SqlDbType.BigInt);

                    SqlParameter par21 = cmd.Parameters.Add("@DeptLocID", SqlDbType.BigInt);
                    SqlParameter par23 = cmd.Parameters.Add("@PtRegistrationID", SqlDbType.BigInt);
                    SqlParameter par24 = cmd.Parameters.Add("@HisID", SqlDbType.BigInt);

                    SqlParameter par25 = cmd.Parameters.Add("@AllowUpdateThoughReturnDrugNotEnough", SqlDbType.Bit);
                    SqlParameter par26 = cmd.Parameters.Add("@V_PrescriptionIssuedCase", SqlDbType.BigInt);

                    SqlParameter par27 = cmd.Parameters.Add("@IssueDeptID", SqlDbType.BigInt);

                    //Toa Cũ

                    par1.Value = ConvertNullObjectToDBNull(entity.ServiceRecID);

                    parIssuedDateTime.Value = ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.IssuedDateTime);

                    par3.Value = ConvertNullObjectToDBNull(entity.CreatorStaffID);
                    par4.Value = ConvertNullObjectToDBNull(entity.ConsultantID);
                    par5.Value = ConvertNullObjectToDBNull(entity.AppointmentDate);
                    par6.Value = ConvertNullObjectToDBNull(entity.Diagnosis);
                    par7.Value = ConvertNullObjectToDBNull(entity.DoctorAdvice);
                    par8.Value = ConvertNullObjectToDBNull(entity.ForOutPatient);
                    par9.Value = ConvertNullObjectToDBNull(entity.V_PrescriptionType);
                    par10.Value = ConvertNullObjectToDBNull(entity.V_PrescriptionNotes);
                    par11.Value = ConvertNullObjectToDBNull(entity.OriginalPrescriptID);

                    parIX_IssuedDateTime.Value = ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.IX_IssuedDateTime);

                    string xmlDetail = entity.ConvertPrescriptionDetailsWithScheduleToXML(entity.PrescriptionDetails);
                    parxmlDetail.Value = ConvertNullObjectToDBNull(xmlDetail);

                    par19.Value = ConvertNullObjectToDBNull(entity.HasAppointment);
                    par15.Value = ConvertNullObjectToDBNull(entity.PatientID);
                    par16.Value = ConvertNullObjectToDBNull(entity.NDay);

                    par17.Value = ConvertNullObjectToDBNull(entity_OLD.PrescriptID);
                    par18.Value = ConvertNullObjectToDBNull(entity_OLD.IssueID);
                    par20.Value = ConvertNullObjectToDBNull(entity_OLD.PrescriptionIssueHistory.OrigCreatorDoctorNames);
                    par21.Value = ConvertNullObjectToDBNull(entity_OLD.PrescriptionIssueHistory.DeptLocID);
                    par22.Value = ConvertNullObjectToDBNull(entity_OLD.SecretaryStaff == null ? 0 : entity_OLD.SecretaryStaff.StaffID);
                    par23.Value = ConvertNullObjectToDBNull(entity_OLD.PtRegistrationID);
                    par24.Value = ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.HisID);
                    par25.Value = ConvertNullObjectToDBNull(AllowUpdateThoughReturnDrugNotEnough);
                    par26.Value = ConvertNullObjectToDBNull(entity_OLD.V_PrescriptionIssuedCase);
                    par27.Value = ConvertNullObjectToDBNull(entity.Department.DeptID);
                    cmd.AddParameter("@ConfirmedDTItemID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ConfirmedDTItemID));
                    cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                    cn.Open();

                    ExecuteNonQuery(cmd);

                    if (cmd.Parameters["@Result"].Value != DBNull.Value)
                        Result = cmd.Parameters["@Result"].Value.ToString();

                    if (cmd.Parameters["@NewPrescriptID"].Value != DBNull.Value)
                        NewPrescriptID = (long)cmd.Parameters["@NewPrescriptID"].Value;

                    if (cmd.Parameters["@IssueID"].Value != DBNull.Value)
                        IssueID = (long)cmd.Parameters["@IssueID"].Value;
                    CleanUpConnectionAndCommand(cn, cmd);
                }
                catch (Exception ex)
                {
                    ServerError = ex.Message;
                    AxLogger.Instance.LogError(ex);
                    Result = "Error-Exception";
                }
            }
        }


        #region "Kiểm tra toa thuốc trước khi lưu"

        private string CheckToaThuocBiTrungTrongNgay(List<IList<PrescriptionDetail>> ListPrescriptionDetailTrongNgay, IList<PrescriptionDetail> pNeedSave)
        {
            string duplexDrug = "";
            if (pNeedSave != null && pNeedSave.Count > 0)
            {
                foreach (var prescriptDetaiil in pNeedSave)
                {
                    if (!CheckThuocBiTrungTrongNgay(ListPrescriptionDetailTrongNgay, prescriptDetaiil))
                    {
                        duplexDrug += prescriptDetaiil.SelectedDrugForPrescription.BrandName + "\n";
                    }
                }
            }
            return duplexDrug;/*chưa có toa nào trong ngày*/
        }

        private bool CheckThuocBiTrungTrongNgay(List<IList<PrescriptionDetail>> ListPrescriptionDetailTrongNgay, PrescriptionDetail pNeedSave)
        {
            if (ListPrescriptionDetailTrongNgay != null && ListPrescriptionDetailTrongNgay.Count > 0)
            {
                foreach (IList<PrescriptionDetail> listdetail_i in ListPrescriptionDetailTrongNgay)
                {
                    if (listdetail_i.Any(x => x.DrugID == 0 ? !string.IsNullOrEmpty(x.BrandName) && x.BrandName == pNeedSave.BrandName : x.DrugID == pNeedSave.DrugID))
                        return false;
                }
            }
            return true;/*chưa có toa nào trong ngày*/
        }

        private bool CheckToaBiTrungTrongNgay(List<IList<PrescriptionDetail>> ListPrescriptionDetailTrongNgay, IList<PrescriptionDetail> pNeedSave)
        {
            if (ListPrescriptionDetailTrongNgay != null && ListPrescriptionDetailTrongNgay.Count > 0)
            {
                foreach (IList<PrescriptionDetail> listdetail_i in ListPrescriptionDetailTrongNgay)
                {
                    if (Check2ToaIsDuplexDrugID(listdetail_i, pNeedSave) == true)
                        return true;
                }
                return false;
            }
            return false;/*chưa có toa nào trong ngày*/
        }
        private bool Check2ToaIsDuplexDrugID(IList<PrescriptionDetail> listi, IList<PrescriptionDetail> pNeedSave)
        {
            if (listi.Count == pNeedSave.Count)
            {
                foreach (PrescriptionDetail item in listi)
                {
                    if (CheckIsExistsInNewPrecriptions(item.DrugID.Value, pNeedSave) == false)
                        return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool CheckIsExistsInNewPrecriptions(Int64 DrugID, IList<PrescriptionDetail> pNeedSave)
        {
            foreach (PrescriptionDetail item in pNeedSave)
            {
                if (item.DrugID == DrugID)
                    return true;
            }
            return false;
        }


        private bool KiemTraChanDoanDaCoToaThuoc(Prescription entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPrescriptions_AddCheckDiagnosis", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter par1 = cmd.Parameters.Add("@ServiceRecID", SqlDbType.BigInt);
                par1.Value = ConvertNullObjectToDBNull(entity.ServiceRecID);
                SqlParameter par2 = cmd.Parameters.Add("@ErrorID", SqlDbType.TinyInt);
                par2.Direction = ParameterDirection.Output;
                cn.Open();
                ExecuteNonQuery(cmd);
                int ErrorID = 0;
                if (cmd.Parameters["@ErrorID"].Value != DBNull.Value)
                {
                    ErrorID = Convert.ToInt16(cmd.Parameters["@ErrorID"].Value);
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return ErrorID > 0;
            }
        }


        #endregion

        long DeptLocID = 0;

        public bool Prescriptions_Add(Int16 NumberTypePrescriptions_Rule, Prescription entity, out long newPrescriptID, out long IssueID, out string OutError)
        {
            IssueID = 0;
            newPrescriptID = 0;
            OutError = "";
            if (!KiemTraChanDoanDaCoToaThuoc(entity))
            {
                string drugList = "";
                if (KiemTraTrungToaThuocTruocKhiAddNew(out drugList, entity))
                {
                    //Lấy số
                    ServiceSequenceNumberProvider ssnp = new ServiceSequenceNumberProvider();
                    byte seqNumberType;
                    uint SeqNumber;

                    try
                    {
                        //if (NumberTypePrescriptions_Rule == 2)
                        //{
                        //    if (!string.IsNullOrEmpty(entity.HICardNo))
                        //    {
                        //        ssnp.PharmacyServiceNumber(out seqNumberType, PharmacyInvType.CO_BAO_HIEM, out SeqNumber);
                        //    }
                        //    else
                        //    {
                        //        ssnp.PharmacyServiceNumber(out seqNumberType, PharmacyInvType.KHONG_BAO_HIEM, out SeqNumber);
                        //    }
                        //}
                        //else
                        //{
                        //    ssnp.PharmacyServiceNumber(out seqNumberType, PharmacyInvType.KHONG_BAO_HIEM, out SeqNumber);
                        //}

                        //KMx: Toa thuốc có BH hay không là do HisID quyết định chứ không phải entity.HICardNo. (22/02/2014 10:52)

                        if (entity.PrescriptionIssueHistory.HisID > 0)
                        {
                            ssnp.PharmacyServiceNumber(out seqNumberType, PharmacyInvType.CO_BAO_HIEM, out SeqNumber);
                        }
                        else
                        {
                            ssnp.PharmacyServiceNumber(out seqNumberType, PharmacyInvType.KHONG_BAO_HIEM, out SeqNumber);
                        }

                        entity.PrescriptionIssueHistory.StoreServiceSeqNumType = seqNumberType;
                        entity.PrescriptionIssueHistory.StoreServiceSeqNum = (short)SeqNumber;
                    }
                    catch (Exception ex)
                    {
                        OutError = ex.ToString();
                        // return false;
                    }
                    //Lấy số

                    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                    {
                        SqlCommand cmd = new SqlCommand("spPrescriptions_AddV2New", cn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter par1 = cmd.Parameters.Add("@ServiceRecID", SqlDbType.BigInt);
                        SqlParameter par2 = cmd.Parameters.Add("@PtRegDetailID", SqlDbType.BigInt);

                        SqlParameter parIssuedDateTime = cmd.Parameters.Add("@IssuedDateTime", SqlDbType.DateTime);

                        SqlParameter par3 = cmd.Parameters.Add("@CreatorStaffID", SqlDbType.BigInt);
                        SqlParameter par4 = cmd.Parameters.Add("@ConsultantDoctorID", SqlDbType.BigInt);
                        SqlParameter par5 = cmd.Parameters.Add("@AppointmentDate", SqlDbType.DateTime);
                        SqlParameter par6 = cmd.Parameters.Add("@Diagnosis", SqlDbType.NVarChar);
                        SqlParameter par7 = cmd.Parameters.Add("@DoctorAdvice", SqlDbType.NVarChar);
                        SqlParameter par8 = cmd.Parameters.Add("@ForOutPatient", SqlDbType.Bit);
                        SqlParameter par9 = cmd.Parameters.Add("@V_PrescriptionType", SqlDbType.BigInt);
                        SqlParameter par10 = cmd.Parameters.Add("@V_PrescriptionNotes", SqlDbType.BigInt);
                        SqlParameter par11 = cmd.Parameters.Add("@OriginalPrescriptID", SqlDbType.BigInt);


                        SqlParameter parSeqNum = cmd.Parameters.Add("@StoreServiceSeqNum", SqlDbType.SmallInt);
                        SqlParameter parSeqNumType = cmd.Parameters.Add("@StoreServiceSeqNumType", SqlDbType.TinyInt);


                        SqlParameter parIX_IssuedDateTime = cmd.Parameters.Add("@IX_IssuedDateTime", SqlDbType.Int);


                        SqlParameter parxmlDetail = cmd.Parameters.Add("@XMLPrescriptionDetails", SqlDbType.Xml);

                        SqlParameter par13 = cmd.Parameters.Add("@NewPrescriptID", SqlDbType.BigInt);
                        par13.Direction = ParameterDirection.Output;
                        SqlParameter par14 = cmd.Parameters.Add("@IssueID", SqlDbType.BigInt);
                        par14.Direction = ParameterDirection.Output;

                        SqlParameter par17 = cmd.Parameters.Add("@HasAppointment", SqlDbType.Bit);
                        SqlParameter par15 = cmd.Parameters.Add("@PatientID", SqlDbType.BigInt);
                        SqlParameter par16 = cmd.Parameters.Add("@NDay", SqlDbType.Int);
                        SqlParameter par18 = cmd.Parameters.Add("@OrigCreatorDoctorNames", SqlDbType.NVarChar);
                        SqlParameter par19 = cmd.Parameters.Add("@DeptLocID", SqlDbType.BigInt);
                        SqlParameter par20 = cmd.Parameters.Add("@SecretaryStaffID", SqlDbType.BigInt);
                        SqlParameter par21 = cmd.Parameters.Add("@PtRegistrationID", SqlDbType.BigInt);
                        SqlParameter par22 = cmd.Parameters.Add("@HisID", SqlDbType.BigInt);
                        //▼====== #004
                        SqlParameter par23 = cmd.Parameters.Add("@PreNoDrug", SqlDbType.Bit);
                        SqlParameter par24 = cmd.Parameters.Add("@IsOutCatConfirmed", SqlDbType.Bit);
                        //▲====== #004
                        //20190518 TBL: Neu toa thuoc co canh bao ve tuong tac hoac tuong tu thi luu xuong bang de lam bao cao
                        SqlParameter par25 = cmd.Parameters.Add("@IsWarningSimilar", SqlDbType.Bit);
                        SqlParameter par26 = cmd.Parameters.Add("@IsWarningInteraction", SqlDbType.Bit);
                        //20190518
                        
                        //▼===== #005
                        SqlParameter par27 = cmd.Parameters.Add("@MedSecretaryID", SqlDbType.BigInt);
                        //▲===== #005

                        par1.Value = ConvertNullObjectToDBNull(entity.ServiceRecID);
                        par2.Value = ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.PtRegDetailID);

                        parIssuedDateTime.Value = ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.IssuedDateTime);

                        par3.Value = ConvertNullObjectToDBNull(entity.CreatorStaffID);
                        par4.Value = ConvertNullObjectToDBNull(entity.ConsultantID);
                        par5.Value = ConvertNullObjectToDBNull(entity.AppointmentDate);
                        par6.Value = ConvertNullObjectToDBNull(entity.Diagnosis);
                        par7.Value = ConvertNullObjectToDBNull(entity.DoctorAdvice);
                        par8.Value = ConvertNullObjectToDBNull(entity.ForOutPatient);
                        par9.Value = ConvertNullObjectToDBNull(entity.V_PrescriptionType);
                        par10.Value = ConvertNullObjectToDBNull(entity.V_PrescriptionNotes);
                        par11.Value = ConvertNullObjectToDBNull(entity.OriginalPrescriptID);
                        par18.Value = ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.OrigCreatorDoctorNames);

                        parSeqNum.Value = ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.StoreServiceSeqNum);
                        parSeqNumType.Value = ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.StoreServiceSeqNumType);

                        parIX_IssuedDateTime.Value = ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.IX_IssuedDateTime);

                        string xmlDetail = entity.ConvertPrescriptionDetailsWithScheduleToXML(entity.PrescriptionDetails);

                        parxmlDetail.Value = ConvertNullObjectToDBNull(xmlDetail);

                        par17.Value = ConvertNullObjectToDBNull(entity.HasAppointment);
                        par15.Value = ConvertNullObjectToDBNull(entity.PatientID);
                        par16.Value = ConvertNullObjectToDBNull(entity.NDay);
                        par19.Value = ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.DeptLocID);
                        par20.Value = ConvertNullObjectToDBNull(entity.SecretaryStaff == null ? 0 : entity.SecretaryStaff.StaffID);
                        par21.Value = ConvertNullObjectToDBNull(entity.PtRegistrationID);
                        par22.Value = ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.HisID);
                        //▼====== #004
                        par23.Value = ConvertNullObjectToDBNull(entity.PreNoDrug);
                        par24.Value = ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.IsOutCatConfirmed);
                        //▲====== #004
                        par25.Value = ConvertNullObjectToDBNull(entity.IsWarningSimilar);
                        par26.Value = ConvertNullObjectToDBNull(entity.IsWarningInteraction);
                        
                        //▼===== #005
                        par27.Value = ConvertNullObjectToDBNull(entity.MedSecretaryID);
                        //▲===== #005
                        cmd.AddParameter("@Reason", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Reason));
                        cmd.AddParameter("@IsOutsideRegimen", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.IsOutsideRegimen));
                        cmd.AddParameter("@PercentExceed", SqlDbType.SmallInt, ConvertNullObjectToDBNull(entity.PercentExceed));
                        //▼====== #008
                        //cmd.AddParameter("@V_PrescriptionIssuedType", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.V_PrescriptionIssuedType));
                        //▲====== #008
                        //▼===== #011
                        cmd.AddParameter("@UserOfficialAccountID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.UserOfficialAccountID));
                        //▲===== #011

                        cn.Open();
                        int retVal = ExecuteNonQuery(cmd);
                        if (cmd.Parameters["@NewPrescriptID"].Value != DBNull.Value)
                        {
                            newPrescriptID = (long)cmd.Parameters["@NewPrescriptID"].Value;
                        }
                        if (cmd.Parameters["@IssueID"].Value != DBNull.Value)
                        {
                            IssueID = (long)cmd.Parameters["@IssueID"].Value;
                        }


                        if (retVal > 0)
                        {
                            //Gọi hàm để báo là đã khám xong để a Tuấn trừ ra
                            ssnp.ConsuConsultationCompleted(DeptLocID);
                            //Gọi hàm để báo là đã khám xong để a Tuấn trừ ra
                        }
                        CleanUpConnectionAndCommand(cn, cmd);
                        return (retVal > 0);
                    }
                }
                else
                {
                    OutError = "Duplex-Prescriptions_PrescriptionsInDay" + "@" + drugList;//Toa Thuốc Bị Trùng Với 1 Toa Trong Ngày Hôm Nay
                    return false;
                }
            }
            else
            {
                OutError = "Chẩn đoán này đã có toa thuốc rồi!";//Toa Thuốc Bị Trùng Với 1 Toa Trong Ngày Hôm Nay
                return false;
            }
        }


        public bool Prescriptions_InPt_Add(Prescription entity, out long newPrescriptID, out long IssueID, out string OutError)
        {
            IssueID = 0;
            newPrescriptID = 0;
            OutError = "";
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spPrescriptions_InPt_Add", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter par1 = cmd.Parameters.Add("@ServiceRecID", SqlDbType.BigInt);

                    SqlParameter parIssuedDateTime = cmd.Parameters.Add("@IssuedDateTime", SqlDbType.DateTime);

                    SqlParameter par3 = cmd.Parameters.Add("@CreatorStaffID", SqlDbType.BigInt);
                    SqlParameter par4 = cmd.Parameters.Add("@ConsultantDoctorID", SqlDbType.BigInt);
                    SqlParameter par5 = cmd.Parameters.Add("@AppointmentDate", SqlDbType.DateTime);
                    SqlParameter par6 = cmd.Parameters.Add("@Diagnosis", SqlDbType.NVarChar);
                    SqlParameter par7 = cmd.Parameters.Add("@DoctorAdvice", SqlDbType.NVarChar);
                    SqlParameter par8 = cmd.Parameters.Add("@ForOutPatient", SqlDbType.Bit);
                    SqlParameter par9 = cmd.Parameters.Add("@V_PrescriptionType", SqlDbType.BigInt);
                    SqlParameter par10 = cmd.Parameters.Add("@V_PrescriptionNotes", SqlDbType.BigInt);
                    SqlParameter par11 = cmd.Parameters.Add("@OriginalPrescriptID", SqlDbType.BigInt);

                    SqlParameter parIX_IssuedDateTime = cmd.Parameters.Add("@IX_IssuedDateTime", SqlDbType.Int);


                    SqlParameter parxmlDetail = cmd.Parameters.Add("@XMLPrescriptionDetails", SqlDbType.Xml);

                    SqlParameter par13 = cmd.Parameters.Add("@NewPrescriptID", SqlDbType.BigInt);
                    par13.Direction = ParameterDirection.Output;
                    SqlParameter par14 = cmd.Parameters.Add("@IssueID", SqlDbType.BigInt);
                    par14.Direction = ParameterDirection.Output;

                    SqlParameter par17 = cmd.Parameters.Add("@HasAppointment", SqlDbType.Bit);
                    SqlParameter par15 = cmd.Parameters.Add("@PatientID", SqlDbType.BigInt);
                    SqlParameter par16 = cmd.Parameters.Add("@NDay", SqlDbType.Int);
                    SqlParameter par18 = cmd.Parameters.Add("@OrigCreatorDoctorNames", SqlDbType.NVarChar);
                    SqlParameter par19 = cmd.Parameters.Add("@DeptLocID", SqlDbType.BigInt);
                    SqlParameter par20 = cmd.Parameters.Add("@SecretaryStaffID", SqlDbType.BigInt);
                    SqlParameter par21 = cmd.Parameters.Add("@PtRegistrationID", SqlDbType.BigInt);
                    SqlParameter par22 = cmd.Parameters.Add("@HisID", SqlDbType.BigInt);
                    SqlParameter par23 = cmd.Parameters.Add("@IssueDeptID", SqlDbType.BigInt);

                    par1.Value = ConvertNullObjectToDBNull(entity.ServiceRecID);

                    parIssuedDateTime.Value = ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.IssuedDateTime);

                    par3.Value = ConvertNullObjectToDBNull(entity.CreatorStaffID);
                    par4.Value = ConvertNullObjectToDBNull(entity.ConsultantID);
                    par5.Value = ConvertNullObjectToDBNull(entity.AppointmentDate);
                    par6.Value = ConvertNullObjectToDBNull(entity.Diagnosis);
                    par7.Value = ConvertNullObjectToDBNull(entity.DoctorAdvice);
                    par8.Value = ConvertNullObjectToDBNull(entity.ForOutPatient);
                    par9.Value = ConvertNullObjectToDBNull(entity.V_PrescriptionType);
                    par10.Value = ConvertNullObjectToDBNull(entity.V_PrescriptionNotes);
                    par11.Value = ConvertNullObjectToDBNull(entity.OriginalPrescriptID);
                    par18.Value = ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.OrigCreatorDoctorNames);

                    parIX_IssuedDateTime.Value = ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.IX_IssuedDateTime);

                    string xmlDetail = entity.ConvertPrescriptionDetailsWithScheduleToXML(entity.PrescriptionDetails);

                    parxmlDetail.Value = ConvertNullObjectToDBNull(xmlDetail);

                    par17.Value = ConvertNullObjectToDBNull(entity.HasAppointment);
                    par15.Value = ConvertNullObjectToDBNull(entity.PatientID);
                    par16.Value = ConvertNullObjectToDBNull(entity.NDay);
                    par19.Value = ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.DeptLocID);
                    par20.Value = ConvertNullObjectToDBNull(entity.SecretaryStaff == null ? 0 : entity.SecretaryStaff.StaffID);
                    par21.Value = ConvertNullObjectToDBNull(entity.PtRegistrationID);
                    par22.Value = ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.HisID);
                    par23.Value = ConvertNullObjectToDBNull(entity.Department.DeptID);
                    cmd.AddParameter("@ConfirmedDTItemID", SqlDbType.BigInt, entity.ConfirmedDTItemID);
                    cn.Open();
                    int retVal = ExecuteNonQuery(cmd);
                    if (cmd.Parameters["@NewPrescriptID"].Value != DBNull.Value)
                    {
                        newPrescriptID = (long)cmd.Parameters["@NewPrescriptID"].Value;
                    }
                    if (cmd.Parameters["@IssueID"].Value != DBNull.Value)
                    {
                        IssueID = (long)cmd.Parameters["@IssueID"].Value;
                    }
                    CleanUpConnectionAndCommand(cn, cmd);
                    return (retVal > 0);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        #region "Kiểm tra trùng toa thuốc trước khi Dược Sĩ Sửa Toa Thuốc"
        private List<Prescription> SubTractPrecriptionsOfBacSiRaSai(List<Prescription> ListPrescriptions_TrongNgay, Prescription ToaChonDeSua)
        {
            foreach (Prescription item in ListPrescriptions_TrongNgay)
            {
                if (item.PrescriptID == ToaChonDeSua.PrescriptID)
                {
                    ListPrescriptions_TrongNgay.Remove(item);
                    break;
                }
            }
            return ListPrescriptions_TrongNgay;
        }
        private bool KiemTraTrungToaThuocTruocKhiDuocSiEditToaThuoc(out string druglist, Prescription entity, Prescription ToaChonDeSua)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                List<IList<PrescriptionDetail>> ListPrescriptionDetailTrongNgay = new List<IList<PrescriptionDetail>>();

                //Ds Toa Trong Ngày: Kiểm Tra Thuốc Toa Lần Này và Toa Trong Ngày Trùng Thuốc
                List<Prescription> ListPrescriptions_TrongNgay = Prescriptions_TrongNgay_ByPatientID(ToaChonDeSua.PatientID.Value);

                if (ListPrescriptions_TrongNgay.Count > 0)
                {
                    List<Prescription> ListPrescriptions_TrongNgayAfterSubtract = SubTractPrecriptionsOfBacSiRaSai(ListPrescriptions_TrongNgay, ToaChonDeSua);

                    foreach (Prescription item in ListPrescriptions_TrongNgayAfterSubtract)
                    {
                        ListPrescriptionDetailTrongNgay.Add(GetPrescriptionDetailsByPrescriptID(item.PrescriptID));
                    }

                    //if (CheckToaBiTrungTrongNgay(ListPrescriptionDetailTrongNgay, entity.PrescriptionDetails) == false)
                    //{
                    //    return true;
                    //}
                    //return false;
                    druglist = CheckToaThuocBiTrungTrongNgay(ListPrescriptionDetailTrongNgay, entity.PrescriptionDetails);
                    cn.Dispose();
                    if (druglist != "")
                    {
                        return false;
                    }
                    return true;
                }
                druglist = "";
                cn.Dispose();
                return true;
            }
        }
        #endregion

        public bool Prescriptions_DuocSiEdit(Prescription entity, Prescription entity_BacSi, out long newPrescriptID, out long IssueID, out string OutError)
        {
            newPrescriptID = 0;
            IssueID = 0;
            OutError = "";
            string drugList = "";
            if (KiemTraTrungToaThuocTruocKhiDuocSiEditToaThuoc(out drugList, entity, entity_BacSi))
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spPrescriptions_DuocSiEditV2", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter par1 = cmd.Parameters.Add("@ServiceRecID", SqlDbType.BigInt);
                    SqlParameter par2 = cmd.Parameters.Add("@PtRegDetailID", SqlDbType.BigInt);

                    SqlParameter parIssuedDateTime = cmd.Parameters.Add("@IssuedDateTime", SqlDbType.DateTime);

                    SqlParameter par3 = cmd.Parameters.Add("@CreatorStaffID", SqlDbType.BigInt);
                    SqlParameter par4 = cmd.Parameters.Add("@ConsultantDoctorID", SqlDbType.BigInt);
                    SqlParameter par5 = cmd.Parameters.Add("@AppointmentDate", SqlDbType.DateTime);
                    SqlParameter par6 = cmd.Parameters.Add("@Diagnosis", SqlDbType.NVarChar);
                    SqlParameter par7 = cmd.Parameters.Add("@DoctorAdvice", SqlDbType.NVarChar);
                    SqlParameter par8 = cmd.Parameters.Add("@ForOutPatient", SqlDbType.Bit);
                    SqlParameter par9 = cmd.Parameters.Add("@V_PrescriptionType", SqlDbType.BigInt);
                    SqlParameter par10 = cmd.Parameters.Add("@V_PrescriptionNotes", SqlDbType.BigInt);
                    SqlParameter par11 = cmd.Parameters.Add("@OriginalPrescriptID", SqlDbType.BigInt);

                    SqlParameter parIX_IssuedDateTime = cmd.Parameters.Add("@IX_IssuedDateTime", SqlDbType.Int);

                    SqlParameter parxmlDetail = cmd.Parameters.Add("@XMLPrescriptionDetails", SqlDbType.Xml);

                    SqlParameter par13 = cmd.Parameters.Add("@NewPrescriptID", SqlDbType.BigInt);
                    par13.Direction = ParameterDirection.Output;
                    SqlParameter par14 = cmd.Parameters.Add("@IssueID", SqlDbType.BigInt);
                    par14.Direction = ParameterDirection.Output;

                    SqlParameter parHasAppointment = cmd.Parameters.Add("@HasAppointment", SqlDbType.Bit);
                    SqlParameter parPatientID = cmd.Parameters.Add("@PatientID", SqlDbType.BigInt);
                    SqlParameter parNDay = cmd.Parameters.Add("@NDay", SqlDbType.Int);

                    SqlParameter par15 = cmd.Parameters.Add("@IssueID_BacSi", SqlDbType.BigInt);
                    SqlParameter par16 = cmd.Parameters.Add("@PrescriptID_BacSi", SqlDbType.BigInt);
                    SqlParameter par17 = cmd.Parameters.Add("@OriginalPrescriptID_BacSi", SqlDbType.BigInt);
                    SqlParameter par18 = cmd.Parameters.Add("@IssuerStaffID_BacSi", SqlDbType.BigInt);
                    SqlParameter par19 = cmd.Parameters.Add("@OrigCreatorDoctorNames", SqlDbType.NVarChar);
                    SqlParameter par20 = cmd.Parameters.Add("@SecretaryStaffID", SqlDbType.BigInt);



                    par1.Value = ConvertNullObjectToDBNull(entity.ServiceRecID);
                    par2.Value = ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.PtRegDetailID);

                    parIssuedDateTime.Value = ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.IssuedDateTime);

                    par3.Value = ConvertNullObjectToDBNull(entity.CreatorStaffID);
                    par4.Value = ConvertNullObjectToDBNull(entity.ConsultantID);
                    par5.Value = ConvertNullObjectToDBNull(entity.AppointmentDate);
                    par6.Value = ConvertNullObjectToDBNull(entity.Diagnosis);
                    par7.Value = ConvertNullObjectToDBNull(entity.DoctorAdvice);
                    par8.Value = ConvertNullObjectToDBNull(entity.ForOutPatient);
                    par9.Value = ConvertNullObjectToDBNull(entity.V_PrescriptionType);
                    par10.Value = ConvertNullObjectToDBNull(entity.V_PrescriptionNotes);
                    par11.Value = ConvertNullObjectToDBNull(entity.OriginalPrescriptID);

                    parIX_IssuedDateTime.Value = ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.IX_IssuedDateTime);

                    string xmlDetail = entity.ConvertPrescriptionDetailsWithScheduleToXML(entity.PrescriptionDetails);

                    parxmlDetail.Value = xmlDetail;

                    parHasAppointment.Value = ConvertNullObjectToDBNull(entity.HasAppointment);
                    parPatientID.Value = ConvertNullObjectToDBNull(entity.PatientID);
                    parNDay.Value = ConvertNullObjectToDBNull(entity.NDay);

                    par15.Value = ConvertNullObjectToDBNull(entity_BacSi.IssueID);
                    par16.Value = ConvertNullObjectToDBNull(entity_BacSi.PrescriptID);
                    par17.Value = ConvertNullObjectToDBNull(entity_BacSi.OriginalPrescriptID);
                    par18.Value = ConvertNullObjectToDBNull(entity_BacSi.ObjIssuerStaffID.StaffID);
                    par19.Value = ConvertNullObjectToDBNull(entity_BacSi.PrescriptionIssueHistory.OrigCreatorDoctorNames);
                    par20.Value = ConvertNullObjectToDBNull(entity_BacSi.SecretaryStaff == null ? 0 : entity_BacSi.SecretaryStaff.StaffID);


                    cn.Open();
                    int retVal = ExecuteNonQuery(cmd);
                    newPrescriptID = (long)cmd.Parameters["@NewPrescriptID"].Value;
                    IssueID = (long)cmd.Parameters["@IssueID"].Value;
                    CleanUpConnectionAndCommand(cn, cmd);
                    return (retVal > 0);
                }
            }
            else
            {
                OutError = "Duplex-Prescriptions_PrescriptionsInDay" + "@" + drugList;//Toa Thuốc Bị Trùng Với 1 Toa Trong Ngày Hôm Nay
                return false;
            }
        }


        public bool Prescriptions_DuocSiEditDuocSi(Prescription entity, Prescription entity_DuocSi, out long newPrescriptID, out long IssueID, out string OutError)
        {
            newPrescriptID = 0;
            IssueID = 0;
            OutError = "";
            string drugList = "";
            if (KiemTraTrungToaThuocTruocKhiDuocSiEditToaThuoc(out drugList, entity, entity_DuocSi))
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spPrescriptions_DuocSiEditDuocSiV2", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter par1 = cmd.Parameters.Add("@ServiceRecID", SqlDbType.BigInt);
                    SqlParameter par2 = cmd.Parameters.Add("@PtRegDetailID", SqlDbType.BigInt);
                    SqlParameter par3 = cmd.Parameters.Add("@CreatorStaffID", SqlDbType.BigInt);
                    SqlParameter par4 = cmd.Parameters.Add("@ConsultantDoctorID", SqlDbType.BigInt);
                    SqlParameter par5 = cmd.Parameters.Add("@AppointmentDate", SqlDbType.DateTime);

                    SqlParameter par6 = cmd.Parameters.Add("@Diagnosis", SqlDbType.NVarChar);
                    SqlParameter par6new = cmd.Parameters.Add("@DTItemID", SqlDbType.BigInt);
                    SqlParameter par7 = cmd.Parameters.Add("@DoctorAdvice", SqlDbType.NVarChar);
                    SqlParameter par8 = cmd.Parameters.Add("@ForOutPatient", SqlDbType.Bit);
                    SqlParameter par9 = cmd.Parameters.Add("@V_PrescriptionType", SqlDbType.BigInt);
                    SqlParameter par10 = cmd.Parameters.Add("@V_PrescriptionNotes", SqlDbType.BigInt);
                    SqlParameter par11 = cmd.Parameters.Add("@OriginalPrescriptID", SqlDbType.BigInt);


                    SqlParameter parIssuedDateTime = cmd.Parameters.Add("@IssuedDateTime", SqlDbType.DateTime);
                    SqlParameter parIX_IssuedDateTime = cmd.Parameters.Add("@IX_IssuedDateTime", SqlDbType.Int);


                    SqlParameter parxmlDetail = cmd.Parameters.Add("@XMLPrescriptionDetails", SqlDbType.Xml);


                    SqlParameter par13 = cmd.Parameters.Add("@NewPrescriptID", SqlDbType.BigInt);
                    par13.Direction = ParameterDirection.Output;
                    SqlParameter par14 = cmd.Parameters.Add("@IssueID", SqlDbType.BigInt);
                    par14.Direction = ParameterDirection.Output;

                    SqlParameter parHasAppointment = cmd.Parameters.Add("@HasAppointment", SqlDbType.Bit);
                    SqlParameter parPatientID = cmd.Parameters.Add("@PatientID", SqlDbType.BigInt);
                    SqlParameter parNDay = cmd.Parameters.Add("@NDay", SqlDbType.Int);

                    SqlParameter par15 = cmd.Parameters.Add("@IssueID_DuocSi", SqlDbType.BigInt);
                    SqlParameter par16 = cmd.Parameters.Add("@PrescriptID_DuocSi", SqlDbType.BigInt);
                    SqlParameter par17 = cmd.Parameters.Add("@OriginalPrescriptID_DuocSi", SqlDbType.BigInt);
                    SqlParameter par18 = cmd.Parameters.Add("@PtRegistrationID", SqlDbType.BigInt);
                    cmd.AddParameter("@HisID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.HisID));


                    par1.Value = ConvertNullObjectToDBNull(entity.ServiceRecID);
                    par2.Value = ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.PtRegDetailID);
                    par3.Value = ConvertNullObjectToDBNull(entity.CreatorStaffID);
                    par4.Value = ConvertNullObjectToDBNull(entity.ConsultantID);
                    par5.Value = ConvertNullObjectToDBNull(entity.AppointmentDate);
                    par6.Value = ConvertNullObjectToDBNull(entity.Diagnosis);
                    par7.Value = ConvertNullObjectToDBNull(entity.DoctorAdvice);
                    par8.Value = ConvertNullObjectToDBNull(entity.ForOutPatient);
                    par9.Value = ConvertNullObjectToDBNull(entity.V_PrescriptionType);
                    par10.Value = ConvertNullObjectToDBNull(entity.V_PrescriptionNotes);
                    par11.Value = ConvertNullObjectToDBNull(entity.OriginalPrescriptID);
                    par18.Value = ConvertNullObjectToDBNull(entity.PtRegistrationID);

                    parIssuedDateTime.Value = ConvertNullObjectToDBNull(entity.IssuedDateTime);
                    parIX_IssuedDateTime.Value = ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.IX_IssuedDateTime);


                    string xmlDetail = entity.ConvertPrescriptionDetailsWithScheduleToXML(entity.PrescriptionDetails);

                    parxmlDetail.Value = xmlDetail;

                    parHasAppointment.Value = ConvertNullObjectToDBNull(entity.HasAppointment);
                    parPatientID.Value = ConvertNullObjectToDBNull(entity.PatientID);
                    parNDay.Value = ConvertNullObjectToDBNull(entity.NDay);

                    par15.Value = ConvertNullObjectToDBNull(entity_DuocSi.IssueID);
                    par16.Value = ConvertNullObjectToDBNull(entity_DuocSi.PrescriptID);
                    par17.Value = ConvertNullObjectToDBNull(entity_DuocSi.OriginalPrescriptID);


                    cn.Open();
                    int retVal = ExecuteNonQuery(cmd);
                    newPrescriptID = (long)cmd.Parameters["@NewPrescriptID"].Value;
                    IssueID = (long)cmd.Parameters["@IssueID"].Value;
                    CleanUpConnectionAndCommand(cn, cmd);
                    return (retVal > 0);
                }
            }
            else
            {
                OutError = "Duplex-Prescriptions_PrescriptionsInDay" + "@" + drugList;//Toa Thuốc Bị Trùng Với 1 Toa Trong Ngày Hôm Nay
                return false;
            }
        }


        public List<Prescription> Prescriptions_TrongNgay_ByPatientID(Int64 PatientID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPrescriptions_TrongNgay_ByPatientID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));

                cn.Open();
                List<Prescription> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPtPrescriptionCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        public Prescription Prescription_ByPrescriptIDIssueID(Int64 PrescriptID, Int64 IssueID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPrescription_ByPrescriptIDIssueID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PrescriptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PrescriptID));
                cmd.AddParameter("@IssueID", SqlDbType.BigInt, ConvertNullObjectToDBNull(IssueID));

                cn.Open();
                Prescription objItem = null;
                IDataReader reader = ExecuteReader(cmd);
                objItem = GetPtPrescriptionItemFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objItem;
            }

        }

        public List<DiagnosisIcd10Items> GetAllDiagnosisIcd10Items_InDay(long PatientID, long ServiceRecID, long PtRegDetailID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllDiagnosisIcd10Items_InDay", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                cmd.AddParameter("@ServiceRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ServiceRecID));
                cmd.AddParameter("@PtRegDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegDetailID));

                cn.Open();
                List<DiagnosisIcd10Items> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetDiagnosisIcd10ItemsCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        public void Prescriptions_UpdateDoctorAdvice(Prescription entity, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spPrescriptions_UpdateDoctorAdvice", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PrescriptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PrescriptID));
                cmd.AddParameter("@DoctorAdvice", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DoctorAdvice.Trim()));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        public List<Prescription> Prescriptions_ListRootByPatientID_Paging(
            PrescriptionSearchCriteria SearchCriteria,

            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPrescriptions_ListRootByPatientID_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@CreatorStaffIDName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria.CreatorStaffIDName));
                cmd.AddParameter("@Diagnosis", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria.Diagnosis));
                cmd.AddParameter("@DoctorStaffIDName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria.DoctorStaffIDName));
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PatientID));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<Prescription> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPtPrescriptionCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;
                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        #endregion

        #region 2.GetDrugForPrescription
        public IList<GetDrugForSellVisitor> GetDrugForPrescription_Auto(String BrandName, int IsInsurance, int PageSize, int PageIndex, int IsMedDept, long? StoreID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetDrugInfoForPrescription_Auto", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@BrandName", SqlDbType.NVarChar, BrandName);
                cmd.AddParameter("@IsInsurance", SqlDbType.SmallInt, IsInsurance);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@IsMedDept", SqlDbType.Int, IsMedDept);
                cmd.AddParameter("@StoreID", SqlDbType.BigInt, StoreID);
                cn.Open();
                IList<GetDrugForSellVisitor> GetDrugForPrescriptions = null;
                IDataReader reader = ExecuteReader(cmd);
                GetDrugForPrescriptions = GetDrugForSellVisitorCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return GetDrugForPrescriptions;
            }
        }

        public IList<GetDrugForSellVisitor> SearchDrugForPrescription_Paging(String BrandName, bool IsSearchByGenericName, int IsInsurance, long? StoreID, int PageIndex, int PageSize, bool CountTotal, out int Total)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSearchDrugForPrescription_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@BrandName", SqlDbType.NVarChar, BrandName);
                cmd.AddParameter("@IsSearchByGenericName", SqlDbType.Bit, IsSearchByGenericName);
                cmd.AddParameter("@IsInsurance", SqlDbType.SmallInt, IsInsurance);
                cmd.AddParameter("@StoreID", SqlDbType.BigInt, StoreID);

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                IList<GetDrugForSellVisitor> lst = null;
                IDataReader reader = ExecuteReader(cmd);
                lst = GetDrugForSellVisitorCollectionFromReader(reader);
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;
                CleanUpConnectionAndCommand(cn, cmd);
                return lst;

            }
        }
        /*▼====: #001*/
        //ConvertToXML
        public string ConvertDetailsListToXmlForDiagnosisIcd10Items(List<string> listICD10Codes)
        {
            StringBuilder sb = new StringBuilder();
            if (listICD10Codes != null)
            {
                sb.Append("<Root>");
                foreach (string icdCode in listICD10Codes)
                {
                    sb.Append("<ICDCodeList>");
                    sb.AppendFormat("<ICDCode>{0}</ICDCode>", icdCode);
                    sb.Append("</ICDCodeList>");
                }
                sb.Append("</Root>");
            }
            return sb.ToString();
        }
        public IList<GetDrugForSellVisitor> GetDrugsInTreatmentRegimen(long PtRegDetailID, List<string> listICD10Codes)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = null;
                if (listICD10Codes != null && listICD10Codes.Count > 0)
                {
                    cmd = new SqlCommand("spGetDrugsInTreatmentRegimen_ByICDCodes", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@ICDCodes_XML", SqlDbType.Xml, ConvertDetailsListToXmlForDiagnosisIcd10Items(listICD10Codes));
                }
                else if (PtRegDetailID > 0)
                {
                    cmd = new SqlCommand("spGetDrugsInTreatmentRegimen", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PtRegDetailID", SqlDbType.BigInt, PtRegDetailID);
                }
                cn.Open();
                IList<GetDrugForSellVisitor> mCollection = null;
                IDataReader reader = ExecuteReader(cmd);
                mCollection = GetDrugForSellVisitorCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return mCollection;
            }
        }
        /*▲====: #001*/
        public IList<RefTreatmentRegimen> GetRefTreatmentRegimensAndDetail(long? TreatmentRegimenID = null, long? PtRegDetailID = null, List<string> listICD10Codes = null, long? PtRegistrationID = null)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("", cn);
                if (PtRegistrationID.GetValueOrDefault(0) > 0)
                {
                    cmd.CommandText = "spGetRefTreatmentRegimensAndDetail";
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, PtRegistrationID);
                }
                else if (PtRegDetailID.GetValueOrDefault(0) > 0)
                {
                    if (listICD10Codes != null && listICD10Codes.Count > 0)
                    {
                        cmd.CommandText = "spGetRefTreatmentRegimensAndDetailByICDCodes";
                        cmd.AddParameter("@ICDCodes_XML", SqlDbType.Xml, ConvertDetailsListToXmlForDiagnosisIcd10Items(listICD10Codes));
                    }
                    else
                    {
                        cmd.CommandText = "spGetRefTreatmentRegimensAndDetail";
                        cmd.AddParameter("@PtRegDetailID", SqlDbType.BigInt, PtRegDetailID);
                    }
                }
                else
                {
                    cmd.CommandText = "spGetRefTreatmentRegimensAndDetail";
                    cmd.AddParameter("@TreatmentRegimenID", SqlDbType.BigInt, TreatmentRegimenID);
                }
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                List<RefTreatmentRegimen> mCollection = GetRefTreatmentRegimenCollectionAndDetailFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return mCollection;
            }
        }

        public bool EditRefTreatmentRegimen(RefTreatmentRegimen aRefTreatmentRegimen, out RefTreatmentRegimen aOutRefTreatmentRegimen)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spEditRefTreatmentRegimen", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@TreatmentRegimenID", SqlDbType.BigInt, aRefTreatmentRegimen.TreatmentRegimenID);
                cmd.AddParameter("@TreatmentRegimenCode", SqlDbType.VarChar, aRefTreatmentRegimen.TreatmentRegimenCode);
                cmd.AddParameter("@TreatmentRegimenName", SqlDbType.NVarChar, aRefTreatmentRegimen.TreatmentRegimenName);
                cmd.AddParameter("@ICD10Code", SqlDbType.VarChar, aRefTreatmentRegimen.ICD10Code);
                cmd.AddParameter("@TreatmentRegimenNote", SqlDbType.NVarChar, aRefTreatmentRegimen.TreatmentRegimenNote);
                cmd.AddParameter("@IsDeleted", SqlDbType.Bit, aRefTreatmentRegimen.IsDeleted);
                cmd.AddParameter("@LastUpdatedStaffID", SqlDbType.BigInt, aRefTreatmentRegimen.LastUpdatedStaffID);
                cmd.AddParameter("@RefTreatmentRegimenDrugDetails", SqlDbType.Xml, aRefTreatmentRegimen.RefTreatmentRegimenDrugDetails.ConvertToXML());
                cmd.AddParameter("@RefTreatmentRegimenPCLDetails", SqlDbType.Xml, aRefTreatmentRegimen.RefTreatmentRegimenPCLDetails.ConvertToXML());
                cmd.AddParameter("@RefTreatmentRegimenServiceDetails", SqlDbType.Xml, aRefTreatmentRegimen.RefTreatmentRegimenServiceDetails.ConvertToXML());
                cmd.AddParameter("@OutTreatmentRegimenID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                cn.Open();
                bool mResult = ExecuteNonQuery(cmd) > 0;
                if (cmd.Parameters["@OutTreatmentRegimenID"].Value != null)
                {
                    var OutTreatmentRegimenID = (long)cmd.Parameters["@OutTreatmentRegimenID"].Value;
                    aOutRefTreatmentRegimen = GetRefTreatmentRegimensAndDetail(OutTreatmentRegimenID).FirstOrDefault();
                }
                else
                    aOutRefTreatmentRegimen = null;
                CleanUpConnectionAndCommand(cn, cmd);
                return mResult;
            }
        }

        public IList<GetDrugForSellVisitor> SearchGenMedProductForPrescription_Paging(String BrandName, int IsInsurance, long? StoreID, int PageIndex, int PageSize, bool CountTotal, bool IsSearchByGenericName, out int Total)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSearchGenMedProductForPrescription_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@BrandName", SqlDbType.NVarChar, BrandName);
                cmd.AddParameter("@IsInsurance", SqlDbType.SmallInt, IsInsurance);
                cmd.AddParameter("@StoreID", SqlDbType.BigInt, StoreID);

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@IsSearchByGenericName", SqlDbType.Bit, IsSearchByGenericName);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                IList<GetDrugForSellVisitor> lst = null;
                IDataReader reader = ExecuteReader(cmd);
                lst = GetDrugForSellVisitorCollectionFromReader(reader);
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;
                CleanUpConnectionAndCommand(cn, cmd);
                return lst;

            }
        }

        public IList<GetDrugForSellVisitor> GetDrugForPrescription_Remaining(long? StoreID, string xml)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetDrugForPrescription_Remaining", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@XML", SqlDbType.Xml, ConvertNullObjectToDBNull(xml));
                cmd.AddParameter("@StoreID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StoreID));
                cn.Open();
                IList<GetDrugForSellVisitor> lst = null;
                IDataReader reader = ExecuteReader(cmd);
                lst = GetDrugForSellVisitorCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return lst;

            }
        }

        public IList<GetDrugForSellVisitor> GetListDrugPatientUsed(long PatientID, int PageIndex, int PageSize, out int Total)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPrescriptionDetails_AllDrug_ByPatientID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, PatientID);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                IList<GetDrugForSellVisitor> lst = null;
                IDataReader reader = ExecuteReader(cmd);
                lst = GetDrugForSellVisitorCollectionFromReader(reader);
                reader.Close();
                if (cmd.Parameters["@Total"].Value != DBNull.Value)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;
                CleanUpConnectionAndCommand(cn, cmd);
                return lst;

            }
        }
        #endregion

        #region 3.Prescription Details

        public IList<PrescriptionDetail> GetPrescriptionDetailsByPrescriptID(long PrescriptID, bool GetRemaining = false)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPrescriptionDetails_LoadID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PrescriptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PrescriptID));
                cmd.AddParameter("@GetRemaining", SqlDbType.Bit, ConvertNullObjectToDBNull(GetRemaining));
                cn.Open();
                List<PrescriptionDetail> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPrescriptionDetailCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }
        public IList<PrescriptionDetail> GetPrescriptionDetailsByPrescriptID_WithNDay(long PrescriptID, out int NDay, bool GetRemaining = false)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPrescriptionDetails_LoadID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PrescriptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PrescriptID));
                cmd.AddParameter("@GetRemaining", SqlDbType.Bit, ConvertNullObjectToDBNull(GetRemaining));
                cn.Open();
                List<PrescriptionDetail> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPrescriptionDetailCollectionFromReader(reader);
                NDay = 0;
                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        if (reader[0] != null && reader[0] != DBNull.Value)
                        {
                            int.TryParse(reader[0].ToString(), out NDay);
                        }
                    }
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }
        //▼====== #003
        public IList<PrescriptionDetail> GetPrescriptionDetailsByPrescriptID_V2(long PrescriptID, long? IssueID, long? AppointmentID, out bool CanEdit, out string ReasonCanEdit, out bool IsEdit, bool GetRemaining = false)
        {
            CanEdit = false;
            ReasonCanEdit = "";
            IsEdit = false;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spCheckAllowEditNDay", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@AppointmentID", SqlDbType.BigInt, ConvertNullObjectToDBNull(AppointmentID));
                cmd.AddParameter("@IsEdit", SqlDbType.Bit, DBNull.Value, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@IsEdit"].Value != null)
                    IsEdit = Convert.ToBoolean(cmd.Parameters["@IsEdit"].Value);
                CleanUpConnectionAndCommand(cn, cmd);
            }

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                CanEdit = false;
                ReasonCanEdit = "";

                SqlCommand cmd = new SqlCommand("spPrescriptions_CheckCanEdit", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PrescriptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PrescriptID));
                cmd.AddParameter("@IssueID", SqlDbType.BigInt, ConvertNullObjectToDBNull(IssueID));

                cmd.AddParameter("@CanEdit", SqlDbType.Bit, DBNull.Value, ParameterDirection.Output);
                cmd.AddParameter("@ReasonCanEdit", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@CanEdit"].Value != DBNull.Value)
                    CanEdit = Convert.ToBoolean(cmd.Parameters["@CanEdit"].Value);

                if (cmd.Parameters["@ReasonCanEdit"].Value != DBNull.Value)
                    ReasonCanEdit = cmd.Parameters["@ReasonCanEdit"].Value.ToString();
                CleanUpConnectionAndCommand(cn, cmd);
            }

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPrescriptionDetails_LoadID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PrescriptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PrescriptID));
                cmd.AddParameter("@GetRemaining", SqlDbType.Bit, ConvertNullObjectToDBNull(GetRemaining));
                cn.Open();
                List<PrescriptionDetail> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPrescriptionDetailCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }
        //▲====== #003
        public IList<PrescriptionDetail> GetPrescriptionDetailsByPrescriptID_InPt(long PrescriptID, long[] V_CatDrugType = null, bool GetRemaining = false, bool OutPatient = false)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPrescriptionDetails_InPt_LoadID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PrescriptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PrescriptID));
                if (V_CatDrugType != null && V_CatDrugType.Length > 0)
                {
                    cmd.AddParameter("@V_CatDrugType", SqlDbType.VarChar, string.Join(",", V_CatDrugType));
                }
                cmd.AddParameter("@GetRemaining", SqlDbType.Bit, ConvertNullObjectToDBNull(GetRemaining));
                cmd.AddParameter("@OutPatient", SqlDbType.Bit, ConvertNullObjectToDBNull(OutPatient));
                cn.Open();
                List<PrescriptionDetail> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPrescriptionDetailCollectionFromReader_InPt(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        public void UpdateDrugNotDisplayInList(long PatientID, long DrugID, bool? NotDisplayInList)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPrescriptionDetails_UpdateNotDisplayInList", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                cmd.AddParameter("@DrugID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DrugID));
                cmd.AddParameter("@NotDisplayInList", SqlDbType.Bit, ConvertNullObjectToDBNull(NotDisplayInList));
                cn.Open();
                cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
            }
        }
        #endregion

        #region 4.patientPaymentOld
        public FeeDrug GetValuePatientPaymentOld(long PtRegistrationID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientTransactionDetail_PtRegistrationID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, PtRegistrationID);
                cn.Open();
                FeeDrug Patientpayment = null;
                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    Patientpayment = GetPatientPaymentOldFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return Patientpayment;
            }
        }
        #endregion

        #region 5.PrescriptionIssueHistory

        public IList<PrescriptionIssueHistory> GetPrescriptionIssueHistory(long prescriptID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPrescriptionIssueHistory_ByPrescriptID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PrescriptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(prescriptID));

                cn.Open();
                List<PrescriptionIssueHistory> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPtPrescriptIssueHisCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }

        }

        public IList<PrescriptionIssueHistory> PrescIssueHistoryByPtRegisID(long PtRegistrationID, bool IsHI = true)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPrescIssueHistoryByPtRegisID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@IsHI", SqlDbType.BigInt, ConvertNullObjectToDBNull(IsHI));

                cn.Open();
                List<PrescriptionIssueHistory> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPtPrescriptIssueHisCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }

        }

        public IList<PrescriptionIssueHistory> GetPrescriptionIssueHistoryBySerRecID(long ServiceRecID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPrescriptionIssueHistoryGetBySerRecID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@ServiceRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ServiceRecID));

                cn.Open();
                List<PrescriptionIssueHistory> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPtPrescriptIssueHisCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }

        }

        private bool AddPrescriptIssueHistory_Function(Int16 NumberTypePrescriptions_Rule, Prescription entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPrescriptionIssueHistory_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@IssueID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.IssueID));
                cmd.AddParameter("@ServiceRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ServiceRecID));
                cmd.AddParameter("@PtRegDetailID", SqlDbType.BigInt,
                                 ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.PtRegDetailID));
                cmd.AddParameter("@ReIssuerStaffID", SqlDbType.BigInt,
                                 ConvertNullObjectToDBNull(entity.ObjReIssuerStaffID.StaffID));


                cmd.AddParameter("@StoreServiceSeqNum", SqlDbType.SmallInt,
                                 ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.StoreServiceSeqNum));
                cmd.AddParameter("@StoreServiceSeqNumType", SqlDbType.TinyInt, ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.StoreServiceSeqNumType));

                cmd.AddParameter("@IssuedDateTime", SqlDbType.DateTime,
                                 ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.IssuedDateTime));
                cmd.AddParameter("@IX_IssuedDateTime", SqlDbType.Int, ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.IX_IssuedDateTime));

                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PatientID));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PtRegistrationID));
                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjDoctorStaffID.StaffID));
                cmd.AddParameter("@HasAppointment", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.HasAppointment));
                cmd.AddParameter("@NDay", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.NDay));
                cmd.AddParameter("@IssueIDOld", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.IssueIDOld));
                cmd.AddParameter("@OrigCreatorDoctorNames", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.OrigCreatorDoctorNames));
                cmd.AddParameter("@SecretaryStaffID", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.SecretaryStaff == null ? 0 : entity.SecretaryStaff.StaffID));
                cmd.AddParameter("@HisID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PrescriptionIssueHistory.HisID));
                SqlParameter par20 = cmd.Parameters.Add("@", SqlDbType.BigInt);

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                string Result = "";

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != DBNull.Value)
                    Result = cmd.Parameters["@Result"].Value.ToString();
                CleanUpConnectionAndCommand(cn, cmd);
                if (Result == "OK")
                {
                    return true;
                }
                return false;
            }
        }

        //KMx: Sau khi kiểm tra toàn bộ chương trình, thấy hàm này không được gọi (không được sử dụng) nữa (22/02/2014 16:38).
        public bool AddPrescriptIssueHistory(Int16 NumberTypePrescriptions_Rule, Prescription entity, out string OutError)
        {
            OutError = "";
            //if (entity.PrescriptionIssueHistory.IssueIDOld.GetValueOrDefault(0) > 0)
            //{
            //    return AddPrescriptIssueHistory_Function(NumberTypePrescriptions_Rule, entity);
            //}
            //else
            //{
            string drugList = "";
            if (KiemTraTrungToaThuocTruocKhiAddNew(out drugList, entity))
            {
                //phat hanh lai thi co can lay lai so hay ko?
                //Lấy số
                ServiceSequenceNumberProvider ssnp = new ServiceSequenceNumberProvider();
                byte seqNumberType;
                uint SeqNumber;

                try
                {
                    if (NumberTypePrescriptions_Rule == 2)
                    {
                        if (!string.IsNullOrEmpty(entity.HICardNo))
                        {
                            ssnp.PharmacyServiceNumber(out seqNumberType, PharmacyInvType.CO_BAO_HIEM, out SeqNumber);
                        }
                        else
                        {
                            ssnp.PharmacyServiceNumber(out seqNumberType, PharmacyInvType.KHONG_BAO_HIEM, out SeqNumber);
                        }
                    }
                    else
                    {
                        ssnp.PharmacyServiceNumber(out seqNumberType, PharmacyInvType.KHONG_BAO_HIEM, out SeqNumber);
                    }

                    entity.PrescriptionIssueHistory.StoreServiceSeqNumType = seqNumberType;
                    entity.PrescriptionIssueHistory.StoreServiceSeqNum = (short)SeqNumber;
                }
                catch (Exception ex)
                {
                    OutError = ex.ToString();
                    return false;
                }
                //Lấy số
                if (AddPrescriptIssueHistory_Function(NumberTypePrescriptions_Rule, entity))
                {
                    ssnp.ConsuConsultationCompleted(DeptLocID);
                    return true;
                }
                else
                { return false; }

            }
            else
            {
                OutError = "Duplex-Prescriptions_PrescriptionsInDay" + "@" + drugList;//Toa Thuốc Bị Trùng Với 1 Toa Trong Ngày Hôm Nay
                return false;
            }
            // }
        }

        #endregion

        #region 6.choose dose member
        public List<ChooseDose> InitChooseDoses()
        {
            return InitChooseDose();
        }
        #endregion

        #region 7.PrescriptionDetailSchedules
        public List<PrescriptionDetailSchedules> PrescriptionDetailSchedules_ByPrescriptDetailID(Int64 PrescriptDetailID, bool IsNotIncat)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPrescriptionDetailSchedules_ByPrescriptDetailID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PrescriptDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PrescriptDetailID));
                cmd.AddParameter("@IsNotIncat", SqlDbType.BigInt, ConvertNullObjectToDBNull(IsNotIncat));

                cn.Open();
                List<PrescriptionDetailSchedules> objList = null;
                IDataReader reader = ExecuteReader(cmd);
                objList = GetPrescriptionDetailSchedulesColectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }

        }

        public List<PrescriptionDetailSchedules> PrescriptionDetailSchedules_ByPrescriptDetailID_InPt(Int64 PrescriptDetailID, bool IsNotIncat)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPrescriptionDetailSchedules_ByPrescriptDetailID_InPt", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PrescriptDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PrescriptDetailID));
                cmd.AddParameter("@IsNotIncat", SqlDbType.BigInt, ConvertNullObjectToDBNull(IsNotIncat));

                cn.Open();
                List<PrescriptionDetailSchedules> objList = null;
                IDataReader reader = ExecuteReader(cmd);
                objList = GetPrescriptionDetailSchedulesColectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }

        }

        #endregion

        #region 8.PrescriptionDetailSchedulesLieuDung
        public List<PrescriptionDetailSchedulesLieuDung> InitChoosePrescriptionDetailSchedulesLieuDung()
        {
            return InitValuePrescriptionDetailSchedulesLieuDung();
        }
        #endregion

        #region 9.Check Toa Được Phép Sửa
        public void Prescriptions_CheckCanEdit(Int64 PrescriptID, Int64 IssueID, out bool CanEdit, out string ReasonCanEdit)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                CanEdit = false;
                ReasonCanEdit = "";

                SqlCommand cmd = new SqlCommand("spPrescriptions_CheckCanEdit", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PrescriptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PrescriptID));
                cmd.AddParameter("@IssueID", SqlDbType.BigInt, ConvertNullObjectToDBNull(IssueID));

                cmd.AddParameter("@CanEdit", SqlDbType.Bit, DBNull.Value, ParameterDirection.Output);
                cmd.AddParameter("@ReasonCanEdit", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@CanEdit"].Value != DBNull.Value)
                    CanEdit = Convert.ToBoolean(cmd.Parameters["@CanEdit"].Value);

                if (cmd.Parameters["@ReasonCanEdit"].Value != DBNull.Value)
                    ReasonCanEdit = cmd.Parameters["@ReasonCanEdit"].Value.ToString();
                CleanUpConnectionAndCommand(cn, cmd);
            }
        }
        #endregion


        #region PrescriptionNoteTemplates
        public IList<PrescriptionNoteTemplates> PrescriptionNoteTemplates_GetAll()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPrescriptionNoteTemplates_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                List<PrescriptionNoteTemplates> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPrescriptionNoteTemplatesColectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        public IList<PrescriptionNoteTemplates> PrescriptionNoteTemplates_GetAllIsActive(PrescriptionNoteTemplates Obj)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPrescriptionNoteTemplates_GetAllIsActive", cn);
                cmd.AddParameter("@V_PrescriptionNoteTempType", SqlDbType.BigInt, ConvertNullObjectToDBNull((long)Obj.V_PrescriptionNoteTempType));
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                List<PrescriptionNoteTemplates> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPrescriptionNoteTemplatesColectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        public void PrescriptionNoteTemplates_Save(PrescriptionNoteTemplates Obj, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spPrescriptionNoteTemplates_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PrescriptNoteTemplateID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PrescriptNoteTemplateID));
                cmd.AddParameter("@NoteDetails", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.NoteDetails));
                cmd.AddParameter("@DetailsTemplate", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.DetailsTemplate));
                cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsActive));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.StaffID));
                cmd.AddParameter("@V_PrescriptionNoteTempType", SqlDbType.BigInt, ConvertNullObjectToDBNull((long)Obj.V_PrescriptionNoteTempType));
                cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        #endregion

        public bool PrescriptionsTemplateInsert(PrescriptionTemplate Obj)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPrescriptionsTemplateInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.DoctorStaffID));
                cmd.AddParameter("@PrescriptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PrescriptID));
                cmd.AddParameter("@Comment", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.Comment));
                cmd.AddParameter("@StaffID", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.StaffID));
                //cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();
                var result = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
                return result > 0;
                //ExecuteNonQuery(cmd);

                //if (cmd.Parameters["@Result"].Value != null)
                //    Result = cmd.Parameters["@Result"].Value.ToString();
            }
        }

        public bool PrescriptionsTemplateDelete(PrescriptionTemplate Obj)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPrescriptionsTemplateDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PrescriptionsTemplateID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PrescriptionTemplateID));
                cn.Open();
                var result = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
                return result > 0;
            }
        }

        public List<PrescriptionTemplate> PrescriptionsTemplateGetAll(PrescriptionTemplate Obj)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {

                SqlCommand cmd = new SqlCommand("spPrescriptionsTemplateGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.DoctorStaffID));

                cn.Open();
                List<PrescriptionTemplate> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPrescriptionTemplateColectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        public void GetAppointmentID(long issueID, bool isInPatient, out long? appointmentID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                appointmentID = null;

                SqlCommand cmd = new SqlCommand("spGetAppointmentIDByIssueID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@IssueID", SqlDbType.BigInt, ConvertNullObjectToDBNull(issueID));
                cmd.AddParameter("@IsInPatient", SqlDbType.Bit, ConvertNullObjectToDBNull(isInPatient));
                SqlParameter pareID = new SqlParameter("@AppointmentID", SqlDbType.BigInt);
                pareID.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(pareID);
                cn.Open();

                ExecuteNonQuery(cmd);

                if (pareID.Value != DBNull.Value)
                {
                    appointmentID = (long)pareID.Value;
                }
                CleanUpConnectionAndCommand(cn, cmd);
            }
        }
        //▼===== #004
        public bool GetPrescriptionByPtRegistrationID(long PtRegistrationID, long DoctorID, out List<PrescriptionDetail> ListPrescriptionDetail)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                bool bOK = false;
                SqlCommand cmd = new SqlCommand("spGetPrescriptionByPtRegistrationID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));

                cmd.AddParameter("@DoctorID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DoctorID));


                cn.Open();
                ListPrescriptionDetail = new List<PrescriptionDetail>();

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    var prescriptionDetail = GetPrescriptionDetailFromReaderBase(reader);
                    if (prescriptionDetail != null)
                    {
                        ListPrescriptionDetail.Add(prescriptionDetail);
                        bOK = true;
                    }
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return bOK;
            }
        }

        public bool GetPatientPCLReqByPtRegistrationID(long PtRegistrationID, out List<PatientPCLRequestDetail> ListPatientPCLReqDetail)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                bool bOK = false;
                SqlCommand cmd = new SqlCommand("spGetPatientPCLReqDetailByPtRegistrationID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));

                cn.Open();
                ListPatientPCLReqDetail = new List<PatientPCLRequestDetail>();

                IDataReader reader = ExecuteReader(cmd);

                ListPatientPCLReqDetail = GetPatientPCLRequestDetailsCollectionFromReader(reader);
                if (ListPatientPCLReqDetail != null)
                {
                    bOK = true;
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return bOK;
            }
        }
        //▲===== #004
        public bool GetListPatientRegistrationDetailForCheckMedicalFile(long PtRegistrationID, out List<PatientRegistrationDetail> ListPatientRegistrationDetail)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                bool bOK = false;
                SqlCommand cmd = new SqlCommand("spGetListPatientRegistrationDetailForCheckMedicalFile_KHTH", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cn.Open();
                ListPatientRegistrationDetail = new List<PatientRegistrationDetail>();
                IDataReader reader = ExecuteReader(cmd);
                ListPatientRegistrationDetail = GetPatientRegistrationDetailsCollectionFromReader(reader);
                if (ListPatientRegistrationDetail != null)
                {
                    bOK = true;
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return bOK;
            }
        }

        public bool GetPatientPCLReqByInPtRegistrationID(long PtRegistrationID, out List<PatientPCLRequestDetail> ListPatientPCLReqDetail)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                bool bOK = false;
                SqlCommand cmd = new SqlCommand("spGetPatientPCLReqDetailByInPtRegistrationID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));

                cn.Open();
                ListPatientPCLReqDetail = new List<PatientPCLRequestDetail>();

                IDataReader reader = ExecuteReader(cmd);

                ListPatientPCLReqDetail = GetPatientPCLRequestDetailsCollectionFromReader(reader);
                if (ListPatientPCLReqDetail != null)
                {
                    bOK = true;
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return bOK;
            }
        }
        public IList<Prescription> SearchOutPtTreatmentPrescription(PrescriptionSearchCriteria Criteria, DateTime SearchTime, int pageIndex, int pageSize, out int totalCount)
        {
            totalCount = 0;
            if (Criteria == null)
            {
                return null;
            }

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spOutPtTreatmentPrescription_Seach_ByPtRegistrationID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(pageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(pageIndex));
                cmd.AddParameter("@SearchTime", SqlDbType.DateTime, ConvertNullObjectToDBNull(SearchTime));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, Criteria.PtRegistrationID);
                cmd.AddParameter("@Total", SqlDbType.Bit, DBNull.Value, ParameterDirection.Output);
                cn.Open();
                List<Prescription> prescriptions = null;

                IDataReader reader = ExecuteReader(cmd);
                if (cmd.Parameters["@Total"].Value != null)
                {
                    totalCount = Convert.ToInt32(cmd.Parameters["@Total"].Value);
                }
                else
                {
                    totalCount = -1;
                }
                prescriptions = GetPtPrescriptionCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return prescriptions;
            }
        }
        public decimal GetTotalHIPaymentForRegistration(long PtRegistrationID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetTotalHIPaymentForRegistration", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cn.Open();
                IDataReader retVal = ExecuteReader(cmd);
                decimal total = 0;
                if (retVal.Read())
                {
                    if (retVal.HasColumn("TotalHIPayment") && retVal["TotalHIPayment"] != DBNull.Value)
                    {
                        total = (decimal)retVal["TotalHIPayment"];
                    }
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return total;
            }
        }
        public bool CheckStatusPCLRequestBeforeCreateNew(long PtRegistrationID, bool IsGCT)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spCheckStatusPCLRequestBeforeCreateNew", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@IsGCT", SqlDbType.Bit, ConvertNullObjectToDBNull(IsGCT));
                cn.Open();
                IDataReader retVal = ExecuteReader(cmd);
                bool result = false;
                if (retVal.Read())
                {
                    result = true;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return result;
            }
        }
        public List<Prescription> Prescriptions_BaoHiemConThuoc_ByPatientID(Int64 PatientID, long PtRegDetailID, bool CungChuyenKhoa)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPrescriptions_BaoHiemConThuoc_ByPatientID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                cmd.AddParameter("@PtRegDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegDetailID));
                cmd.AddParameter("@CungChuyenKhoa", SqlDbType.Bit, ConvertNullObjectToDBNull(CungChuyenKhoa));

                cn.Open();
                List<Prescription> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPtPrescriptionCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }
        public int? GetRemainingMedicineDays(long OutPtTreatmentProgramID, long PtRegDetailID, out int MinNumOfDayMedicine, out int MaxNumOfDayMedicine)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetRemainingMedicineDays", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@OutPtTreatmentProgramID", SqlDbType.BigInt, ConvertNullObjectToDBNull(OutPtTreatmentProgramID));
                cmd.AddParameter("@PtRegDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegDetailID));
                cmd.AddParameter("@MinNumOfDayMedicine", SqlDbType.Int, 16, ParameterDirection.Output);
                cmd.AddParameter("@MaxNumOfDayMedicine", SqlDbType.Int, 16, ParameterDirection.Output);
                cn.Open();

                int? RemainingMedicineDays = null;
                IDataReader reader = ExecuteReader(cmd);
                if (reader != null && reader.Read() && reader.HasColumn("RemainingMedicineDays") && reader["RemainingMedicineDays"] != DBNull.Value)
                {
                    RemainingMedicineDays = (int)reader["RemainingMedicineDays"];
                }
                reader.Close();
                if (cmd.Parameters["@MinNumOfDayMedicine"].Value != DBNull.Value)
                {
                    MinNumOfDayMedicine = Convert.ToInt32(cmd.Parameters["@MinNumOfDayMedicine"].Value);
                }
                else
                {
                    MinNumOfDayMedicine = -1;
                }
                if (cmd.Parameters["@MaxNumOfDayMedicine"].Value != DBNull.Value)
                {
                    MaxNumOfDayMedicine = Convert.ToInt32(cmd.Parameters["@MaxNumOfDayMedicine"].Value);
                }
                else
                {
                    MaxNumOfDayMedicine = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return RemainingMedicineDays;
            }
        }
    }
}

