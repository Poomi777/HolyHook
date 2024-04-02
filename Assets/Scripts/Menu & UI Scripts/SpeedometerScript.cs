using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeedometerScript : MonoBehaviour
{
    
    public float MaxSpeed;
    public float shakeIntensity;
    public float shakeThreshold;
    public GameObject player;


    private Image speedometerComponent;
    private Rigidbody playerBody;
    private TextMeshProUGUI speedometerText;
    private TextMeshProUGUI speedometerTextBackground;
    private Vector3 basePosition;

    void Start()
    {
        speedometerComponent = transform.GetComponent<Image>();
        playerBody = player.GetComponent<Rigidbody>();
        speedometerText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        speedometerTextBackground = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        basePosition = transform.localPosition;
    }

    void LateUpdate()
    {
        speedometerText.text = ((int)(playerBody.velocity.magnitude * 10)).ToString();
        speedometerTextBackground.text = ((int)(playerBody.velocity.magnitude * 10)).ToString();
        if (playerBody.velocity.magnitude <= MaxSpeed && playerBody.velocity.magnitude >= 1)
        {
            transform.localPosition = basePosition;
            speedometerComponent.color = new Color(0.9f, Mathf.Abs((playerBody.velocity.magnitude / MaxSpeed) - 1), 0);
            speedometerComponent.fillAmount =  0.01f * playerBody.velocity.magnitude;
        }
        else if (playerBody.velocity.magnitude < 1)
        {
            speedometerComponent.color = new Color(0.9f, 1, 0);
            speedometerComponent.fillAmount =  0.01f;
        }
        else
        {
            speedometerComponent.fillAmount =  MaxSpeed * 0.01f;
            speedometerComponent.color = new Color(0.9f, 0, 0);
        }


        if (playerBody.velocity.magnitude >= shakeThreshold * MaxSpeed)
        {
            transform.localPosition = new Vector3(basePosition.x + UnityEngine.Random.Range(-shakeIntensity * (playerBody.velocity.magnitude / MaxSpeed), 
                        shakeIntensity * (playerBody.velocity.magnitude / MaxSpeed)), basePosition.y + UnityEngine.Random.Range(-shakeIntensity, shakeIntensity), 
                        basePosition.z);
        }
        else
        {
            transform.localPosition = basePosition;
        }
    }
}
