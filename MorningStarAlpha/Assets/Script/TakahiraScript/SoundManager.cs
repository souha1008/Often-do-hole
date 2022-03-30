using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
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

        Sound.AudioSource_BGM_Max = EditorGUILayout.IntField("BGMオーディオソース最大値", Sound.AudioSource_BGM_Max);
        Sound.AudioSource_SE_Max = EditorGUILayout.IntField("SEオーディオソース最大値", Sound.AudioSource_SE_Max);


        Sound.SoundVolumeMaster = EditorGUILayout.Slider("マスターボリューム", (int)(Sound.SoundVolumeMaster * 100), 0, 100) / 100.0f;
        Sound.SoundVolumeBGM = EditorGUILayout.Slider("BGMボリューム", (int)(Sound.SoundVolumeBGM * 100), 0, 100) / 100.0f;
        Sound.SoundVolumeSE = EditorGUILayout.Slider("SEボリューム", (int)(Sound.SoundVolumeSE * 100), 0, 100) / 100.0f;


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
    NULL = 0,
    BGM,
    SE
}

// サウンドのクリップ管理用
public class SOUND_CLIP
{
    public SOUND_CLIP(string audioName, AudioClip audioClip, SOUND_TYPE soundType)
    {
        AudioName = audioName;
        AudioClip = audioClip;
        SoundType = soundType;
    }
    public string AudioName;    // クリップに付ける名前
    public AudioClip AudioClip; // オーディオクリップ
    public SOUND_TYPE SoundType;// 音の種類
}

// サウンドのソース管理用
public class SOUND_SOURCE
{
    public SOUND_SOURCE(AudioSource audioSource)
    {
        AudioSource = audioSource;
        Volume = 1.0f;
        Mathf.Clamp(Volume, 0.0f, 1.0f);
        isPause = false;
        PauseTime = 0.0f;
        isUse = false;
    }
    public AudioSource AudioSource; // オーディオソース
    public float Volume;            // オーディオソースのボリューム
    public bool isPause;            // ポーズ中フラグ
    public float PauseTime;         // ポーズ中の再生位置
    public bool isUse;              // 使用中フラグ(ポーズ中も含める)
}


public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    // インスペクターで表示用
    public int AudioSource_BGM_Max = 3;   // オーディオソースBGMの最大数(インスペクターで表示)
    public int AudioSource_SE_Max = 10;   // オーディオソースSEの最大数(インスペクターで表示)

    public float SoundVolumeMaster = 1.0f; // ゲーム全体の音量
    public float SoundVolumeSE = 1.0f;  // SEの音量
    public float SoundVolumeBGM = 1.0f; // BGMの音量


    // 内部データ用
    private static int AudioSource_BGM_MAX;   // オーディオソースBGMの最大数(static)
    private static int AudioSource_SE_MAX;    // オーディオソースSEの最大数(static)


    public List<SOUND_CLIP> SoundList = new List<SOUND_CLIP>(); // サウンドのクリップ管理用のリスト
    private SOUND_SOURCE[] AudioSource_BGM;  // オーディオソースBGM
    private SOUND_SOURCE[] AudioSource_SE;   // オーディオソースSE


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
        AudioSource_BGM = new SOUND_SOURCE[AudioSource_BGM_MAX];
        AudioSource_SE = new SOUND_SOURCE[AudioSource_SE_MAX];

        for (int i = 0; i < AudioSource_BGM_MAX; i++)
        {
            AudioSource_BGM[i] = new SOUND_SOURCE(this.gameObject.AddComponent<AudioSource>());
        }
        for (int i = 0; i < AudioSource_SE_MAX; i++)
        {
            AudioSource_SE[i] = new SOUND_SOURCE(this.gameObject.AddComponent<AudioSource>());
        }
    }

    private void Start()
    {
        // サウンドデータ読み込み処理
        StartCoroutine(SetSEData());
        StartCoroutine(SetBGMData());

        // 音量セット
        SetVolume();
    }

    private IEnumerator SetSEData()
    {
        IList<AudioClip> AudioClipIlist; // オーディオクリップ読み込み用

        // SEデータ読み込み
        var handleSE = Addressables.LoadAssetsAsync<AudioClip>("SE", null);

        do
        {
            //Debug.Log("読み込み中");
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

            Debug.Log("SEデータの読み込み成功");
        }
        else
        {
            Debug.Log("SEデータの読み込み失敗");
        }
    }

    private IEnumerator SetBGMData()
    {
        IList<AudioClip> AudioClipIlist; // オーディオクリップ読み込み用


        //BGMデータ読み込み
        var handleBGM = Addressables.LoadAssetsAsync<AudioClip>("BGM", null);

        do
        {
            //Debug.Log("読み込み中");
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

            Debug.Log("BGMデータの読み込み成功");
        }
        else
        {
            Debug.Log("BGMデータの読み込み失敗");
        }
    }

    //===========================
    // 音の再生
    //===========================
    public void PlaySound(string AudioName)
    {
        PlaySound(AudioName, 1.0f);
    }

    public void PlaySound(string AudioName, float Volume)
    {
        SOUND_CLIP Sound = new SOUND_CLIP("", null, SOUND_TYPE.NULL);

        // オーディオの名前と合うもの取得
        for (int i = 0; i < SoundList.Count; i++)
        {
            if (SoundList[i].AudioName == AudioName)
            {
                Sound = SoundList[i];
                Debug.Log("音見つけた");
            }
        }

        if (Sound.AudioClip == null)
            return;

        // 再生
        switch (Sound.SoundType)
        {
            case SOUND_TYPE.NULL:
                break;
            case SOUND_TYPE.BGM:
                for (int i = 0; i < AudioSource_BGM_MAX; i++)
                {
                    if (!AudioSource_BGM[i].isUse)
                    {
                        AudioSource_BGM[i].AudioSource.clip = Sound.AudioClip;
                        AudioSource_BGM[i].AudioSource.Play();
                        AudioSource_BGM[i].AudioSource.loop = true;
                        AudioSource_BGM[i].Volume = Volume;
                        AudioSource_BGM[i].AudioSource.volume = Volume * SoundVolumeBGM;
                        AudioSource_BGM[i].isPause = false;
                        AudioSource_BGM[i].isUse = true;
                        break;
                    }
                }
                break;
            case SOUND_TYPE.SE:
                for (int i = 0; i < AudioSource_SE_MAX; i++)
                {
                    if (!AudioSource_SE[i].isUse)
                    {
                        AudioSource_SE[i].AudioSource.clip = Sound.AudioClip;
                        AudioSource_SE[i].AudioSource.Play();
                        AudioSource_SE[i].AudioSource.loop = false;
                        AudioSource_SE[i].Volume = Volume;
                        AudioSource_SE[i].AudioSource.volume = Volume * SoundVolumeSE;
                        AudioSource_SE[i].isPause = false;
                        AudioSource_SE[i].isUse = true;
                        break;
                    }
                }
                break;
        }
    }


    //===========================
    // 全ての音の再生終了
    //===========================
    public void StopSoundALL()
    {
        for (int i = 0; i < AudioSource_BGM_MAX; i++)
        {
            AudioSource_BGM[i].AudioSource.Stop();
            AudioSource_BGM[i].isPause = false;
            AudioSource_BGM[i].isUse = false;
        }
        for (int i = 0; i < AudioSource_SE_MAX; i++)
        {
            AudioSource_SE[i].AudioSource.Stop();
            AudioSource_SE[i].isPause = false;
            AudioSource_SE[i].isUse = false;
        }
    }

    //===========================
    // 全ての音の一時停止
    //===========================
    public void PauseSoundALL()
    {
        for (int i = 0; i < AudioSource_BGM_MAX; i++)
        {
            if (AudioSource_BGM[i].AudioSource.isPlaying)
            {
                AudioSource_BGM[i].PauseTime = AudioSource_BGM[i].AudioSource.time;
                AudioSource_BGM[i].AudioSource.Stop();
                AudioSource_BGM[i].isPause = true;
            }
        }
        for (int i = 0; i < AudioSource_SE_MAX; i++)
        {
            if (AudioSource_SE[i].AudioSource.isPlaying)
            {
                AudioSource_SE[i].PauseTime = AudioSource_SE[i].AudioSource.time;
                AudioSource_SE[i].AudioSource.Stop();
                AudioSource_SE[i].isPause = true;
            }
        }
    }

    //===========================
    // 全ての音の一時停止解除
    //===========================
    public void UnPauseSoundALL()
    {
        for (int i = 0; i < AudioSource_BGM_MAX; i++)
        {
            if (AudioSource_BGM[i].isPause)
            {
                AudioSource_BGM[i].AudioSource.time = AudioSource_BGM[i].PauseTime;
                AudioSource_BGM[i].AudioSource.Play();
                AudioSource_BGM[i].isPause = false;
            }
        }
        for (int i = 0; i < AudioSource_SE_MAX; i++)
        {
            if (AudioSource_SE[i].isPause)
            {
                AudioSource_SE[i].AudioSource.time = AudioSource_SE[i].PauseTime;
                AudioSource_SE[i].AudioSource.Play();
                AudioSource_SE[i].isPause = false;
            }
        }
    }


    //===========================================
    // 現在の音量を更新(音量を変更したときに使用)
    //===========================================
    public void SetVolume()
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


    // 再生中判定処理
    private void SetNowUse()
    {
        for (int i = 0; i < AudioSource_BGM_MAX; i++)
        {
            if (AudioSource_BGM[i].isUse && !AudioSource_BGM[i].isPause && !AudioSource_BGM[i].AudioSource.isPlaying)
            {
                AudioSource_BGM[i].isUse = false; // 再生終了
            }
            
        }
        for (int i = 0; i < AudioSource_SE_MAX; i++)
        {
            if (AudioSource_SE[i].isUse && !AudioSource_SE[i].isPause && !AudioSource_SE[i].AudioSource.isPlaying)
            {
                AudioSource_SE[i].isUse = false; // 再生終了
            }  
        }
    }


    public void Update()
    {
        // テスト用入力キー

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
            // 音の途中再生
            //AudioSource_SE[0].AudioSource.time = 0.15f;
            //AudioSource_SE[0].AudioSource.Play();
            //AudioSource_SE[0].isUse = true;
        }
    }

    public void FixedUpdate()
    {
        // 音量セット
        SetVolume();

        // 現在使用中か判定処理
        SetNowUse();

        //Debug.Log("ポーズ中：" + AudioSource_BGM[0].isPause);
        //Debug.Log("使っている：" + AudioSource_BGM[0].isUse);
        Debug.Log("ポーズ中：" + AudioSource_SE[0].isPause);
        Debug.Log("使っている：" + AudioSource_SE[0].isUse);
    }
}
