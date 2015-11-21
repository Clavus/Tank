using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BaseNetworkedPlayerControllerBehaviour : NetworkBehaviour, IPlayer, IDamagable {

    public virtual void TakeDamage(DamageData damage)
    {

    }

}
