namespace Bit34.DI.Provider
{
    public interface IInstanceProviderSetter
    {
        //	METHODS
        IInstanceProviderOptions ToValue(object value);
        IInstanceProviderOptions ToType<T>() where T : new();
    }
}
