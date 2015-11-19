using UnityEngine;
using UnityEditor;
using System.Collections;

public class PathScriptGizmoDrawer {

    [DrawGizmo(GizmoType.Selected | GizmoType.Active | GizmoType.NonSelected)]
    public static void DrawGizmo(PathScript scr, GizmoType gizmoType)
    {
        Vector3[] nodes = scr.GetPath();

        if (nodes.Length > 1)
            iTween.DrawPathGizmos(nodes);

        for (int i = 0; i < nodes.Length; i++)
        {
            if (scr.loops && i == nodes.Length - 1 && nodes.Length > 1)
                break;

            if (nodes.Length > 1 && scr.loops)
                Gizmos.color = Color.Lerp(Color.red, Color.blue, (float)i / (nodes.Length - 2));
            else if (nodes.Length > 1)
                Gizmos.color = Color.Lerp(Color.red, Color.blue, (float)i / (nodes.Length - 1));
            else
                Gizmos.color = Color.red;

            if ((gizmoType & GizmoType.NonSelected) > 0)
                Gizmos.DrawWireSphere(nodes[i], 0.25f);
            else
                Gizmos.DrawSphere(nodes[i], 0.5f);

            Vector3 screenPoint = Camera.current.WorldToScreenPoint(nodes[i]);
            GUI.TextField(new Rect(screenPoint.x, screenPoint.y, 20, 20), i.ToString());

        }
    }

}
