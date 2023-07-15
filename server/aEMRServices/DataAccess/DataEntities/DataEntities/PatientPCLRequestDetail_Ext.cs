using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    
    public partial class PatientPCLRequestDetail_Ext : PatientPCLRequestDetail
    {
        public PatientPCLRequestDetail_Ext()
            : base()
        {
            MedProductType = AllLookupValues.MedProductType.CAN_LAM_SANG;
            ExamRegStatus = AllLookupValues.ExamRegStatus.DANG_KY_KHAM;
        }
        #region Factory Method


        /// Create a new PatientPCLRequestDetail_Exts object.

        /// <param name="PCLReqItemID">Initial value of the PCLReqItemID property.</param>
        /// <param name="pCLItemID">Initial value of the PCLItemID property.</param>
        /// <param name="patientPCLAppID">Initial value of the PatientPCLAppID property.</param>
        public static PatientPCLRequestDetail_Ext CreatePatientPCLRequestDetail_Exts(long pCLReqItemID, long pclExamTypeID, long patientPCLReqID)
        {
            PatientPCLRequestDetail_Ext PatientPCLRequestDetail_Exts = new PatientPCLRequestDetail_Ext();
            PatientPCLRequestDetail_Exts.PCLReqItemID = pCLReqItemID;
            PatientPCLRequestDetail_Exts.PCLExamTypeID = pclExamTypeID;
            PatientPCLRequestDetail_Exts.PatientPCLReqID = patientPCLReqID;
            return PatientPCLRequestDetail_Exts;
        }

        #endregion

        private long _PatientPCLReqExtID;
        [DataMemberAttribute]
        public long PatientPCLReqExtID
         {
             get
             {
                 return _PatientPCLReqExtID;
             }
             set
             {
                 _PatientPCLReqExtID = value;
                 RaisePropertyChanged("PatientPCLReqExtID");
             }
         }

        private long _PCLReqItemExtID;
        [DataMemberAttribute]
        public long PCLReqItemExtID
         {
             get
             {
                 return _PCLReqItemExtID;
             }
             set
             {
                 _PCLReqItemExtID = value;
                 RaisePropertyChanged("PCLReqItemExtID");
             }
         }

        private PatientPCLRequest_Ext _patientPCLRequest_Ext;
        [DataMemberAttribute]
        public PatientPCLRequest_Ext patientPCLRequest_Ext
        {
            get
            {
                return _patientPCLRequest_Ext;
            }
            set
            {
                _patientPCLRequest_Ext = value;
                RaisePropertyChanged("patientPCLRequest_Ext");
            }
        }

        public override string ToString()
        {
            return this.PCLExamType==null? "":
                this.PCLExamType.PCLExamTypeID.ToString() + ","+this.PCLExamType.PCLExamTypeName.ToString();
        }
    }
}
