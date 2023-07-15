using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class UltraResParams_FetalEchocardiography2D : NotifyChangedBase, IEditableObject
    {
        public UltraResParams_FetalEchocardiography2D()
            : base()
        {

        }
        public override bool Equals(object obj)
        {
            UltraResParams_FetalEchocardiography2D info = obj as UltraResParams_FetalEchocardiography2D;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.UltraResParams_FetalEchocardiography2DID > 0 && this.UltraResParams_FetalEchocardiography2DID == info.UltraResParams_FetalEchocardiography2DID;
        }
        private UltraResParams_FetalEchocardiography2D _tempUltraResParams_FetalEchocardiography2D;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempUltraResParams_FetalEchocardiography2D = (UltraResParams_FetalEchocardiography2D)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempUltraResParams_FetalEchocardiography2D)
                CopyFrom(_tempUltraResParams_FetalEchocardiography2D);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(UltraResParams_FetalEchocardiography2D p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new UltraResParams_FetalEchocardiography2D object.

        /// <param name="UltraResParams_FetalEchocardiography2DID">Initial value of the UltraResParams_FetalEchocardiography2DID property.</param>

        public static UltraResParams_FetalEchocardiography2D CreateUltraResParams_FetalEchocardiography2D(Byte UltraResParams_FetalEchocardiography2DID, String UltraResParams_FetalEchocardiography2DName)
        {
            UltraResParams_FetalEchocardiography2D UltraResParams_FetalEchocardiography2D = new UltraResParams_FetalEchocardiography2D();
            UltraResParams_FetalEchocardiography2D.UltraResParams_FetalEchocardiography2DID = UltraResParams_FetalEchocardiography2DID;

            return UltraResParams_FetalEchocardiography2D;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long UltraResParams_FetalEchocardiography2DID
        {
            get
            {
                return _UltraResParams_FetalEchocardiography2DID;
            }
            set
            {
                if (_UltraResParams_FetalEchocardiography2DID != value)
                {
                    OnUltraResParams_FetalEchocardiography2DIDChanging(value);
                    _UltraResParams_FetalEchocardiography2DID = value;
                    RaisePropertyChanged("UltraResParams_FetalEchocardiography2DID");
                    OnUltraResParams_FetalEchocardiography2DIDChanged();
                }
            }
        }
        private long _UltraResParams_FetalEchocardiography2DID;
        partial void OnUltraResParams_FetalEchocardiography2DIDChanging(long value);
        partial void OnUltraResParams_FetalEchocardiography2DIDChanged();



        [DataMemberAttribute()]
        public double NTSize
        {
            get
            {
                return _NTSize;
            }
            set
            {
                if (_NTSize != value)
                {
                    OnNTSizeChanging(value);
                    _NTSize = value;
                    RaisePropertyChanged("NTSize");
                    OnNTSizeChanged();
                }
            }
        }
        private double _NTSize;
        partial void OnNTSizeChanging(double value);
        partial void OnNTSizeChanged();

        [DataMemberAttribute()]
        public double NPSize
        {
            get
            {
                return _NPSize;
            }
            set
            {
                if (_NPSize != value)
                {
                    OnNPSizeChanging(value);
                    _NPSize = value;
                    RaisePropertyChanged("NPSize");
                    OnNPSizeChanged();
                }
            }
        }
        private double _NPSize;
        partial void OnNPSizeChanging(double value);
        partial void OnNPSizeChanged();

        [DataMemberAttribute()]
        public bool VanVieussensLeftAtrium
        {
            get
            {
                return _VanVieussensLeftAtrium;
            }
            set
            {
                if (_VanVieussensLeftAtrium != value)
                {
                    OnVanVieussensLeftAtriumChanging(value);
                    _VanVieussensLeftAtrium = value;
                    RaisePropertyChanged("VanVieussensLeftAtrium");
                    OnVanVieussensLeftAtriumChanged();
                }
            }
        }
        private bool _VanVieussensLeftAtrium = true;
        partial void OnVanVieussensLeftAtriumChanging(bool value);
        partial void OnVanVieussensLeftAtriumChanged();


        [DataMemberAttribute()]
        public bool AtrialSeptalDefect
        {
            get
            {
                return _AtrialSeptalDefect;
            }
            set
            {
                if (_AtrialSeptalDefect != value)
                {
                    OnAtrialSeptalDefectChanging(value);
                    _AtrialSeptalDefect = value;
                    RaisePropertyChanged("AtrialSeptalDefect");
                    OnAtrialSeptalDefectChanged();
                }
            }
        }
        private bool _AtrialSeptalDefect = false;
        partial void OnAtrialSeptalDefectChanging(bool value);
        partial void OnAtrialSeptalDefectChanged();

        [DataMemberAttribute()]
        public double MitralValveSize
        {
            get
            {
                return _MitralValveSize;
            }
            set
            {
                if (_MitralValveSize != value)
                {
                    OnMitralValveSizeChanging(value);
                    _MitralValveSize = value;
                    RaisePropertyChanged("MitralValveSize");
                    OnMitralValveSizeChanged();
                }
            }
        }
        private double _MitralValveSize;
        partial void OnMitralValveSizeChanging(double value);
        partial void OnMitralValveSizeChanged();

        [DataMemberAttribute()]
        public double TriscupidValveSize
        {
            get
            {
                return _TriscupidValveSize;
            }
            set
            {
                if (_TriscupidValveSize != value)
                {
                    OnTriscupidValveSizeChanging(value);
                    _TriscupidValveSize = value;
                    RaisePropertyChanged("TriscupidValveSize");
                    OnTriscupidValveSizeChanged();
                }
            }
        }
        private double _TriscupidValveSize;
        partial void OnTriscupidValveSizeChanging(double value);
        partial void OnTriscupidValveSizeChanged();

        [DataMemberAttribute()]
        public bool DifferenceMitralTricuspid
        {
            get
            {
                return _DifferenceMitralTricuspid;
            }
            set
            {
                if (_DifferenceMitralTricuspid != value)
                {
                    OnDifferenceMitralTricuspidChanging(value);
                    _DifferenceMitralTricuspid = value;
                    RaisePropertyChanged("DifferenceMitralTricuspid");
                    OnDifferenceMitralTricuspidChanged();
                }
            }
        }
        private bool _DifferenceMitralTricuspid = true;
        partial void OnDifferenceMitralTricuspidChanging(bool value);
        partial void OnDifferenceMitralTricuspidChanged();

        [DataMemberAttribute()]
        public double TPTTr
        {
            get
            {
                return _TPTTr;
            }
            set
            {
                if (_TPTTr != value)
                {
                    OnTPTTrChanging(value);
                    _TPTTr = value;
                    RaisePropertyChanged("TPTTr");
                    OnTPTTrChanged();
                }
            }
        }
        private double _TPTTr;
        partial void OnTPTTrChanging(double value);
        partial void OnTPTTrChanged();

        [DataMemberAttribute()]
        public double VLTTTr
        {
            get
            {
                return _VLTTTr;
            }
            set
            {
                if (_VLTTTr != value)
                {
                    OnVLTTTrChanging(value);
                    _VLTTTr = value;
                    RaisePropertyChanged("VLTTTr");
                    OnVLTTTrChanged();
                }
            }
        }
        private double _VLTTTr;
        partial void OnVLTTTrChanging(double value);
        partial void OnVLTTTrChanged();

        [DataMemberAttribute()]
        public double TTTTr
        {
            get
            {
                return _TTTTr;
            }
            set
            {
                if (_TTTTr != value)
                {
                    OnTTTTrChanging(value);
                    _TTTTr = value;
                    RaisePropertyChanged("TTTTr");
                    OnTTTTrChanged();
                }
            }
        }
        private double _TTTTr;
        partial void OnTTTTrChanging(double value);
        partial void OnTTTTrChanged();

        [DataMemberAttribute()]
        public double DKTTTTr_VGd
        {
            get
            {
                return _DKTTTTr_VGd;
            }
            set
            {
                if (_DKTTTTr_VGd != value)
                {
                    OnDKTTTTr_VGdChanging(value);
                    _DKTTTTr_VGd = value;
                    RaisePropertyChanged("DKTTTTr_VGd");
                    OnDKTTTTr_VGdChanged();
                }
            }
        }
        private double _DKTTTTr_VGd;
        partial void OnDKTTTTr_VGdChanging(double value);
        partial void OnDKTTTTr_VGdChanged();

        [DataMemberAttribute()]
        public double DKTTTT_VGs
        {
            get
            {
                return _DKTTTT_VGs;
            }
            set
            {
                if (_DKTTTT_VGs != value)
                {
                    OnDKTTTT_VGsChanging(value);
                    _DKTTTT_VGs = value;
                    RaisePropertyChanged("DKTTTT_VGs");
                    OnDKTTTT_VGsChanged();
                }
            }
        }
        private double _DKTTTT_VGs;
        partial void OnDKTTTT_VGsChanging(double value);
        partial void OnDKTTTT_VGsChanged();

        [DataMemberAttribute()]
        public double DKTPTTr_VDd
        {
            get
            {
                return _DKTPTTr_VDd;
            }
            set
            {
                if (_DKTPTTr_VDd != value)
                {
                    OnDKTPTTr_VDdChanging(value);
                    _DKTPTTr_VDd = value;
                    RaisePropertyChanged("DKTPTTr_VDd");
                    OnDKTPTTr_VDdChanged();
                }
            }
        }
        private double _DKTPTTr_VDd;
        partial void OnDKTPTTr_VDdChanging(double value);
        partial void OnDKTPTTr_VDdChanged();

        [DataMemberAttribute()]
        public double DKTPTT_VDs
        {
            get
            {
                return _DKTPTT_VDs;
            }
            set
            {
                if (_DKTPTT_VDs != value)
                {
                    OnDKTPTT_VDsChanging(value);
                    _DKTPTT_VDs = value;
                    RaisePropertyChanged("DKTPTT_VDs");
                    OnDKTPTT_VDsChanged();
                }
            }
        }
        private double _DKTPTT_VDs;
        partial void OnDKTPTT_VDsChanging(double value);
        partial void OnDKTPTT_VDsChanged();


        [DataMemberAttribute()]
        public bool Systolic
        {
            get
            {
                return _Systolic;
            }
            set
            {
                if (_Systolic != value)
                {
                    OnSystolicChanging(value);
                    _Systolic = value;
                    RaisePropertyChanged("Systolic");
                    OnSystolicChanged();
                }
            }
        }
        private bool _Systolic = true;
        partial void OnSystolicChanging(bool value);
        partial void OnSystolicChanged();

        [DataMemberAttribute()]
        public bool VentricularSeptalDefect
        {
            get
            {
                return _VentricularSeptalDefect;
            }
            set
            {
                if (_VentricularSeptalDefect != value)
                {
                    OnVentricularSeptalDefectChanging(value);
                    _VentricularSeptalDefect = value;
                    RaisePropertyChanged("VentricularSeptalDefect");
                    OnVentricularSeptalDefectChanged();
                }
            }
        }
        private bool _VentricularSeptalDefect = true;
        partial void OnVentricularSeptalDefectChanging(bool value);
        partial void OnVentricularSeptalDefectChanged();

        [DataMemberAttribute()]
        public bool AortaCompatible
        {
            get
            {
                return _AortaCompatible;
            }
            set
            {
                if (_AortaCompatible != value)
                {
                    OnAortaCompatibleChanging(value);
                    _AortaCompatible = value;
                    RaisePropertyChanged("AortaCompatible");
                    OnAortaCompatibleChanged();
                }
            }
        }
        private bool _AortaCompatible = true;
        partial void OnAortaCompatibleChanging(bool value);
        partial void OnAortaCompatibleChanged();

        [DataMemberAttribute()]
        public double AortaSize
        {
            get
            {
                return _AortaSize;
            }
            set
            {
                if (_AortaSize != value)
                {
                    OnAortaSizeChanging(value);
                    _AortaSize = value;
                    RaisePropertyChanged("AortaSize");
                    OnAortaSizeChanged();
                }
            }
        }
        private double _AortaSize;
        partial void OnAortaSizeChanging(double value);
        partial void OnAortaSizeChanged();


        [DataMemberAttribute()]
        public double PulmonaryArterySize
        {
            get
            {
                return _PulmonaryArterySize;
            }
            set
            {
                if (_PulmonaryArterySize != value)
                {
                    OnPulmonaryArterySizeChanging(value);
                    _PulmonaryArterySize = value;
                    RaisePropertyChanged("PulmonaryArterySize");
                    OnPulmonaryArterySizeChanged();
                }
            }
        }
        private double _PulmonaryArterySize;
        partial void OnPulmonaryArterySizeChanging(double value);
        partial void OnPulmonaryArterySizeChanged();

        [DataMemberAttribute()]
        public bool AorticArch
        {
            get
            {
                return _AorticArch;
            }
            set
            {
                if (_AorticArch != value)
                {
                    OnAorticArchChanging(value);
                    _AorticArch = value;
                    RaisePropertyChanged("AorticArch");
                    OnAorticArchChanged();
                }
            }
        }
        private bool _AorticArch = true;
        partial void OnAorticArchChanging(bool value);
        partial void OnAorticArchChanged();

        [DataMemberAttribute()]
        public double AorticCoarctation
        {
            get
            {
                return _AorticCoarctation;
            }
            set
            {
                if (_AorticCoarctation != value)
                {
                    OnAorticCoarctationChanging(value);
                    _AorticCoarctation = value;
                    RaisePropertyChanged("AorticCoarctation");
                    OnAorticCoarctationChanged();
                }
            }
        }
        private double _AorticCoarctation;
        partial void OnAorticCoarctationChanging(double value);
        partial void OnAorticCoarctationChanged();

        [DataMemberAttribute()]
        public bool HeartRateNomal
        {
            get
            {
                return _HeartRateNomal;
            }
            set
            {
                if (_HeartRateNomal != value)
                {
                    OnHeartRateNomalChanging(value);
                    _HeartRateNomal = value;
                    RaisePropertyChanged("HeartRateNomal");
                    OnHeartRateNomalChanged();
                }
            }
        }
        private bool _HeartRateNomal = true;
        partial void OnHeartRateNomalChanging(bool value);
        partial void OnHeartRateNomalChanged();

        [DataMemberAttribute()]
        public double RequencyHeartRateNomal
        {
            get
            {
                return _RequencyHeartRateNomal;
            }
            set
            {
                if (_RequencyHeartRateNomal != value)
                {
                    OnRequencyHeartRateNomalChanging(value);
                    _RequencyHeartRateNomal = value;
                    RaisePropertyChanged("RequencyHeartRateNomal");
                    OnRequencyHeartRateNomalChanged();
                }
            }
        }
        private double _RequencyHeartRateNomal;
        partial void OnRequencyHeartRateNomalChanging(double value);
        partial void OnRequencyHeartRateNomalChanged();

        [DataMemberAttribute()]
        public bool PericardialEffusion
        {
            get
            {
                return _PericardialEffusion;
            }
            set
            {
                if (_PericardialEffusion != value)
                {
                    OnPericardialEffusionChanging(value);
                    _PericardialEffusion = value;
                    RaisePropertyChanged("PericardialEffusion");
                    OnPericardialEffusionChanged();
                }
            }
        }
        private bool _PericardialEffusion = false;
        partial void OnPericardialEffusionChanging(bool value);
        partial void OnPericardialEffusionChanged();

        [DataMemberAttribute()]
        public double FetalCardialAxis
        {
            get
            {
                return _FetalCardialAxis;
            }
            set
            {
                if (_FetalCardialAxis != value)
                {
                    OnFetalCardialAxisChanging(value);
                    _FetalCardialAxis = value;
                    RaisePropertyChanged("FetalCardialAxis");
                    OnFetalCardialAxisChanged();
                }
            }
        }
        private double _FetalCardialAxis;
        partial void OnFetalCardialAxisChanging(double value);
        partial void OnFetalCardialAxisChanged();

        [DataMemberAttribute()]
        public double CardialRateS
        {
            get
            {
                return _CardialRateS;
            }
            set
            {
                if (_CardialRateS != value)
                {
                    OnCardialRateSChanging(value);
                    _CardialRateS = value;
                    if (_CardialRateS > 0  && LN > 0)
                    {
                        RateS = _CardialRateS / LN;
                    }
                    RaisePropertyChanged("CardialRateS");
                    OnCardialRateSChanged();
                }
            }
        }
        private double _CardialRateS;
        partial void OnCardialRateSChanging(double value);
        partial void OnCardialRateSChanged();

        [DataMemberAttribute()]
        public double RateS
        {
            get
            {
                return _RateS;
            }
            set
            {
                if (_RateS != value)
                {
                    OnRateSChanging(value);
                    _RateS = value;
                    RaisePropertyChanged("RateS");
                    OnRateSChanged();
                }
            }
        }
        private double _RateS;
        partial void OnRateSChanging(double value);
        partial void OnRateSChanged();

        [DataMemberAttribute()]
        public double LN
        {
            get
            {
                return _LN;
            }
            set
            {
                if (_LN != value)
                {
                    OnLNChanging(value);
                    _LN = value;
                    RaisePropertyChanged("LN");
                    OnLNChanged();
                }
            }
        }
        private double _LN;
        partial void OnLNChanging(double value);
        partial void OnLNChanged();

        [DataMemberAttribute()]
        public string OrderRecord
        {
            get
            {
                return _OrderRecord;
            }
            set
            {
                if (_OrderRecord != value)
                {
                    OnOrderRecordChanging(value);
                    _OrderRecord = value;
                    RaisePropertyChanged("OrderRecord");
                    OnOrderRecordChanged();
                }
            }
        }
        private string _OrderRecord;
        partial void OnOrderRecordChanging(string value);
        partial void OnOrderRecordChanged();


        [DataMemberAttribute()]
        public long PCLImgResultID
        {
            get
            {
                return _PCLImgResultID;
            }
            set
            {
                if (_PCLImgResultID != value)
                {
                    OnPCLImgResultIDChanging(value);
                    _PCLImgResultID = value;
                    RaisePropertyChanged("PCLImgResultID");
                    OnPCLImgResultIDChanged();
                }
            }
        }
        private long _PCLImgResultID;
        partial void OnPCLImgResultIDChanging(long value);
        partial void OnPCLImgResultIDChanged();

        [DataMemberAttribute()]
        public long DoctorStaffID
        {
            get
            {
                return _DoctorStaffID;
            }
            set
            {
                if (_DoctorStaffID != value)
                {
                    OnDoctorStaffIDChanging(value);
                    _DoctorStaffID = value;
                    RaisePropertyChanged("CreateDate");
                    OnDoctorStaffIDChanged();
                }
            }
        }
        private long _DoctorStaffID;
        partial void OnDoctorStaffIDChanging(long value);
        partial void OnDoctorStaffIDChanged();

        [DataMemberAttribute()]
        public DateTime CreateDate
        {
            get
            {
                return _CreateDate;
            }
            set
            {
                if (_CreateDate != value)
                {
                    OnCreateDateChanging(value);
                    _CreateDate = value;
                    RaisePropertyChanged("CreateDate");
                    OnCreateDateChanged();
                }
            }
        }
        private DateTime _CreateDate;
        partial void OnCreateDateChanging(DateTime value);
        partial void OnCreateDateChanged();

        //==== 20161209 CMN Begin: Add PatientPCLReqID
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
        //==== 20161209 End.
        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public Staff VStaff
        {
            get
            {
                return _VStaff;
            }
            set
            {
                if (_VStaff != value)
                {
                    OnVStaffChanging(value);
                    _VStaff = value;
                    RaisePropertyChanged("VStaff");
                    OnVStaffChanged();
                }
            }
        }
        private Staff _VStaff;
        partial void OnVStaffChanging(Staff value);
        partial void OnVStaffChanged();
        #endregion

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        //==== 20161129 CMN Begin: Add button save for all pages
        [DataMemberAttribute()]
        public bool Tab_Update_Required
        {
            get
            {
                return this.UltraResParams_FetalEchocardiography2DID > 0;
            }
        }
        //==== 20161129 CMN End: Add button save for all pages
    }
}
