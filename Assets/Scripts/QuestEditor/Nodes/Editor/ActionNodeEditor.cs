using System.Linq;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace QuestEditor.Nodes.Editor
{
    [CustomNodeEditor(typeof(ActionNode))]
    public class ActionNodeEditor : NodeEditor
    {
        /// <summary> Draws standard field editors for all public fields </summary>
        public override void OnBodyGUI()
        {
            EditorGUIUtility.labelWidth = 100;
#if ODIN_INSPECTOR
            inNodeEditor = true;
#endif

            // Unity specifically requires this to save/update any serial object.
            // serializedObject.Update(); must go at the start of an inspector gui, and
            // serializedObject.ApplyModifiedProperties(); goes at the end.
            serializedObject.Update();
            string[] excludes = { "m_Script", "graph", "position", "ports" };

#if ODIN_INSPECTOR
            InspectorUtilities.BeginDrawPropertyTree(objectTree, true);
            GUIHelper.PushLabelWidth(84);
            objectTree.Draw(true);
            InspectorUtilities.EndDrawPropertyTree(objectTree);
            GUIHelper.PopLabelWidth();
#else

            // Iterate through serialized properties and draw them like the Inspector (But with ports)
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (excludes.Contains(iterator.name)) continue;
//                NodeEditorGUILayout.PropertyField(iterator, true);
                DrawProperty(iterator);
            }
#endif

            // Iterate through dynamic ports and draw them in the order in which they are serialized
            foreach (NodePort dynamicPort in target.DynamicPorts)
            {
                if (NodeEditorGuiLayout.IsDynamicPortListPort(dynamicPort)) continue;
                NodeEditorGuiLayout.PortField(dynamicPort);
            }

            serializedObject.ApplyModifiedProperties();

#if ODIN_INSPECTOR
            // Call repaint so that the graph window elements respond properly to layout changes coming from Odin    
            if (GUIHelper.RepaintRequested) {
                GUIHelper.ClearRepaintRequest();
                window.Repaint();
            }
            inNodeEditor = false;
#endif
        }

        protected void DrawProperty(SerializedProperty property)
        {
            var node = property.serializedObject.targetObject as ActionNode;
            if (node == null) return;
            // Do not show triggered property on trigger action nodes.
            if (node.isTriggerCondition && property.name == "triggered") return;
            NodePort port = node.GetPort(property.name);
            GUIContent label = null;

            // Add parent symbol to input port labels.
            if (port != null && port.IsConnected && port.IsInput)
            {
                var sourceNode = port.Connection.node as ISymbolize;
                if (sourceNode != null)
                {
                    string text = string.Format("{0} [{1}]", property.displayName, sourceNode.Symbol);
                    label = new GUIContent(text);
                }
            }

            // Update child inputs with changes.
            EditorGUI.BeginChangeCheck();
            NodeEditorGuiLayout.PropertyField(property, label, port);

            if (!EditorGUI.EndChangeCheck()) return;

            NodePort outputPort = node.GetOutputPort(property.name);
            if (outputPort == null || !outputPort.IsConnected) return;
            NodePort inputPort = outputPort.Connection;

            var otherNode = inputPort.node as IWatchInput;
            serializedObject.ApplyModifiedProperties();
            if (otherNode != null) otherNode.InputChanged(inputPort.fieldName, property);
        }
    }
}