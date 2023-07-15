using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aEMR.Infrastructure
{
    public interface IServiceClient<T> : IDisposable where T : class 
    {
        
    }
}
