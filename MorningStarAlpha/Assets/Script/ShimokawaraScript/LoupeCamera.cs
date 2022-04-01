using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoupeCamera : MonoBehaviour
{

    public GameObject Obj;
    public float Disttance;
    // Start is called before the first frame update
    void Start()
    {
        TraceObj();
    }

    // Update is called once per frame
    void Update()
    {
        TraceObj();
    }

    void TraceObj()
    {
        Vector3 tempPos = Obj.transform.position;
        tempPos.z -= Disttance;
        transform.position = tempPos;
    }
}
