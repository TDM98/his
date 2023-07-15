using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class TechnicalInspectionAgency : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new TechnicalInspectionAgency object.

        /// <param name="tIANumber">Initial value of the TIANumber property.</param>
        /// <param name="tecInspAgencyName">Initial value of the TecInspAgencyName property.</param>
        /// <param name="tecInspAgencyAddress">Initial value of the TecInspAgencyAddress property.</param>
        public static TechnicalInspectionAgency CreateTechnicalInspectionAgency(long tIANumber, String tecInspAgencyName, String tecInspAgencyAddress)
        {
            TechnicalInspectionAgency technicalInspectionAgency = new TechnicalInspectionAgency();
            technicalInspectionAgency.TIANumber = tIANumber;
            technicalInspectionAgency.TecInspAgencyName = tecInspAgencyName;
            technicalInspectionAgency.TecInspAgencyAddress = tecInspAgencyAddress;
            return technicalInspectionAgency;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long TIANumber
        {
            get
            {
                return _TIANumber;
            }
            set
            {
                if (_TIANumber != value)
                {
                    OnTIANumberChanging(value);
                    ////ReportPropertyChanging("TIANumber");
                    _TIANumber = value;
                    RaisePropertyChanged("TIANumber");
                    OnTIANumberChanged();
                }
            }
        }
        private long _TIANumber;
        partial void OnTIANumberChanging(long value);
        partial void OnTIANumberChanged();





        [DataMemberAttribute()]
        public String TecInspAgencyName
        {
            get
            {
                return _TecInspAgencyName;
            }
            set
            {
                OnTecInspAgencyNameChanging(value);
                ////ReportPropertyChanging("TecInspAgencyName");
                _TecInspAgencyName = value;
                RaisePropertyChanged("TecInspAgencyName");
                OnTecInspAgencyNameChanged();
            }
        }
        private String _TecInspAgencyName;
        partial void OnTecInspAgencyNameChanging(String value);
        partial void OnTecInspAgencyNameChanged();





        [DataMemberAttribute()]
        public String TecInspAgencyAddress
        {
            get
            {
                return _TecInspAgencyAddress;
            }
            set
            {
                OnTecInspAgencyAddressChanging(value);
                ////ReportPropertyChanging("TecInspAgencyAddress");
                _TecInspAgencyAddress = value;
                RaisePropertyChanged("TecInspAgencyAddress");
                OnTecInspAgencyAddressChanged();
            }
        }
        private String _TecInspAgencyAddress;
        partial void OnTecInspAgencyAddressChanging(String value);
        partial void OnTecInspAgencyAddressChanged();





        [DataMemberAttribute()]
        public String TecInspAgencyEmail
        {
            get
            {
                return _TecInspAgencyEmail;
            }
            set
            {
                OnTecInspAgencyEmailChanging(value);
                ////ReportPropertyChanging("TecInspAgencyEmail");
                _TecInspAgencyEmail = value;
                RaisePropertyChanged("TecInspAgencyEmail");
                OnTecInspAgencyEmailChanged();
            }
        }
        private String _TecInspAgencyEmail;
        partial void OnTecInspAgencyEmailChanging(String value);
        partial void OnTecInspAgencyEmailChanged();





        [DataMemberAttribute()]
        public String TecInspAgencyPhone
        {
            get
            {
                return _TecInspAgencyPhone;
            }
            set
            {
                OnTecInspAgencyPhoneChanging(value);
                ////ReportPropertyChanging("TecInspAgencyPhone");
                _TecInspAgencyPhone = value;
                RaisePropertyChanged("TecInspAgencyPhone");
                OnTecInspAgencyPhoneChanged();
            }
        }
        private String _TecInspAgencyPhone;
        partial void OnTecInspAgencyPhoneChanging(String value);
        partial void OnTecInspAgencyPhoneChanged();





        [DataMemberAttribute()]
        public String TecInspAgencyFaxNo
        {
            get
            {
                return _TecInspAgencyFaxNo;
            }
            set
            {
                OnTecInspAgencyFaxNoChanging(value);
                ////ReportPropertyChanging("TecInspAgencyFaxNo");
                _TecInspAgencyFaxNo = value;
                RaisePropertyChanged("TecInspAgencyFaxNo");
                OnTecInspAgencyFaxNoChanged();
            }
        }
        private String _TecInspAgencyFaxNo;
        partial void OnTecInspAgencyFaxNoChanging(String value);
        partial void OnTecInspAgencyFaxNoChanged();





        [DataMemberAttribute()]
        public String TecInspAgencyWebSite
        {
            get
            {
                return _TecInspAgencyWebSite;
            }
            set
            {
                OnTecInspAgencyWebSiteChanging(value);
                ////ReportPropertyChanging("TecInspAgencyWebSite");
                _TecInspAgencyWebSite = value;
                RaisePropertyChanged("TecInspAgencyWebSite");
                OnTecInspAgencyWebSiteChanged();
            }
        }
        private String _TecInspAgencyWebSite;
        partial void OnTecInspAgencyWebSiteChanging(String value);
        partial void OnTecInspAgencyWebSiteChanged();





        [DataMemberAttribute()]
        public String TecInspAgencyNotes
        {
            get
            {
                return _TecInspAgencyNotes;
            }
            set
            {
                OnTecInspAgencyNotesChanging(value);
                ////ReportPropertyChanging("TecInspAgencyNotes");
                _TecInspAgencyNotes = value;
                RaisePropertyChanged("TecInspAgencyNotes");
                OnTecInspAgencyNotesChanged();
            }
        }
        private String _TecInspAgencyNotes;
        partial void OnTecInspAgencyNotesChanging(String value);
        partial void OnTecInspAgencyNotesChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_EXAMMAIN_REL_RM21_TECHNICA", "ExamMaintenanceHistory")]
        public ObservableCollection<ExamMaintenanceHistory> ExamMaintenanceHistories
        {
            get;
            set;
        }

        #endregion
    }
}
