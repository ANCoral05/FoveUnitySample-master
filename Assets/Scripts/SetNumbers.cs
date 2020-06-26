using System.Collections;
using System.Collections.Generic;
using Fove.Unity;
using UnityEngine;
using UnityEngine.UI;

public class SetNumbers : FOVEBehavior
{
    public TextMesh TargetText;

    public Transform Screen;

    public GameObject Infotext;

    [Range(0, 30)]
    public int TargetAmount;

    [Range(0,1), Tooltip("The size of the field in which target numbers will appear.")]
    public float fieldScale;

    private Vector2 placement;

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

    public SaveOnLoad saveOnLoad;

    public int averageCorrectTargets = 0;

    private int currentCorrectTargets;

    // Start is called before the first frame update
    void Start()
    {
        saveOnLoad = FindObjectOfType<SaveOnLoad>();
    }

    
    // Update is called once per frame
    void Update()
    {
        if (step == 0)
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag("TargetNumber");

            foreach (GameObject target in targets) GameObject.Destroy(target);

            TargetNumber = Random.Range(0, 10);

            Infotext.GetComponent<TextMesh>().text = "Suche: \n" + TargetNumber;

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

            loadingProgress += Time.deltaTime*0.5f;
        }
        else
        {
            loadingProgress = 0;

            audioSourceLoad.Stop();
        }

        ProgressCircle.fillAmount = loadingProgress;

        ProgressCircleEdge.transform.rotation = Quaternion.Euler(0, 90, -360 * loadingProgress);

        ProgressCircleEdge.color = new Color(1, 1, 1, loadingProgress*36);

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
            if(averageCorrectTargets == 0)
                averageCorrectTargets = Random.Range(2, 5);

            InvokeRepeating("PlaceNumbers", 0, 0.001f);

            GameObject[] targets = GameObject.FindGameObjectsWithTag("TargetNumber");

            if (targets.Length == TargetAmount)
            {
                CancelInvoke("PlaceNumbers");

                //print("after" + Time.realtimeSinceStartup * 1000);
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
                canvas.SetActive(true);

                Infotext.SetActive(true);

                CorrectTargetNumber = 0;

                step = 0;
            }
        }
    }

    void PlaceNumbers()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("TargetNumber");

        if (targets.Length < TargetAmount)
        {
            placement = Random.insideUnitCircle;

            nearbyColliders = Physics.OverlapSphere(new Vector3(Screen.position.x, Screen.position.y + placement.x * 1.4f, Screen.position.z + placement.y * 1.4f), 0.07f);

            if (nearbyColliders.Length == 0)
            {
                Instantiate(TargetText, Screen.position + new Vector3(0, placement.x * fieldScale*1.4f, placement.y * fieldScale*1.4f), Quaternion.Euler(0, 90, 0));

                if (currentCorrectTargets < averageCorrectTargets)
                {
                    TargetText.text = averageCorrectTargets.ToString();
                }
                else
                {
                    TargetText.text = Random.Range(0, 10).ToString();

                    while(TargetText.text == averageCorrectTargets.ToString())
                        TargetText.text = Random.Range(0, 10).ToString();
                }
            }
        }
    }
}
