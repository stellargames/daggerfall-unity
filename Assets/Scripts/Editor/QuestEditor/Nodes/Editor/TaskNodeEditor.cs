using UnityEngine;
using XNodeEditor;

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
}