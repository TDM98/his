using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

/*
 * 20230520 #001 DatTB: Thêm cột tên tiếng anh các danh mục khoa phòng
*/
namespace DataEntities
{
    public partial class RefDepartments:RefDepartmentsTree
    {
        public RefDepartments()
            : base()
        {

        }

        //private RefDepartments _tempRefDepartments;
        //#region IEditableObject Members

        //public void BeginEdit()
        //{
        //    _tempRefDepartments = (RefDepartments)this.MemberwiseClone();
        //}

        //public void CancelEdit()
        //{
        //    if (null != _tempRefDepartments)
        //        CopyFrom(_tempRefDepartments);
        //    //_tempPatient = null;
        //}

        //public void EndEdit()
        //{
        //}

        //public void CopyFrom(RefDepartments p)
        //{
        //    PropertyCopierHelper.CopyPropertiesTo(p, this);
        //}

        //#endregion
        #region Factory Method
        public static RefDepartments CreateRefDepartments(long pDeptID, long pParDeptID,long pV_DeptType, string pDeptName, string pDeptDescription)
        {
            RefDepartments objRefDepartments = new RefDepartments();
            objRefDepartments.DeptID = pDeptID;
            objRefDepartments.ParDeptID = pParDeptID;
            objRefDepartments.V_DeptType = pV_DeptType;
            objRefDepartments.DeptName = pDeptName;
            objRefDepartments.DeptDescription = pDeptDescription;
            return objRefDepartments;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long DeptID
        {
            get
            {
                return _DeptID;
            }
            set
            {
                if (_DeptID != value)
                {
                    OnDeptIDChanging(value);
                    _DeptID = value;
                    RaisePropertyChanged("DeptID");
                    OnDeptIDChanged();
                }
            }
        }
        private long _DeptID;
        partial void OnDeptIDChanging(long value);
        partial void OnDeptIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> ParDeptID
        {
            get
            {
                return _ParDeptID;
            }
            set
            {
                OnParDeptIDChanging(value);
                _ParDeptID = value;
                RaisePropertyChanged("ParDeptID");
                OnParDeptIDChanged();
            }
        }
        private Nullable<Int64> _ParDeptID;
        partial void OnParDeptIDChanging(Nullable<Int64> value);
        partial void OnParDeptIDChanged();

        [DataMemberAttribute()]
        public new long? V_DeptTypeOperation
        {
            get
            {
                return _V_DeptTypeOperation;
            }
            set
            {
                OnV_DeptTypeOperationChanging(value);
                _V_DeptTypeOperation = value;
                RaisePropertyChanged("V_DeptTypeOperation");
                OnV_DeptTypeOperationChanged();
            }
        }
        private long? _V_DeptTypeOperation;
        partial void OnV_DeptTypeOperationChanging(long? value);
        partial void OnV_DeptTypeOperationChanged();


        [Required(ErrorMessage = "Nhập Tên!")]
        [StringLength(125, ErrorMessage = "Nhập Tên! Phải <= 125 Ký Tự")]
        [DataMemberAttribute()]
        public String DeptName
        {
            get
            {
                return _DeptName;
            }
            set
            {
                OnDeptNameChanging(value);
                ValidateProperty("DeptName", value);
                _DeptName = value;
                RaisePropertyChanged("DeptName");
                OnDeptNameChanged();
            }
        }
        private String _DeptName;
        partial void OnDeptNameChanging(String value);
        partial void OnDeptNameChanged();

        //▼==== #001
        [DataMemberAttribute()]
        public String DeptNameEng
        {
            get
            {
                return _DeptNameEng;
            }
            set
            {
                OnDeptNameEngChanging(value);
                ValidateProperty("DeptNameEng", value);
                _DeptNameEng = value;
                RaisePropertyChanged("DeptNameEng");
                OnDeptNameEngChanged();
            }
        }
        private String _DeptNameEng;
        partial void OnDeptNameEngChanging(String value);
        partial void OnDeptNameEngChanged();
        //▲==== #001

        [DataMemberAttribute()]
        public String DeptDescription
        {
            get
            {
                return _DeptDescription;
            }
            set
            {
                OnDeptDescriptionChanging(value);
                _DeptDescription = value;
                RaisePropertyChanged("DeptDescription");
                OnDeptDescriptionChanged();
            }
        }
        private String _DeptDescription;
        partial void OnDeptDescriptionChanging(String value);
        partial void OnDeptDescriptionChanged();

        [DataMemberAttribute()]
        public bool IsDeleted
        {
            get { return _IsDeleted; }
            set
            {
                _IsDeleted = value;
                RaisePropertyChanged("IsDeleted");
            }
        }
        private bool _IsDeleted = false;
        
        // TxD 09/05/2015: Added the following attribute to indicate that if a department is selected then you must also select a room within that department.
        //                 Initially this flag is used for creating In-Patient Bill of the USIC-CC department
        [DataMemberAttribute()]
        public bool SelectDeptReqSelectRoom
        {
            get { return _SelectDeptReqSelectRoom; }
            set
            {
                _SelectDeptReqSelectRoom = value;
                RaisePropertyChanged("SelectDeptReqSelectRoom");
            }
        }
        private bool _SelectDeptReqSelectRoom = false;

        [DataMemberAttribute()]
        public string DeptCode
        {
            get { return _DeptCode; }
            set
            {
                _DeptCode = value;
                RaisePropertyChanged("DeptCode");
            }
        }
        private string _DeptCode;
        #endregion

        public override bool Equals(object obj)
        {
            RefDepartments info = obj as RefDepartments;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.DeptID > 0 && this.DeptID == info.DeptID;
        }

        public override int GetHashCode()
        {
            return this.DeptID.GetHashCode();
        }
    }
}
