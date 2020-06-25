using System.Collections;
using System.Collections.Generic;
using Fove.Unity;
using UnityEngine;
using UnityEngine.UI;

public class NewSetNumbers : FOVEBehavior
{
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

    public SaveOnLoad saveOnLoad;

    // Start is called before the first frame update
    void Start()
    {
        saveOnLoad = FindObjectOfType<SaveOnLoad>();
    }


    // Update is called once per frame
    void Update()
    {
        if (saveOnLoad.stage == 4 && !canvas.activeSelf)
        {
            canvas.SetActive(true);

            Infotext.SetActive(true);

            saveOnLoad.stage = 5;
        }

        if (step == 0)
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag("TargetNumber");

            foreach (GameObject target in targets) GameObject.Destroy(target);

            TargetNumber = Random.Range(0, 10);

            Infotext.GetComponent<TextMesh>().text = "Suche: \n" + TargetNumber;

            if(saveOnLoad.stage > 3)
                saveOnLoad.stage = 5;

            step = 1;
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

            print("before" + Time.realtimeSinceStartup * 1000);

            //placement = Random.insideUnitCircle;

            //nearbyColliders = Physics.OverlapSphere(new Vector3(Screen.position.x, Screen.position.y + placement.x * 1.4f, Screen.position.z + placement.y * 1.4f), 0.07f);

            //if (nearbyColliders.Length == 0)
            //{
            //    Instantiate(TargetText, Screen.position + new Vector3(0, placement.x * 1.4f, placement.y * 1.4f), Quaternion.Euler(0, 90, 0));

            //    TargetText.text = Random.Range(0, 10).ToString();
            //}

            InvokeRepeating("PlaceNumbers", 0, 0.001f);

            GameObject[] targets = GameObject.FindGameObjectsWithTag("TargetNumber");

            if (targets.Length == TargetAmount)
            {
                CancelInvoke("PlaceNumbers");

                print("after" + Time.realtimeSinceStartup * 1000);
            }
            if (targets.Length == TargetAmount)
            {
                foreach (GameObject target in targets)
                    if (target.GetComponent<TextMesh>().text == TargetNumber.ToString())
                    {
                        //print("Found one!" + target.transform.name);

                        CorrectTargetNumber += 1;
                    }

                //print("Target number:" + TargetNumber + "; Targets: " + CorrectTargetNumber);

                step = 3;
            }
        }

        if (step == 3)
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag("TargetNumber");

            if (targets.Length == (TargetAmount - CorrectTargetNumber))
            {
                saveOnLoad.stage = 6;

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    canvas.SetActive(true);

                    Infotext.SetActive(true);

                    CorrectTargetNumber = 0;

                    step = 0;
                }

                foreach (GameObject target in targets) GameObject.Destroy(target);
            }
        }
    }

    void PlaceNumbers()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("TargetNumber");

        if (targets.Length < TargetAmount)
        {
            coordinates.x = Random.Range(95.0f-fieldScale*0.55f, 95.0f+fieldScale*0.55f);

            coordinates.y = Random.Range(90.0f-fieldScale*0.9f, 90.0f + fieldScale * 0.9f);

            positionVector = new Vector3(
                Screen.transform.position.x + (2.72f * Mathf.Sin(coordinates.x * Mathf.Deg2Rad) * Mathf.Cos(coordinates.y * Mathf.Deg2Rad)), 
                Screen.transform.position.y + (2.72f * Mathf.Cos(coordinates.x * Mathf.Deg2Rad)), 
                Screen.transform.position.z + (2.72f * Mathf.Sin(coordinates.x * Mathf.Deg2Rad) * Mathf.Sin(coordinates.y * Mathf.Deg2Rad)));

            nearbyColliders = Physics.OverlapSphere(positionVector, minDistance);

            if (nearbyColliders.Length == 1)
            {
                Instantiate(TargetText, positionVector, Quaternion.LookRotation(positionVector - Screen.transform.position));

                TargetText.text = Random.Range(0, 10).ToString();
            }
        }
    }
}