using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeCountUp_Result : MonoBehaviour
{
    [SerializeField] Text timeText;
    [SerializeField] int anim_exitFlame = 120;     // アニメーション終了フレーム

    int flame_time = 0;     // フレームカウント
    bool anim_start;
    bool anim_end;

    void Awake()
    {
        anim_start = false;
        anim_end = false;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        flame_time++;

        if(flame_time >= anim_exitFlame)
        {
            //(TimeAnimetion(1f, GameStateManager.GetGameTime(), 4f));
            if (anim_start == false)
            {
                StartCoroutine(TimeAnimetion(1f, 185.0f, 4f));
                anim_start = true;
            }
            //if(anim_end == true)
            //{
            //    timeText.text = 
            //}
        }
    }

    private IEnumerator TimeAnimetion(float startScoreTime, float endScoreTime, float duratino)
    {
        float starttime = Time.time;
        float endtime = starttime + duratino;

        do
        {
            float timeRate = (Time.time - starttime) / duratino;

            float updateValue = (float)((endScoreTime - startScoreTime) * timeRate + startScoreTime);

            timeText.text = updateValue.ToString("f5");

            yield return null;
        } while (Time.time < endtime);

        timeText.text = endScoreTime.ToString("f5");
    }
}
