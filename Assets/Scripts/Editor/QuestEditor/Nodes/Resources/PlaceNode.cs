using System;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.Questing;

[CreateNodeMenu("Resource/Place")]
public class PlaceNode : ResourceNode
{
    public Place.Scopes scope;
    public new string name;
    public int p1;
    public int p2;
    public int p3;
    public SiteDetails siteDetails;

    protected override QuestResource GetResource()
    {
        var resource = new Place(Quest);
        resource.RestoreSaveData(GetSaveData());
        return resource;
    }

    protected override Type GetResourceType()
    {
        return typeof(Place);
    }

    protected override object GetSaveData()
    {
        return new Place.SaveData_v1
        {
            scope = scope,
            name = name,
            p1 = p1,
            p2 = p2,
            p3 = p3,
            siteDetails = siteDetails
        };
    }
}