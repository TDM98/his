/*
 * 20170218 #001 CMN:   Add Checkbox AllDept for InPtBills
 * 20170517 #002 CMN:   Thêm giá trị để cập nhật tên bệnh nhân không giới hạn với tài khoản quản trị
 * 20170522 #003 CMN:   Added variable to check InPt 5 year HI without paid enough\
 * 20180523 #004 TBLD:  Added parameter CreatedDate
 * 20180814 #005 TTM:   Tạo mới method để thực hiện thêm mới và cập nhật bệnh nhân, thẻ BHYT, giấy CV 1 lần.
 * 20181119 #006 TTM:   BM 0005257: Tạo hàm lấy dữ liệu bệnh nhân đang nằm tại khoa.
 * 20181211 #007 TTM:   BM 0004207: Thêm hàm lấy danh sách định dạng thẻ BHYT động.
 * 20181212 #008 TTM:   Tạo hàm cập nhật Chẩn đoán ban đầu (BasicDiagTreatment)
 * 20190510 #009 TTM:   BM 0006811: Bổ sung thêm trường IBeID để gán và cập nhật trong bảng HealthInsurance.
 * 20190623 #010 TTM:   BM 0011797: Đọc dữ liệu đăng ký của bác sĩ theo ngày 
 * 20190831 #011 TTM:   BM 0013214: Thêm chức năng Kiểm tra Ngày thuốc
 * 20190908 #012 TTM:   BM 0013139: [Khám sức khoẻ] Import Bệnh nhân vào chương trình 
 * 20191002 #013 TTM:   BM 0017405: [Đề nghị nhập viện] Mặc định khoa nhập viện từ đề nghị nhập viện 
 * 20191014 #014 TBL:   BM 0017455: Fix lỗi sau khi xác nhận thẻ BHYT, đăng ký dịch vụ hoặc CLS bấm nút Lưu thì HisID lưu xuống bảng PatientRegistrationDetails và PatientPCLRequestDetails là HisID = 99898988
 * 20191102 #015 TNHX:  BM 0017411: Add func for ReCalBillingInvoice With PriceList
 * 20191203 #016 TTM:   BM 0019688: [Tìm kiếm] Bổ sung popup tìm kiếm đăng ký cho khám sức khoẻ để tìm được đăng ký theo công ty trong hợp đồng.
 * 20200704 #017 TTM:   BM 0039324: Điều chỉnh tính lại bill, nếu như trong bill có dịch vụ giãn cách thì tự động gỡ BH.
 * 20200807 #018 TNHX: Them func Xác nhận BN ngoại trú cấp cứu
 * 20201211 #019 TNHX: Them func cập nhật đã đẩy thông tin qua PAC
 * 20210315 #020 BAOLQ  : Task 237 Lấy danh sách bệnh nhân xuất viện theo khoa
 * 20210319 #020 TNHX: 219 Thêm điều kiện bỏ qua các phiếu khám không thu tiền để chuyển chi phí vào nội trú
 * 20210620 #021 TNHX: 359 Thêm biến lưu số bảo hiểm xã hội
 * 20210717 #022 TNHX: Truyền phương thức thanh toán + mã code thanh toán online
 * 20210923 #023 TNHX: Quản lý thực hiện y lệnh cho điều dưỡng
 * 20211203 #024 TNHX: Dsách PTTT từ phòng mổ
 * 20220111 #025 TNHX: Thêm func lấy chi tiết bill từ đăng ký theo khoa/ thời gian
 * 20220316 #026 QTD:  Thêm cờ kiểm tra tìm kiếm ở màn hình tạm ứng
 * 20220407 #027 DatTB: Thêm điều kiện tìm kiếm BN Chưa duyệt toa Bảo hiểm @IsConfirmHI
 * 20220510 #028 TNHX: 1340: Chuyển chi phí vào nội trú khi sát nhập(thuốc cản quang)
 * 20220530 #029 BLQ: Kiểm tra thời gian thao tác của bác sĩ
 * 20220531 #030 DatTB: Thêm biến IsWithOutBill Hủy quyết toán bỏ qua HDDT và phân quyền
 * 20220611 #031 DatTB: Thêm biến IsUnlocked cho phép user đang thao tác trả hồ sơ
 * 20220625 #032 BLQ: 
 *  + Lấy ICD chính cuối cùng của đợt điều trị ngoại trú
 *  + Lấy danh sách ICD cấu hình cho điều trị ngoại trú
 * 20220725 #033 DatTB: Thêm thông tin nhịp thở RespiratoryRate vào InPtDiagAndDoctorInstruction.
 * 20220730 #034 QTD:   Thêm đánh dấu dịch vụ Bill nội trú
 * 20220812 #035 DatTB: Chỉnh sửa màn hình hồ sơ ĐTNT
 * + Thêm hàm check toa thuốc để kiểm tra chỉnh sửa loại hình ĐTNT
 * 20220823 #036 QTD:   Tự đánh dấu Xác nhận cấp cứu nếu BN nhập viện với tình trạng cấp cứu màn hình xác nhận BHYT
 * 20221005 #037 BLQ: Thêm chức năng thẻ khách hàng
 * 20221114 #038 DatTB: Thêm biến để nhận biết gửi HSBA so với điều dưỡng xác nhận xuất viện ra viện.
 * 20230109 #039 QTD    Thêm biến đánh dấu lưu dịch vụ từ màn hình chỉ định dịch vụ của bác sĩ
 * 20230210 #040 BLQ: Thêm hàm check trước khi sát nhập vào nội trú
 * 20230317 #041 QTD   Dữ liệu 130
 * 20230407 #042 BLQ: Thêm chẩn đoán ban đầu và loại KCB màn hình đăng ký
 * 20230530 #043 DatTB: Thêm service tìm kiếm bệnh nhân bằng QRCode CCCD
 * 20230624 #044 DatTB: Thay đổi điều kiện gàng buộc ICD
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using AxLogging;
using DataEntities;
using DataEntities.CustomDTOs;
using DataEntities.MedicalInstruction;
using eHCMS.Configurations;
using eHCMS.DAL;
using eHCMS.Services.Core;
using eHCMSLanguage;
using Service.Core.Common;

namespace aEMR.DataAccessLayer.Providers
{
    public class PatientProvider : DataProviderBase //DataAccess
    {
        static private PatientProvider _instance = null;
        /// <summary>
        /// Returns an instance of the provider type specified in the config file (using reflection)
        /// </summary>
        static public PatientProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PatientProvider();
                }
                return _instance;
            }
        }

        public PatientProvider()
        {
            this.ConnectionString = Globals.Settings.Patients.ConnectionString;

        }

        public override DbConnection CreateConnection()
        {
            return new SqlConnection(this.ConnectionString);
        }

        protected virtual InPatientTransferDeptReq GetInPatientTransferDeptReqFromReader(IDataReader reader)
        {
            InPatientTransferDeptReq p = new InPatientTransferDeptReq();
            p.reqStaff = new Staff();
            p.ReqDeptLoc = new DeptLocation();
            p.ReqDeptLoc.RefDepartment = new RefDepartment();
            p.CurDept = new DeptLocation();
            p.CurDept.RefDepartment = new RefDepartment();
            p.InPatientAdmDisDetails = new InPatientAdmDisDetails();
            p.InPatientAdmDisDetails.PatientRegistration = new PatientRegistration();
            p.InPatientAdmDisDetails.PatientRegistration.Patient = new Patient();
            try
            {
                if (reader.HasColumn("PtRegistrationID"))
                {
                    p.PtRegistrationID = (long)reader["PtRegistrationID"];
                }
                if (reader.HasColumn("InPatientTransferDeptReqID") && reader["InPatientTransferDeptReqID"] != DBNull.Value)
                {
                    p.InPatientTransferDeptReqID = (long)(reader["InPatientTransferDeptReqID"]);
                }
                if (reader.HasColumn("InPatientAdmDisDetailID") && reader["InPatientAdmDisDetailID"] != DBNull.Value)
                {
                    p.InPatientAdmDisDetailID = (long)(reader["InPatientAdmDisDetailID"]);
                    p.InPatientAdmDisDetails.InPatientAdmDisDetailID = (long)(reader["InPatientAdmDisDetailID"]);
                }
                if (reader.HasColumn("ReqDeptLocID") && reader["ReqDeptLocID"] != DBNull.Value)
                {
                    p.ReqDeptLocID = (long)(reader["ReqDeptLocID"]);
                    p.ReqDeptLoc.DeptLocationID = (long)(reader["ReqDeptLocID"]);
                }
                if (reader.HasColumn("ReqDate") && reader["ReqDate"] != DBNull.Value)
                {
                    p.ReqDate = (DateTime)(reader["ReqDate"]);
                }
                if (reader.HasColumn("ReqStaffID") && reader["ReqStaffID"] != DBNull.Value)
                {
                    p.ReqStaffID = (long)(reader["ReqStaffID"]);
                    p.reqStaff.StaffID = (long)(reader["ReqStaffID"]);
                }
                if (reader.HasColumn("CurDeptID") && reader["CurDeptID"] != DBNull.Value)
                {
                    p.CurDept.DeptID = ((long)reader["CurDeptID"]);
                    p.CurDept.RefDepartment.DeptID = ((long)reader["CurDeptID"]);
                }
                if (reader.HasColumn("curDeptName") && reader["curDeptName"] != DBNull.Value)
                {
                    p.CurDept.RefDepartment.DeptName = (reader["curDeptName"].ToString());
                }
                if (reader.HasColumn("ReqDeptID") && reader["ReqDeptID"] != DBNull.Value)
                {
                    p.ReqDeptID = (long)(reader["ReqDeptID"]);
                    p.ReqDeptLoc.RefDepartment.DeptID = (long)(reader["ReqDeptID"]);
                    if (reader.HasColumn("ReqDeptTreatmentForCOVID") && reader["ReqDeptTreatmentForCOVID"] != DBNull.Value)
                    {
                        p.ReqDeptLoc.RefDepartment.IsTreatmentForCOVID = Convert.ToBoolean(reader["ReqDeptTreatmentForCOVID"]);
                    }
                }
                if (reader.HasColumn("ReqDeptName") && reader["ReqDeptName"] != DBNull.Value)
                {
                    p.ReqDeptLoc.RefDepartment.DeptName = (reader["ReqDeptName"].ToString());
                }
                if (reader.HasColumn("FullName") && reader["FullName"] != DBNull.Value)
                {
                    p.reqStaff.FullName = (reader["FullName"].ToString());
                }
                if (reader.HasColumn("PtRegistrationID") && reader["PtRegistrationID"] != DBNull.Value)
                {
                    p.InPatientAdmDisDetails.PatientRegistration.PtRegistrationID = (long)(reader["PtRegistrationID"]);
                }
                if (reader.HasColumn("PtFullName") && reader["PtFullName"] != DBNull.Value)
                {
                    p.InPatientAdmDisDetails.PatientRegistration.Patient.FullName = (reader["PtFullName"].ToString());
                }
                if (reader.HasColumn("PatientCode") && reader["PatientCode"] != DBNull.Value)
                {
                    p.InPatientAdmDisDetails.PatientRegistration.Patient.PatientCode = (reader["PatientCode"].ToString());
                }
                if (reader.HasColumn("IsAccepted") && reader["IsAccepted"] != DBNull.Value)
                {
                    p.IsAccepted = Convert.ToBoolean(reader["IsAccepted"]);
                }
            }
            catch
            { return null; }
            return p;
        }
        protected virtual List<InPatientTransferDeptReq> GetInPatientTransferDeptReqCollectionFromReader(IDataReader reader)
        {
            List<InPatientTransferDeptReq> InPatientTransferDeptReq = new List<InPatientTransferDeptReq>();
            while (reader.Read())
            {
                InPatientTransferDeptReq.Add(GetInPatientTransferDeptReqFromReader(reader));
            }
            return InPatientTransferDeptReq;
        }

        public List<Patient> SearchPatients(PatientSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
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
                cmd.AddParameter("@CityProvinceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.CityProvinceID));
                cmd.AddParameter("@SuburbNameID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.SuburbNameID));
                //▼==== #043
                cmd.AddParameter("@IDNumber", SqlDbType.VarChar, ConvertNullObjectToDBNull(criteria.IDNumber));
                cmd.AddParameter("@IDNumberOld", SqlDbType.VarChar, ConvertNullObjectToDBNull(criteria.IDNumberOld));
                //▲==== #043
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
                CleanUpConnectionAndCommand(cn, cmd);
                return patients;
            }

        }


        public bool DeletePatientByID(long patientID, long StaffID)
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                var cmd = new SqlCommand("spRemovePatientByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, patientID);
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, StaffID);
                cn.Open();
                var retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }
        public List<Patient> GetAllPatients()
        {
            throw new NotImplementedException();
        }

        public Patient GetPatientByID_Simple(long patientID, long PtRegistationID = 0)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                //return GetPatientByID_Simple(patientID, conn, null);

                var cmd = (SqlCommand)cn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spGetPatientByID_Simple";

                cmd.AddParameter("@PatientID", SqlDbType.BigInt, patientID);
                cmd.AddParameter("@PtRegistationID", SqlDbType.BigInt, PtRegistationID);

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
                CleanUpConnectionAndCommand(cn, cmd);
                return p;
            }
        }

        public Patient GetPatientByID_Simple(long patientID, long PtRegistationID, DbConnection connection, DbTransaction tran)
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
            cmd.AddParameter("@PtRegistationID", SqlDbType.BigInt, PtRegistationID);

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

        public Patient GetPatientByID(long patientID, bool ToRegisOutPt = false)
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
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return p;
            }
        }

        public Patient GetPatientByID_InPt(long patientID, bool bGetOutPtRegInfo = false, bool ToRegisOutPt = false)
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
                    reader.Close();
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return p;
            }
        }

        public List<Patient> GetPatientAll()
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
                    reader.Close();
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return lstPatient;
            }
        }

        public Patient GetPatientByID_Full(long patientID, bool ToRegisOutPt = false)
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
                        foreach (var hiItem in p.HealthInsurances)
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

                    reader.Close();
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return p;
            }
        }
        public List<Patient> GetPatients(int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            var criteria = new PatientSearchCriteria();
            return SearchPatients(criteria, pageIndex, pageSize, bCountTotal, out totalCount);
        }

        public List<Location> GetAllLocations(long? departmentID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetAllLocations", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@DepartmentID", SqlDbType.BigInt, ConvertNullObjectToDBNull(departmentID));

                cn.Open();

                var reader = ExecuteReader(cmd);

                var retVal = GetLocationCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public List<InPatientBillingInvoice> GetAllInPatientBillingInvoices(long registrationID, long? DeptID, long V_RegistrationType)
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
            return fcGetAllInPatientBillingInvoices(registrationID, DeptID, V_RegistrationType);
            //==== #001
        }
        public List<PromoDiscountProgram> GetPromoDiscountProgramCollectionByRegID(long PtRegistrationID, long V_RegistrationType = 24001)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetPromoDiscountProgramByRegID", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, V_RegistrationType);
                cn.Open();
                var reader = ExecuteReader(cmd);
                List<PromoDiscountProgram> mPromoDiscountProgram = new List<PromoDiscountProgram>();
                while (reader.Read())
                {
                    mPromoDiscountProgram.Add(GetPromoDiscountProgramFromReader(reader));
                }
                foreach (var item in mPromoDiscountProgram)
                {
                    item.PromoDiscountItems = aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetPromoDiscountItems_ByID(item.PromoDiscProgID);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return mPromoDiscountProgram;
            }
        }

        public PromoDiscountProgram GetPromoDiscountProgramByPromoDiscProgID(long PromoDiscProgID, long V_RegistrationType = 24001)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetPromoDiscountProgramByPromoDiscProgID", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@PromoDiscProgID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PromoDiscProgID));
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, V_RegistrationType);
                cn.Open();
                var reader = ExecuteReader(cmd);
                PromoDiscountProgram mPromoDiscountProgram = null;
                while (reader.Read())
                {
                    mPromoDiscountProgram = GetPromoDiscountProgramFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return mPromoDiscountProgram;
            }
        }
        //==== #001
        private List<InPatientBillingInvoice> fcGetAllInPatientBillingInvoices(long PtRegistrationID, long? DeptID, long V_RegistrationType, List<RefDepartment> DeptArray = null)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("sp_GetAllInPatientBillingInvoices", cn) { CommandType = CommandType.StoredProcedure };
                if (V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU)
                {
                    cmd.AddParameter("@OutPtRegistrationID", SqlDbType.BigInt, PtRegistrationID);
                }
                else
                {
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, PtRegistrationID);
                }
                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptID));
                if (DeptArray != null)
                {
                    cmd.AddParameter("@DeptArray", SqlDbType.Xml, ConvertNullObjectToDBNull(GenerateListToXML(DeptArray).ToString()));
                }
                cn.Open();
                List<InPatientBillingInvoice> retVal;
                var reader = ExecuteReader(cmd);
                retVal = GetInPatientBillingInvoiceCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public List<InPatientBillingInvoice> GetAllInPatientBillingInvoicesByDeptArray(long PtRegistrationID, long? DeptID, List<RefDepartment> DeptArray, long V_RegistrationType)
        {
            return fcGetAllInPatientBillingInvoices(PtRegistrationID, DeptID, V_RegistrationType, DeptArray);
        }
        //==== #001
        public List<InPatientBillingInvoice> GetAllInPatientBillingInvoices_FromListDeptID(long registrationID, List<long> ListDeptIDs)
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
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }


        public List<InPatientBillingInvoice> GetAllInPatientBillingInvoices_ForCreateForm02(long RptForm02_InPtID, long PtRegistrationID, List<long> ListDeptIDs)
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
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }


        //Ny them 

        public bool GetBalanceInPatientBillingInvoice(long StaffID, long PtRegistrationID, long V_RegistrationType, string BillingInvoices, DbConnection connection, DbTransaction tran)
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
            int count = ExecuteNonQuery(cmd);
            return count > 0;
        }

        public InPatientBillingInvoice GetInPatientBillingInvoiceByIdWithoutDetails(long InPatientBillingInvID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                //return GetInPatientBillingInvoiceByIdWithoutDetails(InPatientBillingInvID, cn, null);
                var cmd = (SqlCommand)cn.CreateCommand();
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
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public InPatientBillingInvoice GetInPatientBillingInvoiceByIdWithoutDetails(long InPatientBillingInvID, DbConnection connection, DbTransaction tran)
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
        public List<DeptLocation> GetLocationsByServiceID(long medServiceID)
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

        public List<DeptLocation> GetAllDeptLocForServicesByInOutType(List<long> RefMedicalServiceInOutOthersTypes)
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
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public List<RefMedicalServiceType> GetAllMedicalServiceTypes()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetAllMedicalServiceTypes", cn) { CommandType = CommandType.StoredProcedure };

                cn.Open();

                var reader = ExecuteReader(cmd);

                var retVal = GetMedicalServiceTypeCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public List<RefMedicalServiceType> GetMedicalServiceTypesByInOutType(List<long> RefMedicalServiceInOutOthersTypes)
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
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public List<PatientClassification> GetAllClassifications()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetAllClassifications", cn) { CommandType = CommandType.StoredProcedure };
                cn.Open();

                var reader = ExecuteReader(cmd);

                var retVal = GetPatientClassificationCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public List<BloodType> GetAllBloodTypes()
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
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public bool UpdatePatientBloodTypeID(long PatientID, int? BloodTypeID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spUpdatePatientBloodTypeID", cn) { CommandType = CommandType.StoredProcedure };

                cmd.AddParameter("@BloodTypeID", SqlDbType.SmallInt, ConvertNullObjectToDBNull(BloodTypeID));
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, PatientID);

                cn.Open();

                var retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }
        /*==== #003 ====*/
        //public bool UpdatePatient(Patient patient)
        public bool UpdatePatient(Patient patient, bool IsAdmin = false)
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
                cmd.AddParameter("@V_Job", SqlDbType.BigInt, ConvertNullObjectToDBNull(patient.V_Job));
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
                cmd.AddParameter("@PatientFullStreetAddress", SqlDbType.NVarChar, ConvertNullObjectToDBNull(patient.PatientFullStreetAddress));
                cmd.AddParameter("@IDCreatedDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(patient.IDCreatedDate));
                cmd.AddParameter("@OccupationDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(patient.OccupationDate));
                cmd.AddParameter("@IDCreatedFrom", SqlDbType.NVarChar, ConvertNullObjectToDBNull(patient.IDCreatedFrom));
                //▼====: #021
                cmd.AddParameter("@SocialInsuranceNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(patient.SocialInsuranceNumber));
                //▲====: #021
                cmd.AddParameter("@Passport", SqlDbType.NVarChar, ConvertNullObjectToDBNull(patient.Passport));
                cmd.AddParameter("@Nationality", SqlDbType.NVarChar, ConvertNullObjectToDBNull(patient.Nationality));
                //▼====: #041
                cmd.AddParameter("@JobID130", SqlDbType.BigInt, ConvertNullObjectToDBNull(patient.JobID130));
                //▲====: #041
                cmd.AddParameter("@FContactEmployer", SqlDbType.NVarChar, ConvertNullObjectToDBNull(patient.FContactEmployer));
                cmd.AddParameter("@FContactSocialInsuranceNumber", SqlDbType.VarChar, ConvertNullObjectToDBNull(patient.FContactSocialInsuranceNumber));
                cn.Open();
                var retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }
        public bool AddPatient(Patient newPatient, out long PatientID, out string PatientCode, out string PatientBarCode)
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
                cmd.AddParameter("@V_Job", SqlDbType.BigInt, ConvertNullObjectToDBNull(newPatient.V_Job));
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
                cmd.AddParameter("@PatientFullStreetAddress", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.PatientFullStreetAddress));
                cmd.AddParameter("@IDCreatedDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(newPatient.IDCreatedDate));
                cmd.AddParameter("@OccupationDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(newPatient.OccupationDate));
                cmd.AddParameter("@IDCreatedFrom", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.IDCreatedFrom));
                //▼====: #021
                cmd.AddParameter("@SocialInsuranceNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.SocialInsuranceNumber));
                //▲====: #021
                cmd.AddParameter("@Passport", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.Passport));
                cmd.AddParameter("@Nationality", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.Nationality));
                //▼====: #041
                cmd.AddParameter("@JobID130", SqlDbType.BigInt, ConvertNullObjectToDBNull(newPatient.JobID130));
                //▲====: #041
                cmd.AddParameter("@FContactEmployer", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newPatient.FContactEmployer));
                cmd.AddParameter("@FContactSocialInsuranceNumber", SqlDbType.VarChar, ConvertNullObjectToDBNull(newPatient.FContactSocialInsuranceNumber));
                cn.Open();
                var retVal = ExecuteNonQuery(cmd);
                PatientID = (long)cmd.Parameters["@PatientID"].Value;
                PatientCode = cmd.Parameters["@PatientCodeOutPut"].Value.ToString();
                PatientBarCode = cmd.Parameters["@PatientBarcode"].Value.ToString();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }

        //▼====== #011
        #region Thông tin hành chính, BHYT, CV
        public bool AddPatientAndHIDetails(Patient newPatient, HealthInsurance newHICard, PaperReferal newPaperReferal, out long PatientID, out string PatientCode, out string PatientBarCode, out long HIID, out long paperReferalID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }

        const short cUpdatePatientDetails = 0x0001;
        const short cAddNewHICard = 0x0010;
        const short cUpdateHICard = 0x0020;
        const short cAddPaperReferral = 0x0100;
        const short cUpdatePaperReferral = 0x0200;
        public bool UpdatePatientAndHIDetails(Patient patient, bool IsAdmin, long StaffID, bool IsEditAfterRegistration, out string Result)
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
                bUpdateResult = UpdateHiItem(out Result, patient.CurrentHealthInsurance, IsEditAfterRegistration, StaffID, null);
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
                bUpdateResult = AddPaperReferal(patient.ActivePaperReferal, out paperReferal);
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

        public bool RegisterPatient(PatientRegistration info, out long PatientRegistrationID, out int SequenceNo)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                //return RegisterPatient(info, conn, null, out PatientRegistrationID, out SequenceNo);
                SqlCommand cmd;
                //Neu co bao hiem thi di lay hisid cua the bao hiem nay roi insert vao.
                if (info.HealthInsurance != null && info.HealthInsurance.HIID > 0)
                {
                    cmd = (SqlCommand)cn.CreateCommand();
                    cmd.CommandText = "spGetActiveHisID";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@HIID", SqlDbType.BigInt, info.HealthInsurance.HIID);

                    object o = cmd.ExecuteScalar();
                    if (o != null && o != DBNull.Value)
                    {
                        info.HisID = (long)o;
                    }
                }
                cmd = (SqlCommand)cn.CreateCommand();
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

                IEnumerable<PatientRegistrationDetail> modifiedItems = info.PatientRegistrationDetails.Where(item => { return item.RecordState == RecordState.MODIFIED
                     || item.RecordState == RecordState.DETACHED
                                                                                                                        || item.RecordState == RecordState.ADDED;});

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
                CleanUpConnectionAndCommand(cn, cmd);
                return true;
            }
        }
        public List<PatientClassHistory> GetAllClassificationHistories(long patientID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetAllClassHistories", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, patientID);
                cn.Open();

                var reader = ExecuteReader(cmd);

                var retVal = GetPatientClassHistoryCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public List<HealthInsurance> GetAllHealthInsurances(long patientID, bool IncludeDeletedItems = false)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetAllHealthInsurances", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, patientID);
                cmd.AddParameter("@IncludeDeletedItems", SqlDbType.Bit, IncludeDeletedItems);
                cn.Open();

                var reader = ExecuteReader(cmd);

                var retVal = GetHealthInsuranceCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public HealthInsuranceHIPatientBenefit GetActiveHIBenefit(long HIID)
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
                    reader.Close();
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public PatientClassification GetLatestClassification(long patientID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetLatestClassification", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, patientID);
                cn.Open();
                PatientClassification retVal = null;

                var reader = ExecuteReader(cmd);
                if (reader != null && reader.Read())
                {
                    retVal = GetPatientClassificationFromReader(reader);
                    reader.Close();
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public HealthInsurance GetLatestHealthInsurance(long patientID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetLatestHealthInsurance", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, patientID);
                cn.Open();
                HealthInsurance retVal = null;

                var reader = ExecuteReader(cmd);
                if (reader != null && reader.Read())
                {
                    retVal = GetHealthInsuranceFromReader(reader);
                    reader.Close();
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        #region Manage Registration Types
        public List<RegistrationType> GetAllRegistrationTypes()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetAllRegistrationTypes", cn) { CommandType = CommandType.StoredProcedure };

                cn.Open();
                var reader = ExecuteReader(cmd);

                var retVal = GetRegistrationTypeCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        #endregion

        public HealthInsurance GetActiveHICard(long patientID)
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
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public List<RefMedicalServiceItem> GetMedicalServiceItems(long? departmentID, long? serviceTypeID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetMedicalServiceItems", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@ServiceTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(serviceTypeID));
                cmd.AddParameter("@DepartmentID", SqlDbType.BigInt, ConvertNullObjectToDBNull(departmentID));

                cn.Open();
                var reader = ExecuteReader(cmd);

                var retVal = GetMedicalServiceItemCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }


        public List<RefMedicalServiceItem> GetAllMedicalServiceItemsByType(long? serviceTypeID, long? MedServiceItemPriceListID, long? V_RefMedicalServiceTypes)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetAllMedicalServiceItemsByType", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@ServiceTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(serviceTypeID));
                cmd.AddParameter("@MedServiceItemPriceListID", SqlDbType.BigInt, ConvertNullObjectToDBNull(MedServiceItemPriceListID));
                cmd.AddParameter("@V_RefMedicalServiceTypes", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RefMedicalServiceTypes));
                cn.Open();
                var reader = ExecuteReader(cmd);
                var retVal = GetMedicalServiceItemCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public void AddHICard(HealthInsurance hi, out long HIID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                HIID = (long)cmd.Parameters["@HIID"].Value;
            }
        }
        public void ActivateHICard(long hiCardID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spActivateHICard", cn) { CommandType = CommandType.StoredProcedure };

                cmd.AddParameter("@HIID", SqlDbType.BigInt, hiCardID);
                cn.Open();

                var retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        // TxD 28/01/2018: Commented OUT the following because It has NEVER been USED 
        //public bool ConfirmHIBenefit(long staffID, long patientID, long hiid, float? benefit)
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

        public bool UpdateHICard(HealthInsurance hi)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }

        public bool OpenTransaction(PatientTransaction info, out long transactionID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                //return OpenTransaction(info, out transactionID, cn, null);
                var cmd = (SqlCommand)cn.CreateCommand();
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
                CleanUpConnectionAndCommand(cn, cmd);
                return true;
            }
        }
        public bool OpenTransaction(PatientTransaction info, out long transactionID, DbConnection connection, DbTransaction tran)
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

        public bool OpenTransaction_InPt(PatientTransaction info, out long transactionID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                //return OpenTransaction_InPt(info, out transactionID, cn, null);
                string strXml = info.ConvertPatientTransactionInfoToXml();

                var cmd = (SqlCommand)cn.CreateCommand();

                cmd.CommandText = "spOpenTransaction_InPt_New";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@TransactionInfoXml", SqlDbType.Xml, strXml);

                cmd.AddParameter("@TransactionID", SqlDbType.BigInt, 16, ParameterDirection.Output);

                var retVal = ExecuteNonQuery(cmd);

                transactionID = (long)cmd.Parameters["@TransactionID"].Value;
                CleanUpConnectionAndCommand(cn, cmd);
                return true;
            }
        }
        public bool OpenTransaction_InPt(PatientTransaction info, out long transactionID, DbConnection connection, DbTransaction tran)
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
        public PatientTransaction CreateTransaction(PatientRegistration registration, PatientType patientType, out bool HIServiceUsed)
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
                CleanUpConnectionAndCommand(cn, cmd);
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

        public void GetLatestClassificationAndActiveHICard(long patientID, out PatientClassification classification, out HealthInsurance activeHI)
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
        public List<PatientQueue> GetAllUnqueuedPatients(long locationID, PatientSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
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
                {
                    totalCount = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        /// <summary>
        /// Lấy tất cả các bệnh nhân đã được sắp xếp vào phòng khám, và có QueueID > markerQueueID.
        /// </summary>
        /// <param name="locationID"></param>
        /// <param name="queueType"></param>
        /// <returns></returns>
        public List<PatientQueue> GetAllQueuedPatients(long locationID, long queueType)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetQueuedPatients", cn) { CommandType = CommandType.StoredProcedure };

                cmd.AddParameter("@LID", SqlDbType.BigInt, locationID);
                cmd.AddParameter("@QueueType", SqlDbType.BigInt, queueType);
                cn.Open();
                var reader = ExecuteReader(cmd);
                var retVal = GetPatientQueueItemCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public bool Enqueue(PatientQueue queueItem)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spEnqueue", cn) { CommandType = CommandType.StoredProcedure };

                cmd.AddParameter("@ID", SqlDbType.BigInt, queueItem.ID);
                cmd.AddParameter("returnVal", SqlDbType.Int, 4, ParameterDirection.ReturnValue);
                cn.Open();

                cmd.ExecuteNonQuery();

                var retVal = (int)cmd.Parameters["returnVal"].Value;
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }

        public List<PatientRegistration> GetAllRegistrations(long patientID, bool IsInPtRegistration)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public List<PatientRegistrationDetail> GetAllRegistrationDetails_ForGoToKhamBenh(long registrationID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetAllRegistrationDetails_ForGoToKhamBenh", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, registrationID);
                cn.Open();

                var reader = ExecuteReader(cmd);
                var retVal = GetPatientRegistrationDetailsCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public List<PatientRegistrationDetail> GetAllRegistrationDetails(long registrationID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetAllRegistrationDetails", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, registrationID);
                cn.Open();

                var reader = ExecuteReader(cmd);
                var retVal = GetPatientRegistrationDetailsCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public List<PatientRegistrationDetail> GetAllRegistrationDetails_ByV_ExamRegStatus(Int64 PtRegistrationID, Int64 V_ExamRegStatus)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetAllRegistrationDetails_ByV_ExamRegStatus", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, PtRegistrationID);
                cmd.AddParameter("@V_ExamRegStatus", SqlDbType.BigInt, V_ExamRegStatus);
                cn.Open();

                var reader = ExecuteReader(cmd);
                var retVal = GetPatientRegistrationDetailsCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public List<PatientRegistrationDetail> GetAllRegistrationDetails_ByV_ExamRegStatus(Int64 PtRegistrationID, Int64 V_ExamRegStatus, DbConnection connection, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
            return retVal;
        }

        public PatientRegistration GetRegistration(long registrationID, int FindPatient)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                return GetRegistration(registrationID, FindPatient, cn, null);
            }
        }


        public PatientRegistration GetRegistraionVIPByPatientID(long PatientID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return Res;
            }
        }


        public List<PatientTransactionDetail> GetAlltransactionDetails(long transactionID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetAllTransactionDetails", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@TransactionID", SqlDbType.BigInt, transactionID);
                cn.Open();

                var reader = ExecuteReader(cmd);
                var retVal = GetTransactionDetailsCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public List<PatientTransactionDetail> GetAlltransactionDetails_InPt(long transactionID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetAllTransactionDetails_InPt", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@TransactionID", SqlDbType.BigInt, transactionID);
                cn.Open();

                var reader = ExecuteReader(cmd);
                var retVal = GetTransactionDetailsCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public List<PatientTransactionDetail> GetAlltransactionDetailsSum(long transactionID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetAllTransactionDetails_all", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@TransactionID", SqlDbType.BigInt, transactionID);
                cn.Open();

                var reader = ExecuteReader(cmd);
                var retVal = GetTransactionDetailsCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public List<PatientTransactionDetail> GetAlltransactionDetailsSum_InPt(long transactionID)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetAllTransactionDetails_all_InPt", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@TransactionID", SqlDbType.BigInt, transactionID);
                cn.Open();

                var reader = ExecuteReader(cmd);
                var retVal = GetTransactionDetailsCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public bool AddPaperReferal(PaperReferal referal, out long paperReferalID)
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

        public bool UpdatePaperReferal(PaperReferal referal)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spPaperReferal_Update", cn) { CommandType = CommandType.StoredProcedure };

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
                cn.Open();

                var retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                return true;
            }
        }

        public bool UpdatePaperReferalRegID(PaperReferal referal)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spPaperReferal_UpdateRegID", cn) { CommandType = CommandType.StoredProcedure };

                cmd.AddParameter("@PaperReferalID", SqlDbType.BigInt, referal.RefID);
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(referal.PtRegistrationID));

                cn.Open();

                var retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                return true;
            }
        }

        public void UpdateDateStarted_ForPatientRegistrationDetails(long PtRegDetailID, out DateTime? DateStart)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                DateStart = null;
                var cmd = new SqlCommand("spUpdateDateStarted_ForPatientRegistrationDetails", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@PtRegDetailID", SqlDbType.BigInt, PtRegDetailID);
                cmd.AddParameter("@DateStarted", SqlDbType.DateTime, 16, ParameterDirection.Output);
                cn.Open();
                var retVal = ExecuteNonQuery(cmd);
                DateStart = cmd.Parameters["@DateStarted"].Value as DateTime?;
                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        public bool DeletePaperReferal(PaperReferal referal)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spPaperReferal_Delete", cn) { CommandType = CommandType.StoredProcedure };

                cmd.AddParameter("@PaperReferalID", SqlDbType.BigInt, referal.RefID);

                cn.Open();

                var retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                return true;
            }
        }

        public bool SaveHIItems(long patientID, List<HealthInsurance> allHiItems, out long? activeHICardID, long StaffID = 0)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return true;
            }
        }
        public bool AddHospital(Hospital entity)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spHospitalsInsert", cn) { CommandType = CommandType.StoredProcedure };


                cmd.AddParameter("@HICode", SqlDbType.VarChar, ConvertNullObjectToDBNull(entity.HICode));
                cmd.AddParameter("@CityProvinceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.CityProvinceID));
                cmd.AddParameter("@HospitalCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(entity.HospitalCode));
                cmd.AddParameter("@HosShortName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.HosShortName));
                cmd.AddParameter("@HosName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.HosName));
                cmd.AddParameter("@HosAddress", SqlDbType.NVarChar, ConvertNullObjectToDBNull(string.IsNullOrEmpty(entity.HosAddress) ? "Không rõ" : entity.HosAddress));
                cmd.AddParameter("@HosLogoImgPath", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.HosLogoImgPath));
                cmd.AddParameter("@Slogan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Slogan));
                cmd.AddParameter("@HosPhone", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.HosPhone));
                cmd.AddParameter("@HosWebSite", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.HosWebSite));
                cmd.AddParameter("@V_HospitalType", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_HospitalType));
                cmd.AddParameter("@IsFriends", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.IsFriends));
                cmd.AddParameter("@UsedForPaperReferralOnly", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.UsedForPaperReferralOnly));
                cmd.AddParameter("@IsUsed", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.IsUsed));
                cmd.AddParameter("@V_HospitalClass", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.V_HospitalClass));
                cmd.AddParameter("@LeaderPhone", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.LeaderPhone));
                cmd.AddParameter("@ThongTuyen", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ThongTuyen));
                cn.Open();

                var retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }

        public bool UpdateHospital(Hospital entity)
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
                cmd.AddParameter("@DateModified", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.DateModified));
                cmd.AddParameter("@ModifiedLog", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ModifiedLog));
                cmd.AddParameter("@IsUsed", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.IsUsed));
                cmd.AddParameter("@V_HospitalClass", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_HospitalClass));
                cmd.AddParameter("@LeaderPhone", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.LeaderPhone));
                cmd.AddParameter("@ThongTuyen", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ThongTuyen));
                cn.Open();

                var retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }

        public bool AddHospitalByHiCode(string hospitalName, string hiCode)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("sp_Hospitals_AddByHiCode", cn) { CommandType = CommandType.StoredProcedure };


                cmd.AddParameter("@HosName", SqlDbType.NVarChar, hospitalName);
                cmd.AddParameter("@HICode", SqlDbType.VarChar, hiCode);
                cn.Open();

                var retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }

        public bool AddHiItem(HealthInsurance hiItem, out long HIID, long StaffID)
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

                //▼====== #009
                cmd.AddParameter("@IBeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(hiItem.IBeID));
                //▲====== #009

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                HIID = (long)cmd.Parameters["@HIID"].Value;
                CleanUpConnectionAndCommand(cn, cmd);
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
                CleanUpConnectionAndCommand(cn, cmd);
                return true;
            }
        }

        public bool UpdateHiItem(out string Result, HealthInsurance hiItem, bool IsEditAfterRegistration, long StaffID, string Reason)
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

                //▼====== #009
                cmd.AddParameter("@IBeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(hiItem.IBeID));
                //▲====== #009
                cmd.AddParameter("@Reason", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Reason));
                cmd.AddParameter("@errStr", SqlDbType.VarChar, 50, ParameterDirection.Output);

                cn.Open();

                var retVal = ExecuteNonQuery(cmd);
                Result = cmd.Parameters["@errStr"].Value.ToString();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }
        //public List<HICardType> GetHICardTypeList()
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


        public List<PaperReferal> GetAllPaperReferalsByHealthInsurance(long hiid, bool IncludeDeletedItems = false)
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
            reader.Close();
            CleanUpConnectionAndCommand(null, cmd);
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
        public bool SaveChangesOnRegistration(PatientRegistration info)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                return SaveChangesOnRegistration(info, cn, null);
            }
        }
        public bool SaveChangesOnRegistration(PatientRegistration info, DbConnection connection, DbTransaction tran)
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
            cmd.Dispose();
            return true;
        }

        public bool SaveChangesOnTransaction(PatientTransaction info)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                return SaveChangesOnTransaction(info, cn, null);
            }
        }

        public bool SaveChangesOnTransaction(PatientTransaction info, DbConnection connection, DbTransaction tran)
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
            cmd.Dispose();
            return true;
        }
        //public bool ProcessPayment(PatientPayment payment, long transactionID, out long PaymentID)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        return ProcessPayment(payment, transactionID, out PaymentID, cn, null);
        //    }
        //}
        //public bool ProcessPayment(PatientPayment payment, long transactionID, out long PaymentID, DbConnection connection, DbTransaction tran)
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

        public bool ProcessPayment_New(string ListIDOutTranDetails, PatientTransactionPayment payment, long transactionID, out long PaymentID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                return ProcessPayment_New(ListIDOutTranDetails, payment, transactionID, out PaymentID, cn, null);
            }
        }
        public bool ProcessPayment_New(string ListIDOutTranDetails, PatientTransactionPayment payment, long transactionID, out long PaymentID, DbConnection connection, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
            return true;
        }

        //
        public bool FinializeBillingInv_CreateTranPaymt_ConsolidateTranDetailsAndAdvPaymt(bool bCreateNewTran, PatientTransaction newTranInfo, List<InPatientBillingInvoice> colBillingInvoices,
                    DateTime paidTime, List<PatientTransactionDetail> balancedTranDetails, long PtRegistrationID, long V_RegistrationType, string ListIDOutTranDetails, PatientTransactionPayment payment,
                    long transactionID, out long NewTransactionIDRet, long? bankingTraxId)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    return FinializeBillingInv_CreateTranPaymt_ConsolidateTranDetailsAndAdvPaymt(bCreateNewTran, newTranInfo, colBillingInvoices,
                                paidTime, balancedTranDetails, PtRegistrationID, V_RegistrationType, ListIDOutTranDetails, payment, transactionID, out NewTransactionIDRet, cn, null, bankingTraxId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool FinializeBillingInv_CreateTranPaymt_ConsolidateTranDetailsAndAdvPaymt(bool bCreateNewTran, PatientTransaction newTranInfo, List<InPatientBillingInvoice> colBillingInvoices,
                    DateTime paidTime, List<PatientTransactionDetail> balancedTranDetails, long PtRegistrationID, long V_RegistrationType, string ListIDOutTranDetails, PatientTransactionPayment payment,
                    long transactionID, out long NewTransactionIDRet, DbConnection connection, DbTransaction tran, long? bankingTraxId)
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
                cmd.AddParameter("@HIAmount", SqlDbType.Money, ConvertNullObjectToDBNull(payment.HIAmount));
                cmd.AddParameter("@TotalSupport", SqlDbType.Money, ConvertNullObjectToDBNull(payment.TotalSupport));
                // VuTTM - Add the banking transaction id
                cmd.AddParameter("@BankingTrxId", SqlDbType.BigInt, ConvertNullObjectToDBNull(bankingTraxId));
                int retVal = ExecuteNonQuery(cmd);

                if (cmd.Parameters["@NewTransactionID"].Value != DBNull.Value)
                {
                    NewTransactionIDRet = (long)cmd.Parameters["@NewTransactionID"].Value;
                }
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return true;
        }

        public bool CreatePaymentAndPayAdvance(long PtRegistrationID, long V_RegistrationType, string ListIDOutTranDetails, PatientTransactionPayment payment, long transactionID, out long PaymentID, out long PtCashAdvanceID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                return CreatePaymentAndPayAdvance(PtRegistrationID, V_RegistrationType, ListIDOutTranDetails, payment, transactionID, out PaymentID, out PtCashAdvanceID, cn, null);
            }
        }
        public bool CreatePaymentAndPayAdvance(long PtRegistrationID, long V_RegistrationType, string ListIDOutTranDetails, PatientTransactionPayment payment, long transactionID, out long PaymentID, out long PtCashAdvanceID, DbConnection connection, DbTransaction tran)
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
            cmd.Dispose();
            return true;
        }


        public bool InPatientPayForBill(PatientRegistration registration, IList<InPatientBillingInvoice> billingInvoices, decimal payAmount, long staffID)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                    return retVal > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public string DeleteTransactionFinalization(string FinalizedReceiptNum, long StaffID, long V_RegistrationType, long? PtRegistrationID, bool IsWithOutBill) //<==== #030
        {
            try
            {
                using (SqlConnection mConnection = new SqlConnection(this.ConnectionString))
                {
                    string mMessage = "";
                    mConnection.InfoMessage += (s, e) =>
                    {
                        mMessage += e.Message + Environment.NewLine;
                    };
                    mConnection.Open();
                    SqlCommand mCommand = new SqlCommand("CMN_sp_DeleteTransactionFinalization", mConnection);
                    mCommand.CommandType = CommandType.StoredProcedure;
                    mCommand.AddParameter("@FinalizedReceiptNum", SqlDbType.VarChar, ConvertNullObjectToDBNull(FinalizedReceiptNum));
                    mCommand.AddParameter("@StaffID", SqlDbType.BigInt, StaffID);
                    mCommand.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));
                    mCommand.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                    mCommand.AddParameter("@IsWithOutBill", SqlDbType.Bit, ConvertNullObjectToDBNull(IsWithOutBill)); //<==== #030
                    ExecuteNonQuery(mCommand);
                    CleanUpConnectionAndCommand(mConnection, mCommand);
                    return mMessage;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void DeleteTranacsionPayment_InPt(string ReceiptNumber, long StaffID)
        {
            try
            {
                using (SqlConnection mConnection = new SqlConnection(this.ConnectionString))
                {
                    mConnection.Open();
                    SqlCommand mCommand = new SqlCommand("CMN_sp_DeleteTranacsionPayment_InPt", mConnection);
                    mCommand.CommandType = CommandType.StoredProcedure;
                    mCommand.AddParameter("@ReceiptNumber", SqlDbType.VarChar, ConvertNullObjectToDBNull(ReceiptNumber));
                    mCommand.AddParameter("@StaffID", SqlDbType.BigInt, StaffID);
                    ExecuteNonQuery(mCommand);
                    CleanUpConnectionAndCommand(mConnection, mCommand);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool InPatientSettlement(long ptRegistrationID, long staffID)
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
                    CleanUpConnectionAndCommand(cn, cmd);
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
        public bool UpdateRegItemsToBill(long InPatientBillingInvID, List<long> regDetailIds, List<long> pclRequestIds, List<long> outDrugInvIds, DbConnection connection, DbTransaction tran)
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
            cmd.Dispose();
            return true;
        }
        public bool CheckIfTransactionExists(long registrationID, out long TransactionID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }


        public bool SaveOutwardDrugInvoice(OutwardDrugInvoice invoice, out long outiID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                return SaveOutwardDrugInvoice(invoice, out outiID, cn, null);
            }
        }
        public bool SaveOutwardDrugInvoice(OutwardDrugInvoice invoice, out long outiID, DbConnection connection, DbTransaction tran)
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
            cmd.Dispose();
            return outiID > 0;
        }
        public bool SaveChangesOnOutwardDrugInvoice(OutwardDrugInvoice invoice)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                return SaveChangesOnOutwardDrugInvoice(invoice, cn, null);
            }
        }
        public bool SaveChangesOnOutwardDrugInvoice(OutwardDrugInvoice invoice, DbConnection connection, DbTransaction tran)
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
            cmd.Dispose();
            return true;
        }

        public long? GetActiveHisID(long HIID)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spGetActiveHisID";
                cmd.AddParameter("@HIID", SqlDbType.BigInt, HIID);

                conn.Open();
                object o = cmd.ExecuteScalar();
                CleanUpConnectionAndCommand(conn, cmd);
                if (o != null && o != DBNull.Value)
                {
                    return (long)o;
                }
                return null;
            }
        }
        public bool RegisterPatient(PatientRegistration info, DbConnection connection, DbTransaction tran, out long PatientRegistrationID, out int SequenceNo)
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
                cmd.Dispose();
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
            cmd.Dispose();
            PatientRegistrationID = (long)cmd.Parameters["@PatientRegistrationID"].Value;
            SequenceNo = (int)cmd.Parameters["@SequenceNo"].Value;
            return true;
        }

        public bool AddDeceaseInfo(DeceasedInfo info, DbConnection conn, DbTransaction tran, out long DeceaseId)
        {
            try
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
                cmd.Dispose();
                DeceaseId = (long)cmd.Parameters["@DSNumber"].Value;
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool UpdateDeceaseInfo(DeceasedInfo info, DbConnection conn, DbTransaction tran)
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
            cmd.Dispose();
            return retVal > 0;
        }

        public bool DeleteDeceaseInfo(long deceaseId, DbConnection conn, DbTransaction tran)
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
            cmd.Dispose();
            return retVal > 0;
        }


        public bool CreateSuggestCashAdvance(long InPatientBillingInvID, long StaffID, out long RptPtCashAdvRemID)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                    return retVal > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        public bool AddInPatientBillingInvoice(InPatientBillingInvoice inv, DbConnection conn, DbTransaction tran, out long ID)
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
            cmd.Dispose();
            ID = (long)cmd.Parameters["@InPatientBillingInvID"].Value;
            return true;
        }
        public List<PatientRegistrationDetail> GetAllRegistrationDetails(long registrationID, int FindPatient, DbConnection connection, DbTransaction tran, bool IsProcess = false)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            SqlCommand cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            //20200222 TBL Mod TMV1: Nếu IsProcess = true sẽ chạy store mới cho liệu trình
            if (!IsProcess)
            {
                cmd.CommandText = "spGetAllRegistrationDetails";
            }
            else
            {
                cmd.CommandText = "spGetAllRegistrationDetails_ForProcess";
            }
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
            CleanUpConnectionAndCommand(null, cmd);
            return retVal;
        }
        public InPatientBillingInvoice GetInPatientBillingInvoice(long billingInvoiceID, DbConnection conn, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
            return inv;
        }

        public List<PatientRegistrationDetail> GetAllRegistrationDetailsByBillingInvoiceID(long billingInvoiceID, DbConnection conn, DbTransaction tran, bool IsUsePriceByNewCer)
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
            cmd.AddParameter("@IsUsePriceByNewCer", SqlDbType.Bit, ConvertNullObjectToDBNull(IsUsePriceByNewCer));
            IDataReader reader = ExecuteReader(cmd);
            if (reader == null)
            {
                return null;
            }
            List<PatientRegistrationDetail> retVal = GetPatientRegistrationDetailsCollectionFromReader(reader);
            reader.Close();
            cmd.Dispose();
            return retVal;
        }

        //▼====: 015
        public List<PatientRegistrationDetail> GetAllRegistrationDetailsByBillingInvoiceID(long billingInvoiceID, DbConnection conn, DbTransaction tran, bool IsUsePriceByNewCer
            , bool ReCalBillingInv, long MedServiceItemPriceListID)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            SqlCommand cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spGetAllRegistrationDetailsByBillingInvoiceID_V2";
            cmd.CommandTimeout = int.MaxValue;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.AddParameter("@InPatientBillingInvID", SqlDbType.BigInt, billingInvoiceID);
            cmd.AddParameter("@IsUsePriceByNewCer", SqlDbType.Bit, ConvertNullObjectToDBNull(IsUsePriceByNewCer));
            cmd.AddParameter("@IsReCalBillingInvoices", SqlDbType.Bit, ConvertNullObjectToDBNull(ReCalBillingInv));
            cmd.AddParameter("@MedServiceItemPriceListID", SqlDbType.BigInt, MedServiceItemPriceListID);
            IDataReader reader = ExecuteReader(cmd);
            if (reader == null)
            {
                return null;
            }
            List<PatientRegistrationDetail> retVal = GetPatientRegistrationDetailsCollectionFromReader(reader);
            reader.Close();
            cmd.Dispose();
            return retVal;
        }
        //▲====: 015

        public List<PatientRegistrationDetail> GetAllInPatientRegistrationDetails_NoBill(long registrationId, long? DeptID, DbConnection conn, DbTransaction tran, DateTime? FromDate = null, DateTime? ToDate = null)
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
            CleanUpConnectionAndCommand(null, cmd);
            return retVal;
        }
        public PatientRegistration GetRegistration(long registrationID, int FindPatient, DbConnection connection, DbTransaction tran)
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
            //▼====== 20190105 TTM: Thêm code để lấy dữ liệu PrescriptionIssueHistory để kiểm tra ở giao diện thực hiện
            cmd.CommandText = "spGetPrescriptionIssueHistory_ByPtRegistrationID";
            cmd.Parameters.Clear();
            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, registrationID);
            cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, V_RegistrationType);
            reader = ExecuteReader(cmd);
            if (reader != null)
            {
                reg.ListOfPrescriptionIssueHistory = GetPtPrescriptIssueHisCollectionFromReader(reader).ToObservableCollection();
                reader.Close();
            }
            //▲======
            CleanUpConnectionAndCommand(null, cmd);
            return reg;
        }

        public PatientRegistration GetRegistrationSimple(long registrationID, int PatientFindBy)
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
                    CleanUpConnectionAndCommand(null, cmd);
                    return reg;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public TransferForm CreateBlankTransferFormByRegID(long PtRegistrationID, int PatientFindBy, long V_TransferFormType)
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
                    CleanUpConnectionAndCommand(null, cmd);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public PatientTransaction GetTransactionByID(long TransactionID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetTransactionByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@TransactionID", SqlDbType.BigInt, TransactionID);
                IDataReader reader = ExecuteReader(cmd);
                if (reader != null && reader.Read())
                {
                    var retVal = GetPatientTransactionFromReader(reader);
                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                    return retVal;
                }
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return null;
        }
        public PatientTransaction GetTransactionByRegistrationID(long RegistrationID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetTransactionByRegistrationID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@RegistrationID", SqlDbType.BigInt, RegistrationID);
                IDataReader reader = ExecuteReader(cmd);
                if (reader != null && reader.Read())
                {
                    var retVal = GetPatientTransactionFromReader(reader);
                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                    return retVal;
                }
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return null;
        }
        public List<OutwardDrugInvoice> GetDrugInvoiceListByRegistrationID(long registrationID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                return GetDrugInvoiceListByRegistrationID(registrationID, cn, null);
            }
        }
        public List<OutwardDrugInvoice> GetDrugInvoiceListByRegistrationID(long registrationID, DbConnection conn, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
            return retVal;
        }

        public OutwardDrugInvoice GetDrugInvoiceByID(long outiID, DbConnection conn, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
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
                //▼===== 20200801 TTM: Vì chỗ khác (Báo giá) có sử dụng hàm này để làm việc mà reader.Close sẽ dẫn đến không load hết dữ liệu.
                //if (reader != null)
                //{
                //    reader.Close();
                //}
                //▲===== 
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

        public List<PatientPCLRequest> GetPCLRequestListByRegistrationID(long RegistrationID, DbConnection connection, DbTransaction tran)
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
            var retVal = FillPclRequestList(reader);
            reader.Close();
            CleanUpConnectionAndCommand(null, cmd);
            return retVal;
        }

        /// <summary>
        /// Author: VuTTM
        /// Description: Getting the deleted PCL request list.
        /// </summary>
        /// <param name="RegistrationID"></param>
        /// <param name="connection"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        public List<PatientPCLRequest> GetDeletedPCLRequestListByRegistrationID(long RegistrationID)
        {
            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetDeletedPCLRequestsByRegistrationID", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@RegistrationID", SqlDbType.BigInt, RegistrationID);

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                IDataReader reader = ExecuteReader(cmd);
                var retVal = FillPclRequestList(reader);
                reader.Close();
                CleanUpConnectionAndCommand(null, cmd);
                return retVal;
            }
        }

        public List<PatientPCLRequest> GetPCLRequestListByBillingInvoiceID(long billingInvoiceID, DbConnection connection, DbTransaction tran
            , bool IsUsePriceByNewCer
            , bool IsRecalInSecondTime
            , bool IsPassCheckNonBlockValidPCLExamDate)
        {
            SqlCommand cmd;
            cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spGetPCLRequestByBillingInvoiceID";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = int.MaxValue;
            cmd.AddParameter("@InPatientBillingInvID", SqlDbType.BigInt, billingInvoiceID);
            cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, (long)AllLookupValues.RegistrationType.NOI_TRU);
            cmd.AddParameter("@IsUsePriceByNewCer", SqlDbType.Bit, ConvertNullObjectToDBNull(IsUsePriceByNewCer));
            cmd.AddParameter("@IsRecalInSecondTime", SqlDbType.Bit, IsRecalInSecondTime);
            cmd.AddParameter("@IsPassCheckNonBlockValidPCLExamDate", SqlDbType.Bit, IsPassCheckNonBlockValidPCLExamDate);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            IDataReader reader = ExecuteReader(cmd);
            var retVal = FillPclRequestList(reader);
            reader.Close();
            CleanUpConnectionAndCommand(null, cmd);
            return retVal;
        }

        //▼====: 015
        public List<PatientPCLRequest> GetPCLRequestListByBillingInvoiceID(long billingInvoiceID, DbConnection connection, DbTransaction tran
            , bool IsUsePriceByNewCer = false, bool ReCalBillingInv = false
            , long PCLExamTypePriceListID = 0
            , bool IsRecalInSecondTime = false
            , bool IsPassCheckNonBlockValidPCLExamDate = false)
        {
            SqlCommand cmd;
            cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spGetPCLRequestByBillingInvoiceID";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = int.MaxValue;
            cmd.AddParameter("@InPatientBillingInvID", SqlDbType.BigInt, billingInvoiceID);
            cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, (long)AllLookupValues.RegistrationType.NOI_TRU);
            cmd.AddParameter("@IsUsePriceByNewCer", SqlDbType.Bit, ConvertNullObjectToDBNull(IsUsePriceByNewCer));
            cmd.AddParameter("@IsReCalBillingInvoices", SqlDbType.Bit, ConvertNullObjectToDBNull(ReCalBillingInv));
            cmd.AddParameter("@PCLExamTypePriceListID", SqlDbType.BigInt, PCLExamTypePriceListID);
            cmd.AddParameter("@IsRecalInSecondTime", SqlDbType.Bit, IsRecalInSecondTime);
            cmd.AddParameter("@IsPassCheckNonBlockValidPCLExamDate", SqlDbType.Bit, IsPassCheckNonBlockValidPCLExamDate);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            IDataReader reader = ExecuteReader(cmd);
            var retVal = FillPclRequestList(reader);
            reader.Close();
            CleanUpConnectionAndCommand(null, cmd);
            return retVal;
        }
        //▲====: 015

        public List<PatientPCLRequest> spGetInPatientPCLRequest_NoBill(long registrationId, long? DeptID, DbConnection connection, DbTransaction tran, DateTime? FromDate = null, DateTime? ToDate = null)
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
            var retVal = FillPclRequestList(reader);
            reader.Close();
            CleanUpConnectionAndCommand(null, cmd);
            return retVal;
        }
        public List<PatientPCLRequest> GetPCLRequestListByIDList(List<long> PclIDList, DbConnection connection, DbTransaction tran, long V_RegistrationType)
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
            var retVal = FillPclRequestList(reader);
            reader.Close();
            CleanUpConnectionAndCommand(null, cmd);
            return retVal;
        }
        public List<PatientPCLRequest> GetPCLRequestListByIDList(List<long> PclIDList, long V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                return GetPCLRequestListByIDList(PclIDList, conn, null, V_RegistrationType);
            }
        }
        public List<PatientPCLRequest> GetPCLRequestListByRegistrationID(long RegistrationID)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                return GetPCLRequestListByRegistrationID(RegistrationID, conn, null);
            }
        }
        public List<PatientPCLRequest> GetPCLRequestListByRegistrationID_InPt(long InPtRegistrationID)
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
                var retVal = FillPclRequestList(reader);
                reader.Close();
                CleanUpConnectionAndCommand(null, cmd);
                return retVal;
            }
        }
        public List<PCLItem> GetPCLItemsByMedServiceID(long MedicalServiceID, DbConnection connection, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
            return allItems;
        }
        public List<PCLItem> GetPCLItemsByMedServiceID(long MedicalServiceID)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                return GetPCLItemsByMedServiceID(MedicalServiceID, conn, null);
            }
        }

        public List<PCLExamType> GetPclExamTypesByMedServiceID(long MedicalServiceID, DbConnection connection, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
            return allItems;
        }
        public List<PCLExamType> GetPclExamTypesByMedServiceID(long MedicalServiceID)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                return GetPclExamTypesByMedServiceID(MedicalServiceID, conn, null);
            }
        }
        public PCLExamType GetPclExamTypeByID(long ExamTypeID, long? HosClientContractID = null)
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
                cmd.AddParameter("@HosClientContractID", SqlDbType.BigInt, ConvertNullObjectToDBNull(HosClientContractID));
                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    if (reader.Read())
                    {
                        examType = GetPCLExamTypeFromReader(reader);
                    }
                    reader.Close();
                }
                CleanUpConnectionAndCommand(null, cmd);
            }
            return examType;
        }

        public bool AddNewRegistration(PatientRegistration info, out long PatientRegistrationID
            , out List<long> newRegistrationDetailIdList, out List<long> newPclRequestIdList)
        {
            return AddNewRegistration(info, out PatientRegistrationID, out newRegistrationDetailIdList, out newPclRequestIdList, null);
        }

        public bool AddNewRegistration(PatientRegistration info, out long PatientRegistrationID
            , out List<long> newRegistrationDetailIdList, out List<long> newPclRequestIdList
            , IList<DiagnosisIcd10Items> Icd10Items, bool IsNotCheckInvalid = false, bool IsProcess = false)
        {
            try
            {
                using (var conn = new SqlConnection(this.ConnectionString))
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
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
                            throw new Exception(string.Format("{0}.", eHCMSResources.Z1780_G1_CannotCreatePatient));
                        }
                    }
                    //20200222 TBL Mod TMV1: Nếu IsProcess = true sẽ chạy store mới cho liệu trình
                    if (!IsProcess)
                    {
                        cmd.CommandText = "sp_CreateNewRegistrationWithDetails";
                    }
                    else
                    {
                        cmd.CommandText = "sp_CreateNewRegistrationWithDetails_ForProcess";
                    }
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
                        cmd.AddParameter("@FiveYearsARowDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(info.FiveYearsARowDate));
                        cmd.AddParameter("@IsNotCheckInvalid", SqlDbType.Bit, ConvertNullObjectToDBNull(IsNotCheckInvalid));
                        cmd.AddParameter("@IsSpecialHIRegistration", SqlDbType.Bit, info.IsSpecialHIRegistration);
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
                    else
                    {
                        if (info.Appointment != null)
                        {
                            if (info.Appointment.ClientContract != null)
                            {
                                cmd.AddParameter("@HosClientContractID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.Appointment.ClientContract.HosClientContractID));
                            }
                            cmd.AddParameter("@OutPtTreatmentProgramID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.Appointment.OutPtTreatmentProgramID));
                        }

                        if (info.TicketIssue != null)
                        {
                            cmd.AddParameter("@TicketNumberText", SqlDbType.VarChar, ConvertNullObjectToDBNull(info.TicketIssue.TicketNumberText));
                            if (info.TicketIssue.IssueDateTime != DateTime.MinValue)
                            {
                                cmd.AddParameter("@TicketGetTime", SqlDbType.DateTime, ConvertNullObjectToDBNull(info.TicketIssue.IssueDateTime));
                            }
                            cmd.AddParameter("@TicketTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.TicketIssue.TicketTypeID));
                            cmd.AddParameter("@IsDiffBetweenRegistrationAndTicket", SqlDbType.Bit, ConvertNullObjectToDBNull(info.IsDiffBetweenRegistrationAndTicket));
                        }
                    }

                    //20191015 TBL: Task #1072: Khi đề nghị nhập viện sẽ lấy DoctorStaffID của chẩn đoán để vào StaffID của bảng PatientRegistration_InPt khi tạo ra đăng ký nội trú
                    //cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.StaffID));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.DiagnosisTreatment == null ? info.StaffID : info.DiagnosisTreatment.DoctorStaffID));
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
                    //cmd.AddParameter("@PtRegistrationCode", SqlDbType.VarChar, info.PtRegistrationCode);
                    cmd.AddParameter("@HIComment", SqlDbType.NVarChar, info.HIComment);
                    cmd.AddParameter("@V_RegistrationStatus", SqlDbType.BigInt, (long)info.RegistrationStatus);
                    cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, (long)info.V_RegistrationType);
                    cmd.AddParameter("@PatientRegistrationID", SqlDbType.BigInt, info.PtRegistrationID, ParameterDirection.Output);

                    
                    string detailListString = null;
                    if (info.PatientRegistrationDetails != null && info.PatientRegistrationDetails.Count > 0)
                    {
                        foreach (var item in info.PatientRegistrationDetails)
                        {
                            item.StaffID = info.StaffID;
                            if (item.HIAllowedPrice > 0)
                            {
                                //▼====== #014
                                item.HisID = info.HisID;
                                //▲====== #014
                            }
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
                            //▼====== #014
                            foreach (var pcldetail in item.PatientPCLRequestIndicators)
                            {
                                if (pcldetail.HIAllowedPrice > 0)
                                {
                                    //▼====== #014
                                    pcldetail.HisID = info.HisID;
                                    //▲====== #014
                                }
                            }
                            //▲====== #014
                        }

                        pclRequestListString = ConvertPCLRequestsToXml(info.PCLRequests, PatientMedicalRecordID, info.PatientID.Value).ToString();
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
                        //▼====== #042
                        if (info.AdmissionICD10 != null)
                        {
                            //▼====== #016
                            cmd.AddParameter("@AdmissionICD10Code", SqlDbType.NVarChar, info.AdmissionICD10.ICD10Code);
                            //▲====== #016
                        }
                        cmd.AddParameter("@BasicDiagTreatment", SqlDbType.NVarChar, info.BasicDiagTreatment);
                        cmd.AddParameter("@V_MedicalExaminationType", SqlDbType.BigInt, info.V_MedicalExaminationType == null ? 0 : info.V_MedicalExaminationType.LookupID);
                        cmd.AddParameter("@V_ReasonHospitalStay", SqlDbType.BigInt, info.V_ReasonHospitalStay == null ? 0 : info.V_ReasonHospitalStay.LookupID);
                        cmd.AddParameter("@V_ReceiveMethod", SqlDbType.BigInt, info.V_ReceiveMethod);
                        //▲====== #042
                    }
                    cmd.AddParameter("@V_ObjectMedicalExamination", SqlDbType.BigInt, info.V_ObjectMedicalExamination == null ? 0 : info.V_ObjectMedicalExamination.LookupID);
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
                        cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.DiagnosisTreatment.Department.DeptID));
                        cmd.AddParameter("@ReasonHospitalStay", SqlDbType.NVarChar, ConvertNullObjectToDBNull(info.DiagnosisTreatment.ReasonHospitalStay));
                        cmd.AddParameter("@AdmissionCriteriaList", SqlDbType.NVarChar, ConvertNullObjectToDBNull(info.DiagnosisTreatment.AdmissionCriteriaList));
                        cmd.AddParameter("@IsTreatmentCOVID", SqlDbType.Bit, ConvertNullObjectToDBNull(info.DiagnosisTreatment.IsTreatmentCOVID));
                    }
                    if (info.RefByPatient != null && info.RefByPatient.PatientID > 0)
                    {
                        cmd.AddParameter("@RefByPatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.RefByPatient.PatientID));
                    }
                    if (info.RefByStaff != null && info.RefByStaff.StaffID > 0)
                    {
                        cmd.AddParameter("@RefByStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.RefByStaff.StaffID));
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
                    CleanUpConnectionAndCommand(conn, cmd);
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
        public bool AddBedPatientRegDetail(BedPatientRegDetail bedPatientDetail, out long bedPatientDetailId, DbConnection connection, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
            return retVal > 0;
        }

        public bool AddNewRegistrationDetails(PatientRegistrationDetail regDetails, out long RegDetailsID, DbConnection connection, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
            return retVal > 0;
        }
        public bool AddNewRegistrationDetails(PatientRegistrationDetail regDetails, out long RegDetailsID)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                return AddNewRegistrationDetails(regDetails, out RegDetailsID, conn, null);
            }
        }

        public bool AddNewPCLRequest(PatientPCLRequest info, out long PCLRequestID, DbConnection connection, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
            return true;
        }
        public bool AddNewPCLRequest(PatientPCLRequest info, out long PCLRequestID)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                return AddNewPCLRequest(info, out PCLRequestID, conn, null);
            }
        }

        //public bool UpdatePCLRequest(PatientPCLRequest info, DbConnection connection, DbTransaction tran)
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
        //public bool UpdatePCLRequest(PatientPCLRequest info)
        //{
        //    using (SqlConnection conn = new SqlConnection(this.ConnectionString))
        //    {
        //        return UpdatePCLRequest(info, conn, null);
        //    }
        //}

        public bool AddNewPCLRequestDetails(long PCLRequestID, PatientPCLRequestDetail PCLRequestDetails, out long PCLRequestDetailsID, DbConnection connection, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
            return true;
        }
        public bool AddNewPCLRequestDetails(long PCLRequestID, PatientPCLRequestDetail PCLRequestDetails, out long PCLRequestDetailsID)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                return AddNewPCLRequestDetails(PCLRequestID, PCLRequestDetails, out PCLRequestDetailsID, conn, null);
            }
        }

        public bool UpdatePCLRequestDetails(PatientPCLRequestDetail PCLRequestDetails, DbConnection connection, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
            return true;
        }
        public bool UpdatePCLRequestDetails(PatientPCLRequestDetail PCLRequestDetails)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                return UpdatePCLRequestDetails(PCLRequestDetails, conn, null);
            }
        }

        //Không thấy hàm nào sử dụng.
        public void UpdatePaidTime(List<PatientRegistrationDetail> paidRegDetailsList,
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

        public void UpdatePaidTimeForBillingInvoice(List<InPatientBillingInvoice> billingInvoiceList, DateTime paidTime, DbConnection conn, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
        }

        public void RemovePaidRegItems(List<PatientRegistrationDetail> paidRegDetailsList,
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
            CleanUpConnectionAndCommand(null, cmd);
        }

        public bool AddNewPatientMedicalRecord(PatientMedicalRecord info, out long PatientMedicalRecordID, DbConnection connection, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
            return true;
        }
        public bool AddNewPatientMedicalRecord(PatientMedicalRecord info, out long PatientMedicalRecordID)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                return AddNewPatientMedicalRecord(info, out PatientMedicalRecordID, conn, null);
            }
        }

        public bool AddNewPatientServiceRecord(long PatientMedicalRecordID, long? PatientMedicalFileID
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
            CleanUpConnectionAndCommand(null, cmd);
            return true;
        }
        public bool AddNewPatientServiceRecord(long PatientMedicalRecordID, long? PatientMedicalFileID,
            PatientServiceRecord PtServiceRecord, out long PatientServiceRecordID)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                return AddNewPatientServiceRecord(PatientMedicalRecordID, PatientMedicalFileID, PtServiceRecord, out PatientServiceRecordID, conn, null);
            }
        }

        public bool CheckIfMedicalRecordExists(long PatientID, out long PatientMedicalRecordID, out long? PatientMedicalFileID
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
                CleanUpConnectionAndCommand(null, cmd);
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
                CleanUpConnectionAndCommand(null, cmd);
                return false;
            }
        }
        public bool CheckIfMedicalRecordExists(long PatientID, out long PatientMedicalRecordID, out long? PatientMedicalFileID)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                return CheckIfMedicalRecordExists(PatientID, out PatientMedicalRecordID, out PatientMedicalFileID, conn, null);
            }
        }

        public PatientRegistration GetPatientRegistrationByPtRegistrationID(long PtRegistrationID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public long CheckRegistrationStatus(long PtRegistrationID)
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
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
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

        public List<PatientRegistration> SearchRegistrations(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
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
                {
                    totalCount = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public List<PatientRegistration> SearchRegistrationsInPt(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, bool bCalledFromSearchInPtPopup, out int totalCount)
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
                cmd.AddParameter("@IsSearchByRegistrationDetails", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.IsSearchByRegistrationDetails));
                cmd.AddParameter("@IsSearchForCashAdvance", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.IsSearchForCashAdvance));

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
                {
                    totalCount = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public List<PatientRegistration> SearchRegisPrescription(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
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
                {
                    totalCount = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public List<PatientRegistrationDetail> SearchRegistrationListForOST(long DeptID, long DeptLocID, long StaffID, long ExamRegStatus, long V_OutPtEntStatus)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetRegistrationListByExamStatus", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptID));
                cmd.AddParameter("@DeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocID));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                cmd.AddParameter("@ExamRegStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(ExamRegStatus));
                cmd.AddParameter("@V_OutPtEntStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_OutPtEntStatus));

                cn.Open();
                List<PatientRegistrationDetail> retVal = null;

                IDataReader reader = ExecuteReader(cmd);

                //retVal = GetPatientRegistrationCollectionFromReader(reader);
                retVal = GetPatientRegistrationDetailsCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public List<PatientRegistrationDetail> SearchRegisDetailPrescription(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
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
                {
                    totalCount = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public List<PatientRegistration> SearchRegistrationsDiagTrmt(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
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
                {
                    totalCount = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }


        #region Outstanding Task

        //KMx: Anh Tuấn không cho insert vào bảng PatientQueue.
        //public bool PatientQueue_Insert(PatientQueue ObjPatientQueue, DbConnection connection, DbTransaction tran)
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
        //public bool PatientQueue_Insert(PatientQueue ObjPatientQueue)
        //{
        //    using (SqlConnection conn = new SqlConnection(this.ConnectionString))
        //    {
        //        return PatientQueue_Insert(ObjPatientQueue, conn, null);
        //    }
        //}

        //KMx: Anh Tuấn không cho insert vào bảng PatientQueue.
        //public bool PatientQueue_InsertList(List<PatientQueue> lstPatientQueue, ref List<string> lstPatientQueueFail)
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
        public bool PatientQueue_MarkDelete(Int64 RegistrationID, Int64 IDType, Int64 PatientID, Int64 V_QueueType, Int64 DeptLocID, DbConnection connection, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
            return true;
            //int ReturnVal = (cmd.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd.Parameters["@ReturnVal"].Value) : 0);
            //if (ReturnVal == 1)
            //    return true;
            //return false;            
        }

        public List<PatientQueue> PatientQueue_GetListPaging(PatientQueueSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
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
                {
                    totalCount = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        #endregion

        public List<RefGenMedDrugDetails> SearchMedDrugs(DrugSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
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
                {
                    totalCount = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
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
        public List<RefGenMedProductDetails> SearchMedProducts(MedProductSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
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
                {
                    totalCount = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public List<InwardDrugClinicDept> SearchInwardDrugClinicDept(MedProductSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
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
                {
                    totalCount = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public List<InwardDrugClinicDept> GetAllInwardDrugClinicDeptByProductList(long storeID, long medProductType, List<long> medProductList)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                return GetAllInwardDrugClinicDeptByProductList(storeID, medProductType, medProductList, conn, null);
            }
        }
        public List<InwardDrugClinicDept> GetAllInwardDrugClinicDeptByProductList(long storeID, long medProductType, List<long> medProductList, DbConnection connection, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
            return retVal;
        }

        public List<InwardDrugClinicDept> GetAllInwardDrugClinicDeptByIDList(List<long> inwardDrugIdList)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public List<RefGenMedProductDetails> GetGenMedProductsRemainingInStore(Dictionary<long, List<long>> drugIdList)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public List<RefMedicalServiceItem> SearchMedServices(MedicalServiceSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
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
                {
                    totalCount = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);

                return retVal;
            }
        }

        public List<PCLItem> SearchPCLItems(PCLItemSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
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
                {
                    totalCount = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public bool SaveChangesOnOutwardDrugClinicInvoice(OutwardDrugClinicDeptInvoice info, out List<long> InwardDrugIDList_Error)
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
        public bool SaveChangesOnOutwardDrugClinicInvoice(OutwardDrugClinicDeptInvoice info, DbConnection connection, DbTransaction tran, out List<long> InwardDrugIDList_Error)
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
            CleanUpConnectionAndCommand(null, cmd);
            return true;
        }
        public bool AddOutwardDrugClinicInvoice(OutwardDrugClinicDeptInvoice info, List<InwardDrugClinicDept> updatedInwardItems, DbConnection connection, DbTransaction tran, out List<long> InwardDrugIDList_Error)
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
            CleanUpConnectionAndCommand(null, cmd);
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
        public List<OutwardDrugClinicDeptInvoice> GetAllInPatientInvoices(long registrationID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                return GetAllInPatientInvoices(registrationID, cn, null);
            }
        }
        public List<OutwardDrugClinicDeptInvoice> GetAllInPatientInvoices(long registrationID, DbConnection conn, DbTransaction tran)
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
            var retVal = GetAllInPatientInvoices(reader);
            reader.Close();
            CleanUpConnectionAndCommand(null, cmd);
            return retVal;
        }


        public List<OutwardDrugClinicDeptInvoice> GetInPatientInvoicesHasMedProducts(long registrationID, List<long> MedProductIDs, long? DeptID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                return GetInPatientInvoicesHasMedProducts(registrationID, MedProductIDs, DeptID, cn, null);
            }
        }
        public List<OutwardDrugClinicDeptInvoice> GetInPatientInvoicesHasMedProducts(long registrationID, List<long> MedProductIDs, long? DeptID, DbConnection conn, DbTransaction tran)
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
            var retVal = GetAllInPatientInvoices(reader);
            reader.Close();
            CleanUpConnectionAndCommand(null, cmd);
            return retVal;
        }
        public List<OutwardDrugClinicDeptInvoice> GetAllInPatientInvoicesByBillingInvoiceID(long billingInvoiceID, DbConnection conn, DbTransaction tran)
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
            var retVal = GetAllInPatientInvoices(reader);
            reader.Close();
            CleanUpConnectionAndCommand(null, cmd);
            return retVal;
        }

        /// <summary>
        /// VuTTM
        /// </summary>
        /// <param name="registrationId"></param>
        /// <param name="conn"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public int CheckForPreloadingBills(long registrationId, out string errMsg)
        {
            int canCheck = 0;
            using (var connection = new SqlConnection(ConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                SqlCommand cmd = (SqlCommand)connection.CreateCommand();
                cmd.CommandText = "sp_CheckForPreloadingBills";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, registrationId);
                SqlParameter paramErrMsg = new SqlParameter("@ErrMsg", SqlDbType.NVarChar, int.MaxValue);
                paramErrMsg.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paramErrMsg);
                SqlParameter paramCanCheck = new SqlParameter("@CanCheck", SqlDbType.Int);
                paramCanCheck.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paramCanCheck);
                cmd.ExecuteNonQuery();

                errMsg = null;
                if (DBNull.Value != paramErrMsg.Value)
                {
                    errMsg = paramErrMsg.Value.ToString();
                }
                if (DBNull.Value != paramCanCheck.Value)
                {
                    canCheck = (int) cmd.Parameters["@CanCheck"].Value;
                }
                CleanUpConnectionAndCommand(null, cmd);
            }

            return canCheck;
        }

        public List<OutwardDrugClinicDeptInvoice> GetAllInPatientInvoices_NoBill(long registrationId, long? DeptID, long StoreID, long StaffID, bool IsAdmin, long V_RegistrationType, DbConnection conn, DbTransaction tran, DateTime? FromDate = null, DateTime? ToDate = null)
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
            cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, V_RegistrationType);
            IDataReader reader = ExecuteReader(cmd);
            var retVal = GetAllInPatientInvoices(reader);
            reader.Close();
            CleanUpConnectionAndCommand(null, cmd);
            return retVal;
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

        public List<RefGenMedProductSummaryInfo> GetRefGenMedProductSummaryByRegistration(long registrationID, long medProductType, long? DeptID)
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
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return retVal;
        }

        public bool ChangeRegistrationType(PatientRegistration regInfo, AllLookupValues.RegistrationType newType)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_ChangeRegistrationType", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, regInfo.PtRegistrationID);
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, (long)newType);

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }

        public bool Registrations_UpdateStatus(PatientRegistration regInfo, long V_RegistrationStatus)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                    return retVal > 0;
                }
                catch (SqlException ex)
                {
                    AxLogger.Instance.LogError(ex.ToString());
                    throw new Exception(ex.Message);
                }
            }
        }


        public List<Staff> GetStaffsHaveRegistrations(byte Type)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public List<Staff> GetStaffsHavePayments(long V_TradingPlaces)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public bool AddTransactionDetailList(long StaffID, long transactionId, List<PatientTransactionDetail> tranDetailList, out string ListOutID, DbConnection connection, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
            return true;
        }

        public bool AddTransactionDetailList_InPt(long StaffID, long transactionId, List<PatientTransactionDetail> tranDetailList, out string ListOutID, DbConnection connection, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
            return true;
        }

        public bool UpdatePclRequestInfo(PatientPCLRequest p)
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
                cmd.AddParameter("@IsTransferredToRIS", SqlDbType.Bit, ConvertNullObjectToDBNull(p.IsTransferredToRIS));
                cmd.AddParameter("@IsCancelTransferredToRIS", SqlDbType.Bit, ConvertNullObjectToDBNull(p.IsCancelTransferredToRIS));
                conn.Open();
                cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(conn, cmd);
                return true;
            }
        }
        public long ActiveHisID(HealthInsurance aHealthInsurance)
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
                    CleanUpConnectionAndCommand(conn, cmd);
                    if (o != null && o != DBNull.Value)
                    {
                        return (long)o;
                    }
                }
            }
            return 0;
        }
        public bool AddRegistration(PatientRegistration info, DbConnection conn, DbTransaction tran
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
            CleanUpConnectionAndCommand(null, cmd);
            return true;
        }


        public bool AddRegistrationDetails(long registrationID, int FindPatient, List<PatientRegistrationDetail> regDetailList, DbConnection conn, DbTransaction tran, out List<long> newItemIDList)
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
            CleanUpConnectionAndCommand(null, cmd);
            return true;
        }

        public bool UpdateRegistrationDetails(IList<PatientRegistrationDetail> regDetailList, int FindPatient, DbConnection conn, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
            return retVal > 0;
        }

        public void DeletePCLRequestWithDetails(Int64 PatientPCLReqID, out string Result)
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
                    {
                        Result = cmd.Parameters["@Result"].Value.ToString();
                    }
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void DeleteInPtPCLRequestWithDetails(Int64 PatientPCLReqID)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //KMx: Không truyền ServiceRecID, dùng PtRegistrationID xuống database kiểm tra xem có ServiceRecord chưa, nếu chưa có thì tạo mới (02/11/2014 11:34).
        //public bool AddPCLRequestWithDetails(PatientPCLRequest request, DbConnection connection, DbTransaction tran, out long PCLRequestID)
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


        public bool AddPCLRequestWithDetails(long ptRegistrationID, long V_RegistrationType, long ptMedicalRecordID, PatientPCLRequest request, DbConnection connection, DbTransaction tran, out long PCLRequestID, bool IsNotCheckInvalid = false)
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
            if (IsNotCheckInvalid)
            {
                cmd.AddParameter("@IsNotCheckInvalid", SqlDbType.Bit, ConvertNullObjectToDBNull(IsNotCheckInvalid));
            }
            int retVal = ExecuteNonQuery(cmd);


            PCLRequestID = (long)cmd.Parameters["@PatientPCLReqID"].Value;
            CleanUpConnectionAndCommand(null, cmd);
            return true;
        }



        public bool AddPCLRequestDetails(long pclRequestID, List<PatientPCLRequestDetail> requestDetailList, DbConnection connection, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
            return true;
        }

        public bool AddPCLRequestDetails(List<PatientPCLRequestDetail> requestDetailList, DbConnection connection, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
            return true;
        }
        public bool UpdatePCLRequestDetailList(List<PatientPCLRequestDetail> requestDetailList, DbConnection connection, DbTransaction tran, bool IsNotCheckInvalid = false)
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
            cmd.AddParameter("@IsNotCheckInvalid", SqlDbType.Bit, ConvertNullObjectToDBNull(IsNotCheckInvalid));
            int retVal = ExecuteNonQuery(cmd);
            CleanUpConnectionAndCommand(null, cmd);
            return true;
        }

        public bool DeletePCLRequestDetailList(List<PatientPCLRequestDetail> requestDetailList, DbConnection connection, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
            return true;
        }

        public bool UpdateBillingInvoice(InPatientBillingInvoice billingInv)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                    return retVal > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool DeleteRegistrationDetailList(List<PatientRegistrationDetail> registrationDetailList, DbConnection connection, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
            return true;
        }

        public bool DeleteOutwardDrugClinicDeptList(List<OutwardDrugClinicDept> outwardDrugClinicDeptList, DbConnection connection, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
            return true;
        }

        public bool DeleteOutwardDrugClinicDeptInvoices(List<OutwardDrugClinicDeptInvoice> outwardDrugClinicDeptClinicDeptInvoices, DbConnection connection, DbTransaction tran)
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
                CleanUpConnectionAndCommand(null, cmd);
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

        public bool AddOutwardDrugClinicDeptIntoBill(long PtRegistrationID, long InPatientBillingInvID, List<OutwardDrugClinicDeptInvoice> outwardDrugClinicDeptClinicDeptInvoices, DbConnection connection, DbTransaction tran)
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
                CleanUpConnectionAndCommand(null, cmd);
                return count > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public bool UpdateOutwardDrugClinicDeptList(IList<OutwardDrugClinicDept> outwardDrugClinicDeptList, long? StaffID, DbConnection connection, DbTransaction tran)
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
                            new XElement("HisID", details.HisID),
                            new XElement("CountValue", details.CountValue),
                            new XElement("GenMedVersionID", details.GenMedVersionID)
                            //▼====: #103
                            , new XElement("OtherAmt", details.OtherAmt)
                            , new XElement("IsCountPatientCOVID", details.IsCountPatientCOVID)
                            //▲====: #103
                            //▼====: #034
                            , new XElement("IsCountSE", details.IsCountSE)
                            //▲====: #034
                       )));

            string detailListString = xmlDocument.ToString();

            cmd.AddParameter("@OutwardDrugClinicDeptList", SqlDbType.Xml, ConvertNullObjectToDBNull(detailListString));
            cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
            int retVal = ExecuteNonQuery(cmd);
            CleanUpConnectionAndCommand(null, cmd);
            return true;
        }

        public bool CalculateBillInvoice(long InPatientBillingInvID, long StaffID, bool IsUpdateBill, DbConnection connection, DbTransaction tran)
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
                CleanUpConnectionAndCommand(null, cmd);
                return retVal > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /*==== #004 ====*/
        //public bool CalAdditionalFeeAndTotalBill(long InPatientBillingInvID, long StaffID, bool IsUpdateBill, DbConnection connection, DbTransaction tran)
        public bool CalAdditionalFeeAndTotalBill(long InPatientBillingInvID, long StaffID, bool IsUpdateBill, DbConnection connection, DbTransaction tran, DateTime CreatedDate, double? HIBenefit = 1, bool IsHICard_FiveYearsCont_NoPaid = false, bool ReplaceMaxHIPay = false, bool IsFromRecal = false)
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
                cmd.AddParameter("@ReplaceMaxHIPay", SqlDbType.Bit, ConvertNullObjectToDBNull(ReplaceMaxHIPay));
                cmd.AddParameter("@IsFromRecal", SqlDbType.Bit, ConvertNullObjectToDBNull(IsFromRecal));
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(null, cmd);
                return retVal > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        //dinh them cho nay vo cho ngoai vien
        //---them moi pclrequest dong thoi them detail
        public bool AddPCLRequestExtWithDetails(PatientPCLRequest_Ext request, out long PatientPCLReqExtID)
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
                CleanUpConnectionAndCommand(conn, cmd);
                return true;
            }
        }

        public bool PCLRequestExtUpdate(PatientPCLRequest_Ext request)
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
                CleanUpConnectionAndCommand(conn, cmd);
                return retVal > 0;
            }
        }

        //--them moi detail cho pclrequest
        public bool AddNewPCLRequestDetailsExt(PatientPCLRequest_Ext request)
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
                CleanUpConnectionAndCommand(conn, cmd);
                return retVal > 0;
            }
        }

        public bool DeletePCLRequestDetailExtList(List<PatientPCLRequestDetail_Ext> requestDetailList)
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
                CleanUpConnectionAndCommand(conn, cmd);
                return true;
            }
        }

        public bool DeletePCLRequestExt(long PatientPCLReqExtID)
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
                CleanUpConnectionAndCommand(conn, cmd);
                return true;
            }
        }

        public List<PatientPCLRequest_Ext> GetPCLRequestListExtByRegistrationID(long RegistrationID)
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
                var retVal = FillPclRequestExtList(reader);
                reader.Close();
                CleanUpConnectionAndCommand(conn, cmd);
                return retVal;
            }
        }

        public PatientPCLRequest_Ext GetPCLRequestExtPK(long PatientPCLReqExtID)
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
                reader.Close();
                CleanUpConnectionAndCommand(conn, cmd);
                return temp;

            }
        }

        public PatientPCLRequest_Ext PatientPCLRequestExtByID(long PatientPCLReqExtID)
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
                reader.Close();
                CleanUpConnectionAndCommand(conn, cmd);
                return temp;

            }
        }

        public List<PatientPCLRequest_Ext> PatientPCLRequestExtPaging(PatientPCLRequestExtSearchCriteria SearchCriteria,
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
                {
                    Total = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        public List<PatientPCLRequestDetail_Ext> GetPCLRequestDetailListExtByRegistrationID(long RegistrationID)
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
                    CleanUpConnectionAndCommand(conn, cmd);
                }

            }
        }




        public List<PatientRegistrationDetail> GetAllRegistrationDetailsByIDList(List<long> regDetailIdList, bool IsProcess = false)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                return GetAllRegistrationDetailsByIDList(regDetailIdList, conn, null, IsProcess);
            }
        }

        public List<PatientRegistrationDetail> GetAllRegistrationDetailsByIDList(List<long> regDetailIdList, DbConnection connection, DbTransaction tran, bool IsProcess = false)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            SqlCommand cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            //20200222 TBL Mod TMV1: Nếu IsProcess = true sẽ chạy store mới cho liệu trình
            if (!IsProcess)
            {
                cmd.CommandText = "spGetAllRegistrationDetails";
            }
            else
            {
                cmd.CommandText = "spGetAllRegistrationDetails_ForProcess";
            }

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
                CleanUpConnectionAndCommand(null, cmd);
            }
            return retVal;
        }

        //---Dinh them 
        public List<PatientRegistrationDetail> GetPatientRegistrationDetailsByDay(long DeptLocID)
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
                    CleanUpConnectionAndCommand(conn, cmd);
                }
                return retVal;
            }
        }

        public List<PatientRegistrationDetail> GetPatientRegistrationDetailsByDay_ForXML(IList<DeptLocation> lstDeptLoc)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = (SqlCommand)conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spPatientRegistrationDetailsGetDay_ForXML";
                cmd.AddParameter("@XML", SqlDbType.Xml, ConvertListToXml(lstDeptLoc));
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
                    CleanUpConnectionAndCommand(conn, cmd);
                }
                return retVal;
            }
        }

        public List<PatientRegistrationDetail> GetPtRegisDetailsByConsultTimeSegment(long DeptLocID
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
                    CleanUpConnectionAndCommand(conn, cmd);
                }
                return retVal;
            }
        }

        public List<PatientRegistrationDetail> GetPtRegisDetailsByConsultTimeSegment_ForXML(IList<DeptLocation> lstDeptLocation, long V_TimeSegment, long StartSequenceNumber, long EndSequenceNumber)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = (SqlCommand)conn.CreateCommand();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spGetPtRegisDetailsByConsultTimeSegment_ForXML";

                cmd.AddParameter("@XML", SqlDbType.Xml, ConvertListToXml(lstDeptLocation));
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
                    CleanUpConnectionAndCommand(conn, cmd);
                }
                return retVal;
            }
        }
        private string ConvertListToXml(IList<DeptLocation> lstDeptLocation)
        {
            try
            {
                if (lstDeptLocation != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<DS>");
                    foreach (DeptLocation item in lstDeptLocation)
                    {
                        sb.AppendFormat("<DeptLocationID>{0}</DeptLocationID>", item.DeptLocationID);
                    }
                    sb.Append("</DS>");
                    return sb.ToString();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PatientRegistrationDetailEx> GetPatientRegistrationDetailsByRoom(out int totalCount, SeachPtRegistrationCriteria SeachRegCriteria)
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
                {
                    totalCount = -1;
                }
                CleanUpConnectionAndCommand(conn, cmd);
                return retVal;
            }
        }


        // _TBA: Adapter
        public List<List<string>> ExportToExcelBangKeChiTietKhamBenh(SeachPtRegistrationCriteria SeachRegCriteria)
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

        public List<PatientRegistrationDetail> SearchRegistrationsForDiag(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
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
                cmd.AddParameter("@IsSearchOnlyProcedure", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.IsSearchOnlyProcedure));
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
                {
                    totalCount = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        //20200222 TBL Mod TMV1: Lấy đăng ký của liệu trình
        public List<PatientRegistrationDetail> SearchRegistrationsForProcess(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            totalCount = 0;
            if (criteria == null)
            {
                return null;
            }

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSearchRegistrationsForProcess", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(criteria.PatientCode));
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
                {
                    totalCount = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        //▼===== #016
        public List<PatientRegistrationDetail> SearchRegistrationsForMedicalExaminationDiag(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            totalCount = 0;
            if (criteria == null)
            {
                return null;
            }

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSearchRegistrationsForMedicalExaminationDiag", cn);
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
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ToDate));
                cmd.AddParameter("@V_RegistrationStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.RegStatus));
                cmd.AddParameter("@IsHoanTat", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.IsHoanTat));
                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(pageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(pageIndex));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, ConvertNullObjectToDBNull(bCountTotal));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.OrderBy));
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);
                cmd.AddParameter("@PtRegistrationCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(criteria.PtRegistrationCode));
                cmd.AddParameter("@HosClientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.HosClientID));
                cmd.AddParameter("@DeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptLocationID));
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
                {
                    totalCount = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        //▲===== #016
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
        public bool UpdateRegistrationDetailsStatus(long registrationID, List<PatientRegistrationDetail> regDetailList, DbConnection conn, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
            return true;
        }


        public bool UpdatePclRequestStatus(long registrationID, List<PatientPCLRequest> regPclList, DbConnection conn, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
            return true;
        }

        public bool UpdateProgSumMinusMinHIForRegistration(long registrationID, decimal minHi, decimal progSumMinusMinHi, out decimal curProgSumMinusMinHi, DbConnection conn, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
            return true;
        }
        public bool CorrectHiRegistration(long registrationID, bool hiMinPayExceeded, DbConnection conn, DbTransaction tran, long? returnedOutInvoiceID = null)
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
            CleanUpConnectionAndCommand(null, cmd);
            return true;
        }

        public bool CorrectRegistrationDetails(long registrationID, DbConnection conn, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
            return true;
        }

        public decimal GetTotalPatientPayForTransaction(long transactionID)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                return GetTotalPatientPayForTransaction(transactionID);
            }
        }
        public decimal GetTotalPatientPayForTransaction(long transactionID, DbConnection conn, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
            if (retVal != null && retVal != DBNull.Value)
            {
                return (decimal)retVal;
            }
            return 0;
        }

        public decimal GetTotalPatientPayForTransaction_InPt(long transactionID)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                return GetTotalPatientPayForTransaction_InPt(transactionID);
            }
        }
        public decimal GetTotalPatientPayForTransaction_InPt(long transactionID, DbConnection conn, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
            if (retVal != null && retVal != DBNull.Value)
            {
                return (decimal)retVal;
            }
            return 0;
        }

        public AxServerConfigSection GetApplicationConfigValues()
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
                CleanUpConnectionAndCommand(cn, cmd);
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

        public bool BuildPCLExamTypeDeptLocMap()
        {
            dttest = DateTime.Now;
            MAPPCLExamTypeDeptLoc = (Dictionary<long, PCLExamType>)BuildPCLExamTypeDeptLocMapBase(true);
            if (MAPPCLExamTypeDeptLoc != null)
                return true;

            return false;
        }

        public bool BuildPclDeptLocationList()
        {
            ListAllPCLExamTypeLocations = GetAllPclExamTypeLocations();
            if (ListAllPCLExamTypeLocations != null)
                return true;

            return false;
        }

        public bool BuildAllServiceIdDeptLocMap()
        {
            MAPServiceIdAndDeptLocIDs = BuildAllServiceIdDeptLocMapBase();
            if (MAPServiceIdAndDeptLocIDs != null)
                return true;

            return false;
        }

        public bool TestDatabaseConnectionOK()
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

        public void BuildAllRefGenericRelationMap()
        {
            try
            {
                MAPRefGenericRelation = BuildAllRefGenericRelation();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Hospital SearchHospitalByHICode(string HiCode)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }

        }

        public List<Hospital> LoadCrossRegionHospitals()
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
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public List<Hospital> SearchHospitals(string HospitalName, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
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
                {
                    totalCount = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }

        }

        public List<Hospital> SearchHospitalsNew(HospitalSearchCriteria entity, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
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
                {
                    totalCount = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
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


        public List<RegistrationSummaryInfo> SearchRegistrationSummaryList(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount, out RegistrationsTotalSummary totalSummaryInfo)
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
                {
                    totalCount = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public bool CloseRegistration(long registrationID, AllLookupValues.RegistrationClosingStatus closingStatus, DbConnection conn, DbTransaction tran)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }
        public bool UpdateRegistrationStatus(PatientRegistration registration, DbConnection conn, DbTransaction tran)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                    return retVal > 0;
                }
                catch (SqlException ex)
                {
                    AxLogger.Instance.LogError(ex.ToString());
                    throw new Exception(ex.Message);
                }
            }
        }
        public PaperReferal GetLatestPaperReferal(long hiid)
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
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
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
        public bool GetInPatientRegistrationNonFinalizedLiabilities(long registrationID, bool GetSumOfCashAdvBalanceOnly, DbConnection conn, DbTransaction tran, out decimal TotalLiabilities, out decimal SumOfAdvance, out decimal TotalPatientPayment_PaidInvoice, out decimal TotalRefundPatient, out decimal TotalCashAdvBalanceAmount
                                                                            , out decimal TotalCharityOrgPayment, out decimal TotalPatientPayment_NotFinalized, out decimal TotalPatientPaid_NotFinalized, out decimal TotalCharityOrgPayment_NotFinalized)
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
                CleanUpConnectionAndCommand(null, cmd);
                return false;
            }
            CleanUpConnectionAndCommand(null, cmd);
            return true;
        }
        protected override OutwardDrugClinicDept GetOutwardDrugClinicDeptFromReader(IDataReader reader, long? staffID = null)
        {
            //Phai lai vi ben Ny lay Qty y nghia khac
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
        public bool GetInPatientRegistrationNonFinalizedLiabilities(long registrationID, bool GetSumOfCashAdvBalanceOnly, out decimal TotalLiabilities, out decimal SumOfAdvance, out decimal TotalPatientPayment_PaidInvoice, out decimal TotalRefundPatient, out decimal TotalCashAdvBalanceAmount
                                                                            , out decimal TotalCharityOrgPayment, out decimal TotalPatientPayment_NotFinalized, out decimal TotalPatientPaid_NotFinalized, out decimal TotalCharityOrgPayment_NotFinalized)

        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                return GetInPatientRegistrationNonFinalizedLiabilities(registrationID, GetSumOfCashAdvBalanceOnly, cn, null, out TotalLiabilities, out SumOfAdvance, out TotalPatientPayment_PaidInvoice, out TotalRefundPatient, out TotalCashAdvBalanceAmount
                                                                        , out TotalCharityOrgPayment, out TotalPatientPayment_NotFinalized, out TotalPatientPaid_NotFinalized, out TotalCharityOrgPayment_NotFinalized);

            }
        }

        public bool InsertInPatientAdmDisDetails(InPatientAdmDisDetails entity, long StaffID, long Staff_DeptLocationID, out long admissionID)
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                return InsertInPatientAdmDisDetails(entity, StaffID, Staff_DeptLocationID, cn, null, out admissionID);
            }
        }

        public bool InsertInPatientAdmDisDetails(InPatientAdmDisDetails entity, long StaffID, long Staff_DeptLocationID, DbConnection conn, DbTransaction tran, out long admissionID)
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
                cmd.AddParameter("@IsGuestEmergencyAdmission", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.IsGuestEmergencyAdmission));
                cmd.AddParameter("@IsTreatmentCOVID", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.IsTreatmentCOVID));
                //--▼-- DatTB 20220316 Thêm xác nhận loại bệnh nhân trước khi nhập viện, xác nhận hoãn tạm ứng. lý do
                cmd.AddParameter("@V_ObjectType", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_ObjectType));
                cmd.AddParameter("@IsPostponementAdvancePayment", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.IsPostponementAdvancePayment));
                cmd.AddParameter("@PostponementAdvancePaymentNote", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.PostponementAdvancePaymentNote));
                //--▲-- DatTB 20220316 Thêm xác nhận loại bệnh nhân trước khi nhập viện, xác nhận hoãn tạm ứng. lý do
                int retVal = ExecuteNonQuery(cmd);
                admissionID = (long)cmd.Parameters["@InPatientAdmDisDetailID"].Value;
                CleanUpConnectionAndCommand(null, cmd);
                return retVal > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool UpdateInPatientDischargeInfo(InPatientAdmDisDetails entity, long StaffID, long? ConfirmedDTItemID, DbConnection conn, DbTransaction tran)
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
                cmd.AddParameter("@ConfirmedDTItemID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ConfirmedDTItemID));
                cmd.AddParameter("@DischargeStatus", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DischargeStatus));
                cmd.AddParameter("@TreatmentDischarge", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TreatmentDischarge));
                if(entity.DischargePapersInfo != null)
                {
                    cmd.AddParameter("@HeadOfDepartmentDoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DischargePapersInfo.HeadOfDepartmentDoctorStaffID));
                    cmd.AddParameter("@UnitLeaderDoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DischargePapersInfo.UnitLeaderDoctorStaffID));
                    cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DischargePapersInfo.DoctorStaffID));
                    if (entity.DischargePapersInfo.IsPregnancyTermination)
                    {
                        cmd.AddParameter("@IsPregnancyTermination", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DischargePapersInfo.IsPregnancyTermination));
                        cmd.AddParameter("@PregnancyTerminationDateTime", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.DischargePapersInfo.PregnancyTerminationDateTime));
                        cmd.AddParameter("@ReasonOfPregnancyTermination", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DischargePapersInfo.ReasonOfPregnancyTermination));
                        cmd.AddParameter("@FetalAge", SqlDbType.Int, ConvertNullObjectToDBNull(entity.DischargePapersInfo.FetalAge));
                    }
                    cmd.AddParameter("@NumberDayOfLeaveForTreatment", SqlDbType.Int, ConvertNullObjectToDBNull(entity.DischargePapersInfo.NumberDayOfLeaveForTreatment));
                    cmd.AddParameter("@FromDateLeaveForTreatment", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.DischargePapersInfo.FromDateLeaveForTreatment));
                    cmd.AddParameter("@ToDateLeaveForTreatment", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.DischargePapersInfo.ToDateLeaveForTreatment));
                }
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(null, cmd);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public bool RevertDischarge(InPatientAdmDisDetails entity, long staffID)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                    return retVal > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public bool CheckBeforeDischarge(long registrationID, long DischargeDeptID, bool ConfirmNotTreatedAsInPt
            , DateTime? DischargeDate
            //▼==== #038
            , bool IsSendingCheckMedicalFiles
            //▲==== #038
            , out string errorMessages
            , out string confirmMessages)
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
                    cmd.AddParameter("@DischargeDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(DischargeDate));
                    //▼==== #038
                    cmd.AddParameter("@IsSendingCheckMedicalFiles", SqlDbType.Bit, IsSendingCheckMedicalFiles);
                    //▲==== #038
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
                    CleanUpConnectionAndCommand(cn, cmd);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void CreateForm02(InPatientRptForm02 CurrentRptForm02, List<InPatientBillingInvoice> billingInvoices)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public IList<InPatientRptForm02> GetForm02(long PtRegistrationID)
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
                    CleanUpConnectionAndCommand(cn, cmd);
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
                    CleanUpConnectionAndCommand(conn, cmd);
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

        public PatientPCLRequest UpdateDrAndDiagTrmtForPCLReq(long ServiceRecID, long PCLReqID, long DoctorStaffID)
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
                    CleanUpConnectionAndCommand(conn, cmd);
                    return UpdatedReq;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
        public bool AddUpdateServiceForRegistration(long registrationID, long StaffID
            , long CollectorDeptLocID
            , bool ProgSumMinusMinHIModified, decimal? ProgSumMinusMinHINewValue
            , long PatientMedicalRecordID
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
            , long? V_TradingPlaces
            , bool IsNotCheckInvalid
            , bool IsCheckPaid
            , long? AppointmentID
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
            , double? PtInsuranceBenefit = null
            , bool CalledByPayForRegistration = false
            , IList<InPatientBillingInvoice> aBillingInvoiceCollection = null, long PatientID = 0
            //▼====: #022
            , bool IsRefundBilling = false, bool IsProcess = false, bool IsSettlement = false
            , string TranPaymtNote = null, long? V_PaymentMode = null
            //▲====: #022
            //▼====: #039
            , bool IsFromRequestDoctor = false
            //▲====: #039
            , long? V_ReceiveMethod = null
            )
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                try
                {
                    SqlCommand cmd;

                    cmd = (SqlCommand)conn.CreateCommand();
                    //20200222 TBL Mod TMV1: Nếu IsProcess = true sẽ chạy store mới cho liệu trình
                    if (!IsProcess)
                    {
                        cmd.CommandText = "sp_AddUpdateServiceForRegistration_New";
                    }
                    else
                    {
                        cmd.CommandText = "sp_AddUpdateServiceForRegistration_ForProcess";
                    }
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = int.MaxValue;
                    cmd.AddParameter("@V_TradingPlaces", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_TradingPlaces));
                    cmd.AddParameter("@IsNotCheckInvalid", SqlDbType.Bit, ConvertNullObjectToDBNull(IsNotCheckInvalid));
                    cmd.AddParameter("@IsCheckPaid", SqlDbType.Bit, ConvertNullObjectToDBNull(IsCheckPaid));
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, registrationID);
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, StaffID);
                    cmd.AddParameter("@IsRefundBilling", SqlDbType.Bit, IsRefundBilling);
                    var BillingInvoiceXml = ConvertBillingInvoicesToXml(aBillingInvoiceCollection, 0, false);
                    if (BillingInvoiceXml != null)
                    {
                        cmd.AddParameter("@BillingInvoiceXml", SqlDbType.Xml, BillingInvoiceXml.ToString());
                    }
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
                        pclRequestListString = ConvertPCLRequestsToXml(newPclRequestList, PatientMedicalRecordID, PatientID, StaffID).ToString();
                    }
                    cmd.AddParameter("@PclRequestList_Add", SqlDbType.Xml, ConvertNullObjectToDBNull(pclRequestListString));
                    pclRequestListString = null;
                    if (modifiedPclRequestList != null && modifiedPclRequestList.Count > 0)
                    {
                        pclRequestListString = ConvertPCLRequestsToXml(modifiedPclRequestList, PatientMedicalRecordID, PatientID, StaffID).ToString();
                    }
                    cmd.AddParameter("@PclRequestList_Update", SqlDbType.Xml, ConvertNullObjectToDBNull(pclRequestListString));
                    pclRequestListString = null;
                    if (deletedPclRequestList != null && deletedPclRequestList.Count > 0)
                    {
                        pclRequestListString = ConvertPCLRequestsToXml(deletedPclRequestList, PatientMedicalRecordID, PatientID, StaffID).ToString();
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


                    bool bRunStoreProcOf_AddListPayment = false;
                    if (CalledByPayForRegistration)
                    {
                        bRunStoreProcOf_AddListPayment = true;
                    }

                    if (bRunStoreProcOf_AddListPayment)
                    {
                        cmd.AddParameter("@CombineTransPaymt_InsertXML", SqlDbType.Bit, ConvertNullObjectToDBNull(bRunStoreProcOf_AddListPayment));
                        cmd.AddParameter("@PaymentIDListNy", SqlDbType.Xml, DBNull.Value, ParameterDirection.Output);
                        cmd.AddParameter("@PaymentIDList", SqlDbType.Xml, DBNull.Value, ParameterDirection.Output);

                        cmd.AddParameter("@IsCashAdvance", SqlDbType.Bit, ConvertNullObjectToDBNull(Globals.AxServerSettings.CommonItems.EnableOutPtCashAdvance));
                        cmd.AddParameter("@OutPtCashAdvanceReceiptNumber_2", SqlDbType.VarChar, ConvertNullObjectToDBNull(GetOutPtCashAdvReceiptNum((int)AllLookupValues.PatientFindBy.NGOAITRU)));

                        cmd.AddParameter("@BalanceServicesXML_2", SqlDbType.Xml, ConvertNullObjectToDBNull(OutputBalanceServicesXML));
                        cmd.AddParameter("@IsReported", SqlDbType.Bit, ConvertNullObjectToDBNull(IsReported));

                        if (ConfirmHIStaffID.HasValue && ConfirmHIStaffID.Value > 0 && modifiedInvoiceList != null && modifiedInvoiceList.Count() > 0)
                        {
                            cmd.AddParameter("@OutPtCashAdvanceReceiptNumberExt", SqlDbType.VarChar, ConvertNullObjectToDBNull(GetOutPtCashAdvReceiptNum((int)AllLookupValues.PatientFindBy.NGOAITRU)));
                        }
                    }

                    if(IsSettlement)
                    {
                        cmd.AddParameter("@IsSettlement", SqlDbType.Bit, ConvertNullObjectToDBNull(IsSettlement));
                    }
                    cmd.AddParameter("@AppointmentID", SqlDbType.BigInt, ConvertNullObjectToDBNull(AppointmentID));
                    //▼====: #022
                    cmd.AddParameter("@TranPaymtNote", SqlDbType.NVarChar, ConvertNullObjectToDBNull(TranPaymtNote));
                    cmd.AddParameter("@V_PaymentMode", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_PaymentMode));
                    //▲====: #022
                    //▼====: #039
                    if (IsFromRequestDoctor)
                    {
                        cmd.AddParameter("@IsFromRequestDoctor", SqlDbType.Bit, ConvertNullObjectToDBNull(IsFromRequestDoctor));
                    }
                    //▲====: #039
                    cmd.AddParameter("@V_ReceiveMethod", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_ReceiveMethod));
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

                    PaymentIDListNy = null;
                    newPaymentIDList = null;
                    if (bRunStoreProcOf_AddListPayment)
                    {
                        PaymentIDListNy = cmd.Parameters["@PaymentIDListNy"].Value as string;

                        idList = cmd.Parameters["@PaymentIDList"].Value as string;
                        IDListOutput = DeserializeFromXml<IDListOutput<long>>(idList);
                        newPaymentIDList = IDListOutput != null ? IDListOutput.Ids : null;
                    }


                    // TxD 31/03/2019 BEGIN: Commented OUT the following that call AddListPayment and combine the above store proc with the Store called by AddListPayment ===============
                    //string ListOutputExt = null;
                    //if (cmd.Parameters["@ListOutputExt"].Value != null && cmd.Parameters["@ListOutputExt"].Value != DBNull.Value)
                    //{
                    //    ListOutputExt = cmd.Parameters["@ListOutputExt"].Value as string;
                    //}
                    //bool bOK = AddListPayment(CollectorDeptLocID,
                    //            cmd.Parameters["@ListOutput"].Value as string,
                    //            out PaymentIDListNy, out newPaymentIDList,
                    //            (int)AllLookupValues.PatientFindBy.NGOAITRU, conn, null,
                    //            Globals.AxServerSettings.CommonItems.EnableOutPtCashAdvance,
                    //            registrationID, OutputBalanceServicesXML,
                    //            ConfirmHIStaffID, ListOutputExt, IsReported);

                    //if (!bOK)
                    //{

                    //    //if (tran != null)
                    //    //    tran.Rollback();

                    //    return false;
                    //}
                    // TxD 31/03/2019 END ===============

                    object retVal = cmd.Parameters["@ReturnValue"].Value;
                    CleanUpConnectionAndCommand(conn, cmd);

                    if (retVal != null && retVal != DBNull.Value)
                    {
                        return (int)retVal == 1;
                    }

                    return true;
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

                    throw;
                }
            }
        }

        //private bool AddListPayment(long CollectorDeptLocID, string ListOutput, out string PaymentIDListNy, out List<long> newPaymentIDList, int bNgoaiTru, DbConnection conn, DbTransaction tran)
        //{
        //    return AddListPayment(CollectorDeptLocID, ListOutput, out PaymentIDListNy, out newPaymentIDList, bNgoaiTru, conn, tran);
        //}
        private string GetOutPtCashAdvReceiptNum(int aNgoaiTru)
        {
            return new ServiceSequenceNumberProvider().GetReceiptNumber_NgoaiTru(aNgoaiTru);
        }
        //private bool AddListPayment(long CollectorDeptLocID, string ListOutput, out string PaymentIDListNy, out List<long> newPaymentIDList, int bNgoaiTru
        //    , DbConnection conn, DbTransaction tran, bool IsCashAdvance = false, long PtRegistrationID = 0, string OutputBalanceServicesXML = null
        //    , long? ConfirmHIStaffID = null
        //    , string ListOutputExt = null
        //    , bool IsReported = false)
        //{
        //    PaymentIDListNy = null;
        //    newPaymentIDList = null;
        //    List<PatientTransactionPayment> ListPaymentIDAndService = new List<PatientTransactionPayment>();
        //    List<PatientTransactionPayment> ListPaymentIDAndServiceExt = new List<PatientTransactionPayment>();
        //    if (!string.IsNullOrEmpty(ListOutput) || !string.IsNullOrEmpty(ListOutputExt))
        //    {
        //        if (!string.IsNullOrEmpty(ListOutput))
        //        {
        //            XDocument xdoc1 = XDocument.Parse(ListOutput);
        //            var items = from _student in xdoc1.Element("Root").Elements("IDList") select _student;
        //            foreach (var _student in items)
        //            {
        //                PatientTransactionPayment p = new PatientTransactionPayment();
        //                p.PtPmtAccID = string.IsNullOrEmpty(_student.Element("PtPmtAccID").Value) ? 0 : Convert.ToInt64(_student.Element("PtPmtAccID").Value);
        //                p.InvoiceID = _student.Element("InvoiceID").Value;
        //                p.TransactionID = string.IsNullOrEmpty(_student.Element("TransactionID").Value) ? 0 : Convert.ToInt64(_student.Element("TransactionID").Value);
        //                p.IntRcptTypeID = string.IsNullOrEmpty(_student.Element("IntRcptTypeID").Value) ? 0 : Convert.ToInt64(_student.Element("IntRcptTypeID").Value);
        //                p.TranFinalizationID = string.IsNullOrEmpty(_student.Element("TranFinalizationID").Value) ? 0 : Convert.ToInt64(_student.Element("TranFinalizationID").Value);
        //                p.PaymentDate = string.IsNullOrEmpty(_student.Element("PaymentDate").Value) ? DateTime.Now : Convert.ToDateTime(_student.Element("PaymentDate").Value);
        //                p.PayAmount = Convert.ToDecimal(_student.Element("PayAmount").Value);
        //                p.ManualReceiptNumber = _student.Element("ManualReceiptNumber").Value;
        //                p.V_Currency = string.IsNullOrEmpty(_student.Element("V_Currency").Value) ? 0 : Convert.ToInt64(_student.Element("V_Currency").Value);
        //                p.V_PaymentType = string.IsNullOrEmpty(_student.Element("V_PaymentType").Value) ? 0 : Convert.ToInt64(_student.Element("V_PaymentType").Value);
        //                p.V_PaymentMode = string.IsNullOrEmpty(_student.Element("V_PaymentMode").Value) ? 0 : Convert.ToInt64(_student.Element("V_PaymentMode").Value);
        //                p.CreditOrDebit = Convert.ToInt16(_student.Element("CreditOrDebit").Value);
        //                p.StaffID = Convert.ToInt64(_student.Element("StaffID").Value);
        //                p.CollectorDeptLocID = CollectorDeptLocID;
        //                p.TranPaymtNote = _student.Element("TranPaymtNote").Value;
        //                p.TranPaymtStatus = Convert.ToInt16(_student.Element("TranPaymtStatus").Value);
        //                p.V_TradingPlaces = Convert.ToInt64(_student.Element("V_TradingPlaces").Value);
        //                p.XMLLink = _student.Element("TranItemIDXML").ToString().Replace("<TranItemIDXML>", "").Replace("</TranItemIDXML>", "");
        //                p.XMLService = _student.Element("ServiceXML").ToString().Replace("<ServiceXML>", "").Replace("</ServiceXML>", "");
        //                p.DiscountAmount = _student.Element("DiscountAmount") == null || string.IsNullOrEmpty(_student.Element("DiscountAmount").Value) ? 0 : Convert.ToDecimal(_student.Element("DiscountAmount").Value);
        //                ListPaymentIDAndService.Add(p);
        //            }
        //        }

        //        if (conn.State != ConnectionState.Open)
        //        {
        //            conn.Open();
        //        }

        //        SqlCommand cmd;

        //        cmd = (SqlCommand)conn.CreateCommand();
        //        cmd.Transaction = (SqlTransaction)tran;
        //        cmd.CommandText = "spPatientTransactionPayment_InsertXML";
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.CommandTimeout = int.MaxValue;

        //        //lam sao tra lai so khi bi loi day
        //        cmd.AddParameter("@PaymentXML", SqlDbType.Xml, GetPatientTransactionPaymentToXML(ListPaymentIDAndService, bNgoaiTru, IsCashAdvance).ToString().Replace("&lt;", "<").Replace("&gt;", ">"));
        //        cmd.AddParameter("@PaymentIDListNy", SqlDbType.Xml, DBNull.Value, ParameterDirection.Output);
        //        cmd.AddParameter("@PaymentIDList", SqlDbType.Xml, DBNull.Value, ParameterDirection.Output);

        //        cmd.AddParameter("@IsCashAdvance", SqlDbType.Bit, ConvertNullObjectToDBNull(IsCashAdvance));
        //        cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));

        //        if (IsCashAdvance && ListPaymentIDAndService != null && ListPaymentIDAndService.Sum(x => x.PayAmount) != 0)
        //        {
        //            cmd.AddParameter("@OutPtCashAdvanceReceiptNumber", SqlDbType.VarChar, ConvertNullObjectToDBNull(GetOutPtCashAdvReceiptNum(bNgoaiTru)));
        //        }

        //        cmd.AddParameter("@ConfirmHIStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ConfirmHIStaffID));
        //        cmd.AddParameter("@BalanceServicesXML", SqlDbType.Xml, ConvertNullObjectToDBNull(OutputBalanceServicesXML));
        //        cmd.AddParameter("@IsReported", SqlDbType.Bit, ConvertNullObjectToDBNull(IsReported));

        //        if (!string.IsNullOrEmpty(ListOutputExt))
        //        {
        //            XDocument xdoc2 = XDocument.Parse(ListOutputExt);
        //            var items2 = from _student in xdoc2.Element("Root").Elements("IDList")
        //                         select _student;
        //            foreach (var _student in items2)
        //            {
        //                PatientTransactionPayment p = new PatientTransactionPayment();
        //                p.PtPmtAccID = string.IsNullOrEmpty(_student.Element("PtPmtAccID").Value) ? 0 : Convert.ToInt64(_student.Element("PtPmtAccID").Value);
        //                p.InvoiceID = _student.Element("InvoiceID").Value;
        //                p.TransactionID = string.IsNullOrEmpty(_student.Element("TransactionID").Value) ? 0 : Convert.ToInt64(_student.Element("TransactionID").Value);
        //                p.IntRcptTypeID = string.IsNullOrEmpty(_student.Element("IntRcptTypeID").Value) ? 0 : Convert.ToInt64(_student.Element("IntRcptTypeID").Value);
        //                p.TranFinalizationID = string.IsNullOrEmpty(_student.Element("TranFinalizationID").Value) ? 0 : Convert.ToInt64(_student.Element("TranFinalizationID").Value);
        //                p.PaymentDate = string.IsNullOrEmpty(_student.Element("PaymentDate").Value) ? DateTime.Now : Convert.ToDateTime(_student.Element("PaymentDate").Value);
        //                p.PayAmount = Convert.ToDecimal(_student.Element("PayAmount").Value);
        //                p.ManualReceiptNumber = _student.Element("ManualReceiptNumber").Value;
        //                p.V_Currency = string.IsNullOrEmpty(_student.Element("V_Currency").Value) ? 0 : Convert.ToInt64(_student.Element("V_Currency").Value);
        //                p.V_PaymentType = string.IsNullOrEmpty(_student.Element("V_PaymentType").Value) ? 0 : Convert.ToInt64(_student.Element("V_PaymentType").Value);
        //                p.V_PaymentMode = string.IsNullOrEmpty(_student.Element("V_PaymentMode").Value) ? 0 : Convert.ToInt64(_student.Element("V_PaymentMode").Value);
        //                p.CreditOrDebit = Convert.ToInt16(_student.Element("CreditOrDebit").Value);
        //                p.StaffID = Convert.ToInt64(_student.Element("StaffID").Value);
        //                p.CollectorDeptLocID = CollectorDeptLocID;
        //                p.TranPaymtNote = _student.Element("TranPaymtNote").Value;
        //                p.TranPaymtStatus = Convert.ToInt16(_student.Element("TranPaymtStatus").Value);
        //                p.V_TradingPlaces = Convert.ToInt64(_student.Element("V_TradingPlaces").Value);
        //                p.XMLLink = _student.Element("TranItemIDXML").ToString().Replace("<TranItemIDXML>", "").Replace("</TranItemIDXML>", "");
        //                p.XMLService = _student.Element("ServiceXML").ToString().Replace("<ServiceXML>", "").Replace("</ServiceXML>", "");
        //                p.DiscountAmount = _student.Element("DiscountAmount") == null || string.IsNullOrEmpty(_student.Element("DiscountAmount").Value) ? 0 : Convert.ToDecimal(_student.Element("DiscountAmount").Value);
        //                ListPaymentIDAndServiceExt.Add(p);
        //            }
        //            cmd.AddParameter("@PaymentXMLExt", SqlDbType.Xml, GetPatientTransactionPaymentToXML(ListPaymentIDAndServiceExt, bNgoaiTru, IsCashAdvance).ToString().Replace("&lt;", "<").Replace("&gt;", ">"));
        //            if (IsCashAdvance && ListPaymentIDAndServiceExt != null && ListPaymentIDAndServiceExt.Sum(x => x.PayAmount) != 0)
        //            {
        //                cmd.AddParameter("@OutPtCashAdvanceReceiptNumberExt", SqlDbType.VarChar, ConvertNullObjectToDBNull(GetOutPtCashAdvReceiptNum(bNgoaiTru)));
        //            }
        //        }

        //        ExecuteNonQuery(cmd);

        //        var idList = cmd.Parameters["@PaymentIDList"].Value as string;
        //        var IDListOutput = DeserializeFromXml<IDListOutput<long>>(idList);
        //        newPaymentIDList = IDListOutput != null ? IDListOutput.Ids : null;

        //        PaymentIDListNy = cmd.Parameters["@PaymentIDListNy"].Value as string;
        //        CleanUpConnectionAndCommand(null, cmd);
        //        if (newPaymentIDList != null && newPaymentIDList.Count > 0)
        //        {
        //            return true;
        //        }
        //        else if (Globals.AxServerSettings.CommonItems.EnableOutPtCashAdvance && IsCashAdvance && PtRegistrationID > 0)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        public bool SavePCLRequestsForInPt(long registrationID, long StaffID, long PatientMedicalRecordID, long ReqFromDeptLocID, long ReqFromDeptID, List<PatientPCLRequest> newPclRequestList
                                            , PatientPCLRequest deletedPclRequest, List<PatientPCLRequestDetail> lstDetail_ReUseServiceSeqNum, long V_RegistrationType, bool IsNotCheckInvalid
                                            , out List<long> newPclRequestIdList, bool IsFromMergeRegistration = false)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                try
                {
                    SqlCommand cmd;
                    cmd = (SqlCommand)conn.CreateCommand();
                    if (!IsFromMergeRegistration)
                    {
                        cmd.CommandText = "sp_AddPCLRequestAndDetails_InPt";
                    }
                    else
                    {
                        cmd.CommandText = "sp_AddPCLRequestAndDetails_InPt_Merge";
                    }
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = int.MaxValue;
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, registrationID);
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, StaffID);
                    string pclRequestListString = null;
                    if (newPclRequestList != null && newPclRequestList.Count > 0)
                    {
                        pclRequestListString = ConvertPCLRequestsToXml(newPclRequestList, PatientMedicalRecordID, 0, StaffID).ToString();
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
                    cmd.AddParameter("@IsNotCheckInvalid", SqlDbType.Bit, ConvertNullObjectToDBNull(IsNotCheckInvalid));
                    cmd.AddParameter("@InsertedIDList", SqlDbType.Xml, DBNull.Value, ParameterDirection.Output);
                    ExecuteNonQuery(cmd);
                    var idList = cmd.Parameters["@InsertedIDList"].Value as string;
                    var IDListOutput = DeserializeFromXml<IDListOutput<long>>(idList);
                    newPclRequestIdList = IDListOutput != null ? IDListOutput.Ids : null;
                    CleanUpConnectionAndCommand(conn, cmd);
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
        public bool AddUpdateBillingInvoices(long registrationID
                                   , long patientMedicalRecordID
                                   , List<InPatientBillingInvoice> newInvoiceList
                                   , List<InPatientBillingInvoice> modifiedInvoiceList
                                   , List<InPatientBillingInvoice> deletedInvoiceList
                                   , string modifiedTransactionString
                                   , long V_RegistrationType
                                   , bool IsNotCheckInvalid
                                   , out List<long> newBillingInvoiceList
                                   , out long newPatientTransactionID
                                   , out List<long> newPaymentIDList)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("sp_SaveInPatientBillingInvoiceList", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = int.MaxValue;
                if (V_RegistrationType != (long)AllLookupValues.RegistrationType.NGOAI_TRU)
                {
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, registrationID);
                }
                else
                {
                    cmd.AddParameter("@OutPtRegistrationID", SqlDbType.BigInt, registrationID);
                }
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
                cmd.AddParameter("@IsNotCheckInvalid", SqlDbType.Bit, ConvertNullObjectToDBNull(IsNotCheckInvalid));

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
                CleanUpConnectionAndCommand(conn, cmd);
                if (retVal != null && retVal != DBNull.Value)
                {
                    return (int)retVal == 1;
                }
                return false;
            }
        }
        //public bool UpdateInPatientAdmDisDetails(InPatientAdmDisDetails entity, DeptLocation newDeptLoc, DbConnection conn, DbTransaction tran)
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


        public bool UpdateInPatientAdmDisDetails(InPatientAdmDisDetails entity, DeptLocation newDeptLoc)
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
                    cmd.AddParameter("@IsGuestEmergencyAdmission", SqlDbType.Bit, entity.IsGuestEmergencyAdmission);
                    cmd.AddParameter("@IsTreatmentCOVID", SqlDbType.Bit, entity.IsTreatmentCOVID);
                    //--▼-- DatTB 20220316 Thêm xác nhận loại bệnh nhân trước khi nhập viện, xác nhận hoãn tạm ứng. lý do
                    cmd.AddParameter("@V_ObjectType", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_ObjectType));
                    cmd.AddParameter("@IsPostponementAdvancePayment", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.IsPostponementAdvancePayment));
                    cmd.AddParameter("@PostponementAdvancePaymentNote", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.PostponementAdvancePaymentNote));
                    //--▲-- DatTB 20220316 Thêm xác nhận loại bệnh nhân trước khi nhập viện, xác nhận hoãn tạm ứng. lý do
                    cn.Open();
                    int count = cmd.ExecuteNonQuery();
                    CleanUpConnectionAndCommand(cn, cmd);
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        public InPatientAdmDisDetails GetInPatientAdmissionByID(long admissionId)
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                return GetInPatientAdmissionByID(admissionId, cn, null);
            }
        }
        public InPatientAdmDisDetails GetInPatientAdmissionByID(long admissionId, DbConnection conn, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
            return retVal;
        }
        public bool ChangeInPatientDept(InPatientDeptDetail entity, out long inPatientDeptDetailID)
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                return ChangeInPatientDept(entity, cn, null, out inPatientDeptDetailID);
            }
        }
        public bool ChangeInPatientDept(InPatientDeptDetail entity, DbConnection conn, DbTransaction tran, out long inPatientDeptDetailID)
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
            CleanUpConnectionAndCommand(null, cmd);
            return retVal > 0;
        }
        public bool UpdateAppointmentStatus(long appointmentID, long createdPtRegistrationID, AllLookupValues.ApptStatus newStatus)
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
                CleanUpConnectionAndCommand(conn, cmd);
                return retVal > 0;
            }
        }
        public bool InsertInPatientDeptDetails(InPatientDeptDetail entity, out long inPatientDeptDetailID)
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                return InsertInPatientDeptDetails(entity, cn, null, out inPatientDeptDetailID);
            }
        }
        public bool InsertInPatientDeptDetails(InPatientDeptDetail entity, DbConnection conn, DbTransaction tran, out long inPatientDeptDetailID)
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
            CleanUpConnectionAndCommand(null, cmd);
            return retVal > 0;
        }
        public InPatientAdmDisDetails GetInPatientAdmissionByRegistrationID(long registration, DbConnection conn, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
            return retVal;
        }

        public List<InPatientAdmDisDetails> InPatientAdmDisDetailsSearch(InPatientAdmDisDetailSearchCriteria p, DbConnection conn, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
            return retVal;
        }

        public List<InPatientAdmDisDetails> InPatientAdmDisDetailsSearchPaging(InPatientAdmDisDetailSearchCriteria p, int pageIndex, int pageSize, bool bCountTotal, out int totalCount
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
            {
                totalCount = -1;
            }
            CleanUpConnectionAndCommand(null, cmd);
            return retVal;

        }

        public DateTime? BedPatientAlloc_GetLatestBillToDate(long BedPatientAllocID, DbConnection conn, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
            return retVal as DateTime?;
        }
        public bool AddReportOutPatientCashReceiptList(List<ReportOutPatientCashReceipt> items)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return true;
            }
        }
        public bool UpdatePatientRegistrationStatus(long registrationId, int FindPatient, AllLookupValues.RegistrationStatus newStatus, DbConnection conn, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
            return retVal > 0;
        }

        public List<BedPatientRegDetail> GetAllBedPatientRegDetailsByBedPatientID(long BedPatientId, bool IncludeDeletedItems, DbConnection conn, DbTransaction tran)
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
            CleanUpConnectionAndCommand(null, cmd);
            return retVal;
        }
        public List<BedPatientRegDetail> GetAllBedPatientRegDetailsByBedPatientID(long BedPatientId, bool IncludeDeletedItems)
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                return GetAllBedPatientRegDetailsByBedPatientID(BedPatientId, IncludeDeletedItems, cn, null);
            }
        }

        //* TxD 11/01/2018: Added new parameter ConfirmHICardForInPt_Types to enable joining HI Card
        public bool ApplyHiToInPatientRegistration(long RegistrationID, long HIID, long HisID, double HiBenefit, bool IsCrossRegion, long? PaperReferalID, int FindPatient
            , bool bIsEmergency, long ConfirmHiStaffID, bool IsHICard_FiveYearsCont, bool IsChildUnder6YearsOld, bool IsHICard_FiveYearsCont_NoPaid = false
            , DateTime? FiveYearsAppliedDate = null, DateTime? FiveYearsARowDate = null, TypesOf_ConfirmHICardForInPt eConfirmType = TypesOf_ConfirmHICardForInPt.eConfirmNew
            , bool IsAllowCrossRegion = false)
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
                cmd.AddParameter("@FiveYearsARowDate", SqlDbType.DateTime, FiveYearsARowDate);
                /*==== #002 ====*/
                cmd.AddParameter("@IsChildUnder6YearsOld", SqlDbType.Bit, IsChildUnder6YearsOld);
                //HPT 22/08/2015
                cmd.AddParameter("@IsEmergency", SqlDbType.Bit, bIsEmergency);
                cmd.AddParameter("@PaperReferalID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PaperReferalID));
                cmd.AddParameter("@FindPatient", SqlDbType.Int, ConvertNullObjectToDBNull(FindPatient));
                cmd.AddParameter("@ConfirmHiStaffID", SqlDbType.BigInt, ConfirmHiStaffID);
                cmd.AddParameter("@TypesOfConfirmHICard", SqlDbType.SmallInt, Convert.ToInt16(eConfirmType));

                cmd.AddParameter("@IsAllowCrossRegion", SqlDbType.Bit, IsAllowCrossRegion);
                var retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }

        public bool RemoveHiFromInPatientRegistration(long RegistrationID, bool bIsEmergency, long RemoveHiStaffID, long? OldPaperReferalID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }

        }

        public bool InsertAdditionalFee(long InPatientBillingInvID, long StaffID)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                    return retVal > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        #region InPatientTransferDeptReq
        public List<InPatientTransferDeptReq> GetInPatientTransferDeptReq(InPatientTransferDeptReq p)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return listRG;
            }
        }
        public bool InsertInPatientTransferDeptReq(InPatientTransferDeptReq p)
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
                    cmd.AddParameter("@ConfirmedDTItemID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.ConfirmedDTItemID));
                    cmd.AddParameter("@IsProgressive", SqlDbType.Bit, ConvertNullObjectToDBNull(p.IsProgressive));
                    //cmd.AddParameter("@V_InPatientDeptStatus", SqlDbType.BigInt, AllLookupValues.InPatientDeptStatus.CHUYEN_KHOA);
                    cn.Open();
                    int val = cmd.ExecuteNonQuery();
                    CleanUpConnectionAndCommand(cn, cmd);
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

        public bool UpdateInPatientTransferDeptReq(InPatientTransferDeptReq p)
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
                CleanUpConnectionAndCommand(cn, cmd);
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

        public bool DeleteInPatientTransferDeptReq(long InPatientTransferDeptReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spInPatientTransferDeptReqDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@InPatientTransferDeptReqID", SqlDbType.BigInt, InPatientTransferDeptReqID);

                cn.Open();
                int val = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
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


        public bool UpdateDeleteInPatientDeptDetails(InPatientDeptDetail inDeptDetailToDelete, InPatientDeptDetail inDeptDetailToUpdate, out InPatientDeptDetail inDeptDetailUpdated)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public bool InPatientDeptDetailsTranfer(InPatientDeptDetail p, long InPatientTransferDeptReqID
            , out long InPatientDeptDetailID
            , bool IsAutoCreateOutDeptDiagnosis = false)
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
                    SqlParameter paramBAMedServiceID = new SqlParameter("@BAMedServiceID", SqlDbType.BigInt);
                    SqlParameter paramDoctorStaffID = new SqlParameter("@BADoctorStaffID", SqlDbType.BigInt);
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
                        paramBAMedServiceID.Value = p.BedAllocation.BAMedServiceID;
                        paramDoctorStaffID.Value = p.BedAllocation.DoctorStaffID;
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
                        paramBAMedServiceID.Value = 0;
                        paramDoctorStaffID.Value = 0;
                    }
                    cmd.Parameters.Add(paramBedAllocationID);
                    cmd.Parameters.Add(paramPtRegistrationID);
                    cmd.Parameters.Add(paramResponsibleDeptID);
                    cmd.Parameters.Add(paramCheckInDate);
                    cmd.Parameters.Add(paramExpectedStayingDays);
                    cmd.Parameters.Add(paramCheckOutDate);
                    cmd.Parameters.Add(paramIsActive);
                    cmd.Parameters.Add(paramPatientInBed);
                    cmd.Parameters.Add(paramBAMedServiceID);
                    cmd.Parameters.Add(paramDoctorStaffID);
                    cmd.AddParameter("@InPatientDeptDetailID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                    cmd.AddParameter("@IsAutoCreateOutDeptDiagnosis", SqlDbType.Bit, ConvertNullObjectToDBNull(IsAutoCreateOutDeptDiagnosis));
                    cmd.AddParameter("@IsTreatmentCOVID", SqlDbType.Bit, ConvertNullObjectToDBNull(p.IsTreatmentCOVID));
                    cn.Open();
                    int val = cmd.ExecuteNonQuery();
                    InPatientDeptDetailID = (long)cmd.Parameters["@InPatientDeptDetailID"].Value;
                    CleanUpConnectionAndCommand(cn, cmd);
                    return val > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool DeleteInPatientTransferDeptReqXML(IList<InPatientTransferDeptReq> lstInPtTransDeptReq)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spInPatientTransferDeptReqDeleteXML", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DataXML", SqlDbType.Xml, InPtTransDeptReq_ConvertListToXml(lstInPtTransDeptReq));

                cn.Open();
                int val = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
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

        public bool OutDepartment(InPatientDeptDetail InPtDeptDetail)
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
                    CleanUpConnectionAndCommand(cn, cmd);
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


        public PatientPCLRequestDetail PatientPCLRequestDetails_GetDeptLocID(long PatientPCLReqID)
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
                CleanUpConnectionAndCommand(conn, cmd);
                return obj;
            }
        }

        public PatientRegistrationDetail GetPtRegDetailNewByPatientID(long PatientID)
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
                reader.Close();
                CleanUpConnectionAndCommand(conn, cmd);
                return pr;
            }

        }


        public bool AddInPatientInstruction(PatientRegistration ptRegistration, bool IsUpdateDiagConfirmInPT, long WebIntPtDiagDrInstructionID, out long IntPtDiagDrInstructionID)
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
                if (ptRegistration.InPatientInstruction.IntravenousPlan != null)
                {
                    cmd.AddParameter("@IntravenousXML", SqlDbType.Xml, ConvertBillingInvoicesToXml(ptRegistration.InPatientInstruction.IntravenousPlan).ToString());
                }
                cmd.AddParameter("@IsUpdateDiagConfirmInPT", SqlDbType.Bit, ConvertNullObjectToDBNull(IsUpdateDiagConfirmInPT));
                cmd.AddParameter("@WebIntPtDiagDrInstructionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(WebIntPtDiagDrInstructionID));
                cmd.AddParameter("@IntPtDiagDrInstructionID", SqlDbType.BigInt, 0, ParameterDirection.Output);
                conn.Open();

                int count = ExecuteNonQuery(cmd);
                IntPtDiagDrInstructionID = (long)cmd.Parameters["@IntPtDiagDrInstructionID"].Value;
                CleanUpConnectionAndCommand(conn, cmd);
                return count > 0;
            }
        }
        private List<InPatientInstruction> GetMedicalInstructionCollection(PatientRegistration aRegistration, bool IsGetLast = true, long? aIntPtDiagDrInstructionID = null
            , bool? IsCreatedOutward = null
            , long? V_MedProductType = null
            , long? StoreID=null)
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
                cmd.AddParameter("@IsCreatedOutward", SqlDbType.Bit, ConvertNullObjectToDBNull(IsCreatedOutward));
                cmd.AddParameter("@V_MedProductType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_MedProductType));
                cmd.AddParameter("@StoreID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StoreID));
                conn.Open();
                IDataReader reader = ExecuteReader(cmd);
                cmd.Dispose();
                List<InPatientInstruction> mInPatientInstruction = GetInPatientInstructionCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(conn, cmd);
                return mInPatientInstruction;
            }
        }
        public List<InPatientInstruction> GetInPatientInstructionCollectionForTransmissionMonitor(long PtRegistrationID)
        //PatientRegistration aRegistration, bool IsGetLast = true, long? aIntPtDiagDrInstructionID = null, bool? IsCreatedOutward = null, long? V_MedProductType = null, long? StoreID=null)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetInPatientInstructionCollectionForTransmissionMonitor", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));

                conn.Open();
                IDataReader reader = ExecuteReader(cmd);
                cmd.Dispose();
                List<InPatientInstruction> mInPatientInstruction = GetInPatientInstructionCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(conn, cmd);
                return mInPatientInstruction;
            }
        }
        public List<TransmissionMonitor> GetTransmissionMonitorByInPatientInstructionID(long InPatientInstructionID)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetTransmissionMonitorByInPatientInstructionID", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@InPatientInstructionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(InPatientInstructionID));

                conn.Open();
                IDataReader reader = ExecuteReader(cmd);
                cmd.Dispose();
                List<TransmissionMonitor> mInPatientInstruction = GetInPatientTransmissionMonitorCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(conn, cmd);
                return mInPatientInstruction;
            }
        }
        public bool SaveTransmissionMonitor(TransmissionMonitor CurTransmissionMonitor, long StaffID)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spSaveTransmissionMonitor", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@TransmissionMonitorID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurTransmissionMonitor.TransmissionMonitorID));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurTransmissionMonitor.PtRegistrationID));
                cmd.AddParameter("@IntPtDiagDrInstructionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurTransmissionMonitor.IntPtDiagDrInstructionID));
                cmd.AddParameter("@OutClinicDeptReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurTransmissionMonitor.OutClinicDeptReqID));
                cmd.AddParameter("@TransAmount", SqlDbType.Int, ConvertNullObjectToDBNull(CurTransmissionMonitor.TransAmount));
                cmd.AddParameter("@TransSpeed", SqlDbType.Int, ConvertNullObjectToDBNull(CurTransmissionMonitor.TransSpeed));
                cmd.AddParameter("@StartTime", SqlDbType.DateTime, ConvertNullObjectToDBNull(CurTransmissionMonitor.StartTime));
                cmd.AddParameter("@EndTime", SqlDbType.DateTime, ConvertNullObjectToDBNull(CurTransmissionMonitor.EndTime));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurTransmissionMonitor.StaffID));
                cmd.AddParameter("@UserID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                cmd.AddParameter("@BrandName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurTransmissionMonitor.BrandName));
                cmd.AddParameter("@InBatchNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurTransmissionMonitor.InBatchNumber));
                cmd.AddParameter("@UsageDistance", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurTransmissionMonitor.UsageDistance));

                conn.Open();
                int count = ExecuteNonQuery(cmd);
   
                CleanUpConnectionAndCommand(conn, cmd);
                return count > 0;
            }
        }
        public InPatientInstruction GetInPatientInstruction(PatientRegistration ptRegistration)
        {
            return GetMedicalInstructionCollection(ptRegistration).FirstOrDefault();
        }
        public List<InPatientInstruction> GetInPatientInstructionCollection(PatientRegistration aRegistration)
        {
            return GetMedicalInstructionCollection(aRegistration, false);
        }
        public List<InPatientInstruction> GetInPatientInstructionCollection(PatientRegistration aRegistration, bool? IsCreatedOutward, long V_MedProductType, long StoreID)
        {
            return GetMedicalInstructionCollection(aRegistration, false, null, IsCreatedOutward, V_MedProductType, StoreID);
        }
        public InPatientInstruction GetInPatientInstructionByInstructionID(long aIntPtDiagDrInstructionID)
        {
            return GetMedicalInstructionCollection(null, true, aIntPtDiagDrInstructionID).FirstOrDefault();
        }
        public List<Intravenous> GetIntravenousPlan_InPt(long IntPtDiagDrInstructionID)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("sp_GetIntravenousPlan_InPt", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = int.MaxValue;
                cmd.AddParameter("@IntPtDiagDrInstructionID", SqlDbType.BigInt, IntPtDiagDrInstructionID);
                conn.Open();
                IDataReader reader = ExecuteReader(cmd);
                var retVal = GetIntravenousListFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(conn, cmd);
                return retVal;
            }
        }
        public IList<ReqOutwardDrugClinicDeptPatient> GetAntibioticTreatmentUsageHistory(long PtRegistrationID)
        {
            using (var CurrentConnection = new SqlConnection(ConnectionString))
            {
                var CurrentCommand = new SqlCommand("spGetAntibioticTreatmentUsageHistory", CurrentConnection);
                CurrentCommand.CommandType = CommandType.StoredProcedure;
                CurrentCommand.CommandTimeout = int.MaxValue;
                CurrentCommand.AddParameter("@PtRegistrationID", SqlDbType.BigInt, PtRegistrationID);
                CurrentConnection.Open();
                IDataReader CurrentReader = ExecuteReader(CurrentCommand);
                var ItemCollection = GetReqOutwardDrugClinicDeptPatientCollectionFromReader(CurrentReader);
                CurrentReader.Close();
                CleanUpConnectionAndCommand(CurrentConnection, CurrentCommand);
                return ItemCollection;
            }
        }
        public IList<ReqOutwardDrugClinicDeptPatient> GetAllDrugFromDoctorInstruction(long PtRegistrationID, DateTime CurrentDate)
        {
            using (var CurrentConnection = new SqlConnection(ConnectionString))
            {
                var CurrentCommand = new SqlCommand("spGetAllDrugFromDoctorInstruction", CurrentConnection);
                CurrentCommand.CommandType = CommandType.StoredProcedure;
                CurrentCommand.CommandTimeout = int.MaxValue;
                CurrentCommand.AddParameter("@PtRegistrationID", SqlDbType.BigInt, PtRegistrationID);
                CurrentCommand.AddParameter("@CurrentDate", SqlDbType.DateTime, CurrentDate);
                CurrentConnection.Open();
                IDataReader CurrentReader = ExecuteReader(CurrentCommand);
                var ItemCollection = GetReqOutwardDrugClinicDeptPatientCollectionFromReader(CurrentReader);
                CurrentReader.Close();
                CleanUpConnectionAndCommand(CurrentConnection, CurrentCommand);
                return ItemCollection;
            }
        }
        public bool GetAllItemsByInstructionID(long IntPtDiagDrInstructionID, out List<PatientRegistrationDetail> AllRegistrationItems, out List<PatientPCLRequestDetail> AllPCLRequestItems)
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
                CleanUpConnectionAndCommand(conn, cmd);
                return true;
            }
        }

        public bool SaveInstructionMonitoring(InPatientInstruction InPatientInstruction, long StaffID)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spSaveInstructionMonitoring", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = int.MaxValue;
                cmd.AddParameter("@IntPtDiagDrInstructionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(InPatientInstruction.IntPtDiagDrInstructionID));
                cmd.AddParameter("@PulseAndBloodPressure", SqlDbType.NVarChar, ConvertNullObjectToDBNull(InPatientInstruction.PulseAndBloodPressure));
                cmd.AddParameter("@SpO2", SqlDbType.NVarChar, ConvertNullObjectToDBNull(InPatientInstruction.SpO2));
                cmd.AddParameter("@Temperature", SqlDbType.NVarChar, ConvertNullObjectToDBNull(InPatientInstruction.Temperature));
                cmd.AddParameter("@Sense", SqlDbType.NVarChar, ConvertNullObjectToDBNull(InPatientInstruction.Sense));
                cmd.AddParameter("@BloodSugar", SqlDbType.NVarChar, ConvertNullObjectToDBNull(InPatientInstruction.BloodSugar));
                cmd.AddParameter("@Urine", SqlDbType.NVarChar, ConvertNullObjectToDBNull(InPatientInstruction.Urine));
                cmd.AddParameter("@ECG", SqlDbType.NVarChar, ConvertNullObjectToDBNull(InPatientInstruction.ECG));
                cmd.AddParameter("@PhysicalExamOther", SqlDbType.NVarChar, ConvertNullObjectToDBNull(InPatientInstruction.PhysicalExamOther));
                cmd.AddParameter("@Diet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(InPatientInstruction.Diet));
                cmd.AddParameter("@LevelCare", SqlDbType.BigInt, ConvertNullObjectToDBNull(InPatientInstruction.LevelCare));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                //▼==== #033
                cmd.AddParameter("@RespiratoryRate", SqlDbType.NVarChar, ConvertNullObjectToDBNull(InPatientInstruction.RespiratoryRate));
                //▲==== #033
                conn.Open();
                int result = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(conn, cmd);
                return result > 0;
            }
        }

        public bool AddNewPatientAndHIDetails(Patient newPatientAndHiDetails, out long PatientID, out long HIID, out long PaperReferralID)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                    return (nExecRes > 0);
                }
            }
            catch (DbException dbEx)
            {
                throw dbEx;
            }

        }

        public bool UpdateNewPatientAndHIDetails(Patient updatedPatientAndHiDetails, bool IsAdmin, long StaffID, bool IsEditAfterRegistration, out long PatientID, out long HIID, out long PaperReferralID, out double RebatePercentage, out string Result)
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
                bUpdateResult = UpdateHiItem(out Result, patient.CurrentHealthInsurance, IsEditAfterRegistration, StaffID, null);
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
        public List<PatientRegistrationDetail> SearchInPatientRegistrationListForOST(long DeptID, bool IsSearchForListPatientCashAdvance)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetInPatientRegistrationByDeptID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptID));
                cmd.AddParameter("@IsSearchForListPatientCashAdvance", SqlDbType.Bit, ConvertNullObjectToDBNull(IsSearchForListPatientCashAdvance));
                cn.Open();
                List<PatientRegistrationDetail> retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                retVal = GetPatientRegistrationDetailsCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        //▲====== #013
        public List<PatientRegistrationDetail> SearchInPatientRequestAdmissionListForOST(DateTime? FromDate, DateTime? ToDate)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetInPatientAdmissionList", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(ToDate));
                cn.Open();
                List<PatientRegistrationDetail> retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                retVal = GetPatientRegistrationDetailsCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        //▼====== #015
        public List<eHCMS.Configurations.InsuranceBenefitCategories> GetInsuranceBenefitCategoriesValues()
        {
            List<eHCMS.Configurations.InsuranceBenefitCategories> retVal = null;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllInsuranceBenefitCategories", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                retVal = GetInsuranceBenefitCategoriesCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return retVal;
        }
        //▲====== #015
        //▼====== #016
        public bool UpdateBasicDiagTreatment(PatientRegistration regInfo, DiseasesReference aAdmissionICD10, Staff gSelectedDoctorStaff)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                bool bOK = false;
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spUpdateBasicDiagTreatment";
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, regInfo.PtRegistrationID);
                cmd.AddParameter("@BasicDiagTreatment", SqlDbType.NVarChar, aAdmissionICD10.DiseaseNameVN);
                cmd.AddParameter("@AdmissionICD10Code", SqlDbType.NVarChar, aAdmissionICD10.ICD10Code);
                cmd.AddParameter("@OutHosDiagStaffFullName", SqlDbType.NVarChar, gSelectedDoctorStaff.FullName);
                conn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(conn, cmd);
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
        public PromoDiscountProgram EditPromoDiscountProgram(PromoDiscountProgram aPromoDiscountProgram, long PtRegistrationID)
        {
            using (var mConnection = new SqlConnection(ConnectionString))
            {
                SqlCommand mCommand = new SqlCommand("spEditPromoDiscountProgram", mConnection);
                mCommand.CommandType = CommandType.StoredProcedure;
                mCommand.AddParameter("@PromoDiscountProgram", SqlDbType.Xml, ConvertNullObjectToDBNull(aPromoDiscountProgram.ConvertToXml()));
                mCommand.AddParameter("@PtRegistrationID", SqlDbType.BigInt, PtRegistrationID);
                mCommand.AddParameter("@PromoDiscProgID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                mCommand.AddParameter("@OutPromoDiscCode", SqlDbType.VarChar, 20, DBNull.Value, ParameterDirection.Output);
                mCommand.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(aPromoDiscountProgram.V_RegistrationType));
                mConnection.Open();
                bool mReturnCode = ExecuteNonQuery(mCommand) > 0;
                CleanUpConnectionAndCommand(mConnection, mCommand);
                if (mReturnCode && mCommand.Parameters.Cast<SqlParameter>().Any(x => x.ParameterName == "@PromoDiscProgID" && x.Direction == ParameterDirection.Output && x.Value != null && x.Value != DBNull.Value))
                {
                    aPromoDiscountProgram.PromoDiscProgID = (long)mCommand.Parameters.Cast<SqlParameter>().First(x => x.ParameterName == "@PromoDiscProgID" && x.Direction == ParameterDirection.Output && x.Value != null && x.Value != DBNull.Value).Value;
                }
                if (mReturnCode && mCommand.Parameters.Cast<SqlParameter>().Any(x => x.ParameterName == "@OutPromoDiscCode" && x.Direction == ParameterDirection.Output && x.Value != null && x.Value != DBNull.Value))
                {
                    aPromoDiscountProgram.PromoDiscCode = Convert.ToString(mCommand.Parameters.Cast<SqlParameter>().First(x => x.ParameterName == "@OutPromoDiscCode" && x.Direction == ParameterDirection.Output && x.Value != null && x.Value != DBNull.Value).Value);
                }
                return aPromoDiscountProgram;
            }
        }

        public bool SetEkipForService(PatientRegistration CurrentRegistration)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                bool bOK = false;
                SqlCommand cmd = new SqlCommand("spSetEkipForService", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, CurrentRegistration.PtRegistrationID);
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull((long)CurrentRegistration.V_RegistrationType));
                cmd.AddParameter("@PatientRegistrationDetailXML", SqlDbType.Xml, CurrentRegistration.ConvertDetailsListToXml(CurrentRegistration.PatientRegistrationDetails));
                conn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(conn, cmd);
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
		//▼===== #010

		// VuTTM begin
		public static string PT_REG_PREFIX = "PTRG";
		public static string PCL_REG_PREFIX = "PCL";
		public static string OUTI_REG_PREFIX = "OUT";
		public static string HOS_RECEIPT_SIGN = "CR";
		public static string PHAR_RECEIPT_SIGN = "NT";

		public bool AddBankingRefundDetails(long? outPtCashAdvanceId, long? ptRegDetailID,
			long? pclRequestID, long? outiID, long? bankingRefundTransactionId)
		{
			if (null == outPtCashAdvanceId
				|| null == bankingRefundTransactionId
				|| -1 == bankingRefundTransactionId
				|| (null == ptRegDetailID
					&& null == pclRequestID
					&& null == outiID))
			{
				return false;
			}

			using (var conn = new SqlConnection(ConnectionString))
			{
				SqlCommand cmd = new SqlCommand("AddBankingRefundDetails", conn);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParameter("@OutPtCashAdvanceId", SqlDbType.BigInt,
					ConvertNullObjectToDBNull(outPtCashAdvanceId));
				cmd.AddParameter("@BankingRefundTransactionId",
					SqlDbType.BigInt, ConvertNullObjectToDBNull(bankingRefundTransactionId));
				cmd.AddParameter("@PtRegDetailID",
					SqlDbType.BigInt, ConvertNullObjectToDBNull(ptRegDetailID));
				cmd.AddParameter("@PCLRequestID",
					SqlDbType.BigInt, ConvertNullObjectToDBNull(pclRequestID));
				cmd.AddParameter("@OutiID",
					SqlDbType.BigInt, ConvertNullObjectToDBNull(outiID));
				conn.Open();
				int result = ExecuteNonQuery(cmd);
				CleanUpConnectionAndCommand(conn, cmd);
				if (result == 0)
				{
					return false;
				}
			}

			return true;
		}

		public bool AddPatientSalt(long patientId, string salt)
		{
			if (0 == patientId
				|| String.IsNullOrEmpty(salt))
			{
				return false;
			}
			using (var conn = new SqlConnection(ConnectionString))
			{
				SqlCommand cmd = new SqlCommand("AddPatientSalt", conn);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParameter("@PatientId", SqlDbType.BigInt,
					ConvertNullObjectToDBNull(patientId));
				cmd.AddParameter("@Salt", SqlDbType.VarChar,
					ConvertNullObjectToDBNull(salt));
				conn.Open();
				int result = ExecuteNonQuery(cmd);
				CleanUpConnectionAndCommand(conn, cmd);
				if (result == 0)
				{
					return false;
				}
			}
			return true;
		}

		public bool RemovePatientSalt(long patientId)
		{
			if (0 == patientId)
			{
				return false;
			}
			using (var conn = new SqlConnection(ConnectionString))
			{
				SqlCommand cmd = new SqlCommand("RemovePatientSalt", conn);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParameter("@PatientId", SqlDbType.BigInt,
					ConvertNullObjectToDBNull(patientId));
				conn.Open();
				int result = ExecuteNonQuery(cmd);
				CleanUpConnectionAndCommand(conn, cmd);
				if (result == 0)
				{
					return false;
				}
			}
			return true;
		}

		public bool UpdateOutPatientCashAdvanceByBankingTrxId(long? outPtCashAdvanceId,
			long? bankingTrxId, long paymentMode)
		{
			if (null == outPtCashAdvanceId)
			{
				return false;
			}
			using (var conn = new SqlConnection(ConnectionString))
			{
				SqlCommand cmd = new SqlCommand("UpdateOutPatientCashAdvanceByBankingTrxId", conn);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParameter("@OutPtCashAdvanceId", SqlDbType.BigInt,
					ConvertNullObjectToDBNull(outPtCashAdvanceId));
				cmd.AddParameter("@BankingTrxId", SqlDbType.BigInt,
					ConvertNullObjectToDBNull(bankingTrxId));
				cmd.AddParameter("@PaymentMode", SqlDbType.BigInt,
					ConvertNullObjectToDBNull(paymentMode));
				conn.Open();
				int result = ExecuteNonQuery(cmd);
				CleanUpConnectionAndCommand(conn, cmd);
				if (result == 0)
				{
					return false;
				}
			}
			return true;
		}

        private static readonly string GET_DIFF_RECEIPT_NUM_LST_QUERY = @"
	        SELECT (N'Phiếu ' + opca.CashAdvReceiptNum + N' - Phương Thức Thanh Toán: ' + l.ObjectValue) AS result
	        FROM OutPatientCashAdvance opca
	        LEFT JOIN OutPatientCashAdvanceLink opcal ON opca.OutPtCashAdvanceID = opcal.OutPtCashAdvanceID
	        LEFT JOIN PatientTransactionDetails ptd ON opcal.TransItemID = ptd.TransItemID
	        LEFT JOIN PatientRegistrationDetails prd ON ptd.PtRegDetailID = prd.PtRegDetailID
	        LEFT JOIN BankingRefundDetails brd ON (opca.OutPtCashAdvanceID = brd.OutPtCashAdvanceId
		        AND ((brd.PtRegDetailID IS NOT NULL AND brd.PtRegDetailID = ptd.PtRegDetailID))
			        OR (brd.PCLRequestID IS NOT NULL AND brd.PCLRequestID = ptd.PCLRequestID)
			        OR (brd.OutiID IS NOT NULL AND brd.OutiID = ptd.outiID))
	        LEFT JOIN Lookup l ON opca.V_PaymentMode = l.LookupID
	        WHERE opca.PtRegistrationID = {0}
		        AND opca.IsDeleted = 0
		        AND opca.PaymentAmount > 0
                AND opca.V_PaymentMode <> {1}
                {2}
                {3}
	        ORDER BY opca.OutPtCashAdvanceID asc;
        ";

        public List<string> GetReceiptNumWithDiffPaymentMode(long ptRegistrationID, string cond1,
            string cond2, long paymentMode)
        {
            if (0 == ptRegistrationID)
            {
                return null;
            }

            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(
                        String.Format(GET_DIFF_RECEIPT_NUM_LST_QUERY, ptRegistrationID, paymentMode, cond1, cond2),
                        cn);
                    SqlDataAdapter _Adapter = new SqlDataAdapter(cmd);
                    DataTable _Dt = new DataTable();
                    _Adapter.Fill(_Dt);

                    List<string> _Result = new List<string>();
                    if (null != _Dt
                        && null != _Dt.Rows
                        && _Dt.Rows.Count > 0)
                    {
                        for (int _Idx = 0; _Idx < _Dt.Rows.Count; _Idx++)
                        {
                            _Result.Add(_Dt.Rows[_Idx][0].ToString());
                        }
                    }
                    CleanUpConnectionAndCommand(cn, cmd);
                    return _Result;
                }
            }
            catch (Exception _Ex)
            {
                throw new Exception(_Ex.Message);
            }
        }

		public List<OutPatientCashAdvanceDetails> GetOutPatientCashAdvanceDetailsLst(long ptRegistrationId)
		{
			List<OutPatientCashAdvanceDetails> result = null;
			using (var conn = new SqlConnection(ConnectionString))
			{
				SqlCommand cmd = new SqlCommand("GetOutPtCashAdvanceDetailsByRegId", conn);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParameter("@PtRegistrationId", SqlDbType.BigInt, ptRegistrationId);
				conn.Open();

				IDataReader reader = ExecuteReader(cmd);
				result = new List<OutPatientCashAdvanceDetails>();
				while (reader.Read())
				{
					OutPatientCashAdvanceDetails item = new OutPatientCashAdvanceDetails();
					if (reader.HasColumn("OutPtCashAdvanceID") && reader["OutPtCashAdvanceID"] != DBNull.Value)
					{
						item.OutPtCashAdvanceID = (long)reader["OutPtCashAdvanceID"];
					}
					if (reader.HasColumn("PtRegistrationID") && reader["PtRegistrationID"] != DBNull.Value)
					{
						item.PtRegistrationID = (long)reader["PtRegistrationID"];
					}
					if (reader.HasColumn("BankingTransactionId") && reader["BankingTransactionId"] != DBNull.Value)
					{
						item.BankingTransactionId = (long)reader["BankingTransactionId"];
					}
					if (reader.HasColumn("BankingRefundTransactionId") && reader["BankingRefundTransactionId"] != DBNull.Value)
					{
						item.BankingRefundTransactionId = (long)reader["BankingRefundTransactionId"];
					}
					if (reader.HasColumn("V_PaymentReason") && reader["V_PaymentReason"] != DBNull.Value)
					{
						item.V_PaymentReason = (long)reader["V_PaymentReason"];
					}
					if (reader.HasColumn("V_PaymentMode") && reader["V_PaymentMode"] != DBNull.Value)
					{
						item.V_PaymentMode = (long)reader["V_PaymentMode"];
					}
					if (reader.HasColumn("PtCashAdvanceLinkID") && reader["PtCashAdvanceLinkID"] != DBNull.Value)
					{
						item.PtCashAdvanceLinkID = (long)reader["PtCashAdvanceLinkID"];
					}
					if (reader.HasColumn("TransItemID") && reader["TransItemID"] != DBNull.Value)
					{
						item.TransItemID = (long)reader["TransItemID"];
					}
					if (reader.HasColumn("PtRegDetailID") && reader["PtRegDetailID"] != DBNull.Value)
					{
						item.PtRegDetailID = (long)reader["PtRegDetailID"];
					}
					if (reader.HasColumn("MedServiceID") && reader["MedServiceID"] != DBNull.Value)
					{
						item.MedServiceID = (long)reader["MedServiceID"];
					}
					if (reader.HasColumn("outiID") && reader["outiID"] != DBNull.Value)
					{
						item.outiID = (long)reader["outiID"];
					}
					if (reader.HasColumn("PCLRequestID") && reader["PCLRequestID"] != DBNull.Value)
					{
						item.PCLRequestID = (long)reader["PCLRequestID"];
					}
					if (reader.HasColumn("Amount") && reader["Amount"] != DBNull.Value)
					{
						item.Amount = (decimal)reader["Amount"];
					}

					if (reader.HasColumn("CashAdvReceiptNum") && reader["CashAdvReceiptNum"] != DBNull.Value)
					{
						item.CashAdvReceiptNum = (string)reader["CashAdvReceiptNum"];
					}
					if (reader.HasColumn("CanceledCashAdvReceiptNum") && reader["CanceledCashAdvReceiptNum"] != DBNull.Value)
					{
						item.CanceledCashAdvReceiptNum = (string)reader["CanceledCashAdvReceiptNum"];
					}

					if (reader.HasColumn("PaymentAmount") && reader["PaymentAmount"] != DBNull.Value)
					{
						item.PaymentAmount = (decimal)reader["PaymentAmount"];
					}
					if (reader.HasColumn("BalanceAmount") && reader["BalanceAmount"] != DBNull.Value)
					{
						item.BalanceAmount = (decimal)reader["BalanceAmount"];
					}
					if (reader.HasColumn("PriceDifference") && reader["PriceDifference"] != DBNull.Value)
					{
						item.PriceDifference = (decimal)reader["PriceDifference"];
					}
					if (reader.HasColumn("AmountCoPay") && reader["AmountCoPay"] != DBNull.Value)
					{
						item.AmountCoPay = (decimal)reader["AmountCoPay"];
					}
					if (reader.HasColumn("HealthInsuranceRebate") && reader["HealthInsuranceRebate"] != DBNull.Value)
					{
						item.HealthInsuranceRebate = (decimal)reader["HealthInsuranceRebate"];
					}
					if (reader.HasColumn("ServiceName") && reader["ServiceName"] != DBNull.Value)
					{
						item.ServiceName = (string)reader["ServiceName"];
					}
					result.Add(item);
				}

				reader.Close();
				CleanUpConnectionAndCommand(conn, cmd);
			}
			return result;
		}
		// VuTMM end

		public List<PatientRegistration> SearchRegistrationListForCheckDiagAndPrescription(CheckDiagAndPrescriptionSearchCriteria SearchCriteria)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetRegistrationByDoctorAndDateTime", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DoctorID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.DoctorID));
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(SearchCriteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(SearchCriteria.ToDate));
                cmd.AddParameter("@V_CheckMedicalFilesStatusForDLS", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_CheckMedicalFilesStatusForDLS));
                cmd.AddParameter("@DeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.DeptLocID));
                cmd.AddParameter("@PatientCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria.PatientCode));
                cmd.AddParameter("@FullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria.FullName));
                //▼====: #027
                cmd.AddParameter("@IsConfirmHI", SqlDbType.Bit, ConvertNullObjectToDBNull(SearchCriteria.IsConfirmHI));
                //▲====: #027

                cn.Open();
                List<PatientRegistration> retVal = null;

                IDataReader reader = ExecuteReader(cmd);

                retVal = GetPatientRegistrationCollectionFromReader(reader);

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        //▲===== #010
        public List<PatientRegistration> SearchRegistrationForCirculars56(DateTime FromDate, DateTime ToDate, long PatientFindBy,string PatientCode)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetRegistrationForCirculars56", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientFindBy", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientFindBy));
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(ToDate));
                cmd.AddParameter("@PatientCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(PatientCode));

                cn.Open();
                List<PatientRegistration> retVal = null;

                IDataReader reader = ExecuteReader(cmd);

                retVal = GetPatientRegistrationCollectionFromReader(reader);

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public SummaryMedicalRecords GetSummaryMedicalRecords_ByPtRegID(long PtRegistrationID, long V_RegistrationType)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetSummaryMedicalRecords_ByPtRegID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));


                cn.Open();
                SummaryMedicalRecords retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    retVal = GetSummaryMedicalRecordsFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public PatientTreatmentCertificates GetPatientTreatmentCertificates_ByPtRegID(long PtRegistrationID, long V_RegistrationType, out int LastCode)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetPatientTreatmentCertificates_ByPtRegID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));

                cmd.AddParameter("@LastCode", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                PatientTreatmentCertificates retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    retVal = GetPatientTreatmentCertificatesFromReader(reader);
                }
                reader.Close();
                if (cmd.Parameters["@LastCode"].Value != DBNull.Value)
                {
                    LastCode = (int)cmd.Parameters["@LastCode"].Value;
                }
                else
                {
                    LastCode = 1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public InjuryCertificates GetInjuryCertificates_ByPtRegID(long PtRegistrationID, long V_RegistrationType,out int LastCode)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetInjuryCertificates_ByPtRegID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));

                cmd.AddParameter("@LastCode", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                InjuryCertificates retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    retVal = GetInjuryCertificatesFromReader(reader);
                }
                reader.Close();
                if (cmd.Parameters["@LastCode"].Value != DBNull.Value)
                {
                    LastCode = (int)cmd.Parameters["@LastCode"].Value;
                }
                else
                {
                    LastCode = 1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
              
            }
        }
        public List<BirthCertificates> GetBirthCertificates_ByPtRegID(long PtRegistrationID, long V_RegistrationType, out int LastCode)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetBirthCertificates_ByPtRegID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));

                cmd.AddParameter("@LastCode", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<BirthCertificates> retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader!=null)
                {
                    retVal = GetBirthCertificatesCollectionFromReader(reader);
                }
                reader.Close();
                if (cmd.Parameters["@LastCode"].Value != DBNull.Value)
                {
                    LastCode = (int)cmd.Parameters["@LastCode"].Value;
                }
                else
                {
                    LastCode = 1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
              
            }
        }
        public VacationInsuranceCertificates GetVacationInsuranceCertificates_ByPtRegID(long PtRegistrationID, bool IsPrenatal, out int LastCode)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetVacationInsuranceCertificates_ByPtRegID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@IsPrenatal", SqlDbType.BigInt, ConvertNullObjectToDBNull(IsPrenatal));
                cmd.AddParameter("@LastCode", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                VacationInsuranceCertificates retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    retVal = GetVacationInsuranceCertificatesFromReader(reader);
                }
                reader.Close();
                if (cmd.Parameters["@LastCode"].Value != DBNull.Value)
                {
                    LastCode = (int)cmd.Parameters["@LastCode"].Value;
                }
                else
                {
                    LastCode = 1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
              
            }
        }
        public SummaryMedicalRecords SaveSummaryMedicalRecords(SummaryMedicalRecords CurrentSummaryMedicalRecords, long UserID, string Date)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSaveSummaryMedicalRecords", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@SummaryMedicalRecordID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentSummaryMedicalRecords.SummaryMedicalRecordID));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentSummaryMedicalRecords.PtRegistrationID));
                cmd.AddParameter("@AdmissionDiagnosis", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurrentSummaryMedicalRecords.AdmissionDiagnosis));
                cmd.AddParameter("@DischargeDiagnosis", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurrentSummaryMedicalRecords.DischargeDiagnosis));
                cmd.AddParameter("@PathologicalProcess", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurrentSummaryMedicalRecords.PathologicalProcess));
                cmd.AddParameter("@SummaryResultPCL", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurrentSummaryMedicalRecords.SummaryResultPCL));
                cmd.AddParameter("@Treatment", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurrentSummaryMedicalRecords.Treatment));
                cmd.AddParameter("@DischargeStatus", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurrentSummaryMedicalRecords.DischargeStatus));
                cmd.AddParameter("@Note", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurrentSummaryMedicalRecords.Note));
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentSummaryMedicalRecords.V_RegistrationType));
                cmd.AddParameter("@IsDelete", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentSummaryMedicalRecords.IsDelete));
                cmd.AddParameter("@UserID", SqlDbType.VarChar, ConvertNullObjectToDBNull(UserID));
                cmd.AddParameter("@Date", SqlDbType.VarChar, ConvertNullObjectToDBNull(Date));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(UserID));
                cmd.AddParameter("@V_OutDischargeCondition", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentSummaryMedicalRecords.V_OutDischargeCondition));
                cmd.AddParameter("@ChiefDoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentSummaryMedicalRecords.ChiefDoctorStaffID));


                cn.Open();
                SummaryMedicalRecords retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    retVal = GetSummaryMedicalRecordsFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public PatientTreatmentCertificates SavePatientTreatmentCertificates(PatientTreatmentCertificates CurrentPatientTreatmentCertificates, long UserID, string Date)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSavePatientTreatmentCertificates", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientTreatmentCertificateID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentPatientTreatmentCertificates.PatientTreatmentCertificateID));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentPatientTreatmentCertificates.PtRegistrationID));
                cmd.AddParameter("@PatientTreatmentCertificateCode", SqlDbType.Int, ConvertNullObjectToDBNull(CurrentPatientTreatmentCertificates.PatientTreatmentCertificateCode));
                cmd.AddParameter("@MedicalHistory", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurrentPatientTreatmentCertificates.MedicalHistory));
                cmd.AddParameter("@Diagnosis", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurrentPatientTreatmentCertificates.Diagnosis));
                cmd.AddParameter("@DoctorAdvice", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurrentPatientTreatmentCertificates.DoctorAdvice));
                cmd.AddParameter("@Treatment", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurrentPatientTreatmentCertificates.Treatment));
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentPatientTreatmentCertificates.V_RegistrationType));
                cmd.AddParameter("@IsDelete", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentPatientTreatmentCertificates.IsDelete));
                cmd.AddParameter("@UserID", SqlDbType.VarChar, ConvertNullObjectToDBNull(UserID));
                cmd.AddParameter("@Date", SqlDbType.VarChar, ConvertNullObjectToDBNull(Date));

                cn.Open();
                PatientTreatmentCertificates retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    retVal = GetPatientTreatmentCertificatesFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public InjuryCertificates SaveInjuryCertificates(InjuryCertificates CurrentInjuryCertificates, long UserID, string Date)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSaveInjuryCertificates", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@InjuryCertificateID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentInjuryCertificates.InjuryCertificateID));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentInjuryCertificates.PtRegistrationID));
                cmd.AddParameter("@InjuryCertificateCode", SqlDbType.Int, ConvertNullObjectToDBNull(CurrentInjuryCertificates.InjuryCertificateCode));
                cmd.AddParameter("@ReasonAdmission", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurrentInjuryCertificates.ReasonAdmission));
                cmd.AddParameter("@ClinicalSigns", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurrentInjuryCertificates.ClinicalSigns));
                cmd.AddParameter("@Diagnosis", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurrentInjuryCertificates.Diagnosis));
                cmd.AddParameter("@AdmissionStatus", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurrentInjuryCertificates.AdmissionStatus));
                cmd.AddParameter("@DischargeStatus", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurrentInjuryCertificates.DischargeStatus));
                cmd.AddParameter("@Treatment", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurrentInjuryCertificates.Treatment));
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentInjuryCertificates.V_RegistrationType));
                cmd.AddParameter("@IsDelete", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentInjuryCertificates.IsDelete));
                cmd.AddParameter("@UserID", SqlDbType.VarChar, ConvertNullObjectToDBNull(UserID));
                cmd.AddParameter("@Date", SqlDbType.VarChar, ConvertNullObjectToDBNull(Date));

                cn.Open();
                InjuryCertificates retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    retVal = GetInjuryCertificatesFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public bool SaveBirthCertificates(BirthCertificates CurrentBirthCertificates, long UserID, string Date)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSaveBirthCertificates", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@BirthCertificateID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentBirthCertificates.BirthCertificateID));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentBirthCertificates.PtRegistrationID));
                cmd.AddParameter("@PtRegistrationID_Child", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentBirthCertificates.PtRegistrationID_Child));
                cmd.AddParameter("@BirthCertificateCode", SqlDbType.Int, ConvertNullObjectToDBNull(CurrentBirthCertificates.BirthCertificateCode));
                cmd.AddParameter("@NumOfChild", SqlDbType.TinyInt, ConvertNullObjectToDBNull(CurrentBirthCertificates.NumOfChild));
                cmd.AddParameter("@GenderOfChild", SqlDbType.Char, ConvertNullObjectToDBNull(CurrentBirthCertificates.GenderOfChild));
                cmd.AddParameter("@WeightOfChild", SqlDbType.SmallInt, ConvertNullObjectToDBNull(CurrentBirthCertificates.WeightOfChild));
                cmd.AddParameter("@PlanningName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurrentBirthCertificates.PlanningName));
                cmd.AddParameter("@Note", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurrentBirthCertificates.Note));
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentBirthCertificates.V_RegistrationType));
                cmd.AddParameter("@IsDelete", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentBirthCertificates.IsDelete));
                cmd.AddParameter("@UserID", SqlDbType.VarChar, ConvertNullObjectToDBNull(UserID));
                cmd.AddParameter("@Date", SqlDbType.VarChar, ConvertNullObjectToDBNull(Date));
                cmd.AddParameter("@BirthDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(CurrentBirthCertificates.BirthDate));
                cmd.AddParameter("@V_SurgicalBirth", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentBirthCertificates.V_SurgicalBirth));
                cmd.AddParameter("@V_BirthUnder32", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentBirthCertificates.V_BirthUnder32));
                cmd.AddParameter("@ChildStatus", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurrentBirthCertificates.ChildStatus));
                cmd.AddParameter("@BirthCount", SqlDbType.TinyInt, ConvertNullObjectToDBNull(CurrentBirthCertificates.BirthCount));
                cmd.AddParameter("@ChildCount", SqlDbType.TinyInt, ConvertNullObjectToDBNull(CurrentBirthCertificates.ChildCount));
                cn.Open();
                int retVal=ExecuteNonQuery(cmd);
                //IDataReader reader = ExecuteReader(cmd);
                //if (reader.Read())
                //{
                //    retVal = GetBirthCertificatesFromReader(reader);
                //}
                //reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal>0;
            }
        }
        public VacationInsuranceCertificates SaveVacationInsuranceCertificates(VacationInsuranceCertificates CurrentVacationInsuranceCertificates, bool IsPrenatal, long UserID, string Date)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSaveVacationInsuranceCertificates", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@VacationInsuranceCertificateID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentVacationInsuranceCertificates.VacationInsuranceCertificateID));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentVacationInsuranceCertificates.PtRegistrationID));
                cmd.AddParameter("@VacationInsuranceCertificateCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(CurrentVacationInsuranceCertificates.VacationInsuranceCertificateCode));
                cmd.AddParameter("@SeriNumber", SqlDbType.Int, ConvertNullObjectToDBNull(CurrentVacationInsuranceCertificates.SeriNumber));
                cmd.AddParameter("@Diagnosis", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurrentVacationInsuranceCertificates.Diagnosis));
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(CurrentVacationInsuranceCertificates.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(CurrentVacationInsuranceCertificates.ToDate));
                cmd.AddParameter("@FatherName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurrentVacationInsuranceCertificates.FatherName));
                cmd.AddParameter("@MotherName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurrentVacationInsuranceCertificates.MotherName));
                cmd.AddParameter("@IsDelete", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentVacationInsuranceCertificates.IsDelete));
                cmd.AddParameter("@UserID", SqlDbType.VarChar, ConvertNullObjectToDBNull(UserID));
                cmd.AddParameter("@Date", SqlDbType.VarChar, ConvertNullObjectToDBNull(Date));
                cmd.AddParameter("@IsInsurance", SqlDbType.Bit, ConvertNullObjectToDBNull(CurrentVacationInsuranceCertificates.IsInsurance));
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentVacationInsuranceCertificates.V_RegistrationType));
                cmd.AddParameter("@IsPrenatal", SqlDbType.BigInt, ConvertNullObjectToDBNull(IsPrenatal));
                cmd.AddParameter("@SeriNumberText", SqlDbType.VarChar, ConvertNullObjectToDBNull(CurrentVacationInsuranceCertificates.SeriNumberText));
                cmd.AddParameter("@DocumentNumber", SqlDbType.VarChar, ConvertNullObjectToDBNull(CurrentVacationInsuranceCertificates.DocumentNumber));
                cmd.AddParameter("@MedicalNumber", SqlDbType.VarChar, ConvertNullObjectToDBNull(CurrentVacationInsuranceCertificates.MedicalNumber));
                cmd.AddParameter("@CheifDoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentVacationInsuranceCertificates.CheifDoctorStaffID));
                cmd.AddParameter("@IsSuspendedPregnant", SqlDbType.Bit, ConvertNullObjectToDBNull(CurrentVacationInsuranceCertificates.IsSuspendedPregnant));
                cmd.AddParameter("@ReasonSuspendedPregnant", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurrentVacationInsuranceCertificates.ReasonSuspendedPregnant));
                cmd.AddParameter("@NumOfDayLeave", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentVacationInsuranceCertificates.NumOfDayLeave));
                cmd.AddParameter("@TreatmentMethod", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurrentVacationInsuranceCertificates.TreatmentMethod));
                cmd.AddParameter("@ChildUnder7", SqlDbType.Bit, ConvertNullObjectToDBNull(CurrentVacationInsuranceCertificates.ChildUnder7));
                cmd.AddParameter("@IsTuberculosis", SqlDbType.Bit, ConvertNullObjectToDBNull(CurrentVacationInsuranceCertificates.IsTuberculosis));
                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentVacationInsuranceCertificates.DoctorStaffID));
                cmd.AddParameter("@GestationalAge", SqlDbType.Int, ConvertNullObjectToDBNull(CurrentVacationInsuranceCertificates.GestationalAge));
                cn.Open();
                VacationInsuranceCertificates retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    retVal = GetVacationInsuranceCertificatesFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public string GetSummaryPCLResultByPtRegistrationID(long PtRegistrationID, long V_RegistrationType)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetSummaryPCLResultByPtRegistrationID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));

                cn.Open();
                string retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    if (reader.HasColumn("Result") && reader["Result"] != DBNull.Value)
                    {
                        retVal = reader["Result"].ToString();
                    }
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public string GetPCLResultForInjuryCertificatesByPtRegistrationID(long PtRegistrationID, out string PCLResultFromAdmissionExamination)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetPCLResultForInjuryCertificatesByPtRegistrationID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@OutResult", SqlDbType.NVarChar, 500, ParameterDirection.Output);

                cn.Open();
                string retVal = null;

                var reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    if (reader.HasColumn("Result") && reader["Result"] != DBNull.Value)
                    {
                        retVal = reader["Result"].ToString();
                    }
                }
                reader.Close();
                PCLResultFromAdmissionExamination = cmd.Parameters["@OutResult"].Value.ToString();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        //▼===== #020
        public List<PatientRegistration> SearchRegistrationListForCheckMedicalFiles(CheckMedicalFilesSearchCriteria SearchCriteria)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSearchRegistrationListForCheckMedicalFiles", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.DeptID));
                cmd.AddParameter("@V_CheckMedicalFilesStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_CheckMedicalFilesStatus));
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(SearchCriteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(SearchCriteria.ToDate));
                cmd.AddParameter("@NotDischarge", SqlDbType.Bit, ConvertNullObjectToDBNull(SearchCriteria.NotDischarge));
                cmd.AddParameter("@PatientCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria.PatientCode));
                cmd.AddParameter("@FullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria.FullName));
                
                cn.Open();
                List<PatientRegistration> retVal = null;

                IDataReader reader = ExecuteReader(cmd);

                retVal = GetPatientRegistrationCollectionFromReader(reader);

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public CheckMedicalFiles GetMedicalFilesByPtRegistrationID(long PtRegistrationID, long V_RegistrationType)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetMedicalFilesByPtRegistrationID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));
                
                cn.Open();
                CheckMedicalFiles retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    retVal = GetCheckMedicalFilesFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public bool SaveMedicalFiles(CheckMedicalFiles CheckMedicalFile, bool Is_KHTH, long V_RegistrationType, long StaffID, out long CheckMedicalFileIDNew)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSaveMedicalFiles", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@CheckMedicalFileID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CheckMedicalFile.CheckMedicalFileID));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CheckMedicalFile.PtRegistrationID));
                cmd.AddParameter("@Check_KHTH", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CheckMedicalFile.Check_KHTH));
                cmd.AddParameter("@Check_DLS", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CheckMedicalFile.Check_DLS));
                cmd.AddParameter("@Note_DLS", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CheckMedicalFile.Note_DLS));
                cmd.AddParameter("@V_CheckMedicalFilesStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(CheckMedicalFile.V_CheckMedicalFilesStatus));
                cmd.AddParameter("@Is_KHTH", SqlDbType.Bit, ConvertNullObjectToDBNull(Is_KHTH));
                cmd.AddParameter("@IsDLSChecked", SqlDbType.Bit, ConvertNullObjectToDBNull(CheckMedicalFile.IsDLSChecked));
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                cmd.AddParameter("@SendRequestStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CheckMedicalFile.SendRequestStaffID));
                cmd.AddParameter("@DLSReject", SqlDbType.Bit, ConvertNullObjectToDBNull(CheckMedicalFile.DLSReject));
                cmd.AddParameter("@IsUnlocked", SqlDbType.Bit, ConvertNullObjectToDBNull(CheckMedicalFile.IsUnlocked)); //#031
                cmd.AddParameter("@CheckMedicalFileIDNew", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                var retVal = ExecuteNonQuery(cmd);
                CheckMedicalFileIDNew = (long)cmd.Parameters["@CheckMedicalFileIDNew"].Value;
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }

        public List<CheckMedicalFiles> GetListCheckMedicalFilesByPtID(long PtRegistrationID, long V_RegistrationType)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetListCheckMedicalFilesByPtID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));

                cn.Open();
                List<CheckMedicalFiles> retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                retVal = GetCheckMedicalFilesCollectionFromReader(reader);                
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        //▲===== #020
        public bool SaveNutritionalRating(long PtRegistrationID, NutritionalRating curNutritionalRating, long StaffID, DateTime Date, out long NutritionalRatingIDNew)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSaveNutritionalRating", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@NutritionalRatingID", SqlDbType.BigInt, ConvertNullObjectToDBNull(curNutritionalRating.NutritionalRatingID));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@ROM_BMI", SqlDbType.Bit, curNutritionalRating.ROM_BMI);
                cmd.AddParameter("@ROM_WeightLoss", SqlDbType.Bit, curNutritionalRating.ROM_WeightLoss);
                cmd.AddParameter("@ROM_ReduceEat", SqlDbType.Bit, curNutritionalRating.ROM_ReduceEat);
                cmd.AddParameter("@ROM_SevereIllness", SqlDbType.Bit, curNutritionalRating.ROM_SevereIllness);
                cmd.AddParameter("@RiskOfMalnutrition", SqlDbType.Bit, curNutritionalRating.RiskOfMalnutrition);
                cmd.AddParameter("@WeightLossHospitalStay", SqlDbType.Bit, curNutritionalRating.WeightLossHospitalStay);
                cmd.AddParameter("@WL_Weight", SqlDbType.TinyInt, ConvertNullObjectToDBNull(curNutritionalRating.WL_Weight));
                cmd.AddParameter("@WL_Month", SqlDbType.TinyInt, ConvertNullObjectToDBNull(curNutritionalRating.WL_Month));
                cmd.AddParameter("@WL_Percent", SqlDbType.TinyInt, ConvertNullObjectToDBNull(curNutritionalRating.WL_Percent));
                cmd.AddParameter("@V_EatingType", SqlDbType.BigInt, ConvertNullObjectToDBNull(curNutritionalRating.V_EatingType));
                cmd.AddParameter("@AtrophySubcutaneousFatLayer", SqlDbType.BigInt, ConvertNullObjectToDBNull(curNutritionalRating.AtrophySubcutaneousFatLayer));
                cmd.AddParameter("@AmyotrophicLateralSclerosis", SqlDbType.BigInt, ConvertNullObjectToDBNull(curNutritionalRating.AmyotrophicLateralSclerosis));
                cmd.AddParameter("@PeripheralEdema", SqlDbType.BigInt, ConvertNullObjectToDBNull(curNutritionalRating.PeripheralEdema));
                cmd.AddParameter("@BellyFlap", SqlDbType.BigInt, ConvertNullObjectToDBNull(curNutritionalRating.BellyFlap));
                cmd.AddParameter("@V_SGAType", SqlDbType.BigInt, ConvertNullObjectToDBNull(curNutritionalRating.V_SGAType));
                cmd.AddParameter("@V_NutritionalRequire", SqlDbType.BigInt, ConvertNullObjectToDBNull(curNutritionalRating.V_NutritionalRequire));
                cmd.AddParameter("@ONT_Kcal", SqlDbType.Int, ConvertNullObjectToDBNull(curNutritionalRating.ONT_Kcal));
                cmd.AddParameter("@ONT_Protein", SqlDbType.Int, ConvertNullObjectToDBNull(curNutritionalRating.ONT_Protein));
                cmd.AddParameter("@ONT_Fat", SqlDbType.Int, ConvertNullObjectToDBNull(curNutritionalRating.ONT_Fat));
                cmd.AddParameter("@ONT_Other", SqlDbType.NVarChar, ConvertNullObjectToDBNull(curNutritionalRating.ONT_Other));
                cmd.AddParameter("@V_NutritionalMethods", SqlDbType.BigInt, ConvertNullObjectToDBNull(curNutritionalRating.V_NutritionalMethods));
                cmd.AddParameter("@ConsultationNutritional", SqlDbType.Bit, ConvertNullObjectToDBNull(curNutritionalRating.ConsultationNutritional));
                cmd.AddParameter("@Staff", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                cmd.AddParameter("@Date", SqlDbType.DateTime, ConvertNullObjectToDBNull(Date));
                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(curNutritionalRating.DeptID));

                cmd.AddParameter("@NutritionalRatingIDNew", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                var retVal = ExecuteNonQuery(cmd);
                NutritionalRatingIDNew = (long)cmd.Parameters["@NutritionalRatingIDNew"].Value;
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }
        public List<NutritionalRating> GetNutritionalRatingByPtRegistrationID(long PtRegistrationID)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetNutritionalRatingByPtRegistrationID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                //cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));

                cn.Open();
                List<NutritionalRating> retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                retVal = GetNutritionalRatingCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public bool DeleteNutritionalRating(long NutritionalRatingID, long StaffID, DateTime Date)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeleteNutritionalRating", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@NutritionalRatingID", SqlDbType.BigInt, ConvertNullObjectToDBNull(NutritionalRatingID));
                cmd.AddParameter("@Staff", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                cmd.AddParameter("@Date", SqlDbType.DateTime, ConvertNullObjectToDBNull(Date));

                cn.Open();
                var retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }
        public bool CheckBeforeChangeDept(long registrationID, long DeptID
                                            , out string errorMessages
                                            , out string confirmMessages)
        {
            try
            {
                errorMessages = "";
                confirmMessages = "";
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spCheckBeforeChangeDept", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, registrationID);
                    cmd.AddParameter("@DeptID", SqlDbType.BigInt, DeptID);
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
                            errorMessages += reader["Message"].ToString() + Environment.NewLine;
                        }
                    }
                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //▼===== #011
        public Prescription CheckOldConsultationPatient(long PatientID)
        {
            try
            {

                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spCheckOldConsultationPatient", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PatientID", SqlDbType.BigInt, PatientID);
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    Prescription readPrescription = GetPtPrescriptionItemFromReader(reader);
                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                    return readPrescription;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //▲===== #011
        //▼===== #012
        public List<Patient> AddListPatient(List<APIPatient> ListPatient, string ContractNo)
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                var cmd = new SqlCommand("spInsertListPatient", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@ListPatient", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertDetailsListPatientToXml(ListPatient)));
                cmd.AddParameter("@ContractNo", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ContractNo));
                cmd.CommandTimeout = int.MaxValue;
                cn.Open();
                var reader = ExecuteReader(cmd);
                var patients = GetPatientCollectionFromReader(reader);
                CleanUpConnectionAndCommand(cn, cmd);
                return patients;
            }
        }
        //▲===== #012
        //▼===== #013
        public RefDepartment LoadDeptAdmissionRequest(long PtRegistrationID)
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                var cmd = new SqlCommand("spLoadDeptAdmissionRequest", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cn.Open();
                var reader = ExecuteReader(cmd);
                RefDepartment department = new RefDepartment();
                if (reader.Read())
                {
                    department = GetDepartmentFromReader(reader);
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return department;
            }
        }
        //▲===== #014
        public ObservableCollection<DiagnosysConsultationSummary> GetDiagnosysConsultationSummary(long PtRegistrationID, int PatientFindBy, long PatientID, out DiagnosysConsultationSummary DiagnosysConsultation)
        {
            DiagnosysConsultation = new DiagnosysConsultationSummary();
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                var cmd = new SqlCommand("spGetDiagnosysConsultationSummary", cn) { CommandType = CommandType.StoredProcedure };
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@PatientFindBy", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientFindBy));
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                cn.Open();
                var reader = ExecuteReader(cmd);
                ObservableCollection<DiagnosysConsultationSummary> objDiagConsulCollection = new ObservableCollection<DiagnosysConsultationSummary>();
                DiagnosysConsultationSummary objDiagConsul = new DiagnosysConsultationSummary();

                objDiagConsul.StaffList = new List<Staff>();
                objDiagConsul.ICD10List = new List<DiagnosisIcd10Items>();

                objDiagConsulCollection = GetDiagnosysConsultationSummaryCollection(reader).ToObservableCollection();

                if (reader.NextResult())
                {
                    if (reader.Read())
                    {
                        DiagnosysConsultation = GetDiagnosysConsultationSummaryFromReader(reader);
                    }
                }
                if (reader.NextResult())
                {
                    DiagnosysConsultation.StaffList = GetStaffCollectionFromReader(reader);
                }
                if (reader.NextResult())
                {
                    DiagnosysConsultation.ICD10List = GetDiagnosisIcd10ItemsCollectionFromReader(reader);
                }

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objDiagConsulCollection;
            }
        }

        public IList<InfectionVirus> GetAllInfectionVirus()
        {
            using (var mConnection = new SqlConnection(this.ConnectionString))
            {
                var mCommand = new SqlCommand("spGetAllInfectionVirus", mConnection) { CommandType = CommandType.StoredProcedure };
                mConnection.Open();
                var reader = ExecuteReader(mCommand);
                List<InfectionVirus> mCollection = new List<InfectionVirus>();
                while (reader.Read())
                {
                    InfectionVirus mItem = new InfectionVirus();
                    mItem.FillData(reader);
                    mCollection.Add(mItem);
                }
                reader.Close();
                CleanUpConnectionAndCommand(mConnection, mCommand);
                return mCollection;
            }
        }
        public List<InfectionCase> GetInfectionCaseInfo(long? PtRegistrationID, long? PatientID)
        {
            using (var mConnection = new SqlConnection(this.ConnectionString))
            {
                var mCommand = new SqlCommand("spGetInfectionCaseByPtRegID", mConnection) { CommandType = CommandType.StoredProcedure };
                mCommand.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                mCommand.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                mConnection.Open();
                var reader = ExecuteReader(mCommand);
                List<InfectionCase> mCollection = new List<InfectionCase>();
                while (reader.Read())
                {
                    InfectionCase mItem = new InfectionCase();
                    mItem.FillData(reader);
                    mCollection.Add(mItem);
                }
                reader.Close();
                CleanUpConnectionAndCommand(mConnection, mCommand);
                return mCollection;
            }
        }
        public InfectionCase GetInfectionCaseDetail(InfectionCase aInfectionCase, bool IsGetAllDetails = false)
        {
            using (var mConnection = new SqlConnection(this.ConnectionString))
            {
                var mCommand = new SqlCommand("spGetInfectionCaseDetail", mConnection) { CommandType = CommandType.StoredProcedure };
                if (IsGetAllDetails)
                {
                    mCommand.CommandText = "spGetInfectionCaseAllDetails";
                }
                mCommand.AddParameter("@InfectionCaseID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aInfectionCase.InfectionCaseID));
                mConnection.Open();
                var reader = ExecuteReader(mCommand);
                List<InfectionICD10Item> ICD10Collection = new List<InfectionICD10Item>();
                while (reader.Read())
                {
                    InfectionICD10Item mItem = new InfectionICD10Item();
                    mItem.FillData(reader);
                    ICD10Collection.Add(mItem);
                }
                aInfectionCase.InfectionICD10ListID1Items = ICD10Collection.ToObservableCollection();
                ICD10Collection = new List<InfectionICD10Item>();
                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        InfectionICD10Item mItem = new InfectionICD10Item();
                        mItem.FillData(reader);
                        ICD10Collection.Add(mItem);
                    }
                }
                aInfectionCase.InfectionICD10ListID2Items = ICD10Collection.ToObservableCollection();
                if (reader.NextResult())
                {
                    aInfectionCase.AntibioticTreatmentCollection = new ObservableCollection<AntibioticTreatment>();
                    while (reader.Read())
                    {
                        AntibioticTreatment mItem = new AntibioticTreatment();
                        mItem.FillData(reader);
                        aInfectionCase.AntibioticTreatmentCollection.Add(mItem);
                    }
                }
                if (reader.NextResult())
                {
                    List<AntibioticTreatmentMedProductDetail> mCollection = new List<AntibioticTreatmentMedProductDetail>();
                    while (reader.Read())
                    {
                        AntibioticTreatmentMedProductDetail mItem = new AntibioticTreatmentMedProductDetail();
                        mItem.FillData(reader);
                        mCollection.Add(mItem);
                    }
                    reader.Close();
                    if (aInfectionCase != null && mCollection != null && mCollection.Count > 0)
                    {
                        foreach (var aItem in aInfectionCase.AntibioticTreatmentCollection)
                        {
                            if (!mCollection.Any(x => x.AntibioticTreatmentID == aItem.AntibioticTreatmentID))
                            {
                                continue;
                            }
                            aItem.AntibioticTreatmentMedProductDetailCollection = mCollection.Where(x => x.AntibioticTreatmentID == aItem.AntibioticTreatmentID).ToObservableCollection();
                        }
                    }
                }
                reader.Close();
                CleanUpConnectionAndCommand(mConnection, mCommand);
                return aInfectionCase;
            }
        }
        public long EditInfectionCase(InfectionCase aInfectionCase)
        {
            using (var mConnection = new SqlConnection(this.ConnectionString))
            {
                var mCommand = new SqlCommand("spEditInfectionCase", mConnection) { CommandType = CommandType.StoredProcedure };
                mCommand.AddParameter("@InfectionCaseXml", SqlDbType.Xml, aInfectionCase.ConvertToXml());
                mCommand.AddParameter("@OutInfectionCaseID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                long OutInfectionCaseID = 0;
                mConnection.Open();
                ExecuteNonQuery(mCommand);
                if (mCommand.Parameters["@OutInfectionCaseID"].Value != null && mCommand.Parameters["@OutInfectionCaseID"].Value != DBNull.Value)
                {
                    OutInfectionCaseID = (long)mCommand.Parameters["@OutInfectionCaseID"].Value;
                }
                CleanUpConnectionAndCommand(mConnection, mCommand);
                return OutInfectionCaseID;
            }
        }
        public void EditAntibioticTreatment(AntibioticTreatment aAntibioticTreatment)
        {
            using (var mConnection = new SqlConnection(this.ConnectionString))
            {
                var mCommand = new SqlCommand("spEditAntibioticTreatment", mConnection) { CommandType = CommandType.StoredProcedure };
                mCommand.AddParameter("@AntibioticTreatmentXml", SqlDbType.Xml, aAntibioticTreatment.ConvertToXml());
                mConnection.Open();
                ExecuteNonQuery(mCommand);
                CleanUpConnectionAndCommand(mConnection, mCommand);
            }
        }
        public IList<AntibioticTreatmentMedProductDetail> GetAntibioticTreatmentMedProductDetails(long AntibioticTreatmentID, long V_AntibioticTreatmentType)
        {
            using (var mConnection = new SqlConnection(this.ConnectionString))
            {
                var mCommand = new SqlCommand("spGetAntibioticTreatmentMedProductDetails", mConnection) { CommandType = CommandType.StoredProcedure };
                mCommand.AddParameter("@AntibioticTreatmentID", SqlDbType.BigInt, ConvertNullObjectToDBNull(AntibioticTreatmentID));
                mCommand.AddParameter("@V_AntibioticTreatmentType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_AntibioticTreatmentType));
                mConnection.Open();
                var reader = ExecuteReader(mCommand);
                List<AntibioticTreatmentMedProductDetail> mCollection = new List<AntibioticTreatmentMedProductDetail>();
                while (reader.Read())
                {
                    AntibioticTreatmentMedProductDetail mItem = new AntibioticTreatmentMedProductDetail();
                    mItem.FillData(reader);
                    mCollection.Add(mItem);
                }
                reader.Close();
                CleanUpConnectionAndCommand(mConnection, mCommand);
                return mCollection;
            }
        }
        public IList<OutwardDrugClinicDept> GetDrugsInUseOfAntibioticTreatment(AntibioticTreatment aAntibioticTreatment, long DeptID, long PtRegistrationID)
        {
            using (var mConnection = new SqlConnection(this.ConnectionString))
            {
                var mCommand = new SqlCommand("spGetDrugsInUseOfAntibioticTreatment", mConnection) { CommandType = CommandType.StoredProcedure };
                mCommand.AddParameter("@StartDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aAntibioticTreatment.StartDate));
                mCommand.AddParameter("@EndDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aAntibioticTreatment.EndDate));
                mCommand.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptID));
                mCommand.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                mCommand.AddParameter("@AntibioticTreatmentXML", SqlDbType.Xml, ConvertNullObjectToDBNull(aAntibioticTreatment.ConvertToXml()));
                mConnection.Open();
                var reader = ExecuteReader(mCommand);
                List<OutwardDrugClinicDept> mCollection = GetOutwardDrugClinicDeptCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(mConnection, mCommand);
                return mCollection;
            }
        }
        public IList<InfectionCase> GetInfectionCaseAllContentInfo(DateTime StartDate, DateTime EndDate, long V_InfectionCaseStatus, string DrugName)
        {
            using (var mConnection = new SqlConnection(this.ConnectionString))
            {
                var mCommand = new SqlCommand("spGetInfectionCaseAllContentInfo", mConnection) { CommandType = CommandType.StoredProcedure };
                mCommand.AddParameter("@StartDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(StartDate));
                mCommand.AddParameter("@EndDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(EndDate));
                mCommand.AddParameter("@V_InfectionCaseStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_InfectionCaseStatus));
                mCommand.AddParameter("@DrugName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(DrugName));
                mConnection.Open();
                var reader = ExecuteReader(mCommand);
                List<InfectionCase> mCollection = new List<InfectionCase>();
                while (reader.Read())
                {
                    InfectionCase mItem = new InfectionCase();
                    mItem.FillData(reader);
                    mCollection.Add(mItem);
                }
                if (reader.NextResult())
                {
                    List<InfectionICD10Item> ICD10Collection = new List<InfectionICD10Item>();
                    while (reader.Read())
                    {
                        InfectionICD10Item mItem = new InfectionICD10Item();
                        mItem.FillData(reader);
                        ICD10Collection.Add(mItem);
                    }
                    foreach (var aInfectionCase in mCollection)
                    {
                        if (!ICD10Collection.Any(x => x.InfectionICD10ID == aInfectionCase.InfectionICD10ListID1))
                        {
                            continue;
                        }
                        aInfectionCase.InfectionICD10ListID1Items = ICD10Collection.Where(x => x.InfectionICD10ID == aInfectionCase.InfectionICD10ListID1).ToObservableCollection();
                    }
                }
                if (reader.NextResult())
                {
                    List<InfectionICD10Item> ICD10Collection = new List<InfectionICD10Item>();
                    while (reader.Read())
                    {
                        InfectionICD10Item mItem = new InfectionICD10Item();
                        mItem.FillData(reader);
                        ICD10Collection.Add(mItem);
                    }
                    foreach (var aInfectionCase in mCollection)
                    {
                        if (!ICD10Collection.Any(x => x.InfectionICD10ID == aInfectionCase.InfectionICD10ListID2))
                        {
                            continue;
                        }
                        aInfectionCase.InfectionICD10ListID2Items = ICD10Collection.Where(x => x.InfectionICD10ID == aInfectionCase.InfectionICD10ListID2).ToObservableCollection();
                    }
                }
                if (reader.NextResult())
                {
                    List<AntibioticTreatment> AntibioticTreatmentCollection = new List<AntibioticTreatment>();
                    while (reader.Read())
                    {
                        AntibioticTreatment mItem = new AntibioticTreatment();
                        mItem.FillData(reader);
                        AntibioticTreatmentCollection.Add(mItem);
                    }
                    foreach (var aInfectionCase in mCollection)
                    {
                        if (!AntibioticTreatmentCollection.Any(x => x.InfectionCaseID == aInfectionCase.InfectionCaseID))
                        {
                            continue;
                        }
                        aInfectionCase.AntibioticTreatmentCollection = AntibioticTreatmentCollection.Where(x => x.InfectionCaseID == aInfectionCase.InfectionCaseID).ToObservableCollection();
                    }
                }
                if (reader.NextResult())
                {
                    List<AntibioticTreatmentMedProductDetail> AntibioticTreatmentMedProductDetail = new List<AntibioticTreatmentMedProductDetail>();
                    while (reader.Read())
                    {
                        AntibioticTreatmentMedProductDetail mItem = new AntibioticTreatmentMedProductDetail();
                        mItem.FillData(reader);
                        AntibioticTreatmentMedProductDetail.Add(mItem);
                    }
                    foreach (var aInfectionCase in mCollection)
                    {
                        if (aInfectionCase.AntibioticTreatmentCollection == null)
                        {
                            continue;
                        }
                        foreach(var aAntibioticTreatment in aInfectionCase.AntibioticTreatmentCollection)
                        {
                            if (!AntibioticTreatmentMedProductDetail.Any(x => x.AntibioticTreatmentID == aAntibioticTreatment.AntibioticTreatmentID))
                            {
                                continue;
                            }
                            aAntibioticTreatment.AntibioticTreatmentMedProductDetailCollection = AntibioticTreatmentMedProductDetail.Where(x => x.AntibioticTreatmentID == aAntibioticTreatment.AntibioticTreatmentID).ToObservableCollection();
                        }
                    }
                }
                reader.Close();
                CleanUpConnectionAndCommand(mConnection, mCommand);
                return mCollection;
            }
        }
        public IList<AntibioticTreatmentMedProductDetailSummaryContent> GetAllContentInfoOfInfectionCaseCollection(DateTime StartDate, DateTime EndDate, long V_InfectionCaseStatus, string DrugName)
        {
            using (var mConnection = new SqlConnection(this.ConnectionString))
            {
                var mCommand = new SqlCommand("spGetAllContentInfoOfInfectionCaseCollection", mConnection) { CommandType = CommandType.StoredProcedure };
                mCommand.AddParameter("@StartDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(StartDate));
                mCommand.AddParameter("@EndDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(EndDate));
                mCommand.AddParameter("@V_InfectionCaseStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_InfectionCaseStatus));
                mCommand.AddParameter("@DrugName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(DrugName));
                mConnection.Open();
                var reader = ExecuteReader(mCommand);
                List<AntibioticTreatmentMedProductDetailSummaryContent> mCollection = new List<AntibioticTreatmentMedProductDetailSummaryContent>();
                while (reader.Read())
                {
                    InfectionCase aInfectionCase = new InfectionCase();
                    aInfectionCase.FillData(reader);
                    AntibioticTreatment aAntibioticTreatment = new AntibioticTreatment();
                    aAntibioticTreatment.FillData(reader);
                    AntibioticTreatmentMedProductDetail aAntibioticTreatmentMedProductDetail = new AntibioticTreatmentMedProductDetail();
                    aAntibioticTreatmentMedProductDetail.FillData(reader);
                    AntibioticTreatmentMedProductDetailSummaryContent CurrentAntibioticTreatmentMedProductDetailSummaryContent = new AntibioticTreatmentMedProductDetailSummaryContent();
                    PropertyCopierHelper.CopyPropertiesTo(aAntibioticTreatmentMedProductDetail, CurrentAntibioticTreatmentMedProductDetailSummaryContent);
                    CurrentAntibioticTreatmentMedProductDetailSummaryContent.CurrentAntibioticTreatment = aAntibioticTreatment;
                    CurrentAntibioticTreatmentMedProductDetailSummaryContent.CurrentInfectionCase = aInfectionCase;
                    mCollection.Add(CurrentAntibioticTreatmentMedProductDetailSummaryContent);
                }
                reader.Close();
                CleanUpConnectionAndCommand(mConnection, mCommand);
                return mCollection;
            }
        }
        public IList<AntibioticTreatment> GetAntibioticTreatmentsByPtRegID(long PtRegistrationID)
        {
            using (var mConnection = new SqlConnection(this.ConnectionString))
            {
                var mCommand = new SqlCommand("spGetAntibioticTreatmentsByPtRegID", mConnection) { CommandType = CommandType.StoredProcedure };
                mCommand.AddParameter("@PtRegistrationID", SqlDbType.BigInt, PtRegistrationID);
                mConnection.Open();
                var reader = ExecuteReader(mCommand);
                List<AntibioticTreatment> mCollection = new List<AntibioticTreatment>();
                while (reader.Read())
                {
                    AntibioticTreatment mItem = new AntibioticTreatment();
                    mItem.FillData(reader);
                    mCollection.Add(mItem);
                }
                if (reader.NextResult())
                {
                    List<AntibioticTreatmentMedProductDetail> ChildCollection = new List<AntibioticTreatmentMedProductDetail>();
                    while (reader.Read())
                    {
                        AntibioticTreatmentMedProductDetail mItem = new AntibioticTreatmentMedProductDetail();
                        mItem.FillData(reader);
                        ChildCollection.Add(mItem);
                    }
                    if (ChildCollection != null && ChildCollection.Count > 0)
                    {
                        foreach (var aItem in mCollection)
                        {
                            if (!ChildCollection.Any(x => x.AntibioticTreatmentID == aItem.AntibioticTreatmentID))
                            {
                                continue;
                            }
                            aItem.AntibioticTreatmentMedProductDetailCollection = ChildCollection.Where(x => x.AntibioticTreatmentID == aItem.AntibioticTreatmentID).ToObservableCollection();
                        }
                    }
                }
                reader.Close();
                CleanUpConnectionAndCommand(mConnection, mCommand);
                return mCollection;
            }
        }
        public void InsertAntibioticTreatmentIntoInstruction(long AntibioticTreatmentID, long InfectionCaseID)
        {
            using (var mConnection = new SqlConnection(this.ConnectionString))
            {
                var mCommand = new SqlCommand("spInsertAntibioticTreatmentIntoInstruction", mConnection) { CommandType = CommandType.StoredProcedure };
                mCommand.AddParameter("@AntibioticTreatmentID", SqlDbType.BigInt, AntibioticTreatmentID);
                mCommand.AddParameter("@InfectionCaseID", SqlDbType.BigInt, InfectionCaseID);
                mConnection.Open();
                ExecuteNonQuery(mCommand);
                CleanUpConnectionAndCommand(mConnection, mCommand);
            }
        }
        public bool CheckValid15PercentHIBenefitCase(long PtRegistrationID, long V_RegistrationType, out bool IsNeedRecal)
        {
            using (var CurrentConnection = new SqlConnection(this.ConnectionString))
            {
                var CurrentCommand = new SqlCommand("spCheckValid15PercentHIBenefitCase", CurrentConnection) { CommandType = CommandType.StoredProcedure };
                CurrentCommand.AddParameter("@PtRegistrationID", SqlDbType.BigInt, PtRegistrationID);
                CurrentCommand.AddParameter("@V_RegistrationType", SqlDbType.BigInt, V_RegistrationType);
                CurrentCommand.AddParameter("@IsValid", SqlDbType.Bit, DBNull.Value, ParameterDirection.Output);
                CurrentCommand.AddParameter("@IsNeedRecal", SqlDbType.Bit, DBNull.Value, ParameterDirection.Output);
                CurrentConnection.Open();
                ExecuteNonQuery(CurrentCommand);
                if (CurrentCommand.Parameters.Cast<SqlParameter>().Any(x => x.ParameterName.Equals("@IsNeedRecal") && x.Value != DBNull.Value && Convert.ToBoolean(x.Value)))
                {
                    IsNeedRecal = true;
                }
                else
                {
                    IsNeedRecal = false;
                }
                if (CurrentCommand.Parameters.Cast<SqlParameter>().Any(x => x.ParameterName.Equals("@IsValid") && x.Value != DBNull.Value && Convert.ToBoolean(x.Value)))
                {
                    CleanUpConnectionAndCommand(CurrentConnection, CurrentCommand);
                    return true;
                }
                CleanUpConnectionAndCommand(CurrentConnection, CurrentCommand);
                return false;
            }
        }

        public List<PatientRegistration> GetAllRegistrationForSettlement(long PatientID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllRegistrationForSettlement", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientID", SqlDbType.NVarChar, ConvertNullObjectToDBNull(PatientID));
                cn.Open();
                List<PatientRegistration> retVal = null;

                IDataReader reader = ExecuteReader(cmd);

                retVal = GetPatientRegistrationCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public void UpdateAdmissionDate(long PtRegistrationID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUpdateAdmissionDate", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cn.Open();
                ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        public void AddRegistrationDetailList_InPt(PatientRegistration CurrentRegistration)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_AddRegistrationDetailList_InPt", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentRegistration.InPtRegistrationID));
                //▼====: #020
                if (Globals.AxServerSettings.CommonItems.AllowFirstHIExaminationWithoutPay)
                {
                    cmd.AddParameter("@RegistrationDetails_Add", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertPatientRegistrationDetailsToXml(CurrentRegistration.PatientRegistrationDetails.Where(item => (item.PaidTime == null && item.RefMedicalServiceItem != null &&  item.RefMedicalServiceItem.HITTypeID == 8) || (item.PaidTime != null && item.RefMedicalServiceItem != null && item.RefMedicalServiceItem.HITTypeID != 8)
                    && (item.V_ExamRegStatus != (long)AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI && item.V_ExamRegStatus != (long)AllLookupValues.ExamRegStatus.XOA_TRA_TIEN_LAI))).ToString()));
                }
                else
                {
                    cmd.AddParameter("@RegistrationDetails_Add", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertPatientRegistrationDetailsToXml(CurrentRegistration.PatientRegistrationDetails.Where(item => item.PaidTime != null && (item.V_ExamRegStatus != (long)AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI && item.V_ExamRegStatus != (long)AllLookupValues.ExamRegStatus.XOA_TRA_TIEN_LAI))).ToString()));
                }
                //▲====: #020
                cn.Open();
                ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        public List<PatientPCLImagingResult> GetPatientPCLImagingResult_ByPtRegistrationID(long PtRegistrationID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetPatientPCLImagingResult_ByPtRegistrationID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cn.Open();
                List<PatientPCLImagingResult> retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                retVal = GetPatientPCLImagingResultColectionsFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public List<PatientPCLLaboratoryResultDetail> GetPatientPCLLaboratoryResultDetail_ByPtRegistrationID(long PtRegistrationID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetPatientPCLLaboratoryResultDetail_ByPtRegistrationID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cn.Open();
                List<PatientPCLLaboratoryResultDetail> retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                retVal = GetPtPCLLabExamTypesByReqIDColectionsFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        //▼===== #017
        public bool UpdatePCLRequestDetailList_ForRecalBilling(List<PatientPCLRequestDetail> requestDetailList, DbConnection connection, DbTransaction tran, bool IsNotCheckInvalid = false)
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
            cmd.CommandText = "spUpdatePCLRequestDetailList_ForRecalBiliing";
            cmd.CommandTimeout = int.MaxValue;


            string detailListString = ConvertPCLRequestDetailsToXml(requestDetailList).ToString();

            cmd.AddParameter("@PCLRequestDetails_Update", SqlDbType.Xml, ConvertNullObjectToDBNull(detailListString));
            cmd.AddParameter("@IsNotCheckInvalid", SqlDbType.Bit, ConvertNullObjectToDBNull(IsNotCheckInvalid));
            int retVal = ExecuteNonQuery(cmd);
            CleanUpConnectionAndCommand(null, cmd);
            return true;
        }
        //▲===== #014

        public List<PatientRegistration> SearchRegistration_ByServiceID(long MedServiceID, DateTime? FromDate, DateTime? ToDate, out List<DiagnosisTreatment> DiagnosisList)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSearchRegistration_ByServiceID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(MedServiceID));
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(ToDate));

                cn.Open();
                List<PatientRegistration> retVal = null;
                DiagnosisList = new List<DiagnosisTreatment>();
                IDataReader reader = ExecuteReader(cmd);

                retVal = GetPatientRegistrationCollectionFromReader(reader);

                if (reader.NextResult())
                {
                    DiagnosisList = GetDiagTrmtCollectionFromReader(reader);
                }
                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public List<Resources> GetResourcesForMedicalServices()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetResourcesForMedicalServices", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                List<Resources> retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                retVal = GetResourcesForMedicalServicesListByTypeIDCollectionFromReader(reader);
                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public List<Resources> GetResourcesForMedicalServicesListByTypeIDCollectionFromReader(IDataReader reader)
        {
            List<Resources> p = new List<Resources>();
            while (reader.Read())
            {
                Resources item = new Resources();
                if (reader.HasColumn("HIRepResourceCode") && reader["HIRepResourceCode"] != DBNull.Value)
                {
                    item.HIRepResourceCode = reader["HIRepResourceCode"].ToString();
                }
                if (reader.HasColumn("ItemName") && reader["ItemName"] != DBNull.Value)
                {
                    item.ItemName = reader["ItemName"].ToString();
                }
                if (reader.HasColumn("RscrID") && reader["RscrID"] != DBNull.Value)
                {
                    item.RscrID = Convert.ToInt64(reader["RscrID"]);
                }
                p.Add(item);
            }
            return p;
        }
        public List<RefMedicalServiceItem> GetMedicalServiceItemByHIRepResourceCode(string HIRepResourceCode)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetMedicalServiceItemByHIRepResourceCode", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@HIRepResourceCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(HIRepResourceCode));

                cn.Open();
                List<RefMedicalServiceItem> retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                retVal = GetMedicalServiceItemCollectionFromReader(reader);
                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public bool SaveSmallProcedureAutomatic(ObservableCollection<SmallProcedure> SmallProcedureList)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSaveSmallProcedureAutomatic", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@SmallProcedureListXML", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertSmallProcedureToXml(SmallProcedureList)));
                cn.Open();
                var retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }

        public string ConvertSmallProcedureToXml(ObservableCollection<SmallProcedure> SmallProcedureList)
        {
            if (SmallProcedureList == null || SmallProcedureList.Count == 0)
            {
                return null;
            }
              XDocument  mXDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), new XElement("SmallProcedures",
                  from item in SmallProcedureList
                  select new XElement("SmallProcedure",
                    new XElement("ProcedureDateTime", item.ProcedureDateTime),
                    new XElement("CompletedDateTime", item.CompletedDateTime),
                    new XElement("Diagnosis", item.Diagnosis),
                    new XElement("ProcedureMethod", item.ProcedureMethod),
                    new XElement("DoctorStaffID", item.ProcedureDoctorStaff.StaffID),
                    new XElement("PtRegDetailID", item.PtRegDetailID),
                    new XElement("NurseStaffID", item.NurseStaff.StaffID),
                    new XElement("V_RegistrationType", item.V_RegistrationType),
                    new XElement("ICD10CodeBefore", item.BeforeICD10.ICD10Code),
                    new XElement("DiagnosisFinalBefore", item.BeforeICD10.DiagnosisFinal),
                    new XElement("ICD10CodeAfter", item.AfterICD10.ICD10Code),
                    new XElement("DiagnosisFinalAfter", item.AfterICD10.DiagnosisFinal),
                    new XElement("ServiceRecID", item.ServiceRecID),
                    new XElement("CreatedStaffID", item.CreatedStaffID),
                    new XElement("HIRepResourceCode", item.HIRepResourceCode)
                    )));

            return mXDocument.ToString();
        }

        //▼===== #018
        public bool ConfirmEmergencyOutPtByPtRegistrionID(long PtRegistrationID, long StaffID, long V_RegistrationType)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spConfirmEmergencyOutPtByPtRegistrionID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));
                cn.Open();
                var retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }
        //▲===== #018
        public bool ConfirmSpecialRegistrationByPtRegistrationID(PatientRegistration info, long StaffID)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spConfirmSpecialRegistrationByPtRegistrationID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.PtRegistrationID));
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(info.PatientID));
                cmd.AddParameter("@ExamDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(info.ExamDate));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                cn.Open();
                var retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }
        #region Báo giá
        public void SaveQuotation(InPatientBillingInvoice aBillingInvoice, out long OutQuotationID, string QuotationTitle, long? PatientID)
        {
            using (SqlConnection aConnection = new SqlConnection(this.ConnectionString))
            {
                aConnection.Open();
                SqlCommand aCommand;
                aCommand = (SqlCommand)aConnection.CreateCommand();
                aCommand.CommandText = "spSaveQuotation";
                aCommand.CommandType = CommandType.StoredProcedure;
                aCommand.AddParameter("@QuotationXml", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertBillingInvoicesToXml(new List<InPatientBillingInvoice> { aBillingInvoice }, 0).ToString()));
                aCommand.AddParameter("@OutQuotationID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                aCommand.AddParameter("@QuotationTitle", SqlDbType.NVarChar, ConvertNullObjectToDBNull(QuotationTitle));
                aCommand.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                ExecuteNonQuery(aCommand);
                OutQuotationID = (long)aCommand.Parameters["@OutQuotationID"].Value;
                CleanUpConnectionAndCommand(null, aCommand);
            }
        }
        public InPatientBillingInvoice GetQuotationAllDetail(long InPatientBillingInvID)
        {
            using (SqlConnection mConnection = new SqlConnection(this.ConnectionString))
            {
                if (mConnection.State != ConnectionState.Open)
                {
                    mConnection.Open();
                }
                InPatientBillingInvoice BillingInvoice = null;
                SqlCommand mCommand = (SqlCommand)mConnection.CreateCommand();
                mCommand.CommandText = "spGetQuotationAllDetail";
                mCommand.CommandType = CommandType.StoredProcedure;
                mCommand.AddParameter("@InPatientBillingInvID", SqlDbType.BigInt, InPatientBillingInvID);
                IDataReader CurrentReader = ExecuteReader(mCommand);
                if (CurrentReader == null)
                {
                    return null;
                }
                if (CurrentReader.Read())
                {
                    BillingInvoice = GetInPatientBillingInvoiceFromReader(CurrentReader);
                }
                if (CurrentReader.NextResult())
                {
                    var retVal = GetPatientRegistrationDetailsCollectionFromReader(CurrentReader);
                    if (retVal != null)
                    {
                        BillingInvoice.RegistrationDetails = retVal.ToObservableCollection();
                    }
                }
                if (CurrentReader.NextResult())
                {
                    var retVal = FillPclRequestList(CurrentReader);
                    if (retVal != null)
                    {
                        BillingInvoice.PclRequests = retVal.ToObservableCollection();
                    }
                }
                if (CurrentReader.NextResult())
                {
                    var retVal = GetAllInPatientInvoices(CurrentReader);
                    if (retVal != null)
                    {
                        BillingInvoice.OutwardDrugClinicDeptInvoices = retVal.ToObservableCollection();
                    }
                }
                CurrentReader.Close();
                CleanUpConnectionAndCommand(null, mCommand);
                return BillingInvoice;
            }
        }
        public IList<InPatientBillingInvoice> GetQuotationCollection(short ViewCase)
        {
            using (SqlConnection aConnection = new SqlConnection(this.ConnectionString))
            {
                aConnection.Open();
                SqlCommand cmd = (SqlCommand)aConnection.CreateCommand();
                cmd.CommandText = "spGetQuotationCollection";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("ViewCase", SqlDbType.TinyInt, ConvertNullObjectToDBNull(ViewCase));
                IDataReader CurrentReader = ExecuteReader(cmd);
                if (CurrentReader == null)
                {
                    return null;
                }
                List<InPatientBillingInvoice> QuotationCollection = GetInPatientBillingInvoiceCollectionFromReader(CurrentReader);
                CurrentReader.Close();
                CleanUpConnectionAndCommand(null, cmd);
                return QuotationCollection;
            }
        }
        public void RemoveQuotation(long InPatientBillingInvID)
        {
            using (SqlConnection aConnection = new SqlConnection(this.ConnectionString))
            {
                aConnection.Open();
                SqlCommand aCommand;
                aCommand = (SqlCommand)aConnection.CreateCommand();
                aCommand.CommandText = "spRemoveQuotation";
                aCommand.CommandType = CommandType.StoredProcedure;
                aCommand.AddParameter("@InPatientBillingInvID", SqlDbType.BigInt, ConvertNullObjectToDBNull(InPatientBillingInvID));
                ExecuteNonQuery(aCommand);
                CleanUpConnectionAndCommand(null, aCommand);
            }
        }
        public void CreatePatientQuotation(long InPatientBillingInvID, long PatientID, string QuotationTitle)
        {
            using (SqlConnection aConnection = new SqlConnection(this.ConnectionString))
            {
                aConnection.Open();
                SqlCommand aCommand;
                aCommand = (SqlCommand)aConnection.CreateCommand();
                aCommand.CommandText = "spCreatePatientQuotation";
                aCommand.CommandType = CommandType.StoredProcedure;
                aCommand.AddParameter("@InPatientBillingInvID", SqlDbType.BigInt, ConvertNullObjectToDBNull(InPatientBillingInvID));
                aCommand.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                aCommand.AddParameter("@QuotationTitle", SqlDbType.NVarChar, ConvertNullObjectToDBNull(QuotationTitle));
                ExecuteNonQuery(aCommand);
                CleanUpConnectionAndCommand(null, aCommand);
            }
        }

        public bool AddUpdateRegistrationDetailsForQuotation(long InPatientBillingInvID, List<PatientRegistrationDetail> AddRegistrationList, List<PatientRegistrationDetail> ModifiedRegistrationList)
        {
            using (SqlConnection aConnection = new SqlConnection(this.ConnectionString))
            {
                aConnection.Open();
                SqlCommand aCommand;
                aCommand = (SqlCommand)aConnection.CreateCommand();
                aCommand.CommandText = "spAddUpdateRegistrationDetailsForQuotation";
                aCommand.CommandType = CommandType.StoredProcedure;
                aCommand.AddParameter("@QuotationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(InPatientBillingInvID));

                string AddXml = null;
                string UpdateXml = null;
                if (AddRegistrationList != null && AddRegistrationList.Count > 0)
                {
                    AddXml = ConvertPatientRegistrationDetailsToXml(AddRegistrationList).ToString();
                }
                aCommand.AddParameter("@AddXml", SqlDbType.Xml, ConvertNullObjectToDBNull(AddXml));
                if (ModifiedRegistrationList != null && ModifiedRegistrationList.Count > 0)
                {
                    UpdateXml = ConvertPatientRegistrationDetailsToXml(ModifiedRegistrationList).ToString();
                }
                aCommand.AddParameter("@UpdateXml", SqlDbType.Xml, ConvertNullObjectToDBNull(UpdateXml));
                var retVal = ExecuteNonQuery(aCommand);
                CleanUpConnectionAndCommand(null, aCommand);
                return retVal > 0;
            }
        }
        public bool AddUpdatePCLRequestDetailsForQuotation(long InPatientBillingInvID
            , List<PatientPCLRequestDetail> AddPCLRequestDetailsList
            , List<PatientPCLRequestDetail> ModifiedPCLRequestDetailsList)
        {
            using (SqlConnection aConnection = new SqlConnection(this.ConnectionString))
            {
                aConnection.Open();
                SqlCommand aCommand;
                aCommand = (SqlCommand)aConnection.CreateCommand();
                aCommand.CommandText = "spAddUpdatePatientPCLRequestDetailsForQuotation";
                aCommand.CommandType = CommandType.StoredProcedure;
                aCommand.AddParameter("@QuotationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(InPatientBillingInvID));
                string AddXml = null;
                string UpdateXml = null;
                if (AddPCLRequestDetailsList != null && AddPCLRequestDetailsList.Count > 0)
                {
                    AddXml = ConvertPCLRequestDetailsToXml(AddPCLRequestDetailsList).ToString();
                }
                aCommand.AddParameter("@AddXml", SqlDbType.Xml, ConvertNullObjectToDBNull(AddXml));
                if (ModifiedPCLRequestDetailsList != null && ModifiedPCLRequestDetailsList.Count > 0)
                {
                    UpdateXml = ConvertPCLRequestDetailsToXml(ModifiedPCLRequestDetailsList).ToString();
                }
                aCommand.AddParameter("@UpdateXml", SqlDbType.Xml, ConvertNullObjectToDBNull(UpdateXml));
                var retVal = ExecuteNonQuery(aCommand);
                CleanUpConnectionAndCommand(null, aCommand);
                return retVal > 0;
            }
        }

        public bool AddUpdateOutwardDrugForQuotation(long InPatientBillingInvID, OutwardDrugClinicDeptInvoice OutwardDrugInvoice)
        {
            using (SqlConnection aConnection = new SqlConnection(this.ConnectionString))
            {
                aConnection.Open();
                SqlCommand aCommand;
                aCommand = (SqlCommand)aConnection.CreateCommand();
                aCommand.CommandText = "spAddUpdateDrugForQuotation";
                aCommand.CommandType = CommandType.StoredProcedure;
                aCommand.AddParameter("@QuotationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(InPatientBillingInvID));
                string OutwardDrugInvoiceXML = null;
                if (OutwardDrugInvoice != null && OutwardDrugInvoice.OutwardDrugClinicDepts.Count > 0)
                {
                    OutwardDrugInvoiceXML = ConvertOutwardDrugClinicDeptInvoiceToXmlElement(OutwardDrugInvoice).ToString();
                }
                aCommand.AddParameter("@Xml", SqlDbType.Xml, ConvertNullObjectToDBNull(OutwardDrugInvoiceXML));
                var retVal = ExecuteNonQuery(aCommand);
                CleanUpConnectionAndCommand(null, aCommand);
                return retVal > 0;
            }
        }

        public bool CalAdditionalFeeAndTotalBillForQuotation(long InPatientBillingInvID, long StaffID, bool IsUpdateBill, double? HIBenefit)
        {
            using (SqlConnection aConnection = new SqlConnection(this.ConnectionString))
            {
                aConnection.Open();
                SqlCommand aCommand;
                aCommand = (SqlCommand)aConnection.CreateCommand();
                aCommand.CommandText = "spCalAdditionalFeeAndTotalBillForQuotation";
                aCommand.CommandType = CommandType.StoredProcedure;
                aCommand.AddParameter("@InPatientBillingInvID", SqlDbType.BigInt, ConvertNullObjectToDBNull(InPatientBillingInvID));
                aCommand.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                aCommand.AddParameter("@IsUpdateBill", SqlDbType.Bit, ConvertNullObjectToDBNull(IsUpdateBill));
                aCommand.AddParameter("@HIBenefit", SqlDbType.Float, ConvertNullObjectToDBNull(HIBenefit));
                aCommand.AddParameter("@IsHICard_FiveYearsCont_NoPaid", SqlDbType.Bit, ConvertNullObjectToDBNull(false));
                aCommand.AddParameter("@CreatedDate", SqlDbType.DateTime, DateTime.Now);
                var retVal = ExecuteNonQuery(aCommand);
                CleanUpConnectionAndCommand(null, aCommand);
                return retVal > 0;
            }
        }
        #endregion

        //▼====: #019
        public bool UpdatePclRequestTransferToPAC(ObservableCollection<PatientPCLRequest> PatientPCLRequestList, long V_PCLRequestType)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "sp_UpdatePclRequestTransferToPAC";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientPCLRequestListXML", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertPatientPCLRequestToXml(PatientPCLRequestList)));
                cmd.AddParameter("@V_PCLRequestType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_PCLRequestType));
                conn.Open();
                cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(conn, cmd);
                return true;
            }
        }

        public string ConvertPatientPCLRequestToXml(ObservableCollection<PatientPCLRequest> PatientPCLRequestList)
        {
            if (PatientPCLRequestList == null || PatientPCLRequestList.Count == 0)
            {
                return null;
            }
            XDocument mXDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), new XElement("PatientPCLRequests",
                from item in PatientPCLRequestList
                select new XElement("PatientPCLRequest",
                  new XElement("PatientPCLReqID", item.PatientPCLReqID),
                  new XElement("IsTransferredToRIS", item.IsTransferredToRIS),
                  new XElement("IsCancelTransferredToRIS", item.IsCancelTransferredToRIS)
                  )));

            return mXDocument.ToString();
        }
        //▲====: #019
        //▼====: #022
        public TicketCare SaveTicketCare(TicketCare gTicketCare, long CreatedStaffID)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spSaveTicketCares", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = int.MaxValue;
                cmd.AddParameter("@TicketCareID", SqlDbType.BigInt, ConvertNullObjectToDBNull(gTicketCare.TicketCareID));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(gTicketCare.StaffID));
                cmd.AddParameter("@IntPtDiagDrInstructionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(gTicketCare.IntPtDiagDrInstructionID));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(gTicketCare.PtRegistrationID));
                cmd.AddParameter("@CreatedStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CreatedStaffID));
                cmd.AddParameter("@OrientedTreatment", SqlDbType.NVarChar, ConvertNullObjectToDBNull(gTicketCare.OrientedTreatment));
                cmd.AddParameter("@ExcuteInstruction", SqlDbType.NVarChar, ConvertNullObjectToDBNull(gTicketCare.ExcuteInstruction));
                cmd.AddParameter("@DateExcute", SqlDbType.DateTime, ConvertNullObjectToDBNull(gTicketCare.DateExcute));
                cmd.AddParameter("@MarkAsDeleted", SqlDbType.Bit, ConvertNullObjectToDBNull(gTicketCare.MarkAsDeleted));
                cmd.AddParameter("@V_LevelCare", SqlDbType.BigInt, ConvertNullObjectToDBNull(gTicketCare.V_LevelCare));
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(gTicketCare.V_RegistrationType));
                conn.Open();
                TicketCare retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    retVal = GetTicketCareFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(conn, cmd);
                return retVal;
            }
        }

        public TicketCare GetTicketCare(long IntPtDiagDrInstructionID)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetTicketCare", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = int.MaxValue;
                cmd.AddParameter("@IntPtDiagDrInstructionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(IntPtDiagDrInstructionID));
                conn.Open();
                TicketCare retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    retVal = GetTicketCareFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(conn, cmd);
                return retVal;
            }
        }

        public List<TicketCare> GetTicketCareListForRegistration(long PtRegistrationID)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetTicketCareForRegistration", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = int.MaxValue;
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                conn.Open();
                List<TicketCare> retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                retVal = GetTicketCareListFromReader(reader);
                reader.Close();

                CleanUpConnectionAndCommand(conn, cmd);
                return retVal;
            }
        }

        public List<ExecuteDrug> GetExecuteDrugListForRegistration(long PtRegistrationID)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetExecuteDrugForRegistration", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = int.MaxValue;
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                conn.Open();
                List<ExecuteDrug> retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                retVal = GetExecuteDrugListFromReader(reader);
                reader.Close();

                CleanUpConnectionAndCommand(conn, cmd);
                return retVal;
            }
        }

        public bool SaveExecuteDrug(long ExecuteDrugID, long ExecuteDrugDetailID, long StaffID
            , long CreatedStaffID, DateTime DateExecute)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spSaveExecuteDrug", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = int.MaxValue;
                cmd.AddParameter("@ExecuteDrugID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ExecuteDrugID));
                cmd.AddParameter("@ExecuteDrugDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ExecuteDrugDetailID));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                cmd.AddParameter("@CreatedStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CreatedStaffID));
                cmd.AddParameter("@DateExecute", SqlDbType.DateTime, ConvertNullObjectToDBNull(DateExecute));
                conn.Open();
                var retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(conn, cmd);
                return retVal > 0;
            }
        }

        public bool DeleteExecuteDrug(long ExecuteDrugDetailID, long CreatedStaffID)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spDeleteExecuteDrug", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = int.MaxValue;
                cmd.AddParameter("@ExecuteDrugDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ExecuteDrugDetailID));
                cmd.AddParameter("@CreatedStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CreatedStaffID));
                conn.Open();
                var retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(conn, cmd);
                return retVal > 0;
            }
        }
        //▲====: #022
        //▼====: #024
        public List<PatientRegistrationDetail> GetListMedServiceInSurgeryDept(long PtRegistrationID)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetListMedServiceInSurgeryDept", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = int.MaxValue;
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                conn.Open();
                List<PatientRegistrationDetail> retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                retVal = GetPatientRegistrationDetailsCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(conn, cmd);
                return retVal;
            }
        }
        //▲====: #024
        //▼====: #025
        public List<PatientRegistrationDetail> GetAllRegistrationDetailsByPtRegistrationID(long PtRegistrationID
            , long DeptID, DateTime FromDate, DateTime ToDate
            , DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            SqlCommand cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spGetAllRegistrationDetailsByPtRegistrationID";
            cmd.CommandTimeout = int.MaxValue;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, PtRegistrationID);
            cmd.AddParameter("@DeptID", SqlDbType.BigInt, DeptID);
            cmd.AddParameter("@FromDate", SqlDbType.DateTime, FromDate);
            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ToDate);
            IDataReader reader = ExecuteReader(cmd);
            if (reader == null)
            {
                return null;
            }
            List<PatientRegistrationDetail> retVal = GetPatientRegistrationDetailsCollectionFromReader(reader);
            reader.Close();
            cmd.Dispose();
            return retVal;
        }

        public List<PatientPCLRequest> GetPCLRequestListByPtRegistrationID(long PtRegistrationID
            , long DeptID, DateTime FromDate, DateTime ToDate
            , bool IsPassCheckNonBlockValidPCLExamDate
            , DbConnection connection, DbTransaction tran)
        {
            SqlCommand cmd;
            cmd = (SqlCommand)connection.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spGetPCLRequestByPtRegistrationID";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = int.MaxValue;
            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, PtRegistrationID);
            cmd.AddParameter("@DeptID", SqlDbType.BigInt, DeptID);
            cmd.AddParameter("@FromDate", SqlDbType.DateTime, FromDate);
            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ToDate);
            cmd.AddParameter("@IsPassCheckNonBlockValidPCLExamDate", SqlDbType.Bit, IsPassCheckNonBlockValidPCLExamDate);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            IDataReader reader = ExecuteReader(cmd);
            var retVal = FillPclRequestList(reader);
            reader.Close();
            CleanUpConnectionAndCommand(null, cmd);
            return retVal;
        }

        public List<OutwardDrugClinicDeptInvoice> GetAllInPatientInvoicesByPtRegistrationID(long PtRegistrationID
            , long DeptID, DateTime FromDate, DateTime ToDate
            , DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            SqlCommand cmd;
            cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "spGetAllInPatientInvoicesByPtRegistrationID";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = int.MaxValue;

            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, PtRegistrationID);
            cmd.AddParameter("@DeptID", SqlDbType.BigInt, DeptID);
            cmd.AddParameter("@FromDate", SqlDbType.DateTime, FromDate);
            cmd.AddParameter("@ToDate", SqlDbType.DateTime, ToDate);

            IDataReader reader = ExecuteReader(cmd);
            var retVal = GetAllInPatientInvoices(reader);
            reader.Close();
            CleanUpConnectionAndCommand(null, cmd);
            return retVal;
        }

        public bool UpdateInPatientBillingInvoiceByPtRegistrationID(long? StaffID
            , double? HIBenefit, bool IsHICard_FiveYearsCont_NoPaid
            , List<PatientRegistrationDetail> deletedRegDetails
            , List<PatientPCLRequestDetail> deletedPclRequestDetails
            , List<OutwardDrugClinicDept> deleteOutwardDrugClinicDepts
            , List<PatientRegistrationDetail> modifiedRegDetails
            , List<PatientPCLRequestDetail> modifiedPclRequestDetails
            , List<OutwardDrugClinicDept> modifiedOutwardDrugClinicDepts
            , bool IsNotCheckInvalid
            , DbConnection conn, DbTransaction tran)
        {
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                SqlCommand cmd;

                cmd = (SqlCommand)conn.CreateCommand();
                cmd.Transaction = (SqlTransaction)tran;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "sp_UpdateInPatientBillingInvoiceByPtRegistrationID";
                cmd.CommandTimeout = int.MaxValue;

                string detailListString = null;
                //những DVKT bị cập nhật
                if (modifiedRegDetails != null && modifiedRegDetails.Count > 0)
                {
                    detailListString = ConvertPatientRegistrationDetailsToXml(modifiedRegDetails).ToString();
                    cmd.AddParameter("@modifiedRegDetails", SqlDbType.Xml, ConvertNullObjectToDBNull(detailListString));
                }

                if (modifiedPclRequestDetails != null && modifiedPclRequestDetails.Count > 0)
                {
                    detailListString = ConvertPCLRequestDetailsToXml(modifiedPclRequestDetails).ToString();
                    cmd.AddParameter("@modifiedPclRequestDetails", SqlDbType.Xml, ConvertNullObjectToDBNull(detailListString));
                }

                if (modifiedOutwardDrugClinicDepts != null && modifiedOutwardDrugClinicDepts.Count > 0)
                {
                    XDocument xmlDocumentDrugModified = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                          new XElement("OutwardDrugClinicDepts",
                          from details in modifiedOutwardDrugClinicDepts
                          select new XElement("OutwardDrugClinicDept",
                                new XElement("OutID", details.OutID),
                                new XElement("InID", details.InID),
                                new XElement("GenMedProductID", details.GenMedProductID),
                                new XElement("Qty", details.Qty),
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
                                new XElement("HisID", details.HisID),
                                new XElement("CountValue", details.CountValue),
                                new XElement("GenMedVersionID", details.GenMedVersionID)
                                , new XElement("OtherAmt", details.OtherAmt)
                                , new XElement("IsCountPatientCOVID", details.IsCountPatientCOVID)
                                , new XElement("InPatientBillingInvID", details.DrugInvoice?.InPatientBillingInvID)
                                //▼====: #034
                                , new XElement("IsCountSE", details.IsCountSE)
                                //▲====: #034
                           )));
                    cmd.AddParameter("@modifiedOutwardDrugClinicDepts", SqlDbType.Xml, ConvertNullObjectToDBNull(xmlDocumentDrugModified.ToString()));
                }

                //những DVKT bị xóa
                if (deletedRegDetails != null && deletedRegDetails.Count > 0)
                {
                    XDocument xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                          new XElement("RegistrationDetails",
                          from details in deletedRegDetails
                          select new XElement("RecInfo",
                              new XElement("PtRegDetailID", details.PtRegDetailID)
                              , new XElement("InPatientBillingInvID", details.InPatientBillingInvID)
                           )));

                    cmd.AddParameter("@deletedRegDetails", SqlDbType.Xml, ConvertNullObjectToDBNull(xmlDocument.ToString()));
                }

                if (@deletedPclRequestDetails != null && @deletedPclRequestDetails.Count > 0)
                {
                    detailListString = ConvertPCLRequestDetailsToXml(@deletedPclRequestDetails).ToString();
                    cmd.AddParameter("@deletedPclRequestDetails", SqlDbType.Xml, ConvertNullObjectToDBNull(detailListString));
                }

                if (deleteOutwardDrugClinicDepts != null && deleteOutwardDrugClinicDepts.Count > 0)
                {
                    XDocument xmlDocumentDrugDeleted = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                                new XElement("OutwardDrugClinicDeptInvoices",
                                from details in deleteOutwardDrugClinicDepts
                                select new XElement("RecInfo",
                                    new XElement("outiID", details.outiID)
                                    , new XElement("InPatientBillingInvID", details.DrugInvoice?.InPatientBillingInvID)
                                 )));
                    cmd.AddParameter("@deleteOutwardDrugClinicDepts", SqlDbType.Xml, ConvertNullObjectToDBNull(xmlDocumentDrugDeleted.ToString()));
                }

                cmd.AddParameter("@HIBenefit", SqlDbType.Float, ConvertNullObjectToDBNull(HIBenefit));
                cmd.AddParameter("@IsHICard_FiveYearsCont_NoPaid", SqlDbType.Bit, ConvertNullObjectToDBNull(IsHICard_FiveYearsCont_NoPaid));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                cmd.AddParameter("@IsNotCheckInvalid", SqlDbType.Bit, ConvertNullObjectToDBNull(IsNotCheckInvalid));

                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(null, cmd);
                return retVal > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //▲====: #025
        public bool CreateRequestDrugInwardClinicDept_ByPCLRequest(PatientPCLRequest PatientPCLRequest, out long ReqDrugInClinicDeptID )
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "spRequestDrugInwardClinicDept_InsertNewByPCLRequest";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLRequest.PatientPCLReqID));
                cmd.AddParameter("@ReqDrugInClinicDeptID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                conn.Open();
                var reader = ExecuteReader(cmd);

                // var patients = GetPatientCollectionFromReader(reader);
                reader.Close();

                if (cmd.Parameters["@ReqDrugInClinicDeptID"].Value != DBNull.Value)
                {
                    ReqDrugInClinicDeptID = (long)cmd.Parameters["@ReqDrugInClinicDeptID"].Value;
                }
                else
                {
                    ReqDrugInClinicDeptID = -1;
                }
                CleanUpConnectionAndCommand(conn, cmd);
                return true;
            }
        }
        //▼====: #029
        public bool AddUpdateDoctorContactPatientTime(DoctorContactPatientTime doctorContactPatientTime)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "spAddUpdateDoctorContactPatientTime";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(doctorContactPatientTime.PatientID));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(doctorContactPatientTime.PtRegistrationID));
                cmd.AddParameter("@PtRegDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(doctorContactPatientTime.PtRegDetailID));
                cmd.AddParameter("@StartDatetime", SqlDbType.DateTime, ConvertNullObjectToDBNull(doctorContactPatientTime.StartDatetime));
                cmd.AddParameter("@EndDatetime", SqlDbType.DateTime, ConvertNullObjectToDBNull(doctorContactPatientTime.EndDatetime));
                cmd.AddParameter("@Log", SqlDbType.NVarChar, ConvertNullObjectToDBNull(doctorContactPatientTime.Log));
                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(doctorContactPatientTime.DoctorStaffID));
                
                conn.Open();
                int result = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(conn, cmd);
                return result>0;
            }
        }
        //▲====: #029
        //▼====: #028
        public void AddBillingInvoice_InPt(PatientRegistration CurrentRegistration, long StaffID)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_AddBillingInvoiceMerge_InPt", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentRegistration.InPtRegistrationID));
                cmd.AddParameter("@OutPtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentRegistration.PtRegistrationID));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                cn.Open();
                ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
            }
        }
        //▲====: #028
        //▼====: #032
        //▼==== #044
        public bool GetLastIDC10MainTreatmentProgram_ByPtRegID(long PtRegistrationID, long PtRegDetailID, out long LastOutpatientTreatmentTypeID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                LastOutpatientTreatmentTypeID = 0;
                SqlCommand cmd = new SqlCommand("spGetLastIDC10MainTreatmentProgram_ByPtRegID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@PtRegDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegDetailID));

                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    if (reader.Read())
                    {
                        if (reader.HasColumn("OutpatientTreatmentTypeID") && reader["OutpatientTreatmentTypeID"] != DBNull.Value)
                        {
                            LastOutpatientTreatmentTypeID = Convert.ToInt64(reader["OutpatientTreatmentTypeID"]);
                        }
                    }
                    reader.Close();
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return true;
            }
        }
        //▲==== #044
        public List<OutpatientTreatmentType> GetAllOutpatientTreatmentType()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllOutpatientTreatmentType", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                //List<OutpatientTreatmentType> retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                //if (reader.Read())
                //{
                //    retVal = GetOutpatientTreatmentTypeCollectionFromReader(reader);
                //}
                //reader.Close();

                List<OutpatientTreatmentType> retVal = new List<OutpatientTreatmentType>();
                while (reader.Read())
                {
                    OutpatientTreatmentType mItem = new OutpatientTreatmentType();
                    mItem = GetOutpatientTreatmentTypeFromReader(reader);
                    retVal.Add(mItem);
                }
                if (reader.NextResult())
                {
                    List<OutpatientTreatmentTypeICD10Link> ChildCollection = new List<OutpatientTreatmentTypeICD10Link>();
                    while (reader.Read())
                    {
                        OutpatientTreatmentTypeICD10Link mItem = new OutpatientTreatmentTypeICD10Link();
                        mItem = GetOutpatientTreatmentTypeICD10LinkFromReader(reader);
                        ChildCollection.Add(mItem);
                    }
                    if (ChildCollection != null && ChildCollection.Count > 0)
                    {
                        foreach (var aItem in retVal)
                        {
                            if (!ChildCollection.Any(x => x.OutpatientTreatmentTypeID == aItem.OutpatientTreatmentTypeID))
                            {
                                continue;
                            }
                            aItem.ListICD10Code = ChildCollection.Where(x => x.OutpatientTreatmentTypeID == aItem.OutpatientTreatmentTypeID).ToObservableCollection();
                        }
                    }
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        //▲====: #032
        public bool RefundInPatientCost(PatientRegistration CurrentRegistration, long StaffID)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefundInPatientCost", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentRegistration.PtRegistrationID));
                cmd.AddParameter("@OutPtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentRegistration.OutPtRegistrationID));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                cn.Open();
                var result = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                return result > 0;
            }
        }
        //▼==== #035
        public List<PrescriptionIssueHistory> GetPrescriptionIssueHistory_ByPtRegDetailID(long PtRegDetailID, long V_RegistrationType)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spGetPrescriptionIssueHistory_ByPtRegDetailID", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PtRegDetailID", SqlDbType.BigInt, PtRegDetailID);
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, V_RegistrationType);
                conn.Open();
                List<PrescriptionIssueHistory> retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                retVal = GetPtPrescriptIssueHisCollectionFromReader(reader);
                reader.Close();

                CleanUpConnectionAndCommand(conn, cmd);
                return retVal;
            }
        }
        //▲==== #035
        //▼====: #036
        public bool UpdateIsConfirmedEmergencyPatient(long PtRegistrationID, bool IsConfirmedEmergencyPatient)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUpdateIsConfirmedEmergencyPatient", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@IsConfirmedEmergencyPatient", SqlDbType.Bit, ConvertNullObjectToDBNull(@IsConfirmedEmergencyPatient));
                cn.Open();
                int result = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);

                return result > 0;
            }
        }
        //▲====: #036
        //▼====: #037
        public PatientCardDetail GetCardDetailByPatientID(long PatientID)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetCardDetailByPatientID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
            
                cn.Open();
                PatientCardDetail retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                
                if (reader != null)
                {
                    if (reader.Read())
                    {
                        retVal = GetPatientCardDetailFromReader(reader);
                    }
                    reader.Close();
                }
              
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public bool SaveCardDetail(PatientCardDetail Obj, bool IsExtendCard)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSaveCardDetail", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientCardDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PatientCardDetailID));
                cmd.AddParameter("@CardID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.CardID));
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PatientID));
                cmd.AddParameter("@AccountNumber", SqlDbType.VarChar, ConvertNullObjectToDBNull(Obj.AccountNumber));
                cmd.AddParameter("@V_PatientClass", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_PatientClass));
                cmd.AddParameter("@OpenCardDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Obj.OpenCardDate));
                cmd.AddParameter("@ExpireCardDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Obj.ExpireCardDate));
                cmd.AddParameter("@CreatedStaff", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.CreatedStaff));
                cmd.AddParameter("@Logmodified", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.Logmodified));
                cmd.AddParameter("@IsExtendCard", SqlDbType.Bit, ConvertNullObjectToDBNull(IsExtendCard));
            
                cn.Open();
                int result = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);

                return result > 0;
            }
        }
        //▲====: #037
        //▼==== #040
        public bool CheckBeforeMergerPatientRegistration(long registrationID, out string errorMessages, out string confirmMessages)
        {
            try
            {
                errorMessages = "";
                confirmMessages = "";
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spCheckBeforeMergerPatientRegistration", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, registrationID);
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
                    CleanUpConnectionAndCommand(cn, cmd);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //▲==== #040
        public DeathCheckRecord GetDeathCheckRecordByPtRegID(long PtRegistrationID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetDeathCheckRecordByPtRegID", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, PtRegistrationID);
                    cn.Open();
                    DeathCheckRecord retVal = null;
                    IDataReader reader = ExecuteReader(cmd);
                    if (reader != null)
                    {
                        if (reader.Read())
                        {
                            retVal = GetDeathCheckRecordFromReader(reader);
                        }
                        reader.Close();
                    }
                    CleanUpConnectionAndCommand(cn, cmd);
                    return retVal;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool SaveDeathCheckRecord(DeathCheckRecord CurDeathCheckRecord)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spSaveDeathCheckRecord", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@DeathCheckRecordID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurDeathCheckRecord.DeathCheckRecordID));
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurDeathCheckRecord.PtRegistrationID));
                    cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurDeathCheckRecord.PatientID));
                    cmd.AddParameter("@MedicalCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurDeathCheckRecord.MedicalCode));
                    cmd.AddParameter("@CheckRecordDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(CurDeathCheckRecord.CheckRecordDate));
                    cmd.AddParameter("@PresideStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurDeathCheckRecord.PresideStaffID));
                    cmd.AddParameter("@SecretaryStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurDeathCheckRecord.SecretaryStaffID));
                    cmd.AddParameter("@MemberStaff", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurDeathCheckRecord.MemberStaff));
                    cmd.AddParameter("@TreatmentProcess", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurDeathCheckRecord.TreatmentProcess));
                    cmd.AddParameter("@Conclude", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurDeathCheckRecord.Conclude));
                    cmd.AddParameter("@IsDeleted", SqlDbType.Bit, ConvertNullObjectToDBNull(CurDeathCheckRecord.IsDeleted));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurDeathCheckRecord.StaffID));
                    cn.Open();
                    
                    int result  = ExecuteNonQuery(cmd);
                    CleanUpConnectionAndCommand(cn, cmd);
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

public static class ObservableCollectionExtensions
{
    public static void AddRange<T>(this ObservableCollection<T> root, IEnumerable<T> collection)
    {
        if (collection == null)
        {
            return;
        }
        foreach (var item in collection)
        {
            root.Add(item);
        }
    }
}

// VuTTM Begin
public class OutPatientCashAdvanceDetails
{
	public long OutPtCashAdvanceID { get; set; }
	public long PtRegistrationID { get; set; }
	public long? BankingTransactionId { get; set; }
	public long? BankingRefundTransactionId { get; set; }
	public string CashAdvReceiptNum { get; set; }
	public decimal PaymentAmount { get; set; }
	public decimal BalanceAmount { get; set; }
	public string CanceledCashAdvReceiptNum { get; set; }
	public long V_PaymentReason { get; set; }
	public long V_PaymentMode { get; set; }
	public long PtCashAdvanceLinkID { get; set; }
	public long TransItemID { get; set; }
	public long? PtRegDetailID { get; set; }
	public long MedServiceID { get; set; }
	public long? outiID { get; set; }
	public long? PCLRequestID { get; set; }
	public decimal Amount { get; set; }
	public decimal PriceDifference { get; set; }
	public decimal AmountCoPay { get; set; }
	public decimal HealthInsuranceRebate { get; set; }

	public string ServiceName { get; set; }
}
// VuTTM End
