using System;
using UnityEngine;
using UnityEngine.UI;

namespace PSkrzypa.MVVMUI.Navigation
{
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