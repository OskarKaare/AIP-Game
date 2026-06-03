using UnityEngine;

public class FishFlockController : MonoBehaviour
{
    public float speed = 1f;
    public float radius = 20f;

    private Vector3 center;
    private float angle = 0f;

    void Start()
    {
        center = transform.position;
        speed = Random.Range(speed * 0.8f, speed * 1.2f);
    }

    void Update()
    {
        angle += speed * Time.deltaTime;

        Vector3 newPos = new Vector3(center.x + Mathf.Cos(angle) * radius, center.y,center.z + Mathf.Sin(angle) * radius);

        transform.LookAt(newPos);

        transform.position = newPos;
    }
}