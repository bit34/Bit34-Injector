namespace Com.Bit34Games.Injector.Provider
{
    /// <summary>
    /// <see cref="IInstanceProvider"/> that lazily constructs an instance of
    /// <typeparamref name="T"/> using its parameterless constructor on first request and caches
    /// it for every subsequent request. Used by <see cref="IInstanceProviderSetter{T}.ToType{TProvider}"/>.
    /// </summary>
    /// <typeparam name="T">Concrete type to instantiate. Must have a parameterless constructor.</typeparam>
    public class NewInstanceProvider<T> : BaseInstanceProvider where T : new()
    {
        //  CONSTRUCTORS

        /// <summary>Create a provider that will construct <typeparamref name="T"/> on first <see cref="GetInstance"/> call.</summary>
        public NewInstanceProvider():
            base(typeof(T))
        { }

        //  METHODS

        /// <inheritdoc />
        public override void GetInstance(out object value, out bool isNew)
        {
            if (_instance == null)
            {
                _instance = new T();
                isNew = true;
            }
            else
            {
                isNew = false;
            }
            value = _instance;
        }
    }
}
