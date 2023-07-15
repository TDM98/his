using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ServiceClient.Consultation_PCLs;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Controls;
using eHCMSLanguage;
using System.Linq;
using aEMR.Common.BaseModel;
using aEMR.Common;
using System.Windows.Media;
/*
* 20210427 #001 TNHX: 
* 20220406 #002 TNHX: 1115 - Chỉnh quy trình gửi hồ sơ cho KHTH chờ kiểm duyệt
* 20220711 #003 DatTB: Đổi màu trạng thái khi gửi lần 2.
* 20220730 #004 DatTB: Đổi vị trí 2 else if (Hồ sơ gửi lần 2 vẫn đổi màu khi DLS xử lí)
*/
namespace aEMR.ConsultantEPrescription.CommonRecs.ViewModels
{
    [Export(typeof(IDocumentAccordingMOH)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DocumentAccordingMOHViewModel : ViewModelBase, IDocumentAccordingMOH
        , IHandle<SaveDeathCheckRecord_Event>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public DocumentAccordingMOHViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Globals.EventAggregator.Subscribe(this);
            authorization();
        }

        protected override void OnActivate()
        {
            base.OnActivate();

        }
        public void Handle(SaveDeathCheckRecord_Event message)
        {
            if (message != null && message.Result)
            {
                GetDeathCheckRecordByPtRegID(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID);
            }
        }
        public void InitPatientInfo(PatientRegistration CurrentPatientRegistration = null)
        {
            if (Registration_DataStorage == null
                || Registration_DataStorage.CurrentPatientRegistration == null
                || Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo == null
                || Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.DeceasedInfo == null
                )
            { 
                return;
            }
            GetDeathCheckRecordByPtRegID(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID);
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }

        public void grdCMFHistory_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
            CheckMedicalFiles item = e.Row.DataContext as CheckMedicalFiles;
            if (item == null)
            {
                return;
            }
            if (item.V_CheckMedicalFilesStatus == (long)AllLookupValues.V_CheckMedicalFilesStatus.Tra_Ho_So)
            {
                e.Row.Background = new SolidColorBrush(Colors.Red);
            }
            else if (item.V_CheckMedicalFilesStatus == (long)AllLookupValues.V_CheckMedicalFilesStatus.Da_Duyet)
            {
                //▼==== #003
                e.Row.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#0D69FF");
                e.Row.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#0D69FF");
                e.Row.Foreground = new SolidColorBrush(Colors.White);
                //▲==== #003
            }
            //▼==== #004
            else if (item.IsDLSChecked || item.DLSReject)
            {
                e.Row.Background = new SolidColorBrush(Colors.PaleGreen);
            }
            else if (item.V_CheckMedicalFilesStatus == (long)AllLookupValues.V_CheckMedicalFilesStatus.Cho_Duyet_Lai)
            {
                e.Row.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFA200"); // #003
            }
            //▲==== #004
        }

        #region method
        private void GetDeathCheckRecordByPtRegID(long PtRegistrationID)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetDeathCheckRecordByPtRegID(PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                CurDeathCheckRecord = contract.EndGetDeathCheckRecordByPtRegID(asyncResult);
                                IsNewDeathCheckRecord = CurDeathCheckRecord == null || CurDeathCheckRecord.DeathCheckRecordID == 0;
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }
        #endregion
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
        private DeathCheckRecord _CurDeathCheckRecord;
        public DeathCheckRecord CurDeathCheckRecord
        {
            get { return _CurDeathCheckRecord; }
            set
            {
                _CurDeathCheckRecord = value;
                NotifyOfPropertyChange(() => CurDeathCheckRecord);
            }
        }
        private bool _IsNewDeathCheckRecord;
        public bool IsNewDeathCheckRecord
        {
            get { return _IsNewDeathCheckRecord; }
            set
            {
                _IsNewDeathCheckRecord = value;
                NotifyOfPropertyChange(() => IsNewDeathCheckRecord);
            }
        }
        public void AddKiemDiemTuVong()
        {
            if (Registration_DataStorage == null
                || Registration_DataStorage.CurrentPatientRegistration == null
                || Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo == null
                || Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.DeceasedInfo == null
                )
            {
                MessageBox.Show("Bệnh nhân có kết quả điều trị không phải là tử vong không thể tạo");
                return;
            }

            Action<IDeathCheckRecord> onInitDlg = delegate (IDeathCheckRecord deathCheckRecord)
            {
                this.ActivateItem(deathCheckRecord);
                deathCheckRecord.Registration_DataStorage = Registration_DataStorage;
                deathCheckRecord.CurDeathCheckRecord = new DeathCheckRecord {
                    TreatmentProcess = getTreatmentProcess(Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo)  
                };
                deathCheckRecord.InitPatientInfo(Registration_DataStorage.CurrentPatientRegistration);
            };
            GlobalsNAV.ShowDialog<IDeathCheckRecord>(onInitDlg);
        }
        public void EditKiemDiemTuVong()
        {
            if (Registration_DataStorage == null
                || Registration_DataStorage.CurrentPatientRegistration == null
                || Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo == null
                || Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.DeceasedInfo == null
                )
            {
                MessageBox.Show("Bệnh nhân có kết quả điều trị không phải là tử vong không thể tạo");
                return;
            }

            Action<IDeathCheckRecord> onInitDlg = delegate (IDeathCheckRecord deathCheckRecord)
            {
                this.ActivateItem(deathCheckRecord);
                deathCheckRecord.Registration_DataStorage = Registration_DataStorage;
                deathCheckRecord.CurDeathCheckRecord = CurDeathCheckRecord;
                deathCheckRecord.InitPatientInfo(Registration_DataStorage.CurrentPatientRegistration);
            };
            GlobalsNAV.ShowDialog<IDeathCheckRecord>(onInitDlg);
        }
        public void PrintKiemDiemTuVong()
        {
            if (Registration_DataStorage == null
                || Registration_DataStorage.CurrentPatientRegistration == null
                || Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo == null
                || Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.DeceasedInfo == null
                || CurDeathCheckRecord == null
                )
            {
                MessageBox.Show("Bệnh nhân chưa có biên bản kiểm điểm tử vong");
                return;
            }
            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
            {
                proAlloc.eItem = ReportName.XRpt_BienBanKiemDiemTuVong;
                proAlloc.ID = CurDeathCheckRecord.DeathCheckRecordID;
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());

        }
        private string getTreatmentProcess(InPatientAdmDisDetails inPatientAdmDis)
        {
            return inPatientAdmDis.PathologicalProcess
             + Environment.NewLine + "Sau " + (inPatientAdmDis.DischargeDetailRecCreatedDate.Value - inPatientAdmDis.AdmissionDate.Value).Days
             + " ngày điều trị, tình trạng người bệnh:" + Environment.NewLine + inPatientAdmDis.DischargeStatus
             + Environment.NewLine + "Loại xuất viện: " 
             + Globals.AllLookupValueList.Where(x=>x.ObjectTypeID == (long)LookupValues.DISCHARGE_TYPE 
                && x.LookupID == (long)inPatientAdmDis.V_DischargeType).FirstOrDefault().ObjectValue
             ;
        }
    }
}
