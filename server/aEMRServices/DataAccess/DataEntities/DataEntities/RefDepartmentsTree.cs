using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

/*
 * 20230520 #001 DatTB: Thêm cột tên tiếng anh các danh mục khoa phòng
*/
namespace DataEntities
{
    [KnownType(typeof(RefDepartments))]
     //[DataContract(IsReference=true)]
    public class RefDepartmentsTree : NotifyChangedBase, IEditableObject
    {
         public RefDepartmentsTree()
             : base()
         {

         }

        private RefDepartmentsTree _tempAccidentdrug;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempAccidentdrug = (RefDepartmentsTree)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempAccidentdrug)
                CopyFrom(_tempAccidentdrug);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(RefDepartmentsTree p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }
        #endregion

        private string nodeText;
        private long nodeID;
        private System.Nullable<long> parentID;        
        private List<RefDepartmentsTree> children;
        private List<DeptLocation> childrens;
        private string description;
        private int level=0;
        private bool isDeptLocation = false;

        private List<ConsultationRoomTarget> lstConsultationRoomTarget;
        private List<ConsultationRoomStaffAllocations> lstConsultationRoomStaffAllocations;
  

        private RefDepartmentsTree parent;

        //▼==== #001
        public RefDepartmentsTree(string nodeText, long nodeID, System.Nullable<long> parentID, Nullable<long> V_DeptType, Nullable<long> V_DeptTypeOperation, string description, RefDepartmentsTree parent)
        {
            this.nodeText = nodeText;
            this.nodeID = nodeID;
            this.parentID = parentID;
            this._V_DeptType = V_DeptType;
            this._V_DeptTypeOperation = V_DeptTypeOperation;
            this.description = description;
            this.children = new List<RefDepartmentsTree>();
            this.parent = parent;
        }
        //▲==== #001

        public RefDepartmentsTree(string nodeText, long nodeID, System.Nullable<long> parentID, Nullable<long> V_DeptType,Nullable<long> V_DeptTypeOperation,  string description, RefDepartmentsTree parent, string DeptNameEng)
        {
            this.nodeText = nodeText;
            this.nodeID = nodeID;
            this.parentID = parentID;
            this._V_DeptType = V_DeptType;
            this._V_DeptTypeOperation = V_DeptTypeOperation;
            this.description = description;
            this.children = new List<RefDepartmentsTree>();
            this.parent = parent;
            //▼==== #001
            this.DeptNameEng = DeptNameEng;
            //▲==== #001
        }

        private bool _isExpanded;
        [DataMember]
        public bool IsExpanded
        {
            get
            { return _isExpanded; }
            set
            {
                _isExpanded = value;
                RaisePropertyChanged("IsExpanded");
            }
        }

        private bool _isSelected;
        [DataMember]
        public bool IsSelected
        {
            get
            { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        public RefDepartmentsTree(string nodeText, long nodeID, System.Nullable<long> parentID, Nullable<long> V_DeptType, Nullable<long> V_DeptTypeOperation, string description, RefDepartmentsTree parent, int lvel)
        {
            this.nodeText = nodeText;
            this.nodeID = nodeID;
            this.parentID = parentID;
            this._V_DeptType = V_DeptType;
            this._V_DeptTypeOperation = V_DeptTypeOperation;
            this.description = description;
            this.children = new List<RefDepartmentsTree>();
            this.childrens = new List<DeptLocation>();
            this.Level = lvel;
            this.parent = parent;
        }

        public RefDepartmentsTree(string nodeText, long nodeID, System.Nullable<long> parentID, Nullable<long> V_DeptType, Nullable<long> V_DeptTypeOperation, string description, RefDepartmentsTree parent, int lvel, bool pisDeptLocation, string pImgIcon)
        {
            this.nodeText = nodeText;
            this.nodeID = nodeID;
            this.parentID = parentID;
            this._V_DeptType = V_DeptType;
            this._V_DeptTypeOperation = V_DeptTypeOperation;
            this.description = description;
            this.children = new List<RefDepartmentsTree>();
            this.childrens = new List<DeptLocation>();
            this.Level = lvel;
            this.parent = parent;
            this.isDeptLocation = pisDeptLocation;
            this.ImgIcon = pImgIcon;
        }

        public RefDepartmentsTree(string nodeText, long nodeID, System.Nullable<long> parentID, Nullable<long> V_DeptType, Nullable<long> V_DeptTypeOperation, string description, RefDepartmentsTree parent, int lvel, bool phasDeptLocation, bool pisDeptLocation)
        {
            this.nodeText = nodeText;
            this.nodeID = nodeID;
            this.parentID = parentID;
            this._V_DeptType = V_DeptType;
            this._V_DeptTypeOperation = V_DeptTypeOperation;
            this.description = description;
            this.children = new List<RefDepartmentsTree>();
            this.childrens = new List<DeptLocation>();
            this.Level = lvel;
            this.parent = parent;
            this.hasDeptLocation = phasDeptLocation;
            this.isDeptLocation = pisDeptLocation;
        }


        [Required(ErrorMessage = "Node Text is required")]
        [DataMember]
        public string NodeText
        {
            get
            {
                return this.nodeText;
            }
            set
            {
                ValidateProperty("NodeText", value);
                this.nodeText = value;


            }
        }

        [DataMember]
        public System.Nullable<long> ParentID
        {
            get
            {
                return this.parentID;
            }
            set
            {
                this.parentID = value;
            }
        }

        
        private Nullable<long> _V_DeptType;
         [DataMember]
        public Nullable<long> V_DeptType
        {
            get { return _V_DeptType; }
            set { _V_DeptType = value; }
        }


         private Nullable<long> _V_DeptTypeOperation;
         [DataMember]
         public Nullable<long> V_DeptTypeOperation
         {
             get { return _V_DeptTypeOperation; }
             set { _V_DeptTypeOperation = value; }
         }

        [DataMember]
        public long NodeID
        {
            get
            {
                return this.nodeID;
            }
            set
            {
                this.nodeID = value;
            }
        }
        [DataMember]
        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }
        [DataMember]
        public List<RefDepartmentsTree> Children
        {
            get
            {
                return this.children;
            }
            set
            {
                this.children = value;
            }
        }
        public List<DeptLocation> Childrens
        {
            get
            {
                return this.childrens;
            }
            set
            {
                this.childrens = value;
            }
        }

        [DataMemberAttribute()]
        public RefDepartmentsTree Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                this.parent = value;
                RaisePropertyChanged("Parent");
            }
        }
        
        [DataMemberAttribute()]
        public int Level
        {
            get
            {
                return level;
            }
            set
            {
                if (level == value)
                    return;
                level = value;
                RaisePropertyChanged("Level");
            }
        }

        [DataMemberAttribute()]
        public bool IsDeptLocation
        {
            get { return isDeptLocation; }
            set 
            { 
                isDeptLocation = value;
                RaisePropertyChanged("IsDeptLocation");
            }
        }
        
        private bool hasDeptLocation=false;
        [DataMemberAttribute()]
        public bool HasDeptLocation
        {
            get { return hasDeptLocation; }
            set 
            { 
                hasDeptLocation = value;
                RaisePropertyChanged("HasDeptLocation");
            }
        }

        [DataMemberAttribute()]

        public List<ConsultationRoomTarget> LstConsultationRoomTarget
        {
            get { return lstConsultationRoomTarget; }
            set
            {
                lstConsultationRoomTarget = value;
                RaisePropertyChanged("LstConsultationRoomTarget");
            }
        }
        [DataMemberAttribute()]

        public List<ConsultationRoomStaffAllocations> LstConsultationRoomStaffAllocations
        {
            get { return lstConsultationRoomStaffAllocations; }
            set
            {
                lstConsultationRoomStaffAllocations = value;
                RaisePropertyChanged("LstConsultationRoomStaffAllocations");
            }
        }

        private string _ImgIcon = "/eHCMSCal;component/Assets/Images/Folder-Home-Alternate-White-icon.png";
        [DataMemberAttribute()]
        public string ImgIcon
        {
            get { return _ImgIcon; }
            set 
            {
                _ImgIcon = value;
                RaisePropertyChanged("ImgIcon");
            }
        }

        //▼==== #001
        [DataMemberAttribute()]
        public string DeptNameEng
        {
            get
            {
                return _DeptNameEng;
            }
            set
            {
                _DeptNameEng = value;
                RaisePropertyChanged("DeptNameEng");
            }
        }
        private string _DeptNameEng = "";
        //▲==== #001

        public override bool Equals(object obj)
        {
            var cond = obj as RefDepartmentsTree;
            if (cond == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.NodeID > 0 && this.NodeID == cond.NodeID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
    public static class RefDepartmentsTreeExt
    {
        private static void FillaEMRImgIcon(RefDepartmentsTree aRefDepartmentsTree)
        {
            aRefDepartmentsTree.ImgIcon = aRefDepartmentsTree.ImgIcon.Replace("/eHCMSCal;", "/aEMR.CommonViews;");
            foreach (var item in aRefDepartmentsTree.Children)
            {
                FillaEMRImgIcon(item);
            }
        }
        public static void ConvertaEMRImgIcon(this System.Collections.ObjectModel.ObservableCollection<RefDepartmentsTree> aRefDepartmentsTreeCollection)
        {
            foreach (var item in aRefDepartmentsTreeCollection)
                FillaEMRImgIcon(item);
        }
    }
}