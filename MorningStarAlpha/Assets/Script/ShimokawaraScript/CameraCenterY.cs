using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCenterY : MonoBehaviour
{
    [SerializeField] private GameObject PlayerObj;
    [SerializeField] private GameObject CameraObj;

    // Start is called before the first frame update
    void Start()
    {
        SeekPosition();
    }

    // Update is called once per frame
    void Update()
    {
        SeekPosition();
    }

    void SeekPosition()
    {
        float x = PlayerObj.transform.position.x;
        float y = CameraObj.transform.position.y;

        this.transform.position = new Vector3(x, y, 0);
    }
}
