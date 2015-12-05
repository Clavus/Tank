using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Game
{

    public static GameObject Create(GameObject prefab) { return Create(prefab, Vector3.zero, Quaternion.identity); }
    public static GameObject Create(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (ObjectPool.Contains(prefab))
            return ObjectPool.Get(prefab, position, rotation);
        else
            return (GameObject) Object.Instantiate(prefab, position, rotation);
    }

    public static void Destroy(GameObject obj)
    {
        if (ObjectPool.Contains(obj))
            ObjectPool.Add(obj);
        else
        {
            if (obj.HasComponent<NetworkIdentity>())
                NetworkServer.Destroy(obj);
            else
                Object.Destroy(obj);
        }  
    }

}
