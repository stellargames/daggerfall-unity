using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.Questing.Actions;

[CreateNodeMenu("Action/EndQuest")]
public class EndQuestNode : ActionNode
{
    public int textId;

    public override ActionTemplate GetAction()
    {
        var action = new EndQuest(Quest);
        action.RestoreSaveData(GetSaveData());
        return action;
    }

    protected override object GetSaveData()
    {
        return new EndQuest.SaveData_v1 {textId = textId };
    }
}