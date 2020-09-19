using System;
using Com.Bit34Games.DI.Error;

namespace Com.Bit34Games.DI
{
    public interface IInjectorTester : IInjector
    {
        //  MEMBERS
        int BindingCount{ get; }
        int ProviderCount{ get; }
        int ErrorCount{get;}

        //  METHODS
        InjectionError GetError(int index);
        bool HasBindingForType(Type type);
    }
}
