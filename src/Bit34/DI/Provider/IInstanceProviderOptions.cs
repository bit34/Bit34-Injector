using System;

namespace Bit34.DI.Provider
{
    public interface IInstanceProviderOptions
    {
        void SetPostInjectionCallback(Action<object> postInjectionCallback);
    }
}