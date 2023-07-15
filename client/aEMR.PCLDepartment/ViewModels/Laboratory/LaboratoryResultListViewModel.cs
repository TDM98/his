using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using aEMR.CommonTasks;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using eHCMSLanguage;
using aEMR.Common.BaseModel;

namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof(ILaboratoryResultList)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class LaboratoryResultListViewModel : ViewModelBase, ILaboratoryResultList
        , IHandle<DbClickSelectedObjectEvent<PatientPCLRequest>>
        , IHandle<LocationSelected>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public LaboratoryResultListViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            allPatientPCLRequest = new PagedSortableCollectionView<PatientPCLRequest>();

            eventAggregator.Subscribe(this);

            //if (Globals.PatientPCLReqID_LAB > 0 && Globals.PatientAllDetails.PatientInfo != null && Globals.PatientAllDetails.PatientInfo.PatientID > 0)
            //{
            //    ListPatientPCLRequest_LAB_Paging(Globals.PatientAllDetails.PatientInfo.PatientID, Globals.DeptLocation.DeptLocationID, (long)Globals.PatientPCLRequest_LAB.V_PCLRequestType, 0,
            //                                     allPatientPCLRequest.PageSize, false);
            //}
            allPatientPCLRequest.OnRefresh += new EventHandler<RefreshEventArgs>(allPatientPCLRequest_OnRefresh);
        }



        void allPatientPCLRequest_OnRefresh(object sender, RefreshEventArgs e)
        {
            if (Globals.PatientPCLReqID_LAB > 0 && PatientID > 0)
            {
                ListPatientPCLRequest_LAB_Paging(PatientID, Globals.DeptLocation.DeptLocationID, (long)Globals.PatientPCLRequest_LAB.V_PCLRequestType, allPatientPCLRequest.PageIndex
                    , allPatientPCLRequest.PageSize, false);
            }
        }

        #region properties

        private string _sTitle;
        public string sTitle
        {
            get
            {
                return _sTitle;
            }
            set
            {
                if (_sTitle != value)
                {
                    _sTitle = value;
                    NotifyOfPropertyChange(() => sTitle);
                }
            }
        }

        private PagedSortableCollectionView<PatientPCLRequest> _allPatientPCLRequest;
        public PagedSortableCollectionView<PatientPCLRequest> allPatientPCLRequest
        {
            get
            {
                return _allPatientPCLRequest;
            }
            set
            {
                if (_allPatientPCLRequest == value)
                    return;
                _allPatientPCLRequest = value;
                NotifyOfPropertyChange(() => allPatientPCLRequest);
            }
        }

        #endregion


        #region method

        private void ListPatientPCLRequest_LAB_Paging(long PatientID, long? DeptLocID, long V_PCLRequestType, int PageIndex, int PageSize, bool CountTotal)
        {
            this.ShowBusyIndicator("Danh Sách Phiếu...");
            allPatientPCLRequest.Clear();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginListPatientPCLRequest_LAB_Paging(PatientID, DeptLocID, V_PCLRequestType, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;

                            var allItems = client.EndListPatientPCLRequest_LAB_Paging(out Total, asyncResult);

                            if (allItems != null)
                            {
                                if (CountTotal)
                                {
                                    allPatientPCLRequest.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        allPatientPCLRequest.Add(item);
                                    }
                                }
                            }


                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        #endregion

        public void btnRefresh()
        {
            if (Globals.PatientPCLRequest_LAB != null)
            {
                ListPatientPCLRequest_LAB_Paging(PatientID, Globals.DeptLocation.DeptLocationID, (long)Globals.PatientPCLRequest_LAB.V_PCLRequestType, allPatientPCLRequest.PageIndex, allPatientPCLRequest.PageSize, true);
            }
        }
        long PatientID = 0;
        public void Handle(DbClickSelectedObjectEvent<PatientPCLRequest> message)
        {
            if (message != null)
            {
                if (Globals.PCLDepartment.ObjV_PCLMainCategory.LookupID == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
                {
                    sTitle = eHCMSResources.K1485_G1_CacPhCua;
                    sTitle = string.Format(sTitle, message.Result.FullName.Trim());
                    PatientID = message.Result.PatientID;
                    //20181213 BM 0005357: Khi tim phieu len de thuc hien se khong load danh sach phieu ben Outstanding Tasks nua vi lau
                    //ListPatientPCLRequest_LAB_Paging(message.Result.PatientID, Globals.DeptLocation.DeptLocationID, (long)message.Result.V_PCLRequestType, allPatientPCLRequest.PageIndex, allPatientPCLRequest.PageSize, true);
                }
            }
        }
        public IEnumerator<IResult> WarningMessegeWarning(string message)
        {
            var dialog = new MessageWarningShowDialogTask(message, "",false);
            yield return dialog;            
        }
        public void DoubleClick(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;
            PatientPCLRequest p = eventArgs.Value as PatientPCLRequest;
            string errStr = "";
            bool flag = false;

            //if (flag=p.CheckTraTien(p, ref errStr, Globals.EffectedPCLHours,Globals.EditDiagDays) == false)

            // Txd 25/05/2014 Replaced ConfigList
            if (flag = p.CheckTraTien(p, ref errStr, Globals.ServerConfigSection.Hospitals.EffectedPCLHours, Globals.ServerConfigSection.Hospitals.EditDiagDays) == false)
            {
                Coroutine.BeginExecute(WarningMessegeWarning(errStr));                
            }
            Globals.EventAggregator.Publish(new DbClickPatientPCLRequest<PatientPCLRequest, String> { ObjA = eventArgs.Value as PatientPCLRequest, ObjB = eHCMSResources.Z0055_G1_Edit, IsReadOnly = !flag });
            TryClose();
        }

        public void Handle(LocationSelected message)
        {
            if (message != null && message.DeptLocation != null)
            {
                allPatientPCLRequest = new PagedSortableCollectionView<PatientPCLRequest>();
            }
        }
    }
}