using UnityEngine;
using System.Collections;

public class Util
{
	public static Vector3 GetPointOnUnitSphereCap(Quaternion lookRotation, float maxAngle)
    {
        float rad = Random.Range(0, maxAngle) * Mathf.Deg2Rad;
        Vector2 point = Random.insideUnitCircle.normalized * Mathf.Sin(rad);
        return lookRotation * new Vector3(point.x, point.y, Mathf.Cos(rad));
    }
}
