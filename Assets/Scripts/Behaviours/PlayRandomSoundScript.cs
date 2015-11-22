using UnityEngine;
using System.Collections;

public class PlayRandomSoundScript : MonoBehaviour
{

    public AudioSource[] sources;
    
	void OnEnable()
	{
	    if (sources != null && sources.Length > 0)
	    {
            AudioSource pick = sources[Random.Range(0, sources.Length)];
            if (ObjectPool.Contains(pick))
                ObjectPool.Get(pick, transform.position, Quaternion.identity).GetComponent<AudioSource>().Play();
            else
                ((AudioSource) Instantiate(pick)).Play();
        }
	}
	
}
