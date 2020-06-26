using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComicImage : MonoBehaviour
{
    public Texture[] comics;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshRenderer>().material.mainTexture = comics[Random.Range(0, comics.Length)];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
