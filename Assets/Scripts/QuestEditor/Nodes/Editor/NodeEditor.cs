using System;
using System.Linq;
using Boo.Lang;
using UnityEditor;
using UnityEngine;
using XNode;

namespace QuestEditor.Nodes.Editor
{
    public abstract class NodeEditor : XNodeEditor.NodeEditor
    {
        protected Action<string> PropertyChanged;

        private readonly List<string> changedPropertyNames = new List<string>();
        
        /// <summary> Draws standard field editors for all public fields </summary>
        public override void OnBodyGUI()
        {
            serializedObject.Update();
            string[] excludes = { "m_Script", "graph", "position", "ports" };

            // Iterate through serialized properties and draw them like the Inspector (But with ports)
            SerializedProperty property = serializedObject.GetIterator();
            changedPropertyNames.Clear();
            bool enterChildren = true;
            while (property.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (excludes.Contains(property.name)) continue;
                
                EditorGUI.BeginChangeCheck();
                NodeEditorGuiLayout.PropertyField(property);
                if (EditorGUI.EndChangeCheck())
                {
                    changedPropertyNames.Add(property.name);
                }

                if (property.name == "symbol" && string.IsNullOrEmpty(property.stringValue))
                {
                    EditorGUILayout.HelpBox("Symbol must not be empty", MessageType.Warning);
                }
            }

            // Iterate through dynamic ports and draw them in the order in which they are serialized
            foreach (NodePort dynamicPort in target.DynamicPorts)
            {
                if (NodeEditorGuiLayout.IsDynamicPortListPort(dynamicPort)) continue;
                NodeEditorGuiLayout.PortField(dynamicPort);
            }

            serializedObject.ApplyModifiedProperties();

            // Allow node editors to react to changes to properties.
            foreach (string propertyName in changedPropertyNames)
            {
                if (PropertyChanged != null)
                    PropertyChanged(propertyName);                
            }
            
            window.Repaint();
        }
    }
}