using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

/*
 * 20220611 #001 DatTB: Thêm biến IsUnlocked cho phép user đang thao tác trả hồ sơ
 * 20220712 #002 DatTB: Sửa lịch sử kiểm duyệt theo quy trình mới
 */
namespace DataEntities
{
    public partial class CheckMedicalFiles: NotifyChangedBase
    {
        #region Primitive Properties
        [DataMemberAttribute()]
        public long CheckMedicalFileID
        {
            get
            {
                return _CheckMedicalFileID;
            }
            set
            {
                if (_CheckMedicalFileID != value)
                {
                    OnCheckMedicalFileIDChanging(value);
                    _CheckMedicalFileID = value;
                    RaisePropertyChanged("CheckMedicalFileID");
                    OnCheckMedicalFileIDChanged();
                }
            }
        }
        private long _CheckMedicalFileID;
        partial void OnCheckMedicalFileIDChanging(long value);
        partial void OnCheckMedicalFileIDChanged();

        [DataMemberAttribute()]
        public long PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                OnPtRegistrationIDChanging(value);
                _PtRegistrationID = value;
                RaisePropertyChanged("PtRegistrationID");
                OnPtRegistrationIDChanged();
            }
        }
        private long _PtRegistrationID;
        partial void OnPtRegistrationIDChanging(long value);
        partial void OnPtRegistrationIDChanged();

        [DataMemberAttribute()]
        public string Check_KHTH
        {
            get
            {
                return _Check_KHTH;
            }
            set
            {
                OnCheck_KHTHChanging(value);
                _Check_KHTH = value;
                RaisePropertyChanged("Check_KHTH");
                OnCheck_KHTHChanged();
            }
        }
        private string _Check_KHTH;
        partial void OnCheck_KHTHChanging(string value);
        partial void OnCheck_KHTHChanged();

        [DataMemberAttribute()]
        public string Check_DLS
        {
            get
            {
                return _Check_DLS;
            }
            set
            {
                OnCheck_DLSChanging(value);
                _Check_DLS = value;
                RaisePropertyChanged("Check_DLS");
                OnCheck_DLSChanged();
            }
        }
        private string _Check_DLS;
        partial void OnCheck_DLSChanging(string value);
        partial void OnCheck_DLSChanged();

        [DataMemberAttribute()]
        public string Note_DLS
        {
            get
            {
                return _Note_DLS;
            }
            set
            {
                OnNote_DLSChanging(value);
                _Note_DLS = value;
                RaisePropertyChanged("Note_DLS");
                OnNote_DLSChanged();
            }
        }
        private string _Note_DLS;
        partial void OnNote_DLSChanging(string value);
        partial void OnNote_DLSChanged();

        [DataMemberAttribute()]
        public DateTime DateCreated
        {
            get
            {
                return _DateCreated;
            }
            set
            {
                OnDateCreatedChanging(value);
                _DateCreated = value;
                RaisePropertyChanged("DateCreated");
                OnDateCreatedChanged();
            }
        }
        private DateTime _DateCreated;
        partial void OnDateCreatedChanging(DateTime value);
        partial void OnDateCreatedChanged();

        [DataMemberAttribute()]
        public long V_CheckMedicalFilesStatus
        {
            get
            {
                return _V_CheckMedicalFilesStatus;
            }
            set
            {
                OnV_CheckMedicalFilesStatusChanging(value);
                _V_CheckMedicalFilesStatus = value;
                RaisePropertyChanged("V_CheckMedicalFilesStatus");
                OnV_CheckMedicalFilesStatusChanged();
            }
        }
        private long _V_CheckMedicalFilesStatus;
        partial void OnV_CheckMedicalFilesStatusChanging(long value);
        partial void OnV_CheckMedicalFilesStatusChanged();
        [DataMemberAttribute()]
        public bool IsDLSChecked
        {
            get
            {
                return _IsDLSChecked;
            }
            set
            {
                OnIsDLSCheckedChanging(value);
                _IsDLSChecked = value;
                RaisePropertyChanged("IsDLSChecked");
                OnIsDLSCheckedChanged();
            }
        }
        private bool _IsDLSChecked;
        partial void OnIsDLSCheckedChanging(bool value);
        partial void OnIsDLSCheckedChanged();
        [DataMemberAttribute()]
        public Staff CreatedStaff
        {
            get
            {
                return _CreatedStaff;
            }
            set
            {
                _CreatedStaff = value;
                RaisePropertyChanged("CreatedStaff");
            }
        }
        private Staff _CreatedStaff;
        [DataMemberAttribute()]
        public long SendRequestStaffID
        {
            get
            {
                return _SendRequestStaffID;
            }
            set
            {
                _SendRequestStaffID = value;
                RaisePropertyChanged("SendRequestStaffID");
            }
        }
        private long _SendRequestStaffID;
        [DataMemberAttribute()]
        public Staff SendRequestStaff
        {
            get
            {
                return _SendRequestStaff;
            }
            set
            {
                _SendRequestStaff = value;
                RaisePropertyChanged("SendRequestStaff");
            }
        }
        private Staff _SendRequestStaff;
        [DataMemberAttribute()]
        public DateTime? DateSendRequest
        {
            get
            {
                return _DateSendRequest;
            }
            set
            {
                _DateSendRequest = value;
                RaisePropertyChanged("DateSendRequest");
            }
        }
        private DateTime? _DateSendRequest;
        [DataMemberAttribute()]
        public bool DLSReject
        {
            get
            {
                return _DLSReject;
            }
            set
            {
                _DLSReject = value;
                RaisePropertyChanged("DLSReject");
            }
        }
        private bool _DLSReject;
        #endregion
        [DataMemberAttribute()]
        public CheckMedicalFiles CheckMedicalFile
        {
            get;
            set;
        }

        //--▼-- 20220329 DatTB: Lấy thêm thông tin lịch sử kiểm duyệt hồ sơ
        [DataMemberAttribute()]
        public long KHTH_StaffID
        {
            get
            {
                return _KHTH_StaffID;
            }
            set
            {
                _KHTH_StaffID = value;
                RaisePropertyChanged("KHTH_StaffID");
            }
        }
        private long _KHTH_StaffID;

        [DataMemberAttribute()]
        public Staff KHTH_Staff
        {
            get
            {
                return _KHTH_Staff;
            }
            set
            {
                _KHTH_Staff = value;
                RaisePropertyChanged("KHTH_Staff");
            }
        }
        private Staff _KHTH_Staff;

        [DataMemberAttribute()]
        public long RejectStaffID
        {
            get
            {
                return _RejectStaffID;
            }
            set
            {
                _RejectStaffID = value;
                RaisePropertyChanged("RejectStaffID");
            }
        }
        private long _RejectStaffID;

        [DataMemberAttribute()]
        public Staff RejectStaff
        {
            get
            {
                return _RejectStaff;
            }
            set
            {
                _RejectStaff = value;
                RaisePropertyChanged("RejectStaff");
            }
        }
        private Staff _RejectStaff;

        [DataMemberAttribute()]
        public DateTime? DLS_CheckDate
        {
            get
            {
                return _DLS_CheckDate;
            }
            set
            {
                _DLS_CheckDate = value;
                RaisePropertyChanged("DLS_CheckDate");
            }
        }
        private DateTime? _DLS_CheckDate;

        [DataMemberAttribute()]
        public DateTime? KHTH_CheckDate
        {
            get
            {
                return _KHTH_CheckDate;
            }
            set
            {
                _KHTH_CheckDate = value;
                RaisePropertyChanged("KHTH_CheckDate");
            }
        }
        private DateTime? _KHTH_CheckDate;

        [DataMemberAttribute()]
        public DateTime? RejectDate
        {
            get
            {
                return _RejectDate;
            }
            set
            {
                _RejectDate = value;
                RaisePropertyChanged("RejectDate");
            }
        }
        private DateTime? _RejectDate;
        //--▲-- 20220329 DatTB: Lấy thêm thông tin lịch sử kiểm duyệt hồ sơ
        //▼==== #001
        private bool _IsUnlocked = false;
        [DataMemberAttribute()]
        public bool IsUnlocked
        {
            get { return _IsUnlocked; }
            set
            {
                _IsUnlocked = value;
                RaisePropertyChanged("IsUnlocked");
            }
        }
        //▲==== #001

        //▼==== #002
        [DataMemberAttribute()]
        public long DLS_RejectStaffID
        {
            get
            {
                return _DLS_RejectStaffID;
            }
            set
            {
                _DLS_RejectStaffID = value;
                RaisePropertyChanged("DLS_RejectStaffID");
            }
        }
        private long _DLS_RejectStaffID;

        [DataMemberAttribute()]
        public Staff DLS_RejectStaff
        {
            get
            {
                return _DLS_RejectStaff;
            }
            set
            {
                _DLS_RejectStaff = value;
                RaisePropertyChanged("DLS_RejectStaff");
            }
        }
        private Staff _DLS_RejectStaff;

        [DataMemberAttribute()]
        public long KHTH_RejectStaffID
        {
            get
            {
                return _KHTH_RejectStaffID;
            }
            set
            {
                _KHTH_RejectStaffID = value;
                RaisePropertyChanged("KHTH_RejectStaffID");
            }
        }
        private long _KHTH_RejectStaffID;

        [DataMemberAttribute()]
        public Staff KHTH_RejectStaff
        {
            get
            {
                return _KHTH_RejectStaff;
            }
            set
            {
                _KHTH_RejectStaff = value;
                RaisePropertyChanged("KHTH_RejectStaff");
            }
        }
        private Staff _KHTH_RejectStaff;

        [DataMemberAttribute()]
        public DateTime? KHTH_RejectDate
        {
            get
            {
                return _KHTH_RejectDate;
            }
            set
            {
                _KHTH_RejectDate = value;
                RaisePropertyChanged("KHTH_RejectDate");
            }
        }
        private DateTime? _KHTH_RejectDate;

        [DataMemberAttribute()]
        public string RejectDateStr
        {
            get
            {
                return _RejectDateStr;
            }
            set
            {
                _RejectDateStr = value;
                RaisePropertyChanged("RejectDateStr");
            }
        }
        private string _RejectDateStr;

        [DataMemberAttribute()]
        public string RejectStaffStr
        {
            get
            {
                return _RejectStaffStr;
            }
            set
            {
                _RejectStaffStr = value;
                RaisePropertyChanged("RejectStaffStr");
            }
        }
        private string _RejectStaffStr;
        //▲==== #002
    }
}
