using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SkyBoxFade : MonoBehaviour
{
    public Skybox skybox;
    public Material fadeMaterial;

    public int NowStage = 0;
    int MaxStage = 8;

    public float NowColor = 0.0f;
    float AddColor = 0.02f;

    float[] GoColor = { 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 2.0f, 2.0f, 2.0f }; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        NowStage = Mathf.Min(MaxStage - 1, SelectManager.instance.NowSelectStage);
        if(NowColor != GoColor[NowStage])
        {
            NowColor = ComeFloat(NowColor , GoColor[NowStage], AddColor);
        }

        fadeMaterial.SetFloat("_value", NowColor);
    }

    float ComeFloat (float TransNum , float GoNum, float GoValue)
    {
        if(TransNum < GoNum)
        {
            TransNum = Mathf.Min(TransNum + GoValue, GoNum);
        }
        else
        {
            TransNum = Mathf.Max(TransNum - GoValue, GoNum);
        }

        return TransNum;
    }
}
