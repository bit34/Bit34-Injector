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
        public List<FieldInfo>    Fields     { get; private set; }
        /// <summary>All <c>[Inject]</c>-tagged writable properties, including inherited ones.</summary>
        public List<PropertyInfo> Properties { get; private set; }

        //  Type this cache was built for. Private — only the constructor needs it,
        //  and consumers reach the cache via the dictionary keyed by type already.
        private readonly Type _reflectedType;

        //  DeclaredOnly so each level of the inheritance chain is visited exactly
        //  once — without it, GetFields/GetProperties returns inherited public
        //  members and the recursion would produce duplicates.
        private const BindingFlags MemberLookupFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        //  CONSTRUCTOR

        /// <summary>Walk <paramref name="reflectedType"/> and its base chain, collecting
        /// <c>[Inject]</c>-tagged fields and writable properties.</summary>
        public ReflectionCache(Type reflectedType)
        {
            _reflectedType = reflectedType;
            Fields         = new List<FieldInfo>();
            Properties     = new List<PropertyInfo>();

            AddFields(_reflectedType);
            AddProperties(_reflectedType, new HashSet<string>());
        }

        //  METHODS

        //  Fields don't need name-dedup: fields aren't virtual, and inherited
        //  private fields are distinct FieldInfo instances at different levels
        //  of the inheritance chain.
        private void AddFields(Type reflectedType)
        {
            foreach (FieldInfo fieldInfo in reflectedType.GetFields(MemberLookupFlags))
            {
                if (fieldInfo.IsDefined(typeof(InjectAttribute), true))
                {
                    Fields.Add(fieldInfo);
                }
            }

            if (reflectedType.BaseType != typeof(object))
            {
                AddFields(reflectedType.BaseType);
            }
        }

        //  Properties need name-dedup so that a virtual property re-decorated with
        //  [Inject] in a derived class isn't injected twice (SetValue uses virtual
        //  dispatch, so both PropertyInfo instances would call the same setter).
        //  We walk top-down (derived first), so the override wins over the base.
        private void AddProperties(Type reflectedType, HashSet<string> seenNames)
        {
            foreach (PropertyInfo propertyInfo in reflectedType.GetProperties(MemberLookupFlags))
            {
                //  Skip get-only properties — SetValue would throw at injection time.
                //  CanWrite covers private setters too (we look them up via NonPublic).
                if (propertyInfo.CanWrite
                    && propertyInfo.IsDefined(typeof(InjectAttribute), true)
                    && seenNames.Add(propertyInfo.Name))
                {
                    Properties.Add(propertyInfo);
                }
            }

            if (reflectedType.BaseType != typeof(object))
            {
                AddProperties(reflectedType.BaseType, seenNames);
            }
        }
    }
}
