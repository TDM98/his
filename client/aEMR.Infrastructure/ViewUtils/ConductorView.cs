using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Castle.Core.Logging;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using FluentValidation;
using Action = System.Action;

namespace aEMR.Infrastructure.ViewUtils
{
    public abstract class ConductorView<T> : Conductor<object>, ICommonView
    {
        protected ILogger Logger { get; set; }
        protected IWindsorContainer Container { get; set; }

        protected ConductorView(IWindsorContainer container) 
        {
            Container = container;
            Logger = Container.Resolve<ILogger>();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Initial();
        }

        public abstract void Initial();
    }
}
