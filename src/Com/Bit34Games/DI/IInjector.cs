using System;
using System.Collections.Generic;
using Com.Bit34Games.DI.Error;
using Com.Bit34Games.DI.Provider;

namespace Com.Bit34Games.DI
{
    public interface IInjector
    {
        //  MEMBERS
        int ErrorCount{get;}

        //  METHODS
        IInstanceProviderSetter<T> AddBinding<T>();
        bool HasBindingForType(Type type);
        InjectionError GetError(int index);
        void InjectInto(object container, IMemberInjector injectionOverride = null);
        T GetInstance<T>();
        IEnumerator<T> GetAssignableInstances<T>();
    }
}
