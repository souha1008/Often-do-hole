using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum COLOR_STATE{ 
    Original,
    Clear
}


public class SelectUITextAlpha : MonoBehaviour
{
    Color OriginnalColor;
    Color ClearColor;

    COLOR_STATE ColorState = COLOR_STATE.Original;
    int ColorLerpCnt = 10;

    int LERP_MIN = 0;
    int LERP_MAX = 10;


    // Start is called before the first frame update
    void Start()
    {
        OriginnalColor = GetComponent<Text>().color;
        ClearColor = new Color(OriginnalColor.r, OriginnalColor.g, OriginnalColor.b, 0.0f);

        ColorState = COLOR_STATE.Original;
        ColorLerpCnt = 10;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if(ColorState == COLOR_STATE.Original)
        {
            ColorLerpCnt = Mathf.Clamp(ColorLerpCnt + 1, LERP_MIN, LERP_MAX);
        }
        else
        {
            ColorLerpCnt = Mathf.Clamp(ColorLerpCnt - 1, LERP_MIN, LERP_MAX);
        }

        GetComponent<Text>().color = new Color(OriginnalColor.r , OriginnalColor.g , OriginnalColor.b , (float)ColorLerpCnt / LERP_MAX) ;
    }

    public void SetOriginnal()
    {
        ColorState = COLOR_STATE.Original;
        //GetComponent<Text>().color = OriginnalColor;
    }

    public void SetClear()
    {
        ColorState = COLOR_STATE.Clear;
        //GetComponent<Text>().color = ClearColor; ;
    }

}
