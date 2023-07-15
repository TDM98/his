using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
namespace DataEntities
{
    public partial class Holiday : NotifyChangedBase, IEditableObject
    {
        public Holiday()
            : base()
        {

        }

        private Holiday _tempHoliday;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempHoliday = (Holiday)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempHoliday)
                CopyFrom(_tempHoliday);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(Holiday p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new Holiday object.

        /// <param name="hLDID">Initial value of the HLDID property.</param>
        /// <param name="hLYear">Initial value of the HLYear property.</param>
        /// <param name="hLDate">Initial value of the HLDate property.</param>
        /// <param name="hLName">Initial value of the HLName property.</param>
        public static Holiday CreateHoliday(long hLDID, Int16 hLYear, Int16 hLMonth, Int16 hLDate, String hLName)
        {
            Holiday holiday = new Holiday();
            holiday.HLDID = hLDID;
            holiday.HLYear = hLYear;
            holiday.HLMonth = hLMonth;
            holiday.HLDate = hLDate;
            holiday.HLName = hLName;
            return holiday;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long HLDID
        {
            get
            {
                return _HLDID;
            }
            set
            {
                if (_HLDID != value)
                {
                    _HLDID = value;
                    RaisePropertyChanged("HLDID");
                }
            }
        }
        private long _HLDID;


        [DataMemberAttribute()]
        public Int16 HLYear
        {
            get
            {
                return _HLYear;
            }
            set
            {
                _HLYear = value;
                RaisePropertyChanged("HLYear");
            }
        }
        private Int16 _HLYear;

        [DataMemberAttribute()]
        public Int16 HLMonth
        {
            get
            {
                return _HLMonth;
            }
            set
            {
                _HLMonth = value;
                RaisePropertyChanged("HLMonth");
            }
        }
        private Int16 _HLMonth;


        [DataMemberAttribute()]
        public Int16 HLDate
        {
            get
            {
                return _HLDate;
            }
            set
            {
                _HLDate = value;
                RaisePropertyChanged("HLDate");
            }
        }
        private Int16 _HLDate;


        [DataMemberAttribute()]
        public String HLName
        {
            get
            {
                return _HLName;
            }
            set
            {
                _HLName = value;
                RaisePropertyChanged("HLName");
            }
        }
        private String _HLName;
        #endregion

    }
}
