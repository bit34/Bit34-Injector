using System;
using System.Collections.Generic;
using Com.Bit34Games.Injector.Error;
using Com.Bit34Games.Injector.Provider;

namespace Com.Bit34Games.Injector
{
    /// <summary>
    /// Public entry point for binding types and injecting dependencies.
    /// </summary>
    /// <remarks>
    /// <para>Lifecycle is bind-then-use: register every binding through
    /// <see cref="AddBinding{T}"/> first, then call <see cref="InjectInto"/>,
    /// <see cref="GetInstance{T}"/>, or <see cref="GetAssignableInstances{T}"/>. Once any of
    /// those consumer methods runs, the bind phase ends and further <see cref="AddBinding{T}"/>
    /// calls produce a <see cref="InjectionErrorType.BindingAfterInjection"/> error.</para>
    /// <para>All bindings are singletons — every consumer of a bound type receives the same
    /// instance.</para>
    /// </remarks>
    public interface IInjector
    {
        //  METHODS

        /// <summary>
        /// Begin a fluent binding for type <typeparamref name="T"/>. Complete it with
        /// <see cref="IInstanceProviderSetter{T}.ToValue"/> or
        /// <see cref="IInstanceProviderSetter{T}.ToType{TProvider}"/>.
        /// </summary>
        /// <typeparam name="T">The type that consumers will request (the "binding type").</typeparam>
        /// <returns>
        /// A setter to choose how this binding resolves. In non-throwing mode, the returned
        /// setter is a no-op when the call is invalid (duplicate binding, or after the bind
        /// phase has ended) so chained calls remain safe — check
        /// <see cref="IInjectorTester.HasErrors"/> after the bind phase.
        /// </returns>
        /// <exception cref="InjectionException">
        /// Thrown when <c>shouldThrowException</c> is on and either the type is already bound
        /// or the bind phase has ended.
        /// </exception>
        IInstanceProviderSetter<T> AddBinding<T>();

        /// <summary>
        /// Set every <c>[Inject]</c>-tagged field and writable property on
        /// <paramref name="container"/> using the registered bindings.
        /// </summary>
        /// <param name="container">Object whose injectable members should be set.</param>
        /// <param name="injectionOverride">
        /// Optional per-member hook that can handle a specific field/property itself; returning
        /// <c>true</c> from <see cref="IMemberInjector"/> skips the default lookup for that member.
        /// </param>
        /// <remarks>
        /// First call to <c>InjectInto</c> ends the binding phase.
        /// </remarks>
        void InjectInto(object container, IMemberInjector injectionOverride = null);

        /// <summary>
        /// Resolve a single instance for the binding type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The binding type.</typeparam>
        /// <returns>
        /// The bound instance, or <c>default(T)</c> if no binding is registered and the injector
        /// is in non-throwing mode. Calling code should check
        /// <see cref="IInjectorTester.HasErrors"/> before using the result in that mode.
        /// </returns>
        /// <exception cref="InjectionException">
        /// Thrown when <c>shouldThrowException</c> is on and no binding exists for
        /// <typeparamref name="T"/>.
        /// </exception>
        T GetInstance<T>();

        /// <summary>
        /// Enumerate every registered provider whose instance is assignable to
        /// <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">A base type or interface to filter providers by.</typeparam>
        /// <returns>
        /// Enumerable of the matching instances; safe to <c>foreach</c> over directly. The
        /// result set is cached per <typeparamref name="T"/> on first call; providers added
        /// after that call for the same <typeparamref name="T"/> will not appear (this can't
        /// happen with the default bind-then-use lifecycle).
        /// </returns>
        IEnumerable<T> GetAssignableInstances<T>();
    }
}
