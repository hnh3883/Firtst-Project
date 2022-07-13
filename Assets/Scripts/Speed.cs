using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speed : MonoBehaviour
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
	public float maxSpeed = 350f;    // 전진 최고속도
	public float maxRevSpeed = 100f; // 후진 최고속도
	
	// 백라이트 조정
	// public GameObject brakeLight;
	// public GameObject reverseLight;
	// public Material backBrakeIdle;
	// public Material backBrakeLight;
	// public Material backReverseIdle;
	// public Material backReverseLight;
	
	public int maxTorque = 30;
	
	private float prevSteerAngle;
	private bool bHandBraked = false;

	private float lowStiffness = 0.2f;
	private float highStiffness = 1f;

	public Texture2D speedometerDial;
	public Texture2D speedometerPointer;

	// Use this for initialization
	void Start () 
    {
		GetComponent<Rigidbody>().centerOfMass = new Vector3(0,-0.9f,0.5f); // 무게중심이 높으면 차가 쉽게 전복된다
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
		HandBrake ();
		SideSlip ();
		Control ();
	}
	
	void Update() 
    {
		// 앞바퀴 2개를 이동방향으로 향하기
		// Vector3.up = (0,1,0)
		tireTransformF.Rotate (Vector3.up, colliderF.steerAngle-prevSteerAngle, Space.World);
		prevSteerAngle = colliderF.steerAngle;
		
		WheelSuspension();
		// EngineSound ();
	}
	
	void Control() 
    {
		// 최고속도 제한
		// WheelCollider.rpm 전진:+, 후진:-
		currentSpeed = 2 * 3.14f * colliderR.radius * colliderR.rpm * 60 / 1000;
		
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
		
		//BackLight ();
		
		// 전후진 키를 누르지 않으면 제동이 걸리도록 한다
		if (!Input.GetButton ("Vertical")) 
        {
			colliderR.brakeTorque = decSpeed;
			//reverseLight.GetComponent<Renderer>().material = backReverseIdle;
			//brakeLight.GetComponent<Renderer>().material = backBrakeLight;
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
		wheelTransformF.Rotate (-colliderF.rpm/60*360 * Time.fixedDeltaTime, 0, 0);
		wheelTransformR.Rotate (-colliderR.rpm/60*360 * Time.fixedDeltaTime, 0, 0);
	}
	
	// 브레이크 등, 후진 등 점멸제어
	// //void BackLight() 
    // { 
	// 	float direction = Input.GetAxis("Vertical"); //전진:0.1~1, 후진:-0.1~-1, 아무키도 안눌리면:0

	// 	if (direction==1) {			//전진
	// 		reverseLight.GetComponent<Renderer>().material = backReverseIdle;
	// 		brakeLight.GetComponent<Renderer>().material = backBrakeIdle;
	// 	} else if (direction==-1) {	//후진

	// 		reverseLight.GetComponent<Renderer>().material = backReverseLight;
	// 		brakeLight.GetComponent<Renderer>().material = backBrakeIdle;
	// 	}
	// 	if (!Input.GetButton ("Vertical")  || bHandBraked) {
	// 		reverseLight.GetComponent<Renderer>().material = backReverseIdle;
	// 		brakeLight.GetComponent<Renderer>().material = backBrakeLight;
	// 	}
	// }
	
	private RaycastHit hit;
	private Vector3 wheelPos;
	void WheelSuspension() 
    {
		if(Physics.Raycast(colliderF.transform.position, -colliderF.transform.up,
		                   out hit,colliderF.radius+colliderF.suspensionDistance))
        {
			wheelPos = hit.point +(colliderF.radius * colliderF.transform.up);
			//print ("지면 충돌");
		}
        else
        {
			wheelPos = colliderF.transform.position - (colliderF.transform.up * colliderF.suspensionDistance);
			//print ("충돌 아님");
		}
		wheelPos.y +=3f; // 정상적인 모델이라면 이부분이 없어도 됨
		tireTransformF.position = wheelPos;
		
		if(Physics.Raycast(colliderF.transform.position, -colliderF.transform.up,
		                   out hit,colliderF.radius+colliderF.suspensionDistance))
        {
			wheelPos = hit.point + (colliderF.radius * colliderF.transform.up);
		}
        else
        {
			wheelPos = colliderF.transform.position - (colliderF.transform.up * colliderF.suspensionDistance);
		}
		wheelPos.y +=3f;
		tireTransformF.position = wheelPos;
		
		if(Physics.Raycast(colliderR.transform.position, -colliderR.transform.up,
		                   out hit,colliderR.radius+colliderR.suspensionDistance))
        {
			wheelPos = hit.point + (colliderR.radius * colliderR.transform.up);
		}
        else
        {
			wheelPos = colliderR.transform.position - (colliderR.transform.up * colliderR.suspensionDistance);
		}
		wheelPos.y +=3f;
		tireTransformR.position = wheelPos;
		
		if(Physics.Raycast(colliderR.transform.position, -colliderR.transform.up,
		                   out hit,colliderR.radius+colliderR.suspensionDistance))
        {
			wheelPos = hit.point + (colliderR.radius * colliderR.transform.up);
		}
        else
        {
			wheelPos = colliderR.transform.position - (colliderR.transform.up * colliderR.suspensionDistance);
		}
		wheelPos.y +=3f;
		tireTransformR.position = wheelPos;
	}

	void HandBrake(){
		if(Input.GetButton("Jump")) 
        {
			bHandBraked = true;
			//print ("핸드브레이크 작동");
			//colliderFL.motorTorque = 0;
			//colliderFR.motorTorque = 0;
			//colliderFL.brakeTorque = 100;
			//colliderFR.brakeTorque = 100;
			colliderR.brakeTorque = 100;
		}
        else
        {
			colliderF.brakeTorque = 0;
			colliderR.brakeTorque = 0;
			bHandBraked = false;
			//print ("핸드브레이크 해제");
		}
	}

	void SideSlip() {

		if (!Input.GetButton ("Vertical")) {
			WheelFrictionCurve wfc = new WheelFrictionCurve();
			wfc.asymptoteSlip = colliderR.sidewaysFriction.asymptoteSlip;
			wfc.asymptoteValue = colliderR.sidewaysFriction.asymptoteValue;
			wfc.extremumSlip = colliderR.sidewaysFriction.extremumSlip;
			wfc.extremumValue = colliderR.sidewaysFriction.extremumValue;
			wfc.stiffness = 0.01f;
			colliderR.sidewaysFriction = wfc;

			//colliderRL.forwardFriction = wfc;
			//colliderRR.forwardFriction = wfc;
		} else {
			WheelFrictionCurve wfc = new WheelFrictionCurve();
			wfc.asymptoteSlip = colliderR.sidewaysFriction.asymptoteSlip;
			wfc.asymptoteValue = colliderR.sidewaysFriction.asymptoteValue;
			wfc.extremumSlip = colliderR.sidewaysFriction.extremumSlip;
			wfc.extremumValue = colliderR.sidewaysFriction.extremumValue;
			wfc.stiffness = 1f;
			colliderR.sidewaysFriction = wfc;

			colliderR.forwardFriction = wfc;
		}
	}

	// void EngineSound() {
	// 	GetComponent<AudioSource>().pitch = currentSpeed / maxSpeed + 1;
	// }

	//speedometerDial(320X170), speedometerPointer(320X40)
	void OnGUI() {
		GUI.DrawTexture (new Rect(Screen.width/2-160,Screen.height-170,320,170),speedometerDial);
		float speedFactor = Mathf.Abs (currentSpeed / maxSpeed); // 최고속도에 대한 현재속도의 비
		float rotationAngle = Mathf.Lerp (0, 180, speedFactor);      // 속도비를 0 ~ 180 사이의 수(각도)로 표현
		GUIUtility.RotateAroundPivot(rotationAngle, new Vector2(Screen.width/2,Screen.height-20));
		GUI.DrawTexture (new Rect(Screen.width/2-160,Screen.height-40,320,40),speedometerPointer);
	}

}
