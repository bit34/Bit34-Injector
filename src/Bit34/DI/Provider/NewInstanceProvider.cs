namespace Bit34.DI.Provider
{
    public class NewInstanceProvider<T> : BaseInstanceProvider where T : new()
    {
        //  CONSTRUCTORS
        public NewInstanceProvider():
            base(typeof(T))
        { }

        //  METHODS
        public override void GetInstance(out object value, out bool isNew)
        {
            if(_instance==null)
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
