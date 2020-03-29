using System;
using Com.Bit34Games.DI.Provider;

namespace Com.Bit34Games.DI.Binding
{
    public interface IInjectionRestriction
    {
        //  METHODS
        bool Check(object container, Type typeToInject, IInstanceProvider provider);
        string GetInfo();
    }
}
