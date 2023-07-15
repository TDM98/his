using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
   // [DataContract]
    public class ExamGroupTypeTree : NotifyChangedBase, IEditableObject
    {
        public ExamGroupTypeTree(): base()
        {

        }

        private ExamGroupTypeTree _tempAccidentdrug;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempAccidentdrug = (ExamGroupTypeTree)this.MemberwiseClone();
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

        public void CopyFrom(ExamGroupTypeTree p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion

        private string _PCLExamTypeName;
        private long _PCLExamTypeID;
        private System.Nullable<long> _PCLExamGoupID;
        private List<ExamGroupTypeTree> children;
        private string _PCLExamTypeDescription;
        private string _PCLExamTypeCode;
        private PCLExamType parent;
        public ExamGroupTypeTree(string PCLExamTypeName, long PCLExamTypeID, System.Nullable<long> PCLExamGoupID, string PCLExamTypeDescription, string PCLExamTypeCode, PCLExamType parent)
        {
            this._PCLExamTypeName = PCLExamTypeName;
            this._PCLExamTypeID = PCLExamTypeID;
            this._PCLExamGoupID = PCLExamGoupID;
            this._PCLExamTypeDescription = PCLExamTypeDescription;
            this.children = new List<ExamGroupTypeTree>();
            this.parent = parent;
            this._PCLExamTypeCode = PCLExamTypeCode;
        }

        [DataMember]
        public string PCLExamTypeName
        {
            get
            {
                return this._PCLExamTypeName;
            }
            set
            {
                ValidateProperty("PCLExamTypeName", value);
                this._PCLExamTypeName = value;
               
                
            }
        }
    
        [DataMember]
        public System.Nullable<long> PCLExamGoupID
        {
            get
            {
                return this._PCLExamGoupID;
            }
            set
            {
                this._PCLExamGoupID = value;
            }
        }
        [DataMember]
        public long PCLExamTypeID
        {
            get
            {
                return this._PCLExamTypeID;
            }
            set
            {
                this._PCLExamTypeID = value;
            }
        }
        [DataMember]
        public string PCLExamTypeDescription
        {
            get
            {
                return this._PCLExamTypeDescription;
            }
            set
            {
                this._PCLExamTypeDescription = value;
            }
        }
        [DataMember]
        public List<ExamGroupTypeTree> Children
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

        [DataMember]
        public PCLExamType Parent
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

        [DataMember]
        public string PCLExamTypeCode
        {
            get
            {
                return this._PCLExamTypeCode;
            }
            set
            {
                this._PCLExamTypeCode = value;
            }
        }
    }
}

