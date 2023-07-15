using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System;
using System.Windows.Forms;
using System.Xml.Linq;
using aEMR.Common.BaseModel;
using aEMR.Common;
using System.Windows.Input;
using DataEntities;
using System.Threading;
using aEMR.ServiceClient;
using aEMR.Infrastructure;
using System.IO;
using eHCMSLanguage;
using aEMR.CommonTasks;
using System.Drawing;
using System.Collections.ObjectModel;
using aEMR.Common.Collections;
using eHCMS.Services.Core.Base;
using System.Windows.Controls;
using PCLsProxy;
using aEMR.Infrastructure.Events;
using aEMR.Controls;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using HtmlAgilityPack;
/*
 * 20181207 #001 TTM:   BM 0005339: Thêm trường BS thực hiện và dời Mã máy ra mà hình Template
 * 20190815 #002 TTM:   BM 0013133: Không load lại màn hình kết quả khi tìm đăng ký mới. Dẫn tới có thể gây nhầm lẫn kết quả.
 * 20210118 #003 TNHX: Thêm các hiển thị khi nhận kết quả từ PAC
 * 20211004 #004 TNHX: Lọc danh sách bsi theo cấu hình trách nhiệm + theo tích tạm khóa
 * 20211013 #005 TNHX: Thêm tích chờ kết quả cho xn
 */
namespace aEMR.ViewModels
{
    [Export(typeof(IPatientPCLGeneralResult)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientPCLGeneralResultViewModel : ViewModelBase, IPatientPCLGeneralResult,
        IHandle<PrintEventActionView>
    {
        #region Properties
        //▼====: #001
        private bool _IsWaitResultEnabled = false;
        public bool IsWaitResultEnabled
        {
            get
            {
                return _IsWaitResultEnabled;
            }
            set
            {
                if (_IsWaitResultEnabled != value)
                {
                    _IsWaitResultEnabled = value;
                    NotifyOfPropertyChange(() => IsWaitResultEnabled);
                }
            }
        }
        private bool _IsWaitResultVisibility = false;
        public bool IsWaitResultVisibility
        {
            get
            {
                return _IsWaitResultVisibility;
            }
            set
            {
                if (_IsWaitResultVisibility != value)
                {
                    _IsWaitResultVisibility = value;
                    NotifyOfPropertyChange(() => IsWaitResultVisibility);
                }
            }
        }
        private bool _IsDoneVisibility = false;
        public bool IsDoneVisibility
        {
            get
            {
                return _IsDoneVisibility;
            }
            set
            {
                if (_IsDoneVisibility != value)
                {
                    _IsDoneVisibility = value;
                    NotifyOfPropertyChange(() => IsDoneVisibility);
                }
            }
        }
        //▲====: #001

        public System.Windows.Forms.WebBrowser WBGeneral { get; set; }
        private string _ContentBody;
        public string ContentBody
        {
            get => _ContentBody; set
            {
                _ContentBody = value;
                NotifyOfPropertyChange(() => ContentBody);
            }
        }
        private string _FileName;
        public string FileName
        {
            get => _FileName; set
            {
                _FileName = value;
                NotifyOfPropertyChange(() => FileName);
            }
        }
        GenericCoRoutineTask gGenTask { get; set; }
        List<string> gImageSources;
        public string BackupContentBody { get; set; }
        private ObservableCollection<TemplateFileName> _TemplateFileNameCollection;
        public ObservableCollection<TemplateFileName> TemplateFileNameCollection
        {
            get => _TemplateFileNameCollection; set
            {
                _TemplateFileNameCollection = value;
                NotifyOfPropertyChange(() => TemplateFileNameCollection);
                NotifyOfPropertyChange(() => IsMultiFileName);
            }
        }
        private PatientPCLRequest _PatientPCLRequestObj;
        public PatientPCLRequest PatientPCLRequestObj
        {
            get => _PatientPCLRequestObj; set
            {
                _PatientPCLRequestObj = value;
                IsWaitResultVisibility = _PatientPCLRequestObj.IsHaveWaitResult;
                if (_PatientPCLRequestObj.V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.CLOSE)
                {
                    IsWaitResultEnabled = false;
                    if (_PatientPCLRequestObj.IsWaitResult)
                    {
                        IsDoneVisibility = true;
                    }
                    else
                    {
                        IsDoneVisibility = false;
                    }
                }
                else
                {
                    IsWaitResultEnabled = true;
                    IsDoneVisibility = false;
                }
                NotifyOfPropertyChange(() => PatientPCLRequestObj);
            }
        }
        public bool IsMultiFileName
        {
            get
            {
                return TemplateFileNameCollection != null && TemplateFileNameCollection.Count > 0;
            }
        }
        #endregion
        #region Events
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public PatientPCLGeneralResultViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            eventArg.Subscribe(this);
            //▼====== #001
            if (Globals.PCLDepartment.ObjPCLResultParamImpID != null)
            {
                GetResourcesForMedicalServicesListByTypeID(Globals.PCLDepartment.ObjPCLResultParamImpID.PCLResultParamImpID);
            }
            aucHoldConsultDoctor = Globals.GetViewModel<IAucHoldConsultDoctor>();
            aucHoldConsultDoctor.StaffCatType = (long)V_StaffCatType.BacSi;
            aucDoctorResult = Globals.GetViewModel<IAucHoldConsultDoctor>();
            aucDoctorResult.StaffCatType = (long)V_StaffCatType.BacSi;
            //▼====: #004
            if (Globals.ServerConfigSection.ConsultationElements.FilterDoctorByDeptResponsibilitiesInPt
                && !Globals.IsUserAdmin
                && Globals.PCLDepartment.ObjPCLResultParamImpID != null)
            {
                if (Globals.PCLDepartment.ObjPCLResultParamImpID.ParamName == "Xét Nghiệm")
                {
                    if (Globals.ServerConfigSection.Hospitals.HospitalCode == "95076")
                    {
                        aucDoctorResult.CurrentDeptID = 24;
                        aucHoldConsultDoctor.CurrentDeptID = 24;
                    }
                    else
                    {
                        aucDoctorResult.CurrentDeptID = 21;
                        aucHoldConsultDoctor.CurrentDeptID = 21;
                    }
                }
                else
                {
                    aucDoctorResult.CurrentDeptID = Globals.DeptLocation.DeptID;
                    aucHoldConsultDoctor.CurrentDeptID = Globals.DeptLocation.DeptID;
                }
            }
            //▲====: #004

            ObjPatientPCLImagingResult_General = new PatientPCLImagingResult();
            //▲====== #001
        }
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            if (WBGeneral == null)
            {
                WBGeneral = (this.GetView() as Views.PatientPCLGeneralResultView).WBGeneral;
                //if (!string.IsNullOrEmpty(FileName))
                //    NotifyOfPropertyChange(() => FileName);
                //else if (!string.IsNullOrEmpty(ContentBody))
                //    NotifyOfPropertyChange(() => ContentBody);

                WBGeneral.DocumentCompleted += WBGeneral_DocumentCompleted;
            }
        }
        public override void NotifyOfPropertyChange(string propertyName)
        {
            base.NotifyOfPropertyChange(propertyName);
            //if (WBGeneral == null) return;
            //if (propertyName.Equals("FileName") && !string.IsNullOrEmpty(FileName))
            //{
            //    //WBGeneral.Navigate(new Uri(String.Format("file:///{0}/ReportTemplates/{1}", System.IO.Directory.GetCurrentDirectory(), FileName)));
            //    //WBGeneral.Navigate("about:blank");
            //    string mReportFilePath = Path.Combine("ReportTemplates", FileName);
            //    GetReportBody(mReportFilePath);
            //}
            //else if (propertyName.Equals("ContentBody") && !string.IsNullOrEmpty(ContentBody))
            //{
            //    WBGeneral.Navigate("about:blank");
            //    if (WBGeneral.Document != null)
            //    {
            //        WBGeneral.Document.Write(string.Empty);
            //    }
            //    WBGeneral.DocumentText = ContentBody;
            //}
        }
        protected override IEnumerable<InputBindingCommand> GetInputBindingCommands()
        {
            yield return new InputBindingCommand(PrintContent)
            {
                GestureModifier = ModifierKeys.Control,
                GestureKey = Key.P
            };
        }
        //public static Byte[] PdfSharpConvert(String aHtml)
        //{
        //    Byte[] mBytes = null;
        //    using (MemoryStream mStream = new MemoryStream())
        //    {
        //        var mPdf = TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerator.GeneratePdf(aHtml, PdfSharp.PageSize.A4);
        //        mPdf.Save(mStream);
        //        mBytes = mStream.ToArray();
        //    }
        //    return mBytes;
        //}
        private void WBGeneral_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (gGenTask != null)
            {
                gGenTask.ActionComplete(true);
                gGenTask = null;
            }
        }
        public void cboReportTemplates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is System.Windows.Controls.ComboBox))
            {
                return;
            }
            System.Windows.Controls.ComboBox cboReportTemplates = sender as System.Windows.Controls.ComboBox;
            if (cboReportTemplates.SelectedItem == null || !(cboReportTemplates.SelectedItem is TemplateFileName))
            {
                return;
            }
            TemplateFileName mSelectedTemplateFileName = cboReportTemplates.SelectedItem as TemplateFileName;
            PatientPCLRequestObj.TemplateFileName = mSelectedTemplateFileName.FullName;
            Coroutine.BeginExecute(ApplyElementValues_Routine(null, null, null, "", null, true));
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
        #endregion
        #region Methods
        //▼====: #005
        public void ckbIsWaitResult_Click(object source, object sender)
        {
            if (PatientPCLRequestObj == null)
            {
                return;
            }
            System.Windows.Controls.CheckBox ckbIsChecked = source as System.Windows.Controls.CheckBox;
            if (!ckbIsChecked.IsChecked.GetValueOrDefault(false))
            {
                PatientPCLRequestObj.IsDone = false;
            }
        }
        //▲====: #005

        public string GetBodyValue(IList<PCLResultFileStorageDetail> imagesSavedInCurrentSession, IList<PCLResultFileStorageDetail> imagesNeedToDelete, out string Description, out string Result)
        {
            Description = null;
            Result = null;
            if (WBGeneral == null) return null;
            System.Windows.Forms.HtmlDocument mDocument = WBGeneral.Document is System.Windows.Forms.HtmlDocument ? WBGeneral.Document as System.Windows.Forms.HtmlDocument : null;
            if (mDocument != null)
            {
                if (WBGeneral.Document.Body == null)
                {
                    return null;
                }
                HtmlElement divdetail = mDocument.GetElementById("divdetail");
                HtmlElement divresult = mDocument.GetElementById("divresult");
                Description = divdetail == null ? null : divdetail.InnerText;
                Result = divresult == null ? null : divresult.InnerText;
                if (Description != null)
                {
                    Description = Description.Replace("\0", "").Trim();
                }
                if (Result != null)
                {
                    Result = Result.Replace("\0", "").Trim();
                }
                CleanupImageValues();
                UpdateImageToHtml(imagesSavedInCurrentSession, imagesNeedToDelete);
                return WBGeneral.Document.Body.Parent.InnerHtml;
            }
            return null;
        }
        private void UpdateImageToHtml(IList<PCLResultFileStorageDetail> imagesSavedInCurrentSession, IList<PCLResultFileStorageDetail> imagesNeedToDelete)
        {

            //20200914 TVN: PCLResultFileStorageDetailCollection biến này đang thao tác trên những hình của session hiện tại (những hình vừa mới chụp). dẫn đến tình trạng: hình ảnh luôn lấy từ lần chụp mới nhất mà không quan tâm đến việc mình đã check R ở hình đã lưu ở lần trước.
            var listImages = new List<PCLResultFileStorageDetail>();
            if (PCLResultFileStorageDetailCollection != null && PCLResultFileStorageDetailCollection.Count > 0)
            {
                listImages.AddRange(PCLResultFileStorageDetailCollection);
            }
            if (imagesSavedInCurrentSession != null && imagesSavedInCurrentSession.Count > 0)
            {
                listImages.AddRange(imagesSavedInCurrentSession);
            }
            if (listImages.Count == 0)
            {
                return;
            }
            //20200916 TVN: đọc danh sách file hình sẽ bị delete để update html cho đúng 
            if (imagesNeedToDelete != null && imagesNeedToDelete.Count > 0)
            {
                foreach (var deleteImage in imagesNeedToDelete)
                {
                    listImages.Remove(deleteImage);
                }
            }


            //if (PCLResultFileStorageDetailCollection == null || PCLResultFileStorageDetailCollection.Count == 0 || string.IsNullOrEmpty(HtmlRaw))
            //{
            //    return;
            //}


            gImageSources = new List<string>();
            if (WBGeneral == null) throw new Exception("Controls unloadable");
            System.Windows.Forms.HtmlDocument mBody = WBGeneral.Document is System.Windows.Forms.HtmlDocument ? WBGeneral.Document as System.Windows.Forms.HtmlDocument : null;
            if (mBody == null) throw new Exception("Controls unloadable");



            var imgListElement = GetImageElementCollection(mBody);
            //imgListElement.RemoveAt(0);

            if (imgListElement.Count > 0)
            {
                var pclImageURL = Globals.ServerConfigSection.Pcls.PCLImageURL;

                foreach (var item in listImages.OrderByDescending(x => x.IsUseForPrinting).Where(x => x.V_ResultType == (long)AllLookupValues.FileStorageResultType.IMAGES))
                {
                    //yield return GenericCoRoutineTask.StartTask(GetVideoAndImage, item);

                    //check if fullpath contain .jpg
                    gImageSources.Add(string.Format("{0}/{1}/{2}", pclImageURL, Globals.GetCurServerDateTime().ToString("yyMMdd"), item.PCLResultFileName));
                }

                for (int i = 0; i <= (imgListElement.Count > gImageSources.Count ? gImageSources.Count - 1 : imgListElement.Count - 1); i++)
                {
                    imgListElement[i].SetAttribute("src", gImageSources[i]);
                }
            }

            //HtmlRaw = htmlDocument.DocumentNode.InnerHtml;
        }

        public void ApplyElementValues(string aValueArray, PatientPCLRequest aPatientPCLRequest, IList<PCLResultFileStorageDetail> aPCLResultFileStorageDetailCollection, string PtRegistrationCode, string StaffFullName, string Suggest)
        {
            PatientPCLRequestObj = aPatientPCLRequest;
            Coroutine.BeginExecute(ApplyElementValues_Routine(aValueArray, aPCLResultFileStorageDetailCollection, PtRegistrationCode, StaffFullName, Suggest));
        }
        private void CleanElementValues(GenericCoRoutineTask aGenTask)
        {
            gGenTask = aGenTask;
            RenewWebControlContent();
            WBGeneral.DocumentText = BackupContentBody;
        }
        private void CleanupImageValues()
        {
            if (string.IsNullOrEmpty(ContentBody)) return;
            if (WBGeneral == null) throw new Exception("Controls unloadable");
            System.Windows.Forms.HtmlDocument mBody = WBGeneral.Document is System.Windows.Forms.HtmlDocument ? WBGeneral.Document as System.Windows.Forms.HtmlDocument : null;
            if (mBody == null) throw new Exception("Controls unloadable");
            List<HtmlElement> ImageElementCollection = GetImageElementCollection(mBody);
            if (ImageElementCollection != null)
            {
                foreach (var item in ImageElementCollection)
                {
                    item.SetAttribute("src", "");
                }
            }
        }
        private IEnumerator<IResult> ApplyElementValues_Routine(string aTemplateResultString, IList<PCLResultFileStorageDetail> aPCLResultFileStorageDetailCollection
            , string PtRegistrationCode, string StaffFullName, string Suggest, bool IsRefresh = false)
        {
            this.ShowBusyIndicator();
            PCLResultFileStorageDetailCollection = aPCLResultFileStorageDetailCollection;
            if (!string.IsNullOrEmpty(aTemplateResultString))
            {
                Coroutine.BeginExecute(UpdateValueToBody(aTemplateResultString, PatientPCLRequestObj, aPCLResultFileStorageDetailCollection, PtRegistrationCode, StaffFullName, Suggest));
                this.HideBusyIndicator();
                yield break;
            }
            if (PatientPCLRequestObj.TemplateFileName != FileName)
            {
                if (IsNotApplyTemplatePCLResultNew)
                {
                    yield return GenericCoRoutineTask.StartTask(GetReportBody, PatientPCLRequestObj.TemplateFileName, IsRefresh, Globals.ServerConfigSection.CommonItems.ReportTemplatesLocation);
                }
                else
                {
                    yield return GenericCoRoutineTask.StartTask(GetReportBody, "", false, "");
                }
            }
            else if (!string.IsNullOrEmpty(ContentBody))
            {
                yield return GenericCoRoutineTask.StartTask(CleanElementValues);
            }
            //if (string.IsNullOrEmpty(aTemplateResultString))
            //{
            //    this.HideBusyIndicator();
            //    yield break;
            //}
            //try
            //{
            //    UpdateValueToBody(aTemplateResultString, PatientPCLRequestObj);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
            //    this.HideBusyIndicator();
            //    yield break;
            //}
            if (aPCLResultFileStorageDetailCollection != null)
            {
                Coroutine.BeginExecute(GetVideoValues_Routine(aPCLResultFileStorageDetailCollection));
            }
            this.HideBusyIndicator();
        }
        private IEnumerator<IResult> UpdateValueToBody(string aTemplateResultString, PatientPCLRequest aPatientPCLRequest, IList<PCLResultFileStorageDetail> aPCLResultFileStorageDetailCollection, string PtRegistrationCode, string StaffFullName, string Suggest)
        {
            if (string.IsNullOrEmpty(aTemplateResultString))
            {
                yield break;
            }
            //▼====: #004
            if (string.IsNullOrEmpty(aTemplateResultString)) yield break;
            if (aTemplateResultString.Equals("about:blank")) yield break;
            if (aTemplateResultString.StartsWith("http://") ||
                aTemplateResultString.StartsWith("https://"))
            {
                try
                {
                    if (WBGeneral == null) throw new Exception("Controls unloadable");
                    WBGeneral.Navigate(new Uri(aTemplateResultString));
                }
                catch (UriFormatException)
                {
                    yield break;
                }
            }
            else
            {
                yield return GenericCoRoutineTask.StartTask(GetReportBody, aTemplateResultString, false, "");
                if (WBGeneral == null) throw new Exception("Controls unloadable");
                System.Windows.Forms.HtmlDocument mBody = WBGeneral.Document is System.Windows.Forms.HtmlDocument ? WBGeneral.Document as System.Windows.Forms.HtmlDocument : null;
                if (mBody == null) throw new Exception("Controls unloadable");

                HtmlElement divRequire = mBody.GetElementById("divRequire");
                HtmlElement lbRequire = mBody.GetElementById("lbRequire");
                if (!string.IsNullOrEmpty(Suggest) && divRequire == null)
                {
                    HtmlElement divresult = mBody.GetElementById("divresult");
                    if (divresult != null)
                    {
                        HtmlElement sTitle = mBody.CreateElement("h7");
                        sTitle.Id = "lbRequire";
                        sTitle.InnerHtml = string.Format("<font>{0}</font>", eHCMSResources.K3159_G1_DNghi.ToUpper());
                        HtmlElement eTitle = divresult.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterEnd, sTitle);
                        HtmlElement sContent = mBody.CreateElement("div");
                        sContent.Id = "divRequire";
                        sContent.SetAttribute("className", "divmain");
                        sContent.InnerText = Suggest;
                        eTitle.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterEnd, sContent);
                        HtmlElement sPadding = mBody.CreateElement("");
                        sPadding.InnerHtml = "<br /><br />";
                        sTitle.InsertAdjacentElement(HtmlElementInsertionOrientation.BeforeBegin, sPadding);
                        //RefreshDocumentText();
                    }
                }
                else if (!string.IsNullOrEmpty(Suggest) && divRequire != null)
                {
                    lbRequire.Style = "";
                    divRequire.Style = "";
                    divRequire.InnerText = Suggest;
                    //RefreshDocumentText();
                }
                else if (string.IsNullOrEmpty(Suggest) && divRequire != null)
                {
                    lbRequire.Style = "display:none;";
                    divRequire.Style = "display:none;";
                    //RefreshDocumentText();
                }

                if (aPatientPCLRequest != null && aPatientPCLRequest.Patient != null)
                {
                    XElement xXmleTemplate = new XElement("root");
                    xXmleTemplate.Add(new XElement("InfPatientFullName", aPatientPCLRequest.Patient.FullName));
                    xXmleTemplate.Add(new XElement("InfPatientAge", aPatientPCLRequest.Patient.AgeString == "0" ? aPatientPCLRequest.Patient.MonthsOld + " Th" : aPatientPCLRequest.Patient.AgeString));
                    xXmleTemplate.Add(new XElement("InfPatientGender", aPatientPCLRequest.Patient.GenderString));
                    xXmleTemplate.Add(new XElement("InfPatientAddress", aPatientPCLRequest.Patient.PatientFullStreetAddress));
                    xXmleTemplate.Add(new XElement("InfDoctorStaffFullName", aPatientPCLRequest.RequestedDoctorName));
                    xXmleTemplate.Add(new XElement("InfLocationName", aPatientPCLRequest.ReqFromDeptLocIDName));
                    xXmleTemplate.Add(new XElement("InfMedicalInstructionDate", aPatientPCLRequest.MedicalInstructionDate.HasValue && aPatientPCLRequest.MedicalInstructionDate.Value > DateTime.MinValue ? aPatientPCLRequest.MedicalInstructionDate.Value.ToString("dd/MM/yyyy HH:mm tt") : ""));
                    xXmleTemplate.Add(new XElement("InfResultDate", aPatientPCLRequest.ResultDate.HasValue && aPatientPCLRequest.ResultDate.Value > DateTime.MinValue ? aPatientPCLRequest.ResultDate.Value.ToString("dd/MM/yyyy HH:mm tt") : ""));
                    xXmleTemplate.Add(new XElement("InfDiagnosisFinal", aPatientPCLRequest.Diagnosis));
                    xXmleTemplate.Add(new XElement("InfPCLRequestNumID", aPatientPCLRequest.PCLRequestNumID));
                    xXmleTemplate.Add(new XElement("InfPatientCode", aPatientPCLRequest.Patient.PatientCode));
                    xXmleTemplate.Add(new XElement("InfRegistrationCode", PtRegistrationCode));
                    xXmleTemplate.Add(new XElement("InfStaffFullName", StaffFullName));
                    xXmleTemplate.Add(new XElement("InfPrintDateD", Globals.GetCurServerDateTime().ToString("dd")));
                    xXmleTemplate.Add(new XElement("InfPrintDateM", Globals.GetCurServerDateTime().ToString("MM")));
                    xXmleTemplate.Add(new XElement("InfPrintDateY", Globals.GetCurServerDateTime().ToString("yyyy")));
                    xXmleTemplate.Add(new XElement("InfPCLExamTypeName", aPatientPCLRequest.PCLExamTypeName));
                    HtmlElement pbBarcodeElement = mBody.GetElementById("pbBarcode");
                    if (pbBarcodeElement != null)
                    {
                        Bitmap OrginalBitmap = CommonGlobals.GeneratorBarcodeImage(aPatientPCLRequest.PCLRequestNumID, CmToPixel(2.5, 0.8));
                        using (var aStream = new MemoryStream())
                        {
                            OrginalBitmap.Save(aStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                            pbBarcodeElement.SetAttribute("src", string.Format("data:image/png;base64, {0}", Convert.ToBase64String(aStream.ToArray())));
                            aStream.Close();
                        }
                    }
                    foreach (XElement eChild in xXmleTemplate.Elements().ToList())
                    {
                        HtmlElement mElement = mBody.GetElementById(eChild.Name.ToString());
                        if (mElement == null) continue;
                        if (!string.IsNullOrEmpty(mElement.TagName) && mElement.TagName.ToLower().Equals("div"))
                        {
                            mElement.InnerHtml = eChild.Value;
                        }
                        else if (!string.IsNullOrEmpty(mElement.TagName) && mElement.TagName.ToLower().Equals("label"))
                        {
                            mElement.InnerText = eChild.Value;
                        }
                        else
                        {
                            mElement.SetAttribute("value", eChild.Value.ToString());
                        }
                    }
                }
                CleanupImageValues();
                if (aPCLResultFileStorageDetailCollection != null)
                {
                    Coroutine.BeginExecute(GetVideoValues_Routine(aPCLResultFileStorageDetailCollection));
                }
                this.HideBusyIndicator();
            }
            //▲====: #004

            //try
            //{
            //    if (string.IsNullOrEmpty(ContentBody))
            //    {
            //        return;
            //    }
            //    aTemplateResultString = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(aTemplateResultString));
            //    XElement xXmleTemplate = XElement.Parse(aTemplateResultString);
            //    if (WBGeneral == null) throw new Exception("Controls unloadable");
            //    HtmlDocument mBody = WBGeneral.Document is HtmlDocument ? WBGeneral.Document as HtmlDocument : null;
            //    if (mBody == null) throw new Exception("Controls unloadable");
            //    if (aPatientPCLRequest != null && aPatientPCLRequest.Patient != null)
            //    {
            //        xXmleTemplate.Add(new XElement("InfPatientFullName", aPatientPCLRequest.Patient.FullName));
            //        xXmleTemplate.Add(new XElement("InfPatientAge", aPatientPCLRequest.Patient.AgeString));
            //        xXmleTemplate.Add(new XElement("InfPatientGender", aPatientPCLRequest.Patient.GenderString));
            //        xXmleTemplate.Add(new XElement("InfPatientAddress", aPatientPCLRequest.Patient.PatientStreetAddress));
            //        xXmleTemplate.Add(new XElement("InfDoctorStaffFullName", aPatientPCLRequest.RequestedDoctorName));
            //        xXmleTemplate.Add(new XElement("InfLocationName", aPatientPCLRequest.ReqFromDeptLocIDName));
            //        xXmleTemplate.Add(new XElement("InfMedicalInstructionDate", aPatientPCLRequest.MedicalInstructionDate.HasValue && aPatientPCLRequest.MedicalInstructionDate.Value > DateTime.MinValue ? aPatientPCLRequest.MedicalInstructionDate.Value.ToString("HH:mm dd/MM/yyyy") : ""));
            //        xXmleTemplate.Add(new XElement("InfResultDate", aPatientPCLRequest.ResultDate.HasValue && aPatientPCLRequest.ResultDate.Value > DateTime.MinValue ? aPatientPCLRequest.ResultDate.Value.ToString("HH:mm dd/MM/yyyy") : ""));
            //        xXmleTemplate.Add(new XElement("InfDiagnosisFinal", aPatientPCLRequest.Diagnosis));
            //        xXmleTemplate.Add(new XElement("InfPCLRequestNumID", aPatientPCLRequest.PCLRequestNumID));
            //        xXmleTemplate.Add(new XElement("InfPatientCode", aPatientPCLRequest.Patient.PatientCode));

            //        HtmlElement pbBarcodeElement = mBody.GetElementById("pbBarcode");
            //        if (pbBarcodeElement != null)
            //        {
            //            Bitmap OrginalBitmap = CommonGlobals.GeneratorBarcodeImage(aPatientPCLRequest.PCLRequestNumID, CmToPixel(6.0, 1.5));
            //            using (var aStream = new MemoryStream())
            //            {
            //                OrginalBitmap.Save(aStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            //                pbBarcodeElement.SetAttribute("src", string.Format("data:image/png;base64, {0}", Convert.ToBase64String(aStream.ToArray())));
            //                aStream.Close();
            //            }
            //        }
            //    }
            //    foreach (XElement eChild in xXmleTemplate.Elements().ToList())
            //    {
            //        HtmlElement mElement = mBody.GetElementById(eChild.Name.ToString());
            //        if (mElement == null) throw new Exception(string.Format("{0} could not be found", eChild.Name.ToString()));
            //        if (!string.IsNullOrEmpty(mElement.TagName) && mElement.TagName.ToLower().Equals("div"))
            //        {
            //            mElement.InnerHtml = eChild.Value;
            //        }
            //        else if (!string.IsNullOrEmpty(mElement.TagName) && mElement.TagName.ToLower().Equals("label"))
            //        {
            //            mElement.InnerText = eChild.Value;
            //        }
            //        else
            //        {
            //            mElement.SetAttribute("value", eChild.Value.ToString());
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }
        private void PrintContent()
        {
            if (WBGeneral == null) return;
            WBGeneral.Print();
        }
        public void ApplyImages(IList<PCLResultFileStorageDetail> PCLResultFileStorageDetailCollection)
        {
            Coroutine.BeginExecute(GetVideoValues_Routine(PCLResultFileStorageDetailCollection));
        }
        private Size CmToPixel(double WidthInCm, double HeightInCm)
        {
            float sngWidth = (float)WidthInCm; //cm
            float sngHeight = (float)HeightInCm; //cm
            using (Bitmap bmp = new Bitmap(1, 1))
            {
                sngWidth *= 0.393700787f * bmp.HorizontalResolution; // x-Axis pixel
                sngHeight *= 0.393700787f * bmp.VerticalResolution; // y-Axis pixel
            }
            return new Size((int)sngWidth, (int)sngHeight);
        }
        private void GetVideoAndImage(GenericCoRoutineTask aGenTask, object aFileStorageDetail)
        {
            if (!(aFileStorageDetail is PCLResultFileStorageDetail))
            {
                aGenTask.ActionComplete(false);
            }
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetVideoAndImage((aFileStorageDetail as PCLResultFileStorageDetail).FullPath, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var mImageArray = contract.EndGetVideoAndImage(asyncResult);
                            if (mImageArray != null && mImageArray.Length > 0)
                            {
                                Stream mStream = new MemoryStream(mImageArray);
                                System.Drawing.Image OrginalBitmap = Bitmap.FromStream(mStream);
                                mStream.Close();
                                Bitmap mBitmap = new Bitmap(OrginalBitmap);
                                //20190122 TNHX: Add condition for skip resize with image from Laboratory
                                if (PatientPCLRequestObj.V_PCLMainCategory != (long)AllLookupValues.V_PCLMainCategory.Laboratory)
                                {
                                    mBitmap = new Bitmap(OrginalBitmap, CmToPixel(10.0, Math.Ceiling(OrginalBitmap.Height * 10.0 / OrginalBitmap.Width)));
                                }
                                using (var aStream = new MemoryStream())
                                {
                                    mBitmap.Save(aStream, OrginalBitmap.RawFormat);
                                    gImageSources.Add(Convert.ToBase64String(aStream.ToArray()));
                                    aStream.Close();
                                }
                                aGenTask.ActionComplete(true);
                            }
                            else
                            {
                                aGenTask.ActionComplete(true);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                            aGenTask.ActionComplete(false);
                        }
                    }), null);
                }
            });
            t.Start();
        }
        private List<HtmlElement> GetImageElementCollection(System.Windows.Forms.HtmlDocument mBody, bool IsShowAlso = false)
        {
            if (mBody == null)
            {
                return new List<HtmlElement>();
            }
            List<HtmlElement> ImageElementCollection = new List<HtmlElement>();
            List<HtmlElement> ImagesTableCollection = mBody.GetElementsByTagName("table").Cast<HtmlElement>().Where(x => x.GetAttribute("className") == "ImagesTable").ToList();
            if (ImagesTableCollection == null)
            {
                ImagesTableCollection = new List<HtmlElement>();
            }
            HtmlElement IDImagesTable = mBody.GetElementById("ImagesTable");
            if (IDImagesTable != null)
            {
                ImagesTableCollection.Add(IDImagesTable);
            }
            if (ImagesTableCollection != null)
            {
                foreach (var eImagesTable in ImagesTableCollection)
                {
                    List<HtmlElement> ImageElements = eImagesTable.GetElementsByTagName("img").Cast<HtmlElement>().ToList();
                    if (ImageElements != null || ImageElements.Count > 0)
                    {
                        ImageElementCollection.AddRange(ImageElements);
                        if (IsShowAlso)
                        {
                            eImagesTable.Style = "display:inline-block;";
                        }
                    }
                }
            }
            return ImageElementCollection;
        }
        private IEnumerator<IResult> GetVideoValues_Routine(IList<PCLResultFileStorageDetail> PCLResultFileStorageDetailCollection)
        {
            if (PCLResultFileStorageDetailCollection == null || PCLResultFileStorageDetailCollection.Count == 0 || string.IsNullOrEmpty(ContentBody))
            {
                yield break;
            }
            gImageSources = new List<string>();
            if (WBGeneral == null) throw new Exception("Controls unloadable");
            System.Windows.Forms.HtmlDocument mBody = WBGeneral.Document is System.Windows.Forms.HtmlDocument ? WBGeneral.Document as System.Windows.Forms.HtmlDocument : null;
            if (mBody == null) throw new Exception("Controls unloadable");
            var ImageElementCollection = GetImageElementCollection(mBody, true);
            if (ImageElementCollection != null && ImageElementCollection.Count > 0)
            {
                var pclImageURL = Globals.ServerConfigSection.Pcls.PCLImageURL;

                foreach (var item in PCLResultFileStorageDetailCollection.OrderByDescending(x => x.IsUseForPrinting).Where(x => x.V_ResultType == (long)AllLookupValues.FileStorageResultType.IMAGES))
                {
                    //yield return GenericCoRoutineTask.StartTask(GetVideoAndImage, item);

                    //split the path into many part and get only the last 2 item for usage
                    var listPathItem = item.FullPath.Split('\\');
                    if (listPathItem.Last().Length > 0 && listPathItem.Count() > 1)
                    {
                        //check if fullpath contain .jpg
                        if (!listPathItem[listPathItem.Length - 1].Contains(".jpg"))
                        {
                            gImageSources.Add(string.Format("{0}/{1}/{2}", pclImageURL, listPathItem[listPathItem.Length - 2], listPathItem[listPathItem.Length - 1] + ".jpg"));

                        }
                        else
                        {
                            gImageSources.Add(string.Format("{0}/{1}/{2}", pclImageURL, listPathItem[listPathItem.Length - 2], listPathItem[listPathItem.Length - 1]));
                        }
                    }


                }
                for (int i = 0; i <= (ImageElementCollection.Count > gImageSources.Count ? gImageSources.Count - 1 : ImageElementCollection.Count - 1); i++)
                {
                    ImageElementCollection[i].SetAttribute("src", gImageSources[i]);

                    //ImageElementCollection[i].SetAttribute("src", string.Format("data:image/png;base64, {0}", gImageSources[i]));
                }
            }
            string NewBody = WBGeneral.Document.Body.Parent.InnerHtml;
            RenewWebControlContent();
            WBGeneral.DocumentText = NewBody;
            //byte[] mPdf = PdfSharpConvert(WBGeneral.DocumentText);
            //var mPrintEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, mPdf, ActiveXPrintType.ByteArray);
            //Globals.EventAggregator.Publish(mPrintEvt);
        }
        private void GetReportBody(GenericCoRoutineTask aGenTask, object aReportFilePath, object IsRefresh, object ReportTemplatesLocation)
        {
            try
            {
                ContentBody = null;
                BackupContentBody = null;
                FileName = null;
                if (!Convert.ToBoolean(IsRefresh))
                {
                    TemplateFileNameCollection = new ObservableCollection<TemplateFileName>();
                }
                if (aReportFilePath == null)
                {
                    RenewWebControlContent();
                    aGenTask.ActionComplete(true);
                    return;
                }
                FileName = aReportFilePath.ToString();
                if (FileName.Contains(";") && !Convert.ToBoolean(IsRefresh))
                {
                    RenewWebControlContent();
                    string[] FileNameCollection = FileName.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in FileNameCollection.Where(x => !string.IsNullOrEmpty(x)))
                    {
                        TemplateFileNameCollection.Add(new TemplateFileName { FileName = item.Replace(".html", ""), FullName = item });
                    }
                    NotifyOfPropertyChange(() => IsMultiFileName);
                    aGenTask.ActionComplete(true);
                    return;
                }
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetVideoAndImage(Path.Combine(ReportTemplatesLocation.ToString(), aReportFilePath.ToString()), Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var mFileArray = contract.EndGetVideoAndImage(asyncResult);
                                if (mFileArray == null || mFileArray.Length == 0)
                                {
                                    RenewWebControlContent();
                                    aGenTask.ActionComplete(true);
                                    return;
                                }
                                MemoryStream mMemStream = new MemoryStream(mFileArray);
                                StreamReader mReader = new StreamReader(mMemStream);
                                string mContentBody = mReader.ReadToEnd();
                                mContentBody = Globals.ReplaceStylesHref(mContentBody);
                                ContentBody = mContentBody;
                                BackupContentBody = ContentBody;
                                ChangeContentBodyToWorkWithTemplateHasBase64Image();
                                mMemStream.Close();
                                mReader.Close();
                                if (WBGeneral == null)
                                {
                                    aGenTask.ActionComplete(true);
                                    return;
                                }
                                RenewWebControlContent();
                                gGenTask = aGenTask;
                                WBGeneral.DocumentText = ContentBody;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                                RenewWebControlContent();
                                aGenTask.ActionComplete(true);
                            }
                        }), null);
                    }
                });
                t.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                aGenTask.ActionComplete(true);
            }
        }
        //TVN replace old html has base 64 image to url image

        private void ChangeContentBodyToWorkWithTemplateHasBase64Image()
        {
            HtmlNodeCollection imgCollections = null;
            var htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(ContentBody);
            if (ContentBody.Contains("data:image/png;base64"))
            {
                imgCollections = htmlDocument.DocumentNode.SelectNodes("//td/img");
            }

            if (imgCollections != null && imgCollections.Count > 0 && PCLResultFileStorageDetailCollection != null)
            {
                int i = 1;
                foreach (var item in PCLResultFileStorageDetailCollection.OrderByDescending(x => x.IsUseForPrinting).Where(x => x.V_ResultType == (long)AllLookupValues.FileStorageResultType.IMAGES))
                {
                    if (i < imgCollections.Count)
                    {
                        var listPath = item.FullPath.Split('\\');
                        imgCollections[i++].SetAttributeValue("src", string.Format("{0}\\{1}\\{2}", Globals.ServerConfigSection.Pcls.PCLImageURL, listPath[listPath.Length - 2], listPath[listPath.Length - 1].Contains(".jpg") ? listPath[listPath.Length - 1] : listPath[listPath.Length - 1] + ".jpg"));
                        //imgCollections[i++].SetAttributeValue("src", item.FullPath.Contains(".jpg") ? item.FullPath : item.FullPath + ".jpg");

                    }

                }
            }
            ContentBody = htmlDocument.DocumentNode.InnerHtml;

        }
        private void RenewWebControlContent()
        {
            WBGeneral.Navigate("about:blank");
            if (WBGeneral.Document != null)
            {
                WBGeneral.Document.Write(string.Empty);
            }
        }
        private void RefreshDocumentText()
        {
            ContentBody = WBGeneral.Document.Body.Parent.InnerHtml;
            BackupContentBody = ContentBody;
            RenewWebControlContent();
            WBGeneral.DocumentText = ContentBody;
        }
        public void CallCatureEvent()
        {
            Globals.EventAggregator.Publish(new CallCaptureEvent());
        }
        #endregion
        //▼====== #001
        #region Danh sách máy và chọn bác sĩ thực hiện
        private IAucHoldConsultDoctor _aucHoldConsultDoctor;
        public IAucHoldConsultDoctor aucHoldConsultDoctor
        {
            get
            {
                return _aucHoldConsultDoctor;
            }
            set
            {
                if (_aucHoldConsultDoctor != value)
                {
                    _aucHoldConsultDoctor = value;
                    NotifyOfPropertyChange(() => aucHoldConsultDoctor);
                }
            }
        }

        private IAucHoldConsultDoctor _aucDoctorResult;
        public IAucHoldConsultDoctor aucDoctorResult
        {
            get
            {
                return _aucDoctorResult;
            }
            set
            {
                if (_aucDoctorResult != value)
                {
                    _aucDoctorResult = value;
                    NotifyOfPropertyChange(() => aucDoctorResult);
                }
            }
        }

        private PatientPCLImagingResult _ObjPatientPCLImagingResult_General;
        public PatientPCLImagingResult ObjPatientPCLImagingResult_General
        {
            get { return _ObjPatientPCLImagingResult_General; }
            set
            {
                if (_ObjPatientPCLImagingResult_General != value)
                {
                    _ObjPatientPCLImagingResult_General = value;
                    NotifyOfPropertyChange(() => ObjPatientPCLImagingResult_General);
                }
            }
        }

        private ObservableCollection<Resources> _HIRepResourceCode;
        public ObservableCollection<Resources> HIRepResourceCode
        {
            get { return _HIRepResourceCode; }
            set
            {
                if (HIRepResourceCode != value)
                {
                    _HIRepResourceCode = value;
                }
                NotifyOfPropertyChange(() => HIRepResourceCode);
            }
        }

        public IList<PCLResultFileStorageDetail> PCLResultFileStorageDetailCollection { get; private set; }

        private void GetResourcesForMedicalServicesListByTypeID(long PCLResultParamImpID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetResourcesForMedicalServicesListByTypeID(PCLResultParamImpID
                        , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                List<Resources> results = contract.EndGetResourcesForMedicalServicesListByTypeID(asyncResult);
                                if (results != null)
                                {
                                    if (HIRepResourceCode == null)
                                    {
                                        HIRepResourceCode = new ObservableCollection<Resources>();
                                    }
                                    else
                                    {
                                        HIRepResourceCode.Clear();
                                    }
                                    HIRepResourceCode = results.ToObservableCollection();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            finally
                            {
                                Globals.IsBusy = false;
                            }
                        }), null);
                }
            });

            t.Start();
        }

        AxComboBox Cbo;
        public void cboHiRepResourceCode_Loaded(object sender, SelectionChangedEventArgs e)
        {
            Cbo = sender as AxComboBox;
        }
        public void cboHiRepResourceCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Cbo.SelectedValue != null && !string.IsNullOrEmpty(Cbo.SelectedValue.ToString()))
            {
                ObjPatientPCLImagingResult_General.HIRepResourceCode = Cbo.SelectedValue.ToString();
            }
        }

        #endregion
        //▲====== #001
        #region Handles
        public void Handle(PrintEventActionView message)
        {
            if (message != null)
            {
                if (message.IsDirectPrint)
                {
                    WBGeneral.ShowPrintDialog();
                }
                else
                {
                    WBGeneral.ShowPrintPreviewDialog();
                }
            }
        }
        #endregion
        //▼====== #002
        public void RenewWebContent()
        {
            RenewWebControlContent();
            HIRepResourceCode = new ObservableCollection<Resources>();
            ObjPatientPCLImagingResult_General = new PatientPCLImagingResult();
            aucDoctorResult.setDefault();
            aucHoldConsultDoctor.setDefault();
        }
        //▲======= #002
       
        private bool _IsNotApplyTemplatePCLResultNew = !Globals.ServerConfigSection.CommonItems.ApplyTemplatePCLResultNew;
        public bool IsNotApplyTemplatePCLResultNew
        {
            get { return _IsNotApplyTemplatePCLResultNew; }
            set
            {
                if (_IsNotApplyTemplatePCLResultNew != value)
                {
                    _IsNotApplyTemplatePCLResultNew = value;
                }
                NotifyOfPropertyChange(() => IsNotApplyTemplatePCLResultNew);
            }
        }
    }
    public class TemplateFileName : NotifyChangedBase
    {
        private string _FileName;
        private string _FullName;
        public string FileName
        {
            get => _FileName; set
            {
                _FileName = value;
                RaisePropertyChanged("FileName");
            }
        }
        public string FullName
        {
            get => _FullName; set
            {
                _FullName = value;
                RaisePropertyChanged("FullName");
            }
        }
    }
}
