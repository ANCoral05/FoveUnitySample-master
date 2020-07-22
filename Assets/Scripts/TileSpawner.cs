using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class TileSpawner : MonoBehaviour
{
    public int participantNumber = 0;
    public string trackingFile = "MapLayout.csv"; // maybe change to automatically get subject dependend name?
    public string delimiter = ";";
    Queue trackingDataQueue = new Queue();
    static string msgBuffer = "";
    public string directory;
    public string datasetLine;

    public GameObject[] tiles;
    public GameObject goal;
    public GameObject[] obstacles;
    public GameObject environment;

    public GameObject spawner;
    private GameObject tile;
    public GameObject[] obstacleSpawner;
    private int obstacleCounter;
    private bool findObstacles;
        
    public List<string> tileNames;
    public List<string> obstacleNames;

    public RotationMeasurement rotationMeasurement;

    public int tileCounter;
    // Start is called before the first frame update
    void Start()
    {
        spawner = GameObject.FindGameObjectWithTag("TileSpawner");

        rotationMeasurement = GameObject.FindObjectOfType<RotationMeasurement>();
    }

    // Update is called once per frame
    void Update()
    {
        while (tileCounter < tiles.Length)
        {
            int random = UnityEngine.Random.Range(0, tiles.Length);

            if (tiles[random] != null)
            {
                tile = Instantiate(tiles[random], spawner.transform.position, spawner.transform.rotation, environment.transform);

                tileNames.Add(tile.name);

                tileCounter += 1;

                foreach(Transform child in tile.transform)
                {
                    if (child.tag == "TileSpawner")
                        spawner = child.gameObject;
                }

                tiles[random] = null;
            }

        }
        if (tileCounter == tiles.Length && goal != null)
        {
            foreach (Transform child in tile.transform)
            {
                if (child.tag == "TileSpawner")
                    spawner = child.gameObject;
            }

            tile = Instantiate(goal, spawner.transform.position, spawner.transform.rotation, environment.transform);

            tileNames.Add("goal");

            directory = rotationMeasurement.directory;

            WriteHeader();

            goal = null;
        }

        if(goal == null && !findObstacles)
        {
            obstacleSpawner = GameObject.FindGameObjectsWithTag("ObstacleSpawner");

            foreach(GameObject singleObstacleSpawner in obstacleSpawner)
            {
                obstacleNames.Add(singleObstacleSpawner.transform.GetChild(0).name);
            }

            WriteTrackingData();

            findObstacles = true;
        }
    }

    void WriteHeader()
    {
        StreamWriter sw = new StreamWriter(directory + trackingFile);
        string header = "Number" + delimiter;
        header += "Tile" + delimiter;
        header += "Obstacle" + delimiter;

        sw.WriteLine(header);
        sw.Close();
    }

    public static void Msg(string msg)
    {
        msgBuffer = msg;
    }

    void WriteTrackingData()
    {
        trackingDataQueue.Enqueue(datasetLine);

        for (int i = 0; i < obstacleSpawner.Length; i++)
        {
            datasetLine = i + 1 + delimiter;
            if (i < tileNames.Count)
            {
                datasetLine += tileNames[i] + delimiter;
            }
            else
            {
                datasetLine += "" + delimiter;
            }

            datasetLine += obstacleNames[i+1] + delimiter;

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
