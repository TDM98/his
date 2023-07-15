using System;
using System.Net;
using System.Windows;
using eHCMS.Services.Core.Base;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using System.Text;
using System.Collections.Generic;


namespace DataEntities
{
    public partial class DeptLocMedServices : NotifyChangedBase
    {
        [DataMemberAttribute()]
        private Int64 _DeptLocMedServiceID;
        public Int64 DeptLocMedServiceID
        {
            get { return _DeptLocMedServiceID; }
            set 
            {
                if (_DeptLocMedServiceID != value)
                {
                    OnDeptLocMedServiceIDChanging(value);
                    _DeptLocMedServiceID = value;
                    RaisePropertyChanged("DeptLocMedServiceID");
                    OnDeptLocMedServiceIDChanged();
                }
            }
        }
        partial void OnDeptLocMedServiceIDChanging(Int64 value);
        partial void OnDeptLocMedServiceIDChanged();


        [DataMemberAttribute()]
        private Int64 _MedServiceID;
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
        partial void OnMedServiceIDChanging(Int64 value);
        partial void OnMedServiceIDChanged();


        [DataMemberAttribute()]
        private Int64 _DeptLocationID;
        public Int64 DeptLocationID
        {
            get { return _DeptLocationID; }
            set 
            {
                if (_DeptLocationID != value)
                {
                    OnDeptLocationIDChanging(value);
                    _DeptLocationID = value;
                    RaisePropertyChanged("DeptLocationID");
                    OnDeptLocationIDChanged();
                }
            }
        }
        partial void OnDeptLocationIDChanging(Int64 value);
        partial void OnDeptLocationIDChanged();


        //For Insert XML
        private ObservableCollection<DeptLocMedServices> _ObjDeptLocMedServices_List;
        public ObservableCollection<DeptLocMedServices> ObjDeptLocMedServices_List
        {
            get
            {
                return _ObjDeptLocMedServices_List;
            }
            set
            {
                if (_ObjDeptLocMedServices_List != value)
                {
                    _ObjDeptLocMedServices_List = value;
                    RaisePropertyChanged("ObjDeptLocMedServices_List");
                }
            }
        }

        public string ConvertObjDeptLocMedServices_ListToXml()
        {
            return ConvertObjDeptLocMedServices_ListToXml(_ObjDeptLocMedServices_List);
        }
        public string ConvertObjDeptLocMedServices_ListToXml(IEnumerable<DeptLocMedServices> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (DeptLocMedServices details in items)
                {
                    sb.Append("<DeptLocMedServices>");
                    sb.AppendFormat("<MedServiceID>{0}</MedServiceID>", details.MedServiceID);
                    sb.AppendFormat("<DeptLocationID>{0}</DeptLocationID>", details.DeptLocationID);                    
                    sb.Append("</DeptLocMedServices>");
                }
                sb.Append("</DS>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }
        //For Insert XML

        
    }
}
