using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using System;
using System.Windows.Media.Imaging;
using DataEntities;
using eHCMSLanguage;
using aEMR.ViewContracts;
using aEMR.ReportModel.ReportModels;
using aEMR.Infrastructure;
using Castle.Windsor;
using DevExpress.ReportServer.Printing;
using System.Windows;
using System.Linq;
/*
* 20161227 #001 CMN: Add report for StaffDeptPresence
* 20170105 #002 CMN: Change Report GenericPayment
* 20170215 #003 CMN: Add report TransferForm
* 20170217 #004 CMN: Add TotalInPtRevenue Report
* 20180820 #005 TBL: Edit ReportModel
* 20181005 #006 TNHX: [BM0000034] Add Report PhieuChiDinh
* 20181013 #007 TBL: [BM0002168] Edit paramUltraResParam_EchoCardiographyID -> paramUltraResParams_EchoCardiographyID
* 20181017 #008 TNHX: [BM0002176] Add Report for Thanh Vu Hospital
* 20181023 #009 TNHX: [BM0003221] Update Report for RptPatientPCLRequestDetailsByPatientPCLReqID
* 20181101 #010 TTM:   BM 0004220: Bổ sung thêm param cho report toa thuốc
* 20181124 #011 TNHX: [BM0005316] Add parameter and fix report RptHoatDongQuayDK
* 20181126 #012 TNHX: [BM0005355] Update report "PATIENT_INFO"
* 20181129 #013 TNHX: [BM0005312] Add report PhieuMienGiamNgoaiTru
* 20181205 #014 TNHX: [BM0005300] Add report PhieuChiDinh_DichVu_Ngoaitru
* 20181217 #015 TNHX: [BM0005436] Add param for report PhieuMienGiamNgoaiTru
* 20190622 #016 TNHX: [BM0011874] Create report RptTongHopDoanhThuTheoKhoa
* 20190818 #017 TNHX: [BM0013190] Create report RptBangGiaDV + RptBangGiaCLS
* 20200106 #018 TNHX: [] Create report PhieuChiDinh_DichVu_NoiTru
* 20200417 #019 TNHX: [] Create report XRptPatientInfo_TV
* 20200609 #020 TNHX: [] Chỉnh kết quả XN, lấy chức danh + nhân viên cho kết quả ( dựa vào bảng StaffPosition )
* 20200713 #021 TNHX: [] Thêm report cho cấu hình ViewPrintAllImagingPCLRequestSeparate
* 20200923 #022 TNHX:  Thêm địa chỉ short cho in thông tin bệnh nhân + In thẻ KCB + Chỉnh report thông tin BN cho TV
* 20201003 #023 TNHX: Chỉnh report in thẻ KCB
* 20201108 #024 TNHX: Thêm report Phiếu Cung cấp máu + Toa thuốc gây nghiện/ hướng thần
* 20210430 #025 TNHX: Thêm report Phiếu tư vấn
* 20210923 #026 TNHX: Thêm report Phiếu chăm sóc
* 20220527 #027 DatTB: Thêm Giấy Miễn Tạm Ứng Nội Trú
* 20220527 #028 DatTB: Thêm Giấy Hoãn Tạm Ứng Nội Trú
* 20220901 #029 BLQ: Issue:2174 Chỉnh lại mẫu hình ảnh theo cách mới
* 20221013 #030 BLQ: Thêm mẫu giấy đề nghị mở thẻ
* 20221124 #031 BLQ: Thêm mẫu báo cáo lịch làm việc ngoài giờ
* 20230131 #032 QTD: Thêm Report toa thuốc GN_HT nội trú
* 20230109 #033 DatTB: Thêm phiếu tự khai và cam kết điều trị
* 20230314 #034 DatTB: Tách mẫu report kết quả xét nghiệm sử dụng chung mẫu CDHA
* 20230504 #035 DatTB: Thêm report Kết quả tính tuổi động mạch
* 20230515 #036 DatTB: Thêm thông tin bệnh viện report chi tiết KSK
* 20230603 #037 DatTB: Thêm chức năng song ngữ mẫu kết quả xét nghiệm
* 20230617 #038 DatTB: Thêm biết cấu hình tên BV viết tắt
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(ICommonPreviewView)), PartCreationPolicy(CreationPolicy.Shared)]
    public class CommonPreviewViewModel : Conductor<object>, ICommonPreviewView
    {
        [ImportingConstructor]
        public CommonPreviewViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {

        }
        private RemoteDocumentSource _reportModel;
        public RemoteDocumentSource ReportModel
        {
            get { return _reportModel; }
            set
            {
                _reportModel = value;
                NotifyOfPropertyChange(() => ReportModel);
            }
        }
        private long _ID = 0;
        public long ID
        {
            get { return _ID; }
            set
            {
                _ID = value;
                NotifyOfPropertyChange(() => ID);
            }
        }
        private long _PatientID = 0;
        public long PatientID
        {
            get { return _PatientID; }
            set
            {
                _PatientID = value;
                NotifyOfPropertyChange(() => PatientID);
            }
        }
        private string _PatientCode = "";
        public string PatientCode
        {
            get { return _PatientCode; }
            set
            {
                _PatientCode = value;
                NotifyOfPropertyChange(() => PatientCode);
            }
        }
        //▼====== #019
        private string _PatientFullAddress = "";
        public string PatientFullAddress
        {
            get { return _PatientFullAddress; }
            set
            {
                _PatientFullAddress = value;
                NotifyOfPropertyChange(() => PatientFullAddress);
            }
        }
        //▲====: #019

        //▼====== #022
        private string _PatientShortAddress = "";
        public string PatientShortAddress
        {
            get { return _PatientShortAddress; }
            set
            {
                _PatientShortAddress = value;
                NotifyOfPropertyChange(() => PatientShortAddress);
            }
        }
        //▲====== #022

        private long _registrationID = 0;
        public long RegistrationID
        {
            get { return _registrationID; }
            set
            {
                _registrationID = value;
                NotifyOfPropertyChange(() => RegistrationID);
            }
        }

        private long _registrationDetailID = 0;
        public long RegistrationDetailID
        {
            get { return _registrationDetailID; }
            set
            {
                _registrationDetailID = value;
                NotifyOfPropertyChange(() => RegistrationDetailID);
            }
        }
        private long _outPtTreatmentProgramID = 0;
        public long OutPtTreatmentProgramID
        {
            get { return _outPtTreatmentProgramID; }
            set
            {
                _outPtTreatmentProgramID = value;
                NotifyOfPropertyChange(() => OutPtTreatmentProgramID);
            }
        }
        private long _nutritionalRatingID = 0;
        public long NutritionalRatingID
        {
            get { return _nutritionalRatingID; }
            set
            {
                _nutritionalRatingID = value;
                NotifyOfPropertyChange(() => NutritionalRatingID);
            }
        }
        private long _V_RegistrationType = 24001;
        public long V_RegistrationType
        {
            get { return _V_RegistrationType; }
            set
            {
                _V_RegistrationType = value;
                NotifyOfPropertyChange(() => V_RegistrationType);
            }
        }

        private long _V_PCLRequestType = 25001;
        public long V_PCLRequestType
        {
            get { return _V_PCLRequestType; }
            set
            {
                _V_PCLRequestType = value;
                NotifyOfPropertyChange(() => V_PCLRequestType);
            }
        }

        private ReportName _eItem;
        public ReportName eItem
        {
            get
            {
                return _eItem;
            }
            set
            {
                _eItem = value;
                NotifyOfPropertyChange(() => eItem);
            }
        }

        private long _PatientPCLReqID = 0;
        public long PatientPCLReqID
        {
            get { return _PatientPCLReqID; }
            set
            {
                _PatientPCLReqID = value;
                NotifyOfPropertyChange(() => PatientPCLReqID);
            }
        }

        private WriteableBitmap imgHeartUltraRes1;
        public void SetHeartUltraImgageResult1(WriteableBitmap imgResult)
        {
            imgHeartUltraRes1 = imgResult;
        }

        private string _EchoCardioType1ImageResultFile1;
        public string EchoCardioType1ImageResultFile1
        {
            get { return _EchoCardioType1ImageResultFile1; }
            set
            {
                _EchoCardioType1ImageResultFile1 = value;
                NotifyOfPropertyChange(() => EchoCardioType1ImageResultFile1);
            }
        }

        private string _EchoCardioType1ImageResultFile2;
        public string EchoCardioType1ImageResultFile2
        {
            get { return _EchoCardioType1ImageResultFile2; }
            set
            {
                _EchoCardioType1ImageResultFile2 = value;
                NotifyOfPropertyChange(() => EchoCardioType1ImageResultFile2);
            }
        }

        private long _UltraResParam_EchoCardiographyID = 0;
        public long UltraResParam_EchoCardiographyID
        {
            get { return _UltraResParam_EchoCardiographyID; }
            set
            {
                _UltraResParam_EchoCardiographyID = value;
                NotifyOfPropertyChange(() => UltraResParam_EchoCardiographyID);
            }
        }

        private long _PaymentID = 0;
        public long PaymentID
        {
            get { return _PaymentID; }
            set
            {
                _PaymentID = value;
                NotifyOfPropertyChange(() => PaymentID);
            }
        }

        public int? ReceiptForEachLocationPrintingMode { get; set; }

        private int _FindPatient = 0;
        public int FindPatient
        {
            get { return _FindPatient; }
            set
            {
                _FindPatient = value;
                NotifyOfPropertyChange(() => FindPatient);
            }
        }
        //▼====== #010
        private bool _IsPsychotropicDrugs = false;
        public bool IsPsychotropicDrugs
        {
            get { return _IsPsychotropicDrugs; }
            set
            {
                _IsPsychotropicDrugs = value;
                NotifyOfPropertyChange(() => IsPsychotropicDrugs);
            }
        }

        private bool _IsFuncfoodsOrCosmetics = false;
        public bool IsFuncfoodsOrCosmetics
        {
            get { return _IsFuncfoodsOrCosmetics; }
            set
            {
                _IsFuncfoodsOrCosmetics = value;
                NotifyOfPropertyChange(() => IsFuncfoodsOrCosmetics);
            }
        }

        private bool _IsAddictive = false;
        public bool IsAddictive
        {
            get { return _IsAddictive; }
            set
            {
                _IsAddictive = value;
                NotifyOfPropertyChange(() => IsAddictive);
            }
        }
        //▲====== #010

        private bool _HasPharmacyDrug = false;
        public bool HasPharmacyDrug
        {
            get { return _HasPharmacyDrug; }
            set
            {
                _HasPharmacyDrug = value;
                NotifyOfPropertyChange(() => HasPharmacyDrug);
            }
        }

        private long _IssueID = 0;
        public long IssueID
        {
            get { return _IssueID; }
            set
            {
                _IssueID = value;
                NotifyOfPropertyChange(() => IssueID);
            }
        }

        private long? _ServiceRecID = 0;
        public long? ServiceRecID
        {
            get { return _ServiceRecID; }
            set
            {
                _ServiceRecID = value;
                NotifyOfPropertyChange(() => ServiceRecID);
            }
        }

        private bool? _IsAppointment = true;
        public bool? IsAppointment
        {
            get { return _IsAppointment; }
            set
            {
                _IsAppointment = value;
                NotifyOfPropertyChange(() => IsAppointment);
            }
        }

        private bool _ViewByDate;
        public bool ViewByDate
        {
            get { return _ViewByDate; }
            set
            {
                _ViewByDate = value;
                NotifyOfPropertyChange(() => ViewByDate);
            }
        }

        private long _RepPaymentRecvID;
        public long RepPaymentRecvID
        {
            get { return _RepPaymentRecvID; }
            set
            {
                _RepPaymentRecvID = value;
                NotifyOfPropertyChange(() => RepPaymentRecvID);
            }
        }

        //==== #003
        private long _TransferFormID;
        public long TransferFormID
        {
            get { return _TransferFormID; }
            set
            {
                _TransferFormID = value;
                NotifyOfPropertyChange(() => TransferFormID);
            }
        }
        /*TMA*/
        private int _TransferFormType;
        public int TransferFormType
        {
            get { return _TransferFormType; }
            set
            {
                _TransferFormType = value;
                NotifyOfPropertyChange(() => TransferFormType);
            }
        }

        private int _FindBy;
        public int FindBy
        {
            get { return _FindBy; }
            set
            {
                _FindBy = value;
                NotifyOfPropertyChange(() => FindBy);
            }
        }
        /*TMA*/
        //==== #003
        private string _StaffName;
        public string StaffName
        {
            get { return _StaffName; }
            set
            {
                _StaffName = value;
                NotifyOfPropertyChange(() => StaffName);
            }
        }

        private bool _HasDischarge = true;
        public bool HasDischarge
        {
            get { return _HasDischarge; }
            set
            {
                _HasDischarge = value;
                NotifyOfPropertyChange(() => HasDischarge);
            }
        }

        //▼====: #016
        private bool _IsDetails = false;
        public bool IsDetails
        {
            get { return _IsDetails; }
            set
            {
                _IsDetails = value;
                NotifyOfPropertyChange(() => IsDetails);
            }
        }
        //▲====: #016

        private ReceiptType _receiptType;

        public ReceiptType ReceiptType
        {
            get { return _receiptType; }
            set
            {
                _receiptType = value;
                NotifyOfPropertyChange(() => ReceiptType);
            }
        }

        //▼==== #037
        private bool _IsBilingual = false;
        public bool IsBilingual
        {
            get
            {
                return _IsBilingual;
            }
            set
            {
                _IsBilingual = value;
                NotifyOfPropertyChange(() => IsBilingual);
            }
        }
        //▲==== #037


        static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public List<long> RegistrationDetailIDList { get; set; }
        public List<long> PclRequestIDList { get; set; }

        public Int64 IntPtDiagDrInstructionID { get; set; }
        protected override void OnActivate()
        {
            string strTextParam;
            base.OnActivate();
            DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
            string reportDepartmentOfHealth = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
            string reportHospitalName = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
            string reportHospitalAddress = Globals.ServerConfigSection.CommonItems.ReportHospitalAddress;
            string reportLogoUrl = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
            string hospitalCode = Globals.ServerConfigSection.Hospitals.HospitalCode;
            int PrescriptionOutPtVersion = Globals.ServerConfigSection.ConsultationElements.PrescriptionOutPtVersion;
            int PrescriptionInPtVersion = Globals.ServerConfigSection.ConsultationElements.PrescriptionInPtVersion;
            string PrescriptionMainRightHeader = Globals.ServerConfigSection.Hospitals.PrescriptionMainRightHeader;
            string PrescriptionSubRightHeader = Globals.ServerConfigSection.Hospitals.PrescriptionSubRightHeader;
            //▼====: #020
            StaffPosition parHeadOfLaboratoryFullName = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.TRUONG_KHOA_XET_NGHIEM && x.IsActive).FirstOrDefault();
            //▲====: #020
            //▼==== #037
            string reportDepartmentOfHealthEng = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealthEng;
            string reportHospitalNameEng = Globals.ServerConfigSection.CommonItems.ReportHospitalNameEng;
            string reportHospitalAddressEng = Globals.ServerConfigSection.CommonItems.ReportHospitalAddressEng;
            //▲==== #037
            //▼==== #038
            string reportHospitalNameShort = Globals.ServerConfigSection.CommonItems.ReportHospitalNameShort;
            //▲==== #038
            switch (_eItem)
            {
                case ReportName.CONSULTATION_TOATHUOC_GN_HT:
                    {
                        ReportModel = null;
                        if (PrescriptionOutPtVersion == 6)
                        {
                            if (IsAddictive)
                            {
                                ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescription_V6_GN_SubReport").PreviewModel;
                                rParams["parIsAddictive"].Value = IsAddictive;
                                rParams["parHospitalPhone"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalPhone;

                            }
                            else if (IsPsychotropicDrugs)
                            {
                                ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescription_V6_HT_SubReport").PreviewModel;
                                rParams["parHospitalPhone"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalPhone;
                            }
                            else
                            {
                                if (Globals.ServerConfigSection.CommonItems.IsSeparatePrescription)
                                {
                                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescription_V6_SubReport_TV3").PreviewModel;
                                    rParams["parHasPharmacyDrug"].Value = HasPharmacyDrug;
                                    rParams["parHospitalPhone"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalPhone;
                                }
                                else
                                {
                                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescription_V6_SubReport").PreviewModel;
                                    rParams["parIsYHCTPrescript"].Value = IsYHCTPrescript;
                                    rParams["parHospitalPhone"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalPhone;
                                }
                            }
                            rParams["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                            rParams["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;
                            rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                            rParams["parHospitalAddress"].Value = reportHospitalAddress;
                            rParams["parHospitalName"].Value = reportHospitalName;
                        }
                        else if (PrescriptionOutPtVersion == 5)
                        {
                            if (IsAddictive)
                            {
                                ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescription_V5_GN_SubReport").PreviewModel;
                                rParams["parIsAddictive"].Value = IsAddictive;
                            }
                            else if (IsPsychotropicDrugs)
                            {
                                ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescription_V5_HT_SubReport").PreviewModel;
                            }
                            else
                            {
                                if (Globals.ServerConfigSection.CommonItems.IsSeparatePrescription)
                                {
                                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescription_V5_SubReport_TV3").PreviewModel;
                                    rParams["parHasPharmacyDrug"].Value = HasPharmacyDrug;
                                }
                                else
                                {
                                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescription_V5_SubReport").PreviewModel;
                                    rParams["parIsYHCTPrescript"].Value = IsYHCTPrescript;
                                }
                            }
                            rParams["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                            rParams["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;
                            rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                            rParams["parHospitalAddress"].Value = reportHospitalAddress;
                            rParams["parHospitalName"].Value = reportHospitalName;
                        }
                        else
                        if (PrescriptionOutPtVersion == 4)
                        {
                            if (IsAddictive)
                            {
                                ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescription_GN_SubReport_TV3").PreviewModel;
                                rParams["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                rParams["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;
                                rParams["parIsAddictive"].Value = IsAddictive;
                                //-- DuyNH 6-4-2021
                                rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                                rParams["parHospitalAddress"].Value = reportHospitalAddress;
                                rParams["parHospitalName"].Value = reportHospitalName;
                                //--
                            }
                            else if (IsPsychotropicDrugs)
                            {
                                ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescription_HT_SubReport_TV3").PreviewModel;
                                rParams["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                rParams["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;
                                //-- DuyNH 5-4-2021
                                rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                                rParams["parHospitalAddress"].Value = reportHospitalAddress;
                                rParams["parHospitalName"].Value = reportHospitalName;
                                //--

                            }
                            else
                            {
                                ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescription_V2_SubReport_TV4").PreviewModel;
                                rParams["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                rParams["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;

                                //-- DuyNH 4-5-2021
                                rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                                rParams["parHospitalAddress"].Value = reportHospitalAddress;
                                rParams["parHospitalName"].Value = reportHospitalName;
                                //--
                            }
                        }
                        else if (PrescriptionOutPtVersion == 3)
                        {
                            if (IsAddictive)
                            {
                                ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescription_GN_SubReport_TV3").PreviewModel;
                                rParams["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                rParams["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;
                                rParams["parIsAddictive"].Value = IsAddictive;
                                //-- DuyNH 6-4-2021
                                rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                                rParams["parHospitalAddress"].Value = reportHospitalAddress;
                                rParams["parHospitalName"].Value = reportHospitalName;
                                //--
                            }
                            else if (IsPsychotropicDrugs)
                            {
                                ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescription_HT_SubReport_TV3").PreviewModel;
                                rParams["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                rParams["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;
                                //-- DuyNH 5-4-2021
                                rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                                rParams["parHospitalAddress"].Value = reportHospitalAddress;
                                rParams["parHospitalName"].Value = reportHospitalName;
                                //--


                            }
                            else
                            {
                                ReportModel = new ConsultationEPrescriptionReportModel_TV3().PreviewModel;
                                rParams["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                rParams["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;
                            }
                        }
                        else if (PrescriptionOutPtVersion == 2)
                        {
                            ReportModel = new ConsultationEPrescriptionReportModelNew().PreviewModel;
                        }
                        else
                        {
                            ReportModel = new ConsultationEPrescriptionReportModel().PreviewModel;
                        }
                        //rParams["parKBYTLink"].Value = Globals.ServerConfigSection.CommonItems.KBYTLink;
                        rParams["parKBYTLink"].Value = Globals.ServerConfigSection.CommonItems.LinkKhaoSatNgoaiTru;
                        rParams["parIssueID"].Value = (int)IssueID;
                        rParams["parIsPsychotropicDrugs"].Value = IsPsychotropicDrugs;
                        rParams["parIsFuncfoodsOrCosmetics"].Value = IsFuncfoodsOrCosmetics;
                        if (PrescriptionOutPtVersion != 1)
                        {
                            rParams["parHospitalCode"].Value = Globals.ServerConfigSection.Hospitals.HospitalCode;
                        }
                        break;
                    }
                case ReportName.CONSULTATION_TOATHUOC:
                    {
                        ReportModel = null;
                        if ((int)parTypeOfForm == 1)
                        {
                            if (PrescriptionOutPtVersion == 6)
                            {
                                if (Globals.ServerConfigSection.CommonItems.IsSeparatePrescription)
                                {
                                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescription_V6_SubReport_TV3").PreviewModel;
                                    rParams["parHasPharmacyDrug"].Value = HasPharmacyDrug;
                                    rParams["parHospitalPhone"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalPhone;
                                }
                                else
                                {
                                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescription_V6_SubReport").PreviewModel;
                                    rParams["parIsYHCTPrescript"].Value = IsYHCTPrescript;
                                    rParams["parHospitalPhone"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalPhone;
                                }
                                rParams["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                rParams["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;
                              
                                //-- DuyNH 4-5-2021
                                rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                                rParams["parHospitalAddress"].Value = reportHospitalAddress;
                                rParams["parHospitalName"].Value = reportHospitalName;
                                //--

                            }
                            else if (PrescriptionOutPtVersion == 5)
                            {
                                if (Globals.ServerConfigSection.CommonItems.IsSeparatePrescription)
                                {
                                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescription_V5_SubReport_TV3").PreviewModel;
                                    rParams["parHasPharmacyDrug"].Value = HasPharmacyDrug;
                                }
                                else
                                {
                                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescription_V5_SubReport").PreviewModel;
                                    rParams["parIsYHCTPrescript"].Value = IsYHCTPrescript;
                                }
                                rParams["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                rParams["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;
                              
                                //-- DuyNH 4-5-2021
                                rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                                rParams["parHospitalAddress"].Value = reportHospitalAddress;
                                rParams["parHospitalName"].Value = reportHospitalName;
                                //--

                            }
                            else
                            if (PrescriptionOutPtVersion == 4)
                            {
                                ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescription_V2_SubReport_TV4").PreviewModel;
                                rParams["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                rParams["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;
                                rParams["parHasPharmacyDrug"].Value = HasPharmacyDrug;

                                //-- DuyNH 4-5-2021
                                rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                                rParams["parHospitalAddress"].Value = reportHospitalAddress;
                                rParams["parHospitalName"].Value = reportHospitalName;
                                //--

                            }
                            else if (PrescriptionOutPtVersion == 3)
                            {
                                //2022-03-03 BLQ Bỏ điều kiện gây nghiện hướng thần vì chay theo reportname rồi và 2 đoạn if else như nhau
                                //if (IsAddictive || IsPsychotropicDrugs)
                                //{
                                //    ReportModel = new ConsultationEPrescriptionReportModel_TV3().PreviewModel;
                                //    rParams["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                //    rParams["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;
                                    
                                //    //-- DuyNH 22-1-2021
                                //    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                                //    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                                //    rParams["parHospitalName"].Value = reportHospitalName;

                                //    //--
                                //}
                                //else
                                //{
                                    ReportModel = new ConsultationEPrescriptionReportModel_TV3().PreviewModel;
                                    rParams["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                    rParams["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;

                                    //-- DuyNH 22-1-2021
                                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                                    rParams["parHospitalName"].Value = reportHospitalName;

                                    //--
                                //}
                            }
                            else if (PrescriptionOutPtVersion == 2)
                            {
                                ReportModel = new ConsultationEPrescriptionReportModelNew().PreviewModel;
                            }
                            else
                            {
                                ReportModel = new ConsultationEPrescriptionReportModel().PreviewModel;
                            }
                        }
                        else if ((int)parTypeOfForm == 0)
                        {
                            if (PrescriptionOutPtVersion == 6)
                            {
                                if (Globals.ServerConfigSection.CommonItems.IsSeparatePrescription)
                                {
                                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescription_V6_SubReport_TV3").PreviewModel;
                                    rParams["parHasPharmacyDrug"].Value = HasPharmacyDrug;
                                    rParams["parHospitalPhone"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalPhone;
                                }
                                else
                                {
                                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescription_V6_SubReport").PreviewModel;
                                    rParams["parIsYHCTPrescript"].Value = IsYHCTPrescript;
                                    rParams["parHospitalPhone"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalPhone;
                                }
                                rParams["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                rParams["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;

                                //-- DuyNH 4-5-2021
                                rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                                rParams["parHospitalAddress"].Value = reportHospitalAddress;
                                rParams["parHospitalName"].Value = reportHospitalName;
                                //--

                            }
                            else if (PrescriptionOutPtVersion == 5)
                            {
                                if (Globals.ServerConfigSection.CommonItems.IsSeparatePrescription)
                                {
                                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescription_V5_SubReport_TV3").PreviewModel;
                                    rParams["parHasPharmacyDrug"].Value = HasPharmacyDrug;
                                }
                                else
                                {
                                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescription_V5_SubReport").PreviewModel;
                                    rParams["parIsYHCTPrescript"].Value = IsYHCTPrescript;
                                }
                                rParams["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                rParams["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;

                                //-- DuyNH 4-5-2021
                                rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                                rParams["parHospitalAddress"].Value = reportHospitalAddress;
                                rParams["parHospitalName"].Value = reportHospitalName;
                                //--

                            }
                            else
                            /*▼====: #005*/
                            //Thay thế ConsultationEPrescriptionReportModel thành ConsultationEPrescriptionReportModelNew
                            //ReportModel = new ConsultationEPrescriptionReportModel().PreviewModel;
                            if (PrescriptionOutPtVersion == 4)
                            {
                                ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescription_V2_SubReport_TV4").PreviewModel;
                                rParams["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                rParams["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;

                                //-- DuyNH 4-5-2021
                                rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                                rParams["parHospitalAddress"].Value = reportHospitalAddress;
                                rParams["parHospitalName"].Value = reportHospitalName;
                                //--


                            }
                            else if (PrescriptionOutPtVersion == 3)
                            {
                                ReportModel = new ConsultationEPrescriptionReportModel_TV3().PreviewModel;
                                rParams["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                rParams["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;

                                //-- DuyNH 22-22-2021
                                rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                                rParams["parHospitalAddress"].Value = reportHospitalAddress;
                                rParams["parHospitalName"].Value = reportHospitalName;

                                //--

                            }
                            else if (PrescriptionOutPtVersion == 2)
                            {
                                ReportModel = new ConsultationEPrescriptionReportModelNew().PreviewModel;
                            }
                            else
                            {
                                ReportModel = new ConsultationEPrescriptionReportModel().PreviewModel;
                            }
                            /*▲====: #005*/
                        }
                        //rParams["parKBYTLink"].Value = Globals.ServerConfigSection.CommonItems.KBYTLink;
                        rParams["parKBYTLink"].Value = Globals.ServerConfigSection.CommonItems.LinkKhaoSatNgoaiTru;
                        rParams["parIssueID"].Value = (int)IssueID;
                        //▼====== #010
                        rParams["parIsPsychotropicDrugs"].Value = IsPsychotropicDrugs;
                        rParams["parIsFuncfoodsOrCosmetics"].Value = IsFuncfoodsOrCosmetics;
                        if (PrescriptionOutPtVersion != 1)
                        {
                            rParams["parHospitalCode"].Value = Globals.ServerConfigSection.Hospitals.HospitalCode;
                        }
                        //▲====== #010
                        break;
                    }
                case ReportName.CONSULTATION_TOATHUOC_INPT:
                    {
                        ReportModel = null;
                        if ((int)parTypeOfForm == 1)
                        {
                            if (PrescriptionInPtVersion == 5)
                            {
                                ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescriptionInpt_V5_SubReport").PreviewModel;
                                rParams["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                rParams["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;

                                //-- DuyNH 22-1-2021
                                rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                                rParams["parHospitalAddress"].Value = reportHospitalAddress;
                                rParams["parHospitalName"].Value = reportHospitalName;
                                rParams["parHospitalPhone"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalPhone;
                                //--
                            }
                            else if (PrescriptionInPtVersion == 4)
                            {
                                ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescriptionInpt_V4_SubReport").PreviewModel;
                                rParams["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                rParams["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;

                                //-- DuyNH 22-1-2021
                                rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                                rParams["parHospitalAddress"].Value = reportHospitalAddress;
                                rParams["parHospitalName"].Value = reportHospitalName;

                                //--
                            }
                            else
                            if (PrescriptionInPtVersion == 3)
                            {
                                ReportModel = new ConsultationEPrescription_InPtReportModel_TV3().PreviewModel;
                                rParams["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                rParams["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;

                                //-- DuyNH 22-1-2021
                                rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                                rParams["parHospitalAddress"].Value = reportHospitalAddress;
                                rParams["parHospitalName"].Value = reportHospitalName;

                                //--
                            }
                            else if (PrescriptionInPtVersion == 2)
                            {
                                ReportModel = new ConsultationEPrescription_InPtReportModelNew().PreviewModel;
                            }
                            else
                            {
                                ReportModel = new ConsultationEPrescription_InPtReportModel().PreviewModel;
                            }
                        }
                        else if ((int)parTypeOfForm == 0)
                        {
                            if (PrescriptionInPtVersion == 5)
                            {
                                ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescriptionInpt_V5_SubReport").PreviewModel;
                                rParams["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                rParams["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;

                                //-- DuyNH 22-1-2021
                                rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                                rParams["parHospitalAddress"].Value = reportHospitalAddress;
                                rParams["parHospitalName"].Value = reportHospitalName;
                                rParams["parHospitalPhone"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalPhone;
                                //--
                            }
                            else if (PrescriptionInPtVersion == 4)
                            {
                                ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescriptionInpt_V4_SubReport").PreviewModel;
                                rParams["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                rParams["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;

                                //-- DuyNH 22-1-2021
                                rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                                rParams["parHospitalAddress"].Value = reportHospitalAddress;
                                rParams["parHospitalName"].Value = reportHospitalName;

                                //--
                            }
                            else
                              if (PrescriptionInPtVersion == 3)
                            {
                                ReportModel = new ConsultationEPrescription_InPtReportModel_TV3().PreviewModel;
                                rParams["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                                rParams["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;

                                //-- DuyNH 22-1-2021
                                rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                                rParams["parHospitalAddress"].Value = reportHospitalAddress;
                                rParams["parHospitalName"].Value = reportHospitalName;

                                //--


                            }
                            else if (PrescriptionInPtVersion == 2)
                            {
                                ReportModel = new ConsultationEPrescription_InPtReportModelNew().PreviewModel;
                            }
                            else
                            {
                                ReportModel = new ConsultationEPrescription_InPtReportModel().PreviewModel;
                            }
                        }
                        //rParams["parKBYTLink"].Value = Globals.ServerConfigSection.CommonItems.KBYTLink;
                        rParams["parKBYTLink"].Value = Globals.ServerConfigSection.CommonItems.LinkKhaoSatNoiTru;
                        rParams["parIssueID"].Value = (int)IssueID;
                        if (PrescriptionOutPtVersion != 1)
                        {
                            rParams["parHospitalCode"].Value = Globals.ServerConfigSection.Hospitals.HospitalCode;
                        }
                        break;
                    }
                //▼====: #032
                case ReportName.CONSULTATION_TOATHUOC_INPT_GN_HT:
                    {
                        ReportModel = null;
                        if (parTypeOfForm == 1 && PrescriptionInPtVersion == 5)
                        {
                            if (IsAddictive)
                            {
                                ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescriptionInpt_V5_GN_SubReport").PreviewModel;
                                rParams["parIsAddictive"].Value = IsAddictive;
                            }
                            else
                            {
                                ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescriptionInpt_V5_HT_SubReport").PreviewModel;
                            }
                            rParams["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                            rParams["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;

                            //-- DuyNH 22-1-2021
                            rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                            rParams["parHospitalAddress"].Value = reportHospitalAddress;
                            rParams["parHospitalName"].Value = reportHospitalName;
                            rParams["parHospitalPhone"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalPhone;
                            //--
                        }
                        else {
                            return;
                        }
                        //rParams["parKBYTLink"].Value = Globals.ServerConfigSection.CommonItems.KBYTLink;
                        rParams["parKBYTLink"].Value = Globals.ServerConfigSection.CommonItems.LinkKhaoSatNoiTru;
                        rParams["parIssueID"].Value = (int)IssueID;
                        if (PrescriptionOutPtVersion != 1)
                        {
                            rParams["parHospitalCode"].Value = Globals.ServerConfigSection.Hospitals.HospitalCode;
                        }
                        break;
                    }
                //▲====: #032
                case ReportName.CONSULTATION_TOATHUOC_PRIVATE:
                    {
                        ReportModel = null;
                        ReportModel = new ConsultationEPrescriptionPrivateReportModel().PreviewModel;
                        rParams["parIssueID"].Value = (int)IssueID;
                        break;
                    }
                // 20181024 TNHX: [BM0003200] Apply PhieuXetNghiem for Thanh Vu Hospital
                // 20181216 TNHX: [BM0005430] Add PatientID for report PCLDEPARTMENT_LABORATORY_RESULT
                
                case ReportName.PCLDEPARTMENT_LABORATORY_RESULT:
                    {
                        //ReportModel = null;
                        //ReportModel = new PCLDepartmentLaboratoryResultReportModel().PreviewModel;
                        //rParams["parPatientPCLReqID"].Value = (int)PatientPCLReqID;
                        //rParams["parV_PCLRequestType"].Value = (int)V_PCLRequestType;
                        //rParams["parPatientFindBy"].Value = (int)FindPatient;
                        //break;
                        ReportModel = null;
                        if (Globals.ServerConfigSection.ConsultationElements.LaboratoryResultVersion == 3)
                        {
                            ReportModel = new PCLDepartmentLaboratoryResultReportModel_TV3().PreviewModel;
                            rParams["parPrescriptionMainRightHeader"].Value = PrescriptionMainRightHeader;
                            rParams["parPrescriptionSubRightHeader"].Value = PrescriptionSubRightHeader;
                            //▼====: #020
                            rParams["parHeadOfLaboratoryFullName"].Value = parHeadOfLaboratoryFullName != null ? parHeadOfLaboratoryFullName.FullNameString : "";
                            rParams["parHeadOfLaboratoryPositionName"].Value = parHeadOfLaboratoryFullName != null ? parHeadOfLaboratoryFullName.PositionName : "";
                            //▲====: #020

                            //▼====: 2021 - 01 - 25 DuyNH
                            rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                            rParams["parHospitalAddress"].Value = reportHospitalAddress;
                            rParams["parHospitalName"].Value = reportHospitalName;
                            rParams["parHospitalCode"].Value = hospitalCode;

                            //▲====: 
                            //▼==== #037
                            rParams["parDepartmentOfHealthEng"].Value = reportDepartmentOfHealthEng;
                            rParams["parHospitalAddressEng"].Value = reportHospitalAddressEng;
                            rParams["parHospitalNameEng"].Value = reportHospitalNameEng;
                            //▲==== #037
                        }
                        else if (Globals.ServerConfigSection.ConsultationElements.LaboratoryResultVersion == 2)
                        {
                            ReportModel = new PCLDepartmentLaboratoryResultReportModel_TV().PreviewModel;
                            //▼====: #020
                            rParams["parHeadOfLaboratoryFullName"].Value = parHeadOfLaboratoryFullName != null ? parHeadOfLaboratoryFullName.FullNameString : "";
                            rParams["parHeadOfLaboratoryPositionName"].Value = parHeadOfLaboratoryFullName != null ? parHeadOfLaboratoryFullName.PositionName : "";
                            //▲====: #020
                        }
                        else
                        {
                            ReportModel = new PCLDepartmentLaboratoryResultReportModel().PreviewModel;
                            rParams["parPatientPCLReqID"].Value = (int)PatientPCLReqID;
                            rParams["parV_PCLRequestType"].Value = (int)V_PCLRequestType;
                            rParams["parPatientFindBy"].Value = (int)FindPatient;
                            break;
                        }
                        rParams["parPatientID"].Value = (int)PatientID;
                        rParams["parPatientPCLReqID"].Value = (int)PatientPCLReqID;
                        rParams["parV_PCLRequestType"].Value = (int)V_PCLRequestType;
                        rParams["parPatientFindBy"].Value = (int)FindPatient;
                        rParams["parStaffName"].Value = StaffName;
                        //▼==== #037
                        rParams["IsBilingual"].Value = IsBilingual;
                        //▲==== #037

                        //rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                        //rParams["parHospitalAddress"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalAddress;
                        //rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                        //rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
                        break;
                    }
                case ReportName.PCLDEPARTMENT_IMAGERESULT_HEART_ULTRASOUND:
                    {
                        ReportModel = null;
                        ReportModel = new PCLDeptImageResult_HeartUltraSoundType1ReportModel().PreviewModel;

                        //var bmpEncoder = new BmpEncoder();
                        //var imgMemStream = new MemoryStream();
                        //bmpEncoder.Encode(ImageExtensions.ToImage(imgHeartUltraRes1), imgMemStream);
                        //rParams["paramImage1"].Value = GetString(imgMemStream.ToArray());

                        rParams["paramImage1"].Value = "";
                        if (EchoCardioType1ImageResultFile1 != null && EchoCardioType1ImageResultFile1.Length > 0)
                        {
                            rParams["paramImage1"].Value = EchoCardioType1ImageResultFile1;
                        }

                        rParams["paramImage2"].Value = "";
                        if (EchoCardioType1ImageResultFile2 != null && EchoCardioType1ImageResultFile2.Length > 0)
                        {
                            rParams["paramImage2"].Value = EchoCardioType1ImageResultFile2;
                        }

                        rParams["paramPatientPCLReqID"].Value = (int)PatientPCLReqID;
                        /*▼====: #007*/
                        rParams["paramUltraResParams_EchoCardiographyID"].Value = (int)UltraResParam_EchoCardiographyID;
                        /*▲====: #007*/
                        if (Globals.ServerConfigSection.CommonItems.OrganizationUseSoftware == 0)
                        {
                            // Hospital 
                            rParams["paramHospitalOrPC"].Value = 1;
                        }
                        else
                        {
                            // PC
                            rParams["paramHospitalOrPC"].Value = 2;
                            rParams["paramOrganizationName"].Value = Globals.ServerConfigSection.CommonItems.OrganizationName;
                            rParams["paramOrganizationAddress"].Value = Globals.ServerConfigSection.CommonItems.OrganizationAddress;
                            rParams["paramOrganizationNotes"].Value = Globals.ServerConfigSection.CommonItems.OrganizationNotes;
                        }
                        /*==== #001 ====*/
                        rParams["PatientType"].Value = FindPatient == 1;
                        /*==== #001 ====*/
                        break;
                    }
                case ReportName.ABDOMINAL_ULTRASOUND_RESULT:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRptAbdominalUltrasoundResult").PreviewModel;

                        rParams["paramImage1"].Value = "";
                        if (EchoCardioType1ImageResultFile1 != null && EchoCardioType1ImageResultFile1.Length > 0)
                        {
                            rParams["paramImage1"].Value = EchoCardioType1ImageResultFile1;
                        }

                        rParams["paramImage2"].Value = "";
                        if (EchoCardioType1ImageResultFile2 != null && EchoCardioType1ImageResultFile2.Length > 0)
                        {
                            rParams["paramImage2"].Value = EchoCardioType1ImageResultFile2;
                        }

                        rParams["paramPatientPCLReqID"].Value = (int)PatientPCLReqID;

                        rParams["paramOrganizationName"].Value = Globals.ServerConfigSection.CommonItems.OrganizationName;
                        rParams["paramOrganizationAddress"].Value = Globals.ServerConfigSection.CommonItems.OrganizationAddress;
                        rParams["paramOrganizationNotes"].Value = Globals.ServerConfigSection.CommonItems.OrganizationNotes;

                        break;
                    }
                case ReportName.DiagnosisTreatmentByDoctorStaffID:
                    {
                        ReportModel = null;
                        ReportModel = new DiagnosisTreatmentByDoctorStaffID().PreviewModel;
                        rParams["parDoctorStaffID"].Value = (int)DoctorStaffID;
                        rParams["parFromDate"].Value = FromDate;
                        rParams["parToDate"].Value = ToDate;
                        rParams["parLogoUrl"].Value = reportLogoUrl;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        break;
                    }
                case ReportName.CONSULTATION_BANGKECHITIETKHAM:
                    {
                        ReportModel = null;
                        ReportModel = new ConsultationRoomDetailsReportModel().PreviewModel;
                        //ReportModel = new BangKeChiTietKB().PreviewModel;
                        rParams["DeptLocID"].Value = (int)DeptID;
                        rParams["BeginDate"].Value = FromDate;
                        rParams["EndDate"].Value = ToDate;
                        rParams["StaffID"].Value = (int)DoctorStaffID;
                        rParams["parLogoUrl"].Value = reportLogoUrl;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        break;
                    }
                case ReportName.AllDiagnosisTreatmentGroupByDoctorStaffID:
                    {
                        ReportModel = null;
                        ReportModel = new AllDiagnosisGroupByDoctorStaffIDDeptLocationID().PreviewModel;
                        rParams["parDoctorStaffID"].Value = 0;
                        rParams["parFromDate"].Value = FromDate;
                        rParams["parToDate"].Value = ToDate;
                        rParams["parLogoUrl"].Value = reportLogoUrl;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        break;
                    }
                case ReportName.RptBangGiaThuocKhoaDuoc_AutoCreate:
                    {
                        ReportModel = null;
                        ReportModel = new RptBangGiaThuocYCuHoaChatKhoaDuoc_AutoCreate().PreviewModel;
                        rParams["parTieuDeRpt"].Value = TieuDeRpt;
                        rParams["parV_MedProductType"].Value = (int)V_MedProductType;
                        rParams["parResult"].Value = Result;
                        break;
                    }

                case ReportName.RptBangGiaThuocKhoaDuoc:
                    {
                        ReportModel = null;
                        ReportModel = new RptBangGiaThuocKhoaDuoc().PreviewModel;
                        rParams["parTieuDeRpt"].Value = TieuDeRpt;
                        rParams["parDrugDeptSellingPriceListID"].Value = (int)DrugDeptSellingPriceListID;
                        rParams["parLogoUrl"].Value = reportLogoUrl;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        break;
                    }
                case ReportName.RptBangGiaYCuHoaChatKhoaDuoc_AutoCreate:
                    {
                        ReportModel = null;
                        ReportModel = new RptBangGiaYCuHoaChatKhoaDuoc_AutoCreate().PreviewModel;
                        rParams["parTieuDeRpt"].Value = TieuDeRpt;
                        rParams["parTenYCuHoaChat"].Value = TenYCuHoaChat;
                        rParams["parTenYCuHoaChatTiengViet"].Value = TenYCuHoaChatTiengViet;
                        rParams["parV_MedProductType"].Value = (int)V_MedProductType;
                        rParams["parResult"].Value = Result;
                        break;
                    }
                case ReportName.RptBangGiaYCuHoaChatKhoaDuoc:
                    {
                        ReportModel = null;
                        ReportModel = new RptBangGiaYCuHoaChatKhoaDuoc().PreviewModel;
                        rParams["parTieuDeRpt"].Value = TieuDeRpt;
                        rParams["parTenYCuHoaChat"].Value = TenYCuHoaChat;
                        rParams["parTenYCuHoaChatTiengViet"].Value = TenYCuHoaChatTiengViet;
                        rParams["parDrugDeptSellingPriceListID"].Value = (int)DrugDeptSellingPriceListID;
                        break;
                    }

                //Nhà Thuốc
                case ReportName.RptBangGiaThuocNhaThuoc:
                    {
                        ReportModel = null;
                        ReportModel = new RptBangGiaThuocNhaThuoc().PreviewModel;
                        rParams["parTieuDeRpt"].Value = TieuDeRpt;
                        rParams["parPharmacySellingPriceListID"].Value = (int)PharmacySellingPriceListID;
                        break;
                    }
                case ReportName.XRptPharmacySellingPriceList_Detail_Simple:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPharmacies.XtraReports.Managers.PharmacySellingPriceList.XRptPharmacySellingPriceList_Detail_Simple").PreviewModel;
                        rParams["parTieuDeRpt"].Value = TieuDeRpt;
                        rParams["parPharmacySellingPriceListID"].Value = (int)PharmacySellingPriceListID;
                        rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                        rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
                        break;
                    }
                case ReportName.RptBangGiaThuocNhaThuoc_AutoCreate:
                    {
                        ReportModel = null;
                        ReportModel = new RptBangGiaThuocNhaThuoc_AutoCreate().PreviewModel;
                        rParams["parTieuDeRpt"].Value = TieuDeRpt;
                        rParams["parResult"].Value = Result;
                        break;
                    }
                /*▼====: #009*/
                case ReportName.RptPatientPCLRequestDetailsByPatientPCLReqID:
                    {
                        //ReportModel = null;
                        //ReportModel = new RptPatientPCLRequestDetailsByPatientPCLReqID().PreviewModel;
                        //rParams["parPatientPCLReqID"].Value = (int)PatientPCLReqID;
                        //rParams["paramV_RegistrationType"].Value = (int)V_RegistrationType;
                        //break;
                        ReportModel = null;
                        ReportModel = new RptPatientPCLRequestDetailsByPatientPCLReqID_TV().PreviewModel;
                        rParams["parPatientPCLReqID"].Value = (int)PatientPCLReqID;
                        rParams["paramV_RegistrationType"].Value = (int)V_RegistrationType;
                        rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
                        rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                        break;
                    }
                /*▲====: #009*/
                case ReportName.RptPatientPCLRequestDetailsByPatientPCLReqID_TV_XML:
                    {
                        ReportModel = null;
                        ReportModel = new RptPatientPCLRequestDetailsByPatientPCLReqID_TV_XML().PreviewModel;
                        rParams["parPatientPCLReqID"].Value = Result;
                        rParams["paramV_RegistrationType"].Value = (int)V_RegistrationType;
                        rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
                        rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                        break;
                    }
                case ReportName.PatientPCLRequestDetailsByPatientPCLReqID_XML:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XRptPatientPCLRequestDetailsByPatientPCLReqID_XML_TV").PreviewModel;
                        rParams["parPtRegistrationID"].Value = (int)RegistrationID;
                        rParams["parPCLReqID_XML"].Value = Result;
                        rParams["parTitle"].Value = TieuDeRpt;
                        rParams["paramV_RegistrationType"].Value = (int)V_RegistrationType;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parLogoUrl"].Value = reportLogoUrl;
                        break;
                    }
                case ReportName.RptTongHopDoanhThu:
                    {
                        ReportModel = null;
                        ReportModel = new TransactionsReportModel().PreviewModel;
                        rParams["parTieuDeRpt"].Value = TieuDeRpt;
                        rParams["parFromDate"].Value = FromDate;
                        rParams["parToDate"].Value = ToDate;
                        rParams["paraDeptID"].Value = (int)DeptID.GetValueOrDefault();
                        rParams["paraDeptLocID"].Value = (int)DeptLocID.GetValueOrDefault();
                        rParams["paraDeptName"].Value = DeptName;
                        rParams["paraLocationName"].Value = LocationName;
                        rParams["parLogoUrl"].Value = reportLogoUrl;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        break;
                    }
                case ReportName.RptTongHopDoanhThuNoiTru:
                    {
                        //▼====: #016
                        ReportModel = null;
                        //ReportModel = new RptTongHopDoanhThuNoiTruReportModel().PreviewModel;
                        //rParams["pStartDate"].Value = FromDate;
                        //rParams["pEndDate"].Value = ToDate;
                        //rParams["pDeptID"].Value = (int)DeptID;
                        //rParams["parLogoUrl"].Value = reportLogoUrl;
                        //rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        ReportModel = new RptTongHopDoanhThuTheoKhoaReportModel().PreviewModel;
                        rParams["parFromDate"].Value = FromDate;
                        rParams["parToDate"].Value = ToDate;
                        rParams["parDeptID"].Value = DeptID;
                        rParams["parDeptName"].Value = DeptName;
                        rParams["parIsDetail"].Value = IsDetails;
                        rParams["parHasDischarge"].Value = HasDischarge;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        //▲====: #016
                        break;
                    }
                //▼====: #011
                case ReportName.RptHoatDongQuayDK:
                    {
                        ReportModel = null;
                        ReportModel = new RptHoatDongPhongDangKyReportModel().PreviewModel;
                        rParams["parTieuDeRpt"].Value = TieuDeRpt;
                        rParams["parFromDate"].Value = FromDate;
                        rParams["parToDate"].Value = ToDate;
                        rParams["parDeptLocID"].Value = (int)DeptLocID;
                        rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                        rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
                        break;
                    }
                //▲====: #011
                case ReportName.RptThuTienHangNgay:
                    {
                        ReportModel = null;
                        ReportModel = new ReceiveByStaffReportModel().PreviewModel;
                        rParams["RepPaymentRecvID"].Value = (int)ID;
                        rParams["parLogoUrl"].Value = reportLogoUrl;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;

                        break;
                    }
                case ReportName.BAOCAONHANH_KHUKHAMBENH:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptBaoCaoNhanhPhongKham").PreviewModel;
                        rParams["FromDate"].Value = _FromDate;
                        rParams["ToDate"].Value = _ToDate;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        break;
                    }

                case ReportName.REGISTRATION_OUT_PATIENT_RECEIPT:
                    {
                        ReportModel = null;
                        ReportModel = new OutPatientReceiptReportModel().PreviewModel;
                        rParams["param_PaymentID"].Value = (int)PaymentID;

                        break;
                    }
                case ReportName.REGISTRATION_OUT_PATIENT_RECEIPT_XML:
                    {
                        ReportModel = null;
                        ReportModel = new OutPatientReceiptReportXMLModel().PreviewModel;
                        rParams["param_PaymentID"].Value = Result;
                        break;
                    }
                case ReportName.REGISTRATION_OUT_PATIENT_RECEIPT_XML_V2:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptOutPatientReceiptXML_V2").PreviewModel;
                        rParams["param_PaymentID"].Value = Result;
                        break;
                    }
                case ReportName.REGISTRATION_OUT_PATIENT_RECEIPT_XML_V4:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptOutPatientReceiptXML_V4").PreviewModel;
                        rParams["param_PaymentID"].Value = Result;
                        // 20181027 TNHX: [BM0002176] Add parameter to show HospitalName & DepartmentOfHealth
                        rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                        rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
                        rParams["parLogoUrl"].Value = reportLogoUrl;
                        rParams["parDeptLocIDQMS"].Value = Globals.ServerConfigSection.CommonItems.FloorDeptLocation_0 +
                                Globals.ServerConfigSection.CommonItems.FloorDeptLocation_1 +
                                Globals.ServerConfigSection.CommonItems.FloorDeptLocation_2;
                        break;
                    }
                case ReportName.REGISTRATION_OUT_PATIENT_HI_CONFIRMATION:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.BHYT_NgoaiTru").PreviewModel;
                        rParams["param_RegistrationID"].Value = (int)RegistrationID;
                        rParams["parFullName"].Value = Globals.LoggedUserAccount.Staff.FullName;
                        rParams["parServiceRecID"].Value = (int)ServiceRecID.GetValueOrDefault(0);
                        rParams["parHospitalName"].Value = reportHospitalName;
                        break;
                    }

                case ReportName.REGISTRATION_HI_APPOINTMENT:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.GiayHenTaiKhamBHYT").PreviewModel;
                        rParams["param_RegistrationID"].Value = (int)RegistrationID;
                        rParams["param_AppointmentID"].Value = (int)AppointmentID;
                        rParams["parServiceRecID"].Value = (int)ServiceRecID.GetValueOrDefault(0);
                        rParams["parHospitalName"].Value = reportHospitalName;
                        break;
                    }
                case ReportName.REGISTRATION_HI_APPOINTMENT_INPT:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.GiayHenTaiKhamBHYT_InPt").PreviewModel;
                        rParams["param_RegistrationID"].Value = (int)RegistrationID;
                        rParams["param_AppointmentID"].Value = (int)AppointmentID;
                        rParams["parServiceRecID"].Value = (int)ServiceRecID.GetValueOrDefault(0);
                        break;
                    }
                case ReportName.HI_APPOINTMENT:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptHIAppointment").PreviewModel;
                        rParams["param_AppointmentID"].Value = (int)AppointmentID;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        break;
                    }
                case ReportName.REGISTRATION_IN_PATIENT_HI_CONFIRMATION:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.BHYT_NoiTru").PreviewModel;
                        rParams["param_RegistrationID"].Value = (int)RegistrationID;
                        rParams["parFullName"].Value = Globals.LoggedUserAccount.Staff.FullName;
                        rParams["parServiceRecID"].Value = (int)ServiceRecID.GetValueOrDefault(0);
                        rParams["parHospitalName"].Value = reportHospitalName;
                        break;
                    }

                case ReportName.REGISTRATION_CASH_ADVANCE_BILL_INPT:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptCashAdvanceBill").PreviewModel;
                        rParams["parPtCashAdvanceID"].Value = (int)ID;
                        break;
                    }

                case ReportName.RptPatientApptServiceDetails:
                    {
                        ReportModel = null;
                        //ReportModel = new GenericReportModel("eHCMS.ReportLib.RptAppointment.XRptPatientApptServiceDetails").PreviewModel;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptAppointment.XRptPatientApptServiceDetails_TV").PreviewModel;
                        rParams["parAppointmentID"].Value = (int)AppointmentID;
                        rParams["parLogoUrl"].Value = reportLogoUrl;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        break;
                    }
                case ReportName.RptPatientApptPCLRequests:
                    {
                        ReportModel = null;
                        //ReportModel = new GenericReportModel("eHCMS.ReportLib.RptAppointment.XRptPatientApptPCLRequestsCombo").PreviewModel;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptAppointment.XRptPatientApptPCLRequestsCombo_TV").PreviewModel;
                        rParams["parAppointmentID"].Value = (int)AppointmentID;
                        //rParams["parPatientPCLReqID"].Value = (int)PatientPCLReqID;
                        rParams["parPtPCLReqID_List"].Value = (string)strIDList;
                        rParams["parLogoUrl"].Value = reportLogoUrl;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        break;
                    }
                case ReportName.RptPatientApptPCLRequests_XML:
                    {
                        ReportModel = null;
                        //ReportModel = new GenericReportModel("eHCMS.ReportLib.RptAppointment.XRptPatientApptPCLRequestsXMLNew").PreviewModel;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptAppointment.XRptPatientApptPCLRequestsXMLNew_TV").PreviewModel;
                        rParams["parRequestXML"].Value = Result;
                        rParams["parLogoUrl"].Value = reportLogoUrl;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        break;
                    }
                case ReportName.PATIENTCASHADVANCE_REPORT:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptPatientCashAdvance").PreviewModel;
                        rParams["param_PaymentID"].Value = (int)PaymentID;
                        rParams["FindPatient"].Value = FindPatient;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        break;
                    }
                case ReportName.INPATIENT_SETTLEMENT:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptPatientSettlement").PreviewModel;
                        rParams["param_ID"].Value = (int)ID;
                        rParams["param_flag"].Value = (int)flag;
                        break;
                    }
                case ReportName.INPATIENT_SETTLEMENT_V2:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptPatientSettlement_V2").PreviewModel;
                        rParams["param_ID"].Value = (int)ID;
                        rParams["param_flag"].Value = (int)flag;
                        break;
                    }
                case ReportName.INPATIENT_SETTLEMENT_V4:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptPatientSettlement_V4").PreviewModel;
                        rParams["param_ID"].Value = (int)ID;
                        rParams["param_flag"].Value = (int)flag;
                        rParams["parReceiptType"].Value = (int)ReceiptType;
                        break;
                    }
                case ReportName.REGISTRATION_IN_PT_REPAYCASHADVANCE:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptRepayCashAdvance").PreviewModel;
                        rParams["param_PaymentID"].Value = (int)PaymentID;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        break;
                    }
                case ReportName.BAOCAOCHITIETTHANHTOAN:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.BaoCaoChiTietThanhToan").PreviewModel;
                        rParams["PtRegistrationID"].Value = (int)RegistrationID;//thuc ra la PtRegistrationID do
                        rParams["FindPatient"].Value = FindPatient;
                        rParams["parLogoUrl"].Value = reportLogoUrl;

                        break;
                    }
                case ReportName.PHIEUDENGHITAMUNG:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptPhieuDeNghiTamUng").PreviewModel;
                        rParams["param_RptPtCashAdvRemID"].Value = (int)ID;//thuc ra la PtRegistrationID do
                        break;
                    }
                case ReportName.PHIEUDENGHI_THANHTOAN:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptPhieuDeNghiThanhToan").PreviewModel;
                        rParams["param_RptPtCashAdvRemID"].Value = (int)ID;//thuc ra la PtRegistrationID do
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        break;
                    }
                case ReportName.REGISTRATION_HUY_HOADON:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptReportCancelInvoice").PreviewModel;
                        rParams["RepPaymentRecvID"].Value = (int)ID;//thuc ra la PtRegistrationID do
                        rParams["parHospitalName"].Value = reportHospitalName;
                        break;
                    }
                case ReportName.REGISTRATION_HUY_HOADON_CHITIET:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptReportCancelInvoiceDetails").PreviewModel;
                        rParams["RepPaymentRecvID"].Value = (int)ID;//thuc ra la PtRegistrationID do
                        rParams["parHospitalName"].Value = reportHospitalName;
                        break;
                    }
                case ReportName.TEMP38a:
                    ReportModel = null;
                    ReportModel = new TransactionsTemplate38().PreviewModel;
                    rParams["TransactionID"].Value = 0;
                    rParams["PtRegistrationID"].Value = (int)ID;
                    rParams["parTypeOfForm"].Value = (int)parTypeOfForm;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    break;
                case ReportName.TEMP38aNoiTru:
                    ReportModel = null;
                    ReportModel = new TransactionsTemplate38NoiTru().PreviewModel;
                    rParams["PtRegistrationID"].Value = (int)ID;
                    rParams["FromDate"].Value = FromDate.GetValueOrDefault();
                    rParams["ToDate"].Value = ToDate.GetValueOrDefault();
                    rParams["ViewByDate"].Value = ViewByDate;
                    rParams["DeptID"].Value = (int)DeptID;
                    rParams["DeptName"].Value = DeptName;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    break;
                case ReportName.FORM_02_NoiTru:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_Form02NoiTru").PreviewModel;
                    rParams["PtRegistrationID"].Value = (int)ID;
                    rParams["RptForm02_InPtIDList"].Value = strIDList;
                    rParams["DeptName"].Value = DeptName;
                    break;

                case ReportName.REGISTRATIOBLIST:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptRegistrationList").PreviewModel;
                    rParams["parStaffID"].Value = (int)SeachRegistrationCriteria.StaffID.GetValueOrDefault(0);
                    rParams["parFromDate"].Value = SeachRegistrationCriteria.FromDate;
                    rParams["parToDate"].Value = SeachRegistrationCriteria.ToDate;
                    rParams["parDeptID"].Value = (int)SeachRegistrationCriteria.DeptID.GetValueOrDefault(0);
                    rParams["parDeptLocationID"].Value = (int)SeachRegistrationCriteria.DeptLocationID.GetValueOrDefault(0);
                    rParams["parKhamBenh"].Value = SeachRegistrationCriteria.KhamBenh;
                    rParams["parPatientFindBy"].Value = (int)SeachRegistrationCriteria.PatientFindBy;
                    rParams["parIsCancel"].Value = (bool)SeachRegistrationCriteria.IsCancel;
                    rParams["parTypeSearch"].Value = (int)SeachRegistrationCriteria.TypeSearch;
                    break;

                case ReportName.REGISTRATIONDETAILLIST:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptRegistrationDetailList").PreviewModel;
                    rParams["parStaffID"].Value = (int)SeachRegistrationCriteria.StaffID.GetValueOrDefault(0);
                    rParams["parFromDate"].Value = SeachRegistrationCriteria.FromDate;
                    rParams["parToDate"].Value = SeachRegistrationCriteria.ToDate;
                    rParams["parDeptID"].Value = (int)SeachRegistrationCriteria.DeptID.GetValueOrDefault(0);
                    rParams["parDeptLocationID"].Value = (int)SeachRegistrationCriteria.DeptLocationID.GetValueOrDefault(0);
                    rParams["parKhamBenh"].Value = SeachRegistrationCriteria.KhamBenh;
                    rParams["parIsHoanTat"].Value = SeachRegistrationCriteria.IsHoanTat;
                    break;
                case ReportName.THONG_TIN_XUAT_VIEN:
                    ReportModel = null;
                    //TNHX: Đổi sang mẫu xuất viện giống với Thanh Vũ 1 (29/12/2018 21:13 PM)
                    //KMx: Đổi sang mẫu xuất viện giống với Viện Tim (17/12/2014 11:53).
                    //ReportModel = new GenericReportModel("eHCMS.ReportLib.InPatient.Reports.XrptThongTinXuatVien").PreviewModel;
                    //ReportModel = new GenericReportModel("eHCMS.ReportLib.InPatient.Reports.XrptDischargePaper").PreviewModel;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.InPatient.Reports.XrptDischargePaper_TV").PreviewModel;
                    rParams["InPatientAdmDisDetailID"].Value = ID;
                    rParams["parHospitalCode"].Value = hospitalCode;

                    //rParams["parHospitalName"].Value = reportHospitalName;
                    //rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    //rParams["parHospitalAddress"].Value = reportHospitalAddress;

                    break;

                case ReportName.AppointmentReport:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptAppointment.XRptAppointmentReport").PreviewModel;
                    rParams["para_PatientID"].Value = AppointmentCriteria.PatientID.GetValueOrDefault(0);
                    if (AppointmentCriteria.DateFrom == null)
                    {
                        rParams["DateFromFlag"].Value = 1;
                    }
                    else rParams["DateFromFlag"].Value = 0;
                    if (AppointmentCriteria.DateTo == null)
                    {
                        rParams["DateToFlag"].Value = 1;
                    }
                    else rParams["DateToFlag"].Value = 0;
                    rParams["para_DateFrom"].Value = AppointmentCriteria.DateFrom;
                    rParams["para_DateTo"].Value = AppointmentCriteria.DateTo;
                    rParams["para_DeptLocationID"].Value = AppointmentCriteria.DeptLocationID.GetValueOrDefault(-1);
                    rParams["para_V_ApptStatus"].Value = AppointmentCriteria.V_ApptStatus.GetValueOrDefault(-1);
                    rParams["para_LoaiDV"].Value = (int)AppointmentCriteria.LoaiDV;
                    rParams["para_IsConsultation"].Value = AppointmentCriteria.IsConsultation;
                    rParams["TieuDe"].Value = TieuDeRpt;
                    rParams["para_LocationName"].Value = AppointmentCriteria.LocationName;
                    rParams["para_Status"].Value = AppointmentCriteria.V_ApptStatusName;
                    rParams["para_LoaiDVName"].Value = AppointmentCriteria.LoaiDVName;

                    rParams["para_StaffID"].Value = AppointmentCriteria.StaffID;
                    rParams["para_DoctorStaffID"].Value = AppointmentCriteria.DoctorStaffID;
                    rParams["para_ConsultationTimeSegmentID"].Value = AppointmentCriteria.ApptTimeSegment.ConsultationTimeSegmentID;
                    if (AppointmentCriteria.ApptTimeSegment.ConsultationTimeSegmentID > 0)
                    {
                        rParams["para_StartTime"].Value = AppointmentCriteria.ApptTimeSegment.StartTime;
                        rParams["para_EndTime"].Value = AppointmentCriteria.ApptTimeSegment.EndTime;
                    }
                    else
                    {
                        rParams["para_StartTime"].Value = Globals.GetCurServerDateTime();
                        rParams["para_EndTime"].Value = Globals.GetCurServerDateTime();
                    }
                    rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
                    rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                    break;
                case ReportName.PCLExamTypeTarget:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptAppointment.XRptPatientPCLTargets_TV").PreviewModel;
                    rParams["ShowTitle"].Value = TieuDeRpt;
                    rParams["par_FromDate"].Value = FromDate;
                    rParams["par_ToDate"].Value = ToDate;
                    rParams["par_PCLExamTypeID"].Value = (int)ID;
                    rParams["par_IsAppointment"].Value = IsAppointment;

                    rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                    rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
                    rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                    break;

                case ReportName.PHIEUTHUTIENTONGHOP:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XrptPhieuThuTienTongHop").PreviewModel;
                    //rParams["Quarter"].Value = RptParameters.Quarter;
                    //rParams["Month"].Value = RptParameters.Month;
                    //rParams["Year"].Value = RptParameters.Year;
                    //rParams["flag"].Value = RptParameters.Flag;
                    //rParams["FromDate"].Value = RptParameters.FromDate;
                    //rParams["ToDate"].Value = RptParameters.ToDate;
                    ////rParams["ShowNgayThang"].Value = RptParameters.Show;
                    ////rParams["paraNote"].Value = string.Format("{0} ", eHCMSResources.Z1133_G1_CDHA.ToUpper()) + RptParameters.Show;
                    rParams["par_RepPaymentRecvID"].Value = (int)RepPaymentRecvID;
                    rParams["par_StaffName"].Value = StaffName;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    break;

                case ReportName.BANGLETHUPHI_KB_CDHA_THEONGAY:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptBangKeThuPhiKhamVaCDHATheoNgay_Ext").PreviewModel;
                    rParams["FromDate"].Value = FromDate;
                    rParams["ToDate"].Value = ToDate;
                    rParams["paraStaffName"].Value = StaffName;
                    rParams["paraStaffID"].Value = (int)StaffID;
                    strTextParam = string.Format(" {0} ", eHCMSResources.G1933_G1_TuNg.ToUpper()) + FromDate.GetValueOrDefault().ToString("dd/MM/yyyy") + string.Format(" - {0} ", eHCMSResources.K3192_G1_DenNg.ToUpper()) + ToDate.GetValueOrDefault().ToString("dd/MM/yyyy");
                    rParams["ShowNgayThang"].Value = TieuDeRpt;
                    rParams["paraNote"].Value = string.Format("{0} ", eHCMSResources.Z1132_G1_CDHAKB.ToUpper()) + strTextParam;
                    rParams["paraRepPaymtRecptByStaffID"].Value = (int)RepPaymtRecptByStaffID;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    break;
                case ReportName.BANGKETHUPHI_XN_THEONGAY:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptBangKeThuPhiXNTheoNgay_Ext").PreviewModel;
                    rParams["FromDate"].Value = FromDate;
                    rParams["ToDate"].Value = ToDate;
                    rParams["paraStaffName"].Value = StaffName;
                    rParams["paraStaffID"].Value = (int)StaffID;
                    strTextParam = string.Format(" {0} ", eHCMSResources.G1933_G1_TuNg.ToUpper()) + FromDate.GetValueOrDefault().ToString("dd/MM/yyyy") + string.Format(" - {0} ", eHCMSResources.K3192_G1_DenNg.ToUpper()) + ToDate.GetValueOrDefault().ToString("dd/MM/yyyy");
                    rParams["ShowNgayThang"].Value = TieuDeRpt;
                    rParams["paraNote"].Value = string.Format("{0} ", eHCMSResources.G2613_G1_XN.ToUpper()) + strTextParam;
                    rParams["paraRepPaymtRecptByStaffID"].Value = (int)RepPaymtRecptByStaffID;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    break;

                case ReportName.PHIEU_THU_KHAC:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptOutPatientReceipt_V2").PreviewModel;
                        rParams["parGenPaymtCustName"].Value = CurGenPaymt.PersonName;
                        rParams["parGenPaymtCustAddr"].Value = CurGenPaymt.PersonAddress;
                        rParams["parGenPaymtCustPhone"].Value = CurGenPaymt.PhoneNumber;
                        rParams["parGenPaymtCode"].Value = CurGenPaymt.GenericPaymentCode;
                        rParams["parGenPaymtReason"].Value = CurGenPaymt.V_GenericPaymentReason;
                        rParams["parStaffName"].Value = CurGenPaymt.StaffName;
                        rParams["parPaymentDate"].Value = (DateTime)CurGenPaymt.PaymentDate;
                        rParams["parPaymentAmount"].Value = CurGenPaymt.PaymentAmount.ToString();
                        rParams["parGenPaymtCustDOB"].Value = CurGenPaymt.DOB;
                        rParams["parGenPaymtOrgName"].Value = CurGenPaymt.OrgName;
                        rParams["parIsGenericPayment"].Value = true;
                        break;
                    }
                case ReportName.PHIEU_THU_KHAC_V4:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptOutPatientReceipt_V4").PreviewModel;
                        rParams["parPatientCode"].Value = CurGenPaymt.PatientCode;
                        rParams["parFileCodeNumber"].Value = CurGenPaymt.FileCodeNumber;
                        rParams["parGenPaymtCustName"].Value = CurGenPaymt.PersonName;
                        rParams["parGenPaymtCustAddr"].Value = CurGenPaymt.PersonAddress;
                        rParams["parGenPaymtCustPhone"].Value = CurGenPaymt.PhoneNumber;
                        rParams["parGenPaymtCode"].Value = CurGenPaymt.GenericPaymentCode;
                        rParams["parGenPaymtReason"].Value = CurGenPaymt.V_GenericPaymentReason;
                        rParams["parStaffName"].Value = CurGenPaymt.StaffName;
                        rParams["parPaymentDate"].Value = (DateTime)CurGenPaymt.PaymentDate;
                        //rParams["parPaymentAmount"].Value = CurGenPaymt.PaymentAmount.ToString();
                        rParams["parGenPaymtCustDOB"].Value = CurGenPaymt.DOB;
                        rParams["parGenPaymtOrgName"].Value = CurGenPaymt.OrgName;
                        //==== #002
                        rParams["parTotalPTPaymentAfterVAT"].Value = CurGenPaymt.PaymentAmount.ToString();
                        rParams["parTotalPTPaymentBeforeVAT"].Value = (CurGenPaymt.PaymentAmount - CurGenPaymt.VATAmount.GetValueOrDefault()).ToString();
                        rParams["parVATAmount"].Value = CurGenPaymt.VATAmount.ToString();
                        rParams["parVATPercent"].Value = CurGenPaymt.VATPercent == null ? null : (CurGenPaymt.VATPercent * 100.0 - 100.0).ToString();
                        //==== #002
                        rParams["parIsGenericPayment"].Value = true;
                        rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                        rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
                        break;
                    }
                //▼====: #012
                case ReportName.PATIENT_INFO:
                    {
                        ReportModel = null;
                        //▼====== #019
                        //▼====: #022
                        if (Globals.ServerConfigSection.Hospitals.HospitalCode == "95076" || Globals.ServerConfigSection.Hospitals.HospitalCode == "95078" || Globals.ServerConfigSection.Hospitals.HospitalCode == "96160")
                        {
                            ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptPatientInfo_TV").PreviewModel;
                            if (PatientName.Contains("CON BÀ"))
                            {
                                rParams["PatientFullAddress"].Value = PatientShortAddress;
                                rParams["parIsChild"].Value = true;
                            }
                            else
                            {
                                rParams["PatientFullAddress"].Value = PatientFullAddress;
                            }
                        }
                        else
                        {
                            ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptPatientInfo").PreviewModel;
                        }
                        //▲====: #019
                        //▲====: #022
                        rParams["FileCodeNumber"].Value = FileCodeNumber;
                        rParams["PatientName"].Value = PatientName;
                        rParams["DOB"].Value = DOB;
                        rParams["Age"].Value = Age;
                        rParams["Gender"].Value = Gender;

                        // TxD 30/05/2017: Even if we don't pass AdmissionDate parameter when it's NULL but DevExpress will create 
                        //                  a default DateTime of Now value so we need to set it to 01/01/1970 so our ReportService
                        //                  can check and know that AdmissionDate is not available thus printing Nothing
                        if (AdmissionDate != null)
                        {
                            rParams["AdmissionDate"].Value = AdmissionDate; //.GetValueOrDefault();
                        }
                        else
                        {
                            rParams["AdmissionDate"].Value = new DateTime(1970, 1, 1);
                        }

                        rParams["PatientCode"].Value = PatientCode;
                        break;
                    }
                //▲====: #012
                case ReportName.FETAL_ECHOCARDIOGRAPHY:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRptFetalEchocardiography").PreviewModel;
                        rParams["PatientPCLReqID"].Value = (int)PatientPCLReqID;
                        break;
                    }
                //==== #001
                case ReportName.STAFFDEPTPRESENCE:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptStoreDept.XRptStaffDeptPresence").PreviewModel;
                        rParams["StaffCountDate"].Value = FromDate.Value;
                        rParams["DeptID"].Value = (int)DeptID;
                        break;
                    }
                //==== #001
                //==== #002
                case ReportName.PATIENTINSTRUCTION:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XRptDoctorInstruction_V2").PreviewModel;
                        rParams["IntPtDiagDrInstructionID"].Value = (Int64)IntPtDiagDrInstructionID;
                        break;
                    }
                case ReportName.PATIENTINSTRUCTION_TH:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XRptDoctorInstruction_Summary_V2").PreviewModel;
                        rParams["PtRegistrationID"].Value = (Int64)RegistrationID;
                        rParams["IntPtDiagDrInstructionID"].Value = (Int64)IntPtDiagDrInstructionID;
                        rParams["FromDate"].Value = FromDate.GetValueOrDefault(DateTime.MinValue);
                        rParams["ToDate"].Value = ToDate.GetValueOrDefault(DateTime.MinValue);
                        //▼==== #038
                        //rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                        rParams["parHospitalName"].Value = reportHospitalNameShort;
                        //▲==== #038
                        rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
                        break;
                    }
                //==== #002
                //==== #003
                case ReportName.TRANSFERFORM:
                    {
                        ReportModel = null;
                        if (hospitalCode == "95076" || hospitalCode == "95078" )
                        {
                            ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptTransferForm_TV").PreviewModel;
                        }
                        else
                        {
                            ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptTransferForm").PreviewModel;
                        }
                        rParams["TransferFormID"].Value = Convert.ToInt32(this.TransferFormID);
                        rParams["PtRegistrationID"].Value = Convert.ToInt32(this.RegistrationID);
                        rParams["V_TransferFormType"].Value = Convert.ToInt32(this.TransferFormType);
                        rParams["V_PatientFindBy"].Value = Convert.ToInt32(this.FindBy);
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        break;
                    }
                //==== #003
                case ReportName.MEDICALFILESLIST:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptClinicDepts.XtraReports.RptMedicalFilesList").PreviewModel;
                        if (this.FromDate != null)
                            rParams["StartDate"].Value = this.FromDate;
                        if (this.ToDate != null)
                            rParams["EndDate"].Value = this.ToDate;
                        rParams["Registrations"].Value = this.FileCodeNumber;
                        break;
                    }
                case ReportName.MEDICALFILECHECKOUTCONFIRM:
                    {
                        ReportModel = null;
                        if (String.IsNullOrEmpty(this.IssueName))
                            ReportModel = new GenericReportModel("eHCMS.ReportLib.RptClinicDepts.XtraReports.RptMedicalFilesCheckinConfirm").PreviewModel;
                        else
                            ReportModel = new GenericReportModel("eHCMS.ReportLib.RptClinicDepts.XtraReports.RptMedicalFilesCheckoutConfirm").PreviewModel;
                        if (this.FromDate != null)
                            rParams["StartDate"].Value = this.FromDate;
                        if (this.ToDate != null)
                            rParams["EndDate"].Value = this.ToDate;
                        rParams["Registrations"].Value = this.FileCodeNumber;
                        rParams["IssueName"].Value = this.IssueName;
                        rParams["ReceiveName"].Value = this.ReceiveName;
                        if (this.AdmissionDate != null)
                            rParams["ReturnDate"].Value = this.AdmissionDate;
                        rParams["PtRegistrationID"].Value = Convert.ToInt32(this.RegistrationID);
                        if (this.MedicalFileStorageCheckID != null)
                            rParams["MedicalFileStorageCheckID"].Value = Convert.ToInt32(this.MedicalFileStorageCheckID);
                        break;
                    }
                case ReportName.MEDICALFILECHECKOUTHISTORY:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptClinicDepts.XtraReports.RptMedicalFilesCheckoutHistory").PreviewModel;
                        if (this.FromDate != null)
                            rParams["StartDate"].Value = this.FromDate;
                        if (this.ToDate != null)
                            rParams["EndDate"].Value = this.ToDate;
                        rParams["Registrations"].Value = this.FileCodeNumber;
                        break;
                    }
                case ReportName.CLINIC_INFO:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptClinicDepts.XtraReports.RptInfoPatient").PreviewModel;
                        //rParams["PatientID"].Value = Convert.ToInt32(PatientID);
                        rParams["PatientCode"].Value = PatientCode;
                        rParams["PatientName"].Value = PatientName;
                        rParams["DOB"].Value = DOB;
                        rParams["FileCodeNumber"].Value = FileCodeNumber;
                        break;
                    }
                case ReportName.EstimationPriceReport:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptSugeryEstimationPriceReport").PreviewModel;
                        rParams["pConsultingDiagnosysID"].Value = Convert.ToInt32(this.PrimaryID.GetValueOrDefault(0));
                        break;
                    }
                /*▼====: #006*/
                case ReportName.REGISTRATION_SPECIFY_VOTES_XML:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptOutPatientPhieuChiDinhXML").PreviewModel;
                        rParams["param_ListID"].Value = Result;
                        rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                        rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
                        rParams["parLogoUrl"].Value = reportLogoUrl;
                        break;
                    }
                /*▲====: #006*/
                /*▼====: #008*/
                case ReportName.PHIEUDENGHITAMUNG_TV:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptPhieuDeNghiTamUng_TV").PreviewModel;
                        rParams["param_RptPtCashAdvRemID"].Value = (int)ID;//thuc ra la PtRegistrationID do
                        rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                        rParams["parHospitalAddress"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalAddress;
                        break;
                    }
                /*▲====: #008*/
                //▼====: #013
                case ReportName.PHIEUMIENGIAM_NGOAITRU_TV:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptPhieuMienGiamNgoaiTru_TV").PreviewModel;
                        rParams["parPromoDiscProgID"].Value = (int)PromoDiscProgID;
                        rParams["parTotalMienGiam"].Value = (int)TotalMienGiam;
                        //▼====: #0015
                        rParams["parPtRegistrationID"].Value = RegistrationID;
                        rParams["parPatientPCLReqID"].Value = PatientPCLReqID;
                        //▲====: #0015
                        rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                        rParams["parHospitalAddress"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalAddress;
                        rParams["pV_RegistrationType"].Value = V_RegistrationType;
                        break;
                    }
                //▲====: #013
                //▼====: #014
                case ReportName.RptPatientServiceRequestDetailsByPatientServiceReqID:
                    {
                        ReportModel = null;
                        ReportModel = new RptPatientServiceRequestDetailsByPatientServiceReqID_XML_TV().PreviewModel;
                        rParams["parPtRegistrationID"].Value = (int)RegistrationID;
                        rParams["parRequestXML"].Value = Result;
                        rParams["paramV_RegistrationType"].Value = (int)V_RegistrationType;
                        rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
                        rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                        break;
                    }
                //▲====: #014
                case ReportName.RptOutPtTransactionFinalization:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XtraReports.XRptOutPtTransactionFinalization").PreviewModel;
                        rParams["pPtRegistrationID"].Value = RegistrationID;
                        rParams["pV_RegistrationType"].Value = V_RegistrationType != 0 ? V_RegistrationType : (long)AllLookupValues.RegistrationType.NGOAI_TRU;
                        rParams["pTranFinalizationID"].Value = ID;
                        rParams["pTransactionFinalizationSummaryInfoID"].Value = PrimaryID.GetValueOrDefault(0);
                        break;
                    }
                case ReportName.The_Thu_Thuat:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptTheThuThuat_TV").PreviewModel;
                        rParams["parPtRegistrationDetailID"].Value = RegistrationDetailID;
                        rParams["V_RegistrationType"].Value = V_RegistrationType;
                        break;
                    }
                case ReportName.BAO_CAO_VIEN_PHI_BHYT:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptBaoCaoThuVienPhiBHYT").PreviewModel;
                        rParams["RepPaymentRecvID"].Value = ID;
                        rParams["Case"].Value = (int)AllLookupValues.HIType.HI;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        break;
                    }
                case ReportName.BAO_CAO_VIEN_PHI_NGOAI_TRU:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptBaoCaoThuVienPhiNgoaiTru").PreviewModel;
                        rParams["RepPaymentRecvID"].Value = ID;
                        rParams["Case"].Value = (int)AllLookupValues.HIType.NoHI;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        break;
                    }
                case ReportName.REGISTRATION_OUT_PATIENT_RECEIPT_XML_V4_THERMAL:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptOutPatientReceiptXML_V4_InNhiet").PreviewModel;
                        rParams["param_PaymentID"].Value = Result;
                        // 20181027 TNHX: [BM0002176] Add parameter to show HospitalName & DepartmentOfHealth
                        rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                        rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
                        break;
                    }
                case ReportName.Kham_Suc_Khoe:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptSoKhamSucKhoe").PreviewModel;
                        //rParams["parPtRegDetailID"].Value = RegistrationDetailID;
                        rParams["parPtRegistrationID"].Value = ID;
                        rParams["IsShowExamAllResultDetails"].Value = IsDetails;
                        rParams["ExaminationResultVersion"].Value = Globals.ServerConfigSection.CommonItems.ExaminationResultVersion;
                        //▼==== #036
                        rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                        rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        //▲==== #036
                        break;
                    }
                case ReportName.CT_HD_KhamSucKhoe:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptAppointment.RptHospitalClientContractDetails").PreviewModel;
                        rParams["pHosClientContractID"].Value = ID;
                        break;
                    }
                //▼====: #017
                case ReportName.RptBangGiaDV:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConfiguration.XtraReports.XRpt_BangGiaDV").PreviewModel;
                        rParams["parMedServiceItemPriceListID"].Value = ID;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        break;
                    }
                case ReportName.RptBangGiaCLS:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConfiguration.XtraReports.XRpt_BangGiaCLS").PreviewModel;
                        rParams["parPCLExamTypePriceListID"].Value = ID;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        break;
                    }
                //▲====: #017
                case ReportName.MedicalRecord:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XRptInPtAndrologyRecord").PreviewModel;
                        rParams["PtRegistrationID"].Value = RegistrationID;
                        rParams["V_RegistrationType"].Value = V_RegistrationType;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        break;
                    }
                case ReportName.GeneralOutPtMedicalFile:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XRptGeneralOutPtMedicalFile").PreviewModel;
                    rParams["OutPtTreatmentProgramID"].Value = OutPtTreatmentProgramID;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["DiagConsultationSummaryID"].Value = ID;
                    rParams["V_RegistrationType"].Value = V_RegistrationType;
                    break;
                case ReportName.MaxillofacialOutPtMedicalFile:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XRptMaxillofacialOutPtMedicalFile").PreviewModel;
                    rParams["OutPtTreatmentProgramID"].Value = OutPtTreatmentProgramID;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalCode"].Value = hospitalCode;
                    rParams["PtRegistrationID"].Value = RegistrationID;
                    rParams["DiagConsultationSummaryID"].Value = ID;
                    rParams["V_RegistrationType"].Value = V_RegistrationType;
                    break;
                case ReportName.ObstetricsMedicalFile:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XRptObstetricsMedicalFile").PreviewModel;
                    rParams["PtRegistrationID"].Value = RegistrationID;
                    rParams["PtRegDetailID"].Value = RegistrationDetailID;
                    rParams["V_RegistrationType"].Value = (long)AllLookupValues.RegistrationType.NOI_TRU;
                    break;
                case ReportName.PediatricsMedicalFile:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XRptPediatricsMedicalFile").PreviewModel;
                    rParams["PtRegistrationID"].Value = RegistrationID;
                    rParams["PtRegDetailID"].Value = RegistrationDetailID;
                    rParams["V_RegistrationType"].Value = (long)AllLookupValues.RegistrationType.NOI_TRU;
                    break;
                case ReportName.HosClientContractPatientSummary:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XRptHosClientContractPatientSummaryXML").PreviewModel;
                    rParams["HosContractPtIDCollection"].Value = string.Join("|", RegistrationDetailIDList);
                    break;
                case ReportName.BienBanHoiChan:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XRptBienBanHoiChan").PreviewModel;
                    rParams["DiagConsultationSummaryID"].Value = ID;
                    break;
                //▼====: #018
                case ReportName.REGISTRATION_INPT_SPECIFY_VOTES_XML:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptInPatientPhieuChiDinhXML").PreviewModel;
                        rParams["param_ListID"].Value = Result;
                        rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                        rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
                        rParams["parLogoUrl"].Value = reportLogoUrl;
                        break;
                    }
                //▲====: #018
                //▼====: #021
                case ReportName.XRptPatientPCLRequestDetailsByPatientPCLReqID_TV3:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XRptPatientPCLRequestDetailsByPatientPCLReqID_TV3").PreviewModel;
                        rParams["parPtRegistrationID"].Value = RegistrationID;
                        rParams["parPatientPCLReqXML"].Value = Result;
                        rParams["paramV_RegistrationType"].Value = (int)V_RegistrationType;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parLogoUrl"].Value = reportLogoUrl;
                        break;
                    }
                //▲====: #021
                //▼====: #023
                //▼====: #022
                case ReportName.InTheKCB:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XtraReports.XRpt_InTheKCB").PreviewModel;
                        rParams["parFullName"].Value = CurPatient.FullName;
                        rParams["parDOB"].Value = CurPatient.DOBText;
                        rParams["parParentName"].Value = CurPatient.FContactFullName;
                        rParams["parParentPhone"].Value = CurPatient.FContactCellPhone;
                        rParams["parPatientCode"].Value = CurPatient.PatientCode;
                        rParams["parTypeCard"].Value = (int)(ID == 0 ? 1 : ID);
                        rParams["parImageUrl"].Value = ImageUrl;
                        break;
                    }
                    //▲====: #022
                    //▲====: #023
                //▼====: #024
                case ReportName.XRptPhieuCungCapMau:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XRptPhieuCungCapMau").PreviewModel;
                        rParams["parIntPtDiagDrInstructionID"].Value = ID;
                        rParams["parPatientCode"].Value = PatientCode;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        break;
                    }
                //▲====: #024
                case ReportName.TomTatQuaTrinhDieuTri:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XtraReports.XRptTreatmentProcess").PreviewModel;
                        rParams["parTreatmentProcessID"].Value = (long)TreatmentProcessID;
                        break;
                    }
                //▼====: #025
                case ReportName.XRpt_AdvisoryVotes:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRpt_AdvisoryVotes").PreviewModel;
                        rParams["parPatientName"].Value = PatientName;
                        rParams["parDOB"].Value = DOB;
                        rParams["parAdmissionDate"].Value = AdmissionDate.Value.ToString("dd/MM/yyyy");
                        rParams["parPatientCode"].Value = PatientCode;
                        rParams["parDoctorName"].Value = StaffName;
                        rParams["parDiagnosis"].Value = Result;
                        break;
                    }
                //▲====: #025
                case ReportName.XRpt_NutritionalRating:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRpt_NutritionalRating").PreviewModel;
                        rParams["NutritionalRatingID"].Value = (Int64)NutritionalRatingID;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        break;
                    }
                case ReportName.XRpt_TomTatHoSoBenhAn:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_TomTatHoSoBenhAn").PreviewModel;
                        rParams["SummaryMedicalRecordID"].Value = ID;
                        rParams["V_RegistrationType"].Value = V_RegistrationType;
                        rParams["parHospitalCode"].Value = hospitalCode;
                        //▼==== #038
                        //rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalName"].Value = reportHospitalNameShort;
                        //▲==== #038
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        break;
                    }
                case ReportName.XRpt_XacNhanDieuTri_NgoaiTru_NoiTru:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_XacNhanDieuTri_NgoaiTru_NoiTru").PreviewModel;
                        rParams["PatientTreatmentCertificateID"].Value =ID;
                        rParams["V_RegistrationType"].Value = V_RegistrationType;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parSuffixCode"].Value = Globals.ServerConfigSection.CommonItems.SuffixAutoCodeForCirculars56 ;
                        break;
                    }
                case ReportName.XRpt_ChungNhanThuongTich_NgoaiTru_NoiTru:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_ChungNhanThuongTich_NgoaiTru_NoiTru").PreviewModel;
                        rParams["InjuryCertificateID"].Value =ID;
                        rParams["V_RegistrationType"].Value = V_RegistrationType;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parSuffixCode"].Value = Globals.ServerConfigSection.CommonItems.SuffixAutoCodeForCirculars56;
                        break;
                    }
                case ReportName.XRpt_GiayChungSinh:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_GiayChungSinh").PreviewModel;
                        rParams["BirthCertificateID"].Value =ID;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        break;
                    }
                case ReportName.XRpt_GiayBaoTu:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_GiayBaoTu").PreviewModel;
                        rParams["PtRegistrationID"].Value =RegistrationID;
                        //▼==== #038
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalNameShort"].Value = reportHospitalNameShort;
                        rParams["parHospitalCode"].Value = hospitalCode;
                        //▲==== #038
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        break;
                    }
                case ReportName.XRpt_GiayNghiViecKhongHuongBaoHiem:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_GiayNghiViecKhongHuongBaoHiem").PreviewModel;
                        rParams["VacationInsuranceCertificateID"].Value =ID;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalCode"].Value = hospitalCode;
                        break;
                    }
                case ReportName.XRpt_PCLImagingResult_New:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New").PreviewModel;
                        rParams["PCLImgResultID"].Value =ID;
                        rParams["V_PCLRequestType"].Value = V_PCLRequestType;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalCode"].Value = hospitalCode;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        break;
                    }
                case ReportName.XRpt_BienBanHoiChan:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XRpt_BienBanHoiChan").PreviewModel;
                        rParams["DiagConsultationSummaryID"].Value = ID;
                        rParams["V_RegistrationType"].Value = V_RegistrationType;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        //▼==== #038
                        //rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalName"].Value = reportHospitalNameShort;
                        //▲==== #038
                        break;
                    }
                case ReportName.XRpt_AdmissionExamination:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XRpt_AdmissionExamination").PreviewModel;
                        rParams["PtRegistrationID"].Value = RegistrationID;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalCode"].Value = hospitalCode;
                        break;
                    }
                case ReportName.XRpt_PCLImagingResult_New_4_Hinh:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_4_Hinh").PreviewModel;
                        rParams["PCLImgResultID"].Value =ID;
                        rParams["V_PCLRequestType"].Value = V_PCLRequestType;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalCode"].Value = hospitalCode;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        break;
                    }
                case ReportName.XRpt_PCLImagingResult_New_3_Hinh:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_3_Hinh").PreviewModel;
                        rParams["PCLImgResultID"].Value =ID;
                        rParams["V_PCLRequestType"].Value = V_PCLRequestType;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalCode"].Value = hospitalCode;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        break;
                    }
                case ReportName.XRpt_PCLImagingResult_New_0_Hinh:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_0_Hinh").PreviewModel;
                        rParams["PCLImgResultID"].Value =ID;
                        rParams["V_PCLRequestType"].Value = V_PCLRequestType;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalCode"].Value = hospitalCode;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        break;
                    }
                //▼====: #026
                case ReportName.XRpt_PhieuChamSoc:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XRpt_PhieuChamSoc").PreviewModel;
                        rParams["PtRegistrationID"].Value = RegistrationID;
                        rParams["V_RegistrationType"].Value = V_RegistrationType;
                        rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                        rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
                        break;
                    }
                //▲====: #026
                case ReportName.XRpt_PCLImagingResult_New_1_Hinh:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_1_Hinh").PreviewModel;
                        rParams["PCLImgResultID"].Value = ID;
                        rParams["V_PCLRequestType"].Value = V_PCLRequestType;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalCode"].Value = hospitalCode;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        break;
                    }
                case ReportName.XRpt_XacNhan_DieuTri_Covid:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XRpt_XacNhan_DieuTri_Covid").PreviewModel;
                        rParams["InPatientAdmDisDetailID"].Value = ID;
                        rParams["parHospitalCode"].Value = hospitalCode;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        break;
                    }
                case ReportName.XRpt_PCLImagingResult_New_6_Hinh:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_6_Hinh").PreviewModel;
                        rParams["PCLImgResultID"].Value = ID;
                        rParams["V_PCLRequestType"].Value = V_PCLRequestType;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalCode"].Value = hospitalCode;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        break;
                    }
                case ReportName.XRpt_PCLImagingResult_New_Test_Nhanh_Cov:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Test_Nhanh_Cov").PreviewModel;
                        rParams["PCLImgResultID"].Value = ID;
                        rParams["V_PCLRequestType"].Value = V_PCLRequestType;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalCode"].Value = hospitalCode;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        //▼==== #037
                        rParams["parDepartmentOfHealthEng"].Value = reportDepartmentOfHealthEng;
                        rParams["parHospitalAddressEng"].Value = reportHospitalAddressEng;
                        rParams["parHospitalNameEng"].Value = reportHospitalNameEng;
                        rParams["IsBilingual"].Value = IsBilingual;
                        //▲==== #037
                        break;
                    }
                case ReportName.XRpt_PCLImagingResult_New_Helicobacter_Pylori:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Helicobacter_Pylori").PreviewModel;
                        rParams["PCLImgResultID"].Value = ID;
                        rParams["V_PCLRequestType"].Value = V_PCLRequestType;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalCode"].Value = hospitalCode;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        //▼==== #037
                        rParams["parDepartmentOfHealthEng"].Value = reportDepartmentOfHealthEng;
                        rParams["parHospitalAddressEng"].Value = reportHospitalAddressEng;
                        rParams["parHospitalNameEng"].Value = reportHospitalNameEng;
                        rParams["IsBilingual"].Value = IsBilingual;
                        //▲==== #037
                        break;
                    }
                case ReportName.XRpt_PCLImagingResult_New_Realtime_PCR_Cov:
                    {
                        ReportModel = null;
                        if (IsBilingual && Globals.ServerConfigSection.CommonItems.IsApplyPCRDual)
                        {
                            ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Realtime_PCR_Cov_Dual").PreviewModel;
                        }
                        else
                        {
                            ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Realtime_PCR_Cov").PreviewModel;
                        }
                        
                        rParams["PCLImgResultID"].Value = ID;
                        rParams["V_PCLRequestType"].Value = V_PCLRequestType;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalCode"].Value = hospitalCode;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        break;
                    }
                case ReportName.XRpt_PCLImagingResult_New_Xet_Nghiem:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Xet_Nghiem").PreviewModel;
                        rParams["PCLImgResultID"].Value = ID;
                        rParams["V_PCLRequestType"].Value = V_PCLRequestType;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalCode"].Value = hospitalCode;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        break;
                    }
                case ReportName.XRpt_PCLImagingResult_New_Sieu_Am_Tim:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Sieu_Am_Tim").PreviewModel;
                        rParams["PCLImgResultID"].Value = ID;
                        rParams["V_PCLRequestType"].Value = V_PCLRequestType;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalCode"].Value = hospitalCode;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        break;
                    }
                case ReportName.XRpt_PCLImagingResult_New_Sieu_Am_San_4D:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Sieu_Am_San_4D").PreviewModel;
                        rParams["PCLImgResultID"].Value = ID;
                        rParams["V_PCLRequestType"].Value = V_PCLRequestType;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalCode"].Value = hospitalCode;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        break;
                    }
                case ReportName.XRpt_PCLImagingResult_New_Dien_Tim:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Dien_Tim").PreviewModel;
                        rParams["PCLImgResultID"].Value = ID;
                        rParams["V_PCLRequestType"].Value = V_PCLRequestType;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalCode"].Value = hospitalCode;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        break;
                    }
                case ReportName.XRpt_PCLImagingResult_New_Dien_Nao:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Dien_Nao").PreviewModel;
                        rParams["PCLImgResultID"].Value = ID;
                        rParams["V_PCLRequestType"].Value = V_PCLRequestType;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalCode"].Value = hospitalCode;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        break;
                    }
                case ReportName.XRpt_PCLImagingResult_New_ABI:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_ABI").PreviewModel;
                        rParams["PCLImgResultID"].Value = ID;
                        rParams["V_PCLRequestType"].Value = V_PCLRequestType;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalCode"].Value = hospitalCode;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        break;
                    }
                //▼====: #027
                case ReportName.XRpt_PhieuThucHienYLenhThuoc:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XRpt_PhieuThucHienYLenhThuoc").PreviewModel;
                        rParams["PtRegistrationID"].Value = RegistrationID;
                        rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                        rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
                        break;
                    }
                //▲====: #027
                case ReportName.XRpt_PCLImagingResult_New_Dien_Tim_Gang_Suc:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Dien_Tim_Gang_Suc").PreviewModel;
                        rParams["PCLImgResultID"].Value = ID;
                        rParams["V_PCLRequestType"].Value = V_PCLRequestType;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalCode"].Value = hospitalCode;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        break;
                    }
                case ReportName.XRpt_GiaoNhan_BenhNhan_Covid:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XRpt_GiaoNhan_BenhNhan_Covid").PreviewModel;
                        rParams["InPatientAdmDisDetailID"].Value = ID;
                        rParams["parHospitalCode"].Value = hospitalCode;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        break;
                    }
                case ReportName.XRptRequestDrugDeptByPCLRequest:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XtraReports.XRptRequestDrugDeptByPCLRequest").PreviewModel;
                        rParams["ReqDrugInClinicDeptID"].Value = ID;
                        rParams["V_RegistrationType"].Value = V_RegistrationType;
                        rParams["parLogoUrl"].Value = reportLogoUrl;
                        break;
                    }
                case ReportName.XRpt_PCLImagingResult_New_CLVT_Hai_Ham:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_CLVT_Hai_Ham").PreviewModel;
                        rParams["PCLImgResultID"].Value = ID;
                        rParams["V_PCLRequestType"].Value = V_PCLRequestType;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalCode"].Value = hospitalCode;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        break;
                    }
                //▼==== #027
                case ReportName.GiayHoanTamUngNoiTru:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptGiayHoanTamUngNoiTru").PreviewModel;
                        rParams["InPatientAdmDisDetailID"].Value = (int)ID;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        break;
                    }
                //▲==== #027
                //▼==== #028
                case ReportName.GiayMienTamUngNoiTru:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptGiayMienTamUngNoiTru").PreviewModel;
                        rParams["InPatientAdmDisDetailID"].Value = (int)ID;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        break;
                    }
                //▲==== #028
                case ReportName.BaoCaoThangDiemCanhBaoSom:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.InPatient.Reports.XRpt_BaoCaoThangDiemCanhBaoSom").PreviewModel;
                        rParams["PhyExamID"].Value = (Int64)ID;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        break;
                    }
                //▼====: #029
                case ReportName.XRpt_PCLImagingResult_New_Sieu_Am_San_4D_New:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Sieu_Am_San_4D_New").PreviewModel;
                        rParams["PCLImgResultID"].Value = ID;
                        rParams["V_PCLRequestType"].Value = V_PCLRequestType;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalCode"].Value = hospitalCode;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        break;
                    }
                case ReportName.XRpt_PCLImagingResult_New_6_Hinh_2_New:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_6_Hinh_2_New").PreviewModel;
                        rParams["PCLImgResultID"].Value = ID;
                        rParams["V_PCLRequestType"].Value = V_PCLRequestType;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalCode"].Value = hospitalCode;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        break;
                    }
                case ReportName.XRpt_PCLImagingResult_New_6_Hinh_1_New:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_6_Hinh_1_New").PreviewModel;
                        rParams["PCLImgResultID"].Value = ID;
                        rParams["V_PCLRequestType"].Value = V_PCLRequestType;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalCode"].Value = hospitalCode;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        break;
                    }
                case ReportName.XRpt_PCLImagingResult_New_Sieu_Am_Tim_New:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Sieu_Am_Tim_New").PreviewModel;
                        rParams["PCLImgResultID"].Value = ID;
                        rParams["V_PCLRequestType"].Value = V_PCLRequestType;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalCode"].Value = hospitalCode;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        break;
                    }
                case ReportName.XRpt_PCLImagingResult_New_Noi_Soi_9_Hinh:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Noi_Soi_9_Hinh").PreviewModel;
                        rParams["PCLImgResultID"].Value = ID;
                        rParams["V_PCLRequestType"].Value = V_PCLRequestType;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalCode"].Value = hospitalCode;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        break;
                    }
                //▲====: #029
                //▼====: #030
                case ReportName.XRpt_GiayDeNghiMoTheKCB:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XtraReports.XRpt_GiayDeNghiMoTheKCB").PreviewModel;
                        rParams["CardID"].Value = ID;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                        rParams["parStaffName"].Value = StaffName;
                        break;
                    }
                //▲====: #030
                //▼====: #031
                case ReportName.XRpt_LichDangKyKhamNgoaiGio:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XtraReports.XRpt_LichDangKyKhamNgoaiGio").PreviewModel;
                        rParams["OvertimeWorkingWeekID"].Value = ID;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parStaffName"].Value = StaffName;
                        break;
                    }
                //▲====: #031
                //▼==== #033
                case ReportName.XRptSelfDeclaration:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptSelfDeclaration").PreviewModel;
                        rParams["PtRegistrationID"].Value = RegistrationID;
                        rParams["V_RegistrationType"].Value = V_RegistrationType;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalCode"].Value = hospitalCode;
                        break;
                    }
                case ReportName.ChronicOutPtMedicalFile:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XRptChronicOutPtMedicalFile").PreviewModel;
                    rParams["OutPtTreatmentProgramID"].Value = OutPtTreatmentProgramID;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalCode"].Value = hospitalCode;
                    rParams["PtRegistrationID"].Value = RegistrationID;
                    rParams["DiagConsultationSummaryID"].Value = ID;
                    rParams["V_RegistrationType"].Value = V_RegistrationType;
                    break;
                //▲==== #033
                case ReportName.PhieuTheoDoiDichTruyen:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XRptPhieuTheoDoiDichTruyen").PreviewModel;
                        rParams["PtRegistrationID"].Value = (Int64)RegistrationID;
                        rParams["IntPtDiagDrInstructionID"].Value = (Int64)IntPtDiagDrInstructionID;
                        rParams["FromDate"].Value = FromDate.GetValueOrDefault(DateTime.MinValue);
                        rParams["ToDate"].Value = ToDate.GetValueOrDefault(DateTime.MinValue);
                        rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                        rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
                        break;
                    }

                //▼==== #034
                case ReportName.XRpt_PCLImagingResult_New_0_Hinh_V2:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_0_Hinh_V2").PreviewModel;
                        rParams["PCLImgResultID"].Value = ID;
                        rParams["V_PCLRequestType"].Value = V_PCLRequestType;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalCode"].Value = hospitalCode;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        //▼==== #037
                        rParams["parDepartmentOfHealthEng"].Value = reportDepartmentOfHealthEng;
                        rParams["parHospitalAddressEng"].Value = reportHospitalAddressEng;
                        rParams["parHospitalNameEng"].Value = reportHospitalNameEng;
                        rParams["IsBilingual"].Value = IsBilingual;
                        //▲==== #037
                        break;
                    }
                case ReportName.XRpt_PCLImagingResult_New_Xet_Nghiem_V2:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Xet_Nghiem_V2").PreviewModel;
                        rParams["PCLImgResultID"].Value = ID;
                        rParams["V_PCLRequestType"].Value = V_PCLRequestType;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalCode"].Value = hospitalCode;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        //▼==== #037
                        rParams["parDepartmentOfHealthEng"].Value = reportDepartmentOfHealthEng;
                        rParams["parHospitalAddressEng"].Value = reportHospitalAddressEng;
                        rParams["parHospitalNameEng"].Value = reportHospitalNameEng;
                        rParams["IsBilingual"].Value = IsBilingual;
                        //▲==== #037
                        break;
                    }
                case ReportName.XRpt_PCLImagingResult_New_0_Hinh_XN:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_0_Hinh_XN").PreviewModel;
                        rParams["PCLImgResultID"].Value = ID;
                        rParams["V_PCLRequestType"].Value = V_PCLRequestType;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalCode"].Value = hospitalCode;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        //▼==== #037
                        rParams["parDepartmentOfHealthEng"].Value = reportDepartmentOfHealthEng;
                        rParams["parHospitalAddressEng"].Value = reportHospitalAddressEng;
                        rParams["parHospitalNameEng"].Value = reportHospitalNameEng;
                        rParams["IsBilingual"].Value = IsBilingual;
                        //▲==== #037
                        break;
                    }
                case ReportName.XRpt_PCLImagingResult_New_1_Hinh_XN:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_1_Hinh_XN").PreviewModel;
                        rParams["PCLImgResultID"].Value = ID;
                        rParams["V_PCLRequestType"].Value = V_PCLRequestType;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalCode"].Value = hospitalCode;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        //▼==== #037
                        rParams["parDepartmentOfHealthEng"].Value = reportDepartmentOfHealthEng;
                        rParams["parHospitalAddressEng"].Value = reportHospitalAddressEng;
                        rParams["parHospitalNameEng"].Value = reportHospitalNameEng;
                        rParams["IsBilingual"].Value = IsBilingual;
                        //▲==== #037
                        break;
                    }
                //▲==== #034
                case ReportName.XRpt_DisChargePapers:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptDischargePapers_V2").PreviewModel;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["PtRegistrationID"].Value = RegistrationID;
                        rParams["V_RegistrationType"].Value = V_RegistrationType;
                        break;
                    }
                //▼==== #035
                case ReportName.XRpt_AgeOfTheArtery:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRpt_AgeOfTheArtery").PreviewModel;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["AgeOfTheArteryID"].Value = ID;
                        rParams["parStaffName"].Value = StaffName;
                        break;
                    }
                    //▲==== #035
                case ReportName.XRpt_BienBanKiemDiemTuVong:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_BienBanKiemDiemTuVong").PreviewModel;
                        rParams["DeathCheckRecordID"].Value = ID;
                        //▼==== #038
                        //rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalName"].Value = reportHospitalNameShort;
                        //▲==== #038
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        break;
                    }
                case ReportName.XRpt_TreatmentProcessSummary:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XtraReports.XRptTreatmentProcess_V2").PreviewModel;
                        rParams["parTreatmentProcessID"].Value = TreatmentProcessID;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        //▼==== #038
                        //rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalName"].Value = reportHospitalNameShort;
                        //▲==== #038
                        break;
                    }
            }
            // ReportModel.AutoShowParametersPanel = false;
            ReportModel.CreateDocument(rParams);
        }

        private GenericPayment _CurGenPaymt;
        public GenericPayment CurGenPaymt
        {
            get
            {
                return _CurGenPaymt;
            }
            set
            {
                _CurGenPaymt = value;
                NotifyOfPropertyChange(() => CurGenPaymt);
            }
        }

        private long _StaffID = 0;
        public long StaffID
        {
            get { return _StaffID; }
            set
            {
                _StaffID = value;
                NotifyOfPropertyChange(() => StaffID);
            }
        }

        private long _RepPaymtRecptByStaffID = 0;
        public long RepPaymtRecptByStaffID
        {
            get { return _RepPaymtRecptByStaffID; }
            set
            {
                _RepPaymtRecptByStaffID = value;
                NotifyOfPropertyChange(() => RepPaymtRecptByStaffID);
            }
        }

        private long _DoctorStaffID;
        public long DoctorStaffID
        {
            get { return _DoctorStaffID; }
            set
            {
                _DoctorStaffID = value;
                NotifyOfPropertyChange(() => DoctorStaffID);

            }
        }

        private DateTime? _FromDate;

        public DateTime? FromDate
        {
            get { return _FromDate; }
            set
            {
                _FromDate = value;
                NotifyOfPropertyChange(() => FromDate);
            }
        }

        private DateTime? _ToDate;

        public DateTime? ToDate
        {
            get { return _ToDate; }
            set
            {
                _ToDate = value;
                NotifyOfPropertyChange(() => ToDate);
            }
        }

        private SeachPtRegistrationCriteria _SeachRegistrationCriteria;
        public SeachPtRegistrationCriteria SeachRegistrationCriteria
        {
            get { return _SeachRegistrationCriteria; }
            set
            {
                _SeachRegistrationCriteria = value;
                NotifyOfPropertyChange(() => SeachRegistrationCriteria);
            }
        }

        private AppointmentSearchCriteria _AppointmentCriteria;
        public AppointmentSearchCriteria AppointmentCriteria
        {
            get { return _AppointmentCriteria; }
            set
            {
                _AppointmentCriteria = value;
                NotifyOfPropertyChange(() => AppointmentCriteria);
            }
        }

        private long _V_MedProductType;
        public long V_MedProductType
        {
            get
            {
                return _V_MedProductType;
            }
            set
            {
                if (_V_MedProductType != value)
                {
                    _V_MedProductType = value;
                    NotifyOfPropertyChange(() => V_MedProductType);
                }

            }
        }

        private string _Result;
        public string Result
        {
            get { return _Result; }
            set
            {
                _Result = value;
                NotifyOfPropertyChange(() => Result);
            }
        }

        private string _TieuDeRpt;
        public string TieuDeRpt
        {
            get { return _TieuDeRpt; }
            set
            {
                _TieuDeRpt = value;
                NotifyOfPropertyChange(() => TieuDeRpt);
            }
        }


        private long _DrugDeptSellingPriceListID;
        public long DrugDeptSellingPriceListID
        {
            get { return _DrugDeptSellingPriceListID; }
            set
            {
                _DrugDeptSellingPriceListID = value;
                NotifyOfPropertyChange(() => DrugDeptSellingPriceListID);

            }
        }

        private string _TenYCuHoaChat;
        public string TenYCuHoaChat
        {
            get
            {
                return _TenYCuHoaChat;
            }
            set
            {
                _TenYCuHoaChat = value;
                NotifyOfPropertyChange(() => TenYCuHoaChat);
            }
        }

        private string _TenYCuHoaChatTiengViet;
        public string TenYCuHoaChatTiengViet
        {
            get
            {
                return _TenYCuHoaChatTiengViet;
            }
            set
            {
                _TenYCuHoaChatTiengViet = value;
                NotifyOfPropertyChange(() => TenYCuHoaChatTiengViet);
            }
        }

        private long _PharmacySellingPriceListID;
        public long PharmacySellingPriceListID
        {
            get { return _PharmacySellingPriceListID; }
            set
            {
                _PharmacySellingPriceListID = value;
                NotifyOfPropertyChange(() => PharmacySellingPriceListID);

            }
        }


        private long? _DeptID = 0;
        public long? DeptID
        {
            get { return _DeptID; }
            set
            {
                _DeptID = value;
                NotifyOfPropertyChange(() => DeptID);

            }
        }

        public string DeptName { get; set; }

        private long? _DeptLocID;
        public long? DeptLocID
        {
            get { return _DeptLocID; }
            set
            {
                _DeptLocID = value;
                NotifyOfPropertyChange(() => DeptLocID);

            }
        }

        public string LocationName { get; set; }

        private string _TieuDeRpt1;
        public string TieuDeRpt1
        {
            get { return _TieuDeRpt1; }
            set
            {
                _TieuDeRpt1 = value;
                NotifyOfPropertyChange(() => TieuDeRpt1);
            }
        }

        private long _AppointmentID;
        public long AppointmentID
        {
            get { return _AppointmentID; }
            set
            {
                _AppointmentID = value;
                NotifyOfPropertyChange(() => AppointmentID);
            }
        }

        private string _strIDList;
        public string strIDList
        {
            get
            {
                return _strIDList;
            }
            set
            {
                if (_strIDList != value)
                {
                    _strIDList = value;
                    NotifyOfPropertyChange(() => strIDList);
                }

            }
        }


        private byte _flag;
        public byte flag
        {
            get
            {
                return _flag;
            }
            set
            {
                if (_flag != value)
                {
                    _flag = value;
                    NotifyOfPropertyChange(() => flag);
                }

            }
        }

        private string _fileCodeNumber;
        public string FileCodeNumber
        {
            get
            {
                return _fileCodeNumber;
            }
            set
            {
                if (_fileCodeNumber != value)
                {
                    _fileCodeNumber = value;
                    NotifyOfPropertyChange(() => FileCodeNumber);
                }

            }
        }

        private string _patientName;
        public string PatientName
        {
            get
            {
                return _patientName;
            }
            set
            {
                if (_patientName != value)
                {
                    _patientName = value;
                    NotifyOfPropertyChange(() => PatientName);
                }

            }
        }

        private string _DOB;
        public string DOB
        {
            get
            {
                return _DOB;
            }
            set
            {
                if (_DOB != value)
                {
                    _DOB = value;
                    NotifyOfPropertyChange(() => DOB);
                }

            }
        }

        private string _Age;
        public string Age
        {
            get
            {
                return _Age;
            }
            set
            {
                if (_Age != value)
                {
                    _Age = value;
                    NotifyOfPropertyChange(() => Age);
                }
            }
        }

        private string _gender;
        public string Gender
        {
            get
            {
                return _gender;
            }
            set
            {
                if (_gender != value)
                {
                    _gender = value;
                    NotifyOfPropertyChange(() => Gender);
                }
            }
        }

        private DateTime? _admissionDate;
        public DateTime? AdmissionDate
        {
            get
            {
                return _admissionDate;
            }
            set
            {
                if (_admissionDate != value)
                {
                    _admissionDate = value;
                    NotifyOfPropertyChange(() => AdmissionDate);
                }
            }
        }

        private string _IssueName;
        public string IssueName
        {
            get
            {
                return _IssueName;
            }
            set
            {
                _IssueName = value;
                NotifyOfPropertyChange(() => IssueName);
            }
        }

        private string _ReceiveName;
        public string ReceiveName
        {
            get
            {
                return _ReceiveName;
            }
            set
            {
                _ReceiveName = value;
                NotifyOfPropertyChange(() => ReceiveName);
            }
        }

        private long? _MedicalFileStorageCheckID;
        public long? MedicalFileStorageCheckID
        {
            get
            {
                return _MedicalFileStorageCheckID;
            }
            set
            {
                _MedicalFileStorageCheckID = value;
                NotifyOfPropertyChange(() => MedicalFileStorageCheckID);
            }
        }

        private int _parTypeOfForm;
        public int parTypeOfForm
        {
            get
            {
                return _parTypeOfForm;
            }
            set
            {
                _parTypeOfForm = value;
                NotifyOfPropertyChange(() => parTypeOfForm);
            }
        }

        //▼====: #022
        private Patient _CurPatient;
        public Patient CurPatient
        {
            get
            {
                return _CurPatient;
            }
            set
            {
                _CurPatient = value;
                NotifyOfPropertyChange(() => CurPatient);
            }
        }
        //▲====: #022

        public long? PrimaryID { get; set; }

        //▼====: #013
        public long TotalMienGiam { get; set; }
        public long PromoDiscProgID { get; set; }
        //▲====: #013

        public long TreatmentProcessID { get; set; }

        private bool _IsYHCTPrescript = false;
        public bool IsYHCTPrescript
        {
            get { return _IsYHCTPrescript; }
            set
            {
                _IsYHCTPrescript = value;
                NotifyOfPropertyChange(() => IsYHCTPrescript);
            }
        }
        private string _ImageUrl;
        public string ImageUrl
        {
            get { return _ImageUrl; }
            set
            {
                _ImageUrl = value;
                NotifyOfPropertyChange(() => ImageUrl);
            }
        }
        public void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            ID = 0;
            PatientID = 0;
            FileCodeNumber = "";
            PatientCode = "";
            PatientName = "";
            DOB = "";
            Age = "";
            Gender = "";
            AdmissionDate = null;
            IssueID = 0;
            PatientPCLReqID = 0;
            PaymentID = 0;
            FindPatient = 0;
            RegistrationID = 0;
            RegistrationDetailID = 0;
            IsAppointment = null;
            ViewByDate = false;
            DoctorStaffID = 0;
            FromDate = null;
            ToDate = null;
            V_MedProductType = 0;
            Result = "";
            TieuDeRpt = "";
            DrugDeptSellingPriceListID = 0;
            TenYCuHoaChat = "";
            TenYCuHoaChatTiengViet = "";
            PharmacySellingPriceListID = 0;
            DeptID = null;
            DeptName = "";
            DeptLocID = null;
            LocationName = "";
            TieuDeRpt1 = "";
            AppointmentID = 0;
            ServiceRecID = null;
            RepPaymentRecvID = 0;
            StaffName = "";
            strIDList = "";
            ReceiptForEachLocationPrintingMode = null;
            StaffID = 0;
            RepPaymtRecptByStaffID = 0;
            flag = 0;
            V_RegistrationType = 0;
            EchoCardioType1ImageResultFile1 = "";
            EchoCardioType1ImageResultFile2 = "";
            TransferFormID = 0;
            TransferFormType = 0;
            FindBy = 0;
            V_PCLRequestType = 0;
            IssueName = "";
            ReceiveName = "";
            MedicalFileStorageCheckID = null;
            parTypeOfForm = 0;
            PrimaryID = null;
            IntPtDiagDrInstructionID = 0;
            IsPsychotropicDrugs = false;
            IsFuncfoodsOrCosmetics = false;
            TotalMienGiam = 0;
            PromoDiscProgID = 0;
        }
    }
}
