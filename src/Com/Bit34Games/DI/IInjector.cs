using System;
using System.Collections.Generic;
using Com.Bit34Games.DI.Error;
using Com.Bit34Games.DI.Provider;

namespace Com.Bit34Games.DI
{
    public interface IInjector
    {
        //  MEMBERS
        bool HasErrors { get; }

        //  METHODS
        IInstanceProviderSetter<T> AddBinding<T>();
        void InjectInto(object container, IMemberInjector injectionOverride = null);
        T GetInstance<T>();
        IEnumerator<T> GetAssignableInstances<T>();
    }
}
