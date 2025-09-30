using System;
using UnityEngine;
using UnityEngine.UI;

namespace PSkrzypa.MVVMUI.Navigation
{
    /// <summary>
    /// Component that allows to specify an alternative Selectable UI element to be used for navigation if the main one is not interactable.
    /// </summary>
    [Serializable]
    public class SelectableWithAlternative : MonoBehaviour
    {
        [SerializeField] Selectable alternativeSelectable;

        public Selectable GetAlternativeSelectable()
        {
            if (alternativeSelectable != null && alternativeSelectable.interactable)
            {
                return alternativeSelectable;
            }
            return null;
        }
    }
}