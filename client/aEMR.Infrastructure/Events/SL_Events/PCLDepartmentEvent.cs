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
using DataEntities;

namespace aEMR.Infrastructure.Events
{

    //Danh sách cls yêu cầu
    public class ReLoadListPCLRequest
    {
    }

    //Danh sách cls đang xử lý
    public class ReLoadListPCLRequestProcessing
    {
    }

    //Danh Sách cls chưa xử lý
    public class ReLoadListPCLRequestWaiting
    {
    }

    public class ReloadOutStandingStaskPCLRequest
    {
    }

    public class hplInputKetQua_Click<TPCLReq, TPCLReqDetail>
    {
        public TPCLReq PCLReq { get; set; }
        public TPCLReqDetail PCLReqDetail { get; set; }
    }

    public class btChooseFileResultForPCL_Click<TFile, TTypeFile,TStreamFile>
    {
        public TFile File { get; set; }
        public TTypeFile TypeFile { get; set; }
        public TStreamFile StreamFile { get; set; }
    }

    //public class ChangePCLDepartmentEvent
    //{

    //}

    public class InitialPCLImage_Step1_Event
    {

    }

    public class InitialPCLImage_Step2_Event
    {

    }

    public class InitialPCLImage_Step3_Event
    {

    }

    public class TinhTongTienDuTruEvent
    {

    }

    public class LoadFormInputResultPCL_ImagingEvent
    {
    }

    public class ReaderInfoPatientFromPatientPCLReqEvent<TPCLRequest>
    {
        public TPCLRequest PCLRequest { get; set; }
        //20210818 BLQ: Thêm biến IsReadOnly để khi bắn sự kiện từ tìm kiếm 
        public bool IsReadOnly { get; set; }
    }

    public class PCLRequestDetailRemoveEvent<TPCLRequestDetail>
    {
        public TPCLRequestDetail PCLRequestDetail { get; set; }
    }

    public class AddPCLResultFileStorageDetailsComplete
    {
        
    }

    public class Event_SearchAbUltraRequestCompleted
    {
        public Patient Patient { get; set; }
        public PatientPCLRequest PCLRequest { get; set; }
    }

}
