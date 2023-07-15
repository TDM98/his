using aEMR.Common.BaseModel;
using aEMR.Infrastructure;
using aEMR.ViewContracts;
using DataEntities;
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(ISelectTypeLoadBill)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SelectTypeLoadBillViewModel : ViewModelBase, ISelectTypeLoadBill
    {
        #region Properties
        private DateTime _FromDate;
        private DateTime _ToDate;
        public DateTime FromDate
        {
            get
            {
                return _FromDate;
            }
            set
            {
                _FromDate = value;
                NotifyOfPropertyChange(() => FromDate);
            }
        }
        public DateTime ToDate
        {
            get
            {
                return _ToDate;
            }
            set
            {
                _ToDate = value;
                NotifyOfPropertyChange(() => ToDate);
            }
        }
        public bool IsCompleted { get; set; } = !Globals.ServerConfigSection.CommonItems.AutoAddBedService;
        public int LoadBillType { get; set; } = 0;
        public DateTime? DischargeDate { get; set; }
        CheckBox chkTypeThongThuong;
        CheckBox chkTypeXuatKhoa;
        CheckBox chkTypeXuatVien;
        public void chkTypeThongThuong_Loaded(object sender, RoutedEventArgs e)
        {
            chkTypeThongThuong = sender as CheckBox;
            chkTypeThongThuong.IsChecked = true;
        }

        public void chkTypeThongThuong_Check(object sender, RoutedEventArgs e)
        {
            if (chkTypeThongThuong == null )
            {
                return;
            }
            chkTypeThongThuong.IsChecked = true;
            chkTypeXuatKhoa.IsChecked = false;
            chkTypeXuatVien.IsChecked = false;
            LoadBillType = (int)AllLookupValues.LoadBillType.BINHTHUONG;
        }

        public void chkTypeXuatKhoa_Loaded(object sender, RoutedEventArgs e)
        {
            chkTypeXuatKhoa = sender as CheckBox;
        }

        public void chkTypeXuatKhoa_Check(object sender, RoutedEventArgs e)
        {
            if (chkTypeXuatKhoa == null)
            {
                return;
            }
            chkTypeThongThuong.IsChecked = false;
            chkTypeXuatKhoa.IsChecked = true;
            chkTypeXuatVien.IsChecked = false;
            LoadBillType = (int)AllLookupValues.LoadBillType.XUATKHOA;
        }
        public void chkTypeXuatVien_Loaded(object sender, RoutedEventArgs e)
        {
            chkTypeXuatVien = sender as CheckBox;
        }

        public void chkTypeXuatVien_Check(object sender, RoutedEventArgs e)
        {
            if (chkTypeXuatVien == null)
            {
                return;
            }
            chkTypeThongThuong.IsChecked = false;
            chkTypeXuatKhoa.IsChecked = false;
            chkTypeXuatVien.IsChecked = true;
            LoadBillType = (int)AllLookupValues.LoadBillType.XUATVIEN;
        }
        #endregion

        #region Events
        public void btnOK()
        {
            if (LoadBillType == (int)AllLookupValues.LoadBillType.XUATVIEN && DischargeDate == null)
            {
                MessageBox.Show("Chưa có thông tin xuất viện!");
                TryClose();
            }
            else
            {
                IsCompleted = true;
                TryClose();
            }
        }
        public void btnCancel()
        {
            TryClose();
        }
        #endregion
    }
}
