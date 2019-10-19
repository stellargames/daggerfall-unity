using System;
using DaggerfallWorkshop.Game.Questing;
using QuestEditor.Nodes.Resources;
using UnityEngine;

namespace QuestEditor.Nodes.Actions
{
    [CreateNodeMenu("Action/CreateFoe")]
    public class CreateFoeNode : ActionNode
    {
        [Input(ShowBackingValue.Never)] public FoeNode foe;

        // @TODO: convert minutes to days.hours:minutes and visa versa
        public uint spawnInterval;

        public int spawnMaxTimes;
        [Range(0, 100)] public int spawnChance = 100;
        public int msgMessageId;

        public override ActionTemplate GetAction()
        {
            var action = new CreateFoe(Quest);
            action.RestoreSaveData(GetSaveData());
            return action;
        }

        protected override object GetSaveData()
        {
            foe = GetInputValue<FoeNode>("foe");
            return new CreateFoe.SaveData_v1
            {
                foeSymbol = new Symbol(foe.name),
                lastSpawnTime = 0,
                spawnInterval = spawnInterval,
                spawnMaxTimes = spawnMaxTimes,
                spawnChance = spawnChance,
                spawnCounter = 0,
                isSendAction = false,
                msgMessageID = msgMessageId
            };
        }

        protected override Type GetActionType()
        {
            return typeof(CreateFoe);
        }
    }
}