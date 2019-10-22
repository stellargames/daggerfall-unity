using System.Collections.Generic;
using System.Linq;
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
        public string questName;
        public string displayName;
        public int factionId;

        public List<TextResource> messages = new List<TextResource>();

        private const int StartingMessageId = 1100;
        public Quest Quest { get; private set; }

        private ulong uid;
        private bool questComplete;
        private bool questSuccess;
        private DaggerfallDateTime questStartTime;
        private bool questTombstoned;
        private DaggerfallDateTime questTombstoneTime;

        private List<Quest.LogEntry> activeLogMessages;

//        private readonly Dictionary<int, TextResource> messages = new Dictionary<int, TextResource>();
        private QuestResource.ResourceSaveData_v1[] resources;
        private Dictionary<string, Quest.QuestorData> questors;
        private Task.TaskSaveData_v1[] tasks;

        public Quest GetQuest()
        {
            Quest = new Quest();
            Quest.RestoreSaveData(GetSaveData());
            return Quest;
        }

        public TextResource CreateMessage()
        {
            var newMessage = new TextResource
            {
                id = NextFreeId(),
                name = "",
                variants = new List<string> {""}
            };
            messages.Add(newMessage);
            return newMessage;
        }

        public TextResource GetMessageById(int id)
        {
            return messages.FirstOrDefault(x => x.id == id);
        }

        private int NextFreeId()
        {
            if (messages.Count > 0)
            {
                return Mathf.Max(StartingMessageId, messages.Max(x => x.id) + 1);
            }

            return StartingMessageId;
        }

        public void SetMessage(TextResource message)
        {
            int index = messages.FindIndex(x => x.id == message.id);
            if (index < 0)
            {
                messages.Add(message);
            }
            else
            {
                messages[index] = message;
            }
        }


        private Quest.QuestSaveData_v1 GetSaveData()
        {
            List<QuestResource.ResourceSaveData_v1> resourceSaveDataList = new List<QuestResource.ResourceSaveData_v1>();
            List<Message.MessageSaveData_v1> messageSaveDataList = messages.Select(textResource => textResource.GetSaveData()).ToList();
            List<Task.TaskSaveData_v1> taskSaveDataList = new List<Task.TaskSaveData_v1>();

            foreach (Node node in nodes)
            {
                if (node is ResourceNode)
                {
                    resourceSaveDataList.Add(((ResourceNode) node).GetResourceSaveData());
                }
                else if (node is TaskNode)
                {
                    taskSaveDataList.Add(((TaskNode) node).GetSaveData());
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