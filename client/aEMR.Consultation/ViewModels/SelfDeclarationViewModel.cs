using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.Common.Utilities;
using aEMR.Controls;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.ViewContracts.Consultation_ePrescription;
using Caliburn.Micro;
using Castle.Windsor;
using DataEntities;
using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
/*
 * 20220308 #001 QTD:   Thêm trường Cho điều trị tại khoa, Ghi chú, Lý do vào viện
 * 20221231 #002 BLQ: Truyền thông tin từ màn hình chẩn đoán vào phiếu khám vào viện từ màn hình Hồ sơ bệnh án
 * 20230105 #003 DatTB: Thêm biến xác định nội/ngoại trú
 * 20230109 #004 DatTB: Thêm phiếu tự khai và cam kết điều trị
 */

namespace aEMR.Consultation.ViewModels
{
    [Export(typeof(ISelfDeclaration)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class SelfDeclarationViewModel : ViewModelBase, ISelfDeclaration
    {
        [ImportingConstructor]
        public SelfDeclarationViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }

        public void GetSelfDeclarationByPatientID()
        {
            this.DlgShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetSelfDeclarationByPatientID(PatientID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            CurSelfDeclaration = contract.EndGetSelfDeclarationByPatientID(asyncResult);
                            if(CurSelfDeclaration.SelfDeclarationSheetID == 0)
                            {
                                CurSelfDeclaration.PatientID = PatientID;
                                CurSelfDeclaration.PtRegistrationID = PtRegistrationID;
                                CurSelfDeclaration.V_RegistrationType = V_RegistrationType;
                                MessageBox.Show("Bệnh nhân chưa có thông tin dị ứng trước đây");
                            }
                            else
                            {
                                CurSelfDeclaration.SelfDeclarationSheetID = 0;
                                CurSelfDeclaration.PtRegistrationID = PtRegistrationID;
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //IsWaitingGetBlankDiagnosisTreatmentByPtID = false;
                            this.DlgHideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }
        public void GetSelfDeclarationByPtRegistrationID()
        {
            this.DlgShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetSelfDeclarationByPtRegistrationID(PtRegistrationID, V_RegistrationType, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            CurSelfDeclaration = contract.EndGetSelfDeclarationByPtRegistrationID(asyncResult);
                            if(CurSelfDeclaration.SelfDeclarationSheetID == 0)
                            {
                                CurSelfDeclaration.PatientID = PatientID;
                                CurSelfDeclaration.PtRegistrationID = PtRegistrationID;
                                CurSelfDeclaration.V_RegistrationType = V_RegistrationType;
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //IsWaitingGetBlankDiagnosisTreatmentByPtID = false;
                            this.DlgHideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }
        private void SaveSelfDeclaration()
        {
            this.DlgShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSaveSelfDeclaration(CurSelfDeclaration, Globals.LoggedUserAccount.Staff.StaffID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var result = contract.EndSaveSelfDeclaration(asyncResult);
                            if (result)
                            {
                                GetSelfDeclarationByPtRegistrationID();
                                MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.DlgHideBusyIndicator();
                        }

                    }), null);
                }
            });

            t.Start();
        }

        private long _PatientID;
        public long PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                if (_PatientID != value)
                {
                    _PatientID = value;
                    NotifyOfPropertyChange(() => PatientID);
                }
            }
        }

        private long _PtRegistrationID;
        public long PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                if (_PtRegistrationID != value)
                {
                    _PtRegistrationID = value;
                    NotifyOfPropertyChange(() => PtRegistrationID);
                }
            }
        }

        private long _V_RegistrationType;
        public long V_RegistrationType
        {
            get
            {
                return _V_RegistrationType;
            }
            set
            {
                if (_V_RegistrationType != value)
                {
                    _V_RegistrationType = value;

                    IsInPt = (_V_RegistrationType == Convert.ToInt64(AllLookupValues.RegistrationType.NOI_TRU)) ? true : false;
                    NotifyOfPropertyChange(() => IsInPt);
                    NotifyOfPropertyChange(() => V_RegistrationType);
                }
            }
        }

        private SelfDeclaration _CurSelfDeclaration;
        public SelfDeclaration CurSelfDeclaration
        {
            get
            {
                return _CurSelfDeclaration;
            }
            set
            {
                _CurSelfDeclaration = value;
                NotifyOfPropertyChange(() => CurSelfDeclaration);
            }
        }
        private bool _Answer1_Enable;
        public bool Answer1_Enable
        {
            get
            {
                return _Answer1_Enable;
            }
            set
            {
                _Answer1_Enable = value;
                NotifyOfPropertyChange(() => Answer1_Enable);
            }
        }
        private bool _Answer2_Enable;
        public bool Answer2_Enable
        {
            get
            {
                return _Answer2_Enable;
            }
            set
            {
                _Answer2_Enable = value;
                NotifyOfPropertyChange(() => Answer2_Enable);
            }
        }
        private bool _Answer3_Enable;
        public bool Answer3_Enable
        {
            get
            {
                return _Answer3_Enable;
            }
            set
            {
                _Answer3_Enable = value;
                NotifyOfPropertyChange(() => Answer3_Enable);
            }
        }
        private bool _Answer4_Enable;
        public bool Answer4_Enable
        {
            get
            {
                return _Answer4_Enable;
            }
            set
            {
                _Answer4_Enable = value;
                NotifyOfPropertyChange(() => Answer4_Enable);
            }
        }
        private bool _Answer5_Enable;
        public bool Answer5_Enable
        {
            get
            {
                return _Answer5_Enable;
            }
            set
            {
                _Answer5_Enable = value;
                NotifyOfPropertyChange(() => Answer5_Enable);
            }
        }
        private bool _Answer6_Enable;
        public bool Answer6_Enable
        {
            get
            {
                return _Answer6_Enable;
            }
            set
            {
                _Answer6_Enable = value;
                NotifyOfPropertyChange(() => Answer6_Enable);
            }
        }
        public void cbxAnswer1_SelectedChanged(object sender, EventArgs e)
        {
            if(sender != null)
            {
                ComboBox combo = sender as ComboBox;
                SetDataAnswer(1, combo.SelectedIndex == 1);
            }
        }
        public void cbxAnswer2_SelectedChanged(object sender, EventArgs e)
        {
            if(sender != null)
            {
                ComboBox combo = sender as ComboBox;
                SetDataAnswer(2, combo.SelectedIndex == 1);
            }
        }
        public void cbxAnswer3_SelectedChanged(object sender, EventArgs e)
        {
            if(sender != null)
            {
                ComboBox combo = sender as ComboBox;
                SetDataAnswer(3, combo.SelectedIndex == 1);
            }
        }
        public void cbxAnswer4_SelectedChanged(object sender, EventArgs e)
        {
            if(sender != null)
            {
                ComboBox combo = sender as ComboBox;
                SetDataAnswer(4, combo.SelectedIndex == 1);
            }
        }
        public void cbxAnswer5_SelectedChanged(object sender, EventArgs e)
        {
            if(sender != null)
            {
                ComboBox combo = sender as ComboBox;
                SetDataAnswer(5, combo.SelectedIndex == 1);
            }
        }
        public void cbxAnswer6_SelectedChanged(object sender, EventArgs e)
        {
            if(sender != null)
            {
                ComboBox combo = sender as ComboBox;
                SetDataAnswer(6, combo.SelectedIndex == 1);
            }
        }
        private void SetDataAnswer(int AnswerIdx, bool Answer)
        {
            switch (AnswerIdx)
            {
                case 1:
                    //CurSelfDeclaration.Answer1 = Answer;
                    Answer1_Enable = Answer;
                    if (!Answer && CurSelfDeclaration != null)
                    {
                        CurSelfDeclaration.Answer1_Count = 0;
                        CurSelfDeclaration.Answer1_Drug = "";
                        CurSelfDeclaration.Answer1_Solve = "";
                    }
                    break;
                case 2:
                    //CurSelfDeclaration.Answer2 = Answer;
                    Answer2_Enable = Answer;
                    if (!Answer && CurSelfDeclaration != null)
                    {
                        CurSelfDeclaration.Answer2_Count = 0;
                        CurSelfDeclaration.Answer2_Drug = "";
                        CurSelfDeclaration.Answer2_Solve = "";
                    }
                    break;
                case 3:
                    //CurSelfDeclaration.Answer3 = Answer;
                    Answer3_Enable = Answer;
                    if (!Answer && CurSelfDeclaration != null)
                    {
                        CurSelfDeclaration.Answer3_Count = 0;
                        CurSelfDeclaration.Answer3_Drug = "";
                        CurSelfDeclaration.Answer3_Solve = "";
                    }
                    break;
                case 4:
                    //CurSelfDeclaration.Answer4 = Answer;
                    Answer4_Enable = Answer;
                    if (!Answer && CurSelfDeclaration != null)
                    {
                        CurSelfDeclaration.Answer4_Count = 0;
                        CurSelfDeclaration.Answer4_Drug = "";
                        CurSelfDeclaration.Answer4_Solve = "";
                    }
                    break;
                case 5:
                    //CurSelfDeclaration.Answer5 = Answer;
                    Answer5_Enable = Answer;
                    if (!Answer && CurSelfDeclaration != null)
                    {
                        CurSelfDeclaration.Answer5_Count = 0;
                        CurSelfDeclaration.Answer5_Drug = "";
                        CurSelfDeclaration.Answer5_Solve = "";
                    }
                    break;
                case 6:
                    //CurSelfDeclaration.Answer6 = Answer;
                    Answer6_Enable = Answer;
                    if (!Answer && CurSelfDeclaration != null)
                    {
                        CurSelfDeclaration.Answer6_Count = 0;
                        CurSelfDeclaration.Answer6_Drug = "";
                        CurSelfDeclaration.Answer6_Solve = "";
                    }
                    break;
            }
        }
        private bool CheckValidAnswer()
        {
            if(CurSelfDeclaration == null)
            {
                return false;
            }
            if (CurSelfDeclaration.Answer1 
                && (CurSelfDeclaration.Answer1_Count <= 0
                || string.IsNullOrWhiteSpace(CurSelfDeclaration.Answer1_Drug)
                || string.IsNullOrWhiteSpace(CurSelfDeclaration.Answer1_Solve)))
            {
                return false;
            }
            if (CurSelfDeclaration.Answer2 
                && (CurSelfDeclaration.Answer2_Count <= 0
                || string.IsNullOrWhiteSpace(CurSelfDeclaration.Answer2_Drug)
                || string.IsNullOrWhiteSpace(CurSelfDeclaration.Answer2_Solve)))
            {
                return false;
            }
            if (CurSelfDeclaration.Answer3
                && (CurSelfDeclaration.Answer3_Count <= 0
                || string.IsNullOrWhiteSpace(CurSelfDeclaration.Answer3_Drug)
                || string.IsNullOrWhiteSpace(CurSelfDeclaration.Answer3_Solve)))
            {
                return false;
            }
            if (CurSelfDeclaration.Answer4
                && (CurSelfDeclaration.Answer4_Count <= 0
                || string.IsNullOrWhiteSpace(CurSelfDeclaration.Answer4_Drug)
                || string.IsNullOrWhiteSpace(CurSelfDeclaration.Answer4_Solve)))
            {
                return false;
            }
            if (CurSelfDeclaration.Answer5
                && (CurSelfDeclaration.Answer5_Count <= 0
                || string.IsNullOrWhiteSpace(CurSelfDeclaration.Answer5_Drug)
                || string.IsNullOrWhiteSpace(CurSelfDeclaration.Answer5_Solve)))
            {
                return false;
            }
            if (CurSelfDeclaration.Answer6
                && (CurSelfDeclaration.Answer6_Count <= 0
                || string.IsNullOrWhiteSpace(CurSelfDeclaration.Answer6_Drug)
                || string.IsNullOrWhiteSpace(CurSelfDeclaration.Answer6_Solve)))
            {
                return false;
            }
            return true;
        }
        public void btnCreateByOld()
        {
            if(CurSelfDeclaration.SelfDeclarationSheetID > 0)
            {
                MessageBox.Show("Tờ tự khai hiện tại là lần gần nhất. Không thể lấy lại tờ tự khai nữa");
                return;
            }
            GetSelfDeclarationByPatientID();
        }
        public void btnSave()
        {
            if (!CheckValidAnswer())
            {
                MessageBox.Show("Các dòng có dị ứng phải đánh đầy đủ các thông tin");
                return;
            }
            SaveSelfDeclaration();
        }
        public void btnPreview()
        {
            //▼==== #004
            if (PtRegistrationID == 0)
            {
                MessageBox.Show("Chưa có thông tin bệnh nhân", "Thông báo");
                return;
            }
            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
            {
                //proAlloc.ID = Convert.ToInt64(InPtRegistrationID);
                proAlloc.RegistrationID = Convert.ToInt64(PtRegistrationID);
                proAlloc.V_RegistrationType = Convert.ToInt64(V_RegistrationType);
                proAlloc.eItem = ReportName.XRptSelfDeclaration;
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
            //▲==== #004
        }

        //▼==== #003
        private bool _IsInPt;
        public bool IsInPt
        {
            get
            {
                return _IsInPt;
            }
            set
            {
                _IsInPt = value;
                NotifyOfPropertyChange(() => IsInPt);
            }
        }
        //▲==== #003
    }
}
