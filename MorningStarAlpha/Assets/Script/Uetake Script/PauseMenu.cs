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
    [SerializeField, Tooltip("�ŏ��ɑI�������{�^��")] private Selectable FirstSelect;
    [SerializeField, Tooltip("�T�E���h�ōŏ��ɑI�������{�^��")] private Selectable FirstSelectSound;
    [SerializeField, Tooltip("�U���ύX�I�u�W�F�N�g")] private GameObject VibrationObject;
    [SerializeField, Tooltip("�}�X�^�[�{�����[���X���C�_�[")] private Slider MasterVolumeSlider;
    [SerializeField, Tooltip("BGM�{�����[���X���C�_�[")] private Slider BGMVolumeSlider;
    [SerializeField, Tooltip("SE�{�����[���X���C�_�[")] private Slider SEVolumeSlider;
    private GameObject OldSelectPause;
    private GameObject oldButton;
    private GameObject nowButton;

    private Slider VibrationSlider;

    private RectTransform ButtonRect;

    private RectTransform ButtonChildRect;


    private void Awake()
    {
        PauseCanvas.gameObject.SetActive(false);
        SoundVolumeCanvas.gameObject.SetActive(false);
        nowButton = OldSelectPause = EventSystem.current.gameObject;
        oldButton = null;
        VibrationSlider = VibrationObject.GetComponent<Slider>();

        VibrationSlider.value = CalculationScript.OneZeroChange(VibrationManager.Instance.VibrationFlag);
        MasterVolumeSlider.value = SoundManager.Instance.SoundVolumeMaster * 0.1f;
        BGMVolumeSlider.value = SoundManager.Instance.SoundVolumeBGM * 0.1f;
        SEVolumeSlider.value = SoundManager.Instance.SoundVolumeSE * 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseCanvas.gameObject.activeSelf)�@//�|�[�Y���j���[���A�N�e�B�u�Ȃ�
        {
            nowButton = EventSystem.current.currentSelectedGameObject;

            if (Object.ReferenceEquals(nowButton, oldButton) == false)
            {
                // �T�C�Y�ύX
                SetSizeUp(nowButton);

                if (oldButton != null)
                {
                    SetSizeDown(oldButton);
                }
            }

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
                    // �T�C�Y�ύX
                    SetSizeUp(nowButton);

                    if (oldButton != null)
                    {
                        SetSizeDown(oldButton);
                    }
                }

                // �U��ONOFF�{�^���N���b�N
                if (Input.GetButtonDown("ButtonA") && VibrationObject == EventSystem.current.currentSelectedGameObject)
                {
                    ClickVibrationONOFF();
                }

                // B�{�^���N���b�N
                if (Input.GetButtonDown("ButtonB") && FadeManager.GetNowState() == FADE_STATE.FADE_NONE)
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
        EventSystem.current.SetSelectedGameObject(FirstSelect.gameObject);
        Time.timeScale = 0.0f;
        nowButton = EventSystem.current.currentSelectedGameObject;
        SoundManager.Instance.PauseSound();
        VibrationManager.Instance.StopVibration();
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

        OldSelectPause = EventSystem.current.currentSelectedGameObject;
        oldButton = nowButton;
        EventSystem.current.SetSelectedGameObject(FirstSelectSound.gameObject);
        nowButton = EventSystem.current.currentSelectedGameObject;       
    }

    public void ClickReturnSoundVolume()
    {
        PauseCanvas.gameObject.SetActive(true);
        SoundVolumeCanvas.gameObject.SetActive(false);

        oldButton = nowButton;
        EventSystem.current.SetSelectedGameObject(OldSelectPause);
        nowButton = EventSystem.current.currentSelectedGameObject;
    }

    public void ClickVibrationONOFF()
    {
        if (VibrationSlider.value >= 0.5f)
        {
            VibrationSlider.value = 0;
        }
        else
        {
            VibrationSlider.value = 1;
        }
    }

    public void VibrationSliderChange()
    {
        // �U���X�V
        if (VibrationSlider.value <= 0.5f)
        {
            VibrationManager.Instance.StartVibration(0.0f, 0.0f, 0.0f);
            VibrationManager.Instance.VibrationFlag = false;
        }
        else
        {
            VibrationManager.Instance.VibrationFlag = true;
            VibrationManager.Instance.StartVibration(0.5f, 0.5f, 0.4f);
        }
    }

    public void MasterVolumeSliderChange()
    {
        SoundManager.Instance.SoundVolumeMaster = MasterVolumeSlider.value * 10.0f;
    }

    public void BGMVolumeSliderChange()
    {
        SoundManager.Instance.SoundVolumeBGM = BGMVolumeSlider.value * 10.0f;
    }

    public void SEVolumeSliderChange()
    {
        SoundManager.Instance.SoundVolumeSE = SEVolumeSlider.value * 10.0f;
        SoundManager.Instance.SoundVolumeOBJECT = SEVolumeSlider.value * 10.0f;
    }


    private void SetSizeUp(GameObject gameObject)
    {
        // �T�C�Y�ύX
        if (gameObject.GetComponent<Button>() != null)
        {
            ButtonRect = gameObject.GetComponent<RectTransform>();
            ButtonRect.sizeDelta += new Vector2(70.0f, 8.0f);
        }
        ButtonChildRect = gameObject.transform.GetChild(0).GetComponent<RectTransform>();
        ButtonChildRect.sizeDelta += new Vector2(70.0f, 8.0f);
    }

    private void SetSizeDown(GameObject gameObject)
    {
        // �T�C�Y�ύX
        if (gameObject.GetComponent<Button>() != null)
        {
            ButtonRect = gameObject.GetComponent<RectTransform>();
            ButtonRect.sizeDelta += new Vector2(-70.0f, -8.0f);
        }
        ButtonChildRect = gameObject.transform.GetChild(0).GetComponent<RectTransform>();
        ButtonChildRect.sizeDelta += new Vector2(-70.0f, -8.0f);
    }
}