using UnityEngine;
using System.Collections;

public class PrefabManagerScript : SingletonComponent<PrefabManagerScript>
{
    [Header("Prefabs referenced in non-monobehaviour scripts and such")]
    public ExplosionSystem explosionSystem;
}
