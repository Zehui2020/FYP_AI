using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Quake")]
public class Quake : BaseAbility
{
    public override void OnUseAbility(BaseStats self, BaseStats target)
    {
        if (target.GetComponent<Rigidbody2D>().velocity.y <= 0)
        {
            // deal damage
            float damageDealt = (abilityEffectValue / 100) * target.CalculateDamageDealt(target, out bool isCrit, out DamagePopup.DamageType damageType);
            target.TakeDamage(damageDealt, isCrit, target.transform.position, damageType);
            // push targets away
            Vector3 force = (target.transform.position - PlayerController.Instance.transform.position).normalized;
            force = new Vector3(force.x, 1, 0);
            target.GetComponent<Rigidbody2D>().AddForce(force * abilityDuration, ForceMode2D.Impulse);
        }
    }
}