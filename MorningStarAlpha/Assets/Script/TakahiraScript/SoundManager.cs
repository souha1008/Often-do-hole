using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#if UNITY_EDITOR
using UnityEditor;


// SoundManager�N���X���g��
[CustomEditor(typeof(SoundManager))]

public class SoundManagerEditor : Editor
{
    void OnEnable()
    {

    }
    public override void OnInspectorGUI()
    {
        SoundManager Sound = target as SoundManager;

        // �G�f�B�^�[�̕ύX�m�F
        EditorGUI.BeginChangeCheck();

        Sound.AudioSource_BGM_Max = EditorGUILayout.IntField("BGM�I�[�f�B�I�\�[�X�ő�l", Sound.AudioSource_BGM_Max);
        Sound.AudioSource_SE_Max = EditorGUILayout.IntField("SE�I�[�f�B�I�\�[�X�ő�l", Sound.AudioSource_SE_Max);


        Sound.SoundVolumeMaster = EditorGUILayout.Slider("�}�X�^�[�{�����[��", (int)(Sound.SoundVolumeMaster * 100), 0, 100) / 100.0f;
        Sound.SoundVolumeBGM = EditorGUILayout.Slider("BGM�{�����[��", (int)(Sound.SoundVolumeBGM * 100), 0, 100) / 100.0f;
        Sound.SoundVolumeSE = EditorGUILayout.Slider("SE�{�����[��", (int)(Sound.SoundVolumeSE * 100), 0, 100) / 100.0f;


        // �G�f�B�^�[�̕ύX�m�F
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(target); // �I���I�u�W�F�N�g�X�V
        }
    }
}
#endif


public enum SOUND_TYPE
{
    NULL = 0,
    BGM,
    SE
}

// �T�E���h�̃N���b�v�Ǘ��p
public class SOUND_CLIP
{
    public SOUND_CLIP(string audioName, AudioClip audioClip, SOUND_TYPE soundType)
    {
        AudioName = audioName;
        AudioClip = audioClip;
        SoundType = soundType;
    }
    public string AudioName;    // �N���b�v�ɕt���閼�O
    public AudioClip AudioClip; // �I�[�f�B�I�N���b�v
    public SOUND_TYPE SoundType;// ���̎��
}

// �T�E���h�̃\�[�X�Ǘ��p
public class SOUND_SOURCE
{
    public SOUND_SOURCE(AudioSource audioSource, float volume)
    {
        AudioSource = audioSource;
        Volume = volume;
        Mathf.Clamp(Volume, 0.0f, 1.0f);
    }
    public AudioSource AudioSource; // �I�[�f�B�I�\�[�X
    public float Volume;            // �I�[�f�B�I�\�[�X�̃{�����[��
}


public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    // �C���X�y�N�^�[�ŕ\���p
    public int AudioSource_BGM_Max = 3;   // �I�[�f�B�I�\�[�XBGM�̍ő吔(�C���X�y�N�^�[�ŕ\��)
    public int AudioSource_SE_Max = 10;   // �I�[�f�B�I�\�[�XSE�̍ő吔(�C���X�y�N�^�[�ŕ\��)

    public float SoundVolumeMaster = 1.0f; // �Q�[���S�̂̉���
    public float SoundVolumeSE = 1.0f;  // SE�̉���
    public float SoundVolumeBGM = 1.0f; // BGM�̉���


    // �����f�[�^�p
    private static int AudioSource_BGM_MAX;   // �I�[�f�B�I�\�[�XBGM�̍ő吔(static)
    private static int AudioSource_SE_MAX;    // �I�[�f�B�I�\�[�XSE�̍ő吔(static)


    public List<SOUND_CLIP> SoundList = new List<SOUND_CLIP>(); // �T�E���h�̃N���b�v�Ǘ��p�̃��X�g
    private SOUND_SOURCE[] AudioSource_BGM;  // �I�[�f�B�I�\�[�XBGM
    private SOUND_SOURCE[] AudioSource_SE;   // �I�[�f�B�I�\�[�XSE


    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject); // �V�[�����ς���Ă����ȂȂ�

        // ������
        AudioSource_BGM_MAX = AudioSource_BGM_Max;
        AudioSource_SE_MAX = AudioSource_SE_Max;
        AudioSource_BGM = new SOUND_SOURCE[AudioSource_BGM_MAX];
        AudioSource_SE = new SOUND_SOURCE[AudioSource_SE_MAX];

        for (int i = 0; i < AudioSource_BGM_MAX; i++)
        {
            AudioSource_BGM[i] = new SOUND_SOURCE(this.gameObject.AddComponent<AudioSource>(), 1.0f);
        }
        for (int i = 0; i < AudioSource_SE_MAX; i++)
        {
            AudioSource_SE[i] = new SOUND_SOURCE(this.gameObject.AddComponent<AudioSource>(), 1.0f);
        }
    }

    private void Start()
    {
        // �T�E���h�f�[�^�ǂݍ��ݏ���
        StartCoroutine(SetSEData());
        StartCoroutine(SetBGMData());

        // ���ʃZ�b�g
        SetVolume();
    }

    private IEnumerator SetSEData()
    {
        IList<AudioClip> AudioClipIlist; // �I�[�f�B�I�N���b�v�ǂݍ��ݗp

        // SE�f�[�^�ǂݍ���
        var handleSE = Addressables.LoadAssetsAsync<AudioClip>("SE", null);

        do
        {
            //Debug.Log("�ǂݍ��ݒ�");
            yield return null;
        }
        while (handleSE.Status != AsyncOperationStatus.Succeeded && handleSE.Status != AsyncOperationStatus.Failed);

        if (handleSE.Result != null)
        {
            AudioClipIlist = handleSE.Result;

            for (int i = 0; i < AudioClipIlist.Count; i++)
            {
                SoundList.Add(new SOUND_CLIP(AudioClipIlist[i].name, AudioClipIlist[i], SOUND_TYPE.SE));
                //Debug.Log(AudioClipIlist[i].name);
            }
            AudioClipIlist.Clear();

            Debug.Log("SE�f�[�^�̓ǂݍ��ݐ���");
        }
        else
        {
            Debug.Log("SE�f�[�^�̓ǂݍ��ݎ��s");
        }
    }

    private IEnumerator SetBGMData()
    {
        IList<AudioClip> AudioClipIlist; // �I�[�f�B�I�N���b�v�ǂݍ��ݗp


        //BGM�f�[�^�ǂݍ���
        var handleBGM = Addressables.LoadAssetsAsync<AudioClip>("BGM", null);

        do
        {
            //Debug.Log("�ǂݍ��ݒ�");
            yield return null;
        }
        while (handleBGM.Status != AsyncOperationStatus.Succeeded && handleBGM.Status != AsyncOperationStatus.Failed);

        if (handleBGM.Result != null)
        {
            AudioClipIlist = handleBGM.Result;

            for (int i = 0; i < AudioClipIlist.Count; i++)
            {
                SoundList.Add(new SOUND_CLIP(AudioClipIlist[i].name, AudioClipIlist[i], SOUND_TYPE.BGM));
            }
            AudioClipIlist.Clear();

            Debug.Log("BGM�f�[�^�̓ǂݍ��ݐ���");
        }
        else
        {
            Debug.Log("BGM�f�[�^�̓ǂݍ��ݎ��s");
        }
    }

    //===========================
    // ���̍Đ�
    //===========================
    public void PlaySound(string AudioName)
    {
        PlaySound(AudioName, 1.0f);
    }

    public void PlaySound(string AudioName, float Volume)
    {
        SOUND_CLIP Sound = new SOUND_CLIP("", null, SOUND_TYPE.NULL);

        // �I�[�f�B�I�̖��O�ƍ������̎擾
        for (int i = 0; i < SoundList.Count; i++)
        {
            if (SoundList[i].AudioName == AudioName)
            {
                Sound = SoundList[i];
                Debug.Log("��������");
            }
        }

        if (Sound.AudioClip == null)
            return;

        // �Đ�
        switch (Sound.SoundType)
        {
            case SOUND_TYPE.NULL:
                break;
            case SOUND_TYPE.BGM:
                for (int i = 0; i < AudioSource_BGM_MAX; i++)
                {
                    if (!AudioSource_BGM[i].AudioSource.isPlaying)
                    {
                        AudioSource_BGM[i].AudioSource.clip = Sound.AudioClip;
                        AudioSource_BGM[i].AudioSource.Play();
                        AudioSource_BGM[i].AudioSource.loop = true;
                        AudioSource_BGM[i].Volume = Volume;
                        AudioSource_BGM[i].AudioSource.volume = Volume * SoundVolumeBGM;
                    }
                }
                break;
            case SOUND_TYPE.SE:
                for (int i = 0; i < AudioSource_SE_MAX; i++)
                {
                    if (!AudioSource_SE[i].AudioSource.isPlaying)
                    {
                        AudioSource_SE[i].AudioSource.PlayOneShot(Sound.AudioClip);
                        AudioSource_SE[i].AudioSource.loop = false;
                        AudioSource_SE[i].Volume = Volume;
                        AudioSource_SE[i].AudioSource.volume = Volume * SoundVolumeSE;
                    }
                }
                break;
        }
    }

    private void SetVolume()
    {
        Mathf.Clamp(SoundVolumeMaster, 0.0f, 1.0f);
        Mathf.Clamp(SoundVolumeBGM, 0.0f, 1.0f);
        Mathf.Clamp(SoundVolumeSE, 0.0f, 1.0f);

        AudioListener.volume = SoundVolumeMaster;

        for (int i = 0; i < AudioSource_BGM_MAX; i++)
        {
            AudioSource_BGM[i].AudioSource.volume = AudioSource_BGM[i].Volume * SoundVolumeBGM;
        }
        for (int i = 0; i < AudioSource_SE_MAX; i++)
        {
            AudioSource_SE[i].AudioSource.volume = AudioSource_SE[i].Volume * SoundVolumeSE;
        }
    }


    public void FixedUpdate()
    {
        // ���ʃZ�b�g
        SetVolume();
    }
}
