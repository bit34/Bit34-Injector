namespace Bit34.DI
{
    public interface IInjectorTester : IInjector
    {
        //  MEMBERS
        int BindingCount{ get; }
        int ProviderCount{ get; }
    }
}
