using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPosition : MonoBehaviour
{
    // Start is called before the first frame update

    public float PathDistanceZ = 50;
    void Start()
    {
        this.transform.position += new Vector3(transform.position.x, transform.position.y, - PathDistanceZ);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
