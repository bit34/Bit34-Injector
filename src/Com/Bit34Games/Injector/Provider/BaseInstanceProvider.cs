using System;

namespace Com.Bit34Games.Injector.Provider
{
    /// <summary>
    /// Convenience base for <see cref="IInstanceProvider"/> implementations that produce a
    /// single backing instance. Tracks the produced instance type and exposes a protected
    /// <see cref="_instance"/> field for derived classes to populate.
    /// </summary>
    public abstract class BaseInstanceProvider : IInstanceProvider
    {
        //  MEMBERS

        /// <inheritdoc />
        public    Type   InstanceType { get; protected set; }

        /// <summary>The cached instance, or <c>null</c> until first produced.</summary>
        protected object _instance;

        //  CONSTRUCTORS

        /// <summary>Create a provider whose instances will report <paramref name="instanceType"/>.</summary>
        public BaseInstanceProvider(Type instanceType)
        {
            InstanceType = instanceType;
        }

        //  METHODS

        /// <inheritdoc />
        public abstract void GetInstance(out object value, out bool isNew);
    }
}
