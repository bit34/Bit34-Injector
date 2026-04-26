using System;
using Com.Bit34Games.Injector.Provider;

namespace Com.Bit34Games.Injector.Binding
{
    /// <summary>
    /// Pluggable predicate consulted before the injector sets a value on a target member.
    /// Implement to add custom rules (e.g. namespace, attribute, or assembly checks).
    /// </summary>
    public interface IInjectionRestriction
    {
        //  METHODS

        /// <summary>
        /// Return <c>true</c> when <paramref name="container"/> is allowed to receive an instance
        /// of <paramref name="typeToInject"/> from <paramref name="provider"/>; <c>false</c> to
        /// block this injection (the injector then records an
        /// <see cref="Error.InjectionErrorType.InjectionRestricted"/> error).
        /// </summary>
        bool   Check(object container, Type typeToInject, IInstanceProvider provider);

        /// <summary>Human-readable description of the restriction, used in error messages.</summary>
        string GetInfo();
    }
}
