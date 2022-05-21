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
    //[SerializeField, Tooltip("マスターボリュームスライダー")] private Slider MasterVolumeSlider;
    [SerializeField, Tooltip("BGMボリュームスライダー")] private Slider BGMVolumeSlider;
    [SerializeField, Tooltip("SEボリュームスライダー")] private Slider SEVolumeSlider;
    [SerializeField, Tooltip("メニューマネージャー")] private MenuManager menuManager;
    private GameObject oldButton;
    private GameObject nowButton;

    //private RectTransform ButtonRect;
    //private RectTransform ButtonChildRect;


    private void Awake()
    {
        SoundVolumeCanvas.gameObject.SetActive(false);
        nowButton = EventSystem.current.gameObject;
        oldButton = null;
        VibrationSlider = VibrationObject.GetComponent<Slider>();
    }


    // Update is called once per frame
    void Update()
    {
        if (SoundVolumeCanvas.gameObject.activeSelf)　//ポーズメニューがアクティブなら
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
        SoundVolumeCanvas.gameObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        Time.timeScale = 1.0f;
        SoundManager.Instance.UnPauseSound();
        // データセーブ
        SaveDataManager.Instance.SaveData();

        SetSizeDown(nowButton);
        nowButton = null;
        oldButton = null;

        SoundManager.Instance.PlaySound("決定音");

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
    }

    //public void MasterVolumeSliderChange()
    //{
    //    SoundManager.Instance.SoundVolumeMaster = MasterVolumeSlider.value * 10.0f;
    //}

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

        SoundManager.Instance.PlaySound("決定音");
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
        VibrationSliderChange();
        //MasterVolumeSlider.value = SoundManager.Instance.SoundVolumeMaster * 0.1f;
        BGMVolumeSlider.value = SoundManager.Instance.SoundVolumeBGM * 0.1f;
        SEVolumeSlider.value = SoundManager.Instance.SoundVolumeSE * 0.1f;
    }
}
