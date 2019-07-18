// Copyright (c) 2018 Oğuz Sandıkçı
// This code is licensed under MIT license (see LICENSE.txt for details)

using System;


namespace Bit34.DI.Provider
{
    public class NewInstanceProvider<T> : IInstanceProvider where T : new()
    {
        //	MEMBERS
        public Type InstanceType { get{ return typeof(T); } }
        public Action<object> PostInjectionCallback{get; private set;}
        private object _Instance;


        //  CONSTRUCTOR
        public NewInstanceProvider()
        {}


        //  METHODS
        public void SetPostInjectionCallback(Action<object> postInjectionCallback)
        {
            PostInjectionCallback = postInjectionCallback;
        }
        
        public void GetInstance(out object value, out bool isNew)
        {
            if(_Instance==null)
            {
                _Instance = new T();
                isNew = true;
            }
            else
            {
                isNew = false;
            }
            value = _Instance;
        }
    }
}
