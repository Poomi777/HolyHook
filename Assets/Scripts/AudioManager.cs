using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    // Start is called before the first frame update

    public static AudioManager instance;
    public AudioMixerGroup audioMixer;
    public AudioClip[] enemySounds; 
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (instance == null)   
            instance = this;
        else
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator CreateSoundAtLocation(Vector3 worldPosition)
    {

        Debug.Log("CreateSoundAtLocation");
        GameObject obj = new GameObject();
        obj.transform.position = worldPosition;
        AudioSource objAudio = obj.AddComponent<AudioSource>();
        objAudio.playOnAwake = false;
        objAudio.spatialBlend = 1.0f;
        objAudio.rolloffMode = AudioRolloffMode.Logarithmic;
        objAudio.minDistance = 1.0f;
        objAudio.maxDistance = 3.0f;
        objAudio.PlayOneShot(enemySounds[Random.Range(0, enemySounds.Length)]);
        
        yield return null;

        

    }

    

}
