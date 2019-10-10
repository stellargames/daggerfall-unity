using DaggerfallWorkshop.Game.Questing;
using XNode;

[NodeTint(0.8f, 0.8f, 0.0f)]
public class MessageNode : Node
{
    [Output(ShowBackingValue.Always)] public int id;
    public string[] lines;

    public override object GetValue(NodePort port)
    {
        return id;
    }

    public Message.MessageSaveData_v1 GetSaveData()
    {
        return new Message.MessageSaveData_v1 {id = id, lines = lines};
    }
}