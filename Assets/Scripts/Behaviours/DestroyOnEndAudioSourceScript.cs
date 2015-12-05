using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class DestroyOnEndAudioSourceScript : MonoBehaviour
{
    private AudioSource sound;

    void Awake()
    {
        sound = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        StartCoroutine("CheckIfPlaying");
    }

    IEnumerator CheckIfPlaying()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (!sound.isPlaying)
            {
                Game.Destroy(gameObject);
                break;
            }
        }
    }
}
