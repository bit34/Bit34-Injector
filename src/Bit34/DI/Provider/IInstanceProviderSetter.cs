using Bit34.DI.Binding;

namespace Bit34.DI.Provider
{
    public interface IInstanceProviderSetter<TBinding>
    {
        //	METHODS
        IInjectionBindingOptions ToValue(TBinding value);
        IInjectionBindingOptions ToType<TProvider>()
         where TProvider : TBinding, new();
    }
}
