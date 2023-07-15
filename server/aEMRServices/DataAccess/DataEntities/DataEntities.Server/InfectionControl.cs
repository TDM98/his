using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

/*
 * 20230318 #001 DatTB: Thêm biến lưu theo lần nhập viện
 */
namespace DataEntities
{
    public partial class InfectionControl : EntityBase
    {
        #region Primitive Properties
        [DataMemberAttribute()]
        public long InfectionControlID
        {
            get
            {
                return _InfectionControlID;
            }
            set
            {
                if (_InfectionControlID == value)
                {
                    return;
                }
                _InfectionControlID = value;
                RaisePropertyChanged("InfectionControlID");
            }
        }
        private long _InfectionControlID;

        [DataMemberAttribute()]
        public long PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                if (_PatientID == value)
                {
                    return;
                }
                _PatientID = value;
                RaisePropertyChanged("PatientID");
            }
        }
        private long _PatientID;
        
        [DataMemberAttribute]
        public bool IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                if (_IsDeleted == value)
                {
                    return;
                }
                _IsDeleted = value;
                RaisePropertyChanged("IsDeleted");
            }
        }
        private bool _IsDeleted;

        [DataMemberAttribute]
        public long StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                if (_StaffID == value)
                {
                    return;
                }
                _StaffID = value;
                RaisePropertyChanged("StaffID");
            }
        }
        private long _StaffID;

        [DataMemberAttribute]
        public long CreatedStaffID
        {
            get
            {
                return _CreatedStaffID;
            }
            set
            {
                if (_CreatedStaffID == value)
                {
                    return;
                }
                _CreatedStaffID = value;
                RaisePropertyChanged("CreatedStaffID");
            }
        }
        private long _CreatedStaffID;

        [DataMemberAttribute]
        public DateTime CreatedDate
        {
            get
            {
                return _CreatedDate;
            }
            set
            {
                if (_CreatedDate == value)
                {
                    return;
                }
                _CreatedDate = value;
                RaisePropertyChanged("CreatedDate");
            }
        }
        private DateTime _CreatedDate;

        [DataMemberAttribute]
        public long LastUpdateStaffID
        {
            get
            {
                return _LastUpdateStaffID;
            }
            set
            {
                if (_LastUpdateStaffID == value)
                {
                    return;
                }
                _LastUpdateStaffID = value;
                RaisePropertyChanged("LastUpdateStaffID");
            }
        }
        private long _LastUpdateStaffID;

        [DataMemberAttribute]
        public DateTime LastUpdateDate
        {
            get
            {
                return _LastUpdateDate;
            }
            set
            {
                if (_LastUpdateDate == value)
                {
                    return;
                }
                _LastUpdateDate = value;
                RaisePropertyChanged("LastUpdateDate");
            }
        }
        private DateTime _LastUpdateDate;

        [DataMemberAttribute]
        public int BacteriaType
        {
            get
            {
                return _BacteriaType;
            }
            set
            {
                if (_BacteriaType == value)
                {
                    return;
                }
                _BacteriaType = value;
                RaisePropertyChanged("BacteriaType");
            }
        }
        private int _BacteriaType;

        [DataMemberAttribute]
        public DateTime? DefiniteDate
        {
            get
            {
                return _DefiniteDate;
            }
            set
            {
                if (_DefiniteDate == value)
                {
                    return;
                }
                _DefiniteDate = value;
                RaisePropertyChanged("DefiniteDate");
            }
        }
        private DateTime? _DefiniteDate;

        [DataMemberAttribute]
        public string BacteriaName
        {
            get
            {
                return _BacteriaName;
            }
            set
            {
                if (_BacteriaName == value)
                {
                    return;
                }
                _BacteriaName = value;
                RaisePropertyChanged("BacteriaName");
            }
        }
        private string _BacteriaName;

        [DataMemberAttribute]
        public long V_Bacteria_LOT
        {
            get
            {
                return _V_Bacteria_LOT;
            }
            set
            {
                if (_V_Bacteria_LOT == value)
                {
                    return;
                }
                _V_Bacteria_LOT = value;
                RaisePropertyChanged("V_Bacteria_LOT");
            }
        }
        private long _V_Bacteria_LOT;
               
        [DataMemberAttribute]
        public string Bacteria_LOT_Str
        {
            get
            {
                return _Bacteria_LOT_Str;
            }
            set
            {
                if (_Bacteria_LOT_Str == value)
                {
                    return;
                }
                _Bacteria_LOT_Str = value;
                RaisePropertyChanged("Bacteria_LOT_Str");
            }
        }
        private string _Bacteria_LOT_Str;

        [DataMemberAttribute]
        public string BacteriaMeasure
        {
            get
            {
                return _BacteriaMeasure;
            }
            set
            {
                if (_BacteriaMeasure == value)
                {
                    return;
                }
                _BacteriaMeasure = value;
                RaisePropertyChanged("BacteriaMeasure");
            }
        }
        private string _BacteriaMeasure;

        //▼==== #001

        [DataMemberAttribute]
        public long InPatientAdmDisDetailID
        {
            get
            {
                return _InPatientAdmDisDetailID;
            }
            set
            {
                if (_InPatientAdmDisDetailID == value)
                {
                    return;
                }
                _InPatientAdmDisDetailID = value;
                RaisePropertyChanged("InPatientAdmDisDetailID");
            }
        }
        private long _InPatientAdmDisDetailID;

        [DataMemberAttribute]
        private bool _isCurAdmisIC;
        public bool isCurAdmisIC
        {
            get
            {
                return _isCurAdmisIC;
            }
            set
            {
                if (_isCurAdmisIC == value)
                {
                    return;
                }
                _isCurAdmisIC = value;
                RaisePropertyChanged("isCurAdmisIC");
            }
        }
        //▲==== #001
        #endregion
    }
}
