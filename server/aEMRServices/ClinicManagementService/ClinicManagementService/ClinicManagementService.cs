/*
* 20161223 #001 CMN: Add file manager
* 20220812 #002 QTD: Add SearchPatientMedicalFileManager, GetPatientMedicalFileStorageCheckInCheckOut
* 20221116 #003 BLQ: Thêm chức năng lịch làm việc ngoài giờ
*/
using System;
using System.Collections.Generic;

using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;

using AxLogging;
using DataEntities;
using ErrorLibrary.Resources;

using ErrorLibrary;
using eHCMSLanguage;
using System.Collections.ObjectModel;
using eHCMS.Configurations;

namespace ClinicManagementService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [KnownType(typeof(AxException))]
    public class ClinicManagementService : eHCMS.WCFServiceCustomHeader, IClinicManagementService
    {

        public ClinicManagementService()
        {
            int currentID = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }

        public bool InsertConsultationTimeSegments(string SegmentName
                                                      , string SegmentDescription
                                                      , DateTime StartTime
                                                      , DateTime EndTime
                                                      , DateTime? StartTime2
                                                      , DateTime? EndTime2
                                                      , bool IsActive)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.InsertConsultationTimeSegments(SegmentName
                //                                          , SegmentDescription
                //                                          , StartTime
                //                                          , EndTime
                //                                          , IsActive);
                //}
                //else
                //{
                //    return ClinicManagementProvider.instance.InsertConsultationTimeSegments(SegmentName
                //                                          , SegmentDescription
                //                                          , StartTime
                //                                          , EndTime
                //                                          , IsActive);
                //}
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.InsertConsultationTimeSegments(SegmentName, SegmentDescription, StartTime, EndTime, StartTime2, EndTime2, IsActive);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of INSERTCONSULTATIONTIMESEGMENTS. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, ErrorNames.CLINIC_INSERT_CONSULTATION_TIMESEGMENTS);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void EditStaffConsultationTimeSegments(string SegmentXmlContent, long DeptLocationID)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.EditStaffConsultationTimeSegments(SegmentXmlContent, DeptLocationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of EditStaffConsultationTimeSegments. Status: Failed.", CurrentUser);
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, ErrorNames.CLINIC_INSERT_CONSULTATION_TIMESEGMENTS);
                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool UpdateConsultationTimeSegments(ObservableCollection<ConsultationTimeSegments> consultationTimeSegments)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.UpdateConsultationTimeSegments(ConsultationTimeSegmentID
                //                                                , SegmentName, SegmentDescription, StartTime, EndTime);
                //}
                //else
                //{
                //    return ClinicManagementProvider.instance.UpdateConsultationTimeSegments(ConsultationTimeSegmentID
                //                                                , SegmentName, SegmentDescription, StartTime, EndTime);
                //}
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.UpdateConsultationTimeSegments(consultationTimeSegments);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UPDATECONSULTATIONTIMESEGMENTS. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, ErrorNames.CLINIC_UPDATE_CONSULTATION_TIMESEGMENTS);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteConsultationTimeSegments(long ConsultationTimeSegmentID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.DeleteConsultationTimeSegments(ConsultationTimeSegmentID);
                //}
                //else
                //{
                //    return ClinicManagementProvider.instance.DeleteConsultationTimeSegments(ConsultationTimeSegmentID);
                //}
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.DeleteConsultationTimeSegments(ConsultationTimeSegmentID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DELETECONSULTATIONTIMESEGMENTS. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, ErrorNames.CLINIC_DELETE_CONSULTATION_TIMESEGMENTS);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<ConsultationTimeSegments> GetAllConsultationTimeSegments()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetAllConsultationTimeSegments();
                //}
                //else
                //{
                //    return ClinicManagementProvider.instance.GetAllConsultationTimeSegments();
                //}
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetAllConsultationTimeSegments();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GETALLCONSULTATIONTIMESEGMENTS. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, ErrorNames.CLINIC_GETALL_CONSULTATION_TIMESEGMENTS);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }
        public List<PCLTimeSegment> GetAllPclTimeSegments()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetAllPCLTimeSegments();
                //}
                //else
                //{
                //    return ClinicManagementProvider.instance.GetAllPCLTimeSegments();
                //}
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetAllPCLTimeSegments();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Get all PclTimeSegments. Status: Failed.", CurrentUser);

                var curMethod = MethodBase.GetCurrentMethod();
                var axErr = new AxException(ex, curMethod, ErrorNames.CLINIC_GETALL_PCL_TIMESEGMENTS);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }
        public List<ConsultationTimeSegments> ConsultationTimeSegments_ByDeptLocationID(long DeptLocationID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.ConsultationTimeSegments_ByDeptLocationID(DeptLocationID);
                //}
                //else
                //{
                //    return ClinicManagementProvider.instance.ConsultationTimeSegments_ByDeptLocationID(DeptLocationID);
                //}
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.ConsultationTimeSegments_ByDeptLocationID(DeptLocationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GETALLCONSULTATIONTIMESEGMENTS. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, ErrorNames.CLINIC_GETALL_CONSULTATION_TIMESEGMENTS);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }

        public bool InsertConsultationRoomTarget(ConsultationRoomTarget consultationRoomTarget)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.InsertConsultationRoomTarget(consultationRoomTarget);
                //}
                //else
                //{
                //    return ClinicManagementProvider.instance.InsertConsultationRoomTarget(consultationRoomTarget);
                //}
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.InsertConsultationRoomTarget(consultationRoomTarget);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of INSERTCONSULTATIONROOMTARGET. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, ErrorNames.CLINIC_INSERT_CONSULTATION_ROOM_TARGET);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool InsertConsultationRoomTargetXML(List<ConsultationRoomTarget> lstCRSA)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.InsertConsultationRoomTargetXML(lstCRSA);
                //}
                //else
                //{
                //    return ClinicManagementProvider.instance.InsertConsultationRoomTargetXML(lstCRSA);
                //}
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.InsertConsultationRoomTargetXML(lstCRSA);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of INSERTCONSULTATIONROOMTARGET. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, ErrorNames.CLINIC_INSERT_CONSULTATION_ROOM_TARGET);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateConsultationRoomTargetXML(IList<ConsultationRoomTarget> lstCRSA)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.UpdateConsultationRoomTargetXML(lstCRSA);
                //}
                //else
                //{
                //    return ClinicManagementProvider.instance.UpdateConsultationRoomTargetXML(lstCRSA);
                //}
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.UpdateConsultationRoomTargetXML(lstCRSA);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateConsultationRoomTargetXML. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, ErrorNames.CLINIC_INSERT_CONSULTATION_ROOM_TARGET);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteConsultationRoomTarget(long ConsultationRoomTargetID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.DeleteConsultationRoomTarget(ConsultationRoomTargetID);
                //}
                //else
                //{
                //    return ClinicManagementProvider.instance.DeleteConsultationRoomTarget(ConsultationRoomTargetID);
                //}
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.DeleteConsultationRoomTarget(ConsultationRoomTargetID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of INSERTCONSULTATIONROOMTARGET. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, ErrorNames.CLINIC_INSERT_CONSULTATION_ROOM_TARGET);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<ConsultationRoomTarget> GetConsultationRoomTargetByDeptID(long DeptLocationID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetConsultationRoomTargetByDeptID(DeptLocationID);
                //}
                //else
                //{
                //    return ClinicManagementProvider.instance.GetConsultationRoomTargetByDeptID(DeptLocationID);
                //}
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetConsultationRoomTargetByDeptID(DeptLocationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GETCONSULTATIONROOMTARGETBYDEPTID. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, ErrorNames.CLINIC_GET_CONSULTATION_ROOMTARGET_BY_DEPTID);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<ConsultationRoomTarget> GetConsultationRoomTargetTimeSegment(long DeptLocationID, long ConsultationTimeSegmentID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetConsultationRoomTargetTimeSegment(DeptLocationID, ConsultationTimeSegmentID);
                //}
                //else
                //{
                //    return ClinicManagementProvider.instance.GetConsultationRoomTargetTimeSegment(DeptLocationID, ConsultationTimeSegmentID);
                //}
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetConsultationRoomTargetTimeSegment(DeptLocationID, ConsultationTimeSegmentID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GETCONSULTATIONROOMTARGETTIMESEGMENT. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, ErrorNames.CLINIC_GET_CONSULTATION_ROOMTARGET_TIMESEGMENT);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<ConsultationRoomTarget> GetConsultationRoomTargetTSegment(out DateTime curDate, long DeptLocationID, bool IsHis)
        {
            try
            {
                curDate = DateTime.Now;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetConsultationRoomTargetTSegment(DeptLocationID, IsHis);
                //}
                //else
                //{
                //    return ClinicManagementProvider.instance.GetConsultationRoomTargetTSegment(DeptLocationID, IsHis);
                //}
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetConsultationRoomTargetTSegment(DeptLocationID, IsHis);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GETCONSULTATIONROOMTARGETTIMESEGMENT. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, ErrorNames.CLINIC_GET_CONSULTATION_ROOMTARGET_TIMESEGMENT);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool InsertConsultationRoomStaffAllocations(long DeptLocationID
                                                  , long ConsultationTimeSegmentID
                                                  , long StaffID
                                                  , long StaffCatgID
                                                  , DateTime AllocationDate)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.InsertConsultationRoomStaffAllocations(DeptLocationID
                //                                                                      , ConsultationTimeSegmentID
                //                                                                      , StaffID
                //                                                                      , StaffCatgID
                //                                                                      , AllocationDate);
                //}
                //else
                //{
                //    return ClinicManagementProvider.instance.InsertConsultationRoomStaffAllocations(DeptLocationID
                //                                                                      , ConsultationTimeSegmentID
                //                                                                      , StaffID
                //                                                                      , StaffCatgID
                //                                                                      , AllocationDate);
                //}
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.InsertConsultationRoomStaffAllocations(DeptLocationID
                                                                                      , ConsultationTimeSegmentID
                                                                                      , StaffID
                                                                                      , StaffCatgID
                                                                                      , AllocationDate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of INSERTCONSULTATIONROOMSTAFFALLOCATIONS. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, ErrorNames.CLINIC_INSERT_CONSULTATION_ROOM_STAFFALLOCATIONS);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool InsertConsultationRoomStaffAllocationsXML(IList<ConsultationRoomStaffAllocations> lstCRSA)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.InsertConsultationRoomStaffAllocationsXML(lstCRSA);
                //}
                //else
                //{
                //    return ClinicManagementProvider.instance.InsertConsultationRoomStaffAllocationsXML(lstCRSA);
                //}
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.InsertConsultationRoomStaffAllocationsXML(lstCRSA);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of INSERTCONSULTATIONROOMSTAFFALLOCATIONS. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, ErrorNames.CLINIC_INSERT_CONSULTATION_ROOM_STAFFALLOCATIONS);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool UpdateConsultationRoomStaffAllocations(long ConsultationRoomStaffAllocID, bool IsActive)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.UpdateConsultationRoomStaffAllocations(ConsultationRoomStaffAllocID, IsActive);
                //}
                //else
                //{
                //    return ClinicManagementProvider.instance.UpdateConsultationRoomStaffAllocations(ConsultationRoomStaffAllocID, IsActive);
                //}
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.UpdateConsultationRoomStaffAllocations(ConsultationRoomStaffAllocID, IsActive);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UPDATECONSULTATIONROOMSTAFFALLOCATIONS. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, ErrorNames.CLINIC_UPDATE_CONSULTATION_ROOM_STAFFALLOCATIONS);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateConsultationRoomStaffAllocationsXML(IList<ConsultationRoomStaffAllocations> lstCRSA)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.UpdateConsultationRoomStaffAllocationsXML(lstCRSA);
                //}
                //else
                //{
                //    return ClinicManagementProvider.instance.UpdateConsultationRoomStaffAllocationsXML(lstCRSA);
                //}
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.UpdateConsultationRoomStaffAllocationsXML(lstCRSA);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UPDATECONSULTATIONROOMSTAFFALLOCATIONS. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, ErrorNames.CLINIC_UPDATE_CONSULTATION_ROOM_STAFFALLOCATIONS);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<ConsultationRoomStaffAllocations> GetConsultationRoomStaffAllocations(long DeptLocationID, long ConsultationTimeSegmentID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetConsultationRoomStaffAllocations(DeptLocationID, ConsultationTimeSegmentID);
                //}
                //else
                //{
                //    return ClinicManagementProvider.instance.GetConsultationRoomStaffAllocations(DeptLocationID, ConsultationTimeSegmentID);
                //}
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetConsultationRoomStaffAllocations(DeptLocationID, ConsultationTimeSegmentID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GETCONSULTATIONROOMSTAFFALLOCATIONS. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, ErrorNames.CLINIC_GET_CONSULTATION_ROOM_STAFFALLOCATIONS);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<ConsultationRoomStaffAllocations> GetConsultationRoomStaffAllocations_ForXML(IList<DeptLocation> lstDeptLocation, long ConsultationTimeSegmentID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetConsultationRoomStaffAllocations_ForXML(lstDeptLocation, ConsultationTimeSegmentID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GETCONSULTATIONROOMSTAFFALLOCATIONS. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, ErrorNames.CLINIC_GET_CONSULTATION_ROOM_STAFFALLOCATIONS);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<ConsultationRoomStaffAllocations> GetStaffConsultationTimeSegmentByDate(long DeptLocationID, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetStaffConsultationTimeSegmentByDate(DeptLocationID, FromDate, ToDate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetStaffConsultationTimeSegmentByDate. Status: Failed.", CurrentUser);
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, ErrorNames.CLINIC_GET_CONSULTATION_ROOM_STAFFALLOCATIONS);
                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool DeleteConsultationRoomStaffAllocations(long DeptLocationID, long ConsultationTimeSegmentID
                                , DateTime AllocationDate)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.DeleteConsultationRoomStaffAllocations(DeptLocationID, ConsultationTimeSegmentID, AllocationDate);
                //}
                //else
                //{
                //    return ClinicManagementProvider.instance.DeleteConsultationRoomStaffAllocations(DeptLocationID, ConsultationTimeSegmentID, AllocationDate);
                //}
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.DeleteConsultationRoomStaffAllocations(DeptLocationID, ConsultationTimeSegmentID, AllocationDate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of INSERTCONSULTATIONROOMTARGET. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, ErrorNames.CLINIC_INSERT_CONSULTATION_ROOM_TARGET);

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //==== #001
        #region FileManager
        public List<RefShelves> GetRefShelves(long RefShelfID, long RefRowID, string RefShelfName)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetRefShelves(RefShelfID, StoreID);
                //}
                //else
                //{
                //    return ClinicManagementProvider.instance.GetRefShelves(RefShelfID, StoreID);
                //}
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetRefShelves(RefShelfID, RefRowID, RefShelfName);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetRefShelf. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_GET_APPOINTMENTID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<RefShelves> UpdateRefShelves(long StaffID, List<RefShelves> ListRefShelves, List<RefShelves> ListRefShelvesDeleted, long RefRowID, bool IsPopup)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.UpdateRefShelves(StaffID, ListRefShelves, StoreID);
                //}
                //else
                //{
                //    return ClinicManagementProvider.instance.UpdateRefShelves(StaffID, ListRefShelves, StoreID);
                //}
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.UpdateRefShelves(StaffID, ListRefShelves, ListRefShelvesDeleted, RefRowID, IsPopup);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateRefShelves. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_GET_APPOINTMENTID);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public List<RefShelfDetails> GetRefShelfDetails(long RefShelfID, string LocCode, long RefShelfDetailID, string LocName)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetRefShelfDetails(RefShelfID, LocCode, StoreID);
                //}
                //else
                //{
                //    return ClinicManagementProvider.instance.GetRefShelfDetails(RefShelfID, LocCode, StoreID);
                //}
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetRefShelfDetails(RefShelfID, LocCode, RefShelfDetailID, LocName);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetRefShelfDetail. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_GET_APPOINTMENTID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool UpdateRefShelfDetails(long RefShelfID, long StaffID, List<RefShelfDetails> ListRefShelfDetail, List<RefShelfDetails> ListRefShelfDetailDeleted, bool IsPopup)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.UpdateRefShelfDetails(RefShelfID, ListRefShelfDetail);
                //}
                //else
                //{
                //    return ClinicManagementProvider.instance.UpdateRefShelfDetails(RefShelfID, ListRefShelfDetail);
                //}
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.UpdateRefShelfDetails(RefShelfID, StaffID, ListRefShelfDetail, ListRefShelfDetailDeleted, IsPopup);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateRefShelfDetail. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_GET_APPOINTMENTID);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public List<PatientMedicalFileStorage> GetPatientMedicalFileStorage(long RefShelfDetailID, string FileCodeNumber, out long PatientMedicalFileID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetPatientMedicalFileStorage(RefShelfDetailID, FileCodeNumber, out PatientMedicalFileID);
                //}
                //else
                //{
                //    return ClinicManagementProvider.instance.GetPatientMedicalFileStorage(RefShelfDetailID, FileCodeNumber, out PatientMedicalFileID);
                //}
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetPatientMedicalFileStorage(RefShelfDetailID, FileCodeNumber, out PatientMedicalFileID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetPatientMedicalFileStorage. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_GET_APPOINTMENTID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<PatientMedicalFileStorage> UpdatePatientMedicalFileStorage(long RefShelfDetailID, List<PatientMedicalFileStorage> ListPatientMedicalFileStorage, long StaffID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.UpdatePatientMedicalFileStorage(RefShelfDetailID, ListPatientMedicalFileStorage);
                //}
                //else
                //{
                //    return ClinicManagementProvider.instance.UpdatePatientMedicalFileStorage(RefShelfDetailID, ListPatientMedicalFileStorage);
                //}
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.UpdatePatientMedicalFileStorage(RefShelfDetailID, ListPatientMedicalFileStorage, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdatePatientMedicalFileStorage. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_GET_APPOINTMENTID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<PatientMedicalFileStorageCheckInCheckOut> GetPatientMedicalFileStorageCheckInCheckOut(string FileCodeNumber)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetPatientMedicalFileStorageCheckInCheckOut(FileCodeNumber);
                //}
                //else
                //{
                //    return ClinicManagementProvider.instance.GetPatientMedicalFileStorageCheckInCheckOut(FileCodeNumber);
                //}
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetPatientMedicalFileStorageCheckInCheckOut(FileCodeNumber);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetPatientMedicalFileStorageCheckInCheckOut. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_GET_APPOINTMENTID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<StaffPersons> GetAllStaffPersons()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetAllStaffPersons();
                //}
                //else
                //{
                //    return ClinicManagementProvider.instance.GetAllStaffPersons();
                //}
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetAllStaffPersons();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetAllStaffPersons. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_GET_APPOINTMENTID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool UpdatePatientMedicalFileStorageCheckOut(long StaffID, List<PatientMedicalFileStorageCheckInCheckOut> ListPatientMedicalFileStorageCheckOut, out long MedicalFileStorageCheckID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.UpdatePatientMedicalFileStorageCheckOut(StaffID, ListPatientMedicalFileStorageCheckOut, out MedicalFileStorageCheckID);
                //}
                //else
                //{
                //    return ClinicManagementProvider.instance.UpdatePatientMedicalFileStorageCheckOut(StaffID, ListPatientMedicalFileStorageCheckOut, out MedicalFileStorageCheckID);
                //}
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.UpdatePatientMedicalFileStorageCheckOut(StaffID, ListPatientMedicalFileStorageCheckOut, out MedicalFileStorageCheckID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdatePatientMedicalFileStorageCheckOut. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_GET_APPOINTMENTID);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public List<PatientMedicalFileStorageCheckInCheckOut> GetMedicalFileFromRegistration(long DeptID, long LocationID, DateTime StartDate, DateTime EndDate, bool IsBorrowed, bool IsStored)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetMedicalFileFromRegistration(DeptID, LocationID, StartDate, EndDate, IsBorrowed, IsStored);
                //}
                //else
                //{
                //    return ClinicManagementProvider.instance.GetMedicalFileFromRegistration(DeptID, LocationID, StartDate, EndDate, IsBorrowed, IsStored);
                //}
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetMedicalFileFromRegistration(DeptID, LocationID, StartDate, EndDate, IsBorrowed, IsStored);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetMedicalFileFromRegistration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_GET_APPOINTMENTID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public string GetXMLPatientMedicalFileStorages(List<PatientMedicalFileStorage> ListPatientMedicalFileStorage)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetXMLPatientMedicalFileStorages(ListPatientMedicalFileStorage);
                //}
                //else
                //{
                //    return ClinicManagementProvider.instance.GetXMLPatientMedicalFileStorages(ListPatientMedicalFileStorage);
                //}
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetXMLPatientMedicalFileStorages(ListPatientMedicalFileStorage);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetXMLPatientMedicalFileStorages. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_GET_APPOINTMENTID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<PatientMedicalFileStorageCheckInCheckOut> SearchMedicalFilesHistory(DateTime? StartDate, DateTime? EndDate, string FileCodeNumber, long? StaffID, long? ReceiveStaffID, int Status, int PageSize, int PageIndex, out int TotalRow)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.SearchMedicalFilesHistory(StartDate, EndDate, FileCodeNumber, StaffID, ReceiveStaffID, Status, PageSize, PageIndex, out TotalRow);
                //}
                //else
                //{
                //    return ClinicManagementProvider.instance.SearchMedicalFilesHistory(StartDate, EndDate, FileCodeNumber, StaffID, ReceiveStaffID, Status, PageSize, PageIndex, out TotalRow);
                //}
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.SearchMedicalFilesHistory(StartDate, EndDate, FileCodeNumber, StaffID, ReceiveStaffID, Status, PageSize, PageIndex, out TotalRow);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SearchMedicalFilesHistory. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_GET_APPOINTMENTID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public string GetRegistrationXMLFromMedicalFileList(List<PatientMedicalFileStorageCheckInCheckOut> ListItem)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetRegistrationXMLFromMedicalFileList(ListItem);
                //}
                //else
                //{
                //    return ClinicManagementProvider.instance.GetRegistrationXMLFromMedicalFileList(ListItem);
                //}
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetRegistrationXMLFromMedicalFileList(ListItem);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetRegistrationXMLFromMedicalFileList. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_GET_APPOINTMENTID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<PatientMedicalFileStorageCheckInCheckOut> GetMedicalFileStorageCheckOutHistory(DateTime? StartDate, DateTime? EndDate, long? StaffID, long? ReceiveStaffID, int PageSize, int PageIndex, out int TotalRow)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetMedicalFileStorageCheckOutHistory(StartDate, EndDate, StaffID, ReceiveStaffID, PageSize, PageIndex, out TotalRow);
                //}
                //else
                //{
                //    return ClinicManagementProvider.instance.GetMedicalFileStorageCheckOutHistory(StartDate, EndDate, StaffID, ReceiveStaffID, PageSize, PageIndex, out TotalRow);
                //}
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetMedicalFileStorageCheckOutHistory(StartDate, EndDate, StaffID, ReceiveStaffID, PageSize, PageIndex, out TotalRow);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetMedicalFileStorageCheckOutHistory. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_GET_APPOINTMENTID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<PatientMedicalFileStorageCheckInCheckOut> GetMedicalFileDetails(string FileCodeNumber, long? RefShelfDetailID, int PageSize, int PageIndex, out int TotalRow)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetMedicalFileDetails(FileCodeNumber, RefShelfDetailID, PageSize, PageIndex, out TotalRow);
                //}
                //else
                //{
                //    return ClinicManagementProvider.instance.GetMedicalFileDetails(FileCodeNumber, RefShelfDetailID, PageSize, PageIndex, out TotalRow);
                //}
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetMedicalFileDetails(FileCodeNumber, RefShelfDetailID, PageSize, PageIndex, out TotalRow);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetMedicalFileDetails. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_GET_APPOINTMENTID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion
        //==== #001
        public IList<PatientRegistration> SearchRegistrationsForOutMedicalFileManagement(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria, int ViewCase)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading SearchRegistrationsForOutMedicalFileManagement.", CurrentUser);
                IList<PatientRegistration> bRet;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    bRet = aEMR.DataAccessLayer.Providers.TransactionProvider.Instance.SearchRegistrationsForHIReport(aSeachPtRegistrationCriteria);
                //}
                //else
                //{
                //    bRet = TransactionProvider.Instance.SearchRegistrationsForHIReport(aSeachPtRegistrationCriteria);
                //}
                bRet = aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.SearchRegistrationsForOutMedicalFileManagement(aSeachPtRegistrationCriteria, ViewCase);
                AxLogger.Instance.LogInfo("End loading SearchRegistrationsForOutMedicalFileManagement.", CurrentUser);
                return bRet;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading SearchRegistrationsForOutMedicalFileManagement. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_CREATE, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▼==== #002
        public List<PatientMedicalFileStorage> SearchPatientMedicalFileManager(long V_MedicalFileType, string FileCodeNumber, string PatientName, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.SearchPatientMedicalFileManager(V_MedicalFileType, FileCodeNumber, PatientName, FromDate, ToDate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SearchPatientMedicalFileManager. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_GET_APPOINTMENTID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<PatientMedicalFileStorage> PatientMedicalFileStorage_InsertXML(long RefShelfDetailID, List<PatientMedicalFileStorage> ListPatientMedicalFileStorage, long StaffID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.PatientMedicalFileStorage_InsertXML(RefShelfDetailID, ListPatientMedicalFileStorage, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving PatientMedicalFileStorage_InsertXML. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_GET_APPOINTMENTID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<RefRows> UpdateRefRows(long StaffID, List<RefRows> ListRefRows, List<RefRows> ListRefRowDeleted, long StoreID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.UpdateRefRows(StaffID, ListRefRows, ListRefRowDeleted, StoreID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateRefRows. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_GET_APPOINTMENTID);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public List<RefRows> GetRefRows(long StoreID, string RefRowName, long RefRowID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetRefRows(StoreID, RefRowName, RefRowID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetRefRows. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_GET_APPOINTMENTID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<PatientMedicalFileStorageCheckInCheckOut> SearchMedicalFileDetails(long? StoreID, long? RefRowID, long? RefShelfID, long? RefShelfDetailID, long V_MedicalFileType, string FileCodeNumber,
            string PatientCode, string PatientName, long V_MedicalFileStatus, int PageSize, int PageIndex, out int TotalRow)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.SearchMedicalFileDetails(StoreID, RefRowID, RefShelfID, RefShelfDetailID, V_MedicalFileType, FileCodeNumber,
                    PatientCode, PatientName, V_MedicalFileStatus, PageSize, PageIndex, out TotalRow);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SearchMedicalFileDetails. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_GET_APPOINTMENTID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<PatientMedicalFileStorageCheckInCheckOut> GetPatientMedicalFileStorageCheckInCheckOut_V2(long StoreID, long RefRowID, long RefShelfID, long RefShelfDetailID, DateTime FromDate, DateTime ToDate,
            string PatientCode, string PatientName, string FileCodeNumber, bool IsCheckOut, int PageSize, int PageIndex, out int TotalRow)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetPatientMedicalFileStorageCheckInCheckOut_V2(StoreID, RefRowID, RefShelfID, RefShelfDetailID, FromDate, ToDate,
                PatientCode, PatientName, FileCodeNumber, IsCheckOut, PageSize, PageIndex, out TotalRow);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetPatientMedicalFileStorageCheckInCheckOut_V2. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_GET_APPOINTMENTID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool UpdatePatientMedicalFileStorageCheckOut_V2(long StaffID, long StaffIDCheckOut, long StaffPersonID, int BorrowingDay,
            string Notes, long V_ReasonType, bool IsCheckIn, List<PatientMedicalFileStorageCheckInCheckOut> ListPatientMedicalFileStorageCheckOut, out long MedicalFileStorageCheckID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.UpdatePatientMedicalFileStorageCheckOut_V2(StaffID, StaffIDCheckOut, StaffPersonID, BorrowingDay,
                    Notes, V_ReasonType, IsCheckIn, ListPatientMedicalFileStorageCheckOut, out MedicalFileStorageCheckID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdatePatientMedicalFileStorageCheckOut_V2. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_GET_APPOINTMENTID);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲==== #002
        //▼==== #003
        public OvertimeWorkingWeek GetOvertimeWorkingWeekByDate(int Week, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetOvertimeWorkingWeekByDate(Week, FromDate, ToDate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetOvertimeWorkingWeekByDate. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Error GetOvertimeWorkingWeekByDate");
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public List<OvertimeWorkingSchedule> GetOvertimeWorkingScheduleByDate(DateTime FromDate, DateTime ToDate)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetOvertimeWorkingScheduleByDate(FromDate, ToDate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetOvertimeWorkingScheduleByDate. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Error GetOvertimeWorkingScheduleByDate");
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public List<DeptLocation> GetLocationForOvertimeWorkingWeek()
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetLocationForOvertimeWorkingWeek();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetLocationForOvertimeWorkingWeek. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Error GetLocationForOvertimeWorkingWeek");
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public List<Staff> GetDoctorForOvertimeWorkingWeek()
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetDoctorForOvertimeWorkingWeek();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetDoctorForOvertimeWorkingWeek. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Error GetDoctorForOvertimeWorkingWeek");
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public bool SaveOvertimeWorkingSchedule(OvertimeWorkingWeek OTWObj, OvertimeWorkingSchedule OTSObj, long StaffID, DateTime DateUpdate)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.SaveOvertimeWorkingSchedule(OTWObj, OTSObj, StaffID, DateUpdate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SaveOvertimeWorkingSchedule. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Error SaveOvertimeWorkingSchedule");
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public bool DeleteOvertimeWorkingSchedule(long OvertimeWorkingScheduleID, long StaffID, DateTime DateUpdate)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.DeleteOvertimeWorkingSchedule(OvertimeWorkingScheduleID, StaffID, DateUpdate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DeleteOvertimeWorkingSchedule. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Error DeleteOvertimeWorkingSchedule");
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public bool UpdateOvertimeWorkingWeekStatus(OvertimeWorkingWeek OTWObj, long StaffID, DateTime DateUpdate)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.UpdateOvertimeWorkingWeekStatus(OTWObj, StaffID, DateUpdate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateOvertimeWorkingWeekStatus. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Error UpdateOvertimeWorkingWeekStatus");
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲==== #003
    }
}
