using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
//#if UNITY_EDITOR
//using UnityEditor;


//// SoundManager�N���X���g��
//[CustomEditor(typeof(SoundManager))]

//public class SoundManagerEditor : Editor
//{
//    void OnEnable()
//    {

//    }
//    public override void OnInspectorGUI()
//    {
//        SoundManager Sound = target as SoundManager;

//        // �G�f�B�^�[�̕ύX�m�F
//        EditorGUI.BeginChangeCheck();

//        //Sound.AudioClipList = EditorGUILayout


//        // �G�f�B�^�[�̕ύX�m�F
//        if (EditorGUI.EndChangeCheck())
//        {
//            EditorUtility.SetDirty(target); // �I���I�u�W�F�N�g�X�V
//        }
//    }
//}
//#endif


public enum SOUND_TYPE
{
    NULL = 0,
    BGM,
    SE
}

// �T�E���h�̃N���b�v�Ǘ��p
public class SOUND
{
    public SOUND(string audioName, AudioClip audioClip, SOUND_TYPE soundType)
    {
        AudioName = audioName;
        AudioClip = audioClip;
        SoundType = soundType;
    }
    public string AudioName;    // �N���b�v�ɕt���閼�O
    public AudioClip AudioClip; // �I�[�f�B�I�N���b�v
    public SOUND_TYPE SoundType;// ���̎��
}

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    [SerializeField] private int AudioSource_BGM_Max = 3;   // �I�[�f�B�I�\�[�XBGM�̍ő吔(�C���X�y�N�^�[�ŕ\��)
    [SerializeField] private int AudioSource_SE_Max = 10;   // �I�[�f�B�I�\�[�XSE�̍ő吔(�C���X�y�N�^�[�ŕ\��)

    private static int AudioSource_BGM_MAX;   // �I�[�f�B�I�\�[�XBGM�̍ő吔(static)
    private static int AudioSource_SE_MAX;    // �I�[�f�B�I�\�[�XSE�̍ő吔(static)


    public List<SOUND> SoundList = new List<SOUND>(); // �T�E���h�̃N���b�v�Ǘ��p�̃��X�g
    private AudioSource[] AudioSource_BGM;  // �I�[�f�B�I�\�[�XBGM
    private AudioSource[] AudioSource_SE;   // �I�[�f�B�I�\�[�XSE


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
        AudioSource_BGM = new AudioSource[AudioSource_BGM_MAX];
        AudioSource_SE = new AudioSource[AudioSource_SE_MAX];

        for (int i = 0; i < AudioSource_BGM_MAX; i++)
        {
            AudioSource_BGM[i] = this.gameObject.AddComponent<AudioSource>();
        }
        for (int i = 0; i < AudioSource_SE_MAX; i++)
        {
            AudioSource_SE[i] = this.gameObject.AddComponent<AudioSource>();
        }
    }

    private void Start()
    {
        StartCoroutine(SetSEData());
        //StartCoroutine(SetBGMData());
    }

    private IEnumerator SetSEData()
    {
        IList<AudioClip> AudioClipIlist; // �I�[�f�B�I�N���b�v�ǂݍ��ݗp

        // SE�f�[�^�ǂݍ���
        var handleSE = Addressables.LoadAssetsAsync<AudioClip>("SE", null);

        do
        {
            Debug.Log("�ǂݍ��ݒ�");
            yield return null;
        }
        while (handleSE.Status != AsyncOperationStatus.Succeeded);

        if (handleSE.Result != null)
        {
            AudioClipIlist = handleSE.Result;

            for (int i = 0; i < AudioClipIlist.Count; i++)
            {
                SoundList.Add(new SOUND(AudioClipIlist[i].name, AudioClipIlist[i], SOUND_TYPE.SE));
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
            yield return null;
        }
        while (handleBGM.Status != AsyncOperationStatus.Succeeded);

        if (handleBGM.Result != null)
        {
            AudioClipIlist = handleBGM.Result;

            for (int i = 0; i < AudioClipIlist.Count; i++)
            {
                SoundList.Add(new SOUND(AudioClipIlist[i].name, AudioClipIlist[i], SOUND_TYPE.BGM));
            }
            AudioClipIlist.Clear();

            Debug.Log("BGM�f�[�^�̓ǂݍ��ݐ���");
        }
        else
        {
            Debug.Log("BGM�f�[�^�̓ǂݍ��ݎ��s");
        }
    }


    // ���̍Đ�
    public void PlaySound(string AudioName, bool Loop)
    {
        SOUND Sound = new SOUND("", null, SOUND_TYPE.NULL);

        // �I�[�f�B�I�̖��O�ƍ������̎擾
        for (int i = 0; i < SoundList.Count; i++)
        {
            if (SoundList[i].AudioName == AudioName)
            {
                Sound = SoundList[i];
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
                    if (!AudioSource_BGM[i].isPlaying)
                    {
                        AudioSource_BGM[i].clip = Sound.AudioClip;
                        AudioSource_BGM[i].Play();
                    }
                }
                break;
            case SOUND_TYPE.SE:
                for (int i = 0; i < AudioSource_SE_MAX; i++)
                {
                    if (!AudioSource_SE[i].isPlaying)
                    {
                        AudioSource_SE[i].PlayOneShot(Sound.AudioClip);
                    }
                }
                break;
        }
        
    }

    public void FixedUpdate()
    {
        //for (int i = 0; i < AudioSourceList.Count; i++)
        //{
        //    if (AudioSourceList[i].isPlaying)
        //    {
        //        AudioSourceList.RemoveAt(i);
        //        Debug.Log("�Đ��I��");
        //    }
        //}
    }
}
