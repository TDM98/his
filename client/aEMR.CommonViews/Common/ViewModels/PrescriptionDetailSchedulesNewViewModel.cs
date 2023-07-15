using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Common.Collections;
using Caliburn.Micro;
using DataEntities;
using aEMR.Controls;
using eHCMSLanguage;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IPrescriptionDetailSchedulesNew)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PrescriptionDetailSchedulesNewViewModel : Conductor<object>, IPrescriptionDetailSchedulesNew
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public PrescriptionDetailSchedulesNewViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            allLieuDung = Globals.allPrescriptionDetailSchedulesLieuDung.ToObservableCollection();
        }

        //private double _SoNgayDung;
        //public double SoNgayDung
        //{
        //    get { return _SoNgayDung; }
        //    set
        //    {
        //        _SoNgayDung = value;
        //        NotifyOfPropertyChange(()=> SoNgayDung);
        //    }
        //}

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

        private ObservableCollection<PrescriptionDetailSchedulesLieuDung> _allLieuDung ;
        public ObservableCollection<PrescriptionDetailSchedulesLieuDung> allLieuDung
        {
            get { return _allLieuDung; }
            set
            {
                _allLieuDung = value;
                NotifyOfPropertyChange(() => allLieuDung);
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
                
                //PrescriptionDetailSchedules_ByPrescriptDetailID(0,false);

                //ObjPrescriptionDetailSchedules_ByPrescriptDetailID = new ObservableCollection<PrescriptionDetailSchedules>(Globals.blankPrescriptionDetailSchedules);
                ObjPrescriptionDetailSchedules_ByPrescriptDetailID = ObjectCopier.DeepCopy(Globals.blankPrescriptionDetailSchedules);
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
                    TongThuocPhaiDung = CalcTotalQtyForShedule(ObjPrescriptionDetailSchedules_ByPrescriptDetailID, ObjPrescriptionDetail, NDay);
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
                    TongThuocPhaiDung = Math.Ceiling(Weeks *
                    (TongCong * (ObjPrescriptionDetail.SelectedDrugForPrescription.UnitVolume == 0 ? 1 : ObjPrescriptionDetail.SelectedDrugForPrescription.UnitVolume))
                    / (ObjPrescriptionDetail.SelectedDrugForPrescription.DispenseVolume == 0 ? 1 : ObjPrescriptionDetail.SelectedDrugForPrescription.DispenseVolume));

                    double KQ = TongThuocPhaiDung;//Math.Ceiling((ObjPrescriptionDetail.SelectedDrugForPrescription.DayRpts * TongThuocPhaiDung) / d);
                    TongThuocPhaiDung = KQ;
                    //SoNgayDung = ObjPrescriptionDetail.SelectedDrugForPrescription.DayRpts;
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

        private PrescriptionDetailSchedules _curPrescriptionDetailSchedules;
        public PrescriptionDetailSchedules curPrescriptionDetailSchedules
        {
            get
            {
                return _curPrescriptionDetailSchedules;
            }
            set
            {
                if (_curPrescriptionDetailSchedules != value)
                {
                    _curPrescriptionDetailSchedules = value;
                    NotifyOfPropertyChange(() => curPrescriptionDetailSchedules);
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

                    contract.BeginPrescriptionDetailSchedules_ByPrescriptDetailID(PrescriptDetailID, IsNotIncat, Globals.DispatchCallback((asyncResult) =>
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

        private object _parentVM;

        public object ParentVM
        {
            get 
            {
                return _parentVM; 
            }
            set 
            {
                _parentVM = value; 
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
                //them lua chon tra ve form hay tra ve cho datagrid
                if (ObjPrescriptionDetail.isForm)
                {
                    ((IePrescriptionOldNew)ParentVM).HandleDrugScheduleForm(ObjPrescriptionDetailSchedules_ByPrescriptDetailID, d, ObjPrescriptionDetail.DrugInstructionNotes);

                    //Globals.EventAggregator.Publish(new SendPrescriptionDetailSchedulesFormEvent<ObservableCollection<PrescriptionDetailSchedules>, bool,double,double,string, int>()
                    //{
                    //    Data = ObjPrescriptionDetailSchedules_ByPrescriptDetailID,
                    //    HasSchedule = HasSchedule,
                    //    TongThuoc = TongThuocPhaiDung,
                    //    //SoNgayDung = ObjPrescriptionDetail.SelectedDrugForPrescription.DayRpts,
                    //    SoNgayDung = d,
                    //    GhiChu = ObjPrescriptionDetail.DrugInstructionNotes,
                    //    ModeForm = ModeForm
                    //});
                }
                else
                {
                    //==== 20161115 CMN Begin: Fix Drug Week Calendar
                    //((IePrescriptionOldNew)ParentVM).HandleDrugSchedule(ObjPrescriptionDetailSchedules_ByPrescriptDetailID, d, ObjPrescriptionDetail.DrugInstructionNotes);
                    if (ParentVM is IePrescriptionOldNew)
                        ((IePrescriptionOldNew)ParentVM).HandleDrugSchedule(ObjPrescriptionDetailSchedules_ByPrescriptDetailID, d, ObjPrescriptionDetail.DrugInstructionNotes);
                    else
                        ((IeInPrescriptionOldNew)ParentVM).HandleDrugSchedule(ObjPrescriptionDetailSchedules_ByPrescriptDetailID, d, ObjPrescriptionDetail.DrugInstructionNotes);
                    //==== 20161115 CMN End.
                    //Globals.EventAggregator.Publish(new SendPrescriptionDetailSchedulesEvent<ObservableCollection<PrescriptionDetailSchedules>, bool,double,double,string, int>()
                    //{
                    //    Data = ObjPrescriptionDetailSchedules_ByPrescriptDetailID,
                    //    HasSchedule = HasSchedule,
                    //    TongThuoc = TongThuocPhaiDung,
                    //    //SoNgayDung = ObjPrescriptionDetail.SelectedDrugForPrescription.DayRpts,
                    //    SoNgayDung = d,
                    //    GhiChu = ObjPrescriptionDetail.DrugInstructionNotes,
                    //    ModeForm = ModeForm
                    //});
                }
                
                TryClose();
            }

        }


        public static float CalcDrugQtyFor1PartOfDay(PrescriptionDetailSchedules ObjRow)/*Tổng Thuốc 1 Buổi: Sáng Or Trưa, Chiều, Tối*/
        {
            float TongThuoc1Buoi = 0;
                      
            if (ObjRow.Monday > 0)
            {
                TongThuoc1Buoi += ObjRow.Monday.Value;
            }
            if (ObjRow.Tuesday > 0)
            {
                TongThuoc1Buoi += ObjRow.Tuesday.Value;
            }
            if (ObjRow.Wednesday > 0)
            {
                TongThuoc1Buoi += ObjRow.Wednesday.Value;
            }
            if (ObjRow.Thursday > 0)
            {
                TongThuoc1Buoi += ObjRow.Thursday.Value;
            }
            if (ObjRow.Friday > 0)
            {
                TongThuoc1Buoi += ObjRow.Friday.Value;
            }
            if (ObjRow.Saturday > 0)
            {
                TongThuoc1Buoi += ObjRow.Saturday.Value;
            }
            if (ObjRow.Sunday > 0)
            {
                TongThuoc1Buoi += ObjRow.Sunday.Value;
            }

            return TongThuoc1Buoi;

        }

        public static void Check1RowExistsLess3Value(PrescriptionDetailSchedules ObjRow, out int SumSet, out float TongThuoc1Buoi)/*Tổng Thuốc 1 Buổi: Sáng Or Trưa, Chiều, Tối*/
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

        int IndexCol = 0;
        public void dtgList_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            IndexCol = e.Column.DisplayIndex;
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

        
        AutoCompleteBox acbLieDung { get; set; }
        AutoCompleteBox acbMonday { get; set; }
        AutoCompleteBox acbTuesday { get; set; }
        AutoCompleteBox acbWednesday { get; set; }
        AutoCompleteBox acbThurday { get; set; }
        AutoCompleteBox acbFriday { get; set; }
        AutoCompleteBox acbSaturday { get; set; }
        AutoCompleteBox acbSunday { get; set; }
        public void aucLieuDung_Loaded(object sender, RoutedEventArgs e)
        {
            acbLieDung = (AutoCompleteBox)sender;
            acbLieDung.ItemsSource = Globals.allPrescriptionDetailSchedulesLieuDung.ToObservableCollection();
        }
        public void aucMonday_Loaded(object sender, RoutedEventArgs e)
        {
            acbMonday = (AutoCompleteBox)sender;
            acbMonday.ItemsSource = Globals.allPrescriptionDetailSchedulesLieuDung.ToObservableCollection();
        }
        public void aucTuesday_Loaded(object sender, RoutedEventArgs e)
        {
            acbTuesday = (AutoCompleteBox)sender;
            acbTuesday.ItemsSource = Globals.allPrescriptionDetailSchedulesLieuDung.ToObservableCollection();
        }
        public void aucWednesday_Loaded(object sender, RoutedEventArgs e)
        {
            acbWednesday = (AutoCompleteBox)sender;
            acbWednesday.ItemsSource = Globals.allPrescriptionDetailSchedulesLieuDung.ToObservableCollection();
        }
        public void aucThurday_Loaded(object sender, RoutedEventArgs e)
        {
            acbThurday = (AutoCompleteBox)sender;
            acbThurday.ItemsSource = Globals.allPrescriptionDetailSchedulesLieuDung.ToObservableCollection();
        }
        public void aucFriday_Loaded(object sender, RoutedEventArgs e)
        {
            acbFriday = (AutoCompleteBox)sender;
            acbFriday.ItemsSource = Globals.allPrescriptionDetailSchedulesLieuDung.ToObservableCollection();
        }
        public void aucSaturday_Loaded(object sender, RoutedEventArgs e)
        {
            acbSaturday = (AutoCompleteBox)sender;
            acbSaturday.ItemsSource = Globals.allPrescriptionDetailSchedulesLieuDung.ToObservableCollection();
        }
        public void aucSunday_Loaded(object sender, RoutedEventArgs e)
        {
            acbSunday = (AutoCompleteBox)sender;
            acbSunday.ItemsSource = Globals.allPrescriptionDetailSchedulesLieuDung.ToObservableCollection();
        }

        
        //public void AxAutoComplete_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    PrescriptionDetailSchedules Objtmp = ((AutoCompleteBox)sender).DataContext as PrescriptionDetailSchedules;
        //    if (Objtmp != null)
        //    {
        //        Objtmp.ScheGen = new PrescriptionDetailSchedulesLieuDung(((AutoCompleteBox)sender).Text);
        //        SetDetailSchedules(Objtmp);
        //    }
        //}

                
        public void aucLieuDung_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            PrescriptionDetailSchedules Objtmp = ((AutoCompleteBox)sender).DataContext as PrescriptionDetailSchedules;
            if (Objtmp != null) 
            {
                if (((AutoCompleteBox)sender).Text.Length > 0)
                {
                    Objtmp.ScheGen = new PrescriptionDetailSchedulesLieuDung(((AutoCompleteBox)sender).Text);
                    SetDetailSchedules(Objtmp);
                }
            }
        }

        public void SetDetailSchedules(PrescriptionDetailSchedules Objtmp)
        {
            Objtmp.ScheMonday = new PrescriptionDetailSchedulesLieuDung(Objtmp.ScheGen);
            Objtmp.ScheTuesday = new PrescriptionDetailSchedulesLieuDung(Objtmp.ScheGen);
            Objtmp.ScheWednesday = new PrescriptionDetailSchedulesLieuDung(Objtmp.ScheGen);
            Objtmp.ScheThursday = new PrescriptionDetailSchedulesLieuDung(Objtmp.ScheGen);
            Objtmp.ScheFriday = new PrescriptionDetailSchedulesLieuDung(Objtmp.ScheGen);
            Objtmp.ScheSaturday = new PrescriptionDetailSchedulesLieuDung(Objtmp.ScheGen);
            Objtmp.ScheSunday = new PrescriptionDetailSchedulesLieuDung(Objtmp.ScheGen);            
            TongThuocPhaiDung = CalcTotalQtyForShedule(ObjPrescriptionDetailSchedules_ByPrescriptDetailID, ObjPrescriptionDetail, NDay);
        }

        public void aucMonday_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            acbMonday = (sender as AutoCompleteBox);
            if (acbMonday != null)
            {
                //if (!string.IsNullOrEmpty(acbMonday.Text))
                {
                    (((AxAutoComplete)sender).DataContext as PrescriptionDetailSchedules).ScheMonday = new PrescriptionDetailSchedulesLieuDung(acbMonday.Text);
                    //((AxAutoComplete)sender).SelectedItem = new PrescriptionDetailSchedulesLieuDung(acbMonday.Text);
                }
            }
            TongThuocPhaiDung = CalcTotalQtyForShedule(ObjPrescriptionDetailSchedules_ByPrescriptDetailID, ObjPrescriptionDetail, NDay);
        }

        public void aucTuesday_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            acbTuesday = (sender as AutoCompleteBox);
            if (acbTuesday != null)
            {
                //if (!string.IsNullOrEmpty(acbTuesday.Text))
                {
                    (((AxAutoComplete)sender).DataContext as PrescriptionDetailSchedules).ScheTuesday = new PrescriptionDetailSchedulesLieuDung(acbTuesday.Text);
                    //((AxAutoComplete)sender).SelectedItem = new PrescriptionDetailSchedulesLieuDung(acbTuesday.Text);
                    TongThuocPhaiDung = CalcTotalQtyForShedule(ObjPrescriptionDetailSchedules_ByPrescriptDetailID, ObjPrescriptionDetail, NDay);
                }
            }
        }

        public void aucWednesday_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            acbWednesday = (sender as AutoCompleteBox);
            if (acbWednesday != null)
            {
                //if (!string.IsNullOrEmpty(acbWednesday.Text))
                {
                    (((AxAutoComplete)sender).DataContext as PrescriptionDetailSchedules).ScheWednesday = new PrescriptionDetailSchedulesLieuDung(acbWednesday.Text);
                    //((AxAutoComplete)sender).SelectedItem = new PrescriptionDetailSchedulesLieuDung(acbWednesday.Text);
                    TongThuocPhaiDung = CalcTotalQtyForShedule(ObjPrescriptionDetailSchedules_ByPrescriptDetailID, ObjPrescriptionDetail, NDay);
                }
            }
        }

        public void aucThurday_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            acbThurday = (sender as AutoCompleteBox);
            if (acbThurday != null)
            {
                //if (!string.IsNullOrEmpty(acbThurday.Text))
                {
                    (((AxAutoComplete)sender).DataContext as PrescriptionDetailSchedules).ScheThursday = new PrescriptionDetailSchedulesLieuDung(acbThurday.Text);
                    //((AxAutoComplete)sender).SelectedItem = new PrescriptionDetailSchedulesLieuDung(acbThurday.Text);
                    TongThuocPhaiDung = CalcTotalQtyForShedule(ObjPrescriptionDetailSchedules_ByPrescriptDetailID, ObjPrescriptionDetail, NDay);
                }
            }
        }

        public void aucFriday_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            acbFriday = (sender as AutoCompleteBox);
            if (acbFriday != null)
            {
                //if (!string.IsNullOrEmpty(acbFriday.Text))
                {
                    (((AxAutoComplete)sender).DataContext as PrescriptionDetailSchedules).ScheFriday = new PrescriptionDetailSchedulesLieuDung(acbFriday.Text);
                    //((AxAutoComplete)sender).SelectedItem = new PrescriptionDetailSchedulesLieuDung(acbFriday.Text);
                    TongThuocPhaiDung = CalcTotalQtyForShedule(ObjPrescriptionDetailSchedules_ByPrescriptDetailID, ObjPrescriptionDetail, NDay);
                }
            }
        }

        public void aucSaturday_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            acbSaturday = (sender as AutoCompleteBox);
            if (acbSaturday != null)
            {
                //if (!string.IsNullOrEmpty(acbSaturday.Text))
                {
                    (((AxAutoComplete)sender).DataContext as PrescriptionDetailSchedules).ScheSaturday = new PrescriptionDetailSchedulesLieuDung(acbSaturday.Text);
                    //((AxAutoComplete)sender).SelectedItem = new PrescriptionDetailSchedulesLieuDung(acbSaturday.Text);
                    TongThuocPhaiDung = CalcTotalQtyForShedule(ObjPrescriptionDetailSchedules_ByPrescriptDetailID, ObjPrescriptionDetail, NDay);
                }
            }
        }

        public void aucSunday_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            acbSunday = (sender as AutoCompleteBox);
            if (acbSunday != null)
            {
                //if (!string.IsNullOrEmpty(acbSunday.Text))
                {
                    (((AxAutoComplete)sender).DataContext as PrescriptionDetailSchedules).ScheSunday = new PrescriptionDetailSchedulesLieuDung(acbSunday.Text);
                    //((AxAutoComplete)sender).SelectedItem = new PrescriptionDetailSchedulesLieuDung(acbSunday.Text);
                    TongThuocPhaiDung = CalcTotalQtyForShedule(ObjPrescriptionDetailSchedules_ByPrescriptDetailID, ObjPrescriptionDetail, NDay);
                }
            }
        }


        public void aucLieuDung_LostFocus(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            PrescriptionDetailSchedules Objtmp = ((AutoCompleteBox)sender).DataContext as PrescriptionDetailSchedules;
            if (Objtmp != null)
            {
                if (((AutoCompleteBox)sender).Text.Length > 0)
                {
                    Objtmp.ScheGen = new PrescriptionDetailSchedulesLieuDung(((AutoCompleteBox)sender).Text);
                    SetDetailSchedules(Objtmp);
                }
            }
        }

        private void AdjustQtyOfDay(PrescriptionDetailSchedules Objtmp, int nDayOfWeek, Nullable<Single> sQty, string strQty)
        {
            switch (nDayOfWeek)
            {
                case 2:
                    if (Objtmp.ScheMonday != null)
                    {
                        Objtmp.ScheMonday.ID = sQty.GetValueOrDefault();
                        Objtmp.ScheMonday.Name = strQty;
                    }
                    Objtmp.Monday = sQty;
                    Objtmp.MondayStr = strQty;
                    break;
                case 3:
                    if (Objtmp.ScheTuesday != null)
                    {
                        Objtmp.ScheTuesday.ID = sQty.GetValueOrDefault();
                        Objtmp.ScheTuesday.Name = strQty;
                    }
                    Objtmp.Tuesday = sQty;
                    Objtmp.TuesdayStr = strQty;
                    break;
                case 4:
                    if (Objtmp.ScheWednesday != null)
                    {
                        Objtmp.ScheWednesday.ID = sQty.GetValueOrDefault();
                        Objtmp.ScheWednesday.Name = strQty;
                    }
                    Objtmp.Wednesday = sQty;
                    Objtmp.WednesdayStr = strQty;
                    break;
                case 5:
                    if (Objtmp.ScheThursday != null)
                    {
                        Objtmp.ScheThursday.ID = sQty.GetValueOrDefault();
                        Objtmp.ScheThursday.Name = strQty;
                    }
                    Objtmp.Thursday = sQty;
                    Objtmp.ThursdayStr = strQty;
                    break;
                case 6:
                    if (Objtmp.ScheFriday != null)
                    {
                        Objtmp.ScheFriday.ID = sQty.GetValueOrDefault();
                        Objtmp.ScheFriday.Name = strQty;
                    }
                    Objtmp.Friday = sQty;
                    Objtmp.FridayStr = strQty;
                    break;
                case 7:
                    if (Objtmp.ScheSaturday != null)
                    {
                        Objtmp.ScheSaturday.ID = sQty.GetValueOrDefault();
                        Objtmp.ScheSaturday.Name = strQty;
                    }
                    Objtmp.Saturday = sQty;
                    Objtmp.SaturdayStr = strQty;
                    break;
                case 8:
                    if (Objtmp.ScheSunday != null)
                    {
                        Objtmp.ScheSunday.ID = sQty.GetValueOrDefault();
                        Objtmp.ScheSunday.Name = strQty;
                    }
                    Objtmp.Sunday = sQty;
                    Objtmp.SundayStr = strQty;
                    break;
            }
        }


        public void aucMonday_LostFocus(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            PrescriptionDetailSchedules Objtmp = ((AutoCompleteBox)sender).DataContext as PrescriptionDetailSchedules;
            acbMonday = (sender as AutoCompleteBox);
            if (acbMonday != null)
            {
                //if (!string.IsNullOrEmpty(acbMonday.Text))
                {
                    PrescriptionDetailSchedulesLieuDung theLD = new PrescriptionDetailSchedulesLieuDung(acbMonday.Text);
                    ((AxAutoComplete)sender).SelectedItem = theLD;
                    
                    AdjustQtyOfDay(Objtmp, 2, theLD.ID, theLD.Name);
                    TongThuocPhaiDung = CalcTotalQtyForShedule(ObjPrescriptionDetailSchedules_ByPrescriptDetailID, ObjPrescriptionDetail, NDay);
                }
            }
            
        }

        public void aucTuesday_LostFocus(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            PrescriptionDetailSchedules Objtmp = ((AutoCompleteBox)sender).DataContext as PrescriptionDetailSchedules;
            acbTuesday = (sender as AutoCompleteBox);
            if (acbTuesday != null)
            {
                //if (!string.IsNullOrEmpty(acbTuesday.Text))
                {
                    PrescriptionDetailSchedulesLieuDung theLD = new PrescriptionDetailSchedulesLieuDung(acbTuesday.Text);
                    ((AxAutoComplete)sender).SelectedItem = theLD;

                    AdjustQtyOfDay(Objtmp, 3, theLD.ID, theLD.Name);
                    TongThuocPhaiDung = CalcTotalQtyForShedule(ObjPrescriptionDetailSchedules_ByPrescriptDetailID, ObjPrescriptionDetail, NDay);
                }
            }
        }

        public void aucWednesday_LostFocus(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            PrescriptionDetailSchedules Objtmp = ((AutoCompleteBox)sender).DataContext as PrescriptionDetailSchedules;
            acbWednesday = (sender as AutoCompleteBox);
            if (acbWednesday != null)
            {
                //if (!string.IsNullOrEmpty(acbWednesday.Text))
                {
                    PrescriptionDetailSchedulesLieuDung theLD = new PrescriptionDetailSchedulesLieuDung(acbWednesday.Text);
                    ((AxAutoComplete)sender).SelectedItem = theLD;

                    AdjustQtyOfDay(Objtmp, 4, theLD.ID, theLD.Name);
                    TongThuocPhaiDung = CalcTotalQtyForShedule(ObjPrescriptionDetailSchedules_ByPrescriptDetailID, ObjPrescriptionDetail, NDay);
                }
            }
        }

        public void aucThurday_LostFocus(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            PrescriptionDetailSchedules Objtmp = ((AutoCompleteBox)sender).DataContext as PrescriptionDetailSchedules;
            acbThurday = (sender as AutoCompleteBox);
            if (acbThurday != null)
            {
                //if (!string.IsNullOrEmpty(acbThurday.Text))
                {
                    PrescriptionDetailSchedulesLieuDung theLD = new PrescriptionDetailSchedulesLieuDung(acbThurday.Text);
                    ((AxAutoComplete)sender).SelectedItem = theLD;

                    AdjustQtyOfDay(Objtmp, 5, theLD.ID, theLD.Name);
                    TongThuocPhaiDung = CalcTotalQtyForShedule(ObjPrescriptionDetailSchedules_ByPrescriptDetailID, ObjPrescriptionDetail, NDay);
                }
            }
        }

        public void aucFriday_LostFocus(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            PrescriptionDetailSchedules Objtmp = ((AutoCompleteBox)sender).DataContext as PrescriptionDetailSchedules;
            acbFriday = (sender as AutoCompleteBox);
            if (acbFriday != null)
            {
                //if (!string.IsNullOrEmpty(acbFriday.Text))
                {
                    PrescriptionDetailSchedulesLieuDung theLD = new PrescriptionDetailSchedulesLieuDung(acbFriday.Text);
                    ((AxAutoComplete)sender).SelectedItem = theLD;

                    AdjustQtyOfDay(Objtmp, 6, theLD.ID, theLD.Name);
                    TongThuocPhaiDung = CalcTotalQtyForShedule(ObjPrescriptionDetailSchedules_ByPrescriptDetailID, ObjPrescriptionDetail, NDay);
                }
            }
        }

        public void aucSaturday_LostFocus(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            PrescriptionDetailSchedules Objtmp = ((AutoCompleteBox)sender).DataContext as PrescriptionDetailSchedules;
            acbSaturday = (sender as AutoCompleteBox);
            if (acbSaturday != null)
            {
                //if (!string.IsNullOrEmpty(acbSaturday.Text))
                {
                    PrescriptionDetailSchedulesLieuDung theLD = new PrescriptionDetailSchedulesLieuDung(acbSaturday.Text);
                    ((AxAutoComplete)sender).SelectedItem = theLD;

                    AdjustQtyOfDay(Objtmp, 7, theLD.ID, theLD.Name);
                    TongThuocPhaiDung = CalcTotalQtyForShedule(ObjPrescriptionDetailSchedules_ByPrescriptDetailID, ObjPrescriptionDetail, NDay);
                }
            }
        }

        public void aucSunday_LostFocus(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            PrescriptionDetailSchedules Objtmp = ((AutoCompleteBox)sender).DataContext as PrescriptionDetailSchedules;
            acbSunday = (sender as AutoCompleteBox);
            if (acbSunday != null)
            {
                //if (!string.IsNullOrEmpty(acbSunday.Text))
                {
                    PrescriptionDetailSchedulesLieuDung theLD = new PrescriptionDetailSchedulesLieuDung(acbSunday.Text);
                    ((AxAutoComplete)sender).SelectedItem = theLD;

                    AdjustQtyOfDay(Objtmp, 8, theLD.ID, theLD.Name);
                    TongThuocPhaiDung = CalcTotalQtyForShedule(ObjPrescriptionDetailSchedules_ByPrescriptDetailID, ObjPrescriptionDetail, NDay);
                }
            }
        }

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
                Objtemp.ScheGen = new PrescriptionDetailSchedulesLieuDung("0");
                Objtemp.ScheMonday = new PrescriptionDetailSchedulesLieuDung("0");
                Objtemp.ScheTuesday = new PrescriptionDetailSchedulesLieuDung("0");
                Objtemp.ScheWednesday = new PrescriptionDetailSchedulesLieuDung("0");
                Objtemp.ScheThursday = new PrescriptionDetailSchedulesLieuDung("0");
                Objtemp.ScheFriday = new PrescriptionDetailSchedulesLieuDung("0");
                Objtemp.ScheSaturday = new PrescriptionDetailSchedulesLieuDung("0");
                Objtemp.ScheSunday= new PrescriptionDetailSchedulesLieuDung("0");
                
            }
        }

        public static double CalcTotalQtyForShedule(ObservableCollection<PrescriptionDetailSchedules> ObjPrescriptionDetailSchedules, PrescriptionDetail objPrescriptionDetail, int nTotPrescriptionDays)
        {
            if (objPrescriptionDetail == null || ObjPrescriptionDetailSchedules.Count == 0 || nTotPrescriptionDays <= 0)
            {
                return 0;
            }

            int nTotDayInWeeks = nTotPrescriptionDays % 7 == 0 ? nTotPrescriptionDays / 7 : nTotPrescriptionDays / 7 + 1;

            PrescriptionDetailSchedules ObjRowSang = ObjPrescriptionDetailSchedules[0];
            PrescriptionDetailSchedules ObjRowTrua = ObjPrescriptionDetailSchedules[1];
            PrescriptionDetailSchedules ObjRowChieu = ObjPrescriptionDetailSchedules[2];
            PrescriptionDetailSchedules ObjRowToi = ObjPrescriptionDetailSchedules[3];

            float TongThuocOfRowSang = 0;
            float TongThuocOfRowTrua = 0;
            float TongThuocOfRowChieu = 0;
            float TongThuocOfRowToi = 0;
            double TongCong = 0;

            TongThuocOfRowSang  = CalcDrugQtyFor1PartOfDay(ObjRowSang);
            TongThuocOfRowTrua  = CalcDrugQtyFor1PartOfDay(ObjRowTrua);
            TongThuocOfRowChieu = CalcDrugQtyFor1PartOfDay(ObjRowChieu);
            TongThuocOfRowToi   = CalcDrugQtyFor1PartOfDay(ObjRowToi);

            TongCong = (TongThuocOfRowSang + TongThuocOfRowTrua + TongThuocOfRowChieu + TongThuocOfRowToi);

            double fTongThuocPhaiDung = Math.Ceiling(nTotDayInWeeks *
                (TongCong * (objPrescriptionDetail.SelectedDrugForPrescription.UnitVolume == 0 ? 1 : objPrescriptionDetail.SelectedDrugForPrescription.UnitVolume))
                / (objPrescriptionDetail.SelectedDrugForPrescription.DispenseVolume == 0 ? 1 : objPrescriptionDetail.SelectedDrugForPrescription.DispenseVolume));

            return fTongThuocPhaiDung;
        }

        public static int RoundUpUsageDayOfWeeklySchedule(int nDays)
        {
            int nWeeks = nDays % 7 == 0 ? nDays / 7 : nDays / 7 + 1;
            return 7 * nWeeks;
        }

        private double CalcTongSoNgayDungThuocTrongLich()
        {
            return 7 * Weeks;
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
                    // Txd
                    //TinhTongThuoc();
                    TongThuocPhaiDung = CalcTotalQtyForShedule(ObjPrescriptionDetailSchedules_ByPrescriptDetailID, ObjPrescriptionDetail, NDay);
                }
            }
            catch(Exception ex)
            {
                ClientLoggerHelper.LogInfo(ex.ToString());
                MessageBox.Show(string.Format("{0}.", eHCMSResources.A0559_G1_Msg_InfoGTriKhHopLe));
            }

        }
    }
}


//private double TinhTongThuocNew()
//{
//    //double d = CalcTongSoNgayDungThuocTrongLich();

//    //ObjPrescriptionDetail.SelectedDrugForPrescription.DayRpts = d;
//    //SoNgayDung = d;

//    PrescriptionDetailSchedules ObjRowSang = ObjPrescriptionDetailSchedules_ByPrescriptDetailID[0];
//    PrescriptionDetailSchedules ObjRowTrua = ObjPrescriptionDetailSchedules_ByPrescriptDetailID[1];
//    PrescriptionDetailSchedules ObjRowChieu = ObjPrescriptionDetailSchedules_ByPrescriptDetailID[2];
//    PrescriptionDetailSchedules ObjRowToi = ObjPrescriptionDetailSchedules_ByPrescriptDetailID[3];

//    int SumSang = 0;
//    int SumTrua = 0;
//    int SumChieu = 0;
//    int SumToi = 0;

//    float TongThuocOfRowSang = 0;
//    float TongThuocOfRowTrua = 0;
//    float TongThuocOfRowChieu = 0;
//    float TongThuocOfRowToi = 0;
//    double TongCong = 0;

//    Check1RowExistsLess3Value(ObjRowSang, out SumSang, out TongThuocOfRowSang);
//    Check1RowExistsLess3Value(ObjRowTrua, out SumTrua, out TongThuocOfRowTrua);
//    Check1RowExistsLess3Value(ObjRowChieu, out SumChieu, out TongThuocOfRowChieu);
//    Check1RowExistsLess3Value(ObjRowToi, out SumToi, out TongThuocOfRowToi);

//    TongCong = (TongThuocOfRowSang + TongThuocOfRowTrua + TongThuocOfRowChieu + TongThuocOfRowToi);

//    TongThuocPhaiDung = Math.Ceiling(Weeks *
//        (TongCong * (ObjPrescriptionDetail.SelectedDrugForPrescription.UnitVolume == 0 ? 1 : ObjPrescriptionDetail.SelectedDrugForPrescription.UnitVolume))
//        / (ObjPrescriptionDetail.SelectedDrugForPrescription.DispenseVolume == 0 ? 1 : ObjPrescriptionDetail.SelectedDrugForPrescription.DispenseVolume));

//    return TongThuocPhaiDung;

//}


//private double TinhTongThuoc()
//{
//    double d= CalcTongSoNgayDungThuocTrongLich();

//    ObjPrescriptionDetail.SelectedDrugForPrescription.DayRpts = d;
//    SoNgayDung = d;

//    PrescriptionDetailSchedules ObjRowSang = ObjPrescriptionDetailSchedules_ByPrescriptDetailID[0];
//    PrescriptionDetailSchedules ObjRowTrua = ObjPrescriptionDetailSchedules_ByPrescriptDetailID[1];
//    PrescriptionDetailSchedules ObjRowChieu = ObjPrescriptionDetailSchedules_ByPrescriptDetailID[2];
//    PrescriptionDetailSchedules ObjRowToi = ObjPrescriptionDetailSchedules_ByPrescriptDetailID[3];

//    int SumSang = 0;
//    int SumTrua = 0;
//    int SumChieu = 0;
//    int SumToi = 0;

//    float TongThuocOfRowSang = 0;
//    float TongThuocOfRowTrua = 0;
//    float TongThuocOfRowChieu = 0;
//    float TongThuocOfRowToi = 0;
//    double TongCong = 0;

//    Check1RowExistsLess3Value(ObjRowSang, out SumSang, out TongThuocOfRowSang);
//    Check1RowExistsLess3Value(ObjRowTrua, out SumTrua, out TongThuocOfRowTrua);
//    Check1RowExistsLess3Value(ObjRowChieu, out SumChieu, out TongThuocOfRowChieu);
//    Check1RowExistsLess3Value(ObjRowToi, out SumToi, out TongThuocOfRowToi);

//    TongCong = (TongThuocOfRowSang + TongThuocOfRowTrua + TongThuocOfRowChieu + TongThuocOfRowToi);

//    //if (ObjPrescriptionDetail.IsDrugNotInCat == false)
//    {
//        TongThuocPhaiDung = Math.Ceiling(Weeks *
//            (TongCong * (ObjPrescriptionDetail.SelectedDrugForPrescription.UnitVolume == 0 ? 1 : ObjPrescriptionDetail.SelectedDrugForPrescription.UnitVolume))
//            / (ObjPrescriptionDetail.SelectedDrugForPrescription.DispenseVolume == 0 ? 1 : ObjPrescriptionDetail.SelectedDrugForPrescription.DispenseVolume));
//    }
//    //else
//    //{
//    //    TongThuocPhaiDung = Math.Ceiling(Weeks * TongCong);
//    //}
//    return TongThuocPhaiDung = TongThuocPhaiDung ;
//}




