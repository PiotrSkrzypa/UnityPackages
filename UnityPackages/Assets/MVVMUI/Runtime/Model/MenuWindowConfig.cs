using UnityEngine;
using Alchemy.Inspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PSkrzypa.MVVMUI
{
    /// <summary>
    /// ScriptableObject that defines the configuration for a UI screen.
    /// Includes settings like screen ID, whether it can be closed, and whether it's the initial screen.
    /// </summary>
    [CreateAssetMenu(fileName = "NewMenuWindowConfig", menuName = "UI/MenuWindowConfig", order = 0)]
    public class MenuWindowConfig : ScriptableObject
    {
        /// <summary>
        /// The unique identifier for the screen. This ID is automatically set to the name of the asset.
        /// </summary>
        public string windowID;

        /// <summary>
        /// Indicates whether this screen can be closed by the user.
        /// </summary>
        public bool canBeClosed = true;

        /// <summary>
        /// Indicates wheter this screen can be opened multiple times, allowing for multiple instances of the same screen.
        /// </summary>
        public bool allowMultipleInstances = false;

        /// <summary>
        /// Initl size of the pool for this window. This is used to optimize performance by reusing instances of the window.
        /// </summary>
        [ShowIf("allowMultipleInstances")]public int initialPoolSize = 1;

        /// <summary>
        /// Indicates whether this screen can be temporarily hidden, e.g. by dialogs UI.
        /// </summary>
        public bool canBeHidden = true;

        /// <summary>
        /// Specifies if this screen is the initial screen shown at the start.
        /// </summary>
        public bool isInitialScreen = false;

        /// <summary>
        /// View model type information. The actual instance is created by the resolver.
        /// </summary>
        public SerializedWindowType viewModelType;

        /// <summary>
        /// Prefab for the window associated with this configuration.
        /// </summary>
        public GameObject windowPrefab;

#if UNITY_EDITOR
        /// <summary>
        /// Called whenever this scriptable object is modified. Automatically sets the screenID to the name of the asset.
        /// </summary>
        private void OnValidate()
        {
            windowID = name.ToLower();
            EditorUtility.SetDirty(this);
        }
#endif
    }
}