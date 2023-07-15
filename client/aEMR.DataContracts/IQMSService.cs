using DataEntities;
using System;
using System.Collections.Generic;
using System.ServiceModel;
namespace QMSService
{
    [ServiceContract]
    public interface IQMSService
    {
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetNextTicketIssue(long WorkingCounterDeptLocID, long StaffID, AsyncCallback callback, object state);
        TicketIssue EndGetNextTicketIssue(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateTicketStatusAfterRegister(string TicketNumberText, DateTime TicketGetTime, long WorkingCounterDeptLocID, AsyncCallback callback, object state);
        bool EndUpdateTicketStatusAfterRegister(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRecalQMSTicket(long WorkingCounterDeptLocID, string SerialTicket, DateTime TicketRecalTime, AsyncCallback callback, object state);
        TicketIssue EndRecalQMSTicket(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllTicketIssueByCounter(long WorkingCounterDeptLocID, DateTime TicketRecalTime, AsyncCallback callback, object state);
        List<TicketIssue> EndGetAllTicketIssueByCounter(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetCurrentQMSTicket(long WorkingCounterDeptLocID, DateTime TicketGetTime, AsyncCallback callback, object state);
        TicketIssue EndGetCurrentQMSTicket(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCallGetSeqNumberByTypeForCusCare(string aType, string PatientCode, string HICardNo, string PatientName, int V_IssueBy, bool IsGetTicketIssueAgain, long StaffID, AsyncCallback callback, object state);
        TicketIssue EndCallGetSeqNumberByTypeForCusCare(out string CounterName, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCallSpecialPriorTicket(long WorkingCounterDeptLocID, string SerialTicket, AsyncCallback callback, object state);
        TicketIssue EndCallSpecialPriorTicket(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginChangeWorkingCounterStatus(long DeptLocID, long StaffID, AsyncCallback callback, object state);
        bool EndChangeWorkingCounterStatus(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetWorkingCounterStatus(long DeptLocID, AsyncCallback callback, object state);
        bool EndGetWorkingCounterStatus(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeletedTicketForCusCare(string PatientCode, long StaffID, AsyncCallback callback, object state);
        bool EndDeletedTicketForCusCare(out string TicketNumberText, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetTicketByPatientCode(string PatientCode, AsyncCallback callback, object state);
        TicketIssue EndGetTicketByPatientCode(IAsyncResult asyncResult);

    }
}