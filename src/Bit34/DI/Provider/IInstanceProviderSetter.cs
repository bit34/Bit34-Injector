namespace Bit34.DI.Provider
{
    public interface IInstanceProviderSetter<TBinding>
    {
        //	METHODS
        IInstanceProviderOptions ToValue(TBinding value);
        IInstanceProviderOptions ToType<TProvider>()
         where TProvider : TBinding, new();
    }
}
