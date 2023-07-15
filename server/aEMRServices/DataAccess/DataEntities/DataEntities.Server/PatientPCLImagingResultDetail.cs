using eHCMS.Services.Core;
using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities
{
    public partial class PatientPCLImagingResultDetail : NotifyChangedBase
    {
        #region Factory Method
        /// Create a new PatientPCLLaboratoryResultDetail object.
        /// <param name="PCLImgResultDetailID">Initial value of the PCLImgResultDetailID property.</param>
        public static PatientPCLImagingResultDetail CreatePatientPCLLaboratoryResultDetail(Int64 PCLImgResultDetailID)
        {
            PatientPCLImagingResultDetail patientPCLImagingResultDetail = new PatientPCLImagingResultDetail();
            patientPCLImagingResultDetail.PCLImgResultDetailID = PCLImgResultDetailID;
            return patientPCLImagingResultDetail;
        }

        #endregion
        #region Primitive Properties


        [DataMemberAttribute()]
        public Int64 PCLImgResultDetailID
        {
            get
            {
                return _PCLImgResultDetailID;
            }
            set
            {
                if (_PCLImgResultDetailID != value)
                {
                    OnPCLImgResultDetailIDChanging(value);
                    _PCLImgResultDetailID = value;
                    RaisePropertyChanged("PCLImgResultDetailID");
                    OnPCLImgResultDetailIDChanged();
                }
            }
        }
        private Int64 _PCLImgResultDetailID;
        partial void OnPCLImgResultDetailIDChanging(Int64 value);
        partial void OnPCLImgResultDetailIDChanged();


        [DataMemberAttribute()]
        public Int64 PCLImgResultID
        {
            get
            {
                return _PCLImgResultID;
            }
            set
            {
                OnPCLImgResultIDChanging(value);
                _PCLImgResultID = value;
                RaisePropertyChanged("PCLImgResultID");
                OnPCLImgResultIDChanged();
            }
        }
        private Int64 _PCLImgResultID;
        partial void OnPCLImgResultIDChanging(Int64 value);
        partial void OnPCLImgResultIDChanged();




        [DataMemberAttribute()]
        public Int64 PCLExamTypeID
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
        private Int64 _PCLExamTypeID;
        partial void OnPCLExamTypeIDChanging(Int64 value);
        partial void OnPCLExamTypeIDChanged();

        [DataMemberAttribute()]
        public String Value
        {
            get
            {
                return _Value;
            }
            set
            {
                OnValueChanging(value);
                _Value = value;
                RaisePropertyChanged("Value");
                OnValueChanged();
            }
        }
        private String _Value;
        partial void OnValueChanging(String value);
        partial void OnValueChanged();

        #region Backup
        ///// <summary>
        //
        ///// </summary>
        //[DataMemberAttribute()]
        //public Nullable<Double> Value1
        //{
        //    get
        //    {
        //        return _Value1;
        //    }
        //    set
        //    {
        //        OnValue1Changing(value);
        //        _Value1 = value;
        //        RaisePropertyChanged("Value1");
        //        OnValue1Changed();
        //    }
        //}
        //private Nullable<Double> _Value1;
        //partial void OnValue1Changing(Nullable<Double> value);
        //partial void OnValue1Changed();

        ///// <summary>
        //
        ///// </summary>
        //[DataMemberAttribute()]
        //public Nullable<Double> Value2
        //{
        //    get
        //    {
        //        return _Value2;
        //    }
        //    set
        //    {
        //        OnValue2Changing(value);
        //        _Value2 = value;
        //        RaisePropertyChanged("Value2");
        //        OnValue2Changed();
        //    }
        //}
        //private Nullable<Double> _Value2;
        //partial void OnValue2Changing(Nullable<Double> value);
        //partial void OnValue2Changed();
        #endregion


        [DataMemberAttribute()]
        public Nullable<Boolean> IsNormal
        {
            get
            {
                return _IsNormal;
            }
            set
            {
                OnIsNormalChanging(value);
                _IsNormal = value;
                RaisePropertyChanged("IsNormal");
                OnIsNormalChanged();
            }
        }
        private Nullable<Boolean> _IsNormal;
        partial void OnIsNormalChanging(Nullable<Boolean> value);
        partial void OnIsNormalChanged();


        [DataMemberAttribute()]
        public Nullable<Byte> NumberOfTest
        {
            get
            {
                return _NumberOfTest;
            }
            set
            {
                OnNumberOfTestChanging(value);
                _NumberOfTest = value;
                RaisePropertyChanged("NumberOfTest");
                OnNumberOfTestChanged();
            }
        }
        private Nullable<Byte> _NumberOfTest;
        partial void OnNumberOfTestChanging(Nullable<Byte> value);
        partial void OnNumberOfTestChanged();

        ///// <summary>
        //
        ///// </summary>
        //[DataMemberAttribute()]
        //public String SampleCode
        //{
        //    get
        //    {
        //        return _SampleCode;
        //    }
        //    set
        //    {
        //        OnSampleCodeChanging(value);
        //        _SampleCode = value;
        //        RaisePropertyChanged("SampleCode");
        //        OnSampleCodeChanged();
        //    }
        //}
        //private String _SampleCode;
        //partial void OnSampleCodeChanging(String value);
        //partial void OnSampleCodeChanged();

        [DataMemberAttribute()]
        public String Comments
        {
            get
            {
                return _Comments;
            }
            set
            {
                OnCommentsChanging(value);
                _Comments = value;
                RaisePropertyChanged("Comments");
                OnCommentsChanged();
            }
        }
        private String _Comments;
        partial void OnCommentsChanging(String value);
        partial void OnCommentsChanged();


        [DataMemberAttribute()]
        public long PCLExamTypeTestItemID
        {
            get
            {
                return _PCLExamTypeTestItemID;
            }
            set
            {
                OnPCLExamTypeTestItemIDChanging(value);
                _PCLExamTypeTestItemID = value;
                RaisePropertyChanged("PCLExamTypeTestItemID");
                _V_PCLExamTypeTestItems = new PCLExamTypeTestItems();
                _V_PCLExamTypeTestItems.PCLExamTypeTestItemID = PCLExamTypeTestItemID;
                OnPCLExamTypeTestItemIDChanged();
            }
        }
        private long _PCLExamTypeTestItemID;
        partial void OnPCLExamTypeTestItemIDChanging(long value);
        partial void OnPCLExamTypeTestItemIDChanged();


        [DataMemberAttribute()]
        public long PCLExamTestItemID
        {
            get
            {
                return _PCLExamTestItemID;
            }
            set
            {
                OnPCLExamTestItemIDChanging(value);
                _PCLExamTestItemID = value;
                RaisePropertyChanged("PCLExamTestItemID");
                OnPCLExamTestItemIDChanged();
            }
        }
        private long _PCLExamTestItemID;
        partial void OnPCLExamTestItemIDChanging(long value);
        partial void OnPCLExamTestItemIDChanged();



        [DataMemberAttribute()]
        public bool IsAbnormal
        {
            get
            {
                return _IsAbnormal;
            }
            set
            {
                OnIsAbnormalChanging(value);
                _IsAbnormal = value;
                RaisePropertyChanged("IsAbnormal");
                OnIsAbnormalChanged();
            }
        }
        private bool _IsAbnormal;
        partial void OnIsAbnormalChanging(bool value);
        partial void OnIsAbnormalChanged();


        [DataMemberAttribute()]
        public string PCLExamTestItemName
        {
            get
            {
                return _PCLExamTestItemName;
            }
            set
            {
                OnPCLExamTestItemNameChanging(value);
                _PCLExamTestItemName = value;
                RaisePropertyChanged("PCLExamTestItemName");
                OnPCLExamTestItemNameChanged();
            }
        }
        private string _PCLExamTestItemName;
        partial void OnPCLExamTestItemNameChanging(string value);
        partial void OnPCLExamTestItemNameChanged();


        [DataMemberAttribute()]
        public string PCLExamTestItemUnit
        {
            get
            {
                return _PCLExamTestItemUnit;
            }
            set
            {
                OnPCLExamTestItemUnitChanging(value);
                _PCLExamTestItemUnit = value;
                RaisePropertyChanged("PCLExamTestItemUnit");
                OnPCLExamTestItemUnitChanged();
            }
        }
        private string _PCLExamTestItemUnit;
        partial void OnPCLExamTestItemUnitChanging(string value);
        partial void OnPCLExamTestItemUnitChanged();


        [DataMemberAttribute()]
        public string PCLExamTestItemRefScale
        {
            get
            {
                return _PCLExamTestItemRefScale;
            }
            set
            {
                OnPCLExamTestItemRefScaleChanging(value);
                _PCLExamTestItemRefScale = value;
                RaisePropertyChanged("PCLExamTestItemRefScale");
                OnPCLExamTestItemRefScaleChanged();
            }
        }
        private string _PCLExamTestItemRefScale;
        partial void OnPCLExamTestItemRefScaleChanging(string value);
        partial void OnPCLExamTestItemRefScaleChanged();

        [DataMemberAttribute()]
        public bool IsBold
        {
            get
            {
                return _IsBold;
            }
            set
            {
                OnIsBoldChanging(value);
                _IsBold = value;
                RaisePropertyChanged("IsBold");
                OnIsBoldChanged();
            }
        }
        private bool _IsBold;
        partial void OnIsBoldChanging(bool value);
        partial void OnIsBoldChanged();


        [DataMemberAttribute()]
        public bool IsNoNeedResult
        {
            get
            {
                return _IsNoNeedResult;
            }
            set
            {
                OnIsNoNeedResultChanging(value);
                _IsNoNeedResult = value;
                RaisePropertyChanged("IsNoNeedResult");
                OnIsNoNeedResultChanged();
            }
        }
        private bool _IsNoNeedResult = true;
        partial void OnIsNoNeedResultChanging(bool value);
        partial void OnIsNoNeedResultChanged();


        [DataMemberAttribute()]
        public String Value_Old
        {
            get
            {
                return _Value_Old;
            }
            set
            {
                OnValue_OldChanging(value);
                _Value_Old = value;
                RaisePropertyChanged("Value_Old");
                OnValue_OldChanged();
            }
        }
        private String _Value_Old;
        partial void OnValue_OldChanging(String value);
        partial void OnValue_OldChanged();


        private long _PatientPCLReqID;
        [DataMemberAttribute]
        public long PatientPCLReqID
        {
            get => _PatientPCLReqID; set
            {
                _PatientPCLReqID = value;
                RaisePropertyChanged("PatientPCLReqID");
            }
        }
        #endregion
        #region Navigation Properties

        [DataMemberAttribute()]
        public PatientPCLLaboratoryResult PatientPCLLaboratoryResult
        {
            get;
            set;
        }


        [DataMemberAttribute()]
        public PCLExamType PCLExamType
        {
            get;
            set;
        }


        [DataMemberAttribute()]
        public PCLExamTypeTestItems V_PCLExamTypeTestItems
        {
            get
            {
                return _V_PCLExamTypeTestItems;
            }
            set
            {
                OnV_PCLExamTypeTestItemsChanging(value);
                _V_PCLExamTypeTestItems = value;
                RaisePropertyChanged("V_PCLExamTypeTestItems");
                //if (V_PCLExamTypeTestItems!=null)
                //{
                //    _PCLExamTypeTestItemID = V_PCLExamTypeTestItems.PCLExamTypeTestItemID;    
                //}

                OnV_PCLExamTypeTestItemsChanged();
            }
        }
        private PCLExamTypeTestItems _V_PCLExamTypeTestItems;
        partial void OnV_PCLExamTypeTestItemsChanging(PCLExamTypeTestItems value);
        partial void OnV_PCLExamTypeTestItemsChanged();
        #endregion

        private bool _IsChecked = false;
        [DataMemberAttribute]
        public bool IsChecked
        {
            get
            {
                return _IsChecked;
            }
            set
            {
                if (_IsChecked == value)
                {
                    return;
                }
                _IsChecked = value;
                RaisePropertyChanged("IsChecked");
            }
        }
        private long _PCLSectionID;
        [DataMemberAttribute]
        public long PCLSectionID
        {
            get
            {
                return _PCLSectionID;
            }
            set
            {
                if (_PCLSectionID == value)
                {
                    return;
                }
                _PCLSectionID = value;
                RaisePropertyChanged("PCLSectionID");
            }
        }
        private string _SectionName;
        [DataMemberAttribute]
        public string SectionName
        {
            get
            {
                return _SectionName;
            }
            set
            {
                if (_SectionName == value)
                {
                    return;
                }
                _SectionName = value;
                RaisePropertyChanged("SectionName");
            }
        }
        private Boolean? _IsForMen;
        [DataMemberAttribute]
        public Boolean? IsForMen
        {
            get
            {
                return _IsForMen;
            }
            set
            {
                if (_IsForMen == value)
                {
                    return;
                }
                _IsForMen = value;
                RaisePropertyChanged("IsForMen");
            }
        }
        private int _PrintIdx;
        [DataMemberAttribute]
        public int PrintIdx
        {
            get
            {
                return _PrintIdx;
            }
            set
            {
                if (_PrintIdx == value)
                {
                    return;
                }
                _PrintIdx = value;
                RaisePropertyChanged("PrintIdx");
            }
        }
    }
}
