using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;


// SoundManagerクラスを拡張
[CustomEditor(typeof(SoundManager))]

public class SoundManagerEditor : Editor
{
    void OnEnable()
    {

    }
    public override void OnInspectorGUI()
    {
        SoundManager Sound = target as SoundManager;

        // エディターの変更確認
        EditorGUI.BeginChangeCheck();

        //Sound.AudioClipList = EditorGUILayout


        // エディターの変更確認
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(target); // 選択オブジェクト更新
        }
    }
}
#endif


public enum SOUND_TYPE
{
    BGM = 0,
    SE
}

// サウンドのクリップ管理用
public class Audio_Clip
{
    public string AudioName;    // クリップに付ける名前
    public AudioClip AudioClip; // オーディオクリップ
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

        DontDestroyOnLoad(this.gameObject); // シーンが変わっても死なない
    }

    public void PlaySound(string AudioName, bool Loop)
    {
        AudioClip audioClip = null;


        // オーディオの名前と合うもの取得
        for (int i = 0; i < AudioClipList.Count; i++)
        {
            if (AudioClipList[i].AudioName == AudioName)
            {
                audioClip = AudioClipList[i].AudioClip;
            }
        }

        if (audioClip == null)
            return;

        // オーディオソースの生成
        AudioSourceList.Add(this.gameObject.AddComponent<AudioSource>());

        // 再生
        AudioSourceList[AudioSourceList.Count - 1].PlayOneShot(audioClip, 1.0f);
    }

    public void FixedUpdate()
    {
        for (int i = 0; i < AudioSourceList.Count; i++)
        {
            if (AudioSourceList[i].isPlaying)
            {
                AudioSourceList.RemoveAt(i);
                Debug.Log("再生終了");
            }
        }
    }
}
