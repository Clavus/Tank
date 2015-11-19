using UnityEditor;
using System.Collections;

public class PathNodeScriptGizmoDrawer {

    // Draw parent path gizmo
    [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
    public static void DrawGizmo(PathNodeScript scr, GizmoType gizmoType)
    {
        PathScript path = scr.transform.GetComponentInParent<PathScript>();
        if (path != null)
            PathScriptGizmoDrawer.DrawGizmo(path, gizmoType);
    }

}
