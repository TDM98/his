
using DataEntities;
using System.Windows;
/*
* 20220807 #001 DatTB: Báo cáo thống kê số lượng hồ sơ điều trị ngoại trú
* 20221117 #002 DatTB: - IssueID: 2241 | Thêm trường chọn mẫu cho các sổ khám bệnh.
* 20221201 #003 DatTB: Thêm báo cáo thống kê hồ sơ ĐTNT
* 20230111 #004 DatTB: Thêm ComboBox đối tượng khám bệnh
* 20230304 #005 DatTB: Thêm bộ lọc kho nhận
* 20230513 #006 DatTB: Thêm Báo cáo thời gian chờ tại bệnh viện
* 20230526 #007 DatTB: Thêm Báo cáo thống kê đơn thuốc điện tử
* 20230626 #008 QTD:   Thêm báo cáo ds BN gửi SMS
*/
namespace aEMR.ViewContracts
{
    public interface ICommonReportByDDMMYYYY
    {
        ReportName eItem { get; set; }
        ReportParameters RptParameters { get; set; }
        string strHienThi { get; set; }

        bool mDepartment { get; set; }
        bool mInPatientDeptStatus { get; set; }
        bool mRegistrationType { get; set; }

        bool mXemIn { get; set; }
        bool mIn { get; set; }
        bool mXemChiTiet { get; set; }
        bool isAllStaff { get; set; }
        void LoadListStaff(byte type);

        IAucHoldConsultDoctor aucHoldConsultDoctor { get; set; }

        int ReportSwitch { get; set; }

        // 20220409 DatTB: add IsEnabledFromDatePicker to filter ages for report DLS_BCThongTinDanhMucThuoc
        bool IsEnabledFromDatePicker { get; set; }

        bool IsEnabledToDatePicker { get; set; }
        bool IsShowPaymentMode { get; set; }

        bool vBaoCaoVienPhiBHYT { get; set; }
        int Case { get; set; }

        bool ShowPCLSection { get; set; }

        // 20190315 TNHX: [BM0006654] create report "BaoCaoNhapXuatTheoMucDich" && Get List WareHouse for selected
        void GetListStore(long? StoreType);
        bool ChonKho { get; set; }

        bool ShowLocation { get; set; }

        bool ShowMoreThreeDays { get; set; }
        // 20200409 TNHX: [] add ShowAges to filter ages for report BC_BNTaiKhamBenhManTinh
        bool ShowAges { get; set; }

        bool IsReportForKHTH { get; set; }

        //▼====: #20220217 QTD : add show for new filter report XRptDLS_BCXuatThuocBNKhoaPhong 
        bool IsShowPatientType { get; set; }

        bool IsShowFaName { get; set; }

        bool IsShowWeb { get; set; }

        bool IsDateTime { get; set; }

        // 20220524 DatTB: Thêm radio chọn chi tiết/ tổng hợp để xem in hoặc xuất excel
        bool rdtPrint { get; set; }

        //22020620 DatTB: Thêm điều kiện hiển thị filter danh mục xét nghiệm
        bool ShowPCLExamType { get; set; }

        //▼==== #001
        bool IsShowOutPtTreatmentStatus { get; set; }
        bool ShowLocationSelect { get; set; }
        bool ShowOutPtTreatmentType { get; set; }
        //▲==== #001

        //▼==== #002
        bool ShowHealthRecordsType { get; set; }
        //▲==== #002

        //▼==== #003
        bool ShowYNOutPtTreatmentCode { get; set; }

        bool ShowYNOutPtTreatmentType { get; set; }

        bool ShowYNOutPtTreatmentProgram { get; set; }

        bool ShowYNOutPtTreatmentFinal { get; set; }
        //▲==== #003

        //▼==== #004
        bool ShowPatientClassification { get; set; }
        //▲==== #004
        //▼==== #005
        void GetListInStore(long? StoreType);

        bool ChonKhoNhan { get; set; }
        //▲==== #005
        //▼==== #006
        bool ShowExaminationProcess { get; set; }
        //▲==== #006
        //▼==== #007
        bool ShowHIReportStatus { get; set; }
        bool ShowHIReportFindPatient { get; set; }
        //▲==== #007
        //▼==== #008
        bool ShowSMSStatus { get; set; }
        //▲==== #008
    }
}
