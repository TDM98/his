using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using EPos.ViewContracts;

namespace ESalePos.ViewModels
{
    [Export(typeof(IBaseWindowViewModel))]
    public class BaseWindowViewModel : Conductor<object>, IBaseWindowViewModel
    {
    }
}
