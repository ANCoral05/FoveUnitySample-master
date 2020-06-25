using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataAcquisition : FOVEBehavior
{
    public enum LeftOrRight
    {
        Left,
        Right
    }

    [SerializeField]
    public LeftOrRight whichEye;

    public Vector2 ScreenPos;

    public Camera cam;

    public float[] saccades;

    [Range(0, 5)]
    public float distance;

    public bool projectOnPlane;

    public SetNumbers SetNumbers;

    public List<float> saccadeTime = new List<float>();

    public List<Vector2> saccadePosition = new List<Vector2>();

    public List<Quaternion> camRotation = new List<Quaternion>();

    public List<string> CurrentTime = new List<string>();

    public List<int> Trials = new List<int>();

    public int trial;

    public string filePath;

    public bool SettingToList;

    public bool Saved;

    // Use this for initialization
    void Start()
    {
        filePath = Application.persistentDataPath + "/EyeTrackerData " + System.DateTime.Now.ToString("yyyy_MM_dd HH-mm-ss") + "/";
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }
    }

    // Latepdate ensures that the object doesn't lag behind the user's head motion
    void Update()
    {
        var rays = FoveInterface.GetGazeRays();
        var ray = whichEye == LeftOrRight.Left ? rays.left : rays.right;
        
        //print(ray + ": " + ScreenPos);

        int Step = SetNumbers.step;

        if(Step == 2 && !SettingToList)
        {
            InvokeRepeating("SetToList", 0, 1f/10f);
        }

        if((Step == 0 || Step == 1) && !Saved)
        {
            SettingToList = false;

            CancelInvoke("SetToList");
            //This is the writer, it writes to the filepath
            //if (!File.Exists(filePath + "EyeTracking_Save_" + trial + ".csv"))
            {
                //writer = File.Create(filePath + "EyeTracking_Save_" + trial + ".csv"); //" + System.DateTime.Now.ToString("yyyy_MM_dd HH:mm:ss" + "
            }

            StreamWriter writer = new StreamWriter(filePath + "EyeTracking_Save.csv");

            writer.WriteLine("Number,Trial,Time,Deltatime,ScreenPosX,ScreenPosY,CameraRotX,CameraRotY,CameraRotZ");

            writer.WriteLine("Trials: " + Trials.Count);

            for (int i = 0; i < saccadeTime.Count; i++)
            {
                writer.WriteLine(i + "," +
                    Trials[i] + "," +
                    CurrentTime[i] + "," +
                    saccadeTime[i] + "," +
                    saccadePosition[i].x + "," +
                    saccadePosition[i].y + "," +
                    camRotation[i].x + "," +
                    camRotation[i].y + "," +
                    camRotation[i].z);
            }

            Saved = true;

            trial += 1;
        }

        if(Step == 3)
        {
            Saved = false;
        }

        RaycastHit hit;
        Physics.Raycast(ray, out hit, Mathf.Infinity);
        if (hit.point != Vector3.zero && projectOnPlane) // Vector3 is non-nullable; comparing to null is always false
        {
            transform.position = hit.point;
        }
        else
        {
            transform.position = ray.GetPoint(distance);

            transform.rotation = Quaternion.LookRotation(ray.direction, Vector3.up) * Quaternion.Euler(90, 0, 0); //cam.transform.rotation *


        }
    }

    void SetToList()
    {
        SettingToList = true;

        ScreenPos = cam.WorldToScreenPoint(transform.position);

        saccadeTime.Add(Time.realtimeSinceStartup);

        saccadePosition.Add(ScreenPos);

        camRotation.Add(cam.transform.rotation);

        CurrentTime.Add(System.DateTime.Now.ToString("yyy_MM_dd HH:mm:ss"));

        Trials.Add(trial);
    }
}
