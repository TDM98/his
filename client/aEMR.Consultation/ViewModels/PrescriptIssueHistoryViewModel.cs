using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using System.Threading;
using aEMR.ServiceClient;
using DataEntities;
using System.Collections.ObjectModel;
using System;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;

namespace aEMR.ConsultantEPrescription.ViewModels
{
     [Export(typeof(IPrescriptIssueHistory)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PrescriptIssueHistoryViewModel : Conductor<object>, IPrescriptIssueHistory
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }


        [ImportingConstructor]
        public PrescriptIssueHistoryViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
         {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }
        #region Properties member

         private ObservableCollection<PrescriptionIssueHistory> _PrescriptIssueHis;
         public ObservableCollection<PrescriptionIssueHistory> PrescriptIssueHis
         {
             get
             {
                 return _PrescriptIssueHis;
             }
             set
             {
                 if (_PrescriptIssueHis != value)
                 {
                     _PrescriptIssueHis = value;
                     NotifyOfPropertyChange(()=>PrescriptIssueHis);
                 }
             }
         }

         private long _PrescriptID;
         public long PrescriptID
         {
             get
             {
                 return _PrescriptID;
             }
             set
             {
                 if (_PrescriptID != value)
                 {
                     _PrescriptID = value;
                     NotifyOfPropertyChange(()=>PrescriptID);
                 }
             }
         }

         private long _CurPrecriptionID;
         public long CurPrecriptionID
         {
             get
             {
                 return _CurPrecriptionID;
             }
             set
             {
                 if (_CurPrecriptionID != value)
                 {
                     _CurPrecriptionID = value;
                     NotifyOfPropertyChange(()=>CurPrecriptionID);
                 }
             }
         }

         private long _OrgPrecriptionID;
         public long OrgPrecriptionID
         {
             get
             {
                 return _OrgPrecriptionID;
             }
             set
             {
                 if (_OrgPrecriptionID != value)
                 {
                     _OrgPrecriptionID = value;
                     NotifyOfPropertyChange(()=>OrgPrecriptionID);
                 }
             }
         }

         private string _AuthorFullName;
         public string AuthorFullName
         {
             get
             {
                 return _AuthorFullName;
             }
             set
             {
                 if (_AuthorFullName != value)
                 {
                     _AuthorFullName = value;
                     NotifyOfPropertyChange(()=>AuthorFullName);
                 }
             }
         }
        #endregion

         public void GetPrescriptionIssueHistory()
         {
             //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
             var t = new Thread(() =>
             {
                 IsLoading = true;

                 using (var serviceFactory = new ePrescriptionsServiceClient())
                 {
                     var contract = serviceFactory.ServiceInstance;

                     contract.BeginGetPrescriptionIssueHistory(PrescriptID, Globals.DispatchCallback((asyncResult) =>
                     {

                         try
                         {
                             var results = contract.EndGetPrescriptionIssueHistory(asyncResult);
                             PrescriptIssueHis = results.ToObservableCollection();
                             if (PrescriptIssueHis !=null && PrescriptIssueHis.Count > 0)
                             {
                                 CurPrecriptionID = (long)PrescriptIssueHis[0].PrescriptID;
                                 OrgPrecriptionID = (long)PrescriptIssueHis[PrescriptIssueHis.Count - 1].PrescriptID;
                                 AuthorFullName = PrescriptIssueHis[PrescriptIssueHis.Count - 1].Author;
                             }
                         }
                         catch (Exception ex)
                         {
                             Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                         }
                         finally
                         {
                             //Globals.IsBusy = false;
                             IsLoading = false;

                         }

                     }), null);

                 }

             });

             t.Start();
         }
    }
}
