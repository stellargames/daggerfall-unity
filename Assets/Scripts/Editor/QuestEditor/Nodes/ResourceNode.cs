using DaggerfallWorkshop.Game.Questing;
using XNode;

public abstract class ResourceNode : Node
{
    [Output(ShowBackingValue.Always)] public string symbol;
    public int infoMessageId;
    public int usedMessageId;
    public int rumorsMessageId;
    private bool hasPlayerClicked;
    public bool isHidden;
    protected Quest Quest;

    protected override void Init()
    {
        base.Init();
        Quest = ((QuestNodeGraph) graph).Quest;
    }

    public QuestResource.ResourceSaveData_v1 GetResourceSaveData()
    {
        return new QuestResource.ResourceSaveData_v1
        {
            type = GetType(),
            symbol = new Symbol(symbol),
            infoMessageID = infoMessageId,
            usedMessageID = usedMessageId,
            hasPlayerClicked = hasPlayerClicked,
            isHidden = isHidden,
            resourceSpecific = GetSaveData()
        };
    }

    protected abstract object GetSaveData();
}