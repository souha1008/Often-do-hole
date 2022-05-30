
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StaffRoleManager : MonoBehaviour
{
    //�@�e�L�X�g�̃X�N���[���X�s�[�h
    [SerializeField]
    private float textScrollSpeed = 30;
    //�@�e�L�X�g�̐����ʒu
    [SerializeField]
    private float limitPosition = 730f;
    //�@�G���h���[�����I���������ǂ���
    private bool isStopEndRoll;

    private bool once;

    private void Start()
    {
        isStopEndRoll = false;
        once = false;

        SoundManager.Instance.PlaySound("Ending", 0.6f, AudioReverbPreset.City);
    }

    // Update is called once per frame
    void Update()
    {
        //�@�G���h���[�����I��������
        if (isStopEndRoll)
        {
            if (once == false)
            {
                // ����~
                SoundManager.Instance.FadeSound(SOUND_FADE_TYPE.OUT, 1.0f, 0.0f, true);
                FadeManager.Instance.FadeStart("Title_part2", FADE_KIND.FADE_SCENECHANGE);
                once = true;
            }
        }
        else
        {
            //�@�G���h���[���p�e�L�X�g�����~�b�g���z����܂œ�����
            if (transform.position.y <= limitPosition)
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