using TMPro;
using UnityEngine;

public class NPCBehavior : MonoBehaviour
{
    public Canvas questText;
    private Transform playerTransform;
    private float turnSpeed = 5f;
    public Interactor interactor;
  

    private void Start()
    {
        questText.enabled = false;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        playerTransform = playerObj.transform;
    }

    private void OnTriggerStay(Collider other)
    {
        Vector3 dir = playerTransform.position - transform.position;

        float yaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, yaw, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

       // Debug.Log("Player in range");
        questText.enabled = true;

        if (interactor.dogInBag == true)
        {
            //Debug.Log("Player has the dog, update quest text");
            TextMeshProUGUI textField = questText.GetComponentInChildren<TextMeshProUGUI>();
            textField.text = "Thank you for bringing my dog back!";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        questText.enabled = false;
    }
}
