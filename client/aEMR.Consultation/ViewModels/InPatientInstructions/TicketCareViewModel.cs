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
using DataEntities.MedicalInstruction;
using aEMR.Common.BaseModel;
using aEMR.Infrastructure.Events;

/*
 * 20190816 #001 TBL: BM 0013175: Lưu Thông tin theo dõi theo y lệnh vào bảng mới.
 * 20191109 #002 TTM: BM 0017401: [Y lệnh] Bổ sung ComboBox thời gian theo dõi y lệnh.
 * 20221214 #003 QTD: Thêm mới y lệnh ở phiếu chăm sóc
 * 20230505 #004 TNHX: 2767: chi bat buoc nhap it nhat 1 truong 
 */
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(ITicketCare)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class TicketCareViewModel : ViewModelBase, ITicketCare
        , IHandle<DiseaseProgressionFromTicketCare_Event>
    {
        private TicketCare _gTicketCare = null;
        public TicketCare gTicketCare
        {
            get
            {
                return _gTicketCare;
            }
            set
            {
                _gTicketCare = value;
                NotifyOfPropertyChange(() => gTicketCare);
            }
        }

        private long _IntPtDiagDrInstructionID = 0;
        public long IntPtDiagDrInstructionID
        {
            get
            {
                return _IntPtDiagDrInstructionID;
            }
            set
            {
                _IntPtDiagDrInstructionID = value;
                NotifyOfPropertyChange(() => IntPtDiagDrInstructionID);
            }
        }

        private long _PtRegistrationID = 0;
        public long PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                _PtRegistrationID = value;
                NotifyOfPropertyChange(() => PtRegistrationID);
            }
        }

        private long _V_LevelCare = 0;
        public long V_LevelCare
        {
            get
            {
                return _V_LevelCare;
            }
            set
            {
                _V_LevelCare = value;
                NotifyOfPropertyChange(() => V_LevelCare);
            }
        }

        private bool _IsPopUp = true;
        public bool IsPopUp
        {
            get
            {
                return _IsPopUp;
            }
            set
            {
                _IsPopUp = value;
                NotifyOfPropertyChange(() => IsPopUp);
            }
        }

        private IMinHourDateControl _DateExcuteContent;
        public IMinHourDateControl DateExcuteContent
        {
            get { return _DateExcuteContent; }
            set
            {
                _DateExcuteContent = value;
                NotifyOfPropertyChange(() => DateExcuteContent);
            }
        }

        private string _DoctorOrientedTreatment;
        public string DoctorOrientedTreatment
        {
            get { return _DoctorOrientedTreatment; }
            set
            {
                _DoctorOrientedTreatment = value;
                NotifyOfPropertyChange(() => DoctorOrientedTreatment);
            }
        }

        [ImportingConstructor]
        public TicketCareViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            gTicketCare = new TicketCare();
            DateExcuteContent = Globals.GetViewModel<IMinHourDateControl>();
            DateExcuteContent.DateTime = Globals.GetCurServerDateTime();
            GetShortHandDictionaries((long)Globals.LoggedUserAccount.StaffID);
            Globals.EventAggregator.Subscribe(this);
        }

        public void SaveCmd()
        {
            if (gTicketCare == null)
            {
                return;
            }
            string ErrorMessage = "";
            //▼====: #004
            if (string.IsNullOrEmpty(gTicketCare.OrientedTreatment) && string.IsNullOrEmpty(gTicketCare.ExcuteInstruction))
            {
                ErrorMessage += "\n - Vui lòng nhập ít nhất 1 trường dữ liệu!";
            }
            //▲====: #004
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                MessageBox.Show(ErrorMessage);
                return;
            }
            gTicketCare.DateExcute = DateExcuteContent.DateTime.GetValueOrDefault();
            gTicketCare.V_RegistrationType = V_RegistrationType;
            if (IsPopUp)
            {
                this.DlgShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            }
            else
            {
                this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            }
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSaveTicketCare(gTicketCare, Globals.LoggedUserAccount.StaffID.Value, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                TicketCare results = contract.EndSaveTicketCare(asyncResult);
                                if (results != null)
                                {
                                    gTicketCare = results;
                                    GlobalsNAV.ShowMessagePopup(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                                }
                                else
                                {
                                    GlobalsNAV.ShowMessagePopup(eHCMSResources.Z0477_G1_LuuKhongThanhCong);
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                if (IsPopUp)
                                {
                                    this.DlgHideBusyIndicator();
                                }
                                else
                                {
                                    this.HideBusyIndicator();
                                }
                            }
                        }),
                        null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    if (IsPopUp)
                    {
                        this.DlgHideBusyIndicator();
                    }
                    else
                    {
                        this.HideBusyIndicator();
                    }
                }
            });

            t.Start();
        }

        public void GetTicketCare(long IntPtDiagDrInstructionID = 0)
        {
            if (gTicketCare == null)
            {
                return;
            }
            if (IntPtDiagDrInstructionID == 0)
            {
                gTicketCare.PtRegistrationID = PtRegistrationID;
                gTicketCare.StaffID = Globals.LoggedUserAccount.StaffID.Value;
                return;
            }
            if (IsPopUp)
            {
                this.DlgShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            }
            else
            {
                this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            }
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetTicketCare(IntPtDiagDrInstructionID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                TicketCare results = contract.EndGetTicketCare(asyncResult);
                                if (results != null)
                                {
                                    gTicketCare = results;
                                }
                                else
                                {
                                    gTicketCare.IntPtDiagDrInstructionID = IntPtDiagDrInstructionID;
                                    gTicketCare.PtRegistrationID = PtRegistrationID;
                                    gTicketCare.V_LevelCare = V_LevelCare;
                                    gTicketCare.StaffID = Globals.LoggedUserAccount.StaffID.Value;
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            finally
                            {
                                if (IsPopUp)
                                {
                                    this.DlgHideBusyIndicator();
                                }
                                else
                                {
                                    this.HideBusyIndicator();
                                }
                            }
                        }),
                        null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    if (IsPopUp)
                    {
                        this.DlgHideBusyIndicator();
                    }
                    else
                    {
                        this.HideBusyIndicator();
                    }
                }
            });

            t.Start();
        }

        public void PreviewCmd()
        {
            void onInitDlg(ICommonPreviewView proAlloc)
            {
                proAlloc.eItem = ReportName.XRpt_PhieuChamSoc;
                proAlloc.RegistrationID = PtRegistrationID;
                proAlloc.V_RegistrationType = V_RegistrationType;
            }
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        }

        public void BtnCopyDoctorOrientedTreatment()
        {
            if (gTicketCare != null)
            {
                //▼====#003
                if (IntPtDiagDrInstructionID == 0)
                {
                    btDiseaseProgression();
                }
                else
                {
                    if (string.IsNullOrEmpty(gTicketCare.OrientedTreatment))
                    {
                        gTicketCare.OrientedTreatment = DoctorOrientedTreatment;
                    }
                    else
                    {
                        gTicketCare.OrientedTreatment += ", " + DoctorOrientedTreatment;
                    }
                }
                //▲====#003
            }
        }

        private Dictionary<string, string> _ShortHandDictionaryObj;
        public Dictionary<string, string> ShortHandDictionaryObj
        {
            get => _ShortHandDictionaryObj; set
            {
                _ShortHandDictionaryObj = value;
                NotifyOfPropertyChange(() => ShortHandDictionaryObj);
            }
        }

        public void GetShortHandDictionaries(long StaffID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetShortHandDictionariesByStaffID(StaffID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var mShortHandDictionaries = contract.EndGetShortHandDictionariesByStaffID(asyncResult);
                            if (mShortHandDictionaries == null)
                            {
                                ShortHandDictionaryObj = new Dictionary<string, string>();
                            }
                            else
                            {
                                ShortHandDictionaryObj = mShortHandDictionaries.ToDictionary(x => x.ShortHandDictionaryKey.ToString().ToLower(), x => x.ShortHandDictionaryValue.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobalsNAV.ShowMessagePopup(ex.Message);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            t.Start();
        }
        private long _V_RegistrationType = (long)AllLookupValues.RegistrationType.NOI_TRU;
        public long V_RegistrationType
        {
            get
            {
                return _V_RegistrationType;
            }
            set
            {
                _V_RegistrationType = value;
                IsShowPreview = V_RegistrationType == (long)AllLookupValues.RegistrationType.NOI_TRU;
                NotifyOfPropertyChange(() => V_RegistrationType);
            }
        }
        private bool _IsShowPreview = true;
        public bool IsShowPreview
        {
            get
            {
                return _IsShowPreview;
            }
            set
            {
                _IsShowPreview = value;
                NotifyOfPropertyChange(() => IsShowPreview);
            }
        }
        //▼====#003
        public void btDiseaseProgression()
        {
            GlobalsNAV.ShowDialog<IDiseaseProgression>((proAlloc) => {
                proAlloc.IsOpenFromTicketCare = true;
            }, null, false, true, new Size(950, 950));
        }

        public void Handle(DiseaseProgressionFromTicketCare_Event message)
        {
            if (message != null)
            {
                if (String.IsNullOrEmpty(gTicketCare.OrientedTreatment))
                {
                    gTicketCare.OrientedTreatment += message.SelectedDiseaseProgression.Substring(2, message.SelectedDiseaseProgression.Length - 2);
                }
                else
                {
                    gTicketCare.OrientedTreatment += message.SelectedDiseaseProgression;
                }
            }
        }
        //▲====#003
    }
}
