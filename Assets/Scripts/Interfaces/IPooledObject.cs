using UnityEngine;
using System.Collections;

public interface IPooledObject
{
    /// <summary>
    /// Called when object is added to pool for the first time, before it's activated.
    /// </summary>
    void OnPoolInitialize();

    /// <summary>
    /// Called when the object is spawned from the pool, after it's activated.
    /// </summary>
    void OnPoolSpawn();
}
