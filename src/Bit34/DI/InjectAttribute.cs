// Copyright (c) 2018 Oğuz Sandıkçı
// This code is licensed under MIT license (see LICENSE.txt for details)

using System;


[AttributeUsage(AttributeTargets.Field|AttributeTargets.Property, AllowMultiple = false)]
public class InjectAttribute : Attribute
{
	//	CONSTRUCTOR
	public InjectAttribute()
	{ }
}
