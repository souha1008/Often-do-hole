using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

/// <summary>
/// ゲームシーン内のポーズメニュー管理のクラス
/// 
/// </summary>
public class PauseMenu : MonoBehaviour //ポーズメニューキャンバスにアタッチ
{
    [SerializeField] private Canvas PauseCanvas;
    [SerializeField] private Canvas SoundVolumeCanvas;
    [SerializeField, Tooltip("最初に選択されるボタン")] private Selectable FirstSelect;
    [SerializeField, Tooltip("サウンドで最初に選択されるボタン")] private Selectable FirstSelectSound;
    [SerializeField, Tooltip("振動変更オブジェクト")] private GameObject VibrationObject;
    [SerializeField, Tooltip("マスターボリュームスライダー")] private Slider MasterVolumeSlider;
    [SerializeField, Tooltip("BGMボリュームスライダー")] private Slider BGMVolumeSlider;
    [SerializeField, Tooltip("SEボリュームスライダー")] private Slider SEVolumeSlider;
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
        if (PauseCanvas.gameObject.activeSelf)　//ポーズメニューがアクティブなら
        {
            nowButton = EventSystem.current.currentSelectedGameObject;

            if (Object.ReferenceEquals(nowButton, oldButton) == false)
            {
                // サイズ変更
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
        else //ポーズメニューが非アクティブなら
        {
            // サウンドボリューム設定がアクティブなら
            if (SoundVolumeCanvas.gameObject.activeSelf)
            {
                nowButton = EventSystem.current.currentSelectedGameObject;

                if (Object.ReferenceEquals(nowButton, oldButton) == false)
                {
                    // サイズ変更
                    SetSizeUp(nowButton);

                    if (oldButton != null)
                    {
                        SetSizeDown(oldButton);
                    }
                }

                // 振動ONOFFボタンクリック
                if (Input.GetButtonDown("ButtonA") && VibrationObject == EventSystem.current.currentSelectedGameObject)
                {
                    ClickVibrationONOFF();
                }

                // Bボタンクリック
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
        // 振動更新
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
        // サイズ変更
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
        // サイズ変更
        if (gameObject.GetComponent<Button>() != null)
        {
            ButtonRect = gameObject.GetComponent<RectTransform>();
            ButtonRect.sizeDelta += new Vector2(-70.0f, -8.0f);
        }
        ButtonChildRect = gameObject.transform.GetChild(0).GetComponent<RectTransform>();
        ButtonChildRect.sizeDelta += new Vector2(-70.0f, -8.0f);
    }
}