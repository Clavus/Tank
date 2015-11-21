using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BaseEnemyAIControllerBehaviour : NetworkBehaviour, IEnemy, IDamagable {

    public virtual void SpotObject(GameObject spotted)
    {
        
    }

    public virtual void TakeDamage(DamageData damage)
    {

    }

}
