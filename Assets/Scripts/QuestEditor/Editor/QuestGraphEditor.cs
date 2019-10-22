using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.Serialization;
using UnityEngine;
using XNodeEditor;

namespace QuestEditor.Editor
{
    [CustomNodeGraphEditor(typeof(QuestNodeGraph))]
    public class QuestGraphEditor : NodeGraphEditor
    {
        public override void OnOpen()
        {
            base.OnOpen();
//            TextResourceEditorWindow.Init();
        }

        public override void OnGUI()
        {
            var graph = target as QuestNodeGraph;
            if (graph == null) return;

            if (GUILayout.Button("Debug", GUILayout.Width(80)))
            {
                Quest.QuestSaveData_v1 questData = graph.GetQuest().GetSaveData();
                string json = SaveLoadManager.Serialize(questData.GetType(), questData);
                Debug.Log(json);
            }
        }
    }
}