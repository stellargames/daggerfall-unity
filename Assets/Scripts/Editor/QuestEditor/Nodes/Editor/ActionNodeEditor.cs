using UnityEditor;
using UnityEngine;
using XNodeEditor;
using System.Linq;
using XNode;

[CustomNodeEditor(typeof(ActionNode))]
public class ActionNodeEditor : NodeEditor {
    
    /// <summary> Draws standard field editors for all public fields </summary>
        public override void OnBodyGUI() {
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
            while (iterator.NextVisible(enterChildren)) {
                enterChildren = false;
                if (excludes.Contains(iterator.name)) continue;
//                NodeEditorGUILayout.PropertyField(iterator, true);
                DrawProperty(iterator);
            }
#endif

            // Iterate through dynamic ports and draw them in the order in which they are serialized
            foreach (XNode.NodePort dynamicPort in target.DynamicPorts) {
                if (NodeEditorGUILayout.IsDynamicPortListPort(dynamicPort)) continue;
                NodeEditorGUILayout.PortField(dynamicPort);
            }

            serializedObject.ApplyModifiedProperties();

#if ODIN_INSPECTOR
            // Call repaint so that the graph window elements respond properly to layout changes coming from Odin    
            if (GUIHelper.RepaintRequested) {
                GUIHelper.ClearRepaintRequest();
                window.Repaint();
            }
#endif

#if ODIN_INSPECTOR
            inNodeEditor = false;
#endif
        }

    protected void DrawProperty(SerializedProperty property)
    {
        XNode.Node node = property.serializedObject.targetObject as XNode.Node;
        XNode.NodePort port = node.GetPort(property.name);
        GUIContent label = null;
        if (port != null && port.IsConnected && port.IsInput)
        {
            var sourceNode = port.Connection.node as ISymbolize;
            if (sourceNode != null)
            {
                var text = string.Format("{0} [{1}]", property.displayName, sourceNode.Symbol);
                label = new GUIContent(text);
            }
        }

        EditorGUI.BeginChangeCheck();
        NodeEditorGUILayout.PropertyField(property, label, port, true);
        if (!EditorGUI.EndChangeCheck()) return;
        NodePort outputPort = node.GetOutputPort(property.name);
        if (!outputPort.IsConnected) return;
        NodePort inputPort = outputPort.Connection;
        var otherNode = inputPort.node as IChangeInput;
        if (otherNode != null) otherNode.InputChanged(inputPort.fieldName, property);
    }
    
    
}