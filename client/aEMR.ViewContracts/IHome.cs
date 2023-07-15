namespace aEMR.ViewContracts
{
    public enum MainMenuItemType
    {
        eRegisMenu = 1,
        eConsultMenu = 2,
        eApptMenu = 3,
        ePharmMenu = 4,
        eImagePCLMenu = 5,
        eLabPCLMenu = 6,
        eDrugDeptMenu = 7,
        eClinicDeptMenu = 8,
        eStoreMenu = 9,
        eTranRepMenu = 10,
        eUserAccMenu = 11,
        eConfigManMenu = 12,
        eResourceManMenu = 13,
        eSystemConfigMenu = 14,
        eGenEnqMenu = 15,
        eMedItemDeptMenu = 16
    };
    public interface IHome
    {
        object ActiveContent { get; set; }
        object LeftMenu { get; set; }
        object TopMenuItems { get; set; }
        object OutstandingTaskContent { get; set; }
        bool FindRegistrationCmdVisibility { get; set; }
        void ResetDefault();

        bool HomeMenuPanelBusy { get; set; }
        void LoadAllHospitalInfoAction();

        void MainMenuItem_Click(object sender, MainMenuItemType menuType);
        bool IsEnableLeftMenu { get; set; }
        bool IsExpandLeftMenu { get; set; }

        bool IsEnableOST { get; set; }
        bool IsExpandOST { get; set; }
    }
}