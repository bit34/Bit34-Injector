namespace Com.Bit34Games.DI
{
    public interface IInjectorTester : IInjector
    {
        //  MEMBERS
        int BindingCount{ get; }
        int ProviderCount{ get; }
    }
}
