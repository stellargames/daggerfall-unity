using System;
using DaggerfallWorkshop.Game.Questing;
using UnityEngine;
using XNode;

[Serializable]
public abstract class ResourceNode : Node, ISymbolize
{
    public string Symbol
    {
        get { return symbol; }
        set { symbol = value; }
    }
    
    [SerializeField] private string symbol;
    public int infoMessageId;
    public int usedMessageId;
    public int rumorsMessageId;
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
            infoMessageID = infoMessageId,
            rumorsMessageID = rumorsMessageId,
            usedMessageID = usedMessageId,
            hasPlayerClicked = false,
            isHidden = isHidden,
            resourceSpecific = GetSaveData()
        };
    }

    protected abstract Type GetResourceType();

    protected abstract object GetSaveData();
    protected abstract QuestResource GetResource();
}