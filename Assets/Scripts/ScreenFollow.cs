using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenFollow : MonoBehaviour
{
    public GameObject cam;

    private float angle;

    private static float wrapAngle(float angle)
    {
        angle %= 360;
        if (angle > 180)
            return angle - 360;
        else
            return angle;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        angle = wrapAngle(cam.transform.eulerAngles.y) - wrapAngle(transform.eulerAngles.y);


        if (Mathf.Abs(angle) > 45)
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, cam.transform.eulerAngles.y, transform.eulerAngles.z);
    }
}
