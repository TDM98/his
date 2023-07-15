using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class RefDocument : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new RefDocument object.

        /// <param name="refDocID">Initial value of the RefDocID property.</param>
        /// <param name="v_RefDocType">Initial value of the V_RefDocType property.</param>
        /// <param name="refDocName">Initial value of the RefDocName property.</param>
        /// <param name="refFilePathLocation">Initial value of the RefFilePathLocation property.</param>
        public static RefDocument CreateRefDocument(Int64 refDocID, Int64 v_RefDocType, String refDocName, String refFilePathLocation)
        {
            RefDocument refDocument = new RefDocument();
            refDocument.RefDocID = refDocID;
            refDocument.V_RefDocType = v_RefDocType;
            refDocument.RefDocName = refDocName;
            refDocument.RefFilePathLocation = refFilePathLocation;
            return refDocument;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public Int64 RefDocID
        {
            get
            {
                return _RefDocID;
            }
            set
            {
                if (_RefDocID != value)
                {
                    OnRefDocIDChanging(value);
                    ////ReportPropertyChanging("RefDocID");
                    _RefDocID = value;
                    RaisePropertyChanged("RefDocID");
                    OnRefDocIDChanged();
                }
            }
        }
        private Int64 _RefDocID;
        partial void OnRefDocIDChanging(Int64 value);
        partial void OnRefDocIDChanged();





        [DataMemberAttribute()]
        public Int64 V_RefDocType
        {
            get
            {
                return _V_RefDocType;
            }
            set
            {
                OnV_RefDocTypeChanging(value);
                ////ReportPropertyChanging("V_RefDocType");
                _V_RefDocType = value;
                RaisePropertyChanged("V_RefDocType");
                OnV_RefDocTypeChanged();
            }
        }
        private Int64 _V_RefDocType;
        partial void OnV_RefDocTypeChanging(Int64 value);
        partial void OnV_RefDocTypeChanged();





        [DataMemberAttribute()]
        public String RefDocName
        {
            get
            {
                return _RefDocName;
            }
            set
            {
                OnRefDocNameChanging(value);
                ////ReportPropertyChanging("RefDocName");
                _RefDocName = value;
                RaisePropertyChanged("RefDocName");
                OnRefDocNameChanged();
            }
        }
        private String _RefDocName;
        partial void OnRefDocNameChanging(String value);
        partial void OnRefDocNameChanged();





        [DataMemberAttribute()]
        public String RefFilePathLocation
        {
            get
            {
                return _RefFilePathLocation;
            }
            set
            {
                OnRefFilePathLocationChanging(value);
                ////ReportPropertyChanging("RefFilePathLocation");
                _RefFilePathLocation = value;
                RaisePropertyChanged("RefFilePathLocation");
                OnRefFilePathLocationChanged();
            }
        }
        private String _RefFilePathLocation;
        partial void OnRefFilePathLocationChanging(String value);
        partial void OnRefFilePathLocationChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_CONTRACT_REL_RM15_REFDOCUM", "ContractDetails")]
        public ObservableCollection<ContractDetail> ContractDetails
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_CONTRACT_REL_RM12_REFDOCUM", "Contracts")]
        public ObservableCollection<Contract> Contracts
        {
            get;
            set;
        }

        #endregion
    }
}
