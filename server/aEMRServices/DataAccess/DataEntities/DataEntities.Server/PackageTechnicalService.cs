
using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities.MedicalInstruction
{
    public partial class PackageTechnicalService: NotifyChangedBase
    {
        #region Primitive Properties
        [DataMemberAttribute()]
        public long PackageTechnicalServiceID
        {
            get
            {
                return _PackageTechnicalServiceID;
            }
            set
            {
                if (_PackageTechnicalServiceID != value)
                {
                    _PackageTechnicalServiceID = value;
                    RaisePropertyChanged("PackageTechnicalServiceID");
                }
            }
        }
        private long _PackageTechnicalServiceID;
        [DataMemberAttribute()]
        public bool IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                _IsDeleted = value;
                RaisePropertyChanged("IsDeleted");
            }
        }
        private bool _IsDeleted;
        [DataMemberAttribute()]
        public List<PackageTechnicalServiceDetail> PackageTechnicalServiceDetailList
        {
            get
            {
                return _PackageTechnicalServiceDetailList;
            }
            set
            {
                _PackageTechnicalServiceDetailList = value;
                RaisePropertyChanged("PackageTechnicalServiceDetailList");
            }
        }
        private List<PackageTechnicalServiceDetail> _PackageTechnicalServiceDetailList;
        [DataMemberAttribute()]
        public PackageTechnicalServiceDetail PackageTechnicalServiceDetail
        {
            get
            {
                return _PackageTechnicalServiceDetail;
            }
            set
            {
                _PackageTechnicalServiceDetail = value;
                RaisePropertyChanged("PackageTechnicalServiceDetail");
            }
        }
        private PackageTechnicalServiceDetail _PackageTechnicalServiceDetail;
        private string _LogModified;
        [DataMemberAttribute()]
        public string LogModified
        {
            get
            {
                return _LogModified;
            }
            set
            {
                _LogModified = value;
                RaisePropertyChanged("LogModified");
            }
        }

        private string _Title;
        [DataMemberAttribute()]
        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                _Title = value;
                RaisePropertyChanged("Title");
            }
        }
        [DataMemberAttribute()]
        public long CreatedStaffID
        {
            get
            {
                return _CreatedStaffID;
            }
            set
            {
                if (_CreatedStaffID != value)
                {
                    _CreatedStaffID = value;
                    RaisePropertyChanged("CreatedStaffID");
                }
            }
        }
        private long _CreatedStaffID;
        [DataMemberAttribute()]
        public Staff CreatedStaff
        {
            get
            {
                return _CreatedStaff;
            }
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
            get
            {
                return _RecCreatedDate;
            }
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
        #endregion

        public static PackageTechnicalService CreatePackageTechnicalService()
        {
            PackageTechnicalService PackageTechnicalService = new PackageTechnicalService();
            PackageTechnicalService.PackageTechnicalServiceDetailList = new List<PackageTechnicalServiceDetail>();
            return PackageTechnicalService;
        }
    }
}
