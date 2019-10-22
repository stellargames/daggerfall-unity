using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace QuestEditor.Nodes.Editor
{
    [CustomNodeEditor(typeof(ResourceNode))]
    public class ResourceNodeEditor : NodeEditor {
    
        public override void OnHeaderGUI()
        {
            var node = target as ResourceNode;
            if (node == null) return;
            string text = string.Format("{0} [{1}]", node.name, node.Symbol);
        
            GUILayout.Label(text, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }

        public override void OnBodyGUI()
        {
            EditorGUIUtility.labelWidth = 120;
            base.OnBodyGUI();
        }

    }
}