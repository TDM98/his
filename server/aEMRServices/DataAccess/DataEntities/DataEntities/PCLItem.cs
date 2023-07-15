using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.Collections.Generic;
using System.Text;

namespace DataEntities
{
    public partial class PCLItem : NotifyChangedBase, IChargeableItemPrice
    {
        #region Factory Method


        /// Create a new PCLItem object.

        /// <param name="pCLItemID">Initial value of the PCLItemID property.</param>
        /// <param name="pCLSectionID">Initial value of the PCLSectionID property.</param>
        /// <param name="pCLExamTypeID">Initial value of the PCLExamTypeID property.</param>
        public static PCLItem CreatePCLItem(long pCLItemID, long pCLSectionID, Int64 pCLExamTypeID)
        {
            PCLItem pCLItem = new PCLItem();
            pCLItem.PCLItemID = pCLItemID;
            pCLItem.PCLSectionID = pCLSectionID;
            pCLItem.PCLExamTypeID = pCLExamTypeID;
            return pCLItem;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long PCLItemID
        {
            get
            {
                return _PCLItemID;
            }
            set
            {
                if (_PCLItemID != value)
                {
                    OnPCLItemIDChanging(value);
                    ////ReportPropertyChanging("PCLItemID");
                    _PCLItemID = value;
                    RaisePropertyChanged("PCLItemID");
                    OnPCLItemIDChanged();
                }
            }
        }
        private long _PCLItemID;
        partial void OnPCLItemIDChanging(long value);
        partial void OnPCLItemIDChanged();





        [DataMemberAttribute()]
        public long PCLSectionID
        {
            get
            {
                return _PCLSectionID;
            }
            set
            {
                OnPCLSectionIDChanging(value);
                ////ReportPropertyChanging("PCLSectionID");
                _PCLSectionID = value;
                RaisePropertyChanged("PCLSectionID");
                OnPCLSectionIDChanged();
            }
        }
        private long _PCLSectionID;
        partial void OnPCLSectionIDChanging(long value);
        partial void OnPCLSectionIDChanged();





        [DataMemberAttribute()]
        public Int64 PCLExamTypeID
        {
            get
            {
                return _PCLExamTypeID;
            }
            set
            {
                OnPCLExamTypeIDChanging(value);
                ////ReportPropertyChanging("PCLExamTypeID");
                _PCLExamTypeID = value;
                RaisePropertyChanged("PCLExamTypeID");
                OnPCLExamTypeIDChanged();
            }
        }
        private Int64 _PCLExamTypeID;
        partial void OnPCLExamTypeIDChanging(Int64 value);
        partial void OnPCLExamTypeIDChanged();



        [DataMemberAttribute()]
        public Nullable<Byte> Idx
        {
            get
            {
                return _Idx;
            }
            set
            {
                OnIdxChanging(value);
                ////ReportPropertyChanging("Idx");
                _Idx = value;
                RaisePropertyChanged("Idx");
                OnIdxChanged();
            }
        }
        private Nullable<Byte> _Idx;
        partial void OnIdxChanging(Nullable<Byte> value);
        partial void OnIdxChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PATIENTP_REL_REQPC_PCLITEMS", "PatientPCLRequestDetail")]
        public ObservableCollection<PatientPCLRequestDetail> PatientPCLRequestDetail
        {
            get;
            set;
        }





       
        [DataMemberAttribute()]
        public PCLExamType PCLExamType
        {
            get
            {
                return _PCLExamType;
            }
            set
            {
                if (_PCLExamType != value)
                {
                    _PCLExamType = value;
                    RaisePropertyChanged("PCLExamType"); 
                }
            }
        }
        private PCLExamType _PCLExamType;
        [DataMemberAttribute()]
        public PCLSection PCLSection
        {
            get { return _PCLSection; }
            set 
            {
                if (_PCLSection != value)
                {
                    _PCLSection = value;
                    RaisePropertyChanged("PCLExamType");
                }
            }
        }
        private PCLSection _PCLSection;

        
        //For Insert PCLItems: Danh sách PCLSection để cho chọn ứng với 1 PCLItems(1 Row=1PCLItem)
        [DataMemberAttribute()]
        public ObservableCollection<PCLSection> ObjListPCLSection
        {
            get { return _ObjListPCLSection; }
            set 
            {
                OnObjListPCLSectionChanging(value);
                _ObjListPCLSection = value;
                RaisePropertyChanged("ObjListPCLSection");
                OnObjListPCLSectionChanged();
            }
        }
        private ObservableCollection<PCLSection> _ObjListPCLSection;
        partial void OnObjListPCLSectionChanging(ObservableCollection<PCLSection> value);
        partial void OnObjListPCLSectionChanged();


        private ObservableCollection<PCLForm> _ObjListPCLForm;
        public ObservableCollection<PCLForm> ObjListPCLForm
        {
            get { return _ObjListPCLForm; }
            set
            {
                OnObjListPCLFormChanging(value);
                _ObjListPCLForm = value;
                RaisePropertyChanged("ObjListPCLForm");
                OnObjListPCLFormChanged();
            }
        }
        partial void OnObjListPCLFormChanging(ObservableCollection<PCLForm> value);
        partial void OnObjListPCLFormChanged();

        
        [DataMemberAttribute()]
        public string KeyIDtmp
        {
            get { return _KeyIDtmp; }
            set 
            {
                OnKeyIDtmpChanging(value);
                _KeyIDtmp = value;
                RaisePropertyChanged("KeyIDtmp");
                OnKeyIDtmpChanged();
            }
        }
        private string _KeyIDtmp;
        partial void OnKeyIDtmpChanging(string value);
        partial void OnKeyIDtmpChanged();
        //For Insert PCLItems


        #endregion


        #region IChargeableItemPrice Members
        [DataMemberAttribute()]
        public decimal NormalPrice
        {
            get
            {
                return _NormalPrice;
            }
            set
            {
                _NormalPrice = value;
                RaisePropertyChanged("NormalPrice");
            }
        }
        private decimal _NormalPrice;
       
        [DataMemberAttribute()]
        public decimal HIPatientPrice
        {
            get
            {
                return _HIPatientPrice;
            }
            set
            {
                _HIPatientPrice = value;
                RaisePropertyChanged("HIPatientPrice");
            }
        }
        private decimal _HIPatientPrice;
       
        [DataMemberAttribute()]
        public decimal? HIAllowedPrice
        {
            get
            {
                return _HIAllowedPrice;
            }
            set
            {
                _HIAllowedPrice = value;
                RaisePropertyChanged("HIAllowedPrice");
            }
        }
        private decimal? _HIAllowedPrice;
       
        [DataMemberAttribute()]
        public ChargeableItemType ChargeableItemType
        {
            get
            {
                return DataEntities.ChargeableItemType.PCL;
            }
            set
            {
                _ChargeableItemType = value;
            }
        }
        private ChargeableItemType _ChargeableItemType;

        [DataMemberAttribute()]
        public decimal NormalPriceNew
        {
            get
            {
                return _NormalPriceNew;
            }
            set
            {
                _NormalPriceNew = value;
                RaisePropertyChanged("NormalPriceNew");
            }
        }
        private decimal _NormalPriceNew;

        [DataMemberAttribute()]
        public decimal HIPatientPriceNew
        {
            get
            {
                return _HIPatientPriceNew;
            }
            set
            {
                _HIPatientPriceNew = value;
                RaisePropertyChanged("HIPatientPriceNew");
            }
        }
        private decimal _HIPatientPriceNew;

        [DataMemberAttribute()]
        public decimal? HIAllowedPriceNew
        {
            get
            {
                return _HIAllowedPriceNew;
            }
            set
            {
                _HIAllowedPriceNew = value;
                RaisePropertyChanged("HIAllowedPriceNew");
            }
        }
        private decimal? _HIAllowedPriceNew;
        #endregion

        public override string ToString()
        {
            if (_PCLExamType != null)
            {
                return _PCLExamType.PCLExamTypeName;
            }
            return base.ToString();
        }
    }
}
