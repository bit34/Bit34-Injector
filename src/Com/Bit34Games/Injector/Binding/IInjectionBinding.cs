using System;
using System.Collections.Generic;
using Com.Bit34Games.Injector.Provider;

namespace Com.Bit34Games.Injector.Binding
{
    /// <summary>
    /// Read-only view of a single registered binding — its binding type, the underlying
    /// <see cref="IInstanceProvider"/>, and any restrictions attached to it. Inherits the fluent
    /// <see cref="IInjectionBindingOptions"/> surface so consumers can keep chaining.
    /// </summary>
    public interface IInjectionBinding : IInjectionBindingOptions
    {
        //  MEMBERS

        /// <summary>The type that consumers request when they ask the injector for this binding.</summary>
        Type                                BindingType      { get; }

        /// <summary>The provider that creates or holds the instance returned for this binding.</summary>
        IInstanceProvider                   InstanceProvider { get; }

        /// <summary>
        /// Restrictions attached via <see cref="IInjectionBindingOptions.RestrictToNamespace(string)"/>
        /// (and friends). Read-only by design — only the binding itself appends through the fluent API.
        /// </summary>
        IReadOnlyList<IInjectionRestriction> RestrictionList { get; }
    }
}
