using System;
using System.Linq;
using DaggerfallWorkshop.Game.Questing;
using UnityEditor;
using XNode;

namespace QuestEditor.Nodes
{
    [NodeTint(0.2f, 0.8f, 0.4f)]
    [CreateNodeMenu("Task")]
    [Serializable]
    public class TaskNode : Node, ISymbolize, IWatchInput
    {
        public string Symbol
        {
            get { return symbol; }
        }

        public Task.TaskType type;
        public string symbol;
        [Input(ShowBackingValue.Always)] public bool triggered;

        [Output(dynamicPortList = true, typeConstraint = TypeConstraint.Strict)]
        public TaskNode trigger;

        protected Symbol targetSymbol;
        protected bool prevTriggered;
        protected bool dropped;
        protected string globalVarName;
        protected int globalVarLink = -1;
        protected Quest Quest;

        public Task.TaskSaveData_v1 GetSaveData()
        {
            var connections = GetInputPort("triggered").GetConnections();
            var outputConnections = DynamicOutputs.Select(outputPort => outputPort.Connection).ToList();
            connections.AddRange(outputConnections);
            var actionSaveData = connections.Select(x => ((ActionNode) x.node).GetActionSaveData());

            Task.TaskSaveData_v1 data = new Task.TaskSaveData_v1
            {
                symbol = new Symbol(symbol),
                targetSymbol = targetSymbol,
                triggered = triggered,
                prevTriggered = prevTriggered,
                type = type,
                dropped = dropped,
                globalVarName = globalVarName,
                globalVarLink = type == Task.TaskType.GlobalVarLink ? globalVarLink : -1,
                hasTriggerConditions = GetInputPort("triggered").IsConnected,
                actions = actionSaveData.ToArray()
            };

            return data;
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return this;
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
            triggered = GetInputValue<bool>("triggered");
        }

        public void InputChanged(string fieldName, SerializedProperty property)
        {
            if (fieldName == "triggered")
            {
                triggered = GetInputValue<bool>("triggered");
            }
        }
    }
}