using System;
using System.Collections.Generic;
using Com.Bit34Games.Injector.Provider;

namespace Com.Bit34Games.Injector.Binding
{
    public interface IInjectionBinding : IInjectionBindingOptions
    {
        //	MEMBERS
        Type                        BindingType      { get; }
        IInstanceProvider           InstanceProvider { get; }
        List<IInjectionRestriction> RestrictionList  { get; }
    }
}