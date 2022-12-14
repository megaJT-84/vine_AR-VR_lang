using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class Object_Recorder : MonoBehaviour
{
    public GameObject gameObject;

    Transform cup_transform;

    float timeCounter;
    string fileName = "";

    [System.Serializable]
    public class DataItem
    {
        public string name;
        [HideInInspector] public float x;
        [HideInInspector] public float y;
        [HideInInspector] public float z;
        [HideInInspector] public float time;


    }

    [System.Serializable]
    public class DataList
    {
        public DataItem[] dataItems;
    }
    public DataList dataList = new DataList();

    private void Start()
    {

        //hp = go.GetComponent<HapticPlugin>();// grab the haptic actor component
        cup_transform = gameObject.GetComponent<Transform>();

        string dir = "C:\\Users\\vine2\\Desktop\\ExperimentData";
        string fName = dataList.dataItems[0].name + ".csv";
        fileName = dir + "\\" + fName;
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        else
        {
            TextWriter t_Writer = new StreamWriter(fileName, false);
            t_Writer.WriteLine("X, Y, Z, Time");
            t_Writer.Close();
        }


    }
    private void Update()
    {

        WriteToCSV();
    }

    public void WriteToCSV()
    {
        //Vector3 normalized = Vector3.Normalize(hp.CurrentVelocity);
        timeCounter += Time.deltaTime;
            if (dataList.dataItems.Length > 0)
            {

                TextWriter t_Writer = new StreamWriter(fileName, true);
                for (int i = 0; i < dataList.dataItems.Length; i++)
                {
                    //float x = dataList.dataItems[i].x + (Mathf.Abs(hp.CurrentVelocity.x));
                    //float y = dataList.dataItems[i].y + (Mathf.Abs(hp.CurrentVelocity.y));
                    //float z = dataList.dataItems[i].z + (Mathf.Abs(hp.CurrentVelocity.z));

                    float x = dataList.dataItems[i].x + cup_transform.position.x;
                    float y = dataList.dataItems[i].y + cup_transform.position.y;
                    float z = dataList.dataItems[i].z + cup_transform.position.z;
                    float time = dataList.dataItems[i].time + timeCounter;

                    t_Writer.WriteLine("x:" + x + "," + "y:" + y + "," + "z:" + z + "," + "time:" + time);

                }
                t_Writer.Close();

        }
    }
}