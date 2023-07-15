using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataEntities;
using System.Data.SqlClient;
using System.Data;

using System.Reflection;
using eHCMS.Configurations;
using eHCMS.Services.Core;
using System.Collections.ObjectModel;
using Service.Core.Common;

namespace eHCMS.DAL
{
    public abstract class PtRegistrationProvider : DataProviderBase
    {
           static private PtRegistrationProvider _instance = null;
        /// <summary>
        /// Returns an instance of the provider type specified in the config file (using reflection)
        /// </summary>
        static public PtRegistrationProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    string tempPath = AppDomain.CurrentDomain.RelativeSearchPath;
                    if (string.IsNullOrEmpty(AppDomain.CurrentDomain.RelativeSearchPath))
                        tempPath = AppDomain.CurrentDomain.BaseDirectory;
                    else
                        tempPath = AppDomain.CurrentDomain.RelativeSearchPath;
                    string assemblyPath = System.IO.Path.Combine(tempPath, Globals.Settings.Common.Assembly + ".dll");
                    Assembly assem = Assembly.LoadFrom(assemblyPath);
                    Type t = assem.GetType(Globals.Settings.Common.PatientRg.ProviderType);
                    _instance = (PtRegistrationProvider)Activator.CreateInstance(t);
                    //_instance = (PatientProvider)Activator.CreateInstance(Type.GetType(Globals.Settings.Patients.ProviderType));
                }
                return _instance;
            }
        }

        public PtRegistrationProvider()
        {
            this.ConnectionString = Globals.Settings.Common.ConnectionString;
        }

        public abstract DateTime GetMaxExamDate();
        public abstract List<Patient> SearchPatients(PatientSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        public abstract List<PatientRegistration> SearchPtRegistration(SeachPtRegistrationCriteria criteria,int pageSize,int pageIndex,bool bcount,out int Totalcount);
        public abstract Patient GetPatientInfoByPtRegistration(long? PtRegistrationID, long? PatientID, int FindPatient);
        public abstract bool UpdatePatientRegistrationPayStatus(long PtRegistrationID, long V_RegistrationPaymentStatus);
        public abstract bool UpdatePatientRegistration(long PtRegistrationID, long V_RegistrationStatus);

        public abstract bool PatientRegistration_Close(long PtRegistrationID, int FindPatient, long V_RegistrationClosingStatus);

        public abstract List<PatientRegistration> GetAllPatientRegistration_ByRegType(long PatientID,
                                                                                      long V_RegistrationType);

    }
}
