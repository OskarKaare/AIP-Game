using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float health = 100f;
    [SerializeField] private int currentOxygen = 80;
    [SerializeField] private int maxOxygen = 80;
    [SerializeField] private int oxygenDepletionRate = 1;
    [SerializeField] private float oxygenDepletionInterval = 1f;
    [SerializeField] private int oxygenReplenishmentRate = 1;
  

    void Start()
    {
        StartCoroutine(ManageOxygen());
    }
    private void Update()
    {
        //Debug.Log(transform.position.y);
    }

    IEnumerator ManageOxygen()
    {
        while (true)
        {
            if (transform.position.y < -2.55f)
            {
                // Deplete oxygen
                yield return new WaitForSeconds(oxygenDepletionInterval);
                currentOxygen -= oxygenDepletionRate;
                Debug.Log($"Oxygen: {currentOxygen}/{maxOxygen}");

                if (currentOxygen <= 0)
                {
                    currentOxygen = 0;
                    Die();
                }
            }
            else 
            {
                yield return new WaitForSeconds (0.25f); 
                if (currentOxygen < maxOxygen)
                    currentOxygen += oxygenReplenishmentRate;

            }
        }
    }

    void Die()
    {
        Debug.Log("Player died");
   
    }
}
