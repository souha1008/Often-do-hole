using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDirectionalLight_static : MonoBehaviour
{
    Vector3 staticPOS;
    Quaternion defaultRotation;
    // Start is called before the first frame update
    void Start()
    {
        staticPOS = transform.position;
        Debug.Log("ƒ‰ƒCƒg:"+staticPOS);
        defaultRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.localPosition = staticPOS;
        
        if(PlayerMain.instance.dir == PlayerMoveDir.RIGHT)
        {
            transform.rotation = defaultRotation;
        }
        if(PlayerMain.instance.dir == PlayerMoveDir.LEFT)
        {
            Quaternion inverseRotation = defaultRotation * Quaternion.Euler(0, 0, 180);
            transform.rotation = inverseRotation;
        }
    }
}
