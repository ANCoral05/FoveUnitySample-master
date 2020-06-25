using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class PlayerCollisionCounter : MonoBehaviour
{
    public GameObject cam;

    public int obstacleCollision;

    public List<int> moving;

    public float timer;

    public GameObject environment;

    public GameObject mainEnvironment;

    public List<Vector3> position;

    public List<float> time;

    public bool save;

    public string datasetLine;

    public float knockbackTimer;

    public Vector3 knockbackVector;

    public NavigationControls navigationControls;

    private float realWalkingSpeed;

    //Data storage

    [Header("Output Settings")]
    public int participantNumber = 0;
    public string trackingFile = "CollisionTracking.csv"; // maybe change to automatically get subject dependend name?
    public string delimiter = ";";

    public string directory;
    Queue trackingDataQueue = new Queue();
    static string msgBuffer = "";

    // Start is called before the first frame update
    void Start()
    {
        obstacleCollision = 0;

        directory = "Assets/Resources/Participant_" + participantNumber.ToString() + "_Date_" + System.DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + "/";
        Directory.CreateDirectory(directory);
        WriteHeader();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y - 1f, cam.transform.position.z);

        timer += Time.deltaTime;

        if(knockbackTimer > 0)
        {
            environment.transform.Translate(Time.deltaTime * 3 * realWalkingSpeed * knockbackTimer * new Vector3(knockbackVector.x* -Mathf.Cos(mainEnvironment.transform.eulerAngles.y*Mathf.Deg2Rad)+ knockbackVector.z * -Mathf.Sin(mainEnvironment.transform.eulerAngles.y * Mathf.Deg2Rad), 0, knockbackVector.x * Mathf.Sin(mainEnvironment.transform.eulerAngles.y * Mathf.Deg2Rad) + knockbackVector.z * Mathf.Cos(mainEnvironment.transform.eulerAngles.y * Mathf.Deg2Rad)));

            knockbackTimer -= Time.deltaTime;

            navigationControls.realWalkingSpeed = 0;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "staticObstacle" && timer >= 0.1f)
        {
            obstacleCollision += 1;

            moving.Add(0);

            position.Add(Vector3.zero - environment.transform.localPosition);

            time.Add(Time.time);

            realWalkingSpeed = navigationControls.realWalkingSpeed;

            knockbackTimer = 0.5f;

            knockbackVector = Vector3.Normalize(collision.contacts[0].point - transform.position);

            timer = 0;
        }

        if (collision.gameObject.tag == "movingObstacle" && timer >= 0.1f)
        {
            obstacleCollision += 1;

            moving.Add(1);

            position.Add(Vector3.zero - environment.transform.localPosition);

            time.Add(Time.time);

            realWalkingSpeed = navigationControls.realWalkingSpeed;

            knockbackTimer = 0.5f;

            knockbackVector = Vector3.Normalize(collision.contacts[0].point - transform.position);

            timer = 0;
        }
        if (collision.gameObject.tag == "goal" && !save)
        {
            WriteTrackingData();

            save = true;
        }
    }

    void WriteHeader()
    {
        StreamWriter sw = new StreamWriter(directory + trackingFile);
        string header = "Time" + delimiter;

        header += "CollisionNumber" + delimiter;
        header += "CollisionPosition" + delimiter;
        header += "CollisionTime" + delimiter;
        header += "CollisionType" + delimiter;

        sw.WriteLine(header);
        sw.Close();
    }

    public static void Msg(string msg)
    {
        msgBuffer = msg;
    }

    void WriteTrackingData()
    {
        print("Writing data now!");

        for(int i = 0; i < obstacleCollision; i++)
        {
            datasetLine = Time.time.ToString("F2") + delimiter;

            datasetLine += i+1 + delimiter;
            datasetLine += position[i] + delimiter;
            datasetLine += time[i] + delimiter;
            datasetLine += moving[i] + delimiter;

            if (!String.IsNullOrEmpty(msgBuffer))
            {
                datasetLine += msgBuffer + delimiter;
                msgBuffer = "";
            }
            trackingDataQueue.Enqueue(datasetLine);
        }

        StreamWriter sw = new StreamWriter(directory + trackingFile, true);

        while (trackingDataQueue.Count > 0)
        {
            datasetLine = trackingDataQueue.Dequeue().ToString();
            sw.WriteLine(datasetLine);
        }
        sw.Close();
    }
}
