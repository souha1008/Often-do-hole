using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinImage : MonoBehaviour
{
    public Sprite[] NumberSprite = new Sprite[10]; // 数字用のスプライト

    // 左上からの数字の表示距離1
    [Label("中心からの幅1")]　public float WidthLength1;
    [Label("中心からの高さ1")] public float HeightLength1;

    // 左上からの数字の表示距離2
    [Label("中心からの幅2")] public float WidthLength2;
    [Label("中心からの高さ2")] public float HeightLength2;


    [Label("文字と文字の間隔")] public float NumLength;     // 数字と数字の間隔(単体)

    private Image[] NumImage;   // 数字表示用のイメージ

    // Start is called before the first frame update
    void Start()
    {
        // Image取得
        NumImage = new Image[4];
        for (int i = 0; i < 4; i++)
        {
            NumImage[i] = transform.GetChild(i).gameObject.GetComponent<Image>();
        }

        // 座標入力
        for (int i = 0; i < 2; i++)
        {
            float Length = NumLength * 0.5f;
            if (i == 1) Length *= -1;
            NumImage[i].gameObject.GetComponent<RectTransform>().localPosition = new Vector3(WidthLength1 + Length, HeightLength1);
        }

        for (int i = 0; i < 2; i++)
        {
            float Length = NumLength * 0.5f;
            if (i == 1) Length *= -1;
            NumImage[i + 2].gameObject.GetComponent<RectTransform>().localPosition = new Vector3(WidthLength2 + Length, HeightLength2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        AllCoinNumSet();
        GetCoinNumSet();

        // 座標入力(座標確かめる用)
        //for (int i = 0; i < 2; i++)
        //{
        //    float Length = NumLength * 0.5f;
        //    if (i == 1) Length *= -1;
        //    NumImage[i].gameObject.GetComponent<RectTransform>().localPosition = new Vector3(WidthLength1 + Length, HeightLength1);
        //}

        //for (int i = 0; i < 2; i++)
        //{
        //    float Length = NumLength * 0.5f;
        //    if (i == 1) Length *= -1;
        //    NumImage[i + 2].gameObject.GetComponent<RectTransform>().localPosition = new Vector3(WidthLength2 + Length, HeightLength2);
        //}
    }

    private void AllCoinNumSet()
    {
        for (int i = 0; i < 2; i++)
        {
            int Num = (int)CoinManager.Instance.SubCoin.AllCoin1 + (int)CoinManager.Instance.SubCoin.AllCoin2 + (int)CoinManager.Instance.SubCoin.AllCoin3;

            //一番下のけたを使う
            for (int j = 0; j < i; j++)
            {
                Num /= 10;
            }

            Num = Num % 10;

            NumImage[i + 2].sprite = NumberSprite[Num];

            //if (i == 1 && Num == 0) NumImage[i + 2].enabled = false;
            //else NumImage[i + 2].enabled = true;
        }
    }

    private void GetCoinNumSet()
    {
        for (int i = 0; i < 2; i++)
        {
            int Num = (int)CoinManager.Instance.SubCoin.AllGetCoin1 + (int)CoinManager.Instance.SubCoin.AllGetCoin2 + (int)CoinManager.Instance.SubCoin.AllGetCoin3;

            //一番下のけたを使う
            for (int j = 0; j < i; j++)
            {
                Num /= 10;
            }

            Num = Num % 10;

            NumImage[i].sprite = NumberSprite[Num];

            //if (i == 1 && Num == 0) NumImage[i].enabled = false;
            //else NumImage[i].enabled = true;
        }
    }
}
