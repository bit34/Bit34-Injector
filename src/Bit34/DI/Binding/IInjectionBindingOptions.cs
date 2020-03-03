namespace Bit34.DI.Binding
{
    public interface IInjectionBindingOptions
    {
        IInjectionBindingOptions RestrictToNamespace(string namespaceName);
        IInjectionBindingOptions RestrictToNamespace(params string[] namespaceNameList);
    }
}