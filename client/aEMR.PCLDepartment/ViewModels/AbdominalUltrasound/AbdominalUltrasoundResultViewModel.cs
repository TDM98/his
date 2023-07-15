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
using aEMR.ViewContracts;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using DataEntities;
using aEMR.PCLDepartment.Views;
using System.Windows.Media.Imaging;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;

namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof(IAbdominalUltrasoundResult)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class AbdominalUltrasoundResultViewModel : Conductor<object>, IAbdominalUltrasoundResult
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public AbdominalUltrasoundResultViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }

        private Patient _curPatient;
        public Patient CurPatient
        {
            get { return _curPatient; }
            set
            {
                _curPatient = value;
                NotifyOfPropertyChange(() => CurPatient);
            }
        }


        private AbdominalUltrasound _currentAbUltra;
        public AbdominalUltrasound CurrentAbUltra
        {
            get { return _currentAbUltra; }
            set
            {
                if (_currentAbUltra == value)
                    return;
                _currentAbUltra = value;
                NotifyOfPropertyChange(() => CurrentAbUltra);
            }
        }

        private bool _isMale = true;
        public bool IsMale
        {
            get { return _isMale; }
            set
            {
                if (_isMale == value)
                    return;
                _isMale = value;
                NotifyOfPropertyChange(() => IsMale);
            }
        }


        public void SetResultDetails(Patient curPatient, object resDetails)
        {
            CurPatient = curPatient;

            if (CurPatient != null && CurPatient.Gender == "M")
            {
                IsMale = true;
            }
            else
            {
                IsMale = false;
            }

            CurrentAbUltra = (AbdominalUltrasound)resDetails;

            if (CurrentAbUltra != null && CurrentAbUltra.AbdominalUltrasoundID <= 0)
            {
                CurrentAbUltra.Liver = Globals.ServerConfigSection.Pcls.Ab_Liver;
                CurrentAbUltra.Gallbladder = Globals.ServerConfigSection.Pcls.Ab_Gallbladder;
                CurrentAbUltra.Pancreas = Globals.ServerConfigSection.Pcls.Ab_Pancreas;
                CurrentAbUltra.Spleen = Globals.ServerConfigSection.Pcls.Ab_Spleen;
                CurrentAbUltra.RightKidney = Globals.ServerConfigSection.Pcls.Ab_RightKidney;
                CurrentAbUltra.LeftKidney = Globals.ServerConfigSection.Pcls.Ab_LeftKidney;
                CurrentAbUltra.Bladder = Globals.ServerConfigSection.Pcls.Ab_Bladder;
                CurrentAbUltra.PeritonealFluid = Globals.ServerConfigSection.Pcls.Ab_PeritonealFluid;
                CurrentAbUltra.PleuralFluid = Globals.ServerConfigSection.Pcls.Ab_PleuralFluid;
                CurrentAbUltra.AbdominalAortic = Globals.ServerConfigSection.Pcls.Ab_AbdominalAortic;
                CurrentAbUltra.Conclusion = Globals.ServerConfigSection.Pcls.Ab_Conclusion;

                if (CurPatient != null && CurPatient.Gender == "M")
                {
                    CurrentAbUltra.Prostate = Globals.ServerConfigSection.Pcls.Ab_Prostate;
                }
                else
                {
                    CurrentAbUltra.Uterus = Globals.ServerConfigSection.Pcls.Ab_Uterus;
                    CurrentAbUltra.RightOvary = Globals.ServerConfigSection.Pcls.Ab_RightOvary;
                    CurrentAbUltra.LeftOvary = Globals.ServerConfigSection.Pcls.Ab_LeftOvary;
                }
            }
        }

        private string displayedImagePath = "/aEMR.CommonViews;component/Assets/Images/Abdominal/Abdominal.jpg";

        public string DisplayedImagePath
        {
            get 
            { 
                return displayedImagePath; 
            }
            set 
            { 
                displayedImagePath = value;
                NotifyOfPropertyChange(() => DisplayedImagePath);
            }
        }


        //27062018 TTM: Chuyển toàn bộ đường dẫn từ eHCMSCal => aEMR.CommonViews.
        public void StartStoryboard()
        {
            Storyboard sbImageAnimation = ((AbdominalUltrasoundResultView)this.GetView()).sbImageAnimation;
            sbImageAnimation.Begin();
        }

        public void Liver_Focus(object sender, RoutedEventArgs e)
        {
            DisplayedImagePath = "/aEMR.CommonViews;component/Assets/Images/Abdominal/Liver.jpg";
            StartStoryboard();
        }

        public void Gallbladder_Focus(object sender, RoutedEventArgs e)
        {

            DisplayedImagePath = "/aEMR.CommonViews;component/Assets/Images/Abdominal/Gallbladder.jpg";
            StartStoryboard();
        }

        public void Pancreas_Focus(object sender, RoutedEventArgs e)
        {
            DisplayedImagePath = "/aEMR.CommonViews;component/Assets/Images/Abdominal/Pancreas.jpg";
            StartStoryboard();
        }

        public void Spleen_Focus(object sender, RoutedEventArgs e)
        {
            DisplayedImagePath = "/aEMR.CommonViews;component/Assets/Images/Abdominal/Spleen.jpg";
            StartStoryboard();
        }

        public void RightKidney_Focus(object sender, RoutedEventArgs e)
        {
            DisplayedImagePath = "/aEMR.CommonViews;component/Assets/Images/Abdominal/RightKidney.jpg";
            StartStoryboard();
        }

        public void LeftKidney_Focus(object sender, RoutedEventArgs e)
        {
            DisplayedImagePath = "/aEMR.CommonViews;component/Assets/Images/Abdominal/LeftKidney.jpg";
            StartStoryboard();
        }

        public void Bladder_Focus(object sender, RoutedEventArgs e)
        {
            DisplayedImagePath = "/aEMR.CommonViews;component/Assets/Images/Abdominal/Bladder.jpg";
            StartStoryboard();
        }

        public void Prostate_Focus(object sender, RoutedEventArgs e)
        {
            DisplayedImagePath = "/aEMR.CommonViews;component/Assets/Images/Abdominal/Prostate.jpg";
            StartStoryboard();
        }

        public void Uterus_Focus(object sender, RoutedEventArgs e)
        {
            DisplayedImagePath = "/aEMR.CommonViews;component/Assets/Images/Abdominal/Uterus.jpg";
            StartStoryboard();
        }

        public void RightOvary_Focus(object sender, RoutedEventArgs e)
        {
            DisplayedImagePath = "/aEMR.CommonViews;component/Assets/Images/Abdominal/RightOvary.jpg";
            StartStoryboard();
        }

        public void LeftOvary_Focus(object sender, RoutedEventArgs e)
        {
            DisplayedImagePath = "/aEMR.CommonViews;component/Assets/Images/Abdominal/LeftOvary.jpg";
            StartStoryboard();
        }

        public void PeritonealFluid_Focus(object sender, RoutedEventArgs e)
        {
            DisplayedImagePath = "/aEMR.CommonViews;component/Assets/Images/Abdominal/PeritonealFluid.jpg";
            StartStoryboard();
        }

        public void PleuralFluid_Focus(object sender, RoutedEventArgs e)
        {
            DisplayedImagePath = "/aEMR.CommonViews;component/Assets/Images/Abdominal/PleuralFluid.jpg";
            StartStoryboard();
        }

        public void AbdominalAortic_Focus(object sender, RoutedEventArgs e)
        {
            DisplayedImagePath = "/aEMR.CommonViews;component/Assets/Images/Abdominal/AbdominalAortic.jpg";
            StartStoryboard();
        }

        public void Conclusion_Focus(object sender, RoutedEventArgs e)
        {
            DisplayedImagePath = "/aEMR.CommonViews;component/Assets/Images/Abdominal/Abdominal.jpg";
            StartStoryboard();
        }
    }
}
