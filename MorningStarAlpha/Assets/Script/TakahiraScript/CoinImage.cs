using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinImage : MonoBehaviour
{
    public Sprite[] NumberSprite = new Sprite[10]; // �����p�̃X�v���C�g

    // ���ォ��̐����̕\������1
    [Label("���S����̕�1")]�@public float WidthLength1;
    [Label("���S����̍���1")] public float HeightLength1;

    // ���ォ��̐����̕\������2
    [Label("���S����̕�2")] public float WidthLength2;
    [Label("���S����̍���2")] public float HeightLength2;


    [Label("�����ƕ����̊Ԋu")] public float NumLength;     // �����Ɛ����̊Ԋu(�P��)

    private Image[] NumImage;   // �����\���p�̃C���[�W

    // Start is called before the first frame update
    void Start()
    {
        // Image�擾
        NumImage = new Image[4];
        for (int i = 0; i < 4; i++)
        {
            NumImage[i] = transform.GetChild(i).gameObject.GetComponent<Image>();
        }

        // ���W����
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

        // ���W����(���W�m���߂�p)
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

            //��ԉ��̂������g��
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

            //��ԉ��̂������g��
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
