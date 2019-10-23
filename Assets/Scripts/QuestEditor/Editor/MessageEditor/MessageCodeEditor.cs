using QuestEditor.Editor.MessageEditor.ColorScheme;
using UnityEditor;
using UnityEngine;

namespace QuestEditor.Editor.MessageEditor
{
    public class MessageCodeEditor
    {
        private string TextValue { get;  set; }

        private readonly CodeEditor editor;
        private Vector2 scrollPos;
        private readonly GUIStyle guiStyle;

        public MessageCodeEditor(string name, string textValue)
        {
            TextValue = textValue;

            var font = EditorGUIUtility.Load("Assets/Resources/Fonts/TESFonts/Kingthings Petrock.ttf") as Font;

            Color bgColor;
            ColorUtility.TryParseHtmlString(Solarized.base03, out bgColor);

            editor = new CodeEditor(name)
            {
                backgroundColor = bgColor,
                textColor = Color.yellow,
                highlighter = QrcHighlighter.Highlight
            };
            guiStyle = new GUIStyle(GUI.skin.textArea)
            {
                padding = new RectOffset(6, 6, 6, 6),
                font = font,
                fontSize = 20,
                wordWrap = true
            };
        }

        public string Draw()
        {
            GUILayoutOption minHeight = GUILayout.MinHeight(32);
            GUILayoutOption maxHeight = GUILayout.MaxHeight(Screen.height);
            GUILayoutOption fixedWidth = GUILayout.Width(520);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, minHeight, maxHeight, fixedWidth);

            Color originalCursorColor = GUI.skin.settings.cursorColor; 
            GUI.skin.settings.cursorColor = Color.white;
            
            string editedCode = editor.Draw(TextValue, guiStyle);

            GUI.skin.settings.cursorColor = originalCursorColor;
            
            if (editedCode != TextValue)
            {
                TextValue = editedCode;
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space();

            return TextValue;
        }
    }
}