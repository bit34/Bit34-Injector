using System.Reflection;

namespace Bit34.DI
{
	public interface IMemberInjector
    {
        //  METHODS
        bool InjectIntoField(FieldInfo fieldInfo, object container);
		bool InjectIntoProperty(PropertyInfo propertyInfo, object container);
	}
}