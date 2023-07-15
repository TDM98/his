
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface ILoginInfo
    {
        Staff CurrentStaff { get; set; }
        bool isPreNoteTemp { get; set; }

        // TxD 22/09/2018 Added the following to ALLOW child VM of each Tab to get ACCESS to the CENTRAL Data stored in the MainKB (Main KHAM BENH) Screen
        ICS_DataStorage CS_DS { get; set; }

    }
}
