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

    private AudioSource audioSource;
    public AudioClip playerHitSound;

    private void Awake()
    {

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            //audioSource.spatialBlend = 1.0f;
        }
    }
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.AssignHealth(maxHealth);
    }

    private void Update()
    {
        if (audioSource != null)
        {
            audioSource.outputAudioMixerGroup = AudioManager.instance.audioMixer;
        }
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
        audioSource.PlayOneShot(playerHitSound, 1.0f);
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
