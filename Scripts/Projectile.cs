using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int dmg = 10;
    public float speed = 10f;
    public GameObject target;
    public bool isAimingToBase = false;
    public Vector3 basePosition;

    void Update()
    {
        if (target == null && !isAimingToBase) Destroy(gameObject);
        else
        {
            if (!isAimingToBase) AimToEnemy();
            else AimToBase(basePosition);
        }
    }

    private void AimToEnemy()
    {
        Vector2 direction = (target.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        if (transform.position == target.transform.position)
        {
            DealDamageToEnemy();
            Destroy(gameObject);
        }
    }

    private void AimToBase(Vector3 basePosition)
    {
        transform.position = Vector3.MoveTowards(transform.position, basePosition, speed * Time.deltaTime);
        if (transform.position == basePosition)
        {
            DealDamageToBase();
            Destroy(gameObject);
        }
    }
    private void DealDamageToEnemy()
    {
        target.GetComponent<EnemyHealth>().TakeDamage(dmg);
    }

    private void DealDamageToBase()
    {
        EnemyBaseManagement.Instance.TakeDamage(dmg);
    }
}
