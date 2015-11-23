using UnityEngine;
using System;
using UnityEngine.Networking;

public class FlareScript : NetworkBehaviour
{
    public float durationUntilFade = 7f;
    public float fadeDuration = 3f;
    public EventTrigger[] fadeEventTriggers;

    private float endTime;

    void Awake()
    {
        
    }

	void Start()
	{
        vp_Timer.In(durationUntilFade, delegate { Array.ForEach(fadeEventTriggers, t => t.Trigger(gameObject)); });
	    endTime = Time.time + durationUntilFade + fadeDuration;
	}
	
	void Update()
	{
	    if (isServer && endTime <= Time.time)
            NetworkServer.Destroy(gameObject);
    }
}
