using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float health = 100f;
    [SerializeField] private int currentOxygen = 80;
    [SerializeField] private int maxOxygen = 80;
    [SerializeField] private int oxygenDepletionRate = 1;
    [SerializeField] private float oxygenDepletionInterval = 1f;
    [SerializeField] private int oxygenReplenishmentRate = 1;
    [SerializeField] private float depthThreshold = -2.55f;

    [SerializeField] private Volume postProcessVolume;
    [SerializeField] private VolumeProfile[] allProfiles; // Add all your profiles here

    void Start()
    {
        // Set all vignettes to black at start
        InitializeVignettes();
        StartCoroutine(ManageOxygen());
    }

    void InitializeVignettes()
    {
        // Set vignette in all profiles to black
        if (allProfiles != null)
        {
            foreach (var profile in allProfiles)
            {
                if (profile != null && profile.TryGet(out Vignette vignette))
                {
                    vignette.color.value = Color.black;
                }
            }
        }
    }
    
    private void Update()
    {
        //Debug.Log(transform.position.y);
    }

    IEnumerator ManageOxygen()
    {
        while (true)
        {
            if (transform.position.y < depthThreshold)
            {
                // Deplete oxygen
                yield return new WaitForSeconds(oxygenDepletionInterval);
                currentOxygen -= oxygenDepletionRate;
                Debug.Log($"Oxygen: {currentOxygen}/{maxOxygen}");

                if (currentOxygen <= 0)
                {
                    TakeDamage(10f);
                }
            }
            else 
            {
                yield return new WaitForSeconds (0.25f); 
                if (currentOxygen < maxOxygen)
                    currentOxygen += oxygenReplenishmentRate;
                    if (currentOxygen > maxOxygen)
                        currentOxygen = maxOxygen;

            }
        }
    }

    void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log($"Player took {damage} damage. Current health: {health}");
        StartCoroutine(DamageFeedback());
        if (health <= 0)
        {
            health = 0;
            Die();
        }
    }

    IEnumerator DamageFeedback()
    {
        if (postProcessVolume != null && postProcessVolume.profile != null)
        {
            // Get vignette from CURRENT active profile
            if (postProcessVolume.profile.TryGet(out Vignette vignette))
            {
                // Flash red
                vignette.color.value = Color.red;
               
                
                yield return new WaitForSeconds(0.2f);
                
                // Reset to black
                vignette.color.value = Color.black;
            }
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }
    }
    
    void Die()
    {
        Debug.Log("Player died");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
