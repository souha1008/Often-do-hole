using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

/// <summary>
/// �Q�[���V�[�����̃|�[�Y���j���[�Ǘ��̃N���X
/// 
/// </summary>
public class PauseMenu : MonoBehaviour //�|�[�Y���j���[�L�����o�X�ɃA�^�b�`
{
    [SerializeField] private Canvas PauseCanvas;
    [SerializeField] private Canvas SoundVolumeCanvas;
    [SerializeField, Tooltip("�g�C���[�W�f��")] private Image WakuImage;
    [SerializeField, Tooltip("�ŏ��ɑI�������{�^��")] private Selectable FirstSelect;
    [SerializeField, Tooltip("�T�E���h�ōŏ��ɑI�������{�^��")] private Selectable FirstSelectSound;
    [SerializeField, Tooltip("�U���ύX�I�u�W�F�N�g")] private GameObject VibrationObject;
    private GameObject OldSelectPause;
    private GameObject oldButton;
    private GameObject nowButton;

    private Slider VibrationSlider;
    private float VibrationSliderOldvalue;

    private RectTransform WakuImageRect;

    private RectTransform nowButtunRect;


    private void Awake()
    {
        PauseCanvas.gameObject.SetActive(false);
        SoundVolumeCanvas.gameObject.SetActive(false);
        oldButton = nowButton = OldSelectPause = EventSystem.current.gameObject;
        VibrationSlider = VibrationObject.GetComponent<Slider>();
        WakuImageRect = WakuImage.GetComponent<RectTransform>();
        nowButtunRect = nowButton.GetComponent<RectTransform>();
        VibrationSliderOldvalue = VibrationSlider.value;
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseCanvas.gameObject.activeSelf)�@//�|�[�Y���j���[���A�N�e�B�u�Ȃ�
        {
            nowButton = EventSystem.current.currentSelectedGameObject;
            //Debug.Log(EventSystem.current.currentSelectedGameObject);

            if (Object.ReferenceEquals(nowButton, oldButton) == false)
            {
                //if (oldButton.GetComponent<Image>() != null)
                //    oldButton.GetComponent<Image>().color = Color.white;
                //if (nowButton.GetComponent<Image>() != null)
                //    nowButton.GetComponent<Image>().color = Color.red;
                nowButtunRect = nowButton.GetComponent<RectTransform>();
            }

            // �g�̈ʒu�ύX
            WakuImageRect.position = nowButtunRect.position;
            WakuImageRect.sizeDelta = new Vector2(nowButtunRect.sizeDelta.x + 40.0f, nowButtunRect.sizeDelta.y + 5.0f);
            WakuImageRect.localScale = nowButtunRect.localScale;

            if (Input.GetButtonDown("Button_Select") || Input.GetButtonDown("ButtonB"))
            {
                EndPause();
            }

            oldButton = nowButton;
        }
        else //�|�[�Y���j���[����A�N�e�B�u�Ȃ�
        {
            // �T�E���h�{�����[���ݒ肪�A�N�e�B�u�Ȃ�
            if (SoundVolumeCanvas.gameObject.activeSelf)
            {
                nowButton = EventSystem.current.currentSelectedGameObject;

                if (Object.ReferenceEquals(nowButton, oldButton) == false)
                {
                    //if (oldButton.GetComponent<Image>() != null)
                    //    oldButton.GetComponent<Image>().color = Color.white;
                    //if (nowButton.GetComponent<Image>() != null)
                    //    nowButton.GetComponent<Image>().color = Color.red;
                    nowButtunRect = nowButton.GetComponent<RectTransform>();
                }

                // �g�̈ʒu�ύX
                WakuImageRect.position = nowButtunRect.position;
                WakuImageRect.sizeDelta = new Vector2(nowButtunRect.sizeDelta.x + 40.0f, nowButtunRect.sizeDelta.y + 5.0f);
                WakuImageRect.localScale = nowButtunRect.localScale;

                // �U��ONOFF�{�^���N���b�N
                if (Input.GetButtonDown("ButtonA") && VibrationObject == EventSystem.current.currentSelectedGameObject)
                {
                    ClickVibrationONOFF();
                }
                // �U��ONOFF�{�^���N���b�N(���E����)
                if (VibrationSlider.value != VibrationSliderOldvalue)
                {
                    ClickVibrationONOFF();                 
                }

                // B�{�^���N���b�N
                if (Input.GetButtonDown("ButtonB"))
                {
                    ClickReturnSoundVolume();
                }

                if (Input.GetButtonDown("Button_Select") && FadeManager.GetNowState() == FADE_STATE.FADE_NONE)
                {
                    EndPause();
                }

                oldButton = nowButton;
            }
            else
            {
                if (Input.GetButtonDown("Button_Select") && FadeManager.GetNowState() == FADE_STATE.FADE_NONE)
                {
                    StartPause();
                }
            }
        }
    }


    void EndPause()
    {
        PauseCanvas.gameObject.SetActive(false);
        SoundVolumeCanvas.gameObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        if (FadeManager.GetNowState() == FADE_STATE.FADE_NONE)
        {
            GameStateManager.SetGameState(GAME_STATE.PLAY);
            Time.timeScale = 1.0f;            
            SoundManager.Instance.UnPauseSound();
        }
        
    }

    void StartPause()
    {
        GameStateManager.SetGameState(GAME_STATE.PAUSE);
        PauseCanvas.gameObject.SetActive(true);
        SoundVolumeCanvas.gameObject.SetActive(false);
        WakuImage.gameObject.transform.parent = PauseCanvas.gameObject.transform;
        EventSystem.current.SetSelectedGameObject(FirstSelect.gameObject);
        Time.timeScale = 0.0f;
        nowButton = EventSystem.current.currentSelectedGameObject;
        //FirstSelect.gameObject.GetComponent<Image>().color = Color.red;
        SoundManager.Instance.PauseSound();
        VibrationManager.Instance.StopVibration();
        VibrationSliderChange();    // �U���X�V
    }

    public void ClickResume()
    {
        EndPause();
    }

    public void ClickRetry()
    {
        FadeManager.Instance.FadeGameOver();
        EndPause();
    }

    public void ClickBackStageSelect()
    {
        EndPause();
        //SceneManager.LoadScene("StageSelectScene");
    }

    public void ClickSoundVolume()
    {
        PauseCanvas.gameObject.SetActive(false);
        SoundVolumeCanvas.gameObject.SetActive(true);
        WakuImage.gameObject.transform.parent = SoundVolumeCanvas.gameObject.transform;
        OldSelectPause = EventSystem.current.currentSelectedGameObject;
        EventSystem.current.SetSelectedGameObject(FirstSelectSound.gameObject);
        nowButton = EventSystem.current.currentSelectedGameObject;
    }

    public void ClickReturnSoundVolume()
    {
        PauseCanvas.gameObject.SetActive(true);
        SoundVolumeCanvas.gameObject.SetActive(false);
        WakuImage.gameObject.transform.parent = PauseCanvas.gameObject.transform;
        EventSystem.current.SetSelectedGameObject(OldSelectPause);
        nowButton = EventSystem.current.currentSelectedGameObject;
        nowButtunRect = nowButton.GetComponent<RectTransform>();
    }

    public void ClickVibrationONOFF()
    {
        if (VibrationManager.Instance.VibrationFlag)
        {
            VibrationManager.Instance.StartVibration(0.0f, 0.0f, 0.0f);
            VibrationManager.Instance.VibrationFlag = false;       
        }
        else
        {
            VibrationManager.Instance.VibrationFlag = true;
            VibrationManager.Instance.StartVibration(0.5f, 0.5f, 0.4f);
        }
        VibrationSliderChange();    // �U���X�V
    }

    private void VibrationSliderChange()
    {
        // �U���X�V
        VibrationSlider.value = CalculationScript.OneZeroChange(VibrationManager.Instance.VibrationFlag);
        VibrationSliderOldvalue = VibrationSlider.value;
}
}