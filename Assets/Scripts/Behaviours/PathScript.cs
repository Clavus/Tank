using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;

[ExecuteInEditMode]
public class PathScript : MonoBehaviour
{
    public bool loops = true;

    private PathNodeScript[] pathNodes;
    private Vector3[] path;

    void Awake()
    {
        UpdatePath();
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isEditor)
            UpdatePath();
    }

    public void UpdatePath()
    {
        pathNodes = transform.GetComponentsInChildren<PathNodeScript>();
        List<Vector3> pathList = new List<Vector3>();

        foreach (PathNodeScript node in pathNodes)
        {
            pathList.Add(node.transform.position);
        }

        if (pathList.Count > 1 && loops) // on loop, add first node as last node
            pathList.Add(pathList[0]);

        path = pathList.ToArray();
    }

    public Vector3[] GetPath()
    {
        return path;
    }

}
