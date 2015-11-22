using UnityEngine;
using System.Collections;

public class DestroyAfterPeriodScript : MonoBehaviour
{

    public float period = 1f;
    [Tooltip("If checked and this game object is registered as a pooled object, the object will not be put back in the pool but instead destroyed properly")]
    public bool dontPool = false;

    private bool isPooled = false;
    private float killTime;

    void OnEnable()
    {
        killTime = Time.time + period;
    }
	
	void Update () {

	    if (killTime <= Time.time)
	    {
            if (isPooled)
	            ObjectPool.Add(gameObject);
	        else
                Destroy(gameObject);
	    }

	}

    // Called when spawned for first time in object pool
    void OnPoolStart()
    {
        if (!dontPool)
            isPooled = true;
    }
}
