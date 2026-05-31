using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    private Camera cam;

    private float interactDistance = 3f;
    private float interactCooldown = 1f;
    private float delayTimer = 0f;
    public bool dogInBag;

    private void Start()
    {
        cam = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        delayTimer += Time.deltaTime;
        if (Keyboard.current.eKey.wasPressedThisFrame && interactCooldown < delayTimer)
        {
            delayTimer = 0f;
            TryoToInteract();
        }
    }
    public void TryoToInteract()
    {
        delayTimer = 2f;
        Debug.Log("Interacting");
        Physics.Raycast(cam.transform.position, cam.transform.forward, out var hit, interactDistance);
            if (hit.collider.CompareTag("Dog"))
            {
                Destroy(hit.collider.gameObject);
                Debug.Log("Dog picked up");
                dogInBag = true;
            }
        
    }
}
