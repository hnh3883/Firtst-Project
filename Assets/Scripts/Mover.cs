using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    public WheelCollider[] wheels = new WheelCollider[2]; //휠 콜라이더를 받아온다.
    public Transform[] tires = new Transform[2]; //바퀴가 돌아가는 걸 표현하기위한 메쉬를 받아온다.

    public float maxF = 50.0f; //자동차바퀴를 돌리는 힘
    public float power = 3000.0f; //자동차를 밀어주는 힘(바퀴만으로는 너무 느리다..)
    public float rot = 45;

    Rigidbody rb;



    void Start()
    {
        rb = GetComponent<Rigidbody>(); //리지드바디를 받아온다.

        wheels[0].steerAngle = 90;
        wheels[1].steerAngle = 90;//만약 바퀴와 휠콜라이더의 방향이 교차한다면 90으로 설정해주자.
        wheels[0].ConfigureVehicleSubsteps(1000, 1000, 1000);

        rb.centerOfMass = new Vector3(0, 0, -1); //무게중심을 가운데로 맞춰서 안정적으로 주행하도록 한다.
    }

    private void Update()
    {
        UpdateMeshesPostion(); //바퀴가 돌아가는게 보이도록 함
    }

    void FixedUpdate()
    {
        float accelerate = Input.GetAxis("Vertical");

        rb.AddForce(transform.rotation * new Vector3(accelerate * power, 0, 0)); //뒤에서 밀어준다.

        for (int i = 0; i < 2; i++)
        {
            wheels[i].motorTorque = maxF * accelerate; //바퀴를 돌린다.
        }

        float steer = rot * Input.GetAxis("Horizontal");
        wheels[0].steerAngle = steer+90; //여기도 바퀴와 콜라이더가 직각인사람은 + 90을 해줘야한다.
        wheels[1].steerAngle = steer + 90; //여기도 바퀴와 콜라이더가 직각인사람은 + 90을 해줘야한다.
    }

    void UpdateMeshesPostion()
    {
        for(int i = 0; i <2; i++)
        {
            Quaternion quat;
            Vector3 pos;

            wheels[i].GetWorldPose(out pos, out quat);
            tires[i].position = pos;
            tires[i].rotation = quat;
        }

    }
}