using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDirectionalLight_static : MonoBehaviour
{
    Vector3 staticPOS;
    // Start is called before the first frame update
    void Start()
    {
        staticPOS = transform.position;
        Debug.Log("ƒ‰ƒCƒg:"+staticPOS);
    }

    // Update is called once per frame
    void Update()
    {
        //transform.localPosition = staticPOS;
        
        if(Input.GetAxis("Horizontal") > 0.8f)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        if(Input.GetAxis("Horizontal") < -0.8f)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

    }
}
