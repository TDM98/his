using System.Linq;
using System.ServiceModel;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using DataEntities;
using System.Threading;
using System;
using System.Windows.Media;
using System.Windows.Input;
using System.Collections.Generic;
using System.Diagnostics;
using eHCMSLanguage;
using aEMR.Infrastructure;
using aEMR.ViewContracts;
using aEMR.ServiceClient;
using aEMR.Common.Collections;
using aEMR.Controls;
using aEMR.DataContracts;
using aEMR.Infrastructure.Events;

/*
 * 20180920 #001 TBL:   Chinh sua chuc nang bug mantis ID 0000061, kiem tra tung property trong dataentities 
 *                      neu co su thay doi IsDataChanged = true
 * 20180926 #002 TBL: BM 0000077.          
 * 20181005 #003 TBL: BM 0000151: Lưu toa có chọn hẹn TK nhưng bấm hẹn bệnh không được
 * 20181007 #004 TBL: BM 0000115: Khi tick toa khong thuoc thi toa thuoc da co thay doi
 * 20181029 #005 TTM: BM 0004199: Thêm cảnh báo nếu bác sĩ ra toa thuốc có thuốc ngoài danh mục cần phải có sự xác nhận của bệnh nhân
 * 20190128 #006 TBL: Khi them thuoc co chan doan thi chan doan do duoc them vao chan doan ben tab Kham benh
 * 20190407 #007 TTM: BM 0006731: Chuyển đổi định dạng hiển thị cho phù hợp. Ví dụ: .5 => 0.5 || ,5 => 0.5 || 0,5 => 0.5 ...
 * 20190708 #008 TBL: BM 0011924: Hẹn tự động theo cấu hình AppointmentAuto, ParamAppointmentAuto
 * 20190727 #009 TBL: BM 0013004: Hiện màu khác cho những thuốc chống chỉ định
 * 20200726 #010 TTM: BM 0039412: Fix lỗi chết chương trình khi đang gõ autocomplete hiện ra nhưng bấm liền nút bỏ qua => Chết chương trình.
 *                                Nguyên nhân: Lúc bấm nút bỏ qua selectedIndex của Grid chuyển về -1 => Chết do out of range index.
 */
namespace aEMR.Common.ViewModels
{
    public partial class ePrescriptionOldNewViewModel
    {
        private ObservableCollection<Lookup> _DrugTypes;
        public ObservableCollection<Lookup> DrugTypes
        {
            get
            {
                return _DrugTypes;
            }
            set
            {
                if (_DrugTypes != value)
                {
                    _DrugTypes = value;
                    NotifyOfPropertyChange(() => DrugTypes);
                }
            }
        }

        private ObservableCollection<PrescriptionDetailSchedulesLieuDung> _LieuDung;
        public ObservableCollection<PrescriptionDetailSchedulesLieuDung> LieuDung
        {
            get
            {
                return _LieuDung;
            }
            set
            {
                if (_LieuDung != value)
                {
                    _LieuDung = value;
                    NotifyOfPropertyChange(() => _LieuDung);
                }
            }
        }

        private PhysicalExamination _curPhysicalExamination;
        public PhysicalExamination curPhysicalExamination
        {
            get { return _curPhysicalExamination; }
            set
            {
                if (_curPhysicalExamination != value)
                {
                    _curPhysicalExamination = value;
                    NotifyWhenBusy();
                    NotifyOfPropertyChange(() => curPhysicalExamination);
                }
            }
        }

        private bool _PreNoDrug;
        public bool PreNoDrug
        {
            get { return _PreNoDrug; }
            set
            {
                if (_PreNoDrug != value)
                {
                    /*▼====: #004*/
                    LatestePrecriptions.SetDataChanged();
                    /*▲====: #004*/
                    _PreNoDrug = value;
                    NotifyOfPropertyChange(() => PreNoDrug);
                }
            }
        }

        private bool _isSearchByGenericName;
        public bool IsSearchByGenericName
        {
            get { return _isSearchByGenericName; }
            set
            {
                if (_isSearchByGenericName != value)
                {
                    _isSearchByGenericName = value;
                    NotifyOfPropertyChange(() => IsSearchByGenericName);
                }
            }
        }

        private bool _HisIDVisibility = false;
        public bool HisIDVisibility
        {
            get { return _HisIDVisibility; }
            set
            {
                if (_HisIDVisibility != value)
                {
                    _HisIDVisibility = value;
                    NotifyOfPropertyChange(() => HisIDVisibility);
                }
            }
        }

        private bool _isHisID;
        public bool isHisID
        {
            get { return _isHisID; }
            set
            {
                if (_isHisID != value)
                {
                    _isHisID = value;
                    NotifyOfPropertyChange(() => isHisID);
                }
            }
        }

        private bool _IsShowValidateExpiredPrescription;
        public bool IsShowValidateExpiredPrescription
        {
            get { return _IsShowValidateExpiredPrescription; }
            set
            {
                if (_IsShowValidateExpiredPrescription != value)
                {
                    _IsShowValidateExpiredPrescription = value;
                    NotifyOfPropertyChange(() => IsShowValidateExpiredPrescription);
                }
            }
        }

        public void PrescriptIssue_HICover_CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (_isHisID)
            {
                LatestePrecriptions.PrescriptionIssueHistory.HisID = Registration_DataStorage.CurrentPatientRegistration.HisID.Value;
                /*▼====: #001*/
                LatestePrecriptions.SetDataChanged();
                /*▲====: #001*/
            }
            else
            {
                LatestePrecriptions.PrescriptionIssueHistory.HisID = 0;
                /*▼====: #001*/
                LatestePrecriptions.SetDataChanged();
                /*▲====: #001*/
            }
        }

        private void GetAllLookupForDrugTypes()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {

                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllLookupValuesByType(LookupValues.V_DrugType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetAllLookupValuesByType(asyncResult);
                            DrugTypes = results.ToObservableCollection();
                            NotifyOfPropertyChange(() => DrugTypes);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public object GetDrugType(object value)
        {
            PrescriptionDetail p = value as PrescriptionDetail;
            if (p != null)
            {
                return p.V_DrugType;
            }
            else
            {
                return null;
            }
        }

        public void chkSearchByGenericName_Loaded(object sender, RoutedEventArgs e)
        {
            var chkSearchByGenericName = sender as CheckBox;

            if (Globals.ServerConfigSection.ConsultationElements.DefSearchByGenericName)
            {
                chkSearchByGenericName.IsChecked = true;
            }
            else
            {
                chkSearchByGenericName.IsChecked = false;
            }
        }

        public void cbxDrugType_Loaded(object sender, RoutedEventArgs e)
        {
            var kbEnabledComboBox = sender as KeyEnabledComboBox;
            if (kbEnabledComboBox != null)
            {
                //if (!kbEnabledComboBox.DoneSetKbSelection)
                //{
                //    kbEnabledComboBox.SetKeyboardSelection();
                //}
                kbEnabledComboBox.ItemsSource = DrugTypes;
                // Txd Commented out
                //if (SelectedPrescriptionDetail != null)
                //{
                //    comboBox.SelectedItem = GetDrugType(SelectedPrescriptionDetail);
                //}
            }
        }

        private bool bHandleDrugTypeSelChanged = true;
        public void cbxDrugType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine(" ===>>> cbxDrugType_SelectionChanged : {0}", ((Lookup)((ComboBox)sender).SelectedItem).LookupID);
            if (LatestePrecriptions.PrescriptionDetails.Count <= 0)
            {
                // Txd: This check is ALMOST IMPOSSIBLE to happen but JUST IN CASE IT MAY
                //      HAPPEN IN THIS CRAZY PLACE
                return;
            }

            PrescriptionDetail selPrescriptionDetail = LatestePrecriptions.PrescriptionDetails[grdPrescription.SelectedIndex];
            if (selPrescriptionDetail == null)
                return;

            if (!bHandleDrugTypeSelChanged)
            {
                bHandleDrugTypeSelChanged = true;
                return;
            }

            long nSelDrugTypeLookupID = ((Lookup)((ComboBox)sender).SelectedItem).LookupID;

            Debug.WriteLine(" ===>>> cbxDrugType_SelectionChanged: SELECTED DRUG TYPE = [{0}] --AND-- SAVED V_DRUGTYPE = [{1}].", nSelDrugTypeLookupID, selPrescriptionDetail.V_DrugType);

            if (nSelDrugTypeLookupID != selPrescriptionDetail.V_DrugType)
            {
                Debug.WriteLine(" ===>>> cbxDrugType_SelectionChanged: SELECTED DRUG TYPE = [{0}] --DIFF TO-- SAVED V_DRUGTYPE = [{1}].", nSelDrugTypeLookupID, selPrescriptionDetail.V_DrugType);

                if (selPrescriptionDetail.MDose > 0 || selPrescriptionDetail.NDose > 0 || selPrescriptionDetail.ADose > 0 || selPrescriptionDetail.EDose > 0 || selPrescriptionDetail.Qty > 0)
                {
                    if (MessageBox.Show(string.Format("{0}.", eHCMSResources.A1012_G1_Msg_ConfDoiLoaiThuoc), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    {
                        ComboBox cbxTmp = (ComboBox)sender;
                        bHandleDrugTypeSelChanged = false;
                        cbxTmp.SelectedItem = e.RemovedItems[0];
                        return;
                    }
                }
            }
            else
            {
                Debug.WriteLine(" ===>>> cbxDrugType_SelectionChanged: SELECTED DRUG TYPE = [{0}] --SAME-- AS SAVED V_DRUGTYPE = [{1}] ==> NOTHING TO DO HERE.", nSelDrugTypeLookupID, selPrescriptionDetail.V_DrugType);
                return;
            }

            ClearDataRow(selPrescriptionDetail);

            // Txd 30/09/2013
            // For some unknown reason at this stage that the BINDING of this COMBOBOX is NOT QUITE RIGHT 
            // So we just have to assign V_DrugType manually just incase
            selPrescriptionDetail.V_DrugType = nSelDrugTypeLookupID;

            switch (nSelDrugTypeLookupID)
            {
                case (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN:
                    Debug.WriteLine(" ===>>> cbxDrugType_SelectionChanged: LICH TUAN SELECTED.");
                    // Txd 23/09/2013 Note 
                    // For some reasons that are unknown at this stage, the DrugType property of PrescriptionDetail object 
                    // eventhough BOUND to the CTDrugType TextBlock but not yet UPDATED on this VERY Event 
                    // Thus we have to call hplEditSchedules DIRECTLY to avoid the check in of DrugType in method hplEditSchedules_Click                    

                    // Txd 27/09/2013 For some reason this method cbxDrugType_SelectionChangedis being called twice upon a selection change using 
                    // the keyboard to select so we need to double check to make sure the Schedule Dialog is not being shown twice
                    //if (selPrescriptionDetail.DrugType.LookupID != (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN)
                    //{
                    Debug.WriteLine(" ===>>> cbxDrugType_SelectionChanged: LICH TUAN SELECTED AND SHOW SHEDULE Dialog.");
                    hplEditSchedules(selPrescriptionDetail);
                    //}
                    break;
                case (long)AllLookupValues.V_DrugType.THUOC_UONGKHICAN:
                    Debug.WriteLine(" ===>>> cbxDrugType_SelectionChanged: UONG KHI CAN SELECTED called InitUsageDaysForDrugTakenAsRequired.");
                    InitUsageDaysForDrugTakenAsRequired(selPrescriptionDetail);
                    break;
            }


            //if (((Lookup)((ComboBox)sender).SelectedItem).LookupID == (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN)
            //{
            //    //SelectedPrescriptionDetail.DrugType = (Lookup)((ComboBox)sender).SelectedItem;
            //    //
            //    // Txd 23/09/2013 Note 
            //    // For some reasons that are unknown at this stage, the DrugType property of PrescriptionDetail object 
            //    // eventhough BOUND to the CTDrugType TextBlock but not yet UPDATED on this VERY Event 
            //    // Thus we have to call hplEditSchedules DIRECTLY to avoid the check in of DrugType in method hplEditSchedules_Click
            //    //
            //    // hplEditSchedules_Click(selPrescriptionDetail);
            //    // Txd 27/09/2013 For some reason this method cbxDrugType_SelectionChangedis being called twice upon a selection change using 
            //    // the keyboard to select so we need to double check to make sure the Schedule Dialog is not being shown twice
            //    if (selPrescriptionDetail.DrugType.LookupID != (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN)
            //    {
            //        hplEditSchedules(selPrescriptionDetail);
            //    }
            //}
            //else if (((Lookup)((ComboBox)sender).SelectedItem).LookupID == (long)AllLookupValues.V_DrugType.THUOC_UONGKHICAN)
            //{
            //    //SelectedPrescriptionDetail.DrugType = (Lookup)((ComboBox)sender).SelectedItem;
            //    InitUsageDaysForDrugTakenAsRequired(selPrescriptionDetail);                
            //}

        }


        public void cbxDrugTypeForm_Loaded(object sender, RoutedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                //comboBox.ItemsSource = DrugTypes;
                //if (SelectedPrescriptionDetail != null)
                //{
                //    comboBox.SelectedItem = GetDrugType(tempPrescriptionDetail);
                //}
            }
        }
        public void cbxDrugTypeForm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NotifyOfPropertyChange(() => ObjPrescriptionDetailForForm);
            if (((Lookup)((ComboBox)sender).SelectedItem).LookupID == (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN)
            {
                ObjPrescriptionDetailForForm.DrugType = (Lookup)((ComboBox)sender).SelectedItem;
                hplEditSchedules_Click(ObjPrescriptionDetailForForm);
            }
        }

        private void MovePresDetailRowUpDown(bool bMoveDown)
        {
            if (!SelectedPrescriptionDetailIsValid())
            {
                MessageBox.Show(eHCMSResources.A0338_G1_Msg_InfoChonMotThuoc, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            PrescriptionDetail selPrescriptionDetail = LatestePrecriptions.PrescriptionDetails[grdPrescription.SelectedIndex];
            int selIndex = grdPrescription.SelectedIndex;
            // Txd: The following is just a double check for the above just in case. TO BE REMOVED EVENTUALLY
            if (selPrescriptionDetail == null || (selIndex >= LatestePrecriptions.PrescriptionDetails.Count - 1))
            {
                // This is the empty Row CANNOT Move UP or DOWN
                MessageBox.Show(eHCMSResources.A0338_G1_Msg_InfoChonMotThuoc);
                return;
            }


            if (selPrescriptionDetail.SelectedDrugForPrescription.BrandName.Length < 2)
            {
                MessageBox.Show(eHCMSResources.Z0442_G1_SomethingIsWrongHere);
                return;
            }

            if (bMoveDown)
            {
                if (selIndex >= (LatestePrecriptions.PrescriptionDetails.Count - 2))
                {
                    MessageBox.Show(eHCMSResources.K0270_G1_ViTriThuocKhongThayDoi);
                    return;
                }
            }
            else
            {
                if (selIndex <= 0)
                {
                    MessageBox.Show(eHCMSResources.K0270_G1_ViTriThuocKhongThayDoi);
                    return;
                }
            }

            var newDetails = new ObservableCollection<PrescriptionDetail>();
            var selObj = new PrescriptionDetail();
            selObj = ObjectCopier.DeepCopy(LatestePrecriptions.PrescriptionDetails[selIndex]);

            int nIdxToAddSelObj = (bMoveDown ? (selIndex + 1) : (selIndex - 1));
            int nIdx = 0;
            foreach (var itemPresDetail in LatestePrecriptions.PrescriptionDetails)
            {
                if (!bMoveDown && nIdx == nIdxToAddSelObj)
                {
                    newDetails.Add(selObj);
                }

                if (nIdx != selIndex)
                {
                    var newObj = new PrescriptionDetail();
                    newObj = ObjectCopier.DeepCopy(itemPresDetail);
                    newDetails.Add(newObj);
                }

                if (bMoveDown && nIdx == nIdxToAddSelObj)
                {
                    newDetails.Add(selObj);
                }

                ++nIdx;
            }

            LatestePrecriptions.PrescriptionDetails = newDetails;
            grdPrescription.SelectedIndex = nIdxToAddSelObj;

        }


        public void btnDown()
        {
            MovePresDetailRowUpDown(true);
        }

        public void btnUp()
        {
            MovePresDetailRowUpDown(false);
        }

        public void chk_NotInCat_Click(object sender, RoutedEventArgs e)
        {
            if (LatestePrecriptions.PrescriptionDetails.Count <= 0)
                return;

            PrescriptionDetail selPrescriptionDetail = LatestePrecriptions.PrescriptionDetails[grdPrescription.SelectedIndex];
            if (selPrescriptionDetail == null)
                return;

            CheckBox ckbNotIncat = (CheckBox)sender;
            if (ckbNotIncat.IsChecked == true)
            {
                if (selPrescriptionDetail.DrugID > 0)
                {
                    // Thuoc Trong Danh muc doi sang ngoai danh muc
                    if (MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0733_G1_I), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        //CreateNewDrugForSellVisitor(selPrescriptionDetail);
                        selPrescriptionDetail = NewReInitPrescriptionDetail(false, null);
                        selPrescriptionDetail.IsDrugNotInCat = true;
                    }
                    else
                    {
                        ckbNotIncat.IsChecked = false;
                    }
                }
            }
            else if (ckbNotIncat.IsChecked == false)
            {
                if (selPrescriptionDetail.DrugID > 0)
                {
                    MessageBox.Show(eHCMSResources.Z0442_G1_SomethingIsWrongHere);
                    //return;
                }

                // Thuoc Ngoai Danh muc doi sang trong danh muc
                if (MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0734_G1_I), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    //CreateNewDrugForSellVisitor(selPrescriptionDetail);
                    selPrescriptionDetail = NewReInitPrescriptionDetail(false, null);
                    selPrescriptionDetail.IsDrugNotInCat = false;
                }
                else
                {
                    ckbNotIncat.IsChecked = true;
                }
            }

            //if (((CheckBox)sender).IsChecked == false)
            //{
            //    SelectedPrescriptionDetail = ObjectCopier.DeepCopy(CreateNewDrugForSellVisitor(SelectedPrescriptionDetail));
            //    NotifyOfPropertyChange(() => SelectedPrescriptionDetail);
            //}
        }

        public void chk_NotInCatForm_Checked(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).IsChecked == true && ObjPrescriptionDetailForForm != null)
            {

            }

            ObjPrescriptionDetailForForm.SelectedDrugForPrescription = new GetDrugForSellVisitor();
            ObjPrescriptionDetailForForm.DrugID = 0;
            int nDayVal = LatestePrecriptions.NDay == null ? 0 : LatestePrecriptions.NDay.Value;
            SetDefaultDay(ObjPrescriptionDetailForForm, nDayVal);
            ObjPrescriptionDetailForForm = ObjectCopier.DeepCopy(ObjPrescriptionDetailForForm);

            NotifyOfPropertyChange(() => ObjPrescriptionDetailForForm.IsDrugNotInCat);
            NotifyOfPropertyChange(() => ObjPrescriptionDetailForForm.DrugType);
            ClearForm();
        }

        public void chk_NotInCatForm_Click(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).IsChecked == true && ObjPrescriptionDetailForForm != null)
            {
                ObjPrescriptionDetailForForm.SelectedDrugForPrescription = new GetDrugForSellVisitor();

                // Txd 25/09/2013
                // ObjPrescriptionDetailForForm = ObjectCopier.DeepCopy(RefreshPrescriptObject(ObjPrescriptionDetailForForm));
                ObjPrescriptionDetailForForm = NewReInitPrescriptionDetail(true, ObjPrescriptionDetailForForm, true);
            }
            ClearForm();
        }

        public void chk_HIForm_Click(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).IsChecked == true && ObjPrescriptionDetailForForm != null)
            {

            }
            //ObjPrescriptionDetailForForm = ObjectCopier.DeepCopy(CreateNewDrugForSellVisitor(ObjPrescriptionDetailForForm));
            ObjPrescriptionDetailForForm = NewReInitPrescriptionDetail(true, ObjPrescriptionDetailForForm);
            ClearForm();
            NotifyOfPropertyChange(() => ObjPrescriptionDetailForForm);
        }

        public void chk_HI_Click(object sender, RoutedEventArgs e)
        {
            /* Txd: 20/03/2013
            if (((CheckBox)sender).IsChecked == true
                && SelectedPrescriptionDetail != null)
            {

            }
            else 
            {
                //MessageBox.Show("Bạn mới đổi từ thuốc ngoài danh mục qua thuốc trong danh mục."
                //+ "\nChọn lại thuốc cho dòng này!");
            }
            //SelectedPrescriptionDetail = ObjectCopier.DeepCopy(CreateNewDrugForSellVisitor(SelectedPrescriptionDetail));   
            NotifyOfPropertyChange(() => SelectedPrescriptionDetail);
             * */
            //if (SelectedPrescriptionDetail != null && SelectedPrescriptionDetail.DrugID!=0)
            //{
            //    SelectedPrescriptionDetail.BeOfHIMedicineList = !SelectedPrescriptionDetail.BeOfHIMedicineList;
            //    MessageBox.Show((SelectedPrescriptionDetail.BeOfHIMedicineList?"Thuốc bảo hiểm không thể đổi qua thuốc mua ngoài":"Thuốc mua ngoài không thể đổi qua thuốc bảo hiểm")
            //        +Environment.NewLine+"Vui lòng xóa dòng thuốc này và nhập lại","Thông báo");
            //}
        }

        //KMx: Sau khi kiểm tra, thấy hàm này không còn sử dụng (04/06/2014 09:57).
        //public void checkHIDays(PrescriptionDetail prescriptDObj)
        //{
        //    if (LatestePrecriptions.NDay > xNgayBHToiDa_NgoaiTru)
        //    {
        //        prescriptDObj.DayRpts = xNgayBHToiDa_NgoaiTru;
        //        prescriptDObj.DayExtended = LatestePrecriptions.NDay.GetValueOrDefault() - xNgayBHToiDa_NgoaiTru;
        //    }
        //    else
        //    {
        //        prescriptDObj.DayRpts = LatestePrecriptions.NDay.GetValueOrDefault();
        //    }
        //}

        public void hplEditSchedulesForm_Click()
        {
            if (ObjPrescriptionDetailForForm == null)
            {
                MessageBox.Show(eHCMSResources.A0365_G1_Msg_InfoChonThuocDeRaToa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (!ObjPrescriptionDetailForForm.HasSchedules)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.A0086_G1_Msg_InfoUongThuocTheoLich), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            hplEditSchedules(ObjPrescriptionDetailForForm);
        }

        public void cbxChooseDose_Loaded(object sender, RoutedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                comboBox.ItemsSource = ChooseDoses;
                // Txd Commented out and replace below
                //if (SelectedPrescriptionDetail != null)
                //{
                //    comboBox.SelectedItem = GetChooseDose(SelectedPrescriptionDetail);
                //}
                if (SelectedPrescriptionDetailIsValid())
                {
                    PrescriptionDetail selPrescriptionDetail = LatestePrecriptions.PrescriptionDetails[grdPrescription.SelectedIndex];
                    comboBox.SelectedItem = GetChooseDose(selPrescriptionDetail);
                }
            }
        }

        public void cbxChooseDoseForm_Loaded(object sender, RoutedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                comboBox.ItemsSource = ChooseDoses;
                if (ObjPrescriptionDetailForForm != null)
                {
                    comboBox.SelectedItem = GetChooseDose(ObjPrescriptionDetailForForm);
                }
            }
        }
        #region

        //private int SoNgayTaiKham;
        /*
        public void txtDaysAfter_KeyUp(object sender, KeyEventArgs e)
        {
            int value = 0;
            int.TryParse((sender as TextBox).Text, out value);
            NDayGen = value;
            GetDayRpt(ObjPrescriptionDetailForForm, xNgayBHToiDa, NDayGen);
            if (LatestePrecriptions != null)
            {
                if (LatestePrecriptions.IssueID <= 0) //Thì mới tự động lấy trên xài xuống cho Toa Thuốc, còn Sửa thì phải đợi người ta Bấm Update Ngày Ngày
                {
                    if (SoNgayTaiKham != value && value > 0)
                    {
                        SoNgayTaiKham = value;
                        LatestePrecriptions.NDay = value;
                        //Cap nhat lai ngay va so luong trong toa thuoc
                        AutoAdjustCancelDrugShortDays();
                    }
                }
            }

            if (CtrcboDonVi.SelectedIndex == 0)
            {
                if (value <= 0)
                {
                    CtrtbSoTuan.Text = value.ToString();
                    return;
                }

                if (value > 7)
                {
                    CtrtbSoTuan.Text = (value / 7).ToString();
                }
                else
                {
                    CtrtbSoTuan.Text = "1";
                }
            }
            else
            {
                CtrtbSoTuan.Text = value.ToString();
            }
        }

        public void txtDaysAfter_LostFocus(object sender, RoutedEventArgs e)
        {
            string v = (sender as TextBox).Text;

            if (!string.IsNullOrEmpty(v))
            {
                int num = 0;
                int.TryParse(v, out num);
                
                ((System.Windows.Controls.TextBox)(sender)).Text = num.ToString();//show ra giao diện

                if (LatestePrecriptions != null && LatestePrecriptions.IssueID > 0)
                {
                    if (SoNgayDungThuoc_Root != num)
                    {
                        if (MessageBox.Show(string.Format("{0}! ", eHCMSResources.A0981_G1_Msg_InfoSoNgDungThuocDoi)
                            + Environment.NewLine + "Bạn có đồng ý đổi Ngày Dùng cho toa thuốc không?"
                            , eHCMSResources.G0442_G1_TBao,
                            MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            AutoAdjustCancelDrugShortDays();
                            SoNgayDungThuoc_Root = LatestePrecriptions.NDay;
                        }
                        else
                        {
                            LatestePrecriptions.NDay = SoNgayDungThuoc_Root;
                        }
                    }
                }
            }
        }
        */

        public void ChangeDay(int num)
        {
            //KMx: Bác sĩ yêu cầu không hiện thông báo khi cập nhật ngày dùng (08/05/2014 11:31)
            //if (LatestePrecriptions != null && LatestePrecriptions.IssueID > 0)
            //{
            //    if (MessageBox.Show(string.Format("{0}! ", eHCMSResources.A0981_G1_Msg_InfoSoNgDungThuocDoi) + Environment.NewLine + "Bạn có đồng ý đổi Ngày Dùng cho toa thuốc không?", eHCMSResources.G0442_G1_TBao,
            //            MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            //    {
            //        LatestePrecriptions.NDay = LatestePrecriptions.NDayPreValue; // Txd_15032013: SoNgayDungThuoc_Root da duoc thay the boi LatestePrecriptions.NDayPreValue
            //        return; 
            //    }
            //}

            AutoAdjustCancelDrugShortDays();

        }

        public void tbDayTotal_LostFocus(object sender, RoutedEventArgs e)
        {
            //SplitDayTotal(ObjPrescriptionDetailForForm);

            GetDayRpt(ObjPrescriptionDetailForForm, ObjPrescriptionDetailForForm.RealDay);
        }

        private AxComboBox CtrcboDonVi;
        public void cboDonVi_Loaded(object sender, RoutedEventArgs e)
        {
            CtrcboDonVi = sender as AxComboBox;
        }

        private AxTextBox CtrtbSoTuan;

        public void tbSoTuan_KeyUp(object sender, KeyEventArgs e)
        {
            string v = (sender as TextBox).Text;
            if (!string.IsNullOrEmpty(v))
            {
                int num = 0;
                int.TryParse(v, out num);

                ((System.Windows.Controls.TextBox)(sender)).Text = num.ToString();//show ra giao diện

                if (CtrcboDonVi.SelectedIndex == 0)
                {
                    LatestePrecriptions.NDay = num * 7;
                }
                else
                {
                    LatestePrecriptions.NDay = num;
                }
                // NDayGen = LatestePrecriptions.NDay.Value;
                // GetDayRpt(tempPrescriptionDetail, xNgayBHToiDa, NDayGen);
                // ChangeDay(NDayGen);
            }

        }

        public void cboDonVi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AxComboBox Ctr = sender as AxComboBox;

            string v = CtrtbSoTuan.Text;
            if (!string.IsNullOrEmpty(v))
            {
                int num = 0;
                int.TryParse(v, out num);

                if (Ctr.SelectedIndex == 0)
                {
                    LatestePrecriptions.NDay = num * 7;
                }
                else
                {
                    LatestePrecriptions.NDay = num;
                }
                // NDayGen = LatestePrecriptions.NDay.Value;
            }
        }

        public void tbSoTuan_LostFocus(object sender, RoutedEventArgs e)
        {
            // Txd: 25/09/2013
            //int nDayVal = LatestePrecriptions.NDay == null ? 0 : LatestePrecriptions.NDay.Value;
            //GetDayRpt(ObjPrescriptionDetailForForm, xNgayBHToiDa, nDayVal);
            //SoNgayDungThuocThayDoi(sender, e);
            CheckHIValidDate();
        }
        public void tbSoTuan_Loaded(object sender, RoutedEventArgs e)
        {
            CtrtbSoTuan = sender as AxTextBox;
            CtrtbSoTuan.Focus();
        }
        /*
        public T FindControl(UIElement parent, Type targetType, string ControlName) where T : FrameworkElement
         {

        if (parent == null) return null;

        if (parent.GetType() == targetType && ((T)parent).Name == ControlName)
         {
         return (T)parent;
         }
         T result = null;
         int count = VisualTreeHelper.GetChildrenCount(parent);
         for (int i = 0; i < count; i++)
         {
         UIElement child = (UIElement)VisualTreeHelper.GetChild(parent, i);

        if (FindControl(child, targetType, ControlName) != null)
         {
         result = FindControl(child, targetType, ControlName);
         break;
         }
         }
         return result;
         }
        */

        public void btUpdateNgayDung()
        {
            int nDayVal = LatestePrecriptions.NDay == null ? 0 : LatestePrecriptions.NDay.Value;
            ChangeDay(nDayVal);
        }

        #endregion

        #region Form Cho Thuoc
        AutoCompleteBox AutoThuoc;

        public void AutoThuoc_Loaded(object sender, RoutedEventArgs e)
        {
            AutoThuoc = sender as AutoCompleteBox;
        }
        public void TenThuoc_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ObjPrescriptionDetailForForm != null && ObjPrescriptionDetailForForm.SelectedDrugForPrescription == null)
            {
                ObjPrescriptionDetailForForm.SelectedDrugForPrescription = new GetDrugForSellVisitor();
                ObjPrescriptionDetailForForm.SelectedDrugForPrescription.DispenseVolume = 1;
                ObjPrescriptionDetailForForm.SelectedDrugForPrescription.UnitVolume = 1;
                ObjPrescriptionDetailForForm.SelectedDrugForPrescription.DayRpts = ObjPrescriptionDetailForForm.DayRpts;
                ClearDataRow(ObjPrescriptionDetailForForm);
            }
        }
        public void TenThuoc_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ObjPrescriptionDetailForForm != null
                && ObjPrescriptionDetailForForm.SelectedDrugForPrescription != null
                && ObjPrescriptionDetailForForm.SelectedDrugForPrescription.BrandName != null
                && !ObjPrescriptionDetailForForm.SelectedDrugForPrescription.BrandName.Contains(" *"))
            {
                ObjPrescriptionDetailForForm.BrandName = ObjPrescriptionDetailForForm.SelectedDrugForPrescription.BrandName + " *";
            }
        }
        public void AutoThuocForm_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (ObjPrescriptionDetailForForm == null)
                return;
            if (ObjPrescriptionDetailForForm.IsDrugNotInCat)
            {
                ObjPrescriptionDetailForForm.SelectedDrugForPrescription = new GetDrugForSellVisitor();
                ObjPrescriptionDetailForForm.SelectedDrugForPrescription.BrandName = AutoThuoc.SearchText;
                ClearDataRow(ObjPrescriptionDetailForForm);
            }
            else
            {
                if (AutoThuoc != null && AutoThuoc.SelectedItem != null && AutoThuoc.Text != "" && AutoThuoc.SearchText != "")
                {
                    ObjPrescriptionDetailForForm.SelectedDrugForPrescription = AutoThuoc.SelectedItem as GetDrugForSellVisitor;
                    //ClearDataRow(ObjPrescriptionDetailForForm);

                    // Txd 25/09/2013
                    //ObjPrescriptionDetailForForm = ObjectCopier.DeepCopy(RefreshPrescriptObject(ObjPrescriptionDetailForForm));
                    ObjPrescriptionDetailForForm = NewReInitPrescriptionDetail(true, ObjPrescriptionDetailForForm, true);
                }
            }
        }

        public void AutoThuoc_Populating(object sender, PopulatingEventArgs e)
        {
            //IsEditor = true;
            //if (BrandName != e.Parameter)
            //{
            DrugList.PageIndex = 0;
            BrandName = e.Parameter;
            if (ObjPrescriptionDetailForForm != null)
            {
                if (ObjPrescriptionDetailForForm.IsDrugNotInCat)
                {
                    //return;
                }

                if (ObjPrescriptionDetailForForm.BeOfHIMedicineList)
                {
                    IsInsurance = 1;
                    SearchDrugForPrescription_Paging(StoreID, DrugList.PageIndex, DrugList.PageSize, true);
                }
                else
                {
                    IsInsurance = 0;
                    SearchDrugForPrescription_Paging(StoreID, DrugList.PageIndex, DrugList.PageSize, true);
                }
            }
            //}
        }

        public void btAddNew()
        {
            if (!CheckThuocHopLe(ObjPrescriptionDetailForForm))
            {
                MessageBox.Show(eHCMSResources.K0020_G1_ThuocChonKhongHopLe);
                return;
            }
            RemoveLastRowPrecriptionDetail();

            ObjPrescriptionDetailForForm.isForm = false;
            LatestePrecriptions.PrescriptionDetails.Add(ObjectCopier.DeepCopy(ObjPrescriptionDetailForForm));
            AddNewBlankDrugIntoPrescriptObjectNew();
            AutoThuoc.Focus();
            btClear();
        }

        public void btClear()
        {
            // Txd 24/09/2013
            //ObjPrescriptionDetailForForm = ObjectCopier.DeepCopy(CreateNewPrescriptObject(true));
            ObjPrescriptionDetailForForm = NewReInitPrescriptionDetail(true, null);
            NotifyOfPropertyChange(() => ObjPrescriptionDetailForForm);
            ClearForm();
        }
        AutoCompleteBox DrugAdministrationForm { get; set; }
        public void DrugAdministrationForm_Loaded(object sender, RoutedEventArgs e)
        {
            DrugAdministrationForm = (AutoCompleteBox)sender;
        }
        #endregion

        #region DataGrid
        public void grdPrescription_LoadingRow(object sender, DataGridRowEventArgs e)
        {

            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
            for (int idx = 1; idx < grdPrescription.Columns.Count; idx++)
            {
                //if (idx == (int)DataGridCol.STRENGHT || idx == (int)DataGridCol.UNITS || idx == (int)DataGridCol.UNITUSE || idx == (int)DataGridCol.USAGE || idx == (int)DataGridCol.QTY)
                //{
                //    TextBox obj = grdPrescription.Columns[idx].GetCellContent(e.Row) as TextBox;

                //    if (obj != null)
                //    {
                //        obj.IsReadOnly = true;
                //        //if ((e.Row.GetIndex() % 2) == 1)
                //        obj.Background = new SolidColorBrush(Color.FromArgb(245, 228, 228, 231));
                //        //else
                //        //obj.Background = new SolidColorBrush(Color.FromArgb(255, 248, 248, 248));
                //    }
                //}
            }

            PrescriptionDetail objRows = e.Row.DataContext as PrescriptionDetail;

            if (objRows.HasSchedules)
            {
                e.Row.Background = new SolidColorBrush(Color.FromArgb(205, 180, 200, 120));
            }
            else
            {
                e.Row.Background = new SolidColorBrush(Color.FromArgb(255, 248, 248, 248));
            }

            switch (objRows.V_DrugType)
            {
                case (long)AllLookupValues.V_DrugType.THUOC_UONGKHICAN:
                    e.Row.Background = new SolidColorBrush(Color.FromArgb(205, 180, 200, 120)); break;
            }

            if (objRows.SelectedDrugForPrescription != null
                            && objRows.SelectedDrugForPrescription.MaxDayPrescribed != null
                            && objRows.SelectedDrugForPrescription.MaxDayPrescribed > 0)
            {
                e.Row.Background = new SolidColorBrush(Color.FromArgb(200, 224, 130, 228));//pink
                objRows.BackGroundColor = "#E79DEA";
                NotifyOfPropertyChange(() => e.Row.Background);
            }

            if (Globals.ServerConfigSection.ConsultationElements.EnableTreatmentRegimen && DrugsInTreatmentRegimen != null && DrugsInTreatmentRegimen.Count > 0 && objRows != null && objRows.DrugID.GetValueOrDefault(0) > 0 && !DrugsInTreatmentRegimen.Any(x => x.DrugID == objRows.DrugID))
            {
                e.Row.Background = new SolidColorBrush(Color.FromArgb(46, 204, 113, 248));
            }

            if (objRows.RefGenericDrugDetail != null && objRows.RefGenericDrugDetail.V_CatDrugType == (long)AllLookupValues.V_CatDrugType.DrugDept)
            {
                e.Row.Background = new SolidColorBrush(Color.FromRgb(236, 219, 77)); //Yellow
            }
            //▼====== #009
            if (objRows.IsContraIndicator && Globals.ServerConfigSection.ConsultationElements.AllowBlockContraIndicator)
            {
                e.Row.Background = new SolidColorBrush(Color.FromRgb(255, 69, 0)); //Red
            }
            //▲====== #009
            if (Globals.ServerConfigSection.ConsultationElements.LevelWarningWhenCreateNewAndCopy > 0 && objRows.InsuranceCover == true
                && Registration_DataStorage.CurrentPatientRegistration != null &&
                (Registration_DataStorage.CurrentPatientRegistration.HisID == null || Registration_DataStorage.CurrentPatientRegistration.HisID == 0))
            {
                e.Row.Background = new SolidColorBrush(Color.FromRgb(255, 205, 220)); //Orange
            }
        }

        public void grdPrescription_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            //PrescriptionDetailPreparing = (e.Row.DataContext as PrescriptionDetail).DeepCopy();
        }
        //==== #006 ====
        public void grdPrescription_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!Globals.ServerConfigSection.ConsultationElements.DiagnosisTreatmentForDrug || !IsDiagnosisTreatmentForDrug)
            {
                return;
            }
            PrescriptionDetail item = ((DataGrid)sender).SelectedItem as PrescriptionDetail;
            if (item != null)
            {
                DiagnosisFinalNew = DiagnosisFinalOld = ObjectCopier.DeepCopy(item.SelectedDrugForPrescription.Indication);
            }
            if (item == null)
            {
                DiagnosisFinalNew = DiagnosisFinalOld = "";
            }
        }

        //==== #006 ====
        public void grdPrescription_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                // TxD 20201103: The following was added to FIX the situation where ESCAPE KEY is pressed while a cell was being editted
                //               FIRST TRIAL: SO DO NOTHING HERE to see if it is OK to fix the weird error of doubling rows of Drug in prescription                
                if (sender == null || e == null)
                {
                    // TxD 20201103: This should not happen AT ALL Checking just in case here
                    return;
                }
                if (e != null && e.EditAction == DataGridEditAction.Cancel)
                {
                    e.Cancel = true;
                    return;
                }

                if (LatestePrecriptions.PrescriptionDetails == null)
                {
                    return;
                }

                // Txd 28/092013 
                // For some reason the Grid call this function when focus was outside of the bottom row, so we have to check this to ensure that
                // Removelastrow does not stuff it up.
                if (LatestePrecriptions.PrescriptionDetails.Count <= 0)
                {
                    return;
                }

                //KMx: Click any cell of the last row, then click on space of the gird (click out of grid, error not appear).
                //When you click the save button, the function RemoveLastRowPrecriptionDetail() is called and it remove the last row of LatestePrecriptions.PrescriptionDetails.
                //After remove the last row, caliburn will call grdPrescription_CellEditEnded() and exception occurs at line LatestePrecriptions.PrescriptionDetails[grdPrescription.SelectedIndex] because index was out of range.
                //So we need to check index is valid (04/10/2016 11:43).
                if (grdPrescription.SelectedIndex > LatestePrecriptions.PrescriptionDetails.Count - 1)
                {
                    return;
                }

                //DataGrid axNyGrid = (DataGrid)sender;
                //BindingExpression bindNy = axNyGrid.GetBindingExpression(DataGrid.ItemsSourceProperty);
                //System.Diagnostics.Debug.WriteLine(" =*=*=*====+++>>>> AxDataGridNy Binding Path: " + bindNy.ParentBinding.Path.Path);

                PrescriptionDetail selPrescriptionDetail = null;

                // TxD 29/10/2020: Fix for arrow Up and Down Recalculating Qty SHOULD NOT get SelectedIndex because it has been changed by the arrow key UP or DOWN but using the e.Row.Item still pointing to the previous row before changing to new row 
                if (e.Row != null && e.Row.Item != null)
                {
                    selPrescriptionDetail = (PrescriptionDetail)e.Row.Item;
                }

                if (selPrescriptionDetail == null)
                {
                    return;
                }

                int nDayVal = LatestePrecriptions.NDay == null ? 0 : LatestePrecriptions.NDay.Value;

                if (selPrescriptionDetail.SelectedDrugForPrescription != null
                                && selPrescriptionDetail.SelectedDrugForPrescription.MaxDayPrescribed != null
                                && selPrescriptionDetail.SelectedDrugForPrescription.MaxDayPrescribed > 0)
                {
                    e.Row.Background = new SolidColorBrush(Color.FromArgb(200, 224, 130, 228));
                    selPrescriptionDetail.BackGroundColor = "#E79DEA";
                    //SelectedPrescriptionDetail.BackGroundColor = "#E79DEA";
                    //NotifyOfPropertyChange(() => SelectedPrescriptionDetail.BackGroundColor);
                    NotifyOfPropertyChange(() => selPrescriptionDetail.BackGroundColor);
                }
                if (e.Column.DisplayIndex == (int)DataGridCol.DRUG_NAME)
                {
                    if (selPrescriptionDetail.IsDrugNotInCat && AutoGenMedProduct.SearchText != "")
                    {
                        selPrescriptionDetail.SelectedDrugForPrescription.BrandName = AutoGenMedProduct.SearchText;
                        ClearDataRow(selPrescriptionDetail);
                    }
                }

                //if (ischanged(grdPrescription.SelectedItem)) TxD 20201030 CANNOT use grdPrescription.SelectedItem because when using arrow UP or DOWN Selected Item has already been changed before this CellEditEnding is called.
                if (ischanged(selPrescriptionDetail))
                {
                    if (e.Row.GetIndex() == (LatestePrecriptions.PrescriptionDetails.Count - 1) && e.EditAction == DataGridEditAction.Commit)
                    {
                        //if (((PrescriptionDetail)this.grdPrescription.SelectedItem).V_DrugType == 
                        //    (long)AllLookupValues.V_DrugType.THUOC_NGOAIDANH_MUC)
                        //{
                        //    ((PrescriptionDetail)this.grdPrescription.SelectedItem).IsDrugNotInCat = true;
                        //}
                        AddNewBlankDrugIntoPrescriptObjectNew();
                    }

                    if (e.Column.DisplayIndex == (int)DataGridCol.DRUG_NAME)
                    {
                        if (((PrescriptionDetail)this.grdPrescription.SelectedItem).IsDrugNotInCat == true)
                        {
                            if (Check1ThuocBiDiUng(((PrescriptionDetail)this.grdPrescription.SelectedItem).BrandName))
                            {
                                MessageBox.Show(string.Format(eHCMSResources.Z0990_G1_Thuoc0DiUngBN, ((PrescriptionDetail)this.grdPrescription.SelectedItem).BrandName.Trim()));
                                return;
                            }
                        }
                        else
                        {
                            if (Check1ThuocBiDiUng(((PrescriptionDetail)this.grdPrescription.SelectedItem).SelectedDrugForPrescription.BrandName))
                            {
                                MessageBox.Show(string.Format(eHCMSResources.Z0990_G1_Thuoc0DiUngBN, ((PrescriptionDetail)this.grdPrescription.SelectedItem).SelectedDrugForPrescription.BrandName.Trim()));
                                return;
                            }
                        }


                        if (((PrescriptionDetail)this.grdPrescription.SelectedItem).DrugID != null && (long)((PrescriptionDetail)this.grdPrescription.SelectedItem).DrugID > 0)
                        {
                            Globals.CheckContrain(PtMedCond, (long)((PrescriptionDetail)this.grdPrescription.SelectedItem).DrugID);
                        }
                    }
                    else if (e.Column.DisplayIndex == (int)DataGridCol.QTY)
                    {
                        if (((PrescriptionDetail)this.grdPrescription.SelectedItem).IsDrugNotInCat)
                        {
                            return;
                        }

                        //KMx: Nếu là thuốc cần thì QtyMaxAllowed = Qty (A.Tuấn quyết định) (05/06/2014 16:61).
                        if (selPrescriptionDetail.isNeedToUse)
                        {
                            selPrescriptionDetail.QtyMaxAllowed = selPrescriptionDetail.Qty;
                        }

                        else
                        {
                            AdjustQtyMaxAllowed(selPrescriptionDetail);
                        }

                        if (((PrescriptionDetail)this.grdPrescription.SelectedItem).SelectedDrugForPrescription.Remaining < ((PrescriptionDetail)this.grdPrescription.SelectedItem).Qty)
                        {
                            MessageBox.Show(eHCMSResources.A0977_G1_Msg_InfoSLgKhDuBan);
                        }
                    }

                    //▼====== #007
                    else if (e.Column.DisplayIndex == (int)DataGridCol.MDOSE)
                    {                        
                        selPrescriptionDetail.MDoseStr = AddMoreZeroAndChangeCommaToDot(selPrescriptionDetail.MDoseStr);
                        ChangeAnyDoseQty(1, selPrescriptionDetail != null ? selPrescriptionDetail.MDoseStr : "0", selPrescriptionDetail);
                    }
                    else if (e.Column.DisplayIndex == (int)DataGridCol.NDOSE)
                    {
                        selPrescriptionDetail.NDoseStr = AddMoreZeroAndChangeCommaToDot(selPrescriptionDetail.NDoseStr);
                        ChangeAnyDoseQty(2, selPrescriptionDetail != null ? selPrescriptionDetail.NDoseStr : "0", selPrescriptionDetail);
                    }
                    else if (e.Column.DisplayIndex == (int)DataGridCol.ADOSE)
                    {
                        selPrescriptionDetail.ADoseStr = AddMoreZeroAndChangeCommaToDot(selPrescriptionDetail.ADoseStr);                        
                        ChangeAnyDoseQty(3, selPrescriptionDetail != null ? selPrescriptionDetail.ADoseStr : "0", selPrescriptionDetail);
                    }
                    else if (e.Column.DisplayIndex == (int)DataGridCol.EDOSE)
                    {
                        selPrescriptionDetail.EDoseStr = AddMoreZeroAndChangeCommaToDot(selPrescriptionDetail.EDoseStr);                        
                        ChangeAnyDoseQty(4, selPrescriptionDetail != null ? selPrescriptionDetail.EDoseStr : "0", selPrescriptionDetail);
                    }
                    //▲====== #007

                    //else if (e.Column.DisplayIndex == (int)DataGridCol.DOSAGE)
                    //{
                    //    ChangeDosage(PrescriptionDetailPreparing != null ? PrescriptionDetailPreparing.dosageStr : "0", this.grdPrescription.SelectedItem);
                    //}

                    else if (e.Column.DisplayIndex == (int)DataGridCol.DayTotalCol)
                    {
                        //SplitDayTotal((PrescriptionDetail)this.grdPrescription.SelectedItem);
                        GetDayRpt((PrescriptionDetail)this.grdPrescription.SelectedItem, ((PrescriptionDetail)this.grdPrescription.SelectedItem).RealDay);
                    }

                    //else if (e.Column.DisplayIndex == (int)DataGridCol.DaytExtended)
                    //{
                    //    //ChangeNgayDungExtend(PrescriptionDetailPreparing != null ? PrescriptionDetailPreparing.DayExtended : 0, this.grdPrescription.SelectedItem);
                    //}

                    else if (e.Column.DisplayIndex == (int)DataGridCol.USAGE)
                    {
                        if (selPrescriptionDetail.SelectedDrugForPrescription != null && selPrescriptionDetail.SelectedDrugForPrescription.Administration != null)
                        {
                            selPrescriptionDetail.Administration = selPrescriptionDetail.SelectedDrugForPrescription.Administration;
                        }
                    }
                    else if (e.Column.DisplayIndex == (int)DataGridCol.UNITS)
                    {
                        if (selPrescriptionDetail.SelectedDrugForPrescription != null && selPrescriptionDetail.SelectedDrugForPrescription.UnitName != null)
                        {
                            selPrescriptionDetail.UnitName = selPrescriptionDetail.SelectedDrugForPrescription.UnitName;
                        }
                    }
                    else if (e.Column.DisplayIndex == (int)DataGridCol.UNITUSE)
                    {
                        if (selPrescriptionDetail.SelectedDrugForPrescription != null && selPrescriptionDetail.SelectedDrugForPrescription.UnitUse != null)
                        {
                            selPrescriptionDetail.UnitUse = selPrescriptionDetail.SelectedDrugForPrescription.UnitUse;
                        }
                    }
                    else if (e.Column.DisplayIndex == (int)DataGridCol.NotInCat)
                    {

                    }
                    if (Globals.ServerConfigSection.ConsultationElements.EnableTreatmentRegimen)
                    {
                        if (DrugsInTreatmentRegimen != null && DrugsInTreatmentRegimen.Count > 0 && selPrescriptionDetail != null && selPrescriptionDetail.DrugID.GetValueOrDefault(0) > 0 && !DrugsInTreatmentRegimen.Any(x => x.DrugID == selPrescriptionDetail.DrugID))
                        {
                            e.Row.Background = new SolidColorBrush(Color.FromArgb(46, 204, 113, 248));
                        }
                        else
                        {
                            e.Row.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                        }
                    }
                }

                // TxD 20201116: DropDownClose event when select Drug already called the following method, no need to call it again here ... Commented out to be reviewed
                //GetDiagTreatmentFinal(selPrescriptionDetail.SelectedDrugForPrescription.Indication);
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogError("ePrescriptionOldNewExViewModel grdPrescription_CellEditEnding exception error: " + ex.Message);
            }

        }

        AutoCompleteBox AutoGenMedProduct;
        bool bRePopulateDrugList = true;
        public void acbDrug_Loaded(object sender, RoutedEventArgs e)
        {
            AutoGenMedProduct = sender as AutoCompleteBox;
        }

        // TxD 03/11/2020: Added the following variable to store the current DataGrid selected index that the AutoComplete bOx is being interacted with
        //                  This is to fix the problem when an item is being selected in the AutoCompletebox but the user click another row instead of the current row being worked on 
        //                  So when dropdown close is called another row (being clicked outside of the current autobox) is accidentally changed to the value that was going to be selected
        private int acbDrug_DataGridSelIdx = -10;
        public void acbDrug_Populating(object sender, PopulatingEventArgs e)
        {
            //20190312 TTM: Kiểm tra nếu không phải cấp toa ra về thì không được ra toa, để kiểm tra ở đây là tốt nhất vì không ràng buộc cứng người sử dụng
            //              Và tiện lợi hơn vì Nhập viện, chuyển viện hoặc điều trị ngoại trú cũng cần phải có toa không thuốc mới xác nhận BHYT (Không cần phải ràng buộc nhiều).
            //if (CS_DS != null && CS_DS.DiagTreatment != null && CS_DS.DiagTreatment.V_TreatmentType != (double)AllLookupValues.V_TreatmentType.IssuedPrescription)
            //{
            //    MessageBox.Show(eHCMSResources.Z2604_G1_KhongTheRaToaKhiKhongChonCapToa);
            //    return;
            //}
            acbDrug_DataGridSelIdx = -10;
            if (BrandName != e.Parameter)
            {
            }
            if (!bRePopulateDrugList)
            {
                bRePopulateDrugList = true;
                AutoGenMedProduct.PopulateComplete();
                return;
            }

            if (grdPrescription.SelectedIndex < 0)
                return;

            e.Cancel = true;
            DrugList.PageIndex = 0;
            BrandName = e.Parameter;
            PrescriptionDetail selPrescriptionDetail = LatestePrecriptions.PrescriptionDetails[grdPrescription.SelectedIndex];
            if (selPrescriptionDetail != null)
            {
                if (selPrescriptionDetail.IsDrugNotInCat)
                {
                    return;
                }
                if (selPrescriptionDetail.BeOfHIMedicineList)
                {
                    IsInsurance = 1;
                    SearchDrugForPrescription_Paging(StoreID, DrugList.PageIndex, DrugList.PageSize, true);
                }
                else
                {
                    IsInsurance = 0;
                    SearchDrugForPrescription_Paging(StoreID, DrugList.PageIndex, DrugList.PageSize, true);
                }
                acbDrug_DataGridSelIdx = grdPrescription.SelectedIndex;
            }
        }
        //KMx: Khi dùng chuột chọn 1 loại thuốc trong Drop Down List thì hàm này sẽ được gọi 2 lần. Còn nếu dùng phím Enter để chọn thì chỉ gọi 1 lần (03/06/2014 10:46).
        public void acbDrug_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            Debug.WriteLine("acbDrug_DropDownClosed: " + grdPrescription.SelectedIndex.ToString());
            if (grdPrescription == null )
            {
                return;
            }
            //▼===== #010
            if (grdPrescription.SelectedIndex < 0)
            {
                return;
            }
            //▲===== #010
            if (acbDrug_DataGridSelIdx != grdPrescription.SelectedIndex)
            {
                return;
            }
            acbDrug_DataGridSelIdx = grdPrescription.SelectedIndex;
            PrescriptionDetail selPrescriptionDetail = LatestePrecriptions.PrescriptionDetails[grdPrescription.SelectedIndex];
            if (selPrescriptionDetail.IsDrugNotInCat)
            {
                selPrescriptionDetail.SelectedDrugForPrescription = new GetDrugForSellVisitor();
                selPrescriptionDetail.SelectedDrugForPrescription.BrandName = AutoGenMedProduct.SearchText;
                ClearDataRow(selPrescriptionDetail);
                
            }
            else if (grdPrescription.SelectedItem != null && AutoGenMedProduct != null && AutoGenMedProduct.SelectedItem != null)
            {
                selPrescriptionDetail.SelectedDrugForPrescription = ObjectCopier.DeepCopy(AutoGenMedProduct.SelectedItem as GetDrugForSellVisitor);
                ClearDataRow(selPrescriptionDetail);
            }
            if (selPrescriptionDetail.SelectedDrugForPrescription.HIAllowedPrice > 0)
            {
                if (Globals.ServerConfigSection.CommonItems.ApplyOtherDiagnosis)
                {
                    GetDiagTreatmentOther(selPrescriptionDetail.SelectedDrugForPrescription.Indication);
                }
                else
                {
                    GetDiagTreatmentFinal(selPrescriptionDetail.SelectedDrugForPrescription.Indication);
                }
            }
        }
        public void acbDrug_LostFocus(object sender, RoutedEventArgs e)
        {
        }

        public void acbDrug_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            bRePopulateDrugList = false;
            //AutoGenMedProduct.IsDropDownOpen = true;
        }

        public void acbDrug_TextChanged(object sender, RoutedEventArgs e)
        {
            bRePopulateDrugList = true;
        }

        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            PrescriptionDetail p = grdPrescription.SelectedItem as PrescriptionDetail;
            if (p == null)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0405_G1_KgTheXoaDongRong));
                return;
            }

            int nSelIndex = grdPrescription.SelectedIndex;
            if (nSelIndex >= LatestePrecriptions.PrescriptionDetails.Count - 1)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0405_G1_KgTheXoaDongRong));
                return;
            }

            if (p.DrugID > 0 || p.IsDrugNotInCat)
            {
                //string strMsg = "Bạn Có Chắc Muốn Xóa Dòng Thuoc [" + p.BrandName +"] Này Không ?";
                string strMsg = eHCMSResources.Z0554_G1_CoChacMuonXoaDongNayKg;
                if (MessageBox.Show(strMsg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                {
                    return;
                }
            }

            LatestePrecriptions.PrescriptionDetails.Remove(p);
            
            if (Globals.ServerConfigSection.CommonItems.ApplyOtherDiagnosis)
            {
                if (Globals.ServerConfigSection.ConsultationElements.DiagnosisTreatmentForDrug && IsDiagnosisTreatmentForDrug && p.SelectedDrugForPrescription.Indication != ""
                    && p.SelectedDrugForPrescription.Indication != null && CS_DS.DiagTreatment.DiagnosisOther != null && CS_DS.DiagTreatment.ServiceRecID == ObjDiagnosisTreatment_Current.ServiceRecID)
                {
                    CS_DS.DiagTreatment.DiagnosisOther = CS_DS.DiagTreatment.DiagnosisOther.Replace("; " + p.SelectedDrugForPrescription.Indication, "");
                }
            }
            else
            {
                //==== #006 ====
                if (Globals.ServerConfigSection.ConsultationElements.DiagnosisTreatmentForDrug && IsDiagnosisTreatmentForDrug && p.SelectedDrugForPrescription.Indication != ""
                    && p.SelectedDrugForPrescription.Indication != null && CS_DS.DiagTreatment.DiagnosisFinal != null && CS_DS.DiagTreatment.ServiceRecID == ObjDiagnosisTreatment_Current.ServiceRecID)
                {
                    CS_DS.DiagTreatment.DiagnosisFinal = CS_DS.DiagTreatment.DiagnosisFinal.Replace("; " + p.SelectedDrugForPrescription.Indication, "");
                }
                //==== #006 ====

            }
            /*▼====: #001*/
            LatestePrecriptions.SetDataChanged();
            /*▲====: #001*/
            if (LatestePrecriptions.PrescriptionDetails.Count < 2)
            {
                LatestePrecriptions.PreNoDrug = true;
                PreNoDrug = false;
                NotifyOfPropertyChange(() => LatestePrecriptions.PreNoDrug);
            }
        }

        public void grdPrescription_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (grdPrescription != null)
            {
                if (grdPrescription.SelectedItem != null)
                {
                    grdPrescription.BeginEdit();

                    // TxD 22/10/2020: Try to fix on click on DataGrid wil trigger begin edit of Control embedded inside DataGridCell
                    // The following code try to find the DataGridCell where the Mouse was clicked.
                    var hit = VisualTreeHelper.HitTest((Visual)sender, e.GetPosition((IInputElement)sender));
                    if (hit != null)    // hit == null when left mouse button is pressed to select an item in the AutoCompleteBox or ComboBox embedded insid the DataGrid itself.
                    {
                        DependencyObject cell = VisualTreeHelper.GetParent(hit.VisualHit);
                        while (cell != null && !(cell is DataGridCell))
                        {
                            cell = VisualTreeHelper.GetParent(cell);
                        }
                        DataGridCell targetCell = cell as System.Windows.Controls.DataGridCell;

                        if (targetCell != null)
                        {
                            Control control = Globals.GetFirstChildByType<Control>(targetCell);
                            if (control != null)
                            {
                                control.Focus();
                            }
                        }
                    }
                }
            }
        }

        #endregion


        #region autocomplete box for DVT, Cach Dung, Ghi Chu

        //KMx: Code này được copy từ PatientDetailsViewModel.cs chỗ tìm kiếm Tp/Tỉnh (31/05/2014 11:35).
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

        public void DrugUnitUse_Loaded(object sender, PopulatingEventArgs e)
        {
            ((AxAutoComplete)sender).ItemsSource = DonViTinh;
        }
        public void DrugUnitUse_Populating(object sender, PopulatingEventArgs e)
        {
            if (SelectedPrescriptionDetailIsValid())
            {
                PrescriptionDetail selPrescriptionDetail = LatestePrecriptions.PrescriptionDetails[grdPrescription.SelectedIndex];
                selPrescriptionDetail.UnitUse = e.Parameter;
            }
        }
        public void DrugUnitUse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!SelectedPrescriptionDetailIsValid())
            {
                MessageBox.Show(eHCMSResources.A0534_G1_Msg_InfoDongThuocKhHopLe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            PrescriptionDetail selPrescriptionDetail = LatestePrecriptions.PrescriptionDetails[grdPrescription.SelectedIndex];
            if (grdPrescription != null && grdPrescription.SelectedItem != null && ((AxAutoComplete)sender) != null)
            {
                if (((AxAutoComplete)sender).SelectedItem != null)
                {
                    selPrescriptionDetail.UnitUse = ((PrescriptionNoteTemplates)((AxAutoComplete)sender).SelectedItem).NoteDetails;
                }
                else
                {
                    selPrescriptionDetail.UnitUse = ((AxAutoComplete)sender).Text;
                }
            }
        }

        public void DrugInstructionNotes_Loaded(object sender, PopulatingEventArgs e)
        {
            ((AxAutoComplete)sender).ItemsSource = GhiChu;
        }

        private ObservableCollection<PrescriptionNoteTemplates> _GhiChuAfterFiltering;
        public ObservableCollection<PrescriptionNoteTemplates> GhiChuAfterFiltering
        {
            get { return _GhiChuAfterFiltering; }
            set
            {
                _GhiChuAfterFiltering = value;
                NotifyOfPropertyChange(() => GhiChuAfterFiltering);
            }
        }

        //KMx: Code này được copy từ PatientDetailsViewModel.cs chỗ tìm kiếm Tp/Tỉnh (31/05/2014 11:35).
        public void DrugInstructionNotes_Populating(object sender, PopulatingEventArgs e)
        {
            if (sender != null)
            {
                string SearchText = ((AutoCompleteBox)sender).SearchText;
                GhiChuAfterFiltering = new ObservableCollection<PrescriptionNoteTemplates>(GhiChu.Where(item => ConvertString(item.NoteDetails)
                     .IndexOf(ConvertString(SearchText), StringComparison.InvariantCultureIgnoreCase) >= 0));
                ((AutoCompleteBox)sender).ItemsSource = GhiChuAfterFiltering;
                ((AutoCompleteBox)sender).PopulateComplete();
            }


            //if (SelectedPrescriptionDetail != null)
            //{
            //    SelectedPrescriptionDetail.DrugInstructionNotes = e.Parameter;
            //}
        }

        public void DrugInstructionNotes_LostFocus(object sender, RoutedEventArgs e)
        {
            //if (grdPrescription != null && grdPrescription.SelectedItem != null && ((AxAutoComplete)sender) != null)
            //{
            //    if (((AxAutoComplete)sender).SelectedItem != null)
            //    {
            //        SelectedPrescriptionDetail.DrugInstructionNotes = ((PrescriptionNoteTemplates)((AxAutoComplete)sender).SelectedItem).NoteDetails;
            //    }
            //    else
            //    {
            //        SelectedPrescriptionDetail.DrugInstructionNotes = ((AxAutoComplete)sender).Text;
            //    }
            //}
        }
        public void DrugInstructionNotes_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            //if (grdPrescription != null && grdPrescription.SelectedItem != null && ((AxAutoComplete)sender) != null)
            //{
            //    if (((AxAutoComplete)sender).SelectedItem != null)
            //    {
            //        PrescriptionDetailPreparing.DrugInstructionNotes = ((PrescriptionNoteTemplates)((AxAutoComplete)sender).SelectedItem).NoteDetails;
            //    }
            //    else
            //    {
            //        PrescriptionDetailPreparing.DrugInstructionNotes = ((AxAutoComplete)sender).Text;
            //    }
            //}
        }

        public void DrugInstructionNotesForm_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            //if (((AxAutoComplete)sender) != null)
            //{
            //    if (((AxAutoComplete)sender).SelectedItem != null)
            //    {
            //        tempPrescriptionDetail.DrugInstructionNotes = ((PrescriptionNoteTemplates)((AxAutoComplete)sender).SelectedItem).NoteDetails;
            //    }
            //    else
            //    {
            //        tempPrescriptionDetail.DrugInstructionNotes = ((AxAutoComplete)sender).Text;
            //    }
            //}
        }

        public void DrugAdministration_Loaded(object sender, PopulatingEventArgs e)
        {
            ((AxAutoComplete)sender).ItemsSource = CachDung;
        }
        //public void DrugAdministration_Populating(object sender, PopulatingEventArgs e)
        //{
        //    //if (SelectedPrescriptionDetail != null)
        //    //{
        //    //    SelectedPrescriptionDetail.Administration = e.Parameter;
        //    //}
        //}

        private ObservableCollection<PrescriptionNoteTemplates> _CachDungAfterFiltering;
        public ObservableCollection<PrescriptionNoteTemplates> CachDungAfterFiltering
        {
            get { return _CachDungAfterFiltering; }
            set
            {
                _CachDungAfterFiltering = value;
                NotifyOfPropertyChange(() => CachDungAfterFiltering);
            }
        }

        //KMx: Code này được copy từ PatientDetailsViewModel.cs chỗ tìm kiếm Tp/Tỉnh (31/05/2014 11:35).
        public void DrugAdministration_Populating(object sender, PopulatingEventArgs e)
        {
            if (sender != null)
            {
                string SearchText = ((AutoCompleteBox)sender).SearchText;
                CachDungAfterFiltering = new ObservableCollection<PrescriptionNoteTemplates>(CachDung.Where(item => ConvertString(item.NoteDetails)
                     .IndexOf(ConvertString(SearchText), StringComparison.InvariantCultureIgnoreCase) >= 0));
                ((AutoCompleteBox)sender).ItemsSource = CachDungAfterFiltering;
                ((AutoCompleteBox)sender).PopulateComplete();
            }
        }

        public void DrugAdministration_LostFocus(object sender, RoutedEventArgs e)
        {
            //if (grdPrescription != null && grdPrescription.SelectedItem != null && ((AxAutoComplete)sender) != null)
            //{
            //    if (((AxAutoComplete)sender).SelectedItem != null)
            //    {
            //        SelectedPrescriptionDetail.Administration = ((PrescriptionNoteTemplates)((AxAutoComplete)sender).SelectedItem).NoteDetails;
            //    }
            //    else
            //    {
            //        SelectedPrescriptionDetail.Administration = ((AxAutoComplete)sender).Text;
            //    }
            //}
        }

        public void DrugAdministration_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            //if (grdPrescription != null && grdPrescription.SelectedItem != null && ((AxAutoComplete)sender) != null)
            //{
            //    SelectedPrescriptionDetail.Administration = SelectedPrescriptionDetail.SelectedDrugForPrescription.dru;
            //}
        }

        public void DrugAdministrationForm_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if ((AxAutoComplete)sender != null)
            {
                if (((AxAutoComplete)sender).SelectedItem != null)
                {
                    ObjPrescriptionDetailForForm.Administration = ((PrescriptionNoteTemplates)((AxAutoComplete)sender).SelectedItem).NoteDetails;
                }
                else
                {
                    ObjPrescriptionDetailForForm.Administration = ((AxAutoComplete)sender).Text;
                }
            }
        }
        #endregion
        #region PrescriptionNoteTemplates

        private PrescriptionNoteTemplates _ObjPrescriptionNoteTemplates_Selected;
        public PrescriptionNoteTemplates ObjPrescriptionNoteTemplates_Selected
        {
            get { return _ObjPrescriptionNoteTemplates_Selected; }
            set
            {
                if (_ObjPrescriptionNoteTemplates_Selected == value)
                {
                    return;
                }
                _ObjPrescriptionNoteTemplates_Selected = value;
                NotifyOfPropertyChange(() => ObjPrescriptionNoteTemplates_Selected);

                if (_ObjPrescriptionNoteTemplates_Selected != null && _ObjPrescriptionNoteTemplates_Selected.PrescriptNoteTemplateID > 0)
                {
                    string str = LatestePrecriptions.DoctorAdvice;
                    if (string.IsNullOrEmpty(str))
                    {
                        str = ObjPrescriptionNoteTemplates_Selected.DetailsTemplate;
                    }
                    else
                    {
                        str = str + Environment.NewLine + ObjPrescriptionNoteTemplates_Selected.DetailsTemplate;
                    }

                    LatestePrecriptions.DoctorAdvice = str;
                }
            }
        }

        private ObservableCollection<PrescriptionNoteTemplates> _ObjPrescriptionNoteTemplates_GetAll;
        public ObservableCollection<PrescriptionNoteTemplates> ObjPrescriptionNoteTemplates_GetAll
        {
            get { return _ObjPrescriptionNoteTemplates_GetAll; }
            set
            {
                _ObjPrescriptionNoteTemplates_GetAll = value;
                NotifyOfPropertyChange(() => ObjPrescriptionNoteTemplates_GetAll);
            }
        }

        private ObservableCollection<PrescriptionNoteTemplates> _DonViTinh;
        public ObservableCollection<PrescriptionNoteTemplates> DonViTinh
        {
            get { return _DonViTinh; }
            set
            {
                _DonViTinh = value;
                NotifyOfPropertyChange(() => DonViTinh);
            }
        }

        private ObservableCollection<PrescriptionNoteTemplates> _GhiChu;
        public ObservableCollection<PrescriptionNoteTemplates> GhiChu
        {
            get { return _GhiChu; }
            set
            {
                _GhiChu = value;
                NotifyOfPropertyChange(() => GhiChu);
            }
        }

        private ObservableCollection<PrescriptionNoteTemplates> _CachDung;
        public ObservableCollection<PrescriptionNoteTemplates> CachDung
        {
            get { return _CachDung; }
            set
            {
                _CachDung = value;
                NotifyOfPropertyChange(() => CachDung);
            }
        }

        public void PrescriptionNoteTemplates_GetAllIsActive()
        {
            var t = new Thread(() =>
            {
                IsWaitingPrescriptionNoteTemplates_GetAll = true;

                try
                {
                    using (var serviceFactory = new ePrescriptionsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        var pnt = new PrescriptionNoteTemplates();
                        pnt.V_PrescriptionNoteTempType = AllLookupValues.V_PrescriptionNoteTempType.PrescriptionNoteGen;
                        contract.BeginPrescriptionNoteTemplates_GetAllIsActive(pnt, Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<PrescriptionNoteTemplates> allItems = new ObservableCollection<PrescriptionNoteTemplates>();
                            try
                            {
                                allItems = contract.EndPrescriptionNoteTemplates_GetAllIsActive(asyncResult);

                                ObjPrescriptionNoteTemplates_GetAll = new ObservableCollection<PrescriptionNoteTemplates>(allItems);

                                PrescriptionNoteTemplates firstItem = new PrescriptionNoteTemplates();
                                firstItem.PrescriptNoteTemplateID = -1;
                                firstItem.NoteDetails = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K0616_G1_ChonMau);
                                ObjPrescriptionNoteTemplates_GetAll.Insert(0, firstItem);

                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    IsWaitingPrescriptionNoteTemplates_GetAll = false;
                }
            });
            t.Start();
        }
        public void PrescriptionNoteTemplates_GetAllIsActiveItem()
        {
            var t = new Thread(() =>
            {
                IsWaitingPrescriptionNoteTemplates_GetAll = true;

                try
                {
                    using (var serviceFactory = new ePrescriptionsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        var pnt = new PrescriptionNoteTemplates();
                        pnt.V_PrescriptionNoteTempType = AllLookupValues.V_PrescriptionNoteTempType.PrescriptionNoteItem;
                        contract.BeginPrescriptionNoteTemplates_GetAllIsActive(pnt, Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<PrescriptionNoteTemplates> allItems = new ObservableCollection<PrescriptionNoteTemplates>();
                            try
                            {
                                allItems = contract.EndPrescriptionNoteTemplates_GetAllIsActive(asyncResult);
                                GhiChu = new ObservableCollection<PrescriptionNoteTemplates>(allItems.OrderBy(x => x.NoteDetails));
                                NotifyOfPropertyChange(() => GhiChu);
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    IsWaitingPrescriptionNoteTemplates_GetAll = false;
                }
            });
            t.Start();
        }

        public void PrescriptionNoteTemplates_GetAllIsActiveAdm()
        {
            var t = new Thread(() =>
            {
                IsWaitingPrescriptionNoteTemplates_GetAll = true;

                try
                {
                    using (var serviceFactory = new ePrescriptionsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        var pnt = new PrescriptionNoteTemplates();
                        pnt.V_PrescriptionNoteTempType = AllLookupValues.V_PrescriptionNoteTempType.PrescriptionAdministration;
                        contract.BeginPrescriptionNoteTemplates_GetAllIsActive(pnt, Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<PrescriptionNoteTemplates> allItems = new ObservableCollection<PrescriptionNoteTemplates>();
                            try
                            {
                                allItems = contract.EndPrescriptionNoteTemplates_GetAllIsActive(asyncResult);
                                CachDung = new ObservableCollection<PrescriptionNoteTemplates>(allItems.OrderBy(x => x.NoteDetails));
                                NotifyOfPropertyChange(() => CachDung);
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    IsWaitingPrescriptionNoteTemplates_GetAll = false;
                }
            });
            t.Start();
        }

        //public void PrescriptionNoteTemplates_GetAllIsActiveUnitUse()
        //{
        //    var t = new Thread(() =>
        //    {
        //        IsWaitingPrescriptionNoteTemplates_GetAll = true;

        //        try
        //        {
        //            using (var serviceFactory = new ePrescriptionsServiceClient())
        //            {
        //                var contract = serviceFactory.ServiceInstance;
        //                var pnt = new PrescriptionNoteTemplates();
        //                pnt.V_PrescriptionNoteTempType = AllLookupValues.V_PrescriptionNoteTempType.PrescriptionUnitUse;
        //                contract.BeginPrescriptionNoteTemplates_GetAllIsActive(pnt, Globals.DispatchCallback((asyncResult) =>
        //                {
        //                    IList<PrescriptionNoteTemplates> allItems = new ObservableCollection<PrescriptionNoteTemplates>();
        //                    try
        //                    {
        //                        allItems = contract.EndPrescriptionNoteTemplates_GetAllIsActive(asyncResult);
        //                        DonViTinh = new ObservableCollection<PrescriptionNoteTemplates>(allItems.OrderBy(x => x.NoteDetails));
        //                        NotifyOfPropertyChange(() => DonViTinh);
        //                    }
        //                    catch (FaultException<AxException> fault)
        //                    {

        //                    }
        //                    catch (Exception ex)
        //                    {
        //                    }

        //                }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //        }
        //        finally
        //        {
        //            IsWaitingPrescriptionNoteTemplates_GetAll = false;
        //        }
        //    });
        //    t.Start();
        //}

        public void chk_NotInCat_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        public void chk_NotInCat_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        public void UnitName_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        public void UnitName_GotFocus(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        public void RefreshLookup()
        {
            PrescriptionNoteTemplates_GetAllIsActive();
            PrescriptionNoteTemplates_GetAllIsActiveItem();
            PrescriptionNoteTemplates_GetAllIsActiveAdm();
            //PrescriptionNoteTemplates_GetAllIsActiveUnitUse();
        }
        public void GetToaThuocDaCo(long PrescriptID = 0)
        {
            btnCreateNewIsEnabled = true;
            btnSaveAddNewIsEnabled = false;

            btnCreateAndCopyIsEnabled = true;
            btnCopyToIsEnabled = true;
            IsEnabledPrint = true;

            //KMx: Khi load toa thuốc thì Enable của form phải bằng false, khi nào người dùng nhấn "Chỉnh sửa" thì Enable mới bằng true.
            // Nếu không có dòng dưới thì sẽ bị sai khi đang "chỉnh sửa" toa cho BN A, chọn BN B từ out standing task, thì toa của BN B tự động Enable (29/05/2014 15:56).
            IsEnabled = false;

            if (Registration_DataStorage.PatientServiceRecordCollection == null)
            {
                btnEditIsEnabled = false;
                return;
            }

            btnEditIsEnabled = true;

            LatestePrecriptions = Registration_DataStorage.PatientServiceRecordCollection[0].PrescriptionIssueHistories[0].Prescription;
            //▼====== #005: Không biết lý do tại sao lại new PrescriptionIssueHistory nên cần phải declare 1 biến để lưu tạm và trả lại sau khi đc new.
            bool tmpIsOutCatConfirmed = LatestePrecriptions.PrescriptionIssueHistory.IsOutCatConfirmed;
            LatestePrecriptions.PrescriptionIssueHistory = Registration_DataStorage.PatientServiceRecordCollection[0].PrescriptionIssueHistories[0];
            LatestePrecriptions.PrescriptionIssueHistory.IsOutCatConfirmed = tmpIsOutCatConfirmed;
            //▲====== #005
            if (!IsUpdateWithoutChangeDoctorIDAndDatetime)
            {
                if (!CheckToaThuocDuocPhepCapNhat(DuocSi_IsEditingToaThuoc, LatestePrecriptions))
                {
                    btnEditIsEnabled = false;
                }
            }
            else
            {
                btnEditIsEnabled = true;
            }
            if (LatestePrecriptions.PrescriptionIssueHistory.IssuedDateTime.HasValue && !IsShowValidateExpiredPrescription)
            {
                ValidateExpiredPrescription(LatestePrecriptions);
            }
            else if (IsShowValidateExpiredPrescription)
            {
                btnCopyToIsEnabled = false;
                btnEditIsEnabled = false;
            }
            LatestePrecriptions.PrescriptionIssueHistory.DeptLocID = Globals.DeptLocation.DeptLocationID;


            if (LatestePrecriptions.ObjCreatorStaffID.StaffID != ObjStaff.StaffID)
            {
                LatestePrecriptions.PrescriptionIssueHistory.OrigCreatorDoctorNames += " - " + ObjStaff.FullName;
            }

            //▼===== 20191011 TTM: Loại bỏ Globals.SecretaryLogin
            //if (Globals.SecretaryLogin != null)
            //{
            //    LatestePrecriptions.SecretaryStaff = Globals.SecretaryLogin.Staff;
            //}
            //▲=====

            PrecriptionsForPrint = LatestePrecriptions;
            ContentKhungTaiKhamIsEnabled = LatestePrecriptions.IsAllowEditNDay;

            if (LatestePrecriptions.NDay > 0)
            {
                chkHasAppointmentValue = true;
            }
            else
            {
                chkHasAppointmentValue = false;
            }
            LatestePrecriptions.CanEdit = true;

            if (LatestePrecriptions.NDay >= 0 && CtrtbSoTuan != null)
            {
                CtrtbSoTuan.Text = LatestePrecriptions.NDay.ToString();
            }
            //▼====== 20181101 TTM: Lấy giá trị toa không thuốc lên.
            PreNoDrug = LatestePrecriptions.IsEmptyPrescription;
            //▲======
            GetPrescriptionDetailsByPrescriptID(PrescriptID > 0 ? PrescriptID : LatestePrecriptions.PrescriptID);
            //▼====== #008
            if (LatestePrecriptions.HasAppointment && LatestePrecriptions.AppointmentDate == null && Globals.ServerConfigSection.ConsultationElements.AppointmentAuto)
            {
                PatientAppointments_Save(true);
            }
            //▲====== #008
        }

        //KMx: Hàm này được kết hợp từ 2 hàm GetPrescriptionTypes_New() và GetPrescriptionTypes_DaCo() (17/05/2014 11:15).
        //Lý do: Bỏ bước đi về server lấy Lookup và 2 hàm đó giống nhau.
        private void GetPrescription(bool IsRegDetailHasPrescript, long PrescriptID = 0)
        {

            InitialNewPrescription();

            SetToaBaoHiem_KhongBaoHiem();
            if (IsRegDetailHasPrescript)
            {
                GetToaThuocDaCo(PrescriptID);
            }
            else if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatient != null)
            {
                GetLatestPrescriptionByPtID_New(Registration_DataStorage.CurrentPatient.PatientID);
            }
        }


        public void btnAddPreTemplate()
        {
            //LatestePrecriptions
            Action<IPrescriptionTemplateComment> onInitDlg = delegate (IPrescriptionTemplateComment typeInfo)
            {
                typeInfo.Comment = LatestePrecriptions.Diagnosis;
                var instance = typeInfo as Conductor<object>;
                var deActive = instance as IDeactivate;
                if (deActive == null)
                {
                    Completed(this, new ResultCompletionEventArgs());
                }
                else
                {
                    deActive.Deactivated += new EventHandler<DeactivationEventArgs>((o, e) =>
                    {
                        if (e.WasClosed)
                        {
                            PrescriptionTemplate curPre = new PrescriptionTemplate();
                            curPre.Comment = typeInfo.Comment;
                            curPre.PrescriptID = LatestePrecriptions.PrescriptID;
                            curPre.DoctorStaffID = Globals.LoggedUserAccount.StaffID;
                            PrescriptionsTemplateInsert(curPre);
                        }
                    });
                }
            };
            GlobalsNAV.ShowDialog<IPrescriptionTemplateComment>(onInitDlg);
        }
        void deActive_Deactivated(object sender, DeactivationEventArgs e)
        {
            if (e.WasClosed)
            {
                Completed(this, new ResultCompletionEventArgs());
            }
        }

        public object Dialog { get; private set; }

        public event EventHandler<ResultCompletionEventArgs> Completed;
        private void PrescriptionsTemplateInsert(PrescriptionTemplate obj)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsWaitingGetPrescriptionTypes = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPrescriptionsTemplateInsert(obj, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndPrescriptionsTemplateInsert(asyncResult);
                            MessageBox.Show(string.Format("{0}!", eHCMSResources.A1027_G1_Msg_InfoThemOK));
                            Globals.EventAggregator.Publish(new ReloadePrescriptionTemplateEvent());
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            IsWaitingGetPrescriptionTypes = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }
        //==== #006 ====
        private string DiagnosisFinalOld = "";
        private string DiagnosisFinalNew = "";
        private void GetDiagTreatmentFinal(string DiagTreatment)
        {
            if (!Globals.ServerConfigSection.ConsultationElements.DiagnosisTreatmentForDrug || !IsDiagnosisTreatmentForDrug || string.IsNullOrEmpty(DiagTreatment))
            {
                return;
            }
            if (CS_DS.DiagTreatment.DiagnosisFinal == null)
            {
                CS_DS.DiagTreatment.DiagnosisFinal = "";
            }
            DiagnosisFinalNew = DiagTreatment;
            if (!string.IsNullOrEmpty(DiagnosisFinalOld) && (CS_DS.DiagTreatment.ServiceRecID == null || CS_DS.DiagTreatment.ServiceRecID == ObjDiagnosisTreatment_Current.ServiceRecID)) //20191127 TBL: BM 0019663: Nếu tạo mới toa thuốc và ra thuốc có thêm chẩn đoán nhưng vẫn là chẩn đoán cũ của đăng ký trước thì không thêm chẩn đoán của thuốc vào để tránh lỗi lại cập nhật chẩn đoán cũ
            {
                CS_DS.DiagTreatment.DiagnosisFinal = CS_DS.DiagTreatment.DiagnosisFinal.Replace(DiagnosisFinalOld, DiagnosisFinalNew);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(CS_DS.DiagTreatment.DiagnosisFinal))
                {
                    CS_DS.DiagTreatment.DiagnosisFinal += DiagnosisFinalNew;
                }
                else if (DiagTreatment != "" && (CS_DS.DiagTreatment.ServiceRecID == null || CS_DS.DiagTreatment.ServiceRecID == ObjDiagnosisTreatment_Current.ServiceRecID)) //20191127 TBL: BM 0019663: Nếu tạo mới toa thuốc và ra thuốc có thêm chẩn đoán nhưng vẫn là chẩn đoán cũ của đăng ký trước thì không thêm chẩn đoán của thuốc vào để tránh lỗi lại cập nhật chẩn đoán cũ
                {
                    CS_DS.DiagTreatment.DiagnosisFinal += "; " + DiagnosisFinalNew;
                }
            }
            DiagnosisFinalOld = ObjectCopier.DeepCopy(DiagnosisFinalNew);

        }
        private void GetDiagTreatmentOther(string DiagTreatment)
        {
            if (!Globals.ServerConfigSection.ConsultationElements.DiagnosisTreatmentForDrug || !IsDiagnosisTreatmentForDrug || string.IsNullOrEmpty(DiagTreatment))
            {
                return;
            }
            if (CS_DS.DiagTreatment.DiagnosisOther == null)
            {
                CS_DS.DiagTreatment.DiagnosisOther = "";
            }
            DiagnosisFinalNew = DiagTreatment;
            if (!string.IsNullOrEmpty(DiagnosisFinalOld) && (CS_DS.DiagTreatment.ServiceRecID == null || CS_DS.DiagTreatment.ServiceRecID == ObjDiagnosisTreatment_Current.ServiceRecID)) //20191127 TBL: BM 0019663: Nếu tạo mới toa thuốc và ra thuốc có thêm chẩn đoán nhưng vẫn là chẩn đoán cũ của đăng ký trước thì không thêm chẩn đoán của thuốc vào để tránh lỗi lại cập nhật chẩn đoán cũ
            {
                CS_DS.DiagTreatment.DiagnosisOther = CS_DS.DiagTreatment.DiagnosisOther.Replace(DiagnosisFinalOld, DiagnosisFinalNew);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(CS_DS.DiagTreatment.DiagnosisOther))
                {
                    CS_DS.DiagTreatment.DiagnosisOther += DiagnosisFinalNew;
                }
                else if (DiagTreatment != "" && (CS_DS.DiagTreatment.ServiceRecID == null || CS_DS.DiagTreatment.ServiceRecID == ObjDiagnosisTreatment_Current.ServiceRecID)) //20191127 TBL: BM 0019663: Nếu tạo mới toa thuốc và ra thuốc có thêm chẩn đoán nhưng vẫn là chẩn đoán cũ của đăng ký trước thì không thêm chẩn đoán của thuốc vào để tránh lỗi lại cập nhật chẩn đoán cũ
                {
                    CS_DS.DiagTreatment.DiagnosisOther += "; " + DiagnosisFinalNew;
                }
            }
            DiagnosisFinalOld = ObjectCopier.DeepCopy(DiagnosisFinalNew);

        }
        private bool _IsDiagnosisTreatmentForDrug = Globals.ServerConfigSection.ConsultationElements.DiagnosisTreatmentForDrug;
        public bool IsDiagnosisTreatmentForDrug
        {
            get => _IsDiagnosisTreatmentForDrug; set
            {
                _IsDiagnosisTreatmentForDrug = value;
                NotifyOfPropertyChange(() => IsDiagnosisTreatmentForDrug);
            }
        }
        private bool _IsDiagnosisTreatmentForDrugVisibility = Globals.ServerConfigSection.ConsultationElements.DiagnosisTreatmentForDrug;
        public bool IsDiagnosisTreatmentForDrugVisibility
        {
            get => _IsDiagnosisTreatmentForDrugVisibility; set
            {
                _IsDiagnosisTreatmentForDrugVisibility = value;
                NotifyOfPropertyChange(() => IsDiagnosisTreatmentForDrugVisibility);
            }
        }
        //==== #006 ====

        #region button control

        public enum PrescriptionState
        {
            //Tao moi toa thuoc
            NewPrescriptionState = 1,
            //Hieu chinh toa thuoc voi bac si da ra toa
            EditPrescriptionState = 2,
            //Phat hanh lai toa thuoc neu la voi bac si khac
            //cho chinh sua va mark delete doi voi toa cu
            PublishNewPrescriptionState = 3,
        }


        private PrescriptionState _PrescripState = PrescriptionState.NewPrescriptionState;
        public PrescriptionState PrescripState
        {
            get
            {
                return _PrescripState;
            }
            set
            {
                //if (_PrescripState != value)
                {
                    _PrescripState = value;
                    NotifyOfPropertyChange(() => PrescripState);
                    switch (PrescripState)
                    {
                        case PrescriptionState.NewPrescriptionState:
                            mNewPrescriptionState = true && btnCreateNewVisibility;
                            mEditPrescriptionState = false;
                            mPublishNewPrescriptionState = false;
                            break;
                        case PrescriptionState.EditPrescriptionState:
                            mNewPrescriptionState = false;
                            mEditPrescriptionState = true && btnUpdateVisibility;
                            mPublishNewPrescriptionState = false;
                            break;
                        case PrescriptionState.PublishNewPrescriptionState:
                            mNewPrescriptionState = false;
                            mEditPrescriptionState = false;
                            mPublishNewPrescriptionState = true && btnCopyToVisible;
                            break;
                    }
                }
            }
        }

        private bool _mNewPrescriptionState;
        public bool mNewPrescriptionState
        {
            get
            {
                return _mNewPrescriptionState;
            }
            set
            {
                if (_mNewPrescriptionState != value)
                {
                    _mNewPrescriptionState = value;
                    NotifyOfPropertyChange(() => mNewPrescriptionState);
                }
            }
        }

        private bool _mEditPrescriptionState;
        public bool mEditPrescriptionState
        {
            get
            {
                return _mEditPrescriptionState;
            }
            set
            {
                if (_mEditPrescriptionState != value)
                {
                    _mEditPrescriptionState = value;
                    NotifyOfPropertyChange(() => mEditPrescriptionState);
                    NotifyOfPropertyChange(() => mUpdatePrescriptionState);
                }
            }
        }

        public bool mUpdatePrescriptionState
        {
            get
            {
                return mEditPrescriptionState && IsShowSummaryContent;
            }
        }

        private bool _mPublishNewPrescriptionState;
        public bool mPublishNewPrescriptionState
        {
            get
            {
                return _mPublishNewPrescriptionState;
            }
            set
            {
                if (_mPublishNewPrescriptionState != value)
                {
                    _mPublishNewPrescriptionState = value;
                    NotifyOfPropertyChange(() => mPublishNewPrescriptionState);
                }
            }
        }

        #endregion

        //▼====== #007:
        public string AddMoreZeroAndChangeCommaToDot(string DoseQtyStr)
        {
            if (string.IsNullOrEmpty(DoseQtyStr))
            {
                return "0";
            }
            if (DoseQtyStr.Contains(","))
            {
                DoseQtyStr = DoseQtyStr.Replace(",", ".");
            }
            if (DoseQtyStr.EndsWith("."))
            {
                DoseQtyStr = DoseQtyStr.TrimEnd('.');
                if (string.IsNullOrEmpty(DoseQtyStr))
                {
                    return "0";
                }
            }
            if (DoseQtyStr.StartsWith("."))
            {
                DoseQtyStr = "0" + DoseQtyStr;
            }
            return DoseQtyStr;
        }
        //▲======
    }
}

//public void btnDown()
//{
//    int index = 0;
//    if (SelectedPrescriptionDetail == null)
//    {
//        MessageBox.Show(eHCMSResources.A0338_G1_Msg_InfoChonMotThuoc);
//    }
//    if (SelectedPrescriptionDetail != null && SelectedPrescriptionDetail.SelectedDrugForPrescription != null 
//        && SelectedPrescriptionDetail.SelectedDrugForPrescription.BrandName.Length > 1)
//    {
//        if (SelectedPrescriptionDetail.Index < LatestePrecriptions.PrescriptionDetails.Count - 2)
//        {
//            LatestePrecriptions.PrescriptionDetails[SelectedPrescriptionDetail.Index + 1].Index--;
//            SelectedPrescriptionDetail.Index++;
//            index = SelectedPrescriptionDetail.Index;
//            var temp = ObjectCopier.DeepCopy(LatestePrecriptions.PrescriptionDetails);
//            LatestePrecriptions.PrescriptionDetails = new ObservableCollection<PrescriptionDetail>(
//                temp.OrderBy(item => item.Index));
//            NotifyOfPropertyChange(() => LatestePrecriptions.PrescriptionDetails);
//            SelectedPrescriptionDetail = LatestePrecriptions.PrescriptionDetails[index];
//        }
//    }
//    //LatestePrecriptions.PrescriptionDetails
//}
//public void btnUp()
//{
//    if (SelectedPrescriptionDetail==null)
//    {
//        MessageBox.Show(eHCMSResources.A0338_G1_Msg_InfoChonMotThuoc);
//    }
//    int index = 0;
//    //SelectedPrescriptionDetail = pSelectedItem as PrescriptionDetail;
//    if (SelectedPrescriptionDetail != null && SelectedPrescriptionDetail.SelectedDrugForPrescription != null
//        && SelectedPrescriptionDetail.SelectedDrugForPrescription.BrandName != null
//        && SelectedPrescriptionDetail.SelectedDrugForPrescription.BrandName != "")
//    {
//        if (SelectedPrescriptionDetail.Index > 0)
//        {
//            LatestePrecriptions.PrescriptionDetails[SelectedPrescriptionDetail.Index - 1].Index++;
//            SelectedPrescriptionDetail.Index--;
//            index = SelectedPrescriptionDetail.Index;
//            var temp = ObjectCopier.DeepCopy(LatestePrecriptions.PrescriptionDetails);
//            LatestePrecriptions.PrescriptionDetails = new ObservableCollection<PrescriptionDetail>(
//                temp.OrderBy(item => item.Index));
//            NotifyOfPropertyChange(() => LatestePrecriptions.PrescriptionDetails);
//            SelectedPrescriptionDetail = LatestePrecriptions.PrescriptionDetails[index];
//        }
//    }

//}

//private void SoNgayDungThuocThayDoi(object sender, RoutedEventArgs e)
//{
//    if (LatestePrecriptions.NDay.HasValue && (LatestePrecriptions.NDay.Value != LatestePrecriptions.NDayPreValue))
//    {
//        ChangeDay(LatestePrecriptions.NDay.Value);
//    }            
//}


//public void chk_NotInCat_Checked(object sender, RoutedEventArgs e)
//{

//    if (((CheckBox)sender).IsChecked==true && SelectedPrescriptionDetail!=null)
//    {
//        SelectedPrescriptionDetail = ObjectCopier.DeepCopy(CreateNewDrugForSellVisitor(SelectedPrescriptionDetail));
//    }
//    NotifyOfPropertyChange(() => SelectedPrescriptionDetail.IsDrugNotInCat);
//    NotifyOfPropertyChange(() => SelectedPrescriptionDetail.DrugType); 

//}
//public void chk_NotInCat_Unchecked(object sender, RoutedEventArgs e)
//{         
//    MessageBox.Show("Bạn mới đổi từ thuốc ngoài danh mục qua thuốc trong danh mục."
//        + "\nChọn lại thuốc cho dòng này!");
//    SelectedPrescriptionDetail = ObjectCopier.DeepCopy(CreateNewDrugForSellVisitor(SelectedPrescriptionDetail));
//    NotifyOfPropertyChange(() => SelectedPrescriptionDetail);

//}
//public void chk_NotInCat_Click(object sender, RoutedEventArgs e)
//{
//    if (((CheckBox)sender).IsChecked == false)
//    {
//        SelectedPrescriptionDetail = ObjectCopier.DeepCopy(CreateNewDrugForSellVisitor(SelectedPrescriptionDetail));
//        NotifyOfPropertyChange(() => SelectedPrescriptionDetail);    
//    }            
//}


//public void grdPrescription_CellEditEnded(object sender, DataGridCellEditEndedEventArgs e)
//{
//    PrescriptionDetailPreparing = (e.Row.DataContext as PrescriptionDetail);

//    if (LatestePrecriptions.PrescriptionDetails == null)
//        return;
//    if (PrescriptionDetailPreparing.SelectedDrugForPrescription != null
//                    && PrescriptionDetailPreparing.SelectedDrugForPrescription.MaxDayPrescribed != null
//                    && PrescriptionDetailPreparing.SelectedDrugForPrescription.MaxDayPrescribed > 0)
//    {
//        e.Row.Background = new SolidColorBrush(Color.FromArgb(200, 224, 130, 228));
//        PrescriptionDetailPreparing.BackGroundColor = "#E79DEA";
//        SelectedPrescriptionDetail.BackGroundColor = "#E79DEA";
//        NotifyOfPropertyChange(() => SelectedPrescriptionDetail.BackGroundColor);
//        NotifyOfPropertyChange(() => PrescriptionDetailPreparing.BackGroundColor);
//    }
//    if (e.Column.DisplayIndex == (int)DataGridCol.DRUG_NAME)
//    {
//        if (PrescriptionDetailPreparing.IsDrugNotInCat
//            && AutoGenMedProduct.SearchText != "")
//        {
//            PrescriptionDetailPreparing.SelectedDrugForPrescription.BrandName = AutoGenMedProduct.SearchText;
//            ClearDataRow(PrescriptionDetailPreparing);
//        }
//    }
//    if (ischanged(grdPrescription.SelectedItem))
//    {
//        if (e.Row.GetIndex() == (LatestePrecriptions.PrescriptionDetails.Count - 1) && e.EditAction == DataGridEditAction.Commit)
//        {
//            //if (((PrescriptionDetail)this.grdPrescription.SelectedItem).V_DrugType == 
//            //    (long)AllLookupValues.V_DrugType.THUOC_NGOAIDANH_MUC)
//            //{
//            //    ((PrescriptionDetail)this.grdPrescription.SelectedItem).IsDrugNotInCat = true;
//            //}
//            AddNewBlankDrugIntoPrescriptObjectNew();
//        }

//        if (e.Column.DisplayIndex == (int)DataGridCol.DRUG_NAME)
//        {
//            if (((PrescriptionDetail)this.grdPrescription.SelectedItem).IsDrugNotInCat == true)
//            {
//                if (Check1ThuocBiDiUng(((PrescriptionDetail)this.grdPrescription.SelectedItem).BrandName))
//                {
//                    MessageBox.Show(string.Format("'{0}' {1}", ((PrescriptionDetail)this.grdPrescription.SelectedItem).BrandName.Trim(), eHCMSResources.K0001_G1_DiUngVoiBN));
//                    return;
//                }
//            }
//            else
//            {
//                if (Check1ThuocBiDiUng(((PrescriptionDetail)this.grdPrescription.SelectedItem).SelectedDrugForPrescription.BrandName))
//                {
//                    MessageBox.Show(string.Format("'{0}' {1}!", ((PrescriptionDetail)this.grdPrescription.SelectedItem).SelectedDrugForPrescription.BrandName.Trim(), eHCMSResources.K0001_G1_DiUngVoiBN));
//                    return;
//                }
//            }


//            if (((PrescriptionDetail)this.grdPrescription.SelectedItem).DrugID != null && (long)((PrescriptionDetail)this.grdPrescription.SelectedItem).DrugID > 0)
//            {
//                Globals.CheckContrain(PtMedCond, (long)((PrescriptionDetail)this.grdPrescription.SelectedItem).DrugID);
//            }
//        }
//        else if (e.Column.DisplayIndex == (int)DataGridCol.QTY)
//        {
//            if (((PrescriptionDetail)this.grdPrescription.SelectedItem).IsDrugNotInCat == false)
//            {
//                if (((PrescriptionDetail)this.grdPrescription.SelectedItem).SelectedDrugForPrescription.Remaining < ((PrescriptionDetail)this.grdPrescription.SelectedItem).Qty)
//                {
//                    MessageBox.Show(eHCMSResources.A0977_G1_Msg_InfoSLgKhDuBan);
//                }
//            }
//        }

//        else if (e.Column.DisplayIndex == (int)DataGridCol.MDOSE)
//        {
//            ChangeMDose(PrescriptionDetailPreparing != null ? PrescriptionDetailPreparing.MDoseStr : "0", this.grdPrescription.SelectedItem);
//        }

//        else if (e.Column.DisplayIndex == (int)DataGridCol.EDOSE)
//        {
//            ChangeEDose(PrescriptionDetailPreparing != null ? PrescriptionDetailPreparing.EDoseStr : "0", this.grdPrescription.SelectedItem);
//        }
//        else if (e.Column.DisplayIndex == (int)DataGridCol.ADOSE)
//        {
//            ChangeADose(PrescriptionDetailPreparing != null ? PrescriptionDetailPreparing.ADoseStr : "0", this.grdPrescription.SelectedItem);
//        }

//        else if (e.Column.DisplayIndex == (int)DataGridCol.NDOSE)
//        {
//            ChangeNDose(PrescriptionDetailPreparing != null ? PrescriptionDetailPreparing.NDoseStr : "0", this.grdPrescription.SelectedItem);
//        }

//        //else if (e.Column.DisplayIndex == (int)DataGridCol.DOSAGE)
//        //{
//        //    ChangeDosage(PrescriptionDetailPreparing != null ? PrescriptionDetailPreparing.dosageStr : "0", this.grdPrescription.SelectedItem);
//        //}

//        else if (e.Column.DisplayIndex == (int)DataGridCol.Dayts)
//        {
//            ChangeNgayDung((PrescriptionDetail)this.grdPrescription.SelectedItem);
//        }
//        else if (e.Column.DisplayIndex == (int)DataGridCol.DaytExtended)
//        {
//            //ChangeNgayDungExtend(PrescriptionDetailPreparing != null ? PrescriptionDetailPreparing.DayExtended : 0, this.grdPrescription.SelectedItem);
//        }
//        else if (e.Column.DisplayIndex == (int)DataGridCol.USAGE)
//        {
//            if (PrescriptionDetailPreparing.SelectedDrugForPrescription != null
//                && PrescriptionDetailPreparing.SelectedDrugForPrescription.Administration != null)
//            {
//                PrescriptionDetailPreparing.Administration = PrescriptionDetailPreparing.SelectedDrugForPrescription.Administration;
//            }
//        }
//        else if (e.Column.DisplayIndex == (int)DataGridCol.NotInCat)
//        {

//        }
//    }
//}


//KMx: Bỏ hàm này vì kết hợp 2 hàm GetPrescriptionTypes_New() và GetPrescriptionTypes_DaCo() thành GetPrescription() (17/05/2014 11:15).
//Lý do: Bỏ bước đi về server lấy Lookup và 2 hàm quá giống nhau.
//private void GetPrescriptionTypes_New()
//{
//    //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
//    IsWaitingGetPrescriptionTypes = true;
//    this.ShowBusyIndicator();
//    var t = new Thread(() =>
//    {
//        using (var serviceFactory = new ePrescriptionsServiceClient())
//        {
//            var contract = serviceFactory.ServiceInstance;
//            contract.BeginGetLookupPrescriptionType(Globals.DispatchCallback((asyncResult) =>
//            {

//                try
//                {
//                    var results = contract.EndGetLookupPrescriptionType(asyncResult);

//                    if (results != null)
//                    {
//                        PrescriptionTypeList.Clear();

//                        PrescriptionTypeList = new ObservableCollection<Lookup>(results);

//                        if (PrescriptionTypeList.Count > 0)
//                        {
//                            CurrentPrescriptionType = PrescriptionTypeList[0];
//                        }
//                        InitialNewPrescription();
//                        SetToaBaoHiem_KhongBaoHiem();
//                        GetLatestPrescriptionByPtID_New(Registration_DataStorage.CurrentPatient.PatientID);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show(ex.Message);
//                }
//                finally
//                {
//                    IsWaitingGetPrescriptionTypes = false;
//                    this.HideBusyIndicator();
//                }

//            }), null);

//        }

//    });

//    t.Start();
//}

//KMx: Bỏ hàm này vì kết hợp 2 hàm GetPrescriptionTypes_New() và GetPrescriptionTypes_DaCo() thành GetPrescription() (17/05/2014 11:15).
//Lý do: Bỏ bước đi về server lấy Lookup và 2 hàm quá giống nhau.

//private void GetPrescriptionTypes_DaCo()
//{
//    //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
//    IsWaitingGetPrescriptionTypes = true;
//    var t = new Thread(() =>
//    {
//        using (var serviceFactory = new ePrescriptionsServiceClient())
//        {
//            var contract = serviceFactory.ServiceInstance;
//            contract.BeginGetLookupPrescriptionType(Globals.DispatchCallback((asyncResult) =>
//            {

//                try
//                {
//                    var results = contract.EndGetLookupPrescriptionType(asyncResult);
//                    if (results != null)
//                    {
//                        PrescriptionTypeList.Clear();

//                        PrescriptionTypeList = new ObservableCollection<Lookup>(results);

//                        if (PrescriptionTypeList.Count > 0)
//                        {
//                            CurrentPrescriptionType = PrescriptionTypeList[0];
//                        }
//                        InitialNewPrescription();
//                        SetToaBaoHiem_KhongBaoHiem();
//                        GetToaThuocDaCo();
//                    }
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show(ex.Message);
//                }
//                finally
//                {
//                    IsWaitingGetPrescriptionTypes = false;
//                }

//            }), null);

//        }

//    });

//    t.Start();
//}

//public void tbSoTuan_Loaded(object sender, RoutedEventArgs e)
//{
//    CtrtbSoTuan = sender as AxTextBox;
//}