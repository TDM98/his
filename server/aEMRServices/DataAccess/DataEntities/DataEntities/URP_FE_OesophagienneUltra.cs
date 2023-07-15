using System;
using System.Runtime.Serialization;
using eHCMS.Services.Core.Base;
using System.Collections.Generic;

namespace DataEntities
{
    public partial class URP_FE_OesophagienneUltra : NotifyChangedBase
    {
        private URP_FE_Oesophagienne _ObjURP_FE_Oesophagienne;
        [DataMemberAttribute()]
        public URP_FE_Oesophagienne ObjURP_FE_Oesophagienne
        {
            get { return _ObjURP_FE_Oesophagienne; }
            set
            {
                if (_ObjURP_FE_Oesophagienne != value)
                {
                    OnURP_FE_OesophagienneChanging(value);
                    _ObjURP_FE_Oesophagienne = value;
                    RaisePropertyChanged("ObjURP_FE_Oesophagienne");
                    OnURP_FE_OesophagienneChanged();
                }
            }
        }
        partial void OnURP_FE_OesophagienneChanging(URP_FE_Oesophagienne value);
        partial void OnURP_FE_OesophagienneChanged();

        private URP_FE_OesophagienneCheck _ObjURP_FE_OesophagienneCheck;
        [DataMemberAttribute()]
        public URP_FE_OesophagienneCheck ObjURP_FE_OesophagienneCheck
        {
            get { return _ObjURP_FE_OesophagienneCheck; }
            set
            {
                if (_ObjURP_FE_OesophagienneCheck != value)
                {
                    OnURP_FE_OesophagienneCheckChanging(value);
                    _ObjURP_FE_OesophagienneCheck = value;
                    RaisePropertyChanged("ObjURP_FE_OesophagienneCheck");
                    OnURP_FE_OesophagienneCheckChanged();
                }
            }
        }
        partial void OnURP_FE_OesophagienneCheckChanging(URP_FE_OesophagienneCheck value);
        partial void OnURP_FE_OesophagienneCheckChanged();

        private URP_FE_OesophagienneDiagnosis _ObjURP_FE_OesophagienneDiagnosis;
        [DataMemberAttribute()]
        public URP_FE_OesophagienneDiagnosis ObjURP_FE_OesophagienneDiagnosis
        {
            get { return _ObjURP_FE_OesophagienneDiagnosis; }
            set
            {
                if (_ObjURP_FE_OesophagienneDiagnosis != value)
                {
                    OnURP_FE_OesophagienneDiagnosisChanging(value);
                    _ObjURP_FE_OesophagienneDiagnosis = value;
                    RaisePropertyChanged("ObjURP_FE_OesophagienneDiagnosis");
                    OnURP_FE_OesophagienneDiagnosisChanged();
                }
            }
        }
        partial void OnURP_FE_OesophagienneDiagnosisChanging(URP_FE_OesophagienneDiagnosis value);
        partial void OnURP_FE_OesophagienneDiagnosisChanged();

        [DataMemberAttribute()]
        public PatientPCLImagingResult ObjPatientPCLImagingResult { get; set; }
        [DataMemberAttribute()]
        public List<PCLResultFileStorageDetail> FileForStore { get; set; }
        [DataMemberAttribute()]
        public List<PCLResultFileStorageDetail> FileForDelete { get; set; }
        [DataMemberAttribute()]
        public long PCLRequestID
        {
            get
            {
                return _PCLRequestID;
            }
            set
            {
                if (_PCLRequestID != value)
                {
                    OnPCLRequestIDChanging(value);
                    _PCLRequestID = value;
                    RaisePropertyChanged("PCLRequestID");
                    OnPCLRequestIDChanged();
                }
            }
        }
        private long _PCLRequestID;
        partial void OnPCLRequestIDChanging(long value);
        partial void OnPCLRequestIDChanged();
    }
}
