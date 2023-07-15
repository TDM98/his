using System;
using System.Runtime.Serialization;
using eHCMS.Services.Core.Base;
/*
 * 20230520 #001 DatTB: Thêm cột tên tiếng anh các danh mục xét nghiệm 
*/
namespace DataEntities
{
    public partial class PCLExamTestItems : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new PCLExamParamResult object.

        /// <param name="bedLocNumber">Initial value of the BedLocNumber property.</param>
        /// <param name="allocationID">Initial value of the AllocationID property.</param>
        public static PCLExamTestItems CreatePCLExamTestItems(string PCLExamTestItemName, string PCLExamTestItemCode)
        {
            PCLExamTestItems PCLExamTestItem = new PCLExamTestItems();
            PCLExamTestItem.PCLExamTestItemName = PCLExamTestItemName;
            PCLExamTestItem.PCLExamTestItemCode = PCLExamTestItemCode;

            return PCLExamTestItem;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 PCLExamTestItemID
        {
            get
            {
                return _PCLExamTestItemID;
            }
            set
            {
                if (_PCLExamTestItemID != value)
                {
                    OnPCLExamTestItemIDChanging(value);
                    _PCLExamTestItemID = value;
                    RaisePropertyChanged("PCLExamTestItemID");
                    OnPCLExamTestItemIDChanged();
                }
            }
        }
        private Int64 _PCLExamTestItemID;
        partial void OnPCLExamTestItemIDChanging(Int64 value);
        partial void OnPCLExamTestItemIDChanged();

        [DataMemberAttribute()]
        public String PCLExamTestItemName
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
        private String _PCLExamTestItemName;
        partial void OnPCLExamTestItemNameChanging(String value);
        partial void OnPCLExamTestItemNameChanged();


        [DataMemberAttribute()]
        public String PCLExamTestItemDescription
        {
            get
            {
                return _PCLExamTestItemDescription;
            }
            set
            {
                OnPCLExamTestItemDescriptionChanging(value);
                _PCLExamTestItemDescription = value;
                RaisePropertyChanged("PCLExamTestItemDescription");
                OnPCLExamTestItemDescriptionChanged();
            }
        }
        private String _PCLExamTestItemDescription;
        partial void OnPCLExamTestItemDescriptionChanging(String value);
        partial void OnPCLExamTestItemDescriptionChanged();



        [DataMemberAttribute()]
        public String PCLExamTestItemCode
        {
            get
            {
                return _PCLExamTestItemCode;
            }
            set
            {
                OnPCLExamTestItemCodeChanging(value);
                _PCLExamTestItemCode = value;
                RaisePropertyChanged("PCLExamTestItemCode");
                OnPCLExamTestItemCodeChanged();
            }
        }
        private String _PCLExamTestItemCode;
        partial void OnPCLExamTestItemCodeChanging(String value);
        partial void OnPCLExamTestItemCodeChanged();


        
        [DataMemberAttribute()]
        public String PCLExamTestItemUnit
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
        private String _PCLExamTestItemUnit;
        partial void OnPCLExamTestItemUnitChanging(String value);
        partial void OnPCLExamTestItemUnitChanged();

        
        [DataMemberAttribute()]
        public String PCLExamTestItemRefScale
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
        private String _PCLExamTestItemRefScale;
        partial void OnPCLExamTestItemRefScaleChanging(String value);
        partial void OnPCLExamTestItemRefScaleChanged();


           
        [DataMemberAttribute()]
        public bool TestItemIsExamType
        {
            get
            {
                return _TestItemIsExamType;
            }
            set
            {
                OnTestItemIsExamTypeChanging(value);
                _TestItemIsExamType = value;
                RaisePropertyChanged("TestItemIsExamType");
                OnTestItemIsExamTypeChanged();
            }
        }
        private bool _TestItemIsExamType;
        partial void OnTestItemIsExamTypeChanging(bool value);
        partial void OnTestItemIsExamTypeChanged();
        
        
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
        private bool _IsNoNeedResult;
        partial void OnIsNoNeedResultChanging(bool value);
        partial void OnIsNoNeedResultChanged();


        [DataMemberAttribute()]
        public Nullable<Boolean> IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                OnIsActiveChanging(value);
                _IsActive = value;
                RaisePropertyChanged("IsActive");
                OnIsActiveChanged();
            }
        }
        private Nullable<Boolean> _IsActive;
        partial void OnIsActiveChanging(Nullable<Boolean> value);
        partial void OnIsActiveChanged();


        [DataMemberAttribute()]
        public String CodeTMP
        {
            get
            {
                return _CodeTMP;
            }
            set
            {
                OnCodeTMPChanging(value);

                _CodeTMP = value;
                RaisePropertyChanged("CodeTMP");
                OnCodeTMPChanged();
            }
        }
        private String _CodeTMP;
        partial void OnCodeTMPChanging(String value);
        partial void OnCodeTMPChanged();

        [DataMemberAttribute()]
        public PCLExamType PCLExamType
        {
            get
            {
                return _PCLExamType;
            }
            set
            {
                OnPCLExamTypeChanging(value);

                _PCLExamType = value;
                RaisePropertyChanged("PCLExamType");
                OnPCLExamTypeChanged();
            }
        }
        private PCLExamType _PCLExamType;
        partial void OnPCLExamTypeChanging(PCLExamType value);
        partial void OnPCLExamTypeChanged();

        [DataMemberAttribute()]
        public long PCLExamTypeID
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
        private long _PCLExamTypeID;
        partial void OnPCLExamTypeIDChanging(long value);
        partial void OnPCLExamTypeIDChanged();

        [DataMemberAttribute()]
        public string PCLExamTypeName
        {
            get
            {
                return _PCLExamTypeName;
            }
            set
            {
                OnPCLExamTypeNameChanging(value);

                _PCLExamTypeName = value;
                RaisePropertyChanged("PCLExamTypeName");
                OnPCLExamTypeNameChanged();
            }
        }
        private string _PCLExamTypeName;
        partial void OnPCLExamTypeNameChanging(string value);
        partial void OnPCLExamTypeNameChanged();

        private string _PCLExamTestItemHICode;
        [DataMemberAttribute()]
        public string PCLExamTestItemHICode
        {
            get
            {
                return _PCLExamTestItemHICode;
            }
            set
            {
                _PCLExamTestItemHICode = value;
                RaisePropertyChanged("PCLExamTestItemHICode");
            }
        }
        private string _PCLExamTestItemHIName;
        [DataMemberAttribute()]
        public string PCLExamTestItemHIName
        {
            get
            {
                return _PCLExamTestItemHIName;
            }
            set
            {
                _PCLExamTestItemHIName = value;
                RaisePropertyChanged("PCLExamTestItemHIName");
            }
        }
        #endregion

        public override bool Equals(object obj)
        {
            PCLExamTestItems SelectedItem = obj as PCLExamTestItems;
            if (SelectedItem == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PCLExamTestItemID > 0 && this.PCLExamTestItemID == SelectedItem.PCLExamTestItemID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region Ext member


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


        [DataMemberAttribute()]
        public DateTime? SamplingDate
        {
            get
            {
                return _SamplingDate;
            }
            set
            {
                OnSamplingDateChanging(value);
                _SamplingDate = value;
                RaisePropertyChanged("SamplingDate");
                OnSamplingDateChanged();
            }
        }
        private DateTime? _SamplingDate;
        partial void OnSamplingDateChanging(DateTime? value);
        partial void OnSamplingDateChanged();

        [DataMemberAttribute()]
        public bool Checked
        {
            get
            {
                return _Checked;
            }
            set
            {
                OnCheckedChanging(value);
                _Checked = value;
                RaisePropertyChanged("Checked");
                OnCheckedChanged();
            }
        }
        private bool _Checked;
        partial void OnCheckedChanging(bool value);
        partial void OnCheckedChanged();
        #endregion

        private bool _IsAbnormal;
        [DataMemberAttribute]
        public bool IsAbnormal
        {
            get
            {
                return _IsAbnormal;
            }
            set
            {
                if (_IsAbnormal == value)
                {
                    return;
                }
                _IsAbnormal = value;
                RaisePropertyChanged("IsAbnormal");
            }
        }
        private bool _IsTechnique;
        [DataMemberAttribute]
        public bool IsTechnique
        {
            get
            {
                return _IsTechnique;
            }
            set
            {
                if (_IsTechnique == value)
                {
                    return;
                }
                _IsTechnique = value;
                RaisePropertyChanged("IsTechnique");
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
        private long _PCLExamTypeTestItemID;
        [DataMemberAttribute]
        public long PCLExamTypeTestItemID
        {
            get
            {
                return _PCLExamTypeTestItemID;
            }
            set
            {
                if (_PCLExamTypeTestItemID == value)
                {
                    return;
                }
                _PCLExamTypeTestItemID = value;
                RaisePropertyChanged("PCLExamTypeTestItemID");
            }
        }
        private int? _ColumnValue;
        [DataMemberAttribute]
        public int? ColumnValue
        {
            get
            {
                return _ColumnValue;
            }
            set
            {
                if (_ColumnValue == value)
                {
                    return;
                }
                _ColumnValue = value;
                RaisePropertyChanged("ColumnValue");
            }
        }

        //▼==== #001
        [DataMemberAttribute()]
        public String PCLExamTestItemNameEng
        {
            get
            {
                return _PCLExamTestItemNameEng;
            }
            set
            {
                OnPCLExamTestItemNameEngChanging(value);

                _PCLExamTestItemNameEng = value;
                RaisePropertyChanged("PCLExamTestItemNameEng");
                OnPCLExamTestItemNameEngChanged();
            }
        }
        private String _PCLExamTestItemNameEng;
        partial void OnPCLExamTestItemNameEngChanging(String value);
        partial void OnPCLExamTestItemNameEngChanged();

        [DataMemberAttribute()]
        public String PCLExamTestItemUnitEng
        {
            get
            {
                return _PCLExamTestItemUnitEng;
            }
            set
            {
                OnPCLExamTestItemUnitEngChanging(value);
                _PCLExamTestItemUnitEng = value;
                RaisePropertyChanged("PCLExamTestItemUnitEng");
                OnPCLExamTestItemUnitEngChanged();
            }
        }
        private String _PCLExamTestItemUnitEng;
        partial void OnPCLExamTestItemUnitEngChanging(String value);
        partial void OnPCLExamTestItemUnitEngChanged();

        [DataMemberAttribute()]
        public String PCLExamTestItemRefScaleEng
        {
            get
            {
                return _PCLExamTestItemRefScaleEng;
            }
            set
            {
                OnPCLExamTestItemRefScaleEngChanging(value);
                _PCLExamTestItemRefScaleEng = value;
                RaisePropertyChanged("PCLExamTestItemRefScaleEng");
                OnPCLExamTestItemRefScaleEngChanged();
            }
        }
        private String _PCLExamTestItemRefScaleEng;
        partial void OnPCLExamTestItemRefScaleEngChanging(String value);
        partial void OnPCLExamTestItemRefScaleEngChanged();
        //▲==== #001
    }
}
