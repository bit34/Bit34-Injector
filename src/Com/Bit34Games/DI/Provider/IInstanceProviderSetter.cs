using Com.Bit34Games.DI.Binding;

namespace Com.Bit34Games.DI.Provider
{
    public interface IInstanceProviderSetter<TBinding>
    {
        //	METHODS
        IInjectionBindingOptions ToValue(TBinding value);
        IInjectionBindingOptions ToType<TProvider>()
         where TProvider : TBinding, new();
    }
}
