using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{

    public partial class InwardDMedRscrInvoice : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new InwardDMedRscrInvoice object.

        /// <param name="invDMedRscrID">Initial value of the InvDMedRscrID property.</param>
        /// <param name="invDMedRscrNumber">Initial value of the InvDMedRscrNumber property.</param>
        /// <param name="dateInvDMedRscr">Initial value of the DateInvDMedRscr property.</param>
        public static InwardDMedRscrInvoice CreateInwardDMedRscrInvoice(long invDMedRscrID, String invDMedRscrNumber, DateTime dateInvDMedRscr)
        {
            InwardDMedRscrInvoice inwardDMedRscrInvoice = new InwardDMedRscrInvoice();
            inwardDMedRscrInvoice.InvDMedRscrID = invDMedRscrID;
            inwardDMedRscrInvoice.InvDMedRscrNumber = invDMedRscrNumber;
            inwardDMedRscrInvoice.DateInvDMedRscr = dateInvDMedRscr;
            return inwardDMedRscrInvoice;
        }

        #endregion
        #region Primitive Properties
        [DataMemberAttribute()]
        public long InvDMedRscrID
        {
            get
            {
                return _InvDMedRscrID;
            }
            set
            {
                if (_InvDMedRscrID != value)
                {
                    OnInvDMedRscrIDChanging(value);
                    _InvDMedRscrID = value;
                    RaisePropertyChanged("InvDMedRscrID");
                    OnInvDMedRscrIDChanged();
                }
            }
        }
        private long _InvDMedRscrID;
        partial void OnInvDMedRscrIDChanging(long value);
        partial void OnInvDMedRscrIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> SupplierID
        {
            get
            {
                return _SupplierID;
            }
            set
            {
                OnSupplierIDChanging(value);
                _SupplierID = value;
                RaisePropertyChanged("SupplierID");
                OnSupplierIDChanged();
            }
        }
        private Nullable<long> _SupplierID;
        partial void OnSupplierIDChanging(Nullable<long> value);
        partial void OnSupplierIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> StaffID
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
        private Nullable<Int64> _StaffID;
        partial void OnStaffIDChanging(Nullable<Int64> value);
        partial void OnStaffIDChanged();

        [DataMemberAttribute()]
        public String InvInwardDMedID
        {
            get
            {
                return _InvInwardDMedID;
            }
            set
            {
                OnInvInwardDMedIDChanging(value);
                _InvInwardDMedID = value;
                RaisePropertyChanged("InvInwardDMedID");
                OnInvInwardDMedIDChanged();
            }
        }
        private String _InvInwardDMedID;
        partial void OnInvInwardDMedIDChanging(String value);
        partial void OnInvInwardDMedIDChanged();


        [DataMemberAttribute()]
        public String InvDMedRscrNumber
        {
            get
            {
                return _InvDMedRscrNumber;
            }
            set
            {
                OnInvDMedRscrNumberChanging(value);
                _InvDMedRscrNumber = value;
                RaisePropertyChanged("InvDMedRscrNumber");
                OnInvDMedRscrNumberChanged();
            }
        }
        private String _InvDMedRscrNumber;
        partial void OnInvDMedRscrNumberChanging(String value);
        partial void OnInvDMedRscrNumberChanged();

        [DataMemberAttribute()]
        public DateTime DateInvDMedRscr
        {
            get
            {
                return _DateInvDMedRscr;
            }
            set
            {
                OnDateInvDMedRscrChanging(value);
                _DateInvDMedRscr = value;
                RaisePropertyChanged("DateInvDMedRscr");
                OnDateInvDMedRscrChanged();
            }
        }
        private DateTime _DateInvDMedRscr;
        partial void OnDateInvDMedRscrChanging(DateTime value);
        partial void OnDateInvDMedRscrChanged();


        [DataMemberAttribute()]
        public Nullable<DateTime> DateInvDMedRscrNumber
        {
            get
            {
                return _DateInvDMedRscrNumber;
            }
            set
            {
                OnDateInvDMedRscrNumberChanging(value);
                _DateInvDMedRscrNumber = value;
                RaisePropertyChanged("DateInvDMedRscrNumber");
                OnDateInvDMedRscrNumberChanged();
            }
        }
        private Nullable<DateTime> _DateInvDMedRscrNumber;
        partial void OnDateInvDMedRscrNumberChanging(Nullable<DateTime> value);
        partial void OnDateInvDMedRscrNumberChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> V_Reason
        {
            get
            {
                return _V_Reason;
            }
            set
            {
                OnV_ReasonChanging(value);
                _V_Reason = value;
                RaisePropertyChanged("V_Reason");
                OnV_ReasonChanged();
            }
        }
        private Nullable<Int64> _V_Reason;
        partial void OnV_ReasonChanging(Nullable<Int64> value);
        partial void OnV_ReasonChanged();

        [DataMemberAttribute()]
        public String V_ReasonName
        {
            get
            {
                return _V_ReasonName;
            }
            set
            {
                OnV_ReasonNameChanging(value);
                _V_ReasonName = value;
                RaisePropertyChanged("V_ReasonName");
                OnV_ReasonNameChanged();
            }
        }
        private String _V_ReasonName;
        partial void OnV_ReasonNameChanging(String value);
        partial void OnV_ReasonNameChanged();

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public ObservableCollection<InwardDMedRscr> InwardDMedRscrs
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public Staff SelectedStaff
        {
            get
            {
                return _selectedStaff;
            }
            set
            {
                OnSelectedStaffChanging(value);
                _selectedStaff = value;
                RaisePropertyChanged("SelectedStaff");
                OnSelectedStaffChanged();
            }
        }
        private Staff _selectedStaff;
        partial void OnSelectedStaffChanging(Staff value);
        partial void OnSelectedStaffChanged();

        [Required(ErrorMessage = "Please select one Supplier")]
        [DataMemberAttribute()]
        public Supplier SelectedSupplier
        {
            get
            {
                return _selectedSupplier;
            }
            set
            {
                OnSelectedSupplierChanging(value);
                ValidateProperty("SelectedSupplier", value);
                _selectedSupplier = value;
                RaisePropertyChanged("SelectedSupplier");
                OnSelectedSupplierChanged();
            }
        }
        private Supplier _selectedSupplier;
        partial void OnSelectedSupplierChanging(Supplier value);
        partial void OnSelectedSupplierChanged();
        #endregion
    }
}
