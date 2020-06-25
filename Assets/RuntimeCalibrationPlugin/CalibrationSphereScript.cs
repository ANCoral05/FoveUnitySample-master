using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrationSphereScript : FOVEBehavior
{
    RuntimeCalibration CalScript;

    public int number;

    public float colorChange;

    private float finalAnimation;

    void Start()
    {
        CalScript = FindObjectOfType<RuntimeCalibration>().GetComponent<RuntimeCalibration>();
    }

    void Update()
    {
        Collider collider = this.GetComponent<Collider>();

        if (FoveInterface.Gazecast(collider))
        {
            colorChange += Time.deltaTime;
        }
        else
        {
            colorChange = 0;
        }

        //this.transform.localScale = new Vector3(1f + finalAnimation, 1f + finalAnimation, 1f + finalAnimation);

        if (colorChange >= 1)
        {
            print("Huiiii " + colorChange);

            finalAnimation += 10 * Time.deltaTime;

            GetComponent<Renderer>().material.color = new Color(0, 0, 0, 1);

            
        }
        else
        {
            finalAnimation = 0;

            GetComponent<Renderer>().material.color = new Color(1 - colorChange, colorChange, 0, 1);
        }

    }
}
