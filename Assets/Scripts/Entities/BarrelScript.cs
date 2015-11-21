using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BarrelScript : NetworkBehaviour, IDamagable
{

    public float explosionDelayMin = 0.6f;
    public float explosionDelayMax = 0.9f;
    public GameObject meshObject;

    private bool hasExploded = false;
    private float explodeTime = -1;
    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (!isServer)
	        return;

	    if (explodeTime > 0 && explodeTime <= Time.time && !hasExploded)
	        Explode();
	}

    #region SERVER

    public void TakeDamage(DamageData damage)
    {
        if (damage.inflictor == gameObject)
            return;

        float boomTime = Time.time;
        if (damage.type == DamageType.Explosion)
        {
            float delay = Random.Range(explosionDelayMin, explosionDelayMax);
            boomTime = Time.time + delay; // delay explosion chains

            if (explodeTime < 0) // only trigger effect on first explosion contact
                RpcPrimeForExplosion(delay);
        }

        // set explosion timer to whatever fucked it up the fastest
        if (explodeTime < 0 || explodeTime > boomTime)
            explodeTime = boomTime;
    }

    void Explode()
    {
        if (!isServer) // server only
            return;

        hasExploded = true; // avoid infinite loop with explosion triggers

        Explosion.CreateSystem(transform.position, 1.5f, 50f, 1, 0.1f, gameObject).Trigger();
        Effect.SpawnOnClient("BarrelExplosion", transform.position);
        Effect.ShakeCamera(0.3f, 0.5f);
        NetworkServer.Destroy(gameObject);
    }

    #endregion

    #region CLIENT

    [ClientRpc]
    void RpcPrimeForExplosion(float delay)
    {
        iTween.PunchScale(meshObject, new Vector3(0.8f, 1.2f, 0.8f), delay);
        iTween.ColorTo(meshObject, Color.red, delay);
    }

    void OnDestroy()
    {

    }

    #endregion

}
