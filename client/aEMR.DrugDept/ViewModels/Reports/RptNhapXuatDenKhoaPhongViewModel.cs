using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml.Linq;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using SilverlightTable;
using aEMR.Common;
using Microsoft.Win32;
using aEMR.Common.BaseModel;
using System.Collections.ObjectModel;

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IRptNhapXuatDenKhoaPhong)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RptNhapXuatDenKhoaPhongViewModel : ViewModelBase, IRptNhapXuatDenKhoaPhong
        , IHandle<SelectedObjectWithKey<Object, Object, Object, Object, Object, Object, Object, Object, Object, Object>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        public string TitleForm { get; set; }
        public string TieuDeRpt { get; set; }
        private long _V_MedProductType;
        public long V_MedProductType
        {
            get
            {
                return _V_MedProductType;
            }
            set
            {
                _V_MedProductType = value;
                NotifyOfPropertyChange(() => V_MedProductType);
                if (UCCriteriaB != null)
                {
                    if (_V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                    {
                        UCCriteriaB.HienThi1 = eHCMSResources.Z0949_G1_ChiHienKhoaCoNhanThuoc;
                        UCCriteriaB.HienThi2 = string.Format("{0} ", eHCMSResources.Z0950_G1_ChiHienThuocCoXuat);
                    }
                    else if (_V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                    {
                        UCCriteriaB.HienThi1 = eHCMSResources.Z0951_G1_ChiHienKhoaCoNhanYCu;
                        UCCriteriaB.HienThi2 = string.Format("{0} ", eHCMSResources.Z0952_G1_ChiHienYCuCoXuat);
                    }
                    else
                    {
                        UCCriteriaB.HienThi1 = eHCMSResources.Z0953_G1_ChiHienKhoaCoNhanHChat;
                        UCCriteriaB.HienThi2 = string.Format("{0} ", eHCMSResources.Z0954_G1_ChiHienHChatCoXuat);
                    }
                }
            }
        }

        [ImportingConstructor]
        public RptNhapXuatDenKhoaPhongViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            //Globals.EventAggregator.Subscribe(this);

            var ucDK = Globals.GetViewModel<ICriteriaB>();
            UCCriteriaB = ucDK;
            UCCriteriaB.TextbtViewPrint = eHCMSResources.G2386_G1_Xem;
            this.ActivateItem(ucDK);
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            UCCriteriaB.V_MedProductType = V_MedProductType;
            UCCriteriaB.GetStore();
        }

        public ICriteriaB UCCriteriaB
        {
            get;
            set;
        }

        private bool _IsLoading = false;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set
            {
                if (_IsLoading != value)
                {
                    _IsLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }
        private DataGrid _dataGrid;
        public void dtgList_Loaded(object sender, RoutedEventArgs e)
        {
            _dataGrid = sender as DataGrid;
        }

        private String strXMLResult = "";

        private bool _mIn = true;
        private bool _mXuatExcel = true;

        public bool mIn
        {
            get
            {
                return _mIn;
            }
            set
            {
                if (_mIn == value)
                    return;
                _mIn = value;
                NotifyOfPropertyChange(() => mIn);
            }
        }
        public bool mXuatExcel
        {
            get
            {
                return _mXuatExcel;
            }
            set
            {
                if (_mXuatExcel == value)
                    return;
                _mXuatExcel = value;
                NotifyOfPropertyChange(() => mXuatExcel);
            }
        }

        //20200619 TBL: BM 0039281: aEMR không sử dụng được SortableCollectionView nên đổi qua ObservableCollection
        //SortableCollectionView data = new SortableCollectionView();
        ObservableCollection<Row> data = new ObservableCollection<Row>();

        private void RptNhapXuatDenKhoaPhong(long V_MedProductType, DateTime FromDate, DateTime ToDate, long StoreID, long? StoreClinicID, bool? IsShowHave, bool? IsShowHaveMedProduct)
        {
            data.Clear();
            _dataGrid.Columns.Clear();
            IsLoading = true;
            this.DlgShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ReportServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;

                        client.BeginRptNhapXuatDenKhoaPhong(V_MedProductType, FromDate, ToDate, StoreID, StoreClinicID, IsShowHave, IsShowHaveMedProduct, Globals.DispatchCallback((asyncResult) =>
                           {
                               try
                               {
                                   strXMLResult = client.EndRptNhapXuatDenKhoaPhong(asyncResult);

                                // grab the xml into a XDocument
                                XDocument xmlDoc = XDocument.Parse(strXMLResult);

                                // find the columns
                                List<string> columnNames = xmlDoc.Descendants("column")
                                                                    .Attributes("name").Select(a => a.Value).ToList();


                                // add them to the grid
                                foreach (string columnName in columnNames)
                                   {
                                       _dataGrid.Columns.Add(CreateColumn(columnName));
                                   }

                                // add the rows
                                var rows = xmlDoc.Descendants("row");
                                   foreach (var row in rows)
                                   {
                                       Row rowData = new Row();
                                       int index = 0;
                                       var cells = row.Descendants("cell");
                                       foreach (var cell in cells)
                                       {
                                           rowData[columnNames[index]] = cell.Value;
                                           index++;
                                       }
                                       data.Add(rowData);
                                   }

                                   _dataGrid.ItemsSource = data;

                               }
                               catch (FaultException<AxException> fault)
                               {
                                   ClientLoggerHelper.LogInfo(fault.ToString());
                                   IsLoading = false;

                               }
                               catch (Exception ex)
                               {
                                   ClientLoggerHelper.LogInfo(ex.ToString());
                                   IsLoading = false;
                                   _logger.Error(ex.Message);
                               }
                               finally
                               {
                                   this.DlgHideBusyIndicator();
                               }
                           }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    _logger.Error(ex.Message);
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }
        private RowIndexConverter _rowIndexConverter = new RowIndexConverter();
        private DataGridColumn CreateColumn(string property)
        {
            return new DataGridTextColumn()
            {
                CanUserSort = true,
                Header = property,
                SortMemberPath = property,
                IsReadOnly = false,
                Binding = new Binding("Data")
                {
                    Converter = _rowIndexConverter,
                    ConverterParameter = property
                }
            };
        }


        public void Handle(SelectedObjectWithKey<object, object, object, object, object, object, object, object, object, object> message)
        {
            string strTieuDe = "";
            DateTime? FromDate = null;
            DateTime? ToDate = null;
            int? Thang = 0;
            int? Nam = 0;

            if (message != null)
            {
                if (this.GetView() != null)
                {
                    long StoreID = (message.ObjF as RefStorageWarehouseLocation).StoreID;
                    long? StoreClinicID = (message.ObjG as RefStorageWarehouseLocation) == null ? 0 : (message.ObjG as RefStorageWarehouseLocation).StoreID;
                    bool? IsShowHave = message.ObjH as bool?;
                    bool? IsShowHaveMedProduct = message.ObjK as bool?;
                    switch (message.ObjKey.ToString())
                    {
                        case "1":
                            {
                                FromDate = Convert.ToDateTime(message.ObjA);
                                ToDate = Convert.ToDateTime(message.ObjB);

                                strTieuDe = TieuDeRpt + " (" + Convert.ToDateTime(message.ObjA).ToString("dd/MM/yyyy") + " - " + Convert.ToDateTime(message.ObjB).ToString("dd/MM/yyyy") + ")";

                                break;
                            }
                        case "2":
                            {
                                Thang = Convert.ToInt32(message.ObjD);
                                Nam = Convert.ToInt32(message.ObjE);

                                DateTime Dtmp = new DateTime(Nam.Value, Thang.Value, 01);

                                FromDate = Dtmp;
                                ToDate = Dtmp.AddMonths(1).AddDays(-1);

                                strTieuDe = TieuDeRpt + string.Format(" {0} ", eHCMSResources.G0039_G1_Th) + message.ObjD.ToString() + string.Format(" {0} ", eHCMSResources.N0033_G1_Nam.ToLower()) + message.ObjE.ToString();

                                break;
                            }
                    }

                    RptNhapXuatDenKhoaPhong(V_MedProductType, FromDate.Value, ToDate.Value, StoreID, StoreClinicID, IsShowHave, IsShowHaveMedProduct);

                    //ReportModel = null;
                    //ReportModel = new RptDanhSachXuatKhoaDuoc().PreviewModel;
                    //ReportModel.Parameters["parTieuDeRpt"].Value = strTieuDe;
                    //ReportModel.Parameters["parV_MedProductType"].Value = (int)V_MedProductType;
                    //ReportModel.Parameters["parFromDate"].Value = FromDate;
                    //ReportModel.Parameters["parToDate"].Value = ToDate;
                    //ReportModel.Parameters["parQuy"].Value = Quy.Value;
                    //ReportModel.Parameters["parThang"].Value = Thang.Value;
                    //ReportModel.Parameters["parNam"].Value = Nam.Value;
                    //ReportModel.Parameters["parViewBy"].Value = Convert.ToInt32(message.ObjKey);

                    //// ReportModel.AutoShowParametersPanel = false;
                    //ReportModel.CreateDocument();

                }
            }
        }

        public void btExportExcel()
        {
            if (string.IsNullOrEmpty(strXMLResult))
            {
                MessageBox.Show(eHCMSResources.A0963_G1_Msg_InfoReportKhCoData);
                return;
            }
            ExportDataGrid(_dataGrid);
        }

        public static void ExportDataGrid(DataGrid dGrid)
        {
            try
            {
                //KMx: Cách làm việc của XML và XLS hoàn toàn giống nhau. Chỉ đổi extension để đồng bộ với tất cả các file excel (19/05/2015 10:58).
                //SaveFileDialog objSFD = new SaveFileDialog()
                //                            {
                //                                DefaultExt = "xml",
                //                                Filter = "Excel XML (*.xml)|*.xml",
                //                                FilterIndex = 1
                //                            };
                SaveFileDialog objSFD = new SaveFileDialog()
                {
                    DefaultExt = "xls",
                    Filter = "Excel xls (*.xls)|*.xls",
                    FilterIndex = 1
                };

                if (objSFD.ShowDialog() == true)
                {
                    //string strFormat = objSFD.SafeFileName.Substring(objSFD.SafeFileName.IndexOf('.') + 1).ToUpper();
                    string strFormat = objSFD.SafeFileName.Substring(objSFD.SafeFileName.IndexOf('.') + 1).ToLower();

                    StringBuilder strBuilder = new StringBuilder();

                    if (dGrid.ItemsSource == null)
                    {
                        return;
                    }

                    List<string> lstFields = new List<string>();

                    if (dGrid.HeadersVisibility == DataGridHeadersVisibility.Column || dGrid.HeadersVisibility == DataGridHeadersVisibility.All)
                    {
                        foreach (DataGridColumn dgcol in dGrid.Columns)
                        {
                            lstFields.Add(FormatField(dgcol.Header.ToString(), strFormat));
                        }

                        BuildStringOfRow(strBuilder, lstFields, strFormat);
                    }
                    foreach (object data in dGrid.ItemsSource)
                    {
                        lstFields.Clear();
                        foreach (DataGridColumn col in dGrid.Columns)
                        {
                            string strValue = "";

                            Binding objBinding = null;

                            if (col is DataGridBoundColumn)
                            {
                                objBinding = (Binding)((col as DataGridBoundColumn).Binding);
                            }
                            if (col is DataGridTemplateColumn)
                            {
                                //This is a template column...
                                //    let us see the underlying dependency object
                                DependencyObject objDO = (col as DataGridTemplateColumn).CellTemplate.LoadContent();
                                FrameworkElement oFE = (FrameworkElement)objDO;
                                FieldInfo oFI = oFE.GetType().GetField("TextProperty");
                                if (oFI != null)
                                {
                                    if (oFI.GetValue(null) != null)
                                    {
                                        if (oFE.GetBindingExpression((DependencyProperty)oFI.GetValue(null)) != null)
                                        {
                                            objBinding = oFE.GetBindingExpression((DependencyProperty)oFI.GetValue(null)).ParentBinding;
                                        }
                                    }
                                }
                            }
                            if (objBinding != null)
                            {
                                if (objBinding.Path.Path != "")
                                {
                                    PropertyInfo pi = data.GetType().GetProperty(objBinding.Path.Path);
                                    if (pi != null)
                                    {
                                        strValue = pi.GetValue(data, null).ToString();
                                    }
                                }

                                if (objBinding.Converter != null)
                                {
                                    if (strValue != "")
                                    {
                                        strValue =
                                            objBinding.Converter.Convert(
                                                ((SilverlightTable.Row)
                                                 (((SilverlightTable.Row)
                                                   (data.GetType().GetProperty(objBinding.Path.Path).GetValue(data, null)))
                                                     .Data)).Data,
                                                typeof(string), objBinding.ConverterParameter,
                                                objBinding.ConverterCulture).ToString();
                                    }
                                    else
                                    {
                                        strValue = objBinding.Converter.Convert(data,
                                                                                typeof(string),
                                                                                objBinding.ConverterParameter,
                                                                                objBinding.ConverterCulture).ToString();
                                    }
                                }
                            }

                            lstFields.Add(FormatField(strValue, strFormat));
                        }

                        BuildStringOfRow(strBuilder, lstFields, strFormat);
                    }

                    StreamWriter sw = new StreamWriter(objSFD.OpenFile());

                    //KMx: Cách làm việc của XML và XLS hoàn toàn giống nhau. Chỉ đổi extension để đồng bộ với tất cả các file excel (19/05/2015 10:58).
                    //if (strFormat == "XML")
                    if (strFormat == "xls")
                    {
                        //Let us write the headers for the Excel XML
                        sw.WriteLine("<?xml version=\"1.0\" " +
                                     "encoding=\"utf-8\"?>");
                        sw.WriteLine("<?mso-application progid" +
                                     "=\"Excel.Sheet\"?>");
                        sw.WriteLine("<Workbook xmlns=\"urn:" +
                                     "schemas-microsoft-com:office:spreadsheet\">");
                        sw.WriteLine("<DocumentProperties " +
                                     "xmlns=\"urn:schemas-microsoft-com:" +
                                     "office:office\">");
                        sw.WriteLine("<Author>" + Globals.LoggedUserAccount.Staff.FullName.Trim() + "</Author>");
                        sw.WriteLine("<Created>" +
                                     DateTime.Now.ToLocalTime().ToLongDateString() +
                                     "</Created>");
                        sw.WriteLine("<LastSaved>" +
                                     DateTime.Now.ToLocalTime().ToLongDateString() +
                                     "</LastSaved>");
                        sw.WriteLine("<Company>Viện Tim</Company>");
                        sw.WriteLine("<Version>12.00</Version>");
                        sw.WriteLine("</DocumentProperties>");
                        sw.WriteLine("<Worksheet ss:Name=\"" + eHCMSResources.N0222_G1_NhapXuatDenKP + "\" " +
                                     "xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\">");
                        sw.WriteLine("<Table>");
                    }

                    sw.Write(strBuilder.ToString());

                    //KMx: Cách làm việc của XML và XLS hoàn toàn giống nhau. Chỉ đổi extension để đồng bộ với tất cả các file excel (19/05/2015 10:58).
                    //if (strFormat == "XML")
                    if (strFormat == "xls")
                    {
                        sw.WriteLine("</Table>");
                        sw.WriteLine("</Worksheet>");
                        sw.WriteLine("</Workbook>");
                    }
                    sw.Close();

                    MessageBox.Show(eHCMSResources.A0467_G1_Msg_InfoDaKetXuat);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(eHCMSResources.A0795_G1_Msg_InfoXuatExcelFail);
            }
        }

        private static void BuildStringOfRow(StringBuilder strBuilder, List<string> lstFields, string strFormat)
        {
            switch (strFormat)
            {
                //KMx: Cách làm việc của XML và XLS hoàn toàn giống nhau. Chỉ đổi extension để đồng bộ với tất cả các file excel (19/05/2015 10:58).
                //case "XML":
                case "xls":
                    strBuilder.AppendLine("<Row>");
                    strBuilder.AppendLine(String.Join("\r\n", lstFields.ToArray()));
                    strBuilder.AppendLine("</Row>");
                    break;
                case "csv":
                    strBuilder.AppendLine(String.Join(",", lstFields.ToArray()));
                    break;
            }
        }

        private static string FormatField(string data, string format)
        {
            switch (format)
            {
                //KMx: Cách làm việc của XML và XLS hoàn toàn giống nhau. Chỉ đổi extension để đồng bộ với tất cả các file excel (19/05/2015 10:58).
                //case "XML":
                case "xls":
                    return String.Format("<Cell><Data ss:Type=\"String" +
                       "\">{0}</Data></Cell>", data);
                case "csv":
                    return String.Format("\"{0}\"",
                      data.Replace("\"", "\"\"\"").Replace("\n",
                      "").Replace("\r", ""));
            }
            return data;
        }
    }
}
