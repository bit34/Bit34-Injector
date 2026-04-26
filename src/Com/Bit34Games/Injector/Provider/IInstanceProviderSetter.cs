using Com.Bit34Games.Injector.Binding;

namespace Com.Bit34Games.Injector.Provider
{
    /// <summary>
    /// Step in the binding fluent API where the consumer chooses how the binding resolves —
    /// either a pre-built value (<see cref="ToValue"/>) or a type to instantiate
    /// (<see cref="ToType{TProvider}"/>). Returned by <see cref="IInjector.AddBinding{T}"/>.
    /// </summary>
    /// <typeparam name="TBinding">The type that consumers will request from the injector.</typeparam>
    public interface IInstanceProviderSetter<TBinding>
    {
        //  METHODS

        /// <summary>
        /// Bind to an existing instance of <typeparamref name="TBinding"/>.
        /// </summary>
        /// <param name="value">The instance returned for every consumer.</param>
        IInjectionBindingOptions ToValue(TBinding value);

        /// <summary>
        /// Bind to a type that the injector will instantiate (lazily, on first request) using
        /// its parameterless constructor.
        /// </summary>
        /// <typeparam name="TProvider">
        /// Concrete type to construct. Must be assignable to <typeparamref name="TBinding"/> and
        /// must have a parameterless constructor (or a constructor where every parameter has a
        /// default value).
        /// </typeparam>
        IInjectionBindingOptions ToType<TProvider>() where TProvider : TBinding, new();
    }
}
