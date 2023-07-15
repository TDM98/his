using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class UltraResParams_FetalEchocardiographyDoppler : NotifyChangedBase, IEditableObject
    {
        public UltraResParams_FetalEchocardiographyDoppler()
            : base()
        {

        }
        public override bool Equals(object obj)
        {
            UltraResParams_FetalEchocardiographyDoppler info = obj as UltraResParams_FetalEchocardiographyDoppler;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.UltraResParams_FetalEchocardiographyDopplerID > 0 && this.UltraResParams_FetalEchocardiographyDopplerID == info.UltraResParams_FetalEchocardiographyDopplerID;
        }
        private UltraResParams_FetalEchocardiographyDoppler _tempUltraResParams_FetalEchocardiographyDoppler;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempUltraResParams_FetalEchocardiographyDoppler = (UltraResParams_FetalEchocardiographyDoppler)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempUltraResParams_FetalEchocardiographyDoppler)
                CopyFrom(_tempUltraResParams_FetalEchocardiographyDoppler);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(UltraResParams_FetalEchocardiographyDoppler p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new UltraResParams_FetalEchocardiographyDoppler object.

        /// <param name="UltraResParams_FetalEchocardiographyDopplerID">Initial value of the UltraResParams_FetalEchocardiographyDopplerID property.</param>

        public static UltraResParams_FetalEchocardiographyDoppler CreateUltraResParams_FetalEchocardiographyDoppler(Byte UltraResParams_FetalEchocardiographyDopplerID, String UltraResParams_FetalEchocardiographyDopplerName)
        {
            UltraResParams_FetalEchocardiographyDoppler UltraResParams_FetalEchocardiographyDoppler = new UltraResParams_FetalEchocardiographyDoppler();
            UltraResParams_FetalEchocardiographyDoppler.UltraResParams_FetalEchocardiographyDopplerID = UltraResParams_FetalEchocardiographyDopplerID;

            return UltraResParams_FetalEchocardiographyDoppler;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long UltraResParams_FetalEchocardiographyDopplerID
        {
            get
            {
                return _UltraResParams_FetalEchocardiographyDopplerID;
            }
            set
            {
                if (_UltraResParams_FetalEchocardiographyDopplerID != value)
                {
                    OnUltraResParams_FetalEchocardiographyDopplerIDChanging(value);
                    _UltraResParams_FetalEchocardiographyDopplerID = value;
                    RaisePropertyChanged("UltraResParams_FetalEchocardiographyDopplerID");
                    OnUltraResParams_FetalEchocardiographyDopplerIDChanged();
                }
            }
        }
        private long _UltraResParams_FetalEchocardiographyDopplerID;
        partial void OnUltraResParams_FetalEchocardiographyDopplerIDChanging(long value);
        partial void OnUltraResParams_FetalEchocardiographyDopplerIDChanged();

        
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
                    RaisePropertyChanged("DoctorStaffID");
                    OnDoctorStaffIDChanged();
                }
            }
        }
        private long _DoctorStaffID;
        partial void OnDoctorStaffIDChanging(long value);
        partial void OnDoctorStaffIDChanged();

	    [DataMemberAttribute()]
        public double MitralValve_Vmax
        {
            get
            {
                return _MitralValve_Vmax;
            }
            set
            {
                if (_MitralValve_Vmax != value)
                {
                    OnMitralValve_VmaxChanging(value);
                    _MitralValve_Vmax = value;
                    RaisePropertyChanged("MitralValve_Vmax");
                    if (_MitralValve_Vmax>0)
                    {
                        MitralValve_Gdmax = (_MitralValve_Vmax / 100) * (_MitralValve_Vmax / 100) * 4;
                    }
                    OnMitralValve_VmaxChanged();
                }
            }
        }
        private double _MitralValve_Vmax;
        partial void OnMitralValve_VmaxChanging(double value);
        partial void OnMitralValve_VmaxChanged();

        [DataMemberAttribute()]
        public double MitralValve_Gdmax
        {
            get
            {
                return _MitralValve_Gdmax;
            }
            set
            {
                if (_MitralValve_Gdmax != value)
                {
                    OnMitralValve_GdmaxChanging(value);
                    _MitralValve_Gdmax = value;
                    RaisePropertyChanged("MitralValve_Gdmax");
                    OnMitralValve_GdmaxChanged();
                }
            }
        }
        private double _MitralValve_Gdmax;
        partial void OnMitralValve_GdmaxChanging(double value);
        partial void OnMitralValve_GdmaxChanged();

	    [DataMemberAttribute()]
        public long MitralValve_Open
        {
            get
            {
                return _MitralValve_Open;
            }
            set
            {
                if (_MitralValve_Open != value)
                {
                    OnMitralValve_OpenChanging(value);
                    _MitralValve_Open = value;
                    RaisePropertyChanged("MitralValve_Open");
                    OnMitralValve_OpenChanged();
                }
            }
        }
        private long _MitralValve_Open ;
        partial void OnMitralValve_OpenChanging(long value);
        partial void OnMitralValve_OpenChanged();

	    [DataMemberAttribute()]
        public bool MitralValve_Stenosis
        {
            get
            {
                return _MitralValve_Stenosis;
            }
            set
            {
                if (_MitralValve_Stenosis != value)
                {
                    OnMitralValve_StenosisChanging(value);
                    _MitralValve_Stenosis = value;
                    RaisePropertyChanged("MitralValve_Stenosis");
                    OnMitralValve_StenosisChanged();
                }
            }
        }
        private bool _MitralValve_Stenosis=false;
        partial void OnMitralValve_StenosisChanging(bool value);
        partial void OnMitralValve_StenosisChanged();

        [DataMemberAttribute()]
        public double TriscupidValve_Vmax
        {
            get
            {
                return _TriscupidValve_Vmax;
            }
            set
            {
                if (_TriscupidValve_Vmax != value)
                {
                    OnTriscupidValve_VmaxChanging(value);
                    _TriscupidValve_Vmax = value;
                    RaisePropertyChanged("TriscupidValve_Vmax");
                    if (_TriscupidValve_Vmax > 0)
                    {
                        TriscupidValve_Gdmax = (_TriscupidValve_Vmax / 100) * (_TriscupidValve_Vmax / 100) * 4;
                    }
                    OnTriscupidValve_VmaxChanged();
                }
            }
        }
        private double _TriscupidValve_Vmax;
        partial void OnTriscupidValve_VmaxChanging(double value);
        partial void OnTriscupidValve_VmaxChanged();

        [DataMemberAttribute()]
        public double TriscupidValve_Gdmax
        {
            get
            {
                return _TriscupidValve_Gdmax;
            }
            set
            {
                if (_TriscupidValve_Gdmax != value)
                {
                    OnTriscupidValve_GdmaxChanging(value);
                    _TriscupidValve_Gdmax = value;
                    RaisePropertyChanged("TriscupidValve_Gdmax");
                    OnTriscupidValve_GdmaxChanged();
                }
            }
        }
        private double _TriscupidValve_Gdmax;
        partial void OnTriscupidValve_GdmaxChanging(double value);
        partial void OnTriscupidValve_GdmaxChanged();

        [DataMemberAttribute()]
        public long TriscupidValve_Open
        {
            get
            {
                return _TriscupidValve_Open;
            }
            set
            {
                if (_TriscupidValve_Open != value)
                {
                    OnTriscupidValve_OpenChanging(value);
                    _TriscupidValve_Open = value;
                    RaisePropertyChanged("TriscupidValve_Open");
                    OnTriscupidValve_OpenChanged();
                }
            }
        }
        private long _TriscupidValve_Open;
        partial void OnTriscupidValve_OpenChanging(long value);
        partial void OnTriscupidValve_OpenChanged();

	
        [DataMemberAttribute()]
        public bool TriscupidValve_Stenosis
        {
            get
            {
                return _TriscupidValve_Stenosis;
            }
            set
            {
                if (_TriscupidValve_Stenosis != value)
                {
                    OnTriscupidValve_StenosisChanging(value);
                    _TriscupidValve_Stenosis = value;
                    RaisePropertyChanged("TriscupidValve_Stenosis");
                    OnTriscupidValve_StenosisChanged();
                }
            }
        }
        private bool _TriscupidValve_Stenosis=false;
        partial void OnTriscupidValve_StenosisChanging(bool value);
        partial void OnTriscupidValve_StenosisChanged();

        [DataMemberAttribute()]
        public double AorticValve_Vmax
        {
            get
            {
                return _AorticValve_Vmax;
            }
            set
            {
                if (_AorticValve_Vmax != value)
                {
                    OnAorticValve_VmaxChanging(value);
                    _AorticValve_Vmax = value;
                    RaisePropertyChanged("AorticValve_Vmax");
                    if (_AorticValve_Vmax > 0)
                    {
                        AorticValve_Gdmax = (_AorticValve_Vmax / 100) * (_AorticValve_Vmax / 100) * 4;
                    }
                    OnAorticValve_VmaxChanged();
                }
            }
        }
        private double _AorticValve_Vmax;
        partial void OnAorticValve_VmaxChanging(double value);
        partial void OnAorticValve_VmaxChanged();

        [DataMemberAttribute()]
        public double AorticValve_Gdmax
        {
            get
            {
                return _AorticValve_Gdmax;
            }
            set
            {
                if (_AorticValve_Gdmax != value)
                {
                    OnAorticValve_GdmaxChanging(value);
                    _AorticValve_Gdmax = value;
                    RaisePropertyChanged("AorticValve_Gdmax");
                    OnAorticValve_GdmaxChanged();
                }
            }
        }
        private double _AorticValve_Gdmax;
        partial void OnAorticValve_GdmaxChanging(double value);
        partial void OnAorticValve_GdmaxChanged();

        [DataMemberAttribute()]
        public long AorticValve_Open
        {
            get
            {
                return _AorticValve_Open;
            }
            set
            {
                if (_AorticValve_Open != value)
                {
                    OnAorticValve_OpenChanging(value);
                    _AorticValve_Open = value;
                    RaisePropertyChanged("AorticValve_Open");
                    OnAorticValve_OpenChanged();
                }
            }
        }
        private long _AorticValve_Open;
        partial void OnAorticValve_OpenChanging(long value);
        partial void OnAorticValve_OpenChanged();

	    [DataMemberAttribute()]
        public bool AorticValve_Stenosis
        {
            get
            {
                return _AorticValve_Stenosis;
            }
            set
            {
                if (_AorticValve_Stenosis != value)
                {
                    OnAorticValve_StenosisChanging(value);
                    _AorticValve_Stenosis = value;
                    RaisePropertyChanged("AorticValve_Stenosis");
                    OnAorticValve_StenosisChanged();
                }
            }
        }
        private bool _AorticValve_Stenosis;
        partial void OnAorticValve_StenosisChanging(bool value);
        partial void OnAorticValve_StenosisChanged();

	    [DataMemberAttribute()]
        public double PulmonaryValve_Vmax
        {
            get
            {
                return _PulmonaryValve_Vmax;
            }
            set
            {
                if (_PulmonaryValve_Vmax != value)
                {
                    OnPulmonaryValve_VmaxChanging(value);
                    _PulmonaryValve_Vmax = value;
                    RaisePropertyChanged("PulmonaryValve_Vmax");
                    if (_PulmonaryValve_Vmax > 0)
                    {
                        PulmonaryValve_Gdmax = (_PulmonaryValve_Vmax / 100) * (_PulmonaryValve_Vmax / 100) * 4;
                    }
                    OnPulmonaryValve_VmaxChanged();
                }
            }
        }
        private double _PulmonaryValve_Vmax;
        partial void OnPulmonaryValve_VmaxChanging(double value);
        partial void OnPulmonaryValve_VmaxChanged();

	
        [DataMemberAttribute()]
        public double PulmonaryValve_Gdmax
        {
            get
            {
                return _PulmonaryValve_Gdmax;
            }
            set
            {
                if (_PulmonaryValve_Gdmax != value)
                {
                    OnPulmonaryValve_GdmaxChanging(value);
                    _PulmonaryValve_Gdmax = value;
                    RaisePropertyChanged("PulmonaryValve_Gdmax");
                    OnPulmonaryValve_GdmaxChanged();
                }
            }
        }
        private double _PulmonaryValve_Gdmax;
        partial void OnPulmonaryValve_GdmaxChanging(double value);
        partial void OnPulmonaryValve_GdmaxChanged();

	
        [DataMemberAttribute()]
        public long PulmonaryValve_Open
        {
            get
            {
                return _PulmonaryValve_Open;
            }
            set
            {
                if (_PulmonaryValve_Open != value)
                {
                    OnPulmonaryValve_OpenChanging(value);
                    _PulmonaryValve_Open = value;
                    RaisePropertyChanged("PulmonaryValve_Open");
                    OnPulmonaryValve_OpenChanged();
                }
            }
        }
        private long _PulmonaryValve_Open;
        partial void OnPulmonaryValve_OpenChanging(long value);
        partial void OnPulmonaryValve_OpenChanged();

	    [DataMemberAttribute()]
        public bool PulmonaryValve_Stenosis
        {
            get
            {
                return _PulmonaryValve_Stenosis;
            }
            set
            {
                if (_PulmonaryValve_Stenosis != value)
                {
                    OnPulmonaryValve_StenosisChanging(value);
                    _PulmonaryValve_Stenosis = value;
                    RaisePropertyChanged("PulmonaryValve_Stenosis");
                    OnPulmonaryValve_StenosisChanged();
                }
            }
        }
        private bool _PulmonaryValve_Stenosis;
        partial void OnPulmonaryValve_StenosisChanging(bool value);
        partial void OnPulmonaryValve_StenosisChanged();

	
        [DataMemberAttribute()]
        public double AorticCoarctationBloodTraffic
        {
            get
            {
                return _AorticCoarctationBloodTraffic;
            }
            set
            {
                if (_AorticCoarctationBloodTraffic != value)
                {
                    OnAorticCoarctationBloodTrafficChanging(value);
                    _AorticCoarctationBloodTraffic = value;
                    RaisePropertyChanged("AorticCoarctationBloodTraffic");
                    OnAorticCoarctationBloodTrafficChanged();
                }
            }
        }
        private double _AorticCoarctationBloodTraffic;
        partial void OnAorticCoarctationBloodTrafficChanging(double value);
        partial void OnAorticCoarctationBloodTrafficChanged();

	
        [DataMemberAttribute()]
        public double VanViewessensBloodTraffic
        {
            get
            {
                return _VanViewessensBloodTraffic;
            }
            set
            {
                if (_VanViewessensBloodTraffic != value)
                {
                    OnVanViewessensBloodTrafficChanging(value);
                    _VanViewessensBloodTraffic = value;
                    RaisePropertyChanged("VanViewessensBloodTraffic");
                    OnVanViewessensBloodTrafficChanged();
                }
            }
        }
        private double _VanViewessensBloodTraffic;
        partial void OnVanViewessensBloodTrafficChanging(double value);
        partial void OnVanViewessensBloodTrafficChanged();

	
        [DataMemberAttribute()]
        public double DuctusAteriosusBloodTraffic
        {
            get
            {
                return _DuctusAteriosusBloodTraffic;
            }
            set
            {
                if (_DuctusAteriosusBloodTraffic != value)
                {
                    OnDuctusAteriosusBloodTrafficChanging(value);
                    _DuctusAteriosusBloodTraffic = value;
                    RaisePropertyChanged("DuctusAteriosusBloodTraffic");
                    OnDuctusAteriosusBloodTrafficChanged();
                }
            }
        }
        private double _DuctusAteriosusBloodTraffic;
        partial void OnDuctusAteriosusBloodTrafficChanging(double value);
        partial void OnDuctusAteriosusBloodTrafficChanged();

	    [DataMemberAttribute()]
        public double DuctusVenosusBloodTraffic
        {
            get
            {
                return _DuctusVenosusBloodTraffic;
            }
            set
            {
                if (_DuctusVenosusBloodTraffic != value)
                {
                    OnDuctusVenosusBloodTrafficChanging(value);
                    _DuctusVenosusBloodTraffic = value;
                    RaisePropertyChanged("DuctusVenosusBloodTraffic");
                    OnDuctusVenosusBloodTrafficChanged();
                }
            }
        }
        private double _DuctusVenosusBloodTraffic;
        partial void OnDuctusVenosusBloodTrafficChanging(double value);
        partial void OnDuctusVenosusBloodTrafficChanged();

	
        [DataMemberAttribute()]
        public bool PulmonaryVeins_LeftAtrium
        {
            get
            {
                return _PulmonaryVeins_LeftAtrium;
            }
            set
            {
                if (_PulmonaryVeins_LeftAtrium != value)
                {
                    OnPulmonaryVeins_LeftAtriumChanging(value);
                    _PulmonaryVeins_LeftAtrium = value;
                    RaisePropertyChanged("PulmonaryVeins_LeftAtrium");
                    OnPulmonaryVeins_LeftAtriumChanged();
                }
            }
        }
        private bool _PulmonaryVeins_LeftAtrium=true;
        partial void OnPulmonaryVeins_LeftAtriumChanging(bool value);
        partial void OnPulmonaryVeins_LeftAtriumChanged();

	
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


        [DataMemberAttribute()]
        public Lookup VTriscupidValve_Open
        {
            get
            {
                return _VTriscupidValve_Open;
            }
            set
            {
                if (_VTriscupidValve_Open != value)
                {
                    OnVTriscupidValve_OpenChanging(value);
                    _VTriscupidValve_Open = value;
                    RaisePropertyChanged("VTriscupidValve_Open");
                    TriscupidValve_Open = VTriscupidValve_Open.LookupID;
                    OnVTriscupidValve_OpenChanged();
                }
            }
        }
        private Lookup _VTriscupidValve_Open;
        partial void OnVTriscupidValve_OpenChanging(Lookup value);
        partial void OnVTriscupidValve_OpenChanged();

        [DataMemberAttribute()]
        public Lookup VMitralValve_Open
        {
            get
            {
                return _VMitralValve_Open;
            }
            set
            {
                if (_VMitralValve_Open != value)
                {
                    OnVMitralValve_OpenChanging(value);
                    _VMitralValve_Open = value;
                    RaisePropertyChanged("VMitralValve_Open");
                    MitralValve_Open = VMitralValve_Open.LookupID;
                    OnVMitralValve_OpenChanged();
                }
            }
        }
        private Lookup _VMitralValve_Open;
        partial void OnVMitralValve_OpenChanging(Lookup value);
        partial void OnVMitralValve_OpenChanged();


        [DataMemberAttribute()]
        public Lookup VAorticValve_Open
        {
            get
            {
                return _VAorticValve_Open;
            }
            set
            {
                if (_VAorticValve_Open != value)
                {
                    OnVAorticValve_OpenChanging(value);
                    _VAorticValve_Open = value;
                    RaisePropertyChanged("VAorticValve_Open");
                    AorticValve_Open = VAorticValve_Open.LookupID;
                    OnVAorticValve_OpenChanged();
                }
            }
        }
        private Lookup _VAorticValve_Open;
        partial void OnVAorticValve_OpenChanging(Lookup value);
        partial void OnVAorticValve_OpenChanged();


        [DataMemberAttribute()]
        public Lookup VPulmonaryValve_Open
        {
            get
            {
                return _VPulmonaryValve_Open;
            }
            set
            {
                if (_VPulmonaryValve_Open != value)
                {
                    OnVPulmonaryValve_OpenChanging(value);
                    _VPulmonaryValve_Open = value;
                    RaisePropertyChanged("VPulmonaryValve_Open");
                    PulmonaryValve_Open = VPulmonaryValve_Open.LookupID;
                    OnVPulmonaryValve_OpenChanged();
                }
            }
        }
        private Lookup _VPulmonaryValve_Open;
        partial void OnVPulmonaryValve_OpenChanging(Lookup value);
        partial void OnVPulmonaryValve_OpenChanged();

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
                return this.UltraResParams_FetalEchocardiographyDopplerID > 0;
            }
        }
        //==== 20161129 CMN End: Add button save for all pages
    }
}
