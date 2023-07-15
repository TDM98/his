using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class ServiceAppointment : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new ServiceAppointment object.

        /// <param name="id">Initial value of the ID property.</param>
        public static ServiceAppointment CreateServiceAppointment(Int32 id)
        {
            ServiceAppointment serviceAppointment = new ServiceAppointment();
            serviceAppointment.ID = id;
            return serviceAppointment;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public Int32 ID
        {
            get
            {
                return _ID;
            }
            set
            {
                if (_ID != value)
                {
                    OnIDChanging(value);
                    ////ReportPropertyChanging("ID");
                    _ID = value;
                    RaisePropertyChanged("ID");
                    OnIDChanged();
                }
            }
        }
        private Int32 _ID;
        partial void OnIDChanging(Int32 value);
        partial void OnIDChanged();





        [DataMemberAttribute()]
        public Nullable<Int32> ApointmentID
        {
            get
            {
                return _ApointmentID;
            }
            set
            {
                OnApointmentIDChanging(value);
                ////ReportPropertyChanging("ApointmentID");
                _ApointmentID = value;
                RaisePropertyChanged("ApointmentID");
                OnApointmentIDChanged();
            }
        }
        private Nullable<Int32> _ApointmentID;
        partial void OnApointmentIDChanging(Nullable<Int32> value);
        partial void OnApointmentIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> MedServiceID
        {
            get
            {
                return _MedServiceID;
            }
            set
            {
                OnMedServiceIDChanging(value);
                ////ReportPropertyChanging("MedServiceID");
                _MedServiceID = value;
                RaisePropertyChanged("MedServiceID");
                OnMedServiceIDChanged();
            }
        }
        private Nullable<long> _MedServiceID;
        partial void OnMedServiceIDChanging(Nullable<long> value);
        partial void OnMedServiceIDChanged();

        #endregion

        #region Navigation Properties



        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_SERVICEA_REL_PTAPP_REFMEDIC", "RefMedicalServiceItems")]
        public RefMedicalServiceItem RefMedicalServiceItem
        {
            get;
            set;

        }

        #endregion
    }
}
