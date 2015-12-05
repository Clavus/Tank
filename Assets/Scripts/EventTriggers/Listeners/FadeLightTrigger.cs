using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class FadeLightTrigger : EventTrigger
{

    public float fadeDuration;
    public float targetIntensity;

    private Light l;
    private bool fading = false;
    private float endTime;
    private float baseIntensity;

    void Awake()
    {
        l = GetComponent<Light>();
    }

    void Update()
    {
        if (!fading)
            return;

        float s = (endTime - Time.time)/fadeDuration;

        if (Time.time < endTime)
            l.intensity = baseIntensity*s + targetIntensity*(1-s);
        else
            l.intensity = targetIntensity;
    }

    public override void Trigger(GameObject triggeree)
    {
        if (fading)
            return;

        fading = true;
        baseIntensity = l.intensity;
        endTime = Time.time + fadeDuration;
    }

    public override void ResetTrigger()
    {
        l.intensity = baseIntensity;
        fading = false;
    }

}
