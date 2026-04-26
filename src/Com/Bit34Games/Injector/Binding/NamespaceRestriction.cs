using System;
using Com.Bit34Games.Injector.Provider;

namespace Com.Bit34Games.Injector.Binding
{
    /// <summary>
    /// Built-in <see cref="IInjectionRestriction"/> that allows a binding to be injected only
    /// into containers whose namespace matches one of the configured namespaces exactly, or is
    /// nested within one of them (e.g. <c>"Game.UI"</c> matches <c>"Game.UI.MainMenu"</c>).
    /// </summary>
    public class NamespaceRestriction : IInjectionRestriction
    {
        //  MEMBERS
        private string[] _namespaceNameList;

        //  CONSTRUCTORS

        /// <summary>Restrict to a single namespace.</summary>
        public NamespaceRestriction(string namespaceName)
        {
            _namespaceNameList = new string[]{namespaceName};
        }

        /// <summary>Restrict to any namespace in <paramref name="namespaceNameList"/>.</summary>
        public NamespaceRestriction(string[] namespaceNameList)
        {
            _namespaceNameList = namespaceNameList;
        }

        //  METHODS

        /// <inheritdoc />
        public bool Check(object container, Type typeToInject, IInstanceProvider provider)
        {
            string containerNamespace = container.GetType().Namespace;
            for (int i = 0; i < _namespaceNameList.Length; i++)
            {
                string namespaceName = _namespaceNameList[i];
                int index = containerNamespace.IndexOf(namespaceName);
                if(index==0 && (containerNamespace.Length == namespaceName.Length || containerNamespace[namespaceName.Length] == '.'))
                {
                    return true;
                }
            }
            return false;
        }

        /// <inheritdoc />
        public string GetInfo()
        {
            return "Namespace restriction(" + String.Join(",", _namespaceNameList) + ")";
        }

    }
}
