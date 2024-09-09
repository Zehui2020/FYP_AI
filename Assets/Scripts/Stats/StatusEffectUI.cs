using DesignPatterns.ObjectPool;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.WSA;

public class StatusEffectUI : PooledObject
{
    public enum StatusEffectType
    {
        Bleed,
        Burn,
        Poison,
        Freeze,
        Static,
        Breached
    }
    public StatusEffectType effectType;

    [System.Serializable]
    public struct StatusEffectHolder
    {
        public StatusEffectType effectType;
        public Sprite icon;
        public bool removeAfterNoStacks;
        public bool isStackable;
    }

    [SerializeField] private List<StatusEffectHolder> effects = new();
    [SerializeField] private Image effectIcon;
    [SerializeField] private TextMeshProUGUI effectStacks;

    public void SetStatusEffectUI(StatusEffectType type, int count)
    {
        foreach (StatusEffectHolder holder in effects)
        {
            if (holder.effectType.Equals(type))
            {
                if (count <= 0 && holder.removeAfterNoStacks)
                {
                    Release();
                    gameObject.SetActive(false);
                    return;
                }

                effectIcon.sprite = holder.icon;
                effectType = type;

                if (holder.isStackable)
                    effectStacks.text = count.ToString();
                else
                    effectStacks.text = string.Empty;

                break;
            }
        }
    }

    public void SetStackCount(int count)
    {
        if (count <= 0)
        {
            effectStacks.text = string.Empty;
            return;
        }

        effectStacks.text = count.ToString();
    }

    public void RemoveStatusEffect()
    {
        Release();
        gameObject.SetActive(false);
    }
}