using UnityEditor;
using UnityEngine;

namespace QuestEditor.Editor
{
    [CustomPropertyDrawer(typeof(MessageReference))]
    public class MessageReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var graph = ((XNode.Node) property.serializedObject.targetObject).graph as QuestNodeGraph;
            if (graph == null) return;

            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            label = EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            SerializedProperty idProperty = property.FindPropertyRelative("id");
            int id = idProperty.intValue;

            GUIContent button = id > 0 ? new GUIContent(id.ToString(), "Edit") : new GUIContent("None", "Select");

            if (GUI.Button(position, button))
            {
                MessageReferenceEditorWindow.Open(idProperty, graph);
            }

            EditorGUI.EndProperty();
        }
    }
}