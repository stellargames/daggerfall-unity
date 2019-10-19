using System.Linq;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace QuestEditor.Nodes.Editor
{
    [CustomNodeEditor(typeof(TaskNode))]
    public class TaskNodeEditor : NodeEditor
    {
        public override void OnHeaderGUI()
        {
            // Add the symbol of the task to the header.
            var node = target as TaskNode;
            if (node == null) return;
            string text = string.Format("{0} [{1}]", node.name, node.symbol);
            GUILayout.Label(text, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }

        /// <summary> Draws standard field editors for all public fields </summary>
        public override void OnBodyGUI()
        {
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
                NodeEditorGuiLayout.PropertyField(iterator);
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
#endif

#if ODIN_INSPECTOR
            inNodeEditor = false;
#endif
        }
    }
}