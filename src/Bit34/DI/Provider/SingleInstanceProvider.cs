using System;

namespace Bit34.DI.Provider
{
    public class SingleInstanceProvider : IInstanceProvider
    {
        //  MEMBERS
        public Type InstanceType { get{ return _instance.GetType(); } }
        public Action<object> PostInjectionCallback{get; private set;}
        private object _instance;
        private bool _isNew;


        //  CONSTRUCTOR
        public SingleInstanceProvider( object instance)
        {
            _instance = instance;
            _isNew = true;
        }


        //  METHODS
        public void SetPostInjectionCallback(Action<object> postInjectionCallback)
        {
            PostInjectionCallback = postInjectionCallback;
        }
        
        public void GetInstance(out object instance, out bool isNew)
        {
            instance = _instance;
            isNew = _isNew;
            _isNew = false;
        }
    }
}