using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class ResearchPartnerShip : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new ResearchPartnerShip object.

        /// <param name="partnerShipID">Initial value of the PartnerShipID property.</param>
        /// <param name="partnerShipName">Initial value of the PartnerShipName property.</param>
        /// <param name="partnerShipContactInfo">Initial value of the PartnerShipContactInfo property.</param>
        public static ResearchPartnerShip CreateResearchPartnerShip(long partnerShipID, String partnerShipName, String partnerShipContactInfo)
        {
            ResearchPartnerShip researchPartnerShip = new ResearchPartnerShip();
            researchPartnerShip.PartnerShipID = partnerShipID;
            researchPartnerShip.PartnerShipName = partnerShipName;
            researchPartnerShip.PartnerShipContactInfo = partnerShipContactInfo;
            return researchPartnerShip;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long PartnerShipID
        {
            get
            {
                return _PartnerShipID;
            }
            set
            {
                if (_PartnerShipID != value)
                {
                    OnPartnerShipIDChanging(value);
                    ////ReportPropertyChanging("PartnerShipID");
                    _PartnerShipID = value;
                    RaisePropertyChanged("PartnerShipID");
                    OnPartnerShipIDChanged();
                }
            }
        }
        private long _PartnerShipID;
        partial void OnPartnerShipIDChanging(long value);
        partial void OnPartnerShipIDChanged();





        [DataMemberAttribute()]
        public String PartnerShipName
        {
            get
            {
                return _PartnerShipName;
            }
            set
            {
                OnPartnerShipNameChanging(value);
                ////ReportPropertyChanging("PartnerShipName");
                _PartnerShipName = value;
                RaisePropertyChanged("PartnerShipName");
                OnPartnerShipNameChanged();
            }
        }
        private String _PartnerShipName;
        partial void OnPartnerShipNameChanging(String value);
        partial void OnPartnerShipNameChanged();





        [DataMemberAttribute()]
        public String PartnerShipContactInfo
        {
            get
            {
                return _PartnerShipContactInfo;
            }
            set
            {
                OnPartnerShipContactInfoChanging(value);
                ////ReportPropertyChanging("PartnerShipContactInfo");
                _PartnerShipContactInfo = value;
                RaisePropertyChanged("PartnerShipContactInfo");
                OnPartnerShipContactInfoChanged();
            }
        }
        private String _PartnerShipContactInfo;
        partial void OnPartnerShipContactInfoChanging(String value);
        partial void OnPartnerShipContactInfoChanged();





        [DataMemberAttribute()]
        public String PartnerShipNotes
        {
            get
            {
                return _PartnerShipNotes;
            }
            set
            {
                OnPartnerShipNotesChanging(value);
                ////ReportPropertyChanging("PartnerShipNotes");
                _PartnerShipNotes = value;
                RaisePropertyChanged("PartnerShipNotes");
                OnPartnerShipNotesChanged();
            }
        }
        private String _PartnerShipNotes;
        partial void OnPartnerShipNotesChanging(String value);
        partial void OnPartnerShipNotesChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_REFMEDIC_REL_HOSFM_RESEARCH", "RefMedicalServiceItems")]
        public ObservableCollection<RefMedicalServiceItem> RefMedicalServiceItems
        {
            get;
            set;
        }

        #endregion
    }

  
}
