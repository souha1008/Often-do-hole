using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//#if UNITY_EDITOR
//using UnityEditor;


//// SoundManagerクラスを拡張
//[CustomEditor(typeof(SoundManager))]

//public class SoundManagerEditor : Editor
//{
//    void OnEnable()
//    {

//    }
//    public override void OnInspectorGUI()
//    {
//        SoundManager Sound = target as SoundManager;

//        // エディターの変更確認
//        EditorGUI.BeginChangeCheck();

//        //Sound.AudioClipList = EditorGUILayout


//        // エディターの変更確認
//        if (EditorGUI.EndChangeCheck())
//        {
//            EditorUtility.SetDirty(target); // 選択オブジェクト更新
//        }
//    }
//}
//#endif


public enum SOUND_TYPE
{
    BGM = 0,
    SE
}

// サウンドのクリップ管理用
public struct Audio_Clip
{
    public string AudioName;    // クリップに付ける名前
    public AudioClip AudioClip; // オーディオクリップ
    public SOUND_TYPE SoundType;// 音の種類
}

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    [SerializeField] private int AudioSource_BGM_Max = 3;   // オーディオソースBGMの最大数(インスペクターで表示)
    [SerializeField] private int AudioSource_SE_Max = 10;   // オーディオソースSEの最大数(インスペクターで表示)

    private static int AudioSource_BGM_MAX;   // オーディオソースBGMの最大数(static)
    private static int AudioSource_SE_MAX;    // オーディオソースSEの最大数(static)


    public List<Audio_Clip> AudioClipList = new List<Audio_Clip>(); // オーディオクリップのリスト
    private AudioSource[] AudioSource_BGM;  // オーディオソースBGM
    private AudioSource[] AudioSource_SE;   // オーディオソースSE



    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject); // シーンが変わっても死なない

        // 初期化
        AudioSource_BGM_MAX = AudioSource_BGM_Max;
        AudioSource_SE_MAX = AudioSource_SE_Max;
        AudioSource_BGM = new AudioSource[AudioSource_BGM_MAX];
        AudioSource_SE = new AudioSource[AudioSource_SE_MAX];
    }


    // 音の再生
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

        // 再生
        //AudioSourceList.PlayOneShot(audioClip, 1.0f);
    }

    public void FixedUpdate()
    {
        //for (int i = 0; i < AudioSourceList.Count; i++)
        //{
        //    if (AudioSourceList[i].isPlaying)
        //    {
        //        AudioSourceList.RemoveAt(i);
        //        Debug.Log("再生終了");
        //    }
        //}
    }
}
