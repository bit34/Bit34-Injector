using System;

namespace Bit34.DI.Provider
{
    public interface IInstanceProvider : IInstanceProviderOptions
    {
        //  MEMBERS
        Type InstanceType { get; }
        Action<object> PostInjectionCallback{get;}
        
        //  METHODS
        void GetInstance( out object instance, out bool isNew );
    }
}
