using System;

namespace Com.Bit34Games.Injector.Provider
{
    /// <summary>
    /// Strategy for producing the instance returned by a binding. Two built-in implementations
    /// exist: <see cref="SingleInstanceProvider"/> wraps a pre-built value, and
    /// <see cref="NewInstanceProvider{T}"/> lazily constructs one. Custom strategies can be
    /// added by implementing this interface (or extending <see cref="BaseInstanceProvider"/>).
    /// </summary>
    public interface IInstanceProvider
    {
        //  MEMBERS

        /// <summary>Concrete type of the instance this provider produces.</summary>
        Type InstanceType { get; }

        //  METHODS

        /// <summary>
        /// Produce the instance for this provider.
        /// </summary>
        /// <param name="instance">Output; the produced instance.</param>
        /// <param name="isNew">Output; <c>true</c> the first time the instance is produced (so
        /// the injector knows to recursively inject into it), <c>false</c> on subsequent calls.</param>
        void GetInstance(out object instance, out bool isNew);
    }
}
