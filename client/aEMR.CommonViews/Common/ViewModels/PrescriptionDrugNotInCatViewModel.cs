using System.Linq;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using DataEntities;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using System;
using System.Text;
using aEMR.Controls;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Text.RegularExpressions;
using System.Globalization;
using eHCMSLanguage;
using aEMR.Common.BaseModel;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IPrescriptionDrugNotInCat)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PrescriptionDrugNotInCatViewModel : ViewModelBase, IPrescriptionDrugNotInCat
    {
        private int xNgayBHToiDa_NgoaiTru = 30;
        private int xNgayBHToiDa_NoiTru = 5;


        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public PrescriptionDrugNotInCatViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;


            //if (Globals.ConfigList != null)
            //{
            //    xNgayBHToiDa_NgoaiTru = Convert.ToInt32(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.PharmacyMaxDaysHIRebate_NgoaiTru]);
            //    xNgayBHToiDa_NoiTru = Convert.ToInt32(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.PharmacyMaxDaysHIRebate_NoiTru]);
            //}

            // Txd 25/05/2014 Replaced ConfigList
            xNgayBHToiDa_NgoaiTru = Globals.ServerConfigSection.HealthInsurances.PharmacyMaxDaysHIRebate_NoiTru;
            xNgayBHToiDa_NoiTru = Globals.ServerConfigSection.HealthInsurances.PharmacyMaxDaysHIRebate_NoiTru;

        }


        private int _IndexRow;
        public int IndexRow
        {
            get
            {
                return _IndexRow;
            }
            set
            {
                if (_IndexRow != value)
                {
                    _IndexRow = value;
                    NotifyOfPropertyChange(() => IndexRow);
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

        private ObservableCollection<ChooseDose> _ChooseDoses;
        public ObservableCollection<ChooseDose> ChooseDoses
        {
            get
            {
                return _ChooseDoses;
            }
            set
            {
                if (_ChooseDoses != value)
                {
                    _ChooseDoses = value;
                    NotifyOfPropertyChange(() => ChooseDoses);
                }
            }
        }

        private ChooseDose _ChooseDoseSelected;
        public ChooseDose ChooseDoseSelected
        {
            get
            {
                return _ChooseDoseSelected;
            }
            set
            {
                if (_ChooseDoseSelected != value)
                {
                    _ChooseDoseSelected = value;
                    NotifyOfPropertyChange(() => ChooseDoseSelected);

                    if(ChooseDoseSelected!=null && ChooseDoseSelected.ID>0)
                    {   
                        SetValueFollowComboDose(ChooseDoseSelected, ObjPrescriptionDetail);
                        if (ObjPrescriptionDetail != null && ObjPrescriptionDetail.DayRpts > 0)
                        {
                            SetValueFollowNgayDung(ObjPrescriptionDetail);
                        }
                        
                    }
                }
            }
        }

        private void SetValueFollowComboDose(ChooseDose ObjChooseDose, PrescriptionDetail Objtmp)
        {
            if (Objtmp != null)
            {
                if (ObjChooseDose != null)
                {
                    switch (ObjChooseDose.ID)
                    {
                        case 1:
                            Objtmp.MDose = Objtmp.dosage;
                            Objtmp.ADose = Objtmp.dosage;
                            Objtmp.EDose = Objtmp.dosage;
                            Objtmp.NDose = Objtmp.dosage;

                            Objtmp.MDoseStr = Objtmp.dosageStr;
                            Objtmp.ADoseStr = Objtmp.dosageStr;
                            Objtmp.EDoseStr = Objtmp.dosageStr;
                            Objtmp.NDoseStr = Objtmp.dosageStr;
                            break;
                        case 2:
                            {
                                Objtmp.MDose = Objtmp.dosage;
                                Objtmp.ADose = Objtmp.dosage;
                                Objtmp.EDose = Objtmp.dosage;
                                Objtmp.NDose = 0;

                                Objtmp.MDoseStr = Objtmp.dosageStr;
                                Objtmp.ADoseStr = Objtmp.dosageStr;
                                Objtmp.EDoseStr = Objtmp.dosageStr;
                                Objtmp.NDoseStr = "0";
                                break;
                            }
                        case 3:
                            Objtmp.MDose = Objtmp.dosage;
                            Objtmp.ADose = Objtmp.dosage;
                            Objtmp.EDose = 0;
                            Objtmp.NDose = Objtmp.dosage;

                            Objtmp.MDoseStr = Objtmp.dosageStr;
                            Objtmp.ADoseStr = Objtmp.dosageStr;
                            Objtmp.EDoseStr = "0";
                            Objtmp.NDoseStr = Objtmp.dosageStr;
                            double s = Convert.ToDouble(Objtmp.dosageStr,new CultureInfo("en-US"));
                            break;
                        case 4:
                            Objtmp.MDose = Objtmp.dosage;
                            Objtmp.ADose = 0;
                            Objtmp.EDose = Objtmp.dosage;
                            Objtmp.NDose = Objtmp.dosage;

                            Objtmp.MDoseStr = Objtmp.dosageStr;
                            Objtmp.ADoseStr = "0";
                            Objtmp.EDoseStr = Objtmp.dosageStr;
                            Objtmp.NDoseStr = Objtmp.dosageStr;
                            break;
                        case 5:
                            Objtmp.MDose = 0;
                            Objtmp.ADose = Objtmp.dosage;
                            Objtmp.EDose = Objtmp.dosage;
                            Objtmp.NDose = Objtmp.dosage;

                            Objtmp.MDoseStr = "0";
                            Objtmp.ADoseStr = Objtmp.dosageStr;
                            Objtmp.EDoseStr = Objtmp.dosageStr;
                            Objtmp.NDoseStr = Objtmp.dosageStr;
                            break;
                        case 6:
                            Objtmp.MDose = Objtmp.dosage;
                            Objtmp.ADose = Objtmp.dosage;
                            Objtmp.EDose = 0;
                            Objtmp.NDose = 0;

                            Objtmp.MDoseStr = Objtmp.dosageStr;
                            Objtmp.ADoseStr = Objtmp.dosageStr;
                            Objtmp.EDoseStr = "0";
                            Objtmp.NDoseStr = "0";
                            break;
                        case 7:
                            Objtmp.MDose = Objtmp.dosage;
                            Objtmp.ADose = 0;
                            Objtmp.EDose = Objtmp.dosage;
                            Objtmp.NDose = 0;

                            Objtmp.MDoseStr = Objtmp.dosageStr;
                            Objtmp.ADoseStr = "0";
                            Objtmp.EDoseStr = Objtmp.dosageStr;
                            Objtmp.NDoseStr = "0";
                            break;
                        case 8:
                            Objtmp.MDose = Objtmp.dosage;
                            Objtmp.ADose = 0;
                            Objtmp.EDose = 0;
                            Objtmp.NDose = Objtmp.dosage;

                            Objtmp.MDoseStr = Objtmp.dosageStr;
                            Objtmp.ADoseStr = "0";
                            Objtmp.EDoseStr = "0";
                            Objtmp.NDoseStr = Objtmp.dosageStr;
                            break;
                        case 9:
                            Objtmp.MDose = 0;
                            Objtmp.ADose = Objtmp.dosage;
                            Objtmp.EDose = Objtmp.dosage;
                            Objtmp.NDose = 0;

                            Objtmp.MDoseStr = "0";
                            Objtmp.ADoseStr = Objtmp.dosageStr;
                            Objtmp.EDoseStr = Objtmp.dosageStr;
                            Objtmp.NDoseStr = "0";
                            break;
                        case 10:
                            Objtmp.MDose = 0;
                            Objtmp.ADose = Objtmp.dosage;
                            Objtmp.EDose = 0;
                            Objtmp.NDose = Objtmp.dosage;

                            Objtmp.MDoseStr = "0";
                            Objtmp.ADoseStr = Objtmp.dosageStr;
                            Objtmp.EDoseStr = "0";
                            Objtmp.NDoseStr = Objtmp.dosageStr;
                            break;
                        case 11:
                            Objtmp.MDose = 0;
                            Objtmp.ADose = 0;
                            Objtmp.EDose = Objtmp.dosage;
                            Objtmp.NDose = Objtmp.dosage;

                            Objtmp.MDoseStr = "0";
                            Objtmp.ADoseStr = "0";
                            Objtmp.EDoseStr = Objtmp.dosageStr;
                            Objtmp.NDoseStr = Objtmp.dosageStr;
                            break;
                        case 12:
                            Objtmp.MDose = Objtmp.dosage;
                            Objtmp.ADose = 0;
                            Objtmp.EDose = 0;
                            Objtmp.NDose = 0;

                            Objtmp.MDoseStr = Objtmp.dosageStr;
                            Objtmp.ADoseStr = "0";
                            Objtmp.EDoseStr = "0";
                            Objtmp.NDoseStr = "0";
                            break;
                        case 13:
                            Objtmp.MDose = 0;
                            Objtmp.ADose = Objtmp.dosage;
                            Objtmp.EDose = 0;
                            Objtmp.NDose = 0;

                            Objtmp.MDoseStr = "0";
                            Objtmp.ADoseStr = Objtmp.dosageStr;
                            Objtmp.EDoseStr = "0";
                            Objtmp.NDoseStr = "0";
                            break;
                        case 14:
                            Objtmp.MDose = 0;
                            Objtmp.ADose = 0;
                            Objtmp.EDose = Objtmp.dosage;
                            Objtmp.NDose = 0;

                            Objtmp.MDoseStr = "0";
                            Objtmp.ADoseStr = "0";
                            Objtmp.EDoseStr = Objtmp.dosageStr;
                            Objtmp.NDoseStr = "0";
                            break;
                        case 15:
                            Objtmp.MDose = 0;
                            Objtmp.ADose = 0;
                            Objtmp.EDose = 0;
                            Objtmp.NDose = Objtmp.dosage;

                            Objtmp.MDoseStr = "0";
                            Objtmp.ADoseStr = "0";
                            Objtmp.EDoseStr = "0";
                            Objtmp.NDoseStr = Objtmp.dosageStr;
                            break;
                    }

                }
            }
        }

        private void SetValueFollowNgayDung(PrescriptionDetail Objtmp)
        {
            Nullable<float> TongThuoc = 0;
            float Tong = 0;

            if (Objtmp != null && Objtmp.SelectedDrugForPrescription!=null)
            {
                TongThuoc = Objtmp.MDose + Objtmp.ADose.GetValueOrDefault() + Objtmp.NDose.GetValueOrDefault() + Objtmp.EDose.GetValueOrDefault();
                Tong = (float)(TongThuoc.Value * (Objtmp.DayRpts + Objtmp.DayExtended));
                Objtmp.Qty = Math.Ceiling(Tong);
            }
        }
        

        public void btSave()
        {
            if (CheckValid())
                return;
            
            ObjPrescriptionDetail.IsDrugNotInCat = true;

            Globals.EventAggregator.Publish(new PrescriptionDrugNotInCatSelectedEvent<PrescriptionDetail, int>() { PrescriptionDrugNotInCat = ObjPrescriptionDetail, Index = IndexRow });

            TryClose();
            
        }

        private bool CheckValid()
        {
            bool error = false;

            StringBuilder sb=new StringBuilder();
            sb.AppendLine("Các Mục Sau Phải Được Nhập Liệu:");

            if(string.IsNullOrEmpty(ObjPrescriptionDetail.SelectedDrugForPrescription.BrandName))
            {
                error = true;
                sb.AppendLine("- Nhập Tên Thuốc!");
            }
            if (ObjPrescriptionDetail.HasSchedules)
            {
                return error || false;
            }
            //if (string.IsNullOrEmpty(ObjPrescriptionDetail.SelectedDrugForPrescription.Administration))
            //{
            //    error = true;
            //    sb.AppendLine("- Nhập Cách Dùng!");
            //}

            //if (string.IsNullOrEmpty(ObjPrescriptionDetail.SelectedDrugForPrescription.UnitName))
            //{
            //    error = true;
            //    sb.AppendLine("- Nhập Đơn Vị Tính!");
            //}
            
            //if (string.IsNullOrEmpty(ObjPrescriptionDetail.SelectedDrugForPrescription.UnitUse))
            //{
            //    error = true;
            //    sb.AppendLine("- Nhập Đơn Vị Dùng!");
            //}

            if(  
                (
                (ObjPrescriptionDetail.MDoseStr==null)
                && (ObjPrescriptionDetail.ADoseStr==null)
                && (ObjPrescriptionDetail.EDoseStr==null)
                && (ObjPrescriptionDetail.NDoseStr==null)
                )

                //||
                //(
                //(float.Parse(ObjPrescriptionDetail.MDoseStr) ==0 )
                //&& (float.Parse(ObjPrescriptionDetail.ADoseStr) ==0 )
                //&& (float.Parse(ObjPrescriptionDetail.EDoseStr) ==0 )
                //&& (float.Parse(ObjPrescriptionDetail.NDoseStr) ==0 )
                //)

                ||
                (
                (ObjPrescriptionDetail.MDose == 0)
                && (ObjPrescriptionDetail.ADose == 0)
                && (ObjPrescriptionDetail.EDose == 0)
                && (ObjPrescriptionDetail.NDose == 0)
                )

                )
            {
                error = true;
                sb.AppendLine("- Sáng; Trưa; Chiều; Tối Ít Nhất Phải Chọn 1!");
            }

            if (ObjPrescriptionDetail.DayRpts<1)
            {
                error = true;
                sb.AppendLine(string.Format(" - {0}", eHCMSResources.A0836_G1_Msg_InfoNgDungKhHopLe));
            }
            if (ObjPrescriptionDetail.Qty <1)
            {
                error = true;
                sb.AppendLine(string.Format(" - {0}", eHCMSResources.Z1438_G1_SLgKgHopLe));
            }
            
            //if (CheckQtyLessThanQtyAutoCalc(ObjPrescriptionDetail))
            //{
            //    error = true;
            //    sb.AppendLine(" - Cần Kiểm Tra Lại Số Lượng! " + "Số lượng < Tổng số lượng thành phần");
            //}

            if (error)
            {
                MessageBox.Show(sb.ToString(), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
            
            return error;

        }

        public void btCancel()
        {
            TryClose();
        }


        private bool CheckQtyLessThanQtyAutoCalc(PrescriptionDetail Objtmp)
        {
            if (Objtmp != null && Objtmp.IsDrugNotInCat == false)
            {
                if (Objtmp.HasSchedules == false)
                {
                    Nullable<float> TongThuoc = 0;
                    float Tong = 0;

                    if (Objtmp != null)
                    {
                        TongThuoc = Objtmp.MDose + Objtmp.ADose.GetValueOrDefault() + Objtmp.NDose.GetValueOrDefault() + Objtmp.EDose.GetValueOrDefault();
                        Tong = (float)(TongThuoc.Value * (Objtmp.DayRpts + Objtmp.DayExtended) * Objtmp.SelectedDrugForPrescription.UnitVolume) / (float)Objtmp.SelectedDrugForPrescription.DispenseVolume;

                        if (Objtmp.Qty < Math.Ceiling(Tong))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void ChangeDosage(string value)
        {
            PrescriptionDetail Objtmp = this.ObjPrescriptionDetail;
            if (Objtmp != null)
            {
                Objtmp.dosage = ChangeDoseStringToFloat(Objtmp.dosageStr);
                if (Objtmp.dosage == 0)
                {
                    Objtmp.dosageStr = "0";
                }
                SetValueFollowComboDose(Objtmp.ChooseDose, Objtmp);
                SetValueFollowNgayDung(Objtmp);
            }
        }

        public void tbLieuDung_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox CtrLieuDung = (sender as TextBox);
            ChangeDosage(CtrLieuDung.Text);
        }

        public void cbxChooseDose_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            KeyEnabledComboBox Ctr = (sender as KeyEnabledComboBox);
            if (Ctr != null)
            {
                if (Ctr.SelectedItemEx != null)
                {
                    PrescriptionDetail Objtmp = (Ctr.DataContext) as PrescriptionDetail;

                    ChooseDose ObjChooseDose = Ctr.SelectedItemEx as ChooseDose;

                    ObjPrescriptionDetail.ChooseDose = ObjChooseDose;

                    if (ObjChooseDose.ID > 0)
                    {
                        SetValueFollowComboDose(ObjChooseDose, Objtmp);
                        if (Objtmp != null && Objtmp.DayRpts > 0)
                        {
                            SetValueFollowNgayDung(Objtmp);
                        }
                    }
                }
            }
        }

        public void tbNgayDung_LostFocus(object sender, RoutedEventArgs e)
        {
            float v = 0;
            TextBox Ctr = (sender as TextBox);

            float.TryParse(Ctr.Text, out v);

            Ctr.Text = v.ToString();//show ra giao dien

            //ChangeNgayDung(v,ObjPrescriptionDetail);
            SetValueFollowNgayDung(ObjPrescriptionDetail);
        }

        public void tbNgayDungExt_LostFocus(object sender, RoutedEventArgs e)
        {
            float v = 0;
            TextBox Ctr = (sender as TextBox);

            float.TryParse(Ctr.Text, out v);

            Ctr.Text = v.ToString();//show ra giao dien

            //ChangeNgayDungExtend(v, ObjPrescriptionDetail);
            SetValueFollowNgayDung(ObjPrescriptionDetail);
        }

        private void ChangeNgayDung(double value, object Obj)
        {
            PrescriptionDetail Objtmp = Obj as PrescriptionDetail;
            if (Objtmp != null)
            {
                if (IsPatientInsurance())
                {
                    if (IsBenhNhanNoiTru() == false)
                    {
                        if (Objtmp.DayRpts > xNgayBHToiDa_NgoaiTru)
                        {
                            double NgayInput = Objtmp.DayRpts.DeepCopy();

                            Objtmp.DayRpts = xNgayBHToiDa_NgoaiTru;

                            Objtmp.DayExtended = NgayInput - xNgayBHToiDa_NgoaiTru;

                            Globals.ShowMessage(string.Format(eHCMSResources.Z1085_G1_NgBHKgLonHon0Ng, xNgayBHToiDa_NgoaiTru.ToString(), Objtmp.DayExtended.ToString()), eHCMSResources.G0442_G1_TBao);
                        }
                        else if (Objtmp.DayRpts <= xNgayBHToiDa_NgoaiTru && Objtmp.DayExtended > 0)
                        {
                            Objtmp.DayExtended = 0;
                            Globals.ShowMessage(string.Format(eHCMSResources.Z1087_G1_NgBHNgDungThem2, xNgayBHToiDa_NgoaiTru.ToString()), eHCMSResources.G0442_G1_TBao);
                        }
                    }
                    else
                    {
                        if (Objtmp.DayRpts > xNgayBHToiDa_NoiTru)
                        {
                            double NgayInput = Objtmp.DayRpts.DeepCopy();

                            Objtmp.DayRpts = xNgayBHToiDa_NoiTru;

                            Objtmp.DayExtended = NgayInput - xNgayBHToiDa_NoiTru;

                            Globals.ShowMessage(string.Format(eHCMSResources.Z1085_G1_NgBHKgLonHon0Ng, xNgayBHToiDa_NoiTru.ToString(), Objtmp.DayExtended.ToString()), eHCMSResources.G0442_G1_TBao);
                        }
                        else if (Objtmp.DayRpts <= xNgayBHToiDa_NoiTru && Objtmp.DayExtended > 0)
                        {
                            Objtmp.DayExtended = 0;
                            Globals.ShowMessage(string.Format(eHCMSResources.Z1087_G1_NgBHNgDungThem2, xNgayBHToiDa_NoiTru.ToString()), eHCMSResources.G0442_G1_TBao);
                        }
                    }
                }
            }
        }

        private void ChangeNgayDungExtend(double value, object Obj)
        {
            PrescriptionDetail Objtmp = Obj as PrescriptionDetail;
            if (Objtmp != null && Objtmp.DayExtended != value)
            {
                if (IsPatientInsurance())
                {
                    if (IsBenhNhanNoiTru() == false)
                    {
                        if (Objtmp.DayRpts > xNgayBHToiDa_NgoaiTru)
                        {
                            Objtmp.DayRpts = xNgayBHToiDa_NgoaiTru;
                            Globals.ShowMessage(string.Format(eHCMSResources.Z0963_G1_NgBHKgLonHon0, xNgayBHToiDa_NgoaiTru.ToString()), eHCMSResources.G0442_G1_TBao);
                        }
                        else if (Objtmp.DayRpts < xNgayBHToiDa_NgoaiTru && Objtmp.DayExtended > 0)
                        {
                            Objtmp.DayExtended = 0;
                            Globals.ShowMessage(string.Format(eHCMSResources.Z1087_G1_NgBHNgDungThem2, xNgayBHToiDa_NgoaiTru.ToString()), eHCMSResources.G0442_G1_TBao);
                        }
                    }
                    else
                    {
                        if (Objtmp.DayRpts > xNgayBHToiDa_NoiTru)
                        {
                            Objtmp.DayRpts = xNgayBHToiDa_NoiTru;
                            Globals.ShowMessage(string.Format(eHCMSResources.Z0963_G1_NgBHKgLonHon0, xNgayBHToiDa_NoiTru.ToString()), eHCMSResources.G0442_G1_TBao);
                        }
                        else if (Objtmp.DayRpts < xNgayBHToiDa_NoiTru && Objtmp.DayExtended > 0)
                        {
                            Objtmp.DayExtended = 0;
                            Globals.ShowMessage(string.Format(eHCMSResources.Z1087_G1_NgBHNgDungThem2, xNgayBHToiDa_NoiTru.ToString()), eHCMSResources.G0442_G1_TBao);
                        }
                    }
                }
            }
        }
        
        private bool IsPatientInsurance()
        {
            if (Registration_DataStorage.CurrentPatientRegistration != null)
            {
                if (Registration_DataStorage.CurrentPatientRegistration.HisID != null)
                    return true;
                return false;
            }
            return false;

        }

        private bool IsBenhNhanNoiTru()
        {
            //cho nay can coi lai vi ben nha thuoc sua toa thuoc se khong co RegistrationInfo
            if (Registration_DataStorage.CurrentPatientRegistration != null && Registration_DataStorage.CurrentPatient != null && Registration_DataStorage.CurrentPatient.PatientID > 0)
            {
                if (Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType ==
                    AllLookupValues.RegistrationType.NOI_TRU)
                {
                    return true;
                }
                return false;
            }
            return false;
        }
        
        public void tbSoLuong_LostFocus(object sender, RoutedEventArgs e)
        {
            float v = 0;
            TextBox Ctr = (sender as TextBox);

            float.TryParse(Ctr.Text, out v);

            Ctr.Text = v.ToString();//show ra giao dien
        }

        private float ChangeDoseStringToFloat(string value)
        {
            float result = 0;
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Contains("/"))
                {
                    string pattern = @"\b[\d]+/[\d]+\b";
                    if (!Regex.IsMatch(value, pattern))
                    {
                        Globals.ShowMessage(eHCMSResources.Z1069_G1_LieuDungKgHopLe, eHCMSResources.G0442_G1_TBao);
                        return 0;
                    }
                    else
                    {
                        string[] items = null;
                        items = value.Split('/');
                        if (items.Count() > 2 || items.Count() == 0)
                        {
                            Globals.ShowMessage(eHCMSResources.Z1069_G1_LieuDungKgHopLe, eHCMSResources.G0442_G1_TBao);
                            return 0;
                        }
                        else if (float.Parse(items[1]) == 0)
                        {
                            Globals.ShowMessage(eHCMSResources.Z1069_G1_LieuDungKgHopLe, eHCMSResources.G0442_G1_TBao);
                            return 0;
                        }
                        result = (float)Math.Round((float.Parse(items[0]) / float.Parse(items[1])), 3);
                        if (result < 0)
                        {
                            Globals.ShowMessage(eHCMSResources.Z1071_G1_LieuDungKgNhoHon0, eHCMSResources.G0442_G1_TBao);
                            return 0;
                        }
                    }
                }
                else
                {
                    try
                    {
                        result = float.Parse(value);
                        if (result < 0)
                        {
                            Globals.ShowMessage(eHCMSResources.Z1071_G1_LieuDungKgNhoHon0, eHCMSResources.G0442_G1_TBao);
                            return 0;
                        }
                    }
                    catch
                    {
                        Globals.ShowMessage(eHCMSResources.Z1069_G1_LieuDungKgHopLe, eHCMSResources.G0442_G1_TBao);
                        return 0;
                    }
                }
            }
            return result;
        }

        private void ChangeMDose(string value)
        {
            PrescriptionDetail Objtmp = this.ObjPrescriptionDetail;
            if (Objtmp != null)
            {
                Objtmp.MDose = ChangeDoseStringToFloat(Objtmp.MDoseStr);
                if (Objtmp.MDose == 0)
                {
                    Objtmp.MDoseStr = "0";
                }
            }
        }

        private void ChangeNDose(string value)
        {
            PrescriptionDetail Objtmp = this.ObjPrescriptionDetail;
            if (Objtmp != null)
            {
                Objtmp.NDose = ChangeDoseStringToFloat(Objtmp.NDoseStr);
                if (Objtmp.NDose == 0)
                {
                    Objtmp.NDoseStr = "0";
                }
            }
        }

        private void ChangeEDose(string value)
        {
            PrescriptionDetail Objtmp = this.ObjPrescriptionDetail;
            if (Objtmp != null)
            {
                Objtmp.EDose = ChangeDoseStringToFloat(Objtmp.EDoseStr);
                if (Objtmp.EDose == 0)
                {
                    Objtmp.EDoseStr = "0";
                }
            }
        }

        private void ChangeADose(string value)
        {
            PrescriptionDetail Objtmp = this.ObjPrescriptionDetail;
            if (Objtmp != null)
            {
                Objtmp.ADose = ChangeDoseStringToFloat(Objtmp.ADoseStr);
                if (Objtmp.ADose == 0)
                {
                    Objtmp.ADoseStr = "0";
                }
            }
        }

        public void tbSang_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox Ctr = (sender as TextBox);
            ChangeMDose(Ctr.Text);
            SetValueFollowNgayDung(ObjPrescriptionDetail);
        }

        public void tbTrua_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox Ctr = (sender as TextBox);
            ChangeADose(Ctr.Text);
            SetValueFollowNgayDung(ObjPrescriptionDetail);
        }

        public void tbChieu_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox Ctr = (sender as TextBox);
            ChangeEDose(Ctr.Text);
            SetValueFollowNgayDung(ObjPrescriptionDetail);
        }

        public void tbToi_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox Ctr = (sender as TextBox);
            ChangeNDose(Ctr.Text);
            SetValueFollowNgayDung(ObjPrescriptionDetail);
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
    }
}
