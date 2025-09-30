using System;
using Alchemy.Inspector;
using UnityEngine;
using Zenject;

namespace PSkrzypa.MVVMUI
{
    public class MenuControllerViewer : MonoBehaviour
    {
        [Inject] [NonSerialized] [ShowInInspector][ReadOnly] IMenuController menuController;
    }
}
