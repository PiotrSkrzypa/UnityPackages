using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System;
using System.Linq;

namespace PSkrzypa.MVVMUI.Editor
{
    [CustomPropertyDrawer(typeof(SerializedWindowType))]
    public class SerializedTypeDrawer : PropertyDrawer
    {
        private static string[] _typeNames;
        private static Type[] _types;

        static SerializedTypeDrawer()
        {
            // Cache all types inheriting BaseViewModel
            _types = TypeCache.GetTypesDerivedFrom<BaseViewModel>()
                              .Where(t => !t.IsAbstract && !t.IsGenericType)
                              .ToArray();
            _typeNames = _types.Select(t => t.FullName).Prepend("<None>").ToArray();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var aqnProp = property.FindPropertyRelative("assemblyQualifiedName");
            var currentType = string.IsNullOrEmpty(aqnProp.stringValue)
            ? null
            : Type.GetType(aqnProp.stringValue);

            int currentIndex = 0;
            if (currentType != null)
            {
                var idx = Array.IndexOf(_types, currentType);
                if (idx >= 0) currentIndex = idx + 1; // shift by 1 for "<None>"
            }

            int newIndex = EditorGUI.Popup(position, label.text, currentIndex, _typeNames);
            if (newIndex != currentIndex)
            {
                if (newIndex == 0)
                    aqnProp.stringValue = "";
                else
                    aqnProp.stringValue = _types[newIndex - 1].AssemblyQualifiedName;
            }
        }
    }
#endif

}