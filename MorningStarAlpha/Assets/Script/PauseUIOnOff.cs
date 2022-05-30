using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PauseUIOnOff : MonoBehaviour
{
    float DontMoveTime;
    bool isStop;
    Image image;

    // Start is called before the first frame update
    void Start()
    {
        DontMoveTime = 0.0f;
        isStop = true;
        image = GetComponent<Image>();
        image.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        isStop = true;

        if (Mathf.Abs(PlayerMain.instance.adjustLeftStick.x) > 0.01f)
        {
            isStop = false;
        }

        if (Mathf.Abs(PlayerMain.instance.adjustLeftStick.y) > 0.01f)
        {
            isStop = false;
        }

        if (Input.GetButtonDown("Button_R"))
        {
            isStop = false;
        }



        if(isStop)
        {
            DontMoveTime = Mathf.Min(DontMoveTime + Time.deltaTime, 10.0f) ;
        }
        else
        {
            DontMoveTime = 0.0f;
        }

        if(DontMoveTime > 5.0f)
        {
            if (image.isActiveAndEnabled == false)
            {
                image.enabled = true;
            }
        }
        else
        {
            if (image.isActiveAndEnabled == true)
            {
                image.enabled = false;
            }
        }
    }
}
