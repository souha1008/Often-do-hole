using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogoFade : MonoBehaviour
{
    public Image MyImage;
    public float FadeTime = 1.0f;
    private float NowTime = 0.0f;
    private bool OnceFlag = false;
    public bool VoiceOnce = false;

    private void Update()
    {
        if (!OnceFlag)
        {
            if (NowTime > 0.5f)
            {
                OnceFlag = true;
                NowTime = 0.0f;
            }
            else
                NowTime += Time.deltaTime;
        }
        else if (NowTime < FadeTime)
        {
            float Alpha = 1.0f - (NowTime / FadeTime);
            MyImage.color = new Color(MyImage.color.r, MyImage.color.g, MyImage.color.b, Alpha);

            NowTime += Time.deltaTime;
        }
        else
        {
            if (VoiceOnce == false)
            {
                VoiceOnce = true;
                SoundManager.Instance.PlaySound("blueDiver");
            }
            MyImage.color = new Color(MyImage.color.r, MyImage.color.g, MyImage.color.b, 0);
        }
    }
}
