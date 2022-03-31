using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#if UNITY_EDITOR
using UnityEditor;

/// <summary>
/// �����ӎ�����
/// 
/// 1�F�����ɕt����^�O���� BGM, SE, OBJECT �̂ЂƂ���
/// 2�F�����ɕt���� �^�O��, ���O �͑S�ĕʁX�̖��O�ɂ���
/// 
/// 3�F���ʂ̒i�K [ Play���̉���(�o�͍ő�l) * SetVolume�̉��� * �e�^�C�v�̉���(BGM,SE...) * �}�X�^�[�̉���(�Q�[���S��) ] ���S�� 0.0f�`1.0f
/// 
/// </summary>


// SoundManager�N���X���g��
[CustomEditor(typeof(SoundManager))]

public class SoundManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SoundManager Sound = target as SoundManager;

        // �G�f�B�^�[�̕ύX�m�F
        EditorGUI.BeginChangeCheck();

        Sound.AudioSource_BGM_Max = EditorGUILayout.IntField("BGM�����Đ���MAX", Sound.AudioSource_BGM_Max);
        Sound.AudioSource_SE_Max = EditorGUILayout.IntField("SE�����Đ���MAX", Sound.AudioSource_SE_Max);
        Sound.AudioSource_OBJECT_Max = EditorGUILayout.IntField("OBJECT�����Đ���MAX", Sound.AudioSource_OBJECT_Max);


        Sound.SoundVolumeMaster = EditorGUILayout.Slider("�}�X�^�[�{�����[��", (int)(Sound.SoundVolumeMaster * 100), 0, 100) / 100.0f;
        Sound.SoundVolumeBGM = EditorGUILayout.Slider("BGM�{�����[��", (int)(Sound.SoundVolumeBGM * 100), 0, 100) / 100.0f;
        Sound.SoundVolumeSE = EditorGUILayout.Slider("SE�{�����[��", (int)(Sound.SoundVolumeSE * 100), 0, 100) / 100.0f;
        Sound.SoundVolumeOBJECT = EditorGUILayout.Slider("OBJECT�{�����[��", (int)(Sound.SoundVolumeOBJECT * 100), 0, 100) / 100.0f;


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
    SE,
    OBJECT
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
        Sound_Clip = null;
    }
    public AudioSource AudioSource; // �I�[�f�B�I�\�[�X
    public float StartVolume;       // �Đ��J�n���̃I�[�f�B�I�\�[�X�̃{�����[��
    public float Volume;            // �Đ����̃I�[�f�B�I�\�[�X�̃{�����[��(�J�n���̉��ʂ����ɂ�����)
    public bool isPause;            // �|�[�Y���t���O
    public float PauseTime;         // �|�[�Y���̍Đ��ʒu
    public bool isUse;              // �g�p���t���O(�|�[�Y�����܂߂�)
    public SOUND_CLIP Sound_Clip;   // ���ݎg�p���̃N���b�v���
}


public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    // �C���X�y�N�^�[�ŕ\���p
    public int AudioSource_BGM_Max = 3;     // �I�[�f�B�I�\�[�XBGM�̍ő吔(�C���X�y�N�^�[�ŕ\��)
    public int AudioSource_SE_Max = 10;     // �I�[�f�B�I�\�[�XSE�̍ő吔(�C���X�y�N�^�[�ŕ\��)
    public int AudioSource_OBJECT_Max = 10; // �I�[�f�B�I�\�[�XOBJECT�̍ő吔(�C���X�y�N�^�[�ŕ\��)

    public float SoundVolumeMaster = 1.0f;  // �Q�[���S�̂̉���
    public float SoundVolumeBGM = 1.0f;     // BGM�̉���
    public float SoundVolumeSE = 1.0f;      // SE�̉���   
    public float SoundVolumeOBJECT = 1.0f;  // OBJECT�̉���


    // �����f�[�^�p
    private static int AudioSource_BGM_MAX;   // �I�[�f�B�I�\�[�XBGM�̍ő吔(static)
    private static int AudioSource_SE_MAX;    // �I�[�f�B�I�\�[�XSE�̍ő吔(static)
    private static int AudioSource_OBJECT_MAX;// �I�[�f�B�I�\�[�XOBJECT�̍ő吔(static)


    public List<SOUND_CLIP> SoundList = new List<SOUND_CLIP>(); // �T�E���h�̃N���b�v�Ǘ��p�̃��X�g
    private SOUND_SOURCE[] AudioSource_BGM;  // �I�[�f�B�I�\�[�XBGM
    private SOUND_SOURCE[] AudioSource_SE;   // �I�[�f�B�I�\�[�XSE
    private SOUND_SOURCE[] AudioSource_OBJECT; // �I�[�f�B�I�\�[�XOBJECT

    private delegate void InForDelegate(SOUND_SOURCE[] SoundSource, string SoundName, float Volume, int i); // for�����̕��򏈗��p

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
            var child = new GameObject("AudioSourceBGM_"+ (i + 1)).transform;
            // �q��e�ɐݒ�
            child.SetParent(this.gameObject.transform);
            AudioSource_BGM[i] = new SOUND_SOURCE(child.gameObject.AddComponent<AudioSource>());

            // 3D�����̐ݒ�
            AudioSource_BGM[i].AudioSource.dopplerLevel = 0.0f; // �h�b�v���[���ʖ���
            AudioSource_BGM[i].AudioSource.rolloffMode = AudioRolloffMode.Linear; // ���̌����𒼐��ɂ���
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
        }
    }

    private void Start()
    {
        // �T�E���h�f�[�^�ǂݍ��ݏ���
        StartCoroutine(SetSEData());
        StartCoroutine(SetBGMData());
        StartCoroutine(SetOBJECTData());

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
            }
            AudioClipIlist.Clear();

            Debug.Log("BGM�f�[�^�̓ǂݍ��ݐ���");
        }
        else
        {
            Debug.Log("BGM�f�[�^�̓ǂݍ��ݎ��s");
        }
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

    //========================================================
    // ���̍Đ�
    // 
    // �������FAddressables�A�Z�b�g�ŕt�������O
    // �������F����(0.0f�`1.0f) �������ʂɉ��ʕς����܂�
    // ��O�����F���̍Đ����Ԏw��(�Đ����Ԃ𒴂����ꍇ 0.0f)
    // ��l�����F���̍Đ����W(�w�肵�Ȃ��ꍇ2D,�����ꍇ3D)
    //========================================================
    public void PlaySound(string SoundName)
    {
        PlaySound(SoundName, 1.0f, 0.0f, null);
    }

    public void PlaySound(string SoundName, float Volume)
    {
        PlaySound(SoundName, Volume, 0.0f, null);
    }

    public void PlaySound(string SoundName, float Volume, float PlayTime)
    {
        PlaySound(SoundName, Volume, PlayTime, null);
    }

    public void PlaySound(string SoundName, Vector3? SoundPos)
    {
        PlaySound(SoundName, 1.0f, 0.0f, SoundPos);
    }

    public void PlaySound(string SoundName, float Volume, Vector3? SoundPos)
    {
        PlaySound(SoundName, Volume, 0.0f, SoundPos);
    }

    public void PlaySound(string SoundName, float Volume, float PlayTime, Vector3? SoundPos)
    {
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
            return;

        // ���ʂ͈͎̔w��
        Mathf.Clamp(Volume, 0.0f, 1.0f);

        // �Đ�
        switch (Sound_Clip.SoundType)
        {
            case SOUND_TYPE.NULL:
                break;
            case SOUND_TYPE.BGM:
                for (int i = 0; i < AudioSource_BGM_MAX; i++)
                {
                    if (!AudioSource_BGM[i].isUse)
                    {
                        AudioSource_BGM[i].Sound_Clip = Sound_Clip;

                        StartSoundSource(AudioSource_BGM[i], Volume, PlayTime, SoundPos, true);                       
                        
                        break;
                    }
                }
                break;
            case SOUND_TYPE.SE:
                for (int i = 0; i < AudioSource_SE_MAX; i++)
                {
                    if (!AudioSource_SE[i].isUse)
                    {
                        AudioSource_SE[i].Sound_Clip = Sound_Clip;

                        StartSoundSource(AudioSource_SE[i], Volume, PlayTime, SoundPos, false);
                        
                        break;
                    }
                }
                break;
            case SOUND_TYPE.OBJECT:
                for (int i = 0; i < AudioSource_OBJECT_MAX; i++)
                {
                    if (!AudioSource_OBJECT[i].isUse)
                    {
                        AudioSource_OBJECT[i].Sound_Clip = Sound_Clip;

                        StartSoundSource(AudioSource_OBJECT[i], Volume, PlayTime, SoundPos, true);

                        break;
                    }
                }
                break;
        }
    }


    //===========================
    // �S�Ẳ��̍Đ��I��
    //===========================
    public void StopSoundALL()
    {
        for (int i = 0; i < AudioSource_BGM_MAX; i++)
        {
            if (AudioSource_BGM[i].isUse)
            {
                EndSoundSource(AudioSource_BGM[i]); // �T�E���h�\�[�X���̏I������
            }
        }
        for (int i = 0; i < AudioSource_SE_MAX; i++)
        {
            if (AudioSource_SE[i].isUse)
            {
                EndSoundSource(AudioSource_SE[i]); // �T�E���h�\�[�X���̏I������
            }
        }
        for (int i = 0; i < AudioSource_OBJECT_MAX; i++)
        {
            if (AudioSource_OBJECT[i].isUse)
            {
                EndSoundSource(AudioSource_OBJECT[i]); // �T�E���h�\�[�X���̏I������
            }
        }
    }

    //===========================
    // �ʂ̉��̍Đ��I��
    //===========================
    public void StopSound(string SoundName)
    {
        bool EndFlag = false;

        // �I�[�f�B�I�̖��O�ƍ������̎擾���Ē�~
        for (int i = 0; i < AudioSource_BGM_MAX && !EndFlag; i++)
        {
            if (AudioSource_BGM[i].isUse)
            {
                if (AudioSource_BGM[i].Sound_Clip.SoundName == SoundName)
                {
                    EndSoundSource(AudioSource_BGM[i]); // �T�E���h�\�[�X���̏I������

                    EndFlag = true;
                }
            }
        }
        for (int i = 0; i < AudioSource_SE_MAX && !EndFlag; i++)
        {
            if (AudioSource_SE[i].isUse)
            {
                if (AudioSource_SE[i].Sound_Clip.SoundName == SoundName)
                {
                    EndSoundSource(AudioSource_SE[i]); // �T�E���h�\�[�X���̏I������

                    EndFlag = true;
                }
            }
        }
        for (int i = 0; i < AudioSource_OBJECT_MAX && !EndFlag; i++)
        {
            if (AudioSource_OBJECT[i].isUse)
            {
                if (AudioSource_OBJECT[i].Sound_Clip.SoundName == SoundName)
                {
                    EndSoundSource(AudioSource_OBJECT[i]); // �T�E���h�\�[�X���̏I������

                    EndFlag = true;
                }
            }
        }
    }

    //===========================
    // �S�Ẳ��̈ꎞ��~
    //===========================
    public void PauseSoundALL()
    {
        for (int i = 0; i < AudioSource_BGM_MAX; i++)
        {
            if (AudioSource_BGM[i].isUse && !AudioSource_BGM[i].isPause)
            {
                PauseSoundSource(AudioSource_BGM[i]);
            }
        }
        for (int i = 0; i < AudioSource_SE_MAX; i++)
        {
            if (AudioSource_SE[i].isUse && !AudioSource_SE[i].isPause)
            {
                PauseSoundSource(AudioSource_SE[i]);
            }
        }
        for (int i = 0; i < AudioSource_OBJECT_MAX; i++)
        {
            if (AudioSource_OBJECT[i].isUse && !AudioSource_OBJECT[i].isPause)
            {
                PauseSoundSource(AudioSource_OBJECT[i]);
            }
        }
    }

    //===========================
    // �ʂ̉��̈ꎞ��~
    //===========================
    public void PauseSound(string SoundName)
    {
        bool EndFlag = false;

        // �I�[�f�B�I�̖��O�ƍ������̎擾���Ĉꎞ��~
        for (int i = 0; i < AudioSource_BGM_MAX && !EndFlag; i++)
        {
            if (AudioSource_BGM[i].isUse && !AudioSource_BGM[i].isPause)
            {
                if (AudioSource_BGM[i].Sound_Clip.SoundName == SoundName)
                {
                    PauseSoundSource(AudioSource_BGM[i]);

                    EndFlag = true;
                }
            }
        }
        for (int i = 0; i < AudioSource_SE_MAX && !EndFlag; i++)
        {
            if (AudioSource_SE[i].isUse && !AudioSource_SE[i].isPause)
            {
                if (AudioSource_SE[i].Sound_Clip.SoundName == SoundName)
                {
                    PauseSoundSource(AudioSource_SE[i]);

                    EndFlag = true;
                }
            }
        }
        for (int i = 0; i < AudioSource_OBJECT_MAX && !EndFlag; i++)
        {
            if (AudioSource_OBJECT[i].isUse && !AudioSource_OBJECT[i].isPause)
            {
                if (AudioSource_OBJECT[i].Sound_Clip.SoundName == SoundName)
                {
                    PauseSoundSource(AudioSource_OBJECT[i]);

                    EndFlag = true;
                }
            }
        }
    }

    //===========================
    // �S�Ẳ��̈ꎞ��~����
    //===========================
    public void UnPauseSoundALL()
    {
        for (int i = 0; i < AudioSource_BGM_MAX; i++)
        {
            if (AudioSource_BGM[i].isUse && AudioSource_BGM[i].isPause)
            {
                UnPauseSoundSource(AudioSource_BGM[i]);
            }
        }
        for (int i = 0; i < AudioSource_SE_MAX; i++)
        {
            if (AudioSource_SE[i].isUse && AudioSource_SE[i].isPause)
            {
                UnPauseSoundSource(AudioSource_SE[i]);
            }
        }
        for (int i = 0; i < AudioSource_OBJECT_MAX; i++)
        {
            if (AudioSource_OBJECT[i].isUse && AudioSource_OBJECT[i].isPause)
            {
                UnPauseSoundSource(AudioSource_OBJECT[i]);
            }
        }
    }

    //===========================
    // �ʂ̉��̈ꎞ��~����
    //===========================
    public void UnPauseSound(string SoundName)
    {
        bool EndFlag = false;

        // �I�[�f�B�I�̖��O�ƍ������̎擾���Ĉꎞ��~����
        for (int i = 0; i < AudioSource_BGM_MAX && !EndFlag; i++)
        {
            if (AudioSource_BGM[i].isUse && AudioSource_BGM[i].isPause)
            {
                if (AudioSource_BGM[i].Sound_Clip.SoundName == SoundName)
                {
                    UnPauseSoundSource(AudioSource_BGM[i]);

                    EndFlag = true;
                }
            }
        }
        for (int i = 0; i < AudioSource_SE_MAX && !EndFlag; i++)
        {
            if (AudioSource_SE[i].isUse && AudioSource_SE[i].isPause)
            {
                if (AudioSource_SE[i].Sound_Clip.SoundName == SoundName)
                {
                    UnPauseSoundSource(AudioSource_SE[i]);

                    EndFlag = true;
                }
            }
        }
        for (int i = 0; i < AudioSource_OBJECT_MAX && !EndFlag; i++)
        {
            if (AudioSource_OBJECT[i].isUse && AudioSource_OBJECT[i].isPause)
            {
                if (AudioSource_OBJECT[i].Sound_Clip.SoundName == SoundName)
                {
                    UnPauseSoundSource(AudioSource_OBJECT[i]);

                    EndFlag = true;
                }
            }
        }
    }

    //============================================================
    // �Đ����̑S�Ẳ��ʒ���(���������Ŏg����p)
    // 
    // �������F����(0.0f�`1.0f)���Đ��J�n���̉��ʂƂ͕ʂɐݒ艻
    //============================================================
    public void SetVolumeALL(float Volume)
    {
        Mathf.Clamp(Volume, 0.0f, 1.0f);

        // �I�[�f�B�I�̖��O�ƍ������̎擾���ĉ��ʒ���
        for (int i = 0; i < AudioSource_BGM_MAX; i++)
        {
            if (AudioSource_BGM[i].isUse)
            {
                AudioSource_BGM[i].Volume = Volume; // �Đ����̉��ʃZ�b�g
            }
        }
        for (int i = 0; i < AudioSource_SE_MAX; i++)
        {
            if (AudioSource_SE[i].isUse)
            {
                AudioSource_SE[i].Volume = Volume; // �Đ����̉��ʃZ�b�g
            }
        }
        for (int i = 0; i < AudioSource_OBJECT_MAX; i++)
        {
            if (AudioSource_OBJECT[i].isUse)
            {
                AudioSource_OBJECT[i].Volume = Volume; // �Đ����̉��ʃZ�b�g
            }
        }
        UpdateVolume(); // ���ʍX�V
    }


    //============================================================
    // �Đ����̌ʂ̉��ʒ���(���������Ŏg����p)
    // 
    // �������FAddressables�A�Z�b�g�ŕt�������O
    // �������F����(0.0f�`1.0f)���Đ��J�n���̉��ʂƂ͕ʂɐݒ艻
    //============================================================
    public void SetVolume(string SoundName, float Volume)
    {
        Mathf.Clamp(Volume, 0.0f, 1.0f);

        // �I�[�f�B�I�̖��O�ƍ������̎擾���ĉ��ʒ���
        ForAudioSouce(AudioSource_BGM_MAX, AudioSource_BGM, SoundName, Volume, SetVolumeDelegate);
        ForAudioSouce(AudioSource_SE_MAX, AudioSource_SE, SoundName, Volume, SetVolumeDelegate);
        ForAudioSouce(AudioSource_OBJECT_MAX, AudioSource_OBJECT, SoundName, Volume, SetVolumeDelegate);
    }

    private void SetVolumeDelegate(SOUND_SOURCE[] SoundSource, string SoundName, float Volume, int i)
    {
        if (SoundSource[i].isUse)
        {
            if (SoundSource[i].Sound_Clip.SoundName == SoundName)
            {
                SoundSource[i].Volume = Volume; // �Đ����̉��ʃZ�b�g
                UpdateVolume(); // ���ʍX�V
            }
        }
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

        for (int i = 0; i < AudioSource_BGM_MAX; i++)
        {
            AudioSource_BGM[i].AudioSource.volume = AudioSource_BGM[i].StartVolume * AudioSource_BGM[i].Volume * SoundVolumeBGM;
        }
        for (int i = 0; i < AudioSource_SE_MAX; i++)
        {
            AudioSource_SE[i].AudioSource.volume = AudioSource_SE[i].StartVolume * AudioSource_SE[i].Volume * SoundVolumeSE;
        }
        for (int i = 0; i < AudioSource_OBJECT_MAX; i++)
        {
            AudioSource_OBJECT[i].AudioSource.volume = AudioSource_OBJECT[i].StartVolume * AudioSource_OBJECT[i].Volume * SoundVolumeOBJECT;
        }
    }

    // for���̃f���Q�[�g����
    private void ForAudioSouce(int ForLoopNum, SOUND_SOURCE[] SoundSource, string SoundName, float Volume, InForDelegate  Delegate)
    {
        for (int i = 0; i < ForLoopNum; i++)
        {
            Delegate(SoundSource, SoundName, Volume, i);
        }
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
    private void StartSoundSource(SOUND_SOURCE Sound_Source, float Volume, float PlayTime, Vector3? SoundPos, bool Loop)
    {
        if (SoundPos != null)
        {
            // 3D����
            Sound_Source.AudioSource.gameObject.transform.position = (Vector3)SoundPos; // ���W�ύX
            Sound_Source.AudioSource.spatialBlend = 1.0f; // 3D�̉����ɐ؂�ւ�
        }
        Sound_Source.AudioSource.clip = Sound_Source.Sound_Clip.AudioClip;
        Sound_Source.AudioSource.time = PlayTime;
        Sound_Source.AudioSource.Play();
        Sound_Source.AudioSource.loop = Loop;
        Sound_Source.StartVolume = Volume;
        Sound_Source.AudioSource.volume = Volume * SoundVolumeBGM;
        Sound_Source.isPause = false;
        Sound_Source.isUse = true;
    }

    // �T�E���h�\�[�X���̏I������
    private void EndSoundSource(SOUND_SOURCE Sound_Source)
    {
        Sound_Source.AudioSource.gameObject.transform.position = Vector3.zero;  // ���W�ύX
        Sound_Source.AudioSource.spatialBlend = 0.0f;   // 2D�̉����ɐ؂�ւ�
        Sound_Source.AudioSource.Stop();        // �Đ���~
        Sound_Source.AudioSource.clip = null;   // �N���b�v��null�����
        Sound_Source.AudioSource.time = 0.0f;   // �Đ����ԃ��Z�b�g
        Sound_Source.StartVolume = 1.0f;        // �J�n���̉��ʃ��Z�b�g
        Sound_Source.Volume = 1.0f;             // �g�p���̉��ʃ��Z�b�g
        Sound_Source.Sound_Clip = null;         // ���ݎg�p���̃N���b�v����null�ɂ���
        Sound_Source.isPause = false;           // �|�[�Y���t���O�I�t
        Sound_Source.PauseTime = 0.0f;          // �|�[�Y���̎��ԃ��Z�b�g
        Sound_Source.isUse = false;             // �g�p�t���O�I�t
    }


    // �Đ������菈��
    private void SetNowUse()
    {
        // ���[�v���Ă���z�͔��肵�Ȃ��Ă�����

        //for (int i = 0; i < AudioSource_BGM_MAX; i++)
        //{
        //    if (AudioSource_BGM[i].isUse && !AudioSource_BGM[i].isPause && !AudioSource_BGM[i].AudioSource.isPlaying)
        //    {
        //        EndSoundSource(AudioSource_BGM[i]);
        //    }
            
        //}
        for (int i = 0; i < AudioSource_SE_MAX; i++)
        {
            if (AudioSource_SE[i].isUse && !AudioSource_SE[i].isPause && !AudioSource_SE[i].AudioSource.isPlaying)
            {
                EndSoundSource(AudioSource_SE[i]);
            }  
        }
        //for (int i = 0; i < AudioSource_OBJECT_MAX; i++)
        //{
        //    if (AudioSource_OBJECT[i].isUse && !AudioSource_OBJECT[i].isPause && !AudioSource_OBJECT[i].AudioSource.isPlaying)
        //    {
        //        EndSoundSource(AudioSource_OBJECT[i]);
        //    }
        //}
    }


    public void Update()
    {
        // �e�X�g�p���̓L�[

        if (Input.GetKeyDown(KeyCode.A))
        {
            PauseSoundALL();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            UnPauseSoundALL();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            StopSoundALL();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            SoundManager.Instance.PlaySound("���艹");
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            SoundManager.Instance.PlaySound("TestBGM2", 0.2f, 2.0f, new Vector3(10, 10, 0));
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            SoundManager.Instance.PlaySound("TestBGM2", 0.2f, 2.0f);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            SoundManager.Instance.SetVolume("TestBGM2", 0.5f);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            SoundManager.Instance.SetVolume("TestBGM2", 0.2f);
        }
    }

    public void FixedUpdate()
    {
        // ���ʃZ�b�g
        UpdateVolume();

        // ���ݎg�p�������菈��
        SetNowUse();

        //Debug.Log("�|�[�Y���F" + AudioSource_BGM[0].isPause);
        //Debug.Log("�g���Ă���F" + AudioSource_BGM[0].isUse);
        //Debug.Log("�|�[�Y���F" + AudioSource_SE[0].isPause);
        //Debug.Log("�g���Ă���F" + AudioSource_SE[0].isUse);
    }
}
