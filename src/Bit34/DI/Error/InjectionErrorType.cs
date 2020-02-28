namespace Bit34.DI.Error
{
    public enum InjectionErrorType
    {
        AlreadyAddedBindingForType,
        AlreadyAddedTypeWithDifferentProvider,
        TypeNotAssignableToTarget,
        ValueNotAssignableToBindingType,
        CanNotFindBindingForType,
        BindingAfterInjection
    }
}
