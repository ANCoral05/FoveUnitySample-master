using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class SaveTracking : MonoBehaviour
{
    [Header("Output Settings")]
    public string trackingFile = "tracking.csv"; // maybe change to automatically get subject dependend name?
    public string delimiter = ";";
    [Header("Eye Tracking")]
    public bool saveEyeTracking = true;
    [Header("Object Tracking")]
    public GameObject trackedGameobject;
    public bool savePosition = true;
    public bool saveOrientation = true;
    Queue trackingDataQueue = new Queue();
    static string msgBuffer = "";
    // Start is called before the first frame update
    void Start()
    {
        WriteHeader();
        InvokeRepeating("WriteTrackingData", 0.0f, 1.0f); // save data to file every second
    }
    void Update()
    {
        QueueTrackingData();
    }
    void WriteHeader()
    {
        StreamWriter sw = new StreamWriter(trackingFile);
        string header = "Timestamp" + delimiter;
        if(saveEyeTracking)
            header += "EyeData" + delimiter;
        if(savePosition)
            header += trackedGameobject.name + "_position" + delimiter;
        if(saveOrientation)
            header += trackedGameobject.name + "_orientation" + delimiter;
        header += "messages" + delimiter;
        sw.WriteLine(header);
        sw.Close();
    }
    public static void Msg(string msg)
    {
        msgBuffer = msg;    
    }

    void QueueTrackingData()
    {   
        // timestamp: use time at beginning of frame. What makes sense here? Use eye tracker timestamp?
        string datasetLine = Time.time.ToString("F5") + delimiter; // convert with 3 digits after decimal point
        // eyetracking data
        //if (saveEyeTracking)
        //    datasetLine += eyeTrackingData.ToString + delimiter;
        // tracked game object's position
        if (savePosition)
            datasetLine += trackedGameobject.transform.position.ToString("F3") + delimiter;
        // tracked game object's orientation
        if (saveOrientation)
            datasetLine += trackedGameobject.transform.rotation.ToString("F3") + delimiter;
        // buffered message
        if (!String.IsNullOrEmpty(msgBuffer))
        {
            datasetLine += msgBuffer + delimiter;
            msgBuffer = "";
        }
        trackingDataQueue.Enqueue(datasetLine);
    }

    void WriteTrackingData()
    {
        Debug.Log("Writing Data; fps: " + 1.0f/Time.deltaTime);
        StreamWriter sw = new StreamWriter(trackingFile, true); //true for append
        string datasetLine;
        // dequeue trackingDataQueue until empty
        while (trackingDataQueue.Count > 0)
        {
            datasetLine = trackingDataQueue.Dequeue().ToString();
            sw.WriteLine(datasetLine); // write to file
        }
        sw.Close(); // close file
        Debug.Log("End Writing Data");
    }    
}