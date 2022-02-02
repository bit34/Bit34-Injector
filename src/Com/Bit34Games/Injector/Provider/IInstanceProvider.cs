using System;

namespace Com.Bit34Games.Injector.Provider
{
    public interface IInstanceProvider
    {
        //  MEMBERS
        Type InstanceType { get; }
        
        //  METHODS
        void GetInstance( out object instance, out bool isNew );
    }
}
