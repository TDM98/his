using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
namespace DataEntities
{
    public partial class ApptService : NotifyChangedBase
    {
        [DataMemberAttribute()]        
        public Int64 ApptServiceID
        {
            get { return _ApptServiceID; }
            set 
            {
                if (_ApptServiceID != value)
                {
                    OnApptServiceIDChanging(value);
                    _ApptServiceID = value;
                    RaisePropertyChanged("ApptServiceID");
                    OnApptServiceIDChanged();
                }
            }
        }
        private Int64 _ApptServiceID;
        partial void OnApptServiceIDChanging(Int64 value);
        partial void OnApptServiceIDChanged();
        
        [DataMemberAttribute()]        
        public Int64 MedServiceID
        {
            get { return _MedServiceID; }
            set 
            {
                if (_MedServiceID != value)
                {
                    OnMedServiceIDChanging(value);
                    _MedServiceID = value;
                    RaisePropertyChanged("MedServiceID");
                    OnMedServiceIDChanged();
                }
            }
        }
        private Int64 _MedServiceID;
        partial void OnMedServiceIDChanging(Int64 value);
        partial void OnMedServiceIDChanged();

        [DataMemberAttribute()]                
        public Int64 V_AppointmentType
        {
            get { return _V_AppointmentType; }
            set 
            {
                if (_V_AppointmentType != value)
                {
                    OnV_AppointmentTypeChanging(value);
                    _V_AppointmentType = value;
                    RaisePropertyChanged("V_AppointmentType");
                    OnV_AppointmentTypeChanged();
                }
            }
        }
        private Int64 _V_AppointmentType;
        partial void OnV_AppointmentTypeChanging(Int64 value);
        partial void OnV_AppointmentTypeChanged();

        //Navigate
        [DataMemberAttribute()]
        public Lookup ObjV_AppointmentType
        {
            get { return _ObjV_AppointmentType; }
            set 
            {
                if (_ObjV_AppointmentType != value)
                {
                    OnObjV_AppointmentTypeChanging(value);
                    _ObjV_AppointmentType = value;
                    RaisePropertyChanged("ObjV_AppointmentType");
                    OnObjV_AppointmentTypeChanged();
                }
            }
        }
        private Lookup _ObjV_AppointmentType;
        partial void OnObjV_AppointmentTypeChanging(Lookup value);
        partial void OnObjV_AppointmentTypeChanged();
       
    }
}
