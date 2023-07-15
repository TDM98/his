using System;
using System.Collections.Generic;
/*
 * 20221216 #001 TNHX: 994 Response cua API đơn thuốc điện tử
 */
namespace DataEntities
{
    public class HIAPILogin
    {
        public int maKetQua { get; set; }
        public APIKey APIKey { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }
    public class APIKey
    {
        public string access_token { get; set; }
        public string id_token { get; set; }
        public string token_type { get; set; }
        public DateTime expires_in { get; set; }
    }
    public class HIAPICheckedHICard
    {
        public int maKetQua { get; set; }
        public string ghiChu { get; set; }
        public string maThe { get; set; }
        public string hoTen { get; set; }
        public string ngaySinh { get; set; }
        public string gioiTinh { get; set; }
        public string diaChi { get; set; }
        public string maDKBD { get; set; }
        public string cqBHXH { get; set; }
        public string maSoBHXH { get; set; }
        public string gtTheTu { get; set; }
        public string gtTheDen { get; set; }
        public string maKV { get; set; }
        public string ngayDu5Nam { get; set; }
        public string maTheCu { get; set; }
        public string maTheMoi { get; set; }
        public string gtTheTuMoi { get; set; }
        public string gtTheDenMoi { get; set; }
        public string maDKBDMoi { get; set; }
        public string tenDKBDMoi { get; set; }
        public List<dsLichSuKCB2018> dsLichSuKCB2018 { get; set; }
        public List<dsLichSuKT2018> dsLichSuKT2018 { get; set; }
    }
    public class dsLichSuKCB2018
    {
        public string maHoSo { get; set; }
        public string maCSKCB { get; set; }
        public string ngayVao { get; set; }
        public string ngayRa { get; set; }
        public string tenBenh { get; set; }
        public string tinhTrang { get; set; }
        public string kqDieuTri { get; set; }
    }
    public class dsLichSuKT2018
    {
        public string userKT { get; set; }
        public string thoiGianKT { get; set; }
        public string thongBao { get; set; }
        public string maLoi { get; set; }
    }
    public class HIAPIUploadHIReportXmlResult
    {
        public int maKetQua { get; set; }
        public string maGiaoDich { get; set; }
    }

    public partial class HIAPIServiceResponse
    {
        public int ResCode { get; set; }
        public string codeDesc { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
    }
    //▼====: #001
    public partial class DTDTAPIResponse
    {
        public string token { get; set; }
        public string token_type { get; set; }
        public string success { get; set; }
        public string error { get; set; }
        public string checksum { get; set; }
        public bool IsTransferCompleted { get; set; }
        public List<long> ListPrescriptionErrorWhenCallAPI { get; set; }
    }
    //▲====: #001
}
