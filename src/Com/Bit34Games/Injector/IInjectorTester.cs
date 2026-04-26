using System;
using Com.Bit34Games.Injector.Error;

namespace Com.Bit34Games.Injector
{
    /// <summary>
    /// Diagnostic surface for inspecting an injector's recorded errors and registration counts.
    /// </summary>
    /// <remarks>
    /// Most useful when the injector is constructed with <c>shouldThrowException = false</c>
    /// (development and tests), where errors accumulate instead of throwing immediately.
    /// </remarks>
    public interface IInjectorTester
    {
        //  MEMBERS

        /// <summary>True when at least one <see cref="InjectionError"/> has been recorded.</summary>
        bool HasErrors     { get; }

        /// <summary>Number of bindings registered via <see cref="IInjector.AddBinding{T}"/>.</summary>
        int  BindingCount  { get; }

        /// <summary>Number of underlying providers created by completed bindings.</summary>
        int  ProviderCount { get; }

        /// <summary>Number of recorded errors.</summary>
        int  ErrorCount    { get; }

        //  METHODS

        /// <summary>Return the error at <paramref name="index"/> in registration order.</summary>
        /// <param name="index">Zero-based index, must be less than <see cref="ErrorCount"/>.</param>
        InjectionError GetError(int index);

        /// <summary>True when a binding for <paramref name="type"/> has been registered.</summary>
        bool           HasBindingForType(Type type);
    }
}
