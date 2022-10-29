using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingPictureFade : MonoBehaviour
{
    private enum END_FADE_STATE
    {
        FADE_IN,
        FADE_NONE,
        FADE_OUT,
        WAIT,
    }


    END_FADE_STATE fade;
    public int FadeTime;      //フェードにかかる時間
    public int DysplayTime;  //一枚の絵を表示する時間
    public int WaitTime;  //一枚の絵を表示する時間
    public GameObject[] pictures;

    private int pictureIndex = 0;  //表示している絵の番号
    private int FadeTimer = 0;     //カウント用
    private bool EndPicture = false;

    // Start is called before the first frame update
    void Start()
    {
        fade = END_FADE_STATE.WAIT;
        pictureIndex = 0;
        FadeTimer = 0;
        EndPicture = false;
        for (int i = 0; i < pictures.Length; i++)
        {
            pictures[i].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!EndPicture)
        {
            float alpha = 0.0f;


            if (fade == END_FADE_STATE.WAIT)
            {
                FadeTimer++;

                if (FadeTimer >= WaitTime)
                {
                    FadeTimer = 0;
                    fade = END_FADE_STATE.FADE_IN;
                }

            }
            if (fade == END_FADE_STATE.FADE_IN)
            {
                FadeTimer++;
                alpha = FadeTimer / (float)FadeTime;
                pictures[pictureIndex].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, alpha);

                if (FadeTimer >= FadeTime)
                {
                    FadeTimer = 0;
                    fade = END_FADE_STATE.FADE_NONE;
                }

            }
            else if (fade == END_FADE_STATE.FADE_NONE)
            {
                FadeTimer++;
                if (FadeTimer >= DysplayTime)
                {
                    FadeTimer = 0;
                    fade = END_FADE_STATE.FADE_OUT;
                }
                alpha = 1.0f;
            }
            else if (fade == END_FADE_STATE.FADE_OUT)
            {
                FadeTimer++;
                alpha = 1.0f - (FadeTimer / (float)FadeTime);
                pictures[pictureIndex].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, alpha);

                if (FadeTimer >= FadeTime)
                {
                    FadeTimer = 0;
                    fade = END_FADE_STATE.WAIT;

                    //pictures[pictureIndex].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                    pictureIndex++;
                    if (pictureIndex >= pictures.Length)
                    {
                        EndPicture = true;
                        pictureIndex = 0;
                    }
                }


            }
        }
    }
}
