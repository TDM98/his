using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    [DataContract]
    public partial class FolderTree : NotifyChangedBase
    {
        private string folderName;
        private string parentFolderName;
        private List<FolderTree> folderChildren;
        private string extention;
        private Nullable<DateTime> createDate;

        public FolderTree()
            : base()
        {
        }
        public static FolderTree CreateFolderTree(string folderName)
        {
            FolderTree patientPCLAppointment = new FolderTree();
            patientPCLAppointment.FolderName = folderName;
            patientPCLAppointment.FolderChildren = new List<FolderTree>();
            return patientPCLAppointment;
        }
        //public FolderTree(string folderName, string parentFolderName, List<FolderTree> folderChildren, string extention, Nullable<DateTime> createDate)
        //{
        //    this.folderName = folderName;
        //    this.parentFolderName = parentFolderName;
        //    this.folderChildren = folderChildren;
        //    this.extention = extention;
        //    this.createDate = createDate;
        //}
        [DataMember]
        public string FolderName
        {
            get
            {
                return folderName;
            }
            set
            {
                folderName = value;
                RaisePropertyChanged("FolderName");
            }
        }
        [DataMember]
        public string ParentFolderName
        {
            get
            {
                return parentFolderName;
            }
            set
            {
                parentFolderName = value;
                RaisePropertyChanged("ParentFolderName");
            }
        }
        [DataMember]
        public List<FolderTree> FolderChildren
        {
            get
            {
                return folderChildren;
            }
            set
            {
                folderChildren = value;
                RaisePropertyChanged("FolderChildren");
            }
        }
        [DataMember]
        public string Extention
        {
            get
            {
                return extention;
            }
            set
            {
                extention = value;
                RaisePropertyChanged("Extention");
            }
        }
        [DataMember]
        public Nullable<DateTime> CreateDate
        {
            get
            {
                return createDate;
            }
            set
            {
                createDate = value;
                RaisePropertyChanged("CreateDate");
            }
        }
    }
}
