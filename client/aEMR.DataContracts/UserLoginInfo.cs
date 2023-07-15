using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Documents;
//using System.Windows.Ink;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Animation;
//using System.Windows.Shapes;
using Caliburn.Micro;
using DataEntities;
using eHCMS.Services.Core.Base;

namespace Ax.Contracts.SL
{
    public class UserLoginInfo : NotifyChangedBase
    {
        private string _userName;

        [Required(ErrorMessage = "UserName is required")]
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
                ValidateProperty("UserName", value);
                RaisePropertyChanged("UserName");
            }
        }

        private string _password;
        [Required(ErrorMessage = "Password is required")]
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                ValidateProperty("Password", value);
                RaisePropertyChanged("Password");
            }
        }

        private bool _isRemembered;
        public bool IsRemembered
        {
            get
            {
                return _isRemembered;
            }
            set
            {
                _isRemembered = value;
                RaisePropertyChanged("IsRemembered");
            }
        }

        private bool _rememberMe;
        public bool RememberMe
        {
            get
            {
                return _rememberMe;
            }
            set
            {
                _rememberMe = value;
                RaisePropertyChanged("RememberMe");
            }
        }

        private Location _location;
        public Location Location
        {
            get
            {
                return _location;
            }
            set
            {
                _location = value;
                RaisePropertyChanged("Location");
            }
        }
    }
}
