using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ConditionalFieldAttribute))]
public class ConditionalFieldDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ConditionalFieldAttribute conditional = (ConditionalFieldAttribute)attribute;
        SerializedProperty comparedField = property.serializedObject.FindProperty(conditional.FieldToCheck);

        if (comparedField != null && comparedField.propertyType == SerializedPropertyType.Enum)
        {
            if (comparedField.enumValueIndex == (int)conditional.CompareValue)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
        else
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ConditionalFieldAttribute conditional = (ConditionalFieldAttribute)attribute;
        SerializedProperty comparedField = property.serializedObject.FindProperty(conditional.FieldToCheck);

        if (comparedField != null && comparedField.propertyType == SerializedPropertyType.Enum)
        {
            if (comparedField.enumValueIndex == (int)conditional.CompareValue)
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }
            else
            {
                return -EditorGUIUtility.standardVerticalSpacing;
            }
        }
        else
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
    }
}