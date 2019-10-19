using UnityEngine;
using XNodeEditor;

namespace QuestEditor.Editor
{
    [CustomNodeGraphEditor(typeof(QuestNodeGraph))]
    public class QuestGraphEditor : NodeGraphEditor
    {
        public override void OnGUI()
        {
            base.OnGUI();
            var graph = target as QuestNodeGraph;
            if (graph == null) return;
            if (GUILayout.Button("Debug", GUILayout.Width(100)))
            {
                var questData = graph.GetQuest().GetSaveData();
                var json = DaggerfallWorkshop.Game.Serialization.SaveLoadManager.Serialize(questData.GetType(), questData);
                Debug.Log(json);
            }
        }
    }
}