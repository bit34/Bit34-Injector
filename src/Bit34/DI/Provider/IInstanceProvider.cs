// Copyright (c) 2018 Oğuz Sandıkçı
// This code is licensed under MIT license (see LICENSE.txt for details)

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
