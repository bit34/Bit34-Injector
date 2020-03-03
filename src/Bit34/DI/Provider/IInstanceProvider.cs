using System;

namespace Bit34.DI.Provider
{
    public interface IInstanceProvider
    {
        //  MEMBERS
        Type InstanceType { get; }
        
        //  METHODS
        void GetInstance( out object instance, out bool isNew );
    }
}
