using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Power")]
    public int damage = 5;
    public int hp = 200;
    public float speed = 0.5f;

    [Header("Attack State")]
    public float attackCD = 3;
    public float attackDelay = 2;
    public float dealingDamageMoment = 0.5f;

    [Header("Enemy Loot")]
    public int fleeingSouls = 75;

    [Header("Enemy State")]
    public bool isAttacking = false;
    public bool isFreeze = false;
}
