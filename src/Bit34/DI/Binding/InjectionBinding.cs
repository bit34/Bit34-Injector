using System;
using System.Collections.Generic;
using Bit34.DI.Provider;

namespace Bit34.DI.Binding
{
    public class InjectionBinding<TBinding> : IInjectionBinding, IInstanceProviderSetter<TBinding>
    {
        //	MEMBERS
        public Type BindingType { get; private set; }
        public IInstanceProvider InstanceProvider{get; private set;}
        public List<IInjectionRestriction> RestrictionList { get; private set; }
        private IInstanceProviderList _instanceProviderList;
    

        //	CONSTRUCTOR
        public InjectionBinding(IInstanceProviderList instanceProviderList)
        {
            BindingType = typeof(TBinding);
            _instanceProviderList = instanceProviderList;
            RestrictionList = new List<IInjectionRestriction>();
        }

        //  METHODS
        public IInjectionBindingOptions ToValue(TBinding value)
        {
            InstanceProvider = _instanceProviderList.AddValueProvider(BindingType, value);
            return this;
        }

        public IInjectionBindingOptions ToType<TProvider>()
         where TProvider : TBinding, new()
        {
            InstanceProvider = _instanceProviderList.AddTypedProvider<TProvider>(BindingType);
            return this;
        }

        public IInjectionBindingOptions RestrictToNamespace(string namespaceName)
        {
            RestrictionList.Add(new NamespaceRestriction(namespaceName));
            return this;
        }

        public IInjectionBindingOptions RestrictToNamespace(params string[] namespaceNameList)
        {
            RestrictionList.Add(new NamespaceRestriction(namespaceNameList));
            return this;
        }
    }
}
