namespace Com.Bit34Games.Injector.Binding
{
    /// <summary>
    /// Fluent surface for attaching restrictions to a binding after its provider has been chosen.
    /// Returned by <see cref="Provider.IInstanceProviderSetter{T}.ToValue"/> and
    /// <see cref="Provider.IInstanceProviderSetter{T}.ToType{TProvider}"/>.
    /// </summary>
    public interface IInjectionBindingOptions
    {
        //  METHODS

        /// <summary>
        /// Restrict injection of this binding to containers whose namespace matches
        /// <paramref name="namespaceName"/> exactly, or is nested inside it
        /// (e.g. <c>"Game.UI"</c> matches <c>"Game.UI.MainMenu"</c>).
        /// </summary>
        IInjectionBindingOptions RestrictToNamespace(string namespaceName);

        /// <summary>
        /// Restrict injection of this binding to containers whose namespace matches any of the
        /// given namespaces (exact or nested).
        /// </summary>
        IInjectionBindingOptions RestrictToNamespace(params string[] namespaceNameList);
    }
}
