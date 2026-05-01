using System.Timers;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class SharkFSM : FSM
{
    public enum SharkState
    {
        Patrol,
        Chase,
        Attack,
        Flee,
        Dead,
    }

    public SharkState curState = SharkState.Patrol;

    private float curSpeed = 5f;
    private float curRotSpeed = 2f;
    private bool isDead = false;
    private int health = 100;
    private SphereCollider seekCollider;
    public GameObject[] patrolPoints;
    private Transform playerTransform;
    private Vector3 destPos;
    private Rigidbody rb;
    public float patrollingRadius = 20f;
    public float attackRange = 5f;
    public float PlayerNearRange = 15f;


    protected override void Initialize()
    {
        // Get and set patrol points
        patrolPoints = GameObject.FindGameObjectsWithTag("PatrolPoint");
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            FindNextPoint();
        }
        else
        {
            Debug.LogWarning("No patrol points found");
        }

        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        if (objPlayer != null)
        {
            playerTransform = objPlayer.transform;
        }
        else
        {
            playerTransform = null;
            Debug.LogWarning("No player found");
        }

        rb = GetComponent<Rigidbody>();
    }

    protected override void FSMUpdate()
    {
        switch (curState)
        {
            case SharkState.Patrol:
                UpdatePatrolState();
                break;
            //case SharkState.Chase:
            //    UpdateChaseState();
            //    break;
            //case SharkState.Attack:
            //    UpdateAttackState();
            //    break;
            //case SharkState.Flee:
            //    UpdateDeadState();
            //    break;
            //case SharkState.Dead:
            //    UpdateDeadState();
            //    break;
        }

        if (health <= 0)
            curState = SharkState.Dead;
    }


    protected void UpdatePatrolState()
    {
        // Find another random patrol point if the current point is reached
        if (patrolPoints != null && patrolPoints.Length > 0 && Vector3.Distance(transform.position, destPos) <= patrollingRadius)
        {
            Debug.Log("Patrol point reached, finding next");
            FindNextPoint();
        }
        // Check the distance with player tank
        // When the distance is near, transition to chase state (only if playerTransform exists)
        else if (playerTransform != null && Vector3.Distance(transform.position, playerTransform.position) <= PlayerNearRange)
        {
            print("Player in range, switching to chase state");
            curState = SharkState.Chase;
        }

        // Rotation
        Vector3 currentDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(-currentDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * curRotSpeed);

        transform.Translate(Vector3.forward * Time.deltaTime * curSpeed);
    }

    protected void FindNextPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogWarning("no patrol points found");
            return;
        }

        int randomIndex = Random.Range(0, patrolPoints.Length);
        Vector3 offsetVector = new Vector3(Random.Range(0, 10), Random.Range(0, 10), Random.Range(0, 10));
        destPos = patrolPoints[randomIndex].transform.position + offsetVector;
    }
}
