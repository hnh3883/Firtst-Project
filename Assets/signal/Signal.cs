using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signal : MonoBehaviour
{
    public static bool FirstPoint ;

    void Start()
    {
        FirstPoint = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GetComponent<MeshRenderer>().material.color = Color.red;
            Debug.Log("Turn left");

            FirstPoint = true;
        }
    }

}
