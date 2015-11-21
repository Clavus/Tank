using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class AttackChopperScript : NetworkBehaviour, IDamagable
{

    public Transform rotor;
    public Transform vehicleBody;
    public Transform bombPrefab;
    public Transform bombSpawn;
    [Tooltip("If set to -1, sets itself to the current transform y")]
    public float flyHeight = -1;

    private Quaternion lookDir;
    private float turnRate = 90f;

    private Rigidbody body;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    // Use this for initialization
    void Start ()
    {
        lookDir = body.rotation;

        if (flyHeight == -1)
            flyHeight = transform.position.y;
    }

    void Update()
    {
        rotor.Rotate(Vector3.up, Time.deltaTime * 3600);
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        if (!isServer) // server only
            return;

        Vector3 flyVector = body.position;
        flyVector.y = Mathf.MoveTowards(flyVector.y, flyHeight + Mathf.Sin(Time.time), 2f * Time.deltaTime);
        body.MovePosition(flyVector);
        body.MoveRotation(Quaternion.RotateTowards(body.rotation, lookDir, turnRate * Time.deltaTime));
    }

    public void MoveBy(Vector3 delta)
    {
        body.MovePosition(body.position + delta);
    }

    public void TurnTo(Vector3 look)
    {
        lookDir = Quaternion.LookRotation(look);
    }

    #region SERVER

    [Command]
    public void CmdDropBomb()
    {
        GameObject bullet = (GameObject)Instantiate(bombPrefab.gameObject, bombSpawn.position, bombSpawn.rotation);
        bullet.GetComponent<BulletScript>().SetOwner(gameObject);
        NetworkServer.Spawn(bullet);
    }

    public void TakeDamage(DamageData damage)
    {
        RpcPunchVehicle();
    }

    #endregion

    #region CLIENT

    [ClientRpc]
    void RpcPunchVehicle()
    {
        iTween.Stop(vehicleBody.gameObject);
        vehicleBody.localScale = Vector3.one;

        // delay by a frame, can't seem to trigger tweens right after stopping them.
        vp_Timer.In(0, delegate () { // todo: proper null check in case the vehicle is destroyed
            iTween.PunchScale(vehicleBody.gameObject, new Vector3(0.1f, 0.2f, 0.1f), 0.8f);
        });

    }

    #endregion

}
