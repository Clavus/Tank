using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.VR;

public class EffectManagerScript : NetworkBehaviour
{

    public static EffectManagerScript instance;

    public SpawnableEffect[] registeredEffects;

    [Serializable]
    public struct SpawnableEffect
    {
        public string name;
        public GameObject effect;
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

	void Start ()
	{

    }
	
	void Update () {
	
	}

    GameObject GetEffectByName(string name)
    {
        foreach (SpawnableEffect e in registeredEffects)
        {
            if (e.name == name)
                return e.effect;
        }
        return null;
    }

    public static bool EffectExists(string name)
    {
        return instance.GetEffectByName(name) != null;
    }

    public static void SpawnOnClient(string effect, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        instance.RpcSpawnOnClient(effect, position, rotation, scale);
    }

    [ClientRpc]
    void RpcSpawnOnClient(string effect, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        GameObject effectObj = GetEffectByName(effect);
        if (effectObj == null)
        {
            Debug.LogError("Effect '" + effect + "' is not registered and cannot be spawned!");
            return;
        }

        GameObject spawned = (GameObject)Instantiate(effectObj, position, rotation);
        spawned.transform.localScale = scale;
    }

    public static void ShakeCamera(float power, float time)
    {
        instance.RpcShakeCamera(power, time);
    }

    [ClientRpc]
    void RpcShakeCamera(float power, float time)
    {
        // iTween only shakes world coord, and doesn't reset it
        //iTween.ShakePosition(CameraManagerScript.GetActiveCamera(), power, time);

        // no shaking in VR
        if (VRSettings.enabled)
            return;

        ShakePositionScript shake = CameraManagerScript.GetActiveCamera().AddComponent<ShakePositionScript>();
        shake.decreaseAmplitudeOverDuration = true;
        shake.amplitude = power;
        shake.duration = time;
        shake.Trigger();
    }
}
