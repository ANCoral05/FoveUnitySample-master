using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeChildren : MonoBehaviour
{
    private bool merged;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= 1 && !merged)
        {
            GameObject[] meshCollection = GameObject.FindGameObjectsWithTag("staticObstacle");
            MeshFilter[] meshFilters = new MeshFilter[meshCollection.Length];

            for (int j = 0; j < meshFilters.Length; j++)
            {
                meshFilters[j] = meshCollection[j].GetComponent<MeshFilter>();
            }
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];

            int i = 0;
            while (i < meshFilters.Length)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                meshFilters[i].gameObject.SetActive(false);

                i++;
            }
            transform.GetComponent<MeshFilter>().mesh = new Mesh();
            transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
            transform.gameObject.SetActive(true);

            merged = true;
        }
    }
}
