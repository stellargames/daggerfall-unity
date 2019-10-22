using UnityEditor;
using UnityEngine;

namespace QuestEditor.Editor
{
    public class MessageCodeEditor
    {
        public string Name { get; private set; }
        private string Value { get;  set; }

        private readonly uShaderTemplate.CodeEditor editor;
        private Vector2 scrollPos;
        private readonly GUIStyle guiStyle;

        public string Message
        {
            get { return Value ?? ""; }
            private set { Value = value; }
        }

        public MessageCodeEditor(string name, string value)
        {
            Name = name;
            Value = value;

            var font = EditorGUIUtility.Load("Assets/Resources/Fonts/TESFonts/Kingthings Petrock.ttf") as Font;

            Color color, bgColor;
            ColorUtility.TryParseHtmlString("#002b36", out bgColor);
            ColorUtility.TryParseHtmlString("#ffffff", out color);

            editor = new uShaderTemplate.CodeEditor(name)
            {
                backgroundColor = bgColor,
                textColor = color,
//                highlighter = ShaderHighlighter.Highlight
            };
            guiStyle = new GUIStyle(GUI.skin.textArea)
            {
                padding = new RectOffset(6, 6, 6, 6),
                font = font,
                fontSize = 20,
                wordWrap = false
            };
        }

        public string Draw()
        {
            GUILayoutOption minHeight = GUILayout.MinHeight(100);
            GUILayoutOption maxHeight = GUILayout.MaxHeight(Screen.height);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, minHeight, maxHeight);
            string editedCode = editor.Draw(Value, guiStyle, GUILayout.ExpandHeight(true));
            if (editedCode != Value)
            {
                Value = editedCode;
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();

            return Value;
        }
    }
}