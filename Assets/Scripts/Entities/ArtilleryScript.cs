using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class ArtilleryScript : NetworkBehaviour, IDamagable
{

    [Header("Vehicle")]
    public Transform vehicleBody;
    public float turnRate = 90f;

    [Header("Weapons")]
    public Transform bulletSpawn;
    public Transform bulletPrefab;
    public Transform recoilBarrel;
    public float fireVelocity = 10f;
    public AudioSource fireAudio;

    [Header("Damage effects")]
    public BlinkColorScript damageBlink;
    public float blinkPeriodOnHit = 1f;
    public Vector3 scalePunch = new Vector3(0.5f, 0.75f, 0.5f);
    public float scalePunchPeriod = 1f;

    private Rigidbody body;
    private Quaternion targetRotation;
    private Vector3 barrelPos;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    // Use this for initialization
    void Start ()
    {
        barrelPos = recoilBarrel.localPosition;

        if (isServer)
            targetRotation = body.rotation;
    }

	// Update is called once per frame
	void Update ()
	{
        // restore barrel position if it's off-center
	    recoilBarrel.localPosition = Vector3.MoveTowards(recoilBarrel.localPosition, barrelPos, Time.deltaTime);
    }

    void FixedUpdate()
    {
        if (!isServer) // server only
            return;

        body.MoveRotation(Quaternion.RotateTowards(body.rotation, targetRotation, Time.deltaTime * turnRate));
    }

    #region SERVER

    [Command]
    public void CmdTurn(float relativeAngle)
    {
        targetRotation *= Quaternion.AngleAxis(relativeAngle, Vector3.up);
    }

    [Command]
    public void CmdFireBullet()
    {
        GameObject bullet = Game.Create(bulletPrefab.gameObject, bulletSpawn.position, bulletSpawn.rotation);
        bullet.GetComponent<BulletScript>().SetOwner(gameObject);
        NetworkServer.Spawn(bullet);

        RpcFireEffects();

        Rigidbody bulletBody = bullet.GetComponent<Rigidbody>();
        bulletBody.AddForce(bulletSpawn.forward * fireVelocity, ForceMode.Impulse);
    }

    public void TakeDamage(DamageData damage)
    {
        RpcTookDamage();
    }

    #endregion

    #region CLIENT

    [ClientRpc]
    void RpcFireEffects()
    {
        iTween.PunchPosition(recoilBarrel.gameObject, Vector3.down * 0.9f, 1f);
        ObjectPool.Get(fireAudio, transform.position, Quaternion.identity).GetComponent<AudioSource>().Play();
    }

    [ClientRpc]
    void RpcTookDamage()
    {
        if (damageBlink != null)
            damageBlink.Blink(blinkPeriodOnHit);

        iTween.Stop(vehicleBody.gameObject);
        vehicleBody.localScale = Vector3.one;

        // delay by a frame, can't seem to trigger tweens right after stopping them.
        vp_Timer.In(0, delegate() { // todo: proper null check in case the vehicle is destroyed
            iTween.PunchScale(vehicleBody.gameObject, scalePunch, scalePunchPeriod);
        });
        
    }

    #endregion
}
