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
using aEMR.Common;
using aEMR.Common.Collections;
using DataEntities;
using aEMR.CommonTasks;
using System.Collections.ObjectModel;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;

/*
 * 20180518 #001 TBLD: Load cac lan lam CLS.
 * 20180524 #002 TBLD: Chi cho in cac phieu CLS da hoan tat.
 * 20180826 #003 TTM: Thiếu Case load Siêu âm bụng (Abdominal Ultrasound) bổ sung.
*/
namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof(IListPCLRequest_ImagingByPatientIDV_Param)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ListPCLRequest_ImagingByPatientIDV_ParamViewModel : Conductor<object>, IListPCLRequest_ImagingByPatientIDV_Param
        , IHandle<DbClickSelectedObjectEvent<PatientPCLRequest>>
        , IHandle<ItemSelected<Patient>>
    {
        public object UCDoctorProfileInfo { get; set; }
        public object UCPatientProfileInfo { get; set; }
        public object SearchRegistrationContent { get; set; }
        public Patient curPatient
        {
            get;
            set;
        }
        public void Handle(ItemSelected<Patient> Item)
        {
            if (Item != null)
            {
                SearchCriteria.PatientID = Item.Item.PatientID;
                SearchCriteria.V_Param = Globals.PCLDepartment.ObjPCLResultParamImpID.ParamEnum;
                ObjPatientPCLRequest_ByPatientIDV_Param_Paging.PageIndex = 0;
                ObjPatientPCLRequest_ByPatientIDV_Param_Paging.PageSize = 20;
                PatientPCLRequest_ByPatientIDV_Param_Paging(ObjPatientPCLRequest_ByPatientIDV_Param_Paging.PageIndex, ObjPatientPCLRequest_ByPatientIDV_Param_Paging.PageSize, true);
                Globals.PatientPCLRequest_Result = null;
            }
        }
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ListPCLRequest_ImagingByPatientIDV_ParamViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);

            var uc1 = Globals.GetViewModel<ILoginInfo>();
            UCDoctorProfileInfo = uc1;
            ActivateItem(uc1);

            var uc2 = Globals.GetViewModel<IPatientSummaryInfoV2>();
            UCPatientProfileInfo = uc2;
            ActivateItem(uc2);

            var searchPatientAndRegVm = Globals.GetViewModel<ISearchPatientAndRegistration>();
            searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_PATIENT_BTN);
            searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_PATIENT);

            SearchRegistrationContent = searchPatientAndRegVm;
            ActivateItem(searchPatientAndRegVm);

            ObjPatientPCLRequest_ByPatientIDV_Param_Paging = new PagedSortableCollectionView<PatientPCLRequest>();
            //ObjPatientPCLRequest_ByPatientIDV_Param_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjPatientPCLRequest_ByPatientIDV_Param_Paging_OnRefresh);
            
            SearchCriteria=new PatientPCLRequestSearchCriteria();
 
            LoadData();
        }

        private void LoadData()
        {
            if (Globals.PatientPCLRequest_Result != null)
            {
                SearchCriteria.PatientID = Globals.PatientPCLRequest_Result.PatientID;
                SearchCriteria.V_Param = Globals.PCLDepartment.ObjPCLResultParamImpID.ParamEnum;
                /*▼====: #001*/
                SearchCriteria.PatientFindBy = AllLookupValues.PatientFindBy.CAHAI;
                /*▲====: #001*/
                ObjPatientPCLRequest_ByPatientIDV_Param_Paging.PageIndex = 0;
                PatientPCLRequest_ByPatientIDV_Param_Paging(ObjPatientPCLRequest_ByPatientIDV_Param_Paging.PageIndex, ObjPatientPCLRequest_ByPatientIDV_Param_Paging.PageSize, true);
                Coroutine.BeginExecute(GetPatient());
                
            }

        }
        private IEnumerator<IResult> GetPatient()
        {
            var loadPatients = new LoadPatientTask(Globals.PatientPCLRequest_Result.PatientID);
            yield return loadPatients;
            Patient CurrentPatient = loadPatients.CurrentPatient;
            if (CurrentPatient != null)
            {
                Globals.EventAggregator.Publish(new ItemPatient<Patient>() { Item = CurrentPatient });
            }
        }
        void ObjPatientPCLRequest_ByPatientIDV_Param_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            PatientPCLRequest_ByPatientIDV_Param_Paging(ObjPatientPCLRequest_ByPatientIDV_Param_Paging.PageIndex, ObjPatientPCLRequest_ByPatientIDV_Param_Paging.PageSize, false);
        }

        private PatientPCLRequestSearchCriteria _SearchCriteria;
        public PatientPCLRequestSearchCriteria SearchCriteria
        {
            get { return _SearchCriteria; }
            set
            {
                if (_SearchCriteria != value)
                {
                    _SearchCriteria = value;
                    NotifyOfPropertyChange(() => SearchCriteria);
                }
            }
        }

        private PagedSortableCollectionView<DataEntities.PatientPCLRequest> _ObjPatientPCLRequest_ByPatientIDV_Param_Paging;
        public PagedSortableCollectionView<DataEntities.PatientPCLRequest> ObjPatientPCLRequest_ByPatientIDV_Param_Paging
        {
            get { return _ObjPatientPCLRequest_ByPatientIDV_Param_Paging; }
            set
            {
                _ObjPatientPCLRequest_ByPatientIDV_Param_Paging = value;
                NotifyOfPropertyChange(() => ObjPatientPCLRequest_ByPatientIDV_Param_Paging);
            }
        }

       
        private void PatientPCLRequest_ByPatientIDV_Param_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            ObjPatientPCLRequest_ByPatientIDV_Param_Paging.Clear();

            Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách..." });

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPatientPCLRequest_ByPatientIDV_Param_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DataEntities.PatientPCLRequest> allItems = null;
                            bool bOK = false;

                            try
                            {
                                allItems = client.EndPatientPCLRequest_ByPatientIDV_Param_Paging(out Total, asyncResult);
                                bOK = true;
                            }
                            catch (Exception innerEx)
                            {
                                ClientLoggerHelper.LogInfo(innerEx.ToString());
                            }

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjPatientPCLRequest_ByPatientIDV_Param_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjPatientPCLRequest_ByPatientIDV_Param_Paging.Add(item);
                                    }
                                    ObjPatientPCLRequest_ByPatientIDV_Param_Paging.PageSize = PageSize;
                                    ObjPatientPCLRequest_ByPatientIDV_Param_Paging.PageIndex = PageIndex;
                                }
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
                finally
                {
                    Globals.IsBusy = false;
                }
            });
            t.Start();
        }

        public void DoubleClick(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;
            PatientPCLRequest pcltmp = eventArgs.Value as PatientPCLRequest;
            Globals.PatientPCLReqID_Imaging = pcltmp.PatientPCLReqID;
            /*▼====: #001*/
            Globals.PatientPCLRequest_Result = pcltmp;
            /*▲====: #001*/
            LoadContentByParaEnum();
        }
        private ObservableCollection<PCLResultFileStorageDetailClient> _ObjGetPCLResultFileStoreDetails;
        public ObservableCollection<PCLResultFileStorageDetailClient> ObjGetPCLResultFileStoreDetails
        {
            get { return _ObjGetPCLResultFileStoreDetails; }
            set
            {
                _ObjGetPCLResultFileStoreDetails = value;
                NotifyOfPropertyChange(() => ObjGetPCLResultFileStoreDetails);
            }
        }
        private IEnumerator<IResult> LoadDataCoroutine(PatientPCLRequest p)
        {
            /*▼====: #002*/
            if (p.V_ExamRegStatus == (int)AllLookupValues.ExamRegStatus.HOAN_TAT)
            /*▲====: #002*/
            {
                var ResultFileStoreDetails = new LoadPCLResultFileStoreDetailsTask(p.PatientID, p.PatientPCLReqID, p.PCLExamTypeID, (long)p.V_PCLRequestType);
                yield return ResultFileStoreDetails;
                if (ObjGetPCLResultFileStoreDetails == null)
                {
                    ObjGetPCLResultFileStoreDetails = new ObservableCollection<PCLResultFileStorageDetailClient>();
                }
                ObjGetPCLResultFileStoreDetails.Clear();
                if (ResultFileStoreDetails.ObjGetPCLResultFileStoreDetails != null)
                {
                    foreach (var item in ResultFileStoreDetails.ObjGetPCLResultFileStoreDetails)
                    {
                        ObjGetPCLResultFileStoreDetails.Add(new PCLResultFileStorageDetailClient { IOStream = null, ObjectResult = item });

                    }
                }
                LoadPrintTemplate(p);
                yield break;
            }
            /*▼====: #002*/
            else
            {
                MessageBox.Show(eHCMSLanguage.eHCMSResources.Z2210_G1_KQPCLChuaLuu);
            }
            /*▲====: #002*/
        }
        public void hplPrint_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                PatientPCLRequest pcltmp = selectedItem as PatientPCLRequest;
                Coroutine.BeginExecute(LoadDataCoroutine(pcltmp));
            }
        }
        private void LoadPrintTemplate(PatientPCLRequest pcltmp)
        {
            //var proAlloc = Globals.GetViewModel<ICommonPreviewView>();
            //proAlloc.PatientPCLReqID = pcltmp.PatientPCLReqID;
            //switch (Globals.PCLDepartment.ObjPCLResultParamImpID.ParamEnum)
            //{
            //    case 1:
            //        proAlloc.eItem = ReportName.PCLDEPARTMENT_IMAGERESULT_HEART_ULTRASOUND;
            //        break;
            //    case 5:
            //        proAlloc.eItem = ReportName.FETAL_ECHOCARDIOGRAPHY;
            //        break;
            //    case 14:
            //        proAlloc.eItem = ReportName.ABDOMINAL_ULTRASOUND_RESULT;
            //        break;
            //    default:
            //        MessageBox.Show("Loại chẩn đoán hình ảnh này, không hỗ trợ chức năng In !");
            //        return;


            //}
            //if (ObjGetPCLResultFileStoreDetails != null && ObjGetPCLResultFileStoreDetails.Count > 0)
            //{
            //    proAlloc.EchoCardioType1ImageResultFile1 = ObjGetPCLResultFileStoreDetails[0].ObjectResult.FullPath;
            //}
            //if (ObjGetPCLResultFileStoreDetails != null && ObjGetPCLResultFileStoreDetails.Count > 1)
            //{
            //    proAlloc.EchoCardioType1ImageResultFile2 = ObjGetPCLResultFileStoreDetails[1].ObjectResult.FullPath;
            //}
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });
            //Globals.PatientPCLReqID_Imaging = pcltmp.PatientPCLReqID;
            //LoadContentByParaEnum();

            Action<ICommonPreviewView> onInitDlg = (proAlloc) =>
            {
                proAlloc.PatientPCLReqID = pcltmp.PatientPCLReqID;
                switch (Globals.PCLDepartment.ObjPCLResultParamImpID.ParamEnum)
                {
                    case 1:
                        proAlloc.eItem = ReportName.PCLDEPARTMENT_IMAGERESULT_HEART_ULTRASOUND;
                        break;
                    case 5:
                        proAlloc.eItem = ReportName.FETAL_ECHOCARDIOGRAPHY;
                        break;
                    case 14:
                        proAlloc.eItem = ReportName.ABDOMINAL_ULTRASOUND_RESULT;
                        break;
                    default:
                        MessageBox.Show("Loại chẩn đoán hình ảnh này, không hỗ trợ chức năng In !");
                        return;


                }
                if (ObjGetPCLResultFileStoreDetails != null && ObjGetPCLResultFileStoreDetails.Count > 0)
                {
                    proAlloc.EchoCardioType1ImageResultFile1 = ObjGetPCLResultFileStoreDetails[0].ObjectResult.FullPath;
                }
                if (ObjGetPCLResultFileStoreDetails != null && ObjGetPCLResultFileStoreDetails.Count > 1)
                {
                    proAlloc.EchoCardioType1ImageResultFile2 = ObjGetPCLResultFileStoreDetails[1].ObjectResult.FullPath;
                }
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);

        }
       

        #region Load Content BY ENUM
        private void LoadContentByParaEnum()
        {
            if (Globals.PCLDepartment.ObjV_PCLMainCategory == null)
            {
                return;
            }
            if (Globals.PCLDepartment.ObjV_PCLMainCategory.LookupID != (long)AllLookupValues.V_PCLMainCategory.Imaging)
            {
                return;
            }
            switch (Globals.PCLDepartment.ObjPCLResultParamImpID.ParamEnum)
            {
                case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_TIMMAU:
                    {
                        var Module = Globals.GetViewModel<IPCLDepartmentContent>();
                        var VM = Globals.GetViewModel<ISieuAmTim>();

                        Module.MainContent = VM;
                        (Module as Conductor<object>).ActivateItem(VM);
                        break;
                    }
                case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_MACHMAU:
                    {
                        var Module = Globals.GetViewModel<IPCLDepartmentContent>();
                        var VM = Globals.GetViewModel<ISieuAmMachMauHome>();

                        Module.MainContent = VM;
                        (Module as Conductor<object>).ActivateItem(VM);
                        break;
                    }
                case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_GANGSUC_Dipyridamole:
                    {
                        var Module = Globals.GetViewModel<IPCLDepartmentContent>();
                        var VM = Globals.GetViewModel<ISieuAmTimGangSucDipyridamoleHome>();

                        Module.MainContent = VM;
                        (Module as Conductor<object>).ActivateItem(VM);
                        break;
                    }
                case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_GANGSUC_Dobutamine:
                    {
                        var Module = Globals.GetViewModel<IPCLDepartmentContent>();
                        var VM = Globals.GetViewModel<ISieuAmTimGangSucDobutamineHome>();
 
                        Module.MainContent = VM;
                        (Module as Conductor<object>).ActivateItem(VM);
                        break;
                    }
                case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_TIMTHAI:
                    {
                        var Module = Globals.GetViewModel<IPCLDepartmentContent>();
                        var VM = Globals.GetViewModel<ISieuAmTimThaiHome>();

                        Module.MainContent = VM;
                        (Module as Conductor<object>).ActivateItem(VM);
                        break;
                    }
                case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_THUCQUAN:
                    {
                        var Module = Globals.GetViewModel<IPCLDepartmentContent>();
                        var VM = Globals.GetViewModel<ISieuAmTimQuaThucQuanHome>();

                        Module.MainContent = VM;
                        (Module as Conductor<object>).ActivateItem(VM);
                        break;
                    }
                    //▼======#003
                case (int)AllLookupValues.PCLResultParamImpID.ABDOMINAL_ULTRASOUND:
                    {
                        var Module = Globals.GetViewModel<IPCLDepartmentContent>();
                        var VM = Globals.GetViewModel<IAbdominalUltrasoundMain>();
                        
                        Module.MainContent = VM;
                        (Module as Conductor<object>).ActivateItem(VM);
                        break;
                    }
                    //▲====== #003
                default:
                    {
                        var Module = Globals.GetViewModel<IPCLDepartmentContent>();
                        var VM = Globals.GetViewModel<IPCLDeptImagingResult>();

                        Module.MainContent = VM;
                        (Module as Conductor<object>).ActivateItem(VM);
                        break;
                    }
            }
        }
        #endregion
        
        public void Handle(DbClickSelectedObjectEvent<PatientPCLRequest> message)
        {
            if (message != null)
            {
                //LoadData();
            }
        }
    }
}
