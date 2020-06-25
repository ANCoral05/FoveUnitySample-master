using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaccadeTrainingScript : FOVEBehavior
{
    [Range(5,45), Tooltip("The player's effective field of view in degree.")]
    public int FieldOfView;

    public GameObject[] Planes = new GameObject[2];

    public GameObject Screen;

    public Camera cam;

    public List<GameObject> Dots = new List<GameObject>();

    public GameObject Dot;

    private GameObject NewDot;

    [Range(0,360)]
    public float Angle;

    private int counter;

    public float[] Steps;

    public float CurrentAngleTransform;

    public string[] directions;

    public string currentDirection;

    private Vector3 vector;


    //Start Menu
    public GameObject canvas;

    public SphereCollider StartingCollider;

    public Image ProgressCircle;

    public Image ProgressCircleEdge;

    public float loadingProgress;

    public AudioSource audioSourceLoad;

    public AudioSource audioSourceBling;

    public AudioClip loading;

    public AudioClip loaded;

    public bool isMenu;

    // Start is called before the first frame update
    void Start() 
    {
        Screen.transform.position = cam.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Application.targetFrameRate = 70;

        print(1.0f / Time.deltaTime);

        //Start Menu
        if (FoveInterface.Gazecast(StartingCollider) && isMenu)
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
            isMenu = false;

            canvas.SetActive(false);

            NewDot = GameObject.Instantiate(Dot, new Vector3(Screen.transform.position.x, Screen.transform.position.y, Screen.transform.position.z + 2.73f), new Quaternion(0, 0, 0, 0));

            Dots.Add(NewDot);

            currentDirection = directions[Random.Range(0, directions.Length)];

            loadingProgress = 0;
        }

            foreach (GameObject plane in Planes)
        {
            plane.transform.localScale = new Vector3(0.1f * FieldOfView/30f, 0.1f * FieldOfView / 30f, 0.1f * FieldOfView / 30f);
        }

        if (Input.GetKeyDown(KeyCode.Space) || FoveInterface.Gazecast(Dots[Dots.Count - 1].GetComponent<SphereCollider>()))
        {
            CurrentAngleTransform += Steps[counter % Steps.Length];

            if (currentDirection == "north")
            {
                vector = new Vector3(Screen.transform.position.x, Screen.transform.position.y + 2.73f * Mathf.Sin(((CurrentAngleTransform) / 180f) * 3.14156f), Screen.transform.position.z + 2.73f * Mathf.Cos(((CurrentAngleTransform) / 180f) * 3.14156f));
            }

            if (currentDirection == "east")
            {
                vector = new Vector3(Screen.transform.position.x + 2.73f * Mathf.Sin(((CurrentAngleTransform) / 180f) * 3.14156f), Screen.transform.position.y, Screen.transform.position.z + 2.73f * Mathf.Cos(((CurrentAngleTransform) / 180f) * 3.14156f));
            }

            if (currentDirection == "south")
            {
                vector = new Vector3(Screen.transform.position.x, Screen.transform.position.y - 2.73f * Mathf.Sin(((CurrentAngleTransform) / 180f) * 3.14156f), Screen.transform.position.z + 2.73f * Mathf.Cos(((CurrentAngleTransform) / 180f) * 3.14156f));
            }

            if (currentDirection == "west")
            {
                vector = new Vector3(Screen.transform.position.x - 2.73f * Mathf.Sin(((CurrentAngleTransform) / 180f) * 3.14156f), Screen.transform.position.y, Screen.transform.position.z + 2.73f * Mathf.Cos(((CurrentAngleTransform) / 180f) * 3.14156f));
            }

            counter += 1;

            NewDot = GameObject.Instantiate(Dot, vector, Quaternion.Euler(0, 0, 0));

            Dots.Add(NewDot);
        }

        if ((Mathf.Abs(CurrentAngleTransform) >= 100 + Steps[counter % Steps.Length] && (currentDirection == "east" || currentDirection == "west")) || (Mathf.Abs(CurrentAngleTransform) >= 75 + Steps[counter % Steps.Length] && (currentDirection == "north" || currentDirection == "south")))
        {
            CurrentAngleTransform = 0;

            counter = 0;

            canvas.SetActive(true);

            GameObject[] targets = GameObject.FindGameObjectsWithTag("TargetPoint");

            foreach (GameObject target in targets) GameObject.Destroy(target);

            isMenu = true;

            Dots.Clear();

        }
    }
}
