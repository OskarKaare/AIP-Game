using System.Security.Cryptography;
using UnityEngine;

public class FishFlockController : MonoBehaviour
{
    public Vector3 bound;
    public float speed = 10f;
    private Vector3 initialPosition;
    private Vector3 nextMovementPoint;
    // Use this for initialization
    void Start()
    {
        initialPosition = transform.position;
        CalculateNextMovementPoint();
        speed = Random.Range(speed * 0.5f, speed * 1.5f);
    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(nextMovementPoint - transform.position), 1.0f * Time.deltaTime);

     
    }
    void CalculateNextMovementPoint()
    {

        float posX = Random.Range(-bound.x, bound.x);
        float posY = Random.Range(-bound.y, bound.y);
        float posZ = Random.Range(-bound.z, bound.z);
        nextMovementPoint = initialPosition + new Vector3(posX, posY, posZ);
    }
}
