using System.Collections;
using TMPro;
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
    [SerializeField] private VolumeProfile[] allProfiles;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI oxygenText;
    public TextMeshProUGUI depthText;

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
        healthText.text = $" {health} / 100";
        oxygenText.text = $"{currentOxygen} / {maxOxygen}";
        if (transform.position.y < depthThreshold)
        {
            depthText.text = $"Depth: {Mathf.RoundToInt(-transform.position.y)}m";
        }
        else
        {
            depthText.text = "Depth: 0m";
        }

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
                yield return new WaitForSeconds(0.25f);
                if (currentOxygen < maxOxygen)
                    currentOxygen += oxygenReplenishmentRate;
                if (currentOxygen > maxOxygen)
                    currentOxygen = maxOxygen;

            }
        }
    }
    
    public void BubbleRefil(int amount)
    {
        currentOxygen += amount;
        if (currentOxygen > maxOxygen)
            currentOxygen = maxOxygen;
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
        // get current profile, set vignette to flash red.
        if (postProcessVolume != null && postProcessVolume.profile != null)
        {
            if (postProcessVolume.profile.TryGet(out Vignette vignette))
            {
                vignette.color.value = Color.red;
                yield return new WaitForSeconds(0.2f);
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
