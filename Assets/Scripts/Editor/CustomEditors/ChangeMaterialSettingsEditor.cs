using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ChangeMaterialSettingsScript))]
public class ChangeMaterialSettingsEditor : Editor
{


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Apply material settings"))
        {
            ChangeMaterialSettingsScript matSettings = (ChangeMaterialSettingsScript)target;
            matSettings.OverrideMaterial(matSettings.CopyMaterial());
            matSettings.UpdateMaterial();
        }
    }

}
