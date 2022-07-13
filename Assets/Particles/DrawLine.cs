using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    public GameObject[] checkPoint;

    private void FixedUpdate()
    {
        checkPoint = GameObject.FindGameObjectsWithTag("CheckPoint");

        if (checkPoint.Length > 1)
        {
            for (int i = 0; i < checkPoint.Length - 1; i++)
            {
                Debug.DrawLine(checkPoint[i].transform.position, checkPoint[i + 1].transform.position, Color.red);
            }
        }
    }
}