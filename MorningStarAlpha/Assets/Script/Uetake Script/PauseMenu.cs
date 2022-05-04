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


    private void Awake()
    {
        PauseCanvas.gameObject.SetActive(false);
        SoundVolumeCanvas.gameObject.SetActive(false);
        oldButton = nowButton = OldSelectPause = EventSystem.current.gameObject;
        VibrationSlider = VibrationObject.GetComponent<Slider>();
        VibrationSliderOldvalue = VibrationSlider.value;
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseCanvas.gameObject.activeSelf)�@//�|�[�Y���j���[���A�N�e�B�u�Ȃ�
        {
            nowButton = EventSystem.current.currentSelectedGameObject;
            Debug.Log(EventSystem.current.currentSelectedGameObject);

            if (Object.ReferenceEquals(nowButton, oldButton) == false)
            {
                oldButton.GetComponent<Image>().color = Color.white;
                nowButton.GetComponent<Image>().color = Color.red;
                Debug.Log("change");
            }

            if (Input.GetButtonDown("Button_Select"))
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
                    // �g�̈ʒu�ύX
                    WakuImage.gameObject.transform.position = nowButton.transform.position;
                    WakuImage.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(nowButton.GetComponent<RectTransform>().sizeDelta.x + 40.0f, nowButton.GetComponent<RectTransform>().sizeDelta.y + 5.0f);
                    WakuImage.transform.localScale = nowButton.transform.localScale;
                }

                // �U��ONOFF�{�^���N���b�N
                if (Input.GetButtonDown("ButtonA") && VibrationObject == EventSystem.current.currentSelectedGameObject)
                {
                    ClickVibrationONOFF();
                    VibrationSliderOldvalue = VibrationSlider.value;
                }
                // �U��ONOFF�{�^���N���b�N(���E����)
                if (VibrationSlider.value != VibrationSliderOldvalue)
                {
                    ClickVibrationONOFF();
                    VibrationSliderOldvalue = VibrationSlider.value;                   
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
        GameStateManager.SetGameState(GAME_STATE.PLAY);
        PauseCanvas.gameObject.SetActive(false);
        SoundVolumeCanvas.gameObject.SetActive(false);
        Time.timeScale = 1.0f;
        EventSystem.current.SetSelectedGameObject(null);
        SoundManager.Instance.UnPauseSound();
    }

    void StartPause()
    {
        GameStateManager.SetGameState(GAME_STATE.PAUSE);
        PauseCanvas.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(FirstSelect.gameObject);
        Time.timeScale = 0.0f;
        oldButton = nowButton = EventSystem.current.currentSelectedGameObject;
        FirstSelect.gameObject.GetComponent<Image>().color = Color.red;
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
        FadeManager.Instance.SetNextFade(FADE_STATE.FADE_OUT, FADE_KIND.FADE_GAMOVER);
    }

    public void ClickBackStageSelect()
    {
        EndPause();
        SceneManager.LoadScene("StageSelectScene");
    }

    public void ClickSoundVolume()
    {
        PauseCanvas.gameObject.SetActive(false);
        SoundVolumeCanvas.gameObject.SetActive(true);
        OldSelectPause = EventSystem.current.currentSelectedGameObject;
        EventSystem.current.SetSelectedGameObject(FirstSelectSound.gameObject);
        // �g���W�ύX
        WakuImage.gameObject.transform.position = nowButton.transform.position;
        WakuImage.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(nowButton.GetComponent<RectTransform>().sizeDelta.x + 40.0f, nowButton.GetComponent<RectTransform>().sizeDelta.y + 5.0f);
        WakuImage.transform.localScale = nowButton.transform.localScale;
    }

    public void ClickReturnSoundVolume()
    {
        PauseCanvas.gameObject.SetActive(true);
        SoundVolumeCanvas.gameObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(OldSelectPause);
    }

    public void ClickVibrationONOFF()
    {
        if (VibrationManager.Instance.VibrationFlag)
        {
            VibrationManager.Instance.VibrationFlag = false;
            VibrationManager.Instance.StartVibration(0.0f, 0.0f, 0.0f);
        }
        else
        {
            VibrationManager.Instance.VibrationFlag = true;
            VibrationManager.Instance.StartVibration(0.5f, 0.5f, 0.25f);
        }
        VibrationSliderChange();    // �U���X�V
        Debug.LogWarning("��");
    }

    private void VibrationSliderChange()
    {
        // �U���X�V
        VibrationSlider.value = CalculationScript.OneZeroChange(VibrationManager.Instance.VibrationFlag);
    }
}