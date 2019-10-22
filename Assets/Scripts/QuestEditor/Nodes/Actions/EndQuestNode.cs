using System;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.Questing.Actions;

namespace QuestEditor.Nodes.Actions
{
    [CreateNodeMenu("Action/EndQuest")]
    public class EndQuestNode : ActionNode
    {
        public MessageReference message;

        public override ActionTemplate GetAction()
        {
            var action = new EndQuest(Quest);
            action.RestoreSaveData(GetSaveData());
            return action;
        }

        protected override object GetSaveData()
        {
            return new EndQuest.SaveData_v1 {textId = message.id };
        }

        protected override Type GetActionType()
        {
            return typeof(EndQuest);
        }
    }
}