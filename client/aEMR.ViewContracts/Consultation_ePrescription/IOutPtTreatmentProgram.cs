using DataEntities;
using System;
/*
* 20220811 #001 DatTB: Chỉnh sửa màn hình hồ sơ ĐTNT
* + Không cho phép chỉnh sửa loại hình, ngày dự kiến tổng kết khi đã lưu toa thuốc
* + Thêm ShowDialog_V5 sử dụng Dictionary để truyền thêm title cho Dialog
* + Truyền thêm biến xác định hồ sơ đã xác nhận mới chặn sửa
* 20220812 #002 DatTB: Chỉnh sửa màn hình hồ sơ ĐTNT
* + Thêm hàm check toa thuốc để kiểm tra chỉnh sửa loại hình ĐTNT
* 20220813 #003 BLQ: Thêm interface IOutPtTreatmentProgramItem
*/
namespace aEMR.ViewContracts
{
    public interface IOutPtTreatmentProgram
    {
        PatientRegistration CurrentRegistration { get; set; }

        //▼==== #002
        long PtRegDetailID { get; set; }
        //▲==== #002
    }
    public interface IOutPtTreatmentProgramEdit
    {
        OutPtTreatmentProgram CurrentOutPtTreatmentProgram { get; set; }
        bool IsSuccessed { get; set; }

        //▼==== #001
        bool IsOutPtTProSubmited { get; set; }
        //▲==== #001

        //▼==== #002
        long PtRegDetailID { get; set; }
        //▲==== #002
    }
    //▼==== #003
    public interface IOutPtTreatmentProgramItem
    {
        PatientRegistration CurrentRegistration { get; set; }
        int MinNumOfDayMedicine { get; set; }
        int MaxNumOfDayMedicine { get; set; }
        long OutpatientTreatmentTypeID { get; set; }
    }
    //▲==== #003
}