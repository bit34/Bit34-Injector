using System;

namespace Com.Bit34Games.Injector.Error
{
    public class InjectionException : Exception
    {
        //  MEMBERS
        public readonly InjectionErrorType error;
        
        //  CONSTRUCTOR
        public InjectionException(InjectionErrorType error,string message) : base(message)
        {
            this.error = error;
        }
    }
}
