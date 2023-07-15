/*
 * 20170218 #001 CMN: Add Checkbox AllDept for InPtBills
 * 20170522 #002 CMN: Added variable to check InPt 5 year HI without paid enough
 * 20170609 #003 CMN: Fix SupportFund About TT04 with TT04
 * 20170619 #004 CMN: Service for payment report OutPt with large data
*/
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Activation;
using AxLogging;
using DataEntities;
using Service.Core.Common;
using Service.Core.HelperClasses;
using eHCMS.Caching;
using eHCMS.Configurations;
using eHCMS.DAL;
using eHCMS.Services.Core;
using ErrorLibrary;
using ErrorLibrary.Resources;
using EventManagementService;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Data.SqlClient;
using eHCMSLanguage;

namespace CommonServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [ServiceKnownTypeAttribute(typeof(IInvoiceItem))]
    [Obsolete("This Class has been DEPRECATED. CommonService has been REPLACED by other Classes ie. PatientRegistrationService, CommonServices_V2 etc...", true)]
    public class CommonService : eHCMS.WCFServiceCustomHeader, ICommonService
    {
        private string _ModuleName = "Common Module";

        public bool DoListUpload(List<PCLResultFileStorageDetail> lst)
        {
            try
            {
                foreach (var item in lst)
                {
                    DoUpload(item.PCLResultFileName, item.File, false, item.FullPath);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DoUpload(string filename, byte[] content, bool append, string dir)
        {
            //  string folder = Path.GetFullPath(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "FileStore"));
            string folder = Path.GetFullPath(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, dir));
            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);

            FileMode m = FileMode.Create;
            if (append)
                m = FileMode.Append;

            using (FileStream fs = new FileStream(folder + @"\" + filename, m, FileAccess.Write))
            {
                fs.Write(content, 0, content.Length);
            }
            return true;
        }


        public bool SaveImageCapture(byte[] byteArray)
        {
            try
            {

                string imageFileName = DateTime.Now.ToShortDateString().Replace(@"/", @"-").Replace(@"\", @"-") + ".jpeg";
                FileStream f = new FileStream(@"\\AXSERVER01\ImageCapture\" + imageFileName, FileMode.Create);

                f.Write(byteArray, 0, byteArray.Length);
                f.Flush();
                f.Close();

            }
            catch
            {
                return false;
            }

            return true;
        }
        public bool SaveImageCapture1(MemoryStream byteArray)
        {
            try
            {

                string imageFileName = DateTime.Now.ToShortDateString().Replace(@"/", @"-").Replace(@"\", @"-") + ".jpeg";
                FileStream f = new FileStream(@"\\\\AXSERVER01\\ImageCapture\\" + imageFileName, FileMode.Create);

                f.Write(byteArray.ToArray(), 0, byteArray.ToArray().Length);
                f.Flush();
                f.Close();


            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool UpdateRefApplicationConfigs(string ConfigItemKey, string ConfigItemValue, string ConfigItemNotes)
        {
            try
            {
                return AppConfigsProvider.Instance.UpdateRefApplicationConfigs(ConfigItemKey, ConfigItemValue, ConfigItemNotes);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of calculating UpdateRefApplicationConfigs. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_UpdateRefApplicationConfigs, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<SuburbNames> GetAllSuburbNames()
        {
            try
            {
                return AppConfigsProvider.Instance.GetAllSuburbNames();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of calculating UpdateRefApplicationConfigs. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_UpdateRefApplicationConfigs, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<Gender> GetAllGenders()
        {
            List<Gender> genders = new List<Gender>();
            genders.Add(new Gender("M", "Nam"));
            genders.Add(new Gender("F", "Nữ"));
            return genders;
        }
        public IList<AllLookupValues.RegistrationStatus> GetAllRegistrationStatus()
        {
            List<AllLookupValues.RegistrationStatus> genders = new List<AllLookupValues.RegistrationStatus>();
            genders.Add(AllLookupValues.RegistrationStatus.OPENED);
            //genders.Add(AllLookupValues.RegistrationStatus.PROCESSING);
            genders.Add(AllLookupValues.RegistrationStatus.COMPLETED);
            genders.Add(AllLookupValues.RegistrationStatus.INVALID);
            //genders.Add(AllLookupValues.RegistrationStatus.LOCKED);
            genders.Add(AllLookupValues.RegistrationStatus.REFUND);
            genders.Add(AllLookupValues.RegistrationStatus.PENDING);

            return genders;
        }


        private IList<Lookup> GetLookupByType(LookupValues lookUpType)
        {
            string mainCacheKey = "LookupValues_" + ((int)lookUpType).ToString();
            List<Lookup> lookups;
            //Kiểm tra nếu có trong cache thì lấy từ trong cache.
            if (ServerAppConfig.CachingEnabled)
            {
                lookups = (List<Lookup>)AxCache.Current[mainCacheKey];
                if (lookups != null)
                {
                    return lookups;
                }
            }

            lookups = CommonProvider.Lookups.GetAllLookupsByType(lookUpType);

            //Lưu vào cache để lần sau sử dụng.
            if (ServerAppConfig.CachingEnabled)
            {
                if (ServerAppConfig.SlidingExpirationTime <= 0 || ServerAppConfig.SlidingExpirationTime == int.MaxValue)
                {
                    AxCache.Current[mainCacheKey] = lookups;
                }
                else
                {
                    AxCache.Current.Insert(mainCacheKey, lookups, new TimeSpan(0, 0, ServerAppConfig.SlidingExpirationTime), true);
                }
            }
            return lookups;
        }

        public IList<Lookup> GetAllLookupValuesByType(LookupValues lookUpType)
        {
            IList<Lookup> allLookups = null;
            try
            {
                allLookups = GetLookupByType(lookUpType);
            }
            catch
            {
                AxException axErr = new AxException();
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                axErr.MethodName = curMethod.Name;
                axErr.ClassName = curMethod.DeclaringType.FullName;

                axErr.ErrorCode = "CM.0_0000002";
                axErr.ModuleName = _ModuleName;

                throw new FaultException<AxException>(axErr);
            }
            return allLookups;
        }

        public IList<Lookup> GetAllLookupValuesForTransferForm(LookupValues lookUpType)
        {
            IList<Lookup> allLookups = null;
            try
            {
                allLookups = GetLookupByType(lookUpType);
            }
            catch
            {
                AxException axErr = new AxException();
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                axErr.MethodName = curMethod.Name;
                axErr.ClassName = curMethod.DeclaringType.FullName;

                axErr.ErrorCode = "CM.0_0000002";
                axErr.ModuleName = _ModuleName;

                throw new FaultException<AxException>(axErr);
            }
            return allLookups;
        }

        public IList<Lookup> GetAllMaritalStatus()
        {
            IList<Lookup> allLookups = null;
            try
            {
                allLookups = GetLookupByType(LookupValues.MARITAL_STATUS);
            }
            catch
            {
                AxException axErr = new AxException();
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                axErr.MethodName = curMethod.Name;
                axErr.ClassName = curMethod.DeclaringType.FullName;

                axErr.ErrorCode = "CM.0_0000002";
                axErr.ModuleName = _ModuleName;

                throw new FaultException<AxException>(axErr);
            }
            return allLookups;
        }

        public IList<Lookup> GetAllEthnics()
        {
            return GetLookupByType(LookupValues.ETHNIC);
        }

        public IList<Lookup> GetAllFamilyRelationships()
        {
            return GetLookupByType(LookupValues.FAMILY_RELATIONSHIP);
        }

        public IList<Lookup> GetAllReligion()
        {
            return GetLookupByType(LookupValues.RELIGION);
        }

        public IList<Lookup> GetAllBankName()
        {
            return GetLookupByType(LookupValues.BANK_NAME);
        }
        public IList<RefCountry> GetAllCountries()
        {
            return CommonProvider.Countries.GetAllCountries();
        }

        public IList<Currency> GetAllCurrency(bool? IsActive)
        {
            return CommonProvider.Countries.GetAllCurrency(IsActive);
        }

        public Staff GetStaffByID(Int64 ID)
        {
            return CommonProvider.Staffs.GetStaffByID(ID);
        }
        public IList<Staff> GetAllStaffContain()
        {
            return CommonProvider.Staffs.GetAllStaffContain();
        }

        public List<StaffPosition> GetAllStaffPosition()
        {
            return CommonProvider.Staffs.GetAllStaffPosition();
        }

        public IList<CitiesProvince> GetAllProvinces()
        {
            return CommonProvider.Countries.GetAllProvinces();
        }

        public IList<Hospital> Hospital_ByCityProvinceID(long? CityProvinceID)
        {
            return CommonProvider.Countries.Hospital_ByCityProvinceID(CityProvinceID);
        }

        public IList<RefGenDrugBHYT_Category> RefGenDrugBHYT_Category_Get()
        {
            return CommonProvider.Countries.RefGenDrugBHYT_Category_Get();
        }

        public IList<RefGenericDrugCategory_1> RefGenericDrugCategory_1_Get(long V_MedProductType)
        {
            return CommonProvider.Countries.RefGenericDrugCategory_1_Get(V_MedProductType);
        }

        public IList<RefGenericDrugCategory_2> RefGenericDrugCategory_2_Get(long V_MedProductType)
        {
            return CommonProvider.Countries.RefGenericDrugCategory_2_Get(V_MedProductType);
        }

        public IList<RefPharmacyDrugCategory> LoadRefPharmacyDrugCategory()
        {
            return CommonProvider.Countries.LoadRefPharmacyDrugCategory();
        }

        public IList<RefDepartment> GetAllDepartments(bool bIncludeDeleted)
        {
            return CommonProvider.Lookups.GetAllDepartments(bIncludeDeleted);
        }

        public IList<DeptTransferDocReq> GetAllDocTypeRequire()
        {
            return CommonProvider.Lookups.GetAllDocTypeRequire();
        }

        public IList<RefDepartment> GetAllDepartmentsByV_DeptTypeOperation(long V_DeptTypeOperation)
        {
            return CommonProvider.Lookups.GetAllDepartmentsByV_DeptTypeOperation(V_DeptTypeOperation);
        }


        public IList<Lookup> GetAllDocumentTypeOnHold()
        {
            try
            {
                string mainCacheKey = "AllDocsOnHold";
                IList<Lookup> allDocs;
                //Kiểm tra nếu có trong cache thì lấy từ trong cache.
                if (ServerAppConfig.CachingEnabled)
                {
                    allDocs = (List<Lookup>)AxCache.Current[mainCacheKey];
                    if (allDocs != null)
                    {
                        return allDocs;
                    }
                }

                allDocs = GetLookupByType(LookupValues.DOCUMENT_TYPE_ON_HOLD);

                //Lưu vào cache để lần sau sử dụng.
                if (ServerAppConfig.CachingEnabled)
                {
                    if (ServerAppConfig.SlidingExpirationTime <= 0 || ServerAppConfig.SlidingExpirationTime == int.MaxValue)
                    {
                        AxCache.Current[mainCacheKey] = allDocs;
                    }
                    else
                    {
                        AxCache.Current.Insert(mainCacheKey, allDocs, new TimeSpan(0, 0, ServerAppConfig.SlidingExpirationTime), true);
                    }
                }
                return allDocs;
            }
            catch
            {
                AxException axErr = new AxException();
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                axErr.MethodName = curMethod.Name;
                axErr.ClassName = curMethod.DeclaringType.FullName;

                axErr.ErrorCode = "PR.0_0000002";
                axErr.ModuleName = _ModuleName;

                throw new FaultException<AxException>(axErr);
            }
        }

        public Stream GetVideoAndImage(string path)
        {
            try
            {
                FileInfo fi = new FileInfo(path);
                Stream strm = fi.OpenRead();

                return strm;
            }
            catch
            {
                return new MemoryStream();
            }
        }
        private byte[] WriteImage(string filename)//convert image file to byte[]
        {
            try
            {
                FileStream fs = File.OpenRead(filename);
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();
                return buffer;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        private void ReadImage(byte[] img)//show the byte[] image
        {

            //// Set up the response settings
            //Response.ContentType = "image/jpeg";
            //Response.Cache.SetCacheability(HttpCacheability.Public);
            //Response.BufferOutput = false;

            //Stream stream = new MemoryStream(img);
            //const int buffersize = 1024 * 16;
            //byte[] buffer = new byte[buffersize];
            //int count = stream.Read(buffer, 0, buffersize);
            //while (count > 0)
            //{
            //    Response.OutputStream.Write(buffer, 0, count);
            //    count = stream.Read(buffer, 0, buffersize);
            //}

        }

        //
        // TxD 25/05/2014: The Following Method has been REPLACED by GetServerConfiguration()
        // AND thus all Configuration settings are now come from Globals.AxServerSettings
        //
        //public IList<string> GetAllConfigItemValues()
        //{
        //    //mai xem lai coi bo vao cho nay co hop li ko da

        //    IList<string> refconfig = CommonProvider.AppConfigs.GetAllConfigItemValues();
        //    if (Globals.AxServerSettings.HealthInsurances == null)
        //    {
        //        Globals.AxServerSettings.HealthInsurances = new HealthInsuranceElement();
        //    }
        //    //Globals.ExcelStorePool = refconfig[(int)AppConfigKeys.ConfigItemKey.ExcelStorePool].ToString();
        //    Globals.AxServerSettings.HealthInsurances.Apply15HIPercent = Convert.ToInt16(refconfig[(int)AppConfigKeys.ConfigItemKey.Apply15HIPercent].ToString());
        //    Globals.AxServerSettings.HealthInsurances.HIConsultationServiceHIAllowedPrice = Convert.ToDouble(refconfig[(int)AppConfigKeys.ConfigItemKey.HIConsultationServiceHIAllowedPrice].ToString());
        //    Globals.AxServerSettings.HealthInsurances.SpecialRuleForHIConsultationApplied = Convert.ToBoolean(refconfig[(int)AppConfigKeys.ConfigItemKey.SpecialRuleForHIConsultationApplied].ToString());
        //    Globals.AxServerSettings.HealthInsurances.HiPolicyMinSalary = Convert.ToInt64(refconfig[(int)AppConfigKeys.ConfigItemKey.HIPolicyMinSalary].ToString());
        //    Globals.AxServerSettings.HealthInsurances.HiPolicyPercentageOnPayable = (float)Convert.ToDouble(refconfig[(int)AppConfigKeys.ConfigItemKey.HIPolicyPercentageOnPayable].ToString(), new CultureInfo("en-US"));
        //    Globals.AxServerSettings.HealthInsurances.PaperReferalMaxDays = Convert.ToInt16(refconfig[(int)AppConfigKeys.ConfigItemKey.PaperReferalMaxDays].ToString());
        //    Globals.AxServerSettings.HealthInsurances.RebatePercentageLevel1 = Convert.ToDouble(refconfig[(int)AppConfigKeys.ConfigItemKey.HIRebatePercentageLevel1].ToString(), new CultureInfo("en-US"));
        //    if (Globals.AxServerSettings.Hospitals == null)
        //    {
        //        Globals.AxServerSettings.Hospitals = new HospitalElement();
        //    }
        //    Globals.AxServerSettings.Hospitals.HospitalCode = refconfig[(int)AppConfigKeys.ConfigItemKey.HospitalCode].ToString();

        //    if (Globals.AxServerSettings.CommonItems == null)
        //    {
        //        Globals.AxServerSettings.CommonItems = new CommonItemElement();
        //    }
        //    Globals.AxServerSettings.CommonItems.RefundOrCancelCashReceipt = Convert.ToInt16(refconfig[(int)AppConfigKeys.ConfigItemKey.RefundOrCancelCashReceipt].ToString());

        //    // TxD 21/05/2014: The following line is commented out because it's not appropriate anymore
        //    //CommonProvider.AppConfigs.PriceList_Reset();

        //    return refconfig;
        //}

        public AxServerConfigSection GetServerConfiguration()
        {
            return Globals.AxServerSettings;
        }

        //public string GetConfigItemsValueBySerialNumber(int sNumber)
        //{
        //    return CommonProvider.AppConfigs.GetConfigItemsValueBySerialNumber(sNumber);
        //}

        public IList<RefDepartment> GetDepartments(long locationID)
        {
            try
            {
                string mainCacheKey = locationID.ToString() + "_Dept";
                List<RefDepartment> allDepartments;
                //Kiểm tra nếu có trong cache thì lấy từ trong cache.
                if (ServerAppConfig.CachingEnabled)
                {
                    allDepartments = (List<RefDepartment>)AxCache.Current[mainCacheKey];
                    if (allDepartments != null)
                    {
                        return allDepartments;
                    }
                }

                allDepartments = CommonProvider.Lookups.GetDepartments(locationID);

                //Lưu vào cache để lần sau sử dụng.
                if (ServerAppConfig.CachingEnabled)
                {
                    if (ServerAppConfig.SlidingExpirationTime <= 0 || ServerAppConfig.SlidingExpirationTime == int.MaxValue)
                    {
                        AxCache.Current[mainCacheKey] = allDepartments;
                    }
                    else
                    {
                        AxCache.Current.Insert(mainCacheKey, allDepartments, new TimeSpan(0, 0, ServerAppConfig.SlidingExpirationTime), true);
                    }
                }
                return allDepartments;
            }
            catch
            {
                AxException axErr = new AxException();
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                axErr.MethodName = curMethod.Name;
                axErr.ClassName = curMethod.DeclaringType.FullName;

                axErr.ErrorCode = "PR.0_0000002";
                axErr.ModuleName = _ModuleName;

                throw new FaultException<AxException>(axErr);
            }
        }

        public IList<ConsultationRoomTarget> GetAllConsultationRoomTarget()
        {
            try
            {
                string mainCacheKey = "AllConsultationRoomTarget";
                List<ConsultationRoomTarget> allConsultationRoomTarget;
                //Kiểm tra nếu có trong cache thì lấy từ trong cache.
                if (ServerAppConfig.CachingEnabled)
                {
                    allConsultationRoomTarget = (List<ConsultationRoomTarget>)AxCache.Current[mainCacheKey];
                    if (allConsultationRoomTarget != null)
                    {
                        return allConsultationRoomTarget;
                    }
                }

                allConsultationRoomTarget = ClinicManagementProvider.instance.GetConsultationRoomTargetTimeSegment(0, 0);

                //Lưu vào cache để lần sau sử dụng.
                if (ServerAppConfig.CachingEnabled)
                {
                    if (ServerAppConfig.SlidingExpirationTime <= 0 || ServerAppConfig.SlidingExpirationTime == int.MaxValue)
                    {
                        AxCache.Current[mainCacheKey] = allConsultationRoomTarget;
                    }
                    else
                    {
                        AxCache.Current.Insert(mainCacheKey, allConsultationRoomTarget, new TimeSpan(0, 0, ServerAppConfig.SlidingExpirationTime), true);
                    }
                }
                return allConsultationRoomTarget;
            }
            catch
            {
                AxException axErr = new AxException();
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                axErr.MethodName = curMethod.Name;
                axErr.ClassName = curMethod.DeclaringType.FullName;

                axErr.ErrorCode = "PR.0_0000002";
                axErr.ModuleName = _ModuleName;

                throw new FaultException<AxException>(axErr);
            }
        }
        //------------------Dinh viet ham nay de test ne------------------------------------------------------
        public List<PatientTransactionDetail> GetTransactionSum(long TransactionID)
        {
            try
            {
                List<PatientTransactionDetail> allTranDetails = PatientProvider.Instance.GetAlltransactionDetailsSum(TransactionID);

                foreach (var patientTransactionDetail in allTranDetails)
                {
                    if (patientTransactionDetail.outiID != null)
                    {
                        patientTransactionDetail.TransactionType = "Phiếu Xuất";
                    }
                    if (patientTransactionDetail.PCLRequestID != null)
                    {
                        patientTransactionDetail.TransactionType = "PCLRequest";
                    }
                    if (patientTransactionDetail.PtRegDetailID != null)
                    {
                        patientTransactionDetail.TransactionType = "Service";
                    }
                    if (patientTransactionDetail.V_TranRefType == AllLookupValues.V_TranRefType.BILL_NOI_TRU)
                    {
                        patientTransactionDetail.TransactionType = "BILL NOI TRU";
                    }
                    else if (patientTransactionDetail.V_TranRefType == AllLookupValues.V_TranRefType.DRUG_NGOAITRU)
                    {
                        patientTransactionDetail.TransactionType = "DRUG NGOAI TRU";
                    }
                    else if (patientTransactionDetail.V_TranRefType == AllLookupValues.V_TranRefType.DRUG_NOITRU_KHOCUAKHOA)
                    {
                        patientTransactionDetail.TransactionType = "DRUG NOI TRU KHO CUA KHOA";
                    }
                    else if (patientTransactionDetail.V_TranRefType == AllLookupValues.V_TranRefType.DRUG_NOITRU_KHODUOC)
                    {
                        patientTransactionDetail.TransactionType = "DRUG NOI TRU KHO DUOC";
                    }


                }
                return allTranDetails;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of calculating GetTransactionSum. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetTransactionSum, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }

        public List<PatientTransactionDetail> GetTransactionSum_InPt(long TransactionID)
        {
            try
            {
                List<PatientTransactionDetail> allTranDetails = PatientProvider.Instance.GetAlltransactionDetailsSum_InPt(TransactionID);

                foreach (var patientTransactionDetail in allTranDetails)
                {
                    if (patientTransactionDetail.outiID != null)
                    {
                        patientTransactionDetail.TransactionType = "Phiếu Xuất";
                    }
                    if (patientTransactionDetail.PCLRequestID != null)
                    {
                        patientTransactionDetail.TransactionType = "PCLRequest";
                    }
                    if (patientTransactionDetail.PtRegDetailID != null)
                    {
                        patientTransactionDetail.TransactionType = "Service";
                    }
                    if (patientTransactionDetail.V_TranRefType == AllLookupValues.V_TranRefType.BILL_NOI_TRU)
                    {
                        patientTransactionDetail.TransactionType = "BILL NOI TRU";
                    }
                    else if (patientTransactionDetail.V_TranRefType == AllLookupValues.V_TranRefType.DRUG_NGOAITRU)
                    {
                        patientTransactionDetail.TransactionType = "DRUG NGOAI TRU";
                    }
                    else if (patientTransactionDetail.V_TranRefType == AllLookupValues.V_TranRefType.DRUG_NOITRU_KHOCUAKHOA)
                    {
                        patientTransactionDetail.TransactionType = "DRUG NOI TRU KHO CUA KHOA";
                    }
                    else if (patientTransactionDetail.V_TranRefType == AllLookupValues.V_TranRefType.DRUG_NOITRU_KHODUOC)
                    {
                        patientTransactionDetail.TransactionType = "DRUG NOI TRU KHO DUOC";
                    }


                }
                return allTranDetails;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of calculating GetTransactionSum. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetTransactionSum, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }


        public List<PatientTransactionPayment> GetAllPayments_New(long transactionID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading all payments.", CurrentUser);
                List<PatientTransactionPayment> allPayments = CommonProvider.Payments.GetAllPayments_New(transactionID);
                AxLogger.Instance.LogInfo("End of loading all payments. Status: Success.", CurrentUser);
                return allPayments;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all payments. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //public List<PatientPayment> GetPatientPaymentByDay(PatientPaymentSearchCriteria PatientPaymentSearch)
        //{
        //    try
        //    {
        //        AxLogger.Instance.LogInfo("Start loading all payments.", CurrentUser);
        //        List<PatientPayment> allPayments = CommonProvider.Payments.GetPatientPaymentByDay(PatientPaymentSearch);
        //        AxLogger.Instance.LogInfo("End of loading all payments. Status: Success.", CurrentUser);
        //        return allPayments;
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of loading all payments. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}

        public List<PatientTransactionPayment> GetPatientPaymentByDay_New(PatientPaymentSearchCriteria PatientPaymentSearch, int FindPatient)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading all payments.", CurrentUser);
                List<PatientTransactionPayment> allPayments = CommonProvider.Payments.GetPatientPaymentByDay_New(PatientPaymentSearch, FindPatient);
                AxLogger.Instance.LogInfo("End of loading all payments. Status: Success.", CurrentUser);
                return allPayments;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all payments. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddReportPaymentReceiptByStaff(ReportPaymentReceiptByStaff curReportPaymentReceiptByStaff,
                    List<PatientTransactionPayment> allPayment, out long RepPaymentRecvID)//List<PatientPayment> allPayment,
        {
            try
            {
                return CommonProvider.Payments.AddReportPaymentReceiptByStaff(curReportPaymentReceiptByStaff,
                    allPayment, out RepPaymentRecvID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all payments. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<ReportPaymentReceiptByStaff> GetReportPaymentReceiptByStaff(DateTime FromDate, DateTime ToDate, bool bFilterByStaffID, long loggedStaffID, Int64 nStaffID)
        {
            try
            {
                List<ReportPaymentReceiptByStaff> allReportPaymentReceiptByStaff = CommonProvider.Payments.GetReportPaymentReceiptByStaff(FromDate, ToDate, bFilterByStaffID, nStaffID, loggedStaffID);
                return allReportPaymentReceiptByStaff;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all payments. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public ReportPaymentReceiptByStaffDetails GetReportPaymentReceiptByStaffDetails(long RepPaymentRecvID)
        {
            try
            {
                ReportPaymentReceiptByStaffDetails allReportPaymentReceiptByStaffDetails = CommonProvider.Payments.GetReportPaymentReceiptByStaffDetails(RepPaymentRecvID);
                return allReportPaymentReceiptByStaffDetails;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all payments. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateReportPaymentReceiptByStaff(ReportPaymentReceiptByStaff curReportPaymentReceiptByStaff)
        {
            try
            {
                return CommonProvider.Payments.UpdateReportPaymentReceiptByStaff(curReportPaymentReceiptByStaff);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all payments. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //#endregion
        public bool UpdatePatientRegistrationPayStatus(long PtRegistrationID, long V_RegistrationPaymentStatus)
        {
            try
            {
                return CommonProvider.PatientRg.UpdatePatientRegistrationPayStatus(PtRegistrationID, V_RegistrationPaymentStatus);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdatePatientRegistrationPayStatus. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_UpdatePatientRegistrationPayStatus);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool UpdatePatientRegistration(long PtRegistrationID, long V_RegistrationStatus)
        {
            try
            {
                return CommonProvider.PatientRg.UpdatePatientRegistration(PtRegistrationID, V_RegistrationStatus);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdatePatientRegistration. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_UpdatePatientRegistration);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public bool PatientRegistration_Close(long PtRegistrationID, int FindPatient, long V_RegistrationClosingStatus)
        {
            try
            {
                return CommonProvider.PatientRg.PatientRegistration_Close(PtRegistrationID, FindPatient, V_RegistrationClosingStatus);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving PatientRegistration_Close. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_PatientRegistration_Close);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        #region Ny create
        public IList<PatientRegistration> SearchPtRegistration(SeachPtRegistrationCriteria criteria, int pageSize, int pageIndex, bool bcount, out int Totalcount)
        {
            try
            {
                return CommonProvider.PatientRg.SearchPtRegistration(criteria, pageSize, pageIndex, bcount, out Totalcount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SearchPtRegistration. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_SearchPtRegistration);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public DateTime GetMaxExamDate()
        {
            return CommonProvider.PatientRg.GetMaxExamDate();
        }

        public Patient GetPatientInfoByPtRegistration(long? PtRegistrationID, long? PatientID, int FindPatient)
        {
            try
            {
                return CommonProvider.PatientRg.GetPatientInfoByPtRegistration(PtRegistrationID, PatientID, FindPatient);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetPatientInfoByPtRegistration. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_PATIENTINFO_CANNOT_GET);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        /// <summary>
        /// Tính tiền thuốc cho danh sách thuốc này
        /// </summary>
        /// <param name="registrationID">Mã số đăng ký hiện tại</param>
        /// <param name="drugList">Danh sách thuốc</param>
        /// <param name="CalculatedDrugList">Danh sách thuốc đã được tính</param>
        /// <param name="PayableSum">Tổng số tiền bệnh nhân phải trả cho bệnh viện (tính chung cho cả đăng ký này)</param>
        /// <param name="TotalPaid">Tổng số tiền bệnh nhân đã trả cho bệnh viện</param>
        public void CalcOutwardDrugInvoice(long registrationID, OutwardDrugInvoice drugInvoice, out OutwardDrugInvoice CalculatedDrugInvoice, out PayableSum PayableSum, out decimal TotalPaid)
        {
            throw new Exception(eHCMSResources.Z1785_G1_NotImplemented);
            //RegAndPaymentProcessor paymentProcesssor = new RegAndPaymentProcessor(registrationID);

            //PatientRegistration curRegistration;
            //paymentProcesssor.CalcOutwardDrugInvoice(registrationID, drugInvoice, out curRegistration);
            //CalculatedDrugInvoice = drugInvoice;
            //if (curRegistration.PatientTransaction != null)
            //{
            //    PayableSum = curRegistration.PatientTransaction.PayableSum;
            //}
            //else
            //{
            //    PayableSum = null;
            //}
            ////TODO: Tu tu lam
            //TotalPaid = 0;
        }

        /// <summary>
        /// Lưu lại danh sách thuốc và tính tiền luôn
        /// </summary>
        /// <param name="registrationID">Mã số đăng ký hiện tại</param>
        /// <param name="drugList">Danh sách thuốc</param>
        /// <param name="paymentDetails">Chi tiết thông tin tính tiền của bệnh nhân</param>
        /// <param name="SavedDrugList">Danh sách thuốc đã được lưu</param>
        /// <param name="Transaction">Thông tin transaction. Muốn kiểm tra xem bệnh nhân có trả tiền hết chưa thì xem trong Transaction này</param>
        //private void SaveDrugsAndPay(long registrationID, List<OutwardDrug> drugList, PatientPayment paymentDetails, out List<OutwardDrug> SavedDrugList, out PatientTransaction Transaction)
        //{
        //    throw new Exception("Not implemented");
        //}
        /// <summary>
        /// Hàm này hiện tại thấy chỉ xài cho bên nhận bệnh bảo hiểm thôi
        /// </summary>
        /// <param name="StaffID"></param>
        /// <param name="Apply15HIPercent"></param>
        /// <param name="registrationInfo"></param>
        /// <param name="SavedRegistration"></param>
        public void SaveEmptyRegistration(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, PatientRegistration registrationInfo, long V_RegistrationType, out PatientRegistration SavedRegistration)
        {
            long PatientRegistrationID;
            SavedRegistration = null;

            try
            {
                if (registrationInfo == null)
                {
                    throw new Exception(eHCMSResources.K0300_G1_ChonDK);
                }

                if (registrationInfo.ExamDate == DateTime.MinValue)
                {
                    registrationInfo.ExamDate = DateTime.Now;
                }

                //AxLogger.Instance.LogInfo("Start creating new registration.", CurrentUser);
                //RegAndPaymentProcessorBase paymentProcesssor = RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfo);
                RegAndPaymentProcessorBase paymentProcesssor = RegAndPaymentProcessorFactory.GetPaymentProcessor(PaymentProcessorType.HealthInsurance);
                paymentProcesssor.InitNewTxd(registrationInfo, true, V_RegistrationType);

                List<long> newRegDetailsList;
                List<long> newPclRequestList;

                var registrationDetails = new List<PatientRegistrationDetail>();
                var pclRequests = new List<PatientPCLRequest>();

                if (registrationInfo.AppointmentID.HasValue && registrationInfo.AppointmentID.Value > 0)
                {
                    var appointment = AppointmentProvider.Instance.GetAppointmentByID(registrationInfo.AppointmentID.Value);
                    var createdDate = DateTime.Now.Date;
                    if (appointment != null)
                    {
                        foreach (var item in appointment.PatientApptServiceDetailList)
                        {
                            var regDetails = new PatientRegistrationDetail();
                            regDetails.RefMedicalServiceItem = item.MedService;
                            regDetails.RefMedicalServiceItem.RefMedicalServiceType = new RefMedicalServiceType() { MedicalServiceTypeID = item.MedService.MedicalServiceTypeID.GetValueOrDefault(-1) };
                            regDetails.DeptLocation = item.DeptLocation;
                            regDetails.CreatedDate = createdDate;
                            regDetails.Qty = 1;
                            regDetails.RecordState = RecordState.ADDED;
                            regDetails.CanDelete = true;
                            regDetails.FromAppointment = true;
                            regDetails.StaffID = appointment.DoctorStaffID;
                            regDetails.AppointmentDate = appointment.ApptDate;
                            regDetails.ServiceSeqNum = item.ServiceSeqNum;
                            registrationDetails.Add(regDetails);
                        }

                        if (appointment.PatientApptPCLRequestsList != null)
                        {
                            foreach (var request in appointment.PatientApptPCLRequestsList)
                            {
                                var newReq = new PatientPCLRequest();
                                newReq.V_PCLRequestStatus = AllLookupValues.V_PCLRequestStatus.OPEN;
                                newReq.ExamRegStatus = AllLookupValues.ExamRegStatus.DANG_KY_KHAM;
                                newReq.CreatedDate = createdDate;
                                newReq.RecordState = RecordState.ADDED;
                                newReq.DoctorStaffID = appointment.DoctorStaffID;
                                newReq.CanDelete = true;
                                newReq.V_PCLMainCategory = request.V_PCLMainCategory;
                                newReq.Diagnosis = request.Diagnosis;
                                newReq.ICD10List = request.ICD10List;
                                newReq.DoctorComments = request.DoctorComments;


                                newReq.ObjV_PCLMainCategory = new Lookup { LookupID = 28200, ObjectValue = "Imaging" };
                                pclRequests.Add(newReq);

                                if (request.ObjPatientApptPCLRequestDetailsList != null)
                                {
                                    newReq.PatientPCLRequestIndicators = new ObservableCollection<PatientPCLRequestDetail>();
                                    foreach (var reqDetails in request.ObjPatientApptPCLRequestDetailsList)
                                    {
                                        var examType = PatientProvider.Instance.GetPclExamTypeByID(reqDetails.PCLExamTypeID);

                                        if (examType != null)
                                        {
                                            var newReqDetail = CreateRequestDetailsForPCLExamType(examType);
                                            newReqDetail.RecordState = RecordState.ADDED;
                                            newReqDetail.CreatedDate = createdDate;
                                            newReqDetail.CanDelete = true;
                                            newReqDetail.FromAppointment = true;
                                            newReqDetail.StaffID = appointment.DoctorStaffID;
                                            newReqDetail.AppointmentDate = appointment.ApptDate;
                                            newReqDetail.ServiceSeqNum = reqDetails.ServiceSeqNum;
                                            newReqDetail.PatientPCLRequest = newReq;
                                            newReq.PatientPCLRequestIndicators.Add(newReqDetail);
                                        }
                                    }
                                }
                            }
                        }
                    }

                }

                paymentProcesssor.AddServicesAndPCLRequests(StaffID, CollectorDeptLocID, Apply15HIPercent, registrationInfo, registrationDetails, pclRequests, null, null, registrationInfo.ExamDate, out PatientRegistrationID, out newRegDetailsList, out newPclRequestList);
                SavedRegistration = registrationInfo;

                SavedRegistration.PtRegistrationID = PatientRegistrationID;
                //KMx: Anh Tuấn không cho insert vào bảng PatientQueue
                //try
                //{
                //    var queue = new PatientQueue();
                //    queue.RegistrationID = PatientRegistrationID;//registrationInfo.PtRegistrationID;
                //    queue.V_QueueType = (long)AllLookupValues.QueueType.CHO_DANG_KY;
                //    queue.V_PatientQueueItemsStatus = (long)AllLookupValues.PatientQueueItemsStatus.WAITING;
                //    queue.FullName = registrationInfo.Patient.FullName;
                //    queue.PatientID = registrationInfo.Patient.PatientID;
                //    //PatientProvider.Instance.PatientQueue_Insert(queue);
                //}
                //catch
                //{
                //    throw new Exception("Đã tạo đăng ký thành công nhưng chưa thể thêm vào queue.");
                //}
                //Update lai Paperreferal ID
                if (registrationInfo.PaperReferal != null
                    && registrationInfo.PaperReferal.RefID > 0
                    && (registrationInfo.PaperReferal.PtRegistrationID == null
                        || registrationInfo.PaperReferal.PtRegistrationID < 1))
                {
                    registrationInfo.PaperReferal.PtRegistrationID = PatientRegistrationID;
                    PatientProvider.Instance.UpdatePaperReferalRegID(registrationInfo.PaperReferal);
                }
                //AxLogger.Instance.LogInfo("End of creating new registration.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SaveEmptyRegistration. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_SaveEmptyRegistration, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }

        }

        public void AddInPatientAdmission(PatientRegistration registrationInfo, InPatientDeptDetail deptDetail, long StaffID, long Staff_DeptLocationID, out long RegistrationID)
        {
            try
            {
                if (registrationInfo == null || registrationInfo.PtRegistrationID <= 0)
                {
                    throw new Exception(eHCMSResources.K0300_G1_ChonDK);
                }
                if (registrationInfo.AdmissionInfo == null)
                {
                    throw new Exception(eHCMSResources.Z1686_G1_ChuaCoTTinNV);
                }
                if (registrationInfo.V_RegistrationStatus != (long)AllLookupValues.RegistrationStatus.PENDING_INPT)
                {
                    throw new Exception(eHCMSResources.Z1687_G1_NVChoDKOTrangThaiCho);
                }
                registrationInfo.V_RegistrationStatus = (long)AllLookupValues.RegistrationStatus.OPENED;

                AxLogger.Instance.LogInfo("Start creating adding In-Patient admission.", CurrentUser);
                RegAndPaymentProcessorBase paymentProcesssor = RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfo);

                long newRegistrationId;
                long newAdmissionId;
                paymentProcesssor.AddInPatientAdmission(registrationInfo, deptDetail, StaffID, Staff_DeptLocationID, out newRegistrationId, out newAdmissionId);

                RegistrationID = registrationInfo.PtRegistrationID;

                AxLogger.Instance.LogInfo("End of adding In-Patient admission.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of adding In-Patient admission. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_AddInPatientAdmission, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }

        }

        //public void UpdateInPatientAdmDisDetails(InPatientAdmDisDetails entity, DeptLocation newDeptLoc)
        //{
        //    try
        //    {
        //        using (DbConnection conn = PatientProvider.Instance.CreateConnection())
        //        {
        //            conn.Open();

        //            DbTransaction tran = conn.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
        //            try
        //            {
        //                AxLogger.Instance.LogInfo("Start updating In-Patient admission.", CurrentUser);
        //                PatientProvider.Instance.UpdateInPatientAdmDisDetails(entity, newDeptLoc, conn, tran);
        //                AxLogger.Instance.LogInfo("End of updating In-Patient admission.", CurrentUser);
        //                tran.Commit();
        //            }
        //            catch (Exception e)
        //            {
        //                tran.Rollback();
        //                throw new Exception(e.Message);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of update In-Patient admission. Status: Failed.", CurrentUser);
        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_UpdateInPatientAdmDisDetails, CurrentUser);
        //        throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
        //    }
        //}


        public void UpdateInPatientAdmDisDetails(InPatientAdmDisDetails entity, DeptLocation newDeptLoc)
        {

            try
            {
                AxLogger.Instance.LogInfo("Start updating In-Patient admission.", CurrentUser);
                bool result = PatientProvider.Instance.UpdateInPatientAdmDisDetails(entity, newDeptLoc);
                AxLogger.Instance.LogInfo("End of updating In-Patient admission.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of update In-Patient admission. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_UpdateInPatientAdmDisDetails, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public void CreateBillingInvoiceFromExistingItems(PatientRegistration registrationInfo, InPatientBillingInvoice billingInv, out long NewBillingInvoiceID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start saving inpatient Registration.", CurrentUser);

                RegAndPaymentProcessorBase paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfo);
                paymentProcessor.CreateBillingInvoiceFromExistingItems(registrationInfo, billingInv, out NewBillingInvoiceID);

                AxLogger.Instance.LogInfo("End of saving inpatient Registration.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of saving inpatient Registration. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateBillingInvoiceFromExistingItems, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool CreateSuggestCashAdvance(long InPatientBillingInvID, long StaffID, out long RptPtCashAdvRemID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start CreateSuggestCashAdvance.", CurrentUser);

                return PatientProvider.Instance.CreateSuggestCashAdvance(InPatientBillingInvID, StaffID, out RptPtCashAdvRemID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End CreateSuggestCashAdvance. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateSuggestCashAdvance, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }

        }


        public void AddInPatientBillingInvoice(int? Apply15HIPercent, PatientRegistration registrationInfo, InPatientBillingInvoice billingInv, bool CalcPaymentToEndOfDay
            , out long PatientRegistrationID, out Dictionary<long, List<long>> DrugIDList_Error, out long NewBillingInvoiceID)
        {
            PatientRegistrationID = 0;
            DrugIDList_Error = null;
            NewBillingInvoiceID = 0;
            try
            {
                AxLogger.Instance.LogInfo("Start saving inpatient Registration.", CurrentUser);

                PatientRegistrationID = -1;
                var paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfo);

                paymentProcessor.InitNewTxd(registrationInfo, false);

                paymentProcessor.AddInPatientBillingInvoice(Apply15HIPercent, billingInv, CalcPaymentToEndOfDay, out DrugIDList_Error, out NewBillingInvoiceID);
                PatientRegistrationID = registrationInfo.PtRegistrationID;

                AxLogger.Instance.LogInfo("End of saving inpatient Registration.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of saving inpatient Registration. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateBillingInvoiceFromExistingItems, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public void UpdateInPatientBillingInvoice(long? StaffID, InPatientBillingInvoice billingInv
             , List<PatientRegistrationDetail> newRegDetails
            , List<PatientRegistrationDetail> deletedRegDetails
            , List<PatientPCLRequest> newPclRequests
            , List<PatientPCLRequestDetail> newPclRequestDetails
            , List<PatientPCLRequestDetail> deletedPclRequestDetails
            , List<OutwardDrugClinicDeptInvoice> newOutwardDrugClinicDeptInvoices
            , List<OutwardDrugClinicDeptInvoice> savedOutwardDrugClinicDeptInvoices
            , List<OutwardDrugClinicDeptInvoice> modifiedOutwardDrugClinicDeptInvoices
            , List<OutwardDrugClinicDeptInvoice> deleteOutwardDrugClinicDeptInvoices
            , List<PatientRegistrationDetail> modifiedRegDetails
             , List<PatientPCLRequestDetail> modifiedPclRequestDetails
            , out Dictionary<long, List<long>> DrugIDList_Error)
        {
            try
            {
                if (billingInv == null || billingInv.PtRegistrationID <= 0)
                {
                    throw new Exception(eHCMSResources.Z1688_G1_TTinKgHopLe);
                }
                AxLogger.Instance.LogInfo("Start update billing invoice.", CurrentUser);
                PatientRegistration registrationInfo = null;
                try
                {
                    registrationInfo = PatientProvider.Instance.GetRegistration(billingInv.PtRegistrationID
                        , (int)AllLookupValues.V_FindPatientType.NOI_TRU);//SEARCH NOI TRU
                }
                catch
                {
                    throw new Exception(eHCMSResources.Z0083_G1_KhongTimThayDK);
                }

                RegAndPaymentProcessorBase paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfo);
                paymentProcessor.UpdateInPatientBillingInvoice(StaffID, registrationInfo, billingInv
                                                                    , newRegDetails
                                                                    , deletedRegDetails
                                                                    , newPclRequests
                                                                    , newPclRequestDetails
                                                                    , deletedPclRequestDetails
                                                                    , newOutwardDrugClinicDeptInvoices
                                                                    , savedOutwardDrugClinicDeptInvoices
                                                                    , modifiedOutwardDrugClinicDeptInvoices
                                                                    , deleteOutwardDrugClinicDeptInvoices
                                                                    , modifiedRegDetails
                                                                    , modifiedPclRequestDetails
                                                                    , out DrugIDList_Error);

                AxLogger.Instance.LogInfo("End of billing invoice.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of billing invoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_UpdateInPatientBillingInvoice, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool InsertAdditionalFee(long InPatientBillingInvID, long StaffID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start InsertAdditionalFee.", CurrentUser);
                bool bRet = PatientProvider.Instance.InsertAdditionalFee(InPatientBillingInvID, StaffID);
                AxLogger.Instance.LogInfo("End InsertAdditionalFee.", CurrentUser);
                return bRet;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End InsertAdditionalFee. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_InsertAdditionalFee, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public void PayForRegistration(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, long registrationID, int FindPatient
                                        , PatientTransactionPayment paymentDetails,// PatientPayment paymentDetails,
                                                List<PatientRegistrationDetail> colPaidRegDetails,
                                                List<PatientPCLRequest> colPaidPclRequests,
                                                List<OutwardDrugInvoice> colPaidDrugInvoice,
                                                List<InPatientBillingInvoice> colBillingInvoices,

                                        out PatientTransaction Transaction,
                                        out PatientTransactionPayment paymentInfo,
                                        out List<PaymentAndReceipt> paymentInfoList,
                                        out V_RegistrationError error,
             bool checkBeforePay = false)//out PatientPayment PaymentInfo
        {
            try
            {
                if (registrationID <= 0)
                {
                    throw new Exception(eHCMSResources.Z1688_G1_TTinKgHopLe);
                }
                AxLogger.Instance.LogInfo("Start processing payment.", CurrentUser);
                PatientRegistration regInfo = null;
                Transaction = new PatientTransaction();
                error = V_RegistrationError.mNone;
                try
                {
                    // TxD 31/12/2013: Use RegAndPaymentProcessorBase get PatientRegistration then 
                    //                  assign back into RegAndPaymentProcessorBase to use without reloading in InitTxd function
                    regInfo = RegAndPaymentProcessorBase.GetRegistrationTxd(registrationID, FindPatient, true);
                    //regInfo = GetRegistrationAndOtherDetails(registrationID, FindPatient, true, true);
                }
                catch
                {
                    throw new Exception(eHCMSResources.Z0083_G1_KhongTimThayDK);
                }

                //KMx: Chặn trường hợp 2 màn hình giữa ĐKBH (Bên A) và ĐKDV (Bên B). Bên A nhận bệnh BH, Bên B load BN lên thêm DV và bấm LƯU.
                //Bên A hủy đăng ký. Bên B vẫn tính tiền các DV đã lưu được. (07/04/2014 14:35)
                if (regInfo != null && regInfo.V_RegistrationStatus == (long)AllLookupValues.RegistrationStatus.REFUND)
                {
                    throw new Exception(string.Format("{0}.",eHCMSResources.Z1693_G1_DKBiHuyKgTheTToan));
                }

                ////------kiem tra nhung danh sach them--------------------
                if (checkBeforePay)
                {
                    if (colPaidRegDetails != null)
                    {
                        var addRegDetailPaidList = CheckServiceHasPaidExist(regInfo.PatientRegistrationDetails.ToList(), colPaidRegDetails);
                        if (addRegDetailPaidList != null && addRegDetailPaidList.Count > 0)
                        {
                            error = V_RegistrationError.mRefresh;
                            Transaction = new PatientTransaction();
                            paymentInfo = new PatientTransactionPayment();
                            paymentInfoList = new List<PaymentAndReceipt>();
                            return;
                        }
                    }

                    if (colPaidPclRequests != null)
                    {
                        var addPclRequestPaidList = CheckPCLPaidExist(regInfo.PCLRequests.ToList(), colPaidPclRequests);
                        if (addPclRequestPaidList != null && addPclRequestPaidList.Count > 0)
                        {
                            error = V_RegistrationError.mRefresh;
                            Transaction = new PatientTransaction();
                            paymentInfo = new PatientTransactionPayment();
                            paymentInfoList = new List<PaymentAndReceipt>();
                            return;
                        }
                    }
                }

                RegAndPaymentProcessorBase paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(regInfo);
                paymentProcessor.InitNewTxd(regInfo, false);
                paymentProcessor.PayForRegistration(StaffID, CollectorDeptLocID, Apply15HIPercent, regInfo, paymentDetails, colPaidRegDetails, colPaidPclRequests, colPaidDrugInvoice, colBillingInvoices, out Transaction, out paymentInfo, out paymentInfoList);

                AxLogger.Instance.LogInfo("End of processing payment.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing payment. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_PayForRegistration, CurrentUser);

                //throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        private PatientPCLRequestDetail CreateRequestDetailsForPCLExamType(PCLExamType item)
        {
            var details = new PatientPCLRequestDetail();
            if (item != null)
            {
                details.PCLExamType = item;
                details.PCLExamTypeID = item.PCLExamTypeID;
                details.ExamRegStatus = AllLookupValues.ExamRegStatus.DANG_KY_KHAM;
                details.V_ExamRegStatus = (long)details.ExamRegStatus;
                if (item.PCLExamTypeLocations != null && item.PCLExamTypeLocations.Count > 0)
                {
                    details.DeptLocation = item.PCLExamTypeLocations[0].DeptLocation;
                }
            }
            return details;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="registrationID"></param>
        /// <param name="loadFromAppointment">Neu bien nay set bang true thi se vao bang cuoc hen, lay danh sach cac dich vu da duoc
        /// hen roi add vao dang ky luon. Chi dung khi load dang ky trong, load san danh sach hen de nguoi dung luu dang ky cho tien.</param>
        /// <returns></returns>
        public PatientRegistration GetRegistrationInfo(long registrationID, int FindPatient, bool loadFromAppointment = false)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading Registration Info.", CurrentUser);

                var regInfo = RegAndPaymentProcessorBase.GetRegistrationTxd(registrationID, FindPatient);
                if (loadFromAppointment)
                {
                    if (regInfo != null
                        && regInfo.AppointmentID.HasValue && regInfo.AppointmentID.Value > 0
                        && regInfo.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU
                        && regInfo.V_RegistrationStatus == (long)AllLookupValues.RegistrationStatus.PENDING
                        && (regInfo.PatientRegistrationDetails == null || regInfo.PatientRegistrationDetails.Count == 0)
                        && (regInfo.PCLRequests == null || regInfo.PCLRequests.Count == 0))
                    {
                        PatientAppointment appointment = AppointmentProvider.Instance.GetAppointmentByID(regInfo.AppointmentID.Value);
                        var createdDate = DateTime.Now.Date;
                        if (appointment != null)
                        {
                            regInfo.PatientRegistrationDetails = new ObservableCollection<PatientRegistrationDetail>();
                            foreach (var item in appointment.PatientApptServiceDetailList)
                            {
                                var regDetails = new PatientRegistrationDetail { RefMedicalServiceItem = item.MedService };
                                regDetails.RefMedicalServiceItem.RefMedicalServiceType = new RefMedicalServiceType() { MedicalServiceTypeID = item.MedService.MedicalServiceTypeID.GetValueOrDefault(-1) };
                                regDetails.DeptLocation = item.DeptLocation;
                                regDetails.CreatedDate = createdDate;
                                regDetails.Qty = 1;
                                regDetails.RecordState = RecordState.ADDED;
                                regDetails.CanDelete = true;
                                regDetails.FromAppointment = true;
                                regDetails.StaffID = appointment.DoctorStaffID;
                                regDetails.AppointmentDate = appointment.ApptDate;
                                regInfo.PatientRegistrationDetails.Add(regDetails);
                            }

                            if (appointment.PatientApptPCLRequestsList != null)
                            {
                                regInfo.PCLRequests = new ObservableCollection<PatientPCLRequest>();
                                foreach (var request in appointment.PatientApptPCLRequestsList)
                                {
                                    var newReq = new PatientPCLRequest();
                                    newReq.V_PCLRequestStatus = AllLookupValues.V_PCLRequestStatus.OPEN;
                                    newReq.CreatedDate = createdDate;
                                    newReq.RecordState = RecordState.ADDED;
                                    newReq.ReqFromDeptLocID = 0;
                                    newReq.DoctorStaffID = appointment.DoctorStaffID;
                                    newReq.CanDelete = true;
                                    newReq.V_PCLMainCategory = request.V_PCLMainCategory;
                                    newReq.Diagnosis = request.Diagnosis;
                                    newReq.ICD10List = request.ICD10List;
                                    newReq.DoctorComments = request.DoctorComments;
                                    regInfo.PCLRequests.Add(newReq);

                                    if (request.ObjPatientApptPCLRequestDetailsList != null)
                                    {
                                        newReq.PatientPCLRequestIndicators = new ObservableCollection<PatientPCLRequestDetail>();
                                        foreach (var reqDetails in request.ObjPatientApptPCLRequestDetailsList)
                                        {
                                            var examType = PatientProvider.Instance.GetPclExamTypeByID(reqDetails.PCLExamTypeID);

                                            if (examType != null)
                                            {
                                                var newReqDetail = CreateRequestDetailsForPCLExamType(examType);
                                                newReqDetail.RecordState = RecordState.ADDED;
                                                newReqDetail.CreatedDate = createdDate;
                                                newReqDetail.CanDelete = true;
                                                newReqDetail.FromAppointment = true;
                                                newReqDetail.StaffID = appointment.DoctorStaffID;
                                                newReqDetail.AppointmentDate = appointment.ApptDate;
                                                newReqDetail.DeptLocation = reqDetails.ObjDeptLocID;
                                                newReq.PatientPCLRequestIndicators.Add(newReqDetail);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return regInfo;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading Registration Info. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public InPatientAdmDisDetails GetInPatientAdmissionByRegistrationID(long regID)
        {
            if (regID <= 0)
            {
                return null;
            }
            using (DbConnection connection = PatientProvider.Instance.CreateConnection())
            {
                return PatientProvider.Instance.GetInPatientAdmissionByRegistrationID(regID, connection, null);
            }
        }


        public PatientRegistration GetRegistrationInfo_InPt(long registrationID, int FindPatient, LoadRegistrationSwitch loadRegisSwitch, bool loadFromAppointment = false)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading Registration Info.", CurrentUser);

                var regInfo = RegAndPaymentProcessorBase.GetRegistration_InPt(registrationID, FindPatient, loadRegisSwitch);

                // TxD 06/12/2014: InPatient DO NOT Load from Appointment so a block of code that was a copy of OutPatient has been removed from here
                //
                return regInfo;
            }

            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading Registration Info. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public PatientRegistration GetRegistration(long registrationID, int FindPatient)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading Registration Info.", CurrentUser);

                return PatientProvider.Instance.GetRegistration(registrationID, FindPatient);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GetRegistration. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistration, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public PatientRegistration GetRegistrationSimple(long registrationID, int PatientFindBy)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading Registration Simple.", CurrentUser);

                return PatientProvider.Instance.GetRegistrationSimple(registrationID, PatientFindBy);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading Registration Simple. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_RegistrationSimple, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public TransferForm CreateBlankTransferFormByRegID(long PtRegistrationID, int PatientFindBy, long V_TransferFormType)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start creating BlankTransferFormByRegID .", CurrentUser);

                return PatientProvider.Instance.CreateBlankTransferFormByRegID(PtRegistrationID, PatientFindBy, V_TransferFormType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading CreateBlankTransferFormByRegID . Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_RegistrationSimple, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void GetInPatientBillingInvoiceDetails(long billingInvoiceID
                                                        , out List<PatientRegistrationDetail> regDetails
                                                        , out List<PatientPCLRequest> PCLRequestList
                                                        , out List<OutwardDrugClinicDeptInvoice> allInPatientInvoices)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading all InPatient reg items.", CurrentUser);
                using (DbConnection connection = PatientProvider.Instance.CreateConnection())
                {
                    regDetails = PatientProvider.Instance.GetAllRegistrationDetailsByBillingInvoiceID(billingInvoiceID, connection, null);

                    //Can lam sang
                    PCLRequestList = PatientProvider.Instance.GetPCLRequestListByBillingInvoiceID(billingInvoiceID, connection, null);

                    allInPatientInvoices = PatientProvider.Instance.GetAllInPatientInvoicesByBillingInvoiceID(billingInvoiceID, connection, null);

                    //KMx: Bỏ ra vì thấy không cần thiết. Nếu có sử dụng lại code bên dưới thì phải set thêm NormalPriceNew, HIPatientPriceNew, HIAllowedPriceNew.
                    //Nếu không thì khi tính lại BH, những loại nào không thuộc DM BH, giá bán sẽ = 0 (30/01/2015 09:16).

                    //if (allInPatientInvoices != null)
                    //{
                    //    foreach (var inv in allInPatientInvoices)
                    //    {
                    //        if (inv.OutwardDrugClinicDepts != null)
                    //        {
                    //            foreach (var d in inv.OutwardDrugClinicDepts)
                    //            {
                    //                //d.GenMedProductItem = new RefGenMedProductDetails();

                    //                //d.GenMedProductItem = null;
                    //                //Phai gan lai no moi serialize chinh xac. Bi khung.
                    //                var item = new RefGenMedProductDetails();

                    //                item.GenMedProductID = d.GenMedProductItem.GenMedProductID;
                    //                item.BrandName = d.GenMedProductItem.BrandName;
                    //                item.GenericName = d.GenMedProductItem.GenericName;
                    //                item.Code = d.GenMedProductItem.Code;
                    //                item.HICode = d.GenMedProductItem.HICode;
                    //                item.Functions = d.GenMedProductItem.Functions;
                    //                item.TechInfo = d.GenMedProductItem.TechInfo;
                    //                item.Material = d.GenMedProductItem.Material;
                    //                item.Visa = d.GenMedProductItem.Visa;
                    //                item.DrugClassID = d.GenMedProductItem.DrugClassID;
                    //                item.PCOID = d.GenMedProductItem.PCOID;
                    //                item.CountryID = d.GenMedProductItem.CountryID;
                    //                item.Packaging = d.GenMedProductItem.Packaging;
                    //                item.UnitID = d.GenMedProductItem.UnitID;
                    //                item.UnitUseID = d.GenMedProductItem.UnitUseID;
                    //                item.UnitPackaging = d.GenMedProductItem.UnitPackaging;
                    //                item.Description = d.GenMedProductItem.Description;
                    //                item.Administration = d.GenMedProductItem.Administration;
                    //                item.Storage = d.GenMedProductItem.Storage;
                    //                item.StoreID = d.GenMedProductItem.StoreID;
                    //                item.StoreName = d.GenMedProductItem.StoreName;
                    //                item.InsuranceCover = d.GenMedProductItem.InsuranceCover;
                    //                item.IsActive = d.GenMedProductItem.IsActive;
                    //                item.V_MedProductType = d.GenMedProductItem.V_MedProductType;
                    //                item.NumberOfEstimatedMonths = d.GenMedProductItem.NumberOfEstimatedMonths;
                    //                item.FactorSafety = d.GenMedProductItem.FactorSafety;
                    //                item.SupplierMain = d.GenMedProductItem.SupplierMain;
                    //                item.SelectedUnit = d.GenMedProductItem.SelectedUnit;
                    //                item.SelectedUnitUse = d.GenMedProductItem.SelectedUnitUse;
                    //                item.SelectedCountry = d.GenMedProductItem.SelectedCountry;
                    //                item.SelectedDrugClass = d.GenMedProductItem.SelectedDrugClass;
                    //                item.PharmaceuticalCompany = d.GenMedProductItem.PharmaceuticalCompany;
                    //                item.NormalPrice = d.GenMedProductItem.NormalPrice;
                    //                item.HIPatientPrice = d.GenMedProductItem.HIPatientPrice;
                    //                item.HIAllowedPrice = d.GenMedProductItem.HIAllowedPrice;
                    //                item.InCost = d.GenMedProductItem.InCost;
                    //                item.InBuyingPrice = d.GenMedProductItem.InBuyingPrice;
                    //                item.UnitPrice = d.GenMedProductItem.UnitPrice;
                    //                item.PackagePrice = d.GenMedProductItem.PackagePrice;
                    //                item.RequestQty = d.GenMedProductItem.RequestQty;
                    //                item.Ordered = d.GenMedProductItem.Ordered;
                    //                item.Remaining = d.GenMedProductItem.Remaining;
                    //                item.RemainingFirst = d.GenMedProductItem.RemainingFirst;
                    //                item.RequiredNumber = d.GenMedProductItem.RequiredNumber;
                    //                item.WaitingDeliveryQty = d.GenMedProductItem.WaitingDeliveryQty;
                    //                item.InID = d.GenMedProductItem.InID;
                    //                item.InBatchNumber = d.GenMedProductItem.InBatchNumber;
                    //                item.InExpiryDate = d.GenMedProductItem.InExpiryDate;
                    //                item.OutPrice = d.GenMedProductItem.OutPrice;
                    //                item.STT = d.GenMedProductItem.STT;
                    //                item.SdlDescription = d.GenMedProductItem.SdlDescription;
                    //                item.RefGenMedDrugDetails = d.GenMedProductItem.RefGenMedDrugDetails;
                    //                item.ChargeableItemType = d.GenMedProductItem.ChargeableItemType;

                    //                d.GenMedProductItem = item;

                    //            }
                    //        }
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading  InPatient reg items. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_CANNOT_ADD, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public InPatientBillingInvoice GetInPatientBillingInvoice(long billingInvoiceID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading all InPatient reg items.", CurrentUser);
                using (DbConnection connection = PatientProvider.Instance.CreateConnection())
                {
                    InPatientBillingInvoice invoice = PatientProvider.Instance.GetInPatientBillingInvoice(billingInvoiceID, connection, null);
                    if (null != invoice)
                    {
                        List<PatientRegistrationDetail> regDetails = PatientProvider.Instance.GetAllRegistrationDetailsByBillingInvoiceID(billingInvoiceID, connection, null);
                        if (regDetails != null)
                        {
                            invoice.RegistrationDetails = regDetails.ToObservableCollection();
                        }
                        List<PatientPCLRequest> PCLRequestList = PatientProvider.Instance.GetPCLRequestListByBillingInvoiceID(billingInvoiceID, connection, null);
                        if (PCLRequestList != null)
                        {
                            invoice.PclRequests = PCLRequestList.ToObservableCollection();
                        }
                        List<OutwardDrugClinicDeptInvoice> allInPatientInvoices = PatientProvider.Instance.GetAllInPatientInvoicesByBillingInvoiceID(billingInvoiceID, connection, null);
                        if (allInPatientInvoices != null)
                        {
                            invoice.OutwardDrugClinicDeptInvoices = allInPatientInvoices.ToObservableCollection();
                        }
                    }
                    return invoice;


                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading  InPatient reg items. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_CANNOT_ADD, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool SaveDrugs(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, OutwardDrugInvoice OutwardInvoice, out OutwardDrugInvoice SavedOutwardInvoice)
        {
            try
            {
                SavedOutwardInvoice = null;
                long registrationID = -1;
                if (OutwardInvoice.PtRegistrationID.HasValue && OutwardInvoice.PtRegistrationID.Value > 0)
                {
                    registrationID = OutwardInvoice.PtRegistrationID.Value;
                }
                if (registrationID == -1)
                {
                    throw new Exception(string.Format("{0}.",eHCMSResources.Z1694_G1_ChuaChonDKChoToa));
                }

                PatientRegistration registrationInfo = RegAndPaymentProcessorBase.GetRegistrationTxd(registrationID, (int)AllLookupValues.V_FindPatientType.NGOAI_TRU);//chi su dung cho ngoai tru

                //PatientRegistration registrationInfo = PatientProvider.Instance.GetRegistration(registrationID
                //    , (int)AllLookupValues.V_FindPatientType.NGOAI_TRU);//chi su dung cho ngoai tru

                if (registrationInfo == null)
                {
                    throw new Exception(string.Format("{0}.",eHCMSResources.Z1695_G1_KgTimThayDKCuaToa));
                }

                RegAndPaymentProcessorBase paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfo);
                paymentProcessor.InitNewTxd(registrationInfo, false);
                List<long> newInvoiceIDList;
                paymentProcessor.AddOutwardDrugInvoice(StaffID, CollectorDeptLocID, Apply15HIPercent, OutwardInvoice, out newInvoiceIDList);

                if (newInvoiceIDList != null && newInvoiceIDList.Count > 0)
                {
                    OutwardInvoice.outiID = newInvoiceIDList[0];
                    SavedOutwardInvoice = OutwardInvoice;
                    SavedOutwardInvoice.OutwardDrugs = RefDrugGenericDetailsProvider.Instance.GetOutwardDrugDetailsByOutwardInvoice(OutwardInvoice.outiID).ToObservableCollection();
                }
                return true;

            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving AddOutwardDrugAndDetails_Prescription. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_TRANSACTION_CANNOT_ADD);
                if (ex is SqlException)
                {
                    throw new Exception(ex.Message);
                    //throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
                }
                else
                {
                    // Handle generic ones here.
                    throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                }
            }
        }

        //HPT 24/08/2015: Thêm parameter IsHICard_FiveYearsCont nhận giá trị truyền vào từ hàm gọi ở client để thực hiện tính quyền lợi bảo hiểm
        //Bệnh nhân có đăng ký bảo hiểm y tế đã hợp lệ nếu có giá trị IsHICard_FiveYearsCont = true sẽ được hưởng quyền lợi bảo hiểm 100%
        public double CalculateHiBenefit(HealthInsurance healthInsurance, PaperReferal paperReferal, out bool isConsideredAsCrossRegion, bool IsEmergency = false, long V_RegistrationType = 0, bool IsEmergInPtReExamination = false, bool IsHICard_FiveYearsCont = false, bool IsChildUnder6YearsOld = false, bool IsAllowCrossRegion = false)
        {
            HiPatientRegAndPaymentProcessor processor = new HiPatientRegAndPaymentProcessor();
            PatientRegistration regInfo = new PatientRegistration();
            regInfo.HealthInsurance = healthInsurance;
            regInfo.PaperReferal = paperReferal;
            regInfo.IsEmergency = IsEmergency;
            regInfo.IsHICard_FiveYearsCont = IsHICard_FiveYearsCont;
            regInfo.IsChildUnder6YearsOld = IsChildUnder6YearsOld;
            regInfo.IsAllowCrossRegion = IsAllowCrossRegion;
            return processor.CalcPatientHiBenefit(regInfo, regInfo.HealthInsurance, out isConsideredAsCrossRegion, V_RegistrationType, IsEmergInPtReExamination, IsAllowCrossRegion);
        }
        /*==== #002 ====*/
        public double CalculateHiBenefit_V2(HealthInsurance healthInsurance, PaperReferal paperReferal, out bool isConsideredAsCrossRegion, bool IsEmergency = false, long V_RegistrationType = 0, bool IsEmergInPtReExamination = false, bool IsHICard_FiveYearsCont = false, bool IsChildUnder6YearsOld = false, bool IsAllowCrossRegion = false, bool IsHICard_FiveYearsCont_NoPaid = false)
        {
            HiPatientRegAndPaymentProcessor processor = new HiPatientRegAndPaymentProcessor();
            PatientRegistration regInfo = new PatientRegistration();
            regInfo.HealthInsurance = healthInsurance;
            regInfo.PaperReferal = paperReferal;
            regInfo.IsEmergency = IsEmergency;
            regInfo.IsHICard_FiveYearsCont = IsHICard_FiveYearsCont;
            regInfo.IsChildUnder6YearsOld = IsChildUnder6YearsOld;
            regInfo.IsAllowCrossRegion = IsAllowCrossRegion;
            regInfo.IsHICard_FiveYearsCont_NoPaid = IsHICard_FiveYearsCont_NoPaid;
            return processor.CalcPatientHiBenefit(regInfo, regInfo.HealthInsurance, out isConsideredAsCrossRegion, V_RegistrationType, IsEmergInPtReExamination, IsAllowCrossRegion);
        }
        /*==== #002 ====*/
        //Dung de tinh tien cho truong hop Save And Pay
        public void CalcInvoiceForSaveAndPayButton(OutwardDrugInvoice OutwardInvoice, out OutwardDrugInvoice CalculatedInvoice, out PatientRegistration RegistrationInfo)
        {
            throw new Exception(eHCMSResources.Z1785_G1_NotImplemented);
        }

        /// <summary>
        /// Lưu 1 chỉ định CẬN LÂM SÀNG của bác sĩ.
        /// Trước khi lưu, tính lại giá tiền của các item details trong chỉ định này.
        /// </summary>
        /// <param name="entity">Chỉ định CẬN LÂM SÀNG cần lưu.</param>
        /// <param name="SavedPCLRequest">Thông tin chỉ định CẬN LÂM SÀNG đã được lưu trữ.</param>
        /// 

        public PatientRegistration ChangeRegistrationType(PatientRegistration regInfo, AllLookupValues.RegistrationType newType)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start changing registration Type.", CurrentUser);

                bool retVal = PatientProvider.Instance.ChangeRegistrationType(regInfo, newType);
                if (retVal == false)
                {
                    throw new Exception(string.Format("{0}.",eHCMSResources.T0432_G1_Error));
                }
                regInfo.V_RegistrationType = newType;
                AxLogger.Instance.LogInfo("End of changing registration Type. Status: Success.", CurrentUser);
                return regInfo;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of changing registration Type. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_ChangeRegistrationType, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public bool Registrations_UpdateStatus(PatientRegistration regInfo, long V_RegistrationStatus)
        {
            try
            {
                return PatientProvider.Instance.Registrations_UpdateStatus(regInfo, V_RegistrationStatus);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Registrations_UpdateStatus. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_Registrations_UpdateStatus);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public DateTime GetDate()
        {
            return DateTime.Now;
        }

        public bool Hospitalize(PatientRegistration regInfo, BedPatientAllocs selectedBed, DateTime? admissionDate, int ExpectedStayingDays, out AllLookupValues.RegistrationType NewRegType)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start changing hospitalization.", CurrentUser);
                if (regInfo.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU
                    //|| regInfo.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU_NOI_TRU
                    )
                {
                    throw new Exception(string.Format("{0}.",eHCMSResources.Z1696_G1_BNDaNV));
                }
                if (selectedBed == null)
                {
                    throw new Exception(string.Format("{0}.",eHCMSResources.Z1697_G1_ChuaDatGiuong));
                }
                if (!admissionDate.HasValue)
                {
                    admissionDate = GetDate();
                }
                //Dat giuong cho benh nhan truoc. Neu OK moi chuyen sang dang ky noi tru.
                bool bookOK = BedAllocations.Instance.AddNewBedPatientAllocs(selectedBed.BedAllocationID, regInfo.PtRegistrationID
                                                  , admissionDate
                                                  , ExpectedStayingDays
                                                  , null
                                                  , true);
                if (bookOK)
                {
                    AllLookupValues.RegistrationType newType = AllLookupValues.RegistrationType.Unknown;
                    if (regInfo.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
                    {
                        //newType = AllLookupValues.RegistrationType.NGOAI_TRU_NOI_TRU;
                    }
                    else if (regInfo.V_RegistrationType == AllLookupValues.RegistrationType.Unknown)
                    {
                        newType = AllLookupValues.RegistrationType.NOI_TRU;
                    }
                    bool retVal = PatientProvider.Instance.ChangeRegistrationType(regInfo, newType);
                    if (!retVal)
                    {
                        BedAllocations.Instance.DeleteBedAllocation(selectedBed.BedAllocationID);
                        throw new Exception(string.Format("{0}.",eHCMSResources.Z1698_G1_KgTheNVChoBN));
                    }
                    NewRegType = newType;

                    AxLogger.Instance.LogInfo("End of changing hospitalization. Status: Success.", CurrentUser);
                    return true;
                }
                else
                {
                    throw new Exception(string.Format("{0}.",eHCMSResources.Z1699_G1_KgTheDatGiuongChoBN));
                }
            }

            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of changing hospitalization. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_Hospitalize, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        //public void UpdatePCLRequest(long StaffID, PatientPCLRequest request,out List<PatientPCLRequest> listPclSave, DateTime modifiedDate = default(DateTime))
        //{
        //    try
        //    {
        //        AxLogger.Instance.LogInfo("Start updating Pcl Request.", CurrentUser);
        //        List<long> newRegDetailsList;
        //        List<long> newPclRequestList;
        //        int FindPatient = 0;
        //        if(request.V_RegistrationType==(long)AllLookupValues.RegistrationType.NOI_TRU)
        //        {
        //            FindPatient = 1;
        //        }
        //        var regInfo = PatientProvider.Instance.GetRegistration(request.PtRegistrationID, FindPatient);
        //        if (regInfo == null)
        //        {
        //            throw new Exception(string.Format("{0}.",eHCMSResources.Z0083_G1_KhongTimThayDK));
        //        }
        //        if(modifiedDate == default(DateTime))
        //        {
        //            modifiedDate = DateTime.Now;
        //        }
        //        var paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(regInfo);
        //        paymentProcessor.UpdatePCLRequest(StaffID, regInfo, request,out listPclSave, modifiedDate);

        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of updating pclRequest. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_AddServicesAndPCLRequests, CurrentUser);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }


        //}

        public void CreateNewRegistrationVIP(PatientRegistration regInfo, out long newRegistrationID)
        {
            try
            {
                using (DbConnection conn = PatientProvider.Instance.CreateConnection())
                {
                    conn.Open();

                    int sequenceNo;
                    regInfo.ExamDate = GetDate();
                    //regInfo.V_RegistrationType = AllLookupValues.RegistrationType.DANGKY_VIP;
                    PatientProvider.Instance.AddRegistration(regInfo, conn, null, out newRegistrationID, out sequenceNo);
                    regInfo.PtRegistrationID = newRegistrationID;
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of CreateNewRegistrationVIP. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_VIP, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        ////////////////////////////////////////////////////////////

        public List<PatientRegistrationDetail> CheckServiceExist(List<PatientRegistrationDetail> regDetails, List<PatientRegistrationDetail> regDetailList)
        {
            //loc danh sach regDetailList ngoai tru nhung phan tu nam trong regDetails
            if (regDetailList == null || regDetailList.Count < 1)
            {
                return null;
            }
            if (regDetails == null || regDetails.Count < 1)
            {
                return regDetailList;
            }
            return regDetailList.Where(
                u => regDetailList.Select(i => i.PtRegDetailID).Except(regDetails.
                    Select(l => l.PtRegDetailID)).Contains(u.PtRegDetailID)).ToList();
        }

        public List<PatientRegistrationDetail> CheckServiceHasPaidExist(List<PatientRegistrationDetail> regDetailList, List<PatientRegistrationDetail> delRegDetailList, bool isDoing = false)
        {
            if (regDetailList == null || delRegDetailList == null)
            {
                return null;
            }
            if (isDoing)
            {
                return delRegDetailList.Where(
                u => delRegDetailList.Select(i => i.PtRegDetailID).Intersect(regDetailList.Where(s =>
                    (s.PaidTime != null && s.ExamRegStatus != AllLookupValues.ExamRegStatus.DANG_KY_KHAM
                    && s.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI))
                    .Select(l => l.PtRegDetailID)).Contains(u.PtRegDetailID)).ToList();
            }
            else
            {
                return delRegDetailList.Where(
                u => delRegDetailList.Select(i => i.PtRegDetailID).Intersect(regDetailList.Where(s => (s.PaidTime != null))
                    .Select(l => l.PtRegDetailID)).Contains(u.PtRegDetailID)).ToList();
            }

        }

        public List<PatientPCLRequestDetail> CheckPCLExist(List<PatientPCLRequest> regPCLInfo, List<PatientPCLRequestDetail> regPCLDetails, bool isDoing = false)
        {
            if (regPCLDetails == null || regPCLDetails.Count < 1)
            {
                return null;
            }
            if (regPCLInfo == null || regPCLInfo.Count < 1)
            {
                return regPCLDetails;
            }
            List<PatientPCLRequestDetail> temp = new List<PatientPCLRequestDetail>();
            foreach (var item in regPCLInfo)
            {
                temp.AddRange(regPCLDetails.Where(
                u => regPCLDetails.Select(i => i.PCLReqItemID).
                    Except(item.PatientPCLRequestIndicators.Select(k => k.PCLReqItemID)
                    ).Contains(u.PCLReqItemID)).ToList());
            }
            return temp;
        }

        public List<PatientPCLRequestDetail> CheckPCLPaidExist(List<PatientPCLRequest> regPCLInfo, List<PatientPCLRequestDetail> regPCLDetails, bool isDoing = false)
        {
            List<PatientPCLRequestDetail> temp = new List<PatientPCLRequestDetail>();
            foreach (var item in regPCLInfo)
            {
                if (isDoing)
                {
                    temp.AddRange(regPCLDetails.Where(
                    u => regPCLDetails.Select(i => i.PCLReqItemID).
                        Intersect(item.PatientPCLRequestIndicators.Where(s => (s.PaidTime != null && s.ExamRegStatus != AllLookupValues.ExamRegStatus.DANG_KY_KHAM
                            && s.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI))
                        .Select(k => k.PCLReqItemID)
                        ).Contains(u.PCLReqItemID)).ToList());
                }
                else
                {
                    temp.AddRange(regPCLDetails.Where(
                    u => regPCLDetails.Select(i => i.PCLReqItemID).
                        Intersect(item.PatientPCLRequestIndicators.Where(s => (s.PaidTime != null))
                        .Select(k => k.PCLReqItemID)
                        ).Contains(u.PCLReqItemID)).ToList());
                }
            }
            return temp;
        }

        public List<PatientPCLRequest> CheckPCLPaidExist(List<PatientPCLRequest> regPCLInfo, List<PatientPCLRequest> regPCLDetails, bool isDoing = false)
        {
            List<PatientPCLRequest> temps = new List<PatientPCLRequest>();
            foreach (var item in regPCLDetails)
            {
                var pclDetails = CheckPCLPaidExist(regPCLInfo, item.PatientPCLRequestIndicators.ToList(), isDoing);
                if (pclDetails != null && pclDetails.Count > 0)
                {
                    var temp = item;
                    temp.PatientPCLRequestIndicators = new ObservableCollection<PatientPCLRequestDetail>();
                    temp.PatientPCLRequestIndicators.AddRange(pclDetails);
                    temps.Add(temp);
                }
            }
            return temps;
        }

        private PatientRegistration GetRegistrationAndOtherDetails(long nPtRegistrationID, int nFindPatient, bool bGetRegDetails, bool bGetPCLReqDetails)
        {
            PatientRegistration regInfo = PatientProvider.Instance.GetRegistration(nPtRegistrationID, nFindPatient);
            List<PatientPCLRequest> pclRequestList = null;
            List<PatientRegistrationDetail> regDetails = null;

            using (DbConnection connection = PatientProvider.Instance.CreateConnection())
            {
                if (bGetRegDetails)
                {
                    regDetails = PatientProvider.Instance.GetAllRegistrationDetails(nPtRegistrationID, nFindPatient, connection, null);

                    if (regDetails != null)
                    {
                        regInfo.PatientRegistrationDetails = new ObservableCollection<PatientRegistrationDetail>();
                        foreach (var item in regDetails)
                        {
                            regInfo.PatientRegistrationDetails.Add(item);
                        }
                    }
                }

                if (bGetPCLReqDetails)
                {
                    pclRequestList = PatientProvider.Instance.GetPCLRequestListByRegistrationID(nPtRegistrationID, connection, null);
                    if (pclRequestList != null)
                    {
                        regInfo.PCLRequests = pclRequestList.ToObservableCollection();
                    }
                }
            }

            return regInfo;
        }

        public void AddPCLRequestsForInPt(long StaffID, long ReqFromDeptLocID, long ReqFromDeptID, PatientRegistration regInfo, PatientPCLRequest pclRequest, PatientPCLRequest deletedPclRequest,
                        out List<PatientPCLRequest> SavedPclRequestList, DateTime modifiedDate = default(DateTime))
        {
            try
            {
                SavedPclRequestList = null;

                AxLogger.Instance.LogInfo("Start registering patient.", CurrentUser);
                List<long> newPclRequestList = null;
                bool bLoadRegInforequired = true;
                if (regInfo.PtRegistrationID > 0)
                {
                    regInfo = RegAndPaymentProcessorBase.GetRegistrationTxd(regInfo.PtRegistrationID, regInfo.FindPatient, true);
                    bLoadRegInforequired = false; // Registration Info already loaded here NO NEED to load it again in paymentProcessor (RegAndPaymentProcessorBase) below
                }
                if (regInfo == null)
                {
                    throw new Exception(string.Format("{0}.",eHCMSResources.Z0083_G1_KhongTimThayDK));
                }
                if (regInfo.ExamDate == DateTime.MinValue)
                {
                    regInfo.ExamDate = DateTime.Now;
                }
                if (modifiedDate == default(DateTime))
                {
                    modifiedDate = regInfo.ExamDate;
                }        
                if (pclRequest != null && pclRequest.PatientPCLRequestIndicators != null && pclRequest.PatientPCLRequestIndicators.Count() > 0)
                {
                    if (pclRequest.CreatedDate == DateTime.MinValue)
                    {
                        pclRequest.CreatedDate = DateTime.Now;
                    }
                    if (pclRequest.PatientPCLRequestIndicators != null)
                    {
                        foreach (var requestDetail in pclRequest.PatientPCLRequestIndicators)
                        {
                            if (requestDetail.CreatedDate == DateTime.MinValue)
                            {
                                requestDetail.CreatedDate = DateTime.Now;
                            }
                        }
                    }
                }
                RegAndPaymentProcessorBase paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(regInfo);
                paymentProcessor.InitNewTxd(regInfo, bLoadRegInforequired); // Bước này sẽ gán dữ liệu từ regInfo sang cho CurrentRegistration
                RetryOnDatabaseDeadlock.RetryUntil(() =>
                {
                    paymentProcessor.AddPCLRequestsForInPt(StaffID, ReqFromDeptLocID, ReqFromDeptID, pclRequest, deletedPclRequest, modifiedDate, out newPclRequestList);
                }, 5);

                if (newPclRequestList != null)
                {
                    SavedPclRequestList = PatientProvider.Instance.GetPCLRequestListByIDList(newPclRequestList, (long)AllLookupValues.RegistrationType.NOI_TRU);
                }
                AxLogger.Instance.LogInfo("End of registering patient.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddServicesAndPCLRequestsInPt. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_AddServicesAndPCLRequests, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public PatientPCLRequest UpdateDrAndDiagTrmtForPCLReq(long ServiceRecID, long PCLReqID, long DoctorStaffID)
        {
            try
            {
                return PatientProvider.Instance.UpdateDrAndDiagTrmtForPCLReq(ServiceRecID, PCLReqID, DoctorStaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateDrAndDiagTrmtForPCLReq. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_AddServicesAndPCLRequests, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public PatientRegistration AddServicesAndPCLRequests(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, PatientRegistration regInfo, List<PatientRegistrationDetail> regDetailList, List<PatientPCLRequest> pclRequestList, List<PatientRegistrationDetail> deletedRegDetailList, List<PatientPCLRequest> deletedPclRequestList, out long NewRegistrationID, out List<PatientRegistrationDetail> SavedRegistrationDetailList
            , out List<PatientPCLRequest> SavedPclRequestList, out V_RegistrationError error, DateTime modifiedDate = default(DateTime))
        {
            try
            {
                SavedRegistrationDetailList = null;
                SavedPclRequestList = null;


                AxLogger.Instance.LogInfo("Start registering patient.", CurrentUser);
                List<long> newRegDetailsList = null;
                List<long> newPclRequestList = null;
                bool bLoadRegInforequired = true;
                error = V_RegistrationError.mNone;
                if (regInfo.PtRegistrationID > 0)
                {
                    regInfo = RegAndPaymentProcessorBase.GetRegistrationTxd(regInfo.PtRegistrationID, regInfo.FindPatient, true);
                    //regInfo = GetRegistrationAndOtherDetails(regInfo.PtRegistrationID, regInfo.FindPatient, true, true);
                    bLoadRegInforequired = false; // Registration Info already loaded here NO NEED to load it again in paymentProcessor (RegAndPaymentProcessorBase) below

                    //regInfo = PatientProvider.Instance.GetRegistration(regInfo.PtRegistrationID, regInfo.FindPatient);
                    //regInfo = GetRegistrationInfo(regInfo.PtRegistrationID, regInfo.FindPatient);
                }

                if (regInfo == null)
                {
                    throw new Exception(string.Format("{0}.",eHCMSResources.Z0083_G1_KhongTimThayDK));
                }
                if (regInfo.ExamDate == DateTime.MinValue)
                {
                    regInfo.ExamDate = DateTime.Now;
                }
                if (modifiedDate == default(DateTime))
                {
                    modifiedDate = regInfo.ExamDate;
                }
                //b2d Phải Kiểm Tra thêm ở đây một lần nữa
                //xem dịch vụ cls bị xóa đã có tính tiền chưa thì tra ve error
                //b2d kiểm tra những danh sách xóa
                if (deletedRegDetailList != null && deletedRegDetailList.Count > 0)
                {
                    var deletedRegDetailPaidList = CheckServiceHasPaidExist(regInfo.PatientRegistrationDetails.ToList(), deletedRegDetailList);
                    if (deletedRegDetailPaidList != null && deletedRegDetailPaidList.Count > 0)
                    {
                        error = V_RegistrationError.mRefresh;
                        NewRegistrationID = 0;
                        return null;
                    }
                }

                if (deletedPclRequestList != null && deletedPclRequestList.Count > 0 && regInfo.PCLRequests != null && regInfo.PCLRequests.Count > 0)
                {
                    var deletedPclRequestPaidList = CheckPCLPaidExist(regInfo.PCLRequests.ToList(), deletedPclRequestList);
                    if (deletedPclRequestPaidList != null && deletedPclRequestPaidList.Count > 0)
                    {
                        error = V_RegistrationError.mRefresh;
                        NewRegistrationID = 0;
                        return null;
                    }
                }

                ////------kiem tra nhung danh sach them--------------------

                if (regDetailList != null)
                {
                    var addRegDetailPaidList = CheckServiceHasPaidExist(regInfo.PatientRegistrationDetails.ToList(), regDetailList);
                    if (addRegDetailPaidList != null && addRegDetailPaidList.Count > 0)
                    {
                        error = V_RegistrationError.mRefresh;
                        NewRegistrationID = 0;
                        return null;
                    }
                    foreach (var patientRegistrationDetail in regDetailList)
                    {
                        if (patientRegistrationDetail.CreatedDate == DateTime.MinValue)
                        {
                            //patientRegistrationDetail.CreatedDate = regInfo.ExamDate;
                            patientRegistrationDetail.CreatedDate = DateTime.Now;
                        }
                    }
                }

                if (pclRequestList != null)
                {
                    var addPclRequestPaidList = CheckPCLPaidExist(regInfo.PCLRequests.ToList(), pclRequestList);
                    if (addPclRequestPaidList != null && addPclRequestPaidList.Count > 0)
                    {
                        error = V_RegistrationError.mRefresh;
                        NewRegistrationID = 0;
                        return null;
                    }
                    foreach (var pclRequest in pclRequestList)
                    {
                        if (pclRequest.CreatedDate == DateTime.MinValue)
                        {
                            pclRequest.CreatedDate = DateTime.Now;
                            //pclRequest.CreatedDate = regInfo.ExamDate;
                        }
                        if (pclRequest.PatientPCLRequestIndicators != null)
                        {
                            foreach (var requestDetail in pclRequest.PatientPCLRequestIndicators)
                            {
                                if (requestDetail.CreatedDate == DateTime.MinValue)
                                {
                                    requestDetail.CreatedDate = DateTime.Now;
                                    //requestDetail.CreatedDate = regInfo.ExamDate;
                                }
                            }
                        }
                    }
                }

                RegAndPaymentProcessorBase paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(regInfo);
                paymentProcessor.InitNewTxd(regInfo, bLoadRegInforequired);
                long id = regInfo.PtRegistrationID > 0 ? regInfo.PtRegistrationID : -1;


                RetryOnDatabaseDeadlock.RetryUntil(() =>
                        {
                            paymentProcessor.AddServicesAndPCLRequests(StaffID, CollectorDeptLocID, Apply15HIPercent, regInfo, regDetailList, pclRequestList, deletedRegDetailList, deletedPclRequestList, modifiedDate, out id, out newRegDetailsList, out newPclRequestList);
                        }, 5);

                NewRegistrationID = id;

                if (newRegDetailsList != null)
                {
                    //Lay danh sach dich vu vua add.
                    SavedRegistrationDetailList = PatientProvider.Instance.GetAllRegistrationDetailsByIDList(newRegDetailsList);
                }
                if (newPclRequestList != null)
                {
                    SavedPclRequestList = PatientProvider.Instance.GetPCLRequestListByIDList(newPclRequestList);
                }
                AxLogger.Instance.LogInfo("End of registering patient.", CurrentUser);

                //Add vao QUEUE
                PatientQueue queue;
                if (SavedRegistrationDetailList != null)
                {
                    foreach (var item in SavedRegistrationDetailList)
                    {
                        queue = new PatientQueue();
                        queue.RegistrationID = NewRegistrationID;//regInfo.PtRegistrationID;
                        queue.RegistrationDetailsID = item.PtRegDetailID;
                        queue.V_QueueType = (long)AllLookupValues.QueueType.THANH_TOAN_TIEN_KHAM;
                        queue.V_PatientQueueItemsStatus = (long)AllLookupValues.PatientQueueItemsStatus.WAITING;
                        queue.DeptLocID = item.DeptLocID;
                        queue.FullName = regInfo.Patient.FullName;
                        queue.PatientID = regInfo.Patient.PatientID;
                        //KMx: Anh Tuấn không cho insert vào bảng PatientQueue.
                        //PatientProvider.Instance.PatientQueue_Insert(queue);
                    }
                }

                if (SavedPclRequestList != null)
                {
                    foreach (var item in SavedPclRequestList)
                    {
                        queue = new PatientQueue();
                        queue.RegistrationID = NewRegistrationID;// regInfo.PtRegistrationID;
                        queue.PatientPCLReqID = item.PatientPCLReqID;
                        queue.V_QueueType = (long)AllLookupValues.QueueType.THANH_TOAN_TIEN_KHAM;
                        queue.V_PatientQueueItemsStatus = (long)AllLookupValues.PatientQueueItemsStatus.WAITING;
                        queue.DeptLocID = item.PCLDeptLocID;
                        queue.FullName = regInfo.Patient.FullName;
                        queue.PatientID = regInfo.Patient.PatientID;
                        //KMx: Anh Tuấn không cho insert vào bảng PatientQueue.
                        //PatientProvider.Instance.PatientQueue_Insert(queue);
                    }
                }

                //Update lai Paperreferal ID
                if (regInfo.PaperReferal != null
                    && regInfo.PaperReferal.RefID > 0
                    && (regInfo.PaperReferal.PtRegistrationID == null
                        || regInfo.PaperReferal.PtRegistrationID < 1))
                {
                    regInfo.PaperReferal.PtRegistrationID = NewRegistrationID;
                    PatientProvider.Instance.UpdatePaperReferalRegID(regInfo.PaperReferal);
                }

                // TxD 01/01/2014 : Return PatientRegistration HERE to SAVE the Client a TRIP to CALL and GetRegistrationInfo

                PatientRegistration newRegInfoReloaded = RegAndPaymentProcessorBase.GetRegistrationTxd(NewRegistrationID, regInfo.FindPatient, true);

                return newRegInfoReloaded;

            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddServicesAndPCLRequests. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_AddServicesAndPCLRequests, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void DeletePCLRequestWithDetails(Int64 PatientPCLReqID, out string Result)
        {
            try
            {
                PatientProvider.Instance.DeletePCLRequestWithDetails(PatientPCLReqID, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DeletePCLRequestWithDetails. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_DeletePCLRequestWithDetails);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void DeleteInPtPCLRequestWithDetails(Int64 PatientPCLReqID)
        {
            try
            {
                PatientProvider.Instance.DeleteInPtPCLRequestWithDetails(PatientPCLReqID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DeleteInPtPCLRequestWithDetails. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_DeletePCLRequestWithDetails);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }            
        }

        public void AddPCLRequest(long StaffID, long patientID, long registrationID, PatientPCLRequest pclRequest, out PatientPCLRequest SavedPclRequest)
        {
            try
            {
                SavedPclRequest = null;
                if (patientID <= 0 && registrationID <= 0)
                {
                    throw new Exception(string.Format("{0}.",eHCMSResources.Z1792_G1_ThSoTruyenVaoKgDung));
                }
                AxLogger.Instance.LogInfo("Start adding pcl Request.", CurrentUser);
                long newPclRequest;
                if (registrationID > 0)
                {
                    int PatientFind = 0;
                    if (pclRequest.V_RegistrationType == (int)AllLookupValues.RegistrationType.NOI_TRU)
                    {
                        PatientFind = (int)AllLookupValues.V_FindPatientType.NOI_TRU;
                    }

                    PatientRegistration regInfo = PatientProvider.Instance.GetRegistration(registrationID, PatientFind);
                    if (regInfo == null)
                    {
                        throw new Exception(string.Format("{0}.",eHCMSResources.Z0083_G1_KhongTimThayDK));
                    }
                    RegAndPaymentProcessorBase paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(regInfo);
                    paymentProcessor.AddPCLRequest(StaffID, regInfo, pclRequest, out newPclRequest);
                }
                else //Thêm yêu cầu CLS không không cho bệnh nhân.
                {
                    NormalPatientRegAndPaymentProcessor paymentProcessor = new NormalPatientRegAndPaymentProcessor();
                    paymentProcessor.AddPCLRequestForNonRegisteredPatient(patientID, pclRequest, pclRequest.V_RegistrationType, out newPclRequest);

                }

                if (newPclRequest > 0)
                {
                    List<long> ids = new List<long>() { newPclRequest };
                    List<PatientPCLRequest> requests = PatientProvider.Instance.GetPCLRequestListByIDList(ids);
                    if (requests != null && requests.Count > 0)
                    {
                        SavedPclRequest = requests[0];
                    }
                }
                AxLogger.Instance.LogInfo("End of adding pcl Request.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of adding pcl Request. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_AddPCLRequest, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }


        }

        public void RemovePaidRegItems(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, long registrationID, int FindPatient,
                        List<PatientRegistrationDetail> colPaidRegDetails,
                        List<PatientPCLRequest> colPaidPclRequests,
                        List<OutwardDrugInvoice> colPaidDrugInvoice,
                        List<OutwardDrugClinicDeptInvoice> colPaidMedItemList,
                        List<OutwardDrugClinicDeptInvoice> colPaidChemicalItem
                        , out V_RegistrationError error)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start removing registered items.", CurrentUser);
                //Tim lai dang ky, neu chua co dang ky thi bao loi.
                //Neu dang ky nay chua co transaction cung khong duoc. Vi day la truong hop xoa nhung item da tinh tien roi -> da ton tai Transaction
                bool bLoadRegInforequired = true;
                error = V_RegistrationError.mNone;
                PatientRegistration regInfo = null;
                try
                {
                    //registrationInfo = PatientProvider.Instance.GetRegistration(registrationID, FindPatient);
                    regInfo = RegAndPaymentProcessorBase.GetRegistrationTxd(registrationID, FindPatient, true);
                    //regInfo = GetRegistrationAndOtherDetails(registrationID, FindPatient, true, true);
                    bLoadRegInforequired = false; // Registration Info already loaded here NO NEED to load it again in paymentProcessor (RegAndPaymentProcessorBase) below
                }
                catch
                {
                    throw new Exception(eHCMSResources.Z0083_G1_KhongTimThayDK);
                }
                if (regInfo == null)//|| registrationInfo.PatientTransaction == null
                {
                    throw new Exception(string.Format("{0}.",eHCMSResources.Z1700_G1_KgTimThayGDichCuaDK));
                }

                //b2d Phải Kiểm Tra thêm ở đây một lần nữa
                //xem dịch vụ cls bị xóa đã có tính tiền chưa thì tra ve error
                //b2d kiểm tra những danh sách xóa
                if (colPaidRegDetails != null && colPaidRegDetails.Count > 0)
                {
                    var deletedRegDetailPaidList = CheckServiceHasPaidExist(regInfo.PatientRegistrationDetails.ToList(), colPaidRegDetails, true);
                    if (deletedRegDetailPaidList != null && deletedRegDetailPaidList.Count > 0)
                    {
                        error = V_RegistrationError.mRefresh;
                        return;
                    }
                }

                if (colPaidPclRequests != null && colPaidPclRequests.Count > 0
                    && regInfo.PCLRequests != null && regInfo.PCLRequests.Count > 0)
                {
                    var deletedPclRequestPaidList = CheckPCLPaidExist(regInfo.PCLRequests.ToList(), colPaidPclRequests, true);
                    if (deletedPclRequestPaidList != null && deletedPclRequestPaidList.Count > 0)
                    {
                        error = V_RegistrationError.mRefresh;
                        return;
                    }
                }

                RegAndPaymentProcessorBase paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(regInfo);
                paymentProcessor.InitNewTxd(regInfo, bLoadRegInforequired);

                paymentProcessor.RemovePaidRegItems(StaffID, CollectorDeptLocID, Apply15HIPercent, regInfo, colPaidRegDetails, colPaidPclRequests,
                                                    colPaidDrugInvoice, colPaidMedItemList, colPaidChemicalItem);
                AxLogger.Instance.LogInfo("End of removing registered items.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of removing registered items. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_RemovePaidRegItems, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Outward">Thực ra Ny gửi lên ở đây là Phiếu xuất chứ không phải phiểu trả</param>
        /// <param name="Details"></param>
        /// <param name="outiID"></param>
        /// <returns></returns>
        public bool AddOutwardDrugReturn(long StaffID, int? Apply15HIPercent, OutwardDrugInvoice Outward, List<OutwardDrug> Details, out long outiID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start returning outward drug invoice.", CurrentUser);

                //Tim lai dang ky, neu chua co dang ky thi bao loi.
                //Neu dang ky nay chua co transaction cung khong duoc. Vi day la truong hop xoa nhung item da tinh tien roi -> da ton tai Transaction
                PatientRegistration registrationInfo = null;
                if (!Outward.PtRegistrationID.HasValue)
                {
                    throw new Exception(eHCMSResources.Z1793_G1_KgCoDK);
                }
                try
                {
                    //registrationInfo = PatientProvider.Instance.GetRegistration(Outward.PtRegistrationID.Value
                    //    , (int)AllLookupValues.V_FindPatientType.NGOAI_TRU);
                    registrationInfo = RegAndPaymentProcessorBase.GetRegistrationTxd(Outward.PtRegistrationID.Value, (int)AllLookupValues.V_FindPatientType.NGOAI_TRU, true);
                }
                catch
                {
                    throw new Exception(eHCMSResources.Z0083_G1_KhongTimThayDK);
                }
                if (registrationInfo == null || registrationInfo.PatientTransaction == null)
                {
                    throw new Exception(string.Format("{0}.",eHCMSResources.Z1700_G1_KgTimThayGDichCuaDK));
                }
                var paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfo);
                paymentProcessor.InitNewTxd(registrationInfo, false);
                paymentProcessor.AddOutwardDrugReturn(StaffID, Apply15HIPercent, Outward, Details, out outiID);
                AxLogger.Instance.LogInfo("End of returning outward drug invoice.", CurrentUser);
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of returning outward drug invoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_OUTWARD_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool CancelOutwardDrugInvoice(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, OutwardDrugInvoice invoice)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start returning outward drug invoice.", CurrentUser);
                //Tim lai dang ky, neu chua co dang ky thi bao loi.
                //Neu dang ky nay chua co transaction cung khong duoc. Vi day la truong hop xoa nhung item da tinh tien roi -> da ton tai Transaction
                PatientRegistration registrationInfo = null;
                if (!invoice.PtRegistrationID.HasValue)
                {
                    throw new Exception(eHCMSResources.Z1793_G1_KgCoDK);
                }
                try
                {
                    //registrationInfo = PatientProvider.Instance.GetRegistration(invoice.PtRegistrationID.Value
                    //    , (int)AllLookupValues.V_FindPatientType.NGOAI_TRU);
                    registrationInfo = RegAndPaymentProcessorBase.GetRegistrationTxd(invoice.PtRegistrationID.Value, (int)AllLookupValues.V_FindPatientType.NGOAI_TRU);
                }
                catch
                {
                    throw new Exception(eHCMSResources.Z0083_G1_KhongTimThayDK);
                }
                if (registrationInfo == null || registrationInfo.PatientTransaction == null)
                {
                    throw new Exception(string.Format("{0}.",eHCMSResources.Z1700_G1_KgTimThayGDichCuaDK));
                }

                RegAndPaymentProcessorBase paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfo);
                paymentProcessor.InitNewTxd(registrationInfo, false);
                paymentProcessor.CancelOutwardDrugInvoice(StaffID, CollectorDeptLocID, Apply15HIPercent, invoice);
                AxLogger.Instance.LogInfo("End of returning outward drug invoice.", CurrentUser);
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of returning outward drug invoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CancelOutwardDrugInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Hospital SearchHospitalByHICode(string HiCode)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start searching hospitals by HICode.", CurrentUser);

                Hospital hospital = PatientProvider.Instance.SearchHospitalByHICode(HiCode);

                AxLogger.Instance.LogInfo("End of searching hospitals by HICode. Status: Success.", CurrentUser);
                return hospital;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching hospitals by HICode. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_SearchHospitalByHICode, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<Hospital> LoadCrossRegionHospitals()
        {
            try
            {
                AxLogger.Instance.LogInfo("Start getting list of hospitals which are allowed to cross region.", CurrentUser);

                List<Hospital> hospital = PatientProvider.Instance.LoadCrossRegionHospitals();

                AxLogger.Instance.LogInfo("End getting list of hospitals which are allowed to cross region.", CurrentUser);
                return hospital;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End getting list of hospitals which are allowed to cross region. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_SearchHospitalByHICode, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<Hospital> SearchHospitals(string HospitalName, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start searching hospitals.", CurrentUser);

                List<Hospital> hospitals = PatientProvider.Instance.SearchHospitals(HospitalName, pageIndex, pageSize, bCountTotal, out totalCount);

                AxLogger.Instance.LogInfo("End of searching hospitals. Status: Success.", CurrentUser);
                return hospitals;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching hospitals. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_SearchHospitals, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<Hospital> SearchHospitalsNew(HospitalSearchCriteria entity, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start searching hospitals.", CurrentUser);

                List<Hospital> hospitals = PatientProvider.Instance.SearchHospitalsNew(entity, pageIndex, pageSize, bCountTotal, out totalCount);

                AxLogger.Instance.LogInfo("End of searching hospitals. Status: Success.", CurrentUser);
                return hospitals;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching hospitals. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_SearchHospitals, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool CancelRegistration(PatientRegistration registrationInfo, out PatientRegistration cancelledRegistration)
        {
            try
            {
                cancelledRegistration = null;
                AxLogger.Instance.LogInfo("Start removing registration.", CurrentUser);
                bool bOK = false;
                RegAndPaymentProcessorBase paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfo);
                //HPT 25/08/2015: Trước đây chỉ có thể hủy đăng ký ngoại trú nên không phân biệt
                //Bổ sung điều kiện loại đăng ký, tùy theo là đăng ký ngoại trú hay nội trú mà dùng hàm GetRegistrationInfo hay GetRegistrationInfo_InPt
                //Thêm câu lệnh if ở đây để khỏi định nghĩa thêm một hàm CancelRegistration_InPt trong service. 
                if (registrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
                {
                    bOK = paymentProcessor.CancelRegistration(registrationInfo);
                    if (bOK)
                    {
                        cancelledRegistration = GetRegistrationInfo(registrationInfo.PtRegistrationID, registrationInfo.FindPatient);
                    }
                }
                if (registrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
                {
                    bOK = paymentProcessor.CancelRegistration_InPt(registrationInfo);
                    if (bOK)
                    {
                        //HPT 26/08/2015: đối tượng loadregtask xác định sẽ load lên những phần nào của đăng ký để tránh load dư thừa dữ liệu
                        LoadRegistrationSwitch loadregtask = new LoadRegistrationSwitch();
                        loadregtask.IsGetPatient = true;
                        loadregtask.IsGetRegistration = true;
                        cancelledRegistration = GetRegistrationInfo_InPt(registrationInfo.PtRegistrationID, registrationInfo.FindPatient, loadregtask, false);
                    }
                }
                AxLogger.Instance.LogInfo("End of removing registered items.", CurrentUser);
                return bOK;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of removing registration. Status: Failed.", CurrentUser);
                throw new Exception(ex.Message);

                //AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CancelRegistration, CurrentUser);
                //throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        #region Ny them member

        public bool AddTransactionForDrug(PatientTransactionPayment payment, long outiID, long V_TranRefType, out long PaymentID)//PatientPayment payment,
        {
            try
            {
                AxLogger.Instance.LogInfo("Start AddTransactionForDrug.", CurrentUser);

                bool OK = PaymentProvider.Instance.AddTransactionForDrug(payment, outiID, V_TranRefType, out PaymentID);

                AxLogger.Instance.LogInfo("End of AddTransactionForDrug. Status: Success.", CurrentUser);
                return OK;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddTransactionForDrug. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_AddTransactionForDrug, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        public void PayForInPatientRegistration(long StaffID, long registrationID, PatientTransactionPayment paymentDetails,//PatientPayment paymentDetails,
                                                List<InPatientBillingInvoice> billingInvoices
                                                , out PatientTransaction Transaction, out PatientTransactionPayment PaymentInfo, out long PtCashAdvanceID)//out PatientPayment PaymentInfo
        {
            try
            {
                if (registrationID <= 0)
                {
                    throw new Exception(eHCMSResources.Z1688_G1_TTinKgHopLe);
                }
                AxLogger.Instance.LogInfo("Start processing payment for In-patient registration.", CurrentUser);
                PatientRegistration registrationInfo = null;
                try
                {
                    registrationInfo = PatientProvider.Instance.GetRegistration(registrationID
                        , (int)AllLookupValues.V_FindPatientType.NOI_TRU);
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogError(ex);
                    throw new Exception(eHCMSResources.Z0083_G1_KhongTimThayDK);
                }
                Transaction = null;
                PaymentInfo = null;
                RegAndPaymentProcessorBase paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfo);
                var paidTime = DateTime.Now;
                if (billingInvoices != null)
                {
                    foreach (var inv in billingInvoices)
                    {
                        if (inv.PaidTime == null)
                        {
                            inv.PaidTime = paidTime;
                        }
                    }
                }

                paymentProcessor.PayForInPatientRegistration(StaffID, registrationInfo, paymentDetails, billingInvoices, paidTime, out Transaction, out PaymentInfo, out PtCashAdvanceID);

                AxLogger.Instance.LogInfo("End of processing payment for In-patient registration. Status: Success", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing payment for In-patient registration. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_PayForInPatientRegistration, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void FinalizeInPatientBillingInvoices(long StaffID, long registrationID, PatientTransactionPayment paymentDetails,//PatientPayment paymentDetails,
                                                List<InPatientBillingInvoice> billingInvoices, out PatientTransaction Transaction)
        {
            try
            {
                if (registrationID <= 0)
                {
                    throw new Exception(eHCMSResources.Z1688_G1_TTinKgHopLe);
                }
                AxLogger.Instance.LogInfo("Start processing payment for In-patient registration.", CurrentUser);
                PatientRegistration registrationInfo = null;
                try
                {
                    registrationInfo = PatientProvider.Instance.GetRegistration(registrationID
                        , (int)AllLookupValues.V_FindPatientType.NOI_TRU);
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogError(ex);
                    throw new Exception(eHCMSResources.Z0083_G1_KhongTimThayDK);
                }
                Transaction = null;

                RegAndPaymentProcessorBase paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfo);
                var paidTime = DateTime.Now;
                if (billingInvoices != null)
                {
                    foreach (var inv in billingInvoices)
                    {
                        if (inv.PaidTime == null)
                        {
                            inv.PaidTime = paidTime;
                        }
                    }
                }

                //paymentProcessor.PayForInPatientRegistration(StaffID, registrationInfo, paymentDetails, billingInvoices, paidTime, out Transaction, out PaymentInfo, out PtCashAdvanceID);
                paymentProcessor.FinalizeBillingInvoice_For_InPt(StaffID, registrationInfo, paymentDetails, billingInvoices, paidTime, out Transaction);

                AxLogger.Instance.LogInfo("End of processing payment for In-patient registration. Status: Success", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing payment for In-patient registration. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_PayForInPatientRegistration, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public void CreateForm02(InPatientRptForm02 CurrentRptForm02, List<InPatientBillingInvoice> billingInvoices)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start processing create form 02.", CurrentUser);

                PatientProvider.Instance.CreateForm02(CurrentRptForm02, billingInvoices);

                AxLogger.Instance.LogInfo("End of processing create form 02. Status: Success", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing create form 02. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateForm02, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<InPatientRptForm02> GetForm02(long PtRegistrationID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start processing get form 02.", CurrentUser);

                return PatientProvider.Instance.GetForm02(PtRegistrationID);

                //AxLogger.Instance.LogInfo("Start processing get form 02. Status: Success", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing get form 02. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetForm02, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public void InPatientPayForBill(PatientRegistration registration, IList<InPatientBillingInvoice> billingInvoices, decimal payAmount, long staffID)
        {
            try
            {
                if (registration == null || registration.PtRegistrationID <= 0)
                {
                    throw new Exception(eHCMSResources.Z1688_G1_TTinKgHopLe);
                }
                AxLogger.Instance.LogInfo("Start InPatienPayForBill.", CurrentUser);

                PatientProvider.Instance.InPatientPayForBill(registration, billingInvoices, payAmount, staffID);

                AxLogger.Instance.LogInfo("End InPatienPayForBill. Status: Success", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("InPatienPayForBill. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_InPatientPayForBill, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }

        }


        public void InPatientSettlement(long ptRegistrationID, long staffID)
        {
            try
            {
                if (ptRegistrationID <= 0)
                {
                    throw new Exception(eHCMSResources.Z1688_G1_TTinKgHopLe);
                }
                AxLogger.Instance.LogInfo("Start InPatientSettlement.", CurrentUser);

                PatientProvider.Instance.InPatientSettlement(ptRegistrationID, staffID);

                AxLogger.Instance.LogInfo("End InPatientSettlement. Status: Success", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("InPatientSettlement. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_InPatientSettlement, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }

        }

        public bool ReturnInPatientDrug(long registrationID, List<RefGenMedProductSummaryInfo> returnedItems, long? DeptID, long? StaffID = null)
        {
            try
            {
                if (registrationID <= 0)
                {
                    throw new Exception(eHCMSResources.Z1688_G1_TTinKgHopLe);
                }

                AxLogger.Instance.LogInfo("Start returning payment for In-patient drug.", CurrentUser);

                PatientRegistration registrationInfo = null;
                try
                {
                    registrationInfo = PatientProvider.Instance.GetRegistration(registrationID
                        , (int)AllLookupValues.V_FindPatientType.NOI_TRU);
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogError(ex);
                    throw new Exception(eHCMSResources.Z0083_G1_KhongTimThayDK);
                }

                RegAndPaymentProcessorBase paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfo);
                paymentProcessor.ReturnInPatientDrug(registrationInfo, returnedItems, DeptID, StaffID);
                AxLogger.Instance.LogInfo("End of returning payment for In-patient drug. Status: Success", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of returning payment for In-patient drug. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_ReturnInPatientDrug, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
            return true;
        }

        public bool ChangeInPatientDept(InPatientDeptDetail entity, out long inPatientDeptDetailID)
        {
            inPatientDeptDetailID = 0;
            try
            {
                AxLogger.Instance.LogInfo("Start ChangeInPatientDept.", CurrentUser);
                if (entity.FromDate == DateTime.MinValue)
                {
                    entity.FromDate = DateTime.Now;
                }
                bool bOK = PatientProvider.Instance.ChangeInPatientDept(entity, out inPatientDeptDetailID);

                AxLogger.Instance.LogInfo("End of ChangeInPatientDept. Status: Success", CurrentUser);
                return bOK;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ChangeInPatientDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_ChangeInPatientDept, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public InPatientBillingInvoice LoadInPatientRegItemsIntoBill(long registrationID, long? DeptID, long StoreID, long StaffID, bool IsAdmin)
        {
            try
            {
                PatientRegistration registrationInfo;
                try
                {
                    registrationInfo = PatientProvider.Instance.GetRegistration(registrationID
                        , (int)AllLookupValues.V_FindPatientType.NOI_TRU);
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogError(ex);
                    throw new Exception(eHCMSResources.Z0083_G1_KhongTimThayDK);
                }
                AxLogger.Instance.LogInfo("Start loading LoadInPatientRegItemsIntoBill...", CurrentUser);

                RegAndPaymentProcessorBase paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfo);

                var inv = paymentProcessor.LoadInPatientRegItemsIntoBill(registrationInfo, DeptID, StoreID, StaffID, IsAdmin);

                AxLogger.Instance.LogInfo("End of loading LoadInPatientRegItemsIntoBill. Status: Success.", CurrentUser);
                return inv;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading LoadInPatientRegItemsIntoBill. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_LoadInPatientRegItemsIntoBill);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool CloseRegistration(long registrationID, int FindPatient, out List<string> errorMessages)
        {
            string strLog = "";
            bool retVal = false;
            errorMessages = new List<string>();
            try
            {
                strLog = string.Format("Start closing registration (RegistrationID = {0})", registrationID);
                AxLogger.Instance.LogInfo(strLog, CurrentUser);

                //TODO:
                /*
                 * Kiểm tra đăng ký có hay chưa. 
                 * Kiểm tra có transaction chưa.
                 * Nếu có transaction -> xem lịch sử thanh toán
                 * Tổng số tiền thanh toán có balance không.
                 */
                using (DbConnection connection = PatientProvider.Instance.CreateConnection())
                {
                    PatientRegistration regInfo = PatientProvider.Instance.GetRegistration(registrationID, FindPatient, connection, null);
                    if (regInfo == null)
                    {
                        errorMessages.Add("Không tìm thấy đăng ký");
                        return retVal;
                    }
                    AllLookupValues.RegistrationClosingStatus newStatus;
                    if (regInfo.PatientTransaction != null)
                    {
                        var allPayments = CommonProvider.Payments.GetAllPayments_New(regInfo.PatientTransaction.TransactionID, connection, null);
                        if (allPayments != null)
                        {
                            regInfo.PatientTransaction.PatientTransactionPayments = allPayments.ToObservableCollection();
                        }

                        //var allPayments = CommonProvider.Payments.GetAllPayments(regInfo.PatientTransaction.TransactionID, connection, null);
                        //if (allPayments != null)
                        //{
                        //    regInfo.PatientTransaction.PatientPayments = allPayments.ToObservableCollection();
                        //}
                        decimal total = PatientProvider.Instance.GetTotalPatientPayForTransaction(regInfo.PatientTransaction.TransactionID, connection, null);
                        //decimal totalPatientPaid = regInfo.PatientTransaction.PatientPayments.Sum(payment => payment.PayAmount * payment.CreditOrDebit);
                        decimal totalPatientPaid = regInfo.PatientTransaction.PatientTransactionPayments.Sum(payment => payment.PayAmount * payment.CreditOrDebit);
                        if (total > totalPatientPaid) //BN tra thieu.
                        {
                            //newStatus = AllLookupValues.RegistrationClosingStatus.NOTBALANCED_DEBIT;
                            errorMessages.Add("Bệnh nhân chưa trả hết tiền cho đăng ký này.");
                            return retVal;
                        }
                        else if (total == totalPatientPaid)
                        {
                            newStatus = AllLookupValues.RegistrationClosingStatus.BALANCED;
                        }
                        else
                        {
                            newStatus = AllLookupValues.RegistrationClosingStatus.NOTBALANCED_CREDIT;
                        }
                    }
                    else
                    {
                        newStatus = AllLookupValues.RegistrationClosingStatus.BALANCED;
                    }
                    retVal = PatientProvider.Instance.CloseRegistration(registrationID, newStatus, connection, null);
                }

                strLog = string.Format("End of closing registration (RegistrationID = {0}). Status: Success", registrationID);
                AxLogger.Instance.LogInfo(strLog, CurrentUser);
                if (!retVal)
                {
                    errorMessages = null;
                }
                return retVal;
            }
            catch (Exception ex)
            {
                strLog = string.Format("End of closing registration (RegistrationID = {0}). Status: Failed", registrationID);
                AxLogger.Instance.LogInfo(strLog, CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_LoadInPatientRegItemsIntoBill);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public bool CloseRegistrationAll(long PtRegistrationID, int FindPatient, out string Error)
        {
            bool Res = false;
            Error = "";
            try
            {
                using (DbConnection connection = PatientProvider.Instance.CreateConnection())
                {
                    PatientRegistration regInfo = GetRegistrationInfo(PtRegistrationID, FindPatient, false);

                    if (regInfo == null || regInfo.PtRegistrationID <= 0)
                    {
                        Error = "Không tìm thấy đăng ký!";
                        return false;
                    }

                    if (FindPatient == (int)AllLookupValues.V_FindPatientType.NOI_TRU)
                    {
                        if (regInfo.AdmissionInfo.DischargeDate != null)
                        {
                            Res = PatientRegistration_Close(PtRegistrationID, FindPatient, (long)AllLookupValues.RegistrationClosingStatus.BALANCED);
                        }
                        else
                        {
                            Error = "Bệnh Nhân này chưa hoàn tất thủ tục Xuất Viện! Không thể đóng Đăng Ký!";
                        }
                    }
                    if (FindPatient == (int)AllLookupValues.V_FindPatientType.NGOAI_TRU)
                    {
                        AllLookupValues.RegistrationClosingStatus newStatus = new AllLookupValues.RegistrationClosingStatus();

                        int flag = 0;

                        if (regInfo.PayableSum.TotalHIPayment + regInfo.PayableSum.TotalPatientPaid == regInfo.PayableSum.TotalPrice)
                        {
                            newStatus = AllLookupValues.RegistrationClosingStatus.BALANCED;
                            flag = 0;
                        }
                        if (regInfo.PayableSum.TotalHIPayment + regInfo.PayableSum.TotalPatientPaid < regInfo.PayableSum.TotalPrice)
                        {
                            newStatus = AllLookupValues.RegistrationClosingStatus.NOTBALANCED_DEBIT;
                            flag = 1;
                        }
                        if (regInfo.PayableSum.TotalHIPayment + regInfo.PayableSum.TotalPatientPaid > regInfo.PayableSum.TotalPrice)
                        {
                            newStatus = AllLookupValues.RegistrationClosingStatus.NOTBALANCED_CREDIT;
                            flag = 2;
                        }

                        if (flag == 0)
                        {
                            Res = PatientRegistration_Close(PtRegistrationID, FindPatient, (long)newStatus);
                        }
                        else
                        {
                            if (flag == 1)
                            {
                                Error = "Bệnh Nhân chưa thanh toán hết nợ! Không thể đóng Đăng Ký!";
                            }
                            if (flag == 2)
                            {
                                Error = "Bệnh Viện còn nợ Bệnh Nhân tiền! Cần hoàn tiền lại cho Bệnh Nhân!";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string strLog = string.Format("End of closing registration (PtRegistrationID = {0}). Status: Failed", PtRegistrationID);
                AxLogger.Instance.LogInfo(strLog, CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_PatientRegistration_Close);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

            return Res;
        }


        public bool AddInPatientDischarge(InPatientAdmDisDetails admissionInfo, long StaffID, out List<string> errorMessages)
        {
            string strLog = "";
            bool retVal = false;
            errorMessages = new List<string>();
            try
            {
                strLog = string.Format("Start adding Patient discharge (AdmissionID = {0})", admissionInfo.InPatientAdmDisDetailID);
                AxLogger.Instance.LogInfo(strLog, CurrentUser);

                //TODO:
                /*
                 * Kiểm tra đăng ký có hay chưa. 
                 * Kiểm tra có transaction chưa.
                 * Nếu có transaction -> xem lịch sử thanh toán
                 * Tổng số tiền thanh toán có balance không.
                 */
                using (DbConnection connection = PatientProvider.Instance.CreateConnection())
                {
                    PatientRegistration regInfo = PatientProvider.Instance.GetRegistration(admissionInfo.PtRegistrationID
                        , (int)AllLookupValues.V_FindPatientType.NOI_TRU, connection, null);
                    if (regInfo == null)
                    {
                        errorMessages.Add("Không tìm thấy đăng ký");
                        return retVal;
                    }
                    if (regInfo.PatientTransaction != null)
                    {
                        var allPayments = CommonProvider.Payments.GetAllPayments_InPt(regInfo.PatientTransaction.TransactionID, connection, null);
                        if (allPayments != null)
                        {
                            regInfo.PatientTransaction.PatientTransactionPayments = allPayments.ToObservableCollection();
                        }

                        decimal total = PatientProvider.Instance.GetTotalPatientPayForTransaction_InPt(regInfo.PatientTransaction.TransactionID, connection, null);
                        decimal totalPatientPaid = regInfo.PatientTransaction.PatientTransactionPayments.Sum(payment => payment.PayAmount * payment.CreditOrDebit);
                        if (total > totalPatientPaid) //BN tra thieu.
                        {
                            //newStatus = AllLookupValues.RegistrationClosingStatus.NOTBALANCED_DEBIT;
                            errorMessages.Add("Bệnh nhân chưa trả hết tiền cho đăng ký này.");
                            return retVal;
                        }
                    }

                    //Them thong tin chuyen vien.
                    //Sau do dong dang ky luon.
                    //Dong khong duoc thi thoi. Khong can thiet.
                    bool bOk = false;
                    if (admissionInfo.DeceasedInfo != null)
                    {
                        if (admissionInfo.DeceasedInfo.DSNumber <= 0)
                        {
                            admissionInfo.DeceasedInfo.PtRegistrationID = admissionInfo.PtRegistrationID;
                            long deceasedInfoId;
                            bOk = PatientProvider.Instance.AddDeceaseInfo(admissionInfo.DeceasedInfo, connection, null, out deceasedInfoId);

                            if (bOk)
                            {
                                admissionInfo.DeceasedInfo.DSNumber = deceasedInfoId;
                                bOk = PatientProvider.Instance.UpdateInPatientDischargeInfo(admissionInfo, StaffID, connection, null);
                            }
                            if (!bOk)
                            {
                                errorMessages.Add("Không thể thêm dữ liệu xuất viện.");
                            }
                        }
                        else
                        {
                            if (admissionInfo.DeceasedInfo.EntityState == EntityState.MODIFIED)
                            {
                                PatientProvider.Instance.UpdateDeceaseInfo(admissionInfo.DeceasedInfo, connection, null);
                            }
                            else if (admissionInfo.DeceasedInfo.EntityState == EntityState.DELETED_MODIFIED)
                            {
                                PatientProvider.Instance.DeleteDeceaseInfo(admissionInfo.DeceasedInfo.DSNumber, connection, null);
                                admissionInfo.DeceasedInfo = null;
                                admissionInfo.DSNumber = null;
                            }
                            bOk = PatientProvider.Instance.UpdateInPatientDischargeInfo(admissionInfo, StaffID, connection, null);
                        }
                    }
                    else
                    {
                        bOk = PatientProvider.Instance.UpdateInPatientDischargeInfo(admissionInfo, StaffID, connection, null);
                    }
                    retVal = bOk;

                }

                strLog = string.Format("End of adding Patient discharge (AdmissionID = {0})", admissionInfo.InPatientAdmDisDetailID);
                AxLogger.Instance.LogInfo(strLog, CurrentUser);
                if (!retVal)
                {
                    errorMessages = null;
                }
                return retVal;
            }
            catch (Exception ex)
            {
                strLog = string.Format("End of adding Patient discharge (AdmissionID = {0})", admissionInfo.InPatientAdmDisDetailID);
                AxLogger.Instance.LogInfo(strLog, CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_AddInPatientDischarge);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }


        public bool RevertDischarge(InPatientAdmDisDetails admissionInfo, long staffID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start Revert Discharge.", CurrentUser);

                return PatientProvider.Instance.RevertDischarge(admissionInfo, staffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End Revert Discharge. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_RevertDischarge, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }



        public bool CheckBeforeDischarge(long registrationID, long DischargeDeptID, bool ConfirmNotTreatedAsInPt, out string errorMessages, out string confirmMessages)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start Check Before Discharge.", CurrentUser);

                return PatientProvider.Instance.CheckBeforeDischarge(registrationID, DischargeDeptID, ConfirmNotTreatedAsInPt, out errorMessages, out confirmMessages);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End Check Before Discharge. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CheckBeforeDischarge, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }

        }


        /// <summary>
        /// Chi lay thong tin dang ky noi tru va payment thoi(khong lay cac chi tiet dang ky nhu CLS, KCB)
        /// </summary>
        /// <param name="registrationID"></param>
        /// <param name="TotalLiabilities"></param>
        /// <param name="SumOfAdvance"></param>
        /// <param name="TotalPatientPayment_PaidInvoice"></param>
        /// <returns></returns>
        /// 


        public bool GetInPatientRegistrationAndPaymentInfo(long registrationID, bool GetSumOfCashAdvBalanceOnly, out decimal TotalLiabilities, out decimal SumOfAdvance, out decimal TotalPatientPayment_PaidInvoice, out decimal TotalRefundPatient, out decimal TotalCashAdvBalanceAmount
                                                            , out decimal TotalCharityOrgPayment, out decimal TotalPatientPayment_NotFinalized, out decimal TotalPatientPaid_NotFinalized, out decimal TotalCharityOrgPayment_NotFinalized)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading Payment Info.", CurrentUser);

                return PatientProvider.Instance.GetInPatientRegistrationNonFinalizedLiabilities(registrationID, GetSumOfCashAdvBalanceOnly, out TotalLiabilities, out SumOfAdvance, out TotalPatientPayment_PaidInvoice, out TotalRefundPatient, out TotalCashAdvBalanceAmount
                                                                                                , out TotalCharityOrgPayment, out TotalPatientPayment_NotFinalized, out TotalPatientPaid_NotFinalized, out TotalCharityOrgPayment_NotFinalized);

            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading Payment Info. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }


        }



        //public PatientRegistration GetInPatientRegistrationAndPaymentInfo(long registrationID, out decimal TotalLiabilities, out decimal SumOfAdvance, out decimal TotalPatientPayment_PaidInvoice, out decimal TotalRefundPatient)
        //{
        //    try
        //    {
        //        AxLogger.Instance.LogInfo("Start loading Registration Info.", CurrentUser);

        //        var registrationInfo = GetRegistrationNoiTru(registrationID);
        //        if (registrationInfo == null)
        //        {
        //            throw new Exception(eHCMSResources.Z0083_G1_KhongTimThayDK);
        //        }
        //        if (registrationInfo.V_RegistrationType != AllLookupValues.RegistrationType.NOI_TRU)
        //        {
        //            throw new Exception("Không phải đăng ký nội trú");
        //        }
        //        if (registrationInfo.PatientID.HasValue && registrationInfo.PatientID.Value > 0)
        //        {
        //            registrationInfo.Patient = PatientProvider.Instance.GetPatientByID_Simple(registrationInfo.PatientID.Value);
        //        }

        //        //Neu la dang ky noi tru thi chi lay danh sach billing invoice thoi.
        //        if (//registrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU_NOI_TRU || 
        //            registrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
        //        {
        //            List<InPatientBillingInvoice> billingInvoices = PatientProvider.Instance.GetAllInPatientBillingInvoices(registrationInfo.PtRegistrationID, null);
        //            if (billingInvoices != null)
        //            {
        //                registrationInfo.InPatientBillingInvoices = billingInvoices.ToObservableCollection();
        //            }

        //        }

        //        if (registrationInfo.PatientTransaction != null)
        //        {
        //            var allPayments = CommonProvider.Payments.GetAllPayments_InPt(registrationInfo.PatientTransaction.TransactionID);
        //            if (allPayments != null)
        //            {
        //                registrationInfo.PatientTransaction.PatientTransactionPayments = allPayments.ToObservableCollection();
        //            }
        //        }
        //        var allAdvances = CommonProvider.Payments.PatientCashAdvance_GetAll(registrationInfo.PtRegistrationID, (long)registrationInfo.V_RegistrationType);
        //        if (allAdvances != null)
        //        {
        //            registrationInfo.PatientCashAdvances = allAdvances.ToObservableCollection();
        //        }
        //        var allRptAdvances = CommonProvider.Payments.RptPatientCashAdvReminder_GetAll(registrationInfo.PtRegistrationID);
        //        if (allRptAdvances != null)
        //        {
        //            registrationInfo.RptPatientCashAdvReminders = allRptAdvances.ToObservableCollection();
        //        }
        //        var retVal = PatientProvider.Instance.GetInPatientRegistrationNonFinalizedLiabilities(registrationID, out TotalLiabilities, out SumOfAdvance, out TotalPatientPayment_PaidInvoice, out TotalRefundPatient);
        //        return registrationInfo;
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of loading Registration Info. Status: Failed.", CurrentUser);

        //        var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }


        //}

        public void RecalcInPatientBillingInvoice(long? StaffID, InPatientBillingInvoice billingInv)
        {
            try
            {
                if (billingInv == null || billingInv.PtRegistrationID <= 0)
                {
                    throw new Exception(eHCMSResources.Z1688_G1_TTinKgHopLe);
                }

                AxLogger.Instance.LogInfo("Start RecalcInPatientBillingInvoice.", CurrentUser);
                List<PatientRegistrationDetail> regDetails;
                List<PatientPCLRequest> PCLRequestList;
                List<OutwardDrugClinicDeptInvoice> allInPatientInvoices;

                GetInPatientBillingInvoiceDetails(billingInv.InPatientBillingInvID, out regDetails, out PCLRequestList, out allInPatientInvoices);

                billingInv.RegistrationDetails = regDetails != null ? regDetails.ToObservableCollection() : null;
                billingInv.PclRequests = PCLRequestList != null ? PCLRequestList.ToObservableCollection() : null;
                billingInv.OutwardDrugClinicDeptInvoices = allInPatientInvoices != null ? allInPatientInvoices.ToObservableCollection() : null;

                PatientRegistration registrationInfo;
                try
                {
                    registrationInfo = GetRegistrationNoiTru(billingInv.PtRegistrationID);
                }
                catch
                {
                    throw new Exception(eHCMSResources.Z0083_G1_KhongTimThayDK);
                }

                string ListIDOutTranDetails = "";
                var paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfo);
                paymentProcessor.RecalcInPatientBillingInvoice(out ListIDOutTranDetails, StaffID, registrationInfo, billingInv);

                AxLogger.Instance.LogInfo("End of RecalcInPatientBillingInvoice.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RecalcInPatientBillingInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RecalcInPatientBillingInvoice, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<InPatientBillingInvoice> GetAllInPatientBillingInvoices(long PtRegistrationID, long? DeptID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GetAllInPatientBillingInvoices.", CurrentUser);
                return PatientProvider.Instance.GetAllInPatientBillingInvoices(PtRegistrationID, DeptID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GetAllInPatientBillingInvoices. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        //==== #001
        public List<InPatientBillingInvoice> GetAllInPatientBillingInvoicesByDeptArray(long PtRegistrationID, long? DeptID, List<RefDepartment> DeptArray)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GetAllInPatientBillingInvoicesByDeptArray.", CurrentUser);
                return PatientProvider.Instance.GetAllInPatientBillingInvoicesByDeptArray(PtRegistrationID, DeptID, DeptArray);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GetAllInPatientBillingInvoicesByDeptArray. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //==== #001

        public List<InPatientBillingInvoice> GetAllInPatientBillingInvoices_FromListDeptID(long PtRegistrationID, List<long> ListDeptIDs)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GetAllInPatientBillingInvoices.", CurrentUser);
                return PatientProvider.Instance.GetAllInPatientBillingInvoices_FromListDeptID(PtRegistrationID, ListDeptIDs);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GetAllInPatientBillingInvoices. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<InPatientBillingInvoice> GetAllInPatientBillingInvoices_ForCreateForm02(long RptForm02_InPtID, long PtRegistrationID, List<long> ListDeptIDs)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GetAllInPatientBillingInvoices_ForCreateForm02.", CurrentUser);
                return PatientProvider.Instance.GetAllInPatientBillingInvoices_ForCreateForm02(RptForm02_InPtID, PtRegistrationID, ListDeptIDs);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GetAllInPatientBillingInvoices_ForCreateForm02. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllInPatientBillingInvoices_ForCreateForm02, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PatientPaymentAccount> GetAllPatientPaymentAccounts()
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GetAllPatientPaymentAccounts.", CurrentUser);
                return CommonProvider.Lookups.GetAllPatientPaymentAccounts();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GetAllPatientPaymentAccounts. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public PatientRegistration GetRegistrationNoiTru(long registrationID)
        {
            return PatientProvider.Instance.GetRegistration(registrationID, (int)AllLookupValues.V_FindPatientType.NOI_TRU);
        }

        public PatientRegistration GetRegistrationNgoaiTru(long registrationID)
        {
            return PatientProvider.Instance.GetRegistration(registrationID, (int)AllLookupValues.V_FindPatientType.NGOAI_TRU);
        }

        public List<RefDepartmentReqCashAdv> RefDepartmentReqCashAdv_DeptID(long DeptID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading RefDepartmentReqCashAdv_DeptID.", CurrentUser);
                return PaymentProvider.Instance.RefDepartmentReqCashAdv_DeptID(DeptID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading RefDepartmentReqCashAdv_DeptID. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool PatientCashAdvance_Insert(PatientCashAdvance payment, out long PtCashAdvanceID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading PatientCashAdvance_Insert.", CurrentUser);
                return PaymentProvider.Instance.PatientCashAdvance_Insert(payment, out PtCashAdvanceID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading PatientCashAdvance_Insert. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool PatientCashAdvance_Delete(PatientCashAdvance payment, long staffID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading PatientCashAdvance_Delete.", CurrentUser);
                return PaymentProvider.Instance.PatientCashAdvance_Delete(payment, staffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading PatientCashAdvance_Delete. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_DeletePatientCashAdvance, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }


        public List<PatientCashAdvance> PatientCashAdvance_GetAll(long PtRegistrationID, long V_RegistrationType)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading PatientCashAdvance_GetAll.", CurrentUser);
                return PaymentProvider.Instance.PatientCashAdvance_GetAll(PtRegistrationID, V_RegistrationType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading PatientCashAdvance_GetAll. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //HPT
        //GenericPayment_FullOperation
        public bool GenericPayment_FullOperation(GenericPayment GenPayment, out GenericPayment OutGenericPayment)
        {
            try
            {
                OutGenericPayment = new GenericPayment();
                long OutGenericPaymentID = 0;
                AxLogger.Instance.LogInfo("Start loading GenericPayment_FullOperation.", CurrentUser);
                PaymentProvider.Instance.GenericPayment_FullOperation(GenPayment, out OutGenericPaymentID);
                bool BOK;
                BOK = (OutGenericPaymentID > 0);
                if (BOK)
                {
                    OutGenericPayment = PaymentProvider.Instance.GetGenericPaymentByID(OutGenericPaymentID);
                }
                return BOK;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GenericPayment_FullOperation. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }



        public List<GenericPayment> GenericPayment_GetAll(DateTime? FromDate, DateTime? ToDate, long? V_GenericPaymentType, long? StaffID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GenericPayment_GetAll.", CurrentUser);
                return PaymentProvider.Instance.GenericPayment_GetAll(FromDate, ToDate, V_GenericPaymentType, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GenericPayment_GetAll. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public GenericPayment GenericPayment_SearchByCode(string GenPaymtCode, long StaffID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GenericPayment_GetAll.", CurrentUser);
                return PaymentProvider.Instance.GenericPayment_SearchByCode(GenPaymtCode, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GenericPayment by Code. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public List<PatientCashAdvance> GetCashAdvanceBill(long PtRegistrationID, long V_RegistrationType)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GetCashAdvanceBill.", CurrentUser);
                return PaymentProvider.Instance.GetCashAdvanceBill(PtRegistrationID, V_RegistrationType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GetCashAdvanceBill. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetCashAdvanceBill, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<TransactionFinalization> GetPatientSettlement(long PtRegistrationID, long V_RegistrationType)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GetPatientSettlement.", CurrentUser);
                return PaymentProvider.Instance.GetPatientSettlement(PtRegistrationID, V_RegistrationType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GetPatientSettlement. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetPatientSettlement, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }



        public PatientTransaction GetAllPaymentByRegistrationID_InPt(long registrationID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GetAllPaymentByRegistrationID_InPt.", CurrentUser);
                return PaymentProvider.Instance.GetAllPaymentByRegistrationID_InPt(registrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GetAllPaymentByRegistrationID_InPt. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool ThanhToanTienChoBenhNhan(PatientTransactionPayment payment, PatientTransactionDetail TrDetail, long PtRegistrationID, long V_RegistrationType, out long PtTranPaymtID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading ThanhToanTienChoBenhNhan.", CurrentUser);
                return PaymentProvider.Instance.ThanhToanTienChoBenhNhan(payment, TrDetail, PtRegistrationID, V_RegistrationType, out PtTranPaymtID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading ThanhToanTienChoBenhNhan. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool RptPatientCashAdvReminder_Insert(RptPatientCashAdvReminder payment, out long RptPtCashAdvRemID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading RptPatientCashAdvReminder_Insert.", CurrentUser);
                return PaymentProvider.Instance.RptPatientCashAdvReminder_Insert(payment, out RptPtCashAdvRemID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading RptPatientCashAdvReminder_Insert. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool RptPatientCashAdvReminder_Update(RptPatientCashAdvReminder payment)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading RptPatientCashAdvReminder_Update.", CurrentUser);
                return PaymentProvider.Instance.RptPatientCashAdvReminder_Update(payment);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading RptPatientCashAdvReminder_Update. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool RptPatientCashAdvReminder_Delete(long RptPtCashAdvRemID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading RptPatientCashAdvReminder_Delete.", CurrentUser);
                return PaymentProvider.Instance.RptPatientCashAdvReminder_Delete(RptPtCashAdvRemID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading RptPatientCashAdvReminder_Delete. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<RptPatientCashAdvReminder> RptPatientCashAdvReminder_GetAll(long PtRegistrationID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading RptPatientCashAdvReminder_GetAll.", CurrentUser);
                return PaymentProvider.Instance.RptPatientCashAdvReminder_GetAll(PtRegistrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading RptPatientCashAdvReminder_GetAll. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<ReportOutPatientCashReceipt_Payments> GetReportOutPatientCashReceipt(SearchOutwardReport Searchcriate)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading RptPatientCashAdvReminder_GetAll.", CurrentUser);
                return PaymentProvider.Instance.GetReportOutPatientCashReceipt(Searchcriate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading RptPatientCashAdvReminder_GetAll. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool PatientTransactionPayment_UpdateNote(List<PatientTransactionPayment> allPayment)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading PatientTransactionPayment_UpdateNote.", CurrentUser);
                return PaymentProvider.Instance.PatientTransactionPayment_UpdateNote(allPayment);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading PatientTransactionPayment_UpdateNote. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool PatientTransactionPayment_UpdateID(PatientTransactionPayment Payment)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading PatientTransactionPayment_UpdateNote.", CurrentUser);
                return PaymentProvider.Instance.PatientTransactionPayment_UpdateID(Payment);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading PatientTransactionPayment_UpdateNote. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PatientTransactionPayment> PatientTransactionPayment_Load(long TransactionID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading PatientTransactionPayment_Load.", CurrentUser);
                return PaymentProvider.Instance.PatientTransactionPayment_Load(TransactionID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading PatientTransactionPayment_Load. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<ReportOutPatientCashReceipt_Payments> GetReportOutPatientCashReceipt_TongHop(SearchOutwardReport Searchcriate, bool? IsTongHop, long loggedStaffID, out List<PatientTransactionPayment> OutPaymentLst)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GetReportOutPatientCashReceipt_TongHop.", CurrentUser);
                return PaymentProvider.Instance.GetReportOutPatientCashReceipt_TongHop(Searchcriate, IsTongHop, loggedStaffID, out OutPaymentLst);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GetReportOutPatientCashReceipt_TongHop. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        /*▼====: #004*/
        public List<ReportOutPatientCashReceipt_Payments> GetReportOutPatientCashReceipt_TongHop_Async(SearchOutwardReport Searchcriate, bool? IsTongHop, long loggedStaffID, out List<PatientTransactionPayment> OutPaymentLst, out int AsyncKey)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GetReportOutPatientCashReceipt_TongHop.", CurrentUser);
                return PaymentProvider.Instance.GetReportOutPatientCashReceipt_TongHop_Async(Searchcriate, IsTongHop, loggedStaffID, out OutPaymentLst, out AsyncKey);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GetReportOutPatientCashReceipt_TongHop. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public List<ReportOutPatientCashReceipt_Payments> GetMoreReportOutPatientCashReceipt_TongHop_Async(int RefAsyncKey, out List<PatientTransactionPayment> OutPaymentLst, out int AsyncKey)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GetReportOutPatientCashReceipt_TongHop.", CurrentUser);
                return PaymentProvider.Instance.GetMoreReportOutPatientCashReceipt_TongHop_Async(RefAsyncKey, out OutPaymentLst, out AsyncKey);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GetReportOutPatientCashReceipt_TongHop. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        /*▲====: #004*/

        //export excel all
        #region Export excel from database

        public List<List<string>> ExportToExcellAll_ListRefGenericDrug(DrugSearchCriteria criteria)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading ExportToExcellAll_ListRefGenericDrug.", CurrentUser);
                return AppConfigsProvider.Instance.ExportToExcellAll_ListRefGenericDrug(criteria);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading ExportToExcellAll_ListRefGenericDrug. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<List<string>> ExportToExcellAll_ListRefGenMedProductDetail(RefGenMedProductDetailsSearchCriteria criteria)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading ExportToExcellAll_ListRefGenericDrug.", CurrentUser);
                return AppConfigsProvider.Instance.ExportToExcellAll_ListRefGenMedProductDetail(criteria);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading ExportToExcellAll_ListRefGenMedProductDetail. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region Quỹ hỗ trợ bệnh nhân sử dụng kỹ thuật cao
        public List<CharityOrganization> GetAllCharityOrganization()
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GetAllCharityOrganization", CurrentUser);
                return PaymentProvider.Instance.GetAllCharityOrganization();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GetAllCharityOrganization. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        /*▼====: #003*/
        public List<CharitySupportFund> GetCharitySupportFundForInPt(long PtRegistrationID, long? BillingInvID)
        {
            return GetCharitySupportFundForInPt_V2(PtRegistrationID, BillingInvID, null);
        }
        public List<CharitySupportFund> GetCharitySupportFundForInPt_V2(long PtRegistrationID, long? BillingInvID, bool? IsHighTechServiceBill)
        /*▲====: #003*/
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GetCharitySupportFundForInPt", CurrentUser);
                return PaymentProvider.Instance.GetCharitySupportFundForInPt(PtRegistrationID, BillingInvID, IsHighTechServiceBill);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GetCharitySupportFundForInPt. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        /*▼====: #003*/
        public List<CharitySupportFund> SaveCharitySupportFundForInPt(long PtRegistrationID, long StaffID, long? BillingInvID, List<CharitySupportFund> SupportFunds)
        {
            return SaveCharitySupportFundForInPt_V2(PtRegistrationID, StaffID, BillingInvID, SupportFunds, false);
        }
        public List<CharitySupportFund> SaveCharitySupportFundForInPt_V2(long PtRegistrationID, long StaffID, long? BillingInvID, List<CharitySupportFund> SupportFunds, bool IsHighTechServiceBill)
        /*▲====: #003*/
        {
            try
            {
                AxLogger.Instance.LogInfo("Start saving CharitySupportFundForInPt", CurrentUser);
                return PaymentProvider.Instance.SaveCharitySupportFundForInPt(PtRegistrationID, StaffID, BillingInvID, SupportFunds, IsHighTechServiceBill);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of saving CharitySupportFundForInPt. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        #endregion


        #region InPatientInstruction

        public void AddInPatientInstruction(PatientRegistration ptRegistration, out long IntPtDiagDrInstructionID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start saving inpatient Registration.", CurrentUser);
                var bOK = PatientProvider.Instance.AddInPatientInstruction(ptRegistration, out IntPtDiagDrInstructionID);
                if (ptRegistration.InPatientInstruction.PclRequests != null)
                {
                    foreach (PatientPCLRequest row in ptRegistration.InPatientInstruction.PclRequests)
                    {
                        if (row.PatientPCLRequestIndicators != null)
                        {
                            foreach (PatientPCLRequestDetail itemrow in row.PatientPCLRequestIndicators)
                            {
                                itemrow.PatientPCLRequest = row;
                            }
                        }
                    }
                    List<PatientPCLRequestDetail> newLstRequestDetails = ptRegistration.InPatientInstruction.PclRequests.SelectMany(request => request.PatientPCLRequestIndicators).ToList();
                    var newLstRequests = SplitVote(newLstRequestDetails);

                    ptRegistration.InPatientInstruction.PclRequests = newLstRequests.ToObservableCollection();

                }
                AxLogger.Instance.LogInfo("End of saving inpatient Registration.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of saving inpatient Registration. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateBillingInvoiceFromExistingItems, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public InPatientInstruction GetInPatientInstruction(PatientRegistration ptRegistration)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetInPatientInstruction.", CurrentUser);
                return PatientProvider.Instance.GetInPatientInstruction(ptRegistration);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInPatientInstruction. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateBillingInvoiceFromExistingItems, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public List<Intravenous> GetIntravenousPlan_InPt(long IntPtDiagDrInstructionID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetIntravenousPlan.", CurrentUser);
                return PatientProvider.Instance.GetIntravenousPlan_InPt(IntPtDiagDrInstructionID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetIntravenousPlan. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateBillingInvoiceFromExistingItems, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public void GetAllItemsByInstructionID(long IntPtDiagDrInstructionID, out List<PatientRegistrationDetail> AllRegistrationItems, out List<PatientPCLRequestDetail> AllPCLRequestItems)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetAllRegistrationItemsByInstructionID.", CurrentUser);
                PatientProvider.Instance.GetAllItemsByInstructionID(IntPtDiagDrInstructionID, out AllRegistrationItems, out AllPCLRequestItems);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllRegistrationItemsByInstructionID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateBillingInvoiceFromExistingItems, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //public void AddInPatientBillingInvoice(InPatientBillingInvoice billingInv)
        //{
        //    if (billingInv.PclRequests != null)
        //    {
        //        foreach (PatientPCLRequest row in billingInv.PclRequests)
        //        {
        //            if (row.PatientPCLRequestIndicators != null)
        //            {
        //                foreach (PatientPCLRequestDetail itemrow in row.PatientPCLRequestIndicators)
        //                {
        //                    itemrow.PatientPCLRequest = row;
        //                }
        //            }
        //        }
        //        List<PatientPCLRequestDetail> newLstRequestDetails = billingInv.PclRequests.SelectMany(request => request.PatientPCLRequestIndicators).ToList();
        //        var newLstRequests = SplitVote(newLstRequestDetails);

        //        billingInv.PclRequests = newLstRequests.ToObservableCollection();

        //    }

        //    var bOK = PatientProvider.Instance.AddUpdateBillingInvoices(billingInv);
        //    return bOK;

        //}


        private bool SplitVoteCondition(PatientPCLRequestDetail reqDetails, PatientPCLRequest item)
        {
            bool exists = false;
            if (item.PatientPCLRequestIndicators != null)
            {
                foreach (var row in item.PatientPCLRequestIndicators)
                {
                    //neu = nhau thi tach phieu
                    if (row.PCLExamType.PCLExamTypeID == reqDetails.PCLExamType.PCLExamTypeID)
                    {
                        exists = true;
                        break;
                    }
                }
                if (exists)
                {
                    exists = false;
                }
                else
                {
                    //o day chua kiem tra de tach theo phong
                    if (item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.V_PCLMainCategory == reqDetails.PCLExamType.V_PCLMainCategory
                        && item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.HITTypeID == reqDetails.PCLExamType.HITTypeID
                        && item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.IsExternalExam == reqDetails.PCLExamType.IsExternalExam)
                    {
                        if (item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.IsExternalExam == true)
                        {
                            if (item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.HosIDofExternalExam == reqDetails.PCLExamType.HosIDofExternalExam)
                            {
                                item.PatientPCLRequestIndicators.Add(reqDetails);
                                exists = true;
                            }
                        }
                        else
                        {
                            item.PatientPCLRequestIndicators.Add(reqDetails);
                            exists = true;
                        }
                    }
                }
            }
            return exists;
        }



        private List<PatientPCLRequest> SplitVote(List<PatientPCLRequestDetail> lstDetail_CreateNew)
        {
            var requests = new List<PatientPCLRequest>(); //new Dictionary<long ,PatientPCLRequest>();
            Dictionary<long, PCLExamType> MAPPCLExamTypeDeptLoc = DataProviderBase.MAPPCLExamTypeDeptLoc;

            foreach (var reqDetails in lstDetail_CreateNew)
            {
                reqDetails.ObjDeptLocIDList = MAPPCLExamTypeDeptLoc[reqDetails.PCLExamType.PCLExamTypeID].ObjDeptLocationList;

                if (reqDetails.DeptLocation == null)
                {
                    reqDetails.DeptLocation = new DeptLocation();
                }

                bool exists = false;
                foreach (PatientPCLRequest item in requests)
                {
                    exists = false;
                    if (item.PCLDeptLocID == reqDetails.DeptLocation.DeptLocationID && item.PCLDeptLocID.GetValueOrDefault(0) > 0)
                    {
                        //neu duoc chon phong ngay tu dau
                        exists = SplitVoteCondition(reqDetails, item);
                    }
                    else
                    {
                        PatientPCLRequestDetail OldItem = item.PatientPCLRequestIndicators.FirstOrDefault();
                        if (OldItem.ObjDeptLocIDList != null && OldItem.ObjDeptLocIDList.Count > 0 && reqDetails.ObjDeptLocIDList != null && reqDetails.ObjDeptLocIDList.Count > 0)
                        {
                            foreach (var item1 in OldItem.ObjDeptLocIDList)
                            {
                                foreach (var detail1 in reqDetails.ObjDeptLocIDList)
                                {
                                    if (detail1.DeptLocationID == item1.DeptLocationID)
                                    {
                                        exists = true;
                                        break;
                                    }
                                }
                                if (exists)
                                {
                                    //da ton tai,thi dua vao nhung dk khac de nhu V_Maincatelogy and HITypeID de tach phieu
                                    exists = SplitVoteCondition(reqDetails, item);
                                    break;
                                }
                            }
                        }
                    }

                    if (exists)
                    {
                        break;
                    }
                }
                if (!exists)
                {
                    var newRequest = new PatientPCLRequest();
                    //newRequest.PCLRequestNumID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.PCLRequestNumID : "";
                    newRequest.PatientPCLReqID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.PatientPCLReqID : 0;
                    newRequest.StaffID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.StaffID : 0;
                    newRequest.DoctorStaffID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.DoctorStaffID : 0;
                    newRequest.Diagnosis = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.Diagnosis : "";

                    newRequest.DoctorComments = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.DoctorComments : "";
                    newRequest.DeptID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.DeptID : 0;
                    newRequest.PCLDeptLocID = reqDetails.DeptLocation != null ? reqDetails.DeptLocation.DeptLocationID : 0;
                    newRequest.ReqFromDeptLocID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.ReqFromDeptLocID : 0;
                    newRequest.RequestedDoctorName = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.RequestedDoctorName : "";
                    newRequest.ServiceRecID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.ServiceRecID : 0;

                    newRequest.V_PCLRequestStatus = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.V_PCLRequestStatus : 0;
                    newRequest.V_PCLRequestType = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.V_PCLRequestType : 0;
                    newRequest.V_PCLRequestStatusName = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.V_PCLRequestStatusName : "";
                    newRequest.ExamRegStatus = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.ExamRegStatus : 0;
                    newRequest.RecordState = RecordState.DETACHED;
                    newRequest.EntityState = EntityState.NEW;
                    newRequest.CreatedDate = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.CreatedDate : DateTime.Now;
                    newRequest.ExamDate = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.ExamDate : DateTime.Now;

                    newRequest.PatientPCLRequestIndicators = new ObservableCollection<PatientPCLRequestDetail>();
                    newRequest.PatientPCLRequestIndicators.Add(reqDetails);
                    newRequest.V_PCLMainCategory = reqDetails.PCLExamType.V_PCLMainCategory;
                    newRequest.IsExternalExam = reqDetails.PCLExamType.IsExternalExam;
                    newRequest.AgencyID = reqDetails.PCLExamType.HosIDofExternalExam;
                    newRequest.PCLRequestNumID = new ServiceSequenceNumberProvider().GetCode_PCLRequest_InPt(newRequest.V_PCLMainCategory);

                    requests.Add(newRequest);
                }
            }
            return requests;
        }



        #endregion
    }


}