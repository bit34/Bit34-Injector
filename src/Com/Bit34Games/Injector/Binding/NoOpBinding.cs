using Com.Bit34Games.Injector.Provider;

namespace Com.Bit34Games.Injector.Binding
{
    //  Returned by AddBinding<T>() when the call is invalid (duplicate binding,
    //  or binding attempted after the bind phase has ended) AND exceptions are
    //  disabled. Every chained call is a safe no-op so callers using the fluent
    //  API don't NRE in non-throwing mode. The actual error has already been
    //  recorded; the user is expected to check HasErrors after the bind phase.
    internal class NoOpBinding<TBinding> : IInstanceProviderSetter<TBinding>, IInjectionBindingOptions
    {
        //  METHODS
        public IInjectionBindingOptions ToValue(TBinding value)                                                  { return this; }
        public IInjectionBindingOptions ToType<TProvider>()              where TProvider : TBinding, new()       { return this; }
        public IInjectionBindingOptions RestrictToNamespace(string namespaceName)                                { return this; }
        public IInjectionBindingOptions RestrictToNamespace(params string[] namespaceNameList)                   { return this; }
    }
}
