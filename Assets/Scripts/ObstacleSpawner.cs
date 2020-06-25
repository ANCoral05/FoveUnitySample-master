using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] Obstacles;

    public Vector2 MinMaxObstacleInterval = new Vector2(3, 6);

    public float IntervalTimer = 1;

    void Update()
    {
        IntervalTimer -= Time.deltaTime;

        if (IntervalTimer <= 0)
        {
            int spawner = Random.Range(0, transform.childCount);

            Vector3 position = transform.GetChild(spawner).position;

            GameObject.Instantiate(Obstacles[Random.Range(0, Obstacles.Length)], position, new Quaternion(0, 0, 0, 0));

            IntervalTimer = Random.Range(MinMaxObstacleInterval.x, MinMaxObstacleInterval.y);
        }
    }
}
