using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using DataEntities;
using DataEntities.CustomDTOs;
using eHCMS.Configurations;
using eHCMS.Services.Core;
using Service.Core.Common;
using System.Text.RegularExpressions;
using AxLogging;
using eHCMSLanguage;
/*
 * 20170218 #001 CMN: Add Checkbox AllDept for InPtBills
 * 20170308 #002 CMN: Add HIStore Service
 * 20170517 #003 CMN: Thêm giá trị để cập nhật tên bệnh nhân không giới hạn với tài khoản quản trị
 * 20170522 #004 CMN: Added variable to check InPt 5 year HI without paid enough
 * 20170927 #005 CMN: Added DeadReason
 * 20171026 #006 CMN: Added Find ConsultingDiagnosys for Surgery Registrations
 * 20171107 #007 CMN: Added Added IsConfirmEmergencyTreatment into AdmissionInfo
 * 20180102 #008 CMN: Added properties for 4210 file
 * 20180508 #009 TxD: Commented out all BeginTransaction in this class because it could have caused dead lock in DB (suspection only at this stage)
 * 20180523 #010 TBLD: Added @CreatedDate
 * 20180814 #011 TTM: Tạo mới method để thực hiện thêm mới và cập nhật bệnh nhân, thẻ BHYT, giấy CV 1 lần.
 * 20181113 #012 TTM: BM 0005228: Lưu và cập nhật phường xã trong Patients và HealthInsurance.
 * 20181119 #013 TTM: BM 0005257: Tạo hàm lấy dữ liệu bệnh nhân đang nằm tại khoa.
 * 20181122 #014 TTM: BM 0005299: Cho phép tìm kiếm bệnh nhân bằng tên đi kèm với DOB
 * 20181211 #015 TTM: BM 0004207: Thêm hàm lấy danh sách định dạng thẻ BHYT động.
 * 20181212 #016 TTM: Lưu chẩn đoán ban đầu BasicDiagTreatment và tạo hàm cập nhật.
*/
namespace eHCMS.DAL
{
    public class SqlPatientProvider : PatientProvider
    {
        public SqlPatientProvider()
            : base()
        {

        }
        public override DbConnection CreateConnection()
        {
            return new SqlConnection(this.ConnectionString);
        }

        public override List<Patient> SearchPatients(PatientSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            totalCount = 0;
            if (criteria == null)
            {
                return null;
            }

            using (var cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("proc_SearchPatients", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                var paramFullName = new SqlParameter("@FullName", SqlDbType.NVarChar) { Value = ConvertNullObjectToDBNull(criteria.FullName) };
                var paramFileCodeNumber = new SqlParameter("@FileCodeNumber", SqlDbType.VarChar) { Value = DBNull.Value };

                //KMx: Nếu 3 ký tự đầu của Full Name = "hs:" và sau dấu ":" còn ký tự khác nữa thì chữ "hs:" là do chương trình tạo ra.
                //Nếu sau dấu ":" không còn ký tự nào nữa thì chữ "hs:" là do người dùng nhập vào. Và tìm "hs:" theo Full Name (28/05/2014 14:45).
                if (criteria.FullName != null && criteria.FullName.Length >= 4
                    && criteria.FullName.ToLower().Substring(0, 3) == "hs:" && criteria.FullName.Substring(3) != "")
                {
                    paramFullName.Value = DBNull.Value;
                    paramFileCodeNumber.Value = ConvertNullObjectToDBNull(criteria.FullName.Substring(3));
                }

                cmd.Parameters.Add(paramFullName);
                cmd.Parameters.Add(paramFileCodeNumber);

                cmd.AddParameter("PatientCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(criteria.PatientCode));

                cmd.AddParameter("@EntryDateEnabled", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.EntryDateEnabled));
                cmd.AddParameter("@EntryDateBegin", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.EntryDateBegin));
                cmd.AddParameter("@EntryDateEnd", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.EntryDateEnd));

                cmd.AddParameter("@ReleaseDateEnabled", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.ReleaseDateEnabled));
                cmd.AddParameter("@ReleaseDateBegin", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ReleaseDateBegin));
                cmd.AddParameter("@ReleaseDateEnd", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ReleaseDateEnd));

                cmd.AddParameter("@BirthDateEnabled", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.BirthDateEnabled));
                cmd.AddParameter("@BirthDateBegin", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.BirthDateBegin));
                cmd.AddParameter("@BirthDateEnd", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.BirthDateEnd));

                cmd.AddParameter("@GenderEnabled", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.GenderEnabled));

                if (criteria.Gender != null)
                {
                    cmd.AddParameter("@Gender", SqlDbType.Char, ConvertNullObjectToDBNull(criteria.Gender.ID));
                }
                else
                {
                    cmd.AddParameter("@Gender", SqlDbType.Char, DBNull.Value);
                }

                cmd.AddParameter("@HealthInsuranceCard", SqlDbType.VarChar, ConvertNullObjectToDBNull(criteria.InsuranceCard));
                cmd.AddParameter("@IncludeInactivePatients", SqlDbType.Bit, false);

                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(pageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(pageIndex));

                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.OrderBy));

                cmd.AddParameter("@CountTotal", SqlDbType.Bit, ConvertNullObjectToDBNull(bCountTotal));
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);
                cmd.AddParameter("@IsShowHICardNo", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.IsShowHICardNo));
                cmd.AddParameter("@PtRegistrationCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(criteria.PtRegistrationCode));
                //▼====== #014
                cmd.AddParameter("@DOBNumIndex", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.DOBNumIndex));
                //▲====== #014
                if (!string.IsNullOrEmpty(criteria.PatientNameString) && criteria.PatientNameString.Length > 3 && new string[] { "PH", "PL", "QH", "QL" }.Contains(criteria.PatientNameString.Substring(0, 2))
                    && char.IsDigit(criteria.PatientNameString[3]))
                {
                    cmd.AddParameter("@PCLRequestNumID", SqlDbType.VarChar, ConvertNullObjectToDBNull(criteria.PatientNameString));
                }

                cn.Open();

                var reader = ExecuteReader(cmd);

                var patients = GetPatientCollectionFromReader(reader);
                reader.Close();

                if (bCountTotal && cmd.Parameters["@Total"].Value != DBNull.Value)
                {
                    totalCount = (int)cmd.Parameters["@Total"].Value;
                }
                else
                {
                    totalCount = -1;
                }

                return patients;
            }

        }


        public override bool DeletePatientByID(long patientID)
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                var cmd = new SqlCommand("spRemovePatientByID", cn) { CommandType = CommandType.StoredProcedure };

                cmd.AddParameter("@PatientID", SqlDbType.BigInt, patientID);

                cn.Open();

                var retVal = ExecuteNonQuery(cmd);
                return retVal > 0;
            }
        }
        public override List<Patient> GetAllPatients()
        {
            throw new NotImplementedException();
        }

        public override Patient GetPatientByID_Simple(long patientID)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                return GetPatientByID_Simple(patientID, conn, null);
            }
        }

        public override Patient GetPatientByID_Simple(long patientID, DbConnection connection, DbTransaction tran)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            var cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spGetPatientByID_Simple";

            cmd.AddParameter("@PatientID", SqlDbType.BigInt, patientID);

            Patient p = null;

            var reader = ExecuteReader(cmd);

            if (reader != null)
            {
                if (reader.Read())
                {
                    p = GetPatientFromReader(reader);
                }
                reader.Close();
            }

            return p;
        }

        public override Patient GetPatientByID(long patientID, bool ToRegisOutPt = false)
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                var cmd = new SqlCommand("spGetPatientByID", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, patientID);
                cmd.AddParameter("@ToRegisOutPt", SqlDbType.Bit, ToRegisOutPt);

                cn.Open();
                Patient p = null;

                var reader = ExecuteReader(cmd);

                if (reader != null && reader.Read())
                {
                    //Lay thong tin general info.
                    p = GetPatientFromReader(reader);
                    if (!reader.NextResult())
                    {
                        return p;
                    }
                    if (reader.Read())
                    {
                        p.LatestRegistration = GetPatientRegistrationFromReader(reader);
                    }


                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            p.latestHIRegistration = GetPatientRegistrationFromReader(reader);
                        }
                    }

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            p.LatestRegistration_InPt = GetPatientRegistrationFromReader(reader);
                        }
                    }

                    if (reader.NextResult())
                    {
                        p.AppointmentList = GetAppointmentCollectionFromReader(reader);
                    }
                }
                return p;
            }
        }

        public override Patient GetPatientByID_InPt(long patientID, bool bGetOutPtRegInfo = false, bool ToRegisOutPt = false)
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                var cmd = new SqlCommand("spGetPatientByID_InPt", cn) { CommandType = CommandType.StoredProcedure };

                cmd.AddParameter("@PatientID", SqlDbType.BigInt, patientID);
                cmd.AddParameter("@ToRegisOutPt", SqlDbType.Bit, ToRegisOutPt);
                cmd.AddParameter("@GetLatestOutPtRegInfo", SqlDbType.Bit, (bGetOutPtRegInfo ? 1 : 0));

                cn.Open();
                Patient p = null;

                var reader = ExecuteReader(cmd);

                if (reader != null && reader.Read())
                {
                    //Lay thong tin general info.
                    p = GetPatientFromReader(reader);
                    if (!reader.NextResult())
                    {
                        return p;
                    }

                    if (bGetOutPtRegInfo)   // TxD 20/12/2014: Get OutPt Registration as well
                    {
                        if (reader.Read())
                        {
                            p.LatestRegistration = GetPatientRegistrationFromReader(reader);    // NON HI
                        }

                        if (reader.NextResult())
                        {
                            if (reader.Read())
                            {
                                p.latestHIRegistration = GetPatientRegistrationFromReader(reader); // HI Regis
                            }
                        }

                        reader.NextResult();

                    }

                    if (reader.Read())
                    {
                        p.LatestRegistration_InPt = GetPatientRegistrationFromReader(reader);
                    }

                }
                return p;
            }
        }

        public override List<Patient> GetPatientAll()
        {
            var lstPatient = new List<Patient>();
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spPatientsGetAll", cn) { CommandType = CommandType.StoredProcedure };

                cn.Open();

                var reader = ExecuteReader(cmd);

                if (reader != null && reader.Read())
                {
                    lstPatient = GetPatientCollectionFromReader(reader);
                }
                return lstPatient;
            }
        }

        public override Patient GetPatientByID_Full(long patientID, bool ToRegisOutPt = false)
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                var cmd = new SqlCommand("spGetPatientByID", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, patientID);

                cn.Open();
                Patient p = null;

                var reader = ExecuteReader(cmd);

                if (reader != null && reader.Read())
                {
                    p = GetPatientFromReader(reader);

                    reader.Close();

                    cmd.CommandText = "spGetLatestRegistration";
                    reader = ExecuteReader(cmd);
                    if (reader != null)
                    {
                        if (reader.Read())
                        {
                            p.LatestRegistration = GetPatientRegistrationFromReader(reader);
                        }
                        reader.Close();
                    }
                    var cmdReg = new SqlCommand("spGetLatestRegistration_InPt", cn) { CommandType = CommandType.StoredProcedure };
                    //cmd.CommandText = "spGetLatestRegistration_InPt";
                    cmdReg.AddParameter("@PatientID", SqlDbType.BigInt, patientID);
                    cmdReg.AddParameter("@ToRegisOutPt", SqlDbType.Bit, ConvertNullObjectToDBNull(ToRegisOutPt));
                    reader = ExecuteReader(cmdReg);
                    if (reader != null)
                    {
                        if (reader.Read())
                        {
                            p.LatestRegistration_InPt = GetPatientRegistrationFromReader(reader);
                        }
                        reader.Close();
                    }

                    cmd.CommandText = "spGetAllHealthInsurances";
                    reader = ExecuteReader(cmd);
                    if (reader != null)
                    {
                        var allHIItems = GetHealthInsuranceCollectionFromReader(reader);
                        if (allHIItems != null)
                        {
                            p.HealthInsurances = allHIItems.ToObservableCollection<HealthInsurance>();

                        }
                        reader.Close();
                    }

                    if (p.HealthInsurances != null && p.HealthInsurances.Count > 0)
                    {
                        foreach(var hiItem in p.HealthInsurances)
                        {
                            if (hiItem.IsActive)
                            {
                                p.CurrentHealthInsurance = hiItem;
                                break;
                            }
                        }
                    }
                    else
                    {
                        p.CurrentHealthInsurance = GetActiveHICard(patientID);
                    }

                    List<PaperReferal> allReferals = null;
                    if (p.CurrentHealthInsurance != null)
                    {
                        allReferals = GetAllPaperReferalsByHealthInsurance(p.CurrentHealthInsurance.HIID);
                    }

                    p.PaperReferals = allReferals == null ? null : allReferals.ToObservableCollection();

                }
                return p;
            }
        }
        public override List<Patient> GetPatients(int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            var criteria = new PatientSearchCriteria();
            return SearchPatients(criteria, pageIndex, pageSize, bCountTotal, out totalCount);
        }

        public override List<Location> GetAllLocations(long? departmentID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetAllLocations", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@DepartmentID", SqlDbType.BigInt, ConvertNullObjectToDBNull(departmentID));

                cn.Open();

                var reader = ExecuteReader(cmd);

                var retVal = GetLocationCollectionFromReader(reader);
                return retVal;
            }
        }
        public override List<InPatientBillingInvoice> GetAllInPatientBillingInvoices(long registrationID, long? DeptID)
        {
            //==== #001
            //using (var cn = new SqlConnection(ConnectionString))
            //{
            //    var cmd = new SqlCommand("sp_GetAllInPatientBillingInvoices", cn) { CommandType = CommandType.StoredProcedure };
            //    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, registrationID);
            //    cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptID));
            //    cn.Open();
            //    List<InPatientBillingInvoice> retVal;

            //    var reader = ExecuteReader(cmd);

            //    retVal = GetInPatientBillingInvoiceCollectionFromReader(reader);
            //    return retVal;
            //}
            return fcGetAllInPatientBillingInvoices(registrationID, DeptID);
            //==== #001
        }
        public override PromoDiscountProgram GetPromoDiscountProgramByRegID(long PtRegistrationID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetPromoDiscountProgramByRegID", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cn.Open();
                var reader = ExecuteReader(cmd);
                PromoDiscountProgram mPromoDiscountProgram = null;
                while (reader.Read())
                {
                    mPromoDiscountProgram = GetPromoDiscountProgramFromReader(reader);
                }
                reader.Close();
                return mPromoDiscountProgram;
            }
        }

        public override PromoDiscountProgram GetPromoDiscountProgramByPromoDiscProgID(long PromoDiscProgID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetPromoDiscountProgramByPromoDiscProgID", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@PromoDiscProgID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PromoDiscProgID));
                cn.Open();
                var reader = ExecuteReader(cmd);
                PromoDiscountProgram mPromoDiscountProgram = null;
                while (reader.Read())
                {
                    mPromoDiscountProgram = GetPromoDiscountProgramFromReader(reader);
                }
                reader.Close();
                return mPromoDiscountProgram;
            }
        }
        //==== #001
        private List<InPatientBillingInvoice> fcGetAllInPatientBillingInvoices(long PtRegistrationID, long? DeptID, List<RefDepartment> DeptArray = null)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("sp_GetAllInPatientBillingInvoices", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, PtRegistrationID);
                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptID));
                if (DeptArray != null)
                {
                    cmd.AddParameter("@DeptArray", SqlDbType.Xml, ConvertNullObjectToDBNull(GenerateListToXML(DeptArray).ToString()));
                }
                cn.Open();
                List<InPatientBillingInvoice> retVal;
                var reader = ExecuteReader(cmd);
                retVal = GetInPatientBillingInvoiceCollectionFromReader(reader);
                return retVal;
            }
        }
        public override List<InPatientBillingInvoice> GetAllInPatientBillingInvoicesByDeptArray(long PtRegistrationID, long? DeptID, List<RefDepartment> DeptArray)
        {
            return fcGetAllInPatientBillingInvoices(PtRegistrationID, DeptID, DeptArray);
        }
        //==== #001
        public override List<InPatientBillingInvoice> GetAllInPatientBillingInvoices_FromListDeptID(long registrationID, List<long> ListDeptIDs)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("sp_GetAllInPatientBillingInvoices_FromDeptIDList", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, registrationID);

                XDocument xmlDocument;
                string billingInvoiceIDsString = null;
                if (ListDeptIDs != null && ListDeptIDs.Count > 0)
                {
                    xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                              new XElement("InPtBillingInvListIDs",
                              from item in ListDeptIDs
                              select new XElement("IDs", new XElement("ID", item))));

                    billingInvoiceIDsString = xmlDocument.ToString();
                }

                cmd.AddParameter("@DeptIDList", SqlDbType.Xml, billingInvoiceIDsString);

                cn.Open();
                List<InPatientBillingInvoice> retVal;

                var reader = ExecuteReader(cmd);

                retVal = GetInPatientBillingInvoiceCollectionFromReader(reader);
                return retVal;
            }
        }


        public override List<InPatientBillingInvoice> GetAllInPatientBillingInvoices_ForCreateForm02(long RptForm02_InPtID, long PtRegistrationID, List<long> ListDeptIDs)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("sp_GetAllInPatientBillingInvoices_ForCreateForm02", cn) { CommandType = CommandType.StoredProcedure };

                cmd.AddParameter("@RptForm02_InPtID", SqlDbType.BigInt, RptForm02_InPtID);
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, PtRegistrationID);

                XDocument xmlDocument;
                string billingInvoiceIDsString = null;
                if (ListDeptIDs != null && ListDeptIDs.Count > 0)
                {
                    xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                              new XElement("DeptIDList",
                              from item in ListDeptIDs
                              select new XElement("IDs", new XElement("ID", item))));

                    billingInvoiceIDsString = xmlDocument.ToString();
                }

                cmd.AddParameter("@DeptIDList", SqlDbType.Xml, ConvertNullObjectToDBNull(billingInvoiceIDsString));

                cn.Open();
                List<InPatientBillingInvoice> retVal;

                var reader = ExecuteReader(cmd);

                retVal = GetInPatientBillingInvoiceCollectionFromReader(reader);
                return retVal;
            }
        }


        //Ny them 

        public override bool GetBalanceInPatientBillingInvoice(long StaffID, long PtRegistrationID, long V_RegistrationType, string BillingInvoices, DbConnection connection, DbTransaction tran)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            var cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spBalanceInPatientBillingInvoice";
            cmd.AddParameter("@BillInvoices", SqlDbType.Xml, BillingInvoices);
            cmd.AddParameter("@StaffID", SqlDbType.BigInt, StaffID);
            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, PtRegistrationID);
            cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, V_RegistrationType);
            return ExecuteNonQuery(cmd) > 0;
        }

        public override InPatientBillingInvoice GetInPatientBillingInvoiceByIdWithoutDetails(long InPatientBillingInvID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                return GetInPatientBillingInvoiceByIdWithoutDetails(InPatientBillingInvID, cn, null);
            }
        }
        public override InPatientBillingInvoice GetInPatientBillingInvoiceByIdWithoutDetails(long InPatientBillingInvID, DbConnection connection, DbTransaction tran)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            var cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "sp_GetInPatientBillingInvoiceByIdWithoutDetails";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@InPatientBillingInvID", SqlDbType.BigInt, InPatientBillingInvID);

            InPatientBillingInvoice retVal = null;

            var reader = ExecuteReader(cmd);
            if (reader != null)
            {
                if (reader.Read())
                {
                    retVal = GetInPatientBillingInvoiceFromReader(reader);
                }
                reader.Close();
            }
            return retVal;
        }
        public override List<DeptLocation> GetLocationsByServiceID(long medServiceID)
        {
            if (medServiceID <= 0)
            {
                throw new Exception(eHCMSResources.Z1680_G1_PleaseSelectAService);
            }
            if (MAPServiceIdAndDeptLocIDs.ContainsKey(medServiceID))
            {
                List<DeptLocation> listDeptLoc = new List<DeptLocation>();
                listDeptLoc = MAPServiceIdAndDeptLocIDs[medServiceID];
                return listDeptLoc;
            }
            return null;
        }

        public override List<DeptLocation> GetAllDeptLocForServicesByInOutType(List<long> RefMedicalServiceInOutOthersTypes)
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                var cmd = new SqlCommand("spGetAllServices_DeptLocations_Txd", cn) { CommandType = CommandType.StoredProcedure };
                if (RefMedicalServiceInOutOthersTypes != null && RefMedicalServiceInOutOthersTypes.Count > 0)
                {
                    var idList = new IDListOutput<long>() { Ids = RefMedicalServiceInOutOthersTypes };
                    cmd.AddParameter("@TypeIDList", SqlDbType.Xml, ConvertNullObjectToDBNull(SerializeToXml(idList)));
                }

                cn.Open();

                var reader = ExecuteReader(cmd);

                var retVal = GetDeptLocationCollectionFromReader(reader);
                return retVal;
            }
        }

        public override List<RefMedicalServiceType> GetAllMedicalServiceTypes()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetAllMedicalServiceTypes", cn) { CommandType = CommandType.StoredProcedure };

                cn.Open();

                var reader = ExecuteReader(cmd);

                var retVal = GetMedicalServiceTypeCollectionFromReader(reader);
                return retVal;
            }
        }
        public override List<RefMedicalServiceType> GetMedicalServiceTypesByInOutType(List<long> RefMedicalServiceInOutOthersTypes)
        {
            if (RefMedicalServiceInOutOthersTypes == null || RefMedicalServiceInOutOthersTypes.Count == 0)
            {
                return null;
            }
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                var cmd = new SqlCommand("spGetMedicalServiceTypesByInOutType", cn) { CommandType = CommandType.StoredProcedure };

                var idList = new IDListOutput<long>() { Ids = RefMedicalServiceInOutOthersTypes };
                cmd.AddParameter("@TypeIDList", SqlDbType.Xml, ConvertNullObjectToDBNull(SerializeToXml(idList)));

                cn.Open();

                var reader = ExecuteReader(cmd);

                var retVal = GetMedicalServiceTypeCollectionFromReader(reader);
                return retVal;
            }
        }
        public override List<PatientClassification> GetAllClassifications()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetAllClassifications", cn) { CommandType = CommandType.StoredProcedure };
                cn.Open();

                var reader = ExecuteReader(cmd);

                var retVal = GetPatientClassificationCollectionFromReader(reader);
                return retVal;
            }
        }

        public override List<BloodType> GetAllBloodTypes()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spBloodTypesGetAll", cn) { CommandType = CommandType.StoredProcedure };
                cn.Open();
                var retVal = new List<BloodType>();

                var reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    var p = new BloodType
                    {
                        BloodTypeID = (byte)reader["BloodTypeID"],
                        BloodTypeName = reader["BloodTypeName"].ToString(),
                        RhType = reader["RhType"].ToString()
                    };
                    p.Descript = p.BloodTypeName + " " + p.RhType;
                    retVal.Add(p);
                }

                return retVal;
            }
        }
        public override bool UpdatePatientBloodTypeID(long PatientID, int? BloodTypeID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spUpdatePatientBloodTypeID", cn) { CommandType = CommandType.StoredProcedure };

                cmd.AddParameter("@BloodTypeID", SqlDbType.SmallInt, ConvertNullObjectToDBNull(BloodTypeID));
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, PatientID);

                cn.Open();

                var retVal = ExecuteNonQuery(cmd);
                return retVal > 0;
            }
        }
        /*==== #003 ====*/
        //public override bool UpdatePatient(Patient patient)
        public override bool UpdatePatient(Patient patient, bool IsAdmin = false)
        /*==== #003 ====*/
        {
            patient.ExtractFullName();
            if (!patient.DateBecamePatient.HasValue || patient.DateBecamePatient.Value == DateTime.MinValue)
            {
                patient.DateBecamePatient = DateTime.Now;
            }
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                var cmd = new SqlCommand("spUpdatePatient", cn) { CommandType = CommandType.StoredProcedure };

                cmd.AddParameter("@CountryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(patient.CountryID));
                cmd.AddParameter("@NationalityID", SqlDbType.BigInt, ConvertNullObjectToDBNull(patient.NationalityID));
                cmd.AddParameter("@CityProvinceID", SqlDbType.Int, ConvertNullObjectToDBNull(patient.CityProvinceID));
                cmd.AddParameter("@SuburbNameID", SqlDbType.BigInt, ConvertNullObjectToDBNull(patient.SuburbNameID));
                cmd.AddParameter("@IDNumber", SqlDbType.Char, ConvertNullObjectToDBNull(patient.IDNumber.Trim()));
                cmd.AddParameter("@FirstName", SqlDbType.NVarChar, patient.FirstName);
                cmd.AddParameter("@MiddleName", SqlDbType.NVarChar, patient.MiddleName);
                cmd.AddParameter("@LastName", SqlDbType.NVarChar, patient.LastName);
                if (patient.GenderObj != null)
                {
                    cmd.AddParameter("@Gender", SqlDbType.Char, patient.GenderObj.ID);
                }
                else
                {
                    cmd.AddParameter("@Gender", SqlDbType.Char, DBNull.Value);
                }
                patient.CalDOB();

                cmd.AddParameter("@DOB", SqlDbType.DateTime, ConvertNullObjectToDBNull(patient.DOB));
                cmd.AddParameter("@AgeOnly", SqlDbType.Bit, ConvertNullObjectToDBNull(patient.AgeOnly));
                cmd.AddParameter("@DateBecamePatient", SqlDbType.DateTime, ConvertNullObjectToDBNull(patient.DateBecamePatient));
                cmd.AddParameter("@V_MaritalStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(patient.V_MaritalStatus));
                cmd.AddParameter("@PatientPhoto", SqlDbType.NVarChar, ConvertNullObjectToDBNull(patient.PatientPhoto));
                cmd.AddParameter("@PatientNotes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(patient.PatientNotes));
                cmd.AddParameter("@PatientStreetAddress", SqlDbType.NVarChar, ConvertNullObjectToDBNull(patient.PatientStreetAddress));
                cmd.AddParameter("@PatientSurburb", SqlDbType.NVarChar, ConvertNullObjectToDBNull(patient.PatientSurburb));
                cmd.AddParameter("@PatientPhoneNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(patient.PatientPhoneNumber));
                cmd.AddParameter("@PatientCellPhoneNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(patient.PatientCellPhoneNumber));
                cmd.AddParameter("@PatientEmailAddress", SqlDbType.NVarChar, ConvertNullObjectToDBNull(patient.PatientEmailAddress));
                cmd.AddParameter("@PatientEmployer", SqlDbType.NVarChar, ConvertNullObjectToDBNull(patient.PatientEmployer));
                cmd.AddParameter("@PatientOccupation", SqlDbType.NVarChar, ConvertNullObjectToDBNull(patient.PatientOccupation));
                cmd.AddParameter("@V_Ethnic", SqlDbType.BigInt, ConvertNullObjectToDBNull(patient.V_Ethnic));
                cmd.AddParameter("@FContactFullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(patient.FContactFullName));
                cmd.AddParameter("@V_FamilyRelationship", SqlDbType.BigInt, ConvertNullObjectToDBNull(patient.V_FamilyRelationship));
                cmd.AddParameter("@FContactAddress", SqlDbType.NVarChar, ConvertNullObjectToDBNull(patient.FContactAddress));
                cmd.AddParameter("@FContactHomePhone", SqlDbType.VarChar, ConvertNullObjectToDBNull(patient.FContactHomePhone));
                cmd.AddParameter("@FContactBusinessPhone", SqlDbType.VarChar, ConvertNullObjectToDBNull(patient.FContactBusinessPhone));
                cmd.AddParameter("@FContactCellPhone", SqlDbType.VarChar, ConvertNullObjectToDBNull(patient.FContactCellPhone));
                cmd.AddParameter("@FAlternateContact", SqlDbType.NVarChar, ConvertNullObjectToDBNull(patient.FAlternateContact));
                cmd.AddParameter("@FAlternatePhone", SqlDbType.NVarChar, ConvertNullObjectToDBNull(patient.FAlternatePhone));
                cmd.AddParameter("@FullName", SqlDbType.NVarChar, patient.FullName);
                cmd.AddParameter("@BloodTypeID", SqlDbType.SmallInt, ConvertNullObjectToDBNull(patient.BloodTypeID));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(patient.StaffID));

                cmd.AddParameter("@PatientID", SqlDbType.BigInt, patient.PatientID);
                /*==== #003 ====*/
                cmd.AddParameter("@IsAdmin", SqlDbType.Bit, IsAdmin);
                /*==== #003 ====*/

                //▼====== #012
                cmd.AddParameter("@WardNameID", SqlDbType.BigInt, ConvertNullObjectToDBNull(patient.WardName.WardNameID));
                cmd.AddParameter("@WardName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(patient.WardName.WardName));
                //▲====== #012

                cn.Open();

                var retVal = ExecuteNonQuery(cmd);
                return retVal > 0;
            }
        }
        public override bool AddPatient(Patient newPatient, out long PatientID, out string PatientCode, out string PatientBarCode)
        {
            newPatient.ExtractFullName();
            if (!newPatient.DateBecamePatient.HasValue || newPatient.DateBecamePatient.Value == DateTime.MinValue)
            {
                newPatient.DateBecamePatient = DateTime.Now;
            }

            using (var cn = new SqlConnection(this.ConnectionString))
            {
                var cmd = new SqlCommand("spInsertPatient", cn) { CommandType = CommandType.StoredProcedure };

                cmd.AddParameter("@CountryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(newPatient.CountryID));
                cmd.AddParameter("@NationalityID", SqlDbType.BigInt, ConvertNullObjectToDBNull(newPatient.NationalityID));
                cmd.AddParameter("@CityProvinceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(newPatient.CityProvinceID));
                cmd.AddParameter("@SuburbNameID", SqlDbType.Int, ConvertNullObjectToDBNull(newPatient.SuburbNameID));
                cmd.AddParameter("@IDNumber", SqlDbType.Char, ConvertNullObjectToDBNull(newPatient.IDNumber));
                cmd.AddParameter("@FirstName", SqlDbType.NVarChar, newPatient.FirstName.ToUpper());
                cmd.AddParameter("@MiddleName", SqlDbType.NVarChar, newPatient.MiddleName.ToUpper());
                cmd.AddParameter("@LastName", SqlDbType.NVarChar, newPatient.LastName.ToUpper());
                if (newPatient.GenderObj != null)
                {
                    cmd.AddParameter("@Gender", SqlDbType.Char, newPatient.GenderObj.ID);
                }
                else
                {
                    cmd.AddParameter("@Gender", SqlDbType.Char, DBNull.Value);
                }
                //newPatient.CalDOB();

                cmd.AddParameter("@DOB", SqlDbType.DateTime, ConvertNullObjectToDBNull(newPatient.DOB));
                cmd.AddParameter("@AgeOnly", SqlDbType.Bit, ConvertNullObjectToDBNull(newPatient.AgeOnly));
                cmd.AddParameter("@DateBecamePatient", SqlDbType.DateTime, ConvertNullObjectToDBNull(newPatient.DateBecamePatient));
                cmd.AddParameter("@V_MaritalStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(newPatient.V_MaritalStatus));
                cmd.AddParameter("@PatientPhoto", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.PatientPhoto));
                cmd.AddParameter("@PatientNotes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.PatientNotes));
                cmd.AddParameter("@PatientStreetAddress", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.PatientStreetAddress.ToUpper()));
                cmd.AddParameter("@PatientSurburb", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.PatientSurburb));
                cmd.AddParameter("@PatientPhoneNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.PatientPhoneNumber));
                cmd.AddParameter("@PatientCellPhoneNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.PatientCellPhoneNumber));
                cmd.AddParameter("@PatientEmailAddress", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.PatientEmailAddress));
                cmd.AddParameter("@PatientEmployer", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.PatientEmployer));
                cmd.AddParameter("@PatientOccupation", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.PatientOccupation));
                cmd.AddParameter("@V_Ethnic", SqlDbType.BigInt, ConvertNullObjectToDBNull(newPatient.V_Ethnic));
                cmd.AddParameter("@FContactFullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.FContactFullName));
                cmd.AddParameter("@V_FamilyRelationship", SqlDbType.BigInt, ConvertNullObjectToDBNull(newPatient.V_FamilyRelationship));
                cmd.AddParameter("@FContactAddress", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.FContactAddress.ToUpper()));
                cmd.AddParameter("@FContactHomePhone", SqlDbType.VarChar, ConvertNullObjectToDBNull(newPatient.FContactHomePhone));
                cmd.AddParameter("@FContactBusinessPhone", SqlDbType.VarChar, ConvertNullObjectToDBNull(newPatient.FContactBusinessPhone));
                cmd.AddParameter("@FContactCellPhone", SqlDbType.VarChar, ConvertNullObjectToDBNull(newPatient.FContactCellPhone));
                cmd.AddParameter("@FAlternateContact", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.FAlternateContact));
                cmd.AddParameter("@FAlternatePhone", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.FAlternatePhone));
                cmd.AddParameter("@FullName", SqlDbType.NVarChar, newPatient.FullName.ToUpper());
                cmd.AddParameter("@BloodTypeID", SqlDbType.SmallInt, ConvertNullObjectToDBNull(newPatient.BloodTypeID));
                cmd.AddParameter("@PatientCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(newPatient.PatientCode));
                cmd.AddParameter("@StaffID", SqlDbType.VarChar, ConvertNullObjectToDBNull(newPatient.StaffID));

                //▼====== #012
                cmd.AddParameter("@WardNameID", SqlDbType.BigInt, ConvertNullObjectToDBNull(newPatient.WardName.WardNameID));
                cmd.AddParameter("@WardName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.WardName.WardName));
                //▲====== #012

                cmd.AddParameter("@PatientID", SqlDbType.BigInt, 16, ParameterDirection.Output);
                cmd.AddParameter("@PatientCodeOutPut", SqlDbType.VarChar, 16, ParameterDirection.Output);
                cmd.AddParameter("@PatientBarcode", SqlDbType.Char, 15, ParameterDirection.Output);

                cn.Open();

                var retVal = ExecuteNonQuery(cmd);

                PatientID = (long)cmd.Parameters["@PatientID"].Value;
                PatientCode = cmd.Parameters["@PatientCodeOutPut"].Value.ToString();
                PatientBarCode = cmd.Parameters["@PatientBarcode"].Value.ToString();

                return retVal > 0;
            }
        }

        //▼====== #011
        #region Thông tin hành chính, BHYT, CV
        public override bool AddPatientAndHIDetails(Patient newPatient, HealthInsurance newHICard, PaperReferal newPaperReferal, out long PatientID, out string PatientCode, out string PatientBarCode, out long HIID, out long paperReferalID)
        {
            newPatient.ExtractFullName();
            if (!newPatient.DateBecamePatient.HasValue || newPatient.DateBecamePatient.Value == DateTime.MinValue)
            {
                newPatient.DateBecamePatient = DateTime.Now;
            }

            using (var cn = new SqlConnection(this.ConnectionString))
            {
                var cmd = new SqlCommand("spInsertNewPatientHICardAndPaperReferral_V2", cn) { CommandType = CommandType.StoredProcedure };

                //====== Khu vực lưu Thông tin hành chính bệnh nhân
                cmd.AddParameter("@CountryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(newPatient.CountryID));
                cmd.AddParameter("@NationalityID", SqlDbType.BigInt, ConvertNullObjectToDBNull(newPatient.NationalityID));
                cmd.AddParameter("@CityProvinceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(newPatient.CityProvinceID));
                cmd.AddParameter("@SuburbNameID", SqlDbType.Int, ConvertNullObjectToDBNull(newPatient.SuburbNameID));
                cmd.AddParameter("@IDNumber", SqlDbType.Char, ConvertNullObjectToDBNull(newPatient.IDNumber));
                cmd.AddParameter("@FirstName", SqlDbType.NVarChar, newPatient.FirstName.ToUpper());
                cmd.AddParameter("@MiddleName", SqlDbType.NVarChar, newPatient.MiddleName.ToUpper());
                cmd.AddParameter("@LastName", SqlDbType.NVarChar, newPatient.LastName.ToUpper());
                if (newPatient.GenderObj != null)
                {
                    cmd.AddParameter("@Gender", SqlDbType.Char, newPatient.GenderObj.ID);
                }
                else
                {
                    cmd.AddParameter("@Gender", SqlDbType.Char, DBNull.Value);
                }

                cmd.AddParameter("@DOB", SqlDbType.DateTime, ConvertNullObjectToDBNull(newPatient.DOB));
                cmd.AddParameter("@AgeOnly", SqlDbType.Bit, ConvertNullObjectToDBNull(newPatient.AgeOnly));
                cmd.AddParameter("@DateBecamePatient", SqlDbType.DateTime, ConvertNullObjectToDBNull(newPatient.DateBecamePatient));
                cmd.AddParameter("@V_MaritalStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(newPatient.V_MaritalStatus));
                cmd.AddParameter("@PatientPhoto", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.PatientPhoto));
                cmd.AddParameter("@PatientNotes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.PatientNotes));
                cmd.AddParameter("@PatientStreetAddress", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.PatientStreetAddress.ToUpper()));
                cmd.AddParameter("@PatientSurburb", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.PatientSurburb));
                cmd.AddParameter("@PatientPhoneNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.PatientPhoneNumber));
                cmd.AddParameter("@PatientCellPhoneNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.PatientCellPhoneNumber));
                cmd.AddParameter("@PatientEmailAddress", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.PatientEmailAddress));
                cmd.AddParameter("@PatientEmployer", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.PatientEmployer));
                cmd.AddParameter("@PatientOccupation", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.PatientOccupation));
                cmd.AddParameter("@V_Ethnic", SqlDbType.BigInt, ConvertNullObjectToDBNull(newPatient.V_Ethnic));
                cmd.AddParameter("@FContactFullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.FContactFullName));
                cmd.AddParameter("@V_FamilyRelationship", SqlDbType.BigInt, ConvertNullObjectToDBNull(newPatient.V_FamilyRelationship));
                cmd.AddParameter("@FContactAddress", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.FContactAddress.ToUpper()));
                cmd.AddParameter("@FContactHomePhone", SqlDbType.VarChar, ConvertNullObjectToDBNull(newPatient.FContactHomePhone));
                cmd.AddParameter("@FContactBusinessPhone", SqlDbType.VarChar, ConvertNullObjectToDBNull(newPatient.FContactBusinessPhone));
                cmd.AddParameter("@FContactCellPhone", SqlDbType.VarChar, ConvertNullObjectToDBNull(newPatient.FContactCellPhone));
                cmd.AddParameter("@FAlternateContact", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.FAlternateContact));
                cmd.AddParameter("@FAlternatePhone", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.FAlternatePhone));
                cmd.AddParameter("@FullName", SqlDbType.NVarChar, newPatient.FullName.ToUpper());
                cmd.AddParameter("@BloodTypeID", SqlDbType.SmallInt, ConvertNullObjectToDBNull(newPatient.BloodTypeID));
                cmd.AddParameter("@PatientCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(newPatient.PatientCode));
                cmd.AddParameter("@StaffID", SqlDbType.VarChar, ConvertNullObjectToDBNull(newPatient.StaffID));
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, 16, ParameterDirection.Output);
                cmd.AddParameter("@PatientCodeOutPut", SqlDbType.VarChar, 16, ParameterDirection.Output);
                cmd.AddParameter("@PatientBarcode", SqlDbType.Char, 15, ParameterDirection.Output);

                //====== Khu vực lưu Thông tin hành chính bệnh nhân

                //====== Khu vực lưu thẻ BHYT
                cmd.AddParameter("@AcceptedBenefit", SqlDbType.Float, ConvertNullObjectToDBNull(newHICard.HIPatientBenefit));
                cmd.AddParameter("@IsActive", SqlDbType.Bit, newHICard.IsActive);

                cmd.AddParameter("@HosID", SqlDbType.BigInt, newHICard.HosID);
                cmd.AddParameter("@IBID", SqlDbType.Int, newHICard.IBID);
                if (newHICard.HIPCode != null && newHICard.HIPCode.Length > 0)
                {
                    cmd.AddParameter("@HIPCode", SqlDbType.VarChar, newHICard.HIPCode.Trim());
                }
                else
                {
                    cmd.AddParameter("@HIPCode", SqlDbType.VarChar, newHICard.HIPCode);
                }
                if (newHICard.HICardNo != null && newHICard.HICardNo.Length > 0)
                {
                    cmd.AddParameter("@HICardNo", SqlDbType.VarChar, newHICard.HICardNo.Trim());
                }
                else
                {
                    cmd.AddParameter("@HICardNo", SqlDbType.VarChar, newHICard.HICardNo);
                }
                if (newHICard.RegistrationCode != null && newHICard.RegistrationCode.Length > 0)
                {
                    cmd.AddParameter("@RegistrationCode", SqlDbType.VarChar, newHICard.RegistrationCode.Trim());
                }
                else
                {
                    cmd.AddParameter("@RegistrationCode", SqlDbType.VarChar, newHICard.RegistrationCode);
                }
                if (newHICard.RegistrationLocation != null && newHICard.RegistrationLocation.Length > 0)
                {
                    cmd.AddParameter("@RegistrationLocation", SqlDbType.NVarChar, newHICard.RegistrationLocation.Trim());
                }
                else
                {
                    cmd.AddParameter("@RegistrationLocation", SqlDbType.NVarChar, newHICard.RegistrationLocation);
                }
                if (newHICard.CityProvinceName != null && newHICard.CityProvinceName.Length > 0)
                {
                    cmd.AddParameter("@CityProvinceName", SqlDbType.NVarChar, newHICard.CityProvinceName.Trim());
                }
                else
                {
                    cmd.AddParameter("@CityProvinceName", SqlDbType.NVarChar, newHICard.CityProvinceName);
                }
                cmd.AddParameter("@ValidDateFrom", SqlDbType.DateTime, newHICard.ValidDateFrom);
                cmd.AddParameter("@ValidDateTo", SqlDbType.DateTime, newHICard.ValidDateTo);
                cmd.AddParameter("@V_HICardType", SqlDbType.BigInt, newHICard.V_HICardType);
                cmd.AddParameter("@ArchiveNumber", SqlDbType.NVarChar, newHICard.ArchiveNumber);
                cmd.AddParameter("@CofirmDuplicate", SqlDbType.Bit, newHICard.CofirmDuplicate);
                cmd.AddParameter("@HIID", SqlDbType.BigInt, 16, ParameterDirection.Output);
                if (newHICard.PatientStreetAddress != null && newHICard.PatientStreetAddress.Length > 0)
                {
                    cmd.AddParameter("@PatientStreetAddressHI", SqlDbType.NVarChar, newHICard.PatientStreetAddress.Trim());
                }
                else
                {
                    cmd.AddParameter("@PatientStreetAddressHI", SqlDbType.NVarChar, newHICard.PatientStreetAddress);
                }
                cmd.AddParameter("@SuburbNameIDHI", SqlDbType.BigInt, newHICard.SuburbNameID);
                cmd.AddParameter("@CityProvinceID_Address", SqlDbType.BigInt, newHICard.CityProvinceID_Address);
                cmd.AddParameter("@KVCode", SqlDbType.BigInt, newHICard.KVCode);

                //====== Khu vực lưu thẻ BHYT

                //====== Khu vực lưu thẻ CV


                cmd.AddParameter("@HospitalID", SqlDbType.BigInt, newPaperReferal.HospitalID);
                cmd.AddParameter("@RefCreatedDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(newPaperReferal.RefCreatedDate));
                cmd.AddParameter("@AcceptedDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(newPaperReferal.AcceptedDate));
                cmd.AddParameter("@TreatmentFaculty", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPaperReferal.TreatmentFaculty));
                cmd.AddParameter("@GeneralDiagnoses", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPaperReferal.GeneralDiagnoses));
                cmd.AddParameter("@CurrentStatusOfPt", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPaperReferal.CurrentStatusOfPt));
                cmd.AddParameter("@HosName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPaperReferal.IssuerLocation));
                cmd.AddParameter("@IssuerLocation", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPaperReferal.IssuerLocation));
                cmd.AddParameter("@IssuerCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPaperReferal.IssuerCode));
                cmd.AddParameter("@CityProvinceNamePaper", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPaperReferal.CityProvinceName));
                cmd.AddParameter("@Notes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPaperReferal.Notes));
                cmd.AddParameter("@IsActivePaper", SqlDbType.Bit, ConvertNullObjectToDBNull(newPaperReferal.IsActive));
                cmd.AddParameter("@IsChronicDisease", SqlDbType.Bit, ConvertNullObjectToDBNull(newPaperReferal.IsChronicDisease));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(newPaperReferal.PtRegistrationID));
                cmd.AddParameter("@TransferFormID", SqlDbType.BigInt, ConvertNullObjectToDBNull(newPaperReferal.TransferFormID));
                cmd.AddParameter("@TransferNum", SqlDbType.VarChar, ConvertNullObjectToDBNull(newPaperReferal.TransferNum));
                cmd.AddParameter("@IsReUse", SqlDbType.Bit, ConvertNullObjectToDBNull(newPaperReferal.IsReUse));
                cmd.AddParameter("@PaperReferalID", SqlDbType.BigInt, 16, ParameterDirection.Output);
                //====== Khu vực lưu thẻ CV

                cn.Open();

                var retVal = ExecuteNonQuery(cmd);
                PatientID = (long)cmd.Parameters["@PatientID"].Value;
                PatientCode = cmd.Parameters["@PatientCodeOutPut"].Value.ToString();
                PatientBarCode = cmd.Parameters["@PatientBarcode"].Value.ToString();

                HIID = (long)cmd.Parameters["@HIID"].Value;
                paperReferalID = (long)cmd.Parameters["@PaperReferalID"].Value; 

                return retVal > 0;
            }
        }

        const short cUpdatePatientDetails  = 0x0001;
        const short cAddNewHICard          = 0x0010;
        const short cUpdateHICard          = 0x0020;
        const short cAddPaperReferral      = 0x0100;
        const short cUpdatePaperReferral   = 0x0200;
        public override bool UpdatePatientAndHIDetails(Patient patient, bool IsAdmin, long StaffID, bool IsEditAfterRegistration, out string Result)
        {
            long HIID = 0;
            long paperReferal = 0;
            Result = "Updated OK";
            //18082018 TTM: NumberOfUpdate đc truyền từ client xuống
            //Dùng để xác định trường hợp người dùng đang muốn làm để services làm việc cho chính xác.
            int nUpdateMode = patient.NumberOfUpdate;
            bool bUpdateResult = false;

            if ((nUpdateMode & cUpdatePatientDetails) > 0)
            {
                bUpdateResult = UpdatePatient(patient, IsAdmin);
            }

            if (bUpdateResult == false)
            {
                Result = "Error Updating Patient Details";
                return false;
            }

            if ((nUpdateMode & cAddNewHICard) > 0)
            {
                bUpdateResult = AddHiItem(patient.CurrentHealthInsurance, out HIID, patient.StaffID);
            }
            else if ((nUpdateMode & cUpdateHICard) > 0)
            {
                bUpdateResult = UpdateHiItem(out Result, patient.CurrentHealthInsurance, IsEditAfterRegistration, StaffID);
            }

            if (bUpdateResult == false)
            {
                Result = "Error Updating Patient Details";
                return false;
            }
            //18082018 TTM: 
            //Do PaperReferal cần phải có HIID để lưu thông tin giấy mới, nhưng trong view gộp có thể có trường hợp thêm thẻ BHYT và giấy CV cùng lúc
            //Dẫn đến giấy CV không có HIID để thực hiện thêm mới, nên gán cho giấy CV HIID mà AddHiItem out ra.
            patient.ActivePaperReferal.HiId = HIID;

            if ((nUpdateMode & cAddPaperReferral) > 0)
            {
                bUpdateResult = AddPaperReferal(patient.ActivePaperReferal , out paperReferal);
            }
            else if ((nUpdateMode & cUpdatePaperReferral) > 0)
            {
                bUpdateResult = UpdatePaperReferal(patient.ActivePaperReferal);
            }
            
            
            return bUpdateResult;


            //17082018 TTM: Cần phải xem lại.
            //Vì nếu chỉ cần cập nhật thẻ BHYT, thông tin hành chính bệnh nhân, giấy chuyển viện
            //Hoặc cập nhật 1 2 trường trong những đối tượng đó
            //thì phải chạy cả 3 store, pass nhiều parameter chỉ để thực hiện một việc nhỏ.


            //var HIitem = patient.HealthInsurances[0];
            //var Paperitem = patient.PaperReferals[0];
            //patient.ExtractFullName();
            //if (!patient.DateBecamePatient.HasValue || patient.DateBecamePatient.Value == DateTime.MinValue)
            //{
            //    patient.DateBecamePatient = DateTime.Now;
            //}
            //using (var cn = new SqlConnection(this.ConnectionString))
            //{
            //    var cmd = new SqlCommand("spUpdateNewPatientHICardAndPaperReferral_V2", cn) { CommandType = CommandType.StoredProcedure };

            //    cmd.AddParameter("@CountryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(patient.CountryID));
            //    cmd.AddParameter("@NationalityID", SqlDbType.BigInt, ConvertNullObjectToDBNull(patient.NationalityID));
            //    cmd.AddParameter("@CityProvinceID", SqlDbType.Int, ConvertNullObjectToDBNull(patient.CityProvinceID));
            //    cmd.AddParameter("@SuburbNameID", SqlDbType.BigInt, ConvertNullObjectToDBNull(patient.SuburbNameID));
            //    cmd.AddParameter("@IDNumber", SqlDbType.Char, ConvertNullObjectToDBNull(patient.IDNumber.Trim()));
            //    cmd.AddParameter("@FirstName", SqlDbType.NVarChar, patient.FirstName);
            //    cmd.AddParameter("@MiddleName", SqlDbType.NVarChar, patient.MiddleName);
            //    cmd.AddParameter("@LastName", SqlDbType.NVarChar, patient.LastName);
            //    if (patient.GenderObj != null)
            //    {
            //        cmd.AddParameter("@Gender", SqlDbType.Char, patient.GenderObj.ID);
            //    }
            //    else
            //    {
            //        cmd.AddParameter("@Gender", SqlDbType.Char, DBNull.Value);
            //    }
            //    patient.CalDOB();

            //    cmd.AddParameter("@DOB", SqlDbType.DateTime, ConvertNullObjectToDBNull(patient.DOB));
            //    cmd.AddParameter("@AgeOnly", SqlDbType.Bit, ConvertNullObjectToDBNull(patient.AgeOnly));
            //    cmd.AddParameter("@DateBecamePatient", SqlDbType.DateTime, ConvertNullObjectToDBNull(patient.DateBecamePatient));
            //    cmd.AddParameter("@V_MaritalStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(patient.V_MaritalStatus));
            //    cmd.AddParameter("@PatientPhoto", SqlDbType.NVarChar, ConvertNullObjectToDBNull(patient.PatientPhoto));
            //    cmd.AddParameter("@PatientNotes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(patient.PatientNotes));
            //    cmd.AddParameter("@PatientStreetAddress", SqlDbType.NVarChar, ConvertNullObjectToDBNull(patient.PatientStreetAddress));
            //    cmd.AddParameter("@PatientSurburb", SqlDbType.NVarChar, ConvertNullObjectToDBNull(patient.PatientSurburb));
            //    cmd.AddParameter("@PatientPhoneNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(patient.PatientPhoneNumber));
            //    cmd.AddParameter("@PatientCellPhoneNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(patient.PatientCellPhoneNumber));
            //    cmd.AddParameter("@PatientEmailAddress", SqlDbType.NVarChar, ConvertNullObjectToDBNull(patient.PatientEmailAddress));
            //    cmd.AddParameter("@PatientEmployer", SqlDbType.NVarChar, ConvertNullObjectToDBNull(patient.PatientEmployer));
            //    cmd.AddParameter("@PatientOccupation", SqlDbType.NVarChar, ConvertNullObjectToDBNull(patient.PatientOccupation));
            //    cmd.AddParameter("@V_Ethnic", SqlDbType.BigInt, ConvertNullObjectToDBNull(patient.V_Ethnic));
            //    cmd.AddParameter("@FContactFullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(patient.FContactFullName));
            //    cmd.AddParameter("@V_FamilyRelationship", SqlDbType.BigInt, ConvertNullObjectToDBNull(patient.V_FamilyRelationship));
            //    cmd.AddParameter("@FContactAddress", SqlDbType.NVarChar, ConvertNullObjectToDBNull(patient.FContactAddress));
            //    cmd.AddParameter("@FContactHomePhone", SqlDbType.VarChar, ConvertNullObjectToDBNull(patient.FContactHomePhone));
            //    cmd.AddParameter("@FContactBusinessPhone", SqlDbType.VarChar, ConvertNullObjectToDBNull(patient.FContactBusinessPhone));
            //    cmd.AddParameter("@FContactCellPhone", SqlDbType.VarChar, ConvertNullObjectToDBNull(patient.FContactCellPhone));
            //    cmd.AddParameter("@FAlternateContact", SqlDbType.NVarChar, ConvertNullObjectToDBNull(patient.FAlternateContact));
            //    cmd.AddParameter("@FAlternatePhone", SqlDbType.NVarChar, ConvertNullObjectToDBNull(patient.FAlternatePhone));
            //    cmd.AddParameter("@FullName", SqlDbType.NVarChar, patient.FullName);
            //    cmd.AddParameter("@BloodTypeID", SqlDbType.SmallInt, ConvertNullObjectToDBNull(patient.BloodTypeID));
            //    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(patient.StaffID));
            //    cmd.AddParameter("@PatientID", SqlDbType.BigInt, patient.PatientID);
            //    //@NumberOfUpdate để nhận biết cái nào update cái nào không (thông tin hành chính, thẻ BHYT, Giấy chuyển viện)
            //    cmd.AddParameter("@NumberOfUpdate", SqlDbType.SmallInt, patient.NumberOfUpdate);
            //    cmd.AddParameter("@IsAdmin", SqlDbType.Bit, IsAdmin);


            //    cmd.AddParameter("@HosID", SqlDbType.BigInt, HIitem.HosID);
            //    cmd.AddParameter("@AcceptedBenefit", SqlDbType.Float, ConvertNullObjectToDBNull(HIitem.HIPatientBenefit));
            //    cmd.AddParameter("@IsActive", SqlDbType.Bit, HIitem.IsActive);
            //    cmd.AddParameter("@HIID", SqlDbType.BigInt, HIitem.HIID);
            //    cmd.AddParameter("@IBID", SqlDbType.Int, HIitem.IBID);
            //    cmd.AddParameter("@IsEditAfterRegistration", SqlDbType.Bit, IsEditAfterRegistration);

            //    if (HIitem.HIPCode != null && HIitem.HIPCode.Length > 0)
            //    {
            //        cmd.AddParameter("@HIPCode", SqlDbType.VarChar, HIitem.HIPCode.Trim());
            //    }
            //    else
            //    {
            //        cmd.AddParameter("@HIPCode", SqlDbType.VarChar, HIitem.HIPCode);
            //    }
            //    if (HIitem.HICardNo != null && HIitem.HICardNo.Length > 0)
            //    {
            //        cmd.AddParameter("@HICardNo", SqlDbType.VarChar, HIitem.HICardNo.Trim());
            //    }
            //    else
            //    {
            //        cmd.AddParameter("@HICardNo", SqlDbType.VarChar, HIitem.HICardNo);
            //    }
            //    if (HIitem.RegistrationCode != null && HIitem.RegistrationCode.Length > 0)
            //    {
            //        cmd.AddParameter("@RegistrationCode", SqlDbType.VarChar, HIitem.RegistrationCode.Trim());
            //    }
            //    else
            //    {
            //        cmd.AddParameter("@RegistrationCode", SqlDbType.VarChar, HIitem.RegistrationCode);
            //    }
            //    if (HIitem.RegistrationLocation != null && HIitem.RegistrationLocation.Length > 0)
            //    {
            //        cmd.AddParameter("@RegistrationLocation", SqlDbType.NVarChar, HIitem.RegistrationLocation.Trim());
            //    }
            //    else
            //    {
            //        cmd.AddParameter("@RegistrationLocation", SqlDbType.NVarChar, HIitem.RegistrationLocation);
            //    }
            //    if (HIitem.CityProvinceName != null && HIitem.CityProvinceName.Length > 0)
            //    {
            //        cmd.AddParameter("@CityProvinceName", SqlDbType.NVarChar, HIitem.CityProvinceName.Trim());
            //    }
            //    else
            //    {
            //        cmd.AddParameter("@CityProvinceName", SqlDbType.NVarChar, HIitem.CityProvinceName);
            //    }
            //    if (HIitem.PatientStreetAddress != null && HIitem.PatientStreetAddress.Length > 0)
            //    {
            //        cmd.AddParameter("@PatientStreetAddressHI", SqlDbType.NVarChar, HIitem.PatientStreetAddress.Trim());
            //    }
            //    else
            //    {
            //        cmd.AddParameter("@PatientStreetAddressHI", SqlDbType.NVarChar, HIitem.PatientStreetAddress);
            //    }
            //    cmd.AddParameter("@SuburbNameIDHI", SqlDbType.BigInt, HIitem.SuburbNameID);
            //    cmd.AddParameter("@CityProvinceID_Address", SqlDbType.BigInt, HIitem.CityProvinceID_Address);
            //    cmd.AddParameter("@KVCode", SqlDbType.BigInt, HIitem.KVCode);
            //    cmd.AddParameter("@ValidDateFrom", SqlDbType.DateTime, HIitem.ValidDateFrom);
            //    cmd.AddParameter("@ValidDateTo", SqlDbType.DateTime, HIitem.ValidDateTo);
            //    cmd.AddParameter("@V_HICardType", SqlDbType.BigInt, HIitem.HICardType.LookupID);
            //    cmd.AddParameter("@MarkAsDeleted", SqlDbType.Bit, HIitem.MarkAsDeleted);
            //    cmd.AddParameter("@ArchiveNumber", SqlDbType.NVarChar, HIitem.ArchiveNumber);
            //    cmd.AddParameter("@errStr", SqlDbType.VarChar, 50, ParameterDirection.Output);


            //    cmd.AddParameter("@HospitalID", SqlDbType.BigInt, Paperitem.Hospital.HosID);
            //    cmd.AddParameter("@RefCreatedDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Paperitem.RefCreatedDate.Value));
            //    cmd.AddParameter("@AcceptedDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Paperitem.AcceptedDate));
            //    cmd.AddParameter("@TreatmentFaculty", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Paperitem.TreatmentFaculty));
            //    cmd.AddParameter("@GeneralDiagnoses", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Paperitem.GeneralDiagnoses));
            //    cmd.AddParameter("@CurrentStatusOfPt", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Paperitem.CurrentStatusOfPt));
            //    cmd.AddParameter("@Notes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Paperitem.Notes));
            //    cmd.AddParameter("@PaperReferalID", SqlDbType.BigInt, Paperitem.RefID);
            //    cmd.AddParameter("@MarkAsDeletedPaper", SqlDbType.Bit, Paperitem.MarkAsDeleted);
            //    cmd.AddParameter("@IsActivePaper", SqlDbType.Bit, ConvertNullObjectToDBNull(Paperitem.IsActive));
            //    cmd.AddParameter("@IssuerLocation", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Paperitem.IssuerLocation));
            //    cmd.AddParameter("@IssuerCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Paperitem.IssuerCode));
            //    cmd.AddParameter("@CityProvinceNamePaper", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Paperitem.CityProvinceName));
            //    cmd.AddParameter("@IsChronicDisease", SqlDbType.Bit, ConvertNullObjectToDBNull(Paperitem.IsChronicDisease));
            //    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Paperitem.PtRegistrationID));
            //    cmd.AddParameter("@TransferFormID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Paperitem.TransferFormID));
            //    cmd.AddParameter("@TransferNum", SqlDbType.VarChar, ConvertNullObjectToDBNull(Paperitem.TransferNum));
            //    cmd.AddParameter("@IsReUse", SqlDbType.Bit, ConvertNullObjectToDBNull(Paperitem.IsReUse));
            //    cn.Open();

            //    var retVal = ExecuteNonQuery(cmd);
            //    Result = cmd.Parameters["@errStr"].Value.ToString();
            //    return retVal > 0;
            //}
        }

        #endregion
        //▲===== #011
        /// <summary>
        /// Chuyển danh sách các đối tượng HealthInsurance thành chuỗi dạng xml để insert vào database.
        /// </summary>
        /// <param name="allHIItems"></param>
        /// <returns></returns>
        private string ConvertHealthInsuranceListToXMLFormat(IEnumerable<HealthInsurance> allHIItems)
        {
            var sb = new StringBuilder();
            sb.Append("<HealthInsurances>");
            if (allHIItems != null)
            {
                foreach (var hi in allHIItems)
                {
                    sb.Append("<RecInfo>");

                    sb.AppendFormat("<HIID>{0}</HIID>", hi.HIID);
                    sb.AppendFormat("<HICardNo>{0}</HICardNo>", hi.HICardNo);
                    sb.AppendFormat("<IBID>{0}</IBID>", hi.IBID);
                    sb.AppendFormat("<HIPCode>{0}</HIPCode>", hi.HIPCode);
                    sb.AppendFormat("<RegistrationCode>{0}</RegistrationCode>", HttpUtility.HtmlEncode(hi.RegistrationCode));
                    sb.AppendFormat("<RegistrationLocation>{0}</RegistrationLocation>", HttpUtility.HtmlEncode(hi.RegistrationLocation));
                    sb.AppendFormat("<V_HICardType>{0}</V_HICardType>", hi.HICardType == null ? hi.V_HICardType : hi.HICardType.LookupID);
                    if (hi.ValidDateFrom.HasValue)
                    {
                        sb.AppendFormat("<ValidDateFrom>{0}</ValidDateFrom>", hi.ValidDateFrom.Value.ToString("yyyyMMdd HH:mm:ss.fff tt"));
                    }
                    else
                    {
                        sb.Append("<ValidDateFrom></ValidDateFrom>");
                    }
                    if (hi.ValidDateTo.HasValue)
                    {
                        sb.AppendFormat("<ValidDateTo>{0}</ValidDateTo>", hi.ValidDateTo.Value.ToString("yyyyMMdd HH:mm:ss.fff tt"));
                    }
                    else
                    {
                        sb.Append("<ValidDateTo></ValidDateTo>");
                    }
                    sb.AppendFormat("<HIPatientBenefit>{0}</HIPatientBenefit>", hi.HIPatientBenefit);
                    sb.AppendFormat("<MarkAsDeleted>{0}</MarkAsDeleted>", hi.MarkAsDeleted);
                    sb.AppendFormat("<IsActive>{0}</IsActive>", hi.IsActive);

                    sb.Append("</RecInfo>");
                }
            }
            sb.Append("</HealthInsurances>");
            return sb.ToString();
        }

        public override bool RegisterPatient(PatientRegistration info, out long PatientRegistrationID, out int SequenceNo)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                return RegisterPatient(info, conn, null, out PatientRegistrationID, out SequenceNo);
            }
        }
        public override List<PatientClassHistory> GetAllClassificationHistories(long patientID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetAllClassHistories", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, patientID);
                cn.Open();

                var reader = ExecuteReader(cmd);

                var retVal = GetPatientClassHistoryCollectionFromReader(reader);
                return retVal;
            }
        }
        public override List<HealthInsurance> GetAllHealthInsurances(long patientID, bool IncludeDeletedItems = false)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetAllHealthInsurances", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, patientID);
                cmd.AddParameter("@IncludeDeletedItems", SqlDbType.Bit, IncludeDeletedItems);
                cn.Open();

                var reader = ExecuteReader(cmd);

                var retVal = GetHealthInsuranceCollectionFromReader(reader);
                return retVal;
            }
        }
        public override HealthInsuranceHIPatientBenefit GetActiveHIBenefit(long HIID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetActiveHIBenefit", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@HIID", SqlDbType.BigInt, HIID);
                cn.Open();
                HealthInsuranceHIPatientBenefit retVal = null;

                var reader = ExecuteReader(cmd);
                if (reader != null && reader.Read())
                {
                    retVal = GetHealthInsuranceHIPatientBenefitFromReader(reader);
                }
                return retVal;
            }
        }

        public override PatientClassification GetLatestClassification(long patientID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetLatestClassification", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, patientID);
                cn.Open();
                PatientClassification retVal = null;

                var reader = ExecuteReader(cmd);
                if (reader != null && reader.Read())
                    retVal = GetPatientClassificationFromReader(reader);
                return retVal;
            }
        }
        public override HealthInsurance GetLatestHealthInsurance(long patientID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetLatestHealthInsurance", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, patientID);
                cn.Open();
                HealthInsurance retVal = null;

                var reader = ExecuteReader(cmd);
                if (reader != null && reader.Read())
                    retVal = GetHealthInsuranceFromReader(reader);
                return retVal;
            }
        }
        #region Manage Registration Types
        public override List<RegistrationType> GetAllRegistrationTypes()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetAllRegistrationTypes", cn) { CommandType = CommandType.StoredProcedure };

                cn.Open();
                var reader = ExecuteReader(cmd);

                var retVal = GetRegistrationTypeCollectionFromReader(reader);

                return retVal;
            }
        }
        #endregion

        public override HealthInsurance GetActiveHICard(long patientID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetActiveHICard", cn) { CommandType = CommandType.StoredProcedure };

                cmd.AddParameter("@PatientID", SqlDbType.BigInt, patientID);

                cn.Open();
                var reader = ExecuteReader(cmd);

                if (reader == null || !reader.Read())
                {
                    return null;
                }
                InsuranceBenefit ib = null;
                if (reader["IBID"] != DBNull.Value)
                {
                    ib = GetInsuranceBenefitFromReader(reader);
                }

                var retVal = GetHealthInsuranceFromReader(reader);
                retVal.InsuranceBenefit = ib;

                retVal.HealthInsuranceHistories = new List<HealthInsuranceHistory>().ToObservableCollection();
                retVal.HealthInsuranceHistories.Add(GetHealthInsuranceHistoryFromReader(reader));

                // TxD 01/02/2018: Because Table HealthInsuranceHIPatientBenefit is NOT USED so the following is commented out
                //if (reader.HasColumn("HIBenefitID") && reader["HIBenefitID"] != DBNull.Value)
                //{
                //    retVal.ActiveHealthInsuranceHIPatientBenefit =
                //        GetHealthInsuranceHIPatientBenefitFromReader(reader);
                //    retVal.CalcBenefit();
                //}
                return retVal;
            }
        }

        public override List<RefMedicalServiceItem> GetMedicalServiceItems(long? departmentID, long? serviceTypeID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetMedicalServiceItems", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@ServiceTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(serviceTypeID));
                cmd.AddParameter("@DepartmentID", SqlDbType.BigInt, ConvertNullObjectToDBNull(departmentID));

                cn.Open();
                var reader = ExecuteReader(cmd);

                var retVal = GetMedicalServiceItemCollectionFromReader(reader);
                return retVal;
            }
        }


        public override List<RefMedicalServiceItem> GetAllMedicalServiceItemsByType(long? serviceTypeID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetAllMedicalServiceItemsByType", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@ServiceTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(serviceTypeID));

                cn.Open();
                var reader = ExecuteReader(cmd);

                var retVal = GetMedicalServiceItemCollectionFromReader(reader);
                return retVal;
            }
        }
        public override void AddHICard(HealthInsurance hi, out long HIID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spAddHICard", cn) { CommandType = CommandType.StoredProcedure };

                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(hi.PatientID));
                cmd.AddParameter("@IBID", SqlDbType.Int, ConvertNullObjectToDBNull(hi.IBID));
                cmd.AddParameter("@HIPCode", SqlDbType.Char, ConvertNullObjectToDBNull(hi.HIPCode));
                cmd.AddParameter("@HICardNo", SqlDbType.Char, ConvertNullObjectToDBNull(hi.HICardNo));
                cmd.AddParameter("@RegistrationCode", SqlDbType.Char, ConvertNullObjectToDBNull(hi.RegistrationCode));
                cmd.AddParameter("@RegistrationLocation", SqlDbType.NVarChar, ConvertNullObjectToDBNull(hi.RegistrationLocation));
                cmd.AddParameter("@ValidDateFrom", SqlDbType.DateTime, ConvertNullObjectToDBNull(hi.ValidDateFrom));
                cmd.AddParameter("@ValidDateTo", SqlDbType.DateTime, ConvertNullObjectToDBNull(hi.ValidDateTo));
                cmd.AddParameter("@HIID", SqlDbType.BigInt, null, ParameterDirection.Output);

                cn.Open();

                var retVal = ExecuteNonQuery(cmd);

                HIID = (long)cmd.Parameters["@HIID"].Value;
            }
        }
        public override void ActivateHICard(long hiCardID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spActivateHICard", cn) { CommandType = CommandType.StoredProcedure };

                cmd.AddParameter("@HIID", SqlDbType.BigInt, hiCardID);
                cn.Open();

                var retVal = ExecuteNonQuery(cmd);
            }
        }

        // TxD 28/01/2018: Commented OUT the following because It has NEVER been USED 
        //public override bool ConfirmHIBenefit(long staffID, long patientID, long hiid, float? benefit)
        //{
        //    using (var cn = new SqlConnection(ConnectionString))
        //    {
        //        var cmd = new SqlCommand("spConfirmHIBenefit", cn) { CommandType = CommandType.StoredProcedure };

        //        cmd.AddParameter("@StaffID", SqlDbType.BigInt, staffID);
        //        cmd.AddParameter("@PatientID", SqlDbType.BigInt, patientID);
        //        cmd.AddParameter("@HIID", SqlDbType.BigInt, hiid);
        //        cmd.AddParameter("@Benefit", SqlDbType.Decimal, ConvertNullObjectToDBNull(benefit));

        //        cn.Open();

        //        var retVal = ExecuteNonQuery(cmd);
        //        return retVal > 0;
        //    }
        //}

        public override bool UpdateHICard(HealthInsurance hi)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spUpdateHICard", cn) { CommandType = CommandType.StoredProcedure };

                cmd.AddParameter("@HIID", SqlDbType.BigInt, ConvertNullObjectToDBNull(hi.HIID));
                cmd.AddParameter("@RegistrationCode", SqlDbType.Char, ConvertNullObjectToDBNull(hi.RegistrationCode));
                cmd.AddParameter("@RegistrationLocation", SqlDbType.NVarChar, ConvertNullObjectToDBNull(hi.RegistrationLocation));
                cmd.AddParameter("@ValidDateFrom", SqlDbType.DateTime, ConvertNullObjectToDBNull(hi.ValidDateFrom));
                cmd.AddParameter("@ValidDateTo", SqlDbType.DateTime, ConvertNullObjectToDBNull(hi.ValidDateTo));

                cn.Open();

                var retVal = ExecuteNonQuery(cmd);
                return retVal > 0;
            }
        }

        public override bool OpenTransaction(PatientTransaction info, out long transactionID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                return OpenTransaction(info, out transactionID, cn, null);
            }
        }
        public override bool OpenTransaction(PatientTransaction info, out long transactionID, DbConnection connection, DbTransaction tran)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            var cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spOpenTransaction";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.PtRegistrationID));
            cmd.AddParameter("@TransactionTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.TransactionTypeID));
            cmd.AddParameter("@BDID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.BDID));


            cmd.AddParameter("@TransactionRemarks", SqlDbType.NVarChar, ConvertNullObjectToDBNull(info.TransactionRemarks));
            cmd.AddParameter("@V_TranHIPayment", SqlDbType.BigInt, (long)info.V_TranHIPayment);
            cmd.AddParameter("@V_TranPatientPayment", SqlDbType.BigInt, (long)info.V_TranPatientPayment);
            cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, (long)info.V_RegistrationType);
            cmd.AddParameter("@TransactionID", SqlDbType.BigInt, 16, ParameterDirection.Output);

            var retVal = ExecuteNonQuery(cmd);

            transactionID = (long)cmd.Parameters["@TransactionID"].Value;

            return true;
        }

        public override bool OpenTransaction_InPt(PatientTransaction info, out long transactionID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                return OpenTransaction_InPt(info, out transactionID, cn, null);
            }
        }
        public override bool OpenTransaction_InPt(PatientTransaction info, out long transactionID, DbConnection connection, DbTransaction tran)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            string strXml = info.ConvertPatientTransactionInfoToXml();

            var cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spOpenTransaction_InPt_New";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@TransactionInfoXml", SqlDbType.Xml, strXml);
            
                //cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.PtRegistrationID));
            //cmd.AddParameter("@TransactionTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.TransactionTypeID));
            //cmd.AddParameter("@BDID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.BDID));


            //cmd.AddParameter("@TransactionRemarks", SqlDbType.NVarChar, ConvertNullObjectToDBNull(info.TransactionRemarks));
            //cmd.AddParameter("@V_TranHIPayment", SqlDbType.BigInt, (long)info.V_TranHIPayment);
            //cmd.AddParameter("@V_TranPatientPayment", SqlDbType.BigInt, (long)info.V_TranPatientPayment);
            //cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, (long)info.V_RegistrationType);
            
            cmd.AddParameter("@TransactionID", SqlDbType.BigInt, 16, ParameterDirection.Output);

            var retVal = ExecuteNonQuery(cmd);

            transactionID = (long)cmd.Parameters["@TransactionID"].Value;

            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="registration"></param>
        /// <param name="patientType"></param>
        /// <param name="HIServiceUsed">Có sử dụng dịch vụ bảo hiểm hay không.</param>
        /// <returns></returns>
        public override PatientTransaction CreateTransaction(PatientRegistration registration, PatientType patientType, out bool HIServiceUsed)
        {
            var tran = new PatientTransaction();
            HIServiceUsed = false;

            tran.PtRegistrationID = registration.PtRegistrationID;
            tran.PatientTransactionDetails = new List<PatientTransactionDetail>().ToObservableCollection();// new ObservableCollection<PatientTransactionDetail>();

            List<PatientRegistrationDetail> patientRegistrationDetails;
            HealthInsurance ActiveHI = null;

            using (var cn = new SqlConnection(this.ConnectionString))
            {
                var cmd = new SqlCommand("spGetAllRegistrationDetails", cn) { CommandType = CommandType.StoredProcedure };
                if (registration.FindPatient == (int)AllLookupValues.V_FindPatientType.NOI_TRU)
                {
                    cmd = new SqlCommand("spGetAllRegistrationDetails_InPt", cn) { CommandType = CommandType.StoredProcedure };
                }
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, registration.PtRegistrationID);
                cn.Open();

                var reader = ExecuteReader(cmd);

                patientRegistrationDetails = GetPatientRegistrationDetailsCollectionFromReader(reader);
                if (reader != null)
                {
                    reader.Close();
                }
            }
            var currentClassification = GetLatestClassification(registration.PatientID.Value);
            if (currentClassification.PatientType == patientType && patientType == PatientType.INSUARED_PATIENT)
            {
                ActiveHI = GetActiveHICard(registration.PatientID.Value);
            }


            foreach (var d in patientRegistrationDetails)
            {
                var tranDetails = new PatientTransactionDetail { StaffID = registration.StaffID, PtRegDetailID = d.PtRegDetailID };

                if (!d.ServiceQty.HasValue || d.ServiceQty.Value < 1)
                {
                    d.ServiceQty = 1;
                }
                if (d.RefMedicalServiceItem != null)
                {
                    if (ActiveHI != null && (d.RefMedicalServiceItem.HIAllowedPrice.HasValue && d.RefMedicalServiceItem.HIAllowedPrice.Value > 0))
                    //Kiểm tra có phải là dịch vụ bảo hiểm.
                    {
                        d.RefMedicalServiceItem.CalculatePayment(ActiveHI.InsuranceBenefit);
                        tranDetails.Amount = d.RefMedicalServiceItem.PatientPayment;// *d.ServiceQty.Value;
                        tranDetails.AmountCoPay = d.RefMedicalServiceItem.CoPayment;// *d.ServiceQty.Value;
                        tranDetails.PriceDifference = d.RefMedicalServiceItem.PriceDifference;// *d.ServiceQty.Value;
                        tranDetails.HealthInsuranceRebate = d.RefMedicalServiceItem.HIPayment;// *d.ServiceQty.Value; 

                        HIServiceUsed = true;
                    }
                    else
                    {
                        tranDetails.Amount = d.RefMedicalServiceItem.NormalPrice;// *d.ServiceQty.Value;
                    }
                }
                tranDetails.Qty = d.ServiceQty;
                tran.PatientTransactionDetails.Add(tranDetails);
            }
            return tran;
        }

        public override void GetLatestClassificationAndActiveHICard(long patientID, out PatientClassification classification, out HealthInsurance activeHI)
        {
            try
            {
                classification = GetLatestClassification(patientID);
            }
            catch (Exception)
            {

                classification = null;
            }
            try
            {
                activeHI = GetActiveHICard(patientID);
            }
            catch (Exception)
            {

                activeHI = null;
            }
        }
        /// <summary>
        /// Lấy danh sách các bệnh nhân đã trả tiền nhưng chưa được xếp vào phòng khám.
        /// </summary>
        /// <param name="locationID"></param>
        /// <returns></returns>
        public override List<PatientQueue> GetAllUnqueuedPatients(long locationID, PatientSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetAllUnqueuePatients", cn) { CommandType = CommandType.StoredProcedure };

                cmd.AddParameter("@LID", SqlDbType.BigInt, locationID);
                cmd.AddParameter("@SequenceNo", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.SequenceNo));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, pageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, pageSize);
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, bCountTotal);

                var paramTotal = new SqlParameter("@Total", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(paramTotal);

                cn.Open();

                var reader = ExecuteReader(cmd);

                var retVal = GetPatientQueueItemCollectionFromReader(reader);
                reader.Close();
                if (bCountTotal && paramTotal.Value != null)
                {
                    totalCount = (int)paramTotal.Value;
                }
                else
                    totalCount = -1;

                return retVal;
            }
        }

        /// <summary>
        /// Lấy tất cả các bệnh nhân đã được sắp xếp vào phòng khám, và có QueueID > markerQueueID.
        /// </summary>
        /// <param name="locationID"></param>
        /// <param name="queueType"></param>
        /// <returns></returns>
        public override List<PatientQueue> GetAllQueuedPatients(long locationID, long queueType)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetQueuedPatients", cn) { CommandType = CommandType.StoredProcedure };

                cmd.AddParameter("@LID", SqlDbType.BigInt, locationID);
                cmd.AddParameter("@QueueType", SqlDbType.BigInt, queueType);
                cn.Open();
                var reader = ExecuteReader(cmd);
                var retVal = GetPatientQueueItemCollectionFromReader(reader);
                return retVal;
            }
        }

        public override bool Enqueue(PatientQueue queueItem)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spEnqueue", cn) { CommandType = CommandType.StoredProcedure };

                cmd.AddParameter("@ID", SqlDbType.BigInt, queueItem.ID);
                cmd.AddParameter("returnVal", SqlDbType.Int, 4, ParameterDirection.ReturnValue);
                cn.Open();

                cmd.ExecuteNonQuery();

                var retVal = (int)cmd.Parameters["returnVal"].Value;

                return retVal > 0;
            }
        }

        public override List<PatientRegistration> GetAllRegistrations(long patientID, bool IsInPtRegistration)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetAllRegistrations", cn) { CommandType = CommandType.StoredProcedure };

                cmd.AddParameter("@PatientID", SqlDbType.BigInt, patientID);
                cmd.AddParameter("@IsInPtRegistration", SqlDbType.Bit, IsInPtRegistration);
                cn.Open();

                var reader = ExecuteReader(cmd);

                var retVal = GetPatientRegistrationCollectionFromReader(reader);
                reader.Close();

                return retVal;
            }
        }

        public override List<PatientRegistrationDetail> GetAllRegistrationDetails_ForGoToKhamBenh(long registrationID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetAllRegistrationDetails_ForGoToKhamBenh", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, registrationID);
                cn.Open();

                var reader = ExecuteReader(cmd);

                return GetPatientRegistrationDetailsCollectionFromReader(reader);
            }
        }

        public override List<PatientRegistrationDetail> GetAllRegistrationDetails(long registrationID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetAllRegistrationDetails", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, registrationID);
                cn.Open();

                var reader = ExecuteReader(cmd);

                return GetPatientRegistrationDetailsCollectionFromReader(reader);
            }
        }

        public override List<PatientRegistrationDetail> GetAllRegistrationDetails_ByV_ExamRegStatus(Int64 PtRegistrationID, Int64 V_ExamRegStatus)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetAllRegistrationDetails_ByV_ExamRegStatus", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, PtRegistrationID);
                cmd.AddParameter("@V_ExamRegStatus", SqlDbType.BigInt, V_ExamRegStatus);
                cn.Open();

                var reader = ExecuteReader(cmd);

                return GetPatientRegistrationDetailsCollectionFromReader(reader);
            }
        }
        public override List<PatientRegistrationDetail> GetAllRegistrationDetails_ByV_ExamRegStatus(Int64 PtRegistrationID, Int64 V_ExamRegStatus, DbConnection connection, DbTransaction tran)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            var cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spGetAllRegistrationDetails_ByV_ExamRegStatus";

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, PtRegistrationID);
            cmd.AddParameter("@V_ExamRegStatus", SqlDbType.BigInt, V_ExamRegStatus);

            var reader = ExecuteReader(cmd);
            if (reader == null)
            {
                return null;
            }
            var retVal = GetPatientRegistrationDetailsCollectionFromReader(reader);
            reader.Close();
            return retVal;
        }

        public override PatientRegistration GetRegistration(long registrationID, int FindPatient)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                return GetRegistration(registrationID, FindPatient, cn, null);
            }
        }


        public override PatientRegistration GetRegistraionVIPByPatientID(long PatientID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetRegistraionVIPByPatientID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientID", SqlDbType.BigInt, PatientID);

                cn.Open();

                PatientRegistration Res = null;

                IDataReader reader = ExecuteReader(cmd);

                if (reader.Read())
                {
                    Res = GetPatientRegistrationFromReader(reader);
                }

                reader.Close();
                return Res;
            }
        }


        public override List<PatientTransactionDetail> GetAlltransactionDetails(long transactionID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetAllTransactionDetails", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@TransactionID", SqlDbType.BigInt, transactionID);
                cn.Open();

                var reader = ExecuteReader(cmd);

                return GetTransactionDetailsCollectionFromReader(reader);
            }
        }

        public override List<PatientTransactionDetail> GetAlltransactionDetails_InPt(long transactionID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetAllTransactionDetails_InPt", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@TransactionID", SqlDbType.BigInt, transactionID);
                cn.Open();

                var reader = ExecuteReader(cmd);

                return GetTransactionDetailsCollectionFromReader(reader);
            }
        }
        public override List<PatientTransactionDetail> GetAlltransactionDetailsSum(long transactionID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetAllTransactionDetails_all", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@TransactionID", SqlDbType.BigInt, transactionID);
                cn.Open();

                var reader = ExecuteReader(cmd);

                return GetTransactionDetailsCollectionFromReader(reader);
            }
        }

        public override List<PatientTransactionDetail> GetAlltransactionDetailsSum_InPt(long transactionID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetAllTransactionDetails_all_InPt", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@TransactionID", SqlDbType.BigInt, transactionID);
                cn.Open();

                var reader = ExecuteReader(cmd);

                return GetTransactionDetailsCollectionFromReader(reader);
            }
        }

        public override bool AddPaperReferal(PaperReferal referal, out long paperReferalID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                return AddPaperReferal(referal, out paperReferalID, cn);
            }
        }

        /// <summary>
        /// Thêm emergency record vào database. Dùng hàm này trong phần đăng ký bệnh nhân (để sử dụng chung connection.)
        /// </summary>
        /// <param name="emergencyRecord"></param>
        /// <param name="emergencyRecordID"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        /// HPT: Huyền xóa hàm AddEmergencyRecord vì đã kiểm tra và thấy không dùng đến (17/10/2016) . Kiểm tra Bảng EmergencyRecords cũng không thấy có dữ liệu
        /// <summary>
        /// Thêm paper referal vào database. Dùng hàm này trong phần đăng ký bệnh nhân (để sử dụng chung connection.)
        /// </summary>
        /// <param name="referal"></param>
        /// <param name="paperReferalID"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        private bool AddPaperReferal(PaperReferal referal, out long paperReferalID, SqlConnection conn)
        {
            var cmd = new SqlCommand("spPaperReferal_Add", conn) { CommandType = CommandType.StoredProcedure };

            //18082018 TTM: 
            //Vì hàm AddPaperReferal hiện tại đang được xài chung. Mà giá trị truyền vào của 2 hàm xài hàm này khác nhau
            //Nên cần phải chia trường hợp ra để sử dụng cho chính xác.
            if (referal.HealthInsurance == null)
            {
                cmd.AddParameter("@HIID", SqlDbType.BigInt, referal.HiId);
            }
            else
            {
                cmd.AddParameter("@HIID", SqlDbType.BigInt, referal.HealthInsurance.HIID);
            }
            cmd.AddParameter("@HospitalID", SqlDbType.BigInt, referal.Hospital.HosID);
            cmd.AddParameter("@RefCreatedDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(referal.RefCreatedDate));
            cmd.AddParameter("@AcceptedDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(referal.AcceptedDate));
            cmd.AddParameter("@TreatmentFaculty", SqlDbType.NVarChar, ConvertNullObjectToDBNull(referal.TreatmentFaculty));
            cmd.AddParameter("@GeneralDiagnoses", SqlDbType.NVarChar, ConvertNullObjectToDBNull(referal.GeneralDiagnoses));
            cmd.AddParameter("@CurrentStatusOfPt", SqlDbType.NVarChar, ConvertNullObjectToDBNull(referal.CurrentStatusOfPt));
            cmd.AddParameter("@HosName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(referal.Hospital.HosName));

            cmd.AddParameter("@IssuerLocation", SqlDbType.NVarChar, ConvertNullObjectToDBNull(referal.IssuerLocation));
            cmd.AddParameter("@IssuerCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(referal.IssuerCode));
            cmd.AddParameter("@CityProvinceName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(referal.CityProvinceName));

            cmd.AddParameter("@Notes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(referal.Notes));
            cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(referal.IsActive));

            cmd.AddParameter("@IsChronicDisease", SqlDbType.Bit, ConvertNullObjectToDBNull(referal.IsChronicDisease));
            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(referal.PtRegistrationID));

            /*TMA*/
            cmd.AddParameter("@TransferFormID", SqlDbType.BigInt, ConvertNullObjectToDBNull(referal.TransferFormID));
            cmd.AddParameter("@TransferNum", SqlDbType.VarChar, ConvertNullObjectToDBNull(referal.TransferNum));
            cmd.AddParameter("@IsReUse", SqlDbType.Bit, ConvertNullObjectToDBNull(referal.IsReUse));
            /*TMA*/

            cmd.AddParameter("@PaperReferalID", SqlDbType.BigInt, 16, ParameterDirection.Output);

            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            var retVal = ExecuteNonQuery(cmd);

            paperReferalID = (long)cmd.Parameters["@PaperReferalID"].Value;

            return retVal > 0;
        }

        public override bool UpdatePaperReferal(PaperReferal referal)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spPaperReferal_Update", conn) { CommandType = CommandType.StoredProcedure };

                cmd.AddParameter("@HospitalID", SqlDbType.BigInt, referal.Hospital.HosID);
                cmd.AddParameter("@RefCreatedDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(referal.RefCreatedDate.Value));
                cmd.AddParameter("@AcceptedDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(referal.AcceptedDate));
                cmd.AddParameter("@TreatmentFaculty", SqlDbType.NVarChar, ConvertNullObjectToDBNull(referal.TreatmentFaculty));
                cmd.AddParameter("@GeneralDiagnoses", SqlDbType.NVarChar, ConvertNullObjectToDBNull(referal.GeneralDiagnoses));
                cmd.AddParameter("@CurrentStatusOfPt", SqlDbType.NVarChar, ConvertNullObjectToDBNull(referal.CurrentStatusOfPt));
                cmd.AddParameter("@Notes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(referal.Notes));
                cmd.AddParameter("@PaperReferalID", SqlDbType.BigInt, referal.RefID);
                cmd.AddParameter("@MarkAsDeleted", SqlDbType.Bit, referal.MarkAsDeleted);
                cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(referal.IsActive));

                cmd.AddParameter("@IssuerLocation", SqlDbType.NVarChar, ConvertNullObjectToDBNull(referal.IssuerLocation));
                cmd.AddParameter("@IssuerCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(referal.IssuerCode));
                cmd.AddParameter("@CityProvinceName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(referal.CityProvinceName));

                cmd.AddParameter("@IsChronicDisease", SqlDbType.Bit, ConvertNullObjectToDBNull(referal.IsChronicDisease));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(referal.PtRegistrationID));
                /*TMA*/
                cmd.AddParameter("@TransferFormID", SqlDbType.BigInt, ConvertNullObjectToDBNull(referal.TransferFormID));
                cmd.AddParameter("@TransferNum", SqlDbType.VarChar, ConvertNullObjectToDBNull(referal.TransferNum));
                cmd.AddParameter("@IsReUse", SqlDbType.Bit, ConvertNullObjectToDBNull(referal.IsReUse));
                /*TMA*/
                conn.Open();

                var retVal = ExecuteNonQuery(cmd);

                return true;
            }
        }

        public override bool UpdatePaperReferalRegID(PaperReferal referal)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spPaperReferal_UpdateRegID", conn) { CommandType = CommandType.StoredProcedure };

                cmd.AddParameter("@PaperReferalID", SqlDbType.BigInt, referal.RefID);
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(referal.PtRegistrationID));

                conn.Open();

                var retVal = ExecuteNonQuery(cmd);

                return true;
            }
        }

        public override bool DeletePaperReferal(PaperReferal referal)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spPaperReferal_Delete", conn) { CommandType = CommandType.StoredProcedure };

                cmd.AddParameter("@PaperReferalID", SqlDbType.BigInt, referal.RefID);

                conn.Open();

                var retVal = ExecuteNonQuery(cmd);

                return true;
            }
        }

        public override bool SaveHIItems(long patientID, List<HealthInsurance> allHiItems, out long? activeHICardID, long StaffID = 0)
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                var cmd = new SqlCommand("spUpdatePatientHIItems", cn) { CommandType = CommandType.StoredProcedure };

                IEnumerable<HealthInsurance> colAddedHI = null;
                IEnumerable<HealthInsurance> colUpdatedHI = null;
                IEnumerable<HealthInsurance> colDeletedHI = null;
                IEnumerable<HealthInsurance> colActiveHI = null;

                if (allHiItems != null)
                {
                    colAddedHI = allHiItems.Where(hi => (hi.EntityState == EntityState.NEW || hi.EntityState == EntityState.DETACHED) && hi.IsActive == false);

                    colUpdatedHI = allHiItems.Where(hi => (hi.EntityState == EntityState.MODIFIED || hi.EntityState == EntityState.DELETED_MODIFIED) && hi.IsActive == false);

                    colDeletedHI = allHiItems.Where(hi => hi.EntityState == EntityState.DELETED_MODIFIED);

                    colActiveHI = allHiItems.Where(hi => (hi.EntityState == EntityState.MODIFIED || hi.EntityState == EntityState.NEW || hi.EntityState == EntityState.DETACHED) && hi.IsActive == true);
                }

                cmd.AddParameter("@HealthInsuranceCollectionXML_Add", SqlDbType.Xml, ConvertHealthInsuranceListToXMLFormat(colAddedHI));
                cmd.AddParameter("@HealthInsuranceCollectionXML_Update", SqlDbType.Xml, ConvertHealthInsuranceListToXMLFormat(colUpdatedHI));
                cmd.AddParameter("@HealthInsuranceCollectionXML_Delete", SqlDbType.Xml, ConvertHealthInsuranceListToXMLFormat(colDeletedHI));
                cmd.AddParameter("@ActiveHICardInfo", SqlDbType.Xml, ConvertHealthInsuranceListToXMLFormat(colActiveHI));

                cmd.AddParameter("@StaffID", SqlDbType.BigInt, StaffID);
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, patientID);
                cmd.AddParameter("@ActiveHICardID", SqlDbType.BigInt, 16, ParameterDirection.Output);

                cn.Open();

                var retVal = ExecuteNonQuery(cmd);
                activeHICardID = cmd.Parameters["@ActiveHICardID"].Value as long?;
                return true;
            }
        }
        public override bool AddHospital(Hospital entity)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spHospitalsInsert", cn) { CommandType = CommandType.StoredProcedure };


                cmd.AddParameter("@HICode", SqlDbType.VarChar, ConvertNullObjectToDBNull(entity.HICode));
                cmd.AddParameter("@CityProvinceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.CityProvinceID));
                cmd.AddParameter("@HospitalCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(entity.HospitalCode));
                cmd.AddParameter("@HosShortName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.HosShortName));
                cmd.AddParameter("@HosName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.HosName));
                cmd.AddParameter("@HosAddress", SqlDbType.NVarChar, ConvertNullObjectToDBNull(
                    string.IsNullOrEmpty(entity.HosAddress) ? "Không rõ" : entity.HosAddress));
                cmd.AddParameter("@HosLogoImgPath", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.HosLogoImgPath));
                cmd.AddParameter("@Slogan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Slogan));
                cmd.AddParameter("@HosPhone", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.HosPhone));
                cmd.AddParameter("@HosWebSite", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.HosWebSite));
                cmd.AddParameter("@V_HospitalType", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_HospitalType));
                cmd.AddParameter("@IsFriends", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.IsFriends));
                cmd.AddParameter("@UsedForPaperReferralOnly", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.UsedForPaperReferralOnly));
                cn.Open();

                var retVal = ExecuteNonQuery(cmd);
                return retVal > 0;
            }
        }

        public override bool UpdateHospital(Hospital entity)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spHospitalsUpdate", cn) { CommandType = CommandType.StoredProcedure };

                cmd.AddParameter("@HICode", SqlDbType.VarChar, ConvertNullObjectToDBNull(entity.HICode));
                cmd.AddParameter("@CityProvinceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.CityProvinceID));
                cmd.AddParameter("@HospitalCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(entity.HospitalCode));
                cmd.AddParameter("@HosShortName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.HosShortName));
                cmd.AddParameter("@HosName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.HosName));
                cmd.AddParameter("@HosAddress", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.HosAddress));
                cmd.AddParameter("@HosLogoImgPath", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.HosLogoImgPath));
                cmd.AddParameter("@Slogan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Slogan));
                cmd.AddParameter("@HosPhone", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.HosPhone));
                cmd.AddParameter("@HosWebSite", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.HosWebSite));
                cmd.AddParameter("@V_HospitalType", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_HospitalType));
                cmd.AddParameter("@HosID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.HosID));
                cmd.AddParameter("@IsFriends", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.IsFriends));
                cmd.AddParameter("@UsedForPaperReferralOnly", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.UsedForPaperReferralOnly));
                cn.Open();

                var retVal = ExecuteNonQuery(cmd);
                return retVal > 0;
            }
        }

        public override bool AddHospitalByHiCode(string hospitalName, string hiCode)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("sp_Hospitals_AddByHiCode", cn) { CommandType = CommandType.StoredProcedure };


                cmd.AddParameter("@HosName", SqlDbType.NVarChar, hospitalName);
                cmd.AddParameter("@HICode", SqlDbType.VarChar, hiCode);
                cn.Open();

                var retVal = ExecuteNonQuery(cmd);
                return retVal > 0;
            }
        }

        public override bool AddHiItem(HealthInsurance hiItem, out long HIID, long StaffID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spHealthInsurance_Add", cn) { CommandType = CommandType.StoredProcedure };

                long patientID = -1;
                if (hiItem.Patient != null)
                {
                    patientID = hiItem.Patient.PatientID;
                }
                else
                {
                    if (hiItem.PatientID.HasValue)
                    {
                        patientID = hiItem.PatientID.Value;
                    }
                }
                if (patientID == -1)
                {
                    throw new Exception(eHCMSResources.Z1681_G1_ChuaCoIDBN);
                }
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, StaffID);
                cmd.AddParameter("@AcceptedBenefit", SqlDbType.Float, ConvertNullObjectToDBNull(hiItem.HIPatientBenefit));
                cmd.AddParameter("@IsActive", SqlDbType.Bit, hiItem.IsActive);

                cmd.AddParameter("@HosID", SqlDbType.BigInt, hiItem.HosID);

                cmd.AddParameter("@PatientID", SqlDbType.BigInt, patientID);
                cmd.AddParameter("@IBID", SqlDbType.Int, hiItem.IBID);
                if (hiItem.HIPCode != null && hiItem.HIPCode.Length > 0)
                {
                    cmd.AddParameter("@HIPCode", SqlDbType.VarChar, hiItem.HIPCode.Trim());
                }
                else
                {
                    cmd.AddParameter("@HIPCode", SqlDbType.VarChar, hiItem.HIPCode);
                }
                if (hiItem.HICardNo != null && hiItem.HICardNo.Length > 0)
                {
                    cmd.AddParameter("@HICardNo", SqlDbType.VarChar, hiItem.HICardNo.Trim());
                }
                else
                {
                    cmd.AddParameter("@HICardNo", SqlDbType.VarChar, hiItem.HICardNo);
                }
                if (hiItem.RegistrationCode != null && hiItem.RegistrationCode.Length > 0)
                {
                    cmd.AddParameter("@RegistrationCode", SqlDbType.VarChar, hiItem.RegistrationCode.Trim());
                }
                else
                {
                    cmd.AddParameter("@RegistrationCode", SqlDbType.VarChar, hiItem.RegistrationCode);
                }
                if (hiItem.RegistrationLocation != null && hiItem.RegistrationLocation.Length > 0)
                {
                    cmd.AddParameter("@RegistrationLocation", SqlDbType.NVarChar, hiItem.RegistrationLocation.Trim());
                }
                else
                {
                    cmd.AddParameter("@RegistrationLocation", SqlDbType.NVarChar, hiItem.RegistrationLocation);
                }
                if (hiItem.CityProvinceName != null && hiItem.CityProvinceName.Length > 0)
                {
                    cmd.AddParameter("@CityProvinceName", SqlDbType.NVarChar, hiItem.CityProvinceName.Trim());
                }
                else
                {
                    cmd.AddParameter("@CityProvinceName", SqlDbType.NVarChar, hiItem.CityProvinceName);
                }
                cmd.AddParameter("@ValidDateFrom", SqlDbType.DateTime, hiItem.ValidDateFrom);
                cmd.AddParameter("@ValidDateTo", SqlDbType.DateTime, hiItem.ValidDateTo);
                cmd.AddParameter("@V_HICardType", SqlDbType.BigInt, hiItem.HICardType.LookupID);
                cmd.AddParameter("@ArchiveNumber", SqlDbType.NVarChar, hiItem.ArchiveNumber);
                cmd.AddParameter("@CofirmDuplicate", SqlDbType.Bit, hiItem.CofirmDuplicate);
                cmd.AddParameter("@HIID", SqlDbType.BigInt, 16, ParameterDirection.Output);
                if (hiItem.PatientStreetAddress != null && hiItem.PatientStreetAddress.Length > 0)
                {
                    cmd.AddParameter("@PatientStreetAddress", SqlDbType.NVarChar, hiItem.PatientStreetAddress.Trim());
                }
                else
                {
                    cmd.AddParameter("@PatientStreetAddress", SqlDbType.NVarChar, hiItem.PatientStreetAddress);
                }
                cmd.AddParameter("@SuburbNameID", SqlDbType.BigInt, hiItem.SuburbNameID);
                cmd.AddParameter("@CityProvinceID_Address", SqlDbType.BigInt, hiItem.CityProvinceID_Address);
                cmd.AddParameter("@KVCode", SqlDbType.BigInt, hiItem.KVCode);

                //▼====== #012
                cmd.AddParameter("@WardNameID", SqlDbType.BigInt, ConvertNullObjectToDBNull(hiItem.WardNameID));
                cmd.AddParameter("@WardName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(hiItem.WardName));
                //▲====== #012

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                HIID = (long)cmd.Parameters["@HIID"].Value;
                return true;
            }
        }

        private bool AddHiItem_V2(HealthInsurance hiItem, long StaffID, out long HIID, out double RebatePercentage)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spHealthInsurance_Add_V2", cn) { CommandType = CommandType.StoredProcedure };
                RebatePercentage = 0;
                long patientID = -1;
                if (hiItem.Patient != null)
                {
                    patientID = hiItem.Patient.PatientID;
                }
                else
                {
                    if (hiItem.PatientID.HasValue)
                    {
                        patientID = hiItem.PatientID.Value;
                    }
                }
                if (patientID == -1)
                {
                    throw new Exception(eHCMSResources.Z1681_G1_ChuaCoIDBN);
                }
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, StaffID);
                cmd.AddParameter("@AcceptedBenefit", SqlDbType.Float, ConvertNullObjectToDBNull(hiItem.HIPatientBenefit));
                cmd.AddParameter("@IsActive", SqlDbType.Bit, hiItem.IsActive);

                cmd.AddParameter("@HosID", SqlDbType.BigInt, hiItem.HosID);

                cmd.AddParameter("@PatientID", SqlDbType.BigInt, patientID);
                cmd.AddParameter("@IBID", SqlDbType.Int, hiItem.IBID);
                if (hiItem.HIPCode != null && hiItem.HIPCode.Length > 0)
                {
                    cmd.AddParameter("@HIPCode", SqlDbType.VarChar, hiItem.HIPCode.Trim());
                }
                else
                {
                    cmd.AddParameter("@HIPCode", SqlDbType.VarChar, hiItem.HIPCode);
                }
                if (hiItem.HICardNo != null && hiItem.HICardNo.Length > 0)
                {
                    cmd.AddParameter("@HICardNo", SqlDbType.VarChar, hiItem.HICardNo.Trim());
                }
                else
                {
                    cmd.AddParameter("@HICardNo", SqlDbType.VarChar, hiItem.HICardNo);
                }
                if (hiItem.RegistrationCode != null && hiItem.RegistrationCode.Length > 0)
                {
                    cmd.AddParameter("@RegistrationCode", SqlDbType.VarChar, hiItem.RegistrationCode.Trim());
                }
                else
                {
                    cmd.AddParameter("@RegistrationCode", SqlDbType.VarChar, hiItem.RegistrationCode);
                }
                if (hiItem.RegistrationLocation != null && hiItem.RegistrationLocation.Length > 0)
                {
                    cmd.AddParameter("@RegistrationLocation", SqlDbType.NVarChar, hiItem.RegistrationLocation.Trim());
                }
                else
                {
                    cmd.AddParameter("@RegistrationLocation", SqlDbType.NVarChar, hiItem.RegistrationLocation);
                }
                if (hiItem.CityProvinceName != null && hiItem.CityProvinceName.Length > 0)
                {
                    cmd.AddParameter("@CityProvinceName", SqlDbType.NVarChar, hiItem.CityProvinceName.Trim());
                }
                else
                {
                    cmd.AddParameter("@CityProvinceName", SqlDbType.NVarChar, hiItem.CityProvinceName);
                }
                cmd.AddParameter("@ValidDateFrom", SqlDbType.DateTime, hiItem.ValidDateFrom);
                cmd.AddParameter("@ValidDateTo", SqlDbType.DateTime, hiItem.ValidDateTo);
                cmd.AddParameter("@V_HICardType", SqlDbType.BigInt, hiItem.HICardType.LookupID);
                cmd.AddParameter("@ArchiveNumber", SqlDbType.NVarChar, hiItem.ArchiveNumber);
                cmd.AddParameter("@CofirmDuplicate", SqlDbType.Bit, hiItem.CofirmDuplicate);
                cmd.AddParameter("@HIID", SqlDbType.BigInt, 16, ParameterDirection.Output);
                if (hiItem.PatientStreetAddress != null && hiItem.PatientStreetAddress.Length > 0)
                {
                    cmd.AddParameter("@PatientStreetAddress", SqlDbType.NVarChar, hiItem.PatientStreetAddress.Trim());
                }
                else
                {
                    cmd.AddParameter("@PatientStreetAddress", SqlDbType.NVarChar, hiItem.PatientStreetAddress);
                }
                cmd.AddParameter("@SuburbNameID", SqlDbType.BigInt, hiItem.SuburbNameID);
                cmd.AddParameter("@CityProvinceID_Address", SqlDbType.BigInt, hiItem.CityProvinceID_Address);
                cmd.AddParameter("@KVCode", SqlDbType.BigInt, hiItem.KVCode);

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                HIID = (long)cmd.Parameters["@HIID"].Value;
                return true;
            }
        }

        public override bool UpdateHiItem(out string Result, HealthInsurance hiItem, bool IsEditAfterRegistration, long StaffID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spHealthInsurance_Update", cn) { CommandType = CommandType.StoredProcedure };

                long patientID = -1;
                if (hiItem.Patient != null)
                {
                    patientID = hiItem.Patient.PatientID;
                }
                else
                {
                    if (hiItem.PatientID.HasValue)
                    {
                        patientID = hiItem.PatientID.Value;
                    }
                }
                if (patientID == -1)
                {
                    throw new Exception(eHCMSResources.Z1681_G1_ChuaCoIDBN);
                }

                cmd.AddParameter("@HosID", SqlDbType.BigInt, hiItem.HosID);

                cmd.AddParameter("@StaffID", SqlDbType.BigInt, StaffID);
                cmd.AddParameter("@AcceptedBenefit", SqlDbType.Float, ConvertNullObjectToDBNull(hiItem.HIPatientBenefit));
                cmd.AddParameter("@IsActive", SqlDbType.Bit, hiItem.IsActive);
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, patientID);

                cmd.AddParameter("@HIID", SqlDbType.BigInt, hiItem.HIID);
                cmd.AddParameter("@IBID", SqlDbType.Int, hiItem.IBID);

                cmd.AddParameter("@IsEditAfterRegistration", SqlDbType.Bit, IsEditAfterRegistration);

                if (hiItem.HIPCode != null && hiItem.HIPCode.Length > 0)
                {
                    cmd.AddParameter("@HIPCode", SqlDbType.VarChar, hiItem.HIPCode.Trim());
                }
                else
                {
                    cmd.AddParameter("@HIPCode", SqlDbType.VarChar, hiItem.HIPCode);
                }
                if (hiItem.HICardNo != null && hiItem.HICardNo.Length > 0)
                {
                    cmd.AddParameter("@HICardNo", SqlDbType.VarChar, hiItem.HICardNo.Trim());
                }
                else
                {
                    cmd.AddParameter("@HICardNo", SqlDbType.VarChar, hiItem.HICardNo);
                }
                if (hiItem.RegistrationCode != null && hiItem.RegistrationCode.Length > 0)
                {
                    cmd.AddParameter("@RegistrationCode", SqlDbType.VarChar, hiItem.RegistrationCode.Trim());
                }
                else
                {
                    cmd.AddParameter("@RegistrationCode", SqlDbType.VarChar, hiItem.RegistrationCode);
                }
                if (hiItem.RegistrationLocation != null && hiItem.RegistrationLocation.Length > 0)
                {
                    cmd.AddParameter("@RegistrationLocation", SqlDbType.NVarChar, hiItem.RegistrationLocation.Trim());
                }
                else
                {
                    cmd.AddParameter("@RegistrationLocation", SqlDbType.NVarChar, hiItem.RegistrationLocation);
                }
                if (hiItem.CityProvinceName != null && hiItem.CityProvinceName.Length > 0)
                {
                    cmd.AddParameter("@CityProvinceName", SqlDbType.NVarChar, hiItem.CityProvinceName.Trim());
                }
                else
                {
                    cmd.AddParameter("@CityProvinceName", SqlDbType.NVarChar, hiItem.CityProvinceName);
                }
                if (hiItem.PatientStreetAddress != null && hiItem.PatientStreetAddress.Length > 0)
                {
                    cmd.AddParameter("@PatientStreetAddress", SqlDbType.NVarChar, hiItem.PatientStreetAddress.Trim());
                }
                else
                {
                    cmd.AddParameter("@PatientStreetAddress", SqlDbType.NVarChar, hiItem.PatientStreetAddress);
                }
                cmd.AddParameter("@SuburbNameID", SqlDbType.BigInt, hiItem.SuburbNameID);
                cmd.AddParameter("@CityProvinceID_Address", SqlDbType.BigInt, hiItem.CityProvinceID_Address);
                cmd.AddParameter("@KVCode", SqlDbType.BigInt, hiItem.KVCode);
                cmd.AddParameter("@ValidDateFrom", SqlDbType.DateTime, hiItem.ValidDateFrom);
                cmd.AddParameter("@ValidDateTo", SqlDbType.DateTime, hiItem.ValidDateTo);
                cmd.AddParameter("@V_HICardType", SqlDbType.BigInt, hiItem.HICardType.LookupID);
                cmd.AddParameter("@MarkAsDeleted", SqlDbType.Bit, hiItem.MarkAsDeleted);
                cmd.AddParameter("@ArchiveNumber", SqlDbType.NVarChar, hiItem.ArchiveNumber);

                //▼====== #012
                cmd.AddParameter("@WardNameID", SqlDbType.BigInt, ConvertNullObjectToDBNull(hiItem.WardNameID));
                cmd.AddParameter("@WardName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(hiItem.WardName));
                //▲====== #012

                cmd.AddParameter("@errStr", SqlDbType.VarChar, 50, ParameterDirection.Output);

                cn.Open();

                var retVal = ExecuteNonQuery(cmd);
                Result = cmd.Parameters["@errStr"].Value.ToString();
                return retVal > 0;
            }
        }
        //public override List<HICardType> GetHICardTypeList()
        //{
        //    using (var conn = new SqlConnection(ConnectionString))
        //    {
        //        var cmd = new SqlCommand("spPaperReferal_GetAllByHealthInsuranceID", conn) { CommandType = CommandType.StoredProcedure };
        //        //cmd.AddParameter("@HIID", SqlDbType.BigInt, hiid);
        //        //cmd.AddParameter("@IncludeDeletedItems", SqlDbType.Bit, IncludeDeletedItems);
        //        if (conn.State != ConnectionState.Open)
        //        {
        //            conn.Open();
        //        }
        //        var reader = ExecuteReader(cmd);

        //        var retVal = GetHICardTypeCollectionFromReader(reader);
        //        return retVal;
        //    }
        //}


        public override List<PaperReferal> GetAllPaperReferalsByHealthInsurance(long hiid, bool IncludeDeletedItems = false)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                return GetAllPaperReferalsByHealthInsurance(hiid, cn, IncludeDeletedItems);
            }
        }
        private List<PaperReferal> GetAllPaperReferalsByHealthInsurance(long hiid, SqlConnection conn, bool IncludeDeletedItems = false)
        {
            var cmd = new SqlCommand("spPaperReferal_GetAllByHealthInsuranceID", conn) { CommandType = CommandType.StoredProcedure };
            cmd.AddParameter("@HIID", SqlDbType.BigInt, hiid);
            cmd.AddParameter("@IncludeDeletedItems", SqlDbType.Bit, IncludeDeletedItems);
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            var reader = ExecuteReader(cmd);

            var retVal = GetPaperReferalCollectionFromReader(reader);
            return retVal;
        }


        /// <summary>
        /// Lưu lại những thay đỏi khi người dùng cập nhật đăng ký
        /// Chi luu trong bang dang ky va chi tiet cua no thoi.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="PatientRegistrationID"></param>
        /// <param name="SequenceNo"></param>
        /// <returns></returns>
        public override bool SaveChangesOnRegistration(PatientRegistration info)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                return SaveChangesOnRegistration(info, cn, null);
            }
        }
        public override bool SaveChangesOnRegistration(PatientRegistration info, DbConnection connection, DbTransaction tran)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            SqlCommand cmd;
            cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spSaveChangesOnRegistration";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@PatientRegistrationID", SqlDbType.BigInt, info.PtRegistrationID);
            cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.StaffID));
            cmd.AddParameter("@V_RegistrationStatus", SqlDbType.BigInt, (long)info.RegistrationStatus);
            cmd.AddParameter("@V_DocumentTypeOnHold", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.V_DocumentTypeOnHold));
            cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, (long)info.V_RegistrationType);

            IEnumerable<PatientRegistrationDetail> colAddedItems = null;
            IEnumerable<PatientRegistrationDetail> colUpdatedItems = null;
            IEnumerable<PatientRegistrationDetail> colDeletedItems = null;

            if (info.PatientRegistrationDetails != null)
            {
                colAddedItems = info.PatientRegistrationDetails.Where(item =>
                {
                    return item.RecordState == RecordState.DETACHED
                        || item.RecordState == RecordState.ADDED;
                });


                colUpdatedItems = info.PatientRegistrationDetails.Where(item =>
                {
                    return item.RecordState == RecordState.MODIFIED;
                });

                colDeletedItems = info.PatientRegistrationDetails.Where(item =>
                {
                    return item.RecordState == RecordState.DELETED;
                });
            }

            cmd.AddParameter("@PatientRegistrationDetails_Add", SqlDbType.Xml, info.ConvertDetailsListToXml(colAddedItems));
            cmd.AddParameter("@PatientRegistrationDetails_Update", SqlDbType.Xml, info.ConvertDetailsListToXml(colUpdatedItems));
            cmd.AddParameter("@PatientRegistrationDetails_Delete", SqlDbType.Xml, info.ConvertDetailsListToXml(colDeletedItems));

            int retVal = ExecuteNonQuery(cmd);

            return true;
        }

        public override bool SaveChangesOnTransaction(PatientTransaction info)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                return SaveChangesOnTransaction(info, cn, null);
            }
        }

        public override bool SaveChangesOnTransaction(PatientTransaction info, DbConnection connection, DbTransaction tran)
        {
            SqlCommand cmd;
            cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spUpdateTransaction";
            cmd.CommandType = CommandType.StoredProcedure;

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            cmd.AddParameter("@TransactionID", SqlDbType.BigInt, info.TransactionID);
            cmd.AddParameter("@TransactionRemarks", SqlDbType.NVarChar, ConvertNullObjectToDBNull(info.TransactionRemarks));
            cmd.AddParameter("@V_TranHIPayment", SqlDbType.BigInt, (long)info.V_TranHIPayment);
            cmd.AddParameter("@V_TranPatientPayment", SqlDbType.BigInt, (long)info.V_TranPatientPayment);
            cmd.AddParameter("@IsClosed", SqlDbType.Bit, ConvertNullObjectToDBNull(info.IsClosed));
            cmd.AddParameter("@IsAdjusted", SqlDbType.Bit, ConvertNullObjectToDBNull(info.IsAdjusted));

            int retVal = ExecuteNonQuery(cmd);

            return true;
        }
        //public override bool ProcessPayment(PatientPayment payment, long transactionID, out long PaymentID)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        return ProcessPayment(payment, transactionID, out PaymentID, cn, null);
        //    }
        //}
        //public override bool ProcessPayment(PatientPayment payment, long transactionID, out long PaymentID, DbConnection connection, DbTransaction tran)
        //{
        //    if (connection.State != ConnectionState.Open)
        //    {
        //        connection.Open();
        //    }
        //    SqlCommand cmd;
        //    cmd = (SqlCommand)connection.CreateCommand();
        //    cmd.Transaction = (SqlTransaction)tran;
        //    cmd.CommandText = "spCreatePayment";
        //    cmd.CommandType = CommandType.StoredProcedure;

        //    cmd.AddParameter("@TransactionID", SqlDbType.BigInt, transactionID);

        //    cmd.AddParameter("@PaymentAmount", SqlDbType.Money, ConvertNullObjectToDBNull(payment.PayAmount));
        //    cmd.AddParameter("@V_Currency", SqlDbType.BigInt, payment.V_Currency.HasValue && payment.V_Currency.Value > 0 ? payment.V_Currency : ConvertNullObjectToDBNull(payment.Currency.LookupID));
        //    cmd.AddParameter("@V_PaymentType", SqlDbType.BigInt, payment.V_PaymentType.HasValue && payment.V_PaymentType.Value > 0 ? payment.V_PaymentType : ConvertNullObjectToDBNull(payment.PaymentType.LookupID));
        //    cmd.AddParameter("@V_PaymentMode", SqlDbType.BigInt, payment.V_PaymentMode.HasValue && payment.V_PaymentMode.Value > 0 ? payment.V_PaymentMode : ConvertNullObjectToDBNull(payment.PaymentMode.LookupID));
        //    cmd.AddParameter("@CreditOrDebit", SqlDbType.SmallInt, ConvertNullObjectToDBNull(payment.CreditOrDebit));
        //    cmd.AddParameter("@PaymentReason", SqlDbType.NVarChar, ConvertNullObjectToDBNull(payment.PaymentReason));
        //    cmd.AddParameter("@PtPmtAccID", SqlDbType.BigInt, ConvertNullObjectToDBNull(payment.PtPmtAccID));
        //    cmd.AddParameter("@PtPmtID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);

        //    int retVal = ExecuteNonQuery(cmd);

        //    PaymentID = (long)cmd.Parameters["@PtPmtID"].Value;
        //    return true;
        //}

        public override bool ProcessPayment_New(string ListIDOutTranDetails, PatientTransactionPayment payment, long transactionID, out long PaymentID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                return ProcessPayment_New(ListIDOutTranDetails, payment, transactionID, out PaymentID, cn, null);
            }
        }
        public override bool ProcessPayment_New(string ListIDOutTranDetails, PatientTransactionPayment payment, long transactionID, out long PaymentID, DbConnection connection, DbTransaction tran)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            SqlCommand cmd;
            cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spCreatePayment_New";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@TransactionID", SqlDbType.BigInt, transactionID);

            cmd.AddParameter("@PaymentAmount", SqlDbType.Money, ConvertNullObjectToDBNull(payment.PayAmount));
            cmd.AddParameter("@ReceiptNumber", SqlDbType.VarChar, ConvertNullObjectToDBNull(new ServiceSequenceNumberProvider().GetReceiptNumber_NgoaiTru((int)AllLookupValues.PatientFindBy.NGOAITRU)));
            cmd.AddParameter("@V_Currency", SqlDbType.BigInt, payment.V_Currency.HasValue && payment.V_Currency.Value > 0 ? payment.V_Currency : ConvertNullObjectToDBNull(payment.Currency.LookupID));
            cmd.AddParameter("@V_PaymentType", SqlDbType.BigInt, payment.V_PaymentType.HasValue && payment.V_PaymentType.Value > 0 ? payment.V_PaymentType : ConvertNullObjectToDBNull(payment.PaymentType.LookupID));
            cmd.AddParameter("@V_PaymentMode", SqlDbType.BigInt, payment.V_PaymentMode.HasValue && payment.V_PaymentMode.Value > 0 ? payment.V_PaymentMode : ConvertNullObjectToDBNull(payment.PaymentMode.LookupID));
            cmd.AddParameter("@CreditOrDebit", SqlDbType.SmallInt, ConvertNullObjectToDBNull(payment.CreditOrDebit));
            cmd.AddParameter("@TranPaymtNote", SqlDbType.NVarChar, ConvertNullObjectToDBNull(payment.TranPaymtNote));
            cmd.AddParameter("@PtPmtAccID", SqlDbType.BigInt, ConvertNullObjectToDBNull(payment.PtPmtAccID));
            cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(payment.StaffID));
            cmd.AddParameter("@InsertedIDList", SqlDbType.Xml, ConvertNullObjectToDBNull(ListIDOutTranDetails));
            cmd.AddParameter("@PtTranPaymtID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);

            int retVal = ExecuteNonQuery(cmd);

            PaymentID = (long)cmd.Parameters["@PtTranPaymtID"].Value;
            return true;
        }

        //
        public override bool FinializeBillingInv_CreateTranPaymt_ConsolidateTranDetailsAndAdvPaymt(bool bCreateNewTran, PatientTransaction newTranInfo, List<InPatientBillingInvoice> colBillingInvoices,
                    DateTime paidTime, List<PatientTransactionDetail> balancedTranDetails, long PtRegistrationID, long V_RegistrationType, string ListIDOutTranDetails, PatientTransactionPayment payment, long transactionID, out long NewTransactionIDRet)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    return FinializeBillingInv_CreateTranPaymt_ConsolidateTranDetailsAndAdvPaymt(bCreateNewTran, newTranInfo, colBillingInvoices,
                                paidTime, balancedTranDetails, PtRegistrationID, V_RegistrationType, ListIDOutTranDetails, payment, transactionID, out NewTransactionIDRet, cn, null);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public override bool FinializeBillingInv_CreateTranPaymt_ConsolidateTranDetailsAndAdvPaymt(bool bCreateNewTran, PatientTransaction newTranInfo, List<InPatientBillingInvoice> colBillingInvoices,
                    DateTime paidTime, List<PatientTransactionDetail> balancedTranDetails, long PtRegistrationID, long V_RegistrationType, string ListIDOutTranDetails, PatientTransactionPayment payment, long transactionID, out long NewTransactionIDRet, DbConnection connection, DbTransaction tran)
        {
            try
            {
                NewTransactionIDRet = 0;
                payment.ReceiptNumber = new ServiceSequenceNumberProvider().GetReceiptNumber((int)AllLookupValues.V_FindPatientType.NOI_TRU);
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                SqlCommand cmd;
                cmd = (SqlCommand)connection.CreateCommand();
                cmd.Transaction = (SqlTransaction)tran;
                cmd.CommandText = "sp_FinalizeBillingInvoicePayment";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@CreateNewPtTran", SqlDbType.Bit, bCreateNewTran);

                // Step 1: Check if a new PatientTransaction record is to be created
                string strNewTranInfoXml = "";
                if (bCreateNewTran)
                {
                    strNewTranInfoXml = newTranInfo.ConvertPatientTransactionInfoToXml();
                }
                cmd.AddParameter("@NewPtTranInfoDetails", SqlDbType.Xml, strNewTranInfoXml);

                // Step 2: Create a List of IDs of the Billing Invoices to be finalized
                XDocument xmlDocument;
                string billingInvoicesString = null;
                if (colBillingInvoices != null && colBillingInvoices.Count > 0)
                {
                    xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                              new XElement("InPatientBillingInvoiceList",
                              from item in colBillingInvoices
                              select new XElement("IDs",
                              new XElement("ID", item.InPatientBillingInvID),
                              new XElement("TotalPatientPayment", item.TotalPatientPayment)
                              )));
                    billingInvoicesString = xmlDocument.ToString();
                }

                cmd.AddParameter("@BillingInvoiceIDList", SqlDbType.Xml, ConvertNullObjectToDBNull(billingInvoicesString));

                cmd.AddParameter("@TransactionDetailsList", SqlDbType.Xml, ConvertPatientTransactionDetailsToXml(balancedTranDetails).ToString());

                cmd.AddParameter("@PaidTime", SqlDbType.DateTime, paidTime);

                cmd.AddParameter("@TransactionID", SqlDbType.BigInt, transactionID);

                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(payment.StaffID));

                cmd.AddParameter("@TotalValueOfBillingInvoices", SqlDbType.Money, ConvertNullObjectToDBNull(payment.PayAdvance));

                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));

                cmd.AddParameter("@NewTransactionID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);

                int retVal = ExecuteNonQuery(cmd);

                if (cmd.Parameters["@NewTransactionID"].Value != DBNull.Value)
                {
                    NewTransactionIDRet = (long)cmd.Parameters["@NewTransactionID"].Value;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return true;
        }

        public override bool CreatePaymentAndPayAdvance(long PtRegistrationID, long V_RegistrationType, string ListIDOutTranDetails, PatientTransactionPayment payment, long transactionID, out long PaymentID, out long PtCashAdvanceID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                return CreatePaymentAndPayAdvance(PtRegistrationID, V_RegistrationType, ListIDOutTranDetails, payment, transactionID, out PaymentID, out PtCashAdvanceID, cn, null);
            }
        }
        public override bool CreatePaymentAndPayAdvance(long PtRegistrationID, long V_RegistrationType, string ListIDOutTranDetails, PatientTransactionPayment payment, long transactionID, out long PaymentID, out long PtCashAdvanceID, DbConnection connection, DbTransaction tran)
        {
            payment.ReceiptNumber = new ServiceSequenceNumberProvider().GetReceiptNumber((int)AllLookupValues.V_FindPatientType.NOI_TRU);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            SqlCommand cmd;
            cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spCreatePaymentAndPayAdvance";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@TransactionID", SqlDbType.BigInt, transactionID);

            cmd.AddParameter("@PaymentAmount", SqlDbType.Money, ConvertNullObjectToDBNull(payment.PayAmount));
            cmd.AddParameter("@V_Currency", SqlDbType.BigInt, payment.V_Currency.HasValue && payment.V_Currency.Value > 0 ? payment.V_Currency : ConvertNullObjectToDBNull(payment.Currency.LookupID));
            cmd.AddParameter("@V_PaymentType", SqlDbType.BigInt, payment.V_PaymentType.HasValue && payment.V_PaymentType.Value > 0 ? payment.V_PaymentType : ConvertNullObjectToDBNull(payment.PaymentType.LookupID));
            cmd.AddParameter("@V_PaymentMode", SqlDbType.BigInt, payment.V_PaymentMode.HasValue && payment.V_PaymentMode.Value > 0 ? payment.V_PaymentMode : ConvertNullObjectToDBNull(payment.PaymentMode.LookupID));
            cmd.AddParameter("@CreditOrDebit", SqlDbType.SmallInt, ConvertNullObjectToDBNull(payment.CreditOrDebit));
            cmd.AddParameter("@TranPaymtNote", SqlDbType.NVarChar, ConvertNullObjectToDBNull(payment.TranPaymtNote));
            cmd.AddParameter("@PtPmtAccID", SqlDbType.BigInt, ConvertNullObjectToDBNull(payment.PtPmtAccID));
            cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(payment.StaffID));
            cmd.AddParameter("@PayAdvance", SqlDbType.Money, ConvertNullObjectToDBNull(payment.PayAdvance));
            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
            cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));
            cmd.AddParameter("@ReceiptNumber", SqlDbType.VarChar, ConvertNullObjectToDBNull(payment.ReceiptNumber));
            cmd.AddParameter("@InsertedIDList", SqlDbType.Xml, ConvertNullObjectToDBNull(ListIDOutTranDetails));
            cmd.AddParameter("@PtTranPaymtID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
            cmd.AddParameter("@NewPtCashAdvanceID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);

            int retVal = ExecuteNonQuery(cmd);
            PaymentID = 0;
            if (cmd.Parameters["@PtTranPaymtID"].Value != DBNull.Value)
            {
                PaymentID = (long)cmd.Parameters["@PtTranPaymtID"].Value;
            }
            if (cmd.Parameters["@NewPtCashAdvanceID"].Value != DBNull.Value)
            {
                PtCashAdvanceID = (long)cmd.Parameters["@NewPtCashAdvanceID"].Value;
            }
            else
            {
                PtCashAdvanceID = -1;
            }
            return true;
        }


        public override bool InPatientPayForBill(PatientRegistration registration, IList<InPatientBillingInvoice> billingInvoices, decimal payAmount, long staffID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {

                    SqlCommand cmd = new SqlCommand("spInPatientPayForBill", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    XDocument xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), new XElement("Root", from item in billingInvoices select new XElement("BillingInvoiceIDList", new XElement("BillingInvoiceID", item.InPatientBillingInvID))));

                    string BillingInvoices = xmlDocument.ToString();


                    cmd.AddParameter("@PaymentAmount", SqlDbType.Money, ConvertNullObjectToDBNull(payAmount));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(staffID));
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(registration.PtRegistrationID));
                    cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(registration.V_RegistrationType));
                    cmd.AddParameter("@BillInvoiceIDList", SqlDbType.Xml, ConvertNullObjectToDBNull(BillingInvoices));

                    cn.Open();

                    int retVal = ExecuteNonQuery(cmd);

                    return retVal > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public override bool InPatientSettlement(long ptRegistrationID, long staffID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {

                    SqlCommand cmd = new SqlCommand("spInPatientSettlement", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ptRegistrationID));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(staffID));

                    cn.Open();

                    int retVal = ExecuteNonQuery(cmd);

                    return retVal > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Cap nhat lai cac chi tiet dang ky, CLS, thuoc ve 1 bill
        /// </summary>
        /// <param name="InPatientBillingInvID"></param>
        /// <param name="regDetailIds"></param>
        /// <param name="pclRequestIds"></param>
        /// <param name="outDrugInvIds"></param>
        /// <param name="connection"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        public override bool UpdateRegItemsToBill(long InPatientBillingInvID, List<long> regDetailIds, List<long> pclRequestIds, List<long> outDrugInvIds, DbConnection connection, DbTransaction tran)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            SqlCommand cmd;
            cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "sp_UpdateRegItemsToBill";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@InPatientBillingInvID", SqlDbType.BigInt, InPatientBillingInvID);

            var regItemList = new IDListOutput<long>() { Ids = regDetailIds };
            cmd.AddParameter("@RegistrationDetailList", SqlDbType.Xml, ConvertNullObjectToDBNull(SerializeToXml(regItemList)));

            var pclList = new IDListOutput<long>() { Ids = pclRequestIds };
            cmd.AddParameter("@PclRequestList", SqlDbType.Xml, ConvertNullObjectToDBNull(SerializeToXml(pclList)));

            var drugInvList = new IDListOutput<long>() { Ids = outDrugInvIds };
            cmd.AddParameter("@OutwardDrugClinicDeptInvoiceList", SqlDbType.Xml, ConvertNullObjectToDBNull(SerializeToXml(drugInvList)));

            int retVal = ExecuteNonQuery(cmd);

            return true;
        }
        public override bool CheckIfTransactionExists(long registrationID, out long TransactionID)
        {
            TransactionID = -1;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spCheckIfTransactionExists", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RegistrationID", SqlDbType.BigInt, registrationID);
                cmd.AddParameter("@TransactionID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);

                cmd.AddParameter("@RetVal", SqlDbType.Int, DBNull.Value, ParameterDirection.ReturnValue);

                cn.Open();

                ExecuteNonQuery(cmd);

                bool retVal = (int)cmd.Parameters["@RetVal"].Value > 0;
                if (retVal)
                {
                    TransactionID = (long)cmd.Parameters["@TransactionID"].Value;
                }
                return retVal;
            }
        }


        public override bool SaveOutwardDrugInvoice(OutwardDrugInvoice invoice, out long outiID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                return SaveOutwardDrugInvoice(invoice, out outiID, cn, null);
            }
        }
        public override bool SaveOutwardDrugInvoice(OutwardDrugInvoice invoice, out long outiID, DbConnection connection, DbTransaction tran)
        {
            outiID = 0;
            XDocument xml = GenerateListToXMLWithNameSpace(invoice.OutwardDrugs);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            SqlCommand cmd;
            cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spSaveOutwardDrugInvoice";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@outiID", SqlDbType.BigInt, ConvertZeroObjectToDBNull(invoice.outiID));
            cmd.AddParameter("@StoreID", SqlDbType.BigInt, ConvertZeroObjectToDBNull(invoice.SelectedStorage.StoreID));
            cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertZeroObjectToDBNull(invoice.SelectedStaff.StaffID));
            cmd.AddParameter("@OutInvID", SqlDbType.VarChar, ConvertZeroObjectToDBNull(invoice.OutInvID));
            cmd.AddParameter("@PrescriptID", SqlDbType.BigInt, ConvertZeroObjectToDBNull(invoice.PrescriptID));
            cmd.AddParameter("@IssueID", SqlDbType.BigInt, ConvertZeroObjectToDBNull(invoice.IssueID));
            cmd.AddParameter("@OutDate", SqlDbType.DateTime, invoice.OutDate);
            cmd.AddParameter("@TypID", SqlDbType.BigInt, ConvertZeroObjectToDBNull(invoice.TypID));
            cmd.AddParameter("@V_OutDrugInvStatus", SqlDbType.BigInt, ConvertZeroObjectToDBNull(invoice.V_OutDrugInvStatus));
            cmd.AddParameter("@XML", SqlDbType.Xml, xml.ToString());
            SqlParameter paramTotal = new SqlParameter("@id", SqlDbType.BigInt);
            paramTotal.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(paramTotal);

            ExecuteNonQuery(cmd);
            if (paramTotal.Value != null)
            {
                outiID = (long)paramTotal.Value;
            }
            else
            {
                outiID = -1;
            }

            return outiID > 0;
        }
        public override bool SaveChangesOnOutwardDrugInvoice(OutwardDrugInvoice invoice)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                return SaveChangesOnOutwardDrugInvoice(invoice, cn, null);
            }
        }
        public override bool SaveChangesOnOutwardDrugInvoice(OutwardDrugInvoice invoice, DbConnection connection, DbTransaction tran)
        {
            XDocument xml = GenerateListToXMLWithNameSpace(invoice.OutwardDrugs);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            SqlCommand cmd;
            cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spSaveChangesOnOutwardDrugInvoice";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@outiID", SqlDbType.BigInt, ConvertZeroObjectToDBNull(invoice.outiID));
            cmd.AddParameter("@StoreID", SqlDbType.BigInt, ConvertZeroObjectToDBNull(invoice.SelectedStorage.StoreID));
            cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertZeroObjectToDBNull(invoice.SelectedStaff.StaffID));
            cmd.AddParameter("@PrescriptID", SqlDbType.BigInt, ConvertZeroObjectToDBNull(invoice.PrescriptID));
            cmd.AddParameter("@IssueID", SqlDbType.BigInt, ConvertZeroObjectToDBNull(invoice.IssueID));
            cmd.AddParameter("@TypID", SqlDbType.BigInt, ConvertZeroObjectToDBNull(invoice.TypID));
            cmd.AddParameter("@V_OutDrugInvStatus", SqlDbType.BigInt, ConvertZeroObjectToDBNull(invoice.V_OutDrugInvStatus));
            cmd.AddParameter("@XML", SqlDbType.Xml, xml.ToString());
            SqlParameter paramTotal = new SqlParameter("@id", SqlDbType.BigInt);
            paramTotal.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(paramTotal);

            ExecuteNonQuery(cmd);

            return true;
        }

        public override long? GetActiveHisID(long HIID)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spGetActiveHisID";
                cmd.AddParameter("@HIID", SqlDbType.BigInt, HIID);

                conn.Open();
                object o = cmd.ExecuteScalar();
                if (o != null && o != DBNull.Value)
                {
                    return (long)o;
                }
                return null;
            }
        }
        public override bool RegisterPatient(PatientRegistration info, DbConnection connection, DbTransaction tran, out long PatientRegistrationID, out int SequenceNo)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            SqlCommand cmd;
            //Neu co bao hiem thi di lay hisid cua the bao hiem nay roi insert vao.
            if (info.HealthInsurance != null && info.HealthInsurance.HIID > 0)
            {
                cmd = (SqlCommand)connection.CreateCommand();
                cmd.Transaction = (SqlTransaction)tran;
                cmd.CommandText = "spGetActiveHisID";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@HIID", SqlDbType.BigInt, info.HealthInsurance.HIID);

                object o = cmd.ExecuteScalar();
                if (o != null && o != DBNull.Value)
                {
                    info.HisID = (long)o;
                }
            }
            cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spRegisterPatient";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.StaffID));
            cmd.AddParameter("@PatientID", SqlDbType.Decimal, ConvertNullObjectToDBNull(info.PatientID));
            if (info.RegTypeID.HasValue && info.RegTypeID.Value > 0)
            {
                cmd.AddParameter("@RegistrationTypeID", SqlDbType.Int, ConvertNullObjectToDBNull(info.RegTypeID));
            }
            else
            {
                if (info.RegistrationType != null)
                {
                    cmd.AddParameter("@RegistrationTypeID", SqlDbType.Int, ConvertNullObjectToDBNull(info.RegistrationType.RegTypeID));
                }
                else
                {
                    cmd.AddParameter("@RegistrationTypeID", SqlDbType.Int, DBNull.Value);
                }
            }

            cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, (long)info.V_RegistrationType);

            if (info.DeptID.HasValue && info.DeptID.Value > 0)
            {
                cmd.AddParameter("@DepartmentID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.DeptID));
            }
            else
            {
                if (info.RefDepartment != null)
                {
                    cmd.AddParameter("@DepartmentID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.RefDepartment.DeptID));
                }
                else
                {
                    cmd.AddParameter("@DepartmentID", SqlDbType.BigInt, DBNull.Value);
                }
            }
            cmd.AddParameter("@EmergRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.EmergRecID));
            if (info.PaperReferal != null)
            {
                cmd.AddParameter("@PaperReferalID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.PaperReferal.RefID));
            }
            else
            {
                cmd.AddParameter("@PaperReferalID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.PaperReferalID));
            }

            cmd.AddParameter("@PatientClassID", SqlDbType.BigInt, info.PatientClassification.PatientClassID);
            cmd.AddParameter("@ExamDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(info.ExamDate));
            cmd.AddParameter("@V_DocumentTypeOnHold", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.V_DocumentTypeOnHold));

            IEnumerable<PatientRegistrationDetail> modifiedItems = info.PatientRegistrationDetails
                                                                            .Where(item =>
                                                                            {
                                                                                return item.RecordState == RecordState.MODIFIED
                                                                                    || item.RecordState == RecordState.DETACHED
                                                                                    || item.RecordState == RecordState.ADDED;
                                                                            });

            cmd.AddParameter("@PatientRegistrationDetails", SqlDbType.Xml, info.ConvertDetailsListToXml(modifiedItems));



            cmd.AddParameter("@IsCrossRegion", SqlDbType.Bit, ConvertNullObjectToDBNull(info.IsCrossRegion));
            cmd.AddParameter("@PtInsuranceBenefit", SqlDbType.Decimal, ConvertNullObjectToDBNull(info.PtInsuranceBenefit));

            cmd.AddParameter("@HisID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.HisID));

            cmd.AddParameter("@V_RegistrationStatus", SqlDbType.BigInt, (long)info.RegistrationStatus);

            cmd.AddParameter("@PatientRegistrationID", SqlDbType.BigInt, info.PtRegistrationID, ParameterDirection.Output);
            cmd.AddParameter("@SequenceNo", SqlDbType.Int, info.SequenceNo, ParameterDirection.Output);

            int retVal = ExecuteNonQuery(cmd);

            PatientRegistrationID = (long)cmd.Parameters["@PatientRegistrationID"].Value;
            SequenceNo = (int)cmd.Parameters["@SequenceNo"].Value;
            return true;
        }

        public override bool AddDeceaseInfo(DeceasedInfo info, DbConnection conn, DbTransaction tran, out long DeceaseId)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            SqlCommand cmd;

            cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "sp_AddDeceaseInfo";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, info.PtRegistrationID);
            cmd.AddParameter("@CommonMedRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.CommonMedRecID));
            cmd.AddParameter("@DeceasedDateTime", SqlDbType.DateTime, info.DeceasedDateTime);
            cmd.AddParameter("@V_CategoryOfDecease", SqlDbType.BigInt, info.V_CategoryOfDecease);
            cmd.AddParameter("@MainReasonOfDecease", SqlDbType.NVarChar, ConvertNullObjectToDBNull(info.MainReasonOfDecease));
            cmd.AddParameter("@MainCauseOfDeceaseCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(info.MainCauseOfDeceaseCode));
            cmd.AddParameter("@IsPostMorternExam", SqlDbType.Bit, ConvertNullObjectToDBNull(info.IsPostMorternExam));
            cmd.AddParameter("@PostMortemExamDiagnosis", SqlDbType.NVarChar, ConvertNullObjectToDBNull(info.PostMortemExamDiagnosis));
            cmd.AddParameter("@PostMortemExamCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(info.PostMortemExamCode));

            cmd.AddParameter("@DSNumber", SqlDbType.BigInt, info.DSNumber, ParameterDirection.Output);

            int retVal = ExecuteNonQuery(cmd);

            DeceaseId = (long)cmd.Parameters["@DSNumber"].Value;
            return true;
        }

        public override bool UpdateDeceaseInfo(DeceasedInfo info, DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            SqlCommand cmd;

            cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "sp_UpdateDeceaseInfo";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@DeceasedDateTime", SqlDbType.DateTime, info.DeceasedDateTime);
            cmd.AddParameter("@V_CategoryOfDecease", SqlDbType.BigInt, info.V_CategoryOfDecease);
            cmd.AddParameter("@MainReasonOfDecease", SqlDbType.NVarChar, info.MainReasonOfDecease);
            cmd.AddParameter("@MainCauseOfDeceaseCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(info.MainCauseOfDeceaseCode));
            cmd.AddParameter("@IsPostMorternExam", SqlDbType.Bit, ConvertNullObjectToDBNull(info.IsPostMorternExam));
            cmd.AddParameter("@PostMortemExamDiagnosis", SqlDbType.NVarChar, ConvertNullObjectToDBNull(info.PostMortemExamDiagnosis));
            cmd.AddParameter("@PostMortemExamCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(info.PostMortemExamCode));

            cmd.AddParameter("@DSNumber", SqlDbType.BigInt, info.DSNumber);

            int retVal = ExecuteNonQuery(cmd);

            return retVal > 0;
        }

        public override bool DeleteDeceaseInfo(long deceaseId, DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            var cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "sp_DeleteDeceaseInfo";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@DSNumber", SqlDbType.BigInt, deceaseId);

            var retVal = ExecuteNonQuery(cmd);

            return retVal > 0;
        }


        public override bool CreateSuggestCashAdvance(long InPatientBillingInvID, long StaffID, out long RptPtCashAdvRemID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spAutoCreateSuggestCashAdvance", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@InPatientBillingInvID", SqlDbType.BigInt, InPatientBillingInvID);
                    cmd.AddParameter("@RptPtCashAdvRemID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, StaffID);
                    cn.Open();

                    int retVal = ExecuteNonQuery(cmd);

                    RptPtCashAdvRemID = (long)cmd.Parameters["@RptPtCashAdvRemID"].Value;
                    return retVal > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        public override bool AddInPatientBillingInvoice(InPatientBillingInvoice inv, DbConnection conn, DbTransaction tran, out long ID)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            SqlCommand cmd;

            cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "sp_AddInPatientBillingInvoice";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@BillingInvNum", SqlDbType.VarChar, inv.BillingInvNum);
            cmd.AddParameter("@StaffID", SqlDbType.BigInt, inv.StaffID);
            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, inv.PtRegistrationID);
            cmd.AddParameter("@V_BillingInvType", SqlDbType.BigInt, (long)inv.V_BillingInvType);
            cmd.AddParameter("@V_InPatientBillingInvStatus", SqlDbType.BigInt, (long)inv.V_InPatientBillingInvStatus);
            cmd.AddParameter("@InvDate", SqlDbType.DateTime, inv.InvDate);
            cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(inv.DeptID));
            cmd.AddParameter("@InPatientBillingInvID", SqlDbType.BigInt, inv.InPatientBillingInvID, ParameterDirection.Output);

            int retVal = ExecuteNonQuery(cmd);

            ID = (long)cmd.Parameters["@InPatientBillingInvID"].Value;
            return true;
        }
        public override List<PatientRegistrationDetail> GetAllRegistrationDetails(long registrationID, int FindPatient, DbConnection connection, DbTransaction tran)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            SqlCommand cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spGetAllRegistrationDetails";
            if (FindPatient == (int)AllLookupValues.V_FindPatientType.NOI_TRU)
            {
                cmd.CommandText = "spGetAllRegistrationDetails_InPt";
            }

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, registrationID);

            IDataReader reader = ExecuteReader(cmd);
            if (reader == null)
            {
                return null;
            }
            List<PatientRegistrationDetail> retVal = GetPatientRegistrationDetailsCollectionFromReader(reader);
            reader.Close();
            return retVal;
        }
        public override InPatientBillingInvoice GetInPatientBillingInvoice(long billingInvoiceID, DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            InPatientBillingInvoice inv = null;

            SqlCommand cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "sp_InPatientBillingInvoices_GetByID";

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.AddParameter("@InPatientBillingInvID", SqlDbType.BigInt, billingInvoiceID);

            IDataReader reader = ExecuteReader(cmd);
            if (reader == null)
            {
                return null;
            }
            if (reader.Read())
            {
                inv = GetInPatientBillingInvoiceFromReader(reader);
            }
            reader.Close();
            return inv;
        }

        public override List<PatientRegistrationDetail> GetAllRegistrationDetailsByBillingInvoiceID(long billingInvoiceID, DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            SqlCommand cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spGetAllRegistrationDetailsByBillingInvoiceID";
            cmd.CommandTimeout = int.MaxValue;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.AddParameter("@InPatientBillingInvID", SqlDbType.BigInt, billingInvoiceID);

            IDataReader reader = ExecuteReader(cmd);
            if (reader == null)
            {
                return null;
            }
            List<PatientRegistrationDetail> retVal = GetPatientRegistrationDetailsCollectionFromReader(reader);
            reader.Close();
            return retVal;
        }

        public override List<PatientRegistrationDetail> GetAllInPatientRegistrationDetails_NoBill(long registrationId, long? DeptID, DbConnection conn, DbTransaction tran, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            SqlCommand cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spGetAllInPatientRegDetails_NoBill";

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.AddParameter("@RegistrationID", SqlDbType.BigInt, registrationId);
            //cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, (int)AllLookupValues.RegistrationType.NOI_TRU);
            cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptID));

            IDataReader reader = ExecuteReader(cmd);
            if (reader == null)
            {
                return null;
            }
            List<PatientRegistrationDetail> retVal = GetPatientRegistrationDetailsCollectionFromReader(reader);
            reader.Close();
            return retVal;
        }
        public override PatientRegistration GetRegistration(long registrationID, int FindPatient, DbConnection connection, DbTransaction tran)
        {

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            SqlCommand cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spGetRegistration";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, registrationID);
            cmd.AddParameter("@FindPatient", SqlDbType.BigInt, FindPatient);


            IDataReader reader = ExecuteReader(cmd);
            if (reader == null || !reader.Read())
            {
                if (reader != null)
                {
                    reader.Close();
                }
                return null;
            }
            PatientRegistration reg = GetPatientRegistrationFromReader(reader);
            reg.PatientClassification = GetPatientClassificationFromReader(reader);
            reader.Close();

            HealthInsurance curHI = null;
            if (reg.HisID.HasValue && reg.HisID.Value > 0)
            {
                cmd.CommandText = "spGetHealthInsuranceByHisID";
                cmd.Parameters.Clear();
                cmd.AddParameter("@HisID", SqlDbType.BigInt, reg.HisID.Value);
                reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    if (reader.Read())
                    {
                        curHI = GetHealthInsuranceFromReader(reader);
                        InsuranceBenefit benefit = GetInsuranceBenefitFromReader(reader);
                        curHI.InsuranceBenefit = benefit;
                        curHI.HisID = reg.HisID;
                        reg.HealthInsurance = curHI;
                    }
                    reader.Close();
                }
            }

            /*▼====: #008*/
            if (reg.HisID_2.HasValue && reg.HisID_2.Value > 0)
            {
                cmd.CommandText = "spGetHealthInsuranceByHisID";
                cmd.Parameters.Clear();
                cmd.AddParameter("@HisID", SqlDbType.BigInt, reg.HisID_2.Value);
                reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    if (reader.Read())
                    {
                        HealthInsurance mHealthInsurance2 = null;
                        mHealthInsurance2 = GetHealthInsuranceFromReader(reader);
                        mHealthInsurance2.InsuranceBenefit = GetInsuranceBenefitFromReader(reader);
                        mHealthInsurance2.HisID = reg.HisID_2;
                        reg.HealthInsurance_2 = mHealthInsurance2;
                    }
                    reader.Close();
                }
            }
            if (reg.HisID_3.HasValue && reg.HisID_3.Value > 0)
            {
                cmd.CommandText = "spGetHealthInsuranceByHisID";
                cmd.Parameters.Clear();
                cmd.AddParameter("@HisID", SqlDbType.BigInt, reg.HisID_3.Value);
                reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    if (reader.Read())
                    {
                        HealthInsurance mHealthInsurance3 = null;
                        mHealthInsurance3 = GetHealthInsuranceFromReader(reader);
                        mHealthInsurance3.InsuranceBenefit = GetInsuranceBenefitFromReader(reader);
                        mHealthInsurance3.HisID = reg.HisID_3;
                        reg.HealthInsurance_3 = mHealthInsurance3;
                    }
                    reader.Close();
                }
            }
            /*▲====: #008*/

            if (reg.PaperReferalID.HasValue && reg.PaperReferalID.Value > 0)
            {
                cmd.CommandText = "spGetPaperReferalByID";
                cmd.Parameters.Clear();
                cmd.AddParameter("@PaperReferalID", SqlDbType.BigInt, reg.PaperReferalID.Value);
                reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    if (reader.Read())
                    {
                        reg.PaperReferal = GetPaperReferalFromReader(reader);
                    }
                    reader.Close();
                }
            }
            if (reg.PaperReferralID_2.HasValue && reg.PaperReferralID_2.Value > 0)
            {
                cmd.CommandText = "spGetPaperReferalByID";
                cmd.Parameters.Clear();
                cmd.AddParameter("@PaperReferalID", SqlDbType.BigInt, reg.PaperReferralID_2.Value);
                reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    if (reader.Read())
                    {
                        reg.PaperReferral_2 = GetPaperReferalFromReader(reader);
                    }
                    reader.Close();
                }
            }
            if (reg.PaperReferralID_3.HasValue && reg.PaperReferralID_3.Value > 0)
            {
                cmd.CommandText = "spGetPaperReferalByID";
                cmd.Parameters.Clear();
                cmd.AddParameter("@PaperReferalID", SqlDbType.BigInt, reg.PaperReferralID_3.Value);
                reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    if (reader.Read())
                    {
                        reg.PaperReferral_3 = GetPaperReferalFromReader(reader);
                    }
                    reader.Close();
                }
            }

            cmd.CommandText = "spGetTransactionByRegistrationID";
            cmd.Parameters.Clear();
            cmd.AddParameter("@RegistrationID", SqlDbType.BigInt, registrationID);
            long V_RegistrationType = 0;
            if (FindPatient == (int)AllLookupValues.V_FindPatientType.NOI_TRU)
            {
                V_RegistrationType = (long)AllLookupValues.RegistrationType.NOI_TRU;
            }
            else if (FindPatient == (int)AllLookupValues.V_FindPatientType.NGOAI_TRU)
            {
                V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU;
            }
            cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, V_RegistrationType);


            reader = ExecuteReader(cmd);
            if (reader != null)
            {
                if (reader.Read())
                {
                    reg.PatientTransaction = GetPatientTransactionFromReader(reader);
                }
                reader.Close();
            }

            return reg;
        }

        public override PatientRegistration GetRegistrationSimple(long registrationID, int PatientFindBy)
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }
                    SqlCommand cmd = (SqlCommand)connection.CreateCommand();
                    cmd.CommandText = "spGetRegistrationSimple";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@RegistrationID", SqlDbType.BigInt, registrationID);
                    cmd.AddParameter("@PatientFindBy", SqlDbType.BigInt, PatientFindBy);

                    IDataReader reader = ExecuteReader(cmd);
                    if (reader == null || !reader.Read())
                    {
                        if (reader != null)
                        {
                            reader.Close();
                        }
                        return null;
                    }
                    PatientRegistration reg = GetPatientRegistrationFromReader(reader);
                    if (reg.HisID.HasValue && reg.HisID.Value > 0)
                    {
                        reg.HealthInsurance = GetHealthInsuranceFromReader(reader);
                    }
                    reader.Close();

                    return reg;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public override TransferForm CreateBlankTransferFormByRegID(long PtRegistrationID, int PatientFindBy, long V_TransferFormType)
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }
                    SqlCommand cmd = (SqlCommand)connection.CreateCommand();
                    cmd.CommandText = "spGetRegistrationToTransfer";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, PtRegistrationID);
                    cmd.AddParameter("@PatientFindBy", SqlDbType.BigInt, PatientFindBy);
                    cmd.AddParameter("@V_TransferFormType", SqlDbType.BigInt, V_TransferFormType);

                    IDataReader reader = ExecuteReader(cmd);
                    if (reader == null || !reader.Read())
                    {
                        if (reader != null)
                        {
                            reader.Close();
                        }
                        return null;
                    }
                    TransferForm result = new TransferForm();
                    result.CurPatientRegistration = GetPatientRegistrationFromReader(reader);
                    if (result.CurPatientRegistration.HisID.HasValue && result.CurPatientRegistration.HisID.Value > 0)
                    {
                        result.CurPatientRegistration.HealthInsurance = GetHealthInsuranceFromReader(reader);
                    }
                    if (result.CurPatientRegistration != null && result.CurPatientRegistration.PtRegistrationID > 0)
                    {
                        result.LastDiagnosisTreatment = GetDiagTrmtFromReader(reader);
                        result.ICD10Main = GetDiagnosisIcd10ItemsFromReader(reader);
                    }
                    reader.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public override PatientTransaction GetTransactionByID(long TransactionID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetTransactionByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@TransactionID", SqlDbType.BigInt, TransactionID);
                IDataReader reader = ExecuteReader(cmd);
                if (reader != null && reader.Read())
                {
                    return GetPatientTransactionFromReader(reader);
                }
            }
            return null;
        }
        public override PatientTransaction GetTransactionByRegistrationID(long RegistrationID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetTransactionByRegistrationID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@RegistrationID", SqlDbType.BigInt, RegistrationID);
                IDataReader reader = ExecuteReader(cmd);
                if (reader != null && reader.Read())
                {
                    return GetPatientTransactionFromReader(reader);
                }
            }
            return null;
        }
        public override List<OutwardDrugInvoice> GetDrugInvoiceListByRegistrationID(long registrationID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                return GetDrugInvoiceListByRegistrationID(registrationID, cn, null);
            }
        }
        public override List<OutwardDrugInvoice> GetDrugInvoiceListByRegistrationID(long registrationID, DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            SqlCommand cmd;
            cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spOutwardDrugALL_ByPtRegistrationID";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@RegistrationID", SqlDbType.BigInt, registrationID);

            IDataReader reader = cmd.ExecuteReader();

            var retVal = new List<OutwardDrugInvoice>();
            OutwardDrugInvoice tempInvoice = null;
            if (reader != null)
            {
                int idx;
                while (reader.Read())
                {
                    tempInvoice = GetOutWardDrugInvoiceFromReader(reader);
                    idx = retVal.IndexOf(tempInvoice);
                    if (idx < 0)//Chua co invoice trong list
                    {
                        tempInvoice.OutwardDrugs = new List<OutwardDrug>().ToObservableCollection();
                        retVal.Add(tempInvoice);
                    }
                    else
                    {
                        tempInvoice = retVal[idx];
                    }
                    var tempDrug = GetOutWardDrugDetailFromReader(reader);
                    tempDrug.OutwardDrugInvoice = tempInvoice;
                    tempDrug.CreatedDate = tempInvoice.OutDate;
                    tempInvoice.OutwardDrugs.Add(tempDrug);
                }
                reader.Close();
            }
            //Doc them phieu tra thuoc.
            //==== #002
            //foreach (var invoice in retVal)
            var HIStoreID = Globals.AxServerSettings.CommonItems.StoreIDForHIPrescription;
            foreach (var invoice in retVal.Where(x => x.StoreID != HIStoreID))
            //==== #002
            {
                cmd.CommandText = "spOutwardDrugInvoice_GetReturnInvoice";
                invoice.ReturnedInvoices = new ObservableCollection<OutwardDrugInvoice>();

                cmd.Parameters.Clear();
                cmd.AddParameter("@OutiID", SqlDbType.BigInt, invoice.outiID);

                reader = cmd.ExecuteReader();
                int idx;
                while (reader.Read())
                {
                    tempInvoice = GetOutWardDrugInvoiceFromReader(reader);
                    idx = invoice.ReturnedInvoices.IndexOf(tempInvoice);
                    if (idx < 0)//Chua co invoice trong list
                    {
                        tempInvoice.OutwardDrugs = new List<OutwardDrug>().ToObservableCollection();
                        invoice.ReturnedInvoices.Add(tempInvoice);
                    }
                    else
                    {
                        tempInvoice = invoice.ReturnedInvoices[idx];
                    }
                    var tempDrug = GetOutWardDrugDetailFromReader(reader);
                    tempDrug.OutwardDrugInvoice = tempInvoice;
                    tempDrug.CreatedDate = tempInvoice.OutDate;
                    tempInvoice.OutwardDrugs.Add(tempDrug);
                }
                reader.Close();
            }
            //==== #002
            if (Globals.AxServerSettings.CommonItems.EnableHIStore)
            {
                foreach (var invoice in retVal.Where(x => x.StoreID == HIStoreID))
                {
                    cmd.CommandText = "spOutwardDrugInvoice_GetReturnInvoice";
                    invoice.ReturnedInvoices = new ObservableCollection<OutwardDrugInvoice>();
                    cmd.Parameters.Clear();
                    cmd.AddParameter("@OutiID", SqlDbType.BigInt, invoice.outiID);
                    cmd.AddParameter("@HI", SqlDbType.Bit, true);
                    reader = cmd.ExecuteReader();
                    int idx;
                    while (reader.Read())
                    {
                        tempInvoice = GetOutWardDrugInvoiceFromReader(reader);
                        idx = invoice.ReturnedInvoices.IndexOf(tempInvoice);
                        if (idx < 0)//Chua co invoice trong list
                        {
                            tempInvoice.OutwardDrugs = new List<OutwardDrug>().ToObservableCollection();
                            invoice.ReturnedInvoices.Add(tempInvoice);
                        }
                        else
                        {
                            tempInvoice = invoice.ReturnedInvoices[idx];
                        }
                        var tempDrug = GetOutWardDrugDetailFromReader(reader);
                        tempDrug.OutwardDrugInvoice = tempInvoice;
                        tempDrug.CreatedDate = tempInvoice.OutDate;
                        tempInvoice.OutwardDrugs.Add(tempDrug);
                    }
                    reader.Close();
                }
            }
            //==== #002
            return retVal;
        }

        public override OutwardDrugInvoice GetDrugInvoiceByID(long outiID, DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            SqlCommand cmd;
            cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spGetOutwardDrugInvoiceByID";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@OutiID", SqlDbType.BigInt, outiID);

            IDataReader reader = cmd.ExecuteReader();

            OutwardDrugInvoice tempInvoice = null;
            if (reader.Read())
            {
                tempInvoice = GetOutWardDrugInvoiceFromReader(reader);
                var tempDrug = GetOutWardDrugDetailFromReader(reader);
                tempInvoice.OutwardDrugs = new ObservableCollection<OutwardDrug>();
                tempDrug.OutwardDrugInvoice = tempInvoice;
                tempInvoice.OutwardDrugs.Add(tempDrug);
                while (reader.Read())
                {
                    tempDrug = GetOutWardDrugDetailFromReader(reader);
                    tempDrug.OutwardDrugInvoice = tempInvoice;
                    tempInvoice.OutwardDrugs.Add(tempDrug);
                }
            }

            reader.Close();

            return tempInvoice;
        }
        private List<PatientPCLRequest> FillPclRequestList(IDataReader reader)
        {
            List<PatientPCLRequest> allRequests = new List<PatientPCLRequest>();
            try
            {
                PatientPCLRequest curRequest;
                int idx;
                while (reader.Read())
                {
                    curRequest = GetPatientPCLRequestFromReader(reader);
                    idx = allRequests.IndexOf(curRequest);
                    if (idx < 0)//Chua co invoice trong list
                    {
                        curRequest.PatientPCLRequestIndicators = new List<PatientPCLRequestDetail>().ToObservableCollection();
                        allRequests.Add(curRequest);
                    }
                    else
                    {
                        curRequest = allRequests[idx];
                    }
                    curRequest.PatientPCLRequestIndicators.Add(GetPatientPCLRequestDetailsFromReader(reader, curRequest.StaffID));
                }
            }
            catch
            {

            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
            return allRequests;
        }

        private List<PatientPCLRequest_Ext> FillPclRequestExtList(IDataReader reader)
        {
            List<PatientPCLRequest_Ext> allRequests = new List<PatientPCLRequest_Ext>();
            try
            {
                PatientPCLRequest_Ext curRequest;
                int idx;
                while (reader.Read())
                {
                    curRequest = GetPatientPCLRequestExtFromReader(reader);
                    //idx = allRequests.FindIndex(delegate(PatientPCLRequest_Ext item)
                    //{
                    //    return item.ToString().Equals(curRequest.ToString(),StringComparison.Ordinal);
                    //});
                    idx = allRequests.FindIndex(item => item.ToString().Equals(curRequest.ToString(), StringComparison.Ordinal));

                    if (idx < 0)//Chua co invoice trong list
                    {
                        curRequest.PatientPCLRequestIndicatorsExt = new List<PatientPCLRequestDetail_Ext>().ToObservableCollection();
                        allRequests.Add(curRequest);
                    }
                    else
                    {
                        curRequest = allRequests[idx];
                    }
                    curRequest.PatientPCLRequestIndicatorsExt.Add(GetPatientPCLRequestDetailsExtFromReader(reader, curRequest.StaffID));
                }
            }
            catch
            {

            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
            return allRequests;
        }

        public override List<PatientPCLRequest> GetPCLRequestListByRegistrationID(long RegistrationID, DbConnection connection, DbTransaction tran)
        {
            SqlCommand cmd;
            cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spGetPCLRequestByRegistrationID";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.AddParameter("@RegistrationID", SqlDbType.BigInt, RegistrationID);

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            IDataReader reader = ExecuteReader(cmd);
            return FillPclRequestList(reader);
        }

        public override List<PatientPCLRequest> GetPCLRequestListByBillingInvoiceID(long billingInvoiceID, DbConnection connection, DbTransaction tran)
        {
            SqlCommand cmd;
            cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spGetPCLRequestByBillingInvoiceID";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = int.MaxValue;

            cmd.AddParameter("@InPatientBillingInvID", SqlDbType.BigInt, billingInvoiceID);
            cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt,
                (long)AllLookupValues.RegistrationType.NOI_TRU);

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            IDataReader reader = ExecuteReader(cmd);
            return FillPclRequestList(reader);
        }
        public override List<PatientPCLRequest> spGetInPatientPCLRequest_NoBill(long registrationId, long? DeptID, DbConnection connection, DbTransaction tran, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            SqlCommand cmd;
            cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spGetInPatientPCLRequest_NoBill";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.AddParameter("@RegistrationID", SqlDbType.BigInt, registrationId);
            cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, (int)AllLookupValues.RegistrationType.NOI_TRU);
            cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptID));
            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(FromDate));
            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(ToDate));
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            IDataReader reader = ExecuteReader(cmd);
            return FillPclRequestList(reader);
        }
        public override List<PatientPCLRequest> GetPCLRequestListByIDList(List<long> PclIDList, DbConnection connection, DbTransaction tran, long V_RegistrationType)
        {
            SqlCommand cmd;
            cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spGetPCLRequestByIdList";
            cmd.CommandType = CommandType.StoredProcedure;
            IDListOutput<long> idList = new IDListOutput<long>() { Ids = PclIDList };
            cmd.AddParameter("@PclRequestIDList", SqlDbType.Xml, ConvertNullObjectToDBNull(SerializeToXml<IDListOutput<long>>(idList)));
            cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            IDataReader reader = ExecuteReader(cmd);
            return FillPclRequestList(reader);
        }
        public override List<PatientPCLRequest> GetPCLRequestListByIDList(List<long> PclIDList, long V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                return GetPCLRequestListByIDList(PclIDList, conn, null, V_RegistrationType);
            }
        }
        public override List<PatientPCLRequest> GetPCLRequestListByRegistrationID(long RegistrationID)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                return GetPCLRequestListByRegistrationID(RegistrationID, conn, null);
            }
        }
        public override List<PatientPCLRequest> GetPCLRequestListByRegistrationID_InPt(long InPtRegistrationID)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd;
                cmd = (SqlCommand)conn.CreateCommand();
                /* HPT 06/09/2016: Viết hàm mới để get danh sách phiếu chỉ định cận lâm sàng nội trú nhưng dùng chung stored với ngoại trú, truyền thêm giá trị cho tham số V_RegistrationType để phân biệt
                 * (V_RegistrationType mặc định là ngoại trú)*/
                cmd.CommandText = "spGetPCLRequestByRegistrationID";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@RegistrationID", SqlDbType.BigInt, InPtRegistrationID);
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, AllLookupValues.RegistrationType.NOI_TRU);
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                IDataReader reader = ExecuteReader(cmd);
                return FillPclRequestList(reader);
            }
        }
        public override List<PCLItem> GetPCLItemsByMedServiceID(long MedicalServiceID, DbConnection connection, DbTransaction tran)
        {
            SqlCommand cmd;
            cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spGetDefaultPCLItemsByMedServiceID";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, MedicalServiceID);

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            List<PCLItem> allItems = null;
            IDataReader reader = ExecuteReader(cmd);
            if (reader != null)
            {
                try
                {
                    allItems = GetPCLItemCollectionFromReader(reader);
                }
                finally
                {
                    reader.Close();
                }
            }
            return allItems;
        }
        public override List<PCLItem> GetPCLItemsByMedServiceID(long MedicalServiceID)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                return GetPCLItemsByMedServiceID(MedicalServiceID, conn, null);
            }
        }

        public override List<PCLExamType> GetPclExamTypesByMedServiceID(long MedicalServiceID, DbConnection connection, DbTransaction tran)
        {
            SqlCommand cmd;
            cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spGetDefaultPCLExamTypesByMedServiceID";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, MedicalServiceID);

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            List<PCLExamType> allItems = null;
            IDataReader reader = ExecuteReader(cmd);
            if (reader != null)
            {
                try
                {
                    allItems = GetPCLExamTypeColectionFromReader(reader);
                }
                finally
                {
                    reader.Close();
                }
            }
            return allItems;
        }
        public override List<PCLExamType> GetPclExamTypesByMedServiceID(long MedicalServiceID)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                return GetPclExamTypesByMedServiceID(MedicalServiceID, conn, null);
            }
        }
        public override PCLExamType GetPclExamTypeByID(long ExamTypeID)
        {
            PCLExamType examType = null;
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd;
                cmd = (SqlCommand)conn.CreateCommand();
                conn.Open();
                cmd.CommandText = "spPCLExamTypeID";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@ExamTypeID", SqlDbType.BigInt, ExamTypeID);

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    if (reader.Read())
                    {
                        examType = GetPCLExamTypeFromReader(reader);
                    }
                    reader.Close();
                }
            }
            return examType;
        }

        public override bool AddNewRegistration(PatientRegistration info, out long PatientRegistrationID
            , out List<long> newRegistrationDetailIdList, out List<long> newPclRequestIdList)
        {
            return AddNewRegistration(info, out PatientRegistrationID, out newRegistrationDetailIdList, out newPclRequestIdList, null);
        }

        public override bool AddNewRegistration(PatientRegistration info, out long PatientRegistrationID
            , out List<long> newRegistrationDetailIdList, out List<long> newPclRequestIdList
            , IList<DiagnosisIcd10Items> Icd10Items)
        {
            try
            {
                using (var conn = new SqlConnection(this.ConnectionString))
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spGetMedicalRecordID";
                    cmd.AddParameter("@PatientID", SqlDbType.Decimal, ConvertNullObjectToDBNull(info.PatientID));
                    object obj = cmd.ExecuteScalar();

                    long PatientMedicalRecordID = -1;
                    long? PatientMedicalFileID;
                    if (!CheckIfMedicalRecordExists(info.PatientID.Value, out PatientMedicalRecordID, out PatientMedicalFileID))
                    {
                        //Tao moi PatientMedicalRecord.
                        var medicalRecord = new PatientMedicalRecord();
                        medicalRecord.PatientID = info.PatientID;
                        medicalRecord.NationalMedicalCode = "MedicalCode";
                        if (info.ExamDate == DateTime.MinValue)
                        {
                            medicalRecord.CreatedDate = DateTime.Now;
                        }
                        else
                        {
                            medicalRecord.CreatedDate = info.ExamDate;
                        }

                        bool bCreateMrOk = AddNewPatientMedicalRecord(medicalRecord, out PatientMedicalRecordID, conn, null);
                        if (!bCreateMrOk)
                        {
                            throw new Exception(string.Format("{0}.",eHCMSResources.Z1780_G1_CannotCreatePatient));
                        }
                    }
                    cmd.CommandText = "sp_CreateNewRegistrationWithDetails";
                    if (info.HealthInsurance != null)
                        cmd.AddParameter("@CheckedHICardValidResult", SqlDbType.NVarChar, ConvertNullObjectToDBNull(info.HealthInsurance.CheckedHICardValidResult));

                    // TxD 25/01/2015: Added EmergInPtReExamination bit flag to indicate that an Emergency admitted InPt come back for a reexamination in an Outpt registration
                    //                  This flag is for Outpt ONLY
                    if (info.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
                    {
                        cmd.AddParameter("@EmergInPtReExamination", SqlDbType.Bit, ConvertNullObjectToDBNull(info.EmergInPtReExamination));
                        if (info.PromoDiscountProgramObj != null)
                        {
                            cmd.AddParameter("@PromoDiscountProgram", SqlDbType.Xml, ConvertNullObjectToDBNull(info.PromoDiscountProgramObj.ConvertToXml()));
                        }
                        cmd.AddParameter("@FiveYearsAppliedDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(info.FiveYearsAppliedDate));
                    }

                    //Txd 10/10/2014: Added the following condition for InPatient Registration
                    if (info.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU || info.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU_BHYT
                        || info.V_RegistrationType == AllLookupValues.RegistrationType.CAP_CUU || info.V_RegistrationType == AllLookupValues.RegistrationType.CAP_CUU_BHYT)
                    {
                        cmd.CommandText = "sp_CreateNewRegistrationWithDetails_InPt";
                        cmd.AddParameter("@IsForeigner", SqlDbType.Bit, ConvertNullObjectToDBNull(info.IsForeigner));

                        // TxD 28/03/2015: For now only Add this Parameter for InPt registration
                        cmd.AddParameter("@V_RegForPatientOfType", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.V_RegForPatientOfType));

                        /*▼====: #006*/
                        cmd.AddParameter("@ConsultingDiagnosysID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.Patient.ConsultingDiagnosysID));
                        /*▲====: #006*/
                    }

                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.StaffID));
                    cmd.AddParameter("@HIApprovedStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.HIApprovedStaffID != null ? info.HIApprovedStaffID : info.StaffID));

                    if (info.RegTypeID.HasValue && info.RegTypeID.Value > 0)
                    {
                        cmd.AddParameter("@RegistrationTypeID", SqlDbType.Int, ConvertNullObjectToDBNull(info.RegTypeID));
                    }
                    else
                    {
                        if (info.RegistrationType != null)
                        {
                            cmd.AddParameter("@RegistrationTypeID", SqlDbType.Int, ConvertNullObjectToDBNull(info.RegistrationType.RegTypeID));
                        }
                        else
                        {
                            cmd.AddParameter("@RegistrationTypeID", SqlDbType.Int, DBNull.Value);
                        }
                    }
                    if (info.DeptID.HasValue && info.DeptID.Value > 0)
                    {
                        cmd.AddParameter("@DepartmentID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.DeptID));
                    }
                    else
                    {
                        if (info.RefDepartment != null)
                        {
                            cmd.AddParameter("@DepartmentID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.RefDepartment.DeptID));
                        }
                        else
                        {
                            cmd.AddParameter("@DepartmentID", SqlDbType.BigInt, DBNull.Value);
                        }
                    }
                    cmd.AddParameter("@EmergRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.EmergRecID));
                    if (info.PaperReferal != null)
                    {
                        cmd.AddParameter("@PaperReferalID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.PaperReferal.RefID));
                    }
                    else
                    {
                        cmd.AddParameter("@PaperReferalID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.PaperReferalID));
                    }

                    cmd.AddParameter("@PatientClassID", SqlDbType.BigInt, info.PatientClassification.PatientClassID);
                    cmd.AddParameter("@ExamDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(info.ExamDate));
                    cmd.AddParameter("@V_DocumentTypeOnHold", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.V_DocumentTypeOnHold));

                    cmd.AddParameter("@IsCrossRegion", SqlDbType.Bit, ConvertNullObjectToDBNull(info.IsCrossRegion));
                    //HPT 20/08/2015 BEGIN: Thêm Parameter cho Stored Procedure sp_CreateNewRegistrationWithDetails để lưu giá trị kiểm tra điều kiện thẻ BH 5 năm
                    //Parameter IsHICard_FiveYearsCont dùng gán giá trị cho cột IsHICard_FiveYearsCont chỉ có trong bảng PatientRegistration (Ngoại trú)
                    cmd.AddParameter("@IsHICard_FiveYearsCont", SqlDbType.Bit, ConvertNullObjectToDBNull(info.IsHICard_FiveYearsCont));
                    /*==== #004 ====*/
                    cmd.AddParameter("@IsHICard_FiveYearsCont_NoPaid", SqlDbType.Bit, ConvertNullObjectToDBNull(info.IsHICard_FiveYearsCont_NoPaid));
                    /*==== #004 ====*/
                    cmd.AddParameter("@IsChildUnder6YearsOld", SqlDbType.Bit, ConvertNullObjectToDBNull(info.IsChildUnder6YearsOld));
                    //HPT 20/08/2015 END
                    cmd.AddParameter("@PtInsuranceBenefit", SqlDbType.Decimal, ConvertNullObjectToDBNull(info.PtInsuranceBenefit));
                    //HPT 11/01/2017: Thông tuyến
                    cmd.AddParameter("@IsAllowCrossRegion", SqlDbType.Bit, ConvertNullObjectToDBNull(info.IsAllowCrossRegion));
                    if (info.HisID.HasValue)
                    {
                        cmd.AddParameter("@HisID", SqlDbType.BigInt, info.HisID.Value);
                    }
                    else
                    {
                        cmd.AddParameter("@HisID", SqlDbType.BigInt, DBNull.Value);
                    }

                    cmd.AddParameter("@ProgSumMinusMinHI", SqlDbType.Decimal, ConvertNullObjectToDBNull(info.ProgSumMinusMinHI));
                    cmd.AddParameter("@SequenceNo", SqlDbType.Int, info.SequenceNo);
                    cmd.AddParameter("@PtRegistrationCode", SqlDbType.VarChar, info.PtRegistrationCode);
                    cmd.AddParameter("@HIComment", SqlDbType.NVarChar, info.HIComment);
                    cmd.AddParameter("@V_RegistrationStatus", SqlDbType.BigInt, (long)info.RegistrationStatus);
                    cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, (long)info.V_RegistrationType);
                    cmd.AddParameter("@PatientRegistrationID", SqlDbType.BigInt, info.PtRegistrationID, ParameterDirection.Output);
                    
                    //▼====== #016
                    cmd.AddParameter("@BasicDiagTreatment", SqlDbType.NVarChar, info.BasicDiagTreatment);
                    //▲====== #016
                    
                    string detailListString = null;
                    if (info.PatientRegistrationDetails != null && info.PatientRegistrationDetails.Count > 0)
                    {
                        foreach (var item in info.PatientRegistrationDetails)
                        {
                            item.StaffID = info.StaffID;
                        }

                        detailListString = ConvertPatientRegistrationDetailsToXml(info.PatientRegistrationDetails).ToString();
                    }
                    string pclRequestListString = null;
                    if (info.PCLRequests != null && info.PCLRequests.Count > 0)
                    {
                        foreach (var item in info.PCLRequests)
                        {
                            item.StaffID = info.StaffID;
                            item.PatientServiceRecord.StaffID = info.StaffID;
                        }

                        pclRequestListString = ConvertPCLRequestsToXml(info.PCLRequests, PatientMedicalRecordID).ToString();
                    }

                    cmd.AddParameter("@RegistrationDetails_Add", SqlDbType.Xml, ConvertNullObjectToDBNull(detailListString));
                    cmd.AddParameter("@PclRequestList_Add", SqlDbType.Xml, ConvertNullObjectToDBNull(pclRequestListString));

                    cmd.AddParameter("@NewRegistrationDetailIDList", SqlDbType.Xml, DBNull.Value, ParameterDirection.Output);
                    cmd.AddParameter("@NewPclRequestListIDList", SqlDbType.Xml, DBNull.Value, ParameterDirection.Output);
                    cmd.AddParameter("@ReturnValue", SqlDbType.Xml, DBNull.Value, ParameterDirection.ReturnValue);

                    //KMx: Thêm cột FromAppointmentID để biết đăng ký được tạo ra từ cuộc hẹn nào (10/11/2014 10:56).
                    if (info.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
                    {
                        cmd.AddParameter("@FromAppointmentID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.AppointmentID));
                    }

                    if (info.DiagnosisTreatment != null && Icd10Items != null && info.PtRegistrationTransferID.GetValueOrDefault(0) > 0)
                    {
                        cmd.AddParameter("@Diagnosis", SqlDbType.NVarChar, ConvertNullObjectToDBNull(info.DiagnosisTreatment.Diagnosis));
                        cmd.AddParameter("@DiagnosisFinal", SqlDbType.NVarChar, ConvertNullObjectToDBNull(info.DiagnosisTreatment.DiagnosisFinal));
                        cmd.AddParameter("@OrientedTreatment", SqlDbType.NVarChar, ConvertNullObjectToDBNull(info.DiagnosisTreatment.OrientedTreatment));
                        cmd.AddParameter("@Treatment", SqlDbType.NVarChar, ConvertNullObjectToDBNull(info.DiagnosisTreatment.Treatment));
                        cmd.AddParameter("@DoctorComments", SqlDbType.NVarChar, ConvertNullObjectToDBNull(info.DiagnosisTreatment.DoctorComments));
                        cmd.AddParameter("@ICD10XML", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertDiagnosisIcd10ItemsToXML(Icd10Items)));
                        cmd.AddParameter("@ICD10List", SqlDbType.NChar, ConvertNullObjectToDBNull(info.DiagnosisTreatment.ICD10List));
                        cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.DiagnosisTreatment.DeptLocationID));
                        cmd.AddParameter("@PtRegistrationTransferID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.PtRegistrationTransferID));
                    }
                    try
                    {
                        ExecuteNonQuery(cmd);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message.ToString());
                    }

                    PatientRegistrationID = (long)cmd.Parameters["@PatientRegistrationID"].Value;

                    var idList = cmd.Parameters["@NewRegistrationDetailIDList"].Value as string;
                    var IDListOutput = DeserializeFromXml<IDListOutput<long>>(idList);
                    newRegistrationDetailIdList = IDListOutput != null ? IDListOutput.Ids : null;

                    idList = cmd.Parameters["@NewPclRequestListIDList"].Value as string;
                    IDListOutput = DeserializeFromXml<IDListOutput<long>>(idList);
                    newPclRequestIdList = IDListOutput != null ? IDListOutput.Ids : null;

                    object retVal = cmd.Parameters["@ReturnValue"].Value;
                    if (retVal != null && retVal != DBNull.Value)
                    {
                        return (int)retVal == 1;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        public override bool AddBedPatientRegDetail(BedPatientRegDetail bedPatientDetail, out long bedPatientDetailId, DbConnection connection, DbTransaction tran)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            var cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sp_AddBedPatientRegDetail";

            cmd.AddParameter("@PtRegDetailID", SqlDbType.BigInt, bedPatientDetail.PtRegDetailID);
            cmd.AddParameter("@BedPatientID", SqlDbType.BigInt, bedPatientDetail.BedPatientID);
            cmd.AddParameter("@RecCreatedDate", SqlDbType.DateTime, bedPatientDetail.RecCreatedDate);
            cmd.AddParameter("@BillFromDate", SqlDbType.DateTime, bedPatientDetail.BillFromDate);
            cmd.AddParameter("@BillToDate", SqlDbType.DateTime, bedPatientDetail.BillToDate);
            cmd.AddParameter("@IsDeleted", SqlDbType.Bit, bedPatientDetail.IsDeleted);

            cmd.AddParameter("@PtBedAllocRegDetailID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);

            int retVal = ExecuteNonQuery(cmd);

            bedPatientDetailId = (long)cmd.Parameters["@PtBedAllocRegDetailID"].Value;
            return retVal > 0;
        }

        public override bool AddNewRegistrationDetails(PatientRegistrationDetail regDetails, out long RegDetailsID, DbConnection connection, DbTransaction tran)
        {
            long patientRegistrationID = -1;
            if (regDetails.PatientRegistration != null)
            {
                patientRegistrationID = regDetails.PatientRegistration.PtRegistrationID;
            }
            else
            {
                patientRegistrationID = regDetails.PtRegistrationID.GetValueOrDefault(-1);
            }
            if (patientRegistrationID < 0)
            {
                RegDetailsID = 0;
                return false;
            }

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            SqlCommand cmd = (SqlCommand)connection.CreateCommand();
            cmd.CommandTimeout = int.MaxValue;
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sp_AddRegistrationDetails";

            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, patientRegistrationID);
            cmd.AddParameter("@FindPatient", SqlDbType.BigInt, regDetails.PatientRegistration.FindPatient);

            cmd.AddParameter("@HisID", SqlDbType.BigInt, ConvertNullObjectToDBNull(regDetails.HisID));
            long? deptLocId = null;
            if (regDetails.DeptLocation != null)
            {
                deptLocId = regDetails.DeptLocation.DeptLocationID;
            }
            else
            {
                deptLocId = regDetails.DeptLocID;
            }
            cmd.AddParameter("@DeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(deptLocId));
            cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(regDetails.StaffID));
            cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, regDetails.RefMedicalServiceItem.MedServiceID);


            cmd.AddParameter("@Price", SqlDbType.Money, regDetails.InvoicePrice);
            cmd.AddParameter("@HIAllowedPrice", SqlDbType.Money, ConvertNullObjectToDBNull(regDetails.HIAllowedPrice));
            cmd.AddParameter("@ServiceQty", SqlDbType.Float, regDetails.Qty);
            cmd.AddParameter("@CreatedDate", SqlDbType.DateTime, regDetails.CreatedDate);
            cmd.AddParameter("@V_ExamRegStatus", SqlDbType.BigInt, (long)regDetails.ExamRegStatus);
            cmd.AddParameter("@HIBenefit", SqlDbType.Decimal, ConvertNullObjectToDBNull(regDetails.HIBenefit));
            cmd.AddParameter("@PaidTime", SqlDbType.DateTime, ConvertNullObjectToDBNull(regDetails.PaidTime));
            cmd.AddParameter("@Amount", SqlDbType.Money, regDetails.TotalInvoicePrice);
            cmd.AddParameter("@AmountCoPay", SqlDbType.Money, regDetails.TotalCoPayment);
            cmd.AddParameter("@PriceDifference", SqlDbType.Money, regDetails.TotalPriceDifference);
            cmd.AddParameter("@TotalHIPayment", SqlDbType.Money, regDetails.TotalHIPayment);
            cmd.AddParameter("@SpecialNote", SqlDbType.NVarChar, ConvertNullObjectToDBNull(regDetails.SpecialNote));
            cmd.AddParameter("@InPatientBillingInvID", SqlDbType.BigInt, ConvertNullObjectToDBNull(regDetails.InPatientBillingInvID));

            cmd.AddParameter("@PtRegistrationDetailsID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);

            int retVal = ExecuteNonQuery(cmd);

            RegDetailsID = (long)cmd.Parameters["@PtRegistrationDetailsID"].Value;
            return retVal > 0;
        }
        public override bool AddNewRegistrationDetails(PatientRegistrationDetail regDetails, out long RegDetailsID)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                return AddNewRegistrationDetails(regDetails, out RegDetailsID, conn, null);
            }
        }

        public override bool AddNewPCLRequest(PatientPCLRequest info, out long PCLRequestID, DbConnection connection, DbTransaction tran)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            SqlCommand cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spAddNewPCLRequest";

            long? serviceRecID = null;
            if (info.PatientServiceRecord != null && info.PatientServiceRecord.ServiceRecID > 0)
            {
                serviceRecID = info.PatientServiceRecord.ServiceRecID;
            }
            else
            {
                serviceRecID = info.ServiceRecID;
            }

            cmd.AddParameter("@ServiceRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(serviceRecID));

            cmd.AddParameter("@Diagnosis", SqlDbType.NVarChar, info.Diagnosis);
            cmd.AddParameter("@DoctorComments", SqlDbType.NVarChar, ConvertNullObjectToDBNull(info.DoctorComments));
            cmd.AddParameter("@IsExternalExam", SqlDbType.Bit, ConvertNullObjectToDBNull(info.IsExternalExam));
            cmd.AddParameter("@IsImported", SqlDbType.Bit, ConvertNullObjectToDBNull(info.IsImported));
            cmd.AddParameter("@IsCaseOfEmergency", SqlDbType.Bit, ConvertNullObjectToDBNull(info.IsCaseOfEmergency));

            cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.StaffID));
            cmd.AddParameter("@V_PCLRequestType", SqlDbType.BigInt, (long)info.V_PCLRequestType);
            cmd.AddParameter("@V_PCLRequestStatus", SqlDbType.BigInt, (long)info.V_PCLRequestStatus);

            cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);

            int retVal = ExecuteNonQuery(cmd);


            PCLRequestID = (long)cmd.Parameters["@PatientPCLReqID"].Value;
            return true;
        }
        public override bool AddNewPCLRequest(PatientPCLRequest info, out long PCLRequestID)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                return AddNewPCLRequest(info, out PCLRequestID, conn, null);
            }
        }

        //public override bool UpdatePCLRequest(PatientPCLRequest info, DbConnection connection, DbTransaction tran)
        //{
        //    if (connection.State != ConnectionState.Open)
        //    {
        //        connection.Open();
        //    }

        //    SqlCommand cmd = (SqlCommand)connection.CreateCommand();
        //    cmd.Transaction = (SqlTransaction)tran;
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.CommandText = "spUpdatePCLRequest";

        //    cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, info.PatientPCLReqID);
        //    cmd.AddParameter("@Diagnosis", SqlDbType.NVarChar, info.Diagnosis);
        //    cmd.AddParameter("@DoctorComments", SqlDbType.NVarChar, ConvertNullObjectToDBNull(info.DoctorComments));
        //    cmd.AddParameter("@IsExternalExam", SqlDbType.Bit, ConvertNullObjectToDBNull(info.IsExternalExam));
        //    cmd.AddParameter("@IsImported", SqlDbType.Bit, ConvertNullObjectToDBNull(info.IsImported));
        //    cmd.AddParameter("@IsCaseOfEmergency", SqlDbType.Bit, ConvertNullObjectToDBNull(info.IsCaseOfEmergency));
        //    if (info.RecordState == RecordState.DELETED)
        //    {
        //        info.MarkedAsDeleted = true;
        //    }
        //    else
        //    {
        //        info.MarkedAsDeleted = false;
        //    }
        //    cmd.AddParameter("@MarkedAsDeleted", SqlDbType.Bit, info.MarkedAsDeleted);
        //    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.StaffID));

        //    int retVal = ExecuteNonQuery(cmd);
        //    return true;
        //}
        //public override bool UpdatePCLRequest(PatientPCLRequest info)
        //{
        //    using (SqlConnection conn = new SqlConnection(this.ConnectionString))
        //    {
        //        return UpdatePCLRequest(info, conn, null);
        //    }
        //}

        public override bool AddNewPCLRequestDetails(long PCLRequestID, PatientPCLRequestDetail PCLRequestDetails, out long PCLRequestDetailsID, DbConnection connection, DbTransaction tran)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            SqlCommand cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spAddNewPCLRequestDetails";

            cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, PCLRequestDetails.PCLExamType.PCLExamTypeID);
            cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, PCLRequestDetails.PatientPCLRequest.PatientPCLReqID);
            cmd.AddParameter("@NumberOfTest", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLRequestDetails.Qty));
            cmd.AddParameter("@Price", SqlDbType.Decimal, PCLRequestDetails.InvoicePrice);
            cmd.AddParameter("@HIAllowedPrice", SqlDbType.Decimal, ConvertNullObjectToDBNull(PCLRequestDetails.HIAllowedPrice));
            cmd.AddParameter("@Amount", SqlDbType.Decimal, PCLRequestDetails.TotalInvoicePrice);
            cmd.AddParameter("@AmountCoPay", SqlDbType.Decimal, PCLRequestDetails.TotalCoPayment);
            cmd.AddParameter("@PriceDifference", SqlDbType.Decimal, PCLRequestDetails.TotalPriceDifference);
            cmd.AddParameter("@TotalHIPayment", SqlDbType.Decimal, PCLRequestDetails.TotalHIPayment);
            cmd.AddParameter("@V_ExamRegStatus", SqlDbType.BigInt, (long)PCLRequestDetails.ExamRegStatus);
            cmd.AddParameter("@PaidTime", SqlDbType.DateTime, PCLRequestDetails.PaidTime);

            if (PCLRequestDetails.CreatedDate == DateTime.MinValue)
            {
                PCLRequestDetails.CreatedDate = DateTime.Now;
            }
            cmd.AddParameter("@CreatedDate", SqlDbType.DateTime, PCLRequestDetails.CreatedDate);

            cmd.AddParameter("@PCLReqItemID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);

            int retVal = ExecuteNonQuery(cmd);


            PCLRequestDetailsID = (long)cmd.Parameters["@PCLReqItemID"].Value;
            return true;
        }
        public override bool AddNewPCLRequestDetails(long PCLRequestID, PatientPCLRequestDetail PCLRequestDetails, out long PCLRequestDetailsID)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                return AddNewPCLRequestDetails(PCLRequestID, PCLRequestDetails, out PCLRequestDetailsID, conn, null);
            }
        }

        public override bool UpdatePCLRequestDetails(PatientPCLRequestDetail PCLRequestDetails, DbConnection connection, DbTransaction tran)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            SqlCommand cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spUpdatePCLRequestDetails";

            cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, PCLRequestDetails.PCLExamType.PCLExamTypeID);
            cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, PCLRequestDetails.PatientPCLRequest.PatientPCLReqID);
            cmd.AddParameter("@PtRegDetailID", SqlDbType.BigInt, DBNull.Value);
            cmd.AddParameter("@NumberOfTest", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLRequestDetails.Qty));
            cmd.AddParameter("@Price", SqlDbType.Decimal, PCLRequestDetails.InvoicePrice);
            cmd.AddParameter("@HIAllowedPrice", SqlDbType.Decimal, ConvertNullObjectToDBNull(PCLRequestDetails.HIAllowedPrice));
            cmd.AddParameter("@Amount", SqlDbType.Decimal, PCLRequestDetails.TotalInvoicePrice);
            cmd.AddParameter("@AmountCoPay", SqlDbType.Decimal, PCLRequestDetails.TotalCoPayment);
            cmd.AddParameter("@PriceDifference", SqlDbType.Decimal, PCLRequestDetails.TotalPriceDifference);
            cmd.AddParameter("@TotalHIPayment", SqlDbType.Decimal, PCLRequestDetails.TotalHIPayment);
            cmd.AddParameter("@HIBenefit", SqlDbType.Decimal, ConvertNullObjectToDBNull(PCLRequestDetails.HIBenefit));
            cmd.AddParameter("@V_ExamRegStatus", SqlDbType.BigInt, (long)PCLRequestDetails.ExamRegStatus);

            if (PCLRequestDetails.RecordState == RecordState.DELETED)
            {
                PCLRequestDetails.MarkedAsDeleted = true;
            }
            else
            {
                PCLRequestDetails.MarkedAsDeleted = false;
            }
            cmd.AddParameter("@MarkedAsDeleted", SqlDbType.Bit, PCLRequestDetails.MarkedAsDeleted);
            cmd.AddParameter("@PCLReqItemID", SqlDbType.BigInt, PCLRequestDetails.PCLReqItemID);

            int retVal = ExecuteNonQuery(cmd);

            return true;
        }
        public override bool UpdatePCLRequestDetails(PatientPCLRequestDetail PCLRequestDetails)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                return UpdatePCLRequestDetails(PCLRequestDetails, conn, null);
            }
        }

        //Không thấy hàm nào sử dụng.
        public override void UpdatePaidTime(List<PatientRegistrationDetail> paidRegDetailsList,
                                        List<PatientPCLRequest> paidPclRequestList,
                                        List<OutwardDrugInvoice> paidDrugInvoice,
                                        List<InPatientBillingInvoice> billingInvoiceList, DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            XDocument xmlDocument;
            string regDetailsString = null;
            if (paidRegDetailsList != null && paidRegDetailsList.Count > 0)
            {
                //regDetailsString = string.Join(",", from p in paidRegDetailsList select p.PtRegDetailID);    
                xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                           new XElement("RegDetailsIDList",
                           from item in paidRegDetailsList
                           select new XElement("IDs",
                           new XElement("ID", item.PtRegDetailID),
                           new XElement("PaidTime", item.PaidTime.HasValue ? item.PaidTime.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : ""),
                           new XElement("RefundTime", item.RefundTime.HasValue ? item.RefundTime.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : "")
                           )));
                regDetailsString = xmlDocument.ToString();
            }

            string pclRequestDetailsString = null;
            if (paidPclRequestList != null && paidPclRequestList.Count > 0)
            {
                xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement("PatientPCLRequests",
                from item in paidPclRequestList
                select new XElement("PCLRequest",
                new XElement("ID", item.PatientPCLReqID),
                new XElement("PaidTime", item.PaidTime.HasValue ? item.PaidTime.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : ""),
                new XElement("RefundTime", item.RefundTime.HasValue ? item.RefundTime.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : ""),
                new XElement("PCLRequestDetails",
                from inner in item.PatientPCLRequestIndicators
                select new XElement("Details",
                    new XElement("ID", inner.PCLReqItemID),
                    new XElement("PaidTime", inner.PaidTime.HasValue ? inner.PaidTime.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : ""),
                    new XElement("RefundTime", inner.RefundTime.HasValue ? inner.RefundTime.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : ""))
                ))));
                pclRequestDetailsString = xmlDocument.ToString();

            }
            string paidDrugInvoiceString = null;
            if (paidDrugInvoice != null && paidDrugInvoice.Count > 0)
            {
                xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                           new XElement("OutInvoiceIDList",
                           from item in paidDrugInvoice
                           select new XElement("OutInvoice",
                           new XElement("ID", item.outiID),
                           new XElement("PaidTime", item.PaidTime.HasValue ? item.PaidTime.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : ""),
                           new XElement("RefundTime", item.RefundTime.HasValue ? item.RefundTime.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : ""))));
                paidDrugInvoiceString = xmlDocument.ToString();

            }
            string billingInvoicesString = null;
            if (billingInvoiceList != null && billingInvoiceList.Count > 0)
            {
                xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                          new XElement("InPatientBillingInvoiceList",
                          from item in billingInvoiceList
                          select new XElement("IDs",
                          new XElement("ID", item.InPatientBillingInvID),
                          new XElement("PaidTime", item.PaidTime.HasValue ? item.PaidTime.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : ""),
                          new XElement("RefundTime", item.RefundTime.HasValue ? item.RefundTime.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : "")
                          )));
                billingInvoicesString = xmlDocument.ToString();
            }

            SqlCommand cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spUpdatePaidTime";

            cmd.AddParameter("@PatientRegistrationDetailsList", SqlDbType.Xml, ConvertNullObjectToDBNull(regDetailsString));
            cmd.AddParameter("@PatientPclRequestDetailsList", SqlDbType.Xml, ConvertNullObjectToDBNull(pclRequestDetailsString));
            cmd.AddParameter("@OutwardDrugInvoiceList", SqlDbType.Xml, ConvertNullObjectToDBNull(paidDrugInvoiceString));
            cmd.AddParameter("@BillingInvoiceList", SqlDbType.Xml, ConvertNullObjectToDBNull(billingInvoicesString));

            int retVal = ExecuteNonQuery(cmd);
        }

        public override void UpdatePaidTimeForBillingInvoice(List<InPatientBillingInvoice> billingInvoiceList, DateTime paidTime, DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            XDocument xmlDocument;
            string billingInvoicesString = null;
            if (billingInvoiceList != null && billingInvoiceList.Count > 0)
            {
                xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                          new XElement("InPatientBillingInvoiceList",
                          from item in billingInvoiceList
                          select new XElement("IDs",
                          new XElement("ID", item.InPatientBillingInvID),
                          new XElement("TotalPatientPayment", item.TotalPatientPayment)
                          )));
                billingInvoicesString = xmlDocument.ToString();
            }

            var cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sp_UpdatePaidTimeForBillingInvoice";

            cmd.AddParameter("@BillingInvoiceIDList", SqlDbType.Xml, ConvertNullObjectToDBNull(billingInvoicesString));
            cmd.AddParameter("@PaidTime", SqlDbType.DateTime, paidTime);

            int retVal = ExecuteNonQuery(cmd);
        }

        public override void RemovePaidRegItems(List<PatientRegistrationDetail> paidRegDetailsList,
                                        List<PatientPCLRequest> paidPclRequestList,
                                        List<OutwardDrugInvoice> paidDrugInvoice,
                                        List<OutwardDrugClinicDeptInvoice> paidMedItemList,
                                        List<OutwardDrugClinicDeptInvoice> paidChemicalItemList, DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            XDocument xmlDocument;
            string regDetailsString = null;
            if (paidRegDetailsList != null && paidRegDetailsList.Count > 0)
            {
                xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                           new XElement("RegDetailsIDList",
                           from item in paidRegDetailsList
                           select new XElement("IDs",
                           new XElement("ID", item.PtRegDetailID),
                           new XElement("V_ExamRegStatus", (int)item.ExamRegStatus),
                           new XElement("RefundTime", item.RefundTime.HasValue ? item.RefundTime.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : "")
                           )));
                regDetailsString = xmlDocument.ToString();
            }

            string pclRequestString = null;
            if (paidPclRequestList != null && paidPclRequestList.Count > 0)
            {
                xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement("PatientPCLRequests",
                from item in paidPclRequestList
                where item.RecordState != RecordState.UNCHANGED
                select new XElement("PCLRequest",
                new XElement("ID", item.PatientPCLReqID),
                new XElement("V_PCLRequestStatus", (long)item.V_PCLRequestStatus),
                new XElement("RefundTime", item.RefundTime.HasValue ? item.RefundTime.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : ""),
                new XElement("PCLRequestDetails",
                from inner in item.PatientPCLRequestIndicators
                where inner.RecordState != RecordState.UNCHANGED
                select new XElement("Details",
                    new XElement("ID", inner.PCLReqItemID),
                    new XElement("V_ExamRegStatus", (int)inner.ExamRegStatus),
                    new XElement("RefundTime", inner.RefundTime.HasValue ? inner.RefundTime.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : ""))
                ))));
                pclRequestString = xmlDocument.ToString();

            }
            string paidDrugInvoiceString = null;
            if (paidDrugInvoice != null && paidDrugInvoice.Count > 0)
            {
                xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                           new XElement("OutInvoiceIDList",
                           from item in paidDrugInvoice
                           select new XElement("IDs",
                           new XElement("ID", item.outiID))));
                paidDrugInvoiceString = xmlDocument.ToString();

            }
            string paidMedItemListString = null;
            if (paidMedItemList != null && paidMedItemList.Count > 0)
            {

            }
            string paidChemicalItemListString = null;
            if (paidChemicalItemList != null && paidChemicalItemList.Count > 0)
            {

            }

            SqlCommand cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spRemovePaidRegItems";

            cmd.AddParameter("@PatientRegistrationDetailsList", SqlDbType.Xml, ConvertNullObjectToDBNull(regDetailsString));
            cmd.AddParameter("@PatientPclRequestList", SqlDbType.Xml, ConvertNullObjectToDBNull(pclRequestString));
            cmd.AddParameter("@OutwardDrugInvoiceList", SqlDbType.Xml, ConvertNullObjectToDBNull(paidDrugInvoiceString));
            cmd.AddParameter("@MedicalItemInvoiceList", SqlDbType.Xml, ConvertNullObjectToDBNull(paidMedItemListString));
            cmd.AddParameter("@ChemicalItemInvoiceList", SqlDbType.Xml, ConvertNullObjectToDBNull(paidChemicalItemListString));

            int retVal = ExecuteNonQuery(cmd);
        }

        public override bool AddNewPatientMedicalRecord(PatientMedicalRecord info, out long PatientMedicalRecordID, DbConnection connection, DbTransaction tran)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            SqlCommand cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spAddNewPatientMedicalRecord";

            cmd.AddParameter("@PatientID", SqlDbType.BigInt, info.PatientID);

            cmd.AddParameter("@NationalMedicalCode", SqlDbType.Char, info.NationalMedicalCode);
            cmd.AddParameter("@PatientRecBarCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(info.PatientRecBarCode));
            cmd.AddParameter("@CreatedDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(info.CreatedDate));
            cmd.AddParameter("@FinishedDate", SqlDbType.NVarChar, ConvertNullObjectToDBNull(info.FinishedDate));

            cmd.AddParameter("@PatientRecID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);

            int retVal = ExecuteNonQuery(cmd);

            PatientMedicalRecordID = (long)cmd.Parameters["@PatientRecID"].Value;
            return true;
        }
        public override bool AddNewPatientMedicalRecord(PatientMedicalRecord info, out long PatientMedicalRecordID)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                return AddNewPatientMedicalRecord(info, out PatientMedicalRecordID, conn, null);
            }
        }

        public override bool AddNewPatientServiceRecord(long PatientMedicalRecordID, long? PatientMedicalFileID
            , PatientServiceRecord PtServiceRecord, out long PatientServiceRecordID,
            DbConnection connection, DbTransaction tran)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            SqlCommand cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spAddNewPatientServiceRecord";

            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtServiceRecord.PtRegistrationID));
            cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtServiceRecord.StaffID));
            cmd.AddParameter("@PatientRecID", SqlDbType.BigInt, PatientMedicalRecordID);
            cmd.AddParameter("@PatientMedicalFileID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientMedicalFileID));
            //cmd.AddParameter("@ExamDate", SqlDbType.DateTime, DateTime.Now);
            cmd.AddParameter("@ExamDate", SqlDbType.DateTime, PtServiceRecord.ExamDate);

            cmd.AddParameter("@V_ProcessingType", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtServiceRecord.V_ProcessingType));
            cmd.AddParameter("@V_Behaving", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtServiceRecord.V_Behaving));
            cmd.AddParameter("@DateModified", SqlDbType.DateTime, DBNull.Value);
            cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull((long)PtServiceRecord.V_RegistrationType));
            cmd.AddParameter("@ServiceRecID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);

            int retVal = ExecuteNonQuery(cmd);

            PatientServiceRecordID = (long)cmd.Parameters["@ServiceRecID"].Value;
            return true;
        }
        public override bool AddNewPatientServiceRecord(long PatientMedicalRecordID, long? PatientMedicalFileID,
            PatientServiceRecord PtServiceRecord, out long PatientServiceRecordID)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                return AddNewPatientServiceRecord(PatientMedicalRecordID, PatientMedicalFileID, PtServiceRecord, out PatientServiceRecordID, conn, null);
            }
        }

        public override bool CheckIfMedicalRecordExists(long PatientID, out long PatientMedicalRecordID, out long? PatientMedicalFileID
            , DbConnection connection, DbTransaction tran)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            SqlCommand cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spGetMedicalRecordID";

            cmd.AddParameter("@PatientID", SqlDbType.BigInt, PatientID);

            IDataReader reader = ExecuteReader(cmd);
            try
            {
                reader.Read();
                PatientMedicalRecordID = (long)reader["PatientRecID"];
                PatientMedicalFileID = reader["PatientMedicalFileID"] as long?;
                if (PatientMedicalRecordID < 0)
                {
                    PatientMedicalRecordID = -1;
                    PatientMedicalFileID = null;
                    reader.Close();
                    return false;
                }
                reader.Close();
                return true;
            }
            catch
            {
                PatientMedicalRecordID = -1;
                PatientMedicalFileID = null;
                reader.Close();
                return false;
            }
        }
        public override bool CheckIfMedicalRecordExists(long PatientID, out long PatientMedicalRecordID, out long? PatientMedicalFileID)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                return CheckIfMedicalRecordExists(PatientID, out PatientMedicalRecordID, out PatientMedicalFileID, conn, null);
            }
        }

        public override PatientRegistration GetPatientRegistrationByPtRegistrationID(long PtRegistrationID)
        {

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientRegistration_ByPtRegistrationID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, PtRegistrationID);
                cn.Open();
                PatientRegistration retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetPatientRegistrationFromReader(reader);
                }
                reader.Close();
                return retVal;
            }
        }

        public override long CheckRegistrationStatus(long PtRegistrationID)
        {

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spCheckExistRegistration", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, PtRegistrationID);
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    return (long)reader["V_RegistrationStatus"];
                }
                return 0;
            }
        }

        private string CheckPatientStringForSearch(string inputString)
        {
            string result = inputString;
            Regex regStr = new Regex(@"\d{6,8}");
            if (regStr.IsMatch(inputString))
            {
                result = "hs:" + result;
            }
            return result;
        }

        public override List<PatientRegistration> SearchRegistrations(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            totalCount = 0;
            if (criteria == null)
            {
                return null;
            }

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSearchRegistrations", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@FullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.FullName));
                cmd.AddParameter("@HealthInsuranceCard", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.HICard));
                cmd.AddParameter("@PatientCode", SqlDbType.Char, ConvertNullObjectToDBNull(criteria.PatientCode));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.StaffID));
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                cmd.AddParameter("@V_RegistrationStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.RegStatus));

                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptID));
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptLocationID));
                cmd.AddParameter("@PatientFindBy", SqlDbType.BigInt, ConvertNullObjectToDBNull((long)criteria.PatientFindBy));
                cmd.AddParameter("@KhamBenh", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.KhamBenh));
                cmd.AddParameter("@IsHoanTat", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.IsHoanTat));
                cmd.AddParameter("@IsAdmission", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.IsAdmission));
                cmd.AddParameter("@TypeSearch", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.TypeSearch));
                cmd.AddParameter("@IsCancel", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.IsCancel));

                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(pageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(pageIndex));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, ConvertNullObjectToDBNull(bCountTotal));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.OrderBy));
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);
                if (!string.IsNullOrEmpty(criteria.PatientNameString) && criteria.PatientNameString.Length > 3 && new string[] { "PH", "PL", "QH", "QL" }.Contains(criteria.PatientNameString.Substring(0, 2))
                    && char.IsDigit(criteria.PatientNameString[3]))
                {
                    cmd.AddParameter("@PCLRequestNumID", SqlDbType.VarChar, ConvertNullObjectToDBNull(criteria.PatientNameString));
                }

                cn.Open();
                List<PatientRegistration> retVal = null;

                IDataReader reader = ExecuteReader(cmd);

                retVal = GetPatientRegistrationCollectionFromReader(reader);
                reader.Close();

                if (bCountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    totalCount = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    totalCount = -1;

                return retVal;
            }
        }

        public override List<PatientRegistration> SearchRegistrationsInPt(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, bool bCalledFromSearchInPtPopup, out int totalCount)
        {
            totalCount = 0;
            if (criteria == null)
            {
                return null;
            }

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSearchRegistrations_InPt", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@FullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.FullName));
                cmd.AddParameter("@HealthInsuranceCard", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.HICard));
                cmd.AddParameter("@PatientCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(criteria.PatientCode));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.StaffID));
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                cmd.AddParameter("@V_RegistrationStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.RegStatus));

                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptID));
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptLocationID));
                cmd.AddParameter("@IsAdmission", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.IsAdmission));
                cmd.AddParameter("@IsDischarge", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.IsDischarge));
                cmd.AddParameter("@TypeSearch", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.TypeSearch));
                cmd.AddParameter("@IsCancel", SqlDbType.Int, ConvertNullObjectToDBNull(criteria.IsCancel));

                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(pageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(pageIndex));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, ConvertNullObjectToDBNull(bCountTotal));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.OrderBy));
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);
                cmd.AddParameter("@CalledFromSearchInPtPopup", SqlDbType.Bit, ConvertNullObjectToDBNull(bCalledFromSearchInPtPopup));

                cmd.AddParameter("@SearchByVregForPtOfType", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.SearchByVregForPtOfType));
                cmd.AddParameter("@PtRegistrationCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(criteria.PtRegistrationCode));
                cmd.AddParameter("@IsSearchByRegistrationDetails", SqlDbType.VarChar, ConvertNullObjectToDBNull(criteria.IsSearchByRegistrationDetails));
                cn.Open();
                List<PatientRegistration> retVal = null;

                IDataReader reader = ExecuteReader(cmd);

                retVal = GetPatientRegistrationCollectionFromReader(reader);
                reader.Close();

                if (bCountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    totalCount = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    totalCount = -1;

                return retVal;
            }
        }

        public override List<PatientRegistration> SearchRegisPrescription(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            totalCount = 0;
            if (criteria == null)
            {
                return null;
            }

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSearchRegisPrescription", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@FullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.FullName));
                cmd.AddParameter("@HealthInsuranceCard", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.HICard));
                cmd.AddParameter("@PatientCode", SqlDbType.Char, ConvertNullObjectToDBNull(criteria.PatientCode));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.StaffID));
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                cmd.AddParameter("@V_RegistrationStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.RegStatus));

                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptID));
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptLocationID));
                cmd.AddParameter("@PatientFindBy", SqlDbType.BigInt, ConvertNullObjectToDBNull((long)criteria.PatientFindBy));
                cmd.AddParameter("@KhamBenh", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.KhamBenh));
                cmd.AddParameter("@IsHoanTat", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.IsHoanTat));
                cmd.AddParameter("@IsAdmission", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.IsAdmission));
                cmd.AddParameter("@IsAppointment", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.IsAppointment));

                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(pageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(pageIndex));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, ConvertNullObjectToDBNull(bCountTotal));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.OrderBy));
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<PatientRegistration> retVal = null;

                IDataReader reader = ExecuteReader(cmd);

                //retVal = GetPatientRegistrationCollectionFromReader(reader);
                retVal = GetPatientRegistrationCollectionFromReader(reader);
                reader.Close();

                if (bCountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    totalCount = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    totalCount = -1;

                return retVal;
            }
        }
        public override List<PatientRegistrationDetail> SearchRegistrationListForOST(long DeptID, long DeptLocID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetRegistrationListByExamStatus", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptID));
                cmd.AddParameter("@DeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocID));

                cn.Open();
                List<PatientRegistrationDetail> retVal = null;

                IDataReader reader = ExecuteReader(cmd);

                //retVal = GetPatientRegistrationCollectionFromReader(reader);
                retVal = GetPatientRegistrationDetailsCollectionFromReader(reader);
                reader.Close();

                return retVal;
            }
        }

        public override List<PatientRegistrationDetail> SearchRegisDetailPrescription(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            totalCount = 0;
            if (criteria == null)
            {
                return null;
            }

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSearchRegisPrescription", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@FullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.FullName));
                cmd.AddParameter("@HealthInsuranceCard", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.HICard));
                cmd.AddParameter("@PatientCode", SqlDbType.Char, ConvertNullObjectToDBNull(criteria.PatientCode));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.StaffID));
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                cmd.AddParameter("@V_RegistrationStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.RegStatus));

                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptID));
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptLocationID));
                cmd.AddParameter("@PatientFindBy", SqlDbType.BigInt, ConvertNullObjectToDBNull((long)criteria.PatientFindBy));
                cmd.AddParameter("@KhamBenh", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.KhamBenh));
                cmd.AddParameter("@IsHoanTat", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.IsHoanTat));
                cmd.AddParameter("@IsAdmission", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.IsAdmission));
                cmd.AddParameter("@IsAppointment", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.IsAppointment));

                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(pageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(pageIndex));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, ConvertNullObjectToDBNull(bCountTotal));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.OrderBy));
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<PatientRegistrationDetail> retVal = null;

                IDataReader reader = ExecuteReader(cmd);

                //retVal = GetPatientRegistrationCollectionFromReader(reader);
                retVal = GetPatientRegistrationDetailsCollectionFromReader(reader);
                reader.Close();

                if (bCountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    totalCount = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    totalCount = -1;

                return retVal;
            }
        }

        public override List<PatientRegistration> SearchRegistrationsDiagTrmt(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            totalCount = 0;
            if (criteria == null)
            {
                return null;
            }

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSearchRegistrationsDiagTrmt", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@FullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.FullName));
                cmd.AddParameter("@HealthInsuranceCard", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.HICard));
                cmd.AddParameter("@PatientCode", SqlDbType.Char, ConvertNullObjectToDBNull(criteria.PatientCode));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.StaffID));
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                cmd.AddParameter("@V_RegistrationStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.RegStatus));

                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptID));
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptLocationID));
                cmd.AddParameter("@PatientFindBy", SqlDbType.BigInt, ConvertNullObjectToDBNull((long)criteria.PatientFindBy));

                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(pageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(pageIndex));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, ConvertNullObjectToDBNull(bCountTotal));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.OrderBy));
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<PatientRegistration> retVal = null;

                IDataReader reader = ExecuteReader(cmd);

                retVal = GetPatientRegistrationCollectionFromReader(reader);
                reader.Close();

                if (bCountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    totalCount = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    totalCount = -1;

                return retVal;
            }
        }


        #region Outstanding Task

        //KMx: Anh Tuấn không cho insert vào bảng PatientQueue.
        //public override bool PatientQueue_Insert(PatientQueue ObjPatientQueue, DbConnection connection, DbTransaction tran)
        //{
        //    try
        //    {
        //        if (connection.State != ConnectionState.Open)
        //        {
        //            connection.Open();
        //        }

        //        SqlCommand cmd = (SqlCommand)connection.CreateCommand();
        //        cmd.Transaction = (SqlTransaction)tran;
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        //cmd.CommandText = "spPatientQueue_Insert";
        //        cmd.CommandText = "spPatientQueue_InsertToQueue";

        //        cmd.AddParameter("@PatientAppointmentID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ObjPatientQueue.PatientAppointmentID));
        //        cmd.AddParameter("@RegistrationID", SqlDbType.BigInt, ObjPatientQueue.RegistrationID);
        //        cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ObjPatientQueue.PatientPCLReqID));
        //        cmd.AddParameter("@RegistrationDetailsID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ObjPatientQueue.RegistrationDetailsID));
        //        cmd.AddParameter("@PrescriptionIssueID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ObjPatientQueue.PrescriptionIssueID));
        //        cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ObjPatientQueue.MedServiceID));
        //        cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ObjPatientQueue.PatientID));
        //        cmd.AddParameter("@SequenceNo", SqlDbType.Int, ConvertNullObjectToDBNull(ObjPatientQueue.SequenceNo));
        //        cmd.AddParameter("@FullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ObjPatientQueue.FullName));
        //        cmd.AddParameter("@DeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ObjPatientQueue.DeptLocID));
        //        cmd.AddParameter("@V_QueueType", SqlDbType.BigInt, ConvertNullObjectToDBNull(ObjPatientQueue.V_QueueType));
        //        cmd.AddParameter("@ActionPending", SqlDbType.Bit, ObjPatientQueue.ActionPending);
        //        cmd.AddParameter("@EnqueueSequenceNo", SqlDbType.Int, ObjPatientQueue.EnqueueSequenceNo);
        //        cmd.AddParameter("@Priority", SqlDbType.Int, ObjPatientQueue.Priority);
        //        if (ObjPatientQueue.EnqueueTime == null)
        //        {
        //            ObjPatientQueue.EnqueueTime = DateTime.Today;
        //        }
        //        cmd.AddParameter("@EnqueueTime", SqlDbType.DateTime, ConvertNullObjectToDBNull(ObjPatientQueue.EnqueueTime));
        //        cmd.AddParameter("@V_PatientQueueItemsStatus", SqlDbType.BigInt, ObjPatientQueue.V_PatientQueueItemsStatus);

        //        int retVal = ExecuteNonQuery(cmd);
        //        if (retVal > 0)
        //            return true;
        //    }
        //    catch
        //    {
        //    }

        //    return false;

        //}
        //public override bool PatientQueue_Insert(PatientQueue ObjPatientQueue)
        //{
        //    using (SqlConnection conn = new SqlConnection(this.ConnectionString))
        //    {
        //        return PatientQueue_Insert(ObjPatientQueue, conn, null);
        //    }
        //}

        //KMx: Anh Tuấn không cho insert vào bảng PatientQueue.
        //public override bool PatientQueue_InsertList(List<PatientQueue> lstPatientQueue, ref List<string> lstPatientQueueFail)
        //{
        //    using (SqlConnection conn = new SqlConnection(this.ConnectionString))
        //    {
        //        lstPatientQueueFail = new List<string>();
        //        foreach (PatientQueue pq in lstPatientQueue)
        //        {
        //            SqlCommand cmd = (SqlCommand)conn.CreateCommand();
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            //cmd.CommandText = "spPatientQueue_Insert";
        //            cmd.CommandText = "spPatientQueue_InsertToQueue";

        //            cmd.AddParameter("@PatientAppointmentID", SqlDbType.BigInt, ConvertNullObjectToDBNull(pq.PatientAppointmentID));
        //            cmd.AddParameter("@RegistrationID", SqlDbType.BigInt, pq.RegistrationID);
        //            cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(pq.PatientPCLReqID));
        //            cmd.AddParameter("@RegistrationDetailsID", SqlDbType.BigInt, ConvertNullObjectToDBNull(pq.RegistrationDetailsID));
        //            cmd.AddParameter("@PrescriptionIssueID", SqlDbType.BigInt, ConvertNullObjectToDBNull(pq.PrescriptionIssueID));
        //            cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(pq.MedServiceID));
        //            cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(pq.PatientID));
        //            cmd.AddParameter("@SequenceNo", SqlDbType.Int, ConvertNullObjectToDBNull(pq.SequenceNo));
        //            cmd.AddParameter("@FullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(pq.FullName));
        //            cmd.AddParameter("@DeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(pq.DeptLocID));
        //            cmd.AddParameter("@V_QueueType", SqlDbType.BigInt, ConvertNullObjectToDBNull(pq.V_QueueType));
        //            cmd.AddParameter("@ActionPending", SqlDbType.Bit, pq.ActionPending);
        //            cmd.AddParameter("@EnqueueSequenceNo", SqlDbType.Int, pq.EnqueueSequenceNo);
        //            cmd.AddParameter("@Priority", SqlDbType.Int, pq.Priority);
        //            if (pq.EnqueueTime == null)
        //            {
        //                pq.EnqueueTime = DateTime.Today;
        //            }
        //            cmd.AddParameter("@EnqueueTime", SqlDbType.DateTime, ConvertNullObjectToDBNull(pq.EnqueueTime));
        //            cmd.AddParameter("@V_PatientQueueItemsStatus", SqlDbType.BigInt, pq.V_PatientQueueItemsStatus);
        //            if (conn.State != ConnectionState.Open)
        //            {
        //                conn.Open();
        //            }
        //            int retVal = ExecuteNonQuery(cmd);
        //            if (retVal < 1)
        //            {
        //                lstPatientQueueFail.Add(pq.FullName);
        //            }
        //        }
        //        if (lstPatientQueueFail.Count < 1)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }

        //    }
        //}
        public override bool PatientQueue_MarkDelete(Int64 RegistrationID, Int64 IDType, Int64 PatientID, Int64 V_QueueType, Int64 DeptLocID, DbConnection connection, DbTransaction tran)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            SqlCommand cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spPatientQueue_MarkDelete";

            cmd.AddParameter("@RegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RegistrationID));
            cmd.AddParameter("@IDType", SqlDbType.BigInt, ConvertNullObjectToDBNull(IDType));
            cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
            cmd.AddParameter("@V_QueueType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_QueueType));
            cmd.AddParameter("@DeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocID));

            cmd.AddParameter("@ReturnVal", SqlDbType.BigInt, DBNull.Value, ParameterDirection.ReturnValue);
            cmd.ExecuteNonQuery();
            return true;
            //int ReturnVal = (cmd.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd.Parameters["@ReturnVal"].Value) : 0);
            //if (ReturnVal == 1)
            //    return true;
            //return false;            
        }

        public override List<PatientQueue> PatientQueue_GetListPaging(PatientQueueSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            totalCount = 0;

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientQueue_GetListPaging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@V_QueueType", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.V_QueueType));
                cmd.AddParameter("@LocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.LocationID));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.StaffID));
                cmd.AddParameter("@V_PatientQueueItemsStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.V_PatientQueueItemsStatus));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(pageIndex));
                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(pageSize));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, ConvertNullObjectToDBNull(bCountTotal));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.OrderBy));
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<PatientQueue> retVal = null;

                IDataReader reader = ExecuteReader(cmd);

                retVal = GetPatientQueueItemCollectionFromReader(reader);
                reader.Close();

                if (bCountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    totalCount = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    totalCount = -1;

                return retVal;
            }
        }
        #endregion

        public override List<RefGenMedDrugDetails> SearchMedDrugs(DrugSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            totalCount = 0;

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_drugs_GetDrugs", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@BrandName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.BrandName));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, pageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, pageSize);
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, bCountTotal);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.OrderBy));
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<RefGenMedDrugDetails> retVal = null;

                IDataReader reader = ExecuteReader(cmd);

                retVal = GetRefGenMedDrugDetailsCollectionFromReader(reader);
                reader.Close();

                if (bCountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    totalCount = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    totalCount = -1;

                return retVal;
            }
        }
        /// <summary>
        /// Tam thoi de vay. Ham SearchInwardDrugClinicDept moi dung y nghia
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="bCountTotal"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public override List<RefGenMedProductDetails> SearchMedProducts(MedProductSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            totalCount = 0;

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GenMedProducts_GetMedProducts", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@StoreID", SqlDbType.BigInt, criteria.StoreID);
                cmd.AddParameter("@BrandName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.BrandName));
                cmd.AddParameter("@V_MedProductType", SqlDbType.BigInt, (long)criteria.MedProductType);
                cmd.AddParameter("@PageIndex", SqlDbType.Int, pageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, pageSize);
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, bCountTotal);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.OrderBy));
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<RefGenMedProductDetails> retVal = null;

                IDataReader reader = ExecuteReader(cmd);

                retVal = GetRefGenMedProductDetailsCollectionFromReader(reader);
                reader.Close();

                if (bCountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    totalCount = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    totalCount = -1;

                return retVal;
            }
        }

        public override List<InwardDrugClinicDept> SearchInwardDrugClinicDept(MedProductSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            totalCount = 0;

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_InwardDrugClinicDept_GetMedProducts", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@StoreID", SqlDbType.BigInt, criteria.StoreID);
                cmd.AddParameter("@BrandName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.BrandName));
                cmd.AddParameter("@V_MedProductType", SqlDbType.BigInt, (long)criteria.MedProductType);
                cmd.AddParameter("@PageIndex", SqlDbType.Int, pageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, pageSize);
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, bCountTotal);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.OrderBy));
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<InwardDrugClinicDept> retVal = null;

                IDataReader reader = ExecuteReader(cmd);

                retVal = GetInwardDrugClinicDeptCollectionFromReader(reader);
                reader.Close();

                if (bCountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    totalCount = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    totalCount = -1;

                return retVal;
            }
        }
        public override List<InwardDrugClinicDept> GetAllInwardDrugClinicDeptByProductList(long storeID, long medProductType, List<long> medProductList)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                return GetAllInwardDrugClinicDeptByProductList(storeID, medProductType, medProductList, conn, null);
            }
        }
        public override List<InwardDrugClinicDept> GetAllInwardDrugClinicDeptByProductList(long storeID, long medProductType, List<long> medProductList, DbConnection connection, DbTransaction tran)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            SqlCommand cmd;
            cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "sp_InwardDrugClinicDept_GetMedProducts_All";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@StoreID", SqlDbType.BigInt, (long)storeID);
            cmd.AddParameter("@V_MedProductType", SqlDbType.BigInt, (long)medProductType);
            IDListOutput<long> idList = new IDListOutput<long>();
            idList.Ids = medProductList;
            cmd.AddParameter("@GenMedProductIDList", SqlDbType.Xml, ConvertNullObjectToDBNull(SerializeToXml<IDListOutput<long>>(idList)));

            List<InwardDrugClinicDept> retVal = null;

            IDataReader reader = ExecuteReader(cmd);

            retVal = GetInwardDrugClinicDeptCollectionFromReader(reader);
            reader.Close();

            return retVal;
        }

        public override List<InwardDrugClinicDept> GetAllInwardDrugClinicDeptByIDList(List<long> inwardDrugIdList)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_InwardDrugClinicDept_GetAllByIDList", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                IDListOutput<long> idList = new IDListOutput<long>() { Ids = inwardDrugIdList };
                cmd.AddParameter("@IDList", SqlDbType.Xml, ConvertNullObjectToDBNull(SerializeToXml<IDListOutput<long>>(idList)));

                cn.Open();
                List<InwardDrugClinicDept> retVal = null;

                IDataReader reader = ExecuteReader(cmd);

                retVal = GetInwardDrugClinicDeptCollectionFromReader(reader);
                reader.Close();

                return retVal;
            }
        }

        public override List<RefGenMedProductDetails> GetGenMedProductsRemainingInStore(Dictionary<long, List<long>> drugIdList)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GenMedProducts_GetRemainingInStore", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                string str = null;
                if (drugIdList != null)
                {
                    XDocument xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                                   new XElement("GenMedProducts",
                                   from item in drugIdList
                                   from details in item.Value
                                   select new XElement("Product",
                                   new XElement("ID", item.Key),
                                    new XElement("StoreID", details)
                                    )));
                    str = xmlDocument.ToString();
                }

                cmd.AddParameter("@IDList", SqlDbType.Xml, ConvertNullObjectToDBNull(str));

                cn.Open();
                List<RefGenMedProductDetails> retVal = null;

                IDataReader reader = ExecuteReader(cmd);

                retVal = GetRefGenMedProductDetailsCollectionFromReader(reader);
                reader.Close();

                return retVal;
            }
        }
        public override List<RefMedicalServiceItem> SearchMedServices(MedicalServiceSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            totalCount = 0;

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_MedServices_GetMedServices", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@MedicalServiceName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.MedicalServiceName));
                cmd.AddParameter("@ServiceTypeIDList", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.ServiceTypeIDList));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, pageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, pageSize);
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, bCountTotal);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.OrderBy));
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<RefMedicalServiceItem> retVal = null;

                IDataReader reader = ExecuteReader(cmd);

                retVal = GetMedicalServiceItemCollectionFromReader(reader);
                reader.Close();

                if (bCountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    totalCount = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    totalCount = -1;

                return retVal;
            }
        }

        public override List<PCLItem> SearchPCLItems(PCLItemSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            totalCount = 0;

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_PCL_GetPCLItems", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamTypeName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.PCLExamTypeName));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, pageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, pageSize);
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, bCountTotal);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.OrderBy));
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<PCLItem> retVal = null;

                IDataReader reader = ExecuteReader(cmd);

                retVal = GetPCLItemCollectionFromReader(reader);
                reader.Close();

                if (bCountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    totalCount = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    totalCount = -1;

                return retVal;
            }
        }

        public override bool SaveChangesOnOutwardDrugClinicInvoice(OutwardDrugClinicDeptInvoice info, out List<long> InwardDrugIDList_Error)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                return SaveChangesOnOutwardDrugClinicInvoice(info, cn, null, out InwardDrugIDList_Error);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="connection"></param>
        /// <param name="tran"></param>
        /// <param name="InwardDrugIDList_Error">Danh sach ID cua InwardDrug khong the xuat duoc(khong du so luong)</param>
        /// <returns></returns>
        public override bool SaveChangesOnOutwardDrugClinicInvoice(OutwardDrugClinicDeptInvoice info, DbConnection connection, DbTransaction tran, out List<long> InwardDrugIDList_Error)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            SqlCommand cmd;
            cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spOutwardDrugClinicDeptInvoices_Save";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@outiID", SqlDbType.BigInt, info.outiID);
            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, info.PtRegistrationID.Value);
            cmd.AddParameter("@IMEID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.IMEID));
            cmd.AddParameter("@HITTypeID", SqlDbType.Decimal, ConvertNullObjectToDBNull(info.HITTypeID));
            long? storeID = null;
            if (info.StoreID.GetValueOrDefault(-1) > 0)
            {
                storeID = info.StoreID;
            }
            else if (info.SelectedStorage != null)
            {
                storeID = info.SelectedStorage.StoreID;
            }
            cmd.AddParameter("@StoreID", SqlDbType.BigInt, ConvertNullObjectToDBNull(storeID));
            cmd.AddParameter("@MSCID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.MSCID));
            cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.StaffID));

            cmd.AddParameter("@TypID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.TypID));
            cmd.AddParameter("@OutInvID", SqlDbType.NVarChar, ConvertNullObjectToDBNull(info.OutInvID));
            cmd.AddParameter("@OutInvoiceNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(info.OutInvoiceNumber));
            cmd.AddParameter("@OutDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(info.OutDate));
            cmd.AddParameter("@IsCommitted", SqlDbType.Bit, ConvertNullObjectToDBNull(info.IsCommitted));

            cmd.AddParameter("@IssueID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.IssueID));
            cmd.AddParameter("@ReturnID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.ReturnID));
            cmd.AddParameter("@V_MedProductType", SqlDbType.BigInt, (long)info.MedProductType);
            cmd.AddParameter("@IsDeleted", SqlDbType.Bit, false);

            cmd.AddParameter("@outiID_Out", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);

            cmd.AddParameter("@OutwardDrugClinicDept_Add", SqlDbType.Xml, info.ConvertDetailsListToXml(info.OutwardDrugClinicDepts));
            //cmd.AddParameter("@OutwardDrugClinicDept_Add", SqlDbType.Xml, info.ConvertDetailsListToXml(colAddedItems));
            cmd.AddParameter("@OutwardDrugClinicDept_Update", SqlDbType.Xml, DBNull.Value);
            cmd.AddParameter("@OutwardDrugClinicDept_Delete", SqlDbType.Xml, DBNull.Value);
            cmd.AddParameter("@ErrorItems", SqlDbType.Xml, DBNull.Value, ParameterDirection.Output);

            int retVal = ExecuteNonQuery(cmd);

            string idList = cmd.Parameters["@ErrorItems"].Value as string;
            IDListOutput<long> IDListOutput = DeserializeFromXml<IDListOutput<long>>(idList);
            if (IDListOutput != null)
            {
                InwardDrugIDList_Error = IDListOutput.Ids;
            }
            else
            {
                InwardDrugIDList_Error = null;
            }

            return true;
        }
        public override bool AddOutwardDrugClinicInvoice(OutwardDrugClinicDeptInvoice info, List<InwardDrugClinicDept> updatedInwardItems, DbConnection connection, DbTransaction tran, out List<long> InwardDrugIDList_Error)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            SqlCommand cmd;
            cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spOutwardDrugClinicDeptInvoices_Add";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, info.PtRegistrationID.Value);
            cmd.AddParameter("@IMEID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.IMEID));
            cmd.AddParameter("@HITTypeID", SqlDbType.Decimal, ConvertNullObjectToDBNull(info.HITTypeID));
            long? storeID = null;
            if (info.StoreID.GetValueOrDefault(-1) > 0)
            {
                storeID = info.StoreID;
            }
            else if (info.SelectedStorage != null)
            {
                storeID = info.SelectedStorage.StoreID;
            }
            cmd.AddParameter("@StoreID", SqlDbType.BigInt, ConvertNullObjectToDBNull(storeID));
            cmd.AddParameter("@MSCID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.MSCID));
            cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.StaffID));

            cmd.AddParameter("@TypID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.TypID));
            cmd.AddParameter("@OutInvID", SqlDbType.NVarChar, ConvertNullObjectToDBNull(info.OutInvID));
            cmd.AddParameter("@OutInvoiceNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(info.OutInvoiceNumber));
            cmd.AddParameter("@OutDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(info.OutDate));
            cmd.AddParameter("@IsCommitted", SqlDbType.Bit, ConvertNullObjectToDBNull(info.IsCommitted));

            cmd.AddParameter("@IssueID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.IssueID));
            cmd.AddParameter("@ReturnID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.ReturnID));
            cmd.AddParameter("@V_MedProductType", SqlDbType.BigInt, (long)info.MedProductType);

            cmd.AddParameter("@outiID_Out", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
            cmd.AddParameter("@InPatientBillingInvID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.InPatientBillingInvID));

            cmd.AddParameter("@OutwardDrugClinicDept_Add", SqlDbType.Xml, info.ConvertDetailsListToXml(info.OutwardDrugClinicDepts));
            if (updatedInwardItems != null)
            {
                XDocument xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                              new XElement("InwardDrugClinicDepts",
                              from details in updatedInwardItems
                              select new XElement("InwardDrugClinicDept",
                              new XElement("InID", details.InID),
                               new XElement("Remaining", details.Remaining)
                               )));

                cmd.AddParameter("@InwardDrugClinicDept_Update", SqlDbType.Xml, ConvertNullObjectToDBNull(xmlDocument.ToString()));
            }
            else
            {
                cmd.AddParameter("@InwardDrugClinicDept_Update", SqlDbType.Xml, DBNull.Value);
            }

            cmd.AddParameter("@ErrorItems", SqlDbType.Xml, DBNull.Value, ParameterDirection.Output);

            int retVal = ExecuteNonQuery(cmd);

            string idList = cmd.Parameters["@ErrorItems"].Value as string;
            IDListOutput<long> IDListOutput = DeserializeFromXml<IDListOutput<long>>(idList);
            if (IDListOutput != null)
            {
                InwardDrugIDList_Error = IDListOutput.Ids;
            }
            else
            {
                InwardDrugIDList_Error = null;
            }

            return true;
        }
        private OutwardDrugClinicDeptInvoice FindDrugInvoiceInList(List<OutwardDrugClinicDeptInvoice> invoiceList, long invoiceID)
        {
            foreach (OutwardDrugClinicDeptInvoice inv in invoiceList)
            {
                if (inv.outiID == invoiceID)
                {
                    return inv;
                }
            }
            return null;
        }
        public override List<OutwardDrugClinicDeptInvoice> GetAllInPatientInvoices(long registrationID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                return GetAllInPatientInvoices(registrationID, cn, null);
            }
        }
        public override List<OutwardDrugClinicDeptInvoice> GetAllInPatientInvoices(long registrationID, DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            SqlCommand cmd;
            cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spGetAllInPatientInvoices";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, registrationID);

            IDataReader reader = ExecuteReader(cmd);
            return GetAllInPatientInvoices(reader);
        }


        public override List<OutwardDrugClinicDeptInvoice> GetInPatientInvoicesHasMedProducts(long registrationID, List<long> MedProductIDs, long? DeptID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                return GetInPatientInvoicesHasMedProducts(registrationID, MedProductIDs, DeptID, cn, null);
            }
        }
        public override List<OutwardDrugClinicDeptInvoice> GetInPatientInvoicesHasMedProducts(long registrationID, List<long> MedProductIDs, long? DeptID, DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            SqlCommand cmd;
            cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spGetInPatientInvoicesHasMedProducts";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, registrationID);
            IDListOutput<long> idList = new IDListOutput<long>() { Ids = MedProductIDs };
            cmd.AddParameter("@GenMedProductIDList", SqlDbType.Xml, ConvertNullObjectToDBNull(SerializeToXml<IDListOutput<long>>(idList)));
            cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptID));

            IDataReader reader = ExecuteReader(cmd);
            return GetAllInPatientInvoices(reader);
        }
        public override List<OutwardDrugClinicDeptInvoice> GetAllInPatientInvoicesByBillingInvoiceID(long billingInvoiceID, DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            SqlCommand cmd;
            cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spGetAllInPatientInvoicesByBillingInvoiceID";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = int.MaxValue;

            cmd.AddParameter("@InPatientBillingInvID", SqlDbType.BigInt, billingInvoiceID);

            IDataReader reader = ExecuteReader(cmd);
            return GetAllInPatientInvoices(reader);
        }
        public override List<OutwardDrugClinicDeptInvoice> GetAllInPatientInvoices_NoBill(long registrationId, long? DeptID, long StoreID, long StaffID, bool IsAdmin, DbConnection conn, DbTransaction tran, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            var cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spGetAllInPatientInvoices_NoBill";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@RegistrationID", SqlDbType.BigInt, registrationId);
            cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptID));
            cmd.AddParameter("@StoreID", SqlDbType.BigInt, StoreID);
            cmd.AddParameter("@StaffID", SqlDbType.BigInt, StaffID);
            cmd.AddParameter("@IsAdmin", SqlDbType.Bit, IsAdmin);
            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(FromDate));
            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(ToDate));
            IDataReader reader = ExecuteReader(cmd);
            return GetAllInPatientInvoices(reader);
        }
        private List<OutwardDrugClinicDeptInvoice> GetAllInPatientInvoices(IDataReader reader)
        {
            List<OutwardDrugClinicDeptInvoice> retVal = new List<OutwardDrugClinicDeptInvoice>();
            if (reader != null)
            {
                while (reader.Read())
                {
                    //Lay thuoc truoc.
                    OutwardDrugClinicDeptInvoice drugInvoice = FindDrugInvoiceInList(retVal, (long)reader["outiID"]);
                    if (drugInvoice == null)
                    {
                        drugInvoice = GetOutwardDrugClinicDeptInvoiceFromReader(reader);
                        drugInvoice.MedProductType = AllLookupValues.MedProductType.THUOC;
                        retVal.Add(drugInvoice);
                    }

                    OutwardDrugClinicDept tempClinicDept = GetOutwardDrugClinicDeptFromReader(reader, drugInvoice.StaffID);

                    if (tempClinicDept.GenMedProductItem != null)
                    {
                        // tempClinicDept.GenMedProductItem = GetRefGenMedProductDetailsFromReader(reader);
                        //Gan lai gia tien (khong lay tu bang gia nua.)
                        tempClinicDept.GenMedProductItem.HIAllowedPrice = tempClinicDept.HIAllowedPrice;
                        tempClinicDept.GenMedProductItem.HIPatientPrice = tempClinicDept.InvoicePrice;
                        tempClinicDept.GenMedProductItem.NormalPrice = tempClinicDept.InvoicePrice;
                    }

                    if (drugInvoice.OutwardDrugClinicDepts == null)
                    {
                        drugInvoice.OutwardDrugClinicDepts = new List<OutwardDrugClinicDept>().ToObservableCollection();
                    }

                    tempClinicDept.CreatedDate = drugInvoice.OutDate.GetValueOrDefault();
                    tempClinicDept.DrugInvoice = drugInvoice;
                    drugInvoice.OutwardDrugClinicDepts.Add(tempClinicDept);
                }



                if (!reader.NextResult())
                {
                    reader.Close();
                    return retVal;
                }
                //Y dung cu
                while (reader.Read())
                {
                    OutwardDrugClinicDeptInvoice medItemInvoice = FindDrugInvoiceInList(retVal, (long)reader["outiID"]);
                    if (medItemInvoice == null)
                    {
                        medItemInvoice = GetOutwardDrugClinicDeptInvoiceFromReader(reader);
                        medItemInvoice.MedProductType = AllLookupValues.MedProductType.Y_CU;
                        retVal.Add(medItemInvoice);
                    }

                    OutwardDrugClinicDept tempClinicDept = GetOutwardDrugClinicDeptFromReader(reader, medItemInvoice.StaffID);

                    //KMx: Nếu gọi GetRefGenMedProductDetailsFromReader thì sẽ bị mất NormalPriceNew, HIPatientPriceNew và HIAllowedPriceNew. Nên bỏ ra cho giống thuốc.
                    //Nếu không sẽ bị lỗi khi load bill, giá bằng 0.(18/08/2014 15:40).
                    if (tempClinicDept.GenMedProductItem != null)
                    {
                        //tempClinicDept.GenMedProductItem = GetRefGenMedProductDetailsFromReader(reader);
                        //Gan lai gia tien (khong lay tu bang gia nua.)
                        tempClinicDept.GenMedProductItem.HIAllowedPrice = tempClinicDept.HIAllowedPrice;
                        tempClinicDept.GenMedProductItem.HIPatientPrice = tempClinicDept.InvoicePrice;
                        tempClinicDept.GenMedProductItem.NormalPrice = tempClinicDept.InvoicePrice;
                    }
                    if (medItemInvoice.OutwardDrugClinicDepts == null)
                    {
                        medItemInvoice.OutwardDrugClinicDepts = new List<OutwardDrugClinicDept>().ToObservableCollection();
                    }

                    tempClinicDept.CreatedDate = medItemInvoice.OutDate.GetValueOrDefault();
                    tempClinicDept.DrugInvoice = medItemInvoice;
                    medItemInvoice.OutwardDrugClinicDepts.Add(tempClinicDept);
                }

                if (!reader.NextResult())
                {
                    reader.Close();
                    return retVal;
                }
                //Hoa chat
                while (reader.Read())
                {
                    OutwardDrugClinicDeptInvoice medItemInvoice = FindDrugInvoiceInList(retVal, (long)reader["outiID"]);
                    if (medItemInvoice == null)
                    {
                        medItemInvoice = GetOutwardDrugClinicDeptInvoiceFromReader(reader);
                        medItemInvoice.MedProductType = AllLookupValues.MedProductType.HOA_CHAT;
                        retVal.Add(medItemInvoice);
                    }

                    OutwardDrugClinicDept tempClinicDept = GetOutwardDrugClinicDeptFromReader(reader, medItemInvoice.StaffID);

                    //KMx: Nếu gọi GetRefGenMedProductDetailsFromReader thì sẽ bị mất NormalPriceNew, HIPatientPriceNew và HIAllowedPriceNew. Nên bỏ ra cho giống thuốc.
                    //Nếu không sẽ bị lỗi khi load bill, giá bằng 0.(18/08/2014 15:40).
                    if (tempClinicDept.GenMedProductItem != null)
                    {
                        //tempClinicDept.GenMedProductItem = GetRefGenMedProductDetailsFromReader(reader);
                        //Gan lai gia tien (khong lay tu bang gia nua.)
                        tempClinicDept.GenMedProductItem.HIAllowedPrice = tempClinicDept.HIAllowedPrice;
                        tempClinicDept.GenMedProductItem.HIPatientPrice = tempClinicDept.InvoicePrice;
                        tempClinicDept.GenMedProductItem.NormalPrice = tempClinicDept.InvoicePrice;
                    }
                    if (medItemInvoice.OutwardDrugClinicDepts == null)
                    {
                        medItemInvoice.OutwardDrugClinicDepts = new List<OutwardDrugClinicDept>().ToObservableCollection();
                    }

                    tempClinicDept.CreatedDate = medItemInvoice.OutDate.GetValueOrDefault();
                    tempClinicDept.DrugInvoice = medItemInvoice;
                    medItemInvoice.OutwardDrugClinicDepts.Add(tempClinicDept);
                }

                reader.Close();
            }

            return retVal;
        }

        public override List<RefGenMedProductSummaryInfo> GetRefGenMedProductSummaryByRegistration(long registrationID, long medProductType, long? DeptID)
        {
            List<RefGenMedProductSummaryInfo> retVal = new List<RefGenMedProductSummaryInfo>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spAllRefGenMedProductDetailsByRegistration", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, registrationID);
                cmd.AddParameter("@V_MedProductType", SqlDbType.BigInt, medProductType);
                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptID));
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                RefGenMedProductSummaryInfo temp;
                while (reader.Read())
                {
                    temp = new RefGenMedProductSummaryInfo();
                    temp.MedProductDetails = new RefGenMedProductDetails();
                    temp.MedProductDetails.GenMedProductID = (long)reader["GenMedProductID"];
                    temp.MedProductDetails.BrandName = (string)reader["BrandName"];
                    temp.MedProductDetails.V_MedProductType = (long)reader["V_MedProductType"];

                    temp.TotalQty = (decimal)reader["TotalQty"];
                    temp.TotalQtyReturned = (decimal)reader["TotalQtyReturned"];
                    temp.EntityState = EntityState.PERSITED;

                    retVal.Add(temp);
                }
            }
            return retVal;
        }

        public override bool ChangeRegistrationType(PatientRegistration regInfo, AllLookupValues.RegistrationType newType)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_ChangeRegistrationType", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, regInfo.PtRegistrationID);
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, (long)newType);

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);

                return retVal > 0;
            }
        }

        public override bool Registrations_UpdateStatus(PatientRegistration regInfo, long V_RegistrationStatus)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("spRegistrations_UpdateStatus", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@RegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(regInfo.PtRegistrationID));
                    cmd.AddParameter("@V_RegistrationStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationStatus));

                    cn.Open();

                    int retVal = ExecuteNonQuery(cmd);

                    return retVal > 0;
                }
                catch (SqlException ex)
                {
                    AxLogger.Instance.LogError(ex.ToString());
                    throw new Exception(ex.Message);
                }
            }
        }


        public override List<Staff> GetStaffsHaveRegistrations(byte Type)
        {
            //0: la lay nhan vien dang ky;1:lay nv nhan benh BH
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_Staff_GetStaffsHaveRegistered", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@Type", SqlDbType.SmallInt, ConvertNullObjectToDBNull(Type));

                cn.Open();
                List<Staff> retVal = null;

                IDataReader reader = ExecuteReader(cmd);

                retVal = GetStaffCollectionFromReader(reader);
                reader.Close();

                return retVal;
            }
        }

        public override List<Staff> GetStaffsHavePayments(long V_TradingPlaces)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_Staff_GetStaffsPayment", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@V_TradingPlaces", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_TradingPlaces));
                cn.Open();
                List<Staff> retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                retVal = GetStaffCollectionFromReader(reader);
                reader.Close();

                return retVal;
            }
        }
        public override bool AddTransactionDetailList(long StaffID, long transactionId, List<PatientTransactionDetail> tranDetailList, out string ListOutID, DbConnection connection, DbTransaction tran)
        {
            ListOutID = "";
            if (transactionId <= 0 || tranDetailList == null || tranDetailList.Count == 0)
            {
                return false;
            }

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            SqlCommand cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spAddTransactionDetails";

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.AddParameter("@StaffID", SqlDbType.BigInt, StaffID);
            cmd.AddParameter("@TransactionID", SqlDbType.BigInt, transactionId);
            cmd.AddParameter("@TransactionDetailsList", SqlDbType.Xml, ConvertPatientTransactionDetailsToXml(tranDetailList).ToString());
            SqlParameter paramIDList = new SqlParameter("@InsertedIDList", SqlDbType.Xml);
            paramIDList.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(paramIDList);
            cmd.ExecuteNonQuery();
            if (paramIDList.Value != DBNull.Value)
            {
                ListOutID = paramIDList.Value.ToString();
            }
            return true;
        }

        public override bool AddTransactionDetailList_InPt(long StaffID, long transactionId, List<PatientTransactionDetail> tranDetailList, out string ListOutID, DbConnection connection, DbTransaction tran)
        {
            ListOutID = "";
            if (transactionId <= 0 || tranDetailList == null || tranDetailList.Count == 0)
            {
                return false;
            }

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            SqlCommand cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spAddTransactionDetails_InPt";

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.AddParameter("@StaffID", SqlDbType.BigInt, StaffID);
            cmd.AddParameter("@TransactionID", SqlDbType.BigInt, transactionId);
            cmd.AddParameter("@TransactionDetailsList", SqlDbType.Xml, ConvertPatientTransactionDetailsToXml(tranDetailList).ToString());
            SqlParameter paramIDList = new SqlParameter("@InsertedIDList", SqlDbType.Xml);
            paramIDList.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(paramIDList);
            cmd.ExecuteNonQuery();
            if (paramIDList.Value != DBNull.Value)
            {
                ListOutID = paramIDList.Value.ToString();
            }
            return true;
        }

        public override bool UpdatePclRequestInfo(PatientPCLRequest p)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = (SqlCommand)conn.CreateCommand();
                cmd.CommandText = "sp_UpdatePclRequestInfo";

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DoctorComments", SqlDbType.NVarChar, ConvertNullObjectToDBNull(p.DoctorComments));
                cmd.AddParameter("@Diagnosis", SqlDbType.NVarChar, ConvertNullObjectToDBNull(p.Diagnosis));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.PatientPCLReqID));
                cmd.AddParameter("@V_PCLRequestType", SqlDbType.BigInt, ConvertNullObjectToDBNull((long)p.V_PCLRequestType));
                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.DoctorStaffID));
                cmd.AddParameter("@MedicalInstructionDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(p.MedicalInstructionDate != null ? p.MedicalInstructionDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : null));
                cmd.AddParameter("@AllowToPayAfter", SqlDbType.Bit, ConvertNullObjectToDBNull(p.AllowToPayAfter));

                conn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
        }
        public override long ActiveHisID(HealthInsurance aHealthInsurance)
        {
            if (aHealthInsurance != null && aHealthInsurance.HIID > 0)
            {
                using (SqlConnection conn = new SqlConnection(this.ConnectionString))
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    SqlCommand cmd;
                    cmd = (SqlCommand)conn.CreateCommand();
                    cmd.CommandText = "spGetActiveHisID";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@HIID", SqlDbType.BigInt, aHealthInsurance.HIID);
                    object o = cmd.ExecuteScalar();
                    if (o != null && o != DBNull.Value)
                    {
                        return (long)o;
                    }
                }
            }
            return 0;
        }
        public override bool AddRegistration(PatientRegistration info, DbConnection conn, DbTransaction tran
            , out long PatientRegistrationID, out int SequenceNo)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            SqlCommand cmd;
            //Neu co bao hiem thi di lay hisaid cua the bao hiem nay roi insert vao.
            if (info.HealthInsurance != null && info.HealthInsurance.HIID > 0)
            {
                cmd = (SqlCommand)conn.CreateCommand();
                cmd.Transaction = (SqlTransaction)tran;
                cmd.CommandText = "spGetActiveHisID";

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@HIID", SqlDbType.BigInt, info.HealthInsurance.HIID);

                object o = cmd.ExecuteScalar();
                if (o != null && o != DBNull.Value)
                {
                    info.HisID = (long)o;
                }
            }
            cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spCreateNewRegistration";

            cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.StaffID));
            cmd.AddParameter("@PatientID", SqlDbType.Decimal, ConvertNullObjectToDBNull(info.PatientID));
            if (info.RegTypeID.HasValue && info.RegTypeID.Value > 0)
            {
                cmd.AddParameter("@RegistrationTypeID", SqlDbType.Int, ConvertNullObjectToDBNull(info.RegTypeID));
            }
            else
            {
                if (info.RegistrationType != null)
                {
                    cmd.AddParameter("@RegistrationTypeID", SqlDbType.Int, ConvertNullObjectToDBNull(info.RegistrationType.RegTypeID));
                }
                else
                {
                    cmd.AddParameter("@RegistrationTypeID", SqlDbType.Int, DBNull.Value);
                }
            }
            if (info.DeptID.HasValue && info.DeptID.Value > 0)
            {
                cmd.AddParameter("@DepartmentID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.DeptID));
            }
            else
            {
                if (info.RefDepartment != null)
                {
                    cmd.AddParameter("@DepartmentID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.RefDepartment.DeptID));
                }
                else
                {
                    cmd.AddParameter("@DepartmentID", SqlDbType.BigInt, DBNull.Value);
                }
            }
            cmd.AddParameter("@EmergRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.EmergRecID));
            if (info.PaperReferal != null)
            {
                cmd.AddParameter("@PaperReferalID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.PaperReferal.RefID));
            }
            else
            {
                cmd.AddParameter("@PaperReferalID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.PaperReferalID));
            }

            cmd.AddParameter("@PatientClassID", SqlDbType.BigInt, info.PatientClassification.PatientClassID);
            cmd.AddParameter("@ExamDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(info.ExamDate));
            cmd.AddParameter("@V_DocumentTypeOnHold", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.V_DocumentTypeOnHold));

            cmd.AddParameter("@IsCrossRegion", SqlDbType.Bit, ConvertNullObjectToDBNull(info.IsCrossRegion));
            cmd.AddParameter("@PtInsuranceBenefit", SqlDbType.Decimal, ConvertNullObjectToDBNull(info.PtInsuranceBenefit));

            cmd.AddParameter("@HisID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.HisID));

            cmd.AddParameter("@ProgSumMinusMinHI", SqlDbType.Decimal, ConvertNullObjectToDBNull(info.ProgSumMinusMinHI));

            cmd.AddParameter("@V_RegistrationStatus", SqlDbType.BigInt, (long)info.RegistrationStatus);
            cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, (long)info.V_RegistrationType);
            cmd.AddParameter("@PatientRegistrationID", SqlDbType.BigInt, info.PtRegistrationID, ParameterDirection.Output);
            cmd.AddParameter("@SequenceNo", SqlDbType.Int, info.SequenceNo, ParameterDirection.Output);

            int retVal = ExecuteNonQuery(cmd);

            PatientRegistrationID = (long)cmd.Parameters["@PatientRegistrationID"].Value;
            SequenceNo = (int)cmd.Parameters["@SequenceNo"].Value;
            return true;
        }


        public override bool AddRegistrationDetails(long registrationID, int FindPatient, List<PatientRegistrationDetail> regDetailList, DbConnection conn, DbTransaction tran, out List<long> newItemIDList)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            SqlCommand cmd;

            cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spAddNewRegistrationDetails";
            if (FindPatient == (int)AllLookupValues.V_FindPatientType.NOI_TRU)
            {
                cmd.CommandText = "spAddNewRegistrationDetails_InPt";
            }


            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, registrationID);

            string detailListString = null;
            if (regDetailList != null && regDetailList.Count > 0)
            {
                detailListString = ConvertPatientRegistrationDetailsToXml(regDetailList).ToString();
            }

            cmd.AddParameter("@RegistrationDetails_Add", SqlDbType.Xml, ConvertNullObjectToDBNull(detailListString));
            cmd.AddParameter("@InsertedIDList", SqlDbType.Xml, DBNull.Value, ParameterDirection.Output);

            int retVal = ExecuteNonQuery(cmd);
            string idList = cmd.Parameters["@InsertedIDList"].Value as string;
            IDListOutput<long> IDListOutput = DeserializeFromXml<IDListOutput<long>>(idList);
            if (IDListOutput != null)
            {
                newItemIDList = IDListOutput.Ids;
            }
            else
            {
                newItemIDList = null;
            }
            return true;
        }

        public override bool UpdateRegistrationDetails(IList<PatientRegistrationDetail> regDetailList, int FindPatient, DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            SqlCommand cmd;

            cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sp_UpdateNewRegistrationDetails";
            cmd.CommandTimeout = int.MaxValue;

            string detailListString = null;
            if (regDetailList != null && regDetailList.Count > 0)
            {
                detailListString = ConvertPatientRegistrationDetailsToXml(regDetailList).ToString();
            }

            cmd.AddParameter("@RegistrationDetails", SqlDbType.Xml, ConvertNullObjectToDBNull(detailListString));
            cmd.AddParameter("@FindPatient", SqlDbType.Int, ConvertNullObjectToDBNull(FindPatient));

            int retVal = ExecuteNonQuery(cmd);
            return retVal > 0;
        }

        public override void DeletePCLRequestWithDetails(Int64 PatientPCLReqID, out string Result)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    Result = "";

                    SqlCommand cmd = new SqlCommand("spDeletePCLRequestWithDetails", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));
                    cmd.AddParameter("@Result", SqlDbType.NVarChar, 20, ParameterDirection.Output);

                    cn.Open();

                    ExecuteNonQuery(cmd);
                    if (cmd.Parameters["@Result"].Value != null)
                        Result = cmd.Parameters["@Result"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public override void DeleteInPtPCLRequestWithDetails(Int64 PatientPCLReqID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spDeleteInPtPCLRequestWithDetails", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));
                    cn.Open();
                    ExecuteNonQuery(cmd);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //KMx: Không truyền ServiceRecID, dùng PtRegistrationID xuống database kiểm tra xem có ServiceRecord chưa, nếu chưa có thì tạo mới (02/11/2014 11:34).
        //public override bool AddPCLRequestWithDetails(PatientPCLRequest request, DbConnection connection, DbTransaction tran, out long PCLRequestID)
        //{
        //    if (connection.State != ConnectionState.Open)
        //    {
        //        connection.Open();
        //    }

        //    SqlCommand cmd = (SqlCommand)connection.CreateCommand();
        //    cmd.Transaction = (SqlTransaction)tran;
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.CommandText = "spAddNewPCLRequestWithDetails";

        //    long? serviceRecID = null;
        //    if (request.PatientServiceRecord != null && request.PatientServiceRecord.ServiceRecID > 0)
        //    {
        //        serviceRecID = request.PatientServiceRecord.ServiceRecID;
        //    }
        //    else
        //    {
        //        serviceRecID = request.ServiceRecID;
        //    }
        //    cmd.AddParameter("@ServiceRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(serviceRecID));
        //    cmd.AddParameter("@PCLRequestNumID", SqlDbType.NVarChar, ConvertNullObjectToDBNull(request.PCLRequestNumID));
        //    cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(request.V_PCLMainCategory));
        //    cmd.AddParameter("@ReqFromDeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(request.ReqFromDeptLocID));

        //    cmd.AddParameter("@Diagnosis", SqlDbType.NVarChar, request.Diagnosis);
        //    cmd.AddParameter("@DoctorComments", SqlDbType.NVarChar, ConvertNullObjectToDBNull(request.DoctorComments));
        //    cmd.AddParameter("@IsExternalExam", SqlDbType.Bit, ConvertNullObjectToDBNull(request.IsExternalExam));
        //    cmd.AddParameter("@IsImported", SqlDbType.Bit, ConvertNullObjectToDBNull(request.IsImported));
        //    cmd.AddParameter("@IsCaseOfEmergency", SqlDbType.Bit, ConvertNullObjectToDBNull(request.IsCaseOfEmergency));

        //    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(request.StaffID));
        //    cmd.AddParameter("@V_PCLRequestType", SqlDbType.BigInt, (long)request.V_PCLRequestType);
        //    cmd.AddParameter("@V_PCLRequestStatus", SqlDbType.BigInt, (long)request.V_PCLRequestStatus);
        //    cmd.AddParameter("@CreatedDate", SqlDbType.DateTime, request.CreatedDate);
        //    cmd.AddParameter("@AgencyID", SqlDbType.BigInt, ConvertNullObjectToDBNull(request.AgencyID));
        //    cmd.AddParameter("@InPatientBillingInvID", SqlDbType.BigInt, ConvertNullObjectToDBNull(request.InPatientBillingInvID));

        //    string detailListString = null;
        //    if (request.PatientPCLRequestIndicators != null && request.PatientPCLRequestIndicators.Count > 0)
        //    {
        //        detailListString = ConvertPCLRequestDetailsToXml(request.PatientPCLRequestIndicators).ToString();
        //    }
        //    cmd.AddParameter("@PCLRequestDetails_Add", SqlDbType.Xml, ConvertNullObjectToDBNull(detailListString));

        //    cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);

        //    int retVal = ExecuteNonQuery(cmd);


        //    PCLRequestID = (long)cmd.Parameters["@PatientPCLReqID"].Value;
        //    return true;
        //}


        public override bool AddPCLRequestWithDetails(long ptRegistrationID, long V_RegistrationType, long ptMedicalRecordID, PatientPCLRequest request, DbConnection connection, DbTransaction tran, out long PCLRequestID)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            SqlCommand cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spAddNewPCLRequestWithDetails_InPt";

            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ptRegistrationID));
            cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));
            cmd.AddParameter("@PtMedicalRecordID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ptMedicalRecordID));

            cmd.AddParameter("@PCLRequestNumID", SqlDbType.NVarChar, ConvertNullObjectToDBNull(request.PCLRequestNumID));
            cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(request.PatientPCLReqID));
            cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(request.V_PCLMainCategory));
            cmd.AddParameter("@ReqFromDeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(request.ReqFromDeptLocID));
            cmd.AddParameter("@ReqFromDeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(request.ReqFromDeptID));

            cmd.AddParameter("@Diagnosis", SqlDbType.NVarChar, request.Diagnosis);
            cmd.AddParameter("@DoctorComments", SqlDbType.NVarChar, ConvertNullObjectToDBNull(request.DoctorComments));
            cmd.AddParameter("@IsExternalExam", SqlDbType.Bit, ConvertNullObjectToDBNull(request.IsExternalExam));
            cmd.AddParameter("@IsImported", SqlDbType.Bit, ConvertNullObjectToDBNull(request.IsImported));
            cmd.AddParameter("@IsCaseOfEmergency", SqlDbType.Bit, ConvertNullObjectToDBNull(request.IsCaseOfEmergency));

            cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(request.StaffID));
            cmd.AddParameter("@V_PCLRequestType", SqlDbType.BigInt, (long)request.V_PCLRequestType);
            cmd.AddParameter("@V_PCLRequestStatus", SqlDbType.BigInt, (long)request.V_PCLRequestStatus);
            cmd.AddParameter("@CreatedDate", SqlDbType.DateTime, request.CreatedDate);
            cmd.AddParameter("@AgencyID", SqlDbType.BigInt, ConvertNullObjectToDBNull(request.AgencyID));
            cmd.AddParameter("@InPatientBillingInvID", SqlDbType.BigInt, ConvertNullObjectToDBNull(request.InPatientBillingInvID));

            string detailListString = null;
            if (request.PatientPCLRequestIndicators != null && request.PatientPCLRequestIndicators.Count > 0)
            {
                detailListString = ConvertPCLRequestDetailsToXml(request.PatientPCLRequestIndicators).ToString();
            }
            cmd.AddParameter("@PCLRequestDetails_Add", SqlDbType.Xml, ConvertNullObjectToDBNull(detailListString));
            //HPT 13/09/2016: trường hợp cập nhật bill có load thêm phiếu chỉ định cận lâm sàng vào thì phải có PatientPCLRequestID mới phân biệt được phiếu chỉ định này chỉ cần cập nhật InPatientBillingInvID chứ không tạo mới
            //trước đây int size truyền vào DBNull.Value, giờ truyền PatientPCLReqID xuống
            cmd.AddParameter("@NewPatientPCLReqID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);

            cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(request.DoctorStaffID));
            cmd.AddParameter("@MedicalInstructionDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(request.MedicalInstructionDate));

            int retVal = ExecuteNonQuery(cmd);


            PCLRequestID = (long)cmd.Parameters["@PatientPCLReqID"].Value;
            return true;
        }



        public override bool AddPCLRequestDetails(long pclRequestID, List<PatientPCLRequestDetail> requestDetailList, DbConnection connection, DbTransaction tran)
        {
            if (pclRequestID <= 0 || requestDetailList == null || requestDetailList.Count == 0)
            {
                return false;
            }
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            SqlCommand cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spAddNewPCLRequestDetailList";

            string detailListString = ConvertPCLRequestDetailsToXml(requestDetailList).ToString();

            cmd.AddParameter("@PCLRequestDetails_Add", SqlDbType.Xml, ConvertNullObjectToDBNull(detailListString));
            cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, pclRequestID);

            int retVal = ExecuteNonQuery(cmd);
            return true;
        }

        public override bool AddPCLRequestDetails(List<PatientPCLRequestDetail> requestDetailList, DbConnection connection, DbTransaction tran)
        {
            if (requestDetailList == null || requestDetailList.Count == 0)
            {
                return false;
            }
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            SqlCommand cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spAddNewPCLRequestDetailList";

            string detailListString = ConvertPCLRequestDetailsToXml(requestDetailList).ToString();

            cmd.AddParameter("@PCLRequestDetails_Add", SqlDbType.Xml, ConvertNullObjectToDBNull(detailListString));

            int retVal = ExecuteNonQuery(cmd);
            return true;
        }
        public override bool UpdatePCLRequestDetailList(List<PatientPCLRequestDetail> requestDetailList, DbConnection connection, DbTransaction tran)
        {
            if (requestDetailList == null || requestDetailList.Count == 0)
            {
                return false;
            }
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            SqlCommand cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spUpdatePCLRequestDetailList";
            cmd.CommandTimeout = int.MaxValue;


            string detailListString = ConvertPCLRequestDetailsToXml(requestDetailList).ToString();

            cmd.AddParameter("@PCLRequestDetails_Update", SqlDbType.Xml, ConvertNullObjectToDBNull(detailListString));

            int retVal = ExecuteNonQuery(cmd);
            return true;
        }

        public override bool DeletePCLRequestDetailList(List<PatientPCLRequestDetail> requestDetailList, DbConnection connection, DbTransaction tran)
        {
            if (requestDetailList == null || requestDetailList.Count == 0)
            {
                return false;
            }
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            SqlCommand cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spDeletePCLRequestDetailList";

            string detailListString = ConvertPCLRequestDetailsToXml(requestDetailList).ToString();

            cmd.AddParameter("@PCLRequestDetails_Delete", SqlDbType.Xml, ConvertNullObjectToDBNull(detailListString));

            int retVal = ExecuteNonQuery(cmd);
            return true;
        }

        public override bool UpdateBillingInvoice(InPatientBillingInvoice billingInv)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUpdateBillingInvoice", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@InPatientBillingInvID", SqlDbType.BigInt, billingInv.InPatientBillingInvID);
                    cmd.AddParameter("@BillFromDate", SqlDbType.DateTime, billingInv.BillFromDate);
                    cmd.AddParameter("@BillToDate", SqlDbType.DateTime, billingInv.BillToDate);
                    cmd.AddParameter("@IsAdditionalFee", SqlDbType.Bit, billingInv.IsAdditionalFee);
                    cmd.AddParameter("@NotApplyMaxHIPay", SqlDbType.Bit, billingInv.NotApplyMaxHIPay);

                    cn.Open();

                    int retVal = ExecuteNonQuery(cmd);

                    return retVal > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public override bool DeleteRegistrationDetailList(List<PatientRegistrationDetail> registrationDetailList, DbConnection connection, DbTransaction tran)
        {
            if (registrationDetailList == null || registrationDetailList.Count == 0)
            {
                return false;
            }
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            SqlCommand cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sp_DeleteRegistrationDetailsByIDList";

            XDocument xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                      new XElement("RegistrationDetails",
                      from details in registrationDetailList
                      select new XElement("RecInfo",
                          new XElement("PtRegDetailID", details.PtRegDetailID)
                       )));

            string detailListString = xmlDocument.ToString();

            cmd.AddParameter("@RegistrationDetailsList", SqlDbType.Xml, ConvertNullObjectToDBNull(detailListString));

            int retVal = ExecuteNonQuery(cmd);
            return true;
        }

        public override bool DeleteOutwardDrugClinicDeptList(List<OutwardDrugClinicDept> outwardDrugClinicDeptList, DbConnection connection, DbTransaction tran)
        {
            if (outwardDrugClinicDeptList == null || outwardDrugClinicDeptList.Count == 0)
            {
                return false;
            }
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            SqlCommand cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sp_DeleteOutwardDrugClinicDeptList";

            XDocument xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                      new XElement("OutwardDrugClinicDepts",
                      from details in outwardDrugClinicDeptList
                      select new XElement("RecInfo",
                          new XElement("OutID", details.OutID),
                          new XElement("InID", details.InID),
                          new XElement("Qty", details.Qty)
                       )));

            string detailListString = xmlDocument.ToString();

            cmd.AddParameter("@OutwardDrugClinicDeptList", SqlDbType.Xml, ConvertNullObjectToDBNull(detailListString));

            int retVal = ExecuteNonQuery(cmd);
            return true;
        }

        public override bool DeleteOutwardDrugClinicDeptInvoices(List<OutwardDrugClinicDeptInvoice> outwardDrugClinicDeptClinicDeptInvoices, DbConnection connection, DbTransaction tran)
        {
            try
            {
                if (outwardDrugClinicDeptClinicDeptInvoices == null || outwardDrugClinicDeptClinicDeptInvoices.Count == 0)
                {
                    return false;
                }
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                SqlCommand cmd = (SqlCommand)connection.CreateCommand();
                cmd.Transaction = (SqlTransaction)tran;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "sp_DeleteOutwardDrugClinicDeptInvoices";

                XDocument xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                          new XElement("OutwardDrugClinicDeptInvoices",
                          from details in outwardDrugClinicDeptClinicDeptInvoices
                          select new XElement("RecInfo",
                              new XElement("outiID", details.outiID)
                           )));

                string detailListString = xmlDocument.ToString();

                cmd.AddParameter("@OutwardDrugClinicDeptInvoices", SqlDbType.Xml, ConvertNullObjectToDBNull(detailListString));

                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public override bool AddOutwardDrugClinicDeptIntoBill(long PtRegistrationID, long InPatientBillingInvID, List<OutwardDrugClinicDeptInvoice> outwardDrugClinicDeptClinicDeptInvoices, DbConnection connection, DbTransaction tran)
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                SqlCommand cmd = (SqlCommand)connection.CreateCommand();
                cmd.Transaction = (SqlTransaction)tran;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "sp_AddOutwardDrugClinicDeptInvoiceList";

                XDocument xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"));
                var root = ConvertOutwardDrugClinicDeptInvoicesToXmlElement(outwardDrugClinicDeptClinicDeptInvoices);
                xmlDocument.Add(root);

                string detailListString = xmlDocument.ToString();

                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, PtRegistrationID);
                cmd.AddParameter("@InPatientBillingInvID", SqlDbType.BigInt, InPatientBillingInvID);
                cmd.AddParameter("@OutwardDrugClinicDeptInvoices", SqlDbType.Xml, ConvertNullObjectToDBNull(detailListString));

                SqlParameter parID = new SqlParameter("@InsertedIDList", SqlDbType.Xml);
                parID.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(parID);

                cmd.CommandTimeout = int.MaxValue;

                int count = ExecuteNonQuery(cmd);

                cmd.Dispose();
                return count > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public override bool UpdateOutwardDrugClinicDeptList(IList<OutwardDrugClinicDept> outwardDrugClinicDeptList, long? StaffID, DbConnection connection, DbTransaction tran)
        {
            if (outwardDrugClinicDeptList == null || outwardDrugClinicDeptList.Count == 0)
            {
                return false;
            }
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            SqlCommand cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sp_UpdateOutwardDrugClinicDeptList";
            cmd.CommandTimeout = int.MaxValue;

            XDocument xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                      new XElement("OutwardDrugClinicDepts",
                      from details in outwardDrugClinicDeptList
                      select new XElement("OutwardDrugClinicDept",
                            new XElement("OutID", details.OutID),
                            new XElement("InID", details.InID),
                            new XElement("GenMedProductID", details.GenMedProductID),
                            //new XElement("Qty", details.Qty),
                            new XElement("OutQuantity", details.OutQuantity),
                            new XElement("OutPrice", details.InvoicePrice),
                            new XElement("QtyReturn", details.QtyReturn),
                            new XElement("OutAmount", details.TotalInvoicePrice),
                            new XElement("OutPriceDifference", details.TotalPriceDifference),
                            new XElement("OutAmountCoPay", details.TotalCoPayment),
                            new XElement("OutHIRebate", details.TotalHIPayment),
                            new XElement("HIAllowedPrice", details.HIAllowedPrice),
                            new XElement("IsCountHI", details.IsCountHI),
                            new XElement("IsCountPatient", details.IsCountPatient),
                            new XElement("IsInPackage", details.IsInPackage),
                            new XElement("HIPaymentPercent", details.HIPaymentPercent),
                            new XElement("HIBenefit", details.HIBenefit),
                            new XElement("HisID", details.HisID)
                       )));

            string detailListString = xmlDocument.ToString();

            cmd.AddParameter("@OutwardDrugClinicDeptList", SqlDbType.Xml, ConvertNullObjectToDBNull(detailListString));
            cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
            int retVal = ExecuteNonQuery(cmd);
            return true;
        }

        public override bool CalculateBillInvoice(long InPatientBillingInvID, long StaffID, bool IsUpdateBill, DbConnection connection, DbTransaction tran)
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                SqlCommand cmd = (SqlCommand)connection.CreateCommand();
                cmd.Transaction = (SqlTransaction)tran;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spCalculateBillInvoice";

                cmd.AddParameter("@InPatientBillingInvID", SqlDbType.BigInt, ConvertNullObjectToDBNull(InPatientBillingInvID));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                cmd.AddParameter("@IsUpdateBill", SqlDbType.Bit, ConvertNullObjectToDBNull(IsUpdateBill));
                int retVal = ExecuteNonQuery(cmd);
                return retVal > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /*==== #004 ====*/
        //public override bool CalAdditionalFeeAndTotalBill(long InPatientBillingInvID, long StaffID, bool IsUpdateBill, DbConnection connection, DbTransaction tran)
        public override bool CalAdditionalFeeAndTotalBill(long InPatientBillingInvID, long StaffID, bool IsUpdateBill, DbConnection connection, DbTransaction tran, DateTime CreatedDate, double? HIBenefit = 1, bool IsHICard_FiveYearsCont_NoPaid = false)
        /*==== #004 ====*/
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                SqlCommand cmd = (SqlCommand)connection.CreateCommand();
                cmd.Transaction = (SqlTransaction)tran;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spCalAdditionalFeeAndTotalBill";

                cmd.AddParameter("@InPatientBillingInvID", SqlDbType.BigInt, ConvertNullObjectToDBNull(InPatientBillingInvID));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                cmd.AddParameter("@IsUpdateBill", SqlDbType.Bit, ConvertNullObjectToDBNull(IsUpdateBill));

                /*==== #004 ====*/
                cmd.AddParameter("@HIBenefit", SqlDbType.Float, ConvertNullObjectToDBNull(HIBenefit));
                cmd.AddParameter("@IsHICard_FiveYearsCont_NoPaid", SqlDbType.Bit, ConvertNullObjectToDBNull(IsHICard_FiveYearsCont_NoPaid));
                /*==== #004 ====*/
                /*▼====: #010*/
                cmd.AddParameter("@CreatedDate", SqlDbType.DateTime, CreatedDate);
                /*▲====: #010*/
                int retVal = ExecuteNonQuery(cmd);
                return retVal > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        //dinh them cho nay vo cho ngoai vien
        //---them moi pclrequest dong thoi them detail
        public override bool AddPCLRequestExtWithDetails(PatientPCLRequest_Ext request, out long PatientPCLReqExtID)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {


                SqlCommand cmd = (SqlCommand)conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spAddNewPCLRequestExtWithDetails";

                long? serviceRecID = null;
                if (request.PatientServiceRecord != null && request.PatientServiceRecord.ServiceRecID > 0)
                {
                    serviceRecID = request.PatientServiceRecord.ServiceRecID;
                }
                else
                {
                    serviceRecID = request.ServiceRecID;
                }
                cmd.AddParameter("@ServiceRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(serviceRecID));
                cmd.AddParameter("@PCLRequestNumID", SqlDbType.NVarChar, ConvertNullObjectToDBNull(request.PCLRequestNumID));
                cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(request.V_PCLMainCategory));
                cmd.AddParameter("@ReqFromDeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(request.ReqFromDeptLocID));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(request.PtRegistrationID));

                cmd.AddParameter("@Diagnosis", SqlDbType.NVarChar, request.Diagnosis);
                cmd.AddParameter("@DoctorComments", SqlDbType.NVarChar, ConvertNullObjectToDBNull(request.DoctorComments));
                cmd.AddParameter("@IsExternalExam", SqlDbType.Bit, ConvertNullObjectToDBNull(true));
                cmd.AddParameter("@IsImported", SqlDbType.Bit, ConvertNullObjectToDBNull(request.IsImported));
                cmd.AddParameter("@IsCaseOfEmergency", SqlDbType.Bit, ConvertNullObjectToDBNull(request.IsCaseOfEmergency));

                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(request.StaffID));
                cmd.AddParameter("@V_PCLRequestType", SqlDbType.BigInt, (long)request.V_PCLRequestType);
                cmd.AddParameter("@V_PCLRequestStatus", SqlDbType.BigInt, (long)request.V_PCLRequestStatus);
                cmd.AddParameter("@CreatedDate", SqlDbType.DateTime, DateTime.Now);
                cmd.AddParameter("@AgencyID", SqlDbType.BigInt, ConvertNullObjectToDBNull(request.AgencyID));
                cmd.AddParameter("@InPatientBillingInvID", SqlDbType.BigInt, ConvertNullObjectToDBNull(request.InPatientBillingInvID));

                string detailListString = null;
                if (request.PatientPCLRequestIndicatorsExt != null && request.PatientPCLRequestIndicatorsExt.Count > 0)
                {
                    detailListString = ConvertPCLRequestDetailsExtToXml(request.PatientPCLRequestIndicatorsExt).ToString();
                }
                cmd.AddParameter("@PCLRequestDetails_Add", SqlDbType.Xml, ConvertNullObjectToDBNull(detailListString));

                cmd.AddParameter("@PatientPCLReqExtID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                int retVal = ExecuteNonQuery(cmd);


                PatientPCLReqExtID = (long)cmd.Parameters["@PatientPCLReqExtID"].Value;
                return true;
            }
        }

        public override bool PCLRequestExtUpdate(PatientPCLRequest_Ext request)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = (SqlCommand)conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spPCLRequestExtUpdate";
                cmd.AddParameter("@Diagnosis", SqlDbType.NVarChar, request.Diagnosis);
                cmd.AddParameter("@DoctorComments", SqlDbType.NVarChar, ConvertNullObjectToDBNull(request.DoctorComments));

                cmd.AddParameter("@AgencyID", SqlDbType.BigInt, ConvertNullObjectToDBNull(request.AgencyID));
                cmd.AddParameter("@PatientPCLReqExtID", SqlDbType.BigInt, ConvertNullObjectToDBNull(request.PatientPCLReqExtID));
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                int retVal = ExecuteNonQuery(cmd);

                return retVal > 0;
            }
        }

        //--them moi detail cho pclrequest
        public override bool AddNewPCLRequestDetailsExt(PatientPCLRequest_Ext request)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {


                SqlCommand cmd = (SqlCommand)conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spAddNewPCLRequestDetailsExt";
                cmd.AddParameter("@PatientPCLReqExtID", SqlDbType.BigInt, ConvertNullObjectToDBNull(request.PatientPCLReqExtID));

                string detailListString = null;
                if (request.PatientPCLRequestIndicatorsExt != null && request.PatientPCLRequestIndicatorsExt.Count > 0)
                {
                    detailListString = ConvertPCLRequestDetailsExtToXml(request.PatientPCLRequestIndicatorsExt).ToString();
                }
                cmd.AddParameter("@PCLRequestDetails_Add", SqlDbType.Xml, ConvertNullObjectToDBNull(detailListString));

                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                int retVal = ExecuteNonQuery(cmd);
                return retVal > 0;
            }
        }

        public override bool DeletePCLRequestDetailExtList(List<PatientPCLRequestDetail_Ext> requestDetailList)
        {
            if (requestDetailList == null || requestDetailList.Count == 0)
            {
                return false;
            }
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = (SqlCommand)conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spDeletePCLRequestDetailExtXML";

                string detailListString = ConvertPCLRequestDetailsExtToXml(requestDetailList).ToString();

                cmd.AddParameter("@PCLRequestDetails_Delete", SqlDbType.Xml, ConvertNullObjectToDBNull(detailListString));

                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                int retVal = ExecuteNonQuery(cmd);
                return true;
            }
        }

        public override bool DeletePCLRequestExt(long PatientPCLReqExtID)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = (SqlCommand)conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spDeletePCLRequestExt";

                cmd.AddParameter("@PatientPCLReqExtID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqExtID));

                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                int retVal = ExecuteNonQuery(cmd);
                return true;
            }
        }

        public override List<PatientPCLRequest_Ext> GetPCLRequestListExtByRegistrationID(long RegistrationID)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd;
                cmd = (SqlCommand)conn.CreateCommand();
                cmd.CommandText = "spGetPCLRequestExtByRegistrationID";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@RegistrationID", SqlDbType.BigInt, RegistrationID);

                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                IDataReader reader = ExecuteReader(cmd);
                return FillPclRequestExtList(reader);
            }
        }

        public override PatientPCLRequest_Ext GetPCLRequestExtPK(long PatientPCLReqExtID)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd;
                cmd = (SqlCommand)conn.CreateCommand();
                cmd.CommandText = "spGetPCLRequestExtByPK";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientPCLReqExtID", SqlDbType.BigInt, PatientPCLReqExtID);

                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                IDataReader reader = ExecuteReader(cmd);
                PatientPCLRequest_Ext temp = null;
                if (reader.Read())
                {
                    temp = GetPatientPCLRequestExtFromReader(reader);
                    temp.PatientPCLRequestIndicatorsExt = new ObservableCollection<PatientPCLRequestDetail_Ext>().ToObservableCollection();
                    temp.PatientPCLRequestIndicatorsExt.Add(
                        GetPatientPCLRequestDetailsExtFromReader(reader, temp.StaffID));

                }
                return temp;

            }
        }

        public override PatientPCLRequest_Ext PatientPCLRequestExtByID(long PatientPCLReqExtID)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd;
                cmd = (SqlCommand)conn.CreateCommand();
                cmd.CommandText = "spPatientPCLRequestExtByID";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientPCLReqExtID", SqlDbType.BigInt, PatientPCLReqExtID);

                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                IDataReader reader = ExecuteReader(cmd);
                PatientPCLRequest_Ext temp = null;
                if (reader.Read())
                {
                    temp = GetPatientPCLRequestExtFromReader(reader);
                    temp.PatientPCLRequestIndicatorsExt = new ObservableCollection<PatientPCLRequestDetail_Ext>().ToObservableCollection();
                    temp.PatientPCLRequestIndicatorsExt.Add(
                        GetPatientPCLRequestDetailsExtFromReader(reader, temp.StaffID));

                }
                return temp;

            }
        }

        public override List<PatientPCLRequest_Ext> PatientPCLRequestExtPaging(PatientPCLRequestExtSearchCriteria SearchCriteria,
                        int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLRequestExtPaging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PatientID));
                cmd.AddParameter("@V_PCLRequestStatus", SqlDbType.Int, ConvertNullObjectToDBNull(SearchCriteria.V_PCLRequestStatus));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.Int, ConvertNullObjectToDBNull(SearchCriteria.PtRegistrationID));
                cmd.AddParameter("@TypeList", SqlDbType.Int, (int)(SearchCriteria.LoaiDanhSach));
                cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_PCLMainCategory));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<PatientPCLRequest_Ext> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                lst = FillPclRequestExtList(reader);
                //if (reader != null)
                //{
                //    lst = GetPatientPCLRequestExtCollectionFromReader(reader);
                //}
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;
                return lst;
            }
        }

        public override List<PatientPCLRequestDetail_Ext> GetPCLRequestDetailListExtByRegistrationID(long RegistrationID)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd;
                cmd = (SqlCommand)conn.CreateCommand();
                cmd.CommandText = "spGetPCLRequestDetailExtByRegistrationID";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@RegistrationID", SqlDbType.BigInt, RegistrationID);

                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                IDataReader reader = ExecuteReader(cmd);
                try
                {
                    return GetPatientPCLRequestDetailsExtCollectionFromReader(reader);
                }
                finally
                {
                    reader.Close();
                }


            }
        }




        public override List<PatientRegistrationDetail> GetAllRegistrationDetailsByIDList(List<long> regDetailIdList)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                return GetAllRegistrationDetailsByIDList(regDetailIdList, conn, null);
            }
        }

        public override List<PatientRegistrationDetail> GetAllRegistrationDetailsByIDList(List<long> regDetailIdList, DbConnection connection, DbTransaction tran)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            SqlCommand cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spGetAllRegistrationDetails";

            IDListOutput<long> idList = new IDListOutput<long>() { Ids = regDetailIdList };
            cmd.AddParameter("@RegistrationDetailsList", SqlDbType.Xml, ConvertNullObjectToDBNull(SerializeToXml<IDListOutput<long>>(idList)));

            IDataReader reader = ExecuteReader(cmd);
            List<PatientRegistrationDetail> retVal = null;

            try
            {
                retVal = GetPatientRegistrationDetailsCollectionFromReader(reader);
            }
            finally
            {
                reader.Close();
            }
            return retVal;
        }

        //---Dinh them 
        public override List<PatientRegistrationDetail> GetPatientRegistrationDetailsByDay(long DeptLocID)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = (SqlCommand)conn.CreateCommand();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spPatientRegistrationDetailsGetDay";

                cmd.AddParameter("@DeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocID));
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                IDataReader reader = ExecuteReader(cmd);
                List<PatientRegistrationDetail> retVal = null;

                try
                {
                    retVal = GetPatientRegistrationDetailsCollectionFromReader(reader);
                }
                finally
                {
                    reader.Close();
                }
                return retVal;
            }
        }

        public override List<PatientRegistrationDetail> GetPtRegisDetailsByConsultTimeSegment(long DeptLocID
                                                                                            , long V_TimeSegment
                                                                                            , long StartSequenceNumber
                                                                                            , long EndSequenceNumber)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = (SqlCommand)conn.CreateCommand();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spGetPtRegisDetailsByConsultTimeSegment";

                cmd.AddParameter("@DeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocID));
                cmd.AddParameter("@V_TimeSegment", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_TimeSegment));
                cmd.AddParameter("@StartSequenceNumber", SqlDbType.BigInt, ConvertNullObjectToDBNull(StartSequenceNumber));
                cmd.AddParameter("@EndSequenceNumber", SqlDbType.BigInt, ConvertNullObjectToDBNull(EndSequenceNumber));

                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                IDataReader reader = ExecuteReader(cmd);
                List<PatientRegistrationDetail> retVal = null;

                try
                {
                    retVal = GetPatientRegistrationDetailsCollectionFromReader(reader);
                }
                finally
                {
                    reader.Close();
                }
                return retVal;
            }
        }

        public override List<PatientRegistrationDetailEx> GetPatientRegistrationDetailsByRoom(out int totalCount, SeachPtRegistrationCriteria SeachRegCriteria)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = (SqlCommand)conn.CreateCommand();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spPatientRegistrationDetailsByRoomPaging";

                //cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SeachRegCriteria.DeptID));
                cmd.AddParameter("@DeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SeachRegCriteria.DeptLocationID));
                cmd.AddParameter("@BeginDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(SeachRegCriteria.FromDate));
                cmd.AddParameter("@EndDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(SeachRegCriteria.ToDate));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SeachRegCriteria.StaffID));

                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(SeachRegCriteria.pageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(SeachRegCriteria.pageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.VarChar, ConvertNullObjectToDBNull(""));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, true);


                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                IDataReader reader = ExecuteReader(cmd);
                List<PatientRegistrationDetailEx> retVal = null;

                try
                {
                    retVal = GetPatientRegistrationDetailsExCollectionFromReader(reader);
                }
                finally
                {
                    reader.Close();
                }
                if (cmd.Parameters["@Total"].Value != null)
                {
                    totalCount = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    totalCount = -1;
                return retVal;
            }
        }

        public override List<List<string>> ExportToExcelBangKeChiTietKhamBenh(SeachPtRegistrationCriteria SeachRegCriteria)
        {
            DataSet dsExportToExcelAll = new DataSet();
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = (SqlCommand)conn.CreateCommand();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spExportToExcelBangKeChiTietKhamBenh";

                cmd.AddParameter("@DeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SeachRegCriteria.DeptLocationID));
                cmd.AddParameter("@BeginDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(SeachRegCriteria.FromDate));
                cmd.AddParameter("@EndDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(SeachRegCriteria.ToDate));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SeachRegCriteria.StaffID));

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dsExportToExcelAll);
                List<List<string>> returnAllExcelData = new List<List<string>>();

                //Add the below 4 lines to add the column names to show on the Excel file
                List<string> colname = new List<string>();
                for (int i = 0; i <= dsExportToExcelAll.Tables[0].Columns.Count - 1; i++)
                {
                    colname.Add(dsExportToExcelAll.Tables[0].Columns[i].ToString().Trim());
                }

                returnAllExcelData.Add(colname);

                for (int i = 0; i <= dsExportToExcelAll.Tables[0].Rows.Count - 1; i++)
                {
                    List<string> rowData = new List<string>();
                    for (int j = 0; j <= dsExportToExcelAll.Tables[0].Columns.Count - 1; j++)
                    {
                        rowData.Add(Convert.ToString(dsExportToExcelAll.Tables[0].Rows[i][j]).Replace("<", "&lt;").Replace(">", "&gt;"));
                    }
                    returnAllExcelData.Add(rowData);
                }

                return returnAllExcelData;
            }
        }

        public override List<PatientRegistrationDetail> SearchRegistrationsForDiag(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            totalCount = 0;
            if (criteria == null)
            {
                return null;
            }

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSearchRegistrationsForDiag", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                var paramFullName = new SqlParameter("@FullName", SqlDbType.NVarChar) { Value = ConvertNullObjectToDBNull(criteria.FullName) };
                var paramFileCodeNumber = new SqlParameter("@FileCodeNumber", SqlDbType.VarChar) { Value = DBNull.Value };

                //KMx: Nếu 3 ký tự đầu của Full Name = "hs:" và sau dấu ":" còn ký tự khác nữa thì chữ "hs:" là do chương trình tạo ra.
                //Nếu sau dấu ":" không còn ký tự nào nữa thì chữ "hs:" là do người dùng nhập vào. Và tìm "hs:" theo Full Name (28/05/2014 14:45).
                if (criteria.FullName != null && criteria.FullName.Length >= 4
                    && criteria.FullName.ToLower().Substring(0, 3) == "hs:" && criteria.FullName.Substring(3) != "")
                {
                    paramFullName.Value = DBNull.Value;
                    paramFileCodeNumber.Value = ConvertNullObjectToDBNull(criteria.FullName.Substring(3));
                }

                cmd.Parameters.Add(paramFullName);
                cmd.Parameters.Add(paramFileCodeNumber);

                cmd.AddParameter("@PatientCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(criteria.PatientCode));
                cmd.AddParameter("@HealthInsuranceCard", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.HICard));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.StaffID));
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                cmd.AddParameter("@V_RegistrationStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.RegStatus));

                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptID));
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptLocationID));
                cmd.AddParameter("@IsHoanTat", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.IsHoanTat));

                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(pageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(pageIndex));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, ConvertNullObjectToDBNull(bCountTotal));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.OrderBy));
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);
                cmd.AddParameter("@PtRegistrationCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(criteria.PtRegistrationCode));
                if (!string.IsNullOrEmpty(criteria.PatientNameString) && criteria.PatientNameString.Length > 3 && new string[] { "PH", "PL", "QH", "QL" }.Contains(criteria.PatientNameString.Substring(0, 2))
                    && char.IsDigit(criteria.PatientNameString[3]))
                {
                    cmd.AddParameter("@PCLRequestNumID", SqlDbType.VarChar, ConvertNullObjectToDBNull(criteria.PatientNameString));
                }
                cn.Open();
                List<PatientRegistrationDetail> retVal = null;

                IDataReader reader = ExecuteReader(cmd);

                retVal = GetPatientRegistrationDetailsCollectionFromReader(reader);
                reader.Close();

                if (bCountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    totalCount = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    totalCount = -1;

                return retVal;
            }
        }


        /// <summary>
        /// Cập nhật các thuộc tính của 1 danh sách các chi tiết đăng ký của một đăng ký.
        /// Các thuộc tính được update bao gồm:
        ///                                V_ExamRegStatus
        ///                                MarkedAsDeleted
        ///                                PaidTime
        ///                                RefundTime
        /// </summary>
        /// <param name="registrationID"></param>
        /// <param name="regDetailList"></param>
        /// <param name="conn"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        public override bool UpdateRegistrationDetailsStatus(long registrationID, List<PatientRegistrationDetail> regDetailList, DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            SqlCommand cmd;

            cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spUpdateRegistrationDetailsStatus";

            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, registrationID);

            string detailListString = null;
            if (regDetailList != null && regDetailList.Count > 0)
            {
                detailListString = ConvertPatientRegistrationDetailsToXml(regDetailList).ToString();
            }

            cmd.AddParameter("@RegistrationDetails_Update", SqlDbType.Xml, ConvertNullObjectToDBNull(detailListString));

            int retVal = ExecuteNonQuery(cmd);

            return true;
        }


        public override bool UpdatePclRequestStatus(long registrationID, List<PatientPCLRequest> regPclList, DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            SqlCommand cmd;

            cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spUpdatePclRequestStatus";

            string pclRequestString = null;
            if (regPclList != null && regPclList.Count > 0)
            {
                XDocument xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                           new XElement("PatientPCLRequests",
                           from item in regPclList
                           select new XElement("PCLRequest",
                           new XElement("ID", item.PatientPCLReqID),
                           new XElement("V_PCLRequestStatus", (long)item.V_PCLRequestStatus),
                           new XElement("MarkedAsDeleted", item.RecordState == RecordState.DELETED ? true : false),
                           new XElement("RefundTime", item.RefundTime.HasValue ? item.RefundTime.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : ""),
                           new XElement("PCLRequestDetails",
                           from inner in item.PatientPCLRequestIndicators
                           select new XElement("Details",
                                new XElement("ID", inner.PCLReqItemID),
                                new XElement("V_ExamRegStatus", (long)inner.ExamRegStatus),
                                new XElement("MarkedAsDeleted", inner.RecordState == RecordState.DELETED ? true : false),
                                new XElement("RefundTime", inner.RefundTime.HasValue ? inner.RefundTime.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : ""))
                           ))));

                pclRequestString = xmlDocument.ToString();

            }
            cmd.AddParameter("@PatientPclRequestList", SqlDbType.Xml, ConvertNullObjectToDBNull(pclRequestString));

            int retVal = ExecuteNonQuery(cmd);

            return true;
        }

        public override bool UpdateProgSumMinusMinHIForRegistration(long registrationID, decimal minHi, decimal progSumMinusMinHi, out decimal curProgSumMinusMinHi, DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            SqlCommand cmd;

            cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spUpdateProgSumMinusMinHIForRegistration";

            cmd.AddParameter("@RegistrationID", SqlDbType.BigInt, registrationID);
            cmd.AddParameter("@MinHI", SqlDbType.Money, minHi);
            cmd.AddParameter("@ProgSumMinusMinHI", SqlDbType.Money, progSumMinusMinHi);
            cmd.AddParameter("@OutProgSumMinusMinHI", SqlDbType.Money, DBNull.Value, ParameterDirection.Output);

            int retVal = ExecuteNonQuery(cmd);

            decimal? prog = cmd.Parameters["@OutProgSumMinusMinHI"].Value as decimal?;
            curProgSumMinusMinHi = prog.GetValueOrDefault(0);
            return true;
        }
        public override bool CorrectHiRegistration(long registrationID, bool hiMinPayExceeded, DbConnection conn, DbTransaction tran, long? returnedOutInvoiceID = null)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            SqlCommand cmd;

            cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spCorrectHiRegistration";

            cmd.AddParameter("@RegistrationID", SqlDbType.BigInt, registrationID);
            cmd.AddParameter("@HiMinPayExceeded", SqlDbType.Bit, hiMinPayExceeded);
            cmd.AddParameter("@ReturnedOutiID", SqlDbType.BigInt, ConvertNullObjectToDBNull(returnedOutInvoiceID));

            int retVal = ExecuteNonQuery(cmd);

            return true;
        }

        public override bool CorrectRegistrationDetails(long registrationID, DbConnection conn, DbTransaction tran)
        {

            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            SqlCommand cmd;

            cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spCorrectRegistrationDetails";

            cmd.AddParameter("@RegistrationID", SqlDbType.BigInt, registrationID);

            int retVal = ExecuteNonQuery(cmd);

            return true;
        }

        public override decimal GetTotalPatientPayForTransaction(long transactionID)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                return GetTotalPatientPayForTransaction(transactionID);
            }
        }
        public override decimal GetTotalPatientPayForTransaction(long transactionID, DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            SqlCommand cmd;

            cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spGetTotalPatientPayForTransaction";

            cmd.AddParameter("@TransactionID", SqlDbType.BigInt, transactionID);

            object retVal = ExecuteScalar(cmd);
            if (retVal != null && retVal != DBNull.Value)
            {
                return (decimal)retVal;
            }
            return 0;
        }

        public override decimal GetTotalPatientPayForTransaction_InPt(long transactionID)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                return GetTotalPatientPayForTransaction_InPt(transactionID);
            }
        }
        public override decimal GetTotalPatientPayForTransaction_InPt(long transactionID, DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            SqlCommand cmd;

            cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spGetTotalPatientPayForTransaction_InPt";

            cmd.AddParameter("@TransactionID", SqlDbType.BigInt, transactionID);

            object retVal = ExecuteScalar(cmd);
            if (retVal != null && retVal != DBNull.Value)
            {
                return (decimal)retVal;
            }
            return 0;
        }

        public override AxServerConfigSection GetApplicationConfigValues()
        {
            AxServerConfigSection retVal = null;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllApplicationConfigValues", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                retVal = GetConfigSectionFromReader(reader);
                reader.Close();

                /*
                 * 22/06/2012 Txd: Accepted list of HI Hospital Codes is no longer required because
                 * this list is NOT definite as it was understood previously.
                cmd.CommandText = "spGetAllHiAcceptedCodeList";
                reader = ExecuteReader(cmd);
                retVal.HealthInsurances.CrossRegionCodeAcceptedList = GetHiAcceptedListFromReader(reader);
                 */
            }

            return retVal;

        }

        // Txd BEGIN ===================================

        public override bool BuildPCLExamTypeDeptLocMap()
        {
            dttest = DateTime.Now;
            MAPPCLExamTypeDeptLoc = (Dictionary<long, PCLExamType>)BuildPCLExamTypeDeptLocMapBase(true);
            if (MAPPCLExamTypeDeptLoc != null)
                return true;

            return false;
        }

        public override bool BuildPclDeptLocationList()
        {
            ListAllPCLExamTypeLocations = GetAllPclExamTypeLocations();
            if (ListAllPCLExamTypeLocations != null)
                return true;

            return false;
        }

        public override bool BuildAllServiceIdDeptLocMap()
        {
            MAPServiceIdAndDeptLocIDs = BuildAllServiceIdDeptLocMapBase();
            if (MAPServiceIdAndDeptLocIDs != null)
                return true;

            return false;
        }

        public override bool TestDatabaseConnectionOK()
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    cn.Open();
                    return true;
                }
            }
            catch (SqlException sqlEx)
            {
                AxLogger.Instance.LogError("TestDatabaseConnectionOK Exception Error: " + sqlEx.Message);
                return false;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError("TestDatabaseConnectionOK Exception Error: " + ex.Message);
                return false;
            }
            
        }

        // Txd END =================================== 


        public override Hospital SearchHospitalByHICode(string HiCode)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spHospital_GetByHICode", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@HICode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(HiCode));

                cn.Open();
                Hospital retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    retVal = GetHospitalFromReader(reader);
                }
                reader.Close();

                return retVal;
            }

        }
        
        public override List<Hospital> LoadCrossRegionHospitals()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetListCrossRegionHospital", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                List<Hospital> retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                retVal = GetHospitalCollectionFromReader(reader);
                reader.Close();
                return retVal;
            }
        }

        public override List<Hospital> SearchHospitals(string HospitalName, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            totalCount = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spHospital_Search", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@HosName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(HospitalName));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, pageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, pageSize);
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, bCountTotal);

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paramTotal);

                cn.Open();
                List<Hospital> retVal = null;

                IDataReader reader = ExecuteReader(cmd);

                retVal = GetHospitalCollectionFromReader(reader);
                reader.Close();

                if (bCountTotal && paramTotal.Value != null)
                {
                    totalCount = (int)paramTotal.Value;
                }
                else
                    totalCount = -1;
                return retVal;
            }

        }

        public override List<Hospital> SearchHospitalsNew(HospitalSearchCriteria entity, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            totalCount = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spHospital_SearchNew", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@HosName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.HosName));
                cmd.AddParameter("@IsPaperReferal", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.IsPaperReferal));
                cmd.AddParameter("@IsSearchAll", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.IsSearchAll));


                cmd.AddParameter("@PageIndex", SqlDbType.Int, pageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, pageSize);
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, bCountTotal);

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paramTotal);

                cn.Open();
                List<Hospital> retVal = null;

                IDataReader reader = ExecuteReader(cmd);

                retVal = GetHospitalCollectionFromReader(reader);
                reader.Close();

                if (bCountTotal && paramTotal.Value != null)
                {
                    totalCount = (int)paramTotal.Value;
                }
                else
                    totalCount = -1;
                return retVal;
            }

        }

        public override Hospital GetHospitalFromReader(IDataReader reader)
        {
            Hospital p = base.GetHospitalFromReader(reader);
            if (reader.HasColumn("HICode") && reader["HICode"] != DBNull.Value)
            {
                p.HICode = reader["HICode"] as string;
            }

            if (reader.HasColumn("HosAddress") && reader["HosAddress"] != DBNull.Value)
            {
                p.HosAddress = reader["HosAddress"] as string;
            }

            if (reader.HasColumn("HospitalCode") && reader["HospitalCode"] != DBNull.Value)
            {
                p.HospitalCode = reader["HospitalCode"] as string;
            }

            if (reader.HasColumn("HosShortName") && reader["HosShortName"] != DBNull.Value)
            {
                p.HosShortName = reader["HosShortName"] as string;
            }

            if (reader.HasColumn("V_HospitalType") && reader["V_HospitalType"] != DBNull.Value)
            {
                p.V_HospitalType = (long)reader["V_HospitalType"];
            }

            if (reader.HasColumn("CityProvinceID") && reader["CityProvinceID"] != DBNull.Value)
            {
                p.CityProvinceID = (long)reader["CityProvinceID"];
            }

            if (reader.HasColumn("CityProvinceName") && reader["CityProvinceName"] != DBNull.Value)
            {
                p.CityProvinceName = reader["CityProvinceName"] as string;
            }

            if (reader.HasColumn("CityProviceHICode") && reader["CityProviceHICode"] != DBNull.Value)
            {
                p.CityProvinceHICode = reader["CityProviceHICode"] as string;
            }

            if (reader.HasColumn("HosAddress") && reader["HosAddress"] != DBNull.Value)
            {
                p.HosAddress = reader["HosAddress"] as string;
            }

            if (reader.HasColumn("HosPhone") && reader["HosPhone"] != DBNull.Value)
            {
                p.HosPhone = reader["HosPhone"] as string;
            }

            if (reader.HasColumn("HosWebSite") && reader["HosWebSite"] != DBNull.Value)
            {
                p.HosWebSite = reader["HosWebSite"] as string;
            }
            return p;
        }


        public override List<RegistrationSummaryInfo> SearchRegistrationSummaryList(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount, out RegistrationsTotalSummary totalSummaryInfo)
        {
            totalCount = 0;
            totalSummaryInfo = null;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRegistrations_GetSummaryList", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DateFrom", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                cmd.AddParameter("@DateTo", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.StaffID));

                cmd.AddParameter("@V_RegistrationStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.RegStatus));
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.RegType));
                bool? isHi = null;
                if (criteria.HiChecked)
                {
                    isHi = true;
                }
                else if (criteria.ServiceChecked)
                {
                    isHi = false;
                }
                cmd.AddParameter("@HIFlag", SqlDbType.Bit, ConvertNullObjectToDBNull(isHi));


                cmd.AddParameter("@PageIndex", SqlDbType.Int, pageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, pageSize);
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, bCountTotal);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.OrderBy));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paramTotal);

                cn.Open();

                List<RegistrationSummaryInfo> retVal = null;
                IDataReader reader = ExecuteReader(cmd);

                retVal = GetRegistrationSummaryInfoCollectionFromReader(reader);
                if (reader.NextResult())
                {
                    if (reader.Read())
                    {
                        totalSummaryInfo = GetRegistrationsTotalSummaryFromReader(reader);
                    }
                }
                reader.Close();
                if (bCountTotal && paramTotal.Value != null)
                {
                    totalCount = (int)paramTotal.Value;
                }
                else
                    totalCount = -1;

                return retVal;
            }
        }
        public override bool CloseRegistration(long registrationID, AllLookupValues.RegistrationClosingStatus closingStatus, DbConnection conn, DbTransaction tran)
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                var cmd = new SqlCommand("spRegistrations_Close", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RegistrationID", SqlDbType.BigInt, registrationID);
                cmd.AddParameter("@V_RegistrationStatus", SqlDbType.BigInt, (long)AllLookupValues.RegistrationStatus.COMPLETED);
                cmd.AddParameter("@V_RegistrationClosingStatus", SqlDbType.BigInt, (long)closingStatus);

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                return retVal > 0;
            }
        }
        public override bool UpdateRegistrationStatus(PatientRegistration registration, DbConnection conn, DbTransaction tran)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("spRegistrations_UpdateStatus", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                    cmd.AddParameter("@RegistrationID", SqlDbType.BigInt, registration.PtRegistrationID);
                    cmd.AddParameter("@V_RegistrationStatus", SqlDbType.BigInt, (long)registration.RegistrationStatus);
                    cmd.AddParameter("@RegCancelStaffID", SqlDbType.BigInt, (long)registration.RegCancelStaffID);
                    //HPT 25/08/2015: Thêm parameter truyền loại đăng ký (nội/ngoại trú để phân biệt khi thực hiện update trong stored)
                    cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, (long)registration.V_RegistrationType);
                    //HPT 25/08/2015 END
                    cn.Open();

                    int retVal = ExecuteNonQuery(cmd);
                    return retVal > 0;
                }
                catch (SqlException ex)
                {
                    AxLogger.Instance.LogError(ex.ToString());
                    throw new Exception(ex.Message);
                }
            }
        }
        public override PaperReferal GetLatestPaperReferal(long hiid)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPaperReferal_GetLatestPaperReferalByHIID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@HIID", SqlDbType.BigInt, hiid);

                cn.Open();
                PaperReferal retVal = null;
                IDataReader reader = ExecuteReader(cmd);

                if (reader != null && reader.Read())
                {
                    retVal = GetPaperReferalFromReader(reader);

                }
                return retVal;
            }
        }

        /// <summary>
        /// Lấy tổng số công nợ của những bill của đăng ký nội trú chưa được QUYẾT TOÁN
        /// </summary>
        /// <param name="registrationID">Mã đăng ký</param>
        /// <param name="conn">Connection tới database</param>
        /// <param name="tran">DB transaction</param>
        /// <param name="TotalLiabilities">Tổng tiền công nợ</param>
        /// <param name="SumOfAdvance">Tổng tiền Bệnh nhân ứng trước</param>
        /// <param name="TotalPatientPayment_PaidInvoice">Tổng tiền Bệnh nhân cần trả cho những bill chưa được QUYẾT TOÁN và đã trả tiền rồi</param>
        /// <returns></returns>
        /// TxD 10/12/2014: Added parameter GetSumOfCashAdvBalanceOnly to query for total Cash Advance balance remaining ONLY and still calling this same method
        /// ie. The stored procedure sp_GetInPatientRegistrationNonFinalizedLiabilities has been modified to perform for this newly added parameter.
        public override bool GetInPatientRegistrationNonFinalizedLiabilities(long registrationID, bool GetSumOfCashAdvBalanceOnly, DbConnection conn, DbTransaction tran, out decimal TotalLiabilities, out decimal SumOfAdvance, out decimal TotalPatientPayment_PaidInvoice, out decimal TotalRefundPatient, out decimal TotalCashAdvBalanceAmount
                                                                            , out decimal TotalCharityOrgPayment, out decimal TotalPatientPayment_NotFinalized, out decimal TotalPatientPaid_NotFinalized, out decimal TotalCharityOrgPayment_NotFinalized )
        {
            TotalLiabilities = 0;
            SumOfAdvance = 0;
            TotalPatientPayment_PaidInvoice = 0;
            TotalRefundPatient = 0;
            TotalCashAdvBalanceAmount = 0;
            TotalCharityOrgPayment = 0;
            TotalPatientPayment_NotFinalized = 0;
            TotalPatientPaid_NotFinalized = 0;
            TotalCharityOrgPayment_NotFinalized = 0;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            SqlCommand cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sp_GetInPatientRegistrationNonFinalizedLiabilities";
            cmd.CommandTimeout = int.MaxValue;
            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, registrationID);
            cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, (long)AllLookupValues.RegistrationType.NOI_TRU);

            cmd.AddParameter("@GetSumOfCashAdvBalanceOnly", SqlDbType.Bit, (GetSumOfCashAdvBalanceOnly ? 1 : 0));

            IDataReader reader = ExecuteReader(cmd);

            if (reader == null)
                return false;
            
            if (reader.Read())
            {
                if (reader.HasColumn("TotalCashAdvBalanceAmount") && reader["TotalCashAdvBalanceAmount"] != DBNull.Value)
                {
                    TotalCashAdvBalanceAmount = (decimal)reader["TotalCashAdvBalanceAmount"];
                }

                if (GetSumOfCashAdvBalanceOnly)
                {
                    return true;
                }

                if (reader.HasColumn("TotalPatientPayment") && reader["TotalPatientPayment"] != DBNull.Value)
                {
                    TotalLiabilities = (decimal)reader["TotalPatientPayment"];
                }
                if (reader["TotalPatientPaid"] != DBNull.Value)
                {
                    SumOfAdvance = (decimal)reader["TotalPatientPaid"];
                }
                if (reader["TotalPatientPayment_PaidInvoice"] != DBNull.Value)
                {
                    TotalPatientPayment_PaidInvoice = (decimal)reader["TotalPatientPayment_PaidInvoice"];
                }
                if (reader.HasColumn("TotalRefundPatient") && reader["TotalRefundPatient"] != DBNull.Value)
                {
                    TotalRefundPatient = (decimal)reader["TotalRefundPatient"];
                }
                if (reader.HasColumn("TotalCharityOrgPayment") && reader["TotalCharityOrgPayment"] != DBNull.Value)
                {
                    TotalCharityOrgPayment = (decimal)reader["TotalCharityOrgPayment"];
                }
                if (reader.HasColumn("TotalPatientPayment_NotFinalized") && reader["TotalPatientPayment_NotFinalized"] != DBNull.Value)
                {
                    TotalPatientPayment_NotFinalized = (decimal)reader["TotalPatientPayment_NotFinalized"];
                }
                if (reader.HasColumn("TotalPatientPaid_NotFinalized") && reader["TotalPatientPaid_NotFinalized"] != DBNull.Value)
                {
                    TotalPatientPaid_NotFinalized = (decimal)reader["TotalPatientPaid_NotFinalized"];
                }
                if (reader.HasColumn("TotalCharityOrgPayment_NotFinalized") && reader["TotalCharityOrgPayment_NotFinalized"] != DBNull.Value)
                {
                    TotalCharityOrgPayment_NotFinalized = (decimal)reader["TotalCharityOrgPayment_NotFinalized"];
                }               
                reader.Close();
            }
            else
            {
                reader.Close();
                return false;
            }
            return true;
        }
        protected override OutwardDrugClinicDept GetOutwardDrugClinicDeptFromReader(IDataReader reader, long? staffID = null)
        {
            //Phai override lai vi ben Ny lay Qty y nghia khac
            OutwardDrugClinicDept item = base.GetOutwardDrugClinicDeptFromReader(reader, staffID);
            item.Qty = item.OutQuantity - item.QtyReturn;
            return item;
        }
        /// <summary>
        /// Lấy tổng số công nợ của những bill của đăng ký nội trú chưa được QUYẾT TOÁN
        /// </summary>
        /// <param name="registrationID">Mã đăng ký</param>
        /// <param name="TotalLiabilities">Tổng tiền công nợ</param>
        /// <param name="SumOfAdvance">Tổng tiền Bệnh nhân ứng trước</param>
        /// <param name="TotalPatientPayment_PaidInvoice">Tổng tiền Bệnh nhân cần trả cho những bill chưa được QUYẾT TOÁN và đã trả tiền rồi</param>
        /// <returns></returns>
        public override bool GetInPatientRegistrationNonFinalizedLiabilities(long registrationID, bool GetSumOfCashAdvBalanceOnly, out decimal TotalLiabilities, out decimal SumOfAdvance, out decimal TotalPatientPayment_PaidInvoice, out decimal TotalRefundPatient, out decimal TotalCashAdvBalanceAmount
                                                                            , out decimal TotalCharityOrgPayment, out decimal TotalPatientPayment_NotFinalized, out decimal TotalPatientPaid_NotFinalized, out decimal TotalCharityOrgPayment_NotFinalized )

        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                return GetInPatientRegistrationNonFinalizedLiabilities(registrationID, GetSumOfCashAdvBalanceOnly, cn, null, out TotalLiabilities, out SumOfAdvance, out TotalPatientPayment_PaidInvoice, out TotalRefundPatient, out TotalCashAdvBalanceAmount
                                                                        , out TotalCharityOrgPayment, out TotalPatientPayment_NotFinalized, out TotalPatientPaid_NotFinalized, out TotalCharityOrgPayment_NotFinalized);

            }
        }

        public override bool InsertInPatientAdmDisDetails(InPatientAdmDisDetails entity, long StaffID, long Staff_DeptLocationID, out long admissionID)
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                return InsertInPatientAdmDisDetails(entity, StaffID,  Staff_DeptLocationID, cn, null, out admissionID);
            }
        }

        public override bool InsertInPatientAdmDisDetails(InPatientAdmDisDetails entity, long StaffID, long Staff_DeptLocationID, DbConnection conn, DbTransaction tran, out long admissionID)
        {
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                SqlCommand cmd = (SqlCommand)conn.CreateCommand();
                cmd.Transaction = (SqlTransaction)tran;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spInPatientAdmDisDetails_Insert";

                cmd.AddParameter("@StaffID", SqlDbType.BigInt, StaffID);
                cmd.AddParameter("@Staff_DeptLocationID", SqlDbType.BigInt, Staff_DeptLocationID);


                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, entity.PtRegistrationID);
                cmd.AddParameter("@AdmissionDate", SqlDbType.DateTime, entity.AdmissionDate);
                long deptID = 0;

                deptID = entity.Department != null ? entity.Department.DeptID : entity.DeptID;

                cmd.AddParameter("@DeptID", SqlDbType.BigInt, deptID);

                // TxD 23/07/2014 : Added DeptLocationID
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, entity.DeptLocationID);

                cmd.AddParameter("@V_AdmissionType", SqlDbType.BigInt, entity.V_AdmissionType);
                cmd.AddParameter("@AdmissionNote", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.AdmissionNote));
                cmd.AddParameter("@DischargeDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.DischargeDate));
                cmd.AddParameter("@V_DischargeType", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_DischargeType));
                cmd.AddParameter("@DischargeNote", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DischargeNote));

                cmd.AddParameter("@HosTransferIn", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.HosTransferIn));
                cmd.AddParameter("@HosTransferInID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.HosTransferInID));
                cmd.AddParameter("@ReferralDiagnosis", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ReferralDiagnosis));
                cmd.AddParameter("@V_AccidentCode", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_AccidentCode));

                cmd.AddParameter("@InPatientAdmDisDetailID", SqlDbType.BigInt, null, ParameterDirection.Output);
                /*▼====: #007*/
                cmd.AddParameter("@IsConfirmEmergencyTreatment", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.IsConfirmEmergencyTreatment));
                /*▲====: #007*/
                int retVal = ExecuteNonQuery(cmd);

                admissionID = (long)cmd.Parameters["@InPatientAdmDisDetailID"].Value;

                return retVal > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public override bool UpdateInPatientDischargeInfo(InPatientAdmDisDetails entity, long StaffID, DbConnection conn, DbTransaction tran)
        {
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                SqlCommand cmd = (SqlCommand)conn.CreateCommand();
                cmd.Transaction = (SqlTransaction)tran;
                cmd.CommandType = CommandType.StoredProcedure;

                // TxD 29/03/2015 Added Temporary Discharge Date to allow for a temporary state of being discharge so the Discharge can still be updated 
                //                  and we can still create a new InPt registration to allow for new admission
                if (entity.TempDischargeDate.HasValue && entity.TempDischargeDate.Value > DateTime.MinValue)
                {
                    cmd.CommandText = "sp_UpdateInPatientTempDischarge";
                    cmd.AddParameter("@InPatientAdmDisDetailID", SqlDbType.BigInt, entity.InPatientAdmDisDetailID);
                    cmd.AddParameter("@TempDischargeDate", SqlDbType.DateTime, entity.TempDischargeDate);
                    int retValtmp = ExecuteNonQuery(cmd);
                    return true;
                }

                cmd.CommandText = "sp_UpdateInPatientDischargeInfo";

                long? dsNumber;
                if (entity.DeceasedInfo != null && entity.DeceasedInfo.DSNumber > 0)
                {
                    dsNumber = entity.DeceasedInfo.DSNumber;
                }
                else
                {
                    dsNumber = entity.DSNumber;
                }
                cmd.AddParameter("@DSNumber", SqlDbType.BigInt, ConvertNullObjectToDBNull(dsNumber));
                cmd.AddParameter("@InPatientAdmDisDetailID", SqlDbType.BigInt, entity.InPatientAdmDisDetailID);
                cmd.AddParameter("@DischargeDate", SqlDbType.DateTime, entity.DischargeDate);
                cmd.AddParameter("@DischargeDetailRecCreatedDate", SqlDbType.DateTime, entity.DischargeDetailRecCreatedDate);
                cmd.AddParameter("@V_DischargeType", SqlDbType.BigInt, entity.V_DischargeType);
                cmd.AddParameter("@DischargeNote", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DischargeNote));
                cmd.AddParameter("@DischargeCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(entity.DischargeCode));
                cmd.AddParameter("@DischargeNote2", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DischargeNote2));
                cmd.AddParameter("@DischargeCode2", SqlDbType.VarChar, ConvertNullObjectToDBNull(entity.DischargeCode2));
                cmd.AddParameter("@V_DischargeCondition", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VDischargeCondition));

                cmd.AddParameter("@Comment", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Comment));
                cmd.AddParameter("@DischargeDeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DischargeDepartment.DeptID));
                cmd.AddParameter("@Surgeon", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Surgeon));
                cmd.AddParameter("@Therapist", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Therapist));
                cmd.AddParameter("@ConfirmNotTreatedAsInPt", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ConfirmNotTreatedAsInPt));
                cmd.AddParameter("@HuongDieuTri", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.HuongDieuTri));
                cmd.AddParameter("@HosTransferOut", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.HosTransferOut));
                cmd.AddParameter("@HosTransferOutID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.HosTransferOutID));
                cmd.AddParameter("@OperationDoctor", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.OperationDoctor));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                cmd.AddParameter("@SurgeryDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.SurgeryDate));
                cmd.AddParameter("@CardiacCatheterDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.CardiacCatheterDate));
                /*▼====: #005*/
                cmd.AddParameter("@V_DeadReason", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_DeadReason));
                /*▲====: #005*/

                int retVal = ExecuteNonQuery(cmd);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public override bool RevertDischarge(InPatientAdmDisDetails entity, long staffID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spRevertDischarge", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@InPatientAdmDisDetailID", SqlDbType.BigInt, entity.InPatientAdmDisDetailID);
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, staffID);

                    cn.Open();

                    int retVal = ExecuteNonQuery(cmd);
                    return retVal > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public override bool CheckBeforeDischarge(long registrationID, long DischargeDeptID, bool ConfirmNotTreatedAsInPt, out string errorMessages, out string confirmMessages)
        {
            try
            {
                errorMessages = "";
                confirmMessages = "";

                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spCheckBeforeDischarge", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, registrationID);
                    cmd.AddParameter("@DischargeDeptID", SqlDbType.BigInt, DischargeDeptID);
                    cmd.AddParameter("@ConfirmNotTreatedAsInPt", SqlDbType.Bit, ConfirmNotTreatedAsInPt);

                    cn.Open();

                    IDataReader reader = ExecuteReader(cmd);

                    if (reader == null)
                    {
                        return false;
                    }

                    while (reader.Read())
                    {
                        if (reader.HasColumn("Message") && reader["Message"] != DBNull.Value && !string.IsNullOrEmpty(reader["Message"].ToString()))
                        {
                            if (reader.HasColumn("MessageType") && reader["MessageType"] != DBNull.Value && (byte)reader["MessageType"] == 1)
                            {
                                confirmMessages += reader["Message"].ToString() + Environment.NewLine;
                            }
                            else
                            {
                                errorMessages += reader["Message"].ToString() + Environment.NewLine;
                            }
                        }
                    }

                    reader.Close();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public override void CreateForm02(InPatientRptForm02 CurrentRptForm02, List<InPatientBillingInvoice> billingInvoices)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spCreateForm02", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@RptForm02_InPtID", SqlDbType.BigInt, CurrentRptForm02.RptForm02_InPtID);
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, CurrentRptForm02.PtRegistrationID);
                    cmd.AddParameter("@Description", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurrentRptForm02.Description));
                    cmd.AddParameter("@V_Form02Type", SqlDbType.BigInt, CurrentRptForm02.V_Form02Type);
                    cmd.AddParameter("@DeptID", SqlDbType.BigInt, CurrentRptForm02.Department.DeptID);
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, CurrentRptForm02.StaffID);
                    cmd.AddParameter("@Note", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurrentRptForm02.Note));

                    // Step 2: Create a List of IDs of the Billing Invoices
                    XDocument xmlDocument;
                    string billingInvoicesString = null;
                    if (billingInvoices != null && billingInvoices.Count > 0)
                    {
                        xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                                  new XElement("InPatientBillingInvoiceList",
                                  from item in billingInvoices
                                  select new XElement("IDs",
                                  new XElement("ID", item.InPatientBillingInvID)
                                  )));
                        billingInvoicesString = xmlDocument.ToString();
                    }

                    cmd.AddParameter("@BillingInvoiceIDList", SqlDbType.Xml, ConvertNullObjectToDBNull(billingInvoicesString));

                    cn.Open();

                    ExecuteNonQuery(cmd);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public override IList<InPatientRptForm02> GetForm02(long PtRegistrationID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetForm02", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = int.MaxValue;
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));

                    cn.Open();
                    IList<InPatientRptForm02> RptForm02List = null;
                    IDataReader reader = ExecuteReader(cmd);
                    RptForm02List = GetInPatientRptForm02CollectionFromReader(reader);
                    reader.Close();

                    return RptForm02List;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        /// <summary>
        /// Thêm mới, cập nhật, xóa những dịch vụ (KCB, CLS, thuốc...)
        /// </summary>
        /// <param name="registrationID">ID của đăng ký</param>
        /// <param name="newRegistrationDetailList">Những dịch vụ KCB cần thêm mới</param>
        /// <param name="modifiedRegistrationDetailList">Những dịch vụ KCB cần cập nhật</param>
        /// <param name="deletedRegistrationDetailList">Những dịch vụ KCB cần xóa</param>
        /// <param name="newPclRequestList">Những yêu cầu CLS cần thêm mới</param>
        /// <param name="modifiedPclRequestList">Những yêu cầu CLS cần cập nhật. Trong mỗi yêu cầu CLS này, 
        /// các chi tiết CLS có thể có những trạng thái như: DETACHED(thêm mới), MODIFIED(cập nhật), DELETED(xóa)</param>
        /// <param name="deletedPclRequestList">Những yêu cầu CLS cần xóa</param>
        /// <param name="modifiedTransactionString">Thông tin transaction bằng XML, gồm transaction, transaction details, payment</param>
        /// <param name="newInvoiceList">...Tương tự CLS</param>
        /// <param name="modifiedInvoiceList">...Tương tự CLS</param>
        /// <param name="deletedInvoiceList">...Tương tự CLS</param>
        /// <returns></returns>
        public bool AddUpdateServiceForRegistration_Old(long registrationID, long StaffID,
            bool ProgSumMinusMinHIModified, decimal? ProgSumMinusMinHINewValue,
            long PatientMedicalRecordID
                                    , List<PatientRegistrationDetail> newRegistrationDetailList
                                    , List<PatientRegistrationDetail> modifiedRegistrationDetailList
                                    , List<PatientRegistrationDetail> deletedRegistrationDetailList
                                    , List<PatientPCLRequest> newPclRequestList
                                    , List<PatientPCLRequest> modifiedPclRequestList
                                    , List<PatientPCLRequest> deletedPclRequestList

                                    , List<PatientPCLRequestDetail> lstDetail_ReUseServiceSeqNum

                                    , List<OutwardDrugInvoice> newInvoiceList
                                    , List<OutwardDrugInvoice> modifiedInvoiceList
                                    , List<OutwardDrugInvoice> deletedInvoiceList
                                    , string modifiedTransactionString
                                    , long V_RegistrationType
                                    , out List<long> newRegistrationDetailIdList
                                    , out List<long> newPclRequestIdList
                                    , out long newPatientTransactionID
                                    , out List<long> newPaymentIDList
                                    , out List<long> newOutwardDrugInvoiceIDList
                                    , out string PaymentIDListNy)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    var cmd = new SqlCommand("sp_AddUpdateServiceForRegistration", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, registrationID);
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, StaffID);

                    string detailListString = null;
                    if (newRegistrationDetailList != null && newRegistrationDetailList.Count > 0)
                    {
                        detailListString = ConvertPatientRegistrationDetailsToXml(newRegistrationDetailList).ToString();
                    }
                    cmd.AddParameter("@RegistrationDetails_Add", SqlDbType.Xml, ConvertNullObjectToDBNull(detailListString));

                    detailListString = null;
                    if (modifiedRegistrationDetailList != null && modifiedRegistrationDetailList.Count > 0)
                    {
                        detailListString = ConvertPatientRegistrationDetailsToXml(modifiedRegistrationDetailList).ToString();
                    }
                    cmd.AddParameter("@RegistrationDetails_Update", SqlDbType.Xml, ConvertNullObjectToDBNull(detailListString));

                    detailListString = null;
                    if (deletedRegistrationDetailList != null && deletedRegistrationDetailList.Count > 0)
                    {
                        detailListString = ConvertPatientRegistrationDetailsToXml(deletedRegistrationDetailList).ToString();
                    }
                    cmd.AddParameter("@RegistrationDetails_Delete", SqlDbType.Xml, ConvertNullObjectToDBNull(detailListString));

                    string pclRequestListString = null;
                    if (newPclRequestList != null && newPclRequestList.Count > 0)
                    {
                        pclRequestListString = ConvertPCLRequestsToXml(newPclRequestList, PatientMedicalRecordID).ToString();
                    }
                    cmd.AddParameter("@PclRequestList_Add", SqlDbType.Xml, ConvertNullObjectToDBNull(pclRequestListString));
                    pclRequestListString = null;
                    if (modifiedPclRequestList != null && modifiedPclRequestList.Count > 0)
                    {
                        pclRequestListString = ConvertPCLRequestsToXml(modifiedPclRequestList, PatientMedicalRecordID).ToString();
                    }
                    cmd.AddParameter("@PclRequestList_Update", SqlDbType.Xml, ConvertNullObjectToDBNull(pclRequestListString));
                    pclRequestListString = null;
                    if (deletedPclRequestList != null && deletedPclRequestList.Count > 0)
                    {
                        pclRequestListString = ConvertPCLRequestsToXml(deletedPclRequestList, PatientMedicalRecordID).ToString();
                    }

                    string lstDetail_ReUseServiceSeqNumString = null;
                    if (lstDetail_ReUseServiceSeqNum != null && lstDetail_ReUseServiceSeqNum.Count > 0)
                    {
                        lstDetail_ReUseServiceSeqNumString = ConvertPCLRequestsToXml_ReUseServiceSeqNum(lstDetail_ReUseServiceSeqNum, StaffID).ToString();
                    }

                    cmd.AddParameter("@lstDetail_ReUseServiceSeqNum", SqlDbType.Xml, ConvertNullObjectToDBNull(lstDetail_ReUseServiceSeqNumString));

                    cmd.AddParameter("@PclRequestList_Delete", SqlDbType.Xml, ConvertNullObjectToDBNull(pclRequestListString));
                    cmd.AddParameter("@ProgSumMinusMinHIModified", SqlDbType.Bit, ProgSumMinusMinHIModified);
                    cmd.AddParameter("@ProgSumMinusMinHINewValue", SqlDbType.Decimal, ConvertNullObjectToDBNull(ProgSumMinusMinHINewValue));


                    string outwardDrugInvoiceString = null;
                    if (newInvoiceList != null && newInvoiceList.Count > 0)
                    {
                        outwardDrugInvoiceString = ConvertOutwardDrugInvoicesToXml(newInvoiceList).ToString();
                    }

                    cmd.AddParameter("@DrugInvoiceList_Add", SqlDbType.Xml, ConvertNullObjectToDBNull(outwardDrugInvoiceString));
                    outwardDrugInvoiceString = null;
                    if (modifiedInvoiceList != null && modifiedInvoiceList.Count > 0)
                    {
                        outwardDrugInvoiceString = ConvertOutwardDrugInvoicesToXml(modifiedInvoiceList).ToString();
                    }
                    cmd.AddParameter("@DrugInvoiceList_Update", SqlDbType.Xml, ConvertNullObjectToDBNull(outwardDrugInvoiceString));
                    outwardDrugInvoiceString = null;
                    if (deletedInvoiceList != null && deletedInvoiceList.Count > 0)
                    {
                        outwardDrugInvoiceString = ConvertOutwardDrugInvoicesToXml(deletedInvoiceList).ToString();
                    }
                    cmd.AddParameter("@DrugInvoiceList_Delete", SqlDbType.Xml, ConvertNullObjectToDBNull(outwardDrugInvoiceString));

                    cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));

                    cmd.AddParameter("@PatientTransactionInfo", SqlDbType.Xml, ConvertNullObjectToDBNull(modifiedTransactionString));

                    cmd.AddParameter("@NewRegistrationDetailIDList", SqlDbType.Xml, DBNull.Value, ParameterDirection.Output);
                    cmd.AddParameter("@NewPclRequestListIDList", SqlDbType.Xml, DBNull.Value, ParameterDirection.Output);
                    cmd.AddParameter("@NewOutwardDrugInvoiceIDList", SqlDbType.Xml, DBNull.Value, ParameterDirection.Output);

                    cmd.AddParameter("@NewTransactionID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                    cmd.AddParameter("@NewPaymentIDList", SqlDbType.Xml, DBNull.Value, ParameterDirection.Output);
                    cmd.AddParameter("@PaymentIDListNy", SqlDbType.Xml, DBNull.Value, ParameterDirection.Output);
                    cmd.AddParameter("@ReturnValue", SqlDbType.Xml, DBNull.Value, ParameterDirection.ReturnValue);

                    conn.Open();

                    ExecuteNonQuery(cmd);

                    var idList = cmd.Parameters["@NewRegistrationDetailIDList"].Value as string;
                    var IDListOutput = DeserializeFromXml<IDListOutput<long>>(idList);
                    newRegistrationDetailIdList = IDListOutput != null ? IDListOutput.Ids : null;

                    idList = cmd.Parameters["@NewPclRequestListIDList"].Value as string;
                    IDListOutput = DeserializeFromXml<IDListOutput<long>>(idList);
                    newPclRequestIdList = IDListOutput != null ? IDListOutput.Ids : null;

                    idList = cmd.Parameters["@NewOutwardDrugInvoiceIDList"].Value as string;
                    IDListOutput = DeserializeFromXml<IDListOutput<long>>(idList);
                    newOutwardDrugInvoiceIDList = IDListOutput != null ? IDListOutput.Ids : null;

                    object newTranObj = cmd.Parameters["@NewTransactionID"].Value;
                    if (newTranObj == null || newTranObj == DBNull.Value)
                    {
                        newPatientTransactionID = 0;
                    }
                    else
                    {
                        newPatientTransactionID = (long)newTranObj;
                    }


                    idList = cmd.Parameters["@NewPaymentIDList"].Value as string;
                    IDListOutput = DeserializeFromXml<IDListOutput<long>>(idList);
                    newPaymentIDList = IDListOutput != null ? IDListOutput.Ids : null;

                    PaymentIDListNy = cmd.Parameters["@PaymentIDListNy"].Value as string;

                    object retVal = cmd.Parameters["@ReturnValue"].Value;
                    if (retVal != null && retVal != DBNull.Value)
                    {
                        return (int)retVal == 1;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogError(ex);
                    newRegistrationDetailIdList = null;
                    newPclRequestIdList = null;
                    newPatientTransactionID = 0;
                    newPaymentIDList = null;
                    newOutwardDrugInvoiceIDList = null;
                    PaymentIDListNy = "";
                    return false;
                }
            }
        }

        public override PatientPCLRequest UpdateDrAndDiagTrmtForPCLReq(long ServiceRecID, long PCLReqID, long DoctorStaffID)
        {
            PatientPCLRequest UpdatedReq = new PatientPCLRequest();
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                try
                {
                    SqlCommand cmd;

                    cmd = (SqlCommand)conn.CreateCommand();
                    cmd.CommandText = "spUpdateDrAndDiagTrmtForPCLReq";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@ServiceRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ServiceRecID));
                    cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLReqID));
                    cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DoctorStaffID));

                    IDataReader reader = ExecuteReader(cmd);
                    if (reader != null && reader.Read())
                    {
                        UpdatedReq = GetPatientPCLRequestFromReader(reader);
                        reader.Close();
                    }
                    return UpdatedReq;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }


        public override bool AddUpdateServiceForRegistration(long registrationID, long StaffID,
            long CollectorDeptLocID,
            bool ProgSumMinusMinHIModified, decimal? ProgSumMinusMinHINewValue,
            long PatientMedicalRecordID
            , List<PatientRegistrationDetail> newRegistrationDetailList
            , List<PatientRegistrationDetail> modifiedRegistrationDetailList
            , List<PatientRegistrationDetail> deletedRegistrationDetailList
            , List<PatientPCLRequest> newPclRequestList
            , List<PatientPCLRequest> modifiedPclRequestList
            , List<PatientPCLRequest> deletedPclRequestList
            , List<PatientPCLRequestDetail> lstDetail_ReUseServiceSeqNum
            , List<OutwardDrugInvoice> newInvoiceList
            , List<OutwardDrugInvoice> modifiedInvoiceList
            , List<OutwardDrugInvoice> deletedInvoiceList
            , string modifiedTransactionString
            , long V_RegistrationType
            , out List<long> newRegistrationDetailIdList
            , out List<long> newPclRequestIdList
            , out long newPatientTransactionID
            , out List<long> newPaymentIDList
            , out List<long> newOutwardDrugInvoiceIDList
            , out string PaymentIDListNy
            , PromoDiscountProgram aPromoDiscountProgram
            , long? ConfirmHIStaffID = null
            , string OutputBalanceServicesXML = null
            , bool IsZeroValueHIConfirm = false
            , bool IsReported = false
            , bool IsUpdateHisID = false
            , long? HIID = null
            , double? PtInsuranceBenefit = null)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                // =====▼ #008               
                DbTransaction tran = conn.BeginTransaction();
                //DbTransaction tran = null;
                // =====▲ #008
                try
                {
                    SqlCommand cmd;

                    cmd = (SqlCommand)conn.CreateCommand();
                    // =====▼ #008
                    cmd.Transaction = (SqlTransaction)tran;
                    // =====▲ #008
                    cmd.CommandText = "sp_AddUpdateServiceForRegistration_New";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = int.MaxValue;

                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, registrationID);
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, StaffID);

                    string detailListString = null;
                    if (newRegistrationDetailList != null && newRegistrationDetailList.Count > 0)
                    {
                        detailListString = ConvertPatientRegistrationDetailsToXml(newRegistrationDetailList, StaffID).ToString();
                    }
                    cmd.AddParameter("@RegistrationDetails_Add", SqlDbType.Xml, ConvertNullObjectToDBNull(detailListString));

                    detailListString = null;
                    if (modifiedRegistrationDetailList != null && modifiedRegistrationDetailList.Count > 0)
                    {
                        detailListString = ConvertPatientRegistrationDetailsToXml(modifiedRegistrationDetailList, StaffID).ToString();
                    }
                    cmd.AddParameter("@RegistrationDetails_Update", SqlDbType.Xml, ConvertNullObjectToDBNull(detailListString));

                    detailListString = null;
                    if (deletedRegistrationDetailList != null && deletedRegistrationDetailList.Count > 0)
                    {
                        detailListString = ConvertPatientRegistrationDetailsToXml(deletedRegistrationDetailList, StaffID).ToString();
                    }
                    cmd.AddParameter("@RegistrationDetails_Delete", SqlDbType.Xml, ConvertNullObjectToDBNull(detailListString));

                    string pclRequestListString = null;
                    if (newPclRequestList != null && newPclRequestList.Count > 0)
                    {
                        pclRequestListString = ConvertPCLRequestsToXml(newPclRequestList, PatientMedicalRecordID, StaffID).ToString();
                    }
                    cmd.AddParameter("@PclRequestList_Add", SqlDbType.Xml, ConvertNullObjectToDBNull(pclRequestListString));
                    pclRequestListString = null;
                    if (modifiedPclRequestList != null && modifiedPclRequestList.Count > 0)
                    {
                        pclRequestListString = ConvertPCLRequestsToXml(modifiedPclRequestList, PatientMedicalRecordID, StaffID).ToString();
                    }
                    cmd.AddParameter("@PclRequestList_Update", SqlDbType.Xml, ConvertNullObjectToDBNull(pclRequestListString));
                    pclRequestListString = null;
                    if (deletedPclRequestList != null && deletedPclRequestList.Count > 0)
                    {
                        pclRequestListString = ConvertPCLRequestsToXml(deletedPclRequestList, PatientMedicalRecordID, StaffID).ToString();
                    }

                    string lstDetail_ReUseServiceSeqNumString = null;
                    if (lstDetail_ReUseServiceSeqNum != null && lstDetail_ReUseServiceSeqNum.Count > 0)
                    {
                        lstDetail_ReUseServiceSeqNumString = ConvertPCLRequestsToXml_ReUseServiceSeqNum(lstDetail_ReUseServiceSeqNum, StaffID).ToString();
                    }

                    cmd.AddParameter("@lstDetail_ReUseServiceSeqNum", SqlDbType.Xml, ConvertNullObjectToDBNull(lstDetail_ReUseServiceSeqNumString));

                    cmd.AddParameter("@PclRequestList_Delete", SqlDbType.Xml, ConvertNullObjectToDBNull(pclRequestListString));
                    cmd.AddParameter("@ProgSumMinusMinHIModified", SqlDbType.Bit, ProgSumMinusMinHIModified);
                    cmd.AddParameter("@ProgSumMinusMinHINewValue", SqlDbType.Decimal, ConvertNullObjectToDBNull(ProgSumMinusMinHINewValue));


                    string outwardDrugInvoiceString = null;
                    if (newInvoiceList != null && newInvoiceList.Count > 0)
                    {
                        outwardDrugInvoiceString = ConvertOutwardDrugInvoicesToXml(newInvoiceList).ToString();
                    }

                    cmd.AddParameter("@DrugInvoiceList_Add", SqlDbType.Xml, ConvertNullObjectToDBNull(outwardDrugInvoiceString));
                    outwardDrugInvoiceString = null;
                    if (modifiedInvoiceList != null && modifiedInvoiceList.Count > 0)
                    {
                        outwardDrugInvoiceString = ConvertOutwardDrugInvoicesToXml(modifiedInvoiceList).ToString();
                    }
                    cmd.AddParameter("@DrugInvoiceList_Update", SqlDbType.Xml, ConvertNullObjectToDBNull(outwardDrugInvoiceString));
                    outwardDrugInvoiceString = null;
                    if (deletedInvoiceList != null && deletedInvoiceList.Count > 0)
                    {
                        outwardDrugInvoiceString = ConvertOutwardDrugInvoicesToXml(deletedInvoiceList).ToString();
                    }
                    cmd.AddParameter("@DrugInvoiceList_Delete", SqlDbType.Xml, ConvertNullObjectToDBNull(outwardDrugInvoiceString));

                    cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));

                    cmd.AddParameter("@PatientTransactionInfo", SqlDbType.Xml, ConvertNullObjectToDBNull(modifiedTransactionString));

                    cmd.AddParameter("@NewRegistrationDetailIDList", SqlDbType.Xml, DBNull.Value, ParameterDirection.Output);
                    cmd.AddParameter("@NewPclRequestListIDList", SqlDbType.Xml, DBNull.Value, ParameterDirection.Output);
                    cmd.AddParameter("@NewOutwardDrugInvoiceIDList", SqlDbType.Xml, DBNull.Value, ParameterDirection.Output);

                    cmd.AddParameter("@NewTransactionID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                    cmd.AddParameter("@ListOutput", SqlDbType.Xml, DBNull.Value, ParameterDirection.Output);
                    cmd.AddParameter("@ListOutputExt", SqlDbType.Xml, DBNull.Value, ParameterDirection.Output);
                    cmd.AddParameter("@ReturnValue", SqlDbType.Xml, DBNull.Value, ParameterDirection.ReturnValue);

                    if (aPromoDiscountProgram != null)
                    {
                        cmd.AddParameter("@PromoDiscountProgram", SqlDbType.Xml, ConvertNullObjectToDBNull(aPromoDiscountProgram.ConvertToXml()));
                    }
                    cmd.AddParameter("@ConfirmHIStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ConfirmHIStaffID));
                    if (IsZeroValueHIConfirm)
                    {
                        cmd.AddParameter("@OutPtCashAdvanceReceiptNumber", SqlDbType.VarChar, ConvertNullObjectToDBNull(GetOutPtCashAdvReceiptNum((int)AllLookupValues.PatientFindBy.NGOAITRU)));
                    }
                    if (IsUpdateHisID && HIID > 0)
                    {
                        long HisID = ActiveHisID(new HealthInsurance { HIID = HIID.Value });
                        if (HisID > 0)
                        {
                            cmd.AddParameter("@IsUpdateHisID", SqlDbType.Bit, ConvertNullObjectToDBNull(IsUpdateHisID));
                            cmd.AddParameter("@HisID", SqlDbType.BigInt, ConvertNullObjectToDBNull(HisID));
                            cmd.AddParameter("@PtInsuranceBenefit", SqlDbType.Float, ConvertNullObjectToDBNull(PtInsuranceBenefit));
                        }
                    }
                    ExecuteNonQuery(cmd);

                    var idList = cmd.Parameters["@NewRegistrationDetailIDList"].Value as string;
                    var IDListOutput = DeserializeFromXml<IDListOutput<long>>(idList);
                    newRegistrationDetailIdList = IDListOutput != null ? IDListOutput.Ids : null;

                    idList = cmd.Parameters["@NewPclRequestListIDList"].Value as string;
                    IDListOutput = DeserializeFromXml<IDListOutput<long>>(idList);
                    newPclRequestIdList = IDListOutput != null ? IDListOutput.Ids : null;

                    idList = cmd.Parameters["@NewOutwardDrugInvoiceIDList"].Value as string;
                    IDListOutput = DeserializeFromXml<IDListOutput<long>>(idList);
                    newOutwardDrugInvoiceIDList = IDListOutput != null ? IDListOutput.Ids : null;

                    object newTranObj = cmd.Parameters["@NewTransactionID"].Value;
                    if (newTranObj == null || newTranObj == DBNull.Value)
                    {
                        newPatientTransactionID = 0;
                    }
                    else
                    {
                        newPatientTransactionID = (long)newTranObj;
                    }


                    //idList = cmd.Parameters["@NewPaymentIDList"].Value as string;
                    //IDListOutput = DeserializeFromXml<IDListOutput<long>>(idList);
                    //newPaymentIDList = IDListOutput != null ? IDListOutput.Ids : null;


                    string ListOutputExt = null;
                    if (cmd.Parameters["@ListOutputExt"].Value != null && cmd.Parameters["@ListOutputExt"].Value != DBNull.Value)
                    {
                        ListOutputExt = cmd.Parameters["@ListOutputExt"].Value as string;
                    }
                    bool bOK = AddListPayment(CollectorDeptLocID, cmd.Parameters["@ListOutput"].Value as string, out PaymentIDListNy, out newPaymentIDList, (int)AllLookupValues.PatientFindBy.NGOAITRU, conn, tran, Globals.AxServerSettings.CommonItems.EnableOutPtCashAdvance, registrationID, OutputBalanceServicesXML, ConfirmHIStaffID, ListOutputExt, IsReported);

                    if (!bOK)
                    {
                        // =====▼ #008                                       
                        if (tran != null)
                            tran.Rollback();
                        // =====▲ #008
                        return false;
                    }

                    object retVal = cmd.Parameters["@ReturnValue"].Value;
                    if (retVal != null && retVal != DBNull.Value)
                    {
                        // =====▼ #008                                       
                        tran.Commit();
                        // =====▲ #008
                        return (int)retVal == 1;
                    }

                    // =====▼ #008                                   
                    tran.Commit();
                    // =====▲ #008
                    return true;
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogError(ex);
                    // =====▼ #008                                   
                    if (tran != null)
                        tran.Rollback();
                    // =====▲ #008

                    newRegistrationDetailIdList = null;
                    newPclRequestIdList = null;
                    newPatientTransactionID = 0;
                    newPaymentIDList = null;
                    newOutwardDrugInvoiceIDList = null;
                    PaymentIDListNy = "";
                    //return false;
                    throw;
                }
            }
        }

        private bool AddListPayment(long CollectorDeptLocID, string ListOutput, out string PaymentIDListNy, out List<long> newPaymentIDList, int bNgoaiTru, DbConnection conn, DbTransaction tran)
        {
            return AddListPayment(CollectorDeptLocID, ListOutput, out PaymentIDListNy, out newPaymentIDList, bNgoaiTru, conn, tran);
        }
        private string GetOutPtCashAdvReceiptNum(int aNgoaiTru)
        {
            return new ServiceSequenceNumberProvider().GetReceiptNumber_NgoaiTru(aNgoaiTru);
        }
        private bool AddListPayment(long CollectorDeptLocID, string ListOutput, out string PaymentIDListNy, out List<long> newPaymentIDList, int bNgoaiTru, DbConnection conn, DbTransaction tran, bool IsCashAdvance = false, long PtRegistrationID = 0, string OutputBalanceServicesXML = null
            , long? ConfirmHIStaffID = null
            , string ListOutputExt = null
            , bool IsReported = false)
        {
            PaymentIDListNy = null;
            newPaymentIDList = null;
            List<PatientTransactionPayment> ListPaymentIDAndService = new List<PatientTransactionPayment>();
            List<PatientTransactionPayment> ListPaymentIDAndServiceExt = new List<PatientTransactionPayment>();
            if (!string.IsNullOrEmpty(ListOutput) || !string.IsNullOrEmpty(ListOutputExt))
            {
                if (!string.IsNullOrEmpty(ListOutput))
                {
                    XDocument xdoc1 = XDocument.Parse(ListOutput);
                    var items = from _student in xdoc1.Element("Root").Elements("IDList") select _student;
                    foreach (var _student in items)
                    {
                        PatientTransactionPayment p = new PatientTransactionPayment();
                        p.PtPmtAccID = string.IsNullOrEmpty(_student.Element("PtPmtAccID").Value) ? 0 : Convert.ToInt64(_student.Element("PtPmtAccID").Value);
                        p.InvoiceID = _student.Element("InvoiceID").Value;
                        p.TransactionID = string.IsNullOrEmpty(_student.Element("TransactionID").Value) ? 0 : Convert.ToInt64(_student.Element("TransactionID").Value);
                        p.IntRcptTypeID = string.IsNullOrEmpty(_student.Element("IntRcptTypeID").Value) ? 0 : Convert.ToInt64(_student.Element("IntRcptTypeID").Value);
                        p.TranFinalizationID = string.IsNullOrEmpty(_student.Element("TranFinalizationID").Value) ? 0 : Convert.ToInt64(_student.Element("TranFinalizationID").Value);
                        p.PaymentDate = string.IsNullOrEmpty(_student.Element("PaymentDate").Value) ? DateTime.Now : Convert.ToDateTime(_student.Element("PaymentDate").Value);
                        p.PayAmount = Convert.ToDecimal(_student.Element("PayAmount").Value);
                        p.ManualReceiptNumber = _student.Element("ManualReceiptNumber").Value;
                        p.V_Currency = string.IsNullOrEmpty(_student.Element("V_Currency").Value) ? 0 : Convert.ToInt64(_student.Element("V_Currency").Value);
                        p.V_PaymentType = string.IsNullOrEmpty(_student.Element("V_PaymentType").Value) ? 0 : Convert.ToInt64(_student.Element("V_PaymentType").Value);
                        p.V_PaymentMode = string.IsNullOrEmpty(_student.Element("V_PaymentMode").Value) ? 0 : Convert.ToInt64(_student.Element("V_PaymentMode").Value);
                        p.CreditOrDebit = Convert.ToInt16(_student.Element("CreditOrDebit").Value);
                        p.StaffID = Convert.ToInt64(_student.Element("StaffID").Value);
                        p.CollectorDeptLocID = CollectorDeptLocID;
                        p.TranPaymtNote = _student.Element("TranPaymtNote").Value;
                        p.TranPaymtStatus = Convert.ToInt16(_student.Element("TranPaymtStatus").Value);
                        p.V_TradingPlaces = Convert.ToInt64(_student.Element("V_TradingPlaces").Value);
                        p.XMLLink = _student.Element("TranItemIDXML").ToString().Replace("<TranItemIDXML>", "").Replace("</TranItemIDXML>", "");
                        p.XMLService = _student.Element("ServiceXML").ToString().Replace("<ServiceXML>", "").Replace("</ServiceXML>", "");
                        p.DiscountAmount = _student.Element("DiscountAmount") == null || string.IsNullOrEmpty(_student.Element("DiscountAmount").Value) ? 0 : Convert.ToDecimal(_student.Element("DiscountAmount").Value);
                        ListPaymentIDAndService.Add(p);
                    }
                }

                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                SqlCommand cmd;

                cmd = (SqlCommand)conn.CreateCommand();
                cmd.Transaction = (SqlTransaction)tran;
                cmd.CommandText = "spPatientTransactionPayment_InsertXML";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = int.MaxValue;

                //lam sao tra lai so khi bi loi day
                cmd.AddParameter("@PaymentXML", SqlDbType.Xml, GetPatientTransactionPaymentToXML(ListPaymentIDAndService, bNgoaiTru, IsCashAdvance).ToString().Replace("&lt;", "<").Replace("&gt;", ">"));
                cmd.AddParameter("@PaymentIDListNy", SqlDbType.Xml, DBNull.Value, ParameterDirection.Output);
                cmd.AddParameter("@PaymentIDList", SqlDbType.Xml, DBNull.Value, ParameterDirection.Output);

                cmd.AddParameter("@IsCashAdvance", SqlDbType.Bit, ConvertNullObjectToDBNull(IsCashAdvance));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));

                if (IsCashAdvance && ListPaymentIDAndService != null && ListPaymentIDAndService.Sum(x => x.PayAmount) != 0)
                {
                    cmd.AddParameter("@OutPtCashAdvanceReceiptNumber", SqlDbType.VarChar, ConvertNullObjectToDBNull(GetOutPtCashAdvReceiptNum(bNgoaiTru)));
                }

                cmd.AddParameter("@ConfirmHIStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ConfirmHIStaffID));
                cmd.AddParameter("@BalanceServicesXML", SqlDbType.Xml, ConvertNullObjectToDBNull(OutputBalanceServicesXML));
                cmd.AddParameter("@IsReported", SqlDbType.Bit, ConvertNullObjectToDBNull(IsReported));

                if (!string.IsNullOrEmpty(ListOutputExt))
                {
                    XDocument xdoc2 = XDocument.Parse(ListOutputExt);
                    var items2 = from _student in xdoc2.Element("Root").Elements("IDList")
                                 select _student;
                    foreach (var _student in items2)
                    {
                        PatientTransactionPayment p = new PatientTransactionPayment();
                        p.PtPmtAccID = string.IsNullOrEmpty(_student.Element("PtPmtAccID").Value) ? 0 : Convert.ToInt64(_student.Element("PtPmtAccID").Value);
                        p.InvoiceID = _student.Element("InvoiceID").Value;
                        p.TransactionID = string.IsNullOrEmpty(_student.Element("TransactionID").Value) ? 0 : Convert.ToInt64(_student.Element("TransactionID").Value);
                        p.IntRcptTypeID = string.IsNullOrEmpty(_student.Element("IntRcptTypeID").Value) ? 0 : Convert.ToInt64(_student.Element("IntRcptTypeID").Value);
                        p.TranFinalizationID = string.IsNullOrEmpty(_student.Element("TranFinalizationID").Value) ? 0 : Convert.ToInt64(_student.Element("TranFinalizationID").Value);
                        p.PaymentDate = string.IsNullOrEmpty(_student.Element("PaymentDate").Value) ? DateTime.Now : Convert.ToDateTime(_student.Element("PaymentDate").Value);
                        p.PayAmount = Convert.ToDecimal(_student.Element("PayAmount").Value);
                        p.ManualReceiptNumber = _student.Element("ManualReceiptNumber").Value;
                        p.V_Currency = string.IsNullOrEmpty(_student.Element("V_Currency").Value) ? 0 : Convert.ToInt64(_student.Element("V_Currency").Value);
                        p.V_PaymentType = string.IsNullOrEmpty(_student.Element("V_PaymentType").Value) ? 0 : Convert.ToInt64(_student.Element("V_PaymentType").Value);
                        p.V_PaymentMode = string.IsNullOrEmpty(_student.Element("V_PaymentMode").Value) ? 0 : Convert.ToInt64(_student.Element("V_PaymentMode").Value);
                        p.CreditOrDebit = Convert.ToInt16(_student.Element("CreditOrDebit").Value);
                        p.StaffID = Convert.ToInt64(_student.Element("StaffID").Value);
                        p.CollectorDeptLocID = CollectorDeptLocID;
                        p.TranPaymtNote = _student.Element("TranPaymtNote").Value;
                        p.TranPaymtStatus = Convert.ToInt16(_student.Element("TranPaymtStatus").Value);
                        p.V_TradingPlaces = Convert.ToInt64(_student.Element("V_TradingPlaces").Value);
                        p.XMLLink = _student.Element("TranItemIDXML").ToString().Replace("<TranItemIDXML>", "").Replace("</TranItemIDXML>", "");
                        p.XMLService = _student.Element("ServiceXML").ToString().Replace("<ServiceXML>", "").Replace("</ServiceXML>", "");
                        p.DiscountAmount = _student.Element("DiscountAmount") == null || string.IsNullOrEmpty(_student.Element("DiscountAmount").Value) ? 0 : Convert.ToDecimal(_student.Element("DiscountAmount").Value);
                        ListPaymentIDAndServiceExt.Add(p);
                    }
                    cmd.AddParameter("@PaymentXMLExt", SqlDbType.Xml, GetPatientTransactionPaymentToXML(ListPaymentIDAndServiceExt, bNgoaiTru, IsCashAdvance).ToString().Replace("&lt;", "<").Replace("&gt;", ">"));
                    if (IsCashAdvance && ListPaymentIDAndServiceExt != null && ListPaymentIDAndServiceExt.Sum(x => x.PayAmount) != 0)
                    {
                        cmd.AddParameter("@OutPtCashAdvanceReceiptNumberExt", SqlDbType.VarChar, ConvertNullObjectToDBNull(GetOutPtCashAdvReceiptNum(bNgoaiTru)));
                    }
                }

                ExecuteNonQuery(cmd);

                var idList = cmd.Parameters["@PaymentIDList"].Value as string;
                var IDListOutput = DeserializeFromXml<IDListOutput<long>>(idList);
                newPaymentIDList = IDListOutput != null ? IDListOutput.Ids : null;

                PaymentIDListNy = cmd.Parameters["@PaymentIDListNy"].Value as string;
                if (newPaymentIDList != null && newPaymentIDList.Count > 0)
                {
                    return true;
                }
                else if (Globals.AxServerSettings.CommonItems.EnableOutPtCashAdvance && IsCashAdvance && PtRegistrationID > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public override bool SavePCLRequestsForInPt(long registrationID, long StaffID
                                ,long PatientMedicalRecordID
                                , long ReqFromDeptLocID
                                , long ReqFromDeptID
                                , List<PatientPCLRequest> newPclRequestList
                                , PatientPCLRequest deletedPclRequest
                                , List<PatientPCLRequestDetail> lstDetail_ReUseServiceSeqNum
                                , long V_RegistrationType
                                , out List<long> newPclRequestIdList)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                try
                {
                    SqlCommand cmd;
                    cmd = (SqlCommand)conn.CreateCommand();
                    cmd.CommandText = "sp_AddPCLRequestAndDetails_InPt";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = int.MaxValue;
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, registrationID);
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, StaffID);
                    string pclRequestListString = null;
                    if (newPclRequestList != null && newPclRequestList.Count > 0)
                    {
                        pclRequestListString = ConvertPCLRequestsToXml(newPclRequestList, PatientMedicalRecordID, StaffID).ToString();
                    }
                    string pclRequestDetailString = null;
                    if (deletedPclRequest != null && deletedPclRequest.PatientPCLRequestIndicators.Count > 0)
                    {
                        pclRequestDetailString = ConvertPCLRequestDetailsToXml(deletedPclRequest.PatientPCLRequestIndicators).ToString();
                    }
                    string lstDetail_ReUseServiceSeqNumString = null;
                    if (lstDetail_ReUseServiceSeqNum != null && lstDetail_ReUseServiceSeqNum.Count > 0)
                    {
                        lstDetail_ReUseServiceSeqNumString = ConvertPCLRequestsToXml_ReUseServiceSeqNum(lstDetail_ReUseServiceSeqNum, StaffID).ToString();
                    }
                    cmd.AddParameter("@ReqFromDeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ReqFromDeptLocID));
                    cmd.AddParameter("@ReqFromDeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ReqFromDeptID));
                    cmd.AddParameter("@PtMedicalRecordID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientMedicalRecordID));
                    cmd.AddParameter("@PclRequestList_Add", SqlDbType.Xml, ConvertNullObjectToDBNull(pclRequestListString));
                    cmd.AddParameter("@lstDetail_ReUseServiceSeqNum", SqlDbType.Xml, ConvertNullObjectToDBNull(lstDetail_ReUseServiceSeqNumString));
                    cmd.AddParameter("@PclRequestDetails_Delete", SqlDbType.Xml, ConvertNullObjectToDBNull(pclRequestDetailString));   
                    cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));
                    cmd.AddParameter("@InsertedIDList", SqlDbType.Xml, DBNull.Value, ParameterDirection.Output);
                    ExecuteNonQuery(cmd);
                    var idList = cmd.Parameters["@InsertedIDList"].Value as string;
                    var IDListOutput = DeserializeFromXml<IDListOutput<long>>(idList);
                    newPclRequestIdList = IDListOutput != null ? IDListOutput.Ids : null;            
                    return true;
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogError(ex);

                    newPclRequestIdList = null;
                    //return false;
                    throw;
                }
            }
        }
        public override bool AddUpdateBillingInvoices(long registrationID
                                   , long patientMedicalRecordID
                                   , List<InPatientBillingInvoice> newInvoiceList
                                   , List<InPatientBillingInvoice> modifiedInvoiceList
                                   , List<InPatientBillingInvoice> deletedInvoiceList
                                   , string modifiedTransactionString
                                   , long V_RegistrationType
                                   , out List<long> newBillingInvoiceList
                                   , out long newPatientTransactionID
                                   , out List<long> newPaymentIDList)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("sp_SaveInPatientBillingInvoiceList", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = int.MaxValue;

                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, registrationID);

                string billingInvoiceString = null;
                if (newInvoiceList != null && newInvoiceList.Count > 0)
                {
                    billingInvoiceString = ConvertBillingInvoicesToXml(newInvoiceList, patientMedicalRecordID).ToString();
                    //billingInvoiceString = AxHelper.ConvertObjectToXml(newInvoiceList);
                }

                cmd.AddParameter("@BillingInvoiceList_Add", SqlDbType.Xml, ConvertNullObjectToDBNull(billingInvoiceString));
                billingInvoiceString = null;
                if (modifiedInvoiceList != null && modifiedInvoiceList.Count > 0)
                {
                    billingInvoiceString = ConvertBillingInvoicesToXml(modifiedInvoiceList, patientMedicalRecordID).ToString();
                    //string strTemp = AxHelper.ConvertObjectToPlainXML(modifiedInvoiceList);
                    //billingInvoiceString = AxHelper.ConvertObjectToXml(modifiedInvoiceList);
                }

                cmd.AddParameter("@BillingInvoiceList_Update", SqlDbType.Xml, ConvertNullObjectToDBNull(billingInvoiceString));
                billingInvoiceString = null;
                if (deletedInvoiceList != null && deletedInvoiceList.Count > 0)
                {
                    billingInvoiceString = ConvertBillingInvoicesToXml(deletedInvoiceList, patientMedicalRecordID).ToString();
                    //billingInvoiceString = AxHelper.ConvertObjectToXml(deletedInvoiceList);
                }
                cmd.AddParameter("@BillingInvoiceList_Delete", SqlDbType.Xml, ConvertNullObjectToDBNull(billingInvoiceString));

                cmd.AddParameter("@PatientTransactionInfo", SqlDbType.Xml, ConvertNullObjectToDBNull(modifiedTransactionString));

                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));

                cmd.AddParameter("@PtMedicalRecordID", SqlDbType.BigInt, ConvertNullObjectToDBNull(patientMedicalRecordID));

                cmd.AddParameter("@NewBillingInvoiceIDList", SqlDbType.Xml, DBNull.Value, ParameterDirection.Output);

                cmd.AddParameter("@NewTransactionID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                cmd.AddParameter("@NewPaymentIDList", SqlDbType.Xml, DBNull.Value, ParameterDirection.Output);

                cmd.AddParameter("@ReturnValue", SqlDbType.Xml, DBNull.Value, ParameterDirection.ReturnValue);

                conn.Open();

                ExecuteNonQuery(cmd);

                var idList = cmd.Parameters["@NewBillingInvoiceIDList"].Value as string;
                var IDListOutput = DeserializeFromXml<IDListOutput<long>>(idList);
                newBillingInvoiceList = IDListOutput != null ? IDListOutput.Ids : null;

                object newTranObj = cmd.Parameters["@NewTransactionID"].Value;
                if (newTranObj == null || newTranObj == DBNull.Value)
                {
                    newPatientTransactionID = 0;
                }
                else
                {
                    newPatientTransactionID = (long)newTranObj;
                }

                idList = cmd.Parameters["@NewPaymentIDList"].Value as string;
                IDListOutput = DeserializeFromXml<IDListOutput<long>>(idList);
                newPaymentIDList = IDListOutput != null ? IDListOutput.Ids : null;

                object retVal = cmd.Parameters["@ReturnValue"].Value;
                if (retVal != null && retVal != DBNull.Value)
                {
                    return (int)retVal == 1;
                }
                return false;
            }
        }
        //public override bool UpdateInPatientAdmDisDetails(InPatientAdmDisDetails entity, DeptLocation newDeptLoc, DbConnection conn, DbTransaction tran)
        //{
        //    try
        //    {
        //        if (conn.State != ConnectionState.Open)
        //        {
        //            conn.Open();
        //        }

        //        SqlCommand cmd = (SqlCommand)conn.CreateCommand();
        //        cmd.Transaction = (SqlTransaction)tran;
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.CommandText = "sp_UpdateInPatientAdmDisDetails";

        //        cmd.AddParameter("@InPatientAdmDisDetailID", SqlDbType.BigInt, entity.InPatientAdmDisDetailID);
        //        long deptID = 0;
        //        deptID = entity.Department != null ? entity.Department.DeptID : entity.DeptID;
        //        cmd.AddParameter("@DeptID", SqlDbType.BigInt, deptID);
        //        cmd.AddParameter("@DeptLocID", SqlDbType.BigInt, newDeptLoc.DeptLocationID);

        //        cmd.AddParameter("@AdmissionDate", SqlDbType.DateTime, entity.AdmissionDate);

        //        cmd.AddParameter("@V_AdmissionType", SqlDbType.BigInt, entity.V_AdmissionType);
        //        cmd.AddParameter("@AdmissionNote", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.AdmissionNote));

        //        cmd.AddParameter("@HosTransferIn", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.HosTransferIn));
        //        cmd.AddParameter("@HosTransferInID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.HosTransferInID));
        //        cmd.AddParameter("@ReferralDiagnosis", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ReferralDiagnosis));

        //        int retVal = ExecuteNonQuery(cmd);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}


        public override bool UpdateInPatientAdmDisDetails(InPatientAdmDisDetails entity, DeptLocation newDeptLoc)
        {
            try
            {

                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_UpdateInPatientAdmDisDetails", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@InPatientAdmDisDetailID", SqlDbType.BigInt, entity.InPatientAdmDisDetailID);
                    long deptID = 0;
                    deptID = entity.Department != null ? entity.Department.DeptID : entity.DeptID;
                    cmd.AddParameter("@DeptID", SqlDbType.BigInt, deptID);
                    cmd.AddParameter("@DeptLocID", SqlDbType.BigInt, newDeptLoc.DeptLocationID);

                    cmd.AddParameter("@AdmissionDate", SqlDbType.DateTime, entity.AdmissionDate);

                    cmd.AddParameter("@V_AdmissionType", SqlDbType.BigInt, entity.V_AdmissionType);
                    cmd.AddParameter("@AdmissionNote", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.AdmissionNote));

                    cmd.AddParameter("@HosTransferIn", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.HosTransferIn));
                    cmd.AddParameter("@HosTransferInID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.HosTransferInID));
                    cmd.AddParameter("@ReferralDiagnosis", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ReferralDiagnosis));
                    cmd.AddParameter("@V_AccidentCode", SqlDbType.BigInt, entity.V_AccidentCode);
                    /*▼====: #007*/
                    cmd.AddParameter("@IsConfirmEmergencyTreatment", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.IsConfirmEmergencyTreatment));
                    /*▲====: #007*/
                    cn.Open();
                    int count = cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        public override InPatientAdmDisDetails GetInPatientAdmissionByID(long admissionId)
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                return GetInPatientAdmissionByID(admissionId, cn, null);
            }
        }
        public override InPatientAdmDisDetails GetInPatientAdmissionByID(long admissionId, DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            SqlCommand cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sp_GetInPatientAdmDisDetailsByID";

            cmd.AddParameter("@InPatientAdmDisDetailID", SqlDbType.BigInt, admissionId);
            InPatientAdmDisDetails retVal = null;
            IDataReader reader = ExecuteReader(cmd);
            if (reader != null)
            {
                if (reader.Read())
                {
                    retVal = GetInPatientAdmissionFromReader(reader);
                }
                reader.Close();
            }
            return retVal;
        }
        public override bool ChangeInPatientDept(InPatientDeptDetail entity, out long inPatientDeptDetailID)
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                return ChangeInPatientDept(entity, cn, null, out inPatientDeptDetailID);
            }
        }
        public override bool ChangeInPatientDept(InPatientDeptDetail entity, DbConnection conn, DbTransaction tran, out long inPatientDeptDetailID)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            SqlCommand cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sp_ChangeInPatientDept";

            cmd.AddParameter("@InPatientAdmDisDetailID", SqlDbType.BigInt, entity.InPatientAdmDisDetailID);
            long deptLocId = 0;
            deptLocId = entity.DeptLocation != null ? entity.DeptLocation.DeptLocationID : entity.DeptLocID;
            cmd.AddParameter("@DeptLocID", SqlDbType.BigInt, deptLocId);

            cmd.AddParameter("@FromDate", SqlDbType.DateTime, entity.FromDate);
            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.ToDate));
            cmd.AddParameter("@V_InPatientDeptStatus", SqlDbType.BigInt, entity.V_InPatientDeptStatus);
            cmd.AddParameter("@InPatientDeptDetailID", SqlDbType.BigInt, null, ParameterDirection.Output);

            int retVal = ExecuteNonQuery(cmd);

            inPatientDeptDetailID = (long)cmd.Parameters["@InPatientDeptDetailID"].Value;

            return retVal > 0;
        }
        public override bool UpdateAppointmentStatus(long appointmentID, long createdPtRegistrationID, AllLookupValues.ApptStatus newStatus)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                var cmd = (SqlCommand)conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "sp_PatientAppointments_UpdateStatus";

                //KMx: Thêm cột CreatedPtRegistrationID để biết cuộc hẹn tạo ra đăng ký nào (10/11/2014 10:56).
                cmd.AddParameter("@AppointmentID", SqlDbType.BigInt, appointmentID);
                cmd.AddParameter("@CreatedPtRegistrationID", SqlDbType.BigInt, createdPtRegistrationID);
                cmd.AddParameter("@V_ApptStatus", SqlDbType.BigInt, (long)newStatus);

                var retVal = ExecuteNonQuery(cmd);

                return retVal > 0;
            }
        }
        public override bool InsertInPatientDeptDetails(InPatientDeptDetail entity, out long inPatientDeptDetailID)
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                return InsertInPatientDeptDetails(entity, cn, null, out inPatientDeptDetailID);
            }
        }
        public override bool InsertInPatientDeptDetails(InPatientDeptDetail entity, DbConnection conn, DbTransaction tran, out long inPatientDeptDetailID)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            SqlCommand cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sp_InsertInPatientDeptDetails";

            cmd.AddParameter("@InPatientAdmDisDetailID", SqlDbType.BigInt, entity.InPatientAdmDisDetailID);
            long deptLocId = 0;
            deptLocId = entity.DeptLocation != null ? entity.DeptLocation.DeptLocationID : entity.DeptLocID;
            cmd.AddParameter("@DeptLocID", SqlDbType.BigInt, deptLocId);

            cmd.AddParameter("@FromDate", SqlDbType.DateTime, entity.FromDate);
            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.ToDate));
            cmd.AddParameter("@V_InPatientDeptStatus", SqlDbType.BigInt, entity.V_InPatientDeptStatus);
            cmd.AddParameter("@InPatientDeptDetailID", SqlDbType.BigInt, null, ParameterDirection.Output);

            int retVal = ExecuteNonQuery(cmd);

            inPatientDeptDetailID = (long)cmd.Parameters["@InPatientDeptDetailID"].Value;

            return retVal > 0;
        }
        public override InPatientAdmDisDetails GetInPatientAdmissionByRegistrationID(long registration, DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            SqlCommand cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sp_GetInPatientAdmDisDetailsByRegistrationID";

            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, registration);
            InPatientAdmDisDetails retVal = null;
            IDataReader reader = ExecuteReader(cmd);
            if (reader != null)
            {
                if (reader.Read())
                {
                    retVal = GetInPatientAdmissionFromReader(reader);
                }
                reader.Close();
            }

            if (retVal == null)
            {
                return null;
            }

                cmd.CommandText = "sp_GetAllInPatientDeptDetailsByAdmissionID";
            cmd.Parameters.Clear();
            cmd.AddParameter("@InPatientAdmDisDetailID", SqlDbType.BigInt, retVal.InPatientAdmDisDetailID);
            reader = ExecuteReader(cmd);
            retVal.InPatientDeptDetails = GetInPatientDeptDetailCollectionFromReader(reader).ToObservableCollection();
            reader.Close();

            // Identify which row or record is the one created during Admission (that record is called the admitted record
            Int64 minDeptDetailID = Int64.MaxValue;
            Int64 admDeptDetailID = 0;
            foreach (var admItem in retVal.InPatientDeptDetails)
            {
                if (admItem.InPatientDeptDetailID < minDeptDetailID)
                {
                    minDeptDetailID = admItem.InPatientDeptDetailID;
                    admDeptDetailID = admItem.InPatientDeptDetailID;
                }
            }
            foreach (var admItem in retVal.InPatientDeptDetails)
            {
                if (admItem.InPatientDeptDetailID == admDeptDetailID)
                {
                    admItem.IsAdmittedRecord = true;
                    break;
                }
            }

            //Lay thong tin tu vong neu co.
            if (retVal.DSNumber.GetValueOrDefault(-1) > 0)
            {
                cmd.CommandText = "sp_GetDeceasedInfoByID";
                cmd.Parameters.Clear();
                cmd.AddParameter("@DSNumber", SqlDbType.BigInt, retVal.DSNumber.Value);
                reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    retVal.DeceasedInfo = GetDeceasedInfoFromReader(reader);
                }
                reader.Close();
            }
            
            return retVal;
        }

        public override List<InPatientAdmDisDetails> InPatientAdmDisDetailsSearch(InPatientAdmDisDetailSearchCriteria p, DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            SqlCommand cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spInPatientAdmDisDetailsSearch";

            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, p.PtRegistrationID);
            cmd.AddParameter("@DeptID", SqlDbType.BigInt, p.DeptID);
            cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, p.DeptLocationID);
            cmd.AddParameter("@AdmissionDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(p.AdmissionDate));
            cmd.AddParameter("@V_AdmissionType", SqlDbType.BigInt, p.V_AdmissionType);
            cmd.AddParameter("@DischargeDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(p.DischargeDate));
            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(p.FromDate));
            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(p.ToDate));

            //cmd.AddParameter("@V_DischargeType", SqlDbType.BigInt, p.V_DischargeType);



            List<InPatientAdmDisDetails> retVal = null;
            IDataReader reader = ExecuteReader(cmd);
            if (reader != null)
            {
                retVal = GetInPatientAdmDisDetailsCollectionFromReader(reader);

                reader.Close();
            }

            return retVal;
        }

        public override List<InPatientAdmDisDetails> InPatientAdmDisDetailsSearchPaging(InPatientAdmDisDetailSearchCriteria p, int pageIndex, int pageSize, bool bCountTotal, out int totalCount
            , DbConnection conn, DbTransaction tran)
        {
            totalCount = 0;

            SqlCommand cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spInPatientAdmDisDetailsSearchPaging";


            cmd.AddParameter("@FullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(p.FullName));
            cmd.AddParameter("@PatientCode", SqlDbType.Char, ConvertNullObjectToDBNull(p.PatientCode));
            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, p.PtRegistrationID);
            cmd.AddParameter("@DeptID", SqlDbType.BigInt, p.DeptID);
            cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, p.DeptLocationID);
            cmd.AddParameter("@AdmissionDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(p.AdmissionDate));
            cmd.AddParameter("@V_AdmissionType", SqlDbType.BigInt, p.V_AdmissionType);
            cmd.AddParameter("@DischargeDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(p.DischargeDate));
            cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(p.FromDate));
            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(p.ToDate));

            cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(pageSize));
            cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(pageIndex));
            cmd.AddParameter("@OrderBy", SqlDbType.VarChar, ConvertNullObjectToDBNull(""));
            cmd.AddParameter("@CountTotal", SqlDbType.Bit, ConvertNullObjectToDBNull(bCountTotal));


            cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);



            List<InPatientAdmDisDetails> retVal = null;
            IDataReader reader = ExecuteReader(cmd);
            if (reader != null)
            {
                retVal = GetInPatientAdmDisDetailsCollectionFromReader(reader);

                reader.Close();
            }


            if (bCountTotal && cmd.Parameters["@Total"].Value != null)
            {
                totalCount = (int)cmd.Parameters["@Total"].Value;
            }
            else
                totalCount = -1;

            return retVal;

        }

        public override DateTime? BedPatientAlloc_GetLatestBillToDate(long BedPatientAllocID, DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            var cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sp_BedPatientAlloc_GetLatestBillToDate";

            cmd.AddParameter("@BedPatientID", SqlDbType.BigInt, BedPatientAllocID);

            object retVal = ExecuteScalar(cmd);

            return retVal as DateTime?;
        }
        public override bool AddReportOutPatientCashReceiptList(List<ReportOutPatientCashReceipt> items)
        {
            if (items == null || items.Count == 0)
            {
                return false;
            }
            using (var cn = new SqlConnection(ConnectionString))
            {
                cn.Open();
                var cmd = (SqlCommand)cn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spReportOutPatientCashReceipt_AddList";
                var doc = ConvertReportOutPatientCashReceiptToXml(items);
                cmd.AddParameter("@ReportOutPatientCashReceiptList", SqlDbType.Xml, doc.ToString());

                var retVal = ExecuteNonQuery(cmd);
                return true;
            }
        }
        public override bool UpdatePatientRegistrationStatus(long registrationId, int FindPatient, AllLookupValues.RegistrationStatus newStatus, DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            var cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sp_UpdatePatientRegistrationStatus";

            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, registrationId);
            cmd.AddParameter("@V_RegistrationStatus", SqlDbType.BigInt, (long)newStatus);
            cmd.AddParameter("@FindPatient", SqlDbType.Int, FindPatient);

            int retVal = ExecuteNonQuery(cmd);

            return retVal > 0;
        }

        public override List<BedPatientRegDetail> GetAllBedPatientRegDetailsByBedPatientID(long BedPatientId, bool IncludeDeletedItems, DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            var cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sp_GetAllBedPatientRegDetailsByBedPatientID";

            cmd.AddParameter("@BedPatientID", SqlDbType.BigInt, BedPatientId);
            cmd.AddParameter("@IncludeCancelledItem", SqlDbType.Bit, IncludeDeletedItems);

            IDataReader reader = ExecuteReader(cmd);
            List<BedPatientRegDetail> retVal = null;
            if (reader != null)
            {
                retVal = GetBedPatientRegDetailCollectionFromReader(reader);
                reader.Close();
            }

            return retVal;
        }
        public override List<BedPatientRegDetail> GetAllBedPatientRegDetailsByBedPatientID(long BedPatientId, bool IncludeDeletedItems)
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                return GetAllBedPatientRegDetailsByBedPatientID(BedPatientId, IncludeDeletedItems, cn, null);
            }
        }

         //* TxD 11/01/2018: Added new parameter ConfirmHICardForInPt_Types to enable joining HI Card
        public override bool ApplyHiToInPatientRegistration(long RegistrationID, long HIID, long HisID, double HiBenefit, bool IsCrossRegion, long? PaperReferalID, int FindPatient,
                        bool bIsEmergency, long ConfirmHiStaffID, bool IsHICard_FiveYearsCont, bool IsChildUnder6YearsOld, bool IsHICard_FiveYearsCont_NoPaid = false, DateTime? FiveYearsAppliedDate = null, TypesOf_ConfirmHICardForInPt eConfirmType = TypesOf_ConfirmHICardForInPt.eConfirmNew)
        {
            if (RegistrationID <= 0)
            {
                return false;
            }
            using (var cn = new SqlConnection(ConnectionString))
            {
                cn.Open();
                var cmd = cn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "sp_ApplyHiToInPatientRegistration";

                cmd.AddParameter("@RegistrationID", SqlDbType.BigInt, RegistrationID);
                //cmd.AddParameter("@HIID", SqlDbType.BigInt, HIID);
                cmd.AddParameter("@HisID", SqlDbType.BigInt, HisID);
                cmd.AddParameter("@HiBenefit", SqlDbType.Decimal, HiBenefit);
                cmd.AddParameter("@IsCrossRegion", SqlDbType.Bit, IsCrossRegion);
                //HPT 22/08/2015: Thêm parameter cho Stored
                cmd.AddParameter("@IsHICard_FiveYearsCont", SqlDbType.Bit, IsHICard_FiveYearsCont);
                /*==== #002 ====*/
                cmd.AddParameter("@IsHICard_FiveYearsCont_NoPaid", SqlDbType.Bit, IsHICard_FiveYearsCont_NoPaid);
                cmd.AddParameter("@FiveYearsAppliedDate", SqlDbType.DateTime, FiveYearsAppliedDate);
                /*==== #002 ====*/
                cmd.AddParameter("@IsChildUnder6YearsOld", SqlDbType.Bit, IsChildUnder6YearsOld);
                //HPT 22/08/2015
                cmd.AddParameter("@IsEmergency", SqlDbType.Bit, bIsEmergency);
                cmd.AddParameter("@PaperReferalID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PaperReferalID));
                cmd.AddParameter("@FindPatient", SqlDbType.Int, ConvertNullObjectToDBNull(FindPatient));
                cmd.AddParameter("@ConfirmHiStaffID", SqlDbType.BigInt, ConfirmHiStaffID);
                cmd.AddParameter("@TypesOfConfirmHICard", SqlDbType.SmallInt, Convert.ToInt16(eConfirmType));

                var retVal = ExecuteNonQuery(cmd);
                return retVal > 0;
            }
        }

        public override bool RemoveHiFromInPatientRegistration(long RegistrationID, bool bIsEmergency, long RemoveHiStaffID, long? OldPaperReferalID)
        {
            if (RegistrationID <= 0)
            {
                return false;
            }
            using (var cn = new SqlConnection(ConnectionString))
            {
                cn.Open();
                var cmd = cn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "sp_RemoveHiFromInPatientRegistration";

                cmd.AddParameter("@RegistrationID", SqlDbType.BigInt, RegistrationID);
                cmd.AddParameter("@IsEmergency", SqlDbType.Bit, bIsEmergency);                
                cmd.AddParameter("@RemoveHiStaffID", SqlDbType.BigInt, RemoveHiStaffID);
                cmd.AddParameter("@OldPaperReferalID", SqlDbType.BigInt, ConvertNullObjectToDBNull(OldPaperReferalID));

                var retVal = ExecuteNonQuery(cmd);
                return retVal > 0;
            }
            
        }

        public override bool InsertAdditionalFee(long InPatientBillingInvID, long StaffID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spInsertAdditionalFee", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@InPatientBillingInvID", SqlDbType.BigInt, InPatientBillingInvID);
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, StaffID);

                    cn.Open();

                    var retVal = ExecuteNonQuery(cmd);
                    return retVal > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        #region InPatientTransferDeptReq
        public override List<InPatientTransferDeptReq> GetInPatientTransferDeptReq(InPatientTransferDeptReq p)
        {
            List<InPatientTransferDeptReq> listRG = new List<InPatientTransferDeptReq>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spInPatientTransferDeptReqGet", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@InPatientTransferDeptReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.InPatientTransferDeptReqID));
                cmd.AddParameter("@InPatientAdmDisDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.InPatientAdmDisDetailID));
                cmd.AddParameter("@ReqDeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.ReqDeptLocID));
                cmd.AddParameter("@ReqStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.ReqStaffID));
                cmd.AddParameter("@ReqDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(p.ReqDate));
                cmd.AddParameter("@CurDeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.CurDeptID));
                cmd.AddParameter("@ReqDeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.ReqDeptID));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.PtRegistrationID));
                cmd.AddParameter("@IsAccepted", SqlDbType.Bit, ConvertNullObjectToDBNull(p.IsAccepted));

                cmd.AddParameter("@DataXML", SqlDbType.Xml, ConvertNullObjectToDBNull(InPatientDeptID_ConvertListToXml(p.LstRefDepartment)));
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                listRG = GetInPatientTransferDeptReqCollectionFromReader(reader);
                reader.Close();
                return listRG;
            }
        }
        public override bool InsertInPatientTransferDeptReq(InPatientTransferDeptReq p)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spInPatientTransferDeptReqInsert", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@InPatientAdmDisDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.InPatientAdmDisDetailID));
                    cmd.AddParameter("@ReqDeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.ReqDeptLocID));
                    cmd.AddParameter("@ReqStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.ReqStaffID));
                    cmd.AddParameter("@ReqDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(p.ReqDate));
                    cmd.AddParameter("@CurDeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.CurDeptID));
                    cmd.AddParameter("@ReqDeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.ReqDeptID));

                    //cmd.AddParameter("@V_InPatientDeptStatus", SqlDbType.BigInt, AllLookupValues.InPatientDeptStatus.CHUYEN_KHOA);
                    cn.Open();
                    int val = cmd.ExecuteNonQuery();
                    if (val > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public override bool UpdateInPatientTransferDeptReq(InPatientTransferDeptReq p)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spInPatientTransferDeptReqUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@InPatientTransferDeptReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.InPatientTransferDeptReqID));
                cmd.AddParameter("@InPatientAdmDisDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.InPatientAdmDisDetailID));
                cmd.AddParameter("@ReqDeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.ReqDeptLocID));
                cmd.AddParameter("@ReqStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.ReqStaffID));
                cmd.AddParameter("@ReqDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(p.ReqDate));

                cmd.AddParameter("@CurDeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.CurDeptID));
                cmd.AddParameter("@ReqDeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.ReqDeptID));
                cn.Open();
                int val = cmd.ExecuteNonQuery();
                if (val > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override bool DeleteInPatientTransferDeptReq(long InPatientTransferDeptReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spInPatientTransferDeptReqDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@InPatientTransferDeptReqID", SqlDbType.BigInt, InPatientTransferDeptReqID);

                cn.Open();
                int val = cmd.ExecuteNonQuery();
                if (val > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        
        public override bool UpdateDeleteInPatientDeptDetails(InPatientDeptDetail inDeptDetailToDelete, InPatientDeptDetail inDeptDetailToUpdate, out InPatientDeptDetail inDeptDetailUpdated)
        {
            inDeptDetailUpdated = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUpdateOrDeleteInPatientDeptDetails", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@InPatientDeptDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(inDeptDetailToUpdate.InPatientDeptDetailID));
                    cmd.AddParameter("@DeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(inDeptDetailToUpdate.DeptLocID));
                    cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(inDeptDetailToUpdate.FromDate));
                    cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(inDeptDetailToUpdate.ToDate));
                    cmd.AddParameter("@V_InPatientDeptStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(inDeptDetailToUpdate.V_InPatientDeptStatus));

                    cn.Open();

                    cmd.ExecuteNonQuery();

                }

                return true;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public override bool InPatientDeptDetailsTranfer(InPatientDeptDetail p, long InPatientTransferDeptReqID
            , out long InPatientDeptDetailID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spInsertInPatientDeptDetails", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@InPatientTransferDeptReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(InPatientTransferDeptReqID));
                    cmd.AddParameter("@InPatientAdmDisDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.InPatientAdmDisDetailID));
                    cmd.AddParameter("@DeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.DeptLocID));
                    cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(p.FromDate));
                    cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(p.ToDate));
                    cmd.AddParameter("@V_InPatientDeptStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.V_InPatientDeptStatus));
                    cmd.AddParameter("@IsTemp", SqlDbType.Bit, ConvertNullObjectToDBNull(p.IsTemp));


                    //KMx: Đặt giường (07/09/2014 11:06).
                    SqlParameter paramBedAllocationID = new SqlParameter("@BedAllocationID", SqlDbType.BigInt);
                    SqlParameter paramPtRegistrationID = new SqlParameter("@PtRegistrationID", SqlDbType.BigInt);
                    SqlParameter paramResponsibleDeptID = new SqlParameter("@ResponsibleDeptID", SqlDbType.BigInt);
                    SqlParameter paramCheckInDate = new SqlParameter("@CheckInDate", SqlDbType.DateTime);
                    SqlParameter paramExpectedStayingDays = new SqlParameter("@ExpectedStayingDays", SqlDbType.SmallInt);
                    SqlParameter paramCheckOutDate = new SqlParameter("@CheckOutDate", SqlDbType.DateTime);
                    SqlParameter paramIsActive = new SqlParameter("@IsActive", SqlDbType.Bit);
                    SqlParameter paramPatientInBed = new SqlParameter("@PatientInBed", SqlDbType.Bit);

                    if (p.BedAllocation != null && p.BedAllocation.BedAllocationID > 0)
                    {
                        paramBedAllocationID.Value = p.BedAllocation.BedAllocationID;
                        paramPtRegistrationID.Value = p.BedAllocation.PtRegistrationID;
                        paramResponsibleDeptID.Value = p.BedAllocation.ResponsibleDepartment != null ? p.BedAllocation.ResponsibleDepartment.DeptID : 0;
                        paramCheckInDate.Value = ConvertNullObjectToDBNull(p.BedAllocation.CheckInDate);
                        paramExpectedStayingDays.Value = p.BedAllocation.ExpectedStayingDays;
                        paramCheckOutDate.Value = ConvertNullObjectToDBNull(p.BedAllocation.CheckOutDate);
                        paramIsActive.Value = p.BedAllocation.IsActive;
                        paramPatientInBed.Value = p.BedAllocation.PatientInBed;
                    }
                    else
                    {
                        paramBedAllocationID.Value = 0;
                        paramPtRegistrationID.Value = 0;
                        paramResponsibleDeptID.Value = 0;
                        paramCheckInDate.Value = ConvertNullObjectToDBNull(null);
                        paramExpectedStayingDays.Value = 0;
                        paramCheckOutDate.Value = ConvertNullObjectToDBNull(null);
                        paramIsActive.Value = 0;
                        paramPatientInBed.Value = 0;

                    }

                    cmd.Parameters.Add(paramBedAllocationID);
                    cmd.Parameters.Add(paramPtRegistrationID);
                    cmd.Parameters.Add(paramResponsibleDeptID);
                    cmd.Parameters.Add(paramCheckInDate);
                    cmd.Parameters.Add(paramExpectedStayingDays);
                    cmd.Parameters.Add(paramCheckOutDate);
                    cmd.Parameters.Add(paramIsActive);
                    cmd.Parameters.Add(paramPatientInBed);

                    cmd.AddParameter("@InPatientDeptDetailID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                    

                    cn.Open();
                    int val = cmd.ExecuteNonQuery();
                    InPatientDeptDetailID = (long)cmd.Parameters["@InPatientDeptDetailID"].Value;
                    return val > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public override bool DeleteInPatientTransferDeptReqXML(IList<InPatientTransferDeptReq> lstInPtTransDeptReq)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spInPatientTransferDeptReqDeleteXML", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DataXML", SqlDbType.Xml, InPtTransDeptReq_ConvertListToXml(lstInPtTransDeptReq));

                cn.Open();
                int val = cmd.ExecuteNonQuery();
                if (val > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override bool OutDepartment(InPatientDeptDetail InPtDeptDetail)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spOutDepartment", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@InPatientDeptDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(InPtDeptDetail.InPatientDeptDetailID));
                    cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(InPtDeptDetail.FromDate));
                    cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(InPtDeptDetail.ToDate));
                    cn.Open();
                    int val = cmd.ExecuteNonQuery();
                    if (val > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string InPtTransDeptReq_ConvertListToXml(IList<InPatientTransferDeptReq> lstInPtTransDeptReq)
        {
            if (lstInPtTransDeptReq != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (InPatientTransferDeptReq item in lstInPtTransDeptReq)
                {
                    sb.Append("<InPatientTransferDeptReq>");
                    if (item.InPatientTransferDeptReqID > 0)
                        sb.AppendFormat("<InPatientTransferDeptReqID>{0}</InPatientTransferDeptReqID>", item.InPatientTransferDeptReqID);
                    sb.Append("</InPatientTransferDeptReq>");

                }
                sb.Append("</DS>");

                return sb.ToString();
            }
            else
            {
                return null;
            }
        }

        private string InPatientDeptID_ConvertListToXml(ObservableCollection<long> lstDept)
        {
            if (lstDept != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (long item in lstDept)
                {
                    sb.Append("<InPatientDeptID>");
                    sb.AppendFormat("<DeptID>{0}</DeptID>", item);
                    sb.Append("</InPatientDeptID>");
                }
                sb.Append("</DS>");

                return sb.ToString();
            }
            else
            {
                return null;
            }
        }
        #endregion


        public override PatientPCLRequestDetail PatientPCLRequestDetails_GetDeptLocID(long PatientPCLReqID)
        {

            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spPatientPCLRequestDetails_GetDeptLocID";
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, PatientPCLReqID);

                conn.Open();

                PatientPCLRequestDetail obj = null;

                IDataReader reader = ExecuteReader(cmd);

                if (reader.Read())
                {
                    obj = GetPatientPCLRequestDetailsFromReader(reader);
                }

                reader.Close();

                return obj;
            }
        }

        public override PatientRegistrationDetail GetPtRegDetailNewByPatientID(long PatientID)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spGetPtRegDetailIDNewByPatientID";
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, PatientID);

                conn.Open();
                IDataReader reader = ExecuteReader(cmd);
                PatientRegistrationDetail pr = new PatientRegistrationDetail();
                if (reader.Read())
                {
                    pr = GetPatientRegistrationDetailsFromReader(reader);
                    pr.PatientRegistration = GetPatientRegistrationFromReader(reader);
                }
                return pr;
            }

        }


        public override bool AddInPatientInstruction(PatientRegistration ptRegistration, out long IntPtDiagDrInstructionID)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("sp_SaveInPatientInstruction", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = int.MaxValue;

                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ptRegistration.PtRegistrationID);

                string inPtInstructionString = null;

                inPtInstructionString = ConvertMedicalInstructionToXmlElement(ptRegistration.InPatientInstruction).ToString();


                cmd.AddParameter("@InPatientInstruction_Add", SqlDbType.Xml, ConvertNullObjectToDBNull(inPtInstructionString));
                cmd.AddParameter("@ReqOutwardDrugClinicDeptPatientXML", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertReqOutwardDetailsListToXml(ptRegistration.InPatientInstruction.ReqOutwardDetails)));
                cmd.AddParameter("@IntravenousXML", SqlDbType.Xml, ConvertBillingInvoicesToXml(ptRegistration.InPatientInstruction.IntravenousPlan).ToString());
                cmd.AddParameter("@IntPtDiagDrInstructionID", SqlDbType.BigInt, 0, ParameterDirection.Output);
                conn.Open();

                int count = ExecuteNonQuery(cmd);
                IntPtDiagDrInstructionID = (long)cmd.Parameters["@IntPtDiagDrInstructionID"].Value;
                cmd.Dispose();
                return count > 0;
            }
        }
        private List<InPatientInstruction> GetMedicalInstructionCollection(PatientRegistration aRegistration, bool IsGetLast = true, long? aIntPtDiagDrInstructionID = null)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("sp_GetInPatientInstruction", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = int.MaxValue;
                if (aRegistration != null)
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, aRegistration.PtRegistrationID);
                cmd.AddParameter("@IsGetLast", SqlDbType.Bit, IsGetLast);
                cmd.AddParameter("@IntPtDiagDrInstructionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aIntPtDiagDrInstructionID));
                conn.Open();
                IDataReader reader = ExecuteReader(cmd);
                cmd.Dispose();
                List<InPatientInstruction> mInPatientInstruction = GetInPatientInstructionCollectionFromReader(reader);
                reader.Close();
                return mInPatientInstruction;
            }
        }
        public override InPatientInstruction GetInPatientInstruction(PatientRegistration ptRegistration)
        {
            return GetMedicalInstructionCollection(ptRegistration).FirstOrDefault();
        }
        public override List<InPatientInstruction> GetInPatientInstructionCollection(PatientRegistration aRegistration)
        {
            return GetMedicalInstructionCollection(aRegistration, false);
        }
        public override InPatientInstruction GetInPatientInstructionByInstructionID(long aIntPtDiagDrInstructionID)
        {
            return GetMedicalInstructionCollection(null, true, aIntPtDiagDrInstructionID).FirstOrDefault();
        }
        public override List<Intravenous> GetIntravenousPlan_InPt(long IntPtDiagDrInstructionID)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("sp_GetIntravenousPlan_InPt", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = int.MaxValue;
                cmd.AddParameter("@IntPtDiagDrInstructionID", SqlDbType.BigInt, IntPtDiagDrInstructionID);
                conn.Open();
                IDataReader reader = ExecuteReader(cmd);
                return GetIntravenousListFromReader(reader);
            }
        }
        public override bool GetAllItemsByInstructionID(long IntPtDiagDrInstructionID, out List<PatientRegistrationDetail> AllRegistrationItems, out List<PatientPCLRequestDetail> AllPCLRequestItems)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("sp_GetAllRegistrationItemsByInstructionID", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = int.MaxValue;
                cmd.AddParameter("@IntPtDiagDrInstructionID", SqlDbType.BigInt, IntPtDiagDrInstructionID);
                conn.Open();
                IDataReader reader = ExecuteReader(cmd);
                AllRegistrationItems = GetPatientRegistrationDetailsCollectionFromReader(reader);
                reader.Close();
                cmd = new SqlCommand("sp_GetAllPCLItemsByInstructionID", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = int.MaxValue;
                cmd.AddParameter("@IntPtDiagDrInstructionID", SqlDbType.BigInt, IntPtDiagDrInstructionID);
                reader = ExecuteReader(cmd);
                AllPCLRequestItems = GetPatientPCLRequestDetailsCollectionFromReader(reader);
                reader.Close();
                return true;
            }
        }

        public override bool AddNewPatientAndHIDetails(Patient newPatientAndHiDetails, out long PatientID, out long HIID, out long PaperReferralID)
        {
            try
            {
                PatientID = 0;
                HIID = 0;
                PaperReferralID = 0;
                Patient newPatient = newPatientAndHiDetails;
                newPatient.ExtractFullName();
                if (!newPatient.DateBecamePatient.HasValue || newPatient.DateBecamePatient.Value == DateTime.MinValue)
                {
                    newPatient.DateBecamePatient = DateTime.Now;
                }

                using (var cn = new SqlConnection(this.ConnectionString))
                {
                    var cmd = new SqlCommand("spInsertNewPatientHICardAndPaperReferral_V2", cn) { CommandType = CommandType.StoredProcedure };

                    //====== Khu vực lưu Thông tin hành chính bệnh nhân
                    cmd.AddParameter("@CountryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(newPatient.CountryID));
                    cmd.AddParameter("@NationalityID", SqlDbType.BigInt, ConvertNullObjectToDBNull(newPatient.NationalityID));
                    cmd.AddParameter("@CityProvinceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(newPatient.CityProvinceID));
                    cmd.AddParameter("@SuburbNameID", SqlDbType.Int, ConvertNullObjectToDBNull(newPatient.SuburbNameID));
                    cmd.AddParameter("@IDNumber", SqlDbType.Char, ConvertNullObjectToDBNull(newPatient.IDNumber));
                    cmd.AddParameter("@FirstName", SqlDbType.NVarChar, newPatient.FirstName.ToUpper());
                    cmd.AddParameter("@MiddleName", SqlDbType.NVarChar, newPatient.MiddleName.ToUpper());
                    cmd.AddParameter("@LastName", SqlDbType.NVarChar, newPatient.LastName.ToUpper());
                    if (newPatient.GenderObj != null)
                    {
                        cmd.AddParameter("@Gender", SqlDbType.Char, newPatient.GenderObj.ID);
                    }
                    else
                    {
                        cmd.AddParameter("@Gender", SqlDbType.Char, DBNull.Value);
                    }

                    cmd.AddParameter("@DOB", SqlDbType.DateTime, ConvertNullObjectToDBNull(newPatient.DOB));
                    cmd.AddParameter("@AgeOnly", SqlDbType.Bit, ConvertNullObjectToDBNull(newPatient.AgeOnly));
                    cmd.AddParameter("@DateBecamePatient", SqlDbType.DateTime, ConvertNullObjectToDBNull(newPatient.DateBecamePatient));
                    cmd.AddParameter("@V_MaritalStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(newPatient.V_MaritalStatus));
                    cmd.AddParameter("@PatientPhoto", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.PatientPhoto));
                    cmd.AddParameter("@PatientNotes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.PatientNotes));
                    cmd.AddParameter("@PatientStreetAddress", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.PatientStreetAddress.ToUpper()));
                    cmd.AddParameter("@PatientSurburb", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.PatientSurburb));
                    cmd.AddParameter("@PatientPhoneNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.PatientPhoneNumber));
                    cmd.AddParameter("@PatientCellPhoneNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.PatientCellPhoneNumber));
                    cmd.AddParameter("@PatientEmailAddress", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.PatientEmailAddress));
                    cmd.AddParameter("@PatientEmployer", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.PatientEmployer));
                    cmd.AddParameter("@PatientOccupation", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.PatientOccupation));
                    cmd.AddParameter("@V_Ethnic", SqlDbType.BigInt, ConvertNullObjectToDBNull(newPatient.V_Ethnic));
                    cmd.AddParameter("@FContactFullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.FContactFullName));
                    cmd.AddParameter("@V_FamilyRelationship", SqlDbType.BigInt, ConvertNullObjectToDBNull(newPatient.V_FamilyRelationship));
                    cmd.AddParameter("@FContactAddress", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.FContactAddress.ToUpper()));
                    cmd.AddParameter("@FContactHomePhone", SqlDbType.VarChar, ConvertNullObjectToDBNull(newPatient.FContactHomePhone));
                    cmd.AddParameter("@FContactBusinessPhone", SqlDbType.VarChar, ConvertNullObjectToDBNull(newPatient.FContactBusinessPhone));
                    cmd.AddParameter("@FContactCellPhone", SqlDbType.VarChar, ConvertNullObjectToDBNull(newPatient.FContactCellPhone));
                    cmd.AddParameter("@FAlternateContact", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.FAlternateContact));
                    cmd.AddParameter("@FAlternatePhone", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.FAlternatePhone));
                    cmd.AddParameter("@FullName", SqlDbType.NVarChar, newPatient.FullName.ToUpper());
                    cmd.AddParameter("@BloodTypeID", SqlDbType.SmallInt, ConvertNullObjectToDBNull(newPatient.BloodTypeID));
                    cmd.AddParameter("@PatientCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(newPatient.PatientCode));
                    cmd.AddParameter("@StaffID", SqlDbType.VarChar, ConvertNullObjectToDBNull(newPatient.StaffID));
                    cmd.AddParameter("@PatientID", SqlDbType.BigInt, 16, ParameterDirection.Output);
                    cmd.AddParameter("@PatientCodeOutPut", SqlDbType.VarChar, 16, ParameterDirection.Output);
                    cmd.AddParameter("@PatientBarcode", SqlDbType.Char, 15, ParameterDirection.Output);

                    //====== Khu vực lưu Thông tin hành chính bệnh nhân

                    //====== Khu vực lưu thẻ BHYT
                    HealthInsurance newHICard = newPatientAndHiDetails.CurrentHealthInsurance;
                    cmd.AddParameter("@AcceptedBenefit", SqlDbType.Float, ConvertNullObjectToDBNull(newHICard.HIPatientBenefit));
                    cmd.AddParameter("@IsActive", SqlDbType.Bit, newHICard.IsActive);

                    cmd.AddParameter("@HosID", SqlDbType.BigInt, newHICard.HosID);
                    cmd.AddParameter("@IBID", SqlDbType.Int, newHICard.IBID);
                    if (newHICard.HIPCode != null && newHICard.HIPCode.Length > 0)
                    {
                        cmd.AddParameter("@HIPCode", SqlDbType.VarChar, newHICard.HIPCode.Trim());
                    }
                    else
                    {
                        cmd.AddParameter("@HIPCode", SqlDbType.VarChar, newHICard.HIPCode);
                    }
                    if (newHICard.HICardNo != null && newHICard.HICardNo.Length > 0)
                    {
                        cmd.AddParameter("@HICardNo", SqlDbType.VarChar, newHICard.HICardNo.Trim());
                    }
                    else
                    {
                        cmd.AddParameter("@HICardNo", SqlDbType.VarChar, newHICard.HICardNo);
                    }
                    if (newHICard.RegistrationCode != null && newHICard.RegistrationCode.Length > 0)
                    {
                        cmd.AddParameter("@RegistrationCode", SqlDbType.VarChar, newHICard.RegistrationCode.Trim());
                    }
                    else
                    {
                        cmd.AddParameter("@RegistrationCode", SqlDbType.VarChar, newHICard.RegistrationCode);
                    }
                    if (newHICard.RegistrationLocation != null && newHICard.RegistrationLocation.Length > 0)
                    {
                        cmd.AddParameter("@RegistrationLocation", SqlDbType.NVarChar, newHICard.RegistrationLocation.Trim());
                    }
                    else
                    {
                        cmd.AddParameter("@RegistrationLocation", SqlDbType.NVarChar, newHICard.RegistrationLocation);
                    }
                    if (newHICard.CityProvinceName != null && newHICard.CityProvinceName.Length > 0)
                    {
                        cmd.AddParameter("@CityProvinceName", SqlDbType.NVarChar, newHICard.CityProvinceName.Trim());
                    }
                    else
                    {
                        cmd.AddParameter("@CityProvinceName", SqlDbType.NVarChar, newHICard.CityProvinceName);
                    }
                    cmd.AddParameter("@ValidDateFrom", SqlDbType.DateTime, newHICard.ValidDateFrom);
                    cmd.AddParameter("@ValidDateTo", SqlDbType.DateTime, newHICard.ValidDateTo);
                    cmd.AddParameter("@V_HICardType", SqlDbType.BigInt, newHICard.V_HICardType);
                    cmd.AddParameter("@ArchiveNumber", SqlDbType.NVarChar, newHICard.ArchiveNumber);
                    cmd.AddParameter("@CofirmDuplicate", SqlDbType.Bit, newHICard.CofirmDuplicate);
                    cmd.AddParameter("@HIID", SqlDbType.BigInt, 16, ParameterDirection.Output);
                    if (newHICard.PatientStreetAddress != null && newHICard.PatientStreetAddress.Length > 0)
                    {
                        cmd.AddParameter("@PatientStreetAddressHI", SqlDbType.NVarChar, newHICard.PatientStreetAddress.Trim());
                    }
                    else
                    {
                        cmd.AddParameter("@PatientStreetAddressHI", SqlDbType.NVarChar, newHICard.PatientStreetAddress);
                    }
                    cmd.AddParameter("@SuburbNameIDHI", SqlDbType.BigInt, newHICard.SuburbNameID);
                    cmd.AddParameter("@CityProvinceID_Address", SqlDbType.BigInt, newHICard.CityProvinceID_Address);
                    cmd.AddParameter("@KVCode", SqlDbType.BigInt, newHICard.KVCode);

                    //====== Khu vực lưu thẻ BHYT

                    //====== Khu vực lưu thẻ CV

                    PaperReferal newPaperReferal = newPatientAndHiDetails.ActivePaperReferal;
                    cmd.AddParameter("@HospitalID", SqlDbType.BigInt, newPaperReferal.HospitalID);
                    cmd.AddParameter("@RefCreatedDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(newPaperReferal.RefCreatedDate));
                    cmd.AddParameter("@AcceptedDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(newPaperReferal.AcceptedDate));
                    cmd.AddParameter("@TreatmentFaculty", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPaperReferal.TreatmentFaculty));
                    cmd.AddParameter("@GeneralDiagnoses", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPaperReferal.GeneralDiagnoses));
                    cmd.AddParameter("@CurrentStatusOfPt", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPaperReferal.CurrentStatusOfPt));
                    cmd.AddParameter("@HosName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPaperReferal.IssuerLocation));
                    cmd.AddParameter("@IssuerLocation", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPaperReferal.IssuerLocation));
                    cmd.AddParameter("@IssuerCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPaperReferal.IssuerCode));
                    cmd.AddParameter("@CityProvinceNamePaper", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPaperReferal.CityProvinceName));
                    cmd.AddParameter("@Notes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPaperReferal.Notes));
                    cmd.AddParameter("@IsActivePaper", SqlDbType.Bit, ConvertNullObjectToDBNull(newPaperReferal.IsActive));
                    cmd.AddParameter("@IsChronicDisease", SqlDbType.Bit, ConvertNullObjectToDBNull(newPaperReferal.IsChronicDisease));
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(newPaperReferal.PtRegistrationID));
                    cmd.AddParameter("@TransferFormID", SqlDbType.BigInt, ConvertNullObjectToDBNull(newPaperReferal.TransferFormID));
                    cmd.AddParameter("@TransferNum", SqlDbType.VarChar, ConvertNullObjectToDBNull(newPaperReferal.TransferNum));
                    cmd.AddParameter("@IsReUse", SqlDbType.Bit, ConvertNullObjectToDBNull(newPaperReferal.IsReUse));
                    cmd.AddParameter("@PaperReferalID", SqlDbType.BigInt, 16, ParameterDirection.Output);
                    //====== Khu vực lưu thẻ CV

                    cn.Open();

                    int nExecRes = ExecuteNonQuery(cmd);

                    PatientID = (long)cmd.Parameters["@PatientID"].Value;
                    HIID = (long)cmd.Parameters["@HIID"].Value;
                    PaperReferralID = (long)cmd.Parameters["@PaperReferalID"].Value;

                    return (nExecRes > 0);
                }
            }
            catch (DbException dbEx)
            {
                throw dbEx;
            }

        }

        public override bool UpdateNewPatientAndHIDetails(Patient updatedPatientAndHiDetails, bool IsAdmin, long StaffID, bool IsEditAfterRegistration, out long PatientID, out long HIID, out long PaperReferralID, out double RebatePercentage, out string Result)
        {
            PatientID = 0;
            HIID = 0;
            PaperReferralID = 0;
            RebatePercentage = 0;
            Result = "Updated OK";
            Patient patient = updatedPatientAndHiDetails;
            int nUpdateMode = patient.NumberOfUpdate;
            bool bUpdateResult = false;
            
            if ((nUpdateMode & cUpdatePatientDetails) > 0)
            {
                bUpdateResult = UpdatePatient(patient, IsAdmin);
            }

            if (bUpdateResult == false)
            {
                Result = "Error Updating Patient Details";
                return false;
            }

            if ((nUpdateMode & cAddNewHICard) > 0)
            {
                bUpdateResult = AddHiItem_V2(patient.CurrentHealthInsurance, patient.StaffID, out HIID, out RebatePercentage);
            }
            else if ((nUpdateMode & cUpdateHICard) > 0)
            {
                bUpdateResult = UpdateHiItem(out Result, patient.CurrentHealthInsurance, IsEditAfterRegistration, StaffID);
            }

            if (bUpdateResult == false)
            {
                Result = "Error Updating Patient Details";
                return false;
            }

            if ((nUpdateMode & cAddPaperReferral) > 0)
            {
                bUpdateResult = AddPaperReferal(patient.ActivePaperReferal, out PaperReferralID);
            }
            else if ((nUpdateMode & cUpdatePaperReferral) > 0)
            {
                bUpdateResult = UpdatePaperReferal(patient.ActivePaperReferal);
            }

            return bUpdateResult;
            
        }

        //▼====== #013
        public override List<PatientRegistrationDetail> SearchInPatientRegistrationListForOST(long DeptID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetInPatientRegistrationByDeptID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptID));
                cn.Open();
                List<PatientRegistrationDetail> retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                retVal = GetPatientRegistrationDetailsCollectionFromReader(reader);
                reader.Close();

                return retVal;
            }
        }
        //▲====== #013
        
        //▼====== #015
        public override List<InsuranceBenefitCategories> GetInsuranceBenefitCategoriesValues()
        {
            List<InsuranceBenefitCategories> retVal = null;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllInsuranceBenefitCategories", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                retVal = GetInsuranceBenefitCategoriesCollectionFromReader(reader);
                reader.Close();
            }
            return retVal;
        }
        //▲====== #015
        //▼====== #016
        public override bool UpdateBasicDiagTreatment(PatientRegistration regInfo, string newBasicDiagTreatment)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                bool bOK = false;
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spUpdateBasicDiagTreatment";
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, regInfo.PtRegistrationID);
                cmd.AddParameter("@BasicDiagTreatment", SqlDbType.NVarChar, newBasicDiagTreatment);
                conn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal == 0)
                {
                    bOK = false;
                }
                else
                {
                    bOK = true;
                }
                return bOK;
            }
        }
        //▲====== #016
    }
}

