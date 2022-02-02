using System;
using Com.Bit34Games.Injector.Error;

namespace Com.Bit34Games.Injector
{
    public interface IInjectorTester
    {
        //  MEMBERS
        bool HasErrors     { get; }
        int  BindingCount  { get; }
        int  ProviderCount { get; }
        int  ErrorCount    { get; }

        //  METHODS
        InjectionError GetError(int index);
        bool           HasBindingForType(Type type);
    }
}
