using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities
{
    public class RouteOfAdministrationContactDrug : EntityBase
    {
        
       
        private long _DrugROAID;
        [DataMemberAttribute()]
        public long DrugROAID
        {
            get
            {
                return _DrugROAID;
            }
            set
            {
                _DrugROAID = value;
                RaisePropertyChanged("DrugROAID");
            }
        }
        private long _GenMedProductID;
        [DataMemberAttribute()]
        public long GenMedProductID
        {
            get
            {
                return _GenMedProductID;
            }
            set
            {
                _GenMedProductID = value;
                RaisePropertyChanged("GenMedProductID");
            }
        }
        private Lookup _RouteOfAdministration;
        [DataMemberAttribute()]
        public Lookup RouteOfAdministration
        {
            get
            {
                return _RouteOfAdministration;
            }
            set
            {
                _RouteOfAdministration = value;
                RaisePropertyChanged("RouteOfAdministration");
            }
        }
     
        private DateTime _CreatedDate;
        [DataMemberAttribute()]
        public DateTime CreatedDate
        {
            get
            {
                return _CreatedDate;
            }
            set
            {
                _CreatedDate = value;
                RaisePropertyChanged("CreatedDate");
            }
        }
        private bool _IsDelete;
        [DataMemberAttribute()]
        public bool IsDelete
        {
            get
            {
                return _IsDelete;
            }
            set
            {
                _IsDelete = value;
                RaisePropertyChanged("IsDelete");
            }
        }
    }
}
