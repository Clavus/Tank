using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class TankScript : NetworkBehaviour, IDamagable
{

    public Transform turret;
    public Transform wheelBase;
    public Transform vehicleBody;
    public Transform bulletSpawn;
    public Transform bulletPrefab;
    public float fireVelocity = 20f;
    public AudioSource fireAudio;

    [SyncVar]
    private Quaternion turretTargetRot = Quaternion.identity;

    [SyncVar]
    private Quaternion wheelBaseTargetRot = Quaternion.identity;

    private Rigidbody body;

    private Vector3 moveDir = Vector3.forward;
    private Vector3 wheelForward = Vector3.forward;
    private Vector3 wheelUp = Vector3.up;

    private float lastGroundContact = 0;
    private float wheelBaseUpdatesPerSec = 15;
    private int wheelBaseUpdateFrameInterval;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

	// Use this for initialization
	void Start ()
	{
        if (isServer)
	        turretTargetRot = turret.rotation;

	    if (isLocalPlayer)
	        CameraManagerScript.SetTrackingObject(transform);

	    wheelBaseUpdateFrameInterval = (int) ((1/Time.fixedDeltaTime) * (1/wheelBaseUpdatesPerSec));
	}
	
	void Update ()
    {
	    turret.rotation = Quaternion.RotateTowards(turret.rotation, turretTargetRot, Time.deltaTime*720);
        wheelBase.transform.rotation = Quaternion.RotateTowards(wheelBase.transform.rotation, wheelBaseTargetRot,
                Time.deltaTime * 180);
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        if (!isServer && !isLocalPlayer) return;

        wheelUp = Vector3.zero;

        foreach (ContactPoint contact in collisionInfo.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5)
            {
                float dist = 0.1f;
                Vector3 cast = Vector3.up*dist;
                RaycastHit hit;
                Debug.DrawRay(contact.point + cast * 0.09f, Vector3.down*dist, Color.red);
                if (Physics.Raycast(contact.point + cast*0.09f, Vector3.down, out hit, dist,
                    ~LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer))))
                {
                    wheelUp += hit.normal;
                }

                lastGroundContact = Time.time;
                //wheelUp += contact.normal;

            }
            //Debug.DrawRay(contact.point, contact.normal * 3, Color.red);
        }

        
        // no valid hits, return to normal
        if (wheelUp.magnitude < 1)
            wheelUp = Vector3.up;

        wheelUp.Normalize();
    }

    bool IsOnGround()
    {
        return lastGroundContact >= Time.time - Time.deltaTime;
    }

    public void MoveBy(Vector3 moveVec, float tractionRegainSpeed, float airControlMultiplier)
    {
        if (!isServer && !isLocalPlayer)
            return;

        // Yay for movement tweaking

        // METHOD 1 (MovePosition with velocity tweaks for jumps 'n shit)
        // if we were launched, and we're trying to move opposing our velocity, counter-act the velocity first
        if (body.velocity.magnitude > 0.3f && Vector3.Dot(moveVec.normalized, body.velocity.normalized) < -0.05)
        {
            body.AddForce(moveVec / Time.deltaTime, ForceMode.Acceleration);
            Debug.DrawRay(transform.position, moveVec.normalized * 3f, Color.red);
        }
        else
        {
            body.AddForce(moveVec / Time.deltaTime * 0.3f, ForceMode.Acceleration); // add additional force to counter-act other forces
            body.MovePosition(transform.position + moveVec);
        }

        // METHOD 2
        /*if (IsOnGround() && Vector3.Dot(body.velocity.normalized, moveVec.normalized) < 0.95 && body.velocity.magnitude < tractionRegainSpeed)
            body.velocity = Vector3.zero;

        float velAlongMoveVec = Vector3.Dot(body.velocity, moveVec.normalized);
        moveVec = IsOnGround() ? moveVec : moveVec * airControlMultiplier;
        if (velAlongMoveVec < moveVec.magnitude)
            body.AddForce(moveVec * (moveVec.magnitude - Mathf.Max(0,velAlongMoveVec)) / moveVec.magnitude, ForceMode.VelocityChange);*/

        HandleMove(moveVec);
    }

    private void HandleMove(Vector3 moveVec)
    {
        if (moveVec.magnitude > 0)
            moveDir = moveVec.normalized;

        // move wheel base back if we haven't touched the ground for half a sec
        if (lastGroundContact < Time.time - 0.5)
            wheelUp = Vector3.up;

        wheelForward = Quaternion.FromToRotation(Vector3.up, wheelUp) * moveDir;

        Debug.DrawRay(wheelBase.position, wheelUp * 2, Color.green);
        Debug.DrawRay(wheelBase.position, wheelForward * 2, Color.blue);

        if (wheelForward.magnitude > 0)
        {
            Quaternion targetRot = Quaternion.LookRotation(wheelForward, wheelUp);
            if (Time.frameCount % wheelBaseUpdateFrameInterval == 0)
                CmdUpdateWheelBaseRot(targetRot);
        }
    }

    #region SERVER

    [Command]
    public void CmdUpdateWheelBaseRot(Quaternion rot)
    {
        wheelBaseTargetRot = rot;
    }

    [Command]
    public void CmdTurn(bool turnLeft)
    {
        if (turnLeft)
            turretTargetRot = turretTargetRot * Quaternion.AngleAxis(-45, Vector3.up);
        else
            turretTargetRot = turretTargetRot * Quaternion.AngleAxis(45, Vector3.up);
    }

    [Command]
    public void CmdFireBullet()
    {
        GameObject bullet = (GameObject) Instantiate(bulletPrefab.gameObject, bulletSpawn.position, bulletSpawn.rotation);
        bullet.GetComponent<BulletScript>().SetOwner(gameObject);
        NetworkServer.Spawn(bullet);

        Rigidbody bulletBody = bullet.GetComponent<Rigidbody>();
        bulletBody.AddForce(bulletSpawn.forward * fireVelocity, ForceMode.Impulse);

        RpcFireEffects();
        //bulletBody.velocity = bulletSpawn.forward * fireVelocity; 
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
        if (fireAudio != null)
            ObjectPool.Get(fireAudio, transform.position, Quaternion.identity).GetComponent<AudioSource>().Play();
    }

    [ClientRpc]
    void RpcTookDamage()
    {
        // Punch vehicle scale
        iTween.Stop(vehicleBody.gameObject);
        vehicleBody.localScale = Vector3.one;

        // delay by a frame, can't seem to trigger tweens right after stopping them.
        // todo: proper null check in case the vehicle is destroyed
        vp_Timer.In(0, delegate () {
            iTween.PunchScale(vehicleBody.gameObject, new Vector3(0.1f, 0.2f, 0.1f), 0.8f);
        });

    }

    #endregion

}
