using System;
using System.Net;
using System.Windows;
using eHCMS.Services.Core.Base;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class ResourceMaintenanceLog : NotifyChangedBase
    {
        [DataMemberAttribute()]
        public Int64 RscrMaintLogID
        {
            get { return _RscrMaintLogID; }
            set
            {
                if (_RscrMaintLogID != value)
                {
                    OnRscrMaintLogIDChanging(value);
                    _RscrMaintLogID = value;
                    RaisePropertyChanged("RscrMaintLogID");
                    OnRscrMaintLogIDChanged();
                }
            }
        }
        private Int64 _RscrMaintLogID;
        partial void OnRscrMaintLogIDChanging(Int64 value);
        partial void OnRscrMaintLogIDChanged();


        [DataMemberAttribute()]
        private Int64 _RscrPropertyID;
        public Int64 RscrPropertyID
        {
            get { return _RscrPropertyID; }
            set 
            {
                if (_RscrPropertyID != value)
                {
                    OnRscrPropertyIDChanging(value);
                    _RscrPropertyID = value;
                    RaisePropertyChanged("RscrPropertyID");
                    OnRscrPropertyIDChanged();
                }
            }
        }
        partial void OnRscrPropertyIDChanging(Int64 value);
        partial void OnRscrPropertyIDChanged();


        [DataMemberAttribute()]
        public DateTime RecDateCreated
        {
            get { return _RecDateCreated; }
            set
            {
                if (_RecDateCreated != value)
                {
                    OnRecDateCreatedChanging(value);
                    _RecDateCreated = value;
                    RaisePropertyChanged("RecDateCreated");
                    OnRecDateCreatedChanged();
                }
            }
        }
        private DateTime _RecDateCreated;
        partial void OnRecDateCreatedChanging(DateTime value);
        partial void OnRecDateCreatedChanged();

        [CustomValidation(typeof(ResourceMaintenanceLog), "FuncValidLoggingDate")]
        [Required(ErrorMessage = "Date is required")]
        [DataMemberAttribute()]
        public DateTime LoggingDate
        {
            get { return _LoggingDate; }
            set
            {
                if (_LoggingDate != value)
                {
                    OnLoggingDateChanging(value);
                    ValidateProperty("LoggingDate", value);
                    _LoggingDate = value;
                    RaisePropertyChanged("LoggingDate");
                    OnLoggingDateChanged();
                }
            }
        }
        private DateTime _LoggingDate;
        partial void OnLoggingDateChanging(DateTime value);
        partial void OnLoggingDateChanged();
        //Hàm kiểm tra
        public static ValidationResult FuncValidLoggingDate(DateTime? dateFrom, ValidationContext context)
        {
            if (dateFrom.HasValue)
            {
                if (dateFrom.Value.Date > DateTime.Now.Date)
                {
                    return new ValidationResult("Phải <= ngày hiện tại", new string[] { "LoggingDate" });
                }
            }
            return ValidationResult.Success;
        }
        //Hàm kiểm tra


        [DataMemberAttribute()]
        public string LoggingIssue
        {
            get { return _LoggingIssue; }
            set
            {
                if (_LoggingIssue != value)
                {
                    OnLoggingIssueChanging(value);
                    _LoggingIssue = value;
                    RaisePropertyChanged("LoggingIssue");
                    OnLoggingIssueChanged();
                }
            }
        }
        private string _LoggingIssue;
        partial void OnLoggingIssueChanging(string value);
        partial void OnLoggingIssueChanged();

        
        [DataMemberAttribute()]
        private Int64 _LoggerStaffID;
        public Int64 LoggerStaffID
        {
            get { return _LoggerStaffID; }
            set 
            {
                if (_LoggerStaffID != value)
                {
                    OnLoggerStaffIDChanging(value);
                    _LoggerStaffID = value;
                    RaisePropertyChanged("LoggerStaffID");
                    OnLoggerStaffIDChanged();
                }
            }
        }
        partial void OnLoggerStaffIDChanging(Int64 value);
        partial void OnLoggerStaffIDChanged();        
      
        [DataMemberAttribute()]
        public Staff ObjLoggerStaffID
        {
            get { return _ObjLoggerStaffID; }
            set
            {
                if (_ObjLoggerStaffID != value)
                {
                    OnObjLoggerStaffIDChanging(value);
                    _ObjLoggerStaffID = value;
                    RaisePropertyChanged("ObjLoggerStaffID");
                    OnObjLoggerStaffIDChanged();
                }
            }
        }
        private Staff _ObjLoggerStaffID;
        partial void OnObjLoggerStaffIDChanging(Staff value);
        partial void OnObjLoggerStaffIDChanged();


        [DataMemberAttribute()]
        public Nullable<Int64> AssignStaffID
        {
            get { return _AssignStaffID; }
            set
            {
                if (_AssignStaffID != value)
                {
                    OnAssignStaffIDChanging(value);
                    _AssignStaffID = value;
                    RaisePropertyChanged("AssignStaffID");
                    OnAssignStaffIDChanged();
                }
            }
        }
        private Nullable<Int64> _AssignStaffID;
        partial void OnAssignStaffIDChanging(Nullable<Int64> value);
        partial void OnAssignStaffIDChanged();
       
        [DataMemberAttribute()]
        public Staff ObjAssignStaffID
        {
            get { return _ObjAssignStaffID; }
            set
            {
                if (_ObjAssignStaffID != value)
                {
                    OnObjAssignStaffIDChanging(value);
                    _ObjAssignStaffID = value;
                    RaisePropertyChanged("ObjAssignStaffID");
                    OnObjAssignStaffIDChanged();
                }
            }
        }
        private Staff _ObjAssignStaffID;
        partial void OnObjAssignStaffIDChanging(Staff value);
        partial void OnObjAssignStaffIDChanged();


        [DataMemberAttribute()]
        private Nullable<Int64> _ExternalFixSupplierID;
        public Nullable<Int64> ExternalFixSupplierID
        {
            get { return _ExternalFixSupplierID; }
            set
            {
                if (_ExternalFixSupplierID != value)
                {
                    OnExternalFixSupplierIDChanging(value);
                    _ExternalFixSupplierID = value;
                    RaisePropertyChanged("ExternalFixSupplierID");
                    OnExternalFixSupplierIDChanged();
                }
            }
        }
        partial void OnExternalFixSupplierIDChanging(Nullable<Int64> value);
        partial void OnExternalFixSupplierIDChanged();
        
        [DataMemberAttribute()]
        public Supplier ObjExternalFixSupplierID
        {
            get { return _ObjExternalFixSupplierID; }
            set
            {
                if (_ObjExternalFixSupplierID != value)
                {
                    OnObjExternalFixSupplierIDChanging(value);
                    _ObjExternalFixSupplierID = value;
                    RaisePropertyChanged("ObjExternalFixSupplierID");
                    OnObjExternalFixSupplierIDChanged();
                }
            }
        }
        private Supplier _ObjExternalFixSupplierID;
        partial void OnObjExternalFixSupplierIDChanging(Supplier value);
        partial void OnObjExternalFixSupplierIDChanged();


        [DataMemberAttribute()]
        public string Comments
        {
            get { return _Comments; }
            set
            {
                if (_Comments != value)
                {
                    OnCommentsChanging(value);
                    _Comments = value;
                    RaisePropertyChanged("Comments");
                    OnCommentsChanged();
                }
            }
        }
        private string _Comments;
        partial void OnCommentsChanging(string value);
        partial void OnCommentsChanged();


        [DataMemberAttribute()]
        public Nullable<Int64> V_RscrInitialStatus
        {
            get { return _V_RscrInitialStatus; }
            set
            {
                if (_V_RscrInitialStatus != value)
                {
                    OnV_RscrInitialStatusChanging(value);
                    _V_RscrInitialStatus = value;
                    RaisePropertyChanged("V_RscrInitialStatus");
                    OnV_RscrInitialStatusChanged();
                }
            }
        }
        private Nullable<Int64> _V_RscrInitialStatus;
        partial void OnV_RscrInitialStatusChanging(Nullable<Int64> value);
        partial void OnV_RscrInitialStatusChanged();
       
        [DataMemberAttribute()]
        public Lookup ObjV_RscrInitialStatus
        {
            get { return _ObjV_RscrInitialStatus; }
            set
            {
                if (_ObjV_RscrInitialStatus != value)
                {
                    OnObjV_RscrInitialStatusChanging(value);
                    _ObjV_RscrInitialStatus = value;
                    RaisePropertyChanged("ObjV_RscrInitialStatus");
                    OnObjV_RscrInitialStatusChanged();
                }
            }
        }
        private Lookup _ObjV_RscrInitialStatus;
        partial void OnObjV_RscrInitialStatusChanging(Lookup value);
        partial void OnObjV_RscrInitialStatusChanged();


        [DataMemberAttribute()]
        public Nullable<Int64> FixStaffID
        {
            get
            {
                return _FixStaffID;
            }
            set
            {
                if (_FixStaffID != value)
                {
                    OnFixStaffIDChanging(value);
                    _FixStaffID = value;
                    RaisePropertyChanged("FixStaffID");
                    OnFixStaffIDChanged();
                }
            }
        }
        private Nullable<Int64> _FixStaffID;
        partial void OnFixStaffIDChanging(Nullable<Int64> value);
        partial void OnFixStaffIDChanged();
        
        [DataMemberAttribute()]                        
        public Staff ObjFixStaffID
        {
            get { return _ObjFixStaffID; }
            set 
            {
                if (_ObjFixStaffID != value)
                {
                    OnObjFixStaffIDChanging(value);
                    _ObjFixStaffID = value;
                    RaisePropertyChanged("ObjFixStaffID");
                    OnObjFixStaffIDChanged();
                }
            }
        }
        private Staff _ObjFixStaffID;
        partial void OnObjFixStaffIDChanging(Staff value);
        partial void OnObjFixStaffIDChanged();

        [DataMemberAttribute()]                        
        private Nullable<Int64> _FixSupplierID;
        public Nullable<Int64> FixSupplierID
        {
            get { return _FixSupplierID; }
            set 
            {
                if (_FixSupplierID != value)
                {
                    OnFixSupplierIDChanging(value);
                    _FixSupplierID = value;
                    RaisePropertyChanged("FixSupplierID");
                    OnFixSupplierIDChanged();
                }
            }
        }
        partial void OnFixSupplierIDChanging(Nullable<Int64> value);
        partial void OnFixSupplierIDChanged();

        [DataMemberAttribute()]                        
        private Supplier _ObjFixSupplierID;
        public Supplier ObjFixSupplierID
        {
          get { return _ObjFixSupplierID; }
          set 
          {
              if (_ObjFixSupplierID != value)
              {
                  OnObjFixSupplierIDChanging(value);
                  _ObjFixSupplierID = value;
                  RaisePropertyChanged("ObjFixSupplierID");
                  OnObjFixSupplierIDChanged();
              }
          }
        }
        partial void OnObjFixSupplierIDChanging(Supplier value);
        partial void OnObjFixSupplierIDChanged();


        [DataMemberAttribute()]   
        public Nullable<DateTime> FixDate
        {
            get { return _FixDate; }
            set 
            {
                if (_FixDate != value)
                {
                    OnFixDateChanging(value);
                    _FixDate = value;
                    RaisePropertyChanged("FixDate");
                    OnFixDateChanged();
                }
            }
        }
        private Nullable<DateTime> _FixDate;
        partial void OnFixDateChanging(Nullable<DateTime> value);
        partial void OnFixDateChanged();


        [DataMemberAttribute()]           
        public string FixSolutions
        {
            get { return _FixSolutions; }
            set 
            {
                if (_FixSolutions != value)
                {
                    OnFixSolutionsChanging(value);
                    _FixSolutions = value;
                    RaisePropertyChanged("FixSolutions");
                    OnFixSolutionsChanged();
                }
            }
        }
        private string _FixSolutions;
        partial void OnFixSolutionsChanging(string value);
        partial void OnFixSolutionsChanged();


        [DataMemberAttribute()]                   
        public string FixComments
        {
            get { return _FixComments; }
            set 
            {
                if (_FixComments != value)
                {
                    OnFixCommentsChanging(value);
                    _FixComments = value;
                    RaisePropertyChanged("FixComments");
                    OnFixCommentsChanged();
                }
            }
        }
        private string _FixComments;
        partial void OnFixCommentsChanging(string value);
        partial void OnFixCommentsChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> VerifiedStaffID
        {
            get { return _VerifiedStaffID; }
            set
            {
                if (_VerifiedStaffID != value)
                {
                    OnVerifiedStaffIDChanging(value);
                    _VerifiedStaffID = value;
                    RaisePropertyChanged("VerifiedStaffID");
                    OnVerifiedStaffIDChanged();
                }
            }
        }
        private Nullable<Int64> _VerifiedStaffID;
        partial void OnVerifiedStaffIDChanging(Nullable<Int64> value);
        partial void OnVerifiedStaffIDChanged();

        [DataMemberAttribute()]
        private Staff _ObjVerifiedStaffID;
        public Staff ObjVerifiedStaffID
        {
            get { return _ObjVerifiedStaffID; }
            set
            {
                if (_ObjVerifiedStaffID != value)
                {
                    OnObjVerifiedStaffIDChanging(value);
                    _ObjVerifiedStaffID = value;
                    RaisePropertyChanged("ObjVerifiedStaffID");
                    OnObjVerifiedStaffIDChanged();
                }
            }
        }
        partial void OnObjVerifiedStaffIDChanging(Staff value);
        partial void OnObjVerifiedStaffIDChanged();


        [DataMemberAttribute()]
        public Nullable<Int64> V_RscrFinalStatus
        {
            get { return _V_RscrFinalStatus; }
            set
            {
                if (_V_RscrFinalStatus != value)
                {
                    OnV_RscrFinalStatusChanging(value);
                    _V_RscrFinalStatus = value;
                    RaisePropertyChanged("V_RscrFinalStatus");
                    OnV_RscrFinalStatusChanged();
                }
            }
        }
        private Nullable<Int64> _V_RscrFinalStatus;
        partial void OnV_RscrFinalStatusChanging(Nullable<Int64> value);
        partial void OnV_RscrFinalStatusChanged();
        
        [DataMemberAttribute()]
        public Lookup ObjV_RscrFinalStatus
        {
            get { return _ObjV_RscrFinalStatus; }
            set
            {
                if (_ObjV_RscrFinalStatus != value)
                {
                    OnObjV_RscrFinalStatusChanging(value);
                    _ObjV_RscrFinalStatus = value;
                    RaisePropertyChanged("ObjV_RscrFinalStatus");
                    OnObjV_RscrFinalStatusChanged();
                }
            }
        }
        private Lookup _ObjV_RscrFinalStatus;
        partial void OnObjV_RscrFinalStatusChanging(Lookup value);
        partial void OnObjV_RscrFinalStatusChanged();


        [DataMemberAttribute()]
        public Nullable<bool> IsDeleted
        {
            get { return _IsDeleted; }
            set
            {
                if (_IsDeleted != value)
                {
                    OnIsDeletedChanging(value);
                    _IsDeleted = value;
                    RaisePropertyChanged("IsDeleted");
                    OnIsDeletedChanged();
                }
            }
        }
        private Nullable<bool> _IsDeleted;
        partial void OnIsDeletedChanging(Nullable<bool> value);
        partial void OnIsDeletedChanged();

        #region Navigation Properties
        
        [DataMemberAttribute()]                       
        public Lookup ObjV_CurrentStatus
        {
            get { return _ObjV_CurrentStatus; }
            set 
            {
                if (_ObjV_CurrentStatus != value)
                {
                    OnObjV_CurrentStatusChanging(value);
                    _ObjV_CurrentStatus = value;
                    RaisePropertyChanged("ObjV_CurrentStatus");
                    OnObjV_CurrentStatusChanged();
                }

            }
        }
        private Lookup _ObjV_CurrentStatus;
        partial void OnObjV_CurrentStatusChanging(Lookup value);
        partial void OnObjV_CurrentStatusChanged();       

        [DataMemberAttribute()]
        public string AssignTo
        {
            get { return _AssignTo; }
            set
            {
                if (_AssignTo != value)
                {
                    OnAssignToChanging(value);
                    _AssignTo = value;
                    RaisePropertyChanged("AssignTo");
                    OnAssignToChanged();
                }
            }
        }
        private string _AssignTo;
        partial void OnAssignToChanging(string value);
        partial void OnAssignToChanged();

        [DataMemberAttribute()]        
        public string ObjectFixName
        {
            get { return _ObjectFixName; }
            set 
            {
                if (_ObjectFixName != value)
                {
                    OnObjectFixNameChanging(value);
                    _ObjectFixName = value;
                    RaisePropertyChanged("ObjectFixName");
                    OnObjectFixNameChanged();
                }
            }
        }
        private string _ObjectFixName;
        partial void OnObjectFixNameChanging(string value);
        partial void OnObjectFixNameChanged();


        [DataMemberAttribute()]        
        private Resources _ObjResources;
        public Resources ObjResources
        {
            get { return _ObjResources; }
            set 
            {
                if (_ObjResources != value)
                {
                    OnObjResourcesChanging(value);
                    _ObjResources = value;
                    RaisePropertyChanged("ObjResources");
                    OnObjResourcesChanged();
                }
            }
        }
        partial void OnObjResourcesChanging(Resources value);
        partial void OnObjResourcesChanged();


        [DataMemberAttribute()]        
        private RefDepartments _ObjRefDepartments;
        public RefDepartments ObjRefDepartments
        {
            get { return _ObjRefDepartments; }
            set 
            {
                if (_ObjRefDepartments != value)
                {
                    OnObjRefDepartmentsChanging(value);
                    _ObjRefDepartments = value;
                    RaisePropertyChanged("ObjRefDepartments");
                    OnObjRefDepartmentsChanged();
                }
            }
        }
        partial void OnObjRefDepartmentsChanging(RefDepartments value);
        partial void OnObjRefDepartmentsChanged();


        [DataMemberAttribute()]        
        private Location _ObjLocations;
        public Location ObjLocations
        {
            get { return _ObjLocations; }
            set 
            {
                if (_ObjLocations != value)
                {
                    OnObjLocationsChanging(value);
                    _ObjLocations = value;
                    RaisePropertyChanged("ObjLocations");
                    OnObjLocationsChanged();
                }
            }
        }
        partial void OnObjLocationsChanging(Location value);
        partial void OnObjLocationsChanged();


        [DataMemberAttribute()]        
        private RoomType _ObjRoomType;
        public RoomType ObjRoomType
        {
            get 
            { 
                return _ObjRoomType; 
            }
            set 
            {
                if (_ObjRoomType != value)
                {
                    OnObjRoomTypeChanging(value);
                    _ObjRoomType = value;
                    RaisePropertyChanged("ObjRoomType");
                    OnObjRoomTypeChanged();
                }
            }
        }
        partial void OnObjRoomTypeChanging(RoomType value);
        partial void OnObjRoomTypeChanged();

        [DataMemberAttribute()]        
        private ResourceGroup _ObjResourceGroup;
        public ResourceGroup ObjResourceGroup
        {
            get { return _ObjResourceGroup; }
            set 
            {
                if (_ObjResourceGroup != value)
                {
                    OnObjResourceGroupChanging(value);
                    _ObjResourceGroup = value;
                    RaisePropertyChanged("ObjResourceGroup");
                    OnObjResourceGroupChanged();
                }
            }
        }
        partial void OnObjResourceGroupChanging(ResourceGroup value);
        partial void OnObjResourceGroupChanged();


        [DataMemberAttribute()]
        private ResourceType _ObjResourceType;
        public ResourceType ObjResourceType
        {
            get { return _ObjResourceType; }
            set 
            {
                if (_ObjResourceType != value)
                {
                    OnObjResourceTypeChanging(value);
                    _ObjResourceType = value;
                    RaisePropertyChanged("ObjResourceType");
                    OnObjResourceTypeChanged();
                }
            }
        }
        partial void OnObjResourceTypeChanging(ResourceType value);
        partial void OnObjResourceTypeChanged();
        
        #endregion
    }
}
