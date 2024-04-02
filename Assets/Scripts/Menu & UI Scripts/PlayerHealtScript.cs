using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealtScript : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public float DeathTime;
    public HealthBarScript healthBar;
    public GameObject deathCanvas;
    
    
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.AssignHealth(maxHealth);
    }

    void PlayerDeath()
    {
        transform.GetComponent<PlayerController>().dead = true;
        Time.timeScale = 0.5f;
        deathCanvas.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void TakeDamage(float damage)
    {
        if ((currentHealth - damage) <= 0)
        {
            healthBar.UpdateHealth(currentHealth);
            currentHealth = 0;
            PlayerDeath();
        }
        else
        {
            healthBar.UpdateHealth(damage);
            currentHealth -= damage;
        }
    }

    public void Heal(float healAmount)
    {
        if ((currentHealth + healAmount) > maxHealth)
        {
            healthBar.UpdateHealth(maxHealth - currentHealth);
            currentHealth = maxHealth;
        }
        else
        {
            healthBar.UpdateHealth(healAmount);
            currentHealth += healAmount;
        }
    }
}
