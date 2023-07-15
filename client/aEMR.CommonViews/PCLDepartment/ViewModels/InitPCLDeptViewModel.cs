using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.CommonTasks;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Controls;
using System.Linq;
using aEMR.Common.Collections;
using aEMR.Common;
using eHCMS.CommonUserControls.CommonTasks;
using eHCMSLanguage;


namespace aEMR.ViewModels
{
    [Export(typeof(IInitPCLDept)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class InitPCLDeptViewModel : Conductor<object>, IInitPCLDept
    {
        public InitPCLDeptViewModel()
        {
            authorization();

            //KMx Chuyển thứ tự gọi hàm LoadData() từ trong hàm authorization() ra đây (15/09/2014 10:27).
            LoadData();

            ObjRefDepartments_RecursiveByDeptID=new ObservableCollection<RefDepartment>();
            ObjRefDepartments_RecursiveByDeptID_Selected=new RefDepartment();
            ObjGetAllLocationsByDeptID = new ObservableCollection<DeptLocation>();
            ObjGetAllLocationsByDeptID_Selected=new DeptLocation();

            ObjV_PCLMainCategory_Selected=new Lookup();
            ObjV_PCLMainCategory_Selected.LookupID = (long)AllLookupValues.V_PCLMainCategory.Imaging;
            ObjV_PCLMainCategory=new ObservableCollection<Lookup>();
            
            ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected=new PCLExamTypeSubCategory();
            ObjPCLExamTypeSubCategory_ByV_PCLMainCategory=new ObservableCollection<PCLExamTypeSubCategory>();

            PCLExamTypeSubCategory_ByV_PCLMainCategory();
            
            ObjPCLResultParamImplementations_GetAll=new ObservableCollection<PCLResultParamImplementations>();
            ObjPCLResultParamImplementations_Selected=new PCLResultParamImplementations();

            //RefDepartments_RecursiveByDeptID();
            //LoadV_PCLMainCategory();

            cboPCLResultParamImplementationsIsEnabled = false;
            PCLResultParamImplementations_GetAll();

        }

        //Khoa,Phòng
        private DataEntities.RefDepartment _ObjRefDepartments_RecursiveByDeptID_Selected;
        public DataEntities.RefDepartment ObjRefDepartments_RecursiveByDeptID_Selected
        {
            get { return _ObjRefDepartments_RecursiveByDeptID_Selected; }
            set
            {
                _ObjRefDepartments_RecursiveByDeptID_Selected = value;
                NotifyOfPropertyChange(() => ObjRefDepartments_RecursiveByDeptID_Selected);
            }
        }

        private ObservableCollection<DataEntities.RefDepartment> _ObjRefDepartments_RecursiveByDeptID;
        public ObservableCollection<DataEntities.RefDepartment> ObjRefDepartments_RecursiveByDeptID
        {
            get { return _ObjRefDepartments_RecursiveByDeptID; }
            set
            {
                _ObjRefDepartments_RecursiveByDeptID = value;
                NotifyOfPropertyChange(() => ObjRefDepartments_RecursiveByDeptID);
            }
        }
        //public void RefDepartments_RecursiveByDeptID()
        //{
        //    ObjRefDepartments_RecursiveByDeptID.Clear();

        //    Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z1168_G1_DSKhoa) });

        //    var t = new Thread(() =>
        //    {
        //        using (var serviceFactory = new ConfigurationManagerServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;

        //            //2 các khoa
        //            contract.BeginRefDepartments_RecursiveByDeptID(2, Globals.DispatchCallback((asyncResult) =>
        //            {
        //                try
        //                {
        //                    var items = contract.EndRefDepartments_RecursiveByDeptID(asyncResult);
        //                    if (items != null)
        //                    {
        //                        ObjRefDepartments_RecursiveByDeptID = new ObservableCollection<DataEntities.RefDepartments>(items);

        //                        //Item Default
        //                        DataEntities.RefDepartments ItemDefault = new DataEntities.RefDepartments();
        //                        ItemDefault.DeptID = -1;
        //                        ItemDefault.DeptName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K1997_G1_ChonKhoa);
        //                        ObjRefDepartments_RecursiveByDeptID.Insert(0, ItemDefault);
        //                        //Item Default

        //                        ObjRefDepartments_RecursiveByDeptID_Selected = ItemDefault;
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                }
        //                finally
        //                {
        //                    Globals.IsBusy = false;
        //                }
        //            }), null);
        //        }


        //    });
        //    t.Start();
        //}

        public void cboDeptFrom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem != null)
            {
                RefDepartment Objtmp = ((sender as ComboBox).SelectedItem as RefDepartment);
                if (Objtmp.DeptID > 0)
                {
                    //GetAllLocationsByDeptID(Objtmp.DeptID);
                    Coroutine.BeginExecute(DoLoadLocations(Objtmp.DeptID));
                }
                else
                {
                    ObjGetAllLocationsByDeptID.Clear();
                }
            }
        }


        private DeptLocation _ObjGetAllLocationsByDeptID_Selected;
        public DeptLocation ObjGetAllLocationsByDeptID_Selected
        {
            get
            {
                return _ObjGetAllLocationsByDeptID_Selected;
            }
            set
            {
                _ObjGetAllLocationsByDeptID_Selected = value;
                NotifyOfPropertyChange(() => ObjGetAllLocationsByDeptID_Selected);
            }
        }
        private ObservableCollection<DeptLocation> _ObjGetAllLocationsByDeptID;
        public ObservableCollection<DeptLocation> ObjGetAllLocationsByDeptID
        {
            get
            {
                return _ObjGetAllLocationsByDeptID;
            }
            set
            {
                _ObjGetAllLocationsByDeptID = value;
                NotifyOfPropertyChange(() => ObjGetAllLocationsByDeptID);
            }
        }

        private IEnumerator<IResult> DoLoadLocations(long deptId)
        {
            var deptLoc = new LoadDeptLoctionByIDTask(deptId);
            yield return deptLoc;
            if (deptLoc.DeptLocations != null)
            {
                ObjGetAllLocationsByDeptID = new ObservableCollection<DeptLocation>(deptLoc.DeptLocations);
            }
            else
            {
                ObjGetAllLocationsByDeptID = new ObservableCollection<DeptLocation>();
            }

            var itemDefault = new DeptLocation();
            itemDefault.Location = new Location();
            itemDefault.Location.LID = -1;
            itemDefault.Location.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0116_G1_HayChonPg);
            ObjGetAllLocationsByDeptID.Insert(0, itemDefault);
            ObjGetAllLocationsByDeptID_Selected = itemDefault;
            yield break;
        }

        public void GetAllLocationsByDeptID(long? deptId)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.K3054_G1_DSPg) });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetAllLocationsByDeptIDOld(deptId, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var allItems = contract.EndGetAllLocationsByDeptIDOld(asyncResult);

                            if (allItems != null)
                            {
                                ObjGetAllLocationsByDeptID = new ObservableCollection<DeptLocation>(allItems);
                            }
                            else
                            {
                                ObjGetAllLocationsByDeptID = new ObservableCollection<DeptLocation>();
                            }

                            var itemDefault = new DeptLocation();
                            itemDefault.Location = new Location();
                            itemDefault.Location.LID = -1;
                            itemDefault.Location.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0116_G1_HayChonPg);
                            ObjGetAllLocationsByDeptID.Insert(0, itemDefault);

                            ObjGetAllLocationsByDeptID_Selected = itemDefault;


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

        //Main,Sub
        //Main
        private Lookup _ObjV_PCLMainCategory_Selected;
        public Lookup ObjV_PCLMainCategory_Selected
        {
            get { return _ObjV_PCLMainCategory_Selected; }
            set
            {
                if (_ObjV_PCLMainCategory_Selected != value)
                {
                    _ObjV_PCLMainCategory_Selected = value;
                    NotifyOfPropertyChange(() => ObjV_PCLMainCategory_Selected);

                   
                }
            }
        }

        private ObservableCollection<Lookup> _ObjV_PCLMainCategory;
        public ObservableCollection<Lookup> ObjV_PCLMainCategory
        {
            get { return _ObjV_PCLMainCategory; }
            set
            {
                _ObjV_PCLMainCategory = value;
                NotifyOfPropertyChange(() => ObjV_PCLMainCategory);
            }
        }

        public void LoadV_PCLMainCategory()
        {
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0185_G1_DSLoai)
                });
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllLookupValuesByType(LookupValues.V_PCLMainCategory,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);

                                    ObjV_PCLMainCategory = new ObservableCollection<Lookup>(allItems);
                                    Lookup firstItem = new Lookup();
                                    firstItem.LookupID = -1;
                                    firstItem.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2034_G1_ChonLoai2);
                                    ObjV_PCLMainCategory.Insert(0, firstItem);

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
                    Globals.IsBusy = false;
                }
            });
            t.Start();
        }
        //Main


        //Sub
        private PCLExamTypeSubCategory _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected;
        public PCLExamTypeSubCategory ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected
        {
            get { return _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected; }
            set
            {
                if (_ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected != value)
                {
                    _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected = value;
                    NotifyOfPropertyChange(() => ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected);

                    Filter(_ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected);
                }
            }
        }

        private ObservableCollection<PCLExamTypeSubCategory> _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory;
        public ObservableCollection<PCLExamTypeSubCategory> ObjPCLExamTypeSubCategory_ByV_PCLMainCategory
        {
            get { return _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory; }
            set
            {
                _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypeSubCategory_ByV_PCLMainCategory);
            }
        }
        public void PCLExamTypeSubCategory_ByV_PCLMainCategory()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0532_G1_DSNhom) });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLExamTypeSubCategory_ByV_PCLMainCategory(ObjV_PCLMainCategory_Selected.LookupID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndPCLExamTypeSubCategory_ByV_PCLMainCategory(asyncResult);

                            if (items != null)
                            {
                                ObjPCLExamTypeSubCategory_ByV_PCLMainCategory = new ObservableCollection<PCLExamTypeSubCategory>(items);

                                PCLExamTypeSubCategory firstItem = new PCLExamTypeSubCategory();
                                firstItem.PCLExamTypeSubCategoryID = -1;
                                firstItem.PCLSubCategoryName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2074_G1_ChonNhom2);
                                ObjPCLExamTypeSubCategory_ByV_PCLMainCategory.Insert(0, firstItem);

                                ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected = firstItem;

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


        public void cboV_PCLMainCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AxComboBox Ctr = sender as AxComboBox;
            if (Ctr == null)
                return;

            Lookup Objtmp = Ctr.SelectedItemEx as Lookup;

            if (Objtmp != null)
            {
                PCLExamTypeSubCategory_ByV_PCLMainCategory();

                CheckInputPCLResultParam(Objtmp);
            }
        }

        private void CheckInputPCLResultParam(Lookup ObjMain)
        {
            ObjPCLResultParamImplementations_Selected.PCLResultParamImpID = -1;

            if (ObjMain.LookupID == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
            {
                cboPCLResultParamImplementationsIsEnabled = false;
            }
            else
            {
                cboPCLResultParamImplementationsIsEnabled = true;
            }
        }

        //public void cboPCLExamTypeSubCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    AxComboBox Ctr = sender as AxComboBox;
        //    if (Ctr == null)
        //        return;

        //    Lookup Objtmp = Ctr.SelectedItemEx as Lookup;

        //    if (Objtmp != null)
        //    {
        //        PCLExamTypeSubCategory_ByV_PCLMainCategory();

        //        //////CheckInputPCLResultParam(Objtmp);

        //        //////ObjPCLExamType_Current.ObjPCLExamTestItemsList = new ObservableCollection<DataEntities.PCLExamTestItems>(); /*Khi có chuyển Loại Main khác */
        //    }
        //}
        
        //Main,Sub

        //PCLResultParamImplementations
        private bool _cboPCLResultParamImplementationsIsEnabled;
        public bool cboPCLResultParamImplementationsIsEnabled
        {
            get { return _cboPCLResultParamImplementationsIsEnabled; }
            set
            {
                if(_cboPCLResultParamImplementationsIsEnabled!=value)
                {
                    _cboPCLResultParamImplementationsIsEnabled = value;
                    NotifyOfPropertyChange(() => cboPCLResultParamImplementationsIsEnabled);
                }
            }

        }

        private DataEntities.PCLResultParamImplementations _ObjPCLResultParamImplementations_Selected;
        public DataEntities.PCLResultParamImplementations ObjPCLResultParamImplementations_Selected
        {
            get { return _ObjPCLResultParamImplementations_Selected; }
            set
            {
                _ObjPCLResultParamImplementations_Selected = value;
                NotifyOfPropertyChange(() => ObjPCLResultParamImplementations_Selected);
            }
        }
        private ObservableCollection<DataEntities.PCLResultParamImplementations> ObjPCLResultParamImplementations_GetAll_Root;

        private ObservableCollection<DataEntities.PCLResultParamImplementations> _ObjPCLResultParamImplementations_GetAll;
        public ObservableCollection<DataEntities.PCLResultParamImplementations> ObjPCLResultParamImplementations_GetAll
        {
            get { return _ObjPCLResultParamImplementations_GetAll; }
            set
            {
                _ObjPCLResultParamImplementations_GetAll = value;
                NotifyOfPropertyChange(() => ObjPCLResultParamImplementations_GetAll);
            }
        }

        private void PCLResultParamImplementations_GetAll()
        {
            Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0533_G1_DSPCLResultParam) });

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLResultParamImplementations_GetAll(Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<DataEntities.PCLResultParamImplementations> allItems = null;

                            try
                            {
                                allItems = client.EndPCLResultParamImplementations_GetAll(asyncResult);

                                if (allItems != null)
                                {
                                    if(!Globals.isAccountCheck)
                                    {
                                        ObjPCLResultParamImplementations_GetAll_Root = new ObservableCollection<DataEntities.PCLResultParamImplementations>(allItems);    
                                    }
                                    else
                                    {
                                        ObjPCLResultParamImplementations_GetAll_Root = new ObservableCollection<DataEntities.PCLResultParamImplementations>();    
                                        foreach (var item in allItems)
                                        {
                                            switch (item.ParamEnum)
                                            {
                                                case  (int)AllLookupValues.PCLResultParamImpID.SIEUAM_GANGSUC_Dipyridamole:
                                                    if(mSATGSDipy)
                                                        ObjPCLResultParamImplementations_GetAll_Root.Add(item);
                                                    break;
                                                case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_GANGSUC_Dobutamine:
                                                    if (mSATGSDobu)
                                                        ObjPCLResultParamImplementations_GetAll_Root.Add(item);
                                                    break;
                                                case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_MACHMAU:
                                                    if (mSAMachMau)
                                                        ObjPCLResultParamImplementations_GetAll_Root.Add(item);
                                                    break;
                                                case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_THUCQUAN:
                                                    if (mSATQuaThucQuan)
                                                        ObjPCLResultParamImplementations_GetAll_Root.Add(item);
                                                    break;
                                                case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_TIMMAU:
                                                    if (mSATMau)
                                                        ObjPCLResultParamImplementations_GetAll_Root.Add(item);
                                                    break;
                                                case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_TIMTHAI:
                                                    if (mSATThai)
                                                        ObjPCLResultParamImplementations_GetAll_Root.Add(item);
                                                    break;
                                                case (int)AllLookupValues.PCLResultParamImpID.ABDOMINAL_ULTRASOUND:
                                                    if (mAbUltra)
                                                        ObjPCLResultParamImplementations_GetAll_Root.Add(item);
                                                    break;
                                                case (int)AllLookupValues.PCLResultParamImpID.KHAC:
                                                    ObjPCLResultParamImplementations_GetAll_Root.Add(item);
                                                    break;
                                            }
                                        }
                                    }
                                    

                                    PCLResultParamImplementations firstItem = new PCLResultParamImplementations();
                                    firstItem.PCLResultParamImpID = -1;
                                    firstItem.ParamName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2092_G1_ChonPCLresultparam2);
                                    ObjPCLResultParamImplementations_GetAll_Root.Insert(0, firstItem);

                                    ObjPCLResultParamImplementations_GetAll = ObjPCLResultParamImplementations_GetAll_Root.DeepCopy();

                                    ObjPCLResultParamImplementations_Selected = firstItem;

                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
                finally
                {
                    Globals.IsBusy = false;
                }
            });
            t.Start();
        }
        //PCLResultParamImplementations


        //bt
        public void btSave()
        {
            //if(ObjGetAllLocationsByDeptID_Selected.DeptLocationID<=0)
            //{
            //    MessageBox.Show(eHCMSResources.A0355_G1_Msg_InfoChonPg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            //    return; 
            //}

            ObjV_PCLMainCategory_Selected.LookupID = (long)AllLookupValues.V_PCLMainCategory.Imaging;

            if (ObjV_PCLMainCategory_Selected.LookupID != (long)AllLookupValues.V_PCLMainCategory.Laboratory)
            {
                if(ObjPCLResultParamImplementations_Selected.PCLResultParamImpID<=0)
                {
                    MessageBox.Show(string.Format("{0}!", eHCMSResources.K2092_G1_ChonPCLresultparam2), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }
            }

            //Cập nhật lại Khoa Phòng Top Menu luôn vì vô chọn Khoa Phòng làm CLS Imaging nó nhiều dk chọn hơn thì mới làm được
            //Globals.EventAggregator.Publish(new LocationSelected() { DeptLocation = ObjGetAllLocationsByDeptID_Selected });
            //Cập nhật lại Khoa Phòng Top Menu luôn vì vô chọn Khoa Phòng làm CLS Imaging nó nhiều dk chọn hơn thì mới làm được


            //Globals.PCLDepartment.ObjPCLExamTypeLocationsDeptLocationID = ObjGetAllLocationsByDeptID_Selected;

            Globals.PCLDepartment.ObjV_PCLMainCategory=ObjV_PCLMainCategory_Selected;
            Globals.PCLDepartment.ObjPCLExamTypeSubCategoryID = ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected;
            Globals.PCLDepartment.ObjPCLResultParamImpID = ObjPCLResultParamImplementations_Selected;
            Globals.PCLDepartment.PCLRequestFromDate = Globals.ServerDate.Value;
            Globals.PCLDepartment.PCLRequestToDate = Globals.ServerDate.Value;

            
            //Globals.EventAggregator.Publish(new ChangePCLDepartmentEvent());
            Globals.EventAggregator.Publish(new InitialPCLImage_Step1_Event());
            Globals.EventAggregator.Publish(new InitialPCLImage_Step2_Event());
            //Globals.EventAggregator.Publish(new InitialPCLImage_Step3_Event());

            //Globals.PatientAllDetails.PatientInfo = null;

            //KMx: Không cần xóa. Vì chỉ có Module khám bệnh mới được sử dụng Globals.PatientAllDetails (24/05/2014 16:20).
            //Show Info xóa thông tin BN trước đó
            //Globals.EventAggregator.Publish(new ShowPatientInfo<Patient, PatientRegistration, PatientRegistrationDetail>() { Pt = null, PtReg = null, PtRegDetail = null });
            //Show Info xóa thông tin BN trước đó


            TryClose();
        }

        public void btClose()
        {
            TryClose();
        }

      
        //bt
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                //LoadData();
                return;
            }

            mSAMachMau = Globals.CheckFunction(Globals.listRefModule
                , (int)eModules.mCLSImaging,(int)eCLSImaging.mSAMachMau);
            mSATGSDipy = Globals.CheckFunction(Globals.listRefModule
                , (int)eModules.mCLSImaging, (int)eCLSImaging.mSATGSDipy);
            mSATGSDobu = Globals.CheckFunction(Globals.listRefModule
                , (int)eModules.mCLSImaging, (int)eCLSImaging.mSATGSDobu);
            mSATMau = Globals.CheckFunction(Globals.listRefModule
                , (int)eModules.mCLSImaging, (int)eCLSImaging.mSATMau);
            mSATQuaThucQuan = Globals.CheckFunction(Globals.listRefModule
                , (int)eModules.mCLSImaging, (int)eCLSImaging.mSATQuaThucQuan);
            mSATThai = Globals.CheckFunction(Globals.listRefModule
                , (int)eModules.mCLSImaging, (int)eCLSImaging.mSATThai);
            mAbUltra = Globals.CheckFunction(Globals.listRefModule
                , (int)eModules.mCLSImaging, (int)eCLSImaging.mAbUltra);
            CheckResponsibility();
        }

        public void LoadData()
        {
            Coroutine.BeginExecute(NewLoadDepartments());
        }
        private IEnumerator<IResult> NewLoadDepartments()
        {
            if (Globals.isAccountCheck)
                {
                    //KMx: Sử dụng biến Globals để dùng chung cho tất cả các ViewModel (15/09/2014 10:25).
                    //var departmentTask = new LoadDepartmentsByV_DeptTypeOperationTask((long)V_DeptTypeOperation.KhoaCanLamSang, LstRefDepartment);
                    var departmentTask = new LoadDepartmentsByV_DeptTypeOperationTask((long)V_DeptTypeOperation.KhoaCanLamSang, Globals.LoggedUserAccount.DeptIDResponsibilityList);
                    yield return departmentTask;
                    //NewDepartments = departmentTask.Departments;
                    if (departmentTask.Departments != null
                        && departmentTask.Departments.Count > 0)
                    {
                        ObjRefDepartments_RecursiveByDeptID = departmentTask.Departments;
                    }
                }
                else
                {
                    var departmentTask = new LoadDepartmentsByV_DeptTypeOperationTask((long)V_DeptTypeOperation.KhoaCanLamSang);
                    yield return departmentTask;
                    if (departmentTask.Departments != null
                        && departmentTask.Departments.Count > 0)
                    {
                        ObjRefDepartments_RecursiveByDeptID = departmentTask.Departments;
                    }
                }

            if (ObjRefDepartments_RecursiveByDeptID == null
                            || ObjRefDepartments_RecursiveByDeptID.Count < 1)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.A0375_G1_Msg_InfoChuaCauHinh2));
            }
            else
            {
                ObjRefDepartments_RecursiveByDeptID_Selected = ObjRefDepartments_RecursiveByDeptID[0];
            }
            yield break;

        }

        #region check staff respone

        //private ObservableCollection<long> _LstRefDepartment;

        //public ObservableCollection<long> LstRefDepartment
        //{
        //    get
        //    {
        //        return _LstRefDepartment;
        //    }
        //    set
        //    {
        //        _LstRefDepartment = value;
        //        NotifyOfPropertyChange(() => LstRefDepartment);
        //    }
        //}

        private StaffDeptResponsibilities _curStaffDeptResponSearch;
        public StaffDeptResponsibilities curStaffDeptResponSearch
        {
            get
            {
                return _curStaffDeptResponSearch;
            }
            set
            {
                if (_curStaffDeptResponSearch == value)
                    return;
                _curStaffDeptResponSearch = value;
                NotifyOfPropertyChange(() => curStaffDeptResponSearch);

            }
        }

        public void CheckResponsibility()
        {
            if (Globals.isAccountCheck && (Globals.LoggedUserAccount.DeptIDResponsibilityList == null || Globals.LoggedUserAccount.DeptIDResponsibilityList.Count < 1))
            {
                MessageBox.Show(eHCMSResources.A0109_G1_Msg_InfoChuaCauHinhTNKhoaPg);
            }
        }

        //KMx: Khi login thì lấy DepartmentID của nhân viên được cấu hình trách nhiệm luôn, không cần chuyển nữa (10/07/2014 17:14).
        //public void CheckResponsibility()
        //{
        //    List<StaffDeptResponsibilities> results = Globals.LoggedUserAccount.AllStaffDeptResponsibilities;

        //    LstRefDepartment = new ObservableCollection<long>();
        //    if (results != null && results.Count > 0)
        //    {
        //        foreach (var item in results)
        //        {
        //            LstRefDepartment.Add(item.DeptID);
        //        }
        //        NotifyOfPropertyChange(() => LstRefDepartment);
        //        LoadData();
        //    }

        //    if (LstRefDepartment.Count < 1)
        //    {
        //        MessageBox.Show("Bạn chưa được phân công trách nhiệm với khoa phòng nào. " +
        //                            "\nLiên hệ với người quản trị để được phân bổ khoa phòng chịu trách nhiệm.");
        //    }
        //}

        #endregion
        #region checking account

        private bool _mSAMachMau = true;
        private bool _mSATGSDipy = true;
        private bool _mSATGSDobu = true;
        private bool _mSATMau = true;
        private bool _mSATQuaThucQuan = true;
        private bool _mSATThai = true;
        private bool _mAbUltra = true;

        public bool mSAMachMau
        {
            get
            {
                return _mSAMachMau;
            }
            set
            {
                if (_mSAMachMau == value)
                    return;
                _mSAMachMau = value;
                NotifyOfPropertyChange(() => mSAMachMau);
            }
        }


        public bool mSATGSDipy
        {
            get
            {
                return _mSATGSDipy;
            }
            set
            {
                if (_mSATGSDipy == value)
                    return;
                _mSATGSDipy = value;
                NotifyOfPropertyChange(() => mSATGSDipy);
            }
        }


        public bool mSATGSDobu
        {
            get
            {
                return _mSATGSDobu;
            }
            set
            {
                if (_mSATGSDobu == value)
                    return;
                _mSATGSDobu = value;
                NotifyOfPropertyChange(() => mSATGSDobu);
            }
        }


        public bool mSATMau
        {
            get
            {
                return _mSATMau;
            }
            set
            {
                if (_mSATMau == value)
                    return;
                _mSATMau = value;
                NotifyOfPropertyChange(() => mSATMau);
            }
        }


        public bool mSATQuaThucQuan
        {
            get
            {
                return _mSATQuaThucQuan;
            }
            set
            {
                if (_mSATQuaThucQuan == value)
                    return;
                _mSATQuaThucQuan = value;
                NotifyOfPropertyChange(() => mSATQuaThucQuan);
            }
        }


        public bool mSATThai
        {
            get
            {
                return _mSATThai;
            }
            set
            {
                if (_mSATThai == value)
                    return;
                _mSATThai = value;
                NotifyOfPropertyChange(() => mSATThai);
            }
        }

        public bool mAbUltra
        {
            get
            {
                return _mAbUltra;
            }
            set
            {
                if (_mAbUltra == value)
                    return;
                _mAbUltra = value;
                NotifyOfPropertyChange(() => mAbUltra);
            }
        }




        #endregion

        private void Filter(PCLExamTypeSubCategory item)
        {
            if (item != null && item.PCLExamTypeSubCategoryID > 0)
            {
                if (ObjPCLResultParamImplementations_GetAll_Root != null)
                {
                    ObjPCLResultParamImplementations_GetAll = ObjPCLResultParamImplementations_GetAll_Root.Where(x => x.PCLExamTypeSubCategoryID == item.PCLExamTypeSubCategoryID).ToObservableCollection().DeepCopy();
                    return;
                }
            }
            ObjPCLResultParamImplementations_GetAll = ObjPCLResultParamImplementations_GetAll_Root.DeepCopy();
        }
    }
}

//KMx: Khi login đã lấy cấu hình trách nhiệm rồi, không cần về server lấy nữa (10/07/2014 15:41).
//public void CheckResponsibility()
//{
//    curStaffDeptResponSearch = new StaffDeptResponsibilities();
//    curStaffDeptResponSearch.Staff = Globals.LoggedUserAccount.Staff;
//    curStaffDeptResponSearch.StaffID = (long)Globals.LoggedUserAccount.StaffID;
//    GetStaffDeptResponsibilitiesByDeptID(curStaffDeptResponSearch, false);
//}
//private void GetStaffDeptResponsibilitiesByDeptID(StaffDeptResponsibilities p, bool isHis)
//{
//    //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
//    var t = new Thread(() =>
//    {
//        using (var serviceFactory = new UserAccountsServiceClient())
//        {
//            var contract = serviceFactory.ServiceInstance;
//            contract.BeginGetStaffDeptResponsibilitiesByDeptID(p, isHis, Globals.DispatchCallback((asyncResult) =>
//            {
//                try
//                {
//                    var results = contract.EndGetStaffDeptResponsibilitiesByDeptID(asyncResult);
//                    LstRefDepartment = new ObservableCollection<long>();
//                    if (results != null && results.Count > 0)
//                    {
//                        foreach (var item in results)
//                        {
//                            LstRefDepartment.Add(item.DeptID);
//                        }
//                        NotifyOfPropertyChange(() => LstRefDepartment);
//                        LoadData();
//                    }

//                    if (LstRefDepartment.Count < 1)
//                    {
//                        MessageBox.Show("Bạn chưa được phân công trách nhiệm với khoa phòng nào. " +
//                                            "\nLiên hệ với người quản trị để được phân bổ khoa phòng chịu trách nhiệm.");
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//                }
//                finally
//                {

//                }

//            }), null);
//        }
//    });

//    t.Start();
//}