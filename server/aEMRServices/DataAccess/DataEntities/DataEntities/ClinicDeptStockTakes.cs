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
    public partial class ClinicDeptStockTakes : NotifyChangedBase
    {
        #region Factory Method

        public static ClinicDeptStockTakes CreateClinicDeptStockTakes(Int64 ClinicDeptStockTakeID, Int64 staffID, DateTime StockTakingDate)
        {
            ClinicDeptStockTakes DrugDeptEstimationForPO = new ClinicDeptStockTakes();
            DrugDeptEstimationForPO.ClinicDeptStockTakeID = ClinicDeptStockTakeID;
            DrugDeptEstimationForPO.StaffID = staffID;
            DrugDeptEstimationForPO.StockTakingDate = StockTakingDate;
            return DrugDeptEstimationForPO;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 ClinicDeptStockTakeID
        {
            get
            {
                return _ClinicDeptStockTakeID;
            }
            set
            {
                if (_ClinicDeptStockTakeID != value)
                {
                    OnClinicDeptStockTakeIDChanging(value);
                    _ClinicDeptStockTakeID = value;
                    RaisePropertyChanged("ClinicDeptStockTakeID");
                    OnClinicDeptStockTakeIDChanged();
                }
            }
        }
        private Int64 _ClinicDeptStockTakeID;
        partial void OnClinicDeptStockTakeIDChanging(Int64 value);
        partial void OnClinicDeptStockTakeIDChanged();

       
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
        public string Notes
        {
            get
            {
                return _Notes;
            }
            set
            {
                _Notes = value;
                RaisePropertyChanged("Notes");
            }
        }
        private string _Notes;

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
        private ObservableCollection<ClinicDeptStockTakeDetails> _StockTakeDetails;
        public ObservableCollection<ClinicDeptStockTakeDetails> StockTakeDetails
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
        public string ConvertDetailsListToXml(IEnumerable<ClinicDeptStockTakeDetails> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<StockTakeDetails>");
                foreach (ClinicDeptStockTakeDetails details in items)
                {
                    //if (details.RefGenMedProductDetails!= null  && details.GenMedProductID > 0)
                    if (details.GenMedProductID > 0 && (details.CaculatedQty > 0 || details.ActualQty >= 0))
                    {
                        int EntityState = (int)details.EntityState;
                        sb.Append("<RecInfo>");
                        sb.AppendFormat("<ClinicDeptStockTakeDetailID>{0}</ClinicDeptStockTakeDetailID>", details.ClinicDeptStockTakeDetailID);
                        sb.AppendFormat("<ClinicDeptStockTakeID>{0}</ClinicDeptStockTakeID>", details.ClinicDeptStockTakeID);
                        sb.AppendFormat("<GenMedProductID>{0}</GenMedProductID>", details.GenMedProductID);
                        sb.AppendFormat("<CaculatedQty>{0}</CaculatedQty>", details.CaculatedQty);
                        sb.AppendFormat("<ActualQty>{0}</ActualQty>", details.ActualQty);
                        sb.AppendFormat("<FinalAmount>{0}</FinalAmount>", details.FinalAmount);
                        sb.AppendFormat("<RASFlag>{0}</RASFlag>", details.RowActionStatusFlag);
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

        //--▼--001--20/01/2021 DatTB Convert Danh sách thuốc tính tồn thành kiểu DataTable
        public DataTable ConvertDetailsListToDT()
        {
            return ConvertDetailsListToDT(_StockTakeDetails);
        }
        public DataTable ConvertDetailsListToDT(IEnumerable<ClinicDeptStockTakeDetails> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("ClinicDeptStockTakeDetailID");
                dataTable.Columns.Add("ClinicDeptStockTakeID");
                dataTable.Columns.Add("GenMedProductID");
                dataTable.Columns.Add("CaculatedQty");
                dataTable.Columns.Add("ActualQty");
                dataTable.Columns.Add("FinalAmount");
                dataTable.Columns.Add("RASFlag");
                foreach (ClinicDeptStockTakeDetails details in items)
                {
                    if (details.GenMedProductID > 0 && (details.CaculatedQty > 0 || details.ActualQty >= 0))
                    {
                        int EntityState = (int)details.EntityState;
                        dataTable.Rows.Add(
                            Convert.ToInt32(details.ClinicDeptStockTakeDetailID),
                            Convert.ToInt32(details.ClinicDeptStockTakeID),
                            Convert.ToInt32(details.GenMedProductID),
                            Convert.ToDecimal(details.CaculatedQty),
                            Convert.ToDecimal(details.ActualQty),
                            Convert.ToDecimal(details.FinalAmount),
                            Convert.ToInt32(details.RowActionStatusFlag)
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
        //--▲--001--20/01/2021 DatTB 

        #endregion
    }
}
