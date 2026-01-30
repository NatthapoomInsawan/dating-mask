using UnityEditor;
using UnityEngine;
using static DialogueData;

[CustomPropertyDrawer(typeof(DialogueSentence))]
public class DialogueSentenceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 1. Check if the managed reference is actually null
        if (property.managedReferenceValue == null)
        {
            DrawNullManagedReference(position, property, label);
            return;
        }

        EditorGUI.BeginProperty(position, label, property);

        // 2. Use nameof for safety, but with null checks
        SerializedProperty nameProp = property.FindPropertyRelative(nameof(DialogueSentence.CharacterName));
        SerializedProperty keyProp = property.FindPropertyRelative(nameof(DialogueSentence.ExpressionKey));
        SerializedProperty textProp = property.FindPropertyRelative(nameof(DialogueSentence.SentenceText));
        SerializedProperty choicesProp = property.FindPropertyRelative(nameof(DialogueSentence.Choices));

        // Draw Header
        Rect foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            float currentY = position.y + EditorGUIUtility.singleLineHeight + 2;

            // Simple helper to draw properties if they exist
            DrawProperty(ref currentY, nameProp, position.width);
            DrawProperty(ref currentY, keyProp, position.width);

            if (textProp != null)
            {
                float areaHeight = EditorGUIUtility.singleLineHeight * 2;
                EditorGUI.PropertyField(new Rect(position.x, currentY, position.width, areaHeight), textProp);
                currentY += areaHeight + 2;
            }

            if (choicesProp != null)
            {
                float listHeight = EditorGUI.GetPropertyHeight(choicesProp);
                EditorGUI.PropertyField(new Rect(position.x, currentY, position.width, listHeight), choicesProp, true);
            }
            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    private void DrawNullManagedReference(Rect position, SerializedProperty property, GUIContent label)
    {
        // This draws a button to initialize the class if it's null in a SerializeReference list
        Rect labelRect = new Rect(position.x, position.y, position.width * 0.6f, EditorGUIUtility.singleLineHeight);
        Rect btnRect = new Rect(position.x + labelRect.width, position.y, position.width * 0.4f, EditorGUIUtility.singleLineHeight);

        EditorGUI.LabelField(labelRect, label.text + " (Null Reference)");
        if (GUI.Button(btnRect, "Instantiate"))
        {
            property.managedReferenceValue = new DialogueSentence();
            property.serializedObject.ApplyModifiedProperties();
        }
    }

    private void DrawProperty(ref float y, SerializedProperty prop, float width)
    {
        if (prop == null) return;
        float height = EditorGUI.GetPropertyHeight(prop);
        EditorGUI.PropertyField(new Rect(prop.serializedObject.targetObject ? 0 : 0, y, width, height), prop);
        y += height + 2;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.ManagedReference)
        {
            if (property.managedReferenceValue == null) return EditorGUIUtility.singleLineHeight;
        }
        if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;

        float height = EditorGUIUtility.singleLineHeight + 10; // Header + Padding
        height += EditorGUIUtility.singleLineHeight + 2; // CharacterName
        height += EditorGUIUtility.singleLineHeight + 2; // ExpressionKey
        height += (EditorGUIUtility.singleLineHeight * 2) + 2; // SentenceText

        SerializedProperty choicesProp = property.FindPropertyRelative("Choices");
        if (choicesProp != null) height += EditorGUI.GetPropertyHeight(choicesProp);

        return height;
    }
}