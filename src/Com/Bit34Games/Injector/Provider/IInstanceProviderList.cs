using System;

namespace Com.Bit34Games.Injector.Provider
{
    //  Internal collaborator used by InjectionBinding<T>.ToValue / .ToType to ask
    //  the InjectorContext to create and register the underlying provider.
    //  Not part of the public surface — consumers should never call these directly.
    internal interface IInstanceProviderList
    {
        //  METHODS
        IInstanceProvider AddValueProvider(Type targetType, object value);
        IInstanceProvider AddTypedProvider<T>(Type targetType) where T : new();
    }
}
