using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace QuestEditor.Editor
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

        public static void Open(SerializedProperty property, QuestNodeGraph quest)
        {
            var window = GetWindow<MessageReferenceEditorWindow>("Text Editor");
            window.minSize = new Vector2(400, 200);
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
            if (DropDownOptions.Length > 0)
                MessageSelectorDropDown();

            if (GUILayout.Button("Create new"))
            {
                message = quest.CreateMessage();
                ChangeSelectedMessage();
            }

            if (message == null) return;

            MessageEditor();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Delete"))
            {
                DeleteMessage();
            }

            if (GUILayout.Button("Save"))
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
            message.id = EditorGUILayout.IntField("Id", message.id);

            for (int i = 0; i < message.variants.Count; i++)
            {
                MessageCodeEditor editor = null;
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
                if (i > 0 && GUILayout.Button("x"))
                {
                    message.variants.RemoveAt(i);
                }

                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("+"))
            {
                message.variants.Add("");
            }
        }

        private void MessageSelectorDropDown()
        {
            int index = EditorGUILayout.Popup(messageIndex, DropDownOptions);
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