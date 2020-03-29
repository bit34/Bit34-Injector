namespace Com.Bit34Games.DI.Error
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
