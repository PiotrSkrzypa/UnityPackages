using Alchemy.Editor;
using PSkrzypa.MVVMUI.Samples;
using UnityEditor;

namespace PSkrzypa.MVVMUI.BaseMenuWindow
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BaseWindowView<>), true)]
    public class BaseWindowViewInspector : AlchemyEditor
    {

    }
}
