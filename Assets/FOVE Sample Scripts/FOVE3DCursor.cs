using UnityEngine;

public class FOVE3DCursor : FOVEBehavior
{
	public enum LeftOrRight
	{
		Left,
		Right
	}

	[SerializeField]
	public LeftOrRight whichEye;

    public Vector2 ScreenPos;

    public Camera cam;

    [Range(0, 360)]
    public int yRotation;

    [Range(0,5)]
    public float distance;

    public bool projectOnPlane;

	// Use this for initialization
	void Start ()
    {
    }

	// Latepdate ensures that the object doesn't lag behind the user's head motion
	void Update()
    {
		var rays = FoveInterface.GetGazeRays();
		var ray = whichEye == LeftOrRight.Left ? rays.left : rays.right;

        ScreenPos = cam.WorldToScreenPoint(transform.position);

        RaycastHit hit;
		Physics.Raycast(ray, out hit, Mathf.Infinity);
		if (hit.point != Vector3.zero && projectOnPlane) // Vector3 is non-nullable; comparing to null is always false
		{
			transform.position = hit.point;
		}
		else
		{
			transform.position = ray.GetPoint(distance);

            transform.rotation = Quaternion.LookRotation(ray.direction, Vector3.up) * Quaternion.Euler(90,yRotation,0); //cam.transform.rotation *


        }
	}
}
