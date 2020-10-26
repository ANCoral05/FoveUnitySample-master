using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class EyeMovementFrame
{
    public int FrameNumber;
    public Vector2 Angles;
    public float Time;
    public int Saccade;
    public int SaccadeLeftRight;
    public int SaccadeRadial;
    public Vector2 HeadsetAngles;
    public Vector3 HeadsetPosition;

    public EyeMovementFrame(int frameNumber, Vector2 angles, float time, int saccade, int saccadeLeftRight, int saccadeRadial, Vector2 headsetAngles, Vector3 headsetPosition)
    {
        FrameNumber = frameNumber;
        Angles = angles;
        Time = time;
        Saccade = saccade;
        SaccadeLeftRight = saccadeLeftRight;
        SaccadeRadial = saccadeRadial;
        HeadsetAngles = headsetAngles;
        HeadsetPosition = headsetPosition;
    }
}

public class Saccade
{
    public int SaccadeNumber;
    public Vector2 StartPosition;
    public Vector2 HeadsetNormedStartPosition;
    public Vector2 EndPosition;
    public Vector2 HeadsetNormedEndPosition;
    public int Saccade1Pattern;
    public int Saccade2Pattern;
    public float StartingTime;
    public float Duration;
    public float Magnitude;

    public Saccade(int saccadeNumber, Vector2 start, Vector2 headsetNormedStart, Vector2 end, Vector2 headsetNormedEnd, int saccade1Pattern, int saccade2Pattern, float startingTime, float duration, float magnitude)
    {
        SaccadeNumber = saccadeNumber;
        StartPosition = start;
        HeadsetNormedStartPosition = headsetNormedStart;
        Saccade1Pattern = saccade1Pattern;
        Saccade2Pattern = saccade2Pattern;
        EndPosition = end;
        HeadsetNormedEndPosition = headsetNormedEnd;
        StartingTime = startingTime;
        Duration = duration;
        Magnitude = magnitude;
    }
}

public class RotationMeasurement : FOVEBehavior
{
    [Header("Output Settings")]
    public int participantNumber = 0;
    public string trackingFile = "EyeTracking.csv"; // maybe change to automatically get subject dependend name?
    public string delimiter = ";";

    [Header("Eye Tracking")]
    public bool saveEyeTracking = false;
    public bool foveGazeDirection = true;
    public GameObject saccadePointRight;
    public GameObject saccadePointLeft;
    private float xAngle;
    private float yAngle;

    public List<EyeMovementFrame> eyeMovementFrameList = new List<EyeMovementFrame>();
    public Saccade saccade;

    public Vector2 totalAngles;
    public Vector2 totalAnglesOld;
    public Vector2 headsetAngles;
    public Vector2 difference;
    public int frameNumber;
    public int isSaccade;
    public Vector3 headsetPosition;
    public int saccadeStart;
    private Vector2 oldDirection;
    private Vector2 newDirection;

    [Range(1,200), Tooltip("The lowest angular velocity that is detected as a saccade.")]
    public float minimalValidSaccadeDistance;
    public Vector3 SaccadeGazeAngleTrackerRightOld;
    public Vector3 SaccadeGazeAngleTrackerRightNew;
    private Vector3 angleVector;
    public List<Vector2> angleVectors = new List<Vector2>();
    public float angleDistance;
    [Header("Object Tracking")]
    public GameObject trackedGameobject;
    public Camera mainCamera;
    public bool savePosition = true;
    public bool saveOrientation = true;
    public int saccadeNumber;
    public int pattern1Saccade = 0;
    public int pattern1SaccadeTotal;
    public int pattern2Saccade = 0;
    public int pattern2SaccadeTotal;
    public GameObject screen;
    public GameObject environment;
    public GameObject fullEnvironment;
    public bool save;

    private int saccade1Tracker;
    private int saccade2Tracker;

    public DistanceFromSaccadePathSound saccadePath;
    private ManagerScript manager;
    public PlayerCollisionCounter collisionScript;
    public string directory;
    private string visualTask;
    private string scanningPattern;

    public AudioSource audioSource;

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
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<ManagerScript>();
        collisionScript = FindObjectOfType<PlayerCollisionCounter>();

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

    void Update()
    {
        frameNumber += 1;
        angleVector = Vector3.Normalize(saccadePointRight.transform.position - mainCamera.transform.position);
        SaccadeGazeAngleTrackerRightNew = angleVector;
        angleDistance = Vector3.Angle(SaccadeGazeAngleTrackerRightNew, SaccadeGazeAngleTrackerRightOld);
        xAngle = (SaccadeGazeAngleTrackerRightNew.x - SaccadeGazeAngleTrackerRightOld.x)*Mathf.Rad2Deg;
        yAngle = (SaccadeGazeAngleTrackerRightNew.y - SaccadeGazeAngleTrackerRightOld.y)*Mathf.Rad2Deg;
        totalAngles = new Vector2(Mathf.Asin(SaccadeGazeAngleTrackerRightNew.x) * Mathf.Rad2Deg, Mathf.Asin(SaccadeGazeAngleTrackerRightNew.y) * Mathf.Rad2Deg);
        totalAnglesOld = new Vector2(Mathf.Asin(SaccadeGazeAngleTrackerRightOld.x) * Mathf.Rad2Deg, Mathf.Asin(SaccadeGazeAngleTrackerRightOld.y) * Mathf.Rad2Deg);
        headsetAngles = new Vector2(wrapAngle(mainCamera.transform.rotation.eulerAngles.y), -wrapAngle(mainCamera.transform.rotation.eulerAngles.x));
        headsetPosition = mainCamera.transform.position;

        if (angleDistance > minimalValidSaccadeDistance * Time.deltaTime)
        {
            if (isSaccade == 0)
            {
                saccadeStart = frameNumber - 1;

                isSaccade = 1;

                msgBuffer = "SaccadeStart";
            }
            else
                msgBuffer = null;

            if (Mathf.Abs(xAngle) > Mathf.Abs(3 * yAngle) || (Mathf.Abs(yAngle) > Mathf.Abs(xAngle) && (Mathf.Abs(SaccadeGazeAngleTrackerRightNew.x * Mathf.Rad2Deg) > 4 * saccadePath.horizontalAngle)))
            {
                pattern1Saccade = 1;
            }
            else
                pattern1Saccade = 0;

            if (Mathf.Sin(totalAngles.x / totalAngles.y) < 0.15f + Mathf.Sin(totalAnglesOld.x / totalAnglesOld.y) || Mathf.Sin(totalAngles.x / totalAngles.y) > Mathf.Sin(totalAnglesOld.x / totalAnglesOld.y) - 0.15f || (Mathf.Abs(totalAngles.magnitude - totalAnglesOld.magnitude) > 10 && Mathf.Min(totalAngles.magnitude, totalAnglesOld.magnitude) < 10))
            {
                pattern2Saccade = 1;
            }
            else
                pattern2Saccade = 0;
        }
        else
        {
            pattern1Saccade = 0;
            pattern2Saccade = 0;
            msgBuffer = null;
        }

        eyeMovementFrameList.Add(new EyeMovementFrame(frameNumber, totalAngles, Time.time, isSaccade, pattern1Saccade, pattern2Saccade, headsetAngles, headsetPosition));

        if (frameNumber > 2)
            oldDirection = eyeMovementFrameList[frameNumber - 3].Angles - eyeMovementFrameList[frameNumber - 2].Angles;

        if (frameNumber > 1)
            newDirection = eyeMovementFrameList[frameNumber - 2].Angles - eyeMovementFrameList[frameNumber - 1].Angles;

        if (isSaccade == 1 && saccadeStart != frameNumber - 1 && (angleDistance < minimalValidSaccadeDistance * Time.deltaTime || Vector2.Angle(oldDirection, newDirection) > 30f || Time.time - saccadeStart > 0.1f))
        {
            Vector2 saccadeVector = new Vector2(eyeMovementFrameList[frameNumber -1].Angles.x - eyeMovementFrameList[saccadeStart].Angles.x, eyeMovementFrameList[frameNumber - 1].Angles.y - eyeMovementFrameList[saccadeStart].Angles.y);

            Vector2 headsetNormedVector = totalAngles - headsetAngles;

            //float test = saccadeVector.magnitude / (eyeMovementFrameList[frameNumber-1].Time - eyeMovementFrameList[saccadeStart].Time);

            //print("SaccadeStart: " + eyeMovementFrameList[saccadeStart].Time + "; SaccadeEnd: " + eyeMovementFrameList[frameNumber-1].Time + "; Velocity: " + test + "; Saccade Vector: " + saccadeVector.magnitude);

            if (saccadeVector.magnitude / (eyeMovementFrameList[frameNumber - 1].Time - eyeMovementFrameList[saccadeStart].Time) > minimalValidSaccadeDistance && saccadeVector.magnitude > 1f)
            {
                //audioSource.Play();

                if (Mathf.Abs(saccadeVector.x) > Mathf.Abs(3 * saccadeVector.y) || (Mathf.Abs(saccadeVector.y) > Mathf.Abs(saccadeVector.x) && (Mathf.Abs(SaccadeGazeAngleTrackerRightNew.x * Mathf.Rad2Deg) > 4 * saccadePath.horizontalAngle)))
                {
                    saccade1Tracker = 1;

                    pattern1SaccadeTotal += 1;
                }
                else
                    saccade1Tracker = 0;

                float normedDistanceStart = (eyeMovementFrameList[saccadeStart].Angles - eyeMovementFrameList[saccadeStart].HeadsetAngles).magnitude;

                float normedDistanceEnd = (eyeMovementFrameList[frameNumber - 1].Angles - eyeMovementFrameList[frameNumber - 1].HeadsetAngles).magnitude;

                if ((normedDistanceStart < 15 && normedDistanceEnd > normedDistanceStart + 0.666f * angleDistance) || normedDistanceEnd < 15 && normedDistanceStart > normedDistanceEnd + 0.666f*angleDistance)
                {
                    saccade2Tracker = 1;

                    pattern2SaccadeTotal += 1;

                    //audioSource.Play();

                    //print(pattern2SaccadeTotal);
                }
                else
                    saccade2Tracker = 0;

                saccadeNumber += 1;
                saccade = new Saccade(saccadeNumber, eyeMovementFrameList[saccadeStart].Angles, eyeMovementFrameList[saccadeStart].Angles - eyeMovementFrameList[saccadeStart].HeadsetAngles, eyeMovementFrameList[frameNumber - 1].Angles, eyeMovementFrameList[frameNumber - 1].Angles - eyeMovementFrameList[frameNumber - 1].HeadsetAngles, saccade1Tracker, saccade2Tracker, eyeMovementFrameList[saccadeStart].Time, Time.time - eyeMovementFrameList[saccadeStart].Time, saccadeVector.magnitude);
            }
            isSaccade = 0;
        }

        float normedDistanceStart2 = (totalAngles - headsetAngles).magnitude;

        //print(headsetAngles + " - " + normedDistanceStart2);

        SaccadeGazeAngleTrackerRightOld = SaccadeGazeAngleTrackerRightNew;
        QueueTrackingData();
        difference = totalAngles - headsetAngles;

        if (collisionScript.save)
            save = true;

        if (save == true)
            CancelInvoke("WriteTrackingData");

        //{
        //    if (angleDistance > minimalValidSaccadeDistance * Time.deltaTime)
        //    {
        //        //print("Time: " + Time.time + "; Saccade Amplitude: " + angleDistance + "; Shift: " + new Vector2(xAngle, yAngle).ToString("F2") + "; Start position:" + saccadeList[saccadeList.Count - 1].Start.ToString("F2") + "; End position: " + saccadeList[saccadeList.Count - 1].End.ToString("F2"));

            //        saccadeNumber += 1;



            //        if(Mathf.Abs(xAngle) > Mathf.Abs(3*yAngle) || (Mathf.Abs(yAngle) > Mathf.Abs(xAngle) && (Mathf.Abs(SaccadeGazeAngleTrackerRightNew.x*Mathf.Rad2Deg) > 4*saccadePath.horizontalAngle)))
            //        {
            //            pattern1SaccadeTotal += 1;
            //            pattern1Saccade = 1;
            //        }
            //        else
            //        {
            //            pattern1Saccade = 0;
            //        }

            //        if (Mathf.Sin(totalAngles.x/totalAngles.y) < 0.15f+ Mathf.Sin(totalAnglesOld.x/totalAnglesOld.y) || Mathf.Sin(totalAngles.x / totalAngles.y) > Mathf.Sin(totalAnglesOld.x / totalAnglesOld.y) - 0.15f || (Mathf.Abs(totalAngles.magnitude - totalAnglesOld.magnitude) > 10 && Mathf.Min(totalAngles.magnitude, totalAnglesOld.magnitude) < 10))
            //        {
            //            pattern2SaccadeTotal += 1;
            //            pattern2Saccade = 1;
            //        }
            //        else
            //        {
            //            pattern2Saccade = 0;
            //        }
            //    }
            //    else
            //    {
            //        pattern1Saccade = 0;
            //        pattern2Saccade = 0;
            //    }
            //}
    }

    void WriteHeader()
    {
        StreamWriter sw = new StreamWriter(directory + trackingFile);
        string header = "Frame_Number" + delimiter;
        header += "Time" + delimiter;
        if (foveGazeDirection)
        {
            //header += "EyeData" + delimiter;            
            //header += trackedGameobject.name + "_gazeDirection_right" + delimiter;
            //header += trackedGameobject.name + "_gazeDirection_left" + delimiter;

            header += "Total_Angles_x" + delimiter;
            header += "Total_Angles_y" + delimiter;

            header += "isSaccade" + delimiter;
            header += "isSaccadeLeftRight" + delimiter;
            header += "isSaccaderRadial" + delimiter;

            header += "Head_Rotation_x" + delimiter;
            header += "Head_Rotation_y" + delimiter;
            header += "Head_Position" + delimiter;

            //header += "SaccadeMagnitude" + delimiter;
            //header += "SaccadeHorizontalAngle" + delimiter;
            //header += "SaccadeVerticalAngle" + delimiter;
        }
        //if (savePosition)
        //{
        //    header += trackedGameobject.name + "_localPosition" + delimiter;
        //    header += trackedGameobject.name + "_globalPosition" + delimiter;
        //}
        //if (saveOrientation)
        //{
        //    header += trackedGameobject.name + "_orientation" + delimiter;
        //    header += "World_orientation" + delimiter;
        //}

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
        string datasetLine = (frameNumber).ToString() + delimiter;
        datasetLine += Time.time.ToString("F3") + delimiter; // convert with 3 digits after decimal point
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

            datasetLine += isSaccade + delimiter;
            datasetLine += pattern1Saccade + delimiter;
            datasetLine += pattern2Saccade + delimiter;

            datasetLine += headsetAngles.x + delimiter;
            datasetLine += headsetAngles.y + delimiter;

            datasetLine += mainCamera.transform.position;
        }
        // tracked game object's position
        //if (savePosition)
        //{
        //    datasetLine += trackedGameobject.transform.position.ToString("F2") + delimiter;
        //    datasetLine += Vector3.zero - environment.transform.localPosition;
        //}
        //// tracked game object's orientation
        //if (saveOrientation)
        //{
        //    datasetLine += trackedGameobject.transform.eulerAngles.ToString("F2") + delimiter;
        //    datasetLine += fullEnvironment.transform.eulerAngles.y;
        //}
        //    // buffered message


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
