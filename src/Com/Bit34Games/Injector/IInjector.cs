using System;
using System.Collections.Generic;
using Com.Bit34Games.Injector.Error;
using Com.Bit34Games.Injector.Provider;

namespace Com.Bit34Games.Injector
{
    public interface IInjector
    {
        //  METHODS
        IInstanceProviderSetter<T> AddBinding<T>();
        void InjectInto(object container, IMemberInjector injectionOverride = null);
        T GetInstance<T>();
        IEnumerator<T> GetAssignableInstances<T>();
    }
}
