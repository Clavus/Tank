using UnityEngine;
using System.Collections;

public class Explosion {

    public static ExplosionSystem CreateSystem(Vector3 position, float distance, float force, int damage, float upwardsMofidier = 0, GameObject inflictor = null)
    {
        ExplosionSystem explosion = (ExplosionSystem) Object.Instantiate(PrefabManagerScript.instance.explosionSystem, position,
            Quaternion.identity);

        explosion.distance = distance;
        explosion.force = force;
        explosion.damage = damage;
        explosion.upwardsModifier = upwardsMofidier;
        explosion.inflictor = inflictor;

        return explosion;
    }


}
