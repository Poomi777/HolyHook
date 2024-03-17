using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealtScript : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public HealthBarScript healthBar;
    
    
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.AssignHealth(maxHealth);
    }

    
    public void TakeDamage(float damage)
    {
        if ((currentHealth - damage) < 0)
        {
            healthBar.UpdateHealth(currentHealth);
            currentHealth = 0;    
        }
        else
        {
            healthBar.UpdateHealth(damage);
            currentHealth -= damage;  
        }
    }
}
