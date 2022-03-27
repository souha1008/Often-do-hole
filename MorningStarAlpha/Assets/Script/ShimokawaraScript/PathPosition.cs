using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPosition : MonoBehaviour
{
    void Start()
    {
        this.transform.position = new Vector3(transform.position.x, transform.position.y, - CameraMainShimokawara.instance.CAMERA_DISTANCE);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(transform.position.x, transform.position.y, -CameraMainShimokawara.instance.CAMERA_DISTANCE);
    }
}
