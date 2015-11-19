using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof (ArtilleryScript))]
public class AIArtilleryControllerScript : MonoBehaviour
{
    private ArtilleryScript arty;

    private vp_Timer.Handle fireTimer;
    private vp_Timer.Handle turnTimer;

    private void Awake()
    {
        arty = GetComponent<ArtilleryScript>();
    }

    private void OnEnable()
    {
        if (!NetworkServer.active) // server only, isServer doesn't work in onEnable yet for some reason
            return;

        fireTimer = new vp_Timer.Handle();
        turnTimer = new vp_Timer.Handle();

        float rand = Random.value;
        vp_Timer.In(3 + rand, arty.CmdFireBullet, 999999999, 3 + rand, fireTimer);
        vp_Timer.In(2 + rand, delegate() { arty.CmdTurn(Random.Range(-45f, 45f)); }, 999999999, 3 + rand, turnTimer);
    }

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnDisable()
    {
        if (!NetworkServer.active) // server only
            return;

        fireTimer.Cancel();
        turnTimer.Cancel();
    }
}