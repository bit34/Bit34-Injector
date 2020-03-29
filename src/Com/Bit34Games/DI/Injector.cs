using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Com.Bit34Games.DI.Binding;
using Com.Bit34Games.DI.Error;
using Com.Bit34Games.DI.Provider;
using Com.Bit34Games.DI.Reflection;

namespace Com.Bit34Games.DI
{
    public class Injector : IInjectorTester, IInstanceProviderList
    {
        //  MEMBERS
        public int BindingCount  { get{ return _bindings.Count;  } }
        public int ProviderCount { get{ return _providers.Count; } }
        public int ErrorCount    { get{ return _errors.Count;    } }
        private Dictionary<Type, IInjectionBinding> _bindings;
        private Dictionary<Type, IInstanceProvider> _providers;
        private Dictionary<Type, ReflectionCache>   _reflections;
        private Dictionary<Type, object>            _assignableInstances;
        private bool _shouldThrowException;
        private List<InjectionError> _errors;
        private string[] _errorMessages;
        private bool _isBindingCompleted;

        //	CONSTRUCTORS
        public Injector(bool shouldThrowException=false)
        {
            _bindings = new Dictionary<Type, IInjectionBinding>();
            _providers = new Dictionary<Type, IInstanceProvider>();
            _reflections = new Dictionary<Type, ReflectionCache>();
            _assignableInstances = new Dictionary<Type, object>();
            _shouldThrowException = shouldThrowException;
            _errors = new List<InjectionError>();
            _errorMessages = new string[Enum.GetValues(typeof(InjectionErrorType)).Length];
            _errorMessages[(int)InjectionErrorType.AlreadyAddedBindingForType           ] = "Injection Error:Already added binding for type [{1}]\n{0}";
            _errorMessages[(int)InjectionErrorType.AlreadyAddedTypeWithDifferentProvider] = "Injection Error:Requested provider with type [{2}] already added with a different provider\n[{0}]]";
            _errorMessages[(int)InjectionErrorType.TypeNotAssignableToTarget            ] = "Injection Error:Given type [{2}] is not assignable to binding type [{1}]\n{0}";
            _errorMessages[(int)InjectionErrorType.ValueNotAssignableToBindingType      ] = "Injection Error:Given value of type [{2}] is not assignable to binding type [{1}]\n{0}";
            _errorMessages[(int)InjectionErrorType.CanNotFindBindingForType             ] = "Injection Error:Can not find binding for type [{1}]\n{0}";
            _errorMessages[(int)InjectionErrorType.BindingAfterInjection                ] = "Injection Error:Can not add binding for type [{1}] after injecting\n{0}";
            _errorMessages[(int)InjectionErrorType.InjectionRestricted                  ] = "Injection Error:Binded type [{1}] has restriction that prevent this injection\n{3}\n{0}";
        }

        //  METHODS
#region IInjector implementations

        public IInstanceProviderSetter<T> AddBinding<T>()
        {
            Type bindingType = typeof(T);
            IInjectionBinding binding = null;

            if(!_isBindingCompleted)
            {
                //  Check is there is an existing binding with given type
                if(_bindings.TryGetValue(bindingType,out binding))
                {
                    //  Handler error
                    InjectionError error = CreateError(InjectionErrorType.AlreadyAddedBindingForType, bindingType, null, "", 1);
                    if(_shouldThrowException)
                    {
                        throw new InjectionException(error.error,error.message);
                    }
                }
                else
                {
                    //  Add binding
                    binding = new InjectionBinding<T>(this);
                    _bindings.Add(bindingType, binding);
                }
            }
            else
            {
                //  Handler error
                InjectionError error = CreateError(InjectionErrorType.BindingAfterInjection, bindingType, null, "", 1);
                if(_shouldThrowException)
                {
                    throw new InjectionException(error.error,error.message);
                }
            }

            return (InjectionBinding<T>)binding;
        }

        public bool HasBindingForType(Type type)
        {
            return _bindings.ContainsKey(type);
        }

        public InjectionError GetError(int index)
        {
            return _errors[index];
        }

        public void InjectInto(object container, IMemberInjector injectionOverride = null)
        {
            _isBindingCompleted = true;
            
            //  Get reflection container for object. Will be performed once per type
            ReflectionCache classReflection = GetReflection(container.GetType());

            //  Inject into fields
            foreach (FieldInfo fieldInfo in classReflection.Fields)
            {
                //  handle injection overriding, if any
                if(injectionOverride!=null && injectionOverride.InjectIntoField(fieldInfo, container)) { continue; }

                //  Get binding for field type
                IInjectionBinding binding;
                if(GetBinding(fieldInfo.FieldType, out binding)==false) { continue; }

                //  Check restrictions
                if(CheckRestrictions(container, binding)==false) { continue; }

                //  Inject value
                object value = GetInstanceAndInit(binding.InstanceProvider);
                fieldInfo.SetValue(container, value);
            }

            //  Inject into properties
            foreach (PropertyInfo propertyInfo in classReflection.Properties)
            {
                //  handle injection overriding, if any
                if(injectionOverride!=null && injectionOverride.InjectIntoProperty(propertyInfo, container)) { continue; }

                //  Get binding for field type
                IInjectionBinding binding;
                if(GetBinding(propertyInfo.PropertyType, out binding)==false) { continue; }

                //  Check restrictions
                if(CheckRestrictions(container, binding)==false) { continue; }

                //  Inject value
                object value = GetInstanceAndInit(binding.InstanceProvider);
                propertyInfo.SetValue(container, value, null);
            }
        }

        public T GetInstance<T>()
        {
            _isBindingCompleted = true;

            Type bindingType = typeof(T);

            object value = null;
            IInjectionBinding binding = null;
            if (_bindings.TryGetValue(bindingType, out binding) == true)
            {
                value = GetInstanceAndInit(binding.InstanceProvider);
            }
            else
            {
                //  Handler error
                InjectionError error = CreateError(InjectionErrorType.CanNotFindBindingForType, bindingType, null, "", 1);
                if(_shouldThrowException)
                {
                    throw new InjectionException(error.error,error.message);
                }
            }

            return (T)value;
        }

        public IEnumerator<T> GetAssignableInstances<T>()
        {
            _isBindingCompleted = true;

            Type typeToAssign = typeof(T);

            HashSet<T> assignableInstances;
            object instances;
            if(!_assignableInstances.TryGetValue(typeToAssign, out instances))
            {
                assignableInstances = new HashSet<T>();
                _assignableInstances.Add(typeToAssign, assignableInstances);

                foreach(IInstanceProvider provider in _providers.Values)
                {
                    if(typeToAssign.IsAssignableFrom(provider.InstanceType))
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
            
            return assignableInstances.GetEnumerator();
        }

#endregion

#region IInstanceProviderList implementations

        public IInstanceProvider AddValueProvider(Type bindingType, object value)
        {
            Type providerType = value.GetType();

            //  Check if type of value is assignable to target type
            if (!bindingType.IsAssignableFrom(providerType))
            {
                //  Handler error
                InjectionError error = CreateError(InjectionErrorType.ValueNotAssignableToBindingType, bindingType, providerType, "", 2);
                if(_shouldThrowException)
                {
                    throw new InjectionException(error.error,error.message);
                }

                return null;
            }

            //  Check if a provider with given type exist
            IInstanceProvider provider;
            if(_providers.TryGetValue(providerType, out provider))
            {
                //  Check if existing provider is same with requested one
                if(provider.GetType()!=typeof(SingleInstanceProvider))
                {
                    //  Handler error
                    InjectionError error = CreateError(InjectionErrorType.AlreadyAddedTypeWithDifferentProvider, bindingType, providerType, "", 2);
                    if(_shouldThrowException)
                    {
                        throw new InjectionException(error.error,error.message);
                    }
                }
            }
            else
            {
                provider = new SingleInstanceProvider(value);
                _providers.Add(providerType,provider);
            }

            return provider;
        }

        public IInstanceProvider AddTypedProvider<T>(Type bindingType) where T : new()
        {
            Type providerType = typeof(T);

            //  Check if type T is assignable to target type
            if (!bindingType.IsAssignableFrom(providerType))
            {
                //  Handler error
                InjectionError error = CreateError(InjectionErrorType.TypeNotAssignableToTarget, bindingType, providerType, "", 2);
                if(_shouldThrowException)
                {
                    throw new InjectionException(error.error,error.message);
                }

                return null;
            }
            
            //  Check if a provider with given type exist
            IInstanceProvider provider;
            if(_providers.TryGetValue(providerType, out provider))
            {
                //  Check if existing provider is same with requested one
                if(provider.GetType()!=typeof(NewInstanceProvider<T>))
                {
                    //  Handler error
                    InjectionError error = CreateError(InjectionErrorType.AlreadyAddedTypeWithDifferentProvider, bindingType, providerType, "", 2);
                    if(_shouldThrowException)
                    {
                        throw new InjectionException(error.error,error.message);
                    }
                }
            }
            else
            {
                provider = new NewInstanceProvider<T>();
                _providers.Add(providerType,provider);
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
            if (_bindings.TryGetValue(bindingType, out binding) == false)
            {
                //  Handler error
                InjectionError error = CreateError(InjectionErrorType.CanNotFindBindingForType, bindingType, null, "", 2);
                if(_shouldThrowException)
                {
                    throw new InjectionException(error.error, error.message);
                }
                return false;
            }
            return true;
        }

        private bool CheckRestrictions(object container, IInjectionBinding binding)
        {
            List<IInjectionRestriction> restrictions = binding.RestrictionList;
            for (int i = 0; i < restrictions.Count; i++)
            {
                IInjectionRestriction restriction = restrictions[i];
                bool restrictionResult = restrictions[i].Check(container, binding.BindingType, binding.InstanceProvider);
                if (restrictionResult==false)
                {
                    //  Handler error
                    InjectionError error = CreateError(InjectionErrorType.InjectionRestricted, binding.BindingType, null, restriction.GetInfo(), 2);
                    if(_shouldThrowException)
                    {
                        throw new InjectionException(error.error, error.message);
                    }
                    return false;
                }
            }
            return true;
        }

        private ReflectionCache GetReflection(Type type)
        {
            ReflectionCache reflection = null;

            if (_reflections.TryGetValue(type, out reflection) == false)
            {
                reflection = new ReflectionCache(type);

                _reflections[type] = reflection;
            }

            return reflection;
        }

        private InjectionError CreateError(InjectionErrorType errorType, Type bindingType=null, Type providerType=null, string extraInfo="", int callerLevel=0)
        {
            string callerInfo = GetCallerInfo(1+callerLevel);
            string bindingTypeAsString = (bindingType!=null)?(bindingType.ToString()):("");
            string providerTypeAsString = (providerType!=null)?(providerType.ToString()):("");
            object[] args = new object[]
            {
                callerInfo, 
                bindingTypeAsString, 
                providerTypeAsString,
                extraInfo
            };
            string errorMessage = String.Format(_errorMessages[(int)errorType], args);

            InjectionError error = new InjectionError(errorType,errorMessage);
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
