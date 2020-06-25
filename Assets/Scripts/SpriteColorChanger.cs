using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteColorChanger : MonoBehaviour
{
    public float timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = 1;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if(timer <= 0)
        {
            Color oldColor = GetComponent<SpriteRenderer>().color;

            GetComponent<SpriteRenderer>().color = oldColor - new Color(0, 0.02f, 0, 0);

            timer = 1;
        }
    }
}
