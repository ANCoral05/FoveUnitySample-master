using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DistanceFromSaccadePathSound : FOVEBehavior
{
    [Tooltip("Select 1, 2 or 3 to choose between the different scanning patterns. 0 - free; 1 - left-to-right; 2 - radial")]
    public int scanningPattern;

    [Range(1,120), Tooltip("Choose the resolution of the scanning pattern analysis. Higher value means more accuracy, but slower run speed.")]
    public int stepNumber;

    public GameObject Screen;

    public GameObject[] sphereList;

    public bool[] sphereBool;

    public float angle;

    [Range(1, 20)]
    public int horizontalAngle;

    [Range(1, 20)]
    public int verticalAngle;

    [Range(1, 100)]
    public int radialPatternRadius;

    public int missedTargets;

    public Vector3 ObjectVector;

    public Camera cam;

    public GameObject sphere;

    public Vector2 coordinates;

    public Vector3[] positionVector;

    public int currentSphere = 0;

    public AudioSource audioSource;

    public Vector2 XYAngles;

    public bool missedTargetsCalled;

    public RotationMeasurement rotationMeasurementScript;

    public TexturePaint textureScript;

    public GameObject scoreCanvas;

    public TextMesh scoreText;

    public float startTime;

    public float endTime;

    public int currentStage;

    public SaveOnLoad saveOnLoad;

    // Start is called before the first frame update
    void Start()
    {
        sphereList = new GameObject[stepNumber];

        sphereBool = new bool[stepNumber];

        positionVector = new Vector3[stepNumber];

        saveOnLoad = FindObjectOfType<SaveOnLoad>();

        startTime = 0;

        if (saveOnLoad.stage == 0 && scanningPattern != 0)
        {
            if (scanningPattern == 1)
            {
                for (int i = 0; i < stepNumber; i++)
                {
                    //coordinates = new Vector2((100-3*verticalAngle) + verticalAngle * Mathf.Floor(i / 10), (Mathf.Floor(i / 10) % 2) * (9*horizontalAngle+(i - Mathf.Floor(i / 10)) * horizontalAngle) + (Mathf.Floor(1 + i / 10) % 2) * (9*horizontalAngle - (i - (Mathf.Floor(i / 10))) * horizontalAngle));
                    coordinates = new Vector2((100 - 3 * verticalAngle) + verticalAngle * Mathf.Floor(i / 10), (Mathf.Floor(i / 10) % 2) * ((90 - 4.5f * horizontalAngle) + (i - Mathf.Floor(i / 10) * 10) * horizontalAngle) + (Mathf.Floor(1 + i / 10) % 2) * (90 + (4.5f * horizontalAngle) - (i - (Mathf.Floor(i / 10) * 10)) * horizontalAngle));


                    positionVector[i] = new Vector3(
                    Screen.transform.position.x + (2.72f * Mathf.Sin(coordinates.x * Mathf.Deg2Rad) * Mathf.Cos(coordinates.y * Mathf.Deg2Rad)),
                    Screen.transform.position.y + (2.72f * Mathf.Cos(coordinates.x * Mathf.Deg2Rad)),
                    Screen.transform.position.z + (2.72f * Mathf.Sin(coordinates.x * Mathf.Deg2Rad) * Mathf.Sin(coordinates.y * Mathf.Deg2Rad)));

                    GameObject newSphere = GameObject.Instantiate(sphere, positionVector[i], new Quaternion(0, 0, 0, 0));

                    sphereList[i] = newSphere;
                }
            }

            if (scanningPattern == 2)
            {
                for (int i = 0; i < stepNumber; i++)
                {
                    //    coordinates = new Vector2(12 * Mathf.Floor(i / 10), (Mathf.Floor(i / 10) % 2) * (180 - (i - Mathf.Floor(i / 10)) * 40) + (Mathf.Floor(1 + i / 10) % 2) * (180 + (i - (Mathf.Floor(i / 10))) * 40));
                    coordinates = new Vector2((i % 2) * (0) + (1 - i % 2) * radialPatternRadius * 0.9f, i * 360 / stepNumber);

                    positionVector[i] = new Vector3(
                    Screen.transform.position.x + (2.72f * Mathf.Sin(coordinates.x * Mathf.Deg2Rad) * Mathf.Cos(coordinates.y * Mathf.Deg2Rad)),
                    Screen.transform.position.y + (2.72f * Mathf.Sin(coordinates.x * Mathf.Deg2Rad) * Mathf.Sin(coordinates.y * Mathf.Deg2Rad)),
                    Screen.transform.position.z + (2.72f * Mathf.Cos(coordinates.x * Mathf.Deg2Rad)));

                    GameObject newSphere = GameObject.Instantiate(sphere, positionVector[i], new Quaternion(0, 0, 0, 0));

                    sphereList[i] = newSphere;

                }

            }

            if (scanningPattern == 3)
            {
                for (int i = 0; i < stepNumber; i++)
                {
                    coordinates = new Vector2(20 * Mathf.Floor(i / 10), (Mathf.Floor(i / 10) % 2) * (180 - (i - Mathf.Floor(i / 10)) * 20) + (Mathf.Floor(1 + i / 10) % 2) * (180 + (i - (Mathf.Floor(i / 10))) * 20));

                    positionVector[i] = new Vector3(
                    Screen.transform.position.x + (2.72f * Mathf.Sin(coordinates.x * Mathf.Deg2Rad) * Mathf.Cos(coordinates.y * Mathf.Deg2Rad)),
                    Screen.transform.position.y + (2.72f * Mathf.Sin(coordinates.x * Mathf.Deg2Rad) * Mathf.Sin(coordinates.y * Mathf.Deg2Rad)),
                    Screen.transform.position.z + (2.72f * Mathf.Cos(coordinates.x * Mathf.Deg2Rad)));

                    GameObject newSphere = GameObject.Instantiate(sphere, positionVector[i], new Quaternion(0, 0, 0, 0));

                    sphereList[i] = newSphere;
                }
            }

            for (int i = 0; i < stepNumber - 1; i++)
            {
                sphereList[i].transform.LookAt(sphereList[i + 1].transform);
            }

            LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.widthMultiplier = 0.01f;
            lineRenderer.positionCount = stepNumber;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.blue, 0.0f), new GradientColorKey(Color.cyan, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1, 0.0f), new GradientAlphaKey(1, 1.0f) });
            lineRenderer.colorGradient = gradient;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (saveOnLoad.stage == 0 && scanningPattern != 0)
        {
            if (startTime == 0 && currentSphere > 0)
            {
                startTime = Time.time;

                audioSource.Play();
            }

            LineRenderer lineRenderer = GetComponent<LineRenderer>();

            for (int i = currentSphere; i < stepNumber; i++)
            {
                lineRenderer.SetPosition(i - currentSphere, positionVector[i - currentSphere]);
            }

            for (int i = currentSphere; i < Mathf.Min(currentSphere + 3, stepNumber); i++)
            {
                ObjectVector = (positionVector[i] - cam.transform.position);

                //Vector3 ObjectVectorOld = (positionVector[Mathf.Max(0,currentSphere - 1)] - cam.transform.position);

                //Vector2 xAngle = new Vector2(positionVector[currentSphere].x - cam.transform.position.x, positionVector[currentSphere].z - cam.transform.position.z);

                //Vector2 yAngle = new Vector2(positionVector[currentSphere].y - cam.transform.position.y, positionVector[currentSphere].z - cam.transform.position.z);

                //XYAngles = new Vector2(Vector2.SignedAngle(new Vector2(FoveInterface.GetGazeRays().right.direction.x, FoveInterface.GetGazeRays().right.direction.z), xAngle), Vector2.SignedAngle(new Vector2(FoveInterface.GetGazeRays().right.direction.y, FoveInterface.GetGazeRays().right.direction.z), yAngle));

                angle = Vector3.Angle(ObjectVector, FoveInterface.GetGazeRays().right.direction);

                //float angleOld = Vector3.Angle(ObjectVector, FoveInterface.GetGazeRays().right.direction);

                if (angle < 5 && currentSphere < stepNumber)
                {
                    sphereList[i].GetComponent<Renderer>().material.color = new Color(0.6f, 0.6f, 0.6f, 1);

                    sphereBool[i] = true;

                    currentSphere = i + 1;

                    sphereList[currentSphere].GetComponent<Renderer>().material.color = new Color(1, 0.3f, 0.3f, 1);
                }
            }

            if (currentSphere == stepNumber)
            {
                endTime = Time.time;

                for (int i = 0; i < stepNumber; i++)
                {
                    if (sphereBool[i])
                        missedTargets += 1;
                }

                if (!missedTargetsCalled)
                {
                    OnFinishTask();
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    saveOnLoad.stage += 1;

                    SceneManager.LoadScene(0);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && missedTargetsCalled && saveOnLoad.stage == 3)
        {
            saveOnLoad.stage += 1;

            SceneManager.LoadScene(0);
        }

        if (Input.GetKeyDown(KeyCode.Space) && saveOnLoad.stage == 2)
        {
            print("stage2");
            endTime = Time.time;

            audioSource.Play();

            if (!missedTargetsCalled)
                OnFinishTask();

            saveOnLoad.stage += 1;
        }

        if (saveOnLoad.stage == 1)
        {
            if (Input.GetKeyDown(KeyCode.Space) && startTime == 0)
            {
                print("stage1");
                startTime = Time.time;

                audioSource.Play();

                currentSphere = stepNumber - 1;

                saveOnLoad.stage += 1;
            }
        }

        if(saveOnLoad.stage == 5 && startTime == 0)
        {
            scoreCanvas.SetActive(false);

            endTime = 0;

            startTime = Time.time;

            currentSphere = stepNumber - 1;

        }

        if(saveOnLoad.stage == 6 && endTime == 0)
        {
            endTime = Time.time;

            audioSource.Play();

            if (!missedTargetsCalled)
                OnFinishTask();

            startTime = 0;
        }
        //if(angle >= 10 && angleOld >= 10)
        //{
        //    audioSource.volume = Mathf.Min(angle * 0.03f, angleOld*0.03f);
        //}
        //else
        //{
        //    audioSource.volume = 0;
        //}

        //audioSource.panStereo = -0.02f * XYAngles.x;

        //audioSource.pitch = 1 - 0.01f * XYAngles.y;
    }

    void OnFinishTask()
    {
        scoreCanvas.SetActive(true);

        //print("Missed targets: " + (stepNumber - missedTargets));

        if (saveOnLoad.stage == 0)
        {
            if (scanningPattern == 1)
                scoreText.text = "Genauigkeit: " + (100 * rotationMeasurementScript.pattern1Saccade / rotationMeasurementScript.saccadeNumber) + "% \n Benötigte Zeit: " + (endTime - startTime).ToString("F2") + "s \n Wahrgenommenes Sichtfeld: " + textureScript.totalPercentage + "% \n Drücken Sie die Leertaste und \n wiederholen sie das Augenbewegungsmuster.";

            if (scanningPattern == 2)
                scoreText.text = "Genauigkeit: " + (100 * rotationMeasurementScript.pattern2Saccade / rotationMeasurementScript.saccadeNumber) + "% \n Benötigte Zeit: " + (endTime - startTime).ToString("F2") + "s";

            if (scanningPattern == 3)
                scoreText.text = "Genauigkeit: " + (100 * rotationMeasurementScript.pattern1Saccade / rotationMeasurementScript.saccadeNumber) + "% \n Benötigte Zeit: " + (endTime - startTime).ToString("F2") + "s";
        }

        if (saveOnLoad.stage == 2)
        {
            if (scanningPattern == 1)
                scoreText.text = "Genauigkeit: " + (100 * rotationMeasurementScript.pattern1Saccade / rotationMeasurementScript.saccadeNumber) + "% \n Benötigte Zeit: " + (endTime - startTime).ToString("F2") + "s \n Wahrgenommenes Sichtfeld: " + textureScript.totalPercentage + "% \n Drücken Sie die Leertaste, \n um mit der Suchaufgabe fortzufahren.";
        }
        audioSource.Play();

        if(saveOnLoad.stage == 6)
        {
            if (scanningPattern == 1)
                scoreText.text = "Alle Ziffern gefunden! \n Genauigkeit: " + (100 * rotationMeasurementScript.pattern1Saccade / rotationMeasurementScript.saccadeNumber) + "% \n Benötigte Zeit: " + (endTime - startTime).ToString("F2") + "s \n Wahrgenommenes Sichtfeld: " + textureScript.totalPercentage + "%";
        }

        missedTargetsCalled = true;
    }
}
