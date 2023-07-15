using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
using Service.Core.Common;

namespace DataEntities
{
    public partial class RequestDMedRscrInwardClinicDeptDetail : EntityBase
    {
        #region Factory Method

       
        /// Create a new RequestDMedRscrInwardClinicDeptDetail object.
     
        /// <param name="reqDMedRscrDetailID">Initial value of the ReqDMedRscrDetailID property.</param>
        /// <param name="reqDMedRscrInClinicDeptID">Initial value of the ReqDMedRscrInClinicDeptID property.</param>
        /// <param name="dMedRscrID">Initial value of the DMedRscrID property.</param>
        /// <param name="qty">Initial value of the Qty property.</param>
        public static RequestDMedRscrInwardClinicDeptDetail CreateRequestDMedRscrInwardClinicDeptDetail(Int64 reqDMedRscrDetailID, Int64 reqDMedRscrInClinicDeptID, Int64 dMedRscrID, Int32 qty)
        {
            RequestDMedRscrInwardClinicDeptDetail requestDMedRscrInwardClinicDeptDetail = new RequestDMedRscrInwardClinicDeptDetail();
            requestDMedRscrInwardClinicDeptDetail.ReqDMedRscrDetailID = reqDMedRscrDetailID;
            requestDMedRscrInwardClinicDeptDetail.ReqDMedRscrInClinicDeptID = reqDMedRscrInClinicDeptID;
            requestDMedRscrInwardClinicDeptDetail.DMedRscrID = dMedRscrID;
            requestDMedRscrInwardClinicDeptDetail.Qty = qty;
            return requestDMedRscrInwardClinicDeptDetail;
        }

        #endregion
        #region Primitive Properties

       
      
     
       
        [DataMemberAttribute()]
        public Int64 ReqDMedRscrDetailID
        {
            get
            {
                return _ReqDMedRscrDetailID;
            }
            set
            {
                if (_ReqDMedRscrDetailID != value)
                {
                    OnReqDMedRscrDetailIDChanging(value);
                    //ReportPropertyChanging("ReqDMedRscrDetailID");
                    _ReqDMedRscrDetailID = value;
                    RaisePropertyChanged("ReqDMedRscrDetailID");
                    OnReqDMedRscrDetailIDChanged();
                }
            }
        }
        private Int64 _ReqDMedRscrDetailID;
        partial void OnReqDMedRscrDetailIDChanging(Int64 value);
        partial void OnReqDMedRscrDetailIDChanged();

       
      
     
       
        [DataMemberAttribute()]
        public Int64 ReqDMedRscrInClinicDeptID
        {
            get
            {
                return _ReqDMedRscrInClinicDeptID;
            }
            set
            {
                OnReqDMedRscrInClinicDeptIDChanging(value);
                //ReportPropertyChanging("ReqDMedRscrInClinicDeptID");
                _ReqDMedRscrInClinicDeptID = value;
                RaisePropertyChanged("ReqDMedRscrInClinicDeptID");
                OnReqDMedRscrInClinicDeptIDChanged();
            }
        }
        private Int64 _ReqDMedRscrInClinicDeptID;
        partial void OnReqDMedRscrInClinicDeptIDChanging(Int64 value);
        partial void OnReqDMedRscrInClinicDeptIDChanged();

       
      
     
       
        [DataMemberAttribute()]
        public Int64 DMedRscrID
        {
            get
            {
                return _DMedRscrID;
            }
            set
            {
                OnDMedRscrIDChanging(value);
                //ReportPropertyChanging("DMedRscrID");
                _DMedRscrID = value;
                RaisePropertyChanged("DMedRscrID");
                OnDMedRscrIDChanged();
            }
        }
        private Int64 _DMedRscrID;
        partial void OnDMedRscrIDChanging(Int64 value);
        partial void OnDMedRscrIDChanged();

       
      
     
       
        [DataMemberAttribute()]
        public Int32 Qty
        {
            get
            {
                return _Qty;
            }
            set
            {
                OnQtyChanging(value);
                //ReportPropertyChanging("Qty");
                _Qty = value;
                RaisePropertyChanged("Qty");
                OnQtyChanged();
            }
        }
        private Int32 _Qty;
        partial void OnQtyChanging(Int32 value);
        partial void OnQtyChanged();

       
      
     
     
        [DataMemberAttribute()]
        public String Note
        {
            get
            {
                return _Note;
            }
            set
            {
                OnNoteChanging(value);
                //ReportPropertyChanging("Note");
                _Note = value;
                RaisePropertyChanged("Note");
                OnNoteChanged();
            }
        }
        private String _Note;
        partial void OnNoteChanging(String value);
        partial void OnNoteChanged();

        #endregion

        #region Navigation Properties

        private RefDisposableMedicalResource _RefDisposableMedicalResource;
        [Required(ErrorMessage = "Please select one Drug Name")]
        [DataMemberAttribute()]
        public RefDisposableMedicalResource RefDisposableMedicalResource
        {
            get
            {
                if (_RefDisposableMedicalResource != null)
                {
                    _DMedRscrID = _RefDisposableMedicalResource.DMedRscrID;
                }
                return _RefDisposableMedicalResource;
            }
            set
            {
                if (_RefDisposableMedicalResource != value)
                {
                    _RefDisposableMedicalResource = value;
                    ValidateProperty("RefDisposableMedicalResource", value);
                    if (_RefDisposableMedicalResource != null)
                    {
                        _DMedRscrID = _RefDisposableMedicalResource.DMedRscrID;
                    }
                    RaisePropertyChanged("RefDisposableMedicalResource");
                }
            }
        }

        #endregion

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

    }
}
