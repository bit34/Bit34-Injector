// Copyright (c) 2018 Oğuz Sandıkçı
// This code is licensed under MIT license (see LICENSE.txt for details)

using System;


namespace Bit34.DI.Provider
{
    public class SingleInstanceProvider : IInstanceProvider
    {
        //  MEMBERS
        public Type InstanceType { get{ return _Instance.GetType(); } }
        public Action<object> PostInjectionCallback{get; private set;}
        private object _Instance;
        private bool _IsNew;


        //  CONSTRUCTOR
        public SingleInstanceProvider( object instance)
        {
            _Instance = instance;
            _IsNew = true;
        }


        //  METHODS
        public void SetPostInjectionCallback(Action<object> postInjectionCallback)
        {
            PostInjectionCallback = postInjectionCallback;
        }
        
        public void GetInstance(out object instance, out bool isNew)
        {
            instance = _Instance;
            isNew = _IsNew;
            _IsNew = false;
        }
    }
}