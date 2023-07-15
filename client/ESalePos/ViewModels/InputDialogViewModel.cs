using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Castle.Windsor;
using aEMR.Infrastructure.ViewUtils;
using aEMR.ViewContracts;

namespace aEMRClient.ViewModels
{
    [Export(typeof(IInputDialogViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InputDialogViewModel : CommonView<string>, IInputDialogViewModel
    {
        private readonly IWindsorContainer _container;
        private string _titleView;
        private string _comment;

        [ImportingConstructor]
        public InputDialogViewModel(IWindsorContainer container) : base(container)
        {
            _container = container;

        }

        public override void Initial()
        {
            
        }


        public string TitleView
        {
            get { return _titleView; }
            set
            {
                _titleView = value;
                NotifyOfPropertyChange(() => TitleView);
            }
        }

        public string Comment
        {
            get { return _comment; }
            set
            {
                _comment = value;
                NotifyOfPropertyChange(() => Comment);
            }
        }



    }
}
