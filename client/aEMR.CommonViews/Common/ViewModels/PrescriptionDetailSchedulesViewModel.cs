using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using eHCMSLanguage;
using aEMR.Controls;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IPrescriptionDetailSchedules)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PrescriptionDetailSchedulesViewModel : Conductor<object>, IPrescriptionDetailSchedules
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public PrescriptionDetailSchedulesViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching) 
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

        }

        private double _SoNgayDung;
        public double SoNgayDung
        {
            get { return _SoNgayDung; }
            set
            {
                _SoNgayDung = value;
                NotifyOfPropertyChange(()=> SoNgayDung);
            }
        }

        private int _Weeks=1;
        public int Weeks
        {
            get { return _Weeks; }
            set
            {
                _Weeks = value;
                NotifyOfPropertyChange(() => Weeks);
            }
        }

        private int _NDay = 1;
        public int NDay
        {
            get { return _NDay; }
            set
            {
                _NDay = value;
                NotifyOfPropertyChange(() => NDay);
                Weeks = _NDay % 7 == 0 ? _NDay / 7 : _NDay / 7 + 1;
            }
        }


        private int _ModeForm;
        public int ModeForm
        {
            get { return _ModeForm; }
            set
            {
                if (_ModeForm != value)
                {
                    _ModeForm = value;
                    NotifyOfPropertyChange(() => ModeForm);
                }
            }
        }

        private PrescriptionDetail _ObjPrescriptionDetail;
        public PrescriptionDetail ObjPrescriptionDetail
        {
            get
            {
                return _ObjPrescriptionDetail;
            }
            set
            {
                if (_ObjPrescriptionDetail != value)
                {
                    _ObjPrescriptionDetail = value;
                    NotifyOfPropertyChange(() => ObjPrescriptionDetail);
                }
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
        }

        public void Initialize()
        {
            if (ObjPrescriptionDetailSchedules_ByPrescriptDetailID == null || ObjPrescriptionDetailSchedules_ByPrescriptDetailID.Count <= 0)
            {
                ObjPrescriptionDetail.SelectedDrugForPrescription.DayRpts = 0;
                PrescriptionDetailSchedules_ByPrescriptDetailID(0,false);
            }
            else
            {
                //int days = Convert.ToInt32(ObjPrescriptionDetail.DayRpts);
                //Weeks = days % 7 == 0 ? days / 7 : days / 7 + 1;
                double d = CalcTongSoNgayDungThuocTrongLich();

                if (d <= 0)
                {
                    TongThuocPhaiDung = 0;
                    return;
                }

                if (ObjPrescriptionDetail.SelectedDrugForPrescription.DayRpts == d)
                {
                    TinhTongThuoc();
                }
                else
                {
                    PrescriptionDetailSchedules ObjRowSang = ObjPrescriptionDetailSchedules_ByPrescriptDetailID[0];
                    PrescriptionDetailSchedules ObjRowTrua = ObjPrescriptionDetailSchedules_ByPrescriptDetailID[1];
                    PrescriptionDetailSchedules ObjRowChieu = ObjPrescriptionDetailSchedules_ByPrescriptDetailID[2];
                    PrescriptionDetailSchedules ObjRowToi = ObjPrescriptionDetailSchedules_ByPrescriptDetailID[3];

                    int SumSang = 0;
                    int SumTrua = 0;
                    int SumChieu = 0;
                    int SumToi = 0;

                    float TongThuocOfRowSang = 0;
                    float TongThuocOfRowTrua = 0;
                    float TongThuocOfRowChieu = 0;
                    float TongThuocOfRowToi = 0;
                    double TongCong = 0;

                    Check1RowExistsLess3Value(ObjRowSang, out SumSang, out TongThuocOfRowSang);
                    Check1RowExistsLess3Value(ObjRowTrua, out SumTrua, out TongThuocOfRowTrua);
                    Check1RowExistsLess3Value(ObjRowChieu, out SumChieu, out TongThuocOfRowChieu);
                    Check1RowExistsLess3Value(ObjRowToi, out SumToi, out TongThuocOfRowToi);

                    TongCong = (TongThuocOfRowSang + TongThuocOfRowTrua + TongThuocOfRowChieu + TongThuocOfRowToi);

                    //TongThuocPhaiDung = Math.Ceiling((TongCong * ObjPrescriptionDetail.SelectedDrugForPrescription.UnitVolume) / ObjPrescriptionDetail.SelectedDrugForPrescription.DispenseVolume);
                    TongThuocPhaiDung = Math.Ceiling(Weeks * (TongCong * ObjPrescriptionDetail.SelectedDrugForPrescription.UnitVolume) / ObjPrescriptionDetail.SelectedDrugForPrescription.DispenseVolume);

                    double KQ = TongThuocPhaiDung;//Math.Ceiling((ObjPrescriptionDetail.SelectedDrugForPrescription.DayRpts * TongThuocPhaiDung) / d);
                    TongThuocPhaiDung = KQ;
                    SoNgayDung = ObjPrescriptionDetail.SelectedDrugForPrescription.DayRpts;
                }
            }
        }

        private bool _HasSchedule;
        public bool HasSchedule
        {
            get { return _HasSchedule; }
            set
            {
                if (_HasSchedule != value)
                {
                    _HasSchedule = value;
                    NotifyOfPropertyChange(() => HasSchedule);
                }
            }

        }

        private bool _IsMaxDay=false;
        public bool IsMaxDay
        {
            get { return _IsMaxDay; }
            set
            {
                if (_IsMaxDay != value)
                {
                    _IsMaxDay = value;
                    NotifyOfPropertyChange(() => IsMaxDay);
                }
            }
        }

        private ObservableCollection<PrescriptionDetailSchedules> _ObjPrescriptionDetailSchedules_ByPrescriptDetailID;
        public ObservableCollection<PrescriptionDetailSchedules> ObjPrescriptionDetailSchedules_ByPrescriptDetailID
        {
            get
            {
                return _ObjPrescriptionDetailSchedules_ByPrescriptDetailID;
            }
            set
            {
                if (_ObjPrescriptionDetailSchedules_ByPrescriptDetailID != value)
                {
                    _ObjPrescriptionDetailSchedules_ByPrescriptDetailID = value;
                    NotifyOfPropertyChange(() => ObjPrescriptionDetailSchedules_ByPrescriptDetailID);
                }
            }
        }

        public void PrescriptionDetailSchedules_ByPrescriptDetailID(Int64 PrescriptDetailID, bool IsNotIncat)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z1121_G1_CTietLieuDungTrongTuan) });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPrescriptionDetailSchedules_ByPrescriptDetailID(PrescriptDetailID,  IsNotIncat, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndPrescriptionDetailSchedules_ByPrescriptDetailID(asyncResult);

                            if (items != null)
                            {
                                ObjPrescriptionDetailSchedules_ByPrescriptDetailID = new ObservableCollection<PrescriptionDetailSchedules>(items);
                            }
                            else
                            {
                                ObjPrescriptionDetailSchedules_ByPrescriptDetailID = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
        
        public void hplDelete_Click(Object SelectedItem)
        {
            PrescriptionDetailSchedules Objtemp = SelectedItem as PrescriptionDetailSchedules;

            if (Objtemp != null)
            {
                int Sum = 0;

                float TongThuocOfRow = 0;

                Check1RowExistsLess3Value(Objtemp, out Sum, out TongThuocOfRow);
                if (Sum > 0)
                {
                    if (MessageBox.Show(string.Format(eHCMSResources.Z1076_G1_CoMuonXoaLieuDung, Objtemp.ObjV_PeriodOfDay.ObjectValue.Trim()), eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        Objtemp.Monday = 0;
                        Objtemp.Tuesday = 0;
                        Objtemp.Wednesday = 0;
                        Objtemp.Thursday = 0;
                        Objtemp.Friday = 0;
                        Objtemp.Saturday = 0;
                        Objtemp.Sunday = 0;
                        Objtemp.UsageNote = "";
                    }
                }
            }
        }

        public void btCancel()
        {
            TryClose();
        }

        private double _TongThuocPhaiDung;
        public double TongThuocPhaiDung
        {
            get { return _TongThuocPhaiDung; }
            set
            {
                if(_TongThuocPhaiDung!=value)
                {
                    if (value >= 0)
                    {
                        _TongThuocPhaiDung = value;
                    }
                    else
                    {
                        _TongThuocPhaiDung = 0;
                    }
                    NotifyOfPropertyChange(() => TongThuocPhaiDung);
                }
            }
        }

        public void btSave()
        {
            bool ResultCheck = false;

            double d = CalcTongSoNgayDungThuocTrongLich();

            if (d==0)
            {
                if (MessageBox.Show(eHCMSResources.A0762_G1_Msg_ConfLuuKhCDinhLieuDung, eHCMSResources.G0442_G1_TBao,MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    ResultCheck = true;
                    HasSchedule = false;
                }
            }
            else
            {
                ResultCheck = true;
                HasSchedule = true;
            }

            if (ResultCheck)
            {
                //Phát sự kiện gởi về cho Form Toa Thuốc 
                Globals.EventAggregator.Publish(new SendPrescriptionDetailSchedulesEvent<ObservableCollection<PrescriptionDetailSchedules>, bool,double,double,string, int>()
                {
                    Data = ObjPrescriptionDetailSchedules_ByPrescriptDetailID,
                    HasSchedule = HasSchedule,
                    TongThuoc = TongThuocPhaiDung,
                    //SoNgayDung = ObjPrescriptionDetail.SelectedDrugForPrescription.DayRpts,
                    SoNgayDung = d,
                    GhiChu = ObjPrescriptionDetail.DrugInstructionNotes,
                    ModeForm = ModeForm
                });
                TryClose();
            }

        }

        private void Check1RowExistsLess3Value(PrescriptionDetailSchedules ObjRow, out int SumSet, out float TongThuoc1Buoi)/*Tổng Thuốc 1 Buổi: Sáng Or Trưa, Chiều, Tối*/
        {
            TongThuoc1Buoi = 0;

            int dem = 0;

            if (ObjRow.Monday > 0)
            {
                dem++;
                TongThuoc1Buoi += ObjRow.Monday.Value;
            }
            if (ObjRow.Tuesday > 0)
            {
                dem++;
                TongThuoc1Buoi += ObjRow.Tuesday.Value;
            }
            if (ObjRow.Wednesday > 0)
            {
                dem++;
                TongThuoc1Buoi += ObjRow.Wednesday.Value;
            }
            if (ObjRow.Thursday > 0)
            {
                dem++;
                TongThuoc1Buoi += ObjRow.Thursday.Value;
            }
            if (ObjRow.Friday > 0)
            {
                dem++;
                TongThuoc1Buoi += ObjRow.Friday.Value;
            }
            if (ObjRow.Saturday > 0)
            {
                dem++;
                TongThuoc1Buoi += ObjRow.Saturday.Value;
            }
            if (ObjRow.Sunday > 0)
            {
                dem++;
                TongThuoc1Buoi += ObjRow.Sunday.Value;
            }

            SumSet = dem;
        }

        public void dtgList_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            int IndexCol = e.Column.DisplayIndex;
            string StringNameControl = GetStringNameControl(IndexCol);
            TextBox Ctrtb = ((sender as DataGrid)).CurrentColumn.GetCellContent(e.Row).FindName(StringNameControl) as TextBox;
            if (Ctrtb != null)
            {
                Ctrtb.Focus();
                Ctrtb.SelectAll();
            }
        }

        private string GetStringNameControl(int IndexCol)
        {
            string Result = "";
            switch (IndexCol)
            {
                case 3:
                    {
                        Result = "tbMonday";
                        break;
                    }
                case 4:
                    {
                        Result = "tbTuesday";
                        break;
                    }
                case 5:
                    {
                        Result = "tbWednesday";
                        break;
                    }
                case 6:
                    {
                        Result = "tbThursday";
                        break;
                    }
                case 7:
                    {
                        Result = "tbFriday";
                        break;
                    }
                case 8:
                    {
                        Result = "tbSaturday";
                        break;
                    }
                case 9:
                    {
                        Result = "tbSunday";
                        break;
                    }
                //case 10:
                //    {
                //        Result = "tbUsageNote";
                //        break;
                //    }
            }
            return Result;
        }

        #region Liều Dùng
        public void cboLieuDung_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AxComboBox Ctr = (sender as AxComboBox);
            if (Ctr != null)
            {
                if (Ctr.SelectedItemEx != null)
                {
                    PrescriptionDetailSchedules Objtmp = (Ctr.DataContext) as PrescriptionDetailSchedules;
                    PrescriptionDetailSchedulesLieuDung ObjLieuDung = Ctr.SelectedItemEx as PrescriptionDetailSchedulesLieuDung;

                    Objtmp.Monday = ObjLieuDung.ID;
                    Objtmp.Tuesday = ObjLieuDung.ID;
                    Objtmp.Wednesday = ObjLieuDung.ID;
                    Objtmp.Thursday = ObjLieuDung.ID;
                    Objtmp.Friday = ObjLieuDung.ID;
                    Objtmp.Saturday = ObjLieuDung.ID;
                    Objtmp.Sunday = ObjLieuDung.ID;

                    TinhTongThuoc();
                }
            }
        }
        #endregion
      

        public void btDelete()
        {
            if (MessageBox.Show(eHCMSResources.K0526_G1_XoaLieuDungTrongTuan + " " + eHCMSResources.K0478_G1_BanCoChacKhong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeleteSchedule();
            }
        }

        private void DeleteSchedule()
        {
            foreach (var Objtemp in ObjPrescriptionDetailSchedules_ByPrescriptDetailID)
            {
                Objtemp.Monday = 0;
                Objtemp.Tuesday = 0;
                Objtemp.Wednesday = 0;
                Objtemp.Thursday = 0;
                Objtemp.Friday = 0;
                Objtemp.Saturday = 0;
                Objtemp.Sunday = 0;
                Objtemp.UsageNote = "";    
            }
        }


        private double TinhTongThuoc()
        {
            double d= CalcTongSoNgayDungThuocTrongLich();

            ObjPrescriptionDetail.SelectedDrugForPrescription.DayRpts = d;
            SoNgayDung = d;

            PrescriptionDetailSchedules ObjRowSang = ObjPrescriptionDetailSchedules_ByPrescriptDetailID[0];
            PrescriptionDetailSchedules ObjRowTrua = ObjPrescriptionDetailSchedules_ByPrescriptDetailID[1];
            PrescriptionDetailSchedules ObjRowChieu = ObjPrescriptionDetailSchedules_ByPrescriptDetailID[2];
            PrescriptionDetailSchedules ObjRowToi = ObjPrescriptionDetailSchedules_ByPrescriptDetailID[3];

            int SumSang = 0;
            int SumTrua = 0;
            int SumChieu = 0;
            int SumToi = 0;

            float TongThuocOfRowSang = 0;
            float TongThuocOfRowTrua = 0;
            float TongThuocOfRowChieu = 0;
            float TongThuocOfRowToi = 0;
            double TongCong = 0;

            Check1RowExistsLess3Value(ObjRowSang, out SumSang, out TongThuocOfRowSang);
            Check1RowExistsLess3Value(ObjRowTrua, out SumTrua, out TongThuocOfRowTrua);
            Check1RowExistsLess3Value(ObjRowChieu, out SumChieu, out TongThuocOfRowChieu);
            Check1RowExistsLess3Value(ObjRowToi, out SumToi, out TongThuocOfRowToi);

            TongCong = (TongThuocOfRowSang + TongThuocOfRowTrua + TongThuocOfRowChieu + TongThuocOfRowToi);

            if (ObjPrescriptionDetail.IsDrugNotInCat == false)
            {
                TongThuocPhaiDung = Math.Ceiling(Weeks*(TongCong * ObjPrescriptionDetail.SelectedDrugForPrescription.UnitVolume) / ObjPrescriptionDetail.SelectedDrugForPrescription.DispenseVolume);
            }
            else
            {
                TongThuocPhaiDung = Math.Ceiling(Weeks * TongCong);
            }
            return TongThuocPhaiDung = TongThuocPhaiDung ;
        }

        private double CalcTongSoNgayDungThuocTrongLich()
        {
            double d = 0;

            PrescriptionDetailSchedules ObjRowSang = ObjPrescriptionDetailSchedules_ByPrescriptDetailID[0];
            PrescriptionDetailSchedules ObjRowTrua = ObjPrescriptionDetailSchedules_ByPrescriptDetailID[1];
            PrescriptionDetailSchedules ObjRowChieu = ObjPrescriptionDetailSchedules_ByPrescriptDetailID[2];
            PrescriptionDetailSchedules ObjRowToi = ObjPrescriptionDetailSchedules_ByPrescriptDetailID[3];

            if (ObjRowSang.Monday > 0 || ObjRowTrua.Monday > 0 || ObjRowChieu.Monday > 0 || ObjRowToi.Monday > 0)
                d++;
            if (ObjRowSang.Tuesday > 0 || ObjRowTrua.Tuesday > 0 || ObjRowChieu.Tuesday > 0 || ObjRowToi.Tuesday > 0)
                d++;
            if (ObjRowSang.Wednesday > 0 || ObjRowTrua.Wednesday > 0 || ObjRowChieu.Wednesday > 0 || ObjRowToi.Wednesday > 0)
                d++;
            if (ObjRowSang.Thursday > 0 || ObjRowTrua.Thursday > 0 || ObjRowChieu.Thursday > 0 || ObjRowToi.Thursday > 0)
                d++;
            if (ObjRowSang.Friday > 0 || ObjRowTrua.Friday > 0 || ObjRowChieu.Friday > 0 || ObjRowToi.Friday > 0)
                d++;
            if (ObjRowSang.Saturday > 0 || ObjRowTrua.Saturday > 0 || ObjRowChieu.Saturday > 0 || ObjRowToi.Saturday > 0)
                d++;
            if (ObjRowSang.Sunday > 0 || ObjRowTrua.Sunday > 0 || ObjRowChieu.Sunday > 0 || ObjRowToi.Sunday > 0)
                d++;
            return d*Weeks;
        }

        public void cboLieuDungT2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TinhTongThuoc();
        }
        public void cboLieuDungT3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TinhTongThuoc();
        }
        public void cboLieuDungT4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TinhTongThuoc();
        }
        public void cboLieuDungT5_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TinhTongThuoc();
        }
        public void cboLieuDungT6_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TinhTongThuoc();
        }
        public void cboLieuDungT7_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TinhTongThuoc();
        }
        public void cboLieuDungCN_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TinhTongThuoc();
        }

        
        public void tbSoNgayInput_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox Ctr = (sender as TextBox);

            try 
            {
                if (Ctr != null)
                {
                    int V = 0;
                    int.TryParse(Ctr.Text, out V);
                    double d = CalcTongSoNgayDungThuocTrongLich();
                    TinhTongThuoc();                
                }
            }catch(Exception ex)
            {
                ClientLoggerHelper.LogInfo(ex.ToString());
                MessageBox.Show(string.Format("{0}.", eHCMSResources.A0559_G1_Msg_InfoGTriKhHopLe));
            }
        }
    }
}
