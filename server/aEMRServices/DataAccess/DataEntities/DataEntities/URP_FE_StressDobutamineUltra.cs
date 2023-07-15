using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace DataEntities
{
    public partial class URP_FE_StressDobutamineUltra
    {
        [DataMemberAttribute()]
        public URP_FE_StressDobutamine ObjURP_FE_StressDobutamine { get; set; }
        [DataMemberAttribute()]
        public URP_FE_StressDobutamineElectrocardiogram ObjURP_FE_StressDobutamineElectrocardiogram { get; set; }
        [DataMemberAttribute()]
        public URP_FE_StressDobutamineExam ObjURP_FE_StressDobutamineExam { get; set; }
        [DataMemberAttribute()]
        public URP_FE_StressDobutamineImages ObjURP_FE_StressDobutamineImages { get; set; }
        [DataMemberAttribute()]
        public URP_FE_StressDobutamineResult ObjURP_FE_StressDobutamineResult { get; set; }
        [DataMemberAttribute()]
        public URP_FE_Exam ObjURP_FE_Exam { get; set; }
        [DataMemberAttribute()]
        public PatientPCLImagingResult ObjPatientPCLImagingResult { get; set; }
        [DataMemberAttribute()]
        public List<PCLResultFileStorageDetail> FileForStore { get; set; }
        [DataMemberAttribute()]
        public List<PCLResultFileStorageDetail> FileForDelete { get; set; }
        [DataMemberAttribute()]
        public long PCLRequestID { get; set; }
    }
}
