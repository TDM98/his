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
    public partial class PharmacyStockTakes : NotifyChangedBase
    {
        #region Factory Method

        public static PharmacyStockTakes CreatePharmacyStockTakes(Int64 PharmacyStockTakeID, Int64 staffID, DateTime StockTakingDate)
        {
            PharmacyStockTakes pharmacyEstimationForPO = new PharmacyStockTakes();
            pharmacyEstimationForPO.PharmacyStockTakeID = PharmacyStockTakeID;
            pharmacyEstimationForPO.StaffID = staffID;
            pharmacyEstimationForPO.StockTakingDate = StockTakingDate;
            return pharmacyEstimationForPO;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 PharmacyStockTakeID
        {
            get
            {
                return _PharmacyStockTakeID;
            }
            set
            {
                if (_PharmacyStockTakeID != value)
                {
                    OnPharmacyStockTakeIDChanging(value);
                    _PharmacyStockTakeID = value;
                    RaisePropertyChanged("PharmacyStockTakeID");
                    OnPharmacyStockTakeIDChanged();
                }
            }
        }
        private Int64 _PharmacyStockTakeID;
        partial void OnPharmacyStockTakeIDChanging(Int64 value);
        partial void OnPharmacyStockTakeIDChanged();

       
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

        [DataMemberAttribute]
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
        private bool _IsFinished = false;

        [DataMemberAttribute]
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
        private bool _IsRefresh = false;

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
        private ObservableCollection<PharmacyStockTakeDetails> _StockTakeDetails;
        public ObservableCollection<PharmacyStockTakeDetails> StockTakeDetails
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
        public string ConvertDetailsListToXml(IEnumerable<PharmacyStockTakeDetails> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<StockTakeDetails>");
                foreach (PharmacyStockTakeDetails details in items)
                {
                    if ( details.DrugID > 0 && (details.CaculatedQty > 0 || details.ActualQty >= 0))
                    {
                        int EntityState = (int)details.EntityState;
                        sb.Append("<RecInfo>");
                        sb.AppendFormat("<PharmacyStockTakeDetailID>{0}</PharmacyStockTakeDetailID>", details.PharmacyStockTakeDetailID);
                        sb.AppendFormat("<PharmacyStockTakeID>{0}</PharmacyStockTakeID>", details.PharmacyStockTakeID);
                        sb.AppendFormat("<DrugID>{0}</DrugID>", details.DrugID);
                        sb.AppendFormat("<CaculatedQty>{0}</CaculatedQty>", details.CaculatedQty);
                        sb.AppendFormat("<ActualQty>{0}</ActualQty>", details.ActualQty);
                        sb.AppendFormat("<Price>{0}</Price>",details.Price);
                        sb.AppendFormat("<NewestInwardPrice>{0}</NewestInwardPrice>", details.NewestInwardPrice.ToString());
                        sb.AppendFormat("<InBatchNumber>{0}</InBatchNumber>", details.InBatchNumber);
                        sb.AppendFormat("<InExpiryDate>{0}</InExpiryDate>", details.InExpiryDate.HasValue ? details.InExpiryDate.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : null);
                        sb.AppendFormat("<Notes>{0}</Notes>", details.Notes);
                        sb.AppendFormat("<AmountValue>{0}</AmountValue>", details.TotalPrice);
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

        //--▼--001--22/01/2021 DatTB Convert Danh sách thuốc tính tồn thành kiểu DataTable
        public DataTable ConvertDetailsListToDT()
        {
            return ConvertDetailsListToDT(_StockTakeDetails);
        }
        public DataTable ConvertDetailsListToDT(IEnumerable<PharmacyStockTakeDetails> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("PharmacyStockTakeDetailID");
                dataTable.Columns.Add("PharmacyStockTakeID");
                dataTable.Columns.Add("DrugID");
                dataTable.Columns.Add("CaculatedQty");
                dataTable.Columns.Add("ActualQty");
                dataTable.Columns.Add("Price");
                dataTable.Columns.Add("NewestInwardPrice");
                dataTable.Columns.Add("InBatchNumber");
                dataTable.Columns.Add("InExpiryDate");
                dataTable.Columns.Add("Notes");
                dataTable.Columns.Add("AmountValue");
                foreach (PharmacyStockTakeDetails details in items)
                {
                    if (details.DrugID > 0 && (details.CaculatedQty > 0 || details.ActualQty >= 0))
                    {
                        int EntityState = (int)details.EntityState;
                        dataTable.Rows.Add(
                            Convert.ToInt32(details.PharmacyStockTakeDetailID),
                            Convert.ToInt32(details.PharmacyStockTakeID),
                            Convert.ToInt32(details.DrugID),
                            Convert.ToDecimal(details.CaculatedQty),
                            Convert.ToDecimal(details.ActualQty),
                            Convert.ToDecimal(details.Price),
                            Convert.ToDecimal(details.NewestInwardPrice),
                            details.InBatchNumber,
                            details.InExpiryDate.HasValue ? details.InExpiryDate.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : null,
                            details.Notes,
                            Convert.ToDecimal(details.TotalPrice)
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
        //--▲--001--22/01/2021 DatTB 

        #endregion
    }
}
