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

    private float curSpeed = 6.7f;
    private float curRotSpeed = 5f;
    private bool isDead = false;
    private int health = 100;
    private SphereCollider seekCollider;
    public GameObject[] patrolPoints;
    private Transform playerTransform;
    private Vector3 destPos;
    private Rigidbody rb;
    public float patrollingRadius = 2f;
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
       
        if (Vector3.Distance(transform.position, destPos) <= patrollingRadius)
        {
            print("Point reached, get next point");
            FindNextPoint();
        }
     
        else if (Vector3.Distance(transform.position, playerTransform.position) <= PlayerNearRange)
        {
            print("player close, change to chase state");
            curState = SharkState.Chase;
        }

        //Rotate to the target point
        Vector3 dir = destPos - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * curRotSpeed);

        //Go Forward
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
