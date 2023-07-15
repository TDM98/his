using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using DataEntities;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using eHCMSLanguage;
using System.Linq;
using System.Collections.ObjectModel;
using aEMR.Controls;
using System.IO;
using DirectShowLib;
using aEMR.Common.Collections;
using System;

namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof(IFormulaMedicine)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class FormulaMedicineViewModel : Conductor<object>, IFormulaMedicine
    {
        [ImportingConstructor]
        public FormulaMedicineViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
        }
        #region Properties

        private string _Title;
        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title != value)
                {
                    _Title = value;
                }
                NotifyOfPropertyChange(() => Title);
            }
        }

        private double? _NumberA ;
        public double? NumberA
        {
            get { return _NumberA; }
            set
            {
                if (_NumberA != value)
                {
                    _NumberA = value;
                }
                NotifyOfPropertyChange(() => NumberA);
            }
        }
        private double? _NumberB ;
        public double? NumberB
        {
            get { return _NumberB; }
            set
            {
                if (_NumberB != value)
                {
                    _NumberB = value;
                }
                NotifyOfPropertyChange(() => NumberB);
            }
        }
        private double? _NumberC ;
        public double? NumberC
        {
            get { return _NumberC; }
            set
            {
                if (_NumberC != value)
                {
                    _NumberC = value;
                }
                NotifyOfPropertyChange(() => NumberC);
            }
        }
        private double? _NumberD ;
        public double? NumberD
        {
            get { return _NumberD; }
            set
            {
                if (_NumberD != value)
                {
                    _NumberD = value;
                }
                NotifyOfPropertyChange(() => NumberD);
            }
        }
        private double? _NumberE ;
        public double? NumberE
        {
            get { return _NumberE; }
            set
            {
                if (_NumberE != value)
                {
                    _NumberE = value;
                }
                NotifyOfPropertyChange(() => NumberE);
            }
        }
        private double? _NumberF ;
        public double? NumberF
        {
            get { return _NumberF; }
            set
            {
                if (_NumberF != value)
                {
                    _NumberF = value;
                }
                NotifyOfPropertyChange(() => NumberF);
            }
        }
        private double? _NumberV1 ;
        public double? NumberV1
        {
            get { return _NumberV1; }
            set
            {
                if (_NumberV1 != value)
                {
                    _NumberV1 = value;
                }
                NotifyOfPropertyChange(() => NumberV1);
            }
        }
        private double? _NumberV2 ;
        public double? NumberV2
        {
            get { return _NumberV2; }
            set
            {
                if (_NumberV2 != value)
                {
                    _NumberV2 = value;
                }
                NotifyOfPropertyChange(() => NumberV2);
            }
        }
        #endregion
        #region Visible Properties
        private Visibility _vThyroidVolumeCal = Visibility.Collapsed;
        public Visibility vThyroidVolumeCal
        {
            get { return _vThyroidVolumeCal; }
            set
            {
                if (_vThyroidVolumeCal != value)
                {
                    _vThyroidVolumeCal = value;
                    NotifyOfPropertyChange(() => vThyroidVolumeCal);
                }
            }
        }
        #endregion

        #region Functions
        public void SetCollapseAll()
        {
            vThyroidVolumeCal = Visibility.Collapsed;
        }
        public void btnThyroidVolumeCal()
        {
            SetCollapseAll();
            Title = eHCMSResources.Z2596_G1_TheTichTuyenGiap.ToUpper();
            vThyroidVolumeCal = Visibility.Visible;
        }

        public void btnCalThyroid()
        {
            if (NumberA > 0 && NumberB > 0 && NumberC > 0)
            {
                NumberV1 = Math.Round( (((double)NumberA * (double)NumberB * (double)NumberC) / 2) ,2);
            }

            if (NumberD > 0 && NumberE > 0 && NumberF > 0)
            {
                NumberV2= Math.Round( (((double)NumberD * (double)NumberE * (double)NumberF) / 2) ,2);
            }

        }
        #endregion
    }
}