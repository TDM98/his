using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities
{
    public class BedCategorySearchCriteria: SearchCriteriaBase
    {
        public BedCategorySearchCriteria()
        {

        }
        private Int64 _DeptLocID;
        public Int64 DeptLocID
        {
            get { return _DeptLocID; }
            set
            {
                _DeptLocID = value;
                RaisePropertyChanged("DeptLocID");
            }
        }
        private Int64 _V_BedType;
        public Int64 V_BedType
        {
            get { return _V_BedType; }
            set
            {
                _V_BedType = value;
                RaisePropertyChanged("V_BedType");
            }
        }

        private string _HosBedCode;
        public string HosBedCode
        {
            get { return _HosBedCode; }
            set
            {
                _HosBedCode = value;
                RaisePropertyChanged("HosBedCode");
            }
        }
        private string _HosBedName;
        public string HosBedName
        {
            get { return _HosBedName; }
            set
            {
                _HosBedName = value;
                RaisePropertyChanged("HosBedName");
            }
        }
        private bool _IsBookBed;
        public bool IsBookBed
        {
            get { return _IsBookBed; }
            set
            {
                _IsBookBed = value;
                RaisePropertyChanged("IsBookBed");
            }
        }
    }
}
