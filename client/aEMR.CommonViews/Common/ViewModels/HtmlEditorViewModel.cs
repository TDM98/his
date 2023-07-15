using aEMR.Common.BaseModel;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using DataEntities;
using eHCMSLanguage;
using mshtml;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
/*
 * 20200206 #001 TTM: BM 0022883: Fix lỗi phải bấm nút clear mới gõ tường trình phẫu thuật được.
 */
namespace aEMR.CommonViews.ViewModels
{
    [Export(typeof(IHtmlEditor)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class HtmlEditorViewModel : ViewModelBase, IHtmlEditor
    {
        #region Properties
        public string BodyContent
        {
            get
            {
                return CurrentDocument == null || CurrentDocument.body == null ? null : CurrentDocument.body.innerHTML;
            }
        }
        public string BodyContentText
        {
            get
            {
                return CurrentDocument == null || CurrentDocument.body == null ? null : CurrentDocument.body.innerText;
            }
        }
        private WebBrowser WbMain;
        private HTMLDocument CurrentDocument
        {
            get
            {
                return WbMain == null ? null : WbMain.Document as HTMLDocument;
            }
        }
        private ObservableCollection<SurgeryTemplateContent> _TemplateCollection;
        public ObservableCollection<SurgeryTemplateContent> TemplateCollection
        {
            get
            {
                return _TemplateCollection;
            }
            set
            {
                if (_TemplateCollection == value)
                {
                    return;
                }
                _TemplateCollection = value;
                NotifyOfPropertyChange(() => TemplateCollection);
            }
        }
        private IRegistration_DataStorage _Registration_DataStorage;
        public IRegistration_DataStorage Registration_DataStorage
        {
            get
            {
                return _Registration_DataStorage;
            }
            set
            {
                if (_Registration_DataStorage == value)
                {
                    return;
                }
                _Registration_DataStorage = value;
                NotifyOfPropertyChange(() => Registration_DataStorage);
            }
        }
        private string BaseSectionBodyContent { get; set; }
        #endregion
        #region Events
        public HtmlEditorViewModel()
        {
            LoadAllTemplates();
        }
        public void WbMain_Loaded(object sender)
        {
            WbMain = sender as WebBrowser;
            WbMain.LoadCompleted += WbMain_LoadCompleted;
            //▼===== #001
            if (CurrentDocument == null)
            {
                LoadBaseSection(null);
            }
            //▲===== #001
        }
        private void WbMain_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (CurrentDocument == null)
            {
                return;
            }
            CurrentDocument.designMode = "On";
            //CurrentDocument.execCommand("defaultParagraphSeparator", false, "br");
        }
        public void btnBold()
        {
            if (CurrentDocument == null)
            {
                return;
            }
            CurrentDocument.execCommand("Bold", false, null);
        }
        public void btnItalic()
        {
            if (CurrentDocument == null)
            {
                return;
            }
            CurrentDocument.execCommand("Italic", false, null);
        }
        public void btnUnderline()
        {
            if (CurrentDocument == null)
            {
                return;
            }
            CurrentDocument.execCommand("Underline", false, null);
        }
        public void btnJustifyLeft()
        {
            if (CurrentDocument == null)
            {
                return;
            }
            CurrentDocument.execCommand("JustifyLeft", false, null);
        }
        public void btnJustifyCenter()
        {
            if (CurrentDocument == null)
            {
                return;
            }
            CurrentDocument.execCommand("JustifyCenter", false, null);
        }
        public void btnJustifyRight()
        {
            if (CurrentDocument == null)
            {
                return;
            }
            CurrentDocument.execCommand("JustifyRight", false, null);
        }
        public void btnJustifyFull()
        {
            if (CurrentDocument == null)
            {
                return;
            }
            CurrentDocument.execCommand("JustifyFull", false, null);
        }
        public void btnOrderedList()
        {
            if (CurrentDocument == null)
            {
                return;
            }
            CurrentDocument.execCommand("InsertOrderedList", false, null);
        }
        public void btnBulletList()
        {
            if (CurrentDocument == null)
            {
                return;
            }
            CurrentDocument.execCommand("InsertUnorderedList", false, null);
        }
        public void btnOutIdent()
        {
            if (CurrentDocument == null)
            {
                return;
            }
            CurrentDocument.execCommand("Outdent", false, null);
        }
        public void btnIdent()
        {
            if (CurrentDocument == null)
            {
                return;
            }
            CurrentDocument.execCommand("Indent", false, null);
        }
        public void btnRemoveFormat()
        {
            if (CurrentDocument == null)
            {
                return;
            }
            CurrentDocument.execCommand("removeFormat", false, null);
        }
        public void btnTextColor()
        {
            if (CurrentDocument == null)
            {
                return;
            }
            Color PickedColor;
            if (CommonGlobals.TryPickColor(out PickedColor))
            {
                CurrentDocument.execCommand("ForeColor", false, ColorToHexString(PickedColor));
            }
        }
        public void cboFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentDocument == null)
            {
                return;
            }
            CurrentDocument.execCommand("FontSize", false, (sender as ComboBox).SelectedIndex + 1);
        }
        public void cboTemplate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentDocument == null)
            {
                return;
            }
            var CurrentItem = ((sender as ComboBox).SelectedItem as SurgeryTemplateContent);
            if (CurrentItem == null)
            {
                return;
            }
            LoadTemplateContent(CurrentItem);
        }
        public void WbMain_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(CurrentDocument.selection);
            //if (!(Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) && e.Key == System.Windows.Input.Key.Enter)
            //{
            //    if (CurrentDocument != null)
            //    {
            //        CurrentDocument.execCommand("Paste", false, "\n");
            //    }
            //    e.Handled = true;
            //    return;
            //}
            //else if ((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) && e.Key == System.Windows.Input.Key.Enter)
            //{
            //    if (CurrentDocument != null)
            //    {
            //        CurrentDocument.execCommand("InsertParagraph", false, null);
            //    }
            //    e.Handled = true;
            //    return;
            //}
        }
        public void btnSave()
        {
            TryClose();
        }
        public void btnInsertImage()
        {
            if (CurrentDocument == null)
            {
                return;
            }
            try
            {
                var ImageLinkCollecton = GlobalsNAV.GetWebContent(Path.Combine(Globals.ServerConfigSection.CommonItems.ServerPublicAddress, "Pictures/Surgeries/FileMapdicts.txt"));
                if (ImageLinkCollecton == null)
                {
                    return;
                }
                var ImageFilePaths = ImageLinkCollecton.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Path.Combine(Globals.ServerConfigSection.CommonItems.ServerPublicAddress, string.Format("Pictures/Surgeries/{0}", x))).ToList();
                if (ImageFilePaths == null || ImageFilePaths.Count == 0)
                {
                    return;
                }
                IHtmlImageViewer aView = Globals.GetViewModel<IHtmlImageViewer>();
                aView.FileCollection = new System.Collections.ObjectModel.ObservableCollection<string>(ImageFilePaths);
                GlobalsNAV.ShowDialog_V3(aView);
                if (string.IsNullOrEmpty(aView.SelectedFilePath))
                {
                    return;
                }
                dynamic CurrentRange = CurrentDocument.selection.createRange();
                CurrentRange.pasteHTML(string.Format(@"<img alt=""{1}"" src=""{0}"">", aView.SelectedFilePath, null));
            }
            catch (Exception ex)
            {
                GlobalsNAV.ShowMessagePopup(ex.Message);
            }
        }
        public void btnClear()
        {
            LoadBaseSection(null);
        }
        public void btnGetPCLDesc()
        {
            IPCLResultSummary DialogView = Globals.GetViewModel<IPCLResultSummary>();
            DialogView.InitPatientInfo(Registration_DataStorage);
            DialogView.IsHtmlContent = true;
            GlobalsNAV.ShowDialog_V3(DialogView, null, null, false, true, Globals.GetDefaultDialogViewSize());
            if (DialogView.IsHasDescription && !string.IsNullOrEmpty(DialogView.CurrentDescription))
            {
                if (CurrentDocument == null)
                {
                    return;
                }
                dynamic CurrentRange = CurrentDocument.selection.createRange();
                try
                {
                    CurrentRange.pasteHTML(DialogView.CurrentDescription);
                }
                catch (Exception ex)
                {
                    GlobalsNAV.ShowMessagePopup(ex.Message);
                }
            }
        }
        #endregion
        #region Methods
        private string ColorToHexString(Color aColor)
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}", aColor.R, aColor.G, aColor.B);
        }
        public void LoadBaseSection(string BodyContent)
        {
            if (CurrentDocument != null)
            {
                CurrentDocument.execCommand("Refresh", false, "");
                CurrentDocument.designMode = "Off";
            }
            var CurrentThread = new Thread(() =>
            {
                this.ShowBusyIndicator();
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetVideoAndImage(Path.Combine(Globals.ServerConfigSection.CommonItems.ReportTemplatesLocation, "BaseSection.html"), Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var mFileArray = contract.EndGetVideoAndImage(asyncResult);
                            if (mFileArray == null || mFileArray.Length == 0)
                            {
                                WbMain.NavigateToString("<HTML>");
                                return;
                            }
                            MemoryStream mMemStream = new MemoryStream(mFileArray);
                            StreamReader mReader = new StreamReader(mMemStream);
                            string mContentBody = mReader.ReadToEnd();
                            mContentBody = Globals.ReplaceStylesHref(mContentBody);
                            mContentBody = Regex.Replace(mContentBody, "<body>(.|\n)*?</body>", string.Format("<body>{0}</body>", BodyContent));
                            if (CurrentDocument != null)
                            {
                                CurrentDocument.execCommand("Refresh", false, "");
                                CurrentDocument.designMode = "Off";
                            }
                            BaseSectionBodyContent = mContentBody;
                            WbMain.NavigateToString(mContentBody);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            CurrentThread.Start();
        }
        private void LoadAllTemplates()
        {
            this.ShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetVideoAndImage(Path.Combine(Globals.ServerConfigSection.CommonItems.ReportTemplatesLocation, "TT_SectionFileMapDicts.txt"), Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var mFileArray = contract.EndGetVideoAndImage(asyncResult);
                            if (mFileArray != null && mFileArray.Length > 0)
                            {
                                Stream mFileStream = new MemoryStream(mFileArray);
                                StreamReader mStreamReader = new StreamReader(mFileStream);
                                string FileContent = mStreamReader.ReadToEnd();
                                mFileStream.Close();
                                mFileStream.Dispose();
                                mStreamReader.Close();
                                mStreamReader.Dispose();
                                if (!string.IsNullOrEmpty(FileContent))
                                {
                                    if (TemplateCollection == null)
                                    {
                                        TemplateCollection = new ObservableCollection<SurgeryTemplateContent>();
                                    }
                                    var FileCollection = FileContent.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                                    foreach(var aFile in FileCollection)
                                    {
                                        TemplateCollection.Add(new SurgeryTemplateContent { FileTitle = aFile.Replace("TT_Section[", "").Replace("].html", ""), FileName = aFile });
                                    }
                                }
                            }
                            this.HideBusyIndicator();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            CurrentThread.Start();
        }
        private void LoadTemplateContent(SurgeryTemplateContent CurrentTemplate)
        {
            if (!string.IsNullOrEmpty(CurrentTemplate.FileContent))
            {
                var CurrentBodyHtml = CurrentDocument.documentElement.outerHTML;
                var SeparateContent = "";
                if (CurrentDocument.body == null)
                {
                    CurrentBodyHtml = BaseSectionBodyContent;
                }
                else
                {
                    SeparateContent = string.IsNullOrEmpty(CurrentDocument.body.innerText) ? "" : "<br />";
                }
                CurrentBodyHtml = CurrentBodyHtml.Replace("</body>", string.Format("{1}{0}</body>", CurrentTemplate.FileContent, SeparateContent));
                CurrentDocument.execCommand("Refresh", false, "");
                CurrentDocument.designMode = "Off";
                WbMain.NavigateToString(CurrentBodyHtml);
                return;
            }
            this.ShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetVideoAndImage(Path.Combine(Globals.ServerConfigSection.CommonItems.ReportTemplatesLocation, CurrentTemplate.FileName), Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var mFileArray = contract.EndGetVideoAndImage(asyncResult);
                            if (mFileArray != null && mFileArray.Length > 0)
                            {
                                Stream mFileStream = new MemoryStream(mFileArray);
                                StreamReader mStreamReader = new StreamReader(mFileStream);
                                string FileContent = mStreamReader.ReadToEnd();
                                mFileStream.Close();
                                mFileStream.Dispose();
                                mStreamReader.Close();
                                mStreamReader.Dispose();
                                if (!string.IsNullOrEmpty(FileContent))
                                {
                                    CurrentTemplate.FileContent = FileContent;
                                    var CurrentBodyHtml = CurrentDocument.documentElement.outerHTML;
                                    var SeparateContent = "";
                                    if (CurrentDocument.body == null)
                                    {
                                        CurrentBodyHtml = BaseSectionBodyContent;
                                    }
                                    else
                                    {
                                        SeparateContent = string.IsNullOrEmpty(CurrentDocument.body.innerText) ? "" : "<br />";
                                    }
                                    CurrentBodyHtml = CurrentBodyHtml.Replace("</body>", string.Format("{1}{0}</body>", CurrentTemplate.FileContent, SeparateContent));
                                    CurrentDocument.execCommand("Refresh", false, "");
                                    CurrentDocument.designMode = "Off";
                                    WbMain.NavigateToString(CurrentBodyHtml);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            CurrentThread.Start();
        }
        #endregion
    }
    public class SurgeryTemplateContent
    {
        public string FileTitle { get; set; }
        public string FileName { get; set; }
        public string FileContent { get; set; }
    }
}