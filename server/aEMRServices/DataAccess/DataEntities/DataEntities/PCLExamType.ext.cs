using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

using System.Text;
using System.Collections.Generic;

namespace DataEntities
{
    public partial class PCLExamType : NotifyChangedBase, IChargeableItemPrice, IGenericService
    {        
        #region Extended Primitive Properties
        
        [DataMemberAttribute()]
        public PCLGroup ObjPCLGroupID
        {
            get { return _ObjPCLGroupID; }
            set
            {
                if(_ObjPCLGroupID!=value)
                {
                    OnObjPCLGroupIDChanging(value);
                    _ObjPCLGroupID = value;
                    RaisePropertyChanged("ObjPCLGroupID");
                    OnObjPCLGroupIDChanged();
                }

            }
        }
        private PCLGroup _ObjPCLGroupID;
        partial void OnObjPCLGroupIDChanging(PCLGroup value);
        partial void OnObjPCLGroupIDChanged();



        [DataMemberAttribute()]
        public Nullable<Boolean> IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                if (_IsSelected != value)
                {
                    OnIsSelectedChanging(value);
                    _IsSelected = value;
                    RaisePropertyChanged("IsSelected");
                    OnIsSelectedChanged();
                }
            }
        }
        private Nullable<Boolean> _IsSelected;
        partial void OnIsSelectedChanging(Nullable<Boolean> value);
        partial void OnIsSelectedChanged();

        [DataMemberAttribute()]
        public Boolean IsHaveRequest
        {
            get
            {
                return _IsHaveRequest;
            }
            set
            {
                if (_IsHaveRequest != value)
                {
                    _IsHaveRequest = value;
                    RaisePropertyChanged("IsHaveRequest");
                }
            }
        }
        private Boolean _IsHaveRequest;
   

        [DataMemberAttribute()]
        public long PCLExamTypeComboID
        {
            get
            {
                return _PCLExamTypeComboID;
            }
            set
            {
                if (_PCLExamTypeComboID != value)
                {
                    OnPCLExamTypeComboIDChanging(value);
                    _PCLExamTypeComboID = value;
                    RaisePropertyChanged("PCLExamTypeComboID");
                    OnPCLExamTypeComboIDChanged();
                }
            }
        }
        private long _PCLExamTypeComboID;
        partial void OnPCLExamTypeComboIDChanging(long value);
        partial void OnPCLExamTypeComboIDChanged();

        #region FromPCLItems
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
        public Nullable<Byte> Idx
        {
            get
            {
                return _Idx;
            }
            set
            {
                OnIdxChanging(value);
                _Idx = value;
                RaisePropertyChanged("Idx");
                OnIdxChanged();
            }
        }
        private Nullable<Byte> _Idx;
        partial void OnIdxChanging(Nullable<Byte> value);
        partial void OnIdxChanged();

        #endregion

        #region FRomRefMedServiceItem
      
        [DataMemberAttribute()]
        public Decimal ChildrenUnderSixPrice
        {
            get
            {
                return _ChildrenUnderSixPrice;
            }
            set
            {
                OnChildrenUnderSixPriceChanging(value);
                _ChildrenUnderSixPrice = value;
                RaisePropertyChanged("ChildrenUnderSixPrice");
                OnChildrenUnderSixPriceChanged();
            }
        }
        private Decimal _ChildrenUnderSixPrice;
        partial void OnChildrenUnderSixPriceChanging(Decimal value);
        partial void OnChildrenUnderSixPriceChanged();


        private decimal _PriceDifference;
        public decimal PriceDifference
        {
            get
            {
                return _PriceDifference;
            }
            set
            {
                _PriceDifference = value;
                RaisePropertyChanged("PriceDifference");
            }
        }
        private decimal _CoPayment;
        public decimal CoPayment
        {
            get
            {
                return _CoPayment;
            }
            set
            {
                _CoPayment = value;
                RaisePropertyChanged("CoPayment");
            }
        }

        private decimal _HIPayment;
        public decimal HIPayment
        {
            get
            {
                return _HIPayment;
            }
            set
            {
                _HIPayment = value;
                RaisePropertyChanged("HIPayment");
            }
        }

        private decimal _PatientPayment;
        public decimal PatientPayment
        {
            get
            {
                return _PatientPayment;
            }
            set
            {
                _PatientPayment = value;
                RaisePropertyChanged("PatientPayment");
            }
        }

        private byte _Qty;
        public byte Qty
        {
            get
            {
                return _Qty;
            }
            set
            {
                _Qty = value;
                RaisePropertyChanged("Qty");
            }
        }
        //[DataMemberAttribute()]
        //public Decimal HIPrice
        //{
        //    get
        //    {
        //        return _HIPrice;
        //    }
        //    set
        //    {
        //        if (_HIPrice != value)
        //        {
        //            _HIPrice = value;
        //            RaisePropertyChanged("HIPrice"); 
        //        }
        //    }
        //}
        //private Decimal _HIPrice;
        public void CalculatePayment(InsuranceBenefit benefit)
        {
            if (benefit != null && _hiAllowedPrice.HasValue && _hiAllowedPrice.Value > 0)
            {
                PriceDifference = NormalPrice - _hiAllowedPrice.Value;
                if (PriceDifference < 0)
                {
                    PriceDifference = 0;
                }

                CoPayment = NormalPrice - PriceDifference;

                HIPayment = (decimal)benefit.RebatePercentage * CoPayment;

                PatientPayment = NormalPrice - HIPayment;
            }
            else
            {
                PriceDifference = 0;
                CoPayment = 0;
                HIPayment = 0;
                PatientPayment = NormalPrice;
            }
        }

        #endregion
        #endregion

        #region XMLparsing GetInfo for PatientPClAppointmentIndicator

        public static string[] Nodes = new string[5] { "PCLItemID", "NumberOfTest","Price","PriceDifference","HIAllowedPrice" };
        public static string[] Fields = new string[5] { "PCLItemID", "Qty","NormalPrice","PriceDifference","HIAllowedPrice"};
        public static bool IsContainsField(string fieldName)
        {
            try
            {
                return Array.IndexOf(Fields, fieldName) >= 0 ? true : false;
            }
            catch
            {
                return false;
            }
        }

        private static string BuildXmlString(string xmlRootName, string values)
        {
            StringBuilder xmlString = new StringBuilder();
            xmlString.AppendFormat("<" + xmlRootName + ">{0}</" + xmlRootName + ">", values);
            return xmlString.ToString();
        }

        private static string FormatFieldXML(string data, string xNode)
        {
            if (data != null)
                return String.Format(" " + xNode + "='{0}'", data);
            else
                return "";
        }

        private static void BuildXMLStringOfRow(int rowIdx, StringBuilder strBuilder, List<string> lstFields)
        {
            //strBuilder.Append("<?xml version=''1.0'' encoding=''UTF-8''?>");
            strBuilder.Append("<Row");
            strBuilder.Append(FormatFieldXML(rowIdx.ToString().Trim(), "RowID"));
            strBuilder.Append(String.Join("", lstFields.ToArray()));
            strBuilder.Append(" />");
        }

        public static string ConvertToXML(List<PCLExamType> PCLExamTypeObj)
        {
            StringBuilder strBuilder = new StringBuilder();
            List<string> lstData = new List<string>();
            int RowIdx = 0;
            foreach (PCLExamType p in PCLExamTypeObj)
            {
                RowIdx++;
                Type oType = p.GetType();
                lstData.Clear();
                if (p.IsSelected != null && p.IsSelected == true)
                {
                    foreach (System.Reflection.PropertyInfo r in oType.GetProperties())
                    {
                        if (PCLExamType.IsContainsField(r.Name))
                        {
                            string propertyName = r.Name.Replace(" ", "");
                            string propertyValue = "";
                            object objectValue = null;
                            if (r.CanRead)
                            {
                                objectValue = r.GetValue(p, null);
                                try
                                {
                                    propertyValue = objectValue.ToString();
                                }
                                catch
                                {
                                    propertyValue = null;
                                }
                            }
                            if (objectValue != null)
                                //lstData.Add(FormatFieldXML(propertyValue, propertyName));
                                lstData.Add(FormatFieldXML(propertyValue, Nodes[Array.IndexOf(Fields, propertyName)]));
                        }
                    }
                    BuildXMLStringOfRow(RowIdx, strBuilder, lstData);                    
                }

            }
            string sResult = BuildXmlString("Tbl", strBuilder.ToString());
            return sResult;
        }
        #endregion
        
        #region IChargeableItemPrice Members
        private decimal _normalPrice;
        [DataMemberAttribute()]
        public decimal NormalPrice
        {
            get
            {
                return _normalPrice;
            }
            set
            {
                _normalPrice = value;
                RaisePropertyChanged("NormalPrice");
            }
        }

        private decimal _hiPatientPrice;
        [DataMemberAttribute()]
        public decimal HIPatientPrice
        {
            get
            {
                return _hiPatientPrice;
            }
            set
            {
                _hiPatientPrice = value;
                RaisePropertyChanged("HIPatientPrice");
            }
        }

        private decimal? _hiAllowedPrice;
        [DataMemberAttribute()]
        public decimal? HIAllowedPrice
        {
            get
            {
                return _hiAllowedPrice;
            }
            set
            {
                _hiAllowedPrice = value;
                RaisePropertyChanged("HIAllowedPrice");
            }
        }

        private ChargeableItemType _chargeableItemType;
        [DataMemberAttribute()]
        public ChargeableItemType ChargeableItemType
        {
            get
            {
                return DataEntities.ChargeableItemType.PCL;
            }
            set
            {
                _chargeableItemType = value;
            }
        }

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

        public string GetCode()
        {
            return PCLExamTypeCode;
        }

        public override string ToString()
        {
            return PCLExamTypeName;
        }

        private ObservableCollection<PCLTimeSegment> _apptPclTimeSegments;

        /// <summary>
        /// De lam phan hen benh.
        /// </summary>
        public ObservableCollection<PCLTimeSegment> ApptPclTimeSegments
        {
            get { return _apptPclTimeSegments; }
            set
            {
                _apptPclTimeSegments = value;
                RaisePropertyChanged("ApptPclTimeSegments");
            }
        }
    }
}
