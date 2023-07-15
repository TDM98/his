using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Collections.Generic;
using Service.Core.Common;
using System.Text.RegularExpressions;

/*
 * 20180920 #001 TBL:   Chinh sua chuc nang bug mantis ID 0000061, kiem tra tung property trong dataentities 
 *                      neu co su thay doi IsDataChanged = true
 */
namespace DataEntities
{
    public partial class PrescriptionDetail : EntityBase
    {
        private EntityState _EntityState = EntityState.NEW;
        //private int xNgayBHToiDa_NgoaiTru = 30;
        //private int xNgayBHToiDa_NoiTru = 5;

        [DataMemberAttribute()]
        public override EntityState EntityState
        {
            get
            {
                return _EntityState;
            }
            set
            {
                _EntityState = value;
                RaisePropertyChanged("EntityState");
            }
        }

        #region ExtendedDataMember

        [DataMemberAttribute()]
        public double RealQty
        {
            get
            {
                return _RealQty;
            }
            set
            {
                OnRealQtyChanging(value);
                _RealQty = value;
                RaisePropertyChanged("RealQty");
                OnRealQtyChanged();
            }
        }
        private double _RealQty;
        partial void OnRealQtyChanging(double value);
        partial void OnRealQtyChanged();

        [DataMemberAttribute()]
        public Nullable<bool> IsInsurance
        {
            get
            {
                return _IsInsurance;
            }
            set
            {
                _IsInsurance = value;
                RaisePropertyChanged("IsInsurance");
            }
        }
        private Nullable<bool> _IsInsurance;

        private Single _dosage;
        partial void OndosageChanging(Single value);
        partial void OndosageChanged(Single value);


       //[Range(0.0, 99999999999.0, ErrorMessage = "Liều dùng không được < 0")]
        [DataMemberAttribute()]
        public Single dosage
        {
            get
            {
                return _dosage;
            }
            set
            {
                if (_dosage != value)
                {
                    OndosageChanging(value);
                    _dosage = value;
                    OndosageChanged(value);
                    RaisePropertyChanged("dosage");

                    //RaisePropertyChanged("MDose");
                    //RaisePropertyChanged("ADose");
                    //RaisePropertyChanged("EDose");
                    //RaisePropertyChanged("NDose");
                }
            }
        }

       // [CustomValidation(typeof(PrescriptionDetail), "ValidateDosage")]
        [DataMemberAttribute()]
        public string dosageStr
        {
            get
            {
                return _dosageStr;
            }
            set
            {
                if (_dosageStr != value)
                {
                   // ValidateProperty("dosageStr", value);
                    _dosageStr = value;
                    RaisePropertyChanged("dosageStr");
                }
            }
        }
        private string _dosageStr;

        [DataMemberAttribute()]
        public string MDoseStr
        {
            get
            {
                return _MDoseStr;
            }
            set
            {
                if(_MDoseStr != value)
                {
                    /*▼====: #001*/
                    if ((IsObjectBeingUsedByClient) && _MDoseStr != value)
                    {
                        IsDataChanged = true;
                    }
                    /*▲====: #001*/
                    _MDoseStr = value;
                    //KMx: Trên giao diện đã tính rồi. Tránh tính lại nhiều lần (10/06/2014 15:24).
                    //MDose = ChangeDoseStringToFloat(_MDoseStr);
                    RaisePropertyChanged("MDoseStr");
                }
            }
        }
        private string _MDoseStr;


        [DataMemberAttribute()]
        public string ADoseStr
        {
            get
            {
                return _ADoseStr;
            }
            set
            {
                if (_ADoseStr != value)
                {
                    /*▼====: #001*/
                    if ((IsObjectBeingUsedByClient) && _ADoseStr != value)
                    {
                        IsDataChanged = true;
                    }
                    /*▲====: #001*/
                    _ADoseStr = value;
                    //KMx: Trên giao diện đã tính rồi. Tránh tính lại nhiều lần (10/06/2014 15:24).
                    //ADose = ChangeDoseStringToFloat(_ADoseStr);
                    RaisePropertyChanged("ADoseStr");
                }
            }
        }
        private string _ADoseStr;

        [DataMemberAttribute()]
        public string EDoseStr
        {
            get
            {
                return _EDoseStr;
            }
            set
            {
                if (_EDoseStr != value)
                {
                    /*▼====: #001*/
                    if ((IsObjectBeingUsedByClient) && _EDoseStr != value)
                    {
                        IsDataChanged = true;
                    }
                    /*▲====: #001*/
                    _EDoseStr = value;
                    //KMx: Trên giao diện đã tính rồi. Tránh tính lại nhiều lần (10/06/2014 15:24).
                    //EDose = ChangeDoseStringToFloat(_EDoseStr);
                    RaisePropertyChanged("EDoseStr");
                }
            }
        }
        private string _EDoseStr;

        [DataMemberAttribute()]
        public string NDoseStr
        {
            get
            {
                return _NDoseStr;
            }
            set
            {
                if (_NDoseStr != value)
                {
                    /*▼====: #001*/
                    if ((IsObjectBeingUsedByClient) && _NDoseStr != value)
                    {
                        IsDataChanged = true;
                    }
                    /*▲====: #001*/
                    _NDoseStr = value;
                    //KMx: Trên giao diện đã tính rồi. Tránh tính lại nhiều lần (10/06/2014 15:24).
                    //NDose = ChangeDoseStringToFloat(_NDoseStr);
                    RaisePropertyChanged("NDoseStr");
                }
            }
        }
        private string _NDoseStr;

        private GetDrugForSellVisitor _SelectedDrugForPrescription;
        partial void OnSelectedDrugForPrescriptionChanging(GetDrugForSellVisitor value);
        partial void OnGetDrugForPrescriptionChanged();

        [DataMemberAttribute()]
        public GetDrugForSellVisitor SelectedDrugForPrescription
        {
            get
            {
                return _SelectedDrugForPrescription;
            }
            set
            {
                if (_SelectedDrugForPrescription != value)
                {
                    OnSelectedDrugForPrescriptionChanging(value);
                    /*▼====: #001*/
                    if ((IsObjectBeingUsedByClient) && _SelectedDrugForPrescription != value)
                    {
                        IsDataChanged = true;
                    }
                    /*▲====: #001*/
                    _SelectedDrugForPrescription = value;
                    if (_SelectedDrugForPrescription != null)
                    {
                        if (_SelectedDrugForPrescription.Administration!=null)
                        {
                            Administration = _SelectedDrugForPrescription.Administration;
                        }
                        if (_SelectedDrugForPrescription.UnitName != null)
                        {
                            UnitName = _SelectedDrugForPrescription.UnitName;
                        }
                        if (_SelectedDrugForPrescription.UnitUse != null)
                        {
                            UnitUse = _SelectedDrugForPrescription.UnitUse;
                        }
                        _Strength = _SelectedDrugForPrescription.Content;
                        _DrugID = _SelectedDrugForPrescription.DrugID;
                        if (_DrugID>0)
                        {
                            InsuranceCover = SelectedDrugForPrescription.InsuranceCover == null ?
                            false : SelectedDrugForPrescription.InsuranceCover.Value;
                        }
                        
                        if (_SelectedDrugForPrescription.MaxDayPrescribed>0)
                        {
                            if (DayRpts > _SelectedDrugForPrescription.MaxDayPrescribed)
                            {
                                DayRpts=_SelectedDrugForPrescription.MaxDayPrescribed.Value;
                                DayExtended = 0;
                            }
                        }
                    }
                    else
                    {
                        _Strength = "";
                        _DrugID = null;
                    }
                    OnGetDrugForPrescriptionChanged();
                    RaisePropertyChanged("SelectedDrugForPrescription");
                }
            }
        }

        [DataMemberAttribute()]
        public Boolean InsuranceCover
        {
            get
            {
                return _InsuranceCover;
            }
            set
            {
                OnInsuranceCoverChanging(value);
                _InsuranceCover = value;
                RaisePropertyChanged("InsuranceCover");
                OnInsuranceCoverChanged();
            }
        }
        private Boolean _InsuranceCover;
        partial void OnInsuranceCoverChanging(Boolean value);
        partial void OnInsuranceCoverChanged();

        private ChooseDose _ChooseDose;
        partial void OnChooseDoseChanging(ChooseDose value);
        partial void OnChooseDoseChanged(ChooseDose value);

        [DataMemberAttribute()]
        public ChooseDose ChooseDose
        {
            get
            {
                return _ChooseDose;
            }
            set
            {
                if (_ChooseDose != value)
                {
                    OnChooseDoseChanging(value);
                    _ChooseDose = value;
                    OnChooseDoseChanged(value);
                    RaisePropertyChanged("ChooseDose");
                    //RaisePropertyChanged("MDose");
                    //RaisePropertyChanged("ADose");

                    //RaisePropertyChanged("EDose");
                    //RaisePropertyChanged("NDose");
                }
            }
        }

        //public static ValidationResult ValidateDosage(string value, ValidationContext context)
        //{
        //    if (!string.IsNullOrEmpty(value))
        //    {
        //        if (value.Contains("/"))
        //        {
        //            string pattern = @"\b[\d]+/[\d]+\b";
        //            if (!Regex.IsMatch(value, pattern))
        //            {
        //                return new ValidationResult("Liều dùng không hợp lệ tét", new string[] { "dosageStr" });
        //            }
        //        }
        //        else
        //        {
        //            float outvalue = 0;
        //            try
        //            {
        //                outvalue = float.Parse(value);
        //                if (outvalue < 0)
        //                {
        //                    return new ValidationResult("Liều dùng không được < 0 tét", new string[] { "dosageStr" });
        //                }
        //            }
        //            catch
        //            {
        //                return new ValidationResult("Liều dùng không hợp lệ tét", new string[] { "dosageStr" });
        //            }
        //        }
        //    }
        //    return ValidationResult.Success;
        //}


        //KMx: Trên giao diện đã tính rồi. Tránh tính lại nhiều lần (10/06/2014 15:24).
        //private float ChangeDoseStringToFloat(string value)
        //{
        //    float result = 0;
        //    if (!string.IsNullOrEmpty(value))
        //    {
        //        if (value.Contains("/"))
        //        {
        //            string pattern = @"\b[\d]+/[\d]+\b";
        //            if (!Regex.IsMatch(value, pattern))
        //            {
        //                return 0;
        //            }
        //            else
        //            {
        //                string[] items = null;
        //                items = value.Split('/');
        //                if (items.Length > 2 || items.Length == 0)
        //                {
        //                    return 0;
        //                }
        //                else if (float.Parse(items[1]) == 0)
        //                {
        //                    return 0;
        //                }
        //                result = (float)Math.Round((float.Parse(items[0]) / float.Parse(items[1])), 3);
        //                if (result < 0)
        //                {
        //                    return 0;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            try
        //            {
        //                result = float.Parse(value);
        //                if (result < 0)
        //                {
        //                    return 0;
        //                }
        //            }
        //            catch
        //            {
        //                return 0;
        //            }
        //        }
        //    }
        //    return result;
        //}
        #endregion


        #region Drug Type Control

        
        [DataMemberAttribute()]
        public bool isHICheck
        {
            get
            {
                return _isHICheck;
            }
            set
            {
                OnisHICheckChanging(value);
                _isHICheck = value;
                if (!_isHICheck)
                {
                    BeOfHIMedicineList=false;
                    //isEditDosage = false;
                }
                RaisePropertyChanged("isHICheck");
                OnisHICheckChanged();
            }
        }
        private bool _isHICheck = true;        
        partial void OnisHICheckChanging(bool value);
        partial void OnisHICheckChanged();

        [DataMemberAttribute()]
        public bool isEditDosage
        {
            get
            {
                return _isEditDosage;
            }
            set
            {
                OnisEditDosageChanging(value);
                _isEditDosage = value;                
                RaisePropertyChanged("isEditDosage");
                OnisEditDosageChanged();
            }
        }
        private bool _isEditDosage = true;
        partial void OnisEditDosageChanging(bool value);
        partial void OnisEditDosageChanged();


        [DataMemberAttribute()]
        public int Index
        {
            get
            {
                return _Index;
            }
            set
            {
                OnIndexChanging(value);
                /*▼====: #001*/
                if ((IsObjectBeingUsedByClient) && _Index != value)
                {
                    IsDataChanged = true;
                }
                /*▲====: #001*/
                _Index = value;
                RaisePropertyChanged("Index");
                OnIndexChanged();
            }
        }
        private int _Index;
        partial void OnIndexChanging(int value);
        partial void OnIndexChanged();


        //private bool _isWeeks = false;
        //[DataMemberAttribute()]
        //public bool isWeeks
        //{
        //    get
        //    {
        //        return _isWeeks;
        //    }
        //    set
        //    {
        //        _isWeeks = value;
        //        RaisePropertyChanged("isWeeks");
        //    }
        //}

        
        //private bool _isNeedToUse = false;
        [DataMemberAttribute()]
        public bool isNeedToUse
        {
            get
            {
                return V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_UONGKHICAN;
            }
            //set
            //{
            //    _isNeedToUse = value;
            //    RaisePropertyChanged("isNeedToUse");
            //}
        }

        private bool _isSave = false;
        [DataMemberAttribute()]
        public bool isSave
        {
            get
            {
                return _isSave;
            }
            set
            {
                _isSave = value;
                RaisePropertyChanged("isSave");
            }
        }

        private bool _isComboDrugType = true;
        [DataMemberAttribute()]
        public bool isComboDrugType
        {
            get
            {
                return _isComboDrugType;
            }
            set
            {
                _isComboDrugType = value;
                RaisePropertyChanged("isComboDrugType");
            }
        }


        [DataMemberAttribute()]
        public bool isForm
        {
            get
            {
                return _isForm;
            }
            set
            {
                _isForm = value;
                RaisePropertyChanged("isForm");
            }
        }
        private bool _isForm = false;
        #endregion

        private Lookup _DrugType;
        [DataMemberAttribute()]
        public Lookup DrugType
        {
            get
            {
                return _DrugType;
            }
            set
            {
                //if (_DrugType != value)
                {
                    _DrugType = value;
                    RaisePropertyChanged("DrugType");
                    if (_DrugType != null)
                    {
                        if (_DrugType.LookupID > 0) 
                        {
                            V_DrugType = _DrugType.LookupID;                            
                        }                        
                    }

                }
            }
        }


        [DataMemberAttribute()]
        public string BackGroundColor
        {
            get
            {
                return _BackGroundColor;
            }
            set
            {
                OnBackGroundColorChanging(value);
                _BackGroundColor = value;
                RaisePropertyChanged("BackGroundColor");
                OnBackGroundColorChanged();
            }
        }
        private string _BackGroundColor = "#F8F8F8";//trang
        partial void OnBackGroundColorChanging(string value);
        partial void OnBackGroundColorChanged();


        //private void SetValueFollowNgayDung()
        //{
        //    //if (IsDrugNotInCat == false)
        //    {
        //        if (HasSchedules == false
        //            && isNeedToUse==false)
        //        {
        //            Nullable<float> TongThuoc = 0;
        //            float Tong = 0;

        //            if (SelectedDrugForPrescription != null)
        //            {
        //                TongThuoc = MDose + ADose.GetValueOrDefault() + NDose.GetValueOrDefault() 
        //                    + EDose.GetValueOrDefault();
        //                Tong = (float)(TongThuoc.Value * (DayRpts + DayExtended) *
        //                    (SelectedDrugForPrescription.UnitVolume == null || SelectedDrugForPrescription.UnitVolume ==0 ? 1 : SelectedDrugForPrescription.UnitVolume))
        //                    / (float)(SelectedDrugForPrescription.DispenseVolume == null || SelectedDrugForPrescription.DispenseVolume == 0 ? 1 : SelectedDrugForPrescription.DispenseVolume);
        //                Qty =  Math.Ceiling(Tong);
        //            }
        //        }
        //    }            
        //}
    }
}
