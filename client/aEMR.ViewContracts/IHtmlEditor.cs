using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IHtmlEditor
    {
        void LoadBaseSection(string BodyContent);
        string BodyContent { get; }
        string BodyContentText { get; }
        IRegistration_DataStorage Registration_DataStorage { get; set; }
    }
}
