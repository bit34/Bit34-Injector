using System;
using System.Collections.Generic;
using Com.Bit34Games.Injector.Provider;

namespace Com.Bit34Games.Injector.Binding
{
    //  Implementation detail — consumers receive these through IInstanceProviderSetter<T>
    //  (return type of IInjector.AddBinding<T>) and IInjectionBinding (used internally
    //  by the injection pipeline). Not meant to be constructed directly.
    internal class InjectionBinding<TBinding> : IInjectionBinding, IInstanceProviderSetter<TBinding>
    {
        //  MEMBERS
        public Type              BindingType      { get; private set; }
        public IInstanceProvider InstanceProvider { get; private set; }
        //  Public surface is the read-only view; the concrete List<> is kept
        //  private so callers can't mutate the restriction set from the outside.
        public IReadOnlyList<IInjectionRestriction> RestrictionList { get { return _restrictionList; } }
        private List<IInjectionRestriction>         _restrictionList;
        private IInstanceProviderList               _instanceProviderList;

        //  CONSTRUCTOR
        public InjectionBinding(IInstanceProviderList instanceProviderList)
        {
            BindingType           = typeof(TBinding);
            _instanceProviderList = instanceProviderList;
            _restrictionList      = new List<IInjectionRestriction>();
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
            _restrictionList.Add(new NamespaceRestriction(namespaceName));
            return this;
        }

        public IInjectionBindingOptions RestrictToNamespace(params string[] namespaceNameList)
        {
            _restrictionList.Add(new NamespaceRestriction(namespaceNameList));
            return this;
        }
    }
}
