using System;
using System.Collections.Generic;
using Bit34.DI.Provider;

namespace Bit34.DI.Binding
{
    public interface IInjectionBinding : IInjectionBindingOptions
    {
        //	MEMBERS
        Type BindingType { get; }
        IInstanceProvider InstanceProvider { get; }
        List<IInjectionRestriction> RestrictionList { get; }
    }
}