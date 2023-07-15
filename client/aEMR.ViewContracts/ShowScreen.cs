using System;
using Caliburn.Micro;

namespace aEMR.ViewContracts
{
    public class ShowScreen : IResult ,IDisposable
    {
        readonly Type screenType;
        readonly string name;
        
        public ShowScreen(string name)
        {
            this.name = name;
            
        }

        public ShowScreen(Type screenType)
        {
            this.screenType = screenType;
        }

        public void Execute(ActionExecutionContext context)
        {
            var screen = !string.IsNullOrEmpty(name)
                ? IoC.Get<object>(name)
                : IoC.GetInstance(screenType, null);
            if (screen != null)
            {
                //var home =  Globals.GetViewModel<IHome>();
                var home = GlobalsNAV.GetViewModel<IHome>();
                home.ActiveContent = screen;
                var activeScreen = (home as Conductor<object>);
                activeScreen.ActivateItem(screen);

            }
            
            Completed(this, new ResultCompletionEventArgs());
        }

        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

        public static ShowScreen Of<T>()
        {
            return new ShowScreen(typeof(T));
        }

        public void Dispose()
        {
            Completed -= delegate { };
            Completed = null;
        }
    }  
}
