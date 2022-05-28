using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TimeCountUp_Result : MonoBehaviour
{
    [SerializeField] Text timeText;
    [SerializeField] int anim_exitFlame = 120;     // アニメーション終了フレーム
    [SerializeField] float duration = 3;         // アニメーション時間
    [SerializeField] float Start_Time = 999;       // 一番最初の時間

    [Header("デバッグ用")]
    [SerializeField] bool debug_check;
    [SerializeField] float debug_maxTime;

    enum ClearRank
    {
        Rank_S = 0,
        Rank_A,
        Rank_B,
    }
    [SerializeField] ClearRank clearRank;

    int flame_time = 0;     // フレームカウント
    bool anim_start;

    // タイム保存
    float scoreTime;


    void Awake()
    {
        anim_start = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        scoreTime = GameStateManager.GetGameTime();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetButtonDown("ButtonA") && ResultManager.instance.anim_end == false)
        //{
        //    StopAllCoroutines();
        //    DecimalPoint_Change();
        //    anim_start = true;
        //    ResultManager.instance.anim_end = true;
        //}

        flame_time++;

        if (anim_start == false)
        { 
            if(flame_time >= anim_exitFlame)
            { 
                //StartCoroutine(TimeAnimetion(1f, , 4f));

                // デバッグ用
                if (debug_check)
                {
                    StartCoroutine(TimeAnimetion(Start_Time, debug_maxTime, duration));
                }
                else
                {
                    StartCoroutine(TimeAnimetion(Start_Time, scoreTime, duration));
                }

                SoundManager.Instance.PlaySound("sound_44");

                anim_start = true;
            }
        }


    }

    private IEnumerator TimeAnimetion(float startScoreTime, float endScoreTime, float duration)
    {
        float starttime = Time.time;
        float endtime = starttime + duration;

        do
        {
            float timeRate = (Time.time - starttime) / duration;

            float updateValue = (float)((endScoreTime - startScoreTime) * timeRate + startScoreTime);

            timeText.text = updateValue.ToString("f5");

            yield return null;
        } while (Time.time < endtime);

        if (debug_check)
        {
            switch (clearRank)
            {
                case ClearRank.Rank_S:
                    timeText.text = endScoreTime.ToString("f5");
                    break;

                case ClearRank.Rank_A:
                    timeText.text = endScoreTime.ToString("f4");
                    break;

                case ClearRank.Rank_B:
                    timeText.text = endScoreTime.ToString("f3");
                    break;

            }
        }
        else
        {
            switch (GameStateManager.GetGameRank())
            {
                case GAME_RANK.S:
                    timeText.text = endScoreTime.ToString("f5");
                    break;

                case GAME_RANK.A:
                    timeText.text = endScoreTime.ToString("f4");
                    break;

                case GAME_RANK.B:
                    timeText.text = endScoreTime.ToString("f3");
                    break;
            }
        }
        //StringBuilder stringBuilder = new StringBuilder();
        //stringBuilder.Append("sec");
        //timeText.text += stringBuilder.ToString();
        timeText.text += "<size=60>sec</size>";
        ResultManager.instance.anim_end = true;
    }

    void DecimalPoint_Change()
    {
        if (debug_check)
        {
            switch (clearRank)
            {
                case ClearRank.Rank_S:
                    timeText.text = debug_maxTime.ToString("f5");
                    break;

                case ClearRank.Rank_A:
                    timeText.text = debug_maxTime.ToString("f4");
                    break;

                case ClearRank.Rank_B:
                    timeText.text = debug_maxTime.ToString("f3");
                    break;

            }
        }
        else
        {
            switch (GameStateManager.GetGameRank())
            {
                case GAME_RANK.S:
                    timeText.text = GameStateManager.GetGameTime().ToString("f5");
                    break;

                case GAME_RANK.A:
                    timeText.text = GameStateManager.GetGameTime().ToString("f4");
                    break;

                case GAME_RANK.B:
                    timeText.text = GameStateManager.GetGameTime().ToString("f3");
                    break;

            }
        }
        timeText.text += "<size=60>sec</size>";
    }
}
