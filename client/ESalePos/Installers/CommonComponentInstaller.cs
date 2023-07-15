using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
//using EPos.CommonViews;
//using EPos.CommonViews.ViewModels;

namespace aEMRClient.Installers
{
    public class CommonComponentInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            //container.Register(Component.For<IImageCachingProcessor>().ImplementedBy<ImageCachingProcessor>().LifestyleSingleton());
        }
    }
}
