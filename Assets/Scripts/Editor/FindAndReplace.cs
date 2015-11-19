using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;

public class FindAndReplace : EditorWindow
{
    [MenuItem("Assets/Tools/Find and Replace")]
    private static void OpenWindow()
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

        var win = EditorWindow.GetWindowWithRect<FindAndReplace>(new Rect(100, 100, 500, 300), true, "Find And Replace");
        win.folder = path;
    }


    public string folder;

    public bool renameFiles = true;
    public bool recurse = true;
    public bool replaceText = true;
    public bool ignoreCase = true;

    public string strFind;
    public string strReplace;


    void OnGUI()
    {
        EditorGUILayout.LabelField("Folder", folder);

        GUILayout.Space(5);

        strFind = EditorGUILayout.TextField("Find", strFind);
        strReplace = EditorGUILayout.TextField("Replace", strReplace);

        GUILayout.Space(5);

        renameFiles = EditorGUILayout.Toggle("Rename Files", renameFiles);
        recurse = EditorGUILayout.Toggle("Recurse Into Subfolders", recurse);
        replaceText = EditorGUILayout.Toggle("Replace Text", replaceText);
        ignoreCase = EditorGUILayout.Toggle("Ignore Case", ignoreCase);

        GUILayout.Space(5);

        if (GUILayout.Button("Run"))
        {
            DoActions();
        }
    }

    void DoActions()
    {
        if (renameFiles) DoRenameFiles();

        if (replaceText) DoReplaceText();
    }

    void DoRenameFiles()
    {
        FilesRecursive(folder, recurse, (x) =>
        {
            //if (x.Contains(strFind, ignoreCase ? System.StringComparison.CurrentCultureIgnoreCase : System.StringComparison.CurrentCulture))
            if (Regex.IsMatch(x, strFind, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None))
            {
                var target = x.Replace(folder, "");
                //target = target.Replace(strFind, strReplace, ignoreCase ? System.StringComparison.CurrentCultureIgnoreCase : System.StringComparison.CurrentCulture);
                target = Regex.Replace(target, strFind, strReplace, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);

                FileUtil.MoveFileOrDirectory(x, folder + target);
            }
        });
    }

    public static void FilesRecursive(string folder, bool recurse, System.Action<string> func)
    {
        foreach (var file in System.IO.Directory.GetFiles(folder))
        {
            if (System.IO.Directory.Exists(file))
            {
                if (recurse)
                    FilesRecursive(file, recurse, func);

                continue;
            }

            func(file);
        }

        AssetDatabase.Refresh();
    }

    void DoReplaceText()
    {
        foreach (var obj in AssetDatabase.FindAssets("", new[] { folder }).Select(x => AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(x))))
        {
            ReplaceText(obj);

            if (obj is GameObject)
            {
                foreach (var c in (obj as GameObject).GetComponentsInChildren<Component>(true))
                {
                    ReplaceText(c);
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    //
    // Replace any references to files in Source with Target (same filenames etc)
    //
    public void ReplaceText(Object obj)
    {
        SerializedObject so = new SerializedObject(obj);
        var sp = so.GetIterator();

        while (sp.Next(true))
        {
            if (sp.propertyType == SerializedPropertyType.String)
            {
                var str = sp.stringValue;
                //if (str.Contains(strFind, ignoreCase ? System.Globalization.CompareOptions.IgnoreCase : System.Globalization.CompareOptions.None))
                if (Regex.IsMatch(str, strFind, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None))
                {
                    //sp.stringValue = str.Replace(strFind, strReplace, ignoreCase ? System.StringComparison.CurrentCultureIgnoreCase : System.StringComparison.CurrentCulture);
                    sp.stringValue = Regex.Replace(str, strFind, strReplace, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
                }
            }
        }

        so.ApplyModifiedProperties();
    }

}