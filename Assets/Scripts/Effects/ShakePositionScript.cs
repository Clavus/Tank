using UnityEngine;
using System.Collections;

public class ShakePositionScript : MonoBehaviour
{

    public Transform target;
    public float duration = 1f;
    public float amplitude = 0.2f;
    public bool decreaseAmplitudeOverDuration = false;
    public Space shakeSpace = Space.Self;

    private bool isShaking = false;
    private float shakeStart = -99f;
    private Vector3 origPosition;

    void Awake()
    {
        if (target == null)
            target = transform;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	    if (isShaking)
        {
            if (Time.time < shakeStart + duration)
            {
                float pow = amplitude;
                if (decreaseAmplitudeOverDuration)
                    pow *= 1f - ((Time.time - shakeStart) / duration);

                if (shakeSpace == Space.Self)
                    target.localPosition = origPosition + Random.insideUnitSphere*pow;
                else
                    target.position = origPosition + Random.insideUnitSphere*pow;
            }
            else
            {
                if (shakeSpace == Space.Self)
                    target.localPosition = origPosition;
                else
                    target.position = origPosition;

                Destroy(this);
            }
        }
	    

	}

    public void Trigger()
    {
        shakeStart = Time.time;
        isShaking = true;
        if (shakeSpace == Space.Self)
            origPosition = target.localPosition;
        else
            origPosition = target.position;
    }

}
