using System;
using Com.Bit34Games.Injector.Provider;

namespace Com.Bit34Games.Injector.Binding
{
    public interface IInjectionRestriction
    {
        //  METHODS
        bool   Check(object container, Type typeToInject, IInstanceProvider provider);
        string GetInfo();
    }
}
