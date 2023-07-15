using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using DataEntities;
using Service.Core.Common;
using System.Collections.Generic;
using eHCMSLanguage;
using aEMR.ServiceClient;
using aEMR.Infrastructure;
using aEMR.ViewContracts;
using aEMR.Infrastructure.Events;
using aEMR.Common;
using aEMR.DataContracts;
using aEMR.Common.Collections;
/*
 * 20200818 #001 TNHX: Đưa chẩn đoán của phiếu hẹn CLS từ màn hình vào k lấy từ EditingApptPCLRequest
 */
namespace aEMR.ConsultantEPrescription.ViewModels
{
    public partial class PatientPCLRequestEditImageViewModel
    {
        #region hen benh

        #region Lấy chẩn đoán cuối
        //private DiagnosisTreatment _diagnosisTreatment;
        //public DiagnosisTreatment DiagnosisTreatment
        //{
        //    get
        //    {
        //        return _diagnosisTreatment;
        //    }
        //    set
        //    {
        //        if (_diagnosisTreatment != value)
        //        {
        //            _diagnosisTreatment = value;
        //            NotifyOfPropertyChange(() => DiagnosisTreatment);
        //        }
        //    }
        //}

        private void DiagnosisTreatment_GetLast(long PatientID, long PtRegistrationID, long ServiceRecID)
        {
            IsWaitingLoadChanDoan = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDiagnosisTreatment_GetLast(PatientID, PtRegistrationID, ServiceRecID, Globals.DispatchCallback(asyncResult =>
                    {
                        try
                        {
                            var results = contract.EndDiagnosisTreatment_GetLast(asyncResult);

                            SetDiagnosisTreatmentForAppointment(results);

                            //if (DiagnosisTreatment == null)
                            //    DiagnosisTreatment = new DiagnosisTreatment();

                            //DiagnosisTreatment = results;

                            //string strDiag = eHCMSResources.Z1116_G1_ChuaXacDinh;
                            //if (!string.IsNullOrEmpty(DiagnosisTreatment.DiagnosisFinal))
                            //{
                            //    strDiag = DiagnosisTreatment.DiagnosisFinal;
                            //}
                            //else
                            //{
                            //    if (DiagnosisTreatment.Diagnosis != null)
                            //    {
                            //        strDiag = DiagnosisTreatment.Diagnosis;
                            //        _tempApptPCLRequest.ICD10List = DiagnosisTreatment.ICD10List.Trim();
                            //    }
                            //}

                            //if (!string.IsNullOrEmpty(DiagnosisTreatment.ICD10List))
                            //{
                            //    EditingApptPCLRequest.ICD10List = DiagnosisTreatment.ICD10List.Trim();
                            //    _tempApptPCLRequest.ICD10List = DiagnosisTreatment.ICD10List.Trim();
                            //}

                            //EditingApptPCLRequest.Diagnosis = strDiag.Trim();
                            //CurrentPclRequest.Diagnosis = strDiag.Trim();
                            //_tempApptPCLRequest.Diagnosis = DiagnosisTreatment.Diagnosis;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            IsWaitingLoadChanDoan = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }
        #endregion

        #region Lấy chẩn đoán nội trú

        private void SetDiagnosisTreatmentForAppointment(DiagnosisTreatment item)
        {
            string strDiag = eHCMSResources.Z1116_G1_ChuaXacDinh;
            if (!string.IsNullOrEmpty(item.DiagnosisFinal))
            {
                strDiag = item.DiagnosisFinal;
            }
            else
            {
                if (item.Diagnosis != null)
                {
                    strDiag = item.Diagnosis;
                }
            }

            if (!string.IsNullOrEmpty(item.ICD10List))
            {
                EditingApptPCLRequest.ICD10List = item.ICD10List.Trim();
                _tempApptPCLRequest.ICD10List = item.ICD10List.Trim();
            }
            CurrentPclRequest.Diagnosis = strDiag.Trim();
            EditingApptPCLRequest.Diagnosis = strDiag.Trim();
            _tempApptPCLRequest.Diagnosis = strDiag.Trim();
        }


        private void GetDiagnosisTreatment_InPt(long ServiceRecID)
        {
            IsWaitingLoadChanDoan = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDiagnosisTreatment_InPt(ServiceRecID, Globals.DispatchCallback(asyncResult =>
                    {
                        try
                        {
                            var results = contract.EndGetDiagnosisTreatment_InPt(asyncResult);

                            SetDiagnosisTreatmentForAppointment(results);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            IsWaitingLoadChanDoan = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }
        #endregion


        private PatientApptPCLRequests _tempApptPCLRequest;

        public PatientAppointment CurrentAppointment
        {
            get;
            set;
        }

        private PrescriptionNoteTemplates _curPrescriptionNoteTemplates;
        public PrescriptionNoteTemplates curPrescriptionNoteTemplates
        {
            get { return _curPrescriptionNoteTemplates; }
            set
            {
                _curPrescriptionNoteTemplates = value;
                NotifyOfPropertyChange(() => curPrescriptionNoteTemplates);

                if (_curPrescriptionNoteTemplates != null && _curPrescriptionNoteTemplates.PrescriptNoteTemplateID > 0)
                {
                    string str = EditingApptPCLRequest.ApptPCLNote;
                    if (string.IsNullOrEmpty(str))
                    {
                        str = curPrescriptionNoteTemplates.DetailsTemplate;
                    }
                    else
                    {
                        str = str + Environment.NewLine + curPrescriptionNoteTemplates.DetailsTemplate;
                    }

                    EditingApptPCLRequest.ApptPCLNote = str;
                }
            }
        }


        private PatientApptPCLRequests _editingApptPCLRequest;
        public PatientApptPCLRequests EditingApptPCLRequest
        {
            get { return _editingApptPCLRequest; }
            private set
            {
                _editingApptPCLRequest = value;
                NotifyOfPropertyChange(() => EditingApptPCLRequest);
                if (PCLRequestDetailsContent != null)
                {
                    pclApptRequestDetailsContent.PCLRequest = EditingApptPCLRequest;
                    pclApptRequestDetailsContent.IsEdit = IsEdit;
                }
                if (_editingApptPCLRequest != null)
                {
                    DoctorComments = _editingApptPCLRequest.DoctorComments;
                }
            }
        }

        public void AddEditApptPCLInit()
        {
            FormIsEnabled = true;
            FormInputIsEnabled = true;
            ObjStaff = Globals.LoggedUserAccount.Staff;
            pclApptRequestDetailsContent = Globals.GetViewModel<IEditApptPclRequestDetailList>();
            pclApptRequestDetailsContent.PCLRequest = EditingApptPCLRequest;
            InitControlsForExt();

            //KMx: Sau khi kiểm tra, thấy View này không sử dụng View IPatientMedicalRecords_ByPatientID (25/05/2014 14:48).
            //UC Header PMR
            //var uc3 = Globals.GetViewModel<IPatientMedicalRecords_ByPatientID>();
            //UCHeaderInfoPMR = uc3;

            //ActivateItem(uc3);
            //SelectedTimeSegment = new PCLTimeSegment { ParaclinicalTimeSegmentID = -1 };

            //LoadAppointmentSegmentsPCL();
            ObjGetDiagnosisTreatmentByPtID = new DiagnosisTreatment();
            PrescriptionNoteTemplates_GetAllPCLAppointment();
        }
        //public bool CanbtSubtractAll
        //{
        //    get
        //    {
        //        return EditingApptPCLRequest != null && EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList != null && EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList.Count >= 0;
        //    }
        //}

        //KMx: Phiếu CLS sau khi đã lưu, load lên chỉnh sửa, xóa 1 món CLS A thì COUNT() của phiếu đó không thay đổi, mà món A đó sẽ đổi thành trạng thái DELETED_MODIFIED.
        //Nếu số lượng trạng thái DELETED_MODIFIED = COUNT(), có nghĩa là trong phiếu đó ko còn món nào hết => Ko cho lưu (17/04/2014 16:11).
        public void OkCmd()
        {
            if (_tempApptPCLRequest == null || _editingApptPCLRequest == null
                || EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList == null || EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList.Count() <= 0
                || EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList.Where(x => x.EntityState == EntityState.DELETED_MODIFIED).Count() == EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList.Count())
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.A0414_G1_Msg_InfoChuaCoYCCLS), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            //KMx: Chỉ cập nhật "Yêu cầu" và "Lời dặn".
            PatientApptPCLRequests_UpdateTemplate(EditingApptPCLRequest);

            //▼====: #001
            _tempApptPCLRequest.Diagnosis = CurrentPclRequest.Diagnosis;
            //▲====: #001
            //_tempApptPCLRequest.DoctorComments = CurrentPclRequest.DoctorComments;
            _tempApptPCLRequest.DoctorComments = EditingApptPCLRequest.DoctorComments;
            _tempApptPCLRequest.ApptPCLNote = EditingApptPCLRequest.ApptPCLNote;
            _tempApptPCLRequest.ObjPatientApptPCLRequestDetailsList = _editingApptPCLRequest.ObjPatientApptPCLRequestDetailsList;
            Globals.EventAggregator.Publish(new ItemEdited<PatientApptPCLRequests> { Item = _tempApptPCLRequest, Source = this });
        }
        public void CancelCmd()
        {
            TryClose();
        }

        //KMx: Không còn sử dụng nữa. Chuyển hàm này vào trong hàm OkCmd()
        public void btUpdatePCLTemplate()
        {
            PatientApptPCLRequests_UpdateTemplate(EditingApptPCLRequest);
        }

        public void PrescriptionNoteTemplates_GetAllPCLAppointment()
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePrescriptionsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        var pnt = new PrescriptionNoteTemplates();
                        pnt.V_PrescriptionNoteTempType = AllLookupValues.V_PrescriptionNoteTempType.AppointmentPCLTemplate;
                        contract.BeginPrescriptionNoteTemplates_GetAllIsActive(pnt, Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<PrescriptionNoteTemplates> allItems = new ObservableCollection<PrescriptionNoteTemplates>();
                            try
                            {
                                allItems = contract.EndPrescriptionNoteTemplates_GetAllIsActive(asyncResult);

                                allPrescriptionNoteTemplates = new ObservableCollection<PrescriptionNoteTemplates>(allItems);

                                PrescriptionNoteTemplates firstItem = new PrescriptionNoteTemplates();
                                firstItem.PrescriptNoteTemplateID = -1;
                                firstItem.NoteDetails = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K0616_G1_ChonMau);
                                allPrescriptionNoteTemplates.Insert(0, firstItem);
                                curPrescriptionNoteTemplates = firstItem;

                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {

                }
            });
            t.Start();
        }

        public void StartEditing(long PatientID, PatientApptPCLRequests apptPCLRequest)
        {
            if (apptPCLRequest == null || apptPCLRequest.PatientAppointment == null)
            {
                MessageBox.Show(eHCMSResources.A0299_G1_Msg_InfoChon1YCCLS);
                return;
            }

            _tempApptPCLRequest = apptPCLRequest;

            EditingApptPCLRequest = _tempApptPCLRequest.DeepCopy();

            if (_editingApptPCLRequest != null)
            {
                CurrentAppointment = EditingApptPCLRequest.PatientAppointment;
            }

            //KMx: Nếu cuộc hẹn từ khám bệnh nội trú thì load chẩn đoán nội trú (26/01/2015 15:11).
            if (apptPCLRequest.PatientAppointment.CreatedByInPtRegis)
            {
                GetDiagnosisTreatment_InPt(apptPCLRequest.PatientAppointment.ServiceRecID.GetValueOrDefault());
            }
            else
            {
                DiagnosisTreatment_GetLast(PatientID, 0, apptPCLRequest.PatientAppointment == null ? 0 : apptPCLRequest.PatientAppointment.ServiceRecID.GetValueOrDefault());
            }
        }

        private void ListDeptLocationByPCLExamTypeID(PCLExamType examType)
        {
            try
            {
                //examType.ObjDeptLocationList = Globals.ListPclExamTypesAllPCLFormImages.Where
                //    (o => o.PCLExamTypeID == examType.PCLExamTypeID).FirstOrDefault().ObjDeptLocationList;
                examType.ObjDeptLocationList = Globals.ListPclExamTypesAllPCLFormImages.Where
                    (o => o.PCLExamTypeID == examType.PCLExamTypeID).FirstOrDefault().PCLExamTypeLocations.Select(i => i.DeptLocation).ToObservableCollection();
                if (examType.ObjDeptLocationList != null && examType.ObjDeptLocationList.Count > 0)
                {
                    var appReqDetail = new PatientApptPCLRequestDetails
                    {
                        ObjDeptLocID = examType.ObjDeptLocationList.Count > 1 ?
                        new DeptLocation
                        {
                            DeptLocationID = 0,
                            Location = new Location { LID = 0, LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2094_G1_ChonPg) }
                        } : examType.ObjDeptLocationList.FirstOrDefault(),
                        ObjPCLExamTypes = examType
                    };

                    EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList.Add(appReqDetail);
                }
                else
                {
                    var appReqDetail = new PatientApptPCLRequestDetails
                    {
                        ObjPCLExamTypes = examType,
                        ObjDeptLocID = new DeptLocation
                        {
                            DeptLocationID = 0
                        }
                    };

                    EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList.Add(appReqDetail);
                }

                if (PCLRequestDetailsContent != null)
                {
                    pclApptRequestDetailsContent.PCLRequest = EditingApptPCLRequest;
                }
            }
            catch (Exception)
            {

            }
        }

        private void ListDeptLocation_ByPCLExamTypeID(PCLExamType examType)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginListDeptLocation_ByPCLExamTypeID(examType.PCLExamTypeID, Globals.DispatchCallback(asyncResult =>
                    {
                        try
                        {
                            var results = contract.EndListDeptLocation_ByPCLExamTypeID(asyncResult);

                            examType.ObjDeptLocationList = new ObservableCollection<DeptLocation>(results);
                            if (examType.ObjDeptLocationList != null && examType.ObjDeptLocationList.Count > 0)
                            {
                                var appReqDetail = new PatientApptPCLRequestDetails
                                {
                                    ObjDeptLocID = (examType.ObjDeptLocationList.Count > 1) ?
                                    new DeptLocation
                                    {
                                        DeptLocationID = 0,
                                        Location = new Location { LID = 0, LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2094_G1_ChonPg) }
                                    } : examType.ObjDeptLocationList.FirstOrDefault(),
                                    ObjPCLExamTypes = examType
                                };

                                EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList.Add(appReqDetail);
                            }

                            if (PCLRequestDetailsContent != null)
                            {
                                pclApptRequestDetailsContent.PCLRequest = EditingApptPCLRequest;
                            }
                            pclApptRequestDetailsContent.RefreshView();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                    }), null);

                }

            });

            t.Start();
        }

        public void PatientApptPCLRequests_UpdateTemplate(PatientApptPCLRequests apptPCLRequest)
        {
            if (apptPCLRequest == null || apptPCLRequest.PatientPCLReqID < 1)
            {
                return;
            }
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new AppointmentServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPatientApptPCLRequests_UpdateTemplate(apptPCLRequest, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var res = contract.EndPatientApptPCLRequests_UpdateTemplate(asyncResult);
                                //KMx: Đây là function cập nhật "Yêu cầu" và "Lời dặn". Nếu để câu thông báo "Cập nhật thành công", người dùng sẽ lầm tưởng là cập nhật PCL thành công. Nên chỉ thông báo khi cập nhật không thành công.
                                if (!res)
                                {
                                    MessageBox.Show(string.Format("{0}!", eHCMSResources.A0270_G1_Msg_CNhatCDoan_YCFail));
                                }
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {

                }
            });
            t.Start();
        }
        #endregion
    }
}