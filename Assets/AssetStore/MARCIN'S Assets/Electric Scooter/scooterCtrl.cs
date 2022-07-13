using UnityEngine;

public class scooterCtrl : MonoBehaviour
{
    //[SerializeField]float delay = 1.0f;

    [Range(-30.0f, 30.0f)] public float Size = 0.0f; 

    private void Start()
    {
        //delay = Random.Range(0.0f, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        //Size = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up * skooter.currentSteerAngle/360);

        //Debug.Log(Size);
           // Input.GetAxis("Horizontal")
    }
}
