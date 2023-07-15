using aEMR.Controls;
using System.Windows;
using System.Windows.Controls;

namespace eHCMSApp
{
    public static class FormExtensions
    {
        /// <summary>
        /// Reset lai validationsummary cho datagrid. Vi khi cancel edit no khong clear error list trong validationsummary
        /// </summary>
        /// <param name="valSum"></param>
        public static void ResetValidationSummary(this UIElement root, ValidationSummary valSum)
        {
            UIElement elem = valSum.Target;
            valSum.Target = null;
            valSum.Target = elem;
        }
        public static void SetFocus(this Control root)
        {
            root.Focus(); 
        }
    }
}