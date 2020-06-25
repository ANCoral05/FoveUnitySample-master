using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CorrectedGazeVector
{
    public static Vector3 CorrectedVectorLeft;

    public static Vector3 CorrectedVectorRight;
}

public class RuntimeCalibrationScript : FOVEBehavior
{
    private GameObject calibrationTarget;

    public GameObject Headset;

    [Range(3, 7), Tooltip("The number of targets in horizontal direction. More targets lead to a more precise calibration, but longer calibration duration.")]
    public int HorizontalTargetCount;

    [Range(3, 7), Tooltip("The number of targets in vertical direction. More targets lead to a more precise calibration, but longer calibration duration.")]
    public int VerticalTargetCount;

    [Range(0, 140), Tooltip("The width of the field of view to be tested (in degree). This value should be within the VR device's screen dimensions.")]
    public float MaxAngleHorizontal;

    [Range(0, 120), Tooltip("The height of the field of view to be tested (in degree)This value should be within the VR device's screen dimensions.")]
    public float MaxAngleVertical;

    private GameObject[] Targets;

    public Vector3[] GazeAngleLeft;

    public Vector3[] GazeAngleCorrectedLeft;

    public Vector3[] GazeAngleCorrectionsLeft = new Vector3[8];

    public Vector3[] GazeAngleRight;

    public Vector3[] GazeAngleCorrectedRight;

    public Vector3[] GazeAngleCorrectionsRight = new Vector3[8];

    private float StepTime;

    private int Step;

    private float[] TimeGazedAtTarget;

    private int completedTargets;

    void Start()
    {
        GazeAngleLeft = new Vector3[HorizontalTargetCount*VerticalTargetCount];

        GazeAngleRight = new Vector3[HorizontalTargetCount * VerticalTargetCount];

        GazeAngleCorrectedRight = new Vector3[HorizontalTargetCount*VerticalTargetCount];

        GazeAngleCorrectedLeft = new Vector3[HorizontalTargetCount * VerticalTargetCount];

        TimeGazedAtTarget = new float[HorizontalTargetCount * VerticalTargetCount];

        StartCoroutine("Calibration");
    }

    void Update()
    {
        for (int i = 0; i<Targets.Length; i++)
        {
            //Checks, if the gaze direction is near one of the targets.
            if (FoveInterface.Gazecast(Targets[i].GetComponent<Collider>()))
            {
                TimeGazedAtTarget[i] += Time.deltaTime;
            }
            else
            {
                TimeGazedAtTarget[i] = 0;
            }

            //After looking at a target for one second, the gaze direction is tracked multiple times.
            if(TimeGazedAtTarget[i] >= 1 && TimeGazedAtTarget[i] < 3)
            {
                StepTime += Time.deltaTime;

                if (StepTime >= 0.2f)
                {
                    GazeAngleCorrectionsLeft[Step] = GazeAngleLeft[i].normalized - FoveInterface.GetGazeRays().left.direction;

                    GazeAngleCorrectionsRight[Step] = GazeAngleRight[i].normalized - FoveInterface.GetGazeRays().right.direction;

                    Step += 1;

                    StepTime = 0;
                }
            }

            //After tracking for 2 seconds, the average difference between the gaze vector and the vector to the target is calculated
            //and set as the gaze correction value.
            if (TimeGazedAtTarget[i] >= 3)
            {
                for (int k=0; k<8; k++)
                {
                    GazeAngleCorrectedLeft[i] += GazeAngleCorrectionsLeft[k];

                    GazeAngleCorrectedRight[i] += GazeAngleCorrectionsLeft[k];
                }

                GazeAngleCorrectedLeft[i] = GazeAngleCorrectedLeft[i].normalized;

                GazeAngleCorrectedRight[i] = GazeAngleCorrectedRight[i].normalized;



                completedTargets += 1;
            }
        }

        if(completedTargets == HorizontalTargetCount*VerticalTargetCount)
        {

        }

        //if ()
        //CorrectedGazeVector.CorrectedVectorLeft = 
    }

    IEnumerator Calibration() //Place an array of spheres.
    {
        for (int i = 0; i < HorizontalTargetCount; i++)
        {
            for (int j = 0; j < VerticalTargetCount; j++)
            {
                float AngleInRadHorizontal = (90 + (-MaxAngleHorizontal / 2 + i * MaxAngleHorizontal / (HorizontalTargetCount - 1))) * Mathf.Deg2Rad;

                float AngleInRadVertical = (90 + (-MaxAngleVertical / 2 + j * MaxAngleVertical / (VerticalTargetCount - 1))) * Mathf.Deg2Rad;

                GameObject target = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                target.transform.position = new Vector3(
                    Headset.transform.position.x + (1 * Mathf.Sin(AngleInRadVertical) * Mathf.Cos(AngleInRadHorizontal)),
                    Headset.transform.position.y + (1 * Mathf.Cos(AngleInRadVertical)),
                    Headset.transform.position.z + (1 * Mathf.Sin(AngleInRadVertical) * Mathf.Sin(AngleInRadHorizontal)));

                target.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

                target.transform.parent = Headset.transform;

                GazeAngleLeft[i + j * VerticalTargetCount] = target.transform.position - FoveInterface.GetGazeRays().left.origin;

                GazeAngleRight[i + j * VerticalTargetCount] = target.transform.position - FoveInterface.GetGazeRays().right.origin;

                target.GetComponent<Renderer>().material = new Material(Shader.Find("Unlit/Color"));

                target.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 1);

                Targets[i + j * VerticalTargetCount] = target;

                target.GetComponent<SphereCollider>().radius = 10;
            }
        }
        yield return(0);

    }
    
}
