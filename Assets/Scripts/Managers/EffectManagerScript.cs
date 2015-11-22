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

        [HideInInspector]
        public bool isPooled;
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

	void Start ()
	{
        // Check which effects are pooled, so we can spawn them more efficiently
        for (int i = 0; i < registeredEffects.Length; i++)
            if (ObjectPool.Contains(registeredEffects[i].effect))
                registeredEffects[i].isPooled = true;
    }
	
	void Update () {
	
	}

    SpawnableEffect GetEffectByName(string name)
    {
        foreach (SpawnableEffect e in registeredEffects)
        {
            if (e.name == name)
                return e;
        }
        return new SpawnableEffect();
    }

    public static bool EffectExists(string name)
    {
        return instance.GetEffectByName(name).effect != null;
    }

    public static void SpawnOnClient(string effect, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        instance.RpcSpawnOnClient(effect, position, rotation, scale);
    }

    [ClientRpc]
    void RpcSpawnOnClient(string effect, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        SpawnableEffect sEffect = GetEffectByName(effect);
        if (sEffect.effect == null)
        {
            Debug.LogError("Effect '" + effect + "' is not registered and cannot be spawned!");
            return;
        }

        GameObject spawned;
        if (sEffect.isPooled)
            spawned = ObjectPool.Get(sEffect.effect.name, position, rotation);
        else
            spawned = (GameObject)Instantiate(sEffect.effect, position, rotation);
        
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
