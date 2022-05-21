using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OptionManager : MonoBehaviour
{
    [SerializeField] private Canvas SoundVolumeCanvas;
    [SerializeField, Tooltip("�ŏ��ɑI�������{�^��")] private Selectable FirstSelect;
    [SerializeField, Tooltip("�U���ύX�I�u�W�F�N�g")] private GameObject VibrationObject;
    [SerializeField, Tooltip("�U���ύX�I�u�W�F�N�g")] private Slider VibrationSlider;
    [SerializeField, Tooltip("�U���ύX�I�u�W�F�N�g�̃n���h��")] private GameObject VibrationObjectHandle;
    //[SerializeField, Tooltip("�}�X�^�[�{�����[���X���C�_�[")] private Slider MasterVolumeSlider;
    [SerializeField, Tooltip("BGM�{�����[���X���C�_�[")] private Slider BGMVolumeSlider;
    [SerializeField, Tooltip("SE�{�����[���X���C�_�[")] private Slider SEVolumeSlider;
    [SerializeField, Tooltip("���j���[�}�l�[�W���[")] private MenuManager menuManager;
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
        if (SoundVolumeCanvas.gameObject.activeSelf)�@//�|�[�Y���j���[���A�N�e�B�u�Ȃ�
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
                oldButton = nowButton;
            }

            // �U��ONOFF�{�^���N���b�N
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

    // �I�v�V�����̏I��
    public void EndPause()
    {
        SoundVolumeCanvas.gameObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        Time.timeScale = 1.0f;
        SoundManager.Instance.UnPauseSound();
        // �f�[�^�Z�[�u
        SaveDataManager.Instance.SaveData();

        SetSizeDown(nowButton);
        nowButton = null;
        oldButton = null;

        SoundManager.Instance.PlaySound("���艹");

        menuManager.OnMenu();
    }

    // �I�v�V�����̃X�^�[�g
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

    // �U���̃X���C�_�[�ɕω������������ɌĂ΂�鏈��
    public void VibrationSliderChange()
    {
        // �U���X�V
        if (VibrationSlider.value <= 0.5f)
        {
            VibrationManager.Instance.StartVibration(0.0f, 0.0f, 0.0f);
            VibrationManager.Instance.SetVibrationFlag(false);
            // �C���[�W�ύX
            SetSizeDown(VibrationObjectHandle);
        }
        else
        {
            VibrationManager.Instance.SetVibrationFlag(true);
            VibrationManager.Instance.StartVibration(0.5f, 0.5f, 0.4f);
            // �C���[�W�ύX
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
        // �T�C�Y�ύX
        //if (gameObject.GetComponent<Button>() != null)
        //{
        //    ButtonRect = gameObject.GetComponent<RectTransform>();
        //    ButtonRect.sizeDelta += new Vector2(70.0f, 8.0f);
        //}
        //ButtonChildRect = gameObject.transform.GetChild(0).GetComponent<RectTransform>();
        //ButtonChildRect.sizeDelta += new Vector2(70.0f, 8.0f);

        // �T�C�Y�ύX(�C���[�W�ύX)
        if (gameObject.GetComponent<ButtonTexture>() != null)
        {
            gameObject.GetComponent<ButtonTexture>().ChangeOnImage();
        }
        else
        {
            gameObject.transform.GetComponentInChildren<ButtonTexture>().ChangeOnImage();
        }

        SoundManager.Instance.PlaySound("���艹");
    }

    private void SetSizeDown(GameObject gameObject)
    {
        // �T�C�Y�ύX
        //if (gameObject.GetComponent<Button>() != null)
        //{
        //    ButtonRect = gameObject.GetComponent<RectTransform>();
        //    ButtonRect.sizeDelta += new Vector2(-70.0f, -8.0f);
        //}
        //ButtonChildRect = gameObject.transform.GetChild(0).GetComponent<RectTransform>();
        //ButtonChildRect.sizeDelta += new Vector2(-70.0f, -8.0f);

        // �T�C�Y�ύX(�C���[�W�ύX)
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
