using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IHtmlReport
    {
        void NavigateToString(string BodyContent);
    }
}