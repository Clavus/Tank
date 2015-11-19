using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Damage {
    /// <summary>
    /// Apply damage to collider if it, or its rigidbody, has an IDamagable derived component.
    /// </summary>
    /// <param name="coll"></param>
    /// <param name="amount"></param>
    /// <param name="damageType"></param>
    /// <param name="inflictor"></param>
    public static void ApplyTo(Collider coll, int amount, DamageType damageType, GameObject inflictor)
    {
        IDamagable damagable = coll.GetComponent(typeof(IDamagable)) as IDamagable;
        if (damagable == null && coll.attachedRigidbody != null)
            damagable = coll.attachedRigidbody.GetComponent(typeof(IDamagable)) as IDamagable;
        if (damagable != null)
            damagable.TakeDamage(amount, damageType, inflictor);
    }

    /// <summary>
    /// Apply damage to game object if it has an IDamagable derived component.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="amount"></param>
    /// <param name="damageType"></param>
    /// <param name="inflictor"></param>
    public static void ApplyTo(GameObject obj, int amount, DamageType damageType, GameObject inflictor)
    {
        IDamagable damagable = obj.GetComponent(typeof(IDamagable)) as IDamagable;
        if (damagable != null)
            damagable.TakeDamage(amount, damageType, inflictor);
    }

    /// <summary>
    /// Applies explosion damage to a list of found colliders, but makes sure to apply damage only once to the same gameobject.
    /// </summary>
    /// <param name="colliders"></param>
    /// <param name="amount"></param>
    /// <param name="damageType"></param>
    /// <param name="inflictor"></param>
    public static void ApplyExplosionDamage(Collider[] colliders, int amount, DamageType damageType = DamageType.Explosion, GameObject inflictor = null)
    {
        List<IDamagable> seen = new List<IDamagable>();
        IDamagable damagable;

        foreach (Collider coll in colliders)
        {
            damagable = coll.GetComponent(typeof(IDamagable)) as IDamagable;
            if (damagable == null && coll.attachedRigidbody != null)
                damagable = coll.attachedRigidbody.GetComponent(typeof(IDamagable)) as IDamagable;
            if (damagable != null && !seen.Contains(damagable))
            {
                damagable.TakeDamage(amount, damageType, inflictor);
                seen.Add(damagable);   
            }
        }
    }

}
