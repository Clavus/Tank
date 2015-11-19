using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Renderer))]
public class ChangeMaterialSettingsScript : MonoBehaviour
{

    public FloatSetting[] floats;
    public IntSetting[] integers;
    public ColorSetting[] colors;

    [Serializable]
    public struct FloatSetting
    {
        public string name;
        public float floatValue;
    }

    [Serializable]
    public struct IntSetting
    {
        public string name;
        public int intValue;
    }

    [Serializable]
    public struct ColorSetting
    {
        public string name;
        public Color colorValue;
    }

    void Start()
    {
        UpdateMaterial();
    }

    public void UpdateMaterial()
    {
        Renderer renderer = GetComponent<Renderer>();
        Array.ForEach(floats, x => renderer.material.SetFloat(x.name, x.floatValue));
        Array.ForEach(integers, x => renderer.material.SetFloat(x.name, x.intValue));
        Array.ForEach(colors, x => renderer.material.SetColor(x.name, x.colorValue));
    }

    public Material CopyMaterial()
    {
        Renderer renderer = GetComponent<Renderer>();
        Material mat = new Material(renderer.material.shader);
        mat.mainTexture = renderer.material.mainTexture;
        mat.mainTextureOffset = renderer.material.mainTextureOffset;
        mat.mainTextureScale = renderer.material.mainTextureScale;
        return mat;
    }

    public void OverrideMaterial(Material mat)
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.material = mat;
    }

}
