using UnityEngine;
using System.Collections;
using System;

public class JumpPadTrigger : EventTrigger
{

    [Header("Launch direction is up vector of transform")]
    public float force = 10f;
    public bool ignoreMass = true;
    public bool ignoreIncomingVelocity = true;
    public float delayBetweenLaunches = 0.1f;

    private float nextLaunch = 0;

    public override void Trigger(GameObject triggeree)
    {
        if (nextLaunch > Time.time)
            return;

        Rigidbody body = triggeree.GetComponent<Rigidbody>();

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
    }
}
