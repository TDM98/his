//using eHCMSLanguage;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.Composition;
//using System.ServiceModel;
//using System.Threading;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Input;
//using aEMR.DataContracts;
//using aEMR.Infrastructure;
//using aEMR.Infrastructure.Events;
//using aEMR.ServiceClient;
//using aEMR.ViewContracts;
//using Caliburn.Micro;
//using DataEntities;
//using aEMR.Common;
//using aEMR.Common.Collections;
//using System.Collections.ObjectModel;
//using System.Xml.Linq;
//using System.Linq;
//using System.Windows.Data;
//using System.Text;
//using SilverlightTable;
//using Castle.Windsor;

//namespace aEMR.ConsultantEPrescription.ViewModels
//{
//    [Export(typeof(IViewResultPCLLaboratoryByListExamTest)), PartCreationPolicy(CreationPolicy.NonShared)]
//    public class ViewResultPCLLaboratoryByListExamTestViewModel : Conductor<object>, IViewResultPCLLaboratoryByListExamTest
//    {
//        private bool _isLoading;
//        public bool IsLoading
//        {
//            get { return _isLoading; }
//            set
//            {
//                if (_isLoading != value)
//                {
//                    _isLoading = value;
//                    NotifyOfPropertyChange(() => IsLoading);
//                }
//            }
//        }

//        public void InitData() 
//        {
//        }
//        [ImportingConstructor]
//        public ViewResultPCLLaboratoryByListExamTestViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
//        {
//            Globals.EventAggregator.Subscribe(this);

//            SearchCriteria = new GeneralSearchCriteria();

//            ObjPCLExamTestItemSelectedLst = new ObservableCollection<PCLExamTestItems>();

//            ObjPCLExamTestItems = new PagedSortableCollectionView<DataEntities.PCLExamTestItems>();
//            ObjPCLExamTestItems.PageSize = Globals.PageSize;
//            ObjPCLExamTestItems.OnRefresh += new EventHandler<RefreshEventArgs>(ObjPCLExamTestItems_OnRefresh);

//            PCLExamTestItems_SearchPaging(ObjPCLExamTestItems.PageIndex, ObjPCLExamTestItems.PageSize);
//        }

//        void ObjPCLExamTestItems_OnRefresh(object sender, RefreshEventArgs e)
//        {
//            PCLExamTestItems_SearchPaging(ObjPCLExamTestItems.PageIndex, ObjPCLExamTestItems.PageSize);
//        }

//        public override void DeactivateItem(object item, bool close)
//        {
//            base.DeactivateItem(item, close);
//            Globals.EventAggregator.Unsubscribe(this);
//        }

//        private DateTime? _fromDate;
//        public DateTime? FromDate
//        {
//            get { return _fromDate; }
//            set
//            {
//                _fromDate = value;
//                NotifyOfPropertyChange(() => FromDate);
//            }
//        }

//        private DateTime? _toDate ;
//        public DateTime? ToDate
//        {
//            get { return _toDate; }
//            set
//            {
//                _toDate = value;
//                NotifyOfPropertyChange(() => ToDate);
//            }
//        }

//        private int _TotalRow = 0;
//        public int TotalRow
//        {
//            get { return _TotalRow; }
//            set
//            {
//                _TotalRow = value;
//                NotifyOfPropertyChange(() => TotalRow);
//                NotifyOfPropertyChange(() => IsEnableNext);
//            }
//        }

//        private int _PageIndex = 0;
//        public int PageIndex
//        {
//            get { return _PageIndex; }
//            set
//            {
//                _PageIndex = value;
//                NotifyOfPropertyChange(() => PageIndex);
//                NotifyOfPropertyChange(() => IsEnablePrev);
//                NotifyOfPropertyChange(() => IsEnableNext);
//            }
//        }

//        //so cot ngay : de lay du lieu xem
//        private int _PageSize = 10;
//        public int PageSize
//        {
//            get { return _PageSize; }
//            set
//            {
//                _PageSize = value;
//                NotifyOfPropertyChange(() => PageSize);
//                NotifyOfPropertyChange(() => IsEnableNext);
//            }
//        }

//        public bool IsEnablePrev
//        {
//            get { return PageIndex > 0; }
//        }

//        public bool IsEnableNext
//        {
//            get { return (TotalRow / PageSize > PageIndex); }
//        }


//        public void PrevClick()
//        {
//            if (PageIndex > 0)
//            {
//                PageIndex = PageIndex - 1;
//            }
//            PatientPCLLaboratoryResults_ByExamTest_Crosstab(Globals.PatientAllDetails.PatientInfo.PatientID, strXML, PageIndex, PageSize);
//        }

//        public void NextClick()
//        {
//            if (PageIndex < TotalRow / PageSize)
//            {
//                PageIndex = PageIndex + 1;
//            }
//            PatientPCLLaboratoryResults_ByExamTest_Crosstab(Globals.PatientAllDetails.PatientInfo.PatientID, strXML, PageIndex, PageSize);
//        }

//        #region member ItemTest

//        private GeneralSearchCriteria _SearchCriteria;
//        public GeneralSearchCriteria SearchCriteria
//        {
//            get { return _SearchCriteria; }
//            set
//            {
//                _SearchCriteria = value;
//                NotifyOfPropertyChange(() => SearchCriteria);
//            }
//        }


//        private PagedSortableCollectionView<DataEntities.PCLExamTestItems> _ObjPCLExamTestItems;
//        public PagedSortableCollectionView<DataEntities.PCLExamTestItems> ObjPCLExamTestItems
//        {
//            get { return _ObjPCLExamTestItems; }
//            set
//            {
//                _ObjPCLExamTestItems = value;
//                NotifyOfPropertyChange(() => ObjPCLExamTestItems);
//            }
//        }

//        private ObservableCollection<DataEntities.PCLExamTestItems> _ObjPCLExamTestItemSelectedLst;
//        public ObservableCollection<DataEntities.PCLExamTestItems> ObjPCLExamTestItemSelectedLst
//        {
//            get { return _ObjPCLExamTestItemSelectedLst; }
//            set
//            {
//                _ObjPCLExamTestItemSelectedLst = value;
//                NotifyOfPropertyChange(() => ObjPCLExamTestItemSelectedLst);
//            }
//        }

//        private void PCLExamTestItems_SearchPaging(int PageIndex, int PageSize)
//        {
//            var t = new Thread(() =>
//            {
//                //IsLoading = true;

//                try
//                {
//                    using (var serviceFactory = new PCLsClient())
//                    {
//                        var client = serviceFactory.ServiceInstance;
//                        client.BeginPCLExamTestItems_SearchPaging(SearchCriteria, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
//                        {
//                            int Total = 0;
//                            try
//                            {
//                                var allItems = client.EndPCLExamTestItems_SearchPaging(out Total, asyncResult);
//                                ObjPCLExamTestItems.Clear();
//                                ObjPCLExamTestItems.TotalItemCount = Total;
//                                if (allItems != null)
//                                {
//                                    foreach (var item in allItems)
//                                    {
//                                        ObjPCLExamTestItems.Add(item);
//                                    }

//                                }
//                            }
//                            catch (Exception innerEx)
//                            {
//                                ClientLoggerHelper.LogInfo(innerEx.ToString());
//                            }


//                        }), null)
//                            ;
//                    }
//                }
//                catch (Exception ex)
//                {
//                    ClientLoggerHelper.LogInfo(ex.ToString());
//                }
//                finally
//                {
//                }
//            });
//            t.Start();
//        }

//        public void btSearch()
//        {
//            ObjPCLExamTestItems.PageIndex = 0;
//            PCLExamTestItems_SearchPaging(ObjPCLExamTestItems.PageIndex, ObjPCLExamTestItems.PageSize);
//        }

//        public void DoubleClick(object args)
//        {
//            EventArgs<object> eventArgs = args as EventArgs<object>;
//            DataEntities.PCLExamTestItems items = eventArgs.Value as DataEntities.PCLExamTestItems;
//            bool exists = false;
//            foreach (DataEntities.PCLExamTestItems ite in ObjPCLExamTestItemSelectedLst)
//            {
//                if (items.PCLExamTestItemCode == ite.PCLExamTestItemCode)
//                {
//                    exists = true;
//                    break;
//                }
//            }
//            if (!exists)
//            {
//                ObjPCLExamTestItemSelectedLst.Add(items);
//            }
//        }

//        public void Search_Name(object sender, KeyEventArgs e)
//        {
//            if (e.Key == Key.Enter)
//            {
//                if (SearchCriteria != null)
//                {
//                    SearchCriteria.FindName = (sender as TextBox).Text;
//                }
//                btSearch();
//            }
//        }

//        #endregion

//        #region Results member

//        SortableCollectionView data = new SortableCollectionView();

//        private void PatientPCLLaboratoryResults_ByExamTest_Crosstab(long PatientID, string strXML, int PageIndex, int PageSize)
//        {
//            // Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Đang tìm bệnh nhân..." });
//            data.Clear();
//            _dataGrid.Columns.Clear();

//            var t = new Thread(() =>
//            {
//                AxErrorEventArgs error = null;
//                try
//                {
//                    IsLoading = true;
//                    using (var serviceFactory = new PCLsClient())
//                    {
//                        var client = serviceFactory.ServiceInstance;
//                        client.BeginPatientPCLLaboratoryResults_ByExamTest_Crosstab(PatientID, strXML, FromDate, ToDate, PageIndex, PageSize, Globals.DispatchCallback((asyncResult) =>
//                        {

//                            try
//                            {
//                                int TotalCol = 0;
//                                var Results = client.EndPatientPCLLaboratoryResults_ByExamTest_Crosstab(out TotalCol, asyncResult);
//                                this.TotalRow = TotalCol;

//                                if (!string.IsNullOrEmpty(Results))
//                                {
//                                    // grab the xml into a XDocument
//                                    XDocument xmlDoc = XDocument.Parse(Results);

//                                    // find the columns
//                                    List<string> columnNames = xmlDoc.Descendants("column")
//                                                                     .Attributes("name")
//                                                                     .Select(a => a.Value)
//                                                                     .ToList();


//                                    // add them to the grid
//                                    foreach (string columnName in columnNames)
//                                    {
//                                        _dataGrid.Columns.Add(CreateColumn(columnName));
//                                    }

//                                    // add the rows
//                                    var rows = xmlDoc.Descendants("row");
//                                    foreach (var row in rows)
//                                    {
//                                        Row rowData = new Row();
//                                        int index = 0;
//                                        var cells = row.Descendants("cell");
//                                        foreach (var cell in cells)
//                                        {
//                                            rowData[columnNames[index]] = cell.Value;
//                                            index++;
//                                        }
//                                        data.Add(rowData);
//                                    }

//                                    _dataGrid.ItemsSource = data;
//                                }

//                            }
//                            catch (FaultException<AxException> fault)
//                            {
//                                error = new AxErrorEventArgs(fault);
//                            }
//                            catch (Exception ex)
//                            {
//                                error = new AxErrorEventArgs(ex);
//                            }

//                        }), null)
//                            ;
//                    }
//                }
//                catch (Exception ex)
//                {
//                    error = new AxErrorEventArgs(ex);
//                }
//                finally
//                {
//                    IsLoading = false;
//                    Globals.IsBusy = false;
//                }
//                if (error != null)
//                {
//                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
//                }
//            });
//            t.Start();
//        }

//        private RowIndexConverter _rowIndexConverter = new RowIndexConverter();
//        private DataGridColumn CreateColumn(string property)
//        {
//            return new DataGridTextColumn()
//            {
//                CanUserSort = true,
//                Header = property,
//                SortMemberPath = property,
//                IsReadOnly = false,
//                Binding = new Binding("Data")
//                {
//                    Converter = _rowIndexConverter,
//                    ConverterParameter = property
//                }
//            };
//        }

//        DataGrid _dataGrid = null;
//        public void PendingClientsGrid_Loaded(object sender, RoutedEventArgs e)
//        {
//            _dataGrid = sender as DataGrid;
//        }

//        private string strXML = "";
//        public void btnViewResults()
//        {
//            if (Globals.PatientAllDetails.PatientInfo == null)
//            {
//                MessageBox.Show(eHCMSResources.K0290_G1_ChonBN);
//                return;
//            }
//            if (FromDate != null && ToDate != null)
//            {
//                if (eHCMS.Services.Core.AxHelper.CompareDate(FromDate.GetValueOrDefault(), ToDate.GetValueOrDefault()) == 1)
//                {
//                    MessageBox.Show(eHCMSResources.Z0394_G1_NgNhapKgHopLe);
//                    return;
//                }
//            }
//            StringBuilder sb = new StringBuilder();
//            sb.Append("<Root>");
//            sb.Append("<IDList>");
//            if (ObjPCLExamTestItemSelectedLst != null && ObjPCLExamTestItemSelectedLst.Count() > 0)
//            {
//                foreach (var details in ObjPCLExamTestItemSelectedLst)
//                {
//                    sb.AppendFormat("<ID>{0}</ID>", details.PCLExamTestItemID);
//                }
//            }
//            else
//            {
//                return;
//            }
//            sb.Append("</IDList>");
//            sb.Append("</Root>");
//            strXML = sb.ToString();
//            PageIndex = 0;
//            PatientPCLLaboratoryResults_ByExamTest_Crosstab(Globals.PatientAllDetails.PatientInfo.PatientID, strXML, PageIndex, PageSize);
//        }

//        #endregion

//        private bool _AllChecked;
//        public bool AllChecked
//        {
//            get
//            {
//                return _AllChecked;
//            }
//            set
//            {
//                if (_AllChecked != value)
//                {
//                    _AllChecked = value;
//                    NotifyOfPropertyChange(() => AllChecked);
//                    if (_AllChecked)
//                    {
//                        AllCheckedfc();
//                    }
//                    else
//                    {
//                        UnCheckedfc();
//                    }
//                }
//            }
//        }
//        private void AllCheckedfc()
//        {
//            if (ObjPCLExamTestItemSelectedLst != null && ObjPCLExamTestItemSelectedLst.Count > 0)
//            {
//                for (int i = 0; i < ObjPCLExamTestItemSelectedLst.Count; i++)
//                {
//                    ObjPCLExamTestItemSelectedLst[i].Checked = true;
//                }
//            }
//        }

//        private void UnCheckedfc()
//        {
//            if (ObjPCLExamTestItemSelectedLst != null && ObjPCLExamTestItemSelectedLst.Count > 0)
//            {
//                for (int i = 0; i < ObjPCLExamTestItemSelectedLst.Count; i++)
//                {
//                    ObjPCLExamTestItemSelectedLst[i].Checked = false;
//                }
//            }
//        }

//        public void btnClear()
//        {
//            ObjPCLExamTestItemSelectedLst = ObjPCLExamTestItemSelectedLst.Where(x=>x.Checked==false).ToObservableCollection();
//        }

//    }

//}
