using System;

/// <summary>
/// Marks a field or property as a target of dependency injection. Members tagged with
/// <c>[Inject]</c> are set during <see cref="Com.Bit34Games.Injector.IInjector.InjectInto"/>
/// using the registered binding for the member's type.
/// </summary>
/// <remarks>
/// <para>This attribute lives in the global namespace by design — usage stays terse
/// (<c>[Inject]</c> rather than <c>[Com.Bit34Games.Injector.Inject]</c>) and no <c>using</c>
/// directive is required at the call site.</para>
/// <para>Public, internal, and private members are all supported. Properties must have a
/// (possibly private) setter; getter-only properties are silently skipped at cache build time.</para>
/// </remarks>
[AttributeUsage(AttributeTargets.Field|AttributeTargets.Property, AllowMultiple = false)]
public class InjectAttribute : Attribute
{
	//	CONSTRUCTOR

	/// <summary>Default constructor. <c>[Inject]</c> takes no arguments.</summary>
	public InjectAttribute(){ }
}
