/*
 * 20180602 #001 TTM:
 * 20220905 #002 BLQ: Thêm trường kết luận vào cls để load mặc định
 * 20230519 #003 DatTB: Thêm cột tên tiếng anh các danh mục xét nghiệm 
 */
using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text;

namespace DataEntities
{
    public partial class PCLExamType : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new PCLExamType object.

        /// <param name="pCLExamTypeID">Initial value of the PCLExamTypeID property.</param>
        /// <param name="pCLExamTypeName">Initial value of the PCLExamTypeName property.</param>
        public static PCLExamType CreatePCLExamType(Int64 pCLExamTypeID, String pCLExamTypeName)
        {
            PCLExamType pCLExamType = new PCLExamType();
            pCLExamType.PCLExamTypeID = pCLExamTypeID;
            pCLExamType.PCLExamTypeName = pCLExamTypeName;
            return pCLExamType;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 PCLExamTypeID
        {
            get
            {
                return _PCLExamTypeID;
            }
            set
            {
                if (_PCLExamTypeID != value)
                {
                    OnPCLExamTypeIDChanging(value);
                    _PCLExamTypeID = value;
                    RaisePropertyChanged("PCLExamTypeID");
                    OnPCLExamTypeIDChanged();
                }
            }
        }
        private Int64 _PCLExamTypeID;
        partial void OnPCLExamTypeIDChanging(Int64 value);
        partial void OnPCLExamTypeIDChanged();


        [DataMemberAttribute()]
        public Nullable<Int64> IDCode
        {
            get
            {
                return _IDCode;
            }
            set
            {
                OnIDCodeChanging(value);
                _IDCode = value;
                RaisePropertyChanged("IDCode");
                OnIDCodeChanged();
            }
        }
        private Nullable<Int64> _IDCode;
        partial void OnIDCodeChanging(Nullable<Int64> value);
        partial void OnIDCodeChanged();


        [DataMemberAttribute()]
        public Nullable<Int64> PCLExamTypeSubCategoryID
        {
            get { return _PCLExamTypeSubCategoryID; }
            set
            {
                if (_PCLExamTypeSubCategoryID != value)
                {
                    OnPCLExamTypeSubCategoryIDChanging(value);
                    _PCLExamTypeSubCategoryID = value;
                    RaisePropertyChanged("PCLExamTypeSubCategoryID");
                    OnPCLExamTypeSubCategoryIDChanged();
                }
            }
        }
        private Nullable<Int64> _PCLExamTypeSubCategoryID;
        partial void OnPCLExamTypeSubCategoryIDChanging(Nullable<Int64> value);
        partial void OnPCLExamTypeSubCategoryIDChanged();


        [DataMemberAttribute()]
        public PCLExamTypeSubCategory ObjPCLExamTypeSubCategoryID
        {
            get { return _ObjPCLExamTypeSubCategoryID; }
            set
            {
                if (_ObjPCLExamTypeSubCategoryID != value)
                {
                    OnObjPCLExamTypeSubCategoryIDChanging(value);
                    _ObjPCLExamTypeSubCategoryID = value;
                    RaisePropertyChanged("ObjPCLExamTypeSubCategoryID");
                    OnObjPCLExamTypeSubCategoryIDChanged();
                }
            }
        }
        private PCLExamTypeSubCategory _ObjPCLExamTypeSubCategoryID;
        partial void OnObjPCLExamTypeSubCategoryIDChanging(PCLExamTypeSubCategory value);
        partial void OnObjPCLExamTypeSubCategoryIDChanged();


        //[DataMemberAttribute()]
        //public Nullable<long> PCLExamGroupID
        //{
        //    get
        //    {
        //        return _PCLExamGroupID;
        //    }
        //    set
        //    {
        //        OnPCLExamGroupIDChanging(value);
        //        _PCLExamGroupID = value;
        //        RaisePropertyChanged("PCLExamGroupID");
        //        OnPCLExamGroupIDChanged();
        //    }
        //}
        //private Nullable<long> _PCLExamGroupID;
        //partial void OnPCLExamGroupIDChanging(Nullable<long> value);
        //partial void OnPCLExamGroupIDChanged();

        [Required(ErrorMessage = "Nhập Tên PCLExamType!")]
        [DataMemberAttribute()]
        public String PCLExamTypeName
        {
            get
            {
                return _PCLExamTypeName;
            }
            set
            {
                OnPCLExamTypeNameChanging(value);
                ValidateProperty("PCLExamTypeName", value);
                _PCLExamTypeName = value;
                RaisePropertyChanged("PCLExamTypeName");
                OnPCLExamTypeNameChanged();
            }
        }
        private String _PCLExamTypeName;
        partial void OnPCLExamTypeNameChanging(String value);
        partial void OnPCLExamTypeNameChanged();

        //▼==== #003
        [DataMemberAttribute()]
        public String PCLExamTypeNameEng
        {
            get
            {
                return _PCLExamTypeNameEng;
            }
            set
            {
                OnPCLExamTypeNameEngChanging(value);
                ValidateProperty("PCLExamTypeNameEng", value);
                _PCLExamTypeNameEng = value;
                RaisePropertyChanged("PCLExamTypeNameEng");
                OnPCLExamTypeNameEngChanged();
            }
        }
        private String _PCLExamTypeNameEng;
        partial void OnPCLExamTypeNameEngChanging(String value);
        partial void OnPCLExamTypeNameEngChanged();
        //▲==== #003

        //▼====: #002
        [DataMemberAttribute]
        public string PCLExamTypeTemplateResult
        {
            get { return _PCLExamTypeTemplateResult; }
            set
            {
                if (_PCLExamTypeTemplateResult != value)
                {
                    _PCLExamTypeTemplateResult = value;
                    RaisePropertyChanged("PCLExamTypeTemplateResult");
                }
            }
        }
        private string _PCLExamTypeTemplateResult;
        //▲====: #002

        [DataMemberAttribute()]
        public String PCLExamTypeDescription
        {
            get
            {
                return _PCLExamTypeDescription;
            }
            set
            {
                OnPCLExamTypeDescriptionChanging(value);
                _PCLExamTypeDescription = value;
                RaisePropertyChanged("PCLExamTypeDescription");
                OnPCLExamTypeDescriptionChanged();
            }
        }
        private String _PCLExamTypeDescription;
        partial void OnPCLExamTypeDescriptionChanging(String value);
        partial void OnPCLExamTypeDescriptionChanged();

        [Required(ErrorMessage = "Nhập Mã PCLExamType!")]
        [DataMemberAttribute()]
        public String PCLExamTypeCode
        {
            get
            {
                return _PCLExamTypeCode;
            }
            set
            {
                OnPCLExamTypeCodeChanging(value);
                ValidateProperty("PCLExamTypeCode", value);
                _PCLExamTypeCode = value;
                RaisePropertyChanged("PCLExamTypeCode");
                OnPCLExamTypeCodeChanged();
            }
        }
        private String _PCLExamTypeCode;
        partial void OnPCLExamTypeCodeChanging(String value);
        partial void OnPCLExamTypeCodeChanged();



        [Required(ErrorMessage = "Chọn Đơn Vị Tính!")]
        [DataMemberAttribute()]
        public Int64 V_PCLExamTypeUnit
        {
            get { return _V_PCLExamTypeUnit; }
            set
            {
                if (_V_PCLExamTypeUnit != value)
                {
                    OnV_PCLExamTypeUnitChanging(value);
                    ValidateProperty("V_PCLExamTypeUnit", value);
                    _V_PCLExamTypeUnit = value;
                    RaisePropertyChanged("V_PCLExamTypeUnit");
                    OnV_PCLExamTypeUnitChanged();
                }
            }
        }
        private Int64 _V_PCLExamTypeUnit;
        partial void OnV_PCLExamTypeUnitChanging(Int64 value);
        partial void OnV_PCLExamTypeUnitChanged();


        [DataMemberAttribute()]
        public Lookup ObjV_PCLExamTypeUnit
        {
            get { return _ObjV_PCLExamTypeUnit; }
            set
            {
                if (_ObjV_PCLExamTypeUnit != value)
                {
                    OnObjV_PCLExamTypeUnitChanging(value);
                    _ObjV_PCLExamTypeUnit = value;
                    RaisePropertyChanged("ObjV_PCLExamTypeUnit");
                    OnObjV_PCLExamTypeUnitChanged();
                }
            }
        }
        private Lookup _ObjV_PCLExamTypeUnit;
        partial void OnObjV_PCLExamTypeUnitChanging(Lookup value);
        partial void OnObjV_PCLExamTypeUnitChanged();




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
        public Nullable<Boolean> HIApproved
        {
            get
            {
                return _HIApproved;
            }
            set
            {
                OnHIApprovedChanging(value);
                _HIApproved = value;
                RaisePropertyChanged("HIApproved");
                OnHIApprovedChanged();
            }
        }
        private Nullable<Boolean> _HIApproved;
        partial void OnHIApprovedChanging(Nullable<Boolean> value);
        partial void OnHIApprovedChanged();


        [DataMemberAttribute()]
        public Nullable<Boolean> IsExternalExam
        {
            get
            {
                return _IsExternalExam;
            }
            set
            {
                OnIsExternalExamChanging(value);
                _IsExternalExam = value;
                RaisePropertyChanged("IsExternalExam");
                OnIsExternalExamChanged();
            }
        }
        private Nullable<Boolean> _IsExternalExam;
        partial void OnIsExternalExamChanging(Nullable<Boolean> value);
        partial void OnIsExternalExamChanged();


        [DataMemberAttribute()]
        public Nullable<long> PCLFormID
        {
            get
            {
                return _PCLFormID;
            }
            set
            {
                OnPCLFormIDChanging(value);
                _PCLFormID = value;
                RaisePropertyChanged("PCLFormID");
                OnPCLFormIDChanged();
            }
        }
        private Nullable<long> _PCLFormID;
        partial void OnPCLFormIDChanging(Nullable<long> value);
        partial void OnPCLFormIDChanged();

        //HosIDofExternalExam

        [DataMemberAttribute()]
        public Nullable<long> HosIDofExternalExam
        {
            get
            {
                return _HosIDofExternalExam;
            }
            set
            {
                OnHosIDofExternalExamChanging(value);
                _HosIDofExternalExam = value;
                RaisePropertyChanged("HosIDofExternalExam");
                OnHosIDofExternalExamChanged();
            }
        }
        private Nullable<long> _HosIDofExternalExam;
        partial void OnHosIDofExternalExamChanging(Nullable<long> value);
        partial void OnHosIDofExternalExamChanged();

        [DataMemberAttribute()]
        public string HICode
        {
            get
            {
                return _HICode;
            }
            set
            {
                if (_HICode != value)
                {
                    OnHICodeChanging(value);
                    _HICode = value;
                    RaisePropertyChanged("HICode");
                    OnHICodeChanged();
                }
            }
        }
        private string _HICode;
        partial void OnHICodeChanging(string value);
        partial void OnHICodeChanged();

        [DataMemberAttribute()]
        public string PCLExamTypeName_Ax
        {
            get
            {
                return _PCLExamTypeName_Ax;
            }
            set
            {
                if (_PCLExamTypeName_Ax != value)
                {
                    OnPCLExamTypeName_AxChanging(value);
                    _PCLExamTypeName_Ax = value;
                    RaisePropertyChanged("PCLExamTypeName_Ax");
                    OnHICodeChanged();
                }
            }
        }
        private string _PCLExamTypeName_Ax;
        partial void OnPCLExamTypeName_AxChanging(string value);
        partial void OnPCLExamTypeName_AxChanged();

        [DataMemberAttribute()]
        public string HIIssueCode1
        {
            get
            {
                return _HIIssueCode1;
            }
            set
            {
                if (_HIIssueCode1 != value)
                {
                    OnHIIssueCode1Changing(value);
                    _HIIssueCode1 = value;
                    RaisePropertyChanged("HIIssueCode1");
                    OnHIIssueCode1Changed();
                }
            }
        }
        private string _HIIssueCode1;
        partial void OnHIIssueCode1Changing(string value);
        partial void OnHIIssueCode1Changed();

        [DataMemberAttribute()]
        public string HIIssueCode2
        {
            get
            {
                return _HIIssueCode2;
            }
            set
            {
                if (_HIIssueCode2 != value)
                {
                    OnHIIssueCode2Changing(value);
                    _HIIssueCode2 = value;
                    RaisePropertyChanged("HIIssueCode2");
                    OnHIIssueCode2Changed();
                }
            }
        }
        private string _HIIssueCode2;
        partial void OnHIIssueCode2Changing(string value);
        partial void OnHIIssueCode2Changed();

        #region Backup
        /*
        [DataMemberAttribute()]
        public Nullable<Double> PCLIndMinValue
        {
            get
            {
                return _PCLIndMinValue;
            }
            set
            {
                OnPCLIndMinValueChanging(value);
                _PCLIndMinValue = value;
                RaisePropertyChanged("PCLIndMinValue");
                OnPCLIndMinValueChanged();
            }
        }
        private Nullable<Double> _PCLIndMinValue;
        partial void OnPCLIndMinValueChanging(Nullable<Double> value);
        partial void OnPCLIndMinValueChanged();

        [DataMemberAttribute()]
        public Nullable<Double> PCLIndMaxValue
        {
            get
            {
                return _PCLIndMaxValue;
            }
            set
            {
                OnPCLIndMaxValueChanging(value);
                _PCLIndMaxValue = value;
                RaisePropertyChanged("PCLIndMaxValue");
                OnPCLIndMaxValueChanged();
            }
        }
        private Nullable<Double> _PCLIndMaxValue;
        partial void OnPCLIndMaxValueChanging(Nullable<Double> value);
        partial void OnPCLIndMaxValueChanged();

        [DataMemberAttribute()]
        public Nullable<Double> PCLIndAVGValue
        {
            get
            {
                return _PCLIndAVGValue;
            }
            set
            {
                OnPCLIndAVGValueChanging(value);
                _PCLIndAVGValue = value;
                RaisePropertyChanged("PCLIndAVGValue");
                OnPCLIndAVGValueChanged();
            }
        }
        private Nullable<Double> _PCLIndAVGValue;
        partial void OnPCLIndAVGValueChanging(Nullable<Double> value);
        partial void OnPCLIndAVGValueChanged();

        [DataMemberAttribute()]
        public Nullable<Double> PCLIndOtherValue
        {
            get
            {
                return _PCLIndOtherValue;
            }
            set
            {
                OnPCLIndOtherValueChanging(value);
                _PCLIndOtherValue = value;
                RaisePropertyChanged("PCLIndOtherValue");
                OnPCLIndOtherValueChanged();
            }
        }
        private Nullable<Double> _PCLIndOtherValue;
        partial void OnPCLIndOtherValueChanging(Nullable<Double> value);
        partial void OnPCLIndOtherValueChanged();

        [DataMemberAttribute()]
        public String MeasurementUnit
        {
            get
            {
                return _MeasurementUnit;
            }
            set
            {
                OnMeasurementUnitChanging(value);
                _MeasurementUnit = value;
                RaisePropertyChanged("MeasurementUnit");
                OnMeasurementUnitChanged();
            }
        }
        private String _MeasurementUnit;
        partial void OnMeasurementUnitChanging(String value);
        partial void OnMeasurementUnitChanged();

        */
        #endregion

        [DataMemberAttribute()]
        public long MedServiceID
        {
            get
            {
                return _MedServiceID;
            }
            set
            {
                OnMedServiceIDChanging(value);
                _MedServiceID = value;
                RaisePropertyChanged("MedServiceID");
                OnMedServiceIDChanged();
            }
        }
        private long _MedServiceID;
        partial void OnMedServiceIDChanging(long value);
        partial void OnMedServiceIDChanged();


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
        public bool ExamTypeIsExamTest
        {
            get
            {
                return _ExamTypeIsExamTest;
            }
            set
            {
                OnExamTypeIsExamTestChanging(value);
                _ExamTypeIsExamTest = value;
                RaisePropertyChanged("ExamTypeIsExamTest");
                OnExamTypeIsExamTestChanged();
            }
        }
        private bool _ExamTypeIsExamTest;
        partial void OnExamTypeIsExamTestChanging(bool value);
        partial void OnExamTypeIsExamTestChanged();

        // Hpt 11/11/2015: Thêm các thuộc tính hỗ trợ phân loại giá dịch vụ
        [Required(ErrorMessage = "Chọn Loại giá dịch vụ!")]
        [DataMemberAttribute()]
        public Int32 V_NewPriceType
        {
            get { return _V_NewPriceType; }
            set
            {
                if (_V_NewPriceType != value)
                {
                    OnV_NewPriceTypeChanging(value);
                    ValidateProperty("V_NewPriceType", value);
                    _V_NewPriceType = value;
                    RaisePropertyChanged("V_NewPriceType");
                    OnV_NewPriceTypeChanged();
                }
            }
        }
        private Int32 _V_NewPriceType;
        partial void OnV_NewPriceTypeChanging(Int32 value);
        partial void OnV_NewPriceTypeChanged();

        //▼====: #001
        //Tạo ObservableCollection và hàm convert ObservableCollection này sang XML để lưu xuống Database
        #region ObservableCollection for Resources
        [DataMemberAttribute()]
        public ObservableCollection<Resources> Resource
        {
            get
            {
                return _Resource;
            }
            set
            {
                _Resource = value;
                RaisePropertyChanged("Resource");
            }
        }
        private ObservableCollection<Resources> _Resource;
        public string ConvertDetailsListToXmlForResources()
        {
            return ConvertDetailsListToXmlForResources(_Resource);
        }
        public string ConvertDetailsListToXmlForResources(IEnumerable<Resources> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<Resource>");
                foreach (Resources details in items)
                {
                    if (details.RscrID > 0)
                    {
                        sb.Append("<RecInfo>");
                        sb.AppendFormat("<RscrID>{0}</RscrID>", details.RscrID);
                        sb.Append("</RecInfo>");
                    }
                }
                sb.Append("</Resource>");
                return sb.ToString();
            }
            else
                return null;
        }
        #endregion
        //▲====: #001


        #endregion

        #region Navigation Properties
        private ObservableCollection<PCLExamTypeLocation> _pclExamTypeLocations;
        [DataMemberAttribute()]
        public ObservableCollection<PCLExamTypeLocation> PCLExamTypeLocations
        {
            get
            {
                return _pclExamTypeLocations;
            }
            set
            {
                _pclExamTypeLocations = value;
                RaisePropertyChanged("PCLExamTypeLocations");
            }
        }

        [DataMemberAttribute()]
        public Int64 HITTypeID
        {
            get
            {
                return _HITTypeID;
            }
            set
            {
                if (_HITTypeID != value)
                {
                    OnHITTypeIDChanging(value);
                    _HITTypeID = value;
                    RaisePropertyChanged("HITTypeID");
                    OnHITTypeIDChanged();
                }
            }
        }
        private Int64 _HITTypeID;
        partial void OnHITTypeIDChanging(Int64 value);
        partial void OnHITTypeIDChanged();


        [DataMemberAttribute()]
        public HITransactionType ObjHITTypeID
        {
            get
            {
                return _ObjHITTypeID;
            }
            set
            {
                if (_ObjHITTypeID != value)
                {
                    OnObjHITTypeIDChanging(value);
                    _ObjHITTypeID = value;
                    RaisePropertyChanged("ObjHITTypeID");
                    OnObjHITTypeIDChanged();
                }
            }
        }
        private HITransactionType _ObjHITTypeID;
        partial void OnObjHITTypeIDChanging(HITransactionType value);
        partial void OnObjHITTypeIDChanged();
        //HI


        [DataMemberAttribute()]
        public DiseasesReference DiseasesReference
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<PatientPCLImagingResult> PatientPCLExamResults
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public PCLExamGroup PCLExamGroup
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<PCLItem> PCLItems
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<RefMedicalServiceItem> RefMedicalServiceItems
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        private PCLExamTypePrice _ObjPCLExamTypePrice;
        public PCLExamTypePrice ObjPCLExamTypePrice
        {
            get { return _ObjPCLExamTypePrice; }
            set
            {
                OnObjPCLExamTypePriceChanging(value);
                _ObjPCLExamTypePrice = value;
                RaisePropertyChanged("ObjPCLExamTypePrice");
                OnObjPCLExamTypePriceChanged();
            }
        }
        partial void OnObjPCLExamTypePriceChanging(PCLExamTypePrice value);
        partial void OnObjPCLExamTypePriceChanged();


        [DataMemberAttribute()]
        public Int64 V_PCLMainCategory
        {
            get { return _V_PCLMainCategory; }
            set
            {
                if (_V_PCLMainCategory != value)
                {
                    OnV_PCLMainCategoryChanging(value);
                    _V_PCLMainCategory = value;
                    RaisePropertyChanged("V_PCLMainCategory");
                    OnV_PCLMainCategoryChanged();
                }
            }
        }
        private Int64 _V_PCLMainCategory;
        partial void OnV_PCLMainCategoryChanging(Int64 value);
        partial void OnV_PCLMainCategoryChanged();

        [DataMemberAttribute()]
        public Lookup ObjV_PCLMainCategory
        {
            get { return _ObjV_PCLMainCategory; }
            set
            {
                if (_ObjV_PCLMainCategory != value)
                {
                    OnObjV_PCLMainCategoryChanging(value);
                    _ObjV_PCLMainCategory = value;
                    RaisePropertyChanged("ObjV_PCLMainCategory");
                    OnObjV_PCLMainCategoryChanged();
                }
            }
        }
        private Lookup _ObjV_PCLMainCategory;
        partial void OnObjV_PCLMainCategoryChanging(Lookup value);
        partial void OnObjV_PCLMainCategoryChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> PCLSectionID
        {
            get { return _PCLSectionID; }
            set
            {
                if (_PCLSectionID != value)
                {
                    OnPCLSectionIDChanging(value);
                    _PCLSectionID = value;
                    RaisePropertyChanged("PCLSectionID");
                    OnPCLSectionIDChanged();
                }
            }
        }
        private Nullable<Int64> _PCLSectionID;
        partial void OnPCLSectionIDChanging(Nullable<Int64> value);
        partial void OnPCLSectionIDChanged();


        [DataMemberAttribute()]
        public PCLSection ObjPCLSectionID
        {
            get { return _ObjPCLSectionID; }
            set
            {
                if (_ObjPCLSectionID != value)
                {
                    OnObjPCLSectionIDChanging(value);
                    _ObjPCLSectionID = value;
                    RaisePropertyChanged("ObjPCLSectionID");
                    OnObjPCLSectionIDChanged();

                }
            }
        }
        private PCLSection _ObjPCLSectionID;
        partial void OnObjPCLSectionIDChanging(PCLSection value);
        partial void OnObjPCLSectionIDChanged();


        [DataMemberAttribute()]
        public String PCLSectionName
        {
            get { return _PCLSectionName; }
            set
            {
                if (_PCLSectionName != value)
                {
                    OnPCLSectionNameChanging(value);
                    _PCLSectionName = value;
                    RaisePropertyChanged("PCLSectionName");
                    OnPCLSectionNameChanged();
                }
            }
        }
        private String _PCLSectionName;
        partial void OnPCLSectionNameChanging(String value);
        partial void OnPCLSectionNameChanged();


        /*PCLResultParamImpID: kết quả chi tiết 1 xét nghiệm (vd kết quả: siêu âm màu)*/
        [DataMemberAttribute]
        public Nullable<Int64> PCLResultParamImpID
        {
            get { return _PCLResultParamImpID; }
            set
            {
                if (_PCLResultParamImpID != value)
                {
                    OnPCLResultParamImpIDChanging(value);
                    _PCLResultParamImpID = value;
                    RaisePropertyChanged("PCLResultParamImpID");
                    OnPCLResultParamImpIDChanged();
                }
            }
        }
        private Nullable<Int64> _PCLResultParamImpID;
        partial void OnPCLResultParamImpIDChanging(Nullable<Int64> value);
        partial void OnPCLResultParamImpIDChanged();


        [DataMemberAttribute]
        public PCLResultParamImplementations ObjPCLResultParamImpID
        {
            get { return _ObjPCLResultParamImpID; }
            set
            {
                if (_ObjPCLResultParamImpID != value)
                {
                    OnObjPCLResultParamImpIDChanging(value);
                    _ObjPCLResultParamImpID = value;
                    RaisePropertyChanged("ObjPCLResultParamImpID");
                    OnObjPCLResultParamImpIDChanged();
                }
            }
        }
        private PCLResultParamImplementations _ObjPCLResultParamImpID;
        partial void OnObjPCLResultParamImpIDChanging(PCLResultParamImplementations value);
        partial void OnObjPCLResultParamImpIDChanged();


        //Navigate
        [DataMemberAttribute]
        public ObservableCollection<PCLExamTypeTestItems> ObjPCLExamTypeTestItemsList
        {
            get { return _ObjPCLExamTypeTestItemsList; }
            set
            {
                if (_ObjPCLExamTypeTestItemsList != value)
                {
                    OnObjPCLExamTestItemsListChanging(value);
                    _ObjPCLExamTypeTestItemsList = value;
                    RaisePropertyChanged("ObjPCLExamTypeTestItemsList");
                    OnObjPCLExamTestItemsListChanged();
                }
            }
        }
        private ObservableCollection<PCLExamTypeTestItems> _ObjPCLExamTypeTestItemsList;
        partial void OnObjPCLExamTestItemsListChanging(ObservableCollection<PCLExamTypeTestItems> value);
        partial void OnObjPCLExamTestItemsListChanged();



        //cls so 

        [DataMemberAttribute()]
        private PCLExamTypeServiceTarget _ObjPCLExamTypeServiceTarget;
        public PCLExamTypeServiceTarget ObjPCLExamTypeServiceTarget
        {
            get { return _ObjPCLExamTypeServiceTarget; }
            set
            {
                _ObjPCLExamTypeServiceTarget = value;
                RaisePropertyChanged("ObjPCLExamTypeServiceTarget");
            }
        }

        //Navigate

        #endregion

        public override bool Equals(object obj)
        {
            PCLExamType SelectedExamType = obj as PCLExamType;
            if (SelectedExamType == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PCLExamTypeID == SelectedExamType.PCLExamTypeID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        //Danh sách Phòng làm ExamType này
        private ObservableCollection<DeptLocation> _ObjDeptLocationList;
        [DataMemberAttribute()]
        public ObservableCollection<DeptLocation> ObjDeptLocationList
        {
            get
            {
                return _ObjDeptLocationList;
            }
            set
            {
                _ObjDeptLocationList = value;
                RaisePropertyChanged("ObjDeptLocationList");
            }
        }
        //Danh sách Phòng làm ExamType này
        
        //Hpt 24/08/2016: Vv Cho phép người dùng điều chỉnh mã dùng chung theo công văn 5084
        private string _HICode5084;
        public string HICode5084
        {
            get
            {
                return _HICode5084;
            }
            set
            {
                _HICode5084 = value;
                RaisePropertyChanged("HICode5084");
            }
        }

        private int _IsAllowToPayAfter;
        [DataMemberAttribute()]
        public int IsAllowToPayAfter
        {
            get
            {
                return _IsAllowToPayAfter;
            }
            set
            {
                if (_IsAllowToPayAfter != value)
                {
                    _IsAllowToPayAfter = value;
                    RaisePropertyChanged("IsAllowToPayAfter");
                }
            }
        }
        private long _V_ReportForm;
        [DataMemberAttribute()]
        public long V_ReportForm
        {
            get
            {
                return _V_ReportForm;
            }
            set
            {
                if (_V_ReportForm != value)
                {
                    _V_ReportForm = value;
                    RaisePropertyChanged("V_ReportForm");
                }
            }
        }

        private string _TemplateFileName;
        [DataMemberAttribute]
        public string TemplateFileName
        {
            get => _TemplateFileName; set
            {
                _TemplateFileName = value;
                RaisePropertyChanged("TemplateFileName");
            }
        }

        private int _AllowDayBetweenExams;
        [DataMemberAttribute]
        public int AllowDayBetweenExams
        {
            get => _AllowDayBetweenExams; set
            {
                _AllowDayBetweenExams = value;
                RaisePropertyChanged("AllowDayBetweenExams");
            }
        }

        private bool _IsRegimenChecking = false;
        public bool IsRegimenChecking
        {
            get
            {
                return _IsRegimenChecking;
            }
            set
            {
                _IsRegimenChecking = value;
                RaisePropertyChanged("IsRegimenChecking");
            }
        }

        [DataMemberAttribute()]
        public bool IsUsed
        {
            get
            {
                return _IsUsed;
            }
            set
            {
                _IsUsed = value;
                RaisePropertyChanged("IsUsed");
            }
        }
        private bool _IsUsed;

        private string _ModalityCode;
        [DataMemberAttribute]
        public string ModalityCode
        {
            get
            {
                return _ModalityCode;
            }
            set
            {
                _ModalityCode = value;
                RaisePropertyChanged("ModalityCode");
            }
        }
        private bool _IsCasePermitted = false;
        [DataMemberAttribute]
        public bool IsCasePermitted
        {
            get
            {
                return _IsCasePermitted;
            }
            set
            {
                if (_IsCasePermitted == value)
                {
                    return;
                }
                _IsCasePermitted = value;
                RaisePropertyChanged("IsCasePermitted");
            }
        }
        private bool _InCategoryCOVID;
        [DataMemberAttribute]
        public bool InCategoryCOVID
        {
            get
            {
                return _InCategoryCOVID;
            }
            set
            {
                if (_InCategoryCOVID == value)
                {
                    return;
                }
                _InCategoryCOVID = value;
                RaisePropertyChanged("InCategoryCOVID");
            }
        }

        private bool _IsAllowEditAfterDischarge;
        [DataMemberAttribute]
        public bool IsAllowEditAfterDischarge
        {
            get
            {
                return _IsAllowEditAfterDischarge;
            }
            set
            {
                if (_IsAllowEditAfterDischarge == value)
                {
                    return;
                }
                _IsAllowEditAfterDischarge = value;
                RaisePropertyChanged("IsAllowEditAfterDischarge");
            }
        }

        private int _DateAllowEditAfterDischarge;
        [DataMemberAttribute]
        public int DateAllowEditAfterDischarge
        {
            get
            {
                return _DateAllowEditAfterDischarge;
            }
            set
            {
                if (_DateAllowEditAfterDischarge == value)
                {
                    return;
                }
                _DateAllowEditAfterDischarge = value;
                RaisePropertyChanged("DateAllowEditAfterDischarge");
            }
        }
    }

    public partial class PCLExamTypeServiceTarget : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new PCLExamType object.

        /// <param name="pCLExamTypeID">Initial value of the PCLExamTypeID property.</param>
        /// <param name="pCLExamTypeName">Initial value of the PCLExamTypeName property.</param>
        public static PCLExamTypeServiceTarget CreatePCLExamTypeServiceTarget(Int64 PCLExamTypeServiceTargetID, Int64 pCLExamTypeID)
        {
            PCLExamTypeServiceTarget pCLExamType = new PCLExamTypeServiceTarget();
            pCLExamType.PCLExamTypeID = pCLExamTypeID;
            pCLExamType.PCLExamTypeServiceTargetID = PCLExamTypeServiceTargetID;
            return pCLExamType;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 PCLExamTypeServiceTargetID
        {
            get { return _PCLExamTypeServiceTargetID; }
            set
            {
                if (_PCLExamTypeServiceTargetID != value)
                {
                    OnPCLExamTypeServiceTargetIDChanging(value);
                    _PCLExamTypeServiceTargetID = value;
                    RaisePropertyChanged("PCLExamTypeServiceTargetID");
                    OnPCLExamTypeServiceTargetIDChanged();
                }
            }
        }
        private Int64 _PCLExamTypeServiceTargetID;
        partial void OnPCLExamTypeServiceTargetIDChanging(Int64 value);
        partial void OnPCLExamTypeServiceTargetIDChanged();


        [DataMemberAttribute()]
        public Int64 PCLExamTypeID
        {
            get
            {
                return _PCLExamTypeID;
            }
            set
            {
                if (_PCLExamTypeID != value)
                {
                    OnPCLExamTypeIDChanging(value);
                    _PCLExamTypeID = value;
                    RaisePropertyChanged("PCLExamTypeID");
                    OnPCLExamTypeIDChanged();
                }
            }
        }
        private Int64 _PCLExamTypeID;
        partial void OnPCLExamTypeIDChanging(Int64 value);
        partial void OnPCLExamTypeIDChanged();

        [DataMemberAttribute()]
        public int MondayTargetNumberOfCases
        {
            get
            {
                return _MondayTargetNumberOfCases;
            }
            set
            {
                OnMondayTargetNumberOfCasesChanging(value);
                _MondayTargetNumberOfCases = value;
                RaisePropertyChanged("MondayTargetNumberOfCases");
                OnMondayTargetNumberOfCasesChanged();
            }
        }
        private int _MondayTargetNumberOfCases;
        partial void OnMondayTargetNumberOfCasesChanging(int value);
        partial void OnMondayTargetNumberOfCasesChanged();

        [DataMemberAttribute()]
        public int TuesdayTargetNumberOfCases
        {
            get
            {
                return _TuesdayTargetNumberOfCases;
            }
            set
            {
                OnTuesdayTargetNumberOfCasesChanging(value);
                _TuesdayTargetNumberOfCases = value;
                RaisePropertyChanged("TuesdayTargetNumberOfCases");
                OnTuesdayTargetNumberOfCasesChanged();
            }
        }
        private int _TuesdayTargetNumberOfCases;
        partial void OnTuesdayTargetNumberOfCasesChanging(int value);
        partial void OnTuesdayTargetNumberOfCasesChanged();

        [DataMemberAttribute()]
        public int WednesdayTargetNumberOfCases
        {
            get
            {
                return _WednesdayTargetNumberOfCases;
            }
            set
            {
                OnWednesdayTargetNumberOfCasesChanging(value);
                _WednesdayTargetNumberOfCases = value;
                RaisePropertyChanged("WednesdayTargetNumberOfCases");
                OnWednesdayTargetNumberOfCasesChanged();
            }
        }
        private int _WednesdayTargetNumberOfCases;
        partial void OnWednesdayTargetNumberOfCasesChanging(int value);
        partial void OnWednesdayTargetNumberOfCasesChanged();

        [DataMemberAttribute()]
        public int ThursdayTargetNumberOfCases
        {
            get
            {
                return _ThursdayTargetNumberOfCases;
            }
            set
            {
                OnThursdayTargetNumberOfCasesChanging(value);
                _ThursdayTargetNumberOfCases = value;
                RaisePropertyChanged("ThursdayTargetNumberOfCases");
                OnThursdayTargetNumberOfCasesChanged();
            }
        }
        private int _ThursdayTargetNumberOfCases;
        partial void OnThursdayTargetNumberOfCasesChanging(int value);
        partial void OnThursdayTargetNumberOfCasesChanged();

        [DataMemberAttribute()]
        public int FridayTargetNumberOfCases
        {
            get
            {
                return _FridayTargetNumberOfCases;
            }
            set
            {
                OnFridayTargetNumberOfCasesChanging(value);
                _FridayTargetNumberOfCases = value;
                RaisePropertyChanged("FridayTargetNumberOfCases");
                OnFridayTargetNumberOfCasesChanged();
            }
        }
        private int _FridayTargetNumberOfCases;
        partial void OnFridayTargetNumberOfCasesChanging(int value);
        partial void OnFridayTargetNumberOfCasesChanged();

        [DataMemberAttribute()]
        public int SaturdayTargetNumberOfCases
        {
            get
            {
                return _SaturdayTargetNumberOfCases;
            }
            set
            {
                OnSaturdayTargetNumberOfCasesChanging(value);
                _SaturdayTargetNumberOfCases = value;
                RaisePropertyChanged("SaturdayTargetNumberOfCases");
                OnSaturdayTargetNumberOfCasesChanged();
            }
        }
        private int _SaturdayTargetNumberOfCases;
        partial void OnSaturdayTargetNumberOfCasesChanging(int value);
        partial void OnSaturdayTargetNumberOfCasesChanged();

        [DataMemberAttribute()]
        public int SundayTargetNumberOfCases
        {
            get
            {
                return _SundayTargetNumberOfCases;
            }
            set
            {
                OnSundayTargetNumberOfCasesChanging(value);
                _SundayTargetNumberOfCases = value;
                RaisePropertyChanged("SundayTargetNumberOfCases");
                OnSundayTargetNumberOfCasesChanged();
            }
        }
        private int _SundayTargetNumberOfCases;
        partial void OnSundayTargetNumberOfCasesChanging(int value);
        partial void OnSundayTargetNumberOfCasesChanged();
        #endregion

        #region Ext member

        [DataMemberAttribute()]
        public String PCLExamTypeName
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
        private String _PCLExamTypeName;
        partial void OnPCLExamTypeNameChanging(String value);
        partial void OnPCLExamTypeNameChanged();

        [DataMemberAttribute()]
        public String PCLExamTypeCode
        {
            get
            {
                return _PCLExamTypeCode;
            }
            set
            {
                OnPCLExamTypeCodeChanging(value);
                _PCLExamTypeCode = value;
                RaisePropertyChanged("PCLExamTypeCode");
                OnPCLExamTypeCodeChanged();
            }
        }
        private String _PCLExamTypeCode;
        partial void OnPCLExamTypeCodeChanging(String value);
        partial void OnPCLExamTypeCodeChanged();    

        #endregion

    }
}