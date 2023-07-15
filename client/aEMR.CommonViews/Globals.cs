using aEMR.Common;
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.CommonTasks;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using DataEntities;
using eHCMS.Services.Core.Base;
using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using ZXing;
using ZXing.Common;
/*
 * 20200113 #001 TTM: BM 0021787: Fix lỗi không tính tiền miễn giảm hoặc giá trị bảo hiểm nếu đã lưu dịch vụ rồi.
 * 20200224 #015 TNHX: Lấy khoa đăng nhập làm khoa xem in thủ thuật nếu chẩn đoán trước của TT-PT khi nhập không chọn phòng
 */
namespace aEMR
{
    public partial class CommonGlobals
    {
        public static Bitmap GeneratorBarcodeImage(string aContent, System.Drawing.Size aSize, ZXing.BarcodeFormat aBarcodeFormat = ZXing.BarcodeFormat.CODE_128)
        {
            if (string.IsNullOrEmpty(aContent))
            {
                return null;
            }
            BarcodeWriter mBarcodeWriter = new BarcodeWriter
            {
                Format = aBarcodeFormat,
                Options = new EncodingOptions
                {
                    Width = aSize.Width,
                    Height = aSize.Height,
                    Margin = 0,
                    PureBarcode = true
                }
            };
            Bitmap mBitmap = mBarcodeWriter.Write(aContent);
            return mBitmap;
        }
        #region VNPTEInvoices
        public static void ExportInvoiceToPdfNoPay(string InvFkey, string aFilePath)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new VNPTAccountingPortalServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BegindownloadInvPDFFkeyNoPay(InvFkey, Globals.ServerConfigSection.CommonItems.eInvoiceAdminUserName, Globals.ServerConfigSection.CommonItems.eInvoiceAdminUserPass, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Base64Data = contract.EnddownloadInvPDFFkeyNoPay(asyncResult);
                            if (Base64Data.StartsWith("ERR"))
                            {
                                GlobalsNAV.ShowMessagePopup(Base64Data);
                                return;
                            }
                            Globals.SaveBase64StringToFile(Base64Data, aFilePath);
                            Globals.OpenFileWithWindowsExpore(aFilePath);
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
        public static void ImportPatientEInvoice(GenericCoRoutineTask aGenTask, object aPatient, object aTransactionFinalizationObj, object aLogView)
        {
            Patient mPatient = aPatient as Patient;
            OutPtTransactionFinalization TransactionFinalizationObj = aTransactionFinalizationObj as OutPtTransactionFinalization;
            ILoggerDialog mLogView = aLogView as ILoggerDialog;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new VNPTAccountingPublishServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    VNPTCustomer mVNPTCustomer = new VNPTCustomer(mPatient, TransactionFinalizationObj.TaxMemberAddress);
                    contract.BeginUpdateCus(mVNPTCustomer.ToXML(), Globals.ServerConfigSection.CommonItems.eInvoiceAdminUserName, Globals.ServerConfigSection.CommonItems.eInvoiceAdminUserPass, 0, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int jUpdateInfo = contract.EndUpdateCus(asyncResult);
                            if (jUpdateInfo <= 0)
                            {
                                if (VNPTAccountingPublishServiceClient.ErrorCodeDetails.ContainsKey(jUpdateInfo))
                                {
                                    mLogView.AppendLogMessage(VNPTAccountingPublishServiceClient.ErrorCodeDetails[jUpdateInfo]);
                                    aGenTask.Error = new Exception(VNPTAccountingPublishServiceClient.ErrorCodeDetails[jUpdateInfo]);
                                }
                                else
                                {
                                    mLogView.AppendLogMessage(eHCMSResources.T0074_G1_I + ": " + jUpdateInfo);
                                    aGenTask.Error = new Exception(eHCMSResources.T0074_G1_I + ": " + jUpdateInfo);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            mLogView.AppendLogMessage(ex.Message);
                            aGenTask.Error = ex;
                        }
                        finally
                        {
                            aGenTask.ActionComplete(true);
                        }
                    }), null);
                }
            });
            t.Start();
        }
        public static void AddEInvoice(GenericCoRoutineTask aGenTask, object aCustomer, object aTransactionFinalizationObj, object aRptOutPtTransactionFinalizationDetailCollection, object aLogView)
        {
            VNPTCustomer mCustomer = aCustomer as VNPTCustomer;
            OutPtTransactionFinalization TransactionFinalizationObj = aTransactionFinalizationObj as OutPtTransactionFinalization;
            List<RptOutPtTransactionFinalizationDetail> mRptOutPtTransactionFinalizationDetailCollection = aRptOutPtTransactionFinalizationDetailCollection as List<RptOutPtTransactionFinalizationDetail>;
            ILoggerDialog mLogView = aLogView as ILoggerDialog;
            string aPattern = TransactionFinalizationObj.Denominator;
            string aSerial = TransactionFinalizationObj.Symbol;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new VNPTAccountingPublishServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginImportAndPublishInv(Globals.ServerConfigSection.CommonItems.eInvoiceAccountUserName, Globals.ServerConfigSection.CommonItems.eInvoiceAccountUserPass, new VNPTInvoice().ToXML(mCustomer, TransactionFinalizationObj, mRptOutPtTransactionFinalizationDetailCollection), Globals.ServerConfigSection.CommonItems.eInvoiceAdminUserName, Globals.ServerConfigSection.CommonItems.eInvoiceAdminUserPass, aPattern, aSerial, 0, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string JResult = contract.EndImportAndPublishInv(asyncResult);
                            if (VNPTAccountingPublishServiceClient.ImportAndPublishInvErrorCodeDetails.ContainsKey(JResult))
                            {
                                mLogView.AppendLogMessage(string.Format("{0}-{1}", JResult, VNPTAccountingPublishServiceClient.ImportAndPublishInvErrorCodeDetails[JResult]));
                                aGenTask.Error = new Exception(VNPTAccountingPublishServiceClient.ImportAndPublishInvErrorCodeDetails[JResult]);
                                aGenTask.ActionComplete(true);
                            }
                            else if (JResult.StartsWith("ERR"))
                            {
                                mLogView.AppendLogMessage(JResult);
                                aGenTask.Error = new Exception(JResult);
                                aGenTask.ActionComplete(true);
                            }
                            if (JResult.StartsWith("OK:"))
                            {
                                mLogView.AppendLogMessage(string.Format("{0}: {1}", eHCMSResources.Z2651_G1_ThemMoiHoaDonThanhCong, JResult));
                                string mResultToken = string.Format("OK:{0};{1}-{2}_", TransactionFinalizationObj.Denominator, TransactionFinalizationObj.Symbol, TransactionFinalizationObj.InvoiceKey);
                                if (JResult.StartsWith(mResultToken) && !string.IsNullOrEmpty(JResult.Replace(mResultToken, "")))
                                {
                                    TransactionFinalizationObj.InvoiceNumb = JResult.Replace(mResultToken, "").PadLeft(Globals.ServerConfigSection.CommonItems.MaxEInvoicePaternLength, '0');
                                }
                                TransactionFinalizationObj.eInvoiceToken = JResult;
                                aGenTask.ActionComplete(true);
                            }
                        }
                        catch (Exception ex)
                        {
                            mLogView.AppendLogMessage(ex.Message);
                            aGenTask.Error = new Exception(ex.Message);
                            aGenTask.ActionComplete(true);
                        }
                    }), null);
                }
            });
            t.Start();
        }
        #endregion
        public static void CorrectRegistrationDetails(PatientRegistration CurrentRegistration)
        {
            CurrentRegistration.CorrectRegistrationDetails(Globals.ServerConfigSection.HealthInsurances.SpecialRuleForHIConsultationApplied, Globals.GetCurServerDateTime(), Globals.ServerConfigSection.HealthInsurances.HIPercentOnDifDept, Globals.ServerConfigSection.HealthInsurances.HiPolicyMinSalary, Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, Globals.ServerConfigSection.HealthInsurances.FullHIBenefitForConfirm, false, Globals.ServerConfigSection.CommonItems.AddingServicesPercent, Globals.ServerConfigSection.HealthInsurances.FullHIOfServicesForConfirm
                , Globals.ServerConfigSection.CommonItems.AddingHIServicesPercent);
        }
        public static void CorrectRegistrationDetails_V2(PatientRegistration CurrentRegistration)
        {
            CurrentRegistration.CorrectRegistrationDetails_V2(Globals.ServerConfigSection.HealthInsurances.SpecialRuleForHIConsultationApplied, Globals.GetCurServerDateTime(),
                Globals.ServerConfigSection.HealthInsurances.HIPercentOnDifDept, Globals.ServerConfigSection.HealthInsurances.HiPolicyMinSalary,
                Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, Globals.ServerConfigSection.CommonItems.AddingServicesPercent,
                Globals.ServerConfigSection.CommonItems.AddingHIServicesPercent, Globals.ServerConfigSection.HealthInsurances.FullHIOfServicesForConfirm,
                Globals.ServerConfigSection.HealthInsurances.PercentForEkip, Globals.ServerConfigSection.HealthInsurances.PercentForOtherEkip);
        }
        public static void AddOrUpdateTransactionFinalization(GenericCoRoutineTask aGenTask, object aIsUpdateToken, object aFinalizationObj)
        {
            byte mViewCase = 0;
            OutPtTransactionFinalization mFinalizationObj = aFinalizationObj as OutPtTransactionFinalization;
            bool mIsUpdateToken = Convert.ToBoolean(aIsUpdateToken);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginAddOutPtTransactionFinalization(mFinalizationObj, mIsUpdateToken, mViewCase, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                long TransactionFinalizationSummaryInfoID = 0;
                                long OutTranFinalizationID = 0;
                                if (!contract.EndAddOutPtTransactionFinalization(out TransactionFinalizationSummaryInfoID, out OutTranFinalizationID, asyncResult))
                                {
                                    aGenTask.Error = new Exception(eHCMSResources.A0991_G1_Msg_ErrorSystem);
                                }
                                else
                                {
                                    mFinalizationObj.TranFinalizationID = OutTranFinalizationID;
                                    mFinalizationObj.TransactionFinalizationSummaryInfoID = TransactionFinalizationSummaryInfoID;
                                }
                            }
                            catch (Exception ex)
                            {
                                aGenTask.Error = ex;
                            }
                            finally
                            {
                                aGenTask.ActionComplete(true);
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    aGenTask.Error = ex;
                    aGenTask.ActionComplete(true);
                }
            });
            t.Start();
        }
        public static bool CheckValidRequestRegimen(PatientPCLRequest aCurrentPclRequest, List<RefTreatmentRegimen> aTreatmentRegimenCollection)
        {
            if (aCurrentPclRequest == null || aCurrentPclRequest.PatientPCLRequestIndicators == null || aCurrentPclRequest.PatientPCLRequestIndicators.Count == 0 || aTreatmentRegimenCollection == null
                || aTreatmentRegimenCollection.Count == 0
                || !aTreatmentRegimenCollection.Any(x => x.RefTreatmentRegimenPCLDetails != null && x.RefTreatmentRegimenPCLDetails.Count > 0)
                || !aTreatmentRegimenCollection.Where(x => x.RefTreatmentRegimenPCLDetails != null && x.RefTreatmentRegimenPCLDetails.Count > 0).Any(x => x.RefTreatmentRegimenPCLDetails.Count > 0))
            {
                return true;
            }
            List<RefTreatmentRegimenPCLDetail> mTreatmentRegimenPCLCollection = aTreatmentRegimenCollection.Where(x => x.RefTreatmentRegimenPCLDetails != null && x.RefTreatmentRegimenPCLDetails.Count > 0).SelectMany(x => x.RefTreatmentRegimenPCLDetails).ToList();
            if (aCurrentPclRequest.PatientPCLRequestIndicators.Any(o => (o.ChargeableItem as PCLExamType).IsRegimenChecking && !mTreatmentRegimenPCLCollection.Any(x => o.ChargeableItem != null && o.ChargeableItem is PCLExamType && x.PCLExamTypeID == (o.ChargeableItem as PCLExamType).PCLExamTypeID)))
            {
                return false;
            }
            return true;
        }
        public static void PrintProcedureProcess(ViewModelBase aView, SmallProcedure aSmallProcedure, PatientRegistration CurrentRegistration)
        {
            Patient CurrentPatient = CurrentRegistration.Patient;
            if (aSmallProcedure == null || aSmallProcedure.SmallProcedureID == 0)
            {
                return;
            }
            GlobalsNAV.ShowDialog<IHtmlReport>((mView) =>
            {
                var t = new Thread(() =>
                {
                    aView.ShowBusyIndicator();
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetVideoAndImage(Path.Combine(Globals.ServerConfigSection.CommonItems.ReportTemplatesLocation, "TT-PT.html"), Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var mFileArray = contract.EndGetVideoAndImage(asyncResult);
                                if (mFileArray == null || mFileArray.Length == 0)
                                {
                                    mView.NavigateToString("");
                                    return;
                                }
                                MemoryStream mMemStream = new MemoryStream(mFileArray);
                                StreamReader mReader = new StreamReader(mMemStream);
                                string mContentBody = mReader.ReadToEnd();
                                mContentBody = Globals.ReplaceStylesHref(mContentBody);
                                string DateTimeStringFormat = "{0:HH} giờ {0:mm} phút, ngày {0:dd} tháng {0:MM} năm {0:yyyy}";
                                string DateStringFormat = "Ngày {0:dd} tháng {0:MM} năm {0:yyyy}";
                                mContentBody = mContentBody.Replace("[ReportDepartmentOfHealth]", Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth);
                                mContentBody = mContentBody.Replace("[ReportHospitalName]", Globals.ServerConfigSection.CommonItems.ReportHospitalName);
                                mContentBody = mContentBody.Replace("[FullName]", CurrentPatient == null ? "" : CurrentPatient.FullName);
                                mContentBody = mContentBody.Replace("[DOB]", CurrentPatient == null || CurrentPatient.DOB == null ? "" : CurrentPatient.DOB.Value.ToString("yyyy"));
                                mContentBody = mContentBody.Replace("[Gender]", CurrentPatient == null ? "" : CurrentPatient.GenderString);
                                mContentBody = mContentBody.Replace("[PatientCode]", CurrentPatient == null ? "" : CurrentPatient.PatientCode);
                                mContentBody = mContentBody.Replace("[CompletedTime]", (aSmallProcedure == null || aSmallProcedure.CompletedDateTime == DateTime.MinValue) ? string.Format(DateTimeStringFormat, aSmallProcedure.ProcedureDateTime) : string.Format(DateTimeStringFormat, aSmallProcedure.CompletedDateTime));
                                mContentBody = mContentBody.Replace("[ProcedureTime]", aSmallProcedure == null ? "" : string.Format(DateTimeStringFormat, aSmallProcedure.ProcedureDateTime));
                                mContentBody = mContentBody.Replace("[DiagnosisFinal]", aSmallProcedure == null ? "" : aSmallProcedure.AfterICD10.DiseaseNameVN);
                                mContentBody = mContentBody.Replace("[ProcedureMethod]", aSmallProcedure == null ? "" : aSmallProcedure.ProcedureMethod);
                                //▼====== #014
                                mContentBody = mContentBody.Replace("[ProcedureType]", aSmallProcedure.V_Surgery_Tips_Type == null ? "" : string.Format(aSmallProcedure.V_Surgery_Tips_Type.ObjectValue));
                                //▼====== #015
                                //mContentBody = mContentBody.Replace("[DeptName]", aSmallProcedure == null ? "" : string.Format(aSmallProcedure.DepartmentName));
                                mContentBody = mContentBody.Replace("[DeptName]", aSmallProcedure == null || aSmallProcedure.DepartmentName == null || aSmallProcedure.DepartmentName == "" ? (Globals.DeptLocation.RefDepartment != null ? Globals.DeptLocation.RefDepartment.DeptName : "") : aSmallProcedure.DepartmentName);
                                //▲====== #015
                                if (aSmallProcedure != null && aSmallProcedure.BeforeICD10 != null)
                                {
                                    mContentBody = mContentBody.Replace("[Diagnosis]", aSmallProcedure == null ? "" : aSmallProcedure.BeforeICD10.DiseaseNameVN);
                                }
                                if (aSmallProcedure != null && aSmallProcedure.V_AnesthesiaType == 0)
                                {
                                    mContentBody = mContentBody.Replace("[NarcoticMethod]", aSmallProcedure == null ? "" : aSmallProcedure.NarcoticMethod == null ? "" : aSmallProcedure.NarcoticMethod);
                                }
                                else
                                {
                                    Lookup temp = Globals.AllLookupValueList.Where(x => x.LookupID == aSmallProcedure.V_AnesthesiaType).ToObservableCollection().FirstOrDefault();
                                    mContentBody = mContentBody.Replace("[NarcoticMethod]", temp.ObjectValue);
                                }
                                //▲====== #014
                                mContentBody = mContentBody.Replace("[DateTime]", aSmallProcedure == null ? "" : string.Format(DateStringFormat, aSmallProcedure.ProcedureDateTime));
                                mContentBody = mContentBody.Replace("[ProcedureDescription]", aSmallProcedure == null || string.IsNullOrEmpty(aSmallProcedure.TrinhTu) ? "" : aSmallProcedure.TrinhTu.Replace("\n", "<br/>"));
                                mContentBody = mContentBody.Replace("[PtRegistrationCode]", CurrentRegistration == null ? "" : CurrentRegistration.PtRegistrationCode);
                                mContentBody = mContentBody.Replace("[ProcedureType]", aSmallProcedure == null || aSmallProcedure.V_Surgery_Tips_Type == null ? "" : aSmallProcedure.V_Surgery_Tips_Type.ObjectValue);
                                List<Staff> ProcedureStaffs = new List<Staff>();
                                List<Staff> NarcoticStaffs = new List<Staff>();
                                List<Staff> NurseStaffs = new List<Staff>();
                                // 20200207 TNHX: lấy loại user lên trên xem/in
                                if (aSmallProcedure != null)
                                {
                                    if (aSmallProcedure.ProcedureDoctorStaff != null)
                                    {
                                        Staff TempProcedureDoctorStaff = ObjectCopier.DeepCopy(aSmallProcedure.ProcedureDoctorStaff);
                                        TempProcedureDoctorStaff.FullName = aSmallProcedure.ProcedureDoctorStaff.RefStaffCategory.StaffCatgDescription + aSmallProcedure.ProcedureDoctorStaff.FullName;
                                        ProcedureStaffs.Add(TempProcedureDoctorStaff);
                                    }
                                    if (aSmallProcedure.ProcedureDoctorStaff2 != null)
                                    {
                                        Staff TempProcedureDoctorStaff2 = ObjectCopier.DeepCopy(aSmallProcedure.ProcedureDoctorStaff2);
                                        TempProcedureDoctorStaff2.FullName = aSmallProcedure.ProcedureDoctorStaff2.RefStaffCategory.StaffCatgDescription + aSmallProcedure.ProcedureDoctorStaff2.FullName;
                                        ProcedureStaffs.Add(TempProcedureDoctorStaff2);
                                    }
                                    if (aSmallProcedure.NarcoticDoctorStaff != null)
                                    {
                                        Staff TempNarcoticDoctorStaff = ObjectCopier.DeepCopy(aSmallProcedure.NarcoticDoctorStaff);
                                        TempNarcoticDoctorStaff.FullName = aSmallProcedure.NarcoticDoctorStaff.RefStaffCategory.StaffCatgDescription + aSmallProcedure.NarcoticDoctorStaff.FullName;
                                        NarcoticStaffs.Add(TempNarcoticDoctorStaff);
                                    }
                                    if (aSmallProcedure.NarcoticDoctorStaff3 != null)
                                    {
                                        Staff TempNarcoticDoctorStaff3 = ObjectCopier.DeepCopy(aSmallProcedure.NarcoticDoctorStaff3);
                                        TempNarcoticDoctorStaff3.FullName = aSmallProcedure.NarcoticDoctorStaff3.RefStaffCategory.StaffCatgDescription + aSmallProcedure.NarcoticDoctorStaff3.FullName;
                                        NarcoticStaffs.Add(TempNarcoticDoctorStaff3);
                                    }
                                    if (aSmallProcedure.NarcoticDoctorStaff2 != null)
                                    {
                                        Staff TempNarcoticDoctorStaff2 = ObjectCopier.DeepCopy(aSmallProcedure.NarcoticDoctorStaff2);
                                        TempNarcoticDoctorStaff2.FullName = aSmallProcedure.NarcoticDoctorStaff2.RefStaffCategory.StaffCatgDescription + aSmallProcedure.NarcoticDoctorStaff2.FullName;
                                        NarcoticStaffs.Add(TempNarcoticDoctorStaff2);
                                    }
                                    if (aSmallProcedure.NurseStaff != null)
                                    {
                                        Staff TempNurseStaff = ObjectCopier.DeepCopy(aSmallProcedure.NurseStaff);
                                        TempNurseStaff.FullName = aSmallProcedure.NurseStaff.RefStaffCategory.StaffCatgDescription + aSmallProcedure.NurseStaff.FullName;
                                        NurseStaffs.Add(TempNurseStaff);
                                    }
                                    if (aSmallProcedure.NurseStaff2 != null)
                                    {
                                        Staff TempNurseStaff2 = ObjectCopier.DeepCopy(aSmallProcedure.NurseStaff2);
                                        TempNurseStaff2.FullName = aSmallProcedure.NurseStaff2.RefStaffCategory.StaffCatgDescription + aSmallProcedure.NurseStaff2.FullName;
                                        NurseStaffs.Add(TempNurseStaff2);
                                    }
                                    if (aSmallProcedure.NurseStaff3 != null)
                                    {
                                        Staff TempNurseStaff3 = ObjectCopier.DeepCopy(aSmallProcedure.NurseStaff3);
                                        TempNurseStaff3.FullName = aSmallProcedure.NurseStaff3.RefStaffCategory.StaffCatgDescription + aSmallProcedure.NurseStaff3.FullName;
                                        NurseStaffs.Add(TempNurseStaff3);
                                    }
                                }
                                //▼====== #014
                                mContentBody = mContentBody.Replace("[StaffName]", ProcedureStaffs.Count == 0 ? "" : "" + string.Join(", ", ProcedureStaffs));
                                mContentBody = mContentBody.Replace("[NarcoticStaffName]", NarcoticStaffs.Count == 0 ? "" : "" + string.Join(", ", NarcoticStaffs));
                                mContentBody = mContentBody.Replace("[NurseStaffName]", NurseStaffs.Count == 0 ? "" : "" + string.Join(",  ", NurseStaffs));
                                mContentBody = mContentBody.Replace("[Drainage]", aSmallProcedure == null ? "" : aSmallProcedure.Drainage);
                                mContentBody = mContentBody.Replace("[DateOffStitches]", (aSmallProcedure == null || aSmallProcedure.DateOffStitches == null) ? "" : string.Format(DateTimeStringFormat, aSmallProcedure.DateOffStitches));
                                mContentBody = mContentBody.Replace("[Notes]", aSmallProcedure == null ? "" : aSmallProcedure.Notes);
                                //▲====== #014
                                mView.NavigateToString(mContentBody);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                aView.HideBusyIndicator();
                            }
                        }), null);
                    }
                });
                t.Start();
            }, null, false, true, new System.Windows.Size(1000, 600));
        }
        public static void PrintProcedureProcess_V2(ViewModelBase aView, SmallProcedure aSmallProcedure, PatientRegistration CurrentRegistration)
        {
            Patient CurrentPatient = CurrentRegistration.Patient;
            if (aSmallProcedure == null || aSmallProcedure.SmallProcedureID == 0)
            {
                return;
            }
            GlobalsNAV.ShowDialog<IHtmlReport>((mView) =>
            {
                var t = new Thread(() =>
                {
                    aView.ShowBusyIndicator();
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetVideoAndImage(Path.Combine(Globals.ServerConfigSection.CommonItems.ReportTemplatesLocation, "TT-PT_Template.html"), Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var mFileArray = contract.EndGetVideoAndImage(asyncResult);
                                if (mFileArray == null || mFileArray.Length == 0)
                                {
                                    mView.NavigateToString("");
                                    return;
                                }
                                MemoryStream mMemStream = new MemoryStream(mFileArray);
                                StreamReader mReader = new StreamReader(mMemStream);
                                string mContentBody = mReader.ReadToEnd();
                                mContentBody = Globals.ReplaceStylesHref(mContentBody);
                                string DateTimeStringFormat = "{0:HH} giờ {0:mm} phút, ngày {0:dd} tháng {0:MM} năm {0:yyyy}";
                                mContentBody = mContentBody.Replace("[ReportDepartmentOfHealth]", Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth);
                                mContentBody = mContentBody.Replace("[ReportHospitalName]", Globals.ServerConfigSection.CommonItems.ReportHospitalName);
                                mContentBody = mContentBody.Replace("[FullName]", CurrentPatient == null ? "" : CurrentPatient.FullName);
                                mContentBody = mContentBody.Replace("[DOB]", CurrentPatient == null || CurrentPatient.DOB == null ? "" : CurrentPatient.DOB.Value.ToString("yyyy"));
                                mContentBody = mContentBody.Replace("[Gender]", CurrentPatient == null ? "" : CurrentPatient.GenderString);
                                mContentBody = mContentBody.Replace("[PatientCode]", CurrentPatient == null ? "" : CurrentPatient.PatientCode);
                                mContentBody = mContentBody.Replace("[CompletedTime]", (aSmallProcedure == null || aSmallProcedure.CompletedDateTime == DateTime.MinValue) ? string.Format(DateTimeStringFormat, aSmallProcedure.ProcedureDateTime) : string.Format(DateTimeStringFormat, aSmallProcedure.CompletedDateTime));
                                mContentBody = mContentBody.Replace("[ProcedureTime]", aSmallProcedure == null ? "" : string.Format(DateTimeStringFormat, aSmallProcedure.ProcedureDateTime));
                                mContentBody = mContentBody.Replace("[DiagnosisFinal]", aSmallProcedure == null ? "" : aSmallProcedure.AfterICD10.DiseaseNameVN);
                                mContentBody = mContentBody.Replace("[ProcedureMethod]", aSmallProcedure == null ? "" : aSmallProcedure.ProcedureMethod);
                                //▼====== #014
                                mContentBody = mContentBody.Replace("[ProcedureType]", aSmallProcedure.V_Surgery_Tips_Type == null ? "" : string.Format(aSmallProcedure.V_Surgery_Tips_Type.ObjectValue));
                                //▼====== #015
                                //mContentBody = mContentBody.Replace("[DeptName]", aSmallProcedure == null ? "" : string.Format(aSmallProcedure.DepartmentName));
                                mContentBody = mContentBody.Replace("[DeptName]", aSmallProcedure == null || aSmallProcedure.DepartmentName == null || aSmallProcedure.DepartmentName == "" ? (Globals.DeptLocation.RefDepartment != null ? Globals.DeptLocation.RefDepartment.DeptName : "") : aSmallProcedure.DepartmentName);
                                //▲====== #015
                                if (aSmallProcedure != null && aSmallProcedure.BeforeICD10 != null)
                                {
                                    mContentBody = mContentBody.Replace("[Diagnosis]", aSmallProcedure == null ? "" : aSmallProcedure.BeforeICD10.DiseaseNameVN);
                                }
                                if (aSmallProcedure != null && aSmallProcedure.V_AnesthesiaType == 0)
                                {
                                    mContentBody = mContentBody.Replace("[NarcoticMethod]", aSmallProcedure == null ? "" : aSmallProcedure.NarcoticMethod == null ? "" : aSmallProcedure.NarcoticMethod);
                                }
                                else
                                {
                                    Lookup temp = Globals.AllLookupValueList.Where(x => x.LookupID == aSmallProcedure.V_AnesthesiaType).ToObservableCollection().FirstOrDefault();
                                    mContentBody = mContentBody.Replace("[NarcoticMethod]", temp.ObjectValue);
                                }
                                //▲====== #014
                                mContentBody = mContentBody.Replace("[DateTime]", aSmallProcedure == null ? "" : string.Format(DateTimeStringFormat, aSmallProcedure.ProcedureDateTime));
                                mContentBody = mContentBody.Replace("[ProcedureDescription]", aSmallProcedure == null || string.IsNullOrEmpty(aSmallProcedure.ProcedureDescription) ? "" : aSmallProcedure.ProcedureDescription);
                                mContentBody = mContentBody.Replace("[PtRegistrationCode]", CurrentRegistration == null ? "" : CurrentRegistration.PtRegistrationCode);
                                mContentBody = mContentBody.Replace("[ProcedureType]", aSmallProcedure == null || aSmallProcedure.V_Surgery_Tips_Type == null ? "" : aSmallProcedure.V_Surgery_Tips_Type.ObjectValue);
                                List<Staff> ProcedureStaffs = new List<Staff>();
                                List<Staff> NarcoticStaffs = new List<Staff>();
                                List<Staff> NurseStaffs = new List<Staff>();
                                // 20200207 TNHX: lấy loại user lên trên xem/in
                                if (aSmallProcedure != null)
                                {
                                    if (aSmallProcedure.ProcedureDoctorStaff != null)
                                    {
                                        Staff TempProcedureDoctorStaff = ObjectCopier.DeepCopy(aSmallProcedure.ProcedureDoctorStaff);
                                        TempProcedureDoctorStaff.FullName = aSmallProcedure.ProcedureDoctorStaff.RefStaffCategory.StaffCatgDescription + aSmallProcedure.ProcedureDoctorStaff.FullName;
                                        ProcedureStaffs.Add(TempProcedureDoctorStaff);
                                    }
                                    if (aSmallProcedure.ProcedureDoctorStaff2 != null)
                                    {
                                        Staff TempProcedureDoctorStaff2 = ObjectCopier.DeepCopy(aSmallProcedure.ProcedureDoctorStaff2);
                                        TempProcedureDoctorStaff2.FullName = aSmallProcedure.ProcedureDoctorStaff2.RefStaffCategory.StaffCatgDescription + aSmallProcedure.ProcedureDoctorStaff2.FullName;
                                        ProcedureStaffs.Add(TempProcedureDoctorStaff2);
                                    }
                                    if (aSmallProcedure.NarcoticDoctorStaff != null)
                                    {
                                        Staff TempNarcoticDoctorStaff = ObjectCopier.DeepCopy(aSmallProcedure.NarcoticDoctorStaff);
                                        TempNarcoticDoctorStaff.FullName = aSmallProcedure.NarcoticDoctorStaff.RefStaffCategory.StaffCatgDescription + aSmallProcedure.NarcoticDoctorStaff.FullName;
                                        NarcoticStaffs.Add(TempNarcoticDoctorStaff);
                                    }
                                    if (aSmallProcedure.NarcoticDoctorStaff2 != null)
                                    {
                                        Staff TempNarcoticDoctorStaff2 = ObjectCopier.DeepCopy(aSmallProcedure.NarcoticDoctorStaff2);
                                        TempNarcoticDoctorStaff2.FullName = aSmallProcedure.NarcoticDoctorStaff2.RefStaffCategory.StaffCatgDescription + aSmallProcedure.NarcoticDoctorStaff2.FullName;
                                        NarcoticStaffs.Add(TempNarcoticDoctorStaff2);
                                    }
                                    if (aSmallProcedure.NurseStaff != null)
                                    {
                                        Staff TempNurseStaff = ObjectCopier.DeepCopy(aSmallProcedure.NurseStaff);
                                        TempNurseStaff.FullName = aSmallProcedure.NurseStaff.RefStaffCategory.StaffCatgDescription + aSmallProcedure.NurseStaff.FullName;
                                        NurseStaffs.Add(TempNurseStaff);
                                    }
                                    if (aSmallProcedure.NurseStaff2 != null)
                                    {
                                        Staff TempNurseStaff2 = ObjectCopier.DeepCopy(aSmallProcedure.NurseStaff2);
                                        TempNurseStaff2.FullName = aSmallProcedure.NurseStaff2.RefStaffCategory.StaffCatgDescription + aSmallProcedure.NurseStaff2.FullName;
                                        NurseStaffs.Add(TempNurseStaff2);
                                    }
                                    if (aSmallProcedure.NurseStaff3 != null)
                                    {
                                        Staff TempNurseStaff3 = ObjectCopier.DeepCopy(aSmallProcedure.NurseStaff3);
                                        TempNurseStaff3.FullName = aSmallProcedure.NurseStaff3.RefStaffCategory.StaffCatgDescription + aSmallProcedure.NurseStaff3.FullName;
                                        NurseStaffs.Add(TempNurseStaff3);
                                    }
                                }
                                //▼====== #014
                                mContentBody = mContentBody.Replace("[StaffName]", ProcedureStaffs.Count == 0 ? "" : "" + string.Join(", ", ProcedureStaffs));
                                mContentBody = mContentBody.Replace("[NarcoticStaffName]", NarcoticStaffs.Count == 0 ? "" : "" + string.Join(", ", NarcoticStaffs));
                                mContentBody = mContentBody.Replace("[NurseStaffName]", NurseStaffs.Count == 0 ? "" : "" + string.Join(",  ", NurseStaffs));
                                mContentBody = mContentBody.Replace("[Drainage]", aSmallProcedure == null ? "" : aSmallProcedure.Drainage);
                                mContentBody = mContentBody.Replace("[DateOffStitches]", (aSmallProcedure == null || aSmallProcedure.DateOffStitches == null) ? "" : string.Format(DateTimeStringFormat, aSmallProcedure.DateOffStitches));
                                mContentBody = mContentBody.Replace("[Notes]", aSmallProcedure == null ? "" : aSmallProcedure.Notes);
                                //▲====== #014
                                mView.NavigateToString(mContentBody);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                aView.HideBusyIndicator();
                            }
                        }), null);
                    }
                });
                t.Start();
            }, null, false, true, new System.Windows.Size(1000, 600));
        }
        public static void ChangeHIBenefit(PatientRegistration CurrentRegistration, MedRegItemBase aInvoiceItem, MedRegItemBase aUpdateInvoiceItem)
        {
            CurrentRegistration.ChangeHIBenefit(aInvoiceItem, aUpdateInvoiceItem, Globals.ServerConfigSection.HealthInsurances.FullHIBenefitForConfirm, Globals.ServerConfigSection.HealthInsurances.HiPolicyMinSalary, Globals.ServerConfigSection.HealthInsurances.FullHIOfServicesForConfirm);
        }
        public static bool TryPickColor(out System.Windows.Media.Color aColor)
        {
            using (System.Windows.Forms.ColorDialog aColorDialog = new System.Windows.Forms.ColorDialog())
            {
                aColorDialog.AllowFullOpen = true;
                aColorDialog.FullOpen = true;
                if (aColorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    System.Windows.Media.Color SelectedColor = new System.Windows.Media.Color();
                    SelectedColor.A = aColorDialog.Color.A;
                    SelectedColor.B = aColorDialog.Color.B;
                    SelectedColor.G = aColorDialog.Color.G;
                    SelectedColor.R = aColorDialog.Color.R;
                    aColor = SelectedColor;
                    return true;
                }
            }
            aColor = new System.Windows.Media.Color();
            return false;
        }
        public static void GetAllPositionInHospital(ViewModelBase aView)
        {
            if (Globals.AllPositionInHospital != null && Globals.AllPositionInHospital.Count > 0)
            {
                return;
            }
            aView.ShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                using (var serviceFactory = new TransactionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllPositionInHospital(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            Globals.AllPositionInHospital = contract.EndGetAllPositionInHospital(asyncResult);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            aView.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            CurrentThread.Start();
        }

        //▼===== #001
        public static void ChangeItemsRecordState(MedRegItemBase Items)
        {
            if (Items == null)
            {
                return;
            }
            if (Items is PatientRegistrationDetail)
            {
                (Items as PatientRegistrationDetail).RecordState = (Items as PatientRegistrationDetail).RecordState == RecordState.UNCHANGED ? RecordState.MODIFIED : (Items as PatientRegistrationDetail).RecordState;
            }
            if (Items is PatientPCLRequestDetail)
            {
                if ((Items as PatientPCLRequestDetail).PatientPCLRequest != null
                    && (Items as PatientPCLRequestDetail).PatientPCLRequest.PatientPCLRequestIndicators != null
                    && (Items as PatientPCLRequestDetail).PatientPCLRequest.PatientPCLRequestIndicators.Count > 0)
                {
                    (Items as PatientPCLRequestDetail).PatientPCLRequest.RecordState = (Items as PatientPCLRequestDetail).PatientPCLRequest.RecordState == RecordState.UNCHANGED ? RecordState.MODIFIED : (Items as PatientPCLRequestDetail).PatientPCLRequest.RecordState;
                    foreach (var detail in (Items as PatientPCLRequestDetail).PatientPCLRequest.PatientPCLRequestIndicators)
                    {
                        detail.RecordState = detail.RecordState == RecordState.UNCHANGED ? RecordState.MODIFIED : detail.RecordState;
                    }
                }
            }
        }
        //▲===== #001
    }
}
namespace aEMR.Common
{
    public class StaffWorkingSchedule
    {
        public DeptLocation CurrentDeptLocation { get; set; }
        public RefDepartment CurrentDepartment { get; set; }
        public ConsultationTimeSegments ConsultationSegment { get; set; }
        public Staff DoctorStaff { get; set; }
        public IList<RefMedicalServiceItem> ServiceItemCollection { get; set; }
        public long ConsultationRoomStaffAllocationServiceListID { get; set; }
    }
    public class CalendarDay : NotifyChangedBase
    {
        private DateTime? _CalendarDate;
        private ObservableCollection<StaffWorkingSchedule> _StaffWorkingSchedule;
        private bool _IsSelected = false;
        private ICollectionView _StaffWorkingScheduleView;
        private int _TargetNumberOfCases;
        private int _QuantityOfProcessedCases;
        private bool _IsExtendDate = false;
        private bool _IsOpened = false;
        public int Day
        {
            get
            {
                return CalendarDate == null ? 0 : CalendarDate.Value.Day;
            }
        }
        public bool IsHasValue
        {
            get
            {
                return Day > 0;
            }
        }
        public ObservableCollection<StaffWorkingSchedule> StaffWorkingSchedule
        {
            get
            {
                return _StaffWorkingSchedule;
            }
            set
            {
                if (_StaffWorkingSchedule == value)
                {
                    return;
                }
                _StaffWorkingSchedule = value;
                RaisePropertyChanged("StaffWorkingSchedule");
                StaffWorkingScheduleView = CollectionViewSource.GetDefaultView(StaffWorkingSchedule);
                StaffWorkingScheduleView.GroupDescriptions.Add(new PropertyGroupDescription("CurrentDepartment"));
                StaffWorkingScheduleView.GroupDescriptions.Add(new PropertyGroupDescription("CurrentDeptLocation"));
                StaffWorkingScheduleView.GroupDescriptions.Add(new PropertyGroupDescription("ConsultationSegment"));
            }
        }
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                if (_IsSelected == value)
                {
                    return;
                }
                _IsSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }
        public ICollectionView StaffWorkingScheduleView
        {
            get
            {
                return _StaffWorkingScheduleView;
            }
            set
            {
                if (_StaffWorkingScheduleView == value)
                {
                    return;
                }
                _StaffWorkingScheduleView = value;
                RaisePropertyChanged("StaffWorkingScheduleView");
            }
        }
        public DateTime? CalendarDate
        {
            get
            {
                return _CalendarDate;
            }
            set
            {
                if (_CalendarDate == value)
                {
                    return;
                }
                _CalendarDate = value;
                RaisePropertyChanged("CalendarDate");
                RaisePropertyChanged("Day");
                RaisePropertyChanged("IsHasValue");
            }
        }
        public int TargetNumberOfCases
        {
            get
            {
                return _TargetNumberOfCases;
            }
            set
            {
                if (_TargetNumberOfCases == value)
                {
                    return;
                }
                _TargetNumberOfCases = value;
                RaisePropertyChanged("TargetNumberOfCases");
                RaisePropertyChanged("TargetNumberOfCasesRemaining");
            }
        }
        public int QuantityOfProcessedCases
        {
            get
            {
                return _QuantityOfProcessedCases;
            }
            set
            {
                if (_QuantityOfProcessedCases == value)
                {
                    return;
                }
                _QuantityOfProcessedCases = value;
                RaisePropertyChanged("QuantityOfProcessedCases");
                RaisePropertyChanged("TargetNumberOfCasesRemaining");
            }
        }
        public int TargetNumberOfCasesRemaining
        {
            get
            {
                return TargetNumberOfCases - QuantityOfProcessedCases;
            }
        }
        public bool IsExtendDate
        {
            get
            {
                return _IsExtendDate;
            }
            set
            {
                if (_IsExtendDate == value)
                {
                    return;
                }
                _IsExtendDate = value;
                RaisePropertyChanged("IsExtendDate");
            }
        }
        public bool IsOpened
        {
            get
            {
                return _IsOpened;
            }
            set
            {
                if (_IsOpened == value)
                {
                    return;
                }
                _IsOpened = value;
                RaisePropertyChanged("IsOpened");
            }
        }
    }
    public class CalendarWeek : NotifyChangedBase
    {
        private CalendarDay[] _CalendarDayCollection = new CalendarDay[7] { new CalendarDay(), new CalendarDay(), new CalendarDay(), new CalendarDay(), new CalendarDay(), new CalendarDay(), new CalendarDay() };
        public CalendarDay[] CalendarDayCollection
        {
            get
            {
                return _CalendarDayCollection;
            }
            set
            {
                if (_CalendarDayCollection == value)
                {
                    return;
                }
                _CalendarDayCollection = value;
                RaisePropertyChanged("CalendarDayCollection");
                RaisePropertyChanged("IsHasValue");
            }
        }
        public bool IsHasValue
        {
            get
            {
                return CalendarDayCollection.Sum(x => x.Day) > 0;
            }
        }
    }
}
