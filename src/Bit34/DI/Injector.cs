using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Bit34.DI.Error;
using Bit34.DI.Provider;
using Bit34.DI.Reflection;

namespace Bit34.DI
{
    public class Injector : IInjectorTester, IInstanceProviderList, IMemberInjector
    {
        //  MEMBERS
        public int BindingCount  { get{ return _bindings.Count;  } }
        public int ProviderCount { get{ return _providers.Count; } }
        public int ErrorCount    { get{ return _errors.Count;    } }
        private Dictionary<Type, IInjectionBinding>  _bindings;
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
                    InjectionError error = CreateError(InjectionErrorType.AlreadyAddedBindingForType, bindingType, null, 1);
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
                InjectionError error = CreateError(InjectionErrorType.BindingAfterInjection, bindingType, null, 1);
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
                if(injectionOverride!=null && injectionOverride.InjectIntoField(fieldInfo, container))
                {
                    continue;
                }
                else if (InjectIntoField(fieldInfo, container))
                {
                    continue;
                }
                else
                {
                    //  Handler error
                    InjectionError error = CreateError(InjectionErrorType.CanNotFindBindingForType, fieldInfo.FieldType, null, 1);
                    if(_shouldThrowException)
                    {
                        throw new InjectionException(error.error,error.message);
                    }

                    continue;
                }
            }

            //  Inject into properties
            foreach (PropertyInfo propertyInfo in classReflection.Properties)
            {
                if(injectionOverride!=null && injectionOverride.InjectIntoProperty(propertyInfo, container))
                {
                    continue;
                }
                else if (InjectIntoProperty(propertyInfo, container))
                {
                    continue;
                }
                else
                {
                    //  Handler error
                    InjectionError error = CreateError(InjectionErrorType.CanNotFindBindingForType, propertyInfo.PropertyType, null, 1);
                    if(_shouldThrowException)
                    {
                        throw new InjectionException(error.error,error.message);
                    }

                    continue;
                }
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
                InjectionError error = CreateError(InjectionErrorType.CanNotFindBindingForType, bindingType, null, 1);
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
                        if (provider.PostInjectionCallback!=null)
                        {
                            provider.PostInjectionCallback(value);
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
                InjectionError error = CreateError(InjectionErrorType.ValueNotAssignableToBindingType, bindingType, providerType, 2);
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
                    InjectionError error = CreateError(InjectionErrorType.AlreadyAddedTypeWithDifferentProvider, bindingType, providerType, 2);
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
                InjectionError error = CreateError(InjectionErrorType.TypeNotAssignableToTarget, bindingType, providerType, 2);
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
                    InjectionError error = CreateError(InjectionErrorType.AlreadyAddedTypeWithDifferentProvider, bindingType, providerType, 2);
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
        
#region IMemberInjector implementations
        
        public bool InjectIntoField(FieldInfo fieldInfo, object container)
        {
            IInjectionBinding binding = null;
            if (_bindings.TryGetValue(fieldInfo.FieldType, out binding) == true)
            {
                object value = GetInstanceAndInit(binding.InstanceProvider);
                fieldInfo.SetValue(container, value);
                return true;
            }
            return false;
        }
		
        public bool InjectIntoProperty(PropertyInfo propertyInfo, object container)
        {
            IInjectionBinding binding = null;
            if (_bindings.TryGetValue(propertyInfo.PropertyType, out binding) == true)
            {
                object value = GetInstanceAndInit(binding.InstanceProvider);
                propertyInfo.SetValue(container, value, null);
                return true;
            }
            return false;
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
                if (instanceProvider.PostInjectionCallback != null)
                {
                    instanceProvider.PostInjectionCallback(value);
                }
            }
            return value;
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

        private InjectionError CreateError(InjectionErrorType errorType, Type bindingType=null, Type providerType=null, int callerLevel=0)
        {
            string callerInfo = GetCallerInfo(1+callerLevel);
            string bindingTypeAsString = (bindingType!=null)?(bindingType.ToString()):("");
            string providerTypeAsString = (providerType!=null)?(providerType.ToString()):("");
            object[] args = new object[]{ callerInfo, bindingTypeAsString, providerTypeAsString};
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
