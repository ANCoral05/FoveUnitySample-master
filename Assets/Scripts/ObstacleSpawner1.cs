using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner1 : MonoBehaviour
{
    public GameObject[] Obstacles;

    // Start is called before the first frame update
    void Start()
    {
        GameObject obstacle = Instantiate(Obstacles[Random.Range(0, Obstacles.Length)], transform.position, transform.rotation, transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
