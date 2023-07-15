using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Castle.Core.Logging;
using Castle.Windsor;
using FluentValidation;
using Action = System.Action;

namespace aEMR.Infrastructure.ViewUtils
{
    public abstract class CommonView<T> : Screen, ICommonView
    {
        protected ILogger Logger { get; set; }
        protected IWindsorContainer Container { get; set; }
        
        protected CommonView(IWindsorContainer container)
        {
            Container = container;
            Logger = Container.Resolve<ILogger>();
        }

        public virtual void BeforeInitial() { }
        public abstract void Initial();
        public virtual void AfterInitial() { }

        protected override void OnActivate()
        {
            base.OnActivate();
            try
            {
                BeforeInitial();
                Initial();
                AfterInitial();

            }
            catch(Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            if ( close)
            {
               CloseCommand(); 
            }
        }

        public virtual void PrintCommand() { }
        public virtual void ExportCommand() { }
        public virtual void SearchCommand() { }
        public virtual void CloseCommand() { }
    }
}
