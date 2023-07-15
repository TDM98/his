using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class HIQRCode
    {
        public HIQRCode()
            : base()
        {
        }
        public string FullName { get; set; }
        public string HICardNo { get; set; }
        public Gender Gender { get; set; }
        public string Address { get; set; }
        public DateTime DOB { get; set; }
        public DateTime ValidDateFrom { get; set; }
        public DateTime ValidDateTo { get; set; }
        public string ProvinceHICode { get; set; }
        public string RegistrationCode { get; set; }
        public CitiesProvince CitiesProvince { get; set; }
    }
}
