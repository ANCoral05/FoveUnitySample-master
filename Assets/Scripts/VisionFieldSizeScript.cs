using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionFieldSizeScript : MonoBehaviour
{
    [Range(5,60)]
    public int fieldAngle;

    public GameObject[] Planes = new GameObject[2];

    public GameObject[] Spheres = new GameObject[2];

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < Planes.Length; i++)
        {
            Planes[i].transform.localScale = new Vector3((0.0066f / 30) * fieldAngle, (0.0066f / 30) * fieldAngle, (0.0066f / 30) * fieldAngle);

            Spheres[i].transform.localScale = new Vector3((1.00f / 30) * fieldAngle, (1.00f / 30) * fieldAngle, (1.00f / 30) * fieldAngle);
        }
    }
}
