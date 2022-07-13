using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{

    [SerializeField] float xAngle = 0;
    [SerializeField] float yAngle = 0;
    [SerializeField] float zAngle = 0;

    [SerializeField] float moveSpeed = 1f;

    void Update()
    {
        MovePlayer();

        transform.Rotate(xAngle, yAngle, zAngle);
    }

    void MovePlayer()
    {
        float xValue = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        float zValue = -Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;

        transform.Translate(xValue,0,zValue);
    }
}
