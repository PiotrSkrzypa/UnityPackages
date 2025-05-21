using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PSkrzypa.MMVMUI.BaseMenuWindow
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
        /// Indicates whether this screen can be temporarily hidden, e.g. by dialogs UI.
        /// </summary>
        public bool canBeHidden = true;

        /// <summary>
        /// Specifies if this screen is the initial screen shown at the start.
        /// </summary>
        public bool isInitialScreen = false;

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