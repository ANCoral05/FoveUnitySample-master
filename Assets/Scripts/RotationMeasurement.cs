using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class SaccadeType
{
    public Vector3 Start;
    public Vector3 End;
    public float Magnitude;

    public SaccadeType (Vector3 start, Vector3 end, float magnitude)
    {
        Start = start;
        End = end;
        Magnitude = magnitude;
    }
}

public class RotationMeasurement : FOVEBehavior
{
    [Header("Output Settings")]
    public int participantNumber = 0;
    public string trackingFile = "SaccadeTracking.csv"; // maybe change to automatically get subject dependend name?
    public string delimiter = ";";
    [Header("Eye Tracking")]
    public bool saveEyeTracking = false;
    public bool foveGazeDirection = true;
    public GameObject saccadePointRight;
    public GameObject saccadePointLeft;
    private float xAngle;
    private float yAngle;
    public Vector2 totalAngles;
    public Vector2 headsetAngles;
    public Vector2 difference;
    [Range(0,10), Tooltip("The smallest angle that is detected as a saccade.")]
    public float minimalValidSaccadeDistance;
    public Vector3 SaccadeGazeAngleTrackerRightOld;
    public Vector3 SaccadeGazeAngleTrackerRightNew;
    public List<SaccadeType> saccadeList = new List<SaccadeType>();
    private Vector3 angleVector;
    public float angleDistance;
    public AudioSource saccadeTestSound;
    [Header("Object Tracking")]
    public GameObject trackedGameobject;
    public Camera mainCamera;
    public bool savePosition = true;
    public bool saveOrientation = true;
    public int saccadeNumber;
    public int pattern1Saccade;
    public int pattern2Saccade;
    public GameObject screen;

    public DistanceFromSaccadePathSound saccadePath;
    public string directory;

    private static float wrapAngle(float angle)
    {
        angle %= 360;
        if (angle > 180)
            return angle - 360;
        else
            return angle;
    }

    Queue trackingDataQueue = new Queue();
    static string msgBuffer = "";
    // Start is called before the first frame update
    void Start()
    {
        directory = "Assets/Resources/Participant_" + participantNumber.ToString() + "_Date_" + System.DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + "/";
        Directory.CreateDirectory(directory);
        WriteHeader();
        InvokeRepeating("WriteTrackingData", 0.0f, 1.0f); // save data to file every second
    }

    void Update()
    {
        angleVector = Vector3.Normalize(saccadePointRight.transform.position - mainCamera.transform.position);
        //SaccadeGazeAngleTrackerRightNew = new Vector2(angleVector.x, angleVector.y);
        SaccadeGazeAngleTrackerRightNew = angleVector;
        angleDistance = Vector3.Angle(SaccadeGazeAngleTrackerRightNew, SaccadeGazeAngleTrackerRightOld);
        //Vector3 movementVector = SaccadeGazeAngleTrackerRightNew - SaccadeGazeAngleTrackerRightOld;
        xAngle = (SaccadeGazeAngleTrackerRightNew.x - SaccadeGazeAngleTrackerRightOld.x)*Mathf.Rad2Deg;
        yAngle = (SaccadeGazeAngleTrackerRightNew.y - SaccadeGazeAngleTrackerRightOld.y)*Mathf.Rad2Deg;
        totalAngles = new Vector2(Mathf.Asin(SaccadeGazeAngleTrackerRightNew.x) * Mathf.Rad2Deg, Mathf.Asin(SaccadeGazeAngleTrackerRightNew.y) * Mathf.Rad2Deg);
        headsetAngles = new Vector2(wrapAngle(mainCamera.transform.rotation.eulerAngles.y), -wrapAngle(mainCamera.transform.rotation.eulerAngles.x));

        if (angleDistance > minimalValidSaccadeDistance)
        {
            saccadeList.Add(new SaccadeType(SaccadeGazeAngleTrackerRightNew, SaccadeGazeAngleTrackerRightOld, angleDistance));
            //saccadeTestSound.Play();
        }
        SaccadeGazeAngleTrackerRightOld = SaccadeGazeAngleTrackerRightNew;
        QueueTrackingData();
        difference = totalAngles - headsetAngles;
        print("X: " + difference.x + ", Y: " + difference.y);

        //if(saccadePath.startTime > 0)
        {
            if (angleDistance > minimalValidSaccadeDistance)
            {
                //print("Time: " + Time.time + "; Saccade Amplitude: " + angleDistance + "; Shift: " + new Vector2(xAngle, yAngle).ToString("F2") + "; Start position:" + saccadeList[saccadeList.Count - 1].Start.ToString("F2") + "; End position: " + saccadeList[saccadeList.Count - 1].End.ToString("F2"));

                saccadeNumber += 1;



                if(Mathf.Abs(xAngle) > Mathf.Abs(3*yAngle) || (Mathf.Abs(yAngle) > Mathf.Abs(xAngle) && (Mathf.Abs(SaccadeGazeAngleTrackerRightNew.x*Mathf.Rad2Deg) > 8*saccadePath.horizontalAngle)))
                {
                    pattern1Saccade += 1;
                    Msg("Pattern 1 saccade");
                }
                else
                {
                    Msg("No pattern saccade");
                }
            }
        }
    }
    void WriteHeader()
    {
        StreamWriter sw = new StreamWriter(directory + trackingFile);
        string header = "Timestamp" + delimiter;
        if (foveGazeDirection)
        {
            //header += "EyeData" + delimiter;            
            //header += trackedGameobject.name + "_gazeDirection_right" + delimiter;
            //header += trackedGameobject.name + "_gazeDirection_left" + delimiter;

            header += "Total_Angles_x" + delimiter;
            header += "Total_Angles_y" + delimiter;

            header += "Head_Rotation_x" + delimiter;
            header += "Head_Rotation_y" + delimiter;

            //header += "SaccadeMagnitude" + delimiter;
            //header += "SaccadeHorizontalAngle" + delimiter;
            //header += "SaccadeVerticalAngle" + delimiter;
        }
        if (savePosition)
            header += trackedGameobject.name + "_position" + delimiter;
        if (saveOrientation)
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
        //tracked gaze direction
        if (foveGazeDirection)
        {
            //datasetLine += FoveInterface.GetGazeRays().right.ToString("F3") + delimiter;
            //datasetLine += FoveInterface.GetGazeRays().left.ToString("F3") + delimiter;

            datasetLine += totalAngles.x + delimiter;
            datasetLine += totalAngles.y + delimiter;

            datasetLine += headsetAngles.x + delimiter;
            datasetLine += headsetAngles.y + delimiter;
        }
        // tracked game object's position
        if (savePosition)
            datasetLine += trackedGameobject.transform.position.ToString("F2") + delimiter;
        // tracked game object's orientation
        if (saveOrientation)
            datasetLine += trackedGameobject.transform.eulerAngles.ToString("F2") + delimiter;
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
        //Debug.Log("End Writing Data");
    }
}
