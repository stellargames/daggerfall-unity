using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QuestEditor.Editor.MessageEditor
{
    public class MessageReferenceEditorWindow : EditorWindow
    {
        private TextResource message;
        private readonly Dictionary<string, MessageCodeEditor> editors = new Dictionary<string, MessageCodeEditor>();
        private QuestNodeGraph quest;
        private int messageIndex;
        private int messageId;

        private string[] DropDownOptions { get ; set; }

        private SerializedProperty source;

        private readonly GUIContent addVariantButton = new GUIContent("Add variant",
            "Add a variation of the message. One variant will randomly be chosen when this message is used.");

        private readonly GUIContent deleteVariantButton = new GUIContent("x", "Delete this variation");

        public static void Open(SerializedProperty property, QuestNodeGraph quest)
        {
            var window = GetWindow<MessageReferenceEditorWindow>("Message Editor");
            window.minSize = new Vector2(550, 200);
            window.Init(property, quest);
            window.ShowUtility();
        }

        private void Init(SerializedProperty idProperty, QuestNodeGraph questGraph)
        {
            source = idProperty;
            quest = questGraph;
            messageId = source.intValue;
            SetDropDownOptions();
            message = quest.GetMessageById(messageId);
            messageIndex = quest.messages.IndexOf(message);
        }

        private void SetDropDownOptions()
        {
            DropDownOptions = quest.messages.Select(x => string.Format("{0} [{1}]", x.name, x.id)).ToArray();
        }

        public void OnGUI ()
        {
            EditorGUIUtility.labelWidth = 100;
            EditorGUIUtility.fieldWidth = 200;

            Header();

            if (message == null) return;

            MessageEditor();

            Footer();
        }

        private void Header()
        {
            if (DropDownOptions.Length > 0)
                MessageSelectorDropDown();

            if (GUILayout.Button("Create new", GUILayout.ExpandWidth(false)))
            {
                message = quest.CreateMessage();
                ChangeSelectedMessage();
            }
        }

        private void Footer()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Delete message", GUILayout.ExpandWidth(false)))
            {
                DeleteMessage();
            }

            if (GUILayout.Button("Save to node", GUILayout.ExpandWidth(false)))
            {
                UpdateSource();
                Close();
            }

            GUILayout.EndHorizontal();
        }

        private void DeleteMessage()
        {
            quest.messages.RemoveAt(messageIndex);
            message = quest.messages.Count > 0 ? quest.messages.First() : null;
            ChangeSelectedMessage();
            UpdateSource();
        }

        private void MessageEditor()
        {
            GUILayout.BeginHorizontal();
            message.name = EditorGUILayout.TextField("Name", message.name, GUILayout.ExpandWidth(false));
            EditorGUIUtility.labelWidth = 50;
            EditorGUIUtility.fieldWidth = 50;
            message.id = EditorGUILayout.IntField("Id", message.id, GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();


            for (int i = 0; i < message.variants.Count; i++)
            {
                VariantEditor(i);
            }

            if (GUILayout.Button(addVariantButton, GUILayout.ExpandWidth(false)))
            {
                message.variants.Add("");
            }
        }

        private void VariantEditor(int i)
        {
            MessageCodeEditor editor;
            string controlName = string.Format("message_{0}_variant_{1}", message.id, i);
            if (editors.ContainsKey(controlName))
            {
                editor = editors[controlName];
            }
            else
            {
                editor = new MessageCodeEditor(controlName, message.variants[i]);
                editors.Add(controlName, editor);
            }

            GUILayout.BeginHorizontal();
            message.variants[i] = editor.Draw();
            
            if (i > 0 && GUILayout.Button(deleteVariantButton, GUILayout.ExpandWidth(false)))
            {
                message.variants.RemoveAt(i);
            }
            GUILayout.EndHorizontal();
        }

        private void MessageSelectorDropDown()
        {
            EditorGUILayout.Space();
            int index = EditorGUILayout.Popup("Select message:", messageIndex, DropDownOptions, GUILayout.ExpandWidth(false));
            EditorGUILayout.Space();

            if (index < 0 || index == messageIndex) return;

            messageIndex = index;
            if (message == null || quest.messages[messageIndex].id != message.id)
            {
                message = quest.messages[messageIndex];
                ChangeSelectedMessage();
            }
        }

        private void ChangeSelectedMessage()
        {
            // Update the dropdown.
            messageIndex = quest.messages.IndexOf(message);
            SetDropDownOptions();
        }

        private void UpdateSource()
        {
            // Notify the source of the new id.
            source.intValue = (message == null) ? 0 : message.id;
            source.serializedObject.ApplyModifiedProperties();
        }
    }
}