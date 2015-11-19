using UnityEngine;
using System.Collections;

public interface IDamagable
{
    void TakeDamage(int amount, DamageType damageType, GameObject inflictor = null);
}

public enum DamageType
{
    Bullet, Explosion, Laser
}
