using DaggerfallWorkshop.Game.Questing;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

[CustomNodeGraphEditor(typeof(QuestNodeGraph))]
public class QuestGraphEditor : NodeGraphEditor {

    public override void OnGUI()
    {
        var graph = target as QuestNodeGraph;

        for (int n = 0; n < graph.nodes.Count; n++)
        {
            if (graph.nodes[n] == null) continue;
            if (n >= graph.nodes.Count) return;
            XNode.Node node = graph.nodes[n];
            
        }

        base.OnGUI();
        if (GUILayout.Button("Debug", GUILayout.Width(100)))
        {
            var questData = graph.GetQuest().GetSaveData();
            var json = DaggerfallWorkshop.Game.Serialization.SaveLoadManager.Serialize(questData.GetType(), questData);
            Debug.Log(json);
        }
    }
}