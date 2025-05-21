using UnityEngine;

namespace PSkrzypa.MVVMUI
{
    public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        static protected T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    //Debug.LogWarning("Database of type " + typeof(T).Name + "is not set");
                }
                return instance;
            }
        }
        private void OnEnable()
        {
            if (instance == null)
            {
                instance = this as T;
                instance.hideFlags = HideFlags.DontUnloadUnusedAsset;
            }
            else
            {
                // Debug.LogWarning("Database of type " + typeof(T).Name + "is already created");
            }
        }
        public abstract void RefreshDatabase();
    }
}