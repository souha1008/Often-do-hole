
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StaffRoleManager : MonoBehaviour
{
    public GameObject BlackBoard;
    //　テキストのスクロールスピード
    private float textScrollSpeed = 110.0f;
    //　テキストの制限位置
    private float limitPosition = 1600f;
    //　エンドロールが停止したかどうか
    private bool isStopEndRoll;

    private const float FadeTime = 2f;

    private const float BlackTime = -76.6f;
    private float FadeTimer;
    private const float EndMusicTime = 5.0f;

    private bool isDisplayEnd;
    private bool once;



    private void Start()
    {
        isStopEndRoll = false;
        isDisplayEnd = false;
        once = false;
        FadeTimer = BlackTime;

        SoundManager.Instance.PlaySound("Ending", 0.6f, AudioReverbPreset.City);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(FadeTimer);
        //　エンドロールが終了した時
        if (isDisplayEnd)
        {
            if (once == false)
            {
                if(GameStateManager.Instance.PressAny())
                {
                    // 音停止
                    SoundManager.Instance.FadeSound(SOUND_FADE_TYPE.OUT, 1.0f, 0.0f, true);
                    FadeManager.Instance.FadeStart("Title_part2", FADE_KIND.FADE_SCENECHANGE);
                    once = true;
                }
            }
        }
        else
        {
            FadeTimer += Time.deltaTime;
            float alpha = 1.0f - (FadeTimer / FadeTime);
            alpha = Mathf.Clamp01(alpha);
            Color temp = BlackBoard.GetComponent<Image>().color;
            temp.a = alpha;
            BlackBoard.GetComponent<Image>().color = temp;

            if (FadeTimer >= EndMusicTime)
            {
                temp.a = 0.0f;
                BlackBoard.GetComponent<Image>().color = temp;
                isDisplayEnd = true;
            }
        }


        //スタッフロール移動
        if (!isStopEndRoll)
        {
            //　エンドロール用テキストがリミットを越えるまで動かす
            if (transform.localPosition.y <= limitPosition)
            {
                transform.position = new Vector2(transform.position.x, transform.position.y + textScrollSpeed * Time.deltaTime);
            }
            else
            {
                isStopEndRoll = true;
            }
        }
    }
}