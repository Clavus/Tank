﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[RequireComponent(typeof(TankScript))]
public class PlayerTankControllerScript : BaseNetworkedPlayerControllerBehaviour
{

    public float speed = 4;
    public GameObject damageSound;

    private TankScript tank;

    void Awake()
    {
        tank = GetComponent<TankScript>();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    private void Update()
    {

        if (isLocalPlayer)
        {
            if (Input.GetButtonDown("Turn Left"))
                tank.CmdTurn(true);

            if (Input.GetButtonDown("Turn Right"))
                tank.CmdTurn(false);

            if (Input.GetButtonDown("Fire1"))
                tank.CmdFireBullet();

            if (Input.GetButtonDown("Fire2"))
                tank.CmdFireFlare();
        }

    }

    private void FixedUpdate()
    {
        if (isLocalPlayer)
            tank.MoveBy(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Time.deltaTime * speed, 4, 0.5f);
    }

    #region SERVER

    public override void TakeDamage(DamageData damage)
    {
        RpcTookDamage();
    }

    #endregion

    #region CLIENT

    [ClientRpc]
    void RpcTookDamage()
    {
        if (damageSound != null)
            ObjectPool.Get(damageSound).GetComponent<AudioSource>().Play();
    }

    #endregion

}
