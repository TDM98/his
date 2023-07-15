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
    public partial class PCLExamTypeLocations : NotifyChangedBase
    {
        [DataMemberAttribute()]
        private Int64 _PCLExamTypeLocID;
        public Int64 PCLExamTypeLocID
        {
            get { return _PCLExamTypeLocID; }
            set 
            {
                if (_PCLExamTypeLocID != value)
                {
                    OnPCLExamTypeLocIDChanging(value);
                    _PCLExamTypeLocID = value;
                    RaisePropertyChanged("PCLExamTypeLocID");
                    OnPCLExamTypeLocIDChanged();
                }
            }
        }
        partial void OnPCLExamTypeLocIDChanging(Int64 value);
        partial void OnPCLExamTypeLocIDChanged();


        [DataMemberAttribute()]
        private Int64 _PCLExamTypeID;
        public Int64 PCLExamTypeID
        {
            get { return _PCLExamTypeID; }
            set 
            {
                if (_PCLExamTypeID != value)
                {
                    OnPCLExamTypeIDChanging(value);
                    _PCLExamTypeID = value;
                    RaisePropertyChanged("PCLExamTypeID");
                    OnPCLExamTypeIDChanged();
                }
            }
        }
        partial void OnPCLExamTypeIDChanging(Int64 value);
        partial void OnPCLExamTypeIDChanged();


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
        private ObservableCollection<PCLExamType> _ObjPCLExamType_List;
        public ObservableCollection<PCLExamType> ObjPCLExamType_List
        {
            get
            {
                return _ObjPCLExamType_List;
            }
            set
            {
                if (_ObjPCLExamType_List != value)
                {
                    _ObjPCLExamType_List = value;
                    RaisePropertyChanged("ObjPCLExamType_List");
                }
            }
        }

        //public string ConvertObjPCLExamType_ListToXml()
        //{
        //    return ConvertObjPCLExamType_ListToXml(_ObjPCLExamType_List);
        //}
        //public string ConvertObjPCLExamType_ListToXml(IEnumerable<PCLExamType> items)
        //{
        //    if (items != null)
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        sb.Append("<PCLExamTypeLocations>");
        //        foreach (PCLExamType details in items)
        //        {
        //            sb.Append("<RowInfo>");
        //            sb.AppendFormat("<PCLExamTypeID>{0}</PCLExamTypeID>", details.PCLExamTypeID);
        //            sb.AppendFormat("<DeptLocationID>{0}</DeptLocationID>", DeptLocationID);
        //            sb.Append("</RowInfo>");
        //        }
        //        sb.Append("</PCLExamTypeLocations>");
        //        return sb.ToString();
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
        //For Insert XML

        
    }
}
