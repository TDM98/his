using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.Collections.Generic;
using System.Text;

namespace DataEntities
{
    public partial class DrugDeptEstimationForPO : NotifyChangedBase
    {
        #region Factory Method
        public static DrugDeptEstimationForPO CreateDrugDeptEstimationForPO(Int64 drugDeptEstimatePoID, Int64 staffID, DateTime dateOfEstimation)
        {
            DrugDeptEstimationForPO drugDeptEstimationForPO = new DrugDeptEstimationForPO();
            drugDeptEstimationForPO.DrugDeptEstimatePoID = drugDeptEstimatePoID;
            drugDeptEstimationForPO.StaffID = staffID;
            drugDeptEstimationForPO.DateOfEstimation = dateOfEstimation;
            return drugDeptEstimationForPO;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 DrugDeptEstimatePoID
        {
            get
            {
                return _DrugDeptEstimatePoID;
            }
            set
            {
                if (_DrugDeptEstimatePoID != value)
                {
                    OnDrugDeptEstimatePoIDChanging(value);
                    _DrugDeptEstimatePoID = value;
                    RaisePropertyChanged("DrugDeptEstimatePoID");
                    OnDrugDeptEstimatePoIDChanged();
                    RaisePropertyChanged("CanPrint");
                }
            }
        }
        private Int64 _DrugDeptEstimatePoID;
        partial void OnDrugDeptEstimatePoIDChanging(Int64 value);
        partial void OnDrugDeptEstimatePoIDChanged();

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

        [DataMemberAttribute()]
        public Nullable<Boolean> IsForeign
        {
            get
            {
                return _IsForeign;
            }
            set
            {
                _IsForeign = value;
                if (_IsForeign.GetValueOrDefault())
                {
                    IsTrongNuoc = false;
                    RaisePropertyChanged("IsTrongNuoc");
                }
                RaisePropertyChanged("IsForeign");
            }
        }
        private Nullable<Boolean> _IsForeign = false;

        [DataMemberAttribute()]
        public Nullable<Boolean> IsTrongNuoc
        {
            get
            {
                return _IsTrongNuoc;
            }
            set
            {
                _IsTrongNuoc = value;
                if (_IsTrongNuoc.GetValueOrDefault())
                {
                    IsForeign = false;
                    RaisePropertyChanged("IsForeign");
                }
                RaisePropertyChanged("IsTrongNuoc");
            }
        }
        private Nullable<Boolean> _IsTrongNuoc = true;


        [DataMemberAttribute()]
        public long RefGenDrugCatID_1
        {
            get
            {
                return _RefGenDrugCatID_1;
            }
            set
            {
                if (_RefGenDrugCatID_1 != value)
                {
                    OnRefGenDrugCatID_1Changing(value);
                    _RefGenDrugCatID_1 = value;
                    RaisePropertyChanged("RefGenDrugCatID_1");
                    OnRefGenDrugCatID_1Changed();
                }
            }
        }
        private long _RefGenDrugCatID_1;
        partial void OnRefGenDrugCatID_1Changing(long value);
        partial void OnRefGenDrugCatID_1Changed();
        #endregion

        #region Navigation Properties
        [DataMemberAttribute()]
        private ObservableCollection<DrugDeptEstimationForPoDetail> _EstimationDetails;
        public ObservableCollection<DrugDeptEstimationForPoDetail> EstimationDetails
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
                }
            }
        }
        #endregion

        #region Convert XML

        public string ConvertDetailsListToXml()
        {
            return ConvertDetailsListToXml(_EstimationDetails);
        }
        public string ConvertDetailsListToXml(IEnumerable<DrugDeptEstimationForPoDetail> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<EstimateDetails>");
                foreach (DrugDeptEstimationForPoDetail details in items)
                {
                    if (details.RefGenMedProductDetails != null && details.GenMedProductID > 0)
                    {
                        int EntityState = (int)details.EntityState;
                        sb.Append("<RecInfo>");
                        sb.AppendFormat("<DrugDeptEstPoDetailID>{0}</DrugDeptEstPoDetailID>", details.DrugDeptEstPoDetailID);
                        sb.AppendFormat("<GenMedProductID>{0}</GenMedProductID>", details.GenMedProductID);
                        sb.AppendFormat("<OutQtyPrevFirstMonth>{0}</OutQtyPrevFirstMonth>", details.OutQtyPrevFirstMonth);
                        sb.AppendFormat("<OutQtyPrevSecondMonth>{0}</OutQtyPrevSecondMonth>", details.OutQtyPrevSecondMonth);
                        sb.AppendFormat("<OutQtyPrevThirdMonth>{0}</OutQtyPrevThirdMonth>", details.OutQtyPrevThirdMonth);
                        sb.AppendFormat("<OutQtyPrevFourthMonth>{0}</OutQtyPrevFourthMonth>", details.OutQtyPrevFourthMonth);
                        sb.AppendFormat("<RemainQty>{0}</RemainQty>", details.RemainQty);
                        sb.AppendFormat("<EstimatedQty>{0}</EstimatedQty>", details.EstimatedQty_F);
                        sb.AppendFormat("<AdjustedQty>{0}</AdjustedQty>", details.AdjustedQty);
                        sb.AppendFormat("<NumberOfEstimatedMonths>{0}</NumberOfEstimatedMonths>", details.NumberOfEstimatedMonths_F);
                        sb.AppendFormat("<OutQtyLastTwelveMonths>{0}</OutQtyLastTwelveMonths>", details.OutQtyLastTwelveMonths);
                        sb.AppendFormat("<ToDateOutQty>{0}</ToDateOutQty>", details.ToDateOutQty);
                        sb.AppendFormat("<UnitPrice>{0}</UnitPrice>", details.UnitPrice);
                        sb.AppendFormat("<PackagePrice>{0}</PackagePrice>", details.PackagePrice);
                        sb.AppendFormat("<SupplierID>{0}</SupplierID>", details.SupplierID);
                        sb.AppendFormat("<EntityState>{0}</EntityState>", EntityState);
                        sb.AppendFormat("<BidDetailID>{0}</BidDetailID>", details.BidDetailID);
                        //Thêm cột dự trù mới 20210831
                        sb.AppendFormat("<OutQty>{0}</OutQty>", details.OutQty_F);
                        sb.AppendFormat("<EstimationNote>{0}</EstimationNote>", details.EstimationNote);
                        sb.AppendFormat("<ReqDrugInClinicDeptIDList>{0}</ReqDrugInClinicDeptIDList>", details.ReqDrugInClinicDeptIDList);
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
            }
        }

        public bool CanOK
        {
            get { return IsOrder.GetValueOrDefault(); }
        }
        public bool CanPrint
        {
            get { return DrugDeptEstimatePoID > 0; }
        }

        private long? _BidID;
        [DataMemberAttribute]
        public long? BidID
        {
            get
            {
                return _BidID;
            }
            set
            {
                _BidID = value;
                RaisePropertyChanged("BidID");
            }
        }
    }
}