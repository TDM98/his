using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class IDCardQRCode
    {
        public IDCardQRCode()
            : base()
        {
        }
        public string IDNumber { get; set; }
        public string IDNumberOld { get; set; }
        public string FullName { get; set; }
        public DateTime DOB { get; set; }
        public Gender Gender { get; set; }
        public string Address { get; set; }
        public DateTime IDCreatedDate { get; set; }
        public CitiesProvince CitiesProvince { get; set; }
        public SuburbNames SuburbNames { get; set; }
        public WardNames WardNames { get; set; }
    }
}