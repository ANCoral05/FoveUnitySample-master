using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    public float movingRangeX;
    public float movingRangeZ;
    public float movingSpeed;
    public int movingInterval;
    private float intervalCounter;
    public Vector3 startPosition;
    private float randomInterval;
    private GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.localPosition;
        target = new GameObject("target");
        target.transform.parent = transform.parent;
        target.transform.localPosition = new Vector3(startPosition.x + Random.Range(-0.5f, 0.5f) * movingRangeX, startPosition.y, startPosition.z + Random.Range(-0.5f, 0.5f) * movingRangeZ);
        randomInterval = Random.Range(0.5f, 1.5f) * movingInterval;
    }

    // Update is called once per frame
    void Update()
    {
        intervalCounter += Time.deltaTime;

        if(intervalCounter > randomInterval)
        {
            target.transform.localPosition = new Vector3(startPosition.x + Random.Range(-0.5f, 0.5f) * movingRangeX, startPosition.y, startPosition.z + Random.Range(-0.5f, 0.5f) * movingRangeZ);
            randomInterval = Random.Range(0.5f, 1.5f) * movingInterval;
            intervalCounter = 0;
        }

        float distance = Vector3.Magnitude(target.transform.localPosition - transform.localPosition);

        if(distance >= 0.05f)
        {
            transform.Translate(Vector3.Normalize(target.transform.localPosition - transform.localPosition) * Time.deltaTime * movingSpeed*Mathf.Min(distance*intervalCounter, 1));
        }
    }
}
