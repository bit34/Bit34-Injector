namespace Com.Bit34Games.Injector.Error
{
    /// <summary>
    /// One recorded error from the injector — the structured <see cref="InjectionErrorType"/>
    /// code plus a human-readable message that includes binding/provider type names and caller
    /// source location.
    /// </summary>
    public class InjectionError
    {
        //  MEMBERS

        /// <summary>Structured error code suitable for switching on in tests.</summary>
        public readonly InjectionErrorType Error;

        /// <summary>Pre-formatted error message including binding/provider type and source location.</summary>
        public readonly string             Message;

        //  CONSTRUCTOR

        /// <summary>Create a new <see cref="InjectionError"/>. Constructed by the injector — consumers should not call this directly.</summary>
        public InjectionError(InjectionErrorType error,
                              string             message)
        {
            Error   = error;
            Message = message;
        }
    }
}
