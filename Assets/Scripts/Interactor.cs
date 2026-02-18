using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    private Camera cam;
    private float interactDistance = 3f;

    private void Start()
    {
        cam = GetComponentInChildren<Camera>();
       // onInteract.Invoke();
        
    }

    private void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Interact();
        }

    }
    public void Interact()
    {
        Debug.Log("Interact button pressed!");
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out var hit, interactDistance))
        {
            Debug.Log($"Hit: {hit.collider.name}");
            if (hit.collider.CompareTag("fish"))
            {
                Debug.Log("Interacted with a fish!");

            }
            else if (hit.collider.CompareTag("item"))
            {
                Debug.Log("Interacted with an item!");
            }
        }
    }
}
