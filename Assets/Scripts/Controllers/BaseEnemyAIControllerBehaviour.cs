using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BaseEnemyAIControllerBehaviour : NetworkBehaviour, IEnemy {

    public virtual void OnSpotObject(GameObject spotted)
    {
        
    }
}
