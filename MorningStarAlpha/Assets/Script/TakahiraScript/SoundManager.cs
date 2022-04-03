using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#if UNITY_EDITOR
using UnityEditor;

/// <summary>
/// 
///  ※ 注意事項 ※
/// 
/// 1：音源に付けるタグ名は BGM, SE, OBJECT のひとつだけ
/// 2：音源に付ける タグ名, 名前 は全て別々の名前にする
/// 
/// 3：音量の段階 [ Play時の音量(出力最大値) * SetVolumeの音量 * 各タイプの音量(BGM,SE...) * マスターの音量(ゲーム全体) ] ※全て 0.0f〜1.0f
/// 
/// 
/// ++++使用方法++++
/// 
/// SoundManager.Instance.○○();
/// 
/// (例) SoundManager.Instance.PlaySound("効果音1");
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


        EditorGUILayout.LabelField("[音の同時再生数MAX]");
        EditorGUILayout.Space(3);
        Sound.AudioSource_BGM_Max = EditorGUILayout.IntField("BGM同時再生数", Sound.AudioSource_BGM_Max);
        Sound.AudioSource_SE_Max = EditorGUILayout.IntField("SE同時再生数", Sound.AudioSource_SE_Max);
        Sound.AudioSource_OBJECT_Max = EditorGUILayout.IntField("OBJECT同時再生数", Sound.AudioSource_OBJECT_Max);
        EditorGUILayout.Space(15);


        EditorGUILayout.LabelField("[音のボリューム]");
        EditorGUILayout.Space(3);
        Sound.SoundVolumeMaster = EditorGUILayout.Slider("マスターボリューム", (int)(Sound.SoundVolumeMaster * 100), 0, 100) / 100.0f;
        Sound.SoundVolumeBGM = EditorGUILayout.Slider("BGMボリューム", (int)(Sound.SoundVolumeBGM * 100), 0, 100) / 100.0f;
        Sound.SoundVolumeSE = EditorGUILayout.Slider("SEボリューム", (int)(Sound.SoundVolumeSE * 100), 0, 100) / 100.0f;
        Sound.SoundVolumeOBJECT = EditorGUILayout.Slider("OBJECTボリューム", (int)(Sound.SoundVolumeOBJECT * 100), 0, 100) / 100.0f;
        EditorGUILayout.Space(15);


        EditorGUILayout.LabelField("[3Dサウンド]");
        EditorGUILayout.Space(3);
        Sound.SoundMaxDistance3D = EditorGUILayout.FloatField("3Dサウンドの聞こえる最大距離", Sound.SoundMaxDistance3D);


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

public enum SOUND_FADE_TYPE
{
    IN = 0,
    OUT,
    OUT_IN
}

// サウンドのフェード用クラス
public class SOUND_FADE
{
    public SOUND_FADE(string soundName, SOUND_FADE_TYPE fadeType, float fadeTime, bool soundStop)
    {
        SoundName = soundName;
        FadeType = fadeType;
        FadeTime = fadeTime;
        NowTime = 0.0f;
        SoundStop = soundStop;
    }
    public string SoundName;
    public SOUND_FADE_TYPE FadeType; // フェードの種類
    public float FadeTime;  // フェードする時間
    public float NowTime;   // 経過時間
    public bool SoundStop;  // フェード後に音止めるか
}

// 現在再生中のサウンド管理用
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
        SoundObject = null;
    }
    public AudioSource AudioSource; // オーディオソース
    public float StartVolume;       // 再生開始時のオーディオソースのボリューム
    public float Volume;            // 再生中のオーディオソースのボリューム(開始時の音量を元にかける)
    public bool isPause;            // ポーズ中フラグ
    public float PauseTime;         // ポーズ中の再生位置
    public bool isUse;              // 使用中フラグ
    public GameObject SoundObject;  // 音を鳴らすオブジェクト(3Dサウンドの場合使用)
}


public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    // インスペクターで表示用
    public int AudioSource_BGM_Max = 3;     // オーディオソースBGMの最大数(インスペクターで表示)
    public int AudioSource_SE_Max = 10;     // オーディオソースSEの最大数(インスペクターで表示)
    public int AudioSource_OBJECT_Max = 100; // オーディオソースOBJECTの最大数(インスペクターで表示)

    public float SoundVolumeMaster = 1.0f;  // ゲーム全体の音量
    public float SoundVolumeBGM = 1.0f;     // BGMの音量
    public float SoundVolumeSE = 1.0f;      // SEの音量   
    public float SoundVolumeOBJECT = 1.0f;  // OBJECTの音量

    public float SoundMaxDistance3D = 500.0f; // 3Dサウンドの聞こえる最大距離


    // 内部データ用
    private static int AudioSource_BGM_MAX;   // オーディオソースBGMの最大数(static)
    private static int AudioSource_SE_MAX;    // オーディオソースSEの最大数(static)
    private static int AudioSource_OBJECT_MAX;// オーディオソースOBJECTの最大数(static)


    public static List<SOUND_CLIP> SoundList = new List<SOUND_CLIP>(); // サウンドのクリップ管理用のリスト
    private SOUND_SOURCE[] AudioSource_BGM;  // オーディオソースBGM
    private SOUND_SOURCE[] AudioSource_SE;   // オーディオソースSE
    private SOUND_SOURCE[] AudioSource_OBJECT; // オーディオソースOBJECT


    private List<NOW_PLAY_SOUND> NowPlaySoundList = new List<NOW_PLAY_SOUND>(); // 再生中のサウンド管理用リスト
    //private List<SOUND_FADE> FadeList = new List<SOUND_FADE>(); // サウンドのフェード用リスト


    
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
            var child = new GameObject("AudioSourceBGM_" + (i + 1)).transform;
            // 子を親に設定
            child.SetParent(this.gameObject.transform);
            AudioSource_BGM[i] = new SOUND_SOURCE(child.gameObject.AddComponent<AudioSource>());

            // 3D音響の設定
            AudioSource_BGM[i].AudioSource.dopplerLevel = 0.0f; // ドップラー効果無し
            AudioSource_BGM[i].AudioSource.rolloffMode = AudioRolloffMode.Linear; // 音の減衰を直線にする
            AudioSource_BGM[i].AudioSource.maxDistance = SoundMaxDistance3D;    // 音の聞こえる最大距離
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
            AudioSource_SE[i].AudioSource.maxDistance = SoundMaxDistance3D;    // 音の聞こえる最大距離
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
            AudioSource_OBJECT[i].AudioSource.maxDistance = SoundMaxDistance3D;    // 音の聞こえる最大距離
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
    // 第二引数：音量(0.0f〜1.0f) ※音源個別に音量変えられます
    // 第三引数：音の再生時間指定(再生時間を超えた場合 0.0f)
    // 第四引数：音の再生オブジェクト(指定しない場合2D,した場合3D)
    // 第五引数：音の反響設定(AudioReverbPreset.○○○で指定出来る)
    // 
    // いくつかオーバーロード
    //========================================================
    public void PlaySound(string SoundName)
    {
        PlaySound(SoundName, 1.0f, 0.0f, null, AudioReverbPreset.Off);
    }
    public void PlaySound(string SoundName, float Volume)
    {
        PlaySound(SoundName, Volume, 0.0f, null, AudioReverbPreset.Off);
    }
    public void PlaySound(string SoundName, float Volume, float PlayTime)
    {
        PlaySound(SoundName, Volume, PlayTime, null, AudioReverbPreset.Off);
    }
    public void PlaySound(string SoundName, GameObject SoundObject)
    {
        PlaySound(SoundName, 1.0f, 0.0f, SoundObject, AudioReverbPreset.Off);
    }
    public void PlaySound(string SoundName, float Volume, GameObject SoundObject)
    {
        PlaySound(SoundName, Volume, 0.0f, SoundObject, AudioReverbPreset.Off);
    }
    public void PlaySound(string SoundName, float Volume, float PlayTime, GameObject SoundObject)
    {
        PlaySound(SoundName, Volume, PlayTime, SoundObject, AudioReverbPreset.Off);
    }
    public void PlaySound(string SoundName, AudioReverbPreset ReverbPreset)
    {
        PlaySound(SoundName, 1.0f, 0.0f, null, ReverbPreset);
    }
    public void PlaySound(string SoundName, float Volume, AudioReverbPreset ReverbPreset)
    {
        PlaySound(SoundName, Volume, 0.0f, null, ReverbPreset);
    }
    public void PlaySound(string SoundName, float Volume, float PlayTime, AudioReverbPreset ReverbPreset)
    {
        PlaySound(SoundName, Volume, PlayTime, null, ReverbPreset);
    }

    public void PlaySound(string SoundName, float Volume, float PlayTime, GameObject SoundObject, AudioReverbPreset ReverbPreset)
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
                        NowPlaySoundList.Add(new NOW_PLAY_SOUND(Sound_Clip, AudioSource_BGM[i]));                     

                        StartSoundSource(NowPlaySoundList[NowPlaySoundList.Count - 1], Volume, PlayTime, SoundObject, ReverbPreset, true);

                        return;
                    }
                }
                Debug.LogWarning("BGMが最大再生数を超えた為、再生出来ませんでした");
                break;
            case SOUND_TYPE.SE:
                for (int i = 0; i < AudioSource_SE_MAX; i++)
                {
                    if (!AudioSource_SE[i].isUse)
                    {
                        NowPlaySoundList.Add(new NOW_PLAY_SOUND(Sound_Clip, AudioSource_SE[i]));

                        StartSoundSource(NowPlaySoundList[NowPlaySoundList.Count - 1], Volume, PlayTime, SoundObject, ReverbPreset, false);

                        return;
                    }
                }
                Debug.LogWarning("SEが最大再生数を超えた為、再生出来ませんでした");
                break;
            case SOUND_TYPE.OBJECT:
                for (int i = 0; i < AudioSource_OBJECT_MAX; i++)
                {
                    if (!AudioSource_OBJECT[i].isUse)
                    {
                        NowPlaySoundList.Add(new NOW_PLAY_SOUND(Sound_Clip, AudioSource_OBJECT[i]));

                        StartSoundSource(NowPlaySoundList[NowPlaySoundList.Count - 1], Volume, PlayTime, SoundObject, ReverbPreset, true);

                        return;
                    }
                }
                Debug.LogWarning("OBJECT音が最大再生数を超えた為、再生出来ませんでした");
                break;
        }
    }


    //===========================
    // 全ての音の再生終了
    //===========================
    public void StopSoundALL()
    {
        // 再生終了処理
        for (int i = 0; i < NowPlaySoundList.Count; i++)
        {
            EndSoundSource(NowPlaySoundList[i]);
        }
    }


    //===========================
    // 個別の音の再生終了
    //===========================
    public void StopSound(string SoundName)
    {
        // オーディオの名前と合うもの取得して再生終了
        for (int i = 0; i < NowPlaySoundList.Count; i++)
        {
            if (NowPlaySoundList[i].Sound_Clip.SoundName == SoundName)
            {
                EndSoundSource(NowPlaySoundList[i]);
            }
        }
    }


    //===========================
    // 全ての音の一時停止
    //===========================
    public void PauseSoundALL()
    {
        // 一時停止処理
        for (int i = 0; i < NowPlaySoundList.Count; i++)
        {
            if (!NowPlaySoundList[i].Sound_Source.isPause)
            {
                PauseSoundSource(NowPlaySoundList[i].Sound_Source);
            }            
        }
    }


    //===========================
    // 個別の音の一時停止
    //===========================
    public void PauseSound(string SoundName)
    {
        // オーディオの名前と合うもの取得して一時停止
        for (int i = 0; i < NowPlaySoundList.Count; i++)
        {
            if (!NowPlaySoundList[i].Sound_Source.isPause && NowPlaySoundList[i].Sound_Clip.SoundName == SoundName)
            {
                PauseSoundSource(NowPlaySoundList[i].Sound_Source);
            }
        }
    }


    //===========================
    // 全ての音の一時停止解除
    //===========================
    public void UnPauseSoundALL()
    {
        // 一時停止解除処理
        for (int i = 0; i < NowPlaySoundList.Count; i++)
        {
            if (NowPlaySoundList[i].Sound_Source.isPause)
            {
                UnPauseSoundSource(NowPlaySoundList[i].Sound_Source);
            }
        }
    }


    //===========================
    // 個別の音の一時停止解除
    //===========================
    public void UnPauseSound(string SoundName)
    {
        // オーディオの名前と合うもの取得して一時停止解除
        for (int i = 0; i < NowPlaySoundList.Count; i++)
        {
            if (NowPlaySoundList[i].Sound_Source.isPause && NowPlaySoundList[i].Sound_Clip.SoundName == SoundName)
            {
                UnPauseSoundSource(NowPlaySoundList[i].Sound_Source);
            }
        }
    }


    //============================================================
    // 再生中の全ての音量調節(内部処理で使える用)
    // 
    // 第一引数：音量(0.0f〜1.0f)※再生開始時の音量とは別に設定化
    //============================================================
    public void SetVolumeALL(float Volume)
    {
        Mathf.Clamp(Volume, 0.0f, 1.0f);

        // 音量調節
        for (int i = 0; i < NowPlaySoundList.Count; i++)
        {
            NowPlaySoundList[i].Sound_Source.Volume = Volume; // 再生中の音量セット
        }
        UpdateVolume(); // 音量更新
    }


    //============================================================
    // 再生中の個別の音量調節(内部処理で使える用)
    // 
    // 第一引数：Addressablesアセットで付けた名前
    // 第二引数：音量(0.0f〜1.0f)※再生開始時の音量とは別に設定化
    //============================================================
    public void SetVolume(string SoundName, float Volume)
    {
        Mathf.Clamp(Volume, 0.0f, 1.0f);

        // オーディオの名前と合うもの取得して音量調節
        for (int i = 0; i < NowPlaySoundList.Count; i++)
        {
            if (NowPlaySoundList[i].Sound_Clip.SoundName == SoundName)
            {
                NowPlaySoundList[i].Sound_Source.Volume = Volume; // 再生中の音量セット
            }
        }
        UpdateVolume(); // 音量更新
    }


    //===========================================
    // 音のフェード機能(再生中の音)
    //
    // 第一引数：フェードする音の名前
    // 第二引数：フェードの種類
    // 第三引数：フェードする時間
    // 第四引数：フェード後音を止めるか
    //===========================================
    public void SoundFade(string SoundName, SOUND_FADE_TYPE FadeType, float FadeTime, bool SoundStop)
    {
        if (FadeTime <= 0)
            return;

        // 再生中の音で名前が一致してフェード中でないならフェードセット
        for (int i = 0; i < NowPlaySoundList.Count; i++)
        {
            if (NowPlaySoundList[i].Sound_Clip.SoundName == SoundName && NowPlaySoundList[i].Sound_Fade == null)
            {
                NowPlaySoundList[i].Sound_Fade = new SOUND_FADE(SoundName, FadeType, FadeTime, SoundStop);
            }
        }
    }

    private void UpdateSoundFade()
    {
        for (int i = 0; i < NowPlaySoundList.Count; i++)
        {
            if (NowPlaySoundList[i].Sound_Fade != null && !NowPlaySoundList[i].Sound_Source.isPause)
            {
                switch (NowPlaySoundList[i].Sound_Fade.FadeType)
                {
                    case SOUND_FADE_TYPE.IN:
                        SoundFadeIN(NowPlaySoundList[i]);
                        break;
                    case SOUND_FADE_TYPE.OUT:
                        break;
                    case SOUND_FADE_TYPE.OUT_IN:
                        break;
                }
            }
        }
    }

    private void SoundFadeIN(NOW_PLAY_SOUND NowPlaySound)
    {
        float Volume = NowPlaySound.Sound_Fade.NowTime;
    }

    private void SoundFadeOUT(NOW_PLAY_SOUND NowPlaySound)
    {

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

            // 音量設定
            NowPlaySoundList[i].Sound_Source.AudioSource.volume =
                NowPlaySoundList[i].Sound_Source.StartVolume * NowPlaySoundList[i].Sound_Source.Volume * SoundVol;

            // 音の聞こえる最大距離更新
            NowPlaySoundList[i].Sound_Source.AudioSource.maxDistance = SoundMaxDistance3D;
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
    private void StartSoundSource(NOW_PLAY_SOUND NowPlaySound, float Volume, float PlayTime, GameObject SoundObject, AudioReverbPreset ReverbPreset, bool Loop)
    {
        if (SoundObject != null)
        {
            // 3D音響
            NowPlaySound.Sound_Source.AudioSource.gameObject.transform.position = SoundObject.transform.position; // 座標変更
            NowPlaySound.Sound_Source.SoundObject = SoundObject; // オブジェクトセット
            NowPlaySound.Sound_Source.AudioSource.spatialBlend = 1.0f; // 3Dの音響に切り替え
        }
        if (ReverbPreset != AudioReverbPreset.Off)
        {
            // サウンドフィルターの追加
            AudioReverbFilter Filter =
                NowPlaySound.Sound_Source.AudioSource.gameObject.AddComponent<AudioReverbFilter>();
            Filter.reverbPreset = ReverbPreset;
        }

        NowPlaySound.Sound_Source.AudioSource.clip = NowPlaySound.Sound_Clip.AudioClip; // オーディオクリップセット
        NowPlaySound.Sound_Source.AudioSource.time = PlayTime;  // 再生時間セット
        NowPlaySound.Sound_Source.AudioSource.Play();
        NowPlaySound.Sound_Source.AudioSource.loop = Loop;
        NowPlaySound.Sound_Source.StartVolume = Volume;
        NowPlaySound.Sound_Source.Volume = 1.0f;
        NowPlaySound.Sound_Source.isPause = false;
        NowPlaySound.Sound_Source.isUse = true;

        UpdateVolume(); // 音量更新
    }


    // サウンドソース情報の終了処理
    private void EndSoundSource(NOW_PLAY_SOUND NowPlaySound)
    {
        AudioReverbFilter Filter = null;

        NowPlaySound.Sound_Source.AudioSource.gameObject.transform.position = Vector3.zero;  // 座標変更
        NowPlaySound.Sound_Source.SoundObject = null;    // オブジェクトnull
        NowPlaySound.Sound_Source.AudioSource.spatialBlend = 0.0f;   // 2Dの音響に切り替え
        if ((Filter = NowPlaySound.Sound_Source.AudioSource.gameObject.GetComponent<AudioReverbFilter>()) != null)
        {
            // サウンドフィルターの破棄
            Destroy(Filter);
        }

        NowPlaySound.Sound_Source.AudioSource.Stop();        // 再生停止
        NowPlaySound.Sound_Source.AudioSource.clip = null;   // クリップにnull入れる
        NowPlaySound.Sound_Source.AudioSource.time = 0.0f;   // 再生時間リセット
        NowPlaySound.Sound_Source.StartVolume = 1.0f;        // 開始時の音量リセット
        NowPlaySound.Sound_Source.Volume = 1.0f;             // 使用中の音量リセット
        NowPlaySound.Sound_Source.isPause = false;           // ポーズ中フラグオフ
        NowPlaySound.Sound_Source.PauseTime = 0.0f;          // ポーズ中の時間リセット
        NowPlaySound.Sound_Source.isUse = false;             // 使用フラグオフ

        NowPlaySoundList.Remove(NowPlaySound);               // 再生中リストから消す
    }


    // 再生中判定処理
    private void UpdateNowUse()
    {
        for (int i = 0; i < NowPlaySoundList.Count; i++)
        {
            if (!NowPlaySoundList[i].Sound_Source.isPause && !NowPlaySoundList[i].Sound_Source.AudioSource.isPlaying)
            {
                EndSoundSource(NowPlaySoundList[i]);
            }
        }
    }


    // 3Dサウンドの音発生座標更新処理
    private void Update3DPos()
    {
        for (int i = 0; i < NowPlaySoundList.Count; i++)
        {
            if (NowPlaySoundList[i].Sound_Source.SoundObject != null)
            {
                // 座標更新
                NowPlaySoundList[i].Sound_Source.AudioSource.gameObject.transform.position =
                    NowPlaySoundList[i].Sound_Source.SoundObject.transform.position;
            }
        }
    }


    public void Update()
    {
        // テスト用入力キー

        if (Input.GetKeyDown(KeyCode.A)) // 一時停止
        {
            PauseSoundALL();
        }

        if (Input.GetKeyDown(KeyCode.S)) // 一時停止解除
        {
            UnPauseSoundALL();
        }

        if (Input.GetKeyDown(KeyCode.D)) // 停止
        {
            StopSoundALL();
        }

        if (Input.GetKeyDown(KeyCode.F)) // 効果音再生
        {
            SoundManager.Instance.PlaySound("決定音");
        }

        if (Input.GetKeyDown(KeyCode.G)) // BGM再生
        {
            SoundManager.Instance.PlaySound("TestBGM2", 0.2f, 2.0f);
        }

        if (Input.GetKeyDown(KeyCode.H)) // BGM再生(3D)
        {
            SoundManager.Instance.PlaySound("TestBGM2", 0.2f, 2.0f, this.gameObject);
        }

        if (Input.GetKeyDown(KeyCode.J)) // BGM再生(フィルターあり, お風呂場みたいなの)
        {
            SoundManager.Instance.PlaySound("TestBGM2", 0.2f, 2.0f, AudioReverbPreset.Bathroom);
        }

        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    SoundManager.Instance.SetVolume("TestBGM2", 0.2f);
        //}

        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    AudioSource_BGM[0].AudioSource.
        //}
    }

    public void FixedUpdate()
    {
        // 音量セット
        UpdateVolume();

        // 現在使用中か判定処理
        UpdateNowUse();

        // 3Dの音発生座標更新
        Update3DPos();

        // サウンドのフェード更新
        UpdateSoundFade();

        //Debug.Log("ポーズ中：" + AudioSource_BGM[0].isPause);
        //Debug.Log("使っている：" + AudioSource_BGM[0].isUse);
        //Debug.Log("ポーズ中：" + AudioSource_SE[0].isPause);
        //Debug.Log("使っている：" + AudioSource_SE[0].isUse);
    }
}
