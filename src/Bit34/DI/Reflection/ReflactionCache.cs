// Copyright (c) 2018 Oğuz Sandıkçı
// This code is licensed under MIT license (see LICENSE.txt for details)

using System;
using System.Reflection;
using System.Collections.Generic;


namespace Bit34.DI.Reflection
{
    public class ReflectionCache
    {
		//	MEMBERS
		public readonly Type ReflectedType;
		public LinkedList<FieldInfo> Fields { get; private set; }
		public LinkedList<PropertyInfo> Properties { get; private set; }


        //	CONSTRUCTOR
        public ReflectionCache(Type reflectedType)
		{
            ReflectedType = reflectedType;
			Fields = new LinkedList<FieldInfo> ();
			Properties = new LinkedList<PropertyInfo> ();
            
            AddFields(ReflectedType);
            AddProperties(ReflectedType);
        }


        //  METHODS
        private void AddFields(Type reflectedType)
        {
            MemberInfo[] fieldInfoList = reflectedType.FindMembers( MemberTypes.Field, 
                BindingFlags.Instance | 
                BindingFlags.Public | BindingFlags.NonPublic | 
                BindingFlags.SetField | BindingFlags.SetProperty , null, null);

            foreach (MemberInfo fieldInfo in fieldInfoList)
            {
                object[] attributeList = fieldInfo.GetCustomAttributes(typeof(InjectAttribute), true);
                if (attributeList.Length > 0)
                {
                    if(!Fields.Contains((FieldInfo)fieldInfo))
                    {
                        Fields.AddLast((FieldInfo)fieldInfo);
                    }
                }
            }

            if(reflectedType.BaseType!=typeof(object))
            {
                AddFields(reflectedType.BaseType);
            }
        }

        private void AddProperties(Type reflectedType)
        {
            MemberInfo[] propertyInfoList = reflectedType.FindMembers( MemberTypes.Property,
                BindingFlags.Instance | 
                BindingFlags.Public | BindingFlags.NonPublic | 
                BindingFlags.SetField | BindingFlags.SetProperty , null, null);

            foreach (MemberInfo propertyInfo in propertyInfoList)
            {
                object[] attributeList = propertyInfo.GetCustomAttributes(typeof(InjectAttribute), true);
                if (attributeList.Length > 0)
                {
                    if(!Properties.Contains((PropertyInfo)propertyInfo))
                    {
                        Properties.AddLast((PropertyInfo)propertyInfo);
                    }
                }
            }

            if(reflectedType.BaseType!=typeof(object))
            {
                AddProperties(reflectedType.BaseType);
            }
        }
    }
}
