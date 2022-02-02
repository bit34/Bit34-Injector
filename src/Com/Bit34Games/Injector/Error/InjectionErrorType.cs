namespace Com.Bit34Games.Injector.Error
{
    public enum InjectionErrorType
    {
        AlreadyAddedBindingForType,
        AlreadyAddedTypeWithDifferentProvider,
        TypeNotAssignableToTarget,
        ValueNotAssignableToBindingType,
        CanNotFindBindingForType,
        BindingAfterInjection,
        InjectionRestricted
    }
}
