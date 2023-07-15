using System;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using eHCMS.Configurations;

using eHCMS.Services.Core;
using AxLogging;
using System.Linq;
using System.Collections.Generic;
/*
* 20181023 #001 CMN: Added firsttime load for import libraries to auto print in first time
* 20181211 #002 TTM: BM 0004207: Lấy danh sách định dạng thẻ.
*/
namespace aEMRServices
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        ServiceHostManager _hostManager;
        ServiceHostManager _hostManagerTemp;
        BackgroundWorker workerThread = new BackgroundWorker();
        BackgroundWorker workerThreadTemp = new BackgroundWorker();
        protected override void OnStart(string[] args)
        {
            Environment.CurrentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;

            _hostManager = new ServiceHostManager();
            workerThread.DoWork += workerThread_DoWork;
            workerThread.RunWorkerAsync();
            
            _hostManagerTemp = new ServiceHostManager();
            //workerThreadTemp.DoWork += new DoWorkEventHandler(workerThread_DoWork_Temp);
            workerThreadTemp.RunWorkerAsync();
        }

        void workerThread_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                AxLogger.Instance.LogInfo("Service OnStart: workerThread_DoWork BEGIN");
                var fs = new FileStream(Application.StartupPath + "/Trace.txt", FileMode.OpenOrCreate);
                var myListener = new TextWriterTraceListener(fs);
                Trace.Listeners.Add(myListener);
                Trace.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff --- ") + "Starting ...");
                Trace.Flush();

                //Detect correct GMT
                double mDefaultGMT = +7;
                if ((DateTime.Now - DateTime.UtcNow.ToUniversalTime()).TotalHours != mDefaultGMT)
                {
                    throw new Exception("GMT is incorrect!");
                }

                int num = 0;
                bool bConnectDbOK = false;
                while (num < 8)
                {
                    try
                    {
                        AxLogger.Instance.LogInfo("Service OnStart: Test Database Connection Cnt = " + (++num).ToString());
                        if (aEMR.DataAccessLayer.Providers.PatientProvider.Instance.TestDatabaseConnectionOK())
                        {
                            bConnectDbOK = true;
                            AxLogger.Instance.LogInfo("Service OnStart: Database Connection is OK.");
                            if (num > 1)
                            {
                                // If DB connection is OK and this is NOT the first trial then wait a further 20 seconds for SQL Server for stability
                                System.Threading.Thread.Sleep(20000);
                                break;
                            }
                            else
                            {
                                // Otherwise wait a further 10 seconds just in case
                                System.Threading.Thread.Sleep(10000);
                                break;
                            }
                        }
                        else
                        {
                            System.Threading.Thread.Sleep(30000);
                        }
                    }
                    catch (SqlException sqlEx)
                    {
                        AxLogger.Instance.LogInfo("Service OnStart: Test Database Connection Exception Error: " + sqlEx.Message);
                        System.Threading.Thread.Sleep(30000);
                    }
                }

                if (!bConnectDbOK)
                {
                    AxLogger.Instance.LogInfo("Service OnStart: Test Database Connection has FAILED. Service SHOULD be RE-STARTED to try again.");
                    return;
                }

                int nNumTrials = 0;
                bool bRes = false;
                try
                {
                    while (nNumTrials < 5)
                    {
                        try
                        {
                            AxLogger.Instance.LogInfo("Service OnStart: BuildPCLExamTypeDeptLocMap: " +  (++nNumTrials).ToString());
                            bRes = aEMR.DataAccessLayer.Providers.PatientProvider.Instance.BuildPCLExamTypeDeptLocMap();
                            if (!bRes)
                            {
                                Trace.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff --- ") + "Error has occurred while The PCLExamTypeDeptLoc Map was built.");
                                Trace.Flush();
                            }
                            else
                            {
                                break;
                            }
                        }
                        catch (SqlException sqlEx2)
                        {
                            AxLogger.Instance.LogInfo("Service OnStart: BuildPCLExamTypeDeptLocMap Exception Error: " + sqlEx2.Message);
                            if (sqlEx2.Number == 1205)
                            {
                                System.Threading.Thread.Sleep(30000);
                            }
                            else
                            {
                                System.Threading.Thread.Sleep(10000);
                            }
                        }
                       
                    }
                    AxLogger.Instance.LogInfo("Service OnStart: BuildPclDeptLocationList ");
                    bRes = aEMR.DataAccessLayer.Providers.PatientProvider.Instance.BuildPclDeptLocationList();
                    if (!bRes)
                    {
                        Trace.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff --- ") + "Error has occurred while The PclDeptLocation List was built.");
                        Trace.Flush();
                    }

                    AxLogger.Instance.LogInfo("Service OnStart: BuildAllServiceIdDeptLocMap ");
                    bRes = aEMR.DataAccessLayer.Providers.PatientProvider.Instance.BuildAllServiceIdDeptLocMap();
                    if (!bRes)
                    {
                        Trace.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff --- ") + "Error has occurred while The Services DeptLocations List was built.");
                        Trace.Flush();
                    }

                    AxLogger.Instance.LogInfo("Service OnStart: BuildAllRefGenericRelationMap ");
                    try
                    {
                        aEMR.DataAccessLayer.Providers.PatientProvider.Instance.BuildAllRefGenericRelationMap();
                        bRes = true;
                    }
                    catch
                    {
                        bRes = false;
                        Trace.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff --- ") + "Error has occurred while The RefGenericRelations List was built.");
                        Trace.Flush();
                    }

                    AxLogger.Instance.LogInfo("Service OnStart: Before Loading All Config Values");
                    Globals.AxServerSettings = aEMR.DataAccessLayer.Providers.PatientProvider.Instance.GetApplicationConfigValues();
                    AxLogger.Instance.LogInfo("Service OnStart: After Loading All Config Values");
                    
                    //▼====== #002
                    AxLogger.Instance.LogInfo("Service OnStart: Before Loading All Insurance Benefit Caterories");
                    List<InsuranceBenefitCategories> AxInsuranceBenefitCategory = aEMR.DataAccessLayer.Providers.PatientProvider.Instance.GetInsuranceBenefitCategoriesValues();
                    AxLogger.Instance.LogInfo("Service OnStart: After Loading All Insurance Benefit Caterories");
                    //▲====== #002

                    if (AxInsuranceBenefitCategory != null && AxInsuranceBenefitCategory.Count > 0 && !string.IsNullOrEmpty(Globals.AxServerSettings.CommonItems.ValidHIPattern))
                    {
                        Globals.AxServerSettings.CommonItems.ValidHIPattern = Globals.AxServerSettings.CommonItems.ValidHIPattern.Replace("{0}", string.Join("|", AxInsuranceBenefitCategory.Select(x => x.HIPCode).ToArray()));
                        AxLogger.Instance.LogInfo(string.Format("Generated Check HI Card Valid Pattern: {0}", Globals.AxServerSettings.CommonItems.ValidHIPattern));
                        try
                        {
                            Globals.AxServerSettings.CommonItems.InsuranceBenefitCategories = AxInsuranceBenefitCategory.Select(x => new string[] { x.HIPCode, x.BenefitCode, x.IBeID.ToString() }).ToList();
                            AxLogger.Instance.LogInfo("Generated InsuranceBenefitCategories Dictionary");
                        }
                        catch
                        {
                            Globals.AxServerSettings.CommonItems.InsuranceBenefitCategories = null;
                        }
                    }
                    else
                    {
                        Globals.AxServerSettings.CommonItems.ValidHIPattern = null;
                    }
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogError("Service OnStart: BUILDING Cached Values Exception Error: " + ex.Message);
                    Trace.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff --- ") + "Error occurred while building Cached Reference Data Map. Error: " + ex.Message);
                    Trace.Flush();
                    AxLogger.Instance.LogInfo("Service OnStart: BUILDING Cached Values HAS FAILED. Service SHOULD be RE-STARTED to try again.");
                    return;
                }

                try
                {
                    AxLogger.Instance.LogInfo("Service OnStart: ServiceSequenceNumberProvider.Init BEGIN.");

                    ServiceSequenceNumberProvider.Init();//De cho no khoi tao he thong cap so 1 lan duy nhat.

                    AxLogger.Instance.LogInfo("Service OnStart: ServiceSequenceNumberProvider.Init END. About to Start Services:");

                    _hostManager.AddServiceHost(typeof(PatientRegistrationService.PatientRegistrationService));
                    _hostManager.AddServiceHost(typeof(BillingPaymentWcfServiceLib.BillingPaymentWcfServiceLib));
                    _hostManager.AddServiceHost(typeof(CommonService_V2.CommonService_V2));
                    //_hostManager.AddServiceHost(typeof(CommonServices.CommonService));
                    _hostManager.AddServiceHost(typeof(PharmacyService.DrugService));
                    _hostManager.AddServiceHost(typeof(DispMedRscrService.DispMedRscrService));
                    _hostManager.AddServiceHost(typeof(ConsultationsService.PtDashboard.CommonRecords.CommonRecordsServices));
                    _hostManager.AddServiceHost(typeof(ConsultationsService.PtDashboard.Summary.SummaryServices));
                    _hostManager.AddServiceHost(typeof(ConsultationsService.ePrescriptions.ePrescriptionServices));
                    _hostManager.AddServiceHost(typeof(ConsultationsService.ePMRs.ePMRsServices));
                    _hostManager.AddServiceHost(typeof(ConsultationsService.Common.CommonUtilServices));
                    _hostManager.AddServiceHost(typeof(ConsultationsService.ParaClinical.PCLsImportService));
                    _hostManager.AddServiceHost(typeof(ConsultationsService.ParaClinical.PCLsService));
                    _hostManager.AddServiceHost(typeof(UserManagementService.UserManagementService));
                    _hostManager.AddServiceHost(typeof(PharmacyService.SupplierService));
                    _hostManager.AddServiceHost(typeof(PharmacyService.SaleAndOutward));
                    _hostManager.AddServiceHost(typeof(PharmacyService.StorageService));
                    _hostManager.AddServiceHost(typeof(PharmacyService.Unit));
                    _hostManager.AddServiceHost(typeof(PharmacyService.InwardDrugService));
                    _hostManager.AddServiceHost(typeof(PharmacyService.InCLinicDeptService));
                    _hostManager.AddServiceHost(typeof(PharmacyService.InMedDeptService));
                    _hostManager.AddServiceHost(typeof(PharmacyService.EstimateDrugDeptService));
                    _hostManager.AddServiceHost(typeof(PharmacyService.RefGenMedProductDetailsService));
                    _hostManager.AddServiceHost(typeof(TransactionService.Transactions.TransactionService));
                    _hostManager.AddServiceHost(typeof(ReportService.AxReportService));
                    _hostManager.AddServiceHost(typeof(ResourcesManagementService.ResourcesManagementService));
                    _hostManager.AddServiceHost(typeof(AppointmentService.AppointmentService));
                    _hostManager.AddServiceHost(typeof(ConfigurationManagerService.ConfigurationManagerService));
                    _hostManager.AddServiceHost(typeof(ConfigurationManagerService.BedAllocationsService));
                    _hostManager.AddServiceHost(typeof(ConfigurationManagerService.UserAccountsService));
                    _hostManager.AddServiceHost(typeof(ClinicManagementService.ClinicManagementService));
                    _hostManager.AddServiceHost(typeof(LabSoftService.LabSoftService));
                    _hostManager.AddServiceHost(typeof(RISService.RISService));
                    _hostManager.AddServiceHost(typeof(PACService.PACService));
                    //_hostManager.AddServiceHost(typeof(ConsultationsService.PatientInstruction.PatientInstructionService));
                    _hostManager.Open(true);

                    //▼====: #001
                    ReportService.AxReportService mIAxReportService = new ReportService.AxReportService(true);
                    //▲====: #001

                    AxLogger.Instance.LogInfo("Service OnStart: All services started.");

                    //EventManagementService.EventManagementService.StartOutstandingTaskServer();

                    Trace.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff --- ") + "All services started.");
                    Trace.Flush();

                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogError("Service OnStart: STARTING WCF SERVICES Exception Error: " + ex.Message);
                    AxLogger.Instance.LogInfo("Service OnStart: STARTING WCF SERVICES HAS FAILED. Windows Service SHOULD be RE-STARTED to try again.");
                    return;
                }

            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("Service OnStart: GENERAL EXCEPTION ERROR: " + ex.Message);
                Trace.WriteLine("Could not open the ServiceHost : " + ex.Message);
                Trace.Flush();
                throw;
            }
        }

        //void workerThread_DoWork_Temp(object sender, DoWorkEventArgs e)
        //{
        //    try
        //    {
        //        _hostManagerTemp.AddServiceHost(typeof(EventManagementService.EventManagementService));
        //        _hostManagerTemp.Open();
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.WriteLine("Could not open the ServiceHost : " + ex.Message);
        //        throw;
        //    }
        //}

        protected override void OnStop()
        {            
            _hostManager.CLose();
            _hostManager = null;
        }
        public void StartDebug()
        {
            OnStart(null);
        }
    }
}
