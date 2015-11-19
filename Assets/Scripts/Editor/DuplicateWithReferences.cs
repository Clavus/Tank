using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;

public class DuplicateWithReferences : MonoBehaviour
{
    [MenuItem("Assets/Tools/Duplicate With References")]
    private static void DoRun()
    {
        var path = "";

        // Try to work out what folder we're clicking on. This code is from google.
        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (System.IO.File.Exists(path))
            {
                path = System.IO.Path.GetDirectoryName(path);
                break;
            }
        }

        // Not a directory - bail
        if (!System.IO.Directory.Exists(path))
            return;

        // Create a "<name> 1" path
        var targetPath = AssetDatabase.GenerateUniqueAssetPath(path);

        // Copy the folder
        AssetDatabase.CopyAsset(path, targetPath);
        AssetDatabase.Refresh();

        // For each asset in the folder
        // try to change links that existed in the previous folder
        // with items of the same name in this folder
        foreach (var obj in AssetDatabase.FindAssets("", new[] { targetPath }).Select(x => AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(x))))
        {
            JoinReferences(path, targetPath, obj);

            if (obj is GameObject)
            {
                foreach (var c in (obj as GameObject).GetComponentsInChildren<Component>(true))
                {
                    JoinReferences(path, targetPath, c);
                }
            }
        }

        //
        // Save that shit
        //
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static void FilesRecursive(string folder, System.Action<string> func)
    {
        foreach (var file in System.IO.Directory.GetFiles(folder))
        {
            if (System.IO.Directory.Exists(file))
            {
                FilesRecursive(file, func);
                continue;
            }

            func(file);
        }
    }

    //
    // Replace any references to files in Source with Target (same filenames etc)
    //
    public static void JoinReferences(string strSource, string strTarget, Object obj)
    {
        SerializedObject so = new SerializedObject(obj);
        var sp = so.GetIterator();

        while (sp.Next(true))
        {
            if (sp.propertyType != SerializedPropertyType.ObjectReference) continue;
            if (sp.objectReferenceValue == null) continue;

            var oldPath = AssetDatabase.GetAssetPath(sp.objectReferenceValue);
            if (!oldPath.StartsWith(strSource, System.StringComparison.InvariantCultureIgnoreCase)) continue;

            var newPath = oldPath.Replace(strSource, strTarget);
            var newObj = AssetDatabase.LoadAssetAtPath(newPath, sp.objectReferenceValue.GetType());
            if (newObj == null) continue;

            sp.objectReferenceValue = newObj;
        }

        so.ApplyModifiedProperties();
    }

}