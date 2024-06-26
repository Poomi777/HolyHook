using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HealthBarScript : MonoBehaviour
{
    private float maxHealth;
    private float oldHealth;
    private float currentHealth;
    private float reduceTimer = 3;



    private float currentScale;
    private float dividerToScale;

    void Start()
    {
        // Used to make sure that the scaling of the Healthbar is always appropriate.
        currentScale = transform.GetChild(0).localScale.x;
        dividerToScale = 1 / currentScale;
    }

    void Update()
    {
        if (reduceTimer < 2 && reduceTimer > 1)
        {
            reduceTimer += Time.deltaTime;
            oldHealth = Mathf.Lerp(oldHealth, currentHealth, (reduceTimer - 1));
            transform.GetChild(0).localScale = new Vector3(Mathf.Lerp(oldHealth/maxHealth, currentHealth/maxHealth, (reduceTimer - 1)) / dividerToScale, currentScale, currentScale);
        }
        else if (reduceTimer <= 1)
        {
            reduceTimer += Time.deltaTime;
        }
    }

    public void AssignHealth(float newMax)
    {
        maxHealth = newMax;
        oldHealth = maxHealth;
        currentHealth = maxHealth;
    }

    public void UpdateHealth(float damage)  // NOTE: this function is called by Playerhealthscript, Do not call this directly!
    {
        currentHealth -= damage;
        reduceTimer = 0;
        transform.GetChild(1).localScale = new Vector3((currentHealth/maxHealth) / dividerToScale, currentScale, currentScale);
    }

    public void Heal(float newHealth) // NOTE: this function is called by Playerhealthscript, Do not call this directly!
    {
        currentHealth += newHealth;
        reduceTimer = 3;
        transform.GetChild(1).localScale = new Vector3((currentHealth/maxHealth) / dividerToScale, currentScale, currentScale);
        transform.GetChild(0).localScale = new Vector3((currentHealth/maxHealth) / dividerToScale, currentScale, currentScale);
    }
}
