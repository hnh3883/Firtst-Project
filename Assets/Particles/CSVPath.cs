using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CSVPath : MonoBehaviour
{
    
    string filename = "";

    Vector3 Crashposition; 

    
    public List<float> PositionX = new List<float>();
    public List<float> PositionZ = new List<float>();


    void Start()
    {
        filename = Application.dataPath + "/Path.csv";
    }

    void Update()
    {
        Crashposition = this.gameObject.transform.position;

        PositionX.Add(Crashposition.x);
        PositionZ.Add(Crashposition.z);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            WriteCSV();
            Debug.Log("space2");
        }
    }

    public void WriteCSV()
    {
       
            TextWriter tw = new StreamWriter(filename, false);

            tw.WriteLine("PositionX, PositionZ");
            tw.Close();

            tw = new StreamWriter(filename, true);
            for (int i = 0; i < PositionX.Count; i++)
            {
                tw.WriteLine(PositionX[i] + "," + PositionZ[i] );
            }
            tw.Close();
        
    }

}
