namespace Com.Bit34Games.DI.Binding
{
    public interface IInjectionBindingOptions
    {
        IInjectionBindingOptions RestrictToNamespace(string namespaceName);
        IInjectionBindingOptions RestrictToNamespace(params string[] namespaceNameList);
    }
}