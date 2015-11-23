using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class ScaleParticleRateTrigger : EventTrigger
{
    public float lerpDuration;
    public float emmissionRateScalar = 0f;

    private bool scaling = false;
    private float endTime;
    private float baseRate;
    private float targetRate;
    private ParticleSystem system;

    void Awake()
    {
        system = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (!scaling)
            return;

        float s = (endTime - Time.time) / lerpDuration;

        if (Time.time < endTime)
            system.emissionRate = baseRate*s + targetRate * (1 - s);
        else
            system.emissionRate = targetRate;
    }

    public override void Trigger(GameObject triggeree)
    {
        if (scaling)
            return;

        scaling = true;
        baseRate = system.emissionRate;
        targetRate = baseRate*emmissionRateScalar;
        endTime = Time.time + lerpDuration;
    }

    public override void ResetTrigger()
    {
        system.emissionRate = baseRate;
        scaling = false;
    }
}
