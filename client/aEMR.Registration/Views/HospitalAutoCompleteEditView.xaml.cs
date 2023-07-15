using System.Windows.Controls;
using aEMR.ViewContracts;
using aEMR.Controls;
using System.Windows.Input;

namespace aEMR.Registration.Views
{
    /// <summary>
    /// Hien gio khong su dung.
    /// </summary>
    public partial class HospitalAutoCompleteEditView : AxUserControl
    {
        public HospitalAutoCompleteEditView()
        {
            InitializeComponent();
        }

        private void txtRegistrationCode_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = !IsValidIntegerKey((TextBox)sender, e.Key, (int)e.Key, false);
        }
        private static bool IsValidIntegerKey(TextBox textBox, Key key, int platformKeyCode, bool negativeAllowed)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                return false;
            }
            if (Key.D0 <= key && key <= Key.D9)
            {
                return true;
            }
            if (Key.NumPad0 <= key && key <= Key.NumPad9)
            {
                return true;
            }
            if (negativeAllowed && (key == Key.Subtract || platformKeyCode == 189))
            {
                return 0 == textBox.Text.Length;
            }
            return false;
        }
    }
}
