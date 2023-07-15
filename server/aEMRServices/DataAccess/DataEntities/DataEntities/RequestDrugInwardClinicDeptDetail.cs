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
    public partial class RequestDrugInwardClinicDeptDetail : EntityBase
    {
        #region Factory Method

       
        /// Create a new RequestDrugInwardClinicDeptDetail object.
     
        /// <param name="reqDrugInDetailID">Initial value of the ReqDrugInDetailID property.</param>
        /// <param name="reqDrugInClinicDeptID">Initial value of the ReqDrugInClinicDeptID property.</param>
        /// <param name="drugID">Initial value of the DrugID property.</param>
        /// <param name="qty">Initial value of the Qty property.</param>
        public static RequestDrugInwardClinicDeptDetail CreateRequestDrugInwardClinicDeptDetail(Int64 reqDrugInDetailID, Int64 reqDrugInClinicDeptID, Int32 qty)
        {
            RequestDrugInwardClinicDeptDetail requestDrugInwardClinicDeptDetail = new RequestDrugInwardClinicDeptDetail();
            requestDrugInwardClinicDeptDetail.ReqDrugInDetailID = reqDrugInDetailID;
            requestDrugInwardClinicDeptDetail.ReqDrugInClinicDeptID = reqDrugInClinicDeptID;
            requestDrugInwardClinicDeptDetail.Qty = qty;
            return requestDrugInwardClinicDeptDetail;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 ReqDrugInDetailID
        {
            get
            {
                return _ReqDrugInDetailID;
            }
            set
            {
                if (_ReqDrugInDetailID != value)
                {
                    OnReqDrugInDetailIDChanging(value);
                    _ReqDrugInDetailID = value;
                    RaisePropertyChanged("ReqDrugInDetailID");
                    OnReqDrugInDetailIDChanged();
                }
            }
        }
        private Int64 _ReqDrugInDetailID;
        partial void OnReqDrugInDetailIDChanging(Int64 value);
        partial void OnReqDrugInDetailIDChanged();

        [DataMemberAttribute()]
        public Int64 ReqDrugInClinicDeptID
        {
            get
            {
                return _ReqDrugInClinicDeptID;
            }
            set
            {
                OnReqDrugInClinicDeptIDChanging(value);
                _ReqDrugInClinicDeptID = value;
                RaisePropertyChanged("ReqDrugInClinicDeptID");
                OnReqDrugInClinicDeptIDChanged();
            }
        }
        private Int64 _ReqDrugInClinicDeptID;
        partial void OnReqDrugInClinicDeptIDChanging(Int64 value);
        partial void OnReqDrugInClinicDeptIDChanged();

        [Required(ErrorMessage = "Bạn phải nhập hàng vào")]
        [DataMemberAttribute()]
        public Nullable<long> GenMedProductID
        {
            get
            {
                return _GenMedProductID;
            }
            set
            {
                if (_GenMedProductID != value)
                {
                    OnDrugIDChanging(value);
                    ValidateProperty("GenMedProductID", value);
                    _GenMedProductID = value;
                    RaisePropertyChanged("GenMedProductID");
                    OnDrugIDChanged();
                }
            }
        }
        private Nullable<long> _GenMedProductID;
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

        private RefGenMedProductDetails _RefGenericDrugDetail;
        [DataMemberAttribute()]
        public RefGenMedProductDetails RefGenericDrugDetail
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
                        _GenMedProductID = _RefGenericDrugDetail.GenMedProductID;
                    }
                    else
                    {
                        _GenMedProductID = null;
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
