using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu]
public class WeaponData : ScriptableObject
{
    public enum WeaponType
    {
        Sword
    }

    public WeaponType weaponType;
    public List<AnimationClip> attackAnimations;

    public AnimatorController effectController;
    public List<AnimationClip> attackEffectAnimations;

    public List<float> attackMultipliers;
    public AnimationClip plungeAttackAnimation;
    public float plungeAttackMultiplier;
    public int critRate;
    public float critMultiplier;
    public float comboCooldown;
    public float attackCooldown;
}