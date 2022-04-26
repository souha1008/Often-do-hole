using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGCamera : MonoBehaviour
{
    public GameObject CenterObj;

    Vector3 OldPos;

    public float moveMulti;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        Vector3 Move = CenterObj.transform.position - OldPos;
        
        transform.position += (Move * moveMulti);

        OldPos = CenterObj.transform.position;
    }
}
