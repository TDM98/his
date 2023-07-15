using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using System.Threading;
using aEMR.ServiceClient;
using CommonServiceProxy;
using aEMR.Infrastructure;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Castle.Windsor;
using eHCMSLanguage;
using aEMR.Controls;
using System.Linq;
using aEMR.Common;
using System.ServiceModel;
using aEMR.DataContracts;

/*
 * 20190816 #001 TBL: BM 0013175: Lưu Thông tin theo dõi theo y lệnh vào bảng mới.
 * 20191109 #002 TTM: BM 0017401: [Y lệnh] Bổ sung ComboBox thời gian theo dõi y lệnh.
 * 20220725 #003 DatTB: 
 * + Thêm thông tin nhịp thở RespiratoryRate vào InPtDiagAndDoctorInstruction. 
 * + Lấy danh sách ListV_ReconmendTime cho Đường huyết và ECG vì yêu cầu thêm value cho riêng 2 trường này.
 * 20230202 #004 QTD: Hiển thị cảnh báo khi thay đổi chăm sóc cấp
 */
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IMonitoringVitalSigns)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MonitoringVitalSignsViewModel : Conductor<object>, IMonitoringVitalSigns
    {
        private InPatientInstruction _gInPatientInstruction = null;
        public InPatientInstruction gInPatientInstruction
        {
            get
            {
                return _gInPatientInstruction;
            }
            set
            {
                _gInPatientInstruction = value;
                if (_gInPatientInstruction != null && _gInPatientInstruction.IntPtDiagDrInstructionID > 0)
                {
                    _EnableSaveCmd = true;
                    NotifyOfPropertyChange(() => EnableSaveCmd);
                }
                //if(_gInPatientInstruction.V_LevelCare != null)
                //{
                    SelectedV_LevelCare = _gInPatientInstruction.V_LevelCare;
                //}
                NotifyOfPropertyChange(() => gInPatientInstruction);
            }
        }
        private ObservableCollection<Lookup> _LevelCareList;
        public ObservableCollection<Lookup> LevelCareList
        {
            get { return _LevelCareList; }
            set
            {
                if (_LevelCareList == value)
                    return;
                _LevelCareList = value;
                NotifyOfPropertyChange(() => LevelCareList);
            }
        }
        private bool _EnableSaveCmd;
        public bool EnableSaveCmd
        {
            get { return _EnableSaveCmd; }
            set
            {
                if (_EnableSaveCmd == value)
                {
                    return;
                }
                _EnableSaveCmd = value;
                NotifyOfPropertyChange(() => EnableSaveCmd);
            }
        }

        [ImportingConstructor]
        public MonitoringVitalSignsViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            GetInfusionType();
            //▼===== #002
            LoadV_ReconmendTime();
            //▲===== #002
        }
        private void GetInfusionType()
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllLookupValuesByType(LookupValues.V_LevelCare, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            IList<Lookup> results = contract.EndGetAllLookupValuesByType(asyncResult);
                            if (results != null)
                            {
                                if (LevelCareList == null)
                                {
                                    LevelCareList = new ObservableCollection<Lookup>();
                                }
                                else
                                {
                                    LevelCareList.Clear();
                                }
                                foreach (Lookup p in results)
                                {
                                    LevelCareList.Add(p);
                                }
                                NotifyOfPropertyChange(() => LevelCareList);
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
                    }),
                    null);
                }
            });
            t.Start();
        }
        //▼====== #001
        public void SaveCmd()
        {
            if (gInPatientInstruction == null)
            {
                return;
            }
            if (CheckEmptyInPatientInstruction(gInPatientInstruction))
            {
                MessageBox.Show(eHCMSResources.Z2299_G1_KhongCoGiDeLuu);
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSaveInstructionMonitoring(gInPatientInstruction, Globals.LoggedUserAccount.StaffID.Value, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            bool results = contract.EndSaveInstructionMonitoring(asyncResult);
                            if (results)
                            {
                                MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                            Globals.IsBusy = false;
                        }
                    }),
                    null);
                }
            });
            t.Start();
        }

        private bool CheckEmptyInPatientInstruction(InPatientInstruction InPatientInstruction)
        {
            if (string.IsNullOrEmpty(InPatientInstruction.PulseAndBloodPressure) && string.IsNullOrEmpty(InPatientInstruction.SpO2) && string.IsNullOrEmpty(InPatientInstruction.Temperature)
               //▼==== #003
               && string.IsNullOrEmpty(InPatientInstruction.RespiratoryRate)
               //▲==== #003
               && string.IsNullOrEmpty(InPatientInstruction.Sense) && string.IsNullOrEmpty(InPatientInstruction.BloodSugar) && string.IsNullOrEmpty(InPatientInstruction.Urine)
               && string.IsNullOrEmpty(InPatientInstruction.ECG) && string.IsNullOrEmpty(InPatientInstruction.PhysicalExamOther) && string.IsNullOrEmpty(InPatientInstruction.Diet)
               && (gInPatientInstruction.V_LevelCare == null || gInPatientInstruction.V_LevelCare.LookupID <= 1))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //▲====== #001
        //▼===== #002: Hiện tại ý anh Tuấn là để lưu free text xuống cho họ sử dụng. Khi bắt đầu thực hiện task thực hiện y lệnh sẽ bàn với anh Tuấn về hướng giải quyết cho các thời gian gợi ý này.
        //             Vì anh Tuấn muốn khi người dùng nhập gợi ý sẽ tính được điều dưỡng cần thực hiện bao nhiêu lần các yêu cầu của Bác sĩ trong khoảng thời gian từ lúc tạo y lệnh đến lúc ra y lệnh mới.
        //             Ví dụ: y lệnh ra từ 7h30 -> 9h30 yêu cầu đo mạch 15 phút tức là mỗi 15 phút trong 2 tiếng đo 1 lần => điều dưỡng phải đo 8 lần
        //             Để kiểm tra được điều dưỡng có thực hiện y lệnh đúng như yêu cầu của bác sĩ hay không.
        #region Reconmend Time Value Region
        private ObservableCollection<Lookup> _ListV_ReconmendTime;
        public ObservableCollection<Lookup> ListV_ReconmendTime
        {
            get { return _ListV_ReconmendTime; }
            set
            {
                _ListV_ReconmendTime = value;
                NotifyOfPropertyChange(() => ListV_ReconmendTime);
            }
        }
        private ObservableCollection<Lookup> _ListV_ReconmendTimeAfterFilltering;
        public ObservableCollection<Lookup> ListV_ReconmendTimeAfterFilltering
        {
            get { return _ListV_ReconmendTimeAfterFilltering; }
            set
            {
                _ListV_ReconmendTimeAfterFilltering = value;
                NotifyOfPropertyChange(() => ListV_ReconmendTimeAfterFilltering);
            }
        }
        public string ConvertString(string stringInput)
        {
            stringInput = stringInput.ToUpper();
            string convert = "ĂÂÀẰẦÁẮẤẢẲẨÃẴẪẠẶẬỄẼỂẺÉÊÈỀẾẸỆÔÒỒƠỜÓỐỚỎỔỞÕỖỠỌỘỢƯÚÙỨỪỦỬŨỮỤỰÌÍỈĨỊỲÝỶỸỴĐăâàằầáắấảẳẩãẵẫạặậễẽểẻéêèềếẹệôòồơờóốớỏổởõỗỡọộợưúùứừủửũữụựìíỉĩịỳýỷỹỵđ";
            string To = "AAAAAAAAAAAAAAAAAEEEEEEEEEEEOOOOOOOOOOOOOOOOOUUUUUUUUUUUIIIIIYYYYYDaaaaaaaaaaaaaaaaaeeeeeeeeeeeooooooooooooooooouuuuuuuuuuuiiiiiyyyyyd";
            for (int i = 0; i < To.Length; i++)
            {
                stringInput = stringInput.Replace(convert[i], To[i]);
            }
            return stringInput;
        }
        public void ReconmendTime_Loaded(object sender, PopulatingEventArgs e)
        {
            ((AxAutoComplete)sender).ItemsSource = ListV_ReconmendTime;
        }


        public void ReconmendTime_Populating(object sender, PopulatingEventArgs e)
        {
            if (sender != null)
            {
                string SearchText = ((AutoCompleteBox)sender).SearchText;
                ListV_ReconmendTimeAfterFilltering = new ObservableCollection<Lookup>(ListV_ReconmendTime.Where(item => ConvertString(item.ObjectValue)
                     .IndexOf(ConvertString(SearchText), StringComparison.InvariantCultureIgnoreCase) >= 0));
                ((AutoCompleteBox)sender).ItemsSource = ListV_ReconmendTimeAfterFilltering;
                ((AutoCompleteBox)sender).PopulateComplete();
            }
        }
        public void LoadV_ReconmendTime()
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllLookupValuesByType(LookupValues.V_ReconmendTime, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var allItems = contract.EndGetAllLookupValuesByType(asyncResult);
                                //▼==== #003
                                ListV_ReconmendTime = new ObservableCollection<Lookup>(allItems.Where(item => item.LookupID != (long)V_ReconmendTime.UNFOLLOW).OrderBy(item => Int32.Parse(item.ObjectNotes)));
                                ListV_ReconmendTimeForECG = new ObservableCollection<Lookup>(allItems.OrderBy(item => Int32.Parse(item.ObjectNotes)));
                                //▲==== #003
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                            finally
                            {
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0074_G1_I);
                }
            });
            t.Start();
        }

        //▼==== #003
        private ObservableCollection<Lookup> _ListV_ReconmendTimeForECG;
        public ObservableCollection<Lookup> ListV_ReconmendTimeForECG
        {
            get { return _ListV_ReconmendTimeForECG; }
            set
            {
                _ListV_ReconmendTimeForECG = value;
                NotifyOfPropertyChange(() => ListV_ReconmendTimeForECG);
            }
        }
        private ObservableCollection<Lookup> _ListV_ReconmendTimeForECGAfterFilltering;
        public ObservableCollection<Lookup> ListV_ReconmendTimeForECGAfterFilltering
        {
            get { return _ListV_ReconmendTimeForECGAfterFilltering; }
            set
            {
                _ListV_ReconmendTimeForECGAfterFilltering = value;
                NotifyOfPropertyChange(() => ListV_ReconmendTimeForECGAfterFilltering);
            }
        }
        public void ReconmendTimeForECG_Loaded(object sender, PopulatingEventArgs e)
        {
            ((AxAutoComplete)sender).ItemsSource = ListV_ReconmendTimeForECG;
        }


        public void ReconmendTimeForECG_Populating(object sender, PopulatingEventArgs e)
        {
            if (sender != null)
            {
                string SearchText = ((AutoCompleteBox)sender).SearchText;
                ListV_ReconmendTimeForECGAfterFilltering = new ObservableCollection<Lookup>(ListV_ReconmendTimeForECG.Where(item => ConvertString(item.ObjectValue)
                     .IndexOf(ConvertString(SearchText), StringComparison.InvariantCultureIgnoreCase) >= 0));
                ((AutoCompleteBox)sender).ItemsSource = ListV_ReconmendTimeForECGAfterFilltering;
                ((AutoCompleteBox)sender).PopulateComplete();
            }
        }
        //▲==== #003

        public void cboCdoAn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AxComboBox cboCdoAn = sender as AxComboBox;
            if (cboCdoAn != null)
            {
                if (gInPatientInstruction != null)
                {
                    if (cboCdoAn.SelectedIndex >= 0)
                    {
                        gInPatientInstruction.Diet = cboCdoAn.SelectedValue.ToString();
                    }
                }
            }
        }

        public void cboCdAn_TextChanged(object sender)
        {
            AxComboBox cboCdoAn = sender as AxComboBox;
            if(cboCdoAn != null && gInPatientInstruction != null)
            {
                gInPatientInstruction.Diet = cboCdoAn.Text.ToString();
            }
        }
        #endregion
        //▲===== #002

        //▼===== #003
        private Lookup _SelectedV_LevelCare;
        public Lookup SelectedV_LevelCare
        {
            get 
            { 
                return _SelectedV_LevelCare; 
            }
            set
            {
                if(_SelectedV_LevelCare != value)
                {
                    _SelectedV_LevelCare = value;
                    NotifyOfPropertyChange(() => SelectedV_LevelCare);
                }
            }
        }
        public void cboVLevelCare_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(gInPatientInstruction == null 
                || gInPatientInstruction.V_LevelCare == null 
                || SelectedV_LevelCare == null 
                || SelectedV_LevelCare.LookupID == gInPatientInstruction.V_LevelCare.LookupID)
            {
                return;
            }
            Lookup TempLevelCare = (sender as AxComboBox).SelectedItem as Lookup;

            if (gInPatientInstruction.V_LevelCare.LookupID == 62501)
            {
                if (MessageBox.Show("Anh/Chị có chắc chắn người bệnh cần chuyển sang \nphân cấp chăm sóc 1 (bệnh nặng cần theo dõi) không?"
                    , eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    gInPatientInstruction.V_LevelCare = SelectedV_LevelCare;
                    return;
                }
                else
                {
                    SelectedV_LevelCare = gInPatientInstruction.V_LevelCare;
                    return;
                }
            }
            else if (SelectedV_LevelCare.LookupID == 62501)
            {
                if (MessageBox.Show("Người bệnh nặng đang phân cấp chăm sóc cấp 1, cần theo dõi.\nAnh chị có đồng ý thay đổi phân cấp chăm sóc không?"
                    , eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    gInPatientInstruction.V_LevelCare = SelectedV_LevelCare;
                    return;
                }
                else
                {
                    SelectedV_LevelCare = gInPatientInstruction.V_LevelCare;
                    return;
                }
            }

            //SelectedV_LevelCare = (sender as AxComboBox).SelectedItem as Lookup;
            //if (SelectedV_LevelCare != null && gInPatientInstruction != null && gInPatientInstruction.V_LevelCare != null
            //    && SelectedV_LevelCare.LookupID != gInPatientInstruction.V_LevelCare.LookupID)
            //{
            //    if(SelectedV_LevelCare.LookupID == 62501)
            //    {
            //        if (MessageBox.Show("Anh/Chị có chắc chắn người bệnh cần chuyển sang \nphân cấp chăm sóc 1 (bệnh nặng cần theo dõi) không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            //        {
            //            SelectedV_LevelCare = gInPatientInstruction.V_LevelCare;
            //            return;
            //        }
            //    }
            //    else if (gInPatientInstruction.V_LevelCare.LookupID == 62501)
            //    {
            //        if (MessageBox.Show("Người bệnh nặng đang phân cấp chăm sóc cấp 1, cần theo dõi.\nAnh chị có đồng ý thay đổi phân cấp chăm sóc không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            //        {
            //            SelectedV_LevelCare = gInPatientInstruction.V_LevelCare;
            //            return;
            //        }
            //    }
            //    gInPatientInstruction.V_LevelCare = SelectedV_LevelCare;
            //}
        }
        //▲===== #003
    }
}
