using System;

namespace DataEntities
{
    //public class TicketIssue
    //{
    //    public long TicketID { get; set; }
    //    public DateTime IssueDateTime { get; set; }
    //    public string TicketNumberText { get; set; }
    //    public int TicketNumberSeq { get; set; }
    //    public string HICardNo { get; set; }
    //}

    public class TicketType
    {
        public long TicketTypeID { get; set; }
        public string Description { get; set; }
        public string PrefixString { get; set; }
        public string SuffixString { get; set; }
        public string V_TicketTypeDefinition { get; set; }
        public string SequenceNumFormat { get; set; }

    }

    public enum V_TicketStatus_Enum { UNK_STATUS = 0, TKT_WAITING_REGIS = 20, TKT_ALREADY_REGIS = 21, TKT_BEING_CALLED = 22, TKT_CANCELED = 23 };
    public enum V_IssueBy_Enum { SCAN = 1, STAFF = 2, STAFF_SCAN = 3, CUS_CARE_SCAN = 4 };
    public enum V_TicketType_Enum { DV = 30, BH = 31, UT = 32 };
    public class TicketIssue
    {
        public long TicketID { get; set; }
        public DateTime IssueDateTime { get; set; }
        public string TicketNumberText { get; set; }
        public int TicketNumberSeq { get; set; }
        public string HICardNo { get; set; }
        public int V_TicketStatus { get; set; }
        public int SaveToDb_StatusFlag { get; set; }    // 0 = Already Saved or Updated , 1 = New for Insertion, 2 = Update is Required

        public long TicketTypeID { get; set; }
        public long WorkingCounterID { get; set; }
        public string PatientCode { get; set; }
        public DateTime RecCreatedDate { get; set; }
        public long CounterID { get; set; }
        public long nCounterNumber { get; set; }
        public DateTime TicketGetTime { get; set; }
        public int V_IssueBy { get; set; }
        public string SerialTicket { get; set; }
        public string PatientName { get; set; }
        public int PrintTimes { get; set; } = 1;
        public long StaffID { get; set; }
        public long CalledStaffID { get; set; }
        public DateTime CalledTime { get; set; }
    }
}