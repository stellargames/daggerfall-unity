using System;
using System.Collections;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.Questing.Actions;
using UnityEngine;
using XNode;

[CreateNodeMenu("Trigger/KilledFoe")]
[NodeTint(1.0f, 0.4f, 0.2f)]
public class KilledFoeTriggerNode : ActionNode
{
    [Input(ShowBackingValue.Always)] public string foe;

    public int killsRequired;
    public int sayingId;

    [Output(ShowBackingValue.Always)] public bool trigger;

    protected override Type GetActionType()
    {
        return typeof(KilledFoe);
    }

    public override object GetValue(NodePort port)
    {
        return trigger;
    }

    public override ActionTemplate GetAction()
    {
        var action = new KilledFoe(Quest);
        action.RestoreSaveData(GetSaveData());
        return action;
    }

    protected override object GetSaveData()
    {
        return new KilledFoe.SaveData_v1
        {
            foeSymbol = new Symbol(foe),
            killsRequired = killsRequired,
            sayingID = sayingId
        };
    }

}