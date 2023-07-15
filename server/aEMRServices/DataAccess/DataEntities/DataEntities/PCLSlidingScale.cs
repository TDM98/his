using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
namespace DataEntities
{
    public partial class PCLSlidingScale : NotifyChangedBase
    {
        #region Factory Method

        /// Create a new PCLExamType object.
        /// <param name="pCLExamTypeID">Initial value of the PCLExamTypeID property.</param>
        /// <param name="pCLExamTypeName">Initial value of the PCLExamTypeName property.</param>
        public static PCLSlidingScale CreatePCLExamType(Int64 pCLSlidingScaleID, String measurementUnit)
        {
            PCLSlidingScale pCLSlidingScale = new PCLSlidingScale();
            pCLSlidingScale.PCLSlidingScaleID = pCLSlidingScaleID;
            pCLSlidingScale.MeasurementUnit= measurementUnit;
            return pCLSlidingScale;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 PCLSlidingScaleID
        {
            get
            {
                return _PCLSlidingScaleID;
            }
            set
            {
                if (_PCLSlidingScaleID != value)
                {
                    OnPCLSlidingScaleIDChanging(value);
                    _PCLSlidingScaleID = value;
                    RaisePropertyChanged("PCLSlidingScaleID");
                    OnPCLSlidingScaleIDChanged();
                }
            }
        }
        private Int64 _PCLSlidingScaleID;
        partial void OnPCLSlidingScaleIDChanging(Int64 value);
        partial void OnPCLSlidingScaleIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> PCLExamTypeID
        {
            get
            {
                return _PCLExamTypeID;
            }
            set
            {
                OnPCLExamTypeIDChanging(value);
                _PCLExamTypeID = value;
                RaisePropertyChanged("PCLExamTypeID");
                OnPCLExamTypeIDChanged();
            }
        }
        private Nullable<Int64> _PCLExamTypeID;
        partial void OnPCLExamTypeIDChanging(Nullable<Int64> value);
        partial void OnPCLExamTypeIDChanged();
        [DataMemberAttribute()]
        public Nullable<Double> PCLIndMinValue
        {
            get
            {
                return _PCLIndMinValue;
            }
            set
            {
                OnPCLIndMinValueChanging(value);
                ////ReportPropertyChanging("PCLIndMinValue");
                _PCLIndMinValue = value;
                RaisePropertyChanged("PCLIndMinValue");
                OnPCLIndMinValueChanged();
            }
        }
        private Nullable<Double> _PCLIndMinValue;
        partial void OnPCLIndMinValueChanging(Nullable<Double> value);
        partial void OnPCLIndMinValueChanged();

        [DataMemberAttribute()]
        public Nullable<Double> PCLIndMaxValue
        {
            get
            {
                return _PCLIndMaxValue;
            }
            set
            {
                OnPCLIndMaxValueChanging(value);
                ////ReportPropertyChanging("PCLIndMaxValue");
                _PCLIndMaxValue = value;
                RaisePropertyChanged("PCLIndMaxValue");
                OnPCLIndMaxValueChanged();
            }
        }
        private Nullable<Double> _PCLIndMaxValue;
        partial void OnPCLIndMaxValueChanging(Nullable<Double> value);
        partial void OnPCLIndMaxValueChanged();

        [DataMemberAttribute()]
        public Nullable<Double> PCLIndAVGValue
        {
            get
            {
                return _PCLIndAVGValue;
            }
            set
            {
                OnPCLIndAVGValueChanging(value);
                ////ReportPropertyChanging("PCLIndAVGValue");
                _PCLIndAVGValue = value;
                RaisePropertyChanged("PCLIndAVGValue");
                OnPCLIndAVGValueChanged();
            }
        }
        private Nullable<Double> _PCLIndAVGValue;
        partial void OnPCLIndAVGValueChanging(Nullable<Double> value);
        partial void OnPCLIndAVGValueChanged();
        
        [DataMemberAttribute()]
        public Nullable<Double> PCLIndDeclinationValue
        {
            get
            {
                return _PCLIndDeclinationValue;
            }
            set
            {
                OnPCLIndDeclinationValueChanging(value);
                ////ReportPropertyChanging("PCLIndAVGValue");
                _PCLIndDeclinationValue = value;
                RaisePropertyChanged("PCLIndDeclinationValue");
                OnPCLIndDeclinationValueChanged();
            }
        }
        private Nullable<Double> _PCLIndDeclinationValue;
        partial void OnPCLIndDeclinationValueChanging(Nullable<Double> value);
        partial void OnPCLIndDeclinationValueChanged();
        
        //
        [DataMemberAttribute()]
        public Nullable<Boolean> ClosedInterval
        {
            get
            {
                return _ClosedInterval;
            }
            set
            {
                OnClosedIntervalChanging(value);
                _ClosedInterval = value;
                RaisePropertyChanged("ClosedInterval");
                OnClosedIntervalChanged();
            }
        }
        private Nullable<Boolean> _ClosedInterval;
        partial void OnClosedIntervalChanging(Nullable<Boolean> value);
        partial void OnClosedIntervalChanged();


        [DataMemberAttribute()]
        public String PCLIndOtherValue
        {
            get
            {
                return _PCLIndOtherValue;
            }
            set
            {
                OnPCLIndOtherValueChanging(value);
                ////ReportPropertyChanging("PCLIndOtherValue");
                _PCLIndOtherValue = value;
                RaisePropertyChanged("PCLIndOtherValue");
                OnPCLIndOtherValueChanged();
            }
        }
        private String _PCLIndOtherValue;
        partial void OnPCLIndOtherValueChanging(String value);
        partial void OnPCLIndOtherValueChanged();

        [DataMemberAttribute()]
        public String MeasurementUnit
        {
            get
            {
                return _MeasurementUnit;
            }
            set
            {
                OnMeasurementUnitChanging(value);
                ////ReportPropertyChanging("MeasurementUnit");
                _MeasurementUnit = value;
                RaisePropertyChanged("MeasurementUnit");
                OnMeasurementUnitChanged();
            }
        }
        private String _MeasurementUnit;
        partial void OnMeasurementUnitChanging(String value);
        partial void OnMeasurementUnitChanged();


        [DataMemberAttribute()]
        public byte GenderType
        {
            get
            {
                return _GenderType;
            }
            set
            {
                OnGenderTypeChanging(value);
                _GenderType = value;
                RaisePropertyChanged("GenderType");
                OnGenderTypeChanged();
            }
        }
        private byte _GenderType;
        partial void OnGenderTypeChanging(byte value);
        partial void OnGenderTypeChanged();

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]        
        public PCLExamType PCLExamType
        {
            get;
            set;
        }

        #endregion
        public override bool Equals(object obj)
        {
            PCLSlidingScale SelectedSildingScale = obj as PCLSlidingScale;
            if (SelectedSildingScale == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PCLExamTypeID == SelectedSildingScale.PCLExamTypeID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
