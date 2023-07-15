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
    public class FamilyTherapyService : IFamilyTherapy
    {
        List<FamilyTherapyTreeview> list;
        public FamilyTherapyService()
        {
            int currentID = System.Threading.Thread.CurrentThread.ManagedThreadId;
            list = new List<FamilyTherapyTreeview>();

        }

        #region IFamilyTherapyService Members

        [SingleResult]
        public RefFamilyTherapy GetFamilyTherapyByID(decimal faID)
        {
            using (eHCMSEntities context = new eHCMSEntities())
            {
                context.ContextOptions.LazyLoadingEnabled = false;

                RefFamilyTherapy curFamilyTherapy = context.RefFamilyTherapies.Where(p => p.FaID == faID).First<RefFamilyTherapy>();

                return curFamilyTherapy;
            }
        }

        public CRUDOperationResponse DeleteFamilyTherapy(RefFamilyTherapy familytherapy)
        {
            throw new Exception(eHCMSResources.Z1842_G1_NotImplementedOperation);
        }

        public CRUDOperationResponse DeleteFamilyTherapyByID(decimal faID)
        {
            throw new Exception(eHCMSResources.Z1842_G1_NotImplementedOperation);
        }

        public IList<RefFamilyTherapy> GetAllFamilyTherapies()
        {
            eHCMSEntities context = new eHCMSEntities();
            context.ContextOptions.LazyLoadingEnabled = false;
            return (from reffamilytherapy in context.RefFamilyTherapies select reffamilytherapy).ToList<RefFamilyTherapy>();
        }
        #endregion
        #region IFamilyTherapyService Members

        /// <summary>
        /// Get the patients at a specific page.
        /// </summary>
        /// <param name="pageIndex">The index of current page</param>
        /// <param name="pageSize">The number of records to load</param>
        /// <param name="bCountTotal">This variable indicates whether we want to count all the records satisfy the condition</param>
        /// <param name="totalCount">Total records (available if bCountTotal equals to true)</param>
        /// <returns></returns>
        public IList<RefFamilyTherapy> GetFamilyTherapies(bool bCountTotal, out int totalCount)
        {
            using (eHCMSEntities context = new eHCMSEntities())
            {
                context.ContextOptions.LazyLoadingEnabled = false;

                totalCount = 0;
                if (bCountTotal)
                {
                    totalCount = context.RefFamilyTherapies.Count();
                }

                IList<RefFamilyTherapy> familytherapies = context.RefFamilyTherapies.OrderBy(p => p.FaName).ToList<RefFamilyTherapy>();

                return familytherapies;
            }
        }
        public List<FamilyTherapyTreeview> GetFamilyTherapiesTreeView(bool bCountTotal, out int totalCount)
        {
            using (eHCMSEntities context = new eHCMSEntities())
            {
                context.ContextOptions.LazyLoadingEnabled = false;

                totalCount = 0;
                if (bCountTotal)
                {
                    totalCount = context.RefFamilyTherapies.Count();
                }

                IList<RefFamilyTherapy> familytherapies = context.RefFamilyTherapies.OrderBy(p => p.FaName).ToList<RefFamilyTherapy>();

                List<FamilyTherapyTreeview> results = new List<FamilyTherapyTreeview>();
                // transfer all the items from the result to the unsorted list
                foreach (RefFamilyTherapy item in familytherapies)
                {
                    FamilyTherapyTreeview genericItem = new FamilyTherapyTreeview(item.FaName, item.FaID, item.Ref_FaID,item.FaDescription);
                    this.list.Add(genericItem);
                }

                // Get all the first level nodes. In our case it is only one - House M.D.
                var rootNodes = this.list.Where(x => x.ParentID == x.NodeID);

                // Foreach root node, get all its children and add the node to the HierarchicalDataSource.
                // see bellow how the FindChildren method works
                foreach (FamilyTherapyTreeview node in rootNodes)
                {
                    this.FindChildren(node);
                    results.Add(node);
                }
                return results;
            }
        }

        public IList<RefFamilyTherapy> SearchFamilyTherapies(FamilyTherapySearchCriteria criteria, bool bCountTotal, out int totalCount)
        {
            if (criteria == null)
            {
                return GetFamilyTherapies(bCountTotal, out totalCount);
            }

            totalCount = 0;

            using (eHCMSEntities context = new eHCMSEntities())
            {
                context.ContextOptions.LazyLoadingEnabled = false;
                IQueryable<RefFamilyTherapy> q = context.RefFamilyTherapies.AsExpandable<RefFamilyTherapy>().OrderBy(p => p.FaName);

                var predicate = PredicateBuilder.True<RefFamilyTherapy>();
                Expression<Func<RefFamilyTherapy, bool>> tempPredicate = criteria.FaNameFilter;
                if (tempPredicate != null)
                    predicate = predicate.And(tempPredicate);

                q = q.Where(predicate);

                if (bCountTotal)
                {
                    totalCount = q.Count();
                }
                IList<RefFamilyTherapy> familytherapies = null;
                familytherapies = q.ToList<RefFamilyTherapy>();
                return familytherapies;
            }
        }

        public List<FamilyTherapyTreeview> GetTreeview(FamilyTherapySearchCriteria criteria, bool bCountTotal, out int totalCount)
        {
            if (criteria == null)
            {
                return GetFamilyTherapiesTreeView(bCountTotal, out totalCount);
            }

            totalCount = 0;
            using (eHCMSEntities context = new eHCMSEntities())
            {
                context.ContextOptions.LazyLoadingEnabled = false;
                IQueryable<RefFamilyTherapy> q = context.RefFamilyTherapies.AsExpandable<RefFamilyTherapy>().OrderBy(p => p.FaName);

                var predicate = PredicateBuilder.True<RefFamilyTherapy>();
                Expression<Func<RefFamilyTherapy, bool>> tempPredicate = criteria.FaNameFilter;
                if (tempPredicate != null)
                    predicate = predicate.And(tempPredicate);

                q = q.Where(predicate);

                if (bCountTotal)
                {
                    totalCount = q.Count();
                }
                IList<RefFamilyTherapy> familytherapies = null;
                familytherapies = q.ToList<RefFamilyTherapy>();
                List<FamilyTherapyTreeview> results = new List<FamilyTherapyTreeview>();
                // transfer all the items from the result to the unsorted list
                foreach (RefFamilyTherapy item in familytherapies)
                {
                    FamilyTherapyTreeview genericItem = new FamilyTherapyTreeview(item.FaName, item.FaID, item.Ref_FaID,item.FaDescription);
                    this.list.Add(genericItem);
                }

                // Get all the first level nodes. In our case it is only one - House M.D.
                var rootNodes = this.list.Where(x => x.ParentID == x.NodeID);

                // Foreach root node, get all its children and add the node to the HierarchicalDataSource.
                // see bellow how the FindChildren method works
                foreach (FamilyTherapyTreeview node in rootNodes)
                {
                    this.FindChildren(node);
                    results.Add(node);
                }

                return results;
            }
        }
        private void FindChildren(FamilyTherapyTreeview item)
        {
            // find all the children of the item
            var children = list.Where(x => x.ParentID == item.NodeID && x.NodeID != item.NodeID);

            // add the child to the item's children collection and call the FindChildren recursively, in case the child has children
            foreach (FamilyTherapyTreeview child in children)
            {
                item.Children.Add(child);
                FindChildren(child);
            }
        }
        public CRUDOperationResponse AddNewFamilyTherapy(RefFamilyTherapy newFamilyTherapy)
        {
            CRUDOperationResponse response = new CRUDOperationResponse();
            try
            {
                using (eHCMSEntities context = new eHCMSEntities())
                {
                    context.RefFamilyTherapies.AddObject(newFamilyTherapy);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                response.Error = ex;
                //TODO: Assign the Status code here
            }
            return response;
        }

        #endregion
    }
}
