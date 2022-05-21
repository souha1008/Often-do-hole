using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System;

//ゲームランク
[Serializable]
public enum GAME_RANK
{
    S,
    A,
    B,
    NONE,
}


// データファイル
[Serializable]
public class DataFile
{
    public DataFile()
    {
        Stage = new StageData[15];  // 仮で15ステージ分生成

        for (int i = 0; i < 15; i ++)
        {
            Stage[i] = new StageData();
        }

        SoundVolumeMaster = 100 * 0.7f;
        SoundVolumeBGM = 100 * 0.7f;
        SoundVolumeSE = 100 * 0.7f;
        SoundVolumeOBJECT = 100 * 0.7f;

        VibrationFlag = true;
    }

    public StageData[] Stage;   // ステージ分の配列
    public float SoundVolumeMaster, SoundVolumeBGM, SoundVolumeSE, SoundVolumeOBJECT;   // サウンドボリューム
    public bool VibrationFlag;  // 振動
}

[Serializable]
public class StageData
{
    public StageData()
    {
        Rank = GAME_RANK.NONE;
        Time = 1000.0f;
        coin = null;
        Clear = false;
    }
    public GAME_RANK Rank;      // ランク
    public float Time;          // 時間
    public Coin coin;           // コイン
    public bool Clear;          // ステージクリアフラグ
}


public class SaveDataManager : SingletonMonoBehaviour<SaveDataManager>
{
    // メインセーブデータ
    public DataFile MainData = null;

    // パス
    static private string Path;


    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(this.gameObject); // シーンが変わっても死なない

        Path = Application.persistentDataPath;
        Path = Path + "/Data";
        MainData = null;

        LoadData();
        //Debug.LogWarning("セーブデータ読み込み官僚");
        //Debug.LogWarning(MainData.Stage[0].Rank);
        //Debug.LogWarning(MainData.Stage[1].coin.AllCoin1);
        //Debug.LogWarning(MainData.Stage[0].NowGetCoin1);
    }

    public void Start()
    {
        //Debug.Log(MainData.Stage[0].Rank);
        //Debug.Log(MainData.Stage[0].Time);
        //Debug.Log(MainData.Stage[1].Rank);
        //Debug.Log(MainData.Stage[1].Time);
        //Debug.Log(MainData.Stage[2].Time);
        //Debug.Log(MainData.Stage[2].Time);
    }


    public void SaveData()
    {
        DataFile Data;

        // セーブデータ作成
        if (MainData != null)
            Data = MainData;
        else
            Data = CreateDataFile();

        // バイナリ形式でシリアル化
        BinaryFormatter bf = new BinaryFormatter();

        // 指定したパスにファイルを作成
        FileStream file = File.Create(Path);

        // Closeが確実に呼ばれるように例外処理を用いる
        try
        {
            // 指定したオブジェクトを上で作成したストリームにシリアル化する
            bf.Serialize(file, Data);
        }
        finally
        {
            // ファイル操作には明示的な破棄が必要。Closeを忘れないようにする。
            if (file != null)
                file.Close();
        }
    }


    public void LoadData()
    {
        if (File.Exists(Path))
        {
            // バイナリ形式でデシリアライズ
            BinaryFormatter bf = new BinaryFormatter();

            // 指定したパスのファイルストリームを開く
            FileStream file = File.Open(Path, FileMode.Open);

            try
            {
                // 指定したファイルストリームをオブジェクトにデシリアライズ
                DataFile Data = (DataFile)bf.Deserialize(file);

                // 読み込んだデータを反映させる処理
                MainData = Data;
            }
            finally
            {
                // ファイル操作には明示的な破棄が必要。Closeを忘れないようにする。
                if (file != null)
                    file.Close();

                Debug.Log("セーブデータ読み込み完了");
            }
        }
        else
        {
            Debug.LogWarning("セーブデータがありません\n 新しいセーブデータを作成します");
            MainData = CreateDataFile();

            SaveData();
        }
    }

    // 空セーブデータを作成
    private DataFile CreateDataFile()
    {
        DataFile Data = new DataFile();

        // ステージコインデータ生成
        Data.Stage[0].coin = new Coin(3, 3, 3);
        Data.Stage[1].coin = new Coin(3, 3, 3);
        Data.Stage[2].coin = new Coin(3, 3, 3);
        Data.Stage[3].coin = new Coin(3, 3, 3);
        Data.Stage[4].coin = new Coin(3, 3, 3);
        Data.Stage[5].coin = new Coin(3, 3, 3);
        Data.Stage[6].coin = new Coin(3, 3, 3);
        Data.Stage[7].coin = new Coin(3, 3, 3);
        Data.Stage[8].coin = new Coin(3, 3, 3);
        Data.Stage[9].coin = new Coin(3, 3, 3);
        Data.Stage[10].coin = new Coin(3, 3, 3);


        //Debug.LogWarning(Data.Stage[0]);
        //Data.Stage[0].Rank = 2;
        //Data.Stage[0].Time = 2;
        //Data.Stage[1].Rank = 3;
        //Data.Stage[1].Time = 3;
        //Data.Stage[1].coin.AllCoin1 = 3;

        return Data;
    }


    public StageData GetStageData(int StageNum)
    {
        return MainData.Stage[StageNum];
    }
}
