/*
20161223 #001 CMN: Add file manager
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using DataEntities;
using eHCMS.Configurations;

namespace eHCMS.DAL
{
    public abstract class ClinicManagementProvider: DataProviderBase
    {
        private static ClinicManagementProvider _instance=null;
        //public enum DaysDefine
        //{
        //    Mon = 0x001,
        //    Tues = 0x002,
        //    Wed = 0x004,
        //    Thur = 0x008,
        //    Fri = 0x010,
        //    Sat = 0x020,
        //    Sun = 0x040,
        //}
        //protected virtual int GetNumOfDay(ConsultationRoomTarget cur)
        //{
        //    int temp = 0;
        //    if (cur.Monday)
        //        temp = temp | (int)DaysDefine.Mon;
        //    if (cur.Tuesday)
        //        temp = temp | (int)DaysDefine.Tues;
        //    if (cur.Wednesday)
        //        temp = temp | (int)DaysDefine.Wed;
        //    if (cur.Thursday)
        //        temp = temp | (int)DaysDefine.Thur;
        //    if (cur.Friday)
        //        temp = temp | (int)DaysDefine.Fri;
        //    if (cur.Saturday)
        //        temp = temp | (int)DaysDefine.Sat;
        //    if (cur.Sunday)
        //        temp = temp | (int)DaysDefine.Sun;
        //    return temp;
        //}
        //protected virtual void CheckDay(int temp, ref ConsultationRoomTarget cur)
        //{
        //    cur.Monday = (temp & (int)DaysDefine.Mon) == (int)DaysDefine.Mon ? true : false;
        //    cur.Tuesday = (temp & (int)DaysDefine.Tues) == (int)DaysDefine.Tues ? true : false;
        //    cur.Wednesday = (temp & (int)DaysDefine.Wed) == (int)DaysDefine.Wed ? true : false;
        //    cur.Thursday = (temp & (int)DaysDefine.Thur) == (int)DaysDefine.Thur ? true : false;
        //    cur.Friday = (temp & (int)DaysDefine.Fri) == (int)DaysDefine.Fri ? true : false;
        //    cur.Saturday = (temp & (int)DaysDefine.Sat) == (int)DaysDefine.Sat ? true : false;
        //    cur.Sunday = (temp & (int)DaysDefine.Sun) == (int)DaysDefine.Sun ? true : false;
        //}
        public static ClinicManagementProvider instance
        {
            get
            {
                if (_instance==null)
                {
                    string tempPath = AppDomain.CurrentDomain.RelativeSearchPath;
                    if (string.IsNullOrEmpty(AppDomain.CurrentDomain.RelativeSearchPath))
                    {
                        tempPath =AppDomain.CurrentDomain.BaseDirectory;
                    }
                    else
                    {
                        tempPath = AppDomain.CurrentDomain.RelativeSearchPath;
                    }

                    string assemblyPath = System.IO.Path.Combine(tempPath, Globals.Settings.ClinicManagement.Assembly + ".dll");
                    Assembly assem = Assembly.LoadFrom(assemblyPath);
                    Type t = assem.GetType(Globals.Settings.ClinicManagement.ProviderType);
                    _instance = (ClinicManagementProvider)Activator.CreateInstance(t);
                }
                return _instance;
            }
        }

        public ClinicManagementProvider()
        {
            this.ConnectionString =Globals.Settings.Common.ConnectionString;
        }

#region Clinic member

        public abstract bool InsertConsultationTimeSegments(string SegmentName
                                                      , string SegmentDescription
                                                      , DateTime StartTime
                                                      , DateTime EndTime
                                                      , bool IsActive);

        public abstract bool UpdateConsultationTimeSegments(long ConsultationTimeSegmentID
                                                            , string SegmentName
                                                            , string SegmentDescription
                                                            , DateTime StartTime
                                                            , DateTime EndTime);

        public abstract bool DeleteConsultationTimeSegments(long ConsultationTimeSegmentID);

        public abstract List<ConsultationTimeSegments> GetAllConsultationTimeSegmentsCache();

        public abstract List<ConsultationTimeSegments> GetAllConsultationTimeSegments();
        public abstract List<PCLTimeSegment> GetAllPCLTimeSegments();
        
        public abstract List<ConsultationTimeSegments> ConsultationTimeSegments_ByDeptLocationID(long DeptLocationID);

        public abstract bool InsertConsultationRoomTarget(ConsultationRoomTarget consultationRoomTarget);
        public abstract bool InsertConsultationRoomTargetXML(List<ConsultationRoomTarget> lstCRSA);

        public abstract bool UpdateConsultationRoomTargetXML(IList<ConsultationRoomTarget> lstCRSA);
        public abstract bool DeleteConsultationRoomTarget(long ConsultationRoomTargetID);

        public abstract List<ConsultationRoomTarget> GetConsultationRoomTargetByDeptID(long DeptLocationID);
        public abstract List<ConsultationRoomTarget> GetConsultationRoomTargetTimeSegment(long DeptLocationID,long ConsultationTimeSegmentID);
        public abstract List<ConsultationRoomTarget> GetConsultationRoomTargetTSegment(long DeptLocationID, bool IsHis);
        public abstract void GetAllConsultationRoomTargetCache();
        

        public abstract bool InsertConsultationRoomStaffAllocations(long DeptLocationID
                                                  , long ConsultationTimeSegmentID
                                                  , long StaffID
                                                  , long StaffCatgID
                                                  , DateTime AllocationDate);

        public abstract bool InsertConsultationRoomStaffAllocationsXML(IList<ConsultationRoomStaffAllocations> lstCRSA);

        public abstract bool UpdateConsultationRoomStaffAllocations(long ConsultationRoomStaffAllocID, bool IsActive);

        public abstract bool UpdateConsultationRoomStaffAllocationsXML(IList<ConsultationRoomStaffAllocations> lstCRSA);

        public abstract bool DeleteConsultationRoomStaffAllocations(long DeptLocationID
                                                                , long ConsultationTimeSegmentID
                                                                , DateTime AllocationDate);
        public abstract List<ConsultationRoomStaffAllocations> GetConsultationRoomStaffAllocations(long DeptLocationID
                                                                                        ,long ConsultationTimeSegmentID);
        protected virtual ConsultationRoomStaffAllocations GetConsultationRoomStaffAllocationsObjFromReader(IDataReader reader)
        {
            ConsultationRoomStaffAllocations p = new ConsultationRoomStaffAllocations();
            try
            {
                p.Staff = new Staff();
                p.ConsultationTimeSegments=new ConsultationTimeSegments();
                if (reader.HasColumn("ConsultationTimeSegmentID") && reader["ConsultationTimeSegmentID"] != DBNull.Value)
                {
                    p.ConsultationTimeSegmentID = (long)reader["ConsultationTimeSegmentID"];
                    p.ConsultationTimeSegments.ConsultationTimeSegmentID=(long)reader["ConsultationTimeSegmentID"];
                }

                if (reader.HasColumn("SegmentName") && reader["SegmentName"] != DBNull.Value)
                {
                    p.ConsultationTimeSegments.SegmentName = reader["SegmentName"].ToString();
                }

                if (reader.HasColumn("ConsultationRoomStaffAllocID") && reader["ConsultationRoomStaffAllocID"] != DBNull.Value)
                {
                    p.ConsultationRoomStaffAllocID = (long)reader["ConsultationRoomStaffAllocID"];
                }

                if (reader.HasColumn("DeptLocationID") && reader["DeptLocationID"] != DBNull.Value)
                {
                    p.DeptLocationID = (long)reader["DeptLocationID"];
                }

                if (reader.HasColumn("StaffCatgID") && reader["StaffCatgID"] != DBNull.Value)
                {
                    p.StaffCatgID = Convert.ToInt32(reader["StaffCatgID"]);
                    p.Staff.StaffCatgID = Convert.ToInt32(reader["StaffCatgID"]);
                }

                if (reader.HasColumn("StaffID") && reader["StaffID"] != DBNull.Value)
                {
                    p.StaffID = (long)reader["StaffID"];
                    p.Staff.StaffID = (long)reader["StaffID"];
                }

                if (reader.HasColumn("IsActive") && reader["IsActive"] != DBNull.Value)
                {
                    p.IsActive = (bool)reader["IsActive"];
                }

                if (reader.HasColumn("AllocationDate") && reader["AllocationDate"] != DBNull.Value)
                {
                    p.AllocationDate = (DateTime)reader["AllocationDate"];
                    if (p.AllocationDate > DateTime.Now)
                    {
                        p.isEdit = true;
                    }
                }

                
                if (reader.HasColumn("FullName") && reader["FullName"] != DBNull.Value)
                {
                    p.Staff.FullName = reader["FullName"].ToString();
                }

                p.Staff.RefStaffCategory=new RefStaffCategory();
                if(reader.HasColumn("V_StaffCatType") && reader["V_StaffCatType"]!=DBNull.Value)
                {
                    p.Staff.RefStaffCategory.V_StaffCatType = (long)reader["V_StaffCatType"];
                }
                if (reader.HasColumn("StaffCatgDescription") && reader["StaffCatgDescription"] != DBNull.Value)
                {
                    p.Staff.RefStaffCategory.StaffCatgDescription = reader["StaffCatgDescription"].ToString();
                }

            }catch
            {
                
            }
            
            
            return p;
        }
        protected virtual List<ConsultationRoomStaffAllocations> GetConsultationRoomStaffAllocationsCollectionFromReader(IDataReader reader)
        {
            List<ConsultationRoomStaffAllocations> lst=new List<ConsultationRoomStaffAllocations>();
            while(reader.Read())
            {
                lst.Add(GetConsultationRoomStaffAllocationsObjFromReader(reader));
            }
            return lst;
        }
        #endregion
        //==== #001
        #region FileManager
        public abstract List<RefShelves> GetRefShelves(long RefShelfID, long StoreID);
        public abstract List<RefShelves> UpdateRefShelves(long StaffID, List<RefShelves> ListRefShelves, long StoreID);
        public abstract List<RefShelfDetails> GetRefShelfDetails(long RefShelfID, string LocCode, long StoreID);
        public abstract bool UpdateRefShelfDetails(long RefShelfID, List<RefShelfDetails> ListRefShelfDetail);
        public abstract List<PatientMedicalFileStorage> GetPatientMedicalFileStorage(long RefShelfDetailID, string FileCodeNumber, out long PatientMedicalFileID);
        public abstract List<PatientMedicalFileStorage> UpdatePatientMedicalFileStorage(long RefShelfDetailID, List<PatientMedicalFileStorage> ListPatientMedicalFileStorage);
        public abstract List<PatientMedicalFileStorageCheckInCheckOut> GetPatientMedicalFileStorageCheckInCheckOut(string FileCodeNumber);
        public abstract List<StaffPersons> GetAllStaffPersons();
        public abstract bool UpdatePatientMedicalFileStorageCheckOut(long StaffID, List<PatientMedicalFileStorageCheckInCheckOut> ListPatientMedicalFileStorageCheckOut, out long MedicalFileStorageCheckID);
        public abstract List<PatientMedicalFileStorageCheckInCheckOut> GetMedicalFileFromRegistration(long DeptID, long LocationID, DateTime StartDate, DateTime EndDate, bool IsBorrowed, bool IsStored);
        public abstract string GetXMLPatientMedicalFileStorages(List<PatientMedicalFileStorage> ListPatientMedicalFileStorage);
        public abstract List<PatientMedicalFileStorageCheckInCheckOut> SearchMedicalFilesHistory(DateTime? StartDate, DateTime? EndDate, string FileCodeNumber, long? StaffID, long? ReceiveStaffID, int Status, int PageSize, int PageIndex, out int TotalRow);
        public abstract string GetRegistrationXMLFromMedicalFileList(List<PatientMedicalFileStorageCheckInCheckOut> ListItem);
        public abstract List<PatientMedicalFileStorageCheckInCheckOut> GetMedicalFileStorageCheckOutHistory(DateTime? StartDate, DateTime? EndDate, long? StaffID, long? ReceiveStaffID, int PageSize, int PageIndex, out int TotalRow);
        public abstract List<PatientMedicalFileStorageCheckInCheckOut> GetMedicalFileDetails(string FileCodeNumber, long? RefShelfDetailID, int PageSize, int PageIndex, out int TotalRow);
        #endregion
        //==== #001
    }
}
