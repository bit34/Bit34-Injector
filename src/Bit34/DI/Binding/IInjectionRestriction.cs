using System;
using Bit34.DI.Provider;

namespace Bit34.DI.Binding
{
    public interface IInjectionRestriction
    {
        //  METHODS
        bool Check(object container, Type typeToInject, IInstanceProvider provider);
        string GetInfo();
    }
}
