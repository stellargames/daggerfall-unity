using System;
using DaggerfallWorkshop.Game.Questing;
using UnityEngine;
using XNode;

namespace QuestEditor.Nodes
{
    [Serializable]
    public abstract class ResourceNode : Node, ISymbolize
    {
        public string Symbol
        {
            get { return symbol; }
        }

        [SerializeField] private string symbol;
        public MessageReference infoMessage;
        public MessageReference usedMessage;
        public MessageReference rumorsMessage;
        public bool isHidden;
        protected Quest Quest;

        protected override void Init()
        {
            base.Init();
            Quest = ((QuestNodeGraph) graph).Quest;
        }

        public override object GetValue(NodePort port)
        {
            return this;
        }

        public QuestResource.ResourceSaveData_v1 GetResourceSaveData()
        {
            return new QuestResource.ResourceSaveData_v1
            {
                type = GetResourceType(),
                symbol = new Symbol(symbol),
                infoMessageID = infoMessage.id,
                rumorsMessageID = rumorsMessage.id,
                usedMessageID = usedMessage.id,
                hasPlayerClicked = false,
                isHidden = isHidden,
                resourceSpecific = GetSaveData()
            };
        }

        protected abstract Type GetResourceType();

        protected abstract object GetSaveData();
        protected abstract QuestResource GetResource();
    }
}