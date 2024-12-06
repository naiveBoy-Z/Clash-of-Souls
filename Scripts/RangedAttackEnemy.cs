using System.Collections;
using TMPro;
using UnityEngine;

public class RangedAttackEnemy : AttackEnemy
{
    public GameObject projectileType;
    public override void Attack(GameObject target)
    {
        Vector3 spawnPosition = transform.position;

        Vector2 direction = (target.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion spawnRotation = Quaternion.Euler(0, 0, angle);

        GameObject attackingProjectile = Instantiate(projectileType, spawnPosition, spawnRotation);
        attackingProjectile.GetComponent<Projectile>().target = target;
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

            Vector3 spawnPosition = transform.position;
            Quaternion spawnRotation = Quaternion.identity;
            Vector3 basePosition = transform.position;
            basePosition.x = 24;
            GameObject attackingProjectile = Instantiate(projectileType, spawnPosition, spawnRotation);
            Projectile projectileStats = attackingProjectile.GetComponent<Projectile>();
            projectileStats.isAimingToBase = true;
            projectileStats.basePosition = basePosition;
            projectileStats.dmg = Mathf.RoundToInt(unitStats.damage * 0.8f);

            yield return new WaitForSeconds(1 - unitStats.dealingDamageMomoment);
            animator.SetBool("isAttacking", false);

            yield return new WaitForSeconds(unitStats.attackCD - 0.25f);
        }
    }
}
