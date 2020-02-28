using System;

namespace Bit34.DI.Provider
{
    public class NewInstanceProvider<T> : IInstanceProvider where T : new()
    {
        //	MEMBERS
        public Type InstanceType { get{ return typeof(T); } }
        public Action<object> PostInjectionCallback{get; private set;}
        private object _instance;

        //  CONSTRUCTOR
        public NewInstanceProvider(){}

        //  METHODS
        public void SetPostInjectionCallback(Action<object> postInjectionCallback)
        {
            PostInjectionCallback = postInjectionCallback;
        }
        
        public void GetInstance(out object value, out bool isNew)
        {
            if(_instance==null)
            {
                _instance = new T();
                isNew = true;
            }
            else
            {
                isNew = false;
            }
            value = _instance;
        }
    }
}
