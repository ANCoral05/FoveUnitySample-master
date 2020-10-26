using System.Collections;
using System.Collections.Generic;
using Fove.Unity;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.IO;

public class NewSetNumbers : FOVEBehavior
{
    public int participantNumber = 0;
    public string trackingFile = "SaccadeTracking.csv"; // maybe change to automatically get subject dependend name?
    public string delimiter = ";";
    Queue trackingDataQueue = new Queue();
    static string msgBuffer = "";
    public RotationMeasurement rotationMeasurement;
    public string directory;
    public string datasetLine;
    public AudioSource audioSource;
    public AudioClip upClip;
    public AudioClip downClip;
    public TexturePaint texturePainter;

    private float timeTracker;

    public List<float> targetTime;

    public List<Vector3> targetPosition;

    public TextMesh TargetText;

    public Transform Screen;

    public GameObject Infotext;

    [Range(0, 60)]
    public int TargetAmount;

    [Range(0, 100), Tooltip("The size of the field in which target numbers will appear.")]
    public float fieldScale;

    private Vector2 placement;

    public Vector3 positionVector;

    public int TargetNumber;

    public int CorrectTargetNumber;

    public Collider StartingCollider;

    public int step;

    public Image ProgressCircle;

    public Image ProgressCircleEdge;

    public float loadingProgress;

    public GameObject canvas;

    private Collider[] nearbyColliders;

    private bool SetCoroutine;

    public AudioSource audioSourceLoad;

    public AudioSource audioSourceBling;

    public AudioClip loading;

    public AudioClip loaded;

    public Vector2 coordinates;

    public float minDistance;

    //public SaveOnLoad saveOnLoad;

    public int averageCorrectTargets = 0;

    private int currentCorrectTargets;

    public int currentTargets;

    public int spottedTargets;

    public float startTime;

    public float endTime;

    public Controls controls;

    private float waitForLag;

    // Start is called before the first frame update
    void Awake()
    {
        controls = new Controls();

        //saveOnLoad = FindObjectOfType<SaveOnLoad>();
    }


    private void Start()
    {
        startTime = 0;

        waitForLag = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //if (saveOnLoad.stage == 4 && !canvas.activeSelf)
        //{
        //    canvas.SetActive(true);

        //    Infotext.SetActive(true);

        //    saveOnLoad.stage = 5;
        //}

        if (step == 0)
        {
            canvas.SetActive(true);

            //GameObject[] targets = GameObject.FindGameObjectsWithTag("TargetNumber");

            //foreach (GameObject target in targets) GameObject.Destroy(target);

            waitForLag += Time.deltaTime;

            Infotext.GetComponent<TextMesh>().text = "Weiter in \n" + (4-Mathf.RoundToInt(waitForLag)).ToString();

            //if(saveOnLoad.stage > 3)
            //    saveOnLoad.stage = 5;

            if (waitForLag > 4)
            {
                step = 1;

                TargetNumber = UnityEngine.Random.Range(0, 10);

                Infotext.GetComponent<TextMesh>().text = "Suche: \n" + TargetNumber;
            }
        }

        if (FoveInterface.Gazecast(StartingCollider) && step == 1)
        {
            audioSourceLoad.clip = loading;

            audioSourceLoad.volume = 0.1f + loadingProgress * 0.02f;

            if (!audioSourceLoad.isPlaying)
            {
                audioSourceLoad.Play();
            }

            loadingProgress += Time.deltaTime * 0.5f;
        }
        else
        {
            loadingProgress = 0;

            audioSourceLoad.Stop();
        }

        ProgressCircle.fillAmount = loadingProgress;

        ProgressCircleEdge.transform.rotation = Quaternion.Euler(0, 0, -360 * loadingProgress);

        ProgressCircleEdge.color = new Color(1, 1, 1, loadingProgress * 36);

        if (loadingProgress >= 1)
        {
            step = 2;

            canvas.SetActive(false);

            Infotext.SetActive(false);
        }

        if (step == 2)
        {
            audioSourceBling.volume = 0.1f;

            audioSourceBling.PlayOneShot(loaded);

            //print("before" + Time.realtimeSinceStartup * 1000);

            //placement = Random.insideUnitCircle;

            //nearbyColliders = Physics.OverlapSphere(new Vector3(Screen.position.x, Screen.position.y + placement.x * 1.4f, Screen.position.z + placement.y * 1.4f), 0.07f);

            //if (nearbyColliders.Length == 0)
            //{
            //    Instantiate(TargetText, Screen.position + new Vector3(0, placement.x * 1.4f, placement.y * 1.4f), Quaternion.Euler(0, 90, 0));

            //    TargetText.text = Random.Range(0, 10).ToString();
            //}

            if (averageCorrectTargets == 0)
                averageCorrectTargets = UnityEngine.Random.Range(0, 6);

            InvokeRepeating("PlaceNumbers", 0, 0.001f);

            //GameObject[] targets = GameObject.FindGameObjectsWithTag("TargetNumber");

            if (currentTargets == TargetAmount)
            {
                CancelInvoke("PlaceNumbers");
                step = 3;

                //print("after" + Time.realtimeSinceStartup * 1000);
            }
            //if (targets.Length == TargetAmount)
            //{
            //    foreach (GameObject target in targets)
            //        if (target.GetComponent<TextMesh>().text == TargetNumber.ToString())
            //        {
            //            //print("Found one!" + target.transform.name);

            //            CorrectTargetNumber += 1;
            //        }

                //print("Target number:" + TargetNumber + "; Targets: " + CorrectTargetNumber);


            //}
        }

        if (step == 3)
        {
            if (startTime == 0)
                startTime = Time.time;

            texturePainter.startMeasurement = true;

            GameObject[] targets = GameObject.FindGameObjectsWithTag("TargetNumber");

            if (controls.Search.Up.triggered)
            {
                audioSource.PlayOneShot(upClip);

                spottedTargets += 1;
            }

            if (controls.Search.Down.triggered)
            {
                spottedTargets -= 1;

                audioSource.PlayOneShot(downClip);
            }
            if (controls.Search.Next.triggered || Time.time > startTime + 20f)
            {
                foreach (GameObject target in targets) GameObject.Destroy(target);

                //saveOnLoad.stage = 6;

                endTime = Time.time - startTime;

                //canvas.SetActive(true);

                //Infotext.SetActive(true);

                //CorrectTargetNumber = 0;

                //averageCorrectTargets = 0;

                //currentTargets = 0;

                //currentCorrectTargets = 0;

                //spottedTargets = 0;

                //startTime = 0;

                //step = 0;

                step = 4;
            }

        }

        if(step == 4)
        {
            if (timeTracker == 0)
                timeTracker = Time.time;

            texturePainter.searchFinished = true;

            texturePainter.startMeasurement = false;

            directory = rotationMeasurement.directory;

            Directory.CreateDirectory(directory);
            WriteHeader();

            WriteTrackingData();

            print("Writing Numbers.");

            if (timeTracker + 0.2f < Time.time)
            {
                timeTracker = 1f;

                step = 5;
            }
        }

        if(step == 5)
        {
            timeTracker -= Time.deltaTime;
            if (timeTracker <= 0)
            {
                Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
            }
        }
    }

    void PlaceNumbers()
    {
        //GameObject[] targets = GameObject.FindGameObjectsWithTag("TargetNumber");

        if (currentTargets < TargetAmount)
        {
            coordinates.x = UnityEngine.Random.Range(95.0f-fieldScale*0.55f, 95.0f+fieldScale*0.55f);

            coordinates.y = UnityEngine.Random.Range(90.0f-fieldScale*0.9f, 90.0f + fieldScale * 0.9f);

            positionVector = new Vector3(
                Screen.transform.position.x + (2.72f * Mathf.Sin(coordinates.x * Mathf.Deg2Rad) * Mathf.Cos(coordinates.y * Mathf.Deg2Rad)), 
                Screen.transform.position.y + (2.72f * Mathf.Cos(coordinates.x * Mathf.Deg2Rad)), 
                Screen.transform.position.z + (2.72f * Mathf.Sin(coordinates.x * Mathf.Deg2Rad) * Mathf.Sin(coordinates.y * Mathf.Deg2Rad)));

            nearbyColliders = Physics.OverlapSphere(positionVector, minDistance);

            if (nearbyColliders.Length == 1)
            {
                Instantiate(TargetText, positionVector, Quaternion.LookRotation(positionVector - Screen.transform.position));

                if (currentCorrectTargets < averageCorrectTargets)
                {
                    TargetText.text = TargetNumber.ToString();

                    currentCorrectTargets += 1;
                }
                else
                {
                    TargetText.text = UnityEngine.Random.Range(0, 10).ToString();

                    while (TargetText.text == TargetNumber.ToString())
                        TargetText.text = UnityEngine.Random.Range(0, 10).ToString();
                }
                currentTargets += 1;
            }
        }
    }

    void WriteHeader()
    {
        StreamWriter sw = new StreamWriter(directory + trackingFile);
        string header = "Time of decision" + delimiter;
        header += "actualTargets" + delimiter;
        header += "spottedTargets" + delimiter;
        header += "totalArea" + delimiter;
        header += "averageArea1" + delimiter;
        header += "averageArea3" + delimiter;
        header += "averageArea5" + delimiter;
        header += "averageArea10" + delimiter;

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

        datasetLine = endTime.ToString("F2") + delimiter;
        datasetLine += averageCorrectTargets + delimiter;
        datasetLine += + spottedTargets + delimiter;
        datasetLine += texturePainter.totalPercentage.ToString("F2") + delimiter;
        datasetLine += texturePainter.averageGazeAreaNumberOne.ToString("F2") + delimiter;
        datasetLine += texturePainter.averageGazeAreaNumberThree.ToString("F2") + delimiter;
        datasetLine += texturePainter.averageGazeAreaNumberFive.ToString("F2") + delimiter;
        datasetLine += texturePainter.averageGazeAreaNumberTen.ToString("F2") + delimiter;
        trackingDataQueue.Enqueue(datasetLine);

        //for (int i = 0; i < spottedTargets; i++)
        //{
        //    datasetLine = i + 1 + delimiter;
        //    datasetLine += targetTime[i] + delimiter;
        //    datasetLine += targetPosition[i] + delimiter;

        //    if (!String.IsNullOrEmpty(msgBuffer))
        //    {
        //        datasetLine += msgBuffer + delimiter;
        //        msgBuffer = "";
        //    }
        //    trackingDataQueue.Enqueue(datasetLine);
        //}

        StreamWriter sw = new StreamWriter(directory + trackingFile, true);

        while (trackingDataQueue.Count > 0)
        {
            datasetLine = trackingDataQueue.Dequeue().ToString();
            sw.WriteLine(datasetLine);
        }
        sw.Close();
    }

    private void OnEnable()
    {
        controls.Search.Enable();
    }

    private void OnDisable()
    {
        controls.Search.Disable();
    }
}