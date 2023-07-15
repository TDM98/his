using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Service.Core.Common;

namespace DataEntities
{
    public partial class HealthInsurance
    {
        private EntityState _EntityState = EntityState.NEW;
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
   
        public override bool Equals(object obj)
        {
            var info = obj as HealthInsurance;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.HIID > 0 && this.HIID == info.HIID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool IsActiveAndNotDeleted
        {
            get
            {
                return (!MarkAsDeleted && IsActive);
            }
        }

        private bool _IsActive;
        public bool IsActive 
        { 
            get
            {
                return _IsActive;
            }
            set
            {
                _IsActive = value;
                RaisePropertyChanged("IsActive");
            }
        }

        public void SetIsActive(bool val)
        {
            _IsActive = val;
        }

        private bool _IsHistoryActive;
        public bool IsHistoryActive
        {
            get
            {
                return _IsHistoryActive;
            }
            set
            {
                _IsHistoryActive = value;
                RaisePropertyChanged("IsHistoryActive");
            }
        }

        private bool? _IsValid;
        public bool IsValid
        {
            get
            {
                if (_IsValid == null)
                {
                    ResetIsValidProperty();
                }
                return _IsValid.GetValueOrDefault();
            }
            set
            {
                _IsValid = value;
                RaisePropertyChanged("IsValid");
            }
        }

        // TxD 28/01/2018: Commented OUT the following because It has NEVER been USED
        //public void CalcBenefit()
        //{
        //    if (_ActiveHealthInsuranceHIPatientBenefit != null)
        //    {
        //        HIPatientBenefit = _ActiveHealthInsuranceHIPatientBenefit.Benefit;
        //        return;
        //    }
        //    //if (InsuranceBenefit != null)
        //    //{
        //    //    HIPatientBenefit = InsuranceBenefit.RebatePercentage;
        //    //    return;
        //    //}
        //    HIPatientBenefit = null;
        //}
        
        /// <summary>
        /// Benefit nguoi dung nhap tren form.
        /// </summary>
        private double? _HIPatientBenefit;
        [Range(0.3D,1.0D,ErrorMessage = "Giá trị quyền lợi phải nằm trong khoảng 30% - 100%")]
        [DataMemberAttribute()]
        public double? HIPatientBenefit
        {
            get
            {
                return _HIPatientBenefit;
            }
            set
            {
                if (_HIPatientBenefit != value)
                {
                    _HIPatientBenefit = value;
                    RaisePropertyChanged("HIPatientBenefit");
                }
            }
        }

        // TxD 28/01/2018: Commented OUT the following because It has NEVER been USED
        //private HealthInsuranceHIPatientBenefit _ActiveHealthInsuranceHIPatientBenefit;
        //[DataMemberAttribute()]
        //public HealthInsuranceHIPatientBenefit ActiveHealthInsuranceHIPatientBenefit
        //{
        //    get
        //    {
        //        return _ActiveHealthInsuranceHIPatientBenefit;
        //    }
        //    set
        //    {
        //        if (_ActiveHealthInsuranceHIPatientBenefit != value)
        //        {
        //            _ActiveHealthInsuranceHIPatientBenefit = value;
        //            RaisePropertyChanged("ActiveHealthInsuranceHIPatientBenefit");
        //        }
        //    }
        //}

        public void ResetIsValidProperty()
        {
            if (!this.ValidDateFrom.HasValue || !this.ValidDateTo.HasValue)
            {
                _IsValid = false;
            }
            else
            {
                DateTime today = DateTime.Now;
                _IsValid = _ValidDateFrom.Value <= today && today <= _ValidDateTo.Value;
            }
        }


        public bool ValidateAllFields(out ObservableCollection<ValidationResult> validationResults)
        {
            validationResults = new ObservableCollection<ValidationResult>(); 
            validationResults.Add(ValidateCardNum());
            validationResults.Add(ValidateFromDate());
            validationResults.Add(ValidateToDate());
            foreach (var val in validationResults)
            {
                if (val != null)
                {
                    return false;
                }
            }
            return true;
        }

        // TxD 28/12/2014: Added for new BHYT Card rules applied from 01/01/2015
        public bool ValidateAllFields_New_2015(out ObservableCollection<ValidationResult> validationResults
            , string ValidHIPattern = null
            , List<string[]> InsuranceBenefitCategories = null)
        {
            validationResults = new ObservableCollection<ValidationResult>();
            validationResults.Add(ValidateCardNum_New_2015(ValidHIPattern, InsuranceBenefitCategories));
            validationResults.Add(ValidateFromDate());
            validationResults.Add(ValidateToDate());
            foreach (var val in validationResults)
            {
                if (val != null)
                {
                    return false;
                }
            }
            return true;
        }


        public ValidationResult ValidateCardNum()
        {
            if (!string.IsNullOrWhiteSpace(HICardNo))
            {
                string strDoiTuong, strQuyenLoi, strTinh, strQuanHuyen, strDonVi, strSTT;
                HICardValidatorBase validator = HICardValidatorFactory.Instance.CreateValidator(1);
                if (!validator.ExtractHICardNumber(HICardType.LookupID, HICardNo, out strDoiTuong, out strQuyenLoi, out strTinh, out strQuanHuyen, out strDonVi, out strSTT))
                {
                    return new ValidationResult("Thẻ BH không hợp lệ!", new string[] { "HICardNo" });
                }
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Vui lòng nhập mã thẻ BH!", new string[] { "HICardNo" });
            }
            
        }

        // TxD 28/12/2014: Added for new BHYT Card rules applied from 01/01/2015
        public ValidationResult ValidateCardNum_New_2015(string ValidHIPattern, List<string[]> InsuranceBenefitCategories)
        {
            if (!string.IsNullOrWhiteSpace(HICardNo))
            {
                string strDoiTuong, strQuyenLoi, strTinh, strQuanHuyen, strDonVi, strSTT, IBeID;
                HICardValidatorBase validator = HICardValidatorFactory.Instance.CreateValidator(1);
                if (!validator.ExtractHICardNumber_2015(HICardType.LookupID, HICardNo, out strDoiTuong, out strQuyenLoi, out strTinh, out strQuanHuyen, out strDonVi, out strSTT, out IBeID, ValidHIPattern, InsuranceBenefitCategories))
                {
                    return new ValidationResult("Thẻ BH không hợp lệ!", new string[] { "HICardNo" });
                }
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Vui lòng nhập mã thẻ BH!", new string[] { "HICardNo" });
            }
        }

        public ValidationResult ValidateFromDate()
        {
            if (_ValidDateFrom.HasValue)
            {
                DateTime today;
                if (GetServerDate != null)
                {
                    today = GetServerDate();
                }
                else
                {
                    today = DateTime.Now.Date;
                }

                if (_ValidDateFrom.Value.Year <= 1900)
                {
                    return new ValidationResult("Từ Ngày không hợp lệ!", new string[] { "ValidDateFrom" });
                }

                if (_ValidDateFrom.Value.Date > today)
                {
                    return new ValidationResult("Từ Ngày không được lớn hơn ngày hiện tại!", new string[] { "ValidDateFrom" });
                }
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Vui lòng nhập ngày bắt đầu thẻ BH!", new string[] { "ValidDateFrom" });
            }
            
        }

        public ValidationResult ValidateToDate()
        {
            if (_ValidDateTo.HasValue)
            {
                if (_ValidDateFrom.HasValue)
                {
                    if (_ValidDateTo.Value.Year <= 1900 || _ValidDateTo.Value.Year > DateTime.Now.AddYears(20).Year)
                    {
                        return new ValidationResult("Đến Ngày không hợp lệ!", new string[] { "ValidDateTo" });
                    }

                    if (_ValidDateTo.Value.Date <= _ValidDateFrom.Value.Date)
                    {
                        return new ValidationResult("Đến Ngày phải > Từ Ngày", new string[] { "ValidDateTo" });
                    }
                }
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Vui lòng nhập ngày kết thúc thẻ BH!", new string[] { "ValidDateTo" });
            }
        }



        private string _strObject;
        public string strObject
        {
            get
            {
                if (HICardNo != null)
                {
                    _strObject = _HICardNo.Substring(0,2);
                }
                 return _strObject;
            }
            
        }

        private string _strBenifit;
        public string strBenifit
        {
            get
            {
                if (HICardNo != null)
                {
                    _strBenifit = _HICardNo.Substring(2, 1);
                }
                return _strBenifit;
            }
          
        }
        private string _strProvice;
        public string strProvince
        {
            get
            {
                if (HICardNo != null)
                {
                    _strProvice = _HICardNo.Substring(3, 2);
                }
                return _strProvice;
            }
        }
        private string _strDistrict;
        public string strDistrict
        {
            get
            {
                if (HICardNo != null)
                {
                    _strDistrict = _HICardNo.Substring(5, 2);
                }
                return _strDistrict;
            }
           
        }
        private string _strUnit;
        public string strUnit
        {
           get{
                if (HICardNo != null)
                {
                    _strUnit = _HICardNo.Substring(7, 3);
                }
                return _strUnit;
            }
        }
        private string _strOrderNo;
        public string strOrderNo
        {
            get
            {
                if (HICardNo != null)
                {
                    _strOrderNo = _HICardNo.Substring(10, HICardNo.Length - 10);
                }
                return _strOrderNo;
            }
        }

        private long? _hisID;
        public long? HisID
        {
            get { return _hisID; }
            set
            {
                _hisID = value;
                RaisePropertyChanged("HisID");
            }
        }

        private long? _hosID;
        public long? HosID          // Hospital ID
        {
            get { return _hosID; }
            set
            {
                _hosID = value;
                RaisePropertyChanged("HosID");
            }
        }
        
        private bool _used;
        [DataMemberAttribute()]
        public bool Used
        {
            get { return _used; }
            set
            {
                _used = value;
                RaisePropertyChanged("Used");
            }
        }

        private bool _CofirmDuplicate;
        [DataMemberAttribute()]
        public bool CofirmDuplicate
        {
            get
            {
                return _CofirmDuplicate;
            }
            set
            {
                _CofirmDuplicate = value;
                RaisePropertyChanged("CofirmDuplicate");
            }
        }
    }
}


//public static ValidationResult ValidateHICardNo(long V_HICardType, string cardNo, ValidationContext context)
//{
//    if (!string.IsNullOrWhiteSpace(cardNo))
//    {

//        string strDoiTuong, strQuyenLoi, strTinh, strQuanHuyen, strDonVi, strSTT;
//        //if (!AxHelper.ExtractHICardNumber(cardNo, out strDoiTuong, out strQuyenLoi, out strTinh, out strQuanHuyen, out strDonVi, out strSTT))
//        //{
//        //    return new ValidationResult("Thẻ BH không hợp lệ!", new string[] { "HICardNo" });
//        //}
//        HICardValidatorBase validator = HICardValidatorFactory.Instance.CreateValidator(1);
//        if (!validator.ExtractHICardNumber(V_HICardType, cardNo, out strDoiTuong, out strQuyenLoi, out strTinh, out strQuanHuyen, out strDonVi, out strSTT))
//        {
//            return new ValidationResult("Thẻ BH không hợp lệ!", new string[] { "HICardNo" });
//        }
//    }
//    return ValidationResult.Success;
//}

//public static ValidationResult ValidateValidDateFrom(DateTime? dateFrom, ValidationContext context)
//{
//    if (dateFrom.HasValue)
//    {
//        DateTime today;
//        if (GetServerDate != null)
//        {
//            today = GetServerDate();
//        }
//        else
//        {
//            today = DateTime.Now.Date;
//        }

//        if(dateFrom.Value.Year<=1900)
//        {
//            return new ValidationResult("Từ Ngày không hợp lệ!", new string[] { "ValidDateFrom" });
//        }

//        if (dateFrom.Value.Date > today)
//        {
//            return new ValidationResult("Từ Ngày không được lớn hơn ngày hiện tại!", new string[] { "ValidDateFrom" });
//        }
//    }
//    return ValidationResult.Success;
//}

//public static ValidationResult ValidateValidDateTo(DateTime? dateTo, ValidationContext context)
//{
//    HealthInsurance obj = context.ObjectInstance as HealthInsurance;
//    if (dateTo.HasValue && obj.ValidDateFrom.HasValue)
//    {

//        if (dateTo.Value.Year <= 1900)
//        {
//            return new ValidationResult("Đến Ngày không hợp lệ!", new string[] { "ValidDateTo" });
//        }

//        if (dateTo.Value.Date <= obj.ValidDateFrom.Value.Date)
//        {
//            return new ValidationResult("Đến Ngày phải > Từ Ngày", new string[] { "ValidDateTo" });
//        }
//    }
//    return ValidationResult.Success;
//}