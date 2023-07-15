using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using System.Text;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public class PharmacyReferencePriceList : NotifyChangedBase
    {
        [DataMemberAttribute()]
        public Int64 ReferencePriceListID
        {
            get { return _ReferencePriceListID; }
            set
            {
                if (_ReferencePriceListID != value)
                {
                    _ReferencePriceListID = value;
                    RaisePropertyChanged("ReferencePriceListID");
                }
            }
        }
        private Int64 _ReferencePriceListID;

        [DataMemberAttribute()]
        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title != value)
                {
                    _Title = value;
                    RaisePropertyChanged("Title");
                }
            }
        }
        private string _Title;


        [DataMemberAttribute()]
        public Staff CreatedStaff
        {
            get { return _CreatedStaff; }
            set
            {
                if (_CreatedStaff != value)
                {
                    _CreatedStaff = value;
                    RaisePropertyChanged("CreatedStaff");
                }
            }
        }
        private Staff _CreatedStaff;

        [DataMemberAttribute()]
        public DateTime RecCreatedDate
        {
            get { return _RecCreatedDate; }
            set
            {
                if (_RecCreatedDate != value)
                {
                    _RecCreatedDate = value;
                    RaisePropertyChanged("RecCreatedDate");
                }
            }
        }
        private DateTime _RecCreatedDate;

        [DataMemberAttribute()]
        public ObservableCollection<PharmacyReferenceItemPrice> PharmacyRefItemPriceCollection
        {
            get { return _PharmacyRefItemPriceCollection; }
            set
            {
                if (_PharmacyRefItemPriceCollection != value)
                {
                    _PharmacyRefItemPriceCollection = value;
                    RaisePropertyChanged("PharmacyRefItemPriceCollection");
                }
            }
        }
        private ObservableCollection<PharmacyReferenceItemPrice> _PharmacyRefItemPriceCollection;

        [DataMemberAttribute()]
        public bool IsActive
        {
            get { return _IsActive; }
            set
            {
                if (_IsActive != value)
                {
                    _IsActive = value;
                    RaisePropertyChanged("IsActive");
                }
            }
        }
        private bool _IsActive;

        public string ConvertRefItemPriceToXml()
        {
            return ConvertRefItemPriceToXml(PharmacyRefItemPriceCollection);
        }

        public string ConvertRefItemPriceToXml(IEnumerable<PharmacyReferenceItemPrice> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<PharmacyReferenceItemPrice>");
                foreach (PharmacyReferenceItemPrice details in items)
                {
                    sb.Append("<RecInfo>");
                    sb.AppendFormat("<ReferenceItemPriceID>{0}</ReferenceItemPriceID>", details.ReferenceItemPriceID);
                    sb.AppendFormat("<ReferencePriceListID>{0}</ReferencePriceListID>", details.ReferencePriceListID);
                    sb.AppendFormat("<DrugID>{0}</DrugID>", details.Drug.DrugID);
                    sb.AppendFormat("<ContractPriceAfterVAT>{0}</ContractPriceAfterVAT>", details.ContractPriceAfterVAT);
                    sb.AppendFormat("<HIAllowedPrice>{0}</HIAllowedPrice>", details.HIAllowedPrice);
                    sb.Append("</RecInfo>");
                }
                sb.Append("</PharmacyReferenceItemPrice>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }

    }
}
