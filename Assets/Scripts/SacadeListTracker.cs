using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class SacadeListTracker : MonoBehaviour
{
    public RotationMeasurement rotationMeasurement;
    public ManagerScript manager;
    public NewSetNumbers searchTaskScript;
    private string directory;
    private string visualTask;
    private string scanningPattern;

    private bool hasWrittenSaccade;
    private Saccade saccade;
    private int saccadeNumber;

    [Header("Output Settings")]
    public int participantNumber = 0;
    public string trackingFile = "SaccadeList.csv";
    public string delimiter = ";";



    Queue trackingDataQueue = new Queue();
    static string msgBuffer = "";

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<ManagerScript>();

        if (manager.searchTask)
            visualTask = "_SearchTask";
        if (manager.navigation)
            visualTask = "_Navigation";
        if (!manager.searchTask && !manager.navigation)
            visualTask = "_NoTask";

        if (manager.leftRight)
            scanningPattern = "_LeftRightScanning";
        if (manager.radial)
            scanningPattern = "_RadialScanning";
        if (!manager.radial && !manager.leftRight)
            scanningPattern = "_FreeScanning";

        directory = "Assets/Resources/Participant_" + manager.participantNumber + visualTask + scanningPattern + "_Date_" + System.DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + "/";
        Directory.CreateDirectory(directory);
        WriteHeader();
        InvokeRepeating("WriteTrackingData", 0.0f, 1.0f); // save data to file every second
    }

    // Update is called once per frame
    void Update()
    {
        if (rotationMeasurement.isSaccade == 0 && rotationMeasurement.saccadeNumber > saccadeNumber && !hasWrittenSaccade)
        {
            saccade = rotationMeasurement.saccade;
            QueueTrackingData();
            hasWrittenSaccade = true;
            saccadeNumber += 1;
        }

        if (rotationMeasurement.isSaccade == 1)
            hasWrittenSaccade = false;

        if (rotationMeasurement.save)
            CancelInvoke("WriteTrackingData");
    }

    void WriteHeader()
    {
        StreamWriter sw = new StreamWriter(directory + trackingFile);
        string header = "Saccade_Number" + delimiter;
        header += "Start_Time" + delimiter;

        header += "Left-Right_Saccade" + delimiter;
        header += "Left-Right_Saccade_percentage" + delimiter;
        header += "Radial_Saccade" + delimiter;
        header += "Radial_Saccade_percentage" + delimiter;

        header += "StartPosition_x" + delimiter;
        header += "StartPosition_y" + delimiter;
        header += "HeadsetNormedStartPosition_x" + delimiter;
        header += "HeadsetNormedStartPosition_y" + delimiter;

        header += "EndPosition_x" + delimiter;
        header += "EndPosition_y" + delimiter;
        header += "HeadsetNormedEndPosition_x" + delimiter;
        header += "HeadsetNormedEndPosition_y" + delimiter;

        header += "Saccade_Duration" + delimiter;
        header += "Saccade_Magnitude" + delimiter;
        
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
        string datasetLine = saccade.SaccadeNumber + delimiter;
        datasetLine += saccade.StartingTime.ToString("F3") + delimiter; // convert with 3 digits after decimal point

        datasetLine += saccade.Saccade1Pattern + delimiter;
        datasetLine += (rotationMeasurement.pattern1SaccadeTotal * 1.00f / saccade.SaccadeNumber).ToString() + delimiter;
        datasetLine += saccade.Saccade2Pattern + delimiter;
        datasetLine += (rotationMeasurement.pattern2SaccadeTotal * 1.00f / saccade.SaccadeNumber).ToString() + delimiter;

        datasetLine += saccade.StartPosition.x.ToString("F2") + delimiter;
        datasetLine += saccade.StartPosition.y.ToString("F2") + delimiter;
        datasetLine += saccade.HeadsetNormedStartPosition.x.ToString("F2") + delimiter;
        datasetLine += saccade.HeadsetNormedStartPosition.y.ToString("F2") + delimiter;

        datasetLine += saccade.EndPosition.x.ToString("F3") + delimiter;
        datasetLine += saccade.EndPosition.y.ToString("F3") + delimiter;
        datasetLine += saccade.HeadsetNormedEndPosition.x.ToString("F3") + delimiter;
        datasetLine += saccade.HeadsetNormedEndPosition.y.ToString("F3") + delimiter;

        datasetLine += saccade.Duration.ToString("F3") + delimiter;
        datasetLine += saccade.Magnitude.ToString("F2") + delimiter;

        if (!String.IsNullOrEmpty(msgBuffer))
        {
            datasetLine += msgBuffer + delimiter;
            msgBuffer = "";
        }
        trackingDataQueue.Enqueue(datasetLine);
    }

    void WriteTrackingData()
    {
        //Debug.Log("Writing Data; fps: " + 1.0f / Time.deltaTime);
        StreamWriter sw = new StreamWriter(directory + trackingFile, true); //true for append
        string datasetLine;
        // dequeue trackingDataQueue until empty
        while (trackingDataQueue.Count > 0)
        {
            datasetLine = trackingDataQueue.Dequeue().ToString();
            sw.WriteLine(datasetLine); // write to file
        }
        sw.Close(); // close file
    }
}
