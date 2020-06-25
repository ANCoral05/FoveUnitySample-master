using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovingTargetScript : FOVEBehavior
{
    public Camera cam;

    public int movingTargetNumber;

    public int targetNumber;

    public int trackedTargets;

    public List<GameObject> Targets = new List<GameObject>();

    public GameObject Screen;

    public GameObject Target;

    [Range(0,10)]
    public float speedMin;

    [Range(0, 10)]
    public float speedMax;

    public float minDistance;

    public Vector3 positionVector;

    public GameObject Sphere;

    public Vector2[] coordinates;

    public Vector2[] coordinatesOld;

    public Vector2[] coordinatesNew;

    public int step = 0;

    public Material basic;

    public Material marked;

    public GameObject message;

    [Range(0, 100), Tooltip("The percentage of the screen used by the targets.")]
    public int fieldSize;

    [Range(0,1), Tooltip("The intervals at which the targets change their direction.")]
    public float timer;

    public float currentTimer;

    [Range(0, 10), Tooltip("The speed at which the targets are moving.")]
    public float speed;

    public float stepTimer;

    public int foundTargets;

    // Start is called before the first frame update
    void Start()
    {
        currentTimer = timer;

        coordinates = new Vector2[targetNumber];

        coordinatesOld = new Vector2[targetNumber];

        coordinatesNew = new Vector2[targetNumber];
    }

    // Update is called once per frame
    void Update()
    {
        if(step==0)
        {
            message.SetActive(true);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                stepTimer = 3;

                step = 1;
            }
        }


        if (step == 1)
        {
            message.SetActive(false);

            for (int i = 0; i < 3 * targetNumber; i++)
            {


                if (Targets.Count < targetNumber)
                {
                    coordinates[i].x = Random.Range(40f + (100 - fieldSize) * 0.55f, 150.0f - (100 - fieldSize) * 0.55f);

                    coordinates[i].y = Random.Range(0.0f + (100 - fieldSize) * 0.9f, 180.0f - (100 - fieldSize) * 0.9f);

                    positionVector = new Vector3(Screen.transform.position.x + (2.72f * Mathf.Sin(coordinates[i].x * Mathf.Deg2Rad) * Mathf.Cos(coordinates[i].y * Mathf.Deg2Rad)), Screen.transform.position.y + (2.72f * Mathf.Cos(coordinates[i].x * Mathf.Deg2Rad)), Screen.transform.position.z + (2.72f * Mathf.Sin(coordinates[i].x * Mathf.Deg2Rad) * Mathf.Sin(coordinates[i].y * Mathf.Deg2Rad)));

                    Collider[] nearbyColliders = Physics.OverlapSphere(new Vector3(positionVector.x, positionVector.y, positionVector.z), 0.03f);

                    if (nearbyColliders.Length == 0)
                    {
                        GameObject newTarget = Instantiate(Target, new Vector3(positionVector.x, positionVector.y, positionVector.z), Screen.transform.rotation);

                        Targets.Add(newTarget);
                    }

                    coordinatesNew[i] = coordinates[i];
                }
            }


            for (int i = 0; i < trackedTargets; i++)
            {
                Targets[i].GetComponent<Renderer>().material = marked;
            }


            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0 && Input.GetKeyDown(KeyCode.Space))
            {
                if (Targets.Count == targetNumber)
                {
                    for (int i = 0; i < trackedTargets; i++)
                    {
                        Targets[i].GetComponent<Renderer>().material = basic;
                    }
                }

                currentTimer = timer;

                stepTimer = 10;

                step = 2;
            }
        }

        if(step==2)
        {
            currentTimer -= Time.deltaTime;

            if (currentTimer <= 0)
            {
                for (int i = 0; i < targetNumber; i++)
                {
                    coordinatesOld[i] = coordinates[i];

                    coordinatesNew[i] = new Vector2(Mathf.Max(40+(100 - fieldSize) * 0.55f, Mathf.Min(coordinates[i].x + Random.Range(-10f, 10.0f), 150- (100 - fieldSize) * 0.55f)), Mathf.Max((100 - fieldSize) * 0.9f, Mathf.Min(coordinates[i].y + Random.Range(-10f, 10.0f), 180- (100 - fieldSize) * 0.9f)));

                    currentTimer = timer;
                }
            }

            for (int i = 0; i < targetNumber; i++)
            {
                float x = coordinatesNew[i].x - coordinatesOld[i].x;

                float y = coordinatesNew[i].y - coordinatesOld[i].y;

                coordinates[i] += new Vector2(x / Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2)) * speed * 3 * Time.deltaTime, y / Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2)) * speed * 3 * Time.deltaTime);

                positionVector = new Vector3(Screen.transform.position.x + (2.72f * Mathf.Sin(coordinates[i].x * Mathf.Deg2Rad) * Mathf.Cos(coordinates[i].y * Mathf.Deg2Rad)), Screen.transform.position.y + (2.72f * Mathf.Cos(coordinates[i].x * Mathf.Deg2Rad)), Screen.transform.position.z + (2.72f * Mathf.Sin(coordinates[i].x * Mathf.Deg2Rad) * Mathf.Sin(coordinates[i].y * Mathf.Deg2Rad)));

                Targets[i].transform.position = positionVector;
            }

            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0)
            {
                stepTimer = 5;

                step = 3;
            }
        }

        if(step == 3)
        {
            for (int i = 0; i < trackedTargets; i++)
            {
                float angle;

                angle = Vector3.Angle(Targets[i].transform.position - cam.transform.position, FoveInterface.GetGazeRays().right.direction);

                if (angle<8 && Input.GetKeyDown(KeyCode.Space))
                {
                    Targets[i].GetComponent<Renderer>().material = marked;

                    foundTargets += 1;
                }
            }

            if (foundTargets == trackedTargets)
            {
                foundTargets = 0;

                step = 0;
            }
        }

        //foreach(GameObject sphere in Targets)
        //{
        //    Sphere.transform.position = positionVector;

        //    //Vector2 Translate = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));

        //    //Vector3 TranslateVector = new Vector3((2.72f * Mathf.Sin(Translate.x * Mathf.Deg2Rad) * Mathf.Cos(Translate.y * Mathf.Deg2Rad)), 2.72f - (2.72f * Mathf.Cos(Translate.x * Mathf.Deg2Rad)), (2.72f * Mathf.Sin(Translate.x * Mathf.Deg2Rad) * Mathf.Sin(Translate.y * Mathf.Deg2Rad)));

        //    //sphere.transform.position = sphere.transform.position + TranslateVector;

        //    //print(Translate + " " + TranslateVector);
        //}
    }
}
