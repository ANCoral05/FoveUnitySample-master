using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NavigationControls : MonoBehaviour
{
    public float walkingSpeed;

    public float realWalkingSpeed;

    public float acceleration;

    public float rotateAcceleration;

    public GameObject navigationMap;

    public Camera cam;

    public GameObject rotateObject;

    public float rotateTime;

    private float realRotateTime;

    private Vector3 walkingDirection;

    public Vector2 move;

    public Controls controls;

    // Start is called before the first frame update
    private void Awake()
    {
        controls = new Controls();

        controls.Gameplay.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => move = Vector2.zero;
    }

    void Start()
    {
        realWalkingSpeed = 0;

        realRotateTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        walkingDirection = new Vector3(-cam.transform.forward.x, 0, -cam.transform.forward.z);
        Vector3 leftDirection = new Vector3(cam.transform.right.x, 0, cam.transform.right.z);
        Vector3 rightDirection = new Vector3(-cam.transform.right.x, 0, -cam.transform.right.z);
        Vector3 backwardsDirection = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z);

        Vector3 movementVector = new Vector3(-move.x*Mathf.Cos(cam.transform.eulerAngles.y*Mathf.Deg2Rad) - move.y * Mathf.Sin(cam.transform.eulerAngles.y * Mathf.Deg2Rad), 0, move.x * Mathf.Sin(cam.transform.eulerAngles.y * Mathf.Deg2Rad) - move.y* Mathf.Cos(cam.transform.eulerAngles.y * Mathf.Deg2Rad));

        navigationMap.transform.Translate(Time.deltaTime * realWalkingSpeed * movementVector);

        if(Mathf.Abs(movementVector.magnitude) > 0 && realWalkingSpeed < walkingSpeed)
        {
            realWalkingSpeed += Time.deltaTime * acceleration;
        }
        if (realWalkingSpeed > walkingSpeed)
            realWalkingSpeed = walkingSpeed;
        if(movementVector.magnitude == 0 && realWalkingSpeed > 0)
        {
            realWalkingSpeed -= Time.deltaTime * 2;
        }
        if (realWalkingSpeed < 0)
            realWalkingSpeed = 0;

        //if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && realRotateTime > 0)
        //{
        //    realRotateTime -= 2*Time.deltaTime;
        //}
        //if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && !(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)))
        //{
        //    if (realRotateTime < rotateTime)
        //        realRotateTime = rotateTime;
        //}

        //if ((Input.GetKey(KeyCode.W) || Input.GetMouseButton(0)) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        //{
        //    if (realWalkingSpeed < walkingSpeed)
        //        realWalkingSpeed += Time.deltaTime * acceleration;
        //    if (realWalkingSpeed > walkingSpeed)
        //        realWalkingSpeed = walkingSpeed;
        //}
        //else
        //{
        //    if (realWalkingSpeed > 0)
        //        realWalkingSpeed -= Time.deltaTime * 2;
        //    if (realWalkingSpeed < 0)
        //        realWalkingSpeed = 0;
        //}
                
        //if (Input.GetKey(KeyCode.W) || Input.GetMouseButton(0))
        //{
        //    navigationMap.transform.Translate(Time.deltaTime * realWalkingSpeed * (Mathf.Cos(rotateObject.transform.eulerAngles.y*Mathf.Deg2Rad)*walkingDirection + Mathf.Sin(rotateObject.transform.eulerAngles.y*Mathf.Deg2Rad)*leftDirection));
        //}
        //if (Input.GetKey(KeyCode.A))
        //{
        //    navigationMap.transform.Translate((2*rotateTime-realRotateTime)/(2*rotateTime)*Time.deltaTime * realWalkingSpeed * 0.65f * (Mathf.Cos(rotateObject.transform.eulerAngles.y * Mathf.Deg2Rad) * leftDirection + Mathf.Sin(rotateObject.transform.eulerAngles.y * Mathf.Deg2Rad) * backwardsDirection));
        //    if (realRotateTime < 2 * rotateTime)
        //        realRotateTime += rotateAcceleration * Time.deltaTime;
        //    if (realRotateTime > rotateTime)
        //        rotateObject.transform.Rotate((realRotateTime - rotateTime)/(0.5f*rotateTime) * Time.deltaTime * 45f * Vector3.up);
        //}
        //if (Input.GetKey(KeyCode.D))
        //{
        //    navigationMap.transform.Translate((2 * rotateTime - realRotateTime) / (2 * rotateTime)*Time.deltaTime * realWalkingSpeed * 0.65f * (Mathf.Cos(rotateObject.transform.eulerAngles.y * Mathf.Deg2Rad) * rightDirection + Mathf.Sin(rotateObject.transform.eulerAngles.y * Mathf.Deg2Rad) * walkingDirection));
        //    if (realRotateTime < 2 * rotateTime)
        //        realRotateTime += rotateAcceleration * Time.deltaTime;
        //    if (realRotateTime > rotateTime)
        //        rotateObject.transform.Rotate((realRotateTime - rotateTime) / (0.5f * rotateTime) * Time.deltaTime * -45f * Vector3.up);
        //}
        if (Input.GetKey(KeyCode.S))
        {
            navigationMap.transform.Translate(Time.deltaTime * realWalkingSpeed* 0.65f * (Mathf.Cos(rotateObject.transform.eulerAngles.y * Mathf.Deg2Rad) * backwardsDirection + Mathf.Sin(rotateObject.transform.eulerAngles.y * Mathf.Deg2Rad) * rightDirection));
        }
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }
}
