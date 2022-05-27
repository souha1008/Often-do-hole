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
    [SerializeField, Tooltip("サウンドに遷移するためのボタン")] private Selectable PushSound;
    [SerializeField, Tooltip("サウンドで最初に選択されるボタン")] private Selectable FirstSelectSound;
    [SerializeField, Tooltip("振動変更オブジェクト")] private GameObject VibrationObject;
    [SerializeField, Tooltip("振動変更オブジェクト")] private Slider VibrationSlider;
    [SerializeField, Tooltip("振動変更オブジェクトのハンドル")] private GameObject VibrationObjectHandle;
    [SerializeField, Tooltip("BGMオブジェクトのハンドル")] private GameObject BGMObjectHandle;
    [SerializeField, Tooltip("SEオブジェクトのハンドル")] private GameObject SEObjectHandle;
    //[SerializeField, Tooltip("マスターボリュームスライダー")] private Slider MasterVolumeSlider;
    [SerializeField, Tooltip("BGMボリュームスライダー")] private Slider BGMVolumeSlider;
    [SerializeField, Tooltip("SEボリュームスライダー")] private Slider SEVolumeSlider;
    private GameObject oldButton;
    private GameObject nowButton;

    private bool StartOnceFlag = false;

    //private RectTransform ButtonRect;
    //private RectTransform ButtonChildRect;


    private void Start()
    {
        PauseCanvas.gameObject.SetActive(false);
        SoundVolumeCanvas.gameObject.SetActive(false);
        nowButton = EventSystem.current.gameObject;
        oldButton = null;
    }


    // Update is called once per frame
    void Update()
    {
        if (PauseCanvas.gameObject.activeSelf)　//ポーズメニューがアクティブなら
        {
            nowButton = EventSystem.current.currentSelectedGameObject;

            if (Object.ReferenceEquals(nowButton, oldButton) == false)
            {
                // 選択音
                SoundManager.Instance.PlaySound("sound_04_選択音", 1.0f, 0.1f);

                // サイズ変更
                SetSizeUp(nowButton);

                if (oldButton != null)
                {
                    SetSizeDown(oldButton);
                }
                oldButton = nowButton;
            }

            if (Input.GetButtonDown("Button_Select") || Input.GetButtonDown("ButtonB"))
            {
                EndPause();
            }
        }
        else //ポーズメニューが非アクティブなら
        {
            // サウンドボリューム設定がアクティブなら
            if (SoundVolumeCanvas.gameObject.activeSelf)
            {
                nowButton = EventSystem.current.currentSelectedGameObject;

                if (Object.ReferenceEquals(nowButton, oldButton) == false)
                {
                    // 選択音
                    SoundManager.Instance.PlaySound("sound_04_選択音", 1.0f, 0.1f);

                    // サイズ変更
                    SetSizeUp(nowButton);

                    if (oldButton != null)
                    {
                        SetSizeDown(oldButton);
                    }
                    oldButton = nowButton;
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
        EventSystem.current.SetSelectedGameObject(null);
        if (FadeManager.GetNowState() == FADE_STATE.FADE_NONE)
        {
            GameStateManager.SetGameState(GAME_STATE.PLAY);
            Time.timeScale = 1.0f;            
            SoundManager.Instance.UnPauseSound();
        }
        SetSizeDown(nowButton);
        nowButton = null;
        oldButton = null;

        SoundManager.Instance.PlaySound("sound_41");

        // データセーブ
        SaveDataManager.Instance.SaveData();

        StartOnceFlag = false;

        PauseCanvas.gameObject.SetActive(false);
        SoundVolumeCanvas.gameObject.SetActive(false);
    }

    void StartPause()
    {
        PauseCanvas.gameObject.SetActive(true);
        SoundVolumeCanvas.gameObject.SetActive(false);

        GameStateManager.SetGameState(GAME_STATE.PAUSE);
        EventSystem.current.SetSelectedGameObject(FirstSelect.gameObject);
        Time.timeScale = 0.0f;

        oldButton = nowButton = EventSystem.current.currentSelectedGameObject;
        SetSizeUp(nowButton);

        SoundManager.Instance.PauseSound();
        VibrationManager.Instance.StopVibration();
        SoundVolumeInit();

        SoundManager.Instance.PlaySound("sound_40");
        StartOnceFlag = true;
    }

    public void ClickResume()
    {
        SoundManager.Instance.PlaySound("sound_03_01");
        EndPause();
    }

    public void ClickRetry()
    {
        SoundManager.Instance.PlaySound("sound_03_01");
        GameStateManager.LoadNowStage();
        EndPause();
    }

    public void ClickBackStageSelect()
    {
        SoundManager.Instance.PlaySound("sound_03_01");
        GameStateManager.LoadStageSelect(true);
        EndPause();
    }

    public void ClickSoundVolume()
    {
        PauseCanvas.gameObject.SetActive(false);
        SoundVolumeCanvas.gameObject.SetActive(true);

        SetSizeDown(nowButton);
        EventSystem.current.SetSelectedGameObject(FirstSelectSound.gameObject);
        nowButton = oldButton = EventSystem.current.currentSelectedGameObject;
        SetSizeUp(nowButton);
        
        SoundManager.Instance.PlaySound("sound_03_01");
    }

    public void ClickReturnSoundVolume()
    {
        PauseCanvas.gameObject.SetActive(true);
        SoundVolumeCanvas.gameObject.SetActive(false);

        SetSizeDown(nowButton);
        EventSystem.current.SetSelectedGameObject(PushSound.gameObject);
        nowButton = oldButton = EventSystem.current.currentSelectedGameObject;
        SetSizeUp(nowButton);

        SoundManager.Instance.PlaySound("sound_03_01");
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

    // 振動のスライダーに変化があった時に呼ばれる処理
    public void VibrationSliderChange()
    {
        // 振動更新
        if (VibrationSlider.value <= 0.5f)
        {
            VibrationManager.Instance.StartVibration(0.0f, 0.0f, 0.0f);
            VibrationManager.Instance.SetVibrationFlag(false);
            // イメージ変更
            SetSizeDown(VibrationObjectHandle);
        }
        else
        {
            VibrationManager.Instance.SetVibrationFlag(true);
            VibrationManager.Instance.StartVibration(0.5f, 0.5f, 0.4f);
            // イメージ変更
            SetSizeUp(VibrationObjectHandle);
        }
        if (StartOnceFlag)
            SoundManager.Instance.PlaySound("sound_05_オプション調節SE");
    }

    //public void MasterVolumeSliderChange()
    //{
    //    SoundManager.Instance.SoundVolumeMaster = MasterVolumeSlider.value * 10.0f;
    //}

    public void BGMVolumeSliderChange()
    {
        if (BGMVolumeSlider.value < 0.5f)
            SetSizeDown(BGMObjectHandle);
        else
            SetSizeUp(BGMObjectHandle);
        SoundManager.Instance.SoundVolumeBGM = BGMVolumeSlider.value * 10.0f;
        SoundManager.Instance.UpdateVolume();
        if (StartOnceFlag)
            SoundManager.Instance.PlaySound("sound_05_オプション調節BGM");
    }

    public void SEVolumeSliderChange()
    {
        if (SEVolumeSlider.value < 0.5f)
            SetSizeDown(SEObjectHandle);
        else
            SetSizeUp(SEObjectHandle);
        SoundManager.Instance.SoundVolumeSE = SEVolumeSlider.value * 10.0f;
        SoundManager.Instance.SoundVolumeOBJECT = SEVolumeSlider.value * 10.0f;
        SoundManager.Instance.UpdateVolume();
        if (StartOnceFlag)
            SoundManager.Instance.PlaySound("sound_05_オプション調節SE");
    }


    private void SetSizeUp(GameObject gameObject)
    {
        // サイズ変更
        //if (gameObject.GetComponent<Button>() != null)
        //{
        //    ButtonRect = gameObject.GetComponent<RectTransform>();
        //    ButtonRect.sizeDelta += new Vector2(70.0f, 8.0f);
        //}
        //ButtonChildRect = gameObject.transform.GetChild(0).GetComponent<RectTransform>();
        //ButtonChildRect.sizeDelta += new Vector2(70.0f, 8.0f);

        // サイズ変更(イメージ変更)
        if (gameObject.GetComponent<ButtonTexture>() != null)
        {
            gameObject.GetComponent<ButtonTexture>().ChangeOnImage();
        }
        else
        {
            gameObject.transform.GetComponentInChildren<ButtonTexture>().ChangeOnImage();
        }

        //var Images = gameObject.transform.GetComponentsInChildren<Image>();

        //foreach(Image Image in Images)
        //{
        //    Image.color = new Color(Image.color.r, Image.color.g, Image.color.b, 1.0f);
        //}
    }

    private void SetSizeDown(GameObject gameObject)
    {
        // サイズ変更
        //if (gameObject.GetComponent<Button>() != null)
        //{
        //    ButtonRect = gameObject.GetComponent<RectTransform>();
        //    ButtonRect.sizeDelta += new Vector2(-70.0f, -8.0f);
        //}
        //ButtonChildRect = gameObject.transform.GetChild(0).GetComponent<RectTransform>();
        //ButtonChildRect.sizeDelta += new Vector2(-70.0f, -8.0f);

        // サイズ変更(イメージ変更)
        if (gameObject.GetComponent<ButtonTexture>() != null)
        {
            gameObject.GetComponent<ButtonTexture>().ChangeOffImage();
        }
        else
        {
            gameObject.transform.GetComponentInChildren<ButtonTexture>().ChangeOffImage();
        }
    }

    private void SoundVolumeInit()
    {
        VibrationSlider.value = CalculationScript.OneZeroChange(VibrationManager.Instance.GetVibrationFlag());
        //MasterVolumeSlider.value = SoundManager.Instance.SoundVolumeMaster * 0.1f;
        BGMVolumeSlider.value = SoundManager.Instance.SoundVolumeBGM * 0.1f;
        SEVolumeSlider.value = SoundManager.Instance.SoundVolumeSE * 0.1f;

        VibrationSliderChange();
        BGMVolumeSliderChange();
        SEVolumeSliderChange();
    }
}