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
    [SerializeField, Tooltip("BGM�I�u�W�F�N�g�̃n���h��")] private GameObject BGMObjectHandle;
    [SerializeField, Tooltip("SE�I�u�W�F�N�g�̃n���h��")] private GameObject SEObjectHandle;
    //[SerializeField, Tooltip("�}�X�^�[�{�����[���X���C�_�[")] private Slider MasterVolumeSlider;
    [SerializeField, Tooltip("BGM�{�����[���X���C�_�[")] private Slider BGMVolumeSlider;
    [SerializeField, Tooltip("SE�{�����[���X���C�_�[")] private Slider SEVolumeSlider;
    [SerializeField, Tooltip("���j���[�}�l�[�W���[")] private MenuManager menuManager;
    private GameObject oldButton;
    private GameObject nowButton;

    private bool StartOnceFlag = false;


    private void Start()
    {
        SoundVolumeCanvas.gameObject.SetActive(false);
        nowButton = EventSystem.current.gameObject;
        oldButton = null;
        VibrationSlider = VibrationObject.GetComponent<Slider>();

        Random.InitState(System.DateTime.Now.Millisecond); // ����������
                                                           //�}�E�X���b�N
//#if !UNITY_EDITOR
//        Cursor.lockState = CursorLockMode.None;
//        Cursor.visible = false;
//#endif
    }


    // Update is called once per frame
    void Update()
    {
        if (SoundVolumeCanvas.gameObject.activeSelf)�@//�|�[�Y���j���[���A�N�e�B�u�Ȃ�
        {
            nowButton = EventSystem.current.currentSelectedGameObject;

            if (Object.ReferenceEquals(nowButton, oldButton) == false)
            {
                // �I����
                SoundManager.Instance.PlaySound("sound_04_�I����", 1.0f, 0.1f);

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
        EventSystem.current.SetSelectedGameObject(null);
        Time.timeScale = 1.0f;
        SoundManager.Instance.UnPauseSound();
        // �f�[�^�Z�[�u
        SaveDataManager.Instance.SaveData();

        SetSizeDown(nowButton);
        nowButton = null;
        oldButton = null;

        //�@��
        SoundManager.Instance.PlaySound("sound_41");

        // �U��
        VibrationManager.Instance.StartVibration(0.65f, 0.65f, 0.3f);

        StartOnceFlag = false;

        SoundVolumeCanvas.gameObject.SetActive(false);
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

        // ��
        SoundManager.Instance.PlaySound("sound_40");

        // �U��
        VibrationManager.Instance.StartVibration(0.65f, 0.65f, 0.3f);
        StartOnceFlag = true;   // �ŏ��̈��t���O�؂�ւ�
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
            if (StartOnceFlag) VibrationManager.Instance.StartVibration(0.0f, 0.0f, 0.0f);
            VibrationManager.Instance.SetVibrationFlag(false);
            // �C���[�W�ύX
            SetSizeDown(VibrationObjectHandle);
        }
        else
        {
            VibrationManager.Instance.SetVibrationFlag(true);
            if (StartOnceFlag) VibrationManager.Instance.StartVibration(0.7f, 0.7f, 0.4f);
            // �C���[�W�ύX
            SetSizeUp(VibrationObjectHandle);
        }
        if (StartOnceFlag) SoundManager.Instance.PlaySound("sound_05_�I�v�V��������SE");
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
            SoundManager.Instance.PlaySound("sound_05_�I�v�V��������BGM");
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
            SoundManager.Instance.PlaySound("sound_05_�I�v�V��������SE");
    }


    private void SetSizeUp(GameObject gameObject)
    {
        // �T�C�Y�ύX(�C���[�W�ύX)
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
        //MasterVolumeSlider.value = SoundManager.Instance.SoundVolumeMaster * 0.1f;
        BGMVolumeSlider.value = SoundManager.Instance.SoundVolumeBGM * 0.1f;
        SEVolumeSlider.value = SoundManager.Instance.SoundVolumeSE * 0.1f;

        VibrationSliderChange();
        BGMVolumeSliderChange();
        SEVolumeSliderChange();
    }
}
