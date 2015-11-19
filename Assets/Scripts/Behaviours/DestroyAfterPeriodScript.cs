using UnityEngine;
using System.Collections;

public class DestroyAfterPeriodScript : MonoBehaviour
{

    public float period = 1f;
    public bool isPooled = false;

    private float killTime;

	// Use this for initialization
	void Start ()
	{
	    
	}

    void OnEnable()
    {
        killTime = Time.time + period;
    }
	
	// Update is called once per frame
	void Update () {

	    if (killTime < Time.time)
	    {
	        if (isPooled)
                ObjectPool.Add(gameObject);
            else
                Destroy(gameObject);
	    }

	}
}
