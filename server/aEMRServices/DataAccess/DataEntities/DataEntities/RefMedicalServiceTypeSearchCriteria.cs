using System;
using System.Net;
using System.Windows;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public class RefMedicalServiceTypeSearchCriteria:SearchCriteriaBase
    {
        public RefMedicalServiceTypeSearchCriteria()
        {
        }


        private int _FindBy;//--0:Code, 1: Name, 2: Descr
        public int FindBy
        {
          get { return _FindBy; }
          set 
          { 
              _FindBy = value; 
              RaisePropertyChanged("FindBy");
          }
        }

        
        public Int64 MedicalServiceGroupID
        {
            get { return _MedicalServiceGroupID; }
            set
            {
                _MedicalServiceGroupID = value;
                RaisePropertyChanged("MedicalServiceGroupID");
            }
        }
        private Int64 _MedicalServiceGroupID;



        private Int64 _V_RefMedicalServiceTypes;
        public Int64 V_RefMedicalServiceTypes
        {
            get { return _V_RefMedicalServiceTypes; }
            set
            {
                _V_RefMedicalServiceTypes = value;
                RaisePropertyChanged("V_RefMedicalServiceTypes");
            }
        }


        private string _MedicalServiceTypeCode;
        public string MedicalServiceTypeCode
        {
            get { return _MedicalServiceTypeCode; }
            set 
            { 
                _MedicalServiceTypeCode = value;
                RaisePropertyChanged("MedicalServiceTypeCode");
            }
        }


        private string _MedicalServiceTypeName;
        public string MedicalServiceTypeName
        {
            get { return _MedicalServiceTypeName; }
            set
            {
                _MedicalServiceTypeName = value;
                RaisePropertyChanged("MedicalServiceTypeName");
            }
        }

        private string _MedicalServiceTypeDescription;
        public string MedicalServiceTypeDescription
        {
            get { return _MedicalServiceTypeDescription; }
            set
            {
                _MedicalServiceTypeDescription = value;
                RaisePropertyChanged("MedicalServiceTypeDescription");
            }
        }
     
        private string _OrderBy;
        public string OrderBy
        {
            get { return _OrderBy; }
            set
            {
                _OrderBy = value;
                RaisePropertyChanged("OrderBy");
            }
        }

    }
}
