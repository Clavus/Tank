using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

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
        IDamagable[] damagables = coll.GetComponents(typeof(IDamagable)).Cast<IDamagable>().ToArray();
        DamageData dm = new DamageData(amount, damageType, inflictor);
        Array.ForEach(damagables, damagable => damagable.TakeDamage(dm));

        // Also apply damage to rigidbody of this collider, but only if they're different gameobjects (otherwise we already did)
        if (coll.attachedRigidbody != null && coll.gameObject != coll.attachedRigidbody.gameObject)
        {
            IDamagable[] rigidBodyDamagables = coll.attachedRigidbody.GetComponents(typeof(IDamagable)).Cast<IDamagable>().ToArray();
            Array.ForEach(rigidBodyDamagables, damagable => damagable.TakeDamage(dm));
        }
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
        IDamagable[] damagables = obj.GetComponents(typeof(IDamagable)).Cast<IDamagable>().ToArray();
        DamageData dm = new DamageData(amount, damageType, inflictor);
        Array.ForEach(damagables, damagable => damagable.TakeDamage(dm));
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
        List<GameObject> seen = new List<GameObject>();
        DamageData dm = new DamageData(amount, damageType, inflictor);
        IDamagable[] damagables;
        IDamagable[] rigidBodyDamagables;

        foreach (Collider coll in colliders)
        {
            if (seen.Contains(coll.gameObject))
                continue;

            seen.Add(coll.gameObject); // avoid applying damage twice to the IDamagable components on the same game object

            damagables = coll.GetComponents(typeof (IDamagable)).Cast<IDamagable>().ToArray();
            Array.ForEach(damagables, damagable => damagable.TakeDamage(dm)); ;

            // Also apply damage to rigidbody of this collider, but only if they're different gameobjects (otherwise we already did)
            if (coll.attachedRigidbody != null && !seen.Contains(coll.attachedRigidbody.gameObject))
            {
                seen.Add(coll.attachedRigidbody.gameObject);

                rigidBodyDamagables = coll.attachedRigidbody.GetComponents(typeof(IDamagable)).Cast<IDamagable>().ToArray();
                Array.ForEach(rigidBodyDamagables, damagable => damagable.TakeDamage(dm));
            }
        }

        
    }

}
