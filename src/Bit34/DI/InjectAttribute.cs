using System;

[AttributeUsage(AttributeTargets.Field|AttributeTargets.Property, AllowMultiple = false)]
public class InjectAttribute : Attribute
{
	//	CONSTRUCTOR
	public InjectAttribute(){ }
}
