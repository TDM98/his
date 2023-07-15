using aEMR.Common.BaseModel;
using aEMR.Common.ExportExcel;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Castle.Windsor;
using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
/*
 * 20190731 #001 TTM:   BM 0013038: [FAST] Xuất dữ liệu excel cho từng bảng.
 */
namespace aEMR.TransactionManager.ViewModels
{
    [Export(typeof(IPreviewFastReport)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PreviewFastReportViewModel : ViewModelBase, IPreviewFastReport
    {
        [ImportingConstructor]
        public PreviewFastReportViewModel(IWindsorContainer aContainer, INavigationService aNavigation, ISalePosCaching aCaching)
        {
        }
        #region Properties
        enum TableCase : int
        {
            BienLai = 0,
            HoanUng = 1,
            HoaDon = 2,
            NhapDuoc = 3,
            XuatDuoc = 4,
            TraDuoc = 5,
            CTDoanhThu = 6
        }
        private ComboBox cboTableTypes { get; set; }
        private DataSet _FastReportDetails;
        private DataTable _gDataTable;
        private object _SelectedItemRow;
        public long FastReportID { get; set; }
        public DataSet FastReportDetails
        {
            get => _FastReportDetails; set
            {
                _FastReportDetails = value;
                NotifyOfPropertyChange(() => FastReportDetails);
            }
        }
        public DataTable gDataTable
        {
            get => _gDataTable; set
            {
                _gDataTable = value;
                NotifyOfPropertyChange(() => gDataTable);
            }
        }
        public object SelectedItemRow
        {
            get
            {
                return _SelectedItemRow;
            }
            set
            {
                if (_SelectedItemRow == value)
                {
                    return;
                }
                _SelectedItemRow = value;
                NotifyOfPropertyChange(() => SelectedItemRow);
            }
        }
        private long _V_FastReportType;
        public long V_FastReportType
        {
            get
            {
                return _V_FastReportType;
            }
            set
            {
                _V_FastReportType = value;
                NotifyOfPropertyChange(() => V_FastReportType);
            }
        }
        private Visibility _BienLai_HoanUngVisible = Visibility.Collapsed;
        public Visibility BienLai_HoanUngVisible
        {
            get { return _BienLai_HoanUngVisible; }
            set
            {
                _BienLai_HoanUngVisible = value;
                NotifyOfPropertyChange(() => BienLai_HoanUngVisible);
            }
        }
        private Visibility _HoaDonVisible = Visibility.Collapsed;
        public Visibility HoaDonVisible
        {
            get { return _HoaDonVisible; }
            set
            {
                _HoaDonVisible = value;
                NotifyOfPropertyChange(() => HoaDonVisible);
            }
        }
        private Visibility _NhapTraDuocVisible = Visibility.Collapsed;
        public Visibility NhapTraDuocVisible
        {
            get { return _NhapTraDuocVisible; }
            set
            {
                _NhapTraDuocVisible = value;
                NotifyOfPropertyChange(() => NhapTraDuocVisible);
            }
        }
        private Visibility _XuatDuocVisible = Visibility.Collapsed;
        public Visibility XuatDuocVisible
        {
            get { return _XuatDuocVisible; }
            set
            {
                _XuatDuocVisible = value;
                NotifyOfPropertyChange(() => XuatDuocVisible);
            }
        }
        #endregion
        #region Events
        public void cboTableTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboTableTypes == null)
            {
                return;
            }
            ViewTable();
        }
        public void cboTableTypes_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender == null || !(sender is ComboBox))
            {
                return;
            }
            
            cboTableTypes = sender as ComboBox;

            cboTableTypes.SelectedIndex = CheckDataComboBox();
            ViewTable();
        }
        private int CheckDataComboBox()
        {
            if (V_FastReportType == 85501)
            {
                BienLai_HoanUngVisible = Visibility.Visible;
                return 0;
            }
            else if (V_FastReportType == 85502)
            {
                NhapTraDuocVisible = Visibility.Visible;
                return 3;
            }
            else if (V_FastReportType == 85503)
            {
                XuatDuocVisible = Visibility.Visible;
                return 4;
            }
            else
            {
                BienLai_HoanUngVisible = Visibility.Visible;
                HoaDonVisible = Visibility.Visible;
                NhapTraDuocVisible = Visibility.Visible;
                XuatDuocVisible = Visibility.Visible;
                return 0;
            }
        }
        public void DeleteItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (cboTableTypes == null)
            {
                return;
            }
            int mTableIndex = cboTableTypes.SelectedIndex;
            if (FastReportDetails == null || FastReportDetails.Tables.Count <= mTableIndex || mTableIndex < 0 || SelectedItemRow == null || gDataTable == null)
            {
                return;
            }
            if (mTableIndex == (int)TableCase.HoaDon && false)
            {
                if (!(SelectedItemRow is DataRowView))
                {
                    return;
                }
                DataRowView CurrentRow = SelectedItemRow as DataRowView;
                var so_ct = !gDataTable.Columns.Contains("so_ct") ? null : CurrentRow.Row["so_ct"].ToString();
                var ng_mh = !gDataTable.Columns.Contains("ng_mh") ? null : CurrentRow.Row["ng_mh"].ToString();
                var noi_tru = !gDataTable.Columns.Contains("noi_tru") ? null : CurrentRow.Row["noi_tru"].ToString();
                if (string.IsNullOrEmpty(so_ct) || string.IsNullOrEmpty(noi_tru))
                {
                    return;
                }
                if (MessageBox.Show(string.Format(eHCMSResources.Z2802_G1_XoaHoaDon, string.Format("{0} [{1}]?", so_ct.Trim(), ng_mh.Trim())), eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    this.DlgShowBusyIndicator();
                    var t = new Thread(() =>
                    {
                        try
                        {
                            using (var serviceFactory = new TransactionServiceClient())
                            {
                                var contract = serviceFactory.ServiceInstance;
                                contract.BeginDeleteFastLinkedReportDetail(mTableIndex, FastReportID, so_ct, Convert.ToBoolean(noi_tru.Trim()), null, null, null, Globals.DispatchCallback((asyncResult) =>
                                {
                                    try
                                    {
                                        contract.EndDeleteFastLinkedReportDetail(asyncResult);
                                        while (gDataTable.Select(string.Format("so_ct='{0}'", so_ct)) != null && gDataTable.Select(string.Format("so_ct='{0}'", so_ct)).Count() > 0)
                                        {
                                            gDataTable.Rows.Remove(gDataTable.Select(string.Format("so_ct='{0}' AND noi_tru={1}", so_ct, noi_tru)).First());
                                        }
                                        ViewTable();
                                        MessageBox.Show(string.Format("{0}!", eHCMSResources.K0537_G1_XoaOk), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                            this.DlgHideBusyIndicator();
                        }
                    });
                    t.Start();
                }
            }
            else if (mTableIndex == (int)TableCase.NhapDuoc)
            {
                if (!(SelectedItemRow is DataRowView))
                {
                    return;
                }
                DataRowView CurrentRow = SelectedItemRow as DataRowView;
                var ma_hd = !gDataTable.Columns.Contains("ma_hd") ? null : CurrentRow.Row["ma_hd"].ToString();
                var so_ct = !gDataTable.Columns.Contains("so_ct") ? null : CurrentRow.Row["so_ct"].ToString();
                var ma_kh = !gDataTable.Columns.Contains("ma_kh") ? null : CurrentRow.Row["ma_kh"].ToString();
                var ma_vt = !gDataTable.Columns.Contains("ma_vt") ? null : CurrentRow.Row["ma_vt"].ToString();
                if (string.IsNullOrEmpty(so_ct) || string.IsNullOrEmpty(ma_kh) || string.IsNullOrEmpty(ma_vt))
                {
                    return;
                }
                if (MessageBox.Show(eHCMSResources.Z1403_G1_CoChacMuonXoaPhNhap, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    this.DlgShowBusyIndicator();
                    var t = new Thread(() =>
                    {
                        try
                        {
                            using (var serviceFactory = new TransactionServiceClient())
                            {
                                var contract = serviceFactory.ServiceInstance;
                                contract.BeginDeleteFastLinkedReportDetail(mTableIndex, FastReportID, so_ct, true, ma_kh, null, null, Globals.DispatchCallback((asyncResult) =>
                                {
                                    try
                                    {
                                        contract.EndDeleteFastLinkedReportDetail(asyncResult);
                                        while (gDataTable.Select(string.Format("so_ct='{0}' AND ma_kh='{1}' AND ma_vt='{2}'", so_ct, ma_kh, ma_vt)) != null
                                            && gDataTable.Select(string.Format("so_ct='{0}' AND ma_kh='{1}' AND ma_vt='{2}'", so_ct, ma_kh, ma_vt)).Count() > 0)
                                        {
                                            gDataTable.Rows.Remove(gDataTable.Select(string.Format("so_ct='{0}' AND ma_kh='{1}' AND ma_vt='{2}'", so_ct, ma_kh, ma_vt)).First());
                                        }
                                        ViewTable();
                                        MessageBox.Show(string.Format("{0}!", eHCMSResources.K0537_G1_XoaOk), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                            this.DlgHideBusyIndicator();
                        }
                    });
                    t.Start();
                }
            }
            else if (mTableIndex == (int)TableCase.XuatDuoc)
            {
                if (!(SelectedItemRow is DataRowView))
                {
                    return;
                }
                DataRowView CurrentRow = SelectedItemRow as DataRowView;
                var so_ct = !gDataTable.Columns.Contains("so_ct") ? null : CurrentRow.Row["so_ct"].ToString();
                var ma_vt = !gDataTable.Columns.Contains("ma_vt") ? null : CurrentRow.Row["ma_vt"].ToString();
                var ma_bp = !gDataTable.Columns.Contains("ma_bp") ? null : CurrentRow.Row["ma_bp"].ToString();
                var ma_kho = !gDataTable.Columns.Contains("ma_kho") ? null : CurrentRow.Row["ma_kho"].ToString();
                var noi_tru = !gDataTable.Columns.Contains("noi_tru") ? null : CurrentRow.Row["noi_tru"].ToString();
                if (string.IsNullOrEmpty(so_ct) || string.IsNullOrEmpty(ma_bp) || string.IsNullOrEmpty(ma_vt) || string.IsNullOrEmpty(ma_kho))
                {
                    return;
                }
                if (noi_tru == null || !Convert.ToBoolean(noi_tru.Trim()))
                {
                    return;
                }
                if (MessageBox.Show(eHCMSResources.A0171_G1_Msg_ConfXoaPh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    this.DlgShowBusyIndicator();
                    var t = new Thread(() =>
                    {
                        try
                        {
                            using (var serviceFactory = new TransactionServiceClient())
                            {
                                var contract = serviceFactory.ServiceInstance;
                                contract.BeginDeleteFastLinkedReportDetail(mTableIndex, FastReportID, so_ct, true, null, ma_bp, ma_kho, Globals.DispatchCallback((asyncResult) =>
                                {
                                    try
                                    {
                                        contract.EndDeleteFastLinkedReportDetail(asyncResult);
                                        while (gDataTable.Select(string.Format("so_ct='{0}' AND ma_bp='{1}' AND ma_kho='{2}'", so_ct, ma_bp, ma_kho)) != null
                                            && gDataTable.Select(string.Format("so_ct='{0}' AND ma_bp='{1}' AND ma_kho='{2}'", so_ct, ma_bp, ma_kho)).Count() > 0)
                                        {
                                            gDataTable.Rows.Remove(gDataTable.Select(string.Format("so_ct='{0}' AND ma_bp='{1}' AND ma_kho='{2}'", so_ct, ma_bp, ma_kho)).First());
                                        }
                                        ViewTable();
                                        MessageBox.Show(string.Format("{0}!", eHCMSResources.K0537_G1_XoaOk), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                            this.DlgHideBusyIndicator();
                        }
                    });
                    t.Start();
                }
            }
        }
        #endregion
        #region Methods
        public void ViewTable()
        {
            if (cboTableTypes == null)
            {
                return;
            }
            int mTableIndex = cboTableTypes.SelectedIndex;
            if (FastReportDetails == null || FastReportDetails.Tables.Count <= mTableIndex || mTableIndex < 0)
            {
                return;
            }
            gDataTable = FastReportDetails.Tables[mTableIndex];
        }
        //▼===== #001
        private string strNameExcel = "";
        public void btExportExcel()
        {
            List<List<string>> returnAllExcelData = new List<List<string>>();
            int mTableIndex = cboTableTypes.SelectedIndex;
            strNameExcel = cboTableTypes.Text;
            List<string> colname = new List<string>();
            for (int i = 0; i <= FastReportDetails.Tables[mTableIndex].Columns.Count - 1; i++)
            {
                if (CheckColName(mTableIndex, i))
                {
                    colname.Add(FastReportDetails.Tables[mTableIndex].Columns[i].ToString().Trim());
                }
            }

            returnAllExcelData.Add(colname);
            for (int i = 0; i <= FastReportDetails.Tables[mTableIndex].Rows.Count - 1; i++)
            {
                List<string> rowData = new List<string>();
                for (int j = 0; j <= FastReportDetails.Tables[mTableIndex].Columns.Count - 1; j++)
                {
                    if (CheckColName(mTableIndex, j))
                    {
                        rowData.Add(Convert.ToString(FastReportDetails.Tables[mTableIndex].Rows[i][j]).Replace("<", "&lt;").Replace(">", "&gt;"));
                    }
                }
                returnAllExcelData.Add(rowData);
            }
            ExportToExcelFileAllData.Export(returnAllExcelData, strNameExcel);
        }
        #endregion
        private bool CheckColName(int mTableIndex, int Index)
        {
            string StringCompare1 = "FastReportID";
            string StringCompare2 = "ID_tichhop";
            if (String.Compare(FastReportDetails.Tables[mTableIndex].Columns[Index].ToString(), StringCompare1, true) == 0
                || String.Compare(FastReportDetails.Tables[mTableIndex].Columns[Index].ToString(), StringCompare2, true) == 0)
            {
                return false;
            }
            return true;
        }
        //▲===== #001
    }
}