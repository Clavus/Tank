using UnityEngine;
using System.Collections;

public class Effect {

    public static void SpawnOnClient(string effect, Vector3 position) {  SpawnOnClient(effect, position, Quaternion.identity, Vector3.one); }
    public static void SpawnOnClient(string effect, Vector3 position, Quaternion rotation) { SpawnOnClient(effect, position, rotation, Vector3.one); }
    public static void SpawnOnClient(string effect, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        EffectManagerScript.SpawnOnClient(effect, position, rotation, scale);
    }

    public static void ShakeCamera(float power, float time)
    {
        EffectManagerScript.ShakeCamera(power, time);
    }

    public static bool Exists(string name)
    {
        return EffectManagerScript.EffectExists(name);
    }
}
