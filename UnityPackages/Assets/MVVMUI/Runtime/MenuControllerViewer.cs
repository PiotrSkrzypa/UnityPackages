using System;
using UnityEngine;
using Zenject;

namespace PSkrzypa.MVVMUI
{
    public class MenuControllerViewer : MonoBehaviour
    {
        [Inject] [NonSerialized] /*[ShowInInspector][ReadOnly]*/ MenuController menuController;
    }
}