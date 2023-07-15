using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using DataEntities;
using Castle.Windsor;
using aEMR.Common.BaseModel;
using System.Collections.ObjectModel;

/*
 * 20180923 #001 TTM: 
 * 20190815 #002 TTM:   BM 0013133: Không load lại kết quả CLS khi tìm đăng ký mới 
 */
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IPatientPCLLaboratoryResult)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientPCLLaboratoryResultViewModel : ViewModelBase, IPatientPCLLaboratoryResult
        //, IHandle<ShowPatientInfo<Patient, PatientRegistration, PatientRegistrationDetail>>
        , IHandle<InitDataForPtPCLLaboratoryResult>
    {
        public object UCDoctorProfileInfo { get; set; }

        public object UCPatientProfileInfo { get; set; }

        public IViewResultPCLLaboratoryByRequest UCViewResultPCLLaboratoryByRequestInfo { get; set; }
        public ObservableCollection<PatientPCLLaboratoryResultDetail> PCLLaboratoryResultCollection
        {
            get
            {
                return UCViewResultPCLLaboratoryByRequestInfo == null ? null : UCViewResultPCLLaboratoryByRequestInfo.allPatientPCLLaboratoryResultDetail;
            }
        }
        public PatientPCLRequest SelectedPCLLaboratory
        {
            get
            {
                return UCViewResultPCLLaboratoryByRequestInfo == null ? null : UCViewResultPCLLaboratoryByRequestInfo.ObjPatientPCLRequest_SearchPaging_Selected;
            }
        }
        public bool IsShowCheckTestItem
        {
            get
            {
                return UCViewResultPCLLaboratoryByRequestInfo == null ? false : UCViewResultPCLLaboratoryByRequestInfo.IsShowCheckTestItem;
            }
            set
            {
                if (UCViewResultPCLLaboratoryByRequestInfo != null)
                {
                    UCViewResultPCLLaboratoryByRequestInfo.IsShowCheckTestItem = value;
                }
            }
        }
        public IViewResultPCLLaboratoryByExamTest UCViewResultPCLLaboratoryByExamTestInfo { get; set; }

        //public IViewResultPCLLaboratoryByListExamTest UCViewResultPCLLaboratoryByListExamTestInfo { get; set; }
        [ImportingConstructor]
        public PatientPCLLaboratoryResultViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Globals.EventAggregator.Subscribe(this);
            CreateSubVM();
        }

        //▼====== #001:     Tạo 1 hàm InitPatientInfo để pass thông tin bệnh nhân xuống cho các ContentControl để nhận thông tin Patient
        //                  Vì màn hình này bây giờ vừa là màn hình chính, vừa là màn hình con. 
        //                  Trường hợp:  1. Màn hình chính: Được khởi tạo lại nên lấy đc PatientID từ Globals do màn hình thông tin chung pass vào Globals.
        //                               2. Màn hình con: Không được khởi tạo nên không lấy PatientID đc 
        //                                  => dùng InitPatientInfo pass thông tin từ ConsultationSummary xuống sử dụng
        public void InitPatientInfo(Patient patientInfo)
        {
            if (patientInfo == null)
            {
                return;
            }
            else
            {
                //▼===== #002
                UCViewResultPCLLaboratoryByRequestInfo.InitPatientInfo(patientInfo);
                UCViewResultPCLLaboratoryByRequestInfo.setDefaultWhenRenewPatient();
                UCViewResultPCLLaboratoryByExamTestInfo.InitPatientInfo(patientInfo);
                UCViewResultPCLLaboratoryByExamTestInfo.setDefaultWhenRenewPatient();
                //▲===== #002
            }
        }
        //▲====== #001

        // TxD 22/09/2018 Added the following to ALLOW child VM of each Tab to get ACCESS to the CENTRAL Data stored in the MainKB (Main KHAM BENH) Screen
        private ICS_DataStorage _CS_DS = null;
        public ICS_DataStorage CS_DS
        {
            get
            {
                return _CS_DS;
            }
            set
            {
                _CS_DS = value;
                ((IPatientInfo)UCPatientProfileInfo).CS_DS = CS_DS;
            }
        }

        private void CreateSubVM()
        {
            UCDoctorProfileInfo = Globals.GetViewModel<ILoginInfo>();
            
            UCPatientProfileInfo = Globals.GetViewModel<IPatientInfo>();
                       
            var uc3 = Globals.GetViewModel<IViewResultPCLLaboratoryByRequest>();
            uc3.SearchCriteria.PatientID = Registration_DataStorage != null && Registration_DataStorage.CurrentPatient != null ? Registration_DataStorage.CurrentPatient.PatientID : 0;
            uc3.SearchCriteria.V_PCLMainCategory = (long)AllLookupValues.V_PCLMainCategory.Laboratory;
            UCViewResultPCLLaboratoryByRequestInfo = uc3;
            
            var uc4 = Globals.GetViewModel<IViewResultPCLLaboratoryByExamTest>();
            uc4.SearchCriteria.PatientID = Registration_DataStorage != null && Registration_DataStorage.CurrentPatient != null ? Registration_DataStorage.CurrentPatient.PatientID : 0;
            uc4.SearchCriteria.V_PCLMainCategory = (long)AllLookupValues.V_PCLMainCategory.Laboratory;
            UCViewResultPCLLaboratoryByExamTestInfo = uc4;

            //▼====== #001:     Xaml đã comment ra nhưng không comment ra 
            //                  => Không sử dụng nhưng lại khởi tạo view này lên, mà trong hàm khởi tạo lại đi gọi Begin/End
            //                  => Sẽ làm chậm 1 chút khi thực hiện Begin/End
            //UCViewResultPCLLaboratoryByListExamTestInfo = Globals.GetViewModel<IViewResultPCLLaboratoryByListExamTest>();         
            //▲====== #001
        }

        private void ActivateSubVM()
        {            
            ActivateItem(UCDoctorProfileInfo);
            ActivateItem(UCPatientProfileInfo);
            ActivateItem(UCViewResultPCLLaboratoryByRequestInfo);
            ActivateItem(UCViewResultPCLLaboratoryByExamTestInfo);
            //ActivateItem(UCViewResultPCLLaboratoryByListExamTestInfo);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            ActivateSubVM();
        }

        public void Handle(InitDataForPtPCLLaboratoryResult message)
        {
            if (Registration_DataStorage.CurrentPatient == null)
            {
                return;
            }

            if (UCViewResultPCLLaboratoryByExamTestInfo != null)
            {
                UCViewResultPCLLaboratoryByExamTestInfo.SearchCriteria.PatientID = Registration_DataStorage.CurrentPatient.PatientID;
                //▼===== 20200408 TTM: Không tự động load dữ liệu khi vào màn hình xem kết quả theo test item.
                //UCViewResultPCLLaboratoryByExamTestInfo.InitData();
                //▲===== 
            }

            if (UCViewResultPCLLaboratoryByRequestInfo != null)
            {
                UCViewResultPCLLaboratoryByRequestInfo.SearchCriteria.PatientID = Registration_DataStorage.CurrentPatient.PatientID;
                UCViewResultPCLLaboratoryByRequestInfo.InitData();
            }
        }


        //KMx: Đổi tên event, không sử dụng chung nữa, vì khó debug, event bắn không kiểm soát được (25/05/2014 11:00).
        //public void Handle(ShowPatientInfo<Patient, PatientRegistration, PatientRegistrationDetail> message)
        //{
        //    if (message != null)
        //    {
        //        if (Registration_DataStorage.CurrentPatient != null)
        //        {
        //            if (UCViewResultPCLLaboratoryByExamTestInfo != null)
        //            {
        //                UCViewResultPCLLaboratoryByExamTestInfo.SearchCriteria.PatientID = Registration_DataStorage.CurrentPatient.PatientID;
        //                UCViewResultPCLLaboratoryByExamTestInfo.InitData();
        //            }

        //            if (UCViewResultPCLLaboratoryByRequestInfo != null)
        //            {
        //                UCViewResultPCLLaboratoryByRequestInfo.SearchCriteria.PatientID = Registration_DataStorage.CurrentPatient.PatientID;
        //                UCViewResultPCLLaboratoryByRequestInfo.InitData();
        //            }
        //        }
        //    }
        //}

        private bool _IsShowSummaryContent = true;
        private bool _IsDialogView = false;
        public bool IsShowSummaryContent
        {
            get => _IsShowSummaryContent; set
            {
                _IsShowSummaryContent = value;
                NotifyOfPropertyChange(() => IsShowSummaryContent);
            }
        }
        public bool IsDialogView
        {
            get
            {
                return _IsDialogView;
            }
            set
            {
                if (_IsDialogView == value)
                {
                    return;
                }
                _IsDialogView = value;
                NotifyOfPropertyChange(() => IsDialogView);
                if (UCViewResultPCLLaboratoryByRequestInfo != null)
                {
                    UCViewResultPCLLaboratoryByRequestInfo.IsDialogView = IsDialogView;
                }
                if (UCViewResultPCLLaboratoryByExamTestInfo != null)
                {
                    UCViewResultPCLLaboratoryByExamTestInfo.IsDialogView = IsDialogView;
                }
            }
        }
        public void LoadPCLRequestResult(PatientPCLRequest aPCLRequest)
        {
            if (UCViewResultPCLLaboratoryByRequestInfo != null)
            {
                UCViewResultPCLLaboratoryByRequestInfo.LoadPCLRequestResult(aPCLRequest);
            }
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
                UCViewResultPCLLaboratoryByRequestInfo.Registration_DataStorage = Registration_DataStorage;
                UCViewResultPCLLaboratoryByExamTestInfo.Registration_DataStorage = Registration_DataStorage;
            }
        }
    }
}