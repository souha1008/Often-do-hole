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
    [SerializeField, Tooltip("枠イメージ素材")] private Image WakuImage;
    [SerializeField, Tooltip("最初に選択されるボタン")] private Selectable FirstSelect;
    [SerializeField, Tooltip("サウンドで最初に選択されるボタン")] private Selectable FirstSelectSound;
    [SerializeField, Tooltip("振動変更オブジェクト")] private GameObject VibrationObject;
    private GameObject OldSelectPause;
    private GameObject oldButton;
    private GameObject nowButton;

    private Slider VibrationSlider;
    private float VibrationSliderOldvalue;

    private RectTransform WakuImageRect;

    private RectTransform nowBottunRect;


    private void Awake()
    {
        PauseCanvas.gameObject.SetActive(false);
        SoundVolumeCanvas.gameObject.SetActive(false);
        oldButton = nowButton = OldSelectPause = EventSystem.current.gameObject;
        VibrationSlider = VibrationObject.GetComponent<Slider>();
        WakuImageRect = WakuImage.GetComponent<RectTransform>();
        nowBottunRect = nowButton.GetComponent<RectTransform>();
        VibrationSliderOldvalue = VibrationSlider.value;
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseCanvas.gameObject.activeSelf)　//ポーズメニューがアクティブなら
        {
            nowButton = EventSystem.current.currentSelectedGameObject;
            //Debug.Log(EventSystem.current.currentSelectedGameObject);

            if (Object.ReferenceEquals(nowButton, oldButton) == false)
            {
                if (oldButton.GetComponent<Image>() != null)
                    oldButton.GetComponent<Image>().color = Color.white;
                if (nowButton.GetComponent<Image>() != null)
                    nowButton.GetComponent<Image>().color = Color.red;
            }

            if (Input.GetButtonDown("Button_Select"))
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
                    //if (oldButton.GetComponent<Image>() != null)
                    //    oldButton.GetComponent<Image>().color = Color.white;
                    //if (nowButton.GetComponent<Image>() != null)
                    //    nowButton.GetComponent<Image>().color = Color.red;
                    nowBottunRect = nowButton.GetComponent<RectTransform>();
                }

                // 枠の位置変更
                WakuImageRect.position = nowBottunRect.position;
                WakuImageRect.sizeDelta = new Vector2(nowBottunRect.sizeDelta.x + 40.0f, nowBottunRect.sizeDelta.y + 5.0f);
                WakuImageRect.localScale = nowBottunRect.localScale;

                // 振動ONOFFボタンクリック
                if (Input.GetButtonDown("ButtonA") && VibrationObject == EventSystem.current.currentSelectedGameObject)
                {
                    ClickVibrationONOFF();
                    VibrationSliderOldvalue = VibrationSlider.value;
                }
                // 振動ONOFFボタンクリック(左右入力)
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
        nowButton = EventSystem.current.currentSelectedGameObject;
        FirstSelect.gameObject.GetComponent<Image>().color = Color.red;
        SoundManager.Instance.PauseSound();
        VibrationManager.Instance.StopVibration();
        VibrationSliderChange();    // 振動更新
    }

    public void ClickResume()
    {
        EndPause();
    }

    public void ClickRetry()
    {
        FadeManager.Instance.FadeStart(SceneManager.GetActiveScene().name, FADE_KIND.FADE_SCENECHANGE);
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
        EventSystem.current.SetSelectedGameObject(FirstSelectSound.gameObject);
        if (nowButton.GetComponent<Image>() != null)
            nowButton.GetComponent<Image>().color = Color.white;
        nowButton = EventSystem.current.currentSelectedGameObject;
    }

    public void ClickReturnSoundVolume()
    {
        PauseCanvas.gameObject.SetActive(true);
        SoundVolumeCanvas.gameObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(OldSelectPause);
        nowButton = EventSystem.current.currentSelectedGameObject;
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
        VibrationSliderChange();    // 振動更新
    }

    private void VibrationSliderChange()
    {
        // 振動更新
        VibrationSlider.value = CalculationScript.OneZeroChange(VibrationManager.Instance.VibrationFlag);
    }
}