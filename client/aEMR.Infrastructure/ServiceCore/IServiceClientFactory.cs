using System;

namespace aEMR.Infrastructure.ServiceCore
{
    public interface IServiceClientFactory<T> : IDisposable
    {
        T ServiceInstance
        {
            get;
        }

    }
}
