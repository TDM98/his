using DataEntities;
using Caliburn.Micro;
using System.Collections.Generic;

namespace aEMR.ViewContracts
{
    public interface IInPatientSettlement
    {
        ISearchPatientAndRegistration SearchRegistrationContent { get; set; }
        IPatientSummaryInfoV2 PatientSummaryInfoContent { get; set; }

        PatientRegistration CurRegistration { get; set; }
        string DeptLocTitle { get; set; }

        bool UsedByTaiVuOffice { get; set; }


        // Hpt 27/11/2015: Thêm biến này vào Interface để truyền vào VM, làm trung gian để gán giá trị cho PatientFindBy trong SearchPatientRegistrationViewModel, không dùng bắn sự kiện nữa
        AllLookupValues.PatientFindBy PatientFindBy { get; set; }

        void LoadPaymentInfo();
      
    }
}
