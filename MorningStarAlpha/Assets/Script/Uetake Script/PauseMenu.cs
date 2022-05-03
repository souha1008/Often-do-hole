using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

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
    private GameObject OldSelectPause;
    private GameObject oldButton;
    private GameObject nowButton;


    private void Awake()
    {
        PauseCanvas.gameObject.SetActive(false);
        SoundVolumeCanvas.gameObject.SetActive(false);
        oldButton = nowButton = OldSelectPause = EventSystem.current.gameObject;
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
            // �T�E���h�{�����[�����A�N�e�B�u�Ȃ�
            if (SoundVolumeCanvas.gameObject.activeSelf)
            {
                nowButton = EventSystem.current.currentSelectedGameObject;

                if (Object.ReferenceEquals(nowButton, oldButton) == false)
                {
                    oldButton.GetComponent<Image>().color = Color.white;
                    nowButton.GetComponent<Image>().color = Color.red;
                }

                if (Input.GetButtonDown("Button_Select"))
                {
                    EndPause();
                }

                oldButton = nowButton;
            }
            else
            {
                if (Input.GetButtonDown("Button_Select"))
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
    }

    public void ClickReturnSoundVolume()
    {
        PauseCanvas.gameObject.SetActive(true);
        SoundVolumeCanvas.gameObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(OldSelectPause);
    }
}