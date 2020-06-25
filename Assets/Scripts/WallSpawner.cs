using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpawner : MonoBehaviour
{
    public GameObject[] Obstacles;

    public float Interval = 4.95f;

    public float IntervalTimer = 4.95f;

    private void Start()
    {
        for (int k = 0; k < 5; k++)
        {
            for (int i = 0; i < Obstacles.Length; i++)
            {
                Vector3 position = transform.GetChild(i).position + new Vector3(0, 0, -10*k);

                GameObject.Instantiate(Obstacles[i], position, new Quaternion(0, 0, 0, 0));
            }
        }
    }

    void Update()
    {
        IntervalTimer -= Time.deltaTime;

        if (IntervalTimer <= 0)
        {
            for (int i = 0; i < Obstacles.Length; i++)
            {
                Vector3 position = transform.GetChild(i).position;

                GameObject.Instantiate(Obstacles[i], position, new Quaternion(0, 0, 0, 0));
            }
            IntervalTimer = Interval;
        }
    }
}
