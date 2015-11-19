using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class TankNetworkManager : NetworkManager {

    public override void OnStopHost()
    {
        //Debug.Log("Cancelled all timers");
        vp_Timer.CancelAll();
    }

}
