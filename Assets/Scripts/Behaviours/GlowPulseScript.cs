using UnityEngine;
using System.Collections;

public class GlowPulseScript : MonoBehaviour
{

    public Color pulseColor = Color.white;
    public float period = 2;

    [Range(0.0f, 1.0f)]
    public float pulseRangeMin = 0;

    [Range(0.0f, 1.0f)]
    public float pulseRangeMax = 1;

    public bool useCurveInsteadOfSinus = false;
    public AnimationCurve curve = DefaultCurve();

    [HideInInspector]
    public float fraction;

    private Renderer myRenderer;
    private Light myLight;
    private float pulseTime = 0;
    private float baseGain = 0;
    private float baseIntensity = 0;

    void Awake()
    {
        myRenderer = GetComponent<Renderer>();
        myLight = GetComponent<Light>();
    }

	void OnEnable ()
	{
	    pulseTime = 0;

	    if (myRenderer != null)
	        baseGain = myRenderer.material.GetFloat("_EmissionGain");

        if (myLight != null)
	        baseIntensity = myLight.intensity;
	}

	void Update ()
	{
        UpdateLightColor();

	    if (period > 0)
	    {
	        if (useCurveInsteadOfSinus)
	        {
                pulseTime += Time.deltaTime / period;
	            fraction = pulseRangeMin + (pulseRangeMax - pulseRangeMin) * Mathf.Clamp01(curve.Evaluate(pulseTime));
	        }
            else
	        {
                pulseTime += Time.deltaTime * Mathf.PI / period;
                fraction = Mathf.Clamp01(pulseRangeMin + (pulseRangeMax - pulseRangeMin) * Mathf.Pow(Mathf.Sin(pulseTime), 2.0f));
            }
        }

        if (myRenderer != null)
            myRenderer.material.SetFloat("_EmissionGain", baseGain * fraction);

        if (myLight != null)
	        myLight.intensity = baseIntensity*fraction; //myLight.color = curColor;
	}

    void UpdateLightColor()
    {
        if (myLight != null)
            myLight.color = pulseColor;

        if (myRenderer != null)
            myRenderer.material.SetColor("_EmissionColor", pulseColor);
    }

    private static AnimationCurve DefaultCurve()
    {
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0, 0);
        curve.AddKey(0.5f, 1);
        curve.AddKey(1, 0);
        curve.preWrapMode = WrapMode.Loop;
        curve.postWrapMode = WrapMode.Loop;
        return curve;
    }
}
