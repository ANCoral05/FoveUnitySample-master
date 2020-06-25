using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRate : MonoBehaviour
{
    public TextMesh text;

    private float waitTime = 0;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Application.targetFrameRate = 120;

        waitTime += Time.deltaTime;

        if (waitTime > 0.5f)
        {
            text.text = "Fps: " + 1.0f / Time.smoothDeltaTime;

            waitTime = 0;
        }
    }
}
