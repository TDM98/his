using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using System.Windows;
using System.Windows.Controls;
using DataEntities;
using aEMR.Common;
using aEMR.DataContracts;
using aEMR.CommonTasks;
using System.Collections.Generic;
using aEMR.Common.Collections;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using aEMR.ServiceClient;
using System.Linq;
using aEMR.Controls;
using Castle.Windsor;
using aEMR.Common.BaseModel;
//using Microsoft.Web.WebView2.Wpf;
using System.IO;
using DevExpress.ReportServer.Printing;
using aEMR.ReportModel.ReportModels;
/*
* 20180923 #001 TTM: 
* 20190815 #002 TTM:   BM 0013133: Không load lại màn hình kết quả khi tìm đăng ký mới. Dẫn tới có thể gây nhầm lẫn kết quả.
* 20220901 #003 BLQ: Issue:2174 Chỉnh lại mẫu hình ảnh theo cách mới
*/
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IPatientPCLImagingResult_V2)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientPCLImagingResult_V2ViewModel : ViewModelBase, IPatientPCLImagingResult_V2

    {
        #region Properties member
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
        #endregion
        [ImportingConstructor]
        public PatientPCLImagingResult_V2ViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
           
        }
        private PatientPCLImagingResult _curPatientPCLImagingResult;
        public PatientPCLImagingResult curPatientPCLImagingResult
        {
            get
            {
                return _curPatientPCLImagingResult;
            }
            set
            {
                if (_curPatientPCLImagingResult == value)
                    return;
                _curPatientPCLImagingResult = value;
                NotifyOfPropertyChange(() => curPatientPCLImagingResult);
            }
        }
        //WebView2 webView = null;
        public object TabHinhAnhPCL
        {
            get;
            set;
        }
        public object TabHinhAnhPCL_New
        {
            get;
            set;
        }
        private IHtmlReport _PCLOldView;
        public IHtmlReport PCLOldView
        {
            get { return _PCLOldView; }
            set
            {
                _PCLOldView = value;
                NotifyOfPropertyChange(() => PCLOldView);
            }
        }
        //public void WebView_Loaded(object sender, RoutedEventArgs e)
        //{
        //    webView = (WebView2)sender;
        //}
        public void TabHinhAnhPCL_Loaded(object sender)
        {
            TabHinhAnhPCL = (TabItem)sender;
            if (PCLOldView == null)
            {
                var VMTT = Globals.GetViewModel<IHtmlReport>();
                PCLOldView = VMTT;
                this.ActivateItem(VMTT);
            }
        }
        public void TabHinhAnhPCL_New_Loaded(object sender)
        {
            TabHinhAnhPCL_New = (TabItem)sender;
        }
        private RemoteDocumentSource _reportModel;
        public RemoteDocumentSource ReportModel
        {
            get { return _reportModel; }
            set
            {
                _reportModel = value;
                NotifyOfPropertyChange(() => ReportModel);
            }
        }
        private PatientPCLRequest _ObjPatientPCLRequest;
        public PatientPCLRequest ObjPatientPCLRequest
        {
            get { return _ObjPatientPCLRequest; }
            set
            {
                _ObjPatientPCLRequest = value;
                NotifyOfPropertyChange(() => ObjPatientPCLRequest);
            }
        }
        private bool _mPCLImage_New = false;
        public bool mPCLImage_New
        {
            get
            {
                return _mPCLImage_New;
            }
            set
            {
                if (_mPCLImage_New == value)
                    return;
                _mPCLImage_New = value;
                NotifyOfPropertyChange(() => mPCLImage_New);
            }
        }
        private bool _mPCLImage = false;
        public bool mPCLImage
        {
            get
            {
                return _mPCLImage;
            }
            set
            {
                if (_mPCLImage == value)
                    return;
                _mPCLImage = value;
                NotifyOfPropertyChange(() => mPCLImage);
            }
        }
        public void InitHTML()
        {
            if (PCLOldView == null)
            {
                var VMTT = Globals.GetViewModel<IHtmlReport>();
                PCLOldView = VMTT;
                this.ActivateItem(VMTT);
            }
        }
        public void CheckTemplatePCLResultByReqID(long PatientPCLReqID,bool InPt)
        {

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginCheckTemplatePCLResultByReqID(PatientPCLReqID, InPt, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            bool IsNewTemplate = false;
                            long V_ReportForm = 0;
                            long PCLImgResultID = 0;
                            long V_PCLRequestType = 0;
                            string TemplateResultString = "";
                            var result = contract.EndCheckTemplatePCLResultByReqID(out IsNewTemplate, out V_ReportForm, out PCLImgResultID, out V_PCLRequestType, out TemplateResultString, asyncResult);
                            if (IsNewTemplate)
                            {
                                mPCLImage_New = true;
                                ((TabItem)TabHinhAnhPCL_New).IsSelected = true;
                                //PCLResultParamImpName = curPatientServicesTree.Description;
                                btnViewPrintNew(V_ReportForm, PCLImgResultID, V_PCLRequestType);
                            }
                            else
                            {
                                mPCLImage = true;
                                ((TabItem)TabHinhAnhPCL).IsSelected = true;
                                //PCLResultParamImpName = curPatientServicesTree.Description;
                                //var VMTT = Globals.GetViewModel<IHtmlReport>();
                                //PCLOldView = VMTT;
                                //this.ActivateItem(VMTT);
                                PCLOldView.NavigateToString(TemplateResultString);
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            t.Start();
        }
        //private void GetPatientPCLRequestResultsByReqID(long PatientPCLReqID, bool InPt)
        //{
        //    PatientPCLRequestSearchCriteria mSearchCriteria = new PatientPCLRequestSearchCriteria();
        //    mSearchCriteria.PatientPCLReqID = PatientPCLReqID;
        //    mSearchCriteria.V_PCLMainCategory = (long)AllLookupValues.V_PCLMainCategory.Imaging;
        //    mSearchCriteria.PatientFindBy = InPt ? AllLookupValues.PatientFindBy.NOITRU: AllLookupValues.PatientFindBy.NGOAITRU;
        //    var t = new Thread(() =>
        //    {
        //        using (var serviceFactory = new PCLsClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;
        //            contract.BeginGetPatientPCLRequestResultsByReqID(mSearchCriteria, Globals.DispatchCallback((asyncResult) =>
        //            {
        //                try
        //                {
        //                    var mPCLRequest = contract.EndGetPatientPCLRequestResultsByReqID(asyncResult);
        //                    if (mPCLRequest != null && mPCLRequest.PatientPCLReqID != 0)
        //                    {
        //                        ObjPatientPCLRequest = mPCLRequest;
        //                        Coroutine.BeginExecute(LoadDataCoroutine(ObjPatientPCLRequest));
        //                        //PCLImagingResults_With_ResultOld(ObjPatientPCLRequest_SearchPaging_Selected.PatientID, ObjPatientPCLRequest_SearchPaging_Selected.PatientPCLReqID, (long)ObjPatientPCLRequest_SearchPaging_Selected.V_PCLRequestType);
        //                    }
        //                    else
        //                    {
        //                        this.HideBusyIndicator();
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
        //                    this.HideBusyIndicator();
        //                }
        //            }), null);
        //        }
        //    });
        //    t.Start();
        //}

        //private IEnumerator<IResult> LoadDataCoroutine(PatientPCLRequest p)
        //{
        //    curPatientPCLImagingResult = new PatientPCLImagingResult();
        //    curPatientPCLImagingResult.PatientPCLRequest = p;

        //    curPatientPCLImagingResult.UserOfficialAccountID = Globals.DoctorAccountBorrowed.StaffID;
        //    curPatientPCLImagingResult.PatientPCLReqID = p.PatientPCLReqID;
        //    curPatientPCLImagingResult.PCLExamTypeID = p.PCLExamTypeID.GetValueOrDefault();
        //    curPatientPCLImagingResult.PCLExamType = new PCLExamType
        //    {
        //        PCLExamTypeID = p.PCLExamTypeID.GetValueOrDefault(),
        //        PCLExamTypeName = p.PCLExamTypeName,
        //        V_PCLMainCategory = p.V_PCLMainCategory,
        //        V_ReportForm = p.V_ReportForm
        //    };
        //    var LoadPCLImagingResultsID = new LoadPatientPCLImagingResults_ByIDTask(p.PatientPCLReqID, p.PCLExamTypeID.GetValueOrDefault(), (long)p.V_PCLRequestType);
        //    yield return LoadPCLImagingResultsID;
        //    if (LoadPCLImagingResultsID.ObjPatientPCLImagingResult != null && LoadPCLImagingResultsID.ObjPatientPCLImagingResult.PatientPCLReqID.GetValueOrDefault() > 0)
        //    {
        //        curPatientPCLImagingResult.PCLImgResultID = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.PCLImgResultID;
        //        curPatientPCLImagingResult.TemplateResultString = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.TemplateResultString;
        //    }
        //    else
        //    {
        //        if (curPatientPCLImagingResult != null)
        //        {
        //            curPatientPCLImagingResult.PCLImgResultID = 0;
        //            curPatientPCLImagingResult.TemplateResultString = null;
        //        }
        //    }
        //    if (!mPCLImage_New)
        //    {
        //        //NavigateAsync();
        //        if (File.Exists(curPatientPCLImagingResult.TemplateResultString))
        //        {
        //            string text = System.IO.File.ReadAllText(curPatientPCLImagingResult.TemplateResultString);
        //            PCLOldView.NavigateToString(text);
        //        }
        //    }
        //    else
        //    {
        //        btnViewPrintNew();
        //    }
        //    yield break;
        //}
        //private async void NavigateAsync()
        //{
        //    if (webView != null)
        //    {
        //        await webView.EnsureCoreWebView2Async();
        //        if (File.Exists(curPatientPCLImagingResult.TemplateResultString))
        //        {
        //            string text = System.IO.File.ReadAllText(curPatientPCLImagingResult.TemplateResultString);
        //            webView.CoreWebView2.NavigateToString(text);
        //            webView.IsEnabled = false;
        //        }
        //        else
        //        {
        //            webView.CoreWebView2.Navigate("about:blank");
        //        }
        //    }
        //}
        public void btnViewPrintNew(long V_ReportForm, long PCLImgResultID, long V_PCLRequestType)
        {
            //if (curPatientPCLImagingResult.PatientPCLRequest == null)
            //{
            //    return;
            //}
            //if (curPatientPCLImagingResult.PCLExamType == null || curPatientPCLImagingResult.PCLExamType.V_ReportForm == 0)
            //{
            //    return;
            //}
            DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
            ReportModel = null;
            switch (V_ReportForm)
            {
                case (long)AllLookupValues.V_ReportForm.Mau_0_Hinh:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_0_Hinh").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_1_Hinh:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_1_Hinh").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_2_Hinh:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_3_Hinh:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_3_Hinh").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_4_Hinh:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_4_Hinh").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_6_Hinh:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_6_Hinh").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_Realtime_PCR:
                    if (Globals.ServerConfigSection.CommonItems.IsApplyPCRDual)
                    {
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Realtime_PCR_Cov_Dual").PreviewModel;
                    }
                    else
                    {
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Realtime_PCR_Cov").PreviewModel;
                    }
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_Test_Nhanh:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Test_Nhanh_Cov").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_Xet_Nghiem:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Xet_Nghiem").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_Helicobacter_Pylori:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Helicobacter_Pylori").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_Sieu_Am_Tim:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Sieu_Am_Tim").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_Sieu_Am_San_4D:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Sieu_Am_San_4D").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_Dien_Tim:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Dien_Tim").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_Dien_Nao:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Dien_Nao").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_ABI:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_ABI").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_Dien_Tim_Gang_Suc:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Dien_Tim_Gang_Suc").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_CLVT_Hai_Ham:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_CLVT_Hai_Ham").PreviewModel;
                    break;
                //▼====: #003
                case (long)AllLookupValues.V_ReportForm.Mau_Sieu_Am_San_4D_New:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Sieu_Am_San_4D_New").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_6_Hinh_2_New:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_6_Hinh_2_New").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_6_Hinh_1_New:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_6_Hinh_1_New").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_Sieu_Am_Tim_New:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Sieu_Am_Tim_New").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_Noi_Soi_9_Hinh:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Noi_Soi_9_Hinh").PreviewModel;
                    break;
                
                //▲====: #003
                default:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New").PreviewModel;
                    break;
            }
            rParams["PCLImgResultID"].Value = PCLImgResultID;
            rParams["V_PCLRequestType"].Value = V_PCLRequestType;
            rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
            rParams["parHospitalCode"].Value = Globals.ServerConfigSection.Hospitals.HospitalCode;
            rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
            rParams["parHospitalAddress"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalAddress;
            ReportModel.CreateDocument(rParams);
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
        }
        protected override void OnDeactivate(bool close)
        {
            Globals.EventAggregator.Unsubscribe(this);
            base.OnDeactivate(close);
        }
        //public void LoadDataCoroutineEx(PatientPCLRequest aPCLRequest)
        //{
        //    Coroutine.BeginExecute(LoadDataCoroutine(aPCLRequest));
        //}
    }
}