using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeCount : MonoBehaviour
{
    public Sprite[] sp = new Sprite[10];

    public GameObject[] Num = new GameObject[3];

    public RectTransform CanvasObj;

    public float NumWidth;
    public float NumHeight;

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
            Num[i].GetComponent<RectTransform>().localPosition = new Vector3(CanvasRightButtom.x - Num[i].transform.localScale.x * 0.5f - (Num[i].transform.localScale.x * i)
                , CanvasLeftTop.y - Num[i].transform.localScale.y * 0.5f, 0);
           
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
