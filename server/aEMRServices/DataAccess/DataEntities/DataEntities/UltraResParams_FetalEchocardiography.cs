using System;
using System.Runtime.Serialization;
using eHCMS.Services.Core.Base;
using System.Collections.Generic;

namespace DataEntities
{
    public partial class UltraResParams_FetalEchocardiography : NotifyChangedBase
    {
        private UltraResParams_FetalEchocardiography2D _ObjUltraResParams_FetalEchocardiography2D;
        [DataMemberAttribute()]
        public UltraResParams_FetalEchocardiography2D ObjUltraResParams_FetalEchocardiography2D
        {
            get { return _ObjUltraResParams_FetalEchocardiography2D; }
            set
            {
                if (_ObjUltraResParams_FetalEchocardiography2D != value)
                {
                    OnUltraResParams_FetalEchocardiography2DChanging(value);
                    _ObjUltraResParams_FetalEchocardiography2D = value;
                    RaisePropertyChanged("ObjUltraResParams_FetalEchocardiography2D");
                    OnUltraResParams_FetalEchocardiography2DChanged();
                }
            }
        }
        partial void OnUltraResParams_FetalEchocardiography2DChanging(UltraResParams_FetalEchocardiography2D value);
        partial void OnUltraResParams_FetalEchocardiography2DChanged();

        private UltraResParams_FetalEchocardiographyDoppler _ObjUltraResParams_FetalEchocardiographyDoppler;
        [DataMemberAttribute()]
        public UltraResParams_FetalEchocardiographyDoppler ObjUltraResParams_FetalEchocardiographyDoppler
        {
            get { return _ObjUltraResParams_FetalEchocardiographyDoppler; }
            set
            {
                if (_ObjUltraResParams_FetalEchocardiographyDoppler != value)
                {
                    OnUltraResParams_FetalEchocardiographyDopplerChanging(value);
                    _ObjUltraResParams_FetalEchocardiographyDoppler = value;
                    RaisePropertyChanged("ObjUltraResParams_FetalEchocardiographyDoppler");
                    OnUltraResParams_FetalEchocardiographyDopplerChanged();
                }
            }
        }
        partial void OnUltraResParams_FetalEchocardiographyDopplerChanging(UltraResParams_FetalEchocardiographyDoppler value);
        partial void OnUltraResParams_FetalEchocardiographyDopplerChanged();

        private UltraResParams_FetalEchocardiographyResult _ObjUltraResParams_FetalEchocardiographyResult;
        [DataMemberAttribute()]
        public UltraResParams_FetalEchocardiographyResult ObjUltraResParams_FetalEchocardiographyResult
        {
            get { return _ObjUltraResParams_FetalEchocardiographyResult; }
            set
            {
                if (_ObjUltraResParams_FetalEchocardiographyResult != value)
                {
                    OnUltraResParams_FetalEchocardiographyResultChanging(value);
                    _ObjUltraResParams_FetalEchocardiographyResult = value;
                    RaisePropertyChanged("ObjUltraResParams_FetalEchocardiographyResult");
                    OnUltraResParams_FetalEchocardiographyResultChanged();
                }
            }
        }
        partial void OnUltraResParams_FetalEchocardiographyResultChanging(UltraResParams_FetalEchocardiographyResult value);
        partial void OnUltraResParams_FetalEchocardiographyResultChanged();

        private UltraResParams_FetalEchocardiographyPostpartum _ObjUltraResParams_FetalEchocardiographyPostpartum;
        [DataMemberAttribute()]
        public UltraResParams_FetalEchocardiographyPostpartum ObjUltraResParams_FetalEchocardiographyPostpartum
        {
            get { return _ObjUltraResParams_FetalEchocardiographyPostpartum; }
            set
            {
                if (_ObjUltraResParams_FetalEchocardiographyPostpartum != value)
                {
                    OnUltraResParams_FetalEchocardiographyPostpartumChanging(value);
                    _ObjUltraResParams_FetalEchocardiographyPostpartum = value;
                    RaisePropertyChanged("ObjUltraResParams_FetalEchocardiographyPostpartum");
                    OnUltraResParams_FetalEchocardiographyPostpartumChanged();
                }
            }
        }
        partial void OnUltraResParams_FetalEchocardiographyPostpartumChanging(UltraResParams_FetalEchocardiographyPostpartum value);
        partial void OnUltraResParams_FetalEchocardiographyPostpartumChanged();

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
        //==== 20161213 CMN Begin: Add information
        [DataMemberAttribute()]
        public short FetalAge
        {
            get
            {
                return _FetalAge;
            }
            set
            {
                if (_FetalAge != value)
                {
                    OnFetalAgeChanging(value);
                    _FetalAge = value;
                    RaisePropertyChanged("FetalAge");
                    OnFetalAgeChanged();
                }
            }
        }
        private short _FetalAge;
        partial void OnFetalAgeChanging(short value);
        partial void OnFetalAgeChanged();

        [DataMemberAttribute()]
        public double NuchalTranslucency
        {
            get
            {
                return _NuchalTranslucency;
            }
            set
            {
                if (_NuchalTranslucency != value)
                {
                    OnNuchalTranslucencyChanging(value);
                    _NuchalTranslucency = value;
                    RaisePropertyChanged("NuchalTranslucency");
                    OnNuchalTranslucencyChanged();
                }
            }
        }
        private double _NuchalTranslucency;
        partial void OnNuchalTranslucencyChanging(double value);
        partial void OnNuchalTranslucencyChanged();

        [DataMemberAttribute()]
        public Lookup V_EchographyPosture
        {
            get
            {
                return _V_EchographyPosture;
            }
            set
            {
                if (_V_EchographyPosture != value)
                {
                    OnV_EchographyPostureChanging(value);
                    _V_EchographyPosture = value;
                    RaisePropertyChanged("V_EchographyPosture");
                    OnV_EchographyPostureChanged();
                }
            }
        }
        private Lookup _V_EchographyPosture;
        partial void OnV_EchographyPostureChanging(Lookup value);
        partial void OnV_EchographyPostureChanged();

        [DataMemberAttribute()]
        public Lookup V_MomMedHis
        {
            get
            {
                return _V_MomMedHis;
            }
            set
            {
                if (_V_MomMedHis != value)
                {
                    OnV_MomMedHisChanging(value);
                    _V_MomMedHis = value;
                    RaisePropertyChanged("V_MomMedHis");
                    OnV_MomMedHisChanged();
                }
            }
        }
        private Lookup _V_MomMedHis;
        partial void OnV_MomMedHisChanging(Lookup value);
        partial void OnV_MomMedHisChanged();

        [DataMemberAttribute()]
        public string Notice
        {
            get
            {
                return _Notice;
            }
            set
            {
                if (_Notice != value)
                {
                    OnNoticeChanging(value);
                    _Notice = value;
                    RaisePropertyChanged("Notice");
                    OnNoticeChanged();
                }
            }
        }
        private string _Notice;
        partial void OnNoticeChanging(string value);
        partial void OnNoticeChanged();

        [DataMemberAttribute()]
        public long PatientPCLReqID
        {
            get
            {
                return _PatientPCLReqID;
            }
            set
            {
                if (_PatientPCLReqID != value)
                {
                    OnPatientPCLReqIDChanging(value);
                    _PatientPCLReqID = value;
                    RaisePropertyChanged("PatientPCLReqID");
                    OnPatientPCLReqIDChanged();
                }
            }
        }
        private long _PatientPCLReqID;
        partial void OnPatientPCLReqIDChanging(long value);
        partial void OnPatientPCLReqIDChanged();

        [DataMemberAttribute()]
        public long UltraResParams_FetalEchocardiographyID
        {
            get
            {
                return _UltraResParams_FetalEchocardiographyID;
            }
            set
            {
                if (_UltraResParams_FetalEchocardiographyID != value)
                {
                    OnUltraResParams_FetalEchocardiographyIDChanging(value);
                    _UltraResParams_FetalEchocardiographyID = value;
                    RaisePropertyChanged("UltraResParams_FetalEchocardiographyID");
                    OnUltraResParams_FetalEchocardiographyIDChanged();
                }
            }
        }
        private long _UltraResParams_FetalEchocardiographyID;
        partial void OnUltraResParams_FetalEchocardiographyIDChanging(long value);
        partial void OnUltraResParams_FetalEchocardiographyIDChanged();

        [DataMemberAttribute()]
        public DateTime CreatedDate
        {
            get
            {
                return _CreatedDate;
            }
            set
            {
                if (_CreatedDate != value)
                {
                    OnCreatedDateChanging(value);
                    _CreatedDate = value;
                    RaisePropertyChanged("CreatedDate");
                    OnCreatedDateChanged();
                }
            }
        }
        private DateTime _CreatedDate;
        partial void OnCreatedDateChanging(DateTime value);
        partial void OnCreatedDateChanged();
        //==== 20161213 CMN End.
    }
}