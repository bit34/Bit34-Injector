namespace Com.Bit34Games.Injector.Provider
{
    /// <summary>
    /// <see cref="IInstanceProvider"/> that returns a pre-built instance supplied at
    /// construction. Used by <see cref="IInstanceProviderSetter{T}.ToValue"/>.
    /// </summary>
    public class SingleInstanceProvider : BaseInstanceProvider
    {
        //  MEMBERS
        private bool _isNew;

        //  CONSTRUCTOR

        /// <summary>Wrap <paramref name="instance"/> as a singleton provider.</summary>
        public SingleInstanceProvider( object instance) :
            base(instance.GetType())
        {
            _instance = instance;
            _isNew    = true;
        }

        //  METHODS

        /// <inheritdoc />
        public override void GetInstance(out object instance, out bool isNew)
        {
            instance = _instance;
            isNew    = _isNew;
            _isNew   = false;
        }
    }
}
