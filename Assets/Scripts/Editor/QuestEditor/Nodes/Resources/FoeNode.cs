using System;
using System.Collections.Generic;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.Serialization;
using XNode;

[CreateNodeMenu("Resource/Foe")]
public class FoeNode : ResourceNode
{
    [Output] public Foe foe;

    public int spawnCount = 1;
    public string displayName;
    public MobileTypes foeType;
    public Genders humanoidGender;
    public bool injuredTrigger;
    public bool restrained;

    private Type type;
    private Foe.SaveData_v1 resourceSpecific;
    private ItemCollection itemQueue = new ItemCollection();
    private int killCount = 0;
    private string typeName = String.Empty;
    private List<SpellReference> spellQueue = new List<SpellReference>();

    public override object GetValue(NodePort port)
    {
        return foe;
    }
    
    public QuestResource GetResource()
    {
        var resource = new Foe(Quest);
        resource.RestoreSaveData(GetSaveData());
        return resource;
    }

    protected override object GetSaveData()
    {
        return new Foe.SaveData_v1
        {
            spawnCount = spawnCount,
            foeType = foeType,
            humanoidGender = humanoidGender,
            injuredTrigger = injuredTrigger,
            restrained = restrained,
            killCount = killCount,
            displayName = displayName,
            typeName = typeName,
            spellQueue = spellQueue,
            itemQueue = (itemQueue != null) ? itemQueue.SerializeItems() : null
        };
    }
}