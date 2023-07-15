using DataEntities;
/*
 * 20180923 #001 TTM: Tạo hàm để trường hợp view này là màn hình con lấy đc thông tin Patient từ view cha (ConsultationSummary)
 */
namespace aEMR.ViewContracts
{
    public interface IPatientPCLImagingResult_V2
    {
        void InitHTML();
        void CheckTemplatePCLResultByReqID(long PatientPCLReqID, bool InPt);
        //void LoadDataCoroutineEx(PatientPCLRequest aPCLRequest);
       
    }
}