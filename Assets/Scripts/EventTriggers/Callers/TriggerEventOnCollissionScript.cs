using UnityEngine;
using System;
using System.Collections;

public class TriggerEventOnCollissionScript : MonoBehaviour
{
    [Tooltip("Passes collider.attachedRigidbody.gameObject if available, otherwise collider.gameObject")]
    public bool passRigidbodyObjectAsTriggeree = true;

    public EventTrigger[] eventTriggersOnEnter;
    public EventTrigger[] eventTriggersOnStay;
    public EventTrigger[] eventTriggersOnLeave;

    void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null && passRigidbodyObjectAsTriggeree)
            Array.ForEach(eventTriggersOnEnter, x => x.Trigger(other.attachedRigidbody.gameObject));
        else
            Array.ForEach(eventTriggersOnEnter, x => x.Trigger(other.gameObject));
    }

    void OnTriggerStay(Collider other)
    {
        if (other.attachedRigidbody != null && passRigidbodyObjectAsTriggeree)
            Array.ForEach(eventTriggersOnStay, x => x.Trigger(other.attachedRigidbody.gameObject));
        else
            Array.ForEach(eventTriggersOnStay, x => x.Trigger(other.gameObject));
    }

    void OnTriggerLeave(Collider other)
    {
        if (other.attachedRigidbody != null && passRigidbodyObjectAsTriggeree)
            Array.ForEach(eventTriggersOnLeave, x => x.Trigger(other.attachedRigidbody.gameObject));
        else
            Array.ForEach(eventTriggersOnLeave, x => x.Trigger(other.gameObject));
    }
}
