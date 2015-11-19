using UnityEngine;
using System.Collections;

public class JumpPadEventTrigger : EventTrigger
{

    public ParticleSystem playParticleSystem;
    public GlowPulseScript jumpPadGlowBit;

    public override void Trigger(GameObject triggeree)
    {
        if (playParticleSystem != null)
        {
            if (jumpPadGlowBit != null)
                playParticleSystem.GetComponent<Renderer>().material.SetColor("_TintColor", jumpPadGlowBit.pulseColor);

            playParticleSystem.Play();
        }
            
    }
}
