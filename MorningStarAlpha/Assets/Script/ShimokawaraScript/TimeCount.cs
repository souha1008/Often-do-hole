using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeCount : MonoBehaviour
{
    public Sprite[] sp = new Sprite[10];

    public GameObject[] Num = new GameObject[3];

    public RectTransform CanvasObj;

    public float NumWidth;
    public float NumHeight;

    public float MigiYohaku;
    public float UeYohaku;

    public float YokoKankaku;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < Num.Length; i++)
        {
            Num[i].transform.localScale = new Vector3(NumWidth, NumHeight, 0);
        }

        Vector2 CanvasLeftTop = new Vector2(CanvasObj.rect.x ,
             CanvasObj.rect.y + CanvasObj.rect.height );

        Vector2 CanvasRightButtom = new Vector2(CanvasObj.rect.x + CanvasObj.rect.width ,
             CanvasObj.rect.y );

        for (int i = 0; i < Num.Length; i++)
        {
            Num[i].GetComponent<RectTransform>().localPosition = new Vector3(CanvasRightButtom.x - Num[i].transform.localScale.x * 0.5f - ((Num[i].transform.localScale.x + MigiYohaku) * i) - MigiYohaku 
                , CanvasLeftTop.y - Num[i].transform.localScale.y * 0.5f - UeYohaku, 0);
           
        }

        for (int i = 0; i < Num.Length; i++)
        {
            Num[i].GetComponent<Image>().sprite = sp[0];
        }
    }

    // Update is called once per frame
    void Update()
    {
        NumSet();
    }

    void NumSet()
    {
        for (int i = 0; i < Num.Length; i++)
        {
            int TempNum = (int)GameStateManager.GetGameTime();

            //ˆê”Ô‰º‚Ì‚¯‚½‚ðŽg‚¤
            for(int j = 0; j < i; j++)
            {
                TempNum /= 10;
            }

            TempNum = TempNum % 10;

            Num[i].GetComponent<Image>().sprite = sp[TempNum];
        }
    }

}