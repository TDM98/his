using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Services;
using System.Data.Services.Common;
using System.ServiceModel.Web;
using EFData;
using System.ServiceModel.Activation;

using System.Linq.Expressions;
using eHCMS.LinqKit;
using System.ServiceModel;
using System.Runtime.Serialization;
using eHCMS.Services.Core;
namespace PharmacyService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class CommonService : ICommon
    {
        public CommonService()
        {
            int currentID = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }
        #region ICommonService Members

        //[SingleResult]
        //public IList<RefCountry> GetCountries()
        //{
        //    eHCMSEntities context = new eHCMSEntities();
        //    context.ContextOptions.LazyLoadingEnabled = false;
        //    return (from country in context.RefCountries select country).ToList<RefCountry>();
        //}

        //public IList<RefAddictiveType> GetAddictives()
        //{
        //    eHCMSEntities context = new eHCMSEntities();
        //    context.ContextOptions.LazyLoadingEnabled = false;
        //    return (from addictive in context.RefAddictiveTypes select addictive).ToList<RefAddictiveType>();
        //}

        //public IList<RefPoissonType> GetPoissons()
        //{
        //    eHCMSEntities context = new eHCMSEntities();
        //    context.ContextOptions.LazyLoadingEnabled = false;
        //    return (from poisson in context.RefPoissonTypes select poisson).ToList<RefPoissonType>();
        //}

        //public IList<RefUnit> GetUnits()
        //{
        //    eHCMSEntities context = new eHCMSEntities();
        //    context.ContextOptions.LazyLoadingEnabled = false;
        //    return (from unit in context.RefUnits select unit).ToList<RefUnit>();
        //}

        //public IList<RefFamilyTherapy> GetFamilyTherapies()
        //{
        //    eHCMSEntities context = new eHCMSEntities();
        //    context.ContextOptions.LazyLoadingEnabled = false;
        //    return (from familytherapy in context.RefFamilyTherapies select familytherapy).ToList<RefFamilyTherapy>();
        //}
        #endregion
    }
}
