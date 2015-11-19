using System.IO;
using UnityEditor;
using UnityEngine;

public class MultiScene
{
    [MenuItem("File/Combine Scenes")]
    private static void Combine()
    {
        Object[] objects = Selection.objects;

        EditorApplication.SaveCurrentSceneIfUserWantsTo();
        EditorApplication.NewScene();

        foreach (Object item in objects)
        {
            EditorApplication.OpenSceneAdditive(AssetDatabase.GetAssetPath(item));
        }
    }


    [MenuItem("File/Combine Scenes", true)]
    private static bool CanCombine()
    {
        if (Selection.objects.Length < 2)
        {
            return false;
        }

        foreach (Object item in Selection.objects)
        {
            if (!Path.GetExtension(AssetDatabase.GetAssetPath(item)).ToLower().Equals(".unity"))
            {
                return false;
            }
        }

        return true;
    }
}