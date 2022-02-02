using System;

namespace Com.Bit34Games.Injector.Provider
{
    public abstract class BaseInstanceProvider : IInstanceProvider
    {
        //  MEMBERS
        public    Type   InstanceType { get; protected set; }
        protected object _instance;

        //  CONSTRUCTORS
        public BaseInstanceProvider(Type instanceType)
        {
            InstanceType = instanceType;
        }

        //  METHODS
        public abstract void GetInstance(out object value, out bool isNew);
    }
}