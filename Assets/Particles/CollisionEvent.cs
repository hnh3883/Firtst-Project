using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionEvent : MonoBehaviour
{
    [SerializeField] AudioClip crash;
    [SerializeField] ParticleSystem crashParicles;

    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();    
    }

    public void OnCollisionEnter(UnityEngine.Collision collision)
    {
        audioSource.PlayOneShot(crash);
        crashParicles.Play();
    }
}
