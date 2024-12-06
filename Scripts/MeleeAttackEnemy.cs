using System.Collections;
using UnityEngine;

public class MeleeAttackEnemy : AttackEnemy
{
    public override void Attack(GameObject target)
    {
        EnemyHealth enemyHealth = target.GetComponent<EnemyHealth>();
        enemyHealth.TakeDamage(unitStats.damage);
    }

    public override void AttackBase()
    {
        attackBaseCoroutine = StartCoroutine(AttackBaseCoroutine());
    }

    public IEnumerator AttackBaseCoroutine()
    {
        // delay before attacking the enemy base
        yield return new WaitForSeconds(unitStats.attackDelay);

        // attack the enemy base until it is destroyed
        while (BaseManagement.Instance.maxHP > 0)
        {
            // do NOT attack if not ready to attack the base
            while (!readyToAttackBase)
            {
                yield return null;
            }


            // attack
            animator.SetBool("isAttacking", true);
            yield return new WaitForSeconds(unitStats.dealingDamageMomoment);

            EnemyBaseManagement.Instance.TakeDamage(Mathf.RoundToInt(unitStats.damage * 0.8f));

            yield return new WaitForSeconds(1 - unitStats.dealingDamageMomoment);
            animator.SetBool("isAttacking", false);

            yield return new WaitForSeconds(unitStats.attackCD - 0.25f);
        }
    }
}
