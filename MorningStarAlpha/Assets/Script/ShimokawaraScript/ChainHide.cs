using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainHide : MonoBehaviour
{
    MeshRenderer MyMeshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        MyMeshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MyMeshRenderer.enabled = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MyMeshRenderer.enabled = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MyMeshRenderer.enabled = true;
        }
    }

}
