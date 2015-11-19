using UnityEngine;
using System.Collections;

public class DisableOnSpawnScript : MonoBehaviour {

    void Awake()
    {
        gameObject.SetActive(false);    
    }

}
