using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class DestroyOnEndParticleSystemScript : MonoBehaviour, IPooledObject
{

    [Tooltip("If checked and this game object is registered as a pooled object, the object will not be put back in the pool but instead destroyed properly")]
    public bool dontPool = false;

    private bool isPooled = false;
    private ParticleSystem system;

    void Awake()
    {
        system = GetComponent<ParticleSystem>();
    }

    void OnEnable()
    {
        StartCoroutine("CheckIfAlive");
    }

    IEnumerator CheckIfAlive()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (!system.IsAlive(true))
            {
                if (isPooled)
                    ObjectPool.Add(gameObject);
                else
                    Destroy(gameObject);

                break;
            }
        }
    }

    public void OnPoolInitialize()
    {
        if (!dontPool)
            isPooled = true;
    }

    public void OnPoolSpawn()
    {
        
    }
}
