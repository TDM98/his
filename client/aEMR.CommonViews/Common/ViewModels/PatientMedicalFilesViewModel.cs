using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.BaseModel;
using aEMR.Infrastructure;

/*
//16052018: #001 TTM: Add condition only number for TextBox FileCodeNumber
 * 20221010 #002 QTD: Thêm cấu hình ẩn tạo mới HSBA nội trú
 */

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IPatientMedicalFiles)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientMedicalFilesViewModel : ViewModelBase, IPatientMedicalFiles
    {

        public override bool IsProcessing
        {
            get
            {
                return _IsLoadingGetPMRsByPtIDFinish
                    || _IsLoadingGetPMRsByPtIDCurrent
                    || _IsLoadingSave;
            }
        }

        public override string StatusText
        {
            get
            {
                if (_IsLoadingGetPMRsByPtIDFinish)
                {
                    return string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z1241_G1_LoadDSHSoCu);
                }
                if (_IsLoadingGetPMRsByPtIDCurrent)
                {
                    return string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.T1563_G1_HSoBAnHTai);
                }
                if (_IsLoadingSave)
                {
                    return string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0343_G1_DangLuu);
                }
                return string.Empty;
            }
        }

        private bool _IsLoadingGetPMRsByPtIDFinish;
        public bool IsLoadingGetPMRsByPtIDFinish
        {
            get { return _IsLoadingGetPMRsByPtIDFinish; }
            set
            {
                if (_IsLoadingGetPMRsByPtIDFinish != value)
                {
                    _IsLoadingGetPMRsByPtIDFinish = value;
                    NotifyOfPropertyChange(() => IsLoadingGetPMRsByPtIDFinish);
                    NotifyWhenBusy();
                }
            }
        }

        private bool _IsLoadingGetPMRsByPtIDCurrent;
        public bool IsLoadingGetPMRsByPtIDCurrent
        {
            get { return _IsLoadingGetPMRsByPtIDCurrent; }
            set
            {
                if (_IsLoadingGetPMRsByPtIDCurrent != value)
                {
                    _IsLoadingGetPMRsByPtIDCurrent = value;
                    NotifyOfPropertyChange(() => IsLoadingGetPMRsByPtIDCurrent);
                    NotifyWhenBusy();
                }
            }
        }


        private bool _IsLoadingSave;
        public bool IsLoadingSave
        {
            get { return _IsLoadingSave; }
            set
            {
                if (_IsLoadingSave != value)
                {
                    _IsLoadingSave = value;
                    NotifyOfPropertyChange(() => IsLoadingSave);
                    NotifyWhenBusy();
                }
            }
        }

        private bool _isCreated=false;
        public bool isCreated
        {
            get { return _isCreated; }
            set
            {
                if (_isCreated != value)
                {
                    _isCreated = value;
                    NotifyOfPropertyChange(() => isCreated);                    
                }
            }
        }


        private ObservableCollection<PatientMedicalFile> _allPatientMedicalFile;
        public ObservableCollection<PatientMedicalFile> allPatientMedicalFile
        {
            get { return _allPatientMedicalFile; }
            set
            {
                _allPatientMedicalFile = value;
                NotifyOfPropertyChange(() => allPatientMedicalFile);
            }
        }

        private PatientMedicalFile _PatientMedicalFileCurrent;
        public PatientMedicalFile PatientMedicalFileCurrent
        {
            get { return _PatientMedicalFileCurrent; }
            set
            {
                _PatientMedicalFileCurrent = value;
                NotifyOfPropertyChange(() => PatientMedicalFileCurrent);
            }
        }

        private PatientMedicalFile _selectedPatientMedicalFile;
        public PatientMedicalFile selectedPatientMedicalFile
        {
            get { return _selectedPatientMedicalFile; }
            set
            {
                _selectedPatientMedicalFile = value;
                NotifyOfPropertyChange(() => selectedPatientMedicalFile);
            }
        }

        private ObservableCollection<PatientMedicalRecord> _allPatientMedicalRecord;
        public ObservableCollection<PatientMedicalRecord> allPatientMedicalRecord
        {
            get { return _allPatientMedicalRecord; }
            set
            {
                _allPatientMedicalRecord = value;
                NotifyOfPropertyChange(() => allPatientMedicalRecord);
            }
        }

        private PatientMedicalRecord _curPatientMedicalRecord;
        public PatientMedicalRecord curPatientMedicalRecord
        {
            get { return _curPatientMedicalRecord; }
            set
            {
                _curPatientMedicalRecord = value;
                NotifyOfPropertyChange(() => curPatientMedicalRecord);
            }
        }

        private bool checkExist(string stcheck) 
        {
            if (allPatientMedicalFile == null || allPatientMedicalFile.Count<1)
            {
                return false;
            }
            int count = 0;
            foreach(var item in allPatientMedicalFile)
            {
                if (item.FileCodeNumber.Equals(stcheck))
                {
                    count++;
                    if(count>1) 
                        return true;
                }
            }
            return false;
        }

        private bool CheckValid(PatientMedicalFile p) 
        {
            return p == null ? false : p.Validate();
        }

        private void GetPMRsByPtIDFinish(long? patientID)
        {
            IsLoadingGetPMRsByPtIDFinish = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetPMRsByPtID(patientID, 2, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndGetPMRsByPtID(asyncResult);

                            if (items != null)
                            {
                                allPatientMedicalRecord = new ObservableCollection<PatientMedicalRecord>(items);
                            }
                            else
                            {
                                allPatientMedicalRecord = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoadingGetPMRsByPtIDFinish = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        private void GetPatientMedicalFiles_PatientRecID(long PatientRecID)
        {
            IsLoadingGetPMRsByPtIDFinish = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPatientMedicalFiles_ByPatientRecID(PatientRecID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndPatientMedicalFiles_ByPatientRecID(asyncResult);

                            if (items != null)
                            {
                                allPatientMedicalFile = new ObservableCollection<PatientMedicalFile>(items);
                                
                            }
                            else
                            {
                                allPatientMedicalRecord = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoadingGetPMRsByPtIDFinish = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }
        private void GetPMRsByPtIDCurrent(long? patientID)
        {
            IsLoadingGetPMRsByPtIDCurrent = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetPMRsByPtID(patientID, 1, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndGetPMRsByPtID(asyncResult);

                            if (items != null)
                            {
                                if (items.Count > 0)
                                {
                                    curPatientMedicalRecord = items[0];
                                    isCreated = false;
                                    //Lay danh sach ho so cua benh nhan o day
                                    PatientMedicalFileCurrent.PatientID = curPatientMedicalRecord.PatientID.Value;
                                    PatientMedicalFileCurrent.PatientRecID = curPatientMedicalRecord.PatientRecID;
                                    GetPatientMedicalFiles_PatientRecID(curPatientMedicalRecord.PatientRecID);
                                }
                                else
                                {
                                    curPatientMedicalRecord = new PatientMedicalRecord();
                                    PatientMedicalFileCurrent.PatientID = patientID.Value;
                                    curPatientMedicalRecord.PatientRecID = 0;
                                    isCreated = true;
                                }
                            }
                            else
                            {
                                curPatientMedicalRecord = new PatientMedicalRecord();
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoadingGetPMRsByPtIDCurrent = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        private Patient _ObjPatient;
        public Patient ObjPatient
        {
            get { return _ObjPatient; }
            set
            {
                _ObjPatient = value;
            }
        }
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public PatientMedicalFilesViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;


            authorization();
            PatientMedicalFileCurrent = new PatientMedicalFile();
            PatientMedicalFileCurrent.RecCreatedDate = Globals.ServerDate.Value;
            selectedPatientMedicalFile = new PatientMedicalFile();
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            GetPMRsByPtIDCurrent(ObjPatient.PatientID);
            GetPMRsByPtIDFinish(ObjPatient.PatientID);
        }


        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

            mHoSoBenhAn_Xoa = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient, (int)ePatient.mPMFile,
                                   (int)oRegistrionEx.mHoSoBenhAn_Xoa, (int)ePermission.mView);
        }

        private bool _mHoSoBenhAn_Xoa = true;

        public bool mHoSoBenhAn_Xoa
        {
            get
            {
                return _mHoSoBenhAn_Xoa;
            }
            set
            {
                if (_mHoSoBenhAn_Xoa == value)
                    return;
                _mHoSoBenhAn_Xoa = value;
                NotifyOfPropertyChange(() => mHoSoBenhAn_Xoa);
            }
        }

        public void CancelCmd()
        {
            TryClose();
        }

        public new void Refresh()
        {
            GetPatientMedicalFiles_PatientRecID(curPatientMedicalRecord.PatientRecID);
        }
        public void cmdSave()
        {
            if (PatientMedicalFileCurrent == null)
            {
                PatientMedicalFileCurrent = new PatientMedicalFile();
            }

            //KMx: Sau khi lưu thành công thì new PatientMedicalFileCurrent, dẫn đến tình trạng lưu thêm 1 lần nữa thì không có PatientID và PatientRecID nên phải gán lại.
            if (ObjPatient != null)
            {
                PatientMedicalFileCurrent.PatientID = ObjPatient.PatientID;
            }
            if (curPatientMedicalRecord != null)
            {
                PatientMedicalFileCurrent.PatientRecID = curPatientMedicalRecord.PatientRecID;
            }

            if (string.IsNullOrEmpty(PatientMedicalFileCurrent.FileCodeNumber))
            {
                MessageBox.Show(eHCMSResources.A0878_G1_Msg_InfoNhapSoHS);
                return;
            }
            if (!CheckValid(PatientMedicalFileCurrent))
            {
                MessageBox.Show(eHCMSResources.A0540_G1_Msg_InfoDataKhHopLe);
                return;
            }
            if (checkExist(PatientMedicalFileCurrent.FileCodeNumber.Trim()))
            {
                MessageBox.Show(eHCMSResources.A0968_G1_Msg_InfoSoHSBiTrung);
                return;
            }
            /*▼====: #001*/
            //Su dung Convert de kiem tra chuoi nhap vao co ky tu khong.
            PatientMedicalFileCurrent.FileCodeNumber = PatientMedicalFileCurrent.FileCodeNumber.Trim();
            decimal mCodeNumber = 0;
            decimal.TryParse(PatientMedicalFileCurrent.FileCodeNumber, out mCodeNumber);
            if (mCodeNumber == 0)
            {
                MessageBox.Show(eHCMSResources.Z2206_G1_SoHSKhongHopLe);
                return;
            }
            /*▲====: #001*/
            PatientMedicalFiles_Save(PatientMedicalFileCurrent);
        }


        public void lnkDelete_Click()
        {
            if (!mHoSoBenhAn_Xoa)
            {
                MessageBox.Show(eHCMSResources.A0187_G1_Msg_InfoKhTheXoaSoHS, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (selectedPatientMedicalFile == null)
            {
                MessageBox.Show(eHCMSResources.K0308_G1_ChonHSoXoa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (MessageBox.Show(eHCMSResources.A0133_G1_Msg_ConfXoaSoHoSo, eHCMSResources.G2363_G1_XNhan,MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                return;
            }

            PatientMedicalFiles_Delete(selectedPatientMedicalFile);
        }


        public void ActiveCmd()
        {
            if (selectedPatientMedicalFile == null)
            {
                MessageBox.Show(eHCMSResources.K0307_G1_ChonHSoCNhat, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            PatientMedicalFiles_Active(selectedPatientMedicalFile);
        }


        public void lnkUpdate_Click() 
        {
            if (!CheckValid(selectedPatientMedicalFile))
            {
                MessageBox.Show(eHCMSResources.A0540_G1_Msg_InfoDataKhHopLe);
                return;
            }
            if (checkExist(selectedPatientMedicalFile.FileCodeNumber.Trim()))
            {
                MessageBox.Show(eHCMSResources.A0968_G1_Msg_InfoSoHSBiTrung);
                return;
            }
            PatientMedicalFiles_Update(selectedPatientMedicalFile);
        }


        private void PatientMedicalFiles_Save(PatientMedicalFile curPMFile)
        {
            IsLoadingSave = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginInsert_PatientMedicalFiles(curPMFile, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            long PatientRecID = 0;
                            var b = contract.EndInsert_PatientMedicalFiles(out PatientRecID,asyncResult);
                            if (b)
                            {
                                MessageBox.Show(string.Format("{0}!", eHCMSResources.A0079_G1_Msg_InfoThemMoiOK));
                                PatientMedicalFileCurrent = new PatientMedicalFile();
                                PatientMedicalFileCurrent.RecCreatedDate = Globals.ServerDate.Value;
                                GetPatientMedicalFiles_PatientRecID(PatientRecID);
                                curPatientMedicalRecord.PatientRecID = PatientRecID;
                            }
                            else
                            {
                                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0477_G1_LuuKhongThanhCong));
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                        }
                        finally
                        {
                            IsLoadingSave = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        private void PatientMedicalFiles_Update(PatientMedicalFile curPMFile)
        {
            IsLoadingSave = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPatientMedicalFiles_Update(curPMFile, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var b = contract.EndPatientMedicalFiles_Update(asyncResult);
                            if (b)
                            {
                                MessageBox.Show(eHCMSResources.A0296_G1_Msg_InfoSuaOK);
                                GetPatientMedicalFiles_PatientRecID(curPatientMedicalRecord.PatientRecID);
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                        }
                        finally
                        {
                            IsLoadingSave = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }


        private void PatientMedicalFiles_Delete(PatientMedicalFile curPMFile)
        {
            IsLoadingSave = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPatientMedicalFiles_Delete(curPMFile, Globals.LoggedUserAccount.Staff.StaffID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var b = contract.EndPatientMedicalFiles_Delete(asyncResult);
                            if (b)
                            {
                                MessageBox.Show(eHCMSResources.K0537_G1_XoaOk);
                            }
                            else
                            {
                                MessageBox.Show(string.Format("{0}!", eHCMSResources.K0484_G1_XoaFail));
                            }
                            GetPatientMedicalFiles_PatientRecID(curPatientMedicalRecord.PatientRecID);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoadingSave = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        private void PatientMedicalFiles_Active(PatientMedicalFile curPMFile)
        {
            IsLoadingSave = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPatientMedicalFiles_Active(curPMFile, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var b = contract.EndPatientMedicalFiles_Active(asyncResult);
                            if (b)
                            {
                                MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK);
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail);
                            }
                            GetPatientMedicalFiles_PatientRecID(curPatientMedicalRecord.PatientRecID);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoadingSave = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        public bool IsDisableCreateMedicalFile
        {
            get
            {
                return Globals.ServerConfigSection.CommonItems.IsDisableCreateMedicalFile;
            }
        }
    }
}
