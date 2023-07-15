using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using System.Configuration;
/*
 * 20180606 #001 CMN: Added enum for LabSoft API
 */
namespace eHCMS.Configurations
{
    [DataContract]
    public class PatientElement : ConfigurationElement
    {
        [ConfigurationProperty("connectionStringName")]
        public string ConnectionStringName
        {
            get { return (string)base["connectionStringName"]; }
            set { base["connectionStringName"] = value; }
        }

        public string ConnectionString
        {

            get
            {
                string connStringName = (string.IsNullOrEmpty(this.ConnectionStringName) ?
                   Globals.Settings.DefaultConnectionStringName :
                   this.ConnectionStringName);
                return ConfigurationManager.ConnectionStrings[connStringName].ConnectionString;
            }
        }


        [ConfigurationProperty("providerType", DefaultValue = "eHCMS.DAL.SqlPatientProvider")]
        public string ProviderType
        {
            get { return (string)base["providerType"]; }
            set { base["providerType"] = value; }
        }
        [ConfigurationProperty("assembly", DefaultValue = "SqlPatientProvider")]
        public string Assembly
        {
            get { return (string)base["assembly"]; }
            set { base["assembly"] = value; }
        }
    }

    public class ConfigurationManagerElement : ConfigurationElement
    {
        [ConfigurationProperty("connectionStringName")]
        public string ConnectionStringName
        {
            get { return (string)base["connectionStringName"]; }
            set { base["connectionStringName"] = value; }
        }

        public string ConnectionString
        {
            get
            {
                string connStringName = (string.IsNullOrEmpty(this.ConnectionStringName) ?
                   Globals.Settings.DefaultConnectionStringName :
                   this.ConnectionStringName);
                return ConfigurationManager.ConnectionStrings[connStringName].ConnectionString;

            }
        }


        [ConfigurationProperty("providerType", DefaultValue = "eHCMS.DAL.SqlConfigurationManagerProviders")]
        public string ProviderType
        {
            get { return (string)base["providerType"]; }
            set { base["providerType"] = value; }
        }
        
        [ConfigurationProperty("assembly", DefaultValue = "SqlConfigurationManagerProviders")]
        public string Assembly
        {
            get { return (string)base["assembly"]; }
            set { base["assembly"] = value; }
        }

        [ConfigurationProperty("bedlocation", IsRequired = true)]
        public BedLocationElement bedlocation
        {
            get { return (BedLocationElement)base["bedlocation"]; }
        }
        [ConfigurationProperty("userAccount", IsRequired = true)]
        public UserAccountElement userAccount
        {
            get { return (UserAccountElement)base["userAccount"]; }
        }
    }

    public class ClinicManagementElement:ConfigurationElement
    {
        [ConfigurationProperty("connectionStringName")]
        public string ConnectionStringName
        {
            get { return (string)base["connectionStringName"]; }
            set { base["connectionStringName"] = value; }
        }
        public string ConnectionString
        {
            get 
            {
                string connStringName = string.IsNullOrEmpty(this.ConnectionStringName)?
                                Globals.Settings.DefaultConnectionStringName:
                                this.ConnectionStringName;
                return ConfigurationManager.ConnectionStrings[connStringName].ConnectionString;
            }
        }

        [ConfigurationProperty("providerType", DefaultValue = "eHCMS.DAL.SqlClinicManagementProvider")]
        public string ProviderType
        {
            get { return (string)base["providerType"]; }
            set { base["providerType"] = value; }
        }

        [ConfigurationProperty("assembly", DefaultValue = "SqlClinicManagementProvider")]
        public string Assembly
        {
            get { return (string)base["assembly"]; }
            set { base["assembly"] = value; }
        }

        //[ConfigurationProperty("bedlocation", IsRequired = true)]
        //public BedLocationElement bedlocation
        //{
        //    get { return (BedLocationElement)base["bedlocation"]; }
        //}
    }

    public class AppointmentElement : ConfigurationElement
    {
        [ConfigurationProperty("connectionStringName")]
        public string ConnectionStringName
        {
            get { return (string)base["connectionStringName"]; }
            set { base["connectionStringName"] = value; }
        }

        public string ConnectionString
        {

            get
            {
                string connStringName = (string.IsNullOrEmpty(this.ConnectionStringName) ?
                   Globals.Settings.DefaultConnectionStringName :
                   this.ConnectionStringName);
                return ConfigurationManager.ConnectionStrings[connStringName].ConnectionString;
            }
        }

        
        [ConfigurationProperty("providerType", DefaultValue = "eHCMS.DAL.SqlAppointmentProvider")]
        public string ProviderType
        {
            get { return (string)base["providerType"]; }
            set { base["providerType"] = value; }
        }
        [ConfigurationProperty("assembly", DefaultValue = "SqlAppointmentProvider")]
        public string Assembly
        {
            get { return (string)base["assembly"]; }
            set { base["assembly"] = value; }
        }
    }

    public class UserElement : ConfigurationElement
    {
        [ConfigurationProperty("connectionStringName")]
        public string ConnectionStringName
        {
            get { return (string)base["connectionStringName"]; }
            set { base["connectionStringName"] = value; }
        }

        public string ConnectionString
        {

            get
            {
                string connStringName = (string.IsNullOrEmpty(this.ConnectionStringName) ?
                   Globals.Settings.DefaultConnectionStringName :
                   this.ConnectionStringName);
                return ConfigurationManager.ConnectionStrings[connStringName].ConnectionString;
            }
        }


        [ConfigurationProperty("providerType", DefaultValue = "eHCMS.DAL.SqlUserProvider")]
        public string ProviderType
        {
            get { return (string)base["providerType"]; }
            set { base["providerType"] = value; }
        }
        [ConfigurationProperty("assembly", DefaultValue = "SqlUserProvider")]
        public string Assembly
        {
            get { return (string)base["assembly"]; }
            set { base["assembly"] = value; }
        }
    }

    public class PharmacyElement : ConfigurationElement
    {
        [ConfigurationProperty("connectionStringName")]
        public string ConnectionStringName
        {
            get { return (string)base["connectionStringName"]; }
            set { base["connectionStringName"] = value; }
        }

        public string ConnectionString
        {

            get
            {
                string connStringName = (string.IsNullOrEmpty(this.ConnectionStringName) ?
                   Globals.Settings.DefaultConnectionStringName :
                   this.ConnectionStringName);
                return ConfigurationManager.ConnectionStrings[connStringName].ConnectionString;
            }
        }


        [ConfigurationProperty("providerType", DefaultValue = "eHCMS.DAL.SqlRefGenericDrugDetailsProvider")]
        public string ProviderType
        {
            get { return (string)base["providerType"]; }
            set { base["providerType"] = value; }
        }
        [ConfigurationProperty("assembly", DefaultValue = "SqlRefGenericDrugDetailsProvider")]
        public string Assembly
        {
            get { return (string)base["assembly"]; }
            set { base["assembly"] = value; }
        }
    }

    public class DispMedRscrElement : ConfigurationElement
    {
        [ConfigurationProperty("connectionStringName")]
        public string ConnectionStringName
        {
            get { return (string)base["connectionStringName"]; }
            set { base["connectionStringName"] = value; }
        }

        public string ConnectionString
        {

            get
            {
                string connStringName = (string.IsNullOrEmpty(this.ConnectionStringName) ?
                   Globals.Settings.DefaultConnectionStringName :
                   this.ConnectionStringName);
                return ConfigurationManager.ConnectionStrings[connStringName].ConnectionString;
            }
        }


        [ConfigurationProperty("providerType", DefaultValue = "eHCMS.DAL.SqlDispMedRscrProviders")]
        public string ProviderType
        {
            get { return (string)base["providerType"]; }
            set { base["providerType"] = value; }
        }
        [ConfigurationProperty("assembly", DefaultValue = "SqlDispMedRscrProviders")]
        public string Assembly
        {
            get { return (string)base["assembly"]; }
            set { base["assembly"] = value; }
        }
    }
    
    public class ResourceManageElement : ConfigurationElement
    {
        [ConfigurationProperty("connectionStringName")]
        public string ConnectionStringName
        {
            get { return (string)base["connectionStringName"]; }
            set { base["connectionStringName"] = value; }
        }

        public string ConnectionString
        {

            get
            {
                string connStringName = (string.IsNullOrEmpty(this.ConnectionStringName) ?
                   Globals.Settings.DefaultConnectionStringName :
                   this.ConnectionStringName);
                return ConfigurationManager.ConnectionStrings[connStringName].ConnectionString;
            }
        }


        [ConfigurationProperty("providerType", DefaultValue = "eHCMS.DAL.SqlResourcesManagement")]
        public string ProviderType
        {
            get { return (string)base["providerType"]; }
            set { base["providerType"] = value; }
        }
        [ConfigurationProperty("assembly", DefaultValue = "SqlResourcesManagement")]
        public string Assembly
        {
            get { return (string)base["assembly"]; }
            set { base["assembly"] = value; }
        }
    }
  
    public class ConsultationElement : ConfigurationElement
    {
        [ConfigurationProperty("connectionStringName")]
        public string ConnectionStringName
        {
            get { return (string)base["connectionStringName"]; }
            set { base["connectionStringName"] = value; }
        }

        public string ConnectionString
        {
            get
            {
                string connStringName = (string.IsNullOrEmpty(this.ConnectionStringName) ?
                   Globals.Settings.DefaultConnectionStringName :
                   this.ConnectionStringName);
                return ConfigurationManager.ConnectionStrings[connStringName].ConnectionString;
            }
        }

        [ConfigurationProperty("providerType", DefaultValue = "eHCMS.DAL.SqlConsultationProvider")]
        public string ProviderType
        {
            get { return (string)base["providerType"]; }
            set { base["providerType"] = value; }
        }

        [ConfigurationProperty("assembly", DefaultValue = "SqlConsultationProvider")]
        public string Assembly
        {
            get { return (string)base["assembly"]; }
            set { base["assembly"] = value; }
        }

        [ConfigurationProperty("commonrecords", IsRequired = true)]
        public CommonRecordsElement CommonRecords
        {
            get { return (CommonRecordsElement)base["commonrecords"]; }
        }

        [ConfigurationProperty("summary", IsRequired = true)]
        public SummaryElement Summary
        {
            get { return (SummaryElement)base["summary"]; }
        }

        [ConfigurationProperty("epmrs", IsRequired = true)]
        public ePMRsElement ePMRs
        {
            get { return (ePMRsElement)base["epmrs"]; }
        }

        [ConfigurationProperty("eprescription", IsRequired = true)]
        public ePrescriptionElement ePrescription
        {
            get { return (ePrescriptionElement)base["eprescription"]; }
        }

        [ConfigurationProperty("commonutils", IsRequired = true)]
        public CommonUtilsElement CommonUtils
        {
            get { return (CommonUtilsElement)base["commonutils"]; }
        }

        [ConfigurationProperty("pclsimport", IsRequired = true)]
        public PCLsImportElement PCLsImport
        {
            get { return (PCLsImportElement)base["pclsimport"]; }
        }

        [ConfigurationProperty("pcls", IsRequired = true)]
        public PCLsElement PCLs
        {
            get { return (PCLsElement)base["pcls"]; }
        }

    }
    /// <summary>
    /// This element will be nested inside ConsultationElement
    /// </summary>
    public class CommonRecordsElement : ConfigurationElement
    {
        [ConfigurationProperty("providerType", DefaultValue = "eHCMS.DAL.SqlCommonRecordsProvider")]
        public string ProviderType
        {
            get { return (string)base["providerType"]; }
            set { base["providerType"] = value; }
        }
    }

    /// <summary>
    /// This element will be nested inside ConsultationElement
    /// </summary>
    public class SummaryElement : ConfigurationElement
    {
        [ConfigurationProperty("providerType", DefaultValue = "eHCMS.DAL.SqlSummaryProvider")]
        public string ProviderType
        {
            get { return (string)base["providerType"]; }
            set { base["providerType"] = value; }
        }
    }

    /// <summary>
    /// This element will be nested inside ConsultationElement
    /// </summary>
    public class ePMRsElement : ConfigurationElement
    {
        [ConfigurationProperty("providerType", DefaultValue = "eHCMS.DAL.SqlePMRsProvider")]
        public string ProviderType
        {
            get { return (string)base["providerType"]; }
            set { base["providerType"] = value; }
        }
    }

    /// <summary>
    /// This element will be nested inside ConsultationElement
    /// </summary>
    public class ePrescriptionElement : ConfigurationElement
    {
        [ConfigurationProperty("providerType", DefaultValue = "eHCMS.DAL.SqlePrescriptionsProvider")]
        public string ProviderType
        {
            get { return (string)base["providerType"]; }
            set { base["providerType"] = value; }
        }
    }

    /// <summary>
    /// This element will be nested inside ConsultationElement
    /// </summary>
    public class CommonUtilsElement : ConfigurationElement
    {
        [ConfigurationProperty("providerType", DefaultValue = "eHCMS.DAL.SqlCommonUtilsProvider")]
        public string ProviderType
        {
            get { return (string)base["providerType"]; }
            set { base["providerType"] = value; }
        }
    }

    /// <summary>
    /// This element will be nested inside ConsultationElement
    /// </summary>
    public class PCLsImportElement : ConfigurationElement
    {
        [ConfigurationProperty("providerType", DefaultValue = "eHCMS.DAL.SqlPCLsImportProvider")]
        public string ProviderType
        {
            get { return (string)base["providerType"]; }
            set { base["providerType"] = value; }
        }
    }

    public class PCLsElement : ConfigurationElement
    {
        [ConfigurationProperty("providerType", DefaultValue = "eHCMS.DAL.SqlPCLsProvider")]
        public string ProviderType
        {
            get { return (string)base["providerType"]; }
            set { base["providerType"] = value; }
        }
    }

    public class CommonElement : ConfigurationElement
    {
        [ConfigurationProperty("connectionStringName")]
        public string ConnectionStringName
        {
            get { return (string)base["connectionStringName"]; }
            set { base["connectionStringName"] = value; }
        }

        public string ConnectionString
        {

            get
            {
                string connStringName = (string.IsNullOrEmpty(this.ConnectionStringName) ?
                   Globals.Settings.DefaultConnectionStringName :
                   this.ConnectionStringName);
                return ConfigurationManager.ConnectionStrings[connStringName].ConnectionString;
            }
        }

        [ConfigurationProperty("assembly", DefaultValue = "SqlCommonProviders")]
        public string Assembly
        {
            get { return (string)base["assembly"]; }
            set { base["assembly"] = value; }
        }

        [ConfigurationProperty("lookup", IsRequired = true)]
        public LookupElement Lookup
        {
            get { return (LookupElement)base["lookup"]; }
        }
        [ConfigurationProperty("country", IsRequired = true)]
        public CountryElement Country
        {
            get { return (CountryElement)base["country"]; }
        }

        [ConfigurationProperty("staff", IsRequired = true)]
        public StaffElement Staff
        {
            get { return (StaffElement)base["staff"]; }
        }

        [ConfigurationProperty("payment", IsRequired = true)]
        public PaymentElement Payment
        {
            get { return (PaymentElement)base["payment"]; }
        }

        [ConfigurationProperty("appicationaonfigs", IsRequired = true)]
        public AppConfigElement AppicationConfig
        {
            get { return (AppConfigElement)base["appicationaonfigs"]; }
        }

        [ConfigurationProperty("PatientRg", IsRequired = true)]
        public PtRegistrationElement PatientRg
        {
            get { return (PtRegistrationElement)base["PatientRg"]; }
        }
        //▼====: #001
        [ConfigurationProperty("Common", IsRequired = true)]
        public pCommonElement Common
        {
            get { return (pCommonElement)base["Common"]; }
        }
        //▲====: #001
    }

    public class LookupElement : ConfigurationElement
    {
        [ConfigurationProperty("providerType", DefaultValue = "eHCMS.DAL.SqlLookupProvider")]
        public string ProviderType
        {
            get { return (string)base["providerType"]; }
            set { base["providerType"] = value; }
        }
    }

    public class CountryElement : ConfigurationElement
    {
        [ConfigurationProperty("providerType", DefaultValue = "eHCMS.DAL.SqlCountryProvider")]
        public string ProviderType
        {
            get { return (string)base["providerType"]; }
            set { base["providerType"] = value; }
        }
    }

    public class StaffElement : ConfigurationElement
    {
        [ConfigurationProperty("providerType", DefaultValue = "eHCMS.DAL.SqlStaffProvider")]
        public string ProviderType
        {
            get { return (string)base["providerType"]; }
            set { base["providerType"] = value; }
        }
    }

    /// <summary>
    /// This element will be nested inside CommonElement
    /// </summary>
    public class PaymentElement : ConfigurationElement
    {
        [ConfigurationProperty("providerType", DefaultValue = "eHCMS.DAL.SqlPaymentProvider")]
        public string ProviderType
        {
            get { return (string)base["providerType"]; }
            set { base["providerType"] = value; }
        }
    }

    

    /// <summary>
    /// This element will be nested inside CommonElement
    /// </summary>
    /// 
    public class PtRegistrationElement : ConfigurationElement
    {
        [ConfigurationProperty("providerType", DefaultValue = "eHCMS.DAL.SqlPtRegistrationProvider")]
        public string ProviderType
        {
            get { return (string)base["providerType"]; }
            set { base["providerType"] = value; }
        }
    }
    public class BedLocationElement : ConfigurationElement
    {
        [ConfigurationProperty("providerType", DefaultValue = "eHCMS.DAL.SqlBedLocations")]
        public string ProviderType
        {
            get { return (string)base["providerType"]; }
            set { base["providerType"] = value; }
        }
    }
    public class UserAccountElement : ConfigurationElement
    {
        [ConfigurationProperty("providerType", DefaultValue = "eHCMS.DAL.SqlUserAccount")]
        public string ProviderType
        {
            get { return (string)base["providerType"]; }
            set { base["providerType"] = value; }
        }
    }

    public class TransactionElement : ConfigurationElement
    {
        [ConfigurationProperty("connectionStringName")]
        public string ConnectionStringName
        {
            get { return (string)base["connectionStringName"]; }
            set { base["connectionStringName"] = value; }
        }

        public string ConnectionString
        {

            get
            {
                string connStringName = (string.IsNullOrEmpty(this.ConnectionStringName) ?
                   Globals.Settings.DefaultConnectionStringName :
                   this.ConnectionStringName);
                return ConfigurationManager.ConnectionStrings[connStringName].ConnectionString;
            }
        }


        [ConfigurationProperty("providerType", DefaultValue = "eHCMS.DAL.SqlTransactionProvider")]
        public string ProviderType
        {
            get { return (string)base["providerType"]; }
            set { base["providerType"] = value; }
        }
        [ConfigurationProperty("assembly", DefaultValue = "SqlTransactionProvider")]
        public string Assembly
        {
            get { return (string)base["assembly"]; }
            set { base["assembly"] = value; }
        }
    }

    public class AppConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("providerType", DefaultValue = "eHCMS.DAL.SqlAppConfigsProvider")]
        public string ProviderType
        {
            get { return (string)base["providerType"]; }
            set { base["providerType"] = value; }
        }
    }
    //▼====: #001
    public class pCommonElement : ConfigurationElement
    {
        [ConfigurationProperty("providerType", DefaultValue = "eHCMS.DAL.SqlCommonProvider")]
        public string ProviderType
        {
            get { return (string)base["providerType"]; }
            set { base["providerType"] = value; }
        }
    }
    //▲====: #001
    public class eHCMSSection : ConfigurationSection
    {
        [ConfigurationProperty("defaultConnectionStringName", DefaultValue = "ConnString")]
        public string DefaultConnectionStringName
        {
            get { return (string)base["defaultConnectionStringName"]; }
            set { base["defaultConnectionStringName"] = value; }
        }

        [ConfigurationProperty("ConfigurationManager", IsRequired = true)]
        public ConfigurationManagerElement ConfigurationManager
        {
            get { return (ConfigurationManagerElement)base["ConfigurationManager"]; }
        }

        [ConfigurationProperty("ClinicManagement", IsRequired = true)]
        public ClinicManagementElement ClinicManagement
        {
            get { return (ClinicManagementElement)base["ClinicManagement"]; }
        }

        [ConfigurationProperty("patients", IsRequired = true)]
        public PatientElement Patients
        {
            get { return (PatientElement)base["patients"]; }
        }

        [ConfigurationProperty("appointments", IsRequired = true)]
        public AppointmentElement Appointments
        {
            get { return (AppointmentElement)base["appointments"]; }
        }

        [ConfigurationProperty("pharmacies", IsRequired = true)]
        public PharmacyElement Pharmacies
        {
            get { return (PharmacyElement)base["pharmacies"]; }
        }

        [ConfigurationProperty("DispMedRscr", IsRequired = true)]
        public DispMedRscrElement DispMedRscr
        {
            get { return (DispMedRscrElement)base["DispMedRscr"]; }
        }

        [ConfigurationProperty("resourcesManage", IsRequired = true)]
        public ResourceManageElement resourcesManage
        {
            get { return (ResourceManageElement)base["resourcesManage"]; }
        }

        [ConfigurationProperty("consultations", IsRequired = true)]
        public ConsultationElement Consultations
        {
            get { return (ConsultationElement)base["consultations"]; }
        }

        [ConfigurationProperty("common", IsRequired = true)]
        public CommonElement Common
        {
            get { return (CommonElement)base["common"]; }
        }

        [ConfigurationProperty("users", IsRequired = true)]
        public UserElement Users
        {
            get { return (UserElement)base["users"]; }
        }

        [ConfigurationProperty("transactions", IsRequired = true)]
        public TransactionElement Transactions
        {
            get { return (TransactionElement)base["transactions"]; }
        }

    }
    public static class Globals
    {
        public readonly static eHCMSSection Settings = (eHCMSSection)ConfigurationManager.GetSection("ehcms");
        //public readonly static AxServerConfigSection AxServerSettings = (AxServerConfigSection)ConfigurationManager.GetSection("serverconfig");
        public static AxServerConfigSection AxServerSettings;
        //public static string ExcelStorePool = "D:\\AxDocuments\\Pool\\ExcelStorePool";
        public static bool CanRegHIChildUnder6YearsOld(int PatientAge)
        {
            DateTime ValidateTodate = new DateTime(DateTime.Now.Year, 9, 30);
            if (PatientAge < 6 && DateTime.Now.Date <= ValidateTodate.Date)
            {
                return true;
            }
            return false;
        }
        public static string GetSafeXMLString(string aInput)
        {
            if (string.IsNullOrEmpty(aInput))
            {
                return null;
            }
            return aInput.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
        }
    }
}
