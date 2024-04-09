using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSound : MonoBehaviour
{
    public AudioClip collisionSound;
    private AudioSource audioSource;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1.0f;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (audioSource != null)
        {
            audioSource.outputAudioMixerGroup = AudioManager.instance.audioMixer;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        float audioLevel = collision.relativeVelocity.magnitude / 10.0f;
        audioSource.PlayOneShot(collisionSound, audioLevel);
    }
}
