using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.Text;
using System.Collections.Generic;
using System.Data;

namespace DataEntities
{
    public partial class DrugDeptStockTakes : NotifyChangedBase
    {
        #region Factory Method

        public static DrugDeptStockTakes CreateDrugDeptStockTakes(Int64 DrugDeptStockTakeID, Int64 staffID, DateTime StockTakingDate)
        {
            DrugDeptStockTakes DrugDeptEstimationForPO = new DrugDeptStockTakes();
            DrugDeptEstimationForPO.DrugDeptStockTakeID = DrugDeptStockTakeID;
            DrugDeptEstimationForPO.StaffID = staffID;
            DrugDeptEstimationForPO.StockTakingDate = StockTakingDate;
            return DrugDeptEstimationForPO;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 DrugDeptStockTakeID
        {
            get
            {
                return _DrugDeptStockTakeID;
            }
            set
            {
                if (_DrugDeptStockTakeID != value)
                {
                    OnDrugDeptStockTakeIDChanging(value);
                    _DrugDeptStockTakeID = value;
                    RaisePropertyChanged("DrugDeptStockTakeID");
                    OnDrugDeptStockTakeIDChanged();
                }
            }
        }
        private Int64 _DrugDeptStockTakeID;
        partial void OnDrugDeptStockTakeIDChanging(Int64 value);
        partial void OnDrugDeptStockTakeIDChanged();

       
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
        public DateTime StockTakingDate
        {
            get
            {
                return _StockTakingDate;
            }
            set
            {
                OnStockTakingDateChanging(value);
                _StockTakingDate = value;
                RaisePropertyChanged("StockTakingDate");
                OnStockTakingDateChanged();
            }
        }
        private DateTime _StockTakingDate=DateTime.Now;
        partial void OnStockTakingDateChanging(DateTime value);
        partial void OnStockTakingDateChanged();


        [DataMemberAttribute()]
        public String StockTakePeriodName
        {
            get
            {
                return _StockTakePeriodName;
            }
            set
            {
                OnStockTakePeriodNameChanging(value);
                _StockTakePeriodName = value;
                RaisePropertyChanged("StockTakePeriodName");
                OnStockTakePeriodNameChanged();
            }
        }
        private String _StockTakePeriodName;
        partial void OnStockTakePeriodNameChanging(String value);
        partial void OnStockTakePeriodNameChanged();

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
        public string StockTakeNotes
        {
            get
            {
                return _StockTakeNotes;
            }
            set
            {
                _StockTakeNotes = value;
                RaisePropertyChanged("StockTakeNotes");
            }
        }
        private string _StockTakeNotes;

        [DataMemberAttribute()]
        public string swhlName
        {
            get
            {
                return _swhlName;
            }
            set
            {
                _swhlName = value;
                RaisePropertyChanged("swhlName");
            }
        }
        private string _swhlName;

        [DataMemberAttribute()]
        public Int64 StoreID
        {
            get
            {
                return _StoreID;
            }
            set
            {
                _StoreID = value;
                RaisePropertyChanged("StoreID");
            }
        }
        private Int64 _StoreID;

        [DataMemberAttribute()]
        public Int64 V_MedProductType
        {
            get
            {
                return _V_MedProductType;
            }
            set
            {
                _V_MedProductType = value;
                RaisePropertyChanged("V_MedProductType");
            }
        }
        private Int64 _V_MedProductType;

        [DataMemberAttribute()]
        public long V_StockTakeType
        {
            get
            {
                return _V_StockTakeType;
            }
            set
            {
                _V_StockTakeType = value;
                RaisePropertyChanged("V_StockTakeType");
            }
        }
        private long _V_StockTakeType;

        [DataMemberAttribute()]
        public string StockTakeTypeName
        {
            get
            {
                return _StockTakeTypeName;
            }
            set
            {
                _StockTakeTypeName = value;
                RaisePropertyChanged("StockTakeTypeName");
            }
        }
        private string _StockTakeTypeName;

        [DataMemberAttribute()]
        public bool IsFinished
        {
            get
            {
                return _IsFinished;
            }
            set
            {
                _IsFinished = value;
                RaisePropertyChanged("IsFinished");
            }
        }
        private bool _IsFinished;

        [DataMemberAttribute()]
        public bool IsRefresh
        {
            get
            {
                return _IsRefresh;
            }
            set
            {
                _IsRefresh = value;
                RaisePropertyChanged("IsRefresh");
            }
        }
        private bool _IsRefresh;

        [DataMemberAttribute()]
        public bool IsLocked
        {
            get
            {
                return _IsLocked;
            }
            set
            {
                if (_IsLocked != value)
                {
                    OnIsLockedChanging(value);
                    _IsLocked = value;
                    RaisePropertyChanged("IsLocked");
                    OnIsLockedChanged();
                }
            }
        }
        private bool _IsLocked;
        partial void OnIsLockedChanging(bool value);
        partial void OnIsLockedChanged();
        #endregion

        #region Navigation Properties
        [DataMemberAttribute()]
        private ObservableCollection<DrugDeptStockTakeDetails> _StockTakeDetails;
        public ObservableCollection<DrugDeptStockTakeDetails> StockTakeDetails
        {
            get
            {
                return _StockTakeDetails;
            }
            set
            {
                if (_StockTakeDetails != value)
                {
                    _StockTakeDetails = value;
                    RaisePropertyChanged("StockTakeDetails");
                }
            }
        }
        #endregion

        #region Convert XML

        public string ConvertDetailsListToXml()
        {
            return ConvertDetailsListToXml(_StockTakeDetails);
        }
        public string ConvertDetailsListToXml(IEnumerable<DrugDeptStockTakeDetails> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<StockTakeDetails>");
                foreach (DrugDeptStockTakeDetails details in items)
                {
                    if (details.GenMedProductID > 0 && (details.CaculatedQty > 0 || details.ActualQty >= 0))
                    {
                        int EntityState = (int)details.EntityState;
                        sb.Append("<RecInfo>");
                        sb.AppendFormat("<DrugDeptStockTakeDetailID>{0}</DrugDeptStockTakeDetailID>", details.DrugDeptStockTakeDetailID);
                        sb.AppendFormat("<DrugDeptStockTakeID>{0}</DrugDeptStockTakeID>", details.DrugDeptStockTakeID);
                        sb.AppendFormat("<GenMedProductID>{0}</GenMedProductID>", details.GenMedProductID);
                        sb.AppendFormat("<CaculatedQty>{0}</CaculatedQty>", details.CaculatedQty);
                        sb.AppendFormat("<ActualQty>{0}</ActualQty>", details.ActualQty);
                        sb.AppendFormat("<FinalAmount>{0}</FinalAmount>", details.FinalAmount);
                        sb.AppendFormat("<Price>{0}</Price>", details.Price);
                        sb.AppendFormat("<NewestInwardPrice>{0}</NewestInwardPrice>", details.NewestInwardPrice);
                        sb.AppendFormat("<Notes>{0}</Notes>", details.Notes);
                        sb.AppendFormat("<RASFlag>{0}</RASFlag>", details.RowActionStatusFlag);
                        sb.AppendFormat("<BidDetailID>{0}</BidDetailID>", details.BidDetailID);
                        sb.Append("</RecInfo>");
                    }
                }
                sb.Append("</StockTakeDetails>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }

        //--▼--001--13/01/2021 DatTB Convert Danh sách thuốc tính tồn thành kiểu DataTable
        public DataTable ConvertDetailsListToDT()
        {
            return ConvertDetailsListToDT(_StockTakeDetails);
        }
        public DataTable ConvertDetailsListToDT(IEnumerable<DrugDeptStockTakeDetails> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("DrugDeptStockTakeDetailID");
                dataTable.Columns.Add("DrugDeptStockTakeID");
                dataTable.Columns.Add("GenMedProductID");
                dataTable.Columns.Add("CaculatedQty");
                dataTable.Columns.Add("ActualQty");
                dataTable.Columns.Add("FinalAmount");
                dataTable.Columns.Add("Price");
                dataTable.Columns.Add("NewestInwardPrice");
                dataTable.Columns.Add("Notes");
                dataTable.Columns.Add("RASFlag");
                dataTable.Columns.Add("BidDetailID");
                foreach (DrugDeptStockTakeDetails details in items)
                {
                    if (details.GenMedProductID > 0 && (details.CaculatedQty > 0 || details.ActualQty >= 0))
                    {
                        int EntityState = (int)details.EntityState;
                        dataTable.Rows.Add(
                            Convert.ToInt32(details.DrugDeptStockTakeDetailID),
                            Convert.ToInt32(details.DrugDeptStockTakeID),
                            Convert.ToInt32(details.GenMedProductID),
                            Convert.ToInt32(details.CaculatedQty),
                            Convert.ToInt32(details.ActualQty),
                            Convert.ToDecimal(details.FinalAmount),
                            Convert.ToDecimal(details.Price),
                            Convert.ToDecimal(details.NewestInwardPrice),
                            details.Notes,
                            Convert.ToInt32(details.RowActionStatusFlag),
                            Convert.ToInt32(details.BidDetailID)
                       );
                    }
                }
                return dataTable;
            }
            else
            {
                return null;
            }
        }
        //--▲--001--13/01/2021 DatTB 

        #endregion
    }
}
