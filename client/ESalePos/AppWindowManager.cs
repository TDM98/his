using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Caliburn.Micro;
using aEMR.ViewContracts;

namespace aEMRClient
{
    public class AppWindowManager : WindowManager
    {

        private Window _currentWindow = null;

        protected override Window EnsureWindow(object model, object view, bool isDialog)
        {
            var window = base.EnsureWindow(model, view, isDialog);

            if (_currentWindow != null)
            {
                window.Owner = _currentWindow;
                _currentWindow.Hide();
            }
            window.Closed += Window_Closed;
            _currentWindow = window;
           
            if ( model is IShellViewModel )
            {
                window.SizeToContent = SizeToContent.Manual;
                window.Width = 1200;
                window.Height = 700;
            }
            else
            {
                window.SizeToContent = SizeToContent.Manual;
                window.Width = 800;
                window.Height = 600;
    
            }


            return window;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            var window = sender as Window;
            if ( window == null)
            {
                return;
            }

            window.Closed -= Window_Closed;
            if (window.Owner != null)
            {
                window.Owner.Show();
            }
        }
    }
}
