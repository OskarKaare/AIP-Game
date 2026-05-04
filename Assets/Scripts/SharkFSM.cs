using System.Collections;
using System.Timers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class SharkFSM : FSM
{
    public enum SharkState
    {
        Patrol,
        Chase,
        Cooldown,
        Dead,
    }

    public SharkState curState = SharkState.Patrol;

    private float curSpeed;
    private float curRotSpeed = 5f;
    //private bool isDead = false;
    private int health = 100;
    //private SphereCollider seekCollider;
    public GameObject[] patrolPoints;
    private Transform playerTransform;
    private Vector3 destPos;
    private Rigidbody rb;
    public float patrollingRadius = 2f;
    public float chaseRange = 100f;
    public float attackRange = 10f;
    private Animator animator;
    private PlayerStats playerStats;
    private int cooldownValue = 7;


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

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        playerStats = playerObj.GetComponent<PlayerStats>();
        playerTransform = playerObj.transform;
        animator = GetComponentInChildren<Animator>();
   
    }

    protected override void FSMUpdate()
    {
        switch (curState)
        {
            case SharkState.Patrol:
                UpdatePatrolState();
                break;
            case SharkState.Chase:
                UpdateChaseState();
                break;
            case SharkState.Cooldown:
                 UpdateCooldownState();
                 break;
                //case SharkState.Dead:
                //    UpdateDeadState();
                //    break;
        }

        if (health <= 0)
            curState = SharkState.Dead;
    }


    protected void UpdatePatrolState()
    {
        curSpeed = 6.7f;
        Move();

        if (Vector3.Distance(transform.position, destPos) <= patrollingRadius)
        {
            print("Point reached, get next point");
            FindNextPoint();
        }

        else if (Vector3.Distance(transform.position, playerTransform.position) <= chaseRange && playerTransform.position.y < 0)
        {
            print("player close, change to chase state");
            curState = SharkState.Chase;
        }
    }

    protected void UpdateChaseState()
    {
        if (playerTransform.position.y < -2.3f)
        {
            animator.SetBool("Chasing", true);
            curSpeed = 15f;
            destPos = playerTransform.position;
            Move();
            if (Vector3.Distance(transform.position, playerTransform.position) <= attackRange)
            {
                print("player in attack range, attack and change to cooldown state");
                playerStats.TakeDamage(45);
                curState = SharkState.Cooldown;
                StartCoroutine(CooldownTimer());
            }
            else if (Vector3.Distance(transform.position, playerTransform.position) >= chaseRange)
            {
                print("player escaped, change to patrol state");
                FindNextPoint();
                animator.SetBool("Chasing", false);
                curState = SharkState.Patrol;
            }
        }
        else
        {
            print("player is on land, change to patrol state");
            FindNextPoint();
            animator.SetBool("Chasing", false);
            curState = SharkState.Patrol;
        }
    }

    protected void UpdateCooldownState()
    {
        // this sucks.
        animator.SetBool("Chasing", false);
        curSpeed = 4f;
        Vector3 recoverPoint = transform.forward * -20f;
        destPos = recoverPoint;
        Move();
    }

    IEnumerator CooldownTimer()
    {
        yield return new WaitForSeconds(cooldownValue);
        print("Cooldown over, change to patrol state");
        curState = SharkState.Patrol;
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

    private void Move()
    {
        Vector3 dir = destPos - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * curRotSpeed);
        transform.Translate(Vector3.forward * Time.deltaTime * curSpeed);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
       
    }
}
