using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title_p1_manager : MonoBehaviour
{
    [SerializeField] Camera camera;

    private bool transition;

    // Start is called before the first frame update
    void Start()
    {
        transition = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("A") || Input.GetButton("B")
            || Input.GetButton("X") || Input.GetButton("Y"))
        {
            transition = true;
        }

        if(transition == true)
        {
            //camera.transform.Rotate(new Vector3())
        }
    }
}
