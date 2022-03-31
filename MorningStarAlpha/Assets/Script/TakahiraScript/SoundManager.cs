using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#if UNITY_EDITOR
using UnityEditor;

/// <summary>
/// ※注意事項※
/// 
/// 1：音源に付けるタグ名は BGM, SE, OBJECT のひとつだけ
/// 2：音源に付ける タグ名, 名前 は全て別々の名前にする
/// 
/// 3：音量の段階 [ Play時の音量(出力最大値) * SetVolumeの音量 * 各タイプの音量(BGM,SE...) * マスターの音量(ゲーム全体) ] ※全て 0.0f～1.0f
/// 
/// </summary>


// SoundManagerクラスを拡張
[CustomEditor(typeof(SoundManager))]

public class SoundManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SoundManager Sound = target as SoundManager;

        // エディターの変更確認
        EditorGUI.BeginChangeCheck();

        Sound.AudioSource_BGM_Max = EditorGUILayout.IntField("BGM同時再生数MAX", Sound.AudioSource_BGM_Max);
        Sound.AudioSource_SE_Max = EditorGUILayout.IntField("SE同時再生数MAX", Sound.AudioSource_SE_Max);
        Sound.AudioSource_OBJECT_Max = EditorGUILayout.IntField("OBJECT同時再生数MAX", Sound.AudioSource_OBJECT_Max);


        Sound.SoundVolumeMaster = EditorGUILayout.Slider("マスターボリューム", (int)(Sound.SoundVolumeMaster * 100), 0, 100) / 100.0f;
        Sound.SoundVolumeBGM = EditorGUILayout.Slider("BGMボリューム", (int)(Sound.SoundVolumeBGM * 100), 0, 100) / 100.0f;
        Sound.SoundVolumeSE = EditorGUILayout.Slider("SEボリューム", (int)(Sound.SoundVolumeSE * 100), 0, 100) / 100.0f;
        Sound.SoundVolumeOBJECT = EditorGUILayout.Slider("OBJECTボリューム", (int)(Sound.SoundVolumeOBJECT * 100), 0, 100) / 100.0f;


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
    SE,
    OBJECT
}

// サウンドのクリップ管理用
public class SOUND_CLIP
{
    public SOUND_CLIP(string soundName, AudioClip audioClip, SOUND_TYPE soundType)
    {
        SoundName = soundName;
        AudioClip = audioClip;
        SoundType = soundType;
    }
    public string SoundName;    // クリップに付ける名前
    public AudioClip AudioClip; // オーディオクリップ
    public SOUND_TYPE SoundType;// 音の種類
}

// サウンドのソース管理用
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
    public AudioSource AudioSource; // オーディオソース
    public float StartVolume;       // 再生開始時のオーディオソースのボリューム
    public float Volume;            // 再生中のオーディオソースのボリューム(開始時の音量を元にかける)
    public bool isPause;            // ポーズ中フラグ
    public float PauseTime;         // ポーズ中の再生位置
    public bool isUse;              // 使用中フラグ(ポーズ中も含める)
    public SOUND_CLIP Sound_Clip;   // 現在使用中のクリップ情報
}


public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    // インスペクターで表示用
    public int AudioSource_BGM_Max = 3;     // オーディオソースBGMの最大数(インスペクターで表示)
    public int AudioSource_SE_Max = 10;     // オーディオソースSEの最大数(インスペクターで表示)
    public int AudioSource_OBJECT_Max = 10; // オーディオソースOBJECTの最大数(インスペクターで表示)

    public float SoundVolumeMaster = 1.0f;  // ゲーム全体の音量
    public float SoundVolumeBGM = 1.0f;     // BGMの音量
    public float SoundVolumeSE = 1.0f;      // SEの音量   
    public float SoundVolumeOBJECT = 1.0f;  // OBJECTの音量


    // 内部データ用
    private static int AudioSource_BGM_MAX;   // オーディオソースBGMの最大数(static)
    private static int AudioSource_SE_MAX;    // オーディオソースSEの最大数(static)
    private static int AudioSource_OBJECT_MAX;// オーディオソースOBJECTの最大数(static)


    public List<SOUND_CLIP> SoundList = new List<SOUND_CLIP>(); // サウンドのクリップ管理用のリスト
    private SOUND_SOURCE[] AudioSource_BGM;  // オーディオソースBGM
    private SOUND_SOURCE[] AudioSource_SE;   // オーディオソースSE
    private SOUND_SOURCE[] AudioSource_OBJECT; // オーディオソースOBJECT

    private delegate void InForDelegate(SOUND_SOURCE[] SoundSource, string SoundName, float Volume, int i); // for文内の分岐処理用

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
        AudioSource_OBJECT_MAX = AudioSource_OBJECT_Max;
        AudioSource_BGM = new SOUND_SOURCE[AudioSource_BGM_MAX];
        AudioSource_SE = new SOUND_SOURCE[AudioSource_SE_MAX];
        AudioSource_OBJECT = new SOUND_SOURCE[AudioSource_OBJECT_MAX];
        this.gameObject.transform.position = Vector3.zero;

        for (int i = 0; i < AudioSource_BGM_MAX; i++)
        {
            // オーディオソースBGM子オブジェクトの生成
            var child = new GameObject("AudioSourceBGM_"+ (i + 1)).transform;
            // 子を親に設定
            child.SetParent(this.gameObject.transform);
            AudioSource_BGM[i] = new SOUND_SOURCE(child.gameObject.AddComponent<AudioSource>());

            // 3D音響の設定
            AudioSource_BGM[i].AudioSource.dopplerLevel = 0.0f; // ドップラー効果無し
            AudioSource_BGM[i].AudioSource.rolloffMode = AudioRolloffMode.Linear; // 音の減衰を直線にする
        }
        for (int i = 0; i < AudioSource_SE_MAX; i++)
        {
            // オーディオソースBGM子オブジェクトの生成
            var child = new GameObject("AudioSourceSE_" + (i + 1)).transform;
            // 子を親に設定
            child.SetParent(this.gameObject.transform);
            AudioSource_SE[i] = new SOUND_SOURCE(child.gameObject.AddComponent<AudioSource>());

            // 3D音響の設定
            AudioSource_SE[i].AudioSource.dopplerLevel = 0.0f; // ドップラー効果無し
            AudioSource_SE[i].AudioSource.rolloffMode = AudioRolloffMode.Linear; // 音の減衰を直線にする
        }
        for (int i = 0; i < AudioSource_OBJECT_MAX; i++)
        {
            // オーディオソースOBJECT子オブジェクトの生成
            var child = new GameObject("AudioSourceOBJECT_" + (i + 1)).transform;
            // 子を親に設定
            child.SetParent(this.gameObject.transform);
            AudioSource_OBJECT[i] = new SOUND_SOURCE(child.gameObject.AddComponent<AudioSource>());

            // 3D音響の設定
            AudioSource_OBJECT[i].AudioSource.dopplerLevel = 0.0f; // ドップラー効果無し
            AudioSource_OBJECT[i].AudioSource.rolloffMode = AudioRolloffMode.Linear; // 音の減衰を直線にする
        }
    }

    private void Start()
    {
        // サウンドデータ読み込み処理
        StartCoroutine(SetSEData());
        StartCoroutine(SetBGMData());
        StartCoroutine(SetOBJECTData());

        // 音量セット
        UpdateVolume();
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

    private IEnumerator SetOBJECTData()
    {
        IList<AudioClip> AudioClipIlist; // オーディオクリップ読み込み用

        //OBJECTデータ読み込み
        var handleOBJECT = Addressables.LoadAssetsAsync<AudioClip>("OBJECT", null);

        do
        {
            //Debug.Log("読み込み中");
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

            Debug.Log("OBJECTデータの読み込み成功");
        }
        else
        {
            Debug.Log("OBJECTデータの読み込み失敗");
        }
    }

    //========================================================
    // 音の再生
    // 
    // 第一引数：Addressablesアセットで付けた名前
    // 第二引数：音量(0.0f～1.0f) ※音源個別に音量変えられます
    // 第三引数：音の再生時間指定(再生時間を超えた場合 0.0f)
    // 第四引数：音の再生座標(指定しない場合2D,した場合3D)
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

        // オーディオの名前と合うもの取得
        for (int i = 0; i < SoundList.Count; i++)
        {
            if (SoundList[i].SoundName == SoundName)
            {
                Sound_Clip = SoundList[i];
                //Debug.Log("音見つけた");
                break;
            }
        }

        if (Sound_Clip.AudioClip == null)
            return;

        // 音量の範囲指定
        Mathf.Clamp(Volume, 0.0f, 1.0f);

        // 再生
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
    // 全ての音の再生終了
    //===========================
    public void StopSoundALL()
    {
        for (int i = 0; i < AudioSource_BGM_MAX; i++)
        {
            if (AudioSource_BGM[i].isUse)
            {
                EndSoundSource(AudioSource_BGM[i]); // サウンドソース情報の終了処理
            }
        }
        for (int i = 0; i < AudioSource_SE_MAX; i++)
        {
            if (AudioSource_SE[i].isUse)
            {
                EndSoundSource(AudioSource_SE[i]); // サウンドソース情報の終了処理
            }
        }
        for (int i = 0; i < AudioSource_OBJECT_MAX; i++)
        {
            if (AudioSource_OBJECT[i].isUse)
            {
                EndSoundSource(AudioSource_OBJECT[i]); // サウンドソース情報の終了処理
            }
        }
    }

    //===========================
    // 個別の音の再生終了
    //===========================
    public void StopSound(string SoundName)
    {
        bool EndFlag = false;

        // オーディオの名前と合うもの取得して停止
        for (int i = 0; i < AudioSource_BGM_MAX && !EndFlag; i++)
        {
            if (AudioSource_BGM[i].isUse)
            {
                if (AudioSource_BGM[i].Sound_Clip.SoundName == SoundName)
                {
                    EndSoundSource(AudioSource_BGM[i]); // サウンドソース情報の終了処理

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
                    EndSoundSource(AudioSource_SE[i]); // サウンドソース情報の終了処理

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
                    EndSoundSource(AudioSource_OBJECT[i]); // サウンドソース情報の終了処理

                    EndFlag = true;
                }
            }
        }
    }

    //===========================
    // 全ての音の一時停止
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
    // 個別の音の一時停止
    //===========================
    public void PauseSound(string SoundName)
    {
        bool EndFlag = false;

        // オーディオの名前と合うもの取得して一時停止
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
    // 全ての音の一時停止解除
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
    // 個別の音の一時停止解除
    //===========================
    public void UnPauseSound(string SoundName)
    {
        bool EndFlag = false;

        // オーディオの名前と合うもの取得して一時停止解除
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
    // 再生中の全ての音量調節(内部処理で使える用)
    // 
    // 第一引数：音量(0.0f～1.0f)※再生開始時の音量とは別に設定化
    //============================================================
    public void SetVolumeALL(float Volume)
    {
        Mathf.Clamp(Volume, 0.0f, 1.0f);

        // オーディオの名前と合うもの取得して音量調節
        for (int i = 0; i < AudioSource_BGM_MAX; i++)
        {
            if (AudioSource_BGM[i].isUse)
            {
                AudioSource_BGM[i].Volume = Volume; // 再生中の音量セット
            }
        }
        for (int i = 0; i < AudioSource_SE_MAX; i++)
        {
            if (AudioSource_SE[i].isUse)
            {
                AudioSource_SE[i].Volume = Volume; // 再生中の音量セット
            }
        }
        for (int i = 0; i < AudioSource_OBJECT_MAX; i++)
        {
            if (AudioSource_OBJECT[i].isUse)
            {
                AudioSource_OBJECT[i].Volume = Volume; // 再生中の音量セット
            }
        }
        UpdateVolume(); // 音量更新
    }


    //============================================================
    // 再生中の個別の音量調節(内部処理で使える用)
    // 
    // 第一引数：Addressablesアセットで付けた名前
    // 第二引数：音量(0.0f～1.0f)※再生開始時の音量とは別に設定化
    //============================================================
    public void SetVolume(string SoundName, float Volume)
    {
        Mathf.Clamp(Volume, 0.0f, 1.0f);

        // オーディオの名前と合うもの取得して音量調節
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
                SoundSource[i].Volume = Volume; // 再生中の音量セット
                UpdateVolume(); // 音量更新
            }
        }
    }



    //===========================================
    // 現在の音量を更新(音量を変更したときに使用)
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

    // for文のデリゲート処理
    private void ForAudioSouce(int ForLoopNum, SOUND_SOURCE[] SoundSource, string SoundName, float Volume, InForDelegate  Delegate)
    {
        for (int i = 0; i < ForLoopNum; i++)
        {
            Delegate(SoundSource, SoundName, Volume, i);
        }
    }


    // サウンドソース一時停止処理
    private void PauseSoundSource(SOUND_SOURCE Sound_Source)
    {
        Sound_Source.PauseTime = Sound_Source.AudioSource.time; // ポーズ中の時間ゲット
        Sound_Source.AudioSource.Stop();// 再生一時停止
        Sound_Source.isPause = true;    // ポーズ中フラグオン
    }

    // サウンドソース一時停止解除処理
    private void UnPauseSoundSource(SOUND_SOURCE Sound_Source)
    {
        Sound_Source.AudioSource.time = Sound_Source.PauseTime; // ポーズの時間セット
        Sound_Source.AudioSource.Play();// 再生一時停止解除
        Sound_Source.isPause = false;   // ポーズ中フラグオフ
    }

    // サウンドソース情報の開始処理
    private void StartSoundSource(SOUND_SOURCE Sound_Source, float Volume, float PlayTime, Vector3? SoundPos, bool Loop)
    {
        if (SoundPos != null)
        {
            // 3D音響
            Sound_Source.AudioSource.gameObject.transform.position = (Vector3)SoundPos; // 座標変更
            Sound_Source.AudioSource.spatialBlend = 1.0f; // 3Dの音響に切り替え
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

    // サウンドソース情報の終了処理
    private void EndSoundSource(SOUND_SOURCE Sound_Source)
    {
        Sound_Source.AudioSource.gameObject.transform.position = Vector3.zero;  // 座標変更
        Sound_Source.AudioSource.spatialBlend = 0.0f;   // 2Dの音響に切り替え
        Sound_Source.AudioSource.Stop();        // 再生停止
        Sound_Source.AudioSource.clip = null;   // クリップにnull入れる
        Sound_Source.AudioSource.time = 0.0f;   // 再生時間リセット
        Sound_Source.StartVolume = 1.0f;        // 開始時の音量リセット
        Sound_Source.Volume = 1.0f;             // 使用中の音量リセット
        Sound_Source.Sound_Clip = null;         // 現在使用中のクリップ情報をnullにする
        Sound_Source.isPause = false;           // ポーズ中フラグオフ
        Sound_Source.PauseTime = 0.0f;          // ポーズ中の時間リセット
        Sound_Source.isUse = false;             // 使用フラグオフ
    }


    // 再生中判定処理
    private void SetNowUse()
    {
        // ループしている奴は判定しなくてもいい

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
            SoundManager.Instance.PlaySound("決定音");
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
        // 音量セット
        UpdateVolume();

        // 現在使用中か判定処理
        SetNowUse();

        //Debug.Log("ポーズ中：" + AudioSource_BGM[0].isPause);
        //Debug.Log("使っている：" + AudioSource_BGM[0].isUse);
        //Debug.Log("ポーズ中：" + AudioSource_SE[0].isPause);
        //Debug.Log("使っている：" + AudioSource_SE[0].isUse);
    }
}
