using System;

namespace Com.Bit34Games.Injector.Error
{
    /// <summary>
    /// Exception thrown by the injector in throwing mode (constructor parameter
    /// <c>shouldThrowException = true</c>). Carries the structured
    /// <see cref="InjectionErrorType"/> in addition to the formatted message.
    /// </summary>
    public class InjectionException : Exception
    {
        //  MEMBERS

        /// <summary>Structured error code suitable for switching on in catch blocks.</summary>
        public readonly InjectionErrorType Error;

        //  CONSTRUCTOR

        /// <summary>Create a new <see cref="InjectionException"/>. Constructed by the injector — consumers should not call this directly.</summary>
        public InjectionException(InjectionErrorType error, string message) : base(message)
        {
            Error = error;
        }
    }
}
