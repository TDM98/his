using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace DataEntities
{
    public partial class URP_FE_StressDipyridamoleUltra
    {
        [DataMemberAttribute()]
        public URP_FE_StressDipyridamole ObjURP_FE_StressDipyridamole { get; set; }
        [DataMemberAttribute()]
        public URP_FE_StressDipyridamoleElectrocardiogram ObjURP_FE_StressDipyridamoleElectrocardiogram { get; set; }
        [DataMemberAttribute()]
        public URP_FE_StressDipyridamoleExam ObjURP_FE_StressDipyridamoleExam { get; set; }
        [DataMemberAttribute()]
        public URP_FE_StressDipyridamoleImage ObjURP_FE_StressDipyridamoleImage { get; set; }
        [DataMemberAttribute()]
        public URP_FE_StressDipyridamoleResult ObjURP_FE_StressDipyridamoleResult { get; set; }
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
