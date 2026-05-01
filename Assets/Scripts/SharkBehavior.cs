using System.Collections;
using UnityEngine;

public class SharkBehavior : MonoBehaviour
{
    private SphereCollider seekCollider;
    private int chaseTimer = 10;
    public Transform player;
    private Animator animator;

    private float speed = 4f;
    void Start()
    {
        animator = GetComponent<Animator>();
        seekCollider = GetComponent<SphereCollider>();
        StartCoroutine(Move());
    }


    IEnumerator Move()
    {
        Vector3 currentDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;

        while (true)
        {
            Vector3 targetDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
            float moveDuration = 5f;
            float elapsedTime = 0f;
            float turnDuration = 100f;

            while (elapsedTime < moveDuration)
            {
                float t = Mathf.Clamp01(elapsedTime / turnDuration);
                currentDirection = Vector3.Lerp(currentDirection, targetDirection, t).normalized;

                transform.position += currentDirection * Time.deltaTime * speed;

                Quaternion targetRotation = Quaternion.LookRotation(-currentDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            currentDirection = targetDirection;
        }
    }


    private void OnTriggerStay(Collider other)
    {
        player = other.transform;
        chaseTimer--;
        if (chaseTimer <= 0)
        {
            // Chase the player
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * Time.deltaTime * 5f; // Adjust speed as needed

            //Quaternion targetRotation = Quaternion.LookRotation(direction);
            //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 1f);
        }

    }
    // Update is called once per frame
    void Update()
    {
       
    }
}
