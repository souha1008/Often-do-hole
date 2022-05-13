using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainHide : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("あたった");
        if (other.CompareTag("Player"))
        {
            this.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("離れたた");
        if (other.CompareTag("Player"))
        {
            this.GetComponent<MeshRenderer>().enabled = true;
        }
    }

}
