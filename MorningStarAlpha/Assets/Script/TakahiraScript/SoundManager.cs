using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        //Sound.AudioClipList = EditorGUILayout


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
    BGM = 0,
    SE
}

// �T�E���h�̃N���b�v�Ǘ��p
public class Audio_Clip
{
    public string AudioName;    // �N���b�v�ɕt���閼�O
    public AudioClip AudioClip; // �I�[�f�B�I�N���b�v
    public SOUND_TYPE SoundType;
}

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    public List<Audio_Clip> AudioClipList = new List<Audio_Clip>();
    private List<AudioSource> AudioSourceList = new List<AudioSource>();



    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject); // �V�[�����ς���Ă����ȂȂ�
    }

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

        // �I�[�f�B�I�\�[�X�̐���
        AudioSourceList.Add(this.gameObject.AddComponent<AudioSource>());

        // �Đ�
        AudioSourceList[AudioSourceList.Count - 1].PlayOneShot(audioClip, 1.0f);
    }

    public void FixedUpdate()
    {
        for (int i = 0; i < AudioSourceList.Count; i++)
        {
            if (AudioSourceList[i].isPlaying)
            {
                AudioSourceList.RemoveAt(i);
                Debug.Log("�Đ��I��");
            }
        }
    }
}
