using DesignPatterns.ObjectPool;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectManager : MonoBehaviour
{
    [SerializeField] private StatusEffectStats statusEffectStats;
    [SerializeField] private ParticleVFXManager particleVFXManager;

    [Header("Status Stacks")]
    public StatusEffect bleedStacks;
    public StatusEffect burnStacks;
    public StatusEffect poisonStacks;
    public StatusEffect freezeStacks;
    public StatusEffect staticStacks;

    public event System.Action<StatusEffect.StatusType, int> OnApplyStatusEffect;
    public event System.Action<StatusEffect.StatusType.Status, float> OnThresholdReached;
    public event System.Action<StatusEffect.StatusType.Status> OnCleanse;

    [Header("Status Effect UI")]
    [SerializeField] private RectTransform effectCanvas;
    [SerializeField] private Transform statusEffectUIParent;
    private List<StatusEffectUI> currentEffectUIs = new();

    public void UpdateStatusEffects()
    {
        bleedStacks.UpdateStack();
        burnStacks.UpdateStack();
        poisonStacks.UpdateStack();
        freezeStacks.UpdateStack();
        staticStacks.UpdateStack();
    }

    public void ApplyStatusEffect(StatusEffect.StatusType statusEffect, int amount)
    {
        OnApplyStatusEffect?.Invoke(statusEffect, amount);

        switch (statusEffect.statusEffect)
        {
            case StatusEffect.StatusType.Status.Bleed:
                particleVFXManager.OnBleeding();

                if (bleedStacks.AddStack(amount))
                {
                    particleVFXManager.StopBleeding();
                    particleVFXManager.OnBloodLoss();

                    OnThresholdReached?.Invoke(StatusEffect.StatusType.Status.BloodLoss, 0);
                    bleedStacks.SetThreshold(bleedStacks.stackThreshold * statusEffectStats.bleedThresholdMultiplier);
                    RemoveEffectUI(StatusEffect.StatusType.Status.Bleed);
                    AudioManager.Instance.PlayOneShot(Sound.SoundName.Bleed);
                }
                else
                    AddEffectUI(statusEffect, bleedStacks.stackCount);
                break;
            case StatusEffect.StatusType.Status.Burn:
                burnStacks.AddStack(amount);
                AddEffectUI(statusEffect, burnStacks.stackCount);
                if (!AudioManager.Instance.CheckIfSoundPlaying(Sound.SoundName.Burn))
                    AudioManager.Instance.Play(Sound.SoundName.Burn);
                break;
            case StatusEffect.StatusType.Status.Poison:
                poisonStacks.AddStack(amount);
                AddEffectUI(statusEffect, poisonStacks.stackCount);
                AudioManager.Instance.PlayOneShot(Sound.SoundName.Poison);
                break;
            case StatusEffect.StatusType.Status.Freeze:
                if (freezeStacks.AddStack(amount))
                {
                    OnThresholdReached?.Invoke(StatusEffect.StatusType.Status.Frozen, statusEffectStats.frozenDuration);
                    RemoveEffectUI(StatusEffect.StatusType.Status.Freeze);
                    AudioManager.Instance.PlayOneShot(Sound.SoundName.Frozen);
                }
                else
                {
                    AddEffectUI(statusEffect, freezeStacks.stackCount);
                    AudioManager.Instance.PlayOneShot(Sound.SoundName.FreezingOrb);
                }
                break;
            case StatusEffect.StatusType.Status.Static:
                particleVFXManager.OnStatic();
                if (staticStacks.AddStack(amount))
                {
                    particleVFXManager.StopStatic();
                    particleVFXManager.OnStunned();

                    OnThresholdReached?.Invoke(StatusEffect.StatusType.Status.Stunned, statusEffectStats.stunDuration);
                    RemoveEffectUI(StatusEffect.StatusType.Status.Static);
                    AudioManager.Instance.PlayOneShot(Sound.SoundName.Stunned);
                }
                else
                {
                    AddEffectUI(statusEffect, staticStacks.stackCount);
                    AudioManager.Instance.PlayOneShot(Sound.SoundName.Static);
                }
                break;
            default:
                AddEffectUI(statusEffect, 0);
                break;
        }
    }

    private void AddEffectUI(StatusEffect.StatusType effectType, int count)
    {
        foreach (StatusEffectUI effectUI in currentEffectUIs)
        {
            if (effectUI.effectType.Equals(effectType))
            {
                effectUI.SetStackCount(count);
                return;
            }
        }

        StatusEffectUI statusEffectUI = ObjectPool.Instance.GetPooledObject("StatusEffectUI", true) as StatusEffectUI;
        statusEffectUI.SetStatusEffectUI(effectType, count);
        currentEffectUIs.Add(statusEffectUI);

        statusEffectUI.transform.SetParent(statusEffectUIParent);
        statusEffectUI.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        LayoutRebuilder.ForceRebuildLayoutImmediate(effectCanvas);
    }

    public void RemoveEffectUI(StatusEffect.StatusType.Status effectType)
    {
        StatusEffectUI effectUI = FindStatusEffectUI(effectType);
        if (effectUI == null)
            return;

        effectUI.RemoveStatusEffect();
        currentEffectUIs.Remove(effectUI);
    }

    public bool ReduceEffectStack(StatusEffect.StatusType.Status statusType, int amount)
    {
        StatusEffectUI effectUI = FindStatusEffectUI(statusType);
        if (effectUI == null)
            return false;

        switch (statusType)
        {
            case StatusEffect.StatusType.Status.Bleed:
                ReduceStackCount(bleedStacks, effectUI, amount);
                break;
            case StatusEffect.StatusType.Status.Burn:
                ReduceStackCount(burnStacks, effectUI, amount);

                if (burnStacks.stackCount == 0)
                    AudioManager.Instance.Stop(Sound.SoundName.Burn);
                break;
            case StatusEffect.StatusType.Status.Poison:
                ReduceStackCount(poisonStacks, effectUI, amount);
                break;
            case StatusEffect.StatusType.Status.Freeze:
                ReduceStackCount(freezeStacks, effectUI, amount);
                break;
            case StatusEffect.StatusType.Status.Static:
                ReduceStackCount(staticStacks, effectUI, amount);
                break;
            default:
                return false;
        }

        return true;
    }
    private void ReduceStackCount(StatusEffect targetEffect, StatusEffectUI effectUI, int amount)
    {
        targetEffect.RemoveStack(amount);

        if (targetEffect.stackCount <= 0)
        {
            effectUI.RemoveStatusEffect();
            currentEffectUIs.Remove(effectUI);
        }
        else
        {
            effectUI.SetStackCount(targetEffect.stackCount);
        }
    }

    private StatusEffectUI FindStatusEffectUI(StatusEffect.StatusType.Status effectType)
    {
        foreach (StatusEffectUI effectUI in currentEffectUIs)
        {
            if (effectUI.effectType.statusEffect == effectType)
                return effectUI;
        }

        return null;
    }

    public void Cleanse(StatusEffect.StatusType.Type type)
    {
        List<StatusEffectUI> effectUIsToRemove = new();

        for (int i = 0; i < currentEffectUIs.Count; i++)
        {
            if (!(currentEffectUIs[i].effectType.statusType == type))
                continue;

            effectUIsToRemove.Add(currentEffectUIs[i]);
        }

        foreach (StatusEffectUI effectUI in effectUIsToRemove)
        {
            OnCleanse?.Invoke(effectUI.effectType.statusEffect);

            if (!ReduceEffectStack(effectUI.effectType.statusEffect, Mathf.CeilToInt(Mathf.Infinity)))
            {
                effectUI.RemoveStatusEffect();
                currentEffectUIs.Remove(effectUI);
            }
        }
    }

    public void OnDie()
    {
        bleedStacks.stackCount = 0;
        burnStacks.stackCount = 0;
        poisonStacks.stackCount = 0;
        freezeStacks.stackCount = 0;
        staticStacks.stackCount = 0;
    }

    private void OnDisable()
    {
        OnApplyStatusEffect = null;
        OnThresholdReached = null;
        OnCleanse = null;
    }
}