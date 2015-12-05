using UnityEngine;
using System.Collections;

public class ResizeOverTimeTrigger : EventTrigger
{
    public float lerpDuration;
    public Vector3 targetScale;

    private bool resizing = false;
    private float endTime;
    private Vector3 baseScale;

    void Update()
    {
        if (!resizing)
            return;

        if (Time.time < endTime)
            transform.localScale = Vector3.Lerp(baseScale, targetScale, 1 - (endTime - Time.time)/lerpDuration);
        else
            transform.localScale = targetScale;
    }

    public override void Trigger(GameObject triggeree)
    {
        if (resizing)
            return;

        resizing = true;
        baseScale = transform.localScale;
        endTime = Time.time + lerpDuration;
    }

    public override void ResetTrigger()
    {
        transform.localScale = baseScale;
        resizing = false;
    }
}
