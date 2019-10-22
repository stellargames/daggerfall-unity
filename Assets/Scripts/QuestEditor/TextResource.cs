using System;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Questing;

namespace QuestEditor
{
    [Serializable]
    public class TextResource
    {
        public int id;
        public string name;
        public List<string> variants;

        // Todo: Make DaggerfallWorkshop.Game.Questing.Message.splitToken public so we can use it here.
        private const string SplitToken = "<--->";

        public Message.MessageSaveData_v1 GetSaveData()
        {
            string fullText = string.Join(SplitToken + '\n', variants.ToArray());
            var lines = fullText.Split('\n');
            return new Message.MessageSaveData_v1
            {
                id = id,
                lines = lines
            };
        }
    }
}