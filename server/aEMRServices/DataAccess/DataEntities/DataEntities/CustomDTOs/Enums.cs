using System.ComponentModel;
using System.Runtime.Serialization;

namespace DataEntities
{

    public enum PatientRegistrationStatus:byte
    {
        INVALID = 0,
        OPENED=1,
        ONPROCESS=2,
        COMPLETED=3
    }
   
    [DataContract]
    public enum RecordState : byte
    {
        [EnumMember]
        DETACHED = 0,
        [EnumMember]
        UNCHANGED = 1,
        [EnumMember]
        ADDED = 2,
        [EnumMember]
        DELETED = 3,
        [EnumMember]
        MODIFIED = 4,
        [EnumMember]
        MERGER = 5
    }

    public enum PatientRegistrationType : byte
    {
        DK_KHAM_BENH_NGOAI_TRU = 1,
        DK_KHAM_BENH_NOI_TRU = 2,
        DK_XET_NGHIEM_CAN_LAM_SANG = 3,
        DK_NHAP_VIEN = 4,
        DK_TIEU_PHAU = 5,
        DK_DAI_PHAU = 6,
        DK_CAP_CUU_KCB = 7,
        DK_CAP_CUU_TAI_NAN = 8,
        DK_HEN_BENH = 9
    }

    public enum ChargeableItemType : byte
    {
        NONE =0,
        SERVICES = 1,
        DRUGS = 2,
        MEDICAL_ITEMS = 3,
        PCL = 4,
        CHEMICAL= 5
    }

    [DataContract]
    public enum CommonRecordState : byte
    {
        [EnumMember]
        UNCHANGED = 0,
        [EnumMember]
        INSERTED = 1,
        [EnumMember]
        UPDATED = 2,
        [EnumMember]
        DELETED = 3
    }

    public enum ePatientClassification : long
    {
        [Description("Bệnh nhân không BHYT")]
        PatientService = 1,
        [Description("Bệnh nhân có BHYT")]
        HIService = 2,
        [Description("Dịch vụ")]
        Service = 3,
        [Description("Miễn phí")]
        Free = 4,
        [Description("Nghiên cứu")]
        Research = 5,
        [Description("Nghiên cứu Bramhs")]
        Bramhs = 6,
        [Description("Nhân viên")]
        Employee = 7,
        [Description("Hoạt chất")]
        PayAfter = 8,
        [Description("Khám sức khỏe")]
        CompanyHealthRecord = 9
    }
}