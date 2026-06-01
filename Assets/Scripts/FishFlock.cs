using UnityEngine;
using System.Collections;

public class FishFlock : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float minSpeed = 20f;
    public float turnSpeed = 20f;
    public float randomFreq = 20f;

    public float randomForce = 20f;
    //alignment variables
    public float toOriginForce = 50f;
    public float toOriginRange = 100f;
    public float gravity = 2f;

    //seperation variables
    public float avoidanceRadius = 50f;
    public float avoidanceForce = 20f;

    //cohesion variables
    public float followVelocity = 4f;
    public float followRadius = 40f;

    // player variables
    public float playerAvoidanceRadius = 50f;
    public float playerAvoidanceForce = 150f;



    //these variables control the movement of the boid
    private Transform origin;
    private Vector3 velocity;
    private Vector3 normalizedVelocity;
    private Vector3 randomPush;
    private Vector3 originPush;
    private Transform[] objects;
    private FishFlock[] otherFlocks;
    private Transform transformComponent;
    private float randomFreqInterval;
    public float waterlevel;
    private Transform playerTransform;

    void Start()
    {
        randomFreqInterval = 1f / randomFreq;
        // Assign the parent as origin
        origin = transform.parent;
        // Flock transform
        transformComponent = transform;
        // Temporary components
        Component[] tempFlocks = null;
        // Get all the unity flock components from the parent
        // transform in the group
        if (transform.parent)
        {
            tempFlocks = transform.parent.GetComponentsInChildren<FishFlock>();
        }
        // Assign and store all the flock objects in this group
        objects = new Transform[tempFlocks.Length];
        otherFlocks = new FishFlock[tempFlocks.Length];
        for (int i = 0; i < tempFlocks.Length; i++)
        {
            if (objects == null) continue;
            objects[i] = tempFlocks[i].transform;
            otherFlocks[i] = (FishFlock)tempFlocks[i];

        }
        // Null Parent as the flock leader will be
        // UnityFlockController object
        transform.parent = null;
        // Calculate random push depends on the random
        // frequency provided
        StartCoroutine(UpdateRandom());

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            playerTransform = player.transform;

    }
    IEnumerator UpdateRandom()
    {
        while (true)
        {
            randomPush = Random.insideUnitSphere * randomForce;
            yield return new WaitForSeconds(randomFreqInterval + Random.Range(-randomFreqInterval / 2.0f, randomFreqInterval / 2.0f));
        }
    }
    void Update()
    {
        //SEPARATION
        //Internal variables
        float speed = velocity.magnitude;
        Vector3 avgVelocity = Vector3.zero;
        Vector3 avgPosition = Vector3.zero;
        int count = 0;
        Vector3 myPosition = transformComponent.position;
        Vector3 forceV;
        Vector3 toAvg;
        for (int i = 0; i < objects.Length; i++)
        {
            Transform boidTransform = objects[i];
            if (boidTransform == null) continue;
            if (boidTransform != transformComponent)
            {
                Vector3 otherPosition = boidTransform.position;

                // Only consider other boids that are at or below Y = 0
                if (otherPosition.y <= 0f)
                {
                    // Average position to calculate cohesion
                    avgPosition += otherPosition;
                    count++;

                    //Directional vector from other flock to this flock
                    forceV = myPosition - otherPosition;

                    //Magnitude of that directional
                    //vector(Length)
                    float directionMagnitude = forceV.magnitude;
                    float forceMagnitude = 0f;
                    if (directionMagnitude < followRadius)
                    {
                        if (directionMagnitude < avoidanceRadius)
                        {
                            forceMagnitude = 1f - (directionMagnitude / avoidanceRadius);
                            if (directionMagnitude > 0)
                                avgVelocity += (forceV / directionMagnitude) * forceMagnitude * avoidanceForce;
                            // forceV / directionMagnitude = normalized directon AWAY
                        }
                        forceMagnitude = directionMagnitude / followRadius;
                        FishFlock tempOtherBoid = otherFlocks[i];
                        if (tempOtherBoid == null) continue;
                        avgVelocity += followVelocity * forceMagnitude * tempOtherBoid.normalizedVelocity;
                    }
                }
            }
        }
        if (count > 0)
        {
            //Calculate the average flock velocity(Alignment)
            avgVelocity /= count;
            //Calculate Center value of the flock(Cohesion)
            toAvg = (avgPosition / count) - myPosition;
        }
        else
        {
            toAvg = Vector3.zero;
        }
        //Directional Vector to the leader
        Vector3 leaderForceV = origin.position - myPosition;
        float leaderDirectionMagnitude = leaderForceV.magnitude;
        float leaderForceMagnitude = leaderDirectionMagnitude / toOriginRange;
        //Calculate the velocity of the flock to the leader
        if (leaderDirectionMagnitude > 0)
            originPush = leaderForceMagnitude * toOriginForce * (leaderForceV / leaderDirectionMagnitude);
        if (speed < minSpeed && speed > 0)
        {
            velocity = (velocity / speed) * minSpeed;
        }

        Vector3 playerPush = Vector3.zero;
        if (playerTransform != null)
        {
            Vector3 toPlayer = myPosition - playerTransform.position;
            float playerDist = toPlayer.magnitude;
            if (playerDist < playerAvoidanceRadius && playerDist > 0f)
            {
                float strength = 1f - (playerDist / playerAvoidanceRadius);
                playerPush = (toPlayer / playerDist) * strength * playerAvoidanceForce;
            }

        }
            // final velocity calculations
            Vector3 wantedVel = velocity;
        wantedVel -= wantedVel * Time.deltaTime;
        wantedVel += randomPush * Time.deltaTime;
        wantedVel += originPush * Time.deltaTime;
        wantedVel += avgVelocity * Time.deltaTime;
        wantedVel += playerPush * Time.deltaTime;

            // gravity limitations, we dont want the fish to be able to "fly"
            Vector3 gravityPush = gravity * Time.deltaTime * toAvg.normalized;
        if (myPosition.y >= waterlevel && gravityPush.y > waterlevel)
        {
            gravityPush.y = 0f;
        }
        wantedVel += gravityPush;

        // Prevent foid from gaining velocity that would push it above water level
        float projectedNextY = transformComponent.position.y + wantedVel.y * Time.deltaTime;
        if (projectedNextY > waterlevel)
        {
            wantedVel.y = 0f;
        }

        velocity = Vector3.RotateTowards(velocity, wantedVel, turnSpeed * Time.deltaTime, 100.00f);
        // check if foid is above water level, if so, set the vertical velocity to 0.
        float projectedFinalNextY = transformComponent.position.y + velocity.y * Time.deltaTime;
        if (projectedFinalNextY > waterlevel)
        {
            velocity.y = 0f;
        }

        transformComponent.rotation = Quaternion.LookRotation(velocity);
        //Move the flock based on the calculated velocity
        transformComponent.Translate(velocity * Time.deltaTime, Space.World);
        normalizedVelocity = velocity.normalized;
    }
}

