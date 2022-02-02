using Com.Bit34Games.Injector.Binding;

namespace Com.Bit34Games.Injector.Provider
{
    public interface IInstanceProviderSetter<TBinding>
    {
        //	METHODS
        IInjectionBindingOptions ToValue(TBinding value);
        IInjectionBindingOptions ToType<TProvider>() where TProvider : TBinding, new();
    }
}
