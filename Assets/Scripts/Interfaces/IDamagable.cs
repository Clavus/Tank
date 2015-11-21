using UnityEngine;
using System.Collections;

public interface IDamagable
{
    void TakeDamage(DamageData damage);
}

public struct DamageData
{
    public readonly int amount;
    public readonly DamageType type;
    public readonly GameObject inflictor;

    public DamageData(int amount, DamageType dType = DamageType.Undefined, GameObject inflictor = null)
    {
        this.amount = amount;
        this.type = dType;
        this.inflictor = inflictor;
    }
}

public enum DamageType
{
    Undefined, Bullet, Explosion, Laser, Crush
}
