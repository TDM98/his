using System;
using System.Net;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public class MonthDisplay : NotifyChangedBase
    {
        private string _T1;
        public string T1
        {
            get
            {
                return _T1;
            }
            set
            {
                if (_T1 != value)
                {
                    _T1 = value;
                    RaisePropertyChanged("T1");
                }
            }
        }
        private string _T2;
        public string T2
        {
            get
            {
                return _T2;
            }
            set
            {
                if (_T2 != value)
                {
                    _T2 = value;
                    RaisePropertyChanged("T2");
                }
            }
        }
        private string _T3;
        public string T3
        {
            get
            {
                return _T3;
            }
            set
            {
                if (_T3 != value)
                {
                    _T3 = value;
                    RaisePropertyChanged("T3");
                }
            }
        }
        private string _T4;
        public string T4
        {
            get
            {
                return _T4;
            }
            set
            {
                if (_T4 != value)
                {
                    _T4 = value;
                    RaisePropertyChanged("T4");
                }
            }
        }

        private string _CurrentT;
        public string CurrentT
        {
            get
            {
                return _CurrentT;
            }
            set
            {
                if (_CurrentT != value)
                {
                    _CurrentT = value;
                    RaisePropertyChanged("CurrentT");
                }
            }
        }
    }
}
