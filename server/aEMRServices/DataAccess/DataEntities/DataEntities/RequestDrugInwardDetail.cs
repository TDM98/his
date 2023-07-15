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
    public partial class RequestDrugInwardDetail : EntityBase
    {
        #region Factory Method

       
        /// Create a new RequestDrugInwardDetail object.
     
        /// <param name="ReqDetailID">Initial value of the ReqDetailID property.</param>
        /// <param name="ReqDrugInID">Initial value of the ReqDrugInID property.</param>
        /// <param name="drugID">Initial value of the DrugID property.</param>
        /// <param name="qty">Initial value of the Qty property.</param>
        public static RequestDrugInwardDetail CreateRequestDrugInwardDetail(Int64 ReqDetailID, Int64 ReqDrugInID, Int32 qty)
        {
            RequestDrugInwardDetail RequestDrugInwardDetail = new RequestDrugInwardDetail();
            RequestDrugInwardDetail.ReqDetailID = ReqDetailID;
            RequestDrugInwardDetail.ReqDrugInID = ReqDrugInID;
            RequestDrugInwardDetail.Qty = qty;
            return RequestDrugInwardDetail;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 ReqDetailID
        {
            get
            {
                return _ReqDetailID;
            }
            set
            {
                if (_ReqDetailID != value)
                {
                    OnReqDetailIDChanging(value);
                    _ReqDetailID = value;
                    RaisePropertyChanged("ReqDetailID");
                    OnReqDetailIDChanged();
                }
            }
        }
        private Int64 _ReqDetailID;
        partial void OnReqDetailIDChanging(Int64 value);
        partial void OnReqDetailIDChanged();

        [DataMemberAttribute()]
        public Int64 ReqDrugInID
        {
            get
            {
                return _ReqDrugInID;
            }
            set
            {
                OnReqDrugInIDChanging(value);
                _ReqDrugInID = value;
                RaisePropertyChanged("ReqDrugInID");
                OnReqDrugInIDChanged();
            }
        }
        private Int64 _ReqDrugInID;
        partial void OnReqDrugInIDChanging(Int64 value);
        partial void OnReqDrugInIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> DrugID
        {
            get
            {
                return _DrugID;
            }
            set
            {
                if (_DrugID != value)
                {
                    OnDrugIDChanging(value);
                    ValidateProperty("DrugID", value);
                    _DrugID = value;
                    RaisePropertyChanged("DrugID");
                    OnDrugIDChanged();
                }
            }
        }
        private Nullable<long> _DrugID;
        partial void OnDrugIDChanging(Nullable<long> value);
        partial void OnDrugIDChanged();

        [Range(0.0, 99999999999.0, ErrorMessage = "Số lượng không được nhỏ hơn 0")]
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
                ValidateProperty("Qty",value);
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

        
        private RefGenericDrugDetail _RefGenericDrugDetail;
        [DataMemberAttribute()]
        public RefGenericDrugDetail RefGenericDrugDetail
        {
            get
            {
                return _RefGenericDrugDetail;
            }
            set
            {
                if (_RefGenericDrugDetail != value)
                {
                    _RefGenericDrugDetail = value;
                    if (_RefGenericDrugDetail != null)
                    {
                        _DrugID = _RefGenericDrugDetail.DrugID;
                    }
                    else
                    {
                        _DrugID = null;
                    }
                    RaisePropertyChanged("RefGenericDrugDetail");
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

        #region extension
        [DataMemberAttribute()]
        public int QtyOutward
        {
            get
            {
                return _QtyOutward;
            }
            set
            {
                _QtyOutward = value;
                RaisePropertyChanged("QtyOutward");
            }
        }
        private int _QtyOutward;

        [DataMemberAttribute()]
        public int QtyRemaining
        {
            get
            {
                return _QtyRemaining;
            }
            set
            {
                _QtyRemaining = value;
                RaisePropertyChanged("QtyRemaining");
            }
        }
        private int _QtyRemaining;
        #endregion 

    }
}
