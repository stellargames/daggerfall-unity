using System.Collections.Generic;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Utility;
using QuestEditor.Nodes;
using UnityEngine;
using XNode;

namespace QuestEditor
{
    [CreateAssetMenu]
    public class QuestNodeGraph : NodeGraph
    {
        public Quest Quest { get; private set; }

        public ulong uid;
        public bool questComplete;
        public bool questSuccess;
        public string questName;
        public string displayName;
        public int factionId;
        public DaggerfallDateTime questStartTime;
        public bool questTombstoned;
        public DaggerfallDateTime questTombstoneTime;
        public List<Quest.LogEntry> activeLogMessages;
        public Dictionary<int, Message> messages = new Dictionary<int, Message>();
        public QuestResource.ResourceSaveData_v1[] resources;
        public Dictionary<string, Quest.QuestorData> questors;
        public Task.TaskSaveData_v1[] tasks;

        public Quest GetQuest()
        {
            Quest = new Quest();
            Quest.RestoreSaveData(GetSaveData());
            return Quest;
        }

        private Quest.QuestSaveData_v1 GetSaveData()
        {
            List<Message.MessageSaveData_v1> messageSaveDataList = new List<Message.MessageSaveData_v1>();
            List<Task.TaskSaveData_v1> taskSaveDataList = new List<Task.TaskSaveData_v1>();
            List<QuestResource.ResourceSaveData_v1> resourceSaveDataList = new List<QuestResource.ResourceSaveData_v1>();

            foreach (Node node in nodes)
            {
                if (node is MessageNode)
                {
                    messageSaveDataList.Add(((MessageNode) node).GetSaveData());
                }
                else if (node is TaskNode)
                {
                    taskSaveDataList.Add(((TaskNode) node).GetSaveData());
                }
                else if (node is ResourceNode)
                {
                    resourceSaveDataList.Add(((ResourceNode) node).GetResourceSaveData());
                }
            }

            return new Quest.QuestSaveData_v1
            {
                uid = uid,
                questComplete = false,
                questSuccess = false,
                questName = questName,
                displayName = displayName,
                factionId = factionId,
                questStartTime = null,
                questTombstoned = false,
                questTombstoneTime = null,
                activeLogMessages = activeLogMessages.ToArray(),
                questors = questors,
                resources = resourceSaveDataList.ToArray(),
                messages = messageSaveDataList.ToArray(),
                tasks = taskSaveDataList.ToArray()
            };
        }
    }
}