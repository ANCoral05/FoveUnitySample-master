using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawVision : FOVEBehavior
{
    private Ray ray;

    public GameObject sprite;

    public GameObject screen;

    public Vector3 oldPosition;

    public List<Vector3> spritePosition;

    public List<float> spriteTime;

    [Range(0, 2), Tooltip("The amount of the visual field that is colored.")]
    public float scale;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ray = FoveInterface.GetGazeRays().right;

        RaycastHit hit;
        
        if(Physics.Raycast(ray, out hit, Mathf.Infinity));
        {
            Vector3 distanceVector = hit.point - oldPosition;

            if (distanceVector.magnitude >= 0.2f*scale)
            {
                GameObject thisSprite = GameObject.Instantiate(sprite, hit.point - (hit.point - screen.transform.position) * 0.02f, Quaternion.LookRotation(screen.transform.position - hit.point));

                thisSprite.transform.localScale = new Vector3(scale * 0.1f, scale * 0.1f, scale * 0.1f);

                oldPosition = hit.point - (hit.point - screen.transform.position) * 0.02f;

                spritePosition.Add(thisSprite.transform.position);

                spriteTime.Add(Time.time);
            }
        }
    }
}
