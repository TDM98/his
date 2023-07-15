using System;
using System.Diagnostics;
using System.Text;
using AxLogging;
using aEmrUtilProcSvr2018Lib;
using eHCMS.Configurations;

namespace eHCMS.Services.Core
{
    public class ServiceSequenceNumberProvider
    {
        static private readonly ServiceSequenceNumberProvider _instance = null;

        static public ServiceSequenceNumberProvider Instance
        {
            get
            {
                lock (typeof(ServiceSequenceNumberProvider))
                {
                    if (_instance == null)
                    {
                        return new ServiceSequenceNumberProvider();
                    }
                }
                return _instance;
            }
        }
        static aEmrUtilProcSvr2018Lib.InitUtilProcSvr _workFlowSvr;
        static ServiceSequenceNumberProvider()
        {
            //IInitWorkFlowProcSvr workFlowSvr = new InitWorkFlowProcSvr();
            try
            {
                AxLogger.Instance.LogInfo("ServiceSequenceNumberProvider: About to Create InitWorkFlowProcSvr COM object"); 
                _workFlowSvr = new aEmrUtilProcSvr2018Lib.InitUtilProcSvr();
                ushort retCode;
                AxLogger.Instance.LogInfo("ServiceSequenceNumberProvider: BEFORE Initialize COM Object.");

                string theConnStr = Globals.Settings.Patients.ConnectionString;

                //_workFlowSvr.InitializeServer("", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), 20, out retCode);
                // TxD 12/06/2014: Changed Sequence Number for Non Appointment Service Sequence Num
                //_workFlowSvr.InitializeServer("", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), 1, out retCode);
                
                _workFlowSvr.InitializeServer(theConnStr, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), 1, out retCode);

                AxLogger.Instance.LogInfo("ServiceSequenceNumberProvider: AFTER Initialize COM Object."); 
                Debug.WriteLine("Init workflow server: " + retCode);                
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("ServiceSequenceNumberProvider: FAILED to Create OR Initialize COM Object: " + ex.ToString());    
            }
            
        }
        public static void Init()
        {
            //Khong lam gi het. De ben ngoai goi vay thoi
        }
        static long PCLSequenceNumber = 0;
        static uint ApptRegDetailSeqNum = 0;

        static Int16 PharmacyReceivedDrugtmp = 0;

        public void GetConsultRoomAndSequenceNumberAuto(out byte SequenceNumberType, ref uint DeptLocID, out uint SequenceNumber)
        {
            //TODO:Se goi ham anh Tuan
            IServiceSeqNumProvider provider = new ServiceSeqNumProvider();
            provider.GetConsultRoomAndSeqNumAuto(out SequenceNumberType,ref DeptLocID,out SequenceNumber);
        }

        public bool ConfirmApptConsultRoomAndSequenceNumber(int bAutoReassign, out int bReassignedSeqNum, out byte SequenceNumberType, ref uint DeptLocID, ref uint SequenceNumber)
        {
            IServiceSeqNumProvider provider = new ServiceSeqNumProvider();
            provider.ConfirmApptConsultRoomAndSeqNum(bAutoReassign, out bReassignedSeqNum,out SequenceNumberType,ref DeptLocID,ref SequenceNumber);
           
            return true;
        }
        //b2d : Get SequentNumber for appt service detail
        public void GetApptDetailServiceSequenceNumber(uint DeptLocID, byte ConsultationTimeSegmentID, string ApptDate, out byte SequenceNumberType, out uint SequenceNumber, out byte ErrorNumber, out string ErrorMessage)
        {
            SequenceNumberType = 8;
            ErrorNumber = 0;
            ErrorMessage = "";
            IServiceSeqNumProvider provider = new ServiceSeqNumProvider();            
            object lockthis=new object();
            lock (lockthis)
            {
                SequenceNumber = ++ApptRegDetailSeqNum;
            }
        }

        public void GetPCLSequenceNumber(out byte SequenceNumberType, long DeptLocID, out long SequenceNumber)
        {
            SequenceNumberType = 88;
            SequenceNumber = ++PCLSequenceNumber;
        }
        
        public void GetPCLSequenceNumber(string strSeqNum, out string strSeqNumReturned)
        {
            //TODO:Se goi ham anh Tuan
            IServiceSeqNumProvider provider = new ServiceSeqNumProvider();
            provider.GetPCLSeqNumString(strSeqNum, out strSeqNumReturned);

            return;
        }

        public void ConsuConsultationCompleted(long DeptLocID)
        {

        }

        public void GetPharmacyOutDrugInvSeqNum(out byte SequenceNumberType, PharmacyInvType type, out uint SeqNumber)
        {
            IServiceSeqNumProvider provider = new ServiceSeqNumProvider();
            provider.GetPharmacyOutDrugInvSeqNum(out SequenceNumberType, (byte)type, out SeqNumber);
        }

        public void PharmacyServiceNumber(out byte SequenceNumberType, PharmacyInvType type, out uint SeqNumber)
        {
            IServiceSeqNumProvider provider = new ServiceSeqNumProvider();
            provider.GetPharmacyPrescriptionSeqNum(out SequenceNumberType, (byte)type, out SeqNumber);
        }

        public Int16 PharmacyReceivedDrug(byte SequenceNumberType)
        {
            return ++PharmacyReceivedDrugtmp;
        }

        public string GetPatientCode()
        {
            IIdentInvSeqNumProvider provider = new IdentInvSeqNumProvider();
            string strNewPatientCode;
            provider.GetNewPatientCode(out strNewPatientCode);
            return strNewPatientCode;
        }
        public string GetReceiptNumber(int bNgoaiTru)
        {
            //0:ngoai tru;1:noi tru
            return null;
        }

        public string GetReceiptNumber_NgoaiTru(int bNgoaiTru)
        {
            //0:ngoai tru;1:noi tru
            if (bNgoaiTru > 0)
            {
                return null;
            }
            else
            {
                IIdentInvSeqNumProvider provider = new IdentInvSeqNumProvider();
                string strNewOutPtPaymtInvNum="";
                provider.GetNewOutPtPaymtInvNum(out strNewOutPtPaymtInvNum);
                return strNewOutPtPaymtInvNum;
            }
        }
        public string GetCode_NhapHang_NhaThuoc()
        {
            return "";
        }

        //public string GetCode_PCLRequest(long V_PCLMainCategory)
        //{
        //    string Prefix = "PCL";
        //    //TO DO : goi ham anh Tuan
        //    return Prefix + "";
        //}

        public string GetCode_PCLRequest(long V_PCLMainCategory)
        {
            IIdentInvSeqNumProvider provider = new IdentInvSeqNumProvider();
            string strNewPclReqIdNum = "";
            provider.GetNewPCLReqNum((uint)V_PCLMainCategory, out strNewPclReqIdNum);
            return strNewPclReqIdNum;
        }

        public string GetCode_PCLRequest_InPt(long V_PCLMainCategory)
        {
            IIdentInvSeqNumProvider provider = new IdentInvSeqNumProvider();
            string strNewPclReqIdNum = "";
            provider.GetNewPCLReqNum_InPt((uint)V_PCLMainCategory, out strNewPclReqIdNum);
            return strNewPclReqIdNum;
        }

        public string GetCode_AppPCLRequest(long V_PCLMainCategory)
        {
            string Prefix = "PCL";
            //TO DO : goi ham anh Tuan
            return Prefix + "";
        }
    }

    public enum PharmacyInvType:byte
    {
        CO_BAO_HIEM = 2,
        KHONG_BAO_HIEM = 1
    }
}
