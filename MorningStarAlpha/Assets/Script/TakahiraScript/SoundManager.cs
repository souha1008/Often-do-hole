using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    BGM = 0,
    SE
}

// �T�E���h�̃N���b�v�Ǘ��p
public struct Audio_Clip
{
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


    public List<Audio_Clip> AudioClipList = new List<Audio_Clip>(); // �I�[�f�B�I�N���b�v�̃��X�g
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
    }


    // ���̍Đ�
    public void PlaySound(string AudioName, bool Loop)
    {
        AudioClip audioClip = null;


        // �I�[�f�B�I�̖��O�ƍ������̎擾
        for (int i = 0; i < AudioClipList.Count; i++)
        {
            if (AudioClipList[i].AudioName == AudioName)
            {
                audioClip = AudioClipList[i].AudioClip;
            }
        }

        if (audioClip == null)
            return;

        // �Đ�
        //AudioSourceList.PlayOneShot(audioClip, 1.0f);
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
