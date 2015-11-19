using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ExplosionSystem : MonoBehaviour
{

    public float distance = 3f;
    public float force = 100f;
    public int damage = 1;
    public float upwardsModifier = 0f;
    public GameObject inflictor;
    public LayerMask layerMask;

	void Start () {
	
	}

	void Update () {
	
	}

    public void Trigger(bool destroyAfter = true)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, distance, layerMask.value);
        Damage.ApplyExplosionDamage(colliders, damage, DamageType.Explosion, inflictor);

        List<Rigidbody> bodies = new List<Rigidbody>();
        foreach (Collider coll in colliders)
        {
            if (coll.attachedRigidbody != null && !bodies.Contains(coll.attachedRigidbody))
                bodies.Add(coll.attachedRigidbody);
        }

        foreach (Rigidbody body in bodies)
        {
            body.AddExplosionForce(force, transform.position, distance, upwardsModifier, ForceMode.Impulse);
        }
        
        if (destroyAfter)
            Destroy(gameObject);
    }
}
