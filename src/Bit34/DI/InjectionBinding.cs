// Copyright (c) 2018 Oğuz Sandıkçı
// This code is licensed under MIT license (see LICENSE.txt for details)

using System;
using Bit34.DI.Provider;


namespace Bit34.DI
{
    public class InjectionBinding : IInstanceProviderSetter
    {
        //	MEMBERS
        public readonly Type BindingType;
        public IInstanceProvider InstanceProvider{get; private set;}
        private IInstanceProviderList _InstanceProviderList;
        


        //	CONSTRUCTOR
        public InjectionBinding(Type targetType, IInstanceProviderList instanceProviderList)
        {
            BindingType = targetType;
            _InstanceProviderList = instanceProviderList;
        }


        //  METHODS
        public IInstanceProviderOptions ToValue(object value)
        {
            InstanceProvider = _InstanceProviderList.AddValueProvider(BindingType, value);
            return InstanceProvider;
        }

        public IInstanceProviderOptions ToType<T>() where T : new()
        {
            InstanceProvider = _InstanceProviderList.AddTypedProvider<T>(BindingType);
            return InstanceProvider;
        }
    }
}
