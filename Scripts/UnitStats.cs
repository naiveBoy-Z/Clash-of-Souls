using UnityEngine;

public class UnitStats : MonoBehaviour
{
    [Header("Power")]
    public int type = 1; // Type 1: Melee Unit, Type 2: Ranged Unit
    public int damage = 10;
    public int hp = 100;
    public int speed = 2;

    [Header("Attack State")]
    public float attackCD = 1;
    public float attackDelay = 1;
    public float dealingDamageMomoment = 0.25f;

    [Header("Loot")]
    public int fleeingSouls = 30;
}
