using System;
using Bit34.DI.Provider;

namespace Bit34.DI
{
    public class InjectionBinding : IInstanceProviderSetter
    {
        //	MEMBERS
        public readonly Type bindingType;
        public IInstanceProvider InstanceProvider{get; private set;}
        private IInstanceProviderList _instanceProviderList;
    

        //	CONSTRUCTOR
        public InjectionBinding(Type targetType, IInstanceProviderList instanceProviderList)
        {
            bindingType = targetType;
            _instanceProviderList = instanceProviderList;
        }

        //  METHODS
        public IInstanceProviderOptions ToValue(object value)
        {
            InstanceProvider = _instanceProviderList.AddValueProvider(bindingType, value);
            return InstanceProvider;
        }

        public IInstanceProviderOptions ToType<T>() where T : new()
        {
            InstanceProvider = _instanceProviderList.AddTypedProvider<T>(bindingType);
            return InstanceProvider;
        }
    }
}
