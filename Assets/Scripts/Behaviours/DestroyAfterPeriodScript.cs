using UnityEngine;
using System.Collections;

public class DestroyAfterPeriodScript : MonoBehaviour
{

    public float period = 1f;

    private float killTime;

    void OnEnable()
    {
        killTime = Time.time + period;
    }
	
	void Update () {

	    if (killTime <= Time.time)
	        Destroy(gameObject);

	}

}
