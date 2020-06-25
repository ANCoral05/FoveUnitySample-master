using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveOnLoad : MonoBehaviour
{
    private static SaveOnLoad saveOnLoad;

    public int stage;

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this);

        if (saveOnLoad == null)
            saveOnLoad = this;
        else
            Destroy(transform.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
