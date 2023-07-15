using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using DataEntities;
using ErrorLibrary;
using System.Collections.Generic;

using System;
using System.Reflection;
using AxLogging;
using ErrorLibrary.Resources;

using System.Linq;

using eHCMSLanguage;

namespace AppointmentService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [KnownType(typeof(AxException))]
    public class AppointmentService : eHCMS.WCFServiceCustomHeader, IAppointmentService
    {

        public AppointmentService()
        {
            int currentID = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }

        public List<PatientAppointment> GetAppointments(AppointmentSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int TotalCount)
        {
            TotalCount = 0;
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.GetAllAppointments(criteria, pageIndex, pageSize, bCountTotal, out TotalCount);
                //}
                //else
                //{
                //    return AppointmentProvider.Instance.GetAllAppointments(criteria, pageIndex, pageSize, bCountTotal, out TotalCount);
                //}
                return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.GetAllAppointments(criteria, pageIndex, pageSize, bCountTotal, out TotalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading appointments. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, ErrorNames.APPOINTMENT_LIST_CANNOT_LOAD);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeletePatientAppointments(long AppointmentID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.DeletePatientAppointments(AppointmentID);
                //}
                //else
                //{
                //    return AppointmentProvider.Instance.DeletePatientAppointments(AppointmentID);
                //}
                return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.DeletePatientAppointments(AppointmentID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading appointments. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, ErrorNames.APPOINTMENT_LIST_CANNOT_LOAD);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PatientAppointment> GetAppointmentsDay(AppointmentSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int TotalCount)
        {
            TotalCount = 0;
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.GetAllAppointmentsDay(criteria, pageIndex, pageSize, bCountTotal, out TotalCount);
                //}
                //else
                //{
                //    return AppointmentProvider.Instance.GetAllAppointmentsDay(criteria, pageIndex, pageSize, bCountTotal, out TotalCount);
                //}
                return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.GetAllAppointmentsDay(criteria, pageIndex, pageSize, bCountTotal, out TotalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading appointments. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, ErrorNames.APPOINTMENT_LIST_CANNOT_LOAD);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<PatientAppointment> SearchAppointments(AppointmentSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int TotalCount)
        {
            TotalCount = 0;
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.SearchAppointments(criteria, pageIndex, pageSize, bCountTotal, out TotalCount);
                //}
                //else
                //{
                //    return AppointmentProvider.Instance.SearchAppointments(criteria, pageIndex, pageSize, bCountTotal, out TotalCount);
                //}
                return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.SearchAppointments(criteria, pageIndex, pageSize, bCountTotal, out TotalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading appointments. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, ErrorNames.APPOINTMENT_LIST_CANNOT_LOAD);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<PatientAppointment> GetAppointmentsOfPatient(long patientID, int pageIndex, int pageSize, bool bCountTotal, out int TotalCount)
        {
            TotalCount = 0;
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.GetAppointmentsOfPatient(patientID, pageIndex, pageSize, bCountTotal, out TotalCount);
                //}
                //else
                //{
                //    return AppointmentProvider.Instance.GetAppointmentsOfPatient(patientID, pageIndex, pageSize, bCountTotal, out TotalCount);
                //}
                return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.GetAppointmentsOfPatient(patientID, pageIndex, pageSize, bCountTotal, out TotalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading appointments. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, ErrorNames.APPOINTMENT_LIST_CANNOT_LOAD);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //public void InsertAppointment(PatientAppointment appointment, out long AppointmentID, out PatientAppointment AddedAppointment
        //    , out List<PatientApptServiceDetails> RequestSeqNoFailedList, out List<PatientApptServiceDetails> InsertFailedList)
        //{
        //    try
        //    {
        //        AppointmentID = -1;
        //        RequestSeqNoFailedList = new List<PatientApptServiceDetails>();
        //        InsertFailedList = new List<PatientApptServiceDetails>();
        //        AddedAppointment = appointment;

        //        appointment.DateModified = null;
        //        appointment.RecDateCreated = DateTime.Now;
        //        //Xin so Sequence No.
        //        short seqNo = -1;

        //        //TO DO:
        //        //Kiem tra cuoc hen trong ngay cua benh nhan da co chua.
        //        //Chua co thi tao. Co roi thi bao loi.

        //        //Tao mot cuoc hen (master) truoc.
        //        //Neu tao khong dc thi throw exception roi.
        //        if(appointment.V_ApptStatus == AllLookupValues.ApptStatus.WAITING)
        //        {
        //            //-----Dinh them phan nay
        //            //-----Neu trang thai la waiting thi dua no vao danh sach patient queue de chuyen qua confirm hen benh sau
        //            appointment.V_ApptStatus = AllLookupValues.ApptStatus.PENDING;
        //            var queue = new PatientQueue();
        //            queue.V_QueueType = (long)(int)AllLookupValues.QueueType.DANG_KY_HEN_BENH;
        //            queue.PatientAppointmentID = AppointmentID;
        //            queue.PatientID = appointment.Patient.PatientID;
        //            queue.FullName = appointment.Patient.FullName;
        //            //queue.RegistrationID = appointment.Patient.LatestRegistration.PtRegistrationID;
        //            queue.EnqueueTime = appointment.ApptDate;

        //            PatientQueue_Insert(queue);                
        //        }
        //        else
        //        {
        //            appointment.V_ApptStatus = AllLookupValues.ApptStatus.BOOKED;
        //        }

        //        AppointmentProvider.Instance.CreateAppointment(appointment, out AppointmentID);

        //        foreach (PatientApptServiceDetails item in appointment.PatientApptServiceDetailList)
        //        {
        //            item.AppointmentID = AppointmentID;

        //            try
        //            {
        //                seqNo = AppointmentProvider.Instance.CreateApptSeqNumber(appointment.ApptDate.Value, item.DeptLocation.DeptLocationID,(short)item.ApptTimeSegment.ConsultationTimeSegmentID);
        //            }
        //            catch (Exception requestSeqFailed)
        //            {
        //                RequestSeqNoFailedList.Add(item);
        //                continue;
        //            }
        //            if(seqNo == -1) //Xin so khong duoc
        //            {
        //                RequestSeqNoFailedList.Add(item);
        //            }
        //            else //Lay so duoc roi
        //            {
        //                item.ServiceSeqNum = seqNo;

        //                long detailsID = -1;
        //                try
        //                {
        //                    AppointmentProvider.Instance.InsertAppointmentDetails(item, out detailsID);
        //                }
        //                catch(Exception ex)
        //                {
        //                    InsertFailedList.Add(item);
        //                    //Insert khong duoc thi tra so lai.
        //                    TryReturnSeqNo(appointment.ApptDate.Value, item.DeptLocation.DeptLocationID,(short)item.ApptTimeSegment.ConsultationTimeSegmentID, item.ServiceSeqNum);
        //                }
        //            }
        //        }
        //        AddedAppointment = AppointmentProvider.Instance.GetAppointmentByID(AppointmentID);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of adding appointment. Status: Failed.", CurrentUser);

        //        AxException axErr = new AxException(ex, ErrorNames.APPOINTMENT_LIST_CANNOT_INSERT);

        //        AxLogger.Instance.LogDebug(axErr, CurrentUser);
        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}
        private void TryReturnSeqNo(DateTime appDate, long deptLocID, short segmentID, short seqNo)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.ReturnApptSeqNumber(appDate, deptLocID, segmentID, seqNo);
                //}
                //else
                //{
                //    AppointmentProvider.Instance.ReturnApptSeqNumber(appDate, deptLocID, segmentID, seqNo);
                //}
                aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.ReturnApptSeqNumber(appDate, deptLocID, segmentID, seqNo);
            }
            catch
            {
                //Tra so khong duoc thi thoi.
            }
        }

        //public void UpdateAppointment(PatientAppointment appointment, out PatientAppointment UpdatedAppointment, out List<PatientApptServiceDetails> RequestSeqNoFailedList, out List<PatientApptServiceDetails> InsertFailedList, out List<PatientApptServiceDetails> DeleteFailedList)
        //{
        //    try
        //    {
        //        RequestSeqNoFailedList = new List<PatientApptServiceDetails>();
        //        InsertFailedList = new List<PatientApptServiceDetails>();
        //        DeleteFailedList = new List<PatientApptServiceDetails>();

        //        UpdatedAppointment = appointment;
        //        appointment.DateModified = DateTime.Now;
        //        //dinh them--- neu la pending thi chuyen sang book
        //        if (appointment.V_ApptStatus == AllLookupValues.ApptStatus.PENDING) 
        //        {
        //            appointment.V_ApptStatus = AllLookupValues.ApptStatus.BOOKED;
        //        }

        //        //Update cuoc hen (master).
        //        bool bUpdateOK = AppointmentProvider.Instance.UpdateAppointment(appointment);
        //        if(bUpdateOK)
        //        {
        //            short seqNo = -1;
        //            //Duyet qua details
        //            //Delete nhung item danh dau xoa truoc, de tra so nghiem chinh roi moi xu ly nhung item duoc them moi.
        //            foreach (PatientApptServiceDetails item in appointment.PatientApptServiceDetailList)
        //            {
        //                //item.AppointmentID = appointment.AppointmentID;
        //                //Neu item bi delete thi delete no, tra so lai. (so sequence No.)
        //                if(item.EntityState == EntityState.DELETED_MODIFIED || item.EntityState == EntityState.DELETED_PERSITED)
        //                {
        //                    if (AppointmentProvider.Instance.DeleteAppointmentDetails(item))//Neu delete duoc thi tra so.
        //                    {
        //                        TryReturnSeqNo(appointment.ApptDate.Value, item.DeptLocation.DeptLocationID, (short)item.ApptTimeSegment.ConsultationTimeSegmentID, item.ServiceSeqNum);
        //                    }
        //                    else
        //                    {
        //                        DeleteFailedList.Add(item);
        //                    }
        //                }
        //            }

        //            foreach (PatientApptServiceDetails item in appointment.PatientApptServiceDetailList)
        //            {
        //                if (item.EntityState == EntityState.DETACHED)
        //                {
        //                    item.AppointmentID = appointment.AppointmentID;
        //                    //Xin so sequence No. roi insert vao database.
        //                    try
        //                    {
        //                        seqNo = AppointmentProvider.Instance.CreateApptSeqNumber(appointment.ApptDate.Value, item.DeptLocation.DeptLocationID, (short)item.ApptTimeSegment.ConsultationTimeSegmentID);
        //                    }
        //                    catch (Exception requestSeqFailed)
        //                    {
        //                        RequestSeqNoFailedList.Add(item);
        //                        continue;
        //                    }
        //                    if (seqNo == -1) //Xin so khong duoc
        //                    {
        //                        RequestSeqNoFailedList.Add(item);
        //                    }
        //                    else //Lay so duoc roi
        //                    {
        //                        item.ServiceSeqNum = seqNo;

        //                        long detailsID = -1;
        //                        try
        //                        {
        //                            AppointmentProvider.Instance.InsertAppointmentDetails(item, out detailsID);
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            InsertFailedList.Add(item);
        //                            TryReturnSeqNo(appointment.ApptDate.Value, item.DeptLocation.DeptLocationID,(short) item.ApptTimeSegment.ConsultationTimeSegmentID, item.ServiceSeqNum);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        UpdatedAppointment = AppointmentProvider.Instance.GetAppointmentByID(appointment.AppointmentID);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of updating appointment. Status: Failed.", CurrentUser);

        //        AxException axErr = new AxException(ex, ErrorNames.APPOINTMENT_LIST_CANNOT_UPDATE);

        //        AxLogger.Instance.LogDebug(axErr, CurrentUser);
        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}

        public List<PatientApptTimeSegment> GetAllAppointmentSegments()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.GetAllAppointmentSegments();
                //}
                //else
                //{
                //    return AppointmentProvider.Instance.GetAllAppointmentSegments();
                //}
                return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.GetAllAppointmentSegments();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of updating appointment. Status: Failed.", CurrentUser);

                AxException axErr = new AxException(ex, ErrorNames.APPOINTMENT_SEGMENT_LIST_CANNOT_LOAD);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<RefMedicalServiceItem> GetAllServicesByAppointmentType(long appointmentType)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading service list.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.GetAllServicesByAppointmentType(appointmentType);
                //}
                //else
                //{
                //    return AppointmentProvider.Instance.GetAllServicesByAppointmentType(appointmentType);
                //}
                return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.GetAllServicesByAppointmentType(appointmentType);
                //AxLogger.Instance.LogInfo("End of loading service list. Status: OK.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading service list. Status: Failed.", CurrentUser);

                AxException axErr = new AxException(ex, ErrorNames.APPOINTMENT_SERVICE_LIST_CANNOT_LOAD);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        // TxD 31/07/2014 Commented out GetAllLocationsByService because it's not USED
        //public List<Location> GetAllLocationsByService(long serviceID)
        //{
        //    try
        //    {
        //        return AppointmentProvider.Instance.GetAllLocationsByService(serviceID);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of loading location list. Status: Failed.", CurrentUser);

        //        AxException axErr = new AxException(ex, ErrorNames.APPOINTMENT_LOCATION_LIST_CANNOT_LOAD);

        //        AxLogger.Instance.LogDebug(axErr, CurrentUser);
        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}

        public List<DeptLocation> GetAllDeptLocationsByService(long serviceID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.GetAllDeptLocationsByService(serviceID);
                //}
                //else
                //{
                //    return AppointmentProvider.Instance.GetAllDeptLocationsByService(serviceID);
                //}
                return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.GetAllDeptLocationsByService(serviceID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading location list. Status: Failed.", CurrentUser);

                AxException axErr = new AxException(ex, ErrorNames.APPOINTMENT_LOCATION_LIST_CANNOT_LOAD);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<DeptLocation> GetAllDeptLocationsByService_WithSeqNumberSegment(long MedServiceID, DateTime ApptDate)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.GetAllDeptLocationsByService_WithSeqNumberSegment(MedServiceID, ApptDate);
                //}
                //else
                //{
                //    return AppointmentProvider.Instance.GetAllDeptLocationsByService_WithSeqNumberSegment(MedServiceID, ApptDate);
                //}
                return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.GetAllDeptLocationsByService_WithSeqNumberSegment(MedServiceID, ApptDate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading location list. Status: Failed.", CurrentUser);

                AxException axErr = new AxException(ex, ErrorNames.APPOINTMENT_GetAllDeptLocationsByService_WithSeqNumberSegment);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public List<ConsultationTimeSegments> Segments_WithAppDateDeptLocIDSeqNumber(long DeptLocationID, DateTime ApptDate)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.Segments_WithAppDateDeptLocIDSeqNumber(DeptLocationID, ApptDate);
                //}
                //else
                //{
                //    return AppointmentProvider.Instance.Segments_WithAppDateDeptLocIDSeqNumber(DeptLocationID, ApptDate);
                //}
                return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.Segments_WithAppDateDeptLocIDSeqNumber(DeptLocationID, ApptDate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading location list. Status: Failed.", CurrentUser);

                AxException axErr = new AxException(ex, ErrorNames.APPOINTMENT_Segments_WithAppDateDeptLocIDSeqNumber);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public PatientAppointment GetAppointmentByID(long appointmentID, long? IssueID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading appointment information.", CurrentUser);
                PatientAppointment appointment = new PatientAppointment();
                appointment = aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.GetAppointmentByID(appointmentID, IssueID);
                AxLogger.Instance.LogInfo("End of loading appointment information. Status: OK.", CurrentUser);
                return appointment;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading appointment information. Status: Failed.", CurrentUser);
                AxException axErr = new AxException(ex, ErrorNames.APPOINTMENT_GetAppointmentByID);
                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }



        #region IAppointmentService Members


        public PatientAppointment GetAppointmentOfPatientByDate(long patientID, DateTime ApptDate)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading appointment information.", CurrentUser);
                PatientAppointment appointment = new PatientAppointment();
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    appointment = aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.GetAppointmentOfPatientByDate(patientID, ApptDate);
                //}
                //else
                //{
                //    appointment = AppointmentProvider.Instance.GetAppointmentOfPatientByDate(patientID, ApptDate);
                //}
                appointment = aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.GetAppointmentOfPatientByDate(patientID, ApptDate);
                AxLogger.Instance.LogInfo("End of loading appointment information. Status: OK.", CurrentUser);

                return appointment;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading appointment information. Status: Failed.", CurrentUser);

                AxException axErr = new AxException(ex, ErrorNames.APPOINTMENT_GetAppointmentOfPatientByDate);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region IAppointmentService Members


        public int GetNumberOfAppointments(DateTime ApptDate, long DeptLocID, short segmentID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start calculating number of appointments.", CurrentUser);
                int total;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    total = aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.GetNumberOfAppointments(ApptDate, DeptLocID, segmentID);
                //}
                //else
                //{
                //    total = AppointmentProvider.Instance.GetNumberOfAppointments(ApptDate, DeptLocID, segmentID);
                //}
                total = aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.GetNumberOfAppointments(ApptDate, DeptLocID, segmentID);
                AxLogger.Instance.LogInfo("End of calculating number of appointments. Status: OK.", CurrentUser);

                return total;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of calculating number of appointments. Status: Failed.", CurrentUser);

                AxException axErr = new AxException(ex, ErrorNames.APPOINTMENT_GetNumberOfAppointments);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region IAppointmentService Members


        public void GetNumberOfAvailablePosition(DateTime ApptDate, long DeptLocID, short segmentID, out int MaxNumOfAppointments, out int NumOfAppts)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start calculating number of appointments.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.GetNumberOfAvailablePosition(ApptDate, DeptLocID, segmentID, out MaxNumOfAppointments, out NumOfAppts);
                //}
                //else
                //{
                //    AppointmentProvider.Instance.GetNumberOfAvailablePosition(ApptDate, DeptLocID, segmentID, out MaxNumOfAppointments, out NumOfAppts);
                //}
                aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.GetNumberOfAvailablePosition(ApptDate, DeptLocID, segmentID, out MaxNumOfAppointments, out NumOfAppts);
                AxLogger.Instance.LogInfo("End of calculating number of appointments. Status: OK.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of calculating number of appointments. Status: Failed.", CurrentUser);

                AxException axErr = new AxException(ex, ErrorNames.APPOINTMENT_GetNumberOfAvailablePosition);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        //KMx: Anh Tuấn không cho insert vào bảng PatientQueue.
        //public bool PatientQueue_Insert(PatientQueue ObjPatientQueue)
        //{
        //    try
        //    {
        //        AxLogger.Instance.LogInfo("Start insert PatientQueue.", CurrentUser);
        //        return PatientProvider.Instance.PatientQueue_Insert(ObjPatientQueue);
        //        //AxLogger.Instance.LogInfo("End of insert PatientQueue. Status: OK.", CurrentUser);

        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of insert PatientQueue. Status: Failed.", CurrentUser);

        //        AxException axErr = new AxException(ex, ErrorNames.APPOINTMENT_PatientQueue_Insert);

        //        AxLogger.Instance.LogDebug(axErr, CurrentUser);
        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}

        //KMx: Anh Tuấn không cho insert vào bảng PatientQueue.
        //public bool PatientQueue_InsertList(List<PatientQueue> lstPatientQueue,out List<string> lstPatientQueueInsertFail) 
        //{
        //    try
        //    {
        //        AxLogger.Instance.LogInfo("Start insert PatientQueue.", CurrentUser);
        //        lstPatientQueueInsertFail = null;
        //        return PatientProvider.Instance.PatientQueue_InsertList(lstPatientQueue,ref lstPatientQueueInsertFail);
        //        //AxLogger.Instance.LogInfo("End of insert PatientQueue. Status: OK.", CurrentUser);

        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of insert PatientQueue. Status: Failed.", CurrentUser);

        //        AxException axErr = new AxException(ex, ErrorNames.APPOINTMENT_PatientQueue_InsertList);

        //        AxLogger.Instance.LogDebug(axErr, CurrentUser);
        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}


        public List<DeptLocation> AllDeptLocation_LAB()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.AllDeptLocation_LAB();
                //}
                //else
                //{
                //    return AppointmentProvider.Instance.AllDeptLocation_LAB();
                //}
                return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.AllDeptLocation_LAB();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading location list. Status: Failed.", CurrentUser);

                AxException axErr = new AxException(ex, ErrorNames.APPOINTMENT_LOCATION_LIST_CANNOT_LOAD);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<DeptLocation> AllDeptLocation_NotLAB(long V_PCLMainCategory, long PCLExamTypeSubCategoryID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.AllDeptLocation_NotLAB(V_PCLMainCategory, PCLExamTypeSubCategoryID);
                //}
                //else
                //{
                //    return AppointmentProvider.Instance.AllDeptLocation_NotLAB(V_PCLMainCategory, PCLExamTypeSubCategoryID);
                //}
                return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.AllDeptLocation_NotLAB(V_PCLMainCategory, PCLExamTypeSubCategoryID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading location list. Status: Failed.", CurrentUser);

                AxException axErr = new AxException(ex, ErrorNames.APPOINTMENT_LOCATION_LIST_CANNOT_LOAD);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public List<PatientApptPCLRequestDetails> PatientApptPCLRequestDetails_ByID(long AppointmentID, long PatientPCLReqID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.PatientApptPCLRequestDetails_ByID(AppointmentID, PatientPCLReqID);
                //}
                //else
                //{
                //    return AppointmentProvider.Instance.PatientApptPCLRequestDetails_ByID(AppointmentID, PatientPCLReqID);
                //}
                return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.PatientApptPCLRequestDetails_ByID(AppointmentID, PatientPCLReqID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading location list. Status: Failed.", CurrentUser);

                AxException axErr = new AxException(ex, ErrorNames.PatientApptPCLRequestDetails_LOAD);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        /// <summary>
        /// Trả về chỉ tiêu của phòng nào cho ca nào
        /// </summary>
        /// <param name="curDate"></param>
        /// <param name="crt"></param>
        /// <returns></returns>
        private int GetTargetNumberOfCases(DateTime curDate, ConsultationRoomTarget crt)
        {
            switch (curDate.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return crt.MondayTargetNumberOfCases;

                case DayOfWeek.Tuesday:
                    return crt.TuesdayTargetNumberOfCases;

                case DayOfWeek.Wednesday:
                    return crt.WednesdayTargetNumberOfCases;

                case DayOfWeek.Thursday:
                    return crt.ThursdayTargetNumberOfCases;

                case DayOfWeek.Friday:
                    return crt.FridayTargetNumberOfCases;

                case DayOfWeek.Saturday:
                    return crt.SaturdayTargetNumberOfCases;

                case DayOfWeek.Sunday:
                    return crt.SundayTargetNumberOfCases;
                default: return 0;
            }
        }

        private ConsultationRoomTarget GetConsultationRoomTarget(List<ConsultationRoomTarget> allConsultationRoomTarget
            , long DeptLocationID, long ConsultationTimeSegmentID)
        {
            return allConsultationRoomTarget.Where(o => o.ConsultationTimeSegmentID == ConsultationTimeSegmentID
                && o.DeptLocationID == DeptLocationID).FirstOrDefault();
        }
        private bool CheckOverTarget(int seqNum, DateTime curDate, List<ConsultationRoomTarget> allConsultationRoomTarget
            , long DeptLocationID, long ConsultationTimeSegmentID, out V_AppointmentError appointmentError)
        {
            appointmentError = V_AppointmentError.mNone;
            var CRTarget = GetConsultationRoomTarget(allConsultationRoomTarget, DeptLocationID, ConsultationTimeSegmentID);
            if (CRTarget == null)
            {
                appointmentError = V_AppointmentError.mNotTarget;
                return false;
            }
            int target = GetTargetNumberOfCases(curDate, CRTarget);
            if (seqNum > target)
            {
                appointmentError = V_AppointmentError.mOverTarget;
                return false;
            }
            return true;
        }

        public bool PatientAppointments_Save(PatientAppointment ObjPatientAppointment
            , bool PassCheckFullTarget
            , long? IssueID
            , out long AppointmentID
            , out string ErrorDetail
            , out string ListNotConfig
            , out string ListTargetFull
            , out string ListMax
            , out string ListRequestID)
        {
            try
            {
                AppointmentID = 0;

                //ErrorDetail = "";
                //// Cấp Số TT hẹn bệnh cho dịch vụ ở đây
                //// Kiểm tra có vượt chỉ tiêu chưa
                ////ClinicManagementProvider.instance.GetAllConsultationRoomTargetCache();
                //var provider = new ServiceSequenceNumberProvider();
                //bool flag = true;
                //if (ObjPatientAppointment.ObjApptServiceDetailsList_Add!=null
                //    && ObjPatientAppointment.ObjApptServiceDetailsList_Add.Count>0)
                //{
                //    foreach (var item in ObjPatientAppointment.ObjApptServiceDetailsList_Add)
                //    {
                //        uint sequenceNo = 0;
                //        byte sequenceNoType = 0;
                //        string ErrorMessage = "";
                //        byte ErrorNumber = 0;
                //        sequenceNo = (uint)item.ServiceSeqNum;
                //        //Goi dll cap so cua a tuan
                //        provider.GetApptDetailServiceSequenceNumber((uint)item.DeptLocationID, (byte)item.ApptTimeSegmentID
                //                                                        , ObjPatientAppointment.ApptDate.Value.ToShortDateString(), out sequenceNoType, out sequenceNo
                //                                                        , out ErrorNumber, out ErrorMessage);
                //        if (ErrorNumber>0)
                //        {
                //            switch (ErrorMessage)
                //            {
                //                case "NT":
                //                    ErrorDetail = "Chưa đặt chỉ tiêu cho phòng";
                //                    break;
                //                case "OT":
                //                    ErrorDetail = "Phòng " + item.DeptLocation.Location.LocationName + " đã vượt quá chỉ tiêu";
                //                    break;
                //            }
                //            flag = false;
                //            break;
                //        }
                //        else
                //        {
                //            item.ServiceSeqNum = (short)sequenceNo;
                //        }
                //    }
                //}

                //if(!flag)
                //{
                //    ListNotConfig = "";
                //    ListTargetFull = "";
                //    ListMax = "";
                //    ListRequestID = "";
                //    return false;
                //}

                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.PatientAppointments_Save(
                //                                                                        ObjPatientAppointment,
                //                                                                        PassCheckFullTarget,
                //                                                                        out AppointmentID, out ErrorDetail,
                //                                                                        out ListNotConfig, out ListTargetFull, out ListMax, out ListRequestID);
                //}
                //else
                //{
                //    return AppointmentProvider.Instance.PatientAppointments_Save(
                //        ObjPatientAppointment,
                //        PassCheckFullTarget,
                //        out AppointmentID, out ErrorDetail,
                //        out ListNotConfig, out ListTargetFull, out ListMax, out ListRequestID);
                //}
                return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.PatientAppointments_Save(
                                                                                        ObjPatientAppointment,
                                                                                        PassCheckFullTarget,
                                                                                        IssueID,
                                                                                        out AppointmentID, out ErrorDetail,
                                                                                        out ListNotConfig, out ListTargetFull, out ListMax, out ListRequestID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of adding appointment. Status: Failed.", CurrentUser);

                AxException axErr = new AxException(ex, ErrorNames.APPOINTMENT_LIST_CANNOT_INSERT);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }


        public bool PatientApptPCLRequests_UpdateTemplate(PatientApptPCLRequests ObjPatientAppointment)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.PatientApptPCLRequests_UpdateTemplate(ObjPatientAppointment);
                //}
                //else
                //{
                //    return AppointmentProvider.Instance.PatientApptPCLRequests_UpdateTemplate(ObjPatientAppointment);
                //}
                return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.PatientApptPCLRequests_UpdateTemplate(ObjPatientAppointment);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of adding appointment. Status: Failed.", CurrentUser);

                AxException axErr = new AxException(ex, ErrorNames.APPOINTMENT_LIST_CANNOT_INSERT);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<Holiday> GetAllHoliday()
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.GetAllHoliday();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Get all Holiday. Status: Failed.", CurrentUser);
                AxException axErr = new AxException(ex, ErrorNames.APPOINTMENT_LIST_CANNOT_INSERT);
                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #region ApptService
        //public List<RefDepartments> RefMedicalServiceItems_GetDeptIDIsKhamBenhNotPCL()
        //{
        //    try
        //    {
        //        return AppointmentProvider.Instance.RefMedicalServiceItems_GetDeptIDIsKhamBenhNotPCL();
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of retrieving RefMedicalServiceItems_GetDeptIDIsKhamBenhNotPCL. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.APPOINTMENT_RefMedicalServiceItems_GetDeptIDIsKhamBenhNotPCL);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}

        public List<RefMedicalServiceItem> RefMedicalServiceItems_GetByDeptIDIsKhamBenhNotPCL_Paging(RefMedicalServiceItemsSearchCriteria Criteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.RefMedicalServiceItems_GetByDeptIDIsKhamBenhNotPCL_Paging(Criteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    return AppointmentProvider.Instance.RefMedicalServiceItems_GetByDeptIDIsKhamBenhNotPCL_Paging(Criteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.RefMedicalServiceItems_GetByDeptIDIsKhamBenhNotPCL_Paging(Criteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving RefMedicalServiceItems_GetByDeptIDIsKhamBenhNotPCL_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.APPOINTMENT_RefMedicalServiceItems_GetByDeptIDIsKhamBenhNotPCL_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PatientApptServiceDetails> PatientApptServiceDetailsGetAll(PatientApptServiceDetailsSearchCriteria searchCriteria,
                                                                            int PageIndex,
                                                                             int PageSize,
                                                                             string OrderBy,
                                                                             bool CountTotal,
                                                                             out int Total)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.PatientApptServiceDetailsGetAll(searchCriteria,
                //                                                                    PageIndex,
                //                                                                    PageSize,
                //                                                                    OrderBy,
                //                                                                    CountTotal,
                //                                                                     out Total);
                //}
                //else
                //{
                //    return AppointmentProvider.Instance.PatientApptServiceDetailsGetAll(searchCriteria,
                //                                                                    PageIndex,
                //                                                                    PageSize,
                //                                                                    OrderBy,
                //                                                                    CountTotal,
                //                                                                     out Total);
                //}
                return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.PatientApptServiceDetailsGetAll(searchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving RefMedicalServiceItems_GetByDeptIDIsKhamBenhNotPCL_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.APPOINTMENT_RefMedicalServiceItems_GetByDeptIDIsKhamBenhNotPCL_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<Lookup> ApptService_GetByMedServiceID(long MedServiceID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.ApptService_GetByMedServiceID(MedServiceID);
                //}
                //else
                //{
                //    return AppointmentProvider.Instance.ApptService_GetByMedServiceID(MedServiceID);
                //}
                return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.ApptService_GetByMedServiceID(MedServiceID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving ApptService_GetByMedServiceID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.APPOINTMENT_ApptService_GetByMedServiceID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void ApptService_XMLSave(long MedServiceID, IEnumerable<Lookup> ObjList, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.ApptService_XMLSave(MedServiceID, ObjList, out Result);
                //}
                //else
                //{
                //    AppointmentProvider.Instance.ApptService_XMLSave(MedServiceID, ObjList, out Result);
                //}
                aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.ApptService_XMLSave(MedServiceID, ObjList, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving ApptService_GetByMedServiceID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.APPOINTMENT_ApptService_XMLSave);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region "PatientApptLocTargets"
        public List<PatientApptLocTargets> PatientApptLocTargetsByDepartmentLocID(long DepartmentLocID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.PatientApptLocTargetsByDepartmentLocID(DepartmentLocID);
                //}
                //else
                //{
                //    return AppointmentProvider.Instance.PatientApptLocTargetsByDepartmentLocID(DepartmentLocID);
                //}
                return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.PatientApptLocTargetsByDepartmentLocID(DepartmentLocID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading location list. Status: Failed.", CurrentUser);

                AxException axErr = new AxException(ex, ErrorNames.PatientApptLocTargets_LOAD);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void PatientApptLocTargets_Save(PatientApptLocTargets Obj, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.PatientApptLocTargets_Save(Obj, out Result);
                //}
                //else
                //{
                //    AppointmentProvider.Instance.PatientApptLocTargets_Save(Obj, out Result);
                //}
                aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.PatientApptLocTargets_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PatientApptLocTargets_Save. Status: Failed.", CurrentUser);

                AxException axErr = new AxException(ex, ErrorNames.PatientApptLocTargets_Save);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool PatientApptLocTargets_Delete(long PatientApptTargetID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.PatientApptLocTargets_Delete(PatientApptTargetID);
                //}
                //else
                //{
                //    return AppointmentProvider.Instance.PatientApptLocTargets_Delete(PatientApptTargetID);
                //}
                return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.PatientApptLocTargets_Delete(PatientApptTargetID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PatientApptLocTargets_Delete. Status: Failed.", CurrentUser);

                AxException axErr = new AxException(ex, ErrorNames.PatientApptLocTargets_Delete);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region HealthExaminationRecord
        public IList<HospitalClient> GetHospitalClients()
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.GetHospitalClients();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetHospitalClients. Status: Failed.", CurrentUser);
                AxException axErr = new AxException(ex, ErrorNames.APPOINTMENT_LIST_CANNOT_INSERT);
                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public HospitalClient EditHospitalClient(HospitalClient aHospitalClient, out bool ExCode)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.EditHospitalClient(aHospitalClient, out ExCode);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of EditHospitalClient. Status: Failed.", CurrentUser);
                AxException axErr = new AxException(ex, ErrorNames.APPOINTMENT_LIST_CANNOT_INSERT);
                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public HospitalClientContract EditHospitalClientContract(HospitalClientContract aClientContract, List<HosClientContractPatientGroup> PatientGroup, out bool ExCode)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.EditHospitalClientContract(aClientContract, PatientGroup, out ExCode);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of EditHospitalClientContract. Status: Failed.", CurrentUser);
                AxException axErr = new AxException(ex, ErrorNames.APPOINTMENT_LIST_CANNOT_INSERT);
                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<HospitalClientContract> GetHospitalClientContracts(HospitalClientContract aClientContract)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.GetHospitalClientContracts(aClientContract);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetHospitalClientContracts. Status: Failed.", CurrentUser);
                AxException axErr = new AxException(ex, ErrorNames.APPOINTMENT_LIST_CANNOT_INSERT);
                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public HospitalClientContract GetHospitalClientContractDetails(HospitalClientContract aClientContract)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.GetHospitalClientContractDetails(aClientContract);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetHospitalClientContractDetails. Status: Failed.", CurrentUser);
                AxException axErr = new AxException(ex, ErrorNames.APPOINTMENT_LIST_CANNOT_INSERT);
                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool ActiveHospitalClientContract(long HosClientContractID, long StaffID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.ActiveHospitalClientContract(HosClientContractID, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ActiveHospitalClientContract. Status: Failed.", CurrentUser);
                AxException axErr = new AxException(ex, ErrorNames.APPOINTMENT_LIST_CANNOT_INSERT);
                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<HosClientContractPatient> GetContractPaidAmount(long HosClientContractID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.GetContractPaidAmount(HosClientContractID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetContractPaidAmount. Status: Failed.", CurrentUser);
                AxException axErr = new AxException(ex, ErrorNames.APPOINTMENT_LIST_CANNOT_INSERT);
                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public void CompleteHospitalClientContract(long HosClientContractID, long StaffID)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.CompleteHospitalClientContract(HosClientContractID, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of CompleteHospitalClientContract. Status: Failed.", CurrentUser);
                AxException axErr = new AxException(ex, ErrorNames.APPOINTMENT_LIST_CANNOT_INSERT);
                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public void FinalizeHospitalClientContract(long HosClientContractID, long StaffID, decimal TotalContractAmount, bool IsConfirmFinalized)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.FinalizeHospitalClientContract(HosClientContractID, StaffID, TotalContractAmount, IsConfirmFinalized);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of FinalizeHospitalClientContract. Status: Failed.", CurrentUser);
                AxException axErr = new AxException(ex, ErrorNames.APPOINTMENT_LIST_CANNOT_INSERT);
                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        #endregion

        public void SaveConsultationRoomStaffAllocationServiceList(long ConsultationRoomStaffAllocationServiceListID, string ConsultationRoomStaffAllocationServiceListTitle, long[] MedServiceIDCollection)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.SaveConsultationRoomStaffAllocationServiceList(ConsultationRoomStaffAllocationServiceListID, ConsultationRoomStaffAllocationServiceListTitle, MedServiceIDCollection);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SaveConsultationRoomStaffAllocationServiceList. Status: Failed.", CurrentUser);
                AxException axErr = new AxException(ex, ErrorNames.APPOINTMENT_LIST_CANNOT_INSERT);
                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<ConsultationRoomStaffAllocationServiceList> GetConsultationRoomStaffAllocationServiceLists()
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.GetConsultationRoomStaffAllocationServiceLists();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetConsultationRoomStaffAllocationServiceLists. Status: Failed.", CurrentUser);
                AxException axErr = new AxException(ex, ErrorNames.APPOINTMENT_LIST_CANNOT_INSERT);
                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void SaveHosClientContractPatientGroup(HosClientContractPatientGroup PatientGroup, out long OutHosClientContractPatientGroupID)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.SaveHosClientContractPatientGroup(PatientGroup, out OutHosClientContractPatientGroupID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of HosClientContractPatientGroup. Status: Failed.", CurrentUser);
                AxException axErr = new AxException(ex, ErrorNames.APPOINTMENT_LIST_CANNOT_INSERT);
                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<HosClientContractPatientGroup> GetHosClientContractPatientGroups(long HosClientContractID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.GetHosClientContractPatientGroups(HosClientContractID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetHosClientContractPatientGroups. Status: Failed.", CurrentUser);
                AxException axErr = new AxException(ex, ErrorNames.APPOINTMENT_LIST_CANNOT_INSERT);
                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<HospitalClientContract> GetContractName_ByHosClientID(long HosClientID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.GetContractName_ByHosClientID(HosClientID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetContractName_ByHosClientID. Status: Failed.", CurrentUser);
                AxException axErr = new AxException(ex, ErrorNames.APPOINTMENT_LIST_CANNOT_INSERT);
                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        #region API for MedPro
        public PatientAppointment MedPro_PatientAppointments_Save(PatientAppointment ObjPatientAppointment)
        {
            try
            {
                AxLogger.Instance.LogInfo("Bắt đầu lưu thông tin cuộc hẹn từ MedPro", CurrentUser);
                long AppointmentID = 0;
                AxLogger.Instance.LogInfo("Bắt đầu lấy ConsultationTimeSegmentID mặc định cho danh sách dịch vụ", CurrentUser);
                foreach (var item in ObjPatientAppointment.ObjApptServiceDetailsList_Add)
                {
                    ConsultationTimeSegments AppointmentSegmentsSelected = new ConsultationTimeSegments();
                    List<ConsultationTimeSegments> AppointmentSegmentsList = aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.Segments_WithAppDateDeptLocIDSeqNumber(item.DeptLocationID, (DateTime)ObjPatientAppointment.ApptDate);
                    AppointmentSegmentsSelected = AppointmentSegmentsList.Where(x => x.ApptdayMaxNumConsultationAllowed > 0 && x.NumberOfSeq < x.ApptdayMaxNumConsultationAllowed).FirstOrDefault();
                    item.ApptTimeSegmentID = (short)AppointmentSegmentsSelected.ConsultationTimeSegmentID;
                }
                AxLogger.Instance.LogInfo("Kết thúc lấy ConsultationTimeSegmentID mặc định cho danh sách dịch vụ. Thành công!", CurrentUser);
                AxLogger.Instance.LogInfo("Bắt đầu lưu thông tin cuộc hẹn", CurrentUser);
                bool IsSaveSuccessful = aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.PatientAppointments_Save(
                                                                                        ObjPatientAppointment,
                                                                                        true,
                                                                                        null,
                                                                                        out AppointmentID, out string ErrorDetail,
                                                                                        out string ListNotConfig, out string ListTargetFull,
                                                                                        out string ListMax, out string ListRequestID);
                AxLogger.Instance.LogInfo("Kết thúc lưu thông tin cuộc hẹn", CurrentUser);
                if (IsSaveSuccessful)
                {
                    AxLogger.Instance.LogInfo("Bắt đầu lấy thông tin cuộc hẹn", CurrentUser);
                    PatientAppointment appointment = new PatientAppointment();
                    appointment = aEMR.DataAccessLayer.Providers.AppointmentProvider.Instance.GetAppointmentByID(AppointmentID);
                    AxLogger.Instance.LogInfo("Kết thúc lấy thông tin cuộc hẹn. Thành công", CurrentUser);
                    return appointment;
                }
                else
                {
                    AxLogger.Instance.LogInfo("Kết thúc lưu cuộc hẹn từ MedPro. Không thành công.", CurrentUser);
                    string errMessage = "Kết thúc lưu cuộc hẹn từ MedPro. Không thành công. Không phát sinh exception";
                    AxException axErr = new AxException(new Exception(errMessage), ErrorNames.APPOINTMENT_LIST_CANNOT_INSERT);
                    throw new FaultException<AxException>(axErr, new FaultReason(errMessage));
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("Kết thúc lưu cuộc hẹn từ MedPro. Không thành công.", CurrentUser);
                AxException axErr = new AxException(ex, ErrorNames.APPOINTMENT_LIST_CANNOT_INSERT);
                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        #endregion
    }
}