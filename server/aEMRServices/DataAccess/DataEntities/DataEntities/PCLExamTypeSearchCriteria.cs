using System;
using System.Net;
using System.Windows;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public class PCLExamTypeSearchCriteria : SearchCriteriaBase
    {
        public PCLExamTypeSearchCriteria()
        { 
        }


        private Int64 _PCLExamTypePriceListID;
        public Int64 PCLExamTypePriceListID
        {
            get { return _PCLExamTypePriceListID; }
            set
            {
                if (_PCLExamTypePriceListID != value)
                {
                    _PCLExamTypePriceListID = value;
                    RaisePropertyChanged("PCLExamTypePriceListID");
                }
            }
        }


        //private long? _pclCategory;
        //public long? PclCategory
        //{
        //    get
        //    {
        //        return _pclCategory;
        //    }
        //    set
        //    {
        //        _pclCategory = value;
        //        RaisePropertyChanged("PclCategory");
        //    }
        //}

        //private long? _pclGroupID;
        //public long? PCLGroupID
        //{
        //    get 
        //    {
        //        return _pclGroupID; 
        //    }
        //    set 
        //    {
        //        _pclGroupID = value;
        //        RaisePropertyChanged("PCLGroupID");
        //    }
        //}


        private string _pclExamTypeName;
        public string PCLExamTypeName
        {
            get 
            { 
                return _pclExamTypeName; 
            }
            set 
            { 
                _pclExamTypeName = value;
                RaisePropertyChanged("PCLExamTypeName");
            }
        }

        //private long? _V_PCLCategory;
        //public long? V_PCLCategory
        //{
        //    get
        //    {
        //        return _V_PCLCategory;
        //    }
        //    set
        //    {
        //        _V_PCLCategory = value;
        //        RaisePropertyChanged("V_PCLCategory");
        //    }
        //}



        private Int64 _V_PCLMainCategory;
        public Int64 V_PCLMainCategory
        {
            get { return _V_PCLMainCategory; }
            set
            {
                if (_V_PCLMainCategory != value)
                {
                    _V_PCLMainCategory = value;
                    RaisePropertyChanged("V_PCLMainCategory");
                }
            }
        }

        private Int64 _PCLExamTypeSubCategoryID;
        public Int64 PCLExamTypeSubCategoryID
        {
            get { return _PCLExamTypeSubCategoryID; }
            set
            {
                if (_PCLExamTypeSubCategoryID != value)
                {
                    _PCLExamTypeSubCategoryID = value;
                    RaisePropertyChanged("PCLExamTypeSubCategoryID");
                }
            }
        }

        private Boolean _IsNotInPCLItems;
        public Boolean IsNotInPCLItems
        {
            get { return _IsNotInPCLItems;}
            set
            {
                if(_IsNotInPCLItems!=value)
                {
                    _IsNotInPCLItems = value;
                    RaisePropertyChanged("IsNotInPCLItems");
                }
            }
        }

        private Boolean _IsNotInPCLExamTypeLocations;
        public Boolean IsNotInPCLExamTypeLocations
        {
            get { return _IsNotInPCLExamTypeLocations; }
            set
            {
                if (_IsNotInPCLExamTypeLocations != value)
                {
                    _IsNotInPCLExamTypeLocations = value;
                    RaisePropertyChanged("IsNotInPCLExamTypeLocations");
                }
            }
        }


        private Nullable<Int64> _PCLFormID;
        public Nullable<Int64> PCLFormID
        {
            get { return _PCLFormID; }
            set
            {
                if (_PCLFormID != value)
                {
                    _PCLFormID = value;
                    RaisePropertyChanged("PCLFormID");
                }
            }
        }


        private string _orderBy;
        public string OrderBy
        {
            get { return _orderBy; }
            set { _orderBy = value;
            RaisePropertyChanged("OrderBy");
            }
        }

        private bool? _IsExternalExam;
        public bool? IsExternalExam
        {
            get { return _IsExternalExam; }
            set
            {
                _IsExternalExam = value;
                RaisePropertyChanged("IsExternalExam");
            }
        }


    }
}
