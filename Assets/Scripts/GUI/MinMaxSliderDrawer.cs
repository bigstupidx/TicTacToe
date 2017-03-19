#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
class MinMaxSliderDrawer : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        if (property.propertyType == SerializedPropertyType.Vector2) {
            Vector2 range = property.vector2Value;
            float min = range.x;
            float max = range.y;
            MinMaxSliderAttribute attr = attribute as MinMaxSliderAttribute;
            EditorGUI.BeginChangeCheck();
            // EditorGUI.LabelField(position, System.Math.Round(range.x, 2).ToString());
            EditorGUI.MinMaxSlider(position, label, ref min, ref max, attr.min, attr.max);
            // EditorGUI.LabelField(position, System.Math.Round(range.y, 2).ToString());
            if (EditorGUI.EndChangeCheck()) {
                range.x = min;
                range.y = max;
                property.vector2Value = range;
            }
        } else {
            EditorGUI.LabelField(position, label, "Use only with Vector2");
        }
    }
}
#endif