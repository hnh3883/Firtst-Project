using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signal_UI : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (Signal.FirstPoint == true)
        {
            Debug.Log("aaaaaaaaaaaaa");
            //gameObject.SetActive(true);
        }
    }

}
