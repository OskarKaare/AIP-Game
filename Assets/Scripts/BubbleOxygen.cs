using System.Collections.Generic;
using UnityEngine;

public class BubbleOxygen : MonoBehaviour
{
    public int oxygenPerBubble = 5;

    private ParticleSystem ps;
    private List<ParticleCollisionEvent> collisionEvents;
    private int playerLayer;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
        playerLayer = LayerMask.NameToLayer("Player");
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.layer != playerLayer)
            return;

        int hitCount = ps.GetCollisionEvents(other, collisionEvents);

        PlayerStats stats = other.GetComponent<PlayerStats>();

        if (stats != null)
        {
            stats.BubbleRefil(5);
        }
    }
}

