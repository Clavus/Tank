using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof (Rigidbody))]
public class BulletScript : NetworkBehaviour
{
    [Tooltip("Set to -1 for infinite lifetime")]
    public float lifeTime = 3;

    public string effectOnImpact = "PlayerBulletImpact";
    public float gravityModifier = 1f;
    public bool turnToGravityDirection = true;

    private GameObject owner;
    private Rigidbody body;
    private float dieTime;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        body.useGravity = false;

        dieTime = Time.time + lifeTime;
    }

    private void Update()
    {
        if (turnToGravityDirection)
            transform.rotation = Quaternion.LookRotation(body.velocity.normalized, Vector3.up);

        if (lifeTime > 0 && dieTime <= Time.time)
        {
            Explode();
        }
    }

    private void FixedUpdate()
    {
        body.AddForce(Physics.gravity*gravityModifier, ForceMode.Acceleration);
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (!isServer) // server only
            return;

        //Debug.Log("Collide with " + coll.gameObject.name);

        if (owner != null && coll.attachedRigidbody != null && coll.attachedRigidbody.gameObject == owner)
            return;

        //if (coll.attachedRigidbody != null)
        //    Debug.Log("Collided with: " + coll.attachedRigidbody.gameObject.name);

        Damage.ApplyTo(coll, 1, DamageType.Bullet, owner);

        Explode();
    }

    public void Explode()
    {
        if (!isServer) // server only
            return;

        if (Effect.Exists(effectOnImpact))
            Effect.SpawnOnClient(effectOnImpact, transform.position);

        NetworkServer.Destroy(gameObject);
    }

    public void SetOwner(GameObject owner)
    {
        this.owner = owner;
    }
}