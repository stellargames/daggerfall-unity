using System;
using DaggerfallWorkshop.Game.Questing;
using XNode;

[NodeTint(0.4f, 0.6f, 1f)]
public abstract class ActionNode : Node
{
    [Input] public bool triggered;
    protected Quest Quest;

    public Type type;
    protected bool IsTriggerCondition;
    protected bool IsAlwaysOnTriggerCondition;
    protected string debugSource;
    public object actionSpecific;
    
    public abstract ActionTemplate GetAction();

    protected abstract object GetSaveData();
    
    public override object GetValue(NodePort port)
    {
        return triggered;
    }

    protected override void Init()
    {
        base.Init();
        Quest = ((QuestNodeGraph) graph).Quest;
    }
    
    public ActionTemplate.ActionSaveData_v1 GetActionSaveData()
    {
        return new ActionTemplate.ActionSaveData_v1
        {
            type = GetType(),
            isComplete = false,
            isTriggerCondition = IsTriggerCondition,
            isAlwaysOnTriggerCondition = IsAlwaysOnTriggerCondition,
            debugSource = debugSource,
            actionSpecific = GetSaveData()
        };
    }
}