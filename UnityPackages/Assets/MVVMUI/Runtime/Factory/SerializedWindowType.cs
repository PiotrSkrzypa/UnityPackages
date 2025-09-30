using System;
using UnityEngine;

namespace PSkrzypa.MVVMUI
{
    [Serializable]
    public class SerializedWindowType
    {
        [SerializeField] private string assemblyQualifiedName;

        public Type StoredType =>
            string.IsNullOrEmpty(assemblyQualifiedName) ? null : Type.GetType(assemblyQualifiedName);

        public void Set(Type type)
        {
            assemblyQualifiedName = type?.AssemblyQualifiedName;
        }
    }

}