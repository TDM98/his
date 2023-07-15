using System;

namespace aEMR.Infrastructure.ServiceCore
{
    public interface IAsyncUtility
    {
        AsyncCallback DispatchCallback(AsyncCallback callback);
    }
}
