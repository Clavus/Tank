using UnityEngine;
using System;
using System.Collections;

public class BlinkColorScript : MonoBehaviour
{

    public Color color = Color.red;
    public float colorOnTime = 1;
    public float colorOffTime = 1;

    private Renderer[] renderers;
    private Color[] originalColors;
    private bool[] skipColor;

    private bool active = false;
    private bool colorToggle = false;
    private float nextToggleTime = 0;
    private float stopTime = 0;
    private bool infinite = false;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];
        skipColor = new bool[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
                originalColors[i] = renderers[i].material.color;
            else
                skipColor[i] = true;
        }
            
    }
    
	void Start () {
	
	}
	
	void Update () {

	    if (active)
	    {
	        if (Time.time >= nextToggleTime)
	        {
                ToggleColor();
	            if (colorToggle)
	                nextToggleTime = Time.time + colorOnTime;
	            else
	                nextToggleTime = Time.time + colorOffTime;
	        }
	            
	        if (Time.time >= stopTime && !infinite)
                StopBlinking();
	    }


	}

    /// <summary>
    /// Blink the color for the set duration. Overrides previously set durations.
    /// </summary>
    /// <param name="duration"></param>
    public void Blink(float duration = 5f)
    {
        if (duration == 0)
        {
            StopBlinking();
            return;
        }

        infinite = false;
        stopTime = Time.time + duration;

        if (!active)
        {
            colorToggle = true;
            active = true;
            SetColorOn();
            nextToggleTime = Time.time + colorOnTime;
        }
    }

    /// <summary>
    /// Blink until StopBlinking is called.
    /// </summary>
    public void BlinkLoop()
    {
        infinite = true;
        stopTime = 0;

        if (!active)
        {
            colorToggle = true;
            SetColorOn();
            nextToggleTime = Time.time + colorOnTime;
            active = true;
        }
    }

    /// <summary>
    /// Stop blinking
    /// </summary>
    public void StopBlinking()
    {
        infinite = false;
        active = false;
        SetColorOff();
    }

    private void ToggleColor()
    {
        colorToggle = !colorToggle;

        if (colorToggle)
            SetColorOn();
        else
            SetColorOff();
    }

    private void SetColorOn()
    {
        for (int i = 0; i < renderers.Length; i++)
            if (!skipColor[i])
                renderers[i].material.color = color;
    }

    private void SetColorOff()
    {
        for (int i = 0; i < renderers.Length; i++)
            if (!skipColor[i])
                renderers[i].material.color = originalColors[i];
    }
}
