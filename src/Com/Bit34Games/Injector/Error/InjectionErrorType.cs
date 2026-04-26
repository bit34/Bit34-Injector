namespace Com.Bit34Games.Injector.Error
{
    /// <summary>
    /// Enumeration of structured error codes produced by the injector. Carried by both
    /// <see cref="InjectionError"/> (non-throwing mode) and <see cref="InjectionException"/>
    /// (throwing mode).
    /// </summary>
    public enum InjectionErrorType
    {
        /// <summary>A binding was added more than once for the same type.</summary>
        AlreadyAddedBindingForType,
        /// <summary>A provider was already registered for the requested provider type with a different provider implementation.</summary>
        AlreadyAddedTypeWithDifferentProvider,
        /// <summary>The provider type passed to <c>ToType</c> is not assignable to the binding type.</summary>
        TypeNotAssignableToTarget,
        /// <summary>The value passed to <c>ToValue</c> is not assignable to the binding type.</summary>
        ValueNotAssignableToBindingType,
        /// <summary>No binding has been registered for the requested type.</summary>
        CanNotFindBindingForType,
        /// <summary>A binding was attempted after the bind phase ended (after the first injection).</summary>
        BindingAfterInjection,
        /// <summary>The injection was blocked by an <see cref="Binding.IInjectionRestriction"/>.</summary>
        InjectionRestricted
    }
}
