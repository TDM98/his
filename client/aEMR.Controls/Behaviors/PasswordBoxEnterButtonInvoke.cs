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
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls.Primitives;


namespace aEMR.Controls
{
    public class PasswordBoxEnterButtonInvoke : TargetedTriggerAction<PasswordBox>
    {
        /// <summary>
        /// Gets or sets the peer.
        /// </summary>
        /// <value>The peer.</value>
        private AutomationPeer _peer { get; set; }

        /// <summary>
        /// Gets or sets the target button
        /// </summary>
        private PasswordBox _targetedPasswordBox { get; set; }

        /// <summary>
        /// Called after the TargetedTriggerAction is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>Override this to hook up functionality to the AssociatedObject.</remarks>
        protected override void OnAttached()
        {
            base.OnAttached();
            _targetedPasswordBox = this.Target;
            if (null == _targetedPasswordBox)
            {
                return;
            }

            // set peer
            this._peer = FrameworkElementAutomationPeer.FromElement(_targetedPasswordBox);
            if (this._peer == null)
            {
                this._peer = FrameworkElementAutomationPeer.CreatePeerForElement(_targetedPasswordBox);
            }
        }

        /// <summary>
        /// Called after targeted Button change.
        /// </summary>
        /// <remarks>Override this to hook up functionality to the new targeted Button.</remarks>
        protected override void OnTargetChanged(PasswordBox oldTarget, PasswordBox newTarget)
        {
            base.OnTargetChanged(oldTarget, newTarget);
            _targetedPasswordBox = newTarget;
            if (null == _targetedPasswordBox)
            {
                return;
            }

            // set peer
            this._peer = FrameworkElementAutomationPeer.FromElement(_targetedPasswordBox);
            if (this._peer == null)
            {
                this._peer = FrameworkElementAutomationPeer.CreatePeerForElement(_targetedPasswordBox);
            }
        }

        /// <summary>
        /// Invokes the targeted Button when Enter key is pressed inside TextBox.
        /// </summary>
        /// <param name="parameter">KeyEventArgs with Enter key</param>
        protected override void Invoke(object parameter)
        {
            KeyEventArgs keyEventArgs = parameter as KeyEventArgs;
            if (null != keyEventArgs && keyEventArgs.Key == Key.Enter)
            {
                //if (null != _peer && _peer.IsEnabled())
                {
                    var binding = ((PasswordBox)this.AssociatedObject).GetBindingExpression(PasswordBox.PasswordCharProperty);
                    if (binding != null)
                        binding.UpdateSource();

                    IInvokeProvider invokeProvider = _peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                    invokeProvider.Invoke();
                }
            }
        }
    }
}
