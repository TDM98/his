
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace aEMR.Controls
{
    public class AxDatePicker : DatePicker
    {
        public AxDatePicker()
        {
            DefaultStyleKey = typeof(DatePicker);
        }
       
        DatePickerTextBox dtTextBox;
        Popup dtPopup;
        Button dtButton;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            dtTextBox = base.GetTemplateChild("TextBox") as DatePickerTextBox;
            dtPopup = base.GetTemplateChild("Popup") as Popup;
            dtButton = base.GetTemplateChild("Button") as Button;

            //dtTextBox.Watermark = "-- Select Date --";
            // TxD 04/06/2018 Commented OUT the following because It will CAUSE an exception in XAML 
            //                  when open under DESIGN MODE , not sure if Exception will also happen 
            //                  at runtime, to be REVIEWED if NOT
            //                  ALSO a new Style was ADDED to AppStyleNew.xaml called aEMR_TextBoxWaterMark which
            //                  can be used directly in XAML instead of code like here.
            //var theWatermark = dtTextBox.Template.FindName("Watermark", dtTextBox);
            //if (theWatermark != null)
            //{
            //    theWatermark = "-- Select Date --";
            //}

            if (dtTextBox != null)
            {
                dtTextBox.RemoveHandler(KeyDownEvent, new KeyEventHandler(dtTextBox_KeyDown));
                dtTextBox.AddHandler(KeyDownEvent, new KeyEventHandler(dtTextBox_KeyDown), true);
            }

        }
        void dtTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Tab) || e.Key.Equals(Key.Enter))
            {
                if (this.IsDropDownOpen)
                {
                }
                else
                {
                    ButtonAutomationPeer peer = new ButtonAutomationPeer(dtButton);
                    IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                    invokeProv.Invoke();

                    e.Handled = true;
                }
            }
        }
    }
}