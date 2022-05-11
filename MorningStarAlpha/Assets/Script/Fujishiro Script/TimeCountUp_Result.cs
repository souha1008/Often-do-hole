using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeCountUp_Result : MonoBehaviour
{
    [SerializeField] Text timeText;
    [SerializeField] int anim_exitFlame = 120;     // �A�j���[�V�����I���t���[��
    [SerializeField] float duration = 3;         // �A�j���[�V��������
    [SerializeField] float Start_Time = 999;       // ��ԍŏ��̎���

    [Header("�f�o�b�O�p")]
    [SerializeField] bool debug_Fixedtime;
    [SerializeField] float debug_maxTime;

    int flame_time = 0;     // �t���[���J�E���g
    bool anim_start;

    void Awake()
    {
        anim_start = false;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Jump") || Input.GetButton("Fire1"))
        {
            StopAllCoroutines();
            if (debug_Fixedtime)
            {
                timeText.text = debug_maxTime.ToString("f5");
            }
            else
            {
                timeText.text = GameStateManager.GetGameTime().ToString("f5");
            }
            anim_start = true;
            ResultManager.instance.anim_end = true;
        }

        flame_time++;

        if (anim_start == false)
        { 
            if(flame_time >= anim_exitFlame)
            { 
                //StartCoroutine(TimeAnimetion(1f, , 4f));

                // �f�o�b�O�p
                if (debug_Fixedtime)
                {
                    StartCoroutine(TimeAnimetion(Start_Time, debug_maxTime, duration));
                }
                else
                {
                    StartCoroutine(TimeAnimetion(Start_Time, GameStateManager.GetGameTime(), duration));
                }


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

        timeText.text = endScoreTime.ToString("f5");
        ResultManager.instance.anim_end = true;
    }
}
