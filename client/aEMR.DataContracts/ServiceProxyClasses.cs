/*
 *   20161231 #001 CMN: Add variable for VAT
 *   20181003 #002 TTM: Thêm cấu hình IsAllowSearchingPtByName để hạn chế tìm kiếm bằng tên (hiện tại cấu hình đang là False (True là không hạn chế, False là hạn chế)).
 *   20181004 #003 TBL: Them cau hinh IsAllowCopyDiagTrmt tao chan doan moi dua tren cu cho phep copy bao nhieu truong
 *   20181005 #004 TBL: Them cau hinh MinNumOfChar rang buoc so chu cho cac truong cua chan doan
 *   20181005 #005 TNHX: [BM0000034] Add Config PhieuChiDinhPrintingMode
 *   20181015 #006 TBL: Them cau hinh IsAllowInputDiagTrmt rang buoc truong phai nhap cua chan doan
 *   20181020 #007 TNHX: [BM0003199] Add config for HiConfirmationPrintingMode
 *   20181023 #008 TNHX: [BM0003221] Add config for Report Parameter (hospitalname, hospitaladdress, logoUrl, departmentOfHealth)
 *   20181124 #009 TTM:  Tạo cấu hình cho ID của kho BHYT
 *   20181201 #010 TNHX: [BM0005312] Add config For PrintingMode of PhieuMienGiam
 *   20190520 #011 TNHX: [BM0006874] Add config For PrintingReceiptWithDrugBill
 *   20190522 #012 TNHX: [BM0006500] Add config For PrintingPhieuChiDinhForService
 *   20190613 #013 TNHX: [BM0006826] Add config For AllowTimeUpdatePrescription
 *   20190704 #014 TNHX: Add config For DQGUnitname
 *   20190917 #015 TNHX: [BM0013247] Add config For AllowSearchInReport
 *   20191109 #016 TNHX: [BM 0013015] Add config For BlockPaymentWhenNotSettlement
 *   20191109 #017 TNHX: [BM ] Add config For ViewPrintAllImagingPCLRequestSeparate
 *   20200328 #018 TNHX: [BM ] Add config For BlockAddictiveAndPsychotropicDrugRequest
 *   20200329 #019 TNHX: [BM ] Add config For SecondExportBlockFormTheRequestForm
 *   20200519 #020 TNHX: [BM ] Add config For PrescriptionOutPtVersion + PrescriptionInPtVersion + LaboratoryResultVersion
 *   20200717 #021 TNHX: [BM ] Add config For BlockPrescriptionMaxHIPay
 *   20200807 #022 TNHX: [BM ] Add config For AllowConfirmEmergencyOutPt
 *   20200811 #023 TNHX: [BM ] Add config For PhieuNhanThuocPrintingModeInConfirmHIView
 *   20200903 #024 TNHX: [BM ] Add config For PharmacySearchByGenericName + PrintingWithoutExportPDF
 *   20200906 #025 TNHX: [BM ] Modified config BlockRegNoTicket
 *   20200926 #026 TNHX: [BM ] Add config ApplyFilterPrescriptionsHasHIPayTable
 *   20201026 #027 TNHX: [BM ] Add config DisableBtnCheckCountPatientInPt + BlockOutwardDrugFromMedDeptToClinicWhenRequestQtyDiffOutQty
 *   20201110 #028 TNHX: [BM ] Add config WhichHospitalUseThisApp
 *   20201117 #029 TNHX: [BM ] Add config NumDayHIAgreeToPayAfterHIExpiresInPt
 *   20201127 #030 TNHX: [BM ] Add config AutoGetHICardDataFromHIPortal
 *   20201211 #031 TNHX: [BM ] Add config AutoCreatePACWorklist + PACSAPIAddress + PACUserName + PACPassword + BlockInteractionSeverityLevelInPt + FilterDoctorByDeptResponsibilitiesInPt
 *   20201218 #032 TNHX: [BM ] Add config ApplyTemp12Version6556
 *   20210206 #033 TNHX: [BM ] Add config NgayNhapLaiTDK
 *   20210223 #034 TNHX: 214 Add config ThuocDuocXuatThapPhan
 *   20210228 #035 TNHX: 219 Add config AllowFirstHIExaminationWithoutPay
 *   20210323 #036 TNHX: 240 Add config AutoSavePhysicalExamination
 *   20210411 #037 TNHX:  Add config AllowReSelectRoomWhenLeaveDept
 *   20210430 #038 TNHX:  Add config ListICDShowAdvisoryVotes
 *   20210710 #039 TNHX:  260 Add config AllowToBorrowDoctorAccount
 *   20210722 #040 TNHX:  Add config AgeMustHasDHST
 *   20210805 #041 TNHX:  428 Add config RefGenDrugCatID_2ForDrug
 *   20210921 #042 TNHX:  436 Add config AutoAddBedService
 *   20210925 #043 QTD:  Add config EnableCheckboxXCD
 *   20220103 #044 TNHX:  Add config AllowEditDiagnosisFinalForPatientCOVID
 *   20220225 #045 QTD:  Add config TimeForAllowUpdateMedicalInstruction
 *   20220329 #046 QTD:  Add config for enable QMS for PCL/Prescription
 *   20220414 #047 TNHX: Add config MinimumPopulateDelay For time delay search drug in outpt
 *   20220416 #048 DatTB: Thêm cấu hình xác nhận hoãn tạm ứng
 *   20220521 #049 BaoLQ: Thêm cấu hình in toa thuốc khi xem mẫu 12
 *   20220530 #050 BLQ: Thêm cấu hình thời gian thao tác của bác sĩ
 *   20220815 #051 BLQ: Thêm Cấu hình check thông tin bệnh nhân khi lưu toa thuốc điều trị ngoại trú
 *   20220829 #052 TNHX: Thêm cấu hình số ngày tìm kiếm mặc định cho KSK NumDayFindOutRegistrationMedicalExamination
 *   20220903 #053 BLQ: Thêm cấu hình in mẫu 12 màn hình duyệt toa
 *   20220922 #054 BLQ: Thêm cấu hình số lượng thuốc cho phép khi cấp toa
 *   20220924 #055 BLQ: Thêm cấu hình tách toa thuốc mua ngoài
 *   20221008 #056 TNHX: 2344 Thêm cấu hình load dsach mã thẻ ND70 (TT_5149_List_HIPCode)
 *   20221010 #057 QTD: Thêm cấu hình ẩn tạo mã HSBA nội trú
 *   20221018 #058 QTD: Thêm cấu hình in biên lai và phiếu chỉ định khi lưu và trả tiền
 *   20221020 #059 QTD: Thêm cấu hình in mẫu 01/KBCB màn hình Xuất viện/Ra viện
 *   20221028 #060 BLQ: Thêm Cấu hình triệu chứng không tự đẩy qua màn hình đề nghị nhập viện
 *   20221128 #061 TNHX: Thêm cấu hình đường dẫn kết quả xét nghiệm chữ ký số + cấu hình user/pass cho ftp
 *   20221205 #062 QTD: Thêm cấu hình HIAPI và đẩy cổng tự động
 *                      + Tách cấu hình Ngoại trú - Nội trú
 *   20221212 #063 QTD: Thêm cấu hình cho phép nhập y lệnh diễn tiến không có Thuốc/CLS/DV Nội trú
 *   20221209 #064 TNHX: 994 Thêm trường cho bsi sử dụng để đăng nhập đơn thuốc điện tử + Cấu hình đăng nhập của bệnh viện
 *   20221228 #065 QTD: Thêm cấu hình chỉ định gói dịch vụ từ màn hình khám bệnh bác sĩ
 *   20230108 #066 TNHX: 994 Thêm cấu hình đẩy dữ liệu đơn thuốc điện tử khi xác nhận BHYT - ngoại trú
 *   20230103 #067 QTD  Thêm cấu hình lập phiếu dự trù màn hình Lập phiếu lĩnh VPP/VTTH
 *   20230211 #068 BLQ: Thêm cấu hình số toa thuốc điện tử cho phép xác nhận 1 lần
 *   20230531 #069 QTD: Thêm cấu hình màn hình quản lý danh mục
 *   20230603 #070 DatTB: Thêm chức năng song ngữ mẫu kết quả xét nghiệm
 *   20230617 #071 DatTB: Thêm cấu hình tên BV viết tắt
 *   20230619 #072 QTD: Thêm cấu hình URL API cho SMS XN
 *   20230706 #073 DatTB: Thêm cấu hình cho quét QRCode của CCCD
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace aEMR.DataContracts
{
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "PharmacyConfigElement", Namespace = "http://schemas.datacontract.org/2004/07/eHCMS.Configurations")]
    public partial class PharmacyConfigElement : object, System.ComponentModel.INotifyPropertyChanged
    {
        //▼====: #024
        private bool _PharmacySearchByGenericName;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool PharmacySearchByGenericName
        {
            get
            {
                return _PharmacySearchByGenericName;
            }
            set
            {
                if (_PharmacySearchByGenericName.Equals(value) != true)
                {
                    _PharmacySearchByGenericName = value;
                    RaisePropertyChanged("PharmacySearchByGenericName");
                }
            }
        }
        //▲====: #024
        private double _PharmacyDefaultVATInward;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double PharmacyDefaultVATInward
        {
            get { return _PharmacyDefaultVATInward; }
            set
            {
                _PharmacyDefaultVATInward = value;
                RaisePropertyChanged("PharmacyDefaultVATInward");
            }
        }
        private int _PharmacyCountMoneyIndependent;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int PharmacyCountMoneyIndependent
        {
            get { return _PharmacyCountMoneyIndependent; }
            set
            {
                _PharmacyCountMoneyIndependent = value;
                RaisePropertyChanged("PharmacyCountMoneyIndependent");
            }
        }
        private int _AllowedPharmacyChangeHIPrescript;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int AllowedPharmacyChangeHIPrescript
        {
            get { return _AllowedPharmacyChangeHIPrescript; }
            set
            {
                _AllowedPharmacyChangeHIPrescript = value;
                RaisePropertyChanged("AllowedPharmacyChangeHIPrescript");
            }
        }

        private int _allowTimeUpdateOutInvoice;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int AllowTimeUpdateOutInvoice
        {
            get { return _allowTimeUpdateOutInvoice; }
            set
            {
                _allowTimeUpdateOutInvoice = value;
                RaisePropertyChanged("AllowTimeUpdateOutInvoice");
            }
        }

        private bool _onlyRoundResultForOutward;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool OnlyRoundResultForOutward
        {
            get { return _onlyRoundResultForOutward; }
            set
            {
                _onlyRoundResultForOutward = value;
                RaisePropertyChanged("OnlyRoundResultForOutward");
            }
        }
        //▼====== #009
        private int _HIStorageID;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int HIStorageID
        {
            get { return _HIStorageID; }
            set
            {
                _HIStorageID = value;
                RaisePropertyChanged("HIStorageID");
            }
        }
        //▲====== #009
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        private bool _CalForPriceProfitScale;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool CalForPriceProfitScale
        {
            get { return _CalForPriceProfitScale; }
            set
            {
                _CalForPriceProfitScale = value;
                RaisePropertyChanged("CalForPriceProfitScale");
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "MedDeptConfigElement", Namespace = "http://schemas.datacontract.org/2004/07/eHCMS.Configurations")]
    public partial class MedDeptConfigElement : object, System.ComponentModel.INotifyPropertyChanged
    {

        private bool _autoCreateMedCode;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool AutoCreateMedCode
        {
            get { return _autoCreateMedCode; }
            set
            {
                _autoCreateMedCode = value;
                RaisePropertyChanged("AutoCreateMedCode");
            }
        }


        private bool _medDeptCanGetCash;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool MedDeptCanGetCash
        {
            get { return _medDeptCanGetCash; }
            set
            {
                _medDeptCanGetCash = value;
                RaisePropertyChanged("MedDeptCanGetCash");
            }
        }

        private string _prefixCodeMedical;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PrefixCodeMedical
        {
            get { return _prefixCodeMedical; }
            set
            {
                _prefixCodeMedical = value;
                RaisePropertyChanged("PrefixCodeMedical");
            }
        }

        private string _prefixCodeMachine;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PrefixCodeMachine
        {
            get { return _prefixCodeMachine; }
            set
            {
                _prefixCodeMachine = value;
                RaisePropertyChanged("PrefixCodeMachine");
            }
        }

        private string _prefixCodeChemical;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PrefixCodeChemical
        {
            get { return _prefixCodeChemical; }
            set
            {
                _prefixCodeChemical = value;
                RaisePropertyChanged("PrefixCodeChemical");
            }
        }


        private long _IntravenousCatID;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long IntravenousCatID
        {
            get { return _IntravenousCatID; }
            set
            {
                _IntravenousCatID = value;
                RaisePropertyChanged("IntravenousCatID");
            }
        }

        private bool _CheckValueBuyPriceOnImportTempInward;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool CheckValueBuyPriceOnImportTempInward
        {
            get
            {
                return _CheckValueBuyPriceOnImportTempInward;
            }
            set
            {
                _CheckValueBuyPriceOnImportTempInward = value;
                RaisePropertyChanged("CheckValueBuyPriceOnImportTempInward");
            }
        }

        private bool _IsEnableMedSubStorage;
        [DataMemberAttribute]
        public bool IsEnableMedSubStorage
        {
            get
            {
                return _IsEnableMedSubStorage;
            }
            set
            {
                _IsEnableMedSubStorage = value;
                RaisePropertyChanged("IsEnableMedSubStorage");
            }
        }

        private bool _IsComplChkPointAfterStockTake;
        [DataMemberAttribute]
        public bool IsComplChkPointAfterStockTake
        {
            get
            {
                return _IsComplChkPointAfterStockTake;
            }
            set
            {
                _IsComplChkPointAfterStockTake = value;
                RaisePropertyChanged("IsComplChkPointAfterStockTake");
            }
        }

        private bool _UseBidDetailOnInward;
        [DataMemberAttribute]
        public bool UseBidDetailOnInward
        {
            get
            {
                return _UseBidDetailOnInward;
            }
            set
            {
                _UseBidDetailOnInward = value;
                RaisePropertyChanged("UseBidDetailOnInward");
            }
        }

        private bool _UseDrugDeptAs2DistinctParts;
        [DataMemberAttribute]
        public bool UseDrugDeptAs2DistinctParts
        {
            get
            {
                return _UseDrugDeptAs2DistinctParts;
            }
            set
            {
                if (_UseDrugDeptAs2DistinctParts == value)
                {
                    return;
                }
                _UseDrugDeptAs2DistinctParts = value;
                RaisePropertyChanged("UseDrugDeptAs2DistinctParts");
            }
        }

        private bool _CalForPriceProfitScale_DrugDept;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool CalForPriceProfitScale_DrugDept
        {
            get { return _CalForPriceProfitScale_DrugDept; }
            set
            {
                _CalForPriceProfitScale_DrugDept = value;
                RaisePropertyChanged("CalForPriceProfitScale_DrugDept");
            }
        }

        //▼====: #019
        private bool _SecondExportBlockFormTheRequestForm;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool SecondExportBlockFormTheRequestForm
        {
            get { return _SecondExportBlockFormTheRequestForm; }
            set
            {
                _SecondExportBlockFormTheRequestForm = value;
                RaisePropertyChanged("SecondExportBlockFormTheRequestForm");
            }
        }
        //▲====: #019
        //▼====: #027
        private bool _BlockOutwardDrugFromMedDeptToClinicWhenRequestQtyDiffOutQty;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool BlockOutwardDrugFromMedDeptToClinicWhenRequestQtyDiffOutQty
        {
            get { return _BlockOutwardDrugFromMedDeptToClinicWhenRequestQtyDiffOutQty; }
            set
            {
                _BlockOutwardDrugFromMedDeptToClinicWhenRequestQtyDiffOutQty = value;
                RaisePropertyChanged("BlockOutwardDrugFromMedDeptToClinicWhenRequestQtyDiffOutQty");
            }
        }
        //▲====: #027

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        private bool _IsEnableFilterStorage;
        [DataMemberAttribute]
        public bool IsEnableFilterStorage
        {
            get
            {
                return _IsEnableFilterStorage;
            }
            set
            {
                _IsEnableFilterStorage = value;
                RaisePropertyChanged("IsEnableFilterStorage");
            }
        }
    }


    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "ClinicDeptConfigElement", Namespace = "http://schemas.datacontract.org/2004/07/eHCMS.Configurations")]
    public partial class ClinicDeptConfigElement : object, System.ComponentModel.INotifyPropertyChanged
    {
        //▼====: #034
        private string _ThuocDuocXuatThapPhan;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ThuocDuocXuatThapPhan
        {
            get { return _ThuocDuocXuatThapPhan; }
            set
            {
                _ThuocDuocXuatThapPhan = value;
                RaisePropertyChanged("ThuocDuocXuatThapPhan");
            }
        }
        //▲====: #034

        private bool _updateOutwardToPatientNew;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool UpdateOutwardToPatientNew
        {
            get { return _updateOutwardToPatientNew; }
            set
            {
                _updateOutwardToPatientNew = value;
                RaisePropertyChanged("UpdateOutwardToPatientNew");
            }
        }

        private bool _requireDoctorAndDateForMed;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool RequireDoctorAndDateForMed
        {
            get { return _requireDoctorAndDateForMed; }
            set
            {
                _requireDoctorAndDateForMed = value;
                RaisePropertyChanged("RequireDoctorAndDateForMed");
            }
        }

        private bool _requireDoctorAndDateForMat;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool RequireDoctorAndDateForMat
        {
            get { return _requireDoctorAndDateForMat; }
            set
            {
                _requireDoctorAndDateForMat = value;
                RaisePropertyChanged("RequireDoctorAndDateForMat");
            }
        }

        private bool _requireDoctorAndDateForLab;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool RequireDoctorAndDateForLab
        {
            get { return _requireDoctorAndDateForLab; }
            set
            {
                _requireDoctorAndDateForLab = value;
                RaisePropertyChanged("RequireDoctorAndDateForLab");
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        private long _FileEmployeeID;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long FileEmployeeID
        {
            get
            {
                return _FileEmployeeID;
            }
            set
            {
                if (ReferenceEquals(_FileEmployeeID, value) != true)
                {
                    _FileEmployeeID = value;
                    RaisePropertyChanged("FileEmployeeID");
                }
            }
        }
        //20181115 TBL: Cau hinh lam tron SL xuat noi tru
        private bool _LamTronSLXuatNoiTru;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool LamTronSLXuatNoiTru
        {
            get { return _LamTronSLXuatNoiTru; }
            set
            {
                _LamTronSLXuatNoiTru = value;
                RaisePropertyChanged("LamTronSLXuatNoiTru");
            }
        }

        private bool _RoundDownInwardOutQty;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool RoundDownInwardOutQty
        {
            get { return _RoundDownInwardOutQty; }
            set
            {
                _RoundDownInwardOutQty = value;
                RaisePropertyChanged("RoundDownInwardOutQty");
            }
        }

        private string _ProductTypeNotDocAndDateReq;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ProductTypeNotDocAndDateReq
        {
            get { return _ProductTypeNotDocAndDateReq; }
            set
            {
                _ProductTypeNotDocAndDateReq = value;
                RaisePropertyChanged("ProductTypeNotDocAndDateReq");
            }
        }

        private short _DrugDeptOutDrugExpiryDateRule;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public short DrugDeptOutDrugExpiryDateRule
        {
            get { return _DrugDeptOutDrugExpiryDateRule; }
            set
            {
                _DrugDeptOutDrugExpiryDateRule = value;
                RaisePropertyChanged("DrugDeptOutDrugExpiryDateRule");
            }
        }

        private byte _LoadOutwardTempBy;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public byte LoadOutwardTempBy
        {
            get { return _LoadOutwardTempBy; }
            set
            {
                _LoadOutwardTempBy = value;
                RaisePropertyChanged("LoadOutwardTempBy");
            }
        }
    }


    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "OutRegisConfigElement", Namespace = "http://schemas.datacontract.org/2004/07/eHCMS.Configurations")]
    public partial class OutRegisConfigElement : object, System.ComponentModel.INotifyPropertyChanged
    {
        //▼====: #052
        private int _NumDayFindOutRegistrationMedicalExamination;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int NumDayFindOutRegistrationMedicalExamination
        {
            get { return _NumDayFindOutRegistrationMedicalExamination; }
            set
            {
                if (_NumDayFindOutRegistrationMedicalExamination != value)
                {
                    _NumDayFindOutRegistrationMedicalExamination = value;
                    RaisePropertyChanged("NumDayFindOutRegistrationMedicalExamination");
                }
            }
        }
        //▲====: #052
        //▼====: #024
        private bool _PrintingWithoutExportPDF;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool PrintingWithoutExportPDF
        {
            get { return _PrintingWithoutExportPDF; }
            set
            {
                if (_PrintingWithoutExportPDF != value)
                {
                    _PrintingWithoutExportPDF = value;
                    RaisePropertyChanged("PrintingWithoutExportPDF");
                }
            }
        }
        //▲====: #024
        private bool _allowToChooseTypeOf01Form;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool AllowToChooseTypeOf01Form
        {
            get { return _allowToChooseTypeOf01Form; }
            set
            {
                _allowToChooseTypeOf01Form = value;
                RaisePropertyChanged("AllowToChooseTypeOf01Form");
            }
        }

        private bool _assignSequenceNumberManually;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool AssignSequenceNumberManually
        {
            get { return _assignSequenceNumberManually; }
            set
            {
                _assignSequenceNumberManually = value;
                RaisePropertyChanged("AssignSequenceNumberManually");
            }
        }

        private int _maxNumberOfServicesAllowForOutPatient;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int MaxNumberOfServicesAllowForOutPatient
        {
            get { return _maxNumberOfServicesAllowForOutPatient; }
            set
            {
                _maxNumberOfServicesAllowForOutPatient = value;
                RaisePropertyChanged("MaxNumberOfServicesAllowForOutPatient");
            }
        }

        private bool _AutoLocationAllocation;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool AutoLocationAllocation
        {
            get { return _AutoLocationAllocation; }
            set
            {
                _AutoLocationAllocation = value;
                RaisePropertyChanged("AutoLocationAllocation");
            }
        }

        private bool _IsPerformingTMVFunctionsA;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsPerformingTMVFunctionsA
        {
            get { return _IsPerformingTMVFunctionsA; }
            set
            {
                _IsPerformingTMVFunctionsA = value;
                RaisePropertyChanged("IsPerformingTMVFunctionsA");
            }
        }

        private bool _CheckDoctorStaffID;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool CheckDoctorStaffID
        {
            get { return _CheckDoctorStaffID; }
            set
            {
                _CheckDoctorStaffID = value;
                RaisePropertyChanged("CheckDoctorStaffID");
            }
        }

        private int _DayStartAndEndFindAppointment;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int DayStartAndEndFindAppointment
        {
            get { return _DayStartAndEndFindAppointment; }
            set
            {
                _DayStartAndEndFindAppointment = value;
                RaisePropertyChanged("DayStartAndEndFindAppointment");
            }
        }

        //▼====: #022
        private bool _AllowConfirmEmergencyOutPt;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool AllowConfirmEmergencyOutPt
        {
            get { return _AllowConfirmEmergencyOutPt; }
            set
            {
                if (_AllowConfirmEmergencyOutPt != value)
                {
                    _AllowConfirmEmergencyOutPt = value;
                    RaisePropertyChanged("AllowConfirmEmergencyOutPt");
                }
            }
        }
        //▲====: #022

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }



    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "InRegisConfigElement", Namespace = "http://schemas.datacontract.org/2004/07/eHCMS.Configurations")]
    public partial class InRegisConfigElement : object, System.ComponentModel.INotifyPropertyChanged
    {
        //▼====: #044
        private bool _AllowEditDiagnosisFinalForPatientCOVID;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool AllowEditDiagnosisFinalForPatientCOVID
        {
            get { return _AllowEditDiagnosisFinalForPatientCOVID; }
            set
            {
                if (_AllowEditDiagnosisFinalForPatientCOVID != value)
                {
                    _AllowEditDiagnosisFinalForPatientCOVID = value;
                    RaisePropertyChanged("AllowEditDiagnosisFinalForPatientCOVID");
                }
            }
        }
        //▲====: #044
        //▼====: #041
        private long _RefGenDrugCatID_2ForDrug;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long RefGenDrugCatID_2ForDrug
        {
            get { return _RefGenDrugCatID_2ForDrug; }
            set
            {
                if (_RefGenDrugCatID_2ForDrug != value)
                {
                    _RefGenDrugCatID_2ForDrug = value;
                    RaisePropertyChanged("RefGenDrugCatID_2ForDrug");
                }
            }
        }
        //▲====: #041
        //▼====: #029
        private int _NumDayHIAgreeToPayAfterHIExpiresInPt;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int NumDayHIAgreeToPayAfterHIExpiresInPt
        {
            get { return _NumDayHIAgreeToPayAfterHIExpiresInPt; }
            set
            {
                if (_NumDayHIAgreeToPayAfterHIExpiresInPt != value)
                {
                    _NumDayHIAgreeToPayAfterHIExpiresInPt = value;
                    RaisePropertyChanged("NumDayHIAgreeToPayAfterHIExpiresInPt");
                }
            }
        }
        //▲====: #029
        //▼====: #027
        private bool _DisableBtnCheckCountPatientInPt;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool DisableBtnCheckCountPatientInPt
        {
            get { return _DisableBtnCheckCountPatientInPt; }
            set
            {
                if (_DisableBtnCheckCountPatientInPt != value)
                {
                    _DisableBtnCheckCountPatientInPt = value;
                    RaisePropertyChanged("DisableBtnCheckCountPatientInPt");
                }
            }
        }
        //▲====: #027
        //▼====: #016
        private bool _BlockPaymentWhenNotSettlement;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool BlockPaymentWhenNotSettlement
        {
            get { return _BlockPaymentWhenNotSettlement; }
            set
            {
                _BlockPaymentWhenNotSettlement = value;
                RaisePropertyChanged("BlockPaymentWhenNotSettlement");
            }
        }
        //▲====: #016

        private bool _addMedProductToBillDirectly;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool AddMedProductToBillDirectly
        {
            get { return _addMedProductToBillDirectly; }
            set
            {
                _addMedProductToBillDirectly = value;
                RaisePropertyChanged("AddMedProductToBillDirectly");
            }
        }

        private bool _onlyInsertToCashAdvance;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool OnlyInsertToCashAdvance
        {
            get { return _onlyInsertToCashAdvance; }
            set
            {
                _onlyInsertToCashAdvance = value;
                RaisePropertyChanged("OnlyInsertToCashAdvance");
            }
        }

        private bool _dischargeInPtWith2Steps;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool DischargeInPtWith2Steps 
        {
            get { return _dischargeInPtWith2Steps; }
            set
            {
                _dischargeInPtWith2Steps = value;
                RaisePropertyChanged("DischargeInPtWith2Steps");
            }
        }

        private bool _excludeDeptAAndB;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool ExcludeDeptAAndB
        {
            get { return _excludeDeptAAndB; }
            set
            {
                _excludeDeptAAndB = value;
                RaisePropertyChanged("ExcludeDeptAAndB");
            }
        }


        private int _numOfDayAllowSaveBillAfterDischarge;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int NumOfDayAllowSaveBillAfterDischarge
        {
            get { return _numOfDayAllowSaveBillAfterDischarge; }
            set
            {
                _numOfDayAllowSaveBillAfterDischarge = value;
                RaisePropertyChanged("NumOfDayAllowSaveBillAfterDischarge");
            }
        } 

        //HPT: Số ngày chờ tối đa của đăng ký vãng lai
        private int _numOfDayAllowPending_CasualReg;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int NumOfDayAllowPending_CasualReg
        {
            get { return _numOfDayAllowPending_CasualReg; }
            set
            {
                _numOfDayAllowPending_CasualReg = value;
                RaisePropertyChanged("NumOfDayAllowPending_CasualReg");
            }
        }
        //HPT: Số ngày chờ tối đa của đăng ký Tiền Giải Phẫu
        private int _numOfDayAllowPending_PreOpReg;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int NumOfDayAllowPending_PreOpReg
        {
            get { return _numOfDayAllowPending_PreOpReg; }
            set
            {
                _numOfDayAllowPending_PreOpReg = value;
                RaisePropertyChanged("NumOfDayAllowPending_PreOpReg");
            }
        }
        
        private int _CheckToLockReportedRegistration = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CheckToLockReportedRegistration
        {
            get { return _CheckToLockReportedRegistration; }
            set
            {
                _CheckToLockReportedRegistration = value;
                RaisePropertyChanged("CheckToLockReportedRegistration");
            }
        }
        private decimal _maxHIPayForHighTechServiceBill;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal MaxHIPayForHighTechServiceBill
        {
            get { return _maxHIPayForHighTechServiceBill; }
            set
            {
                _maxHIPayForHighTechServiceBill = value;
                RaisePropertyChanged("MaxHIPayForHighTechServiceBill");
            }
        }

        private bool _AllowChildUnder6YearsOldUseHIOverDate;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool AllowChildUnder6YearsOldUseHIOverDate
        {
            get { return _AllowChildUnder6YearsOldUseHIOverDate; }
            set
            {
                _AllowChildUnder6YearsOldUseHIOverDate = value;
                RaisePropertyChanged("AllowChildUnder6YearsOldUseHIOverDate");
            }
        }

        private bool _ShowEmergInPtReExamination;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool ShowEmergInPtReExamination
        {
            get { return _ShowEmergInPtReExamination; }
            set
            {
                _ShowEmergInPtReExamination = value;
                RaisePropertyChanged("ShowEmergInPtReExamination");
            }
        }

        private long _EmerDeptID;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long EmerDeptID
        {
            get { return _EmerDeptID; }
            set
            {
                _EmerDeptID = value;
                RaisePropertyChanged("EmerDeptID");
            }
        }

        private int _NumOfOverDaysInDischargeForm;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int NumOfOverDaysInDischargeForm
        {
            get
            {
                return _NumOfOverDaysInDischargeForm;
            }
            set
            {
                _NumOfOverDaysInDischargeForm = value;
                RaisePropertyChanged("NumOfOverDaysInDischargeForm");
            }
        }

        private bool _CheckDischargeDate;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool CheckDischargeDate
        {
            get
            {
                return _CheckDischargeDate;
            }
            set
            {
                _CheckDischargeDate = value;
                RaisePropertyChanged("CheckDischargeDate");
            }
        }

        private int _NumOfOverDaysForMedicalInstructDate;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int NumOfOverDaysForMedicalInstructDate
        {
            get
            {
                return _NumOfOverDaysForMedicalInstructDate;
            }
            set
            {
                _NumOfOverDaysForMedicalInstructDate = value;
                RaisePropertyChanged("NumOfOverDaysForMedicalInstructDate");
            }
        }

        private bool _CheckMedicalInstructDate;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool CheckMedicalInstructDate
        {
            get
            {
                return _CheckMedicalInstructDate;
            }
            set
            {
                _CheckMedicalInstructDate = value;
                RaisePropertyChanged("CheckMedicalInstructDate");
            }
        }

        private bool _NotCountHIOnPackItem;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool NotCountHIOnPackItem
        {
            get
            {
                return _NotCountHIOnPackItem;
            }
            set
            {
                _NotCountHIOnPackItem = value;
                RaisePropertyChanged("NotCountHIOnPackItem");
            }
        }

        private bool _Use_SaveRegisThenPay;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Use_SaveRegisThenPay
        {
            get { return _Use_SaveRegisThenPay; }
            set
            {
                _Use_SaveRegisThenPay = value;
                RaisePropertyChanged("Use_SaveRegisThenPay");
            }
        }

        private bool _ShowMessageBoxForLockReportedRegistration;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool ShowMessageBoxForLockReportedRegistration
        {
            get { return _ShowMessageBoxForLockReportedRegistration; }
            set
            {
                _ShowMessageBoxForLockReportedRegistration = value;
                RaisePropertyChanged("ShowMessageBoxForLockReportedRegistration");
            }
        }

        private int _MergerPatientRegistration;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int MergerPatientRegistration
        {
            get { return _MergerPatientRegistration; }
            set
            {
                _MergerPatientRegistration = value;
                RaisePropertyChanged("MergerPatientRegistration");
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "ConsultationConfigElement", Namespace = "http://schemas.datacontract.org/2004/07/eHCMS.Configurations")]
    public partial class ConsultationConfigElement : object, System.ComponentModel.INotifyPropertyChanged
    {
        //▼====: #056
        private string _TT5149ListHIPCode;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string TT5149ListHIPCode
        {
            get { return _TT5149ListHIPCode; }
            set
            {
                if (ReferenceEquals(_TT5149ListHIPCode, value) != true)
                {
                    _TT5149ListHIPCode = value;
                    RaisePropertyChanged("TT5149ListHIPCode");
                }
            }
        }
        //▲====: #056
        //▼====: #038
        private string _ListICDShowAdvisoryVotes;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ListICDShowAdvisoryVotes
        {
            get { return _ListICDShowAdvisoryVotes; }
            set
            {
                if (ReferenceEquals(_ListICDShowAdvisoryVotes, value) != true)
                {
                    _ListICDShowAdvisoryVotes = value;
                    RaisePropertyChanged("ListICDShowAdvisoryVotes");
                }
            }
        }
        //▲====: #038
        //▼====: #020
        private int _PrescriptionOutPtVersion;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int PrescriptionOutPtVersion
        {
            get { return _PrescriptionOutPtVersion; }
            set
            {
                if (ReferenceEquals(_PrescriptionOutPtVersion, value) != true)
                {
                    _PrescriptionOutPtVersion = value;
                    RaisePropertyChanged("PrescriptionOutPtVersion");
                }
            }
        }
        private int _PrescriptionInPtVersion;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int PrescriptionInPtVersion
        {
            get { return _PrescriptionInPtVersion; }
            set
            {
                if (ReferenceEquals(_PrescriptionInPtVersion, value) != true)
                {
                    _PrescriptionInPtVersion = value;
                    RaisePropertyChanged("PrescriptionInPtVersion");
                }
            }
        }
        private int _LaboratoryResultVersion;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int LaboratoryResultVersion
        {
            get { return _LaboratoryResultVersion; }
            set
            {
                if (ReferenceEquals(_LaboratoryResultVersion, value) != true)
                {
                    _LaboratoryResultVersion = value;
                    RaisePropertyChanged("LaboratoryResultVersion");
                }
            }
        }
        //▲====: #020
        private bool _defSearchByGenericName;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool DefSearchByGenericName
        {
            get { return _defSearchByGenericName; }
            set
            {
                _defSearchByGenericName = value;
                RaisePropertyChanged("DefSearchByGenericName");
            }
        }

        private bool _AllowToUpdateDiagnosisIntoPCLReq;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool AllowToUpdateDiagnosisIntoPCLReq
        {
            get { return _AllowToUpdateDiagnosisIntoPCLReq; }
            set
            {
                _AllowToUpdateDiagnosisIntoPCLReq = value;
                RaisePropertyChanged("AllowToUpdateDiagnosisIntoPCLReq");
            }
        }     

        /*TMA 25/11/2017*/
        private bool _IsSeparatePsychotropicPrescription;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsSeparatePsychotropicPrescription
        {
            get { return _IsSeparatePsychotropicPrescription; }
            set
            {
                _IsSeparatePsychotropicPrescription = value;
                RaisePropertyChanged("IsSeparatePsychotropicPrescription");
            }
        }

        private bool _IsSeparatePsychotropicPrescription_Inpt;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsSeparatePsychotropicPrescription_Inpt
        {
            get { return _IsSeparatePsychotropicPrescription_Inpt; }
            set
            {
                _IsSeparatePsychotropicPrescription_Inpt = value;
                RaisePropertyChanged("IsSeparatePsychotropicPrescription_Inpt");
            }
        }
        /*TMA 25/11/2017*/

        private bool _EnableTreatmentRegimen;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool EnableTreatmentRegimen
        {
            get { return _EnableTreatmentRegimen; }
            set
            {
                _EnableTreatmentRegimen = value;
                RaisePropertyChanged("EnableTreatmentRegimen");
            }
        }
        //▼====== #002
        private bool _IsAllowSearchingPtByName;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsAllowSearchingPtByName
        {
            get { return _IsAllowSearchingPtByName; }
            set
            {
                _IsAllowSearchingPtByName = value;
                RaisePropertyChanged("IsAllowSearchingPtByName");
            }
        }
        //▲====== #002
        /*▼====: #003*/
        private int _IsAllowCopyDiagTrmt;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int IsAllowCopyDiagTrmt
        {
            get { return _IsAllowCopyDiagTrmt; }
            set
            {
                if (ReferenceEquals(_IsAllowCopyDiagTrmt, value) != true)
                {
                    _IsAllowCopyDiagTrmt = value;
                    RaisePropertyChanged("IsAllowCopyDiagTrmt");
                }
            }
        }
        /*▲====: #003*/
        /*▼====: #004*/
        private int _MinNumOfChar;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int MinNumOfChar
        {
            get { return _MinNumOfChar; }
            set
            {
                if (ReferenceEquals(_MinNumOfChar, value) != true)
                {
                    _MinNumOfChar = value;
                    RaisePropertyChanged("MinNumOfChar");
                }
            }
        }
        /*▲====: #004*/
        /*▼====: #006*/
        private int _IsAllowInputDiagTrmt;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int IsAllowInputDiagTrmt
        {
            get { return _IsAllowInputDiagTrmt; }
            set
            {
                if (ReferenceEquals(_IsAllowInputDiagTrmt, value) != true)
                {
                    _IsAllowInputDiagTrmt = value;
                    RaisePropertyChanged("IsAllowInputDiagTrmt");
                }
            }
        }
        /*▲====: #006*/

        private bool _AllowWorkingOnSunday;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool AllowWorkingOnSunday
        {
            get { return _AllowWorkingOnSunday; }
            set
            {
                _AllowWorkingOnSunday = value;
                RaisePropertyChanged("AllowWorkingOnSunday");
            }
        }

        private bool _DiagnosisTreatmentForDrug;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool DiagnosisTreatmentForDrug
        {
            get { return _DiagnosisTreatmentForDrug; }
            set
            {
                _DiagnosisTreatmentForDrug = value;
                RaisePropertyChanged("DiagnosisTreatmentForDrug");
            }
        }

        private bool _AllowSaveQuantityNotEnough;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool AllowSaveQuantityNotEnough
        {
            get { return _AllowSaveQuantityNotEnough; }
            set
            {
                _AllowSaveQuantityNotEnough = value;
                RaisePropertyChanged("AllowSaveQuantityNotEnough");
            }
        }

        private bool _AllowBlockContraIndicator;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool AllowBlockContraIndicator
        {
            get { return _AllowBlockContraIndicator; }
            set
            {
                if (_AllowBlockContraIndicator != value)
                {
                    _AllowBlockContraIndicator = value;
                    RaisePropertyChanged("AllowBlockContraIndicator");
                }
            }
        }

        private bool _AllowBlockContraIndicatorInDay;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool AllowBlockContraIndicatorInDay
        {
            get { return _AllowBlockContraIndicatorInDay; }
            set
            {
                if (_AllowBlockContraIndicatorInDay != value)
                {
                    _AllowBlockContraIndicatorInDay = value;
                    RaisePropertyChanged("AllowBlockContraIndicatorInDay");
                }
            }
        }

        private bool _CheckToaThuocBiTrungTheoHoatChat;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool CheckToaThuocBiTrungTheoHoatChat
        {
            get { return _CheckToaThuocBiTrungTheoHoatChat; }
            set
            {
                if (_CheckToaThuocBiTrungTheoHoatChat != value)
                {
                    _CheckToaThuocBiTrungTheoHoatChat = value;
                    RaisePropertyChanged("CheckToaThuocBiTrungTheoHoatChat");
                }
            }
        }

        private bool _CheckToaThuocBiTrungNhomThuoc;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool CheckToaThuocBiTrungNhomThuoc
        {
            get { return _CheckToaThuocBiTrungNhomThuoc; }
            set
            {
                if (_CheckToaThuocBiTrungNhomThuoc != value)
                {
                    _CheckToaThuocBiTrungNhomThuoc = value;
                    RaisePropertyChanged("CheckToaThuocBiTrungNhomThuoc");
                }
            }
        }

        private bool _KiemTraQuanHeHoatChat;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool KiemTraQuanHeHoatChat
        {
            get { return _KiemTraQuanHeHoatChat; }
            set
            {
                if (_KiemTraQuanHeHoatChat != value)
                {
                    _KiemTraQuanHeHoatChat = value;
                    RaisePropertyChanged("KiemTraQuanHeHoatChat");
                }
            }
        }

        //▼====: #013
        private int _AllowTimeUpdatePrescription;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int AllowTimeUpdatePrescription
        {
            get { return _AllowTimeUpdatePrescription; }
            set
            {
                if (_AllowTimeUpdatePrescription != value)
                {
                    _AllowTimeUpdatePrescription = value;
                    RaisePropertyChanged("AllowTimeUpdatePrescription");
                }
            }
        }
        //▲====: #013

        private bool _AppointmentAuto;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool AppointmentAuto
        {
            get { return _AppointmentAuto; }
            set
            {
                if (_AppointmentAuto != value)
                {
                    _AppointmentAuto = value;
                    RaisePropertyChanged("AppointmentAuto");
                }
            }
        }

        private string _ParamAppointmentAuto;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ParamAppointmentAuto
        {
            get
            {
                return _ParamAppointmentAuto;
            }
            set
            {
                if (_ParamAppointmentAuto != value)
                {
                    _ParamAppointmentAuto = value;
                    RaisePropertyChanged("ParamAppointmentAuto");
                }
            }
        }

        private bool _CheckedTreatmentRegimen;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool CheckedTreatmentRegimen
        {
            get { return _CheckedTreatmentRegimen; }
            set
            {
                _CheckedTreatmentRegimen = value;
                RaisePropertyChanged("CheckedTreatmentRegimen");
            }
        }

        private long[] _HealthExamDeptLocIDArray;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long[] HealthExamDeptLocIDArray
        {
            get { return _HealthExamDeptLocIDArray; }
            set
            {
                if (_HealthExamDeptLocIDArray != value)
                {
                    _HealthExamDeptLocIDArray = value;
                    RaisePropertyChanged("HealthExamDeptLocIDArray");
                }
            }
        }
        private long _ModeShowInforDrugForAutoCompleteInstruction;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long ModeShowInforDrugForAutoCompleteInstruction
        {
            get { return _ModeShowInforDrugForAutoCompleteInstruction; }
            set
            {
                if (_ModeShowInforDrugForAutoCompleteInstruction != value)
                {
                    _ModeShowInforDrugForAutoCompleteInstruction = value;
                    RaisePropertyChanged("ModeShowInforDrugForAutoCompleteInstruction");
                }
            }
        }

        private bool _UseOnlyDailyDiagnosis;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool UseOnlyDailyDiagnosis
        {
            get { return _UseOnlyDailyDiagnosis; }
            set
            {
                if (_UseOnlyDailyDiagnosis != value)
                {
                    _UseOnlyDailyDiagnosis = value;
                    RaisePropertyChanged("UseOnlyDailyDiagnosis");
                }
            }
        }

        private int _LevelWarningWhenCreateNewAndCopy;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int LevelWarningWhenCreateNewAndCopy
        {
            get { return _LevelWarningWhenCreateNewAndCopy; }
            set
            {
                _LevelWarningWhenCreateNewAndCopy = value;
                RaisePropertyChanged("LevelWarningWhenCreateNewAndCopy");
            }
        }

        private decimal _PrescriptionMaxHIPay;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal PrescriptionMaxHIPay
        {
            get { return _PrescriptionMaxHIPay; }
            set
            {
                if (ReferenceEquals(_PrescriptionMaxHIPay, value) != true)
                {
                    _PrescriptionMaxHIPay = value;
                    RaisePropertyChanged("PrescriptionMaxHIPay");
                }
            }
        }

        private int _IsAllowCopyDiagTrmtInstruction;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int IsAllowCopyDiagTrmtInstruction
        {
            get { return _IsAllowCopyDiagTrmtInstruction; }
            set
            {
                if (ReferenceEquals(_IsAllowCopyDiagTrmtInstruction, value) != true)
                {
                    _IsAllowCopyDiagTrmtInstruction = value;
                    RaisePropertyChanged("IsAllowCopyDiagTrmtInstruction");
                }
            }
        }

        private int _CheckMonitoringVitalSigns;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CheckMonitoringVitalSigns
        {
            get { return _CheckMonitoringVitalSigns; }
            set
            {
                if (_CheckMonitoringVitalSigns != value)
                {
                    _CheckMonitoringVitalSigns = value;
                    RaisePropertyChanged("CheckMonitoringVitalSigns");
                }
            }
        }

        private int _IsAllowCopyInstruction;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int IsAllowCopyInstruction
        {
            get { return _IsAllowCopyInstruction; }
            set
            {
                if (ReferenceEquals(_IsAllowCopyInstruction, value) != true)
                {
                    _IsAllowCopyInstruction = value;
                    RaisePropertyChanged("IsAllowCopyInstruction");
                }
            }
        }

        private string _ConsultMinTimeReqBeforeExit;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ConsultMinTimeReqBeforeExit
        {
            get { return _ConsultMinTimeReqBeforeExit; }
            set
            {
                if (_ConsultMinTimeReqBeforeExit != value)
                {
                    _ConsultMinTimeReqBeforeExit = value;
                    RaisePropertyChanged("ConsultMinTimeReqBeforeExit");
                }
            }
        }

        private bool _IsCheckApmtOnPrescription;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsCheckApmtOnPrescription
        {
            get { return _IsCheckApmtOnPrescription; }
            set
            {
                if (_IsCheckApmtOnPrescription != value)
                {
                    _IsCheckApmtOnPrescription = value;
                    RaisePropertyChanged("IsCheckApmtOnPrescription");
                }
            }
        }

        private double _PercentPrescriptionForHI;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double PercentPrescriptionForHI
        {
            get { return _PercentPrescriptionForHI; }
            set
            {
                if (ReferenceEquals(_PercentPrescriptionForHI, value) != true)
                {
                    _PercentPrescriptionForHI = value;
                    RaisePropertyChanged("PercentPrescriptionForHI");
                }
            }
        }

        //▼====: #021
        private bool _BlockPrescriptionMaxHIPay;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool BlockPrescriptionMaxHIPay
        {
            get { return _BlockPrescriptionMaxHIPay; }
            set
            {
                if (_BlockPrescriptionMaxHIPay != value)
                {
                    _BlockPrescriptionMaxHIPay = value;
                    RaisePropertyChanged("BlockPrescriptionMaxHIPay");
                }
            }
        }
        //▲====: #021
        //▼====: #026
        private bool _ApplyFilterPrescriptionsHasHIPayTable;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool ApplyFilterPrescriptionsHasHIPayTable
        {
            get { return _ApplyFilterPrescriptionsHasHIPayTable; }
            set
            {
                if (_ApplyFilterPrescriptionsHasHIPayTable != value)
                {
                    _ApplyFilterPrescriptionsHasHIPayTable = value;
                    RaisePropertyChanged("ApplyFilterPrescriptionsHasHIPayTable");
                }
            }
        }
        //▲====: #026
        //▼====: #031
        private long _BlockInteractionSeverityLevelInPt;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long BlockInteractionSeverityLevelInPt
        {
            get { return _BlockInteractionSeverityLevelInPt; }
            set
            {
                if (_BlockInteractionSeverityLevelInPt != value)
                {
                    _BlockInteractionSeverityLevelInPt = value;
                    RaisePropertyChanged("BlockInteractionSeverityLevelInPt");
                }
            }
        }
        private bool _FilterDoctorByDeptResponsibilitiesInPt;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool FilterDoctorByDeptResponsibilitiesInPt
        {
            get { return _FilterDoctorByDeptResponsibilitiesInPt; }
            set
            {
                if (_FilterDoctorByDeptResponsibilitiesInPt != value)
                {
                    _FilterDoctorByDeptResponsibilitiesInPt = value;
                    RaisePropertyChanged("FilterDoctorByDeptResponsibilitiesInPt");
                }
            }
        }
        //▲====: #031
        private bool _CheckToaThuocBiTrungTheoHoatChatVaNgayThuocBaoHiem;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool CheckToaThuocBiTrungTheoHoatChatVaNgayThuocBaoHiem
        {
            get { return _CheckToaThuocBiTrungTheoHoatChatVaNgayThuocBaoHiem; }
            set
            {
                if (_CheckToaThuocBiTrungTheoHoatChatVaNgayThuocBaoHiem != value)
                {
                    _CheckToaThuocBiTrungTheoHoatChatVaNgayThuocBaoHiem = value;
                    RaisePropertyChanged("CheckToaThuocBiTrungTheoHoatChatVaNgayThuocBaoHiem");
                }
            }
        }
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "CommonItemElement", Namespace = "http://schemas.datacontract.org/2004/07/eHCMS.Configurations")]
    public partial class CommonItemElement : object, System.ComponentModel.INotifyPropertyChanged
    {
        //▼====: #065
        private bool _IsApplyAutoCreateDTDTReportWhenConfirmHI;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsApplyAutoCreateDTDTReportWhenConfirmHI
        {
            get
            {
                return _IsApplyAutoCreateDTDTReportWhenConfirmHI;
            }
            set
            {
                _IsApplyAutoCreateDTDTReportWhenConfirmHI = value;
                RaisePropertyChanged("IsApplyAutoCreateDTDTReportWhenConfirmHI");
            }
        }
        //▲====: #065
        //▼====: #064
        private bool _ApplyDTDT;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool ApplyDTDT
        {
            get
            {
                return _ApplyDTDT;
            }
            set
            {
                _ApplyDTDT = value;
                RaisePropertyChanged("ApplyDTDT");
            }
        }
        private string _DTDTUsername;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DTDTUsername
        {
            get
            {
                return _DTDTUsername;
            }
            set
            {
                _DTDTUsername = value;
                RaisePropertyChanged("DTDTUsername");
            }
        }
        private string _DTDTPassword;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DTDTPassword
        {
            get
            {
                return _DTDTPassword;
            }
            set
            {
                _DTDTPassword = value;
                RaisePropertyChanged("DTDTPassword");
            }
        }
        private string _DonThuocQuocGiaAPIUrl;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DonThuocQuocGiaAPIUrl
        {
            get
            {
                return _DonThuocQuocGiaAPIUrl;
            }
            set
            {
                _DonThuocQuocGiaAPIUrl = value;
                RaisePropertyChanged("DonThuocQuocGiaAPIUrl");
            }
        }
        //▲====: #064
        //▼====: #061
        private string _FTPServerSighHashUrl;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FTPServerSighHashUrl
        {
            get
            {
                return _FTPServerSighHashUrl;
            }
            set
            {
                _FTPServerSighHashUrl = value;
                RaisePropertyChanged("FTPServerSighHashUrl");
            }
        }
        private string _HISSighHashSmartCAUrl;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string HISSighHashSmartCAUrl
        {
            get
            {
                return _HISSighHashSmartCAUrl;
            }
            set
            {
                _HISSighHashSmartCAUrl = value;
                RaisePropertyChanged("HISSighHashSmartCAUrl");
            }
        }
        private string _ServicePool;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ServicePool
        {
            get
            {
                return _ServicePool;
            }
            set
            {
                _ServicePool = value;
                RaisePropertyChanged("ServicePool");
            }
        }
        private string _ServiceUrl;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ServiceUrl
        {
            get
            {
                return _ServiceUrl;
            }
            set
            {
                _ServiceUrl = value;
                RaisePropertyChanged("ServiceUrl");
            }
        }
        private string _PDFFileResultSignedPath;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PDFFileResultSignedPath
        {
            get
            {
                return _PDFFileResultSignedPath;
            }
            set
            {
                _PDFFileResultSignedPath = value;
                RaisePropertyChanged("PDFFileResultSignedPath");
            }
        }
        private string _PDFFileResultToSignPath;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PDFFileResultToSignPath
        {
            get
            {
                return _PDFFileResultToSignPath;
            }
            set
            {
                _PDFFileResultToSignPath = value;
                RaisePropertyChanged("PDFFileResultToSignPath");
            }
        }
        //▲====: #061
        //▼====: #047
        [DataMemberAttribute]
        private int _MinimumPopulateDelay;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int MinimumPopulateDelay
        {
            get
            {
                return _MinimumPopulateDelay;
            }
            set
            {
                _MinimumPopulateDelay = value;
                RaisePropertyChanged("MinimumPopulateDelay");
            }
        }
        //▲====: #047
        //▼====: #042
        private bool _AutoAddBedService;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool AutoAddBedService
        {
            get
            {
                return _AutoAddBedService;
            }
            set
            {
                _AutoAddBedService = value;
                RaisePropertyChanged("AutoAddBedService");
            }
        }
        //▲====: #042
        //▼====: #040
        private long _AgeMustHasDHST;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long AgeMustHasDHST
        {
            get
            {
                return _AgeMustHasDHST;
            }
            set
            {
                _AgeMustHasDHST = value;
                RaisePropertyChanged("AgeMustHasDHST");
            }
        }
        //▲====: #040
        //▼====: #039
        private bool _AllowToBorrowDoctorAccount;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool AllowToBorrowDoctorAccount
        {
            get
            {
                return _AllowToBorrowDoctorAccount;
            }
            set
            {
                _AllowToBorrowDoctorAccount = value;
                RaisePropertyChanged("AllowToBorrowDoctorAccount");
            }
        }
        //▲====: #039
        //▼====: #037
        private bool _AllowReSelectRoomWhenLeaveDept;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool AllowReSelectRoomWhenLeaveDept
        {
            get
            {
                return _AllowReSelectRoomWhenLeaveDept;
            }
            set
            {
                _AllowReSelectRoomWhenLeaveDept = value;
                RaisePropertyChanged("AllowReSelectRoomWhenLeaveDept");
            }
        }
        //▲====: #037
        //▼====: #036
        private bool _AutoSavePhysicalExamination;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool AutoSavePhysicalExamination
        {
            get
            {
                return _AutoSavePhysicalExamination;
            }
            set
            {
                _AutoSavePhysicalExamination = value;
                RaisePropertyChanged("AutoSavePhysicalExamination");
            }
        }
        //▲====: #036
        private int _ExaminationResultVersion;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int ExaminationResultVersion
        {
            get
            {
                return _ExaminationResultVersion;
            }
            set
            {
                _ExaminationResultVersion = value;
                RaisePropertyChanged("ExaminationResultVersion");
            }
        }
        //▼====: #035
        private bool _AllowFirstHIExaminationWithoutPay;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool AllowFirstHIExaminationWithoutPay
        {
            get
            {
                return _AllowFirstHIExaminationWithoutPay;
            }
            set
            {
                _AllowFirstHIExaminationWithoutPay = value;
                RaisePropertyChanged("AllowFirstHIExaminationWithoutPay");
            }
        }
        //▲====: #035
        private bool _ApplyCheckInPtRegistration;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool ApplyCheckInPtRegistration
        {
            get
            {
                return _ApplyCheckInPtRegistration;
            }
            set
            {
                _ApplyCheckInPtRegistration = value;
                RaisePropertyChanged("ApplyCheckInPtRegistration");
            }
        }
        private bool _ApplyAutoCodeForCirculars56;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool ApplyAutoCodeForCirculars56
        {
            get
            {
                return _ApplyAutoCodeForCirculars56;
            }
            set
            {
                _ApplyAutoCodeForCirculars56 = value;
                RaisePropertyChanged("ApplyAutoCodeForCirculars56");
            }
        }
        private string _SuffixAutoCodeForCirculars56;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string SuffixAutoCodeForCirculars56
        {
            get
            {
                return _SuffixAutoCodeForCirculars56;
            }
            set
            {
                _SuffixAutoCodeForCirculars56 = value;
                RaisePropertyChanged("SuffixAutoCodeForCirculars56");
            }
        }   
        //▼====: #033
        private string _NgayNhapLaiTDK;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NgayNhapLaiTDK
        {
            get
            {
                return _NgayNhapLaiTDK;
            }
            set
            {
                _NgayNhapLaiTDK = value;
                RaisePropertyChanged("NgayNhapLaiTDK");
            }
        }
        //▲====: #033
        private bool _ApplyOtherDiagnosis;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool ApplyOtherDiagnosis
        {
            get
            {
                return _ApplyOtherDiagnosis;
            }
            set
            {
                _ApplyOtherDiagnosis = value;
                RaisePropertyChanged("ApplyOtherDiagnosis");
            }
        }
        private string _ApplyCheckV_TreatmentType;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ApplyCheckV_TreatmentType
        {
            get
            {
                return _ApplyCheckV_TreatmentType;
            }
            set
            {
                _ApplyCheckV_TreatmentType = value;
                RaisePropertyChanged("ApplyCheckV_TreatmentType");
            }
        }
        // VuTTM - QMS Service
        private string _QMS_API_Url;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string QMS_API_Url
        {
            get
            {
                return _QMS_API_Url;
            }
            set
            {
                _QMS_API_Url = value;
                RaisePropertyChanged("QMS_API_Url");
            }
        }
        private bool _ApplyQMSAPI;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool ApplyQMSAPI
        {
            get
            {
                return _ApplyQMSAPI;
            }
            set
            {
                _ApplyQMSAPI = value;
                RaisePropertyChanged("ApplyQMSAPI");
            }
        }
        private bool _ApplyFixReCalcHIBenefit;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool ApplyFixReCalcHIBenefit
        {
            get
            {
                return _ApplyFixReCalcHIBenefit;
            }
            set
            {
                _ApplyFixReCalcHIBenefit = value;
                RaisePropertyChanged("ApplyFixReCalcHIBenefit");
            }
        }
        private long _Cashier1;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long Cashier1
        {
            get
            {
                return _Cashier1;
            }
            set
            {
                _Cashier1 = value;
                RaisePropertyChanged("Cashier1");
            }
        }
        private long _Cashier2;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long Cashier2
        {
            get
            {
                return _Cashier2;
            }
            set
            {
                _Cashier2 = value;
                RaisePropertyChanged("Cashier2");
            }
        }
        private string _QMSDepts;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string QMSDepts
        {
            get
            {
                return _QMSDepts;
            }
            set
            {
                _QMSDepts = value;
                RaisePropertyChanged("QMSDepts");
            }
        }

        private string _ApplyingQMSDepts;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ApplyingQMSDepts
        {
            get
            {
                return _ApplyingQMSDepts;
            }
            set
            {
                _ApplyingQMSDepts = value;
                RaisePropertyChanged("ApplyingQMSDepts");
            }
        }

        private string _FloorDeptLocation_2;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FloorDeptLocation_2
        {
            get
            {
                return _FloorDeptLocation_2;
            }
            set
            {
                _FloorDeptLocation_2 = value;
                RaisePropertyChanged("FloorDeptLocation_2");
            }
        }

        private string _OutpatientDept;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string OutpatientDept
        {
            get
            {
                return _OutpatientDept;
            }
            set
            {
                _OutpatientDept = value;
                RaisePropertyChanged("OutpatientDept");
            }
        }

        private long _PharmacyDepartment;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long PharmacyDepartment
        {
            get
            {
                return _PharmacyDepartment;
            }
            set
            {
                _PharmacyDepartment = value;
                RaisePropertyChanged("PharmacyDepartment");
            }
        }

        private long _MedDepartment;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long MedDepartment
        {
            get
            {
                return _MedDepartment;
            }
            set
            {
                _MedDepartment = value;
                RaisePropertyChanged("MedDepartment");
            }
        }

        private string _Excluded_Room;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Excluded_Room
        {
            get
            {
                return _Excluded_Room;
            }
            set
            {
                _Excluded_Room = value;
                RaisePropertyChanged("Excluded_Room");
            }
        }

        private string _FloorDeptLocation_1;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FloorDeptLocation_1
        {
            get
            {
                return _FloorDeptLocation_1;
            }
            set
            {
                _FloorDeptLocation_1 = value;
                RaisePropertyChanged("FloorDeptLocation_1");
            }
        }

        private string _FloorDeptLocation_0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FloorDeptLocation_0
        {
            get
            {
                return _FloorDeptLocation_0;
            }
            set
            {
                _FloorDeptLocation_0 = value;
                RaisePropertyChanged("FloorDeptLocation_0");
            }
        }

        private bool _ApplyFloorNumberKiosk;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool ApplyFloorNumberKiosk
        {
            get
            {
                return _ApplyFloorNumberKiosk;
            }
            set
            {
                _ApplyFloorNumberKiosk = value;
                RaisePropertyChanged("ApplyFloorNumberKiosk");
            }
        }

        //▼====: #033
        private bool _ApplyNewFuncExportExcel;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool ApplyNewFuncExportExcel
        {
            get
            {
                return _ApplyNewFuncExportExcel;
            }
            set
            {
                _ApplyNewFuncExportExcel = value;
                RaisePropertyChanged("ApplyNewFuncExportExcel");
            }
        }
        //▲====: #033
        //▼====: #032
        private bool _ApplyTemp12Version6556;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool ApplyTemp12Version6556
        {
            get
            {
                return _ApplyTemp12Version6556;
            }
            set
            {
                _ApplyTemp12Version6556 = value;
                RaisePropertyChanged("ApplyTemp12Version6556");
            }
        }
        //▲====: #032
        //▼====: #030
        private bool _AutoGetHICardDataFromHIPortal;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool AutoGetHICardDataFromHIPortal
        {
            get
            {
                return _AutoGetHICardDataFromHIPortal;
            }
            set
            {
                _AutoGetHICardDataFromHIPortal = value;
                RaisePropertyChanged("AutoGetHICardDataFromHIPortal");
            }
        }
        //▲====: #030
        //▼====: #028
        private int _WhichHospitalUseThisApp;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int WhichHospitalUseThisApp
        {
            get
            {
                return _WhichHospitalUseThisApp;
            }
            set
            {
                _WhichHospitalUseThisApp = value;
                RaisePropertyChanged("WhichHospitalUseThisApp");
            }
        }
        //▲====: #028
        //▼====: #015
        private bool _ShowInCostInInternalInwardPharmacy;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool ShowInCostInInternalInwardPharmacy
        {
            get
            {
                return _ShowInCostInInternalInwardPharmacy;
            }
            set
            {
                _ShowInCostInInternalInwardPharmacy = value;
                RaisePropertyChanged("ShowInCostInInternalInwardPharmacy");
            }
        }
        //▲====: #015
        //▼====: #014
        private string _DQGUnitname;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DQGUnitname
        {
            get
            {
                return _DQGUnitname;
            }
            set
            {
                _DQGUnitname = value;
                RaisePropertyChanged("DQGUnitname");
            }
        }
        //▲====: #014
        /*▼====: #008*/
        private string _reportHospitalNameField;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ReportHospitalName
        {
            get
            {
                return _reportHospitalNameField;
            }
            set
            {
                    _reportHospitalNameField = value;
                    RaisePropertyChanged("ReportHospitalName");
            }
        }

        private string _reportHospitalAddresField;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ReportHospitalAddress
        {
            get
            {
                return _reportHospitalAddresField;
            }
            set
            {
                _reportHospitalAddresField = value;
                RaisePropertyChanged("ReportHospitalAddress");
            }
        }

        private string _reportLogoUrlField;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ReportLogoUrl
        {
            get
            {
                return _reportLogoUrlField;
            }
            set
            {
                _reportLogoUrlField = value;
                RaisePropertyChanged("ReportLogoUrl");
            }
        }

        private string _reportDepartmentOfHealthField;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ReportDepartmentOfHealth
        {
            get
            {
                return _reportDepartmentOfHealthField;
            }
            set
            {
                _reportDepartmentOfHealthField = value;
                RaisePropertyChanged("ReportDepartmentOfHealth");
            }
        }
        /*▲====: #008*/


        //▼==== #070
        private string _reportHospitalNameEngField;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ReportHospitalNameEng
        {
            get
            {
                return _reportHospitalNameEngField;
            }
            set
            {
                _reportHospitalNameEngField = value;
                RaisePropertyChanged("ReportHospitalNameEng");
            }
        }

        private string _reportHospitalAddresEngField;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ReportHospitalAddressEng
        {
            get
            {
                return _reportHospitalAddresEngField;
            }
            set
            {
                _reportHospitalAddresEngField = value;
                RaisePropertyChanged("ReportHospitalAddressEng");
            }
        }

        private string _reportDepartmentOfHealthEngField;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ReportDepartmentOfHealthEng
        {
            get
            {
                return _reportDepartmentOfHealthEngField;
            }
            set
            {
                _reportDepartmentOfHealthEngField = value;
                RaisePropertyChanged("ReportDepartmentOfHealthEng");
            }
        }
        //▲==== #070

        //▼==== #071
        private string _reportHospitalNameShortField;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ReportHospitalNameShort
        {
            get
            {
                return _reportHospitalNameShortField;
            }
            set
            {
                _reportHospitalNameShortField = value;
                RaisePropertyChanged("ReportHospitalNameShort");
            }
        }
        //▲==== #071

        /*▼====: #005*/
        private int _phieuChiDinhPrintingModeField;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int PhieuChiDinhPrintingMode
        {
            get
            {
                return _phieuChiDinhPrintingModeField;
            }
            set
            {
                if (_phieuChiDinhPrintingModeField.Equals(value) != true)
                {
                    _phieuChiDinhPrintingModeField = value;
                    RaisePropertyChanged("PhieuChiDinhPrintingMode");
                }
            }
        }
        //▲====: #005
        //▼====: #010
        private int _phieuMienGiamPrintingModeField;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int PhieuMienGiamPrintingMode
        {
            get
            {
                return _phieuMienGiamPrintingModeField;
            }
            set
            {
                if (_phieuMienGiamPrintingModeField.Equals(value) != true)
                {
                    _phieuMienGiamPrintingModeField = value;
                    RaisePropertyChanged("PhieuMienGiamPrintingMode");
                }
            }
        }
        //▲====: #010
        //▼====: #007
        private int _hiConfirmationPrintingModeField;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int HiConfirmationPrintingMode
        {
            get
            {
                return _hiConfirmationPrintingModeField;
            }
            set
            {
                if (_hiConfirmationPrintingModeField.Equals(value) != true)
                {
                    _hiConfirmationPrintingModeField = value;
                    RaisePropertyChanged("HiConfirmationPrintingMode");
                }
            }
        }
        /*▲====: #007*/

        private int _receiptPrintingModeField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int ReceiptPrintingMode
        {
            get
            {
                return _receiptPrintingModeField;
            }
            set
            {
                if (_receiptPrintingModeField.Equals(value) != true)
                {
                    _receiptPrintingModeField = value;
                    RaisePropertyChanged("ReceiptPrintingMode");
                }
            }
        }

        private int _ReceiptVersion;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int ReceiptVersion
        {
            get
            {
                return _ReceiptVersion;
            }
            set
            {
                if (_ReceiptVersion.Equals(value) != true)
                {
                    _ReceiptVersion = value;
                    RaisePropertyChanged("ReceiptVersion");
                }
            }
        }


        private bool _isPrintReceiptPatientNoPay;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsPrintReceiptPatientNoPay
        {
            get
            {
                return _isPrintReceiptPatientNoPay;
            }
            set
            {
                if (_isPrintReceiptPatientNoPay.Equals(value) != true)
                {
                    _isPrintReceiptPatientNoPay = value;
                    RaisePropertyChanged("IsPrintReceiptPatientNoPay");
                }
            }
        }


        private bool _isPrintReceiptHINoPay;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsPrintReceiptHINoPay
        {
            get
            {
                return _isPrintReceiptHINoPay;
            }
            set
            {
                if (_isPrintReceiptHINoPay.Equals(value) != true)
                {
                    _isPrintReceiptHINoPay = value;
                    RaisePropertyChanged("IsPrintReceiptHINoPay");
                }
            }
        }

        private int _editHIBenefit;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int EditHIBenefit
        {
            get
            {
                return _editHIBenefit;
            }
            set
            {
                if (_editHIBenefit.Equals(value) != true)
                {
                    _editHIBenefit = value;
                    RaisePropertyChanged("EditHIBenefit");
                }
            }
        }

        private int _numberOfCopiesPrescription;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int NumberOfCopiesPrescription
        {
            get
            {
                return _numberOfCopiesPrescription;
            }
            set
            {
                if (_numberOfCopiesPrescription.Equals(value) != true)
                {
                    _numberOfCopiesPrescription = value;
                    RaisePropertyChanged("NumberOfCopiesPrescription");
                }
            }
        }

        private int _defaultNumOfCopyPrescriptNormalPT;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int DefaultNumOfCopyPrescriptNormalPT
        {
            get
            {
                return _defaultNumOfCopyPrescriptNormalPT;
            }
            set
            {
                if (_defaultNumOfCopyPrescriptNormalPT.Equals(value) != true)
                {
                    _defaultNumOfCopyPrescriptNormalPT = value;
                    RaisePropertyChanged("DefaultNumOfCopyPrescriptNormalPT");
                }
            }
        }

        private int _defaultNumOfCopyPrescriptHIPT;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int DefaultNumOfCopyPrescriptHIPT
        {
            get
            {
                return _defaultNumOfCopyPrescriptHIPT;
            }
            set
            {
                if (_defaultNumOfCopyPrescriptHIPT.Equals(value) != true)
                {
                    _defaultNumOfCopyPrescriptHIPT = value;
                    RaisePropertyChanged("DefaultNumOfCopyPrescriptHIPT");
                }
            }
        }

        private int _organizationUseSoftware;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int OrganizationUseSoftware
        {
            get
            {
                return _organizationUseSoftware;
            }
            set
            {
                if (_organizationUseSoftware.Equals(value) != true)
                {
                    _organizationUseSoftware = value;
                    RaisePropertyChanged("OrganizationUseSoftware");
                }
            }
        }

        private string _organizationName;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string OrganizationName
        {
            get
            {
                return _organizationName;
            }
            set
            {
                _organizationName = value;
                RaisePropertyChanged("OrganizationName");
            }
        }

        private string _organizationAddress;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string OrganizationAddress
        {
            get
            {
                return _organizationAddress;
            }
            set
            {
                _organizationAddress = value;
                RaisePropertyChanged("OrganizationAddress");           
            }
        }

        private string _organizationNotes;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string OrganizationNotes
        {
            get
            {
                return _organizationNotes;
            }
            set
            {
                _organizationNotes = value;
                RaisePropertyChanged("OrganizationNotes");
            }
        }

        private bool _showApptCheck;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool ShowApptCheck
        {
            get
            {
                return _showApptCheck;
            }
            set
            {
                if (_showApptCheck.Equals(value) != true)
                {
                    _showApptCheck = value;
                    RaisePropertyChanged("ShowApptCheck");
                }
            }
        }


        private bool _showLoginNameOnReport38;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool ShowLoginNameOnReport38
        {
            get
            {
                return _showLoginNameOnReport38;
            }
            set
            {
                if (_showLoginNameOnReport38.Equals(value) != true)
                {
                    _showLoginNameOnReport38 = value;
                    RaisePropertyChanged("ShowLoginNameOnReport38");
                }
            }
        }

        private int _StaffCatTypeBAC_SIField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int StaffCatTypeBAC_SI
        {
            get
            {
                return _StaffCatTypeBAC_SIField;
            }
            set
            {
                if (_StaffCatTypeBAC_SIField.Equals(value) != true)
                {
                    _StaffCatTypeBAC_SIField = value;
                    RaisePropertyChanged("StaffCatTypeBAC_SI");
                }
            }
        }

        private int _receiptForEachLocationPrintingModeField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int ReceiptForEachLocationPrintingMode
        {
            get
            {
                return _receiptForEachLocationPrintingModeField;
            }
            set
            {
                if (_receiptForEachLocationPrintingModeField.Equals(value) != true)
                {
                    _receiptForEachLocationPrintingModeField = value;
                    RaisePropertyChanged("ReceiptForEachLocationPrintingMode");
                }
            }
        }

        //TxD 24/05/2014 Adding more Fields below

        private bool _ShowAddRegisButton;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool ShowAddRegisButton
        {
            get { return _ShowAddRegisButton;  }
            set
            {
                _ShowAddRegisButton = value;
                RaisePropertyChanged("ShowAddRegisButton");
            }
        }
        private int _AllowDuplicateMedicalServiceItems;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int AllowDuplicateMedicalServiceItems
        {
            get { return _AllowDuplicateMedicalServiceItems; }
            set
            {
                _AllowDuplicateMedicalServiceItems = value;
                RaisePropertyChanged("AllowDuplicateMedicalServiceItems");
            }
        }
        private int _NumberTypePrescriptions_Rule;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int NumberTypePrescriptions_Rule
        {
            get { return _NumberTypePrescriptions_Rule;  }
            set
            {
                _NumberTypePrescriptions_Rule = value;
                RaisePropertyChanged("NumberTypePrescriptions_Rule");
            }
        }

        private int _ExpRelAndBuildVersion;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int ExpRelAndBuildVersion
        {
            get { return _ExpRelAndBuildVersion; }
            set
            {
                _ExpRelAndBuildVersion = value;
                RaisePropertyChanged("ExpRelAndBuildVersion");
            }
        }

        private byte _printPatientInfoOption;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public byte PrintPatientInfoOption
        {
            get
            {
                return _printPatientInfoOption;
            }
            set
            {
                if (_printPatientInfoOption.Equals(value) != true)
                {
                    _printPatientInfoOption = value;
                    RaisePropertyChanged("PrintPatientInfoOption");
                }
            }
        }

        //▼====: #048
        private bool _EnablePostponementAdvancePayment;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool EnablePostponementAdvancePayment
        {
            get
            {
                return _EnablePostponementAdvancePayment;
            }
            set
            {
                if (_EnablePostponementAdvancePayment == value)
                {
                    return;
                }
                _EnablePostponementAdvancePayment = value;
                RaisePropertyChanged("EnablePostponementAdvancePayment");
            }
        }
        //▲====: #048
        //▼====: #050
        private int _DoctorContactPatientTime;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int DoctorContactPatientTime
        {
            get
            {
                return _DoctorContactPatientTime;
            }
            set
            {
                if (_DoctorContactPatientTime == value)
                {
                    return;
                }
                _DoctorContactPatientTime = value;
                RaisePropertyChanged("DoctorContactPatientTime");
            }
        }
        private string _LocationNotCheckDoctorContactPatientTime;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LocationNotCheckDoctorContactPatientTime
        {
            get
            {
                return _LocationNotCheckDoctorContactPatientTime;
            }
            set
            {
                if (_LocationNotCheckDoctorContactPatientTime == value)
                {
                    return;
                }
                _LocationNotCheckDoctorContactPatientTime = value;
                RaisePropertyChanged("LocationNotCheckDoctorContactPatientTime");
            }
        }
        //▲====: #050
        private string _NotesKhongCheckTocDoTruyen;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NotesKhongCheckTocDoTruyen
        {
            get
            {
                return _NotesKhongCheckTocDoTruyen;
            }
            set
            {
                if (_NotesKhongCheckTocDoTruyen == value)
                {
                    return;
                }
                _NotesKhongCheckTocDoTruyen = value;
                RaisePropertyChanged("NotesKhongCheckTocDoTruyen");
            }
        }
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        //==== #001
        private float _DefaultVATPercent;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public float DefaultVATPercent
        {
            get
            {
                return _DefaultVATPercent;
            }
            set
            {
                if (ReferenceEquals(_DefaultVATPercent, value) != true)
                {
                    _DefaultVATPercent = value;
                    RaisePropertyChanged("DefaultVATPercent");
                }
            }
        }
        private bool _VATAlreadyInPrice;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool VATAlreadyInPrice
        {
            get
            {
                return _VATAlreadyInPrice;
            }
            set
            {
                if (ReferenceEquals(_VATAlreadyInPrice, value) != true)
                {
                    _VATAlreadyInPrice = value;
                    RaisePropertyChanged("VATAlreadyInPrice");
                }
            }
        }
        private bool _UseVATOnBill;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool UseVATOnBill
        {
            get
            {
                return _UseVATOnBill;
            }
            set
            {
                if (ReferenceEquals(_UseVATOnBill, value) != true)
                {
                    _UseVATOnBill = value;
                    RaisePropertyChanged("UseVATOnBill");
                }
            }
        }

        private bool _UseQRCode;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool UseQRCode
        {
            get
            {
                return _UseQRCode;
            }
            set
            {
                if (ReferenceEquals(_UseQRCode, value) != true)
                {
                    _UseQRCode = value;
                    RaisePropertyChanged("UseQRCode");
                }
            }
        }

        //▼==== #073
        private bool _UseIDCardQRCode;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool UseIDCardQRCode
        {
            get
            {
                return _UseIDCardQRCode;
            }
            set
            {
                if (ReferenceEquals(_UseIDCardQRCode, value) != true)
                {
                    _UseIDCardQRCode = value;
                    RaisePropertyChanged("UseIDCardQRCode");
                }
            }
        }
        //▲==== #073

        private bool _EnableHIStore;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool EnableHIStore
        {
            get
            {
                return _EnableHIStore;
            }
            set
            {
                if (ReferenceEquals(_EnableHIStore, value) != true)
                {
                    _EnableHIStore = value;
                    RaisePropertyChanged("EnableHIStore");
                }
            }
        }

        private bool _EnablePayAfter;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool EnablePayAfter
        {
            get
            {
                return _EnablePayAfter;
            }
            set
            {
                if (ReferenceEquals(_EnablePayAfter, value) != true)
                {
                    _EnablePayAfter = value;
                    RaisePropertyChanged("EnablePayAfter");
                }
            }
        }
        //==== #001
        private int _BorrowTimeLimit;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int BorrowTimeLimit
        {
            get
            {
                return _BorrowTimeLimit;
            }
            set
            {
                if (ReferenceEquals(_BorrowTimeLimit, value) != true)
                {
                    _BorrowTimeLimit = value;
                    RaisePropertyChanged("BorrowTimeLimit");
                }
            }
        }
        private bool _EnableMedicalFileManagement;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool EnableMedicalFileManagement
        {
            get
            {
                return _EnableMedicalFileManagement;
            }
            set
            {
                if (ReferenceEquals(_EnableMedicalFileManagement, value) != true)
                {
                    _EnableMedicalFileManagement = value;
                    RaisePropertyChanged("EnableMedicalFileManagement");
                }
            }
        }

        private bool _IsUseDailyDiagnostic;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsUseDailyDiagnostic
        {
            get
            {
                return _IsUseDailyDiagnostic;
            }
            set
            {
                if (ReferenceEquals(_IsUseDailyDiagnostic, value) != true)
                {
                    _IsUseDailyDiagnostic = value;
                    RaisePropertyChanged("IsUseDailyDiagnostic");
                }
            }
        }

        private bool _EnableTestFunction;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool EnableTestFunction
        {
            get
            {
                return _EnableTestFunction;
            }
            set
            {
                if (ReferenceEquals(_EnableTestFunction, value) != true)
                {
                    _EnableTestFunction = value;
                    RaisePropertyChanged("EnableTestFunction");
                }
            }
        }

        private bool _IsEnalbeInDeptChangeLocFucn;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsEnalbeInDeptChangeLocFucn
        {
            get
            {
                return _IsEnalbeInDeptChangeLocFucn;
            }
            set
            {
                if (ReferenceEquals(_IsEnalbeInDeptChangeLocFucn, value) != true)
                {
                    _IsEnalbeInDeptChangeLocFucn = value;
                    RaisePropertyChanged("IsEnalbeInDeptChangeLocFucn");
                }
            }
        }

        private bool _IsEnalbeTempInDeptFuction;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsEnalbeTempInDeptFuction
        {
            get
            {
                return _IsEnalbeTempInDeptFuction;
            }
            set
            {
                if (ReferenceEquals(_IsEnalbeTempInDeptFuction, value) != true)
                {
                    _IsEnalbeTempInDeptFuction = value;
                    RaisePropertyChanged("IsEnalbeTempInDeptFuction");
                }
            }
        }

        private bool _AllowZeroHIPriceWithFlag;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool AllowZeroHIPriceWithFlag
        {
            get
            {
                return _AllowZeroHIPriceWithFlag;
            }
            set
            {
                if (ReferenceEquals(_AllowZeroHIPriceWithFlag, value) != true)
                {
                    _AllowZeroHIPriceWithFlag = value;
                    RaisePropertyChanged("AllowZeroHIPriceWithFlag");
                }
            }
        }

        private bool _EnableOutPtCashAdvance;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool EnableOutPtCashAdvance
        {
            get
            {
                return _EnableOutPtCashAdvance;
            }
            set
            {
                if (ReferenceEquals(_EnableOutPtCashAdvance, value) != true)
                {
                    _EnableOutPtCashAdvance = value;
                    RaisePropertyChanged("EnableOutPtCashAdvance");
                }
            }
        }

        private bool _MixedHIPharmacyStores;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool MixedHIPharmacyStores
        {
            get
            {
                return _MixedHIPharmacyStores;
            }
            set
            {
                if (ReferenceEquals(_MixedHIPharmacyStores, value) != true)
                {
                    _MixedHIPharmacyStores = value;
                    RaisePropertyChanged("MixedHIPharmacyStores");
                }
            }
        }

        private string _ReportTemplatesLocation;
        [DataMemberAttribute]
        public string ReportTemplatesLocation
        {
            get
            {
                return _ReportTemplatesLocation;
            }
            set
            {
                _ReportTemplatesLocation = value;
                RaisePropertyChanged("ReportTemplatesLocation");
            }
        }

        private string _ValidHIPattern;
        [DataMemberAttribute]
        public string ValidHIPattern
        {
            get
            {
                return _ValidHIPattern;
            }
            set
            {
                _ValidHIPattern = value;
                RaisePropertyChanged("ValidHIPattern");
            }
        }

        private List<string[]> _InsuranceBenefitCategories;
        [DataMemberAttribute]
        public List<string[]> InsuranceBenefitCategories
        {
            get
            {
                return _InsuranceBenefitCategories;
            }
            set
            {
                _InsuranceBenefitCategories = value;
                RaisePropertyChanged("InsuranceBenefitCategories");
            }
        }

        private bool _PayOnComfirmHI;
        [DataMemberAttribute]
        public bool PayOnComfirmHI
        {
            get
            {
                return _PayOnComfirmHI;
            }
            set
            {
                _PayOnComfirmHI = value;
                RaisePropertyChanged("PayOnComfirmHI");
            }
        }

        private string _ServerPublicAddress;
        [DataMemberAttribute]
        public string ServerPublicAddress
        {
            get
            {
                return _ServerPublicAddress;
            }
            set
            {
                _ServerPublicAddress = value;
                RaisePropertyChanged("ServerPublicAddress");
            }
        }

        private decimal _AddingServicesPercent;
        [DataMemberAttribute]
        public decimal AddingServicesPercent
        {
            get
            {
                return _AddingServicesPercent;
            }
            set
            {
                _AddingServicesPercent = value;
                RaisePropertyChanged("AddingServicesPercent");
            }
        }

        private string _eInvoicePatern;
        [DataMemberAttribute]
        public string eInvoicePatern
        {
            get
            {
                return _eInvoicePatern;
            }
            set
            {
                _eInvoicePatern = value;
                RaisePropertyChanged("eInvoicePatern");
            }
        }

        private string _eInvoiceSerial;
        [DataMemberAttribute]
        public string eInvoiceSerial
        {
            get
            {
                return _eInvoiceSerial;
            }
            set
            {
                _eInvoiceSerial = value;
                RaisePropertyChanged("eInvoiceSerial");
            }
        }

        private string _eInvoiceAdminUserName;
        [DataMemberAttribute]
        public string eInvoiceAdminUserName
        {
            get
            {
                return _eInvoiceAdminUserName;
            }
            set
            {
                _eInvoiceAdminUserName = value;
                RaisePropertyChanged("eInvoiceAdminUserName");
            }
        }

        private string _eInvoiceAdminUserPass;
        [DataMemberAttribute]
        public string eInvoiceAdminUserPass
        {
            get
            {
                return _eInvoiceAdminUserPass;
            }
            set
            {
                _eInvoiceAdminUserPass = value;
                RaisePropertyChanged("eInvoiceAdminUserPass");
            }
        }

        private string _eInvoiceAccountUserName;
        [DataMemberAttribute]
        public string eInvoiceAccountUserName
        {
            get
            {
                return _eInvoiceAccountUserName;
            }
            set
            {
                _eInvoiceAccountUserName = value;
                RaisePropertyChanged("eInvoiceAccountUserName");
            }
        }

        private string _eInvoiceAccountUserPass;
        [DataMemberAttribute]
        public string eInvoiceAccountUserPass
        {
            get
            {
                return _eInvoiceAccountUserPass;
            }
            set
            {
                _eInvoiceAccountUserPass = value;
                RaisePropertyChanged("eInvoiceAccountUserPass");
            }
        }

        private string _DQGUsername;
        [DataMemberAttribute]
        public string DQGUsername
        {
            get
            {
                return _DQGUsername;
            }
            set
            {
                _DQGUsername = value;
                RaisePropertyChanged("DQGUsername");
            }
        }

        private string _DQGPassword;
        [DataMemberAttribute]
        public string DQGPassword
        {
            get
            {
                return _DQGPassword;
            }
            set
            {
                _DQGPassword = value;
                RaisePropertyChanged("DQGPassword");
            }
        }

        private string _DQGUnitcode;
        [DataMemberAttribute]
        public string DQGUnitcode
        {
            get
            {
                return _DQGUnitcode;
            }
            set
            {
                _DQGUnitcode = value;
                RaisePropertyChanged("DQGUnitcode");
            }
        }

        private string _DQGHUsername;
        [DataMemberAttribute]
        public string DQGHUsername
        {
            get
            {
                return _DQGHUsername;
            }
            set
            {
                _DQGHUsername = value;
                RaisePropertyChanged("DQGHUsername");
            }
        }

        private string _DQGHPassword;
        [DataMemberAttribute]
        public string DQGHPassword
        {
            get
            {
                return _DQGHPassword;
            }
            set
            {
                _DQGHPassword = value;
                RaisePropertyChanged("DQGHPassword");
            }
        }
        //▼====: #011
        private bool _PrintingReceiptWithDrugBillField;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool PrintingReceiptWithDrugBill
        {
            get
            {
                return _PrintingReceiptWithDrugBillField;
            }
            set
            {
                if (_PrintingReceiptWithDrugBillField.Equals(value) != true)
                {
                    _PrintingReceiptWithDrugBillField = value;
                    RaisePropertyChanged("PrintingReceiptWithDrugBill");
                }
            }
        }
        //▲====: #011
        //▼====: #012
        private bool _PrintingPhieuChiDinhForServiceField;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool PrintingPhieuChiDinhForService
        {
            get
            {
                return _PrintingPhieuChiDinhForServiceField;
            }
            set
            {
                if (_PrintingPhieuChiDinhForServiceField.Equals(value) != true)
                {
                    _PrintingPhieuChiDinhForServiceField = value;
                    RaisePropertyChanged("PrintingPhieuChiDinhForService");
                }
            }
        }
        //▲====: #012
        //▼====: #015
        private bool _AllowSearchInReport;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool AllowSearchInReport
        {
            get
            {
                return _AllowSearchInReport;
            }
            set
            {
                if (_AllowSearchInReport.Equals(value) != true)
                {
                    _AllowSearchInReport = value;
                    RaisePropertyChanged("AllowSearchInReport");
                }
            }
        }
        //▲====: #015

        private bool _UserCanEditInvoicePatern;
        [DataMemberAttribute]
        public bool UserCanEditInvoicePatern
        {
            get
            {
                return _UserCanEditInvoicePatern;
            }
            set
            {
                _UserCanEditInvoicePatern = value;
                RaisePropertyChanged("UserCanEditInvoicePatern");
            }
        }

        private int _MaxEInvoicePaternLength;
        [DataMemberAttribute]
        public int MaxEInvoicePaternLength
        {
            get
            {
                return _MaxEInvoicePaternLength;
            }
            set
            {
                _MaxEInvoicePaternLength = value;
                RaisePropertyChanged("MaxEInvoicePaternLength");
            }
        }

        private long _DefaultVIPServiceItemID;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long DefaultVIPServiceItemID
        {
            get
            {
                return _DefaultVIPServiceItemID;
            }
            set
            {
                if (_DefaultVIPServiceItemID.Equals(value) != true)
                {
                    _DefaultVIPServiceItemID = value;
                    RaisePropertyChanged("DefaultVIPServiceItemID");
                }
            }
        }

        private bool _ShowAddressPKBSHuan;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool ShowAddressPKBSHuan
        {
            get
            {
                return _ShowAddressPKBSHuan;
            }
            set
            {
                if (_ShowAddressPKBSHuan != value)
                {
                    _ShowAddressPKBSHuan = value;
                    RaisePropertyChanged("ShowAddressPKBSHuan");
                }
            }
        }

        private decimal _AddingHIServicesPercent;
        [DataMemberAttribute]
        public decimal AddingHIServicesPercent
        {
            get
            {
                return _AddingHIServicesPercent;
            }
            set
            {
                _AddingHIServicesPercent = value;
                RaisePropertyChanged("AddingHIServicesPercent");
            }
        }
        //▼====: #017
        private bool _ViewPrintAllImagingPCLRequestSeparate;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool ViewPrintAllImagingPCLRequestSeparate
        {
            get
            {
                return _ViewPrintAllImagingPCLRequestSeparate;
            }
            set
            {
                if (_ViewPrintAllImagingPCLRequestSeparate.Equals(value) != true)
                {
                    _ViewPrintAllImagingPCLRequestSeparate = value;
                    RaisePropertyChanged("ViewPrintAllImagingPCLRequestSeparate");
                }
            }
        }
        //▲====: #017
        private bool _ChangeHIAfterSaveAndPayRule;
        [DataMemberAttribute]
        public bool ChangeHIAfterSaveAndPayRule
        {
            get
            {
                return _ChangeHIAfterSaveAndPayRule;
            }
            set
            {
                _ChangeHIAfterSaveAndPayRule = value;
                RaisePropertyChanged("ChangeHIAfterSaveAndPayRule");
            }
        }

        
        private int _MaxTimeForSmallProcedure;
        [DataMemberAttribute]
        public int MaxTimeForSmallProcedure
        {
            get { return _MaxTimeForSmallProcedure; }
            set
            {
                _MaxTimeForSmallProcedure = value;
                RaisePropertyChanged("MaxTimeForSmallProcedure");
            }
        }

        private int _LIDForConsultationAtHome;
        [DataMemberAttribute]
        public int LIDForConsultationAtHome
        {
            get { return _LIDForConsultationAtHome; }
            set
            {
                _LIDForConsultationAtHome = value;
                RaisePropertyChanged("LIDForConsultationAtHome");
            }
        }

        private string _CSSUrlPattern;
        [DataMemberAttribute]
        public string CSSUrlPattern
        {
            get { return _CSSUrlPattern; }
            set
            {
                _CSSUrlPattern = value;
                RaisePropertyChanged("CSSUrlPattern");
            }
        }

        private bool _UpdateTicketStatusAfterRegister;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool UpdateTicketStatusAfterRegister
        {
            get { return _UpdateTicketStatusAfterRegister; }
            set
            {
                _UpdateTicketStatusAfterRegister = value;
                RaisePropertyChanged("UpdateTicketStatusAfterRegister");
            }
        }
        private bool _ReportTwoRegistrationSameTime;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool ReportTwoRegistrationSameTime
        {
            get { return _ReportTwoRegistrationSameTime; }
            set
            {
                _ReportTwoRegistrationSameTime = value;
                RaisePropertyChanged("ReportTwoRegistrationSameTime");
            }
        }
        private bool _UseQMSSystem;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool UseQMSSystem
        {
            get { return _UseQMSSystem; }
            set
            {
                _UseQMSSystem = value;
                RaisePropertyChanged("UseQMSSystem");
            }
        }
        private bool _CheckPatientInfoQMSSystem;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool CheckPatientInfoQMSSystem
        {
            get { return _CheckPatientInfoQMSSystem; }
            set
            {
                _CheckPatientInfoQMSSystem = value;
                RaisePropertyChanged("CheckPatientInfoQMSSystem");
            }
        }

        //▼====: #025
        private int _BlockRegNoTicket;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int BlockRegNoTicket
        {
            get { return _BlockRegNoTicket; }
            set
            {
                _BlockRegNoTicket = value;
                RaisePropertyChanged("BlockRegNoTicket");
            }
        }
        //▲====: #025

        private long _DefaultStoreIDForQuotation;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public long DefaultStoreIDForQuotation
        {
            get
            {
                return _DefaultStoreIDForQuotation;
            }
            set
            {
                if (_DefaultStoreIDForQuotation.Equals(value) != true)
                {
                    _DefaultStoreIDForQuotation = value;
                    RaisePropertyChanged("DefaultStoreIDForQuotation");
                }
            }
        }

        //▼====: #023
        private int _phieuNhanThuocPrintingModeInConfirmHIViewField;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int PhieuNhanThuocPrintingModeInConfirmHIView
        {
            get
            {
                return _phieuNhanThuocPrintingModeInConfirmHIViewField;
            }
            set
            {
                if (_phieuNhanThuocPrintingModeInConfirmHIViewField.Equals(value) != true)
                {
                    _phieuNhanThuocPrintingModeInConfirmHIViewField = value;
                    RaisePropertyChanged("PhieuNhanThuocPrintingModeInConfirmHIView");
                }
            }
        }
        //▲====: #023


        private bool _SpecialHIRegistration;
        [DataMemberAttribute]
        public bool SpecialHIRegistration
        {
            get
            {
                return _SpecialHIRegistration;
            }
            set
            {
                if (_SpecialHIRegistration.Equals(value) != true)
                {
                    _SpecialHIRegistration = value;
                    RaisePropertyChanged("SpecialHIRegistration");
                }
            }
        }

        private bool _NewMethodToReport4210;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool NewMethodToReport4210
        {
            get { return _NewMethodToReport4210; }
            set
            {
                _NewMethodToReport4210 = value;
                RaisePropertyChanged("NewMethodToReport4210");
            }
        }

        private bool _ChangeVATCreditOnInwardInvoice;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool ChangeVATCreditOnInwardInvoice
        {
            get { return _ChangeVATCreditOnInwardInvoice; }
            set
            {
                _ChangeVATCreditOnInwardInvoice = value;
                RaisePropertyChanged("ChangeVATCreditOnInwardInvoice");
            }
        }
        private decimal _InwardDifferenceValue;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal InwardDifferenceValue
        {
            get { return _InwardDifferenceValue; }
            set
            {
                _InwardDifferenceValue = value;
                RaisePropertyChanged("InwardDifferenceValue");
            }
        }
        private string _ICDCategorySearchUrl;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ICDCategorySearchUrl
        {
            get
            {
                return _ICDCategorySearchUrl;
            }
            set
            {
                _ICDCategorySearchUrl = value;
                RaisePropertyChanged("ICDCategorySearchUrl");
            }
        }
        private bool _ApplyTemplatePCLResultNew;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool ApplyTemplatePCLResultNew
        {
            get
            {
                return _ApplyTemplatePCLResultNew;
            }
            set
            {
                _ApplyTemplatePCLResultNew = value;
                RaisePropertyChanged("ApplyTemplatePCLResultNew");
            }
        }
        private string _RuntimeUrl;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string RuntimeUrl
        {
            get
            {
                return _RuntimeUrl;
            }
            set
            {
                _RuntimeUrl = value;
                RaisePropertyChanged("RuntimeUrl");
            }
        }
        private string _RuntimeLocation;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string RuntimeLocation
        {
            get
            {
                return _RuntimeLocation;
            }
            set
            {
                _RuntimeLocation = value;
                RaisePropertyChanged("RuntimeLocation");
            }
        }
        private string _RuntimeReg32;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string RuntimeReg32
        {
            get
            {
                return _RuntimeReg32;
            }
            set
            {
                _RuntimeReg32 = value;
                RaisePropertyChanged("RuntimeReg32");
            }
        }
        private string _RuntimeReg64;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string RuntimeReg64
        {
            get
            {
                return _RuntimeReg64;
            }
            set
            {
                _RuntimeReg64 = value;
                RaisePropertyChanged("RuntimeReg64");
            }
        }
        private bool _EnableCheckboxXCD;
        [DataMemberAttribute]
        public bool EnableCheckboxXCD
        {
            get
            {
                return _EnableCheckboxXCD;
            }
            set
            {             
                _EnableCheckboxXCD = value;
                RaisePropertyChanged("EnableCheckboxXCD");
            }
        }
        private string _KBYTLink;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string KBYTLink
        {
            get
            {
                return _KBYTLink;
            }
            set
            {
                _KBYTLink = value;
                RaisePropertyChanged("KBYTLink");
            }
        }
        private int _WarningOutTimeSegments;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int WarningOutTimeSegments
        {
            get
            {
                return _WarningOutTimeSegments;
            }
            set
            {
                _WarningOutTimeSegments = value;
                RaisePropertyChanged("WarningOutTimeSegments");
            }
        }
        private bool _IsApplyTimeSegments;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsApplyTimeSegments
        {
            get
            {
                return _IsApplyTimeSegments;
            }
            set
            {
                _IsApplyTimeSegments = value;
                RaisePropertyChanged("IsApplyTimeSegments");
            }
        }
        private string _BearerToken;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string BearerToken
        {
            get
            {
                return _BearerToken;
            }
            set
            {
                _BearerToken = value;
                RaisePropertyChanged("BearerToken");
            }
        }
        private string _ExamCovidAPIBaseURL;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ExamCovidAPIBaseURL
        {
            get
            {
                return _ExamCovidAPIBaseURL;
            }
            set
            {
                _ExamCovidAPIBaseURL = value;
                RaisePropertyChanged("ExamCovidAPIBaseURL");
            }
        }
        private string _ExamCovidAPIGetHistory;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ExamCovidAPIGetHistory
        {
            get
            {
                return _ExamCovidAPIGetHistory;
            }
            set
            {
                _ExamCovidAPIGetHistory = value;
                RaisePropertyChanged("ExamCovidAPIGetHistory");
            }
        }
        private string _ExamCovidAPIGetPrintPreview;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ExamCovidAPIGetPrintPreview
        {
            get
            {
                return _ExamCovidAPIGetPrintPreview;
            }
            set
            {
                _ExamCovidAPIGetPrintPreview = value;
                RaisePropertyChanged("ExamCovidAPIGetPrintPreview");
            }
        }
        private bool _IsApplyPCRDual;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsApplyPCRDual
        {
            get
            {
                return _IsApplyPCRDual;
            }
            set
            {
                _IsApplyPCRDual = value;
                RaisePropertyChanged("IsApplyPCRDual");
            }
        }
        private bool _IsApplyAutoCreateHIReport;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsApplyAutoCreateHIReport
        {
            get
            {
                return _IsApplyAutoCreateHIReport;
            }
            set
            {
                _IsApplyAutoCreateHIReport = value;
                RaisePropertyChanged("IsApplyAutoCreateHIReport");
            }
        }
        private bool _IsApplyUpdateInstruction;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsApplyUpdateInstruction
        {
            get
            {
                return _IsApplyUpdateInstruction;
            }
            set
            {
                _IsApplyUpdateInstruction = value;
                RaisePropertyChanged("IsApplyUpdateInstruction");
            }
        }

        //▼====: #045
        private int _TimeForAllowUpdateMedicalInstruction;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int TimeForAllowUpdateMedicalInstruction
        {
            get
            {
                return _TimeForAllowUpdateMedicalInstruction;
            }
            set
            {
                _TimeForAllowUpdateMedicalInstruction = value;
                RaisePropertyChanged("TimeForAllowUpdateMedicalInstruction");
            }
        }

        private bool _IsApplyTimeForAllowUpdateMedicalInstruction;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsApplyTimeForAllowUpdateMedicalInstruction
        {
            get
            {
                return _IsApplyTimeForAllowUpdateMedicalInstruction;
            }
            set
            {
                _IsApplyTimeForAllowUpdateMedicalInstruction = value;
                RaisePropertyChanged("IsApplyTimeForAllowUpdateMedicalInstruction");
            }
        }
        //▲====: #045

        //▼====: #046
        private bool _IsEnableQMSForPCL;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsEnableQMSForPCL
        {
            get
            {
                return _IsEnableQMSForPCL;
            }
            set
            {
                _IsEnableQMSForPCL= value;
                RaisePropertyChanged("IsEnableQMSForPCL");
            }
        }
        private bool _IsEnableQMSForPrescription;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsEnableQMSForPrescription
        {
            get
            {
                return _IsEnableQMSForPrescription;
            }
            set
            {
                _IsEnableQMSForPrescription = value;
                RaisePropertyChanged("IsEnableQMSForPrescription");
            }
        }
        private bool _IsEnableCreateOrderFromAccountant;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsEnableCreateOrderFromAccountant
        {
            get
            {
                return _IsEnableCreateOrderFromAccountant;
            }
            set
            {
                _IsEnableCreateOrderFromAccountant = value;
                RaisePropertyChanged("IsEnableCreateOrderFromAccountant");
            }
        }
        //▲====: #046
        //▼====: #049
        private bool _PrintPrescriptionWithTemp12;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool PrintPrescriptionWithTemp12
        {
            get
            {
                return _PrintPrescriptionWithTemp12;
            }
            set
            {
                _PrintPrescriptionWithTemp12 = value;
                RaisePropertyChanged("PrintPrescriptionWithTemp12");
            }
        }
        //▲====: #049
        private string _LinkKhaoSatNgoaiTru;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LinkKhaoSatNgoaiTru
        {
            get
            {
                return _LinkKhaoSatNgoaiTru;
            }
            set
            {
                _LinkKhaoSatNgoaiTru = value;
                RaisePropertyChanged("LinkKhaoSatNgoaiTru");
            }
        }
        private string _LinkKhaoSatNoiTru;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LinkKhaoSatNoiTru
        {
            get
            {
                return _LinkKhaoSatNoiTru;
            }
            set
            {
                _LinkKhaoSatNoiTru = value;
                RaisePropertyChanged("LinkKhaoSatNoiTru");
            }
        }
        private string _DeptCheckPainLevel;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DeptCheckPainLevel
        {
            get
            {
                return _DeptCheckPainLevel;
            }
            set
            {
                _DeptCheckPainLevel = value;
                RaisePropertyChanged("DeptCheckPainLevel");
            }
        }
        //▼====: #051
        private bool _CheckPatientInfoWhenSavePrescript;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool CheckPatientInfoWhenSavePrescript
        {
            get
            {
                return _CheckPatientInfoWhenSavePrescript;
            }
            set
            {
                _CheckPatientInfoWhenSavePrescript = value;
                RaisePropertyChanged("CheckPatientInfoWhenSavePrescript");
            }
        }
        //▲====: #051
        //▼====: #053
        private int _Temp12PrintingMode;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Temp12PrintingMode
        {
            get
            {
                return _Temp12PrintingMode;
            }
            set
            {
                _Temp12PrintingMode = value;
                RaisePropertyChanged("Temp12PrintingMode");
            }
        }
        //▲====: #053
        private int _CountSendTransaction;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CountSendTransaction
        {
            get
            {
                return _CountSendTransaction;
            }
            set
            {
                _CountSendTransaction = value;
                RaisePropertyChanged("CountSendTransaction");
            }
        }
        //▼====: #054
        private int _MaxNumDayPrescriptAllow;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int MaxNumDayPrescriptAllow
        {
            get
            {
                return _MaxNumDayPrescriptAllow;
            }
            set
            {
                _MaxNumDayPrescriptAllow = value;
                RaisePropertyChanged("MaxNumDayPrescriptAllow");
            }
        }
        private int _MaxNumDayPrescriptAllow_InPt;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int MaxNumDayPrescriptAllow_InPt
        {
            get
            {
                return _MaxNumDayPrescriptAllow_InPt;
            }
            set
            {
                _MaxNumDayPrescriptAllow_InPt = value;
                RaisePropertyChanged("MaxNumDayPrescriptAllow_InPt");
            }
        }
        //▲====: #054
        //▼====: #055
        private bool _IsSeparatePrescription;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsSeparatePrescription
        {
            get
            {
                return _IsSeparatePrescription;
            }
            set
            {
                _IsSeparatePrescription = value;
                RaisePropertyChanged("IsSeparatePrescription");
            }
        }
        private string _ReportHospitalPhone;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ReportHospitalPhone
        {
            get
            {
                return _ReportHospitalPhone;
            }
            set
            {
                _ReportHospitalPhone = value;
                RaisePropertyChanged("ReportHospitalPhone");
            }
        }
        //▲====: #055
        //▼====: #057
        private bool _IsDisableCreateMedicalFile;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsDisableCreateMedicalFile
        {
            get
            {
                return _IsDisableCreateMedicalFile;
            }
            set
            {
                _IsDisableCreateMedicalFile = value;
                RaisePropertyChanged("IsDisableCreateMedicalFile");
            }
        }
        //▲====: #057
        //▼====: #058
        private bool _IsEnablePrintReceiptAndRequest;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsEnablePrintReceiptAndRequest
        {
            get
            {
                return _IsEnablePrintReceiptAndRequest;
            }
            set
            {
                _IsEnablePrintReceiptAndRequest = value;
                RaisePropertyChanged("IsEnablePrintReceiptAndRequest");
            }
        }
        //▲====: #058
        //▼====: #059
        private bool _PrintTemp01KBCB;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool PrintTemp01KBCB
        {
            get
            {
                return _PrintTemp01KBCB;
            }
            set
            {
                _PrintTemp01KBCB = value;
                RaisePropertyChanged("PrintTemp01KBCB");
            }
        }
        //▲====: #059
        //▼====: #060
        private string _SymptomNotUseForAdmission;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string SymptomNotUseForAdmission
        {
            get
            {
                return _SymptomNotUseForAdmission;
            }
            set
            {
                _SymptomNotUseForAdmission = value;
                RaisePropertyChanged("SymptomNotUseForAdmission");
            }
        }
        //▲====: #060
        //▼====: #062
        private string _APISendHIReportAddress;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string APISendHIReportAddress
        {
            get
            {
                return _APISendHIReportAddress;
            }
            set
            {
                _APISendHIReportAddress = value;
                RaisePropertyChanged("APISendHIReportAddress");
            }
        }
        private bool _IsApplyAutoCreateHIReportWhenConfirmHI;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsApplyAutoCreateHIReportWhenConfirmHI
        {
            get
            {
                return _IsApplyAutoCreateHIReportWhenConfirmHI;
            }
            set
            {
                _IsApplyAutoCreateHIReportWhenConfirmHI = value;
                RaisePropertyChanged("IsApplyAutoCreateHIReportWhenConfirmHI");
            }
        }
        private bool _IsApplyAutoCreateHIReportWhenSettlement;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsApplyAutoCreateHIReportWhenSettlement
        {
            get
            {
                return _IsApplyAutoCreateHIReportWhenSettlement;
            }
            set
            {
                _IsApplyAutoCreateHIReportWhenSettlement = value;
                RaisePropertyChanged("IsApplyAutoCreateHIReportWhenSettlement");
            }
        }
        //▲====: #062
        //▼====: #063
        private bool _IsSaveMedicalInstructionWithoutPrescription;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsSaveMedicalInstructionWithoutPrescription
        {
            get
            {
                return _IsSaveMedicalInstructionWithoutPrescription;
            }
            set
            {
                _IsSaveMedicalInstructionWithoutPrescription = value;
                RaisePropertyChanged("IsSaveMedicalInstructionWithoutPrescription");
            }
        }
        //▲====: #063
        //▼====: #067
        private bool _IsApplyCreateRequestForEstimation;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsApplyCreateRequestForEstimation
        {
            get
            {
                return _IsApplyCreateRequestForEstimation;
            }
            set
            {
                _IsApplyCreateRequestForEstimation = value;
                RaisePropertyChanged("IsApplyCreateRequestForEstimation");
            }
        }
        //▲====: #067
        //▼====: #065
        private bool _IsEnableAddRegPackByDoctor;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsEnableAddRegPackByDoctor
        {
            get
            {
                return _IsEnableAddRegPackByDoctor;
            }
            set
            {
                _IsEnableAddRegPackByDoctor = value;
                RaisePropertyChanged("IsEnableAddRegPackByDoctor");
            }
        }
        //▲====: #065
        //▼====: #070
        private int _ElectronicPrescriptionMaxReport;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int ElectronicPrescriptionMaxReport
        {
            get
            {
                return _ElectronicPrescriptionMaxReport;
            }
            set
            {
                _ElectronicPrescriptionMaxReport = value;
                RaisePropertyChanged("ElectronicPrescriptionMaxReport");
            }
        }
        //▲====: #070
        private bool _ApplyReport130;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool ApplyReport130
        {
            get
            {
                return _ApplyReport130;
            }
            set
            {
                _ApplyReport130 = value;
                RaisePropertyChanged("ApplyReport130");
            }
        }
        private string _DeptLocIDApplyQMS;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DeptLocIDApplyQMS
        {
            get
            {
                return _DeptLocIDApplyQMS;
            }
            set
            {
                _DeptLocIDApplyQMS = value;
                RaisePropertyChanged("DeptLocIDApplyQMS");
            }
        }

        private bool _IsEnableFilterPerformStaff;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsEnableFilterPerformStaff
        {
            get
            {
                return _IsEnableFilterPerformStaff;
            }
            set
            {
                _IsEnableFilterPerformStaff = value;
                RaisePropertyChanged("IsEnableFilterPerformStaff");
            }
        }
        private string _LocationAllowPrenatalCertificates;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LocationAllowPrenatalCertificates
        {
            get
            {
                return _LocationAllowPrenatalCertificates;
            }
            set
            {
                _LocationAllowPrenatalCertificates = value;
                RaisePropertyChanged("LocationAllowPrenatalCertificates");
            }
        }
        private string _InsuranceCertificatePrefix;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string InsuranceCertificatePrefix
        {
            get
            {
                return _InsuranceCertificatePrefix;
            }
            set
            {
                _InsuranceCertificatePrefix = value;
                RaisePropertyChanged("InsuranceCertificatePrefix");
            }
        }
        private long _DeptIDKhoaSan;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long DeptIDKhoaSan
        {
            get
            {
                return _DeptIDKhoaSan;
            }
            set
            {
                _DeptIDKhoaSan = value;
                RaisePropertyChanged("DeptIDKhoaSan");
            }
        }

        private string _ListObjectTypeIDForMngt;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ListObjectTypeIDForMngt
        {
            get
            {
                return _ListObjectTypeIDForMngt;
            }
            set
            {
                _ListObjectTypeIDForMngt = value;
                RaisePropertyChanged("ListObjectTypeIDForMngt");
            }
        }

        private bool _EnableCheckPaymentCeilingForTechService;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool EnableCheckPaymentCeilingForTechService
        {
            get
            {
                return _EnableCheckPaymentCeilingForTechService;
            }
            set
            {
                _EnableCheckPaymentCeilingForTechService = value;
                RaisePropertyChanged("EnableCheckPaymentCeilingForTechService");
            }
        }
        private bool _CheckHIWhenConfirm;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool CheckHIWhenConfirm
        {
            get
            {
                return _CheckHIWhenConfirm;
            }
            set
            {
                _CheckHIWhenConfirm = value;
                RaisePropertyChanged("CheckHIWhenConfirm");
            }
        }

        //▼====: #072
        private string _SMS_API_Url;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string SMS_API_Url
        {
            get
            {
                return _SMS_API_Url;
            }
            set
            {
                _SMS_API_Url = value;
                RaisePropertyChanged("SMS_API_Url");
            }
        }

        private bool _IsEnableSendSMSLab;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsEnableSendSMSLab
        {
            get
            {
                return _IsEnableSendSMSLab;
            }
            set
            {
                _IsEnableSendSMSLab = value;
                RaisePropertyChanged("IsEnableSendSMSLab");
            }
        }
        //▲====: #072
        private string _BacSiTruongPhoKhoa;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string BacSiTruongPhoKhoa
        {
            get
            {
                return _BacSiTruongPhoKhoa;
            }
            set
            {
                _BacSiTruongPhoKhoa = value;
                RaisePropertyChanged("BacSiTruongPhoKhoa");
            }
        }
        private string _ThuTruongDonVi;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ThuTruongDonVi
        {
            get
            {
                return _ThuTruongDonVi;
            }
            set
            {
                _ThuTruongDonVi = value;
                RaisePropertyChanged("ThuTruongDonVi");
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "HealthInsuranceElement", Namespace = "http://schemas.datacontract.org/2004/07/eHCMS.Configurations")]
    public partial class HealthInsuranceElement : object, System.ComponentModel.INotifyPropertyChanged
    {

        private ObservableCollection<string> _crossRegionCodeAcceptedListField;
        
        private long _hiPolicyMinSalaryField;

        private float _hiPolicyPercentageOnPayableField;

        private double _rebatePercentageLevel1Field;

        private int _paperReferalMaxDaysField;

        //TxD 24/05/2014 Adding more Fields below
        private bool _specialRuleForHIConsultationApplied;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool SpecialRuleForHIConsultationApplied
        {
            get { return _specialRuleForHIConsultationApplied; }
            set 
            {
                _specialRuleForHIConsultationApplied = value;
                RaisePropertyChanged("SpecialRuleForHIConsultationApplied");
            }
        }
        private double _HIConsultationServiceHIAllowedPrice;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double HIConsultationServiceHIAllowedPrice
        {
            get { return _HIConsultationServiceHIAllowedPrice;  }
            set
            {
                _HIConsultationServiceHIAllowedPrice = value;
                RaisePropertyChanged("HIConsultationServiceHIAllowedPrice");
            }
        }
        private int _Apply15HIPercent;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Apply15HIPercent
        {
            get { return _Apply15HIPercent;  }
            set
            {
                _Apply15HIPercent = value;
                RaisePropertyChanged("Apply15HIPercent");
            }
        }
        private int _RefundOrCancelCashReceipt;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int RefundOrCancelCashReceipt
        {
            get { return _RefundOrCancelCashReceipt;  }
            set
            {
                _RefundOrCancelCashReceipt = value;
                RaisePropertyChanged("RefundOrCancelCashReceipt");
            }
        }
        private int _PharmacyMaxDaysHIRebate_NgoaiTru;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int PharmacyMaxDaysHIRebate_NgoaiTru
        {
            get { return _PharmacyMaxDaysHIRebate_NgoaiTru;  }
            set
            {
                _PharmacyMaxDaysHIRebate_NgoaiTru = value;
                RaisePropertyChanged("PharmacyMaxDaysHIRebate_NgoaiTru");
            }
        }
        private int _PharmacyMaxDaysHIRebate_NoiTru;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int PharmacyMaxDaysHIRebate_NoiTru
        {
            get { return _PharmacyMaxDaysHIRebate_NoiTru; }
            set
            {
                _PharmacyMaxDaysHIRebate_NoiTru = value;
                RaisePropertyChanged("PharmacyMaxDaysHIRebate_NoiTru");
            }
        }
        private int _DifferenceDayPrecriptHI;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int DifferenceDayPrecriptHI
        {
            get { return _DifferenceDayPrecriptHI;  }
            set
            {
                _DifferenceDayPrecriptHI = value;
                RaisePropertyChanged("DifferenceDayPrecriptHI");
            }
        }
        private int _DifferenceDayRegistrationHI;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int DifferenceDayRegistrationHI
        {
            get { return _DifferenceDayRegistrationHI; }
            set
            {
                _DifferenceDayRegistrationHI = value;
                RaisePropertyChanged("DifferenceDayRegistrationHI");
            }
        }
        private int _MaxDaySellPrescriptInsurance;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int MaxDaySellPrescriptInsurance
        {
            get { return _MaxDaySellPrescriptInsurance; }
            set
            {
                _MaxDaySellPrescriptInsurance = value;
                RaisePropertyChanged("MaxDaySellPrescriptInsurance");
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute]
        public ObservableCollection<string> CrossRegionCodeAcceptedList
        {
            get
            {
                return _crossRegionCodeAcceptedListField;
            }
            set
            {
                if (ReferenceEquals(_crossRegionCodeAcceptedListField, value) != true)
                {
                    _crossRegionCodeAcceptedListField = value;
                    RaisePropertyChanged("CrossRegionCodeAcceptedList");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public long HiPolicyMinSalary
        {
            get
            {
                return _hiPolicyMinSalaryField;
            }
            set
            {
                if (_hiPolicyMinSalaryField.Equals(value) != true)
                {
                    _hiPolicyMinSalaryField = value;
                    RaisePropertyChanged("HiPolicyMinSalary");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public float HiPolicyPercentageOnPayable
        {
            get
            {
                return _hiPolicyPercentageOnPayableField;
            }
            set
            {
                if (_hiPolicyPercentageOnPayableField.Equals(value) != true)
                {
                    _hiPolicyPercentageOnPayableField = value;
                    RaisePropertyChanged("HiPolicyPercentageOnPayable");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public double RebatePercentageLevel1
        {
            get
            {
                return _rebatePercentageLevel1Field;
            }
            set
            {
                if (_rebatePercentageLevel1Field.Equals(value) != true)
                {
                    _rebatePercentageLevel1Field = value;
                    RaisePropertyChanged("RebatePercentageLevel1");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int PaperReferalMaxDays
        {
            get
            {
                return _paperReferalMaxDaysField;
            }
            set
            {
                _paperReferalMaxDaysField = value;
                RaisePropertyChanged("PaperReferalMaxDays");
            }
        }

        private bool _applyHINewRule20150101;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool ApplyHINewRule20150101
        {
            get
            {
                return _applyHINewRule20150101;
            }
            set
            {
                _applyHINewRule20150101 = value;
                RaisePropertyChanged("ApplyHINewRule20150101");
            }
        }

        //Huyen 12/08/2015: Thêm thuộc tính để kích hoạt các chức năng theo luật bảo hiểm mới tháng 07/2015
        //Thuộc tính ApplyHINew_Report20150701 đã được tạo ra trong AxServerConfig
        //Đối với nhóm HealthInsuranceElement cần thêm vào ServiceProxyClasses lần nữa Client mới có thể nhìn thấy
        private bool _applyHINew_Report20150701;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool ApplyHINew_Report20150701
        {
            get
            {
                return _applyHINew_Report20150701;
            }
            set
            {
                _applyHINew_Report20150701 = value;
            }
        }
        //Huyen_end
        //HPT: Cấu hình có hay không ràng buộc mã code trong giấy chuyển viện
        private bool _IsCheckHICodeInPaperReferal;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsCheckHICodeInPaperReferal
        {
            get
            {
                return _IsCheckHICodeInPaperReferal;
            }
            set
            {
                _IsCheckHICodeInPaperReferal = value;
                RaisePropertyChanged("IsCheckHICodeInPaperReferal");
            }
        }
        
        //HPT: Cấu hình có hay không bắt buộc nhập địa chỉ thường trú của bệnh nhân trong thẻ BHYT
        private bool _CheckAddressInHealthInsurance;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool CheckAddressInHealthInsurance
        {
            get
            {
                return _CheckAddressInHealthInsurance;
            }
            set
            {
                _CheckAddressInHealthInsurance = value;
                RaisePropertyChanged("CheckAddressInHealthInsurance");
            }
        }

        private bool _allowOutPtCrossRegion;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool AllowOutPtCrossRegion
        {
            get
            {
                return _allowOutPtCrossRegion;
            }
            set
            {
                _allowOutPtCrossRegion = value;
                RaisePropertyChanged("AllowOutPtCrossRegion");
            }
        }

        private bool _AllowInPtCrossRegion;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool AllowInPtCrossRegion
        {
            get { return _AllowInPtCrossRegion; }
            set
            {
                _AllowInPtCrossRegion = value;
                RaisePropertyChanged("AllowInPtCrossRegion");
            }
        }


        private bool _calcHIBenefitBaseOnPatientClassType;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool CalcHIBenefitBaseOnPatientClassType
        {
            get
            {
                return _calcHIBenefitBaseOnPatientClassType;
            }
            set
            {
                _calcHIBenefitBaseOnPatientClassType = value;
                RaisePropertyChanged("CalcHIBenefitBaseOnPatientClassType");
            }
        }

        private bool _ValidateApplyingHIBenefit;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool ValidateApplyingHIBenefit
        {
            get
            {
                return _ValidateApplyingHIBenefit;
            }
            set
            {
                _ValidateApplyingHIBenefit = value;
                RaisePropertyChanged("ValidateApplyingHIBenefit");
            }
        }

        private bool _FiveYearNotPaidEnough;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool FiveYearNotPaidEnough
        {
            get
            {
                return _FiveYearNotPaidEnough;
            }
            set
            {
                _FiveYearNotPaidEnough = value;
                RaisePropertyChanged("FiveYearNotPaidEnough");
            }
        }

        private long _MaxHIPaidOnMoreAddedItem;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long MaxHIPaidOnMoreAddedItem
        {
            get
            {
                return _MaxHIPaidOnMoreAddedItem;
            }
            set
            {
                _MaxHIPaidOnMoreAddedItem = value;
                RaisePropertyChanged("MaxHIPaidOnMoreAddedItem");
            }
        }

        private string _NotPermittedHICard;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NotPermittedHICard
        {
            get
            {
                return _NotPermittedHICard;
            }
            set
            {
                _NotPermittedHICard = value;
                RaisePropertyChanged("NotPermittedHICard");
            }
        }

        private float[] _HIPercentOnDifDept;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public float[] HIPercentOnDifDept
        {
            get
            {
                return _HIPercentOnDifDept;
            }
            set
            {
                _HIPercentOnDifDept = value;
                RaisePropertyChanged("HIPercentOnDifDept");
            }
        }

        private bool _FullHIBenefitForConfirm;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool FullHIBenefitForConfirm
        {
            get
            {
                return _FullHIBenefitForConfirm;
            }
            set
            {
                _FullHIBenefitForConfirm = value;
                RaisePropertyChanged("FullHIBenefitForConfirm");
            }
        }

        private bool _FullHIOfServicesForConfirm;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool FullHIOfServicesForConfirm
        {
            get
            {
                return _FullHIOfServicesForConfirm;
            }
            set
            {
                _FullHIOfServicesForConfirm = value;
                RaisePropertyChanged("FullHIOfServicesForConfirm");
            }
        }

        private bool _UseConfirmRecalcHIOutPt;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool UseConfirmRecalcHIOutPt
        {
            get { return _UseConfirmRecalcHIOutPt; }
            set
            {
                _UseConfirmRecalcHIOutPt = value;
                RaisePropertyChanged("UseConfirmRecalcHIOutPt");
            }
        }

        private float _PercentForEkip;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public float PercentForEkip
        {
            get { return _PercentForEkip; }
            set
            {
                _PercentForEkip = value;
                RaisePropertyChanged("PercentForEkip");
            }
        }

        private float _PercentForOtherEkip;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public float PercentForOtherEkip
        {
            get { return _PercentForOtherEkip; }
            set
            {
                _PercentForOtherEkip = value;
                RaisePropertyChanged("PercentForOtherEkip");
            }
        }

        private double _RebatePercentage2015Level1_InPt;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double RebatePercentage2015Level1_InPt
        {
            get
            {
                return _RebatePercentage2015Level1_InPt;
            }
            set
            {
                if (_RebatePercentage2015Level1_InPt.Equals(value) != true)
                {
                    _RebatePercentage2015Level1_InPt = value;
                    RaisePropertyChanged("RebatePercentage2015Level1_InPt");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "HospitalElement", Namespace = "http://schemas.datacontract.org/2004/07/eHCMS.Configurations")]
    public partial class HospitalElement : object, System.ComponentModel.INotifyPropertyChanged
    {

        private string _hospitalCodeField;

        private string _logoImagePathField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string HospitalCode
        {
            get
            {
                return _hospitalCodeField;
            }
            set
            {
                if (ReferenceEquals(_hospitalCodeField, value) != true)
                {
                    _hospitalCodeField = value;
                    RaisePropertyChanged("HospitalCode");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LogoImagePath
        {
            get
            {
                return _logoImagePathField;
            }
            set
            {
                if (ReferenceEquals(_logoImagePathField, value) != true)
                {
                    _logoImagePathField = value;
                    RaisePropertyChanged("LogoImagePath");
                }
            }
        }

        //TxD 24/05/2014 Adding more Fields below
        private int _KhoaPhongKham;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int KhoaPhongKham
        {
            get { return _KhoaPhongKham; }
            set
            {
                _KhoaPhongKham = value;
                RaisePropertyChanged("KhoaPhongKham");
            }
        }
        private int _FindRegistrationInDays_NgoaiTru;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int FindRegistrationInDays_NgoaiTru
        {
            get { return _FindRegistrationInDays_NgoaiTru;  }
            set
            {
                _FindRegistrationInDays_NgoaiTru = value;
                RaisePropertyChanged("FindRegistrationInDays_NgoaiTru");
            }
        }
        private int _EffectedDiagHours;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int EffectedDiagHours
        {
            get { return _EffectedDiagHours;  }
            set
            {
                _EffectedDiagHours = value;
                RaisePropertyChanged("EffectedDiagHours");
            }
        }
        private int _EffectedPCLHours;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int EffectedPCLHours
        {
            get { return _EffectedPCLHours; }
            set
            {
                _EffectedPCLHours = value;
                RaisePropertyChanged("EffectedPCLHours");
            }
        }
        private int _EditDiagDays;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int EditDiagDays
        {
            get { return _EditDiagDays; }
            set
            {
                _EditDiagDays = value;
                RaisePropertyChanged("EditDiagDays");
            }
        }
        private int _RoomFunction;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int RoomFunction
        {
            get { return _RoomFunction; }
            set
            {
                _RoomFunction = value;
                RaisePropertyChanged("RoomFunction");
            }
        }
        private int _LaboRmTp;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int LaboRmTp
        {
            get { return _LaboRmTp; }
            set
            {
                _LaboRmTp = value;
                RaisePropertyChanged("LaboRmTp");
            }
        }
        private bool _IsConfirmHI;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsConfirmHI
        {
            get { return _IsConfirmHI; }
            set
            {
                _IsConfirmHI = value;
                RaisePropertyChanged("IsConfirmHI");
            }
        }
        private double _MinPatientCashAdvance;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double MinPatientCashAdvance
        {
            get { return _MinPatientCashAdvance; }
            set
            {
                _MinPatientCashAdvance = value;
                RaisePropertyChanged("MinPatientCashAdvance");
            }
        }
        private string _PCLResourcePool;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PCLResourcePool
        {
            get { return _PCLResourcePool; }
            set
            {
                _PCLResourcePool = value;
                RaisePropertyChanged("PCLResourcePool");
            }
        }
        private string _PCLStorePool;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PCLStorePool
        {
            get { return _PCLStorePool; }
            set
            {
                _PCLStorePool = value;
                RaisePropertyChanged("PCLStorePool");
            }
        }
        private string _PCLThumbTemp;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PCLThumbTemp
        {
            get { return _PCLThumbTemp; }
            set
            {
                _PCLThumbTemp = value;
                RaisePropertyChanged("PCLThumbTemp");
            }
        }
        private int _RegistrationVIP;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int RegistrationVIP
        {
            get { return _RegistrationVIP; }
            set
            {
                _RegistrationVIP = value;
                RaisePropertyChanged("RegistrationVIP");
            }
        }
        private int _NeedICD10;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int NeedICD10
        {
            get { return _NeedICD10; }
            set
            {
                _NeedICD10 = value;
                RaisePropertyChanged("NeedICD10");
            }
        }

        private bool _IsDirectorSignature;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsDirectorSignature
        {
            get { return _IsDirectorSignature; }
            set
            {
                _IsDirectorSignature = value;
                RaisePropertyChanged("IsDirectorSignature");
            }
        }

        private long _SurgeryDeptID;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long SurgeryDeptID
        {
            get { return _SurgeryDeptID; }
            set
            {
                _SurgeryDeptID = value;
                RaisePropertyChanged("SurgeryDeptID");
            }
        }

        private string _HIAPILoginAccount;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string HIAPILoginAccount
        {
            get { return _HIAPILoginAccount; }
            set
            {
                _HIAPILoginAccount = value;
                RaisePropertyChanged("HIAPILoginAccount");
            }
        }

        private string _HIAPILoginPassword;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string HIAPILoginPassword
        {
            get { return _HIAPILoginPassword; }
            set
            {
                _HIAPILoginPassword = value;
                RaisePropertyChanged("HIAPILoginPassword");
            }
        }
        //▼====: #018
        private bool _BlockAddictiveAndPsychotropicDrugRequest;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool BlockAddictiveAndPsychotropicDrugRequest
        {
            get { return _BlockAddictiveAndPsychotropicDrugRequest; }
            set
            {
                _BlockAddictiveAndPsychotropicDrugRequest = value;
                RaisePropertyChanged("BlockAddictiveAndPsychotropicDrugRequest");
            }
        }
        //▲====: #018
        //▼====: #020
        private string _PrescriptionMainRightHeader;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PrescriptionMainRightHeader
        {
            get { return _PrescriptionMainRightHeader; }
            set
            {
                _PrescriptionMainRightHeader = value;
                RaisePropertyChanged("PrescriptionMainRightHeader");
            }
        }
        private string _PrescriptionSubRightHeader;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PrescriptionSubRightHeader
        {
            get { return _PrescriptionSubRightHeader; }
            set
            {
                _PrescriptionSubRightHeader = value;
                RaisePropertyChanged("PrescriptionSubRightHeader");
            }
        }
        //▲====: #020
        //▼====: #061
        private string _FTPAdminUserName;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FTPAdminUserName
        {
            get { return _FTPAdminUserName; }
            set
            {
                _FTPAdminUserName = value;
                RaisePropertyChanged("FTPAdminUserName");
            }
        }
        private string _FTPAdminPassword;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FTPAdminPassword
        {
            get { return _FTPAdminPassword; }
            set
            {
                _FTPAdminPassword = value;
                RaisePropertyChanged("FTPAdminPassword");
            }
        }
        //▲====: #061

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "PCLElement", Namespace = "http://schemas.datacontract.org/2004/07/eHCMS.Configurations")]
    public partial class PCLElement : object, System.ComponentModel.INotifyPropertyChanged
    {
        //▼====: #031
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool AutoCreatePACWorklist
        {
            get
            {
                return _AutoCreatePACWorklist;
            }
            set
            {
                if (ReferenceEquals(_AutoCreatePACWorklist, value) != true)
                {
                    _AutoCreatePACWorklist = value;
                    RaisePropertyChanged("AutoCreatePACWorklist");
                }
            }
        }
        private bool _AutoCreatePACWorklist;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PACSAPIAddress
        {
            get
            {
                return _PACSAPIAddress;
            }
            set
            {
                if (ReferenceEquals(_PACSAPIAddress, value) != true)
                {
                    _PACSAPIAddress = value;
                    RaisePropertyChanged("PACSAPIAddress");
                }
            }
        }
        private string _PACSAPIAddress;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PACUserName
        {
            get
            {
                return _PACUserName;
            }
            set
            {
                if (ReferenceEquals(_PACUserName, value) != true)
                {
                    _PACUserName = value;
                    RaisePropertyChanged("PACUserName");
                }
            }
        }
        private string _PACUserName;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PACPassword
        {
            get
            {
                return _PACPassword;
            }
            set
            {
                if (ReferenceEquals(_PACPassword, value) != true)
                {
                    _PACPassword = value;
                    RaisePropertyChanged("PACPassword");
                }
            }
        }
        private string _PACPassword;
        //▲====: #031

        private string _pclImageStoragePathField;
        private string _netWorkMapDriverField;
        private string _nwmdUserField;
        private string _nwmdPassField;
        private string _localFolderNameField;
        private int _requireDiagnosisForInPtPCLReq;
        private short _MaxEchogramImageFile;
        private string _Ab_Liver;
        private string _Ab_Gallbladder;
        private string _Ab_Pancreas;
        private string _Ab_Spleen;
        private string _Ab_LeftKidney;
        private string _Ab_RightKidney;
        private string _Ab_Bladder;
        private string _Ab_Prostate;
        private string _Ab_Uterus;
        private string _Ab_RightOvary;
        private string _Ab_LeftOvary;
        private string _Ab_PeritonealFluid;
        private string _Ab_PleuralFluid;
        private string _Ab_AbdominalAortic;
        private string _Ab_Conclusion;
        private bool _AutoCreateRISWorklist;
        private string _RISAPIAddress;
        private string _PCLImageURL;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PclImageStoragePath
        {
            get
            {
                return _pclImageStoragePathField;
            }
            set
            {
                if (ReferenceEquals(_pclImageStoragePathField, value) != true)
                {
                    _pclImageStoragePathField = value;
                    RaisePropertyChanged("PclImageStoragePath");
                }
            }
        }
        private string _ImageCaptureFileLocalStorePath;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ImageCaptureFileLocalStorePath
        {
            get { return _ImageCaptureFileLocalStorePath; }
            set
            {
                _ImageCaptureFileLocalStorePath = value;
                RaisePropertyChanged("ImageCaptureFileLocalStorePath");
            }
        }

        private bool _SaveImgWhenCapturing_Local;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool SaveImgWhenCapturing_Local
        {
            get { return _SaveImgWhenCapturing_Local; }
            set
            {
                _SaveImgWhenCapturing_Local = value;
                RaisePropertyChanged("SaveImgWhenCapturing_Local");
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NetWorkMapDriver
        {
            get
            {
                return _netWorkMapDriverField;
            }
            set
            {
                if (ReferenceEquals(_netWorkMapDriverField, value) != true)
                {
                    _netWorkMapDriverField = value;
                    RaisePropertyChanged("NetWorkMapDriver");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NWMDUser
        {
            get
            {
                return _nwmdUserField;
            }
            set
            {
                if (ReferenceEquals(_nwmdUserField, value) != true)
                {
                    _nwmdUserField = value;
                    RaisePropertyChanged("NWMDUser");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NWMDPass
        {
            get
            {
                return _nwmdPassField;
            }
            set
            {
                if (ReferenceEquals(_nwmdPassField, value) != true)
                {
                    _nwmdPassField = value;
                    RaisePropertyChanged("NWMDPass");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LocalFolderName
        {
            get
            {
                return _localFolderNameField;
            }
            set
            {
                if (ReferenceEquals(_localFolderNameField, value) != true)
                {
                    _localFolderNameField = value;
                    RaisePropertyChanged("LocalFolderName");
                }
            }
        }

        //20161014 CMN Begin: Add Permit for FileStore
        [System.Runtime.Serialization.DataMemberAttribute()]
        public short MaxEchogramImageFile
        {
            get
            {
                return _MaxEchogramImageFile;
            }
            set
            {
                if (ReferenceEquals(_MaxEchogramImageFile, value) != true)
                {
                    _MaxEchogramImageFile = value;
                    RaisePropertyChanged("MaxEchogramImageFile");
                }
            }
        }
        //20161014 CMN End.

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Ab_Liver
        {
            get
            {
                return _Ab_Liver;
            }
            set
            {
                _Ab_Liver = value;
                RaisePropertyChanged("Ab_Liver");
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Ab_Gallbladder
        {
            get
            {
                return _Ab_Gallbladder;
            }
            set
            {
                _Ab_Gallbladder = value;
                RaisePropertyChanged("Ab_Gallbladder");
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Ab_Pancreas
        {
            get
            {
                return _Ab_Pancreas;
            }
            set
            {
                _Ab_Pancreas = value;
                RaisePropertyChanged("Ab_Pancreas");
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Ab_Spleen
        {
            get
            {
                return _Ab_Spleen;
            }
            set
            {
                _Ab_Spleen = value;
                RaisePropertyChanged("Ab_Spleen");
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Ab_LeftKidney
        {
            get
            {
                return _Ab_LeftKidney;
            }
            set
            {
                _Ab_LeftKidney = value;
                RaisePropertyChanged("Ab_LeftKidney");
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Ab_RightKidney
        {
            get
            {
                return _Ab_RightKidney;
            }
            set
            {
                _Ab_RightKidney = value;
                RaisePropertyChanged("Ab_RightKidney");
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Ab_Bladder
        {
            get
            {
                return _Ab_Bladder;
            }
            set
            {
                _Ab_Bladder = value;
                RaisePropertyChanged("Ab_Bladder");
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Ab_Prostate
        {
            get
            {
                return _Ab_Prostate;
            }
            set
            {
                _Ab_Prostate = value;
                RaisePropertyChanged("Ab_Prostate");
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Ab_Uterus
        {
            get
            {
                return _Ab_Uterus;
            }
            set
            {
                _Ab_Uterus = value;
                RaisePropertyChanged("Ab_Uterus");
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Ab_RightOvary
        {
            get
            {
                return _Ab_RightOvary;
            }
            set
            {
                _Ab_RightOvary = value;
                RaisePropertyChanged("Ab_RightOvary");
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Ab_LeftOvary
        {
            get
            {
                return _Ab_LeftOvary;
            }
            set
            {
                _Ab_LeftOvary = value;
                RaisePropertyChanged("Ab_LeftOvary");
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Ab_PeritonealFluid
        {
            get
            {
                return _Ab_PeritonealFluid;
            }
            set
            {
                _Ab_PeritonealFluid = value;
                RaisePropertyChanged("Ab_PeritonealFluid");
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Ab_PleuralFluid
        {
            get
            {
                return _Ab_PleuralFluid;
            }
            set
            {
                _Ab_PleuralFluid = value;
                RaisePropertyChanged("Ab_PleuralFluid");
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Ab_AbdominalAortic
        {
            get
            {
                return _Ab_AbdominalAortic;
            }
            set
            {
                _Ab_AbdominalAortic = value;
                RaisePropertyChanged("Ab_AbdominalAortic");
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Ab_Conclusion
        {
            get
            {
                return _Ab_Conclusion;
            }
            set
            {
                _Ab_Conclusion = value;
                RaisePropertyChanged("Ab_Conclusion");
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int RequireDiagnosisForPCLReq
        {
            get
            {
                return _requireDiagnosisForInPtPCLReq;
            }
            set
            {
                _requireDiagnosisForInPtPCLReq = value;
                RaisePropertyChanged("RequireDiagnosisForInPtPCLReq");
            }
        }
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool AutoCreateRISWorklist
        {
            get
            {
                return _AutoCreateRISWorklist;
            }
            set
            {
                _AutoCreateRISWorklist = value;
                RaisePropertyChanged("AutoCreateRISWorklist");
            }
        }
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string RISAPIAddress
        {
            get
            {
                return _RISAPIAddress;
            }
            set
            {
                _RISAPIAddress = value;
                RaisePropertyChanged("RISAPIAddress");
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PCLImageURL
        {
            get
            {
                return _PCLImageURL;
            }
            set
            {
                if (ReferenceEquals(_PCLImageURL, value) != true)
                {
                    _PCLImageURL = value;
                    RaisePropertyChanged("PCLImageURL");
                }
            }
        }
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "ServerElement", Namespace = "http://schemas.datacontract.org/2004/07/eHCMS.Configurations")]
    public partial class ServerElement : object, System.ComponentModel.INotifyPropertyChanged
    {
        private string _outstandingServerIpField;
        private long _outstandingServerPortField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string OutstandingServerIP
        {
            get
            {
                return _outstandingServerIpField;
            }
            set
            {
                if (ReferenceEquals(_outstandingServerIpField, value) != true)
                {
                    _outstandingServerIpField = value;
                    RaisePropertyChanged("OutstandingServerIP");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public long OutstandingServerPort
        {
            get
            {
                return _outstandingServerPortField;
            }
            set
            {
                _outstandingServerPortField = value;
                RaisePropertyChanged("OutstandingServerPort");
            }
        }

        private string _ImageCaptureFilePublicStorePath;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ImageCaptureFilePublicStorePath
        {
            get
            {
                return _ImageCaptureFilePublicStorePath;
            }
            set
            {
                if (ReferenceEquals(_ImageCaptureFilePublicStorePath, value) != true)
                {
                    _ImageCaptureFilePublicStorePath = value;
                    RaisePropertyChanged("ImageCaptureFilePublicStorePath");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "AxServerConfigSection", Namespace = "http://schemas.datacontract.org/2004/07/eHCMS.Configurations")]
    public partial class AxServerConfigSection : object, System.ComponentModel.INotifyPropertyChanged
    {
        private CommonItemElement _commonItemsField;

        private HealthInsuranceElement _healthInsurancesField;

        private HospitalElement _hospitalsField;

        private PCLElement _pclsField;

        private ServerElement _serversField;

        private PharmacyConfigElement _pharmacyConfigField;

        private MedDeptConfigElement _medDeptElements;

        private ClinicDeptConfigElement _clinicDeptElements;

        private OutRegisConfigElement _outRegisElements;

        private InRegisConfigElement _inRegisElements;

        private ConsultationConfigElement _consultationElements;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public PharmacyConfigElement PharmacyElements
        {
            get { return _pharmacyConfigField; }
            set
            {
                _pharmacyConfigField = value;
                RaisePropertyChanged("PharmacyElements");
            }
        }

        //KMx: Cấu hình cho Khoa Dược.
        [System.Runtime.Serialization.DataMemberAttribute()]
        public MedDeptConfigElement MedDeptElements
        {
            get { return _medDeptElements; }
            set
            {
                _medDeptElements = value;
                RaisePropertyChanged("MedDeptElements");
            }
        }

        //KMx: Cấu hình cho kho phòng.
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ClinicDeptConfigElement ClinicDeptElements
        {
            get { return _clinicDeptElements; }
            set
            {
                _clinicDeptElements = value;
                RaisePropertyChanged("ClinicDeptElements");
            }
        }

        //KMx: Cấu hình cho đăng ký ngoại trú.
        [System.Runtime.Serialization.DataMemberAttribute()]
        public OutRegisConfigElement OutRegisElements
        {
            get { return _outRegisElements; }
            set
            {
                _outRegisElements = value;
                RaisePropertyChanged("OutRegisElements");
            }
        }

        //KMx: Cấu hình cho đăng ký nội trú.
        [System.Runtime.Serialization.DataMemberAttribute()]
        public InRegisConfigElement InRegisElements
        {
            get { return _inRegisElements; }
            set
            {
                _inRegisElements = value;
                RaisePropertyChanged("InRegisElements");
            }
        }

        //KMx: Cấu hình cho khám bệnh (29/08/2014 09:45).
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ConsultationConfigElement ConsultationElements
        {
            get { return _consultationElements; }
            set
            {
                _consultationElements = value;
                RaisePropertyChanged("ConsultationElements");
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public CommonItemElement CommonItems
        {
            get
            {
                return _commonItemsField;
            }
            set
            {
                if (ReferenceEquals(_commonItemsField, value) != true)
                {
                    _commonItemsField = value;
                    RaisePropertyChanged("CommonItems");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public HealthInsuranceElement HealthInsurances
        {
            get
            {
                return _healthInsurancesField;
            }
            set
            {
                if (ReferenceEquals(_healthInsurancesField, value) != true)
                {
                    _healthInsurancesField = value;
                    RaisePropertyChanged("HealthInsurances");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public HospitalElement Hospitals
        {
            get
            {
                return _hospitalsField;
            }
            set
            {
                if (ReferenceEquals(_hospitalsField, value) != true)
                {
                    _hospitalsField = value;
                    RaisePropertyChanged("Hospitals");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public PCLElement Pcls
        {
            get
            {
                return _pclsField;
            }
            set
            {
                if (ReferenceEquals(_pclsField, value) != true)
                {
                    _pclsField = value;
                    RaisePropertyChanged("Pcls");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ServerElement Servers
        {
            get
            {
                return _serversField;
            }
            set
            {
                if (ReferenceEquals(_serversField, value) != true)
                {
                    _serversField = value;
                    RaisePropertyChanged("Servers");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "AxException", Namespace = "http://schemas.datacontract.org/2004/07/ErrorLibrary")]
    public partial class AxException : object, System.ComponentModel.INotifyPropertyChanged
    {

        private string _classNameField;

        private string _errorCodeField;

        private string _errorMessageField;

        private AxException _innerExceptionField;

        private string _methodNameField;

        private string _moduleNameField;

        [System.Runtime.Serialization.DataMemberAttribute]
        public string ClassName
        {
            get
            {
                return _classNameField;
            }
            set
            {
                if (ReferenceEquals(_classNameField, value) != true)
                {
                    this._classNameField = value;
                    this.RaisePropertyChanged("ClassName");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ErrorCode
        {
            get
            {
                return _errorCodeField;
            }
            set
            {
                if (ReferenceEquals(_errorCodeField, value) != true)
                {
                    _errorCodeField = value;
                    RaisePropertyChanged("ErrorCode");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ErrorMessage
        {
            get
            {
                return _errorMessageField;
            }
            set
            {
                if (ReferenceEquals(_errorMessageField, value) != true)
                {
                    _errorMessageField = value;
                    RaisePropertyChanged("ErrorMessage");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public AxException InnerException
        {
            get
            {
                return _innerExceptionField;
            }
            set
            {
                if (ReferenceEquals(_innerExceptionField, value) != true)
                {
                    _innerExceptionField = value;
                    RaisePropertyChanged("InnerException");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string MethodName
        {
            get
            {
                return _methodNameField;
            }
            set
            {
                if (ReferenceEquals(_methodNameField, value) != true)
                {
                    _methodNameField = value;
                    RaisePropertyChanged("MethodName");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ModuleName
        {
            get
            {
                return _moduleNameField;
            }
            set
            {
                if (ReferenceEquals(_moduleNameField, value) != true)
                {
                    _moduleNameField = value;
                    RaisePropertyChanged("ModuleName");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    

    [System.Runtime.Serialization.DataContractAttribute(Name = "AxResponse", Namespace = "http://schemas.datacontract.org/2004/07/ErrorLibrary")]
    public partial class AxResponse : object, System.ComponentModel.INotifyPropertyChanged
    {

        private AxException _exceptionField;

        private object _resultField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public AxException Exception
        {
            get
            {
                return _exceptionField;
            }
            set
            {
                if (ReferenceEquals(_exceptionField, value) != true)
                {
                    _exceptionField = value;
                    RaisePropertyChanged("Exception");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public object Result
        {
            get
            {
                return _resultField;
            }
            set
            {
                if (ReferenceEquals(_resultField, value) != true)
                {
                    _resultField = value;
                    RaisePropertyChanged("Result");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class AxErrorEventArgs : EventArgs
    {
        public object UserState { get; set; }
        public FaultException<AxException> ServerError
        {
            get;
            private set;
        }
        public Exception ClientError
        {
            get;
            private set;
        }
        public AxErrorEventArgs()
        {
            ServerError = null;
        }
        public AxErrorEventArgs(Exception clientError)
        {
            ClientError = clientError;
        }
        public AxErrorEventArgs(FaultException<AxException> error)
        {
            ServerError = error;
        }
        public AxErrorEventArgs(FaultException<AxException> error, object userState)
            : this(error)
        {
            UserState = userState;
        }
        public override string ToString()
        {
            if (ServerError != null)
            {
                return ServerError.Detail.ErrorMessage;
            }
            return ClientError != null ? ClientError.Message : "";
        }
    }

    public class PdfPrinterEventArgs : EventArgs
    {
        public PdfPrinterEventArgs() { }
        public PdfPrinterEventArgs(string fName, bool preview, bool deleteFileWhenCompleted)
        {
            this.FileName = fName;
            this.ShowPreview = preview;
            this.DeleteFileAfterPrinting = deleteFileWhenCompleted;
        }
        public PdfPrinterEventArgs(string fName)
            : this(fName, false, true)
        {
        }
        /// <summary>
        /// Ten file pdf can in
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Check xem co nen preview truoc khi in khong
        /// </summary>
        public bool ShowPreview { get; set; }

        /// <summary>
        /// Co nen xoa file sau khi in khong
        /// </summary>
        public bool DeleteFileAfterPrinting { get; set; }
    }
}
