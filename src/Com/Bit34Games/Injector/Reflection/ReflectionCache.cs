using System;
using System.Reflection;
using System.Collections.Generic;

namespace Com.Bit34Games.Injector.Reflection
{
    /// <summary>
    /// Per-type cache of <c>[Inject]</c>-tagged fields and writable properties (including
    /// inherited members). Built once per target type during <see cref="IInjector.InjectInto"/>
    /// and reused on subsequent calls for the same type.
    /// </summary>
    public class ReflectionCache
    {
        //  MEMBERS
        /// <summary>All <c>[Inject]</c>-tagged fields, including inherited ones.</summary>
        public LinkedList<FieldInfo>    Fields     { get; private set; }
        /// <summary>All <c>[Inject]</c>-tagged writable properties, including inherited ones.</summary>
        public LinkedList<PropertyInfo> Properties { get; private set; }
        //  Type this cache was built for. Private — only the constructor needs it,
        //  and consumers reach the cache via the dictionary keyed by type already.
        private readonly Type           _reflectedType;

        //  CONSTRUCTOR
        /// <summary>Walk <paramref name="reflectedType"/> and its base chain, collecting
        /// <c>[Inject]</c>-tagged fields and writable properties.</summary>
        public ReflectionCache(Type reflectedType)
        {
            _reflectedType = reflectedType;
            Fields         = new LinkedList<FieldInfo> ();
            Properties     = new LinkedList<PropertyInfo> ();

            AddFields(_reflectedType);
            AddProperties(_reflectedType);
        }

        //  METHODS
        private void AddFields(Type reflectedType)
        {
            MemberInfo[] fieldInfoList = reflectedType.FindMembers(MemberTypes.Field,
                BindingFlags.Instance |
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.SetField | BindingFlags.SetProperty, null, null);

            foreach (MemberInfo fieldInfo in fieldInfoList)
            {
                object[] attributeList = fieldInfo.GetCustomAttributes(typeof(InjectAttribute), true);
                if (attributeList.Length > 0)
                {
                    if (!Fields.Contains((FieldInfo)fieldInfo))
                    {
                        Fields.AddLast((FieldInfo)fieldInfo);
                    }
                }
            }

            if (reflectedType.BaseType != typeof(object))
            {
                AddFields(reflectedType.BaseType);
            }
        }

        private void AddProperties(Type reflectedType)
        {
            MemberInfo[] propertyInfoList = reflectedType.FindMembers(MemberTypes.Property,
                BindingFlags.Instance |
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.SetField | BindingFlags.SetProperty, null, null);

            foreach (MemberInfo propertyInfo in propertyInfoList)
            {
                object[] attributeList = propertyInfo.GetCustomAttributes(typeof(InjectAttribute), true);
                if (attributeList.Length > 0)
                {
                    PropertyInfo property = (PropertyInfo)propertyInfo;
                    //  Skip get-only properties — SetValue would throw at injection time.
                    //  CanWrite covers private setters too (we look them up via NonPublic).
                    if (property.CanWrite && !Properties.Contains(property))
                    {
                        Properties.AddLast(property);
                    }
                }
            }

            if (reflectedType.BaseType != typeof(object))
            {
                AddProperties(reflectedType.BaseType);
            }
        }
    }
}
