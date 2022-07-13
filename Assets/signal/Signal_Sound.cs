using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signal_Sound : MonoBehaviour
{
    [SerializeField] AudioClip FirstSignal;
    AudioSource audioSource;

    public GameObject Target;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Target.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Signal.FirstPoint == true)
        {
            Target.SetActive(true);
            audioSource.PlayOneShot(FirstSignal);
            Signal.FirstPoint = false;

            Invoke("ImageOff", 5f);
        }
        
    }

    void ImageOff()
    {
        Target.SetActive(false);
    }
}
