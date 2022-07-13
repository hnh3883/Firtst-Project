using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CSVWriter : MonoBehaviour
{
    string filename = "";

    public static int count = 0;  
    Vector3 Crashposition; 

    public List<int> CollisionCounting = new List<int>();
    public List<float> CollisionPositionX = new List<float>();
    public List<float> CollisionPositionY = new List<float>();
    public List<float> CollisionPositionZ = new List<float>();
    public List<float> CollisionTime = new List<float>();

    void Start()
    {
        filename = Application.dataPath + "/Collision.csv";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            WriteCSV();
            Debug.Log("space2");
        }
    }

    public void OnCollisionEnter(UnityEngine.Collision collision)
    {
        count += 1;
        CollisionCounting.Add(count); 
        Debug.Log(count);

        Crashposition = this.gameObject.transform.position;

        CollisionPositionX.Add(Crashposition.x);
        CollisionPositionY.Add(Crashposition.y);
        CollisionPositionZ.Add(Crashposition.z);

        Debug.Log("x : " + Crashposition.x);
        Debug.Log("y : " + Crashposition.y);
        Debug.Log("z : " + Crashposition.z);

        CollisionTime.Add(Time.time);


    }

    public void WriteCSV()
    {
        if (CollisionCounting.Count > 0)
        {
            TextWriter tw = new StreamWriter(filename, false);

            tw.WriteLine("CollisionCounting, CollisionPositionX, CollisionPositionY, CollisionPositionZ, CollisionTime");
            tw.Close();

            tw = new StreamWriter(filename, true);
            for (int i = 0; i < CollisionCounting.Count; i++)
            {
                tw.WriteLine(CollisionCounting[i] + "," + CollisionPositionX[i] + "," +
                    CollisionPositionY[i] + "," + CollisionPositionZ[i] + "," + CollisionTime[i]);
            }
            tw.Close();
        }
    }
}
