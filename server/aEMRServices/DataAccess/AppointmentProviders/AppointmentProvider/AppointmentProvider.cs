using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataEntities;
using System.Data.SqlClient;
using System.Data;

using System.Reflection;
using eHCMS.Configurations;
using eHCMS.Services.Core;
using System.Collections.ObjectModel;
using Service.Core.Common;

namespace eHCMS.DAL
{
    public abstract class AppointmentProvider : DataProviderBase
    {
        static private AppointmentProvider _instance = null;
        /// <summary>
        /// Returns an instance of the provider type specified in the config file (using reflection)
        /// </summary>
        static public AppointmentProvider Instance
        {
            get
            {
                lock (typeof(AppointmentProvider))
                {
                    if (_instance == null)
                    {
                        string tempPath = AppDomain.CurrentDomain.RelativeSearchPath;
                        if (string.IsNullOrEmpty(AppDomain.CurrentDomain.RelativeSearchPath))
                            tempPath = AppDomain.CurrentDomain.BaseDirectory;
                        else
                            tempPath = AppDomain.CurrentDomain.RelativeSearchPath;
                        string assemblyPath = System.IO.Path.Combine(tempPath, Globals.Settings.Appointments.Assembly + ".dll");
                        Assembly assem = Assembly.LoadFrom(assemblyPath);
                        Type t = assem.GetType(Globals.Settings.Appointments.ProviderType);
                        _instance = (AppointmentProvider)Activator.CreateInstance(t);
                    }
                }
                return _instance;
            }
        }

        public AppointmentProvider()
        {
            this.ConnectionString = Globals.Settings.Appointments.ConnectionString;

        }

        public abstract List<PatientApptTimeSegment> GetAllAppointmentSegments();
        /// <summary>
        /// Lấy danh sách các service ứng với một loại hẹn.(Tái khám, trả kết quả xét nghiệm...)
        /// </summary>
        /// <returns></returns>
        public abstract List<RefMedicalServiceItem> GetAllServicesByAppointmentType(long appointmentType);
        public abstract List<PatientAppointment> GetAppointmentsOfPatient(long patientID, int pageIndex, int pageSize, bool bCountTotal, out int TotalCount);
        public abstract List<PatientAppointment> GetAllAppointments(AppointmentSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int TotalCount);
        public abstract List<PatientAppointment> GetAllAppointmentsDay(AppointmentSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int TotalCount);

        // TxD 31/07/2014 Commented out GetAllLocationsByService because it's not USED
        //public abstract List<Location> GetAllLocationsByService(long serviceID);

        //public abstract bool InsertAppointment(PatientAppointment appointment, out long AppointmentID);
        //public abstract bool UpdateAppointment(PatientAppointment appointment);
        public abstract PatientAppointment GetAppointmentByID(long appointmentID);
        public abstract short CreateApptSeqNumber(long PatientID, DateTime ApptDate, long DepartmentLocID, short ApptTimeSegmentID);
        public abstract List<DeptLocation> GetAllDeptLocationsByService(long serviceID);

        public abstract List<DeptLocation> GetAllDeptLocationsByService_WithSeqNumberSegment(long MedServiceID, DateTime ApptDate);

        public abstract List<ConsultationTimeSegments> Segments_WithAppDateDeptLocIDSeqNumber(long DeptLocationID, DateTime ApptDate);

        public abstract bool CreateAppointment(PatientAppointment appointment, out long AppointmentID);
        public abstract bool InsertAppointmentDetails(PatientApptServiceDetails apptDetails, out long ApptDetailsID);
        public abstract bool DeletePatientAppointments(long AppointmentID);
        public abstract bool DeletePatientAppointments_Ext(long AppointmentID, long? NewAppointmentID);
        public abstract bool DeleteAppointmentDetails(PatientApptServiceDetails apptDetails);
        public abstract bool ReturnApptSeqNumber(DateTime ApptDate, long DepartmentLocID, short ApptTimeSegmentID, short SeqNumber);
        public abstract PatientAppointment GetAppointmentOfPatientByDate(long patientID, DateTime ApptDate);
        public abstract int GetNumberOfAppointments(DateTime ApptDate, long DeptLocID, short segmentID);
        public abstract void GetNumberOfAvailablePosition(DateTime ApptDate, long DeptLocID, short segmentID, out int MaxNumOfAppointments, out int NumOfAppts);
        public abstract List<PatientAppointment> SearchAppointments(AppointmentSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int TotalCount);

        public abstract List<DeptLocation> AllDeptLocation_LAB();
        public abstract List<DeptLocation> AllDeptLocation_NotLAB(long V_PCLMainCategory, long PCLExamTypeSubCategoryID);


        public abstract List<PatientApptPCLRequestDetails> PatientApptPCLRequestDetails_ByID(long AppointmentID, long PatientPCLReqID);


        public abstract bool PatientAppointments_Save(PatientAppointment ObjPatientAppointment
            , bool PassCheckFullTarget
            , out long AppointmentID
            , out string ErrorDetail
            , out string ListNotConfig
            , out string ListTargetFull
            , out string ListMax
            , out string ListRequestID);
        public abstract bool PatientApptPCLRequests_UpdateTemplate(PatientApptPCLRequests ObjPatientAppointment);

        #region ApptService
        //public abstract List<RefDepartments> RefMedicalServiceItems_GetDeptIDIsKhamBenhNotPCL();

        public abstract List<RefMedicalServiceItem> RefMedicalServiceItems_GetByDeptIDIsKhamBenhNotPCL_Paging(
         RefMedicalServiceItemsSearchCriteria Criteria,

         int PageIndex,
         int PageSize,
         string OrderBy,
         bool CountTotal,
         out int Total
         );

        public abstract List<PatientApptServiceDetails> PatientApptServiceDetailsGetAll(PatientApptServiceDetailsSearchCriteria searchCriteria,
            int PageIndex,
                                     int PageSize,
                                     string OrderBy,
                                     bool CountTotal,
                                     out int Total);
        
        public abstract List<Lookup> ApptService_GetByMedServiceID(Int64 MedServiceID);

        public abstract void ApptService_XMLSave(Int64 MedServiceID, IEnumerable<Lookup> ObjList, out string Result);

        #endregion

        #region "PatientApptLocTargets"
        public abstract List<PatientApptLocTargets> PatientApptLocTargetsByDepartmentLocID(long DepartmentLocID);

        public abstract void PatientApptLocTargets_Save(PatientApptLocTargets Obj, out string Result);

        public abstract bool PatientApptLocTargets_Delete(long PatientApptTargetID);
        #endregion
    }
}
