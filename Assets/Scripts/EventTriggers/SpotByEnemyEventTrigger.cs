using UnityEngine;
using System.Collections;

public class SpotByEnemyEventTrigger : EventTrigger
{

    public BaseEnemyAIControllerBehaviour enemy;

    public override void Trigger(GameObject triggerree)
    {
        enemy.SpotObject(triggerree);
    }

}
