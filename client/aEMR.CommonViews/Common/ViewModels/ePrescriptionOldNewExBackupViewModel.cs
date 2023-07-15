//using System.Linq;
//using System.ServiceModel;
//using System.Windows;
//using System.Windows.Controls;
//using System.Collections.ObjectModel;
//using DataEntities;
//using System.Threading;
//using System;
//using System.Collections.Generic;
//using eHCMSLanguage;
//using aEMR.ServiceClient;
//using aEMR.Infrastructure;
//using aEMR.Common.Collections;
//using aEMR.DataContracts;
///*
// * 20180922 #001 TTM:   Chuyển lấy Lookup từ gọi về Service sang lấy từ cache trên client. Vì đã có lấy tất cả Lookup lúc đăng nhập rồi không cần phải
// *                      gọi về Service tốn thời gian.
// */
//namespace aEMR.Common.ViewModels
//{
    
//    public partial class ePrescriptionOldNewBackupViewModel 
//    {
//        private ObservableCollection<Lookup> _DrugTypes;
//        public ObservableCollection<Lookup> DrugTypes
//        {
//            get
//            {
//                return _DrugTypes;
//            }
//            set
//            {
//                if (_DrugTypes != value)
//                {
//                    _DrugTypes = value;
//                    NotifyOfPropertyChange(() => DrugTypes);
//                }
//            }
//        }


//        private ObservableCollection<PrescriptionDetailSchedulesLieuDung> _LieuDung;
//        public ObservableCollection<PrescriptionDetailSchedulesLieuDung> LieuDung
//        {
//            get
//            {
//                return _LieuDung;
//            }
//            set
//            {
//                if (_LieuDung != value)
//                {
//                    _LieuDung = value;
//                    NotifyOfPropertyChange(() => _LieuDung);
//                }
//            }
//        }

//        private void GetAllLookupForDrugTypes()
//        {
//            var t = new Thread(() =>
//            {

//                using (var serviceFactory = new CommonService_V2Client())
//                {
//                    var contract = serviceFactory.ServiceInstance;
//                    contract.BeginGetAllLookupValuesByType(LookupValues.V_DrugType, Globals.DispatchCallback((asyncResult) =>
//                    {
//                        try
//                        {
//                            var results = contract.EndGetAllLookupValuesByType(asyncResult);
//                            DrugTypes = results.ToObservableCollection();
//                            NotifyOfPropertyChange(() => DrugTypes);
//                        }
//                        catch (Exception ex)
//                        {
//                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//                        }
//                        finally
//                        {
//                            //Globals.IsBusy = false;
//                        }

//                    }), null);

//                }

//            });

//            t.Start();
//        }

//        public object GetDrugType(object value)
//        {
//            PrescriptionDetail p = value as PrescriptionDetail;
//            if (p != null)
//            {
//                return p.V_DrugType;
//            }
//            else
//            {
//                return null;
//            }
//        }

//        public void cbxDrugType_Loaded(object sender, RoutedEventArgs e)
//        {
//            var comboBox = sender as ComboBox;
//            if (comboBox != null)
//            {
//                comboBox.ItemsSource = DrugTypes;
//                if (SelectedPrescriptionDetail != null)
//                {
//                    comboBox.SelectedItem = GetDrugType(SelectedPrescriptionDetail);
//                }
//            }
//        }
//        public void cbxDrugType_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            NotifyOfPropertyChange(() => SelectedPrescriptionDetail);
//            if (((Lookup)((ComboBox)sender).SelectedItem).LookupID == (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN)
//            {
//                SelectedPrescriptionDetail.DrugType = (Lookup)((ComboBox)sender).SelectedItem;
//                SelectedPrescriptionDetail.isForm = false;
//                hplEditSchedules_Click(SelectedPrescriptionDetail);
//            }
//        }


//        public void cbxDrugTypeForm_Loaded(object sender, RoutedEventArgs e)
//        {
//            var comboBox = sender as ComboBox;
//            if (comboBox != null)
//            {
//                //comboBox.ItemsSource = DrugTypes;
//                //if (SelectedPrescriptionDetail != null)
//                //{
//                //    comboBox.SelectedItem = GetDrugType(tempPrescriptionDetail);
//                //}
//            }
//        }
//        public void cbxDrugTypeForm_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            NotifyOfPropertyChange(() => tempPrescriptionDetail);
//            if (((Lookup)((ComboBox)sender).SelectedItem).LookupID == (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN)
//            {
//                tempPrescriptionDetail.DrugType = (Lookup)((ComboBox)sender).SelectedItem;
//                tempPrescriptionDetail.isForm = true;
//                hplEditSchedules_Click(tempPrescriptionDetail);
//            }
//        }

//        public void hplDown_Click(Object pSelectedItem)
//        {
//            SelectedPrescriptionDetail = pSelectedItem as PrescriptionDetail;
//            if (SelectedPrescriptionDetail != null && SelectedPrescriptionDetail.SelectedDrugForPrescription != null
//                && SelectedPrescriptionDetail.SelectedDrugForPrescription.BrandName != null
//                && SelectedPrescriptionDetail.SelectedDrugForPrescription.BrandName != "")
//            {
//                if (SelectedPrescriptionDetail.Index < LatestePrecriptions.PrescriptionDetails.Count-2)
//                {
//                    LatestePrecriptions.PrescriptionDetails[SelectedPrescriptionDetail.Index + 1].Index--;
//                    SelectedPrescriptionDetail.Index++;
//                    var temp = ObjectCopier.DeepCopy(LatestePrecriptions.PrescriptionDetails);
//                    LatestePrecriptions.PrescriptionDetails = new ObservableCollection<PrescriptionDetail>(
//                        temp.OrderBy(item => item.Index));
//                    NotifyOfPropertyChange(() => LatestePrecriptions.PrescriptionDetails);
//                }
//            }
//            //LatestePrecriptions.PrescriptionDetails
//        }
//        public void hplUp_Click(Object pSelectedItem)
//        {
//            SelectedPrescriptionDetail = pSelectedItem as PrescriptionDetail;
//            if (SelectedPrescriptionDetail!=null && SelectedPrescriptionDetail.SelectedDrugForPrescription!=null
//                && SelectedPrescriptionDetail.SelectedDrugForPrescription.BrandName != null 
//                && SelectedPrescriptionDetail.SelectedDrugForPrescription.BrandName != "")
//            {
//                if (SelectedPrescriptionDetail.Index >0)
//                {
//                    LatestePrecriptions.PrescriptionDetails[SelectedPrescriptionDetail.Index-1].Index++;
//                    SelectedPrescriptionDetail.Index--;
//                    var temp =ObjectCopier.DeepCopy( LatestePrecriptions.PrescriptionDetails);
//                    LatestePrecriptions.PrescriptionDetails = new ObservableCollection<PrescriptionDetail>(
//                        temp.OrderBy(item => item.Index));
//                    NotifyOfPropertyChange(() => LatestePrecriptions.PrescriptionDetails);
//                }
//            }

//        }

//        public void btnDown()
//        {
//            int index = 0;
//            if (SelectedPrescriptionDetail == null)
//            {
//                MessageBox.Show(eHCMSResources.A0338_G1_Msg_InfoChonMotThuoc);
//            }
//            if (SelectedPrescriptionDetail != null && SelectedPrescriptionDetail.SelectedDrugForPrescription != null
//                && SelectedPrescriptionDetail.SelectedDrugForPrescription.BrandName != null
//                && SelectedPrescriptionDetail.SelectedDrugForPrescription.BrandName != "")
//            {
//                if (SelectedPrescriptionDetail.Index < LatestePrecriptions.PrescriptionDetails.Count - 2)
//                {
//                    LatestePrecriptions.PrescriptionDetails[SelectedPrescriptionDetail.Index + 1].Index--;
//                    SelectedPrescriptionDetail.Index++;
//                    index = SelectedPrescriptionDetail.Index;
//                    var temp = ObjectCopier.DeepCopy(LatestePrecriptions.PrescriptionDetails);
//                    LatestePrecriptions.PrescriptionDetails = new ObservableCollection<PrescriptionDetail>(
//                        temp.OrderBy(item => item.Index));
//                    NotifyOfPropertyChange(() => LatestePrecriptions.PrescriptionDetails);
//                    SelectedPrescriptionDetail = LatestePrecriptions.PrescriptionDetails[index];
//                }
//            }
//            //LatestePrecriptions.PrescriptionDetails
//        }
//        public void btnUp()
//        {
//            if (SelectedPrescriptionDetail==null)
//            {
//                MessageBox.Show(eHCMSResources.A0338_G1_Msg_InfoChonMotThuoc);
//            }
//            int index = 0;
//            //SelectedPrescriptionDetail = pSelectedItem as PrescriptionDetail;
//            if (SelectedPrescriptionDetail != null && SelectedPrescriptionDetail.SelectedDrugForPrescription != null
//                && SelectedPrescriptionDetail.SelectedDrugForPrescription.BrandName != null
//                && SelectedPrescriptionDetail.SelectedDrugForPrescription.BrandName != "")
//            {
//                if (SelectedPrescriptionDetail.Index > 0)
//                {
//                    LatestePrecriptions.PrescriptionDetails[SelectedPrescriptionDetail.Index - 1].Index++;
//                    SelectedPrescriptionDetail.Index--;
//                    index = SelectedPrescriptionDetail.Index;
//                    var temp = ObjectCopier.DeepCopy(LatestePrecriptions.PrescriptionDetails);
//                    LatestePrecriptions.PrescriptionDetails = new ObservableCollection<PrescriptionDetail>(
//                        temp.OrderBy(item => item.Index));
//                    NotifyOfPropertyChange(() => LatestePrecriptions.PrescriptionDetails);
//                    SelectedPrescriptionDetail = LatestePrecriptions.PrescriptionDetails[index];
//                }
//            }

//        }

//        public void chk_NotInCat_Checked(object sender, RoutedEventArgs e)
//        {
//            if (((CheckBox)sender).IsChecked==true
//                && SelectedPrescriptionDetail!=null)
//            {
//                SelectedPrescriptionDetail = ObjectCopier.DeepCopy(CreateNewDrugForSellVisitor(SelectedPrescriptionDetail));
//            }
//            NotifyOfPropertyChange(() => SelectedPrescriptionDetail.IsDrugNotInCat);
//            NotifyOfPropertyChange(() => SelectedPrescriptionDetail.DrugType);           
//        }

//        public void chk_NotInCat_Unchecked(object sender, RoutedEventArgs e)
//        {
            
//        }

//        public void chk_NotInCatForm_Checked(object sender, RoutedEventArgs e)
//        {
//            if (((CheckBox)sender).IsChecked == true
//                && tempPrescriptionDetail != null)
//            {
                
//            }
//            //tempPrescriptionDetail = ObjectCopier.DeepCopy(CreateNewDrugForSellVisitor(tempPrescriptionDetail));
//            tempPrescriptionDetail.SelectedDrugForPrescription = new GetDrugForSellVisitor();
//            tempPrescriptionDetail = ObjectCopier.DeepCopy(RefreshDayRptsPrescriptObject(tempPrescriptionDetail));
//            NotifyOfPropertyChange(() => tempPrescriptionDetail.IsDrugNotInCat);
//            NotifyOfPropertyChange(() => tempPrescriptionDetail.DrugType);
//            ClearForm();
//        }

//        public void chk_NotInCatForm_Click(object sender, RoutedEventArgs e)
//        {
//            if (((CheckBox)sender).IsChecked == true
//                && tempPrescriptionDetail != null)
//            {
//                tempPrescriptionDetail.SelectedDrugForPrescription=new GetDrugForSellVisitor();
//                tempPrescriptionDetail = ObjectCopier.DeepCopy( RefreshPrescriptObject(tempPrescriptionDetail));
//            }
//            ClearForm();
//        }

//        public void chk_HIForm_Click(object sender, RoutedEventArgs e)
//        {
//            if (((CheckBox)sender).IsChecked == true
//                && tempPrescriptionDetail != null)
//            {
                
//            }
//            tempPrescriptionDetail = ObjectCopier.DeepCopy( CreateNewDrugForSellVisitor(tempPrescriptionDetail));
//            ClearForm();
//            NotifyOfPropertyChange(() => tempPrescriptionDetail);  
//        }

//        public void chk_HI_Click(object sender, RoutedEventArgs e)
//        {
//            if (((CheckBox)sender).IsChecked == true
//                && tempPrescriptionDetail != null)
//            {

//            }
//            else 
//            {
//                MessageBox.Show(eHCMSResources.A0188_G1_Msg_InfoVuaDoiThuocThanhTrongDM);
//            }
//            SelectedPrescriptionDetail = ObjectCopier.DeepCopy(CreateNewDrugForSellVisitor(SelectedPrescriptionDetail));   
//            NotifyOfPropertyChange(() => SelectedPrescriptionDetail);
//        }

//        public void checkHIDays(PrescriptionDetail prescriptDObj) 
//        {
//            if (LatestePrecriptions.NDay > xNgayBHToiDa_NgoaiTru)
//            {
//                prescriptDObj.DayRpts = xNgayBHToiDa_NgoaiTru;
//                prescriptDObj.DayExtended = LatestePrecriptions.NDay.GetValueOrDefault() - xNgayBHToiDa_NgoaiTru;
//            }
//            else
//            {
//                prescriptDObj.DayRpts = LatestePrecriptions.NDay.GetValueOrDefault();
//            }
//        }

//        public void CheckDayRpts(PrescriptionDetail objPrescriptionDetail) 
//        {
//            //if (objPrescriptionDetail.BeOfHIMedicineList.Value)
//            {
//                if (IsBenhNhanNoiTru() == false)
//                {
//                    GetDayRpt(objPrescriptionDetail, xNgayBHToiDa_NgoaiTru);
//                }
//                else
//                {
//                    GetDayRpt(objPrescriptionDetail, xNgayBHToiDa_NoiTru);
//                }
//            }
//        } 

//        public void hplEditSchedulesForm_Click()
//        {
//            if (tempPrescriptionDetail==null)
//            {
//                return;
//            }
//            if (!tempPrescriptionDetail.HasSchedules)
//            {
//                MessageBox.Show(eHCMSResources.A0086_G1_Msg_InfoUongThuocTheoLich, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                return;
//            }

//            if (tempPrescriptionDetail == null)
//            {
//                MessageBox.Show(eHCMSResources.A0365_G1_Msg_InfoChonThuocDeRaToa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                return;
//            }

//            hplEditSchedules(tempPrescriptionDetail);
//        }

//        #region PrescriptionNoteTemplates

//        private PrescriptionNoteTemplates _ObjPrescriptionNoteTemplates_Selected;
//        public PrescriptionNoteTemplates ObjPrescriptionNoteTemplates_Selected
//        {
//            get { return _ObjPrescriptionNoteTemplates_Selected; }
//            set
//            {
//                _ObjPrescriptionNoteTemplates_Selected = value;
//                NotifyOfPropertyChange(() => ObjPrescriptionNoteTemplates_Selected);

//                if (_ObjPrescriptionNoteTemplates_Selected != null && _ObjPrescriptionNoteTemplates_Selected.PrescriptNoteTemplateID > 0)
//                {
//                    string str = LatestePrecriptions.DoctorAdvice;
//                    if (string.IsNullOrEmpty(str))
//                    {
//                        str = ObjPrescriptionNoteTemplates_Selected.DetailsTemplate;
//                    }
//                    else
//                    {
//                        str = str + Environment.NewLine + ObjPrescriptionNoteTemplates_Selected.DetailsTemplate;
//                    }

//                    LatestePrecriptions.DoctorAdvice = str;
//                }
//            }
//        }

//        private ObservableCollection<PrescriptionNoteTemplates> _ObjPrescriptionNoteTemplates_GetAll;
//        public ObservableCollection<PrescriptionNoteTemplates> ObjPrescriptionNoteTemplates_GetAll
//        {
//            get { return _ObjPrescriptionNoteTemplates_GetAll; }
//            set
//            {
//                _ObjPrescriptionNoteTemplates_GetAll = value;
//                NotifyOfPropertyChange(() => ObjPrescriptionNoteTemplates_GetAll);
//            }
//        }

//        private ObservableCollection<PrescriptionNoteTemplates> _DonViTinh;
//        public ObservableCollection<PrescriptionNoteTemplates> DonViTinh
//        {
//            get { return _DonViTinh; }
//            set
//            {
//                _DonViTinh = value;
//                NotifyOfPropertyChange(() => DonViTinh);
//            }
//        }

//        private ObservableCollection<PrescriptionNoteTemplates> _GhiChu;
//        public ObservableCollection<PrescriptionNoteTemplates> GhiChu
//        {
//            get { return _GhiChu; }
//            set
//            {
//                _GhiChu = value;
//                NotifyOfPropertyChange(() => GhiChu);
//            }
//        }

//        private ObservableCollection<PrescriptionNoteTemplates> _CachDung;
//        public ObservableCollection<PrescriptionNoteTemplates> CachDung
//        {
//            get { return _CachDung; }
//            set
//            {
//                _CachDung = value;
//                NotifyOfPropertyChange(() => CachDung);
//            }
//        }

//        public void PrescriptionNoteTemplates_GetAllIsActive()
//        {
//            var t = new Thread(() =>
//            {
//                IsWaitingPrescriptionNoteTemplates_GetAll = true;

//                try
//                {
//                    using (var serviceFactory = new ePrescriptionsServiceClient())
//                    {
//                        var contract = serviceFactory.ServiceInstance;
//                        var pnt = new PrescriptionNoteTemplates();
//                        pnt.V_PrescriptionNoteTempType = AllLookupValues.V_PrescriptionNoteTempType.PrescriptionNoteGen;
//                        contract.BeginPrescriptionNoteTemplates_GetAllIsActive(pnt, Globals.DispatchCallback((asyncResult) =>
//                        {
//                            IList<PrescriptionNoteTemplates> allItems = new ObservableCollection<PrescriptionNoteTemplates>();
//                            try
//                            {
//                                allItems = contract.EndPrescriptionNoteTemplates_GetAllIsActive(asyncResult);

//                                ObjPrescriptionNoteTemplates_GetAll = new ObservableCollection<PrescriptionNoteTemplates>(allItems);

//                                PrescriptionNoteTemplates firstItem = new PrescriptionNoteTemplates();
//                                firstItem.PrescriptNoteTemplateID = -1;
//                                firstItem.NoteDetails = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K0616_G1_ChonMau);
//                                ObjPrescriptionNoteTemplates_GetAll.Insert(0, firstItem);

//                            }
//                            catch (FaultException<AxException> fault)
//                            {
//                                ClientLoggerHelper.LogInfo(fault.ToString());
//                            }
//                            catch (Exception ex)
//                            {
//                                ClientLoggerHelper.LogInfo(ex.ToString());
//                            }

//                        }), null);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//                }
//                finally
//                {
//                    IsWaitingPrescriptionNoteTemplates_GetAll = false;
//                }
//            });
//            t.Start();
//        }
//        public void PrescriptionNoteTemplates_GetAllIsActiveItem()
//        {
//            var t = new Thread(() =>
//            {
//                IsWaitingPrescriptionNoteTemplates_GetAll = true;

//                try
//                {
//                    using (var serviceFactory = new ePrescriptionsServiceClient())
//                    {
//                        var contract = serviceFactory.ServiceInstance;
//                        var pnt = new PrescriptionNoteTemplates();
//                        pnt.V_PrescriptionNoteTempType = AllLookupValues.V_PrescriptionNoteTempType.PrescriptionNoteItem;
//                        contract.BeginPrescriptionNoteTemplates_GetAllIsActive(pnt, Globals.DispatchCallback((asyncResult) =>
//                        {
//                            IList<PrescriptionNoteTemplates> allItems = new ObservableCollection<PrescriptionNoteTemplates>();
//                            try
//                            {
//                                allItems = contract.EndPrescriptionNoteTemplates_GetAllIsActive(asyncResult);
//                                GhiChu = new ObservableCollection<PrescriptionNoteTemplates>(allItems.OrderBy(x => x.NoteDetails));
//                                NotifyOfPropertyChange(() => GhiChu);
//                            }
//                            catch (FaultException<AxException> fault)
//                            {
//                                ClientLoggerHelper.LogInfo(fault.ToString());
//                            }
//                            catch (Exception ex)
//                            {
//                                ClientLoggerHelper.LogInfo(ex.ToString());
//                            }

//                        }), null);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//                }
//                finally
//                {
//                    IsWaitingPrescriptionNoteTemplates_GetAll = false;
//                }
//            });
//            t.Start();
//        }

//        public void PrescriptionNoteTemplates_GetAllIsActiveAdm()
//        {
//            var t = new Thread(() =>
//            {
//                IsWaitingPrescriptionNoteTemplates_GetAll = true;

//                try
//                {
//                    using (var serviceFactory = new ePrescriptionsServiceClient())
//                    {
//                        var contract = serviceFactory.ServiceInstance;
//                        var pnt = new PrescriptionNoteTemplates();
//                        pnt.V_PrescriptionNoteTempType = AllLookupValues.V_PrescriptionNoteTempType.PrescriptionAdministration;
//                        contract.BeginPrescriptionNoteTemplates_GetAllIsActive(pnt, Globals.DispatchCallback((asyncResult) =>
//                        {
//                            IList<PrescriptionNoteTemplates> allItems = new ObservableCollection<PrescriptionNoteTemplates>();
//                            try
//                            {
//                                allItems = contract.EndPrescriptionNoteTemplates_GetAllIsActive(asyncResult);
//                                CachDung = new ObservableCollection<PrescriptionNoteTemplates>(allItems.OrderBy(x => x.NoteDetails));
//                                NotifyOfPropertyChange(() => CachDung);
//                            }
//                            catch (FaultException<AxException> fault)
//                            {
//                                ClientLoggerHelper.LogInfo(fault.ToString());
//                            }
//                            catch (Exception ex)
//                            {
//                                ClientLoggerHelper.LogInfo(ex.ToString());
//                            }

//                        }), null);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//                }
//                finally
//                {
//                    IsWaitingPrescriptionNoteTemplates_GetAll = false;
//                }
//            });
//            t.Start();
//        }

//        //public void PrescriptionNoteTemplates_GetAllIsActiveUnitUse()
//        //{
//        //    var t = new Thread(() =>
//        //    {
//        //        IsWaitingPrescriptionNoteTemplates_GetAll = true;

//        //        try
//        //        {
//        //            using (var serviceFactory = new ePrescriptionsServiceClient())
//        //            {
//        //                var contract = serviceFactory.ServiceInstance;
//        //                var pnt = new PrescriptionNoteTemplates();
//        //                pnt.V_PrescriptionNoteTempType = AllLookupValues.V_PrescriptionNoteTempType.PrescriptionUnitUse;
//        //                contract.BeginPrescriptionNoteTemplates_GetAllIsActive(pnt, Globals.DispatchCallback((asyncResult) =>
//        //                {
//        //                    IList<PrescriptionNoteTemplates> allItems = new ObservableCollection<PrescriptionNoteTemplates>();
//        //                    try
//        //                    {
//        //                        allItems = contract.EndPrescriptionNoteTemplates_GetAllIsActive(asyncResult);
//        //                        DonViTinh = new ObservableCollection<PrescriptionNoteTemplates>(allItems.OrderBy(x => x.NoteDetails));
//        //                        NotifyOfPropertyChange(() => DonViTinh);
//        //                    }
//        //                    catch (FaultException<AxException> fault)
//        //                    {

//        //                    }
//        //                    catch (Exception ex)
//        //                    {
//        //                    }

//        //                }), null);
//        //            }
//        //        }
//        //        catch (Exception ex)
//        //        {
//        //            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//        //        }
//        //        finally
//        //        {
//        //            IsWaitingPrescriptionNoteTemplates_GetAll = false;
//        //        }
//        //    });
//        //    t.Start();
//        //}

//        public void chk_NotInCat_LostFocus(object sender, RoutedEventArgs e)
//        {

//        }

//        public void chk_NotInCat_GotFocus(object sender, RoutedEventArgs e)
//        {

//        }

//        public void UnitName_LostFocus(object sender, RoutedEventArgs e)
//        {

//        }

//        public void UnitName_GotFocus(object sender, RoutedEventArgs e)
//        {

//        }
//        #endregion

//        public void RefreshLookup()
//        {
//            PrescriptionNoteTemplates_GetAllIsActive();
//            PrescriptionNoteTemplates_GetAllIsActiveItem();
//            PrescriptionNoteTemplates_GetAllIsActiveAdm();
//            //PrescriptionNoteTemplates_GetAllIsActiveUnitUse();
//        }

//        public void GetToaThuocDaCo() 
//        {
//            btnCreateNewIsEnabled = true;
//            btnSaveAddNewIsEnabled = false;
            
//            btnCreateAndCopyIsEnabled = true;
//            btnCopyToIsEnabled = true;
//            IsEnabledPrint = true;

//            LatestePrecriptions = Globals.curLstPatientServiceRecord[0].PrescriptionIssueHistories[0].Prescription;
//            if (CheckToaThuocDuocPhepCapNhat(DuocSi_IsEditingToaThuoc, LatestePrecriptions) == false)
//            {
//                btnEditIsEnabled = false;
//            }
//            else
//            {
//                btnEditIsEnabled = true;
//            }
//            //LatestePrecriptions.ObjDiagnosisTreatment = Globals.curLstPatientServiceRecord[0].DiagnosisTreatments[0];

//            PrecriptionsForPrint = LatestePrecriptions;

//            ContentKhungTaiKhamIsEnabled = LatestePrecriptions.IsAllowEditNDay;

//            SoNgayDungThuoc_Root = LatestePrecriptions.NDay;

//            if (LatestePrecriptions.NDay > 0)
//            {
//                chkHasAppointmentValue = true;
//            }
//            else
//            {
//                chkHasAppointmentValue = false;
//            }
//            LatestePrecriptions.CanEdit = true;
//            GetPrescriptionDetailsByPrescriptID(LatestePrecriptions.PrescriptID);
//        }

//        private void GetPrescriptionTypes_New()
//        {

//            //▼====== #001
//            PrescriptionTypeList = new ObservableCollection<Lookup>();
//            foreach (var tmpLookup in Globals.AllLookupValueList)
//            {
//                if (tmpLookup.ObjectTypeID == (long)(LookupValues.PRESCRIPTION_TYPE))
//                {
//                    PrescriptionTypeList.Add(tmpLookup);
//                }
//            }
//            if (PrescriptionTypeList.Count > 0)
//            {
//                CurrentPrescriptionType = PrescriptionTypeList[0];
//            }
//            InitialNewPrescription();
//            SetToaBaoHiem_KhongBaoHiem();
//            GetLatestPrescriptionByPtID_New(Globals.PatientAllDetails.PatientInfo.PatientID);
//            //▲====== #001
//            ////Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
//            //IsWaitingGetPrescriptionTypes = true;

//            //var t = new Thread(() =>
//            //{
//            //    using (var serviceFactory = new ePrescriptionsServiceClient())
//            //    {
//            //        var contract = serviceFactory.ServiceInstance;
//            //        contract.BeginGetLookupPrescriptionType(Globals.DispatchCallback((asyncResult) =>
//            //        {

//            //            try
//            //            {
//            //                var results = contract.EndGetLookupPrescriptionType(asyncResult);
//            //                if (results != null)
//            //                {
//            //                    PrescriptionTypeList.Clear();

//            //                    PrescriptionTypeList = new ObservableCollection<Lookup>(results);

//            //                    if (PrescriptionTypeList.Count > 0)
//            //                    {
//            //                        CurrentPrescriptionType = PrescriptionTypeList[0];
//            //                    }
//            //                    InitialNewPrescription();
//            //                    SetToaBaoHiem_KhongBaoHiem();
//            //                    GetLatestPrescriptionByPtID_New(Globals.PatientAllDetails.PatientInfo.PatientID);
//            //                }
//            //            }
//            //            catch (Exception ex)
//            //            {
//            //                MessageBox.Show(ex.Message);
//            //            }
//            //            finally
//            //            {
//            //                IsWaitingGetPrescriptionTypes = false;
//            //            }

//            //        }), null);

//            //    }

//            //});

//            //t.Start();
//        }

//        private void GetPrescriptionTypes_DaCo()
//        {
//            //▼====== #001
//            PrescriptionTypeList = new ObservableCollection<Lookup>();
//            foreach (var tmpLookup in Globals.AllLookupValueList)
//            {
//                if (tmpLookup.ObjectTypeID == (long)(LookupValues.PRESCRIPTION_TYPE))
//                {
//                    PrescriptionTypeList.Add(tmpLookup);
//                }
//            }
//            if (PrescriptionTypeList.Count > 0)
//            {
//                CurrentPrescriptionType = PrescriptionTypeList[0];
//            }
//            InitialNewPrescription();
//            SetToaBaoHiem_KhongBaoHiem();
//            GetToaThuocDaCo();
//            //▲====== #001
//            ////Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
//            //IsWaitingGetPrescriptionTypes = true;            
//            //var t = new Thread(() =>
//            //{
//            //    using (var serviceFactory = new ePrescriptionsServiceClient())
//            //    {
//            //        var contract = serviceFactory.ServiceInstance;
//            //        contract.BeginGetLookupPrescriptionType(Globals.DispatchCallback((asyncResult) =>
//            //        {

//            //            try
//            //            {
//            //                var results = contract.EndGetLookupPrescriptionType(asyncResult);
//            //                if (results != null)
//            //                {
//            //                    PrescriptionTypeList.Clear();

//            //                    PrescriptionTypeList = new ObservableCollection<Lookup>(results);

//            //                    if (PrescriptionTypeList.Count > 0)
//            //                    {
//            //                        CurrentPrescriptionType = PrescriptionTypeList[0];
//            //                    }
//            //                    InitialNewPrescription();
//            //                    SetToaBaoHiem_KhongBaoHiem();
//            //                    GetToaThuocDaCo();
//            //                }
//            //            }
//            //            catch (Exception ex)
//            //            {
//            //                MessageBox.Show(ex.Message);
//            //            }
//            //            finally
//            //            {
//            //                IsWaitingGetPrescriptionTypes = false;
//            //            }

//            //        }), null);

//            //    }

//            //});

//            //t.Start();
//        }

//        #region button control

//        public enum PrescriptionState
//        {
//            //Tao moi toa thuoc
//            NewPrescriptionState = 1,
//            //Hieu chinh toa thuoc voi bac si da ra toa
//            EditPrescriptionState = 2,
//            //Phat hanh lai toa thuoc neu la voi bac si khac
//            //cho chinh sua va mark delete doi voi toa cu
//            PublishNewPrescriptionState = 3,
//        }


//        private PrescriptionState _PrescripState = PrescriptionState.NewPrescriptionState;
//        public PrescriptionState PrescripState
//        {
//            get
//            {
//                return _PrescripState;
//            }
//            set
//            {
//                //if (_PrescripState != value)
//                {
//                    _PrescripState = value;
//                    NotifyOfPropertyChange(() => PrescripState);
//                    switch (PrescripState)
//                    {
//                        case PrescriptionState.NewPrescriptionState:
//                            mNewPrescriptionState = true && btnCreateNewVisibility;
//                            mEditPrescriptionState = false;
//                            mPublishNewPrescriptionState = false;                            
//                            break;
//                        case PrescriptionState.EditPrescriptionState:
//                            mNewPrescriptionState = false;
//                            mEditPrescriptionState = true && btnUpdateVisibility;
//                            mPublishNewPrescriptionState = false;
//                            break;
//                        case PrescriptionState.PublishNewPrescriptionState:
//                            mNewPrescriptionState = false;
//                            mEditPrescriptionState = false;
//                            mPublishNewPrescriptionState = true && btnCopyToVisible;
//                            break;
//                    }
//                }
//            }
//        }

//        private bool _mNewPrescriptionState;
//        public bool mNewPrescriptionState
//        {
//            get
//            {
//                return _mNewPrescriptionState;
//            }
//            set
//            {
//                if (_mNewPrescriptionState != value)
//                {
//                    _mNewPrescriptionState = value;
//                    NotifyOfPropertyChange(() => mNewPrescriptionState);
//                }
//            }
//        }

//        private bool _mEditPrescriptionState;
//        public bool mEditPrescriptionState
//        {
//            get
//            {
//                return _mEditPrescriptionState;
//            }
//            set
//            {
//                if (_mEditPrescriptionState != value)
//                {
//                    _mEditPrescriptionState = value;
//                    NotifyOfPropertyChange(() => mEditPrescriptionState);
//                }
//            }
//        }

//        private bool _mPublishNewPrescriptionState;
//        public bool mPublishNewPrescriptionState
//        {
//            get
//            {
//                return _mPublishNewPrescriptionState;
//            }
//            set
//            {
//                if (_mPublishNewPrescriptionState != value)
//                {
//                    _mPublishNewPrescriptionState = value;
//                    NotifyOfPropertyChange(() => mPublishNewPrescriptionState);
//                }
//            }
//        }
        
//        #endregion
//    }
//}

