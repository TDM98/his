using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.Collections.Generic;
using System.Text;

namespace DataEntities
{
    public partial class OutwardDrugClinicDeptTemplate : NotifyChangedBase
    {
        #region Factory Method

        public static OutwardDrugClinicDeptTemplate CreateOutwardDrugClinicDeptTemplate(Int64 outwardDrugClinicDeptTemplateID, string outwardDrugClinicDeptTemplateName, Int64 V_MedProductType)
        {
            OutwardDrugClinicDeptTemplate outwardDrugClinicDeptTemplate = new OutwardDrugClinicDeptTemplate();
            outwardDrugClinicDeptTemplate.OutwardDrugClinicDeptTemplateID = outwardDrugClinicDeptTemplateID;
            outwardDrugClinicDeptTemplate.OutwardDrugClinicDeptTemplateName = outwardDrugClinicDeptTemplateName;
            outwardDrugClinicDeptTemplate.V_MedProductType = V_MedProductType;
            return outwardDrugClinicDeptTemplate;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 OutwardDrugClinicDeptTemplateID
        {
            get
            {
                return _OutwardDrugClinicDeptTemplateID;
            }
            set
            {
                if (_OutwardDrugClinicDeptTemplateID != value)
                {
                    OnOutwardDrugClinicDeptTemplateIDChanging(value);
                    _OutwardDrugClinicDeptTemplateID = value;
                    RaisePropertyChanged("OutwardDrugClinicDeptTemplateID");
                    OnOutwardDrugClinicDeptTemplateIDChanged();
                }
            }
        }
        private Int64 _OutwardDrugClinicDeptTemplateID;
        partial void OnOutwardDrugClinicDeptTemplateIDChanging(Int64 value);
        partial void OnOutwardDrugClinicDeptTemplateIDChanged();

        [DataMemberAttribute()]
        public string OutwardDrugClinicDeptTemplateName
        {
            get
            {
                return _OutwardDrugClinicDeptTemplateName;
            }
            set
            {
                if (_OutwardDrugClinicDeptTemplateName != value)
                {
                    OnOutwardDrugClinicDeptTemplateNameChanging(value);
                    _OutwardDrugClinicDeptTemplateName = value;
                    RaisePropertyChanged("OutwardDrugClinicDeptTemplateName");
                    OnOutwardDrugClinicDeptTemplateNameChanged();
                }
            }
        }
        private string _OutwardDrugClinicDeptTemplateName;
        partial void OnOutwardDrugClinicDeptTemplateNameChanging(string value);
        partial void OnOutwardDrugClinicDeptTemplateNameChanged();

        [DataMemberAttribute()]
        public Staff CreatedStaff
        {
            get
            {
                return _CreatedStaff;
            }
            set
            {
                _CreatedStaff = value;
                RaisePropertyChanged("CreatedStaff");
            }
        }
        private Staff _CreatedStaff;


        [DataMemberAttribute()]
        public Int64 V_MedProductType
        {
            get
            {
                return _V_MedProductType;
            }
            set
            {
                OnV_MedProductTypeChanging(value);
                _V_MedProductType = value;
                RaisePropertyChanged("V_MedProductType");
                OnV_MedProductTypeChanged();
            }
        }
        private Int64 _V_MedProductType;
        partial void OnV_MedProductTypeChanging(Int64 value);
        partial void OnV_MedProductTypeChanged();

        [DataMemberAttribute()]
        public RefDepartment Department
        {
            get
            {
                return _Department;
            }
            set
            {
                _Department = value;
                RaisePropertyChanged("Department");
            }
        }
        private RefDepartment _Department;

        [DataMemberAttribute()]
        public DateTime CreateDate
        {
            get
            {
                return _CreateDate;
            }
            set
            {
                _CreateDate = value;
                RaisePropertyChanged("CreateDate");
            }
        }
        private DateTime _CreateDate;

        private long _V_OutwardTemplateType;
        [DataMemberAttribute]
        public long V_OutwardTemplateType
        {
            get
            {
                return _V_OutwardTemplateType;
            }
            set
            {
                if (_V_OutwardTemplateType == value)
                {
                    return;
                }
                _V_OutwardTemplateType = value;
                RaisePropertyChanged("V_OutwardTemplateType");
            }
        }
        [DataMemberAttribute()]
        public bool IsShared
        {
            get
            {
                return _IsShared;
            }
            set
            {
                _IsShared = value;
                RaisePropertyChanged("IsShared");
            }
        }
        private bool _IsShared;
        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public ObservableCollection<OutwardDrugClinicDeptTemplateItem> OutwardTemplateItems
        {
            get
            {
                return _OutwardTemplateItems;
            }
            set
            {
                if (_OutwardTemplateItems != value)
                {
                    _OutwardTemplateItems = value;
                    RaisePropertyChanged("OutwardTemplateItems");
                }
            }
        }
        private ObservableCollection<OutwardDrugClinicDeptTemplateItem> _OutwardTemplateItems;


        [DataMemberAttribute()]
        public ObservableCollection<OutwardDrugClinicDeptTemplateItem> OutwardTemplateItems_Behind
        {
            get
            {
                return _OutwardTemplateItems_Behind;
            }
            set
            {
                if (_OutwardTemplateItems_Behind != value)
                {
                    _OutwardTemplateItems_Behind = value;
                    RaisePropertyChanged("OutwardTemplateItems_Behind");
                }
            }
        }
        private ObservableCollection<OutwardDrugClinicDeptTemplateItem> _OutwardTemplateItems_Behind;
        #endregion

        #region convert XML member

        public string ConvertDetailsListToXml()
        {
            return ConvertDetailsListToXml(_OutwardTemplateItems_Behind);
        }
        public string ConvertDetailsListToXml(IEnumerable<OutwardDrugClinicDeptTemplateItem> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<OutwardTemplateItems>");
                foreach (OutwardDrugClinicDeptTemplateItem details in items)
                {
                    sb.Append("<RecInfo>");

                    sb.AppendFormat("<OutwardDrugClinicDeptTemplateItemID>{0}</OutwardDrugClinicDeptTemplateItemID>", details.OutwardDrugClinicDeptTemplateItemID);
                    sb.AppendFormat("<GenMedProductID>{0}</GenMedProductID>", details.GenMedProductID);
                    sb.AppendFormat("<ReqOutQuantity>{0}</ReqOutQuantity>", details.ReqOutQuantity);
                    sb.AppendFormat("<ItemNote>{0}</ItemNote>", details.ItemNote);
                    sb.AppendFormat("<V_RecordState>{0}</V_RecordState>", details.V_RecordState);
                    sb.Append("</RecInfo>");
                }
                sb.Append("</OutwardTemplateItems>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}