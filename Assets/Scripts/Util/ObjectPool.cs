using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

// Taken and improved from: http://forum.unity3d.com/threads/simple-reusable-object-pool-help-limit-your-instantiations.76851/

/// <summary>
///     Repository of commonly used prefabs.
/// </summary>
[AddComponentMenu("Gameplay/ObjectPool")]
public class ObjectPool : SingletonComponent<ObjectPool>
{
    #region member

    /// <summary>
    ///     Member class for a prefab entered into the object pool
    /// </summary>
    [Serializable]
    public class ObjectPoolEntry
    {
        /// <summary>
        ///     The object to pre-instantiate
        /// </summary>
        [SerializeField]
        public GameObject Prefab;

        /// <summary>
        ///     Amount of object to pre-instantiate at start
        /// </summary>
        [SerializeField]
        public int StartBufferSize;

        /// <summary>
        ///     The object pool
        /// </summary>
        [HideInInspector]
        public Queue<GameObject> Pool;
    }

    #endregion

    /// <summary>
    ///     The object prefabs which the pool can handle.
    /// </summary>
    public ObjectPoolEntry[] pooledObjects;

    // The faster lookup dictionary
    private Dictionary<string, ObjectPoolEntry> entries;

    void Start()
    {
        // Loop through the object prefabs and make a new queue for each one.
        // Store everything in the entries dictionary for faster lookup.
        entries = new Dictionary<string, ObjectPoolEntry>();

        for (int i = 0; i < pooledObjects.Length; i++)
        {
            ObjectPoolEntry pooledObj = pooledObjects[i];
            if (pooledObj.Prefab == null)
                continue;

            string pname = pooledObj.Prefab.name;
            if (entries.ContainsKey(pname))
            {
                Debug.LogError("Object pool contains multiple entries for name '" + pname + "'");
                continue;
            }

            // Create the repository
            pooledObj.Pool = new Queue<GameObject>();

            entries.Add(pname, pooledObj);

            // Fill the buffer
            for (int n = 0; n < pooledObj.StartBufferSize; n++)
            {
                pooledObj.Prefab.SetActive(false); // deactivate before spawn
                GameObject newObj = Instantiate(pooledObj.Prefab);
                newObj.name = pooledObj.Prefab.name;

                IPooledObject[] poolScripts = newObj.GetComponentsInChildren(typeof(IPooledObject), true).Cast<IPooledObject>().ToArray();
                Array.ForEach(poolScripts, o => o.OnPoolInitialize());

                AddInternal(newObj);
            }
        }
    }

    /// <summary>
    ///     Gets a new object for the name type provided.  If no object type exists or if onlypooled is true and there is no
    ///     objects of that type in the pool
    ///     then null will be returned.
    /// </summary>
    /// <returns>
    ///     The object for type.
    /// </returns>
    /// <param name='objectType'>
    ///     Object type.
    /// </param>
    /// <param name='onlyPooled'>
    ///     If true, it will only return an object if there is one currently pooled.
    /// </param>
    /// <param name='position'>
    ///     New position of the object.
    /// </param>
    /// <param name='rotation'>
    ///     New rotation of the object.
    /// </param>
    public static GameObject Get(string objectType, bool onlyPooled, Vector3 position, Quaternion rotation)
    {
        return instance.GetInternal(objectType, onlyPooled, position, rotation);
    }

    public static GameObject Get(GameObject obj, bool onlyPooled, Vector3 position, Quaternion rotation)
    {
        return instance.GetInternal(obj.name, onlyPooled, position, rotation);
    }

    public static GameObject Get(Component obj, bool onlyPooled, Vector3 position, Quaternion rotation)
    {
        return instance.GetInternal(obj.name, onlyPooled, position, rotation);
    }

    public static GameObject Get(string objectType, Vector3 position, Quaternion rotation)
    {
        return instance.GetInternal(objectType, false, position, rotation);
    }

    public static GameObject Get(GameObject obj, Vector3 position, Quaternion rotation)
    {
        return instance.GetInternal(obj.name, false, position, rotation);
    }

    public static GameObject Get(Component obj, Vector3 position, Quaternion rotation)
    {
        return instance.GetInternal(obj.name, false, position, rotation);
    }

    public static GameObject Get(string objectType)
    {
        return instance.GetInternal(objectType, false, Vector3.zero, Quaternion.identity);
    }

    public static GameObject Get(GameObject obj)
    {
        return instance.GetInternal(obj.name, false, Vector3.zero, Quaternion.identity);
    }

    public static GameObject Get(Component obj)
    {
        return instance.GetInternal(obj.name, false, Vector3.zero, Quaternion.identity);
    }

    private GameObject GetInternal(string objectType, bool onlyPooled, Vector3 position, Quaternion rotation)
    {
        ObjectPoolEntry pooledObj;

        if (!entries.TryGetValue(objectType, out pooledObj))
        {
            Debug.LogError("'" + objectType + "' is not a pooled object!");
            return null;
        }

        GameObject prefab = pooledObj.Prefab;
        Queue<GameObject> pool = pooledObj.Pool;

        if (pool.Count > 0)
        {
            GameObject pooledObject = pool.Dequeue();
            //pooledObject.transform.parent = null;
            pooledObject.transform.position = position;
            pooledObject.transform.rotation = rotation;
            pooledObject.SetActive(true);

            IPooledObject[] poolScripts = pooledObject.GetComponentsInChildren(typeof(IPooledObject), true).Cast<IPooledObject>().ToArray();
            Array.ForEach(poolScripts, o => o.OnPoolSpawn());
            return pooledObject;
        }
        if (!onlyPooled)
        {
            GameObject newObj = (GameObject)Instantiate(prefab, position, rotation);
            newObj.name = prefab.name;
            newObj.transform.parent = transform;

            IPooledObject[] poolScripts = newObj.GetComponentsInChildren(typeof(IPooledObject), true).Cast<IPooledObject>().ToArray();
            Array.ForEach(poolScripts, o => o.OnPoolInitialize());
            newObj.SetActive(true);
            Array.ForEach(poolScripts, o => o.OnPoolSpawn());

            return newObj;
        }

        // Only returned if onlyPooled is set to true and we ran out of pooled objects
        return null;
    }

    /// <summary>
    ///     Pools the object specified.
    /// </summary>
    /// <param name='obj'>
    ///     Object to be pooled.
    /// </param>
    public static void Add(GameObject obj)
    {
        instance.AddInternal(obj);
    }

    public static void Add(Component obj)
    {
        instance.AddInternal(obj.gameObject);
    }

    private void AddInternal(GameObject obj)
    {
        ObjectPoolEntry pooledObj;

        if (!entries.TryGetValue(obj.name, out pooledObj))
        {
            Debug.LogWarning("Trying to add non-pooled object '" + obj.name + "' to pool, destroying instead...");
            Destroy(obj);
            return;
        }

        Queue<GameObject> pool = pooledObj.Pool;
        obj.SetActive(false);
        obj.transform.parent = transform;
        pool.Enqueue(obj);
    }

    /// <summary>
    ///     Checks if object pool has this specific object pooled.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool Contains(GameObject obj)
    {
        return instance.ContainsInternal(obj);
    }

    public static bool Contains(Component obj)
    {
        return instance.ContainsInternal(obj.gameObject);
    }

    private bool ContainsInternal(GameObject obj)
    {
        return entries.ContainsKey(obj.name);
    }

}