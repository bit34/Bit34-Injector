using System.Reflection;

namespace Com.Bit34Games.Injector
{
    /// <summary>
    /// Optional per-member injection hook passed to <see cref="IInjector.InjectInto"/>.
    /// Implementations decide on a member-by-member basis whether to handle the value
    /// themselves and skip the default binding lookup.
    /// </summary>
    public interface IMemberInjector
    {
        //  METHODS

        /// <summary>
        /// Optionally set <paramref name="fieldInfo"/> on <paramref name="container"/>.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this implementation handled the field — the default binding lookup is
        /// skipped for this member. <c>false</c> to let the default injection run.
        /// </returns>
        bool InjectIntoField(FieldInfo fieldInfo, object container);

        /// <summary>
        /// Optionally set <paramref name="propertyInfo"/> on <paramref name="container"/>.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this implementation handled the property — the default binding lookup
        /// is skipped for this member. <c>false</c> to let the default injection run.
        /// </returns>
        bool InjectIntoProperty(PropertyInfo propertyInfo, object container);
    }
}
