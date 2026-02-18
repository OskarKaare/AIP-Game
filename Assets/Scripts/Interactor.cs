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
            Interact();
        }
    }
    public void Interact()
    {
        Debug.Log("Interacting");
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out var hit, interactDistance))
        {
            //if (hit.collider.TryGetComponent(out IInteractable interactable))
            //{
            //    interactable.Interact();
            //}
        }
    }
}
