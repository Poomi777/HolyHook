using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingScript : MonoBehaviour
{
    // Start is called before the first frame update

    public float maxHealth;
    public float currentHealth;
    public float minObjectHurtSpeed;
    public float objectVelocityMultiplier;


    public GlassWinObject WindowWin1;
    public GlassWinObject WindowWin2;
    public GlassWinObject WindowWin3;

    public Light Torch;
    public GameObject deathCanvas;
    public Light Torch1;


    void Update()
    {
        if (currentHealth <= 0)
        {
            WindowWin1.enabled = true;
            WindowWin2.enabled = true;
            WindowWin3.enabled = true;
            deathCanvas.SetActive(true);

            Torch.color = new Color(0.00784313725490196f, 0.7843137254901961f, 0);
            Torch.intensity = 30;
            Torch1.color = new Color(0.00784313725490196f, 0.7843137254901961f, 0);
            Torch1.intensity = 30;

            GameManager.instance.enemyKills++;

            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        if ((other.GetComponent<Rigidbody>().velocity.magnitude > minObjectHurtSpeed || this.GetComponent<Rigidbody>().velocity.magnitude > minObjectHurtSpeed)  && other.gameObject.layer != LayerMask.NameToLayer("Enemy"))
        {
            currentHealth -= (other.GetComponent<Rigidbody>().velocity.magnitude + this.GetComponent<Rigidbody>().velocity.magnitude) * objectVelocityMultiplier;
        }
    }
}
