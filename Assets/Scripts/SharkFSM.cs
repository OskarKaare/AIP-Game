using System.Collections;
using System.Collections.Generic;
using System.Timers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class SharkFSM : FSM
{
    public enum SharkState
    {
        Patrol,
        Lurk,
        Chase,
        Cooldown,
        Dead,
    }

    public SharkState curState = SharkState.Patrol;

    private float curSpeed;
    private float curRotSpeed = 5f;
    //private bool isDead = false;
    private bool deadLimit = false;
    private int health = 100;
    //private SphereCollider seekCollider;
    public List<Vector3> patrolPoints;
    private Transform playerTransform;
    private Vector3 destPos;
    private Rigidbody rb;
    public float patrollingRadius = 2f;
    public float chaseRange = 150f;
    public float attackRange = 10f;
    private Animator animator;
    private PlayerStats playerStats;
    private int cooldownValue = 7;
    private float underwaterLevel = -2.3f;
    private Rigidbody sharkRb;


    protected override void Initialize()
    {
        // Get and set patrol points
        patrolPoints = new List<Vector3>();
        foreach (Transform child in transform)
        {
            if (child.CompareTag("PatrolPoint"))
            {
                patrolPoints.Add(child.position);
                //Debug.Log ("Patrol point added: " + child.gameObject.name);

            }
        }
        if (patrolPoints != null && patrolPoints.Count > 0)
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
        sharkRb = GetComponent<Rigidbody>();

    }
    protected void FindNextPoint()
    {
        if (patrolPoints == null || patrolPoints.Count == 0)
        {
            Debug.LogWarning("no patrol points found");
            return;
        }
        int randomIndex = Random.Range(0, patrolPoints.Count);
        Vector3 offsetVector = new Vector3(Random.Range(0, 10), Random.Range(0, 10), Random.Range(0, 10));
        destPos = patrolPoints[randomIndex] + offsetVector;
    }
    protected override void FSMUpdate()
    {
        switch (curState)
        {
            case SharkState.Patrol:
                UpdatePatrolState();
                break;
            case SharkState.Lurk:
                UpdateLurkState();
                break;
            case SharkState.Chase:
                UpdateChaseState();
                break;
            case SharkState.Cooldown:
                UpdateCooldownState();
                break;
            case SharkState.Dead:
                animator.SetBool("Dead", true);
                if (!deadLimit)
                    StartCoroutine(DeathRemove());
                break;
        }

        if (health <= 0)
            curState = SharkState.Dead;
    }


    protected void UpdatePatrolState()
    {
        curSpeed = 19f;
        Move();

        if (Vector3.Distance(transform.position, destPos) <= patrollingRadius)
        {
            //print("Point reached, get next point");
            FindNextPoint();
        }

        else if (Vector3.Distance(transform.position, playerTransform.position) <= chaseRange && playerTransform.position.y < 0)
        {
            // print("player close, change to chase state");
            curState = SharkState.Lurk;
        }
    }

    protected void UpdateLurkState()
    {
        // dive down below the player
        curSpeed = 25f;
        destPos = new Vector3(playerTransform.position.x, playerTransform.position.y -75f, playerTransform.position.z);
        Move();

        // chase when player looks away, and if they escape go to patrol
        Vector3 toShark = transform.position - playerTransform.position;
        toShark.y = 0f;
        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0f;

        float angle = Vector3.Angle(camForward, toShark);
        if (angle > 90f)
        {
            curState = SharkState.Chase;
        }
        else if (Vector3.Distance(transform.position, playerTransform.position) >= chaseRange)
        {
         
            FindNextPoint();
            curState = SharkState.Patrol;
        }
    }

    protected void UpdateChaseState()
    {
        if (playerTransform.position.y < -underwaterLevel)
        {
            animator.SetBool("Chasing", true);
            curSpeed = 75f;
            destPos = playerTransform.position;
            Move();
            if (Vector3.Distance(transform.position, playerTransform.position) <= attackRange)
            {
                // print("player in attack range, attack and change to cooldown state");
                playerStats.TakeDamage(45);
                curState = SharkState.Cooldown;
                StartCoroutine(CooldownTimer());
            }
            else if (Vector3.Distance(transform.position, playerTransform.position) >= chaseRange)
            {
                // print("player escaped, change to patrol state");
                FindNextPoint();
                animator.SetBool("Chasing", false);
                curState = SharkState.Patrol;
            }
        }
        else
        {
            // print("player is on land, change to patrol state");
            FindNextPoint();
            animator.SetBool("Chasing", false);
            curState = SharkState.Patrol;
        }
    }

    protected void UpdateCooldownState()
    {
        // attack cooldown
        animator.SetBool("Chasing", false);
        curSpeed = 15f;
        Vector3 recoverPoint = transform.forward * -20f;
        destPos = recoverPoint;
        Move();
    }

    IEnumerator CooldownTimer()
    {
        yield return new WaitForSeconds(cooldownValue);
        //print("Cooldown over, change to patrol state");
        curState = SharkState.Patrol;
    }


    private void Move()
    {
        Vector3 dir = destPos - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(dir);
        sharkRb.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * curRotSpeed);
        sharkRb.MovePosition(transform.position + transform.forward * curSpeed * Time.deltaTime);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
    }

    IEnumerator DeathRemove()
    {
        deadLimit = true;
        //Debug.Log("Shark is dead, start death timer");
        yield return new WaitForSeconds(9.98f);
        Destroy(gameObject);
    }
}
