namespace Com.Bit34Games.Injector.Provider
{
    public class SingleInstanceProvider : BaseInstanceProvider
    {
        //  MEMBERS
        private bool _isNew;

        //  CONSTRUCTOR
        public SingleInstanceProvider( object instance) :
            base(instance.GetType())
        {
            _instance = instance;
            _isNew    = true;
        }

        //  METHODS
        public override void GetInstance(out object instance, out bool isNew)
        {
            instance = _instance;
            isNew    = _isNew;
            _isNew   = false;
        }
    }
}