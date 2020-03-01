using System;
using Bit34.DI.Provider;

namespace Bit34.DI
{
    public class InjectionBinding<TBinding> : IInjectionBinding, IInstanceProviderSetter<TBinding>
    {
        //	MEMBERS
        public readonly Type bindingType;
        public IInstanceProvider InstanceProvider{get; private set;}
        private IInstanceProviderList _instanceProviderList;
    

        //	CONSTRUCTOR
        public InjectionBinding(IInstanceProviderList instanceProviderList)
        {
            bindingType = typeof(TBinding);
            _instanceProviderList = instanceProviderList;
        }

        //  METHODS
        public IInstanceProviderOptions ToValue(TBinding value)
        {
            InstanceProvider = _instanceProviderList.AddValueProvider(bindingType, value);
            return InstanceProvider;
        }

        public IInstanceProviderOptions ToType<TProvider>()
         where TProvider : TBinding, new()
        {
            InstanceProvider = _instanceProviderList.AddTypedProvider<TProvider>(bindingType);
            return InstanceProvider;
        }
    }
}
