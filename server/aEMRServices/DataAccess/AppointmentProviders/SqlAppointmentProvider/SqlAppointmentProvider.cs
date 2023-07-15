using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using DataEntities;
using eHCMS.Services.Core;
using Service.Core.Common;
using eHCMS.Configurations;
//using SqlAppointmentProvider;

namespace eHCMS.DAL
{
    public class SqlAppointmentProvider : AppointmentProvider
    {
        public SqlAppointmentProvider()
            : base()
        {

        }
        public override List<PatientAppointment> SearchAppointments(AppointmentSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int TotalCount)
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("sp_Appointments_Search", cn)
                              {
                                  CommandType = CommandType.StoredProcedure
                              };

                cmd.AddParameter("@FullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.FullName));
                cmd.AddParameter("@PatientCode", SqlDbType.Char, ConvertNullObjectToDBNull(criteria.PatientCode), ParameterDirection.Input);
                cmd.AddParameter("@HealthInsuranceCard", SqlDbType.VarChar, 15, ConvertNullObjectToDBNull(criteria.InsuranceCard), ParameterDirection.Input);

                cmd.AddParameter("@DateFrom", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.DateFrom));
                cmd.AddParameter("@DateTo", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.DateTo));
                cmd.AddParameter("@V_ApptStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.V_ApptStatus));
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptLocationID));                

                cmd.AddParameter("@PageSize", SqlDbType.Int, pageSize);
                cmd.AddParameter("@PageIndex", SqlDbType.Int, pageIndex);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, DBNull.Value);
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, bCountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                var retVal = new List<PatientAppointment>();
                var reader = ExecuteReader(cmd);

                PatientAppointment temp;
                while (reader.Read())
                {
                    temp = GetAppointmentFromReader(reader);
                    try
                    {
                        temp.Patient = GetPatientFromReader(reader);
                    }
                    catch
                    {
                        temp.Patient = null;
                    }

                    retVal.Add(temp);
                }
                reader.Close();
                if (bCountTotal && cmd.Parameters["@Total"].Value != DBNull.Value)
                {
                    TotalCount = (int)cmd.Parameters["@Total"].Value;
                }
                else
                {
                    TotalCount = 0;
                }
                return retVal;
            }
        }
        public override List<PatientAppointment> GetAllAppointments(AppointmentSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int TotalCount)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAppointments", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.PatientID));
                cmd.AddParameter("@DateFrom", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.DateFrom));
                cmd.AddParameter("@DateTo", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.DateTo));
                cmd.AddParameter("@V_ApptStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.V_ApptStatus));
                cmd.AddParameter("@LoaiDV", SqlDbType.SmallInt, ConvertNullObjectToDBNull(criteria.LoaiDV));//0:Cả  Hai;1:KB;2:CLS
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DeptLocationID));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.StaffID));
                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.DoctorStaffID));
                if (criteria.ApptTimeSegment.ConsultationTimeSegmentID > 0)
                {
                    cmd.AddParameter("@StartTime", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ApptTimeSegment.StartTime));
                    cmd.AddParameter("@EndTime", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.ApptTimeSegment.EndTime));
                }
                else
                {
                    cmd.AddParameter("@StartTime", SqlDbType.DateTime, ConvertNullObjectToDBNull(null));
                    cmd.AddParameter("@EndTime", SqlDbType.DateTime, ConvertNullObjectToDBNull(null));
                }


                cmd.AddParameter("@IsConsultation", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.IsConsultation));

                cmd.AddParameter("@PageSize", SqlDbType.Int, pageSize);
                cmd.AddParameter("@PageIndex", SqlDbType.Int, pageIndex);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, bCountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<PatientAppointment> retVal = new List<PatientAppointment>();
                IDataReader reader = ExecuteReader(cmd);

                PatientAppointment temp;
                while (reader.Read())
                {
                    temp = GetAppointmentFromReader(reader);
                    try
                    {
                        temp.ApptStatus = GetLookupFromReader(reader);
                    }
                    catch
                    {
                        temp.ApptStatus = null;
                    }
                    try
                    {
                        temp.Patient = GetPatientFromReader(reader);
                    }
                    catch
                    {
                        temp.Patient = null;
                    }

                    retVal.Add(temp);
                }
                reader.Close();
                if (bCountTotal && cmd.Parameters["@Total"].Value != DBNull.Value)
                {
                    TotalCount = (int)cmd.Parameters["@Total"].Value;
                }
                else
                {
                    TotalCount = 0;
                }
                return retVal;
            }
        }
        public override List<PatientAppointment> GetAllAppointmentsDay(AppointmentSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int TotalCount)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAppointmentsDay", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.PatientID));
                cmd.AddParameter("@DateFrom", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.DateFrom));
                cmd.AddParameter("@DateTo", SqlDbType.DateTime, ConvertNullObjectToDBNull(criteria.DateTo));
                cmd.AddParameter("@V_ApptStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.V_ApptStatus));

                cmd.AddParameter("@PageSize", SqlDbType.Int, pageSize);
                cmd.AddParameter("@PageIndex", SqlDbType.Int, pageIndex);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, DBNull.Value);
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, bCountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<PatientAppointment> retVal = new List<PatientAppointment>();
                IDataReader reader = ExecuteReader(cmd);
                //retVal = GetAppointmentCollectionFromReader(reader);
                PatientAppointment temp;
                while (reader.Read())
                {
                    temp = GetAppointmentFromReader(reader);
                    try
                    {
                        temp.ApptStatus = GetLookupFromReader(reader);
                    }
                    catch
                    {
                        temp.ApptStatus = null;
                    }
                    try
                    {
                        temp.Patient = GetPatientFromReader(reader);
                    }
                    catch
                    {
                        temp.Patient = null;
                    }

                    retVal.Add(temp);
                }
                reader.Close();
                if (bCountTotal && cmd.Parameters["@Total"].Value != DBNull.Value)
                {
                    TotalCount = (int)cmd.Parameters["@Total"].Value;
                }
                else
                {
                    TotalCount = 0;
                }
                return retVal;
            }
        }
        public override List<PatientAppointment> GetAppointmentsOfPatient(long patientID, int pageIndex, int pageSize, bool bCountTotal, out int TotalCount)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAppointmentsOfPatient", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientID", SqlDbType.BigInt, patientID);
                cmd.AddParameter("@PageSize", SqlDbType.Int, pageSize);
                cmd.AddParameter("@PageIndex", SqlDbType.Int, pageIndex);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, DBNull.Value);
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, bCountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<PatientAppointment> retVal = new List<PatientAppointment>();
                IDataReader reader = ExecuteReader(cmd);
                //retVal = GetAppointmentCollectionFromReader(reader);
                PatientAppointment temp;
                while (reader.Read())
                {
                    temp = GetAppointmentFromReader(reader);
                    try
                    {
                        temp.ApptStatus = GetLookupFromReader(reader);
                    }
                    catch
                    {
                        temp.ApptStatus = null;
                    }
                    retVal.Add(temp);
                }
                reader.Close();
                if (bCountTotal && cmd.Parameters["@Total"].Value != DBNull.Value)
                {
                    TotalCount = (int)cmd.Parameters["@Total"].Value;
                }
                else
                {
                    TotalCount = 0;
                }
                return retVal;
            }
        }
        //public override bool InsertAppointment(PatientAppointment appointment, out long AppointmentID)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spInsertAppointment", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(appointment.StaffID));
        //        cmd.AddParameter("@V_ApptStatus", SqlDbType.BigInt, appointment.ApptStatus != null ? appointment.ApptStatus.LookupID : (long)appointment.V_ApptStatus);
        //        cmd.AddParameter("@RecDateCreated", SqlDbType.DateTime, appointment.RecDateCreated);
        //        cmd.AddParameter("@ApptDate", SqlDbType.DateTime, appointment.ApptDate);
        //        cmd.AddParameter("@AppointmentDetails_Add", SqlDbType.Xml, ConvertNullObjectToDBNull(appointment.ConvertDetailsListToXml()));
        //        cmd.AddParameter("@AppointmentID", SqlDbType.BigInt, 16, ParameterDirection.Output);

        //        cn.Open();

        //        int retVal = ExecuteNonQuery(cmd);

        //        AppointmentID = (long)cmd.Parameters["@AppointmentID"].Value;

        //        return retVal > 0;
        //    }
        //}

        //public override bool UpdateAppointment(PatientAppointment appointment)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spUpdateAppointment", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        cmd.AddParameter("@AppointmentID", SqlDbType.BigInt, appointment.AppointmentID);
        //        cmd.AddParameter("@V_ApptStatus", SqlDbType.BigInt, appointment.ApptStatus != null ? appointment.ApptStatus.LookupID : (long)appointment.V_ApptStatus);
        //        cmd.AddParameter("@DateModified", SqlDbType.DateTime, appointment.DateModified);
        //        cmd.AddParameter("@HasChronicDisease", SqlDbType.Bit, ConvertNullObjectToDBNull(appointment.HasChronicDisease));

        //        cmd.AddParameter("@RetVal", SqlDbType.Bit, false, ParameterDirection.ReturnValue);
        //        cn.Open();

        //        ExecuteNonQuery(cmd);
        //        int RetVal = (int)cmd.Parameters["@RetVal"].Value;

        //        return RetVal > 0;
        //    }
        //}

        public override List<PatientApptTimeSegment> GetAllAppointmentSegments()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllAppointmentSegments", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                List<PatientApptTimeSegment> retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                retVal = GetAppointmentSegmentCollectionFromReader(reader);
                return retVal;
            }
        }
        public override List<RefMedicalServiceItem> GetAllServicesByAppointmentType(long appointmentType)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllServicesByAppointmentType", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@V_AppointmentType", SqlDbType.BigInt, appointmentType);

                cn.Open();
                List<RefMedicalServiceItem> retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                retVal = GetMedicalServiceItemCollectionFromReader(reader);
                return retVal;
            }
        }

        // TxD 31/07/2014 Commented out GetAllLocationsByService because it's not USED
        //public override List<Location> GetAllLocationsByService(long serviceID)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spGetAllLocationsByService", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, serviceID);

        //        cn.Open();
        //        List<Location> retVal = null;
        //        IDataReader reader = ExecuteReader(cmd);
        //        retVal = GetLocationCollectionFromReader(reader);
        //        return retVal;
        //    }
        //}

        public override PatientAppointment GetAppointmentByID(long appointmentID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAppointmentByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@AppointmentID", SqlDbType.BigInt, appointmentID);

                cn.Open();
                PatientAppointment retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                if (reader == null)
                {
                    return null;
                }
                if (reader.Read())
                {
                    retVal = GetAppointmentFromReader(reader);
                    retVal.PatientApptServiceDetailList = new List<PatientApptServiceDetails>().ToObservableCollection();
                    retVal.PatientApptPCLRequestsList = new List<PatientApptPCLRequests>().ToObservableCollection();
                }
                if (reader.NextResult())
                {
                    PatientApptServiceDetails details;
                    while (reader.Read())
                    {
                        details = GetAppointmentDetailsFromReader(reader);
                        details.DeptLocation = GetDeptLocationFromReader(reader);
                        details.MedService = GetMedicalServiceItemFromReader(reader);
                        details.ApptTimeSegment = GetConsultationTimeSegmentsFromReader(reader);
                        details.AppointmentType = GetLookupFromReader(reader);

                        var listDept = GetAllDeptLocationsByService(details.MedService.MedServiceID);
                        details.DeptLocationList = new ObservableCollection<DeptLocation>(listDept);

                        var listSegment = ClinicManagementProvider.instance.ConsultationTimeSegments_ByDeptLocationID(details.DeptLocationID);

                        details.ApptTimeSegmentList = new ObservableCollection<ConsultationTimeSegments>(listSegment);

                        retVal.PatientApptServiceDetailList.Add(details);
                    }
                }

                if (reader.NextResult())
                {
                    PatientApptPCLRequests reqPCL;
                    while (reader.Read())
                    {
                        reqPCL = GetPatientApptPCLRequestsFromReader(reader);
                        //reqPCL.DeptLocation = GetDeptLocationFromReader(reader);
                        //reqPCL.ApptTimeSegment = GetAppointmentSegmentFromReader(reader);

                        reqPCL.ObjPatientApptPCLRequestDetailsList = new ObservableCollection<PatientApptPCLRequestDetails>(PatientApptPCLRequestDetails_ByID(appointmentID, reqPCL.PatientPCLReqID));


                        foreach (var items in reqPCL.ObjPatientApptPCLRequestDetailsList)
                        {
                            var deptlocPCL = ConfigurationManagerProviders.Instance.ListDeptLocation_ByPCLExamTypeID(items.ObjPCLExamTypes.PCLExamTypeID);
                            items.ObjPCLExamTypes.ObjDeptLocationList = new ObservableCollection<DeptLocation>(deptlocPCL);
                        }

                        retVal.PatientApptPCLRequestsList.Add(reqPCL);

                    }
                }
                return retVal;
            }
        }

        //Danh sách phòng 


        public override short CreateApptSeqNumber(long PatientID, DateTime ApptDate, long DepartmentLocID, short ApptTimeSegmentID)
        {
            //Enter Critical Section.
            lock (this)
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spCreateApptSeqNumber", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@ApptDate", SqlDbType.DateTime, ApptDate);
                    cmd.AddParameter("@DepartmentLocID", SqlDbType.BigInt, DepartmentLocID);
                    cmd.AddParameter("@ApptTimeSegmentID", SqlDbType.SmallInt, ApptTimeSegmentID);
                    cmd.AddParameter("@PatientID", SqlDbType.BigInt, PatientID);

                    cmd.AddParameter("@SeqNumber", SqlDbType.SmallInt, DBNull.Value, ParameterDirection.Output);

                    cn.Open();
                    ExecuteNonQuery(cmd);
                    object o = cmd.Parameters["@SeqNumber"].Value;
                    if (o != DBNull.Value)
                    {
                        return (short)o;
                    }
                    return -1;
                }
            }
        }
        public override List<DeptLocation> GetAllDeptLocationsByService(long serviceID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllDeptLocationsByService", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, serviceID);

                cn.Open();
                List<DeptLocation> retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                retVal = GetDeptLocationCollectionFromReader(reader);
                return retVal;
            }
        }


        public override List<DeptLocation> GetAllDeptLocationsByService_WithSeqNumberSegment(long MedServiceID, DateTime ApptDate)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllDeptLocationsByService_WithSeqNumberSegment", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(MedServiceID));
                cmd.AddParameter("@ApptDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(ApptDate));

                cn.Open();
                List<DeptLocation> retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                retVal = GetDeptLocationCollectionFromReader(reader);
                return retVal;
            }
        }


        public override List<ConsultationTimeSegments> Segments_WithAppDateDeptLocIDSeqNumber(long DeptLocationID, DateTime ApptDate)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSegments_WithAppDateDeptLocIDSeqNumber", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));
                cmd.AddParameter("@ApptDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(ApptDate));

                cn.Open();
                List<ConsultationTimeSegments> retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                retVal = GetConsultationTimeSegmentsCheckDayOfWeekCollectionFromReader(ApptDate, reader);
                return retVal;
            }
        }


        public override bool CreateAppointment(PatientAppointment appointment, out long AppointmentID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spCreateAppointment", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(appointment.StaffID));
                cmd.AddParameter("@V_ApptStatus", SqlDbType.BigInt, appointment.V_ApptStatus);
                cmd.AddParameter("@RecDateCreated", SqlDbType.DateTime, appointment.RecDateCreated);
                cmd.AddParameter("@ApptDate", SqlDbType.DateTime, appointment.ApptDate);
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, appointment.PatientID);
                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(appointment.DoctorStaffID));
                cmd.AddParameter("@ServiceRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(appointment.ServiceRecID));
                cmd.AddParameter("@NDay", SqlDbType.Int, ConvertNullObjectToDBNull(appointment.NDay));
                cmd.AddParameter("@HasChronicDisease", SqlDbType.Bit, ConvertNullObjectToDBNull(appointment.HasChronicDisease));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(appointment.PtRegistrationID));
                cmd.AddParameter("@AppointmentID", SqlDbType.BigInt, 16, ParameterDirection.Output);

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);

                AppointmentID = (long)cmd.Parameters["@AppointmentID"].Value;

                return true;
            }
        }

        public override bool InsertAppointmentDetails(PatientApptServiceDetails apptDetails, out long ApptDetailsID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spInsertAppointmentDetails", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@AppointmentID", SqlDbType.BigInt, apptDetails.AppointmentID);
                cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, apptDetails.MedService.MedServiceID);
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, apptDetails.DeptLocation.DeptLocationID);
                cmd.AddParameter("@ApptTimeSegmentID", SqlDbType.SmallInt, apptDetails.ApptTimeSegment.ConsultationTimeSegmentID);

                //cmd.AddParameter("@StartTime", SqlDbType.DateTime, apptDetails.StartTime);
                //cmd.AddParameter("@EndTime", SqlDbType.DateTime, apptDetails.EndTime);

                cmd.AddParameter("@ServiceSeqNum", SqlDbType.SmallInt, apptDetails.ServiceSeqNum);
                cmd.AddParameter("@ServiceSeqNumType", SqlDbType.SmallInt, apptDetails.ServiceSeqNumType);

                cmd.AddParameter("@V_AppointmentType", SqlDbType.BigInt, apptDetails.AppointmentType.LookupID);

                cmd.AddParameter("@ApptSvcDetailID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);

                ApptDetailsID = (long)cmd.Parameters["@ApptSvcDetailID"].Value;

                return true;
            }
        }

        public override bool DeletePatientAppointments(long AppointmentID)
        {
            return DeletePatientAppointments_Ext(AppointmentID, null);
        }

        public override bool DeletePatientAppointments_Ext(long AppointmentID, long? NewAppointmentID)
        {
            string Result = "";

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientAppointmentsDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@AppointmentID", SqlDbType.BigInt, AppointmentID);
                cmd.AddParameter("@NewAppointmentID", SqlDbType.BigInt, ConvertNullObjectToDBNull(NewAppointmentID));
                cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                {
                    Result = cmd.Parameters["@Result"].Value.ToString();
                }

                if (Result == "OK")
                    return true;
                return false;
            }
        }
        public override bool DeleteAppointmentDetails(PatientApptServiceDetails apptDetails)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeleteAppointmentDetails", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@ApptSvcDetailID", SqlDbType.BigInt, apptDetails.ApptSvcDetailID);

                cmd.AddParameter("@RetVal", SqlDbType.Bit, false, ParameterDirection.ReturnValue);

                cn.Open();

                ExecuteNonQuery(cmd);

                int RetVal = (int)cmd.Parameters["@RetVal"].Value;

                return RetVal > 0;
            }
        }
        /// <summary>
        /// Tra so lai cho he thong.
        /// </summary>
        /// <param name="ApptDate"></param>
        /// <param name="DepartmentLocID"></param>
        /// <param name="ApptTimeSegmentID"></param>
        /// <param name="SeqNumber"></param>
        /// <returns></returns>
        public override bool ReturnApptSeqNumber(DateTime ApptDate, long DepartmentLocID, short ApptTimeSegmentID, short SeqNumber)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spReturnApptSeqNumber", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@ApptDate", SqlDbType.DateTime, ApptDate);
                cmd.AddParameter("@SeqNumber", SqlDbType.SmallInt, SeqNumber);

                cmd.AddParameter("@DepartmentLocID", SqlDbType.BigInt, DepartmentLocID);
                cmd.AddParameter("@ApptTimeSegmentID", SqlDbType.SmallInt, ApptTimeSegmentID);

                cn.Open();

                ExecuteNonQuery(cmd);

                return true;
            }
        }
        /// <summary>
        /// Lay cuoc hen cua benh nhan trong ngay. Trong moi ngay chi co toi da 1 cuoc hen.
        /// </summary>
        /// <param name="patientID"></param>
        /// <param name="ApptDate"></param>
        /// <returns></returns>
        public override PatientAppointment GetAppointmentOfPatientByDate(long patientID, DateTime ApptDate)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spCheckIfAppointmentExists", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientID", SqlDbType.BigInt, patientID);
                cmd.AddParameter("@ApptDate", SqlDbType.DateTime, ApptDate);
                cmd.AddParameter("@AppointmentID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                //Kiem tra xem co cuoc hen chua. Neu chua co thi tra ve null.
                ExecuteNonQuery(cmd);
                object o = cmd.Parameters["@AppointmentID"].Value;
                if (o == DBNull.Value || (long)o <= 0)
                {
                    return null;
                }
                //Neu co thi tra ve cuoc hen nay (day du chi tiet)
                cmd.CommandText = "spGetAppointmentByID";
                cmd.Parameters.RemoveAt(0);
                cmd.Parameters.RemoveAt(0);
                cmd.Parameters["@AppointmentID"].Direction = ParameterDirection.Input;
                PatientAppointment retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                if (reader == null)
                {
                    return null;
                }
                if (reader.Read())
                {
                    retVal = GetAppointmentFromReader(reader);
                    retVal.PatientApptServiceDetailList = new List<PatientApptServiceDetails>().ToObservableCollection();
                }
                if (reader.NextResult())
                {
                    PatientApptServiceDetails details;
                    while (reader.Read())
                    {
                        details = GetAppointmentDetailsFromReader(reader);
                        details.DeptLocation = GetDeptLocationFromReader(reader);
                        details.MedService = GetMedicalServiceItemFromReader(reader);
                        details.ApptTimeSegment = GetConsultationTimeSegmentsFromReader(reader);
                        details.AppointmentType = GetLookupFromReader(reader);

                        retVal.PatientApptServiceDetailList.Add(details);
                    }
                }
                return retVal;
            }
        }
        public override int GetNumberOfAppointments(DateTime ApptDate, long DeptLocID, short segmentID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetNumberOfAppointmentDetails", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@ApptDate", SqlDbType.DateTime, ApptDate);
                cmd.AddParameter("@DepartmentLocID", SqlDbType.BigInt, DeptLocID);
                cmd.AddParameter("@ApptTimeSegmentID", SqlDbType.SmallInt, segmentID);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                ExecuteNonQuery(cmd);
                int Total = 0;
                if (cmd.Parameters["@Total"].Value != DBNull.Value)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                return Total;
            }
        }
        public override void GetNumberOfAvailablePosition(DateTime ApptDate, long DeptLocID, short segmentID, out int MaxNumOfAppointments, out int NumOfAppts)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetNumberOfAvailablePosition", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@ApptDate", SqlDbType.DateTime, ApptDate);
                cmd.AddParameter("@DepartmentLocID", SqlDbType.BigInt, DeptLocID);
                cmd.AddParameter("@ApptTimeSegmentID", SqlDbType.SmallInt, segmentID);
                cmd.AddParameter("@Total", SqlDbType.SmallInt, DBNull.Value, ParameterDirection.Output);
                cmd.AddParameter("@NumOfAppt", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Total"].Value != DBNull.Value)
                {
                    MaxNumOfAppointments = (int)(short)cmd.Parameters["@Total"].Value;
                }
                else
                {
                    MaxNumOfAppointments = 0;
                }
                if (cmd.Parameters["@NumOfAppt"].Value != DBNull.Value)
                {
                    NumOfAppts = (int)cmd.Parameters["@NumOfAppt"].Value;
                }
                else
                {
                    NumOfAppts = 0;
                }
            }
        }


        public override List<DeptLocation> AllDeptLocation_LAB()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spAllDeptLocation_LAB", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                //cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, serviceID);

                cn.Open();
                List<DeptLocation> retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                retVal = GetDeptLocationCollectionFromReader(reader);
                return retVal;
            }
        }

        public override List<DeptLocation> AllDeptLocation_NotLAB(long V_PCLMainCategory, long PCLExamTypeSubCategoryID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spAllDeptLocation_NotLAB", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, V_PCLMainCategory);
                cmd.AddParameter("@PCLExamTypeSubCategoryID", SqlDbType.BigInt, PCLExamTypeSubCategoryID);

                cn.Open();
                List<DeptLocation> retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                retVal = GetDeptLocationCollectionFromReader(reader);
                return retVal;
            }
        }

        public override List<PatientApptPCLRequestDetails> PatientApptPCLRequestDetails_ByID(long AppointmentID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientApptPCLRequestDetails_ByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@AppointmentID", SqlDbType.BigInt, AppointmentID);
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, PatientPCLReqID);

                cn.Open();
                List<PatientApptPCLRequestDetails> retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                retVal = GetPatientApptPCLRequestDetailsCollectionFromReader(reader);
                return retVal;
            }
        }


        #region Hẹn Bệnh

        private void CopyPatientApptPCLRequests(ref PatientApptPCLRequests target, PatientApptPCLRequests source)
        {
            target.PatientPCLReqID = source.PatientPCLReqID;
            target.AppointmentID = source.AppointmentID;
            target.PCLRequestNumID = source.PCLRequestNumID;
            target.StaffID = source.StaffID;
            target.ReqFromDeptLocID = source.ReqFromDeptLocID;
            target.Diagnosis = source.Diagnosis;
            target.ICD10List = source.ICD10List;
            target.DoctorComments = source.DoctorComments;
            target.ApptPCLNote = source.ApptPCLNote;
            target.V_PCLMainCategory = source.V_PCLMainCategory;

        }

        private void TachDetail(PatientApptPCLRequests p, PatientApptPCLRequestDetails item, PatientApptPCLRequestDetails detail, out bool exists)
        {
            exists = false;
            if (item != null && detail.ObjPCLExamTypes.HITTypeID == item.ObjPCLExamTypes.HITTypeID && detail.ObjPCLExamTypes.V_PCLMainCategory == item.ObjPCLExamTypes.V_PCLMainCategory)
            {
                // neu da duoc chon phong thi so sanh phong
                if (detail.ObjDeptLocID != null && detail.ObjDeptLocID.DeptLocationID > 0)
                {
                    //,giong nhau thi cung 1 phieu
                    if (item.ObjDeptLocID != null && item.ObjDeptLocID.DeptLocationID == detail.ObjDeptLocID.DeptLocationID)
                    {
                        p.ObjPatientApptPCLRequestDetailsList.Add(detail);
                        exists = true;
                    }
                    else
                    {
                        //neu dv nay chua dc gan phong thi lay danh sach phong ra so sanh
                        //neu chua phong dc gan o tren thi cho vao 1 phong luon
                        if (item.ObjDeptLocIDList != null && item.ObjDeptLocIDList.Count > 0)
                        {
                            foreach (var item1 in item.ObjDeptLocIDList)
                            {
                                if (detail.ObjDeptLocID.DeptLocationID == item1.DeptLocationID)
                                {
                                    p.ObjPatientApptPCLRequestDetailsList.Add(detail);
                                    exists = true;
                                    break;
                                }
                            }
                        }

                    }
                }

                //neu tat ca deu khong duoc gan phong thi di so sanh danh sach phong
                else if (item.ObjDeptLocIDList != null && item.ObjDeptLocIDList.Count > 0 && detail.ObjDeptLocIDList != null && detail.ObjDeptLocIDList.Count > 0)
                {
                    foreach (var item1 in item.ObjDeptLocIDList)
                    {
                        foreach (var detail1 in detail.ObjDeptLocIDList)
                        {
                            if (detail1.DeptLocationID == item1.DeptLocationID)
                            {
                                //da ton tai
                                p.ObjPatientApptPCLRequestDetailsList.Add(detail);
                                exists = true;
                                break;
                            }
                        }
                        if (exists)
                        {
                            break;
                        }
                    }
                }
            }

        }

        private void TachAppointment(PatientAppointment ObjPatientAppointment
            , out List<PatientApptPCLRequests> lstPCLFinal
            , out List<PatientApptPCLRequests> lstPCLDelete)
        {
            lstPCLFinal = new List<PatientApptPCLRequests>();
            lstPCLDelete = new List<PatientApptPCLRequests>();
            List<PatientApptPCLRequests> lstPCLFinaltemp = new List<PatientApptPCLRequests>();
                        
            foreach (var lstPCLnew in ObjPatientAppointment.PatientApptPCLRequestsList)
            {
                //neu la phieu cap nhat
                //phai lay chi tiet cu da duoc luu ra so sanh luon
                //voi nhung phieu da co roi se lay nhung item da dc insert roi
                var lstDetailexists = lstPCLnew.ObjPatientApptPCLRequestDetailsList.Where(x => x.PCLReqItemID > 0);

                //lay nhung item moi can dc them vao
                var lstDetailNew = lstPCLnew.ObjPatientApptPCLRequestDetailsList.Where(x => x.EntityState == EntityState.DETACHED
                    || x.EntityState == EntityState.NEW);
                    //|| x.EntityState == EntityState.DELETED_MODIFIED);
                var lstDetailDelete = lstPCLnew.ObjPatientApptPCLRequestDetailsList.Where(x => x.EntityState == EntityState.DETACHED
                    || x.EntityState == EntityState.DELETED_MODIFIED);
                if (lstDetailDelete != null && lstDetailDelete.Count()>0)
                {
                    PatientApptPCLRequests p = new PatientApptPCLRequests();
                    p.ObjPatientApptPCLRequestDetailsList = new ObservableCollection<PatientApptPCLRequestDetails>();
                    foreach (var detail in lstDetailDelete)
                    {
                        CopyPatientApptPCLRequests(ref p, lstPCLnew);
                        //p = lstPCLnew.EntityDeepCopy();
                        p.ObjPatientApptPCLRequestDetailsList.Add(detail);
                    }
                    lstPCLDelete.Add(p);
                }

                if (lstDetailNew == null || lstDetailNew.Count() <= 0)
                {
                    //khong co item moi thi khong lam j het
                    continue;
                }

                bool exists = false;
                bool existolds = false;
                foreach (var detail in lstDetailNew)
                {
                    //lay danh sach phong
                    //kiem cai j do de chua list phong

                    detail.ObjDeptLocIDList = MAPPCLExamTypeDeptLoc[detail.ObjPCLExamTypes.PCLExamTypeID].ObjDeptLocationList;

                    exists = false;
                    existolds = false;

                    if (lstPCLFinaltemp.Count <= 0)
                    {
                        PatientApptPCLRequests p = new PatientApptPCLRequests();
                        p.ObjPatientApptPCLRequestDetailsList = new ObservableCollection<PatientApptPCLRequestDetails>();
                        CopyPatientApptPCLRequests(ref p, lstPCLnew);
                        //p = lstPCLnew.EntityDeepCopy();
                        PatientApptPCLRequestDetails item = new PatientApptPCLRequestDetails();

                        if (lstDetailexists != null && lstDetailexists.Count() > 0)
                        {
                            item = lstDetailexists != null ? lstDetailexists.FirstOrDefault() : null;
                            item.ObjDeptLocIDList = MAPPCLExamTypeDeptLoc[item.ObjPCLExamTypes.PCLExamTypeID].ObjDeptLocationList;
                            //thuc ra o day khong can P
                            TachDetail(p, item, detail, out existolds);
                            //neu co ton tai thi add moi nhung lay so phieu cu
                            if (existolds)
                            {
                                //tao phieu moi roi add vao
                                //o day muc dich la lay ma phieu cu,de add vao chung 1 phieu
                                lstPCLFinaltemp.Add(p);
                                continue;
                            }
                            else
                            {
                                p.ObjPatientApptPCLRequestDetailsList.Add(detail);
                                p.PatientPCLReqID = 0;
                                lstPCLFinaltemp.Add(p);
                                continue;
                            }
                        }
                        else
                        {
                            p.ObjPatientApptPCLRequestDetailsList.Add(detail);
                            p.PatientPCLReqID = 0;
                            lstPCLFinaltemp.Add(p);
                            continue;
                        }
                    }
                    else
                    {
                        foreach (var p in lstPCLFinaltemp)
                        {
                            PatientApptPCLRequestDetails item = new PatientApptPCLRequestDetails();
                            //neu co detail cu,duyet qua detail cu
                            //
                            if (lstDetailexists != null && lstDetailexists.Count() > 0)
                            {
                                item = lstDetailexists != null ? lstDetailexists.FirstOrDefault() : null;
                                item.ObjDeptLocIDList = MAPPCLExamTypeDeptLoc[item.ObjPCLExamTypes.PCLExamTypeID].ObjDeptLocationList;
                                TachDetail(p, item, detail, out existolds);
                                //neu co ton tai thi add moi nhung lay so phieu cu
                                if (existolds)
                                {
                                    //tao phieu moi roi add vao
                                    exists = true;
                                    //PatientApptPCLRequests newRequest = new PatientApptPCLRequests();
                                    //CopyPatientApptPCLRequests(ref newRequest, lstPCLnew);
                                    //if (newRequest.ObjPatientApptPCLRequestDetailsList == null)
                                    //{
                                    //    newRequest.ObjPatientApptPCLRequestDetailsList = new ObservableCollection<PatientApptPCLRequestDetails>();
                                    //}
                                    //newRequest.ObjPatientApptPCLRequestDetailsList.Add(detail);
                                    //lstPCLFinaltemp.Add(newRequest);
                                    break;
                                }
                            }
                            if (!existolds)
                            {
                                item = p.ObjPatientApptPCLRequestDetailsList != null ? p.ObjPatientApptPCLRequestDetailsList.FirstOrDefault() : null;
                                TachDetail(p, item, detail, out exists);
                                if (exists)
                                {
                                    break;
                                }
                            }
                        }


                        if (!exists)
                        {
                            //tao phieu moi roi add vao
                            PatientApptPCLRequests newRequest = new PatientApptPCLRequests();
                            CopyPatientApptPCLRequests(ref newRequest, lstPCLnew);
                            //newRequest = lstPCLnew.EntityDeepCopy();
                            newRequest.PatientPCLReqID = 0;
                            if (newRequest.ObjPatientApptPCLRequestDetailsList == null)
                            {
                                newRequest.ObjPatientApptPCLRequestDetailsList = new ObservableCollection<PatientApptPCLRequestDetails>();
                            }
                            newRequest.ObjPatientApptPCLRequestDetailsList.Add(detail);
                            lstPCLFinaltemp.Add(newRequest);
                        }
                    }
                }

                lstPCLFinal.AddRange(lstPCLFinaltemp);
                lstPCLFinaltemp.Clear();
            }
        }

        private void TachAppointmentDeleteAndCreateNew(PatientAppointment ObjPatientAppointment, out List<PatientApptPCLRequests> lstPCLFinal)
        {
            lstPCLFinal = new List<PatientApptPCLRequests>();
            List<PatientApptPCLRequests> lstPCLFinaltemp = new List<PatientApptPCLRequests>();
            //Dictionary<long, PCLExamType> MAPPCLExamTypeDeptLoc = ServiceCollectioneHCMS.MAPPCLExamTypeDeptLoc;

            foreach (var lstPCLnew in ObjPatientAppointment.PatientApptPCLRequestsList)
            {
                lstPCLnew.PatientPCLReqID = 0;
                if (lstPCLnew.EntityState == EntityState.DELETED_MODIFIED || lstPCLnew.EntityState == EntityState.DELETED_PERSITED)
                {
                    continue;
                }
                //lay nhung item moi can dc them vao
                var lstDetailNew = lstPCLnew.ObjPatientApptPCLRequestDetailsList.Where(x=>x.EntityState !=EntityState.DELETED_PERSITED && x.EntityState !=EntityState.DELETED_MODIFIED);

                if (lstDetailNew == null || lstDetailNew.Count() <= 0)
                {
                    //khong co item moi thi khong lam j het
                    continue;
                }

                bool exists = false;
                foreach (var detail in lstDetailNew)
                {
                    //lay danh sach phong
                    //kiem cai j do de chua list phong

                    detail.ObjDeptLocIDList = MAPPCLExamTypeDeptLoc[detail.ObjPCLExamTypes.PCLExamTypeID].ObjDeptLocationList;

                    exists = false;

                    if (lstPCLFinaltemp.Count <= 0)
                    {
                        PatientApptPCLRequests p = new PatientApptPCLRequests();
                        p.ObjPatientApptPCLRequestDetailsList = new ObservableCollection<PatientApptPCLRequestDetails>();
                        CopyPatientApptPCLRequests(ref p, lstPCLnew);
                        //p = lstPCLnew.EntityDeepCopy();
                        PatientApptPCLRequestDetails item = new PatientApptPCLRequestDetails();

                        p.ObjPatientApptPCLRequestDetailsList.Add(detail);
                        p.PatientPCLReqID = 0;
                        lstPCLFinaltemp.Add(p);
                        continue;
                    }
                    else
                    {
                        foreach (var p in lstPCLFinaltemp)
                        {
                            PatientApptPCLRequestDetails item = new PatientApptPCLRequestDetails();
                            item = p.ObjPatientApptPCLRequestDetailsList != null ? p.ObjPatientApptPCLRequestDetailsList.FirstOrDefault() : null;
                            TachDetail(p, item, detail, out exists);
                            if (exists)
                            {
                                break;
                            }
                        }
                        if (!exists)
                        {
                            //tao phieu moi roi add vao
                            PatientApptPCLRequests newRequest = new PatientApptPCLRequests();
                            CopyPatientApptPCLRequests(ref newRequest, lstPCLnew);
                            //newRequest = lstPCLnew.EntityDeepCopy();
                            newRequest.PatientPCLReqID = 0;
                            if (newRequest.ObjPatientApptPCLRequestDetailsList == null)
                            {
                                newRequest.ObjPatientApptPCLRequestDetailsList = new ObservableCollection<PatientApptPCLRequestDetails>();
                            }
                            newRequest.ObjPatientApptPCLRequestDetailsList.Add(detail);
                            lstPCLFinaltemp.Add(newRequest);
                        }
                    }
                }

                lstPCLFinal.AddRange(lstPCLFinaltemp);
                lstPCLFinaltemp.Clear();
            }
        }

        public override bool PatientAppointments_Save(PatientAppointment ObjPatientAppointment,
            bool PassCheckFullTarget,
            out long AppointmentID,
            out string ErrorDetail, out string ListNotConfig, out string ListTargetFull, out string ListMax, out string ListRequestID)
        {
            try
            {
                bool Res = false;

                ListNotConfig = "";
                ListTargetFull = "";
                ListMax = "";
                ListRequestID = "";

                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {

                    //Kiểm có hẹn detail rồi, ngày hẹn thì bị thay đổi
                    bool HasAppointmentDetailApptDateChange = false;

                    SqlCommand cmd = new SqlCommand("spCheckHasAppointmentDetailApptDateChange", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@AppointmentID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ObjPatientAppointment.AppointmentID));
                    cmd.AddParameter("@ApptDate_New", SqlDbType.DateTime, ConvertNullObjectToDBNull(ObjPatientAppointment.ApptDate));

                    cmd.AddParameter("@Result", SqlDbType.Bit, DBNull.Value, ParameterDirection.Output);

                    cn.Open();

                    ExecuteNonQuery(cmd);

                    if (cmd.Parameters["@Result"].Value != null)
                        HasAppointmentDetailApptDateChange = Convert.ToBoolean(cmd.Parameters["@Result"].Value);

                    if (HasAppointmentDetailApptDateChange == false)
                    {
                        //tach phieu o day di
                        if (ObjPatientAppointment.PatientApptPCLRequestsList != null)
                        {
                            //voi nhung phieu da co roi
                            List<PatientApptPCLRequests> lstPCLFinal = null;
                            List<PatientApptPCLRequests> lstPCLDelete = null;
                            TachAppointment(ObjPatientAppointment, out lstPCLFinal, out lstPCLDelete);

                            ObjPatientAppointment.ObjApptPCLRequestsList_Add = lstPCLFinal.ToObservableCollection();
                            if (lstPCLDelete != null && lstPCLDelete.Count > 0)
                            {
                                if (ObjPatientAppointment.ObjApptPCLRequestsList_Delete == null)
                                {
                                    ObjPatientAppointment.ObjApptPCLRequestsList_Delete = new ObservableCollection<PatientApptPCLRequests>();
                                }
                                foreach (var item in lstPCLDelete)
                                {
                                    ObjPatientAppointment.ObjApptPCLRequestsList_Delete.Add(item);
                                }

                            }

                        }

                        Res = PatientAppointments_CUD(ObjPatientAppointment, PassCheckFullTarget, out AppointmentID, out ErrorDetail, out ListRequestID);
                    }
                    else/*Xóa rồi tạo lại*/
                    {
                        Res = PatientAppointments_DeleteAndCreate(ObjPatientAppointment, PassCheckFullTarget, out AppointmentID, out ErrorDetail, out ListRequestID);
                    }

                    if (!string.IsNullOrEmpty(ErrorDetail))
                    {
                        DataTable dt = ParseXML(ErrorDetail);

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            IList<PatientApptServiceDetails> PatientApptServiceDetailsError = new List<PatientApptServiceDetails>();

                            foreach (DataRow row in dt.Rows)
                            {
                                PatientApptServiceDetails o = new PatientApptServiceDetails();
                                o.MedServiceID = Convert.ToInt64(row["MedServiceID"]);
                                o.DeptLocationID = Convert.ToInt64(row["DeptLocationID"]);
                                o.ApptTimeSegmentID = short.Parse(row["ApptTimeSegmentID"].ToString());
                                o.Result = row["Result"].ToString();
                                PatientApptServiceDetailsError.Add( o);
                            }

                            //Gom nhóm
                            var listFull = (from c in PatientApptServiceDetailsError
                                            where c.Result == "TargetFull"
                                            select c);

                            var listNotConfig = (from c in PatientApptServiceDetailsError
                                                 where c.Result == "NotConfig"
                                                 select c);

                            var listMax = (from c in PatientApptServiceDetailsError
                                           where c.Result == "Max"
                                           select c);

                            if (listNotConfig != null && listNotConfig.Count() > 0)
                            {
                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine("+ Các mục chưa cấu hình Số Hẹn:");
                                foreach (var item in listNotConfig)
                                {
                                    string str = GetTextMedIDDeptLocIDIDSegmentID(item.MedServiceID, item.DeptLocationID, item.ApptTimeSegmentID);
                                    sb.AppendLine("-" + str);
                                }
                                ListNotConfig = sb.ToString();
                            }


                            if (listFull != null && listFull.Count() > 0)
                            {
                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine("+ Các mục vượt chỉ tiêu:");
                                foreach (var item in listFull)
                                {
                                    string str = GetTextMedIDDeptLocIDIDSegmentID(item.MedServiceID, item.DeptLocationID, item.ApptTimeSegmentID);
                                    sb.AppendLine("-" + str);
                                }
                                ListTargetFull = sb.ToString();
                            }


                            if (listMax != null && listMax.Count() > 0)
                            {
                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine("+ Các mục Hẹn đã đầy:");
                                foreach (var item in listMax)
                                {
                                    string str = GetTextMedIDDeptLocIDIDSegmentID(item.MedServiceID, item.DeptLocationID, item.ApptTimeSegmentID);
                                    sb.AppendLine("-" + str);
                                }
                                ListMax = sb.ToString();
                            }

                        }
                    }

                    return Res;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public string GetTextMedIDDeptLocIDIDSegmentID(
            long MedServiceID,
            long DeptLocationID,
            long ApptTimeSegmentID
            )
        {
            string Result = "";

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spGetTextMedIDDeptLocIDIDSegmentID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, MedServiceID);
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, DeptLocationID);
                cmd.AddParameter("@ApptTimeSegmentID", SqlDbType.BigInt, ApptTimeSegmentID);

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
            }
            return Result;
        }

        private DataTable ParseXML(string xmlString)
        {
            try
            {
                DataSet ds = new DataSet();
                byte[] xmlBytes = Encoding.UTF8.GetBytes(xmlString);
                Stream memory = new MemoryStream(xmlBytes);
                ds.ReadXml(memory);
                return ds.Tables[0];
            }
            catch (Exception)
            {
                return null;
            }
        }


        private bool PatientAppointments_CUD(PatientAppointment ObjPatientAppointment,
           bool PassCheckFullTarget,
           out long AppointmentID,
           out string ErrorDetail,
           out string ListRequestID)
        {   
            try
            {
                ErrorDetail = "";
                ListRequestID = "";
                AppointmentID = 0;
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spPatientAppointments_Save", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (ObjPatientAppointment.RecDateCreated == DateTime.MinValue)
                    {
                        ObjPatientAppointment.RecDateCreated = DateTime.Now;
                    }

                    cmd.AddParameter("@AppointmentID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ObjPatientAppointment.AppointmentID));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ObjPatientAppointment.StaffID));
                    cmd.AddParameter("@V_ApptStatus", SqlDbType.BigInt, ObjPatientAppointment.V_ApptStatus);
                    cmd.AddParameter("@RecDateCreated", SqlDbType.DateTime, ObjPatientAppointment.RecDateCreated);
                    cmd.AddParameter("@ApptDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(ObjPatientAppointment.ApptDate));
                    cmd.AddParameter("@PatientID", SqlDbType.BigInt, ObjPatientAppointment.PatientID);
                    cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ObjPatientAppointment.DoctorStaffID));
                    cmd.AddParameter("@ServiceRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ObjPatientAppointment.ServiceRecID));
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ObjPatientAppointment.PtRegistrationID));
                    cmd.AddParameter("@NDay", SqlDbType.Int, ConvertNullObjectToDBNull(ObjPatientAppointment.NDay));
                    cmd.AddParameter("@AllowPaperReferralUseNextConsult", SqlDbType.Bit, ConvertNullObjectToDBNull(ObjPatientAppointment.AllowPaperReferralUseNextConsult));
                    cmd.AddParameter("@HasChronicDisease", SqlDbType.Bit, ConvertNullObjectToDBNull(ObjPatientAppointment.HasChronicDisease));

                    cmd.AddParameter("@CreatedByInPtRegis", SqlDbType.Bit, ConvertNullObjectToDBNull(ObjPatientAppointment.CreatedByInPtRegis));

                    cmd.AddParameter("@ReasonToAllowPaperReferral", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ObjPatientAppointment.ReasonToAllowPaperReferral));
                    //HPT_20160708
                    cmd.AddParameter("@IsEmergInPtReExamApp", SqlDbType.Bit, ConvertNullObjectToDBNull(ObjPatientAppointment.IsEmergInPtReExamApp));
                    bool bErrorSerA = false;
                    string ErrorDetailSerA = "";

                    bool bErrorSerU = false;
                    string ErrorDetailSerU = "";

                    bool bErrorSerD = false;
                    string ErrorDetailSerD = "";

                    cmd.AddParameter("@xmlPatientApptServiceDetails_Add", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertServiceDetailsToXml(ObjPatientAppointment.PatientID, true, ObjPatientAppointment.ApptDate.Value, ObjPatientAppointment.ObjApptServiceDetailsList_Add, out bErrorSerA, out ErrorDetailSerA)));
                    cmd.AddParameter("@xmlPatientApptServiceDetails_Update", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertServiceDetailsToXml(ObjPatientAppointment.PatientID, false, ObjPatientAppointment.ApptDate.Value, ObjPatientAppointment.ObjApptServiceDetailsList_Update, out bErrorSerU, out ErrorDetailSerU)));
                    cmd.AddParameter("@xmlPatientApptServiceDetails_Delete", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertServiceDetailsToXml(ObjPatientAppointment.PatientID, false, ObjPatientAppointment.ApptDate.Value, ObjPatientAppointment.ObjApptServiceDetailsList_Delete, out bErrorSerD, out ErrorDetailSerD)));
                    cmd.AddParameter("@PassCheckFullTarget", SqlDbType.Bit, ConvertNullObjectToDBNull(PassCheckFullTarget));

                    bool bErrorPCLA = false;
                    string ErrorDetailPCLA = "";

                    bool bErrorPCLU = false;
                    string ErrorDetailPCLU = "";

                    bool bErrorPCLD = false;
                    string ErrorDetailPCLD = "";

                    cmd.AddParameter("@xmlPatientApptPCLRequests_Add", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertApptPCLRequestsListToXml(ObjPatientAppointment.ObjApptPCLRequestsList_Add, out bErrorPCLA, out ErrorDetailPCLA)));
                    cmd.AddParameter("@xmlPatientApptPCLRequests_Update", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertApptPCLRequestsListToXml(ObjPatientAppointment.ObjApptPCLRequestsList_Update, out bErrorPCLU, out ErrorDetailPCLU)));
                    cmd.AddParameter("@xmlPatientApptPCLRequests_Delete", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertApptPCLRequestsListToXml(ObjPatientAppointment.ObjApptPCLRequestsList_Delete, out bErrorPCLD, out ErrorDetailPCLD)));

                    cmd.AddParameter("@Result", SqlDbType.NVarChar, 8000, ParameterDirection.Output);
                    cmd.AddParameter("@ListRequestID", SqlDbType.Xml, DBNull.Value, ParameterDirection.Output);

                    if (bErrorSerA || bErrorSerU || bErrorSerD)
                    {
                        ErrorDetail = ErrorDetailSerA.Trim() + ErrorDetailSerU.Trim() + ErrorDetailSerD.Trim() + ErrorDetailPCLA.Trim() + ErrorDetailPCLU.Trim() + ErrorDetailPCLD.Trim();
                        return false;
                    }
                    else
                    {
                        cn.Open();

                        ExecuteNonQuery(cmd);

                        if (cmd.Parameters["@Result"].Value != null)
                        {
                            Int64.TryParse(cmd.Parameters["@Result"].Value.ToString(), out AppointmentID);
                        }
                        if (cmd.Parameters["@ListRequestID"].Value != null)
                        {
                            ListRequestID = cmd.Parameters["@ListRequestID"].Value.ToString();
                        }

                        if (AppointmentID > 0)
                        {
                            return true;
                        }
                        else
                        {
                            ErrorDetail = cmd.Parameters["@Result"].Value.ToString();
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }

        public override bool PatientApptPCLRequests_UpdateTemplate(PatientApptPCLRequests ObjPatientAppointment)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientApptPCLRequests_UpdateTemplate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ObjPatientAppointment.PatientPCLReqID));
                cmd.AddParameter("@ApptPCLNote", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ObjPatientAppointment.ApptPCLNote));
                cmd.AddParameter("@Diagnosis", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ObjPatientAppointment.Diagnosis));
                cmd.AddParameter("@ICD10List", SqlDbType.VarChar, ConvertNullObjectToDBNull(ObjPatientAppointment.ICD10List));
                cmd.AddParameter("@DoctorComments", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ObjPatientAppointment.DoctorComments));
                cn.Open();

                ExecuteNonQuery(cmd);
                return true;
            }
        }


        private bool PatientAppointments_DeleteAndCreate(PatientAppointment ObjPatientAppointment_NeedDel,
           bool PassCheckFullTarget,
           out long AppointmentID,
           out string ErrorDetail,
           out string ListRequestID)
        {
            try
            {
                ErrorDetail = "";
                AppointmentID = 0;
                ListRequestID = "";

                long DeleteApptID = ObjPatientAppointment_NeedDel.AppointmentID;
                DateTime NgayHenMoi = ObjPatientAppointment_NeedDel.ApptDate.Value;
                PatientAppointment ObjPatientAppointment = new PatientAppointment();
                ObjPatientAppointment = ObjPatientAppointment_NeedDel;
                //KMx: Phải lưu thành công rồi thì mới xóa. Nếu không sẽ bị sai, trường hợp xóa xong rồi lưu cuộc hẹn mới không thành công (13/03/2014 11:35).
                //if (DeletePatientAppointments(ObjPatientAppointment_NeedDel.AppointmentID) == false)
                //{
                //    ErrorDetail = "Xóa Hẹn để tạo hẹn mới không thành công";
                //    return false;
                //}

                ObjPatientAppointment.ApptDate = NgayHenMoi;
                ObjPatientAppointment.ServiceRecID = ObjPatientAppointment_NeedDel.ServiceRecID;
                ObjPatientAppointment.AppointmentID = 0;
                if (ObjPatientAppointment_NeedDel.PatientApptPCLRequestsList != null)
                {
                    List<PatientApptPCLRequests> pcllstfinal = null;
                    TachAppointmentDeleteAndCreateNew(ObjPatientAppointment_NeedDel, out pcllstfinal);
                    ObjPatientAppointment.ObjApptPCLRequestsList_Add = pcllstfinal.ToObservableCollection();
                }
                ObjPatientAppointment.ObjApptServiceDetailsList_Add = ObjPatientAppointment_NeedDel.PatientApptServiceDetailList;
                //  ObjPatientAppointment.ObjApptPCLRequestsList_Add = ObjPatientAppointment_NeedDel.PatientApptPCLRequestsList;

                //foreach (var pclReq in ObjPatientAppointment.ObjApptPCLRequestsList_Add)
                //{
                //    pclReq.PatientPCLReqID = 0;
                //}

                //return PatientAppointments_CUD(ObjPatientAppointment, PassCheckFullTarget, out AppointmentID, out ErrorDetail, out ListRequestID);
                bool IsSaveNewApptSuccess = PatientAppointments_CUD(ObjPatientAppointment, PassCheckFullTarget, out AppointmentID, out ErrorDetail, out ListRequestID);
                if (IsSaveNewApptSuccess && AppointmentID > 0 && string.IsNullOrEmpty(ErrorDetail))
                {
                    //KMx: Phải gán DeleteApptID = ObjPatientAppointment_NeedDel.AppointmentID rồi truyền DeleteApptID đi, không được truyền ObjPatientAppointment_NeedDel.AppointmentID.
                    //Vì ở trên đã gán ObjPatientAppointment = ObjPatientAppointment_NeedDel. Cho nên nếu ObjPatientAppointment.AppointmentID thì tự động ObjPatientAppointment_NeedDel.AppointmentID bị đổi theo. (13/03/2014)
                    bool IsDeleteOldApptSuccess = DeletePatientAppointments_Ext(DeleteApptID, AppointmentID);
                    if (IsDeleteOldApptSuccess == false)
                    {
                        ErrorDetail = "Xóa Hẹn để tạo hẹn mới không thành công";
                        return false;
                    }
                }
                return IsSaveNewApptSuccess;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        private string ConvertServiceDetailsToXml(long PatientID, bool HasGetNumber, Nullable<DateTime> ApptDate, IEnumerable<PatientApptServiceDetails> ObjList, out bool Error, out string ErrorDetail)
        {
            Error = false;
            ErrorDetail = "";

            if (ObjList != null && ObjList.Count() > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<PatientApptServiceDetails>");
                foreach (PatientApptServiceDetails item in ObjList)
                {
                    sb.Append("<Info>");
                    sb.AppendFormat("<ApptSvcDetailID>{0}</ApptSvcDetailID>", item.ApptSvcDetailID);
                    sb.AppendFormat("<MedServiceID>{0}</MedServiceID>", item.MedService.MedServiceID);
                    sb.AppendFormat("<DeptLocationID>{0}</DeptLocationID>", item.DeptLocation.DeptLocationID);
                    sb.AppendFormat("<ApptTimeSegmentID>{0}</ApptTimeSegmentID>", item.ApptTimeSegment.ConsultationTimeSegmentID);
                    sb.AppendFormat("<SequenceNumber>{0}</SequenceNumber>", ConvertNullObjectToDBNull(item.ServiceSeqNum));/*dành cho cột trong bảng PatientApptCancelSeqNumbers: khi thiết kế DB khi thì SequenceNumber khi thì SequenceNum giờ để vầy luôn, mắc công sửa store nhiều quá*/

                    sb.AppendFormat("<ServiceSeqNum>{0}</ServiceSeqNum>", ConvertNullObjectToDBNull(item.ServiceSeqNum));
                    sb.AppendFormat("<ServiceSeqNumType>{0}</ServiceSeqNumType>", ConvertNullObjectToDBNull(item.ServiceSeqNumType));
                    sb.AppendFormat("<V_AppointmentType>{0}</V_AppointmentType>", ConvertNullObjectToDBNull(item.V_AppointmentType));

                    sb.Append("</Info>");
                }
                sb.Append("</PatientApptServiceDetails>");

                return sb.ToString();
            }
            else
            {
                return null;
            }
        }

        private string ConvertApptPCLRequestsListToXml(IEnumerable<PatientApptPCLRequests> ObjList, out bool Error, out string ErrorDetail)
        {
            Error = false;
            ErrorDetail = "";

            if (ObjList != null && ObjList.Count() > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<PatientApptPCLRequests>");
                foreach (PatientApptPCLRequests item in ObjList)
                {
                    sb.Append("<Info>");
                    sb.AppendFormat("<PatientPCLReqID>{0}</PatientPCLReqID>", ConvertNullObjectToDBNull(item.PatientPCLReqID));
                    sb.AppendFormat("<ReqFromDeptLocID>{0}</ReqFromDeptLocID>", ConvertNullObjectToDBNull(item.ReqFromDeptLocID));
                    sb.AppendFormat("<PCLRequestNumID>{0}</PCLRequestNumID>", ConvertNullObjectToDBNull(item.PatientPCLReqID <= 0 ? new ServiceSequenceNumberProvider().GetCode_AppPCLRequest(item.V_PCLMainCategory) : item.PCLRequestNumID));
                    sb.AppendFormat("<Diagnosis>{0}</Diagnosis>", ConvertNullObjectToDBNull(item.Diagnosis));
                    sb.AppendFormat("<ICD10List>{0}</ICD10List>", ConvertNullObjectToDBNull(item.ICD10List));
                    sb.AppendFormat("<ApptPCLNote>{0}</ApptPCLNote>", ConvertNullObjectToDBNull(item.ApptPCLNote));
                    sb.AppendFormat("<AppointmentID>{0}</AppointmentID>", ConvertNullObjectToDBNull(item.AppointmentID));
                    sb.AppendFormat("<DoctorComments>{0}</DoctorComments>", ConvertNullObjectToDBNull(item.DoctorComments));
                    sb.AppendFormat("<V_PCLMainCategory>{0}</V_PCLMainCategory>", ConvertNullObjectToDBNull(item.V_PCLMainCategory));

                    if (item.ObjPatientApptPCLRequestDetailsList.Count > 0)
                    {
                        sb.Append("<PatientApptPCLRequestDetails>");
                        foreach (var detail in item.ObjPatientApptPCLRequestDetailsList)
                        {
                            sb.Append("<Info>");
                            sb.AppendFormat("<PCLReqItemID>{0}</PCLReqItemID>", ConvertNullObjectToDBNull(detail.PCLReqItemID));
                            sb.AppendFormat("<PCLExamTypeID>{0}</PCLExamTypeID>", ConvertNullObjectToDBNull(detail.ObjPCLExamTypes != null ? detail.ObjPCLExamTypes.PCLExamTypeID : detail.PCLExamTypeID));
                            sb.AppendFormat("<DeptLocationID>{0}</DeptLocationID>", ConvertNullObjectToDBNull(detail.ObjDeptLocID != null ? detail.ObjDeptLocID.DeptLocationID : detail.DeptLocID));
                            sb.AppendFormat("<ApptTimeSegmentID>{0}</ApptTimeSegmentID>", ConvertNullObjectToDBNull(detail.ApptTimeSegment == null ? default(long?) : detail.ApptTimeSegment.ParaclinicalTimeSegmentID));
                            sb.Append("</Info>");
                        }
                        sb.Append("</PatientApptPCLRequestDetails>");

                    }
                    sb.Append("</Info>");
                }
                sb.Append("</PatientApptPCLRequests>");

                return sb.ToString();
            }
            else
            {
                return null;
            }
        }

        public override List<PatientApptServiceDetails> PatientApptServiceDetailsGetAll(PatientApptServiceDetailsSearchCriteria searchCriteria,
                                        int PageIndex,
                                     int PageSize,
                                     string OrderBy,
                                     bool CountTotal,
                                     out int Total) 
        {
            using(SqlConnection cn=new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientApptServiceDetails", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(searchCriteria.DeptLocationID));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(searchCriteria.StaffID));
                cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(searchCriteria.MedServiceID));
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(searchCriteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(searchCriteria.ToDate));

                cn.Open();
                List<PatientApptServiceDetails> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetAppointmentDetailsCollectionFromReader(reader);
                }
                reader.Close();
                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;
                return objList;

            }
        }

        #endregion


        #region ApptService
        //public override List<RefDepartments> RefMedicalServiceItems_GetDeptIDIsKhamBenhNotPCL()
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spRefMedicalServiceItems_GetDeptIDIsKhamBenhNotPCL", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        //cmd.AddParameter("@ParDeptID", SqlDbType.NVarChar, ParDeptID);

        //        cn.Open();
        //        List<RefDepartments> objList = null;

        //        IDataReader reader = ExecuteReader(cmd);
        //        if (reader != null)
        //        {
        //            objList = GetRefDepartmentsCollectionFromReader(reader);
        //        }
        //        reader.Close();
        //        return objList;
        //    }
        //}

        public override List<RefMedicalServiceItem> RefMedicalServiceItems_GetByDeptIDIsKhamBenhNotPCL_Paging(
         RefMedicalServiceItemsSearchCriteria Criteria,

         int PageIndex,
         int PageSize,
         string OrderBy,
         bool CountTotal,
         out int Total
         )
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefMedicalServiceItems_GetByDeptIDIsKhamBenhNotPCL_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.DeptID));
                cmd.AddParameter("@MedServiceCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.MedServiceCode));
                cmd.AddParameter("@MedServiceName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.MedServiceName));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<RefMedicalServiceItem> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetMedicalServiceItemCollectionFromReader(reader);
                }
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

        public override List<Lookup> ApptService_GetByMedServiceID(Int64 MedServiceID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spApptService_GetByMedServiceID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, MedServiceID);

                cn.Open();
                List<Lookup> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetLookupCollectionFromReader(reader);
                }
                reader.Close();
                return objList;
            }
        }

        public override void ApptService_XMLSave(Int64 MedServiceID, IEnumerable<Lookup> ObjList, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spApptService_XMLSave", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(MedServiceID));
                cmd.AddParameter("@DataXML", SqlDbType.Xml, ApptService_XMLSave_ConvertListToXml(MedServiceID, ObjList));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
            }
        }
        private string ApptService_XMLSave_ConvertListToXml(Int64 MedServiceID, IEnumerable<Lookup> ObjList)
        {
            if (ObjList != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (Lookup item in ObjList)
                {
                    sb.Append("<ApptService>");
                    sb.AppendFormat("<MedServiceID>{0}</MedServiceID>", MedServiceID);
                    sb.AppendFormat("<V_AppointmentType>{0}</V_AppointmentType>", item.LookupID);
                    sb.Append("</ApptService>");
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

        #region PatientApptLocTargets
        public override List<PatientApptLocTargets> PatientApptLocTargetsByDepartmentLocID(long DepartmentLocID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientApptLocTargetsByDepartmentLocID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DepartmentLocID", SqlDbType.BigInt, DepartmentLocID);

                cn.Open();
                List<PatientApptLocTargets> retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                retVal = GetPatientApptLocTargetsCollectionFromReader(reader);
                return retVal;
            }
        }


        public override void PatientApptLocTargets_Save(PatientApptLocTargets Obj, out string Result)
        {
            Result = "";

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientApptLocTargets_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientApptTargetID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PatientApptTargetID));
                cmd.AddParameter("@DepartmentLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.ObjDepartmentLocID.DeptLocationID));
                cmd.AddParameter("@ApptTimeSegmentID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.ObjApptTimeSegmentID.ConsultationTimeSegmentID));
                cmd.AddParameter("@NumberOfAppt", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.NumberOfAppt));
                cmd.AddParameter("@StartSequenceNumber", SqlDbType.SmallInt, ConvertNullObjectToDBNull(Obj.StartSequenceNumber));
                cmd.AddParameter("@EndSequenceNumber", SqlDbType.SmallInt, ConvertNullObjectToDBNull(Obj.EndSequenceNumber));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 50, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
            }
        }

        public override bool PatientApptLocTargets_Delete(long PatientApptTargetID)
        {
            string Result = "";

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientApptLocTargets_Delete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientApptTargetID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientApptTargetID));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 50, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();

                if (Result == "1")
                    return true;
                return false;
            }
        }



        #endregion
    }
}
