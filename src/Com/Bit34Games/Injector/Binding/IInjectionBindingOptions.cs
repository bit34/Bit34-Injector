namespace Com.Bit34Games.Injector.Binding
{
    public interface IInjectionBindingOptions
    {
        //  METHODS
        IInjectionBindingOptions RestrictToNamespace(string namespaceName);
        IInjectionBindingOptions RestrictToNamespace(params string[] namespaceNameList);
    }
}