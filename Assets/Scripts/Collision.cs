using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Collision : MonoBehaviour
{
    public static int count = 0;
    [SerializeField] TextMeshProUGUI DisplayCount;

    private void Start()
    {
        DisplayCount.text = "not Collision";
    }

    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
            count += 1;
        Debug.Log(count);
        DisplayCount.text = "Collision : " + count;
    }

    

}
