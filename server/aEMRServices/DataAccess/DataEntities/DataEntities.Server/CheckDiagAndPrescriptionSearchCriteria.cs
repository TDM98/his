using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
/*
 * 20220407 #001 DatTB: Thêm điều kiện tìm kiếm BN Chưa duyệt toa Bảo hiểm @IsConfirmHI
 */
namespace DataEntities
{
    public partial class CheckDiagAndPrescriptionSearchCriteria: NotifyChangedBase
    {
        #region Primitive Properties
        [DataMemberAttribute()]
        public long DoctorID
        {
            get
            {
                return _DoctorID;
            }
            set
            {
                _DoctorID = value;
                RaisePropertyChanged("DoctorID");
            }
        }
        private long _DoctorID;

        [DataMemberAttribute()]
        public long DeptLocID
        {
            get
            {
                return _DeptLocID;
            }
            set
            {
                _DeptLocID = value;
                RaisePropertyChanged("DeptLocID");
            }
        }
        private long _DeptLocID;

        [DataMemberAttribute()]
        public string PatientCode
        {
            get
            {
                return _PatientCode;
            }
            set
            {
                _PatientCode = value;
                RaisePropertyChanged("PatientCode");
            }
        }
        private string _PatientCode;

        [DataMemberAttribute()]
        public string FullName
        {
            get
            {
                return _FullName;
            }
            set
            {
                _FullName = value;
                RaisePropertyChanged("FullName");
            }
        }
        private string _FullName;


        [DataMemberAttribute()]
        public DateTime FromDate
        {
            get
            {
                return _FromDate;
            }
            set
            {
                _FromDate = value;
                RaisePropertyChanged("FromDate");
            }
        }
        private DateTime _FromDate;


        [DataMemberAttribute()]
        public DateTime ToDate
        {
            get
            {
                return _ToDate;
            }
            set
            {
                _ToDate = value;
                RaisePropertyChanged("ToDate");
            }
        }
        private DateTime _ToDate;

        [DataMemberAttribute()]
        public long V_CheckMedicalFilesStatusForDLS
        {
            get
            {
                return _V_CheckMedicalFilesStatusForDLS;
            }
            set
            {
                _V_CheckMedicalFilesStatusForDLS = value;
                RaisePropertyChanged("V_CheckMedicalFilesStatusForDLS");
            }
        }
        private long _V_CheckMedicalFilesStatusForDLS;

        //▼====: #001
        [DataMemberAttribute()]
        public bool IsConfirmHI
        {
            get
            {
                return _IsConfirmHI;
            }
            set
            {
                _IsConfirmHI = value;
                RaisePropertyChanged("IsConfirmHI");
            }
        }
        private bool _IsConfirmHI;
        //▲====: #001
        #endregion

    }
}
