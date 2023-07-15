using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class PCLExamParamResult : NotifyChangedBase, IEditableObject
    {
        public PCLExamParamResult()
            : base()
        {

        }

        private PCLExamParamResult _tempPCLExamParamResult;
        public override bool Equals(object obj)
        {
            PCLExamParamResult info = obj as PCLExamParamResult;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PCLExamResultID > 0 && this.PCLExamResultID == info.PCLExamResultID;
        }
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempPCLExamParamResult = (PCLExamParamResult)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempPCLExamParamResult)
                CopyFrom(_tempPCLExamParamResult);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(PCLExamParamResult p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new PCLExamParamResult object.

        /// <param name="bedLocNumber">Initial value of the BedLocNumber property.</param>
        /// <param name="allocationID">Initial value of the AllocationID property.</param>
        public static PCLExamParamResult CreatePCLExamParamResult(String bedLocNumber, long allocationID)
        {
            PCLExamParamResult PCLExamParamResult = new PCLExamParamResult();
            //PCLExamParamResult.BedLocNumber = bedLocNumber;
            //PCLExamParamResult.AllocationID = allocationID;
            return PCLExamParamResult;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long PCLExamResultID
        {
            get
            {
                return _PCLExamResultID;
            }
            set
            {
                OnPCLExamResultIDChanging(value);
                _PCLExamResultID = value;
                RaisePropertyChanged("PCLExamResultID");
                OnPCLExamResultIDChanged();
            }
        }
        private long _PCLExamResultID;
        partial void OnPCLExamResultIDChanging(long value);
        partial void OnPCLExamResultIDChanged();




        [DataMemberAttribute()]
        public int ParamEnum
        {
            get
            {
                return _ParamEnum;
            }
            set
            {
                OnParamEnumChanging(value);
                _ParamEnum = value;
                RaisePropertyChanged("ParamEnum");
                OnParamEnumChanged();
            }
        }
        private int _ParamEnum;
        partial void OnParamEnumChanging(int value);
        partial void OnParamEnumChanged();




        [DataMemberAttribute()]
        public int PCLExamGroupTemplateResultID
        {
            get
            {
                return _PCLExamGroupTemplateResultID;
            }
            set
            {
                OnPCLExamGroupTemplateResultIDChanging(value);
                _PCLExamGroupTemplateResultID = value;
                RaisePropertyChanged("PCLExamGroupTemplateResultID");
                OnPCLExamGroupTemplateResultIDChanged();
            }
        }
        private int _PCLExamGroupTemplateResultID;
        partial void OnPCLExamGroupTemplateResultIDChanging(int value);
        partial void OnPCLExamGroupTemplateResultIDChanged();




        [DataMemberAttribute()]
        public string GroupName
        {
            get
            {
                return _GroupName;
            }
            set
            {
                OnGroupNameChanging(value);
                _GroupName = value;
                RaisePropertyChanged("GroupName");
                OnGroupNameChanged();
            }
        }
        private string _GroupName;
        partial void OnGroupNameChanging(string value);
        partial void OnGroupNameChanged(); 


        #endregion

        #region Navigation Properties


        #endregion

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public partial class PCLExamResultTemplate : NotifyChangedBase, IEditableObject
    {
        public PCLExamResultTemplate()
            : base()
        {

        }

        private PCLExamResultTemplate _tempPCLExamResultTemplate;
        public override bool Equals(object obj)
        {
            PCLExamResultTemplate info = obj as PCLExamResultTemplate;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PCLExamResultTemplateID > 0 && this.PCLExamResultTemplateID == info.PCLExamResultTemplateID;
        }
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempPCLExamResultTemplate = (PCLExamResultTemplate)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempPCLExamResultTemplate)
                CopyFrom(_tempPCLExamResultTemplate);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(PCLExamResultTemplate p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new PCLExamResultTemplate object.

        /// <param name="bedLocNumber">Initial value of the BedLocNumber property.</param>
        /// <param name="allocationID">Initial value of the AllocationID property.</param>
        public static PCLExamResultTemplate CreatePCLExamResultTemplate(String bedLocNumber, long allocationID)
        {
            PCLExamResultTemplate PCLExamResultTemplate = new PCLExamResultTemplate();
            //PCLExamResultTemplate.BedLocNumber = bedLocNumber;
            //PCLExamResultTemplate.AllocationID = allocationID;
            return PCLExamResultTemplate;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long PCLExamResultTemplateID
        {
            get
            {
                return _PCLExamResultTemplateID;
            }
            set
            {
                OnPCLExamResultTemplateIDChanging(value);
                _PCLExamResultTemplateID = value;
                RaisePropertyChanged("PCLExamResultTemplateID");
                OnPCLExamResultTemplateIDChanged();
            }
        }
        private long _PCLExamResultTemplateID;
        partial void OnPCLExamResultTemplateIDChanging(long value);
        partial void OnPCLExamResultTemplateIDChanged();




        [DataMemberAttribute()]
        public string PCLExamTemplateName
        {
            get
            {
                return _PCLExamTemplateName;
            }
            set
            {
                OnPCLExamTemplateNameChanging(value);
                _PCLExamTemplateName = value;
                RaisePropertyChanged("PCLExamTemplateName");
                OnPCLExamTemplateNameChanged();
            }
        }
        private string _PCLExamTemplateName;
        partial void OnPCLExamTemplateNameChanging(string value);
        partial void OnPCLExamTemplateNameChanged();




        [DataMemberAttribute()]
        public int PCLExamGroupTemplateResultID
        {
            get
            {
                return _PCLExamGroupTemplateResultID;
            }
            set
            {
                OnPCLExamGroupTemplateResultIDChanging(value);
                _PCLExamGroupTemplateResultID = value;
                RaisePropertyChanged("PCLExamGroupTemplateResultID");
                OnPCLExamGroupTemplateResultIDChanged();
            }
        }
        private int _PCLExamGroupTemplateResultID;
        partial void OnPCLExamGroupTemplateResultIDChanging(int value);
        partial void OnPCLExamGroupTemplateResultIDChanged();




        [DataMemberAttribute()]
        public string ResultContent
        {
            get
            {
                return _ResultContent;
            }
            set
            {
                OnResultContentChanging(value);
                _ResultContent = value;
                RaisePropertyChanged("ResultContent");
                OnResultContentChanged();
            }
        }
        private string _ResultContent;
        partial void OnResultContentChanging(string value);
        partial void OnResultContentChanged();




        [DataMemberAttribute()]
        public string Descriptions
        {
            get
            {
                return _Descriptions;
            }
            set
            {
                OnDescriptionsChanging(value);
                _Descriptions = value;
                RaisePropertyChanged("Descriptions");
                OnDescriptionsChanged();
            }
        }
        private string _Descriptions;
        partial void OnDescriptionsChanging(string value);
        partial void OnDescriptionsChanged();

        #endregion

        #region Navigation Properties


        #endregion

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
