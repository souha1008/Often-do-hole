
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StaffRoleManager : MonoBehaviour
{
    public GameObject BlackBoard;
    //�@�e�L�X�g�̃X�N���[���X�s�[�h
    private float textScrollSpeed = 110.0f;
    //�@�e�L�X�g�̐����ʒu
    private float limitPosition = 1600f;
    //�@�G���h���[������~�������ǂ���
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
        //�@�G���h���[�����I��������
        if (isDisplayEnd)
        {
            if (once == false)
            {
                if(GameStateManager.Instance.PressAny())
                {
                    // ����~
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


        //�X�^�b�t���[���ړ�
        if (!isStopEndRoll)
        {
            //�@�G���h���[���p�e�L�X�g�����~�b�g���z����܂œ�����
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