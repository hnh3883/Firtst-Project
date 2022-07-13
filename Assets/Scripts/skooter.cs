using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skooter : MonoBehaviour
{
    // 키 설정 
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private float horizontalInput;
    private float verticalInput;
    public static float currentSteerAngle;
    private float currentbreakForce;
    private bool isBreaking;
    Rigidbody rb;

    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteerAngle;

    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;


    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;

    private void Start() 
    {
        rb = GetComponent<Rigidbody>();
        // 물체의 무게중심 위치를 변경
        // 무게 중심을 y축 아래 방향으로 낮춘다. 
        rb.centerOfMass = new Vector3(0, -200, 0);    
    }

    // 휠 콜라이더는 계속 물리연산을 하기 때문에 이러한 함수를 사용 
    private void FixedUpdate()
    {
        GetInput(); 
        HandleMotor();  
        HandleSteering(); 
        UpdateWheels();  
    }


    private void GetInput() 
    {
        // 전진 및 방향 회전, 브레이크 설정 
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        isBreaking = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor() 
    {
        // 바퀴가 회전하는 힘(커질수록 커짐)
        rearLeftWheelCollider.motorTorque = verticalInput * motorForce;
        // 조건 연산자 (isBreaking이 참이면 breakForce, 거짓이면 0f 실행)
        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();
    }

    private void ApplyBreaking()
    {
        // 바퀴가 회전하는 것을 멈추는 힘 대입
        rearLeftWheelCollider.brakeTorque = currentbreakForce;

    }

    private void HandleSteering()
    {
        // 바퀴의 조향 각도(앞바퀴의 회전 조절)
        currentSteerAngle = maxSteerAngle * horizontalInput;
        // 조향 각도를 더해줌 
        frontLeftWheelCollider.steerAngle = currentSteerAngle;

    }

    private void UpdateWheels()
    {
        // 자동으로 바퀴를 굴러가는 움직임을 구현하기 위해 추가 
        // UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;

        // 휠 콜라이더의 회전이나 방향전환 등의 정보를 추출 
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }
}