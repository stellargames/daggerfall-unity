using System;
using System.Collections.Generic;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Utility;
using UnityEngine;

[CreateNodeMenu("Resource/Foe")]
[Serializable]
public class FoeNode : ResourceNode
{
    [Output] public FoeNode foe;
    [Range(0, Foe.maxSpawnCount)] public int spawnCount = 1;
    public string displayName;
    public MobileTypes foeType;
    public Genders humanoidGender;
    public bool injuredTrigger;
    public bool restrained;

    private Type type;
    private Foe.SaveData_v1 resourceSpecific;
    private ItemCollection itemQueue = new ItemCollection();
    private List<SpellReference> spellQueue = new List<SpellReference>();

    protected override QuestResource GetResource()
    {
        var resource = new Foe(Quest);
        resource.RestoreSaveData(GetSaveData());
        return resource;
    }

    protected override Type GetResourceType()
    {
        return typeof(Foe);
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
            killCount = 0,
            displayName = displayName,
            typeName = GetTypeName(foeType),
            spellQueue = spellQueue,
            itemQueue = (itemQueue != null) ? itemQueue.SerializeItems() : null
        };
    }

    private string GetDisplayName(MobileTypes mobileType)
    {
        // Monster types get a random monster name
        if (mobileType < MobileTypes.Humanoid)
        {
            DFRandom.srand(DateTime.Now.Millisecond + DFRandom.random_range(1, 1000000));
            return DaggerfallUnity.Instance.NameHelper.MonsterName();
        }

        // Randomly assign a gender for humanoid foes
        humanoidGender = (UnityEngine.Random.Range(0.0f, 1.0f) < 0.5f) ? Genders.Male : Genders.Female;

        // Create a random display name for humanoid foes
        DFRandom.srand(DateTime.Now.Millisecond);
        NameHelper.BankTypes nameBank = GameManager.Instance.PlayerGPS.GetNameBankOfCurrentRegion();
        return DaggerfallUnity.Instance.NameHelper.FullName(nameBank, humanoidGender);
    }

    private static string GetTypeName(MobileTypes mobileType)
    {
        MobileEnemy enemy;
        return EnemyBasics.GetEnemy(mobileType, out enemy) ? enemy.Name : mobileType.ToString();
    }
}