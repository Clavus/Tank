using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TankScript))]
public class AITankControllerScript : BaseEnemyAIControllerBehaviour
{

    public PathScript path;
    public float speed = 4f;

    private Vector3[] pathNodes;
    private float pathLength;
    private float pathCompletionTime;

    private TankScript tank;
    private Vector3 pathTargetPoint;
    private float pathProgressTime;

    void Awake()
    {
        tank = GetComponent<TankScript>();
    }

	// Use this for initialization
	void Start () {

	    if (path != null)
	    {
            pathNodes = path.GetPath();
	        pathLength = iTween.PathLength(pathNodes);
	        pathCompletionTime = pathLength/speed;

            pathTargetPoint = iTween.PointOnPath(pathNodes, 0);
        }

	}
	
	// Update is called once per frame
	void FixedUpdate () {

	    if (path != null)
	    {
            Vector3 interval = pathTargetPoint - transform.position;
	        interval.y = 0;
	        if (interval.magnitude <= speed * Time.deltaTime * 2)
	        {
                pathProgressTime += Time.deltaTime;
                pathTargetPoint = iTween.PointOnPath(pathNodes, (pathProgressTime % pathCompletionTime) / pathCompletionTime);
            }
                
            Vector3 step = Vector3.ClampMagnitude(interval, speed * Time.deltaTime);
            tank.MoveBy(step, 4, 0.5f);
        }  

    }

    public override void OnSpotObject(GameObject spotted)
    {

    }
}
