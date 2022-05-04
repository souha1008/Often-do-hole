using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
#endif

/// <summary>
/// 
///  �� ���ӎ��� ��
/// 
/// 
/// ���ʂ̒i�K [ Play���̉���(�o�͍ő�l) * SetVolume�̉��� * �e�^�C�v�̉���(BGM,SE...) * �}�X�^�[�̉���(�Q�[���S��) ] ���S�� 0.0f�`1.0f
/// 
/// 
/// ++++�g�p���@++++
/// 
/// 
/// SoundManager.Instance.����();
/// 
/// (��) SoundManager.Instance.PlaySound("���ʉ�1");
/// 
/// 
/// "�@"�ɓ���閼�O�̓t�@�C��������g���q������������
/// 
/// (��) Sound1.wav ���@Sound1
/// 
/// 
/// ++++�@�\�ꗗ++++
/// 
/// PlaySound()         �F   �Đ�
/// StopSound()         �F   ��~
/// PauseSound()        �F   �ꎞ��~
/// UnPauseSound()      �F   �ꎞ��~����
/// SetVolume()         �F   ���ʕύX
/// FadeSound()         �F   ���̃t�F�[�h
/// 
/// 
/// ���O���w��@���@��
/// �w�肵�Ȃ��@���@�S��
/// 
/// </summary>


#if UNITY_EDITOR
// SoundManager�N���X���g��
[CustomEditor(typeof(SoundManager))]

public class SoundManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SoundManager Sound = target as SoundManager;

        // �G�f�B�^�[�̕ύX�m�F
        EditorGUI.BeginChangeCheck();


        EditorGUILayout.LabelField("[���̓����Đ���MAX]");
        EditorGUILayout.Space(3);
        Sound.AudioSource_BGM_Max = EditorGUILayout.IntField("BGM�����Đ���", Sound.AudioSource_BGM_Max);
        Sound.AudioSource_SE_Max = EditorGUILayout.IntField("SE�����Đ���", Sound.AudioSource_SE_Max);
        Sound.AudioSource_OBJECT_Max = EditorGUILayout.IntField("OBJECT�����Đ���", Sound.AudioSource_OBJECT_Max);
        EditorGUILayout.Space(15);


        EditorGUILayout.LabelField("[���̃{�����[��]");
        EditorGUILayout.Space(3);
        Sound.SoundVolumeMaster = EditorGUILayout.Slider("�}�X�^�[�{�����[��", (int)(Sound.SoundVolumeMaster * 100), 0, 100) / 100.0f;
        Sound.SoundVolumeBGM = EditorGUILayout.Slider("BGM�{�����[��", (int)(Sound.SoundVolumeBGM * 100), 0, 100) / 100.0f;
        Sound.SoundVolumeSE = EditorGUILayout.Slider("SE�{�����[��", (int)(Sound.SoundVolumeSE * 100), 0, 100) / 100.0f;
        Sound.SoundVolumeOBJECT = EditorGUILayout.Slider("OBJECT�{�����[��", (int)(Sound.SoundVolumeOBJECT * 100), 0, 100) / 100.0f;
        EditorGUILayout.Space(15);


        EditorGUILayout.LabelField("[3D�T�E���h]");
        EditorGUILayout.Space(3);
        Sound.SoundMaxDistance3D = EditorGUILayout.FloatField("3D�T�E���h�̕�������ő勗��", Sound.SoundMaxDistance3D);
        EditorGUILayout.Space(15);


        EditorGUILayout.LabelField("[�T�E���h�ڍ׏��]");
        EditorGUILayout.Space(3);
        GUI.enabled = false;  // ���͕s��
        EditorGUILayout.IntField("���ݍĐ�����BGM��", Sound.NowPlaySoundBGM);
        EditorGUILayout.IntField("���ݍĐ�����SE��", Sound.NowPlaySoundSE);
        EditorGUILayout.IntField("���ݍĐ�����OBJECT��", Sound.NowPlaySoundOBJECT);
        GUI.enabled = true;  // ���͉\




        // �G�f�B�^�[�̕ύX�m�F
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(target); // �I���I�u�W�F�N�g�X�V
        }
    }
}
#endif

#if UNITY_EDITOR
// �A�Z�b�g�C���|�[�g���Ɏ����Őݒ肷��
public class AssetPostProcessorSound : AssetPostprocessor
{
    public void OnPostprocessAudio(AudioClip audioClip)
    {
        AudioImporter audioImporter = assetImporter as AudioImporter;
        string Path = assetImporter.assetPath;

        // BGM, OBJECT�T�E���h�̓o�b�N�O���E���h�ǂݍ��݂���
        audioImporter.loadInBackground |= Path.Contains("BGM");
        audioImporter.loadInBackground |= Path.Contains("OBJECT");

        // �^�O�Z�b�g
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        var group = settings.DefaultGroup;
        var AssetData = AssetDatabase.AssetPathToGUID(audioImporter.assetPath);
        var entry = group.GetAssetEntry(AssetData);
        if (Path.Contains("BGM"))
        {           
            entry.SetLabel("BGM", true, true);
        }
        else if (Path.Contains("SE"))
        {
            entry.SetLabel("SE", true, true);
        }
        else if (Path.Contains("OBJECT"))
        {
            entry.SetLabel("OBJECT", true, true);
        }
    }
}
#endif


public enum SOUND_TYPE
{
    NULL = 0,
    BGM,
    SE,
    OBJECT
}

public enum SOUND_FADE_TYPE
{
    NULL = 0,
    IN,
    OUT,
    OUT_IN
}

// �T�E���h�̃t�F�[�h�p�N���X
public class SOUND_FADE
{
    public SOUND_FADE(SOUND_FADE_TYPE fadeType, float fadeTime, float startVolume, float endVolume, bool soundStop)
    {
        FadeType = fadeType;
        FadeTime = fadeTime;
        NowTime = 0.0f;
        StartVolume = startVolume;
        EndVolume = endVolume;
        SoundStop = soundStop;
    }
    public SOUND_FADE_TYPE FadeType; // �t�F�[�h�̎��
    public float FadeTime;      // �t�F�[�h���鎞��
    public float NowTime;       // �o�ߎ���
    public float StartVolume;   // �J�n���̉���
    public float EndVolume;     // �I�����̉���
    public bool SoundStop;      // �t�F�[�h��ɉ��~�߂邩
}

// ���ݍĐ����̃T�E���h�Ǘ��p
public class NOW_PLAY_SOUND
{
    public NOW_PLAY_SOUND(SOUND_CLIP sound_Clip, SOUND_SOURCE sound_Source)
    {
        Sound_Clip = sound_Clip;
        Sound_Source = sound_Source;
        Sound_Fade = null;
    }
    public SOUND_CLIP Sound_Clip;
    public SOUND_SOURCE Sound_Source;
    public SOUND_FADE Sound_Fade;
}


// �T�E���h�̃N���b�v�Ǘ��p
public class SOUND_CLIP
{
    public SOUND_CLIP(string soundName, AudioClip audioClip, SOUND_TYPE soundType)
    {
        SoundName = soundName;
        AudioClip = audioClip;
        SoundType = soundType;
    }
    public string SoundName;    // �N���b�v�ɕt���閼�O
    public AudioClip AudioClip; // �I�[�f�B�I�N���b�v
    public SOUND_TYPE SoundType;// ���̎��
}

// �T�E���h�̃\�[�X�Ǘ��p
public class SOUND_SOURCE
{
    public SOUND_SOURCE(AudioSource audioSource)
    {
        AudioSource = audioSource;
        StartVolume = 1.0f;
        Volume = 1.0f;
        isPause = false;
        PauseTime = 0.0f;
        isUse = false;
        SoundObject = null;
    }
    public AudioSource AudioSource; // �I�[�f�B�I�\�[�X
    public float StartVolume;       // �Đ��J�n���̃I�[�f�B�I�\�[�X�̃{�����[��
    public float Volume;            // �Đ����̃I�[�f�B�I�\�[�X�̃{�����[��(�J�n���̉��ʂ����ɂ�����)
    public bool isPause;            // �|�[�Y���t���O
    public float PauseTime;         // �|�[�Y���̍Đ��ʒu
    public bool isUse;              // �g�p���t���O
    public GameObject SoundObject;  // ����炷�I�u�W�F�N�g(3D�T�E���h�̏ꍇ�g�p)
}


public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    // �C���X�y�N�^�[�ŕ\���p
    public int AudioSource_BGM_Max = 3;     // �I�[�f�B�I�\�[�XBGM�̍ő吔(�C���X�y�N�^�[�ŕ\��)
    public int AudioSource_SE_Max = 10;     // �I�[�f�B�I�\�[�XSE�̍ő吔(�C���X�y�N�^�[�ŕ\��)
    public int AudioSource_OBJECT_Max = 100; // �I�[�f�B�I�\�[�XOBJECT�̍ő吔(�C���X�y�N�^�[�ŕ\��)

    public float SoundVolumeMaster = 0.8f;  // �Q�[���S�̂̉���
    public float SoundVolumeBGM = 0.8f;     // BGM�̉���
    public float SoundVolumeSE = 0.8f;      // SE�̉���   
    public float SoundVolumeOBJECT = 0.8f;  // OBJECT�̉���

    public float SoundMaxDistance3D = 500.0f; // 3D�T�E���h�̕�������ő勗��

    public int NowPlaySoundBGM = 0;     // ���ݍĐ�����BGM��
    public int NowPlaySoundSE = 0;      // ���ݍĐ�����SE��
    public int NowPlaySoundOBJECT = 0;  // ���ݍĐ�����OBJECT��


    // �����f�[�^�p
    private static int AudioSource_BGM_MAX;   // �I�[�f�B�I�\�[�XBGM�̍ő吔(static)
    private static int AudioSource_SE_MAX;    // �I�[�f�B�I�\�[�XSE�̍ő吔(static)
    private static int AudioSource_OBJECT_MAX;// �I�[�f�B�I�\�[�XOBJECT�̍ő吔(static)


    public static List<SOUND_CLIP> SoundList = new List<SOUND_CLIP>(); // �T�E���h�̃N���b�v�Ǘ��p�̃��X�g
    private SOUND_SOURCE[] AudioSource_BGM;  // �I�[�f�B�I�\�[�XBGM
    private SOUND_SOURCE[] AudioSource_SE;   // �I�[�f�B�I�\�[�XSE
    private SOUND_SOURCE[] AudioSource_OBJECT; // �I�[�f�B�I�\�[�XOBJECT


    private List<NOW_PLAY_SOUND> NowPlaySoundList = new List<NOW_PLAY_SOUND>(); // �Đ����̃T�E���h�Ǘ��p���X�g

    private bool SoundLoadFlag = true;

    
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
        AudioSource_OBJECT_MAX = AudioSource_OBJECT_Max;
        AudioSource_BGM = new SOUND_SOURCE[AudioSource_BGM_MAX];
        AudioSource_SE = new SOUND_SOURCE[AudioSource_SE_MAX];
        AudioSource_OBJECT = new SOUND_SOURCE[AudioSource_OBJECT_MAX];
        this.gameObject.transform.position = Vector3.zero;

        for (int i = 0; i < AudioSource_BGM_MAX; i++)
        {
            // �I�[�f�B�I�\�[�XBGM�q�I�u�W�F�N�g�̐���
            var child = new GameObject("AudioSourceBGM_" + (i + 1)).transform;
            // �q��e�ɐݒ�
            child.SetParent(this.gameObject.transform);
            AudioSource_BGM[i] = new SOUND_SOURCE(child.gameObject.AddComponent<AudioSource>());

            // �D��x�Z�b�g
            AudioSource_BGM[i].AudioSource.priority = 1;

            // 3D�����̐ݒ�
            AudioSource_BGM[i].AudioSource.dopplerLevel = 0.0f; // �h�b�v���[���ʖ���
            AudioSource_BGM[i].AudioSource.rolloffMode = AudioRolloffMode.Linear; // ���̌����𒼐��ɂ���
            AudioSource_BGM[i].AudioSource.maxDistance = SoundMaxDistance3D;    // ���̕�������ő勗��
        }
        for (int i = 0; i < AudioSource_SE_MAX; i++)
        {
            // �I�[�f�B�I�\�[�XBGM�q�I�u�W�F�N�g�̐���
            var child = new GameObject("AudioSourceSE_" + (i + 1)).transform;
            // �q��e�ɐݒ�
            child.SetParent(this.gameObject.transform);
            AudioSource_SE[i] = new SOUND_SOURCE(child.gameObject.AddComponent<AudioSource>());

            // 3D�����̐ݒ�
            AudioSource_SE[i].AudioSource.dopplerLevel = 0.0f; // �h�b�v���[���ʖ���
            AudioSource_SE[i].AudioSource.rolloffMode = AudioRolloffMode.Linear; // ���̌����𒼐��ɂ���
            AudioSource_SE[i].AudioSource.maxDistance = SoundMaxDistance3D;    // ���̕�������ő勗��
        }
        for (int i = 0; i < AudioSource_OBJECT_MAX; i++)
        {
            // �I�[�f�B�I�\�[�XOBJECT�q�I�u�W�F�N�g�̐���
            var child = new GameObject("AudioSourceOBJECT_" + (i + 1)).transform;
            // �q��e�ɐݒ�
            child.SetParent(this.gameObject.transform);
            AudioSource_OBJECT[i] = new SOUND_SOURCE(child.gameObject.AddComponent<AudioSource>());

            // 3D�����̐ݒ�
            AudioSource_OBJECT[i].AudioSource.dopplerLevel = 0.0f; // �h�b�v���[���ʖ���
            AudioSource_OBJECT[i].AudioSource.rolloffMode = AudioRolloffMode.Linear; // ���̌����𒼐��ɂ���
            AudioSource_OBJECT[i].AudioSource.maxDistance = SoundMaxDistance3D;    // ���̕�������ő勗��
        }

        // �T�E���h�f�[�^�ǂݍ��ݏ���
        StartCoroutine(SetSEData());
        StartCoroutine(SetBGMData());
        StartCoroutine(SetOBJECTData());
    }

    private void Start()
    {
        // �Z�[�u�f�[�^���特�ʃf�[�^�ǂݍ���
        if (SaveDataManager.Instance != null)
        {
            //Debug.LogWarning("���ʓǂݍ���");
            SoundVolumeMaster = SaveDataManager.Instance.MainData.SoundVolumeMaster;
            SoundVolumeBGM = SaveDataManager.Instance.MainData.SoundVolumeBGM;
            SoundVolumeSE = SaveDataManager.Instance.MainData.SoundVolumeSE;
            SoundVolumeOBJECT = SaveDataManager.Instance.MainData.SoundVolumeOBJECT;
        }

        // ���ʃZ�b�g
        UpdateVolume();
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
                //Debug.Log(AudioClipIlist[i].name);
            }
            AudioClipIlist.Clear();

            Debug.Log("BGM�f�[�^�̓ǂݍ��ݐ���");
        }
        else
        {
            Debug.Log("BGM�f�[�^�̓ǂݍ��ݎ��s");
        }
        SoundLoadFlag = false;
    }

    private IEnumerator SetOBJECTData()
    {
        IList<AudioClip> AudioClipIlist; // �I�[�f�B�I�N���b�v�ǂݍ��ݗp

        //OBJECT�f�[�^�ǂݍ���
        var handleOBJECT = Addressables.LoadAssetsAsync<AudioClip>("OBJECT", null);

        do
        {
            //Debug.Log("�ǂݍ��ݒ�");
            yield return null;
        }
        while (handleOBJECT.Status != AsyncOperationStatus.Succeeded && handleOBJECT.Status != AsyncOperationStatus.Failed);

        if (handleOBJECT.Result != null)
        {
            AudioClipIlist = handleOBJECT.Result;

            for (int i = 0; i < AudioClipIlist.Count; i++)
            {
                SoundList.Add(new SOUND_CLIP(AudioClipIlist[i].name, AudioClipIlist[i], SOUND_TYPE.OBJECT));
            }
            AudioClipIlist.Clear();

            Debug.Log("OBJECT�f�[�^�̓ǂݍ��ݐ���");
        }
        else
        {
            Debug.Log("OBJECT�f�[�^�̓ǂݍ��ݎ��s");
        }
    }

    //=====================================================================
    // ���̍Đ�
    //=====================================================================
    // 
    // �������F�T�E���h�̃f�[�^���@(��) Sound1.wav�@���@Sound1
    // 
    // �������F����(0.0f�`1.0f) �������ʂɉ��ʕς����܂�
    // 
    // ��O�����F���̍Đ����Ԏw��(�Đ����Ԃ𒴂����ꍇ 0.0f)
    // 
    // ��l�����F���̍Đ��I�u�W�F�N�g(�w�肵�Ȃ��ꍇ2D,�����ꍇ3D)
    // 
    // ��܈����F���̔����ݒ�(AudioReverbPreset.�������Ŏw��o����)
    //
    // ��Z�����`�F�Đ����̉��̃t�F�[�h�@�\(������SoundFade�Q��)
    // 
    //
    // ���������I�[�o�[���[�h�����ׁA�g���������������g�������\
    //=====================================================================

    #region +++  PlaySound�̃I�[�o�[���[�h  +++

    public void PlaySound(string SoundName)
    {
        PlaySound(SoundName, 1.0f, 0.0f, null, AudioReverbPreset.Off, SOUND_FADE_TYPE.NULL, 0.0f, 1.0f, false);
    }
    public void PlaySound(string SoundName, float Volume)
    {
        PlaySound(SoundName, Volume, 0.0f, null, AudioReverbPreset.Off, SOUND_FADE_TYPE.NULL, 0.0f, 1.0f, false);
    }
    public void PlaySound(string SoundName, float Volume, float PlayTime)
    {
        PlaySound(SoundName, Volume, PlayTime, null, AudioReverbPreset.Off, SOUND_FADE_TYPE.NULL, 0.0f, 1.0f, false);
    }
    public void PlaySound(string SoundName, float Volume, float PlayTime, GameObject SoundObject)
    {
        PlaySound(SoundName, Volume, PlayTime, SoundObject, AudioReverbPreset.Off, SOUND_FADE_TYPE.NULL, 0.0f, 1.0f, false);
    }
    public void PlaySound(string SoundName, float Volume, float PlayTime, GameObject SoundObject, AudioReverbPreset ReverbPreset)
    {
        PlaySound(SoundName, Volume, PlayTime, SoundObject, ReverbPreset, SOUND_FADE_TYPE.NULL, 0.0f, 1.0f, false);
    }
    public void PlaySound(string SoundName, float Volume, float PlayTime, GameObject SoundObject, SOUND_FADE_TYPE FadeType, float FadeTime, float FadeEndVolume, bool SoundStop)
    {
        PlaySound(SoundName, Volume, PlayTime, SoundObject, AudioReverbPreset.Off, FadeType, FadeTime, FadeEndVolume, SoundStop);
    }
    public void PlaySound(string SoundName, float Volume, float PlayTime, AudioReverbPreset ReverbPreset)
    {
        PlaySound(SoundName, Volume, PlayTime, null, ReverbPreset, SOUND_FADE_TYPE.NULL, 0.0f, 1.0f, false);
    }
    public void PlaySound(string SoundName, float Volume, float PlayTime, AudioReverbPreset ReverbPreset, SOUND_FADE_TYPE FadeType, float FadeTime, float FadeEndVolume, bool SoundStop)
    {
        PlaySound(SoundName, Volume, PlayTime, null, ReverbPreset, FadeType, FadeTime, FadeEndVolume, SoundStop);
    }
    public void PlaySound(string SoundName, float Volume, float PlayTime, SOUND_FADE_TYPE FadeType, float FadeTime, float FadeEndVolume, bool SoundStop)
    {
        PlaySound(SoundName, Volume, PlayTime, null, AudioReverbPreset.Off, FadeType, FadeTime, FadeEndVolume, SoundStop);
    }
    public void PlaySound(string SoundName, float Volume, GameObject SoundObject)
    {
        PlaySound(SoundName, Volume, 0.0f, SoundObject, AudioReverbPreset.Off, SOUND_FADE_TYPE.NULL, 0.0f, 1.0f, false);
    }
    public void PlaySound(string SoundName, float Volume, GameObject SoundObject, AudioReverbPreset ReverbPreset)
    {
        PlaySound(SoundName, Volume, 0.0f, SoundObject, ReverbPreset, SOUND_FADE_TYPE.NULL, 0.0f, 1.0f, false);
    }
    public void PlaySound(string SoundName, float Volume, GameObject SoundObject, AudioReverbPreset ReverbPreset, SOUND_FADE_TYPE FadeType, float FadeTime, float FadeEndVolume, bool SoundStop)
    {
        PlaySound(SoundName, Volume, 0.0f, SoundObject, ReverbPreset, FadeType, FadeTime, FadeEndVolume, SoundStop);
    }
    public void PlaySound(string SoundName, float Volume, GameObject SoundObject, SOUND_FADE_TYPE FadeType, float FadeTime, float FadeEndVolume, bool SoundStop)
    {
        PlaySound(SoundName, Volume, 0.0f, SoundObject, AudioReverbPreset.Off, FadeType, FadeTime, FadeEndVolume, SoundStop);
    }
    public void PlaySound(string SoundName, float Volume, AudioReverbPreset ReverbPreset)
    {
        PlaySound(SoundName, Volume, 0.0f, null, ReverbPreset, SOUND_FADE_TYPE.NULL, 0.0f, 1.0f, false);
    }
    public void PlaySound(string SoundName, float Volume, AudioReverbPreset ReverbPreset, SOUND_FADE_TYPE FadeType, float FadeTime, float FadeEndVolume, bool SoundStop)
    {
        PlaySound(SoundName, Volume, 0.0f, null, ReverbPreset, FadeType, FadeTime, FadeEndVolume, SoundStop);
    }
    public void PlaySound(string SoundName, float Volume, SOUND_FADE_TYPE FadeType, float FadeTime, float FadeEndVolume, bool SoundStop)
    {
        PlaySound(SoundName, Volume, 0.0f, null, AudioReverbPreset.Off, FadeType, FadeTime, FadeEndVolume, SoundStop);
    }
    public void PlaySound(string SoundName, GameObject SoundObject)
    {
        PlaySound(SoundName, 1.0f, 0.0f, SoundObject, AudioReverbPreset.Off, SOUND_FADE_TYPE.NULL, 0.0f, 1.0f, false);
    }
    public void PlaySound(string SoundName, GameObject SoundObject, AudioReverbPreset ReverbPreset)
    {
        PlaySound(SoundName, 1.0f, 0.0f, SoundObject, ReverbPreset, SOUND_FADE_TYPE.NULL, 0.0f, 1.0f, false);
    }
    public void PlaySound(string SoundName, GameObject SoundObject, AudioReverbPreset ReverbPreset, SOUND_FADE_TYPE FadeType, float FadeTime, float FadeEndVolume, bool SoundStop)
    {
        PlaySound(SoundName, 1.0f, 0.0f, SoundObject, ReverbPreset, FadeType, FadeTime, FadeEndVolume, SoundStop);
    }
    public void PlaySound(string SoundName, GameObject SoundObject, SOUND_FADE_TYPE FadeType, float FadeTime, float FadeEndVolume, bool SoundStop)
    {
        PlaySound(SoundName, 1.0f, 0.0f, SoundObject, AudioReverbPreset.Off, FadeType, FadeTime, FadeEndVolume, SoundStop);
    }
    public void PlaySound(string SoundName, AudioReverbPreset ReverbPreset)
    {
        PlaySound(SoundName, 1.0f, 0.0f, null, ReverbPreset, SOUND_FADE_TYPE.NULL, 0.0f, 1.0f, false);
    }
    public void PlaySound(string SoundName, AudioReverbPreset ReverbPreset, SOUND_FADE_TYPE FadeType, float FadeTime, float FadeEndVolume, bool SoundStop)
    {
        PlaySound(SoundName, 1.0f, 0.0f, null, ReverbPreset, FadeType, FadeTime, FadeEndVolume, SoundStop);
    }
    public void PlaySound(string SoundName, SOUND_FADE_TYPE FadeType, float FadeTime, float FadeEndVolume, bool SoundStop)
    {
        PlaySound(SoundName, 1.0f, 0.0f, null, AudioReverbPreset.Off, FadeType, FadeTime, FadeEndVolume, SoundStop);
    }

    #endregion

    public void PlaySound(string SoundName, float Volume, float PlayTime, GameObject SoundObject, AudioReverbPreset ReverbPreset, SOUND_FADE_TYPE FadeType, float FadeTime, float FadeEndVolume, bool SoundStop)
    {
        StartCoroutine(PlaySoundIE(SoundName, Volume, PlayTime, SoundObject, ReverbPreset, FadeType, FadeTime, FadeEndVolume, SoundStop));
    }

    private IEnumerator PlaySoundIE(string SoundName, float Volume, float PlayTime, GameObject SoundObject, AudioReverbPreset ReverbPreset, SOUND_FADE_TYPE FadeType, float FadeTime, float FadeEndVolume, bool SoundStop)
    {
        do
        {
            //Debug.Log("�ǂݍ��ݒ�");
            yield return null;
        }
        while (SoundLoadFlag);

        SOUND_CLIP Sound_Clip = new SOUND_CLIP("", null, SOUND_TYPE.NULL);

        // �I�[�f�B�I�̖��O�ƍ������̎擾
        for (int i = 0; i < SoundList.Count; i++)
        {
            if (SoundList[i].SoundName == SoundName)
            {
                Sound_Clip = SoundList[i];
                //Debug.Log("��������");
                break;
            }
        }

        if (Sound_Clip.AudioClip == null)
            yield break;


        // ���ʂ͈͎̔w��
        Mathf.Clamp(Volume, 0.0f, 1.0f);

        // �Đ�
        switch (Sound_Clip.SoundType)
        {
            case SOUND_TYPE.NULL:
                break;
            case SOUND_TYPE.BGM:
                // �Đ�����BGM�Ɠ���BGM�𗬂����Ƃ����ꍇ�̍Đ����~����
                //for (int i = 0; i < NowPlaySoundList.Count; i++)
                //{
                //    if (NowPlaySoundList[i].Sound_Clip.SoundName == Sound_Clip.SoundName)
                //    {
                //        Debug.LogWarning("�Đ�����BGM�Ɠ������̂��Đ����悤�Ƃ������ߍĐ����~");
                //        return;
                //    }
                //}
                for (int i = 0; i < AudioSource_BGM_MAX; i++)
                {
                    if (!AudioSource_BGM[i].isUse)
                    {
                        NowPlaySoundList.Add(new NOW_PLAY_SOUND(Sound_Clip, AudioSource_BGM[i]));

                        StartSoundSource(NowPlaySoundList[NowPlaySoundList.Count - 1], Volume, PlayTime, SoundObject, ReverbPreset, true, FadeType);
                        FadeSound(Sound_Clip.SoundName, FadeType, FadeTime, FadeEndVolume, SoundStop);

                        yield break;
                    }
                }
                Debug.LogWarning("BGM���ő�Đ����𒴂����ׁA�Đ��o���܂���ł���");
                break;
            case SOUND_TYPE.SE:
                for (int i = 0; i < AudioSource_SE_MAX; i++)
                {
                    if (!AudioSource_SE[i].isUse)
                    {
                        NowPlaySoundList.Add(new NOW_PLAY_SOUND(Sound_Clip, AudioSource_SE[i]));

                        StartSoundSource(NowPlaySoundList[NowPlaySoundList.Count - 1], Volume, PlayTime, SoundObject, ReverbPreset, false, FadeType);
                        FadeSound(Sound_Clip.SoundName, FadeType, FadeTime, FadeEndVolume, SoundStop);

                        yield break;
                    }
                }
                Debug.LogWarning("SE���ő�Đ����𒴂����ׁA�Đ��o���܂���ł���");
                break;
            case SOUND_TYPE.OBJECT:
                for (int i = 0; i < AudioSource_OBJECT_MAX; i++)
                {
                    if (!AudioSource_OBJECT[i].isUse)
                    {
                        NowPlaySoundList.Add(new NOW_PLAY_SOUND(Sound_Clip, AudioSource_OBJECT[i]));

                        StartSoundSource(NowPlaySoundList[NowPlaySoundList.Count - 1], Volume, PlayTime, SoundObject, ReverbPreset, true, FadeType);
                        FadeSound(Sound_Clip.SoundName, FadeType, FadeTime, FadeEndVolume, SoundStop);

                        yield break;
                    }
                }
                Debug.LogWarning("OBJECT�����ő�Đ����𒴂����ׁA�Đ��o���܂���ł���");
                break;
        }
    }


    //===========================
    // ���̍Đ��I��
    //===========================
    public void StopSound()
    {
        // �Đ��I������
        for (int i = 0; i < NowPlaySoundList.Count;)
        {
            EndSoundSource(NowPlaySoundList[i]);
        }
    }
    public void StopSound(string SoundName)
    {
        // �I�[�f�B�I�̖��O�ƍ������̎擾���čĐ��I��
        for (int i = 0; i < NowPlaySoundList.Count; i++)
        {
            if (NowPlaySoundList[i].Sound_Clip.SoundName == SoundName)
            {
                EndSoundSource(NowPlaySoundList[i]);
                i -= 1;
            }
        }
    }


    //===========================
    // ���̈ꎞ��~
    //===========================
    public void PauseSound()
    {
        // �ꎞ��~����
        for (int i = 0; i < NowPlaySoundList.Count; i++)
        {
            if (!NowPlaySoundList[i].Sound_Source.isPause)
            {
                PauseSoundSource(NowPlaySoundList[i].Sound_Source);
            }            
        }
    }
    public void PauseSound(string SoundName)
    {
        // �I�[�f�B�I�̖��O�ƍ������̎擾���Ĉꎞ��~
        for (int i = 0; i < NowPlaySoundList.Count; i++)
        {
            if (!NowPlaySoundList[i].Sound_Source.isPause && NowPlaySoundList[i].Sound_Clip.SoundName == SoundName)
            {
                PauseSoundSource(NowPlaySoundList[i].Sound_Source);
            }
        }
    }


    //===========================
    // ���̈ꎞ��~����
    //===========================
    public void UnPauseSound()
    {
        // �ꎞ��~��������
        for (int i = 0; i < NowPlaySoundList.Count; i++)
        {
            if (NowPlaySoundList[i].Sound_Source.isPause)
            {
                UnPauseSoundSource(NowPlaySoundList[i].Sound_Source);
            }
        }
    }
    public void UnPauseSound(string SoundName)
    {
        // �I�[�f�B�I�̖��O�ƍ������̎擾���Ĉꎞ��~����
        for (int i = 0; i < NowPlaySoundList.Count; i++)
        {
            if (NowPlaySoundList[i].Sound_Source.isPause && NowPlaySoundList[i].Sound_Clip.SoundName == SoundName)
            {
                UnPauseSoundSource(NowPlaySoundList[i].Sound_Source);
            }
        }
    }


    //============================================================
    // �Đ����̑S�Ẳ��ʒ���(���������Ŏg����p)
    // 
    // ����(0.0f�`1.0f)���Đ��J�n���̉��ʂƂ͕ʂɐݒ艻
    //============================================================
    public void SetVolume(float Volume)
    {
        Mathf.Clamp(Volume, 0.0f, 1.0f);

        // ���ʒ���
        for (int i = 0; i < NowPlaySoundList.Count; i++)
        {
            NowPlaySoundList[i].Sound_Source.Volume = Volume; // �Đ����̉��ʃZ�b�g
        }
        UpdateVolume(); // ���ʍX�V
    }
    public void SetVolume(string SoundName, float Volume)
    {
        Mathf.Clamp(Volume, 0.0f, 1.0f);

        // �I�[�f�B�I�̖��O�ƍ������̎擾���ĉ��ʒ���
        for (int i = 0; i < NowPlaySoundList.Count; i++)
        {
            if (NowPlaySoundList[i].Sound_Clip.SoundName == SoundName)
            {
                NowPlaySoundList[i].Sound_Source.Volume = Volume; // �Đ����̉��ʃZ�b�g
            }
        }
        UpdateVolume(); // ���ʍX�V
    }


    //===========================================
    // ���̃t�F�[�h�@�\(�Đ����̉�)
    //
    // �������F�t�F�[�h���鉹�̃f�[�^���@(��) Sound1.wav�@���@Sound1
    // �������F�t�F�[�h�̎��
    // ��O�����F�t�F�[�h���鎞��
    // ��l�����F�t�F�[�h�㉹�̃{�����[��
    // ��܈����F�t�F�[�h�㉹���~�߂邩
    //===========================================
    public void FadeSound(SOUND_FADE_TYPE FadeType, float FadeTime, float FadeEndVolume, bool SoundStop)
    {
        if (FadeType == SOUND_FADE_TYPE.NULL || FadeTime < 0.01f) // �t�F�[�h���Ԃ�������������t�F�[�h���Ȃ�
            return;

        Mathf.Clamp(FadeEndVolume, 0.0f, 1.0f);

        // �Đ����̉��Ńt�F�[�h���łȂ��Ȃ�t�F�[�h�Z�b�g
        for (int i = 0; i < NowPlaySoundList.Count; i++)
        {
            if (NowPlaySoundList[i].Sound_Fade == null)
            {
                switch (FadeType)
                {
                    case SOUND_FADE_TYPE.IN:
                        if (NowPlaySoundList[i].Sound_Source.Volume >= FadeEndVolume)
                            continue;
                        break;
                    case SOUND_FADE_TYPE.OUT:
                        if (NowPlaySoundList[i].Sound_Source.Volume <= FadeEndVolume)
                            continue;
                        break;
                }
                NowPlaySoundList[i].Sound_Fade = new SOUND_FADE(FadeType, FadeTime, NowPlaySoundList[i].Sound_Source.Volume, FadeEndVolume, SoundStop);
            }
        }
    }
    public void FadeSound(string SoundName, SOUND_FADE_TYPE FadeType, float FadeTime, float FadeEndVolume, bool SoundStop)
    {
        if (FadeType == SOUND_FADE_TYPE.NULL || FadeTime < 0.01f) // �t�F�[�h���Ԃ�������������t�F�[�h���Ȃ�
            return;

        Mathf.Clamp(FadeEndVolume, 0.0f, 1.0f);

        // �Đ����̉��Ŗ��O����v���ăt�F�[�h���łȂ��Ȃ�t�F�[�h�Z�b�g
        for (int i = 0; i < NowPlaySoundList.Count; i++)
        {
            if (NowPlaySoundList[i].Sound_Clip.SoundName == SoundName && NowPlaySoundList[i].Sound_Fade == null)
            {
                switch (FadeType)
                {
                    case SOUND_FADE_TYPE.IN:
                        if (NowPlaySoundList[i].Sound_Source.Volume >= FadeEndVolume)
                            continue;
                        break;
                    case SOUND_FADE_TYPE.OUT:
                        if (NowPlaySoundList[i].Sound_Source.Volume <= FadeEndVolume)
                            continue;
                        break;
                }
                NowPlaySoundList[i].Sound_Fade = new SOUND_FADE(FadeType, FadeTime, NowPlaySoundList[i].Sound_Source.Volume, FadeEndVolume, SoundStop);
            }
        }
    }


    // �T�E���h�t�F�[�h�X�V����
    private void UpdateFadeSound()
    {
        for (int i = 0; i < NowPlaySoundList.Count; i++)
        {
            if (NowPlaySoundList[i].Sound_Fade != null && !NowPlaySoundList[i].Sound_Source.isPause) // �t�F�[�h���Ăă|�[�Y���ł͂Ȃ�
            {
                switch (NowPlaySoundList[i].Sound_Fade.FadeType)
                {
                    case SOUND_FADE_TYPE.IN:
                        if (SoundFadeIN(NowPlaySoundList[i]))
                            i -= 1;
                        break;
                    case SOUND_FADE_TYPE.OUT:
                        if (SoundFadeOUT(NowPlaySoundList[i]))
                            i -= 1;
                        break;
                    case SOUND_FADE_TYPE.OUT_IN:
                        if (SoundFadeOUT(NowPlaySoundList[i]))
                            i -= 1;
                        break;
                }
            }
        }
    }


    // �t�F�[�hIN����(�߂�l�F�T�E���h���Đ��I��������)
    private bool SoundFadeIN(NOW_PLAY_SOUND NowPlaySound)
    {
        if (NowPlaySound.Sound_Fade.NowTime <= NowPlaySound.Sound_Fade.FadeTime)
        {
            float Volume = NowPlaySound.Sound_Fade.NowTime / NowPlaySound.Sound_Fade.FadeTime; // ����

            SetVolume(NowPlaySound.Sound_Clip.SoundName, NowPlaySound.Sound_Fade.StartVolume + 
                (NowPlaySound.Sound_Fade.EndVolume - NowPlaySound.Sound_Fade.StartVolume) * Volume); // ���ʃZ�b�g

            NowPlaySound.Sound_Fade.NowTime += Time.unscaledDeltaTime; // ���ԉ��Z
        }
        else
        {
            switch (NowPlaySound.Sound_Fade.FadeType)
            {
                case SOUND_FADE_TYPE.IN:

                    if (NowPlaySound.Sound_Fade.SoundStop)
                    {
                        StopSound(NowPlaySound.Sound_Clip.SoundName); // �T�E���h��~
                        return true;
                    }

                    SetVolume(NowPlaySound.Sound_Clip.SoundName, NowPlaySound.Sound_Fade.EndVolume); // ���ʃZ�b�g
                    NowPlaySound.Sound_Fade = null; // �t�F�[�h�I��

                    break;
            }
        }
        return false;
    }


    // �t�F�[�h�A�E�g����(�߂�l�F�T�E���h���Đ��I��������)
    private bool SoundFadeOUT(NOW_PLAY_SOUND NowPlaySound)
    {
        if (NowPlaySound.Sound_Fade.NowTime <= NowPlaySound.Sound_Fade.FadeTime)
        {
            float Volume = 1.0f - (NowPlaySound.Sound_Fade.NowTime / NowPlaySound.Sound_Fade.FadeTime); // ����

            switch (NowPlaySound.Sound_Fade.FadeType)
            {
                case SOUND_FADE_TYPE.OUT:

                    SetVolume(NowPlaySound.Sound_Clip.SoundName, NowPlaySound.Sound_Fade.EndVolume +
                        (NowPlaySound.Sound_Fade.StartVolume - NowPlaySound.Sound_Fade.EndVolume) * Volume);   // ���ʃZ�b�g

                    break;
                case SOUND_FADE_TYPE.OUT_IN:

                    SetVolume(NowPlaySound.Sound_Clip.SoundName, NowPlaySound.Sound_Fade.StartVolume * Volume);   // ���ʃZ�b�g

                    break;
            }           

            NowPlaySound.Sound_Fade.NowTime += Time.unscaledDeltaTime; // ���ԉ��Z
        }
        else
        {
            switch (NowPlaySound.Sound_Fade.FadeType)
            {
                case SOUND_FADE_TYPE.OUT:

                    if (NowPlaySound.Sound_Fade.SoundStop)
                    {
                        StopSound(NowPlaySound.Sound_Clip.SoundName); // �T�E���h��~
                        return true;
                    }

                    SetVolume(NowPlaySound.Sound_Clip.SoundName, NowPlaySound.Sound_Fade.EndVolume); // ���ʃZ�b�g
                    NowPlaySound.Sound_Fade = null; // �t�F�[�h�I��

                    break;
                case SOUND_FADE_TYPE.OUT_IN:
                    SetVolume(NowPlaySound.Sound_Clip.SoundName, 0.0f); // ���ʐݒ�
                    NowPlaySound.Sound_Fade = new SOUND_FADE(SOUND_FADE_TYPE.IN, NowPlaySound.Sound_Fade.FadeTime, 0.0f, NowPlaySound.Sound_Fade.EndVolume, NowPlaySound.Sound_Fade.SoundStop);

                    break;
            }
        }
        return false;
    }


    //===========================================
    // ���݂̉��ʂ��X�V(���ʂ�ύX�����Ƃ��Ɏg�p)
    //===========================================
    public void UpdateVolume()
    {
        Mathf.Clamp(SoundVolumeMaster, 0.0f, 1.0f);
        Mathf.Clamp(SoundVolumeBGM, 0.0f, 1.0f);
        Mathf.Clamp(SoundVolumeSE, 0.0f, 1.0f);
        Mathf.Clamp(SoundVolumeOBJECT, 0.0f, 1.0f);

        AudioListener.volume = SoundVolumeMaster;
        for (int i = 0; i < NowPlaySoundList.Count; i++)
        {
            float SoundVol = 0.0f;

            switch (NowPlaySoundList[i].Sound_Clip.SoundType)
            {
                case SOUND_TYPE.BGM:
                    SoundVol = SoundVolumeBGM;
                    break;
                case SOUND_TYPE.SE:
                    SoundVol = SoundVolumeSE;
                    break;
                case SOUND_TYPE.OBJECT:
                    SoundVol = SoundVolumeOBJECT;
                    break;
                default:
                    break;
            }

            // ���ʐݒ�
            NowPlaySoundList[i].Sound_Source.AudioSource.volume =
                NowPlaySoundList[i].Sound_Source.StartVolume * NowPlaySoundList[i].Sound_Source.Volume * SoundVol;

            // ���̕�������ő勗���X�V
            NowPlaySoundList[i].Sound_Source.AudioSource.maxDistance = SoundMaxDistance3D;
        }

        // ���ʂ��Z�[�u�f�[�^�ɃZ�b�g
        SaveDataManager.Instance.MainData.SoundVolumeMaster= SoundVolumeMaster;
        SaveDataManager.Instance.MainData.SoundVolumeBGM = SoundVolumeBGM;
        SaveDataManager.Instance.MainData.SoundVolumeSE = SoundVolumeSE;
        SaveDataManager.Instance.MainData.SoundVolumeOBJECT = SoundVolumeOBJECT;
    }



    // �T�E���h�\�[�X�ꎞ��~����
    private void PauseSoundSource(SOUND_SOURCE Sound_Source)
    {
        Sound_Source.PauseTime = Sound_Source.AudioSource.time; // �|�[�Y���̎��ԃQ�b�g
        Sound_Source.AudioSource.Stop();// �Đ��ꎞ��~
        Sound_Source.isPause = true;    // �|�[�Y���t���O�I��
    }


    // �T�E���h�\�[�X�ꎞ��~��������
    private void UnPauseSoundSource(SOUND_SOURCE Sound_Source)
    {
        Sound_Source.AudioSource.time = Sound_Source.PauseTime; // �|�[�Y�̎��ԃZ�b�g
        Sound_Source.AudioSource.Play();// �Đ��ꎞ��~����
        Sound_Source.isPause = false;   // �|�[�Y���t���O�I�t
    }


    // �T�E���h�\�[�X���̊J�n����
    private void StartSoundSource(NOW_PLAY_SOUND NowPlaySound, float Volume, float PlayTime, GameObject SoundObject, AudioReverbPreset ReverbPreset, bool Loop, SOUND_FADE_TYPE FadeType)
    {
        if (SoundObject != null)
        {
            // 3D����
            NowPlaySound.Sound_Source.AudioSource.gameObject.transform.position = SoundObject.transform.position; // ���W�ύX
            NowPlaySound.Sound_Source.SoundObject = SoundObject; // �I�u�W�F�N�g�Z�b�g
            NowPlaySound.Sound_Source.AudioSource.spatialBlend = 1.0f; // 3D�̉����ɐ؂�ւ�
        }
        if (ReverbPreset != AudioReverbPreset.Off)
        {
            // �T�E���h�t�B���^�[�̒ǉ�
            AudioReverbFilter Filter =
                NowPlaySound.Sound_Source.AudioSource.gameObject.AddComponent<AudioReverbFilter>();
            Filter.reverbPreset = ReverbPreset;
        }

        NowPlaySound.Sound_Source.AudioSource.clip = NowPlaySound.Sound_Clip.AudioClip; // �I�[�f�B�I�N���b�v�Z�b�g
        NowPlaySound.Sound_Source.AudioSource.time = PlayTime;  // �Đ����ԃZ�b�g
        NowPlaySound.Sound_Source.StartVolume = Volume;
        if (FadeType == SOUND_FADE_TYPE.IN)
            NowPlaySound.Sound_Source.Volume = 0.0f;
        else
            NowPlaySound.Sound_Source.Volume = 1.0f;
        NowPlaySound.Sound_Source.AudioSource.Play();
        NowPlaySound.Sound_Source.AudioSource.loop = Loop;       
        NowPlaySound.Sound_Source.isPause = false;
        NowPlaySound.Sound_Source.isUse = true;

        UpdateVolume(); // ���ʍX�V
    }


    // �T�E���h�\�[�X���̏I������
    private void EndSoundSource(NOW_PLAY_SOUND NowPlaySound)
    {
        AudioReverbFilter Filter = null;

        NowPlaySound.Sound_Source.AudioSource.gameObject.transform.position = Vector3.zero;  // ���W�ύX
        NowPlaySound.Sound_Source.SoundObject = null;    // �I�u�W�F�N�gnull
        NowPlaySound.Sound_Source.AudioSource.spatialBlend = 0.0f;   // 2D�̉����ɐ؂�ւ�
        if ((Filter = NowPlaySound.Sound_Source.AudioSource.gameObject.GetComponent<AudioReverbFilter>()) != null)
        {
            // �T�E���h�t�B���^�[�̔j��
            Destroy(Filter);
        }

        NowPlaySound.Sound_Source.AudioSource.Stop();        // �Đ���~
        NowPlaySound.Sound_Source.AudioSource.clip = null;   // �N���b�v��null�����
        NowPlaySound.Sound_Source.AudioSource.time = 0.0f;   // �Đ����ԃ��Z�b�g
        NowPlaySound.Sound_Source.StartVolume = 1.0f;        // �J�n���̉��ʃ��Z�b�g
        NowPlaySound.Sound_Source.Volume = 1.0f;             // �g�p���̉��ʃ��Z�b�g
        NowPlaySound.Sound_Source.isPause = false;           // �|�[�Y���t���O�I�t
        NowPlaySound.Sound_Source.PauseTime = 0.0f;          // �|�[�Y���̎��ԃ��Z�b�g
        NowPlaySound.Sound_Source.isUse = false;             // �g�p�t���O�I�t

        NowPlaySoundList.Remove(NowPlaySound);               // �Đ������X�g�������
    }


    // �Đ������菈��
    private void UpdateNowUse()
    {
        for (int i = 0; i < NowPlaySoundList.Count; i++)
        {
            if (!NowPlaySoundList[i].Sound_Source.isPause && !NowPlaySoundList[i].Sound_Source.AudioSource.isPlaying)
            {
                EndSoundSource(NowPlaySoundList[i]);
                i -= 1;
            }
        }
    }


    // 3D�T�E���h�̉��������W�X�V����
    private void Update3DPos()
    {
        for (int i = 0; i < NowPlaySoundList.Count; i++)
        {
            if (NowPlaySoundList[i].Sound_Source.SoundObject != null)
            {
                // ���W�X�V
                NowPlaySoundList[i].Sound_Source.AudioSource.gameObject.transform.position =
                    NowPlaySoundList[i].Sound_Source.SoundObject.transform.position;
            }
        }
    }

    // �C���X�y�N�^�[�̃T�E���h�ڍ׏��X�V����
    private void UpdateSoundInspector()
    {
        int BGMNum = 0;
        int SENum = 0;
        int OBJECTNum = 0;

        for (int i = 0; i < NowPlaySoundList.Count; i++)
        {
            switch (NowPlaySoundList[i].Sound_Clip.SoundType)
            {
                case SOUND_TYPE.BGM:
                    BGMNum++;
                    break;
                case SOUND_TYPE.SE:
                    SENum++;
                    break;
                case SOUND_TYPE.OBJECT:
                    OBJECTNum++;
                    break;
            }
        }
        NowPlaySoundBGM = BGMNum;
        NowPlaySoundSE = SENum;
        NowPlaySoundOBJECT = OBJECTNum;
    }


    public void Update()
    {
        // ���ʃZ�b�g�@���@�ŏI�I�ɉ��ʃI�v�V�����ŕύX������\��
        UpdateVolume();

        // ���ݎg�p�������菈��
        UpdateNowUse();

        // 3D�̉��������W�X�V
        Update3DPos();

        // �T�E���h�̃t�F�[�h�X�V
        UpdateFadeSound();

#if UNITY_EDITOR
        // �T�E���h�̃C���X�y�N�^�[�p�ڍ׏��X�V
        UpdateSoundInspector();
#endif


        // �e�X�g�p���̓L�[

        if (Input.GetKeyDown(KeyCode.A)) // �ꎞ��~
        {
            PauseSound();
        }

        if (Input.GetKeyDown(KeyCode.S)) // �ꎞ��~����
        {
            UnPauseSound();
        }

        if (Input.GetKeyDown(KeyCode.D)) // ��~
        {
            StopSound();
        }

        if (Input.GetKeyDown(KeyCode.F)) // ���ʉ��Đ�
        {
            SoundManager.Instance.PlaySound("���艹");
        }

        if (Input.GetKeyDown(KeyCode.G)) // BGM�Đ�
        {
            SoundManager.Instance.PlaySound("Test2�R�s�[", 0.2f, 2.0f);
        }

        if (Input.GetKeyDown(KeyCode.H)) // BGM�Đ�(3D)
        {
            SoundManager.Instance.PlaySound("TestBGM2", 0.2f, 2.0f, this.gameObject);
        }

        if (Input.GetKeyDown(KeyCode.J)) // BGM�Đ�(�t�B���^�[����, �����C��݂����Ȃ�)
        {
            SoundManager.Instance.PlaySound("TestBGM2", 0.2f, 2.0f, AudioReverbPreset.Bathroom);
        }

        if (Input.GetKeyDown(KeyCode.K)) // BGM�̃t�F�[�h�C���Đ�
        {
            //SoundManager.Instance.SoundFade("TestBGM2", SOUND_FADE_TYPE.OUT_IN, 3.0f, false);
            SoundManager.Instance.PlaySound("TestBGM2", 0.2f, 6.0f, AudioReverbPreset.Bathroom, SOUND_FADE_TYPE.IN, 5.0f, 1.0f, false);
        }

        if (Input.GetKeyDown(KeyCode.L)) // BGM�̃t�F�[�h�A�E�g��~
        {
            SoundManager.Instance.FadeSound("TestBGM2", SOUND_FADE_TYPE.OUT, 5.0f, 0.0f, true);
        }
    }

    public void FixedUpdate()
    {
        //Debug.Log("�|�[�Y���F" + AudioSource_BGM[0].isPause);
        //Debug.Log("�g���Ă���F" + AudioSource_BGM[0].isUse);
        //Debug.Log("�|�[�Y���F" + AudioSource_SE[0].isPause);
        //Debug.Log("�g���Ă���F" + AudioSource_SE[0].isUse);
    }
}
