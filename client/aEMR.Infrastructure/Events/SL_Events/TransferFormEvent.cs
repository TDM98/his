using DataEntities;
/*
 * 20220909 #001 DatTB: Thêm event load "Lời dặn" qua tab Xuất viện
 */
namespace aEMR.Infrastructure.Events
{
    public class TransferFormEvent
    {
        public TransferForm Item { get; set; }

        public class OnChangedPaperReferal
        {
            public TransferForm TransferForm { get; set; }
        }

        //▼==== #001
        public class OnChangedUpdatePrescription
        {
            public string AdmissionInfoComment { get; set; }
        }
        //▲==== #001
    }
}
