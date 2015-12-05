using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class PlayParticleSystemTrigger : EventTrigger
{
    private ParticleSystem system;

    void Awake()
    {
        system = GetComponent<ParticleSystem>();
    }

    public override void Trigger(GameObject triggeree)
    {
        system.Play();
    }

}
