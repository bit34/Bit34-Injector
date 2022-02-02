using System.Reflection;

namespace Com.Bit34Games.Injector
{
	public interface IMemberInjector
    {
        //  METHODS
        bool InjectIntoField(FieldInfo fieldInfo, object container);
		bool InjectIntoProperty(PropertyInfo propertyInfo, object container);
	}
}