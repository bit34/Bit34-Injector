// Copyright (c) 2018 Oğuz Sandıkçı
// This code is licensed under MIT license (see LICENSE.txt for details)

using System;
using System.Reflection;


namespace Bit34.DI
{
	public interface IMemberInjector
    {
        //  METHODS
        bool InjectIntoField(FieldInfo fieldInfo, object container);
		bool InjectIntoProperty(PropertyInfo propertyInfo, object container);
	}
}