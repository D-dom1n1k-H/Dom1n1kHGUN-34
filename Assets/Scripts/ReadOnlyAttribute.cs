using UnityEngine;
using UnityEditor.UIElements;


#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UIElements;
#endif

public class ReadOnlyAttribute : PropertyAttribute { }

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyPropertyDrawer : PropertyDrawer
{
    // Для старого интерфейса (IMGUI)
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label);
        GUI.enabled = true;
    }

    // Для нового интерфейса (UIElements)
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var element = new PropertyField(property);
        element.SetEnabled(false);
        return element;
    }
}
#endif
