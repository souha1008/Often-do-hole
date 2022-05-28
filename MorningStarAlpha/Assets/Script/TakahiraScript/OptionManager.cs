using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OptionManager : MonoBehaviour
{
    [SerializeField] private Canvas SoundVolumeCanvas;
    [SerializeField, Tooltip("最初に選択されるボタン")] private Selectable FirstSelect;
    [SerializeField, Tooltip("振動変更オブジェクト")] private GameObject VibrationObject;
    [SerializeField, Tooltip("振動変更オブジェクト")] private Slider VibrationSlider;
    [SerializeField, Tooltip("振動変更オブジェクトのハンドル")] private GameObject VibrationObjectHandle;
    [SerializeField, Tooltip("BGMオブジェクトのハンドル")] private GameObject BGMObjectHandle;
    [SerializeField, Tooltip("SEオブジェクトのハンドル")] private GameObject SEObjectHandle;
    //[SerializeField, Tooltip("マスターボリュームスライダー")] private Slider MasterVolumeSlider;
    [SerializeField, Tooltip("BGMボリュームスライダー")] private Slider BGMVolumeSlider;
    [SerializeField, Tooltip("SEボリュームスライダー")] private Slider SEVolumeSlider;
    [SerializeField, Tooltip("メニューマネージャー")] private MenuManager menuManager;
    private GameObject oldButton;
    private GameObject nowButton;

    private bool StartOnceFlag = false;


    private void Start()
    {
        SoundVolumeCanvas.gameObject.SetActive(false);
        nowButton = EventSystem.current.gameObject;
        oldButton = null;
        VibrationSlider = VibrationObject.GetComponent<Slider>();

        Random.InitState(System.DateTime.Now.Millisecond); // 乱数初期化
                                                           //マウスロック
//#if !UNITY_EDITOR
//        Cursor.lockState = CursorLockMode.None;
//        Cursor.visible = false;
//#endif
    }


    // Update is called once per frame
    void Update()
    {
        if (SoundVolumeCanvas.gameObject.activeSelf)　//ポーズメニューがアクティブなら
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

            if (Input.GetButtonDown("Button_Select") || Input.GetButtonDown("ButtonB"))
            {
                EndPause();
            }
        }
    }

    // オプションの終了
    public void EndPause()
    {
        EventSystem.current.SetSelectedGameObject(null);
        Time.timeScale = 1.0f;
        SoundManager.Instance.UnPauseSound();
        // データセーブ
        SaveDataManager.Instance.SaveData();

        SetSizeDown(nowButton);
        nowButton = null;
        oldButton = null;

        //　音
        SoundManager.Instance.PlaySound("sound_41");

        // 振動
        VibrationManager.Instance.StartVibration(0.65f, 0.65f, 0.3f);

        StartOnceFlag = false;

        SoundVolumeCanvas.gameObject.SetActive(false);
        menuManager.OnMenu();
    }

    // オプションのスタート
    public void StartPause()
    {
        menuManager.OffMenu();

        SoundVolumeCanvas.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(FirstSelect.gameObject);
        Time.timeScale = 0.0f;

        oldButton = nowButton = EventSystem.current.currentSelectedGameObject;
        SetSizeUp(nowButton);

        SoundManager.Instance.PauseSound();
        VibrationManager.Instance.StopVibration();
        SoundVolumeInit();

        // 音
        SoundManager.Instance.PlaySound("sound_40");

        // 振動
        VibrationManager.Instance.StartVibration(0.65f, 0.65f, 0.3f);
        StartOnceFlag = true;   // 最初の一回フラグ切り替え
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
            if (StartOnceFlag) VibrationManager.Instance.StartVibration(0.0f, 0.0f, 0.0f);
            VibrationManager.Instance.SetVibrationFlag(false);
            // イメージ変更
            SetSizeDown(VibrationObjectHandle);
        }
        else
        {
            VibrationManager.Instance.SetVibrationFlag(true);
            if (StartOnceFlag) VibrationManager.Instance.StartVibration(0.7f, 0.7f, 0.4f);
            // イメージ変更
            SetSizeUp(VibrationObjectHandle);
        }
        if (StartOnceFlag) SoundManager.Instance.PlaySound("sound_05_オプション調節SE");
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
        // サイズ変更(イメージ変更)
        if (gameObject.GetComponent<ButtonTexture>() != null)
        {
            gameObject.GetComponent<ButtonTexture>().ChangeOnImage();
        }
        else
        {
            gameObject.transform.GetComponentInChildren<ButtonTexture>().ChangeOnImage();
        }
    }

    private void SetSizeDown(GameObject gameObject)
    {
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
