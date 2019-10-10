using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DaggerfallWorkshop.Game.Questing;
using UnityEngine;
using XNode;

[NodeTint(0.2f, 0.8f, 0.4f)]
public class TaskNode : Node
{
    public Task.TaskType type;
    public string symbol;
    [Input(ShowBackingValue.Always)] public bool triggered;
    [Output(ShowBackingValue.Always)] public bool trigger;

    protected Symbol targetSymbol;
    protected bool prevTriggered;
    protected bool dropped;
    protected string globalVarName;
    protected int globalVarLink;
    protected Quest Quest;

    public Task.TaskSaveData_v1 GetSaveData()
    {
        var connections = GetInputPort("triggered").GetConnections().Concat(
            GetOutputPort("trigger").GetConnections()
        );

        Task.TaskSaveData_v1 data = new Task.TaskSaveData_v1
        {
            symbol = new Symbol(symbol),
            targetSymbol = targetSymbol,
            triggered = triggered,
            prevTriggered = prevTriggered,
            type = type,
            dropped = dropped,
            globalVarName = globalVarName,
            globalVarLink = globalVarLink,
            hasTriggerConditions = GetInputPort("triggered").IsConnected,
            actions = connections.Select(port => port.node).OfType<ActionNode>().Select(actionNode => actionNode.GetActionSaveData()).ToArray()
        };

        return data;
    }

    // Return the correct value of an output port when requested
    public override object GetValue(NodePort port)
    {
        triggered = GetInputValue<bool>("triggered");
        return symbol;
    }

    public Task GetTask()
    {
        Task task = new Task(Quest);
        task.RestoreSaveData(GetSaveData());
        return task;
    }

    protected override void Init()
    {
        base.Init();
        Quest = ((QuestNodeGraph) graph).Quest;
    }
}