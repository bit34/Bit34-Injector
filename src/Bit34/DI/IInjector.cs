using System;
using System.Collections.Generic;
using Bit34.DI.Error;
using Bit34.DI.Provider;

namespace Bit34.DI
{
    public interface IInjector
    {
        //  MEMBERS
        int ErrorCount{get;}

        //  METHODS
        IInstanceProviderSetter AddBinding<T>();
        bool HasBindingForType(Type type);
        InjectionError GetError(int index);
        void InjectInto(object container, IMemberInjector injectionOverride = null);
        T GetInstance<T>();
        IEnumerator<T> GetAssignableInstances<T>();
    }
}
