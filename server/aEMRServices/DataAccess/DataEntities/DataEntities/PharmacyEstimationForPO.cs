using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.Text;
using System.Collections.Generic;
namespace DataEntities
{
    public partial class PharmacyEstimationForPO : NotifyChangedBase
    {
        #region Factory Method
        /// Create a new PharmacyEstimationForPO object.
        /// <param name="pharmacyEstimatePoID">Initial value of the PharmacyEstimatePoID property.</param>
        /// <param name="staffID">Initial value of the StaffID property.</param>
        /// <param name="dateOfEstimation">Initial value of the DateOfEstimation property.</param>
        public static PharmacyEstimationForPO CreatePharmacyEstimationForPO(Int64 pharmacyEstimatePoID, Int64 staffID, DateTime dateOfEstimation)
        {
            PharmacyEstimationForPO pharmacyEstimationForPO = new PharmacyEstimationForPO();
            pharmacyEstimationForPO.PharmacyEstimatePoID = pharmacyEstimatePoID;
            pharmacyEstimationForPO.StaffID = staffID;
            pharmacyEstimationForPO.DateOfEstimation = dateOfEstimation;
            return pharmacyEstimationForPO;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 PharmacyEstimatePoID
        {
            get
            {
                return _PharmacyEstimatePoID;
            }
            set
            {
                if (_PharmacyEstimatePoID != value)
                {
                    OnPharmacyEstimatePoIDChanging(value);
                    _PharmacyEstimatePoID = value;
                    RaisePropertyChanged("PharmacyEstimatePoID");
                    OnPharmacyEstimatePoIDChanged();

                    RaisePropertyChanged("CanPrint");
                    RaisePropertyChanged("CanDelete");
                    RaisePropertyChanged("CanNew");
                }
            }
        }
        private Int64 _PharmacyEstimatePoID;
        partial void OnPharmacyEstimatePoIDChanging(Int64 value);
        partial void OnPharmacyEstimatePoIDChanged();

        [DataMemberAttribute()]
        public String EstimationCode
        {
            get
            {
                return _EstimationCode;
            }
            set
            {
                OnEstimationCodeChanging(value);
                _EstimationCode = value;
                RaisePropertyChanged("EstimationCode");
                OnEstimationCodeChanged();
            }
        }
        private String _EstimationCode;
        partial void OnEstimationCodeChanging(String value);
        partial void OnEstimationCodeChanged();

        [DataMemberAttribute()]
        public Int64 StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                OnStaffIDChanging(value);
                _StaffID = value;
                RaisePropertyChanged("StaffID");
                OnStaffIDChanged();
            }
        }
        private Int64 _StaffID;
        partial void OnStaffIDChanging(Int64 value);
        partial void OnStaffIDChanged();

        [DataMemberAttribute()]
        public DateTime DateOfEstimation
        {
            get
            {
                return _DateOfEstimation;
            }
            set
            {
                OnDateOfEstimationChanging(value);
                _DateOfEstimation = value;
                RaisePropertyChanged("DateOfEstimation");
                OnDateOfEstimationChanged();
            }
        }
        private DateTime _DateOfEstimation;
        partial void OnDateOfEstimationChanging(DateTime value);
        partial void OnDateOfEstimationChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> V_MedProductType
        {
            get
            {
                return _V_MedProductType;
            }
            set
            {
                OnV_MedProductTypeChanging(value);
                _V_MedProductType = value;
                RaisePropertyChanged("V_MedProductType");
                OnV_MedProductTypeChanged();
            }
        }
        private Nullable<Int64> _V_MedProductType;
        partial void OnV_MedProductTypeChanging(Nullable<Int64> value);
        partial void OnV_MedProductTypeChanged();

        [DataMemberAttribute()]
        public String EstRemark
        {
            get
            {
                return _EstRemark;
            }
            set
            {
                OnEstRemarkChanging(value);
                _EstRemark = value;
                RaisePropertyChanged("EstRemark");
                OnEstRemarkChanged();

                RaisePropertyChanged("CanNew");
            }
        }
        private String _EstRemark;
        partial void OnEstRemarkChanging(String value);
        partial void OnEstRemarkChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> V_EstimateType
        {
            get
            {
                return _V_EstimateType;
            }
            set
            {
                OnV_EstimateTypeChanging(value);
                _V_EstimateType = value;
                RaisePropertyChanged("V_EstimateType");
                OnV_EstimateTypeChanged();
            }
        }
        private Nullable<Int64> _V_EstimateType;
        partial void OnV_EstimateTypeChanging(Nullable<Int64> value);
        partial void OnV_EstimateTypeChanged();

        [DataMemberAttribute()]
        public string FullName
        {
            get
            {
                return _FullName;
            }
            set
            {
                _FullName = value;
                RaisePropertyChanged("FullName");
            }
        }
        private string _FullName;

        [DataMemberAttribute()]
        public string ObjectValue
        {
            get
            {
                return _ObjectValue;
            }
            set
            {
                _ObjectValue = value;
                RaisePropertyChanged("ObjectValue");
            }
        }
        private string _ObjectValue;
        #endregion

        #region Navigation Properties
        [DataMemberAttribute()]
        private ObservableCollection<PharmacyEstimationForPODetail> _EstimationDetails;
        public ObservableCollection<PharmacyEstimationForPODetail> EstimationDetails
        {
            get
            {
                return _EstimationDetails;
            }
            set
            {
                if (_EstimationDetails != value)
                {
                    _EstimationDetails = value;
                    RaisePropertyChanged("EstimationDetails");
                    RaisePropertyChanged("CanNew");
                }
            }
        }
        #endregion

        #region Convert XML

        public string ConvertDetailsListToXml()
        {
            return ConvertDetailsListToXml(_EstimationDetails);
        }
        public string ConvertDetailsListToXml(IEnumerable<PharmacyEstimationForPODetail> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<EstimateDetails>");
                foreach (PharmacyEstimationForPODetail details in items)
                {
                    if (details != null && details.RefGenMedProductDetails != null && details.RefGenMedProductDetails.DrugID > 0)
                    {
                        int EntityState = (int)details.EntityState;
                        sb.Append("<RecInfo>");
                        sb.AppendFormat("<PharmacyEstimatePoDetailID>{0}</PharmacyEstimatePoDetailID>", details.PharmacyEstimatePoDetailID);
                        sb.AppendFormat("<DrugID>{0}</DrugID>", details.RefGenMedProductDetails.DrugID);
                        sb.AppendFormat("<OutQtyPrevFirstMonth>{0}</OutQtyPrevFirstMonth>", details.OutQtyPrevFirstMonth);
                        sb.AppendFormat("<OutQtyPrevSecondMonth>{0}</OutQtyPrevSecondMonth>", details.OutQtyPrevSecondMonth);
                        sb.AppendFormat("<OutQtyPrevThirdMonth>{0}</OutQtyPrevThirdMonth>", details.OutQtyPrevThirdMonth);
                        sb.AppendFormat("<OutQtyPrevFourthMonth>{0}</OutQtyPrevFourthMonth>", details.OutQtyPrevFourthMonth);
                        sb.AppendFormat("<RemainQty>{0}</RemainQty>", details.RemainQty);
                        sb.AppendFormat("<EstimatedQty>{0}</EstimatedQty>", details.EstimatedQty_F);
                        sb.AppendFormat("<PackageQty>{0}</PackageQty>", details.PackageQty);
                        sb.AppendFormat("<AdjustedQty>{0}</AdjustedQty>", details.AdjustedQty);
                        sb.AppendFormat("<NumberOfEstimatedMonths>{0}</NumberOfEstimatedMonths>", details.NumberOfEstimatedMonths_F);
                        sb.AppendFormat("<OutQtyLastTwelveMonths>{0}</OutQtyLastTwelveMonths>", details.OutQtyLastTwelveMonths);
                        sb.AppendFormat("<ToDateOutQty>{0}</ToDateOutQty>", details.ToDateOutQty);
                        sb.AppendFormat("<EntityState>{0}</EntityState>", EntityState);
                        sb.AppendFormat("<UnitPrice>{0}</UnitPrice>", details.UnitPrice);
                        sb.AppendFormat("<PackagePrice>{0}</PackagePrice>", details.PackagePrice);
                        sb.AppendFormat("<SupplierID>{0}</SupplierID>", details.SupplierID);
                        sb.Append("</RecInfo>");
                    }
                }
                sb.Append("</EstimateDetails>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }

        #endregion

        [DataMemberAttribute()]
        private bool? _IsOrder;
        public bool? IsOrder
        {
            get
            {
                return _IsOrder;
            }
            set
            {
                _IsOrder = value;
                RaisePropertyChanged("IsOrder");
                RaisePropertyChanged("CanOK");
                RaisePropertyChanged("CanDelete");
                RaisePropertyChanged("CanSave");
            }
        }

        public bool CanOK
        {
            get { return IsOrder.GetValueOrDefault(); }
        }

        public bool CanSave
        {
            get { return !IsOrder.GetValueOrDefault(); }
        }
        public bool CanNew
        {
            get { return (PharmacyEstimatePoID > 0 || !string.IsNullOrEmpty(EstRemark) || (EstimationDetails !=null && EstimationDetails.Count > 0)); }
        }
        public bool CanDelete
        {
            get { return PharmacyEstimatePoID > 0 && !IsOrder.GetValueOrDefault(); }
        }
        public bool CanPrint
        {
            get { return PharmacyEstimatePoID>0; }
        }
    }

}
