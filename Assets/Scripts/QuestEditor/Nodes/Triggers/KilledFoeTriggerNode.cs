using System;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.Questing.Actions;
using QuestEditor.Nodes.Resources;
using UnityEngine;
using XNode;

namespace QuestEditor.Nodes.Triggers
{
    [CreateNodeMenu("Trigger/KilledFoe")]
    [NodeTint(1.0f, 0.4f, 0.2f)]
    public class KilledFoeTriggerNode : ActionNode
    {
        [Input(ShowBackingValue.Never)] public FoeNode foe;

        [Range(1, 1000)] public int killsRequired;
        public int sayingId;

        [Output(ShowBackingValue.Always)] public bool trigger;

        public KilledFoeTriggerNode()
        {
            isTriggerCondition = true;
            killsRequired = 1;
        }

        protected override Type GetActionType()
        {
            return typeof(KilledFoe);
        }

        public override object GetValue(NodePort port)
        {
            switch (port.fieldName)
            {
                case "trigger":
                    return trigger;
                default:
                    return this;
            }
        }

        public override ActionTemplate GetAction()
        {
            var action = new KilledFoe(Quest);
            action.RestoreSaveData(GetSaveData());
            return action;
        }

        protected override object GetSaveData()
        {
            foe = GetInputValue<FoeNode>("foe");
            return new KilledFoe.SaveData_v1
            {
                foeSymbol = new Symbol(foe.name),
                killsRequired = killsRequired,
                sayingID = sayingId
            };
        }
    }
}