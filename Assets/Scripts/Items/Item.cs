using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Item : ScriptableObject
{
    public ItemStats itemStats;

    public enum Rarity { Common, Uncommon, Legendary, Special };
    public Rarity itemRarity;

    public enum ItemType { HDHUD, AdrenalineShot, Gasoline, CrudeKnife, RitualSickle, HoloDie, BundleOfDynamite, ColdOne, WarmOne, 
        StunGrenade, Shungite, XKILLDrum, HealthPack, JackInTheBox, KnuckleDuster, CorruptedBoots, AmmoStash, Abascus, IncendiaryRound,
        PiercingRound, BlackCard};
    public ItemType itemType;

    public enum ItemCatagory { Damage, Healing, Utility };
    public ItemCatagory itemCatagory;

    public Sprite spriteIcon;
    [TextArea(3, 10)]
    public string title;
    [TextArea(3, 10)]
    public string description;
    public int itemStack;
    public float alertDuration;

    public virtual void Initialize() { }

    public virtual void IncrementStack() { itemStack++; }

    public virtual void DecrementStack() { itemStack--; }

    public void SetCount(int newCount) { itemStack = newCount; }
}