using System;
using UnityEngine;
using Zenject;
using Alchemy.Inspector;

namespace PSkrzypa.MVVMUI
{
    public class MenuControllerViewer : MonoBehaviour
    {
        [Inject] [NonSerialized] [ShowInInspector][ReadOnly] MenuController menuController;
    }
}