using System;
using Com.Bit34Games.DI.Provider;

namespace Com.Bit34Games.DI.Binding
{
    public class NamespaceRestriction : IInjectionRestriction
    {
        //  MEMBERS
        private string[] _namespaceNameList;

        //  CONSTRUCTORS
        public NamespaceRestriction(string namespaceName)
        {
            _namespaceNameList = new string[]{namespaceName};
        }
        public NamespaceRestriction(string[] namespaceNameList)
        {
            _namespaceNameList = namespaceNameList;
        }

        //  METHODS
        public bool Check(object container, Type typeToInject, IInstanceProvider provider)
        {
            string containerNamespace = container.GetType().Namespace;
            for (int i = 0; i < _namespaceNameList.Length; i++)
            {
                string namespaceName = _namespaceNameList[i];
                int index = containerNamespace.IndexOf(namespaceName);
                if(index==0 && (containerNamespace.Length == namespaceName.Length || containerNamespace[namespaceName.Length] == '.'))
                {
                    return true;
                }
            }
            return false;
        }

        public string GetInfo()
        {
            return "Namespace restriction(" + String.Join(",", _namespaceNameList) + ")";
        }

    }
}