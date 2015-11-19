using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AttackChopperScript))]
public class AIAttackChopperControllerScript : BaseEnemyAIControllerBehaviour
{
    public float speed = 6f;
    public float strafePauseInterval = 3f;
    public float strafeBombInterval = 0.25f;

    private GameObject target;

    private Vector3 strafeDirection;
    private bool isStrafing = false;
    private float nextStrafeEnd = 0;
    private float nextBomb = 0;

    private AttackChopperScript chopper;

    void Awake()
    {
        chopper = GetComponent<AttackChopperScript>();
    }

    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update () {

        if (target != null)
        {
            if (isStrafing)
            {
                chopper.MoveBy(strafeDirection * Time.deltaTime * speed);
                chopper.TurnTo(strafeDirection);

                Vector3 pointDir = target.transform.position - transform.position;
                
                if (isServer && pointDir.magnitude < 12 && nextBomb < Time.time)
                {
                    // Bombs away!
                    chopper.CmdDropBomb();
                    nextBomb = Time.time + strafeBombInterval;
                }

                if (pointDir.magnitude > 14)
                {
                    if (Vector3.Dot(pointDir.normalized, strafeDirection) < -0.1f)
                    {
                        // If we passed the target, pause
                        isStrafing = false;
                        nextStrafeEnd = Time.time + strafePauseInterval;
                    }
                    else // Adjust strafe path if we're far off but still headed towards the target
                        UpdateStrafeDirection();
                }
            }
            else
            {
                UpdateStrafeDirection();
                chopper.TurnTo(strafeDirection);

                if (nextStrafeEnd < Time.time)
                    isStrafing = true;
            }
        }

    }

    void UpdateStrafeDirection()
    {
        strafeDirection = (new Vector3(target.transform.position.x - transform.position.x, 0, target.transform.position.z - transform.position.z)).normalized;
    }

    public override void OnSpotObject(GameObject spotted)
    {
        IPlayer player = spotted.GetComponent(typeof(IPlayer)) as IPlayer;
        if (player == null)
            return;

        target = spotted;
    }

}
