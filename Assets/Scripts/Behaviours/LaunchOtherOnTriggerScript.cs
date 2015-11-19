using UnityEngine;
using System.Collections;
using System;

public class LaunchOtherOnTriggerScript : MonoBehaviour
{
    [Header("Launch direction is up vector of transform")]
    public float force = 10f;
    public bool ignoreMass = true;
    public bool ignoreIncomingVelocity = true;
    public float delayBetweenLaunches = 0.1f;

    public EventTrigger[] eventTriggersOnLaunch;

    private float nextLaunch = 0;

    void OnTriggerEnter(Collider other)
    {
        if (nextLaunch > Time.time)
            return;

        Rigidbody body = other.attachedRigidbody;

        if (body == null)
            return;

        Vector3 launchDirection = transform.up;
        nextLaunch = Time.time + delayBetweenLaunches;
        Vector3 resVelocity = Vector3.zero;

        if (ignoreIncomingVelocity)
        {
            body.velocity = Vector3.ProjectOnPlane(body.velocity, launchDirection);
            //resVelocity = Vector3.ProjectOnPlane(body.velocity, launchDirection);
        }

        body.AddForce(launchDirection * force + resVelocity, ignoreMass ? ForceMode.VelocityChange : ForceMode.Impulse);

        // trigger all events
        Array.ForEach(eventTriggersOnLaunch, x => x.Trigger(body.gameObject));
    }

}
