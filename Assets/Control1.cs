using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control1 : MonoBehaviour
{
    // 바퀴 방향조종을 위한 Transform 2개
	public Transform tireTransformF;
	public Transform tireTransformR;
	
	public WheelCollider colliderF;
	public WheelCollider colliderR;
	
	// 바퀴 회전을 위한 Transform
	public Transform wheelTransformF;
	public Transform wheelTransformR;
	
	// 속도에 따라서 방향전환율을 다르게 적용하기 위한 준비
	public float highestSpeed = 500f;
	public float lowSpeedSteerAngle = 0.1f;
	public float highSpeedStreerAngle = 25f;
	
	// 감속량
	public float decSpeed = 7f;
	
	// 속도제한을 위한 변수들
	public float currentSpeed;
	public float maxSpeed = 500f;    // 전진 최고속도
	public float maxRevSpeed = 100f; // 후진 최고속도
	

	
	public int maxTorque = 50;
	
	private float prevSteerAngle;
	private bool bHandBraked = false;

	private float lowStiffness = 0.2f;
	private float highStiffness = 1f;

	public Texture2D speedometerDial;
	public Texture2D speedometerPointer;

	void Start () 
    {
		GetComponent<Rigidbody>().centerOfMass = new Vector3(0,-100f,0); // 무게중심이 높으면 차가 쉽게 전복된다
	}
	
	void FixedUpdate () 
    {
        Use();
		UpdateWheels();  
		// HandleMotor();  
        // HandleSteering();   
        HandBrake ();
		SideSlip ();
	}
	
	void Update() 
    {
		// 앞바퀴 2개를 이동방향으로 향하기
		tireTransformF.Rotate (Vector3.up, colliderF.steerAngle-prevSteerAngle, Space.World);
		prevSteerAngle = colliderF.steerAngle;
	}
	
	void Use() 
    {
		// 최고속도 제한
		// WheelCollider.rpm 전진:+, 후진:-
		currentSpeed = 2 * 3.14f * colliderR.radius * colliderR.rpm * 60 / 300;
		
		float direction = Input.GetAxis("Vertical"); //전진:0.1~1, 후진:-0.1~-1
		//print ("direction:" + direction);
		float torque = maxTorque * direction;
		
		if(!bHandBraked && direction>0 && currentSpeed<maxSpeed) 
        {
			//print ("전진");
			colliderF.motorTorque = torque;
		}
        else if(!bHandBraked && direction<0 && Mathf.Abs(currentSpeed)<maxRevSpeed) 
        {
			//print ("후진");
			colliderF.motorTorque = torque;
		}
        else
        {
			colliderF.motorTorque = 0;
		}
		
		
		// 전후진 키를 누르지 않으면 제동이 걸리도록 한다
		if (!Input.GetButton ("Vertical")) 
        {
			colliderR.brakeTorque = decSpeed;
		} else 
        {
			colliderR.brakeTorque = 0;
		}
		
		// 속도에 따라 방향전환율을 달리 적용하기 위한 계산
		float speedFactor = GetComponent<Rigidbody>().velocity.magnitude / highestSpeed;
		/** Mathf.Lerp(from, to, t) : Linear Interpolation(선형보간)
		 * from:시작값, to:끝값, t:중간값(0.0 ~ 1.0)
		 * t가 0이면 from을 리턴, t가 1이면 to 를 리턴함, 0.5라면 from, to 의 중간값이 리턴됨
		*/
		float steerAngle = Mathf.Lerp (lowSpeedSteerAngle, highSpeedStreerAngle, 1/speedFactor);
		//print ("steerAngle:" + steerAngle);
        // x*=y-> x = x*y
		steerAngle *= Input.GetAxis("Horizontal");
		
		//좌우 방향전환
		colliderF.steerAngle = steerAngle;
		
		// 바퀴회전효과
		//wheelTransformF.Rotate (-colliderF.rpm/60*360 * Time.fixedDeltaTime, 0, 0);
		//wheelTransformR.Rotate (-colliderR.rpm/60*360 * Time.fixedDeltaTime, 0, 0);
	}
	
	//speedometerDial(320X170), speedometerPointer(320X40)
	void OnGUI() {
		GUI.DrawTexture (new Rect(Screen.width/2-160,Screen.height-170,320,170),speedometerDial);
		float speedFactor = Mathf.Abs (currentSpeed / maxSpeed); // 최고속도에 대한 현재속도의 비
		float rotationAngle = Mathf.Lerp (0, 180, speedFactor);      // 속도비를 0 ~ 180 사이의 수(각도)로 표현
		GUIUtility.RotateAroundPivot(rotationAngle, new Vector2(Screen.width/2,Screen.height-20));
		GUI.DrawTexture (new Rect(Screen.width/2-160,Screen.height-40,320,40),speedometerPointer);
	}

	private void UpdateWheels()
    {
        // 자동으로 바퀴를 굴러가는 움직임을 구현하기 위해 추가 
        // UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(colliderR, wheelTransformR);
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

	void HandBrake()
	{
		if(Input.GetButton("Jump")) {
			bHandBraked = true;		
			colliderF.brakeTorque = 100;
		}
		else{
			colliderF.brakeTorque = 0;					
			colliderR.brakeTorque = 0;				
			bHandBraked = false;			
		}	
	}

	void SideSlip() {
		// 앞바퀴 브레이크 적용시 뒷바퀴가 미끄러지도록 마찰력을 줄임
		// if (!Input.GetButton ("Vertical"))
		// {          
		// 	WheelFrictionCurve wfc = new WheelFrictionCurve();
		// 	wfc.asymptoteSlip = colliderR.sidewaysFriction.asymptoteSlip;
		// 	wfc.asymptoteValue = colliderR.sidewaysFriction.asymptoteValue;
		// 	wfc.extremumSlip = colliderR.sidewaysFriction.extremumSlip;
		// 	wfc.extremumValue = colliderR.sidewaysFriction.extremumValue;
		// 	wfc.stiffness = 0.01f;
		// 	colliderR.sidewaysFriction = wfc;
		// 	colliderR.sidewaysFriction = wfc;	
		// 	} 
		// else 
		// {
		WheelFrictionCurve wfc = new WheelFrictionCurve();     
		wfc.asymptoteSlip = colliderR.sidewaysFriction.asymptoteSlip;
		wfc.asymptoteValue = colliderR.sidewaysFriction.asymptoteValue;
		wfc.extremumSlip = colliderR.sidewaysFriction.extremumSlip;
		wfc.extremumValue = colliderR.sidewaysFriction.extremumValue;  
		wfc.stiffness = 1f;    
		colliderR.sidewaysFriction = wfc;       
		colliderR.forwardFriction = wfc;      
		// }  
		 }
}