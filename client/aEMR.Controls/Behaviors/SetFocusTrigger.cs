using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Interactivity;

namespace aEMR.Controls
{
    public class SetFocusTrigger : TargetedTriggerAction<Control>
    {
        /// <summary>
        /// Set focus to target control
        /// </summary>
        /// <param name="parameter">Not used</param>
        protected override void Invoke(object parameter)
        {
            // ignore null targets
            if (Target == null) return;

            // call Focus method
            // * note, this probably should check that a target control actually
            // * supports the Focus method, but we're just keeping it simple for now
            Target.Focus();
        }

        /// <summary>
        /// Set focus on the associated control (the control to which the trigger is attached)
        /// </summary>        
    }
    public class SetFocusOnMeTrigger : TriggerAction<Control>
    {
        /// <summary>
        /// Set focus on associated object (should be a control)
        /// </summary>
        /// <param name="parameter">Not used</param>
        protected override void Invoke(object parameter)
        {
            if (AssociatedObject is Control)
            {
                (AssociatedObject as Control).Focus();
            }
            else
            {
                if (AssociatedObject == null)
                {
                    System.Diagnostics.Debug.WriteLine("SetFocusOnMeTrigger cannot set focus because AssociatedObject is null");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("SetFocusOnMeTrigger cannot set focus because AssociatedObject is not a Control (type is '" + AssociatedObject.GetType() + "')");
                }
            }
        }
    }
}
