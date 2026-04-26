using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Com.Bit34Games.Injector.Binding;
using Com.Bit34Games.Injector.Error;
using Com.Bit34Games.Injector.Provider;
using Com.Bit34Games.Injector.Reflection;

namespace Com.Bit34Games.Injector
{
    /// <summary>
    /// The default <see cref="IInjector"/> implementation. Holds bindings and producers,
    /// performs the reflection-based injection, and (depending on
    /// <c>shouldThrowException</c>) either throws on errors or records them for later
    /// inspection through <see cref="IInjectorTester"/>.
    /// </summary>
    public class InjectorContext : IInjector, IInjectorTester, IInstanceProviderList
    {
        //  MEMBERS

        /// <inheritdoc />
        public int BindingCount  { get { return _bindings.Count;  } }
        /// <inheritdoc />
        public int ProviderCount { get { return _providers.Count; } }
        /// <inheritdoc />
        public bool HasErrors    { get { return _errors.Count > 0;  } }
        /// <inheritdoc />
        public IReadOnlyList<InjectionError> Errors { get { return _errors; } }
        private Dictionary<Type, IInjectionBinding> _bindings;
        private Dictionary<Type, IInstanceProvider> _providers;
        private Dictionary<Type, ReflectionCache>   _reflections;
        private Dictionary<Type, object>            _assignableInstances;
        private bool                                _shouldThrowException;
        private List<InjectionError>                _errors;
        private string[]                            _errorMessages;
        private bool                                _isBindingCompleted;

        //  CONSTRUCTORS

        /// <summary>
        /// Create a new injector context.
        /// </summary>
        /// <param name="shouldThrowException">
        /// <para>When <c>true</c> (recommended for almost all use cases, including production),
        /// the injector throws an <see cref="InjectionException"/> immediately when an error
        /// occurs.</para>
        /// <para>When <c>false</c>, errors are recorded into an internal list — intended for
        /// development and tests only. In this mode <see cref="AddBinding{T}"/> returns a no-op
        /// setter on error and <see cref="GetInstance{T}"/> returns <c>default(T)</c>; callers
        /// must check <see cref="HasErrors"/> after the bind phase to see what went wrong.
        /// See README "Creating Injector" for the full contract.</para>
        /// </param>
        public InjectorContext(bool shouldThrowException = false)
        {
            _bindings             = new Dictionary<Type, IInjectionBinding>();
            _providers            = new Dictionary<Type, IInstanceProvider>();
            _reflections          = new Dictionary<Type, ReflectionCache>();
            _assignableInstances  = new Dictionary<Type, object>();
            _shouldThrowException = shouldThrowException;

            _errors        = new List<InjectionError>();
            _errorMessages = new string[Enum.GetValues(typeof(InjectionErrorType)).Length];
            _errorMessages[(int)InjectionErrorType.AlreadyAddedBindingForType           ] = "Injection Error:Already added binding for type [{1}]\n{0}";
            _errorMessages[(int)InjectionErrorType.AlreadyAddedTypeWithDifferentProvider] = "Injection Error:Requested provider with type [{2}] already added with a different provider\n[{0}]";
            _errorMessages[(int)InjectionErrorType.TypeNotAssignableToTarget            ] = "Injection Error:Given type [{2}] is not assignable to binding type [{1}]\n{0}";
            _errorMessages[(int)InjectionErrorType.ValueNotAssignableToBindingType      ] = "Injection Error:Given value of type [{2}] is not assignable to binding type [{1}]\n{0}";
            _errorMessages[(int)InjectionErrorType.CanNotFindBindingForType             ] = "Injection Error:Can not find binding for type [{1}]\n{0}";
            _errorMessages[(int)InjectionErrorType.BindingAfterInjection                ] = "Injection Error:Can not add binding for type [{1}] after injecting\n{0}";
            _errorMessages[(int)InjectionErrorType.InjectionRestricted                  ] = "Injection Error:Binded type [{1}] has restriction that prevent this injection\n{3}\n{0}";
        }

        //  METHODS
#region IInjector implementations

        /// <inheritdoc />
        public IInstanceProviderSetter<T> AddBinding<T>()
        {
            Type bindingType = typeof(T);

            //  Bind phase has ended — record error and return a no-op setter so
            //  fluent chaining stays safe in non-throwing mode.
            if (_isBindingCompleted)
            {
                InjectionError error = CreateError(InjectionErrorType.BindingAfterInjection, bindingType, null, "", 1);
                if (_shouldThrowException)
                {
                    throw new InjectionException(error.Error, error.Message);
                }
                return new NoOpBinding<T>();
            }

            //  Duplicate binding for type — record error and return a no-op
            //  setter so chained .ToValue/.ToType calls don't reconfigure the
            //  existing binding by accident.
            if (_bindings.ContainsKey(bindingType))
            {
                InjectionError error = CreateError(InjectionErrorType.AlreadyAddedBindingForType, bindingType, null, "", 1);
                if (_shouldThrowException)
                {
                    throw new InjectionException(error.Error, error.Message);
                }
                return new NoOpBinding<T>();
            }

            //  Add binding (happy path)
            InjectionBinding<T> binding = new InjectionBinding<T>(this);
            _bindings.Add(bindingType, binding);
            return binding;
        }

        /// <inheritdoc />
        public void InjectInto(object container, IMemberInjector injectionOverride = null)
        {
            _isBindingCompleted = true;

            //  Get reflection container for object. Will be performed once per type
            ReflectionCache classReflection = GetReflection(container.GetType());

            //  Inject into fields
            foreach (FieldInfo fieldInfo in classReflection.Fields)
            {
                //  handle injection overriding, if any
                if (injectionOverride != null && injectionOverride.TryInjectIntoField(fieldInfo, container)) { continue; }

                //  Get binding for field type
                IInjectionBinding binding;
                if (!GetBinding(fieldInfo.FieldType, out binding)) { continue; }

                //  Check restrictions
                if (!CheckRestrictions(container, binding)) { continue; }

                //  Inject value
                object value = GetInstanceAndInit(binding.InstanceProvider);
                fieldInfo.SetValue(container, value);
            }

            //  Inject into properties
            foreach (PropertyInfo propertyInfo in classReflection.Properties)
            {
                //  handle injection overriding, if any
                if (injectionOverride != null && injectionOverride.TryInjectIntoProperty(propertyInfo, container)) { continue; }

                //  Get binding for field type
                IInjectionBinding binding;
                if (!GetBinding(propertyInfo.PropertyType, out binding)) { continue; }

                //  Check restrictions
                if (!CheckRestrictions(container, binding)) { continue; }

                //  Inject value
                object value = GetInstanceAndInit(binding.InstanceProvider);
                propertyInfo.SetValue(container, value, null);
            }
        }

        /// <inheritdoc />
        public T GetInstance<T>()
        {
            _isBindingCompleted = true;

            Type bindingType = typeof(T);

            object value = null;
            IInjectionBinding binding = null;
            if (_bindings.TryGetValue(bindingType, out binding))
            {
                value = GetInstanceAndInit(binding.InstanceProvider);
            }
            else
            {
                //  Handle error
                InjectionError error = CreateError(InjectionErrorType.CanNotFindBindingForType, bindingType, null, "", 1);
                if (_shouldThrowException)
                {
                    throw new InjectionException(error.Error, error.Message);
                }
            }

            return (T)value;
        }

        /// <inheritdoc />
        public IEnumerable<T> GetAssignableInstances<T>()
        {
            _isBindingCompleted = true;

            Type typeToAssign = typeof(T);

            HashSet<T> assignableInstances;
            object instances;
            if (!_assignableInstances.TryGetValue(typeToAssign, out instances))
            {
                assignableInstances = new HashSet<T>();
                _assignableInstances.Add(typeToAssign, assignableInstances);

                foreach (IInstanceProvider provider in _providers.Values)
                {
                    if (typeToAssign.IsAssignableFrom(provider.InstanceType))
                    {
                        object value = null;
                        bool isNew;
                        provider.GetInstance(out value, out isNew);
                        if (isNew)
                        {
                            InjectInto(value);
                        }
                        assignableInstances.Add((T)value);
                    }
                }
            }
            else
            {
                assignableInstances = (HashSet<T>)instances;
            }

            //  Return the HashSet<T> directly — it implements IEnumerable<T>, so callers can
            //  foreach over the result without forcing them through a manual MoveNext loop.
            return assignableInstances;
        }

#endregion

#region IInjectorTester implementations

        /// <inheritdoc />
        public bool HasBindingForType(Type type)
        {
            return _bindings.ContainsKey(type);
        }

#endregion

#region IInstanceProviderList implementations

        IInstanceProvider IInstanceProviderList.AddValueProvider(Type bindingType, object value)
        {
            Type providerType = value.GetType();

            //  Check if type of value is assignable to target type
            if (!bindingType.IsAssignableFrom(providerType))
            {
                //  Handle error
                InjectionError error = CreateError(InjectionErrorType.ValueNotAssignableToBindingType, bindingType, providerType, "", 2);
                if (_shouldThrowException)
                {
                    throw new InjectionException(error.Error, error.Message);
                }

                return null;
            }

            //  Check if a provider with given type exist
            IInstanceProvider provider;
            if (_providers.TryGetValue(providerType, out provider))
            {
                //  Check if existing provider is same with requested one
                if (provider.GetType() != typeof(SingleInstanceProvider))
                {
                    //  Handle error
                    InjectionError error = CreateError(InjectionErrorType.AlreadyAddedTypeWithDifferentProvider, bindingType, providerType, "", 2);
                    if (_shouldThrowException)
                    {
                        throw new InjectionException(error.Error, error.Message);
                    }
                }
            }
            else
            {
                provider = new SingleInstanceProvider(value);
                _providers.Add(providerType, provider);
            }

            return provider;
        }

        //  Constraint `where T : new()` is inherited from the interface declaration
        //  and must not be repeated on an explicit interface implementation.
        IInstanceProvider IInstanceProviderList.AddTypedProvider<T>(Type bindingType)
        {
            Type providerType = typeof(T);

            //  Check if type T is assignable to target type
            if (!bindingType.IsAssignableFrom(providerType))
            {
                //  Handle error
                InjectionError error = CreateError(InjectionErrorType.TypeNotAssignableToTarget, bindingType, providerType, "", 2);
                if (_shouldThrowException)
                {
                    throw new InjectionException(error.Error, error.Message);
                }

                return null;
            }

            //  Check if a provider with given type exist
            IInstanceProvider provider;
            if (_providers.TryGetValue(providerType, out provider))
            {
                //  Check if existing provider is same with requested one
                if (provider.GetType() != typeof(NewInstanceProvider<T>))
                {
                    //  Handle error
                    InjectionError error = CreateError(InjectionErrorType.AlreadyAddedTypeWithDifferentProvider, bindingType, providerType, "", 2);
                    if (_shouldThrowException)
                    {
                        throw new InjectionException(error.Error, error.Message);
                    }
                }
            }
            else
            {
                provider = new NewInstanceProvider<T>();
                _providers.Add(providerType, provider);
            }

            return provider;
        }

#endregion

        private object GetInstanceAndInit(IInstanceProvider instanceProvider)
        {
            object value = null;
            bool isNew;
            instanceProvider.GetInstance(out value, out isNew);
            if (isNew)
            {
                InjectInto(value);
            }
            return value;
        }

        private bool GetBinding(Type bindingType, out IInjectionBinding binding)
        {
            if (!_bindings.TryGetValue(bindingType, out binding))
            {
                //  Handle error
                InjectionError error = CreateError(InjectionErrorType.CanNotFindBindingForType, bindingType, null, "", 2);
                if (_shouldThrowException)
                {
                    throw new InjectionException(error.Error, error.Message);
                }
                return false;
            }
            return true;
        }

        private bool CheckRestrictions(object container, IInjectionBinding binding)
        {
            IReadOnlyList<IInjectionRestriction> restrictions = binding.RestrictionList;
            for (int i = 0; i < restrictions.Count; i++)
            {
                IInjectionRestriction restriction = restrictions[i];
                bool restrictionResult = restrictions[i].Check(container, binding.BindingType, binding.InstanceProvider);
                if (!restrictionResult)
                {
                    //  Handle error
                    InjectionError error = CreateError(InjectionErrorType.InjectionRestricted, binding.BindingType, null, restriction.GetInfo(), 2);
                    if (_shouldThrowException)
                    {
                        throw new InjectionException(error.Error, error.Message);
                    }
                    return false;
                }
            }
            return true;
        }

        private ReflectionCache GetReflection(Type type)
        {
            ReflectionCache reflection = null;

            if (!_reflections.TryGetValue(type, out reflection))
            {
                reflection = new ReflectionCache(type);

                _reflections[type] = reflection;
            }

            return reflection;
        }

        private InjectionError CreateError(InjectionErrorType errorType, Type bindingType = null, Type providerType = null, string extraInfo="", int callerLevel=0)
        {
            string callerInfo = GetCallerInfo(1+callerLevel);
            string bindingTypeAsString = (bindingType != null)?(bindingType.ToString()):("");
            string providerTypeAsString = (providerType != null)?(providerType.ToString()):("");
            object[] args = new object[]
            {
                callerInfo,
                bindingTypeAsString,
                providerTypeAsString,
                extraInfo
            };
            string errorMessage = String.Format(_errorMessages[(int)errorType], args);

            InjectionError error = new InjectionError(errorType, errorMessage);
            _errors.Add(error);

            return error;
        }

        private string GetCallerInfo(int callerLevel=0)
        {
            StackTrace st = new StackTrace(true);
            StackFrame sf = st.GetFrame(1+callerLevel);
            string info = String.Format("\tFilename:{0}\n\tMethod:{1}\n\tLine:{2}", sf.GetFileName(), sf.GetMethod(), sf.GetFileLineNumber() );

            return info;
        }

    }
}
